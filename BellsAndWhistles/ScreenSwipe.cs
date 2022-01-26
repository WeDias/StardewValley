// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.ScreenSwipe
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class ScreenSwipe
  {
    public const int swipe_bundleComplete = 0;
    public const int borderPixelWidth = 7;
    private Rectangle bgSource;
    private Rectangle flairSource;
    private Rectangle messageSource;
    private Rectangle movingFlairSource;
    private Rectangle bgDest;
    private int yPosition;
    private int durationAfterSwipe;
    private int originalBGSourceXLimit;
    private List<Vector2> flairPositions = new List<Vector2>();
    private Vector2 messagePosition;
    private Vector2 movingFlairPosition;
    private Vector2 movingFlairMotion;
    private float swipeVelocity;

    public ScreenSwipe(int which, float swipeVelocity = -1f, int durationAfterSwipe = -1)
    {
      Game1.playSound("throw");
      if ((double) swipeVelocity == -1.0)
        swipeVelocity = 5f;
      if (durationAfterSwipe == -1)
        durationAfterSwipe = 2700;
      this.swipeVelocity = swipeVelocity;
      this.durationAfterSwipe = durationAfterSwipe;
      Vector2 vector2 = new Vector2((float) (Game1.uiViewport.Width / 2), (float) (Game1.uiViewport.Height / 2));
      if (which == 0)
        this.messageSource = new Rectangle(128, 1367, 150, 14);
      if (which == 0)
      {
        this.bgSource = new Rectangle(128, 1296, 1, 71);
        this.flairSource = new Rectangle(144, 1303, 144, 58);
        this.movingFlairSource = new Rectangle(643, 768, 8, 13);
        this.originalBGSourceXLimit = this.bgSource.X + this.bgSource.Width;
        this.yPosition = (int) vector2.Y - this.bgSource.Height * 4 / 2;
        this.messagePosition = new Vector2(vector2.X - (float) (this.messageSource.Width * 4 / 2), vector2.Y - (float) (this.messageSource.Height * 4 / 2));
        this.flairPositions.Add(new Vector2((float) ((double) this.messagePosition.X - (double) (this.flairSource.Width * 4) - 64.0), (float) (this.yPosition + 28)));
        this.flairPositions.Add(new Vector2((float) ((double) this.messagePosition.X + (double) (this.messageSource.Width * 4) + 64.0), (float) (this.yPosition + 28)));
        this.movingFlairPosition = new Vector2((float) ((double) this.messagePosition.X + (double) (this.messageSource.Width * 4) + 192.0), vector2.Y + 32f);
        this.movingFlairMotion = new Vector2(0.0f, -0.5f);
      }
      this.bgDest = new Rectangle(0, this.yPosition, this.bgSource.Width * 4, this.bgSource.Height * 4);
    }

    public bool update(GameTime time)
    {
      TimeSpan elapsedGameTime;
      if (this.durationAfterSwipe > 0 && this.bgDest.Width <= Game1.uiViewport.Width)
      {
        ref int local = ref this.bgDest.Width;
        int num1 = local;
        double swipeVelocity = (double) this.swipeVelocity;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        int num2 = (int) (swipeVelocity * totalMilliseconds);
        local = num1 + num2;
        if (this.bgDest.Width > Game1.uiViewport.Width)
          Game1.playSound("newRecord");
      }
      else if (this.durationAfterSwipe <= 0)
      {
        ref int local = ref this.bgDest.X;
        int num3 = local;
        double swipeVelocity = (double) this.swipeVelocity;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        int num4 = (int) (swipeVelocity * totalMilliseconds);
        local = num3 + num4;
        for (int index = 0; index < this.flairPositions.Count; ++index)
        {
          if ((double) this.bgDest.X > (double) this.flairPositions[index].X)
            this.flairPositions[index] = new Vector2((float) this.bgDest.X, this.flairPositions[index].Y);
        }
        if ((double) this.bgDest.X > (double) this.messagePosition.X)
          this.messagePosition = new Vector2((float) this.bgDest.X, this.messagePosition.Y);
        if ((double) this.bgDest.X > (double) this.movingFlairPosition.X)
          this.movingFlairPosition = new Vector2((float) this.bgDest.X, this.movingFlairPosition.Y);
      }
      if (this.bgDest.Width > Game1.uiViewport.Width && this.durationAfterSwipe > 0)
      {
        if (Game1.oldMouseState.LeftButton == ButtonState.Pressed)
          this.durationAfterSwipe = 0;
        int durationAfterSwipe = this.durationAfterSwipe;
        elapsedGameTime = time.ElapsedGameTime;
        int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
        this.durationAfterSwipe = durationAfterSwipe - totalMilliseconds;
        if (this.durationAfterSwipe <= 0)
          Game1.playSound("tinyWhip");
      }
      this.movingFlairPosition += this.movingFlairMotion;
      return this.bgDest.X > Game1.uiViewport.Width;
    }

    public Rectangle getAdjustedSourceRect(Rectangle sourceRect, float xStartPosition)
    {
      if ((double) xStartPosition > (double) this.bgDest.Width || (double) xStartPosition + (double) (sourceRect.Width * 4) < (double) this.bgDest.X)
        return Rectangle.Empty;
      int x = (int) Math.Max((float) sourceRect.X, (float) sourceRect.X + (float) (((double) this.bgDest.X - (double) xStartPosition) / 4.0));
      return new Rectangle(x, sourceRect.Y, (int) Math.Min((float) (sourceRect.Width - (x - sourceRect.X) / 4), (float) (((double) this.bgDest.Width - (double) xStartPosition) / 4.0)), sourceRect.Height);
    }

    public void draw(SpriteBatch b)
    {
      b.Draw(Game1.mouseCursors, this.bgDest, new Rectangle?(this.bgSource), Color.White);
      foreach (Vector2 flairPosition in this.flairPositions)
      {
        Rectangle adjustedSourceRect = this.getAdjustedSourceRect(this.flairSource, flairPosition.X);
        if (adjustedSourceRect.Right >= this.originalBGSourceXLimit)
          adjustedSourceRect.Width = this.originalBGSourceXLimit - adjustedSourceRect.X;
        b.Draw(Game1.mouseCursors, flairPosition, new Rectangle?(adjustedSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      b.Draw(Game1.mouseCursors, this.movingFlairPosition, new Rectangle?(this.getAdjustedSourceRect(this.movingFlairSource, this.movingFlairPosition.X)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, this.messagePosition, new Rectangle?(this.getAdjustedSourceRect(this.messageSource, this.messagePosition.X)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }
  }
}
