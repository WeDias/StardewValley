// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Cat
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;

namespace StardewValley.Characters
{
  public class Cat : Pet
  {
    public const int behavior_StandUp = 54;
    public const int behavior_Flop = 55;
    public const int behavior_Leap = 56;

    public Cat()
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
      if (animation_event == "blink")
      {
        bool flag = Game1.random.NextDouble() < 0.45;
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(19, flag ? 200 : Game1.random.Next(1000, 9000)),
          new FarmerSprite.AnimationFrame(18, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold))
        });
        this.Sprite.loop = false;
        if (!flag || Game1.random.NextDouble() >= 0.2)
          return;
        this.playContentSound();
        this.shake(200);
      }
      else
      {
        if (!(animation_event == "lick"))
          return;
        List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(19, 300),
          new FarmerSprite.AnimationFrame(20, 200),
          new FarmerSprite.AnimationFrame(21, 200),
          new FarmerSprite.AnimationFrame(22, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(this.lickSound)),
          new FarmerSprite.AnimationFrame(23, 200)
        };
        int num = Game1.random.Next(1, 6);
        for (int index = 0; index < num; ++index)
        {
          animation.Add(new FarmerSprite.AnimationFrame(21, 150));
          animation.Add(new FarmerSprite.AnimationFrame(22, 150, false, false, new AnimatedSprite.endOfAnimationBehavior(this.lickSound)));
          animation.Add(new FarmerSprite.AnimationFrame(23, 150));
        }
        animation.Add(new FarmerSprite.AnimationFrame(18, 1, false, false, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold)));
        this.Sprite.loop = false;
        this.Sprite.setCurrentAnimation(animation);
      }
    }

    public Cat(int xTile, int yTile, int breed)
    {
      this.Name = nameof (Cat);
      this.displayName = (string) (NetFieldBase<string, NetString>) this.name;
      this.whichBreed.Value = breed;
      this.Sprite = new AnimatedSprite(this.getPetTextureName(), 0, 32, 32);
      this.Position = new Vector2((float) xTile, (float) yTile) * 64f;
      this.Breather = false;
      this.willDestroyObjectsUnderfoot = false;
      this.currentLocation = Game1.currentLocation;
      this.HideShadow = true;
    }

    public override string getPetTextureName() => "Animals\\cat" + (this.whichBreed.Value == 0 ? "" : this.whichBreed.Value.ToString() ?? "");

    public override void OnNewBehavior()
    {
      base.OnNewBehavior();
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
            new FarmerSprite.AnimationFrame(24, 100),
            new FarmerSprite.AnimationFrame(25, 100),
            new FarmerSprite.AnimationFrame(26, 100),
            new FarmerSprite.AnimationFrame(27, Game1.random.Next(8000, 30000), false, false, new AnimatedSprite.endOfAnimationBehavior(this.flopSound))
          });
          this.Sprite.loop = false;
          break;
        case 56:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(30, 300),
            new FarmerSprite.AnimationFrame(31, 300),
            new FarmerSprite.AnimationFrame(30, 300),
            new FarmerSprite.AnimationFrame(31, 300),
            new FarmerSprite.AnimationFrame(30, 300),
            new FarmerSprite.AnimationFrame(31, 500),
            new FarmerSprite.AnimationFrame(24, 800, false, false, new AnimatedSprite.endOfAnimationBehavior(this.leap)),
            new FarmerSprite.AnimationFrame(4, 1)
          });
          this.Sprite.loop = false;
          break;
      }
    }

    public override void RunState(GameTime time)
    {
      base.RunState(time);
      if (this.CurrentBehavior == 1)
      {
        if (Game1.IsMasterGame && Game1.timeOfDay < 2000 && Game1.random.NextDouble() < 0.001)
          this.CurrentBehavior = 0;
        if (Game1.random.NextDouble() < 0.002)
          this.doEmote(24);
      }
      else if (this.CurrentBehavior == 2)
      {
        if (this.Sprite.currentFrame != 18 && this.Sprite.CurrentAnimation == null)
          this.Sprite.currentFrame = 18;
        else if (this.Sprite.currentFrame == 18 && Game1.IsMasterGame && Game1.random.NextDouble() < 0.01)
        {
          switch (Game1.random.Next(6))
          {
            case 0:
            case 1:
              this.faceDirection(2);
              this.CurrentBehavior = 54;
              break;
            case 2:
            case 3:
              this.petAnimationEvent.Fire("lick");
              break;
            default:
              this.petAnimationEvent.Fire("blink");
              break;
          }
        }
      }
      if (this.CurrentBehavior == 54)
      {
        if (this.Sprite.CurrentAnimation != null)
          return;
        this.CurrentBehavior = 0;
      }
      else if (this.CurrentBehavior == 0)
      {
        if (!Game1.IsMasterGame || this.Sprite.CurrentAnimation != null || Game1.random.NextDouble() >= 0.01 || this.forceUpdateTimer > 0)
          return;
        switch (Game1.random.Next(4))
        {
          case 3:
            switch (this.FacingDirection)
            {
              case 0:
              case 2:
                this.faceDirection(2);
                this.CurrentBehavior = 2;
                return;
              case 1:
                if (Game1.random.NextDouble() < 0.85)
                {
                  this.CurrentBehavior = 55;
                  return;
                }
                this.CurrentBehavior = 56;
                return;
              case 3:
                if (Game1.random.NextDouble() < 0.85)
                {
                  this.CurrentBehavior = 55;
                  return;
                }
                this.CurrentBehavior = 56;
                return;
              default:
                return;
            }
        }
      }
      else
      {
        if (this.CurrentBehavior != 55 && this.CurrentBehavior != 56 || !Game1.IsMasterGame)
          return;
        if (this.CurrentBehavior == 56 && this.yJumpOffset != 0)
        {
          if (this.FacingDirection == 1)
            this.xVelocity = 4f;
          else if (this.FacingDirection == 3)
            this.xVelocity = -4f;
          this.MovePosition(time, Game1.viewport, this.currentLocation);
        }
        if (this.Sprite.CurrentAnimation != null)
          return;
        this.CurrentBehavior = 0;
      }
    }

    public void lickSound(Farmer who)
    {
      if (!Utility.isOnScreen(this.getTileLocationPoint(), 128, this.currentLocation))
        return;
      Game1.playSound("Cowboy_Footstep");
    }

    public void leap(Farmer who)
    {
      if (!this.currentLocation.Equals(Game1.currentLocation))
        return;
      this.jump();
    }

    public void flopSound(Farmer who)
    {
      if (Utility.isOnScreen(this.getTileLocationPoint(), 128, this.currentLocation))
        Game1.playSound("thudStep");
      if (Game1.IsMasterGame)
        return;
      this.hold(who);
    }

    public override void playContentSound()
    {
      if (!Utility.isOnScreen(this.getTileLocationPoint(), 128, this.currentLocation) || Game1.options.muteAnimalSounds)
        return;
      Game1.playSound("cat");
    }
  }
}
