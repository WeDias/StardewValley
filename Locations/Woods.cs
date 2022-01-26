// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Woods
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class Woods : GameLocation
  {
    public const int numBaubles = 25;
    private List<Vector2> baubles;
    private List<WeatherDebris> weatherDebris;
    public readonly NetObjectList<ResourceClump> stumps = new NetObjectList<ResourceClump>();
    [XmlElement("hasUnlockedStatue")]
    public readonly NetBool hasUnlockedStatue = new NetBool();
    [XmlIgnore]
    [Obsolete]
    public bool hasFoundStardrop;
    [XmlElement("addedSlimesToday")]
    private readonly NetBool addedSlimesToday = new NetBool();
    [XmlIgnore]
    private readonly NetEvent0 statueAnimationEvent = new NetEvent0();
    protected Color _ambientLightColor = Color.White;
    private int statueTimer;

    public Woods()
    {
    }

    public Woods(string map, string name)
      : base(map, name)
    {
      this.isOutdoors.Value = true;
      this.ignoreDebrisWeather.Value = true;
      this.ignoreOutdoorLighting.Value = true;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.stumps, (INetSerializable) this.addedSlimesToday, (INetSerializable) this.statueAnimationEvent, (INetSerializable) this.hasUnlockedStatue);
      this.statueAnimationEvent.onEvent += new NetEvent0.Event(this.doStatueAnimation);
    }

    public bool localPlayerHasFoundStardrop() => Game1.player.hasOrWillReceiveMail("CF_Statue");

    public override void checkForMusic(GameTime time)
    {
      if (!Game1.isMusicContextActiveButNotPlaying())
        return;
      if (Game1.isRaining)
      {
        Game1.changeMusicTrack("rain");
      }
      else
      {
        if (Game1.isDarkOut())
          return;
        Game1.changeMusicTrack(Game1.currentSeason + "_day_ambient");
      }
    }

    public void statueAnimation(Farmer who)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.hasUnlockedStatue)
        return;
      who.reduceActiveItemByOne();
      this.hasUnlockedStatue.Value = true;
      this.statueAnimationEvent.Fire();
    }

    private void doStatueAnimation()
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 7f) * 64f, Color.White, 9, animationInterval: 50f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 7f) * 64f, Color.Orange, 9, animationInterval: 70f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 6f) * 64f, Color.White, 9, animationInterval: 60f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 6f) * 64f, Color.OrangeRed, 9, animationInterval: 120f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 5f) * 64f, Color.Red, 9));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 5f) * 64f, Color.White, 9, animationInterval: 170f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 464f), Color.Orange, 9, animationInterval: 40f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 464f), Color.White, 9, animationInterval: 90f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 400f), Color.OrangeRed, 9, animationInterval: 190f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 400f), Color.White, 9, animationInterval: 80f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 336f), Color.Red, 9, animationInterval: 69f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 336f), Color.OrangeRed, 9, animationInterval: 130f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(480f, 464f), Color.Orange, 9, animationInterval: 40f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(672f, 368f), Color.White, 9, animationInterval: 90f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(480f, 464f), Color.Red, 9, animationInterval: 30f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(672f, 368f), Color.White, 9, animationInterval: 180f));
      this.localSound("secret1");
      this.updateStatueEyes();
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
      if (tile != null && who.IsLocalPlayer)
      {
        switch (tile.TileIndex)
        {
          case 1140:
          case 1141:
            if (!(bool) (NetFieldBase<bool, NetBool>) this.hasUnlockedStatue)
            {
              if (who.ActiveObject != null && who.ActiveObject.ParentSheetIndex == 417)
              {
                this.statueTimer = 1000;
                who.freezePause = 1000;
                Game1.changeMusicTrack("none");
                this.playSound("newArtifact");
              }
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Woods_Statue").Replace('\n', '^'));
            }
            if ((bool) (NetFieldBase<bool, NetBool>) this.hasUnlockedStatue && !this.localPlayerHasFoundStardrop() && who.freeSpotsInInventory() > 0)
            {
              who.addItemByMenuIfNecessaryElseHoldUp((Item) new StardewValley.Object(434, 1));
              if (!Game1.player.mailReceived.Contains("CF_Statue"))
                Game1.player.mailReceived.Add("CF_Statue");
            }
            return true;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public override StardewValley.Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      return Game1.random.NextDouble() < 0.08 ? (StardewValley.Object) new Furniture(2425, Vector2.Zero) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      foreach (ResourceClump stump in (NetList<ResourceClump, NetRef<ResourceClump>>) this.stumps)
      {
        if (stump.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) stump.tile).Intersects(position))
          return true;
      }
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if (t is Axe)
      {
        Point point = new Point(tileX * 64 + 32, tileY * 64 + 32);
        for (int index = this.stumps.Count - 1; index >= 0; --index)
        {
          if (this.stumps[index].getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.stumps[index].tile).Contains(point))
          {
            if (this.stumps[index].performToolAction(t, 1, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.stumps[index].tile, (GameLocation) this))
              this.stumps.RemoveAt(index);
            return true;
          }
        }
      }
      return false;
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this.characters[index] != null && this.characters[index] is Monster)
        {
          this.characters.RemoveAt(index);
          --index;
        }
      }
      this.addedSlimesToday.Value = false;
      PropertyValue propertyValue;
      this.map.Properties.TryGetValue("Stumps", out propertyValue);
      if (propertyValue == null)
        return;
      string[] strArray = propertyValue.ToString().Split(' ');
      for (int index = 0; index < strArray.Length; index += 3)
      {
        Vector2 vector2 = new Vector2((float) Convert.ToInt32(strArray[index]), (float) Convert.ToInt32(strArray[index + 1]));
        bool flag = false;
        foreach (ResourceClump stump in (NetList<ResourceClump, NetRef<ResourceClump>>) this.stumps)
        {
          if (stump.tile.Equals((object) vector2))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          this.stumps.Add(new ResourceClump(600, 2, 2, vector2));
          this.removeObject(vector2, false);
          this.removeObject(vector2 + new Vector2(1f, 0.0f), false);
          this.removeObject(vector2 + new Vector2(1f, 1f), false);
          this.removeObject(vector2 + new Vector2(0.0f, 1f), false);
        }
      }
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      Game1.changeMusicTrack("none");
      if (this.baubles != null)
        this.baubles.Clear();
      if (this.weatherDebris == null)
        return;
      this.weatherDebris.Clear();
    }

    public override bool isTileLocationTotallyClearAndPlaceable(Vector2 v)
    {
      foreach (ResourceClump stump in (NetList<ResourceClump, NetRef<ResourceClump>>) this.stumps)
      {
        if (stump.occupiesTile((int) v.X, (int) v.Y))
          return false;
      }
      return base.isTileLocationTotallyClearAndPlaceable(v);
    }

    protected override void resetSharedState()
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.addedSlimesToday)
      {
        this.addedSlimesToday.Value = true;
        Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame + 12);
        for (int index = 50; index > 0; --index)
        {
          Vector2 randomTile = this.getRandomTile();
          if (random.NextDouble() < 0.25 && this.isTileLocationTotallyClearAndPlaceable(randomTile))
          {
            string currentSeason = Game1.currentSeason;
            if (!(currentSeason == "spring"))
            {
              if (!(currentSeason == "summer"))
              {
                if (!(currentSeason == "fall"))
                {
                  if (currentSeason == "winter")
                    this.characters.Add((NPC) new GreenSlime(randomTile * 64f, 40));
                }
                else
                  this.characters.Add((NPC) new GreenSlime(randomTile * 64f, random.NextDouble() < 0.5 ? 0 : 40));
              }
              else
                this.characters.Add((NPC) new GreenSlime(randomTile * 64f, 0));
            }
            else
              this.characters.Add((NPC) new GreenSlime(randomTile * 64f, 0));
          }
        }
      }
      base.resetSharedState();
    }

    protected void _updateWoodsLighting()
    {
      if (Game1.currentLocation != this)
        return;
      int minutes1 = Utility.ConvertTimeToMinutes(Game1.getStartingToGetDarkTime());
      int minutes2 = Utility.ConvertTimeToMinutes(Game1.getModeratelyDarkTime());
      int minutes3 = Utility.ConvertTimeToMinutes(Game1.getModeratelyDarkTime());
      int minutes4 = Utility.ConvertTimeToMinutes(Game1.getTrulyDarkTime());
      double num = (double) Utility.ConvertTimeToMinutes(Game1.timeOfDay) + (double) Game1.gameTimeInterval / 7000.0 * 10.0;
      float t1 = Utility.Clamp(((float) num - (float) minutes1) / (float) (minutes2 - minutes1), 0.0f, 1f);
      float t2 = Utility.Clamp(((float) num - (float) minutes3) / (float) (minutes4 - minutes3), 0.0f, 1f);
      Game1.ambientLight.R = (byte) Utility.Lerp((float) this._ambientLightColor.R, (float) Game1.outdoorLight.R, t1);
      Game1.ambientLight.G = (byte) Utility.Lerp((float) this._ambientLightColor.G, (float) Game1.outdoorLight.G, t1);
      Game1.ambientLight.B = (byte) Utility.Lerp((float) this._ambientLightColor.B, (float) Game1.outdoorLight.B, t1);
      Game1.ambientLight.A = (byte) Utility.Lerp((float) this._ambientLightColor.A, (float) Game1.outdoorLight.A, t1);
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

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      this.updateStatueEyes();
    }

    protected override void resetLocalState()
    {
      this._ambientLightColor = new Color(150, 120, 50);
      this.ignoreOutdoorLighting.Value = false;
      if (!Game1.player.mailReceived.Contains("beenToWoods"))
        Game1.player.mailReceived.Add("beenToWoods");
      base.resetLocalState();
      this._updateWoodsLighting();
      int num1 = 25 + new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2).Next(0, 75);
      if (!Game1.isRaining)
      {
        this.baubles = new List<Vector2>();
        for (int index = 0; index < num1; ++index)
          this.baubles.Add(new Vector2((float) Game1.random.Next(0, this.map.DisplayWidth), (float) Game1.random.Next(0, this.map.DisplayHeight)));
        if (!Game1.currentSeason.Equals("winter"))
        {
          this.weatherDebris = new List<WeatherDebris>();
          int maxValue = 192;
          int which = 1;
          if (Game1.currentSeason.Equals("fall"))
            which = 2;
          for (int index = 0; index < num1; ++index)
          {
            List<WeatherDebris> weatherDebris1 = this.weatherDebris;
            int num2 = index * maxValue;
            Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
            int width1 = viewport.Width;
            double x = (double) (num2 % width1 + Game1.random.Next(maxValue));
            int num3 = index * maxValue;
            viewport = Game1.graphics.GraphicsDevice.Viewport;
            int width2 = viewport.Width;
            int num4 = num3 / width2 * maxValue;
            viewport = Game1.graphics.GraphicsDevice.Viewport;
            int height = viewport.Height;
            double y = (double) (num4 % height + Game1.random.Next(maxValue));
            WeatherDebris weatherDebris2 = new WeatherDebris(new Vector2((float) x, (float) y), which, (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f);
            weatherDebris1.Add(weatherDebris2);
          }
        }
      }
      if (Game1.timeOfDay >= 1800)
        return;
      Game1.changeMusicTrack("woodsTheme");
    }

    private void updateStatueEyes()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.hasUnlockedStatue && !this.localPlayerHasFoundStardrop())
      {
        this.map.GetLayer("Front").Tiles[8, 6].TileIndex = 1117;
        this.map.GetLayer("Front").Tiles[9, 6].TileIndex = 1118;
      }
      else
      {
        this.map.GetLayer("Front").Tiles[8, 6].TileIndex = 1115;
        this.map.GetLayer("Front").Tiles[9, 6].TileIndex = 1116;
      }
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
      this.statueAnimationEvent.Poll();
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this._updateWoodsLighting();
      if (this.statueTimer > 0)
      {
        this.statueTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.statueTimer <= 0)
          this.statueAnimation(Game1.player);
      }
      if (this.baubles != null)
      {
        for (int index = 0; index < this.baubles.Count; ++index)
        {
          Vector2 vector2 = new Vector2();
          vector2.X = (float) ((double) this.baubles[index].X - (double) Math.Max(0.4f, Math.Min(1f, (float) index * 0.01f)) - (double) index * 0.00999999977648258 * Math.Sin(2.0 * Math.PI * (double) time.TotalGameTime.Milliseconds / 8000.0));
          vector2.Y = this.baubles[index].Y + Math.Max(0.5f, Math.Min(1.2f, (float) index * 0.02f));
          if ((double) vector2.Y > (double) this.map.DisplayHeight || (double) vector2.X < 0.0)
          {
            vector2.X = (float) Game1.random.Next(0, this.map.DisplayWidth);
            vector2.Y = -64f;
          }
          this.baubles[index] = vector2;
        }
      }
      if (this.weatherDebris != null)
      {
        foreach (WeatherDebris weatherDebri in this.weatherDebris)
          weatherDebri.update();
        Game1.updateDebrisWeatherForMovement(this.weatherDebris);
      }
      foreach (ResourceClump stump in (NetList<ResourceClump, NetRef<ResourceClump>>) this.stumps)
        stump.tickUpdate(time, (Vector2) (NetFieldBase<Vector2, NetVector2>) stump.tile, (GameLocation) this);
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (Game1.eventUp && (this.currentEvent == null || !this.currentEvent.showGroundObjects))
        return;
      foreach (ResourceClump stump in (NetList<ResourceClump, NetRef<ResourceClump>>) this.stumps)
        stump.draw(b, (Vector2) (NetFieldBase<Vector2, NetVector2>) stump.tile);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      if (this.baubles != null)
      {
        for (int index = 0; index < this.baubles.Count; ++index)
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.baubles[index]), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(346 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (index * 25)) % 600.0) / 150 * 5, 1971, 5, 5)), Color.White, (float) index * 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      if (this.weatherDebris == null || this.currentEvent != null)
        return;
      foreach (WeatherDebris weatherDebri in this.weatherDebris)
        weatherDebri.draw(b);
    }
  }
}
