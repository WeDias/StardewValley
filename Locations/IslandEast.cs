// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandEast
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandEast : IslandForestLocation
  {
    protected PerchingBirds _parrots;
    protected Texture2D _parrotTextures;
    protected NetEvent0 bananaShrineEvent = new NetEvent0();
    public NetBool bananaShrineComplete = new NetBool();
    public NetBool bananaShrineNutAwarded = new NetBool();

    public IslandEast()
    {
    }

    public IslandEast(string map, string name)
      : base(map, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields(this.bananaShrineEvent.NetFields, (INetSerializable) this.bananaShrineComplete, (INetSerializable) this.bananaShrineNutAwarded);
      this.bananaShrineEvent.onEvent += new NetEvent0.Event(this.OnBananaShrine);
    }

    public virtual void AddTorchLights()
    {
      this.removeTemporarySpritesWithIDLocal(6666f);
      int num1 = 1280;
      int num2 = 704;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1965, 8, 8), new Vector2((float) (num1 + 24), (float) (num2 + 48)), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 7,
        light = true,
        id = 6666f,
        lightRadius = 1f,
        scale = 3f,
        layerDepth = (float) ((double) (num2 + 48) / 10000.0 + 9.99999974737875E-05),
        delayBeforeAnimationStart = 0
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1984, 12, 12), new Vector2((float) (num1 + 16), (float) (num2 + 28)), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        id = 6666f,
        lightRadius = 1f,
        scale = 3f,
        layerDepth = (float) ((double) (num2 + 28) / 10000.0 + 9.99999974737875E-05),
        delayBeforeAnimationStart = 0
      });
      int num3 = 1472;
      int num4 = 704;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1965, 8, 8), new Vector2((float) (num3 + 24), (float) (num4 + 48)), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 7,
        light = true,
        id = 6666f,
        lightRadius = 1f,
        scale = 3f,
        layerDepth = (float) ((double) (num4 + 48) / 10000.0 + 9.99999974737875E-05),
        delayBeforeAnimationStart = 0
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1984, 12, 12), new Vector2((float) (num3 + 16), (float) (num4 + 28)), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        id = 6666f,
        lightRadius = 1f,
        scale = 3f,
        layerDepth = (float) ((double) (num4 + 28) / 10000.0 + 9.99999974737875E-05),
        delayBeforeAnimationStart = 0
      });
    }

    protected override void resetLocalState()
    {
      this._parrotTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\parrots");
      base.resetLocalState();
      for (int index = 0; index < 5; ++index)
        this.critters.Add((Critter) new Firefly(Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(14, 3, 16, 12), Game1.random)));
      this.AddTorchLights();
      if (this.bananaShrineComplete.Value)
        this.AddGorillaShrineTorches(0);
      this._parrots = new PerchingBirds(this._parrotTextures, 3, 24, 24, new Vector2(12f, 19f), new Point[9]
      {
        new Point(18, 8),
        new Point(17, 9),
        new Point(20, 7),
        new Point(21, 8),
        new Point(22, 7),
        new Point(23, 8),
        new Point(18, 12),
        new Point(25, 11),
        new Point(27, 8)
      }, new Point[0]);
      this._parrots.peckDuration = 0;
      for (int index = 0; index < 5; ++index)
        this._parrots.AddBird(Game1.random.Next(0, 4));
      if (!this.bananaShrineComplete.Value || new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + 1111).NextDouble() >= 0.1)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(32, 352, 32, 32), 500f, 2, 999, new Vector2(15.5f, 19f) * 64f, false, false, 0.1216f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = 888f,
        yStopCoordinate = 1497,
        motion = new Vector2(0.0f, 1f),
        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.gorillaReachedShrineCosmetic),
        delayBeforeAnimationStart = 1000
      });
    }

    public override void cleanupBeforePlayerExit()
    {
      this._parrots = (PerchingBirds) null;
      this._parrotTextures = (Texture2D) null;
      base.cleanupBeforePlayerExit();
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.bananaShrineEvent.Poll();
      if (this._parrots != null)
        this._parrots.Update(time);
      if (!this.bananaShrineComplete.Value || Game1.random.NextDouble() >= 0.005)
        return;
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(888);
      if (temporarySpriteById == null || !temporarySpriteById.motion.Equals(Vector2.Zero))
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(128, 352, 32, 32), (float) (200 + (Game1.random.NextDouble() < 0.1 ? Game1.random.Next(1000, 3000) : 0)), 1, 1, temporarySpriteById.position, false, false, 0.12224f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
    }

    public virtual void SpawnBananaNutReward()
    {
      if (this.bananaShrineNutAwarded.Value || !Game1.IsMasterGame)
        return;
      Game1.player.team.MarkCollectedNut("BananaShrine");
      this.bananaShrineNutAwarded.Value = true;
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(16.5f, 25f) * 64f, 0, (GameLocation) this, 1280);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(16.5f, 25f) * 64f, 0, (GameLocation) this, 1280);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(16.5f, 25f) * 64f, 0, (GameLocation) this, 1280);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      if (Game1.IsMasterGame && this.bananaShrineComplete.Value && !this.bananaShrineNutAwarded.Value)
        this.SpawnBananaNutReward();
      base.DayUpdate(dayOfMonth);
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(27, 27, 3, 3);
      for (int index = 0; index < 8; ++index)
      {
        Vector2 randomTile = this.getRandomTile();
        if ((double) randomTile.Y < 24.0)
          randomTile.Y += 24f;
        if ((double) randomTile.X > 4.0 && this.getTileIndexAt((int) randomTile.X, (int) randomTile.Y, "AlwaysFront") == -1 && this.isTileLocationTotallyClearAndPlaceable(randomTile) && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y, "Type", "Back") == "Grass" && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "NoSpawn", "Back") == null && this.doesTileHavePropertyNoNull((int) randomTile.X + 1, (int) randomTile.Y, "Type", "Back") != "Stone" && this.doesTileHavePropertyNoNull((int) randomTile.X - 1, (int) randomTile.Y, "Type", "Back") != "Stone" && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y + 1, "Type", "Back") != "Stone" && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y - 1, "Type", "Back") != "Stone" && !rectangle.Contains((int) randomTile.X, (int) randomTile.Y))
        {
          if (Game1.random.NextDouble() < 0.04)
          {
            StardewValley.Object @object = new StardewValley.Object(randomTile, 259, 1);
            @object.isSpawnedObject.Value = true;
            this.objects.Add(randomTile, @object);
          }
          else
            this.objects.Add(randomTile, new StardewValley.Object(randomTile, 882 + Game1.random.Next(3), 1));
        }
      }
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this._parrots != null)
        this._parrots.Draw(b);
      base.drawAboveAlwaysFrontLayer(b);
    }

    public virtual void AddGorillaShrineTorches(int delay)
    {
      if (this.getTemporarySpriteByID(12038) != null)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(15f, 24f) * 64f + new Vector2(8f, -16f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 2f,
        delayBeforeAnimationStart = delay,
        scale = 4f,
        layerDepth = 0.16704f,
        id = 12038f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(17f, 24f) * 64f + new Vector2(8f, -16f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightRadius = 2f,
        delayBeforeAnimationStart = delay,
        scale = 4f,
        layerDepth = 0.16704f,
        id = 12097f
      });
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      if (!(l is IslandEast))
        return;
      IslandEast islandEast = l as IslandEast;
      this.bananaShrineComplete.Value = islandEast.bananaShrineComplete.Value;
      this.bananaShrineNutAwarded.Value = islandEast.bananaShrineNutAwarded.Value;
    }

    public virtual void OnBananaShrine()
    {
      Location location = new Location(16, 26);
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(304, 48, 16, 16), new Vector2(16f, (float) (location.Y - 1)) * 64f, false, 0.0f, Color.White)
      {
        id = 88976f,
        scale = 4f,
        layerDepth = (float) (((double) location.Y + 1.20000004768372) * 64.0 / 10000.0),
        dontClearOnAreaEntry = true
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(32, 352, 32, 32), 400f, 2, 999, new Vector2(15.5f, 19f) * 64f, false, false, 0.1216f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = 777f,
        yStopCoordinate = 1497,
        motion = new Vector2(0.0f, 2f),
        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.gorillaReachedShrine),
        delayBeforeAnimationStart = 1000,
        dontClearOnAreaEntry = true
      });
      if (Game1.currentLocation == this)
      {
        Game1.playSound("coin");
        DelayedAction.playSoundAfterDelay("fireball", 800);
      }
      this.AddGorillaShrineTorches(800);
      if (Game1.currentLocation != this)
        return;
      DelayedAction.playSoundAfterDelay("grassyStep", 1400);
      DelayedAction.playSoundAfterDelay("grassyStep", 1800);
      DelayedAction.playSoundAfterDelay("grassyStep", 2200);
      DelayedAction.playSoundAfterDelay("grassyStep", 2600);
      DelayedAction.playSoundAfterDelay("grassyStep", 3000);
      Game1.changeMusicTrack("none");
      DelayedAction.playSoundAfterDelay("gorilla_intro", 2000);
    }

    public override bool isActionableTile(int xTile, int yTile, Farmer who) => base.isActionableTile(xTile, yTile, who);

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      switch (action)
      {
        case "BananaShrine":
          if (who.CurrentItem != null && who.CurrentItem is StardewValley.Object && Utility.IsNormalObjectAtParentSheetIndex(who.CurrentItem, 91) && this.getTemporarySpriteByID(777) == null && !this.bananaShrineComplete.Value)
          {
            this.bananaShrineComplete.Value = true;
            who.reduceActiveItemByOne();
            this.bananaShrineEvent.Fire();
            return true;
          }
          if (this.getTemporarySpriteByID(777) == null && !this.bananaShrineComplete.Value)
          {
            who.doEmote(8);
            break;
          }
          break;
      }
      return base.performAction(action, who, tileLocation);
    }

    private void gorillaReachedShrine(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 352, 32, 32);
      temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 1;
      temporarySpriteById.interval = 1000f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.motion = Vector2.Zero;
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaGrabBanana);
    }

    private void gorillaReachedShrineCosmetic(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(888);
      temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(192, 352, 32, 32);
      temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 999999;
      temporarySpriteById.interval = 8000f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.motion = Vector2.Zero;
      temporarySpriteById.animationLength = 1;
    }

    private void gorillaGrabBanana(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => this.removeTemporarySpritesWithID(88976)), 50);
      if (Game1.currentLocation == this)
        Game1.playSound("slimeHit");
      temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(96, 352, 32, 32);
      temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 1;
      temporarySpriteById.interval = 1000f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaEatBanana);
      this.temporarySprites.Add(temporarySpriteById);
    }

    private void gorillaEatBanana(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(128, 352, 32, 32);
      temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 5;
      temporarySpriteById.interval = 300f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.animationLength = 2;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaAfterEat);
      if (Game1.currentLocation == this)
      {
        Game1.playSound("eat");
        DelayedAction.playSoundAfterDelay("eat", 600);
        DelayedAction.playSoundAfterDelay("eat", 1200);
        DelayedAction.playSoundAfterDelay("eat", 1800);
        DelayedAction.playSoundAfterDelay("eat", 2400);
      }
      this.temporarySprites.Add(temporarySpriteById);
    }

    private void gorillaAfterEat(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 352, 32, 32);
      temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 1;
      temporarySpriteById.interval = 1000f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.motion = Vector2.Zero;
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaSpawnNut);
      temporarySpriteById.shakeIntensity = 1f;
      temporarySpriteById.shakeIntensityChange = -0.01f;
      this.temporarySprites.Add(temporarySpriteById);
    }

    private void gorillaSpawnNut(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 352, 32, 32);
      temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 1;
      temporarySpriteById.interval = 1000f;
      temporarySpriteById.shakeIntensity = 2f;
      temporarySpriteById.shakeIntensityChange = -0.01f;
      if (Game1.currentLocation == this)
        Game1.playSound("grunt");
      if (Game1.IsMasterGame)
        this.SpawnBananaNutReward();
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.motion = Vector2.Zero;
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaReturn);
      this.temporarySprites.Add(temporarySpriteById);
    }

    private void gorillaReturn(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(32, 352, 32, 32);
      temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 6;
      temporarySpriteById.interval = 200f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.motion = new Vector2(0.0f, -3f);
      temporarySpriteById.animationLength = 2;
      temporarySpriteById.yStopCoordinate = 1280;
      temporarySpriteById.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x => this.removeTemporarySpritesWithID(777));
      this.temporarySprites.Add(temporarySpriteById);
      if (Game1.currentLocation != this)
        return;
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => Game1.playMorningSong()), 3000);
    }
  }
}
