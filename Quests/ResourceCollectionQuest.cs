// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.ResourceCollectionQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class ResourceCollectionQuest : Quest
  {
    [XmlElement("target")]
    public readonly NetString target = new NetString();
    [XmlElement("targetMessage")]
    public readonly NetString targetMessage = new NetString();
    [XmlElement("numberCollected")]
    public readonly NetInt numberCollected = new NetInt();
    [XmlElement("number")]
    public readonly NetInt number = new NetInt();
    [XmlElement("reward")]
    public readonly NetInt reward = new NetInt();
    [XmlElement("resource")]
    public readonly NetInt resource = new NetInt();
    [XmlElement("deliveryItem")]
    public readonly NetRef<StardewValley.Object> deliveryItem = new NetRef<StardewValley.Object>();
    public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
    public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
    [XmlElement("objective")]
    public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

    public ResourceCollectionQuest() => this.questType.Value = 10;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.parts, (INetSerializable) this.dialogueparts, (INetSerializable) this.objective, (INetSerializable) this.target, (INetSerializable) this.targetMessage, (INetSerializable) this.numberCollected, (INetSerializable) this.number, (INetSerializable) this.reward, (INetSerializable) this.resource, (INetSerializable) this.deliveryItem);
    }

    public void loadQuestInfo()
    {
      if (this.target.Value != null || Game1.gameMode == (byte) 6)
        return;
      this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13640");
      this.resource.Value = this.random.Next(6) * 2;
      for (int index = 0; index < this.random.Next(1, 100); ++index)
        this.random.Next();
      int val1_1 = 0;
      int val1_2 = 0;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
        val1_1 = Math.Max(val1_1, allFarmer.MiningLevel);
      foreach (Farmer allFarmer in Game1.getAllFarmers())
        val1_2 = Math.Max(val1_2, allFarmer.ForagingLevel);
      switch (this.resource.Value)
      {
        case 0:
          this.resource.Value = 378;
          this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.resource, 1);
          this.number.Value = 20 + val1_1 * 2 + this.random.Next(-2, 4) * 2;
          this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.number * 10;
          this.number.Value = (int) (NetFieldBase<int, NetInt>) this.number - (int) (NetFieldBase<int, NetInt>) this.number % 5;
          this.target.Value = "Clint";
          break;
        case 2:
          this.resource.Value = 380;
          this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.resource, 1);
          this.number.Value = 15 + val1_1 + this.random.Next(-1, 3) * 2;
          this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.number * 15;
          this.number.Value = (int) ((double) (int) (NetFieldBase<int, NetInt>) this.number * 0.75);
          this.number.Value = (int) (NetFieldBase<int, NetInt>) this.number - (int) (NetFieldBase<int, NetInt>) this.number % 5;
          this.target.Value = "Clint";
          break;
        case 4:
          this.resource.Value = 382;
          this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.resource, 1);
          this.number.Value = 10 + val1_1 + this.random.Next(-1, 3) * 2;
          this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.number * 25;
          this.number.Value = (int) ((double) (int) (NetFieldBase<int, NetInt>) this.number * 0.75);
          this.number.Value = (int) (NetFieldBase<int, NetInt>) this.number - (int) (NetFieldBase<int, NetInt>) this.number % 5;
          this.target.Value = "Clint";
          break;
        case 6:
          this.resource.Value = Utility.GetAllPlayerDeepestMineLevel() > 40 ? 384 : 378;
          this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.resource, 1);
          this.number.Value = 8 + val1_1 / 2 + this.random.Next(-1, 1) * 2;
          this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.number * 30;
          this.number.Value = (int) ((double) (int) (NetFieldBase<int, NetInt>) this.number * 0.75);
          this.number.Value = (int) (NetFieldBase<int, NetInt>) this.number - (int) (NetFieldBase<int, NetInt>) this.number % 2;
          this.target.Value = "Clint";
          break;
        case 8:
          this.resource.Value = 388;
          this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.resource, 1);
          this.number.Value = 25 + val1_2 + this.random.Next(-3, 3) * 2;
          this.number.Value = (int) (NetFieldBase<int, NetInt>) this.number - (int) (NetFieldBase<int, NetInt>) this.number % 5;
          this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.number * 8;
          this.target.Value = "Robin";
          break;
        case 10:
          this.resource.Value = 390;
          this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.resource, 1);
          this.number.Value = 25 + val1_1 + this.random.Next(-3, 3) * 2;
          this.number.Value = (int) (NetFieldBase<int, NetInt>) this.number - (int) (NetFieldBase<int, NetInt>) this.number % 5;
          this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.number * 8;
          this.target.Value = "Robin";
          break;
      }
      if (this.target.Value == null)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.resource < 388)
      {
        this.parts.Clear();
        int index = this.random.Next(4);
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13647", (object) this.number.Value, (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) new DescriptionElement[4]
        {
          (DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13649",
          (DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13650",
          (DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13651",
          (DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13652"
        }).ElementAt<DescriptionElement>(index)));
        if (index == 3)
        {
          this.dialogueparts.Clear();
          this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13655");
          this.dialogueparts.Add((DescriptionElement) (this.random.NextDouble() < 0.3 ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13656" : (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13657" : "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13658")));
          this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13659");
        }
        else
        {
          this.dialogueparts.Clear();
          this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13662");
          this.dialogueparts.Add((DescriptionElement) (this.random.NextDouble() < 0.3 ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13656" : (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13657" : "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13658")));
          this.dialogueparts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13667", this.random.NextDouble() < 0.3 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13668") : (this.random.NextDouble() < 0.5 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13669") : (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13670"))) : (DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13672");
          this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13673");
        }
      }
      else
      {
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13674", (object) this.number.Value, (object) this.deliveryItem.Value));
        this.dialogueparts.Clear();
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13677", (int) (NetFieldBase<int, NetInt>) this.resource == 388 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13678") : (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13679")));
        this.dialogueparts.Add((DescriptionElement) (this.random.NextDouble() < 0.3 ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13681" : (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13682" : "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13683")));
      }
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13607", (object) this.reward.Value));
      this.parts.Add((DescriptionElement) (this.target.Value.Equals("Clint") ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13688" : ""));
      this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13691", (object) "0", (object) this.number.Value, (object) this.deliveryItem.Value);
    }

    public override void reloadDescription()
    {
      if (this._questDescription == "")
        this.loadQuestInfo();
      if (this.parts.Count == 0 || this.parts == null || this.dialogueparts.Count == 0 || this.dialogueparts == null)
        return;
      string str1 = "";
      string str2 = "";
      foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>) this.parts)
        str1 += part.loadDescriptionElement();
      foreach (DescriptionElement dialoguepart in (NetList<DescriptionElement, NetDescriptionElementRef>) this.dialogueparts)
        str2 += dialoguepart.loadDescriptionElement();
      this.questDescription = str1;
      this.targetMessage.Value = str2;
    }

    public override void reloadObjective()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.numberCollected < (int) (NetFieldBase<int, NetInt>) this.number)
        this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13691", (object) this.numberCollected.Value, (object) this.number.Value, (object) this.deliveryItem.Value);
      if (this.objective.Value == null)
        return;
      this.currentObjective = this.objective.Value.loadDescriptionElement();
    }

    public override bool checkIfComplete(
      NPC n = null,
      int resourceCollected = -1,
      int amount = -1,
      Item item = null,
      string monsterName = null)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed)
        return false;
      if (n == null && resourceCollected != -1 && amount != -1 && resourceCollected == (int) (NetFieldBase<int, NetInt>) this.resource && (int) (NetFieldBase<int, NetInt>) this.numberCollected < (int) (NetFieldBase<int, NetInt>) this.number)
      {
        this.numberCollected.Value = Math.Min((int) (NetFieldBase<int, NetInt>) this.number, (int) (NetFieldBase<int, NetInt>) this.numberCollected + amount);
        if ((int) (NetFieldBase<int, NetInt>) this.numberCollected < (int) (NetFieldBase<int, NetInt>) this.number)
        {
          if (this.deliveryItem.Value == null)
            this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.resource, 1);
        }
        else
        {
          this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13277", (object) Game1.getCharacterFromName((string) (NetFieldBase<string, NetString>) this.target));
          Game1.playSound("jingle1");
        }
        Game1.dayTimeMoneyBox.moneyDial.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(387, 497, 3, 8), 800f, 1, 0, Game1.dayTimeMoneyBox.position + new Vector2(228f, 244f), false, false, 1f, 0.01f, Color.White, 4f, 0.3f, 0.0f, 0.0f)
        {
          scaleChangeChange = -0.012f
        });
      }
      else if (n != null && this.target.Value != null && (int) (NetFieldBase<int, NetInt>) this.numberCollected >= (int) (NetFieldBase<int, NetInt>) this.number && n.Name.Equals(this.target.Value) && n.isVillager())
      {
        n.CurrentDialogue.Push(new Dialogue((string) (NetFieldBase<string, NetString>) this.targetMessage, n));
        this.moneyReward.Value = (int) (NetFieldBase<int, NetInt>) this.reward;
        n.Name.Equals("Robin");
        this.questComplete();
        Game1.drawDialogue(n);
        return true;
      }
      return false;
    }
  }
}
