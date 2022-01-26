// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.FarmhandMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StardewValley.Menus
{
  public class FarmhandMenu : LoadGameMenu
  {
    public bool gettingFarmhands;
    public bool approvingFarmhand;
    public Client client;

    public FarmhandMenu()
      : this((Client) null)
    {
    }

    public FarmhandMenu(Client client)
    {
      if (client == null && Program.sdk.Networking != null)
        client = Program.sdk.Networking.GetRequestedClient();
      this.client = client;
      if (client == null)
        return;
      this.gettingFarmhands = true;
    }

    public override bool readyToClose() => !this.loading;

    protected override bool hasDeleteButtons() => false;

    protected override void startListPopulation()
    {
    }

    public override void UpdateButtons()
    {
      base.UpdateButtons();
      if (!LocalMultiplayer.IsLocalMultiplayer() || Game1.game1.IsMainInstance || this.backButton == null)
        return;
      this.backButton.visible = false;
    }

    protected override bool checkListPopulation()
    {
      if (this.client != null && (this.gettingFarmhands || this.approvingFarmhand) && (this.client.availableFarmhands != null || this.client.connectionMessage != null))
      {
        this.timerToLoad = 0;
        this.selected = -1;
        this.loading = false;
        this.gettingFarmhands = false;
        if (this.menuSlots == null)
          this.menuSlots = new List<LoadGameMenu.MenuSlot>();
        else
          this.menuSlots.Clear();
        if (this.client.availableFarmhands == null)
        {
          this.approvingFarmhand = true;
        }
        else
        {
          this.approvingFarmhand = false;
          this.menuSlots.AddRange((IEnumerable<LoadGameMenu.MenuSlot>) this.client.availableFarmhands.Select<Farmer, FarmhandMenu.FarmhandSlot>((Func<Farmer, FarmhandMenu.FarmhandSlot>) (farmer => new FarmhandMenu.FarmhandSlot(this, farmer))));
        }
        if (Game1.activeClickableMenu is TitleMenu)
          Game1.gameMode = (byte) 0;
        else if (!Game1.game1.IsMainInstance)
          Game1.gameMode = (byte) 0;
        this.UpdateButtons();
        if (Game1.options.SnappyMenus)
        {
          this.populateClickableComponentList();
          this.snapToDefaultClickableComponent();
        }
      }
      return false;
    }

    public override void receiveGamePadButton(Buttons b)
    {
      if (b == Buttons.B && this.readyToClose())
        this.exitThisMenu();
      base.receiveGamePadButton(b);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      for (int index = 0; index < this.slotButtons.Count; ++index)
      {
        if (this.slotButtons[index].containsPoint(x, y) && index < this.MenuSlots.Count && this.MenuSlots[this.currentItemIndex + index] is FarmhandMenu.FarmhandSlot && (this.MenuSlots[this.currentItemIndex + index] as FarmhandMenu.FarmhandSlot).BelongsToAnotherPlayer())
        {
          Game1.playSound("cancel");
          return;
        }
      }
      base.receiveLeftClick(x, y, playSound);
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      if (!(this.hoverText == ""))
        return;
      for (int index = 0; index < this.slotButtons.Count; ++index)
      {
        if (this.currentItemIndex + index < this.MenuSlots.Count && this.slotButtons[index].containsPoint(x, y))
        {
          LoadGameMenu.MenuSlot menuSlot = this.MenuSlots[this.currentItemIndex + index];
          if (menuSlot is FarmhandMenu.FarmhandSlot && (menuSlot as FarmhandMenu.FarmhandSlot).BelongsToAnotherPlayer())
            this.hoverText = Game1.content.LoadString("Strings\\UI:Farmhand_Locked");
        }
      }
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return (b == null || b.myID != 800 && b.myID != 801 || this.menuSlots.Count > 4) && base.IsAutomaticSnapValid(direction, a, b);
    }

    public override void update(GameTime time)
    {
      if (this.client != null)
      {
        if (!this.client.connectionStarted && this.drawn)
          this.client.connect();
        if (this.client.connectionStarted)
          this.client.receiveMessages();
        if (this.client.readyToPlay)
        {
          Game1.gameMode = (byte) 3;
          this.loadClientOptions();
          switch (Game1.activeClickableMenu)
          {
            case FarmhandMenu _:
label_8:
              Game1.exitActiveMenu();
              break;
            case TitleMenu _:
              if (!(TitleMenu.subMenu is FarmhandMenu))
                break;
              goto label_8;
          }
        }
        else if (this.client.timedOut)
        {
          if (this.approvingFarmhand)
            Game1.multiplayer.clientRemotelyDisconnected(Multiplayer.IsTimeout(this.client.pendingDisconnect) ? Multiplayer.DisconnectType.Timeout_FarmhandSelection : this.client.pendingDisconnect);
          else
            this.menuSlots.RemoveAll((Predicate<LoadGameMenu.MenuSlot>) (slot => slot is FarmhandMenu.FarmhandSlot));
        }
      }
      base.update(time);
    }

    private void loadClientOptions()
    {
      Action action = (Action) (() =>
      {
        StartupPreferences startupPreferences = new StartupPreferences();
        startupPreferences.loadPreferences(false, false);
        Game1.options = !Game1.game1.IsMainInstance ? new Options() : startupPreferences.clientOptions;
        Game1.initializeVolumeLevels();
      });
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        Game1.currentSong = Game1.soundBank.GetCue("spring_day_ambient");
        action();
      }
      else
      {
        Task task = new Task(action);
        Game1.hooks.StartTask(task, "ClientOptions_Load");
      }
    }

    protected override string getStatusText()
    {
      if (this.client == null)
        return Game1.content.LoadString("Strings\\UI:CoopMenu_NoInvites");
      if (this.client.timedOut)
        return Game1.content.LoadString("Strings\\UI:CoopMenu_Failed");
      if (this.client.connectionMessage != null)
        return this.client.connectionMessage;
      if (this.gettingFarmhands || this.approvingFarmhand)
        return Game1.content.LoadString("Strings\\UI:CoopMenu_Connecting");
      return this.menuSlots.Count == 0 ? Game1.content.LoadString("Strings\\UI:CoopMenu_NoSlots") : (string) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (this.client != null & disposing && Game1.client != this.client)
      {
        Multiplayer.LogDisconnect(Multiplayer.IsTimeout(this.client.pendingDisconnect) ? Multiplayer.DisconnectType.Timeout_FarmhandSelection : Multiplayer.DisconnectType.ExitedToMainMenu_FromFarmhandSelect);
        this.client.disconnect();
        if (!Game1.game1.IsMainInstance)
          GameRunner.instance.RemoveGameInstance(Game1.game1);
      }
      base.Dispose(disposing);
    }

    public class FarmhandSlot : LoadGameMenu.SaveFileSlot
    {
      protected FarmhandMenu menu;
      protected bool _belongsToAnotherPlayer;

      public bool BelongsToAnotherPlayer() => (Game1.game1 == null || Game1.game1.IsMainInstance) && this._belongsToAnotherPlayer;

      public FarmhandSlot(FarmhandMenu menu, Farmer farmer)
        : base((LoadGameMenu) menu, farmer)
      {
        this.menu = menu;
        if (Program.sdk.Networking == null)
          return;
        string userId = Program.sdk.Networking.GetUserID();
        if (!(userId != "") || farmer == null || !(farmer.userID.Value != "") || !(userId != (string) (NetFieldBase<string, NetString>) farmer.userID))
          return;
        this._belongsToAnotherPlayer = true;
      }

      public override void Activate()
      {
        if (this.menu.client == null)
          return;
        Game1.loadForNewGame();
        Game1.player = this.Farmer;
        this.menu.client.availableFarmhands = (List<Farmer>) null;
        this.menu.client.sendPlayerIntroduction();
        this.menu.approvingFarmhand = true;
        this.menu.menuSlots.Clear();
        Game1.gameMode = (byte) 6;
      }

      public override float getSlotAlpha() => this.BelongsToAnotherPlayer() ? 0.5f : base.getSlotAlpha();

      protected override void drawSlotName(SpriteBatch b, int i)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.Farmer.isCustomized)
        {
          base.drawSlotName(b, i);
        }
        else
        {
          string s = Game1.content.LoadString("Strings\\UI:CoopMenu_NewFarmhand");
          SpriteText.drawString(b, s, this.menu.slotButtons[i].bounds.X + 128 + 36, this.menu.slotButtons[i].bounds.Y + 36);
        }
      }

      protected override void drawSlotShadow(SpriteBatch b, int i)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.Farmer.isCustomized)
          return;
        base.drawSlotShadow(b, i);
      }

      protected override void drawSlotFarmer(SpriteBatch b, int i)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.Farmer.isCustomized)
          return;
        base.drawSlotFarmer(b, i);
      }

      protected override void drawSlotTimer(SpriteBatch b, int i)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.Farmer.isCustomized)
          return;
        base.drawSlotTimer(b, i);
      }

      protected override void drawSlotMoney(SpriteBatch b, int i)
      {
      }
    }
  }
}
