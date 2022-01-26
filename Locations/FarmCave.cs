// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.FarmCave
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;

namespace StardewValley.Locations
{
  public class FarmCave : GameLocation
  {
    public FarmCave()
    {
    }

    public FarmCave(string map, string name)
      : base(map, name)
    {
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if ((int) (NetFieldBase<int, NetInt>) Game1.MasterPlayer.caveChoice != 1)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(0.0f, 0.0f), false, 0.0f, Color.White)
      {
        interval = 3000f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        layerDepth = 1f,
        light = true,
        lightRadius = 0.5f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(8f, 0.0f), false, 0.0f, Color.White)
      {
        interval = 3000f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        layerDepth = 1f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(320f, -64f), false, 0.0f, Color.White)
      {
        interval = 2000f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 500,
        layerDepth = 1f,
        light = true,
        lightRadius = 0.5f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(328f, -64f), false, 0.0f, Color.White)
      {
        interval = 2000f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 500,
        layerDepth = 1f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(128f, (float) (this.map.Layers[0].LayerHeight * 64 - 64)), false, 0.0f, Color.White)
      {
        interval = 1600f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 250,
        layerDepth = 1f,
        light = true,
        lightRadius = 0.5f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(136f, (float) (this.map.Layers[0].LayerHeight * 64 - 64)), false, 0.0f, Color.White)
      {
        interval = 1600f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 250,
        layerDepth = 1f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 + 4), 192f), false, 0.0f, Color.White)
      {
        interval = 2800f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 750,
        layerDepth = 1f,
        light = true,
        lightRadius = 0.5f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 + 12), 192f), false, 0.0f, Color.White)
      {
        interval = 2800f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 750,
        layerDepth = 1f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 + 4), 576f), false, 0.0f, Color.White)
      {
        interval = 2200f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 750,
        layerDepth = 1f,
        light = true,
        lightRadius = 0.5f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 + 12), 576f), false, 0.0f, Color.White)
      {
        interval = 2200f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 750,
        layerDepth = 1f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-60f, 128f), false, 0.0f, Color.White)
      {
        interval = 2600f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 750,
        layerDepth = 1f,
        light = true,
        lightRadius = 0.5f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-52f, 128f), false, 0.0f, Color.White)
      {
        interval = 2600f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 750,
        layerDepth = 1f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-64f, 384f), false, 0.0f, Color.White)
      {
        interval = 3400f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 650,
        layerDepth = 1f,
        light = true,
        lightRadius = 0.5f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-52f, 384f), false, 0.0f, Color.White)
      {
        interval = 3400f,
        animationLength = 3,
        totalNumberOfLoops = 99999,
        scale = 4f,
        delayBeforeAnimationStart = 650,
        layerDepth = 1f
      });
      Game1.ambientLight = new Color(70, 90, 0);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if ((int) (NetFieldBase<int, NetInt>) Game1.MasterPlayer.caveChoice == 1 && Game1.random.NextDouble() < 0.002 && Game1.currentLocation == this)
      {
        this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(640, 1664, 16, 16), 80f, 4, 9999, new Vector2((float) Game1.random.Next(this.map.Layers[0].LayerWidth), (float) this.map.Layers[0].LayerHeight) * 64f, false, false, 1f, 0.0f, Color.Black, 4f, 0.0f, 0.0f, 0.0f)
        {
          xPeriodic = true,
          xPeriodicLoopTime = 2000f,
          xPeriodicRange = 64f,
          motion = new Vector2(0.0f, -8f)
        });
        if (Game1.random.NextDouble() < 0.15 && Game1.currentLocation == this)
          this.localSound("batScreech");
        for (int index = 1; index < 5; ++index)
          DelayedAction.playSoundAfterDelay("batFlap", 320 * index - 80);
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) Game1.MasterPlayer.caveChoice != 1 || Game1.random.NextDouble() >= 0.005)
          return;
        this.temporarySprites.Add((TemporaryAnimatedSprite) new BatTemporarySprite(new Vector2(Game1.random.NextDouble() < 0.5 ? 0.0f : (float) (this.map.DisplayWidth - 64), (float) (this.map.DisplayHeight - 64))));
      }
    }

    public override void checkForMusic(GameTime time)
    {
    }

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      if (Game1.currentLocation == this)
        this.UpdateReadyFlag();
      base.performTenMinuteUpdate(timeOfDay);
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      this.UpdateReadyFlag();
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      if ((int) (NetFieldBase<int, NetInt>) Game1.MasterPlayer.caveChoice == 1)
      {
        while (Game1.random.NextDouble() < 0.66)
        {
          int parentSheetIndex = 410;
          switch (Game1.random.Next(5))
          {
            case 0:
              parentSheetIndex = 296;
              break;
            case 1:
              parentSheetIndex = 396;
              break;
            case 2:
              parentSheetIndex = 406;
              break;
            case 3:
              parentSheetIndex = 410;
              break;
            case 4:
              parentSheetIndex = Game1.random.NextDouble() < 0.1 ? 613 : Game1.random.Next(634, 639);
              break;
          }
          Vector2 v = new Vector2((float) Game1.random.Next(1, this.map.Layers[0].LayerWidth - 1), (float) Game1.random.Next(1, this.map.Layers[0].LayerHeight - 4));
          if (this.isTileLocationTotallyClearAndPlaceable(v))
            this.setObject(v, new Object(parentSheetIndex, 1)
            {
              IsSpawnedObject = true
            });
        }
      }
      this.UpdateReadyFlag();
    }

    public virtual void UpdateReadyFlag()
    {
      bool flag = false;
      foreach (Object @object in this.objects.Values)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) @object.isSpawnedObject)
        {
          flag = true;
          break;
        }
        if ((bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable && @object.heldObject.Value != null && (int) (NetFieldBase<int, NetIntDelta>) @object.minutesUntilReady <= 0 && @object.ParentSheetIndex == 128)
        {
          flag = true;
          break;
        }
      }
      Game1.getFarm().farmCaveReady.Value = flag;
    }

    public void setUpMushroomHouse()
    {
      this.setObject(new Vector2(4f, 5f), new Object(new Vector2(4f, 5f), 128));
      this.setObject(new Vector2(6f, 5f), new Object(new Vector2(6f, 5f), 128));
      this.setObject(new Vector2(8f, 5f), new Object(new Vector2(8f, 5f), 128));
      this.setObject(new Vector2(4f, 7f), new Object(new Vector2(4f, 7f), 128));
      this.setObject(new Vector2(6f, 7f), new Object(new Vector2(6f, 7f), 128));
      this.setObject(new Vector2(8f, 7f), new Object(new Vector2(8f, 7f), 128));
    }
  }
}
