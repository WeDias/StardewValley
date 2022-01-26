// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.BotchedNetBool
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Xml;

namespace StardewValley.Characters
{
  public class BotchedNetBool : BotchedNetField<bool, NetBool>
  {
    public BotchedNetBool() => this.netField = new NetBool();

    public BotchedNetBool(bool default_value) => this.netField = new NetBool(default_value);

    protected override object _ParseValue(XmlReader reader) => (object) bool.Parse(reader.Value);
  }
}
