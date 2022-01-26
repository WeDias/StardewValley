// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandSouthEast
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandSouthEast : IslandLocation
  {
    [XmlIgnore]
    public Texture2D mermaidSprites;
    [XmlIgnore]
    public int lastPlayedNote = -1;
    [XmlIgnore]
    public int songIndex = -1;
    [XmlIgnore]
    public int[] mermaidIdle = new int[1];
    [XmlIgnore]
    public int[] mermaidWave = new int[4]{ 1, 1, 2, 2 };
    [XmlIgnore]
    public int[] mermaidReward = new int[7]
    {
      3,
      3,
      3,
      3,
      3,
      4,
      4
    };
    [XmlIgnore]
    public int[] mermaidDance = new int[6]
    {
      5,
      5,
      5,
      6,
      6,
      6
    };
    [XmlIgnore]
    public int mermaidFrameIndex;
    [XmlIgnore]
    public int[] currentMermaidAnimation;
    [XmlIgnore]
    public float mermaidFrameTimer;
    [XmlIgnore]
    public float mermaidDanceTime;
    [XmlIgnore]
    public NetEvent0 mermaidPuzzleSuccess = new NetEvent0();
    [XmlElement("mermaidPuzzleFinished")]
    public NetBool mermaidPuzzleFinished = new NetBool();
    [XmlIgnore]
    public NetEvent0 fishWalnutEvent = new NetEvent0();
    [XmlElement("fishedWalnut")]
    public NetBool fishedWalnut = new NetBool();

    public IslandSouthEast()
    {
    }

    public IslandSouthEast(string map, string name)
      : base(map, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.mermaidPuzzleSuccess, (INetSerializable) this.mermaidPuzzleFinished, (INetSerializable) this.fishWalnutEvent, (INetSerializable) this.fishedWalnut);
      this.mermaidPuzzleSuccess.onEvent += new NetEvent0.Event(this.OnMermaidPuzzleSuccess);
      this.fishWalnutEvent.onEvent += new NetEvent0.Event(this.OnFishWalnut);
    }

    public virtual void OnMermaidPuzzleSuccess()
    {
      this.currentMermaidAnimation = this.mermaidReward;
      this.mermaidFrameTimer = 0.0f;
      if (Game1.currentLocation == this)
        Game1.playSound("yoba");
      if (!Game1.IsMasterGame || this.mermaidPuzzleFinished.Value)
        return;
      Game1.player.team.MarkCollectedNut("Mermaid");
      this.mermaidPuzzleFinished.Value = true;
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(32f, 33f) * 64f, 0, (GameLocation) this, 0);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(32f, 33f) * 64f, 0, (GameLocation) this, 0);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(32f, 33f) * 64f, 0, (GameLocation) this, 0);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(32f, 33f) * 64f, 0, (GameLocation) this, 0);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(32f, 33f) * 64f, 0, (GameLocation) this, 0);
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (Game1.IsRainingHere((GameLocation) this))
      {
        this.setMapTile(16, 27, 3, "Back", "", 2);
        this.setMapTile(18, 27, 4, "Back", "", 2);
        this.setMapTile(20, 27, 5, "Back", "", 2);
        this.setMapTile(22, 27, 6, "Back", "", 2);
        this.setMapTile(24, 27, 7, "Back", "", 2);
        this.setMapTile(26, 27, 8, "Back", "", 2);
      }
      else
      {
        this.setMapTile(16, 27, 39, "Back", "");
        this.setMapTile(18, 27, 39, "Back", "");
        this.setMapTile(20, 27, 39, "Back", "");
        this.setMapTile(22, 27, 39, "Back", "");
        this.setMapTile(24, 27, 39, "Back", "");
        this.setMapTile(26, 27, 39, "Back", "");
      }
      if (IslandSouthEastCave.isPirateNight())
      {
        this.setMapTileIndex(29, 18, 36, "Buildings", 2);
        this.setTileProperty(29, 18, "Buildings", "Passable", "T");
        this.setMapTileIndex(29, 19, 68, "Buildings", 2);
        this.setTileProperty(29, 19, "Buildings", "Passable", "T");
        this.setMapTileIndex(30, 18, 99, "Buildings", 2);
        this.setTileProperty(30, 18, "Buildings", "Passable", "T");
        this.setMapTileIndex(30, 19, 131, "Buildings", 2);
        this.setTileProperty(30, 19, "Buildings", "Passable", "T");
      }
      else
      {
        this.setMapTileIndex(29, 18, 35, "Buildings", 2);
        this.setTileProperty(29, 18, "Buildings", "Passable", "T");
        this.setMapTileIndex(29, 19, 67, "Buildings", 2);
        this.setTileProperty(29, 19, "Buildings", "Passable", "T");
        this.setMapTileIndex(30, 18, 35, "Buildings", 2);
        this.setTileProperty(30, 18, "Buildings", "Passable", "T");
        this.setMapTileIndex(30, 19, 67, "Buildings", 2);
        this.setTileProperty(30, 19, "Buildings", "Passable", "T");
      }
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.mermaidSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
      if (IslandSouthEastCave.isPirateNight())
      {
        Game1.changeMusicTrack("PIRATE_THEME(muffled)", true, Game1.MusicContext.SubLocation);
        if (!this.hasLightSource(797))
          this.sharedLights.Add(797, new LightSource(1, new Vector2(30.5f, 18.5f) * 64f, 4f));
      }
      if (!(Game1.currentSeason == "winter") || Game1.IsRainingHere((GameLocation) this) || !Game1.isDarkOut())
        return;
      this.addMoonlightJellies(50, new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame - 24917), new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
    }

    public override void cleanupBeforePlayerExit()
    {
      this.removeLightSource(797);
      base.cleanupBeforePlayerExit();
    }

    public override void DayUpdate(int dayOfMonth) => base.DayUpdate(dayOfMonth);

    public override void SetBuriedNutLocations()
    {
      base.SetBuriedNutLocations();
      this.buriedNutPoints.Add(new Point(25, 17));
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.mermaidPuzzleSuccess.Poll();
      this.fishWalnutEvent.Poll();
      if (!(bool) (NetFieldBase<bool, NetBool>) this.fishedWalnut && Game1.random.NextDouble() < 0.005)
      {
        this.playSound("waterSlosh");
        this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 150f, 8, 0, new Vector2(1216f, 1344f), false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
      }
      if (!this.MermaidIsHere())
        return;
      bool flag = false;
      if (this.mermaidPuzzleFinished.Value)
      {
        foreach (Character farmer in this.farmers)
        {
          Point tileLocationPoint = farmer.getTileLocationPoint();
          if (tileLocationPoint.X > 24 && tileLocationPoint.Y > 25)
          {
            flag = true;
            break;
          }
        }
      }
      if (flag && (this.currentMermaidAnimation == null || this.currentMermaidAnimation == this.mermaidIdle))
      {
        this.currentMermaidAnimation = this.mermaidWave;
        this.mermaidFrameIndex = 0;
        this.mermaidFrameTimer = 0.0f;
      }
      if ((double) this.mermaidDanceTime > 0.0)
      {
        if (this.currentMermaidAnimation == null || this.currentMermaidAnimation == this.mermaidIdle)
        {
          this.currentMermaidAnimation = this.mermaidDance;
          this.mermaidFrameTimer = 0.0f;
        }
        this.mermaidDanceTime -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.mermaidDanceTime < 0.0 && this.currentMermaidAnimation == this.mermaidDance)
        {
          this.currentMermaidAnimation = this.mermaidIdle;
          this.mermaidFrameTimer = 0.0f;
        }
      }
      this.mermaidFrameTimer += (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.mermaidFrameTimer <= 0.25)
        return;
      this.mermaidFrameTimer = 0.0f;
      ++this.mermaidFrameIndex;
      if (this.currentMermaidAnimation == null)
      {
        this.mermaidFrameIndex = 0;
      }
      else
      {
        if (this.mermaidFrameIndex < this.currentMermaidAnimation.Length)
          return;
        this.mermaidFrameIndex = 0;
        if (this.currentMermaidAnimation == this.mermaidReward)
        {
          if (flag)
            this.currentMermaidAnimation = this.mermaidWave;
          else
            this.currentMermaidAnimation = this.mermaidIdle;
        }
        else
        {
          if (flag || this.currentMermaidAnimation != this.mermaidWave)
            return;
          this.currentMermaidAnimation = this.mermaidIdle;
        }
      }
    }

    public bool MermaidIsHere() => Game1.IsRainingHere((GameLocation) this);

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (!this.MermaidIsHere())
        return;
      int num = 0;
      if (this.currentMermaidAnimation != null && this.mermaidFrameIndex < this.currentMermaidAnimation.Length)
        num = this.currentMermaidAnimation[this.mermaidFrameIndex];
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(32f, 32f) * 64f + new Vector2(0.0f, -8f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(304 + 28 * num, 592, 28, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0009f);
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
      if ((int) bobberTile.X < 18 || (int) bobberTile.X > 20 || (int) bobberTile.Y < 20 || (int) bobberTile.Y > 22)
        return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName = (string) null);
      if (!this.fishedWalnut.Value)
      {
        Game1.player.team.MarkCollectedNut("StardropPool");
        if (!Game1.IsMultiplayer)
        {
          this.fishedWalnut.Value = true;
          return new StardewValley.Object(73, 1);
        }
        this.fishWalnutEvent.Fire();
      }
      return (StardewValley.Object) null;
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action != null && who.IsLocalPlayer)
      {
        string str = action.Split(' ')[0];
      }
      return base.performAction(action, who, tileLocation);
    }

    public void OnFishWalnut()
    {
      if (this.fishedWalnut.Value || !Game1.IsMasterGame)
        return;
      Vector2 vector2 = new Vector2(19f, 21f);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), vector2 * 64f + new Vector2(0.5f, 1.5f) * 64f, 0, (GameLocation) this, 0);
      Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite(28, 100f, 2, 1, vector2 * 64f, false, false)
      {
        layerDepth = (float) ((((double) vector2.Y + 0.5) * 64.0 + 2.0) / 10000.0)
      });
      this.playSound("dropItemInWater");
      this.fishedWalnut.Value = true;
    }

    public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null) => base.isTileOccupiedForPlacement(tileLocation, toPlace);

    public override void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public override void updateSeasonalTileSheets(Map map = null)
    {
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      if (!(l is IslandSouthEast))
        return;
      this.mermaidPuzzleFinished.Value = (l as IslandSouthEast).mermaidPuzzleFinished.Value;
      this.fishedWalnut.Value = (l as IslandSouthEast).fishedWalnut.Value;
    }

    public virtual void OnFlutePlayed(int pitch)
    {
      if (!this.MermaidIsHere())
        return;
      if (this.songIndex == -1)
      {
        this.lastPlayedNote = pitch;
        this.songIndex = 0;
      }
      int num = pitch - this.lastPlayedNote;
      if (num == 900)
      {
        this.songIndex = 1;
        this.mermaidDanceTime = 5f;
      }
      else if (this.songIndex == 1)
      {
        if (num == -200)
        {
          ++this.songIndex;
          this.mermaidDanceTime = 5f;
        }
        else
        {
          this.songIndex = -1;
          this.mermaidDanceTime = 0.0f;
          this.currentMermaidAnimation = this.mermaidIdle;
        }
      }
      else if (this.songIndex == 2)
      {
        if (num == -400)
        {
          ++this.songIndex;
          this.mermaidDanceTime = 5f;
        }
        else
        {
          this.songIndex = -1;
          this.mermaidDanceTime = 0.0f;
          this.currentMermaidAnimation = this.mermaidIdle;
        }
      }
      else if (this.songIndex == 3)
      {
        if (num == 200)
        {
          this.songIndex = 0;
          this.mermaidPuzzleSuccess.Fire();
          this.mermaidDanceTime = 0.0f;
        }
        else
        {
          this.songIndex = -1;
          this.mermaidDanceTime = 0.0f;
          this.currentMermaidAnimation = this.mermaidIdle;
        }
      }
      this.lastPlayedNote = pitch;
    }
  }
}
