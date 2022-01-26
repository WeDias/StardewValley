// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.FishShop
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.BellsAndWhistles;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class FishShop : ShopLocation
  {
    public FishShop()
    {
    }

    public FishShop(string map, string name)
      : base(map, name)
    {
    }

    public override string getPurchasedItemDialogueForNPC(Object i, NPC n)
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
        case 4:
          if (Game1.random.NextDouble() < (double) (int) (NetFieldBase<int, NetInt>) i.quality * 0.5 + 0.2)
          {
            itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_1_QualityHigh_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomDeliciousAdjective(n));
            break;
          }
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_1_QualityLow_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomNegativeFoodAdjective(n));
          break;
        case 1:
          itemDialogueForNpc = (int) (NetFieldBase<int, NetInt>) i.quality != 0 ? (!n.Name.Equals("Jodi") ? Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_2_QualityHigh_Willy", (object) sub1, (object) sub2, (object) i.DisplayName) : Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_2_QualityHigh_Jodi_Willy", (object) sub1, (object) sub2, (object) i.DisplayName)) : Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_2_QualityLow_Willy", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
        case 2:
          if (n.Manners == 2)
          {
            if ((int) (NetFieldBase<int, NetInt>) i.quality != 2)
            {
              itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_3_QualityLow_Rude_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice() / 2), (object) Lexicon.getRandomNegativeFoodAdjective(n), (object) Lexicon.getRandomNegativeItemSlanderNoun());
              break;
            }
            Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_3_QualityHigh_Rude_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice() / 2), (object) Lexicon.getRandomSlightlyPositiveAdjectiveForEdibleNoun(n));
            break;
          }
          Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_3_NonRude_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice() / 2));
          break;
        case 3:
          itemDialogueForNpc = Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_4_Willy", (object) sub1, (object) sub2, (object) i.DisplayName);
          break;
      }
      if (n.Name == "Willy")
        itemDialogueForNpc = (int) (NetFieldBase<int, NetInt>) i.quality != 0 ? Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Pierre_QualityHigh_Willy", (object) sub1, (object) sub2, (object) i.DisplayName) : Game1.content.LoadString("Data\\ExtraDialogue:PurchasedItem_Pierre_QualityLow_Willy", (object) sub1, (object) sub2, (object) i.DisplayName);
      return itemDialogueForNpc;
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action == "WarpBoatTunnel")
      {
        if (Game1.player.mailReceived.Contains("willyBackRoomInvitation"))
        {
          Game1.warpFarmer("BoatTunnel", 6, 12, false);
          this.playSound("doorClose");
        }
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
      }
      return base.performAction(action, who, tileLocation);
    }
  }
}
