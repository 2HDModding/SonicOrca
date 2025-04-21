// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.Font
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics
{

    public class Font : ILoadedResource, IDisposable, IEnumerable<Font.CharacterDefinition>, IEnumerable
    {
      private readonly Dictionary<char, Font.CharacterDefinition> _characterDefinitions;
      private readonly ResourceTree _resourceTree;
      private readonly string _shapeResourceKey;
      private readonly IEnumerable<string> _overlayResourceKeys;
      private ITexture _shapeTexture;
      private IReadOnlyList<ITexture> _overlayTextures;

      public Resource Resource { get; set; }

      public int DefaultWidth { get; private set; }

      public int Height { get; private set; }

      public int Tracking { get; private set; }

      public Vector2i? DefaultShadow { get; private set; }

      public ITexture ShapeTexture => this._shapeTexture;

      public IReadOnlyList<ITexture> OverlayTextures => this._overlayTextures;

      public Font.CharacterDefinition this[char key]
      {
        get => this._characterDefinitions.GetValueOrDefault<char, Font.CharacterDefinition>(key);
      }

      public Font(
        ResourceTree resourceTree,
        string shapeResourceKey,
        IEnumerable<string> overlayResourceKeys,
        int defaultWidth,
        int height,
        int tracking,
        Vector2i? shadow,
        IEnumerable<Font.CharacterDefinition> characterDefinitions)
      {
        this._resourceTree = resourceTree;
        this._shapeResourceKey = shapeResourceKey;
        this._overlayResourceKeys = (IEnumerable<string>) overlayResourceKeys.ToArray<string>();
        this.DefaultWidth = defaultWidth;
        this.Height = height;
        this.Tracking = tracking;
        this.DefaultShadow = shadow;
        this._characterDefinitions = characterDefinitions.ToDictionary<Font.CharacterDefinition, char>((Func<Font.CharacterDefinition, char>) (x => x.Key));
      }

      public Font(
        ITexture shapeTexture,
        IEnumerable<ITexture> overlayTextures,
        int defaultWidth,
        int height,
        int tracking,
        Vector2i? shadow,
        IEnumerable<Font.CharacterDefinition> characterDefinitions)
      {
        this._shapeTexture = shapeTexture;
        this._overlayTextures = (IReadOnlyList<ITexture>) overlayTextures.ToArray<ITexture>();
        this.DefaultWidth = defaultWidth;
        this.Height = height;
        this.Tracking = tracking;
        this.DefaultShadow = shadow;
        this._characterDefinitions = characterDefinitions.ToDictionary<Font.CharacterDefinition, char>((Func<Font.CharacterDefinition, char>) (x => x.Key));
      }

      public void OnLoaded()
      {
        if (this._resourceTree == null)
          return;
        this._shapeTexture = this._resourceTree.GetLoadedResource<ITexture>(this._shapeResourceKey);
        this._overlayTextures = (IReadOnlyList<ITexture>) this._overlayResourceKeys.Select<string, ITexture>((Func<string, ITexture>) (x => this._resourceTree.GetLoadedResource<ITexture>(x))).ToArray<ITexture>();
      }

      public void Dispose()
      {
      }

      public Rectangle MeasureString(string text)
      {
        return this.MeasureString(text, new Rectangle(), FontAlignment.Left);
      }

      public Rectangle MeasureString(string text, Rectangle boundary, FontAlignment alignment)
      {
        double width = 0.0;
        double height = (double) this.Height;
        foreach (char key in text)
        {
          Font.CharacterDefinition characterDefinition;
          width = width + (this._characterDefinitions.TryGetValue(key, out characterDefinition) ? (double) characterDefinition.Width : (double) this.DefaultWidth) + (double) this.Tracking;
        }
        if (text.Length > 0)
          width -= (double) this.Tracking;
        double x = 0.0;
        double y = 0.0;
        FontAlignment fontAlignment1 = alignment & FontAlignment.HorizontalMask;
        FontAlignment fontAlignment2 = alignment & FontAlignment.VerticalMask;
        switch (fontAlignment1)
        {
          case FontAlignment.Left:
            x = boundary.X;
            break;
          case FontAlignment.MiddleX:
            x = boundary.X + (boundary.Width - width) / 2.0;
            break;
          case FontAlignment.Right:
            x = boundary.Right - width;
            break;
        }
        switch (fontAlignment2)
        {
          case FontAlignment.Left:
            y = boundary.Y;
            break;
          case FontAlignment.MiddleY:
            y = boundary.Y + (boundary.Height - height) / 2.0;
            break;
          case FontAlignment.Bottom:
            y = boundary.Bottom - height;
            break;
        }
        return new Rectangle(x, y, width, height);
      }

      public override string ToString() => $"{this._characterDefinitions.Count} defined characters";

      public IEnumerator<Font.CharacterDefinition> GetEnumerator()
      {
        return (IEnumerator<Font.CharacterDefinition>) this._characterDefinitions.Values.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      public class CharacterDefinition
      {
        private readonly char _key;
        private readonly Rectanglei _sourceRectangle;
        private readonly Vector2i _offset;
        private readonly int _width;

        public char Key => this._key;

        public Rectanglei SourceRectangle => this._sourceRectangle;

        public Vector2i Offset => this._offset;

        public int Width => this._width;

        public CharacterDefinition(char key, Rectanglei sourceRectangle, Vector2i offset)
          : this(key, sourceRectangle, offset, sourceRectangle.Width)
        {
        }

        public CharacterDefinition(char key, Rectanglei sourceRectangle, Vector2i offset, int width)
        {
          this._key = key;
          this._sourceRectangle = sourceRectangle;
          this._offset = offset;
          this._width = width;
        }

        public override string ToString() => this._key.ToString();
      }
    }
}
