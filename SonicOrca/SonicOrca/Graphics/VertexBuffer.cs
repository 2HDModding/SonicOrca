// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.VertexBuffer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics
{

    public abstract class VertexBuffer : IDisposable
    {
      private readonly Dictionary<string, int> _attributeLocations = new Dictionary<string, int>();

      public abstract IReadOnlyList<int> VectorCounts { get; }

      protected IEnumerable<int> SetAttributeLocations(
        IShaderProgram shaderProgram,
        IEnumerable<string> names,
        IEnumerable<int> vectorCounts)
      {
        string[] array1 = names.ToArray<string>();
        int[] array2 = vectorCounts.ToArray<int>();
        foreach (string str in array1)
          this._attributeLocations[str] = shaderProgram.GetAttributeLocation(str);
        int[] numArray = new int[array2.Length];
        for (int index = 0; index < numArray.Length; ++index)
          numArray[this._attributeLocations[array1[index]]] = array2[index];
        return (IEnumerable<int>) numArray;
      }

      public abstract void Dispose();

      public abstract void SetBufferData(int index, IEnumerable<double> data);

      public abstract void Begin();

      public abstract void End();

      public abstract void Render(PrimitiveType type);

      public abstract void Render(PrimitiveType type, int index, int count);

      public abstract void AddValue(int index, double value);

      public void AddValue(string name, double value)
      {
        this.AddValue(this._attributeLocations[name], value);
      }

      public void AddValue(string name, Vector2 value)
      {
        this.AddValue(this._attributeLocations[name], value);
      }

      public void AddValue(string name, Vector3 value)
      {
        this.AddValue(this._attributeLocations[name], value);
      }

      public void AddValue(string name, Vector4 value)
      {
        this.AddValue(this._attributeLocations[name], value);
      }

      public void AddValue(string name, Colour value)
      {
        this.AddValue(this._attributeLocations[name], value);
      }

      public void AddValue(int index, Vector2 value)
      {
        this.AddValue(index, value.X);
        this.AddValue(index, value.Y);
      }

      public void AddValue(int index, Vector3 value)
      {
        this.AddValue(index, value.X);
        this.AddValue(index, value.Y);
        this.AddValue(index, value.Z);
      }

      public void AddValue(int index, Vector4 value)
      {
        this.AddValue(index, value.X);
        this.AddValue(index, value.Y);
        this.AddValue(index, value.Z);
        this.AddValue(index, value.W);
      }

      public void AddValue(int index, Colour value)
      {
        this.AddValue(index, (double) value.Red / (double) byte.MaxValue);
        this.AddValue(index, (double) value.Green / (double) byte.MaxValue);
        this.AddValue(index, (double) value.Blue / (double) byte.MaxValue);
        this.AddValue(index, (double) value.Alpha / (double) byte.MaxValue);
      }

      public void AddValues(params object[] values)
      {
        for (int index = 0; index < values.Length; ++index)
        {
          if (values[index] is float || values[index] is double)
            this.AddValue(index, (double) values[index]);
          else if (values[index] is Vector2)
            this.AddValue(index, (Vector2) values[index]);
          else if (values[index] is Vector3)
            this.AddValue(index, (Vector3) values[index]);
          else if (values[index] is Vector4)
            this.AddValue(index, (Vector4) values[index]);
          else if (values[index] is Colour)
            this.AddValue(index, (Colour) values[index]);
          else
            this.AddValue(index, (double) (int) values[index]);
        }
      }
    }
}
