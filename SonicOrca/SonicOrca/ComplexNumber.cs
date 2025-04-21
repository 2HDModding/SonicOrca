// Decompiled with JetBrains decompiler
// Type: SonicOrca.ComplexNumber
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca
{

    public struct ComplexNumber : IEquatable<ComplexNumber>
    {
      public double Real { get; set; }

      public double Imaginary { get; set; }

      public double Magnitude => Math.Sqrt(this.Real * this.Real + this.Imaginary * this.Imaginary);

      public ComplexNumber(double real, double imaginary)
        : this()
      {
        this.Real = real;
        this.Imaginary = imaginary;
      }

      public override bool Equals(object obj) => this.Equals((ComplexNumber) obj);

      public bool Equals(ComplexNumber other)
      {
        return this.Real == other.Real && this.Imaginary == other.Imaginary;
      }

      public override int GetHashCode()
      {
        return (13 * 7 + this.Real.GetHashCode()) * 7 + this.Imaginary.GetHashCode();
      }

      public override string ToString() => $"{this.Real} + {this.Imaginary}i";
    }
}
