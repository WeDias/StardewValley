// Decompiled with JetBrains decompiler
// Type: StardewValley.NoiseGenerator
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace StardewValley
{
  [InstanceStatics]
  internal static class NoiseGenerator
  {
    public static int Seed { get; set; }

    public static int Octaves { get; set; }

    public static double Amplitude { get; set; }

    public static double Persistence { get; set; }

    public static double Frequency { get; set; }

    static NoiseGenerator()
    {
      NoiseGenerator.Seed = new Random().Next(int.MaxValue);
      NoiseGenerator.Octaves = 8;
      NoiseGenerator.Amplitude = 1.0;
      NoiseGenerator.Frequency = 0.015;
      NoiseGenerator.Persistence = 0.65;
    }

    public static double Noise(int x, int y)
    {
      double num = 0.0;
      double frequency = NoiseGenerator.Frequency;
      double amplitude = NoiseGenerator.Amplitude;
      for (int index = 0; index < NoiseGenerator.Octaves; ++index)
      {
        num += NoiseGenerator.Smooth((double) x * frequency, (double) y * frequency) * amplitude;
        frequency *= 2.0;
        amplitude *= NoiseGenerator.Persistence;
      }
      if (num < -2.4)
        num = -2.4;
      else if (num > 2.4)
        num = 2.4;
      return num / 2.4;
    }

    public static double NoiseGeneration(int x, int y)
    {
      int num1 = x + y * 57;
      int num2 = num1 << 13 ^ num1;
      return 1.0 - (double) (num2 * (num2 * num2 * 15731 + 789221) + NoiseGenerator.Seed & int.MaxValue) / 1073741824.0;
    }

    private static double Interpolate(double x, double y, double a)
    {
      double num = (1.0 - Math.Cos(a * Math.PI)) * 0.5;
      return x * (1.0 - num) + y * num;
    }

    private static double Smooth(double x, double y)
    {
      double x1 = NoiseGenerator.NoiseGeneration((int) x, (int) y);
      double num = NoiseGenerator.NoiseGeneration((int) x + 1, (int) y);
      double x2 = NoiseGenerator.NoiseGeneration((int) x, (int) y + 1);
      double y1 = NoiseGenerator.NoiseGeneration((int) x + 1, (int) y + 1);
      double y2 = num;
      double a = x - (double) (int) x;
      return NoiseGenerator.Interpolate(NoiseGenerator.Interpolate(x1, y2, a), NoiseGenerator.Interpolate(x2, y1, x - (double) (int) x), y - (double) (int) y);
    }
  }
}
