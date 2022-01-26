// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Sewer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class Sewer : GameLocation
  {
    public const float steamZoom = 4f;
    public const float steamYMotionPerMillisecond = 0.1f;
    public const float millisecondsPerSteamFrame = 50f;
    private Texture2D steamAnimation;
    private Vector2 steamPosition;
    private Color steamColor = new Color(200, (int) byte.MaxValue, 200);

    public Sewer()
    {
    }

    public Sewer(string map, string name)
      : base(map, name)
    {
      this.waterColor.Value = Color.LimeGreen;
    }

    private Dictionary<ISalable, int[]> generateKrobusStock()
    {
      Dictionary<ISalable, int[]> krobusStock = new Dictionary<ISalable, int[]>();
      krobusStock.Add((ISalable) new StardewValley.Object(769, 1), new int[2]
      {
        100,
        10
      });
      krobusStock.Add((ISalable) new StardewValley.Object(768, 1), new int[2]
      {
        80,
        10
      });
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      switch (Game1.dayOfMonth % 7)
      {
        case 0:
          krobusStock.Add((ISalable) new StardewValley.Object(767, 1), new int[2]
          {
            30,
            10
          });
          break;
        case 1:
          krobusStock.Add((ISalable) new StardewValley.Object(766, 1), new int[2]
          {
            10,
            50
          });
          break;
        case 2:
          krobusStock.Add((ISalable) new StardewValley.Object(749, 1), new int[2]
          {
            300,
            1
          });
          break;
        case 3:
          krobusStock.Add((ISalable) new StardewValley.Object(random.Next(698, 709), 1), new int[2]
          {
            200,
            5
          });
          break;
        case 4:
          krobusStock.Add((ISalable) new StardewValley.Object(770, 1), new int[2]
          {
            30,
            10
          });
          break;
        case 5:
          krobusStock.Add((ISalable) new StardewValley.Object(645, 1), new int[2]
          {
            10000,
            1
          });
          break;
        case 6:
          int parentSheetIndex = random.Next(194, 245);
          if (parentSheetIndex == 217)
            parentSheetIndex = 216;
          krobusStock.Add((ISalable) new StardewValley.Object(parentSheetIndex, 1), new int[2]
          {
            random.Next(5, 51) * 10,
            5
          });
          break;
      }
      krobusStock.Add((ISalable) new StardewValley.Object(305, 1), new int[2]
      {
        5000,
        int.MaxValue
      });
      krobusStock.Add((ISalable) new StardewValley.Object(Vector2.Zero, 34), new int[2]
      {
        350,
        int.MaxValue
      });
      krobusStock.Add((ISalable) new Furniture(1800, Vector2.Zero), new int[2]
      {
        20000,
        int.MaxValue
      });
      return krobusStock;
    }

    public Dictionary<ISalable, int[]> getShadowShopStock()
    {
      Dictionary<ISalable, int[]> krobusStock = this.generateKrobusStock();
      Game1.player.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(SynchronizedShopStock.SynchedShop.Krobus, krobusStock);
      if (!Game1.player.hasOrWillReceiveMail("CF_Sewer"))
      {
        Item key = (Item) new StardewValley.Object(434, 1);
        krobusStock.Add((ISalable) key, new int[2]
        {
          20000,
          1
        });
      }
      if (!Game1.player.craftingRecipes.ContainsKey("Crystal Floor"))
      {
        StardewValley.Object @object = new StardewValley.Object(333, 1, true);
        krobusStock.Add((ISalable) new StardewValley.Object(333, 1, true), new int[2]
        {
          500,
          1
        });
      }
      if (!Game1.player.craftingRecipes.ContainsKey("Wicked Statue"))
      {
        Item key = (Item) new StardewValley.Object(Vector2.Zero, 83, true);
        krobusStock.Add((ISalable) key, new int[2]
        {
          1000,
          1
        });
      }
      if (!Game1.player.hasOrWillReceiveMail("ReturnScepter"))
      {
        Item key = (Item) new Wand();
        krobusStock.Add((ISalable) key, new int[2]
        {
          2000000,
          1
        });
      }
      return krobusStock;
    }

    public bool onShopPurchase(ISalable item, Farmer farmer, int amount)
    {
      Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Krobus, item, amount);
      return false;
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      for (float x = -1000f * Game1.options.zoomLevel + this.steamPosition.X; (double) x < (double) Game1.graphics.GraphicsDevice.Viewport.Width + 256.0; x += 256f)
      {
        for (float y = this.steamPosition.Y - 256f; (double) y < (double) (Game1.graphics.GraphicsDevice.Viewport.Height + 128); y += 256f)
          b.Draw(this.steamAnimation, new Vector2(x, y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64)), this.steamColor * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.steamPosition.Y -= (float) time.ElapsedGameTime.Milliseconds * 0.1f;
      this.steamPosition.Y %= -256f;
      this.steamPosition -= Game1.getMostRecentViewportMotion();
      if (Game1.random.NextDouble() >= 0.001)
        return;
      this.localSound("cavedrip");
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      switch (this.map.GetLayer("Buildings").Tiles[tileLocation] != null ? this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : -1)
      {
        case 21:
          Game1.warpFarmer("Town", 35, 97, 2);
          DelayedAction.playSoundAfterDelay("stairsdown", 250);
          return true;
        case 84:
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(this.getShadowShopStock(), who: "KrobusGone", on_purchase: new Func<ISalable, Farmer, int, bool>(this.onShopPurchase));
          return true;
        default:
          return base.checkAction(tileLocation, viewport, who);
      }
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      this.waterColor.Value = Color.LimeGreen * 0.75f;
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.steamPosition = new Vector2(0.0f, 0.0f);
      this.steamAnimation = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\steamAnimation");
      Game1.ambientLight = new Color(250, 140, 160);
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (Game1.getCharacterFromName("Krobus").isMarried())
      {
        this.setMapTileIndex(31, 17, 84, "Buildings", 1);
        this.setMapTileIndex(31, 16, 1, "Front", 1);
      }
      else
      {
        this.setMapTileIndex(31, 17, -1, "Buildings");
        this.setMapTileIndex(31, 16, -1, "Front");
      }
    }

    public override StardewValley.Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      float num = 0.0f;
      if (who != null && who.CurrentTool is FishingRod && (who.CurrentTool as FishingRod).getBobberAttachmentIndex() == 856)
        num += 0.1f;
      if (Game1.random.NextDouble() < 0.1 + (double) num + (who.getTileX() <= 14 || who.getTileY() <= 42 ? 0.0 : 0.08))
      {
        if (Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"))
          return new StardewValley.Object(901, 1);
        if (!who.fishCaught.ContainsKey(682))
          return new StardewValley.Object(682, 1);
      }
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      Game1.changeMusicTrack("none");
    }
  }
}
