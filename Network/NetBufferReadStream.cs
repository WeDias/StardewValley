// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetBufferReadStream
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Lidgren.Network;
using System;
using System.IO;

namespace StardewValley.Network
{
  public class NetBufferReadStream : Stream
  {
    private long offset;
    public NetBuffer Buffer;

    public NetBufferReadStream(NetBuffer buffer)
    {
      this.Buffer = buffer;
      this.offset = buffer.Position;
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => ((long) this.Buffer.LengthBits - this.offset) / 8L;

    public override long Position
    {
      get => (this.Buffer.Position - this.offset) / 8L;
      set => this.Buffer.Position = this.offset + value * 8L;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.Buffer.ReadBytes(buffer, offset, count);
      return count;
    }

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
          this.Position = this.Length + offset;
          break;
      }
      return this.Position;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
  }
}
