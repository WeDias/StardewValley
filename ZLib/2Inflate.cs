// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.InflateCodes
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace Ionic.Zlib
{
  internal sealed class InflateCodes
  {
    private const int START = 0;
    private const int LEN = 1;
    private const int LENEXT = 2;
    private const int DIST = 3;
    private const int DISTEXT = 4;
    private const int COPY = 5;
    private const int LIT = 6;
    private const int WASH = 7;
    private const int END = 8;
    private const int BADCODE = 9;
    internal int mode;
    internal int len;
    internal int[] tree;
    internal int tree_index;
    internal int need;
    internal int lit;
    internal int bitsToGet;
    internal int dist;
    internal byte lbits;
    internal byte dbits;
    internal int[] ltree;
    internal int ltree_index;
    internal int[] dtree;
    internal int dtree_index;

    internal InflateCodes()
    {
    }

    internal void Init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index)
    {
      this.mode = 0;
      this.lbits = (byte) bl;
      this.dbits = (byte) bd;
      this.ltree = tl;
      this.ltree_index = tl_index;
      this.dtree = td;
      this.dtree_index = td_index;
      this.tree = (int[]) null;
    }

    internal int Process(InflateBlocks blocks, int r)
    {
      ZlibCodec codec = blocks._codec;
      int nextIn = codec.NextIn;
      int availableBytesIn = codec.AvailableBytesIn;
      int bitb = blocks.bitb;
      int bitk = blocks.bitk;
      int num1 = blocks.writeAt;
      int num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
      while (true)
      {
        switch (this.mode)
        {
          case 0:
            if (num2 >= 258 && availableBytesIn >= 10)
            {
              blocks.bitb = bitb;
              blocks.bitk = bitk;
              codec.AvailableBytesIn = availableBytesIn;
              codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
              codec.NextIn = nextIn;
              blocks.writeAt = num1;
              r = this.InflateFast((int) this.lbits, (int) this.dbits, this.ltree, this.ltree_index, this.dtree, this.dtree_index, blocks, codec);
              nextIn = codec.NextIn;
              availableBytesIn = codec.AvailableBytesIn;
              bitb = blocks.bitb;
              bitk = blocks.bitk;
              num1 = blocks.writeAt;
              num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
              int num3;
              switch (r)
              {
                case 0:
                  goto label_7;
                case 1:
                  num3 = 7;
                  break;
                default:
                  num3 = 9;
                  break;
              }
              this.mode = num3;
              continue;
            }
label_7:
            this.need = (int) this.lbits;
            this.tree = this.ltree;
            this.tree_index = this.ltree_index;
            this.mode = 1;
            goto case 1;
          case 1:
            int need1;
            for (need1 = this.need; bitk < need1; bitk += 8)
            {
              if (availableBytesIn != 0)
              {
                r = 0;
                --availableBytesIn;
                bitb |= ((int) codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << bitk;
              }
              else
              {
                blocks.bitb = bitb;
                blocks.bitk = bitk;
                codec.AvailableBytesIn = availableBytesIn;
                codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
                codec.NextIn = nextIn;
                blocks.writeAt = num1;
                return blocks.Flush(r);
              }
            }
            int index1 = (this.tree_index + (bitb & InternalInflateConstants.InflateMask[need1])) * 3;
            bitb >>= this.tree[index1 + 1];
            bitk -= this.tree[index1 + 1];
            int num4 = this.tree[index1];
            if (num4 == 0)
            {
              this.lit = this.tree[index1 + 2];
              this.mode = 6;
              continue;
            }
            if ((num4 & 16) != 0)
            {
              this.bitsToGet = num4 & 15;
              this.len = this.tree[index1 + 2];
              this.mode = 2;
              continue;
            }
            if ((num4 & 64) == 0)
            {
              this.need = num4;
              this.tree_index = index1 / 3 + this.tree[index1 + 2];
              continue;
            }
            if ((num4 & 32) != 0)
            {
              this.mode = 7;
              continue;
            }
            goto label_21;
          case 2:
            int bitsToGet1;
            for (bitsToGet1 = this.bitsToGet; bitk < bitsToGet1; bitk += 8)
            {
              if (availableBytesIn != 0)
              {
                r = 0;
                --availableBytesIn;
                bitb |= ((int) codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << bitk;
              }
              else
              {
                blocks.bitb = bitb;
                blocks.bitk = bitk;
                codec.AvailableBytesIn = availableBytesIn;
                codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
                codec.NextIn = nextIn;
                blocks.writeAt = num1;
                return blocks.Flush(r);
              }
            }
            this.len += bitb & InternalInflateConstants.InflateMask[bitsToGet1];
            bitb >>= bitsToGet1;
            bitk -= bitsToGet1;
            this.need = (int) this.dbits;
            this.tree = this.dtree;
            this.tree_index = this.dtree_index;
            this.mode = 3;
            goto case 3;
          case 3:
            int need2;
            for (need2 = this.need; bitk < need2; bitk += 8)
            {
              if (availableBytesIn != 0)
              {
                r = 0;
                --availableBytesIn;
                bitb |= ((int) codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << bitk;
              }
              else
              {
                blocks.bitb = bitb;
                blocks.bitk = bitk;
                codec.AvailableBytesIn = availableBytesIn;
                codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
                codec.NextIn = nextIn;
                blocks.writeAt = num1;
                return blocks.Flush(r);
              }
            }
            int index2 = (this.tree_index + (bitb & InternalInflateConstants.InflateMask[need2])) * 3;
            bitb >>= this.tree[index2 + 1];
            bitk -= this.tree[index2 + 1];
            int num5 = this.tree[index2];
            if ((num5 & 16) != 0)
            {
              this.bitsToGet = num5 & 15;
              this.dist = this.tree[index2 + 2];
              this.mode = 4;
              continue;
            }
            if ((num5 & 64) == 0)
            {
              this.need = num5;
              this.tree_index = index2 / 3 + this.tree[index2 + 2];
              continue;
            }
            goto label_37;
          case 4:
            int bitsToGet2;
            for (bitsToGet2 = this.bitsToGet; bitk < bitsToGet2; bitk += 8)
            {
              if (availableBytesIn != 0)
              {
                r = 0;
                --availableBytesIn;
                bitb |= ((int) codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << bitk;
              }
              else
              {
                blocks.bitb = bitb;
                blocks.bitk = bitk;
                codec.AvailableBytesIn = availableBytesIn;
                codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
                codec.NextIn = nextIn;
                blocks.writeAt = num1;
                return blocks.Flush(r);
              }
            }
            this.dist += bitb & InternalInflateConstants.InflateMask[bitsToGet2];
            bitb >>= bitsToGet2;
            bitk -= bitsToGet2;
            this.mode = 5;
            goto case 5;
          case 5:
            int num6 = num1 - this.dist;
            while (num6 < 0)
              num6 += blocks.end;
            for (; this.len != 0; --this.len)
            {
              if (num2 == 0)
              {
                if (num1 == blocks.end && blocks.readAt != 0)
                {
                  num1 = 0;
                  num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
                }
                if (num2 == 0)
                {
                  blocks.writeAt = num1;
                  r = blocks.Flush(r);
                  num1 = blocks.writeAt;
                  num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
                  if (num1 == blocks.end && blocks.readAt != 0)
                  {
                    num1 = 0;
                    num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
                  }
                  if (num2 == 0)
                  {
                    blocks.bitb = bitb;
                    blocks.bitk = bitk;
                    codec.AvailableBytesIn = availableBytesIn;
                    codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
                    codec.NextIn = nextIn;
                    blocks.writeAt = num1;
                    return blocks.Flush(r);
                  }
                }
              }
              blocks.window[num1++] = blocks.window[num6++];
              --num2;
              if (num6 == blocks.end)
                num6 = 0;
            }
            this.mode = 0;
            continue;
          case 6:
            if (num2 == 0)
            {
              if (num1 == blocks.end && blocks.readAt != 0)
              {
                num1 = 0;
                num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
              }
              if (num2 == 0)
              {
                blocks.writeAt = num1;
                r = blocks.Flush(r);
                num1 = blocks.writeAt;
                num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
                if (num1 == blocks.end && blocks.readAt != 0)
                {
                  num1 = 0;
                  num2 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
                }
                if (num2 == 0)
                  goto label_67;
              }
            }
            r = 0;
            blocks.window[num1++] = (byte) this.lit;
            --num2;
            this.mode = 0;
            continue;
          case 7:
            goto label_69;
          case 8:
            goto label_74;
          case 9:
            goto label_75;
          default:
            goto label_76;
        }
      }
label_21:
      this.mode = 9;
      codec.Message = "invalid literal/length code";
      r = -3;
      blocks.bitb = bitb;
      blocks.bitk = bitk;
      codec.AvailableBytesIn = availableBytesIn;
      codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
      codec.NextIn = nextIn;
      blocks.writeAt = num1;
      return blocks.Flush(r);
label_37:
      this.mode = 9;
      codec.Message = "invalid distance code";
      r = -3;
      blocks.bitb = bitb;
      blocks.bitk = bitk;
      codec.AvailableBytesIn = availableBytesIn;
      codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
      codec.NextIn = nextIn;
      blocks.writeAt = num1;
      return blocks.Flush(r);
label_67:
      blocks.bitb = bitb;
      blocks.bitk = bitk;
      codec.AvailableBytesIn = availableBytesIn;
      codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
      codec.NextIn = nextIn;
      blocks.writeAt = num1;
      return blocks.Flush(r);
label_69:
      if (bitk > 7)
      {
        bitk -= 8;
        ++availableBytesIn;
        --nextIn;
      }
      blocks.writeAt = num1;
      r = blocks.Flush(r);
      num1 = blocks.writeAt;
      int num7 = num1 < blocks.readAt ? blocks.readAt - num1 - 1 : blocks.end - num1;
      if (blocks.readAt != blocks.writeAt)
      {
        blocks.bitb = bitb;
        blocks.bitk = bitk;
        codec.AvailableBytesIn = availableBytesIn;
        codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
        codec.NextIn = nextIn;
        blocks.writeAt = num1;
        return blocks.Flush(r);
      }
      this.mode = 8;
label_74:
      r = 1;
      blocks.bitb = bitb;
      blocks.bitk = bitk;
      codec.AvailableBytesIn = availableBytesIn;
      codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
      codec.NextIn = nextIn;
      blocks.writeAt = num1;
      return blocks.Flush(r);
label_75:
      r = -3;
      blocks.bitb = bitb;
      blocks.bitk = bitk;
      codec.AvailableBytesIn = availableBytesIn;
      codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
      codec.NextIn = nextIn;
      blocks.writeAt = num1;
      return blocks.Flush(r);
label_76:
      r = -2;
      blocks.bitb = bitb;
      blocks.bitk = bitk;
      codec.AvailableBytesIn = availableBytesIn;
      codec.TotalBytesIn += (long) (nextIn - codec.NextIn);
      codec.NextIn = nextIn;
      blocks.writeAt = num1;
      return blocks.Flush(r);
    }

    internal int InflateFast(
      int bl,
      int bd,
      int[] tl,
      int tl_index,
      int[] td,
      int td_index,
      InflateBlocks s,
      ZlibCodec z)
    {
      int nextIn = z.NextIn;
      int availableBytesIn = z.AvailableBytesIn;
      int num1 = s.bitb;
      int num2 = s.bitk;
      int destinationIndex = s.writeAt;
      int num3 = destinationIndex < s.readAt ? s.readAt - destinationIndex - 1 : s.end - destinationIndex;
      int num4 = InternalInflateConstants.InflateMask[bl];
      int num5 = InternalInflateConstants.InflateMask[bd];
      do
      {
        for (; num2 < 20; num2 += 8)
        {
          --availableBytesIn;
          num1 |= ((int) z.InputBuffer[nextIn++] & (int) byte.MaxValue) << num2;
        }
        int num6 = num1 & num4;
        int[] numArray1 = tl;
        int num7 = tl_index;
        int index1 = (num7 + num6) * 3;
        int index2;
        if ((index2 = numArray1[index1]) == 0)
        {
          num1 >>= numArray1[index1 + 1];
          num2 -= numArray1[index1 + 1];
          s.window[destinationIndex++] = (byte) numArray1[index1 + 2];
          --num3;
        }
        else
        {
          do
          {
            num1 >>= numArray1[index1 + 1];
            num2 -= numArray1[index1 + 1];
            if ((index2 & 16) != 0)
            {
              int index3 = index2 & 15;
              int length1 = numArray1[index1 + 2] + (num1 & InternalInflateConstants.InflateMask[index3]);
              int num8 = num1 >> index3;
              int num9;
              for (num9 = num2 - index3; num9 < 15; num9 += 8)
              {
                --availableBytesIn;
                num8 |= ((int) z.InputBuffer[nextIn++] & (int) byte.MaxValue) << num9;
              }
              int num10 = num8 & num5;
              int[] numArray2 = td;
              int num11 = td_index;
              int index4 = (num11 + num10) * 3;
              int index5 = numArray2[index4];
              while (true)
              {
                num8 >>= numArray2[index4 + 1];
                num9 -= numArray2[index4 + 1];
                if ((index5 & 16) == 0)
                {
                  if ((index5 & 64) == 0)
                  {
                    num10 = num10 + numArray2[index4 + 2] + (num8 & InternalInflateConstants.InflateMask[index5]);
                    index4 = (num11 + num10) * 3;
                    index5 = numArray2[index4];
                  }
                  else
                    goto label_30;
                }
                else
                  break;
              }
              int index6;
              for (index6 = index5 & 15; num9 < index6; num9 += 8)
              {
                --availableBytesIn;
                num8 |= ((int) z.InputBuffer[nextIn++] & (int) byte.MaxValue) << num9;
              }
              int num12 = numArray2[index4 + 2] + (num8 & InternalInflateConstants.InflateMask[index6]);
              num1 = num8 >> index6;
              num2 = num9 - index6;
              num3 -= length1;
              int sourceIndex1;
              int num13;
              if (destinationIndex >= num12)
              {
                int sourceIndex2 = destinationIndex - num12;
                if (destinationIndex - sourceIndex2 > 0 && 2 > destinationIndex - sourceIndex2)
                {
                  byte[] window1 = s.window;
                  int index7 = destinationIndex;
                  int num14 = index7 + 1;
                  byte[] window2 = s.window;
                  int index8 = sourceIndex2;
                  int num15 = index8 + 1;
                  int num16 = (int) window2[index8];
                  window1[index7] = (byte) num16;
                  byte[] window3 = s.window;
                  int index9 = num14;
                  destinationIndex = index9 + 1;
                  byte[] window4 = s.window;
                  int index10 = num15;
                  sourceIndex1 = index10 + 1;
                  int num17 = (int) window4[index10];
                  window3[index9] = (byte) num17;
                  length1 -= 2;
                }
                else
                {
                  Array.Copy((Array) s.window, sourceIndex2, (Array) s.window, destinationIndex, 2);
                  destinationIndex += 2;
                  sourceIndex1 = sourceIndex2 + 2;
                  length1 -= 2;
                }
              }
              else
              {
                sourceIndex1 = destinationIndex - num12;
                do
                {
                  sourceIndex1 += s.end;
                }
                while (sourceIndex1 < 0);
                int length2 = s.end - sourceIndex1;
                if (length1 > length2)
                {
                  length1 -= length2;
                  if (destinationIndex - sourceIndex1 > 0 && length2 > destinationIndex - sourceIndex1)
                  {
                    do
                    {
                      s.window[destinationIndex++] = s.window[sourceIndex1++];
                    }
                    while (--length2 != 0);
                  }
                  else
                  {
                    Array.Copy((Array) s.window, sourceIndex1, (Array) s.window, destinationIndex, length2);
                    destinationIndex += length2;
                    num13 = sourceIndex1 + length2;
                  }
                  sourceIndex1 = 0;
                }
              }
              if (destinationIndex - sourceIndex1 > 0 && length1 > destinationIndex - sourceIndex1)
              {
                do
                {
                  s.window[destinationIndex++] = s.window[sourceIndex1++];
                }
                while (--length1 != 0);
                goto label_37;
              }
              else
              {
                Array.Copy((Array) s.window, sourceIndex1, (Array) s.window, destinationIndex, length1);
                destinationIndex += length1;
                num13 = sourceIndex1 + length1;
                goto label_37;
              }
label_30:
              z.Message = "invalid distance code";
              int num18 = z.AvailableBytesIn - availableBytesIn;
              int num19 = num9 >> 3 < num18 ? num9 >> 3 : num18;
              int num20 = availableBytesIn + num19;
              int num21 = nextIn - num19;
              int num22 = num9 - (num19 << 3);
              s.bitb = num8;
              s.bitk = num22;
              z.AvailableBytesIn = num20;
              z.TotalBytesIn += (long) (num21 - z.NextIn);
              z.NextIn = num21;
              s.writeAt = destinationIndex;
              return -3;
            }
            if ((index2 & 64) == 0)
            {
              num6 = num6 + numArray1[index1 + 2] + (num1 & InternalInflateConstants.InflateMask[index2]);
              index1 = (num7 + num6) * 3;
            }
            else
              goto label_34;
          }
          while ((index2 = numArray1[index1]) != 0);
          num1 >>= numArray1[index1 + 1];
          num2 -= numArray1[index1 + 1];
          s.window[destinationIndex++] = (byte) numArray1[index1 + 2];
          --num3;
          goto label_37;
label_34:
          if ((index2 & 32) != 0)
          {
            int num23 = z.AvailableBytesIn - availableBytesIn;
            int num24 = num2 >> 3 < num23 ? num2 >> 3 : num23;
            int num25 = availableBytesIn + num24;
            int num26 = nextIn - num24;
            int num27 = num2 - (num24 << 3);
            s.bitb = num1;
            s.bitk = num27;
            z.AvailableBytesIn = num25;
            z.TotalBytesIn += (long) (num26 - z.NextIn);
            z.NextIn = num26;
            s.writeAt = destinationIndex;
            return 1;
          }
          z.Message = "invalid literal/length code";
          int num28 = z.AvailableBytesIn - availableBytesIn;
          int num29 = num2 >> 3 < num28 ? num2 >> 3 : num28;
          int num30 = availableBytesIn + num29;
          int num31 = nextIn - num29;
          int num32 = num2 - (num29 << 3);
          s.bitb = num1;
          s.bitk = num32;
          z.AvailableBytesIn = num30;
          z.TotalBytesIn += (long) (num31 - z.NextIn);
          z.NextIn = num31;
          s.writeAt = destinationIndex;
          return -3;
        }
label_37:;
      }
      while (num3 >= 258 && availableBytesIn >= 10);
      int num33 = z.AvailableBytesIn - availableBytesIn;
      int num34 = num2 >> 3 < num33 ? num2 >> 3 : num33;
      int num35 = availableBytesIn + num34;
      int num36 = nextIn - num34;
      int num37 = num2 - (num34 << 3);
      s.bitb = num1;
      s.bitk = num37;
      z.AvailableBytesIn = num35;
      z.TotalBytesIn += (long) (num36 - z.NextIn);
      z.NextIn = num36;
      s.writeAt = destinationIndex;
      return 0;
    }
  }
}
