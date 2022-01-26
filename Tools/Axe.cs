// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Axe
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using xTile.ObjectModel;

namespace StardewValley.Tools
{
  public class Axe : Tool
  {
    public const int StumpStrength = 4;
    private int stumpTileX;
    private int stumpTileY;
    private int hitsToStump;
    public NetInt additionalPower = new NetInt(0);

    public Axe()
      : base(nameof (Axe), 0, 189, 215, false)
    {
      this.UpgradeLevel = 0;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.additionalPower);
    }

    public override Item getOne()
    {
      Axe destination = new Axe();
      destination.UpgradeLevel = this.UpgradeLevel;
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Axe.cs.1");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Axe.cs.14019");

    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      this.Update(who.FacingDirection, 0, who);
      who.EndUsingTool();
      return true;
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isEfficient)
        who.Stamina -= (float) (2 * power) - (float) who.ForagingLevel * 0.1f;
      int num1 = x / 64;
      int num2 = y / 64;
      Rectangle rectangle = new Rectangle(num1 * 64, num2 * 64, 64, 64);
      Vector2 vector2_1 = new Vector2((float) num1, (float) num2);
      if (location.Map.GetLayer("Buildings").Tiles[num1, num2] != null)
      {
        PropertyValue propertyValue = (PropertyValue) null;
        location.Map.GetLayer("Buildings").Tiles[num1, num2].TileIndexProperties.TryGetValue("TreeStump", out propertyValue);
        if (propertyValue != null)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Axe.cs.14023"));
          return;
        }
      }
      this.upgradeLevel.Value += this.additionalPower.Value;
      location.performToolAction((Tool) this, num1, num2);
      if (location.terrainFeatures.ContainsKey(vector2_1) && location.terrainFeatures[vector2_1].performToolAction((Tool) this, 0, vector2_1, location))
        location.terrainFeatures.Remove(vector2_1);
      Rectangle boundingBox;
      if (location.largeTerrainFeatures != null)
      {
        for (int index = location.largeTerrainFeatures.Count - 1; index >= 0; --index)
        {
          boundingBox = location.largeTerrainFeatures[index].getBoundingBox();
          if (boundingBox.Intersects(rectangle) && location.largeTerrainFeatures[index].performToolAction((Tool) this, 0, vector2_1, location))
            location.largeTerrainFeatures.RemoveAt(index);
        }
      }
      Vector2 vector2_2 = new Vector2((float) num1, (float) num2);
      if (location.Objects.ContainsKey(vector2_2) && location.Objects[vector2_2].Type != null && location.Objects[vector2_2].performToolAction((Tool) this, location))
      {
        if (location.Objects[vector2_2].type.Equals((object) "Crafting") && (int) (NetFieldBase<int, NetInt>) location.Objects[vector2_2].fragility != 2)
        {
          NetCollection<Debris> debris1 = location.debris;
          int objectIndex = (bool) (NetFieldBase<bool, NetBool>) location.Objects[vector2_2].bigCraftable ? -location.Objects[vector2_2].ParentSheetIndex : location.Objects[vector2_2].ParentSheetIndex;
          Vector2 toolLocation = who.GetToolLocation();
          boundingBox = who.GetBoundingBox();
          double x1 = (double) boundingBox.Center.X;
          boundingBox = who.GetBoundingBox();
          double y1 = (double) boundingBox.Center.Y;
          Vector2 playerPosition = new Vector2((float) x1, (float) y1);
          Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
          debris1.Add(debris2);
        }
        location.Objects[vector2_2].performRemoveAction(vector2_2, location);
        location.Objects.Remove(vector2_2);
      }
      this.upgradeLevel.Value -= this.additionalPower.Value;
    }
  }
}
