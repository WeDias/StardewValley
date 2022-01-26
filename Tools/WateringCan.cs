// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.WateringCan
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Tools
{
  public class WateringCan : Tool
  {
    [XmlElement("isBottomless")]
    public readonly NetBool isBottomless = new NetBool();
    [XmlIgnore]
    protected bool _emptyCanPlayed;
    public int waterCanMax = 40;
    private int waterLeft = 40;

    public int WaterLeft
    {
      get => this.waterLeft;
      set => this.waterLeft = value;
    }

    public bool IsBottomless
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isBottomless;
      set => this.isBottomless.Value = value;
    }

    public WateringCan()
      : base("Watering Can", 0, 273, 296, false)
    {
      this.UpgradeLevel = 0;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.isBottomless);
    }

    public override Item getOne()
    {
      WateringCan destination = new WateringCan();
      destination.UpgradeLevel = this.UpgradeLevel;
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14324");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14325");

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      base.drawInMenu(spriteBatch, location + (Game1.player.hasWateringCanEnchantment ? new Vector2(0.0f, -4f) : new Vector2(0.0f, -12f)), scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
      if (drawStackNumber == StackDrawType.Hide || Game1.player.hasWateringCanEnchantment)
        return;
      spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(4f, 44f), new Rectangle?(new Rectangle(297, 420, 14, 5)), Color.White * transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 0.0001f);
      spriteBatch.Draw(Game1.staminaRect, new Rectangle((int) location.X + 8, (int) location.Y + 64 - 16, (int) ((double) this.waterLeft / (double) this.waterCanMax * 48.0), 8), this.IsBottomless ? Color.BlueViolet * 1f * transparency : Color.DodgerBlue * 0.7f * transparency);
    }

    public override string getDescription() => Game1.parseText(this.description + (Game1.player.hasWateringCanEnchantment ? Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:WateringCan_enchant") : ""), Game1.smallFont, this.getDescriptionWidth());

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      power = who.toolPower;
      who.stopJittering();
      List<Vector2> vector2List = this.tilesAffected(new Vector2((float) (x / 64), (float) (y / 64)), power, who);
      if (Game1.currentLocation.CanRefillWateringCanOnTile(x / 64, y / 64))
      {
        who.jitterStrength = 0.5f;
        switch ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel)
        {
          case 0:
            this.waterCanMax = 40;
            break;
          case 1:
            this.waterCanMax = 55;
            break;
          case 2:
            this.waterCanMax = 70;
            break;
          case 3:
            this.waterCanMax = 85;
            break;
          case 4:
            this.waterCanMax = 100;
            break;
        }
        this.waterLeft = this.waterCanMax;
        location.playSound("slosh");
        DelayedAction.playSoundAfterDelay("glug", 250, location);
      }
      else if (this.waterLeft > 0 || who.hasWateringCanEnchantment)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isEfficient)
          who.Stamina -= (float) (2 * (power + 1)) - (float) who.FarmingLevel * 0.1f;
        int num = 0;
        foreach (Vector2 vector2 in vector2List)
        {
          if (location.terrainFeatures.ContainsKey(vector2))
            location.terrainFeatures[vector2].performToolAction((Tool) this, 0, vector2, location);
          if (location.objects.ContainsKey(vector2))
            location.Objects[vector2].performToolAction((Tool) this, location);
          location.performToolAction((Tool) this, (int) vector2.X, (int) vector2.Y);
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(13, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, sourceRectWidth: 64, layerDepth: ((float) (((double) vector2.Y * 64.0 + 32.0) / 10000.0 - 0.00999999977648258)))
          {
            delayBeforeAnimationStart = 200 + num * 10
          });
          ++num;
        }
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isBottomless)
          this.waterLeft -= power + 1;
        Vector2 vector2_1 = new Vector2((float) ((double) who.Position.X - 32.0 - 4.0), (float) ((double) who.Position.Y - 16.0 - 4.0));
        switch (who.FacingDirection)
        {
          case 0:
            vector2_1 = Vector2.Zero;
            break;
          case 1:
            vector2_1.X += 136f;
            break;
          case 2:
            vector2_1.X += 72f;
            vector2_1.Y += 44f;
            break;
        }
        if (vector2_1.Equals(Vector2.Zero))
          return;
        for (int index = 0; index < 30; ++index)
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("", new Rectangle(0, 0, 1, 1), 999f, 1, 999, vector2_1 + new Vector2((float) (Game1.random.Next(-3, 0) * 4), (float) (Game1.random.Next(2) * 4)), false, false, (float) (who.GetBoundingBox().Bottom + 32) / 10000f, 0.04f, Game1.random.NextDouble() < 0.5 ? Color.DeepSkyBlue : Color.LightBlue, 4f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = index * 15,
            motion = new Vector2((float) Game1.random.Next(-10, 11) / 100f, 0.5f),
            acceleration = new Vector2(0.0f, 0.1f)
          });
      }
      else
      {
        if (this._emptyCanPlayed)
          return;
        this._emptyCanPlayed = true;
        who.doEmote(4);
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335"));
      }
    }

    public override bool CanUseOnStandingTile() => true;

    public override void tickUpdate(GameTime time, Farmer who)
    {
      base.tickUpdate(time, who);
      if (who.IsLocalPlayer)
      {
        if (!Game1.areAllOfTheseKeysUp(Game1.input.GetKeyboardState(), Game1.options.useToolButton) || Game1.input.GetMouseState().LeftButton != ButtonState.Released || !Game1.input.GetGamePadState().IsButtonUp(Buttons.X))
          return;
        this._emptyCanPlayed = false;
      }
      else
        this._emptyCanPlayed = false;
    }
  }
}
