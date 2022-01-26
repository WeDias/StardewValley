// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ClickableComponent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class ClickableComponent
  {
    public const int ID_ignore = -500;
    public const int CUSTOM_SNAP_BEHAVIOR = -7777;
    public const int SNAP_AUTOMATIC = -99998;
    public const int SNAP_TO_DEFAULT = -99999;
    public Rectangle bounds;
    public string name;
    public string label;
    public float scale = 1f;
    public Item item;
    public bool visible = true;
    public bool leftNeighborImmutable;
    public bool rightNeighborImmutable;
    public bool upNeighborImmutable;
    public bool downNeighborImmutable;
    public bool tryDefaultIfNoDownNeighborExists;
    public bool tryDefaultIfNoRightNeighborExists;
    public bool fullyImmutable;
    public int myID = -500;
    public int leftNeighborID = -1;
    public int rightNeighborID = -1;
    public int upNeighborID = -1;
    public int downNeighborID = -1;
    public int myAlternateID = -500;
    public int region;

    public ClickableComponent(Rectangle bounds, string name)
    {
      this.bounds = bounds;
      this.name = name;
    }

    public ClickableComponent(Rectangle bounds, string name, string label)
    {
      this.bounds = bounds;
      this.name = name;
      this.label = label;
    }

    public ClickableComponent(Rectangle bounds, Item item)
    {
      this.bounds = bounds;
      this.item = item;
    }

    public virtual bool containsPoint(int x, int y)
    {
      if (!this.visible || !this.bounds.Contains(x, y))
        return false;
      Game1.SetFreeCursorDrag();
      return true;
    }

    public virtual void snapMouseCursor() => Game1.setMousePosition(this.bounds.Right - this.bounds.Width / 8, this.bounds.Bottom - this.bounds.Height / 8);

    public void snapMouseCursorToCenter() => Game1.setMousePosition(this.bounds.Center.X, this.bounds.Center.Y);

    public static void SetUpNeighbors<T>(List<T> components, int id) where T : ClickableComponent
    {
      for (int index = 0; index < components.Count; ++index)
      {
        T component = components[index];
        if ((object) component != null)
          component.upNeighborID = id;
      }
    }

    public static void ChainNeighborsLeftRight<T>(List<T> components) where T : ClickableComponent
    {
      ClickableComponent clickableComponent = (ClickableComponent) null;
      for (int index = 0; index < components.Count; ++index)
      {
        T component = components[index];
        if ((object) component != null)
        {
          component.rightNeighborID = -1;
          component.leftNeighborID = -1;
          if (clickableComponent != null)
          {
            component.leftNeighborID = clickableComponent.myID;
            clickableComponent.rightNeighborID = component.myID;
          }
          clickableComponent = (ClickableComponent) component;
        }
      }
    }

    public static void ChainNeighborsUpDown<T>(List<T> components) where T : ClickableComponent
    {
      ClickableComponent clickableComponent = (ClickableComponent) null;
      for (int index = 0; index < components.Count; ++index)
      {
        T component = components[index];
        if ((object) component != null)
        {
          component.downNeighborID = -1;
          component.upNeighborID = -1;
          if (clickableComponent != null)
          {
            component.upNeighborID = clickableComponent.myID;
            clickableComponent.downNeighborID = component.myID;
          }
          clickableComponent = (ClickableComponent) component;
        }
      }
    }
  }
}
