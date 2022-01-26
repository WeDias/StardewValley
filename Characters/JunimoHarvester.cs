// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.JunimoHarvester
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StardewValley.Characters
{
  public class JunimoHarvester : NPC
  {
    private float alpha = 1f;
    private float alphaChange;
    private Vector2 motion = Vector2.Zero;
    private Rectangle nextPosition;
    private readonly NetColor color = new NetColor();
    private bool destroy;
    private Item lastItemHarvested;
    private Task backgroundTask;
    public int whichJunimoFromThisHut;
    public readonly NetBool isPrismatic = new NetBool(false);
    private readonly NetGuid netHome = new NetGuid();
    private readonly NetEvent1Field<int, NetInt> netAnimationEvent = new NetEvent1Field<int, NetInt>();
    private int harvestTimer;

    private JunimoHut home
    {
      get => Game1.getFarm().buildings[this.netHome.Value] as JunimoHut;
      set => this.netHome.Value = Game1.getFarm().buildings.GuidOf((Building) value);
    }

    public JunimoHarvester()
    {
    }

    public JunimoHarvester(
      Vector2 position,
      JunimoHut myHome,
      int whichJunimoNumberFromThisHut,
      Color? c)
      : base(new AnimatedSprite("Characters\\Junimo", 0, 16, 16), position, 2, "Junimo")
    {
      this.home = myHome;
      this.whichJunimoFromThisHut = whichJunimoNumberFromThisHut;
      if (!c.HasValue)
        this.pickColor();
      else
        this.color.Value = c.Value;
      this.nextPosition = this.GetBoundingBox();
      this.Breather = false;
      this.speed = 3;
      this.forceUpdateTimer = 9999;
      this.collidesWithOtherCharacters.Value = true;
      this.ignoreMovementAnimation = true;
      this.farmerPassesThrough = true;
      this.Scale = 0.75f;
      this.willDestroyObjectsUnderfoot = false;
      this.currentLocation = (GameLocation) Game1.getFarm();
      Vector2 v = Vector2.Zero;
      switch (whichJunimoNumberFromThisHut)
      {
        case 0:
          v = Utility.recursiveFindOpenTileForCharacter((Character) this, this.currentLocation, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.home.tileX + 1), (float) ((int) (NetFieldBase<int, NetInt>) this.home.tileY + (int) (NetFieldBase<int, NetInt>) this.home.tilesHigh + 1)), 30);
          break;
        case 1:
          v = Utility.recursiveFindOpenTileForCharacter((Character) this, this.currentLocation, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.home.tileX - 1), (float) (int) (NetFieldBase<int, NetInt>) this.home.tileY), 30);
          break;
        case 2:
          v = Utility.recursiveFindOpenTileForCharacter((Character) this, this.currentLocation, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.home.tileX + (int) (NetFieldBase<int, NetInt>) this.home.tilesWide), (float) (int) (NetFieldBase<int, NetInt>) this.home.tileY), 30);
          break;
      }
      if (v != Vector2.Zero)
        this.controller = new PathFindController((Character) this, this.currentLocation, Utility.Vector2ToPoint(v), -1, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100);
      if ((this.controller == null || this.controller.pathToEndPoint == null) && Game1.IsMasterGame)
      {
        this.pathfindToRandomSpotAroundHut();
        if (this.controller == null || this.controller.pathToEndPoint == null)
          this.destroy = true;
      }
      this.collidesWithOtherCharacters.Value = false;
    }

    private void pickColor()
    {
      Random random = new Random((int) (NetFieldBase<int, NetInt>) this.home.tileX + (int) (NetFieldBase<int, NetInt>) this.home.tileY * 777 + this.whichJunimoFromThisHut);
      if (random.NextDouble() < 0.25)
      {
        switch (random.Next(8))
        {
          case 0:
            this.color.Value = Color.Red;
            break;
          case 1:
            this.color.Value = Color.Goldenrod;
            break;
          case 2:
            this.color.Value = Color.Yellow;
            break;
          case 3:
            this.color.Value = Color.Lime;
            break;
          case 4:
            this.color.Value = new Color(0, (int) byte.MaxValue, 180);
            break;
          case 5:
            this.color.Value = new Color(0, 100, (int) byte.MaxValue);
            break;
          case 6:
            this.color.Value = Color.MediumPurple;
            break;
          case 7:
            this.color.Value = Color.Salmon;
            break;
        }
        if (random.NextDouble() >= 0.01)
          return;
        this.color.Value = Color.White;
      }
      else
      {
        switch (random.Next(8))
        {
          case 0:
            this.color.Value = Color.LimeGreen;
            break;
          case 1:
            this.color.Value = Color.Orange;
            break;
          case 2:
            this.color.Value = Color.LightGreen;
            break;
          case 3:
            this.color.Value = Color.Tan;
            break;
          case 4:
            this.color.Value = Color.GreenYellow;
            break;
          case 5:
            this.color.Value = Color.LawnGreen;
            break;
          case 6:
            this.color.Value = Color.PaleGreen;
            break;
          case 7:
            this.color.Value = Color.Turquoise;
            break;
        }
      }
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.color, this.netHome.NetFields, (INetSerializable) this.netAnimationEvent, (INetSerializable) this.isPrismatic);
      this.netAnimationEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doAnimationEvent);
    }

    protected virtual void doAnimationEvent(int animId)
    {
      switch (animId)
      {
        case 0:
          this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
          break;
        case 2:
          this.Sprite.currentFrame = 0;
          break;
        case 3:
          this.Sprite.currentFrame = 1;
          break;
        case 4:
          this.Sprite.currentFrame = 2;
          break;
        case 5:
          this.Sprite.currentFrame = 44;
          break;
        case 6:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(12, 200),
            new FarmerSprite.AnimationFrame(13, 200),
            new FarmerSprite.AnimationFrame(14, 200),
            new FarmerSprite.AnimationFrame(15, 200)
          });
          break;
        case 7:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(44, 200),
            new FarmerSprite.AnimationFrame(45, 200),
            new FarmerSprite.AnimationFrame(46, 200),
            new FarmerSprite.AnimationFrame(47, 200)
          });
          break;
        case 8:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(28, 100),
            new FarmerSprite.AnimationFrame(29, 100),
            new FarmerSprite.AnimationFrame(30, 100),
            new FarmerSprite.AnimationFrame(31, 100)
          });
          break;
      }
    }

    public void reachFirstDestinationFromHut(Character c, GameLocation l) => this.tryToHarvestHere();

    public void tryToHarvestHere()
    {
      if (this.currentLocation == null)
        return;
      if (this.isHarvestable())
        this.harvestTimer = 2000;
      else
        this.pokeToHarvest();
    }

    public void pokeToHarvest()
    {
      if (!this.home.isTilePassable(this.getTileLocation()) && Game1.IsMasterGame)
      {
        this.destroy = true;
      }
      else
      {
        if (this.harvestTimer > 0 || Game1.random.NextDouble() >= 0.7)
          return;
        this.pathfindToNewCrop();
      }
    }

    public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

    public void setMoving(int xSpeed, int ySpeed)
    {
      this.motion.X = (float) xSpeed;
      this.motion.Y = (float) ySpeed;
    }

    public void setMoving(Vector2 motion) => this.motion = motion;

    public override void Halt()
    {
      base.Halt();
      this.motion = Vector2.Zero;
    }

    public override bool canTalk() => false;

    public void junimoReachedHut(Character c, GameLocation l)
    {
      this.controller = (PathFindController) null;
      this.motion.X = 0.0f;
      this.motion.Y = -1f;
      this.destroy = true;
    }

    public bool foundCropEndFunction(
      PathNode currentNode,
      Point endPoint,
      GameLocation location,
      Character c)
    {
      return location.isCropAtTile(currentNode.x, currentNode.y) && (location.terrainFeatures[new Vector2((float) currentNode.x, (float) currentNode.y)] as HoeDirt).readyForHarvest() || location.terrainFeatures.ContainsKey(new Vector2((float) currentNode.x, (float) currentNode.y)) && location.terrainFeatures[new Vector2((float) currentNode.x, (float) currentNode.y)] is Bush && (int) (NetFieldBase<int, NetInt>) (location.terrainFeatures[new Vector2((float) currentNode.x, (float) currentNode.y)] as Bush).tileSheetOffset == 1;
    }

    public void pathFindToNewCrop_doWork()
    {
      if (Game1.timeOfDay > 1900)
      {
        if (this.controller != null)
          return;
        this.returnToJunimoHut(this.currentLocation);
      }
      else if (Game1.random.NextDouble() < 0.035 || (bool) (NetFieldBase<bool, NetBool>) this.home.noHarvest)
      {
        this.pathfindToRandomSpotAroundHut();
      }
      else
      {
        this.controller = new PathFindController((Character) this, this.currentLocation, new PathFindController.isAtEnd(this.foundCropEndFunction), -1, false, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100, Point.Zero);
        if (this.controller.pathToEndPoint == null || Math.Abs(this.controller.pathToEndPoint.Last<Point>().X - ((int) (NetFieldBase<int, NetInt>) this.home.tileX + 1)) > 8 || Math.Abs(this.controller.pathToEndPoint.Last<Point>().Y - ((int) (NetFieldBase<int, NetInt>) this.home.tileY + 1)) > 8)
        {
          if (Game1.random.NextDouble() < 0.5 && !this.home.lastKnownCropLocation.Equals(Point.Zero))
            this.controller = new PathFindController((Character) this, this.currentLocation, this.home.lastKnownCropLocation, -1, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100);
          else if (Game1.random.NextDouble() < 0.25)
          {
            this.netAnimationEvent.Fire(0);
            this.returnToJunimoHut(this.currentLocation);
          }
          else
            this.pathfindToRandomSpotAroundHut();
        }
        else
          this.netAnimationEvent.Fire(0);
      }
    }

    public void pathfindToNewCrop()
    {
      if (this.backgroundTask != null && !this.backgroundTask.IsCompleted)
        return;
      this.pathFindToNewCrop_doWork();
    }

    public void returnToJunimoHut(GameLocation location)
    {
      if (Utility.isOnScreen(Utility.Vector2ToPoint(this.position.Value / 64f), 64, this.currentLocation))
        this.jump();
      this.collidesWithOtherCharacters.Value = false;
      if (Game1.IsMasterGame)
      {
        this.controller = new PathFindController((Character) this, location, new Point((int) (NetFieldBase<int, NetInt>) this.home.tileX + 1, (int) (NetFieldBase<int, NetInt>) this.home.tileY + 1), 0, new PathFindController.endBehavior(this.junimoReachedHut));
        if (this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0 || location.isCollidingPosition(this.nextPosition, Game1.viewport, false, 0, false, (Character) this))
          this.destroy = true;
      }
      if (!Utility.isOnScreen(Utility.Vector2ToPoint(this.position.Value / 64f), 64, this.currentLocation))
        return;
      location.playSound("junimoMeep1");
    }

    public override void faceDirection(int direction)
    {
    }

    protected override void updateSlaveAnimation(GameTime time)
    {
    }

    private bool isHarvestable() => this.currentLocation.terrainFeatures.ContainsKey(this.getTileLocation()) && this.currentLocation.terrainFeatures[this.getTileLocation()] is HoeDirt && (this.currentLocation.terrainFeatures[this.getTileLocation()] as HoeDirt).readyForHarvest() || this.currentLocation.terrainFeatures.ContainsKey(this.getTileLocation()) && this.currentLocation.terrainFeatures[this.getTileLocation()] is Bush && (int) (NetFieldBase<int, NetInt>) (this.currentLocation.terrainFeatures[this.getTileLocation()] as Bush).tileSheetOffset == 1;

    public override void update(GameTime time, GameLocation location)
    {
      if (this.backgroundTask != null && !this.backgroundTask.IsCompleted && Game1.IsMasterGame)
      {
        this.Sprite.Animate(time, 8, 4, 100f);
      }
      else
      {
        this.netAnimationEvent.Poll();
        base.update(time, location);
        if (this.isPrismatic.Value)
          this.color.Value = Utility.GetPrismaticColor(this.whichJunimoFromThisHut);
        this.forceUpdateTimer = 99999;
        if (this.eventActor)
          return;
        if (this.destroy)
          this.alphaChange = -0.05f;
        this.alpha += this.alphaChange;
        if ((double) this.alpha > 1.0)
          this.alpha = 1f;
        else if ((double) this.alpha < 0.0)
        {
          this.alpha = 0.0f;
          if (this.destroy && Game1.IsMasterGame)
          {
            location.characters.Remove((NPC) this);
            this.home.myJunimos.Remove(this);
          }
        }
        if (Game1.IsMasterGame)
        {
          if (this.harvestTimer > 0)
          {
            int harvestTimer = this.harvestTimer;
            this.harvestTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.harvestTimer > 1800)
              this.netAnimationEvent.Fire(2);
            else if (this.harvestTimer > 1600)
              this.netAnimationEvent.Fire(3);
            else if (this.harvestTimer > 1000)
            {
              this.netAnimationEvent.Fire(4);
              this.shake(50);
            }
            else if (harvestTimer >= 1000 && this.harvestTimer < 1000)
            {
              this.netAnimationEvent.Fire(2);
              if (this.currentLocation != null && !(bool) (NetFieldBase<bool, NetBool>) this.home.noHarvest && this.isHarvestable())
              {
                this.netAnimationEvent.Fire(5);
                this.lastItemHarvested = (Item) null;
                if (this.currentLocation.terrainFeatures[this.getTileLocation()] is Bush && (int) (NetFieldBase<int, NetInt>) (this.currentLocation.terrainFeatures[this.getTileLocation()] as Bush).tileSheetOffset == 1)
                {
                  this.tryToAddItemToHut((Item) new StardewValley.Object(815, 1));
                  (this.currentLocation.terrainFeatures[this.getTileLocation()] as Bush).tileSheetOffset.Value = 0;
                  (this.currentLocation.terrainFeatures[this.getTileLocation()] as Bush).setUpSourceRect();
                  if (Utility.isOnScreen(this.getTileLocationPoint(), 64, this.currentLocation))
                    (this.currentLocation.terrainFeatures[this.getTileLocation()] as Bush).performUseAction(this.getTileLocation(), this.currentLocation);
                  if (Utility.isOnScreen(this.getTileLocationPoint(), 64, this.currentLocation))
                    DelayedAction.playSoundAfterDelay("coin", 260, this.currentLocation);
                }
                else if ((this.currentLocation.terrainFeatures[this.getTileLocation()] as HoeDirt).crop.harvest(this.getTileX(), this.getTileY(), this.currentLocation.terrainFeatures[this.getTileLocation()] as HoeDirt, this))
                  (this.currentLocation.terrainFeatures[this.getTileLocation()] as HoeDirt).destroyCrop(this.getTileLocation(), this.currentLocation.farmers.Any(), this.currentLocation);
                if (this.lastItemHarvested != null && this.currentLocation.farmers.Any())
                {
                  Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.lastItemHarvested.parentSheetIndex, 16, 16), 1000f, 1, 0, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position + new Vector2(0.0f, -40f), false, false, (float) ((double) this.getStandingY() / 10000.0 + 0.00999999977648258), 0.02f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.08f, -0.25f)
                  });
                  if (this.lastItemHarvested is ColoredObject)
                    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.lastItemHarvested.parentSheetIndex + 1, 16, 16), 1000f, 1, 0, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position + new Vector2(0.0f, -40f), false, false, (float) ((double) this.getStandingY() / 10000.0 + 0.0149999996647239), 0.02f, (Color) (NetFieldBase<Color, NetColor>) (this.lastItemHarvested as ColoredObject).color, 4f, -0.01f, 0.0f, 0.0f)
                    {
                      motion = new Vector2(0.08f, -0.25f)
                    });
                }
              }
            }
            else if (this.harvestTimer <= 0)
              this.pokeToHarvest();
          }
          else if ((double) this.alpha > 0.0 && this.controller == null)
          {
            if ((this.addedSpeed > 0 || this.speed > 3 || this.isCharging) && Game1.IsMasterGame)
              this.destroy = true;
            this.nextPosition = this.GetBoundingBox();
            this.nextPosition.X += (int) this.motion.X;
            bool flag = false;
            if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, (Character) this))
            {
              this.position.X += (float) (int) this.motion.X;
              flag = true;
            }
            this.nextPosition.X -= (int) this.motion.X;
            this.nextPosition.Y += (int) this.motion.Y;
            if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, (Character) this))
            {
              this.position.Y += (float) (int) this.motion.Y;
              flag = true;
            }
            if (!this.motion.Equals(Vector2.Zero) & flag && Game1.random.NextDouble() < 0.005)
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(Game1.random.NextDouble() < 0.5 ? 10 : 11, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position, (Color) (NetFieldBase<Color, NetColor>) this.color)
              {
                motion = this.motion / 4f,
                alphaFade = 0.01f,
                layerDepth = 0.8f,
                scale = 0.75f,
                alpha = 0.75f
              });
            if (Game1.random.NextDouble() < 0.002)
            {
              switch (Game1.random.Next(6))
              {
                case 0:
                  this.netAnimationEvent.Fire(6);
                  break;
                case 1:
                  this.netAnimationEvent.Fire(7);
                  break;
                case 2:
                  this.netAnimationEvent.Fire(0);
                  break;
                case 3:
                  this.jumpWithoutSound();
                  this.yJumpVelocity /= 2f;
                  this.netAnimationEvent.Fire(0);
                  break;
                case 4:
                  if (!(bool) (NetFieldBase<bool, NetBool>) this.home.noHarvest)
                  {
                    this.pathfindToNewCrop();
                    break;
                  }
                  break;
                case 5:
                  this.netAnimationEvent.Fire(8);
                  break;
              }
            }
          }
        }
        bool moveRight = this.moveRight;
        bool moveLeft = this.moveLeft;
        bool moveUp = this.moveUp;
        bool moveDown = this.moveDown;
        bool flag1;
        bool flag2;
        bool flag3;
        bool flag4;
        if (Game1.IsMasterGame)
        {
          if (this.controller == null && this.motion.Equals(Vector2.Zero))
            return;
          flag1 = ((moveRight ? 1 : 0) | ((double) Math.Abs(this.motion.X) <= (double) Math.Abs(this.motion.Y) ? 0 : ((double) this.motion.X > 0.0 ? 1 : 0))) != 0;
          flag2 = ((moveLeft ? 1 : 0) | ((double) Math.Abs(this.motion.X) <= (double) Math.Abs(this.motion.Y) ? 0 : ((double) this.motion.X < 0.0 ? 1 : 0))) != 0;
          flag3 = ((moveUp ? 1 : 0) | ((double) Math.Abs(this.motion.Y) <= (double) Math.Abs(this.motion.X) ? 0 : ((double) this.motion.Y < 0.0 ? 1 : 0))) != 0;
          flag4 = ((moveDown ? 1 : 0) | ((double) Math.Abs(this.motion.Y) <= (double) Math.Abs(this.motion.X) ? 0 : ((double) this.motion.Y > 0.0 ? 1 : 0))) != 0;
        }
        else
        {
          flag2 = this.IsRemoteMoving() && this.FacingDirection == 3;
          flag1 = this.IsRemoteMoving() && this.FacingDirection == 1;
          flag3 = this.IsRemoteMoving() && this.FacingDirection == 0;
          flag4 = this.IsRemoteMoving() && this.FacingDirection == 2;
          if (!flag1 && !flag2 && !flag3 && !flag4)
            return;
        }
        this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
        if (flag1)
        {
          this.flip = false;
          if (!this.Sprite.Animate(time, 16, 8, 50f))
            return;
          this.Sprite.currentFrame = 16;
        }
        else if (flag2)
        {
          if (this.Sprite.Animate(time, 16, 8, 50f))
            this.Sprite.currentFrame = 16;
          this.flip = true;
        }
        else if (flag3)
        {
          if (!this.Sprite.Animate(time, 32, 8, 50f))
            return;
          this.Sprite.currentFrame = 32;
        }
        else
        {
          if (!flag4)
            return;
          this.Sprite.Animate(time, 0, 8, 50f);
        }
      }
    }

    public void pathfindToRandomSpotAroundHut() => this.controller = new PathFindController((Character) this, this.currentLocation, Utility.Vector2ToPoint(new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.home.tileX + 1 + Game1.random.Next(-8, 9)), (float) ((int) (NetFieldBase<int, NetInt>) this.home.tileY + 1 + Game1.random.Next(-8, 9)))), -1, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100);

    public void tryToAddItemToHut(Item i)
    {
      this.lastItemHarvested = i;
      Item obj = this.home.output.Value.addItem(i);
      if (obj == null || !(i is StardewValley.Object))
        return;
      for (int index = 0; index < obj.Stack; ++index)
        Game1.createObjectDebris((int) (NetFieldBase<int, NetInt>) i.parentSheetIndex, this.getTileX(), this.getTileY(), -1, (int) (NetFieldBase<int, NetInt>) (i as StardewValley.Object).quality, 1f, this.currentLocation);
    }

    public override void draw(SpriteBatch b, float alpha = 1f)
    {
      if ((double) this.alpha <= 0.0)
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0 * 4.0 / Math.Pow((double) (this.Sprite.SpriteHeight / 16), 2.0) + (double) this.yJumpOffset - 8.0)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(this.Sprite.SourceRect), this.color.Value * this.alpha, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) ((double) (this.Sprite.SpriteHeight * 4) * 3.0 / 4.0)) / 4f, Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
      if ((bool) (NetFieldBase<bool, NetBool>) this.swimming)
        return;
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position + new Vector2((float) (this.Sprite.SpriteWidth * 4) / 2f, 44f));
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color color = this.color.Value * this.alpha;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double scale = (4.0 + (double) this.yJumpOffset / 40.0) * (double) (float) (NetFieldBase<float, NetFloat>) this.scale;
      double layerDepth = (double) Math.Max(0.0f, (float) this.getStandingY() / 10000f) - 9.99999997475243E-07;
      spriteBatch.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
    }
  }
}
