// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Matrix4
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Text;

namespace SonicOrca.Geometry
{

    public struct Matrix4 : IEquatable<Matrix4>
    {
      public double M11;
      public double M21;
      public double M31;
      public double M41;
      public double M12;
      public double M22;
      public double M32;
      public double M42;
      public double M13;
      public double M23;
      public double M33;
      public double M43;
      public double M14;
      public double M24;
      public double M34;
      public double M44;
      public static Matrix4 Identity = new Matrix4(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);

      public Vector4 Row1
      {
        get => new Vector4(this.M11, this.M21, this.M31, this.M41);
        set
        {
          this.M11 = value.X;
          this.M21 = value.Y;
          this.M31 = value.Z;
          this.M41 = value.W;
        }
      }

      public Vector4 Row2
      {
        get => new Vector4(this.M12, this.M22, this.M32, this.M42);
        set
        {
          this.M12 = value.X;
          this.M22 = value.Y;
          this.M32 = value.Z;
          this.M42 = value.W;
        }
      }

      public Vector4 Row3
      {
        get => new Vector4(this.M13, this.M23, this.M33, this.M43);
        set
        {
          this.M13 = value.X;
          this.M23 = value.Y;
          this.M33 = value.Z;
          this.M43 = value.W;
        }
      }

      public Vector4 Row4
      {
        get => new Vector4(this.M14, this.M24, this.M34, this.M44);
        set
        {
          this.M14 = value.X;
          this.M24 = value.Y;
          this.M34 = value.Z;
          this.M44 = value.W;
        }
      }

      public double this[int x, int y]
      {
        get
        {
          if (x == 0 && y == 0)
            return this.M11;
          if (x == 1 && y == 0)
            return this.M21;
          if (x == 2 && y == 0)
            return this.M31;
          if (x == 3 && y == 0)
            return this.M41;
          if (x == 0 && y == 1)
            return this.M12;
          if (x == 1 && y == 1)
            return this.M22;
          if (x == 2 && y == 1)
            return this.M32;
          if (x == 3 && y == 1)
            return this.M42;
          if (x == 0 && y == 2)
            return this.M13;
          if (x == 1 && y == 2)
            return this.M23;
          if (x == 2 && y == 2)
            return this.M33;
          if (x == 3 && y == 2)
            return this.M43;
          if (x == 0 && y == 3)
            return this.M14;
          if (x == 1 && y == 3)
            return this.M24;
          if (x == 2 && y == 3)
            return this.M34;
          if (x == 3 && y == 3)
            return this.M44;
          throw new ArgumentOutOfRangeException();
        }
        set
        {
          if (x == 0 && y == 0)
            this.M11 = value;
          else if (x == 1 && y == 0)
            this.M21 = value;
          else if (x == 2 && y == 0)
            this.M31 = value;
          else if (x == 3 && y == 0)
            this.M41 = value;
          else if (x == 0 && y == 1)
            this.M12 = value;
          else if (x == 1 && y == 1)
            this.M22 = value;
          else if (x == 2 && y == 1)
            this.M32 = value;
          else if (x == 3 && y == 1)
            this.M42 = value;
          else if (x == 0 && y == 2)
            this.M13 = value;
          else if (x == 1 && y == 2)
            this.M23 = value;
          else if (x == 2 && y == 2)
            this.M33 = value;
          else if (x == 3 && y == 2)
            this.M43 = value;
          else if (x == 0 && y == 3)
            this.M14 = value;
          else if (x == 1 && y == 3)
            this.M24 = value;
          else if (x == 2 && y == 3)
          {
            this.M34 = value;
          }
          else
          {
            if (x != 3 || y != 3)
              throw new ArgumentOutOfRangeException();
            this.M44 = value;
          }
        }
      }

      public Matrix4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4)
        : this()
      {
        this.Row1 = row1;
        this.Row2 = row2;
        this.Row3 = row3;
        this.Row4 = row4;
      }

      public override bool Equals(object obj) => this.Equals((Matrix4) obj);

      public bool Equals(Matrix4 other)
      {
        return this.Row1 == other.Row1 && this.Row2 == other.Row2 && this.Row3 == other.Row3 && this.Row4 == other.Row4;
      }

      public override int GetHashCode()
      {
        return (((13 * 7 + this.Row1.GetHashCode()) * 7 + this.Row2.GetHashCode()) * 7 + this.Row3.GetHashCode()) * 7 + this.Row4.GetHashCode();
      }

      public override string ToString()
      {
        Func<Vector4, string> func = (Func<Vector4, string>) (v => $"({v.X},{v.Y},{v.Z},{v.W})");
        return string.Join(Environment.NewLine, func(this.Row1), func(this.Row2), func(this.Row3), func(this.Row4));
      }

