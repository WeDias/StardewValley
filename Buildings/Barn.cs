// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Barn
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
  public class Barn : Building
  {
    public static int openAnimalDoorPosition = -76;
    private const int closedAnimalDoorPosition = 0;
    [XmlElement("yPositionOfAnimalDoor")]
    private readonly NetInt yPositionOfAnimalDoor = new NetInt();
    [XmlElement("animalDoorMotion")]
    private readonly NetInt animalDoorMotion = new NetInt();

    public Barn(BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
    }

    public Barn()
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
      if (!(nameOfIndoorsWithoutUnique == "Barn2"))
      {
        if (nameOfIndoorsWithoutUnique == "Barn3")
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
        this.yPositionOfAnimalDoor.Value = Barn.openAnimalDoorPosition;
      return indoors;
    }

    public override Rectangle getRectForAnimalDoor()
    {
      Point point = this.animalDoor.Get();
      return new Rectangle((point.X + (int) (NetFieldBase<int, NetInt>) this.tileX) * 64, ((int) (NetFieldBase<int, NetInt>) this.tileY + point.Y) * 64, 128, 64);
    }

    public override bool doAction(Vector2 tileLocation, Farmer who)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0 || (double) tileLocation.X != (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X) && (double) tileLocation.X != (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X + 1) || (double) tileLocation.Y != (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y) || !Game1.didPlayerJustRightClick(true))
        return base.doAction(tileLocation, who);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen)
        who.currentLocation.playSound("doorCreak");
      else
        who.currentLocation.playSound("doorCreakReverse");
      this.animalDoorOpen.Value = !(bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen;
      this.animalDoorMotion.Value = (bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen ? -3 : 2;
      return true;
    }

    public override void updateWhenFarmNotCurrentLocation(GameTime time)
    {
      base.updateWhenFarmNotCurrentLocation(time);
      ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.indoors).updateWhenNotCurrentLocation((Building) this, time);
    }

    public override Rectangle getSourceRectForMenu() => new Rectangle(0, 0, this.texture.Value.Bounds.Width, this.texture.Value.Bounds.Height - 16);

    public override void performActionOnUpgrade(GameLocation location)
    {
      (this.indoors.Value as AnimalHouse).animalLimit.Value += 4;
      if ((int) (NetFieldBase<int, NetInt>) (this.indoors.Value as AnimalHouse).animalLimit != 8)
        return;
      StardewValley.Object @object = new StardewValley.Object(new Vector2(1f, 3f), 104);
      @object.fragility.Value = 2;
      this.indoors.Value.objects.Add(new Vector2(1f, 3f), @object);
    }

    public override void performActionOnConstruction(GameLocation location)
    {
      base.performActionOnConstruction(location);
      StardewValley.Object @object = new StardewValley.Object(new Vector2(6f, 3f), 99);
      @object.fragility.Value = 2;
      this.indoors.Value.objects.Add(new Vector2(6f, 3f), @object);
      this.daysOfConstructionLeft.Value = 3;
    }

    public override void dayUpdate(int dayOfMonth)
    {
      base.dayUpdate(dayOfMonth);
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
        return;
      this.currentOccupants.Value = ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.indoors).animals.Count();
      if ((int) (NetFieldBase<int, NetInt>) (this.indoors.Value as AnimalHouse).animalLimit != 16)
        return;
      int num = Math.Min((this.indoors.Value as AnimalHouse).animals.Count() - this.indoors.Value.numberOfObjectsWithName("Hay"), (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay);
      (Game1.getLocationFromName("Farm") as Farm).piecesOfHay.Value -= num;
      for (int index = 0; index < 16 && num > 0; ++index)
      {
        Vector2 key = new Vector2((float) (8 + index), 3f);
        if (!this.indoors.Value.objects.ContainsKey(key))
          this.indoors.Value.objects.Add(key, new StardewValley.Object(178, 1));
        --num;
      }
    }

    public override void Update(GameTime time)
    {
      base.Update(time);
      if ((int) (NetFieldBase<int, NetInt>) this.animalDoorMotion == 0)
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.animalDoorOpen && (int) (NetFieldBase<int, NetInt>) this.yPositionOfAnimalDoor <= Barn.openAnimalDoorPosition)
      {
        this.animalDoorMotion.Value = 0;
        this.yPositionOfAnimalDoor.Value = Barn.openAnimalDoorPosition;
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
      if (this.buildingType.Equals((object) "Big Barn"))
      {
        ++this.animalDoor.X;
        this.indoors.Value.moveObject(15, 3, 18, 13);
        this.indoors.Value.moveObject(16, 3, 19, 13);
        this.indoors.Value.moveObject(1, 4, 20, 3);
        for (int index = 4; index < 13; ++index)
          this.indoors.Value.moveObject(16, index, 20, index);
      }
      else
      {
        this.indoors.Value.moveObject(20, 3, 1, 4);
        for (int index = 6; index < 12; ++index)
          this.indoors.Value.moveObject(20, index, 23, index);
        this.indoors.Value.moveObject(20, 4, 20, 13);
        this.indoors.Value.moveObject(20, 5, 21, 13);
        this.indoors.Value.moveObject(20, 12, 22, 13);
      }
    }

    public override Vector2 getUpgradeSignLocation() => new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1)) * 64f + new Vector2(192f, 4f);

    public override void drawInMenu(SpriteBatch b, int x, int y)
    {
      this.drawShadow(b, x, y);
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y) + new Vector2((float) this.animalDoor.X, (float) (this.animalDoor.Y + 3)) * 64f, new Rectangle?(new Rectangle(64, 112, 32, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.888f);
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y) + new Vector2((float) this.animalDoor.X, (float) this.animalDoor.Y + 2.25f) * 64f, new Rectangle?(new Rectangle(0, 112, 32, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64) / 10000.0 - 1.0000000116861E-07));
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(0, 0, 112, 112)), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.89f);
    }

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
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y - 1)) * 64f), new Rectangle?(new Rectangle(32, 112, 32, 16)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y)) * 64f), new Rectangle?(new Rectangle(64, 112, 32, 16)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X) * 64), (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y) * 64 + (int) (NetFieldBase<int, NetInt>) this.yPositionOfAnimalDoor - 48))), new Rectangle?(new Rectangle(0, 112, 32, 12)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 - 9.99999974737875E-05));
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X) * 64), (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y) * 64 + (int) (NetFieldBase<int, NetInt>) this.yPositionOfAnimalDoor))), new Rectangle?(new Rectangle(0, 112, 32, 16)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 - 9.99999974737875E-05));
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Rectangle?(new Rectangle(0, 0, 112, 112)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 112f), 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 - 9.99999974737875E-06));
        if ((int) (NetFieldBase<int, NetInt>) this.daysUntilUpgrade <= 0)
          return;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getUpgradeSignLocation()), new Rectangle?(new Rectangle(367, 309, 16, 15)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
      }
    }
  }
}
