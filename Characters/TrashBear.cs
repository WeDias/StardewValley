// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.TrashBear
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Characters
{
  public class TrashBear : NPC
  {
    private int showWantBubbleTimer;
    private int itemWantedIndex;
    [XmlIgnore]
    private readonly NetEvent0 cutsceneEvent = new NetEvent0();
    [XmlIgnore]
    private readonly NetEvent1Field<int, NetInt> eatEvent = new NetEvent1Field<int, NetInt>();
    [XmlIgnore]
    private int itemBeingEaten;

    public TrashBear()
      : base(new AnimatedSprite("Characters\\TrashBear", 0, 32, 32), new Vector2(102f, 95f) * 64f, 0, nameof (TrashBear))
    {
      this.CurrentDialogue.Clear();
      this.HideShadow = true;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.cutsceneEvent, (INetSerializable) this.eatEvent);
      this.cutsceneEvent.onEvent += new NetEvent0.Event(this.doCutscene);
      this.eatEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doEatEvent);
    }

    public override bool checkAction(Farmer who, GameLocation l)
    {
      if (this.sprite.Value.CurrentAnimation != null)
        return false;
      if (who.ActiveObject != null && !(bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.bigCraftable && who.ActiveObject.ParentSheetIndex == this.itemWantedIndex)
      {
        this.tryToReceiveActiveObject(who);
        return true;
      }
      this.faceTowardFarmerForPeriod(4000, 3, false, who);
      this.shake(500);
      Game1.playSound("trashbear");
      this.showWantBubbleTimer = 3000;
      this.updateItemWanted();
      return false;
    }

    private void updateItemWanted()
    {
      int num1 = 0;
      if (NetWorldState.checkAnywhereForWorldStateID("trashBear1"))
        num1 = 1;
      if (NetWorldState.checkAnywhereForWorldStateID("trashBear2"))
        num1 = 2;
      if (NetWorldState.checkAnywhereForWorldStateID("trashBear3"))
        num1 = 3;
      int randomSeedAddition = 777111 + num1;
      this.itemWantedIndex = Utility.getRandomPureSeasonalItem(Game1.currentSeason, randomSeedAddition);
      if (num1 <= 1)
        return;
      int num2 = new Random((int) Game1.uniqueIDForThisGame + randomSeedAddition).Next(CraftingRecipe.cookingRecipes.Count);
      int num3 = 0;
      foreach (string str in CraftingRecipe.cookingRecipes.Values)
      {
        if (num3 == num2)
        {
          this.itemWantedIndex = Convert.ToInt32(str.Split('/')[2].Split(' ')[0]);
          break;
        }
        ++num3;
      }
    }

    public override void update(GameTime time, GameLocation location)
    {
      base.update(time, location);
      this.cutsceneEvent.Poll();
      this.eatEvent.Poll();
      if (this.showWantBubbleTimer <= 0)
        return;
      this.showWantBubbleTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
    }

    public override void tryToReceiveActiveObject(Farmer who)
    {
      this.updateItemWanted();
      if (who.ActiveObject == null || (bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.bigCraftable || who.ActiveObject.ParentSheetIndex != this.itemWantedIndex)
        return;
      Game1.currentLocation.playSound("coin");
      if (NetWorldState.checkAnywhereForWorldStateID("trashBear3"))
        NetWorldState.addWorldStateIDEverywhere("trashBearDone");
      else if (NetWorldState.checkAnywhereForWorldStateID("trashBear2"))
        NetWorldState.addWorldStateIDEverywhere("trashBear3");
      else if (NetWorldState.checkAnywhereForWorldStateID("trashBear1"))
        NetWorldState.addWorldStateIDEverywhere("trashBear2");
      else
        NetWorldState.addWorldStateIDEverywhere("trashBear1");
      this.eatEvent.Fire(this.itemWantedIndex);
      who.reduceActiveItemByOne();
    }

    public void doEatEvent(int item_index)
    {
      if (!(Game1.currentLocation is Forest))
        return;
      this.showWantBubbleTimer = 0;
      this.itemBeingEaten = item_index;
      this.sprite.Value.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(9, 1500, false, false, new AnimatedSprite.endOfAnimationBehavior(this.throwUpItem), true),
        new FarmerSprite.AnimationFrame(5, 1000, false, false),
        new FarmerSprite.AnimationFrame(6, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.chew)),
        new FarmerSprite.AnimationFrame(7, 250, false, false),
        new FarmerSprite.AnimationFrame(6, 250, false, false),
        new FarmerSprite.AnimationFrame(7, 250, false, false),
        new FarmerSprite.AnimationFrame(6, 250, false, false),
        new FarmerSprite.AnimationFrame(7, 500, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneAnimating), true)
      });
      this.sprite.Value.loop = false;
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.itemBeingEaten, 16, 16), 1500f, 1, 0, this.Position + new Vector2(96f, -92f), false, false, (float) (this.getStandingY() + 1) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
    }

    private void throwUpItem(Farmer who)
    {
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.itemBeingEaten, 16, 16), 1000f, 1, 0, this.Position + new Vector2(96f, -108f), false, false, (float) (this.getStandingY() + 1) / 10000f, 0.0f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
      {
        motion = new Vector2(-0.8f, -15f),
        acceleration = new Vector2(0.0f, 0.5f)
      });
      Game1.playSound("dwop");
    }

    private void chew(Farmer who)
    {
      Game1.playSound("eat");
      DelayedAction.playSoundAfterDelay("dirtyHit", 500);
      DelayedAction.playSoundAfterDelay("dirtyHit", 1000);
      DelayedAction.playSoundAfterDelay("gulp", 1400);
      for (int index = 0; index < 8; ++index)
      {
        Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.itemBeingEaten, 16, 16);
        standardTileSheet.X += 8;
        standardTileSheet.Y += 8;
        standardTileSheet.Width = 4;
        standardTileSheet.Height = 4;
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet, 400f, 1, 0, this.Position + new Vector2(64f, -48f), false, false, (float) ((double) this.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2((float) Game1.random.Next(-30, 31) / 10f, (float) Game1.random.Next(-6, -3)),
          acceleration = new Vector2(0.0f, 0.5f)
        });
      }
    }

    private void doneAnimating(Farmer who)
    {
      this.sprite.Value.CurrentFrame = 8;
      if (!NetWorldState.checkAnywhereForWorldStateID("trashBearDone") || !(Game1.currentLocation is Forest))
        return;
      this.doCutsceneEvent();
    }

    private void doCutsceneEvent() => this.cutsceneEvent.Fire();

    private void doCutscene()
    {
      if (!(Game1.currentLocation is Forest))
        return;
      if (Game1.activeClickableMenu != null && Game1.activeClickableMenu.readyToClose())
        Game1.activeClickableMenu.exitThisMenuNoSound();
      if (Game1.activeClickableMenu != null)
        return;
      Game1.player.freezePause = 2000;
      Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => Game1.currentLocation.startEvent(new StardewValley.Event("spring_day_ambient/-1000 -1000/farmer 104 95 3/skippable/addTemporaryActor TrashBear 32 32 102 95 0 false/animate TrashBear false true 250 0 1/viewport 102 97 clamp true/pause 3000/stopAnimation TrashBear/move TrashBear 0 2 2/faceDirection farmer 2/pause 1000/animate TrashBear false true 275 13 14 15 14/playSound trashbear_flute/specificTemporarySprite trashBearPrelude/viewport move -1 1 4000/pause 9000/stopAnimation TrashBear/playSound yoba/specificTemporarySprite trashBearMagic/pause 500/animate farmer false true 100 94/jump farmer/pause 2000/viewport move 1 -1 4000/stopAnimation farmer/move farmer 0 2 2/pause 4000/playSound trashbear/specificTemporarySprite trashBearUmbrella1/warp TrashBear -100 -100/pause 2000/faceDirection farmer 1/pause 2000/fade/viewport -5000 -5000/changeLocation Town/viewport 54 68 true/specificTemporarySprite trashBearTown/pause 10000/end", 777111))));
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this.showWantBubbleTimer <= 0)
        return;
      float num1 = (float) (2.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      Point tileLocationPoint = this.getTileLocationPoint();
      float num2 = (float) ((tileLocationPoint.Y + 1) * 64) / 10000f;
      float num3 = num1 - 40f;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (tileLocationPoint.X * 64 + 32), (float) (tileLocationPoint.Y * 64 - 96 - 48) + num3)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num2 + 1E-06f);
      b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (tileLocationPoint.X * 64 + 64 + 8), (float) (tileLocationPoint.Y * 64 - 64 - 32 - 8) + num3)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.itemWantedIndex, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num2 + 1E-05f);
    }
  }
}
