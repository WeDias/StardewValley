// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Blueprints
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace StardewValley.Tools
{
  public class Blueprints : Tool
  {
    public Blueprints()
      : base("Farmer's Catalogue", 0, 75, 75, false)
    {
      this.UpgradeLevel = 0;
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
      this.InstantUse = true;
    }

    public override Item getOne()
    {
      Blueprints one = new Blueprints();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Blueprints.cs.14039");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Blueprints.cs.14040");

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      if (Game1.activeClickableMenu == null)
      {
        Game1.activeClickableMenu = (IClickableMenu) new BlueprintsMenu(Game1.viewport.Width / 2 - (Game1.viewport.Width / 2 + 96) / 2, Game1.viewport.Height / 4);
        ((BlueprintsMenu) Game1.activeClickableMenu).changePosition(Game1.activeClickableMenu.xPositionOnScreen, Game1.viewport.Height / 2 - Game1.activeClickableMenu.height / 2);
      }
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
    }

    public override void tickUpdate(GameTime time, Farmer who)
    {
    }
  }
}
