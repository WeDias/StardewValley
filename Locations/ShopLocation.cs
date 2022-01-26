// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.ShopLocation
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.BellsAndWhistles;

namespace StardewValley.Locations
{
  public class ShopLocation : GameLocation
  {
    public const int maxItemsToSellFromPlayer = 11;
    public readonly NetObjectList<Item> itemsFromPlayerToSell = new NetObjectList<Item>();
    public readonly NetObjectList<Item> itemsToStartSellingTomorrow = new NetObjectList<Item>();

    public ShopLocation()
    {
    }

    public ShopLocation(string map, string name)
      : base(map, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.itemsFromPlayerToSell, (INetSerializable) this.itemsToStartSellingTomorrow);
    }

    public virtual string getPurchasedItemDialogueForNPC(Object i, NPC n)
    {
      string itemDialogueForNpc = "...";
      string[] strArray = Game1.content.LoadString("Strings\\Lexicon:GenericPlayerTerm").Split('^');
      string str = strArray[0];
      if (strArray.Length > 1 && !(bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale)
        str = strArray[1];
      string sub1 = Game1.random.NextDouble() < (double) (Game1.player.getFriendshipLevelForNPC(n.Name) / 1250) ? Game1.player.Name : str;
      if (n.Age != 0)
        sub1 = Game1.player.Name;
      string sub2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? Lexicon.getProperArticleForWord(i.name) : "";
      if ((i.Category == -4 || i.Category == -75 || i.Category == -79) && Game1.random.NextDouble() < 0.5)
        sub2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:SeedShop.cs.9701");
      int num = Game1.random.Next(5);
      if (n.Manners == 2)
        num = 2;
      switch (num)
      {
        case 0:
          if (Game1.random.NextDouble() < (double) (int) (NetFieldBase<int, NetInt>) i.quality * 0.5 + 0.2)
          {
            itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_1_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomDeliciousAdjective(n));
            break;
          }
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_1_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomNegativeFoodAdjective(n));
          break;
        case 1:
          itemDialogueForNpc = (int) (NetFieldBase<int, NetInt>) i.quality != 0 ? (!n.Name.Equals("Jodi") ? Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_2_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName) : Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_2_QualityHigh_Jodi", (object) sub1, (object) sub2, (object) i.DisplayName)) : Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_2_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case 2:
          if (n.Manners == 2)
          {
            if ((int) (NetFieldBase<int, NetInt>) i.quality != 2)
            {
              itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_3_QualityLow_Rude", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice() / 2), (object) Lexicon.getRandomNegativeFoodAdjective(n), (object) Lexicon.getRandomNegativeItemSlanderNoun());
              break;
            }
            Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_3_QualityHigh_Rude", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice() / 2), (object) Lexicon.getRandomSlightlyPositiveAdjectiveForEdibleNoun(n));
            break;
          }
          Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_3_NonRude", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice() / 2));
          break;
        case 3:
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_4", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case 4:
          if (i.Category == -75 || i.Category == -79)
          {
            itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_5_VegetableOrFruit", (object) sub1, (object) sub2, (object) i.DisplayName);
            break;
          }
          if (i.Category == -7)
          {
            string forEventOrPerson = Lexicon.getRandomPositiveAdjectiveForEventOrPerson(n);
            itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_5_Cooking", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getProperArticleForWord(forEventOrPerson), (object) forEventOrPerson);
            break;
          }
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_5_Foraged", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
      }
      if (n.Age == 1 && Game1.random.NextDouble() < 0.6)
        itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Teen", (object) sub1, (object) sub2, (object) i.DisplayName);
      switch (n.Name)
      {
        case "Abigail":
          if ((int) (NetFieldBase<int, NetInt>) i.quality == 0)
          {
            itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Abigail_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomNegativeItemSlanderNoun());
            break;
          }
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Abigail_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case "Alex":
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Alex", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case "Caroline":
          itemDialogueForNpc = (int) (NetFieldBase<int, NetInt>) i.quality != 0 ? Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Caroline_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName) : Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Caroline_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case "Elliott":
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Elliott", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case "Haley":
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Haley", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case "Leah":
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Leah", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case "Pierre":
          itemDialogueForNpc = (int) (NetFieldBase<int, NetInt>) i.quality != 0 ? Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Pierre_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName) : Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Pierre_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
      }
      return itemDialogueForNpc;
    }

    public override void DayUpdate(int dayOfMonth)
    {
      for (int index = this.itemsToStartSellingTomorrow.Count - 1; index >= 0; --index)
      {
        Item obj1 = this.itemsToStartSellingTomorrow[index];
        if (this.itemsFromPlayerToSell.Count < 11)
        {
          bool flag = false;
          foreach (Item obj2 in (NetList<Item, NetRef<Item>>) this.itemsFromPlayerToSell)
          {
            if (obj2.Name.Equals(obj1.Name) && (NetFieldBase<int, NetInt>) (obj2 as Object).quality == (obj1 as Object).quality)
            {
              obj2.Stack += obj1.Stack;
              flag = true;
              break;
            }
          }
          this.itemsToStartSellingTomorrow.RemoveAt(index);
          if (!flag)
            this.itemsFromPlayerToSell.Add(obj1);
        }
      }
      base.DayUpdate(dayOfMonth);
    }
  }
}
