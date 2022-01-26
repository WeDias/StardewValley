// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmerSprite
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley
{
  public class FarmerSprite : AnimatedSprite
  {
    public const int walkDown = 0;
    public const int walkRight = 8;
    public const int walkUp = 16;
    public const int walkLeft = 24;
    public const int runDown = 32;
    public const int runRight = 40;
    public const int runUp = 48;
    public const int runLeft = 56;
    public const int grabDown = 64;
    public const int grabRight = 72;
    public const int grabUp = 80;
    public const int grabLeft = 88;
    public const int carryWalkDown = 96;
    public const int carryWalkRight = 104;
    public const int carryWalkUp = 112;
    public const int carryWalkLeft = 120;
    public const int carryRunDown = 128;
    public const int carryRunRight = 136;
    public const int carryRunUp = 144;
    public const int carryRunLeft = 152;
    public const int toolDown = 160;
    public const int toolRight = 168;
    public const int toolUp = 176;
    public const int toolLeft = 184;
    public const int toolChooseDown = 192;
    public const int toolChooseRight = 194;
    public const int toolChooseUp = 196;
    public const int toolChooseLeft = 198;
    public const int seedThrowDown = 200;
    public const int seedThrowRight = 204;
    public const int seedThrowUp = 208;
    public const int seedThrowLeft = 212;
    public const int eat = 216;
    public const int sick = 224;
    public const int swordswipeDown = 232;
    public const int swordswipeRight = 240;
    public const int swordswipeUp = 248;
    public const int swordswipeLeft = 256;
    public const int punchDown = 272;
    public const int punchRight = 274;
    public const int punchUp = 276;
    public const int punchLeft = 278;
    public const int harvestItemUp = 279;
    public const int harvestItemRight = 280;
    public const int harvestItemDown = 281;
    public const int harvestItemLeft = 282;
    public const int shearUp = 283;
    public const int shearRight = 284;
    public const int shearDown = 285;
    public const int shearLeft = 286;
    public const int milkUp = 287;
    public const int milkRight = 288;
    public const int milkDown = 289;
    public const int milkLeft = 290;
    public const int tired = 291;
    public const int tired2 = 292;
    public const int passOutTired = 293;
    public const int drink = 294;
    public const int fishingUp = 295;
    public const int fishingRight = 296;
    public const int fishingDown = 297;
    public const int fishingLeft = 298;
    public const int fishingDoneUp = 299;
    public const int fishingDoneRight = 300;
    public const int fishingDoneDown = 301;
    public const int fishingDoneLeft = 302;
    public const int pan = 303;
    public const int showHoldingEdible = 304;
    private int currentToolIndex;
    private float oldInterval;
    public bool pauseForSingleAnimation;
    public bool animateBackwards;
    public bool loopThisAnimation;
    public bool freezeUntilDialogueIsOver;
    private int currentSingleAnimation = -1;
    private int currentAnimationFrames;
    public float currentSingleAnimationInterval = 200f;
    public float intervalModifier = 1f;
    public string currentStep = "sandyStep";
    private Farmer owner;
    public bool animatingBackwards;
    public const int cheer = 97;

    public FarmerSprite.AnimationFrame CurrentAnimationFrame => this.CurrentAnimation == null ? new FarmerSprite.AnimationFrame(0, 100, 0, false, false) : this.CurrentAnimation[this.currentAnimationIndex % this.CurrentAnimation.Count];

    public int CurrentSingleAnimation => this.CurrentAnimation != null ? this.CurrentAnimation[0].frame : -1;

    public void setOwner(Farmer owner) => this.owner = owner;

    public override int CurrentFrame
    {
      get => this.currentFrame;
      set
      {
        if (this.currentFrame != value && !this.freezeUntilDialogueIsOver)
        {
          this.currentFrame = value;
          this.UpdateSourceRect();
        }
        if (value <= FarmerRenderer.featureYOffsetPerFrame.Length - 1)
          return;
        this.currentFrame = 0;
      }
    }

    public void setCurrentAnimation(FarmerSprite.AnimationFrame[] animation)
    {
      this.currentSingleAnimation = -1;
      this.currentAnimation.Clear();
      this.currentAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) animation);
      this.oldFrame = this.CurrentFrame;
      this.currentAnimationIndex = 0;
      if (this.CurrentAnimation.Count <= 0)
        return;
      this.interval = (float) this.CurrentAnimation[0].milliseconds;
      this.CurrentFrame = this.CurrentAnimation[0].frame;
      this.currentAnimationFrames = this.CurrentAnimation.Count;
    }

    public override void faceDirection(int direction)
    {
      bool flag = false;
      if (this.owner != null)
        flag = this.owner.IsCarrying();
      if (this.IsPlayingBasicAnimation(direction, flag))
        return;
      switch (direction)
      {
        case 0:
          this.setCurrentFrame(12, 1, 100, 1, false, flag);
          break;
        case 1:
          this.setCurrentFrame(6, 1, 100, 1, false, flag);
          break;
        case 2:
          this.setCurrentFrame(0, 1, 100, 1, false, flag);
          break;
        case 3:
          this.setCurrentFrame(6, 1, 100, 1, true, flag);
          break;
      }
      this.UpdateSourceRect();
    }

    public virtual bool IsPlayingBasicAnimation(int direction, bool carrying)
    {
      bool flag = false;
      if (this.owner != null && this.owner.CanMove && this.owner.isMoving())
        flag = true;
      switch (direction)
      {
        case 0:
          if (carrying)
          {
            if (!flag)
              return this.CurrentFrame == 113;
            if (this.currentSingleAnimation == 112 || this.currentSingleAnimation == 144)
              return true;
            break;
          }
          if (!flag)
            return this.CurrentFrame == 17;
          if (this.currentSingleAnimation == 16 || this.currentSingleAnimation == 48)
            return true;
          break;
        case 1:
          if (carrying)
          {
            if (!flag)
              return this.CurrentFrame == 105;
            if (this.currentSingleAnimation == 104 || this.currentSingleAnimation == 136)
              return true;
            break;
          }
          if (!flag)
            return this.CurrentFrame == 9;
          if (this.currentSingleAnimation == 8 || this.currentSingleAnimation == 40)
            return true;
          break;
        case 2:
          if (carrying)
          {
            if (!flag)
              return this.CurrentFrame == 97;
            if (this.currentSingleAnimation == 96 || this.currentSingleAnimation == 128)
              return true;
            break;
          }
          if (!flag)
            return this.CurrentFrame == 1;
          if (this.currentSingleAnimation == 0 || this.currentSingleAnimation == 32)
            return true;
          break;
        case 3:
          if (carrying)
          {
            if (!flag)
              return this.CurrentFrame == 121;
            if (this.currentSingleAnimation == 120 || this.currentSingleAnimation == 152)
              return true;
            break;
          }
          if (!flag)
            return this.CurrentFrame == 25;
          if (this.currentSingleAnimation == 24 || this.currentSingleAnimation == 56)
            return true;
          break;
      }
      return false;
    }

    public void setCurrentSingleFrame(int which, short interval = 32000, bool secondaryArm = false, bool flip = false)
    {
      this.loopThisAnimation = false;
      this.currentAnimation.Clear();
      this.currentAnimation.Add(new FarmerSprite.AnimationFrame((int) (short) which, (int) interval, secondaryArm, flip));
      this.CurrentFrame = this.CurrentAnimation[0].frame;
    }

    public void setCurrentFrame(int which) => this.setCurrentFrame(which, 0);

    public void setCurrentFrame(int which, int offset) => this.setCurrentFrame(which, offset, 100, 1, false, false);

    public void setCurrentFrameBackwards(
      int which,
      int offset,
      int interval,
      int numFrames,
      bool secondaryArm,
      bool flip)
    {
      FarmerSprite.getAnimationFromIndex(which, this, interval, numFrames, secondaryArm, flip);
      this.CurrentAnimation.Reverse();
      this.CurrentFrame = this.CurrentAnimation[Math.Min(this.CurrentAnimation.Count - 1, offset)].frame;
    }

    public void setCurrentFrame(
      int which,
      int offset,
      int interval,
      int numFrames,
      bool flip,
      bool secondaryArm)
    {
      FarmerSprite.getAnimationFromIndex(which, this, interval, numFrames, flip, secondaryArm);
      this.currentAnimationIndex = Math.Min(this.CurrentAnimation.Count - 1, offset);
      this.CurrentFrame = this.CurrentAnimation[this.currentAnimationIndex].frame;
      this.interval = (float) this.CurrentAnimationFrame.milliseconds;
      this.timer = 0.0f;
    }

    public bool PauseForSingleAnimation
    {
      get => this.pauseForSingleAnimation;
      set => this.pauseForSingleAnimation = value;
    }

    public int CurrentToolIndex
    {
      get => this.currentToolIndex;
      set => this.currentToolIndex = value;
    }

    public FarmerSprite()
    {
      this.interval /= 2f;
      this.SpriteWidth = 16;
      this.SpriteHeight = 32;
      this.UpdateSourceRect();
    }

    public FarmerSprite(string texture)
      : base(texture)
    {
      this.interval /= 2f;
      this.SpriteWidth = 16;
      this.SpriteHeight = 32;
      this.UpdateSourceRect();
    }

    public void animate(int whichAnimation, GameTime time) => this.animate(whichAnimation, time.ElapsedGameTime.Milliseconds);

    public void animate(int whichAnimation, int milliseconds)
    {
      if (this.PauseForSingleAnimation)
        return;
      if (whichAnimation != this.currentSingleAnimation || this.CurrentAnimation == null || this.CurrentAnimation.Count <= 1)
      {
        float timer = this.timer;
        int currentAnimationIndex = this.currentAnimationIndex;
        this.currentSingleAnimation = whichAnimation;
        this.setCurrentFrame(whichAnimation);
        this.timer = timer;
        this.CurrentFrame = this.CurrentAnimation[Math.Min(currentAnimationIndex, this.CurrentAnimation.Count - 1)].frame;
        this.currentAnimationIndex = currentAnimationIndex % this.CurrentAnimation.Count;
        this.UpdateSourceRect();
      }
      this.animate(milliseconds);
    }

    public void checkForSingleAnimation(GameTime time)
    {
      if (!this.PauseForSingleAnimation)
        return;
      if (!this.animateBackwards)
        this.animateOnce(time);
      else
        this.animateBackwardsOnce(time);
    }

    public void animateOnce(int whichAnimation, float animationInterval, int numberOfFrames) => this.animateOnce(whichAnimation, animationInterval, numberOfFrames, (AnimatedSprite.endOfAnimationBehavior) null);

    public void animateOnce(
      int whichAnimation,
      float animationInterval,
      int numberOfFrames,
      AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction)
    {
      this.animateOnce(whichAnimation, animationInterval, numberOfFrames, endOfBehaviorFunction, false, false);
    }

    public void animateOnce(
      int whichAnimation,
      float animationInterval,
      int numberOfFrames,
      AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction,
      bool flip,
      bool secondaryArm)
    {
      this.animateOnce(whichAnimation, animationInterval, numberOfFrames, endOfBehaviorFunction, flip, secondaryArm, false);
    }

    public void animateOnce(
      FarmerSprite.AnimationFrame[] animation,
      AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction = null)
    {
      this.currentSingleAnimation = -1;
      this.CurrentFrame = this.currentSingleAnimation;
      this.PauseForSingleAnimation = true;
      this.oldFrame = this.CurrentFrame;
      this.oldInterval = this.interval;
      this.currentSingleAnimationInterval = 100f;
      this.timer = 0.0f;
      this.currentAnimation.Clear();
      this.currentAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) animation);
      this.CurrentFrame = this.CurrentAnimation[0].frame;
      this.currentAnimationFrames = this.CurrentAnimation.Count;
      this.currentAnimationIndex = 0;
      this.interval = (float) this.CurrentAnimationFrame.milliseconds;
      this.loopThisAnimation = false;
      this.endOfAnimationFunction = endOfBehaviorFunction;
      if (this.currentAnimationFrames <= 0 || this.CurrentAnimation[0].frameStartBehavior == null)
        return;
      this.CurrentAnimation[0].frameStartBehavior(this.owner);
    }

    public void showFrameUntilDialogueOver(int whichFrame)
    {
      this.freezeUntilDialogueIsOver = true;
      this.setCurrentFrame(whichFrame);
      this.UpdateSourceRect();
    }

    public void animateOnce(
      int whichAnimation,
      float animationInterval,
      int numberOfFrames,
      AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction,
      bool flip,
      bool secondaryArm,
      bool backwards)
    {
      if (whichAnimation != this.currentSingleAnimation)
        this.PauseForSingleAnimation = false;
      if (this.PauseForSingleAnimation || this.freezeUntilDialogueIsOver)
        return;
      this.currentSingleAnimation = whichAnimation;
      this.CurrentFrame = this.currentSingleAnimation;
      this.PauseForSingleAnimation = true;
      this.oldFrame = this.CurrentFrame;
      this.oldInterval = this.interval;
      this.currentSingleAnimationInterval = animationInterval;
      this.endOfAnimationFunction = endOfBehaviorFunction;
      this.timer = 0.0f;
      this.animatingBackwards = false;
      if (backwards)
      {
        this.animatingBackwards = true;
        this.setCurrentFrameBackwards(this.currentSingleAnimation, 0, (int) animationInterval, numberOfFrames, secondaryArm, flip);
      }
      else
        this.setCurrentFrame(this.currentSingleAnimation, 0, (int) animationInterval, numberOfFrames, secondaryArm, flip);
      if (this.CurrentAnimation[0].frameStartBehavior != null)
        this.CurrentAnimation[0].frameStartBehavior(this.owner);
      if ((double) this.owner.Stamina <= 0.0 && (bool) (NetFieldBase<bool, NetBool>) this.owner.usingTool)
      {
        for (int index = 0; index < this.CurrentAnimation.Count; ++index)
          this.CurrentAnimation[index] = new FarmerSprite.AnimationFrame(this.CurrentAnimation[index].frame, this.CurrentAnimation[index].milliseconds * 2, this.CurrentAnimation[index].positionOffset, this.CurrentAnimation[index].secondaryArm, this.CurrentAnimation[index].flip, this.CurrentAnimation[index].frameStartBehavior, this.CurrentAnimation[index].frameEndBehavior, this.CurrentAnimation[index].xOffset);
      }
      this.currentAnimationFrames = this.CurrentAnimation.Count;
      this.currentAnimationIndex = 0;
      this.interval = (float) this.CurrentAnimationFrame.milliseconds;
      if (!this.owner.UsingTool || this.owner.CurrentTool == null)
        return;
      this.CurrentToolIndex = this.owner.CurrentTool.CurrentParentTileIndex;
      if (!(this.owner.CurrentTool is FishingRod))
        return;
      if (this.owner.FacingDirection == 3 || this.owner.FacingDirection == 1)
        this.CurrentToolIndex = 55;
      else
        this.CurrentToolIndex = 48;
    }

    public void animateBackwardsOnce(int whichAnimation, float animationInterval) => this.animateOnce(whichAnimation, animationInterval, 6, (AnimatedSprite.endOfAnimationBehavior) null, false, false, true);

    public bool isUsingWeapon()
    {
      if (!this.PauseForSingleAnimation)
        return false;
      if (this.currentSingleAnimation >= 232 && this.currentSingleAnimation < 264)
        return true;
      return this.currentSingleAnimation >= 272 && this.currentSingleAnimation < 280;
    }

    public int getWeaponTypeFromAnimation()
    {
      if (this.currentSingleAnimation >= 272 && this.currentSingleAnimation < 280)
        return 1;
      return this.currentSingleAnimation >= 232 && this.currentSingleAnimation < 264 ? 3 : -1;
    }

    public bool isOnToolAnimation()
    {
      if (!this.PauseForSingleAnimation && !this.owner.UsingTool)
        return false;
      if (this.currentSingleAnimation >= 160 && this.currentSingleAnimation < 192 || this.currentSingleAnimation >= 232 && this.currentSingleAnimation < 264)
        return true;
      return this.currentSingleAnimation >= 272 && this.currentSingleAnimation < 280;
    }

    public bool isPassingOut()
    {
      if (!this.PauseForSingleAnimation)
        return false;
      return this.currentSingleAnimation == 293 || this.CurrentFrame == 5;
    }

    private void doneWithAnimation()
    {
      --this.CurrentFrame;
      this.interval = this.oldInterval;
      if (!Game1.eventUp)
      {
        this.owner.CanMove = true;
        this.owner.Halt();
      }
      this.PauseForSingleAnimation = false;
      this.animatingBackwards = false;
    }

    private void currentAnimationTick()
    {
      if (this.currentAnimationIndex >= this.CurrentAnimation.Count)
        return;
      if (this.CurrentAnimation[this.currentAnimationIndex].frameEndBehavior != null)
        this.CurrentAnimation[this.currentAnimationIndex].frameEndBehavior(this.owner);
      ++this.currentAnimationIndex;
      if (this.loopThisAnimation)
        this.currentAnimationIndex %= this.CurrentAnimation.Count;
      else if (this.currentAnimationIndex >= this.CurrentAnimation.Count)
      {
        this.loopThisAnimation = false;
        return;
      }
      if (this.CurrentAnimation[this.currentAnimationIndex].frameStartBehavior != null)
        this.CurrentAnimation[this.currentAnimationIndex].frameStartBehavior(this.owner);
      if (this.CurrentAnimation != null && this.currentAnimationIndex < this.CurrentAnimation.Count)
      {
        this.currentSingleAnimationInterval = (float) this.CurrentAnimation[this.currentAnimationIndex].milliseconds;
        this.CurrentFrame = this.CurrentAnimation[this.currentAnimationIndex].frame;
        this.interval = (float) this.CurrentAnimation[this.currentAnimationIndex].milliseconds;
      }
      else
      {
        this.owner.completelyStopAnimatingOrDoingAction();
        this.owner.forceCanMove();
      }
    }

    public override void UpdateSourceRect() => this.SourceRect = new Rectangle(this.CurrentFrame * this.SpriteWidth % 96, this.CurrentFrame * this.SpriteWidth / 96 * this.SpriteHeight, this.SpriteWidth, this.SpriteHeight);

    private void animateOnce(GameTime time)
    {
      if (this.freezeUntilDialogueIsOver || this.owner == null)
        return;
      this.timer += (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.timer > (double) this.interval * (double) this.intervalModifier)
      {
        this.currentAnimationTick();
        this.timer = 0.0f;
        if (this.currentAnimationIndex > this.currentAnimationFrames - 1)
        {
          if (this.CurrentAnimationFrame.frameEndBehavior != null)
            this.CurrentAnimationFrame.frameEndBehavior(this.owner);
          if (this.endOfAnimationFunction != null)
          {
            AnimatedSprite.endOfAnimationBehavior animationFunction = this.endOfAnimationFunction;
            this.endOfAnimationFunction = (AnimatedSprite.endOfAnimationBehavior) null;
            Farmer owner = this.owner;
            animationFunction(owner);
            if (this.owner.UsingTool && this.owner.CurrentTool.Name.Equals("Fishing Rod"))
            {
              this.PauseForSingleAnimation = false;
              this.interval = this.oldInterval;
              this.owner.CanMove = false;
              return;
            }
            if (this.owner.CurrentTool is MeleeWeapon && (int) (NetFieldBase<int, NetInt>) (this.owner.CurrentTool as MeleeWeapon).type == 1)
              return;
            this.doneWithAnimation();
            return;
          }
          this.doneWithAnimation();
          if (this.owner.isEating)
            this.owner.doneEating();
        }
        int currentSingleAnimation = this.currentSingleAnimation;
        if (currentSingleAnimation <= 173)
        {
          if (currentSingleAnimation <= 165)
          {
            if (((uint) (currentSingleAnimation - 160) <= 1U || currentSingleAnimation == 165) && this.owner.CurrentTool != null)
              this.owner.CurrentTool.Update(2, this.currentAnimationIndex, this.owner);
          }
          else if (currentSingleAnimation == 168 || (uint) (currentSingleAnimation - 172) <= 1U)
            ;
        }
        else if (currentSingleAnimation <= 181)
        {
          if ((currentSingleAnimation == 176 || (uint) (currentSingleAnimation - 180) <= 1U) && this.owner.CurrentTool != null)
            this.owner.CurrentTool.Update(0, this.currentAnimationIndex, this.owner);
        }
        else if (currentSingleAnimation == 184 || (uint) (currentSingleAnimation - 188) <= 1U)
          ;
        if (this.CurrentFrame == 109 && this.owner.ShouldHandleAnimationSound())
          this.owner.currentLocation.localSound("eat");
        if (this.isOnToolAnimation() && !this.isUsingWeapon() && this.currentAnimationIndex == 4 && this.currentToolIndex % 2 == 0 && !(this.owner.CurrentTool is FishingRod))
          ++this.currentToolIndex;
      }
      this.UpdateSourceRect();
    }

    private void checkForFootstep()
    {
      if (Game1.player.isRidingHorse() || this.owner == null || this.owner.currentLocation != Game1.currentLocation)
        return;
      Vector2 key = this.owner != null ? this.owner.getTileLocation() : Game1.player.getTileLocation();
      if (Game1.currentLocation.IsOutdoors || Game1.currentLocation.Name.ToLower().Contains("mine") || Game1.currentLocation.Name.ToLower().Contains("cave") || Game1.currentLocation.IsGreenhouse)
      {
        string str = Game1.currentLocation.doesTileHaveProperty((int) key.X, (int) key.Y, "Type", "Buildings");
        if (str == null || str.Length < 1)
          str = Game1.currentLocation.doesTileHaveProperty((int) key.X, (int) key.Y, "Type", "Back");
        if (str != null)
        {
          if (!(str == "Dirt"))
          {
            if (!(str == "Stone"))
            {
              if (!(str == "Grass"))
              {
                if (str == "Wood")
                  this.currentStep = "woodyStep";
              }
              else
                this.currentStep = Game1.currentLocation.GetSeasonForLocation().Equals("winter") ? "snowyStep" : "grassyStep";
            }
            else
              this.currentStep = "stoneStep";
          }
          else
            this.currentStep = "sandyStep";
        }
      }
      else
        this.currentStep = "thudStep";
      if ((this.currentSingleAnimation >= 32 && this.currentSingleAnimation <= 56 || this.currentSingleAnimation >= 128 && this.currentSingleAnimation <= 152) && this.currentAnimationIndex % 4 == 0)
      {
        string cueName = this.owner.currentLocation.getFootstepSoundReplacement(this.currentStep);
        if (this.owner.onBridge.Value)
        {
          if (this.owner.currentLocation == Game1.currentLocation && Utility.isOnScreen(this.owner.Position, 384))
            cueName = "thudStep";
          if (this.owner.bridge != null)
            this.owner.bridge.OnFootstep(this.owner.Position);
        }
        if (Game1.currentLocation.terrainFeatures.ContainsKey(key) && Game1.currentLocation.terrainFeatures[key] is Flooring)
          cueName = ((Flooring) Game1.currentLocation.terrainFeatures[key]).getFootstepSound();
        Vector2 position = (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.owner.position;
        if (this.owner.shouldShadowBeOffset)
          position += this.owner.drawOffset.Value;
        if (cueName.Equals("sandyStep"))
        {
          Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(128, 2948, 64, 64), 80f, 8, 0, new Vector2(position.X + 16f + (float) Game1.random.Next(-8, 8), position.Y + (float) (Game1.random.Next(-3, -1) * 4)), false, Game1.random.NextDouble() < 0.5, position.Y / 10000f, 0.03f, Color.Khaki * 0.45f, (float) (0.75 + (double) Game1.random.Next(-3, 4) * 0.0500000007450581), 0.0f, 0.0f, 0.0f));
          Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(128, 2948, 64, 64), 80f, 8, 0, new Vector2(position.X + 16f + (float) Game1.random.Next(-4, 4), position.Y + (float) (Game1.random.Next(-3, -1) * 4)), false, Game1.random.NextDouble() < 0.5, position.Y / 10000f, 0.03f, Color.Khaki * 0.45f, (float) (0.550000011920929 + (double) Game1.random.Next(-3, 4) * 0.0500000007450581), 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 20
          });
        }
        else if (cueName.Equals("snowyStep"))
          Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(position.X + 24f + (float) (Game1.random.Next(-4, 4) * 4), position.Y + 8f + (float) (Game1.random.Next(-4, 4) * 4)), false, false, position.Y / 1E+07f, 0.01f, Color.White, (float) (3.0 + Game1.random.NextDouble()), 0.0f, (int) this.owner.facingDirection == 1 || (int) this.owner.facingDirection == 3 ? -0.7853982f : 0.0f, 0.0f));
        if (cueName != null && this.owner.currentLocation == Game1.currentLocation && Utility.isOnScreen(this.owner.Position, 384) && (this.owner == Game1.player || !LocalMultiplayer.IsLocalMultiplayer(true)))
          Game1.playSound(cueName);
        Game1.stats.takeStep();
      }
      else
      {
        if ((this.currentSingleAnimation < 0 || this.currentSingleAnimation > 24) && (this.currentSingleAnimation < 96 || this.currentSingleAnimation > 120))
          return;
        if (this.owner.onBridge.Value && this.currentAnimationIndex % 2 == 0)
        {
          if (this.owner.currentLocation == Game1.currentLocation && Utility.isOnScreen(this.owner.Position, 384) && (this.owner == Game1.player || !LocalMultiplayer.IsLocalMultiplayer(true)))
            Game1.playSound("thudStep");
          if (this.owner.bridge != null)
            this.owner.bridge.OnFootstep(this.owner.Position);
        }
        if (this.currentAnimationIndex != 0)
          return;
        Game1.stats.takeStep();
      }
    }

    private void animateBackwardsOnce(GameTime time)
    {
      this.timer += (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.timer > (double) this.currentSingleAnimationInterval)
      {
        --this.CurrentFrame;
        this.timer = 0.0f;
        if (this.currentAnimationIndex > this.currentAnimationFrames - 1)
        {
          if (this.CurrentFrame < 63 || this.CurrentFrame > 96)
            this.CurrentFrame = this.oldFrame;
          else
            this.CurrentFrame = this.CurrentFrame % 16 + 8;
          this.interval = this.oldInterval;
          this.PauseForSingleAnimation = false;
          this.animatingBackwards = false;
          if (!Game1.eventUp)
            this.owner.CanMove = true;
          if (this.owner.CurrentTool != null && this.owner.CurrentTool.Name.Equals("Fishing Rod"))
            this.owner.UsingTool = false;
          this.owner.Halt();
          if (this.CurrentSingleAnimation >= 160 && this.CurrentSingleAnimation < 192 || this.CurrentSingleAnimation >= 200 && this.CurrentSingleAnimation < 216 || this.CurrentSingleAnimation >= 232 && this.CurrentSingleAnimation < 264)
            Game1.toolAnimationDone(this.owner);
        }
        if (this.owner.UsingTool && this.owner.CurrentTool != null && this.owner.CurrentTool.Name.Equals("Fishing Rod"))
        {
          switch (this.CurrentFrame)
          {
            case 164:
              this.owner.CurrentTool.Update(2, 0, this.owner);
              break;
            case 168:
              this.owner.CurrentTool.Update(1, 0, this.owner);
              break;
            case 180:
              this.owner.CurrentTool.Update(0, 0, this.owner);
              break;
            case 184:
              this.owner.CurrentTool.Update(3, 0, this.owner);
              break;
          }
        }
      }
      this.UpdateSourceRect();
    }

    public int frameOfCurrentSingleAnimation()
    {
      if (this.PauseForSingleAnimation)
        return this.CurrentFrame - (this.currentSingleAnimation - this.currentSingleAnimation % 8);
      if (!Game1.pickingTool && this.owner.CurrentTool != null && this.owner.CurrentTool.Name.Equals("Watering Can"))
        return 4;
      return !Game1.pickingTool && this.owner.UsingTool && (this.currentToolIndex >= 48 && this.currentToolIndex <= 55 || this.owner.CurrentTool != null && this.owner.CurrentTool.Name.Equals("Fishing Rod")) ? 6 : 0;
    }

    public void setCurrentSingleAnimation(int which)
    {
      this.CurrentFrame = which;
      this.currentSingleAnimation = which;
      FarmerSprite.getAnimationFromIndex(which, this, 100, 1, false, false);
      if (this.CurrentAnimation != null && this.CurrentAnimation.Count > 0)
      {
        this.currentAnimationFrames = this.CurrentAnimation.Count;
        this.interval = (float) this.CurrentAnimation.First<FarmerSprite.AnimationFrame>().milliseconds;
        this.CurrentFrame = this.CurrentAnimation.First<FarmerSprite.AnimationFrame>().frame;
      }
      if ((double) this.interval <= 50.0)
        this.interval = 800f;
      this.UpdateSourceRect();
    }

    private void animate(int Milliseconds, int firstFrame, int lastFrame)
    {
      if (this.CurrentFrame > lastFrame || this.CurrentFrame < firstFrame)
        this.CurrentFrame = firstFrame;
      this.timer += (float) Milliseconds;
      if ((double) this.timer > (double) this.interval * (double) this.intervalModifier)
      {
        ++this.CurrentFrame;
        this.timer = 0.0f;
        if (this.CurrentFrame > lastFrame)
          this.CurrentFrame = firstFrame;
        this.checkForFootstep();
      }
      this.UpdateSourceRect();
    }

    private void animate(int Milliseconds)
    {
      this.timer += (float) Milliseconds;
      if ((double) this.timer > (double) this.interval * (double) this.intervalModifier)
      {
        this.currentAnimationTick();
        this.timer = 0.0f;
        this.checkForFootstep();
      }
      this.UpdateSourceRect();
    }

    public override void StopAnimation()
    {
      bool flag = false;
      if (this.pauseForSingleAnimation)
        return;
      this.interval = 0.0f;
      if (this.CurrentFrame >= 64 && this.CurrentFrame <= 155 && this.owner != null && !(bool) (NetFieldBase<bool, NetBool>) this.owner.bathingClothes)
      {
        switch (this.owner.FacingDirection)
        {
          case 0:
            this.CurrentFrame = 12;
            break;
          case 1:
            this.CurrentFrame = 6;
            break;
          case 2:
            this.CurrentFrame = 0;
            break;
          case 3:
            this.CurrentFrame = 6;
            break;
        }
        flag = true;
      }
      else if (!Game1.pickingTool && this.owner != null && !this.IsPlayingBasicAnimation((int) this.owner.facingDirection, this.owner.ActiveObject != null && Game1.eventUp))
      {
        flag = true;
        switch (this.owner.FacingDirection)
        {
          case 0:
            if (this.owner.ActiveObject != null && !Game1.eventUp)
            {
              this.setCurrentFrame(112, 1);
              break;
            }
            this.setCurrentFrame(16, 1);
            break;
          case 1:
            if (this.owner.ActiveObject != null && !Game1.eventUp)
            {
              this.setCurrentFrame(104, 1);
              break;
            }
            this.setCurrentFrame(8, 1);
            break;
          case 2:
            if (this.owner.ActiveObject != null && !Game1.eventUp)
            {
              this.setCurrentFrame(96, 1);
              break;
            }
            this.setCurrentFrame(0, 1);
            break;
          case 3:
            if (this.owner.ActiveObject != null && !Game1.eventUp)
            {
              this.setCurrentFrame(120, 1);
              break;
            }
            this.setCurrentFrame(24, 1);
            break;
        }
        this.currentSingleAnimation = -1;
      }
      if (!flag)
        return;
      this.currentAnimationIndex = 0;
      this.UpdateSourceRect();
    }

    public static void getAnimationFromIndex(
      int index,
      FarmerSprite requester,
      int interval,
      int numberOfFrames,
      bool flip,
      bool secondaryArm)
    {
      bool secondaryArm1 = index >= 96 && index < 160 || index == 232 || index == 248;
      if (requester.owner != null && requester.owner.ActiveObject != null && requester.owner.ActiveObject is Furniture)
        secondaryArm1 = false;
      requester.loopThisAnimation = true;
      int frame = 0;
      if (requester.owner != null && (bool) (NetFieldBase<bool, NetBool>) requester.owner.bathingClothes)
        frame += 108;
      List<FarmerSprite.AnimationFrame> currentAnimation = requester.currentAnimation;
      currentAnimation.Clear();
      float num = 1f;
      if (requester.owner != null && requester.owner.CurrentTool != null)
        num = requester.owner.CurrentTool.AnimationSpeedModifier;
      requester.currentSingleAnimation = index;
      switch (index)
      {
        case -1:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 100, secondaryArm1, false));
          return;
        case 0:
        case 96:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(1 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(2 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(frame, 200, secondaryArm1, false));
          return;
        case 8:
        case 104:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(7 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(8 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6 + frame, 200, secondaryArm1, false));
          return;
        case 16:
        case 112:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(13 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(12 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(14 + frame, 200, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(12 + frame, 200, secondaryArm1, false));
          return;
        case 24:
        case 120:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(7 + frame, 200, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6 + frame, 200, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(8 + frame, 200, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6 + frame, 200, secondaryArm1, true));
          return;
        case 32:
        case 128:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 90, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(1, 60, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(18, 120, -4, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(1, 60, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 90, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(2, 60, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(19, 120, -4, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(2, 60, -2, secondaryArm1, false));
          return;
        case 40:
        case 136:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 80, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 10, -1, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(20, 140, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(11, 100, 0, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 80, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 10, -1, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(21, 140, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(17, 100, 0, secondaryArm1, false));
          return;
        case 43:
          flip = requester.owner.FacingDirection == 3;
          break;
        case 48:
        case 144:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(12, 90, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(13, 60, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(22, 120, -3, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(13, 60, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(12, 90, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(14, 60, -2, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(23, 120, -3, secondaryArm1, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(14, 60, -2, secondaryArm1, false));
          return;
        case 56:
        case 152:
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 80, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 10, -1, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(20, 140, -2, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(11, 100, 0, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 80, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 10, -1, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(21, 140, -2, secondaryArm1, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(17, 100, 0, secondaryArm1, true));
          return;
        case 64:
        case 71:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 0, false, false));
          return;
        case 72:
        case 79:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 0, false, false));
          return;
        case 80:
        case 87:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(12, 0, false, false));
          return;
        case 83:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 0, false, false));
          return;
        case 88:
        case 95:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(6, 0, false, true));
          return;
        case 97:
          requester.loopThisAnimation = false;
          flip = requester.owner.FacingDirection == 3;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(97, 800, false, flip));
          return;
        case 160:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(66, (int) (150.0 * (double) num), false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(67, (int) (40.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(68, (int) (40.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(69, (int) (short) ((double) (170 + requester.owner.toolPower * 30) * (double) num), false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(70, (int) (75.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 164:
        case 166:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(54, 0, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(54, 75, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(55, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(25, 500, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 168:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(48, (int) (100.0 * (double) num), false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(49, (int) (40.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(50, (int) (40.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(51, (int) (short) ((double) (220 + requester.owner.toolPower * 30) * (double) num), false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(52, (int) (75.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 172:
        case 174:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 0, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 75, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(45, 500, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 176:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(36, (int) (100.0 * (double) num), false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(37, (int) (40.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(38, (int) (40.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(63, (int) (short) ((double) (220 + requester.owner.toolPower * 30) * (double) num), false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, (int) (75.0 * (double) num), false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 180:
        case 182:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, 0, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, 75, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(63, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(46, 500, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 184:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(48, (int) (100.0 * (double) num), false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(49, (int) (40.0 * (double) num), false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(50, (int) (40.0 * (double) num), false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(51, (int) (short) ((double) (220 + requester.owner.toolPower * 30) * (double) num), false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(52, (int) (75.0 * (double) num), false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 188:
        case 190:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 0, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 75, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 100, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(45, 500, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 192:
          index = 3;
          interval = 500;
          break;
        case 194:
          index = 9;
          interval = 500;
          break;
        case 196:
          index = 15;
          interval = 500;
          break;
        case 198:
          index = 9;
          flip = true;
          interval = 500;
          break;
        case 216:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 0));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(84, requester.owner.mostRecentlyGrabbedItem == null || !(requester.owner.mostRecentlyGrabbedItem is Object) || (requester.owner.mostRecentlyGrabbedItem as Object).ParentSheetIndex != 434 ? 250 : 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showEatingItem)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(85, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showEatingItem)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(86, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showEatingItem), true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(86, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showEatingItem), true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(87, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(88, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(87, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(88, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(87, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showEatingItem)));
          return;
        case 224:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(104, 350, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(105, 350, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(104, 350, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(105, 350, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(104, 350, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(105, 350, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(104, 350, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(105, 350, false, false));
          return;
        case 232:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(24, 55, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(25, 45, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(26, 25, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(27, 25, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(28, 25, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(29, (int) (short) interval * 2, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(29, 0, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 234:
          index = 28;
          secondaryArm = true;
          break;
        case 240:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(30, 55, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(31, 45, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(32, 25, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(33, 25, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(34, 25, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(35, (int) (short) interval * 2, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(35, 0, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 242:
        case 243:
          index = 34;
          break;
        case 248:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(36, 55, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(37, 45, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(38, 25, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(39, 25, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(40, 25, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(41, (int) (short) interval * 2, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(41, 0, secondaryArm1, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 252:
          index = 40;
          secondaryArm = true;
          break;
        case 256:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(30, 55, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(31, 45, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(32, 25, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(33, 25, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(34, 25, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(35, (int) (short) interval * 2, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(35, 0, true, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 258:
        case 259:
          index = 34;
          flip = true;
          break;
        case 272:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(25, (int) (short) interval, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(27, (int) (short) interval, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(27, 0, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 274:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(34, (int) (short) interval, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(33, (int) (short) interval, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(33, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 276:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(40, (int) (short) interval, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(38, (int) (short) interval, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(38, 0, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 278:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(34, (int) (short) interval, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(33, (int) (short) interval, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showSwordSwipe)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(33, 0, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true));
          return;
        case 279:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, 0, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(63, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(64, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(65, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(65, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          return;
        case 280:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 0, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(60, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(61, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(61, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          return;
        case 281:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(54, 0, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(54, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(55, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(56, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(57, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(57, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          return;
        case 282:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 0, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 100, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 100, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(60, 100, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(61, 200, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(61, 0, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake)));
          return;
        case 283:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(82, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(83, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Shears.playSnip)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(82, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(83, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 284:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(80, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(81, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Shears.playSnip)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(80, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(81, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 285:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(78, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(79, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Shears.playSnip)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(78, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(79, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 286:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(80, 400, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(81, 400, false, true, new AnimatedSprite.endOfAnimationBehavior(Shears.playSnip)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(80, 400, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(81, 400, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 287:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(63, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(63, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 288:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 289:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(54, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(55, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(54, 400));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(55, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 290:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 400, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 400, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(58, 400, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(59, 400, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 291:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(16, 1500));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(16, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.completelyStopAnimating)));
          return;
        case 292:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(16, 500));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 500));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(16, 500));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 500));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.completelyStopAnimating)));
          return;
        case 293:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(16, 1000));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 500));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(16, 1000));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(4, 200));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(5, 2000, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.doSleepEmote)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(5, 2000, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.passOutFromTired)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(5, 2000));
          return;
        case 294:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(0, 1));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(90, 250));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(91, 150));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(92, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(93, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.drinkGlug)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(92, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(93, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.drinkGlug)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(92, 250, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(93, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.drinkGlug)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(91, 250));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(90, 50));
          return;
        case 295:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(76, 100, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(38, 40, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(63, 40, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(62, 80, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(63, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(FishingRod.doneWithCastingAnimation), true));
          return;
        case 296:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(48, 100, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(49, 40, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(50, 40, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(51, 80, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(52, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(FishingRod.doneWithCastingAnimation), true));
          return;
        case 297:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(66, 100, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(67, 40, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(68, 40, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(69, 80, false, false));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(70, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(FishingRod.doneWithCastingAnimation), true));
          return;
        case 298:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(48, 100, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(49, 40, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(50, 40, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(51, 80, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(52, 200, false, true, new AnimatedSprite.endOfAnimationBehavior(FishingRod.doneWithCastingAnimation), true));
          return;
        case 299:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(76, 5000, false, false));
          return;
        case 300:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(72, 5000, false, false));
          return;
        case 301:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(74, 5000, false, false));
          return;
        case 302:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(72, 5000, false, true));
          return;
        case 303:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(124, 150, false, true, new AnimatedSprite.endOfAnimationBehavior(Pan.playSlosh)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(125, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(124, 150, false, true, new AnimatedSprite.endOfAnimationBehavior(Pan.playSlosh)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(125, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(124, 150, false, true, new AnimatedSprite.endOfAnimationBehavior(Pan.playSlosh)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(125, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 150, false, true));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(124, 150, false, true, new AnimatedSprite.endOfAnimationBehavior(Pan.playSlosh)));
          currentAnimation.Add(new FarmerSprite.AnimationFrame(123, 500, false, true, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true));
          return;
        case 304:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(84, 99999999, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showEatingItem)));
          return;
        case 999996:
          requester.loopThisAnimation = false;
          currentAnimation.Add(new FarmerSprite.AnimationFrame(96, 800, false, false));
          return;
      }
      if (index > FarmerRenderer.featureYOffsetPerFrame.Length - 1)
        index = 0;
      requester.loopThisAnimation = false;
      for (int index1 = 0; index1 < numberOfFrames; ++index1)
        currentAnimation.Add(new FarmerSprite.AnimationFrame((int) (short) (index1 + index), (int) (short) interval, secondaryArm, flip));
    }

    public struct AnimationFrame
    {
      public int frame;
      public int milliseconds;
      public int positionOffset;
      public int xOffset;
      public bool secondaryArm;
      public bool flip;
      public AnimatedSprite.endOfAnimationBehavior frameStartBehavior;
      public AnimatedSprite.endOfAnimationBehavior frameEndBehavior;

      public AnimationFrame(
        int frame,
        int milliseconds,
        int position_offset,
        bool secondary_arm,
        bool flip,
        AnimatedSprite.endOfAnimationBehavior frame_start_behavior,
        AnimatedSprite.endOfAnimationBehavior frame_end_behavior,
        int x_offset)
      {
        this.frame = frame;
        this.milliseconds = milliseconds;
        this.positionOffset = position_offset;
        this.secondaryArm = secondary_arm;
        this.flip = flip;
        this.frameStartBehavior = frame_start_behavior;
        this.frameEndBehavior = frame_end_behavior;
        this.xOffset = x_offset;
      }

      public AnimationFrame(
        int frame,
        int milliseconds,
        int positionOffset,
        bool secondaryArm,
        bool flip,
        AnimatedSprite.endOfAnimationBehavior frameBehavior = null,
        bool behaviorAtEndOfFrame = false,
        int xOffset = 0)
        : this(frame, milliseconds, positionOffset, secondaryArm, flip, (AnimatedSprite.endOfAnimationBehavior) null, (AnimatedSprite.endOfAnimationBehavior) null, xOffset)
      {
        if (!behaviorAtEndOfFrame)
          this.frameStartBehavior = frameBehavior;
        else
          this.frameEndBehavior = frameBehavior;
      }

      public AnimationFrame(
        int frame,
        int milliseconds,
        bool secondaryArm,
        bool flip,
        AnimatedSprite.endOfAnimationBehavior frameBehavior = null,
        bool behaviorAtEndOfFrame = false)
        : this(frame, milliseconds, 0, secondaryArm, flip, frameBehavior, behaviorAtEndOfFrame)
      {
      }

      public AnimationFrame(int frame, int milliseconds)
        : this(frame, milliseconds, false, false)
      {
      }

      public FarmerSprite.AnimationFrame AddFrameAction(
        AnimatedSprite.endOfAnimationBehavior callback)
      {
        this.frameStartBehavior += callback;
        return this;
      }

      public FarmerSprite.AnimationFrame AddFrameEndAction(
        AnimatedSprite.endOfAnimationBehavior callback)
      {
        this.frameEndBehavior += callback;
        return this;
      }
    }
  }
}
