// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.BreakableContainer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class BreakableContainer : StardewValley.Object
  {
    public const int barrel = 118;
    public const int frostBarrel = 120;
    public const int darkBarrel = 122;
    public const int desertBarrel = 124;
    public const int volcanoBarrel = 174;
    public const int waterBarrel = 262;
    [XmlElement("debris")]
    private readonly NetInt debris = new NetInt();
    private new int shakeTimer;
    [XmlElement("health")]
    private readonly NetInt health = new NetInt();
    [XmlElement("containerType")]
    private readonly NetInt containerType = new NetInt();
    [XmlElement("hitSound")]
    private readonly NetString hitSound = new NetString();
    [XmlElement("breakSound")]
    private readonly NetString breakSound = new NetString();
    [XmlElement("breakDebrisSource")]
    private readonly NetRectangle breakDebrisSource = new NetRectangle();
    [XmlElement("breakDebrisSource2")]
    private readonly NetRectangle breakDebrisSource2 = new NetRectangle();

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.debris, (INetSerializable) this.health, (INetSerializable) this.containerType, (INetSerializable) this.hitSound, (INetSerializable) this.breakSound, (INetSerializable) this.breakDebrisSource, (INetSerializable) this.breakDebrisSource2);
    }

    public BreakableContainer()
    {
    }

    public BreakableContainer(Vector2 tile, int type, MineShaft mine)
      : base(tile, BreakableContainer.typeToIndex(type))
    {
      this.containerType.Value = type;
      if (type != 118)
        return;
      if (mine.GetAdditionalDifficulty() > 0)
      {
        if (mine.getMineArea() == 0 || mine.getMineArea() == 10)
          this.ParentSheetIndex = mine.isDarkArea() ? 118 : 262;
        else if (mine.getMineArea() == 40)
          this.ParentSheetIndex = 118;
      }
      else
      {
        if (mine.getMineArea() == 40)
        {
          this.ParentSheetIndex = 120;
          this.containerType.Value = 120;
        }
        if (mine.getMineArea() == 80)
        {
          this.ParentSheetIndex = 122;
          this.containerType.Value = 122;
        }
        if (mine.getMineArea() == 121)
        {
          this.ParentSheetIndex = 124;
          this.containerType.Value = 124;
        }
      }
      if (Game1.random.NextDouble() < 0.5)
        ++this.ParentSheetIndex;
      this.health.Value = 3;
      this.debris.Value = 12;
      this.hitSound.Value = "woodWhack";
      this.breakSound.Value = "barrelBreak";
      this.breakDebrisSource.Value = new Rectangle(598, 1275, 13, 4);
      this.breakDebrisSource2.Value = new Rectangle(611, 1275, 10, 4);
    }

    public BreakableContainer(Vector2 tile, bool isVolcano)
      : base(tile, 174)
    {
      this.containerType.Value = 174;
      this.ParentSheetIndex = 174;
      if (Game1.random.NextDouble() < 0.5)
        ++this.ParentSheetIndex;
      this.health.Value = 4;
      this.debris.Value = 14;
      this.hitSound.Value = "clank";
      this.breakSound.Value = "boulderBreak";
      this.breakDebrisSource.Value = new Rectangle(598, 1275, 13, 4);
      this.breakDebrisSource2.Value = new Rectangle(611, 1275, 10, 4);
    }

    public static int typeToIndex(int type) => type == 118 || type == 120 ? type : 0;

    public override bool performToolAction(Tool t, GameLocation location)
    {
      if (t != null && t.isHeavyHitter())
      {
        --this.health.Value;
        if (t is MeleeWeapon && (int) (NetFieldBase<int, NetInt>) (t as MeleeWeapon).type == 2)
          --this.health.Value;
        if ((int) (NetFieldBase<int, NetInt>) this.health <= 0)
        {
          if ((NetFieldBase<string, NetString>) this.breakSound != (NetString) null)
            location.playSound((string) (NetFieldBase<string, NetString>) this.breakSound);
          this.releaseContents(t.getLastFarmerToUse().currentLocation, t.getLastFarmerToUse());
          t.getLastFarmerToUse().currentLocation.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
          int num = Game1.random.Next(4, 12);
          Color color = (int) (NetFieldBase<int, NetInt>) this.containerType == 120 ? Color.White : ((int) (NetFieldBase<int, NetInt>) this.containerType == 122 ? new Color(109, 122, 80) : ((int) (NetFieldBase<int, NetInt>) this.containerType == 174 ? new Color(107, 76, 83) : new Color(130, 80, 30)));
          for (int index = 0; index < num; ++index)
            Game1.multiplayer.broadcastSprites(t.getLastFarmerToUse().currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", (Rectangle) (Game1.random.NextDouble() < 0.5 ? (NetFieldBase<Rectangle, NetRectangle>) this.breakDebrisSource : (NetFieldBase<Rectangle, NetRectangle>) this.breakDebrisSource2), 999f, 1, 0, this.tileLocation.Value * 64f + new Vector2(32f, 32f), false, Game1.random.NextDouble() < 0.5, (float) (((double) this.tileLocation.Y * 64.0 + 32.0) / 10000.0), 0.01f, color, 4f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 8.0), (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 64.0))
            {
              motion = new Vector2((float) Game1.random.Next(-30, 31) / 10f, (float) Game1.random.Next(-10, -7)),
              acceleration = new Vector2(0.0f, 0.3f)
            });
        }
        else if ((NetFieldBase<string, NetString>) this.hitSound != (NetString) null)
        {
          this.shakeTimer = 300;
          location.playSound((string) (NetFieldBase<string, NetString>) this.hitSound);
          Game1.createRadialDebris(t.getLastFarmerToUse().currentLocation, (int) (NetFieldBase<int, NetInt>) this.containerType == 174 ? 14 : 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 7), false, color: ((int) (NetFieldBase<int, NetInt>) this.containerType == 120 ? 10000 : -1));
        }
      }
      return false;
    }

    public override bool onExplosion(Farmer who, GameLocation location)
    {
      if (who == null)
        who = Game1.player;
      this.releaseContents(location, who);
      int num = Game1.random.Next(4, 12);
      Color color = (int) (NetFieldBase<int, NetInt>) this.containerType == 120 ? Color.White : ((int) (NetFieldBase<int, NetInt>) this.containerType == 122 ? new Color(109, 122, 80) : ((int) (NetFieldBase<int, NetInt>) this.containerType == 174 ? new Color(107, 76, 83) : new Color(130, 80, 30)));
      for (int index = 0; index < num; ++index)
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", (Rectangle) (Game1.random.NextDouble() < 0.5 ? (NetFieldBase<Rectangle, NetRectangle>) this.breakDebrisSource : (NetFieldBase<Rectangle, NetRectangle>) this.breakDebrisSource2), 999f, 1, 0, this.tileLocation.Value * 64f + new Vector2(32f, 32f), false, Game1.random.NextDouble() < 0.5, (float) (((double) this.tileLocation.Y * 64.0 + 32.0) / 10000.0), 0.01f, color, 4f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 8.0), (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 64.0))
        {
          motion = new Vector2((float) Game1.random.Next(-30, 31) / 10f, (float) Game1.random.Next(-10, -7)),
          acceleration = new Vector2(0.0f, 0.3f)
        });
      return true;
    }

    public void releaseContents(GameLocation location, Farmer who)
    {
      Random random = new Random((int) this.tileLocation.X + (int) this.tileLocation.Y * 10000 + (int) Game1.stats.DaysPlayed);
      int x = (int) this.tileLocation.X;
      int y = (int) this.tileLocation.Y;
      int level = -1;
      int num = 0;
      if (location is MineShaft)
      {
        level = ((MineShaft) location).mineLevel;
        if (((MineShaft) location).isContainerPlatform(x, y))
          ((MineShaft) location).updateMineLevelData(0, -1);
        num = ((MineShaft) location).GetAdditionalDifficulty();
      }
      if (random.NextDouble() < 0.2)
        return;
      if (Game1.random.NextDouble() <= 0.05 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        Game1.createMultipleObjectDebris(890, x, y, random.Next(1, 3), who.UniqueMultiplayerID, location);
      if (num > 0)
      {
        if (random.NextDouble() < 0.15)
          return;
        if (random.NextDouble() < 0.008)
          Game1.createMultipleObjectDebris(858, x, y, 1, location);
        if (random.NextDouble() < 0.01)
          Game1.createItemDebris((Item) new StardewValley.Object(Vector2.Zero, 71), new Vector2((float) x, (float) y) * 64f + new Vector2(32f), 0);
        if (random.NextDouble() < 0.01)
          Game1.createMultipleObjectDebris(random.Next(918, 921), x, y, 1, location);
        if (random.NextDouble() < 0.01)
          Game1.createMultipleObjectDebris(386, x, y, random.Next(1, 4), location);
        switch (random.Next(17))
        {
          case 0:
            Game1.createMultipleObjectDebris(382, x, y, random.Next(1, 3), location);
            break;
          case 1:
            Game1.createMultipleObjectDebris(380, x, y, random.Next(1, 4), location);
            break;
          case 2:
            Game1.createMultipleObjectDebris(62, x, y, 1, location);
            break;
          case 3:
            Game1.createMultipleObjectDebris(390, x, y, random.Next(2, 6), location);
            break;
          case 4:
            Game1.createMultipleObjectDebris(80, x, y, random.Next(2, 3), location);
            break;
          case 5:
            Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? 84 : (Game1.random.NextDouble() < 0.5 ? 92 : 370), x, y, random.Next(2, 4), location);
            break;
          case 6:
            Game1.createMultipleObjectDebris(70, x, y, 1, location);
            break;
          case 7:
            Game1.createMultipleObjectDebris(390, x, y, random.Next(2, 6), location);
            break;
          case 8:
            Game1.createMultipleObjectDebris(random.Next(218, 245), x, y, 1, location);
            break;
          case 9:
            Game1.createMultipleObjectDebris(Game1.whichFarm == 6 ? 920 : 749, x, y, 1, location);
            break;
          case 10:
            Game1.createMultipleObjectDebris(286, x, y, 1, location);
            break;
          case 11:
            Game1.createMultipleObjectDebris(378, x, y, random.Next(1, 4), location);
            break;
          case 12:
            Game1.createMultipleObjectDebris(384, x, y, random.Next(1, 4), location);
            break;
          case 13:
            Game1.createMultipleObjectDebris(287, x, y, 1, location);
            break;
        }
      }
      else
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.containerType)
        {
          case 118:
            if (random.NextDouble() < 0.65)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(9))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(382, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(378, x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(390, x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris(388, x, y, random.Next(2, 3), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? 80 : (Game1.random.NextDouble() < 0.5 ? 92 : 370), x, y, random.Next(2, 4), location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris(388, x, y, random.Next(2, 6), location);
                    return;
                  case 7:
                    Game1.createMultipleObjectDebris(390, x, y, random.Next(2, 6), location);
                    return;
                  case 8:
                    Game1.createMultipleObjectDebris(770, x, y, 1, location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(4))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(535, x, y, random.Next(1, 3), location);
                    return;
                  default:
                    return;
                }
              }
            }
            else
            {
              if (random.NextDouble() >= 0.4)
                break;
              switch (random.Next(5))
              {
                case 0:
                  Game1.createMultipleObjectDebris(66, x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris(68, x, y, 1, location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris(709, x, y, 1, location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris(535, x, y, 1, location);
                  return;
                case 4:
                  Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(level, x, y), new Vector2((float) x, (float) y) * 64f + new Vector2(32f, 32f), random.Next(4), location);
                  return;
                default:
                  return;
              }
            }
          case 120:
            if (random.NextDouble() < 0.65)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(9))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(382, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(380, x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(378, x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris(388, x, y, random.Next(2, 6), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? 84 : (Game1.random.NextDouble() < 0.5 ? 92 : 371), x, y, random.Next(2, 4), location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris(390, x, y, random.Next(2, 4), location);
                    return;
                  case 7:
                    Game1.createMultipleObjectDebris(390, x, y, random.Next(2, 6), location);
                    return;
                  case 8:
                    Game1.createMultipleObjectDebris(770, x, y, 1, location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(4))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(536, x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  default:
                    return;
                }
              }
            }
            else
            {
              if (random.NextDouble() >= 0.4)
                break;
              switch (random.Next(5))
              {
                case 0:
                  Game1.createMultipleObjectDebris(62, x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris(70, x, y, 1, location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris(709, x, y, random.Next(1, 4), location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris(536, x, y, 1, location);
                  return;
                case 4:
                  Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(level, x, y), new Vector2((float) x, (float) y) * 64f + new Vector2(32f, 32f), random.Next(4), location);
                  return;
                default:
                  return;
              }
            }
          case 122:
          case 124:
            if (random.NextDouble() < 0.65)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(8))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(382, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(384, x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(380, x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris(378, x, y, random.Next(2, 6), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris(390, x, y, random.Next(2, 6), location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris(388, x, y, random.Next(2, 6), location);
                    return;
                  case 7:
                    Game1.createMultipleObjectDebris(881, x, y, random.Next(2, 6), location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(4))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(537, x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? 82 : 78, x, y, random.Next(1, 3), location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  default:
                    return;
                }
              }
            }
            else
            {
              if (random.NextDouble() >= 0.4)
                break;
              switch (random.Next(6))
              {
                case 0:
                  Game1.createMultipleObjectDebris(60, x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris(64, x, y, 1, location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris(709, x, y, random.Next(1, 4), location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris(749, x, y, 1, location);
                  return;
                case 4:
                  Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(level, x, y), new Vector2((float) x, (float) y) * 64f + new Vector2(32f, 32f), random.Next(4), location);
                  return;
                case 5:
                  Game1.createMultipleObjectDebris(688, x, y, 1, location);
                  return;
                default:
                  return;
              }
            }
          case 174:
            if (random.NextDouble() < 0.1)
              Game1.player.team.RequestLimitedNutDrops("VolcanoBarrel", location, x * 64, y * 64, 5);
            if (location is VolcanoDungeon && (int) (NetFieldBase<int, NetInt>) (location as VolcanoDungeon).level == 5 && x == 34)
            {
              Game1.createItemDebris((Item) new StardewValley.Object(851, 1, quality: 2), new Vector2((float) x, (float) y) * 64f, 1);
              break;
            }
            if (random.NextDouble() < 0.75)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(7))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(382, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(384, x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    location.characters.Add((NPC) new DwarvishSentry(new Vector2((float) x, (float) y) * 64f));
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(380, x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris(378, x, y, random.Next(2, 6), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris(66, x, y, 1, location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris(709, x, y, random.Next(2, 6), location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(5))
                {
                  case 0:
                    Game1.createMultipleObjectDebris(78, x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris(749, x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris(60, x, y, 1, location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris(64, x, y, 1, location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris(68, x, y, 1, location);
                    return;
                  default:
                    return;
                }
              }
            }
            else if (random.NextDouble() < 0.4)
            {
              switch (random.Next(9))
              {
                case 0:
                  Game1.createMultipleObjectDebris(72, x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris(831, x, y, random.Next(1, 4), location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris(833, x, y, random.Next(1, 3), location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris(749, x, y, 1, location);
                  return;
                case 4:
                  Game1.createMultipleObjectDebris(386, x, y, 1, location);
                  return;
                case 5:
                  Game1.createMultipleObjectDebris(848, x, y, 1, location);
                  return;
                case 6:
                  Game1.createMultipleObjectDebris(856, x, y, 1, location);
                  return;
                case 7:
                  Game1.createMultipleObjectDebris(886, x, y, 1, location);
                  return;
                case 8:
                  Game1.createMultipleObjectDebris(688, x, y, 1, location);
                  return;
                default:
                  return;
              }
            }
            else
            {
              location.characters.Add((NPC) new DwarvishSentry(new Vector2((float) x, (float) y) * 64f));
              break;
            }
        }
      }
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (this.shakeTimer <= 0)
        return;
      this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      Vector2 vector2 = this.getScale() * 4f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64)));
      Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2.X / 2.0), (int) ((double) local.Y - (double) vector2.Y / 2.0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
      if (this.shakeTimer > 0)
      {
        int num = this.shakeTimer / 100 + 1;
        destinationRectangle.X += Game1.random.Next(-num, num + 1);
        destinationRectangle.Y += Game1.random.Next(-num, num + 1);
      }
      spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable((bool) (NetFieldBase<bool, NetBool>) this.showNextIndex ? this.ParentSheetIndex + 1 : this.ParentSheetIndex)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 - 1) / 10000f) + ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 ? 0.0015f : 0.0f));
    }
  }
}
