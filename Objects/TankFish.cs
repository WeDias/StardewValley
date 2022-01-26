// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.TankFish
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.Objects
{
  public class TankFish
  {
    protected FishTankFurniture _tank;
    public Vector2 position;
    public float zPosition;
    public bool facingLeft;
    public Vector2 velocity = Vector2.Zero;
    protected Texture2D _texture;
    public float nextSwim;
    public int fishIndex;
    public int currentFrame;
    public int numberOfDarts;
    public TankFish.FishType fishType;
    public float minimumVelocity;
    public float fishScale = 1f;
    public List<int> currentAnimation;
    public List<int> idleAnimation;
    public List<int> dartStartAnimation;
    public List<int> dartHoldAnimation;
    public List<int> dartEndAnimation;
    public int currentAnimationFrame;
    public float currentFrameTime;
    public float nextBubble;

    public TankFish(FishTankFurniture tank, Item item)
    {
      this._tank = tank;
      this._texture = this._tank.GetAquariumTexture();
      string[] strArray1 = this._tank.GetAquariumData()[item.ParentSheetIndex].Split('/');
      this.fishIndex = int.Parse(strArray1[0]);
      this.currentFrame = this.fishIndex;
      this.zPosition = Utility.RandomFloat(4f, 10f);
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
      this.fishScale = 0.75f;
      if (dictionary.ContainsKey(item.ParentSheetIndex))
      {
        string[] strArray2 = dictionary[item.ParentSheetIndex].Split('/');
        if (!(strArray2[1] == "trap"))
        {
          this.minimumVelocity = Utility.RandomFloat(0.25f, 0.35f);
          if (strArray2[2] == "smooth")
            this.minimumVelocity = Utility.RandomFloat(0.5f, 0.6f);
          if (strArray2[2] == "dart")
            this.minimumVelocity = 0.0f;
        }
      }
      if (strArray1.Length > 1)
      {
        string str = strArray1[1];
        if (str == "eel")
        {
          this.fishType = TankFish.FishType.Eel;
          this.minimumVelocity = Utility.Clamp(this.fishScale, 0.3f, 0.4f);
        }
        else if (str == "cephalopod")
        {
          this.fishType = TankFish.FishType.Cephalopod;
          this.minimumVelocity = 0.0f;
        }
        else if (str == "ground")
        {
          this.fishType = TankFish.FishType.Ground;
          this.zPosition = 4f;
          this.minimumVelocity = 0.0f;
        }
        else if (str == "static")
          this.fishType = TankFish.FishType.Static;
        else if (str == "crawl")
        {
          this.fishType = TankFish.FishType.Crawl;
          this.minimumVelocity = 0.0f;
        }
        else if (str == "front_crawl")
        {
          this.fishType = TankFish.FishType.Crawl;
          this.zPosition = 3f;
          this.minimumVelocity = 0.0f;
        }
        else if (str == "float")
          this.fishType = TankFish.FishType.Float;
      }
      if (strArray1.Length > 2)
      {
        string[] strArray3 = strArray1[2].Split(' ');
        this.idleAnimation = new List<int>();
        foreach (string s in strArray3)
          this.idleAnimation.Add(int.Parse(s));
        this.SetAnimation(this.idleAnimation);
      }
      if (strArray1.Length > 3)
      {
        string str = strArray1[3];
        string[] strArray4 = str.Split(' ');
        this.dartStartAnimation = new List<int>();
        if (str != "")
        {
          foreach (string s in strArray4)
            this.dartStartAnimation.Add(int.Parse(s));
        }
      }
      if (strArray1.Length > 4)
      {
        string str = strArray1[4];
        string[] strArray5 = str.Split(' ');
        this.dartHoldAnimation = new List<int>();
        if (str != "")
        {
          foreach (string s in strArray5)
            this.dartHoldAnimation.Add(int.Parse(s));
        }
      }
      if (strArray1.Length > 5)
      {
        string str = strArray1[5];
        string[] strArray6 = str.Split(' ');
        this.dartEndAnimation = new List<int>();
        if (str != "")
        {
          foreach (string s in strArray6)
            this.dartEndAnimation.Add(int.Parse(s));
        }
      }
      Rectangle tankBounds = this._tank.GetTankBounds() with
      {
        X = 0,
        Y = 0
      };
      this.position = Vector2.Zero;
      this.position = Utility.getRandomPositionInThisRectangle(tankBounds, Game1.random);
      this.nextSwim = Utility.RandomFloat(0.1f, 10f);
      this.nextBubble = Utility.RandomFloat(0.1f, 10f);
      this.facingLeft = Game1.random.Next(2) == 1;
      this.velocity = !this.facingLeft ? new Vector2(1f, 0.0f) : new Vector2(-1f, 0.0f);
      this.velocity *= this.minimumVelocity;
      if (this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Static)
        this.position.Y = 0.0f;
      this.ConstrainToTank();
    }

    public void SetAnimation(List<int> frames)
    {
      if (this.currentAnimation == frames)
        return;
      this.currentAnimation = frames;
      this.currentAnimationFrame = 0;
      this.currentFrameTime = 0.0f;
      if (this.currentAnimation == null || this.currentAnimation.Count <= 0)
        return;
      this.currentFrame = frames[0];
    }

    public virtual void Draw(SpriteBatch b, float alpha, float draw_layer)
    {
      SpriteEffects effects1 = SpriteEffects.None;
      int num1 = -12;
      int width = 8;
      if (this.fishType == TankFish.FishType.Eel)
        width = 4;
      int num2 = width;
      if (this.facingLeft)
      {
        effects1 = SpriteEffects.FlipHorizontally;
        num2 *= -1;
        num1 = -num1 - width + 1;
      }
      TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
      float y1 = (float) Math.Sin(totalGameTime.TotalSeconds * 1.25 + (double) this.position.X / 32.0) * 2f;
      if (this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Static)
        y1 = 0.0f;
      float scale1 = this.GetScale();
      int num3 = this._texture.Width / 24;
      int x1 = this.currentFrame % num3 * 24;
      int y2 = this.currentFrame / num3 * 48;
      int num4 = 10;
      float num5 = 1f;
      if (this.fishType == TankFish.FishType.Eel)
      {
        num4 = 20;
        y1 *= 0.0f;
      }
      if (this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Static)
      {
        float rotation = 0.0f;
        b.Draw(this._texture, Game1.GlobalToLocal(this.GetWorldPosition() + new Vector2(0.0f, y1) * 4f * scale1), new Rectangle?(new Rectangle(x1, y2, 24, 24)), Color.White * alpha, rotation, new Vector2(12f, 12f), 4f * scale1, effects1, draw_layer);
      }
      else if (this.fishType == TankFish.FishType.Cephalopod || this.fishType == TankFish.FishType.Float)
      {
        float rotation = Utility.Clamp(this.velocity.X, -0.5f, 0.5f);
        b.Draw(this._texture, Game1.GlobalToLocal(this.GetWorldPosition() + new Vector2(0.0f, y1) * 4f * scale1), new Rectangle?(new Rectangle(x1, y2, 24, 24)), Color.White * alpha, rotation, new Vector2(12f, 12f), 4f * scale1, effects1, draw_layer);
      }
      else
      {
        for (int index = 0; index < 24 / width; ++index)
        {
          float num6 = 1f - (float) (index * width) / (float) num4;
          float num7 = this.velocity.Length() / 1f;
          float num8 = 1f;
          float num9 = 0.0f;
          float num10 = Utility.Clamp(num7, 0.2f, 1f);
          float num11 = Utility.Clamp(num6, 0.0f, 1f);
          if (this.fishType == TankFish.FishType.Eel)
          {
            num11 = 1f;
            num10 = 1f;
            num8 = 0.1f;
            num9 = 4f;
          }
          if (this.facingLeft)
            num9 *= -1f;
          SpriteBatch spriteBatch = b;
          Texture2D texture = this._texture;
          Vector2 worldPosition = this.GetWorldPosition();
          double x2 = (double) (num1 + index * num2);
          double num12 = (double) y1;
          double num13 = (double) (index * 20);
          totalGameTime = Game1.currentGameTime.TotalGameTime;
          double num14 = totalGameTime.TotalSeconds * 25.0 * (double) num8;
          double num15 = (double) ((float) Math.Sin(num13 + num14 + (double) num9 * (double) this.position.X / 16.0) * num5 * num11 * num10);
          double y3 = num12 + num15;
          Vector2 vector2 = new Vector2((float) x2, (float) y3) * 4f * scale1;
          Vector2 local = Game1.GlobalToLocal(worldPosition + vector2);
          Rectangle? sourceRectangle = new Rectangle?(new Rectangle(x1 + index * width, y2, width, 24));
          Color color = Color.White * alpha;
          Vector2 origin = new Vector2(0.0f, 12f);
          double scale2 = 4.0 * (double) scale1;
          int effects2 = (int) effects1;
          double layerDepth = (double) draw_layer;
          spriteBatch.Draw(texture, local, sourceRectangle, color, 0.0f, origin, (float) scale2, (SpriteEffects) effects2, (float) layerDepth);
        }
      }
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(new Vector2(this.GetWorldPosition().X, (float) this._tank.GetTankBounds().Bottom - this.zPosition * 4f)), new Rectangle?(), Color.White * alpha * 0.75f, 0.0f, new Vector2((float) (Game1.shadowTexture.Width / 2), (float) (Game1.shadowTexture.Height / 2)), new Vector2(4f * scale1, 1f), SpriteEffects.None, this._tank.GetFishSortRegion().X - 1E-07f);
    }

    public Vector2 GetWorldPosition() => new Vector2((float) this._tank.GetTankBounds().X + this.position.X, (float) ((double) this._tank.GetTankBounds().Bottom - (double) this.position.Y - (double) this.zPosition * 4.0));

    public void ConstrainToTank()
    {
      Rectangle tankBounds = this._tank.GetTankBounds();
      Rectangle bounds1 = this.GetBounds();
      tankBounds.X = 0;
      tankBounds.Y = 0;
      if (bounds1.X < tankBounds.X)
      {
        this.position.X += (float) (tankBounds.X - bounds1.X);
        bounds1 = this.GetBounds();
      }
      if (bounds1.Y < tankBounds.Y)
      {
        this.position.Y -= (float) (tankBounds.Y - bounds1.Y);
        bounds1 = this.GetBounds();
      }
      if (bounds1.Right > tankBounds.Right)
      {
        this.position.X += (float) (tankBounds.Right - bounds1.Right);
        bounds1 = this.GetBounds();
      }
      Rectangle bounds2;
      if (this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Static)
      {
        if ((double) this.position.Y <= (double) tankBounds.Bottom)
          return;
        this.position.Y -= (float) tankBounds.Bottom - this.position.Y;
        bounds2 = this.GetBounds();
      }
      else
      {
        if (bounds1.Bottom <= tankBounds.Bottom)
          return;
        this.position.Y -= (float) (tankBounds.Bottom - bounds1.Bottom);
        bounds2 = this.GetBounds();
      }
    }

    public virtual float GetScale() => this.fishScale;

    public Rectangle GetBounds()
    {
      Vector2 vector2 = new Vector2(24f, 18f) * (4f * this.GetScale());
      return this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Static ? new Rectangle((int) ((double) this.position.X - (double) vector2.X / 2.0), (int) ((double) this._tank.GetTankBounds().Height - (double) this.position.Y - (double) vector2.Y), (int) vector2.X, (int) vector2.Y) : new Rectangle((int) ((double) this.position.X - (double) vector2.X / 2.0), (int) ((double) this._tank.GetTankBounds().Height - (double) this.position.Y - (double) vector2.Y / 2.0), (int) vector2.X, (int) vector2.Y);
    }

    public virtual void Update(GameTime time)
    {
      if (this.currentAnimation != null && this.currentAnimation.Count > 0)
      {
        this.currentFrameTime += (float) time.ElapsedGameTime.TotalSeconds;
        float num = 0.125f;
        if ((double) this.currentFrameTime > (double) num)
        {
          this.currentAnimationFrame += (int) ((double) this.currentFrameTime / (double) num);
          this.currentFrameTime %= num;
          if (this.currentAnimationFrame >= this.currentAnimation.Count)
          {
            if (this.currentAnimation == this.idleAnimation)
            {
              this.currentAnimationFrame %= this.currentAnimation.Count;
              this.currentFrame = this.currentAnimation[this.currentAnimationFrame];
            }
            else if (this.currentAnimation == this.dartStartAnimation)
            {
              if (this.dartHoldAnimation != null)
                this.SetAnimation(this.dartHoldAnimation);
              else
                this.SetAnimation(this.idleAnimation);
            }
            else if (this.currentAnimation == this.dartHoldAnimation)
            {
              this.currentAnimationFrame %= this.currentAnimation.Count;
              this.currentFrame = this.currentAnimation[this.currentAnimationFrame];
            }
            else if (this.currentAnimation == this.dartEndAnimation)
              this.SetAnimation(this.idleAnimation);
          }
          else
            this.currentFrame = this.currentAnimation[this.currentAnimationFrame];
        }
      }
      if (this.fishType != TankFish.FishType.Static)
      {
        Rectangle tankBounds = this._tank.GetTankBounds() with
        {
          X = 0,
          Y = 0
        };
        float num1 = this.velocity.X;
        if (this.fishType == TankFish.FishType.Crawl)
          num1 = Utility.Clamp(num1, -0.5f, 0.5f);
        this.position.X += num1;
        Rectangle bounds1 = this.GetBounds();
        if (bounds1.Left < tankBounds.Left || bounds1.Right > tankBounds.Right)
        {
          this.ConstrainToTank();
          this.GetBounds();
          this.velocity.X *= -1f;
          this.facingLeft = !this.facingLeft;
        }
        this.position.Y += this.velocity.Y;
        Rectangle bounds2 = this.GetBounds();
        if (bounds2.Top < tankBounds.Top || bounds2.Bottom > tankBounds.Bottom)
        {
          this.ConstrainToTank();
          this.velocity.Y *= 0.0f;
        }
        float a = this.velocity.Length();
        if ((double) a > (double) this.minimumVelocity)
        {
          float t = 0.015f;
          if (this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground)
            t = 0.03f;
          float num2 = Utility.Lerp(a, this.minimumVelocity, t);
          if ((double) num2 < 9.99999974737875E-05)
            num2 = 0.0f;
          this.velocity.Normalize();
          this.velocity *= num2;
          if (this.currentAnimation == this.dartHoldAnimation && (double) num2 <= (double) this.minimumVelocity + 0.5)
          {
            if (this.dartEndAnimation != null && this.dartEndAnimation.Count > 0)
              this.SetAnimation(this.dartEndAnimation);
            else if (this.idleAnimation != null && this.idleAnimation.Count > 0)
              this.SetAnimation(this.idleAnimation);
          }
        }
        this.nextSwim -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.nextSwim <= 0.0)
        {
          if (this.numberOfDarts == 0)
          {
            this.numberOfDarts = Game1.random.Next(1, 4);
            this.nextSwim = Utility.RandomFloat(6f, 12f);
            if (this.fishType == TankFish.FishType.Cephalopod)
              this.nextSwim = Utility.RandomFloat(2f, 5f);
            if (Game1.random.NextDouble() < 0.300000011920929)
              this.facingLeft = !this.facingLeft;
          }
          else
          {
            this.nextSwim = Utility.RandomFloat(0.1f, 0.5f);
            --this.numberOfDarts;
            if (Game1.random.NextDouble() < 0.0500000007450581)
              this.facingLeft = !this.facingLeft;
          }
          if (this.dartStartAnimation != null && this.dartStartAnimation.Count > 0)
            this.SetAnimation(this.dartStartAnimation);
          else if (this.dartHoldAnimation != null && this.dartHoldAnimation.Count > 0)
            this.SetAnimation(this.dartHoldAnimation);
          this.velocity.X = 1.5f;
          if (this._tank.getTilesWide() <= 2)
            this.velocity.X *= 0.5f;
          if (this.facingLeft)
            this.velocity.X *= -1f;
          if (this.fishType == TankFish.FishType.Cephalopod)
            this.velocity.Y = Utility.RandomFloat(0.5f, 0.75f);
          else if (this.fishType == TankFish.FishType.Ground)
          {
            this.velocity.X *= 0.5f;
            this.velocity.Y = Utility.RandomFloat(0.5f, 0.25f);
          }
          else
            this.velocity.Y = Utility.RandomFloat(-0.5f, 0.5f);
          if (this.fishType == TankFish.FishType.Crawl)
            this.velocity.Y = 0.0f;
        }
      }
      if (this.fishType == TankFish.FishType.Cephalopod || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Static)
      {
        float num = 0.2f;
        if (this.fishType == TankFish.FishType.Static)
          num = 0.6f;
        if ((double) this.position.Y > 0.0)
          this.position.Y -= num;
      }
      this.nextBubble -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.nextBubble <= 0.0)
      {
        this.nextBubble = Utility.RandomFloat(1f, 10f);
        float num = 0.0f;
        if (this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Normal || this.fishType == TankFish.FishType.Eel)
          num = 32f;
        if (this.facingLeft)
          num *= -1f;
        this._tank.bubbles.Add(new Vector4(this.position.X + num * this.fishScale, this.position.Y + this.zPosition, this.zPosition, 0.25f));
      }
      this.ConstrainToTank();
    }

    public enum FishType
    {
      Normal,
      Eel,
      Cephalopod,
      Float,
      Ground,
      Crawl,
      Static,
    }
  }
}
