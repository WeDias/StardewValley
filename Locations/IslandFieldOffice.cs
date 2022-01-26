// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandFieldOffice
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandFieldOffice : IslandLocation
  {
    public const int totalPieces = 11;
    public const int piece_Skeleton_Back_Leg = 0;
    public const int piece_Skeleton_Ribs = 1;
    public const int piece_Skeleton_Front_Leg = 2;
    public const int piece_Skeleton_Tail = 3;
    public const int piece_Skeleton_Spine = 4;
    public const int piece_Skeleton_Skull = 5;
    public const int piece_Snake_Tail = 6;
    public const int piece_Snake_Spine = 7;
    public const int piece_Snake_Skull = 8;
    public const int piece_Bat = 9;
    public const int piece_Frog = 10;
    [XmlElement("uncollectedRewards")]
    public NetList<Item, NetRef<Item>> uncollectedRewards = new NetList<Item, NetRef<Item>>();
    [XmlIgnore]
    public NetMutex safariGuyMutex = new NetMutex();
    private NPC safariGuy;
    [XmlElement("piecesDonated")]
    public NetList<bool, NetBool> piecesDonated = new NetList<bool, NetBool>(11);
    [XmlElement("centerSkeletonRestored")]
    public readonly NetBool centerSkeletonRestored = new NetBool();
    [XmlElement("snakeRestored")]
    public readonly NetBool snakeRestored = new NetBool();
    [XmlElement("batRestored")]
    public readonly NetBool batRestored = new NetBool();
    [XmlElement("frogRestored")]
    public readonly NetBool frogRestored = new NetBool();
    [XmlElement("plantsRestoredLeft")]
    public readonly NetBool plantsRestoredLeft = new NetBool();
    [XmlElement("plantsRestoredRight")]
    public readonly NetBool plantsRestoredRight = new NetBool();
    public readonly NetBool hasFailedSurveyToday = new NetBool();
    private bool _shouldTriggerFinalCutscene;
    private float speakerTimer;

    public IslandFieldOffice()
    {
    }

    public IslandFieldOffice(string map, string name)
      : base(map, name)
    {
      while (this.piecesDonated.Count < 11)
        this.piecesDonated.Add(false);
    }

    public NPC getSafariGuy() => this.safariGuy;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.piecesDonated, (INetSerializable) this.centerSkeletonRestored, (INetSerializable) this.snakeRestored, (INetSerializable) this.batRestored, (INetSerializable) this.frogRestored, (INetSerializable) this.plantsRestoredLeft, (INetSerializable) this.plantsRestoredRight, (INetSerializable) this.uncollectedRewards, (INetSerializable) this.hasFailedSurveyToday, (INetSerializable) this.safariGuyMutex.NetFields);
      this.centerSkeletonRestored.InterpolationWait = false;
      this.centerSkeletonRestored.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplySkeletonRestore();
      });
      this.snakeRestored.InterpolationWait = false;
      this.snakeRestored.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplySnakeRestore();
      });
      this.batRestored.InterpolationWait = false;
      this.batRestored.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyBatRestore();
      });
      this.frogRestored.InterpolationWait = false;
      this.frogRestored.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyFrogRestore();
      });
      this.plantsRestoredLeft.InterpolationWait = false;
      this.plantsRestoredLeft.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyPlantRestoreLeft();
      });
      this.plantsRestoredRight.InterpolationWait = false;
      this.plantsRestoredRight.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyPlantRestoreRight();
      });
    }

    private void ApplyPlantRestoreLeft()
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f, new Color(0, 220, 150))
      {
        layerDepth = 1f,
        motion = new Vector2(1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(0, 220, 150) * 0.75f)
      {
        scale = 0.75f,
        flipped = true,
        layerDepth = 1f,
        motion = new Vector2(-1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(0, 220, 150) * 0.75f)
      {
        scale = 0.75f,
        delayBeforeAnimationStart = 50,
        layerDepth = 1f,
        motion = new Vector2(1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(0, 220, 150) * 0.75f)
      {
        scale = 0.75f,
        flipped = true,
        delayBeforeAnimationStart = 100,
        layerDepth = 1f,
        motion = new Vector2(-1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(250, 100, 250) * 0.75f)
      {
        scale = 0.75f,
        flipped = true,
        delayBeforeAnimationStart = 150,
        layerDepth = 1f,
        motion = new Vector2(0.0f, -3f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      if (Game1.gameMode == (byte) 6 || Utility.ShouldIgnoreValueChangeCallback())
        return;
      if (Game1.currentLocation == this)
      {
        Game1.playSound("leafrustle");
        DelayedAction.playSoundAfterDelay("leafrustle", 150);
      }
      if (!Game1.IsMasterGame)
        return;
      Game1.player.team.MarkCollectedNut("IslandLeftPlantRestored");
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(1.5f, 3.3f) * 64f, 1, (GameLocation) this, 256);
    }

    private void ApplyPlantRestoreRight()
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(7.5f, 3.3f) * 64f, new Color(0, 220, 150))
      {
        layerDepth = 1f,
        motion = new Vector2(1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(0, 220, 150) * 0.75f)
      {
        scale = 0.75f,
        flipped = true,
        layerDepth = 1f,
        motion = new Vector2(-1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8.3f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(0, 200, 120) * 0.75f)
      {
        scale = 0.75f,
        delayBeforeAnimationStart = 50,
        layerDepth = 1f,
        motion = new Vector2(1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(0, 220, 150) * 0.75f)
      {
        scale = 0.75f,
        flipped = true,
        delayBeforeAnimationStart = 100,
        layerDepth = 1f,
        motion = new Vector2(-1f, -4f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8.5f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), new Color(0, 250, 180) * 0.75f)
      {
        scale = 0.75f,
        flipped = true,
        delayBeforeAnimationStart = 150,
        layerDepth = 1f,
        motion = new Vector2(0.0f, -3f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      if (Game1.gameMode == (byte) 6 || Utility.ShouldIgnoreValueChangeCallback())
        return;
      if (Game1.currentLocation == this)
      {
        Game1.playSound("leafrustle");
        DelayedAction.playSoundAfterDelay("leafrustle", 150);
      }
      if (!Game1.IsMasterGame)
        return;
      Game1.player.team.MarkCollectedNut("IslandRightPlantRestored");
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(7.5f, 3.3f) * 64f, 3, (GameLocation) this, 256);
    }

    private void ApplyFrogRestore()
    {
      if (Game1.gameMode != (byte) 6 && !Utility.ShouldIgnoreValueChangeCallback() && Game1.currentLocation == this)
        Game1.playSound("dirtyHit");
      for (int index = 0; index < 3; ++index)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) (6.5 + (double) Game1.random.Next(-10, 11) / 100.0), 3f) * 64f, false, 0.007f, Color.White)
        {
          alpha = 0.75f,
          motion = new Vector2(0.0f, -1f),
          acceleration = new Vector2(1f / 500f, 0.0f),
          interval = 99999f,
          layerDepth = 1f,
          scale = 4f,
          scaleChange = 0.02f,
          rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
          delayBeforeAnimationStart = index * 100
        });
    }

    private void ApplyBatRestore()
    {
      if (Game1.gameMode != (byte) 6 && !Utility.ShouldIgnoreValueChangeCallback() && Game1.currentLocation == this)
        Game1.playSound("dirtyHit");
      for (int index = 0; index < 3; ++index)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) (2.5 + (double) Game1.random.Next(-10, 11) / 100.0), 3f) * 64f, false, 0.007f, Color.White)
        {
          alpha = 0.75f,
          motion = new Vector2(0.0f, -1f),
          acceleration = new Vector2(1f / 500f, 0.0f),
          interval = 99999f,
          layerDepth = 1f,
          scale = 4f,
          scaleChange = 0.02f,
          rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
          delayBeforeAnimationStart = index * 100
        });
    }

    private void ApplySnakeRestore()
    {
    }

    private void ApplySkeletonRestore()
    {
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      IslandFieldOffice islandFieldOffice = l as IslandFieldOffice;
      this.uncollectedRewards.Clear();
      this.uncollectedRewards.Set((IList<Item>) islandFieldOffice.uncollectedRewards);
      this.piecesDonated.Clear();
      this.piecesDonated.Set((IList<bool>) islandFieldOffice.piecesDonated);
      this.centerSkeletonRestored.Value = islandFieldOffice.centerSkeletonRestored.Value;
      this.snakeRestored.Value = islandFieldOffice.snakeRestored.Value;
      this.batRestored.Value = islandFieldOffice.batRestored.Value;
      this.frogRestored.Value = islandFieldOffice.frogRestored.Value;
      this.plantsRestoredLeft.Value = islandFieldOffice.plantsRestoredLeft.Value;
      this.plantsRestoredRight.Value = islandFieldOffice.plantsRestoredRight.Value;
      this.hasFailedSurveyToday.Value = islandFieldOffice.hasFailedSurveyToday.Value;
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened") && this.safariGuy == null)
        this.safariGuy = new NPC(new AnimatedSprite("Characters\\SafariGuy", 0, 16, 32), new Vector2(8f, 6f) * 64f, "IslandFieldOFfice", 2, "Professor Snail", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\SafariGuy"));
      if (this.safariGuy != null && !Game1.player.hasOrWillReceiveMail("safariGuyIntro"))
      {
        this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Intro_Event")));
        Game1.player.mailReceived.Add("safariGuyIntro");
        Game1.player.Halt();
      }
      else
      {
        if (this.safariGuy != null)
        {
          Game1.changeMusicTrack("fieldofficeTentMusic");
          if (Game1.random.NextDouble() < 0.5)
          {
            this.safariGuy.Halt();
            this.safariGuy.showTextAboveHead(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Welcome_" + Game1.random.Next(4).ToString()));
            this.safariGuy.faceTowardFarmerForPeriod(60000, 5, false, Game1.player);
          }
          else
            this.safariGuy.Sprite.CurrentAnimation = new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(18, 900, 0, false, false),
              new FarmerSprite.AnimationFrame(19, 900, 0, false, false)
            };
        }
        if (Game1.player.hasOrWillReceiveMail("fieldOfficeFinale") || !this.isRangeAllTrue(0, 11) || !this.plantsRestoredRight.Value || !this.plantsRestoredLeft.Value || this.currentEvent != null)
          return;
        this._StartFinaleEvent();
      }
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (!(Game1.getMusicTrackName() == "fieldofficeTentMusic"))
        return;
      Game1.changeMusicTrack("none");
    }

    /// <summary>returns true if a new uncollected reward was added.</summary>
    /// <param name="which"></param>
    /// <returns></returns>
    public bool donatePiece(int which)
    {
      this.piecesDonated[which] = true;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.centerSkeletonRestored && this.isRangeAllTrue(0, 6))
      {
        this.centerSkeletonRestored.Value = true;
        this.uncollectedRewards.Add((Item) new StardewValley.Object(73, 6));
        this.uncollectedRewards.Add((Item) new StardewValley.Object(69, 1));
        Game1.player.team.MarkCollectedNut("IslandCenterSkeletonRestored");
        return true;
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.snakeRestored && this.isRangeAllTrue(6, 9))
      {
        this.snakeRestored.Value = true;
        this.uncollectedRewards.Add((Item) new StardewValley.Object(73, 3));
        this.uncollectedRewards.Add((Item) new StardewValley.Object(835, 1));
        Game1.player.team.MarkCollectedNut("IslandSnakeRestored");
        return true;
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.batRestored && this.piecesDonated[9])
      {
        this.batRestored.Value = true;
        this.uncollectedRewards.Add((Item) new StardewValley.Object(73, 1));
        Game1.player.team.MarkCollectedNut("IslandBatRestored");
        return true;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.frogRestored || !this.piecesDonated[10])
        return false;
      this.frogRestored.Value = true;
      this.uncollectedRewards.Add((Item) new StardewValley.Object(73, 1));
      Game1.player.team.MarkCollectedNut("IslandFrogRestored");
      return true;
    }

    public bool isRangeAllTrue(int low, int high)
    {
      for (int index = low; index < high; ++index)
      {
        if (!this.piecesDonated[index])
          return false;
      }
      return true;
    }

    public void triggerFinaleCutscene() => this._shouldTriggerFinalCutscene = true;

    private void _triggerFinaleCutsceneActual()
    {
      Game1.player.Halt();
      Game1.player.freezePause = 500;
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
      {
        if (Game1.activeClickableMenu != null)
          Game1.activeClickableMenu = (IClickableMenu) null;
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this._StartFinaleEvent()));
      }), 500);
      this._shouldTriggerFinalCutscene = false;
    }

    protected void _StartFinaleEvent()
    {
      if (this.safariGuy != null)
        this.safariGuy.clearTextAboveHead();
      this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:FieldOfficeFinale")));
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this.safariGuy != null && !Game1.eventUp)
        this.safariGuy.draw(b);
      if ((bool) (NetFieldBase<bool, NetBool>) this.centerSkeletonRestored)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(3f, 4f) * 64f + new Vector2(0.0f, 4f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(210, 184, 46, 43)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0512f);
      if ((bool) (NetFieldBase<bool, NetBool>) this.snakeRestored)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(1f, 5f) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(195, 185, 14, 42)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448f);
      if ((bool) (NetFieldBase<bool, NetBool>) this.batRestored)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(2.5f, 2.7f) * 64f + new Vector2(1f, 1f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(212, 171, 16, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0256f);
      if ((bool) (NetFieldBase<bool, NetBool>) this.frogRestored)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(6f, 2f) * 64f + new Vector2(9f, 10f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(232, 169, 14, 15)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0256f);
      if ((bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredLeft)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(1f, 4f) * 64f + new Vector2(0.0f, -7f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194, 167, 16, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.032f);
      if ((bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredRight)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(7f, 3f) * 64f + new Vector2(8f, 3f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(224, 148, 32, 21)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.032f);
      if (this.safariGuy == null || (bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredLeft && (bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredRight || Game1.eventUp)
        return;
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / 250.0), 2));
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(324f, 144f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(220, 160, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 16.0)), SpriteEffects.None, 1f);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      if (this.safariGuy == null)
        return;
      this.safariGuy.drawAboveAlwaysFrontLayer(b);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.safariGuyMutex.Update((GameLocation) this);
      if (this.safariGuy != null)
      {
        this.safariGuy.update(time, (GameLocation) this);
        this.speakerTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
        if ((double) this.speakerTimer <= 0.0)
        {
          this.speakerTimer = 600f;
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(211, 161, 5, 5), new Vector2(74.75f, 20.75f) * 4f, false, 0.0f, Color.White)
          {
            scale = 5f,
            scaleChange = -0.05f,
            motion = new Vector2(0.125f, 0.125f),
            animationLength = 1,
            totalNumberOfLoops = 1,
            interval = 400f,
            layerDepth = 1f
          });
        }
      }
      if (Game1.currentLocation != this || !this._shouldTriggerFinalCutscene || Game1.activeClickableMenu != null)
        return;
      this._triggerFinaleCutsceneActual();
    }

    public virtual void OnCollectReward(Item item, Farmer farmer)
    {
      if (!(Game1.activeClickableMenu is ItemGrabMenu) || (Game1.activeClickableMenu as ItemGrabMenu).context != this)
        return;
      ItemGrabMenu activeClickableMenu = Game1.activeClickableMenu as ItemGrabMenu;
      if (Game1.player.addItemToInventoryBool(activeClickableMenu.heldItem))
      {
        this.uncollectedRewards.Remove(item);
        activeClickableMenu.ItemsToGrabMenu.actualInventory = (IList<Item>) new List<Item>((IEnumerable<Item>) this.uncollectedRewards);
        activeClickableMenu.heldItem = (Item) null;
        if ((int) (NetFieldBase<int, NetInt>) item.parentSheetIndex == 73)
          return;
        Game1.playSound("coin");
      }
      else
      {
        Game1.playSound("cancel");
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        activeClickableMenu.ItemsToGrabMenu.actualInventory = (IList<Item>) new List<Item>((IEnumerable<Item>) this.uncollectedRewards);
        activeClickableMenu.heldItem = (Item) null;
      }
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "PurpleFlowerSurvey_Correct":
          Game1.drawDialogue(this.safariGuy, Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Correct"));
          this.plantsRestoredLeft.Value = true;
          Game1.multiplayer.globalChatInfoMessage("FinishedSurvey", (string) (NetFieldBase<string, NetString>) Game1.player.name);
          break;
        case "PurpleFlowerSurvey_Wrong":
          Game1.drawDialogue(this.safariGuy, Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Wrong"));
          this.hasFailedSurveyToday.Value = true;
          break;
        case "PurpleStarfishSurvey_Correct":
          Game1.drawDialogue(this.safariGuy, Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Correct"));
          this.plantsRestoredRight.Value = true;
          Game1.multiplayer.globalChatInfoMessage("FinishedSurvey", (string) (NetFieldBase<string, NetString>) Game1.player.name);
          break;
        case "PurpleStarfishSurvey_Wrong":
          Game1.drawDialogue(this.safariGuy, Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Wrong"));
          this.hasFailedSurveyToday.Value = true;
          break;
        case "Safari_Collect":
          Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) new List<Item>((IEnumerable<Item>) this.uncollectedRewards), false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, "Rewards", new ItemGrabMenu.behaviorOnItemSelect(this.OnCollectReward), canBeExitedWithKey: true, playRightClickSound: false, allowRightClick: false, context: ((object) this));
          Game1.activeClickableMenu.exitFunction += (IClickableMenu.onExit) (() => this.safariGuyMutex.ReleaseLock());
          break;
        case "Safari_Donate":
          Game1.activeClickableMenu = (IClickableMenu) new FieldOfficeMenu(this);
          Game1.activeClickableMenu.exitFunction += (IClickableMenu.onExit) (() => this.safariGuyMutex.ReleaseLock());
          break;
        case "Safari_Hint":
          int num = this.getRandomUnfoundBoneIndex();
          if (num == 823)
            num = 824;
          Game1.drawDialogue(this.safariGuy, Game1.content.LoadString("Data\\ExtraDialogue:ProfessorSnail_Hint_" + num.ToString()));
          break;
        case "Safari_Leave":
          this.safariGuyMutex.ReleaseLock();
          break;
        case "Survey_Yes":
          if (!(bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredLeft)
          {
            List<Response> responseList = new List<Response>();
            for (int index = 18; index < 25; ++index)
              responseList.Add(new Response(index == 22 ? "Correct" : "Wrong", index.ToString() ?? ""));
            responseList.Add(new Response("No", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")).SetHotKey(Keys.Escape));
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Question"), responseList.ToArray(), "PurpleFlowerSurvey");
            break;
          }
          if (!(bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredRight)
          {
            List<Response> responseList = new List<Response>();
            for (int index = 11; index < 19; ++index)
              responseList.Add(new Response(index == 18 ? "Correct" : "Wrong", index.ToString() ?? ""));
            responseList.Add(new Response("No", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")).SetHotKey(Keys.Escape));
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleStarfish_Question"), responseList.ToArray(), "PurpleStarfishSurvey");
            break;
          }
          break;
        case null:
          return false;
      }
      if (!Game1.player.hasOrWillReceiveMail("fieldOfficeFinale") && this.isRangeAllTrue(0, 11) && this.plantsRestoredRight.Value && this.plantsRestoredLeft.Value)
        this.triggerFinaleCutscene();
      return base.answerDialogueAction(questionAndAnswer, questionParams);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      this.hasFailedSurveyToday.Value = false;
      base.DayUpdate(dayOfMonth);
    }

    public virtual void TalkToSafariGuy()
    {
      List<Response> responseList = new List<Response>();
      responseList.Add(new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")));
      if (this.uncollectedRewards.Count > 0)
        responseList.Add(new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")));
      if (this.getRandomUnfoundBoneIndex() != -1)
        responseList.Add(new Response("Hint", Game1.content.LoadString("Strings\\Locations:Hint")));
      responseList.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave")));
      this.createQuestionDialogue("", responseList.ToArray(), "Safari");
    }

    private int getRandomUnfoundBoneIndex()
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      for (int index = 0; index < 25; ++index)
      {
        int num = random.Next(11);
        if (!this.piecesDonated[num])
          return FieldOfficeMenu.getDonationPieceIndexNeededForSpot(num);
      }
      for (int index = 0; index < this.piecesDonated.Count; ++index)
      {
        if (!this.piecesDonated[index])
          return FieldOfficeMenu.getDonationPieceIndexNeededForSpot(index);
      }
      return -1;
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action == "FieldOfficeDesk")
      {
        if (this.safariGuy != null)
        {
          this.safariGuyMutex.RequestLock(new Action(this.TalkToSafariGuy));
          return true;
        }
      }
      else if (action == "FieldOfficeSurvey" && this.safariGuy != null)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.hasFailedSurveyToday)
        {
          Game1.drawDialogue(this.safariGuy, Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Failed"));
          return true;
        }
        if (!(bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredLeft)
        {
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Prompt_LeftPlant"), new List<Response>()
          {
            new Response("Yes", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Yes")),
            new Response("No", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Notyet"))
          }.ToArray(), "Survey");
          (Game1.activeClickableMenu as DialogueBox).aboveDialogueImage = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(194, 167, 16, 17), 1f, 1, 1, Vector2.Zero, false, false)
          {
            scale = 4f
          };
        }
        else if (!(bool) (NetFieldBase<bool, NetBool>) this.plantsRestoredRight)
        {
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Prompt_RightPlant"), new List<Response>()
          {
            new Response("Yes", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Yes")),
            new Response("No", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Notyet"))
          }.ToArray(), "Survey");
          (Game1.activeClickableMenu as DialogueBox).aboveDialogueImage = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(193, 150, 16, 16), 1f, 1, 1, Vector2.Zero, false, false)
          {
            scale = 4f
          };
        }
        return true;
      }
      return base.performAction(action, who, tileLocation);
    }
  }
}
