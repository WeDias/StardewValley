// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.CoopMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.GameData;
using StardewValley.Network;
using StardewValley.SDKs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StardewValley.Menus
{
  public class CoopMenu : LoadGameMenu
  {
    public const int region_refresh = 810;
    public const int region_joinTab = 811;
    public const int region_hostTab = 812;
    public const int region_tabs = 1000;
    protected List<LoadGameMenu.MenuSlot> hostSlots = new List<LoadGameMenu.MenuSlot>();
    public ClickableComponent refreshButton;
    public ClickableComponent joinTab;
    public ClickableComponent hostTab;
    private LobbyUpdateListener lobbyUpdateListener;
    private CoopMenu.Tab currentTab;
    private bool smallScreenFormat;
    private bool isSetUp;
    private int updateCounter;
    private float _refreshDelay = -1f;
    private bool _tooManyFarms;
    public static string lastEnteredInviteCode;
    private StringBuilder _stringBuilder = new StringBuilder();

    public CoopMenu(bool tooManyFarms) => this._tooManyFarms = tooManyFarms;

    public override bool readyToClose() => !this.isSetUp || base.readyToClose();

    protected override bool hasDeleteButtons() => false;

    protected override List<LoadGameMenu.MenuSlot> MenuSlots
    {
      get
      {
        if (this.currentTab == CoopMenu.Tab.JOIN_TAB)
          return this.menuSlots;
        return this.currentTab == CoopMenu.Tab.HOST_TAB ? this.hostSlots : (List<LoadGameMenu.MenuSlot>) null;
      }
      set
      {
        if (this.currentTab == CoopMenu.Tab.JOIN_TAB)
        {
          this.menuSlots = value;
        }
        else
        {
          if (this.currentTab != CoopMenu.Tab.HOST_TAB)
            return;
          this.hostSlots = value;
        }
      }
    }

    protected override void startListPopulation()
    {
    }

    protected virtual void connectionFinished()
    {
      string str1 = Game1.content.LoadString("Strings\\UI:CoopMenu_Refresh");
      int width1 = (int) Game1.dialogueFont.MeasureString(str1).X + 64;
      Vector2 vector2_1 = new Vector2((float) (this.backButton.bounds.Right - width1), (float) (this.backButton.bounds.Y - 128));
      this.refreshButton = new ClickableComponent(new Rectangle((int) vector2_1.X, (int) vector2_1.Y, width1, 96), "", str1)
      {
        myID = 810,
        upNeighborID = -99998,
        leftNeighborID = -99998,
        rightNeighborID = -99998,
        downNeighborID = 81114
      };
      this._refreshDelay = 8f;
      this.smallScreenFormat = Game1.graphics.GraphicsDevice.Viewport.Height < 1080;
      string str2 = Game1.content.LoadString("Strings\\UI:CoopMenu_Join");
      int width2 = (int) Game1.dialogueFont.MeasureString(str2).X + 64;
      Vector2 vector2_2 = this.smallScreenFormat ? new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen) : new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen - 96));
      this.joinTab = new ClickableComponent(new Rectangle((int) vector2_2.X, (int) vector2_2.Y, width2, this.smallScreenFormat ? 72 : 64), "", str2)
      {
        myID = 811,
        downNeighborID = -99998,
        rightNeighborID = 812,
        region = 1000
      };
      string str3 = Game1.content.LoadString("Strings\\UI:CoopMenu_Host");
      int width3 = (int) Game1.dialogueFont.MeasureString(str3).X + 64;
      Vector2 vector2_3 = this.smallScreenFormat ? new Vector2((float) (this.joinTab.bounds.Right + (this.smallScreenFormat ? 0 : 4)), (float) this.yPositionOnScreen) : new Vector2((float) (this.joinTab.bounds.Right + 4), (float) (this.yPositionOnScreen - 64));
      this.hostTab = new ClickableComponent(new Rectangle((int) vector2_3.X, (int) vector2_3.Y, width3, this.smallScreenFormat ? 72 : 64), "", str3)
      {
        myID = 812,
        downNeighborID = -99998,
        leftNeighborID = 811,
        rightNeighborID = 800,
        region = 1000
      };
      this.backButton.upNeighborID = 810;
      if (this._tooManyFarms)
        this.hostSlots.Add((LoadGameMenu.MenuSlot) new CoopMenu.TooManyFarmsSlot(this));
      else
        this.hostSlots.Add((LoadGameMenu.MenuSlot) new CoopMenu.HostNewFarmSlot(this));
      this.menuSlots.Add((LoadGameMenu.MenuSlot) new CoopMenu.LanSlot(this));
      if (Program.sdk.Networking != null && Program.sdk.Networking.SupportsInviteCodes())
        this.menuSlots.Add((LoadGameMenu.MenuSlot) new CoopMenu.InviteCodeSlot(this));
      this.SetTab(this.currentTab, false);
      this.isSetUp = true;
      base.startListPopulation();
      this.populateClickableComponentList();
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (this.IsDoingTask())
        return;
      if (b == Buttons.LeftTrigger && this.joinTab.visible)
      {
        this.SetTab(CoopMenu.Tab.JOIN_TAB);
        this.setCurrentlySnappedComponentTo(this.joinTab.myID);
        this.snapCursorToCurrentSnappedComponent();
      }
      else
      {
        if (b != Buttons.RightTrigger || !this.hostTab.visible)
          return;
        this.SetTab(CoopMenu.Tab.HOST_TAB);
        this.setCurrentlySnappedComponentTo(this.hostTab.myID);
        this.snapCursorToCurrentSnappedComponent();
      }
    }

    public override void UpdateButtons()
    {
      base.UpdateButtons();
      foreach (ClickableComponent slotButton in this.slotButtons)
      {
        if (slotButton.myID == 0)
          slotButton.upNeighborID = this.currentItemIndex != 0 ? -7777 : 811;
      }
    }

    public override void update(GameTime time)
    {
      float totalSeconds = (float) time.ElapsedGameTime.TotalSeconds;
      ++this.updateCounter;
      if (!this.isSetUp)
      {
        if (!Program.sdk.ConnectionFinished)
          return;
        this.connectionFinished();
      }
      else
      {
        if (this.refreshButton != null && this.refreshButton.visible && (double) this._refreshDelay > 0.0)
          this._refreshDelay -= totalSeconds;
        base.update(time);
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.smallScreenFormat = Game1.graphics.GraphicsDevice.Viewport.Height < 1080;
      string text1 = Game1.content.LoadString("Strings\\UI:CoopMenu_Join");
      int num = (int) Game1.dialogueFont.MeasureString(text1).X + 64;
      Vector2 vector2_1 = this.smallScreenFormat ? new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen) : new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen - 96));
      this.joinTab.bounds.X = (int) vector2_1.X;
      this.joinTab.bounds.Y = (int) vector2_1.Y;
      string text2 = Game1.content.LoadString("Strings\\UI:CoopMenu_Host");
      num = (int) Game1.dialogueFont.MeasureString(text2).X + 64;
      Vector2 vector2_2 = this.smallScreenFormat ? new Vector2((float) (this.joinTab.bounds.Right + (this.smallScreenFormat ? 0 : 4)), (float) this.yPositionOnScreen) : new Vector2((float) (this.joinTab.bounds.Right + 4), (float) (this.yPositionOnScreen - 64));
      this.hostTab.bounds.X = (int) vector2_2.X;
      this.hostTab.bounds.Y = (int) vector2_2.Y;
      string text3 = Game1.content.LoadString("Strings\\UI:CoopMenu_Refresh");
      vector2_2 = new Vector2((float) (this.backButton.bounds.Right - ((int) Game1.dialogueFont.MeasureString(text3).X + 64)), (float) (this.backButton.bounds.Y - 128));
      this.refreshButton.bounds.X = (int) vector2_2.X;
      this.refreshButton.bounds.Y = (int) vector2_2.Y;
    }

    protected override void saveFileScanComplete()
    {
      if (Program.sdk.Networking == null)
        return;
      this.lobbyUpdateListener = (LobbyUpdateListener) new CoopMenu.LobbyUpdateCallback(new Action<object>(this.onLobbyUpdate));
      Program.sdk.Networking.AddLobbyUpdateListener(this.lobbyUpdateListener);
      Program.sdk.Networking.RequestFriendLobbyData();
    }

    protected virtual CoopMenu.FriendFarmData readLobbyFarmData(object lobby)
    {
      CoopMenu.FriendFarmData friendFarmData = new CoopMenu.FriendFarmData()
      {
        Lobby = lobby,
        Date = new WorldDate()
      };
      friendFarmData.OwnerName = Program.sdk.Networking.GetLobbyOwnerName(lobby);
      friendFarmData.FarmName = Program.sdk.Networking.GetLobbyData(lobby, "farmName");
      string lobbyData1 = Program.sdk.Networking.GetLobbyData(lobby, "farmType");
      string lobbyData2 = Program.sdk.Networking.GetLobbyData(lobby, "modFarmType");
      string lobbyData3 = Program.sdk.Networking.GetLobbyData(lobby, "date");
      int int32_1 = Convert.ToInt32(lobbyData1);
      int int32_2 = Convert.ToInt32(lobbyData3);
      friendFarmData.FarmType = int32_1;
      friendFarmData.ModFarmType = (ModFarmType) null;
      if (!string.IsNullOrEmpty(lobbyData2))
      {
        List<ModFarmType> modFarmTypeList = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
        if (modFarmTypeList != null)
        {
          foreach (ModFarmType modFarmType in modFarmTypeList)
          {
            if (modFarmType.ID == lobbyData2)
            {
              friendFarmData.ModFarmType = modFarmType;
              break;
            }
          }
        }
      }
      friendFarmData.Date.TotalDays = int32_2;
      friendFarmData.ProtocolVersion = Program.sdk.Networking.GetLobbyData(lobby, "protocolVersion");
      friendFarmData.FarmName = Program.sdk.FilterDirtyWords(friendFarmData.FarmName);
      friendFarmData.OwnerName = Program.sdk.FilterDirtyWords(friendFarmData.OwnerName);
      return friendFarmData;
    }

    protected virtual bool checkFriendFarmCompatibility(CoopMenu.FriendFarmData farm) => farm.FarmType >= 0 && farm.FarmType <= 7 && !(farm.ProtocolVersion != "1.5.5");

    protected virtual void onLobbyUpdate(object lobby)
    {
      try
      {
        string lobbyData1 = Program.sdk.Networking.GetLobbyData(lobby, "protocolVersion");
        if (lobbyData1 != "1.5.5")
          return;
        Console.WriteLine("Receiving friend lobby data...");
        Console.WriteLine("Owner: " + Program.sdk.Networking.GetLobbyOwnerName(lobby));
        Console.WriteLine("farmName = " + Program.sdk.Networking.GetLobbyData(lobby, "farmName"));
        Console.WriteLine("farmType = " + Program.sdk.Networking.GetLobbyData(lobby, "farmType"));
        Console.WriteLine("date = " + Program.sdk.Networking.GetLobbyData(lobby, "date"));
        Console.WriteLine("protocolVersion = " + lobbyData1);
        Console.WriteLine("farmhands = " + Program.sdk.Networking.GetLobbyData(lobby, "farmhands"));
        Console.WriteLine("newFarmhands = " + Program.sdk.Networking.GetLobbyData(lobby, "newFarmhands"));
        CoopMenu.FriendFarmData friendFarmData = this.readLobbyFarmData(lobby);
        if (!this.checkFriendFarmCompatibility(friendFarmData) || friendFarmData.FarmType == 7 && friendFarmData.ModFarmType == null)
          return;
        string userId = Program.sdk.Networking.GetUserID();
        string lobbyData2 = Program.sdk.Networking.GetLobbyData(lobby, "farmhands");
        bool boolean = Convert.ToBoolean(Program.sdk.Networking.GetLobbyData(lobby, "newFarmhands"));
        if (lobbyData2 == "" && !boolean)
          return;
        string[] source = lobbyData2.Split(',');
        if (!((IEnumerable<string>) source).Contains<string>(userId) && !boolean)
          return;
        friendFarmData.PreviouslyJoined = ((IEnumerable<string>) source).Contains<string>(userId);
        if (this.menuSlots == null)
          return;
        foreach (LoadGameMenu.MenuSlot menuSlot in this.menuSlots)
        {
          if (menuSlot is CoopMenu.FriendFarmSlot friendFarmSlot && friendFarmSlot.MatchAddress(lobby))
          {
            friendFarmSlot.Update(friendFarmData);
            return;
          }
        }
        this.menuSlots.Add((LoadGameMenu.MenuSlot) new CoopMenu.FriendFarmSlot(this, friendFarmData));
        this.UpdateButtons();
        this.populateClickableComponentList();
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return (a.region != 1000 || direction != 2 && direction != 0 || b.region != 1000) && (a.myID != 810 || direction != 0 || b.region == 900) && (a.myID != 810 || direction != 1 || b.myID != 81114) && base.IsAutomaticSnapValid(direction, a, b);
    }

    protected override void addSaveFiles(List<Farmer> files)
    {
      this.hostSlots.AddRange(files.Where<Farmer>((Func<Farmer, bool>) (file => file.slotCanHost)).Select<Farmer, LoadGameMenu.MenuSlot>((Func<Farmer, LoadGameMenu.MenuSlot>) (file => (LoadGameMenu.MenuSlot) new CoopMenu.HostFileSlot(this, file))));
      this.UpdateButtons();
    }

    protected virtual void setMenu(IClickableMenu menu)
    {
      if (Game1.activeClickableMenu is TitleMenu)
        TitleMenu.subMenu = menu;
      else
        Game1.activeClickableMenu = menu;
    }

    private void enterIPPressed()
    {
      string default_text = "";
      try
      {
        StartupPreferences startupPreferences = new StartupPreferences();
        startupPreferences.loadPreferences(false, false);
        default_text = startupPreferences.lastEnteredIP;
      }
      catch (Exception ex)
      {
      }
      this.setMenu((IClickableMenu) new TitleTextInputMenu(Game1.content.LoadString("Strings\\UI:CoopMenu_EnterIP"), (NamingMenu.doneNamingBehavior) (address =>
      {
        try
        {
          StartupPreferences startupPreferences = new StartupPreferences();
          startupPreferences.loadPreferences(false, false);
          startupPreferences.lastEnteredIP = address;
          startupPreferences.savePreferences(false);
        }
        catch (Exception ex)
        {
        }
        if (address == "")
          address = "localhost";
        this.setMenu((IClickableMenu) new FarmhandMenu(Game1.multiplayer.InitClient((Client) new LidgrenClient(address))));
      }), default_text, "join_menu"));
    }

    private void enterInviteCodePressed()
    {
      if (Program.sdk.Networking == null || !Program.sdk.Networking.SupportsInviteCodes())
        return;
      this.setMenu((IClickableMenu) new TitleTextInputMenu(Game1.content.LoadString("Strings\\UI:CoopMenu_EnterInviteCode"), (NamingMenu.doneNamingBehavior) (code =>
      {
        CoopMenu.lastEnteredInviteCode = code;
        object lobbyFromInviteCode = Program.sdk.Networking.GetLobbyFromInviteCode(code);
        if (lobbyFromInviteCode == null)
          return;
        this.setMenu((IClickableMenu) new FarmhandMenu(Program.sdk.Networking.CreateClient(lobbyFromInviteCode)));
      }), CoopMenu.lastEnteredInviteCode, "join_menu"));
    }

    private bool tabClick(int x, int y)
    {
      if (this.joinTab.visible && this.joinTab.containsPoint(x, y))
      {
        this.SetTab(CoopMenu.Tab.JOIN_TAB);
        return true;
      }
      if (!this.hostTab.visible || !this.hostTab.containsPoint(x, y))
        return false;
      this.SetTab(CoopMenu.Tab.HOST_TAB);
      return true;
    }

    public virtual void SetTab(CoopMenu.Tab new_tab, bool play_sound = true)
    {
      if (this.currentTab == new_tab)
        return;
      this.currentTab = new_tab;
      if (!this.smallScreenFormat && this.isSetUp)
      {
        if (this.currentTab == CoopMenu.Tab.HOST_TAB)
        {
          this.hostTab.bounds.Y = this.yPositionOnScreen - 96;
          this.joinTab.bounds.Y = this.yPositionOnScreen - 64;
        }
        else
        {
          this.hostTab.bounds.Y = this.yPositionOnScreen - 64;
          this.joinTab.bounds.Y = this.yPositionOnScreen - 96;
        }
      }
      if (play_sound)
        Game1.playSound("smallSelect");
      if (!this.isSetUp)
        return;
      this.UpdateButtons();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!this.isSetUp)
        return;
      if (this.refreshButton.visible && this.refreshButton.containsPoint(x, y))
      {
        if ((double) this._refreshDelay >= 0.0)
          return;
        Game1.playSound("bigDeSelect");
        this.setMenu((IClickableMenu) new CoopMenu(this._tooManyFarms));
      }
      else
      {
        if (this.smallScreenFormat && this.tabClick(x, y))
          return;
        base.receiveLeftClick(x, y, playSound);
        if (this.smallScreenFormat || this.loading)
          return;
        this.tabClick(x, y);
      }
    }

    public override void performHoverAction(int x, int y)
    {
      if (!this.isSetUp)
        return;
      this.refreshButton.scale = !this.refreshButton.visible || !this.refreshButton.containsPoint(x, y) ? 0.0f : 1f;
      if (this.smallScreenFormat && (this.hostTab.containsPoint(x, y) || this.joinTab.containsPoint(x, y)))
        base.performHoverAction(-100, -100);
      else
        base.performHoverAction(x, y);
    }

    protected override string getStatusText() => (string) null;

    private void drawTabs(SpriteBatch b)
    {
      if (!this.isSetUp)
        return;
      Color color1 = this.smallScreenFormat ? Color.Orange : new Color((int) byte.MaxValue, (int) byte.MaxValue, 150);
      Color yellow = Color.Yellow;
      Color color2 = this.smallScreenFormat ? Color.DarkOrange : new Color(221, 148, 84);
      Color darkGoldenrod = Color.DarkGoldenrod;
      if (this.joinTab.visible)
      {
        bool flag1 = this.currentTab == CoopMenu.Tab.JOIN_TAB;
        bool flag2 = this.currentTab != CoopMenu.Tab.JOIN_TAB && this.joinTab.containsPoint(Game1.getMouseX(), Game1.getMouseY());
        IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), this.joinTab.bounds.X, this.joinTab.bounds.Y, this.joinTab.bounds.Width, this.joinTab.bounds.Height + (this.smallScreenFormat ? 0 : 64), flag1 ? color1 : (flag2 ? yellow : Color.White), drawShadow: false);
        Utility.drawTextWithColoredShadow(b, this.joinTab.label, Game1.dialogueFont, new Vector2((float) this.joinTab.bounds.Center.X, (float) (this.joinTab.bounds.Y + 40)) - Game1.dialogueFont.MeasureString(this.joinTab.label) / 2f, Game1.textColor, flag2 ? darkGoldenrod : (flag1 ? color2 : new Color(221, 148, 84)), 1.01f);
      }
      if (!this.hostTab.visible)
        return;
      bool flag3 = this.currentTab == CoopMenu.Tab.HOST_TAB;
      bool flag4 = this.currentTab != CoopMenu.Tab.HOST_TAB && this.hostTab.containsPoint(Game1.getMouseX(), Game1.getMouseY());
      IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), this.hostTab.bounds.X, this.hostTab.bounds.Y, this.hostTab.bounds.Width, this.hostTab.bounds.Height + (this.smallScreenFormat ? 0 : 64), flag3 ? color1 : (flag4 ? yellow : Color.White), drawShadow: false);
      Utility.drawTextWithColoredShadow(b, this.hostTab.label, Game1.dialogueFont, new Vector2((float) this.hostTab.bounds.Center.X, (float) (this.hostTab.bounds.Y + 40)) - Game1.dialogueFont.MeasureString(this.hostTab.label) / 2f, Game1.textColor, flag4 ? darkGoldenrod : (flag3 ? color2 : new Color(221, 148, 84)), 1.01f);
    }

    public override void snapToDefaultClickableComponent()
    {
      base.snapToDefaultClickableComponent();
      if (this.currentlySnappedComponent != null)
        return;
      this.currentlySnappedComponent = this.getComponentWithID(811);
      this.snapCursorToCurrentSnappedComponent();
    }

    protected override void drawBefore(SpriteBatch b)
    {
      base.drawBefore(b);
      if (!this.isSetUp || this.smallScreenFormat)
        return;
      this.drawTabs(b);
    }

    protected override void drawExtra(SpriteBatch b)
    {
      base.drawExtra(b);
      if (!this.isSetUp)
        return;
      if (this.refreshButton.visible)
      {
        Color color = (double) this.refreshButton.scale > 0.0 ? Color.Wheat : Color.White;
        if ((double) this._refreshDelay > 0.0)
          color = Color.Gray;
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), this.refreshButton.bounds.X, this.refreshButton.bounds.Y, this.refreshButton.bounds.Width, this.refreshButton.bounds.Height, color, 4f);
        Utility.drawTextWithShadow(b, this.refreshButton.label, Game1.dialogueFont, new Vector2((float) this.refreshButton.bounds.Center.X, (float) (this.refreshButton.bounds.Center.Y + 4)) - Game1.dialogueFont.MeasureString(this.refreshButton.label) / 2f, Game1.textColor, shadowIntensity: 0.0f);
      }
      if (!this.smallScreenFormat)
        return;
      this.drawTabs(b);
    }

    protected override void drawStatusText(SpriteBatch b)
    {
      if (this.getStatusText() != null)
      {
        base.drawStatusText(b);
      }
      else
      {
        if (this.isSetUp)
          return;
        int num1 = 1 + Program.sdk.ConnectionProgress;
        int num2 = this.updateCounter / 5 % num1;
        string str = Game1.content.LoadString("Strings\\UI:CoopMenu_ConnectingOnlineServices");
        this._stringBuilder.Clear();
        this._stringBuilder.Append(str);
        for (int index = 0; index < num2; ++index)
          this._stringBuilder.Append(".");
        string s = this._stringBuilder.ToString();
        for (int index = num2; index < num1; ++index)
          this._stringBuilder.Append(".");
        int widthOfString = SpriteText.getWidthOfString(this._stringBuilder.ToString());
        SpriteText.drawString(b, s, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.X - widthOfString / 2, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.Y);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (this.lobbyUpdateListener != null && Program.sdk.Networking != null)
        Program.sdk.Networking.RemoveLobbyUpdateListener(this.lobbyUpdateListener);
      this.lobbyUpdateListener = (LobbyUpdateListener) null;
      base.Dispose(disposing);
    }

    public enum Tab
    {
      JOIN_TAB,
      HOST_TAB,
    }

    protected abstract class CoopMenuSlot : LoadGameMenu.MenuSlot
    {
      protected CoopMenu menu;

      public CoopMenuSlot(CoopMenu menu)
        : base((LoadGameMenu) menu)
      {
        this.menu = menu;
      }
    }

    protected abstract class LabeledSlot : CoopMenu.CoopMenuSlot
    {
      private string message;

      public LabeledSlot(CoopMenu menu, string message)
        : base(menu)
      {
        this.message = message;
      }

      public abstract override void Activate();

      public override void Draw(SpriteBatch b, int i)
      {
        int widthOfString = SpriteText.getWidthOfString(this.message);
        int heightOfString = SpriteText.getHeightOfString(this.message);
        Rectangle bounds = this.menu.slotButtons[i].bounds;
        int x = bounds.X + (bounds.Width - widthOfString) / 2;
        int y = bounds.Y + (bounds.Height - heightOfString) / 2;
        SpriteText.drawString(b, this.message, x, y);
      }
    }

    protected class LanSlot : CoopMenu.LabeledSlot
    {
      public LanSlot(CoopMenu menu)
        : base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_JoinLANGame"))
      {
      }

      public override void Activate() => this.menu.enterIPPressed();
    }

    protected class InviteCodeSlot : CoopMenu.LabeledSlot
    {
      public InviteCodeSlot(CoopMenu menu)
        : base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_EnterInviteCode"))
      {
      }

      public override void Activate() => this.menu.enterInviteCodePressed();
    }

    protected class HostNewFarmSlot : CoopMenu.LabeledSlot
    {
      public HostNewFarmSlot(CoopMenu menu)
        : base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_HostNewFarm"))
      {
        this.ActivateDelay = 2150;
      }

      public override void Activate()
      {
        Game1.resetPlayer();
        TitleMenu.subMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.HostNewFarm);
        Game1.changeMusicTrack("CloudCountry");
      }
    }

    protected class TooManyFarmsSlot : CoopMenu.LabeledSlot
    {
      public TooManyFarmsSlot(CoopMenu menu)
        : base(menu, Game1.content.LoadString("Strings\\UI:TooManyFarmsMenu_TooManyFarms"))
      {
      }

      public override void Activate()
      {
      }
    }

    protected class HostFileSlot : LoadGameMenu.SaveFileSlot
    {
      protected CoopMenu menu;

      public HostFileSlot(CoopMenu menu, Farmer farmer)
        : base((LoadGameMenu) menu, farmer)
      {
        this.menu = menu;
      }

      public override void Activate()
      {
        Game1.multiplayerMode = (byte) 2;
        base.Activate();
      }

      protected override void drawSlotSaveNumber(SpriteBatch b, int i)
      {
      }

      protected override string slotName() => Game1.content.LoadString("Strings\\UI:CoopMenu_HostFile", (object) this.Farmer.Name, (object) this.Farmer.farmName.Value);

      protected override string slotSubName() => this.Farmer.Name;

      protected override Vector2 portraitOffset() => base.portraitOffset() - new Vector2(32f, 0.0f);
    }

    protected class FriendFarmData
    {
      public object Lobby;
      public string OwnerName;
      public string FarmName;
      public int FarmType;
      public ModFarmType ModFarmType;
      public WorldDate Date;
      public bool PreviouslyJoined;
      public string ProtocolVersion;
    }

    protected class FriendFarmSlot : CoopMenu.CoopMenuSlot
    {
      public CoopMenu.FriendFarmData Farm;

      public FriendFarmSlot(CoopMenu menu, CoopMenu.FriendFarmData farm)
        : base(menu)
      {
        this.Farm = farm;
      }

      public bool MatchAddress(object Lobby) => object.Equals(this.Farm.Lobby, Lobby);

      public void Update(CoopMenu.FriendFarmData newData) => this.Farm = newData;

      public override void Activate() => this.menu.setMenu((IClickableMenu) new FarmhandMenu(Program.sdk.Networking.CreateClient(this.Farm.Lobby)));

      protected virtual string slotName()
      {
        string path = this.Farm.PreviouslyJoined ? "Strings\\UI:CoopMenu_RevisitFriendFarm" : "Strings\\UI:CoopMenu_JoinFriendFarm";
        return Game1.content.LoadString(path, (object) this.Farm.FarmName);
      }

      protected virtual void drawSlotName(SpriteBatch b, int i) => SpriteText.drawString(b, this.slotName(), this.menu.slotButtons[i].bounds.X + 128 + 36, this.menu.slotButtons[i].bounds.Y + 36);

      protected virtual void drawSlotDate(SpriteBatch b, int i) => Utility.drawTextWithShadow(b, this.Farm.Date.Localize(), Game1.dialogueFont, new Vector2((float) (this.menu.slotButtons[i].bounds.X + 128 + 32), (float) (this.menu.slotButtons[i].bounds.Y + 64 + 40)), Game1.textColor);

      protected virtual void drawSlotFarm(SpriteBatch b, int i)
      {
        int num = this.Farm.FarmType;
        if (num == 7)
          num = 0;
        Rectangle rectangle1 = new Rectangle(22 * (num % 5), 324 + 21 * (num / 5), 22, 20);
        Texture2D mouseCursors = Game1.mouseCursors;
        Rectangle rectangle2 = new Rectangle(this.menu.slotButtons[i].bounds.X, this.menu.slotButtons[i].bounds.Y, 160, this.menu.slotButtons[i].bounds.Height);
        Rectangle destinationRectangle = new Rectangle(rectangle2.X + (rectangle2.Width - rectangle1.Width * 4) / 2, rectangle2.Y + (rectangle2.Height - rectangle1.Height * 4) / 2, rectangle1.Width * 4, rectangle1.Height * 4);
        if (this.Farm.ModFarmType != null && this.Farm.ModFarmType.IconTexture != null)
        {
          Texture2D texture = Game1.content.Load<Texture2D>(this.Farm.ModFarmType.IconTexture);
          b.Draw(texture, destinationRectangle, new Rectangle?(), Color.White);
        }
        else
          b.Draw(mouseCursors, destinationRectangle, new Rectangle?(rectangle1), Color.White);
      }

      protected virtual void drawSlotOwnerName(SpriteBatch b, int i)
      {
        float scale = 1f;
        float num1 = 128f;
        float num2 = 44f;
        Utility.drawTextWithShadow(b, this.Farm.OwnerName, Game1.dialogueFont, new Vector2((float) ((double) (this.menu.slotButtons[i].bounds.X + this.menu.width) - (double) num1 - (double) Game1.dialogueFont.MeasureString(this.Farm.OwnerName).X * (double) scale), (float) this.menu.slotButtons[i].bounds.Y + num2), Game1.textColor, scale);
      }

      public override void Draw(SpriteBatch b, int i)
      {
        this.drawSlotName(b, i);
        this.drawSlotDate(b, i);
        this.drawSlotFarm(b, i);
        this.drawSlotOwnerName(b, i);
      }
    }

    private class LobbyUpdateCallback : LobbyUpdateListener
    {
      private Action<object> callback;

      public LobbyUpdateCallback(Action<object> callback) => this.callback = callback;

      public void OnLobbyUpdate(object lobby)
      {
        if (this.callback == null)
          return;
        this.callback(lobby);
      }
    }
  }
}
