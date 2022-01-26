// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.GoSomewhereQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class GoSomewhereQuest : Quest
  {
    [XmlElement("whereToGo")]
    public readonly NetString whereToGo = new NetString();

    public GoSomewhereQuest()
    {
    }

    public GoSomewhereQuest(string where) => this.whereToGo.Value = where;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.whereToGo);
    }

    public override void adjustGameLocation(GameLocation location) => this.checkIfComplete((NPC) null, -1, -2, (Item) null, (string) (NetFieldBase<string, NetString>) location.name);

    public override bool checkIfComplete(NPC n = null, int number1 = -1, int number2 = -2, Item item = null, string str = null)
    {
      if (str == null || !str.Equals(this.whereToGo.Value))
        return false;
      this.questComplete();
      return true;
    }
  }
}
