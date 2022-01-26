// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Sword
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System;

namespace StardewValley.Tools
{
  public class Sword : Tool
  {
    public const double baseCritChance = 0.02;
    public int whichUpgrade;

    public Sword()
    {
    }

    public Sword(string name, int spriteIndex)
      : base(name, 0, spriteIndex, spriteIndex, false)
    {
    }

    public override Item getOne()
    {
      Sword one = new Sword(this.BaseName, this.InitialParentTileIndex);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public void DoFunction(
      GameLocation location,
      int x,
      int y,
      int facingDirection,
      int power,
      Farmer who)
    {
      this.DoFunction(location, x, y, power, who);
      Vector2 vector2_1 = Vector2.Zero;
      Vector2 vector2_2 = Vector2.Zero;
      Rectangle rectangle = Rectangle.Empty;
      Rectangle boundingBox = who.GetBoundingBox();
      switch (facingDirection)
      {
        case 0:
          rectangle = new Rectangle(x - 64, boundingBox.Y - 64, 128, 64);
          vector2_1 = new Vector2((float) ((Game1.random.NextDouble() < 0.5 ? rectangle.Left : rectangle.Right) / 64), (float) (rectangle.Top / 64));
          vector2_2 = new Vector2((float) (rectangle.Center.X / 64), (float) (rectangle.Top / 64));
          break;
        case 1:
          rectangle = new Rectangle(boundingBox.Right, y - 64, 64, 128);
          vector2_1 = new Vector2((float) (rectangle.Center.X / 64), (float) ((Game1.random.NextDouble() < 0.5 ? rectangle.Top : rectangle.Bottom) / 64));
          vector2_2 = new Vector2((float) (rectangle.Center.X / 64), (float) (rectangle.Center.Y / 64));
          break;
        case 2:
          rectangle = new Rectangle(x - 64, boundingBox.Bottom, 128, 64);
          vector2_1 = new Vector2((float) ((Game1.random.NextDouble() < 0.5 ? rectangle.Left : rectangle.Right) / 64), (float) (rectangle.Center.Y / 64));
          vector2_2 = new Vector2((float) (rectangle.Center.X / 64), (float) (rectangle.Center.Y / 64));
          break;
        case 3:
          rectangle = new Rectangle(boundingBox.Left - 64, y - 64, 64, 128);
          vector2_1 = new Vector2((float) (rectangle.Left / 64), (float) ((Game1.random.NextDouble() < 0.5 ? rectangle.Top : rectangle.Bottom) / 64));
          vector2_2 = new Vector2((float) (rectangle.Left / 64), (float) (rectangle.Center.Y / 64));
          break;
      }
      int minDamage = (this.whichUpgrade == 2 ? 3 : (this.whichUpgrade == 4 ? 6 : this.whichUpgrade)) + 1;
      int maxDamage = 4 * ((this.whichUpgrade == 2 ? 3 : (this.whichUpgrade == 4 ? 5 : this.whichUpgrade)) + 1);
      bool flag1 = location.damageMonster(rectangle, minDamage, maxDamage, false, who);
      if (this.whichUpgrade == 4 && !flag1)
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(352, (float) Game1.random.Next(50, 120), 2, 1, new Vector2((float) (rectangle.Center.X - 32), (float) (rectangle.Center.Y - 32)) + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-32, 32)), false, Game1.random.NextDouble() < 0.5));
      string cueName = "";
      if (!flag1)
      {
        if (location.objects.ContainsKey(vector2_1) && !location.Objects[vector2_1].Name.Contains("Stone") && !location.Objects[vector2_1].Name.Contains("Stick") && !location.Objects[vector2_1].Name.Contains("Stump") && !location.Objects[vector2_1].Name.Contains("Boulder") && !location.Objects[vector2_1].Name.Contains("Lumber") && !location.Objects[vector2_1].IsHoeDirt)
        {
          if (location.Objects[vector2_1].Name.Contains("Weed"))
          {
            if ((double) who.Stamina <= 0.0)
              return;
            ++Game1.stats.WeedsEliminated;
            this.checkWeedForTreasure(vector2_1, who);
            cueName = location.Objects[vector2_1].Category != -2 ? "cut" : "stoneCrack";
            location.removeObject(vector2_1, true);
          }
          else
            location.objects[vector2_1].performToolAction((Tool) this, location);
        }
        if (location.objects.ContainsKey(vector2_2) && !location.Objects[vector2_2].Name.Contains("Stone") && !location.Objects[vector2_2].Name.Contains("Stick") && !location.Objects[vector2_2].Name.Contains("Stump") && !location.Objects[vector2_2].Name.Contains("Boulder") && !location.Objects[vector2_2].Name.Contains("Lumber") && !location.Objects[vector2_2].IsHoeDirt)
        {
          if (location.Objects[vector2_2].Name.Contains("Weed"))
          {
            if ((double) who.Stamina <= 0.0)
              return;
            ++Game1.stats.WeedsEliminated;
            this.checkWeedForTreasure(vector2_2, who);
          }
          else
            location.objects[vector2_2].performToolAction((Tool) this, location);
        }
      }
      bool flag2 = false;
      foreach (Vector2 vector2_3 in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(rectangle))
      {
        if (location.terrainFeatures.ContainsKey(vector2_3) && location.terrainFeatures[vector2_3].performToolAction((Tool) this, 0, vector2_3, location))
        {
          location.terrainFeatures.Remove(vector2_3);
          flag2 = true;
        }
      }
      int num = flag2 ? 1 : 0;
      if (!cueName.Equals(""))
        Game1.playSound(cueName);
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
    }

    public void checkWeedForTreasure(Vector2 tileLocation, Farmer who)
    {
      Random random = new Random((int) ((double) (Game1.uniqueIDForThisGame + (ulong) Game1.stats.DaysPlayed) + (double) tileLocation.X * 13.0 + (double) tileLocation.Y * 29.0));
      if (random.NextDouble() < 0.07)
        Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, random.Next(1, 3));
      else if (random.NextDouble() < 0.02 + (double) who.LuckLevel / 10.0)
      {
        Game1.createDebris(random.NextDouble() < 0.5 ? 4 : 8, (int) tileLocation.X, (int) tileLocation.Y, random.Next(1, 4));
      }
      else
      {
        if (random.NextDouble() >= 0.006 + (double) who.LuckLevel / 20.0)
          return;
        Game1.createObjectDebris(114, (int) tileLocation.X, (int) tileLocation.Y);
      }
    }

    protected override string loadDisplayName()
    {
      if (this.Name.Equals("Battered Sword"))
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1205");
      switch (this.whichUpgrade)
      {
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14292");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14294");
        case 4:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14296");
        default:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14290");
      }
    }

    protected override string loadDescription()
    {
      switch (this.whichUpgrade)
      {
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14291");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14293");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14295");
        case 4:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Sword.cs.14297");
        default:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1206");
      }
    }

    public void upgrade(int which)
    {
      if (which > this.whichUpgrade)
      {
        this.whichUpgrade = which;
        switch (which)
        {
          case 1:
            this.Name = "Hero's Sword";
            this.IndexOfMenuItemView = 68;
            break;
          case 2:
            this.Name = "Holy Sword";
            this.IndexOfMenuItemView = 70;
            break;
          case 3:
            this.Name = "Dark Sword";
            this.IndexOfMenuItemView = 69;
            break;
          case 4:
            this.Name = "Galaxy Sword";
            this.IndexOfMenuItemView = 71;
            break;
        }
        this.displayName = (string) null;
        this.description = (string) null;
        this.UpgradeLevel = which;
      }
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
    }
  }
}
