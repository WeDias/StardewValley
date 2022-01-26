// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SkillsPage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class SkillsPage : IClickableMenu
  {
    public const int region_special1 = 10201;
    public const int region_special2 = 10202;
    public const int region_special3 = 10203;
    public const int region_special4 = 10204;
    public const int region_special5 = 10205;
    public const int region_special6 = 10206;
    public const int region_special7 = 10207;
    public const int region_special8 = 10208;
    public const int region_special9 = 10209;
    public const int region_special_skullkey = 10210;
    public const int region_special_townkey = 10211;
    public const int region_skillArea1 = 0;
    public const int region_skillArea2 = 1;
    public const int region_skillArea3 = 2;
    public const int region_skillArea4 = 3;
    public const int region_skillArea5 = 4;
    public List<ClickableTextureComponent> skillBars = new List<ClickableTextureComponent>();
    public List<ClickableTextureComponent> skillAreas = new List<ClickableTextureComponent>();
    public List<ClickableTextureComponent> specialItems = new List<ClickableTextureComponent>();
    private string hoverText = "";
    private string hoverTitle = "";
    private int professionImage = -1;
    private int playerPanelIndex;
    private int playerPanelTimer;
    private Rectangle playerPanel;
    private int[] playerPanelFrames = new int[4]
    {
      0,
      1,
      0,
      2
    };

    public SkillsPage(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
      int x1 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 80;
      int y1 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int) ((double) height / 2.0) + 80;
      this.playerPanel = new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 128, 192);
      if (Game1.player.canUnderstandDwarves)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11587"), Game1.mouseCursors, new Rectangle(129, 320, 16, 16), 4f, true);
        textureComponent.myID = 10201;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.hasRustyKey)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 68, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11588"), Game1.mouseCursors, new Rectangle(145, 320, 16, 16), 4f, true);
        textureComponent.myID = 10202;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.hasClubCard)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 136, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11589"), Game1.mouseCursors, new Rectangle(161, 320, 16, 16), 4f, true);
        textureComponent.myID = 10203;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.hasSpecialCharm)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 204, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11590"), Game1.mouseCursors, new Rectangle(176, 320, 16, 16), 4f, true);
        textureComponent.myID = 10204;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.hasSkullKey)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 272, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11591"), Game1.mouseCursors, new Rectangle(192, 320, 16, 16), 4f, true);
        textureComponent.myID = 10210;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.hasMagnifyingGlass)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 340, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.magnifyingglass"), Game1.mouseCursors, new Rectangle(208, 320, 16, 16), 4f, true);
        textureComponent.myID = 10205;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.hasDarkTalisman)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 408, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\Objects:DarkTalisman"), Game1.mouseCursors, new Rectangle(225, 320, 16, 16), 4f, true);
        textureComponent.myID = 10206;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.hasMagicInk)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 476, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\Objects:MagicInk"), Game1.mouseCursors, new Rectangle(241, 320, 16, 16), 4f, true);
        textureComponent.myID = 10207;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.eventsSeen.Contains(2120303))
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 544, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\Objects:BearPaw"), Game1.mouseCursors, new Rectangle(192, 336, 16, 16), 4f, true);
        textureComponent.myID = 10208;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.eventsSeen.Contains(3910979))
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 612, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\Objects:SpringOnionBugs"), Game1.mouseCursors, new Rectangle(208, 336, 16, 16), 4f, true);
        textureComponent.myID = 10209;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      if (Game1.player.HasTownKey)
      {
        List<ClickableTextureComponent> specialItems = this.specialItems;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(x1, y1, 64, 64), (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:KeyToTheTown"), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 912, 16, 16), 4f, true);
        textureComponent.myID = 10211;
        textureComponent.rightNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.upNeighborID = 4;
        specialItems.Add(textureComponent);
      }
      else
        this.specialItems.Add((ClickableTextureComponent) null);
      int num1 = 680 / this.specialItems.Count;
      for (int index = 0; index < this.specialItems.Count; ++index)
      {
        if (this.specialItems[index] != null)
          this.specialItems[index].bounds.X = x1 + index * num1;
      }
      ClickableComponent.SetUpNeighbors<ClickableTextureComponent>(this.specialItems, 4);
      ClickableComponent.ChainNeighborsLeftRight<ClickableTextureComponent>(this.specialItems);
      int num2 = 0;
      int num3 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it ? this.xPositionOnScreen + width - 448 - 48 + 4 : this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 4;
      int num4 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 12;
      for (int index1 = 4; index1 < 10; index1 += 5)
      {
        for (int index2 = 0; index2 < 5; ++index2)
        {
          string professionBlurb = "";
          string professionTitle = "";
          bool flag = false;
          int whichProfession = -1;
          switch (index2)
          {
            case 0:
              flag = Game1.player.FarmingLevel > index1;
              whichProfession = Game1.player.getProfessionForSkill(0, index1 + 1);
              this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
              break;
            case 1:
              flag = Game1.player.MiningLevel > index1;
              whichProfession = Game1.player.getProfessionForSkill(3, index1 + 1);
              this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
              break;
            case 2:
              flag = Game1.player.ForagingLevel > index1;
              whichProfession = Game1.player.getProfessionForSkill(2, index1 + 1);
              this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
              break;
            case 3:
              flag = Game1.player.FishingLevel > index1;
              whichProfession = Game1.player.getProfessionForSkill(1, index1 + 1);
              this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
              break;
            case 4:
              flag = Game1.player.CombatLevel > index1;
              whichProfession = Game1.player.getProfessionForSkill(4, index1 + 1);
              this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
              break;
            case 5:
              flag = Game1.player.LuckLevel > index1;
              whichProfession = Game1.player.getProfessionForSkill(5, index1 + 1);
              this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
              break;
          }
          if (flag && (index1 + 1) % 5 == 0)
          {
            List<ClickableTextureComponent> skillBars = this.skillBars;
            ClickableTextureComponent textureComponent = new ClickableTextureComponent(whichProfession.ToString() ?? "", new Rectangle(num2 + num3 - 4 + index1 * 36, num4 + index2 * 56, 56, 36), (string) null, professionBlurb, Game1.mouseCursors, new Rectangle(159, 338, 14, 9), 4f, true);
            textureComponent.myID = index1 + 1 == 5 ? 100 + index2 : 200 + index2;
            textureComponent.leftNeighborID = index1 + 1 == 5 ? index2 : 100 + index2;
            textureComponent.rightNeighborID = index1 + 1 == 5 ? 200 + index2 : -1;
            textureComponent.downNeighborID = -99998;
            skillBars.Add(textureComponent);
          }
        }
        num2 += 24;
      }
      for (int index = 0; index < this.skillBars.Count; ++index)
      {
        if (index < this.skillBars.Count - 1 && Math.Abs(this.skillBars[index + 1].myID - this.skillBars[index].myID) < 50)
        {
          this.skillBars[index].downNeighborID = this.skillBars[index + 1].myID;
          this.skillBars[index + 1].upNeighborID = this.skillBars[index].myID;
        }
      }
      if (this.skillBars.Count > 1 && this.skillBars.Last<ClickableTextureComponent>().myID >= 200 && this.skillBars[this.skillBars.Count - 2].myID >= 200)
        this.skillBars.Last<ClickableTextureComponent>().upNeighborID = this.skillBars[this.skillBars.Count - 2].myID;
      for (int index = 0; index < 5; ++index)
      {
        int num5 = index;
        switch (num5)
        {
          case 1:
            num5 = 3;
            break;
          case 3:
            num5 = 1;
            break;
        }
        string hoverText = "";
        switch (num5)
        {
          case 0:
            if (Game1.player.FarmingLevel > 0)
            {
              hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11592", (object) Game1.player.FarmingLevel) + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11594", (object) Game1.player.FarmingLevel);
              break;
            }
            break;
          case 1:
            if (Game1.player.FishingLevel > 0)
            {
              hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11598", (object) Game1.player.FishingLevel);
              break;
            }
            break;
          case 2:
            if (Game1.player.ForagingLevel > 0)
            {
              hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11596", (object) Game1.player.ForagingLevel);
              break;
            }
            break;
          case 3:
            if (Game1.player.MiningLevel > 0)
            {
              hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11600", (object) Game1.player.MiningLevel);
              break;
            }
            break;
          case 4:
            if (Game1.player.CombatLevel > 0)
            {
              hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11602", (object) (Game1.player.CombatLevel * 5));
              break;
            }
            break;
        }
        List<ClickableTextureComponent> skillAreas = this.skillAreas;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(num5.ToString() ?? "", new Rectangle(num3 - 128 - 48, num4 + index * 56, 148, 36), num5.ToString() ?? "", hoverText, (Texture2D) null, Rectangle.Empty, 1f);
        textureComponent.myID = index;
        textureComponent.downNeighborID = index < 4 ? index + 1 : -99998;
        textureComponent.upNeighborID = index > 0 ? index - 1 : 12341;
        textureComponent.rightNeighborID = 100 + index;
        skillAreas.Add(textureComponent);
      }
    }

    private void parseProfessionDescription(
      ref string professionBlurb,
      ref string professionTitle,
      List<string> professionDescription)
    {
      if (professionDescription.Count <= 0)
        return;
      professionTitle = professionDescription[0];
      for (int index = 1; index < professionDescription.Count; ++index)
      {
        professionBlurb += professionDescription[index];
        if (index < professionDescription.Count - 1)
          professionBlurb += Environment.NewLine;
      }
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.skillAreas.Count > 0 ? this.getComponentWithID(0) : (ClickableComponent) null;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      this.hoverTitle = "";
      this.professionImage = -1;
      foreach (ClickableTextureComponent specialItem in this.specialItems)
      {
        if (specialItem != null && specialItem.containsPoint(x, y))
        {
          this.hoverText = specialItem.hoverText;
          break;
        }
      }
      foreach (ClickableTextureComponent skillBar in this.skillBars)
      {
        skillBar.scale = 4f;
        if (skillBar.containsPoint(x, y) && skillBar.hoverText.Length > 0 && !skillBar.name.Equals("-1"))
        {
          this.hoverText = skillBar.hoverText;
          this.hoverTitle = LevelUpMenu.getProfessionTitleFromNumber(Convert.ToInt32(skillBar.name));
          this.professionImage = Convert.ToInt32(skillBar.name);
          skillBar.scale = 0.0f;
        }
      }
      foreach (ClickableTextureComponent skillArea in this.skillAreas)
      {
        if (skillArea.containsPoint(x, y) && skillArea.hoverText.Length > 0)
        {
          this.hoverText = skillArea.hoverText;
          this.hoverTitle = Farmer.getSkillDisplayNameFromIndex(Convert.ToInt32(skillArea.name));
          break;
        }
      }
      if (this.playerPanel.Contains(x, y))
      {
        this.playerPanelTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (this.playerPanelTimer > 0)
          return;
        this.playerPanelIndex = (this.playerPanelIndex + 1) % 4;
        this.playerPanelTimer = 150;
      }
      else
        this.playerPanelIndex = 0;
    }

    public override void draw(SpriteBatch b)
    {
      int x1 = this.xPositionOnScreen + 64 - 12;
      int y1 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
      b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, new Vector2((float) x1, (float) y1), Color.White);
      FarmerRenderer.isDrawingForUI = true;
      Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame((bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 108 : this.playerPanelFrames[this.playerPanelIndex], 0, false, false), (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 108 : this.playerPanelFrames[this.playerPanelIndex], new Rectangle(this.playerPanelFrames[this.playerPanelIndex] * 16, (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 576 : 0, 16, 32), new Vector2((float) (x1 + 32), (float) (y1 + 32)), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, Game1.player);
      if (Game1.timeOfDay >= 1900)
        Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(this.playerPanelFrames[this.playerPanelIndex], 0, false, false), this.playerPanelFrames[this.playerPanelIndex], new Rectangle(this.playerPanelFrames[this.playerPanelIndex] * 16, 0, 16, 32), new Vector2((float) (x1 + 32), (float) (y1 + 32)), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0.0f, 1f, Game1.player);
      FarmerRenderer.isDrawingForUI = false;
      b.DrawString(Game1.smallFont, Game1.player.Name, new Vector2((float) (x1 + 64) - Game1.smallFont.MeasureString(Game1.player.Name).X / 2f, (float) (y1 + 192 + 4)), Game1.textColor);
      b.DrawString(Game1.smallFont, Game1.player.getTitle(), new Vector2((float) (x1 + 64) - Game1.smallFont.MeasureString(Game1.player.getTitle()).X / 2f, (float) (y1 + 256 - 32)), Game1.textColor);
      int num1 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it ? this.xPositionOnScreen + this.width - 448 - 48 : this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 8;
      int num2 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 8;
      int num3 = 0;
      for (int index1 = 0; index1 < 10; ++index1)
      {
        for (int index2 = 0; index2 < 5; ++index2)
        {
          bool flag1 = false;
          bool flag2 = false;
          string text = "";
          int number = 0;
          Rectangle rectangle = Rectangle.Empty;
          switch (index2)
          {
            case 0:
              flag1 = Game1.player.FarmingLevel > index1;
              if (index1 == 0)
                text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604");
              number = Game1.player.FarmingLevel;
              flag2 = (int) (NetFieldBase<int, NetInt>) Game1.player.addedFarmingLevel > 0;
              rectangle = new Rectangle(10, 428, 10, 10);
              break;
            case 1:
              flag1 = Game1.player.MiningLevel > index1;
              if (index1 == 0)
                text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605");
              number = Game1.player.MiningLevel;
              flag2 = (int) (NetFieldBase<int, NetInt>) Game1.player.addedMiningLevel > 0;
              rectangle = new Rectangle(30, 428, 10, 10);
              break;
            case 2:
              flag1 = Game1.player.ForagingLevel > index1;
              if (index1 == 0)
                text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606");
              number = Game1.player.ForagingLevel;
              flag2 = (int) (NetFieldBase<int, NetInt>) Game1.player.addedForagingLevel > 0;
              rectangle = new Rectangle(60, 428, 10, 10);
              break;
            case 3:
              flag1 = Game1.player.FishingLevel > index1;
              if (index1 == 0)
                text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607");
              number = Game1.player.FishingLevel;
              flag2 = (int) (NetFieldBase<int, NetInt>) Game1.player.addedFishingLevel > 0 || Game1.player.CurrentTool != null && Game1.player.CurrentTool.hasEnchantmentOfType<MasterEnchantment>();
              rectangle = new Rectangle(20, 428, 10, 10);
              break;
            case 4:
              flag1 = Game1.player.CombatLevel > index1;
              if (index1 == 0)
                text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608");
              number = Game1.player.CombatLevel;
              flag2 = (int) (NetFieldBase<int, NetInt>) Game1.player.addedCombatLevel > 0;
              rectangle = new Rectangle(120, 428, 10, 10);
              break;
            case 5:
              flag1 = Game1.player.LuckLevel > index1;
              if (index1 == 0)
                text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11609");
              number = Game1.player.LuckLevel;
              flag2 = (int) (NetFieldBase<int, NetInt>) Game1.player.addedLuckLevel > 0;
              rectangle = new Rectangle(50, 428, 10, 10);
              break;
          }
          if (!text.Equals(""))
          {
            b.DrawString(Game1.smallFont, text, new Vector2((float) ((double) num1 - (double) Game1.smallFont.MeasureString(text).X + 4.0 - 64.0), (float) (num2 + 4 + index2 * 56)), Game1.textColor);
            b.Draw(Game1.mouseCursors, new Vector2((float) (num1 - 56), (float) (num2 + index2 * 56)), new Rectangle?(rectangle), Color.Black * 0.3f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.85f);
            b.Draw(Game1.mouseCursors, new Vector2((float) (num1 - 52), (float) (num2 - 4 + index2 * 56)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
          }
          if (!flag1 && (index1 + 1) % 5 == 0)
          {
            b.Draw(Game1.mouseCursors, new Vector2((float) (num3 + num1 - 4 + index1 * 36), (float) (num2 + index2 * 56)), new Rectangle?(new Rectangle(145, 338, 14, 9)), Color.Black * 0.35f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2((float) (num3 + num1 + index1 * 36), (float) (num2 - 4 + index2 * 56)), new Rectangle?(new Rectangle(145 + (flag1 ? 14 : 0), 338, 14, 9)), Color.White * (flag1 ? 1f : 0.65f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
          }
          else if ((index1 + 1) % 5 != 0)
          {
            b.Draw(Game1.mouseCursors, new Vector2((float) (num3 + num1 - 4 + index1 * 36), (float) (num2 + index2 * 56)), new Rectangle?(new Rectangle(129, 338, 8, 9)), Color.Black * 0.35f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.85f);
            b.Draw(Game1.mouseCursors, new Vector2((float) (num3 + num1 + index1 * 36), (float) (num2 - 4 + index2 * 56)), new Rectangle?(new Rectangle(129 + (flag1 ? 8 : 0), 338, 8, 9)), Color.White * (flag1 ? 1f : 0.65f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
          }
          if (index1 == 9)
          {
            NumberSprite.draw(number, b, new Vector2((float) (num3 + num1 + (index1 + 2) * 36 + 12 + (number >= 10 ? 12 : 0)), (float) (num2 + 16 + index2 * 56)), Color.Black * 0.35f, 1f, 0.85f, 1f, 0);
            NumberSprite.draw(number, b, new Vector2((float) (num3 + num1 + (index1 + 2) * 36 + 16 + (number >= 10 ? 12 : 0)), (float) (num2 + 12 + index2 * 56)), (flag2 ? Color.LightGreen : Color.SandyBrown) * (number == 0 ? 0.75f : 1f), 1f, 0.87f, 1f, 0);
          }
        }
        if ((index1 + 1) % 5 == 0)
          num3 += 24;
      }
      foreach (ClickableTextureComponent skillBar in this.skillBars)
        skillBar.draw(b);
      foreach (ClickableTextureComponent skillBar in this.skillBars)
      {
        if ((double) skillBar.scale == 0.0)
        {
          IClickableMenu.drawTextureBox(b, skillBar.bounds.X - 16 - 8, skillBar.bounds.Y - 16 - 16, 96, 96, Color.White);
          b.Draw(Game1.mouseCursors, new Vector2((float) (skillBar.bounds.X - 8), (float) (skillBar.bounds.Y - 32 + 16)), new Rectangle?(new Rectangle(this.professionImage % 6 * 16, 624 + this.professionImage / 6 * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }
      }
      int x2 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32 + 16;
      int y2 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 320 - 8;
      if ((int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.GoldenWalnuts > 0)
      {
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) x2, (float) y2), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
        int x3 = x2 + 48;
        b.DrawString(Game1.smallFont, Game1.netWorldState.Value.GoldenWalnuts?.ToString() ?? "", new Vector2((float) x3, (float) y2), Game1.textColor);
        x2 = x3 + 64;
      }
      if (Game1.player.QiGems > 0)
      {
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) x2, (float) y2), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
        int x4 = x2 + 48;
        b.DrawString(Game1.smallFont, Game1.player.QiGems.ToString() ?? "", new Vector2((float) x4, (float) y2), Game1.textColor);
        int num4 = x4 + 64;
      }
      Game1.drawDialogueBox(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int) ((double) this.height / 2.0) - 32, this.width - 64 - IClickableMenu.spaceToClearSideBorder * 2, this.height / 4 + 64, false, true);
      this.drawBorderLabel(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11610"), Game1.smallFont, this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 96, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int) ((double) this.height / 2.0) - 32);
      foreach (ClickableTextureComponent specialItem in this.specialItems)
        specialItem?.draw(b);
      if (this.hoverText.Length <= 0)
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, boldTitleText: (this.hoverTitle.Length > 0 ? this.hoverTitle : (string) null));
    }
  }
}
