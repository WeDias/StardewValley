// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.BuildingPaintMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class BuildingPaintMenu : IClickableMenu
  {
    public const int region_colorButtons = 1000;
    public const int region_okButton = 101;
    public const int region_nextRegion = 102;
    public const int region_prevRegion = 103;
    public const int region_copyColor = 104;
    public const int region_defaultColor = 105;
    public const int region_hueSlider = 106;
    public const int region_saturationSlider = 107;
    public const int region_lightnessSlider = 108;
    public static int WINDOW_WIDTH = 1024;
    public static int WINDOW_HEIGHT = 576;
    public int maxWidthOfBuildingViewer = 448;
    public int maxHeightOfBuildingViewer = 512;
    public Rectangle previewPane;
    public Rectangle colorPane;
    public BuildingPaintMenu.BuildingColorSlider activeSlider;
    public ClickableTextureComponent okButton;
    public static List<Vector3> savedColors = (List<Vector3>) null;
    public List<Color> buttonColors = new List<Color>();
    public BuildingPaintMenu.ColorSliderPanel colorSliderPanel;
    private string hoverText = "";
    public Building building;
    public Func<Texture2D> getNonBuildingTexture;
    public Rectangle nonBuildingSourceRect;
    public string buildingType = "";
    public BuildingPaintColor colorTarget;
    protected Dictionary<string, string> _paintData;
    public int currentPaintRegion;
    public List<string> regionNames;
    public Dictionary<string, Vector2> regionData;
    public ClickableTextureComponent nextRegionButton;
    public ClickableTextureComponent previousRegionButton;
    public ClickableTextureComponent copyColorButton;
    public ClickableTextureComponent defaultColorButton;
    public List<ClickableTextureComponent> savedColorButtons = new List<ClickableTextureComponent>();
    public List<ClickableComponent> sliderHandles = new List<ClickableComponent>();

    public BuildingPaintMenu(
      string building_type,
      Func<Texture2D> get_non_building_texture,
      Rectangle non_building_source_rect,
      BuildingPaintColor target)
      : base(Game1.uiViewport.Width / 2 - BuildingPaintMenu.WINDOW_WIDTH / 2, Game1.uiViewport.Height / 2 - BuildingPaintMenu.WINDOW_HEIGHT / 2, BuildingPaintMenu.WINDOW_WIDTH, BuildingPaintMenu.WINDOW_HEIGHT)
    {
      this.InitializeSavedColors();
      this._paintData = Game1.content.Load<Dictionary<string, string>>("Data\\PaintData");
      Game1.player.Halt();
      this.building = (Building) null;
      this.buildingType = building_type;
      this.nonBuildingSourceRect = non_building_source_rect;
      this.getNonBuildingTexture = get_non_building_texture;
      this.colorTarget = target;
      this.SetRegion(0);
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public BuildingPaintMenu(Building target_building)
      : base(Game1.uiViewport.Width / 2 - BuildingPaintMenu.WINDOW_WIDTH / 2, Game1.uiViewport.Height / 2 - BuildingPaintMenu.WINDOW_HEIGHT / 2, BuildingPaintMenu.WINDOW_WIDTH, BuildingPaintMenu.WINDOW_HEIGHT)
    {
      this.InitializeSavedColors();
      this._paintData = Game1.content.Load<Dictionary<string, string>>("Data\\PaintData");
      Game1.player.Halt();
      this.building = target_building;
      this.colorTarget = target_building.netBuildingPaintColor.Value;
      this.buildingType = this.building.buildingType.Value;
      this.SetRegion(0);
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public virtual void InitializeSavedColors()
    {
      if (BuildingPaintMenu.savedColors != null)
        return;
      BuildingPaintMenu.savedColors = new List<Vector3>();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(101);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void applyMovementKey(int direction)
    {
      if (this.colorSliderPanel.ApplyMovementKey(direction))
        return;
      base.applyMovementKey(direction);
    }

    public override void receiveGamePadButton(Buttons b)
    {
      switch (b)
      {
        case Buttons.RightTrigger:
          Game1.playSound("shwip");
          this.SetRegion((this.currentPaintRegion + 1 + this.regionNames.Count) % this.regionNames.Count);
          break;
        case Buttons.LeftTrigger:
          Game1.playSound("shwip");
          this.SetRegion((this.currentPaintRegion - 1 + this.regionNames.Count) % this.regionNames.Count);
          break;
      }
      base.receiveGamePadButton(b);
    }

    public override void receiveKeyPress(Keys key) => base.receiveKeyPress(key);

    public override void update(GameTime time)
    {
      if (this.activeSlider != null)
        this.activeSlider.Update(Game1.getMouseX(), Game1.getMouseY());
      base.update(time);
    }

    public override void releaseLeftClick(int x, int y)
    {
      if (this.activeSlider != null)
        this.activeSlider = (BuildingPaintMenu.BuildingColorSlider) null;
      base.releaseLeftClick(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      for (int index = 0; index < this.savedColorButtons.Count; ++index)
      {
        if (this.savedColorButtons[index].containsPoint(x, y))
        {
          BuildingPaintMenu.savedColors.RemoveAt(index);
          this.RepositionElements();
          Game1.playSound("coin");
          return;
        }
      }
      base.receiveRightClick(x, y, playSound);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.colorSliderPanel.ReceiveLeftClick(x, y, playSound))
        return;
      if (this.defaultColorButton.containsPoint(x, y))
      {
        if (this.currentPaintRegion == 0)
          this.colorTarget.Color1Default.Value = true;
        else if (this.currentPaintRegion == 1)
          this.colorTarget.Color2Default.Value = true;
        else
          this.colorTarget.Color3Default.Value = true;
        Game1.playSound("coin");
        this.RepositionElements();
      }
      else
      {
        for (int index = 0; index < this.savedColorButtons.Count; ++index)
        {
          if (this.savedColorButtons[index].containsPoint(x, y))
          {
            this.colorSliderPanel.hueSlider.SetValue((int) BuildingPaintMenu.savedColors[index].X);
            this.colorSliderPanel.saturationSlider.SetValue((int) BuildingPaintMenu.savedColors[index].Y);
            this.colorSliderPanel.lightnessSlider.SetValue((int) Utility.Lerp((float) this.colorSliderPanel.lightnessSlider.min, (float) this.colorSliderPanel.lightnessSlider.max, BuildingPaintMenu.savedColors[index].Z));
            Game1.playSound("coin");
            return;
          }
        }
        if (this.copyColorButton.containsPoint(x, y))
        {
          if (this.SaveColor())
          {
            Game1.playSound("coin");
            this.RepositionElements();
          }
          else
            Game1.playSound("cancel");
        }
        else if (this.okButton.containsPoint(x, y))
          this.exitThisMenu(playSound);
        else if (this.previousRegionButton.containsPoint(x, y))
        {
          Game1.playSound("shwip");
          this.SetRegion((this.currentPaintRegion - 1 + this.regionNames.Count) % this.regionNames.Count);
        }
        else if (this.nextRegionButton.containsPoint(x, y))
        {
          Game1.playSound("shwip");
          this.SetRegion((this.currentPaintRegion + 1) % this.regionNames.Count);
        }
        else
          base.receiveLeftClick(x, y, playSound);
      }
    }

    public override bool overrideSnappyMenuCursorMovementBan() => false;

    public override bool readyToClose() => true;

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      this.okButton.tryHover(x, y);
      this.previousRegionButton.tryHover(x, y);
      this.nextRegionButton.tryHover(x, y);
      this.copyColorButton.tryHover(x, y);
      this.defaultColorButton.tryHover(x, y);
      foreach (ClickableTextureComponent savedColorButton in this.savedColorButtons)
        savedColorButton.tryHover(x, y);
      this.colorSliderPanel.PerformHoverAction(x, y);
    }

    public virtual void RepositionElements()
    {
      this.previewPane.X = this.xPositionOnScreen;
      this.previewPane.Y = this.yPositionOnScreen;
      this.previewPane.Width = 512;
      this.previewPane.Height = 576;
      this.colorPane.Width = 448;
      this.colorPane.X = this.xPositionOnScreen + this.width - this.colorPane.Width;
      this.colorPane.Y = this.yPositionOnScreen;
      this.colorPane.Height = 576;
      Rectangle start_rect = this.colorPane;
      start_rect.Inflate(-32, -32);
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(start_rect.Left, start_rect.Top, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent1.myID = 103;
      textureComponent1.leftNeighborID = -99998;
      textureComponent1.rightNeighborID = -99998;
      textureComponent1.downNeighborID = 105;
      textureComponent1.upNeighborID = -99998;
      textureComponent1.fullyImmutable = true;
      this.previousRegionButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(start_rect.Right - 64, start_rect.Top, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent2.myID = 102;
      textureComponent2.leftNeighborID = -99998;
      textureComponent2.rightNeighborID = -99998;
      textureComponent2.downNeighborID = 105;
      textureComponent2.upNeighborID = -99998;
      textureComponent2.fullyImmutable = true;
      this.nextRegionButton = textureComponent2;
      start_rect.Y += 64;
      start_rect.Height = 0;
      int left = start_rect.Left;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(left, start_rect.Bottom, 64, 64), Game1.mouseCursors2, new Rectangle(80, 144, 16, 16), 4f);
      textureComponent3.region = 1000;
      textureComponent3.myID = 105;
      textureComponent3.upNeighborID = -99998;
      textureComponent3.downNeighborID = -99998;
      textureComponent3.leftNeighborID = -99998;
      textureComponent3.rightNeighborID = -99998;
      textureComponent3.fullyImmutable = true;
      this.defaultColorButton = textureComponent3;
      int x = left + 80;
      this.savedColorButtons.Clear();
      this.buttonColors.Clear();
      for (int index = 0; index < BuildingPaintMenu.savedColors.Count; ++index)
      {
        if (x + 64 > start_rect.X + start_rect.Width)
        {
          x = start_rect.X;
          start_rect.Y += 72;
        }
        ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(x, start_rect.Bottom, 64, 64), Game1.mouseCursors2, new Rectangle(96, 144, 16, 16), 4f);
        textureComponent4.region = 1000;
        textureComponent4.myID = index;
        textureComponent4.upNeighborID = -99998;
        textureComponent4.downNeighborID = -99998;
        textureComponent4.leftNeighborID = -99998;
        textureComponent4.rightNeighborID = -99998;
        textureComponent4.fullyImmutable = true;
        ClickableTextureComponent textureComponent5 = textureComponent4;
        x += 80;
        this.savedColorButtons.Add(textureComponent5);
        Vector3 savedColor = BuildingPaintMenu.savedColors[index];
        int r = 0;
        int g = 0;
        int b = 0;
        Utility.HSLtoRGB((double) savedColor.X, (double) savedColor.Y / 100.0, (double) Utility.Lerp(0.25f, 0.5f, savedColor.Z), out r, out g, out b);
        this.buttonColors.Add(new Color((int) (byte) r, (int) (byte) g, (int) (byte) b));
      }
      if (x + 64 > start_rect.X + start_rect.Width)
      {
        x = start_rect.X;
        start_rect.Y += 72;
      }
      ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Rectangle(x, start_rect.Bottom, 64, 64), Game1.mouseCursors, new Rectangle(274, 284, 16, 16), 4f);
      textureComponent6.region = 1000;
      textureComponent6.myID = 104;
      textureComponent6.upNeighborID = -99998;
      textureComponent6.downNeighborID = -99998;
      textureComponent6.leftNeighborID = -99998;
      textureComponent6.rightNeighborID = -99998;
      textureComponent6.fullyImmutable = true;
      this.copyColorButton = textureComponent6;
      start_rect.Y += 80;
      start_rect = this.colorSliderPanel.Reposition(start_rect);
      start_rect.Y += 64;
      ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(new Rectangle(this.colorPane.Right - 64 - 16, this.colorPane.Bottom - 64 - 16, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent7.myID = 101;
      textureComponent7.upNeighborID = 108;
      this.okButton = textureComponent7;
      this.populateClickableComponentList();
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      if (a.region == 1000 && b.region != 1000)
      {
        switch (direction)
        {
          case 1:
          case 3:
            return false;
          case 2:
            if (b.myID != 106)
              return false;
            break;
        }
      }
      return base.IsAutomaticSnapValid(direction, a, b);
    }

    public virtual bool SaveColor()
    {
      if (this.currentPaintRegion == 0 && this.colorTarget.Color1Default.Value || this.currentPaintRegion == 1 && this.colorTarget.Color2Default.Value || this.currentPaintRegion == 2 && this.colorTarget.Color3Default.Value)
        return false;
      Vector3 vector3 = new Vector3((float) this.colorSliderPanel.hueSlider.GetValue(), (float) this.colorSliderPanel.saturationSlider.GetValue(), (float) (this.colorSliderPanel.lightnessSlider.GetValue() - this.colorSliderPanel.lightnessSlider.min) / (float) (this.colorSliderPanel.lightnessSlider.max - this.colorSliderPanel.lightnessSlider.min));
      if (BuildingPaintMenu.savedColors.Count >= 8)
        BuildingPaintMenu.savedColors.RemoveAt(0);
      BuildingPaintMenu.savedColors.Add(vector3);
      return true;
    }

    public virtual void SetRegion(int new_region)
    {
      if (this.regionData == null)
        this.LoadRegionData();
      if (new_region < this.regionNames.Count && new_region >= 0)
      {
        this.currentPaintRegion = new_region;
        string regionName = this.regionNames[this.currentPaintRegion];
        this.colorSliderPanel = new BuildingPaintMenu.ColorSliderPanel(this, new_region, regionName, (int) this.regionData[regionName].X, (int) this.regionData[regionName].Y);
      }
      this.RepositionElements();
    }

    public virtual void LoadRegionData()
    {
      string str = (string) null;
      if (this.regionData != null)
        return;
      this.regionData = new Dictionary<string, Vector2>();
      this.regionNames = new List<string>();
      if (this._paintData.ContainsKey(this.buildingType))
        str = this._paintData[this.buildingType].Replace("\n", "").Replace("\t", "");
      if (str == null)
        return;
      string[] strArray1 = str.Split('/');
      for (int index = 0; index < strArray1.Length / 2; ++index)
      {
        if (!(strArray1[index].Trim() == ""))
        {
          string key = strArray1[index * 2];
          string[] strArray2 = strArray1[index * 2 + 1].Split(' ');
          int x = -100;
          int y = 100;
          if (strArray2.Length >= 2)
          {
            try
            {
              x = int.Parse(strArray2[0]);
              y = int.Parse(strArray2[1]);
            }
            catch (Exception ex)
            {
            }
          }
          this.regionData[key] = new Vector2((float) x, (float) y);
          this.regionNames.Add(key);
        }
      }
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      Game1.DrawBox(this.previewPane.X, this.previewPane.Y, this.previewPane.Width, this.previewPane.Height);
      Vector2 position = new Vector2((float) (this.previewPane.X + this.previewPane.Width / 2), (float) (this.previewPane.Y + this.previewPane.Height / 2 - 16));
      if (this.building != null)
        this.building.drawInMenu(b, (int) position.X - (int) ((double) (int) (NetFieldBase<int, NetInt>) this.building.tilesWide / 2.0 * 64.0), (int) position.Y - this.building.getSourceRectForMenu().Height * 4 / 2);
      else if (this.getNonBuildingTexture != null)
      {
        Texture2D texture = this.getNonBuildingTexture();
        if (texture != null)
        {
          position = new Vector2((float) (this.previewPane.X + this.previewPane.Width / 2), (float) (this.previewPane.Y + this.previewPane.Height / 2));
          Rectangle buildingSourceRect = this.nonBuildingSourceRect;
          int num1 = this.previewPane.Width / 4 - 4;
          if (buildingSourceRect.Width > num1)
          {
            buildingSourceRect.X = buildingSourceRect.Center.X - num1 / 2;
            buildingSourceRect.Width = num1;
          }
          int num2 = this.previewPane.Height / 4 - 4;
          if (buildingSourceRect.Height > num2)
          {
            buildingSourceRect.Y = buildingSourceRect.Center.Y - num2 / 2;
            buildingSourceRect.Height = num2;
          }
          b.Draw(texture, position, new Rectangle?(buildingSourceRect), Color.White, 0.0f, new Vector2((float) (buildingSourceRect.Width / 2), (float) (buildingSourceRect.Height / 2)), 4f, SpriteEffects.None, 1f);
        }
      }
      Game1.DrawBox(this.colorPane.X, this.colorPane.Y, this.colorPane.Width, this.colorPane.Height);
      string regionName = this.regionNames[this.currentPaintRegion];
      int heightOfString = SpriteText.getHeightOfString(regionName);
      SpriteText.drawStringHorizontallyCenteredAt(b, regionName, this.colorPane.X + this.colorPane.Width / 2, this.nextRegionButton.bounds.Center.Y - heightOfString / 2);
      this.okButton.draw(b);
      this.colorSliderPanel.Draw(b);
      this.nextRegionButton.draw(b);
      this.previousRegionButton.draw(b);
      this.copyColorButton.draw(b);
      this.defaultColorButton.draw(b);
      for (int index = 0; index < this.savedColorButtons.Count; ++index)
        this.savedColorButtons[index].draw(b, this.buttonColors[index], 1f);
      this.drawMouse(b);
    }

    public class ColorSliderPanel
    {
      public BuildingPaintMenu buildingPaintMenu;
      public int regionIndex;
      public string name = "Paint Region Name";
      public Rectangle rectangle;
      public Vector2 labelDrawPosition;
      public Vector2 colorDrawPosition;
      public List<KeyValuePair<string, List<int>>> colors = new List<KeyValuePair<string, List<int>>>();
      public int selectedColor;
      public BuildingPaintMenu.BuildingColorSlider hueSlider;
      public BuildingPaintMenu.BuildingColorSlider saturationSlider;
      public BuildingPaintMenu.BuildingColorSlider lightnessSlider;
      public int minimumBrightness = -100;
      public int maximumBrightness = 100;

      public ColorSliderPanel(
        BuildingPaintMenu menu,
        int region_index,
        string region_name_data,
        int min_brightness = -100,
        int max_brightness = 100)
      {
        this.regionIndex = region_index;
        this.buildingPaintMenu = menu;
        this.name = region_name_data;
        this.minimumBrightness = min_brightness;
        this.maximumBrightness = max_brightness;
      }

      public virtual int GetHeight() => this.rectangle.Height;

      public virtual Rectangle Reposition(Rectangle start_rect)
      {
        this.buildingPaintMenu.sliderHandles.Clear();
        this.rectangle.X = start_rect.X;
        this.rectangle.Y = start_rect.Y;
        this.rectangle.Width = start_rect.Width;
        this.rectangle.Height = 0;
        this.lightnessSlider = (BuildingPaintMenu.BuildingColorSlider) null;
        this.hueSlider = (BuildingPaintMenu.BuildingColorSlider) null;
        this.saturationSlider = (BuildingPaintMenu.BuildingColorSlider) null;
        this.colorDrawPosition = new Vector2((float) (start_rect.X + start_rect.Width - 64), (float) start_rect.Y);
        this.hueSlider = new BuildingPaintMenu.BuildingColorSlider(this.buildingPaintMenu, 106, new Rectangle(this.rectangle.Left, this.rectangle.Bottom, this.rectangle.Width - 100, 12), 0, 360, (Action<int>) (v =>
        {
          if (this.regionIndex == 0)
            this.buildingPaintMenu.colorTarget.Color1Default.Value = false;
          else if (this.regionIndex == 1)
            this.buildingPaintMenu.colorTarget.Color2Default.Value = false;
          else
            this.buildingPaintMenu.colorTarget.Color3Default.Value = false;
          this.ApplyColors();
        }));
        this.hueSlider.getDrawColor += (Func<float, Color>) (val => this.GetColorForValues(val, 100f));
        if (this.regionIndex == 0)
          this.hueSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color1Hue, true);
        else if (this.regionIndex == 1)
          this.hueSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color2Hue, true);
        else
          this.hueSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color3Hue, true);
        this.rectangle.Height += 24;
        this.saturationSlider = new BuildingPaintMenu.BuildingColorSlider(this.buildingPaintMenu, 107, new Rectangle(this.rectangle.Left, this.rectangle.Bottom, this.rectangle.Width - 100, 12), 0, 75, (Action<int>) (v =>
        {
          if (this.regionIndex == 0)
            this.buildingPaintMenu.colorTarget.Color1Default.Value = false;
          else if (this.regionIndex == 1)
            this.buildingPaintMenu.colorTarget.Color2Default.Value = false;
          else
            this.buildingPaintMenu.colorTarget.Color3Default.Value = false;
          this.ApplyColors();
        }));
        this.saturationSlider.getDrawColor += (Func<float, Color>) (val => this.GetColorForValues((float) this.hueSlider.GetValue(), val));
        if (this.regionIndex == 0)
          this.saturationSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color1Saturation, true);
        else if (this.regionIndex == 1)
          this.saturationSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color2Saturation, true);
        else
          this.saturationSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color3Saturation, true);
        this.rectangle.Height += 24;
        this.lightnessSlider = new BuildingPaintMenu.BuildingColorSlider(this.buildingPaintMenu, 108, new Rectangle(this.rectangle.Left, this.rectangle.Bottom, this.rectangle.Width - 100, 12), this.minimumBrightness, this.maximumBrightness, (Action<int>) (v =>
        {
          if (this.regionIndex == 0)
            this.buildingPaintMenu.colorTarget.Color1Default.Value = false;
          else if (this.regionIndex == 1)
            this.buildingPaintMenu.colorTarget.Color2Default.Value = false;
          else
            this.buildingPaintMenu.colorTarget.Color3Default.Value = false;
          this.ApplyColors();
        }));
        this.lightnessSlider.getDrawColor += (Func<float, Color>) (val => this.GetColorForValues((float) this.hueSlider.GetValue(), (float) this.saturationSlider.GetValue(), val));
        if (this.regionIndex == 0)
          this.lightnessSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color1Lightness, true);
        else if (this.regionIndex == 1)
          this.lightnessSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color2Lightness, true);
        else
          this.lightnessSlider.SetValue((int) (NetFieldBase<int, NetInt>) this.buildingPaintMenu.colorTarget.Color3Lightness, true);
        this.rectangle.Height += 24;
        if (this.regionIndex == 0 && this.buildingPaintMenu.colorTarget.Color1Default.Value || this.regionIndex == 1 && this.buildingPaintMenu.colorTarget.Color2Default.Value || this.regionIndex == 2 && this.buildingPaintMenu.colorTarget.Color3Default.Value)
        {
          this.hueSlider.SetValue(this.hueSlider.min, true);
          this.saturationSlider.SetValue(this.saturationSlider.max, true);
          this.lightnessSlider.SetValue((this.lightnessSlider.min + this.lightnessSlider.max) / 2, true);
        }
        this.buildingPaintMenu.sliderHandles.Add((ClickableComponent) this.hueSlider.handle);
        this.buildingPaintMenu.sliderHandles.Add((ClickableComponent) this.saturationSlider.handle);
        this.buildingPaintMenu.sliderHandles.Add((ClickableComponent) this.lightnessSlider.handle);
        this.hueSlider.handle.upNeighborID = 104;
        this.hueSlider.handle.downNeighborID = 107;
        this.saturationSlider.handle.downNeighborID = 108;
        this.saturationSlider.handle.upNeighborID = 106;
        this.lightnessSlider.handle.upNeighborID = 107;
        this.rectangle.Height += 32;
        start_rect.Y += this.rectangle.Height;
        return start_rect;
      }

      public virtual void ApplyColors()
      {
        if (this.regionIndex == 0)
        {
          this.buildingPaintMenu.colorTarget.Color1Hue.Value = this.hueSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color1Saturation.Value = this.saturationSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color1Lightness.Value = this.lightnessSlider.GetValue();
        }
        else if (this.regionIndex == 1)
        {
          this.buildingPaintMenu.colorTarget.Color2Hue.Value = this.hueSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color2Saturation.Value = this.saturationSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color2Lightness.Value = this.lightnessSlider.GetValue();
        }
        else
        {
          this.buildingPaintMenu.colorTarget.Color3Hue.Value = this.hueSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color3Saturation.Value = this.saturationSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color3Lightness.Value = this.lightnessSlider.GetValue();
        }
      }

      public virtual void Draw(SpriteBatch b)
      {
        if ((this.regionIndex != 0 || !(bool) (NetFieldBase<bool, NetBool>) this.buildingPaintMenu.colorTarget.Color1Default) && (this.regionIndex != 1 || !(bool) (NetFieldBase<bool, NetBool>) this.buildingPaintMenu.colorTarget.Color2Default) && (this.regionIndex != 2 || !(bool) (NetFieldBase<bool, NetBool>) this.buildingPaintMenu.colorTarget.Color3Default))
        {
          Color colorForValues = this.GetColorForValues((float) this.hueSlider.GetValue(), (float) this.saturationSlider.GetValue(), (float) this.lightnessSlider.GetValue());
          b.Draw(Game1.staminaRect, new Rectangle((int) this.colorDrawPosition.X - 4, (int) this.colorDrawPosition.Y - 4, 72, 72), new Rectangle?(), Game1.textColor, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
          b.Draw(Game1.staminaRect, new Rectangle((int) this.colorDrawPosition.X, (int) this.colorDrawPosition.Y, 64, 64), new Rectangle?(), colorForValues, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
        if (this.hueSlider != null)
          this.hueSlider.Draw(b);
        if (this.saturationSlider != null)
          this.saturationSlider.Draw(b);
        if (this.lightnessSlider == null)
          return;
        this.lightnessSlider.Draw(b);
      }

      public Color GetColorForValues(float hue_slider, float saturation_slider)
      {
        int r;
        int g;
        int b;
        Utility.HSLtoRGB((double) hue_slider, (double) saturation_slider / 100.0, 0.5, out r, out g, out b);
        return new Color((int) (byte) r, g, b);
      }

      public Color GetColorForValues(
        float hue_slider,
        float saturation_slider,
        float lightness_slider)
      {
        int r;
        int g;
        int b;
        Utility.HSLtoRGB((double) hue_slider, (double) saturation_slider / 100.0, (double) Utility.Lerp(0.25f, 0.5f, (lightness_slider - (float) this.lightnessSlider.min) / (float) (this.lightnessSlider.max - this.lightnessSlider.min)), out r, out g, out b);
        return new Color((int) (byte) r, g, b);
      }

      public virtual bool ApplyMovementKey(int direction)
      {
        if (direction == 3 || direction == 1)
        {
          if (this.saturationSlider.handle == this.buildingPaintMenu.currentlySnappedComponent)
          {
            this.saturationSlider.ApplyMovementKey(direction);
            return true;
          }
          if (this.hueSlider.handle == this.buildingPaintMenu.currentlySnappedComponent)
          {
            this.hueSlider.ApplyMovementKey(direction);
            return true;
          }
          if (this.lightnessSlider.handle == this.buildingPaintMenu.currentlySnappedComponent)
          {
            this.lightnessSlider.ApplyMovementKey(direction);
            return true;
          }
        }
        return false;
      }

      public virtual void PerformHoverAction(int x, int y)
      {
      }

      public virtual bool ReceiveLeftClick(int x, int y, bool play_sound = true)
      {
        if (this.hueSlider != null)
          this.hueSlider.ReceiveLeftClick(x, y);
        if (this.saturationSlider != null)
          this.saturationSlider.ReceiveLeftClick(x, y);
        if (this.lightnessSlider != null)
          this.lightnessSlider.ReceiveLeftClick(x, y);
        return false;
      }
    }

    public class BuildingColorSlider
    {
      public ClickableTextureComponent handle;
      public BuildingPaintMenu buildingPaintMenu;
      public Rectangle bounds;
      protected float _sliderPosition;
      public int min;
      public int max;
      public Action<int> onValueSet;
      public Func<float, Color> getDrawColor;
      protected int _displayedValue;

      public BuildingColorSlider(
        BuildingPaintMenu bpm,
        int handle_id,
        Rectangle bounds,
        int min,
        int max,
        Action<int> on_value_set = null)
      {
        this.handle = new ClickableTextureComponent(new Rectangle(0, 0, 4, 5), Game1.mouseCursors, new Rectangle(72, 256, 16, 20), 1f);
        this.handle.myID = handle_id;
        this.handle.upNeighborID = -99998;
        this.handle.upNeighborImmutable = true;
        this.handle.downNeighborID = -99998;
        this.handle.downNeighborImmutable = true;
        this.handle.leftNeighborImmutable = true;
        this.handle.rightNeighborImmutable = true;
        this.buildingPaintMenu = bpm;
        this.bounds = bounds;
        this.min = min;
        this.max = max;
        this.onValueSet = on_value_set;
      }

      public virtual void ApplyMovementKey(int direction)
      {
        int num = Math.Max((this.max - this.min) / 50, 1);
        if (direction == 3)
          this.SetValue(this._displayedValue - num);
        else
          this.SetValue(this._displayedValue + num);
        if (this.buildingPaintMenu.currentlySnappedComponent != this.handle || !Game1.options.SnappyMenus)
          return;
        this.buildingPaintMenu.snapCursorToCurrentSnappedComponent();
      }

      public virtual void ReceiveLeftClick(int x, int y)
      {
        if (!this.bounds.Contains(x, y))
          return;
        this.buildingPaintMenu.activeSlider = this;
        this.SetValueFromPosition(x, y);
      }

      public virtual void SetValueFromPosition(int x, int y)
      {
        if (this.bounds.Width == 0 || this.min == this.max)
          return;
        float num1 = (float) (x - this.bounds.Left) / (float) this.bounds.Width;
        if ((double) num1 < 0.0)
          num1 = 0.0f;
        if ((double) num1 > 1.0)
          num1 = 1f;
        int num2 = this.max - this.min;
        float num3 = num1 / (float) num2 * (float) num2;
        if ((double) this._sliderPosition == (double) num3)
          return;
        this._sliderPosition = num3;
        this.SetValue(this.min + (int) ((double) this._sliderPosition * (double) num2));
      }

      public void SetValue(int value, bool skip_value_set = false)
      {
        if (value > this.max)
          value = this.max;
        if (value < this.min)
          value = this.min;
        this._sliderPosition = (float) (value - this.min) / (float) (this.max - this.min);
        this.handle.bounds.X = (int) Utility.Lerp((float) this.bounds.Left, (float) this.bounds.Right, this._sliderPosition) - this.handle.bounds.Width / 2 * 4;
        this.handle.bounds.Y = this.bounds.Top - 4;
        if (this._displayedValue == value)
          return;
        this._displayedValue = value;
        if (skip_value_set || this.onValueSet == null)
          return;
        this.onValueSet(value);
      }

      public int GetValue() => this._displayedValue;

      public virtual void Draw(SpriteBatch b)
      {
        int num = 20;
        for (int index = 0; index < num; ++index)
        {
          Rectangle destinationRectangle = new Rectangle((int) ((double) this.bounds.X + (double) this.bounds.Width / (double) num * (double) index), this.bounds.Y, (int) Math.Ceiling((double) this.bounds.Width / (double) num), this.bounds.Height);
          Color color = Color.Black;
          if (this.getDrawColor != null)
            color = this.getDrawColor(Utility.Lerp((float) this.min, (float) this.max, (float) index / (float) num));
          b.Draw(Game1.staminaRect, destinationRectangle, color);
        }
        this.handle.draw(b);
      }

      public virtual void Update(int x, int y) => this.SetValueFromPosition(x, y);
    }
  }
}
