// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.CompressionMode
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace Ionic.Zlib
{
  /// <summary>
  /// An enum to specify the direction of transcoding - whether to compress or decompress.
  /// </summary>
  public enum CompressionMode
  {
    /// <summary>
    /// Used to specify that the stream should compress the data.
    /// </summary>
    Compress,
    /// <summary>
    /// Used to specify that the stream should decompress the data.
    /// </summary>
    Decompress,
  }
}
