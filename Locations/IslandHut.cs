// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandHut
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandHut : IslandLocation
  {
    public NetBool treeNutObtained = new NetBool();
    [XmlIgnore]
    public NetEvent0 hitTreeEvent = new NetEvent0();
    [XmlIgnore]
    public NetEvent0 parrotBoyEvent = new NetEvent0();
    [XmlIgnore]
    public bool treeHitLocal;
    [XmlElement("firstParrotDone")]
    public readonly NetBool firstParrotDone = new NetBool();
    [XmlIgnore]
    public List<string> hintDialogues = new List<string>();
    [XmlElement("hintForToday")]
    public NetString hintForToday = new NetString((string) null);
    [XmlIgnore]
    public float hintShowTime = -1f;
    [XmlIgnore]
    public float hintShakeTime = -1f;

    public override void draw(SpriteBatch b)
    {
      if (this.treeHitLocal)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(10f, 7f) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 192, 16, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
      base.draw(b);
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (!(action == "Parrot"))
        return base.performAction(action, who, tileLocation);
      this.ShowNutHint();
      return true;
    }

    public virtual int ShowNutHint()
    {
      List<KeyValuePair<string, int>> keyValuePairList = new List<KeyValuePair<string, int>>();
      int running_total1 = 0;
      int running_total2 = 0;
      if (this.MissingTheseNuts(ref running_total2, "Bush_IslandNorth_13_33", "Bush_IslandNorth_5_30"))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_VolcanoLava", 0));
      bool mail1 = Game1.MasterPlayer.hasOrWillReceiveMail("Island_UpgradeBridge");
      int running_total3 = 0;
      if (this.MissingTheseNuts(ref running_total3, "Buried_IslandNorth_19_39") && mail1)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_BuriedArch", 0));
      this.MissingTheseNuts(ref running_total2, "Bush_IslandNorth_4_42");
      this.MissingTheseNuts(ref running_total2, "Bush_IslandNorth_45_38", "Bush_IslandNorth_47_40");
      bool flag = false;
      if (this.MissingTheseNuts(ref running_total1, "IslandLeftPlantRestored", "IslandRightPlantRestored", "IslandBatRestored", "IslandFrogRestored"))
        flag = true;
      if (this.MissingTheseNuts(ref running_total1, "IslandCenterSkeletonRestored"))
      {
        running_total1 += 5;
        flag = true;
      }
      if (this.MissingTheseNuts(ref running_total1, "IslandSnakeRestored"))
      {
        running_total1 += 2;
        flag = true;
      }
      if (flag && Utility.doesAnyFarmerHaveOrWillReceiveMail("islandNorthCaveOpened"))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_Arch", 0));
      if (this.MissingTheseNuts(ref running_total3, "Buried_IslandNorth_19_13", "Buried_IslandNorth_57_79", "Buried_IslandNorth_54_21", "Buried_IslandNorth_42_77", "Buried_IslandNorth_62_54", "Buried_IslandNorth_26_81"))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_NorthBuried", running_total3));
      this.MissingTheseNuts(ref running_total2, "Bush_IslandNorth_20_26", "Bush_IslandNorth_9_84");
      this.MissingTheseNuts(ref running_total2, "Bush_IslandNorth_56_27");
      this.MissingTheseNuts(ref running_total2, "Bush_IslandSouth_31_5");
      running_total2 += running_total3;
      if (running_total2 > 0)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_NorthHidden", running_total2));
      running_total1 += running_total2;
      if (this.MissingTheseNuts(ref running_total1, "TreeNut"))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_HutTree", 0));
      bool mail2 = Game1.MasterPlayer.hasOrWillReceiveMail("Island_Turtle");
      int running_total4 = 0;
      if (this.MissingTheseNuts(ref running_total4, "IslandWestCavePuzzle"))
        running_total4 += 2;
      this.MissingTheseNuts(ref running_total4, "SandDuggy");
      if (this.MissingLimitedNutDrops(ref running_total4, "TigerSlimeNut") && mail2)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_TigerSlime", 0));
      int running_total5 = 0;
      if (this.MissingTheseNuts(ref running_total5, "Buried_IslandWest_21_81", "Buried_IslandWest_62_76", "Buried_IslandWest_39_24", "Buried_IslandWest_88_14", "Buried_IslandWest_43_74", "Buried_IslandWest_30_75"))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_WestBuried", running_total5));
      running_total4 += running_total5;
      int running_total6 = 0;
      if (this.MissingLimitedNutDrops(ref running_total6, "MusselStone", 5) && mail2)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_MusselStone", running_total6));
      running_total1 += running_total6;
      bool mail3 = Game1.MasterPlayer.hasOrWillReceiveMail("Island_UpgradeHouse");
      int running_total7 = 0;
      if (this.MissingLimitedNutDrops(ref running_total7, "IslandFarming", 5) && mail3)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_IslandFarming", running_total7));
      this.MissingTheseNuts(ref running_total4, "Bush_IslandWest_104_3", "Bush_IslandWest_31_24", "Bush_IslandWest_38_56", "Bush_IslandWest_75_29", "Bush_IslandWest_64_30");
      this.MissingTheseNuts(ref running_total4, "Bush_IslandWest_54_18", "Bush_IslandWest_25_30", "Bush_IslandWest_15_3");
      running_total1 += running_total7;
      running_total1 += running_total4;
      if (running_total4 > 0 && mail2)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_WestHidden", running_total4));
      int running_total8 = 0;
      if (this.MissingLimitedNutDrops(ref running_total8, "IslandFishing", 5))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_IslandFishing", running_total8));
      running_total1 += running_total8;
      int running_total9 = 0;
      this.MissingLimitedNutDrops(ref running_total9, "VolcanoNormalChest");
      this.MissingLimitedNutDrops(ref running_total9, "VolcanoRareChest");
      if (running_total9 > 0)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_VolcanoTreasure", running_total9));
      running_total1 += running_total9;
      int running_total10 = 0;
      if (this.MissingLimitedNutDrops(ref running_total10, "VolcanoBarrel", 5))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_VolcanoBarrel", running_total10));
      running_total1 += running_total10;
      int running_total11 = 0;
      if (this.MissingLimitedNutDrops(ref running_total11, "VolcanoMining", 5))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_VolcanoMining", running_total11));
      running_total1 += running_total11;
      int running_total12 = 0;
      if (this.MissingLimitedNutDrops(ref running_total12, "VolcanoMonsterDrop", 5))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_VolcanoMonsters", running_total12));
      running_total1 += running_total12;
      int running_total13 = 0;
      this.MissingLimitedNutDrops(ref running_total13, "Island_N_BuriedTreasureNut");
      this.MissingLimitedNutDrops(ref running_total13, "Island_W_BuriedTreasureNut");
      this.MissingLimitedNutDrops(ref running_total13, "Island_W_BuriedTreasureNut2");
      if (this.MissingTheseNuts(ref running_total13, "Mermaid"))
        running_total13 += 4;
      this.MissingTheseNuts(ref running_total13, "TreeNutShot");
      if (running_total13 > 0 && Utility.HasAnyPlayerSeenSecretNote(GameLocation.JOURNAL_INDEX + 1))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_Journal", running_total13));
      running_total1 += running_total13;
      bool mail4 = Game1.MasterPlayer.hasOrWillReceiveMail("Island_Resort");
      int running_total14 = 0;
      if (this.MissingTheseNuts(ref running_total14, "Buried_IslandSouthEastCave_36_26", "Buried_IslandSouthEast_25_17") && mail4)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_SouthEastBuried", running_total14));
      running_total1 += running_total14;
      if (this.MissingTheseNuts(ref running_total1, "StardropPool") && mail4)
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_StardropPool", 0));
      if (this.MissingTheseNuts(ref running_total1, "Bush_Caldera_28_36", "Bush_Caldera_9_34"))
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_Caldera", 0));
      this.MissingTheseNuts(ref running_total1, "Bush_CaptainRoom_2_4");
      if (this.MissingTheseNuts(ref running_total1, "BananaShrine"))
        running_total1 += 2;
      this.MissingTheseNuts(ref running_total1, "Bush_IslandEast_17_37");
      this.MissingLimitedNutDrops(ref running_total1, "Darts", 3);
      int running_total15 = 0;
      if (this.MissingTheseNuts(ref running_total15, "IslandGourmand1", "IslandGourmand2", "IslandGourmand3"))
      {
        if (Utility.doesAnyFarmerHaveOrWillReceiveMail("talkedToGourmand"))
          keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_Gourmand", 0));
        running_total15 *= 5;
      }
      running_total1 += running_total15;
      if (this.MissingTheseNuts(ref running_total1, "IslandShrinePuzzle"))
      {
        running_total1 += 4;
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_IslandShrine", 0));
      }
      this.MissingTheseNuts(ref running_total1, "Bush_IslandShrine_23_34");
      if (!(bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GoldenCoconutCracked)
      {
        ++running_total1;
        keyValuePairList.Add(new KeyValuePair<string, int>("Strings\\Locations:NutHint_GoldenCoconut", 0));
      }
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotBirdieReward"))
        running_total1 += 5;
      KeyValuePair<string, int>? nullable = new KeyValuePair<string, int>?();
      KeyValuePair<string, int> keyValuePair1;
      if (this.hintForToday.Value == null)
      {
        Random random = new Random((int) Game1.uniqueIDForThisGame + Game1.Date.TotalDays * 642);
        if (keyValuePairList.Count > 0)
        {
          nullable = new KeyValuePair<string, int>?(keyValuePairList[random.Next(keyValuePairList.Count)]);
          NetString hintForToday = this.hintForToday;
          keyValuePair1 = nullable.Value;
          string key = keyValuePair1.Key;
          hintForToday.Value = key;
        }
      }
      else
      {
        foreach (KeyValuePair<string, int> keyValuePair2 in keyValuePairList)
        {
          if (keyValuePair2.Key == this.hintForToday.Value)
          {
            nullable = new KeyValuePair<string, int>?(keyValuePair2);
            break;
          }
        }
      }
      this.hintShowTime = 1.5f;
      this.hintShakeTime = 0.5f;
      this.hintDialogues.Clear();
      this.Squawk();
      if (nullable.HasValue)
      {
        this.hintDialogues.Add(Game1.content.LoadString("Strings\\Locations:NutHint_Squawk"));
        List<string> hintDialogues = this.hintDialogues;
        LocalizedContentManager content = Game1.content;
        keyValuePair1 = nullable.Value;
        string key = keyValuePair1.Key;
        keyValuePair1 = nullable.Value;
        // ISSUE: variable of a boxed type
        __Boxed<int> sub1 = (ValueType) keyValuePair1.Value;
        string str = content.LoadString(key, (object) sub1);
        hintDialogues.Add(str);
        this.hintDialogues.Add(Game1.content.LoadString("Strings\\Locations:NutHint_Squawk"));
      }
      else
        this.hintDialogues.Add(Game1.content.LoadString("Strings\\Locations:NutHint_Squawk"));
      return running_total1;
    }

    public virtual void Squawk()
    {
      if (this.parrotUpgradePerches.Count <= 0)
        return;
      this.parrotUpgradePerches[0].ShowInsufficientNuts();
    }

    protected virtual bool MissingLimitedNutDrops(ref int running_total, string key, int count = 1)
    {
      count -= Math.Max(Game1.player.team.GetDroppedLimitedNutCount(key), 0);
      running_total += count;
      return count > 0;
    }

    protected virtual bool MissingTheseNuts(ref int running_total, params string[] keys)
    {
      int num = 0;
      foreach (string key in keys)
      {
        if (!Game1.player.team.collectedNutTracker.ContainsKey(key))
          ++num;
      }
      running_total += num;
      return num > 0;
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.hitTreeEvent.Poll();
      this.parrotBoyEvent.Poll();
      if (this.hintDialogues.Count <= 0)
        return;
      this.hintShowTime -= (float) time.ElapsedGameTime.TotalSeconds;
      this.hintShakeTime -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.hintShowTime > 0.0)
        return;
      this.hintDialogues.RemoveAt(0);
      if (this.hintDialogues.Count > 0)
      {
        this.hintShowTime = this.hintDialogues.Count != 2 ? 1.5f : 3f;
        this.hintShakeTime = 0.5f;
        this.Squawk();
      }
      else
        this.hintShowTime = -1f;
    }

    public IslandHut()
    {
    }

    public IslandHut(string map, string name)
      : base(map, name)
    {
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(7, 6), new Microsoft.Xna.Framework.Rectangle(-1000, -1000, 1, 1), 1, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_FirstParrot", true, true);
        this.firstParrotDone.Value = true;
        this.parrotBoyEvent.Fire();
      }), (Func<bool>) (() => this.firstParrotDone.Value), "Hut"));
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if (tileX == 10 && tileY == 8)
      {
        switch (t)
        {
          case Pickaxe _:
          case Axe _:
            if (!this.treeHitLocal)
            {
              this.hitTreeEvent.Fire();
              break;
            }
            break;
        }
      }
      return base.performToolAction(t, tileX, tileY);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      this.hintForToday.Value = (string) null;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.treeNutObtained, this.hitTreeEvent.NetFields, (INetSerializable) this.firstParrotDone, this.parrotBoyEvent.NetFields, (INetSerializable) this.hintForToday);
      this.hitTreeEvent.onEvent += new NetEvent0.Event(this.SpitTreeNut);
      this.parrotBoyEvent.onEvent += new NetEvent0.Event(this.ParrotBoyEvent_onEvent);
    }

    private void ParrotBoyEvent_onEvent()
    {
      if (Game1.player.currentLocation.Equals((GameLocation) this) && !Game1.IsFading())
      {
        Game1.addMailForTomorrow("sawParrotBoyIntro", true);
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandHut_Event_ParrotBoyIntro")))));
      }
      else
      {
        if (Game1.locationRequest == null || Game1.locationRequest.Location == null || !(Game1.locationRequest.Location.NameOrUniqueName == this.NameOrUniqueName) || Game1.warpingForForcedRemoteEvent)
          return;
        Game1.addMailForTomorrow("sawParrotBoyIntro", true);
        this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandHut_Event_ParrotBoyIntro")));
      }
    }

    public virtual void SpitTreeNut()
    {
      if (this.treeHitLocal)
        return;
      this.treeHitLocal = true;
      if (Game1.currentLocation == this)
      {
        Game1.playSound("boulderBreak");
        DelayedAction.playSoundAfterDelay("croak", 300);
        DelayedAction.playSoundAfterDelay("slimeHit", 1250);
        DelayedAction.playSoundAfterDelay("coin", 1250);
      }
      this.temporarySprites.Add(new TemporaryAnimatedSprite(5, new Vector2(10f, 5f) * 64f, Color.White)
      {
        motion = new Vector2(0.0f, -1.5f),
        interval = 25f,
        delayBeforeAnimationStart = 1250
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(32, 192, 16, 32), 1250f, 1, 1, new Vector2(10f, 7f) * 64f, false, false, 0.0001f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        shakeIntensity = 1f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(46, new Vector2(10f, 5f) * 64f, Color.White)
      {
        motion = new Vector2(0.0f, -3f),
        interval = 25f,
        delayBeforeAnimationStart = 1250
      });
      for (int index = 0; index < 5; ++index)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(352, 1200, 16, 16), 50f, 11, 3, new Vector2(10f, 5f) * 64f, false, false, 0.1f, 0.01f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = {
            X = Utility.RandomFloat(-3f, 3f),
            Y = Utility.RandomFloat(-1f, -3f)
          },
          acceleration = {
            Y = 0.05f
          },
          delayBeforeAnimationStart = 1250
        });
      if (!Game1.IsMasterGame || this.treeNutObtained.Value)
        return;
      Game1.player.team.MarkCollectedNut("TreeNut");
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(10.5f, 7f) * 64f, 0, (GameLocation) this, 0)), 1250);
      this.treeNutObtained.Value = true;
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is IslandHut)
      {
        IslandHut islandHut = l as IslandHut;
        this.treeNutObtained.Value = islandHut.treeNutObtained.Value;
        this.firstParrotDone.Value = islandHut.firstParrotDone.Value;
        this.hintForToday.Value = islandHut.hintForToday.Value;
      }
      base.TransferDataFromSavedLocation(l);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this.hintDialogues.Count > 0)
      {
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(7.25f, 3f) * 64f);
        if ((double) this.hintShakeTime > 0.0)
        {
          local.X += Utility.RandomFloat(-1f, 1f);
          local.Y += Utility.RandomFloat(-1f, 1f);
        }
        SpriteText.drawStringWithScrollCenteredAt(b, this.hintDialogues[0], (int) local.X, (int) local.Y, alpha: Math.Min(1f, this.hintShowTime * 2f), scrollType: 1, layerDepth: 1f);
      }
      base.drawAboveAlwaysFrontLayer(b);
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.hintDialogues.Clear();
      this.hintShowTime = -1f;
      this.treeHitLocal = this.treeNutObtained.Value;
      if (Game1.netWorldState.Value.GoldenWalnutsFound.Value < 10)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\shadow", new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 7), new Vector2(5.15f, 2.25f) * 64f, false, 0.0f, Color.White)
        {
          id = 777f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 9999f,
          animationLength = 1,
          layerDepth = 0.95f,
          drawAboveAlwaysFront = true
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\ParrotBoy", new Microsoft.Xna.Framework.Rectangle(32, 128, 16, 32), new Vector2(5f, 0.5f) * 64f, false, 0.0f, Color.White)
        {
          id = 777f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 9999f,
          animationLength = 1,
          layerDepth = 1f,
          drawAboveAlwaysFront = true
        });
      }
      if (!this.firstParrotDone.Value || Game1.MasterPlayer.hasOrWillReceiveMail("addedParrotBoy") || Game1.player.hasOrWillReceiveMail("sawParrotBoyIntro"))
        return;
      this.ParrotBoyEvent_onEvent();
    }
  }
}
