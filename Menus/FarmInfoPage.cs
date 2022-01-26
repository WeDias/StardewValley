// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.FarmInfoPage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class FarmInfoPage : IClickableMenu
  {
    private string descriptionText = "";
    private string hoverText = "";
    private ClickableTextureComponent moneyIcon;
    private ClickableTextureComponent farmMap;
    private ClickableTextureComponent mapFarmer;
    private ClickableTextureComponent farmHouse;
    private List<ClickableTextureComponent> animals = new List<ClickableTextureComponent>();
    private List<ClickableTextureComponent> mapBuildings = new List<ClickableTextureComponent>();
    private List<MiniatureTerrainFeature> mapFeatures = new List<MiniatureTerrainFeature>();
    private Farm farm;
    private int mapX;
    private int mapY;

    public FarmInfoPage(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
      this.moneyIcon = new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 32, Game1.player.Money > 9999 ? 18 : 20, 16), Game1.player.Money.ToString() + "g", "", Game1.debrisSpriteSheet, new Rectangle(88, 280, 16, 16), 1f);
      this.mapX = x + IClickableMenu.spaceToClearSideBorder + 128 + 32 + 16;
      this.mapY = y + IClickableMenu.spaceToClearTopBorder + 21 - 4;
      this.farmMap = new ClickableTextureComponent(new Rectangle(this.mapX, this.mapY, 20, 20), Game1.content.Load<Texture2D>("LooseSprites\\farmMap"), Rectangle.Empty, 1f);
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      int num6 = 0;
      int num7 = 0;
      int num8 = 0;
      int num9 = 0;
      int num10 = 0;
      int num11 = 0;
      int num12 = 0;
      int num13 = 0;
      int num14 = 0;
      int num15 = 0;
      int num16 = 0;
      this.farm = (Farm) Game1.getLocationFromName("Farm");
      this.farmHouse = new ClickableTextureComponent("FarmHouse", new Rectangle(this.mapX + 443, this.mapY + 43, 80, 72), "FarmHouse", "", Game1.content.Load<Texture2D>("Buildings\\houses"), new Rectangle(0, 0, 160, 144), 0.5f);
      foreach (FarmAnimal allFarmAnimal in this.farm.getAllFarmAnimals())
      {
        if (allFarmAnimal.type.Value.Contains("Chicken"))
        {
          ++num1;
          num9 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
        }
        else
        {
          string str = allFarmAnimal.type.Value;
          if (!(str == "Cow"))
          {
            if (!(str == "Duck"))
            {
              if (!(str == "Rabbit"))
              {
                if (!(str == "Sheep"))
                {
                  if (!(str == "Goat"))
                  {
                    if (str == "Pig")
                    {
                      ++num8;
                      num16 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
                    }
                    else
                    {
                      ++num4;
                      num12 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
                    }
                  }
                  else
                  {
                    ++num7;
                    num14 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
                  }
                }
                else
                {
                  ++num6;
                  num15 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
                }
              }
              else
              {
                ++num3;
                num10 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
              }
            }
            else
            {
              ++num2;
              num11 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
            }
          }
          else
          {
            ++num5;
            num13 += (int) (NetFieldBase<int, NetInt>) allFarmAnimal.friendshipTowardFarmer;
          }
        }
      }
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64, 40, 32), num1.ToString() ?? "", "Chickens" + (num1 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num9 / num1)) : ""), Game1.mouseCursors, new Rectangle(256, 64, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 36, 40, 32), num2.ToString() ?? "", "Ducks" + (num2 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num11 / num2)) : ""), Game1.mouseCursors, new Rectangle(288, 64, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 72, 40, 32), num3.ToString() ?? "", "Rabbits" + (num3 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num10 / num3)) : ""), Game1.mouseCursors, new Rectangle(256, 96, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 108, 40, 32), num5.ToString() ?? "", "Cows" + (num5 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num13 / num5)) : ""), Game1.mouseCursors, new Rectangle(320, 64, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 144, 40, 32), num7.ToString() ?? "", "Goats" + (num7 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num14 / num7)) : ""), Game1.mouseCursors, new Rectangle(352, 64, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 180, 40, 32), num6.ToString() ?? "", "Sheep" + (num6 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num15 / num6)) : ""), Game1.mouseCursors, new Rectangle(352, 96, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 216, 40, 32), num8.ToString() ?? "", "Pigs" + (num8 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num16 / num8)) : ""), Game1.mouseCursors, new Rectangle(320, 96, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 252, 40, 32), num4.ToString() ?? "", "???" + (num4 > 0 ? Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10425", (object) (num12 / num4)) : ""), Game1.mouseCursors, new Rectangle(288, 96, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 288, 40, 32), Game1.stats.CropsShipped.ToString() ?? "", Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10440"), Game1.mouseCursors, new Rectangle(480, 64, 32, 32), 1f));
      this.animals.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.spaceToClearSideBorder + 32, y + IClickableMenu.spaceToClearTopBorder + 64 + 324, 40, 32), this.farm.buildings.Count<Building>().ToString() ?? "", Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmInfoPage.cs.10441"), Game1.mouseCursors, new Rectangle(448, 64, 32, 32), 1f));
      int num17 = 8;
      foreach (Building building in this.farm.buildings)
        this.mapBuildings.Add(new ClickableTextureComponent("", new Rectangle(this.mapX + (int) (NetFieldBase<int, NetInt>) building.tileX * num17, this.mapY + (int) (NetFieldBase<int, NetInt>) building.tileY * num17 + ((int) (NetFieldBase<int, NetInt>) building.tilesHigh + 1) * num17 - (int) ((double) building.texture.Value.Height / 8.0), (int) (NetFieldBase<int, NetInt>) building.tilesWide * num17, (int) ((double) building.texture.Value.Height / 8.0)), "", (string) (NetFieldBase<string, NetString>) building.buildingType, building.texture.Value, building.getSourceRectForMenu(), 0.125f));
      foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.farm.terrainFeatures.Pairs)
        this.mapFeatures.Add(new MiniatureTerrainFeature(pair.Value, new Vector2(pair.Key.X * (float) num17 + (float) this.mapX, pair.Key.Y * (float) num17 + (float) this.mapY), pair.Key, 0.125f));
      if (!(Game1.currentLocation is Farm))
        return;
      this.mapFarmer = new ClickableTextureComponent("", new Rectangle(this.mapX + (int) ((double) Game1.player.Position.X / 8.0), this.mapY + (int) ((double) Game1.player.Position.Y / 8.0), 8, 12), "", Game1.player.Name, (Texture2D) null, new Rectangle(0, 0, 64, 96), 0.125f);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.descriptionText = "";
      this.hoverText = "";
      foreach (ClickableTextureComponent animal in this.animals)
      {
        if (animal.containsPoint(x, y))
        {
          this.hoverText = animal.hoverText;
          return;
        }
      }
      foreach (ClickableTextureComponent mapBuilding in this.mapBuildings)
      {
        if (mapBuilding.containsPoint(x, y))
        {
          this.hoverText = mapBuilding.hoverText;
          return;
        }
      }
      if (this.mapFarmer == null || !this.mapFarmer.containsPoint(x, y))
        return;
      this.hoverText = this.mapFarmer.hoverText;
    }

    public override void draw(SpriteBatch b)
    {
      this.drawVerticalPartition(b, this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 128);
      this.moneyIcon.draw(b);
      foreach (ClickableTextureComponent animal in this.animals)
        animal.draw(b);
      this.farmMap.draw(b);
      foreach (ClickableTextureComponent mapBuilding in this.mapBuildings)
        mapBuilding.draw(b);
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      this.farmMap.draw(b);
      foreach (ClickableTextureComponent mapBuilding in this.mapBuildings)
        mapBuilding.draw(b);
      foreach (MiniatureTerrainFeature mapFeature in this.mapFeatures)
        mapFeature.draw(b);
      this.farmHouse.draw(b);
      if (this.mapFarmer != null)
        Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float) (this.mapFarmer.bounds.X - 16), (float) (this.mapFarmer.bounds.Y - 16)), 0.99f, 2f, 2, Game1.player);
      foreach (KeyValuePair<long, FarmAnimal> pair in this.farm.animals.Pairs)
        b.Draw(pair.Value.Sprite.Texture, new Vector2((float) (this.mapX + (int) ((double) pair.Value.Position.X / 8.0)), (float) (this.mapY + (int) ((double) pair.Value.Position.Y / 8.0))), new Rectangle?(pair.Value.Sprite.SourceRect), Color.White, 0.0f, Vector2.Zero, 0.125f, SpriteEffects.None, (float) (0.860000014305115 + (double) pair.Value.Position.Y / 8.0 / 20000.0 + 0.0125000001862645));
      foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.farm.objects.Pairs)
        pair.Value.drawInMenu(b, new Vector2((float) this.mapX + pair.Key.X * 8f, (float) this.mapY + pair.Key.Y * 8f), 0.125f, 1f, (float) (0.860000014305115 + ((double) this.mapY + (double) pair.Key.Y * 8.0 - 25.0) / 20000.0));
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (this.hoverText.Equals(""))
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
  }
}
