// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.CataloguePage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewValley.Menus
{
  public class CataloguePage : IClickableMenu
  {
    public static int widthToMoveActiveTab = 8;
    public static int blueprintButtonMargin = 32;
    public const int buildingsTab = 0;
    public const int upgradesTab = 1;
    public const int animalsTab = 2;
    public const int demolishTab = 3;
    public const int numberOfTabs = 4;
    private string descriptionText = "";
    private string hoverText = "";
    private InventoryMenu inventory;
    private Item heldItem;
    private int currentTab;
    private BluePrint hoveredItem;
    private List<ClickableTextureComponent> sideTabs = new List<ClickableTextureComponent>();
    private List<Dictionary<ClickableComponent, BluePrint>> blueprintButtons = new List<Dictionary<ClickableComponent, BluePrint>>();
    private bool demolishing;
    private bool upgrading;
    private bool placingStructure;
    private BluePrint structureForPlacement;
    private GameMenu parent;
    private Texture2D buildingPlacementTiles;

    public CataloguePage(int x, int y, int width, int height, GameMenu parent)
      : base(x, y, width, height)
    {
      this.parent = parent;
      this.buildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");
      CataloguePage.widthToMoveActiveTab = 8;
      CataloguePage.blueprintButtonMargin = 32;
      this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 320 - 16, false);
      this.sideTabs.Add(new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 48 + CataloguePage.widthToMoveActiveTab, this.yPositionOnScreen + 128, 64, 64), "", "Buildings", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 4), 1f));
      this.sideTabs.Add(new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 192, 64, 64), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10138"), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 5), 1f));
      this.sideTabs.Add(new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 256, 64, 64), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10139"), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 8), 1f));
      this.sideTabs.Add(new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 320, 64, 64), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10140"), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 6), 1f));
      for (int index = 0; index < 4; ++index)
        this.blueprintButtons.Add(new Dictionary<ClickableComponent, BluePrint>());
      int num = 512;
      int[] numArray = new int[4];
      for (int index = 0; index < Game1.player.blueprints.Count; ++index)
      {
        BluePrint structureToPlace = new BluePrint(Game1.player.blueprints[index]);
        if (CataloguePage.canPlaceThisBuildingOnTheCurrentMap(structureToPlace, Game1.currentLocation))
          structureToPlace.canBuildOnCurrentMap = true;
        int tabNumberFromName = this.getTabNumberFromName(structureToPlace.blueprintType);
        if (structureToPlace.blueprintType != null)
        {
          int width1 = (int) ((double) Math.Max(structureToPlace.tilesWidth, 4) / 4.0 * 64.0) + CataloguePage.blueprintButtonMargin;
          if (numArray[tabNumberFromName] % (num - IClickableMenu.borderWidth * 2) + width1 > num - IClickableMenu.borderWidth * 2)
            numArray[tabNumberFromName] += num - IClickableMenu.borderWidth * 2 - numArray[tabNumberFromName] % (num - IClickableMenu.borderWidth * 2);
          this.blueprintButtons[Math.Min(3, tabNumberFromName)].Add(new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(x + IClickableMenu.borderWidth + numArray[tabNumberFromName] % (num - IClickableMenu.borderWidth * 2), y + IClickableMenu.borderWidth + numArray[tabNumberFromName] / (num - IClickableMenu.borderWidth * 2) * 64 * 2 + 64, width1, 128), structureToPlace.name), structureToPlace);
          numArray[tabNumberFromName] += width1;
        }
      }
    }

    public int getTabNumberFromName(string name)
    {
      int tabNumberFromName = -1;
      if (!(name == "Buildings"))
      {
        if (!(name == "Upgrades"))
        {
          if (!(name == "Demolish"))
          {
            if (name == "Animals")
              tabNumberFromName = 2;
          }
          else
            tabNumberFromName = 3;
        }
        else
          tabNumberFromName = 1;
      }
      else
        tabNumberFromName = 0;
      return tabNumberFromName;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!this.placingStructure)
      {
        this.heldItem = this.inventory.leftClick(x, y, this.heldItem);
        for (int index = 0; index < this.sideTabs.Count; ++index)
        {
          if (this.sideTabs[index].containsPoint(x, y) && this.currentTab != index)
          {
            Game1.playSound("smallSelect");
            if (index == 3)
            {
              this.placingStructure = true;
              this.demolishing = true;
              this.parent.invisible = true;
            }
            else
            {
              this.sideTabs[this.currentTab].bounds.X -= CataloguePage.widthToMoveActiveTab;
              this.currentTab = index;
              this.sideTabs[index].bounds.X += CataloguePage.widthToMoveActiveTab;
            }
          }
        }
        foreach (ClickableComponent key in this.blueprintButtons[this.currentTab].Keys)
        {
          if (key.containsPoint(x, y))
          {
            if (this.blueprintButtons[this.currentTab][key].doesFarmerHaveEnoughResourcesToBuild())
            {
              this.structureForPlacement = this.blueprintButtons[this.currentTab][key];
              this.placingStructure = true;
              this.parent.invisible = true;
              if (this.currentTab == 1)
                this.upgrading = true;
              Game1.playSound("smallSelect");
              break;
            }
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10002"), Color.Red, 3500f));
            break;
          }
        }
      }
      else if (this.demolishing)
      {
        if (!(Game1.currentLocation is Farm))
          return;
        if (Game1.IsClient)
        {
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10148"), Color.Red, 3500f));
        }
        else
        {
          Building buildingAt = ((BuildableGameLocation) Game1.currentLocation).getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
          if (Game1.IsMultiplayer && buildingAt != null && buildingAt.indoors.Value.farmers.Any())
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10149"), Color.Red, 3500f));
          else if (buildingAt != null && ((BuildableGameLocation) Game1.currentLocation).destroyStructure(buildingAt))
          {
            int groundLevelTile = (int) (NetFieldBase<int, NetInt>) buildingAt.tileY + (int) (NetFieldBase<int, NetInt>) buildingAt.tilesHigh;
            for (int index = 0; index < buildingAt.texture.Value.Bounds.Height / 64; ++index)
              Game1.createRadialDebris(Game1.currentLocation, buildingAt.textureName(), new Microsoft.Xna.Framework.Rectangle(buildingAt.texture.Value.Bounds.Center.X, buildingAt.texture.Value.Bounds.Center.Y, 4, 4), (int) (NetFieldBase<int, NetInt>) buildingAt.tileX + Game1.random.Next((int) (NetFieldBase<int, NetInt>) buildingAt.tilesWide), (int) (NetFieldBase<int, NetInt>) buildingAt.tileY + (int) (NetFieldBase<int, NetInt>) buildingAt.tilesHigh - index, Game1.random.Next(20, 45), groundLevelTile);
            Game1.playSound("explosion");
            Utility.spreadAnimalsAround(buildingAt, (Farm) Game1.currentLocation);
          }
          else
          {
            this.parent.invisible = false;
            this.placingStructure = false;
            this.demolishing = false;
          }
        }
      }
      else if (this.upgrading && Game1.currentLocation is Farm)
        (Game1.currentLocation as Farm).tryToUpgrade(((BuildableGameLocation) Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64))), this.structureForPlacement);
      else if (!CataloguePage.canPlaceThisBuildingOnTheCurrentMap(this.structureForPlacement, Game1.currentLocation))
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10152"), Color.Red, 3500f));
      else if (!this.structureForPlacement.doesFarmerHaveEnoughResourcesToBuild())
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10002"), Color.Red, 3500f));
      else if (this.tryToBuild())
      {
        this.structureForPlacement.consumeResources();
        if (this.structureForPlacement.blueprintType.Equals("Animals"))
          return;
        Game1.playSound("axe");
      }
      else
      {
        if (Game1.IsClient)
          return;
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10016"), Color.Red, 3500f));
      }
    }

    public static bool canPlaceThisBuildingOnTheCurrentMap(
      BluePrint structureToPlace,
      GameLocation map)
    {
      return true;
    }

    private bool tryToBuild() => this.structureForPlacement.blueprintType.Equals("Animals") ? ((Farm) Game1.getLocationFromName("Farm")).placeAnimal(this.structureForPlacement, new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64)), false, Game1.player.UniqueMultiplayerID) : (Game1.currentLocation as BuildableGameLocation).buildStructure(this.structureForPlacement, new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64)), Game1.player);

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (this.placingStructure)
      {
        this.placingStructure = false;
        this.upgrading = false;
        this.demolishing = false;
        this.parent.invisible = false;
      }
      else
        this.heldItem = this.inventory.rightClick(x, y, this.heldItem);
    }

    public override bool readyToClose() => this.heldItem == null && !this.placingStructure;

    public override void performHoverAction(int x, int y)
    {
      this.descriptionText = "";
      this.hoverText = "";
      foreach (ClickableTextureComponent sideTab in this.sideTabs)
      {
        if (sideTab.containsPoint(x, y))
        {
          this.hoverText = sideTab.hoverText;
          return;
        }
      }
      bool flag = false;
      foreach (ClickableComponent key in this.blueprintButtons[this.currentTab].Keys)
      {
        if (key.containsPoint(x, y))
        {
          key.scale = Math.Min(key.scale + 0.01f, 1.1f);
          this.hoveredItem = this.blueprintButtons[this.currentTab][key];
          flag = true;
        }
        else
          key.scale = Math.Max(key.scale - 0.01f, 1f);
      }
      if (this.demolishing)
      {
        foreach (Building building in ((BuildableGameLocation) Game1.getLocationFromName("Farm")).buildings)
          building.color.Value = Color.White;
        Building buildingAt = ((BuildableGameLocation) Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
        if (buildingAt != null)
          buildingAt.color.Value = Color.Red * 0.8f;
      }
      else if (this.upgrading)
      {
        foreach (Building building in ((BuildableGameLocation) Game1.getLocationFromName("Farm")).buildings)
          building.color.Value = Color.White;
        Building buildingAt = ((BuildableGameLocation) Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
        if (buildingAt != null && this.structureForPlacement.nameOfBuildingToUpgrade != null && this.structureForPlacement.nameOfBuildingToUpgrade.Equals((string) (NetFieldBase<string, NetString>) buildingAt.buildingType))
          buildingAt.color.Value = Color.Green * 0.8f;
        else if (buildingAt != null)
          buildingAt.color.Value = Color.Red * 0.8f;
      }
      if (flag)
        return;
      this.hoveredItem = (BluePrint) null;
    }

    private int getTileSheetIndexForStructurePlacementTile(int x, int y)
    {
      if (x == this.structureForPlacement.humanDoor.X && y == this.structureForPlacement.humanDoor.Y)
        return 2;
      return x == this.structureForPlacement.animalDoor.X && y == this.structureForPlacement.animalDoor.Y ? 4 : 0;
    }

    public override void receiveKeyPress(Keys key)
    {
      if (!Game1.options.doesInputListContain(Game1.options.menuButton, key) || !this.placingStructure)
        return;
      this.placingStructure = false;
      this.upgrading = false;
      this.demolishing = false;
      this.parent.invisible = false;
    }

    public override void draw(SpriteBatch b)
    {
      if (!this.placingStructure)
      {
        foreach (ClickableTextureComponent sideTab in this.sideTabs)
          sideTab.draw(b);
        this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256);
        this.drawVerticalUpperIntersectingPartition(b, this.xPositionOnScreen + 576, 328);
        this.inventory.draw(b);
        foreach (ClickableComponent key in this.blueprintButtons[this.currentTab].Keys)
        {
          Texture2D texture = this.blueprintButtons[this.currentTab][key].texture;
          Vector2 origin = new Vector2((float) this.blueprintButtons[this.currentTab][key].sourceRectForMenuView.Center.X, (float) this.blueprintButtons[this.currentTab][key].sourceRectForMenuView.Center.Y);
          b.Draw(texture, new Vector2((float) key.bounds.Center.X, (float) key.bounds.Center.Y), new Microsoft.Xna.Framework.Rectangle?(this.blueprintButtons[this.currentTab][key].sourceRectForMenuView), this.blueprintButtons[this.currentTab][key].canBuildOnCurrentMap ? Color.White : Color.Gray * 0.8f, 0.0f, origin, (float) (1.0 * (double) key.scale + (this.currentTab == 2 ? 0.75 : 0.0)), SpriteEffects.None, 0.9f);
        }
        if (this.hoveredItem != null)
          this.hoveredItem.drawDescription(b, this.xPositionOnScreen + 576 + 42, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 32, 224);
        if (this.heldItem != null)
          this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
        if (this.hoverText.Equals(""))
          return;
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      }
      else
      {
        if (this.demolishing || this.upgrading)
          return;
        Vector2 vector2_1 = new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64));
        for (int y = 0; y < this.structureForPlacement.tilesHeight; ++y)
        {
          for (int x = 0; x < this.structureForPlacement.tilesWidth; ++x)
          {
            int structurePlacementTile = this.getTileSheetIndexForStructurePlacementTile(x, y);
            Vector2 vector2_2 = new Vector2(vector2_1.X + (float) x, vector2_1.Y + (float) y);
            if (Game1.player.getTileLocation().Equals(vector2_2) || Game1.currentLocation.isTileOccupied(vector2_2) || !Game1.currentLocation.isTilePassable(new Location((int) vector2_2.X, (int) vector2_2.Y), Game1.viewport))
              ++structurePlacementTile;
            b.Draw(this.buildingPlacementTiles, Game1.GlobalToLocal(Game1.viewport, vector2_2 * 64f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(this.buildingPlacementTiles, structurePlacementTile)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
          }
        }
      }
    }
  }
}
