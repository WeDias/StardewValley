// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandWestCave1
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandWestCave1 : IslandLocation
  {
    [XmlIgnore]
    protected List<IslandWestCave1.CaveCrystal> crystals = new List<IslandWestCave1.CaveCrystal>();
    public const int PHASE_INTRO = 0;
    public const int PHASE_PLAY_SEQUENCE = 1;
    public const int PHASE_WAIT_FOR_PLAYER_INPUT = 2;
    public const int PHASE_NOTHING = 3;
    public const int PHASE_SUCCESSFUL_SEQUENCE = 4;
    public const int PHASE_OUTRO = 5;
    [XmlElement("completed")]
    public NetBool completed = new NetBool();
    [XmlIgnore]
    public NetBool isActivated = new NetBool(false);
    [XmlIgnore]
    public NetFloat netPhaseTimer = new NetFloat();
    [XmlIgnore]
    public float localPhaseTimer;
    [XmlIgnore]
    public float betweenNotesTimer;
    [XmlIgnore]
    public int localPhase;
    [XmlIgnore]
    public NetInt netPhase = new NetInt(3);
    [XmlIgnore]
    public NetInt currentDifficulty = new NetInt(2);
    [XmlIgnore]
    public NetInt currentCrystalSequenceIndex = new NetInt(0);
    [XmlIgnore]
    public int currentPlaybackCrystalSequenceIndex;
    [XmlIgnore]
    public NetInt timesFailed = new NetInt(0);
    [XmlIgnore]
    public NetList<int, NetInt> currentCrystalSequence = new NetList<int, NetInt>();
    [XmlIgnore]
    public NetEvent1Field<int, NetInt> enterValueEvent = new NetEvent1Field<int, NetInt>();

    public IslandWestCave1()
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.netPhase, (INetSerializable) this.isActivated, (INetSerializable) this.currentDifficulty, (INetSerializable) this.currentCrystalSequenceIndex, (INetSerializable) this.currentCrystalSequence, this.enterValueEvent.NetFields, (INetSerializable) this.netPhaseTimer, (INetSerializable) this.completed, (INetSerializable) this.timesFailed);
      this.enterValueEvent.onEvent += new AbstractNetEvent1<int>.Event(this.enterValue);
      this.isActivated.fieldChangeVisibleEvent += new NetFieldBase<bool, NetBool>.FieldChange(this.onActivationChanged);
    }

    public IslandWestCave1(string map, string name)
      : base(map, name)
    {
    }

    public void onActivationChanged(NetBool field, bool old_value, bool new_value) => this.updateActivationVisuals();

    protected override void resetSharedState()
    {
      base.resetSharedState();
      this.resetPuzzle();
    }

    public void resetPuzzle()
    {
      this.isActivated.Value = false;
      this.updateActivationVisuals();
      this.netPhase.Value = 3;
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      this.UpdateActivationTiles();
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (this.crystals.Count == 0)
      {
        this.crystals.Add(new IslandWestCave1.CaveCrystal()
        {
          tileLocation = new Vector2(3f, 4f),
          color = new Color(220, 0, (int) byte.MaxValue),
          currentColor = new Color(220, 0, (int) byte.MaxValue),
          id = 1,
          pitch = 0
        });
        this.crystals.Add(new IslandWestCave1.CaveCrystal()
        {
          tileLocation = new Vector2(4f, 6f),
          color = Color.Lime,
          currentColor = Color.Lime,
          id = 2,
          pitch = 700
        });
        this.crystals.Add(new IslandWestCave1.CaveCrystal()
        {
          tileLocation = new Vector2(6f, 7f),
          color = new Color((int) byte.MaxValue, 50, 100),
          currentColor = new Color((int) byte.MaxValue, 50, 100),
          id = 3,
          pitch = 1200
        });
        this.crystals.Add(new IslandWestCave1.CaveCrystal()
        {
          tileLocation = new Vector2(8f, 6f),
          color = new Color(0, 200, (int) byte.MaxValue),
          currentColor = new Color(0, 200, (int) byte.MaxValue),
          id = 4,
          pitch = 1400
        });
        this.crystals.Add(new IslandWestCave1.CaveCrystal()
        {
          tileLocation = new Vector2(9f, 4f),
          color = new Color((int) byte.MaxValue, 180, 0),
          currentColor = new Color((int) byte.MaxValue, 180, 0),
          id = 5,
          pitch = 1600
        });
      }
      this.updateActivationVisuals();
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action != null && who.IsLocalPlayer)
      {
        string[] strArray = action.Split(' ');
        if (strArray[0] == "Crystal" && (this.netPhase.Value == 5 || this.netPhase.Value == 3 || this.netPhase.Value == 2))
        {
          this.enterValueEvent.Fire(Convert.ToInt32(strArray[1]));
          return true;
        }
        if (strArray[0] == "CrystalCaveActivate" && !(bool) (NetFieldBase<bool, NetBool>) this.isActivated && !this.completed.Value)
        {
          this.isActivated.Value = true;
          Game1.playSound("openBox");
          this.updateActivationVisuals();
          this.netPhaseTimer.Value = 1200f;
          this.netPhase.Value = 0;
          this.currentDifficulty.Value = 2;
          return true;
        }
      }
      return base.performAction(action, who, tileLocation);
    }

    public virtual void updateActivationVisuals()
    {
      if (this.map == null || Game1.gameMode == (byte) 6 || Game1.currentLocation != this)
        return;
      if (this.isActivated.Value || this.completed.Value)
        Game1.currentLightSources.Add(new LightSource(1, new Vector2(6.5f, 1f) * 64f, 2f, Color.Black, 99));
      else
        Utility.removeLightSource(99);
      this.UpdateActivationTiles();
      if (!this.completed.Value)
        return;
      this.addCompletionTorches();
    }

    public virtual void UpdateActivationTiles()
    {
      if (this.map == null || Game1.gameMode == (byte) 6 || Game1.currentLocation != this)
        return;
      if (this.isActivated.Value || this.completed.Value)
        this.setMapTileIndex(6, 1, 33, "Buildings");
      else
        this.setMapTileIndex(6, 1, 31, "Buildings");
    }

    public virtual void enterValue(int which)
    {
      if (this.netPhase.Value == 2 && Game1.IsMasterGame && this.currentCrystalSequence.Count > (int) (NetFieldBase<int, NetInt>) this.currentCrystalSequenceIndex)
      {
        if (this.currentCrystalSequence[(int) (NetFieldBase<int, NetInt>) this.currentCrystalSequenceIndex] == which - 1)
        {
          ++this.currentCrystalSequenceIndex.Value;
          if ((int) (NetFieldBase<int, NetInt>) this.currentCrystalSequenceIndex >= this.currentCrystalSequence.Count)
          {
            DelayedAction.playSoundAfterDelay((int) (NetFieldBase<int, NetInt>) this.currentDifficulty == 7 ? "discoverMineral" : "newArtifact", 500, (GameLocation) this);
            this.netPhaseTimer.Value = 2000f;
            this.netPhase.Value = 4;
          }
        }
        else
        {
          this.playSound("cancel");
          this.resetPuzzle();
          ++this.timesFailed.Value;
          return;
        }
      }
      if (this.crystals.Count <= which - 1)
        return;
      this.crystals[which - 1].activate();
    }

    public override void cleanupBeforePlayerExit()
    {
      this.crystals.Clear();
      base.cleanupBeforePlayerExit();
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      this.enterValueEvent.Poll();
      if ((this.localPhase != 1 || this.currentPlaybackCrystalSequenceIndex < 0 || this.currentPlaybackCrystalSequenceIndex >= this.currentCrystalSequence.Count) && this.localPhase != this.netPhase.Value)
      {
        this.localPhaseTimer = this.netPhaseTimer.Value;
        this.localPhase = this.netPhase.Value;
        this.currentPlaybackCrystalSequenceIndex = this.localPhase == 1 ? 0 : -1;
      }
      base.UpdateWhenCurrentLocation(time);
      foreach (IslandWestCave1.CaveCrystal crystal in this.crystals)
        crystal.update();
      TimeSpan elapsedGameTime;
      if ((double) this.localPhaseTimer > 0.0)
      {
        double localPhaseTimer = (double) this.localPhaseTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.localPhaseTimer = (float) (localPhaseTimer - totalMilliseconds);
        if ((double) this.localPhaseTimer <= 0.0)
        {
          switch (this.localPhase)
          {
            case 0:
            case 4:
              this.currentPlaybackCrystalSequenceIndex = 0;
              if (Game1.IsMasterGame)
              {
                ++this.currentDifficulty.Value;
                this.currentCrystalSequence.Clear();
                this.currentCrystalSequenceIndex.Value = 0;
                if ((int) (NetFieldBase<int, NetInt>) this.currentDifficulty > ((int) (NetFieldBase<int, NetInt>) this.timesFailed < 8 ? 7 : 6))
                {
                  this.netPhaseTimer.Value = 10f;
                  this.netPhase.Value = 5;
                  break;
                }
                for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.currentDifficulty; ++index)
                  this.currentCrystalSequence.Add(Game1.random.Next(5));
                this.netPhase.Value = 1;
              }
              this.betweenNotesTimer = 600f;
              break;
            case 5:
              if (Game1.currentLocation == this)
              {
                Game1.playSound("fireball");
                Utility.addSmokePuff((GameLocation) this, new Vector2(5f, 1f) * 64f);
                Utility.addSmokePuff((GameLocation) this, new Vector2(7f, 1f) * 64f);
              }
              if (Game1.IsMasterGame)
              {
                Game1.player.team.MarkCollectedNut("IslandWestCavePuzzle");
                Game1.createObjectDebris(73, 5, 1, (GameLocation) this);
                Game1.createObjectDebris(73, 7, 1, (GameLocation) this);
                Game1.createObjectDebris(73, 6, 1, (GameLocation) this);
              }
              this.completed.Value = true;
              if (Game1.currentLocation == this)
              {
                this.addCompletionTorches();
                break;
              }
              break;
          }
        }
      }
      if (this.localPhase == 1)
      {
        double betweenNotesTimer = (double) this.betweenNotesTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.betweenNotesTimer = (float) (betweenNotesTimer - totalMilliseconds);
        if ((double) this.betweenNotesTimer > 0.0 || this.currentCrystalSequence.Count <= 0 || this.currentPlaybackCrystalSequenceIndex < 0)
          return;
        int index = this.currentCrystalSequence[this.currentPlaybackCrystalSequenceIndex];
        if (index < this.crystals.Count)
          this.crystals[index].activate();
        ++this.currentPlaybackCrystalSequenceIndex;
        int num = (int) (NetFieldBase<int, NetInt>) this.currentDifficulty;
        if ((int) (NetFieldBase<int, NetInt>) this.currentDifficulty > 5)
        {
          --num;
          if ((int) (NetFieldBase<int, NetInt>) this.timesFailed >= 4)
            --num;
          if ((int) (NetFieldBase<int, NetInt>) this.timesFailed >= 6)
            --num;
          if ((int) (NetFieldBase<int, NetInt>) this.timesFailed >= 8)
            num = 3;
        }
        else if ((int) (NetFieldBase<int, NetInt>) this.timesFailed >= 4 && (int) (NetFieldBase<int, NetInt>) this.currentDifficulty > 4)
          --num;
        this.betweenNotesTimer = 1500f / (float) num;
        if ((int) (NetFieldBase<int, NetInt>) this.currentDifficulty > ((int) (NetFieldBase<int, NetInt>) this.timesFailed < 8 ? 7 : 6))
          this.betweenNotesTimer = 100f;
        if (this.currentPlaybackCrystalSequenceIndex < this.currentCrystalSequence.Count)
          return;
        this.currentPlaybackCrystalSequenceIndex = -1;
        if ((int) (NetFieldBase<int, NetInt>) this.currentDifficulty > ((int) (NetFieldBase<int, NetInt>) this.timesFailed < 8 ? 7 : 6))
        {
          if (!Game1.IsMasterGame)
            return;
          this.netPhaseTimer.Value = 1000f;
          this.netPhase.Value = 5;
        }
        else
        {
          if (!Game1.IsMasterGame)
            return;
          this.netPhase.Value = 2;
          this.currentCrystalSequenceIndex.Value = 0;
        }
      }
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      if (!(l is IslandWestCave1))
        return;
      this.completed.Value = (l as IslandWestCave1).completed.Value;
    }

    public void addCompletionTorches()
    {
      if (!this.completed.Value)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(5f, 1f) * 64f + new Vector2(0.0f, -20f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = 0.01344f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(7f, 1f) * 64f + new Vector2(8f, -20f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = 0.01344f
      });
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      foreach (IslandWestCave1.CaveCrystal crystal in this.crystals)
        crystal.draw(b);
    }

    public class CaveCrystal
    {
      public Vector2 tileLocation;
      public int id;
      public int pitch;
      public Color color;
      public Color currentColor;
      public float shakeTimer;
      public float glowTimer;

      public void update()
      {
        if ((double) this.glowTimer > 0.0)
        {
          this.glowTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
          this.currentColor.R = (byte) Utility.Lerp((float) this.color.R, (float) byte.MaxValue, this.glowTimer / 1000f);
          this.currentColor.G = (byte) Utility.Lerp((float) this.color.G, (float) byte.MaxValue, this.glowTimer / 1000f);
          this.currentColor.B = (byte) Utility.Lerp((float) this.color.B, (float) byte.MaxValue, this.glowTimer / 1000f);
        }
        if ((double) this.shakeTimer <= 0.0)
          return;
        this.shakeTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      }

      public void activate()
      {
        this.glowTimer = 1000f;
        this.shakeTimer = 100f;
        ICue cue = Game1.soundBank.GetCue("crystal");
        cue.SetVariable("Pitch", this.pitch);
        cue.Play();
        this.currentColor = this.color;
      }

      public void draw(SpriteBatch b)
      {
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(this.tileLocation * 64f + new Vector2(8f, 10f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(188, 228, 52, 28)), this.currentColor, 0.0f, new Vector2(52f, 28f) / 2f, 4f, SpriteEffects.None, (float) (((double) this.tileLocation.Y * 64.0 + 64.0 - 8.0) / 10000.0));
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(this.tileLocation * 64f + new Vector2(0.0f, -52f) + new Vector2((double) this.shakeTimer > 0.0 ? (float) Game1.random.Next(-1, 2) : 0.0f, (double) this.shakeTimer > 0.0 ? (float) Game1.random.Next(-1, 2) : 0.0f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(240, 227, 16, 29)), this.currentColor, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.tileLocation.Y * 64.0 + 64.0 - 4.0) / 10000.0));
      }
    }
  }
}
