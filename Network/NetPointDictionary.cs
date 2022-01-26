// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetPointDictionary`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.Network
{
  public class NetPointDictionary<T, TField> : 
    NetFieldDictionary<Point, T, TField, SerializableDictionary<Point, T>, NetPointDictionary<T, TField>>
    where TField : NetField<T, TField>, new()
  {
    public NetPointDictionary()
    {
    }

    public NetPointDictionary(IEnumerable<KeyValuePair<Point, T>> dict)
      : base(dict)
    {
    }

    protected override Point ReadKey(BinaryReader reader) => new Point(reader.ReadInt32(), reader.ReadInt32());

    protected override void WriteKey(BinaryWriter writer, Point key)
    {
      writer.Write(key.X);
      writer.Write(key.Y);
    }
  }
}
