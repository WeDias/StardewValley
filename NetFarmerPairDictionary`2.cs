// Decompiled with JetBrains decompiler
// Type: StardewValley.NetFarmerPairDictionary`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.IO;

namespace StardewValley
{
  public class NetFarmerPairDictionary<T, TField> : 
    NetFieldDictionary<FarmerPair, T, TField, SerializableDictionary<FarmerPair, T>, NetFarmerPairDictionary<T, TField>>
    where TField : NetField<T, TField>, new()
  {
    public NetFarmerPairDictionary()
    {
    }

    public NetFarmerPairDictionary(IEnumerable<KeyValuePair<FarmerPair, T>> dict)
      : base(dict)
    {
    }

    protected override FarmerPair ReadKey(BinaryReader reader) => FarmerPair.MakePair(reader.ReadInt64(), reader.ReadInt64());

    protected override void WriteKey(BinaryWriter writer, FarmerPair key)
    {
      writer.Write(key.Farmer1);
      writer.Write(key.Farmer2);
    }
  }
}
