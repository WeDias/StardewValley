// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.CollectionsPage
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
  public class CollectionsPage : IClickableMenu
  {
    public const int region_sideTabShipped = 7001;
    public const int region_sideTabFish = 7002;
    public const int region_sideTabArtifacts = 7003;
    public const int region_sideTabMinerals = 7004;
    public const int region_sideTabCooking = 7005;
    public const int region_sideTabAchivements = 7006;
    public const int region_sideTabSecretNotes = 7007;
    public const int region_sideTabLetters = 7008;
    public const int region_forwardButton = 707;
    public const int region_backButton = 706;
    public static int widthToMoveActiveTab = 8;
    public const int organicsTab = 0;
    public const int fishTab = 1;
    public const int archaeologyTab = 2;
    public const int mineralsTab = 3;
    public const int cookingTab = 4;
    public const int achievementsTab = 5;
    public const int secretNotesTab = 6;
    public const int lettersTab = 7;
    public const int distanceFromMenuBottomBeforeNewPage = 128;
    private string descriptionText = "";
    private string hoverText = "";
    public ClickableTextureComponent backButton;
    public ClickableTextureComponent forwardButton;
    public Dictionary<int, ClickableTextureComponent> sideTabs = new Dictionary<int, ClickableTextureComponent>();
    public int currentTab;
    public int currentPage;
    public int secretNoteImage = -1;
    public Dictionary<int, List<List<ClickableTextureComponent>>> collections = new Dictionary<int, List<List<ClickableTextureComponent>>>();
    public Dictionary<int, string> secretNotesData;
    public Texture2D secretNoteImageTexture;
    public LetterViewerMenu letterviewerSubMenu;
    private Item hoverItem;
    private CraftingRecipe hoverCraftingRecipe;
    private int value;

    public CollectionsPage(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
      Dictionary<int, ClickableTextureComponent> sideTabs1 = this.sideTabs;
      int num1 = 0;
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 + CollectionsPage.widthToMoveActiveTab, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Shipped"), Game1.mouseCursors, new Rectangle(640, 80, 16, 16), 4f);
      textureComponent1.myID = 7001;
      textureComponent1.downNeighborID = -99998;
      textureComponent1.rightNeighborID = 0;
      sideTabs1.Add(0, textureComponent1);
      this.collections.Add(0, new List<List<ClickableTextureComponent>>());
      Dictionary<int, ClickableTextureComponent> sideTabs2 = this.sideTabs;
      num1 = 1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Fish"), Game1.mouseCursors, new Rectangle(640, 64, 16, 16), 4f);
      textureComponent2.myID = 7002;
      textureComponent2.upNeighborID = -99998;
      textureComponent2.downNeighborID = -99998;
      textureComponent2.rightNeighborID = 0;
      sideTabs2.Add(1, textureComponent2);
      this.collections.Add(1, new List<List<ClickableTextureComponent>>());
      Dictionary<int, ClickableTextureComponent> sideTabs3 = this.sideTabs;
      num1 = 2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Artifacts"), Game1.mouseCursors, new Rectangle(656, 64, 16, 16), 4f);
      textureComponent3.myID = 7003;
      textureComponent3.upNeighborID = -99998;
      textureComponent3.downNeighborID = -99998;
      textureComponent3.rightNeighborID = 0;
      sideTabs3.Add(2, textureComponent3);
      this.collections.Add(2, new List<List<ClickableTextureComponent>>());
      Dictionary<int, ClickableTextureComponent> sideTabs4 = this.sideTabs;
      num1 = 3;
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Minerals"), Game1.mouseCursors, new Rectangle(672, 64, 16, 16), 4f);
      textureComponent4.myID = 7004;
      textureComponent4.upNeighborID = -99998;
      textureComponent4.downNeighborID = -99998;
      textureComponent4.rightNeighborID = 0;
      sideTabs4.Add(3, textureComponent4);
      this.collections.Add(3, new List<List<ClickableTextureComponent>>());
      Dictionary<int, ClickableTextureComponent> sideTabs5 = this.sideTabs;
      num1 = 4;
      ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Cooking"), Game1.mouseCursors, new Rectangle(688, 64, 16, 16), 4f);
      textureComponent5.myID = 7005;
      textureComponent5.upNeighborID = -99998;
      textureComponent5.downNeighborID = -99998;
      textureComponent5.rightNeighborID = 0;
      sideTabs5.Add(4, textureComponent5);
      this.collections.Add(4, new List<List<ClickableTextureComponent>>());
      Dictionary<int, ClickableTextureComponent> sideTabs6 = this.sideTabs;
      num1 = 5;
      ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Achievements"), Game1.mouseCursors, new Rectangle(656, 80, 16, 16), 4f);
      textureComponent6.myID = 7006;
      textureComponent6.upNeighborID = 7005;
      textureComponent6.downNeighborID = -99998;
      textureComponent6.rightNeighborID = 0;
      sideTabs6.Add(5, textureComponent6);
      this.collections.Add(5, new List<List<ClickableTextureComponent>>());
      Dictionary<int, ClickableTextureComponent> sideTabs7 = this.sideTabs;
      num1 = 7;
      ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Letters"), Game1.mouseCursors, new Rectangle(688, 80, 16, 16), 4f);
      textureComponent7.myID = 7008;
      textureComponent7.upNeighborID = -99998;
      textureComponent7.downNeighborID = -99998;
      textureComponent7.rightNeighborID = 0;
      sideTabs7.Add(7, textureComponent7);
      this.collections.Add(7, new List<List<ClickableTextureComponent>>());
      if (Game1.player.secretNotesSeen.Count > 0)
      {
        Dictionary<int, ClickableTextureComponent> sideTabs8 = this.sideTabs;
        num1 = 6;
        ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 64 * (2 + this.sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_SecretNotes"), Game1.mouseCursors, new Rectangle(672, 80, 16, 16), 4f);
        textureComponent8.myID = 7007;
        textureComponent8.upNeighborID = -99998;
        textureComponent8.rightNeighborID = 0;
        sideTabs8.Add(6, textureComponent8);
        this.collections.Add(6, new List<List<ClickableTextureComponent>>());
      }
      this.sideTabs[0].upNeighborID = -1;
      this.sideTabs[0].upNeighborImmutable = true;
      int key1 = 0;
      int num2 = 0;
      foreach (int key2 in this.sideTabs.Keys)
      {
        if (this.sideTabs[key2].bounds.Y > num2)
        {
          num2 = this.sideTabs[key2].bounds.Y;
          key1 = key2;
        }
      }
      this.sideTabs[key1].downNeighborID = -1;
      this.sideTabs[key1].downNeighborImmutable = true;
      CollectionsPage.widthToMoveActiveTab = 8;
      ClickableTextureComponent textureComponent9 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 48, this.yPositionOnScreen + height - 80, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent9.myID = 706;
      textureComponent9.rightNeighborID = -7777;
      this.backButton = textureComponent9;
      ClickableTextureComponent textureComponent10 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width - 32 - 60, this.yPositionOnScreen + height - 80, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent10.myID = 707;
      textureComponent10.leftNeighborID = -7777;
      this.forwardButton = textureComponent10;
      int[] numArray = new int[8];
      int num3 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
      int num4 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;
      int num5 = 10;
      List<KeyValuePair<int, string>> keyValuePairList = new List<KeyValuePair<int, string>>((IEnumerable<KeyValuePair<int, string>>) Game1.objectInformation);
      keyValuePairList.Sort((Comparison<KeyValuePair<int, string>>) ((a, b) => a.Key.CompareTo(b.Key)));
      foreach (KeyValuePair<int, string> keyValuePair in keyValuePairList)
      {
        string str = keyValuePair.Value.Split('/')[3];
        bool drawShadow = false;
        bool flag = false;
        int key3;
        if (str.Contains("Arch"))
        {
          key3 = 2;
          if (Game1.player.archaeologyFound.ContainsKey(keyValuePair.Key))
            drawShadow = true;
        }
        else if (str.Contains("Fish"))
        {
          if ((keyValuePair.Key < 167 || keyValuePair.Key > 172) && (keyValuePair.Key < 898 || keyValuePair.Key > 902))
          {
            key3 = 1;
            if (Game1.player.fishCaught.ContainsKey(keyValuePair.Key))
              drawShadow = true;
          }
          else
            continue;
        }
        else if (str.Contains("Mineral") || str.Substring(str.Length - 3).Equals("-2"))
        {
          key3 = 3;
          if (Game1.player.mineralsFound.ContainsKey(keyValuePair.Key))
            drawShadow = true;
        }
        else if (str.Contains("Cooking") || str.Substring(str.Length - 3).Equals("-7"))
        {
          key3 = 4;
          string key4 = keyValuePair.Value.Split('/')[0];
          switch (key4)
          {
            case "Cheese Cauli.":
              key4 = "Cheese Cauliflower";
              break;
            case "Cheese Cauliflower":
              key4 = "Cheese Cauli.";
              break;
            case "Cookie":
              key4 = "Cookies";
              break;
            case "Cranberry Sauce":
              key4 = "Cran. Sauce";
              break;
            case "Dish O' The Sea":
              key4 = "Dish o' The Sea";
              break;
            case "Eggplant Parmesan":
              key4 = "Eggplant Parm.";
              break;
            case "Vegetable Medley":
              key4 = "Vegetable Stew";
              break;
          }
          if (Game1.player.recipesCooked.ContainsKey(keyValuePair.Key))
            drawShadow = true;
          else if (Game1.player.cookingRecipes.ContainsKey(key4))
            flag = true;
          if (keyValuePair.Key == 217 || keyValuePair.Key == 772 || keyValuePair.Key == 773 || keyValuePair.Key == 279 || keyValuePair.Key == 873)
            continue;
        }
        else if (StardewValley.Object.isPotentialBasicShippedCategory(keyValuePair.Key, str.Substring(str.Length - 3)))
        {
          key3 = 0;
          if (Game1.player.basicShipped.ContainsKey(keyValuePair.Key))
            drawShadow = true;
        }
        else
          continue;
        int x1 = num3 + numArray[key3] % num5 * 68;
        int y1 = num4 + numArray[key3] / num5 * 68;
        if (y1 > this.yPositionOnScreen + height - 128)
        {
          this.collections[key3].Add(new List<ClickableTextureComponent>());
          numArray[key3] = 0;
          x1 = num3;
          y1 = num4;
        }
        if (this.collections[key3].Count == 0)
          this.collections[key3].Add(new List<ClickableTextureComponent>());
        List<ClickableTextureComponent> textureComponentList = this.collections[key3].Last<List<ClickableTextureComponent>>();
        ClickableTextureComponent textureComponent11 = new ClickableTextureComponent(keyValuePair.Key.ToString() + " " + drawShadow.ToString() + " " + flag.ToString(), new Rectangle(x1, y1, 64, 64), (string) null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, keyValuePair.Key, 16, 16), 4f, drawShadow);
        textureComponent11.myID = this.collections[key3].Last<List<ClickableTextureComponent>>().Count;
        textureComponent11.rightNeighborID = (this.collections[key3].Last<List<ClickableTextureComponent>>().Count + 1) % num5 == 0 ? -1 : this.collections[key3].Last<List<ClickableTextureComponent>>().Count + 1;
        textureComponent11.leftNeighborID = this.collections[key3].Last<List<ClickableTextureComponent>>().Count % num5 == 0 ? 7001 : this.collections[key3].Last<List<ClickableTextureComponent>>().Count - 1;
        textureComponent11.downNeighborID = y1 + 68 > this.yPositionOnScreen + height - 128 ? -7777 : this.collections[key3].Last<List<ClickableTextureComponent>>().Count + num5;
        textureComponent11.upNeighborID = this.collections[key3].Last<List<ClickableTextureComponent>>().Count < num5 ? 12345 : this.collections[key3].Last<List<ClickableTextureComponent>>().Count - num5;
        textureComponent11.fullyImmutable = true;
        textureComponentList.Add(textureComponent11);
        ++numArray[key3];
      }
      if (this.collections[5].Count == 0)
        this.collections[5].Add(new List<ClickableTextureComponent>());
      foreach (KeyValuePair<int, string> achievement in Game1.achievements)
      {
        bool flag = Game1.player.achievements.Contains(achievement.Key);
        string[] strArray = achievement.Value.Split('^');
        if (flag || strArray[2].Equals("true") && (strArray[3].Equals("-1") || this.farmerHasAchievements(strArray[3])))
        {
          int x2 = num3 + numArray[5] % num5 * 68;
          int y2 = num4 + numArray[5] / num5 * 68;
          this.collections[5][0].Add(new ClickableTextureComponent(achievement.Key.ToString() + " " + flag.ToString(), new Rectangle(x2, y2, 64, 64), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 25), 1f));
          ++numArray[5];
        }
      }
      if (Game1.player.secretNotesSeen.Count > 0)
      {
        if (this.collections[6].Count == 0)
          this.collections[6].Add(new List<ClickableTextureComponent>());
        this.secretNotesData = Game1.content.Load<Dictionary<int, string>>("Data\\SecretNotes");
        this.secretNoteImageTexture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\SecretNotesImages");
        bool flag1 = Game1.player.secretNotesSeen.Contains(GameLocation.JOURNAL_INDEX + 1);
        foreach (int key5 in this.secretNotesData.Keys)
        {
          if (key5 >= GameLocation.JOURNAL_INDEX)
          {
            if (!flag1)
              continue;
          }
          else if (!Game1.player.hasMagnifyingGlass)
            continue;
          int x3 = num3 + numArray[6] % num5 * 68;
          int y3 = num4 + numArray[6] / num5 * 68;
          bool flag2;
          if (key5 >= GameLocation.JOURNAL_INDEX)
          {
            List<ClickableTextureComponent> textureComponentList = this.collections[6][0];
            string str1 = key5.ToString();
            flag2 = Game1.player.secretNotesSeen.Contains(key5);
            string str2 = flag2.ToString();
            ClickableTextureComponent textureComponent12 = new ClickableTextureComponent(str1 + " " + str2, new Rectangle(x3, y3, 64, 64), (string) null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 842, 16, 16), 4f, Game1.player.secretNotesSeen.Contains(key5));
            textureComponentList.Add(textureComponent12);
          }
          else
          {
            List<ClickableTextureComponent> textureComponentList = this.collections[6][0];
            string str3 = key5.ToString();
            flag2 = Game1.player.secretNotesSeen.Contains(key5);
            string str4 = flag2.ToString();
            ClickableTextureComponent textureComponent13 = new ClickableTextureComponent(str3 + " " + str4, new Rectangle(x3, y3, 64, 64), (string) null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 79, 16, 16), 4f, Game1.player.secretNotesSeen.Contains(key5));
            textureComponentList.Add(textureComponent13);
          }
          ++numArray[6];
        }
      }
      if (this.collections[7].Count == 0)
        this.collections[7].Add(new List<ClickableTextureComponent>());
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
      foreach (string key6 in (NetList<string, NetString>) Game1.player.mailReceived)
      {
        if (dictionary.ContainsKey(key6))
        {
          int x4 = num3 + numArray[7] % num5 * 68;
          int y4 = num4 + numArray[7] / num5 * 68;
          string[] source = dictionary[key6].Split(new string[1]
          {
            "[#]"
          }, StringSplitOptions.None);
          if (y4 > this.yPositionOnScreen + height - 128)
          {
            this.collections[7].Add(new List<ClickableTextureComponent>());
            numArray[7] = 0;
            x4 = num3;
            y4 = num4;
          }
          List<ClickableTextureComponent> textureComponentList = this.collections[7].Last<List<ClickableTextureComponent>>();
          ClickableTextureComponent textureComponent14 = new ClickableTextureComponent(key6 + " true " + (((IEnumerable<string>) source).Count<string>() > 1 ? source[1] : "???"), new Rectangle(x4, y4, 64, 64), (string) null, "", Game1.mouseCursors, new Rectangle(190, 423, 14, 11), 4f, true);
          textureComponent14.myID = this.collections[7].Last<List<ClickableTextureComponent>>().Count;
          textureComponent14.rightNeighborID = (this.collections[7].Last<List<ClickableTextureComponent>>().Count + 1) % num5 == 0 ? -1 : this.collections[7].Last<List<ClickableTextureComponent>>().Count + 1;
          textureComponent14.leftNeighborID = this.collections[7].Last<List<ClickableTextureComponent>>().Count % num5 == 0 ? 7008 : this.collections[7].Last<List<ClickableTextureComponent>>().Count - 1;
          textureComponent14.downNeighborID = y4 + 68 > this.yPositionOnScreen + height - 128 ? -7777 : this.collections[7].Last<List<ClickableTextureComponent>>().Count + num5;
          textureComponent14.upNeighborID = this.collections[7].Last<List<ClickableTextureComponent>>().Count < num5 ? 12345 : this.collections[7].Last<List<ClickableTextureComponent>>().Count - num5;
          textureComponent14.fullyImmutable = true;
          textureComponentList.Add(textureComponent14);
          ++numArray[7];
        }
      }
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      base.customSnapBehavior(direction, oldRegion, oldID);
      switch (direction)
      {
        case 1:
          if (oldID != 706 || this.collections[this.currentTab].Count <= this.currentPage + 1)
            break;
          this.currentlySnappedComponent = this.getComponentWithID(707);
          break;
        case 2:
          if (this.currentPage > 0)
            this.currentlySnappedComponent = this.getComponentWithID(706);
          else if (this.currentPage == 0 && this.collections[this.currentTab].Count > 1)
            this.currentlySnappedComponent = this.getComponentWithID(707);
          this.backButton.upNeighborID = oldID;
          this.forwardButton.upNeighborID = oldID;
          break;
        case 3:
          if (oldID != 707 || this.currentPage <= 0)
            break;
          this.currentlySnappedComponent = this.getComponentWithID(706);
          break;
      }
    }

    public override void snapToDefaultClickableComponent()
    {
      base.snapToDefaultClickableComponent();
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    private bool farmerHasAchievements(string listOfAchievementNumbers)
    {
      foreach (string str in listOfAchievementNumbers.Split(' '))
      {
        if (!Game1.player.achievements.Contains(Convert.ToInt32(str)))
          return false;
      }
      return true;
    }

    public override bool readyToClose() => this.letterviewerSubMenu == null && base.readyToClose();

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.letterviewerSubMenu == null)
        return;
      this.letterviewerSubMenu.update(time);
      if (!this.letterviewerSubMenu.destroy)
        return;
      this.letterviewerSubMenu = (LetterViewerMenu) null;
      if (!Game1.options.SnappyMenus)
        return;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);
      if (this.letterviewerSubMenu == null)
        return;
      this.letterviewerSubMenu.receiveKeyPress(key);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.letterviewerSubMenu != null)
      {
        this.letterviewerSubMenu.receiveLeftClick(x, y, true);
      }
      else
      {
        foreach (KeyValuePair<int, ClickableTextureComponent> sideTab in this.sideTabs)
        {
          if (sideTab.Value.containsPoint(x, y) && this.currentTab != sideTab.Key)
          {
            Game1.playSound("smallSelect");
            this.sideTabs[this.currentTab].bounds.X -= CollectionsPage.widthToMoveActiveTab;
            this.currentTab = Convert.ToInt32(sideTab.Value.name);
            this.currentPage = 0;
            sideTab.Value.bounds.X += CollectionsPage.widthToMoveActiveTab;
          }
        }
        if (this.currentPage > 0 && this.backButton.containsPoint(x, y))
        {
          --this.currentPage;
          Game1.playSound("shwip");
          this.backButton.scale = this.backButton.baseScale;
          if (Game1.options.snappyMenus && Game1.options.gamepadControls && this.currentPage == 0)
          {
            this.currentlySnappedComponent = (ClickableComponent) this.forwardButton;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
          }
        }
        if (this.currentPage < this.collections[this.currentTab].Count - 1 && this.forwardButton.containsPoint(x, y))
        {
          ++this.currentPage;
          Game1.playSound("shwip");
          this.forwardButton.scale = this.forwardButton.baseScale;
          if (Game1.options.snappyMenus && Game1.options.gamepadControls && this.currentPage == this.collections[this.currentTab].Count - 1)
          {
            this.currentlySnappedComponent = (ClickableComponent) this.backButton;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
          }
        }
        if (this.currentTab == 7)
        {
          Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
          foreach (ClickableComponent clickableComponent in this.collections[this.currentTab][this.currentPage])
          {
            if (clickableComponent.containsPoint(x, y))
              this.letterviewerSubMenu = new LetterViewerMenu(dictionary[clickableComponent.name.Split(' ')[0]], clickableComponent.name.Split(' ')[0], true);
          }
        }
        else
        {
          if (this.currentTab != 6)
            return;
          foreach (ClickableComponent clickableComponent in this.collections[this.currentTab][this.currentPage])
          {
            if (clickableComponent.containsPoint(x, y))
            {
              int result = -1;
              string[] strArray = clickableComponent.name.Split(' ');
              if (strArray[1] == "True" && int.TryParse(strArray[0], out result))
              {
                this.letterviewerSubMenu = new LetterViewerMenu(result);
                this.letterviewerSubMenu.isFromCollection = true;
                break;
              }
            }
          }
        }
      }
    }

    public override bool shouldDrawCloseButton() => this.letterviewerSubMenu == null;

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (this.letterviewerSubMenu == null)
        return;
      this.letterviewerSubMenu.receiveRightClick(x, y, true);
    }

    public override void applyMovementKey(int direction)
    {
      if (this.letterviewerSubMenu != null)
        this.letterviewerSubMenu.applyMovementKey(direction);
      else
        base.applyMovementKey(direction);
    }

    public override void gamePadButtonHeld(Buttons b)
    {
      if (this.letterviewerSubMenu != null)
        this.letterviewerSubMenu.gamePadButtonHeld(b);
      else
        base.gamePadButtonHeld(b);
    }

    public override void receiveGamePadButton(Buttons b)
    {
      if (this.letterviewerSubMenu != null)
        this.letterviewerSubMenu.receiveGamePadButton(b);
      else
        base.receiveGamePadButton(b);
    }

    public override void performHoverAction(int x, int y)
    {
      this.descriptionText = "";
      this.hoverText = "";
      this.value = -1;
      this.secretNoteImage = -1;
      if (this.letterviewerSubMenu != null)
      {
        this.letterviewerSubMenu.performHoverAction(x, y);
      }
      else
      {
        foreach (ClickableTextureComponent textureComponent in this.sideTabs.Values)
        {
          if (textureComponent.containsPoint(x, y))
          {
            this.hoverText = textureComponent.hoverText;
            return;
          }
        }
        bool flag = false;
        foreach (ClickableTextureComponent textureComponent in this.collections[this.currentTab][this.currentPage])
        {
          if (textureComponent.containsPoint(x, y))
          {
            textureComponent.scale = Math.Min(textureComponent.scale + 0.02f, textureComponent.baseScale + 0.1f);
            string[] strArray = textureComponent.name.Split(' ');
            if (this.currentTab == 5 || strArray.Length > 1 && Convert.ToBoolean(strArray[1]) || strArray.Length > 2 && Convert.ToBoolean(strArray[2]))
            {
              this.hoverText = this.currentTab != 7 ? this.createDescription(Convert.ToInt32(strArray[0])) : Game1.parseText(textureComponent.name.Substring(textureComponent.name.IndexOf(' ', textureComponent.name.IndexOf(' ') + 1) + 1), Game1.smallFont, 256);
            }
            else
            {
              if (this.hoverText != "???")
                this.hoverItem = (Item) null;
              this.hoverText = "???";
            }
            flag = true;
          }
          else
            textureComponent.scale = Math.Max(textureComponent.scale - 0.02f, textureComponent.baseScale);
        }
        if (!flag)
          this.hoverItem = (Item) null;
        this.forwardButton.tryHover(x, y, 0.5f);
        this.backButton.tryHover(x, y, 0.5f);
      }
    }

    public string createDescription(int index)
    {
      string description = "";
      if (this.currentTab == 5)
      {
        string[] strArray = Game1.achievements[index].Split('^');
        description = description + strArray[0] + Environment.NewLine + Environment.NewLine + strArray[1];
      }
      else if (this.currentTab == 6)
      {
        if (this.secretNotesData != null)
        {
          description = index >= GameLocation.JOURNAL_INDEX ? description + Game1.content.LoadString("Strings\\Locations:Journal_Name") + " #" + (index - GameLocation.JOURNAL_INDEX).ToString() : description + Game1.content.LoadString("Strings\\Locations:Secret_Note_Name") + " #" + index.ToString();
          if (this.secretNotesData[index][0] == '!')
          {
            this.secretNoteImage = Convert.ToInt32(this.secretNotesData[index].Split(' ')[1]);
          }
          else
          {
            string str = Game1.parseText(Utility.ParseGiftReveals(this.secretNotesData[index]).TrimStart(' ', '^').Replace("^", Environment.NewLine).Replace("@", (string) (NetFieldBase<string, NetString>) Game1.player.name), Game1.smallFont, 512);
            string[] strArray1 = str.Split(new string[1]
            {
              Environment.NewLine
            }, StringSplitOptions.None);
            int length = 15;
            if (strArray1.Length > length)
            {
              string[] strArray2 = new string[length];
              for (int index1 = 0; index1 < length; ++index1)
                strArray2[index1] = strArray1[index1];
              str = string.Join(Environment.NewLine, strArray2).Trim() + Environment.NewLine + "(...)";
            }
            description = description + Environment.NewLine + Environment.NewLine + str;
          }
        }
      }
      else
      {
        string[] strArray = Game1.objectInformation[index].Split('/');
        string str1 = strArray[4];
        string str2 = description + str1 + Environment.NewLine + Environment.NewLine + Game1.parseText(strArray[5], Game1.smallFont, 256) + Environment.NewLine + Environment.NewLine;
        if (strArray[3].Contains("Arch"))
          description = str2 + (Game1.player.archaeologyFound.ContainsKey(index) ? Game1.content.LoadString("Strings\\UI:Collections_Description_ArtifactsFound", (object) Game1.player.archaeologyFound[index][0]) : "");
        else if (strArray[3].Contains("Cooking"))
        {
          description = str2 + (Game1.player.recipesCooked.ContainsKey(index) ? Game1.content.LoadString("Strings\\UI:Collections_Description_RecipesCooked", (object) Game1.player.recipesCooked[index]) : "");
          if (this.hoverItem == null || this.hoverItem.ParentSheetIndex != index)
          {
            this.hoverItem = (Item) new StardewValley.Object(index, 1);
            string name = this.hoverItem.Name;
            switch (name)
            {
              case "Cheese Cauli.":
                name = "Cheese Cauliflower";
                break;
              case "Cheese Cauliflower":
                name = "Cheese Cauli.";
                break;
              case "Cookie":
                name = "Cookies";
                break;
              case "Cranberry Sauce":
                name = "Cran. Sauce";
                break;
              case "Dish O' The Sea":
                name = "Dish o' The Sea";
                break;
              case "Eggplant Parmesan":
                name = "Eggplant Parm.";
                break;
              case "Vegetable Medley":
                name = "Vegetable Stew";
                break;
            }
            this.hoverCraftingRecipe = new CraftingRecipe(name, true);
          }
        }
        else if (strArray[3].Contains("Fish"))
        {
          description = str2 + Game1.content.LoadString("Strings\\UI:Collections_Description_FishCaught", (object) (Game1.player.fishCaught.ContainsKey(index) ? Game1.player.fishCaught[index][0] : 0));
          if (Game1.player.fishCaught.ContainsKey(index) && Game1.player.fishCaught[index][1] > 0)
            description = description + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Collections_Description_BiggestCatch", (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (object) (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? Math.Round((double) Game1.player.fishCaught[index][1] * 2.54) : (double) Game1.player.fishCaught[index][1])));
        }
        else
          description = strArray[3].Contains("Minerals") || strArray[3].Substring(strArray[3].Length - 3).Equals("-2") ? str2 + Game1.content.LoadString("Strings\\UI:Collections_Description_MineralsFound", (object) (Game1.player.mineralsFound.ContainsKey(index) ? Game1.player.mineralsFound[index] : 0)) : str2 + Game1.content.LoadString("Strings\\UI:Collections_Description_NumberShipped", (object) (Game1.player.basicShipped.ContainsKey(index) ? Game1.player.basicShipped[index] : 0));
        this.value = Convert.ToInt32(strArray[1]);
      }
      return description;
    }

    public override void draw(SpriteBatch b)
    {
      foreach (ClickableTextureComponent textureComponent in this.sideTabs.Values)
        textureComponent.draw(b);
      if (this.currentPage > 0)
        this.backButton.draw(b);
      if (this.currentPage < this.collections[this.currentTab].Count - 1)
        this.forwardButton.draw(b);
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      foreach (ClickableTextureComponent textureComponent in this.collections[this.currentTab][this.currentPage])
      {
        bool boolean = Convert.ToBoolean(textureComponent.name.Split(' ')[1]);
        bool flag = this.currentTab == 4 && Convert.ToBoolean(textureComponent.name.Split(' ')[2]);
        textureComponent.draw(b, flag ? Color.DimGray * 0.4f : (boolean ? Color.White : Color.Black * 0.2f), 0.86f);
        if (this.currentTab == 5 & boolean)
        {
          int num = new Random(Convert.ToInt32(textureComponent.name.Split(' ')[0])).Next(12);
          b.Draw(Game1.mouseCursors, new Vector2((float) (textureComponent.bounds.X + 16 + 16), (float) (textureComponent.bounds.Y + 20 + 16)), new Rectangle?(new Rectangle(256 + num % 6 * 64 / 2, 128 + num / 6 * 64 / 2, 32, 32)), Color.White, 0.0f, new Vector2(16f, 16f), textureComponent.scale, SpriteEffects.None, 0.88f);
        }
      }
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (this.hoverItem != null)
        IClickableMenu.drawToolTip(b, this.hoverItem.getDescription(), this.hoverItem.DisplayName, this.hoverItem, craftingIngredients: this.hoverCraftingRecipe);
      else if (!this.hoverText.Equals(""))
      {
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, moneyAmountToDisplayAtBottom: this.value);
        if (this.secretNoteImage != -1)
        {
          IClickableMenu.drawTextureBox(b, Game1.getOldMouseX(), Game1.getOldMouseY() + 64 + 32, 288, 288, Color.White);
          b.Draw(this.secretNoteImageTexture, new Vector2((float) (Game1.getOldMouseX() + 16), (float) (Game1.getOldMouseY() + 64 + 32 + 16)), new Rectangle?(new Rectangle(this.secretNoteImage * 64 % this.secretNoteImageTexture.Width, this.secretNoteImage * 64 / this.secretNoteImageTexture.Width * 64, 64, 64)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
        }
      }
      if (this.letterviewerSubMenu == null)
        return;
      this.letterviewerSubMenu.draw(b);
    }
  }
}
