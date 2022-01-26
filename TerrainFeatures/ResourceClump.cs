// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.ResourceClump
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class ResourceClump : TerrainFeature
  {
    public const int stumpIndex = 600;
    public const int hollowLogIndex = 602;
    public const int meteoriteIndex = 622;
    public const int boulderIndex = 672;
    public const int mineRock1Index = 752;
    public const int mineRock2Index = 754;
    public const int mineRock3Index = 756;
    public const int mineRock4Index = 758;
    [XmlElement("width")]
    public readonly NetInt width = new NetInt();
    [XmlElement("height")]
    public readonly NetInt height = new NetInt();
    [XmlElement("parentSheetIndex")]
    public readonly NetInt parentSheetIndex = new NetInt();
    [XmlElement("health")]
    public readonly NetFloat health = new NetFloat();
    [XmlElement("tile")]
    public readonly NetVector2 tile = new NetVector2();
    protected float shakeTimer;

    public ResourceClump()
      : base(true)
    {
      this.NetFields.AddFields((INetSerializable) this.width, (INetSerializable) this.height, (INetSerializable) this.parentSheetIndex, (INetSerializable) this.health, (INetSerializable) this.tile);
    }

    public ResourceClump(int parentSheetIndex, int width, int height, Vector2 tile)
      : this()
    {
      this.width.Value = width;
      this.height.Value = height;
      this.parentSheetIndex.Value = parentSheetIndex;
      this.tile.Value = tile;
      switch (parentSheetIndex)
      {
        case 600:
          this.health.Value = 10f;
          break;
        case 602:
          this.health.Value = 20f;
          break;
        case 622:
          this.health.Value = 20f;
          break;
        case 672:
          this.health.Value = 10f;
          break;
        case 752:
        case 754:
        case 756:
        case 758:
          this.health.Value = 8f;
          break;
      }
    }

    public override bool isPassable(Character c = null) => false;

    public override bool performToolAction(
      Tool t,
      int damage,
      Vector2 tileLocation,
      GameLocation location)
    {
      if (t == null)
        return false;
      int debrisType = 12;
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 600:
          switch (t)
          {
            case Axe _ when (int) (NetFieldBase<int, NetInt>) t.upgradeLevel < 1:
              location.playSound("axe");
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13945"));
              Game1.player.jitterStrength = 1f;
              return false;
            case Axe _:
              location.playSound("axchop");
              break;
            default:
              return false;
          }
          break;
        case 602:
          switch (t)
          {
            case Axe _ when (int) (NetFieldBase<int, NetInt>) t.upgradeLevel < 2:
              location.playSound("axe");
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13948"));
              Game1.player.jitterStrength = 1f;
              return false;
            case Axe _:
              location.playSound("axchop");
              break;
            default:
              return false;
          }
          break;
        case 622:
          switch (t)
          {
            case Pickaxe _ when (int) (NetFieldBase<int, NetInt>) t.upgradeLevel < 3:
              location.playSound("clubhit");
              location.playSound("clank");
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13952"));
              Game1.player.jitterStrength = 1f;
              return false;
            case Pickaxe _:
              location.playSound("hammer");
              debrisType = 14;
              break;
            default:
              return false;
          }
          break;
        case 672:
          switch (t)
          {
            case Pickaxe _ when (int) (NetFieldBase<int, NetInt>) t.upgradeLevel < 2:
              location.playSound("clubhit");
              location.playSound("clank");
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13956"));
              Game1.player.jitterStrength = 1f;
              return false;
            case Pickaxe _:
              location.playSound("hammer");
              debrisType = 14;
              break;
            default:
              return false;
          }
          break;
        case 752:
        case 754:
        case 756:
        case 758:
          if (!(t is Pickaxe))
            return false;
          location.playSound("hammer");
          debrisType = 14;
          this.shakeTimer = 500f;
          this.NeedsUpdate = true;
          break;
      }
      float num1 = Math.Max(1f, (float) ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel + 1) * 0.75f);
      this.health.Value -= num1;
      if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double) num1 / 12.0 && ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 602 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 600))
      {
        Debris debris = new Debris(709, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()));
        debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
        debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
        location.debris.Add(debris);
      }
      Game1.createRadialDebris(Game1.currentLocation, debrisType, (int) tileLocation.X + Game1.random.Next((int) (NetFieldBase<int, NetInt>) this.width / 2 + 1), (int) tileLocation.Y + Game1.random.Next((int) (NetFieldBase<int, NetInt>) this.height / 2 + 1), Game1.random.Next(4, 9), false);
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
      {
        if (t != null && location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()) && Game1.random.NextDouble() < 0.05)
        {
          StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
          if (unseenSecretNote != null)
            Game1.createItemDebris((Item) unseenSecretNote, tileLocation * 64f, -1, location);
        }
        if (Game1.IsMultiplayer)
        {
          Random multiplayerRandom = Game1.recentMultiplayerRandom;
        }
        else
        {
          Random random1 = new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 7.0 + (double) tileLocation.Y * 11.0 + (double) Game1.stats.DaysPlayed + (double) (float) (NetFieldBase<float, NetFloat>) this.health));
        }
        Random random2;
        switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
        {
          case 600:
          case 602:
            if (t.getLastFarmerToUse() == Game1.player)
              ++Game1.stats.StumpsChopped;
            t.getLastFarmerToUse().gainExperience(2, 25);
            int number1 = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 602 ? 8 : 2;
            Random random3;
            if (Game1.IsMultiplayer)
            {
              Game1.recentMultiplayerRandom = new Random((int) tileLocation.X * 1000 + (int) tileLocation.Y);
              random3 = Game1.recentMultiplayerRandom;
            }
            else
              random3 = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
            if (t.getLastFarmerToUse().professions.Contains(12))
            {
              if (number1 == 8)
                number1 = 10;
              else if (random3.NextDouble() < 0.5)
                ++number1;
            }
            if (Game1.IsMultiplayer)
              Game1.createMultipleObjectDebris(709, (int) tileLocation.X, (int) tileLocation.Y, number1, t.getLastFarmerToUse().UniqueMultiplayerID);
            else
              Game1.createMultipleObjectDebris(709, (int) tileLocation.X, (int) tileLocation.Y, number1);
            location.playSound("stumpCrack");
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(23, tileLocation * 64f, Color.White, 4, animationInterval: 140f, sourceRectWidth: 128, sourceRectHeight: 128));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(385, 1522, (int) sbyte.MaxValue, 79), 2000f, 1, 1, tileLocation * 64f + new Vector2(0.0f, 49f), false, false, 1E-05f, 0.016f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
            Game1.createRadialDebris(Game1.currentLocation, 34, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(4, 9), false);
            if (random3.NextDouble() < 0.1)
              Game1.createMultipleObjectDebris(292, (int) tileLocation.X, (int) tileLocation.Y, 1);
            if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
              Game1.createObjectDebris(890, (int) tileLocation.X, (int) tileLocation.Y - 3, ((int) tileLocation.Y + 1) * 64, 0, 1f, location);
            return true;
          case 622:
            int number2 = 6;
            if (Game1.IsMultiplayer)
            {
              Game1.recentMultiplayerRandom = new Random((int) tileLocation.X * 1000 + (int) tileLocation.Y);
              random2 = Game1.recentMultiplayerRandom;
            }
            else
              random2 = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
            if (Game1.IsMultiplayer)
            {
              Game1.createMultipleObjectDebris(386, (int) tileLocation.X, (int) tileLocation.Y, number2, t.getLastFarmerToUse().UniqueMultiplayerID);
              Game1.createMultipleObjectDebris(390, (int) tileLocation.X, (int) tileLocation.Y, number2, t.getLastFarmerToUse().UniqueMultiplayerID);
              Game1.createMultipleObjectDebris(535, (int) tileLocation.X, (int) tileLocation.Y, 2, t.getLastFarmerToUse().UniqueMultiplayerID);
            }
            else
            {
              Game1.createMultipleObjectDebris(386, (int) tileLocation.X, (int) tileLocation.Y, number2);
              Game1.createMultipleObjectDebris(390, (int) tileLocation.X, (int) tileLocation.Y, number2);
              Game1.createMultipleObjectDebris(535, (int) tileLocation.X, (int) tileLocation.Y, 2);
            }
            location.playSound("boulderBreak");
            Game1.createRadialDebris(Game1.currentLocation, 32, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(6, 12), false);
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f, Color.White));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 0.0f)) * 64f, Color.White, animationInterval: 110f));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 1f)) * 64f, Color.White, flipped: true, animationInterval: 80f));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(0.0f, 1f)) * 64f, Color.White, animationInterval: 90f));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f + new Vector2(32f, 32f), Color.White, animationInterval: 70f));
            return true;
          case 672:
          case 752:
          case 754:
          case 756:
          case 758:
            int num2 = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 672 ? 15 : 10;
            if (Game1.IsMultiplayer)
            {
              Game1.recentMultiplayerRandom = new Random((int) tileLocation.X * 1000 + (int) tileLocation.Y);
              random2 = Game1.recentMultiplayerRandom;
            }
            else
              random2 = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
            if (Game1.IsMultiplayer)
              Game1.createMultipleObjectDebris(390, (int) tileLocation.X, (int) tileLocation.Y, num2, t.getLastFarmerToUse().UniqueMultiplayerID);
            else
              Game1.createRadialDebris(Game1.currentLocation, 390, (int) tileLocation.X, (int) tileLocation.Y, num2, false, item: true);
            location.playSound("boulderBreak");
            Game1.createRadialDebris(Game1.currentLocation, 32, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(6, 12), false);
            Color color = Color.White;
            switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
            {
              case 752:
                color = new Color(188, 119, 98);
                break;
              case 754:
                color = new Color(168, 120, 95);
                break;
              case 756:
              case 758:
                color = new Color(67, 189, 238);
                break;
            }
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(48, tileLocation * 64f, color, 5, animationInterval: 180f, sourceRectWidth: 128, sourceRectHeight: 128)
            {
              alphaFade = 0.01f
            });
            return true;
        }
      }
      else
      {
        this.shakeTimer = 100f;
        this.NeedsUpdate = true;
      }
      return false;
    }

    public override Rectangle getBoundingBox(Vector2 tileLocation) => new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, (int) (NetFieldBase<int, NetInt>) this.width * 64, (int) (NetFieldBase<int, NetInt>) this.height * 64);

    public bool occupiesTile(int x, int y) => (double) x >= (double) this.tile.X && (double) x - (double) this.tile.X < (double) (int) (NetFieldBase<int, NetInt>) this.width && (double) y >= (double) this.tile.Y && (double) y - (double) this.tile.Y < (double) (int) (NetFieldBase<int, NetInt>) this.height;

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
      Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 16, 16) with
      {
        Width = (int) (NetFieldBase<int, NetInt>) this.width * 16,
        Height = (int) (NetFieldBase<int, NetInt>) this.height * 16
      };
      Vector2 globalPosition = this.tile.Value * 64f;
      if ((double) this.shakeTimer > 0.0)
        globalPosition.X += (float) Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 4f;
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(standardTileSheet), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.tile.Y + 1.0) * 64.0 / 10000.0 + (double) this.tile.X / 100000.0));
    }

    public override void loadSprite()
    {
    }

    public override bool performUseAction(Vector2 tileLocation, GameLocation location)
    {
      if (!Game1.didPlayerJustRightClick(true))
      {
        Game1.haltAfterCheck = false;
        return false;
      }
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 602:
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13962")));
          return true;
        case 622:
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13964")));
          return true;
        case 672:
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13963")));
          return true;
        default:
          return false;
      }
    }

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      if ((double) this.shakeTimer > 0.0)
        this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      else
        this.NeedsUpdate = false;
      return false;
    }

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
    }

    public override bool seasonUpdate(bool onLoad) => false;
  }
}
