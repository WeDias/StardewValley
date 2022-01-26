// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BathHousePool
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;

namespace StardewValley.Locations
{
  public class BathHousePool : GameLocation
  {
    public const float steamZoom = 4f;
    public const float steamYMotionPerMillisecond = 0.1f;
    public const float millisecondsPerSteamFrame = 50f;
    private Texture2D steamAnimation;
    private Texture2D swimShadow;
    private Vector2 steamPosition;
    private float steamYOffset;
    private int swimShadowTimer;
    private int swimShadowFrame;

    public BathHousePool()
    {
    }

    public BathHousePool(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      Game1.changeMusicTrack("pool_ambient");
      this.steamPosition = new Vector2((float) -Game1.viewport.X, (float) -Game1.viewport.Y);
      this.steamAnimation = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\steamAnimation");
      this.swimShadow = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\swimShadow");
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (Game1.player.swimming.Value)
        Game1.player.swimming.Value = false;
      if (Game1.locationRequest != null && !Game1.locationRequest.Name.Contains("BathHouse"))
        Game1.player.bathingClothes.Value = false;
      Game1.changeMusicTrack("none");
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this.currentEvent != null)
      {
        foreach (NPC actor in this.currentEvent.actors)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) actor.swimming)
            b.Draw(this.swimShadow, Game1.GlobalToLocal(Game1.viewport, actor.Position + new Vector2(0.0f, (float) (actor.Sprite.SpriteHeight / 3 * 4 + 4))), new Rectangle?(new Rectangle(this.swimShadowFrame * 16, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
        }
      }
      else
      {
        foreach (NPC character in this.characters)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) character.swimming)
            b.Draw(this.swimShadow, Game1.GlobalToLocal(Game1.viewport, character.Position + new Vector2(0.0f, (float) (character.Sprite.SpriteHeight / 3 * 4 + 4))), new Rectangle?(new Rectangle(this.swimShadowFrame * 16, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
        }
        foreach (Farmer farmer in this.farmers)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) farmer.swimming)
            b.Draw(this.swimShadow, Game1.GlobalToLocal(Game1.viewport, farmer.Position + new Vector2(0.0f, (float) (farmer.Sprite.SpriteHeight / 4 * 4))), new Rectangle?(new Rectangle(this.swimShadowFrame * 16, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
        }
      }
      int num = (bool) (NetFieldBase<bool, NetBool>) Game1.player.swimming ? 1 : 0;
    }

    public override void checkForMusic(GameTime time) => base.checkForMusic(time);

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      for (float x = this.steamPosition.X; (double) x < (double) Game1.graphics.GraphicsDevice.Viewport.Width + 256.0; x += 256f)
      {
        for (float y = this.steamPosition.Y + this.steamYOffset; (double) y < (double) (Game1.graphics.GraphicsDevice.Viewport.Height + 128); y += 256f)
          b.Draw(this.steamAnimation, new Vector2(x, y), new Rectangle?(new Rectangle(0, 0, 64, 64)), Color.White * 0.8f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.steamYOffset -= (float) time.ElapsedGameTime.Milliseconds * 0.1f;
      this.steamYOffset %= -256f;
      this.steamPosition -= Game1.getMostRecentViewportMotion();
      this.swimShadowTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.swimShadowTimer > 0)
        return;
      this.swimShadowTimer = 70;
      ++this.swimShadowFrame;
      this.swimShadowFrame %= 10;
    }
  }
}
