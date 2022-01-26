// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Coop
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Util;
using System;
using System.Xml.Serialization;

namespace StardewValley.Buildings
{
  public class Coop : Building
  {
    public static int openAnimalDoorPosition = -52;
    private const int closedAnimalDoorPosition = 0;
    [XmlElement("yPositionOfAnimalDoor")]
    private readonly NetInt yPositionOfAnimalDoor = new NetInt();
    [XmlElement("animalDoorMotion")]
    private readonly NetInt animalDoorMotion = new NetInt();

    public Coop(BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
    }

    public Coop()
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.yPositionOfAnimalDoor, (INetSerializable) this.animalDoorMotion);
    }

    protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
    {
      GameLocation indoors = (GameLocation) new AnimalHouse("Maps\\" + nameOfIndoorsWithoutUnique, (string) (NetFieldBase<string, NetString>) this.buildingType);
      indoors.IsFarm = true;
      indoors.isStructure.Value = true;
      indoors.uniqueName.Value = nameOfIndoorsWithoutUnique + GuidHelper.NewGuid().ToString();
      if (!(nameOfIndoorsWithoutUnique == "Coop2"))
      {
        if (nameOfIndoorsWithoutUnique == "Coop3")
          (indoors as AnimalHouse).animalLimit.Value = 12;
      }
      else
        (indoors as AnimalHouse).animalLimit.Value = 8;
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) indoors.warps)
      {
        warp.TargetX = this.humanDoor.X + (int) (NetFieldBase<int, NetInt>) this.tileX;
        warp.TargetY = this.humanDoor.Y + (int) (NetFieldBase<int, NetInt>) this.tileY + 1;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen)
        this.yPositionOfAnimalDoor.Value = Coop.openAnimalDoorPosition;
      if ((indoors as AnimalHouse).incubatingEgg.Y > 0)
        indoors.map.GetLayer("Front").Tiles[1, 2].TileIndex += Game1.player.ActiveObject.ParentSheetIndex == 180 || Game1.player.ActiveObject.ParentSheetIndex == 182 ? 2 : 1;
      return indoors;
    }

    public override void performActionOnConstruction(GameLocation location)
    {
      base.performActionOnConstruction(location);
      StardewValley.Object @object = new StardewValley.Object(new Vector2(3f, 3f), 99);
      @object.fragility.Value = 2;
      this.indoors.Value.objects.Add(new Vector2(3f, 3f), @object);
      this.daysOfConstructionLeft.Value = 3;
    }

    public override void performActionOnUpgrade(GameLocation location)
    {
      (this.indoors.Value as AnimalHouse).animalLimit.Value += 4;
      if ((int) (NetFieldBase<int, NetInt>) (this.indoors.Value as AnimalHouse).animalLimit == 8)
      {
        StardewValley.Object @object = new StardewValley.Object(new Vector2(2f, 3f), 104);
        @object.fragility.Value = 2;
        this.indoors.Value.objects.Add(new Vector2(2f, 3f), @object);
        this.indoors.Value.moveObject(1, 3, 14, 7);
      }
      else
      {
        this.indoors.Value.moveObject(14, 7, 21, 7);
        this.indoors.Value.moveObject(14, 8, 21, 8);
        this.indoors.Value.moveObject(14, 4, 20, 4);
      }
    }

    public override Rectangle getSourceRectForMenu() => new Rectangle(0, 0, this.texture.Value.Bounds.Width, this.texture.Value.Bounds.Height - 16);

    public override bool doAction(Vector2 tileLocation, Farmer who)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0 || (double) tileLocation.X != (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X) || (double) tileLocation.Y != (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y))
        return base.doAction(tileLocation, who);
      if (Game1.didPlayerJustRightClick(true))
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen)
          who.currentLocation.playSound("doorCreak");
        else
          who.currentLocation.playSound("doorCreakReverse");
        this.animalDoorOpen.Value = !(bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen;
        this.animalDoorMotion.Value = (bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen ? -2 : 2;
      }
      return true;
    }

    public override void updateWhenFarmNotCurrentLocation(GameTime time)
    {
      base.updateWhenFarmNotCurrentLocation(time);
      ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.indoors).updateWhenNotCurrentLocation((Building) this, time);
    }

    public override void dayUpdate(int dayOfMonth)
    {
      base.dayUpdate(dayOfMonth);
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0)
      {
        if ((this.indoors.Value as AnimalHouse).incubatingEgg.Y > 0)
        {
          --(this.indoors.Value as AnimalHouse).incubatingEgg.X;
          if ((this.indoors.Value as AnimalHouse).incubatingEgg.X <= 0)
          {
            long newId = Game1.multiplayer.getNewID();
            FarmAnimal farmAnimal = new FarmAnimal((this.indoors.Value as AnimalHouse).incubatingEgg.Y == 442 ? "Duck" : ((this.indoors.Value as AnimalHouse).incubatingEgg.Y == 180 || (this.indoors.Value as AnimalHouse).incubatingEgg.Y == 182 ? "BrownChicken" : ((this.indoors.Value as AnimalHouse).incubatingEgg.Y == 107 ? "Dinosaur" : "Chicken")), newId, (long) this.owner);
            (this.indoors.Value as AnimalHouse).incubatingEgg.X = 0;
            (this.indoors.Value as AnimalHouse).incubatingEgg.Y = -1;
            this.indoors.Value.map.GetLayer("Front").Tiles[1, 2].TileIndex = 45;
            ((AnimalHouse) this.indoors.Value).animals.Add(newId, farmAnimal);
          }
        }
        if ((int) (NetFieldBase<int, NetInt>) (this.indoors.Value as AnimalHouse).animalLimit == 16)
        {
          int num = Math.Min((this.indoors.Value as AnimalHouse).animals.Count() - this.indoors.Value.numberOfObjectsWithName("Hay"), (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay);
          (Game1.getLocationFromName("Farm") as Farm).piecesOfHay.Value -= num;
          for (int index = 0; index < 16 && num > 0; ++index)
          {
            Vector2 key = new Vector2((float) (6 + index), 3f);
            if (!this.indoors.Value.objects.ContainsKey(key))
              this.indoors.Value.objects.Add(key, new StardewValley.Object(178, 1));
            --num;
          }
        }
      }
      this.currentOccupants.Value = ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.indoors).animals.Count();
    }

    public override void Update(GameTime time)
    {
      base.Update(time);
      if ((int) (NetFieldBase<int, NetInt>) this.animalDoorMotion == 0)
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen && (int) (NetFieldBase<int, NetInt>) this.yPositionOfAnimalDoor <= Coop.openAnimalDoorPosition)
      {
        this.animalDoorMotion.Value = 0;
        this.yPositionOfAnimalDoor.Value = Coop.openAnimalDoorPosition;
      }
      else if (!(bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen && (int) (NetFieldBase<int, NetInt>) this.yPositionOfAnimalDoor >= 0)
      {
        this.animalDoorMotion.Value = 0;
        this.yPositionOfAnimalDoor.Value = 0;
      }
      this.yPositionOfAnimalDoor.Value += (int) (NetFieldBase<int, NetInt>) this.animalDoorMotion;
    }

    public override void upgrade()
    {
      base.upgrade();
      if (this.buildingType.Equals((object) "Big Coop"))
      {
        this.indoors.Value.moveObject(2, 3, 14, 8);
        this.indoors.Value.moveObject(1, 3, 14, 7);
        this.indoors.Value.moveObject(10, 4, 14, 4);
        this.indoors.Value.objects.Add(new Vector2(2f, 3f), new StardewValley.Object(new Vector2(2f, 3f), 101));
        if (!Game1.player.hasOrWillReceiveMail("incubator"))
          Game1.mailbox.Add("incubator");
      }
      if ((int) (NetFieldBase<int, NetInt>) (this.indoors.Value as AnimalHouse).animalLimit == 8)
        return;
      this.indoors.Value.moveObject(14, 7, 21, 7);
      this.indoors.Value.moveObject(14, 8, 21, 8);
      this.indoors.Value.moveObject(14, 4, 20, 4);
    }

    public override void drawInMenu(SpriteBatch b, int x, int y)
    {
      this.drawShadow(b, x, y);
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y) + new Vector2((float) this.animalDoor.X, (float) (this.animalDoor.Y + 4)) * 64f, new Rectangle?(new Rectangle(16, 112, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y) + new Vector2((float) this.animalDoor.X, (float) this.animalDoor.Y + 3.5f) * 64f, new Rectangle?(new Rectangle(0, 112, 16, 15)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 - 1.0000000116861E-07));
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(0, 0, 96, 112)), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.89f);
    }

    public override Vector2 getUpgradeSignLocation() => new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1)) * 64f + new Vector2(128f, 4f);

    public override void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
      {
        this.drawInConstruction(b);
      }
      else
      {
        this.drawShadow(b);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y)) * 64f), new Rectangle?(new Rectangle(16, 112, 16, 16)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X) * 64), (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y) * 64 + (int) (NetFieldBase<int, NetInt>) this.yPositionOfAnimalDoor))), new Rectangle?(new Rectangle(0, 112, 16, 16)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 - 1.0000000116861E-07));
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Rectangle?(new Rectangle(0, 0, 96, 112)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 112f), 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000f);
        if ((int) (NetFieldBase<int, NetInt>) this.daysUntilUpgrade <= 0)
          return;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getUpgradeSignLocation()), new Rectangle?(new Rectangle(367, 309, 16, 15)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
      }
    }
  }
}
