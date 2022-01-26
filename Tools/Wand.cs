// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Wand
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;

namespace StardewValley.Tools
{
  public class Wand : Tool
  {
    public bool charged;

    public Wand()
      : base("Return Scepter", 0, 2, 2, false)
    {
      this.UpgradeLevel = 0;
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
      this.InstantUse = true;
    }

    public override Item getOne()
    {
      Wand destination = new Wand();
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Wand.cs.14318");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Wand.cs.14319");

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) who.bathingClothes || !who.IsLocalPlayer || who.onBridge.Value)
        return;
      this.indexOfMenuItemView.Value = 2;
      this.CurrentParentTileIndex = 2;
      for (int index = 0; index < 12; ++index)
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.position.X - 256, (int) who.position.X + 192), (float) Game1.random.Next((int) who.position.Y - 256, (int) who.position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
      location.playSound("wand");
      Game1.displayFarmer = false;
      who.temporarilyInvincible = true;
      who.temporaryInvincibilityTimer = -2000;
      who.Halt();
      who.faceDirection(2);
      who.CanMove = false;
      who.freezePause = 2000;
      Game1.flashAlpha = 1f;
      DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.wandWarpForReal), 1000);
      new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
      int num = 0;
      for (int x1 = who.getTileX() + 8; x1 >= who.getTileX() - 8; --x1)
      {
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(6, new Vector2((float) x1, (float) who.getTileY()) * 64f, Color.White, animationInterval: 50f)
        {
          layerDepth = 1f,
          delayBeforeAnimationStart = num * 25,
          motion = new Vector2(-0.25f, 0.0f)
        });
        ++num;
      }
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
    }

    public override bool actionWhenPurchased()
    {
      Game1.player.mailReceived.Add("ReturnScepter");
      return base.actionWhenPurchased();
    }

    private void wandWarpForReal()
    {
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
      if (homeOfFarmer == null)
        return;
      Point frontDoorSpot = homeOfFarmer.getFrontDoorSpot();
      Game1.warpFarmer("Farm", frontDoorSpot.X, frontDoorSpot.Y, false);
      if (!Game1.isStartingToGetDarkOut() && !Game1.isRaining)
        Game1.playMorningSong();
      else
        Game1.changeMusicTrack("none");
      Game1.fadeToBlackAlpha = 0.99f;
      Game1.screenGlow = false;
      this.lastUser.temporarilyInvincible = false;
      this.lastUser.temporaryInvincibilityTimer = 0;
      Game1.displayFarmer = true;
      this.lastUser.CanMove = true;
    }
  }
}
