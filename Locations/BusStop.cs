// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BusStop
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using System;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class BusStop : GameLocation
  {
    public const int busDefaultXTile = 11;
    public const int busDefaultYTile = 6;
    private TemporaryAnimatedSprite minecartSteam;
    private TemporaryAnimatedSprite busDoor;
    private Vector2 busPosition;
    private Vector2 busMotion;
    public bool drivingOff;
    public bool drivingBack;
    public bool leaving;
    private int forceWarpTimer;
    private Microsoft.Xna.Framework.Rectangle busSource = new Microsoft.Xna.Framework.Rectangle(288, 1247, 128, 64);
    private Microsoft.Xna.Framework.Rectangle pamSource = new Microsoft.Xna.Framework.Rectangle(384, 1311, 15, 19);
    private Vector2 pamOffset = new Vector2(0.0f, 29f);

    public BusStop()
    {
    }

    public BusStop(string mapPath, string name)
      : base(mapPath, name)
    {
      this.busPosition = new Vector2(11f, 6f) * 64f;
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null)
      {
        switch (this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex)
        {
          case 958:
          case 1080:
          case 1081:
            if (Game1.player.mount != null)
              return true;
            if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
            {
              if (Game1.player.isRidingHorse() && Game1.player.mount != null)
              {
                Game1.player.mount.checkAction(Game1.player, (GameLocation) this);
              }
              else
              {
                Response[] answerChoices;
                if (Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom"))
                  answerChoices = new Response[4]
                  {
                    new Response("Mines", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines")),
                    new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
                    new Response("Quarry", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Quarry")),
                    new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
                  };
                else
                  answerChoices = new Response[3]
                  {
                    new Response("Mines", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines")),
                    new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
                    new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
                  };
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), answerChoices, "Minecart");
                break;
              }
            }
            else
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
            return true;
          case 1057:
            if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
            {
              if (Game1.player.isRidingHorse() && Game1.player.mount != null)
              {
                Game1.player.mount.checkAction(Game1.player, (GameLocation) this);
              }
              else
              {
                if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.es)
                {
                  this.createQuestionDialogueWithCustomWidth(Game1.content.LoadString("Strings\\Locations:BusStop_BuyTicketToDesert"), this.createYesNoResponses(), "Bus");
                  break;
                }
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_BuyTicketToDesert"), this.createYesNoResponses(), "Bus");
                break;
              }
            }
            else
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_DesertOutOfService"));
            return true;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    private void playerReachedBusDoor(Character c, GameLocation l)
    {
      this.forceWarpTimer = 0;
      Game1.player.position.X = -10000f;
      Game1.changeMusicTrack("none");
      this.busDriveOff();
      this.playSound("stoneStep");
      if (Game1.player.mount == null)
        return;
      Game1.player.mount.farmerPassesThrough = false;
    }

    public override bool answerDialogue(Response answer)
    {
      if (this.lastQuestionKey == null || this.afterQuestion != null || !(this.lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey == "Bus_Yes"))
        return base.answerDialogue(answer);
      NPC characterFromName = Game1.getCharacterFromName("Pam");
      if (Game1.player.Money >= (Game1.shippingTax ? 50 : 500) && this.characters.Contains(characterFromName) && characterFromName.getTileLocation().Equals(new Vector2(11f, 10f)))
      {
        Game1.player.Money -= Game1.shippingTax ? 50 : 500;
        Game1.freezeControls = true;
        Game1.viewportFreeze = true;
        this.forceWarpTimer = 8000;
        Game1.player.controller = new PathFindController((Character) Game1.player, (GameLocation) this, new Point(12, 9), 0, new PathFindController.endBehavior(this.playerReachedBusDoor));
        Game1.player.setRunning(true);
        if (Game1.player.mount != null)
          Game1.player.mount.farmerPassesThrough = true;
        Desert.warpedToDesert = false;
      }
      else if (Game1.player.Money < (Game1.shippingTax ? 50 : 500))
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NoDriver"));
      return true;
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.leaving = false;
      if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
        this.minecartSteam = new TemporaryAnimatedSprite(27, new Vector2(392f, 144f), Color.White)
        {
          totalNumberOfLoops = 999999,
          interval = 60f,
          flipped = true
        };
      if ((int) (NetFieldBase<int, NetInt>) Game1.getFarm().grandpaScore == 0 && Game1.year >= 3 && Game1.player.eventsSeen.Contains(558292))
        Game1.player.eventsSeen.Remove(558292);
      if (Game1.player.getTileY() > 16 || Game1.eventUp || Game1.player.getTileX() == 0 || Game1.player.isRidingHorse() || !Game1.player.previousLocationName.Equals("Desert"))
      {
        this.drivingOff = false;
        this.drivingBack = false;
        this.busMotion = Vector2.Zero;
        this.busPosition = new Vector2(11f, 6f) * 64f;
        this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
        {
          interval = 999999f,
          animationLength = 6,
          holdLastFrame = true,
          layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
          scale = 4f
        };
      }
      else
      {
        this.busPosition = new Vector2(11f, 6f) * 64f;
        this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
        {
          interval = 999999f,
          animationLength = 1,
          holdLastFrame = true,
          layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
          scale = 4f
        };
        Game1.displayFarmer = false;
        this.busDriveBack();
      }
      if (Game1.player.getTileY() <= 16 || !Game1.MasterPlayer.mailReceived.Contains("Capsule_Broken") || !Game1.isDarkOut() || Game1.random.NextDouble() >= 0.01)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16, 25), new Vector2(12f, 6.5f) * 64f, true, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(-3f, 0.0f),
        animationLength = 4,
        interval = 80f,
        totalNumberOfLoops = 200,
        layerDepth = 0.0448f,
        delayBeforeAnimationStart = Game1.random.Next(1500)
      });
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (this.farmers.Count > 1)
        return;
      this.minecartSteam = (TemporaryAnimatedSprite) null;
      this.busDoor = (TemporaryAnimatedSprite) null;
    }

    public void busDriveOff()
    {
      this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
      {
        interval = 999999f,
        animationLength = 6,
        holdLastFrame = true,
        layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
        scale = 4f
      };
      this.busDoor.timer = 0.0f;
      this.busDoor.interval = 70f;
      this.busDoor.endFunction = new TemporaryAnimatedSprite.endBehavior(this.busStartMovingOff);
      this.localSound("trashcanlid");
      this.drivingBack = false;
      this.busDoor.paused = false;
    }

    public void busDriveBack()
    {
      this.busPosition.X = (float) this.map.GetLayer("Back").DisplayWidth;
      this.busDoor.Position = this.busPosition + new Vector2(16f, 26f) * 4f;
      this.drivingBack = true;
      this.drivingOff = false;
      this.localSound("busDriveOff");
      this.busMotion = new Vector2(-6f, 0.0f);
      Game1.freezeControls = true;
    }

    private void busStartMovingOff(int extraInfo) => Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
    {
      Game1.globalFadeToClear();
      this.localSound("batFlap");
      this.drivingOff = true;
      this.localSound("busDriveOff");
      Game1.changeMusicTrack("none");
    }));

    private void pamReturnedToSpot(Character c, GameLocation l)
    {
    }

    private void doorOpenAfterReturn(int extraInfo)
    {
      this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
      {
        interval = 999999f,
        animationLength = 6,
        holdLastFrame = true,
        layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
        scale = 4f
      };
      Game1.player.Position = new Vector2(12f, 10f) * 64f;
      this.lastTouchActionLocation = Game1.player.getTileLocation();
      Game1.displayFarmer = true;
      Game1.player.forceCanMove();
      Game1.player.faceDirection(2);
    }

    private void busLeftToDesert()
    {
      Game1.viewportFreeze = true;
      Game1.warpFarmer("Desert", 16, 24, true);
      Game1.globalFade = false;
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if (this.forceWarpTimer > 0)
      {
        this.forceWarpTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.forceWarpTimer <= 0)
          this.playerReachedBusDoor((Character) Game1.player, (GameLocation) this);
      }
      if (this.minecartSteam != null)
        this.minecartSteam.update(time);
      if (this.drivingOff && !this.leaving)
      {
        this.busMotion.X -= 0.075f;
        if ((double) this.busPosition.X + 512.0 < 0.0)
        {
          this.leaving = true;
          this.busLeftToDesert();
        }
      }
      if (this.drivingBack && this.busMotion != Vector2.Zero)
      {
        Game1.player.Position = this.busPosition;
        if ((double) this.busPosition.X - 704.0 < 256.0)
          this.busMotion.X = Math.Min(-1f, this.busMotion.X * 0.98f);
        if ((double) Math.Abs(this.busPosition.X - 704f) <= (double) Math.Abs(this.busMotion.X * 1.5f))
        {
          this.busPosition.X = 704f;
          this.busMotion = Vector2.Zero;
          Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
          {
            this.drivingBack = false;
            this.busDoor.Position = this.busPosition + new Vector2(16f, 26f) * 4f;
            this.busDoor.pingPong = true;
            this.busDoor.interval = 70f;
            this.busDoor.currentParentTileIndex = 5;
            this.busDoor.endFunction = new TemporaryAnimatedSprite.endBehavior(this.doorOpenAfterReturn);
            this.localSound("trashcanlid");
            if (Game1.player.horseName.Value != null && Game1.player.horseName.Value != "")
            {
              for (int index = 0; index < this.characters.Count; ++index)
              {
                if (this.characters[index] is Horse && (this.characters[index] as Horse).getOwner() == Game1.player)
                {
                  if (this.characters[index].Name == null || this.characters[index].Name.Length == 0)
                  {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:BusStop_ReturnToHorse2", (object) this.characters[index].displayName));
                    break;
                  }
                  Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:BusStop_ReturnToHorse" + (Game1.random.Next(2) + 1).ToString(), (object) this.characters[index].displayName));
                  break;
                }
              }
            }
            Game1.globalFadeToClear();
          }));
        }
      }
      if (!this.busMotion.Equals(Vector2.Zero))
      {
        this.busPosition += this.busMotion;
        if (this.busDoor != null)
          this.busDoor.Position += this.busMotion;
      }
      if (this.busDoor == null)
        return;
      this.busDoor.update(time);
    }

    public override bool shouldHideCharacters() => this.drivingOff || this.drivingBack;

    public override void draw(SpriteBatch spriteBatch)
    {
      base.draw(spriteBatch);
      if (this.minecartSteam != null)
        this.minecartSteam.draw(spriteBatch);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y)), new Microsoft.Xna.Framework.Rectangle?(this.busSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0) / 10000.0));
      if (this.busDoor != null)
        this.busDoor.draw(spriteBatch);
      if (this.drivingBack && Desert.warpedToDesert)
      {
        Game1.player.faceDirection(3);
        Game1.player.blinkTimer = -1000;
        Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(117, 99999, 0, false, true), 117, new Microsoft.Xna.Framework.Rectangle(48, 608, 16, 32), Game1.GlobalToLocal(new Vector2((float) (int) ((double) this.busPosition.X + 4.0), (float) (int) ((double) this.busPosition.Y - 8.0)) + this.pamOffset * 4f), Vector2.Zero, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0), Color.White, 0.0f, 1f, Game1.player);
        spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) ((int) this.busPosition.Y - 40)) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 21, 41)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 8.0) / 10000.0));
      }
      else
      {
        if (!this.drivingOff && !this.drivingBack)
          return;
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(this.pamSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0));
      }
    }
  }
}
