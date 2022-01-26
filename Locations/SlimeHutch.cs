// Decompiled with JetBrains decompiler
// Type: StardewValley.SlimeHutch
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

namespace StardewValley
{
  public class SlimeHutch : GameLocation
  {
    [XmlElement("slimeMatingsLeft")]
    public readonly NetInt slimeMatingsLeft = new NetInt();
    public readonly NetArray<bool, NetBool> waterSpots = new NetArray<bool, NetBool>(4);

    public SlimeHutch()
    {
    }

    public SlimeHutch(string m, string name)
      : base(m, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.slimeMatingsLeft, (INetSerializable) this.waterSpots);
    }

    public void updateWhenNotCurrentLocation(Building parentBuilding, GameTime time)
    {
    }

    public bool isFull() => this.characters.Count >= 20;

    public Building getBuilding()
    {
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value.Equals((GameLocation) this))
          return building;
      }
      return (Building) null;
    }

    public override bool canSlimeMateHere()
    {
      int slimeMatingsLeft = (int) (NetFieldBase<int, NetInt>) this.slimeMatingsLeft;
      --this.slimeMatingsLeft.Value;
      return !this.isFull() && slimeMatingsLeft > 0;
    }

    public override bool canSlimeHatchHere() => !this.isFull();

    public override void DayUpdate(int dayOfMonth)
    {
      int val2 = 0;
      int num1 = Game1.random.Next(this.waterSpots.Length);
      for (int index = 0; index < this.waterSpots.Length; ++index)
      {
        if (this.waterSpots[(index + num1) % this.waterSpots.Length] && val2 * 5 < this.characters.Count)
        {
          ++val2;
          this.waterSpots[(index + num1) % this.waterSpots.Length] = false;
        }
      }
      for (int index = this.objects.Count() - 1; index >= 0; --index)
      {
        if (this.objects.Pairs.ElementAt(index).Value.IsSprinkler())
        {
          foreach (Vector2 sprinklerTile in this.objects.Pairs.ElementAt(index).Value.GetSprinklerTiles())
          {
            if ((double) sprinklerTile.X == 16.0 && (double) sprinklerTile.Y >= 6.0 && (double) sprinklerTile.Y <= 9.0)
              this.waterSpots[(int) sprinklerTile.Y - 6] = true;
          }
        }
      }
      for (int index = Math.Min(this.characters.Count / 5, val2); index > 0; --index)
      {
        int num2 = 50;
        Vector2 randomTile;
        for (randomTile = this.getRandomTile(); (!this.isTileLocationTotallyClearAndPlaceable(randomTile) || this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "NPCBarrier", "Back") != null || (double) randomTile.Y >= 12.0) && num2 > 0; --num2)
          randomTile = this.getRandomTile();
        if (num2 > 0)
          this.objects.Add(randomTile, new Object(randomTile, 56));
      }
      while ((int) (NetFieldBase<int, NetInt>) this.slimeMatingsLeft > 0)
      {
        if (this.characters.Count > 1 && !this.isFull())
        {
          NPC character = this.characters[Game1.random.Next(this.characters.Count)];
          if (character is GreenSlime)
          {
            GreenSlime greenSlime = character as GreenSlime;
            if ((int) (NetFieldBase<int, NetInt>) greenSlime.ageUntilFullGrown <= 0)
            {
              for (int index = 1; index < 10; ++index)
              {
                GreenSlime mateToPursue = (GreenSlime) Utility.checkForCharacterWithinArea(greenSlime.GetType(), character.Position, (GameLocation) this, new Rectangle((int) greenSlime.Position.X - 64 * index, (int) greenSlime.Position.Y - 64 * index, 64 * (index * 2 + 1), 64 * (index * 2 + 1)));
                if (mateToPursue != null && (NetFieldBase<bool, NetBool>) mateToPursue.cute != greenSlime.cute && (int) (NetFieldBase<int, NetInt>) mateToPursue.ageUntilFullGrown <= 0)
                {
                  greenSlime.mateWith(mateToPursue, (GameLocation) this);
                  break;
                }
              }
            }
          }
        }
        --this.slimeMatingsLeft.Value;
      }
      this.slimeMatingsLeft.Value = this.characters.Count / 5 + 1;
      base.DayUpdate(dayOfMonth);
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is SlimeHutch)
      {
        for (int index = 0; index < this.waterSpots.Length; ++index)
        {
          if (index < (l as SlimeHutch).waterSpots.Count)
            this.waterSpots[index] = (l as SlimeHutch).waterSpots[index];
        }
      }
      base.TransferDataFromSavedLocation(l);
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if (t is WateringCan && tileX == 16 && tileY >= 6 && tileY <= 9)
        this.waterSpots[tileY - 6] = true;
      return false;
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      for (int index = 0; index < this.waterSpots.Length; ++index)
      {
        if (this.waterSpots[index])
          this.setMapTileIndex(16, 6 + index, 2135, "Buildings");
        else
          this.setMapTileIndex(16, 6 + index, 2134, "Buildings");
      }
    }
  }
}