      public string GetFormattedString()
      {
        string[,] strArray = new string[4, 4];
        int[] numArray = new int[4];
        for (int y = 0; y < 4; ++y)
        {
          for (int x = 0; x < 4; ++x)
          {
            strArray[x, y] = this[x, y].ToString();
            numArray[x] = Math.Max(numArray[x], strArray[x, y].Length);
          }
        }
        StringBuilder stringBuilder = new StringBuilder();
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
          {
            int count = numArray[index2] - strArray[index2, index1].Length;
            stringBuilder.Append(' ');
            stringBuilder.Append(new string(' ', count));
            stringBuilder.Append(strArray[index2, index1]);
            stringBuilder.Append(' ');
          }
          if (index1 < 3)
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
      }

      public Matrix4 Translate(double x, double y, double z = 0.0)
      {
        return Matrix4.CreateTranslation(x, y, z) * this;
      }

      public Matrix4 Translate(Vector2 v) => Matrix4.CreateTranslation(v) * this;

      public Matrix4 Translate(Vector3 v) => Matrix4.CreateTranslation(v) * this;

      public Matrix4 Scale(Vector2 v) => Matrix4.CreateScale(v.X, v.Y) * this;

      public Matrix4 Scale(double x, double y, double z = 1.0) => Matrix4.CreateScale(x, y, z) * this;

      public Matrix4 RotateX(double angle) => this * Matrix4.CreateRotationX(angle);

      public Matrix4 RotateY(double angle) => this * Matrix4.CreateRotationY(angle);

      public Matrix4 RotateZ(double angle) => this * Matrix4.CreateRotationZ(angle);

      public static Matrix4 operator *(Matrix4 lhs, Matrix4 rhs)
      {
        return new Matrix4()
        {
          M11 = lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31 + lhs.M14 * rhs.M41,
          M12 = lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32 + lhs.M14 * rhs.M42,
          M13 = lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33 + lhs.M14 * rhs.M43,
          M14 = lhs.M11 * rhs.M14 + lhs.M12 * rhs.M24 + lhs.M13 * rhs.M34 + lhs.M14 * rhs.M44,
          M21 = lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31 + lhs.M24 * rhs.M41,
          M22 = lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32 + lhs.M24 * rhs.M42,
          M23 = lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33 + lhs.M24 * rhs.M43,
          M24 = lhs.M21 * rhs.M14 + lhs.M22 * rhs.M24 + lhs.M23 * rhs.M34 + lhs.M24 * rhs.M44,
          M31 = lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31 + lhs.M34 * rhs.M41,
          M32 = lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32 + lhs.M34 * rhs.M42,
          M33 = lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33 + lhs.M34 * rhs.M43,
          M34 = lhs.M31 * rhs.M14 + lhs.M32 * rhs.M24 + lhs.M33 * rhs.M34 + lhs.M34 * rhs.M44,
          M41 = lhs.M41 * rhs.M11 + lhs.M42 * rhs.M21 + lhs.M43 * rhs.M31 + lhs.M44 * rhs.M41,
          M42 = lhs.M41 * rhs.M12 + lhs.M42 * rhs.M22 + lhs.M43 * rhs.M32 + lhs.M44 * rhs.M42,
          M43 = lhs.M41 * rhs.M13 + lhs.M42 * rhs.M23 + lhs.M43 * rhs.M33 + lhs.M44 * rhs.M43,
          M44 = lhs.M41 * rhs.M14 + lhs.M42 * rhs.M24 + lhs.M43 * rhs.M34 + lhs.M44 * rhs.M44
        };
      }

      public static Vector2 operator *(Matrix4 mat, Vector2 vec)
      {
        return (mat * new Vector4(vec.X, vec.Y, 1.0, 1.0)).XY;
      }

      public static Vector3 operator *(Matrix4 mat, Vector3 vec)
      {
        return (mat * new Vector4(vec.X, vec.Y, vec.Z, 1.0)).XYZ;
      }

      public static Vector4 operator *(Matrix4 mat, Vector4 vec)
      {
        return new Vector4(vec.X * mat.Row1.X + vec.Y * mat.Row2.X + vec.Z * mat.Row3.X + vec.W * mat.Row4.X, vec.X * mat.Row1.Y + vec.Y * mat.Row2.Y + vec.Z * mat.Row3.Y + vec.W * mat.Row4.Y, vec.X * mat.Row1.Z + vec.Y * mat.Row2.Z + vec.Z * mat.Row3.Z + vec.W * mat.Row4.Z, vec.X * mat.Row1.W + vec.Y * mat.Row2.W + vec.Z * mat.Row3.W + vec.W * mat.Row4.W);
      }

      public static bool operator ==(Matrix4 lhs, Matrix4 rhs) => lhs.Equals(rhs);

