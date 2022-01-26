// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Flooring
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class Flooring : TerrainFeature
  {
    public const byte N = 1;
    public const byte E = 2;
    public const byte S = 4;
    public const byte W = 8;
    public const byte NE = 16;
    public const byte NW = 32;
    public const byte SE = 64;
    public const byte SW = 128;
    public const byte Cardinals = 15;
    public static readonly Vector2 N_Offset = new Vector2(0.0f, -1f);
    public static readonly Vector2 E_Offset = new Vector2(1f, 0.0f);
    public static readonly Vector2 S_Offset = new Vector2(0.0f, 1f);
    public static readonly Vector2 W_Offset = new Vector2(-1f, 0.0f);
    public static readonly Vector2 NE_Offset = new Vector2(1f, -1f);
    public static readonly Vector2 NW_Offset = new Vector2(-1f, -1f);
    public static readonly Vector2 SE_Offset = new Vector2(1f, 1f);
    public static readonly Vector2 SW_Offset = new Vector2(-1f, 1f);
    public const int wood = 0;
    public const int stone = 1;
    public const int ghost = 2;
    public const int iceTile = 3;
    public const int straw = 4;
    public const int gravel = 5;
    public const int boardwalk = 6;
    public const int colored_cobblestone = 7;
    public const int cobblestone = 8;
    public const int steppingStone = 9;
    public const int brick = 10;
    public const int plankFlooring = 11;
    public const int townFlooring = 12;
    public static Texture2D floorsTexture;
    public static Texture2D floorsTextureWinter;
    [InstancedStatic]
    public static Dictionary<byte, int> drawGuide;
    [InstancedStatic]
    public static List<int> drawGuideList;
    [XmlElement("whichFloor")]
    public readonly NetInt whichFloor = new NetInt();
    [XmlElement("whichView")]
    public readonly NetInt whichView = new NetInt();
    [XmlElement("isPathway")]
    public readonly NetBool isPathway = new NetBool();
    [XmlElement("isSteppingStone")]
    public readonly NetBool isSteppingStone = new NetBool();
    [XmlElement("drawContouredShadow")]
    public readonly NetBool drawContouredShadow = new NetBool();
    [XmlElement("cornerDecoratedBorders")]
    public readonly NetBool cornerDecoratedBorders = new NetBool();
    private byte neighborMask;
    private static readonly Flooring.NeighborLoc[] _offsets = new Flooring.NeighborLoc[8]
    {
      new Flooring.NeighborLoc(Flooring.N_Offset, (byte) 1, (byte) 4),
      new Flooring.NeighborLoc(Flooring.S_Offset, (byte) 4, (byte) 1),
      new Flooring.NeighborLoc(Flooring.E_Offset, (byte) 2, (byte) 8),
      new Flooring.NeighborLoc(Flooring.W_Offset, (byte) 8, (byte) 2),
      new Flooring.NeighborLoc(Flooring.NE_Offset, (byte) 16, (byte) 128),
      new Flooring.NeighborLoc(Flooring.NW_Offset, (byte) 32, (byte) 64),
      new Flooring.NeighborLoc(Flooring.SE_Offset, (byte) 64, (byte) 32),
      new Flooring.NeighborLoc(Flooring.SW_Offset, (byte) 128, (byte) 16)
    };
    private List<Flooring.Neighbor> _neighbors = new List<Flooring.Neighbor>();

    public Flooring()
      : base(false)
    {
      this.NetFields.AddFields((INetSerializable) this.whichFloor, (INetSerializable) this.whichView, (INetSerializable) this.isPathway, (INetSerializable) this.isSteppingStone, (INetSerializable) this.drawContouredShadow, (INetSerializable) this.cornerDecoratedBorders);
      this.loadSprite();
      if (Flooring.drawGuide != null)
        return;
      Flooring.populateDrawGuide();
    }

    public Flooring(int which)
      : this()
    {
      this.whichFloor.Value = which;
      this.ApplyFlooringFlags();
    }

    public virtual void ApplyFlooringFlags()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.whichFloor == 5 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 6 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 8 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 7 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 11)
        this.isPathway.Value = true;
      if ((int) (NetFieldBase<int, NetInt>) this.whichFloor == 11 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 12)
        this.drawContouredShadow.Value = true;
      if ((int) (NetFieldBase<int, NetInt>) this.whichFloor == 12)
        this.cornerDecoratedBorders.Value = true;
      if ((int) (NetFieldBase<int, NetInt>) this.whichFloor != 9)
        return;
      this.whichView.Value = Game1.random.Next(16);
      this.isSteppingStone.Value = true;
      this.isPathway.Value = true;
    }

    public override Rectangle getBoundingBox(Vector2 tileLocation) => new Rectangle((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.Y * 64.0), 64, 64);

    public static void populateDrawGuide()
    {
      Flooring.drawGuide = new Dictionary<byte, int>();
      Flooring.drawGuide.Add((byte) 0, 0);
      Flooring.drawGuide.Add((byte) 6, 1);
      Flooring.drawGuide.Add((byte) 14, 2);
      Flooring.drawGuide.Add((byte) 12, 3);
      Flooring.drawGuide.Add((byte) 4, 16);
      Flooring.drawGuide.Add((byte) 7, 17);
      Flooring.drawGuide.Add((byte) 15, 18);
      Flooring.drawGuide.Add((byte) 13, 19);
      Flooring.drawGuide.Add((byte) 5, 32);
      Flooring.drawGuide.Add((byte) 3, 33);
      Flooring.drawGuide.Add((byte) 11, 34);
      Flooring.drawGuide.Add((byte) 9, 35);
      Flooring.drawGuide.Add((byte) 1, 48);
      Flooring.drawGuide.Add((byte) 2, 49);
      Flooring.drawGuide.Add((byte) 10, 50);
      Flooring.drawGuide.Add((byte) 8, 51);
      Flooring.drawGuideList = new List<int>(Flooring.drawGuide.Count);
      foreach (KeyValuePair<byte, int> keyValuePair in Flooring.drawGuide)
        Flooring.drawGuideList.Add(keyValuePair.Value);
    }

    public override void loadSprite()
    {
      if (Flooring.floorsTexture == null)
      {
        try
        {
          Flooring.floorsTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\Flooring");
        }
        catch (Exception ex)
        {
        }
      }
      if (Flooring.floorsTextureWinter == null)
      {
        try
        {
          Flooring.floorsTextureWinter = Game1.content.Load<Texture2D>("TerrainFeatures\\Flooring_winter");
        }
        catch (Exception ex)
        {
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.whichFloor == 5 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 6 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 8 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 7 || (int) (NetFieldBase<int, NetInt>) this.whichFloor == 9)
        this.isPathway.Value = true;
      if ((int) (NetFieldBase<int, NetInt>) this.whichFloor != 9)
        return;
      this.isSteppingStone.Value = true;
    }

    public override void doCollisionAction(
      Rectangle positionOfCollider,
      int speedOfCollision,
      Vector2 tileLocation,
      Character who,
      GameLocation location)
    {
      base.doCollisionAction(positionOfCollider, speedOfCollision, tileLocation, who, location);
      if (who == null || !(who is Farmer) || !(location is Farm))
        return;
      (who as Farmer).temporarySpeedBuff = 0.1f;
    }

    public override bool isPassable(Character c = null) => true;

    public string getFootstepSound()
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.whichFloor)
      {
        case 0:
        case 2:
        case 4:
        case 11:
          return "woodyStep";
        case 1:
        case 10:
        case 12:
          return "stoneStep";
        case 3:
        case 6:
          return "thudStep";
        case 5:
          return "dirtyHit";
        default:
          return "stoneStep";
      }
    }

    private Texture2D getTexture() => Game1.GetSeasonForLocation(this.currentLocation)[0] == 'w' && (this.currentLocation == null || !(bool) (NetFieldBase<bool, NetBool>) this.currentLocation.isGreenhouse) ? Flooring.floorsTextureWinter : Flooring.floorsTexture;

    public override bool performToolAction(
      Tool t,
      int damage,
      Vector2 tileLocation,
      GameLocation location)
    {
      if (location == null)
        location = Game1.currentLocation;
      if (t != null || damage > 0)
      {
        if (damage <= 0)
        {
          switch (t)
          {
            case Pickaxe _:
            case Axe _:
              break;
            default:
              goto label_20;
          }
        }
        Game1.createRadialDebris(location, (int) (NetFieldBase<int, NetInt>) this.whichFloor == 0 ? 12 : 14, (int) tileLocation.X, (int) tileLocation.Y, 4, false);
        int parentSheetIndex = -1;
        switch ((int) (NetFieldBase<int, NetInt>) this.whichFloor)
        {
          case 0:
            location.playSound("axchop");
            parentSheetIndex = 328;
            break;
          case 1:
            location.playSound("hammer");
            parentSheetIndex = 329;
            break;
          case 2:
            location.playSound("axchop");
            parentSheetIndex = 331;
            break;
          case 3:
            location.playSound("hammer");
            parentSheetIndex = 333;
            break;
          case 4:
            location.playSound("axchop");
            parentSheetIndex = 401;
            break;
          case 5:
            location.playSound("hammer");
            parentSheetIndex = 407;
            break;
          case 6:
            location.playSound("axchop");
            parentSheetIndex = 405;
            break;
          case 7:
            location.playSound("hammer");
            parentSheetIndex = 409;
            break;
          case 8:
            location.playSound("hammer");
            parentSheetIndex = 411;
            break;
          case 9:
            location.playSound("hammer");
            parentSheetIndex = 415;
            break;
          case 10:
            location.playSound("hammer");
            parentSheetIndex = 293;
            break;
          case 11:
            location.playSound("axchop");
            parentSheetIndex = 840;
            break;
          case 12:
            location.playSound("hammer");
            parentSheetIndex = 841;
            break;
        }
        location.debris.Add(new Debris((Item) new StardewValley.Object(parentSheetIndex, 1), tileLocation * 64f + new Vector2(32f, 32f)));
        return true;
      }
label_20:
      return false;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 positionOnScreen,
      Vector2 tileLocation,
      float scale,
      float layerDepth)
    {
      int num1 = (int) (NetFieldBase<int, NetInt>) this.whichFloor * 4 * 64;
      byte key1 = 0;
      Vector2 key2 = tileLocation;
      ++key2.X;
      GameLocation locationFromName = Game1.getLocationFromName("Farm");
      if (locationFromName.terrainFeatures.ContainsKey(key2) && locationFromName.terrainFeatures[key2] is Flooring)
        key1 += (byte) 2;
      key2.X -= 2f;
      if (locationFromName.terrainFeatures.ContainsKey(key2) && Game1.currentLocation.terrainFeatures[key2] is Flooring)
        key1 += (byte) 8;
      ++key2.X;
      ++key2.Y;
      if (Game1.currentLocation.terrainFeatures.ContainsKey(key2) && locationFromName.terrainFeatures[key2] is Flooring)
        key1 += (byte) 4;
      key2.Y -= 2f;
      if (locationFromName.terrainFeatures.ContainsKey(key2) && locationFromName.terrainFeatures[key2] is Flooring)
        ++key1;
      int num2 = Flooring.drawGuide[key1];
      spriteBatch.Draw(Flooring.floorsTexture, positionOnScreen, new Rectangle?(new Rectangle(num2 % 16 * 16, num2 / 16 * 16 + num1, 16, 16)), Color.White, 0.0f, Vector2.Zero, scale * 4f, SpriteEffects.None, layerDepth + positionOnScreen.Y / 20000f);
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
      if (this.cornerDecoratedBorders.Value)
      {
        int num = 6;
        if (((int) this.neighborMask & 9) == 9 && ((int) this.neighborMask & 32) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(new Rectangle(64 - num + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), 48 - num + (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, num, num)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0) / 20000.0));
        if (((int) this.neighborMask & 3) == 3 && ((int) this.neighborMask & 16) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 64.0) - (float) (num * 4), tileLocation.Y * 64f)), new Rectangle?(new Rectangle(16 + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), 48 - num + (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, num, num)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0 + (double) (int) (NetFieldBase<int, NetInt>) this.whichFloor) / 20000.0));
        if (((int) this.neighborMask & 6) == 6 && ((int) this.neighborMask & 64) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 64.0) - (float) (num * 4), (float) ((double) tileLocation.Y * 64.0 + 64.0) - (float) (num * 4))), new Rectangle?(new Rectangle(16 + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, num, num)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0) / 20000.0));
        if (((int) this.neighborMask & 12) == 12 && ((int) this.neighborMask & 128) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, (float) ((double) tileLocation.Y * 64.0 + 64.0) - (float) (num * 4))), new Rectangle?(new Rectangle(64 - num + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, num, num)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0) / 20000.0));
      }
      else if (!(bool) (NetFieldBase<bool, NetBool>) this.isPathway)
      {
        if (((int) this.neighborMask & 9) == 9 && ((int) this.neighborMask & 32) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(new Rectangle(60 + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), 44 + (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, 4, 4)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0) / 20000.0));
        if (((int) this.neighborMask & 3) == 3 && ((int) this.neighborMask & 16) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 48.0), tileLocation.Y * 64f)), new Rectangle?(new Rectangle(16 + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), 44 + (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, 4, 4)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0 + (double) (int) (NetFieldBase<int, NetInt>) this.whichFloor) / 20000.0));
        if (((int) this.neighborMask & 6) == 6 && ((int) this.neighborMask & 64) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 48.0), (float) ((double) tileLocation.Y * 64.0 + 48.0))), new Rectangle?(new Rectangle(16 + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, 4, 4)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0) / 20000.0));
        if (((int) this.neighborMask & 12) == 12 && ((int) this.neighborMask & 128) == 0)
          spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, (float) ((double) tileLocation.Y * 64.0 + 48.0))), new Rectangle?(new Rectangle(60 + 64 * ((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4), (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, 4, 4)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 2.0 + (double) tileLocation.X / 10000.0) / 20000.0));
        if (!this.drawContouredShadow.Value)
          spriteBatch.Draw(Game1.staminaRect, new Rectangle((int) ((double) tileLocation.X * 64.0) - 4 - Game1.viewport.X, (int) ((double) tileLocation.Y * 64.0) + 4 - Game1.viewport.Y, 64, 64), Color.Black * 0.33f);
      }
      byte key = (byte) ((uint) this.neighborMask & 15U);
      int drawGuide = Flooring.drawGuide[key];
      if ((bool) (NetFieldBase<bool, NetBool>) this.isSteppingStone)
        drawGuide = Flooring.drawGuideList[this.whichView.Value];
      if ((bool) (NetFieldBase<bool, NetBool>) this.drawContouredShadow)
      {
        Color black = Color.Black;
        black.A = (byte) ((double) black.A * 0.330000013113022);
        spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)) + new Vector2(-4f, 4f), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4 * 64 + drawGuide * 16 % 256, drawGuide / 16 * 16 + (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, 16, 16)), black, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-10f);
      }
      spriteBatch.Draw(this.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.whichFloor % 4 * 64 + drawGuide * 16 % 256, drawGuide / 16 * 16 + (int) (NetFieldBase<int, NetInt>) this.whichFloor / 4 * 64, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-09f);
    }

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      this.NeedsUpdate = false;
      return false;
    }

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
    }

    public override bool seasonUpdate(bool onLoad) => false;

    private List<Flooring.Neighbor> gatherNeighbors(GameLocation loc, Vector2 tilePos)
    {
      List<Flooring.Neighbor> neighbors = this._neighbors;
      neighbors.Clear();
      TerrainFeature terrainFeature = (TerrainFeature) null;
      NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = loc.terrainFeatures;
      foreach (Flooring.NeighborLoc offset in Flooring._offsets)
      {
        Vector2 vector2 = tilePos + offset.Offset;
        if (loc.map != null && !loc.isTileOnMap(vector2))
        {
          Flooring.Neighbor neighbor = new Flooring.Neighbor((Flooring) null, offset.Direction, offset.InvDirection);
          neighbors.Add(neighbor);
        }
        else if (terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature != null && terrainFeature is Flooring a && (NetFieldBase<int, NetInt>) a.whichFloor == this.whichFloor)
        {
          Flooring.Neighbor neighbor = new Flooring.Neighbor(a, offset.Direction, offset.InvDirection);
          neighbors.Add(neighbor);
        }
      }
      return neighbors;
    }

    public void OnAdded(GameLocation loc, Vector2 tilePos)
    {
      List<Flooring.Neighbor> neighborList = this.gatherNeighbors(loc, tilePos);
      this.neighborMask = (byte) 0;
      foreach (Flooring.Neighbor neighbor in neighborList)
      {
        this.neighborMask |= neighbor.direction;
        if (neighbor.feature != null)
          neighbor.feature.OnNeighborAdded(neighbor.invDirection);
      }
    }

    public void OnRemoved(GameLocation loc, Vector2 tilePos)
    {
      List<Flooring.Neighbor> neighborList = this.gatherNeighbors(loc, tilePos);
      this.neighborMask = (byte) 0;
      foreach (Flooring.Neighbor neighbor in neighborList)
      {
        if (neighbor.feature != null)
          neighbor.feature.OnNeighborRemoved(neighbor.invDirection);
      }
    }

    public void OnNeighborAdded(byte direction) => this.neighborMask |= direction;

    public void OnNeighborRemoved(byte direction) => this.neighborMask &= ~direction;

    private struct NeighborLoc
    {
      public readonly Vector2 Offset;
      public readonly byte Direction;
      public readonly byte InvDirection;

      public NeighborLoc(Vector2 a, byte b, byte c)
      {
        this.Offset = a;
        this.Direction = b;
        this.InvDirection = c;
      }
    }

    private struct Neighbor
    {
      public readonly Flooring feature;
      public readonly byte direction;
      public readonly byte invDirection;

      public Neighbor(Flooring a, byte b, byte c)
      {
        this.feature = a;
        this.direction = b;
        this.invDirection = c;
      }
    }
  }
}
