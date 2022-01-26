// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandShrine
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Locations
{
  public class IslandShrine : IslandForestLocation
  {
    [XmlIgnore]
    public ItemPedestal northPedestal = (ItemPedestal) (NetFieldBase<ItemPedestal, NetRef<ItemPedestal>>) new NetRef<ItemPedestal>();
    [XmlIgnore]
    public ItemPedestal southPedestal = (ItemPedestal) (NetFieldBase<ItemPedestal, NetRef<ItemPedestal>>) new NetRef<ItemPedestal>();
    [XmlIgnore]
    public ItemPedestal eastPedestal = (ItemPedestal) (NetFieldBase<ItemPedestal, NetRef<ItemPedestal>>) new NetRef<ItemPedestal>();
    [XmlIgnore]
    public ItemPedestal westPedestal = (ItemPedestal) (NetFieldBase<ItemPedestal, NetRef<ItemPedestal>>) new NetRef<ItemPedestal>();
    [XmlIgnore]
    public NetEvent0 puzzleFinishedEvent = new NetEvent0();
    [XmlElement("puzzleFinished")]
    public NetBool puzzleFinished = new NetBool();

    public IslandShrine()
    {
    }

    public IslandShrine(string map, string name)
      : base(map, name)
    {
      this.AddMissingPedestals();
    }

    public override List<Vector2> GetAdditionalWalnutBushes() => new List<Vector2>()
    {
      new Vector2(23f, 34f)
    };

    public virtual void AddMissingPedestals()
    {
      Vector2 vector2 = new Vector2(0.0f, 0.0f);
      vector2.X = 21f;
      vector2.Y = 27f;
      Object objectAtTile1 = this.getObjectAtTile((int) vector2.X, (int) vector2.Y);
      IslandGemBird.GemBirdType birdTypeForLocation1 = IslandGemBird.GetBirdTypeForLocation("IslandWest");
      if (objectAtTile1 == null)
      {
        this.westPedestal = new ItemPedestal(vector2, (Object) null, false, Color.White);
        this.objects.Add(vector2, (Object) this.westPedestal);
        this.westPedestal.requiredItem.Value = new Object(Vector2.Zero, IslandGemBird.GetItemIndex(birdTypeForLocation1), 1);
        this.westPedestal.successColor.Value = new Color(0, 0, 0, 0);
      }
      else if (objectAtTile1 is ItemPedestal)
      {
        ItemPedestal itemPedestal = objectAtTile1 as ItemPedestal;
        int itemIndex = IslandGemBird.GetItemIndex(birdTypeForLocation1);
        if (itemPedestal.requiredItem.Value == null || itemPedestal.requiredItem.Value.ParentSheetIndex != itemIndex)
        {
          itemPedestal.requiredItem.Value = new Object(Vector2.Zero, itemIndex, 1);
          if (itemPedestal.heldObject.Value != null && itemPedestal.heldObject.Value.ParentSheetIndex != itemIndex)
            itemPedestal.heldObject.Value = (Object) null;
        }
      }
      vector2.X = 27f;
      vector2.Y = 27f;
      Object objectAtTile2 = this.getObjectAtTile((int) vector2.X, (int) vector2.Y);
      IslandGemBird.GemBirdType birdTypeForLocation2 = IslandGemBird.GetBirdTypeForLocation("IslandEast");
      if (objectAtTile2 == null)
      {
        this.eastPedestal = new ItemPedestal(vector2, (Object) null, false, Color.White);
        this.objects.Add(vector2, (Object) this.eastPedestal);
        this.eastPedestal.requiredItem.Value = new Object(Vector2.Zero, IslandGemBird.GetItemIndex(birdTypeForLocation2), 1);
        this.eastPedestal.successColor.Value = new Color(0, 0, 0, 0);
      }
      else if (objectAtTile2 is ItemPedestal)
      {
        ItemPedestal itemPedestal = objectAtTile2 as ItemPedestal;
        int itemIndex = IslandGemBird.GetItemIndex(birdTypeForLocation2);
        if (itemPedestal.requiredItem.Value == null || itemPedestal.requiredItem.Value.ParentSheetIndex != itemIndex)
        {
          itemPedestal.requiredItem.Value = new Object(Vector2.Zero, itemIndex, 1);
          if (itemPedestal.heldObject.Value != null && itemPedestal.heldObject.Value.ParentSheetIndex != itemIndex)
            itemPedestal.heldObject.Value = (Object) null;
        }
      }
      vector2.X = 24f;
      vector2.Y = 28f;
      Object objectAtTile3 = this.getObjectAtTile((int) vector2.X, (int) vector2.Y);
      IslandGemBird.GemBirdType birdTypeForLocation3 = IslandGemBird.GetBirdTypeForLocation("IslandSouth");
      if (objectAtTile3 == null)
      {
        this.southPedestal = new ItemPedestal(vector2, (Object) null, false, Color.White);
        this.objects.Add(vector2, (Object) this.southPedestal);
        this.southPedestal.requiredItem.Value = new Object(Vector2.Zero, IslandGemBird.GetItemIndex(birdTypeForLocation3), 1);
        this.southPedestal.successColor.Value = new Color(0, 0, 0, 0);
      }
      else if (objectAtTile3 is ItemPedestal)
      {
        ItemPedestal itemPedestal = objectAtTile3 as ItemPedestal;
        int itemIndex = IslandGemBird.GetItemIndex(birdTypeForLocation3);
        if (itemPedestal.requiredItem.Value == null || itemPedestal.requiredItem.Value.ParentSheetIndex != itemIndex)
        {
          itemPedestal.requiredItem.Value = new Object(Vector2.Zero, itemIndex, 1);
          if (itemPedestal.heldObject.Value != null && itemPedestal.heldObject.Value.ParentSheetIndex != itemIndex)
            itemPedestal.heldObject.Value = (Object) null;
        }
      }
      vector2.X = 24f;
      vector2.Y = 25f;
      Object objectAtTile4 = this.getObjectAtTile((int) vector2.X, (int) vector2.Y);
      IslandGemBird.GemBirdType birdTypeForLocation4 = IslandGemBird.GetBirdTypeForLocation("IslandNorth");
      if (objectAtTile4 == null)
      {
        this.northPedestal = new ItemPedestal(vector2, (Object) null, false, Color.White);
        this.objects.Add(vector2, (Object) this.northPedestal);
        this.northPedestal.requiredItem.Value = new Object(Vector2.Zero, IslandGemBird.GetItemIndex(birdTypeForLocation4), 1);
        this.northPedestal.successColor.Value = new Color(0, 0, 0, 0);
      }
      else
      {
        if (!(objectAtTile4 is ItemPedestal))
          return;
        ItemPedestal itemPedestal = objectAtTile4 as ItemPedestal;
        int itemIndex = IslandGemBird.GetItemIndex(birdTypeForLocation4);
        if (itemPedestal.requiredItem.Value != null && itemPedestal.requiredItem.Value.ParentSheetIndex == itemIndex)
          return;
        itemPedestal.requiredItem.Value = new Object(Vector2.Zero, itemIndex, 1);
        if (itemPedestal.heldObject.Value == null || itemPedestal.heldObject.Value.ParentSheetIndex == itemIndex)
          return;
        itemPedestal.heldObject.Value = (Object) null;
      }
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.puzzleFinished, (INetSerializable) this.puzzleFinishedEvent);
      this.puzzleFinishedEvent.onEvent += new NetEvent0.Event(this.OnPuzzleFinish);
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (!Game1.IsMasterGame)
        return;
      this.AddMissingPedestals();
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (!this.puzzleFinished.Value)
        return;
      this.ApplyFinishedTiles();
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      if (!(l is IslandShrine))
        return;
      IslandShrine islandShrine = l as IslandShrine;
      this.northPedestal = islandShrine.getObjectAtTile((int) this.northPedestal.TileLocation.X, (int) this.northPedestal.TileLocation.Y) as ItemPedestal;
      this.southPedestal = islandShrine.getObjectAtTile((int) this.southPedestal.TileLocation.X, (int) this.southPedestal.TileLocation.Y) as ItemPedestal;
      this.eastPedestal = islandShrine.getObjectAtTile((int) this.eastPedestal.TileLocation.X, (int) this.eastPedestal.TileLocation.Y) as ItemPedestal;
      this.westPedestal = islandShrine.getObjectAtTile((int) this.westPedestal.TileLocation.X, (int) this.westPedestal.TileLocation.Y) as ItemPedestal;
      this.puzzleFinished.Value = islandShrine.puzzleFinished.Value;
    }

    public void OnPuzzleFinish()
    {
      if (Game1.IsMasterGame)
      {
        Game1.createItemDebris((Item) new Object(73, 1), new Vector2(24f, 19f) * 64f, -1, (GameLocation) this);
        Game1.createItemDebris((Item) new Object(73, 1), new Vector2(24f, 19f) * 64f, -1, (GameLocation) this);
        Game1.createItemDebris((Item) new Object(73, 1), new Vector2(24f, 19f) * 64f, -1, (GameLocation) this);
        Game1.createItemDebris((Item) new Object(73, 1), new Vector2(24f, 19f) * 64f, -1, (GameLocation) this);
        Game1.createItemDebris((Item) new Object(73, 1), new Vector2(24f, 19f) * 64f, -1, (GameLocation) this);
      }
      if (Game1.currentLocation != this)
        return;
      Game1.playSound("boulderBreak");
      Game1.playSound("secret1");
      Game1.flashAlpha = 1f;
      this.ApplyFinishedTiles();
    }

    public virtual void ApplyFinishedTiles()
    {
      this.setMapTileIndex(23, 19, 142, "AlwaysFront", 2);
      this.setMapTileIndex(24, 19, 143, "AlwaysFront", 2);
      this.setMapTileIndex(25, 19, 144, "AlwaysFront", 2);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if (!Game1.IsMasterGame || this.puzzleFinished.Value || !this.northPedestal.match.Value || !this.southPedestal.match.Value || !this.eastPedestal.match.Value || !this.westPedestal.match.Value)
        return;
      Game1.player.team.MarkCollectedNut("IslandShrinePuzzle");
      this.puzzleFinishedEvent.Fire();
      this.puzzleFinished.Value = true;
      this.northPedestal.locked.Value = true;
      this.northPedestal.heldObject.Value = (Object) null;
      this.southPedestal.locked.Value = true;
      this.southPedestal.heldObject.Value = (Object) null;
      this.eastPedestal.locked.Value = true;
      this.eastPedestal.heldObject.Value = (Object) null;
      this.westPedestal.locked.Value = true;
      this.westPedestal.heldObject.Value = (Object) null;
    }
  }
}
