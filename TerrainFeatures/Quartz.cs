// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Quartz
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class Quartz : TerrainFeature
  {
    public const float shakeRate = 0.01570796f;
    public const float shakeDecayRate = 0.003067962f;
    public const double chanceForDiamond = 0.02;
    public const double chanceForPrismaticShard = 0.005;
    public const double chanceForIridium = 0.007;
    public const double chanceForLevelUnique = 0.03;
    public const double chanceForRefinedQuartz = 0.04;
    public const int startingHealth = 10;
    public const int large = 3;
    public const int medium = 2;
    public const int small = 1;
    public const int tiny = 0;
    public const int pointingLeft = 0;
    public const int pointingUp = 1;
    public const int pointingRight = 2;
    private Texture2D texture;
    [XmlElement("health")]
    public readonly NetFloat health = new NetFloat();
    [XmlElement("flipped")]
    public readonly NetBool flipped = new NetBool();
    private bool shakeLeft;
    [XmlElement("falling")]
    private readonly NetBool falling = new NetBool();
    private float shakeRotation;
    private float maxShake;
    [XmlElement("glow")]
    private readonly NetFloat glow = new NetFloat(0.0f);
    [XmlElement("bigness")]
    public readonly NetInt bigness = new NetInt();
    private int identifier;
    [XmlElement("color")]
    private readonly NetColor color = new NetColor();

    public Quartz()
      : base(true)
    {
      this.NetFields.AddFields((INetSerializable) this.health, (INetSerializable) this.flipped, (INetSerializable) this.falling, (INetSerializable) this.glow, (INetSerializable) this.bigness, (INetSerializable) this.color);
      this.loadSprite();
    }

    public Quartz(int bigness, Color color)
      : this()
    {
      if (bigness > 3)
        this.bigness.Value = 2;
      this.health.Value = (float) (10 - (3 - bigness) * 2);
      this.bigness.Value = bigness;
      this.color.Value = color;
    }

    public override void loadSprite()
    {
      try
      {
        this.texture = Game1.content.Load<Texture2D>("TerrainFeatures\\Quartz");
      }
      catch (Exception ex)
      {
      }
      this.identifier = Game1.random.Next(-999999, 999999);
    }

    public override Microsoft.Xna.Framework.Rectangle getBoundingBox(Vector2 tileLocation) => (int) (NetFieldBase<int, NetInt>) this.bigness == 3 ? new Microsoft.Xna.Framework.Rectangle((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.Y * 64.0), 128, 128) : new Microsoft.Xna.Framework.Rectangle((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.Y * 64.0), 64, 64);

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.glow > 0.0)
      {
        this.glow.Value -= 0.01f;
        LightSource lightSource = Utility.getLightSource((int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y));
        if (lightSource != null)
          lightSource.color.Value = new Color((int) byte.MaxValue - (int) this.color.R, (int) byte.MaxValue - (int) this.color.G, (int) byte.MaxValue - (int) this.color.B, (int) ((double) byte.MaxValue * (double) (float) (NetFieldBase<float, NetFloat>) this.glow));
        if ((double) this.glow.Value <= 0.0)
          Utility.removeLightSource((int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y));
      }
      if ((double) this.maxShake > 0.0)
      {
        if (this.shakeLeft)
        {
          this.shakeRotation -= (float) Math.PI / 200f;
          if ((double) this.shakeRotation <= -(double) this.maxShake)
            this.shakeLeft = false;
        }
        else
        {
          this.shakeRotation += (float) Math.PI / 200f;
          if ((double) this.shakeRotation >= (double) this.maxShake)
            this.shakeLeft = true;
        }
      }
      if ((double) this.maxShake > 0.0)
        this.maxShake = Math.Max(0.0f, this.maxShake - 0.003067962f);
      return (double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0;
    }

    public override void performPlayerEntryAction(Vector2 tileLocation)
    {
    }

    private void shake(Vector2 tileLocation)
    {
      if ((double) this.maxShake != 0.0)
        return;
      this.shakeLeft = (double) Game1.player.getTileLocation().X > (double) tileLocation.X || (double) Game1.player.getTileLocation().X == (double) tileLocation.X && Game1.random.NextDouble() < 0.5;
      this.maxShake = (float) Math.PI / 128f;
    }

    public override bool performUseAction(Vector2 tileLocation, GameLocation location)
    {
      if (Game1.soundBank != null)
      {
        Random random = new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 7.0 + (double) tileLocation.Y * 11.0 + (double) Game1.CurrentMineLevel));
        ICue cue = Game1.soundBank.GetCue("crystal");
        int num = random.Next(2400);
        int val = num - num % 100;
        cue.SetVariable("Pitch", val);
        cue.Play();
      }
      this.glow.Value = 0.7f;
      Color color = ((double) this.glow.Value > 0.0 ? new Color((int) this.color.R + (int) ((double) this.glow.Value * 50.0), (int) this.color.G + (int) ((double) this.glow.Value * 50.0), (int) this.color.B + (int) ((double) this.glow.Value * 50.0)) : (Color) (NetFieldBase<Color, NetColor>) this.color) * (0.3f + this.glow.Value);
      Utility.removeLightSource((int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y));
      if ((int) (NetFieldBase<int, NetInt>) this.bigness < 2)
        Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0)), 1f, Utility.getOppositeColor(color), (int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y)));
      else if ((int) (NetFieldBase<int, NetInt>) this.bigness >= 2)
        Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0)), 1f, Utility.getOppositeColor(color), (int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y)));
      return false;
    }

    public override bool isPassable(Character c = null) => (double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0;

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
    }

    public override bool seasonUpdate(bool onLoad) => false;

    private Microsoft.Xna.Framework.Rectangle getSourceRect(int size)
    {
      switch (size)
      {
        case 0:
          return new Microsoft.Xna.Framework.Rectangle(16, 0, 16, 16);
        case 1:
          return new Microsoft.Xna.Framework.Rectangle(64 + ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 3.0 ? 16 : 0), 16, 16, 16);
        case 2:
          return new Microsoft.Xna.Framework.Rectangle((int) ((8.0 - (double) (float) (NetFieldBase<float, NetFloat>) this.health) / 2.0) * 16, 0, 16, 32);
        case 3:
          return new Microsoft.Xna.Framework.Rectangle((int) ((10.0 - (double) (float) (NetFieldBase<float, NetFloat>) this.health) / 3.0) * 16 * 2, 32, 32, 48);
        default:
          return Microsoft.Xna.Framework.Rectangle.Empty;
      }
    }

    public override bool performToolAction(
      Tool t,
      int explosion,
      Vector2 tileLocation,
      GameLocation location = null)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health > 0.0)
      {
        float num1 = 0.0f;
        if (t == null && explosion > 0)
          num1 = (float) explosion;
        else if (t.BaseName.Contains("Pickaxe"))
        {
          switch ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel)
          {
            case 0:
              num1 = 2f;
              break;
            case 1:
              num1 = 2.5f;
              break;
            case 2:
              num1 = 3.34f;
              break;
            case 3:
              num1 = 5f;
              break;
            case 4:
              num1 = 10f;
              break;
          }
          Game1.playSound("hammer");
        }
        if ((double) num1 > 0.0)
        {
          this.glow.Value = 0.7f;
          this.shake(tileLocation);
          this.health.Value -= num1;
          this.glow.Value = 0.25f;
          Color color = new Color((int) byte.MaxValue - (int) this.color.R, (int) byte.MaxValue - (int) this.color.G, (int) byte.MaxValue - (int) this.color.B, (int) ((double) byte.MaxValue * (double) (float) (NetFieldBase<float, NetFloat>) this.glow));
          Utility.removeLightSource((int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y));
          if ((int) (NetFieldBase<int, NetInt>) this.bigness < 2)
            Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0)), 1f, Utility.getOppositeColor(color), (int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y)));
          else if ((int) (NetFieldBase<int, NetInt>) this.bigness == 2)
            Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0)), 1f, Utility.getOppositeColor(color), (int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y)));
          if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
          {
            Random random = new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 7.0 + (double) tileLocation.Y * 11.0 + (double) Game1.CurrentMineLevel + (double) Game1.player.timesReachedMineBottom));
            double num2 = 1.0 + Game1.player.team.AverageDailyLuck(Game1.currentLocation) + Game1.player.team.AverageLuckLevel(Game1.currentLocation) / 100.0 + (double) (int) (NetFieldBase<int, NetInt>) Game1.player.miningLevel / 50.0;
            if (random.NextDouble() < 0.005 * num2)
              Game1.createObjectDebris(74, (int) tileLocation.X, (int) tileLocation.Y, location);
            else if (random.NextDouble() < 0.007 * num2)
              Game1.createDebris(10, (int) tileLocation.X, (int) tileLocation.Y, 2, location);
            else if (random.NextDouble() < 0.02 * num2)
              Game1.createObjectDebris(72, (int) tileLocation.X, (int) tileLocation.Y, location);
            else if (random.NextDouble() < 0.03 * num2)
              Game1.createObjectDebris(Game1.CurrentMineLevel < 40 ? 86 : (Game1.CurrentMineLevel < 80 ? 84 : 82), (int) tileLocation.X, (int) tileLocation.Y, location);
            else if (random.NextDouble() < 0.04 * num2)
              Game1.createObjectDebris(338, (int) tileLocation.X, (int) tileLocation.Y, location);
            for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.bigness * 3; ++index)
            {
              int x = Game1.random.Next(this.getBoundingBox(tileLocation).X, this.getBoundingBox(tileLocation).Right);
              int y = Game1.random.Next(this.getBoundingBox(tileLocation).Y, this.getBoundingBox(tileLocation).Bottom);
              Game1.currentLocation.TemporarySprites.Add((TemporaryAnimatedSprite) new CosmeticDebris(this.texture, new Vector2((float) x, (float) y), (float) Game1.random.Next(-25, 25) / 100f, (float) (x - this.getBoundingBox(tileLocation).Center.X) / 30f, (float) Game1.random.Next(-800, -100) / 100f, (int) tileLocation.Y * 64 + 64, new Microsoft.Xna.Framework.Rectangle(Game1.random.Next(4, 8) * 16, 0, 16, 16), (Color) (NetFieldBase<Color, NetColor>) this.color, Game1.soundBank != null ? Game1.soundBank.GetCue("boulderCrack") : (ICue) null, new LightSource(4, Vector2.Zero, 0.25f, color), 24, 1000));
            }
            Utility.removeLightSource((int) ((double) tileLocation.X * 1000.0 + (double) tileLocation.Y));
          }
        }
      }
      return false;
    }

    private Vector2 getPivot()
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.bigness)
      {
        case 1:
          return new Vector2(8f, 16f);
        case 2:
          return new Vector2(8f, 32f);
        case 3:
          return new Vector2(16f, 48f);
        default:
          return Vector2.Zero;
      }
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
        return;
      SpriteBatch spriteBatch1 = spriteBatch;
      Texture2D texture = this.texture;
      xTile.Dimensions.Rectangle viewport = Game1.viewport;
      Microsoft.Xna.Framework.Rectangle boundingBox = this.getBoundingBox(tileLocation);
      double x = (double) boundingBox.Center.X;
      boundingBox = this.getBoundingBox(tileLocation);
      double bottom = (double) boundingBox.Bottom;
      Vector2 globalPosition = new Vector2((float) x, (float) bottom);
      Vector2 local = Game1.GlobalToLocal(viewport, globalPosition);
      Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(this.getSourceRect((int) (NetFieldBase<int, NetInt>) this.bigness));
      Color color = (Color) (NetFieldBase<Color, NetColor>) this.color;
      double shakeRotation = (double) this.shakeRotation;
      Vector2 pivot = this.getPivot();
      double layerDepth = ((double) tileLocation.Y * 64.0 + 64.0) / 10000.0;
      spriteBatch1.Draw(texture, local, sourceRectangle, color, (float) shakeRotation, pivot, 4f, SpriteEffects.None, (float) layerDepth);
    }
  }
}
