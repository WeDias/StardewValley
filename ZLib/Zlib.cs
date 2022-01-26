// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.ZlibException
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Runtime.InteropServices;

namespace Ionic.Zlib
{
  /// <summary>
  /// A general purpose exception class for exceptions in the Zlib library.
  /// </summary>
  [Guid("ebc25cf6-9120-4283-b972-0e5520d0000E")]
  public class ZlibException : Exception
  {
    /// <summary>
    /// The ZlibException class captures exception information generated
    /// by the Zlib library.
    /// </summary>
    public ZlibException()
    {
    }

    /// <summary>
    /// This ctor collects a message attached to the exception.
    /// </summary>
    /// <param name="s">the message for the exception.</param>
    public ZlibException(string s)
      : base(s)
    {
    }
  }
}
