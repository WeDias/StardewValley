// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.BotchedNetLong
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Xml;

namespace StardewValley.Characters
{
  public class BotchedNetLong : BotchedNetField<long, NetLong>
  {
    public BotchedNetLong() => this.netField = new NetLong();

    public BotchedNetLong(long default_value) => this.netField = new NetLong(default_value);

    protected override object _ParseValue(XmlReader reader) => (object) long.Parse(reader.Value);
  }
}
