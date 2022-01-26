// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.SeedShop
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewValley.Locations
{
  public class SeedShop : ShopLocation
  {
    protected bool _stockListGranted;

    public SeedShop()
    {
    }

    public SeedShop(string map, string name)
      : base(map, name)
    {
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems == 12)
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(456f, 1088f)), new Rectangle?(new Rectangle((int) byte.MaxValue, 1436, 12, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1232f);
      else if ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems < 36)
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(456f, 1088f)), new Rectangle?(new Rectangle(267, 1436, 12, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1232f);
      else
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Rectangle(452, 1184, 112, 20)), new Rectangle?(new Rectangle(258, 1449, 1, 1)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1232f);
    }

    public override string getPurchasedItemDialogueForNPC(StardewValley.Object i, NPC n)
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

    private void addStock(
      Dictionary<ISalable, int[]> stock,
      int parentSheetIndex,
      int buyPrice = -1,
      string item_season = null)
    {
      float num1 = 2f;
      int num2 = buyPrice;
      StardewValley.Object key1 = new StardewValley.Object(Vector2.Zero, parentSheetIndex, 1);
      if (buyPrice == -1)
      {
        num2 = key1.salePrice();
        num1 = 1f;
      }
      else if (key1.isSapling())
        num1 *= Game1.MasterPlayer.difficultyModifier;
      if (item_season != null && item_season != Game1.currentSeason)
      {
        if (!Game1.MasterPlayer.hasOrWillReceiveMail("PierreStocklist"))
          return;
        num1 *= 1.5f;
      }
      int num3 = (int) ((double) num2 * (double) num1);
      if (item_season != null)
      {
        foreach (KeyValuePair<ISalable, int[]> keyValuePair in stock)
        {
          if (keyValuePair.Key != null && keyValuePair.Key is StardewValley.Object)
          {
            StardewValley.Object key2 = keyValuePair.Key as StardewValley.Object;
            if (Utility.IsNormalObjectAtParentSheetIndex((Item) key2, parentSheetIndex))
            {
              if (keyValuePair.Value.Length == 0 || num3 >= keyValuePair.Value[0])
                return;
              keyValuePair.Value[0] = num3;
              stock[(ISalable) key2] = keyValuePair.Value;
              return;
            }
          }
        }
      }
      stock.Add((ISalable) key1, new int[2]
      {
        num3,
        int.MaxValue
      });
    }

    public Dictionary<ISalable, int[]> shopStock()
    {
      Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
      this.addStock(stock, 472, item_season: "spring");
      this.addStock(stock, 473, item_season: "spring");
      this.addStock(stock, 474, item_season: "spring");
      this.addStock(stock, 475, item_season: "spring");
      this.addStock(stock, 427, item_season: "spring");
      this.addStock(stock, 477, item_season: "spring");
      this.addStock(stock, 429, item_season: "spring");
      if (Game1.year > 1)
      {
        this.addStock(stock, 476, item_season: "spring");
        this.addStock(stock, 273, item_season: "spring");
      }
      this.addStock(stock, 479, item_season: "summer");
      this.addStock(stock, 480, item_season: "summer");
      this.addStock(stock, 481, item_season: "summer");
      this.addStock(stock, 482, item_season: "summer");
      this.addStock(stock, 483, item_season: "summer");
      this.addStock(stock, 484, item_season: "summer");
      this.addStock(stock, 453, item_season: "summer");
      this.addStock(stock, 455, item_season: "summer");
      this.addStock(stock, 302, item_season: "summer");
      this.addStock(stock, 487, item_season: "summer");
      this.addStock(stock, 431, 100, "summer");
      if (Game1.year > 1)
        this.addStock(stock, 485, item_season: "summer");
      this.addStock(stock, 490, item_season: "fall");
      this.addStock(stock, 487, item_season: "fall");
      this.addStock(stock, 488, item_season: "fall");
      this.addStock(stock, 491, item_season: "fall");
      this.addStock(stock, 492, item_season: "fall");
      this.addStock(stock, 493, item_season: "fall");
      this.addStock(stock, 483, item_season: "fall");
      this.addStock(stock, 431, 100, "fall");
      this.addStock(stock, 425, item_season: "fall");
      this.addStock(stock, 299, item_season: "fall");
      this.addStock(stock, 301, item_season: "fall");
      if (Game1.year > 1)
        this.addStock(stock, 489, item_season: "fall");
      this.addStock(stock, 297);
      if (!Game1.player.craftingRecipes.ContainsKey("Grass Starter"))
        stock.Add((ISalable) new StardewValley.Object(297, 1, true), new int[2]
        {
          1000,
          1
        });
      this.addStock(stock, 245);
      this.addStock(stock, 246);
      this.addStock(stock, 423);
      this.addStock(stock, 247);
      this.addStock(stock, 419);
      if ((int) Game1.stats.DaysPlayed >= 15)
      {
        this.addStock(stock, 368, 50);
        this.addStock(stock, 370, 50);
        this.addStock(stock, 465, 50);
      }
      if (Game1.year > 1)
      {
        this.addStock(stock, 369, 75);
        this.addStock(stock, 371, 75);
        this.addStock(stock, 466, 75);
      }
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      int which = random.Next(112);
      if (which == 21)
        which = 36;
      Wallpaper key1 = new Wallpaper(which);
      stock.Add((ISalable) key1, new int[2]
      {
        key1.salePrice(),
        int.MaxValue
      });
      Wallpaper key2 = new Wallpaper(random.Next(56), true);
      stock.Add((ISalable) key2, new int[2]
      {
        key2.salePrice(),
        int.MaxValue
      });
      Furniture key3 = new Furniture(1308, Vector2.Zero);
      stock.Add((ISalable) key3, new int[2]
      {
        key3.salePrice(),
        int.MaxValue
      });
      this.addStock(stock, 628, 1700);
      this.addStock(stock, 629, 1000);
      this.addStock(stock, 630, 2000);
      this.addStock(stock, 631, 3000);
      this.addStock(stock, 632, 3000);
      this.addStock(stock, 633, 2000);
      foreach (Item key4 in (NetList<Item, NetRef<Item>>) this.itemsFromPlayerToSell)
      {
        if (key4.Stack > 0)
        {
          int num = key4.salePrice();
          if (key4 is StardewValley.Object)
            num = (key4 as StardewValley.Object).sellToStorePrice();
          stock.Add((ISalable) key4, new int[2]
          {
            num,
            key4.Stack
          });
        }
      }
      if (Game1.player.hasAFriendWithHeartLevel(8, true))
        this.addStock(stock, 458);
      return stock;
    }
  }
}
