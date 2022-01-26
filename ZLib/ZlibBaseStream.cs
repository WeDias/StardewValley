// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.ZlibBaseStream
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Ionic.Crc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ionic.Zlib
{
  internal class ZlibBaseStream : Stream
  {
    protected internal ZlibCodec _z;
    protected internal ZlibBaseStream.StreamMode _streamMode = ZlibBaseStream.StreamMode.Undefined;
    protected internal FlushType _flushMode;
    protected internal ZlibStreamFlavor _flavor;
    protected internal CompressionMode _compressionMode;
    protected internal CompressionLevel _level;
    protected internal bool _leaveOpen;
    protected internal byte[] _workingBuffer;
    protected internal int _bufferSize = 16384;
    protected internal byte[] _buf1 = new byte[1];
    protected internal Stream _stream;
    protected internal CompressionStrategy Strategy;
    private CRC32 crc;
    protected internal string _GzipFileName;
    protected internal string _GzipComment;
    protected internal DateTime _GzipMtime;
    protected internal int _gzipHeaderByteCount;
    private bool nomoreinput;

    internal int Crc32 => this.crc == null ? 0 : this.crc.Crc32Result;

    public ZlibBaseStream(
      Stream stream,
      CompressionMode compressionMode,
      CompressionLevel level,
      ZlibStreamFlavor flavor,
      bool leaveOpen)
    {
      this._flushMode = FlushType.None;
      this._stream = stream;
      this._leaveOpen = leaveOpen;
      this._compressionMode = compressionMode;
      this._flavor = flavor;
      this._level = level;
      if (flavor != ZlibStreamFlavor.GZIP)
        return;
      this.crc = new CRC32();
    }

    protected internal bool _wantCompress => this._compressionMode == CompressionMode.Compress;

    private ZlibCodec z
    {
      get
      {
        if (this._z == null)
        {
          bool flag = this._flavor == ZlibStreamFlavor.ZLIB;
          this._z = new ZlibCodec();
          if (this._compressionMode == CompressionMode.Decompress)
          {
            this._z.InitializeInflate(flag);
          }
          else
          {
            this._z.Strategy = this.Strategy;
            this._z.InitializeDeflate(this._level, flag);
          }
        }
        return this._z;
      }
    }

    private byte[] workingBuffer
    {
      get
      {
        if (this._workingBuffer == null)
          this._workingBuffer = new byte[this._bufferSize];
        return this._workingBuffer;
      }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.crc != null)
        this.crc.SlurpBlock(buffer, offset, count);
      if (this._streamMode == ZlibBaseStream.StreamMode.Undefined)
        this._streamMode = ZlibBaseStream.StreamMode.Writer;
      else if (this._streamMode != ZlibBaseStream.StreamMode.Writer)
        throw new ZlibException("Cannot Write after Reading.");
      if (count == 0)
        return;
      this.z.InputBuffer = buffer;
      this._z.NextIn = offset;
      this._z.AvailableBytesIn = count;
      bool flag;
      do
      {
        this._z.OutputBuffer = this.workingBuffer;
        this._z.NextOut = 0;
        this._z.AvailableBytesOut = this._workingBuffer.Length;
        switch (this._wantCompress ? this._z.Deflate(this._flushMode) : this._z.Inflate(this._flushMode))
        {
          case 0:
          case 1:
            this._stream.Write(this._workingBuffer, 0, this._workingBuffer.Length - this._z.AvailableBytesOut);
            flag = this._z.AvailableBytesIn == 0 && (uint) this._z.AvailableBytesOut > 0U;
            if (this._flavor == ZlibStreamFlavor.GZIP && !this._wantCompress)
              flag = this._z.AvailableBytesIn == 8 && (uint) this._z.AvailableBytesOut > 0U;
            continue;
          default:
            throw new ZlibException((this._wantCompress ? "de" : "in") + "flating: " + this._z.Message);
        }
      }
      while (!flag);
    }

    private void finish()
    {
      if (this._z == null)
        return;
      if (this._streamMode == ZlibBaseStream.StreamMode.Writer)
      {
        bool flag;
        do
        {
          this._z.OutputBuffer = this.workingBuffer;
          this._z.NextOut = 0;
          this._z.AvailableBytesOut = this._workingBuffer.Length;
          int num = this._wantCompress ? this._z.Deflate(FlushType.Finish) : this._z.Inflate(FlushType.Finish);
          switch (num)
          {
            case 0:
            case 1:
              if (this._workingBuffer.Length - this._z.AvailableBytesOut > 0)
                this._stream.Write(this._workingBuffer, 0, this._workingBuffer.Length - this._z.AvailableBytesOut);
              flag = this._z.AvailableBytesIn == 0 && (uint) this._z.AvailableBytesOut > 0U;
              if (this._flavor == ZlibStreamFlavor.GZIP && !this._wantCompress)
                flag = this._z.AvailableBytesIn == 8 && (uint) this._z.AvailableBytesOut > 0U;
              continue;
            default:
              string str = (this._wantCompress ? "de" : "in") + "flating";
              if (this._z.Message == null)
                throw new ZlibException(string.Format("{0}: (rc = {1})", (object) str, (object) num));
              throw new ZlibException(str + ": " + this._z.Message);
          }
        }
        while (!flag);
        this.Flush();
        if (this._flavor != ZlibStreamFlavor.GZIP)
          return;
        if (!this._wantCompress)
          throw new ZlibException("Writing with decompression is not supported.");
        this._stream.Write(BitConverter.GetBytes(this.crc.Crc32Result), 0, 4);
        this._stream.Write(BitConverter.GetBytes((int) (this.crc.TotalBytesRead & (long) uint.MaxValue)), 0, 4);
      }
      else
      {
        if (this._streamMode != ZlibBaseStream.StreamMode.Reader || this._flavor != ZlibStreamFlavor.GZIP)
          return;
        if (this._wantCompress)
          throw new ZlibException("Reading with compression is not supported.");
        if (this._z.TotalBytesOut == 0L)
          return;
        byte[] numArray = new byte[8];
        if (this._z.AvailableBytesIn < 8)
        {
          Array.Copy((Array) this._z.InputBuffer, this._z.NextIn, (Array) numArray, 0, this._z.AvailableBytesIn);
          int count = 8 - this._z.AvailableBytesIn;
          int num = this._stream.Read(numArray, this._z.AvailableBytesIn, count);
          if (count != num)
            throw new ZlibException(string.Format("Missing or incomplete GZIP trailer. Expected 8 bytes, got {0}.", (object) (this._z.AvailableBytesIn + num)));
        }
        else
          Array.Copy((Array) this._z.InputBuffer, this._z.NextIn, (Array) numArray, 0, numArray.Length);
        int int32_1 = BitConverter.ToInt32(numArray, 0);
        int crc32Result = this.crc.Crc32Result;
        int int32_2 = BitConverter.ToInt32(numArray, 4);
        int num1 = (int) (this._z.TotalBytesOut & (long) uint.MaxValue);
        if (crc32Result != int32_1)
          throw new ZlibException(string.Format("Bad CRC32 in GZIP trailer. (actual({0:X8})!=expected({1:X8}))", (object) crc32Result, (object) int32_1));
        if (num1 != int32_2)
          throw new ZlibException(string.Format("Bad size in GZIP trailer. (actual({0})!=expected({1}))", (object) num1, (object) int32_2));
      }
    }

    private void end()
    {
      if (this.z == null)
        return;
      if (this._wantCompress)
        this._z.EndDeflate();
      else
        this._z.EndInflate();
      this._z = (ZlibCodec) null;
    }

    public override void Close()
    {
      if (this._stream == null)
        return;
      try
      {
        this.finish();
      }
      finally
      {
        this.end();
        if (!this._leaveOpen)
          this._stream.Close();
        this._stream = (Stream) null;
      }
    }

    public override void Flush() => this._stream.Flush();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => this._stream.SetLength(value);

    private string ReadZeroTerminatedString()
    {
      List<byte> byteList = new List<byte>();
      bool flag = false;
      while (this._stream.Read(this._buf1, 0, 1) == 1)
      {
        if (this._buf1[0] == (byte) 0)
          flag = true;
        else
          byteList.Add(this._buf1[0]);
        if (flag)
        {
          byte[] array = byteList.ToArray();
          return GZipStream.iso8859dash1.GetString(array, 0, array.Length);
        }
      }
      throw new ZlibException("Unexpected EOF reading GZIP header.");
    }

    private int _ReadAndValidateGzipHeader()
    {
      int num1 = 0;
      byte[] buffer1 = new byte[10];
      int num2 = this._stream.Read(buffer1, 0, buffer1.Length);
      switch (num2)
      {
        case 0:
          return 0;
        case 10:
          int num3 = buffer1[0] == (byte) 31 && buffer1[1] == (byte) 139 && buffer1[2] == (byte) 8 ? BitConverter.ToInt32(buffer1, 4) : throw new ZlibException("Bad GZIP header.");
          this._GzipMtime = GZipStream._unixEpoch.AddSeconds((double) num3);
          int num4 = num1 + num2;
          if (((int) buffer1[3] & 4) == 4)
          {
            int num5 = this._stream.Read(buffer1, 0, 2);
            int num6 = num4 + num5;
            short length = (short) ((int) buffer1[0] + (int) buffer1[1] * 256);
            byte[] buffer2 = new byte[(int) length];
            int num7 = this._stream.Read(buffer2, 0, buffer2.Length);
            if (num7 != (int) length)
              throw new ZlibException("Unexpected end-of-file reading GZIP header.");
            num4 = num6 + num7;
          }
          if (((int) buffer1[3] & 8) == 8)
            this._GzipFileName = this.ReadZeroTerminatedString();
          if (((int) buffer1[3] & 16) == 16)
            this._GzipComment = this.ReadZeroTerminatedString();
          if (((int) buffer1[3] & 2) == 2)
            this.Read(this._buf1, 0, 1);
          return num4;
        default:
          throw new ZlibException("Not a valid GZIP stream.");
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this._streamMode == ZlibBaseStream.StreamMode.Undefined)
      {
        if (!this._stream.CanRead)
          throw new ZlibException("The stream is not readable.");
        this._streamMode = ZlibBaseStream.StreamMode.Reader;
        this.z.AvailableBytesIn = 0;
        if (this._flavor == ZlibStreamFlavor.GZIP)
        {
          this._gzipHeaderByteCount = this._ReadAndValidateGzipHeader();
          if (this._gzipHeaderByteCount == 0)
            return 0;
        }
      }
      if (this._streamMode != ZlibBaseStream.StreamMode.Reader)
        throw new ZlibException("Cannot Read after Writing.");
      if (count == 0 || this.nomoreinput && this._wantCompress)
        return 0;
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (offset < buffer.GetLowerBound(0))
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (offset + count > buffer.GetLength(0))
        throw new ArgumentOutOfRangeException(nameof (count));
      this._z.OutputBuffer = buffer;
      this._z.NextOut = offset;
      this._z.AvailableBytesOut = count;
      this._z.InputBuffer = this.workingBuffer;
      int num1;
      do
      {
        if (this._z.AvailableBytesIn == 0 && !this.nomoreinput)
        {
          this._z.NextIn = 0;
          this._z.AvailableBytesIn = this._stream.Read(this._workingBuffer, 0, this._workingBuffer.Length);
          if (this._z.AvailableBytesIn == 0)
            this.nomoreinput = true;
        }
        num1 = this._wantCompress ? this._z.Deflate(this._flushMode) : this._z.Inflate(this._flushMode);
        if (this.nomoreinput && num1 == -5)
          return 0;
        if (num1 != 0 && num1 != 1)
          throw new ZlibException(string.Format("{0}flating:  rc={1}  msg={2}", this._wantCompress ? (object) "de" : (object) "in", (object) num1, (object) this._z.Message));
      }
      while ((!this.nomoreinput && num1 != 1 || this._z.AvailableBytesOut != count) && this._z.AvailableBytesOut > 0 && !this.nomoreinput && num1 == 0);
      if (this._z.AvailableBytesOut > 0)
      {
        if (num1 == 0)
        {
          int availableBytesIn = this._z.AvailableBytesIn;
        }
        if (this.nomoreinput && this._wantCompress)
        {
          int num2 = this._z.Deflate(FlushType.Finish);
          switch (num2)
          {
            case 0:
            case 1:
              break;
            default:
              throw new ZlibException(string.Format("Deflating:  rc={0}  msg={1}", (object) num2, (object) this._z.Message));
          }
        }
      }
      int count1 = count - this._z.AvailableBytesOut;
      if (this.crc != null)
        this.crc.SlurpBlock(buffer, offset, count1);
      return count1;
    }

    public override bool CanRead => this._stream.CanRead;

    public override bool CanSeek => this._stream.CanSeek;

    public override bool CanWrite => this._stream.CanWrite;

    public override long Length => this._stream.Length;

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public static void CompressString(string s, Stream compressor)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(s);
      using (compressor)
        compressor.Write(bytes, 0, bytes.Length);
    }

    public static void CompressBuffer(byte[] b, Stream compressor)
    {
      using (compressor)
        compressor.Write(b, 0, b.Length);
    }

    public static string UncompressString(byte[] compressed, Stream decompressor)
    {
      byte[] buffer = new byte[1024];
      Encoding utF8 = Encoding.UTF8;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (decompressor)
        {
          int count;
          while ((count = decompressor.Read(buffer, 0, buffer.Length)) != 0)
            memoryStream.Write(buffer, 0, count);
        }
        memoryStream.Seek(0L, SeekOrigin.Begin);
        return new StreamReader((Stream) memoryStream, utF8).ReadToEnd();
      }
    }

    public static byte[] UncompressBuffer(byte[] compressed, Stream decompressor)
    {
      byte[] buffer = new byte[1024];
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (decompressor)
        {
          int count;
          while ((count = decompressor.Read(buffer, 0, buffer.Length)) != 0)
            memoryStream.Write(buffer, 0, count);
        }
        return memoryStream.ToArray();
      }
    }

    internal enum StreamMode
    {
      Writer,
      Reader,
      Undefined,
    }
  }
}
