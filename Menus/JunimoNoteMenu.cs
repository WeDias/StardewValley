// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.JunimoNoteMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class JunimoNoteMenu : IClickableMenu
  {
    public const int region_ingredientSlotModifier = 250;
    public const int region_ingredientListModifier = 1000;
    public const int region_bundleModifier = 5000;
    public const int region_areaNextButton = 101;
    public const int region_areaBackButton = 102;
    public const int region_backButton = 103;
    public const int region_purchaseButton = 104;
    public const int region_presentButton = 105;
    public const string noteTextureName = "LooseSprites\\JunimoNote";
    public Texture2D noteTexture;
    private bool specificBundlePage;
    public const int baseWidth = 320;
    public const int baseHeight = 180;
    public InventoryMenu inventory;
    public Item partialDonationItem;
    public List<Item> partialDonationComponents = new List<Item>();
    public BundleIngredientDescription? currentPartialIngredientDescription;
    public int currentPartialIngredientDescriptionIndex = -1;
    private Item heldItem;
    private Item hoveredItem;
    public static bool canClick = true;
    private int whichArea;
    public bool bundlesChanged;
    public static ScreenSwipe screenSwipe;
    public static string hoverText = "";
    public List<Bundle> bundles = new List<Bundle>();
    public static List<TemporaryAnimatedSprite> tempSprites = new List<TemporaryAnimatedSprite>();
    public List<ClickableTextureComponent> ingredientSlots = new List<ClickableTextureComponent>();
    public List<ClickableTextureComponent> ingredientList = new List<ClickableTextureComponent>();
    public List<ClickableTextureComponent> otherClickableComponents = new List<ClickableTextureComponent>();
    public bool fromGameMenu;
    public bool fromThisMenu;
    public bool scrambledText;
    public ClickableTextureComponent backButton;
    public ClickableTextureComponent purchaseButton;
    public ClickableTextureComponent areaNextButton;
    public ClickableTextureComponent areaBackButton;
    public ClickableAnimatedComponent presentButton;
    private Bundle currentPageBundle;

    public JunimoNoteMenu(bool fromGameMenu, int area = 1, bool fromThisMenu = false)
      : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 360, 1280, 720, true)
    {
      CommunityCenter locationFromName = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
      if (fromGameMenu && !fromThisMenu)
      {
        for (int index = 0; index < locationFromName.areasComplete.Count; ++index)
        {
          if (locationFromName.shouldNoteAppearInArea(index) && !locationFromName.areasComplete[index])
          {
            area = index;
            this.whichArea = area;
            break;
          }
        }
        if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible") && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
          area = 6;
      }
      this.setUpMenu(area, locationFromName.bundlesDict());
      Game1.player.forceCanMove();
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 128, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent1.visible = false;
      textureComponent1.myID = 101;
      textureComponent1.leftNeighborID = 102;
      textureComponent1.leftNeighborImmutable = true;
      textureComponent1.downNeighborID = -99998;
      this.areaNextButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent2.visible = false;
      textureComponent2.myID = 102;
      textureComponent2.rightNeighborID = 101;
      textureComponent2.rightNeighborImmutable = true;
      textureComponent2.downNeighborID = -99998;
      this.areaBackButton = textureComponent2;
      int num = 6;
      for (int area1 = 0; area1 < num; ++area1)
      {
        if (area1 != area && locationFromName.shouldNoteAppearInArea(area1))
        {
          this.areaNextButton.visible = true;
          this.areaBackButton.visible = true;
          break;
        }
      }
      this.fromGameMenu = fromGameMenu;
      this.fromThisMenu = fromThisMenu;
      foreach (Bundle bundle in this.bundles)
        bundle.depositsAllowed = false;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public JunimoNoteMenu(int whichArea, Dictionary<int, bool[]> bundlesComplete)
      : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 360, 1280, 720, true)
    {
      this.setUpMenu(whichArea, bundlesComplete);
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      if (this.specificBundlePage)
        this.currentlySnappedComponent = this.getComponentWithID(0);
      else
        this.currentlySnappedComponent = this.getComponentWithID(5000);
      this.snapCursorToCurrentSnappedComponent();
    }

    protected override bool _ShouldAutoSnapPrioritizeAlignedElements() => !this.specificBundlePage;

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      if (!Game1.player.hasOrWillReceiveMail("canReadJunimoText") || oldID - 5000 < 0 || oldID - 5000 >= 10 || this.currentlySnappedComponent == null)
        return;
      int num1 = -1;
      int num2 = 999999;
      Point center1 = this.currentlySnappedComponent.bounds.Center;
      for (int index = 0; index < this.bundles.Count; ++index)
      {
        if (this.bundles[index].myID != oldID)
        {
          int num3 = 999999;
          Point center2 = this.bundles[index].bounds.Center;
          switch (direction)
          {
            case 0:
              if (center2.Y < center1.Y)
              {
                num3 = center1.Y - center2.Y + Math.Abs(center1.X - center2.X) * 3;
                break;
              }
              break;
            case 1:
              if (center2.X > center1.X)
              {
                num3 = center2.X - center1.X + Math.Abs(center1.Y - center2.Y) * 3;
                break;
              }
              break;
            case 2:
              if (center2.Y > center1.Y)
              {
                num3 = center2.Y - center1.Y + Math.Abs(center1.X - center2.X) * 3;
                break;
              }
              break;
            case 3:
              if (center2.X < center1.X)
              {
                num3 = center1.X - center2.X + Math.Abs(center1.Y - center2.Y) * 3;
                break;
              }
              break;
          }
          if (num3 < 10000 && num3 < num2)
          {
            num2 = num3;
            num1 = index;
          }
        }
      }
      if (num1 != -1)
      {
        this.currentlySnappedComponent = this.getComponentWithID(num1 + 5000);
        this.snapCursorToCurrentSnappedComponent();
      }
      else
      {
        switch (direction)
        {
          case 1:
            if (this.areaNextButton == null || !this.areaNextButton.visible)
              break;
            this.currentlySnappedComponent = (ClickableComponent) this.areaNextButton;
            this.snapCursorToCurrentSnappedComponent();
            this.areaNextButton.leftNeighborID = oldID;
            break;
          case 2:
            if (this.presentButton == null)
              break;
            this.currentlySnappedComponent = (ClickableComponent) this.presentButton;
            this.snapCursorToCurrentSnappedComponent();
            this.presentButton.upNeighborID = oldID;
            break;
          case 3:
            if (this.areaBackButton == null || !this.areaBackButton.visible)
              break;
            this.currentlySnappedComponent = (ClickableComponent) this.areaBackButton;
            this.snapCursorToCurrentSnappedComponent();
            this.areaBackButton.rightNeighborID = oldID;
            break;
        }
      }
    }

    public void setUpMenu(int whichArea, Dictionary<int, bool[]> bundlesComplete)
    {
      this.noteTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\JunimoNote");
      if (!Game1.player.hasOrWillReceiveMail("seenJunimoNote"))
      {
        Game1.player.removeQuest(26);
        Game1.player.mailReceived.Add("seenJunimoNote");
      }
      if (!Game1.player.hasOrWillReceiveMail("wizardJunimoNote"))
        Game1.addMailForTomorrow("wizardJunimoNote");
      if (!Game1.player.hasOrWillReceiveMail("hasSeenAbandonedJunimoNote") && whichArea == 6)
        Game1.player.mailReceived.Add("hasSeenAbandonedJunimoNote");
      this.scrambledText = !Game1.player.hasOrWillReceiveMail("canReadJunimoText");
      JunimoNoteMenu.tempSprites.Clear();
      this.whichArea = whichArea;
      this.inventory = new InventoryMenu(this.xPositionOnScreen + 128, this.yPositionOnScreen + 140, true, highlightMethod: new InventoryMenu.highlightThisItem(this.HighlightObjects), capacity: 36, rows: 6, horizontalGap: 8, verticalGap: 8, drawSlots: false)
      {
        capacity = 36
      };
      for (int index = 0; index < this.inventory.inventory.Count; ++index)
      {
        if (index >= this.inventory.actualInventory.Count)
          this.inventory.inventory[index].visible = false;
      }
      foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Bottom))
        clickableComponent.downNeighborID = -99998;
      foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
        clickableComponent.rightNeighborID = -99998;
      this.inventory.dropItemInvisibleButton.visible = false;
      Dictionary<string, string> bundleData = Game1.netWorldState.Value.BundleData;
      string areaNameFromNumber = CommunityCenter.getAreaNameFromNumber(whichArea);
      int whichBundle = 0;
      foreach (string key in bundleData.Keys)
      {
        if (key.Contains(areaNameFromNumber))
        {
          int int32 = Convert.ToInt32(key.Split('/')[1]);
          List<Bundle> bundles = this.bundles;
          Bundle bundle = new Bundle(int32, bundleData[key], bundlesComplete[int32], this.getBundleLocationFromNumber(whichBundle), "LooseSprites\\JunimoNote", this);
          bundle.myID = whichBundle + 5000;
          bundle.rightNeighborID = -7777;
          bundle.leftNeighborID = -7777;
          bundle.upNeighborID = -7777;
          bundle.downNeighborID = -7777;
          bundle.fullyImmutable = true;
          bundles.Add(bundle);
          ++whichBundle;
        }
      }
      ClickableTextureComponent textureComponent = new ClickableTextureComponent("Back", new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth * 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 4, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent.myID = 103;
      this.backButton = textureComponent;
      this.checkForRewards();
      JunimoNoteMenu.canClick = true;
      Game1.playSound("shwip");
      bool flag = false;
      foreach (Bundle bundle in this.bundles)
      {
        if (!bundle.complete && !bundle.Equals((object) this.currentPageBundle))
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).markAreaAsComplete(whichArea);
      this.exitFunction = new IClickableMenu.onExit(this.restoreAreaOnExit);
      ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).areaCompleteReward(whichArea);
    }

    public virtual bool HighlightObjects(Item item) => this.partialDonationItem != null && this.currentPageBundle != null && this.currentPartialIngredientDescriptionIndex >= 0 ? this.currentPageBundle.IsValidItemForThisIngredientDescription(item, this.currentPageBundle.ingredients[this.currentPartialIngredientDescriptionIndex]) : Utility.highlightSmallObjects(item);

    public override bool readyToClose() => !this.specificBundlePage && this.isReadyToCloseMenuOrBundle();

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!JunimoNoteMenu.canClick)
        return;
      base.receiveLeftClick(x, y, playSound);
      if (this.scrambledText)
        return;
      if (this.specificBundlePage)
      {
        if (!this.currentPageBundle.complete && this.currentPageBundle.completionTimer <= 0)
          this.heldItem = this.inventory.leftClick(x, y, this.heldItem);
        if (this.backButton.containsPoint(x, y) && this.heldItem == null)
          this.closeBundlePage();
        if (this.partialDonationItem != null)
        {
          if (this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
          {
            for (int index = 0; index < this.ingredientSlots.Count; ++index)
            {
              if (this.ingredientSlots[index].item == this.partialDonationItem)
                this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
            }
          }
          else
          {
            for (int index = 0; index < this.ingredientSlots.Count; ++index)
            {
              if (this.ingredientSlots[index].containsPoint(x, y) && this.ingredientSlots[index].item == this.partialDonationItem)
              {
                if (this.heldItem != null)
                {
                  this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
                  return;
                }
                this.ReturnPartialDonations(!Game1.oldKBState.IsKeyDown(Keys.LeftShift));
                return;
              }
            }
          }
        }
        else if (this.heldItem != null)
        {
          if (Game1.oldKBState.IsKeyDown(Keys.LeftShift))
          {
            for (int index = 0; index < this.ingredientSlots.Count; ++index)
            {
              if (this.currentPageBundle.canAcceptThisItem(this.heldItem, this.ingredientSlots[index]))
              {
                if (this.ingredientSlots[index].item == null)
                {
                  this.heldItem = this.currentPageBundle.tryToDepositThisItem(this.heldItem, this.ingredientSlots[index], "LooseSprites\\JunimoNote");
                  this.checkIfBundleIsComplete();
                  return;
                }
              }
              else if (this.ingredientSlots[index].item == null)
                this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
            }
          }
          for (int index = 0; index < this.ingredientSlots.Count; ++index)
          {
            if (this.ingredientSlots[index].containsPoint(x, y))
            {
              if (this.currentPageBundle.canAcceptThisItem(this.heldItem, this.ingredientSlots[index]))
              {
                this.heldItem = this.currentPageBundle.tryToDepositThisItem(this.heldItem, this.ingredientSlots[index], "LooseSprites\\JunimoNote");
                this.checkIfBundleIsComplete();
              }
              else if (this.ingredientSlots[index].item == null)
                this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
            }
          }
        }
        if (this.purchaseButton != null && this.purchaseButton.containsPoint(x, y))
        {
          int stack = this.currentPageBundle.ingredients.Last<BundleIngredientDescription>().stack;
          if (Game1.player.Money >= stack)
          {
            Game1.player.Money -= stack;
            Game1.playSound("select");
            this.currentPageBundle.completionAnimation(this);
            if (this.purchaseButton != null)
              this.purchaseButton.scale = this.purchaseButton.baseScale * 0.75f;
            ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundleRewards[this.currentPageBundle.bundleIndex] = true;
            (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundles.FieldDict[this.currentPageBundle.bundleIndex][0] = true;
            this.checkForRewards();
            bool flag = false;
            foreach (Bundle bundle in this.bundles)
            {
              if (!bundle.complete && !bundle.Equals((object) this.currentPageBundle))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).markAreaAsComplete(this.whichArea);
              this.exitFunction = new IClickableMenu.onExit(this.restoreAreaOnExit);
              ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).areaCompleteReward(this.whichArea);
            }
            else
              ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).getJunimoForArea(this.whichArea)?.bringBundleBackToHut(Bundle.getColorFromColorIndex(this.currentPageBundle.bundleColor), Game1.getLocationFromName("CommunityCenter"));
            Game1.multiplayer.globalChatInfoMessage("Bundle");
          }
          else
            Game1.dayTimeMoneyBox.moneyShakeTimer = 600;
        }
        if (this.upperRightCloseButton != null && this.isReadyToCloseMenuOrBundle() && this.upperRightCloseButton.containsPoint(x, y))
        {
          this.closeBundlePage();
          return;
        }
      }
      else
      {
        foreach (Bundle bundle in this.bundles)
        {
          if (bundle.canBeClicked() && bundle.containsPoint(x, y))
          {
            this.setUpBundleSpecificPage(bundle);
            Game1.playSound("shwip");
            return;
          }
        }
        if (this.presentButton != null && this.presentButton.containsPoint(x, y) && !this.fromGameMenu && !this.fromThisMenu)
          this.openRewardsMenu();
        if (this.fromGameMenu)
        {
          Game1.getLocationFromName("CommunityCenter");
          if (this.areaNextButton.containsPoint(x, y))
            this.SwapPage(1);
          else if (this.areaBackButton.containsPoint(x, y))
            this.SwapPage(-1);
        }
      }
      if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
        return;
      Game1.playSound("throwDownITem");
      Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
      this.heldItem = (Item) null;
    }

    public virtual void ReturnPartialDonation(Item item, bool play_sound = true)
    {
      List<Item> affected_items_list = new List<Item>();
      Item inventory = Game1.player.addItemToInventory(item, affected_items_list);
      foreach (Item obj in affected_items_list)
        this.inventory.ShakeItem(obj);
      if (inventory != null)
      {
        Utility.CollectOrDrop(inventory);
        this.inventory.ShakeItem(inventory);
      }
      if (!play_sound)
        return;
      Game1.playSound("coin");
    }

    public virtual void ReturnPartialDonations(bool to_hand = true)
    {
      if (this.partialDonationComponents.Count > 0)
      {
        bool play_sound = true;
        foreach (Item donationComponent in this.partialDonationComponents)
        {
          if (this.heldItem == null & to_hand)
          {
            Game1.playSound("dwop");
            this.heldItem = donationComponent;
          }
          else
          {
            this.ReturnPartialDonation(donationComponent, play_sound);
            play_sound = false;
          }
        }
      }
      this.ResetPartialDonation();
    }

    public virtual void ResetPartialDonation()
    {
      this.partialDonationComponents.Clear();
      this.currentPartialIngredientDescription = new BundleIngredientDescription?();
      this.currentPartialIngredientDescriptionIndex = -1;
      foreach (ClickableTextureComponent ingredientSlot in this.ingredientSlots)
      {
        if (ingredientSlot.item == this.partialDonationItem)
          ingredientSlot.item = (Item) null;
      }
      this.partialDonationItem = (Item) null;
    }

    public virtual bool CanBePartiallyOrFullyDonated(Item item)
    {
      if (this.currentPageBundle == null)
        return false;
      int descriptionIndexForItem = this.currentPageBundle.GetBundleIngredientDescriptionIndexForItem(item);
      if (descriptionIndexForItem < 0)
        return false;
      BundleIngredientDescription ingredient = this.currentPageBundle.ingredients[descriptionIndexForItem];
      int num = 0;
      if (this.currentPageBundle.IsValidItemForThisIngredientDescription(item, ingredient))
        num += item.Stack;
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Game1.player.items)
      {
        if (this.currentPageBundle.IsValidItemForThisIngredientDescription(obj, ingredient))
          num += obj.Stack;
      }
      if (descriptionIndexForItem == this.currentPartialIngredientDescriptionIndex && this.partialDonationItem != null)
        num += this.partialDonationItem.Stack;
      return num >= ingredient.stack;
    }

    public virtual void HandlePartialDonation(Item item, ClickableTextureComponent slot)
    {
      if (this.currentPageBundle != null && !this.currentPageBundle.depositsAllowed || this.partialDonationItem != null && slot.item != this.partialDonationItem || !this.CanBePartiallyOrFullyDonated(item))
        return;
      if (!this.currentPartialIngredientDescription.HasValue)
      {
        this.currentPartialIngredientDescriptionIndex = this.currentPageBundle.GetBundleIngredientDescriptionIndexForItem(item);
        if (this.currentPartialIngredientDescriptionIndex != -1)
          this.currentPartialIngredientDescription = new BundleIngredientDescription?(this.currentPageBundle.ingredients[this.currentPartialIngredientDescriptionIndex]);
      }
      if (!this.currentPartialIngredientDescription.HasValue || !this.currentPageBundle.IsValidItemForThisIngredientDescription(item, this.currentPartialIngredientDescription.Value))
        return;
      bool flag = true;
      int num;
      if (slot.item == null)
      {
        Game1.playSound("sell");
        flag = false;
        this.partialDonationItem = item.getOne();
        num = Math.Min(this.currentPartialIngredientDescription.Value.stack, item.Stack);
        this.partialDonationItem.Stack = num;
        item.Stack -= num;
        if (this.partialDonationItem is StardewValley.Object)
          (this.partialDonationItem as StardewValley.Object).Quality = this.currentPartialIngredientDescription.Value.quality;
        slot.item = this.partialDonationItem;
        slot.sourceRect.X = 512;
        slot.sourceRect.Y = 244;
      }
      else
      {
        num = Math.Min(this.currentPartialIngredientDescription.Value.stack - this.partialDonationItem.Stack, item.Stack);
        this.partialDonationItem.Stack += num;
        item.Stack -= num;
      }
      if (num > 0)
      {
        Item one = this.heldItem.getOne();
        one.Stack = num;
        foreach (Item donationComponent in this.partialDonationComponents)
        {
          if (donationComponent.canStackWith((ISalable) this.heldItem))
            one.Stack = donationComponent.addToStack(one);
        }
        if (one.Stack > 0)
          this.partialDonationComponents.Add(one);
        this.partialDonationComponents.Sort((Comparison<Item>) ((a, b) => b.Stack.CompareTo(a.Stack)));
      }
      if (item.Stack <= 0 && item == this.heldItem)
        this.heldItem = (Item) null;
      if (this.partialDonationItem.Stack >= this.currentPartialIngredientDescription.Value.stack)
      {
        slot.item = (Item) null;
        this.partialDonationItem = this.currentPageBundle.tryToDepositThisItem(this.partialDonationItem, slot, "LooseSprites\\JunimoNote");
        if (this.partialDonationItem != null && this.partialDonationItem.Stack > 0)
          this.ReturnPartialDonation(this.partialDonationItem);
        this.partialDonationItem = (Item) null;
        this.ResetPartialDonation();
        this.checkIfBundleIsComplete();
      }
      else
      {
        if (num <= 0 || !flag)
          return;
        Game1.playSound("sell");
      }
    }

    public bool isReadyToCloseMenuOrBundle() => (!this.specificBundlePage || this.currentPageBundle == null || this.currentPageBundle.completionTimer <= 0) && this.heldItem == null;

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (!this.fromGameMenu || this.specificBundlePage)
        return;
      Game1.getLocationFromName("CommunityCenter");
      if (b == Buttons.RightTrigger)
      {
        this.SwapPage(1);
      }
      else
      {
        if (b != Buttons.LeftTrigger)
          return;
        this.SwapPage(-1);
      }
    }

    public void SwapPage(int direction)
    {
      if (direction > 0 && !this.areaNextButton.visible || direction < 0 && !this.areaBackButton.visible)
        return;
      CommunityCenter locationFromName = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
      int whichArea = this.whichArea;
      int num1 = 6;
      for (int index = 0; index < num1; ++index)
      {
        whichArea += direction;
        if (whichArea < 0)
          whichArea += num1;
        if (whichArea >= num1)
          whichArea -= num1;
        if (locationFromName.shouldNoteAppearInArea(whichArea))
        {
          int num2 = -1;
          if (this.currentlySnappedComponent != null && (this.currentlySnappedComponent.myID >= 5000 || this.currentlySnappedComponent.myID == 101 || this.currentlySnappedComponent.myID == 102))
            num2 = this.currentlySnappedComponent.myID;
          JunimoNoteMenu junimoNoteMenu = new JunimoNoteMenu(true, whichArea, true);
          Game1.activeClickableMenu = (IClickableMenu) junimoNoteMenu;
          if (num2 >= 0)
          {
            junimoNoteMenu.currentlySnappedComponent = junimoNoteMenu.getComponentWithID(this.currentlySnappedComponent.myID);
            junimoNoteMenu.snapCursorToCurrentSnappedComponent();
          }
          if (junimoNoteMenu.getComponentWithID(this.areaNextButton.leftNeighborID) != null)
            junimoNoteMenu.areaNextButton.leftNeighborID = this.areaNextButton.leftNeighborID;
          else
            junimoNoteMenu.areaNextButton.leftNeighborID = junimoNoteMenu.areaBackButton.myID;
          junimoNoteMenu.areaNextButton.rightNeighborID = this.areaNextButton.rightNeighborID;
          junimoNoteMenu.areaNextButton.upNeighborID = this.areaNextButton.upNeighborID;
          junimoNoteMenu.areaNextButton.downNeighborID = this.areaNextButton.downNeighborID;
          if (junimoNoteMenu.getComponentWithID(this.areaBackButton.rightNeighborID) != null)
            junimoNoteMenu.areaBackButton.leftNeighborID = this.areaBackButton.leftNeighborID;
          else
            junimoNoteMenu.areaBackButton.leftNeighborID = junimoNoteMenu.areaNextButton.myID;
          junimoNoteMenu.areaBackButton.rightNeighborID = this.areaBackButton.rightNeighborID;
          junimoNoteMenu.areaBackButton.upNeighborID = this.areaBackButton.upNeighborID;
          junimoNoteMenu.areaBackButton.downNeighborID = this.areaBackButton.downNeighborID;
          break;
        }
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);
      if (key.Equals((object) Keys.Delete) && this.heldItem != null && this.heldItem.canBeTrashed())
      {
        Utility.trashItem(this.heldItem);
        this.heldItem = (Item) null;
      }
      if (!Game1.options.doesInputListContain(Game1.options.menuButton, key) || !this.isReadyToCloseMenuOrBundle())
        return;
      this.closeBundlePage();
    }

    private void closeBundlePage()
    {
      if (this.partialDonationItem != null)
      {
        this.ReturnPartialDonations(false);
      }
      else
      {
        if (!this.specificBundlePage)
          return;
        this.hoveredItem = (Item) null;
        this.inventory.descriptionText = "";
        if (this.heldItem == null)
        {
          this.takeDownBundleSpecificPage(this.currentPageBundle);
          Game1.playSound("shwip");
        }
        else
          this.heldItem = this.inventory.tryToAddItem(this.heldItem);
      }
    }

    private void reOpenThisMenu()
    {
      int num = this.specificBundlePage ? 1 : 0;
      JunimoNoteMenu junimoNoteMenu = this.fromGameMenu || this.fromThisMenu ? new JunimoNoteMenu(this.fromGameMenu, this.whichArea, this.fromThisMenu) : new JunimoNoteMenu(this.whichArea, ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundlesDict());
      if (num != 0)
      {
        foreach (Bundle bundle in junimoNoteMenu.bundles)
        {
          if (bundle.bundleIndex == this.currentPageBundle.bundleIndex)
          {
            junimoNoteMenu.setUpBundleSpecificPage(bundle);
            break;
          }
        }
      }
      Game1.activeClickableMenu = (IClickableMenu) junimoNoteMenu;
    }

    private void updateIngredientSlots()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      int index1 = 0;
      for (int index2 = 0; index2 < this.currentPageBundle.ingredients.Count; ++index2)
      {
        if (this.currentPageBundle.ingredients[index2].completed && index1 < this.ingredientSlots.Count)
        {
          int num = this.currentPageBundle.ingredients[index2].index;
          if (num < 0)
            num = JunimoNoteMenu.GetObjectOrCategoryIndex(num);
          this.ingredientSlots[index1].item = (Item) new StardewValley.Object(num, this.currentPageBundle.ingredients[index2].stack, quality: this.currentPageBundle.ingredients[index2].quality);
          this.currentPageBundle.ingredientDepositAnimation(this.ingredientSlots[index1], "LooseSprites\\JunimoNote", true);
          ++index1;
        }
      }
    }

    public static int GetObjectOrCategoryIndex(int category)
    {
      if (category < 0)
      {
        foreach (int key in (IEnumerable<int>) Game1.objectInformation.Keys)
        {
          string str = Game1.objectInformation[key];
          if (str != null)
          {
            string[] strArray = str.Split('/');
            if (strArray.Length > 3 && strArray[3].EndsWith(category.ToString()))
              return key;
          }
        }
      }
      return category;
    }

    public static void GetBundleRewards(int area, List<Item> rewards)
    {
      Dictionary<string, string> bundleData = Game1.netWorldState.Value.BundleData;
      foreach (string key in bundleData.Keys)
      {
        if (key.Contains(CommunityCenter.getAreaNameFromNumber(area)))
        {
          int int32 = Convert.ToInt32(key.Split('/')[1]);
          if (((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundleRewards[int32])
          {
            Item standardTextDescription = Utility.getItemFromStandardTextDescription(bundleData[key].Split('/')[1], Game1.player);
            standardTextDescription.SpecialVariable = int32;
            rewards.Add(standardTextDescription);
          }
        }
      }
    }

    private void openRewardsMenu()
    {
      Game1.playSound("smallSelect");
      List<Item> objList = new List<Item>();
      JunimoNoteMenu.GetBundleRewards(this.whichArea, objList);
      Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) objList, false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.rewardGrabbed), canBeExitedWithKey: true, context: ((object) this));
      Game1.activeClickableMenu.exitFunction = this.exitFunction != null ? this.exitFunction : new IClickableMenu.onExit(this.reOpenThisMenu);
    }

    private void rewardGrabbed(Item item, Farmer who) => ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundleRewards[item.SpecialVariable] = false;

    private void checkIfBundleIsComplete()
    {
      this.ReturnPartialDonations();
      if (!this.specificBundlePage || this.currentPageBundle == null)
        return;
      int num = 0;
      foreach (ClickableTextureComponent ingredientSlot in this.ingredientSlots)
      {
        if (ingredientSlot.item != null && ingredientSlot.item != this.partialDonationItem)
          ++num;
      }
      if (num < this.currentPageBundle.numberOfIngredientSlots)
        return;
      if (this.heldItem != null)
      {
        Game1.player.addItemToInventory(this.heldItem);
        this.heldItem = (Item) null;
      }
      for (int index = 0; index < ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundles[this.currentPageBundle.bundleIndex].Length; ++index)
        ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundles.FieldDict[this.currentPageBundle.bundleIndex][index] = true;
      ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).checkForNewJunimoNotes();
      JunimoNoteMenu.screenSwipe = new ScreenSwipe(0);
      this.currentPageBundle.completionAnimation(this, delay: 400);
      JunimoNoteMenu.canClick = false;
      ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundleRewards[this.currentPageBundle.bundleIndex] = true;
      Game1.multiplayer.globalChatInfoMessage("Bundle");
      bool flag = false;
      foreach (Bundle bundle in this.bundles)
      {
        if (!bundle.complete && !bundle.Equals((object) this.currentPageBundle))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        if (this.whichArea == 6)
        {
          this.exitFunction = new IClickableMenu.onExit(this.restoreaAreaOnExit_AbandonedJojaMart);
        }
        else
        {
          ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).markAreaAsComplete(this.whichArea);
          this.exitFunction = new IClickableMenu.onExit(this.restoreAreaOnExit);
          ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).areaCompleteReward(this.whichArea);
        }
      }
      else
        ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).getJunimoForArea(this.whichArea)?.bringBundleBackToHut(Bundle.getColorFromColorIndex(this.currentPageBundle.bundleColor), Game1.getLocationFromName("CommunityCenter"));
      this.checkForRewards();
    }

    private void restoreaAreaOnExit_AbandonedJojaMart() => ((AbandonedJojaMart) Game1.getLocationFromName("AbandonedJojaMart")).restoreAreaCutscene();

    private void restoreAreaOnExit()
    {
      if (this.fromGameMenu)
        return;
      ((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).restoreAreaCutscene(this.whichArea);
    }

    public void checkForRewards()
    {
      Dictionary<string, string> bundleData = Game1.netWorldState.Value.BundleData;
      foreach (string key in bundleData.Keys)
      {
        if (key.Contains(CommunityCenter.getAreaNameFromNumber(this.whichArea)) && bundleData[key].Split('/')[1].Length > 1)
        {
          int int32 = Convert.ToInt32(key.Split('/')[1]);
          if (((CommunityCenter) Game1.getLocationFromName("CommunityCenter")).bundleRewards[int32])
          {
            this.presentButton = new ClickableAnimatedComponent(new Rectangle(this.xPositionOnScreen + 592, this.yPositionOnScreen + 512, 72, 72), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10783"), new TemporaryAnimatedSprite("LooseSprites\\JunimoNote", new Rectangle(548, 262, 18, 20), 70f, 4, 99999, new Vector2(-64f, -64f), false, false, 0.5f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true));
            break;
          }
        }
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (!JunimoNoteMenu.canClick)
        return;
      if (this.specificBundlePage)
      {
        this.heldItem = this.inventory.rightClick(x, y, this.heldItem);
        if (this.partialDonationItem != null)
        {
          for (int index = 0; index < this.ingredientSlots.Count; ++index)
          {
            if (this.ingredientSlots[index].containsPoint(x, y) && this.ingredientSlots[index].item == this.partialDonationItem)
            {
              if (this.partialDonationComponents.Count > 0)
              {
                Item one = this.partialDonationComponents[0].getOne();
                bool flag = false;
                if (this.heldItem == null)
                {
                  this.heldItem = one;
                  Game1.playSound("dwop");
                  flag = true;
                }
                else if (this.heldItem.canStackWith((ISalable) one))
                {
                  this.heldItem.addToStack(one);
                  Game1.playSound("dwop");
                  flag = true;
                }
                if (flag)
                {
                  --this.partialDonationComponents[0].Stack;
                  if (this.partialDonationComponents[0].Stack <= 0)
                    this.partialDonationComponents.RemoveAt(0);
                  int num = 0;
                  foreach (Item donationComponent in this.partialDonationComponents)
                    num += donationComponent.Stack;
                  if (this.partialDonationItem != null)
                    this.partialDonationItem.Stack = num;
                  if (this.partialDonationComponents.Count == 0)
                  {
                    this.ResetPartialDonation();
                    break;
                  }
                  break;
                }
                break;
              }
              break;
            }
          }
        }
      }
      if (this.specificBundlePage || !this.isReadyToCloseMenuOrBundle())
        return;
      this.exitThisMenu();
    }

    public override void update(GameTime time)
    {
      if (this.specificBundlePage && this.currentPageBundle != null && this.currentPageBundle.completionTimer <= 0 && this.isReadyToCloseMenuOrBundle() && this.currentPageBundle.complete)
        this.takeDownBundleSpecificPage(this.currentPageBundle);
      foreach (Bundle bundle in this.bundles)
        bundle.update(time);
      for (int index = JunimoNoteMenu.tempSprites.Count - 1; index >= 0; --index)
      {
        if (JunimoNoteMenu.tempSprites[index].update(time))
          JunimoNoteMenu.tempSprites.RemoveAt(index);
      }
      if (this.presentButton != null)
        this.presentButton.update(time);
      if (JunimoNoteMenu.screenSwipe != null)
      {
        JunimoNoteMenu.canClick = false;
        if (JunimoNoteMenu.screenSwipe.update(time))
        {
          JunimoNoteMenu.screenSwipe = (ScreenSwipe) null;
          JunimoNoteMenu.canClick = true;
        }
      }
      if (!this.bundlesChanged || !this.fromGameMenu)
        return;
      this.reOpenThisMenu();
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      if (this.scrambledText)
        return;
      JunimoNoteMenu.hoverText = "";
      if (this.specificBundlePage)
      {
        this.backButton.tryHover(x, y);
        this.hoveredItem = this.currentPageBundle.complete || this.currentPageBundle.completionTimer > 0 ? (Item) null : this.inventory.hover(x, y, this.heldItem);
        foreach (ClickableTextureComponent ingredient in this.ingredientList)
        {
          if (ingredient.bounds.Contains(x, y))
          {
            JunimoNoteMenu.hoverText = ingredient.hoverText;
            break;
          }
        }
        if (this.heldItem != null)
        {
          foreach (ClickableTextureComponent ingredientSlot in this.ingredientSlots)
          {
            if (ingredientSlot.bounds.Contains(x, y) && this.CanBePartiallyOrFullyDonated(this.heldItem) && (this.partialDonationItem == null || ingredientSlot.item == this.partialDonationItem))
            {
              ingredientSlot.sourceRect.X = 530;
              ingredientSlot.sourceRect.Y = 262;
            }
            else
            {
              ingredientSlot.sourceRect.X = 512;
              ingredientSlot.sourceRect.Y = 244;
            }
          }
        }
        if (this.purchaseButton == null)
          return;
        this.purchaseButton.tryHover(x, y);
      }
      else
      {
        if (this.presentButton != null)
          JunimoNoteMenu.hoverText = this.presentButton.tryHover(x, y);
        foreach (Bundle bundle in this.bundles)
          bundle.tryHoverAction(x, y);
        if (!this.fromGameMenu)
          return;
        Game1.getLocationFromName("CommunityCenter");
        this.areaNextButton.tryHover(x, y);
        this.areaBackButton.tryHover(x, y);
      }
    }

    public override void draw(SpriteBatch b)
    {
      if (Game1.options.showMenuBackground)
        this.drawBackground(b);
      else
        b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
      if (!this.specificBundlePage)
      {
        b.Draw(this.noteTexture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(0, 0, 320, 180)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
        SpriteText.drawStringHorizontallyCenteredAt(b, this.scrambledText ? CommunityCenter.getAreaEnglishDisplayNameFromNumber(this.whichArea) : CommunityCenter.getAreaDisplayNameFromNumber(this.whichArea), this.xPositionOnScreen + this.width / 2 + 16, this.yPositionOnScreen + 12, height: 99999, alpha: 0.88f, junimoText: this.scrambledText);
        if (this.scrambledText)
        {
          SpriteText.drawString(b, LocalizedContentManager.CurrentLanguageLatin ? Game1.content.LoadString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10786") : Game1.content.LoadBaseString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10786"), this.xPositionOnScreen + 96, this.yPositionOnScreen + 96, width: (this.width - 192), height: 99999, alpha: 0.88f, junimoText: true);
          base.draw(b);
          if (Game1.options.SnappyMenus || !JunimoNoteMenu.canClick)
            return;
          this.drawMouse(b);
          return;
        }
        foreach (Bundle bundle in this.bundles)
          bundle.draw(b);
        if (this.presentButton != null)
          this.presentButton.draw(b);
        foreach (TemporaryAnimatedSprite tempSprite in JunimoNoteMenu.tempSprites)
          tempSprite.draw(b, true);
        if (this.fromGameMenu)
        {
          if (this.areaNextButton.visible)
            this.areaNextButton.draw(b);
          if (this.areaBackButton.visible)
            this.areaBackButton.draw(b);
        }
      }
      else
      {
        b.Draw(this.noteTexture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(320, 0, 320, 180)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
        if (this.currentPageBundle != null)
        {
          int num1 = this.currentPageBundle.bundleIndex;
          Texture2D texture = this.noteTexture;
          int num2 = 180;
          if (this.currentPageBundle.bundleTextureIndexOverride >= 0)
            num1 = this.currentPageBundle.bundleTextureIndexOverride;
          if (this.currentPageBundle.bundleTextureOverride != null)
          {
            texture = this.currentPageBundle.bundleTextureOverride;
            num2 = 0;
          }
          b.Draw(texture, new Vector2((float) (this.xPositionOnScreen + 872), (float) (this.yPositionOnScreen + 88)), new Rectangle?(new Rectangle(num1 * 16 * 2 % texture.Width, num2 + 32 * (num1 * 16 * 2 / texture.Width), 32, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.15f);
          float x = Game1.dialogueFont.MeasureString(!Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", (object) this.currentPageBundle.label)).X;
          b.Draw(this.noteTexture, new Vector2((float) (this.xPositionOnScreen + 936 - (int) x / 2 - 16), (float) (this.yPositionOnScreen + 228)), new Rectangle?(new Rectangle(517, 266, 4, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
          b.Draw(this.noteTexture, new Rectangle(this.xPositionOnScreen + 936 - (int) x / 2, this.yPositionOnScreen + 228, (int) x, 68), new Rectangle?(new Rectangle(520, 266, 1, 17)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
          b.Draw(this.noteTexture, new Vector2((float) (this.xPositionOnScreen + 936 + (int) x / 2), (float) (this.yPositionOnScreen + 228)), new Rectangle?(new Rectangle(524, 266, 4, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
          b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", (object) this.currentPageBundle.label), new Vector2((float) (this.xPositionOnScreen + 936) - x / 2f, (float) (this.yPositionOnScreen + 236)) + new Vector2(2f, 2f), Game1.textShadowColor);
          b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", (object) this.currentPageBundle.label), new Vector2((float) (this.xPositionOnScreen + 936) - x / 2f, (float) (this.yPositionOnScreen + 236)) + new Vector2(0.0f, 2f), Game1.textShadowColor);
          b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", (object) this.currentPageBundle.label), new Vector2((float) (this.xPositionOnScreen + 936) - x / 2f, (float) (this.yPositionOnScreen + 236)) + new Vector2(2f, 0.0f), Game1.textShadowColor);
          b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", (object) this.currentPageBundle.label), new Vector2((float) (this.xPositionOnScreen + 936) - x / 2f, (float) (this.yPositionOnScreen + 236)), Game1.textColor * 0.9f);
        }
        this.backButton.draw(b);
        if (this.purchaseButton != null)
        {
          this.purchaseButton.draw(b);
          Game1.dayTimeMoneyBox.drawMoneyBox(b);
        }
        float extraAlpha = 1f;
        if (this.partialDonationItem != null)
          extraAlpha = 0.25f;
        foreach (TemporaryAnimatedSprite tempSprite in JunimoNoteMenu.tempSprites)
          tempSprite.draw(b, true, extraAlpha: extraAlpha);
        foreach (ClickableTextureComponent ingredientSlot in this.ingredientSlots)
        {
          float alpha = 1f;
          if (this.partialDonationItem != null && ingredientSlot.item != this.partialDonationItem)
            alpha = 0.25f;
          if (ingredientSlot.item == null || this.partialDonationItem != null && ingredientSlot.item == this.partialDonationItem)
            ingredientSlot.draw(b, (this.fromGameMenu ? Color.LightGray * 0.5f : Color.White) * alpha, 0.89f);
          ingredientSlot.drawItem(b, 4, 4, alpha);
        }
        for (int index = 0; index < this.ingredientList.Count; ++index)
        {
          float num = 1f;
          if (this.currentPartialIngredientDescriptionIndex >= 0 && this.currentPartialIngredientDescriptionIndex != index)
            num = 0.25f;
          ClickableTextureComponent ingredient = this.ingredientList[index];
          bool flag = false;
          int bundleColor = this.currentPageBundle.bundleColor;
          if (this.currentPageBundle != null && this.currentPageBundle.ingredients != null && index < this.currentPageBundle.ingredients.Count && this.currentPageBundle.ingredients[index].completed)
            flag = true;
          if (!flag)
            b.Draw(Game1.shadowTexture, new Vector2((float) (ingredient.bounds.Center.X - Game1.shadowTexture.Bounds.Width * 4 / 2 - 4), (float) (ingredient.bounds.Center.Y + 4)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * num, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
          if (ingredient.item != null && ingredient.visible)
            ingredient.item.drawInMenu(b, new Vector2((float) ingredient.bounds.X, (float) ingredient.bounds.Y), ingredient.scale / 4f, 1f, 0.9f, StackDrawType.Draw, Color.White * (flag ? 0.25f : num), false);
        }
        this.inventory.draw(b);
      }
      SpriteText.drawStringWithScrollCenteredAt(b, this.getRewardNameForArea(this.whichArea), this.xPositionOnScreen + this.width / 2, Math.Min(this.yPositionOnScreen + this.height + 20, Game1.uiViewport.Height - 64 - 8));
      base.draw(b);
      Game1.mouseCursorTransparency = 1f;
      if (JunimoNoteMenu.canClick)
        this.drawMouse(b);
      if (this.heldItem != null)
        this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16), (float) (Game1.getOldMouseY() + 16)), 1f);
      if (this.inventory.descriptionText.Length > 0)
      {
        if (this.hoveredItem != null)
          IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem);
      }
      else
        IClickableMenu.drawHoverText(b, Game1.player.hasOrWillReceiveMail("canReadJunimoText") || JunimoNoteMenu.hoverText.Length <= 0 ? JunimoNoteMenu.hoverText : "???", Game1.dialogueFont);
      if (JunimoNoteMenu.screenSwipe == null)
        return;
      JunimoNoteMenu.screenSwipe.draw(b);
    }

    public string getRewardNameForArea(int whichArea)
    {
      switch (whichArea)
      {
        case 0:
          return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardPantry");
        case 1:
          return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardCrafts");
        case 2:
          return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardFishTank");
        case 3:
          return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardBoiler");
        case 4:
          return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardVault");
        case 5:
          return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardBulletin");
        default:
          return "???";
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - 640;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - 360;
      this.backButton = new ClickableTextureComponent("Back", new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth * 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 4, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      if (this.fromGameMenu)
      {
        ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 128, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
        textureComponent1.visible = false;
        this.areaNextButton = textureComponent1;
        ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
        textureComponent2.visible = false;
        this.areaBackButton = textureComponent2;
      }
      this.inventory = new InventoryMenu(this.xPositionOnScreen + 128, this.yPositionOnScreen + 140, true, highlightMethod: new InventoryMenu.highlightThisItem(this.HighlightObjects), capacity: ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems), rows: 6, horizontalGap: 8, verticalGap: 8, drawSlots: false);
      for (int index = 0; index < this.inventory.inventory.Count; ++index)
      {
        if (index >= this.inventory.actualInventory.Count)
          this.inventory.inventory[index].visible = false;
      }
      for (int index = 0; index < this.bundles.Count; ++index)
      {
        Point locationFromNumber = this.getBundleLocationFromNumber(index);
        this.bundles[index].bounds.X = locationFromNumber.X;
        this.bundles[index].bounds.Y = locationFromNumber.Y;
        this.bundles[index].sprite.position = new Vector2((float) locationFromNumber.X, (float) locationFromNumber.Y);
      }
      if (!this.specificBundlePage)
        return;
      int ofIngredientSlots = this.currentPageBundle.numberOfIngredientSlots;
      List<Rectangle> toAddTo1 = new List<Rectangle>();
      this.addRectangleRowsToList(toAddTo1, ofIngredientSlots, 932, 540);
      this.ingredientSlots.Clear();
      for (int index = 0; index < toAddTo1.Count; ++index)
        this.ingredientSlots.Add(new ClickableTextureComponent(toAddTo1[index], this.noteTexture, new Rectangle(512, 244, 18, 18), 4f));
      List<Rectangle> toAddTo2 = new List<Rectangle>();
      this.ingredientList.Clear();
      this.addRectangleRowsToList(toAddTo2, this.currentPageBundle.ingredients.Count, 932, 364);
      for (int index = 0; index < toAddTo2.Count; ++index)
      {
        if (Game1.objectInformation.ContainsKey(this.currentPageBundle.ingredients[index].index))
        {
          List<ClickableTextureComponent> ingredientList = this.ingredientList;
          ClickableTextureComponent textureComponent = new ClickableTextureComponent("", toAddTo2[index], "", Game1.objectInformation[this.currentPageBundle.ingredients[index].index].Split('/')[0], Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.currentPageBundle.ingredients[index].index, 16, 16), 4f);
          textureComponent.myID = index + 1000;
          textureComponent.item = (Item) new StardewValley.Object(this.currentPageBundle.ingredients[index].index, this.currentPageBundle.ingredients[index].stack, quality: this.currentPageBundle.ingredients[index].quality);
          textureComponent.upNeighborID = -99998;
          textureComponent.rightNeighborID = -99998;
          textureComponent.leftNeighborID = -99998;
          textureComponent.downNeighborID = -99998;
          ingredientList.Add(textureComponent);
        }
      }
      this.updateIngredientSlots();
    }

    private void setUpBundleSpecificPage(Bundle b)
    {
      JunimoNoteMenu.tempSprites.Clear();
      this.currentPageBundle = b;
      this.specificBundlePage = true;
      if (this.whichArea == 4)
      {
        if (this.fromGameMenu)
          return;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 800, this.yPositionOnScreen + 504, 260, 72), this.noteTexture, new Rectangle(517, 286, 65, 20), 4f);
        textureComponent.myID = 797;
        textureComponent.leftNeighborID = 103;
        this.purchaseButton = textureComponent;
        if (!Game1.options.SnappyMenus)
          return;
        this.currentlySnappedComponent = (ClickableComponent) this.purchaseButton;
        this.snapCursorToCurrentSnappedComponent();
      }
      else
      {
        int ofIngredientSlots = b.numberOfIngredientSlots;
        List<Rectangle> toAddTo1 = new List<Rectangle>();
        this.addRectangleRowsToList(toAddTo1, ofIngredientSlots, 932, 540);
        for (int index = 0; index < toAddTo1.Count; ++index)
        {
          List<ClickableTextureComponent> ingredientSlots = this.ingredientSlots;
          ClickableTextureComponent textureComponent = new ClickableTextureComponent(toAddTo1[index], this.noteTexture, new Rectangle(512, 244, 18, 18), 4f);
          textureComponent.myID = index + 250;
          textureComponent.upNeighborID = -99998;
          textureComponent.rightNeighborID = -99998;
          textureComponent.leftNeighborID = -99998;
          textureComponent.downNeighborID = -99998;
          ingredientSlots.Add(textureComponent);
        }
        List<Rectangle> toAddTo2 = new List<Rectangle>();
        this.addRectangleRowsToList(toAddTo2, b.ingredients.Count, 932, 364);
        for (int index = 0; index < toAddTo2.Count; ++index)
        {
          int num = b.ingredients[index].index;
          if (num < 0)
            num = JunimoNoteMenu.GetObjectOrCategoryIndex(num);
          if (Game1.objectInformation.ContainsKey(num))
          {
            string hoverText = Game1.objectInformation[num].Split('/')[4];
            if (b.ingredients[index].index < 0)
            {
              if (b.ingredients[index].index == -2)
                hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
              else if (b.ingredients[index].index == -75)
                hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");
              else if (b.ingredients[index].index == -4)
                hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
              else if (b.ingredients[index].index == -5)
                hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
              else if (b.ingredients[index].index == -6)
                hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
            }
            List<ClickableTextureComponent> ingredientList = this.ingredientList;
            ClickableTextureComponent textureComponent = new ClickableTextureComponent("ingredient_list_slot", toAddTo2[index], "", hoverText, Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, b.ingredients[index].index, 16, 16), 4f);
            textureComponent.myID = index + 1000;
            textureComponent.item = (Item) new StardewValley.Object(num, b.ingredients[index].stack, quality: b.ingredients[index].quality);
            textureComponent.upNeighborID = -99998;
            textureComponent.rightNeighborID = -99998;
            textureComponent.leftNeighborID = -99998;
            textureComponent.downNeighborID = -99998;
            ingredientList.Add(textureComponent);
          }
        }
        this.updateIngredientSlots();
        if (!Game1.options.SnappyMenus)
          return;
        this.populateClickableComponentList();
        if (this.inventory != null && this.inventory.inventory != null)
        {
          for (int index = 0; index < this.inventory.inventory.Count; ++index)
          {
            if (this.inventory.inventory[index] != null)
            {
              if (this.inventory.inventory[index].downNeighborID == 101)
                this.inventory.inventory[index].downNeighborID = -1;
              if (this.inventory.inventory[index].leftNeighborID == -1)
                this.inventory.inventory[index].leftNeighborID = 103;
              if (this.inventory.inventory[index].upNeighborID >= 1000)
                this.inventory.inventory[index].upNeighborID = 103;
            }
          }
        }
        this.currentlySnappedComponent = this.getComponentWithID(0);
        this.snapCursorToCurrentSnappedComponent();
      }
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return (this.currentPartialIngredientDescriptionIndex < 0 || (!((IEnumerable<ClickableComponent>) this.ingredientSlots).Contains<ClickableComponent>(b) || b.item == this.partialDonationItem) && (!((IEnumerable<ClickableComponent>) this.ingredientList).Contains<ClickableComponent>(b) || this.ingredientList.IndexOf(b as ClickableTextureComponent) == this.currentPartialIngredientDescriptionIndex)) && (a.myID >= 5000 || a.myID == 101 ? 1 : (a.myID == 102 ? 1 : 0)) == (b.myID >= 5000 || b.myID == 101 ? 1 : (b.myID == 102 ? 1 : 0));
    }

    private void addRectangleRowsToList(
      List<Rectangle> toAddTo,
      int numberOfItems,
      int centerX,
      int centerY)
    {
      switch (numberOfItems)
      {
        case 1:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 1, 72, 72, 12));
          break;
        case 2:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 2, 72, 72, 12));
          break;
        case 3:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 3, 72, 72, 12));
          break;
        case 4:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 4, 72, 72, 12));
          break;
        case 5:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 3, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 2, 72, 72, 12));
          break;
        case 6:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 3, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 3, 72, 72, 12));
          break;
        case 7:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 4, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 3, 72, 72, 12));
          break;
        case 8:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 4, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 4, 72, 72, 12));
          break;
        case 9:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 5, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 4, 72, 72, 12));
          break;
        case 10:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 5, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 5, 72, 72, 12));
          break;
        case 11:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 6, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 5, 72, 72, 12));
          break;
        case 12:
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 6, 72, 72, 12));
          toAddTo.AddRange((IEnumerable<Rectangle>) this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 6, 72, 72, 12));
          break;
      }
    }

    private List<Rectangle> createRowOfBoxesCenteredAt(
      int xStart,
      int yStart,
      int numBoxes,
      int boxWidth,
      int boxHeight,
      int horizontalGap)
    {
      List<Rectangle> ofBoxesCenteredAt = new List<Rectangle>();
      int num = xStart - numBoxes * (boxWidth + horizontalGap) / 2;
      int y = yStart - boxHeight / 2;
      for (int index = 0; index < numBoxes; ++index)
        ofBoxesCenteredAt.Add(new Rectangle(num + index * (boxWidth + horizontalGap), y, boxWidth, boxHeight));
      return ofBoxesCenteredAt;
    }

    public void takeDownBundleSpecificPage(Bundle b = null)
    {
      if (!this.isReadyToCloseMenuOrBundle())
        return;
      this.ReturnPartialDonations(false);
      this.hoveredItem = (Item) null;
      if (!this.specificBundlePage)
        return;
      if (b == null)
        b = this.currentPageBundle;
      this.specificBundlePage = false;
      this.ingredientSlots.Clear();
      this.ingredientList.Clear();
      JunimoNoteMenu.tempSprites.Clear();
      this.purchaseButton = (ClickableTextureComponent) null;
      if (!Game1.options.SnappyMenus)
        return;
      if (this.currentPageBundle != null)
      {
        this.currentlySnappedComponent = (ClickableComponent) this.currentPageBundle;
        this.snapCursorToCurrentSnappedComponent();
      }
      else
        this.snapToDefaultClickableComponent();
    }

    private Point getBundleLocationFromNumber(int whichBundle)
    {
      Point locationFromNumber = new Point(this.xPositionOnScreen, this.yPositionOnScreen);
      switch (whichBundle)
      {
        case 0:
          locationFromNumber.X += 592;
          locationFromNumber.Y += 136;
          break;
        case 1:
          locationFromNumber.X += 392;
          locationFromNumber.Y += 384;
          break;
        case 2:
          locationFromNumber.X += 784;
          locationFromNumber.Y += 388;
          break;
        case 3:
          locationFromNumber.X += 304;
          locationFromNumber.Y += 252;
          break;
        case 4:
          locationFromNumber.X += 892;
          locationFromNumber.Y += 252;
          break;
        case 5:
          locationFromNumber.X += 588;
          locationFromNumber.Y += 276;
          break;
        case 6:
          locationFromNumber.X += 588;
          locationFromNumber.Y += 380;
          break;
        case 7:
          locationFromNumber.X += 440;
          locationFromNumber.Y += 164;
          break;
        case 8:
          locationFromNumber.X += 776;
          locationFromNumber.Y += 164;
          break;
      }
      return locationFromNumber;
    }
  }
}
