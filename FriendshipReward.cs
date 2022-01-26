// Decompiled with JetBrains decompiler
// Type: StardewValley.FriendshipReward
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class FriendshipReward : OrderReward
  {
    [XmlElement("targetName")]
    public NetString targetName = new NetString();
    [XmlElement("amount")]
    public NetInt amount = new NetInt();

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.targetName, (INetSerializable) this.amount);
    }

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      string requester = (string) (NetFieldBase<string, NetString>) order.requester;
      if (data.ContainsKey("TargetName"))
        requester = data["TargetName"];
      this.targetName.Value = order.Parse(requester);
      string data1 = "250";
      if (data.ContainsKey("Amount"))
        data1 = data["Amount"];
      this.amount.Value = int.Parse(order.Parse(data1));
    }

    public override void Grant()
    {
      NPC characterFromName = Game1.getCharacterFromName(this.targetName.Value);
      if (characterFromName == null)
        return;
      Game1.player.changeFriendship(this.amount.Value, characterFromName);
    }
  }
}
