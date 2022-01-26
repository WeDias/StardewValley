// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Grass
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class Grass : TerrainFeature
  {
    public const float defaultShakeRate = 0.03926991f;
    public const float maximumShake = 0.3926991f;
    public const float shakeDecayRate = 0.008975979f;
    public const byte springGrass = 1;
    public const byte caveGrass = 2;
    public const byte frostGrass = 3;
    public const byte lavaGrass = 4;
    public const byte caveGrass2 = 5;
    public const byte cobweb = 6;
    public static ICue grassSound;
    [XmlElement("grassType")]
    public readonly NetByte grassType = new NetByte();
    private bool shakeLeft;
    protected float shakeRotation;
    protected float maxShake;
    protected float shakeRate;
    [XmlElement("numberOfWeeds")]
    public readonly NetInt numberOfWeeds = new NetInt();
    [XmlElement("grassSourceOffset")]
    public readonly NetInt grassSourceOffset = new NetInt();
    [XmlIgnore]
    public Lazy<Texture2D> texture;
    private int[] whichWeed = new int[4];
    private int[] offset1 = new int[4];
    private int[] offset2 = new int[4];
    private int[] offset3 = new int[4];
    private int[] offset4 = new int[4];
    private bool[] flip = new bool[4];
    private double[] shakeRandom = new double[4];

    public Grass()
      : base(true)
    {
      this.texture = new Lazy<Texture2D>((Func<Texture2D>) (() => Game1.content.Load<Texture2D>(this.textureName())));
      this.NetFields.AddFields((INetSerializable) this.grassType, (INetSerializable) this.numberOfWeeds, (INetSerializable) this.grassSourceOffset);
    }

    public Grass(int which, int numberOfWeeds)
      : this()
    {
      this.grassType.Value = (byte) which;
      this.loadSprite();
      this.numberOfWeeds.Value = numberOfWeeds;
    }

    public virtual string textureName() => "TerrainFeatures\\grass";

    public override bool isPassable(Character c = null) => true;

    public override void loadSprite()
    {
      try
      {
        if (Game1.soundBank != null)
          Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
        if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 1)
        {
          string seasonForLocation = Game1.GetSeasonForLocation(this.currentLocation);
          if (!(seasonForLocation == "spring"))
          {
            if (!(seasonForLocation == "summer"))
            {
              if (!(seasonForLocation == "fall"))
              {
                if (!(seasonForLocation == "winter"))
                  return;
                if (this.currentLocation != null && this.currentLocation.IsOutdoors)
                  this.grassSourceOffset.Value = 80;
                else
                  this.grassSourceOffset.Value = 0;
              }
              else
                this.grassSourceOffset.Value = 40;
            }
            else
              this.grassSourceOffset.Value = 20;
          }
          else
            this.grassSourceOffset.Value = 0;
        }
        else if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 2)
          this.grassSourceOffset.Value = 60;
        else if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 3)
          this.grassSourceOffset.Value = 80;
        else if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 4)
          this.grassSourceOffset.Value = 100;
        else
          this.grassSourceOffset.Value = ((int) (byte) (NetFieldBase<byte, NetByte>) this.grassType + 1) * 20;
      }
      catch (Exception ex)
      {
      }
    }

    public override void OnAddedToLocation(GameLocation location, Vector2 tile) => this.loadSprite();

    public override Rectangle getBoundingBox(Vector2 tileLocation) => new Rectangle((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.Y * 64.0), 64, 64);

    public override Rectangle getRenderBounds(Vector2 tileLocation) => new Rectangle((int) ((double) tileLocation.X * 64.0) - 32, (int) ((double) tileLocation.Y * 64.0) - 32, 128, 112);

    public override void doCollisionAction(
      Rectangle positionOfCollider,
      int speedOfCollision,
      Vector2 tileLocation,
      Character who,
      GameLocation location)
    {
      if (location != Game1.currentLocation)
        return;
      if (speedOfCollision > 0 && (double) this.maxShake == 0.0 && positionOfCollider.Intersects(this.getBoundingBox(tileLocation)))
      {
        if ((who == null || !(who is FarmAnimal)) && Grass.grassSound != null && !Grass.grassSound.IsPlaying && Utility.isOnScreen(new Point((int) tileLocation.X, (int) tileLocation.Y), 2, location) && Game1.soundBank != null)
        {
          Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
          Grass.grassSound.Play();
        }
        this.shake(0.3926991f / (float) ((5 + Game1.player.addedSpeed) / speedOfCollision), (float) Math.PI / 80f / (float) ((5 + Game1.player.addedSpeed) / speedOfCollision), (double) positionOfCollider.Center.X > (double) tileLocation.X * 64.0 + 32.0);
      }
      if (who is Farmer && Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon && ((MeleeWeapon) Game1.player.CurrentTool).isOnSpecial && (int) (NetFieldBase<int, NetInt>) ((MeleeWeapon) Game1.player.CurrentTool).type == 0 && (double) Math.Abs(this.shakeRotation) < 1.0 / 1000.0 && this.performToolAction(Game1.player.CurrentTool, -1, tileLocation, location))
        Game1.currentLocation.terrainFeatures.Remove(tileLocation);
      if (!(who is Farmer))
        return;
      (who as Farmer).temporarySpeedBuff = -1f;
      if ((byte) (NetFieldBase<byte, NetByte>) this.grassType != (byte) 6)
        return;
      (who as Farmer).temporarySpeedBuff = -3f;
    }

    public bool reduceBy(int number, Vector2 tileLocation, bool showDebris)
    {
      this.numberOfWeeds.Value -= number;
      if (showDebris)
        Game1.createRadialDebris(Game1.currentLocation, this.textureName(), new Rectangle(2, 8, 8, 8), 1, ((int) tileLocation.X + 1) * 64, ((int) tileLocation.Y + 1) * 64, Game1.random.Next(6, 14), (int) tileLocation.Y + 1, Color.White, 4f);
      return (int) (NetFieldBase<int, NetInt>) this.numberOfWeeds <= 0;
    }

    protected void shake(float shake, float rate, bool left)
    {
      this.maxShake = shake;
      this.shakeRate = rate;
      this.shakeRotation = 0.0f;
      this.shakeLeft = left;
      this.NeedsUpdate = true;
    }

    public override void performPlayerEntryAction(Vector2 tileLocation)
    {
      base.performPlayerEntryAction(tileLocation);
      if (this.shakeRandom[0] != 0.0)
        return;
      this.setUpRandom(tileLocation);
    }

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      if (this.shakeRandom[0] == 0.0)
        this.setUpRandom(tileLocation);
      if ((double) this.maxShake > 0.0)
      {
        if (this.shakeLeft)
        {
          this.shakeRotation -= this.shakeRate;
          if ((double) Math.Abs(this.shakeRotation) >= (double) this.maxShake)
            this.shakeLeft = false;
        }
        else
        {
          this.shakeRotation += this.shakeRate;
          if ((double) this.shakeRotation >= (double) this.maxShake)
          {
            this.shakeLeft = true;
            this.shakeRotation -= this.shakeRate;
          }
        }
        this.maxShake = Math.Max(0.0f, this.maxShake - (float) Math.PI / 350f);
      }
      else
      {
        this.shakeRotation /= 2f;
        if ((double) this.shakeRotation <= 0.00999999977648258)
        {
          this.NeedsUpdate = false;
          this.shakeRotation = 0.0f;
        }
      }
      return false;
    }

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
      if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 1 && (!environment.GetSeasonForLocation().Equals("winter") || environment.getMapProperty("AllowGrassGrowInWinter") != null) && (int) (NetFieldBase<int, NetInt>) this.numberOfWeeds < 4)
        this.numberOfWeeds.Value = Utility.Clamp(this.numberOfWeeds.Value + Game1.random.Next(1, 4), 0, 4);
      this.setUpRandom(tileLocation);
    }

    public void setUpRandom(Vector2 tileLocation)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed / 28 + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
      for (int index = 0; index < 4; ++index)
      {
        this.whichWeed[index] = random.Next(3);
        this.offset1[index] = random.Next(-2, 3);
        this.offset2[index] = random.Next(-2, 3);
        this.offset3[index] = random.Next(-2, 3);
        this.offset4[index] = random.Next(-2, 3);
        this.flip[index] = random.NextDouble() < 0.5;
        this.shakeRandom[index] = random.NextDouble();
      }
    }

    public override bool seasonUpdate(bool onLoad)
    {
      if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 1 && Game1.GetSeasonForLocation(this.currentLocation).Equals("winter") && this.currentLocation.IsOutdoors && this.currentLocation.getMapProperty("AllowGrassSurviveInWinter") == "" && !onLoad)
        return true;
      if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 1)
        this.loadSprite();
      return false;
    }

    public override bool performToolAction(
      Tool t,
      int explosion,
      Vector2 tileLocation,
      GameLocation location)
    {
      if (location == null)
        location = Game1.currentLocation;
      if (t != null && t is MeleeWeapon && (int) (NetFieldBase<int, NetInt>) ((MeleeWeapon) t).type != 2 || explosion > 0)
      {
        if (t != null && (int) (NetFieldBase<int, NetInt>) (t as MeleeWeapon).type != 1)
          DelayedAction.playSoundAfterDelay("daggerswipe", 50);
        else
          location.playSound("swordswipe");
        this.shake(3f * (float) Math.PI / 32f, (float) Math.PI / 40f, Game1.random.NextDouble() < 0.5);
        int num1 = explosion <= 0 ? 1 : Math.Max(1, explosion + 2 - Game1.recentMultiplayerRandom.Next(2));
        if (t is MeleeWeapon && t.InitialParentTileIndex == 53)
          num1 = 2;
        if ((byte) (NetFieldBase<byte, NetByte>) this.grassType == (byte) 6 && Game1.random.NextDouble() < 0.5)
          num1 = 0;
        this.numberOfWeeds.Value = (int) (NetFieldBase<int, NetInt>) this.numberOfWeeds - num1;
        Color color = Color.Green;
        switch ((byte) (NetFieldBase<byte, NetByte>) this.grassType)
        {
          case 1:
            string seasonForLocation = location.GetSeasonForLocation();
            if (!(seasonForLocation == "spring"))
            {
              if (!(seasonForLocation == "summer"))
              {
                if (seasonForLocation == "fall")
                {
                  color = new Color(219, 102, 58);
                  break;
                }
                break;
              }
              color = new Color(110, 190, 24);
              break;
            }
            color = new Color(60, 180, 58);
            break;
          case 2:
            color = new Color(148, 146, 71);
            break;
          case 3:
            color = new Color(216, 240, (int) byte.MaxValue);
            break;
          case 4:
            color = new Color(165, 93, 58);
            break;
          case 6:
            color = Color.White * 0.6f;
            break;
        }
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(28, tileLocation * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-16, 16)), color, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: ((float) Game1.random.Next(60, 100))));
        if ((int) (NetFieldBase<int, NetInt>) this.numberOfWeeds <= 0)
        {
          if ((byte) (NetFieldBase<byte, NetByte>) this.grassType != (byte) 1)
          {
            Random random = Game1.IsMultiplayer ? Game1.recentMultiplayerRandom : new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 1000.0 + (double) tileLocation.Y * 11.0 + (double) Game1.CurrentMineLevel + (double) Game1.player.timesReachedMineBottom));
            if (random.NextDouble() < 0.005)
              Game1.createObjectDebris(114, (int) tileLocation.X, (int) tileLocation.Y, -1, 0, 1f, location);
            else if (random.NextDouble() < 0.01)
              Game1.createDebris(4, (int) tileLocation.X, (int) tileLocation.Y, random.Next(1, 2), location);
            else if (random.NextDouble() < 0.02)
              Game1.createDebris(92, (int) tileLocation.X, (int) tileLocation.Y, random.Next(2, 4), location);
          }
          else if (t is MeleeWeapon && (t.Name.Contains("Scythe") || (t as MeleeWeapon).isScythe()))
          {
            Random random = Game1.IsMultiplayer ? Game1.recentMultiplayerRandom : new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 1000.0 + (double) tileLocation.Y * 11.0));
            double num2 = t.InitialParentTileIndex == 53 ? 0.75 : 0.5;
            if (random.NextDouble() < num2 && (Game1.getLocationFromName("Farm") as Farm).tryToAddHay(1) == 0)
            {
              Game1.multiplayer.broadcastSprites(t.getLastFarmerToUse().currentLocation, new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16, 16), 750f, 1, 0, t.getLastFarmerToUse().Position - new Vector2(0.0f, 128f), false, false, t.getLastFarmerToUse().Position.Y / 10000f, 0.005f, Color.White, 4f, -0.005f, 0.0f, 0.0f)
              {
                motion = {
                  Y = -1f
                },
                layerDepth = (float) (1.0 - (double) Game1.random.Next(100) / 10000.0),
                delayBeforeAnimationStart = Game1.random.Next(350)
              });
              Game1.addHUDMessage(new HUDMessage("Hay", 1, true, Color.LightGoldenrodYellow, (Item) new StardewValley.Object(178, 1)));
            }
          }
          return true;
        }
      }
      return false;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 positionOnScreen,
      Vector2 tileLocation,
      float scale,
      float layerDepth)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed / 28 + (int) positionOnScreen.X * 7 + (int) positionOnScreen.Y * 11);
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.numberOfWeeds; ++index)
      {
        int num = random.Next(3);
        Vector2 position = index != 4 ? tileLocation * 64f + new Vector2((float) (index % 2 * 64 / 2 + random.Next(-2, 2) * 4 - 4) + 30f, (float) (index / 2 * 64 / 2 + random.Next(-2, 2) * 4 + 40)) : tileLocation * 64f + new Vector2((float) (16 + random.Next(-2, 2) * 4 - 4) + 30f, (float) (16 + random.Next(-2, 2) * 4 + 40));
        spriteBatch.Draw(this.texture.Value, position, new Rectangle?(new Rectangle(num * 15, (int) (NetFieldBase<int, NetInt>) this.grassSourceOffset, 15, 20)), Color.White, this.shakeRotation / (float) (random.NextDouble() + 1.0), Vector2.Zero, scale, SpriteEffects.None, layerDepth + (float) ((32.0 * (double) scale + 300.0) / 20000.0));
      }
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.numberOfWeeds; ++index)
      {
        Vector2 globalPosition = index != 4 ? tileLocation * 64f + new Vector2((float) (index % 2 * 64 / 2 + this.offset3[index] * 4 - 4) + 30f, (float) (index / 2 * 64 / 2 + this.offset4[index] * 4 + 40)) : tileLocation * 64f + new Vector2((float) (16 + this.offset1[index] * 4 - 4) + 30f, (float) (16 + this.offset2[index] * 4 + 40));
        spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(new Rectangle(this.whichWeed[index] * 15, (int) (NetFieldBase<int, NetInt>) this.grassSourceOffset, 15, 20)), Color.White, this.shakeRotation / (float) (this.shakeRandom[index] + 1.0), new Vector2(7.5f, 17.5f), 4f, this.flip[index] ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) globalPosition.Y + 16.0 - 20.0) / 10000.0 + (double) globalPosition.X / 10000000.0));
      }
    }
  }
}
