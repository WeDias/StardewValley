// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.InflateManager
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace Ionic.Zlib
{
  internal sealed class InflateManager
  {
    private const int PRESET_DICT = 32;
    private const int Z_DEFLATED = 8;
    private InflateManager.InflateManagerMode mode;
    internal ZlibCodec _codec;
    internal int method;
    internal uint computedCheck;
    internal uint expectedCheck;
    internal int marker;
    private bool _handleRfc1950HeaderBytes = true;
    internal int wbits;
    internal InflateBlocks blocks;
    private static readonly byte[] mark = new byte[4]
    {
      (byte) 0,
      (byte) 0,
      byte.MaxValue,
      byte.MaxValue
    };

    internal bool HandleRfc1950HeaderBytes
    {
      get => this._handleRfc1950HeaderBytes;
      set => this._handleRfc1950HeaderBytes = value;
    }

    public InflateManager()
    {
    }

    public InflateManager(bool expectRfc1950HeaderBytes) => this._handleRfc1950HeaderBytes = expectRfc1950HeaderBytes;

    internal int Reset()
    {
      this._codec.TotalBytesIn = this._codec.TotalBytesOut = 0L;
      this._codec.Message = (string) null;
      this.mode = this.HandleRfc1950HeaderBytes ? InflateManager.InflateManagerMode.METHOD : InflateManager.InflateManagerMode.BLOCKS;
      int num = (int) this.blocks.Reset();
      return 0;
    }

    internal int End()
    {
      if (this.blocks != null)
        this.blocks.Free();
      this.blocks = (InflateBlocks) null;
      return 0;
    }

    internal int Initialize(ZlibCodec codec, int w)
    {
      this._codec = codec;
      this._codec.Message = (string) null;
      this.blocks = (InflateBlocks) null;
      if (w < 8 || w > 15)
      {
        this.End();
        throw new ZlibException("Bad window size.");
      }
      this.wbits = w;
      this.blocks = new InflateBlocks(codec, this.HandleRfc1950HeaderBytes ? (object) this : (object) (InflateManager) null, 1 << w);
      this.Reset();
      return 0;
    }

    internal int Inflate(FlushType flush)
    {
      if (this._codec.InputBuffer == null)
        throw new ZlibException("InputBuffer is null. ");
      int num1 = 0;
      int r = -5;
      while (true)
      {
        switch (this.mode)
        {
          case InflateManager.InflateManagerMode.METHOD:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              if (((this.method = (int) this._codec.InputBuffer[this._codec.NextIn++]) & 15) != 8)
              {
                this.mode = InflateManager.InflateManagerMode.BAD;
                this._codec.Message = string.Format("unknown compression method (0x{0:X2})", (object) this.method);
                this.marker = 5;
                continue;
              }
              if ((this.method >> 4) + 8 > this.wbits)
              {
                this.mode = InflateManager.InflateManagerMode.BAD;
                this._codec.Message = string.Format("invalid window size ({0})", (object) ((this.method >> 4) + 8));
                this.marker = 5;
                continue;
              }
              this.mode = InflateManager.InflateManagerMode.FLAG;
              continue;
            }
            goto label_5;
          case InflateManager.InflateManagerMode.FLAG:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              int num2 = (int) this._codec.InputBuffer[this._codec.NextIn++] & (int) byte.MaxValue;
              if (((this.method << 8) + num2) % 31 != 0)
              {
                this.mode = InflateManager.InflateManagerMode.BAD;
                this._codec.Message = "incorrect header check";
                this.marker = 5;
                continue;
              }
              this.mode = (num2 & 32) == 0 ? InflateManager.InflateManagerMode.BLOCKS : InflateManager.InflateManagerMode.DICT4;
              continue;
            }
            goto label_12;
          case InflateManager.InflateManagerMode.DICT4:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              this.expectedCheck = (uint) ((ulong) ((int) this._codec.InputBuffer[this._codec.NextIn++] << 24) & 4278190080UL);
              this.mode = InflateManager.InflateManagerMode.DICT3;
              continue;
            }
            goto label_17;
          case InflateManager.InflateManagerMode.DICT3:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              this.expectedCheck += (uint) ((int) this._codec.InputBuffer[this._codec.NextIn++] << 16 & 16711680);
              this.mode = InflateManager.InflateManagerMode.DICT2;
              continue;
            }
            goto label_20;
          case InflateManager.InflateManagerMode.DICT2:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              this.expectedCheck += (uint) ((int) this._codec.InputBuffer[this._codec.NextIn++] << 8 & 65280);
              this.mode = InflateManager.InflateManagerMode.DICT1;
              continue;
            }
            goto label_23;
          case InflateManager.InflateManagerMode.DICT1:
            goto label_25;
          case InflateManager.InflateManagerMode.DICT0:
            goto label_28;
          case InflateManager.InflateManagerMode.BLOCKS:
            r = this.blocks.Process(r);
            if (r == -3)
            {
              this.mode = InflateManager.InflateManagerMode.BAD;
              this.marker = 0;
              continue;
            }
            if (r == 0)
              r = num1;
            if (r == 1)
            {
              r = num1;
              this.computedCheck = this.blocks.Reset();
              if (this.HandleRfc1950HeaderBytes)
              {
                this.mode = InflateManager.InflateManagerMode.CHECK4;
                continue;
              }
              goto label_36;
            }
            else
              goto label_34;
          case InflateManager.InflateManagerMode.CHECK4:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              this.expectedCheck = (uint) ((ulong) ((int) this._codec.InputBuffer[this._codec.NextIn++] << 24) & 4278190080UL);
              this.mode = InflateManager.InflateManagerMode.CHECK3;
              continue;
            }
            goto label_39;
          case InflateManager.InflateManagerMode.CHECK3:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              this.expectedCheck += (uint) ((int) this._codec.InputBuffer[this._codec.NextIn++] << 16 & 16711680);
              this.mode = InflateManager.InflateManagerMode.CHECK2;
              continue;
            }
            goto label_42;
          case InflateManager.InflateManagerMode.CHECK2:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              this.expectedCheck += (uint) ((int) this._codec.InputBuffer[this._codec.NextIn++] << 8 & 65280);
              this.mode = InflateManager.InflateManagerMode.CHECK1;
              continue;
            }
            goto label_45;
          case InflateManager.InflateManagerMode.CHECK1:
            if (this._codec.AvailableBytesIn != 0)
            {
              r = num1;
              --this._codec.AvailableBytesIn;
              ++this._codec.TotalBytesIn;
              this.expectedCheck += (uint) this._codec.InputBuffer[this._codec.NextIn++] & (uint) byte.MaxValue;
              if ((int) this.computedCheck != (int) this.expectedCheck)
              {
                this.mode = InflateManager.InflateManagerMode.BAD;
                this._codec.Message = "incorrect data check";
                this.marker = 5;
                continue;
              }
              goto label_51;
            }
            else
              goto label_48;
          case InflateManager.InflateManagerMode.DONE:
            goto label_52;
          case InflateManager.InflateManagerMode.BAD:
            goto label_53;
          default:
            goto label_54;
        }
      }
