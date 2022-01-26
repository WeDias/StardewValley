// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsPage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class OptionsPage : IClickableMenu
  {
    public const int itemsPerPage = 7;
    private string descriptionText = "";
    private string hoverText = "";
    public List<ClickableComponent> optionSlots = new List<ClickableComponent>();
    public int currentItemIndex;
    private ClickableTextureComponent upArrow;
    private ClickableTextureComponent downArrow;
    private ClickableTextureComponent scrollBar;
    private bool scrolling;
    public List<OptionsElement> options = new List<OptionsElement>();
    private Rectangle scrollBarRunner;
    protected static int _lastSelectedIndex;
    protected static int _lastCurrentItemIndex;
    public int lastRebindTick = -1;
    private int optionsSlotHeld = -1;

    public OptionsPage(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
      this.upArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width + 16, this.yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
      this.downArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width + 16, this.yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
      this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, height - 128 - this.upArrow.bounds.Height - 8);
      for (int index1 = 0; index1 < 7; ++index1)
        this.optionSlots.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 80 + 4 + index1 * ((height - 128) / 7) + 16, width - 32, (height - 128) / 7 + 4), index1.ToString() ?? "")
        {
          myID = index1,
          downNeighborID = index1 < 6 ? index1 + 1 : -7777,
          upNeighborID = index1 > 0 ? index1 - 1 : -7777,
          fullyImmutable = true
        });
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11233")));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11234"), 0));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11235"), 7));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11236"), 8));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11237"), 11));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11238"), 12));
      if (Game1.game1.IsMainInstance)
        this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\UI:Options_GamepadMode"), 38));
      this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\UI:Options_StowingMode"), 28));
      this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\UI:Options_SlingshotMode"), 41));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11239"), 27));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11240"), 14));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\UI:Options_GamepadStyleMenus"), 29));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\UI:Options_ShowAdvancedCraftingInformation"), 34));
      bool flag = false;
      if (Game1.game1.IsMainInstance && Game1.game1.IsLocalCoopJoinable())
        flag = true;
      if (Game1.multiplayerMode == (byte) 2 | flag)
        this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:OptionsPage_MultiplayerSection")));
      if (Game1.multiplayerMode == (byte) 2 && Game1.server != null && !Game1.server.IsLocalMultiplayerInitiatedServer())
      {
        this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\UI:GameMenu_ServerMode"), 31));
        this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\UI:OptionsPage_IPConnections"), 30));
        this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\UI:OptionsPage_FarmhandCreation"), 32));
      }
      if (Game1.multiplayerMode == (byte) 2 && Game1.server != null)
        this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions"), 40));
      if (Game1.multiplayerMode == (byte) 2 && Game1.server != null && !Game1.server.IsLocalMultiplayerInitiatedServer() && Program.sdk.Networking != null)
      {
        this.options.Add((OptionsElement) new OptionsButton(Game1.content.LoadString("Strings\\UI:GameMenu_ServerInvite"), (Action) (() => this.offerInvite())));
        if (Program.sdk.Networking.SupportsInviteCodes())
          this.options.Add((OptionsElement) new OptionsButton(Game1.content.LoadString("Strings\\UI:OptionsPage_ShowInviteCode"), (Action) (() => this.showInviteCode())));
      }
      if (flag)
        this.options.Add((OptionsElement) new OptionsButton(Game1.content.LoadString("Strings\\UI:StartLocalMulti"), (Action) (() =>
        {
          this.exitThisMenu(false);
          Game1.game1.ShowLocalCoopJoinMenu();
        })));
      if (Game1.IsMultiplayer)
        this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\UI:OptionsPage_ShowReadyStatus"), 35));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11241")));
      if (Game1.game1.IsMainInstance)
      {
        this.options.Add((OptionsElement) new OptionsSlider(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11242"), 1));
        this.options.Add((OptionsElement) new OptionsSlider(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11243"), 2));
        this.options.Add((OptionsElement) new OptionsSlider(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11244"), 20));
        this.options.Add((OptionsElement) new OptionsSlider(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11245"), 21));
      }
      this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\StringsFromCSFiles:BiteChime"), 42));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11246"), 3));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:Options_ToggleAnimalSounds"), 43));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11247")));
      if (!Game1.conventionMode && Game1.game1.IsMainInstance)
      {
        this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11248"), 13));
        this.options.Add((OptionsElement) new OptionsDropDown(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11251"), 6));
      }
      if (Game1.game1.IsMainInstance)
        this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\UI:Options_Vsync"), 37));
      List<string> stringList1 = new List<string>();
      for (int index2 = 75; index2 <= 150; index2 += 5)
        stringList1.Add(index2.ToString() + "%");
      this.options.Add((OptionsElement) new OptionsPlusMinus(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage_UIScale"), 39, stringList1, stringList1));
      List<string> stringList2 = new List<string>();
      for (int index3 = 75; index3 <= 200; index3 += 5)
        stringList2.Add(index3.ToString() + "%");
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11252"), 9));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11253"), 15));
      this.options.Add((OptionsElement) new OptionsPlusMinus(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11254"), 18, stringList2, stringList2));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11266"), 19));
      if (Game1.game1.IsMainInstance)
        this.options.Add((OptionsElement) new OptionsPlusMinus(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11267"), 25, new List<string>()
        {
          "Low",
          "Med.",
          "High"
        }, new List<string>()
        {
          Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11268"),
          Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11269"),
          Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11270")
        }));
      this.options.Add((OptionsElement) new OptionsSlider(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11271"), 23));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11272"), 24));
      if (!LocalMultiplayer.IsLocalMultiplayer())
        this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11273"), 26));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11274")));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11275"), 16));
      this.options.Add((OptionsElement) new OptionsCheckbox(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11276"), 22));
      if (Game1.game1.IsMainInstance)
      {
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11277"), -1, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11278"), 7, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11279"), 10, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11280"), 15, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11281"), 18, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11282"), 19, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11283"), 11, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11284"), 14, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11285"), 13, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11286"), 12, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11287"), 17, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\UI:Input_EmoteButton"), 33, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11288"), 16, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.toolbarSwap"), 32, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11289"), 20, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11290"), 21, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11291"), 22, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11292"), 23, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11293"), 24, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11294"), 25, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11295"), 26, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11296"), 27, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11297"), 28, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11298"), 29, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11299"), 30, this.optionSlots[0].bounds.Width));
        this.options.Add((OptionsElement) new OptionsInputListener(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11300"), 31, this.optionSlots[0].bounds.Width));
      }
      if (!Game1.game1.CanTakeScreenshots())
        return;
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:OptionsPage_ScreenshotHeader")));
      int index = this.options.Count;
      if (!Game1.game1.CanZoomScreenshots())
      {
        Action action = (Action) (() =>
        {
          OptionsElement e = this.options[index];
          Game1.flashAlpha = 1f;
          Console.WriteLine("{0}.greyedOut = {1}", (object) e.label, (object) true);
          e.greyedOut = true;
          Action onDone = (Action) (() =>
          {
            Console.WriteLine("{0}.greyedOut = {1}", (object) e.label, (object) false);
            e.greyedOut = false;
          });
          string mapScreenshot = Game1.game1.takeMapScreenshot(new float?(), (string) null, onDone);
          if (mapScreenshot != null)
            Game1.addHUDMessage(new HUDMessage(mapScreenshot, 6));
          Game1.playSound("cameraNoise");
        });
        OptionsButton optionsButton = new OptionsButton(Game1.content.LoadString("Strings\\UI:OptionsPage_ScreenshotHeader").Replace(":", ""), action);
        if (Game1.game1.ScreenshotBusy)
          optionsButton.greyedOut = true;
        this.options.Add((OptionsElement) optionsButton);
      }
      else
      {
        List<OptionsElement> options1 = this.options;
        string label = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11254");
        List<string> options2 = new List<string>();
        options2.Add("25%");
        options2.Add("50%");
        options2.Add("75%");
        options2.Add("100%");
        List<string> displayOptions = new List<string>();
        displayOptions.Add("25%");
        displayOptions.Add("50%");
        displayOptions.Add("75%");
        displayOptions.Add("100%");
        Texture2D mouseCursors2 = Game1.mouseCursors2;
        Rectangle buttonRect = new Rectangle(72, 31, 18, 16);
        OptionsPlusMinusButton optionsPlusMinusButton = new OptionsPlusMinusButton(label, 36, options2, displayOptions, mouseCursors2, buttonRect, (Action<string>) (selection =>
        {
          Game1.flashAlpha = 1f;
          selection = selection.Substring(0, selection.Length - 1);
          int result = 25;
          if (!int.TryParse(selection, out result))
            result = 25;
          string mapScreenshot = Game1.game1.takeMapScreenshot(new float?((float) result / 100f), (string) null, (Action) null);
          if (mapScreenshot != null)
            Game1.addHUDMessage(new HUDMessage(mapScreenshot, 6));
          Game1.playSound("cameraNoise");
        }));
        options1.Add((OptionsElement) optionsPlusMinusButton);
      }
      if (!Game1.game1.CanBrowseScreenshots())
        return;
      this.options.Add((OptionsElement) new OptionsButton(Game1.content.LoadString("Strings\\UI:OptionsPage_OpenFolder"), (Action) (() => Game1.game1.BrowseScreenshots())));
    }

    public override bool readyToClose() => this.lastRebindTick != Game1.ticks && base.readyToClose();

    private void waitForServerConnection(Action onConnection)
    {
      if (Game1.server == null)
        return;
      if (Game1.server.connected())
      {
        onConnection();
      }
      else
      {
        IClickableMenu thisMenu = Game1.activeClickableMenu;
        ConfirmationDialog.behavior onClose = (ConfirmationDialog.behavior) (who =>
        {
          Game1.activeClickableMenu = thisMenu;
          thisMenu.snapCursorToCurrentSnappedComponent();
        });
        Game1.activeClickableMenu = (IClickableMenu) new ServerConnectionDialog((ConfirmationDialog.behavior) (who =>
        {
          onClose(who);
          onConnection();
        }), onClose);
      }
    }

    private void offerInvite() => this.waitForServerConnection((Action) (() => Game1.server.offerInvite()));

    private void showInviteCode()
    {
      IClickableMenu thisMenu = Game1.activeClickableMenu;
      this.waitForServerConnection((Action) (() =>
      {
        ConfirmationDialog.behavior onClose = (ConfirmationDialog.behavior) (who =>
        {
          Game1.activeClickableMenu = thisMenu;
          thisMenu.snapCursorToCurrentSnappedComponent();
        });
        Game1.activeClickableMenu = (IClickableMenu) new InviteCodeDialog(Game1.server.getInviteCode(), onClose);
      }));
    }

    public override void snapToDefaultClickableComponent()
    {
      base.snapToDefaultClickableComponent();
      this.currentlySnappedComponent = this.getComponentWithID(1);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void applyMovementKey(int direction)
    {
      if (this.IsDropdownActive())
      {
        if (this.optionsSlotHeld == -1 || this.optionsSlotHeld + this.currentItemIndex >= this.options.Count || !(this.options[this.currentItemIndex + this.optionsSlotHeld] is OptionsDropDown) || direction == 2)
          ;
      }
      else
        base.applyMovementKey(direction);
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      base.customSnapBehavior(direction, oldRegion, oldID);
      if (oldID == 6 && direction == 2 && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
      {
        this.downArrowPressed();
        Game1.playSound("shiny4");
      }
      else
      {
        if (oldID != 0 || direction != 0)
          return;
        if (this.currentItemIndex > 0)
        {
          this.upArrowPressed();
          Game1.playSound("shiny4");
        }
        else
        {
          this.currentlySnappedComponent = this.getComponentWithID(12346);
          if (this.currentlySnappedComponent != null)
            this.currentlySnappedComponent.downNeighborID = 0;
          this.snapCursorToCurrentSnappedComponent();
        }
      }
    }

    private void setScrollBarToCurrentIndex()
    {
      if (this.options.Count <= 0)
        return;
      this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.options.Count - 7 + 1) * this.currentItemIndex + this.upArrow.bounds.Bottom + 4;
      if (this.scrollBar.bounds.Y <= this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4)
        return;
      this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
    }

    public override void snapCursorToCurrentSnappedComponent()
    {
      if (this.currentlySnappedComponent != null && this.currentlySnappedComponent.myID < this.options.Count)
      {
        if (this.options[this.currentlySnappedComponent.myID + this.currentItemIndex] is OptionsDropDown option)
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + option.bounds.Right - 32, this.currentlySnappedComponent.bounds.Center.Y - 4);
        else if (this.options[this.currentlySnappedComponent.myID + this.currentItemIndex] is OptionsPlusMinusButton)
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 64, this.currentlySnappedComponent.bounds.Center.Y + 4);
        else if (this.options[this.currentlySnappedComponent.myID + this.currentItemIndex] is OptionsInputListener)
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Right - 48, this.currentlySnappedComponent.bounds.Center.Y - 12);
        else
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 48, this.currentlySnappedComponent.bounds.Center.Y - 12);
      }
      else
      {
        if (this.currentlySnappedComponent == null)
          return;
        base.snapCursorToCurrentSnappedComponent();
      }
    }

    public override void leftClickHeld(int x, int y)
    {
      if (GameMenu.forcePreventClose)
        return;
      base.leftClickHeld(x, y);
      if (this.scrolling)
      {
        int y1 = this.scrollBar.bounds.Y;
        this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upArrow.bounds.Height + 20));
        this.currentItemIndex = Math.Min(this.options.Count - 7, Math.Max(0, (int) ((double) this.options.Count * (double) ((float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height))));
        this.setScrollBarToCurrentIndex();
        int y2 = this.scrollBar.bounds.Y;
        if (y1 == y2)
          return;
        Game1.playSound("shiny4");
      }
      else
      {
        if (this.optionsSlotHeld == -1 || this.optionsSlotHeld + this.currentItemIndex >= this.options.Count)
          return;
        this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickHeld(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
      }
    }

    public override ClickableComponent getCurrentlySnappedComponent() => this.currentlySnappedComponent;

    public override void setCurrentlySnappedComponentTo(int id)
    {
      this.currentlySnappedComponent = this.getComponentWithID(id);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveKeyPress(Keys key)
    {
      if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count || Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        if (this.currentlySnappedComponent != null && Game1.options.snappyMenus && Game1.options.gamepadControls && this.options.Count > this.currentItemIndex + this.currentlySnappedComponent.myID && this.currentItemIndex + this.currentlySnappedComponent.myID >= 0)
          this.options[this.currentItemIndex + this.currentlySnappedComponent.myID].receiveKeyPress(key);
        else if (this.options.Count > this.currentItemIndex + this.optionsSlotHeld && this.currentItemIndex + this.optionsSlotHeld >= 0)
          this.options[this.currentItemIndex + this.optionsSlotHeld].receiveKeyPress(key);
      }
      base.receiveKeyPress(key);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      if (GameMenu.forcePreventClose || this.IsDropdownActive())
        return;
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && this.currentItemIndex > 0)
      {
        this.upArrowPressed();
        Game1.playSound("shiny4");
      }
      else if (direction < 0 && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
      {
        this.downArrowPressed();
        Game1.playSound("shiny4");
      }
      if (!Game1.options.SnappyMenus)
        return;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void releaseLeftClick(int x, int y)
    {
      if (GameMenu.forcePreventClose)
        return;
      base.releaseLeftClick(x, y);
      if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count)
        this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickReleased(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
      this.optionsSlotHeld = -1;
      this.scrolling = false;
    }

    public bool IsDropdownActive() => this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count && this.options[this.currentItemIndex + this.optionsSlotHeld] is OptionsDropDown;

    private void downArrowPressed()
    {
      if (this.IsDropdownActive())
        return;
      this.UnsubscribeFromSelectedTextbox();
      this.downArrow.scale = this.downArrow.baseScale;
      ++this.currentItemIndex;
      this.setScrollBarToCurrentIndex();
    }

    public virtual void UnsubscribeFromSelectedTextbox()
    {
      if (Game1.keyboardDispatcher.Subscriber == null)
        return;
      foreach (OptionsElement option in this.options)
      {
        if (option is OptionsTextEntry && Game1.keyboardDispatcher.Subscriber == (option as OptionsTextEntry).textBox)
        {
          Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
          break;
        }
      }
    }

    public void preWindowSizeChange()
    {
      OptionsPage._lastSelectedIndex = this.getCurrentlySnappedComponent() != null ? this.getCurrentlySnappedComponent().myID : -1;
      OptionsPage._lastCurrentItemIndex = this.currentItemIndex;
    }

    public void postWindowSizeChange()
    {
      if (Game1.options.SnappyMenus)
        Game1.activeClickableMenu.setCurrentlySnappedComponentTo(OptionsPage._lastSelectedIndex);
      this.currentItemIndex = OptionsPage._lastCurrentItemIndex;
      this.setScrollBarToCurrentIndex();
    }

    private void upArrowPressed()
    {
      if (this.IsDropdownActive())
        return;
      this.UnsubscribeFromSelectedTextbox();
      this.upArrow.scale = this.upArrow.baseScale;
      --this.currentItemIndex;
      this.setScrollBarToCurrentIndex();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (GameMenu.forcePreventClose)
        return;
      if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
      {
        this.downArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
      {
        this.upArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.scrollBar.containsPoint(x, y))
        this.scrolling = true;
      else if (!this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
      {
        this.scrolling = true;
        this.leftClickHeld(x, y);
        this.releaseLeftClick(x, y);
      }
      this.currentItemIndex = Math.Max(0, Math.Min(this.options.Count - 7, this.currentItemIndex));
      this.UnsubscribeFromSelectedTextbox();
      for (int index = 0; index < this.optionSlots.Count; ++index)
      {
        if (this.optionSlots[index].bounds.Contains(x, y) && this.currentItemIndex + index < this.options.Count && this.options[this.currentItemIndex + index].bounds.Contains(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y))
        {
          this.options[this.currentItemIndex + index].receiveLeftClick(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y);
          this.optionsSlotHeld = index;
          break;
        }
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      for (int index = 0; index < this.optionSlots.Count; ++index)
      {
        if (this.currentItemIndex >= 0 && this.currentItemIndex + index < this.options.Count && this.options[this.currentItemIndex + index].bounds.Contains(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y))
        {
          Game1.SetFreeCursorDrag();
          break;
        }
      }
      if (this.scrollBarRunner.Contains(x, y))
        Game1.SetFreeCursorDrag();
      if (GameMenu.forcePreventClose)
        return;
      this.descriptionText = "";
      this.hoverText = "";
      this.upArrow.tryHover(x, y);
      this.downArrow.tryHover(x, y);
      this.scrollBar.tryHover(x, y);
      int num = this.scrolling ? 1 : 0;
    }

    public override void draw(SpriteBatch b)
    {
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      for (int index = 0; index < this.optionSlots.Count; ++index)
      {
        if (this.currentItemIndex >= 0 && this.currentItemIndex + index < this.options.Count)
          this.options[this.currentItemIndex + index].draw(b, this.optionSlots[index].bounds.X, this.optionSlots[index].bounds.Y, (IClickableMenu) this);
      }
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (!GameMenu.forcePreventClose)
      {
        this.upArrow.draw(b);
        this.downArrow.draw(b);
        if (this.options.Count > 7)
        {
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f, false);
          this.scrollBar.draw(b);
        }
      }
      if (this.hoverText.Equals(""))
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
  }
}
