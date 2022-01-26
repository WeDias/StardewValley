// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandForestLocation
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using xTile;

namespace StardewValley.Locations
{
  public class IslandForestLocation : IslandLocation
  {
    protected Color _ambientLightColor = Color.White;
    private List<Wisp> _wisps;
    private List<WeatherDebris> weatherDebris;
    protected Texture2D _rayTexture;
    protected Random _rayRandom;
    protected int _raySeed;

    public IslandForestLocation()
    {
    }

    public IslandForestLocation(string map, string name)
      : base(map, name)
    {
    }

    public override void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public override void updateSeasonalTileSheets(Map map = null)
    {
    }

    public override void tryToAddCritters(bool onlyIfOnScreen = false)
    {
    }

    protected override void resetLocalState()
    {
      this._raySeed = (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
      this._rayTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\LightRays");
      this._ambientLightColor = new Color(150, 120, 50);
      this.ignoreOutdoorLighting.Value = false;
      base.resetLocalState();
      this._updateWoodsLighting();
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      this._wisps = new List<Wisp>();
      for (int index = 0; index < 30; ++index)
        this._wisps.Add(new Wisp(index));
      this.weatherDebris = new List<WeatherDebris>();
      int maxValue = 192;
      int which = 3;
      for (int index = 0; index < 10; ++index)
      {
        List<WeatherDebris> weatherDebris1 = this.weatherDebris;
        int num1 = index * maxValue;
        Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
        int width1 = viewport.Width;
        double x = (double) (num1 % width1 + Game1.random.Next(maxValue));
        int num2 = index * maxValue;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int width2 = viewport.Width;
        int num3 = num2 / width2 * maxValue;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int height = viewport.Height;
        double y = (double) (num3 % height + Game1.random.Next(maxValue));
        WeatherDebris weatherDebris2 = new WeatherDebris(new Vector2((float) x, (float) y), which, (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f);
        weatherDebris1.Add(weatherDebris2);
      }
    }

    public override void cleanupBeforePlayerExit()
    {
      if (this._wisps != null)
        this._wisps.Clear();
      if (this.weatherDebris != null)
        this.weatherDebris.Clear();
      if (Game1.locationRequest.Location != null && !(Game1.locationRequest.Location is IslandForestLocation) && !(Game1.locationRequest.Location is IslandHut))
        Game1.changeMusicTrack("none");
      base.cleanupBeforePlayerExit();
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this._updateWoodsLighting();
      if (this._wisps != null)
      {
        for (int index = 0; index < this._wisps.Count; ++index)
          this._wisps[index].Update(time);
      }
      if (this.weatherDebris == null)
        return;
      foreach (WeatherDebris weatherDebri in this.weatherDebris)
        weatherDebri.update();
      Game1.updateDebrisWeatherForMovement(this.weatherDebris);
    }

    protected void _updateWoodsLighting()
    {
      if (Game1.currentLocation != this)
        return;
      int num1 = Utility.ConvertTimeToMinutes(Game1.getModeratelyDarkTime()) - 60;
      int minutes1 = Utility.ConvertTimeToMinutes(Game1.getTrulyDarkTime());
      int minutes2 = Utility.ConvertTimeToMinutes(Game1.getStartingToGetDarkTime());
      int minutes3 = Utility.ConvertTimeToMinutes(Game1.getModeratelyDarkTime());
      double num2 = (double) Utility.ConvertTimeToMinutes(Game1.timeOfDay) + (double) Game1.gameTimeInterval / 7000.0 * 10.0;
      float t1 = Utility.Clamp(((float) num2 - (float) num1) / (float) (minutes1 - num1), 0.0f, 1f);
      float t2 = Utility.Clamp(((float) num2 - (float) minutes2) / (float) (minutes3 - minutes2), 0.0f, 1f);
      Game1.ambientLight.R = (byte) Utility.Lerp((float) this._ambientLightColor.R, (float) Game1.eveningColor.R, t1);
      Game1.ambientLight.G = (byte) Utility.Lerp((float) this._ambientLightColor.G, (float) Game1.eveningColor.G, t1);
      Game1.ambientLight.B = (byte) Utility.Lerp((float) this._ambientLightColor.B, (float) Game1.eveningColor.B, t1);
      Game1.ambientLight.A = (byte) Utility.Lerp((float) this._ambientLightColor.A, (float) Game1.eveningColor.A, t1);
      Color black = Color.Black with
      {
        A = (byte) Utility.Lerp((float) byte.MaxValue, 0.0f, t2)
      };
      foreach (LightSource currentLightSource in Game1.currentLightSources)
      {
        if (currentLightSource.lightContext.Value == LightSource.LightContext.MapLight)
          currentLightSource.color.Value = black;
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this._wisps == null)
        return;
      for (int index = 0; index < this._wisps.Count; ++index)
        this._wisps[index].Draw(b);
    }

    public virtual void DrawRays(SpriteBatch b)
    {
      Random random = new Random(this._raySeed);
      float scale = (float) ((double) Game1.graphics.GraphicsDevice.Viewport.Height * 0.600000023841858 / 128.0);
      int num1 = -(int) (128.0 / (double) scale);
      int num2 = Game1.graphics.GraphicsDevice.Viewport.Width / (int) (32.0 * (double) scale);
      for (int index = num1; index < num2; ++index)
      {
        Color white = Color.White;
        float num3 = (float) ((double) Game1.viewport.X * (double) Utility.RandomFloat(0.75f, 1f, random) + (double) Game1.viewport.Y * (double) Utility.RandomFloat(0.2f, 0.5f, random) + Game1.currentGameTime.TotalGameTime.TotalSeconds * 20.0);
        double num4 = (double) num3 / 360.0;
        float num5 = num3 % 360f;
        float a = num5 * ((float) Math.PI / 180f);
        Color color = white * (Utility.Clamp((float) Math.Sin((double) a), 0.0f, 1f) * Utility.RandomFloat(0.15f, 0.4f, random));
        float num6 = Utility.Lerp(-Utility.RandomFloat(24f, 32f, random), 0.0f, num5 / 360f);
        b.Draw(this._rayTexture, new Vector2(((float) (index * 32) - num6) * scale, Utility.RandomFloat(0.0f, -32f * scale, random)), new Rectangle?(new Rectangle(128 * random.Next(0, 2), 0, 128, 128)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      }
    }

    public override void drawWater(SpriteBatch b) => base.drawWater(b);

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      this.DrawRays(b);
    }
  }
}
