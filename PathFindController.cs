// Decompiled with JetBrains decompiler
// Type: StardewValley.PathFindController
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Threading;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley
{
  /// This class finds a path from one point to another using the A* pathfinding algorithm. Then it will guide the given character along that path.
  ///             Can only be used on maps where the tile width and height are each 127 or less.
  [InstanceStatics]
  public class PathFindController
  {
    public const byte impassable = 255;
    public const int timeToWaitBeforeCancelling = 5000;
    private Character character;
    public GameLocation location;
    public Stack<Point> pathToEndPoint;
    public Point endPoint;
    public int finalFacingDirection;
    public int pausedTimer;
    public int limit;
    private PathFindController.isAtEnd endFunction;
    public PathFindController.endBehavior endBehaviorFunction;
    public bool nonDestructivePathing;
    public bool allowPlayerPathingInEvent;
    public bool NPCSchedule;
    private static readonly sbyte[,] Directions = new sbyte[4, 2]
    {
      {
        (sbyte) -1,
        (sbyte) 0
      },
      {
        (sbyte) 1,
        (sbyte) 0
      },
      {
        (sbyte) 0,
        (sbyte) 1
      },
      {
        (sbyte) 0,
        (sbyte) -1
      }
    };
    private static PriorityQueue _openList = new PriorityQueue();
    private static HashSet<int> _closedList = new HashSet<int>();
    private static int _counter = 0;
    public int timerSinceLastCheckPoint;

    public PathFindController(
      Character c,
      GameLocation location,
      Point endPoint,
      int finalFacingDirection)
      : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, false, (PathFindController.endBehavior) null, 10000, endPoint)
    {
    }

    public PathFindController(
      Character c,
      GameLocation location,
      Point endPoint,
      int finalFacingDirection,
      PathFindController.endBehavior endBehaviorFunction)
      : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, false, (PathFindController.endBehavior) null, 10000, endPoint)
    {
      this.endPoint = endPoint;
      this.endBehaviorFunction = endBehaviorFunction;
    }

    public PathFindController(
      Character c,
      GameLocation location,
      Point endPoint,
      int finalFacingDirection,
      PathFindController.endBehavior endBehaviorFunction,
      int limit)
      : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, false, (PathFindController.endBehavior) null, limit, endPoint)
    {
      this.endPoint = endPoint;
      this.endBehaviorFunction = endBehaviorFunction;
    }

    public PathFindController(
      Character c,
      GameLocation location,
      Point endPoint,
      int finalFacingDirection,
      bool eraseOldPathController,
      bool clearMarriageDialogues = true)
      : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, eraseOldPathController, (PathFindController.endBehavior) null, 10000, endPoint, clearMarriageDialogues)
    {
    }

    public static bool isAtEndPoint(
      PathNode currentNode,
      Point endPoint,
      GameLocation location,
      Character c)
    {
      return currentNode.x == endPoint.X && currentNode.y == endPoint.Y;
    }

    public PathFindController(
      Stack<Point> pathToEndPoint,
      GameLocation location,
      Character c,
      Point endPoint)
    {
      this.pathToEndPoint = pathToEndPoint;
      this.location = location;
      this.character = c;
      this.endPoint = endPoint;
    }

    public PathFindController(Stack<Point> pathToEndPoint, Character c, GameLocation l)
    {
      this.pathToEndPoint = pathToEndPoint;
      this.character = c;
      this.location = l;
      this.NPCSchedule = true;
    }

    public PathFindController(
      Character c,
      GameLocation location,
      PathFindController.isAtEnd endFunction,
      int finalFacingDirection,
      bool eraseOldPathController,
      PathFindController.endBehavior endBehaviorFunction,
      int limit,
      Point endPoint,
      bool clearMarriageDialogues = true)
    {
      this.limit = limit;
      this.character = c;
      if (c is NPC npc && npc.CurrentDialogue.Count > 0 && npc.CurrentDialogue.Peek().removeOnNextMove)
        npc.CurrentDialogue.Pop();
      if (npc != null & clearMarriageDialogues)
      {
        if (npc.currentMarriageDialogue.Count > 0)
          npc.currentMarriageDialogue.Clear();
        npc.shouldSayMarriageDialogue.Value = false;
      }
      this.location = location;
      this.endFunction = endFunction == null ? new PathFindController.isAtEnd(PathFindController.isAtEndPoint) : endFunction;
      this.endBehaviorFunction = endBehaviorFunction;
      if (endPoint == Point.Zero)
        endPoint = new Point((int) c.getTileLocation().X, (int) c.getTileLocation().Y);
      this.finalFacingDirection = finalFacingDirection;
      if (!(this.character is NPC) && !this.isPlayerPresent() && endFunction == new PathFindController.isAtEnd(PathFindController.isAtEndPoint) && endPoint.X > 0 && endPoint.Y > 0)
      {
        this.character.Position = new Vector2((float) (endPoint.X * 64), (float) (endPoint.Y * 64 - 32));
      }
      else
      {
        this.pathToEndPoint = PathFindController.findPath(new Point((int) c.getTileLocation().X, (int) c.getTileLocation().Y), endPoint, endFunction, location, this.character, limit);
        if (this.pathToEndPoint != null)
          return;
        FarmHouse farmHouse = location as FarmHouse;
      }
    }

    public bool isPlayerPresent() => this.location.farmers.Any();

    public bool update(GameTime time)
    {
      if (this.pathToEndPoint == null || this.pathToEndPoint.Count == 0)
        return true;
      if (!this.NPCSchedule && !this.isPlayerPresent() && this.endPoint.X > 0 && this.endPoint.Y > 0)
      {
        this.character.Position = new Vector2((float) (this.endPoint.X * 64), (float) (this.endPoint.Y * 64 - 32));
        return true;
      }
      if (Game1.activeClickableMenu == null || Game1.IsMultiplayer)
      {
        int sinceLastCheckPoint = this.timerSinceLastCheckPoint;
        TimeSpan elapsedGameTime = time.ElapsedGameTime;
        int milliseconds1 = elapsedGameTime.Milliseconds;
        this.timerSinceLastCheckPoint = sinceLastCheckPoint + milliseconds1;
        Vector2 position = this.character.Position;
        this.moveCharacter(time);
        if (this.character.Position.Equals(position))
        {
          int pausedTimer = this.pausedTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds2 = elapsedGameTime.Milliseconds;
          this.pausedTimer = pausedTimer + milliseconds2;
        }
        else
          this.pausedTimer = 0;
        if (!this.NPCSchedule && this.pausedTimer > 5000)
          return true;
      }
      return false;
    }

    public static Stack<Point> findPath(
      Point startPoint,
      Point endPoint,
      PathFindController.isAtEnd endPointFunction,
      GameLocation location,
      Character character,
      int limit)
    {
      if (Interlocked.Increment(ref PathFindController._counter) != 1)
        throw new Exception();
      try
      {
        bool flag = false;
        if (character is FarmAnimal && (character as FarmAnimal).type.Value == "Duck" && (character as FarmAnimal).isSwimming.Value)
          flag = true;
        PathFindController._openList.Clear();
        PathFindController._closedList.Clear();
        PriorityQueue openList = PathFindController._openList;
        HashSet<int> closedList = PathFindController._closedList;
        int num1 = 0;
        openList.Enqueue(new PathNode(startPoint.X, startPoint.Y, (byte) 0, (PathNode) null), Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
        int layerWidth = location.map.Layers[0].LayerWidth;
        int layerHeight = location.map.Layers[0].LayerHeight;
        while (!openList.IsEmpty())
        {
          PathNode pathNode1 = openList.Dequeue();
          if (endPointFunction(pathNode1, endPoint, location, character))
            return PathFindController.reconstructPath(pathNode1);
          closedList.Add(pathNode1.id);
          int num2 = (int) (byte) ((uint) pathNode1.g + 1U);
          for (int index = 0; index < 4; ++index)
          {
            int x = pathNode1.x + (int) PathFindController.Directions[index, 0];
            int y = pathNode1.y + (int) PathFindController.Directions[index, 1];
            int hash = PathNode.ComputeHash(x, y);
            if (!closedList.Contains(hash))
            {
              if ((x != endPoint.X || y != endPoint.Y) && (x < 0 || y < 0 || x >= layerWidth || y >= layerHeight))
              {
                closedList.Add(hash);
              }
              else
              {
                PathNode pathNode2 = new PathNode(x, y, pathNode1);
                pathNode2.g = (byte) ((uint) pathNode1.g + 1U);
                if (!flag && location.isCollidingPosition(new Rectangle(pathNode2.x * 64 + 1, pathNode2.y * 64 + 1, 62, 62), Game1.viewport, character is Farmer, 0, false, character, true))
                {
                  closedList.Add(hash);
                }
                else
                {
                  int priority = num2 + (Math.Abs(endPoint.X - x) + Math.Abs(endPoint.Y - y));
                  closedList.Add(hash);
                  openList.Enqueue(pathNode2, priority);
                }
              }
            }
          }
          ++num1;
          if (num1 >= limit)
            return (Stack<Point>) null;
        }
        return (Stack<Point>) null;
      }
      finally
      {
        if (Interlocked.Decrement(ref PathFindController._counter) != 0)
          throw new Exception();
      }
    }

    public static Stack<Point> reconstructPath(PathNode finalNode)
    {
      Stack<Point> pointStack = new Stack<Point>();
      pointStack.Push(new Point(finalNode.x, finalNode.y));
      for (PathNode parent = finalNode.parent; parent != null; parent = parent.parent)
        pointStack.Push(new Point(parent.x, parent.y));
      return pointStack;
    }

    private byte[,] createMapGrid(GameLocation location, Point endPoint)
    {
      byte[,] mapGrid = new byte[location.map.Layers[0].LayerWidth, location.map.Layers[0].LayerHeight];
      for (int index1 = 0; index1 < location.map.Layers[0].LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < location.map.Layers[0].LayerHeight; ++index2)
          mapGrid[index1, index2] = location.isCollidingPosition(new Rectangle(index1 * 64 + 1, index2 * 64 + 1, 62, 62), Game1.viewport, false, 0, false, this.character, true) ? byte.MaxValue : (byte) (Math.Abs(endPoint.X - index1) + Math.Abs(endPoint.Y - index2));
      }
      return mapGrid;
    }

    private void moveCharacter(GameTime time)
    {
      Point point = this.pathToEndPoint.Peek();
      Rectangle rectangle = new Rectangle(point.X * 64, point.Y * 64, 64, 64);
      rectangle.Inflate(-2, 0);
      Rectangle boundingBox = this.character.GetBoundingBox();
      if ((rectangle.Contains(boundingBox) || boundingBox.Width > rectangle.Width && rectangle.Contains(boundingBox.Center)) && rectangle.Bottom - boundingBox.Bottom >= 2)
      {
        this.timerSinceLastCheckPoint = 0;
        this.pathToEndPoint.Pop();
        this.character.stopWithoutChangingFrame();
        if (this.pathToEndPoint.Count != 0)
          return;
        this.character.Halt();
        if (this.finalFacingDirection != -1)
          this.character.faceDirection(this.finalFacingDirection);
        if (this.NPCSchedule)
        {
          NPC character = this.character as NPC;
          character.DirectionsToNewLocation = (SchedulePathDescription) null;
          character.endOfRouteMessage.Value = character.nextEndOfRouteMessage;
        }
        if (this.endBehaviorFunction == null)
          return;
        this.endBehaviorFunction(this.character, this.location);
      }
      else
      {
        if (this.character is Farmer character1)
          character1.movementDirections.Clear();
        else if (!(this.location is MovieTheater))
        {
          string name = this.character.Name;
          foreach (NPC character in this.location.characters)
          {
            if (!character.Equals((object) this.character) && character.GetBoundingBox().Intersects(boundingBox) && character.isMoving() && string.Compare(character.Name, name, StringComparison.Ordinal) < 0)
            {
              this.character.Halt();
              return;
            }
          }
        }
        if (boundingBox.Left < rectangle.Left && boundingBox.Right < rectangle.Right)
          this.character.SetMovingRight(true);
        else if (boundingBox.Right > rectangle.Right && boundingBox.Left > rectangle.Left)
          this.character.SetMovingLeft(true);
        else if (boundingBox.Top <= rectangle.Top)
          this.character.SetMovingDown(true);
        else if (boundingBox.Bottom >= rectangle.Bottom - 2)
          this.character.SetMovingUp(true);
        this.character.MovePosition(time, Game1.viewport, this.location);
        if (this.nonDestructivePathing)
        {
          if (rectangle.Intersects(this.character.nextPosition((int) this.character.facingDirection)))
          {
            Vector2 vector2 = this.character.nextPositionVector2();
            Object objectAt = this.location.getObjectAt((int) vector2.X, (int) vector2.Y);
            if (objectAt != null)
            {
              if (objectAt is Fence fence && (bool) (NetFieldBase<bool, NetBool>) fence.isGate)
                fence.toggleGate(this.location, true);
              else if (!objectAt.isPassable())
              {
                this.character.Halt();
                this.character.controller = (PathFindController) null;
                return;
              }
            }
          }
          this.handleWarps(this.character.nextPosition(this.character.getDirection()));
        }
        else
        {
          if (!this.NPCSchedule)
            return;
          this.handleWarps(this.character.nextPosition(this.character.getDirection()));
        }
      }
    }

    public void handleWarps(Rectangle position)
    {
      Warp warp = this.location.isCollidingWithWarpOrDoor(position, this.character);
      if (warp == null)
        return;
      if (warp.TargetName == "Trailer" && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        warp = new Warp(warp.X, warp.Y, "Trailer_Big", 13, 24, false);
      if (this.character is NPC && (this.character as NPC).isMarried() && (this.character as NPC).followSchedule)
      {
        NPC character = this.character as NPC;
        if (this.location is FarmHouse)
          warp = new Warp(warp.X, warp.Y, "BusStop", 0, 23, false);
        if (this.location is BusStop && warp.X <= 0)
          warp = new Warp(warp.X, warp.Y, (string) (NetFieldBase<string, NetString>) character.getHome().name, (character.getHome() as FarmHouse).getEntryLocation().X, (character.getHome() as FarmHouse).getEntryLocation().Y, false);
        if (character.temporaryController != null && character.controller != null)
          character.controller.location = Game1.getLocationFromName(warp.TargetName);
      }
      this.location = Game1.getLocationFromName(warp.TargetName);
      if (this.character is NPC && (warp.TargetName == "FarmHouse" || warp.TargetName == "Cabin") && (this.character as NPC).isMarried() && (this.character as NPC).getSpouse() != null)
      {
        this.location = (GameLocation) Utility.getHomeOfFarmer((this.character as NPC).getSpouse());
        warp = new Warp(warp.X, warp.Y, (string) (NetFieldBase<string, NetString>) this.location.name, (this.location as FarmHouse).getEntryLocation().X, (this.location as FarmHouse).getEntryLocation().Y, false);
        if ((this.character as NPC).temporaryController != null && (this.character as NPC).controller != null)
          (this.character as NPC).controller.location = this.location;
        Game1.warpCharacter(this.character as NPC, this.location, new Vector2((float) warp.TargetX, (float) warp.TargetY));
      }
      else
        Game1.warpCharacter(this.character as NPC, warp.TargetName, new Vector2((float) warp.TargetX, (float) warp.TargetY));
      if (this.isPlayerPresent() && this.location.doors.ContainsKey(new Point(warp.X, warp.Y)))
        this.location.playSoundAt("doorClose", new Vector2((float) warp.X, (float) warp.Y), NetAudio.SoundContext.NPC);
      if (this.isPlayerPresent() && this.location.doors.ContainsKey(new Point(warp.TargetX, warp.TargetY - 1)))
        this.location.playSoundAt("doorClose", new Vector2((float) warp.TargetX, (float) warp.TargetY), NetAudio.SoundContext.NPC);
      if (this.pathToEndPoint.Count > 0)
        this.pathToEndPoint.Pop();
      while (this.pathToEndPoint.Count > 0 && (Math.Abs(this.pathToEndPoint.Peek().X - this.character.getTileX()) > 1 || Math.Abs(this.pathToEndPoint.Peek().Y - this.character.getTileY()) > 1))
        this.pathToEndPoint.Pop();
    }

    public static bool IsPositionImpassableOnFarm(GameLocation loc, int x, int y)
    {
      if (loc is Farm farm)
      {
        NPC.isCheckingSpouseTileOccupancy = true;
        if (farm.isTileOccupied(new Vector2((float) x, (float) y), "", true))
        {
          NPC.isCheckingSpouseTileOccupancy = false;
          return true;
        }
        NPC.isCheckingSpouseTileOccupancy = false;
        if (farm.getBuildingAt(new Vector2((float) x, (float) y)) != null)
          return true;
      }
      return PathFindController.isPositionImpassableForNPCSchedule(loc, x, y);
    }

    public static Stack<Point> FindPathOnFarm(
      Point startPoint,
      Point endPoint,
      GameLocation location,
      int limit)
    {
      Dictionary<Vector2, int> weight_map = new Dictionary<Vector2, int>();
      PriorityQueue priorityQueue = new PriorityQueue();
      HashSet<int> intSet = new HashSet<int>();
      int num = 0;
      priorityQueue.Enqueue(new PathNode(startPoint.X, startPoint.Y, (byte) 0, (PathNode) null), Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
      PathNode pathNode1 = (PathNode) priorityQueue.Peek();
      int layerWidth = location.map.Layers[0].LayerWidth;
      int layerHeight = location.map.Layers[0].LayerHeight;
      while (!priorityQueue.IsEmpty())
      {
        PathNode pathNode2 = priorityQueue.Dequeue();
        if (pathNode2.x == endPoint.X && pathNode2.y == endPoint.Y)
          return PathFindController.reconstructPath(pathNode2);
        intSet.Add(pathNode2.id);
        for (int index = 0; index < 4; ++index)
        {
          int x = pathNode2.x + (int) PathFindController.Directions[index, 0];
          int y = pathNode2.y + (int) PathFindController.Directions[index, 1];
          int hash = PathNode.ComputeHash(x, y);
          if (!intSet.Contains(hash))
          {
            PathNode p = new PathNode(x, y, pathNode2);
            p.g = (byte) ((uint) pathNode2.g + 1U);
            if (p.x == endPoint.X && p.y == endPoint.Y || p.x >= 0 && p.y >= 0 && p.x < layerWidth && p.y < layerHeight && !PathFindController.IsPositionImpassableOnFarm(location, p.x, p.y))
            {
              int priority = (int) p.g + PathFindController.GetFarmTileWeight(location, p.x, p.y, weight_map) + (Math.Abs(endPoint.X - p.x) + Math.Abs(endPoint.Y - p.y));
              if (pathNode1.x - pathNode2.x == pathNode2.x - x && pathNode1.y - pathNode2.y == pathNode2.y - y)
                priority -= 25;
              if (!priorityQueue.Contains(p, priority))
                priorityQueue.Enqueue(p, priority);
            }
          }
        }
        pathNode1 = pathNode2;
        ++num;
        if (num >= limit)
          return (Stack<Point>) null;
      }
      return (Stack<Point>) null;
    }

    public static int GetFarmTileWeight(
      GameLocation location,
      int x,
      int y,
      Dictionary<Vector2, int> weight_map)
    {
      Vector2 key = new Vector2((float) x, (float) y);
      if (weight_map.ContainsKey(key))
        return weight_map[key];
      int farmTileWeight = 0;
      Object objectAtTile = location.getObjectAtTile(x, y);
      if (objectAtTile != null && !objectAtTile.isPassable() && (!(objectAtTile is Fence fence) || !(bool) (NetFieldBase<bool, NetBool>) fence.isGate))
        farmTileWeight = 9999;
      TerrainFeature terrainFeature;
      if (location.terrainFeatures.TryGetValue(key, out terrainFeature) && terrainFeature is Flooring)
        farmTileWeight -= 50;
      weight_map[key] = farmTileWeight;
      return farmTileWeight;
    }

    public static int CheckClearance(GameLocation location, Rectangle rect)
    {
      int num;
      for (num = 0; num < 5; ++num)
      {
        bool flag = false;
        for (int left = rect.Left; left < rect.Right; ++left)
        {
          for (int top = rect.Top; top < rect.Bottom; ++top)
          {
            Object objectAtTile = location.getObjectAtTile(left, top);
            if (objectAtTile != null && !objectAtTile.isPassable())
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
          return num;
      }
      return num;
    }

    public static Stack<Point> findPathForNPCSchedules(
      Point startPoint,
      Point endPoint,
      GameLocation location,
      int limit)
    {
      PriorityQueue priorityQueue = new PriorityQueue();
      HashSet<int> intSet = new HashSet<int>();
      int num = 0;
      priorityQueue.Enqueue(new PathNode(startPoint.X, startPoint.Y, (byte) 0, (PathNode) null), Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
      PathNode pathNode1 = (PathNode) priorityQueue.Peek();
      int layerWidth = location.map.Layers[0].LayerWidth;
      int layerHeight = location.map.Layers[0].LayerHeight;
      while (!priorityQueue.IsEmpty())
      {
        PathNode pathNode2 = priorityQueue.Dequeue();
        if (pathNode2.x == endPoint.X && pathNode2.y == endPoint.Y)
          return PathFindController.reconstructPath(pathNode2);
        intSet.Add(pathNode2.id);
        for (int index = 0; index < 4; ++index)
        {
          int x = pathNode2.x + (int) PathFindController.Directions[index, 0];
          int y = pathNode2.y + (int) PathFindController.Directions[index, 1];
          int hash = PathNode.ComputeHash(x, y);
          if (!intSet.Contains(hash))
          {
            PathNode p = new PathNode(x, y, pathNode2);
            p.g = (byte) ((uint) pathNode2.g + 1U);
            if (p.x == endPoint.X && p.y == endPoint.Y || p.x >= 0 && p.y >= 0 && p.x < layerWidth && p.y < layerHeight && !PathFindController.isPositionImpassableForNPCSchedule(location, p.x, p.y))
            {
              int priority = (int) p.g + PathFindController.getPreferenceValueForTerrainType(location, p.x, p.y) + (Math.Abs(endPoint.X - p.x) + Math.Abs(endPoint.Y - p.y) + (p.x == pathNode2.x && p.x == pathNode1.x || p.y == pathNode2.y && p.y == pathNode1.y ? -2 : 0));
              if (!priorityQueue.Contains(p, priority))
                priorityQueue.Enqueue(p, priority);
            }
          }
        }
        pathNode1 = pathNode2;
        ++num;
        if (num >= limit)
          return (Stack<Point>) null;
      }
      return (Stack<Point>) null;
    }

    private static bool isPositionImpassableForNPCSchedule(GameLocation loc, int x, int y)
    {
      Tile tile = loc.Map.GetLayer("Buildings").Tiles[x, y];
      if (tile != null && tile.TileIndex != -1)
      {
        PropertyValue propertyValue = (PropertyValue) null;
        tile.TileIndexProperties.TryGetValue("Action", out propertyValue);
        if (propertyValue == null)
          tile.Properties.TryGetValue("Action", out propertyValue);
        if (propertyValue != null)
        {
          string str = propertyValue.ToString();
          if (str.StartsWith("LockedDoorWarp") || !str.Contains("Door") && !str.Contains("Passable"))
            return true;
        }
        else if (loc.doesTileHaveProperty(x, y, "Passable", "Buildings") == null && loc.doesTileHaveProperty(x, y, "NPCPassable", "Buildings") == null)
          return true;
      }
      if (loc.doesTileHaveProperty(x, y, "NoPath", "Back") != null)
        return true;
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) loc.warps)
      {
        if (warp.X == x && warp.Y == y)
          return true;
      }
      return loc.isTerrainFeatureAt(x, y);
    }

    private static int getPreferenceValueForTerrainType(GameLocation l, int x, int y)
    {
      string str = l.doesTileHaveProperty(x, y, "Type", "Back");
      if (str != null)
      {
        string lower = str.ToLower();
        if (lower == "stone")
          return -7;
        if (lower == "wood")
          return -4;
        if (lower == "dirt")
          return -2;
        if (lower == "grass")
          return -1;
      }
      return 0;
    }

    public delegate bool isAtEnd(
      PathNode currentNode,
      Point endPoint,
      GameLocation location,
      Character c);

    public delegate void endBehavior(Character c, GameLocation location);
  }
}
