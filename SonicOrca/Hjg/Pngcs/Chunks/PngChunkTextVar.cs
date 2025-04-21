// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkTextVar
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public abstract class PngChunkTextVar : PngChunkMultiple
    {
      protected internal string key;
      protected internal string val;
      public const string KEY_Title = "Title";
      public const string KEY_Author = "Author";
      public const string KEY_Description = "Description";
      public const string KEY_Copyright = "Copyright";
      public const string KEY_Creation_Time = "Creation Time";
      public const string KEY_Software = "Software";
      public const string KEY_Disclaimer = "Disclaimer";
      public const string KEY_Warning = "Warning";
      public const string KEY_Source = "Source";
      public const string KEY_Comment = "Comment";

      protected internal PngChunkTextVar(string id, ImageInfo info)
        : base(id, info)
      {
      }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NONE;
      }

      public string GetKey() => this.key;

      public string GetVal() => this.val;

      public void SetKeyVal(string key, string val)
      {
        this.key = key;
        this.val = val;
      }

      public class PngTxtInfo
      {
        public string title;
        public string author;
        public string description;
        public string creation_time;
        public string software;
        public string disclaimer;
        public string warning;
        public string source;
        public string comment;
      }
    }
}
