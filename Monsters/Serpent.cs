// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Serpent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewValley.Monsters
{
  public class Serpent : Monster
  {
    public const float rotationIncrement = 0.04908739f;
    private int wasHitCounter;
    private float targetRotation;
    private bool turningRight;
    private readonly NetFarmerRef killer = new NetFarmerRef().Delayed(false);
    public List<Vector3> segments = new List<Vector3>();
    public NetInt segmentCount = new NetInt(0);

    public Serpent()
    {
    }

    public Serpent(Vector2 position)
      : base(nameof (Serpent), position)
    {
      this.InitializeAttributes();
    }

    public Serpent(Vector2 position, string name)
      : base(name, position)
    {
      this.InitializeAttributes();
      if (!(name == "Royal Serpent"))
        return;
      this.segmentCount.Value = Game1.random.Next(3, 7);
      if (Game1.random.NextDouble() < 0.1)
        this.segmentCount.Value = Game1.random.Next(5, 10);
      else if (Game1.random.NextDouble() < 0.01)
        this.segmentCount.Value *= 3;
      this.reloadSprite();
      this.MaxHealth += this.segmentCount.Value * 50;
      this.Health = this.MaxHealth;
    }

    public virtual void InitializeAttributes()
    {
      this.Slipperiness = 24 + Game1.random.Next(10);
      this.Halt();
      this.IsWalkingTowardPlayer = false;
      this.Sprite.SpriteWidth = 32;
      this.Sprite.SpriteHeight = 32;
      this.Scale = 0.75f;
      this.HideShadow = true;
    }

    public bool IsRoyalSerpent() => this.segmentCount.Value > 1;

    public override bool TakesDamageFromHitbox(Rectangle area_of_effect)
    {
      if (base.TakesDamageFromHitbox(area_of_effect))
        return true;
      if (this.IsRoyalSerpent())
      {
        Rectangle boundingBox = this.GetBoundingBox();
        Vector2 vector2 = new Vector2((float) boundingBox.X - this.Position.X, (float) boundingBox.Y - this.Position.Y);
        foreach (Vector3 segment in this.segments)
        {
          boundingBox.X = (int) ((double) segment.X + (double) vector2.X);
          boundingBox.Y = (int) ((double) segment.Y + (double) vector2.Y);
          if (boundingBox.Intersects(area_of_effect))
            return true;
        }
      }
      return false;
    }

    public override bool OverlapsFarmerForDamage(Farmer who)
    {
      if (base.OverlapsFarmerForDamage(who))
        return true;
      if (this.IsRoyalSerpent())
      {
        Rectangle boundingBox1 = who.GetBoundingBox();
        Rectangle boundingBox2 = this.GetBoundingBox();
        Vector2 vector2 = new Vector2((float) boundingBox2.X - this.Position.X, (float) boundingBox2.Y - this.Position.Y);
        foreach (Vector3 segment in this.segments)
        {
          boundingBox2.X = (int) ((double) segment.X + (double) vector2.X);
          boundingBox2.Y = (int) ((double) segment.Y + (double) vector2.Y);
          if (boundingBox2.Intersects(boundingBox1))
            return true;
        }
      }
      return false;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.killer.NetFields, (INetSerializable) this.segmentCount);
      this.segmentCount.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) =>
      {
        if (new_value <= 0)
          return;
        this.reloadSprite();
      });
    }

    public override void reloadSprite()
    {
      if (this.IsRoyalSerpent())
      {
        this.Sprite = new AnimatedSprite("Characters\\Monsters\\Royal Serpent");
        this.Scale = 1f;
      }
      else
      {
        this.Sprite = new AnimatedSprite("Characters\\Monsters\\Serpent");
        this.Scale = 0.75f;
      }
      this.Sprite.SpriteWidth = 32;
      this.Sprite.SpriteHeight = 32;
      this.HideShadow = true;
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      int damage1 = Math.Max(1, damage - (int) (NetFieldBase<int, NetInt>) this.resilience);
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
      {
        damage1 = -1;
      }
      else
      {
        this.Health -= damage1;
        this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
        this.wasHitCounter = 500;
        this.currentLocation.playSound("serpentHit");
        if (this.Health <= 0)
        {
          this.killer.Value = who;
          this.deathAnimation();
        }
      }
      this.addedSpeed = Game1.random.Next(-1, 1);
      return damage1;
    }

    protected override void sharedDeathAnimation()
    {
    }

    protected override void localDeathAnimation()
    {
      if (this.killer.Value == null)
        return;
      Rectangle boundingBox1 = this.GetBoundingBox();
      boundingBox1.Inflate(-boundingBox1.Width / 2 + 1, -boundingBox1.Height / 2 + 1);
      Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(boundingBox1.Center, 4f, this.killer.Value);
      int x = -(int) velocityTowardPlayer.X;
      int y = -(int) velocityTowardPlayer.Y;
      if (this.IsRoyalSerpent())
      {
        this.currentLocation.localSound("serpentDie");
        for (int index = -1; index < this.segments.Count; ++index)
        {
          Vector2 position = Vector2.Zero;
          Rectangle sourceRect = new Rectangle(0, 0, 32, 32);
          float num = this.rotation;
          float t = 0.0f;
          if (index == -1)
          {
            position = this.Position;
            sourceRect = new Rectangle(0, 64, 32, 32);
          }
          else
          {
            if (this.segments.Count <= 0 || index >= this.segments.Count)
              break;
            t = (float) (index + 1) / (float) this.segments.Count;
            position = new Vector2(this.segments[index].X, this.segments[index].Y);
            boundingBox1.X = (int) ((double) position.X - (double) (boundingBox1.Width / 2));
            boundingBox1.Y = (int) ((double) position.Y - (double) (boundingBox1.Height / 2));
            sourceRect = new Rectangle(32, 64, 32, 32);
            if (index == this.segments.Count - 1)
              sourceRect = new Rectangle(64, 64, 32, 32);
            num = this.segments[index].Z;
          }
          TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, sourceRect, 800f, 1, 0, position, false, false, 0.9f, 1f / 1000f, new Color()
          {
            R = (byte) Utility.Lerp((float) byte.MaxValue, (float) byte.MaxValue, t),
            G = (byte) Utility.Lerp(0.0f, 166f, t),
            B = (byte) Utility.Lerp(0.0f, 0.0f, t),
            A = byte.MaxValue
          }, 4f * (float) (NetFieldBase<float, NetFloat>) this.scale, 0.01f, num + 3.141593f, (float) ((double) Game1.random.Next(3, 5) * Math.PI / 64.0))
          {
            motion = new Vector2((float) x, (float) y),
            layerDepth = 1f
          };
          temporaryAnimatedSprite1.alphaFade = 0.025f;
          this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite1);
          TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox1.Center) + new Vector2(-32f, 0.0f), Color.LightGreen * 0.9f, 10, animationInterval: 70f)
          {
            delayBeforeAnimationStart = 50,
            motion = new Vector2((float) x, (float) y),
            layerDepth = 1f
          };
          if (index == -1)
            temporaryAnimatedSprite2.startSound = "cowboy_monsterhit";
          this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite2);
          TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox1.Center) + new Vector2(32f, 0.0f), Color.LightGreen * 0.8f, 10, animationInterval: 70f)
          {
            delayBeforeAnimationStart = 100,
            startSound = "cowboy_monsterhit",
            motion = new Vector2((float) x, (float) y) * 0.8f,
            layerDepth = 1f
          };
          if (index == -1)
            temporaryAnimatedSprite3.startSound = "cowboy_monsterhit";
          this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite3);
          TemporaryAnimatedSprite temporaryAnimatedSprite4 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox1.Center) + new Vector2(0.0f, -32f), Color.LightGreen * 0.7f, 10)
          {
            delayBeforeAnimationStart = 150,
            startSound = "cowboy_monsterhit",
            motion = new Vector2((float) x, (float) y) * 0.6f,
            layerDepth = 1f
          };
          if (index == -1)
            temporaryAnimatedSprite4.startSound = "cowboy_monsterhit";
          this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite4);
          TemporaryAnimatedSprite temporaryAnimatedSprite5 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox1.Center), Color.LightGreen * 0.6f, 10, animationInterval: 70f)
          {
            delayBeforeAnimationStart = 200,
            startSound = "cowboy_monsterhit",
            motion = new Vector2((float) x, (float) y) * 0.4f,
            layerDepth = 1f
          };
          if (index == -1)
            temporaryAnimatedSprite5.startSound = "cowboy_monsterhit";
          this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite5);
          TemporaryAnimatedSprite temporaryAnimatedSprite6 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox1.Center) + new Vector2(0.0f, 32f), Color.LightGreen * 0.5f, 10)
          {
            delayBeforeAnimationStart = 250,
            startSound = "cowboy_monsterhit",
            motion = new Vector2((float) x, (float) y) * 0.2f,
            layerDepth = 1f
          };
          if (index == -1)
            temporaryAnimatedSprite6.startSound = "cowboy_monsterhit";
          this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite6);
        }
      }
      else
      {
        this.currentLocation.localSound("serpentDie");
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(0, 64, 32, 32), 200f, 4, 0, this.Position, false, false, 0.9f, 1f / 1000f, Color.White, 4f * (float) (NetFieldBase<float, NetFloat>) this.scale, 0.01f, this.rotation + 3.141593f, (float) ((double) Game1.random.Next(3, 5) * Math.PI / 64.0))
        {
          motion = new Vector2((float) x, (float) y),
          layerDepth = 1f
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, Utility.PointToVector2(this.GetBoundingBox().Center) + new Vector2(-32f, 0.0f), Color.LightGreen * 0.9f, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 50,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y),
          layerDepth = 1f
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, Utility.PointToVector2(this.GetBoundingBox().Center) + new Vector2(32f, 0.0f), Color.LightGreen * 0.8f, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 100,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.8f,
          layerDepth = 1f
        });
        List<TemporaryAnimatedSprite> temporarySprites1 = this.currentLocation.temporarySprites;
        Rectangle boundingBox2 = this.GetBoundingBox();
        temporarySprites1.Add(new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox2.Center) + new Vector2(0.0f, -32f), Color.LightGreen * 0.7f, 10)
        {
          delayBeforeAnimationStart = 150,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.6f,
          layerDepth = 1f
        });
        List<TemporaryAnimatedSprite> temporarySprites2 = this.currentLocation.temporarySprites;
        boundingBox2 = this.GetBoundingBox();
        temporarySprites2.Add(new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox2.Center), Color.LightGreen * 0.6f, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 200,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.4f,
          layerDepth = 1f
        });
        List<TemporaryAnimatedSprite> temporarySprites3 = this.currentLocation.temporarySprites;
        boundingBox2 = this.GetBoundingBox();
        temporarySprites3.Add(new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox2.Center) + new Vector2(0.0f, 32f), Color.LightGreen * 0.5f, 10)
        {
          delayBeforeAnimationStart = 250,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.2f,
          layerDepth = 1f
        });
      }
    }

    public override List<Item> getExtraDropItems()
    {
      List<Item> extraDropItems = new List<Item>();
      if (Game1.random.NextDouble() < 0.002)
        extraDropItems.Add((Item) new StardewValley.Object(485, 1));
      return extraDropItems;
    }

    public override void drawAboveAllLayers(SpriteBatch b)
    {
      Vector2 globalPosition1 = this.Position;
      bool flag = this.IsRoyalSerpent();
      for (int index = -1; index < this.segmentCount.Value; ++index)
      {
        Vector2 vector2_1 = Vector2.Zero;
        float num1 = (float) ((double) (index + 1) * -0.25 / 10000.0);
        float num2 = (float) ((double) (int) (NetFieldBase<int, NetInt>) this.segmentCount * -0.25 / 10000.0 - 4.99999987368938E-05);
        if ((double) (this.getStandingY() - 1) / 10000.0 + (double) num2 < 0.0)
          num1 += (float) -((double) (this.getStandingY() - 1) / 10000.0 + (double) num2);
        Rectangle rectangle = this.Sprite.SourceRect;
        Vector2 globalPosition2 = this.Position;
        float rotation1;
        if (index == -1)
        {
          if (flag)
            rectangle = new Rectangle(0, 0, 32, 32);
          vector2_1 = this.Position;
          rotation1 = this.rotation;
        }
        else
        {
          if (index >= this.segments.Count)
            break;
          Vector3 segment = this.segments[index];
          vector2_1 = new Vector2(segment.X, segment.Y);
          rectangle = new Rectangle(32, 0, 32, 32);
          if (index == this.segments.Count - 1)
            rectangle = new Rectangle(64, 0, 32, 32);
          rotation1 = segment.Z;
          globalPosition2 = (globalPosition1 + vector2_1) / 2f;
        }
        if (Utility.isOnScreen(vector2_1, 128))
        {
          Vector2 vector2_2 = Game1.GlobalToLocal(Game1.viewport, vector2_1) + (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawOffset + new Vector2(0.0f, (float) this.yJumpOffset);
          Vector2 vector2_3 = Game1.GlobalToLocal(Game1.viewport, globalPosition2) + (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawOffset + new Vector2(0.0f, (float) this.yJumpOffset);
          SpriteBatch spriteBatch = b;
          Texture2D shadowTexture = Game1.shadowTexture;
          Vector2 position = vector2_3 + new Vector2(64f, (float) this.GetBoundingBox().Height);
          Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
          Color white = Color.White;
          Rectangle bounds = Game1.shadowTexture.Bounds;
          double x = (double) bounds.Center.X;
          bounds = Game1.shadowTexture.Bounds;
          double y = (double) bounds.Center.Y;
          Vector2 origin = new Vector2((float) x, (float) y);
          double layerDepth = (double) (this.getStandingY() - 1) / 10000.0 + (double) num1;
          spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
          b.Draw(this.Sprite.Texture, vector2_2 + new Vector2(64f, (float) (this.GetBoundingBox().Height / 2)), new Rectangle?(rectangle), Color.White, rotation1, new Vector2(16f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (this.getStandingY() + 8) / 10000f + num1));
          if (this.isGlowing)
            b.Draw(this.Sprite.Texture, vector2_2 + new Vector2(64f, (float) (this.GetBoundingBox().Height / 2)), new Rectangle?(rectangle), this.glowingColor * this.glowingTransparency, rotation1, new Vector2(16f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) (this.getStandingY() + 8) / 10000.0 + 9.99999974737875E-05) + num1));
          if (flag)
          {
            float num3 = num1 - 5E-05f;
            float rotation2 = 0.0f;
            rectangle = new Rectangle(96, 0, 32, 32);
            Vector2 vector2_4 = Game1.GlobalToLocal(Game1.viewport, globalPosition1) + (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawOffset + new Vector2(0.0f, (float) this.yJumpOffset);
            if (index > 0)
              b.Draw(Game1.shadowTexture, vector2_4 + new Vector2(64f, (float) this.GetBoundingBox().Height), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) (this.getStandingY() - 1) / 10000f + num3);
            b.Draw(this.Sprite.Texture, vector2_4 + new Vector2(64f, (float) (this.GetBoundingBox().Height / 2)), new Rectangle?(rectangle), Color.White, rotation2, new Vector2(16f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (this.getStandingY() + 8) / 10000f + num3));
            if (this.isGlowing)
              b.Draw(this.Sprite.Texture, vector2_4 + new Vector2(64f, (float) (this.GetBoundingBox().Height / 2)), new Rectangle?(rectangle), this.glowingColor * this.glowingTransparency, rotation2, new Vector2(16f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) (this.getStandingY() + 8) / 10000.0 + 9.99999974737875E-05) + num3));
          }
        }
        globalPosition1 = vector2_1;
      }
    }

    public override Rectangle GetBoundingBox()
    {
      Vector2 position = this.Position;
      return new Rectangle((int) position.X + 8, (int) position.Y, this.Sprite.SpriteWidth * 4 * 3 / 4, 96);
    }

    protected override void updateAnimation(GameTime time)
    {
      if (this.IsRoyalSerpent())
      {
        if (this.segments.Count < this.segmentCount.Value)
        {
          for (int index = 0; index < this.segmentCount.Value; ++index)
          {
            Vector2 position = this.Position;
            this.segments.Add(new Vector3(position.X, position.Y, 0.0f));
          }
        }
        Vector2 vector2_1 = this.Position;
        for (int index = 0; index < this.segments.Count; ++index)
        {
          Vector2 vector2_2 = new Vector2(this.segments[index].X, this.segments[index].Y);
          Vector2 vector2_3 = vector2_2 - vector2_1;
          int num1 = 64;
          int num2 = (int) vector2_3.Length();
          vector2_3.Normalize();
          int num3 = num1;
          if (num2 > num3)
            vector2_2 = vector2_3 * (float) num1 + vector2_1;
          double z = Math.Atan2((double) vector2_3.Y, (double) vector2_3.X) - Math.PI / 2.0;
          this.segments[index] = new Vector3(vector2_2.X, vector2_2.Y, (float) z);
          vector2_1 = vector2_2;
        }
      }
      base.updateAnimation(time);
      if (this.wasHitCounter >= 0)
        this.wasHitCounter -= time.ElapsedGameTime.Milliseconds;
      if (!this.IsRoyalSerpent())
        this.Sprite.Animate(time, 0, 9, 40f);
      if (this.withinPlayerThreshold() && this.invincibleCountdown <= 0)
      {
        Rectangle boundingBox = this.Player.GetBoundingBox();
        int x1 = boundingBox.Center.X;
        boundingBox = this.GetBoundingBox();
        int x2 = boundingBox.Center.X;
        float num4 = (float) -(x1 - x2);
        float num5 = (float) (this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y);
        float num6 = Math.Max(1f, Math.Abs(num4) + Math.Abs(num5));
        if ((double) num6 < 64.0)
        {
          this.xVelocity = Math.Max(-7f, Math.Min(7f, this.xVelocity * 1.1f));
          this.yVelocity = Math.Max(-7f, Math.Min(7f, this.yVelocity * 1.1f));
        }
        float x3 = num4 / num6;
        float num7 = num5 / num6;
        if (this.wasHitCounter <= 0)
        {
          this.targetRotation = (float) Math.Atan2(-(double) num7, (double) x3) - 1.570796f;
          if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextDouble() < 0.5)
            this.turningRight = true;
          else if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) < Math.PI / 8.0)
            this.turningRight = false;
          if (this.turningRight)
            this.rotation -= (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
          else
            this.rotation += (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
          this.rotation %= 6.283185f;
          this.wasHitCounter = 5 + Game1.random.Next(-1, 2);
        }
        float num8 = Math.Min(7f, Math.Max(2f, (float) (7.0 - (double) num6 / 64.0 / 2.0)));
        float num9 = (float) Math.Cos((double) this.rotation + Math.PI / 2.0);
        float num10 = -(float) Math.Sin((double) this.rotation + Math.PI / 2.0);
        this.xVelocity += (float) (-(double) num9 * (double) num8 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
        this.yVelocity += (float) (-(double) num10 * (double) num8 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
        if ((double) Math.Abs(this.xVelocity) > (double) Math.Abs((float) (-(double) num9 * 7.0)))
          this.xVelocity -= (float) (-(double) num9 * (double) num8 / 6.0);
        if ((double) Math.Abs(this.yVelocity) > (double) Math.Abs((float) (-(double) num10 * 7.0)))
          this.yVelocity -= (float) (-(double) num10 * (double) num8 / 6.0);
      }
      this.resetAnimationSpeed();
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
        this.Health = -500;
      if ((double) this.Position.X <= -640.0 || (double) this.Position.Y <= -640.0 || (double) this.Position.X >= (double) (this.currentLocation.Map.Layers[0].LayerWidth * 64 + 640) || (double) this.Position.Y >= (double) (this.currentLocation.Map.Layers[0].LayerHeight * 64 + 640))
        this.Health = -500;
      if (!this.withinPlayerThreshold() || this.invincibleCountdown > 0)
        return;
      this.faceDirection(2);
    }
  }
}
