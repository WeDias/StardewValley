// Decompiled with JetBrains decompiler
// Type: TilePositionComparer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class TilePositionComparer : IEqualityComparer<Vector2>
{
  public bool Equals(Vector2 a, Vector2 b) => a.Equals(b);

  public int GetHashCode(Vector2 a) => (int) (ushort) a.X | (int) (ushort) a.Y << 16;
}
