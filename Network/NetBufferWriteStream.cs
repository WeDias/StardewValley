// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetBufferWriteStream
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Lidgren.Network;
using System;
using System.IO;

namespace StardewValley.Network
{
  public class NetBufferWriteStream : Stream
  {
    private int offset;
    public NetBuffer Buffer;

    public NetBufferWriteStream(NetBuffer buffer)
    {
      this.Buffer = buffer;
      this.offset = buffer.LengthBits;
    }

    public override bool CanRead => false;

    public override bool CanSeek => true;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => (long) ((this.Buffer.LengthBits - this.offset) / 8);
      set => this.Buffer.LengthBits = (int) ((long) this.offset + value * 8L);
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          this.Position = offset;
          break;
        case SeekOrigin.Current:
          this.Position += offset;
          break;
        case SeekOrigin.End:
          throw new NotSupportedException();
      }
      return this.Position;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => this.Buffer.Write(buffer, offset, count);
  }
}
