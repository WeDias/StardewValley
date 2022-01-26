// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.BoatJourney
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StardewValley.Minigames
{
  public class BoatJourney : IMinigame
  {
    public float _age;
    public Texture2D texture;
    public Rectangle mapSourceRectangle;
    protected float _zoomLevel = 1f;
    protected Vector2 viewTarget = new Vector2(0.0f, 0.0f);
    protected Vector2 _upperLeft;
    public List<BoatJourney.Entity> entities;
    protected float _currentBoatSpeed;
    public float boatSpeed = 0.5f;
    public float dockSpeed = 0.1f;
    protected float _nextSlosh;
    protected bool _fadeComplete;
    public Vector2[] points = new Vector2[9]
    {
      new Vector2(293f, 53f),
      new Vector2(293f, 60f),
      new Vector2(294f, 88f),
      new Vector2(340f, 121f),
      new Vector2(357f, 215f),
      new Vector2(204f, 633f),
      new Vector2(274f, 750f),
      new Vector2(352f, 720f),
      new Vector2(352f, 700f)
    };
    protected List<Vector2> _interpolatedPoints;
    protected List<float> _cumulativeDistances;
    protected float _totalPathDistance;
    protected float traveledBoatDistance;
    protected float nextSmoke;
    public float departureDelay = 1.5f;
    protected BoatJourney.Boat _boat;
    protected List<BoatJourney.Entity> _seagulls = new List<BoatJourney.Entity>();

    public BoatJourney()
    {
      Game1.globalFadeToClear();
      Game1.changeMusicTrack("sweet", music_context: Game1.MusicContext.MiniGame);
      this.mapSourceRectangle = new Rectangle(0, 0, 640, 849);
      this.texture = Game1.temporaryContent.Load<Texture2D>("Minigames\\boatJourneyMap");
      this.changeScreenSize();
      Rectangle r = new Rectangle(0, 112, 640, 528);
      this._interpolatedPoints = new List<Vector2>();
      this._cumulativeDistances = new List<float>();
      this._interpolatedPoints.Add(this.points[0]);
      for (int index1 = 0; index1 < this.points.Length - 3; ++index1)
      {
        this._interpolatedPoints.Add(this.points[index1 + 1]);
        for (int index2 = 0; index2 < 10; ++index2)
          this._interpolatedPoints.Add(Vector2.CatmullRom(this.points[index1], this.points[index1 + 1], this.points[index1 + 2], this.points[index1 + 3], (float) index2 / 10f));
        this._interpolatedPoints.Add(this.points[index1 + 2]);
      }
      this._interpolatedPoints.Add(this.points[this.points.Length - 1]);
      Vector2 interpolatedPoint = this._interpolatedPoints[0];
      this._totalPathDistance = 0.0f;
      for (int index = 0; index < this._interpolatedPoints.Count; ++index)
      {
        this._totalPathDistance += (interpolatedPoint - this._interpolatedPoints[index]).Length();
        interpolatedPoint = this._interpolatedPoints[index];
        this._cumulativeDistances.Add(this._totalPathDistance);
      }
      this.entities = new List<BoatJourney.Entity>();
      for (int index = 0; index < 8; ++index)
      {
        Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
        Rectangle source_rect = new Rectangle(640, 0, 150, 130);
        if (Game1.random.NextDouble() < 0.449999988079071)
          source_rect = new Rectangle(640, 136, 150, 120);
        else if (Game1.random.NextDouble() < 0.25)
          source_rect = new Rectangle(640, 256, 150, 80);
        this.entities.Add(new BoatJourney.Entity(this, "Minigames\\boatJourneyMap", source_rect, new Vector2((float) (source_rect.Width / 2), (float) source_rect.Height), positionInThisRectangle)
        {
          velocity = new Vector2(-1f, -1f) * Utility.RandomFloat(0.05f, 0.15f),
          drawOnTop = true
        });
      }
      List<Vector2> other_boat_positions = new List<Vector2>();
      for (int index = 0; index < 2; ++index)
      {
        if (Game1.random.NextDouble() < 0.300000011920929)
          this.SpawnBoat(new Rectangle(640, 416, 32, 32), new Vector2(-1f, 0.0f), other_boat_positions);
      }
      if (Game1.random.NextDouble() < 0.200000002980232)
        this.SpawnBoat(new Rectangle(704, 416, 32, 32), new Vector2(-1f, 0.0f), other_boat_positions);
      for (int index = 0; index < 2; ++index)
      {
        if (Game1.random.NextDouble() < 0.300000011920929)
          this.SpawnBoat(new Rectangle(640, 448, 32, 32), new Vector2(1f, 0.0f), other_boat_positions);
      }
      for (int index = 0; index < 16; ++index)
        this.entities.Add((BoatJourney.Entity) new BoatJourney.Wave(this, Utility.getRandomPositionInThisRectangle(r, Game1.random)));
      for (int index = 0; index < 8; ++index)
        this.entities.Add((BoatJourney.Entity) new BoatJourney.WaterSparkle(this));
      Vector2 positionInThisRectangle1 = Utility.getRandomPositionInThisRectangle(r, Game1.random);
      this.CreateFlockOfSeagulls((int) positionInThisRectangle1.X, (int) positionInThisRectangle1.Y, Game1.random.Next(4, 8));
      for (int index = 0; index < 3; ++index)
      {
        Vector2 positionInThisRectangle2 = Utility.getRandomPositionInThisRectangle(r, Game1.random);
        this.CreateFlockOfSeagulls((int) positionInThisRectangle2.X, (int) positionInThisRectangle2.Y, 1);
      }
      this._seagulls.Sort((Comparison<BoatJourney.Entity>) ((a, b) => a.position.Y.CompareTo(b.position.Y)));
      this._boat = new BoatJourney.Boat(this, "Minigames\\boatJourneyMap", new Rectangle(640, 352, 32, 32), new Vector2(16f, 16f), new Vector2(293f, 53f));
      this._boat.smokeStack = new Vector2?(new Vector2(0.0f, -12f));
      this._boat.numFrames = 2;
      this.entities.Add((BoatJourney.Entity) this._boat);
      this.entities.Add(new BoatJourney.Entity(this, "Minigames\\boatJourneyMap", new Rectangle(643, 538, 29, 17), Vector2.Zero, new Vector2(16f, 829f))
      {
        numFrames = 2,
        frameInterval = 0.75f
      });
    }

    public void SpawnBoat(
      Rectangle boat_sprite_rect,
      Vector2 direction,
      List<Vector2> other_boat_positions)
    {
      Vector2 position;
      bool flag;
      do
      {
        Vector2 random;
        do
        {
          random = Utility.GetRandom<Vector2>(this._interpolatedPoints);
        }
        while (!new Rectangle(0, 112, 640, 528).Contains((int) random.X, (int) random.Y));
        position = random + direction * Utility.RandomFloat(8f, 64f);
        flag = false;
        foreach (Vector2 otherBoatPosition in other_boat_positions)
        {
          if ((double) (otherBoatPosition - position).Length() < 24.0)
          {
            flag = true;
            break;
          }
        }
      }
      while (flag);
      BoatJourney.Boat boat = new BoatJourney.Boat(this, "Minigames\\boatJourneyMap", boat_sprite_rect, new Vector2(16f, 14f), position);
      boat.velocity = direction * Utility.RandomFloat(0.05f, 0.1f);
      boat.numFrames = 2;
      boat.frameInterval = 0.75f;
      other_boat_positions.Add(position);
      this.entities.Add((BoatJourney.Entity) boat);
    }

    public void CreateFlockOfSeagulls(int x, int y, int depth)
    {
      Vector2 vector2 = new Vector2(-0.15f, -0.25f);
      BoatJourney.Entity entity1 = new BoatJourney.Entity(this, "Minigames\\boatJourneyMap", new Rectangle(646, 560, 5, 14), new Vector2(2f, 14f), new Vector2((float) x, (float) y));
      entity1.numFrames = 8;
      entity1.currentFrame = Game1.random.Next(0, 8);
      entity1.velocity = vector2 + new Vector2(Utility.RandomFloat(-1f / 1000f, 1f / 1000f), Utility.RandomFloat(-1f / 1000f, 1f / 1000f));
      entity1.frameInterval = Utility.RandomFloat(0.1f, 0.15f);
      this.entities.Add(entity1);
      this._seagulls.Add(entity1);
      Vector2 position1 = new Vector2((float) x, (float) y);
      Vector2 position2 = new Vector2((float) x, (float) y);
      for (int index = 1; index < depth; ++index)
      {
        position1.X -= (float) Game1.random.Next(5, 8);
        position1.Y += (float) Game1.random.Next(6, 9);
        position2.X += (float) Game1.random.Next(5, 8);
        position2.Y += (float) Game1.random.Next(6, 9);
        BoatJourney.Entity entity2 = new BoatJourney.Entity(this, "Minigames\\boatJourneyMap", new Rectangle(646, 560, 5, 14), new Vector2(2f, 14f), position1);
        entity2.numFrames = 8;
        entity2.currentFrame = Game1.random.Next(0, 8);
        entity2.velocity = vector2 + new Vector2(Utility.RandomFloat(-1f / 1000f, 1f / 1000f), Utility.RandomFloat(-1f / 1000f, 1f / 1000f));
        entity2.frameInterval = Utility.RandomFloat(0.1f, 0.15f);
        this.entities.Add(entity2);
        this._seagulls.Add(entity2);
        BoatJourney.Entity entity3 = new BoatJourney.Entity(this, "Minigames\\boatJourneyMap", new Rectangle(646, 560, 5, 14), new Vector2(2f, 14f), position2);
        entity3.numFrames = 8;
        entity3.currentFrame = Game1.random.Next(0, 8);
        entity3.velocity = vector2 + new Vector2(Utility.RandomFloat(-1f / 1000f, 1f / 1000f), Utility.RandomFloat(-1f / 1000f, 1f / 1000f));
        entity3.frameInterval = Utility.RandomFloat(0.1f, 0.15f);
        this.entities.Add(entity3);
        this._seagulls.Add(entity3);
      }
    }

    public Vector2 TransformDraw(Vector2 position)
    {
      position.X = (float) ((int) ((double) position.X * (double) this._zoomLevel) - (int) this._upperLeft.X);
      position.Y = (float) ((int) ((double) position.Y * (double) this._zoomLevel) - (int) this._upperLeft.Y);
      return position;
    }

    public Rectangle TransformDraw(Rectangle dest)
    {
      dest.X = (int) ((double) dest.X * (double) this._zoomLevel) - (int) this._upperLeft.X;
      dest.Y = (int) ((double) dest.Y * (double) this._zoomLevel) - (int) this._upperLeft.Y;
      dest.Width = (int) ((double) dest.Width * (double) this._zoomLevel);
      dest.Height = (int) ((double) dest.Height * (double) this._zoomLevel);
      return dest;
    }

    public bool tick(GameTime time)
    {
      if (this._fadeComplete)
      {
        Game1.warpFarmer("IslandSouth", 21, 43, 0);
        return true;
      }
      this._age += (float) time.ElapsedGameTime.TotalSeconds;
      for (int index = 0; index < this.entities.Count; ++index)
      {
        if (this.entities[index].Update(time))
        {
          this.entities.RemoveAt(index);
          --index;
        }
      }
      this.viewTarget.X = this._boat.position.X;
      this.viewTarget.Y = this._boat.position.Y;
      if (this._seagulls != null && this._seagulls.Count > 0 && (double) this._boat.position.Y > (double) this._seagulls[0].position.Y)
      {
        if ((double) Math.Abs(this._boat.position.X - this._seagulls[0].position.X) < 128.0 && Game1.random.NextDouble() < 0.25)
          Game1.playSound("seagulls");
        this._seagulls.RemoveAt(0);
      }
      if (this._interpolatedPoints.Count > 1)
      {
        if ((double) this.departureDelay > 0.0)
        {
          this.departureDelay -= (float) time.ElapsedGameTime.TotalSeconds;
        }
        else
        {
          if ((double) this.traveledBoatDistance < (double) this._totalPathDistance)
          {
            float to = this.boatSpeed;
            if (this._interpolatedPoints.Count <= 2)
              to = this.dockSpeed;
            this._currentBoatSpeed = Utility.MoveTowards(this._currentBoatSpeed, to, 0.01f);
            this.traveledBoatDistance += this._currentBoatSpeed;
            if ((double) this.traveledBoatDistance > (double) this._totalPathDistance)
              this.traveledBoatDistance = this._totalPathDistance;
          }
          this._nextSlosh -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this._nextSlosh <= 0.0)
          {
            this._nextSlosh = 0.75f;
            Game1.playSound("waterSlosh");
          }
        }
        while (this._interpolatedPoints.Count >= 2 && (double) this.traveledBoatDistance >= (double) this._cumulativeDistances[1])
        {
          this._interpolatedPoints.RemoveAt(0);
          this._cumulativeDistances.RemoveAt(0);
        }
        if (this._interpolatedPoints.Count <= 1)
        {
          this._interpolatedPoints.Clear();
          this._cumulativeDistances.Clear();
          Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this._fadeComplete = true));
        }
        else
        {
          Vector2 vector2 = this._interpolatedPoints[1] - this._interpolatedPoints[0];
          if ((double) Math.Abs(vector2.X) > (double) Math.Abs(vector2.Y))
          {
            if ((double) vector2.X < 0.0)
              this._boat.SetSourceRect(new Rectangle(704, 384, 32, 32));
            else
              this._boat.SetSourceRect(new Rectangle(704, 352, 32, 32));
          }
          else if ((double) vector2.Y > 0.0)
            this._boat.SetSourceRect(new Rectangle(640, 384, 32, 32));
          else
            this._boat.SetSourceRect(new Rectangle(640, 352, 32, 32));
          float t = (float) (((double) this.traveledBoatDistance - (double) this._cumulativeDistances[0]) / ((double) this._cumulativeDistances[1] - (double) this._cumulativeDistances[0]));
          this._boat.position = new Vector2(Utility.Lerp(this._interpolatedPoints[0].X, this._interpolatedPoints[1].X, t), Utility.Lerp(this._interpolatedPoints[0].Y, this._interpolatedPoints[1].Y, t));
        }
      }
      this._upperLeft.X = this.viewTarget.X * this._zoomLevel - (float) (Game1.viewport.Width / 2);
      this._upperLeft.Y = this.viewTarget.Y * this._zoomLevel - (float) (Game1.viewport.Height / 2);
      if ((double) this._upperLeft.Y < 0.0)
        this._upperLeft.Y = 0.0f;
      if ((double) this._upperLeft.Y + (double) Game1.viewport.Height > (double) this.mapSourceRectangle.Height * (double) this._zoomLevel)
        this._upperLeft.Y = (float) this.mapSourceRectangle.Height * this._zoomLevel - (float) Game1.viewport.Height;
      if ((double) this.nextSmoke <= 0.0)
      {
        this.nextSmoke = 0.75f;
        BoatJourney.Entity entity = new BoatJourney.Entity(this, "Minigames\\boatJourneyMap", new Rectangle(640, 480, 16, 16), new Vector2(8f, 8f), new Vector2(350f, 665f));
        entity.numFrames = 7;
        Vector2 vector2 = new Vector2(Utility.RandomFloat(-0.04f, -0.03f), Utility.RandomFloat(-0.1f, -0.2f));
        entity.velocity = vector2;
        entity.destroyAfterAnimation = true;
        this.entities.Add(entity);
      }
      else
        this.nextSmoke -= (float) time.ElapsedGameTime.TotalSeconds;
      return false;
    }

    public void afterFade()
    {
      Game1.currentMinigame = (IMinigame) null;
      Game1.globalFadeToClear();
      if (Game1.currentLocation.currentEvent == null)
        return;
      ++Game1.currentLocation.currentEvent.CurrentCommand;
      Game1.currentLocation.temporarySprites.Clear();
    }

    public bool forceQuit() => false;

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveKeyPress(Keys k)
    {
      if (k != Keys.Escape)
        return;
      this.forceQuit();
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Rectangle?(), new Color(49, 79, 155), 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
      b.Draw(this.texture, this.TransformDraw(this.mapSourceRectangle), new Rectangle?(this.mapSourceRectangle), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1E-05f);
      for (int index = 0; index < this.entities.Count; ++index)
      {
        if (!this.entities[index].drawOnTop)
          this.entities[index].Draw(b);
      }
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      for (int index = 0; index < this.entities.Count; ++index)
      {
        if (this.entities[index].drawOnTop)
          this.entities[index].Draw(b);
      }
      b.End();
    }

    public void changeScreenSize()
    {
      this._zoomLevel = 4f;
      if ((double) this.mapSourceRectangle.Height * (double) this._zoomLevel >= (double) Game1.viewport.Height)
        return;
      this._zoomLevel = (float) Game1.viewport.Height / (float) this.mapSourceRectangle.Height;
    }

    public void unload() => Game1.stopMusicTrack(Game1.MusicContext.MiniGame);

    public void receiveEventPoke(int data) => throw new NotImplementedException();

    public string minigameId() => (string) null;

    public bool doMainGameUpdates() => false;

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public class WaterSparkle : BoatJourney.Entity
    {
      protected Vector2 _startPosition;

      public WaterSparkle(BoatJourney context)
        : base(context, "Minigames\\boatJourneyMap", new Rectangle(647, 524, 1, 1), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f))
      {
        this.currentFrame = Game1.random.Next(0, 7);
        this.numFrames = 7;
        this.frameInterval = 0.1f;
        this._startPosition = this.position;
        this.RandomizePosition();
      }

      public override bool Update(GameTime time) => base.Update(time);

      public void RandomizePosition()
      {
        Rectangle r = new Rectangle(0, 112, 640, 528);
        do
        {
          this._startPosition = this.position = Utility.getRandomPositionInThisRectangle(r, Game1.random);
        }
        while (new Rectangle(508, 11, 125, 138).Contains((int) this._startPosition.X, (int) this._startPosition.Y));
        this.velocity.X = Utility.RandomFloat(-0.1f, 0.1f);
      }

      public override void OnAnimationFinished()
      {
        this.RandomizePosition();
        base.OnAnimationFinished();
      }

      public override float GetLayerDepth() => (double) this.layerDepth >= 0.0 ? this.layerDepth : 0.0001f;
    }

    public class Wave : BoatJourney.Entity
    {
      protected Vector2 _startPosition;

      public Wave(BoatJourney context, Vector2 position = default (Vector2))
        : base(context, "Minigames\\boatJourneyMap", new Rectangle(640, 506, 32, 12), new Vector2(16f, 6f), position)
      {
        this.numFrames = 2;
        this.frameInterval = 1.25f;
        this._startPosition = position;
      }

      public override bool Update(GameTime time)
      {
        this.position = this._startPosition + new Vector2(1f, 0.0f) * (float) Math.Sin((double) this._startPosition.X * 0.333000004291534 + (double) this._startPosition.Y * 0.100000001490116 + (double) this._age) * 3f;
        return base.Update(time);
      }

      public override float GetLayerDepth() => (double) this.layerDepth >= 0.0 ? this.layerDepth : 0.0003f;
    }

    public class Boat : BoatJourney.Entity
    {
      protected float nextSmokeStackSmoke;
      protected float nextRipple;
      public Vector2? smokeStack;
      public Vector2 _lastPosition;
      public float idleAnimationInterval = 0.75f;
      public float moveAnimationInterval = 0.25f;

      public Boat(
        BoatJourney context,
        string texture_path,
        Rectangle source_rect,
        Vector2 origin = default (Vector2),
        Vector2 position = default (Vector2))
        : base(context, texture_path, source_rect, origin, position)
      {
      }

      public override bool Update(GameTime time)
      {
        bool flag = false;
        if (this._lastPosition != this.position)
        {
          this._lastPosition = this.position;
          flag = true;
        }
        if (flag)
          this.frameInterval = this.moveAnimationInterval;
        else
          this.frameInterval = this.idleAnimationInterval;
        if (this.smokeStack.HasValue)
        {
          if ((double) this.nextSmokeStackSmoke <= 0.0)
          {
            this.nextSmokeStackSmoke = 0.25f;
            if (flag)
            {
              BoatJourney.Entity entity = new BoatJourney.Entity(this._context, "Minigames\\boatJourneyMap", new Rectangle(689, 337, 2, 2), new Vector2(1f, 1f), this.position + this.smokeStack.Value);
              entity.numFrames = 3;
              Vector2 vector2 = new Vector2(Utility.RandomFloat(-0.04f, -0.03f), Utility.RandomFloat(-0.05f, -0.1f));
              entity.velocity = vector2;
              entity.destroyAfterAnimation = true;
              this._context.entities.Add(entity);
            }
          }
          else
            this.nextSmokeStackSmoke -= (float) time.ElapsedGameTime.TotalSeconds;
        }
        if ((double) this.nextRipple <= 0.0)
        {
          this.nextRipple = 0.25f;
          if (flag)
            this._context.entities.Add(new BoatJourney.Entity(this._context, "Minigames\\boatJourneyMap", new Rectangle(640, 336, 9, 16), new Vector2(4f, 0.0f), this.position + new Vector2(0.0f, 0.0f))
            {
              numFrames = 5,
              layerDepth = 2E-05f,
              destroyAfterAnimation = true
            });
        }
        else
          this.nextRipple -= (float) time.ElapsedGameTime.TotalSeconds;
        return base.Update(time);
      }
    }

    public class Entity
    {
      protected BoatJourney _context;
      public Vector2 position;
      protected Texture2D _texture;
      protected Rectangle _sourceRect;
      protected float lifeTime;
      protected float _age;
      public Vector2 velocity;
      public Vector2 origin;
      public bool flipX;
      protected float _frameTime;
      public float frameInterval = 0.25f;
      public int currentFrame;
      public int numFrames = 1;
      public int columns;
      public bool destroyAfterAnimation;
      public bool drawOnTop;
      public float layerDepth = -1f;

      public Entity(
        BoatJourney context,
        string texture_path,
        Rectangle source_rect,
        Vector2 origin = default (Vector2),
        Vector2 position = default (Vector2))
      {
        this._context = context;
        this._texture = Game1.temporaryContent.Load<Texture2D>(texture_path);
        this._sourceRect = source_rect;
        this.origin = origin;
        this.position = position;
      }

      public virtual bool Update(GameTime time)
      {
        this._age += (float) time.ElapsedGameTime.TotalSeconds;
        this._frameTime += (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.lifeTime > 0.0 && (double) this.lifeTime >= (double) this._age)
          return true;
        if ((double) this.frameInterval > 0.0 && (double) this._frameTime > (double) this.frameInterval)
        {
          this._frameTime -= this.frameInterval;
          ++this.currentFrame;
          if (this.currentFrame >= this.numFrames)
          {
            this.OnAnimationFinished();
            this.currentFrame -= this.numFrames;
            if (this.destroyAfterAnimation)
              return true;
          }
        }
        this.position += this.velocity;
        return false;
      }

      public virtual void OnAnimationFinished()
      {
      }

      public virtual void SetSourceRect(Rectangle rectangle) => this._sourceRect = rectangle;

      public virtual Rectangle GetSourceRect()
      {
        int currentFrame = this.currentFrame;
        int num = 0;
        if (this.columns > 0)
        {
          num = currentFrame / this.columns;
          currentFrame %= this.columns;
        }
        return new Rectangle(this._sourceRect.X + currentFrame * this._sourceRect.Width, this._sourceRect.Y + num * this._sourceRect.Width, this._sourceRect.Width, this._sourceRect.Height);
      }

      public virtual float GetLayerDepth() => (double) this.layerDepth >= 0.0 ? this.layerDepth : this.position.Y / 100000f;

      public virtual void Draw(SpriteBatch b) => b.Draw(this._texture, this._context.TransformDraw(this.position), new Rectangle?(this.GetSourceRect()), Color.White, 0.0f, this.origin, this._context._zoomLevel, this.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.GetLayerDepth());
    }
  }
}
