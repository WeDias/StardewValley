// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Furniture
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Furniture : StardewValley.Object, ISittable
  {
    public const int chair = 0;
    public const int bench = 1;
    public const int couch = 2;
    public const int armchair = 3;
    public const int dresser = 4;
    public const int longTable = 5;
    public const int painting = 6;
    public const int lamp = 7;
    public const int decor = 8;
    public const int other = 9;
    public const int bookcase = 10;
    public const int table = 11;
    public const int rug = 12;
    public const int window = 13;
    public const int fireplace = 14;
    public const int bed = 15;
    public const int torch = 16;
    public const int sconce = 17;
    public const string furnitureTextureName = "TileSheets\\furniture";
    public const string furnitureFrontTextureName = "TileSheets\\furnitureFront";
    [XmlIgnore]
    public static Texture2D furnitureTexture;
    [XmlIgnore]
    public static Texture2D furnitureFrontTexture;
    [XmlElement("furniture_type")]
    public readonly NetInt furniture_type = new NetInt();
    [XmlElement("rotations")]
    public readonly NetInt rotations = new NetInt();
    [XmlElement("currentRotation")]
    public readonly NetInt currentRotation = new NetInt();
    [XmlElement("sourceIndexOffset")]
    private readonly NetInt sourceIndexOffset = new NetInt();
    [XmlElement("drawPosition")]
    protected readonly NetVector2 drawPosition = new NetVector2();
    [XmlElement("sourceRect")]
    public readonly NetRectangle sourceRect = new NetRectangle();
    [XmlElement("defaultSourceRect")]
    public readonly NetRectangle defaultSourceRect = new NetRectangle();
    [XmlElement("defaultBoundingBox")]
    public readonly NetRectangle defaultBoundingBox = new NetRectangle();
    [XmlIgnore]
    public bool flaggedForPickUp;
    [XmlElement("drawHeldObjectLow")]
    public readonly NetBool drawHeldObjectLow = new NetBool();
    [XmlIgnore]
    public NetLongDictionary<int, NetInt> sittingFarmers = new NetLongDictionary<int, NetInt>();
    [XmlIgnore]
    public Vector2? lightGlowPosition;
    public static bool isDrawingLocationFurniture;
    [XmlIgnore]
    private int _placementRestriction = -1;
    [XmlIgnore]
    private string _description;
    private const int fireIDBase = 944469;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.furniture_type, (INetSerializable) this.rotations, (INetSerializable) this.currentRotation, (INetSerializable) this.sourceIndexOffset, (INetSerializable) this.drawPosition, (INetSerializable) this.sourceRect, (INetSerializable) this.defaultSourceRect, (INetSerializable) this.defaultBoundingBox, (INetSerializable) this.drawHeldObjectLow, (INetSerializable) this.sittingFarmers);
    }

    [XmlIgnore]
    public int placementRestriction
    {
      get
      {
        if (this._placementRestriction < 0)
        {
          bool flag = true;
          string[] data = this.getData();
          if (data.Length > 6 && int.TryParse(data[6], out this._placementRestriction))
            flag = false;
          if (flag)
            this._placementRestriction = !this.name.Contains("TV") ? (this.furniture_type.Value == 11 || this.furniture_type.Value == 1 || this.furniture_type.Value == 0 || this.furniture_type.Value == 5 || this.furniture_type.Value == 8 || this.furniture_type.Value == 16 ? 2 : 0) : 0;
        }
        return this._placementRestriction;
      }
    }

    [XmlIgnore]
    public string description
    {
      get
      {
        if (this._description == null)
          this._description = this.loadDescription();
        return this._description;
      }
    }

    public Furniture()
    {
      this.updateDrawPosition();
      this.isOn.Value = false;
    }

    public Furniture(int which, Vector2 tile, int initialRotations)
      : this(which, tile)
    {
      for (int index = 0; index < initialRotations; ++index)
        this.rotate();
      this.isOn.Value = false;
    }

    public virtual void OnAdded(GameLocation loc, Vector2 tilePos)
    {
      if (!this.IntersectsForCollision(Game1.player.GetBoundingBox()))
        return;
      Game1.player.TemporaryPassableTiles.Add(this.getBoundingBox(tilePos));
    }

    public Furniture(int which, Vector2 tile)
    {
      this.tileLocation.Value = tile;
      this.isOn.Value = false;
      this.ParentSheetIndex = which;
      string[] data = this.getData();
      this.name = data[0];
      this.furniture_type.Value = this.getTypeNumberFromName(data[1]);
      this.defaultSourceRect.Value = new Rectangle(which * 16 % Furniture.furnitureTexture.Width, which * 16 / Furniture.furnitureTexture.Width * 16, 1, 1);
      this.drawHeldObjectLow.Value = this.Name.ToLower().Contains("tea");
      if (data[2].Equals("-1"))
      {
        this.sourceRect.Value = this.getDefaultSourceRectForType(which, (int) (NetFieldBase<int, NetInt>) this.furniture_type);
        this.defaultSourceRect.Value = this.sourceRect.Value;
      }
      else
      {
        this.defaultSourceRect.Width = Convert.ToInt32(data[2].Split(' ')[0]);
        this.defaultSourceRect.Height = Convert.ToInt32(data[2].Split(' ')[1]);
        this.sourceRect.Value = new Rectangle(which * 16 % Furniture.furnitureTexture.Width, which * 16 / Furniture.furnitureTexture.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
        this.defaultSourceRect.Value = this.sourceRect.Value;
      }
      this.defaultBoundingBox.Value = new Rectangle((int) this.tileLocation.X, (int) this.tileLocation.Y, 1, 1);
      if (data[3].Equals("-1"))
      {
        this.boundingBox.Value = this.getDefaultBoundingBoxForType((int) (NetFieldBase<int, NetInt>) this.furniture_type);
        this.defaultBoundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;
      }
      else
      {
        this.defaultBoundingBox.Width = Convert.ToInt32(data[3].Split(' ')[0]);
        this.defaultBoundingBox.Height = Convert.ToInt32(data[3].Split(' ')[1]);
        this.boundingBox.Value = new Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, this.defaultBoundingBox.Width * 64, this.defaultBoundingBox.Height * 64);
        this.defaultBoundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;
      }
      this.updateDrawPosition();
      this.rotations.Value = Convert.ToInt32(data[4]);
      this.price.Value = Convert.ToInt32(data[5]);
    }

    protected string[] getData()
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
      if (!dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex))
        dictionary = Game1.content.LoadBase<Dictionary<int, string>>("Data\\Furniture");
      return dictionary[(int) (NetFieldBase<int, NetInt>) this.parentSheetIndex].Split('/');
    }

    protected override string loadDisplayName()
    {
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
        return this.name;
      string[] data = this.getData();
      return data[data.Length - 1];
    }

    protected virtual string loadDescription()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1308)
        return Game1.parseText(Game1.content.LoadString("Strings\\Objects:CatalogueDescription"), Game1.smallFont, 320);
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1226)
        return Game1.parseText(Game1.content.LoadString("Strings\\Objects:FurnitureCatalogueDescription"), Game1.smallFont, 320);
      if (this.placementRestriction == 1)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Outdoors_Description");
      return this.placementRestriction == 2 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Decoration_Description") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12623");
    }

    private void specialVariableChange(bool newValue)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type != 14 && (int) (NetFieldBase<int, NetInt>) this.furniture_type != 16 || !newValue)
        return;
      Game1.playSound("fireball");
    }

    public override string getDescription() => Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());

    public override bool performDropDownAction(Farmer who)
    {
      this.resetOnPlayerEntry(who == null ? Game1.currentLocation : who.currentLocation, true);
      return false;
    }

    public override void hoverAction()
    {
      base.hoverAction();
      if (Game1.player.isInventoryFull())
        return;
      Game1.mouseCursor = 2;
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return true;
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 1226:
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAllFurnituresForFree(), context: "Furniture Catalogue");
          return true;
        case 1308:
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAllWallpapersAndFloorsForFree(), context: "Catalogue");
          return true;
        case 1309:
          Game1.playSound("openBox");
          this.shakeTimer = 500;
          if (Game1.getMusicTrackName().Equals("sam_acoustic1"))
            Game1.changeMusicTrack("none", true);
          else
            Game1.changeMusicTrack("sam_acoustic1");
          return true;
        case 1402:
          Game1.activeClickableMenu = (IClickableMenu) new Billboard();
          return true;
        default:
          if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 14 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 16)
          {
            this.isOn.Value = !this.isOn.Value;
            this.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
            this.setFireplace(who.currentLocation, broadcast: true);
            return true;
          }
          if (this.GetSeatCapacity() <= 0)
            return this.clicked(who);
          who.BeginSitting((ISittable) this);
          return true;
      }
    }

    public virtual void setFireplace(GameLocation location, bool playSound = true, bool broadcast = false)
    {
      double x = (double) this.tileLocation.X;
      double y = (double) this.tileLocation.Y;
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOn)
      {
        if (this.lightSource == null)
          this.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        if (this.lightSource != null && (bool) (NetFieldBase<bool, NetBool>) this.isOn && !location.hasLightSource(this.lightSource.Identifier))
          location.sharedLights[(int) (NetFieldBase<int, NetInt>) this.lightSource.identifier] = this.lightSource.Clone();
        if (playSound)
          location.localSound("fireball");
        AmbientLocationSounds.addSound(new Vector2(this.tileLocation.X, this.tileLocation.Y), 1);
      }
      else
      {
        if (playSound)
          location.localSound("fireball");
        base.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location);
        AmbientLocationSounds.removeSound(new Vector2(this.tileLocation.X, this.tileLocation.Y));
      }
    }

    public virtual void AttemptRemoval(Action<Furniture> removal_action)
    {
      if (removal_action == null)
        return;
      removal_action(this);
    }

    public virtual bool canBeRemoved(Farmer who) => !this.HasSittingFarmers() && this.heldObject.Value == null;

    public override bool clicked(Farmer who)
    {
      Game1.haltAfterCheck = false;
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 11 && who.ActiveObject != null && who.ActiveObject != null && this.heldObject.Value == null || this.heldObject.Value == null)
        return false;
      StardewValley.Object @object = this.heldObject.Value;
      this.heldObject.Value = (StardewValley.Object) null;
      if (who.addItemToInventoryBool((Item) @object))
      {
        @object.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, who.currentLocation);
        Game1.playSound("coin");
        return true;
      }
      this.heldObject.Value = @object;
      return false;
    }

    public virtual int GetSeatCapacity()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 0)
        return 1;
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 1)
        return 2;
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 2)
        return this.defaultBoundingBox.Width / 64 - 1;
      return (int) (NetFieldBase<int, NetInt>) this.furniture_type == 3 ? 1 : 0;
    }

    public virtual bool IsSeatHere(GameLocation location) => location.furniture.Contains(this);

    public virtual bool IsSittingHere(Farmer who) => this.sittingFarmers.ContainsKey(who.UniqueMultiplayerID);

    public virtual Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false) => this.sittingFarmers.ContainsKey(who.UniqueMultiplayerID) ? new Vector2?(this.GetSeatPositions(ignore_offsets)[this.sittingFarmers[who.UniqueMultiplayerID]]) : new Vector2?();

    public virtual bool HasSittingFarmers() => this.sittingFarmers.Count() > 0;

    public virtual void RemoveSittingFarmer(Farmer farmer) => this.sittingFarmers.Remove(farmer.UniqueMultiplayerID);

    public virtual int GetSittingFarmerCount() => this.sittingFarmers.Count();

    public virtual Rectangle GetSeatBounds()
    {
      Rectangle boundingBox = this.getBoundingBox(this.TileLocation);
      boundingBox.X /= 64;
      boundingBox.Y /= 64;
      boundingBox.Width /= 64;
      boundingBox.Height /= 64;
      return boundingBox;
    }

    public virtual int GetSittingDirection()
    {
      if (this.Name.Contains("Stool"))
        return Game1.player.FacingDirection;
      if (this.currentRotation.Value == 0)
        return 2;
      if (this.currentRotation.Value == 1)
        return 1;
      if (this.currentRotation.Value == 2)
        return 0;
      return this.currentRotation.Value == 3 ? 3 : 2;
    }

    public virtual Vector2? AddSittingFarmer(Farmer who)
    {
      List<Vector2> seatPositions = this.GetSeatPositions(false);
      int num1 = -1;
      Vector2? nullable = new Vector2?();
      float num2 = 96f;
      for (int index = 0; index < seatPositions.Count; ++index)
      {
        if (!this.sittingFarmers.Values.Contains<int>(index))
        {
          float num3 = ((seatPositions[index] + new Vector2(0.5f, 0.5f)) * 64f - who.getStandingPosition()).Length();
          if ((double) num3 < (double) num2)
          {
            num2 = num3;
            nullable = new Vector2?(seatPositions[index]);
            num1 = index;
          }
        }
      }
      if (nullable.HasValue)
        this.sittingFarmers[who.UniqueMultiplayerID] = num1;
      return nullable;
    }

    public virtual List<Vector2> GetSeatPositions(bool ignore_offsets = false)
    {
      List<Vector2> seatPositions = new List<Vector2>();
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 0)
        seatPositions.Add(this.TileLocation);
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 1)
      {
        for (int x = 0; x < this.getTilesWide(); ++x)
        {
          for (int y = 0; y < this.getTilesHigh(); ++y)
            seatPositions.Add(this.TileLocation + new Vector2((float) x, (float) y));
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 2)
      {
        int num = this.defaultBoundingBox.Width / 64 - 1;
        if ((int) (NetFieldBase<int, NetInt>) this.currentRotation == 0 || (int) (NetFieldBase<int, NetInt>) this.currentRotation == 2)
        {
          seatPositions.Add(this.TileLocation + new Vector2(0.5f, 0.0f));
          for (int index = 1; index < num - 1; ++index)
            seatPositions.Add(this.TileLocation + new Vector2((float) index + 0.5f, 0.0f));
          seatPositions.Add(this.TileLocation + new Vector2((float) (num - 1) + 0.5f, 0.0f));
        }
        else if ((int) (NetFieldBase<int, NetInt>) this.currentRotation == 1)
        {
          for (int y = 0; y < num; ++y)
            seatPositions.Add(this.TileLocation + new Vector2(1f, (float) y));
        }
        else
        {
          for (int y = 0; y < num; ++y)
            seatPositions.Add(this.TileLocation + new Vector2(0.0f, (float) y));
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 3)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.currentRotation == 0 || (int) (NetFieldBase<int, NetInt>) this.currentRotation == 2)
          seatPositions.Add(this.TileLocation + new Vector2(0.5f, 0.0f));
        else if ((int) (NetFieldBase<int, NetInt>) this.currentRotation == 1)
          seatPositions.Add(this.TileLocation + new Vector2(1f, 0.0f));
        else
          seatPositions.Add(this.TileLocation + new Vector2(0.0f, 0.0f));
      }
      return seatPositions;
    }

    public override void DayUpdate(GameLocation location)
    {
      base.DayUpdate(location);
      this.sittingFarmers.Clear();
      if (!Game1.isDarkOut() || Game1.newDay && !Game1.IsRainingHere(location))
        this.removeLights(location);
      else
        this.addLights(location);
      this.RemoveLightGlow(location);
    }

    public virtual void AddLightGlow(GameLocation location)
    {
      if (this.lightGlowPosition.HasValue)
        return;
      Vector2 vector2 = new Vector2((float) (this.boundingBox.X + 32), (float) (this.boundingBox.Y + 64));
      if (location.lightGlows.Contains(vector2))
        return;
      this.lightGlowPosition = new Vector2?(vector2);
      location.lightGlows.Add(vector2);
    }

    public virtual void RemoveLightGlow(GameLocation location)
    {
      if (this.lightGlowPosition.HasValue && location.lightGlows.Contains(this.lightGlowPosition.Value))
        location.lightGlows.Remove(this.lightGlowPosition.Value);
      this.lightGlowPosition = new Vector2?();
    }

    public virtual void resetOnPlayerEntry(GameLocation environment, bool dropDown = false)
    {
      this.isTemporarilyInvisible = false;
      this.RemoveLightGlow(environment);
      this.removeLights(environment);
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 14 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 16)
        this.setFireplace(environment, false);
      if (Game1.isDarkOut())
      {
        this.addLights(environment);
        if (this.heldObject.Value != null && this.heldObject.Value is Furniture)
          (this.heldObject.Value as Furniture).addLights(environment);
      }
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 1971 || dropDown)
        return;
      environment.instantiateCrittersList();
      environment.addCritter((Critter) new Butterfly(environment.getRandomTile()).setStayInbounds(true));
      while (Game1.random.NextDouble() < 0.5)
        environment.addCritter((Critter) new Butterfly(environment.getRandomTile()).setStayInbounds(true));
    }

    public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
    {
      if (!(dropInItem is StardewValley.Object @object) || (int) (NetFieldBase<int, NetInt>) this.furniture_type != 11 && (int) (NetFieldBase<int, NetInt>) this.furniture_type != 5 || this.heldObject.Value != null || (bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable)
        return false;
      switch (@object)
      {
        case Wallpaper _:
label_8:
          return false;
        case Furniture _:
          if ((@object as Furniture).getTilesWide() != 1 || (@object as Furniture).getTilesHigh() != 1)
            goto label_8;
          else
            break;
      }
      this.heldObject.Value = (StardewValley.Object) @object.getOne();
      this.heldObject.Value.tileLocation.Value = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation;
      this.heldObject.Value.boundingBox.X = this.boundingBox.X;
      this.heldObject.Value.boundingBox.Y = this.boundingBox.Y;
      this.heldObject.Value.performDropDownAction(who);
      if (!probe)
      {
        who.currentLocation.playSound("woodyStep");
        who?.reduceActiveItemByOne();
      }
      return true;
    }

    protected virtual int lightSourceIdentifier() => (int) ((double) this.tileLocation.X * 2000.0 + (double) this.tileLocation.Y);

    public virtual void addLights(GameLocation environment)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 7 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 17)
      {
        this.sourceRect.Value = this.defaultSourceRect.Value;
        this.sourceIndexOffset.Value = 1;
        if (this.lightSource != null)
          return;
        environment.removeLightSource(this.lightSourceIdentifier());
        this.lightSource = new LightSource(4, new Vector2((float) (this.boundingBox.X + 32), (float) (this.boundingBox.Y + ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 7 ? -64 : 64))), (int) (NetFieldBase<int, NetInt>) this.furniture_type == 17 ? 1f : 2f, Color.Black, this.lightSourceIdentifier());
        environment.sharedLights[(int) (NetFieldBase<int, NetInt>) this.lightSource.identifier] = this.lightSource.Clone();
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 13)
      {
        this.sourceRect.Value = this.defaultSourceRect.Value;
        this.sourceIndexOffset.Value = 1;
        this.RemoveLightGlow(environment);
      }
      else
      {
        if (!(this is FishTankFurniture) || this.lightSource != null)
          return;
        int num = this.lightSourceIdentifier();
        Vector2 position = new Vector2((float) ((double) this.tileLocation.X * 64.0 + 32.0 + 2.0), (float) ((double) this.tileLocation.Y * 64.0 + 12.0));
        for (int index = 0; index < this.getTilesWide(); ++index)
        {
          environment.removeLightSource(num);
          this.lightSource = new LightSource(8, position, 2f, Color.Black, num);
          environment.sharedLights[num] = this.lightSource.Clone();
          position.X += 64f;
          num += 2000;
        }
      }
    }

    public virtual void removeLights(GameLocation environment)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 7 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 17)
      {
        this.sourceRect.Value = this.defaultSourceRect.Value;
        this.sourceIndexOffset.Value = 0;
        environment.removeLightSource(this.lightSourceIdentifier());
        this.lightSource = (LightSource) null;
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 13)
      {
        if (Game1.IsRainingHere(environment))
        {
          this.sourceRect.Value = this.defaultSourceRect.Value;
          this.sourceIndexOffset.Value = 1;
        }
        else
        {
          this.sourceRect.Value = this.defaultSourceRect.Value;
          this.sourceIndexOffset.Value = 0;
          this.AddLightGlow(environment);
        }
      }
      else
      {
        if (!(this is FishTankFurniture))
          return;
        int identifier = this.lightSourceIdentifier();
        for (int index = 0; index < this.getTilesWide(); ++index)
        {
          environment.removeLightSource(identifier);
          identifier += 2000;
        }
        this.lightSource = (LightSource) null;
      }
    }

    public override bool minutesElapsed(int minutes, GameLocation environment)
    {
      if (Game1.isDarkOut())
      {
        this.addLights(environment);
        if (this.heldObject.Value != null && this.heldObject.Value is Furniture)
          (this.heldObject.Value as Furniture).addLights(environment);
      }
      else
      {
        this.removeLights(environment);
        if (this.heldObject.Value != null && this.heldObject.Value is Furniture)
          (this.heldObject.Value as Furniture).removeLights(environment);
      }
      return false;
    }

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
    {
      this.removeLights(environment);
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 14 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 16)
      {
        this.isOn.Value = false;
        this.setFireplace(environment, false);
      }
      this.RemoveLightGlow(environment);
      base.performRemoveAction(tileLocation, environment);
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 14 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 16)
        this.lightSource = (LightSource) null;
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1309 && Game1.getMusicTrackName().Equals("sam_acoustic1"))
        Game1.changeMusicTrack("none", true);
      this.sittingFarmers.Clear();
    }

    public virtual void rotate()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.rotations < 2)
        return;
      int currentRotation = (int) (NetFieldBase<int, NetInt>) this.currentRotation;
      this.currentRotation.Value += (int) (NetFieldBase<int, NetInt>) this.rotations == 4 ? 1 : 2;
      this.currentRotation.Value %= 4;
      this.updateRotation();
    }

    public virtual void updateRotation()
    {
      this.flipped.Value = false;
      Point point1 = new Point();
      switch ((int) (NetFieldBase<int, NetInt>) this.furniture_type)
      {
        case 2:
          point1.Y = 1;
          point1.X = -1;
          break;
        case 3:
          point1.X = -1;
          point1.Y = 1;
          break;
        case 5:
          point1.Y = 0;
          point1.X = -1;
          break;
        case 12:
          point1.X = 0;
          point1.Y = 0;
          break;
      }
      bool flag1 = (int) (NetFieldBase<int, NetInt>) this.furniture_type == 5 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 12 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 724 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 727;
      bool flag2 = this.defaultBoundingBox.Width != this.defaultBoundingBox.Height;
      if (flag1 && (int) (NetFieldBase<int, NetInt>) this.currentRotation == 2)
        this.currentRotation.Value = 1;
      if (flag2)
      {
        int height = this.boundingBox.Height;
        switch ((int) (NetFieldBase<int, NetInt>) this.currentRotation)
        {
          case 0:
          case 2:
            this.boundingBox.Height = this.defaultBoundingBox.Height;
            this.boundingBox.Width = this.defaultBoundingBox.Width;
            break;
          case 1:
          case 3:
            this.boundingBox.Height = this.boundingBox.Width + point1.X * 64;
            this.boundingBox.Width = height + point1.Y * 64;
            break;
        }
      }
      Point point2 = new Point();
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 12)
      {
        point2.X = 1;
        point2.Y = -1;
      }
      if (flag2)
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.currentRotation)
        {
          case 0:
            this.sourceRect.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.defaultSourceRect;
            break;
          case 1:
            this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Height - 16 + point1.Y * 16 + point2.X * 16, this.defaultSourceRect.Width + 16 + point1.X * 16 + point2.Y * 16);
            break;
          case 2:
            this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width + this.defaultSourceRect.Height - 16 + point1.Y * 16 + point2.X * 16, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
            break;
          case 3:
            this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Height - 16 + point1.Y * 16 + point2.X * 16, this.defaultSourceRect.Width + 16 + point1.X * 16 + point2.Y * 16);
            this.flipped.Value = true;
            break;
        }
      }
      else
      {
        this.flipped.Value = (int) (NetFieldBase<int, NetInt>) this.currentRotation == 3;
        if ((int) (NetFieldBase<int, NetInt>) this.rotations == 2)
          this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + ((int) (NetFieldBase<int, NetInt>) this.currentRotation == 2 ? 1 : 0) * this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
        else
          this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + ((int) (NetFieldBase<int, NetInt>) this.currentRotation == 3 ? 1 : (int) (NetFieldBase<int, NetInt>) this.currentRotation) * this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
      }
      if (flag1 && (int) (NetFieldBase<int, NetInt>) this.currentRotation == 1)
        this.currentRotation.Value = 2;
      this.updateDrawPosition();
    }

    public virtual bool isGroundFurniture() => (int) (NetFieldBase<int, NetInt>) this.furniture_type != 13 && (int) (NetFieldBase<int, NetInt>) this.furniture_type != 6 && (int) (NetFieldBase<int, NetInt>) this.furniture_type != 17 && (int) (NetFieldBase<int, NetInt>) this.furniture_type != 13;

    public override bool canBeGivenAsGift() => false;

    public static Furniture GetFurnitureInstance(int index, Vector2? position = null)
    {
      if (!position.HasValue)
        position = new Vector2?(Vector2.Zero);
      if (index == 1466 || index == 1468 || index == 1680 || index == 2326)
        return (Furniture) new TV(index, position.Value);
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
      if (dictionary.ContainsKey(index))
      {
        string str = dictionary[index].Split('/')[1];
        if (str == "fishtank")
          return (Furniture) new FishTankFurniture(index, position.Value);
        if (str.StartsWith("bed"))
          return (Furniture) new BedFurniture(index, position.Value);
        if (str == "dresser")
          return (Furniture) new StorageFurniture(index, position.Value);
      }
      return new Furniture(index, position.Value);
    }

    public virtual bool IsCloseEnoughToFarmer(Farmer f, int? override_tile_x = null, int? override_tile_y = null)
    {
      Rectangle rectangle = new Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, this.getTilesWide() * 64, this.getTilesHigh() * 64);
      if (override_tile_x.HasValue)
        rectangle.X = override_tile_x.Value * 64;
      if (override_tile_y.HasValue)
        rectangle.Y = override_tile_y.Value * 64;
      rectangle.Inflate(96, 96);
      return rectangle.Contains(Utility.Vector2ToPoint(Game1.player.getStandingPosition()));
    }

    public virtual int GetModifiedWallTilePosition(GameLocation l, int tile_x, int tile_y)
    {
      if (this.isGroundFurniture() || l == null || !(l is DecoratableLocation))
        return tile_y;
      int wallTopY = (l as DecoratableLocation).GetWallTopY(tile_x, tile_y);
      return wallTopY != -1 ? wallTopY : tile_y;
    }

    public override bool canBePlacedHere(GameLocation l, Vector2 tile)
    {
      if (!l.CanPlaceThisFurnitureHere(this))
        return false;
      if (!this.isGroundFurniture())
        tile.Y = (float) this.GetModifiedWallTilePosition(l, (int) tile.X, (int) tile.Y);
      for (int x = 0; x < this.boundingBox.Width / 64; ++x)
      {
        for (int y = 0; y < this.boundingBox.Height / 64; ++y)
        {
          Vector2 vector2 = tile * 64f + new Vector2((float) x, (float) y) * 64f;
          vector2.X += 32f;
          vector2.Y += 32f;
          foreach (Furniture furniture in l.furniture)
          {
            if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 11 && furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Contains((int) vector2.X, (int) vector2.Y) && furniture.heldObject.Value == null && this.getTilesWide() == 1 && this.getTilesHigh() == 1)
              return true;
            if (((int) (NetFieldBase<int, NetInt>) furniture.furniture_type != 12 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 12) && furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Contains((int) vector2.X, (int) vector2.Y) && !furniture.AllowPlacementOnThisTile((int) tile.X + x, (int) tile.Y + y))
              return false;
          }
          Vector2 key = tile + new Vector2((float) x, (float) y);
          if (l.Objects.ContainsKey(key) || l.getLargeTerrainFeatureAt((int) key.X, (int) key.Y) != null || l.terrainFeatures.ContainsKey(key) && l.terrainFeatures[key] is Tree || l.isTerrainFeatureAt((int) key.X, (int) key.Y))
            return false;
        }
      }
      Rectangle rectangle = new Rectangle(this.boundingBox.Value.X, this.boundingBox.Value.Y, this.boundingBox.Value.Width, this.boundingBox.Value.Height);
      rectangle.X = (int) tile.X * 64;
      rectangle.Y = (int) tile.Y * 64;
      if (!this.isPassable())
      {
        foreach (Character farmer in l.farmers)
        {
          if (farmer.GetBoundingBox().Intersects(rectangle))
            return false;
        }
        foreach (Character character in l.characters)
        {
          if (character.GetBoundingBox().Intersects(rectangle))
            return false;
        }
      }
      return this.GetAdditionalFurniturePlacementStatus(l, (int) tile.X * 64, (int) tile.Y * 64) == 0 && base.canBePlacedHere(l, tile);
    }

    public virtual void updateDrawPosition() => this.drawPosition.Value = new Vector2((float) this.boundingBox.X, (float) (this.boundingBox.Y - (this.sourceRect.Height * 4 - this.boundingBox.Height)));

    public virtual int getTilesWide() => this.boundingBox.Width / 64;

    public virtual int getTilesHigh() => this.boundingBox.Height / 64;

    public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      if (!this.isGroundFurniture())
        y = this.GetModifiedWallTilePosition(location, x / 64, y / 64) * 64;
      if (this.GetAdditionalFurniturePlacementStatus(location, x, y, who) != 0)
        return false;
      this.boundingBox.Value = new Rectangle(x / 64 * 64, y / 64 * 64, this.boundingBox.Width, this.boundingBox.Height);
      foreach (Furniture furniture in location.furniture)
      {
        if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 11 && furniture.heldObject.Value == null && furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Intersects((Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox))
        {
          furniture.performObjectDropInAction((Item) this, false, who == null ? Game1.player : who);
          return true;
        }
      }
      this.updateDrawPosition();
      return base.placementAction(location, x, y, who);
    }

    public virtual int GetAdditionalFurniturePlacementStatus(
      GameLocation location,
      int x,
      int y,
      Farmer who = null)
    {
      if (!location.CanPlaceThisFurnitureHere(this))
        return 4;
      Point point = new Point(x / 64, y / 64);
      this.tileLocation.Value = new Vector2((float) point.X, (float) point.Y);
      bool flag1 = false;
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 6 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 17 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 13 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1293)
      {
        int num = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1293 ? 3 : 0;
        bool flag2 = false;
        if (location is DecoratableLocation)
        {
          DecoratableLocation decoratableLocation = location as DecoratableLocation;
          if (((int) (NetFieldBase<int, NetInt>) this.furniture_type == 6 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 17 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 13 || num != 0) && decoratableLocation.isTileOnWall(point.X, point.Y - num) && decoratableLocation.GetWallTopY(point.X, point.Y - num) + num == point.Y)
            flag2 = true;
          else if (!this.isGroundFurniture() && decoratableLocation.isTileOnWall(point.X, point.Y - 1) && decoratableLocation.GetWallTopY(point.X, point.Y) + 1 == point.Y)
            flag2 = true;
        }
        if (!flag2)
          return 1;
        flag1 = true;
      }
      int num1 = this.getTilesHigh();
      if ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 6 && num1 > 2)
        num1 = 2;
      for (int x1 = point.X; x1 < point.X + this.getTilesWide(); ++x1)
      {
        for (int y1 = point.Y; y1 < point.Y + num1; ++y1)
        {
          if (location.doesTileHaveProperty(x1, y1, "NoFurniture", "Back") != null)
            return 2;
          if (!flag1 && location is DecoratableLocation && (location as DecoratableLocation).isTileOnWall(x1, y1))
          {
            if (!(this is BedFurniture) || y1 != point.Y)
              return 3;
          }
          else
          {
            int tileIndexAt = location.getTileIndexAt(x1, y1, "Buildings");
            if (tileIndexAt != -1 && (!(location is IslandFarmHouse) || tileIndexAt < 192 || tileIndexAt > 194 || !(location.getTileSheetIDAt(x1, y1, "Buildings") == "untitled tile sheet")) || location is BuildableGameLocation && (location as BuildableGameLocation).isTileOccupiedForPlacement(new Vector2((float) x1, (float) y1), (StardewValley.Object) this))
              return -1;
          }
        }
      }
      return 0;
    }

    public override bool isPassable() => this.furniture_type.Value == 12 || base.isPassable();

    public override bool isPlaceable() => true;

    public virtual bool AllowPlacementOnThisTile(int tile_x, int tile_y) => false;

    public override Rectangle getBoundingBox(Vector2 tileLocation) => this.isTemporarilyInvisible ? Rectangle.Empty : (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;

    protected virtual Rectangle getDefaultSourceRectForType(int tileIndex, int type)
    {
      int num1;
      int num2;
      switch (type)
      {
        case 0:
          num1 = 1;
          num2 = 2;
          break;
        case 1:
          num1 = 2;
          num2 = 2;
          break;
        case 2:
          num1 = 3;
          num2 = 2;
          break;
        case 3:
          num1 = 2;
          num2 = 2;
          break;
        case 4:
          num1 = 2;
          num2 = 2;
          break;
        case 5:
          num1 = 5;
          num2 = 3;
          break;
        case 6:
          num1 = 2;
          num2 = 2;
          break;
        case 7:
          num1 = 1;
          num2 = 3;
          break;
        case 8:
          num1 = 1;
          num2 = 2;
          break;
        case 10:
          num1 = 2;
          num2 = 3;
          break;
        case 11:
          num1 = 2;
          num2 = 3;
          break;
        case 12:
          num1 = 3;
          num2 = 2;
          break;
        case 13:
          num1 = 1;
          num2 = 2;
          break;
        case 14:
          num1 = 2;
          num2 = 5;
          break;
        case 16:
          num1 = 1;
          num2 = 2;
          break;
        case 17:
          num1 = 1;
          num2 = 2;
          break;
        default:
          num1 = 1;
          num2 = 2;
          break;
      }
      return new Rectangle(tileIndex * 16 % Furniture.furnitureTexture.Width, tileIndex * 16 / Furniture.furnitureTexture.Width * 16, num1 * 16, num2 * 16);
    }

    protected virtual Rectangle getDefaultBoundingBoxForType(int type)
    {
      int num1;
      int num2;
      switch (type)
      {
        case 0:
          num1 = 1;
          num2 = 1;
          break;
        case 1:
          num1 = 2;
          num2 = 1;
          break;
        case 2:
          num1 = 3;
          num2 = 1;
          break;
        case 3:
          num1 = 2;
          num2 = 1;
          break;
        case 4:
          num1 = 2;
          num2 = 1;
          break;
        case 5:
          num1 = 5;
          num2 = 2;
          break;
        case 6:
          num1 = 2;
          num2 = 2;
          break;
        case 7:
          num1 = 1;
          num2 = 1;
          break;
        case 8:
          num1 = 1;
          num2 = 1;
          break;
        case 10:
          num1 = 2;
          num2 = 1;
          break;
        case 11:
          num1 = 2;
          num2 = 2;
          break;
        case 12:
          num1 = 3;
          num2 = 2;
          break;
        case 13:
          num1 = 1;
          num2 = 2;
          break;
        case 14:
          num1 = 2;
          num2 = 1;
          break;
        case 16:
          num1 = 1;
          num2 = 1;
          break;
        case 17:
          num1 = 1;
          num2 = 2;
          break;
        default:
          num1 = 1;
          num2 = 1;
          break;
      }
      return new Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, num1 * 64, num2 * 64);
    }

    private int getTypeNumberFromName(string typeName)
    {
      if (typeName.ToLower().StartsWith("bed"))
        return 15;
      switch (typeName.ToLower())
      {
        case "armchair":
          return 3;
        case "bench":
          return 1;
        case "bookcase":
          return 10;
        case "chair":
          return 0;
        case "couch":
          return 2;
        case "decor":
          return 8;
        case "dresser":
          return 4;
        case "fireplace":
          return 14;
        case "lamp":
          return 7;
        case "long table":
          return 5;
        case "painting":
          return 6;
        case "rug":
          return 12;
        case "sconce":
          return 17;
        case "table":
          return 11;
        case "torch":
          return 16;
        case "window":
          return 13;
        default:
          return 9;
      }
    }

    public override int salePrice() => (int) (NetFieldBase<int, NetInt>) this.price;

    public override int maximumStackSize() => 1;

    public override int addToStack(Item stack) => 1;

    public override string Name => this.name;

    protected virtual float getScaleSize()
    {
      int num1 = this.defaultSourceRect.Width / 16;
      int num2 = this.defaultSourceRect.Height / 16;
      if (num1 >= 7)
        return 0.5f;
      if (num1 >= 6)
        return 0.66f;
      if (num1 >= 5)
        return 0.75f;
      if (num2 >= 5)
        return 0.8f;
      if (num2 >= 3)
        return 1f;
      if (num1 <= 2)
        return 2f;
      return num1 <= 4 ? 1f : 0.1f;
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (Game1.IsMasterGame && this.sittingFarmers.Count() > 0)
      {
        List<long> longList = new List<long>();
        foreach (long key in this.sittingFarmers.Keys)
        {
          if (!Game1.player.team.playerIsOnline(key))
            longList.Add(key);
        }
        foreach (long key in longList)
          this.sittingFarmers.Remove(key);
      }
      if (this.shakeTimer <= 0)
        return;
      this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
    }

    public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
    {
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      spriteBatch.Draw(Furniture.furnitureTexture, location + new Vector2(32f, 32f), new Rectangle?((Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.defaultSourceRect), color * transparency, 0.0f, new Vector2((float) (this.defaultSourceRect.Width / 2), (float) (this.defaultSourceRect.Height / 2)), 1f * this.getScaleSize() * scaleSize, SpriteEffects.None, layerDepth);
      if (((drawStackNumber != StackDrawType.Draw || this.maximumStackSize() <= 1 || this.Stack <= 1) && drawStackNumber != StackDrawType.Draw_OneInclusive || (double) scaleSize <= 0.3 ? 0 : (this.Stack != int.MaxValue ? 1 : 0)) == 0)
        return;
      Utility.drawTinyDigits((int) (NetFieldBase<int, NetInt>) this.stack, spriteBatch, location + new Vector2((float) (64 - Utility.getWidthOfTinyDigitString((int) (NetFieldBase<int, NetInt>) this.stack, 3f * scaleSize)) + 3f * scaleSize, (float) (64.0 - 18.0 * (double) scaleSize + 2.0)), 3f * scaleSize, 1f, color);
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      if (this.isTemporarilyInvisible)
        return;
      Rectangle rectangle = this.sourceRect.Value;
      rectangle.X += rectangle.Width * this.sourceIndexOffset.Value;
      if (Furniture.isDrawingLocationFurniture)
      {
        if (this.HasSittingFarmers() && this.sourceRect.Right <= Furniture.furnitureFrontTexture.Width && this.sourceRect.Bottom <= Furniture.furnitureFrontTexture.Height)
        {
          spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(rectangle), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (this.boundingBox.Value.Top + 16) / 10000f);
          spriteBatch.Draw(Furniture.furnitureFrontTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(rectangle), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (this.boundingBox.Value.Bottom - 8) / 10000f);
        }
        else
          spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(rectangle), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (int) (NetFieldBase<int, NetInt>) this.furniture_type == 12 ? (float) (1.99999994343614E-09 + (double) this.tileLocation.Y / 100000.0) : (float) (this.boundingBox.Value.Bottom - ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 6 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 17 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 13 ? 48 : 8)) / 10000f);
      }
      else
        spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 - (this.sourceRect.Height * 4 - this.boundingBox.Height) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?((Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.sourceRect), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (int) (NetFieldBase<int, NetInt>) this.furniture_type == 12 ? (float) (1.99999994343614E-09 + (double) this.tileLocation.Y / 100000.0) : (float) (this.boundingBox.Value.Bottom - ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 6 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 17 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 13 ? 48 : 8)) / 10000f);
      if (this.heldObject.Value != null)
      {
        if (this.heldObject.Value is Furniture)
        {
          (this.heldObject.Value as Furniture).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32), (float) (this.boundingBox.Center.Y - (this.heldObject.Value as Furniture).sourceRect.Height * 4 - ((bool) (NetFieldBase<bool, NetBool>) this.drawHeldObjectLow ? -16 : 16)))), (float) (this.boundingBox.Bottom - 7) / 10000f, alpha);
        }
        else
        {
          spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32), (float) (this.boundingBox.Center.Y - ((bool) (NetFieldBase<bool, NetBool>) this.drawHeldObjectLow ? 32 : 85)))) + new Vector2(32f, 53f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) this.boundingBox.Bottom / 10000f);
          spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32), (float) (this.boundingBox.Center.Y - ((bool) (NetFieldBase<bool, NetBool>) this.drawHeldObjectLow ? 32 : 85)))), new Rectangle?(GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (this.boundingBox.Bottom + 1) / 10000f);
        }
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOn && (int) (NetFieldBase<int, NetInt>) this.furniture_type == 14)
      {
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 12), (float) (this.boundingBox.Center.Y - 64))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (this.getBoundingBox(new Vector2((float) x, (float) y)).Bottom - 2) / 10000f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32 - 4), (float) (this.boundingBox.Center.Y - 64))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 2047) + (double) (y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (this.getBoundingBox(new Vector2((float) x, (float) y)).Bottom - 1) / 10000f);
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.isOn && (int) (NetFieldBase<int, NetInt>) this.furniture_type == 16)
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 20), (float) this.boundingBox.Center.Y - 105.6f)), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (this.getBoundingBox(new Vector2((float) x, (float) y)).Bottom - 2) / 10000f);
      if (!Game1.debugMode)
        return;
      spriteBatch.DrawString(Game1.smallFont, this.parentSheetIndex?.ToString() ?? "", Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition), Color.Yellow, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }

    public virtual void drawAtNonTileSpot(
      SpriteBatch spriteBatch,
      Vector2 location,
      float layerDepth,
      float alpha = 1f)
    {
      Rectangle rectangle = this.sourceRect.Value;
      rectangle.X += rectangle.Width * (int) (NetFieldBase<int, NetInt>) this.sourceIndexOffset;
      spriteBatch.Draw(Furniture.furnitureTexture, location, new Rectangle?(rectangle), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
    }

    public virtual int GetAdditionalTilePropertyRadius() => 0;

    public virtual bool DoesTileHaveProperty(
      int tile_x,
      int tile_y,
      string property_name,
      string layer_name,
      ref string property_value)
    {
      return false;
    }

    public virtual bool IntersectsForCollision(Rectangle rect) => this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation).Intersects(rect);

    public override Item getOne()
    {
      Furniture one = new Furniture((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      one.drawPosition.Value = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition;
      one.defaultBoundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.defaultBoundingBox;
      one.boundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;
      one.currentRotation.Value = (int) (NetFieldBase<int, NetInt>) this.currentRotation - 1;
      one.isOn.Value = false;
      one.rotations.Value = (int) (NetFieldBase<int, NetInt>) this.rotations;
      one.rotate();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }
  }
}
