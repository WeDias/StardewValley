// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetVector2Dictionary`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.Network
{
  public sealed class NetVector2Dictionary<T, TField> : 
    NetFieldDictionary<Vector2, T, TField, SerializableDictionary<Vector2, T>, NetVector2Dictionary<T, TField>>
    where TField : NetField<T, TField>, new()
  {
    public NetVector2Dictionary()
    {
    }

    public NetVector2Dictionary(IEnumerable<KeyValuePair<Vector2, T>> dict)
      : base(dict)
    {
    }

    protected override Vector2 ReadKey(BinaryReader reader) => new Vector2(reader.ReadSingle(), reader.ReadSingle());

    protected override void WriteKey(BinaryWriter writer, Vector2 key)
    {
      writer.Write(key.X);
      writer.Write(key.Y);
    }
  }
}
