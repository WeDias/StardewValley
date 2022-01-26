// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.DwarfGate
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewValley.Locations
{
  public class DwarfGate : INetObject<NetFields>
  {
    public NetPoint tilePosition = new NetPoint();
    public NetLocationRef locationRef = new NetLocationRef();
    public bool triggeredOpen;
    public NetPointDictionary<bool, NetBool> switches = new NetPointDictionary<bool, NetBool>();
    public Dictionary<Point, bool> localSwitches = new Dictionary<Point, bool>();
    public NetBool opened = new NetBool(false);
    public bool localOpened;
    public NetInt pressedSwitches = new NetInt(0);
    public int localPressedSwitches;
    public NetInt gateIndex = new NetInt(0);
    public NetEvent0 openEvent = new NetEvent0();
    public NetEvent1Field<Point, NetPoint> pressEvent = new NetEvent1Field<Point, NetPoint>();

    public NetFields NetFields { get; } = new NetFields();

    public DwarfGate() => this.InitNetFields();

    public DwarfGate(VolcanoDungeon location, int gate_index, int x, int y, int seed)
      : this()
    {
      this.locationRef.Value = (GameLocation) location;
      this.tilePosition.X = x;
      this.tilePosition.Y = y;
      this.gateIndex.Value = gate_index;
      Random rng = new Random(seed);
      if (location.possibleSwitchPositions.ContainsKey(gate_index))
      {
        int val2 = Math.Min(location.possibleSwitchPositions[gate_index].Count, 3);
        if (gate_index > 0)
          val2 = 1;
        List<Point> list = new List<Point>((IEnumerable<Point>) location.possibleSwitchPositions[gate_index]);
        Utility.Shuffle<Point>(rng, list);
        int num = Math.Min(rng.Next(1, Math.Max(1, val2)), val2);
        if (location.isMonsterLevel())
          num = val2;
        for (int index = 0; index < num; ++index)
          this.switches[list[index]] = false;
      }
      this.UpdateLocalStates();
      this.ApplyTiles();
    }

    public virtual void InitNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.tilePosition, (INetSerializable) this.locationRef.NetFields, (INetSerializable) this.switches, (INetSerializable) this.pressedSwitches, this.openEvent.NetFields, (INetSerializable) this.opened, this.pressEvent.NetFields, (INetSerializable) this.gateIndex);
      this.pressEvent.onEvent += new AbstractNetEvent1<Point>.Event(this.OnPress);
      this.openEvent.onEvent += new NetEvent0.Event(this.OpenGate);
      this.switches.InterpolationWait = false;
      this.pressedSwitches.InterpolationWait = false;
      this.pressEvent.InterpolationWait = false;
    }

    public virtual void OnPress(Point point)
    {
      if (Game1.IsMasterGame && this.switches.ContainsKey(point) && !this.switches[point])
      {
        this.switches[point] = true;
        ++this.pressedSwitches.Value;
      }
      if (Game1.currentLocation == this.locationRef.Value)
        Game1.playSound("openBox");
      this.localSwitches[point] = true;
      this.ApplyTiles();
    }

    public virtual void OpenGate()
    {
      if (Game1.currentLocation == this.locationRef.Value)
        Game1.playSound("cowboy_gunload");
      if (Game1.IsMasterGame)
      {
        if (this.gateIndex.Value == -1 && !Game1.MasterPlayer.hasOrWillReceiveMail("volcanoShortcutUnlocked"))
          Game1.addMailForTomorrow("volcanoShortcutUnlocked", true);
        this.opened.Value = true;
      }
      this.localOpened = true;
      this.ApplyTiles();
    }

    public virtual void ResetLocalState()
    {
      this.UpdateLocalStates();
      this.ApplyTiles();
    }

    public virtual void UpdateLocalStates()
    {
      this.localOpened = this.opened.Value;
      this.localPressedSwitches = this.pressedSwitches.Value;
      foreach (Point key in this.switches.Keys)
        this.localSwitches[key] = this.switches[key];
    }

    public virtual void Draw(SpriteBatch b)
    {
      if (this.localOpened)
        return;
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.tilePosition.X, (float) this.tilePosition.Y) * 64f + new Vector2(1f, -5f) * 4f), new Rectangle?(new Rectangle(178, 189, 14, 34)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, (float) ((this.tilePosition.Y + 2) * 64) / 10000f);
    }

    public virtual void UpdateWhenCurrentLocation(GameTime time, GameLocation location)
    {
      this.openEvent.Poll();
      this.pressEvent.Poll();
      if (this.localPressedSwitches != this.pressedSwitches.Value)
      {
        this.localPressedSwitches = this.pressedSwitches.Value;
        this.ApplyTiles();
      }
      if (!this.localOpened && this.opened.Value)
      {
        this.localOpened = true;
        this.ApplyTiles();
      }
      foreach (Point key in this.switches.Keys)
      {
        if (this.switches[key] && !this.localSwitches[key])
        {
          this.localSwitches[key] = true;
          this.ApplyTiles();
        }
      }
    }

    public virtual void ApplyTiles()
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      foreach (Point key in this.localSwitches.Keys)
      {
        ++num1;
        if (this.switches[key])
          ++num3;
        if (this.localSwitches[key])
        {
          ++num2;
          this.locationRef.Value.setMapTileIndex(key.X, key.Y, VolcanoDungeon.GetTileIndex(1, 31), "Back");
          this.locationRef.Value.removeTileProperty(key.X, key.Y, "Back", "TouchAction");
        }
        else
        {
          this.locationRef.Value.setMapTileIndex(key.X, key.Y, VolcanoDungeon.GetTileIndex(0, 31), "Back");
          this.locationRef.Value.setTileProperty(key.X, key.Y, "Back", "TouchAction", "DwarfSwitch");
        }
      }
      switch (num1)
      {
        case 1:
          this.locationRef.Value.setMapTileIndex(this.tilePosition.X - 1, this.tilePosition.Y, VolcanoDungeon.GetTileIndex(10 + num2, 23), "Buildings");
          break;
        case 2:
          this.locationRef.Value.setMapTileIndex(this.tilePosition.X - 1, this.tilePosition.Y, VolcanoDungeon.GetTileIndex(12 + num2, 23), "Buildings");
          break;
        case 3:
          this.locationRef.Value.setMapTileIndex(this.tilePosition.X - 1, this.tilePosition.Y, VolcanoDungeon.GetTileIndex(10 + num2, 22), "Buildings");
          break;
      }
      if (!this.triggeredOpen && num3 >= num1)
      {
        this.triggeredOpen = true;
        if (Game1.IsMasterGame)
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => this.openEvent.Fire()), 500);
      }
      if (this.localOpened)
        this.locationRef.Value.removeTile(this.tilePosition.X, this.tilePosition.Y + 1, "Buildings");
      else
        this.locationRef.Value.setMapTileIndex(this.tilePosition.X, this.tilePosition.Y + 1, 0, "Buildings");
    }
  }
}
