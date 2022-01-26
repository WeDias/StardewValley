// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Dog
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;

namespace StardewValley.Characters
{
  public class Dog : Pet
  {
    public const int behavior_SitSide = 50;
    public const int behavior_Sprint = 51;
    public const int behavior_StandUp = 54;
    public const int behavior_StandUpRight = 55;
    protected int sprintTimer;
    private bool wagging;

    public Dog()
    {
      this.Sprite = new AnimatedSprite(this.getPetTextureName(), 0, 32, 32);
      this.HideShadow = true;
      this.Breather = false;
      this.willDestroyObjectsUnderfoot = false;
    }

    public override void OnPetAnimationEvent(string animation_event)
    {
      if (this.CurrentBehavior == 1)
        return;
      if (animation_event == "bark")
      {
        if (Utility.isOnScreen(this.getTileLocationPoint(), 640, this.currentLocation))
        {
          if (!Game1.options.muteAnimalSounds)
            Game1.playSound("dog_bark");
          this.shake(500);
        }
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(26, 500, false, false),
          new FarmerSprite.AnimationFrame(23, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold))
        });
      }
      else if (animation_event == "close_eyes")
      {
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(27, Game1.random.NextDouble() < 0.3 ? 500 : Game1.random.Next(2000, 15000)),
          new FarmerSprite.AnimationFrame(18, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold))
        });
        this.Sprite.loop = false;
      }
      else if (animation_event == "sit_animation")
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(23, Game1.random.Next(2000, 6000), false, false),
          new FarmerSprite.AnimationFrame(23, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold))
        });
      else if (animation_event == "wag")
      {
        this.wag();
      }
      else
      {
        if (!(animation_event == "pant"))
          return;
        if (this.CurrentBehavior == 50)
        {
          List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(24, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(this.pantSound)),
            new FarmerSprite.AnimationFrame(25, 200, false, false)
          };
          int num = Game1.random.Next(5, 15);
          for (int index = 0; index < num; ++index)
          {
            animation.Add(new FarmerSprite.AnimationFrame(24, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(this.pantSound)));
            animation.Add(new FarmerSprite.AnimationFrame(25, 200, false, false));
          }
          this.Sprite.setCurrentAnimation(animation);
          this.Sprite.loop = false;
        }
        else
        {
          if (this.CurrentBehavior != 2)
            return;
          List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(18, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(this.pantSound)),
            new FarmerSprite.AnimationFrame(19, 200)
          };
          int num = Game1.random.Next(7, 20);
          for (int index = 0; index < num; ++index)
          {
            animation.Add(new FarmerSprite.AnimationFrame(18, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(this.pantSound)));
            animation.Add(new FarmerSprite.AnimationFrame(19, 200));
          }
          this.Sprite.setCurrentAnimation(animation);
          this.Sprite.loop = false;
        }
      }
    }

    public Dog(int xTile, int yTile, int breed)
    {
      this.Name = nameof (Dog);
      this.displayName = (string) (NetFieldBase<string, NetString>) this.name;
      this.whichBreed.Value = breed;
      this.Sprite = new AnimatedSprite(this.getPetTextureName(), 0, 32, 32);
      this.Position = new Vector2((float) xTile, (float) yTile) * 64f;
      this.Breather = false;
      this.willDestroyObjectsUnderfoot = false;
      this.currentLocation = Game1.currentLocation;
      this.HideShadow = true;
    }

    public override string getPetTextureName() => "Animals\\dog" + (this.whichBreed.Value == 0 ? "" : this.whichBreed.Value.ToString() ?? "");

    public override void dayUpdate(int dayOfMonth)
    {
      base.dayUpdate(dayOfMonth);
      this.sprintTimer = 0;
    }

    public override void RunState(GameTime time)
    {
      base.RunState(time);
      if (this.Sprite.CurrentAnimation == null)
        this.wagging = false;
      if (this.CurrentBehavior == 51)
      {
        if (!Game1.IsMasterGame || this.sprintTimer <= 0)
          return;
        this.sprintTimer -= time.ElapsedGameTime.Milliseconds;
        this.speed = 6;
        this.tryToMoveInDirection(this.FacingDirection, false, -1, false);
        if (this.sprintTimer > 0)
          return;
        this.speed = 2;
        this.CurrentBehavior = 0;
      }
      else if (this.CurrentBehavior == 55 || this.CurrentBehavior == 54)
      {
        if (this.Sprite.CurrentAnimation != null)
          return;
        this.CurrentBehavior = 0;
      }
      else if (this.CurrentBehavior == 1)
      {
        if (Game1.IsMasterGame && Game1.timeOfDay < 2000 && Game1.random.NextDouble() < 0.001)
          this.CurrentBehavior = 0;
        if (Game1.random.NextDouble() >= 0.002)
          return;
        this.doEmote(24);
      }
      else if (this.CurrentBehavior == 50)
      {
        if (this.withinPlayerThreshold(2))
        {
          if (this.wagging)
            return;
          this.wag();
        }
        else if (this.Sprite.CurrentFrame != 23 && this.Sprite.CurrentAnimation == null)
        {
          this.Sprite.CurrentFrame = 23;
        }
        else
        {
          if (this.Sprite.CurrentFrame != 23 || !Game1.IsMasterGame || Game1.random.NextDouble() >= 0.01)
            return;
          switch (Game1.random.Next(7))
          {
            case 0:
              this.CurrentBehavior = 55;
              break;
            case 1:
              this.petAnimationEvent.Fire("bark");
              break;
            case 2:
              this.petAnimationEvent.Fire("wag");
              break;
            case 3:
            case 4:
              this.petAnimationEvent.Fire("sit_animation");
              break;
            default:
              this.petAnimationEvent.Fire("pant");
              break;
          }
        }
      }
      else if (this.CurrentBehavior == 2)
      {
        if (this.Sprite.currentFrame != 18 && this.Sprite.CurrentAnimation == null)
        {
          this.Sprite.currentFrame = 18;
        }
        else
        {
          if (this.Sprite.currentFrame != 18 || !Game1.IsMasterGame || Game1.random.NextDouble() >= 0.01)
            return;
          switch (Game1.random.Next(4))
          {
            case 0:
            case 1:
              this.faceDirection(2);
              this.CurrentBehavior = 54;
              break;
            case 2:
              this.petAnimationEvent.Fire("pant");
              break;
            case 3:
              this.petAnimationEvent.Fire("close_eyes");
              break;
          }
        }
      }
      else
      {
        if (this.CurrentBehavior != 0 || !Game1.IsMasterGame || this.Sprite.CurrentAnimation != null || Game1.random.NextDouble() >= 0.01 || this.forceUpdateTimer > 0)
          return;
        switch (Game1.random.Next(7 + (this.currentLocation is Farm ? 1 : 0)))
        {
          case 4:
          case 5:
            switch (this.FacingDirection)
            {
              case 0:
              case 1:
              case 3:
                if (this.FacingDirection == 0)
                  this.FacingDirection = Game1.random.NextDouble() < 0.5 ? 3 : 1;
                this.faceDirection(this.FacingDirection);
                this.CurrentBehavior = 50;
                return;
              case 2:
                this.faceDirection(2);
                this.CurrentBehavior = 2;
                return;
              default:
                return;
            }
          case 6:
          case 7:
            this.Halt();
            this.CurrentBehavior = 51;
            break;
        }
      }
    }

    public override void OnNewBehavior()
    {
      base.OnNewBehavior();
      this.sprintTimer = 0;
      switch (this.CurrentBehavior)
      {
        case 2:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(16, 100, false, false),
            new FarmerSprite.AnimationFrame(17, 100, false, false),
            new FarmerSprite.AnimationFrame(18, 100, false, false),
            new FarmerSprite.AnimationFrame(19, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold))
          });
          break;
        case 50:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(20, 100, false, false),
            new FarmerSprite.AnimationFrame(21, 100, false, false),
            new FarmerSprite.AnimationFrame(22, 100, false, false),
            new FarmerSprite.AnimationFrame(23, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold))
          });
          break;
        case 51:
          if (Game1.IsMasterGame)
          {
            this.faceDirection(Game1.random.NextDouble() < 0.5 ? 3 : 1);
            this.sprintTimer = Game1.random.Next(1000, 3500);
          }
          if (Utility.isOnScreen(this.getTileLocationPoint(), 64, this.currentLocation) && !Game1.options.muteAnimalSounds)
            Game1.playSound("dog_bark");
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(32, 100, false, false),
            new FarmerSprite.AnimationFrame(33, 100, false, false),
            new FarmerSprite.AnimationFrame(34, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(this.hitGround)),
            new FarmerSprite.AnimationFrame(33, 100, false, false)
          });
          this.Sprite.loop = true;
          break;
        case 54:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(17, 200),
            new FarmerSprite.AnimationFrame(16, 200),
            new FarmerSprite.AnimationFrame(0, 200)
          });
          this.Sprite.loop = false;
          break;
        case 55:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(23, 100, false, false),
            new FarmerSprite.AnimationFrame(22, 100, false, false),
            new FarmerSprite.AnimationFrame(21, 100, false, false),
            new FarmerSprite.AnimationFrame(20, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold))
          });
          this.Sprite.loop = false;
          break;
      }
    }

    public void wag()
    {
      int milliseconds = this.withinPlayerThreshold(2) ? 120 : 200;
      this.wagging = true;
      this.Sprite.loop = false;
      List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(31, milliseconds, false, false),
        new FarmerSprite.AnimationFrame(23, milliseconds, false, false, new AnimatedSprite.endOfAnimationBehavior(this.hitGround))
      };
      int num = Game1.random.Next(2, 6);
      for (int index = 0; index < num; ++index)
      {
        animation.Add(new FarmerSprite.AnimationFrame(31, milliseconds, false, false));
        animation.Add(new FarmerSprite.AnimationFrame(23, milliseconds, false, false, new AnimatedSprite.endOfAnimationBehavior(this.hitGround)));
      }
      animation.Add(new FarmerSprite.AnimationFrame(23, 2, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWagging)));
      this.Sprite.setCurrentAnimation(animation);
    }

    public void doneWagging(Farmer who) => this.wagging = false;

    public void hitGround(Farmer who)
    {
      if (!Utility.isOnScreen(this.getTileLocationPoint(), 128, this.currentLocation))
        return;
      this.currentLocation.playTerrainSound(this.getTileLocation(), (Character) this, false);
    }

    public void pantSound(Farmer who)
    {
      if (!this.withinPlayerThreshold(5) || Game1.options.muteAnimalSounds)
        return;
      this.currentLocation.localSound("dog_pant");
    }

    public void thumpSound(Farmer who)
    {
      if (!this.withinPlayerThreshold(4))
        return;
      this.currentLocation.localSound("thudStep");
    }

    public override void playContentSound()
    {
      if (!Utility.isOnScreen(this.getTileLocationPoint(), 128, this.currentLocation) || Game1.options.muteAnimalSounds)
        return;
      Game1.playSound("dog_pant");
      DelayedAction.playSoundAfterDelay("dog_pant", 400);
    }
  }
}
