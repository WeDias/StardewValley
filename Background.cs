// Decompiled with JetBrains decompiler
// Type: StardewValley.Background
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class Background
  {
    public int defaultChunkIndex;
    public int numChunksInSheet;
    public double chanceForDeviationFromDefault;
    private Texture2D backgroundImage;
    private Texture2D cloudsTexture;
    private Vector2 position = Vector2.Zero;
    private int chunksWide;
    private int chunksHigh;
    private int chunkWidth;
    private int chunkHeight;
    private int[] chunks;
    private float zoom;
    public Color c;
    private bool summitBG;
    private bool onlyMapBG;
    public int yOffset;
    public List<TemporaryAnimatedSprite> tempSprites;
    private int initialViewportY;

    /// <summary>constructor for summit background</summary>
    public Background()
    {
      this.summitBG = true;
      this.c = Color.White;
      this.initialViewportY = Game1.viewport.Y;
      this.cloudsTexture = Game1.content.Load<Texture2D>("Minigames\\Clouds");
    }

    public Background(Color color, bool onlyMapBG)
    {
      this.c = color;
      this.onlyMapBG = onlyMapBG;
      this.tempSprites = new List<TemporaryAnimatedSprite>();
    }

    public Background(
      Texture2D bgImage,
      int seedValue,
      int chunksWide,
      int chunksHigh,
      int chunkWidth,
      int chunkHeight,
      float zoom,
      int defaultChunkIndex,
      int numChunksInSheet,
      double chanceForDeviation,
      Color c)
    {
      this.backgroundImage = bgImage;
      this.chunksWide = chunksWide;
      this.chunksHigh = chunksHigh;
      this.zoom = zoom;
      this.chunkWidth = chunkWidth;
      this.chunkHeight = chunkHeight;
      this.defaultChunkIndex = defaultChunkIndex;
      this.numChunksInSheet = numChunksInSheet;
      this.chanceForDeviationFromDefault = chanceForDeviation;
      this.c = c;
      Random random = new Random(seedValue);
      this.chunks = new int[chunksWide * chunksHigh];
      for (int index = 0; index < chunksHigh * chunksWide; ++index)
        this.chunks[index] = random.NextDouble() >= this.chanceForDeviationFromDefault ? defaultChunkIndex : random.Next(numChunksInSheet);
    }

    public void update(xTile.Dimensions.Rectangle viewport)
    {
      this.position.X = (float) -((double) (viewport.X + viewport.Width / 2) / ((double) Game1.currentLocation.map.GetLayer("Back").LayerWidth * 64.0) * ((double) (this.chunksWide * this.chunkWidth) * (double) this.zoom - (double) viewport.Width));
      this.position.Y = (float) -((double) (viewport.Y + viewport.Height / 2) / ((double) Game1.currentLocation.map.GetLayer("Back").LayerHeight * 64.0) * ((double) (this.chunksHigh * this.chunkHeight) * (double) this.zoom - (double) viewport.Height));
    }

    public void draw(SpriteBatch b)
    {
      if (this.summitBG)
      {
        if (Game1.viewport.X <= -1000)
          return;
        int num1 = 0;
        string currentSeason = Game1.currentSeason;
        if (!(currentSeason == "summer"))
        {
          if (!(currentSeason == "fall"))
          {
            if (currentSeason == "winter")
              num1 = 2;
          }
          else
            num1 = 1;
        }
        else
          num1 = 0;
        int num2 = -Game1.viewport.Y / 4 + this.initialViewportY / 4;
        float num3 = 1f;
        float num4 = 1f;
        Color color = Color.White;
        int num5 = (int) ((double) (Game1.timeOfDay - Game1.timeOfDay % 100) + (double) (Game1.timeOfDay % 100 / 10) * 16.6599998474121);
        int num6 = Game1.currentSeason == "winter" ? 30 : 0;
        if (Game1.timeOfDay >= 1800)
        {
          this.c = new Color((float) byte.MaxValue, (float) byte.MaxValue - Math.Max(100f, (float) ((double) num5 + (double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697 - 1800.0)), (float) byte.MaxValue - Math.Max(100f, (float) (((double) num5 + (double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697 - 1800.0) / 2.0)));
          color = Game1.currentSeason == "winter" ? Color.Black * 0.5f : Color.Blue * 0.5f;
          num3 = Math.Max(0.0f, Math.Min(1f, (float) ((2000.0 - ((double) num5 + (double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697)) / 200.0)));
          num4 = Math.Max(0.0f, Math.Min(1f, (float) ((2200.0 - ((double) num5 + (double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697)) / 400.0)));
          Game1.ambientLight = new Color((int) Utility.Lerp(0.0f, 30f, 1f - num3), (int) Utility.Lerp(0.0f, 60f, 1f - num3), (int) Utility.Lerp(0.0f, 15f, 1f - num3));
        }
        b.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(639, 858, 1, 144)), this.c * num4, 0.0f, Vector2.Zero, SpriteEffects.None, 5E-08f);
        b.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Microsoft.Xna.Framework.Rectangle?(Game1.currentSeason == "fall" ? new Microsoft.Xna.Framework.Rectangle(639, 1051, 1, 400) : new Microsoft.Xna.Framework.Rectangle(639 + (num1 + 1), 1051, 1, 400)), this.c * num3, 0.0f, Vector2.Zero, SpriteEffects.None, 1E-07f);
        if (Game1.timeOfDay >= 1800)
          b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.viewport.Height / 2 - 780)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 1453, 638, 195)), Color.White * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
        if (Game1.dayOfMonth == 28 && Game1.timeOfDay > 1900)
          b.Draw(Game1.mouseCursors, new Vector2((float) ((double) ((float) num5 + (float) ((double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697)) / 2600.0 * (double) Game1.viewport.Width / 4.0), (float) (Game1.viewport.Height / 2 + 176) - (float) ((double) ((float) (num5 - 1900) + (float) ((double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697)) / 700.0 * (double) Game1.viewport.Height / 2.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(642, 834, 43, 44)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 5E-08f);
        if (Game1.currentSeason != "winter" && ((bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GetWeatherForLocation(Game1.currentLocation.GetLocationContext()).isDebrisWeather || (bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GetWeatherForLocation(Game1.currentLocation.GetLocationContext()).isRaining))
          b.Draw(this.cloudsTexture, new Vector2((float) Game1.viewport.Width - ((float) num5 + (float) ((double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697)) / 2600f * (float) (Game1.viewport.Width + 2048), (float) (Game1.viewport.Height - 584 - 600 + num2 / 2 + Game1.dayOfMonth * 6)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 512, 340)), Color.White * num3, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 5.6E-08f);
        b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle(0, Game1.viewport.Height - 584 + num2 / 2, Game1.viewport.Width, Game1.viewport.Height / 2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), new Color((int) ((double) num6 + 60.0 * (double) num4), (int) ((double) (num6 + 10) + 170.0 * (double) num4), (int) ((double) (num6 + 20) + 205.0 * (double) num4)), 0.0f, Vector2.Zero, SpriteEffects.None, 2E-07f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), Color.White * Math.Max((float) this.c.A, 0.5f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), color * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 2E-06f);
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), Color.White * Math.Max((float) this.c.A, 0.5f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), color * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 2E-06f);
        foreach (TemporaryAnimatedSprite temporarySprite in Game1.currentLocation.temporarySprites)
          temporarySprite.draw(b);
        b.Draw(this.cloudsTexture, new Vector2(0.0f, (float) (Game1.viewport.Height - 568) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 554 + num1 * 153, 164, 142)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        b.Draw(this.cloudsTexture, new Vector2((float) (Game1.viewport.Width - 488), (float) (Game1.viewport.Height - 612) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(390, 543 + num1 * 153, 122, 153)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        b.Draw(this.cloudsTexture, new Vector2(0.0f, (float) (Game1.viewport.Height - 568) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 554 + num1 * 153, 164, 142)), Color.Black * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.cloudsTexture, new Vector2((float) (Game1.viewport.Width - 488), (float) (Game1.viewport.Height - 612) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(390, 543 + num1 * 153, 122, 153)), Color.Black * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      else if (this.backgroundImage == null)
      {
        Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
        if (this.onlyMapBG)
        {
          destinationRectangle.X = Math.Max(0, -Game1.viewport.X);
          destinationRectangle.Y = Math.Max(0, -Game1.viewport.Y);
          destinationRectangle.Width = Math.Min(Game1.viewport.Width, Game1.currentLocation.map.DisplayWidth);
          destinationRectangle.Height = Math.Min(Game1.viewport.Height, Game1.currentLocation.map.DisplayHeight);
        }
        b.Draw(Game1.staminaRect, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), this.c, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        for (int index = this.tempSprites.Count - 1; index >= 0; --index)
        {
          if (this.tempSprites[index].update(Game1.currentGameTime))
            this.tempSprites.RemoveAt(index);
        }
        for (int index = 0; index < this.tempSprites.Count; ++index)
          this.tempSprites[index].draw(b);
      }
      else
      {
        Vector2 zero = Vector2.Zero;
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, this.chunkWidth, this.chunkHeight);
        for (int index = 0; index < this.chunks.Length; ++index)
        {
          zero.X = this.position.X + (float) (index * this.chunkWidth % (this.chunksWide * this.chunkWidth)) * this.zoom;
          zero.Y = this.position.Y + (float) (index * this.chunkWidth / (this.chunksWide * this.chunkWidth) * this.chunkHeight) * this.zoom;
          if (this.backgroundImage == null)
          {
            b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int) zero.X, (int) zero.Y, Game1.viewport.Width, Game1.viewport.Height), new Microsoft.Xna.Framework.Rectangle?(rectangle), this.c, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
          }
          else
          {
            rectangle.X = this.chunks[index] * this.chunkWidth % this.backgroundImage.Width;
            rectangle.Y = this.chunks[index] * this.chunkWidth / this.backgroundImage.Width * this.chunkHeight;
            b.Draw(this.backgroundImage, zero, new Microsoft.Xna.Framework.Rectangle?(rectangle), this.c, 0.0f, Vector2.Zero, this.zoom, SpriteEffects.None, 0.0f);
          }
        }
      }
    }
  }
}
