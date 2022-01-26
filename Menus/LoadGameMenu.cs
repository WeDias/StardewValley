// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LoadGameMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StardewValley.Menus
{
  public class LoadGameMenu : IClickableMenu, IDisposable
  {
    protected const int CenterOffset = 0;
    public const int region_upArrow = 800;
    public const int region_downArrow = 801;
    public const int region_okDelete = 802;
    public const int region_cancelDelete = 803;
    public const int region_slots = 900;
    public const int region_deleteButtons = 901;
    public const int region_navigationButtons = 902;
    public const int region_deleteConfirmations = 903;
    public const int itemsPerPage = 4;
    public List<ClickableComponent> slotButtons = new List<ClickableComponent>();
    public List<ClickableTextureComponent> deleteButtons = new List<ClickableTextureComponent>();
    protected int currentItemIndex;
    protected int timerToLoad;
    protected int selected = -1;
    protected int selectedForDelete = -1;
    public ClickableTextureComponent upArrow;
    public ClickableTextureComponent downArrow;
    public ClickableTextureComponent scrollBar;
    public ClickableTextureComponent okDeleteButton;
    public ClickableTextureComponent cancelDeleteButton;
    public ClickableComponent backButton;
    public bool scrolling;
    public bool deleteConfirmationScreen;
    protected List<LoadGameMenu.MenuSlot> menuSlots = new List<LoadGameMenu.MenuSlot>();
    private Rectangle scrollBarRunner;
    protected string hoverText = "";
    protected bool loading;
    protected bool drawn;
    private bool deleting;
    private int _updatesSinceLastDeleteConfirmScreen;
    private Task<List<Farmer>> _initTask;
    private Task _deleteTask;
    private bool disposedValue;

    public bool IsDoingTask() => this._initTask != null || this._deleteTask != null || this.loading || this.deleting;

    public override bool readyToClose() => !this.IsDoingTask() && this._updatesSinceLastDeleteConfirmScreen > 1;

    protected virtual List<LoadGameMenu.MenuSlot> MenuSlots
    {
      get => this.menuSlots;
      set => this.menuSlots = value;
    }

    public LoadGameMenu()
      : base(Game1.uiViewport.Width / 2 - (1100 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 1100 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2)
    {
      this.backButton = new ClickableComponent(new Rectangle(Game1.uiViewport.Width - 198 - 48, Game1.uiViewport.Height - 81 - 24, 198, 81), "")
      {
        myID = 81114,
        upNeighborID = -99998,
        leftNeighborID = -99998,
        rightNeighborID = -99998,
        downNeighborID = -99998
      };
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + 16, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
      textureComponent1.myID = 800;
      textureComponent1.downNeighborID = 801;
      textureComponent1.leftNeighborID = -99998;
      textureComponent1.rightNeighborID = -99998;
      textureComponent1.region = 902;
      this.upArrow = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + this.height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
      textureComponent2.myID = 801;
      textureComponent2.upNeighborID = 800;
      textureComponent2.leftNeighborID = -99998;
      textureComponent2.downNeighborID = -99998;
      textureComponent2.rightNeighborID = -99998;
      textureComponent2.region = 902;
      this.downArrow = textureComponent2;
      this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 64 - this.upArrow.bounds.Height - 28);
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.10992"), new Rectangle((int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).X - 64, (int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).Y + 128, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent3.myID = 802;
      textureComponent3.rightNeighborID = 803;
      textureComponent3.region = 903;
      this.okDeleteButton = textureComponent3;
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.10993"), new Rectangle((int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).X + 64, (int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).Y + 128, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
      textureComponent4.myID = 803;
      textureComponent4.leftNeighborID = 802;
      textureComponent4.region = 903;
      this.cancelDeleteButton = textureComponent4;
      for (int index = 0; index < 4; ++index)
      {
        this.slotButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + index * (this.height / 4), this.width - 32, this.height / 4 + 4), index.ToString() ?? "")
        {
          myID = index,
          region = 900,
          downNeighborID = index < 3 ? -99998 : -7777,
          upNeighborID = index > 0 ? -99998 : -7777,
          rightNeighborID = -99998,
          fullyImmutable = true
        });
        if (this.hasDeleteButtons())
        {
          List<ClickableTextureComponent> deleteButtons = this.deleteButtons;
          ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width - 64 - 4, this.yPositionOnScreen + 32 + 4 + index * (this.height / 4), 48, 48), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.10994"), Game1.mouseCursors, new Rectangle(322, 498, 12, 12), 3f);
          textureComponent5.myID = index + 100;
          textureComponent5.region = 901;
          textureComponent5.leftNeighborID = -99998;
          textureComponent5.downNeighborImmutable = true;
          textureComponent5.downNeighborID = -99998;
          textureComponent5.upNeighborImmutable = true;
          textureComponent5.upNeighborID = index > 0 ? -99998 : -1;
          textureComponent5.rightNeighborID = -99998;
          deleteButtons.Add(textureComponent5);
        }
      }
      this.startListPopulation();
      if (Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        this.populateClickableComponentList();
        this.snapToDefaultClickableComponent();
      }
      this.UpdateButtons();
    }

    protected virtual bool hasDeleteButtons() => true;

    protected virtual void startListPopulation()
    {
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        this.addSaveFiles(LoadGameMenu.FindSaveGames());
        this.saveFileScanComplete();
      }
      else
      {
        this._initTask = new Task<List<Farmer>>((Func<List<Farmer>>) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          return LoadGameMenu.FindSaveGames();
        }));
        Game1.hooks.StartTask<List<Farmer>>(this._initTask, "Find Save Games");
      }
    }

    public virtual void UpdateButtons()
    {
      for (int index = 0; index < this.slotButtons.Count; ++index)
      {
        ClickableTextureComponent textureComponent = (ClickableTextureComponent) null;
        if (this.hasDeleteButtons() && index >= 0 && index < this.deleteButtons.Count)
          textureComponent = this.deleteButtons[index];
        if (this.currentItemIndex + index < this.MenuSlots.Count)
        {
          this.slotButtons[index].visible = true;
          if (textureComponent != null)
            textureComponent.visible = true;
        }
        else
        {
          this.slotButtons[index].visible = false;
          if (textureComponent != null)
            textureComponent.visible = false;
        }
      }
    }

    protected virtual void addSaveFiles(List<Farmer> files)
    {
      this.MenuSlots.AddRange(files.Select<Farmer, LoadGameMenu.MenuSlot>((Func<Farmer, LoadGameMenu.MenuSlot>) (file => (LoadGameMenu.MenuSlot) new LoadGameMenu.SaveFileSlot(this, file))));
      this.UpdateButtons();
    }

    private static List<Farmer> FindSaveGames()
    {
      List<Farmer> saveGames = new List<Farmer>();
      string str = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"));
      if (Directory.Exists(str))
      {
        foreach (string path2 in Directory.EnumerateDirectories(str).ToList<string>())
        {
          string path3 = ((IEnumerable<string>) path2.Split(Path.DirectorySeparatorChar)).Last<string>();
          string path = Path.Combine(str, path2, "SaveGameInfo");
          if (File.Exists(Path.Combine(str, path2, path3)))
          {
            Farmer target = (Farmer) null;
            try
            {
              using (FileStream fileStream = File.OpenRead(path))
              {
                target = (Farmer) SaveGame.farmerSerializer.Deserialize((Stream) fileStream);
                SaveGame.loadDataToFarmer(target);
                target.slotName = path3;
                saveGames.Add(target);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine("Exception occured trying to access file '{0}'", (object) path);
              Console.WriteLine(ex.GetBaseException().ToString());
              target?.unload();
            }
          }
        }
      }
      saveGames.Sort();
      return saveGames;
    }

    public override void receiveGamePadButton(Buttons b)
    {
      if (b != Buttons.B || !this.deleteConfirmationScreen)
        return;
      this.deleteConfirmationScreen = false;
      this.selectedForDelete = -1;
      Game1.playSound("smallSelect");
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      if (this.deleteConfirmationScreen)
        this.currentlySnappedComponent = this.getComponentWithID(803);
      else
        this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      if (direction == 2 && this.currentItemIndex < Math.Max(0, this.MenuSlots.Count - 4))
      {
        this.downArrowPressed();
        this.currentlySnappedComponent = this.getComponentWithID(3);
        this.snapCursorToCurrentSnappedComponent();
      }
      else
      {
        if (direction != 0 || this.currentItemIndex <= 0)
          return;
        this.upArrowPressed();
        this.currentlySnappedComponent = this.getComponentWithID(0);
        this.snapCursorToCurrentSnappedComponent();
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.xPositionOnScreen = (newBounds.Width - this.width) / 2;
      this.yPositionOnScreen = (newBounds.Height - (this.height + 32)) / 2;
      this.backButton.bounds.X = Game1.uiViewport.Width - 198 - 48;
      this.backButton.bounds.Y = Game1.uiViewport.Height - 81 - 24;
      this.upArrow.bounds.X = this.xPositionOnScreen + this.width + 16;
      this.upArrow.bounds.Y = this.yPositionOnScreen + 16;
      this.downArrow.bounds.X = this.xPositionOnScreen + this.width + 16;
      this.downArrow.bounds.Y = this.yPositionOnScreen + this.height - 64;
      this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 64 - this.upArrow.bounds.Height - 28);
      this.okDeleteButton.bounds.X = (int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).X - 64;
      this.okDeleteButton.bounds.Y = (int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).Y + 128;
      this.cancelDeleteButton.bounds.X = (int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).X + 64;
      this.cancelDeleteButton.bounds.Y = (int) Utility.getTopLeftPositionForCenteringOnScreen(64, 64).Y + 128;
      for (int index = 0; index < this.slotButtons.Count; ++index)
      {
        this.slotButtons[index].bounds.X = this.xPositionOnScreen + 16;
        this.slotButtons[index].bounds.Y = this.yPositionOnScreen + 16 + index * (this.height / 4);
      }
      for (int index = 0; index < this.deleteButtons.Count; ++index)
      {
        this.deleteButtons[index].bounds.X = this.xPositionOnScreen + this.width - 64 - 4;
        this.deleteButtons[index].bounds.Y = this.yPositionOnScreen + 32 + 4 + index * (this.height / 4);
      }
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      int id = this.currentlySnappedComponent != null ? this.currentlySnappedComponent.myID : 81114;
      this.populateClickableComponentList();
      this.currentlySnappedComponent = this.getComponentWithID(id);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      base.performHoverAction(x, y);
      if (this.deleteConfirmationScreen)
      {
        this.okDeleteButton.tryHover(x, y);
        this.cancelDeleteButton.tryHover(x, y);
        if (this.okDeleteButton.containsPoint(x, y))
        {
          this.hoverText = "";
        }
        else
        {
          if (!this.cancelDeleteButton.containsPoint(x, y))
            return;
          this.hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.10993");
        }
      }
      else
      {
        this.upArrow.tryHover(x, y);
        this.downArrow.tryHover(x, y);
        this.scrollBar.tryHover(x, y);
        foreach (ClickableTextureComponent deleteButton in this.deleteButtons)
        {
          deleteButton.tryHover(x, y, 0.2f);
          if (deleteButton.containsPoint(x, y))
          {
            this.hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.10994");
            return;
          }
        }
        if (this.scrolling)
          return;
        for (int index = 0; index < this.slotButtons.Count; ++index)
        {
          if (this.currentItemIndex + index < this.MenuSlots.Count && this.slotButtons[index].containsPoint(x, y))
          {
            if ((double) this.slotButtons[index].scale == 1.0)
              Game1.playSound("Cowboy_gunshot");
            this.slotButtons[index].scale = Math.Min(this.slotButtons[index].scale + 0.03f, 1.1f);
          }
          else
            this.slotButtons[index].scale = Math.Max(1f, this.slotButtons[index].scale - 0.03f);
        }
      }
    }

    public override void leftClickHeld(int x, int y)
    {
      base.leftClickHeld(x, y);
      if (!this.scrolling)
        return;
      int y1 = this.scrollBar.bounds.Y;
      this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upArrow.bounds.Height + 20));
      this.currentItemIndex = Math.Min(this.MenuSlots.Count - 4, Math.Max(0, (int) ((double) this.MenuSlots.Count * (double) ((float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height))));
      this.setScrollBarToCurrentIndex();
      int y2 = this.scrollBar.bounds.Y;
      if (y1 == y2)
        return;
      Game1.playSound("shiny4");
    }

    public override void releaseLeftClick(int x, int y)
    {
      base.releaseLeftClick(x, y);
      this.scrolling = false;
    }

    protected void setScrollBarToCurrentIndex()
    {
      if (this.MenuSlots.Count > 0)
      {
        this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.MenuSlots.Count - 4 + 1) * this.currentItemIndex + this.upArrow.bounds.Bottom + 4;
        if (this.currentItemIndex == this.MenuSlots.Count - 4)
          this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
      }
      this.UpdateButtons();
    }

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && this.currentItemIndex > 0)
      {
        this.upArrowPressed();
      }
      else
      {
        if (direction >= 0 || this.currentItemIndex >= Math.Max(0, this.MenuSlots.Count - 4))
          return;
        this.downArrowPressed();
      }
    }

    private void downArrowPressed()
    {
      this.downArrow.scale = this.downArrow.baseScale;
      ++this.currentItemIndex;
      Game1.playSound("shwip");
      this.setScrollBarToCurrentIndex();
    }

    private void upArrowPressed()
    {
      this.upArrow.scale = this.upArrow.baseScale;
      --this.currentItemIndex;
      Game1.playSound("shwip");
      this.setScrollBarToCurrentIndex();
    }

    private void deleteFile(int which)
    {
      if (!(this.MenuSlots[which] is LoadGameMenu.SaveFileSlot menuSlot))
        return;
      string slotName = menuSlot.Farmer.slotName;
      string path = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), slotName));
      Thread.Sleep(Game1.random.Next(1000, 5000));
      if (!Directory.Exists(path))
        return;
      Directory.Delete(path, true);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.timerToLoad > 0 || this.loading || this.deleting)
        return;
      if (this.deleteConfirmationScreen)
      {
        if (this.cancelDeleteButton.containsPoint(x, y))
        {
          this.deleteConfirmationScreen = false;
          this.selectedForDelete = -1;
          Game1.playSound("smallSelect");
          if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
            return;
          this.currentlySnappedComponent = this.getComponentWithID(0);
          this.snapCursorToCurrentSnappedComponent();
        }
        else
        {
          if (!this.okDeleteButton.containsPoint(x, y))
            return;
          this.deleting = true;
          if (LocalMultiplayer.IsLocalMultiplayer())
          {
            this.deleteFile(this.selectedForDelete);
            this.deleting = false;
          }
          else
          {
            this._deleteTask = new Task((Action) (() =>
            {
              Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
              this.deleteFile(this.selectedForDelete);
            }));
            Game1.hooks.StartTask(this._deleteTask, "Farm_Delete");
          }
          this.deleteConfirmationScreen = false;
          if (Game1.options.snappyMenus && Game1.options.gamepadControls)
          {
            this.currentlySnappedComponent = this.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
          }
          Game1.playSound("trashcan");
        }
      }
      else
      {
        base.receiveLeftClick(x, y, playSound);
        if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.MenuSlots.Count - 4))
          this.downArrowPressed();
        else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
          this.upArrowPressed();
        else if (this.scrollBar.containsPoint(x, y))
          this.scrolling = true;
        else if (!this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
        {
          this.scrolling = true;
          this.leftClickHeld(x, y);
          this.releaseLeftClick(x, y);
        }
        if (this.selected == -1)
        {
          for (int index = 0; index < this.deleteButtons.Count; ++index)
          {
            if (this.deleteButtons[index].containsPoint(x, y) && index < this.MenuSlots.Count && !this.deleteConfirmationScreen)
            {
              this.deleteConfirmationScreen = true;
              Game1.playSound("drumkit6");
              this.selectedForDelete = this.currentItemIndex + index;
              if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;
              this.currentlySnappedComponent = this.getComponentWithID(803);
              this.snapCursorToCurrentSnappedComponent();
              return;
            }
          }
        }
        if (!this.deleteConfirmationScreen)
        {
          for (int index = 0; index < this.slotButtons.Count; ++index)
          {
            if (this.slotButtons[index].containsPoint(x, y) && index < this.MenuSlots.Count)
            {
              if (this.MenuSlots[this.currentItemIndex + index] is LoadGameMenu.SaveFileSlot menuSlot && menuSlot.versionComparison < 0)
              {
                menuSlot.redTimer = Game1.currentGameTime.TotalGameTime.TotalSeconds + 1.0;
                Game1.playSound("cancel");
              }
              else
              {
                Game1.playSound("select");
                this.timerToLoad = this.MenuSlots[this.currentItemIndex + index].ActivateDelay;
                if (this.timerToLoad > 0)
                {
                  this.loading = true;
                  this.selected = this.currentItemIndex + index;
                  return;
                }
                this.MenuSlots[this.currentItemIndex + index].Activate();
                return;
              }
            }
          }
        }
        this.currentItemIndex = Math.Max(0, Math.Min(this.MenuSlots.Count - 4, this.currentItemIndex));
      }
    }

    protected virtual void saveFileScanComplete()
    {
    }

    protected virtual bool checkListPopulation()
    {
      if (!this.deleteConfirmationScreen)
        ++this._updatesSinceLastDeleteConfirmScreen;
      else
        this._updatesSinceLastDeleteConfirmScreen = 0;
      if (this._initTask == null)
        return false;
      if (this._initTask.IsCanceled || this._initTask.IsCompleted || this._initTask.IsFaulted)
      {
        if (this._initTask.IsCompleted)
        {
          this.addSaveFiles(this._initTask.Result);
          this.saveFileScanComplete();
        }
        this._initTask = (Task<List<Farmer>>) null;
      }
      return true;
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.checkListPopulation())
        return;
      if (this._deleteTask != null)
      {
        if (!this._deleteTask.IsCanceled && !this._deleteTask.IsCompleted && !this._deleteTask.IsFaulted)
          return;
        if (!this._deleteTask.IsCompleted)
          this.selectedForDelete = -1;
        this._deleteTask = (Task) null;
        this.deleting = false;
      }
      else
      {
        if (this.selectedForDelete != -1 && !this.deleteConfirmationScreen && !this.deleting && this.MenuSlots[this.selectedForDelete] is LoadGameMenu.SaveFileSlot menuSlot)
        {
          menuSlot.Farmer.unload();
          this.MenuSlots.RemoveAt(this.selectedForDelete);
          this.selectedForDelete = -1;
          this.slotButtons.Clear();
          this.deleteButtons.Clear();
          for (int index = 0; index < 4; ++index)
          {
            this.slotButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + index * (this.height / 4), this.width - 32, this.height / 4 + 4), index.ToString() ?? ""));
            if (this.hasDeleteButtons())
              this.deleteButtons.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width - 64 - 4, this.yPositionOnScreen + 32 + 4 + index * (this.height / 4), 48, 48), "", "Delete File", Game1.mouseCursors, new Rectangle(322, 498, 12, 12), 3f));
          }
          if (this.MenuSlots.Count <= 4)
          {
            this.currentItemIndex = 0;
            this.setScrollBarToCurrentIndex();
          }
        }
        if (this.timerToLoad <= 0)
          return;
        this.timerToLoad -= time.ElapsedGameTime.Milliseconds;
        if (this.timerToLoad > 0)
          return;
        if (this.MenuSlots.Count > this.selected)
          this.MenuSlots[this.selected].Activate();
        else
          Game1.ExitToTitle();
      }
    }

    protected virtual string getStatusText()
    {
      if (this._initTask != null)
        return Game1.content.LoadString("Strings\\UI:LoadGameMenu_LookingForSavedGames");
      if (this.deleting)
        return Game1.content.LoadString("Strings\\UI:LoadGameMenu_Deleting");
      return this.MenuSlots.Count == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11022") : (string) null;
    }

    protected virtual void drawExtra(SpriteBatch b)
    {
    }

    protected virtual void drawSlotBackground(SpriteBatch b, int i, LoadGameMenu.MenuSlot slot) => IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), this.slotButtons[i].bounds.X, this.slotButtons[i].bounds.Y, this.slotButtons[i].bounds.Width, this.slotButtons[i].bounds.Height, this.currentItemIndex + i == this.selected && this.timerToLoad % 150 > 75 && this.timerToLoad > 1000 || this.selected == -1 && (double) this.slotButtons[i].scale > 1.0 && !this.scrolling && !this.deleteConfirmationScreen ? (this.deleteButtons.Count <= i || !this.deleteButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White) : Color.White, 4f, false);

    protected virtual void drawBefore(SpriteBatch b)
    {
    }

    protected virtual void drawStatusText(SpriteBatch b)
    {
      string statusText = this.getStatusText();
      if (statusText == null)
        return;
      SpriteBatch b1 = b;
      string s = statusText;
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      int x = viewport.Bounds.Center.X;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int y = viewport.Bounds.Center.Y;
      SpriteText.drawStringHorizontallyCenteredAt(b1, s, x, y);
    }

    public override void draw(SpriteBatch b)
    {
      this.drawBefore(b);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height + 32, Color.White, 4f);
      if (this.selectedForDelete == -1 || !this.deleting || this.deleteConfirmationScreen)
      {
        for (int index = 0; index < this.slotButtons.Count; ++index)
        {
          if (this.currentItemIndex + index < this.MenuSlots.Count)
          {
            this.drawSlotBackground(b, index, this.MenuSlots[this.currentItemIndex + index]);
            this.MenuSlots[this.currentItemIndex + index].Draw(b, index);
            if (this.deleteButtons.Count > index)
              this.deleteButtons[index].draw(b, Color.White * 0.75f, 1f);
          }
        }
      }
      this.drawStatusText(b);
      this.upArrow.draw(b);
      this.downArrow.draw(b);
      if (this.MenuSlots.Count > 4)
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f, false);
        this.scrollBar.draw(b);
      }
      if (this.deleteConfirmationScreen && this.MenuSlots[this.selectedForDelete] is LoadGameMenu.SaveFileSlot)
      {
        b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.75f);
        string s = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11023", (object) (this.MenuSlots[this.selectedForDelete] as LoadGameMenu.SaveFileSlot).Farmer.Name);
        int num = this.okDeleteButton.bounds.X + (this.cancelDeleteButton.bounds.X - this.okDeleteButton.bounds.X) / 2 + this.okDeleteButton.bounds.Width / 2;
        SpriteText.drawString(b, s, num - SpriteText.getWidthOfString(s) / 2, (int) Utility.getTopLeftPositionForCenteringOnScreen(192, 64).Y, 9999, height: 9999, layerDepth: 1f, color: 4);
        this.okDeleteButton.draw(b);
        this.cancelDeleteButton.draw(b);
      }
      base.draw(b);
      if (this.hoverText.Length > 0)
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
      this.drawExtra(b);
      if (this.selected != -1 && this.timerToLoad < 1000)
        b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * (float) (1.0 - (double) this.timerToLoad / 1000.0));
      if (Game1.activeClickableMenu == this && (!Game1.options.SnappyMenus || this.currentlySnappedComponent != null) && !this.IsDoingTask())
        this.drawMouse(b);
      this.drawn = true;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        if (this.MenuSlots != null)
        {
          foreach (LoadGameMenu.MenuSlot menuSlot in this.MenuSlots)
            menuSlot.Dispose();
          this.MenuSlots.Clear();
          this.MenuSlots = (List<LoadGameMenu.MenuSlot>) null;
        }
        if (this._initTask != null)
          this._initTask = (Task<List<Farmer>>) null;
        if (this._deleteTask != null)
          this._deleteTask = (Task) null;
      }
      this.disposedValue = true;
    }

    ~LoadGameMenu() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      if (a.region == 901 && b.region != 901 && direction == 2 && b.myID != 81114)
        return true;
      return (a.region != 901 || direction != 3 || b.region == 900) && (direction != 1 || a.region != 900 || !this.hasDeleteButtons() || b.region == 901) && (a.region == 903 || b.region != 903) && (direction != 0 && direction != 2 || a.myID != 81114 || b.region != 902) && base.IsAutomaticSnapValid(direction, a, b);
    }

    protected override bool _ShouldAutoSnapPrioritizeAlignedElements() => false;

    [Conditional("LOG_FS_IO")]
    private static void LogFsio(string format, params object[] args) => Console.WriteLine(format, args);

    public abstract class MenuSlot : IDisposable
    {
      public int ActivateDelay;
      protected LoadGameMenu menu;

      public MenuSlot(LoadGameMenu menu) => this.menu = menu;

      public abstract void Activate();

      public abstract void Draw(SpriteBatch b, int i);

      public virtual void Dispose()
      {
      }
    }

    public class SaveFileSlot : LoadGameMenu.MenuSlot
    {
      public Farmer Farmer;
      public double redTimer;
      public int versionComparison;

      public SaveFileSlot(LoadGameMenu menu, Farmer farmer)
        : base(menu)
      {
        this.ActivateDelay = 2150;
        this.Farmer = farmer;
        this.versionComparison = Utility.CompareGameVersions(Game1.version, farmer.gameVersion, true);
      }

      public override void Activate()
      {
        SaveGame.Load(this.Farmer.slotName);
        Game1.exitActiveMenu();
      }

      protected virtual void drawSlotSaveNumber(SpriteBatch b, int i)
      {
        SpriteBatch b1 = b;
        int num1 = this.menu.currentItemIndex + i + 1;
        string s = num1.ToString() + ".";
        int num2 = this.menu.slotButtons[i].bounds.X + 28 + 32;
        num1 = this.menu.currentItemIndex + i + 1;
        int num3 = SpriteText.getWidthOfString(num1.ToString() + ".") / 2;
        int x = num2 - num3;
        int y = this.menu.slotButtons[i].bounds.Y + 36;
        SpriteText.drawString(b1, s, x, y);
      }

      protected virtual string slotName() => this.Farmer.Name;

      public virtual float getSlotAlpha() => 1f;

      protected virtual void drawSlotName(SpriteBatch b, int i) => SpriteText.drawString(b, this.slotName(), this.menu.slotButtons[i].bounds.X + 128 + 36, this.menu.slotButtons[i].bounds.Y + 36, alpha: this.getSlotAlpha());

      protected virtual void drawSlotShadow(SpriteBatch b, int i)
      {
        Vector2 vector2 = this.portraitOffset();
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = new Vector2((float) ((double) this.menu.slotButtons[i].bounds.X + (double) vector2.X + 32.0), (float) (this.menu.slotButtons[i].bounds.Y + 128 + 16));
        Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        Rectangle bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, 0.8f);
      }

      protected virtual Vector2 portraitOffset() => new Vector2(92f, 20f);

      protected virtual void drawSlotFarmer(SpriteBatch b, int i)
      {
        Vector2 vector2 = this.portraitOffset();
        FarmerRenderer.isDrawingForUI = true;
        this.Farmer.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, 0, false, false), 0, new Rectangle(0, 0, 16, 32), new Vector2((float) this.menu.slotButtons[i].bounds.X + vector2.X, (float) this.menu.slotButtons[i].bounds.Y + vector2.Y), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, this.Farmer);
        FarmerRenderer.isDrawingForUI = false;
      }

      protected virtual void drawSlotDate(SpriteBatch b, int i)
      {
        string text = !this.Farmer.dayOfMonthForSaveGame.HasValue || !this.Farmer.seasonForSaveGame.HasValue || !this.Farmer.yearForSaveGame.HasValue ? this.Farmer.dateStringForSaveGame : Utility.getDateStringFor(this.Farmer.dayOfMonthForSaveGame.Value, this.Farmer.seasonForSaveGame.Value, this.Farmer.yearForSaveGame.Value);
        Utility.drawTextWithShadow(b, text, Game1.dialogueFont, new Vector2((float) (this.menu.slotButtons[i].bounds.X + 128 + 32), (float) (this.menu.slotButtons[i].bounds.Y + 64 + 40)), Game1.textColor * this.getSlotAlpha());
      }

      protected virtual string slotSubName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11019", (object) this.Farmer.farmName);

      protected virtual void drawSlotSubName(SpriteBatch b, int i)
      {
        string text = this.slotSubName();
        Utility.drawTextWithShadow(b, text, Game1.dialogueFont, new Vector2((float) (this.menu.slotButtons[i].bounds.X + this.menu.width - 128) - Game1.dialogueFont.MeasureString(text).X, (float) (this.menu.slotButtons[i].bounds.Y + 44)), Game1.textColor * this.getSlotAlpha());
      }

      protected virtual void drawSlotMoney(SpriteBatch b, int i)
      {
        string text = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) Utility.getNumberWithCommas(this.Farmer.Money));
        if (this.Farmer.Money == 1 && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt)
          text = text.Substring(0, text.Length - 1);
        int x = (int) Game1.dialogueFont.MeasureString(text).X;
        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.menu.slotButtons[i].bounds.X + this.menu.width - 192 - 100 - x), (float) (this.menu.slotButtons[i].bounds.Y + 64 + 44)), new Rectangle(193, 373, 9, 9), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Vector2 position = new Vector2((float) (this.menu.slotButtons[i].bounds.X + this.menu.width - 192 - 60 - x), (float) (this.menu.slotButtons[i].bounds.Y + 64 + 44));
        if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
          position.Y += 5f;
        Utility.drawTextWithShadow(b, text, Game1.dialogueFont, position, Game1.textColor * this.getSlotAlpha());
      }

      protected virtual void drawSlotTimer(SpriteBatch b, int i)
      {
        Vector2 position = new Vector2((float) (this.menu.slotButtons[i].bounds.X + this.menu.width - 192 - 44), (float) (this.menu.slotButtons[i].bounds.Y + 64 + 36));
        Utility.drawWithShadow(b, Game1.mouseCursors, position, new Rectangle(595, 1748, 9, 11), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        position = new Vector2((float) (this.menu.slotButtons[i].bounds.X + this.menu.width - 192 - 4), (float) (this.menu.slotButtons[i].bounds.Y + 64 + 44));
        if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
          position.Y += 5f;
        Utility.drawTextWithShadow(b, Utility.getHoursMinutesStringFromMilliseconds(this.Farmer.millisecondsPlayed), Game1.dialogueFont, position, Game1.textColor * this.getSlotAlpha());
      }

      public virtual void drawVersionMismatchSlot(SpriteBatch b, int i)
      {
        SpriteText.drawString(b, this.slotName(), this.menu.slotButtons[i].bounds.X + 128, this.menu.slotButtons[i].bounds.Y + 36);
        string text1 = this.slotSubName();
        Utility.drawTextWithShadow(b, text1, Game1.dialogueFont, new Vector2((float) (this.menu.slotButtons[i].bounds.X + this.menu.width - 128) - Game1.dialogueFont.MeasureString(text1).X, (float) (this.menu.slotButtons[i].bounds.Y + 44)), Game1.textColor);
        string sub1 = this.Farmer.gameVersion;
        if (sub1 == "-1")
          sub1 = "<1.4";
        string text2 = Game1.content.LoadString("Strings\\UI:VersionMismatch", (object) sub1);
        Color color = Game1.textColor;
        if (Game1.currentGameTime.TotalGameTime.TotalSeconds < this.redTimer && (int) ((this.redTimer - Game1.currentGameTime.TotalGameTime.TotalSeconds) / 0.25) % 2 == 1)
          color = Color.Red;
        Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2((float) (this.menu.slotButtons[i].bounds.X + 128), (float) (this.menu.slotButtons[i].bounds.Y + 64 + 40)), color);
      }

      public override void Draw(SpriteBatch b, int i)
      {
        this.drawSlotSaveNumber(b, i);
        if (this.versionComparison < 0)
        {
          this.drawVersionMismatchSlot(b, i);
        }
        else
        {
          this.drawSlotName(b, i);
          this.drawSlotShadow(b, i);
          this.drawSlotFarmer(b, i);
          this.drawSlotDate(b, i);
          this.drawSlotSubName(b, i);
          this.drawSlotMoney(b, i);
          this.drawSlotTimer(b, i);
        }
      }

      public new void Dispose() => this.Farmer.unload();
    }
  }
}
