// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.WizardHouse
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley.Locations
{
  public class WizardHouse : GameLocation
  {
    private int cauldronTimer = 250;

    public WizardHouse()
    {
    }

    public WizardHouse(string m, string name)
      : base(m, name)
    {
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this.wasUpdated)
        return;
      base.UpdateWhenCurrentLocation(time);
      this.cauldronTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.cauldronTimer > 0)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(3f, 20f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), (float) Game1.random.Next(16)), false, 1f / 500f, Color.Lime)
      {
        alpha = 0.75f,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2(-1f / 500f, 0.0f),
        interval = 99999f,
        layerDepth = (float) (0.143999993801117 - (double) Game1.random.Next(100) / 10000.0),
        scale = 3f,
        scaleChange = 0.01f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
      });
      this.cauldronTimer = 100;
    }

    public override void cleanupBeforePlayerExit()
    {
      Game1.changeMusicTrack("none");
      base.cleanupBeforePlayerExit();
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (!Game1.player.eventsSeen.Contains(418172))
        return;
      this.setMapTileIndex(2, 12, 2143, "Front", 1);
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(276, 1985, 12, 11), new Vector2(10f, 12f) * 64f + new Vector2(32f, -32f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 2f,
        scale = 4f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(276, 1985, 12, 11), new Vector2(2f, 21f) * 64f + new Vector2(51f, 32f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 1f,
        scale = 2f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(276, 1985, 12, 11), new Vector2(3f, 21f) * 64f + new Vector2(16f, 32f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 1f,
        scale = 3f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(276, 1985, 12, 11), new Vector2(4f, 21f) * 64f + new Vector2(-16f, 32f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 1f,
        scale = 2f
      });
    }
  }
}
