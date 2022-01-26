// Decompiled with JetBrains decompiler
// Type: FrameRateCounter
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

public class FrameRateCounter : DrawableGameComponent
{
  private LocalizedContentManager content;
  private SpriteBatch spriteBatch;
  private int frameRate;
  private int frameCounter;
  private TimeSpan elapsedTime = TimeSpan.Zero;

  public FrameRateCounter(Game game)
    : base(game)
  {
    this.content = new LocalizedContentManager((IServiceProvider) game.Services, this.Game.Content.RootDirectory);
  }

  protected override void LoadContent() => this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

  protected override void UnloadContent() => this.content.Unload();

  public override void Update(GameTime gameTime)
  {
    this.elapsedTime += gameTime.ElapsedGameTime;
    if (!(this.elapsedTime > TimeSpan.FromSeconds(1.0)))
      return;
    this.elapsedTime -= TimeSpan.FromSeconds(1.0);
    this.frameRate = this.frameCounter;
    this.frameCounter = 0;
  }

  public override void Draw(GameTime gameTime)
  {
    ++this.frameCounter;
    string text = string.Format("fps: {0}", (object) this.frameRate);
    this.spriteBatch.Begin();
    this.spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(33f, 33f), Microsoft.Xna.Framework.Color.Black);
    this.spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(32f, 32f), Microsoft.Xna.Framework.Color.White);
    this.spriteBatch.End();
  }
}