      public static bool operator !=(Matrix4 lhs, Matrix4 rhs) => !lhs.Equals(rhs);

      public static Matrix4 CreateOrthographic(double width, double height, double zNear, double zFar)
      {
        return Matrix4.CreateOrthographicOffCenter(-width / 2.0, width / 2.0, -height / 2.0, height / 2.0, zNear, zFar);
      }

      public static Matrix4 CreateOrthographicOffCenter(
        double left,
        double right,
        double bottom,
        double top,
        double zNear,
        double zFar)
      {
        Matrix4 orthographicOffCenter = new Matrix4();
        double num1 = 1.0 / (right - left);
        double num2 = 1.0 / (top - bottom);
        double num3 = 1.0 / (zFar - zNear);
        orthographicOffCenter.M11 = 2.0 * num1;
        orthographicOffCenter.M22 = 2.0 * num2;
        orthographicOffCenter.M33 = -2.0 * num3;
        orthographicOffCenter.M14 = -(right + left) * num1;
        orthographicOffCenter.M24 = -(top + bottom) * num2;
        orthographicOffCenter.M34 = -(zFar + zNear) * num3;
        orthographicOffCenter.M44 = 1.0;
        return orthographicOffCenter;
      }

      public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
      {
        Vector3 normalised1 = (eye - target).Normalised;
        Vector3 normalised2 = up.Cross(normalised1).Normalised;
        Vector3 normalised3 = normalised1.Cross(normalised2).Normalised;
        return new Matrix4(new Vector4(normalised2.X, normalised3.X, normalised1.X, 0.0), new Vector4(normalised2.Y, normalised3.Y, normalised1.Y, 0.0), new Vector4(normalised2.Z, normalised3.Z, normalised1.Z, 0.0), Vector4.UnitW) * Matrix4.CreateTranslation(-eye);
      }

      public static Matrix4 LookAt(
        double eyeX,
        double eyeY,
        double eyeZ,
        double targetX,
        double targetY,
        double targetZ,
        double upX,
        double upY,
        double upZ)
      {
        return Matrix4.LookAt(new Vector3(eyeX, eyeY, eyeZ), new Vector3(targetX, targetY, targetZ), new Vector3(upX, upY, upZ));
      }

      public static Matrix4 Frustum(
        double left,
        double right,
        double bottom,
        double top,
        double near,
        double far)
      {
        double num1 = 1.0 / (right - left);
        double num2 = 1.0 / (top - bottom);
        double num3 = 1.0 / (far - near);
        return new Matrix4(new Vector4(2.0 * near * num1, 0.0, 0.0, 0.0), new Vector4(0.0, 2.0 * near * num2, 0.0, 0.0), new Vector4((right + left) * num1, (top + bottom) * num2, -(far + near) * num3, -1.0), new Vector4(0.0, 0.0, -2.0 * far * near * num3, 0.0));
      }

      public static Matrix4 Perspective(double fovy, double aspect, double near, double far)
      {
        double top = near * Math.Tan(0.5 * fovy);
        double bottom = -top;
        return Matrix4.Frustum(bottom * aspect, top * aspect, bottom, top, near, far);
      }

      public static Matrix4 CreateTranslation(Vector2 value)
      {
        return Matrix4.Identity with
        {
          M14 = value.X,
          M24 = value.Y
        };
      }

      public static Matrix4 CreateTranslation(Vector3 value)
      {
        return Matrix4.Identity with
        {
          M14 = value.X,
          M24 = value.Y,
          M34 = value.Z
        };
      }

      public static Matrix4 CreateTranslation(double x, double y, double z = 0.0)
      {
        return Matrix4.Identity with
        {
          M14 = x,
          M24 = y,
          M34 = z
        };
      }

      public static Matrix4 CreateScale(double x, double y, double z = 1.0)
      {
        return Matrix4.Identity with
        {
          M11 = x,
          M22 = y,
          M33 = z
        };
      }

      public static Matrix4 CreateRotationX(double angle)
      {
        double num1 = Math.Cos(angle);
        double num2 = Math.Sin(angle);
        return Matrix4.Identity with
        {
          M21 = num1,
          M31 = num2,
          M22 = -num2,
          M32 = num2
        };
      }

      public static Matrix4 CreateRotationY(double angle)
      {
        double num1 = Math.Cos(angle);
        double num2 = Math.Sin(angle);
        return Matrix4.Identity with
        {
          M11 = num1,
          M31 = -num2,
          M13 = num2,
          M33 = num1
        };
      }

      public static Matrix4 CreateRotationZ(double angle)
      {
        double num1 = Math.Cos(angle);
        double num2 = Math.Sin(angle);
        return Matrix4.Identity with
        {
          M11 = num1,
          M21 = num2,
          M12 = -num2,
          M22 = num1
        };
      }
    }
}
