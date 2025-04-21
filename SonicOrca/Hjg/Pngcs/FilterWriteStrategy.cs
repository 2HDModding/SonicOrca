// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.FilterWriteStrategy
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs
{

    internal class FilterWriteStrategy
    {
      private static readonly int COMPUTE_STATS_EVERY_N_LINES = 8;
      private readonly ImageInfo imgInfo;
      private readonly FilterType configuredType;
      private FilterType currentType;
      private int lastRowTested = -1000000;
      private double[] lastSums = new double[5];
      private double[] lastEntropies = new double[5];
      private double[] preference = new double[5]
      {
        1.1,
        1.1,
        1.1,
        1.1,
        1.2
      };
      private int discoverEachLines = -1;
      private double[] histogram1 = new double[256 /*0x0100*/];

      internal FilterWriteStrategy(ImageInfo imgInfo, FilterType configuredType)
      {
        this.imgInfo = imgInfo;
        this.configuredType = configuredType;
        this.currentType = configuredType >= FilterType.FILTER_NONE ? configuredType : (imgInfo.Rows < 8 && imgInfo.Cols < 8 || imgInfo.Indexed || imgInfo.BitDepth < 8 ? FilterType.FILTER_NONE : FilterType.FILTER_PAETH);
        if (configuredType == FilterType.FILTER_AGGRESSIVE)
          this.discoverEachLines = FilterWriteStrategy.COMPUTE_STATS_EVERY_N_LINES;
        if (configuredType != FilterType.FILTER_VERYAGGRESSIVE)
          return;
        this.discoverEachLines = 1;
      }

      internal bool shouldTestAll(int rown)
      {
        if (this.discoverEachLines <= 0 || this.lastRowTested + this.discoverEachLines > rown)
          return false;
        this.currentType = FilterType.FILTER_UNKNOWN;
        return true;
      }

      internal void setPreference(double none, double sub, double up, double ave, double paeth)
      {
        this.preference = new double[5]
        {
          none,
          sub,
          up,
          ave,
          paeth
        };
      }

      internal bool computesStatistics() => this.discoverEachLines > 0;

      internal void fillResultsForFilter(
        int rown,
        FilterType type,
        double sum,
        int[] histo,
        bool tentative)
      {
        this.lastRowTested = rown;
        this.lastSums[(int) type] = sum;
        if (histo == null)
          return;
        double num1 = rown == 0 ? 0.0 : 0.3;
        double num2 = 1.0 - num1;
        double num3 = 0.0;
        for (int index = 0; index < 256 /*0x0100*/; ++index)
        {
          double num4 = (double) histo[index] / (double) this.imgInfo.Cols;
          double d = this.histogram1[index] * num1 + num4 * num2;
          if (tentative)
            num3 += d > 1E-08 ? d * Math.Log(d) : 0.0;
          else
            this.histogram1[index] = d;
        }
        this.lastEntropies[(int) type] = -num3;
      }

      internal FilterType gimmeFilterType(int rown, bool useEntropy)
      {
        if (this.currentType == FilterType.FILTER_UNKNOWN)
        {
          if (rown == 0)
          {
            this.currentType = FilterType.FILTER_SUB;
          }
          else
          {
            double num1 = double.MaxValue;
            for (int index = 0; index < 5; ++index)
            {
              double num2 = (useEntropy ? this.lastEntropies[index] : this.lastSums[index]) / this.preference[index];
              if (num2 <= num1)
              {
                num1 = num2;
                this.currentType = (FilterType) index;
              }
            }
          }
        }
        if (this.configuredType == FilterType.FILTER_CYCLIC)
          this.currentType = (FilterType) ((int) (this.currentType + 1) % 5);
        return this.currentType;
      }
    }
}
