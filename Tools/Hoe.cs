// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Hoe
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewValley.Tools
{
  public class Hoe : Tool
  {
    public Hoe()
      : base(nameof (Hoe), 0, 21, 47, false)
    {
      this.UpgradeLevel = 0;
    }

    public override Item getOne()
    {
      Hoe destination = new Hoe();
      destination.UpgradeLevel = this.UpgradeLevel;
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Hoe.cs.14101");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Hoe.cs.14102");

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      if (location.Name.StartsWith("UndergroundMine"))
        power = 1;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isEfficient)
        who.Stamina -= (float) (2 * power) - (float) who.FarmingLevel * 0.1f;
      power = who.toolPower;
      who.stopJittering();
      location.playSound("woodyHit");
      Vector2 vector2_1 = new Vector2((float) (x / 64), (float) (y / 64));
      List<Vector2> vector2List = this.tilesAffected(vector2_1, power, who);
      foreach (Vector2 vector2_2 in vector2List)
      {
        vector2_2.Equals(vector2_1);
        if (location.terrainFeatures.ContainsKey(vector2_2))
        {
          if (location.terrainFeatures[vector2_2].performToolAction((Tool) this, 0, vector2_2, location))
            location.terrainFeatures.Remove(vector2_2);
        }
        else
        {
          if (location.objects.ContainsKey(vector2_2) && location.Objects[vector2_2].performToolAction((Tool) this, location))
          {
            if (location.Objects[vector2_2].type.Equals((object) "Crafting") && (int) (NetFieldBase<int, NetInt>) location.Objects[vector2_2].fragility != 2)
            {
              NetCollection<Debris> debris1 = location.debris;
              int objectIndex = (bool) (NetFieldBase<bool, NetBool>) location.Objects[vector2_2].bigCraftable ? -location.Objects[vector2_2].ParentSheetIndex : location.Objects[vector2_2].ParentSheetIndex;
              Vector2 toolLocation = who.GetToolLocation();
              Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
              double x1 = (double) boundingBox.Center.X;
              boundingBox = who.GetBoundingBox();
              double y1 = (double) boundingBox.Center.Y;
              Vector2 playerPosition = new Vector2((float) x1, (float) y1);
              Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
              debris1.Add(debris2);
            }
            location.Objects[vector2_2].performRemoveAction(vector2_2, location);
            location.Objects.Remove(vector2_2);
          }
          if (location.doesTileHaveProperty((int) vector2_2.X, (int) vector2_2.Y, "Diggable", "Back") != null)
          {
            if (location.Name.StartsWith("UndergroundMine") && !location.isTileOccupied(vector2_2))
            {
              if ((location as MineShaft).getMineArea() != 77377)
              {
                location.makeHoeDirt(vector2_2);
                location.playSound("hoeHit");
                Game1.removeSquareDebrisFromTile((int) vector2_2.X, (int) vector2_2.Y);
                location.checkForBuriedItem((int) vector2_2.X, (int) vector2_2.Y, false, false, who);
                Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(vector2_1.X * 64f, vector2_1.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
                if (vector2List.Count > 2)
                  Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(6, new Vector2(vector2_2.X * 64f, vector2_2.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: (Vector2.Distance(vector2_1, vector2_2) * 30f)));
              }
            }
            else if (!location.isTileOccupied(vector2_2) && location.isTilePassable(new Location((int) vector2_2.X, (int) vector2_2.Y), Game1.viewport))
            {
              location.makeHoeDirt(vector2_2);
              location.playSound("hoeHit");
              Game1.removeSquareDebrisFromTile((int) vector2_2.X, (int) vector2_2.Y);
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(vector2_2.X * 64f, vector2_2.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
              if (vector2List.Count > 2)
                Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(6, new Vector2(vector2_2.X * 64f, vector2_2.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: (Vector2.Distance(vector2_1, vector2_2) * 30f)));
              location.checkForBuriedItem((int) vector2_2.X, (int) vector2_2.Y, false, false, who);
            }
            ++Game1.stats.DirtHoed;
          }
        }
      }
    }
  }
}
