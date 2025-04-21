// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelLayer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class LevelLayer : ILevelLayerTreeNode
    {
      private readonly LevelMap _map;
      private readonly List<LayerRowDefinition> _layerRowDefinitions = new List<LayerRowDefinition>();
      private readonly List<LevelLayerShadow> _shadows = new List<LevelLayerShadow>();

      public Level Level => this._map.Level;

      public LevelMap Map => this._map;

      public int Index { get; set; }

      public string Name { get; set; }

      public bool Editing { get; set; }

      public bool Visible { get; set; }

      public IList<ILevelLayerTreeNode> Children => (IList<ILevelLayerTreeNode>) null;

      public IList<LayerRowDefinition> LayerRowDefinitions
      {
        get => (IList<LayerRowDefinition>) this._layerRowDefinitions;
      }

      public int Columns { get; private set; }

      public int Rows { get; private set; }

      public int[,] Tiles { get; private set; }

      public int OffsetY { get; set; }

      public double ParallaxY { get; set; }

      public bool AutomaticYParallax { get; set; }

      public bool WrapX { get; set; }

      public bool WrapY { get; set; }

      public IList<LevelLayerShadow> Shadows => (IList<LevelLayerShadow>) this._shadows;

      public LevelLayerLighting Lighting { get; set; }

      public IEnumerable<Rectanglei> WaterfallEffects { get; set; }

      public Colour MiniMapColour { get; set; }

      public LevelLayer(LevelMap map)
      {
        this._map = map;
        this.Visible = true;
        this.Columns = 0;
        this.Rows = 0;
        this.Tiles = new int[0, 0];
        this.Name = string.Empty;
        this.ParallaxY = 1.0;
        this.Lighting = new LevelLayerLighting();
      }

      public override string ToString()
      {
        return !string.IsNullOrEmpty(this.Name) ? this.Name : base.ToString();
      }

      public void Resize(int columns, int rows)
      {
        int num1 = Math.Min(rows, this.Rows);
        int num2 = Math.Min(columns, this.Columns);
        int[,] numArray = new int[columns, rows];
        for (int index1 = 0; index1 < num1; ++index1)
        {
          for (int index2 = 0; index2 < num2; ++index2)
            numArray[index2, index1] = this.Tiles[index2, index1];
        }
        this.Columns = columns;
        this.Rows = rows;
        this.Tiles = numArray;
      }

      public void Update()
      {
      }

      public void Animate()
      {
        foreach (LayerRowDefinition layerRowDefinition in this._layerRowDefinitions)
          layerRowDefinition.Animate();
      }

      public void Draw(
        Renderer renderer,
        Viewport viewport,
        LayerViewOptions viewOptions,
        LevelLayerShadow shadow)
      {
        if (!this.Visible)
          return;
        if (this.Editing)
          this.DrawNonLayerTiles(renderer, viewport, viewOptions);
        using (viewport.ApplyRendererState(renderer))
        {
          if (viewOptions.ShowObjects)
            this.Level.ObjectManager.Draw(renderer, viewport, this, viewOptions, false);
          if (viewOptions.ShowLandscape)
          {
            Matrix4 matrix4 = Matrix4.Identity;
            if (viewOptions.Shadows)
            {
              Vector2i displacement = shadow.Displacement;
              double x = (double) displacement.X * viewport.Scale.X;
              displacement = shadow.Displacement;
              double y = (double) displacement.Y * viewport.Scale.Y;
              matrix4 = Matrix4.CreateTranslation((Vector2) (Vector2i) new Vector2(x, y));
            }
            ITileRenderer tileRenderer = renderer.GetTileRenderer();
            tileRenderer.ClipRectangle = (Rectangle) viewport.Destination;
            tileRenderer.ModelMatrix = matrix4;
            tileRenderer.Textures = ((IEnumerable<ITexture>) this._map.Level.TileSet.Textures).ToArray<ITexture>();
            tileRenderer.Filter = viewOptions.Filter;
            tileRenderer.FilterAmount = viewOptions.FilterAmount;
            tileRenderer.BeginRender();
            this.DrawTilesVertical(renderer, viewport, viewOptions);
            tileRenderer.EndRender();
          }
          if (viewOptions.ShowObjects)
            this.Level.ObjectManager.Draw(renderer, viewport, this, viewOptions, true);
          renderer.DeativateRenderer();
        }
      }

      public void DrawNonLayerTiles(Renderer renderer, Viewport viewport, LayerViewOptions viewOptions)
      {
        int num1 = Math.Max(0, this.Columns * 64 /*0x40*/ - viewport.Bounds.X);
        int num2 = Math.Max(0, this.Rows * 64 /*0x40*/ - viewport.Bounds.Y);
        int val2 = (int) ((double) num1 * viewport.Scale.X);
        int num3 = (int) ((double) num2 * viewport.Scale.Y);
        List<Rectanglei> rects = new List<Rectanglei>(2);
        Rectanglei destination;
        if (val2 < viewport.Destination.Width)
        {
          Rectanglei rectanglei;
          ref Rectanglei local = ref rectanglei;
          int x = val2;
          int width = viewport.Destination.Width - val2;
          destination = viewport.Destination;
          int height = destination.Height;
          local = new Rectanglei(x, 0, width, height);
          rects.Add(rectanglei);
        }
        int num4 = num3;
        destination = viewport.Destination;
        int height1 = destination.Height;
        if (num4 < height1 && val2 > 0)
        {
          Rectanglei rectanglei;
          ref Rectanglei local = ref rectanglei;
          int y = num3;
          destination = viewport.Destination;
          int width = Math.Min(destination.Width, val2);
          destination = viewport.Destination;
          int height2 = destination.Height - num3;
          local = new Rectanglei(0, y, width, height2);
          rects.Add(rectanglei);
        }
        if (rects.Count <= 0)
          return;
        renderer.GetNonLayerRenderer().Render((IEnumerable<Rectanglei>) rects);
      }

      private void DrawTilesVertical(
        Renderer renderer,
        Viewport viewport,
        LayerViewOptions viewOptions)
      {
        if (this.Rows == 0 || this.Columns == 0)
          return;
        bool flag = !this.Editing && this.WrapY;
        int offsetY = this.Editing ? 0 : this.OffsetY;
        double num1 = this.Editing ? 1.0 : this.ParallaxY;
        double num2 = (double) (this.Rows * 64 /*0x40*/);
        Vector2 scale = viewport.Scale;
        double y1 = scale.Y;
        int num3 = (int) (num2 * y1);
        Rectanglei rectanglei = viewport.Bounds;
        double y2 = (double) rectanglei.Y;
        scale = viewport.Scale;
        double y3 = scale.Y;
        int num4 = (int) (y2 * y3 * num1);
        if (!flag && num4 >= num3)
          return;
        int sourceY = num4 % num3;
        rectanglei = viewport.Destination;
        int destinationY = rectanglei.Y + offsetY;
        while (true)
        {
          do
          {
            int num5 = destinationY;
            rectanglei = viewport.Destination;
            int bottom = rectanglei.Bottom;
            if (num5 < bottom)
            {
              int val1 = num3 - sourceY;
              rectanglei = viewport.Destination;
              int val2 = rectanglei.Bottom - destinationY;
              int height = Math.Min(val1, val2);
              this.DrawTilesHorizontal(renderer, viewport, viewOptions, sourceY, destinationY, ref height);
              if (height > 0)
              {
                sourceY += height;
                destinationY += height;
                if (flag)
                  goto label_7;
              }
              else
                goto label_3;
            }
            else
              goto label_12;
          }
          while (sourceY <= num3);
          goto label_11;
    label_7:
          sourceY %= num3;
        }
    label_3:
        return;
    label_11:
        return;
    label_12:;
      }

      private void DrawTilesHorizontal(
        Renderer renderer,
        Viewport viewport,
        LayerViewOptions viewOptions,
        int sourceY,
        int destinationY,
        ref int height)
      {
        int width = (int) (64.0 * viewport.Scale.X);
        int num1 = this.Columns * width;
        int num2 = this.Rows * width;
        bool flag = !this.Editing && this.WrapX;
        double num3 = this.Editing ? 1.0 : this.ParallaxY;
        int num4 = 0;
        int top;
        LayerRowDefinition rowDefinitionAt = this.GetRowDefinitionAt((int) ((double) sourceY / viewport.Scale.Y), out top);
        if (rowDefinitionAt != null && !this.Editing)
        {
          num3 = rowDefinitionAt.Parallax;
          num4 = (int) rowDefinitionAt.CurrentOffset;
          if (rowDefinitionAt.Width != 0)
            num1 = (int) ((double) rowDefinitionAt.Width * viewport.Scale.X);
          top = (int) ((double) top * viewport.Scale.Y);
          height = Math.Min(height, top + (int) ((double) rowDefinitionAt.Height * viewport.Scale.Y) - sourceY);
          height = Math.Min(height, 64 /*0x40*/ - sourceY % 64 /*0x40*/);
          if (height <= 0)
          {
            height = 1;
            return;
          }
        }
        if (height > width)
          height = width - sourceY % width;
        if (destinationY + height < viewport.Destination.Top)
          return;
        int num5 = (int) ((double) viewport.Bounds.X * viewport.Scale.X * num3) - num4;
        if (flag)
        {
          while (num5 < 0)
            num5 += num1;
        }
        if (!flag && num5 > num1)
          return;
        if (flag)
          num5 %= num1;
        int num6 = num5;
        int x = viewport.Destination.X;
        while (x < viewport.Destination.Right)
        {
          int index = 0;
          if (num6 >= 0 && sourceY >= 0 && num6 < num1 && sourceY < num2)
            index = this.Tiles[num6 / width, sourceY / width];
          int num7 = -(num6 % width);
          int num8 = width - num6 % width;
          if (num8 == 0)
            break;
          Rectanglei source = new Rectanglei(0, (int) ((double) (sourceY % width) / viewport.Scale.Y), 64 /*0x40*/, (int) ((double) height / viewport.Scale.Y));
          Rectanglei destination = new Rectanglei(x + num7, destinationY, width, height);
          this.Level.TileSet.DrawTile(renderer, index, source, destination);
          num6 += num8;
          x += num8;
          if (flag)
            num6 %= num1;
          else if (num6 > num1)
            break;
        }
      }

      public LayerRowDefinition GetRowDefinitionAt(int y, out int top)
      {
        int num = 0;
        foreach (LayerRowDefinition layerRowDefinition in this._layerRowDefinitions)
        {
          if (y >= num && y < num + layerRowDefinition.Height)
          {
            top = num;
            return layerRowDefinition;
          }
          num += layerRowDefinition.Height;
        }
        top = 0;
        return (LayerRowDefinition) null;
      }

      public void Merge(LevelLayer layer)
      {
        int length1 = Math.Max(this.Columns, layer.Columns);
        int length2 = Math.Max(this.Rows, layer.Rows);
        int[,] numArray = new int[length1, length2];
        for (int index1 = 0; index1 < length2; ++index1)
        {
          for (int index2 = 0; index2 < length1; ++index2)
          {
            if (this.Columns > index2 && this.Rows > index1)
              numArray[index2, index1] = this.Tiles[index2, index1];
            if (layer.Columns > index2 && layer.Rows > index1 && layer.Tiles[index2, index1] != 0)
              numArray[index2, index1] = layer.Tiles[index2, index1];
          }
        }
        this.Columns = length1;
        this.Rows = length2;
        this.Tiles = numArray;
      }
    }
}
