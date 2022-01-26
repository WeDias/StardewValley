// Decompiled with JetBrains decompiler
// Type: StardewValley.Shed
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using System.Collections.Generic;

namespace StardewValley
{
  public class Shed : DecoratableLocation
  {
    public readonly NetInt upgradeLevel = new NetInt(0);
    private bool isRobinUpgrading;

    public Shed()
    {
    }

    public Shed(string m, string name)
      : base(m, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.upgradeLevel);
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (Game1.isDarkOut())
        Game1.ambientLight = new Color(180, 180, 0);
      if (Game1.getFarm().isThereABuildingUnderConstruction() && Game1.getFarm().getBuildingUnderConstruction().indoors.Value != null && Game1.getFarm().getBuildingUnderConstruction().indoors.Value.Equals((GameLocation) this))
        this.isRobinUpgrading = true;
      else
        this.isRobinUpgrading = false;
    }

    public Building getBuilding()
    {
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value.Equals((GameLocation) this))
          return building;
      }
      return (Building) null;
    }

    public virtual void setUpgradeLevel(int upgrade_level)
    {
      this.upgradeLevel.Set(upgrade_level);
      this.updateMap();
      this.updateLayout();
    }

    public void updateLayout()
    {
      this.updateDoors();
      this.updateWarps();
      this.setWallpapers();
      this.setFloors();
    }

    public override List<Rectangle> getWalls()
    {
      List<Rectangle> walls = new List<Rectangle>();
      if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel == 0)
        walls.Add(new Rectangle(1, 1, 11, 3));
      else if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel == 1)
        walls.Add(new Rectangle(1, 1, 17, 3));
      return walls;
    }

    public override List<Rectangle> getFloors()
    {
      List<Rectangle> floors = new List<Rectangle>();
      if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel == 0)
        floors.Add(new Rectangle(1, 3, 11, 11));
      else if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel == 1)
        floors.Add(new Rectangle(1, 3, 17, 14));
      return floors;
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (!this.isRobinUpgrading)
        return;
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(64f, 64f)), new Rectangle?(new Rectangle(90, 0, 33, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01546f);
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(64f, 84f)), new Rectangle?(new Rectangle(90, 0, 33, 31)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01536f);
    }
  }
}
