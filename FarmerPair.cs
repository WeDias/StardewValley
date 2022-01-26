// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmerPair
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace StardewValley
{
  public struct FarmerPair
  {
    public long Farmer1;
    public long Farmer2;

    public static FarmerPair MakePair(long f1, long f2) => new FarmerPair()
    {
      Farmer1 = Math.Min(f1, f2),
      Farmer2 = Math.Max(f1, f2)
    };

    public bool Contains(long f) => this.Farmer1 == f || this.Farmer2 == f;

    public long GetOther(long f) => this.Farmer1 == f ? this.Farmer2 : this.Farmer1;

    public bool Equals(FarmerPair other) => this.Farmer1 == other.Farmer1 && this.Farmer2 == other.Farmer2;

    public override bool Equals(object obj) => obj is FarmerPair other && this.Equals(other);

    public override int GetHashCode() => this.Farmer1.GetHashCode() ^ this.Farmer2.GetHashCode() << 16;
  }
}
