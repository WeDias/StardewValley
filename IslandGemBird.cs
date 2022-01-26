// Decompiled with JetBrains decompiler
// Type: StardewValley.IslandGemBird
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class IslandGemBird : INetObject<NetFields>
  {
    [XmlIgnore]
    public Texture2D texture;
    [XmlElement("position")]
    public NetVector2 position = new NetVector2();
    [XmlIgnore]
    protected float _destroyTimer;
    [XmlElement("height")]
    public NetFloat height = new NetFloat();
    [XmlIgnore]
    public int[] idleAnimation = new int[1];
    [XmlIgnore]
    public int[] lookBackAnimation = new int[17]
    {
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1,
      1
    };
    [XmlIgnore]
    public int[] scratchAnimation = new int[19]
    {
      0,
      1,
      2,
      3,
      2,
      3,
      2,
      3,
      2,
      3,
      2,
      3,
      2,
      3,
      2,
      3,
      2,
      3,
      2
    };
    [XmlIgnore]
    public int[] flyAnimation = new int[11]
    {
      4,
      5,
      6,
      7,
      7,
      6,
      6,
      5,
      5,
      4,
      4
    };
    [XmlIgnore]
    public int[] currentAnimation;
    [XmlIgnore]
    public float frameTimer;
    [XmlIgnore]
    public int currentFrameIndex;
    [XmlIgnore]
    public float idleAnimationTime;
    [XmlElement("alpha")]
    public NetFloat alpha = new NetFloat(1f);
    [XmlElement("flying")]
    public NetBool flying = new NetBool();
    [XmlElement("color")]
    public NetColor color = new NetColor();
    [XmlElement("itemIndex")]
    public NetInt itemIndex = new NetInt();

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public IslandGemBird()
    {
      this.texture = Game1.content.Load<Texture2D>("LooseSprites\\GemBird");
      this.InitNetFields();
    }

    public IslandGemBird(Vector2 tile_position, IslandGemBird.GemBirdType bird_type)
      : this()
    {
      this.position.Value = (tile_position + new Vector2(0.5f, 0.5f)) * 64f;
      this.color.Value = IslandGemBird.GetColor(bird_type);
      this.itemIndex.Value = IslandGemBird.GetItemIndex(bird_type);
    }

    public static Color GetColor(IslandGemBird.GemBirdType bird_type)
    {
      switch (bird_type)
      {
        case IslandGemBird.GemBirdType.Emerald:
          return new Color(67, (int) byte.MaxValue, 83);
        case IslandGemBird.GemBirdType.Aquamarine:
          return new Color(74, 243, (int) byte.MaxValue);
        case IslandGemBird.GemBirdType.Ruby:
          return new Color((int) byte.MaxValue, 38, 38);
        case IslandGemBird.GemBirdType.Amethyst:
          return new Color((int) byte.MaxValue, 67, 251);
        case IslandGemBird.GemBirdType.Topaz:
          return new Color((int) byte.MaxValue, 156, 33);
        default:
          return Color.White;
      }
    }

    public static int GetItemIndex(IslandGemBird.GemBirdType bird_type)
    {
      switch (bird_type)
      {
        case IslandGemBird.GemBirdType.Emerald:
          return 60;
        case IslandGemBird.GemBirdType.Aquamarine:
          return 62;
        case IslandGemBird.GemBirdType.Ruby:
          return 64;
        case IslandGemBird.GemBirdType.Amethyst:
          return 66;
        case IslandGemBird.GemBirdType.Topaz:
          return 68;
        default:
          return 0;
      }
    }

    public static IslandGemBird.GemBirdType GetBirdTypeForLocation(string location)
    {
      List<string> stringList = new List<string>();
      stringList.Add("IslandNorth");
      stringList.Add("IslandSouth");
      stringList.Add("IslandEast");
      stringList.Add("IslandWest");
      if (!stringList.Contains(location))
        return IslandGemBird.GemBirdType.Aquamarine;
      Random rng = new Random((int) Game1.uniqueIDForThisGame);
      List<IslandGemBird.GemBirdType> list = new List<IslandGemBird.GemBirdType>();
      for (int index = 0; index < 5; ++index)
        list.Add((IslandGemBird.GemBirdType) index);
      Utility.Shuffle<IslandGemBird.GemBirdType>(rng, list);
      return list[stringList.IndexOf(location)];
    }

    public void Draw(SpriteBatch b)
    {
      if (this.currentAnimation == null)
        return;
      int num = this.currentAnimation[Math.Min(this.currentFrameIndex, this.currentAnimation.Length - 1)];
      b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(0.0f, -this.height.Value)), new Rectangle?(new Rectangle(num * 32, 0, 32, 32)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(16f, 32f), 4f, SpriteEffects.None, (float) (((double) this.position.Value.Y - 1.0) / 10000.0));
      b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(0.0f, -this.height.Value)), new Rectangle?(new Rectangle(num * 32, 32, 32, 32)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(16f, 32f), 4f, SpriteEffects.None, this.position.Value.Y / 10000f);
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position.Value), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, (float) (((double) this.position.Y - 2.0) / 10000.0));
    }

    public void InitNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.position, (INetSerializable) this.flying, (INetSerializable) this.height, (INetSerializable) this.color, (INetSerializable) this.alpha, (INetSerializable) this.itemIndex);
      this.position.Interpolated(true, true);
      this.height.Interpolated(true, true);
      this.alpha.Interpolated(true, true);
    }

    public bool Update(GameTime time, GameLocation location)
    {
      if (this.currentAnimation == null)
        this.currentAnimation = this.idleAnimation;
      double frameTimer = (double) this.frameTimer;
      TimeSpan elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds1 = elapsedGameTime.TotalSeconds;
      this.frameTimer = (float) (frameTimer + totalSeconds1);
      float num = 0.15f;
      if ((bool) (NetFieldBase<bool, NetBool>) this.flying)
        num = 0.05f;
      if ((double) this.frameTimer >= (double) num)
      {
        this.frameTimer = 0.0f;
        ++this.currentFrameIndex;
        if (this.currentFrameIndex >= this.currentAnimation.Length)
        {
          this.currentFrameIndex = 0;
          if (this.currentAnimation == this.flyAnimation && location == Game1.currentLocation && Utility.isOnScreen(this.position.Value + new Vector2(0.0f, -this.height.Value), 64))
            Game1.playSound("batFlap");
          if (this.currentAnimation == this.lookBackAnimation || this.currentAnimation == this.scratchAnimation)
            this.currentAnimation = this.idleAnimation;
        }
      }
      if (this.flying.Value)
      {
        this.currentAnimation = this.flyAnimation;
        if (Game1.IsMasterGame)
        {
          this.height.Value += 4f;
          this.position.X -= 3f;
          if ((double) this.alpha.Value > 0.0 && (double) (float) (NetFieldBase<float, NetFloat>) this.height >= 300.0)
          {
            this.alpha.Value -= 0.01f;
            if ((double) this.alpha.Value < 0.0)
              this.alpha.Value = 0.0f;
          }
        }
      }
      else
      {
        if (this.currentAnimation == this.idleAnimation)
        {
          double idleAnimationTime = (double) this.idleAnimationTime;
          elapsedGameTime = time.ElapsedGameTime;
          double totalSeconds2 = elapsedGameTime.TotalSeconds;
          this.idleAnimationTime = (float) (idleAnimationTime - totalSeconds2);
        }
        if ((double) this.idleAnimationTime <= 0.0)
        {
          this.currentFrameIndex = 0;
          this.currentAnimation = Game1.random.NextDouble() >= 0.75 ? this.scratchAnimation : this.lookBackAnimation;
          this.idleAnimationTime = Utility.RandomFloat(1f, 3f);
        }
      }
      if (Game1.IsMasterGame && !this.flying.Value)
      {
        foreach (Character farmer in location.farmers)
        {
          Vector2 vector2 = farmer.Position - this.position.Value;
          if ((double) Math.Abs(vector2.X) <= 128.0 && (double) Math.Abs(vector2.Y) <= 128.0)
          {
            this.flying.Value = true;
            location.playSound("parrot");
            Game1.createObjectDebris(this.itemIndex.Value, (int) ((double) this.position.X / 64.0), (int) ((double) this.position.Y / 64.0), location);
          }
        }
      }
      if ((double) this.alpha.Value <= 0.0)
      {
        if ((double) this._destroyTimer == 0.0)
          this._destroyTimer = 3f;
        else if ((double) this._destroyTimer >= 0.0)
        {
          double destroyTimer = (double) this._destroyTimer;
          elapsedGameTime = time.ElapsedGameTime;
          double totalSeconds3 = elapsedGameTime.TotalSeconds;
          this._destroyTimer = (float) (destroyTimer - totalSeconds3);
          if ((double) this._destroyTimer <= 0.0)
            return true;
        }
      }
      return false;
    }

    public enum GemBirdType
    {
      Emerald,
      Aquamarine,
      Ruby,
      Amethyst,
      Topaz,
      MAX,
    }
  }
}
