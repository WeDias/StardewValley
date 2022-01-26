// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetStringDictionary`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.Network
{
  public class NetStringDictionary<T, TField> : 
    NetFieldDictionary<string, T, TField, SerializableDictionary<string, T>, NetStringDictionary<T, TField>>
    where TField : NetField<T, TField>, new()
  {
    public NetStringDictionary()
    {
    }

    public NetStringDictionary(IEnumerable<KeyValuePair<string, T>> dict)
      : base(dict)
    {
    }

    protected override string ReadKey(BinaryReader reader) => reader.ReadString();

    protected override void WriteKey(BinaryWriter writer, string key) => writer.Write(key);
  }
}
