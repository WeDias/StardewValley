// Decompiled with JetBrains decompiler
// Type: StardewValley.HaymakerEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley
{
  public class HaymakerEnchantment : BaseWeaponEnchantment
  {
    public override string GetName() => "Haymaker";

    protected override void _OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who)
    {
      base._OnCutWeed(tile_location, location, who);
      if (Game1.random.NextDouble() < 0.5)
        Game1.createItemDebris((Item) new Object(771, 1), new Vector2((float) ((double) tile_location.X * 64.0 + 32.0), (float) ((double) tile_location.Y * 64.0 + 32.0)), -1);
      if (Game1.random.NextDouble() >= 0.33)
        return;
      if ((Game1.getLocationFromName("Farm") as Farm).tryToAddHay(1) == 0)
      {
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16, 16), 750f, 1, 0, who.Position - new Vector2(0.0f, 128f), false, false, who.Position.Y / 10000f, 0.005f, Color.White, 4f, -0.005f, 0.0f, 0.0f)
        {
          motion = {
            Y = -1f
          },
          layerDepth = (float) (1.0 - (double) Game1.random.Next(100) / 10000.0),
          delayBeforeAnimationStart = Game1.random.Next(350)
        });
        Game1.addHUDMessage(new HUDMessage("Hay", 1, true, Color.LightGoldenrodYellow, (Item) new Object(178, 1)));
      }
      else
        Game1.createItemDebris(new Object(178, 1).getOne(), new Vector2((float) ((double) tile_location.X * 64.0 + 32.0), (float) ((double) tile_location.Y * 64.0 + 32.0)), -1);
    }
  }
}
