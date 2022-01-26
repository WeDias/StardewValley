// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Horse
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Characters
{
  public class Horse : NPC
  {
    private readonly NetGuid horseId = new NetGuid();
    private readonly NetFarmerRef netRider = new NetFarmerRef();
    public readonly NetLong ownerId = new NetLong();
    [XmlIgnore]
    public readonly NetBool mounting = new NetBool();
    [XmlIgnore]
    public readonly NetBool dismounting = new NetBool();
    private Vector2 dismountTile;
    private int ridingAnimationDirection;
    private bool roomForHorseAtDismountTile;
    [XmlElement("hat")]
    public readonly NetRef<Hat> hat = new NetRef<Hat>();
    public readonly NetMutex mutex = new NetMutex();
    [XmlIgnore]
    public Action<string> onFootstepAction;
    private bool squeezingThroughGate;

    public Guid HorseId
    {
      get => this.horseId.Value;
      set => this.horseId.Value = value;
    }

    [XmlIgnore]
    public Farmer rider
    {
      get => this.netRider.Value;
      set => this.netRider.Value = value;
    }

    public Horse()
    {
      this.Sprite = new AnimatedSprite("Animals\\horse", 0, 32, 32);
      this.Breather = false;
      this.willDestroyObjectsUnderfoot = false;
      this.HideShadow = true;
      this.Sprite.textureUsesFlippedRightForLeft = true;
      this.Sprite.loop = true;
      this.drawOffset.Set(new Vector2(-16f, 0.0f));
      this.faceDirection(3);
      this.onFootstepAction = new Action<string>(this.PerformDefaultHorseFootstep);
    }

    public Horse(Guid horseId, int xTile, int yTile)
      : this()
    {
      this.Name = "";
      this.displayName = this.Name;
      this.Position = new Vector2((float) xTile, (float) yTile) * 64f;
      this.currentLocation = Game1.currentLocation;
      this.HorseId = horseId;
    }

    public override bool canTalk() => false;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.horseId, (INetSerializable) this.netRider.NetFields, (INetSerializable) this.mounting, (INetSerializable) this.dismounting, (INetSerializable) this.hat, (INetSerializable) this.mutex.NetFields, (INetSerializable) this.ownerId);
      this.position.Field.AxisAlignedMovement = false;
    }

    public Farmer getOwner() => this.ownerId.Value == 0L ? (Farmer) null : Game1.getFarmerMaybeOffline(this.ownerId.Value);

    public override void reloadSprite()
    {
    }

    public override void dayUpdate(int dayOfMonth) => this.faceDirection(3);

    public override Rectangle GetBoundingBox()
    {
      Rectangle boundingBox = base.GetBoundingBox();
      if (this.squeezingThroughGate && (this.FacingDirection == 0 || this.FacingDirection == 2))
        boundingBox.Inflate(-36, 0);
      return boundingBox;
    }

    public override bool canPassThroughActionTiles() => false;

    public void squeezeForGate()
    {
      this.squeezingThroughGate = true;
      if (this.rider == null)
        return;
      this.rider.TemporaryPassableTiles.Add(this.GetBoundingBox());
    }

    public override void update(GameTime time, GameLocation location)
    {
      this.currentLocation = location;
      this.mutex.Update(location);
      this.squeezingThroughGate = false;
      this.faceTowardFarmer = false;
      this.faceTowardFarmerTimer = -1;
      this.Sprite.loop = this.rider != null && !(bool) (NetFieldBase<bool, NetBool>) this.rider.hidden;
      if (this.rider != null && (bool) (NetFieldBase<bool, NetBool>) this.rider.hidden)
        return;
      if (this.rider != null && this.rider.isAnimatingMount)
        this.rider.showNotCarrying();
      if ((bool) (NetFieldBase<bool, NetBool>) this.mounting)
      {
        if (this.rider == null || !this.rider.IsLocalPlayer)
          return;
        if (this.rider.mount != null)
        {
          this.mounting.Value = false;
          this.rider.isAnimatingMount = false;
          this.rider = (Farmer) null;
          this.Halt();
          this.farmerPassesThrough = false;
          return;
        }
        if ((double) this.rider.Position.X < (double) (this.GetBoundingBox().X + 16 - 4))
          this.rider.position.X += 4f;
        else if ((double) this.rider.Position.X > (double) (this.GetBoundingBox().X + 16 + 4))
          this.rider.position.X -= 4f;
        if (this.rider.getStandingY() < this.GetBoundingBox().Y - 4)
          this.rider.position.Y += 4f;
        else if (this.rider.getStandingY() > this.GetBoundingBox().Y + 4)
          this.rider.position.Y -= 4f;
        if (this.rider.yJumpOffset >= -8 && (double) this.rider.yJumpVelocity <= 0.0)
        {
          this.Halt();
          this.Sprite.loop = true;
          this.currentLocation.characters.Remove((NPC) this);
          this.rider.mount = this;
          this.rider.freezePause = -1;
          this.mounting.Value = false;
          this.rider.isAnimatingMount = false;
          this.rider.canMove = true;
          if (this.FacingDirection == 1)
            this.rider.xOffset += 8f;
        }
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.dismounting)
      {
        if (this.rider == null || !this.rider.IsLocalPlayer)
        {
          this.Halt();
          return;
        }
        if (this.rider.isAnimatingMount)
          this.rider.faceDirection(this.FacingDirection);
        Vector2 vector2 = new Vector2((float) ((double) this.dismountTile.X * 64.0 + 32.0) - (float) (this.rider.GetBoundingBox().Width / 2), (float) ((double) this.dismountTile.Y * 64.0 + 4.0));
        if ((double) Math.Abs(this.rider.Position.X - vector2.X) > 4.0)
        {
          if ((double) this.rider.Position.X < (double) vector2.X)
            this.rider.position.X += Math.Min(4f, vector2.X - this.rider.Position.X);
          else if ((double) this.rider.Position.X > (double) vector2.X)
            this.rider.position.X += Math.Max(-4f, vector2.X - this.rider.Position.X);
        }
        if ((double) Math.Abs(this.rider.Position.Y - vector2.Y) > 4.0)
        {
          if ((double) this.rider.Position.Y < (double) vector2.Y)
            this.rider.position.Y += Math.Min(4f, vector2.Y - this.rider.Position.Y);
          else if ((double) this.rider.Position.Y > (double) vector2.Y)
            this.rider.position.Y += Math.Max(-4f, vector2.Y - this.rider.Position.Y);
        }
        if (this.rider.yJumpOffset >= 0 && (double) this.rider.yJumpVelocity <= 0.0)
        {
          this.rider.position.Y += 8f;
          this.rider.position.X = vector2.X;
          int num = 0;
          while (this.rider.currentLocation.isCollidingPosition(this.rider.GetBoundingBox(), Game1.viewport, true, 0, false, (Character) this.rider) && num < 6)
          {
            ++num;
            this.rider.position.Y -= 4f;
          }
          if (num == 6)
          {
            this.rider.Position = this.Position;
            this.dismounting.Value = false;
            this.rider.isAnimatingMount = false;
            this.rider.freezePause = -1;
            this.rider.canMove = true;
            return;
          }
          this.dismount();
        }
      }
      else if (this.rider == null && this.FacingDirection != 2 && this.Sprite.CurrentAnimation == null && Game1.random.NextDouble() < 0.002)
      {
        this.Sprite.loop = false;
        switch (this.FacingDirection)
        {
          case 0:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(25, Game1.random.Next(250, 750)),
              new FarmerSprite.AnimationFrame(14, 10)
            });
            break;
          case 1:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(21, 100),
              new FarmerSprite.AnimationFrame(22, 100),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(24, 400),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(24, 400),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(24, 400),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(22, 100),
              new FarmerSprite.AnimationFrame(21, 100)
            });
            break;
          case 3:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(21, 100, false, true),
              new FarmerSprite.AnimationFrame(22, 100, false, true),
              new FarmerSprite.AnimationFrame(23, 100, false, true),
              new FarmerSprite.AnimationFrame(24, 400, false, true),
              new FarmerSprite.AnimationFrame(23, 400, false, true),
              new FarmerSprite.AnimationFrame(24, 400, false, true),
              new FarmerSprite.AnimationFrame(23, 400, false, true),
              new FarmerSprite.AnimationFrame(24, 400, false, true),
              new FarmerSprite.AnimationFrame(23, 400, false, true),
              new FarmerSprite.AnimationFrame(22, 100, false, true),
              new FarmerSprite.AnimationFrame(21, 100, false, true)
            });
            break;
        }
      }
      else if (this.rider != null)
      {
        if (this.FacingDirection != this.rider.FacingDirection || this.ridingAnimationDirection != this.FacingDirection)
        {
          this.Sprite.StopAnimation();
          this.faceDirection(this.rider.FacingDirection);
        }
        int num = !this.rider.movementDirections.Any<int>() || !this.rider.CanMove ? (this.rider.position.Field.IsInterpolating() ? 1 : 0) : 1;
        this.SyncPositionToRider();
        if (num != 0 && this.Sprite.CurrentAnimation == null)
        {
          if (this.FacingDirection == 1)
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(8, 70),
              new FarmerSprite.AnimationFrame(9, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(10, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(11, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(12, 70),
              new FarmerSprite.AnimationFrame(13, 70)
            });
          else if (this.FacingDirection == 3)
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(8, 70, false, true),
              new FarmerSprite.AnimationFrame(9, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(10, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(11, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(12, 70, false, true),
              new FarmerSprite.AnimationFrame(13, 70, false, true)
            });
          else if (this.FacingDirection == 0)
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(15, 70),
              new FarmerSprite.AnimationFrame(16, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(17, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(18, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(19, 70),
              new FarmerSprite.AnimationFrame(20, 70)
            });
          else if (this.FacingDirection == 2)
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(1, 70),
              new FarmerSprite.AnimationFrame(2, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(3, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(4, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
              new FarmerSprite.AnimationFrame(5, 70),
              new FarmerSprite.AnimationFrame(6, 70)
            });
          this.ridingAnimationDirection = this.FacingDirection;
        }
        if (num == 0)
        {
          this.Sprite.StopAnimation();
          this.faceDirection(this.rider.FacingDirection);
        }
      }
      if (this.FacingDirection == 3)
        this.drawOffset.Set(Vector2.Zero);
      else
        this.drawOffset.Set(new Vector2(-16f, 0.0f));
      this.flip = this.FacingDirection == 3;
      base.update(time, location);
    }

    public virtual void OnMountFootstep(Farmer who)
    {
      if (this.onFootstepAction == null || this.rider == null)
        return;
      this.onFootstepAction(this.rider.currentLocation.doesTileHaveProperty((int) this.rider.getTileLocation().X, (int) this.rider.getTileLocation().Y, "Type", "Back"));
    }

    public virtual void PerformDefaultHorseFootstep(string step_type)
    {
      if (this.rider == null)
        return;
      if (!(step_type == "Stone"))
      {
        if (step_type == "Wood")
        {
          if (this.rider.ShouldHandleAnimationSound())
            this.rider.currentLocation.localSoundAt("woodyStep", this.getTileLocation());
          if (this.rider != Game1.player)
            return;
          Rumble.rumble(0.1f, 50f);
        }
        else
        {
          if (this.rider.ShouldHandleAnimationSound())
            this.rider.currentLocation.localSoundAt("thudStep", this.getTileLocation());
          if (this.rider != Game1.player)
            return;
          Rumble.rumble(0.3f, 50f);
        }
      }
      else
      {
        if (this.rider.ShouldHandleAnimationSound())
          this.rider.currentLocation.localSoundAt("stoneStep", this.getTileLocation());
        if (this.rider != Game1.player)
          return;
        Rumble.rumble(0.1f, 50f);
      }
    }

    public override void collisionWithFarmerBehavior() => base.collisionWithFarmerBehavior();

    public void dismount(bool from_demolish = false)
    {
      this.mutex.ReleaseLock();
      this.rider.mount = (Horse) null;
      if (this.currentLocation == null)
        return;
      Stable stable = (Stable) null;
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building is Stable && (building as Stable).HorseId == this.HorseId)
        {
          stable = building as Stable;
          break;
        }
      }
      if (stable != null && !from_demolish && !this.currentLocation.characters.Where<NPC>((Func<NPC, bool>) (c => c is Horse && (c as Horse).HorseId == this.HorseId)).Any<NPC>())
        this.currentLocation.characters.Add((NPC) this);
      this.SyncPositionToRider();
      this.rider.TemporaryPassableTiles.Add(new Rectangle((int) this.dismountTile.X * 64, (int) this.dismountTile.Y * 64, 64, 64));
      this.rider.freezePause = -1;
      this.dismounting.Value = false;
      this.rider.isAnimatingMount = false;
      this.rider.canMove = true;
      this.rider.forceCanMove();
      this.rider.xOffset = 0.0f;
      this.rider = (Farmer) null;
      this.Halt();
      this.farmerPassesThrough = false;
    }

    public void nameHorse(string name)
    {
      if (name.Length <= 0)
        return;
      Game1.multiplayer.globalChatInfoMessage("HorseNamed", Game1.player.Name, name);
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (allCharacter.isVillager() && allCharacter.Name.Equals(name))
          name += " ";
      }
      this.Name = name;
      this.displayName = name;
      if (Game1.player.horseName.Value == null)
        Game1.player.horseName.Value = name;
      Game1.exitActiveMenu();
      Game1.playSound("newArtifact");
      if (!this.mutex.IsLockHeld())
        return;
      this.mutex.ReleaseLock();
    }

    public override bool checkAction(Farmer who, GameLocation l)
    {
      if (who != null && !who.canMove)
        return false;
      if (this.rider == null)
      {
        this.mutex.RequestLock((Action) (() =>
        {
          if (who.mount != null || this.rider != null || who.FarmerSprite.PauseForSingleAnimation)
            this.mutex.ReleaseLock();
          else if ((this.getOwner() == Game1.player || this.getOwner() == null && (Game1.player.horseName.Value == null || Game1.player.horseName.Value.Length == 0 || Utility.findHorseForPlayer(Game1.player.UniqueMultiplayerID) == null)) && this.Name.Length <= 0)
          {
            foreach (Building building in (Game1.getLocationFromName("Farm") as Farm).buildings)
            {
              if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0 && building is Stable)
              {
                Stable stable = building as Stable;
                if (stable.getStableHorse() == this)
                {
                  stable.owner.Value = who.UniqueMultiplayerID;
                  stable.updateHorseOwnership();
                }
                else if ((long) stable.owner == who.UniqueMultiplayerID)
                {
                  stable.owner.Value = 0L;
                  stable.updateHorseOwnership();
                }
              }
            }
            if (Game1.player.horseName.Value != null && Game1.player.horseName.Value.Length != 0)
              return;
            Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.nameHorse), Game1.content.LoadString("Strings\\Characters:NameYourHorse"), Game1.content.LoadString("Strings\\Characters:DefaultHorseName"));
          }
          else if (who.items.Count > who.CurrentToolIndex && who.items[who.CurrentToolIndex] != null && who.Items[who.CurrentToolIndex] is Hat)
          {
            if (this.hat.Value != null)
            {
              Game1.createItemDebris((Item) (Hat) (NetFieldBase<Hat, NetRef<Hat>>) this.hat, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position, (int) this.facingDirection);
              this.hat.Value = (Hat) null;
            }
            else
            {
              Hat hat = who.Items[who.CurrentToolIndex] as Hat;
              who.Items[who.CurrentToolIndex] = (Item) null;
              this.hat.Value = hat;
              Game1.playSound("dirtyHit");
            }
            this.mutex.ReleaseLock();
          }
          else
          {
            this.rider = who;
            this.rider.freezePause = 5000;
            this.rider.synchronizedJump(6f);
            this.rider.Halt();
            if ((double) this.rider.Position.X < (double) this.Position.X)
              this.rider.faceDirection(1);
            l.playSound("dwop");
            this.mounting.Value = true;
            this.rider.isAnimatingMount = true;
            this.rider.completelyStopAnimatingOrDoingAction();
            this.rider.faceGeneralDirection(Utility.PointToVector2(this.GetBoundingBox().Center), 0, false, false);
          }
        }));
        return true;
      }
      this.dismounting.Value = true;
      this.rider.isAnimatingMount = true;
      this.farmerPassesThrough = false;
      this.rider.TemporaryPassableTiles.Clear();
      Vector2 tileForCharacter = Utility.recursiveFindOpenTileForCharacter((Character) this.rider, this.rider.currentLocation, this.rider.getTileLocation(), 8);
      this.Position = new Vector2((float) ((double) tileForCharacter.X * 64.0 + 32.0) - (float) (this.GetBoundingBox().Width / 2), (float) ((double) tileForCharacter.Y * 64.0 + 4.0));
      this.roomForHorseAtDismountTile = !this.currentLocation.isCollidingPosition(this.GetBoundingBox(), Game1.viewport, true, 0, false, (Character) this);
      this.Position = this.rider.Position;
      this.dismounting.Value = false;
      this.rider.isAnimatingMount = false;
      this.Halt();
      if (!tileForCharacter.Equals(Vector2.Zero) && (double) Vector2.Distance(tileForCharacter, this.rider.getTileLocation()) < 2.0)
      {
        this.rider.synchronizedJump(6f);
        l.playSound("dwop");
        this.rider.freezePause = 5000;
        this.rider.Halt();
        this.rider.xOffset = 0.0f;
        this.dismounting.Value = true;
        this.rider.isAnimatingMount = true;
        this.dismountTile = tileForCharacter;
        Game1.debugOutput = "dismount tile: " + tileForCharacter.ToString();
      }
      else
        this.dismount();
      return true;
    }

    public void SyncPositionToRider()
    {
      if (this.rider == null || (bool) (NetFieldBase<bool, NetBool>) this.dismounting && !this.roomForHorseAtDismountTile)
        return;
      this.Position = (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.rider.position;
    }

    public override void draw(SpriteBatch b)
    {
      this.flip = this.FacingDirection == 3;
      this.Sprite.UpdateSourceRect();
      base.draw(b);
      if (this.FacingDirection == 2 && this.rider != null)
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(48f, -24f - this.rider.yOffset), new Rectangle?(new Rectangle(160, 96, 9, 15)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.Position.Y + 64.0) / 10000.0));
      bool flag = true;
      if (this.hat.Value == null)
        return;
      Vector2 zero = Vector2.Zero;
      switch ((int) (NetFieldBase<int, NetInt>) this.hat.Value.which)
      {
        case 6:
          zero.Y += 2f;
          if (this.FacingDirection == 2)
          {
            --zero.Y;
            break;
          }
          break;
        case 9:
        case 32:
          if (this.FacingDirection == 0 || this.FacingDirection == 2)
          {
            ++zero.Y;
            break;
          }
          break;
        case 10:
          zero.Y += 3f;
          if ((int) this.facingDirection == 0)
          {
            flag = false;
            break;
          }
          break;
        case 11:
        case 39:
          if (this.FacingDirection == 3 || this.FacingDirection == 1)
          {
            if (this.flip)
            {
              zero.X += 2f;
              break;
            }
            zero.X -= 2f;
            break;
          }
          break;
        case 14:
          if ((int) this.facingDirection == 0)
          {
            zero.X = -100f;
            break;
          }
          break;
        case 26:
          if (this.FacingDirection == 3 || this.FacingDirection == 1)
          {
            if (this.flip)
            {
              ++zero.X;
              break;
            }
            --zero.X;
            break;
          }
          break;
        case 31:
          ++zero.Y;
          break;
        case 56:
        case 67:
          if (this.FacingDirection == 0)
          {
            flag = false;
            break;
          }
          break;
      }
      zero *= 4f;
      if (this.shakeTimer > 0)
        zero += new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      if ((double) zero.X <= -100.0)
        return;
      float num = (float) this.GetBoundingBox().Center.Y / 10000f;
      if (this.rider != null)
        num = this.FacingDirection != 0 ? (this.FacingDirection != 2 ? (float) (((double) this.position.Y + 64.0 - 1.0) / 10000.0) : (float) (((double) this.position.Y + 64.0 + (this.rider != null ? 1.0 : 1.0)) / 10000.0)) : (float) (((double) this.position.Y + 64.0 - 32.0) / 10000.0);
      if (!flag)
        return;
      float layerDepth = num + 1E-07f;
      switch (this.Sprite.CurrentFrame)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(30f, (float) (-42.0 - (this.rider != null ? (double) this.rider.yOffset : 0.0)))), 1.333333f, 1f, layerDepth, 2);
          break;
        case 7:
        case 11:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -74f), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -74f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 8:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -74f), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -74f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 9:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -70f), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -70f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 10:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -70f), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -70f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 12:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -78f)), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -78f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 13:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -78f)), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -78f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 14:
        case 15:
        case 16:
        case 17:
        case 18:
        case 19:
        case 20:
        case 25:
          this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(28f, (float) (-106.0 - (this.rider != null ? (double) this.rider.yOffset : 0.0))), 1.333333f, 1f, layerDepth, 0);
          break;
        case 21:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -66f)), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -66f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 22:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -54f)), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -54f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 23:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -42f)), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -42f)), 1.333333f, 1f, layerDepth, 1);
          break;
        case 24:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -42f)), 1.333333f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -42f)), 1.333333f, 1f, layerDepth, 1);
          break;
      }
    }
  }
}
