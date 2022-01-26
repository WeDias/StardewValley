// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.InflateBlocks
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace Ionic.Zlib
{
  internal sealed class InflateBlocks
  {
    private const int MANY = 1440;
    internal static readonly int[] border = new int[19]
    {
      16,
      17,
      18,
      0,
      8,
      7,
      9,
      6,
      10,
      5,
      11,
      4,
      12,
      3,
      13,
      2,
      14,
      1,
      15
    };
    private InflateBlocks.InflateBlockMode mode;
    internal int left;
    internal int table;
    internal int index;
    internal int[] blens;
    internal int[] bb = new int[1];
    internal int[] tb = new int[1];
    internal InflateCodes codes = new InflateCodes();
    internal int last;
    internal ZlibCodec _codec;
    internal int bitk;
    internal int bitb;
    internal int[] hufts;
    internal byte[] window;
    internal int end;
    internal int readAt;
    internal int writeAt;
    internal object checkfn;
    internal uint check;
    internal InfTree inftree = new InfTree();

    internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
    {
      this._codec = codec;
      this.hufts = new int[4320];
      this.window = new byte[w];
      this.end = w;
      this.checkfn = checkfn;
      this.mode = InflateBlocks.InflateBlockMode.TYPE;
      int num = (int) this.Reset();
    }

    internal uint Reset()
    {
      int check = (int) this.check;
      this.mode = InflateBlocks.InflateBlockMode.TYPE;
      this.bitk = 0;
      this.bitb = 0;
      this.readAt = this.writeAt = 0;
      if (this.checkfn == null)
        return (uint) check;
      this._codec._Adler32 = this.check = Adler.Adler32(0U, (byte[]) null, 0, 0);
      return (uint) check;
    }

    internal int Process(int r)
    {
      int nextIn = this._codec.NextIn;
      int availableBytesIn = this._codec.AvailableBytesIn;
      int num1 = this.bitb;
      int num2 = this.bitk;
      int destinationIndex = this.writeAt;
      int num3 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
      int num4;
      int num5;
      while (true)
      {
        int length1;
        do
        {
          switch (this.mode)
          {
            case InflateBlocks.InflateBlockMode.TYPE:
              for (; num2 < 3; num2 += 8)
              {
                if (availableBytesIn != 0)
                {
                  r = 0;
                  --availableBytesIn;
                  num1 |= ((int) this._codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << num2;
                }
                else
                {
                  this.bitb = num1;
                  this.bitk = num2;
                  this._codec.AvailableBytesIn = availableBytesIn;
                  this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                  this._codec.NextIn = nextIn;
                  this.writeAt = destinationIndex;
                  return this.Flush(r);
                }
              }
              int num6 = num1 & 7;
              this.last = num6 & 1;
              switch ((uint) num6 >> 1)
              {
                case 0:
                  int num7 = num1 >> 3;
                  int num8 = num2 - 3;
                  int num9 = num8 & 7;
                  num1 = num7 >> num9;
                  num2 = num8 - num9;
                  this.mode = InflateBlocks.InflateBlockMode.LENS;
                  continue;
                case 1:
                  int[] bl1 = new int[1];
                  int[] bd1 = new int[1];
                  int[][] tl1 = new int[1][];
                  int[][] td1 = new int[1][];
                  InfTree.inflate_trees_fixed(bl1, bd1, tl1, td1, this._codec);
                  this.codes.Init(bl1[0], bd1[0], tl1[0], 0, td1[0], 0);
                  num1 >>= 3;
                  num2 -= 3;
                  this.mode = InflateBlocks.InflateBlockMode.CODES;
                  continue;
                case 2:
                  num1 >>= 3;
                  num2 -= 3;
                  this.mode = InflateBlocks.InflateBlockMode.TABLE;
                  continue;
                case 3:
                  int num10 = num1 >> 3;
                  int num11 = num2 - 3;
                  this.mode = InflateBlocks.InflateBlockMode.BAD;
                  this._codec.Message = "invalid block type";
                  r = -3;
                  this.bitb = num10;
                  this.bitk = num11;
                  this._codec.AvailableBytesIn = availableBytesIn;
                  this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                  this._codec.NextIn = nextIn;
                  this.writeAt = destinationIndex;
                  return this.Flush(r);
                default:
                  continue;
              }
            case InflateBlocks.InflateBlockMode.LENS:
              for (; num2 < 32; num2 += 8)
              {
                if (availableBytesIn != 0)
                {
                  r = 0;
                  --availableBytesIn;
                  num1 |= ((int) this._codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << num2;
                }
                else
                {
                  this.bitb = num1;
                  this.bitk = num2;
                  this._codec.AvailableBytesIn = availableBytesIn;
                  this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                  this._codec.NextIn = nextIn;
                  this.writeAt = destinationIndex;
                  return this.Flush(r);
                }
              }
              if ((~num1 >> 16 & (int) ushort.MaxValue) != (num1 & (int) ushort.MaxValue))
              {
                this.mode = InflateBlocks.InflateBlockMode.BAD;
                this._codec.Message = "invalid stored block lengths";
                r = -3;
                this.bitb = num1;
                this.bitk = num2;
                this._codec.AvailableBytesIn = availableBytesIn;
                this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                this._codec.NextIn = nextIn;
                this.writeAt = destinationIndex;
                return this.Flush(r);
              }
              this.left = num1 & (int) ushort.MaxValue;
              num1 = num2 = 0;
              this.mode = this.left != 0 ? InflateBlocks.InflateBlockMode.STORED : (this.last != 0 ? InflateBlocks.InflateBlockMode.DRY : InflateBlocks.InflateBlockMode.TYPE);
              continue;
            case InflateBlocks.InflateBlockMode.STORED:
              if (availableBytesIn == 0)
              {
                this.bitb = num1;
                this.bitk = num2;
                this._codec.AvailableBytesIn = availableBytesIn;
                this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                this._codec.NextIn = nextIn;
                this.writeAt = destinationIndex;
                return this.Flush(r);
              }
              if (num3 == 0)
              {
                if (destinationIndex == this.end && this.readAt != 0)
                {
                  destinationIndex = 0;
                  num3 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
                }
                if (num3 == 0)
                {
                  this.writeAt = destinationIndex;
                  r = this.Flush(r);
                  destinationIndex = this.writeAt;
                  num3 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
                  if (destinationIndex == this.end && this.readAt != 0)
                  {
                    destinationIndex = 0;
                    num3 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
                  }
                  if (num3 == 0)
                  {
                    this.bitb = num1;
                    this.bitk = num2;
                    this._codec.AvailableBytesIn = availableBytesIn;
                    this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                    this._codec.NextIn = nextIn;
                    this.writeAt = destinationIndex;
                    return this.Flush(r);
                  }
                }
              }
              r = 0;
              length1 = this.left;
              if (length1 > availableBytesIn)
                length1 = availableBytesIn;
              if (length1 > num3)
                length1 = num3;
              Array.Copy((Array) this._codec.InputBuffer, nextIn, (Array) this.window, destinationIndex, length1);
              nextIn += length1;
              availableBytesIn -= length1;
              destinationIndex += length1;
              num3 -= length1;
              continue;
            case InflateBlocks.InflateBlockMode.TABLE:
              goto label_37;
            case InflateBlocks.InflateBlockMode.BTREE:
              goto label_49;
            case InflateBlocks.InflateBlockMode.DTREE:
              goto label_57;
            case InflateBlocks.InflateBlockMode.CODES:
              goto label_79;
            case InflateBlocks.InflateBlockMode.DRY:
              goto label_84;
            case InflateBlocks.InflateBlockMode.DONE:
              goto label_87;
            case InflateBlocks.InflateBlockMode.BAD:
              goto label_88;
            default:
              goto label_89;
          }
        }
        while ((this.left -= length1) != 0);
        this.mode = this.last != 0 ? InflateBlocks.InflateBlockMode.DRY : InflateBlocks.InflateBlockMode.TYPE;
        continue;
label_37:
        for (; num2 < 14; num2 += 8)
        {
          if (availableBytesIn != 0)
          {
            r = 0;
            --availableBytesIn;
            num1 |= ((int) this._codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << num2;
          }
          else
          {
            this.bitb = num1;
            this.bitk = num2;
            this._codec.AvailableBytesIn = availableBytesIn;
            this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
            this._codec.NextIn = nextIn;
            this.writeAt = destinationIndex;
            return this.Flush(r);
          }
        }
        int num12;
        this.table = num12 = num1 & 16383;
        if ((num12 & 31) <= 29 && (num12 >> 5 & 31) <= 29)
        {
          int length2 = 258 + (num12 & 31) + (num12 >> 5 & 31);
          if (this.blens == null || this.blens.Length < length2)
            this.blens = new int[length2];
          else
            Array.Clear((Array) this.blens, 0, length2);
          num1 >>= 14;
          num2 -= 14;
          this.index = 0;
          this.mode = InflateBlocks.InflateBlockMode.BTREE;
        }
        else
          break;
label_49:
        while (this.index < 4 + (this.table >> 10))
        {
          for (; num2 < 3; num2 += 8)
          {
            if (availableBytesIn != 0)
            {
              r = 0;
              --availableBytesIn;
              num1 |= ((int) this._codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << num2;
            }
            else
            {
              this.bitb = num1;
              this.bitk = num2;
              this._codec.AvailableBytesIn = availableBytesIn;
              this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
              this._codec.NextIn = nextIn;
              this.writeAt = destinationIndex;
              return this.Flush(r);
            }
          }
          this.blens[InflateBlocks.border[this.index++]] = num1 & 7;
          num1 >>= 3;
          num2 -= 3;
        }
        while (this.index < 19)
          this.blens[InflateBlocks.border[this.index++]] = 0;
        this.bb[0] = 7;
        num4 = this.inftree.inflate_trees_bits(this.blens, this.bb, this.tb, this.hufts, this._codec);
        if (num4 == 0)
        {
          this.index = 0;
          this.mode = InflateBlocks.InflateBlockMode.DTREE;
        }
        else
          goto label_53;
label_57:
        while (true)
        {
          int table1 = this.table;
          if (this.index < 258 + (table1 & 31) + (table1 >> 5 & 31))
          {
            int index1;
            for (index1 = this.bb[0]; num2 < index1; num2 += 8)
            {
              if (availableBytesIn != 0)
              {
                r = 0;
                --availableBytesIn;
                num1 |= ((int) this._codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << num2;
              }
              else
              {
                this.bitb = num1;
                this.bitk = num2;
                this._codec.AvailableBytesIn = availableBytesIn;
                this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                this._codec.NextIn = nextIn;
                this.writeAt = destinationIndex;
                return this.Flush(r);
              }
            }
            int huft1 = this.hufts[(this.tb[0] + (num1 & InternalInflateConstants.InflateMask[index1])) * 3 + 1];
            int huft2 = this.hufts[(this.tb[0] + (num1 & InternalInflateConstants.InflateMask[huft1])) * 3 + 2];
            if (huft2 < 16)
            {
              num1 >>= huft1;
              num2 -= huft1;
              this.blens[this.index++] = huft2;
            }
            else
            {
              int index2 = huft2 == 18 ? 7 : huft2 - 14;
              int num13 = huft2 == 18 ? 11 : 3;
              for (; num2 < huft1 + index2; num2 += 8)
              {
                if (availableBytesIn != 0)
                {
                  r = 0;
                  --availableBytesIn;
                  num1 |= ((int) this._codec.InputBuffer[nextIn++] & (int) byte.MaxValue) << num2;
                }
                else
                {
                  this.bitb = num1;
                  this.bitk = num2;
                  this._codec.AvailableBytesIn = availableBytesIn;
                  this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
                  this._codec.NextIn = nextIn;
                  this.writeAt = destinationIndex;
                  return this.Flush(r);
                }
              }
              int num14 = num1 >> huft1;
              int num15 = num2 - huft1;
              int num16 = num13 + (num14 & InternalInflateConstants.InflateMask[index2]);
              num1 = num14 >> index2;
              num2 = num15 - index2;
              int index3 = this.index;
              int table2 = this.table;
              if (index3 + num16 <= 258 + (table2 & 31) + (table2 >> 5 & 31) && (huft2 != 16 || index3 >= 1))
              {
                int num17 = huft2 == 16 ? this.blens[index3 - 1] : 0;
                do
                {
                  this.blens[index3++] = num17;
                }
                while (--num16 != 0);
                this.index = index3;
              }
              else
                goto label_71;
            }
          }
          else
            break;
        }
        this.tb[0] = -1;
        int[] bl2 = new int[1]{ 9 };
        int[] bd2 = new int[1]{ 6 };
        int[] tl2 = new int[1];
        int[] td2 = new int[1];
        int table = this.table;
        num5 = this.inftree.inflate_trees_dynamic(257 + (table & 31), 1 + (table >> 5 & 31), this.blens, bl2, bd2, tl2, td2, this.hufts, this._codec);
        switch (num5)
        {
          case -3:
            goto label_76;
          case 0:
            this.codes.Init(bl2[0], bd2[0], this.hufts, tl2[0], this.hufts, td2[0]);
            this.mode = InflateBlocks.InflateBlockMode.CODES;
            break;
          default:
            goto label_77;
        }
label_79:
        this.bitb = num1;
        this.bitk = num2;
        this._codec.AvailableBytesIn = availableBytesIn;
        this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
        this._codec.NextIn = nextIn;
        this.writeAt = destinationIndex;
        r = this.codes.Process(this, r);
        if (r == 1)
        {
          r = 0;
          nextIn = this._codec.NextIn;
          availableBytesIn = this._codec.AvailableBytesIn;
          num1 = this.bitb;
          num2 = this.bitk;
          destinationIndex = this.writeAt;
          num3 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
          if (this.last == 0)
            this.mode = InflateBlocks.InflateBlockMode.TYPE;
          else
            goto label_83;
        }
        else
          goto label_80;
      }
      this.mode = InflateBlocks.InflateBlockMode.BAD;
      this._codec.Message = "too many length or distance symbols";
      r = -3;
      this.bitb = num1;
      this.bitk = num2;
      this._codec.AvailableBytesIn = availableBytesIn;
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_53:
      r = num4;
      if (r == -3)
      {
        this.blens = (int[]) null;
        this.mode = InflateBlocks.InflateBlockMode.BAD;
      }
      this.bitb = num1;
      this.bitk = num2;
      this._codec.AvailableBytesIn = availableBytesIn;
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_71:
      this.blens = (int[]) null;
      this.mode = InflateBlocks.InflateBlockMode.BAD;
      this._codec.Message = "invalid bit length repeat";
      r = -3;
      this.bitb = num1;
      this.bitk = num2;
      this._codec.AvailableBytesIn = availableBytesIn;
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_76:
      this.blens = (int[]) null;
      this.mode = InflateBlocks.InflateBlockMode.BAD;
label_77:
      r = num5;
      this.bitb = num1;
      this.bitk = num2;
      this._codec.AvailableBytesIn = availableBytesIn;
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_80:
      return this.Flush(r);
label_83:
      this.mode = InflateBlocks.InflateBlockMode.DRY;
label_84:
      this.writeAt = destinationIndex;
      r = this.Flush(r);
      destinationIndex = this.writeAt;
      int num18 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
      if (this.readAt != this.writeAt)
      {
        this.bitb = num1;
        this.bitk = num2;
        this._codec.AvailableBytesIn = availableBytesIn;
        this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
        this._codec.NextIn = nextIn;
        this.writeAt = destinationIndex;
        return this.Flush(r);
      }
      this.mode = InflateBlocks.InflateBlockMode.DONE;
label_87:
      r = 1;
      this.bitb = num1;
      this.bitk = num2;
      this._codec.AvailableBytesIn = availableBytesIn;
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_88:
      r = -3;
      this.bitb = num1;
      this.bitk = num2;
      this._codec.AvailableBytesIn = availableBytesIn;
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_89:
      r = -2;
      this.bitb = num1;
      this.bitk = num2;
      this._codec.AvailableBytesIn = availableBytesIn;
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this.writeAt = destinationIndex;
      return this.Flush(r);
    }

    internal void Free()
    {
      int num = (int) this.Reset();
      this.window = (byte[]) null;
      this.hufts = (int[]) null;
    }

    internal void SetDictionary(byte[] d, int start, int n)
    {
      Array.Copy((Array) d, start, (Array) this.window, 0, n);
      this.readAt = this.writeAt = n;
    }

    internal int SyncPoint() => this.mode != InflateBlocks.InflateBlockMode.LENS ? 0 : 1;

    internal int Flush(int r)
    {
      for (int index = 0; index < 2; ++index)
      {
        int num = index != 0 ? this.writeAt - this.readAt : (this.readAt <= this.writeAt ? this.writeAt : this.end) - this.readAt;
        if (num == 0)
        {
          if (r == -5)
            r = 0;
          return r;
        }
        if (num > this._codec.AvailableBytesOut)
          num = this._codec.AvailableBytesOut;
        if (num != 0 && r == -5)
          r = 0;
        this._codec.AvailableBytesOut -= num;
        this._codec.TotalBytesOut += (long) num;
        if (this.checkfn != null)
          this._codec._Adler32 = this.check = Adler.Adler32(this.check, this.window, this.readAt, num);
        Array.Copy((Array) this.window, this.readAt, (Array) this._codec.OutputBuffer, this._codec.NextOut, num);
        this._codec.NextOut += num;
        this.readAt += num;
        if (this.readAt == this.end && index == 0)
        {
          this.readAt = 0;
          if (this.writeAt == this.end)
            this.writeAt = 0;
        }
        else
          ++index;
      }
      return r;
    }

    private enum InflateBlockMode
    {
      TYPE,
      LENS,
      STORED,
      TABLE,
      BTREE,
      DTREE,
      CODES,
      DRY,
      DONE,
      BAD,
    }
  }
}
