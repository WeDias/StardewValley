// Decompiled with JetBrains decompiler
// Type: StardewValley.ClearingActivity
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using xTile.Dimensions;

namespace StardewValley
{
  public class ClearingActivity : FarmActivity
  {
    protected override bool _AttemptActivity(Farm farm)
    {
      Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(0, 0, farm.map.Layers[0].LayerWidth, farm.map.Layers[0].LayerHeight);
      for (int index = 0; index < 5; ++index)
      {
        Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
        positionInThisRectangle.X = (float) (int) positionInThisRectangle.X;
        positionInThisRectangle.Y = (float) (int) positionInThisRectangle.Y;
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y, 1, 1);
        if (farm.isTileLocationTotallyClearAndPlaceableIgnoreFloors(positionInThisRectangle))
        {
          rectangle.Inflate(1, 1);
          bool flag = false;
          for (int left = rectangle.Left; left < rectangle.Right; ++left)
          {
            for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
            {
              if ((double) left != (double) positionInThisRectangle.X || (double) top != (double) positionInThisRectangle.Y)
              {
                if (!farm.isTileOnMap(new Vector2((float) left, (float) top)))
                {
                  flag = true;
                  break;
                }
                if (farm.isTileOccupiedIgnoreFloors(new Vector2((float) left, (float) top)))
                {
                  flag = true;
                  break;
                }
                if (!farm.isTilePassable(new Location(left, top), Game1.viewport))
                {
                  flag = true;
                  break;
                }
                if (farm.getBuildingAt(new Vector2((float) left, (float) top)) != null)
                {
                  flag = true;
                  break;
                }
              }
            }
            if (flag)
              break;
          }
          if (!flag)
          {
            this.activityPosition = positionInThisRectangle;
            this.activityDirection = 2;
            return true;
          }
        }
      }
      return false;
    }

    protected override void _BeginActivity()
    {
      if (this._character.Name == "Haley" && Game1.random.NextDouble() <= 0.5)
        this._character.StartActivityRouteEndBehavior("haley_photo", "");
      else
        this._character.StartActivityWalkInSquare(2, 2, 0);
    }

    protected override bool _Update(GameTime time)
    {
      if ((double) this._age > 5.0)
      {
        if (!this._character.IsReturningToEndPoint())
          this._character.EndActivityRouteEndBehavior();
        if (!this._character.IsWalkingInSquare)
          return true;
      }
      return false;
    }

    protected override void _EndActivity()
    {
    }
  }
}