label_5:
      return r;
label_12:
      return r;
label_17:
      return r;
label_20:
      return r;
label_23:
      return r;
label_25:
      if (this._codec.AvailableBytesIn == 0)
        return r;
      --this._codec.AvailableBytesIn;
      ++this._codec.TotalBytesIn;
      this.expectedCheck += (uint) this._codec.InputBuffer[this._codec.NextIn++] & (uint) byte.MaxValue;
      this._codec._Adler32 = this.expectedCheck;
      this.mode = InflateManager.InflateManagerMode.DICT0;
      return 2;
label_28:
      this.mode = InflateManager.InflateManagerMode.BAD;
      this._codec.Message = "need dictionary";
      this.marker = 0;
      return -2;
label_34:
      return r;
label_36:
      this.mode = InflateManager.InflateManagerMode.DONE;
      return 1;
label_39:
      return r;
label_42:
      return r;
label_45:
      return r;
label_48:
      return r;
label_51:
      this.mode = InflateManager.InflateManagerMode.DONE;
      return 1;
label_52:
      return 1;
label_53:
      throw new ZlibException(string.Format("Bad state ({0})", (object) this._codec.Message));
label_54:
      throw new ZlibException("Stream error.");
    }

    internal int SetDictionary(byte[] dictionary)
    {
      int start = 0;
      int n = dictionary.Length;
      if (this.mode != InflateManager.InflateManagerMode.DICT0)
        throw new ZlibException("Stream error.");
      if ((int) Adler.Adler32(1U, dictionary, 0, dictionary.Length) != (int) this._codec._Adler32)
        return -3;
      this._codec._Adler32 = Adler.Adler32(0U, (byte[]) null, 0, 0);
      if (n >= 1 << this.wbits)
      {
        n = (1 << this.wbits) - 1;
        start = dictionary.Length - n;
      }
      this.blocks.SetDictionary(dictionary, start, n);
      this.mode = InflateManager.InflateManagerMode.BLOCKS;
      return 0;
    }

    internal int Sync()
    {
      if (this.mode != InflateManager.InflateManagerMode.BAD)
      {
        this.mode = InflateManager.InflateManagerMode.BAD;
        this.marker = 0;
      }
      int availableBytesIn;
      if ((availableBytesIn = this._codec.AvailableBytesIn) == 0)
        return -5;
      int nextIn = this._codec.NextIn;
      int index;
      for (index = this.marker; availableBytesIn != 0 && index < 4; --availableBytesIn)
      {
        if ((int) this._codec.InputBuffer[nextIn] == (int) InflateManager.mark[index])
          ++index;
        else
          index = this._codec.InputBuffer[nextIn] == (byte) 0 ? 4 - index : 0;
        ++nextIn;
      }
      this._codec.TotalBytesIn += (long) (nextIn - this._codec.NextIn);
      this._codec.NextIn = nextIn;
      this._codec.AvailableBytesIn = availableBytesIn;
      this.marker = index;
      if (index != 4)
        return -3;
      long totalBytesIn = this._codec.TotalBytesIn;
      long totalBytesOut = this._codec.TotalBytesOut;
      this.Reset();
      this._codec.TotalBytesIn = totalBytesIn;
      this._codec.TotalBytesOut = totalBytesOut;
      this.mode = InflateManager.InflateManagerMode.BLOCKS;
      return 0;
    }

    internal int SyncPoint(ZlibCodec z) => this.blocks.SyncPoint();

    private enum InflateManagerMode
    {
      METHOD,
      FLAG,
      DICT4,
      DICT3,
      DICT2,
      DICT1,
      DICT0,
      BLOCKS,
      CHECK4,
      CHECK3,
      CHECK2,
      CHECK1,
      DONE,
      BAD,
    }
  }
}
