// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.AdventureGuild
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class AdventureGuild : GameLocation
  {
    private NPC Gil = new NPC((AnimatedSprite) null, new Vector2(-1000f, -1000f), nameof (AdventureGuild), 2, nameof (Gil), false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Gil"));
    private bool talkedToGil;

    public AdventureGuild()
    {
    }

    public AdventureGuild(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      switch (this.map.GetLayer("Buildings").Tiles[tileLocation] != null ? this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : -1)
      {
        case 1291:
        case 1292:
        case 1355:
        case 1356:
        case 1357:
        case 1358:
          this.gil();
          return true;
        case 1306:
          this.showMonsterKillList();
          return true;
        default:
          return base.checkAction(tileLocation, viewport, who);
      }
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.talkedToGil = false;
      if (Game1.player.mailReceived.Contains("guildMember"))
        return;
      Game1.player.mailReceived.Add("guildMember");
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (Game1.player.mailReceived.Contains("checkedMonsterBoard"))
        return;
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(504f, 464f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.064801f);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(544f, 504f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12)), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 0.06481f);
    }

    private string killListLine(string monsterType, int killCount, int target)
    {
      string sub3 = Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_" + monsterType);
      if (killCount == 0)
        return Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_None", (object) killCount, (object) target, (object) sub3) + "^";
      return killCount >= target ? Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_OverTarget", (object) killCount, (object) target, (object) sub3) + "^" : Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat", (object) killCount, (object) target, (object) sub3) + "^";
    }

    public void showMonsterKillList()
    {
      if (!Game1.player.mailReceived.Contains("checkedMonsterBoard"))
        Game1.player.mailReceived.Add("checkedMonsterBoard");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Header").Replace('\n', '^') + "^");
      int killCount1 = Game1.stats.getMonstersKilled("Green Slime") + Game1.stats.getMonstersKilled("Frost Jelly") + Game1.stats.getMonstersKilled("Sludge") + Game1.stats.getMonstersKilled("Tiger Slime");
      int killCount2 = Game1.stats.getMonstersKilled("Shadow Guy") + Game1.stats.getMonstersKilled("Shadow Shaman") + Game1.stats.getMonstersKilled("Shadow Brute") + Game1.stats.getMonstersKilled("Shadow Sniper");
      int killCount3 = Game1.stats.getMonstersKilled("Skeleton") + Game1.stats.getMonstersKilled("Skeleton Mage");
      int killCount4 = Game1.stats.getMonstersKilled("Rock Crab") + Game1.stats.getMonstersKilled("Lava Crab") + Game1.stats.getMonstersKilled("Iridium Crab");
      int killCount5 = Game1.stats.getMonstersKilled("Grub") + Game1.stats.getMonstersKilled("Fly") + Game1.stats.getMonstersKilled("Bug");
      int killCount6 = Game1.stats.getMonstersKilled("Bat") + Game1.stats.getMonstersKilled("Frost Bat") + Game1.stats.getMonstersKilled("Lava Bat") + Game1.stats.getMonstersKilled("Iridium Bat");
      int killCount7 = Game1.stats.getMonstersKilled("Duggy") + Game1.stats.getMonstersKilled("Magma Duggy");
      int monstersKilled1 = Game1.stats.getMonstersKilled("Dust Spirit");
      int monstersKilled2 = Game1.stats.getMonstersKilled("Mummy");
      int monstersKilled3 = Game1.stats.getMonstersKilled("Pepper Rex");
      int killCount8 = Game1.stats.getMonstersKilled("Serpent") + Game1.stats.getMonstersKilled("Royal Serpent");
      int killCount9 = Game1.stats.getMonstersKilled("Magma Sprite") + Game1.stats.getMonstersKilled("Magma Sparker");
      stringBuilder.Append(this.killListLine("Slimes", killCount1, 1000));
      stringBuilder.Append(this.killListLine("VoidSpirits", killCount2, 150));
      stringBuilder.Append(this.killListLine("Bats", killCount6, 200));
      stringBuilder.Append(this.killListLine("Skeletons", killCount3, 50));
      stringBuilder.Append(this.killListLine("CaveInsects", killCount5, 125));
      stringBuilder.Append(this.killListLine("Duggies", killCount7, 30));
      stringBuilder.Append(this.killListLine("DustSprites", monstersKilled1, 500));
      stringBuilder.Append(this.killListLine("RockCrabs", killCount4, 60));
      stringBuilder.Append(this.killListLine("Mummies", monstersKilled2, 100));
      stringBuilder.Append(this.killListLine("PepperRex", monstersKilled3, 50));
      stringBuilder.Append(this.killListLine("Serpent", killCount8, 250));
      stringBuilder.Append(this.killListLine("MagmaSprite", killCount9, 150));
      stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Footer").Replace('\n', '^'));
      Game1.drawLetterMessage(stringBuilder.ToString());
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      Game1.changeMusicTrack("none");
    }

    public static bool areAllMonsterSlayerQuestsComplete()
    {
      int num1 = Game1.stats.getMonstersKilled("Green Slime") + Game1.stats.getMonstersKilled("Frost Jelly") + Game1.stats.getMonstersKilled("Sludge") + Game1.stats.getMonstersKilled("Tiger Slime");
      int num2 = Game1.stats.getMonstersKilled("Shadow Guy") + Game1.stats.getMonstersKilled("Shadow Shaman") + Game1.stats.getMonstersKilled("Shadow Brute") + Game1.stats.getMonstersKilled("Shadow Sniper");
      int num3 = Game1.stats.getMonstersKilled("Skeleton") + Game1.stats.getMonstersKilled("Skeleton Mage");
      int num4 = Game1.stats.getMonstersKilled("Rock Crab") + Game1.stats.getMonstersKilled("Lava Crab") + Game1.stats.getMonstersKilled("Iridium Crab");
      int num5 = Game1.stats.getMonstersKilled("Grub") + Game1.stats.getMonstersKilled("Fly") + Game1.stats.getMonstersKilled("Bug");
      int num6 = Game1.stats.getMonstersKilled("Bat") + Game1.stats.getMonstersKilled("Frost Bat") + Game1.stats.getMonstersKilled("Lava Bat") + Game1.stats.getMonstersKilled("Iridium Bat");
      int num7 = Game1.stats.getMonstersKilled("Duggy") + Game1.stats.getMonstersKilled("Magma Duggy");
      Game1.stats.getMonstersKilled("Metal Head");
      Game1.stats.getMonstersKilled("Stone Golem");
      int monstersKilled1 = Game1.stats.getMonstersKilled("Dust Spirit");
      int monstersKilled2 = Game1.stats.getMonstersKilled("Mummy");
      int monstersKilled3 = Game1.stats.getMonstersKilled("Pepper Rex");
      int num8 = Game1.stats.getMonstersKilled("Serpent") + Game1.stats.getMonstersKilled("Royal Serpent");
      int num9 = Game1.stats.getMonstersKilled("Magma Sprite") + Game1.stats.getMonstersKilled("Magma Sparker");
      return num1 >= 1000 && num2 >= 150 && num3 >= 50 && num5 >= 125 && num6 >= 200 && num7 >= 30 && monstersKilled1 >= 500 && num4 >= 60 && monstersKilled2 >= 100 && monstersKilled3 >= 50 && num8 >= 250 && num9 >= 150;
    }

    public static bool willThisKillCompleteAMonsterSlayerQuest(string nameOfMonster)
    {
      int num1 = Game1.stats.getMonstersKilled("Green Slime") + Game1.stats.getMonstersKilled("Frost Jelly") + Game1.stats.getMonstersKilled("Sludge") + Game1.stats.getMonstersKilled("Tiger Slime");
      int num2 = Game1.stats.getMonstersKilled("Shadow Guy") + Game1.stats.getMonstersKilled("Shadow Shaman") + Game1.stats.getMonstersKilled("Shadow Brute") + Game1.stats.getMonstersKilled("Shadow Sniper");
      int num3 = Game1.stats.getMonstersKilled("Skeleton") + Game1.stats.getMonstersKilled("Skeleton Mage");
      int num4 = Game1.stats.getMonstersKilled("Rock Crab") + Game1.stats.getMonstersKilled("Lava Crab") + Game1.stats.getMonstersKilled("Iridium Crab");
      int num5 = Game1.stats.getMonstersKilled("Grub") + Game1.stats.getMonstersKilled("Fly") + Game1.stats.getMonstersKilled("Bug");
      int num6 = Game1.stats.getMonstersKilled("Bat") + Game1.stats.getMonstersKilled("Frost Bat") + Game1.stats.getMonstersKilled("Lava Bat") + Game1.stats.getMonstersKilled("Iridium Bat");
      int num7 = Game1.stats.getMonstersKilled("Duggy") + Game1.stats.getMonstersKilled("Magma Duggy");
      Game1.stats.getMonstersKilled("Metal Head");
      Game1.stats.getMonstersKilled("Stone Golem");
      int monstersKilled1 = Game1.stats.getMonstersKilled("Dust Spirit");
      int monstersKilled2 = Game1.stats.getMonstersKilled("Mummy");
      int monstersKilled3 = Game1.stats.getMonstersKilled("Pepper Rex");
      int num8 = Game1.stats.getMonstersKilled("Serpent") + Game1.stats.getMonstersKilled("Royal Serpent");
      int num9 = Game1.stats.getMonstersKilled("Magma Sprite") + Game1.stats.getMonstersKilled("Magma Sparker");
      int num10 = num1 + (nameOfMonster.Equals("Green Slime") || nameOfMonster.Equals("Frost Jelly") || nameOfMonster.Equals("Sludge") || nameOfMonster.Equals("Tiger Slime") ? 1 : 0);
      int num11 = num2 + (nameOfMonster.Equals("Shadow Guy") || nameOfMonster.Equals("Shadow Shaman") || nameOfMonster.Equals("Shadow Brute") || nameOfMonster.Equals("Shadow Sniper") ? 1 : 0);
      int num12 = num3 + (nameOfMonster.Equals("Skeleton") || nameOfMonster.Equals("Skeleton Mage") ? 1 : 0);
      int num13 = num4 + (nameOfMonster.Equals("Rock Crab") || nameOfMonster.Equals("Lava Crab") || nameOfMonster.Equals("Iridium Crab") ? 1 : 0);
      int num14 = num5 + (nameOfMonster.Equals("Grub") || nameOfMonster.Equals("Fly") || nameOfMonster.Equals("Bug") ? 1 : 0);
      int num15 = num6 + (nameOfMonster.Equals("Bat") || nameOfMonster.Equals("Frost Bat") || nameOfMonster.Equals("Lava Bat") ? 1 : 0);
      int num16 = num7 + (nameOfMonster.Contains("Duggy") ? 1 : 0);
      nameOfMonster.Equals("Metal Head");
      nameOfMonster.Equals("Stone Golem");
      int num17 = monstersKilled1 + (nameOfMonster.Equals("Dust Spirit") ? 1 : 0);
      int num18 = monstersKilled2 + (nameOfMonster.Equals("Mummy") ? 1 : 0);
      int num19 = monstersKilled3 + (nameOfMonster.Equals("Pepper Rex") ? 1 : 0);
      int num20 = num8 + (nameOfMonster.Contains("Serpent") ? 1 : 0);
      int num21 = num9 + (nameOfMonster.Equals("Magma Sprite") || nameOfMonster.Equals("Magma Sparker") ? 1 : 0);
      return num1 < 1000 && num10 >= 1000 && !Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring") || num2 < 150 && num11 >= 150 && !Game1.player.mailReceived.Contains("Gil_Savage Ring") || num3 < 50 && num12 >= 50 && !Game1.player.mailReceived.Contains("Gil_Skeleton Mask") || num5 < 125 && num14 >= 125 && !Game1.player.mailReceived.Contains("Gil_Insect Head") || num6 < 200 && num15 >= 200 && !Game1.player.mailReceived.Contains("Gil_Vampire Ring") || num7 < 30 && num16 >= 30 && !Game1.player.mailReceived.Contains("Gil_Hard Hat") || monstersKilled1 < 500 && num17 >= 500 && !Game1.player.mailReceived.Contains("Gil_Burglar's Ring") || num4 < 60 && num13 >= 60 && !Game1.player.mailReceived.Contains("Gil_Crabshell Ring") || monstersKilled2 < 100 && num18 >= 100 && !Game1.player.mailReceived.Contains("Gil_Arcane Hat") || monstersKilled3 < 50 && num19 >= 50 && !Game1.player.mailReceived.Contains("Gil_Knight's Helmet") || num8 < 250 && num20 >= 250 && !Game1.player.mailReceived.Contains("Gil_Napalm Ring") || num9 < 150 && num21 >= 150 && !Game1.player.mailReceived.Contains("Gil_Telephone");
    }

    public void onRewardCollected(Item item, Farmer who)
    {
      if (item == null || who.hasOrWillReceiveMail("Gil_" + item.Name))
        return;
      who.mailReceived.Add("Gil_" + item.Name);
    }

    private void gil()
    {
      List<Item> inventory = new List<Item>();
      int num1 = Game1.stats.getMonstersKilled("Green Slime") + Game1.stats.getMonstersKilled("Frost Jelly") + Game1.stats.getMonstersKilled("Sludge") + Game1.stats.getMonstersKilled("Tiger Slime");
      int num2 = Game1.stats.getMonstersKilled("Shadow Guy") + Game1.stats.getMonstersKilled("Shadow Shaman") + Game1.stats.getMonstersKilled("Shadow Brute") + Game1.stats.getMonstersKilled("Shadow Sniper");
      int num3 = Game1.stats.getMonstersKilled("Skeleton") + Game1.stats.getMonstersKilled("Skeleton Mage");
      int num4 = Game1.stats.getMonstersKilled("Goblin Warrior") + Game1.stats.getMonstersKilled("Goblin Wizard");
      int num5 = Game1.stats.getMonstersKilled("Rock Crab") + Game1.stats.getMonstersKilled("Lava Crab") + Game1.stats.getMonstersKilled("Iridium Crab");
      int num6 = Game1.stats.getMonstersKilled("Grub") + Game1.stats.getMonstersKilled("Fly") + Game1.stats.getMonstersKilled("Bug");
      int num7 = Game1.stats.getMonstersKilled("Bat") + Game1.stats.getMonstersKilled("Frost Bat") + Game1.stats.getMonstersKilled("Lava Bat") + Game1.stats.getMonstersKilled("Iridium Bat");
      int num8 = Game1.stats.getMonstersKilled("Duggy") + Game1.stats.getMonstersKilled("Magma Duggy");
      int monstersKilled1 = Game1.stats.getMonstersKilled("Metal Head");
      int monstersKilled2 = Game1.stats.getMonstersKilled("Stone Golem");
      int monstersKilled3 = Game1.stats.getMonstersKilled("Dust Spirit");
      int monstersKilled4 = Game1.stats.getMonstersKilled("Mummy");
      int monstersKilled5 = Game1.stats.getMonstersKilled("Pepper Rex");
      int num9 = Game1.stats.getMonstersKilled("Serpent") + Game1.stats.getMonstersKilled("Royal Serpent");
      int num10 = Game1.stats.getMonstersKilled("Magma Sprite") + Game1.stats.getMonstersKilled("Magma Sparker");
      if (num1 >= 1000 && !Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring"))
        inventory.Add((Item) new Ring(520));
      if (num2 >= 150 && !Game1.player.mailReceived.Contains("Gil_Savage Ring"))
        inventory.Add((Item) new Ring(523));
      if (num3 >= 50 && !Game1.player.mailReceived.Contains("Gil_Skeleton Mask"))
        inventory.Add((Item) new Hat(8));
      if (num4 >= 50)
        Game1.player.specialItems.Contains(9);
      if (num5 >= 60 && !Game1.player.mailReceived.Contains("Gil_Crabshell Ring"))
        inventory.Add((Item) new Ring(810));
      if (num6 >= 125 && !Game1.player.mailReceived.Contains("Gil_Insect Head"))
        inventory.Add((Item) new MeleeWeapon(13));
      if (num7 >= 200 && !Game1.player.mailReceived.Contains("Gil_Vampire Ring"))
        inventory.Add((Item) new Ring(522));
      if (num8 >= 30 && !Game1.player.mailReceived.Contains("Gil_Hard Hat"))
        inventory.Add((Item) new Hat(27));
      if (monstersKilled1 >= 50)
        Game1.player.specialItems.Contains(519);
      if (monstersKilled2 >= 50)
        Game1.player.specialItems.Contains(517);
      if (monstersKilled3 >= 500 && !Game1.player.mailReceived.Contains("Gil_Burglar's Ring"))
        inventory.Add((Item) new Ring(526));
      if (monstersKilled4 >= 100 && !Game1.player.mailReceived.Contains("Gil_Arcane Hat"))
        inventory.Add((Item) new Hat(60));
      if (monstersKilled5 >= 50 && !Game1.player.mailReceived.Contains("Gil_Knight's Helmet"))
        inventory.Add((Item) new Hat(50));
      if (num9 >= 250 && !Game1.player.mailReceived.Contains("Gil_Napalm Ring"))
        inventory.Add((Item) new Ring(811));
      if (num10 >= 150 && !Game1.player.mailReceived.Contains("Gil_Telephone"))
      {
        Game1.addMail("Gil_Telephone", true, true);
        Game1.drawDialogue(this.Gil, Game1.content.LoadString("Strings\\Locations:Gil_Telephone"));
      }
      else
      {
        foreach (Item obj in inventory)
        {
          if (obj is StardewValley.Object)
            (obj as StardewValley.Object).specialItem = true;
        }
        if (inventory.Count > 0)
        {
          Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) inventory, (object) this)
          {
            behaviorOnItemGrab = new ItemGrabMenu.behaviorOnItemSelect(this.onRewardCollected)
          };
        }
        else
        {
          if (this.talkedToGil)
            Game1.drawDialogue(this.Gil, Game1.content.LoadString("Characters\\Dialogue\\Gil:Snoring"));
          else
            Game1.drawDialogue(this.Gil, Game1.content.LoadString("Characters\\Dialogue\\Gil:ComeBackLater"));
          this.talkedToGil = true;
        }
      }
    }
  }
}
