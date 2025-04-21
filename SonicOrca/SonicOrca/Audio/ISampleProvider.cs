// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.ISampleProvider
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Audio
{

    public interface ISampleProvider
    {
      double CalculatedVolume { get; }

      double Pan { get; }

      bool Playing { get; }

      int Read(byte[] buffer, int offset, int count);
    }
}
