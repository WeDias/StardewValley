// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.TelescopeScene
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using xTile;
using xTile.Dimensions;
using xTile.Layers;

namespace StardewValley.Minigames
{
  public class TelescopeScene : IMinigame
  {
    public LocalizedContentManager temporaryContent;
    public Texture2D background;
    public Texture2D trees;
    public float yOffset;
    public GameLocation walkSpace;

    public TelescopeScene(NPC Maru)
    {
      this.temporaryContent = Game1.content.CreateTemporary();
      this.background = this.temporaryContent.Load<Texture2D>("LooseSprites\\nightSceneMaru");
      this.trees = this.temporaryContent.Load<Texture2D>("LooseSprites\\nightSceneMaruTrees");
      this.walkSpace = new GameLocation((string) null, nameof (walkSpace));
      this.walkSpace.map = new Map();
      this.walkSpace.map.AddLayer(new Layer("Back", this.walkSpace.map, new Size(30, 1), new Size(64)));
      Game1.currentLocation = this.walkSpace;
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool tick(GameTime time) => false;

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
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      SpriteBatch spriteBatch1 = b;
      Texture2D background = this.background;
      Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      double x1 = (double) (viewport1.Width / 2 - this.background.Bounds.Width / 2 * 4);
      int num1 = -(this.background.Bounds.Height * 4);
      viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int height1 = viewport1.Height;
      double y1 = (double) (num1 + height1);
      Vector2 position1 = new Vector2((float) x1, (float) y1);
      Microsoft.Xna.Framework.Rectangle? sourceRectangle1 = new Microsoft.Xna.Framework.Rectangle?(this.background.Bounds);
      Color white1 = Color.White;
      Vector2 zero1 = Vector2.Zero;
      spriteBatch1.Draw(background, position1, sourceRectangle1, white1, 0.0f, zero1, 4f, SpriteEffects.None, 1f / 1000f);
      SpriteBatch spriteBatch2 = b;
      Texture2D trees = this.trees;
      Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      double x2 = (double) (viewport2.Width / 2 - this.trees.Bounds.Width / 2 * 4);
      int num2 = -(this.trees.Bounds.Height * 4);
      viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      int height2 = viewport2.Height;
      double y2 = (double) (num2 + height2);
      Vector2 position2 = new Vector2((float) x2, (float) y2);
      Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = new Microsoft.Xna.Framework.Rectangle?(this.trees.Bounds);
      Color white2 = Color.White;
      Vector2 zero2 = Vector2.Zero;
      spriteBatch2.Draw(trees, position2, sourceRectangle2, white2, 0.0f, zero2, 4f, SpriteEffects.None, 1f);
      b.End();
    }

    public void changeScreenSize()
    {
    }

    public void unload() => this.temporaryContent.Unload();

    public void receiveEventPoke(int data) => throw new NotImplementedException();

    public string minigameId() => (string) null;

    public bool doMainGameUpdates() => false;

    public bool forceQuit() => false;
  }
}
