// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.SpecialStage.HalfPipeSpecialStageScreen
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Input;
using SonicOrca.Menu;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core.SpecialStage
{

    internal class HalfPipeSpecialStageScreen : Screen
    {
      private const string MusicResourceKey = "SONICORCA/MUSIC/SPECIALSTAGE/S2";
      private readonly SonicOrcaGameContext _gameContext;
      private readonly IGraphicsContext _graphicsContext;
      private readonly ResourceSession _resourceSession;
      private Font _font;
      private ITexture _backgroundTexture;
      private ITexture _floorTexture;
      private ITexture _ballTexture;
      private SampleInfo _musicSampleInfo;
      private Matrix4 _projectionMatrix;
      private Matrix4 _modelViewMatrix;
      private VertexBuffer _vertexBuffer;
      private ManagedShaderProgram _pipeShader;
      private ManagedShaderProgram _objectShader;
      private SampleInstance _musicInstance;
      private double _fov = 60.0;
      private int _currentTrackNode;
      private HalfPipeSpecialStageScreen.SegmentGeometry[] _layout = new HalfPipeSpecialStageScreen.SegmentGeometry[92]
      {
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Rise,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Drop,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnLeft,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnLeft,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnLeft,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Straight,
        HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight,
        HalfPipeSpecialStageScreen.SegmentGeometry.Rise
      };
      private const double PipeRadius = 4.0;
      private const int NumPointsWide = 32 /*0x20*/;
      private const int PointsPerSegment = 28;
      private const int PointLength = 2;
      private List<HalfPipeSpecialStageScreen.TrackNode> _trackNodes = new List<HalfPipeSpecialStageScreen.TrackNode>();
      private int _ticks;
      private Vector3 _camera;
      private Vector3 _cameraTarget;

      public HalfPipeSpecialStageScreen(SonicOrcaGameContext gameContext)
      {
        this._gameContext = gameContext;
        this._graphicsContext = this._gameContext.Window.GraphicsContext;
        this._resourceSession = new ResourceSession(gameContext.ResourceTree);
      }

      public override async Task LoadAsync(ScreenLoadingProgress progress, CancellationToken ct = default (CancellationToken))
      {
        this._resourceSession.PushDependency("SONICORCA/FONTS/HUD");
        this._resourceSession.PushDependency("SONICORCA/SPECIALSTAGE/HALFPIPE/BACKGROUND");
        this._resourceSession.PushDependency("SONICORCA/SPECIALSTAGE/HALFPIPE/FLOOR");
        this._resourceSession.PushDependency("SONICORCA/SPECIALSTAGE/HALFPIPE/BALL");
        this._resourceSession.PushDependency("SONICORCA/MUSIC/SPECIALSTAGE/S2");
        await this._resourceSession.LoadAsync(ct);
        this._font = this._gameContext.ResourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/HUD");
        this._backgroundTexture = this._gameContext.ResourceTree.GetLoadedResource<ITexture>("SONICORCA/SPECIALSTAGE/HALFPIPE/BACKGROUND");
        this._floorTexture = this._gameContext.ResourceTree.GetLoadedResource<ITexture>("SONICORCA/SPECIALSTAGE/HALFPIPE/FLOOR");
        this._ballTexture = this._gameContext.ResourceTree.GetLoadedResource<ITexture>("SONICORCA/SPECIALSTAGE/HALFPIPE/BALL");
        this._musicSampleInfo = this._gameContext.ResourceTree.GetLoadedResource<SampleInfo>("SONICORCA/MUSIC/SPECIALSTAGE/S2");
      }

      public override void Initialise()
      {
        this._pipeShader = OrcaShader.CreateFromFile(this._graphicsContext, "shaders/halfpipe/test.shader");
        this._objectShader = OrcaShader.CreateFromFile(this._graphicsContext, "shaders/halfpipe/object.shader");
        this._vertexBuffer = this._graphicsContext.CreateVertexBuffer(3, 2);
        this._projectionMatrix = Matrix4.Perspective(MathX.ToRadians(this._fov), 16.0 / 9.0, 1.0, 1024.0);
        this.CreateTrackNodes();
      }

      private void CreateTrackNodes()
      {
        double num1 = 0.0;
        Vector3 vector3 = new Vector3();
        foreach (HalfPipeSpecialStageScreen.SegmentGeometry segmentGeometry in this._layout)
        {
          double y1 = vector3.Y;
          double radians = MathX.ToRadians(90.0);
          for (int index = 0; index < 28; ++index)
          {
            switch (segmentGeometry)
            {
              case HalfPipeSpecialStageScreen.SegmentGeometry.Drop:
                radians -= MathX.ToRadians(45.0 / 7.0);
                double num2 = Math.Cos(radians) / -2.0;
                break;
              case HalfPipeSpecialStageScreen.SegmentGeometry.Rise:
                radians += MathX.ToRadians(45.0 / 7.0);
                double num3 = Math.Cos(radians) / -2.0;
                break;
              case HalfPipeSpecialStageScreen.SegmentGeometry.TurnLeft:
                num1 -= MathX.ToRadians(45.0 / 14.0);
                break;
              case HalfPipeSpecialStageScreen.SegmentGeometry.TurnRight:
                num1 += MathX.ToRadians(45.0 / 14.0);
                break;
            }
            Vector3 position = vector3 + new Vector3(Math.Sin(num1), -Math.Cos(radians), Math.Cos(num1)) * 2.0;
            double y2 = radians - MathX.ToRadians(90.0);
            if (y2 > MathX.ToRadians(90.0))
              y2 = MathX.ToRadians(180.0) - y2;
            else if (y2 < MathX.ToRadians(-90.0))
              y2 = MathX.ToRadians(-180.0) - y2;
            this._trackNodes.Add(new HalfPipeSpecialStageScreen.TrackNode(position, new Vector2(num1, y2)));
            vector3 = position;
          }
        }
      }

      public override void Update()
      {
        KeyboardState keyboard = this._gameContext.Input.Pressed.Keyboard;
        if (keyboard[46])
          ++this._fov;
        else if (keyboard[45])
          --this._fov;
        if (this._ticks == 0)
        {
          this._musicInstance = new SampleInstance(this._gameContext, this._musicSampleInfo);
          this._musicInstance.Play();
        }
        if (this._ticks % 2 == 0)
          this._currentTrackNode = (this._currentTrackNode + 1) % this._trackNodes.Count;
        this.UpdateCamera();
        ++this._ticks;
      }

      private void UpdateCamera()
      {
        int index = Math.Min(this._trackNodes.Count - 1, this._currentTrackNode + 1);
        double num = (double) (this._ticks % 2) / 2.0;
        HalfPipeSpecialStageScreen.TrackNode trackNode1 = this._trackNodes[this._currentTrackNode];
        HalfPipeSpecialStageScreen.TrackNode trackNode2 = this._trackNodes[index];
        Vector3 vector3_1 = trackNode1.Position + new Vector3(0.0, -2.0, 0.0);
        Vector2 angle1 = trackNode1.Angle;
        Vector3 vector3_2 = trackNode2.Position + new Vector3(0.0, -2.0, 0.0);
        Vector2 angle2 = trackNode2.Angle;
        Vector3 vector3_3 = vector3_1 + (vector3_2 - vector3_1) * num;
        Vector2 vector2 = new Vector2(angle1.X + (angle2.X - angle1.X) * num, angle1.Y + (angle2.Y - angle1.Y) * num);
        Vector3 vector3_4 = new Vector3(Math.Sin(vector2.X), Math.Sin(vector2.Y), Math.Cos(vector2.X));
        this._camera = vector3_3;
        this._cameraTarget = this._camera + vector3_4;
      }

      public override void Draw(Renderer renderer)
      {
        this.DrawBackground(renderer);
        this.DrawPipe(renderer);
        this.DrawHud(renderer);
      }

      private void DrawBackground(Renderer renderer)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.BlendMode = BlendMode.Alpha;
        obj.Colour = Colours.White;
        obj.ClipRectangle = new Rectangle(0.0, 0.0, 1920.0, 1080.0);
        obj.RenderTexture(this._backgroundTexture, new Rectangle(0.0, 0.0, 1920.0, 1080.0));
        obj.Deactivate();
      }

      private void DrawPipe(Renderer renderer)
      {
        this._modelViewMatrix = Matrix4.LookAt(this._camera.X, this._camera.Y, -this._camera.Z, this._cameraTarget.X, this._cameraTarget.Y, -this._cameraTarget.Z, 0.0, 1.0, 0.0);
        this._modelViewMatrix *= Matrix4.CreateScale(1.0, 1.0, -1.0);
        this._graphicsContext.ClearDepthBuffer();
        this._graphicsContext.DepthTesting = true;
        List<HalfPipeSpecialStageScreen.TexturedQuad> source = new List<HalfPipeSpecialStageScreen.TexturedQuad>();
        int num1 = (int) ((double) this._floorTexture.Height * (8.0 / (double) this._floorTexture.Width) / 2.0);
        Vector3[] vector3Array = (Vector3[]) null;
        for (int index1 = 0; index1 < this._trackNodes.Count; ++index1)
        {
          if (index1 >= this._currentTrackNode - 128 /*0x80*/)
          {
            if (index1 <= this._currentTrackNode + 128 /*0x80*/)
            {
              HalfPipeSpecialStageScreen.TrackNode trackNode = this._trackNodes[index1];
              Matrix4 transform = Matrix4.Identity.RotateY(trackNode.Angle.X).Translate(trackNode.Position);
              Vector3[] array = ((IEnumerable<Vector3>) this.GetCrossSection()).Select<Vector3, Vector3>((Func<Vector3, Vector3>) (x => transform * x)).ToArray<Vector3>();
              if (vector3Array != null)
              {
                for (int index2 = 0; index2 < array.Length - 1; ++index2)
                {
                  int num2 = index1 % num1;
                  source.Add(new HalfPipeSpecialStageScreen.TexturedQuad(new Vector3[4]
                  {
                    array[index2],
                    vector3Array[index2],
                    vector3Array[index2 + 1],
                    array[index2 + 1]
                  }, new Vector2[4]
                  {
                    new Vector2((double) index2 / (double) (array.Length - 1), 1.0 - (double) (num2 + 1) / (double) num1),
                    new Vector2((double) index2 / (double) (array.Length - 1), 1.0 - (double) num2 / (double) num1),
                    new Vector2((double) (index2 + 1) / (double) (array.Length - 1), 1.0 - (double) num2 / (double) num1),
                    new Vector2((double) (index2 + 1) / (double) (array.Length - 1), 1.0 - (double) (num2 + 1) / (double) num1)
                  }, this._floorTexture));
                }
              }
              vector3Array = array;
              if ((index1 + num1 / 2 + 1) % num1 == 0)
              {
                double num3 = 0.5;
                double num4 = 3.0;
                for (int index3 = 0; index3 < 7; ++index3)
                {
                  double num5 = (double) index3 * (MathX.ToRadians(180.0) / 6.0);
                  Vector3 vector3 = new Vector3(-Math.Cos(num5) * num4, Math.Sin(num5) * num4, 0.0);
                  source.Add(new HalfPipeSpecialStageScreen.TexturedQuad(((IEnumerable<Vector3>) new Vector3[4]
                  {
                    new Vector3(vector3.X - num3, vector3.Y - num3, 0.0),
                    new Vector3(vector3.X - num3, vector3.Y + num3, 0.0),
                    new Vector3(vector3.X + num3, vector3.Y + num3, 0.0),
                    new Vector3(vector3.X + num3, vector3.Y - num3, 0.0)
                  }).Select<Vector3, Vector3>((Func<Vector3, Vector3>) (x => transform * x)).ToArray<Vector3>(), new Vector2[4]
                  {
                    new Vector2(0.0, 0.0),
                    new Vector2(0.0, 1.0),
                    new Vector2(1.0, 1.0),
                    new Vector2(1.0, 0.0)
                  }, this._ballTexture));
                }
              }
            }
            else
              break;
          }
        }
        List<Tuple<int, ITexture>> tupleList = new List<Tuple<int, ITexture>>();
        int num6 = 0;
        ITexture texture = (ITexture) null;
        this._vertexBuffer.Begin();
        foreach (HalfPipeSpecialStageScreen.TexturedQuad texturedQuad in (IEnumerable<HalfPipeSpecialStageScreen.TexturedQuad>) source.OrderByDescending<HalfPipeSpecialStageScreen.TexturedQuad, double>((Func<HalfPipeSpecialStageScreen.TexturedQuad, double>) (x => (this._camera - x.Position[0]).Length)))
        {
          for (int index = 0; index < 4; ++index)
          {
            this._vertexBuffer.AddValue(0, texturedQuad.Position[index]);
            this._vertexBuffer.AddValue(1, texturedQuad.TextureMapping[index]);
          }
          if (num6 == 0)
            texture = texturedQuad.Texture;
          else if (texture != texturedQuad.Texture)
          {
            tupleList.Add(new Tuple<int, ITexture>(num6, texture));
            num6 = 0;
            texture = texturedQuad.Texture;
          }
          ++num6;
        }
        if (num6 > 0)
          tupleList.Add(new Tuple<int, ITexture>(num6, texture));
        this._vertexBuffer.End();
        IShaderProgram program = this._pipeShader.Program;
        program.Activate();
        program.SetUniform("ProjectionMatrix", this._projectionMatrix);
        program.SetUniform("ModelViewMatrix", this._modelViewMatrix);
        program.SetUniform("InputTexture", 0);
        program.SetUniform("InputLightSource", this._trackNodes[this._currentTrackNode].Position + new Vector3(0.0, 0.0, 0.0));
        int num7 = 0;
        foreach (Tuple<int, ITexture> tuple in tupleList)
        {
          this._graphicsContext.SetTexture(tuple.Item2);
          this._vertexBuffer.Render(PrimitiveType.Quads, num7 * 4, tuple.Item1 * 4);
          num7 += tuple.Item1;
        }
        this._graphicsContext.DepthTesting = false;
      }

      private Vector3[] GetCrossSection()
      {
        Vector3[] crossSection = new Vector3[32 /*0x20*/];
        for (int index = 0; index < crossSection.Length; ++index)
        {
          double num = (double) index / 31.0 * Math.PI;
          crossSection[index] = new Vector3(-Math.Cos(num) * 4.0, -Math.Sin(num) * 4.0, 0.0);
        }
        return crossSection;
      }

      private void DrawHud(Renderer renderer)
      {
        renderer.GetFontRenderer().RenderString($"SEGMENT: {this._currentTrackNode} / {this._trackNodes.Count}", new Rectangle(), FontAlignment.Left, this._font, 0);
      }

      private enum SegmentGeometry
      {
        Straight,
        Drop,
        Rise,
        TurnLeft,
        TurnRight,
      }

      private struct TrackNode
      {
        public Vector3 Position { get; set; }

        public Vector2 Angle { get; set; }

        public TrackNode(Vector3 position, Vector2 angle)
          : this()
        {
          this.Position = position;
          this.Angle = angle;
        }

        public override string ToString()
        {
                return string.Format("{0}, {1:0} DEG, {2:0} DEG", this.Position, MathX.ToDegrees(this.Angle.X), MathX.ToDegrees(this.Angle.Y));
        }
      }

      private struct TexturedQuad
      {
        public Vector3[] Position { get; set; }

        public Vector2[] TextureMapping { get; set; }

        public ITexture Texture { get; set; }

        public TexturedQuad(Vector3[] position, Vector2[] textureMapping, ITexture texture)
          : this()
        {
          this.Position = position;
          this.TextureMapping = textureMapping;
          this.Texture = texture;
        }
      }
    }
}
