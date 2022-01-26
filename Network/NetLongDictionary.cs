// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetLongDictionary`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.Network
{
  public class NetLongDictionary<T, TField> : 
    NetFieldDictionary<long, T, TField, SerializableDictionary<long, T>, NetLongDictionary<T, TField>>
    where TField : NetField<T, TField>, new()
  {
    public NetLongDictionary()
    {
    }

    public NetLongDictionary(IEnumerable<KeyValuePair<long, T>> dict)
      : base(dict)
    {
    }

    protected override long ReadKey(BinaryReader reader) => reader.ReadInt64();

    protected override void WriteKey(BinaryWriter writer, long key) => writer.Write(key);
  }
}
