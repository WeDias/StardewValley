// Decompiled with JetBrains decompiler
// Type: StardewValley.ISittable
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StardewValley
{
  public interface ISittable
  {
    bool IsSittingHere(Farmer who);

    bool HasSittingFarmers();

    void RemoveSittingFarmer(Farmer farmer);

    int GetSittingFarmerCount();

    List<Vector2> GetSeatPositions(bool ignore_offsets = false);

    Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false);

    Vector2? AddSittingFarmer(Farmer who);

    int GetSittingDirection();

    Rectangle GetSeatBounds();

    bool IsSeatHere(GameLocation location);
  }
}
