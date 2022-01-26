// Decompiled with JetBrains decompiler
// Type: StardewValley.PathNode
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace StardewValley
{
  public class PathNode : IEquatable<PathNode>
  {
    public readonly int x;
    public readonly int y;
    public readonly int id;
    public byte g;
    public PathNode parent;

    public PathNode(int x, int y, PathNode parent)
    {
      this.x = x;
      this.y = y;
      this.parent = parent;
      this.id = PathNode.ComputeHash(x, y);
    }

    public PathNode(int x, int y, byte g, PathNode parent)
    {
      this.x = x;
      this.y = y;
      this.g = g;
      this.parent = parent;
      this.id = PathNode.ComputeHash(x, y);
    }

    public bool Equals(PathNode obj) => obj != null && this.x == obj.x && this.y == obj.y;

    public override bool Equals(object obj) => obj is PathNode pathNode && this.x == pathNode.x && this.y == pathNode.y;

    public override int GetHashCode() => this.id;

    public static int ComputeHash(int x, int y) => 100000 * x + y;
  }
}
