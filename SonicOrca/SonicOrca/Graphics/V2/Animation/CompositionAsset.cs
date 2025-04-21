// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionAsset
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionAsset
    {
      private string _id;
      private int _width;
      private int _height;
      private string _path;
      private string _fileName;

      public string ID => this._id;

      public int Width => this._width;

      public int Height => this._height;

      public string Path => this._path;

      public string FileName => this._fileName;

      public CompositionAsset(string id, int width, int height, string path, string fileName)
      {
        this._id = id;
        this._width = width;
        this._height = height;
        this._path = path;
        this._fileName = fileName;
      }
    }
}
