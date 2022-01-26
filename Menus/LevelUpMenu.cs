// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LevelUpMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class LevelUpMenu : IClickableMenu
  {
    public const int region_okButton = 101;
    public const int region_leftProfession = 102;
    public const int region_rightProfession = 103;
    public const int basewidth = 768;
    public const int baseheight = 512;
    public bool informationUp;
    public bool isActive;
    public bool isProfessionChooser;
    public bool hasUpdatedProfessions;
    private int currentLevel;
    private int currentSkill;
    private int timerBeforeStart;
    private float scale;
    private Color leftProfessionColor = Game1.textColor;
    private Color rightProfessionColor = Game1.textColor;
    private MouseState oldMouseState;
    public ClickableTextureComponent starIcon;
    public ClickableTextureComponent okButton;
    public ClickableComponent leftProfession;
    public ClickableComponent rightProfession;
    private List<CraftingRecipe> newCraftingRecipes = new List<CraftingRecipe>();
    private List<string> extraInfoForLevel = new List<string>();
    private List<string> leftProfessionDescription = new List<string>();
    private List<string> rightProfessionDescription = new List<string>();
    private Rectangle sourceRectForLevelIcon;
    private string title;
    private List<int> professionsToChoose = new List<int>();
    private List<TemporaryAnimatedSprite> littleStars = new List<TemporaryAnimatedSprite>();
    public bool hasMovedSelection;

    public LevelUpMenu()
      : base(Game1.uiViewport.Width / 2 - 384, Game1.uiViewport.Height / 2 - 256, 768, 512)
    {
      Game1.player.team.endOfNightStatus.UpdateState("level");
      this.width = 768;
      this.height = 512;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 64 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent.myID = 101;
      this.okButton = textureComponent;
      this.RepositionOkButton();
    }

    public LevelUpMenu(int skill, int level)
      : base(Game1.uiViewport.Width / 2 - 384, Game1.uiViewport.Height / 2 - 256, 768, 512)
    {
      Game1.player.team.endOfNightStatus.UpdateState(nameof (level));
      this.timerBeforeStart = 250;
      this.isActive = true;
      this.width = 960;
      this.height = 512;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 64 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent.myID = 101;
      this.okButton = textureComponent;
      this.newCraftingRecipes.Clear();
      this.extraInfoForLevel.Clear();
      Game1.player.completelyStopAnimatingOrDoingAction();
      this.informationUp = true;
      this.isProfessionChooser = false;
      this.currentLevel = level;
      this.currentSkill = skill;
      if (level == 10)
      {
        Game1.getSteamAchievement("Achievement_SingularTalent");
        if ((int) (NetFieldBase<int, NetInt>) Game1.player.farmingLevel == 10 && (int) (NetFieldBase<int, NetInt>) Game1.player.miningLevel == 10 && (int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel == 10 && (int) (NetFieldBase<int, NetInt>) Game1.player.foragingLevel == 10 && (int) (NetFieldBase<int, NetInt>) Game1.player.combatLevel == 10)
          Game1.getSteamAchievement("Achievement_MasterOfTheFiveWays");
        if (skill == 0)
          Game1.addMailForTomorrow("marnieAutoGrabber");
      }
      this.title = Game1.content.LoadString("Strings\\UI:LevelUp_Title", (object) this.currentLevel, (object) Farmer.getSkillDisplayNameFromIndex(this.currentSkill));
      this.extraInfoForLevel = this.getExtraInfoForLevel(this.currentSkill, this.currentLevel);
      switch (this.currentSkill)
      {
        case 0:
          this.sourceRectForLevelIcon = new Rectangle(0, 0, 16, 16);
          break;
        case 1:
          this.sourceRectForLevelIcon = new Rectangle(16, 0, 16, 16);
          break;
        case 2:
          this.sourceRectForLevelIcon = new Rectangle(80, 0, 16, 16);
          break;
        case 3:
          this.sourceRectForLevelIcon = new Rectangle(32, 0, 16, 16);
          break;
        case 4:
          this.sourceRectForLevelIcon = new Rectangle(128, 16, 16, 16);
          break;
        case 5:
          this.sourceRectForLevelIcon = new Rectangle(64, 0, 16, 16);
          break;
      }
      if ((this.currentLevel == 5 || this.currentLevel == 10) && this.currentSkill != 5)
      {
        this.professionsToChoose.Clear();
        this.isProfessionChooser = true;
      }
      int num = 0;
      foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
      {
        string str = craftingRecipe.Value.Split('/')[4];
        if (str.Contains(Farmer.getSkillNameFromIndex(this.currentSkill)) && str.Contains(this.currentLevel.ToString() ?? ""))
        {
          this.newCraftingRecipes.Add(new CraftingRecipe(craftingRecipe.Key, false));
          if (!Game1.player.craftingRecipes.ContainsKey(craftingRecipe.Key))
            Game1.player.craftingRecipes.Add(craftingRecipe.Key, 0);
          num += this.newCraftingRecipes.Last<CraftingRecipe>().bigCraftable ? 128 : 64;
        }
      }
      foreach (KeyValuePair<string, string> cookingRecipe in CraftingRecipe.cookingRecipes)
      {
        string str = cookingRecipe.Value.Split('/')[3];
        if (str.Contains(Farmer.getSkillNameFromIndex(this.currentSkill)) && str.Contains(this.currentLevel.ToString() ?? ""))
        {
          this.newCraftingRecipes.Add(new CraftingRecipe(cookingRecipe.Key, true));
          if (!Game1.player.cookingRecipes.ContainsKey(cookingRecipe.Key))
          {
            Game1.player.cookingRecipes.Add(cookingRecipe.Key, 0);
            if (!Game1.player.hasOrWillReceiveMail("robinKitchenLetter"))
              Game1.mailbox.Add("robinKitchenLetter");
          }
          num += this.newCraftingRecipes.Last<CraftingRecipe>().bigCraftable ? 128 : 64;
        }
      }
      this.height = num + 256 + this.extraInfoForLevel.Count * 64 * 3 / 4;
      Game1.player.freezePause = 100;
      this.gameWindowSizeChanged(Rectangle.Empty, Rectangle.Empty);
      if (this.isProfessionChooser)
      {
        this.leftProfession = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 2, this.height), "")
        {
          myID = 102,
          rightNeighborID = 103
        };
        this.rightProfession = new ClickableComponent(new Rectangle(this.width / 2 + this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 2, this.height), "")
        {
          myID = 103,
          leftNeighborID = 102
        };
      }
      this.populateClickableComponentList();
    }

    public bool CanReceiveInput() => this.informationUp && this.timerBeforeStart <= 0;

    public override void snapToDefaultClickableComponent()
    {
      if (this.isProfessionChooser)
      {
        this.currentlySnappedComponent = this.getComponentWithID(103);
        Game1.setMousePosition(this.xPositionOnScreen + this.width + 64, this.yPositionOnScreen + this.height + 64);
      }
      else
      {
        this.currentlySnappedComponent = this.getComponentWithID(101);
        this.snapCursorToCurrentSnappedComponent();
      }
    }

    public override void applyMovementKey(int direction)
    {
      if (!this.CanReceiveInput())
        return;
      if (direction == 3 || direction == 1)
        this.hasMovedSelection = true;
      base.applyMovementKey(direction);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2;
      this.RepositionOkButton();
    }

    public virtual void RepositionOkButton()
    {
      this.okButton.bounds = new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 64 - IClickableMenu.borderWidth, 64, 64);
      if (this.okButton.bounds.Right > Game1.uiViewport.Width)
        this.okButton.bounds.X = Game1.uiViewport.Width - 64;
      if (this.okButton.bounds.Bottom <= Game1.uiViewport.Height)
        return;
      this.okButton.bounds.Y = Game1.uiViewport.Height - 64;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public List<string> getExtraInfoForLevel(int whichSkill, int whichLevel)
    {
      List<string> extraInfoForLevel = new List<string>();
      switch (whichSkill)
      {
        case 0:
          extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Farming1"));
          extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Farming2"));
          break;
        case 1:
          extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Fishing"));
          break;
        case 2:
          extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Foraging1"));
          if (whichLevel == 1)
            extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Foraging2"));
          if (whichLevel == 4 || whichLevel == 8)
          {
            extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Foraging3"));
            break;
          }
          break;
        case 3:
          extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Mining"));
          break;
        case 4:
          extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Combat"));
          break;
        case 5:
          extraInfoForLevel.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ExtraInfo_Luck"));
          break;
      }
      return extraInfoForLevel;
    }

    private static void addProfessionDescriptions(List<string> descriptions, string professionName)
    {
      descriptions.Add(Game1.content.LoadString("Strings\\UI:LevelUp_ProfessionName_" + professionName));
      descriptions.AddRange((IEnumerable<string>) Game1.content.LoadString("Strings\\UI:LevelUp_ProfessionDescription_" + professionName).Split('\n'));
    }

    private static string getProfessionName(int whichProfession)
    {
      switch (whichProfession)
      {
        case 0:
          return "Rancher";
        case 1:
          return "Tiller";
        case 2:
          return "Coopmaster";
        case 3:
          return "Shepherd";
        case 4:
          return "Artisan";
        case 5:
          return "Agriculturist";
        case 6:
          return "Fisher";
        case 7:
          return "Trapper";
        case 8:
          return "Angler";
        case 9:
          return "Pirate";
        case 10:
          return "Mariner";
        case 11:
          return "Luremaster";
        case 12:
          return "Forester";
        case 13:
          return "Gatherer";
        case 14:
          return "Lumberjack";
        case 15:
          return "Tapper";
        case 16:
          return "Botanist";
        case 17:
          return "Tracker";
        case 18:
          return "Miner";
        case 19:
          return "Geologist";
        case 20:
          return "Blacksmith";
        case 21:
          return "Prospector";
        case 22:
          return "Excavator";
        case 23:
          return "Gemologist";
        case 24:
          return "Fighter";
        case 25:
          return "Scout";
        case 26:
          return "Brute";
        case 27:
          return "Defender";
        case 28:
          return "Acrobat";
        default:
          return "Desperado";
      }
    }

    public static List<string> getProfessionDescription(int whichProfession)
    {
      List<string> descriptions = new List<string>();
      LevelUpMenu.addProfessionDescriptions(descriptions, LevelUpMenu.getProfessionName(whichProfession));
      return descriptions;
    }

    public static string getProfessionTitleFromNumber(int whichProfession) => Game1.content.LoadString("Strings\\UI:LevelUp_ProfessionName_" + LevelUpMenu.getProfessionName(whichProfession));

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (b != Buttons.Start && b != Buttons.B || this.isProfessionChooser || !this.isActive)
        return;
      this.okButtonClicked();
    }

    public static void AddMissedProfessionChoices(Farmer farmer)
    {
      int[] numArray = new int[5]{ 0, 1, 2, 3, 4 };
      foreach (int num in numArray)
      {
        if (farmer.GetUnmodifiedSkillLevel(num) >= 5 && !farmer.newLevels.Contains(new Point(num, 5)) && farmer.getProfessionForSkill(num, 5) == -1)
          farmer.newLevels.Add(new Point(num, 5));
        if (farmer.GetUnmodifiedSkillLevel(num) >= 10 && !farmer.newLevels.Contains(new Point(num, 10)) && farmer.getProfessionForSkill(num, 10) == -1)
          farmer.newLevels.Add(new Point(num, 10));
      }
    }

    public static void AddMissedLevelRecipes(Farmer farmer)
    {
      int[] numArray = new int[5]{ 0, 1, 2, 3, 4 };
      foreach (int num in numArray)
      {
        for (int y = 0; y <= farmer.GetUnmodifiedSkillLevel(num); ++y)
        {
          if (!farmer.newLevels.Contains(new Point(num, y)))
          {
            foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
            {
              string str = craftingRecipe.Value.Split('/')[4];
              if (str.Contains(Farmer.getSkillNameFromIndex(num)) && str.Contains(y.ToString() ?? "") && !farmer.craftingRecipes.ContainsKey(craftingRecipe.Key))
              {
                Console.WriteLine(farmer.Name + " was missing recipe " + craftingRecipe.Key + " from skill level up.");
                farmer.craftingRecipes.Add(craftingRecipe.Key, 0);
              }
            }
            foreach (KeyValuePair<string, string> cookingRecipe in CraftingRecipe.cookingRecipes)
            {
              string str = cookingRecipe.Value.Split('/')[3];
              if (str.Contains(Farmer.getSkillNameFromIndex(num)) && str.Contains(y.ToString() ?? "") && !farmer.cookingRecipes.ContainsKey(cookingRecipe.Key))
              {
                Console.WriteLine(farmer.Name + " was missing recipe " + cookingRecipe.Key + " from skill level up.");
                farmer.cookingRecipes.Add(cookingRecipe.Key, 0);
              }
            }
          }
        }
      }
    }

    public static void removeImmediateProfessionPerk(int whichProfession)
    {
      switch (whichProfession)
      {
        case 24:
          Game1.player.maxHealth -= 15;
          break;
        case 27:
          Game1.player.maxHealth -= 25;
          break;
      }
      if (Game1.player.health <= Game1.player.maxHealth)
        return;
      Game1.player.health = Game1.player.maxHealth;
    }

    public void getImmediateProfessionPerk(int whichProfession)
    {
      switch (whichProfession)
      {
        case 24:
          Game1.player.maxHealth += 15;
          break;
        case 27:
          Game1.player.maxHealth += 25;
          break;
      }
      Game1.player.health = Game1.player.maxHealth;
      Game1.player.Stamina = (float) (int) (NetFieldBase<int, NetInt>) Game1.player.maxStamina;
    }

    public static void RevalidateHealth(Farmer farmer)
    {
      int maxHealth = farmer.maxHealth;
      int num1 = 100;
      if (farmer.mailReceived.Contains("qiCave"))
        num1 += 25;
      for (int y = 1; y <= farmer.GetUnmodifiedSkillLevel(4); ++y)
      {
        if (!farmer.newLevels.Contains(new Point(4, y)) && y != 5 && y != 10)
          num1 += 5;
      }
      if (farmer.professions.Contains(24))
        num1 += 15;
      if (farmer.professions.Contains(27))
        num1 += 25;
      if (farmer.maxHealth >= num1)
        return;
      Console.WriteLine("Fixing max health of: " + farmer.Name + " was " + farmer.maxHealth.ToString() + " (expected: " + num1.ToString() + ")");
      int num2 = num1 - farmer.maxHealth;
      farmer.maxHealth = num1;
      farmer.health += num2;
    }

    public override void update(GameTime time)
    {
      if (!this.isActive)
      {
        this.exitThisMenu();
      }
      else
      {
        if (this.isProfessionChooser && !this.hasUpdatedProfessions)
        {
          if (this.currentLevel == 5)
          {
            this.professionsToChoose.Add(this.currentSkill * 6);
            this.professionsToChoose.Add(this.currentSkill * 6 + 1);
          }
          else if (Game1.player.professions.Contains(this.currentSkill * 6))
          {
            this.professionsToChoose.Add(this.currentSkill * 6 + 2);
            this.professionsToChoose.Add(this.currentSkill * 6 + 3);
          }
          else
          {
            this.professionsToChoose.Add(this.currentSkill * 6 + 4);
            this.professionsToChoose.Add(this.currentSkill * 6 + 5);
          }
          this.leftProfessionDescription = LevelUpMenu.getProfessionDescription(this.professionsToChoose[0]);
          this.rightProfessionDescription = LevelUpMenu.getProfessionDescription(this.professionsToChoose[1]);
          this.hasUpdatedProfessions = true;
        }
        for (int index = this.littleStars.Count - 1; index >= 0; --index)
        {
          if (this.littleStars[index].update(time))
            this.littleStars.RemoveAt(index);
        }
        if (Game1.random.NextDouble() < 0.03)
        {
          Vector2 position = new Vector2(0.0f, (float) (Game1.random.Next(this.yPositionOnScreen - 128, this.yPositionOnScreen - 4) / 20 * 4 * 5 + 32));
          position.X = Game1.random.NextDouble() >= 0.5 ? (float) Game1.random.Next(this.xPositionOnScreen + this.width / 2 + 116, this.xPositionOnScreen + this.width - 160) : (float) Game1.random.Next(this.xPositionOnScreen + this.width / 2 - 228, this.xPositionOnScreen + this.width / 2 - 132);
          if ((double) position.Y < (double) (this.yPositionOnScreen - 64 - 8))
            position.X = (float) Game1.random.Next(this.xPositionOnScreen + this.width / 2 - 116, this.xPositionOnScreen + this.width / 2 + 116);
          position.X = (float) ((double) position.X / 20.0 * 4.0 * 5.0);
          this.littleStars.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(364, 79, 5, 5), 80f, 7, 1, position, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            local = true
          });
        }
        if (this.timerBeforeStart > 0)
        {
          this.timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
          if (this.timerBeforeStart > 0 || !Game1.options.SnappyMenus)
            return;
          this.populateClickableComponentList();
          this.snapToDefaultClickableComponent();
        }
        else
        {
          if (this.isActive && this.isProfessionChooser)
          {
            this.leftProfessionColor = Game1.textColor;
            this.rightProfessionColor = Game1.textColor;
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.player.freezePause = 100;
            if (Game1.getMouseY() > this.yPositionOnScreen + 192 && Game1.getMouseY() < this.yPositionOnScreen + this.height)
            {
              if (Game1.getMouseX() > this.xPositionOnScreen && Game1.getMouseX() < this.xPositionOnScreen + this.width / 2)
              {
                this.leftProfessionColor = Color.Green;
                if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && this.oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)) && this.readyToClose())
                {
                  Game1.player.professions.Add(this.professionsToChoose[0]);
                  this.getImmediateProfessionPerk(this.professionsToChoose[0]);
                  this.isActive = false;
                  this.informationUp = false;
                  this.isProfessionChooser = false;
                  this.RemoveLevelFromLevelList();
                }
              }
              else if (Game1.getMouseX() > this.xPositionOnScreen + this.width / 2 && Game1.getMouseX() < this.xPositionOnScreen + this.width)
              {
                this.rightProfessionColor = Color.Green;
                if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && this.oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)) && this.readyToClose())
                {
                  Game1.player.professions.Add(this.professionsToChoose[1]);
                  this.getImmediateProfessionPerk(this.professionsToChoose[1]);
                  this.isActive = false;
                  this.informationUp = false;
                  this.isProfessionChooser = false;
                  this.RemoveLevelFromLevelList();
                }
              }
            }
            this.height = 512;
          }
          this.oldMouseState = Game1.input.GetMouseState();
          if (this.isActive && !this.informationUp && this.starIcon != null)
            this.starIcon.sourceRect.X = !this.starIcon.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 310 : 294;
          if (this.isActive && this.starIcon != null && !this.informationUp && (this.oldMouseState.LeftButton == ButtonState.Pressed || Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A)) && this.starIcon.containsPoint(this.oldMouseState.X, this.oldMouseState.Y))
          {
            this.newCraftingRecipes.Clear();
            this.extraInfoForLevel.Clear();
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.playSound("bigSelect");
            this.informationUp = true;
            this.isProfessionChooser = false;
            this.currentLevel = Game1.player.newLevels.First<Point>().Y;
            this.currentSkill = Game1.player.newLevels.First<Point>().X;
            this.title = Game1.content.LoadString("Strings\\UI:LevelUp_Title", (object) this.currentLevel, (object) Farmer.getSkillDisplayNameFromIndex(this.currentSkill));
            this.extraInfoForLevel = this.getExtraInfoForLevel(this.currentSkill, this.currentLevel);
            switch (this.currentSkill)
            {
              case 0:
                this.sourceRectForLevelIcon = new Rectangle(0, 0, 16, 16);
                break;
              case 1:
                this.sourceRectForLevelIcon = new Rectangle(16, 0, 16, 16);
                break;
              case 2:
                this.sourceRectForLevelIcon = new Rectangle(80, 0, 16, 16);
                break;
              case 3:
                this.sourceRectForLevelIcon = new Rectangle(32, 0, 16, 16);
                break;
              case 4:
                this.sourceRectForLevelIcon = new Rectangle(128, 16, 16, 16);
                break;
              case 5:
                this.sourceRectForLevelIcon = new Rectangle(64, 0, 16, 16);
                break;
            }
            if ((this.currentLevel == 5 || this.currentLevel == 10) && this.currentSkill != 5)
            {
              this.professionsToChoose.Clear();
              this.isProfessionChooser = true;
              if (this.currentLevel == 5)
              {
                this.professionsToChoose.Add(this.currentSkill * 6);
                this.professionsToChoose.Add(this.currentSkill * 6 + 1);
              }
              else if (Game1.player.professions.Contains(this.currentSkill * 6))
              {
                this.professionsToChoose.Add(this.currentSkill * 6 + 2);
                this.professionsToChoose.Add(this.currentSkill * 6 + 3);
              }
              else
              {
                this.professionsToChoose.Add(this.currentSkill * 6 + 4);
                this.professionsToChoose.Add(this.currentSkill * 6 + 5);
              }
              this.leftProfessionDescription = LevelUpMenu.getProfessionDescription(this.professionsToChoose[0]);
              this.rightProfessionDescription = LevelUpMenu.getProfessionDescription(this.professionsToChoose[1]);
            }
            int num = 0;
            foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
            {
              string str = craftingRecipe.Value.Split('/')[4];
              if (str.Contains(Farmer.getSkillNameFromIndex(this.currentSkill)) && str.Contains(this.currentLevel.ToString() ?? ""))
              {
                this.newCraftingRecipes.Add(new CraftingRecipe(craftingRecipe.Key, false));
                if (!Game1.player.craftingRecipes.ContainsKey(craftingRecipe.Key))
                  Game1.player.craftingRecipes.Add(craftingRecipe.Key, 0);
                num += this.newCraftingRecipes.Last<CraftingRecipe>().bigCraftable ? 128 : 64;
              }
            }
            foreach (KeyValuePair<string, string> cookingRecipe in CraftingRecipe.cookingRecipes)
            {
              string str = cookingRecipe.Value.Split('/')[3];
              if (str.Contains(Farmer.getSkillNameFromIndex(this.currentSkill)) && str.Contains(this.currentLevel.ToString() ?? ""))
              {
                this.newCraftingRecipes.Add(new CraftingRecipe(cookingRecipe.Key, true));
                if (!Game1.player.cookingRecipes.ContainsKey(cookingRecipe.Key))
                  Game1.player.cookingRecipes.Add(cookingRecipe.Key, 0);
                num += this.newCraftingRecipes.Last<CraftingRecipe>().bigCraftable ? 128 : 64;
              }
            }
            this.height = num + 256 + this.extraInfoForLevel.Count * 64 * 3 / 4;
            Game1.player.freezePause = 100;
          }
          if (!this.isActive || !this.informationUp)
            return;
          Game1.player.completelyStopAnimatingOrDoingAction();
          if (this.okButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !this.isProfessionChooser)
          {
            this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
            if ((this.oldMouseState.LeftButton == ButtonState.Pressed || Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A)) && this.readyToClose())
              this.okButtonClicked();
          }
          else
            this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
          Game1.player.freezePause = 100;
        }
      }
    }

    protected override void cleanupBeforeExit()
    {
      if (!this.isActive)
        return;
      this.okButtonClicked();
    }

    public void okButtonClicked()
    {
      this.getLevelPerk(this.currentSkill, this.currentLevel);
      this.RemoveLevelFromLevelList();
      this.isActive = false;
      this.informationUp = false;
    }

    public virtual void RemoveLevelFromLevelList()
    {
      for (int index = 0; index < Game1.player.newLevels.Count; ++index)
      {
        Point newLevel = Game1.player.newLevels[index];
        if (newLevel.X == this.currentSkill && newLevel.Y == this.currentLevel)
        {
          Game1.player.newLevels.RemoveAt(index);
          --index;
        }
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      if ((Game1.options.doesInputListContain(Game1.options.cancelButton, key) || Game1.options.doesInputListContain(Game1.options.menuButton, key)) && this.isProfessionChooser)
        return;
      base.receiveKeyPress(key);
    }

    public void getLevelPerk(int skill, int level)
    {
      switch (skill)
      {
        case 1:
          switch (level)
          {
            case 2:
              if (!Game1.player.hasOrWillReceiveMail("fishing2"))
              {
                Game1.addMailForTomorrow("fishing2");
                break;
              }
              break;
            case 6:
              if (!Game1.player.hasOrWillReceiveMail("fishing6"))
              {
                Game1.addMailForTomorrow("fishing6");
                break;
              }
              break;
          }
          break;
        case 4:
          Game1.player.maxHealth += 5;
          break;
      }
      Game1.player.health = Game1.player.maxHealth;
      Game1.player.Stamina = (float) (int) (NetFieldBase<int, NetInt>) Game1.player.maxStamina;
    }

    public override void draw(SpriteBatch b)
    {
      if (this.timerBeforeStart > 0)
        return;
      b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
      foreach (TemporaryAnimatedSprite littleStar in this.littleStars)
        littleStar.draw(b);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + this.width / 2 - 116), (float) (this.yPositionOnScreen - 32 + 12)), new Rectangle?(new Rectangle(363, 87, 58, 22)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      if (!this.informationUp && this.isActive && this.starIcon != null)
      {
        this.starIcon.draw(b);
      }
      else
      {
        if (!this.informationUp)
          return;
        if (this.isProfessionChooser)
        {
          if (this.professionsToChoose.Count<int>() == 0)
            return;
          Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
          this.drawHorizontalPartition(b, this.yPositionOnScreen + 192);
          this.drawVerticalIntersectingPartition(b, this.xPositionOnScreen + this.width / 2 - 32, this.yPositionOnScreen + 192);
          Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), this.sourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f);
          b.DrawString(Game1.dialogueFont, this.title, new Vector2((float) (this.xPositionOnScreen + this.width / 2) - Game1.dialogueFont.MeasureString(this.title).X / 2f, (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), Game1.textColor);
          Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2((float) (this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 64), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), this.sourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f);
          string text = Game1.content.LoadString("Strings\\UI:LevelUp_ChooseProfession");
          b.DrawString(Game1.smallFont, text, new Vector2((float) (this.xPositionOnScreen + this.width / 2) - Game1.smallFont.MeasureString(text).X / 2f, (float) (this.yPositionOnScreen + 64 + IClickableMenu.spaceToClearTopBorder)), Game1.textColor);
          b.DrawString(Game1.dialogueFont, this.leftProfessionDescription[0], new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160)), this.leftProfessionColor);
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 2 - 112), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16)), new Rectangle?(new Rectangle(this.professionsToChoose[0] % 6 * 16, 624 + this.professionsToChoose[0] / 6 * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          for (int index = 1; index < this.leftProfessionDescription.Count; ++index)
            b.DrawString(Game1.smallFont, Game1.parseText(this.leftProfessionDescription[index], Game1.smallFont, this.width / 2 - 64), new Vector2((float) (this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + 32), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1))), this.leftProfessionColor);
          b.DrawString(Game1.dialogueFont, this.rightProfessionDescription[0], new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 2), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160)), this.rightProfessionColor);
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width - 128), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16)), new Rectangle?(new Rectangle(this.professionsToChoose[1] % 6 * 16, 624 + this.professionsToChoose[1] / 6 * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          for (int index = 1; index < this.rightProfessionDescription.Count; ++index)
            b.DrawString(Game1.smallFont, Game1.parseText(this.rightProfessionDescription[index], Game1.smallFont, this.width / 2 - 48), new Vector2((float) (this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + this.width / 2), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1))), this.rightProfessionColor);
        }
        else
        {
          Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
          Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), this.sourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f);
          b.DrawString(Game1.dialogueFont, this.title, new Vector2((float) (this.xPositionOnScreen + this.width / 2) - Game1.dialogueFont.MeasureString(this.title).X / 2f, (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), Game1.textColor);
          Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2((float) (this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 64), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), this.sourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f);
          int y = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 80;
          foreach (string text in this.extraInfoForLevel)
          {
            b.DrawString(Game1.smallFont, text, new Vector2((float) (this.xPositionOnScreen + this.width / 2) - Game1.smallFont.MeasureString(text).X / 2f, (float) y), Game1.textColor);
            y += 48;
          }
          foreach (CraftingRecipe newCraftingRecipe in this.newCraftingRecipes)
          {
            string sub1 = Game1.content.LoadString("Strings\\UI:LearnedRecipe_" + (newCraftingRecipe.isCookingRecipe ? "cooking" : "crafting"));
            string text = Game1.content.LoadString("Strings\\UI:LevelUp_NewRecipe", (object) sub1, (object) newCraftingRecipe.DisplayName);
            b.DrawString(Game1.smallFont, text, new Vector2((float) ((double) (this.xPositionOnScreen + this.width / 2) - (double) Game1.smallFont.MeasureString(text).X / 2.0 - 64.0), (float) (y + (newCraftingRecipe.bigCraftable ? 38 : 12))), Game1.textColor);
            newCraftingRecipe.drawMenuView(b, (int) ((double) (this.xPositionOnScreen + this.width / 2) + (double) Game1.smallFont.MeasureString(text).X / 2.0 - 48.0), y - 16);
            y += (newCraftingRecipe.bigCraftable ? 128 : 64) + 8;
          }
          this.okButton.draw(b);
        }
        if (Game1.options.SnappyMenus && this.isProfessionChooser && !this.hasMovedSelection)
          return;
        Game1.mouseCursorTransparency = 1f;
        this.drawMouse(b);
      }
    }
  }
}
