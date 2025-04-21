// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectEditorPropertyInteger
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Core
{

    public class ObjectEditorPropertyInteger : ObjectEditorProperty
    {
      private readonly int _minValue;
      private readonly int _maxValue;

      public int MinValue => this._minValue;

      public int MaxValue => this._maxValue;

      public ObjectEditorPropertyInteger(
        string name,
        string key,
        int minValue,
        int maxValue,
        int defaultValue = 0,
        string description = null)
        : base(name, key, typeof (int), (object) defaultValue, description)
      {
        this._minValue = minValue;
        this._maxValue = maxValue;
      }

      public override bool Validate(ref object value)
      {
        int result;
        if (value is string)
        {
          if (!int.TryParse((string) value, out result))
            return false;
        }
        else
        {
          if (!(value is int))
            return false;
          result = (int) value;
        }
        value = (object) MathX.Clamp(this._minValue, result, this._maxValue);
        return true;
      }
    }
}
