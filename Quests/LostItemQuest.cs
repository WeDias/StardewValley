// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.LostItemQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class LostItemQuest : Quest
  {
    [XmlElement("npcName")]
    public readonly NetString npcName = new NetString();
    [XmlElement("locationOfItem")]
    public readonly NetString locationOfItem = new NetString();
    [XmlElement("itemIndex")]
    public readonly NetInt itemIndex = new NetInt();
    [XmlElement("tileX")]
    public readonly NetInt tileX = new NetInt();
    [XmlElement("tileY")]
    public readonly NetInt tileY = new NetInt();
    [XmlElement("itemFound")]
    public readonly NetBool itemFound = new NetBool();
    [XmlElement("objective")]
    public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

    public LostItemQuest()
    {
    }

    public LostItemQuest(
      string npcName,
      string locationOfItem,
      int itemIndex,
      int tileX,
      int tileY)
    {
      this.npcName.Value = npcName;
      this.locationOfItem.Value = locationOfItem;
      this.itemIndex.Value = itemIndex;
      this.tileX.Value = tileX;
      this.tileY.Value = tileY;
      this.questType.Value = 9;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.objective, (INetSerializable) this.npcName, (INetSerializable) this.locationOfItem, (INetSerializable) this.itemIndex, (INetSerializable) this.tileX, (INetSerializable) this.tileY, (INetSerializable) this.itemFound);
    }

    public override void adjustGameLocation(GameLocation location)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.itemFound || !location.name.Equals((object) this.locationOfItem.Value))
        return;
      Vector2 vector2 = new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) (int) (NetFieldBase<int, NetInt>) this.tileY);
      if (location.overlayObjects.ContainsKey(vector2))
        location.overlayObjects.Remove(vector2);
      Object @object = new Object(vector2, (int) (NetFieldBase<int, NetInt>) this.itemIndex, 1);
      @object.questItem.Value = true;
      @object.questId.Value = (int) (NetFieldBase<int, NetInt>) this.id;
      @object.isSpawnedObject.Value = true;
      location.overlayObjects.Add(vector2, @object);
    }

    public new void reloadObjective()
    {
      if (this.objective.Value == null)
        return;
      this.currentObjective = this.objective.Value.loadDescriptionElement();
    }

    public override bool checkIfComplete(NPC n = null, int number1 = -1, int number2 = -2, Item item = null, string str = null)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed)
        return false;
      if (item != null && item is Object && (item as Object).parentSheetIndex.Value == this.itemIndex.Value && !(bool) (NetFieldBase<bool, NetBool>) this.itemFound)
      {
        this.itemFound.Value = true;
        string sub2 = (string) (NetFieldBase<string, NetString>) this.npcName;
        NPC characterFromName = Game1.getCharacterFromName((string) (NetFieldBase<string, NetString>) this.npcName);
        if (characterFromName != null)
          sub2 = characterFromName.displayName;
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Quests:MessageFoundLostItem", (object) item.DisplayName, (object) sub2));
        this.objective.Value = new DescriptionElement("Strings\\Quests:ObjectiveReturnToNPC", (object) characterFromName);
        Game1.playSound("jingle1");
      }
      else if (n != null && n.Name.Equals(this.npcName.Value) && n.isVillager() && (bool) (NetFieldBase<bool, NetBool>) this.itemFound && Game1.player.hasItemInInventory((int) (NetFieldBase<int, NetInt>) this.itemIndex, 1))
      {
        this.questComplete();
        Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
        string s = dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Length > 9 ? dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Split('/')[9] : Game1.content.LoadString("Data\\ExtraDialogue:LostItemQuest_DefaultThankYou");
        n.setNewDialogue(s);
        Game1.drawDialogue(n);
        Game1.player.changeFriendship(250, n);
        Game1.player.removeFirstOfThisItemFromInventory((int) (NetFieldBase<int, NetInt>) this.itemIndex);
        return true;
      }
      return false;
    }
  }
}
