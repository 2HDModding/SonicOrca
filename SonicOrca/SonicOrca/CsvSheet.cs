// Decompiled with JetBrains decompiler
// Type: SonicOrca.CsvSheet
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SonicOrca
{

    public class CsvSheet
    {
      private List<List<string>> mRows = new List<List<string>>();
      private int mColumns;

      public CsvSheet()
      {
      }

      public CsvSheet(string filename)
      {
        using (FileStream fileStream = new FileStream(filename, FileMode.Open))
          this.Load((Stream) fileStream);
      }

      public CsvSheet(Stream stream) => this.Load(stream);

      private void Load(Stream stream)
      {
        this.mRows = new List<List<string>>();
        using (StreamReader streamReader = new StreamReader(stream))
        {
          string line;
          while ((line = streamReader.ReadLine()) != null)
            this.mRows.Add(new List<string>((IEnumerable<string>) this.ProcessLine(line)));
        }
        this.CalculateColumns();
      }

      public void Save(string filename)
      {
        using (FileStream fileStream = new FileStream(filename, FileMode.Create))
          this.Save((Stream) fileStream);
      }

      public void Save(Stream stream)
      {
        using (StreamWriter streamWriter = new StreamWriter(stream))
        {
          foreach (List<string> mRow in this.mRows)
          {
            for (int index = 0; index < mRow.Count - 1; ++index)
              streamWriter.Write("{0},", (object) this.FormatAsCSV(mRow[index]));
            if (mRow.Count > 0)
              streamWriter.Write(this.FormatAsCSV(mRow[mRow.Count - 1]));
            streamWriter.WriteLine();
          }
        }
      }

      private string FormatAsCSV(string cell)
      {
        if (cell.Contains<char>('"'))
          cell = cell.Replace("\"", "\"\"");
        if (cell.Contains(",") || cell.StartsWith("\"") || cell.EndsWith("\""))
          cell = $"\"{cell}\"";
        return cell;
      }

      private string Get(int x, int y)
      {
        if (x < 0 || y < 0)
          throw new ArgumentOutOfRangeException();
        if (y >= this.mRows.Count)
          return string.Empty;
        List<string> mRow = this.mRows[y];
        return x >= mRow.Count ? string.Empty : mRow[x];
      }

      private void Set(int x, int y, string value)
      {
        if (x < 0 || y < 0)
          throw new ArgumentOutOfRangeException();
        while (y >= this.mRows.Count)
          this.mRows.Add(new List<string>());
        List<string> mRow = this.mRows[y];
        while (x >= mRow.Count)
          mRow.Add(string.Empty);
        mRow[x] = value;
        this.CalculateColumns();
      }

      private void CalculateColumns()
      {
        this.mColumns = this.mRows.Max<List<string>>((Func<List<string>, int>) (row => row.Count));
      }

      private string[] ProcessLine(string line)
      {
        List<string> stringList = new List<string>();
        using (StringReader sr = new StringReader(line))
        {
          string nextCell;
          while ((nextCell = this.GetNextCell(sr)) != null)
            stringList.Add(nextCell);
        }
        return stringList.ToArray();
      }

      private string GetNextCell(StringReader sr)
      {
        if (sr.Peek() == -1)
          return (string) null;
        int count = 0;
        bool flag = false;
        StringBuilder stringBuilder = new StringBuilder();
        while (sr.Peek() != -1)
        {
          char c = (char) sr.Read();
          if (!char.IsWhiteSpace(c) || flag || !string.IsNullOrEmpty(stringBuilder.ToString()))
          {
            if (c != ',' || flag)
            {
              if (c == ' ' && !flag)
              {
                ++count;
              }
              else
              {
                stringBuilder.Append(new string(' ', count));
                count = 0;
                if (c == '"' && (ushort) sr.Peek() == (ushort) 34)
                {
                  sr.Read();
                  stringBuilder.Append('"');
                }
                else if (c == '"')
                  flag = !flag;
                else
                  stringBuilder.Append(c);
              }
            }
            else
              break;
          }
        }
        return stringBuilder.ToString();
      }

      public string this[int x, int y]
      {
        get => this.Get(x, y);
        set => this.Set(x, y, value);
      }

      public int Rows => this.mRows.Count;

      public int Columns => this.mColumns;
    }
}
