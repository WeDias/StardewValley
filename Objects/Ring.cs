// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Ring
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Text;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Ring : Item
  {
    public const int ringLowerIndexRange = 516;
    public const int slimeCharmer = 520;
    public const int yobaRing = 524;
    public const int sturdyRing = 525;
    public const int burglarsRing = 526;
    public const int jukeboxRing = 528;
    public const int ringUpperIndexRange = 534;
    public const int protectiveRing = 861;
    public const int sapperRing = 862;
    public const int phoenixRing = 863;
    [XmlElement("price")]
    public readonly NetInt price = new NetInt();
    [XmlElement("indexInTileSheet")]
    public readonly NetInt indexInTileSheet = new NetInt();
    [XmlElement("uniqueID")]
    public readonly NetInt uniqueID = new NetInt();
    [XmlIgnore]
    public string description;
    [XmlIgnore]
    public string displayName;
    [XmlIgnore]
    protected int? _lightSourceID;
    [XmlIgnore]
    public bool zeroStack;

    public Ring() => this.NetFields.AddFields((INetSerializable) this.price, (INetSerializable) this.indexInTileSheet, (INetSerializable) this.uniqueID);

    public Ring(int which)
      : this()
    {
      string[] strArray = Game1.objectInformation[which].Split('/');
      this.Category = -96;
      this.Name = strArray[0];
      this.price.Value = Convert.ToInt32(strArray[1]);
      this.indexInTileSheet.Value = which;
      this.ParentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.indexInTileSheet;
      this.uniqueID.Value = Game1.year + Game1.dayOfMonth + Game1.timeOfDay + (int) (NetFieldBase<int, NetInt>) this.indexInTileSheet + Game1.player.getTileX() + (int) Game1.stats.MonstersKilled + (int) Game1.stats.itemsCrafted;
      this.loadDisplayFields();
    }

    public virtual void onDayUpdate(Farmer who, GameLocation location)
    {
      if (this.indexInTileSheet.Value != 859)
        return;
      this.onEquip(who, location);
    }

    public virtual void onEquip(Farmer who, GameLocation location)
    {
      if (this._lightSourceID.HasValue)
      {
        location.removeLightSource(this._lightSourceID.Value);
        this._lightSourceID = new int?();
      }
      switch ((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet)
      {
        case 516:
          this._lightSourceID = new int?((int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID);
          while (location.sharedLights.ContainsKey(this._lightSourceID.Value))
            this._lightSourceID = new int?(this._lightSourceID.Value + 1);
          location.sharedLights[this._lightSourceID.Value] = new LightSource(1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 5f, new Color(0, 50, 170), (int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID, playerID: who.UniqueMultiplayerID);
          break;
        case 517:
          this._lightSourceID = new int?((int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID);
          while (location.sharedLights.ContainsKey(this._lightSourceID.Value))
            this._lightSourceID = new int?(this._lightSourceID.Value + 1);
          location.sharedLights[this._lightSourceID.Value] = new LightSource(1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 10f, new Color(0, 30, 150), (int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID, playerID: who.UniqueMultiplayerID);
          break;
        case 518:
          who.magneticRadius.Value += 64;
          break;
        case 519:
          who.magneticRadius.Value += 128;
          break;
        case 527:
          this._lightSourceID = new int?((int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID);
          while (location.sharedLights.ContainsKey(this._lightSourceID.Value))
            this._lightSourceID = new int?(this._lightSourceID.Value + 1);
          location.sharedLights[this._lightSourceID.Value] = new LightSource(1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 10f, new Color(0, 80, 0), (int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID, playerID: who.UniqueMultiplayerID);
          who.magneticRadius.Value += 128;
          who.attackIncreaseModifier += 0.1f;
          break;
        case 529:
          who.knockbackModifier += 0.1f;
          break;
        case 530:
          who.weaponPrecisionModifier += 0.1f;
          break;
        case 531:
          who.critChanceModifier += 0.1f;
          break;
        case 532:
          who.critPowerModifier += 0.1f;
          break;
        case 533:
          who.weaponSpeedModifier += 0.1f;
          break;
        case 534:
          who.attackIncreaseModifier += 0.1f;
          break;
        case 810:
          who.resilience += 5;
          break;
        case 859:
          ++who.addedLuckLevel.Value;
          break;
        case 887:
          who.immunity += 4;
          break;
        case 888:
          this._lightSourceID = new int?((int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID);
          while (location.sharedLights.ContainsKey(this._lightSourceID.Value))
            this._lightSourceID = new int?(this._lightSourceID.Value + 1);
          location.sharedLights[this._lightSourceID.Value] = new LightSource(1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 10f, new Color(0, 80, 0), (int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID, playerID: who.UniqueMultiplayerID);
          who.magneticRadius.Value += 128;
          break;
      }
    }

    public virtual void onUnequip(Farmer who, GameLocation location)
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet)
      {
        case 516:
        case 517:
          if (!this._lightSourceID.HasValue)
            break;
          location.removeLightSource(this._lightSourceID.Value);
          this._lightSourceID = new int?();
          break;
        case 518:
          who.magneticRadius.Value -= 64;
          break;
        case 519:
          who.magneticRadius.Value -= 128;
          break;
        case 527:
          who.magneticRadius.Value -= 128;
          who.attackIncreaseModifier -= 0.1f;
          if (!this._lightSourceID.HasValue)
            break;
          location.removeLightSource(this._lightSourceID.Value);
          this._lightSourceID = new int?();
          break;
        case 529:
          who.knockbackModifier -= 0.1f;
          break;
        case 530:
          who.weaponPrecisionModifier -= 0.1f;
          break;
        case 531:
          who.critChanceModifier -= 0.1f;
          break;
        case 532:
          who.critPowerModifier -= 0.1f;
          break;
        case 533:
          who.weaponSpeedModifier -= 0.1f;
          break;
        case 534:
          who.attackIncreaseModifier -= 0.1f;
          break;
        case 810:
          who.resilience -= 5;
          break;
        case 859:
          --who.addedLuckLevel.Value;
          break;
        case 887:
          who.immunity -= 4;
          break;
        case 888:
          if (this._lightSourceID.HasValue)
          {
            location.removeLightSource(this._lightSourceID.Value);
            this._lightSourceID = new int?();
          }
          who.magneticRadius.Value -= 128;
          break;
      }
    }

    public override string getCategoryName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Ring.cs.1");

    public virtual void onNewLocation(Farmer who, GameLocation environment)
    {
      if (this._lightSourceID.HasValue)
      {
        environment.removeLightSource(this._lightSourceID.Value);
        this._lightSourceID = new int?();
      }
      switch ((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet)
      {
        case 516:
        case 517:
          this.onEquip(who, environment);
          break;
        case 527:
        case 888:
          this._lightSourceID = new int?((int) (NetFieldBase<int, NetInt>) this.uniqueID + (int) who.UniqueMultiplayerID);
          while (environment.sharedLights.ContainsKey(this._lightSourceID.Value))
            this._lightSourceID = new int?(this._lightSourceID.Value + 1);
          environment.sharedLights[this._lightSourceID.Value] = new LightSource(1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 10f, new Color(0, 30, 150), playerID: who.UniqueMultiplayerID);
          break;
      }
    }

    public virtual void onLeaveLocation(Farmer who, GameLocation environment)
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet)
      {
        case 516:
        case 517:
        case 527:
        case 888:
          if (!this._lightSourceID.HasValue)
            break;
          environment.removeLightSource(this._lightSourceID.Value);
          this._lightSourceID = new int?();
          break;
      }
    }

    public override int salePrice() => (int) (NetFieldBase<int, NetInt>) this.price;

    public virtual void onMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet)
      {
        case 521:
          if (Game1.random.NextDouble() >= 0.1 + (double) Game1.player.LuckLevel / 100.0)
            break;
          Game1.buffsDisplay.addOtherBuff(new Buff(20));
          Game1.playSound("warrior");
          break;
        case 522:
          Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + 2);
          break;
        case 523:
          Game1.buffsDisplay.addOtherBuff(new Buff(22));
          break;
        case 811:
          location.explode(m.getTileLocation(), 2, who, false);
          break;
        case 860:
          if (Game1.random.NextDouble() < 0.25)
          {
            m.objectsToDrop.Add(395);
            break;
          }
          if (Game1.random.NextDouble() >= 0.1)
            break;
          m.objectsToDrop.Add(253);
          break;
        case 862:
          Game1.player.Stamina = Math.Min((float) Game1.player.MaxStamina, Game1.player.Stamina + 4f);
          break;
      }
    }

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
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.indexInTileSheet, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
    }

    public virtual void update(GameTime time, GameLocation environment, Farmer who)
    {
      if (this._lightSourceID.HasValue)
      {
        Vector2 zero = Vector2.Zero;
        if (who.shouldShadowBeOffset)
          zero += (Vector2) (NetFieldBase<Vector2, NetVector2>) who.drawOffset;
        environment.repositionLightSource(this._lightSourceID.Value, new Vector2(who.Position.X + 21f, who.Position.Y) + zero);
        if (!(bool) (NetFieldBase<bool, NetBool>) environment.isOutdoors)
        {
          switch (environment)
          {
            case MineShaft _:
            case VolcanoDungeon _:
              break;
            default:
              LightSource lightSource = environment.getLightSource(this._lightSourceID.Value);
              if (lightSource != null)
              {
                lightSource.radius.Value = 3f;
                break;
              }
              break;
          }
        }
      }
      int indexInTileSheet = (int) (NetFieldBase<int, NetInt>) this.indexInTileSheet;
    }

    public override int maximumStackSize() => 1;

    public override int addToStack(Item stack) => 1;

    public override Point getExtraSpaceNeededForTooltipSpecialIcons(
      SpriteFont font,
      int minWidth,
      int horizontalBuffer,
      int startingHeight,
      StringBuilder descriptionText,
      string boldTitleText,
      int moneyAmountToDisplayAtBottom)
    {
      Point tooltipSpecialIcons = new Point(0, startingHeight);
      int num = 0;
      if (this.GetsEffectOfRing(810))
        ++num;
      if (this.GetsEffectOfRing(887))
        ++num;
      if (this.GetsEffectOfRing(859))
        ++num;
      tooltipSpecialIcons.X = (int) Math.Max((float) minWidth, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) 9999)).X + (float) horizontalBuffer);
      tooltipSpecialIcons.Y += num * Math.Max((int) font.MeasureString("TT").Y, 48);
      return tooltipSpecialIcons;
    }

    public virtual bool GetsEffectOfRing(int ring_index) => (int) (NetFieldBase<int, NetInt>) this.indexInTileSheet == ring_index;

    public virtual int GetEffectsOfRingMultiplier(int ring_index) => this.GetsEffectOfRing(ring_index) ? 1 : 0;

    public override void drawTooltip(
      SpriteBatch spriteBatch,
      ref int x,
      ref int y,
      SpriteFont font,
      float alpha,
      StringBuilder overrideText)
    {
      Utility.drawTextWithShadow(spriteBatch, Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth()), font, new Vector2((float) (x + 16), (float) (y + 16 + 4)), Game1.textColor);
      y += (int) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y;
      if (this.GetsEffectOfRing(810))
      {
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) (5 * this.GetEffectsOfRingMultiplier(810))), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), Game1.textColor * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      if (this.GetsEffectOfRing(887))
      {
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(150, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", (object) (4 * this.GetEffectsOfRingMultiplier(887))), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), Game1.textColor * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      if (!this.GetsEffectOfRing(859))
        return;
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(50, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, "+" + Game1.content.LoadString("Strings\\UI:ItemHover_Buff4", (object) this.GetEffectsOfRingMultiplier(859)), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), Game1.textColor * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }

    public override string getDescription()
    {
      if (this.description == null)
        this.loadDisplayFields();
      return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
    }

    public override bool isPlaceable() => false;

    [XmlIgnore]
    public override string DisplayName
    {
      get
      {
        if (this.displayName == null)
          this.loadDisplayFields();
        return this.displayName;
      }
      set => this.displayName = value;
    }

    [XmlIgnore]
    public override int Stack
    {
      get => this.zeroStack ? 0 : 1;
      set
      {
        if (value == 0)
          this.zeroStack = true;
        else
          this.zeroStack = false;
      }
    }

    public override Item getOne()
    {
      Ring one = new Ring((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected virtual bool loadDisplayFields()
    {
      if (Game1.objectInformation == null || !((NetFieldBase<int, NetInt>) this.indexInTileSheet != (NetInt) null))
        return false;
      string[] strArray = Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.indexInTileSheet].Split('/');
      this.displayName = strArray[4];
      this.description = strArray[5];
      return true;
    }

    public virtual bool CanCombine(Ring ring) => !(ring is CombinedRing) && !(this is CombinedRing) && this.ParentSheetIndex != ring.ParentSheetIndex;

    public Ring Combine(Ring ring)
    {
      CombinedRing combinedRing = new CombinedRing(880);
      combinedRing.combinedRings.Add(this.getOne() as Ring);
      combinedRing.combinedRings.Add(ring.getOne() as Ring);
      combinedRing.UpdateDescription();
      combinedRing.uniqueID.Value = this.uniqueID.Value;
      return (Ring) combinedRing;
    }
  }
}
