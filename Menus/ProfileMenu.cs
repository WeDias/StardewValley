// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ProfileMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class ProfileMenu : IClickableMenu
  {
    public const int region_characterSelectors = 500;
    public const int region_categorySelector = 501;
    public const int region_itemButtons = 502;
    public const int region_backButton = 101;
    public const int region_forwardButton = 102;
    public const int region_upArrow = 105;
    public const int region_downArrow = 106;
    public const int region_scrollButtons = 107;
    public const int letterWidth = 320;
    public const int letterHeight = 180;
    public Texture2D letterTexture;
    public Texture2D secretNoteImageTexture;
    protected string hoverText = "";
    protected List<ProfileItem> _profileItems;
    protected Character _target;
    public Item hoveredItem;
    public ClickableTextureComponent backButton;
    public ClickableTextureComponent forwardButton;
    public ClickableTextureComponent nextCharacterButton;
    public ClickableTextureComponent previousCharacterButton;
    protected Rectangle characterSpriteBox;
    protected int _currentCategory;
    protected AnimatedSprite _animatedSprite;
    protected float _directionChangeTimer;
    protected float _hiddenEmoteTimer = -1f;
    protected int _currentDirection;
    protected int _hideTooltipTime;
    protected SocialPage _socialPage;
    protected string _status = "";
    protected string _printedName = "";
    protected Vector2 _characterEntrancePosition = new Vector2(0.0f, 0.0f);
    public ClickableTextureComponent upArrow;
    public ClickableTextureComponent downArrow;
    protected ClickableTextureComponent scrollBar;
    protected Rectangle scrollBarRunner;
    public List<ClickableComponent> clickableProfileItems;
    protected List<Character> _charactersList;
    protected Friendship _friendship;
    protected Vector2 _characterNamePosition;
    protected Vector2 _heartDisplayPosition;
    protected Vector2 _birthdayHeadingDisplayPosition;
    protected Vector2 _birthdayDisplayPosition;
    protected Vector2 _statusHeadingDisplayPosition;
    protected Vector2 _statusDisplayPosition;
    protected Vector2 _giftLogHeadingDisplayPosition;
    protected Vector2 _giftLogCategoryDisplayPosition;
    protected Vector2 _errorMessagePosition;
    protected Vector2 _characterSpriteDrawPosition;
    protected Rectangle _characterStatusDisplayBox;
    protected List<ClickableTextureComponent> _clickableTextureComponents;
    public Rectangle _itemDisplayRect;
    protected int scrollPosition;
    protected int scrollStep = 36;
    protected int scrollSize;
    public static ProfileMenu.ProfileItemCategory[] itemCategories = new ProfileMenu.ProfileItemCategory[10]
    {
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_LikedGifts", (int[]) null),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_FruitsAndVegetables", new int[2]
      {
        -75,
        -79
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_AnimalProduce", new int[4]
      {
        -6,
        -5,
        -14,
        -18
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_ArtisanItems", new int[1]
      {
        -26
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_CookedItems", new int[1]
      {
        -7
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_ForagedItems", new int[4]
      {
        -80,
        -81,
        -23,
        -17
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_Fish", new int[1]
      {
        -4
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_Ingredients", new int[2]
      {
        -27,
        -25
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_MineralsAndGems", new int[3]
      {
        -15,
        -12,
        -2
      }),
      new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_Misc", (int[]) null)
    };
    protected Dictionary<int, List<Item>> _sortedItems;
    public bool scrolling;
    private int _characterSpriteRandomInt;

    public ProfileMenu(Character character)
      : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, true)
    {
      this._charactersList = new List<Character>();
      this._socialPage = new SocialPage(0, 0, 0, 0);
      this._printedName = "";
      this._characterEntrancePosition = new Vector2(0.0f, 4f);
      foreach (object name in this._socialPage.names)
      {
        if (!(name is long) && name is string)
        {
          NPC characterFromName = Game1.getCharacterFromName((string) name);
          if (characterFromName != null && Game1.player.friendshipData.ContainsKey((string) (NetFieldBase<string, NetString>) characterFromName.name))
            this._charactersList.Add((Character) characterFromName);
        }
      }
      this._profileItems = new List<ProfileItem>();
      this.clickableProfileItems = new List<ClickableComponent>();
      this.UpdateButtons();
      this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
      this._SetCharacter(character);
    }

    public Character GetCharacter() => this._target;

    public NPC GetTemporaryCharacter(Character character)
    {
      Texture2D portrait;
      try
      {
        portrait = Game1.content.Load<Texture2D>("Portraits\\" + (character as NPC).getTextureName());
      }
      catch (Exception ex)
      {
        return (NPC) null;
      }
      int num = character.name.Contains("Dwarf") || character.name.Equals((object) "Krobus") ? 96 : 128;
      return new NPC(new AnimatedSprite("Characters\\" + (character as NPC).getTextureName(), 0, 16, num / 4), new Vector2(0.0f, 0.0f), character.Name, (int) character.facingDirection, (string) (NetFieldBase<string, NetString>) character.name, (Dictionary<int, int[]>) null, portrait, true);
    }

    protected void _SetCharacter(Character character)
    {
      this._target = character;
      this._sortedItems = new Dictionary<int, List<Item>>();
      if (this._target is NPC)
      {
        this._friendship = this._socialPage.getFriendship((string) (NetFieldBase<string, NetString>) this._target.name);
        NPC temporaryCharacter = this.GetTemporaryCharacter(this._target);
        this._animatedSprite = temporaryCharacter.Sprite.Clone();
        this._animatedSprite.tempSpriteHeight = -1;
        this._animatedSprite.faceDirection(2);
        foreach (int key1 in (IEnumerable<int>) Game1.objectInformation.Keys)
        {
          StardewValley.Object @object = new StardewValley.Object(key1, 1);
          if (Game1.player.hasGiftTasteBeenRevealed(temporaryCharacter, key1) && (!(@object.Name == "Stone") || key1 == 390))
          {
            for (int key2 = 0; key2 < ProfileMenu.itemCategories.Length; ++key2)
            {
              if (ProfileMenu.itemCategories[key2].categoryName == "Profile_Gift_Category_LikedGifts")
              {
                switch (temporaryCharacter.getGiftTasteForThisItem((Item) @object))
                {
                  case 0:
                  case 2:
                    if (!this._sortedItems.ContainsKey(key2))
                      this._sortedItems[key2] = new List<Item>();
                    this._sortedItems[key2].Add((Item) @object);
                    continue;
                  default:
                    continue;
                }
              }
              else if (ProfileMenu.itemCategories[key2].categoryName == "Profile_Gift_Category_Misc")
              {
                bool flag = false;
                for (int index = 0; index < ProfileMenu.itemCategories.Length; ++index)
                {
                  if (ProfileMenu.itemCategories[index].validCategories != null && ((IEnumerable<int>) ProfileMenu.itemCategories[index].validCategories).Contains<int>(@object.Category))
                  {
                    flag = true;
                    break;
                  }
                }
                if (!flag)
                {
                  temporaryCharacter.getGiftTasteForThisItem((Item) @object);
                  if (!this._sortedItems.ContainsKey(key2))
                    this._sortedItems[key2] = new List<Item>();
                  this._sortedItems[key2].Add((Item) @object);
                }
              }
              else if (((IEnumerable<int>) ProfileMenu.itemCategories[key2].validCategories).Contains<int>(@object.Category))
              {
                if (!this._sortedItems.ContainsKey(key2))
                  this._sortedItems[key2] = new List<Item>();
                this._sortedItems[key2].Add((Item) @object);
              }
            }
          }
        }
        int num1 = SocialPage.isDatable((string) (NetFieldBase<string, NetString>) this._target.name) ? 1 : 0;
        bool flag1 = this._friendship.IsMarried();
        bool flag2 = flag1 && SocialPage.isRoommateOfAnyone((string) (NetFieldBase<string, NetString>) this._target.name);
        this._status = "";
        int num2 = flag2 ? 1 : 0;
        if ((num1 | num2) != 0)
        {
          string text = !Game1.content.ShouldUseGenderedCharacterTranslations() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : (this._socialPage.getGender((string) (NetFieldBase<string, NetString>) this._target.name) == 0 ? ((IEnumerable<string>) Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')).First<string>() : ((IEnumerable<string>) Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')).Last<string>());
          if (flag2)
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:Housemate");
          else if (flag1)
            text = this._socialPage.getGender((string) (NetFieldBase<string, NetString>) this._target.name) == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637");
          else if (this._socialPage.isMarriedToAnyone((string) (NetFieldBase<string, NetString>) this._target.name))
            text = this._socialPage.getGender((string) (NetFieldBase<string, NetString>) this._target.name) == 0 ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC");
          else if (!Game1.player.isMarried() && this._friendship.IsDating())
            text = this._socialPage.getGender((string) (NetFieldBase<string, NetString>) this._target.name) == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640");
          else if (this._socialPage.getFriendship((string) (NetFieldBase<string, NetString>) this._target.name).IsDivorced())
            text = this._socialPage.getGender((string) (NetFieldBase<string, NetString>) this._target.name) == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643");
          this._status = Utility.capitalizeFirstLetter(Game1.parseText(text, Game1.smallFont, this.width).Replace("(", "").Replace(")", "").Replace("（", "").Replace("）", ""));
        }
        this._UpdateList();
      }
      this._directionChangeTimer = 2000f;
      this._currentDirection = 2;
      this._hiddenEmoteTimer = -1f;
    }

    public void ChangeCharacter(int offset)
    {
      int num = this._charactersList.IndexOf(this._target);
      if (num == -1)
      {
        if (this._charactersList.Count <= 0)
          return;
        this._SetCharacter(this._charactersList[0]);
      }
      else
      {
        int index = num + offset;
        while (index < 0)
          index += this._charactersList.Count;
        while (index >= this._charactersList.Count)
          index -= this._charactersList.Count;
        this._SetCharacter(this._charactersList[index]);
        Game1.playSound("smallSelect");
        this._printedName = "";
        this._characterEntrancePosition = new Vector2((float) (Math.Sign(offset) * -4), 0.0f);
        if (!Game1.options.SnappyMenus || this.currentlySnappedComponent != null && this.currentlySnappedComponent.visible)
          return;
        this.snapToDefaultClickableComponent();
      }
    }

    protected void _UpdateList()
    {
      for (int index = 0; index < this._profileItems.Count; ++index)
        this._profileItems[index].Unload();
      this._profileItems.Clear();
      if (!(this._target is NPC))
        return;
      NPC target = this._target as NPC;
      List<Item> values1 = new List<Item>();
      List<Item> values2 = new List<Item>();
      List<Item> values3 = new List<Item>();
      List<Item> values4 = new List<Item>();
      List<Item> values5 = new List<Item>();
      if (this._sortedItems.ContainsKey(this._currentCategory))
      {
        foreach (Item obj in this._sortedItems[this._currentCategory])
        {
          switch (target.getGiftTasteForThisItem(obj))
          {
            case 0:
              values1.Add(obj);
              continue;
            case 2:
              values2.Add(obj);
              continue;
            case 4:
              values4.Add(obj);
              continue;
            case 6:
              values5.Add(obj);
              continue;
            case 8:
              values3.Add(obj);
              continue;
            default:
              continue;
          }
        }
      }
      this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Loved"), values1));
      this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Liked"), values2));
      this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Neutral"), values3));
      this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Disliked"), values4));
      this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Hated"), values5));
      this.SetupLayout();
      this.populateClickableComponentList();
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || this.currentlySnappedComponent != null && this.allClickableComponents.Contains(this.currentlySnappedComponent))
        return;
      this.snapToDefaultClickableComponent();
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return (direction != 2 || a.region != 501 || b.region != 500) && base.IsAutomaticSnapValid(direction, a, b);
    }

    public override void snapToDefaultClickableComponent()
    {
      if (this.clickableProfileItems.Count > 0)
        this.currentlySnappedComponent = this.clickableProfileItems[0];
      else
        this.currentlySnappedComponent = (ClickableComponent) this.backButton;
      this.snapCursorToCurrentSnappedComponent();
    }

    public void UpdateButtons()
    {
      this._clickableTextureComponents = new List<ClickableTextureComponent>();
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(0, 0, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
      textureComponent1.myID = 105;
      textureComponent1.upNeighborID = 102;
      textureComponent1.upNeighborImmutable = true;
      textureComponent1.downNeighborID = 106;
      textureComponent1.downNeighborImmutable = true;
      textureComponent1.leftNeighborID = -99998;
      textureComponent1.leftNeighborImmutable = true;
      this.upArrow = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(0, 0, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
      textureComponent2.myID = 106;
      textureComponent2.upNeighborID = 105;
      textureComponent2.upNeighborImmutable = true;
      textureComponent2.leftNeighborID = -99998;
      textureComponent2.leftNeighborImmutable = true;
      this.downArrow = textureComponent2;
      this.scrollBar = new ClickableTextureComponent(new Rectangle(0, 0, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent3.myID = 101;
      textureComponent3.name = "Back Button";
      textureComponent3.upNeighborID = -99998;
      textureComponent3.downNeighborID = -99998;
      textureComponent3.downNeighborImmutable = true;
      textureComponent3.leftNeighborID = -99998;
      textureComponent3.rightNeighborID = -99998;
      textureComponent3.region = 501;
      this.backButton = textureComponent3;
      this._clickableTextureComponents.Add(this.backButton);
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent4.myID = 102;
      textureComponent4.name = "Forward Button";
      textureComponent4.upNeighborID = -99998;
      textureComponent4.downNeighborID = -99998;
      textureComponent4.downNeighborImmutable = true;
      textureComponent4.leftNeighborID = -99998;
      textureComponent4.rightNeighborID = -99998;
      textureComponent4.region = 501;
      this.forwardButton = textureComponent4;
      this._clickableTextureComponents.Add(this.forwardButton);
      ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent5.myID = 0;
      textureComponent5.name = "Previous Char";
      textureComponent5.upNeighborID = -99998;
      textureComponent5.downNeighborID = -99998;
      textureComponent5.leftNeighborID = -99998;
      textureComponent5.rightNeighborID = -99998;
      textureComponent5.region = 500;
      this.previousCharacterButton = textureComponent5;
      this._clickableTextureComponents.Add(this.previousCharacterButton);
      ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent6.myID = 0;
      textureComponent6.name = "Next Char";
      textureComponent6.upNeighborID = -99998;
      textureComponent6.downNeighborID = -99998;
      textureComponent6.leftNeighborID = -99998;
      textureComponent6.rightNeighborID = -99998;
      textureComponent6.region = 500;
      this.nextCharacterButton = textureComponent6;
      this._clickableTextureComponents.Add(this.nextCharacterButton);
      this._clickableTextureComponents.Add(this.upArrow);
      this._clickableTextureComponents.Add(this.downArrow);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      if (direction > 0)
      {
        this.Scroll(-this.scrollStep);
      }
      else
      {
        if (direction >= 0)
          return;
        this.Scroll(this.scrollStep);
      }
    }

    public void ChangePage(int offset)
    {
      this.scrollPosition = 0;
      this._currentCategory += offset;
      while (this._currentCategory < 0)
        this._currentCategory += ProfileMenu.itemCategories.Length;
      while (this._currentCategory >= ProfileMenu.itemCategories.Length)
        this._currentCategory -= ProfileMenu.itemCategories.Length;
      Game1.playSound("shwip");
      this._UpdateList();
      if (!Game1.options.SnappyMenus || this.currentlySnappedComponent != null && this.currentlySnappedComponent.visible)
        return;
      this.snapToDefaultClickableComponent();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.xPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X;
      this.yPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y;
      this.UpdateButtons();
      this.SetupLayout();
      this.initializeUpperRightCloseButton();
      this.populateClickableComponentList();
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      switch (b)
      {
        case Buttons.Back:
          this.PlayHiddenEmote();
          break;
        case Buttons.LeftShoulder:
          this.ChangeCharacter(-1);
          break;
        case Buttons.RightShoulder:
          this.ChangeCharacter(1);
          break;
        case Buttons.RightTrigger:
          this.ChangePage(1);
          break;
        case Buttons.LeftTrigger:
          this.ChangePage(-1);
          break;
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      if (key == Keys.None)
        return;
      if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
      {
        this.exitThisMenu();
      }
      else
      {
        if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || this.overrideSnappyMenuCursorMovementBan())
          return;
        this.applyMovementKey(key);
      }
    }

    public override void applyMovementKey(int direction)
    {
      base.applyMovementKey(direction);
      this.ConstrainSelectionToView();
    }

    public override void releaseLeftClick(int x, int y)
    {
      base.releaseLeftClick(x, y);
      this.scrolling = false;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.scrollBar.containsPoint(x, y))
        this.scrolling = true;
      else if (this.scrollBarRunner.Contains(x, y))
      {
        this.scrolling = true;
        this.leftClickHeld(x, y);
        this.releaseLeftClick(x, y);
      }
      if (this.upperRightCloseButton != null && this.readyToClose() && this.upperRightCloseButton.containsPoint(x, y))
        this.exitThisMenu();
      else if (Game1.activeClickableMenu == null && Game1.currentMinigame == null)
        this.unload();
      else if (this.backButton.containsPoint(x, y))
        this.ChangePage(-1);
      else if (this.forwardButton.containsPoint(x, y))
        this.ChangePage(1);
      else if (this.previousCharacterButton.containsPoint(x, y))
        this.ChangeCharacter(-1);
      else if (this.nextCharacterButton.containsPoint(x, y))
      {
        this.ChangeCharacter(1);
      }
      else
      {
        if (this.downArrow.containsPoint(x, y))
          this.Scroll(this.scrollStep);
        if (this.upArrow.containsPoint(x, y))
          this.Scroll(-this.scrollStep);
        if (!this.characterSpriteBox.Contains(x, y))
          return;
        this.PlayHiddenEmote();
      }
    }

    public void PlayHiddenEmote()
    {
      if (this.GetCharacter() == null)
        return;
      string name = (string) (NetFieldBase<string, NetString>) this.GetCharacter().name;
      if (Game1.player.getFriendshipHeartLevelForNPC((string) (NetFieldBase<string, NetString>) this.GetCharacter().name) >= 4)
      {
        this._currentDirection = 2;
        this._characterSpriteRandomInt = Game1.random.Next(4);
        if (name == "Leo")
        {
          if ((double) this._hiddenEmoteTimer > 0.0)
            return;
          Game1.playSound("parrot_squawk");
          this._hiddenEmoteTimer = 300f;
        }
        else
        {
          Game1.playSound("drumkit6");
          this._hiddenEmoteTimer = 4000f;
        }
      }
      else
      {
        this._currentDirection = 2;
        this._directionChangeTimer = 5000f;
        Game1.playSound("Cowboy_Footstep");
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.hoveredItem = (Item) null;
      if (this._itemDisplayRect.Contains(x, y))
      {
        foreach (ProfileItem profileItem in this._profileItems)
          profileItem.performHover(x, y);
      }
      this.upArrow.tryHover(x, y);
      this.downArrow.tryHover(x, y);
      this.backButton.tryHover(x, y, 0.6f);
      this.forwardButton.tryHover(x, y, 0.6f);
      this.nextCharacterButton.tryHover(x, y, 0.6f);
      this.previousCharacterButton.tryHover(x, y, 0.6f);
    }

    public void ConstrainSelectionToView()
    {
      if (!Game1.options.snappyMenus)
        return;
      if (this.currentlySnappedComponent != null && this.currentlySnappedComponent.region == 502 && !this._itemDisplayRect.Contains(this.currentlySnappedComponent.bounds))
      {
        if (this.currentlySnappedComponent.bounds.Bottom > this._itemDisplayRect.Bottom)
          this.Scroll((int) Math.Ceiling(((double) this.currentlySnappedComponent.bounds.Bottom - (double) this._itemDisplayRect.Bottom) / (double) this.scrollStep) * this.scrollStep);
        else if (this.currentlySnappedComponent.bounds.Top < this._itemDisplayRect.Top)
          this.Scroll((int) Math.Floor(((double) this.currentlySnappedComponent.bounds.Top - (double) this._itemDisplayRect.Top) / (double) this.scrollStep) * this.scrollStep);
      }
      if (this.scrollPosition > this.scrollStep)
        return;
      this.scrollPosition = 0;
      this.UpdateScroll();
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (this._target != null && this._target.displayName != null && this._printedName.Length < this._target.displayName.Length)
        this._printedName += this._target.displayName[this._printedName.Length].ToString();
      if (this._hideTooltipTime > 0)
      {
        this._hideTooltipTime -= time.ElapsedGameTime.Milliseconds;
        if (this._hideTooltipTime < 0)
          this._hideTooltipTime = 0;
      }
      if ((double) this._characterEntrancePosition.X != 0.0)
        this._characterEntrancePosition.X -= (float) Math.Sign(this._characterEntrancePosition.X) * 0.25f;
      if ((double) this._characterEntrancePosition.Y != 0.0)
        this._characterEntrancePosition.Y -= (float) Math.Sign(this._characterEntrancePosition.Y) * 0.25f;
      if (this._animatedSprite == null)
        return;
      if ((double) this._hiddenEmoteTimer > 0.0)
      {
        this._hiddenEmoteTimer -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this._hiddenEmoteTimer <= 0.0)
        {
          this._hiddenEmoteTimer = -1f;
          this._currentDirection = 2;
          this._directionChangeTimer = 2000f;
          if (this.GetCharacter() != null && (string) (NetFieldBase<string, NetString>) this.GetCharacter().name == "Leo")
            this.GetCharacter().Sprite.AnimateDown(time);
        }
      }
      else if ((double) this._directionChangeTimer > 0.0)
      {
        this._directionChangeTimer -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this._directionChangeTimer <= 0.0)
        {
          this._directionChangeTimer = 2000f;
          this._currentDirection = (this._currentDirection + 1) % 4;
        }
      }
      if (this._characterEntrancePosition != Vector2.Zero)
      {
        if ((double) this._characterEntrancePosition.X < 0.0)
          this._animatedSprite.AnimateRight(time, 2);
        else if ((double) this._characterEntrancePosition.X > 0.0)
          this._animatedSprite.AnimateLeft(time, 2);
        else if ((double) this._characterEntrancePosition.Y > 0.0)
        {
          this._animatedSprite.AnimateUp(time, 2);
        }
        else
        {
          if ((double) this._characterEntrancePosition.Y >= 0.0)
            return;
          this._animatedSprite.AnimateDown(time, 2);
        }
      }
      else if ((double) this._hiddenEmoteTimer > 0.0)
      {
        switch ((string) (NetFieldBase<string, NetString>) this.GetCharacter().name)
        {
          case "Abigail":
            this._animatedSprite.Animate(time, 16, 4, 200f);
            break;
          case "Alex":
            this._animatedSprite.Animate(time, 16, 8, 170f);
            break;
          case "Caroline":
            this._animatedSprite.Animate(time, 19, 1, 200f);
            break;
          case "Clint":
            this._animatedSprite.Animate(time, 39, 1, 200f);
            break;
          case "Demetrius":
            this._animatedSprite.Animate(time, 30, 2, 200f);
            break;
          case "Dwarf":
            this._animatedSprite.Animate(time, 16, 4, 100f);
            break;
          case "Elliott":
            this._animatedSprite.Animate(time, 33, 2, 800f);
            break;
          case "Emily":
            this._animatedSprite.Animate(time, 16 + this._characterSpriteRandomInt * 2, 2, 300f);
            break;
          case "Evelyn":
            this._animatedSprite.Animate(time, 20, 1, 200f);
            break;
          case "George":
            this._animatedSprite.Animate(time, 16, 4, 400f);
            break;
          case "Gus":
            this._animatedSprite.Animate(time, 18, 3, 200f);
            break;
          case "Haley":
            this._animatedSprite.Animate(time, 26, 1, 200f);
            break;
          case "Harvey":
            this._animatedSprite.Animate(time, 20, 2, 800f);
            break;
          case "Jas":
            this._animatedSprite.Animate(time, 16, 4, 100f);
            break;
          case "Jodi":
            this._animatedSprite.Animate(time, 16, 2, 200f);
            break;
          case "Kent":
            this._animatedSprite.Animate(time, 16, 1, 200f);
            break;
          case "Krobus":
            this._animatedSprite.Animate(time, 20, 4, 200f);
            break;
          case "Leah":
            this._animatedSprite.Animate(time, 16, 4, 200f);
            break;
          case "Leo":
            this._animatedSprite.Animate(time, 17, 1, 200f);
            break;
          case "Lewis":
            this._animatedSprite.Animate(time, 24, 1, 170f);
            break;
          case "Linus":
            this._animatedSprite.Animate(time, 22, 1, 200f);
            break;
          case "Marnie":
            this._animatedSprite.Animate(time, 28, 4, 120f);
            break;
          case "Maru":
            this._animatedSprite.Animate(time, 16, 8, 150f);
            break;
          case "Pam":
            this._animatedSprite.Animate(time, 28, 2, 200f);
            break;
          case "Penny":
            this._animatedSprite.Animate(time, 18, 2, 1000f);
            break;
          case "Pierre":
            this._animatedSprite.Animate(time, 23, 1, 200f);
            break;
          case "Robin":
            this._animatedSprite.Animate(time, 32, 2, 120f);
            break;
          case "Sam":
            this._animatedSprite.Animate(time, 20, 2, 300f);
            break;
          case "Sandy":
            this._animatedSprite.Animate(time, 16, 2, 200f);
            break;
          case "Sebastian":
            this._animatedSprite.Animate(time, 16, 8, 180f);
            break;
          case "Shane":
            this._animatedSprite.Animate(time, 28, 2, 500f);
            break;
          case "Vincent":
            this._animatedSprite.Animate(time, 18, 2, 600f);
            break;
          case "Willy":
            this._animatedSprite.Animate(time, 28, 4, 200f);
            break;
          case "Wizard":
            this._animatedSprite.Animate(time, 16, 1, 170f);
            break;
          default:
            this._animatedSprite.AnimateDown(time, 2);
            break;
        }
      }
      else
      {
        switch (this._currentDirection)
        {
          case 0:
            this._animatedSprite.AnimateUp(time, 2);
            break;
          case 1:
            this._animatedSprite.AnimateRight(time, 2);
            break;
          case 2:
            this._animatedSprite.AnimateDown(time, 2);
            break;
          case 3:
            this._animatedSprite.AnimateLeft(time, 2);
            break;
        }
      }
    }

    public void SetupLayout()
    {
      int x = this.xPositionOnScreen + 64 - 12;
      int y = this.yPositionOnScreen + IClickableMenu.borderWidth;
      Rectangle rectangle1 = new Rectangle(x, y, 400, 720 - IClickableMenu.borderWidth * 2);
      Rectangle rectangle2 = new Rectangle(x, y, 1204, 720 - IClickableMenu.borderWidth * 2);
      rectangle2.X += rectangle1.Width;
      rectangle2.Width -= rectangle1.Width;
      this._characterStatusDisplayBox = new Rectangle(rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height);
      rectangle1.Y += 32;
      rectangle1.Height -= 32;
      this._characterSpriteDrawPosition = new Vector2((float) (rectangle1.X + (rectangle1.Width - Game1.nightbg.Width) / 2), (float) rectangle1.Y);
      this.characterSpriteBox = new Rectangle(this.xPositionOnScreen + 64 - 12 + (400 - Game1.nightbg.Width) / 2, this.yPositionOnScreen + IClickableMenu.borderWidth, Game1.nightbg.Width, Game1.nightbg.Height);
      this.previousCharacterButton.bounds.X = (int) this._characterSpriteDrawPosition.X - 64 - this.previousCharacterButton.bounds.Width / 2;
      this.previousCharacterButton.bounds.Y = (int) this._characterSpriteDrawPosition.Y + Game1.nightbg.Height / 2 - this.previousCharacterButton.bounds.Height / 2;
      this.nextCharacterButton.bounds.X = (int) this._characterSpriteDrawPosition.X + Game1.nightbg.Width + 64 - this.nextCharacterButton.bounds.Width / 2;
      this.nextCharacterButton.bounds.Y = (int) this._characterSpriteDrawPosition.Y + Game1.nightbg.Height / 2 - this.nextCharacterButton.bounds.Height / 2;
      rectangle1.Y += Game1.daybg.Height + 32;
      rectangle1.Height -= Game1.daybg.Height + 32;
      this._characterNamePosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
      rectangle1.Y += 96;
      rectangle1.Height -= 96;
      this._heartDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
      if (this._target is NPC)
      {
        rectangle1.Y += 56;
        rectangle1.Height -= 48;
        this._birthdayHeadingDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
        if ((this._target as NPC).birthday_Season.Value != null)
        {
          int seasonNumber = Utility.getSeasonNumber((string) (NetFieldBase<string, NetString>) (this._target as NPC).birthday_Season);
          if (seasonNumber >= 0)
          {
            rectangle1.Y += 48;
            rectangle1.Height -= 48;
            this._birthdayDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
            string str = (this._target as NPC).Birthday_Day.ToString() + " " + Utility.getSeasonNameFromNumber(seasonNumber);
            rectangle1.Y += 64;
            rectangle1.Height -= 64;
          }
        }
        if (this._status != "")
        {
          this._statusHeadingDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
          rectangle1.Y += 48;
          rectangle1.Height -= 48;
          this._statusDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
          rectangle1.Y += 64;
          rectangle1.Height -= 64;
        }
      }
      rectangle2.Height -= 96;
      rectangle2.Y -= 8;
      this._giftLogHeadingDisplayPosition = new Vector2((float) rectangle2.Center.X, (float) rectangle2.Top);
      rectangle2.Y += 80;
      rectangle2.Height -= 70;
      this.backButton.bounds.X = rectangle2.Left + 64 - this.forwardButton.bounds.Width / 2;
      this.backButton.bounds.Y = rectangle2.Top;
      this.forwardButton.bounds.X = rectangle2.Right - 64 - this.forwardButton.bounds.Width / 2;
      this.forwardButton.bounds.Y = rectangle2.Top;
      rectangle2.Width -= 250;
      rectangle2.X += 125;
      this._giftLogCategoryDisplayPosition = new Vector2((float) rectangle2.Center.X, (float) rectangle2.Top);
      rectangle2.Y += 64;
      rectangle2.Y += 32;
      rectangle2.Height -= 32;
      this._itemDisplayRect = rectangle2;
      int num = 64;
      this.scrollBarRunner = new Rectangle(rectangle2.Right + 48, rectangle2.Top + num, this.scrollBar.bounds.Width, rectangle2.Height - num * 2);
      this.downArrow.bounds.Y = this.scrollBarRunner.Bottom + 16;
      this.downArrow.bounds.X = this.scrollBarRunner.Center.X - this.downArrow.bounds.Width / 2;
      this.upArrow.bounds.Y = this.scrollBarRunner.Top - 16 - this.upArrow.bounds.Height;
      this.upArrow.bounds.X = this.scrollBarRunner.Center.X - this.upArrow.bounds.Width / 2;
      float draw_y = 0.0f;
      if (this._profileItems.Count > 0)
      {
        int index1 = 0;
        for (int index2 = 0; index2 < this._profileItems.Count; ++index2)
        {
          ProfileItem profileItem = this._profileItems[index2];
          if (profileItem.ShouldDraw())
          {
            draw_y = profileItem.HandleLayout(draw_y, this._itemDisplayRect, index1);
            ++index1;
          }
        }
      }
      this.scrollSize = (int) draw_y - this._itemDisplayRect.Height;
      if (this.NeedsScrollBar())
      {
        this.upArrow.visible = true;
        this.downArrow.visible = true;
      }
      else
      {
        this.upArrow.visible = false;
        this.downArrow.visible = false;
      }
      this.UpdateScroll();
    }

    public override void leftClickHeld(int x, int y)
    {
      if (GameMenu.forcePreventClose)
        return;
      base.leftClickHeld(x, y);
      if (!this.scrolling)
        return;
      int scrollPosition1 = this.scrollPosition;
      Console.WriteLine((float) (y - this.scrollBarRunner.Top) / (float) this.scrollBarRunner.Height);
      this.scrollPosition = (int) Math.Round((double) (y - this.scrollBarRunner.Top) / (double) this.scrollBarRunner.Height * (double) this.scrollSize / (double) this.scrollStep) * this.scrollStep;
      this.UpdateScroll();
      int scrollPosition2 = this.scrollPosition;
      if (scrollPosition1 == scrollPosition2)
        return;
      Game1.playSound("shiny4");
    }

    public bool NeedsScrollBar() => this.scrollSize > 0;

    public void Scroll(int offset)
    {
      if (!this.NeedsScrollBar())
        return;
      int scrollPosition1 = this.scrollPosition;
      this.scrollPosition += offset;
      this.UpdateScroll();
      int scrollPosition2 = this.scrollPosition;
      if (scrollPosition1 == scrollPosition2)
        return;
      Game1.playSound("shwip");
    }

    public virtual void UpdateScroll()
    {
      this.scrollPosition = Utility.Clamp(this.scrollPosition, 0, this.scrollSize);
      float draw_y = (float) (this._itemDisplayRect.Top - this.scrollPosition);
      this._errorMessagePosition = new Vector2((float) this._itemDisplayRect.Center.X, (float) this._itemDisplayRect.Center.Y);
      if (this._profileItems.Count > 0)
      {
        int index1 = 0;
        for (int index2 = 0; index2 < this._profileItems.Count; ++index2)
        {
          ProfileItem profileItem = this._profileItems[index2];
          if (profileItem.ShouldDraw())
          {
            draw_y = profileItem.HandleLayout(draw_y, this._itemDisplayRect, index1);
            ++index1;
          }
        }
      }
      if (this.scrollSize <= 0)
        return;
      this.scrollBar.bounds.X = this.scrollBarRunner.Center.X - this.scrollBar.bounds.Width / 2;
      this.scrollBar.bounds.Y = (int) Utility.Lerp((float) this.scrollBarRunner.Top, (float) (this.scrollBarRunner.Bottom - this.scrollBar.bounds.Height), (float) this.scrollPosition / (float) this.scrollSize);
      if (!Game1.options.SnappyMenus)
        return;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
      b.Draw(this.letterTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2), (float) (this.yPositionOnScreen + this.height / 2)), new Rectangle?(new Rectangle(0, 0, 320, 180)), Color.White, 0.0f, new Vector2(160f, 90f), 4f, SpriteEffects.None, 0.86f);
      Game1.DrawBox(this._characterStatusDisplayBox.X, this._characterStatusDisplayBox.Y, this._characterStatusDisplayBox.Width, this._characterStatusDisplayBox.Height);
      b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, this._characterSpriteDrawPosition, Color.White);
      Vector2 vector2 = new Vector2(0.0f, (float) ((32 - this._animatedSprite.SpriteHeight) * 4)) + this._characterEntrancePosition * 4f;
      if (!(this._target is Farmer) && this._target is NPC)
      {
        this._animatedSprite.draw(b, new Vector2(this._characterSpriteDrawPosition.X + 32f + vector2.X, this._characterSpriteDrawPosition.Y + 32f + vector2.Y), 0.8f);
        int heartLevelForNpc = Game1.player.getFriendshipHeartLevelForNPC((string) (NetFieldBase<string, NetString>) this._target.name);
        bool flag1 = SocialPage.isDatable((string) (NetFieldBase<string, NetString>) this._target.name);
        bool flag2 = this._friendship.IsMarried();
        int num1 = !flag2 ? 0 : (SocialPage.isRoommateOfAnyone((string) (NetFieldBase<string, NetString>) this._target.name) ? 1 : 0);
        int val2 = Math.Max(10, Utility.GetMaximumHeartsForCharacter(this._target));
        float num2 = this._heartDisplayPosition.X - (float) (Math.Min(10, val2) * 32 / 2);
        float num3 = val2 > 10 ? -16f : 0.0f;
        for (int index = 0; index < val2; ++index)
        {
          int x = index < heartLevelForNpc ? 211 : 218;
          if (flag1 && !this._friendship.IsDating() && !flag2 && index >= 8)
            x = 211;
          if (index < 10)
            b.Draw(Game1.mouseCursors, new Vector2(num2 + (float) (index * 32), this._heartDisplayPosition.Y + num3), new Rectangle?(new Rectangle(x, 428, 7, 6)), !flag1 || this._friendship.IsDating() || flag2 || index < 8 ? Color.White : Color.Black * 0.35f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
          else
            b.Draw(Game1.mouseCursors, new Vector2(num2 + (float) ((index - 10) * 32), (float) ((double) this._heartDisplayPosition.Y + (double) num3 + 32.0)), new Rectangle?(new Rectangle(x, 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
        }
      }
      if (this._printedName.Length < this._target.displayName.Length)
        SpriteText.drawStringWithScrollCenteredAt(b, "", (int) this._characterNamePosition.X, (int) this._characterNamePosition.Y, this._printedName);
      else
        SpriteText.drawStringWithScrollCenteredAt(b, this._target.displayName, (int) this._characterNamePosition.X, (int) this._characterNamePosition.Y);
      if ((this._target as NPC).birthday_Season.Value != null)
      {
        int seasonNumber = Utility.getSeasonNumber((string) (NetFieldBase<string, NetString>) (this._target as NPC).birthday_Season);
        if (seasonNumber >= 0)
        {
          SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:Profile_Birthday"), (int) this._birthdayHeadingDisplayPosition.X, (int) this._birthdayHeadingDisplayPosition.Y);
          string text = (this._target as NPC).Birthday_Day.ToString() + " " + Utility.getSeasonNameFromNumber(seasonNumber);
          b.DrawString(Game1.dialogueFont, text, new Vector2((float) (-(double) Game1.dialogueFont.MeasureString(text).X / 2.0) + this._birthdayDisplayPosition.X, this._birthdayDisplayPosition.Y), Game1.textColor);
        }
        if (this._status != "")
        {
          SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:Profile_Status"), (int) this._statusHeadingDisplayPosition.X, (int) this._statusHeadingDisplayPosition.Y);
          b.DrawString(Game1.dialogueFont, this._status, new Vector2((float) (-(double) Game1.dialogueFont.MeasureString(this._status).X / 2.0) + this._statusDisplayPosition.X, this._statusDisplayPosition.Y), Game1.textColor);
        }
      }
      SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\UI:Profile_GiftLog"), (int) this._giftLogHeadingDisplayPosition.X, (int) this._giftLogHeadingDisplayPosition.Y);
      SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:" + ProfileMenu.itemCategories[this._currentCategory].categoryName, (object) this._target.displayName), (int) this._giftLogCategoryDisplayPosition.X, (int) this._giftLogCategoryDisplayPosition.Y);
      bool flag3 = false;
      b.End();
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
      b.GraphicsDevice.ScissorRectangle = this._itemDisplayRect;
      if (this._profileItems.Count > 0)
      {
        for (int index = 0; index < this._profileItems.Count; ++index)
        {
          ProfileItem profileItem = this._profileItems[index];
          if (profileItem.ShouldDraw())
          {
            flag3 = true;
            profileItem.Draw(b);
          }
        }
      }
      b.End();
      b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (this.NeedsScrollBar())
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f, false);
        this.scrollBar.draw(b);
      }
      if (!flag3)
      {
        string text = Game1.content.LoadString("Strings\\UI:Profile_GiftLog_NoGiftsGiven");
        b.DrawString(Game1.smallFont, text, new Vector2((float) (-(double) Game1.smallFont.MeasureString(text).X / 2.0) + this._errorMessagePosition.X, this._errorMessagePosition.Y), Game1.textColor);
      }
      foreach (ClickableTextureComponent textureComponent in this._clickableTextureComponents)
        textureComponent.draw(b);
      base.draw(b);
      this.drawMouse(b, true);
      if (this.hoveredItem == null)
        return;
      bool flag4 = true;
      if (Game1.options.snappyMenus && Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse && this._hideTooltipTime > 0)
        flag4 = false;
      if (!flag4)
        return;
      IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem);
    }

    public void unload() => this._socialPage = (SocialPage) null;

    public override void receiveRightClick(int x, int y, bool playSound = true) => this.receiveLeftClick(x, y, playSound);

    public void RegisterClickable(ClickableComponent clickable) => this.clickableProfileItems.Add(clickable);

    public void UnregisterClickable(ClickableComponent clickable) => this.clickableProfileItems.Remove(clickable);

    public class ProfileItemCategory
    {
      public string categoryName;
      public int[] validCategories;

      public ProfileItemCategory(string name, int[] valid_categories)
      {
        this.categoryName = name;
        this.validCategories = valid_categories;
      }
    }
  }
}
