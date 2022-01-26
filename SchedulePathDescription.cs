// Decompiled with JetBrains decompiler
// Type: StardewValley.SchedulePathDescription
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StardewValley
{
  public class SchedulePathDescription
  {
    public Stack<Point> route;
    public int facingDirection;
    public string endOfRouteBehavior;
    public string endOfRouteMessage;

    public SchedulePathDescription(
      Stack<Point> route,
      int facingDirection,
      string endBehavior,
      string endMessage)
    {
      this.endOfRouteMessage = endMessage;
      this.route = route;
      this.facingDirection = facingDirection;
      this.endOfRouteBehavior = endBehavior;
    }
  }
}
