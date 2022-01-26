// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LetterViewerMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class LetterViewerMenu : IClickableMenu
  {
    public const int region_backButton = 101;
    public const int region_forwardButton = 102;
    public const int region_acceptQuestButton = 103;
    public const int region_itemGrabButton = 104;
    public const int letterWidth = 320;
    public const int letterHeight = 180;
    public Texture2D letterTexture;
    public Texture2D secretNoteImageTexture;
    public int moneyIncluded;
    public int questID = -1;
    public int secretNoteImage = -1;
    public int whichBG;
    public string learnedRecipe = "";
    public string cookingOrCrafting = "";
    public string mailTitle;
    public List<string> mailMessage = new List<string>();
    public int page;
    public List<ClickableComponent> itemsToGrab = new List<ClickableComponent>();
    public float scale;
    public bool isMail;
    public bool isFromCollection;
    public new bool destroy;
    public int customTextColor = -1;
    public bool usingCustomBackground;
    public ClickableTextureComponent backButton;
    public ClickableTextureComponent forwardButton;
    public ClickableComponent acceptQuestButton;
    public const float scaleChange = 0.003f;

    public LetterViewerMenu(string text)
      : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, true)
    {
      Game1.playSound("shwip");
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent1.myID = 101;
      textureComponent1.rightNeighborID = 102;
      this.backButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent2.myID = 102;
      textureComponent2.leftNeighborID = 101;
      this.forwardButton = textureComponent2;
      this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
      text = this.ApplyCustomFormatting(text);
      this.mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(text, this.width - 64, this.height - 128);
      this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
      this.backButton.visible = this.page > 0;
      this.OnPageChange();
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public LetterViewerMenu(int secretNoteIndex)
      : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, true)
    {
      Game1.playSound("shwip");
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent1.myID = 101;
      textureComponent1.rightNeighborID = 102;
      this.backButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent2.myID = 102;
      textureComponent2.leftNeighborID = 101;
      this.forwardButton = textureComponent2;
      this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
      string str = Game1.content.Load<Dictionary<int, string>>("Data\\SecretNotes")[secretNoteIndex];
      if (str[0] == '!')
      {
        this.secretNoteImageTexture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\SecretNotesImages");
        this.secretNoteImage = Convert.ToInt32(str.Split(' ')[1]);
      }
      else
      {
        this.whichBG = secretNoteIndex > 1000 ? 0 : 1;
        this.mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(this.ApplyCustomFormatting(Utility.ParseGiftReveals(str.Replace("@", (string) (NetFieldBase<string, NetString>) Game1.player.name))), this.width - 64, this.height - 128);
      }
      this.OnPageChange();
      this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
      this.backButton.visible = this.page > 0;
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public virtual void OnPageChange()
    {
      this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
      this.backButton.visible = this.page > 0;
      foreach (ClickableComponent clickableComponent in this.itemsToGrab)
        clickableComponent.visible = this.ShouldShowInteractable();
      if (this.acceptQuestButton != null)
        this.acceptQuestButton.visible = this.ShouldShowInteractable();
      if (!Game1.options.SnappyMenus || this.currentlySnappedComponent != null && this.currentlySnappedComponent.visible)
        return;
      this.snapToDefaultClickableComponent();
    }

    public LetterViewerMenu(string mail, string mailTitle, bool fromCollection = false)
      : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, true)
    {
      this.isFromCollection = fromCollection;
      mail = mail.Split(new string[1]{ "[#]" }, StringSplitOptions.None)[0];
      mail = mail.Replace("@", Game1.player.Name);
      if (mail.Contains("%update"))
        mail = mail.Replace("%update", Utility.getStardewHeroStandingsString());
      this.isMail = true;
      Game1.playSound("shwip");
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent1.myID = 101;
      textureComponent1.rightNeighborID = 102;
      this.backButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent2.myID = 102;
      textureComponent2.leftNeighborID = 101;
      this.forwardButton = textureComponent2;
      this.acceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128, this.yPositionOnScreen + this.height - 128, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
      {
        myID = 103,
        rightNeighborID = 102,
        leftNeighborID = 101
      };
      this.mailTitle = mailTitle;
      this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
      if (mail.Contains("¦"))
        mail = Game1.player.IsMale ? mail.Substring(0, mail.IndexOf("¦")) : mail.Substring(mail.IndexOf("¦") + 1);
      if (mailTitle.Equals("winter_5_2") || mailTitle.Equals("winter_12_1") || mailTitle.ToLower().Contains("wizard"))
        this.whichBG = 2;
      else if (mailTitle.Equals("Sandy"))
        this.whichBG = 1;
      else if (mailTitle.Contains("Krobus"))
        this.whichBG = 3;
      mail = this.ApplyCustomFormatting(mail);
      if (mail.Contains("%item"))
      {
        string oldValue = mail.Substring(mail.IndexOf("%item"), mail.IndexOf("%%") + 2 - mail.IndexOf("%item"));
        string[] strArray1 = oldValue.Split(' ');
        mail = mail.Replace(oldValue, "");
        if (!this.isFromCollection)
        {
          if (strArray1[1].Equals("object"))
          {
            int maxValue = strArray1.Length - 1;
            int num = Game1.random.Next(2, maxValue);
            int index = num - num % 2;
            this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96), (Item) new StardewValley.Object(Vector2.Zero, Convert.ToInt32(strArray1[index]), Convert.ToInt32(strArray1[index + 1])))
            {
              myID = 104,
              leftNeighborID = 101,
              rightNeighborID = 102
            });
            this.backButton.rightNeighborID = 104;
            this.forwardButton.leftNeighborID = 104;
          }
          else if (strArray1[1].Equals("tools"))
          {
            for (int index = 2; index < strArray1.Length; ++index)
            {
              Item obj = (Item) null;
              string str = strArray1[index];
              if (!(str == "Axe"))
              {
                if (!(str == "Hoe"))
                {
                  if (!(str == "Can"))
                  {
                    if (!(str == "Scythe"))
                    {
                      if (str == "Pickaxe")
                        obj = (Item) new Pickaxe();
                    }
                    else
                      obj = (Item) new MeleeWeapon(47);
                  }
                  else
                    obj = (Item) new WateringCan();
                }
                else
                  obj = (Item) new Hoe();
              }
              else
                obj = (Item) new Axe();
              if (obj != null)
                this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96), obj));
            }
          }
          else if (strArray1[1].Equals("bigobject"))
          {
            int maxValue = strArray1.Length - 1;
            int index = Game1.random.Next(2, maxValue);
            this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96), (Item) new StardewValley.Object(Vector2.Zero, Convert.ToInt32(strArray1[index])))
            {
              myID = 104,
              leftNeighborID = 101,
              rightNeighborID = 102
            });
            this.backButton.rightNeighborID = 104;
            this.forwardButton.leftNeighborID = 104;
          }
          else if (strArray1[1].Equals("furniture"))
          {
            int maxValue = strArray1.Length - 1;
            int index = Game1.random.Next(2, maxValue);
            this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96), (Item) Furniture.GetFurnitureInstance(Convert.ToInt32(strArray1[index])))
            {
              myID = 104,
              leftNeighborID = 101,
              rightNeighborID = 102
            });
            this.backButton.rightNeighborID = 104;
            this.forwardButton.leftNeighborID = 104;
          }
          else if (strArray1[1].Equals("money"))
          {
            int num1 = strArray1.Length > 4 ? Game1.random.Next(Convert.ToInt32(strArray1[2]), Convert.ToInt32(strArray1[3])) : Convert.ToInt32(strArray1[2]);
            int num2 = num1 - num1 % 10;
            Game1.player.Money += num2;
            this.moneyIncluded = num2;
          }
          else if (strArray1[1].Equals("conversationTopic"))
          {
            string key = strArray1[2];
            int int32 = Convert.ToInt32(strArray1[3].Replace("%%", ""));
            Game1.player.activeDialogueEvents.Add(key, int32);
            if (key.Equals("ElliottGone3"))
              Utility.getHomeOfFarmer(Game1.player).fridge.Value.addItem((Item) new StardewValley.Object(732, 1));
          }
          else if (strArray1[1].Equals("cookingRecipe"))
          {
            Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
            int num = 1000;
            string key1 = "";
            foreach (string key2 in dictionary.Keys)
            {
              string[] strArray2 = dictionary[key2].Split('/');
              string[] strArray3 = strArray2[3].Split(' ');
              if (strArray3[0].Equals("f") && strArray3[1].Equals(mailTitle.Replace("Cooking", "")) && !Game1.player.cookingRecipes.ContainsKey(key2))
              {
                int int32 = Convert.ToInt32(strArray3[2]);
                if (int32 <= num)
                {
                  num = int32;
                  key1 = key2;
                  this.learnedRecipe = key2;
                  if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                    this.learnedRecipe = strArray2[strArray2.Length - 1];
                }
              }
            }
            if (key1 != "")
            {
              if (!Game1.player.cookingRecipes.ContainsKey(key1))
                Game1.player.cookingRecipes.Add(key1, 0);
              this.cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_cooking");
            }
          }
          else if (strArray1[1].Equals("craftingRecipe"))
          {
            this.learnedRecipe = strArray1[2].Replace('_', ' ');
            if (!Game1.player.craftingRecipes.ContainsKey(this.learnedRecipe))
              Game1.player.craftingRecipes.Add(this.learnedRecipe, 0);
            this.cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_crafting");
            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
            {
              Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
              if (dictionary.ContainsKey(this.learnedRecipe))
              {
                string[] strArray4 = dictionary[this.learnedRecipe].Split('/');
                this.learnedRecipe = strArray4[strArray4.Length - 1];
              }
            }
          }
          else if (strArray1[1].Equals("itemRecovery"))
          {
            if (Game1.player.recoveredItem != null)
            {
              Item recoveredItem = Game1.player.recoveredItem;
              Game1.player.recoveredItem = (Item) null;
              this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96), recoveredItem)
              {
                myID = 104,
                leftNeighborID = 101,
                rightNeighborID = 102
              });
              this.backButton.rightNeighborID = 104;
              this.forwardButton.leftNeighborID = 104;
            }
          }
          else if (strArray1[1].Equals("quest"))
          {
            this.questID = Convert.ToInt32(strArray1[2].Replace("%%", ""));
            if (strArray1.Length > 4)
            {
              if (!Game1.player.mailReceived.Contains("NOQUEST_" + this.questID.ToString()))
                Game1.player.addQuest(this.questID);
              this.questID = -1;
            }
            this.backButton.rightNeighborID = 103;
            this.forwardButton.leftNeighborID = 103;
          }
        }
      }
      if (mailTitle == "ccBulletinThankYou" && !Game1.player.hasOrWillReceiveMail("ccBulletinThankYouReceived"))
      {
        foreach (NPC allCharacter in Utility.getAllCharacters())
        {
          if (!(bool) (NetFieldBase<bool, NetBool>) allCharacter.datable && allCharacter.isVillager())
            Game1.player.changeFriendship(500, allCharacter);
        }
        Game1.addMailForTomorrow("ccBulletinThankYouReceived", true);
      }
      Random r = new Random((int) (Game1.uniqueIDForThisGame / 2UL) ^ Game1.year ^ (int) Game1.player.UniqueMultiplayerID);
      bool flag = fromCollection;
      if (Game1.currentSeason == "winter" && Game1.dayOfMonth >= 18 && Game1.dayOfMonth <= 25)
        flag = false;
      mail = mail.Replace("%secretsanta", flag ? "???" : Utility.getRandomTownNPC(r).displayName);
      int height = this.height - 128;
      if (this.HasInteractable())
        height = this.height - 128 - 32;
      this.mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(mail, this.width - 64, height);
      this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
      this.backButton.visible = this.page > 0;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
      if (this.mailMessage == null || this.mailMessage.Count > 1)
        return;
      this.backButton.myID = -100;
      this.forwardButton.myID = -100;
    }

    public virtual string ApplyCustomFormatting(string text)
    {
      for (int startIndex = text.IndexOf("["); startIndex >= 0; startIndex = text.IndexOf("[", startIndex + 1))
      {
        int num = text.IndexOf("]", startIndex);
        if (num >= 0)
        {
          bool flag = false;
          try
          {
            string[] strArray1 = text.Substring(startIndex + 1, num - startIndex - 1).Split(' ');
            if (strArray1[0] == "letterbg")
            {
              if (strArray1.Length == 2)
                this.whichBG = int.Parse(strArray1[1]);
              else if (strArray1.Length == 3)
              {
                this.usingCustomBackground = true;
                this.letterTexture = Game1.temporaryContent.Load<Texture2D>(strArray1[1]);
                this.whichBG = int.Parse(strArray1[2]);
              }
              flag = true;
            }
            else if (strArray1[0] == "textcolor")
            {
              string lower = strArray1[1].ToLower();
              string[] strArray2 = new string[9]
              {
                "black",
                "blue",
                "red",
                "purple",
                "white",
                "orange",
                "green",
                "cyan",
                "gray"
              };
              this.customTextColor = -1;
              for (int index = 0; index < strArray2.Length; ++index)
              {
                if (lower == strArray2[index])
                {
                  this.customTextColor = index;
                  break;
                }
              }
              flag = true;
            }
          }
          catch (Exception ex)
          {
          }
          if (flag)
          {
            text = text.Remove(startIndex, num - startIndex + 1);
            --startIndex;
          }
        }
      }
      return text;
    }

    public override void snapToDefaultClickableComponent()
    {
      if (this.questID != -1 && this.ShouldShowInteractable())
        this.currentlySnappedComponent = this.getComponentWithID(103);
      else if (this.itemsToGrab != null && this.itemsToGrab.Count > 0 && this.ShouldShowInteractable())
        this.currentlySnappedComponent = this.getComponentWithID(104);
      else if (this.currentlySnappedComponent == null || this.currentlySnappedComponent != this.backButton && this.currentlySnappedComponent != this.forwardButton)
        this.currentlySnappedComponent = (ClickableComponent) this.forwardButton;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.xPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X;
      this.yPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y;
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent1.myID = 101;
      textureComponent1.rightNeighborID = 102;
      this.backButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent2.myID = 102;
      textureComponent2.leftNeighborID = 101;
      this.forwardButton = textureComponent2;
      this.acceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128, this.yPositionOnScreen + this.height - 128, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
      {
        myID = 103,
        rightNeighborID = 102,
        leftNeighborID = 101
      };
      foreach (ClickableComponent clickableComponent in this.itemsToGrab)
        clickableComponent.bounds = new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96);
    }

    public override void receiveKeyPress(Keys key)
    {
      if (key == Keys.None)
        return;
      if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
        this.exitThisMenu(this.ShouldPlayExitSound());
      else
        base.receiveKeyPress(key);
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (this.isFromCollection && b == Buttons.B)
        this.exitThisMenu(false);
      else if (b == Buttons.LeftTrigger && this.page > 0)
      {
        --this.page;
        Game1.playSound("shwip");
        this.OnPageChange();
      }
      else
      {
        if (b != Buttons.RightTrigger || this.page >= this.mailMessage.Count - 1)
          return;
        ++this.page;
        Game1.playSound("shwip");
        this.OnPageChange();
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if ((double) this.scale < 1.0)
        return;
      if (this.upperRightCloseButton != null && this.readyToClose() && this.upperRightCloseButton.containsPoint(x, y))
      {
        if (playSound)
          Game1.playSound("bigDeSelect");
        if (!this.isFromCollection)
          this.exitThisMenu(this.ShouldPlayExitSound());
        else
          this.destroy = true;
      }
      if (Game1.activeClickableMenu == null && Game1.currentMinigame == null)
      {
        this.unload();
      }
      else
      {
        if (this.ShouldShowInteractable())
        {
          foreach (ClickableComponent clickableComponent in this.itemsToGrab)
          {
            if (clickableComponent.containsPoint(x, y) && clickableComponent.item != null)
            {
              Game1.playSound("coin");
              Game1.player.addItemByMenuIfNecessary(clickableComponent.item);
              clickableComponent.item = (Item) null;
              return;
            }
          }
        }
        if (this.backButton.containsPoint(x, y) && this.page > 0)
        {
          --this.page;
          Game1.playSound("shwip");
          this.OnPageChange();
        }
        else if (this.forwardButton.containsPoint(x, y) && this.page < this.mailMessage.Count - 1)
        {
          ++this.page;
          Game1.playSound("shwip");
          this.OnPageChange();
        }
        else if (this.ShouldShowInteractable() && this.acceptQuestButton != null && this.acceptQuestButton.containsPoint(x, y))
          this.AcceptQuest();
        else if (this.isWithinBounds(x, y))
        {
          if (this.page < this.mailMessage.Count - 1)
          {
            ++this.page;
            Game1.playSound("shwip");
            this.OnPageChange();
          }
          else if (!this.isMail)
          {
            this.exitThisMenuNoSound();
            Game1.playSound("shwip");
          }
          else
          {
            if (!this.isFromCollection)
              return;
            this.destroy = true;
          }
        }
        else
        {
          if (this.itemsLeftToGrab())
            return;
          if (!this.isFromCollection)
          {
            this.exitThisMenuNoSound();
            Game1.playSound("shwip");
          }
          else
            this.destroy = true;
        }
      }
    }

    public virtual bool ShouldPlayExitSound() => this.questID == -1 && !this.isFromCollection;

    public bool itemsLeftToGrab()
    {
      if (this.itemsToGrab == null)
        return false;
      foreach (ClickableComponent clickableComponent in this.itemsToGrab)
      {
        if (clickableComponent.item != null)
          return true;
      }
      return false;
    }

    public void AcceptQuest()
    {
      if (this.questID == -1)
        return;
      Game1.player.addQuest(this.questID);
      if (this.questID == 20)
        MineShaft.CheckForQiChallengeCompletion();
      this.questID = -1;
      Game1.playSound("newArtifact");
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      if (this.ShouldShowInteractable())
      {
        foreach (ClickableComponent clickableComponent in this.itemsToGrab)
          clickableComponent.scale = !clickableComponent.containsPoint(x, y) ? Math.Max(1f, clickableComponent.scale - 0.03f) : Math.Min(clickableComponent.scale + 0.03f, 1.1f);
      }
      this.backButton.tryHover(x, y, 0.6f);
      this.forwardButton.tryHover(x, y, 0.6f);
      if (!this.ShouldShowInteractable() || this.questID == -1)
        return;
      float scale = this.acceptQuestButton.scale;
      this.acceptQuestButton.scale = this.acceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
      if ((double) this.acceptQuestButton.scale <= (double) scale)
        return;
      Game1.playSound("Cowboy_gunshot");
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
      this.backButton.visible = this.page > 0;
      if ((double) this.scale < 1.0)
      {
        this.scale += (float) time.ElapsedGameTime.Milliseconds * (3f / 1000f);
        if ((double) this.scale >= 1.0)
          this.scale = 1f;
      }
      if (this.page >= this.mailMessage.Count - 1 || this.forwardButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
        return;
      this.forwardButton.scale = (float) (4.0 + Math.Sin((double) time.TotalGameTime.Milliseconds / (64.0 * Math.PI)) / 1.5);
    }

    public virtual int getTextColor()
    {
      if (this.customTextColor >= 0)
        return this.customTextColor;
      if (this.usingCustomBackground)
        return -1;
      switch (this.whichBG)
      {
        case 1:
          return 8;
        case 2:
          return 7;
        case 3:
          return 4;
        default:
          return -1;
      }
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
      b.Draw(this.letterTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2), (float) (this.yPositionOnScreen + this.height / 2)), new Rectangle?(new Rectangle(this.whichBG * 320, 0, 320, 180)), Color.White, 0.0f, new Vector2(160f, 90f), 4f * this.scale, SpriteEffects.None, 0.86f);
      if ((double) this.scale == 1.0)
      {
        if (this.secretNoteImage != -1)
        {
          b.Draw(this.secretNoteImageTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2 - 128 - 4), (float) (this.yPositionOnScreen + this.height / 2 - 128 + 8)), new Rectangle?(new Rectangle(this.secretNoteImage * 64 % this.secretNoteImageTexture.Width, this.secretNoteImage * 64 / this.secretNoteImageTexture.Width * 64, 64, 64)), Color.Black * 0.4f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
          b.Draw(this.secretNoteImageTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2 - 128), (float) (this.yPositionOnScreen + this.height / 2 - 128)), new Rectangle?(new Rectangle(this.secretNoteImage * 64 % this.secretNoteImageTexture.Width, this.secretNoteImage * 64 / this.secretNoteImageTexture.Width * 64, 64, 64)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
          b.Draw(this.secretNoteImageTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2 - 40), (float) (this.yPositionOnScreen + this.height / 2 - 192)), new Rectangle?(new Rectangle(193, 65, 14, 21)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.867f);
        }
        else
          SpriteText.drawString(b, this.mailMessage[this.page], this.xPositionOnScreen + 32, this.yPositionOnScreen + 32, width: (this.width - 64), alpha: 0.75f, layerDepth: 0.865f, color: this.getTextColor());
        if (this.ShouldShowInteractable())
        {
          foreach (ClickableComponent clickableComponent in this.itemsToGrab)
          {
            b.Draw(this.letterTexture, clickableComponent.bounds, new Rectangle?(new Rectangle(this.whichBG * 24, 180, 24, 24)), Color.White);
            if (clickableComponent.item != null)
              clickableComponent.item.drawInMenu(b, new Vector2((float) (clickableComponent.bounds.X + 16), (float) (clickableComponent.bounds.Y + 16)), clickableComponent.scale);
          }
          if (this.moneyIncluded > 0)
          {
            string s = Game1.content.LoadString("Strings\\UI:LetterViewer_MoneyIncluded", (object) this.moneyIncluded);
            SpriteText.drawString(b, s, this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString(s) / 2, this.yPositionOnScreen + this.height - 96, height: 9999, alpha: 0.75f, layerDepth: 0.865f);
          }
          else if (this.learnedRecipe != null && this.learnedRecipe.Length > 0)
          {
            string s = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", (object) this.cookingOrCrafting);
            SpriteText.drawStringHorizontallyCenteredAt(b, s, this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 - SpriteText.getHeightOfString(s) * 2, height: 9999, alpha: 0.65f, layerDepth: 0.865f, color: this.getTextColor());
            SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipeName", (object) this.learnedRecipe), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 - SpriteText.getHeightOfString("t"), height: 9999, alpha: 0.9f, layerDepth: 0.865f, color: this.getTextColor());
          }
        }
        base.draw(b);
        this.forwardButton.draw(b);
        this.backButton.draw(b);
        if (this.ShouldShowInteractable() && this.questID != -1)
        {
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.acceptQuestButton.bounds.X, this.acceptQuestButton.bounds.Y, this.acceptQuestButton.bounds.Width, this.acceptQuestButton.bounds.Height, (double) this.acceptQuestButton.scale > 1.0 ? Color.LightPink : Color.White, 4f * this.acceptQuestButton.scale);
          Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2((float) (this.acceptQuestButton.bounds.X + 12), (float) (this.acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12))), Game1.textColor);
        }
      }
      if (Game1.options.SnappyMenus && (double) this.scale < 1.0 || Game1.options.SnappyMenus && !this.forwardButton.visible && !this.backButton.visible && this.questID == -1 && !this.itemsLeftToGrab())
        return;
      this.drawMouse(b);
    }

    public virtual bool ShouldShowInteractable() => this.HasInteractable() && this.page == this.mailMessage.Count - 1;

    public virtual bool HasInteractable() => !this.isFromCollection && (this.questID != -1 || this.moneyIncluded > 0 || this.itemsToGrab.Count > 0 || this.learnedRecipe != null && this.learnedRecipe.Length > 0);

    public void unload()
    {
    }

    protected override void cleanupBeforeExit()
    {
      if (this.questID != -1)
        this.AcceptQuest();
      if (this.itemsLeftToGrab())
      {
        List<Item> itemsToAdd = new List<Item>();
        foreach (ClickableComponent clickableComponent in this.itemsToGrab)
        {
          if (clickableComponent.item != null)
          {
            itemsToAdd.Add(clickableComponent.item);
            clickableComponent.item = (Item) null;
          }
        }
        if (itemsToAdd.Count > 0)
        {
          Game1.playSound("coin");
          Game1.player.addItemsByMenuIfNecessary(itemsToAdd);
        }
      }
      if (this.isFromCollection)
      {
        this.destroy = true;
        Game1.oldKBState = Game1.GetKeyboardState();
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldPadState = Game1.input.GetGamePadState();
      }
      base.cleanupBeforeExit();
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (this.isFromCollection)
        this.destroy = true;
      else
        this.receiveLeftClick(x, y, playSound);
    }
  }
}
