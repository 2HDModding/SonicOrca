// Decompiled with JetBrains decompiler
// Type: SonicOrca.Original.Nemesis
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace SonicOrca.Original
{

    public static class Nemesis
    {
      public static byte[] Decompress(byte[] input)
      {
        using (MemoryStream input1 = new MemoryStream(input))
          return Nemesis.Decompress((Stream) input1);
      }

      public static byte[] Decompress(Stream input)
      {
        using (MemoryStream output = new MemoryStream())
        {
          Nemesis.Decompress(input, (Stream) output);
          return output.ToArray();
        }
      }

      public static int Decompress(Stream input, Stream output)
      {
        byte[] numArray = new byte[4];
        MemoryStream output1 = new MemoryStream();
        Nemesis.CodeTreeNode codeTree = new Nemesis.CodeTreeNode();
        int num = (int) (byte) input.ReadByte() << 8 | (int) (byte) input.ReadByte();
        int xorOutput = (num & 32768 /*0x8000*/) == 0 ? 0 : 1;
        int numTiles = num & -32769;
        Nemesis.ReadHeader(input, (Stream) output1, codeTree);
        Nemesis.ReadInternal(input, (Stream) output1, codeTree, (ushort) numTiles, xorOutput);
        int length = (int) output1.Length;
        output1.Position = 0L;
        if (xorOutput != 0)
        {
          Array.Clear((Array) numArray, 0, numArray.Length);
          for (int index = 0; index < length; ++index)
          {
            numArray[index % 4] ^= (byte) output1.ReadByte();
            output.WriteByte(numArray[index % 4]);
          }
        }
        else
          output1.CopyTo(output);
        return length;
      }

      private static void ReadHeader(Stream input, Stream output, Nemesis.CodeTreeNode codeTree)
      {
        byte nibble = 0;
        byte num;
        while ((num = (byte) input.ReadByte()) != byte.MaxValue)
        {
          if (((int) num & 128 /*0x80*/) != 0)
          {
            nibble = (byte) ((uint) num & 15U);
            num = (byte) input.ReadByte();
          }
          Nemesis.SetCode(codeTree, (byte) input.ReadByte(), (int) num & 15, new Nemesis.NibbleRun(nibble, (byte) ((((int) num & 112 /*0x70*/) >> 4) + 1)));
        }
        Nemesis.SetCode(codeTree, (byte) 63 /*0x3F*/, 6, new Nemesis.NibbleRun((byte) 0, byte.MaxValue));
      }

      private static void ReadInternal(
        Stream input,
        Stream output,
        Nemesis.CodeTreeNode codeTree,
        ushort numTiles,
        int xorOutput)
      {
        BitReader bitReader = new BitReader(input, 1);
        BitWriter output1 = new BitWriter(output, 1);
        int num = (int) numTiles << 8;
        int bitsWritten = 0;
        Nemesis.CodeTreeNode codeTreeNode = codeTree;
        while (bitsWritten < num)
        {
          Nemesis.NibbleRun nibbleRun = codeTreeNode.NibbleRun;
          if (nibbleRun.Count == byte.MaxValue)
          {
            byte count = (byte) (bitReader.ReadBits(3) + 1);
            byte nibble = (byte) bitReader.ReadBits(4);
            Nemesis.WriteNibbleRun(output1, count, nibble, ref bitsWritten);
            codeTreeNode = codeTree;
          }
          else if (nibbleRun.Count != (byte) 0)
          {
            Nemesis.WriteNibbleRun(output1, nibbleRun.Count, nibbleRun.Nibble, ref bitsWritten);
            codeTreeNode = codeTree;
          }
          else
          {
            codeTreeNode = !bitReader.ReadBit() ? codeTreeNode.Clear : codeTreeNode.Set;
            if (codeTreeNode == null)
              throw new NemesisException("Invalid code.");
          }
        }
      }

      private static void WriteNibbleRun(
        BitWriter output,
        byte count,
        byte nibble,
        ref int bitsWritten)
      {
        bitsWritten += (int) count * 4;
        if (((int) count & 1) != 0)
          output.WriteBits((int) nibble, 4);
        count >>= 1;
        nibble |= (byte) ((uint) nibble << 4);
        while (count-- != (byte) 0)
          output.WriteBits((int) nibble, 8);
      }

      private static void SetCode(
        Nemesis.CodeTreeNode codeTree,
        byte code,
        int length,
        Nemesis.NibbleRun nibbleRun)
      {
        if (length == 0)
        {
          if (codeTree.Clear != null || codeTree.Set != null)
            throw new NemesisException("Code already used as prefix.");
          codeTree.NibbleRun = nibbleRun;
        }
        else
        {
          if (codeTree.NibbleRun.Count != (byte) 0)
            throw new NemesisException("Prefix already used as code.");
          --length;
          if (((int) code & 1 << length) == 0)
          {
            if (codeTree.Clear == null)
              codeTree.Clear = new Nemesis.CodeTreeNode();
            Nemesis.SetCode(codeTree.Clear, code, length, nibbleRun);
          }
          else
          {
            if (codeTree.Set == null)
              codeTree.Set = new Nemesis.CodeTreeNode();
            Nemesis.SetCode(codeTree.Set, (byte) ((uint) code & (uint) ((1 << length) - 1)), length, nibbleRun);
          }
        }
      }

      private struct NibbleRun
      {
        public byte Nibble { get; set; }

        public byte Count { get; set; }

        public NibbleRun(byte nibble, byte count)
          : this()
        {
          this.Nibble = nibble;
          this.Count = count;
        }

        public override string ToString() => $"{this.Nibble} x{this.Count}";
      }

      private class CodeTreeNode
      {
        public Nemesis.CodeTreeNode Clear { get; set; }

        public Nemesis.CodeTreeNode Set { get; set; }

        public Nemesis.NibbleRun NibbleRun { get; set; }

        public override string ToString() => this.Clear == null && this.Set == null ? "Leaf" : "Node";
      }
    }
}
