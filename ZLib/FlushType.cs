// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.FlushType
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace Ionic.Zlib
{
  /// <summary>Describes how to flush the current deflate operation.</summary>
  /// <remarks>
  /// The different FlushType values are useful when using a Deflate in a streaming application.
  /// </remarks>
  public enum FlushType
  {
    /// <summary>No flush at all.</summary>
    None,
    /// <summary>Closes the current block, but doesn't flush it to
    /// the output. Used internally only in hypothetical
    /// scenarios.  This was supposed to be removed by Zlib, but it is
    /// still in use in some edge cases.
    /// </summary>
    Partial,
    /// <summary>
    /// Use this during compression to specify that all pending output should be
    /// flushed to the output buffer and the output should be aligned on a byte
    /// boundary.  You might use this in a streaming communication scenario, so that
    /// the decompressor can get all input data available so far.  When using this
    /// with a ZlibCodec, <c>AvailableBytesIn</c> will be zero after the call if
    /// enough output space has been provided before the call.  Flushing will
    /// degrade compression and so it should be used only when necessary.
    /// </summary>
    Sync,
    /// <summary>
    /// Use this during compression to specify that all output should be flushed, as
    /// with <c>FlushType.Sync</c>, but also, the compression state should be reset
    /// so that decompression can restart from this point if previous compressed
    /// data has been damaged or if random access is desired.  Using
    /// <c>FlushType.Full</c> too often can significantly degrade the compression.
    /// </summary>
    Full,
    /// <summary>Signals the end of the compression/decompression stream.</summary>
    Finish,
  }
}
