// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Club
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace StardewValley.Locations
{
  public class Club : GameLocation
  {
    public static int timesPlayedCalicoJack;
    public static int timesPlayedSlots;
    private string coinBuffer;

    public Club()
    {
    }

    public Club(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.lightGlows.Clear();
      string str;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ru:
          str = "     ";
          break;
        case LocalizedContentManager.LanguageCode.zh:
          str = "　　";
          break;
        default:
          str = "  ";
          break;
      }
      this.coinBuffer = str;
    }

    public override void checkForMusic(GameTime time)
    {
      if (Game1.random.NextDouble() >= 0.002)
        return;
      this.localSound("boop");
    }

    public override void cleanupBeforePlayerExit()
    {
      Game1.changeMusicTrack("none");
      base.cleanupBeforePlayerExit();
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      SpriteText.drawStringWithScrollBackground(b, this.coinBuffer + Game1.player.clubCoins.ToString(), 64, 16);
      Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(68f, 20f), new Rectangle(211, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
    }
  }
}
