// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.GeodeMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class GeodeMenu : MenuWithInventory
  {
    public const int region_geodeSpot = 998;
    public ClickableComponent geodeSpot;
    public AnimatedSprite clint;
    public TemporaryAnimatedSprite geodeDestructionAnimation;
    public TemporaryAnimatedSprite sparkle;
    public int geodeAnimationTimer;
    public int yPositionOfGem;
    public int alertTimer;
    public float delayBeforeShowArtifactTimer;
    public Item geodeTreasure;
    public Item geodeTreasureOverride;
    public bool waitingForServerResponse;
    private List<TemporaryAnimatedSprite> fluffSprites = new List<TemporaryAnimatedSprite>();

    public GeodeMenu()
      : base(okButton: true, trashCan: true, inventoryXOffset: 12, inventoryYOffset: 132)
    {
      if (this.yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
        this.movePosition(0, -IClickableMenu.spaceToClearTopBorder);
      this.inventory.highlightMethod = new InventoryMenu.highlightThisItem(this.highlightGeodes);
      this.geodeSpot = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4, 560, 308), "")
      {
        myID = 998,
        downNeighborID = 0
      };
      this.clint = new AnimatedSprite("Characters\\Clint", 8, 32, 48);
      if (this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
      {
        for (int index = 0; index < 12; ++index)
        {
          if (this.inventory.inventory[index] != null)
            this.inventory.inventory[index].upNeighborID = 998;
        }
      }
      if (this.trashCan != null)
        this.trashCan.myID = 106;
      if (this.okButton != null)
        this.okButton.leftNeighborID = 11;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override bool readyToClose() => base.readyToClose() && this.geodeAnimationTimer <= 0 && this.heldItem == null && !this.waitingForServerResponse;

    public bool highlightGeodes(Item i) => this.heldItem != null || Utility.IsGeode(i);

    public virtual void startGeodeCrack()
    {
      this.geodeSpot.item = this.heldItem.getOne();
      --this.heldItem.Stack;
      if (this.heldItem.Stack <= 0)
        this.heldItem = (Item) null;
      this.geodeAnimationTimer = 2700;
      Game1.player.Money -= 25;
      Game1.playSound("stoneStep");
      this.clint.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(8, 300),
        new FarmerSprite.AnimationFrame(9, 200),
        new FarmerSprite.AnimationFrame(10, 80),
        new FarmerSprite.AnimationFrame(11, 200),
        new FarmerSprite.AnimationFrame(12, 100),
        new FarmerSprite.AnimationFrame(8, 300)
      });
      this.clint.loop = false;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.waitingForServerResponse)
        return;
      base.receiveLeftClick(x, y, true);
      if (!this.geodeSpot.containsPoint(x, y))
        return;
      if (this.heldItem != null && Utility.IsGeode(this.heldItem) && Game1.player.Money >= 25 && this.geodeAnimationTimer <= 0)
      {
        if (Game1.player.freeSpotsInInventory() > 1 || Game1.player.freeSpotsInInventory() == 1 && this.heldItem.Stack == 1)
        {
          if (this.heldItem.ParentSheetIndex == 791 && !Game1.netWorldState.Value.GoldenCoconutCracked.Value)
          {
            this.waitingForServerResponse = true;
            Game1.player.team.goldenCoconutMutex.RequestLock((Action) (() =>
            {
              this.waitingForServerResponse = false;
              this.geodeTreasureOverride = (Item) new StardewValley.Object(73, 1);
              this.startGeodeCrack();
            }), (Action) (() =>
            {
              this.waitingForServerResponse = false;
              this.startGeodeCrack();
            }));
          }
          else
            this.startGeodeCrack();
        }
        else
        {
          this.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_InventoryFull");
          this.wiggleWordsTimer = 500;
          this.alertTimer = 1500;
        }
      }
      else
      {
        if (Game1.player.Money >= 25)
          return;
        this.wiggleWordsTimer = 500;
        Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) => base.receiveRightClick(x, y, true);

    public override void performHoverAction(int x, int y)
    {
      if (this.alertTimer > 0)
        return;
      base.performHoverAction(x, y);
      if (!this.descriptionText.Equals(""))
        return;
      if (Game1.player.Money < 25)
        this.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_Description_NotEnoughMoney");
      else
        this.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_Description");
    }

    public override void emergencyShutDown()
    {
      base.emergencyShutDown();
      if (this.heldItem == null)
        return;
      Game1.player.addItemToInventoryBool(this.heldItem);
    }

    public override void update(GameTime time)
    {
      base.update(time);
      for (int index = this.fluffSprites.Count - 1; index >= 0; --index)
      {
        if (this.fluffSprites[index].update(time))
          this.fluffSprites.RemoveAt(index);
      }
      if (this.alertTimer > 0)
        this.alertTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.geodeAnimationTimer <= 0)
        return;
      Game1.changeMusicTrack("none");
      this.geodeAnimationTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.geodeAnimationTimer <= 0)
      {
        this.geodeDestructionAnimation = (TemporaryAnimatedSprite) null;
        this.geodeSpot.item = (Item) null;
        if (this.geodeTreasure != null && Utility.IsNormalObjectAtParentSheetIndex(this.geodeTreasure, 73))
          Game1.netWorldState.Value.GoldenCoconutCracked.Value = true;
        Game1.player.addItemToInventoryBool(this.geodeTreasure);
        this.geodeTreasure = (Item) null;
        this.yPositionOfGem = 0;
        this.fluffSprites.Clear();
        this.delayBeforeShowArtifactTimer = 0.0f;
      }
      else
      {
        int currentFrame = this.clint.currentFrame;
        this.clint.animateOnce(time);
        if (this.clint.currentFrame == 11 && currentFrame != 11)
        {
          if (this.geodeSpot.item != null && (int) (NetFieldBase<int, NetInt>) this.geodeSpot.item.parentSheetIndex == 275)
          {
            Game1.playSound("hammer");
            Game1.playSound("woodWhack");
          }
          else
          {
            Game1.playSound("hammer");
            Game1.playSound("stoneCrack");
          }
          ++Game1.stats.GeodesCracked;
          int y = 448;
          if (this.geodeSpot.item != null)
          {
            switch ((int) (NetFieldBase<int, NetInt>) (this.geodeSpot.item as StardewValley.Object).parentSheetIndex)
            {
              case 536:
                y += 64;
                break;
              case 537:
                y += 128;
                break;
            }
            this.geodeDestructionAnimation = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, y, 64, 64), 100f, 8, 0, new Vector2((float) (this.geodeSpot.bounds.X + 392 - 32), (float) (this.geodeSpot.bounds.Y + 192 - 32)), false, false);
            if (this.geodeSpot.item != null && (int) (NetFieldBase<int, NetInt>) this.geodeSpot.item.parentSheetIndex == 275)
            {
              this.geodeDestructionAnimation = new TemporaryAnimatedSprite()
              {
                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                sourceRect = new Rectangle(388, 123, 18, 21),
                sourceRectStartingPos = new Vector2(388f, 123f),
                animationLength = 6,
                position = new Vector2((float) (this.geodeSpot.bounds.X + 380 - 32), (float) (this.geodeSpot.bounds.Y + 192 - 32)),
                holdLastFrame = true,
                interval = 100f,
                id = 777f,
                scale = 4f
              };
              for (int index = 0; index < 6; ++index)
              {
                this.fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2((float) (this.geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21)), (float) (this.geodeSpot.bounds.Y + 192 - 16)), false, 1f / 500f, new Color((int) byte.MaxValue, 222, 198))
                {
                  alphaFade = 0.02f,
                  motion = new Vector2((float) Game1.random.Next(-20, 21) / 10f, (float) Game1.random.Next(5, 20) / 10f),
                  interval = 99999f,
                  layerDepth = 0.9f,
                  scale = 3f,
                  scaleChange = 0.01f,
                  rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                  delayBeforeAnimationStart = index * 20
                });
                this.fluffSprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                  sourceRect = new Rectangle(499, 132, 5, 5),
                  sourceRectStartingPos = new Vector2(499f, 132f),
                  motion = new Vector2((float) Game1.random.Next(-30, 31) / 10f, (float) Game1.random.Next(-7, -4)),
                  acceleration = new Vector2(0.0f, 0.25f),
                  totalNumberOfLoops = 1,
                  interval = 1000f,
                  alphaFade = 0.015f,
                  animationLength = 1,
                  layerDepth = 1f,
                  scale = 4f,
                  rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                  delayBeforeAnimationStart = index * 10,
                  position = new Vector2((float) (this.geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21)), (float) (this.geodeSpot.bounds.Y + 192 - 16))
                });
                this.delayBeforeShowArtifactTimer = 500f;
              }
            }
            if (this.geodeTreasureOverride != null)
            {
              this.geodeTreasure = this.geodeTreasureOverride;
              this.geodeTreasureOverride = (Item) null;
            }
            else
              this.geodeTreasure = Utility.getTreasureFromGeode(this.geodeSpot.item);
            if ((int) (NetFieldBase<int, NetInt>) this.geodeSpot.item.parentSheetIndex != 275 && (!(this.geodeTreasure is StardewValley.Object) || !(this.geodeTreasure as StardewValley.Object).Type.Contains("Mineral")) && this.geodeTreasure is StardewValley.Object && (this.geodeTreasure as StardewValley.Object).Type.Contains("Arch") && !Game1.player.hasOrWillReceiveMail("artifactFound"))
              this.geodeTreasure = (Item) new StardewValley.Object(390, 5);
          }
        }
        if (this.geodeDestructionAnimation != null && ((double) this.geodeDestructionAnimation.id != 777.0 && this.geodeDestructionAnimation.currentParentTileIndex < 7 || (double) this.geodeDestructionAnimation.id == 777.0 && this.geodeDestructionAnimation.currentParentTileIndex < 5))
        {
          this.geodeDestructionAnimation.update(time);
          if ((double) this.delayBeforeShowArtifactTimer > 0.0)
          {
            this.delayBeforeShowArtifactTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
            if ((double) this.delayBeforeShowArtifactTimer <= 0.0)
            {
              this.fluffSprites.Add(this.geodeDestructionAnimation);
              this.fluffSprites.Reverse();
              this.geodeDestructionAnimation = new TemporaryAnimatedSprite()
              {
                interval = 100f,
                animationLength = 6,
                alpha = 1f / 1000f,
                id = 777f
              };
            }
          }
          else
          {
            if (this.geodeDestructionAnimation.currentParentTileIndex < 3)
              --this.yPositionOfGem;
            --this.yPositionOfGem;
            if ((this.geodeDestructionAnimation.currentParentTileIndex == 7 || (double) this.geodeDestructionAnimation.id == 777.0 && this.geodeDestructionAnimation.currentParentTileIndex == 5) && (!(this.geodeTreasure is StardewValley.Object) || (int) (NetFieldBase<int, NetInt>) (this.geodeTreasure as StardewValley.Object).price > 75))
            {
              this.sparkle = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 640, 64, 64), 100f, 8, 0, new Vector2((float) (this.geodeSpot.bounds.X + 392 - 32), (float) (this.geodeSpot.bounds.Y + 192 + this.yPositionOfGem - 32)), false, false);
              Game1.playSound("discoverMineral");
            }
            else if ((this.geodeDestructionAnimation.currentParentTileIndex == 7 || (double) this.geodeDestructionAnimation.id == 777.0 && this.geodeDestructionAnimation.currentParentTileIndex == 5) && this.geodeTreasure is StardewValley.Object && (int) (NetFieldBase<int, NetInt>) (this.geodeTreasure as StardewValley.Object).price <= 75)
              Game1.playSound("newArtifact");
          }
        }
        if (this.sparkle == null || !this.sparkle.update(time))
          return;
        this.sparkle = (TemporaryAnimatedSprite) null;
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.geodeSpot = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4, 560, 308), "Anvil");
      int yPosition = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16 + 128 + 4;
      this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 12, yPosition, false, highlightMethod: this.inventory.highlightMethod);
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
      this.draw(b, true, true, -1, -1, -1);
      Game1.dayTimeMoneyBox.drawMoneyBox(b);
      b.Draw(Game1.mouseCursors, new Vector2((float) this.geodeSpot.bounds.X, (float) this.geodeSpot.bounds.Y), new Rectangle?(new Rectangle(0, 512, 140, 78)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      if (this.geodeSpot.item != null)
      {
        if (this.geodeDestructionAnimation == null)
          this.geodeSpot.item.drawInMenu(b, new Vector2((float) (this.geodeSpot.bounds.X + 360 + ((int) (NetFieldBase<int, NetInt>) this.geodeSpot.item.parentSheetIndex == 275 ? -8 : 0)), (float) (this.geodeSpot.bounds.Y + 160 + ((int) (NetFieldBase<int, NetInt>) this.geodeSpot.item.parentSheetIndex == 275 ? 8 : 0))), 1f);
        else
          this.geodeDestructionAnimation.draw(b, true);
        foreach (TemporaryAnimatedSprite fluffSprite in this.fluffSprites)
          fluffSprite.draw(b, true);
        if (this.geodeTreasure != null && (double) this.delayBeforeShowArtifactTimer <= 0.0)
          this.geodeTreasure.drawInMenu(b, new Vector2((float) (this.geodeSpot.bounds.X + 360), (float) (this.geodeSpot.bounds.Y + 160 + this.yPositionOfGem)), 1f);
        if (this.sparkle != null)
          this.sparkle.draw(b, true);
      }
      this.clint.draw(b, new Vector2((float) (this.geodeSpot.bounds.X + 384), (float) (this.geodeSpot.bounds.Y + 64)), 0.877f);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      if (this.heldItem != null)
        this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
      if (Game1.options.hardwareCursor)
        return;
      this.drawMouse(b);
    }
  }
}
