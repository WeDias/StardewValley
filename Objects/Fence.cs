// Decompiled with JetBrains decompiler
// Type: StardewValley.Fence
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class Fence : Object
  {
    public const int debrisPieces = 4;
    public static int fencePieceWidth = 16;
    public static int fencePieceHeight = 32;
    public const int gateClosedPosition = 0;
    public const int gateOpenedPosition = 88;
    public const int sourceRectForSoloGate = 17;
    public const int globalHealthMultiplier = 2;
    public const int N = 1000;
    public const int E = 100;
    public const int S = 500;
    public const int W = 10;
    public new const int wood = 1;
    public new const int stone = 2;
    public const int steel = 3;
    public const int gate = 4;
    public new const int gold = 5;
    [XmlIgnore]
    public Lazy<Texture2D> fenceTexture;
    public static Dictionary<int, int> fenceDrawGuide;
    [XmlElement("health")]
    public readonly NetFloat health = new NetFloat();
    [XmlElement("maxHealth")]
    public readonly NetFloat maxHealth = new NetFloat();
    [XmlElement("whichType")]
    public readonly NetInt whichType = new NetInt();
    [XmlElement("gatePosition")]
    public readonly NetInt gatePosition = new NetInt();
    public int gateMotion;
    [XmlElement("isGate")]
    public readonly NetBool isGate = new NetBool();
    [XmlIgnore]
    public readonly NetBool repairQueued = new NetBool();

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.health, (INetSerializable) this.maxHealth, (INetSerializable) this.whichType, (INetSerializable) this.gatePosition, (INetSerializable) this.isGate, (INetSerializable) this.repairQueued);
    }

    public Fence(Vector2 tileLocation, int whichType, bool isGate)
      : this()
    {
      this.whichType.Value = whichType;
      this.ResetHealth((float) Game1.random.Next(-100, 101) / 100f);
      this.price.Value = whichType;
      this.isGate.Value = isGate;
      this.tileLocation.Value = tileLocation;
      this.canBeSetDown.Value = true;
      this.canBeGrabbed.Value = true;
      this.price.Value = 1;
      if (isGate)
        this.health.Value *= 2f;
      this.Type = "Crafting";
      this.boundingBox.Value = new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);
    }

    public virtual void ResetHealth(float amount_adjustment)
    {
      float baseHealthForType = (float) this.GetBaseHealthForType((int) (NetFieldBase<int, NetInt>) this.whichType);
      if ((int) (NetFieldBase<int, NetInt>) this.whichType == 4)
        amount_adjustment = 0.0f;
      this.health.Value = baseHealthForType + amount_adjustment;
      switch ((int) (NetFieldBase<int, NetInt>) this.whichType)
      {
        case 1:
          this.name = "Wood Fence";
          this.ParentSheetIndex = -5;
          break;
        case 2:
          this.name = "Stone Fence";
          this.ParentSheetIndex = -6;
          break;
        case 3:
          this.name = "Iron Fence";
          this.ParentSheetIndex = -7;
          break;
        case 4:
          this.name = "Gate";
          this.ParentSheetIndex = -9;
          break;
        case 5:
          this.name = "Hardwood Fence";
          this.ParentSheetIndex = -8;
          break;
      }
      this.health.Value *= 2f;
      this.maxHealth.Value = this.health.Value;
    }

    public virtual int GetBaseHealthForType(int fence_type)
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.whichType)
      {
        case 1:
          return 28;
        case 2:
          return 60;
        case 3:
          return 125;
        case 4:
          return 100;
        case 5:
          return 280;
        default:
          return 100;
      }
    }

    public Fence()
    {
      this.fenceTexture = new Lazy<Texture2D>(new Func<Texture2D>(this.loadFenceTexture));
      if (Fence.fenceDrawGuide == null)
        Fence.populateFenceDrawGuide();
      this.price.Value = 1;
    }

    public virtual void repair() => this.ResetHealth((float) Game1.random.Next(-100, 101) / 100f);

    public static void populateFenceDrawGuide()
    {
      Fence.fenceDrawGuide = new Dictionary<int, int>();
      Fence.fenceDrawGuide.Add(0, 5);
      Fence.fenceDrawGuide.Add(10, 9);
      Fence.fenceDrawGuide.Add(100, 10);
      Fence.fenceDrawGuide.Add(1000, 3);
      Fence.fenceDrawGuide.Add(500, 5);
      Fence.fenceDrawGuide.Add(1010, 8);
      Fence.fenceDrawGuide.Add(1100, 6);
      Fence.fenceDrawGuide.Add(1500, 3);
      Fence.fenceDrawGuide.Add(600, 0);
      Fence.fenceDrawGuide.Add(510, 2);
      Fence.fenceDrawGuide.Add(110, 7);
      Fence.fenceDrawGuide.Add(1600, 0);
      Fence.fenceDrawGuide.Add(1610, 4);
      Fence.fenceDrawGuide.Add(1510, 2);
      Fence.fenceDrawGuide.Add(1110, 7);
      Fence.fenceDrawGuide.Add(610, 4);
    }

    public virtual void PerformRepairIfNecessary()
    {
      if (!Game1.IsMasterGame || !this.repairQueued.Value)
        return;
      this.ResetHealth(this.GetRepairHealthAdjustment());
      this.repairQueued.Value = false;
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      this.PerformRepairIfNecessary();
      int newValue = this.gatePosition.Get() + this.gateMotion;
      if (newValue == 88)
      {
        switch (this.getDrawSum(environment))
        {
          case 10:
          case 100:
          case 110:
          case 500:
          case 1000:
          case 1500:
            break;
          default:
            this.toggleGate(Game1.player, false);
            break;
        }
      }
      this.gatePosition.Set(newValue);
      if (newValue >= 88 || newValue <= 0)
        this.gateMotion = 0;
      this.heldObject.Get()?.updateWhenCurrentLocation(time, environment);
    }

    public int getDrawSum(GameLocation location)
    {
      int drawSum = 0;
      Vector2 tileLocation = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation;
      ++tileLocation.X;
      if (location.objects.ContainsKey(tileLocation) && location.objects[tileLocation] is Fence && ((Fence) location.objects[tileLocation]).countsForDrawing((int) (NetFieldBase<int, NetInt>) this.whichType))
        drawSum += 100;
      tileLocation.X -= 2f;
      if (location.objects.ContainsKey(tileLocation) && location.objects[tileLocation] is Fence && ((Fence) location.objects[tileLocation]).countsForDrawing((int) (NetFieldBase<int, NetInt>) this.whichType))
        drawSum += 10;
      ++tileLocation.X;
      ++tileLocation.Y;
      if (location.objects.ContainsKey(tileLocation) && location.objects[tileLocation] is Fence && ((Fence) location.objects[tileLocation]).countsForDrawing((int) (NetFieldBase<int, NetInt>) this.whichType))
        drawSum += 500;
      tileLocation.Y -= 2f;
      if (location.objects.ContainsKey(tileLocation) && location.objects[tileLocation] is Fence && ((Fence) location.objects[tileLocation]).countsForDrawing((int) (NetFieldBase<int, NetInt>) this.whichType))
        drawSum += 1000;
      return drawSum;
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (!justCheckingForActivity && who != null && who.currentLocation.objects.ContainsKey(new Vector2((float) who.getTileX(), (float) (who.getTileY() - 1))) && who.currentLocation.objects.ContainsKey(new Vector2((float) who.getTileX(), (float) (who.getTileY() + 1))) && who.currentLocation.objects.ContainsKey(new Vector2((float) (who.getTileX() + 1), (float) who.getTileY())) && who.currentLocation.objects.ContainsKey(new Vector2((float) (who.getTileX() - 1), (float) who.getTileY())))
        this.performToolAction((Tool) null, who.currentLocation);
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 1.0)
        return false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.isGate)
      {
        if (justCheckingForActivity)
          return true;
        this.getDrawSum(who.currentLocation);
        if ((bool) (NetFieldBase<bool, NetBool>) this.isGate)
          this.toggleGate(who, (int) (NetFieldBase<int, NetInt>) this.gatePosition == 0);
        return true;
      }
      if (justCheckingForActivity)
        return false;
      foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation))
      {
        if (who.currentLocation.objects.ContainsKey(adjacentTileLocation) && who.currentLocation.objects[adjacentTileLocation] is Fence && (bool) (NetFieldBase<bool, NetBool>) ((Fence) who.currentLocation.objects[adjacentTileLocation]).isGate)
        {
          who.currentLocation.objects[adjacentTileLocation].checkForAction(who);
          return true;
        }
      }
      return (double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0;
    }

    public virtual void toggleGate(
      GameLocation location,
      bool open,
      bool is_toggling_counterpart = false,
      Farmer who = null)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 1.0)
        return;
      int drawSum = this.getDrawSum(location);
      switch (drawSum)
      {
        case 10:
        case 100:
        case 110:
        case 500:
        case 1000:
        case 1500:
          who?.TemporaryPassableTiles.Add(new Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, 64, 64));
          if (open)
            this.gatePosition.Value = 88;
          else
            this.gatePosition.Value = 0;
          if (!is_toggling_counterpart)
          {
            location.playSound("doorClose");
            break;
          }
          break;
        default:
          who?.TemporaryPassableTiles.Add(new Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, 64, 64));
          this.gatePosition.Value = 0;
          break;
      }
      if (is_toggling_counterpart)
        return;
      switch (drawSum)
      {
        case 10:
          Vector2 key1 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(1f, 0.0f);
          if (!Game1.currentLocation.objects.ContainsKey(key1) || !(Game1.currentLocation.objects[key1] is Fence) || !(bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key1]).isGate || ((Fence) Game1.currentLocation.objects[key1]).getDrawSum(Game1.currentLocation) != 100)
            break;
          ((Fence) Game1.currentLocation.objects[key1]).toggleGate(location, (uint) (int) (NetFieldBase<int, NetInt>) this.gatePosition > 0U, true, who);
          break;
        case 100:
          Vector2 key2 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(-1f, 0.0f);
          if (!Game1.currentLocation.objects.ContainsKey(key2) || !(Game1.currentLocation.objects[key2] is Fence) || !(bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key2]).isGate || ((Fence) Game1.currentLocation.objects[key2]).getDrawSum(Game1.currentLocation) != 10)
            break;
          ((Fence) Game1.currentLocation.objects[key2]).toggleGate(location, (uint) (int) (NetFieldBase<int, NetInt>) this.gatePosition > 0U, true, who);
          break;
        case 500:
          Vector2 key3 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(0.0f, -1f);
          if (!Game1.currentLocation.objects.ContainsKey(key3) || !(Game1.currentLocation.objects[key3] is Fence) || !(bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key3]).isGate || ((Fence) Game1.currentLocation.objects[key3]).getDrawSum(Game1.currentLocation) != 1000)
            break;
          ((Fence) Game1.currentLocation.objects[key3]).toggleGate(location, (uint) (int) (NetFieldBase<int, NetInt>) this.gatePosition > 0U, true, who);
          break;
        case 1000:
          Vector2 key4 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(0.0f, 1f);
          if (!Game1.currentLocation.objects.ContainsKey(key4) || !(Game1.currentLocation.objects[key4] is Fence) || !(bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key4]).isGate || ((Fence) Game1.currentLocation.objects[key4]).getDrawSum(Game1.currentLocation) != 500)
            break;
          ((Fence) Game1.currentLocation.objects[key4]).toggleGate(location, (uint) (int) (NetFieldBase<int, NetInt>) this.gatePosition > 0U, true, who);
          break;
      }
    }

    public void toggleGate(Farmer who, bool open, bool is_toggling_counterpart = false) => this.toggleGate(who.currentLocation, open, is_toggling_counterpart, who);

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
    {
      this.ParentSheetIndex = this.GetItemParentSheetIndex();
      base.performRemoveAction(tileLocation, environment);
    }

    public override void dropItem(GameLocation location, Vector2 origin, Vector2 destination) => location.debris.Add(new Debris(this.GetItemParentSheetIndex(), origin, destination));

    public virtual int GetItemParentSheetIndex()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.isGate)
        return 325;
      switch ((int) (NetFieldBase<int, NetInt>) this.whichType)
      {
        case 1:
          return 322;
        case 2:
          return 323;
        case 3:
          return 324;
        case 5:
          return 298;
        default:
          return 322;
      }
    }

    public override bool performToolAction(Tool t, GameLocation location)
    {
      if (this.heldObject.Value != null && t != null && !(t is MeleeWeapon) && t.isHeavyHitter())
      {
        Object @object = this.heldObject.Value;
        this.heldObject.Value.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location);
        this.heldObject.Value = (Object) null;
        Game1.createItemDebris(@object.getOne(), this.TileLocation * 64f, -1);
        location.playSound("axchop");
        return false;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isGate && t != null && (t is Axe || t is Pickaxe))
      {
        location.playSound("axchop");
        Game1.createObjectDebris(325, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.player.UniqueMultiplayerID, Game1.player.currentLocation);
        location.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, 6, false);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
      }
      if (((int) (NetFieldBase<int, NetInt>) this.whichType == 1 || (int) (NetFieldBase<int, NetInt>) this.whichType == 5) && (t == null || t is Axe))
      {
        location.playSound("axchop");
        location.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        for (int index = 0; index < 4; ++index)
          location.temporarySprites.Add((TemporaryAnimatedSprite) new CosmeticDebris(this.fenceTexture.Value, new Vector2((float) ((double) this.tileLocation.X * 64.0 + 32.0), (float) ((double) this.tileLocation.Y * 64.0 + 32.0)), (float) Game1.random.Next(-5, 5) / 100f, (float) Game1.random.Next(-64, 64) / 30f, (float) Game1.random.Next(-800, -100) / 100f, (int) (((double) this.tileLocation.Y + 1.0) * 64.0), new Rectangle(32 + Game1.random.Next(2) * 16 / 2, 96 + Game1.random.Next(2) * 16 / 2, 8, 8), Color.White, Game1.soundBank != null ? Game1.soundBank.GetCue("shiny4") : (ICue) null, (LightSource) null, 0, 200));
        Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, 6, false);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.maxHealth - (double) (float) (NetFieldBase<float, NetFloat>) this.health < 0.5)
        {
          switch ((int) (NetFieldBase<int, NetInt>) this.whichType)
          {
            case 1:
              location.debris.Add(new Debris((Item) new Object(322, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
              break;
            case 5:
              location.debris.Add(new Debris((Item) new Object(298, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
              break;
          }
        }
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.whichType == 2 || (int) (NetFieldBase<int, NetInt>) this.whichType == 3)
      {
        switch (t)
        {
          case null:
          case Pickaxe _:
            location.playSound("hammer");
            location.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
            for (int index = 0; index < 4; ++index)
              location.temporarySprites.Add((TemporaryAnimatedSprite) new CosmeticDebris(this.fenceTexture.Value, new Vector2((float) ((double) this.tileLocation.X * 64.0 + 32.0), (float) ((double) this.tileLocation.Y * 64.0 + 32.0)), (float) Game1.random.Next(-5, 5) / 100f, (float) Game1.random.Next(-64, 64) / 30f, (float) Game1.random.Next(-800, -100) / 100f, (int) (((double) this.tileLocation.Y + 1.0) * 64.0), new Rectangle(32 + Game1.random.Next(2) * 16 / 2, 96 + Game1.random.Next(2) * 16 / 2, 8, 8), Color.White, Game1.soundBank != null ? Game1.soundBank.GetCue("shiny4") : (ICue) null, (LightSource) null, 0, 200));
            Game1.createRadialDebris(location, 14, (int) this.tileLocation.X, (int) this.tileLocation.Y, 6, false);
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
            if ((double) (float) (NetFieldBase<float, NetFloat>) this.maxHealth - (double) (float) (NetFieldBase<float, NetFloat>) this.health < 0.5)
            {
              switch ((int) (NetFieldBase<int, NetInt>) this.whichType)
              {
                case 2:
                  location.debris.Add(new Debris((Item) new Object(323, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
                  break;
                case 3:
                  location.debris.Add(new Debris((Item) new Object(324, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
                  break;
              }
            }
            else
              break;
            break;
        }
      }
      return false;
    }

    public override bool minutesElapsed(int minutes, GameLocation l)
    {
      if (!Game1.IsMasterGame)
        return false;
      this.PerformRepairIfNecessary();
      if (!Game1.getFarm().isBuildingConstructed("Gold Clock"))
      {
        this.health.Value -= (float) minutes / 1440f;
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= -1.0 && (Game1.timeOfDay <= 610 || Game1.timeOfDay > 1800))
          return true;
      }
      return false;
    }

    public override void actionOnPlayerEntry()
    {
      base.actionOnPlayerEntry();
      if (this.heldObject.Value == null)
        return;
      this.heldObject.Value.actionOnPlayerEntry();
      this.heldObject.Value.isOn.Value = true;
      this.heldObject.Value.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
    }

    public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
    {
      if ((int) (NetFieldBase<int, NetInt>) dropIn.parentSheetIndex == 325)
      {
        if (probe)
          return false;
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isGate)
        {
          Vector2 tileLocation = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation;
          int drawSum = this.getDrawSum(who.currentLocation);
          switch (drawSum)
          {
            case 10:
            case 100:
            case 110:
            case 500:
            case 1000:
            case 1500:
              Vector2 key1 = new Vector2();
              if (drawSum == 10)
              {
                key1 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(1f, 0.0f);
                if (Game1.currentLocation.objects.ContainsKey(key1) && Game1.currentLocation.objects[key1] is Fence && (bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key1]).isGate)
                {
                  switch (((Fence) Game1.currentLocation.objects[key1]).getDrawSum(Game1.currentLocation))
                  {
                    case 100:
                    case 110:
                      break;
                    default:
                      return false;
                  }
                }
              }
              else if (drawSum == 100)
              {
                key1 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(-1f, 0.0f);
                if (Game1.currentLocation.objects.ContainsKey(key1) && Game1.currentLocation.objects[key1] is Fence && (bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key1]).isGate)
                {
                  switch (((Fence) Game1.currentLocation.objects[key1]).getDrawSum(Game1.currentLocation))
                  {
                    case 10:
                    case 110:
                      break;
                    default:
                      return false;
                  }
                }
              }
              else if (drawSum == 1000)
              {
                key1 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(0.0f, 1f);
                if (Game1.currentLocation.objects.ContainsKey(key1) && Game1.currentLocation.objects[key1] is Fence && (bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key1]).isGate)
                {
                  switch (((Fence) Game1.currentLocation.objects[key1]).getDrawSum(Game1.currentLocation))
                  {
                    case 500:
                    case 1500:
                      break;
                    default:
                      return false;
                  }
                }
              }
              else if (drawSum == 500)
              {
                key1 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(0.0f, -1f);
                if (Game1.currentLocation.objects.ContainsKey(key1) && Game1.currentLocation.objects[key1] is Fence && (bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key1]).isGate)
                {
                  switch (((Fence) Game1.currentLocation.objects[key1]).getDrawSum(Game1.currentLocation))
                  {
                    case 1000:
                    case 1500:
                      break;
                    default:
                      return false;
                  }
                }
              }
              foreach (Vector2 key2 in new List<Vector2>()
              {
                (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(1f, 0.0f),
                (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(-1f, 0.0f),
                (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(0.0f, -1f),
                (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(0.0f, 1f)
              })
              {
                if (!(key2 == key1) && Game1.currentLocation.objects.ContainsKey(key2) && Game1.currentLocation.objects[key2] is Fence && (bool) (NetFieldBase<bool, NetBool>) ((Fence) Game1.currentLocation.objects[key2]).isGate && Game1.currentLocation.objects[key2].type.Value == this.type.Value)
                  return false;
              }
              this.isGate.Value = true;
              who.currentLocation.playSound("axe");
              return true;
          }
        }
      }
      else if ((int) (NetFieldBase<int, NetInt>) dropIn.parentSheetIndex == 93 && this.heldObject.Value == null && !(bool) (NetFieldBase<bool, NetBool>) this.isGate)
      {
        this.heldObject.Value = (Object) new Torch((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 93);
        this.heldObject.Value.name = "Torch";
        if (!probe)
        {
          who.currentLocation.playSound("axe");
          this.heldObject.Value.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        }
        return true;
      }
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health > 1.0 || this.repairQueued.Value || !this.CanRepairWithThisItem(dropIn))
        return base.performObjectDropInAction(dropIn, probe, who);
      if (probe)
        return true;
      string repairSound = this.GetRepairSound();
      if (repairSound != null && repairSound != "")
        who.currentLocation.playSound(repairSound);
      this.repairQueued.Value = true;
      return true;
    }

    public virtual float GetRepairHealthAdjustment()
    {
      switch (this.whichType.Value)
      {
        case 1:
          return (float) Game1.random.Next(-500, 500) / 100f;
        case 2:
          return (float) Game1.random.Next(-500, 600) / 100f;
        case 3:
          return (float) Game1.random.Next(-500, 700) / 100f;
        case 5:
          return (float) Game1.random.Next(-2000, 2000) / 100f;
        default:
          return 0.0f;
      }
    }

    public virtual string GetRepairSound()
    {
      switch (this.whichType.Value)
      {
        case 1:
          return "axe";
        case 2:
          return "stoneStep";
        case 3:
          return "hammer";
        case 5:
          return "axe";
        default:
          return "";
      }
    }

    public virtual bool CanRepairWithThisItem(Item item) => (double) (float) (NetFieldBase<float, NetFloat>) this.health <= 1.0 && item is Object && ((int) (NetFieldBase<int, NetInt>) this.whichType == 1 && Utility.IsNormalObjectAtParentSheetIndex(item, 322) || (int) (NetFieldBase<int, NetInt>) this.whichType == 2 && Utility.IsNormalObjectAtParentSheetIndex(item, 323) || (int) (NetFieldBase<int, NetInt>) this.whichType == 3 && Utility.IsNormalObjectAtParentSheetIndex(item, 324) || (int) (NetFieldBase<int, NetInt>) this.whichType == 5 && Utility.IsNormalObjectAtParentSheetIndex(item, 298));

    public override bool performDropDownAction(Farmer who)
    {
      this.tileLocation.Value = new Vector2((float) (int) ((double) Game1.player.GetDropLocation().X / 64.0), (float) (int) ((double) Game1.player.GetDropLocation().Y / 64.0));
      return false;
    }

    public virtual Texture2D loadFenceTexture()
    {
      int val2 = this.whichType.Value;
      if (this.whichType.Value == 4)
      {
        val2 = 1;
        this.isGate.Value = true;
      }
      return Game1.content.Load<Texture2D>("LooseSprites\\Fence" + Math.Max(1, val2).ToString());
    }

    public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f) => spriteBatch.Draw(this.fenceTexture.Value, objectPosition - new Vector2(0.0f, 64f), new Rectangle?(new Rectangle(5 * Fence.fencePieceWidth % this.fenceTexture.Value.Bounds.Width, 5 * Fence.fencePieceWidth / this.fenceTexture.Value.Bounds.Width * Fence.fencePieceHeight, Fence.fencePieceWidth, Fence.fencePieceHeight)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) (f.getStandingY() + 1) / 10000f);

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scale,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      location.Y -= 64f * scale;
      int drawSum = this.getDrawSum(Game1.currentLocation);
      int tilePosition = Fence.fenceDrawGuide[drawSum];
      if ((bool) (NetFieldBase<bool, NetBool>) this.isGate)
      {
        if (drawSum == 110)
        {
          spriteBatch.Draw(this.fenceTexture.Value, location + new Vector2(6f, 6f), new Rectangle?(new Rectangle(0, 512, 88, 24)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
          return;
        }
        if (drawSum == 1500)
        {
          spriteBatch.Draw(this.fenceTexture.Value, location + new Vector2(6f, 6f), new Rectangle?(new Rectangle(112, 512, 16, 64)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
          return;
        }
      }
      spriteBatch.Draw(this.fenceTexture.Value, location + new Vector2(32f, 32f) * scale, new Rectangle?(Game1.getArbitrarySourceRect(this.fenceTexture.Value, 64, 128, tilePosition)), color * transparency, 0.0f, new Vector2(32f, 32f) * scale, scale, SpriteEffects.None, layerDepth);
    }

    public bool countsForDrawing(int type)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 1.0 && !this.repairQueued.Value || (bool) (NetFieldBase<bool, NetBool>) this.isGate)
        return false;
      return type == (int) (NetFieldBase<int, NetInt>) this.whichType || type == 4;
    }

    public override bool isPassable() => (bool) (NetFieldBase<bool, NetBool>) this.isGate && (int) (NetFieldBase<int, NetInt>) this.gatePosition >= 88;

    public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
    {
      int num = 1;
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health > 1.0 || this.repairQueued.Value)
      {
        int drawSum = this.getDrawSum(Game1.currentLocation);
        num = Fence.fenceDrawGuide[drawSum];
        if ((bool) (NetFieldBase<bool, NetBool>) this.isGate)
        {
          Vector2 vector2_1 = new Vector2(0.0f, 0.0f);
          Vector2 tileLocation = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation;
          Vector2 vector2_2 = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(-1f, 0.0f);
          switch (drawSum)
          {
            case 10:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2((float) (x * 64 - 16), (float) (y * 64 - 128))), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.gatePosition == 88 ? 24 : 0, 192, 24, 48)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 + 32 + 1) / 10000f);
              return;
            case 100:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2((float) (x * 64 - 16), (float) (y * 64 - 128))), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.gatePosition == 88 ? 24 : 0, 240, 24, 48)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 + 32 + 1) / 10000f);
              return;
            case 110:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2((float) (x * 64 - 16), (float) (y * 64 - 64))), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.gatePosition == 88 ? 24 : 0, 128, 24, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 + 32 + 1) / 10000f);
              return;
            case 500:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2((float) (x * 64 + 20), (float) (y * 64 - 64 - 20))), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.gatePosition == 88 ? 24 : 0, 320, 24, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 + 96 - 1) / 10000f);
              return;
            case 1000:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2((float) (x * 64 + 20), (float) (y * 64 - 64 - 20))), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.gatePosition == 88 ? 24 : 0, 288, 24, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 - 32 + 2) / 10000f);
              return;
            case 1500:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2((float) (x * 64 + 20), (float) (y * 64 - 64 - 20))), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.gatePosition == 88 ? 16 : 0, 160, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 - 32 + 2) / 10000f);
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2((float) (x * 64 + 20), (float) (y * 64 - 64 + 44))), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.gatePosition == 88 ? 16 : 0, 176, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 + 96 - 1) / 10000f);
              return;
            default:
              num = 17;
              break;
          }
        }
        else if (this.heldObject.Value != null)
        {
          Vector2 zero = Vector2.Zero;
          switch (drawSum)
          {
            case 10:
              zero.X = this.whichType.Value != 2 ? (this.whichType.Value != 3 ? 0.0f : 8f) : -4f;
              break;
            case 100:
              zero.X = this.whichType.Value != 2 ? (this.whichType.Value != 3 ? -4f : -8f) : 0.0f;
              break;
          }
          if ((int) (NetFieldBase<int, NetInt>) this.whichType == 2)
            zero.Y = 16f;
          else if ((int) (NetFieldBase<int, NetInt>) this.whichType == 3)
            zero.Y -= 8f;
          if ((int) (NetFieldBase<int, NetInt>) this.whichType == 3)
            zero.X -= 2f;
          this.heldObject.Value.draw(b, x * 64 + (int) zero.X, (y - 1) * 64 - 16 + (int) zero.Y, (float) (y * 64 + 64) / 10000f, 1f);
        }
      }
      b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64))), new Rectangle?(new Rectangle(num * Fence.fencePieceWidth % this.fenceTexture.Value.Bounds.Width, num * Fence.fencePieceWidth / this.fenceTexture.Value.Bounds.Width * Fence.fencePieceHeight, Fence.fencePieceWidth, Fence.fencePieceHeight)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 + 32) / 10000f);
    }
  }
}
