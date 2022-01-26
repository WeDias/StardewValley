// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.ItemDeliveryQuest
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
  public class ItemDeliveryQuest : Quest
  {
    public string targetMessage;
    [XmlElement("target")]
    public readonly NetString target = new NetString();
    [XmlElement("item")]
    public readonly NetInt item = new NetInt();
    [XmlElement("number")]
    public readonly NetInt number = new NetInt(1);
    [XmlElement("deliveryItem")]
    public readonly NetRef<StardewValley.Object> deliveryItem = new NetRef<StardewValley.Object>();
    public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
    public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
    [XmlElement("objective")]
    public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();
    [XmlIgnore]
    [Obsolete]
    public NPC actualTarget;

    public ItemDeliveryQuest() => this.questType.Value = 3;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.target, (INetSerializable) this.item, (INetSerializable) this.number, (INetSerializable) this.deliveryItem, (INetSerializable) this.parts, (INetSerializable) this.dialogueparts, (INetSerializable) this.objective);
    }

    public List<NPC> GetValidTargetList()
    {
      List<NPC> source = new List<NPC>();
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer != null)
        {
          foreach (string key in allFarmer.friendshipData.Keys)
          {
            NPC characterFromName = Game1.getCharacterFromName(key);
            if (characterFromName != null && !source.Contains(characterFromName))
              source.Add(characterFromName);
          }
        }
      }
      source.OrderBy<NPC, string>((Func<NPC, string>) (n => n.Name));
      for (int index = 0; index < source.Count; ++index)
      {
        NPC npc = source[index];
        if (npc.IsInvisible)
        {
          source.RemoveAt(index);
          --index;
        }
        else if (npc.Age == 2)
        {
          source.RemoveAt(index);
          --index;
        }
        else if (!npc.isVillager())
        {
          source.RemoveAt(index);
          --index;
        }
        else if (npc.Name.Equals("Krobus") || npc.Name.Equals("Qi") || npc.Name.Equals("Dwarf") || npc.Name.Equals("Gunther") || npc.Name.Equals("Bouncer") || npc.Name.Equals("Henchman") || npc.Name.Equals("Marlon") || npc.Name.Equals("Mariner"))
        {
          source.RemoveAt(index);
          --index;
        }
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        for (int index = 0; index < source.Count; ++index)
        {
          if (source[index].Name.Equals(allFarmer.spouse))
          {
            source.RemoveAt(index);
            --index;
          }
        }
      }
      for (int index = 0; index < source.Count; ++index)
      {
        if (source[index].Name.Equals("Sandy"))
        {
          bool flag = false;
          foreach (Farmer allFarmer in Game1.getAllFarmers())
          {
            if (allFarmer.eventsSeen.Contains(67))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            source.RemoveAt(index);
            int num = index - 1;
            break;
          }
          break;
        }
      }
      return source;
    }

    public void loadQuestInfo()
    {
      if (this.target.Value != null)
        return;
      this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13285");
      List<NPC> validTargetList = this.GetValidTargetList();
      if (Game1.player.friendshipData == null || Game1.player.friendshipData.Count() <= 0 || validTargetList.Count <= 0)
        return;
      NPC characterFromName = validTargetList[this.random.Next(validTargetList.Count)];
      if (characterFromName == null)
        return;
      this.target.Value = (string) (NetFieldBase<string, NetString>) characterFromName.name;
      if (this.target.Value.Equals("Wizard") && !Game1.player.mailReceived.Contains("wizardJunimoNote") && !Game1.player.mailReceived.Contains("JojaMember"))
      {
        this.target.Value = "Demetrius";
        characterFromName = Game1.getCharacterFromName(this.target.Value);
      }
      if (!Game1.currentSeason.Equals("winter") && this.random.NextDouble() < 0.15)
      {
        List<int> source = Utility.possibleCropsAtThisTime(Game1.currentSeason, Game1.dayOfMonth <= 7);
        this.item.Value = source.ElementAt<int>(this.random.Next(source.Count));
        this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, this.item.Value, 1);
        this.parts.Clear();
        this.parts.Add((DescriptionElement) (this.random.NextDouble() < 0.3 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13299" : (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13300" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13301")));
        this.parts.Add(this.random.NextDouble() < 0.3 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13302", (object) this.deliveryItem.Value) : (this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13303", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13304", (object) this.deliveryItem.Value)));
        this.parts.Add((DescriptionElement) (this.random.NextDouble() < 0.25 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13306" : (this.random.NextDouble() < 0.33 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13307" : (this.random.NextDouble() < 0.5 ? "" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13308"))));
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName));
        if (this.target.Value.Equals("Demetrius"))
        {
          this.parts.Clear();
          this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13311", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13314", (object) this.deliveryItem.Value));
        }
        if (this.target.Value.Equals("Marnie"))
        {
          this.parts.Clear();
          this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13317", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13320", (object) this.deliveryItem.Value));
        }
        if (this.target.Value.Equals("Sebastian"))
        {
          this.parts.Clear();
          this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13324", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13327", (object) this.deliveryItem.Value));
        }
      }
      else
      {
        this.item.Value = Utility.getRandomItemFromSeason(Game1.currentSeason, 1000, true);
        if ((int) (NetFieldBase<int, NetInt>) this.item == -5)
          this.item.Value = 176;
        if ((int) (NetFieldBase<int, NetInt>) this.item == -6)
          this.item.Value = 184;
        this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.item, 1);
        DescriptionElement[] source1 = (DescriptionElement[]) null;
        DescriptionElement[] descriptionElementArray1 = (DescriptionElement[]) null;
        DescriptionElement[] descriptionElementArray2 = (DescriptionElement[]) null;
        if (Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.item].Split('/')[3].Split(' ')[0].Equals("Cooking") && !this.target.Value.Equals("Wizard"))
        {
          if (this.random.NextDouble() < 0.33)
          {
            DescriptionElement[] source2 = new DescriptionElement[12]
            {
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13336",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13337",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13338",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13339",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13340",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13341",
              Game1.samBandName.Equals(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156")) ? (!Game1.elliottBookName.Equals(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2157")) ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13342", (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2157")) : (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13346") : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13347", (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2156")),
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13349",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13350",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13351",
              (DescriptionElement) (Game1.currentSeason.Equals("winter") ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13353" : (Game1.currentSeason.Equals("summer") ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13355" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13356")),
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13357"
            };
            this.parts.Clear();
            this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13333", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) source2).ElementAt<DescriptionElement>(this.random.Next(12))) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13334", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) source2).ElementAt<DescriptionElement>(this.random.Next(12))));
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName));
          }
          else
          {
            DescriptionElement descriptionElement = new DescriptionElement();
            switch (Game1.dayOfMonth % 7)
            {
              case 0:
                descriptionElement = (DescriptionElement) "Strings\\StringsFromCSFiles:Game1.cs.3042";
                break;
              case 1:
                descriptionElement = (DescriptionElement) "Strings\\StringsFromCSFiles:Game1.cs.3043";
                break;
              case 2:
                descriptionElement = (DescriptionElement) "Strings\\StringsFromCSFiles:Game1.cs.3044";
                break;
              case 3:
                descriptionElement = (DescriptionElement) "Strings\\StringsFromCSFiles:Game1.cs.3045";
                break;
              case 4:
                descriptionElement = (DescriptionElement) "Strings\\StringsFromCSFiles:Game1.cs.3046";
                break;
              case 5:
                descriptionElement = (DescriptionElement) "Strings\\StringsFromCSFiles:Game1.cs.3047";
                break;
              case 6:
                descriptionElement = (DescriptionElement) "Strings\\StringsFromCSFiles:Game1.cs.3048";
                break;
            }
            source1 = new DescriptionElement[5]
            {
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13360", (object) this.deliveryItem.Value),
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13364", (object) this.deliveryItem.Value),
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13367", (object) this.deliveryItem.Value),
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13370", (object) this.deliveryItem.Value),
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13373", (object) descriptionElement, (object) this.deliveryItem.Value, (object) characterFromName)
            };
            descriptionElementArray1 = new DescriptionElement[5]
            {
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
              new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
              (DescriptionElement) ""
            };
            descriptionElementArray2 = new DescriptionElement[5]
            {
              (DescriptionElement) "",
              (DescriptionElement) "",
              (DescriptionElement) "",
              (DescriptionElement) "",
              (DescriptionElement) ""
            };
          }
          this.parts.Clear();
          int index = this.random.Next(((IEnumerable<DescriptionElement>) source1).Count<DescriptionElement>());
          this.parts.Add(source1[index]);
          this.parts.Add(descriptionElementArray1[index]);
          this.parts.Add(descriptionElementArray2[index]);
          if (this.target.Value.Equals("Sebastian"))
          {
            this.parts.Clear();
            this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13378", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13381", (object) this.deliveryItem.Value));
          }
        }
        else if (this.random.NextDouble() < 0.5 && Convert.ToInt32(Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.item].Split('/')[2]) > 0)
        {
          DescriptionElement[] source3 = new DescriptionElement[2]
          {
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13383", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) new DescriptionElement[12]
            {
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13385",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13386",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13387",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13388",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13389",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13390",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13391",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13392",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13393",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13394",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13395",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13396"
            }).ElementAt<DescriptionElement>(this.random.Next(12))),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13400", (object) this.deliveryItem.Value)
          };
          DescriptionElement[] descriptionElementArray3 = new DescriptionElement[2]
          {
            new DescriptionElement(this.random.NextDouble() < 0.5 ? "" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13398"),
            new DescriptionElement(this.random.NextDouble() < 0.5 ? "" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13402")
          };
          DescriptionElement[] descriptionElementArray4 = new DescriptionElement[2]
          {
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName)
          };
          if (this.random.NextDouble() < 0.33)
          {
            DescriptionElement[] source4 = new DescriptionElement[12]
            {
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13336",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13337",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13338",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13339",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13340",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13341",
              Game1.samBandName.Equals(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156")) ? (!Game1.elliottBookName.Equals(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2157")) ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13342", (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2157")) : (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13346") : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13347", (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2156")),
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13420",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13421",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13422",
              (DescriptionElement) (Game1.currentSeason.Equals("winter") ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13424" : (Game1.currentSeason.Equals("summer") ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13426" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13427")),
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13357"
            };
            this.parts.Clear();
            this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13333", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) source4).ElementAt<DescriptionElement>(this.random.Next(12))) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13334", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) source4).ElementAt<DescriptionElement>(this.random.Next(12))));
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName));
          }
          else
          {
            this.parts.Clear();
            int index = this.random.Next(((IEnumerable<DescriptionElement>) source3).Count<DescriptionElement>());
            this.parts.Add(source3[index]);
            this.parts.Add(descriptionElementArray3[index]);
            this.parts.Add(descriptionElementArray4[index]);
          }
          if (this.target.Value.Equals("Demetrius"))
          {
            this.parts.Clear();
            this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13311", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13314", (object) this.deliveryItem.Value));
          }
          if (this.target.Value.Equals("Marnie"))
          {
            this.parts.Clear();
            this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13317", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13320", (object) this.deliveryItem.Value));
          }
          if (this.target.Value.Equals("Harvey"))
          {
            DescriptionElement[] source5 = new DescriptionElement[12]
            {
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13448",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13449",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13450",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13451",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13452",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13453",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13454",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13455",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13456",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13457",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13458",
              (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13459"
            };
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13446", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) source5).ElementAt<DescriptionElement>(this.random.Next(12))));
          }
          if (this.target.Value.Equals("Gus") && this.random.NextDouble() < 0.6)
          {
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13462", (object) this.deliveryItem.Value));
          }
        }
        else if (this.random.NextDouble() < 0.5 && Convert.ToInt32(Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.item].Split('/')[2]) < 0)
        {
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13464", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) new DescriptionElement[5]
          {
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13465",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13466",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13467",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13468",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13469"
          }).ElementAt<DescriptionElement>(this.random.Next(5))));
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName));
          if (this.target.Value.Equals("Emily"))
          {
            this.parts.Clear();
            this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13473", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13476", (object) this.deliveryItem.Value));
          }
        }
        else
        {
          DescriptionElement[] source6 = new DescriptionElement[12]
          {
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13502",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13503",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13504",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13505",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13506",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13507",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13508",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13509",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13510",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13511",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13512",
            (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13513"
          };
          DescriptionElement[] source7 = new DescriptionElement[9]
          {
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13480", (object) characterFromName, (object) this.deliveryItem.Value),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13481", (object) this.deliveryItem.Value),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13485", (object) this.deliveryItem.Value),
            this.random.NextDouble() < 0.4 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13491", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13492", (object) this.deliveryItem.Value),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13494", (object) this.deliveryItem.Value),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13497", (object) this.deliveryItem.Value),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13500", (object) this.deliveryItem.Value, (object) ((IEnumerable<DescriptionElement>) source6).ElementAt<DescriptionElement>(this.random.Next(12))),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13518", (object) characterFromName, (object) this.deliveryItem.Value),
            this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13520", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13523", (object) this.deliveryItem.Value)
          };
          DescriptionElement[] descriptionElementArray5 = new DescriptionElement[9]
          {
            (DescriptionElement) "",
            (DescriptionElement) (this.random.NextDouble() < 0.3 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13482" : (this.random.NextDouble() < 0.5 ? "" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13483")),
            (DescriptionElement) (this.random.NextDouble() < 0.25 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13487" : (this.random.NextDouble() < 0.33 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13488" : (this.random.NextDouble() < 0.5 ? "" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13489"))),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
            (DescriptionElement) (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13514" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13516"),
            (DescriptionElement) "",
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName)
          };
          DescriptionElement[] descriptionElementArray6 = new DescriptionElement[9]
          {
            (DescriptionElement) "",
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
            (DescriptionElement) "",
            (DescriptionElement) "",
            (DescriptionElement) "",
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", (object) characterFromName),
            (DescriptionElement) "",
            (DescriptionElement) ""
          };
          this.parts.Clear();
          int index = this.random.Next(((IEnumerable<DescriptionElement>) source7).Count<DescriptionElement>());
          this.parts.Add(source7[index]);
          this.parts.Add(descriptionElementArray5[index]);
          this.parts.Add(descriptionElementArray6[index]);
        }
      }
      this.dialogueparts.Clear();
      this.dialogueparts.Add(this.random.NextDouble() < 0.3 || this.target.Value.Equals("Evelyn") ? (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13526" : (this.random.NextDouble() < 0.5 ? (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13527" : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13528", (object) Game1.player.Name)));
      this.dialogueparts.Add(this.random.NextDouble() < 0.3 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13530", (object) this.deliveryItem.Value) : (this.random.NextDouble() < 0.5 ? (DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13532" : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13533", this.random.NextDouble() < 0.3 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13534") : (this.random.NextDouble() < 0.5 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13535") : (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13536")))));
      this.dialogueparts.Add((DescriptionElement) (this.random.NextDouble() < 0.3 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13538" : (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13539" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13540")));
      this.dialogueparts.Add((DescriptionElement) (this.random.NextDouble() < 0.3 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13542" : (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13543" : "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13544")));
      if (this.target.Value.Equals("Wizard"))
      {
        this.parts.Clear();
        if (this.random.NextDouble() < 0.5)
          this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13546", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13548", (object) this.deliveryItem.Value));
        else
          this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13551", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13553", (object) this.deliveryItem.Value));
        this.dialogueparts.Clear();
        this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13555");
      }
      if (this.target.Value.Equals("Haley"))
      {
        this.parts.Clear();
        this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13557", (object) this.deliveryItem.Value) : ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13560", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13563", (object) this.deliveryItem.Value)));
        this.dialogueparts.Clear();
        this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13566");
      }
      if (this.target.Value.Equals("Sam"))
      {
        this.parts.Clear();
        this.parts.Add(this.random.NextDouble() < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13568", (object) this.deliveryItem.Value) : ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13571", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13574", (object) this.deliveryItem.Value)));
        this.dialogueparts.Clear();
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13577", (object) Game1.player.Name));
      }
      if (this.target.Value.Equals("Maru"))
      {
        this.parts.Clear();
        double num = this.random.NextDouble();
        this.parts.Add(num < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13580", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13583", (object) this.deliveryItem.Value));
        this.dialogueparts.Clear();
        this.dialogueparts.Add(num < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13585", (object) Game1.player.Name) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13587", (object) Game1.player.Name));
      }
      if (this.target.Value.Equals("Abigail"))
      {
        this.parts.Clear();
        double num = this.random.NextDouble();
        this.parts.Add(num < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13590", (object) this.deliveryItem.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13593", (object) this.deliveryItem.Value));
        this.dialogueparts.Add(num < 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13597", (object) Game1.player.Name) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13599", (object) Game1.player.Name));
      }
      if (this.target.Value.Equals("Sebastian"))
      {
        this.dialogueparts.Clear();
        this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13602");
      }
      if (this.target.Value.Equals("Elliott"))
      {
        this.dialogueparts.Clear();
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13604", (object) this.deliveryItem.Value, (object) Game1.player.Name));
      }
      DescriptionElement descriptionElement1 = this.random.NextDouble() >= 0.3 ? (this.random.NextDouble() >= 0.5 ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13612", (object) characterFromName) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13610", (object) characterFromName)) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13608", (object) characterFromName);
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13607", (object) ((int) (NetFieldBase<int, NetInt>) this.deliveryItem.Value.price * 3)));
      this.parts.Add(descriptionElement1);
      this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13614", (object) characterFromName, (object) this.deliveryItem.Value);
    }

    public override void reloadDescription()
    {
      if (this._questDescription == "")
        this.loadQuestInfo();
      string str1 = "";
      string str2 = "";
      if (this.parts != null && this.parts.Count != 0)
      {
        foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>) this.parts)
          str1 += part.loadDescriptionElement();
        this.questDescription = str1;
      }
      if (this.dialogueparts != null && this.dialogueparts.Count != 0)
      {
        foreach (DescriptionElement dialoguepart in (NetList<DescriptionElement, NetDescriptionElementRef>) this.dialogueparts)
          str2 += dialoguepart.loadDescriptionElement();
        this.targetMessage = str2;
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.id == 0)
          return;
        Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
        if (dictionary == null || !dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) this.id))
          return;
        string[] strArray = dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Split('/');
        if (strArray == null || strArray.Length < 9)
          return;
        this.targetMessage = strArray[9];
      }
    }

    public override void reloadObjective()
    {
      if (this.objective.Value == null)
        return;
      this.currentObjective = this.objective.Value.loadDescriptionElement();
    }

    public override bool checkIfComplete(
      NPC n = null,
      int number1 = -1,
      int number2 = -1,
      Item item = null,
      string monsterName = null)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed || item == null || !(item is StardewValley.Object) || n == null || !n.isVillager() || !n.Name.Equals(this.target.Value) || (item as StardewValley.Object).ParentSheetIndex != (int) (NetFieldBase<int, NetInt>) this.item && (item as StardewValley.Object).Category != (int) (NetFieldBase<int, NetInt>) this.item)
        return false;
      if (item.Stack >= (int) (NetFieldBase<int, NetInt>) this.number)
      {
        Game1.player.ActiveObject.Stack -= (int) (NetFieldBase<int, NetInt>) this.number - 1;
        this.reloadDescription();
        n.CurrentDialogue.Push(new Dialogue(this.targetMessage, n));
        Game1.drawDialogue(n);
        Game1.player.reduceActiveItemByOne();
        if ((bool) (NetFieldBase<bool, NetBool>) this.dailyQuest)
        {
          Game1.player.changeFriendship(150, n);
          if (this.deliveryItem.Value == null)
            this.deliveryItem.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.item, 1);
          this.moneyReward.Value = (int) (NetFieldBase<int, NetInt>) this.deliveryItem.Value.price * 3;
        }
        else
          Game1.player.changeFriendship((int) byte.MaxValue, n);
        this.questComplete();
        return true;
      }
      n.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13615", (object) this.number.Value), n));
      Game1.drawDialogue(n);
      return false;
    }
  }
}
