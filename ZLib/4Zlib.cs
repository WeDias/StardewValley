// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.Adler
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace Ionic.Zlib
{
  /// <summary>Computes an Adler-32 checksum.</summary>
  /// <remarks>
  /// The Adler checksum is similar to a CRC checksum, but faster to compute, though less
  /// reliable.  It is used in producing RFC1950 compressed streams.  The Adler checksum
  /// is a required part of the "ZLIB" standard.  Applications will almost never need to
  /// use this class directly.
  /// </remarks>
  /// <exclude />
  public sealed class Adler
  {
    private static readonly uint BASE = 65521;
    private static readonly int NMAX = 5552;

    /// <summary>Calculates the Adler32 checksum.</summary>
    /// <remarks>
    ///   <para>
    ///     This is used within ZLIB.  You probably don't need to use this directly.
    ///   </para>
    /// </remarks>
    /// <example>
    ///    To compute an Adler32 checksum on a byte array:
    ///  <code>
    ///    var adler = Adler.Adler32(0, null, 0, 0);
    ///    adler = Adler.Adler32(adler, buffer, index, length);
    ///  </code>
    /// </example>
    public static uint Adler32(uint adler, byte[] buf, int index, int len)
    {
      if (buf == null)
        return 1;
      uint num1 = adler & (uint) ushort.MaxValue;
      uint num2 = adler >> 16 & (uint) ushort.MaxValue;
      while (len > 0)
      {
        int num3 = len < Adler.NMAX ? len : Adler.NMAX;
        len -= num3;
        for (; num3 >= 16; num3 -= 16)
        {
          uint num4 = num1 + (uint) buf[index++];
          uint num5 = num2 + num4;
          uint num6 = num4 + (uint) buf[index++];
          uint num7 = num5 + num6;
          uint num8 = num6 + (uint) buf[index++];
          uint num9 = num7 + num8;
          uint num10 = num8 + (uint) buf[index++];
          uint num11 = num9 + num10;
          uint num12 = num10 + (uint) buf[index++];
          uint num13 = num11 + num12;
          uint num14 = num12 + (uint) buf[index++];
          uint num15 = num13 + num14;
          uint num16 = num14 + (uint) buf[index++];
          uint num17 = num15 + num16;
          uint num18 = num16 + (uint) buf[index++];
          uint num19 = num17 + num18;
          uint num20 = num18 + (uint) buf[index++];
          uint num21 = num19 + num20;
          uint num22 = num20 + (uint) buf[index++];
          uint num23 = num21 + num22;
          uint num24 = num22 + (uint) buf[index++];
          uint num25 = num23 + num24;
          uint num26 = num24 + (uint) buf[index++];
          uint num27 = num25 + num26;
          uint num28 = num26 + (uint) buf[index++];
          uint num29 = num27 + num28;
          uint num30 = num28 + (uint) buf[index++];
          uint num31 = num29 + num30;
          uint num32 = num30 + (uint) buf[index++];
          uint num33 = num31 + num32;
          num1 = num32 + (uint) buf[index++];
          num2 = num33 + num1;
        }
        if (num3 != 0)
        {
          do
          {
            num1 += (uint) buf[index++];
            num2 += num1;
          }
          while (--num3 != 0);
        }
        num1 %= Adler.BASE;
        num2 %= Adler.BASE;
      }
      return num2 << 16 | num1;
    }
  }
}
