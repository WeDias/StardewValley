// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Bush
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class Bush : LargeTerrainFeature
  {
    public const float shakeRate = 0.01570796f;
    public const float shakeDecayRate = 0.003067962f;
    public const int smallBush = 0;
    public const int mediumBush = 1;
    public const int largeBush = 2;
    public const int greenTeaBush = 3;
    public const int walnutBush = 4;
    public const int daysToMatureGreenTeaBush = 20;
    [XmlElement("size")]
    public readonly NetInt size = new NetInt();
    [XmlElement("datePlanted")]
    public readonly NetInt datePlanted = new NetInt();
    [XmlElement("tileSheetOffset")]
    public readonly NetInt tileSheetOffset = new NetInt();
    [XmlElement("overrideSeason")]
    public readonly NetInt overrideSeason = new NetInt(-1);
    public float health;
    [XmlElement("flipped")]
    public readonly NetBool flipped = new NetBool();
    [XmlElement("townBush")]
    public readonly NetBool townBush = new NetBool();
    [XmlElement("greenhouseBush")]
    public readonly NetBool greenhouseBush = new NetBool();
    [XmlElement("drawShadow")]
    public readonly NetBool drawShadow = new NetBool(true);
    private bool shakeLeft;
    private float shakeRotation;
    private float maxShake;
    private float alpha = 1f;
    private long lastPlayerToHit;
    private float shakeTimer;
    [XmlElement("sourceRect")]
    private readonly NetRectangle sourceRect = new NetRectangle();
    [XmlIgnore]
    public NetMutex uniqueSpawnMutex = new NetMutex();
    public static Lazy<Texture2D> texture = new Lazy<Texture2D>((Func<Texture2D>) (() => Game1.content.Load<Texture2D>("TileSheets\\bushes")));
    public static Rectangle treeTopSourceRect = new Rectangle(0, 0, 48, 96);
    public static Rectangle stumpSourceRect = new Rectangle(32, 96, 16, 32);
    public static Rectangle shadowSourceRect = new Rectangle(663, 1011, 41, 30);
    private float yDrawOffset;

    public Bush()
      : base(true)
    {
      this.NetFields.AddFields((INetSerializable) this.size, (INetSerializable) this.tileSheetOffset, (INetSerializable) this.flipped, (INetSerializable) this.townBush, (INetSerializable) this.drawShadow, (INetSerializable) this.sourceRect, (INetSerializable) this.datePlanted, (INetSerializable) this.greenhouseBush, (INetSerializable) this.overrideSeason, (INetSerializable) this.uniqueSpawnMutex.NetFields);
    }

    public Bush(Vector2 tileLocation, int size, GameLocation location, int datePlantedOverride = -1)
      : this()
    {
      this.tilePosition.Value = tileLocation;
      this.size.Value = size;
      if (location is Town && (double) tileLocation.X % 5.0 != 0.0)
        this.townBush.Value = true;
      if (location.map.GetLayer("Front").Tiles[(int) tileLocation.X, (int) tileLocation.Y] != null)
        this.drawShadow.Value = false;
      this.datePlanted.Value = datePlantedOverride == -1 ? (int) Game1.stats.DaysPlayed : datePlantedOverride;
      if (size == 3)
        this.drawShadow.Value = false;
      if (location.IsGreenhouse)
        this.greenhouseBush.Value = true;
      if (size == 4)
      {
        this.tileSheetOffset.Value = 1;
        this.overrideSeason.Value = 1;
      }
      GameLocation currentLocation = this.currentLocation;
      this.currentLocation = location;
      this.loadSprite();
      this.currentLocation = currentLocation;
      this.flipped.Value = Game1.random.NextDouble() < 0.5;
    }

    public int getAge() => (int) Game1.stats.DaysPlayed - this.datePlanted.Value;

    public void setUpSourceRect()
    {
      int num = (int) (NetFieldBase<int, NetInt>) this.overrideSeason == -1 ? Utility.getSeasonNumber(Game1.GetSeasonForLocation(this.currentLocation)) : (int) (NetFieldBase<int, NetInt>) this.overrideSeason;
      if (this.greenhouseBush.Value)
        num = 0;
      if ((int) (NetFieldBase<int, NetInt>) this.size == 0)
        this.sourceRect.Value = new Rectangle(num * 16 * 2 + (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset * 16, 224, 16, 32);
      else if ((int) (NetFieldBase<int, NetInt>) this.size == 1)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.townBush)
          this.sourceRect.Value = new Rectangle(num * 16 * 2, 96, 32, 32);
        else
          this.sourceRect.Value = new Rectangle((num * 16 * 4 + (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset * 16 * 2) % Bush.texture.Value.Bounds.Width, (num * 16 * 4 + (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset * 16 * 2) / Bush.texture.Value.Bounds.Width * 3 * 16, 32, 48);
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.size == 2)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.townBush && (num == 0 || num == 1))
        {
          this.sourceRect.Value = new Rectangle(48, 176, 48, 48);
        }
        else
        {
          switch (num)
          {
            case 0:
            case 1:
              this.sourceRect.Value = new Rectangle(0, 128, 48, 48);
              break;
            case 2:
              this.sourceRect.Value = new Rectangle(48, 128, 48, 48);
              break;
            case 3:
              this.sourceRect.Value = new Rectangle(0, 176, 48, 48);
              break;
          }
        }
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
      {
        int age = this.getAge();
        switch (num)
        {
          case 0:
            this.sourceRect.Value = new Rectangle(Math.Min(2, age / 10) * 16 + (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset * 16, 256, 16, 32);
            break;
          case 1:
            this.sourceRect.Value = new Rectangle(64 + Math.Min(2, age / 10) * 16 + (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset * 16, 256, 16, 32);
            break;
          case 2:
            this.sourceRect.Value = new Rectangle(Math.Min(2, age / 10) * 16 + (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset * 16, 288, 16, 32);
            break;
          case 3:
            this.sourceRect.Value = new Rectangle(64 + Math.Min(2, age / 10) * 16 + (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset * 16, 288, 16, 32);
            break;
        }
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.size != 4)
          return;
        this.sourceRect.Value = new Rectangle(this.tileSheetOffset.Value * 32, 320, 32, 32);
      }
    }

    public bool inBloom(string season, int dayOfMonth)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.size == 4)
        return this.tileSheetOffset.Value == 1;
      if ((int) (NetFieldBase<int, NetInt>) this.overrideSeason != -1)
        season = Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.overrideSeason);
      if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
        return this.getAge() >= 20 && dayOfMonth >= 22 && (!season.Equals("winter") || (bool) (NetFieldBase<bool, NetBool>) this.greenhouseBush);
      if (season.Equals("spring"))
      {
        if (dayOfMonth > 14 && dayOfMonth < 19)
          return true;
      }
      else if (season.Equals("fall") && dayOfMonth > 7 && dayOfMonth < 12)
        return true;
      return false;
    }

    public override bool isActionable() => true;

    public override void loadSprite()
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame + (int) this.tilePosition.X + (int) this.tilePosition.Y * 777);
      if ((int) (NetFieldBase<int, NetInt>) this.size != 4)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.size == 1 && (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset == 0 && random.NextDouble() < 0.5 && this.inBloom(Game1.GetSeasonForLocation(this.currentLocation), Game1.dayOfMonth))
          this.tileSheetOffset.Value = 1;
        else if (!Game1.GetSeasonForLocation(this.currentLocation).Equals("summer") && !this.inBloom(Game1.GetSeasonForLocation(this.currentLocation), Game1.dayOfMonth))
          this.tileSheetOffset.Value = 0;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
        this.tileSheetOffset.Value = this.inBloom(Game1.GetSeasonForLocation(this.currentLocation), Game1.dayOfMonth) ? 1 : 0;
      this.setUpSourceRect();
    }

    public override Rectangle getBoundingBox(Vector2 tileLocation)
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.size)
      {
        case 0:
        case 3:
          return new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);
        case 1:
        case 4:
          return new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 128, 64);
        case 2:
          return new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 192, 64);
        default:
          return Rectangle.Empty;
      }
    }

    public override Rectangle getRenderBounds(Vector2 tileLocation)
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.size)
      {
        case 0:
        case 3:
          return new Rectangle((int) tileLocation.X * 64, (int) ((double) tileLocation.Y - 1.0) * 64, 64, 160);
        case 1:
        case 4:
          return new Rectangle((int) tileLocation.X * 64, (int) ((double) tileLocation.Y - 2.0) * 64, 128, 256);
        case 2:
          return new Rectangle((int) tileLocation.X * 64, (int) ((double) tileLocation.Y - 2.0) * 64, 192, 256);
        default:
          return Rectangle.Empty;
      }
    }

    public override bool performUseAction(Vector2 tileLocation, GameLocation location)
    {
      string str = (int) (NetFieldBase<int, NetInt>) this.overrideSeason == -1 ? Game1.GetSeasonForLocation(location) : Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.overrideSeason);
      if (Game1.didPlayerJustRightClick(true))
        this.shakeTimer = 0.0f;
      if ((double) this.shakeTimer <= 0.0)
      {
        if ((double) this.maxShake == 0.0 && ((bool) (NetFieldBase<bool, NetBool>) this.greenhouseBush || (int) (NetFieldBase<int, NetInt>) this.size != 3 || !str.Equals("winter")))
          location.localSound("leafrustle");
        GameLocation currentLocation = this.currentLocation;
        this.currentLocation = location;
        this.shake(tileLocation, false);
        this.currentLocation = currentLocation;
        this.shakeTimer = 500f;
      }
      return true;
    }

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      if ((double) this.shakeTimer > 0.0)
        this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((int) (NetFieldBase<int, NetInt>) this.size == 4)
        this.uniqueSpawnMutex.Update(location);
      this.alpha = Math.Min(1f, this.alpha + 0.05f);
      if ((double) this.maxShake > 0.0)
      {
        if (this.shakeLeft)
        {
          this.shakeRotation -= (float) Math.PI / 200f;
          if ((double) this.shakeRotation <= -(double) this.maxShake)
            this.shakeLeft = false;
        }
        else
        {
          this.shakeRotation += (float) Math.PI / 200f;
          if ((double) this.shakeRotation >= (double) this.maxShake)
            this.shakeLeft = true;
        }
      }
      if ((double) this.maxShake > 0.0)
        this.maxShake = Math.Max(0.0f, this.maxShake - 0.003067962f);
      return false;
    }

    private void shake(Vector2 tileLocation, bool doEvenIfStillShaking)
    {
      if (!((double) this.maxShake == 0.0 | doEvenIfStillShaking))
        return;
      this.shakeLeft = (double) Game1.player.getTileLocation().X > (double) tileLocation.X || (double) Game1.player.getTileLocation().X == (double) tileLocation.X && Game1.random.NextDouble() < 0.5;
      this.maxShake = (float) Math.PI / 128f;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.townBush && (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset == 1 && this.inBloom(Game1.GetSeasonForLocation(this.currentLocation), Game1.dayOfMonth))
      {
        string str = (int) (NetFieldBase<int, NetInt>) this.overrideSeason == -1 ? Game1.GetSeasonForLocation(this.currentLocation) : Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.overrideSeason);
        int shakeOff = -1;
        if (!(str == "spring"))
        {
          if (str == "fall")
            shakeOff = 410;
        }
        else
          shakeOff = 296;
        if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
          shakeOff = 815;
        if ((int) (NetFieldBase<int, NetInt>) this.size == 4)
          shakeOff = 73;
        if (shakeOff == -1)
          return;
        this.tileSheetOffset.Value = 0;
        this.setUpSourceRect();
        Random random = new Random((int) tileLocation.X + (int) tileLocation.Y * 5000 + (int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
        if ((int) (NetFieldBase<int, NetInt>) this.size == 3 || (int) (NetFieldBase<int, NetInt>) this.size == 4)
        {
          int num = 1;
          for (int index = 0; index < num; ++index)
          {
            if ((int) (NetFieldBase<int, NetInt>) this.size == 4)
              this.uniqueSpawnMutex.RequestLock((Action) (() =>
              {
                Game1.player.team.MarkCollectedNut("Bush_" + this.currentLocation.Name + "_" + tileLocation.X.ToString() + "_" + tileLocation.Y.ToString());
                StardewValley.Object @object = new StardewValley.Object(shakeOff, 1);
                Rectangle boundingBox = this.getBoundingBox();
                double x = (double) boundingBox.Center.X;
                boundingBox = this.getBoundingBox();
                double y = (double) (boundingBox.Bottom - 2);
                Vector2 origin = new Vector2((float) x, (float) y);
                GameLocation currentLocation = this.currentLocation;
                boundingBox = this.getBoundingBox();
                int bottom = boundingBox.Bottom;
                Game1.createItemDebris((Item) @object, origin, 0, currentLocation, bottom);
              }));
            else
              Game1.createObjectDebris(shakeOff, (int) tileLocation.X, (int) tileLocation.Y);
          }
        }
        else
        {
          int num = random.Next(1, 2) + Game1.player.ForagingLevel / 4;
          for (int index = 0; index < num; ++index)
            Game1.createItemDebris((Item) new StardewValley.Object(shakeOff, 1, quality: (Game1.player.professions.Contains(16) ? 4 : 0)), Utility.PointToVector2(this.getBoundingBox().Center), Game1.random.Next(1, 4));
        }
        if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
          return;
        DelayedAction.playSoundAfterDelay("leafrustle", 100);
      }
      else if ((double) tileLocation.X == 20.0 && (double) tileLocation.Y == 8.0 && Game1.dayOfMonth == 28 && Game1.timeOfDay == 1200 && !Game1.player.mailReceived.Contains("junimoPlush"))
        Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new Furniture(1733, Vector2.Zero), new ItemGrabMenu.behaviorOnItemSelect(this.junimoPlushCallback));
      else if ((double) tileLocation.X == 28.0 && (double) tileLocation.Y == 14.0 && Game1.player.eventsSeen.Contains(520702) && !Game1.player.hasMagnifyingGlass && Game1.currentLocation is Town)
      {
        (Game1.currentLocation as Town).initiateMagnifyingGlassGet();
      }
      else
      {
        if ((double) tileLocation.X != 47.0 || (double) tileLocation.Y != 100.0 || !Game1.player.secretNotesSeen.Contains(21) || Game1.timeOfDay != 2440 || !(Game1.currentLocation is Town) || Game1.player.mailReceived.Contains("secretNote21_done"))
          return;
        Game1.player.mailReceived.Add("secretNote21_done");
        (Game1.currentLocation as Town).initiateMarnieLewisBush();
      }
    }

    public void junimoPlushCallback(Item item, Farmer who)
    {
      if (item == null || !(item is Furniture) || (int) (NetFieldBase<int, NetInt>) (item as Furniture).parentSheetIndex != 1733 || who == null)
        return;
      who.mailReceived.Add("junimoPlush");
    }

    public override bool isPassable(Character c = null) => c is JunimoHarvester;

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
      string season = (int) (NetFieldBase<int, NetInt>) this.overrideSeason == -1 ? Game1.GetSeasonForLocation(environment) : Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.overrideSeason);
      if ((int) (NetFieldBase<int, NetInt>) this.size == 4)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.size == 1 && (int) (NetFieldBase<int, NetInt>) this.tileSheetOffset == 0 && Game1.random.NextDouble() < 0.2 && this.inBloom(season, Game1.dayOfMonth))
        this.tileSheetOffset.Value = 1;
      else if (!season.Equals("summer") && !this.inBloom(season, Game1.dayOfMonth))
        this.tileSheetOffset.Value = 0;
      if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
        this.tileSheetOffset.Value = this.inBloom(season, Game1.dayOfMonth) ? 1 : 0;
      GameLocation currentLocation = this.currentLocation;
      this.currentLocation = environment;
      this.setUpSourceRect();
      this.currentLocation = currentLocation;
      if ((double) tileLocation.X == 6.0 && (double) tileLocation.Y == 7.0 && environment.Name == "Sunroom")
        return;
      this.health = 0.0f;
    }

    public override bool seasonUpdate(bool onLoad)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.size == 4 || Game1.IsMultiplayer && !Game1.IsServer)
        return false;
      string str = (int) (NetFieldBase<int, NetInt>) this.overrideSeason == -1 ? Game1.GetSeasonForLocation(this.currentLocation) : Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.overrideSeason);
      if ((int) (NetFieldBase<int, NetInt>) this.size == 1 && str.Equals("summer") && Game1.random.NextDouble() < 0.5)
        this.tileSheetOffset.Value = 1;
      else
        this.tileSheetOffset.Value = 0;
      this.loadSprite();
      return false;
    }

    public override bool performToolAction(
      Tool t,
      int explosion,
      Vector2 tileLocation,
      GameLocation location)
    {
      if (location == null)
        location = Game1.currentLocation;
      if ((int) (NetFieldBase<int, NetInt>) this.size == 4)
        return false;
      if (explosion > 0)
      {
        GameLocation currentLocation = this.currentLocation;
        this.currentLocation = location;
        this.shake(tileLocation, true);
        this.currentLocation = currentLocation;
        return false;
      }
      if (t != null && t is Axe && this.isDestroyable(location, tileLocation))
      {
        location.playSound("leafrustle");
        GameLocation currentLocation = this.currentLocation;
        this.currentLocation = location;
        this.shake(tileLocation, true);
        this.currentLocation = currentLocation;
        if ((int) (NetFieldBase<int, NetInt>) (t as Axe).upgradeLevel >= 1 || (int) (NetFieldBase<int, NetInt>) this.size == 3)
        {
          this.health -= (int) (NetFieldBase<int, NetInt>) this.size == 3 ? 0.5f : (float) (int) (NetFieldBase<int, NetInt>) (t as Axe).upgradeLevel / 5f;
          if ((double) this.health <= -1.0)
          {
            location.playSound("treethud");
            DelayedAction.playSoundAfterDelay("leafrustle", 100);
            Color color = Color.Green;
            string str = (int) (NetFieldBase<int, NetInt>) this.overrideSeason == -1 ? Game1.GetSeasonForLocation(location) : Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.overrideSeason);
            if (!(str == "spring"))
            {
              if (!(str == "summer"))
              {
                if (!(str == "fall"))
                {
                  if (str == "winter")
                    color = Color.Cyan;
                }
                else
                  color = Color.IndianRed;
              }
              else
                color = Color.ForestGreen;
            }
            else
              color = Color.Green;
            if ((bool) (NetFieldBase<bool, NetBool>) this.greenhouseBush)
            {
              color = Color.Green;
              if (location != null && location.Name.Equals("Sunroom"))
              {
                foreach (NPC character in location.characters)
                {
                  character.jump();
                  character.doEmote(12);
                }
              }
            }
            for (int index1 = 0; index1 <= this.getEffectiveSize(); ++index1)
            {
              for (int index2 = 0; index2 < 12; ++index2)
              {
                Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(355, 1200 + (str.Equals("fall") ? 16 : (str.Equals("winter") ? -16 : 0)), 16, 16), Utility.getRandomPositionInThisRectangle(this.getBoundingBox(), Game1.random) - new Vector2(0.0f, (float) Game1.random.Next(64)), false, 0.01f, color)
                {
                  motion = new Vector2((float) Game1.random.Next(-10, 11) / 10f, (float) -Game1.random.Next(5, 7)),
                  acceleration = new Vector2(0.0f, (float) Game1.random.Next(13, 17) / 100f),
                  accelerationChange = new Vector2(0.0f, -1f / 1000f),
                  scale = 4f,
                  layerDepth = (float) (((double) tileLocation.Y + 1.0) * 64.0 / 10000.0),
                  animationLength = 11,
                  totalNumberOfLoops = 99,
                  interval = (float) Game1.random.Next(20, 90),
                  delayBeforeAnimationStart = (index1 + 1) * index2 * 20
                });
                if (index2 % 6 == 0)
                {
                  Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, Utility.getRandomPositionInThisRectangle(this.getBoundingBox(), Game1.random) - new Vector2(32f, (float) Game1.random.Next(32, 64)), color));
                  Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, Utility.getRandomPositionInThisRectangle(this.getBoundingBox(), Game1.random) - new Vector2(32f, (float) Game1.random.Next(32, 64)), Color.White));
                }
              }
            }
            return true;
          }
          location.playSound("axchop");
        }
      }
      return false;
    }

    public bool isDestroyable(GameLocation location, Vector2 tile)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
        return true;
      if (location != null && location is Farm)
      {
        switch (Game1.whichFarm)
        {
          case 1:
            return new Rectangle(32, 11, 11, 25).Contains((int) tile.X, (int) tile.Y);
          case 2:
            return (double) tile.X == 13.0 && (double) tile.Y == 35.0 || (double) tile.X == 37.0 && (double) tile.Y == 9.0 || new Rectangle(43, 11, 34, 50).Contains((int) tile.X, (int) tile.Y);
          case 3:
            return new Rectangle(24, 56, 10, 8).Contains((int) tile.X, (int) tile.Y);
          case 6:
            return new Rectangle(20, 44, 36, 44).Contains((int) tile.X, (int) tile.Y);
        }
      }
      return false;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 positionOnScreen,
      Vector2 tileLocation,
      float scale,
      float layerDepth)
    {
      layerDepth += positionOnScreen.X / 100000f;
      spriteBatch.Draw(Bush.texture.Value, positionOnScreen + new Vector2(0.0f, -64f * scale), new Rectangle?(new Rectangle(32, 96, 16, 32)), Color.White, 0.0f, Vector2.Zero, scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale - 1.0) / 20000.0));
    }

    public override void performPlayerEntryAction(Vector2 tileLocation)
    {
      base.performPlayerEntryAction(tileLocation);
      string str = (int) (NetFieldBase<int, NetInt>) this.overrideSeason == -1 ? Game1.GetSeasonForLocation(this.currentLocation) : Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.overrideSeason);
      if (str.Equals("winter") || Game1.IsRainingHere(this.currentLocation) || !Game1.isDarkOut() || Game1.random.NextDouble() >= (str.Equals("summer") ? 0.08 : 0.04))
        return;
      AmbientLocationSounds.addSound(tileLocation, 3);
      Game1.debugOutput = Game1.debugOutput + "  added cricket at " + tileLocation.ToString();
    }

    private int getEffectiveSize()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.size == 3)
        return 0;
      return (int) (NetFieldBase<int, NetInt>) this.size == 4 ? 1 : (int) (NetFieldBase<int, NetInt>) this.size;
    }

    public void draw(SpriteBatch spriteBatch, Vector2 tileLocation, float yDrawOffset)
    {
      this.yDrawOffset = yDrawOffset;
      this.draw(spriteBatch, tileLocation);
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
      Rectangle rectangle;
      if ((bool) (NetFieldBase<bool, NetBool>) this.drawShadow)
      {
        if (this.getEffectiveSize() > 0)
        {
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((double) tileLocation.X + (this.getEffectiveSize() == 1 ? 0.5 : 1.0)) * 64.0 - 51.0), (float) ((double) tileLocation.Y * 64.0 - 16.0) + this.yDrawOffset)), new Rectangle?(Bush.shadowSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
        }
        else
        {
          SpriteBatch spriteBatch1 = spriteBatch;
          Texture2D shadowTexture = Game1.shadowTexture;
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 64.0 - 4.0) + this.yDrawOffset));
          Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
          Color color = Color.White * this.alpha;
          double x = (double) Game1.shadowTexture.Bounds.Center.X;
          rectangle = Game1.shadowTexture.Bounds;
          double y = (double) rectangle.Center.Y;
          Vector2 origin = new Vector2((float) x, (float) y);
          spriteBatch1.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, 1E-06f);
        }
      }
      SpriteBatch spriteBatch2 = spriteBatch;
      Texture2D texture = Bush.texture.Value;
      Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + (float) ((this.getEffectiveSize() + 1) * 64 / 2), (float) (((double) tileLocation.Y + 1.0) * 64.0 - (this.getEffectiveSize() <= 0 || (bool) (NetFieldBase<bool, NetBool>) this.townBush && this.getEffectiveSize() == 1 || (int) (NetFieldBase<int, NetInt>) this.size == 4 ? 0.0 : 64.0)) + this.yDrawOffset));
      Rectangle? sourceRectangle1 = new Rectangle?((Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.sourceRect);
      Color color1 = Color.White * this.alpha;
      double shakeRotation = (double) this.shakeRotation;
      Vector2 origin1 = new Vector2((float) ((this.getEffectiveSize() + 1) * 16 / 2), 32f);
      int effects = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
      rectangle = this.getBoundingBox(tileLocation);
      double layerDepth = (double) (rectangle.Center.Y + 48) / 10000.0 - (double) tileLocation.X / 1000000.0;
      spriteBatch2.Draw(texture, local1, sourceRectangle1, color1, (float) shakeRotation, origin1, 4f, (SpriteEffects) effects, (float) layerDepth);
    }
  }
}
