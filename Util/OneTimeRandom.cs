// Decompiled with JetBrains decompiler
// Type: StardewValley.OneTimeRandom
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley
{
  internal class OneTimeRandom
  {
    private const double shift3 = 0.125;
    private const double shift9 = 0.001953125;
    private const double shift27 = 7.45058059692383E-09;
    private const double shift53 = 1.11022302462516E-16;

    public static ulong GetLong(ulong a, ulong b, ulong c, ulong d)
    {
      ulong num1 = (ulong) ((((long) a ^ ((long) (b >> 14) | (long) b << 50)) + (((long) (c >> 31) | (long) c << 33) ^ ((long) (d >> 18) | (long) d << 46))) * 1911413418482053185L);
      ulong num2 = (ulong) (((((long) (a >> 30) | (long) a << 34) ^ (long) c) + (((long) (b >> 32) | (long) b << 32) ^ ((long) (d >> 50) | (long) d << 14))) * 1139072524405308145L);
      ulong num3 = (ulong) (((((long) (a >> 49) | (long) a << 15) ^ ((long) (d >> 33) | (long) d << 31)) + ((long) b ^ ((long) (c >> 48) | (long) c << 16))) * 8792993707439626365L);
      ulong num4 = (ulong) (((((long) (a >> 17) | (long) a << 47) ^ ((long) (b >> 47) | (long) b << 17)) + (((long) (c >> 15) | (long) c << 49) ^ (long) d)) * 1089642907432013597L);
      return (ulong) (((long) num1 ^ (long) num2 ^ ((long) (num3 >> 21) | (long) num3 << 43) ^ ((long) (num4 >> 44) | (long) num4 << 20)) * 2550117894111961111L + (((long) (num1 >> 20) | (long) num1 << 44) ^ ((long) (num2 >> 41) | (long) num2 << 23) ^ ((long) (num3 >> 42) | (long) num3 << 22) ^ (long) num4) * 8786584852613159497L + (((long) (num1 >> 43) | (long) num1 << 21) ^ ((long) (num2 >> 22) | (long) num2 << 42) ^ (long) num3 ^ ((long) (num4 >> 23) | (long) num4 << 41)) * 3971056679291618767L);
    }

    public static double GetDouble(ulong a, ulong b, ulong c, ulong d) => (double) (OneTimeRandom.GetLong(a, b, c, d) >> 11) * 1.11022302462516E-16;
  }
}
