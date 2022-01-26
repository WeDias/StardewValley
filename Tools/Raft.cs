// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Raft
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley.Tools
{
  public class Raft : Tool
  {
    public Raft()
      : base(nameof (Raft), 0, 1, 1, false)
    {
      this.UpgradeLevel = 0;
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
      this.InstantUse = true;
    }

    public override Item getOne()
    {
      Raft one = new Raft();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Raft.cs.14204");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Raft.cs.14205");

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      if (!who.isRafting && location.doesTileHaveProperty(x / 64, y / 64, "Water", "Back") != null)
      {
        who.isRafting = true;
        Rectangle position = new Rectangle(x - 32, y - 32, 64, 64);
        if (location.isCollidingPosition(position, Game1.viewport, true))
        {
          who.isRafting = false;
          return;
        }
        who.xVelocity = who.FacingDirection == 1 ? 3f : (who.FacingDirection == 3 ? -3f : 0.0f);
        who.yVelocity = who.FacingDirection == 2 ? 3f : (who.FacingDirection == 0 ? -3f : 0.0f);
        who.Position = new Vector2((float) (x - 32), (float) (y - 32 - 32 - (y < who.getStandingY() ? 64 : 0)));
        Game1.playSound("dropItemInWater");
      }
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
    }
  }
}
