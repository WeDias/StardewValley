// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Rabbit
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Rabbit : Critter
  {
    private int characterCheckTimer = 200;
    private bool running;

    public Rabbit(Vector2 position, bool flip)
    {
      this.position = position * 64f;
      position.Y += 48f;
      this.flip = flip;
      this.baseFrame = Game1.currentSeason.Equals("winter") ? 74 : 54;
      this.sprite = new AnimatedSprite(Critter.critterTexture, Game1.currentSeason.Equals("winter") ? 69 : 68, 32, 32);
      this.sprite.loop = true;
      this.startingPosition = position;
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      this.characterCheckTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.characterCheckTimer <= 0 && !this.running)
      {
        if (Utility.isOnScreen(this.position, -32))
        {
          this.running = true;
          this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(this.baseFrame, 40),
            new FarmerSprite.AnimationFrame(this.baseFrame + 1, 40),
            new FarmerSprite.AnimationFrame(this.baseFrame + 2, 40),
            new FarmerSprite.AnimationFrame(this.baseFrame + 3, 100),
            new FarmerSprite.AnimationFrame(this.baseFrame + 5, 70),
            new FarmerSprite.AnimationFrame(this.baseFrame + 5, 40)
          });
          this.sprite.loop = true;
        }
        this.characterCheckTimer = 200;
      }
      if (this.running)
        this.position.X += this.flip ? -6f : 6f;
      if (this.running && this.characterCheckTimer <= 0)
      {
        this.characterCheckTimer = 200;
        if (environment.largeTerrainFeatures != null)
        {
          Rectangle rectangle = new Rectangle((int) this.position.X + 32, (int) this.position.Y - 32, 4, 192);
          foreach (LargeTerrainFeature largeTerrainFeature in environment.largeTerrainFeatures)
          {
            if (largeTerrainFeature is Bush && largeTerrainFeature.getBoundingBox().Intersects(rectangle))
            {
              (largeTerrainFeature as Bush).performUseAction((Vector2) (NetFieldBase<Vector2, NetVector2>) largeTerrainFeature.tilePosition, environment);
              return true;
            }
          }
        }
      }
      return base.update(time, environment);
    }
  }
}
