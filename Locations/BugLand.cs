// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BugLand
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System.Xml.Serialization;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class BugLand : GameLocation
  {
    [XmlElement("hasSpawnedBugsToday")]
    public bool hasSpawnedBugsToday;

    public BugLand()
    {
    }

    public BugLand(string map, string name)
      : base(map, name)
    {
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is BugLand)
        this.hasSpawnedBugsToday = (l as BugLand).hasSpawnedBugsToday;
      base.TransferDataFromSavedLocation(l);
    }

    public override void hostSetup()
    {
      base.hostSetup();
      if (!Game1.IsMasterGame || this.hasSpawnedBugsToday)
        return;
      this.InitializeBugLand();
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this.characters[index] is Grub || this.characters[index] is Fly)
        {
          this.characters.RemoveAt(index);
          --index;
        }
      }
      this.hasSpawnedBugsToday = false;
    }

    public virtual void InitializeBugLand()
    {
      if (this.hasSpawnedBugsToday)
        return;
      this.hasSpawnedBugsToday = true;
      for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
      {
        for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
        {
          if (Game1.random.NextDouble() < 0.33)
          {
            Tile tile = this.map.GetLayer("Paths").Tiles[x, y];
            if (tile != null)
            {
              Vector2 vector2 = new Vector2((float) x, (float) y);
              switch (tile.TileIndex)
              {
                case 13:
                case 14:
                case 15:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, GameLocation.getWeedForSeason(Game1.random, "spring"), 1));
                    continue;
                  }
                  continue;
                case 16:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 343 : 450, 1));
                    continue;
                  }
                  continue;
                case 17:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 343 : 450, 1));
                    continue;
                  }
                  continue;
                case 18:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 294 : 295, 1));
                    continue;
                  }
                  continue;
                case 28:
                  if (this.isTileLocationTotallyClearAndPlaceable(vector2) && this.characters.Count < 50)
                  {
                    this.characters.Add((NPC) new Grub(new Vector2(vector2.X * 64f, vector2.Y * 64f), true));
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
    }
  }
}
