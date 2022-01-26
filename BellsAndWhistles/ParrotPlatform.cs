// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.ParrotPlatform
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace StardewValley.BellsAndWhistles
{
  public class ParrotPlatform
  {
    [XmlIgnore]
    [InstancedStatic]
    public static ParrotPlatform activePlatform;
    [XmlIgnore]
    public Vector2 position;
    [XmlIgnore]
    public Texture2D texture;
    [XmlIgnore]
    public List<ParrotPlatform.Parrot> parrots = new List<ParrotPlatform.Parrot>();
    [XmlIgnore]
    public float height;
    [XmlIgnore]
    protected Event _takeoffEvent;
    [XmlIgnore]
    public ParrotPlatform.TakeoffState takeoffState;
    [XmlIgnore]
    public float stateTimer;
    [XmlIgnore]
    public float liftSpeed;
    [XmlIgnore]
    protected bool _onActivationTile;
    public Vector2 shake = Vector2.Zero;
    public string currentLocationKey = "";
    public KeyValuePair<string, KeyValuePair<string, Point>> currentDestination;

    public static List<KeyValuePair<string, KeyValuePair<string, Point>>> GetDestinations(
      bool only_show_accessible = true)
    {
      List<KeyValuePair<string, KeyValuePair<string, Point>>> destinations = new List<KeyValuePair<string, KeyValuePair<string, Point>>>();
      destinations.Add(new KeyValuePair<string, KeyValuePair<string, Point>>("Volcano", new KeyValuePair<string, Point>("IslandNorth", new Point(60, 17))));
      if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_UpgradeBridge") || !only_show_accessible)
        destinations.Add(new KeyValuePair<string, KeyValuePair<string, Point>>("Archaeology", new KeyValuePair<string, Point>("IslandNorth", new Point(5, 49))));
      destinations.Add(new KeyValuePair<string, KeyValuePair<string, Point>>("Farm", new KeyValuePair<string, Point>("IslandWest", new Point(74, 10))));
      destinations.Add(new KeyValuePair<string, KeyValuePair<string, Point>>("Forest", new KeyValuePair<string, Point>("IslandEast", new Point(28, 29))));
      destinations.Add(new KeyValuePair<string, KeyValuePair<string, Point>>("Docks", new KeyValuePair<string, Point>("IslandSouth", new Point(6, 32))));
      return destinations;
    }

    public static List<ParrotPlatform> CreateParrotPlatformsForArea(
      GameLocation location)
    {
      List<ParrotPlatform> platformsForArea = new List<ParrotPlatform>();
      foreach (KeyValuePair<string, KeyValuePair<string, Point>> destination in ParrotPlatform.GetDestinations(false))
      {
        if (location.Name == destination.Value.Key)
          platformsForArea.Add(new ParrotPlatform(destination.Value.Value.X - 1, destination.Value.Value.Y - 2, destination.Key));
      }
      return platformsForArea;
    }

    public ParrotPlatform() => this.texture = Game1.content.Load<Texture2D>("LooseSprites\\ParrotPlatform");

    public ParrotPlatform(int tile_x, int tile_y, string key)
      : this()
    {
      this.currentLocationKey = key;
      this.position = new Vector2((float) (tile_x * 64), (float) (tile_y * 64));
      this.parrots.Add(new ParrotPlatform.Parrot(this, 15, 20, false, false));
      this.parrots.Add(new ParrotPlatform.Parrot(this, 33, 20, true, false));
    }

    public virtual void StartDeparture()
    {
      this.takeoffState = ParrotPlatform.TakeoffState.Boarding;
      Game1.playSound("parrot");
      foreach (ParrotPlatform.Parrot parrot in this.parrots)
        parrot.squawkTime = 0.25f;
      this.stateTimer = 0.5f;
      Game1.player.shouldShadowBeOffset = true;
      xTile.Dimensions.Rectangle viewport = Game1.viewport;
      StringBuilder stringBuilder = new StringBuilder();
      Vector2 position = Game1.player.Position;
      stringBuilder.Append("/0 0/farmer " + Game1.player.getTileX().ToString() + " " + Game1.player.getTileY().ToString() + " " + Game1.player.facingDirection?.ToString());
      stringBuilder.Append("/playerControl parrotRide");
      this._takeoffEvent = new Event(stringBuilder.ToString())
      {
        showWorldCharacters = true,
        showGroundObjects = true
      };
      Game1.currentLocation.currentEvent = this._takeoffEvent;
      this._takeoffEvent.checkForNextCommand(Game1.player.currentLocation, Game1.currentGameTime);
      Game1.player.Position = position;
      Game1.eventUp = true;
      Game1.viewport = viewport;
      foreach (ParrotPlatform.Parrot parrot in this.parrots)
      {
        parrot.height = 21f;
        parrot.position = parrot.anchorPosition;
      }
    }

    public virtual void Update(GameTime time)
    {
      if (this.takeoffState == ParrotPlatform.TakeoffState.Idle && !Game1.player.IsBusyDoingSomething())
      {
        bool flag = new Microsoft.Xna.Framework.Rectangle((int) this.position.X / 64, (int) this.position.Y / 64, 3, 1).Contains(Game1.player.getTileLocationPoint());
        if (this._onActivationTile != flag)
        {
          this._onActivationTile = flag;
          if (this._onActivationTile && Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value)
            this.Activate();
        }
      }
      this.shake = Vector2.Zero;
      if (this.takeoffState == ParrotPlatform.TakeoffState.Liftoff)
      {
        this.shake.X = Utility.RandomFloat(-0.5f, 0.5f) * 4f;
        this.shake.Y = Utility.RandomFloat(-0.5f, 0.5f) * 4f;
      }
      if ((double) this.stateTimer > 0.0)
        this.stateTimer -= (float) time.ElapsedGameTime.TotalSeconds;
      if (this.takeoffState == ParrotPlatform.TakeoffState.Boarding && (double) this.stateTimer <= 0.0)
      {
        this.takeoffState = ParrotPlatform.TakeoffState.BeginFlying;
        Game1.playSound("dwoop");
      }
      if (this.takeoffState == ParrotPlatform.TakeoffState.BeginFlying && (double) this.parrots[0].height >= 64.0 && (double) this.stateTimer <= 0.0)
      {
        this.takeoffState = ParrotPlatform.TakeoffState.Liftoff;
        this.stateTimer = 0.5f;
        Game1.playSound("treethud");
      }
      if (this.takeoffState == ParrotPlatform.TakeoffState.Liftoff && (double) this.stateTimer <= 0.0)
        this.takeoffState = ParrotPlatform.TakeoffState.Flying;
      if (this.takeoffState >= ParrotPlatform.TakeoffState.Flying && (double) this.parrots[0].height >= 64.0)
      {
        this.height += this.liftSpeed;
        this.liftSpeed += 0.025f;
        Game1.player.drawOffset.Value = new Vector2(0.0f, (float) (-(double) this.height * 4.0));
        if ((double) this.height >= 128.0 && this.takeoffState != ParrotPlatform.TakeoffState.Finished)
        {
          this.takeoffState = ParrotPlatform.TakeoffState.Finished;
          this._takeoffEvent.command_end(Game1.currentLocation, Game1.currentGameTime, new string[0]);
          this._takeoffEvent = (Event) null;
          LocationRequest locationRequest = Game1.getLocationRequest(this.currentDestination.Value.Key);
          locationRequest.OnWarp += (LocationRequest.Callback) (() =>
          {
            this.takeoffState = ParrotPlatform.TakeoffState.Idle;
            Game1.player.shouldShadowBeOffset = false;
            Game1.player.drawOffset.Value = Vector2.Zero;
          });
          KeyValuePair<string, Point> keyValuePair = this.currentDestination.Value;
          int x = keyValuePair.Value.X;
          keyValuePair = this.currentDestination.Value;
          int y = keyValuePair.Value.Y;
          Game1.warpFarmer(locationRequest, x, y, 2);
        }
      }
      foreach (ParrotPlatform.Parrot parrot in this.parrots)
        parrot.Update(time);
    }

    public virtual void Activate()
    {
      List<Response> responseList = new List<Response>();
      foreach (KeyValuePair<string, KeyValuePair<string, Point>> destination in ParrotPlatform.GetDestinations())
      {
        if (destination.Key != this.currentLocationKey)
          responseList.Add(new Response("Go" + destination.Key, Game1.content.LoadString("Strings\\UI:ParrotPlatform_" + destination.Key)));
      }
      responseList.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:ParrotPlatform_Question"), responseList.ToArray(), nameof (ParrotPlatform));
      ParrotPlatform.activePlatform = this;
    }

    public virtual bool AnswerQuestion(Response answer)
    {
      if (this == ParrotPlatform.activePlatform)
      {
        if (Game1.currentLocation.lastQuestionKey != null && Game1.currentLocation.afterQuestion == null && (Game1.currentLocation.lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey).StartsWith("ParrotPlatform_Go"))
        {
          string str = answer.responseKey.Substring(2);
          foreach (KeyValuePair<string, KeyValuePair<string, Point>> destination in ParrotPlatform.GetDestinations())
          {
            if (destination.Key == str)
            {
              this.currentDestination = destination;
              break;
            }
          }
          this.StartDeparture();
          return true;
        }
        ParrotPlatform.activePlatform = (ParrotPlatform) null;
      }
      return false;
    }

    public virtual void Cleanup() => ParrotPlatform.activePlatform = (ParrotPlatform) null;

    public virtual bool CheckCollisions(Microsoft.Xna.Framework.Rectangle rectangle)
    {
      int num = 16;
      return rectangle.Intersects(new Microsoft.Xna.Framework.Rectangle((int) this.position.X, (int) this.position.Y, 192, num)) || rectangle.Intersects(new Microsoft.Xna.Framework.Rectangle((int) this.position.X, (int) this.position.Y + 128 - num, 64, num)) || rectangle.Intersects(new Microsoft.Xna.Framework.Rectangle((int) this.position.X + 128, (int) this.position.Y + 128 - num, 64, num)) || this.takeoffState > ParrotPlatform.TakeoffState.Idle && rectangle.Intersects(new Microsoft.Xna.Framework.Rectangle((int) this.position.X + 64, (int) this.position.Y + 128 - num, 64, num)) || rectangle.Intersects(new Microsoft.Xna.Framework.Rectangle((int) this.position.X, (int) this.position.Y, num, 128)) || rectangle.Intersects(new Microsoft.Xna.Framework.Rectangle((int) this.position.X + 192 - num, (int) this.position.Y, num, 128));
    }

    public virtual bool OccupiesTile(Vector2 tile_pos) => (double) tile_pos.X >= (double) this.position.X / 64.0 && (double) tile_pos.X < (double) this.position.X / 64.0 + 3.0 && (double) tile_pos.Y >= (double) this.position.Y / 64.0 && (double) tile_pos.Y < (double) this.position.Y / 64.0 + 2.0;

    public virtual Vector2 GetDrawPosition() => this.position - new Vector2(0.0f, (float) (128.0 + (double) this.height * 4.0)) + this.shake;

    public virtual void Draw(SpriteBatch b)
    {
      b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.position - new Vector2(0.0f, 128f) + new Vector2(-2f, 38f) * 4f + new Vector2(48f, 32f) * 4f / 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(48, 73, 48, 32)), Color.White, 0.0f, new Vector2(48f, 32f) / 2f, (float) (4.0 * (1.0 - (double) Math.Min(1f, this.height / 480f))), SpriteEffects.None, 0.0f);
      b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.GetDrawPosition()), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 68)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, this.position.Y / 10000f);
      b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.GetDrawPosition()), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(48, 0, 48, 68)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.position.Y + 128.0) / 10000.0));
      if (!Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value)
        return;
      foreach (ParrotPlatform.Parrot parrot in this.parrots)
        parrot.Draw(b);
    }

    public enum TakeoffState
    {
      Idle,
      Boarding,
      BeginFlying,
      Liftoff,
      Flying,
      Finished,
    }

    public class Parrot
    {
      public Vector2 position;
      public Vector2 anchorPosition;
      public Texture2D texture;
      protected ParrotPlatform _platform;
      protected bool facingRight;
      protected bool facingUp;
      public const int START_HEIGHT = 21;
      public const int END_HEIGHT = 64;
      public float height = 21f;
      public bool flapping;
      public float nextFlap;
      public float slack;
      public Vector2[] points = new Vector2[4];
      public float swayOffset;
      public float liftSpeed;
      public float squawkTime;

      public Parrot(ParrotPlatform platform, int x, int y, bool facing_right, bool facing_up)
      {
        this._platform = platform;
        this.texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\parrots");
        this.position = new Vector2((float) x, (float) y);
        this.anchorPosition = this.position;
        this.facingRight = facing_right;
        this.facingUp = facing_up;
        this.swayOffset = Utility.RandomFloat(0.0f, 100f);
      }

      public virtual void UpdateLine(Vector2 start, Vector2 end)
      {
        float num = Utility.Lerp(15f, 0.0f, (this.height - 21f) / 43f);
        for (int index = 0; index < this.points.Length; ++index)
        {
          Vector2 vector2 = new Vector2(Utility.Lerp(start.X, end.X, (float) index / (float) (this.points.Length - 1)), Utility.Lerp(start.Y, end.Y, (float) index / (float) (this.points.Length - 1)));
          vector2.Y -= (float) (Math.Pow(2.0 * ((double) index / (double) (this.points.Length - 1)) - 1.0, 2.0) - 1.0) * num;
          this.points[index] = vector2;
        }
      }

      public virtual void Update(GameTime time)
      {
        TimeSpan elapsedGameTime;
        if ((double) this.squawkTime > 0.0)
        {
          double squawkTime = (double) this.squawkTime;
          elapsedGameTime = time.ElapsedGameTime;
          double totalSeconds = elapsedGameTime.TotalSeconds;
          this.squawkTime = (float) (squawkTime - totalSeconds);
        }
        if (this._platform.takeoffState < ParrotPlatform.TakeoffState.BeginFlying)
          return;
        double nextFlap = (double) this.nextFlap;
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds1 = elapsedGameTime.TotalSeconds;
        this.nextFlap = (float) (nextFlap - totalSeconds1);
        if ((double) this.nextFlap <= 0.0)
        {
          this.flapping = !this.flapping;
          if (this.flapping)
          {
            Game1.playSound("batFlap");
            this.nextFlap = Utility.RandomFloat(0.025f, 0.1f);
          }
          else
            this.nextFlap = Utility.RandomFloat(0.075f, 0.15f);
        }
        if ((double) this.height >= 64.0)
          return;
        this.height += this.liftSpeed;
        this.liftSpeed += 0.025f;
        if (this.facingRight)
          this.position.X += 0.15f;
        else
          this.position.X -= 0.15f;
        if (this.facingUp)
          this.position.Y -= 0.15f;
        else
          this.position.Y += 0.15f;
      }

      public virtual void Draw(SpriteBatch b)
      {
        Vector2 vector2_1 = this._platform.GetDrawPosition() + this.position * 4f;
        float num1 = Utility.Lerp(0.0f, 2f, (this.height - 21f) / 43f);
        Vector2 vector2_2;
        ref Vector2 local = ref vector2_2;
        TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
        double x = Math.Sin(totalGameTime.TotalSeconds * 4.0 + (double) this.swayOffset) * (double) num1;
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        double y = Math.Cos(totalGameTime.TotalSeconds * 16.0 + (double) this.swayOffset) * (double) num1;
        local = new Vector2((float) x, (float) y);
        if (this._platform.takeoffState <= ParrotPlatform.TakeoffState.Boarding)
        {
          int num2 = 0;
          if ((double) this.squawkTime > 0.0)
          {
            vector2_2.X += Utility.RandomFloat(-0.15f, 0.15f) * 4f;
            vector2_2.Y += Utility.RandomFloat(-0.15f, 0.15f) * 4f;
            num2 = 1;
          }
          b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, vector2_1 - new Vector2(0.0f, this.height * 4f) + vector2_2 * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num2 * 24, 0, 24, 24)), Color.White, 0.0f, new Vector2(12f, 19f), 4f, this.facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) vector2_1.Y + 0.100000001490116 + 192.0) / 10000.0));
        }
        else
        {
          int num3 = this.flapping ? 1 : 0;
          if (this.flapping && (double) this.nextFlap <= 0.0500000007450581)
            num3 = 2;
          int num4 = 5;
          if (this.facingUp)
            num4 = 8;
          b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, vector2_1 - new Vector2(0.0f, this.height * 4f) + vector2_2 * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((num4 + num3) * 24, 0, 24, 24)), Color.White, 0.0f, new Vector2(12f, 19f), 4f, this.facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) vector2_1.Y + 0.100000001490116 + 128.0) / 10000.0));
          Vector2 vector2_3 = this._platform.position + this.anchorPosition * 4f;
          Vector2 drawPosition = this._platform.GetDrawPosition();
          this.UpdateLine(Utility.snapDrawPosition(Game1.GlobalToLocal(drawPosition + (this.anchorPosition - new Vector2(0.0f, 21f)) * 4f)) + new Vector2(2f, 0.0f), Utility.snapDrawPosition(Game1.GlobalToLocal(drawPosition + (this.position - new Vector2(0.0f, this.height) + vector2_2) * 4f)));
          if (this.points == null)
            return;
          Vector2? nullable = new Vector2?();
          float num5 = 1E-06f;
          float num6 = 0.0f;
          float num7 = (float) (((double) vector2_3.Y + 0.0500000007450581) / 10000.0);
          foreach (Vector2 point in this.points)
          {
            b.Draw(this._platform.texture, point, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 68, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num7 + num6);
            num6 += num5;
            if (nullable.HasValue)
            {
              Vector2 vector2_4 = point - nullable.Value;
              int num8 = (int) Math.Ceiling((double) vector2_4.Length() / 4.0);
              float rotation = (float) (-Math.Atan2((double) vector2_4.X, (double) vector2_4.Y) + 1.57079637050629);
              b.Draw(this._platform.texture, nullable.Value, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 68, 16, 16)), Color.White, rotation, new Vector2(0.0f, 8f), new Vector2((float) (4 * num8) / 16f, 4f), SpriteEffects.None, num7 + num6);
              num6 += num5;
            }
            nullable = new Vector2?(point);
          }
        }
      }
    }
  }
}
