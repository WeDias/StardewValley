// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.SecretLostItemQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class SecretLostItemQuest : Quest
  {
    [XmlElement("npcName")]
    public readonly NetString npcName = new NetString();
    [XmlElement("friendshipReward")]
    public readonly NetInt friendshipReward = new NetInt();
    [XmlElement("exclusiveQuestId")]
    public readonly NetInt exclusiveQuestId = new NetInt(0);
    [XmlElement("itemIndex")]
    public readonly NetInt itemIndex = new NetInt();
    [XmlElement("itemFound")]
    public readonly NetBool itemFound = new NetBool();

    public SecretLostItemQuest()
    {
    }

    public SecretLostItemQuest(
      string npcName,
      int itemIndex,
      int friendshipReward,
      int exclusiveQuestId)
    {
      this.npcName.Value = npcName;
      this.itemIndex.Value = itemIndex;
      this.friendshipReward.Value = friendshipReward;
      this.exclusiveQuestId.Value = exclusiveQuestId;
      this.questType.Value = 9;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.npcName, (INetSerializable) this.friendshipReward, (INetSerializable) this.exclusiveQuestId, (INetSerializable) this.itemFound);
    }

    public override bool isSecretQuest() => true;

    public override bool checkIfComplete(NPC n = null, int number1 = -1, int number2 = -2, Item item = null, string str = null)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed)
        return false;
      if (item != null && item is Object && (item as Object).parentSheetIndex.Value == this.itemIndex.Value && !(bool) (NetFieldBase<bool, NetBool>) this.itemFound)
      {
        this.itemFound.Value = true;
        Game1.playSound("jingle1");
      }
      else if (n != null && n.Name.Equals(this.npcName.Value) && n.isVillager() && (bool) (NetFieldBase<bool, NetBool>) this.itemFound && Game1.player.hasItemInInventory((int) (NetFieldBase<int, NetInt>) this.itemIndex, 1))
      {
        this.questComplete();
        Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
        string s = dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Length > 9 ? dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Split('/')[9] : Game1.content.LoadString("Data\\ExtraDialogue:LostItemQuest_DefaultThankYou");
        n.setNewDialogue(s);
        Game1.drawDialogue(n);
        Game1.player.changeFriendship(this.friendshipReward.Value, n);
        Game1.player.removeFirstOfThisItemFromInventory((int) (NetFieldBase<int, NetInt>) this.itemIndex);
        return true;
      }
      return false;
    }

    public override void questComplete()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed)
        return;
      this.completed.Value = true;
      Game1.player.questLog.Remove((Quest) this);
      foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) Game1.player.questLog)
      {
        if (quest != null && (int) (NetFieldBase<int, NetInt>) quest.id == this.exclusiveQuestId.Value)
          quest.destroy.Value = true;
      }
      Game1.playSound("questcomplete");
    }
  }
}
