// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.JunimoHut
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Buildings
{
  public class JunimoHut : Building
  {
    public const int cropHarvestRadius = 8;
    [XmlElement("output")]
    public readonly NetRef<Chest> output = new NetRef<Chest>();
    [XmlElement("noHarvest")]
    public readonly NetBool noHarvest = new NetBool();
    [XmlElement("wasLit")]
    public readonly NetBool wasLit = new NetBool(false);
    public Rectangle sourceRect;
    private int junimoSendOutTimer;
    [XmlIgnore]
    public List<JunimoHarvester> myJunimos = new List<JunimoHarvester>();
    [XmlIgnore]
    public Point lastKnownCropLocation = Point.Zero;
    [XmlElement("shouldSendOutJunimos")]
    public NetBool shouldSendOutJunimos = new NetBool(false);
    private Rectangle lightInteriorRect = new Rectangle(195, 0, 18, 17);
    private Rectangle bagRect = new Rectangle(208, 51, 15, 13);

    public JunimoHut(BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
      this.sourceRect = this.getSourceRectForMenu();
      this.output.Value = new Chest(true);
    }

    public JunimoHut() => this.sourceRect = this.getSourceRectForMenu();

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.output, (INetSerializable) this.noHarvest, (INetSerializable) this.wasLit, (INetSerializable) this.shouldSendOutJunimos);
      this.wasLit.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((field, old_value, new_value) => this.updateLightState());
    }

    public override Rectangle getRectForAnimalDoor() => new Rectangle((1 + (int) (NetFieldBase<int, NetInt>) this.tileX) * 64, ((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64, 64, 64);

    public override Rectangle getSourceRectForMenu() => new Rectangle(Utility.getSeasonNumber(Game1.currentSeason) * 48, 0, 48, 64);

    public override void load()
    {
      base.load();
      this.sourceRect = this.getSourceRectForMenu();
    }

    public override void dayUpdate(int dayOfMonth)
    {
      base.dayUpdate(dayOfMonth);
      int constructionLeft = (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft;
      this.sourceRect = this.getSourceRectForMenu();
      this.myJunimos.Clear();
      this.wasLit.Value = false;
      this.shouldSendOutJunimos.Value = true;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.isActive() && allFarmer.currentLocation != null && (allFarmer.currentLocation is FarmHouse || allFarmer.currentLocation.isStructure.Value))
          this.shouldSendOutJunimos.Value = false;
      }
    }

    public void sendOutJunimos() => this.junimoSendOutTimer = 1000;

    public override void performActionOnConstruction(GameLocation location)
    {
      base.performActionOnConstruction(location);
      this.sendOutJunimos();
    }

    public override void resetLocalState()
    {
      base.resetLocalState();
      this.updateLightState();
    }

    public void updateLightState()
    {
      if (Game1.currentLocation != Game1.getFarm())
        return;
      if (this.wasLit.Value)
      {
        if (Utility.getLightSource((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tileY * 777) == null)
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1)) * 64f + new Vector2(32f, 32f), 0.5f)
          {
            Identifier = (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tileY * 777
          });
        AmbientLocationSounds.addSound(new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1)), 1);
      }
      else
      {
        Utility.removeLightSource((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tileY * 777);
        AmbientLocationSounds.removeSound(new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1)));
      }
    }

    public int getUnusedJunimoNumber()
    {
      for (int unusedJunimoNumber = 0; unusedJunimoNumber < 3; ++unusedJunimoNumber)
      {
        if (unusedJunimoNumber >= this.myJunimos.Count)
          return unusedJunimoNumber;
        bool flag = false;
        foreach (JunimoHarvester junimo in this.myJunimos)
        {
          if (junimo.whichJunimoFromThisHut == unusedJunimoNumber)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return unusedJunimoNumber;
      }
      return 2;
    }

    public override void updateWhenFarmNotCurrentLocation(GameTime time)
    {
      base.updateWhenFarmNotCurrentLocation(time);
      this.output.Value.mutex.Update((GameLocation) Game1.getFarm());
      if (this.output.Value.mutex.IsLockHeld() && Game1.activeClickableMenu == null)
        this.output.Value.mutex.ReleaseLock();
      if (!Game1.IsMasterGame || this.junimoSendOutTimer <= 0 || !this.shouldSendOutJunimos.Value)
        return;
      this.junimoSendOutTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.junimoSendOutTimer > 0 || this.myJunimos.Count >= 3 || Game1.IsWinter || Game1.isRaining || !this.areThereMatureCropsWithinRadius() || Game1.farmEvent != null)
        return;
      int unusedJunimoNumber = this.getUnusedJunimoNumber();
      bool isPrismatic = false;
      Color? gemColor = this.getGemColor(ref isPrismatic);
      JunimoHarvester junimoHarvester = new JunimoHarvester(new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1)) * 64f + new Vector2(0.0f, 32f), this, unusedJunimoNumber, gemColor);
      junimoHarvester.isPrismatic.Value = isPrismatic;
      Game1.getFarm().characters.Add((NPC) junimoHarvester);
      this.myJunimos.Add(junimoHarvester);
      this.junimoSendOutTimer = 1000;
      if (!Utility.isOnScreen(Utility.Vector2ToPoint(new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1))), 64, (GameLocation) Game1.getFarm()))
        return;
      try
      {
        Game1.getFarm().playSound("junimoMeep1");
      }
      catch (Exception ex)
      {
      }
    }

    public override void Update(GameTime time)
    {
      if (!this.shouldSendOutJunimos.Value)
        this.shouldSendOutJunimos.Value = true;
      base.Update(time);
    }

    private Color? getGemColor(ref bool isPrismatic)
    {
      List<Color> colorList = new List<Color>();
      foreach (Item dye_object in (NetList<Item, NetRef<Item>>) this.output.Value.items)
      {
        if (dye_object != null && (dye_object.Category == -12 || dye_object.Category == -2))
        {
          Color? dyeColor = TailoringMenu.GetDyeColor(dye_object);
          if (dye_object.Name == "Prismatic Shard")
            isPrismatic = true;
          if (dyeColor.HasValue)
            colorList.Add(dyeColor.Value);
        }
      }
      return colorList.Count > 0 ? new Color?(colorList[Game1.random.Next(colorList.Count)]) : new Color?();
    }

    public bool areThereMatureCropsWithinRadius()
    {
      Farm farm = Game1.getFarm();
      for (int index1 = (int) (NetFieldBase<int, NetInt>) this.tileX + 1 - 8; index1 < (int) (NetFieldBase<int, NetInt>) this.tileX + 2 + 8; ++index1)
      {
        for (int index2 = (int) (NetFieldBase<int, NetInt>) this.tileY - 8 + 1; index2 < (int) (NetFieldBase<int, NetInt>) this.tileY + 2 + 8; ++index2)
        {
          if (farm.isCropAtTile(index1, index2) && (farm.terrainFeatures[new Vector2((float) index1, (float) index2)] as HoeDirt).readyForHarvest())
          {
            this.lastKnownCropLocation = new Point(index1, index2);
            return true;
          }
          if (farm.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) index2)) && farm.terrainFeatures[new Vector2((float) index1, (float) index2)] is Bush && (int) (NetFieldBase<int, NetInt>) (farm.terrainFeatures[new Vector2((float) index1, (float) index2)] as Bush).tileSheetOffset == 1)
          {
            this.lastKnownCropLocation = new Point(index1, index2);
            return true;
          }
        }
      }
      this.lastKnownCropLocation = Point.Zero;
      return false;
    }

    public override void performTenMinuteAction(int timeElapsed)
    {
      base.performTenMinuteAction(timeElapsed);
      for (int index = this.myJunimos.Count - 1; index >= 0; --index)
      {
        if (!Game1.getFarm().characters.Contains((NPC) this.myJunimos[index]))
          this.myJunimos.RemoveAt(index);
        else
          this.myJunimos[index].pokeToHarvest();
      }
      if (this.myJunimos.Count < 3 && Game1.timeOfDay < 1900)
        this.junimoSendOutTimer = 1;
      if (Game1.timeOfDay >= 2000 && Game1.timeOfDay < 2400 && !Game1.IsWinter && Game1.random.NextDouble() < 0.2)
      {
        this.wasLit.Value = true;
      }
      else
      {
        if (Game1.timeOfDay != 2400 || Game1.IsWinter)
          return;
        this.wasLit.Value = false;
      }
    }

    public override bool doAction(Vector2 tileLocation, Farmer who)
    {
      if ((double) tileLocation.X < (double) (int) (NetFieldBase<int, NetInt>) this.tileX || (double) tileLocation.X >= (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide) || (double) tileLocation.Y < (double) (int) (NetFieldBase<int, NetInt>) this.tileY || (double) tileLocation.Y >= (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh))
        return base.doAction(tileLocation, who);
      this.output.Value.mutex.RequestLock((Action) (() => Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.output.Value.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.output.Value.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.output.Value.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, whichSpecialButton: 1, context: ((object) this))));
      return true;
    }

    public override List<Item> GetAdditionalItemsToCheckBeforeDemolish() => new List<Item>((IEnumerable<Item>) this.output.Value.items);

    public override void drawInMenu(SpriteBatch b, int x, int y)
    {
      this.drawShadow(b, x, y);
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(0, 0, 48, 64)), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.89f);
    }

    public override void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
      {
        this.drawInConstruction(b);
      }
      else
      {
        this.drawShadow(b);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Rectangle?(this.sourceRect), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) this.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64) / 10000f);
        bool flag = false;
        foreach (Item obj in (NetList<Item, NetRef<Item>>) this.output.Value.items)
        {
          if (obj != null && obj.Category != -12 && obj.Category != -2)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 128 + 12), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 - 32))), new Rectangle?(this.bagRect), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64 + 1) / 10000f);
        if (Game1.timeOfDay < 2000 || Game1.timeOfDay >= 2400 || Game1.IsWinter || !this.wasLit.Value)
          return;
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 - 64))), new Rectangle?(this.lightInteriorRect), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64 + 1) / 10000f);
      }
    }
  }
}
