// Decompiled with JetBrains decompiler
// Type: StardewValley.Noise
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley
{
  public class Noise
  {
    private static byte[] perm = new byte[512]
    {
      (byte) 151,
      (byte) 160,
      (byte) 137,
      (byte) 91,
      (byte) 90,
      (byte) 15,
      (byte) 131,
      (byte) 13,
      (byte) 201,
      (byte) 95,
      (byte) 96,
      (byte) 53,
      (byte) 194,
      (byte) 233,
      (byte) 7,
      (byte) 225,
      (byte) 140,
      (byte) 36,
      (byte) 103,
      (byte) 30,
      (byte) 69,
      (byte) 142,
      (byte) 8,
      (byte) 99,
      (byte) 37,
      (byte) 240,
      (byte) 21,
      (byte) 10,
      (byte) 23,
      (byte) 190,
      (byte) 6,
      (byte) 148,
      (byte) 247,
      (byte) 120,
      (byte) 234,
      (byte) 75,
      (byte) 0,
      (byte) 26,
      (byte) 197,
      (byte) 62,
      (byte) 94,
      (byte) 252,
      (byte) 219,
      (byte) 203,
      (byte) 117,
      (byte) 35,
      (byte) 11,
      (byte) 32,
      (byte) 57,
      (byte) 177,
      (byte) 33,
      (byte) 88,
      (byte) 237,
      (byte) 149,
      (byte) 56,
      (byte) 87,
      (byte) 174,
      (byte) 20,
      (byte) 125,
      (byte) 136,
      (byte) 171,
      (byte) 168,
      (byte) 68,
      (byte) 175,
      (byte) 74,
      (byte) 165,
      (byte) 71,
      (byte) 134,
      (byte) 139,
      (byte) 48,
      (byte) 27,
      (byte) 166,
      (byte) 77,
      (byte) 146,
      (byte) 158,
      (byte) 231,
      (byte) 83,
      (byte) 111,
      (byte) 229,
      (byte) 122,
      (byte) 60,
      (byte) 211,
      (byte) 133,
      (byte) 230,
      (byte) 220,
      (byte) 105,
      (byte) 92,
      (byte) 41,
      (byte) 55,
      (byte) 46,
      (byte) 245,
      (byte) 40,
      (byte) 244,
      (byte) 102,
      (byte) 143,
      (byte) 54,
      (byte) 65,
      (byte) 25,
      (byte) 63,
      (byte) 161,
      (byte) 1,
      (byte) 216,
      (byte) 80,
      (byte) 73,
      (byte) 209,
      (byte) 76,
      (byte) 132,
      (byte) 187,
      (byte) 208,
      (byte) 89,
      (byte) 18,
      (byte) 169,
      (byte) 200,
      (byte) 196,
      (byte) 135,
      (byte) 130,
      (byte) 116,
      (byte) 188,
      (byte) 159,
      (byte) 86,
      (byte) 164,
      (byte) 100,
      (byte) 109,
      (byte) 198,
      (byte) 173,
      (byte) 186,
      (byte) 3,
      (byte) 64,
      (byte) 52,
      (byte) 217,
      (byte) 226,
      (byte) 250,
      (byte) 124,
      (byte) 123,
      (byte) 5,
      (byte) 202,
      (byte) 38,
      (byte) 147,
      (byte) 118,
      (byte) 126,
      byte.MaxValue,
      (byte) 82,
      (byte) 85,
      (byte) 212,
      (byte) 207,
      (byte) 206,
      (byte) 59,
      (byte) 227,
      (byte) 47,
      (byte) 16,
      (byte) 58,
      (byte) 17,
      (byte) 182,
      (byte) 189,
      (byte) 28,
      (byte) 42,
      (byte) 223,
      (byte) 183,
      (byte) 170,
      (byte) 213,
      (byte) 119,
      (byte) 248,
      (byte) 152,
      (byte) 2,
      (byte) 44,
      (byte) 154,
      (byte) 163,
      (byte) 70,
      (byte) 221,
      (byte) 153,
      (byte) 101,
      (byte) 155,
      (byte) 167,
      (byte) 43,
      (byte) 172,
      (byte) 9,
      (byte) 129,
      (byte) 22,
      (byte) 39,
      (byte) 253,
      (byte) 19,
      (byte) 98,
      (byte) 108,
      (byte) 110,
      (byte) 79,
      (byte) 113,
      (byte) 224,
      (byte) 232,
      (byte) 178,
      (byte) 185,
      (byte) 112,
      (byte) 104,
      (byte) 218,
      (byte) 246,
      (byte) 97,
      (byte) 228,
      (byte) 251,
      (byte) 34,
      (byte) 242,
      (byte) 193,
      (byte) 238,
      (byte) 210,
      (byte) 144,
      (byte) 12,
      (byte) 191,
      (byte) 179,
      (byte) 162,
      (byte) 241,
      (byte) 81,
      (byte) 51,
      (byte) 145,
      (byte) 235,
      (byte) 249,
      (byte) 14,
      (byte) 239,
      (byte) 107,
      (byte) 49,
      (byte) 192,
      (byte) 214,
      (byte) 31,
      (byte) 181,
      (byte) 199,
      (byte) 106,
      (byte) 157,
      (byte) 184,
      (byte) 84,
      (byte) 204,
      (byte) 176,
      (byte) 115,
      (byte) 121,
      (byte) 50,
      (byte) 45,
      (byte) 127,
      (byte) 4,
      (byte) 150,
      (byte) 254,
      (byte) 138,
      (byte) 236,
      (byte) 205,
      (byte) 93,
      (byte) 222,
      (byte) 114,
      (byte) 67,
      (byte) 29,
      (byte) 24,
      (byte) 72,
      (byte) 243,
      (byte) 141,
      (byte) 128,
      (byte) 195,
      (byte) 78,
      (byte) 66,
      (byte) 215,
      (byte) 61,
      (byte) 156,
      (byte) 180,
      (byte) 151,
      (byte) 160,
      (byte) 137,
      (byte) 91,
      (byte) 90,
      (byte) 15,
      (byte) 131,
      (byte) 13,
      (byte) 201,
      (byte) 95,
      (byte) 96,
      (byte) 53,
      (byte) 194,
      (byte) 233,
      (byte) 7,
      (byte) 225,
      (byte) 140,
      (byte) 36,
      (byte) 103,
      (byte) 30,
      (byte) 69,
      (byte) 142,
      (byte) 8,
      (byte) 99,
      (byte) 37,
      (byte) 240,
      (byte) 21,
      (byte) 10,
      (byte) 23,
      (byte) 190,
      (byte) 6,
      (byte) 148,
      (byte) 247,
      (byte) 120,
      (byte) 234,
      (byte) 75,
      (byte) 0,
      (byte) 26,
      (byte) 197,
      (byte) 62,
      (byte) 94,
      (byte) 252,
      (byte) 219,
      (byte) 203,
      (byte) 117,
      (byte) 35,
      (byte) 11,
      (byte) 32,
      (byte) 57,
      (byte) 177,
      (byte) 33,
      (byte) 88,
      (byte) 237,
      (byte) 149,
      (byte) 56,
      (byte) 87,
      (byte) 174,
      (byte) 20,
      (byte) 125,
      (byte) 136,
      (byte) 171,
      (byte) 168,
      (byte) 68,
      (byte) 175,
      (byte) 74,
      (byte) 165,
      (byte) 71,
      (byte) 134,
      (byte) 139,
      (byte) 48,
      (byte) 27,
      (byte) 166,
      (byte) 77,
      (byte) 146,
      (byte) 158,
      (byte) 231,
      (byte) 83,
      (byte) 111,
      (byte) 229,
      (byte) 122,
      (byte) 60,
      (byte) 211,
      (byte) 133,
      (byte) 230,
      (byte) 220,
      (byte) 105,
      (byte) 92,
      (byte) 41,
      (byte) 55,
      (byte) 46,
      (byte) 245,
      (byte) 40,
      (byte) 244,
      (byte) 102,
      (byte) 143,
      (byte) 54,
      (byte) 65,
      (byte) 25,
      (byte) 63,
      (byte) 161,
      (byte) 1,
      (byte) 216,
      (byte) 80,
      (byte) 73,
      (byte) 209,
      (byte) 76,
      (byte) 132,
      (byte) 187,
      (byte) 208,
      (byte) 89,
      (byte) 18,
      (byte) 169,
      (byte) 200,
      (byte) 196,
      (byte) 135,
      (byte) 130,
      (byte) 116,
      (byte) 188,
      (byte) 159,
      (byte) 86,
      (byte) 164,
      (byte) 100,
      (byte) 109,
      (byte) 198,
      (byte) 173,
      (byte) 186,
      (byte) 3,
      (byte) 64,
      (byte) 52,
      (byte) 217,
      (byte) 226,
      (byte) 250,
      (byte) 124,
      (byte) 123,
      (byte) 5,
      (byte) 202,
      (byte) 38,
      (byte) 147,
      (byte) 118,
      (byte) 126,
      byte.MaxValue,
      (byte) 82,
      (byte) 85,
      (byte) 212,
      (byte) 207,
      (byte) 206,
      (byte) 59,
      (byte) 227,
      (byte) 47,
      (byte) 16,
      (byte) 58,
      (byte) 17,
      (byte) 182,
      (byte) 189,
      (byte) 28,
      (byte) 42,
      (byte) 223,
      (byte) 183,
      (byte) 170,
      (byte) 213,
      (byte) 119,
      (byte) 248,
      (byte) 152,
      (byte) 2,
      (byte) 44,
      (byte) 154,
      (byte) 163,
      (byte) 70,
      (byte) 221,
      (byte) 153,
      (byte) 101,
      (byte) 155,
      (byte) 167,
      (byte) 43,
      (byte) 172,
      (byte) 9,
      (byte) 129,
      (byte) 22,
      (byte) 39,
      (byte) 253,
      (byte) 19,
      (byte) 98,
      (byte) 108,
      (byte) 110,
      (byte) 79,
      (byte) 113,
      (byte) 224,
      (byte) 232,
      (byte) 178,
      (byte) 185,
      (byte) 112,
      (byte) 104,
      (byte) 218,
      (byte) 246,
      (byte) 97,
      (byte) 228,
      (byte) 251,
      (byte) 34,
      (byte) 242,
      (byte) 193,
      (byte) 238,
      (byte) 210,
      (byte) 144,
      (byte) 12,
      (byte) 191,
      (byte) 179,
      (byte) 162,
      (byte) 241,
      (byte) 81,
      (byte) 51,
      (byte) 145,
      (byte) 235,
      (byte) 249,
      (byte) 14,
      (byte) 239,
      (byte) 107,
      (byte) 49,
      (byte) 192,
      (byte) 214,
      (byte) 31,
      (byte) 181,
      (byte) 199,
      (byte) 106,
      (byte) 157,
      (byte) 184,
      (byte) 84,
      (byte) 204,
      (byte) 176,
      (byte) 115,
      (byte) 121,
      (byte) 50,
      (byte) 45,
      (byte) 127,
      (byte) 4,
      (byte) 150,
      (byte) 254,
      (byte) 138,
      (byte) 236,
      (byte) 205,
      (byte) 93,
      (byte) 222,
      (byte) 114,
      (byte) 67,
      (byte) 29,
      (byte) 24,
      (byte) 72,
      (byte) 243,
      (byte) 141,
      (byte) 128,
      (byte) 195,
      (byte) 78,
      (byte) 66,
      (byte) 215,
      (byte) 61,
      (byte) 156,
      (byte) 180
    };

    /// <summary>1D simplex noise</summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Generate(float x)
    {
      int num1 = Noise.FastFloor(x);
      int num2 = num1 + 1;
      float x1 = x - (float) num1;
      float x2 = x1 - 1f;
      double num3 = 1.0 - (double) x1 * (double) x1;
      double num4 = num3 * num3;
      float num5 = (float) (num4 * num4) * Noise.grad((int) Noise.perm[num1 & (int) byte.MaxValue], x1);
      double num6 = 1.0 - (double) x2 * (double) x2;
      double num7 = num6 * num6;
      float num8 = (float) (num7 * num7) * Noise.grad((int) Noise.perm[num2 & (int) byte.MaxValue], x2);
      return (float) (0.395000010728836 * ((double) num5 + (double) num8));
    }

    /// <summary>2D simplex noise</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float Generate(float x, float y)
    {
      float num1 = (float) (((double) x + (double) y) * 0.366025388240814);
      double x1 = (double) x + (double) num1;
      float x2 = y + num1;
      int num2 = Noise.FastFloor((float) x1);
      int num3 = Noise.FastFloor(x2);
      float num4 = (float) (num2 + num3) * 0.2113249f;
      float num5 = (float) num2 - num4;
      float num6 = (float) num3 - num4;
      float x3 = x - num5;
      float y1 = y - num6;
      int num7;
      int num8;
      if ((double) x3 > (double) y1)
      {
        num7 = 1;
        num8 = 0;
      }
      else
      {
        num7 = 0;
        num8 = 1;
      }
      float x4 = (float) ((double) x3 - (double) num7 + 0.211324870586395);
      float y2 = (float) ((double) y1 - (double) num8 + 0.211324870586395);
      float x5 = (float) ((double) x3 - 1.0 + 0.422649741172791);
      float y3 = (float) ((double) y1 - 1.0 + 0.422649741172791);
      int num9 = num2 % 256;
      int index = num3 % 256;
      float num10 = (float) (0.5 - (double) x3 * (double) x3 - (double) y1 * (double) y1);
      float num11;
      if ((double) num10 < 0.0)
      {
        num11 = 0.0f;
      }
      else
      {
        float num12 = num10 * num10;
        num11 = num12 * num12 * Noise.grad((int) Noise.perm[num9 + (int) Noise.perm[index]], x3, y1);
      }
      float num13 = (float) (0.5 - (double) x4 * (double) x4 - (double) y2 * (double) y2);
      float num14;
      if ((double) num13 < 0.0)
      {
        num14 = 0.0f;
      }
      else
      {
        float num15 = num13 * num13;
        num14 = num15 * num15 * Noise.grad((int) Noise.perm[num9 + num7 + (int) Noise.perm[index + num8]], x4, y2);
      }
      float num16 = (float) (0.5 - (double) x5 * (double) x5 - (double) y3 * (double) y3);
      float num17;
      if ((double) num16 < 0.0)
      {
        num17 = 0.0f;
      }
      else
      {
        float num18 = num16 * num16;
        num17 = num18 * num18 * Noise.grad((int) Noise.perm[num9 + 1 + (int) Noise.perm[index + 1]], x5, y3);
      }
      return (float) (40.0 * ((double) num11 + (double) num14 + (double) num17));
    }

    private static int FastFloor(float x) => (double) x <= 0.0 ? (int) x - 1 : (int) x;

    private static float grad(int hash, float x)
    {
      int num1 = hash & 15;
      float num2 = 1f + (float) (num1 & 7);
      if ((num1 & 8) != 0)
        num2 = -num2;
      return num2 * x;
    }

    private static float grad(int hash, float x, float y)
    {
      int num1 = hash & 7;
      float num2 = num1 < 4 ? x : y;
      float num3 = num1 < 4 ? y : x;
      return (float) (((num1 & 1) != 0 ? -(double) num2 : (double) num2) + ((num1 & 2) != 0 ? -2.0 * (double) num3 : 2.0 * (double) num3));
    }

    private static float grad(int hash, float x, float y, float z)
    {
      int num1 = hash & 15;
      float num2 = num1 < 8 ? x : y;
      float num3 = num1 < 4 ? y : (num1 == 12 || num1 == 14 ? x : z);
      return (float) (((num1 & 1) != 0 ? -(double) num2 : (double) num2) + ((num1 & 2) != 0 ? -(double) num3 : (double) num3));
    }

    private static float grad(int hash, float x, float y, float z, float t)
    {
      int num1 = hash & 31;
      float num2 = num1 < 24 ? x : y;
      float num3 = num1 < 16 ? y : z;
      float num4 = num1 < 8 ? z : t;
      return (float) (((num1 & 1) != 0 ? -(double) num2 : (double) num2) + ((num1 & 2) != 0 ? -(double) num3 : (double) num3) + ((num1 & 4) != 0 ? -(double) num4 : (double) num4));
    }
  }
}
