// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Pan
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Tools
{
  public class Pan : Tool
  {
    [XmlIgnore]
    private readonly NetEvent0 finishEvent = new NetEvent0();

    public Pan()
      : base("Copper Pan", -1, 12, 12, false)
    {
    }

    public override Item getOne()
    {
      Pan destination = new Pan();
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Pan.cs.14180");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Pan.cs.14181");

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.finishEvent);
      this.finishEvent.onEvent += new NetEvent0.Event(this.doFinish);
    }

    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      this.CurrentParentTileIndex = 12;
      this.IndexOfMenuItemView = 12;
      bool flag = false;
      Rectangle rectangle = new Rectangle(location.orePanPoint.X * 64 - 64, location.orePanPoint.Y * 64 - 64, 256, 256);
      if (rectangle.Contains(x, y) && (double) Utility.distance((float) who.getStandingX(), (float) rectangle.Center.X, (float) who.getStandingY(), (float) rectangle.Center.Y) <= 192.0)
        flag = true;
      who.lastClick = Vector2.Zero;
      x = (int) who.GetToolLocation().X;
      y = (int) who.GetToolLocation().Y;
      who.lastClick = new Vector2((float) x, (float) y);
      if ((NetFieldBase<Point, NetPoint>) location.orePanPoint != (NetPoint) null && !location.orePanPoint.Equals((object) Point.Zero))
      {
        Rectangle boundingBox = who.GetBoundingBox();
        if (flag || boundingBox.Intersects(rectangle))
        {
          who.faceDirection(2);
          who.FarmerSprite.animateOnce(303, 50f, 4);
          return true;
        }
      }
      who.forceCanMove();
      return true;
    }

    public static void playSlosh(Farmer who) => who.currentLocation.localSound("slosh");

    public override void tickUpdate(GameTime time, Farmer who)
    {
      this.lastUser = who;
      base.tickUpdate(time, who);
      this.finishEvent.Poll();
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      x = (int) who.GetToolLocation().X;
      y = (int) who.GetToolLocation().Y;
      this.CurrentParentTileIndex = 12;
      this.IndexOfMenuItemView = 12;
      location.localSound("coin");
      who.addItemsByMenuIfNecessary(this.getPanItems(location, who));
      location.orePanPoint.Value = Point.Zero;
      this.finish();
    }

    private void finish() => this.finishEvent.Fire();

    private void doFinish()
    {
      this.lastUser.CanMove = true;
      this.lastUser.UsingTool = false;
      this.lastUser.canReleaseTool = true;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      this.IndexOfMenuItemView = 12;
      base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
    }

    public List<Item> getPanItems(GameLocation location, Farmer who)
    {
      List<Item> panItems = new List<Item>();
      int parentSheetIndex1 = 378;
      int parentSheetIndex2 = -1;
      Random random = new Random(location.orePanPoint.X + location.orePanPoint.Y * 1000 + (int) Game1.stats.DaysPlayed);
      double num1 = random.NextDouble() - (double) (int) (NetFieldBase<int, NetInt>) who.luckLevel * 0.001 - who.DailyLuck;
      if (num1 < 0.01)
        parentSheetIndex1 = 386;
      else if (num1 < 0.241)
        parentSheetIndex1 = 384;
      else if (num1 < 0.6)
        parentSheetIndex1 = 380;
      int initialStack1 = random.Next(5) + 1 + (int) ((random.NextDouble() + 0.1 + (double) (int) (NetFieldBase<int, NetInt>) who.luckLevel / 10.0 + who.DailyLuck) * 2.0);
      int initialStack2 = random.Next(5) + 1 + (int) ((random.NextDouble() + 0.1 + (double) (int) (NetFieldBase<int, NetInt>) who.luckLevel / 10.0) * 2.0);
      if (random.NextDouble() - who.DailyLuck < 0.4 + (double) who.LuckLevel * 0.04)
      {
        double num2 = random.NextDouble() - who.DailyLuck;
        parentSheetIndex2 = 382;
        if (num2 < 0.02 + (double) who.LuckLevel * 0.002)
        {
          parentSheetIndex2 = 72;
          initialStack2 = 1;
        }
        else if (num2 < 0.1)
        {
          parentSheetIndex2 = 60 + random.Next(5) * 2;
          initialStack2 = 1;
        }
        else if (num2 < 0.36)
        {
          parentSheetIndex2 = 749;
          initialStack2 = Math.Max(1, initialStack2 / 2);
        }
        else if (num2 < 0.5)
        {
          parentSheetIndex2 = random.NextDouble() < 0.3 ? 82 : (random.NextDouble() < 0.5 ? 84 : 86);
          initialStack2 = 1;
        }
        if (num2 < (double) who.LuckLevel * 0.002)
          panItems.Add((Item) new Ring(859));
      }
      panItems.Add((Item) new StardewValley.Object(parentSheetIndex1, initialStack1));
      if (parentSheetIndex2 != -1)
        panItems.Add((Item) new StardewValley.Object(parentSheetIndex2, initialStack2));
      switch (location)
      {
        case IslandNorth _ when (bool) (NetFieldBase<bool, NetBool>) (Game1.getLocationFromName("IslandNorth") as IslandNorth).bridgeFixed && random.NextDouble() < 0.2:
          panItems.Add((Item) new StardewValley.Object(822, 1));
          break;
        case IslandLocation _ when random.NextDouble() < 0.2:
          panItems.Add((Item) new StardewValley.Object(831, random.Next(2, 6)));
          break;
      }
      return panItems;
    }
  }
}
