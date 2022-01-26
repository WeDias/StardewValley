// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetIntDictionary`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.Network
{
  public class NetIntDictionary<T, TField> : 
    NetFieldDictionary<int, T, TField, SerializableDictionary<int, T>, NetIntDictionary<T, TField>>
    where TField : NetField<T, TField>, new()
  {
    public NetIntDictionary()
    {
    }

    public NetIntDictionary(IEnumerable<KeyValuePair<int, T>> dict)
      : base(dict)
    {
    }

    protected override int ReadKey(BinaryReader reader) => reader.ReadInt32();

    protected override void WriteKey(BinaryWriter writer, int key) => writer.Write(key);
  }
}
