// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.ItemHarvestQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class ItemHarvestQuest : Quest
  {
    [XmlElement("itemIndex")]
    public readonly NetInt itemIndex = new NetInt();
    [XmlElement("number")]
    public readonly NetInt number = new NetInt();

    public ItemHarvestQuest()
    {
    }

    public ItemHarvestQuest(int index, int number = 1)
    {
      this.itemIndex.Value = index;
      this.number.Value = number;
      this.questType.Value = 9;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.itemIndex, (INetSerializable) this.number);
    }

    public override bool checkIfComplete(
      NPC n = null,
      int itemIndex = -1,
      int numberHarvested = 1,
      Item item = null,
      string str = null)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.completed && itemIndex != -1 && itemIndex == this.itemIndex.Value)
      {
        this.number.Value -= numberHarvested;
        if ((int) (NetFieldBase<int, NetInt>) this.number <= 0)
        {
          this.questComplete();
          return true;
        }
      }
      return false;
    }
  }
}
