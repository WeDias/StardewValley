// Decompiled with JetBrains decompiler
// Type: StardewValley.NetIntIntArrayDictionary
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewValley
{
  public class NetIntIntArrayDictionary : 
    NetDictionary<int, int[], NetArray<int, NetInt>, SerializableDictionary<int, int[]>, NetIntIntArrayDictionary>
  {
    protected override int ReadKey(BinaryReader reader) => reader.ReadInt32();

    protected override void WriteKey(BinaryWriter writer, int key) => writer.Write(key);

    protected override void setFieldValue(NetArray<int, NetInt> field, int key, int[] value) => field.Set((IList<int>) value);

    protected override int[] getFieldValue(NetArray<int, NetInt> field) => field.ToArray<int>();

    protected override int[] getFieldTargetValue(NetArray<int, NetInt> field) => field.ToArray<int>();
  }
}
