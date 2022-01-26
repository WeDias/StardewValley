// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.HaleyCowPictures
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace StardewValley.Minigames
{
  public class HaleyCowPictures : IMinigame
  {
    private const int pictureWidth = 416;
    private const int pictureHeight = 496;
    private const int sourceWidth = 104;
    private const int sourceHeight = 124;
    private int numberOfPhotosSoFar;
    private int betweenPhotoTimer = 1000;
    private LocalizedContentManager content;
    private Vector2 centerOfScreen;
    private Texture2D pictures;
    private float fadeAlpha;

    public HaleyCowPictures()
    {
      this.content = Game1.content.CreateTemporary();
      this.pictures = Game1.currentSeason.Equals("winter") ? this.content.Load<Texture2D>("LooseSprites\\cowPhotosWinter") : this.content.Load<Texture2D>("LooseSprites\\cowPhotos");
      float num = 1f / Game1.options.zoomLevel;
      this.centerOfScreen = new Vector2((float) (Game1.game1.localMultiplayerWindow.Width / 2), (float) (Game1.game1.localMultiplayerWindow.Height / 2)) * num;
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool tick(GameTime time)
    {
      this.betweenPhotoTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.betweenPhotoTimer <= 0)
      {
        this.betweenPhotoTimer = 5000;
        ++this.numberOfPhotosSoFar;
        if (this.numberOfPhotosSoFar < 5)
          Game1.playSound("cameraNoise");
        if (this.numberOfPhotosSoFar >= 6)
        {
          ++Game1.currentLocation.currentEvent.CurrentCommand;
          return true;
        }
      }
      if (this.numberOfPhotosSoFar >= 5)
        this.fadeAlpha = Math.Min(1f, this.fadeAlpha += 0.007f);
      if (this.numberOfPhotosSoFar > 0)
      {
        Game1.player.blinkTimer = 0;
        Game1.player.currentEyes = 0;
      }
      return false;
    }

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveKeyPress(Keys k)
    {
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap);
      Rectangle bounds;
      if (this.numberOfPhotosSoFar > 0)
      {
        b.Draw(this.pictures, this.centerOfScreen + new Vector2(-208f, -248f), new Rectangle?(new Rectangle(0, 0, 104, 124)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
        Game1.player.faceDirection(2);
        Game1.player.FarmerRenderer.draw(b, Game1.player, 0, this.centerOfScreen + new Vector2(-208f, -248f) + new Vector2(70f, 66f) * 4f, 0.01f);
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = this.centerOfScreen + new Vector2(-208f, -248f) + new Vector2(70f, 66f) * 4f + new Vector2(32f, 120f);
        Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        double x = (double) Game1.shadowTexture.Bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, 0.005f);
      }
      if (this.numberOfPhotosSoFar > 1)
      {
        Game1.player.faceDirection(3);
        b.Draw(this.pictures, this.centerOfScreen + new Vector2(-208f, -248f) + new Vector2(16f, 16f), new Rectangle?(new Rectangle(104, 0, 104, 124)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
        Game1.player.FarmerRenderer.draw(b, Game1.player, 6, this.centerOfScreen + new Vector2(-208f, -248f) + new Vector2(16f, 16f) + new Vector2(64f, 66f) * 4f, 0.11f, true);
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = this.centerOfScreen + new Vector2(-208f, -248f) + new Vector2(16f, 16f) + new Vector2(64f, 66f) * 4f + new Vector2(32f, 120f);
        Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, 0.105f);
      }
      if (this.numberOfPhotosSoFar > 2)
      {
        Game1.player.faceDirection(3);
        b.Draw(this.pictures, this.centerOfScreen + new Vector2(-208f, -248f) - new Vector2(24f, 8f), new Rectangle?(new Rectangle(0, 124, 104, 124)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.2f);
        Game1.player.FarmerRenderer.draw(b, Game1.player, 89, this.centerOfScreen + new Vector2(-208f, -248f) - new Vector2(24f, 8f) + new Vector2(55f, 66f) * 4f, 0.21f, true);
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = this.centerOfScreen + new Vector2(-208f, -248f) - new Vector2(24f, 8f) + new Vector2(55f, 66f) * 4f + new Vector2(32f, 120f);
        Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, 0.205f);
      }
      if (this.numberOfPhotosSoFar > 3)
      {
        Game1.player.faceDirection(2);
        b.Draw(this.pictures, this.centerOfScreen + new Vector2(-208f, -248f), new Rectangle?(new Rectangle(104, 124, 104, 124)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.3f);
        Game1.player.FarmerRenderer.draw(b, Game1.player, 94, this.centerOfScreen + new Vector2(-208f, -248f) + new Vector2(70f, 66f) * 4f, 0.31f);
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = this.centerOfScreen + new Vector2(-208f, -248f) + new Vector2(70f, 66f) * 4f + new Vector2(32f, 120f);
        Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, 0.305f);
      }
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height), new Rectangle?(Game1.staminaRect.Bounds), Color.Black * this.fadeAlpha, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
      b.End();
    }

    public void changeScreenSize()
    {
      float num = 1f / Game1.options.zoomLevel;
      this.centerOfScreen = new Vector2((float) (Game1.game1.localMultiplayerWindow.Width / 2), (float) (Game1.game1.localMultiplayerWindow.Height / 2)) * num;
    }

    public void unload() => this.content.Unload();

    public void receiveEventPoke(int data) => throw new NotImplementedException();

    public string minigameId() => (string) null;

    public bool doMainGameUpdates() => false;

    public bool forceQuit() => false;
  }
}
