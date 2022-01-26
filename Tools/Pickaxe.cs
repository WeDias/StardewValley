// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Pickaxe
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;

namespace StardewValley.Tools
{
  public class Pickaxe : Tool
  {
    public const int hitMargin = 8;
    public const int BoulderStrength = 4;
    private int boulderTileX;
    private int boulderTileY;
    private int hitsToBoulder;
    public NetInt additionalPower = new NetInt(0);

    public Pickaxe()
      : base(nameof (Pickaxe), 0, 105, 131, false)
    {
      this.UpgradeLevel = 0;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.additionalPower);
    }

    public override Item getOne()
    {
      Pickaxe destination = new Pickaxe();
      destination.UpgradeLevel = this.UpgradeLevel;
      destination.additionalPower.Value = this.additionalPower.Value;
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14184");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14185");

    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      this.Update(who.FacingDirection, 0, who);
      who.EndUsingTool();
      return true;
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      power = who.toolPower;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isEfficient)
        who.Stamina -= (float) (2 * (power + 1)) - (float) who.MiningLevel * 0.1f;
      Utility.clampToTile(new Vector2((float) x, (float) y));
      int num1 = x / 64;
      int num2 = y / 64;
      Vector2 vector2 = new Vector2((float) num1, (float) num2);
      if (location.performToolAction((Tool) this, num1, num2))
        return;
      StardewValley.Object @object = (StardewValley.Object) null;
      location.Objects.TryGetValue(vector2, out @object);
      if (@object == null)
      {
        if (who.FacingDirection == 0 || who.FacingDirection == 2)
        {
          num1 = (x - 8) / 64;
          location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
          if (@object == null)
          {
            num1 = (x + 8) / 64;
            location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
          }
        }
        else
        {
          num2 = (y + 8) / 64;
          location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
          if (@object == null)
          {
            num2 = (y - 8) / 64;
            location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
          }
        }
        x = num1 * 64;
        y = num2 * 64;
        if (location.terrainFeatures.ContainsKey(vector2) && location.terrainFeatures[vector2].performToolAction((Tool) this, 0, vector2, location))
          location.terrainFeatures.Remove(vector2);
      }
      vector2 = new Vector2((float) num1, (float) num2);
      if (@object != null)
      {
        if (@object.Name.Equals("Stone"))
        {
          location.playSound("hammer");
          if ((int) (NetFieldBase<int, NetIntDelta>) @object.minutesUntilReady > 0)
          {
            int num3 = Math.Max(1, (int) (NetFieldBase<int, NetInt>) this.upgradeLevel + 1) + this.additionalPower.Value;
            @object.minutesUntilReady.Value -= num3;
            @object.shakeTimer = 200;
            if ((int) (NetFieldBase<int, NetIntDelta>) @object.minutesUntilReady > 0)
            {
              Game1.createRadialDebris(Game1.currentLocation, 14, num1, num2, Game1.random.Next(2, 5), false);
              return;
            }
          }
          if (@object.ParentSheetIndex < 200 && !Game1.objectInformation.ContainsKey(@object.ParentSheetIndex + 1) && (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex != 25)
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(@object.ParentSheetIndex + 1, 300f, 1, 2, new Vector2((float) (x - x % 64), (float) (y - y % 64)), true, (bool) (NetFieldBase<bool, NetBool>) @object.flipped)
            {
              alphaFade = 0.01f
            });
          else
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(47, new Vector2((float) (num1 * 64), (float) (num2 * 64)), Color.Gray, 10, animationInterval: 80f));
          Game1.createRadialDebris(location, 14, num1, num2, Game1.random.Next(2, 5), false);
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(46, new Vector2((float) (num1 * 64), (float) (num2 * 64)), Color.White, 10, animationInterval: 80f)
          {
            motion = new Vector2(0.0f, -0.6f),
            acceleration = new Vector2(0.0f, 1f / 500f),
            alphaFade = 0.015f
          });
          location.OnStoneDestroyed((int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, num1, num2, this.getLastFarmerToUse());
          if ((int) (NetFieldBase<int, NetIntDelta>) @object.minutesUntilReady > 0)
            return;
          @object.performRemoveAction(new Vector2((float) num1, (float) num2), location);
          location.Objects.Remove(new Vector2((float) num1, (float) num2));
          location.playSound("stoneCrack");
          ++Game1.stats.RocksCrushed;
        }
        else if (@object.Name.Contains("Boulder"))
        {
          location.playSound("hammer");
          if (this.UpgradeLevel < 2)
          {
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14194")));
          }
          else
          {
            if (num1 == this.boulderTileX && num2 == this.boulderTileY)
            {
              this.hitsToBoulder += power + 1;
              @object.shakeTimer = 190;
            }
            else
            {
              this.hitsToBoulder = 0;
              this.boulderTileX = num1;
              this.boulderTileY = num2;
            }
            if (this.hitsToBoulder < 4)
              return;
            location.removeObject(vector2, false);
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2((float) (64.0 * (double) vector2.X - 32.0), (float) (64.0 * ((double) vector2.Y - 1.0))), Color.Gray, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f)
            {
              delayBeforeAnimationStart = 0
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2((float) (64.0 * (double) vector2.X + 32.0), (float) (64.0 * ((double) vector2.Y - 1.0))), Color.Gray, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f)
            {
              delayBeforeAnimationStart = 200
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector2.X, (float) (64.0 * ((double) vector2.Y - 1.0) - 32.0)), Color.Gray, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f)
            {
              delayBeforeAnimationStart = 400
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector2.X, (float) (64.0 * (double) vector2.Y - 32.0)), Color.Gray, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f)
            {
              delayBeforeAnimationStart = 600
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2(64f * vector2.X, 64f * vector2.Y), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f, sourceRectHeight: 128));
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2((float) (64.0 * (double) vector2.X + 32.0), 64f * vector2.Y), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f, sourceRectHeight: 128)
            {
              delayBeforeAnimationStart = 250
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2((float) (64.0 * (double) vector2.X - 32.0), 64f * vector2.Y), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f, sourceRectHeight: 128)
            {
              delayBeforeAnimationStart = 500
            });
            location.playSound("boulderBreak");
            ++Game1.stats.BouldersCracked;
          }
        }
        else
        {
          if (!@object.performToolAction((Tool) this, location))
            return;
          @object.performRemoveAction(vector2, location);
          if (@object.type.Equals((object) "Crafting") && (int) (NetFieldBase<int, NetInt>) @object.fragility != 2)
          {
            NetCollection<Debris> debris1 = Game1.currentLocation.debris;
            int objectIndex = (bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable ? -@object.ParentSheetIndex : @object.ParentSheetIndex;
            Vector2 toolLocation = who.GetToolLocation();
            Rectangle boundingBox = who.GetBoundingBox();
            double x1 = (double) boundingBox.Center.X;
            boundingBox = who.GetBoundingBox();
            double y1 = (double) boundingBox.Center.Y;
            Vector2 playerPosition = new Vector2((float) x1, (float) y1);
            Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
            debris1.Add(debris2);
          }
          Game1.currentLocation.Objects.Remove(vector2);
        }
      }
      else
      {
        location.playSound("woodyHit");
        if (location.doesTileHaveProperty(num1, num2, "Diggable", "Back") == null)
          return;
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2((float) (num1 * 64), (float) (num2 * 64)), Color.White, animationInterval: 80f)
        {
          alphaFade = 0.015f
        });
      }
    }
  }
}
