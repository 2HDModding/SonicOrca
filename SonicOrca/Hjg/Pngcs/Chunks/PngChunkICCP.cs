// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkICCP
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkICCP(ImageInfo info) : PngChunkSingle("iCCP", info)
    {
      public const string ID = "iCCP";
      private string profileName;
      private byte[] compressedProfile;

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(this.profileName.Length + this.compressedProfile.Length + 2, true);
        Array.Copy((Array) ChunkHelper.ToBytes(this.profileName), 0, (Array) emptyChunk.Data, 0, this.profileName.Length);
        emptyChunk.Data[this.profileName.Length] = (byte) 0;
        emptyChunk.Data[this.profileName.Length + 1] = (byte) 0;
        Array.Copy((Array) this.compressedProfile, 0, (Array) emptyChunk.Data, this.profileName.Length + 2, this.compressedProfile.Length);
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw chunk)
      {
        int count = ChunkHelper.PosNullByte(chunk.Data);
        this.profileName = PngHelperInternal.charsetLatin1.GetString(chunk.Data, 0, count);
        if (((int) chunk.Data[count + 1] & (int) byte.MaxValue) != 0)
          throw new Exception("bad compression for ChunkTypeICCP");
        int length = chunk.Data.Length - (count + 2);
        this.compressedProfile = new byte[length];
        Array.Copy((Array) chunk.Data, count + 2, (Array) this.compressedProfile, 0, length);
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkICCP pngChunkIccp = (PngChunkICCP) other;
        this.profileName = pngChunkIccp.profileName;
        this.compressedProfile = new byte[pngChunkIccp.compressedProfile.Length];
        Array.Copy((Array) pngChunkIccp.compressedProfile, (Array) this.compressedProfile, this.compressedProfile.Length);
      }

      public void SetProfileNameAndContent(string name, string profile)
      {
        this.SetProfileNameAndContent(name, ChunkHelper.ToBytes(this.profileName));
      }

      public void SetProfileNameAndContent(string name, byte[] profile)
      {
        this.profileName = name;
        this.compressedProfile = ChunkHelper.compressBytes(profile, true);
      }

      public string GetProfileName() => this.profileName;

      public byte[] GetProfile() => ChunkHelper.compressBytes(this.compressedProfile, false);

      public string GetProfileAsString() => ChunkHelper.ToString(this.GetProfile());
    }
}
