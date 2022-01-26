// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.MeleeWeapon
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace StardewValley.Tools
{
  public class MeleeWeapon : Tool
  {
    public const int defenseCooldownTime = 1500;
    public const int attackSwordCooldownTime = 2000;
    public const int daggerCooldownTime = 3000;
    public const int clubCooldownTime = 6000;
    public const int millisecondsPerSpeedPoint = 40;
    public const int defaultSpeed = 400;
    public const int stabbingSword = 0;
    public const int dagger = 1;
    public const int club = 2;
    public const int defenseSword = 3;
    public const int baseClubSpeed = -8;
    public const int scythe = 47;
    public const int goldenScythe = 53;
    public const int MAX_FORGES = 3;
    [XmlElement("type")]
    public readonly NetInt type = new NetInt();
    [XmlElement("minDamage")]
    public readonly NetInt minDamage = new NetInt();
    [XmlElement("maxDamage")]
    public readonly NetInt maxDamage = new NetInt();
    [XmlElement("speed")]
    public readonly NetInt speed = new NetInt();
    [XmlElement("addedPrecision")]
    public readonly NetInt addedPrecision = new NetInt();
    [XmlElement("addedDefense")]
    public readonly NetInt addedDefense = new NetInt();
    [XmlElement("addedAreaOfEffect")]
    public readonly NetInt addedAreaOfEffect = new NetInt();
    [XmlElement("knockback")]
    public readonly NetFloat knockback = new NetFloat();
    [XmlElement("critChance")]
    public readonly NetFloat critChance = new NetFloat();
    [XmlElement("critMultiplier")]
    public readonly NetFloat critMultiplier = new NetFloat();
    [XmlElement("appearance")]
    public readonly NetInt appearance = new NetInt(-1);
    public bool isOnSpecial;
    public static int defenseCooldown;
    public static int attackSwordCooldown;
    public static int daggerCooldown;
    public static int clubCooldown;
    public static int daggerHitsLeft;
    public static int timedHitTimer;
    private static float addedSwordScale = 0.0f;
    private static float addedClubScale = 0.0f;
    private static float addedDaggerScale = 0.0f;
    private bool hasBegunWeaponEndPause;
    private float swipeSpeed;
    [XmlIgnore]
    public Rectangle mostRecentArea;
    [XmlIgnore]
    public List<Monster> monstersHitThisSwing = new List<Monster>();
    [XmlIgnore]
    private readonly NetEvent0 animateSpecialMoveEvent = new NetEvent0();
    [XmlIgnore]
    private readonly NetEvent0 defenseSwordEvent = new NetEvent0();
    [XmlIgnore]
    private readonly NetEvent1Field<int, NetInt> daggerEvent = new NetEvent1Field<int, NetInt>();
    private bool anotherClick;
    private static Vector2 center = new Vector2(1f, 15f);

    public MeleeWeapon()
    {
      this.NetFields.AddFields((INetSerializable) this.type, (INetSerializable) this.minDamage, (INetSerializable) this.maxDamage, (INetSerializable) this.speed, (INetSerializable) this.addedPrecision, (INetSerializable) this.addedDefense, (INetSerializable) this.addedAreaOfEffect, (INetSerializable) this.knockback, (INetSerializable) this.critChance, (INetSerializable) this.critMultiplier, (INetSerializable) this.appearance);
      this.Category = -98;
    }

    public MeleeWeapon(int spriteIndex)
      : this()
    {
      this.Category = -98;
      int key = spriteIndex > -10000 ? spriteIndex : Math.Abs(spriteIndex) - -10000;
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
      if (dictionary.ContainsKey(key))
      {
        string[] strArray = dictionary[key].Split('/');
        this.BaseName = strArray[0];
        this.minDamage.Value = Convert.ToInt32(strArray[2]);
        this.maxDamage.Value = Convert.ToInt32(strArray[3]);
        this.knockback.Value = (float) Convert.ToDouble(strArray[4], (IFormatProvider) CultureInfo.InvariantCulture);
        this.speed.Value = Convert.ToInt32(strArray[5]);
        this.addedPrecision.Value = Convert.ToInt32(strArray[6]);
        this.addedDefense.Value = Convert.ToInt32(strArray[7]);
        this.type.Set(Convert.ToInt32(strArray[8]));
        if ((int) (NetFieldBase<int, NetInt>) this.type == 0)
          this.type.Set(3);
        this.addedAreaOfEffect.Value = Convert.ToInt32(strArray[11]);
        this.critChance.Value = (float) Convert.ToDouble(strArray[12], (IFormatProvider) CultureInfo.InvariantCulture);
        this.critMultiplier.Value = (float) Convert.ToDouble(strArray[13], (IFormatProvider) CultureInfo.InvariantCulture);
      }
      this.Stack = 1;
      this.InitialParentTileIndex = key;
      this.CurrentParentTileIndex = this.InitialParentTileIndex;
      this.IndexOfMenuItemView = this.CurrentParentTileIndex;
      if (!this.isScythe(spriteIndex))
        return;
      this.Category = -99;
    }

    public override int GetMaxForges() => 3;

    public override Item getOne()
    {
      MeleeWeapon destination = new MeleeWeapon(this.InitialParentTileIndex);
      destination.appearance.Value = this.appearance.Value;
      destination.IndexOfMenuItemView = this.IndexOfMenuItemView;
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName()
    {
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
        return this.Name;
      string[] strArray = Game1.content.Load<Dictionary<int, string>>("Data\\weapons")[(int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex].Split('/');
      return strArray[strArray.Length - 1];
    }

    protected override string loadDescription() => Game1.content.Load<Dictionary<int, string>>("Data\\weapons")[(int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex].Split('/')[1];

    private void OnLanguageChange(LocalizedContentManager.LanguageCode code)
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
      if (!dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex))
        return;
      dictionary[(int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex].Split('/');
      this.description = this.loadDescription();
      this.DisplayName = this.loadDisplayName();
    }

    public MeleeWeapon(int spriteIndex, int type)
      : this()
    {
      this.type.Set(type);
      this.BaseName = "";
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.animateSpecialMoveEvent, (INetSerializable) this.defenseSwordEvent, (INetSerializable) this.daggerEvent);
      this.animateSpecialMoveEvent.onEvent += new NetEvent0.Event(this.doAnimateSpecialMove);
      this.defenseSwordEvent.onEvent += new NetEvent0.Event(this.doDefenseSwordFunction);
      this.daggerEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doDaggerFunction);
    }

    public override string checkForSpecialItemHoldUpMeessage() => this.InitialParentTileIndex == 4 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14122") : (string) null;

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
      float num1 = 0.0f;
      float num2 = 0.0f;
      if (!this.isScythe())
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.type)
        {
          case 0:
          case 3:
            if (MeleeWeapon.defenseCooldown > 0)
              num1 = (float) MeleeWeapon.defenseCooldown / 1500f;
            num2 = MeleeWeapon.addedSwordScale;
            break;
          case 1:
            if (MeleeWeapon.daggerCooldown > 0)
              num1 = (float) MeleeWeapon.daggerCooldown / 3000f;
            num2 = MeleeWeapon.addedDaggerScale;
            break;
          case 2:
            if (MeleeWeapon.clubCooldown > 0)
              num1 = (float) MeleeWeapon.clubCooldown / 6000f;
            num2 = MeleeWeapon.addedClubScale;
            break;
        }
      }
      bool flag = drawShadow && drawStackNumber == StackDrawType.Hide;
      if (!drawShadow | flag)
        num2 = 0.0f;
      spriteBatch.Draw(Tool.weaponsTexture, location + ((int) (NetFieldBase<int, NetInt>) this.type == 1 ? new Vector2(38f, 25f) : new Vector2(32f, 32f)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, this.IndexOfMenuItemView, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f), (float) (4.0 * ((double) scaleSize + (double) num2)), SpriteEffects.None, layerDepth);
      if (!((double) num1 > 0.0 & drawShadow) || flag || this.isScythe() || Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu && (double) scaleSize == 1.0)
        return;
      spriteBatch.Draw(Game1.staminaRect, new Rectangle((int) location.X, (int) location.Y + (64 - (int) ((double) num1 * 64.0)), 64, (int) ((double) num1 * 64.0)), Color.Red * 0.66f);
    }

    public override int maximumStackSize() => 1;

    public override int salePrice() => this.getItemLevel() * 100;

    public static void weaponsTypeUpdate(GameTime time)
    {
      if ((double) MeleeWeapon.addedSwordScale > 0.0)
        MeleeWeapon.addedSwordScale -= 0.01f;
      if ((double) MeleeWeapon.addedClubScale > 0.0)
        MeleeWeapon.addedClubScale -= 0.01f;
      if ((double) MeleeWeapon.addedDaggerScale > 0.0)
        MeleeWeapon.addedDaggerScale -= 0.01f;
      if ((double) MeleeWeapon.timedHitTimer > 0.0)
        MeleeWeapon.timedHitTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
      if (MeleeWeapon.defenseCooldown > 0)
      {
        MeleeWeapon.defenseCooldown -= time.ElapsedGameTime.Milliseconds;
        if (MeleeWeapon.defenseCooldown <= 0)
        {
          MeleeWeapon.addedSwordScale = 0.5f;
          Game1.playSound("objectiveComplete");
        }
      }
      if (MeleeWeapon.attackSwordCooldown > 0)
      {
        MeleeWeapon.attackSwordCooldown -= time.ElapsedGameTime.Milliseconds;
        if (MeleeWeapon.attackSwordCooldown <= 0)
        {
          MeleeWeapon.addedSwordScale = 0.5f;
          Game1.playSound("objectiveComplete");
        }
      }
      if (MeleeWeapon.daggerCooldown > 0)
      {
        MeleeWeapon.daggerCooldown -= time.ElapsedGameTime.Milliseconds;
        if (MeleeWeapon.daggerCooldown <= 0)
        {
          MeleeWeapon.addedDaggerScale = 0.5f;
          Game1.playSound("objectiveComplete");
        }
      }
      if (MeleeWeapon.clubCooldown <= 0)
        return;
      MeleeWeapon.clubCooldown -= time.ElapsedGameTime.Milliseconds;
      if (MeleeWeapon.clubCooldown > 0)
        return;
      MeleeWeapon.addedClubScale = 0.5f;
      Game1.playSound("objectiveComplete");
    }

    public override void tickUpdate(GameTime time, Farmer who)
    {
      this.lastUser = who;
      base.tickUpdate(time, who);
      this.animateSpecialMoveEvent.Poll();
      this.defenseSwordEvent.Poll();
      this.daggerEvent.Poll();
      if (this.isOnSpecial && (int) (NetFieldBase<int, NetInt>) this.type == 1 && MeleeWeapon.daggerHitsLeft > 0 && !who.UsingTool)
      {
        this.quickStab(who);
        this.triggerDaggerFunction(who, MeleeWeapon.daggerHitsLeft);
      }
      if (!this.anotherClick)
        return;
      this.leftClick(who);
    }

    public override bool doesShowTileLocationMarker() => false;

    public int getNumberOfDescriptionCategories()
    {
      int descriptionCategories = 1;
      if ((int) (NetFieldBase<int, NetInt>) this.speed != ((int) (NetFieldBase<int, NetInt>) this.type == 2 ? -8 : 0))
        ++descriptionCategories;
      if ((int) (NetFieldBase<int, NetInt>) this.addedDefense > 0)
        ++descriptionCategories;
      float num = (float) (NetFieldBase<float, NetFloat>) this.critChance;
      if ((int) (NetFieldBase<int, NetInt>) this.type == 1)
        num = (num + 0.005f) * 1.12f;
      if ((double) num / 0.02 >= 1.10000002384186)
        ++descriptionCategories;
      if (((double) (float) (NetFieldBase<float, NetFloat>) this.critMultiplier - 3.0) / 0.02 >= 1.0)
        ++descriptionCategories;
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.knockback != (double) this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type))
        ++descriptionCategories;
      if (this.enchantments.Count > 0 && this.enchantments[this.enchantments.Count - 1] is DiamondEnchantment)
        ++descriptionCategories;
      return descriptionCategories;
    }

    public override void leftClick(Farmer who)
    {
      if (who.health <= 0 || Game1.activeClickableMenu != null || Game1.farmEvent != null || Game1.eventUp || who.swimming.Value || who.bathingClothes.Value || who.onBridge.Value)
        return;
      if (!this.isScythe() && who.FarmerSprite.currentAnimationIndex > ((int) (NetFieldBase<int, NetInt>) this.type == 2 ? 5 : ((int) (NetFieldBase<int, NetInt>) this.type == 1 ? 0 : 5)))
      {
        who.completelyStopAnimatingOrDoingAction();
        who.CanMove = false;
        who.UsingTool = true;
        who.canReleaseTool = true;
        this.setFarmerAnimating(who);
      }
      else
      {
        if (this.isScythe() || who.FarmerSprite.currentAnimationIndex <= ((int) (NetFieldBase<int, NetInt>) this.type == 2 ? 3 : ((int) (NetFieldBase<int, NetInt>) this.type == 1 ? 0 : 3)))
          return;
        this.anotherClick = true;
      }
    }

    public virtual bool isScythe(int index = -1)
    {
      if (index == -1)
        index = this.InitialParentTileIndex;
      return this.InitialParentTileIndex == 47 || this.InitialParentTileIndex == 53;
    }

    public virtual int getItemLevel()
    {
      float num = 0.0f + (float) (int) ((double) (((int) (NetFieldBase<int, NetInt>) this.maxDamage + (int) (NetFieldBase<int, NetInt>) this.minDamage) / 2) * (1.0 + 0.03 * (double) (Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.speed) + ((int) (NetFieldBase<int, NetInt>) this.type == 1 ? 15 : 0)))) + (float) (int) ((double) ((int) (NetFieldBase<int, NetInt>) this.addedPrecision / 2 + (int) (NetFieldBase<int, NetInt>) this.addedDefense) + ((double) (float) (NetFieldBase<float, NetFloat>) this.critChance - 0.02) * 200.0 + ((double) (float) (NetFieldBase<float, NetFloat>) this.critMultiplier - 3.0) * 6.0);
      if (this.InitialParentTileIndex == 2)
        num += 20f;
      return (int) ((double) (num + (float) ((int) (NetFieldBase<int, NetInt>) this.addedDefense * 2)) / 7.0 + 1.0);
    }

    public override string getDescription()
    {
      if (this.isScythe(this.IndexOfMenuItemView))
        return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth()));
      stringBuilder.AppendLine();
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14132", (object) this.minDamage, (object) this.maxDamage));
      if ((int) (NetFieldBase<int, NetInt>) this.speed != 0)
        stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14134", (int) (NetFieldBase<int, NetInt>) this.speed > 0 ? (object) "+" : (object) "-", (object) Math.Abs((int) (NetFieldBase<int, NetInt>) this.speed)));
      if ((int) (NetFieldBase<int, NetInt>) this.addedAreaOfEffect > 0)
        stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14136", (object) this.addedAreaOfEffect));
      if ((int) (NetFieldBase<int, NetInt>) this.addedPrecision > 0)
        stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14138", (object) this.addedPrecision));
      if ((int) (NetFieldBase<int, NetInt>) this.addedDefense > 0)
        stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14140", (object) this.addedDefense));
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.critChance / 0.02 >= 2.0)
        stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14142", (object) (int) ((double) (float) (NetFieldBase<float, NetFloat>) this.critChance / 0.02)));
      if (((double) (float) (NetFieldBase<float, NetFloat>) this.critMultiplier - 3.0) / 0.02 >= 1.0)
        stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14144", (object) (int) (((double) (float) (NetFieldBase<float, NetFloat>) this.critMultiplier - 3.0) / 0.02)));
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.knockback != (double) this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type))
        stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14140", (double) (float) (NetFieldBase<float, NetFloat>) this.knockback > (double) this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type) ? (object) "+" : (object) "", (object) (int) Math.Ceiling((double) Math.Abs((float) (NetFieldBase<float, NetFloat>) this.knockback - this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type)) * 10.0)));
      return stringBuilder.ToString();
    }

    public virtual float defaultKnockBackForThisType(int type)
    {
      switch (type)
      {
        case 0:
        case 3:
          return 1f;
        case 1:
          return 0.5f;
        case 2:
          return 1.5f;
        default:
          return -1f;
      }
    }

    public virtual Rectangle getAreaOfEffect(
      int x,
      int y,
      int facingDirection,
      ref Vector2 tileLocation1,
      ref Vector2 tileLocation2,
      Rectangle wielderBoundingBox,
      int indexInCurrentAnimation)
    {
      Rectangle areaOfEffect = Rectangle.Empty;
      int num1;
      int num2;
      int num3;
      int num4;
      if ((int) (NetFieldBase<int, NetInt>) this.type == 1)
      {
        num1 = 74;
        num2 = 48;
        num3 = 42;
        num4 = -32;
      }
      else
      {
        num1 = 64;
        num2 = 64;
        num4 = -32;
        num3 = 0;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.type == 1)
      {
        switch (facingDirection)
        {
          case 0:
            areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Y - num2 - num3, num1 / 2, num2 + num3);
            tileLocation1 = new Vector2((float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Left : areaOfEffect.Right) / 64), (float) (areaOfEffect.Top / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) (areaOfEffect.Top / 64));
            areaOfEffect.Offset(20, -16);
            areaOfEffect.Height += 16;
            areaOfEffect.Width += 20;
            break;
          case 1:
            areaOfEffect = new Rectangle(wielderBoundingBox.Right, y - num2 / 2 + num4, num2, num1);
            tileLocation1 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Top : areaOfEffect.Bottom) / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) (areaOfEffect.Center.Y / 64));
            areaOfEffect.Offset(-4, 0);
            areaOfEffect.Width += 16;
            break;
          case 2:
            areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Bottom, num1, num2);
            tileLocation1 = new Vector2((float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Left : areaOfEffect.Right) / 64), (float) (areaOfEffect.Center.Y / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) (areaOfEffect.Center.Y / 64));
            areaOfEffect.Offset(12, -8);
            areaOfEffect.Width -= 21;
            break;
          case 3:
            areaOfEffect = new Rectangle(wielderBoundingBox.Left - num2, y - num2 / 2 + num4, num2, num1);
            tileLocation1 = new Vector2((float) (areaOfEffect.Left / 64), (float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Top : areaOfEffect.Bottom) / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Left / 64), (float) (areaOfEffect.Center.Y / 64));
            areaOfEffect.Offset(-12, 0);
            areaOfEffect.Width += 16;
            break;
        }
      }
      else
      {
        switch (facingDirection)
        {
          case 0:
            areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Y - num2 - num3, num1, num2 + num3);
            tileLocation1 = new Vector2((float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Left : areaOfEffect.Right) / 64), (float) (areaOfEffect.Top / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) (areaOfEffect.Top / 64));
            switch (indexInCurrentAnimation)
            {
              case 0:
                areaOfEffect.Offset(-60, -12);
                break;
              case 1:
                areaOfEffect.Offset(-48, -56);
                areaOfEffect.Height += 32;
                break;
              case 2:
                areaOfEffect.Offset(-12, -68);
                areaOfEffect.Height += 48;
                break;
              case 3:
                areaOfEffect.Offset(40, -60);
                areaOfEffect.Height += 48;
                break;
              case 4:
                areaOfEffect.Offset(56, -32);
                areaOfEffect.Height += 32;
                break;
              case 5:
                areaOfEffect.Offset(76, -32);
                break;
            }
            break;
          case 1:
            areaOfEffect = new Rectangle(wielderBoundingBox.Right, y - num2 / 2 + num4, num2, num1);
            tileLocation1 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Top : areaOfEffect.Bottom) / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) (areaOfEffect.Center.Y / 64));
            switch (indexInCurrentAnimation)
            {
              case 0:
                areaOfEffect.Offset(-44, -84);
                break;
              case 1:
                areaOfEffect.Offset(4, -44);
                break;
              case 2:
                areaOfEffect.Offset(12, -4);
                break;
              case 3:
                areaOfEffect.Offset(12, 37);
                break;
              case 4:
                areaOfEffect.Offset(-28, 60);
                break;
              case 5:
                areaOfEffect.Offset(-60, 72);
                break;
            }
            break;
          case 2:
            areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Bottom, num1, num2);
            tileLocation1 = new Vector2((float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Left : areaOfEffect.Right) / 64), (float) (areaOfEffect.Center.Y / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64), (float) (areaOfEffect.Center.Y / 64));
            switch (indexInCurrentAnimation)
            {
              case 0:
                areaOfEffect.Offset(72, -92);
                break;
              case 1:
                areaOfEffect.Offset(56, -32);
                break;
              case 2:
                areaOfEffect.Offset(40, -28);
                break;
              case 3:
                areaOfEffect.Offset(-12, -8);
                break;
              case 4:
                areaOfEffect.Offset(-80, -24);
                areaOfEffect.Width += 32;
                break;
              case 5:
                areaOfEffect.Offset(-68, -44);
                break;
            }
            break;
          case 3:
            areaOfEffect = new Rectangle(wielderBoundingBox.Left - num2, y - num2 / 2 + num4, num2, num1);
            tileLocation1 = new Vector2((float) (areaOfEffect.Left / 64), (float) ((Game1.random.NextDouble() < 0.5 ? areaOfEffect.Top : areaOfEffect.Bottom) / 64));
            tileLocation2 = new Vector2((float) (areaOfEffect.Left / 64), (float) (areaOfEffect.Center.Y / 64));
            switch (indexInCurrentAnimation)
            {
              case 0:
                areaOfEffect.Offset(56, -76);
                break;
              case 1:
                areaOfEffect.Offset(-8, -56);
                break;
              case 2:
                areaOfEffect.Offset(-16, -4);
                break;
              case 3:
                areaOfEffect.Offset(0, 37);
                break;
              case 4:
                areaOfEffect.Offset(24, 60);
                break;
              case 5:
                areaOfEffect.Offset(64, 64);
                break;
            }
            break;
        }
      }
      areaOfEffect.Inflate((int) (NetFieldBase<int, NetInt>) this.addedAreaOfEffect, (int) (NetFieldBase<int, NetInt>) this.addedAreaOfEffect);
      return areaOfEffect;
    }

    public void triggerDefenseSwordFunction(Farmer who) => this.defenseSwordEvent.Fire();

    private void doDefenseSwordFunction()
    {
      this.isOnSpecial = false;
      this.lastUser.UsingTool = false;
      this.lastUser.CanMove = true;
      this.lastUser.FarmerSprite.PauseForSingleAnimation = false;
    }

    public void doStabbingSwordFunction(Farmer who)
    {
      this.isOnSpecial = false;
      who.UsingTool = false;
      who.xVelocity = 0.0f;
      who.yVelocity = 0.0f;
    }

    public void triggerDaggerFunction(Farmer who, int dagger_hits_left) => this.daggerEvent.Fire(dagger_hits_left);

    private void doDaggerFunction(int dagger_hits)
    {
      Vector2 positionAwayFromBox = this.lastUser.getUniformPositionAwayFromBox(this.lastUser.FacingDirection, 48);
      int daggerHitsLeft = MeleeWeapon.daggerHitsLeft;
      MeleeWeapon.daggerHitsLeft = dagger_hits;
      this.DoDamage(Game1.currentLocation, (int) positionAwayFromBox.X, (int) positionAwayFromBox.Y, this.lastUser.FacingDirection, 1, this.lastUser);
      MeleeWeapon.daggerHitsLeft = daggerHitsLeft;
      if (this.lastUser != null && this.lastUser.IsLocalPlayer)
        --MeleeWeapon.daggerHitsLeft;
      this.isOnSpecial = false;
      this.lastUser.UsingTool = false;
      this.lastUser.CanMove = true;
      this.lastUser.FarmerSprite.PauseForSingleAnimation = false;
      if (MeleeWeapon.daggerHitsLeft <= 0 || this.lastUser == null || !this.lastUser.IsLocalPlayer)
        return;
      this.quickStab(this.lastUser);
    }

    public void triggerClubFunction(Farmer who)
    {
      who.currentLocation.playSound("clubSmash");
      who.currentLocation.damageMonster(new Rectangle((int) this.lastUser.Position.X - 192, this.lastUser.GetBoundingBox().Y - 192, 384, 384), (int) (NetFieldBase<int, NetInt>) this.minDamage, (int) (NetFieldBase<int, NetInt>) this.maxDamage, false, 1.5f, 100, 0.0f, 1f, false, this.lastUser);
      Game1.viewport.Y -= 21;
      Game1.viewport.X += Game1.random.Next(-32, 32);
      Vector2 positionAwayFromBox = this.lastUser.getUniformPositionAwayFromBox(this.lastUser.FacingDirection, 64);
      switch (this.lastUser.FacingDirection)
      {
        case 0:
        case 2:
          positionAwayFromBox.X -= 32f;
          positionAwayFromBox.Y -= 32f;
          break;
        case 1:
          positionAwayFromBox.X -= 42f;
          positionAwayFromBox.Y -= 32f;
          break;
        case 3:
          positionAwayFromBox.Y -= 32f;
          break;
      }
      Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 128, 64, 64), 40f, 4, 0, positionAwayFromBox, false, this.lastUser.FacingDirection == 1));
      this.lastUser.jitterStrength = 2f;
    }

    private void beginSpecialMove(Farmer who)
    {
      if (Game1.fadeToBlack)
        return;
      this.isOnSpecial = true;
      who.UsingTool = true;
      who.CanMove = false;
    }

    private void quickStab(Farmer who)
    {
      AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction = (AnimatedSprite.endOfAnimationBehavior) (f => this.triggerDaggerFunction(f, MeleeWeapon.daggerHitsLeft));
      if (!this.lastUser.IsLocalPlayer)
        endOfBehaviorFunction = (AnimatedSprite.endOfAnimationBehavior) null;
      switch (who.FacingDirection)
      {
        case 0:
          ((FarmerSprite) who.Sprite).animateOnce(276, 15f, 2, endOfBehaviorFunction);
          this.Update(0, 0, who);
          break;
        case 1:
          ((FarmerSprite) who.Sprite).animateOnce(274, 15f, 2, endOfBehaviorFunction);
          this.Update(1, 0, who);
          break;
        case 2:
          ((FarmerSprite) who.Sprite).animateOnce(272, 15f, 2, endOfBehaviorFunction);
          this.Update(2, 0, who);
          break;
        case 3:
          ((FarmerSprite) who.Sprite).animateOnce(278, 15f, 2, endOfBehaviorFunction);
          this.Update(3, 0, who);
          break;
      }
      this.beginSpecialMove(who);
      who.currentLocation.localSound("daggerswipe");
    }

    private int specialCooldown()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.type == 3)
        return MeleeWeapon.defenseCooldown;
      if ((int) (NetFieldBase<int, NetInt>) this.type == 1)
        return MeleeWeapon.daggerCooldown;
      if ((int) (NetFieldBase<int, NetInt>) this.type == 2)
        return MeleeWeapon.clubCooldown;
      return (int) (NetFieldBase<int, NetInt>) this.type == 0 ? MeleeWeapon.attackSwordCooldown : 0;
    }

    public void animateSpecialMove(Farmer who)
    {
      this.lastUser = who;
      if ((int) (NetFieldBase<int, NetInt>) this.type == 3 && (this.BaseName.Contains("Scythe") || this.isScythe()) || Game1.fadeToBlack || this.specialCooldown() > 0)
        return;
      this.animateSpecialMoveEvent.Fire();
    }

    private void doAnimateSpecialMove()
    {
      if (this.lastUser == null || this.lastUser.CurrentTool != this)
        return;
      if (this.lastUser.isEmoteAnimating)
        this.lastUser.EndEmoteAnimation();
      if ((int) (NetFieldBase<int, NetInt>) this.type == 3)
      {
        AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction = new AnimatedSprite.endOfAnimationBehavior(this.triggerDefenseSwordFunction);
        if (!this.lastUser.IsLocalPlayer)
          endOfBehaviorFunction = (AnimatedSprite.endOfAnimationBehavior) null;
        switch (this.lastUser.FacingDirection)
        {
          case 0:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(252, 500f, 1, endOfBehaviorFunction);
            this.Update(0, 0, this.lastUser);
            break;
          case 1:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(243, 500f, 1, endOfBehaviorFunction);
            this.Update(1, 0, this.lastUser);
            break;
          case 2:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(234, 500f, 1, endOfBehaviorFunction);
            this.Update(2, 0, this.lastUser);
            break;
          case 3:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(259, 500f, 1, endOfBehaviorFunction);
            this.Update(3, 0, this.lastUser);
            break;
        }
        this.lastUser.currentLocation.localSound("batFlap");
        this.beginSpecialMove(this.lastUser);
        if (this.lastUser.IsLocalPlayer)
          MeleeWeapon.defenseCooldown = 1500;
        if (this.lastUser.professions.Contains(28))
          MeleeWeapon.defenseCooldown /= 2;
        if (!this.hasEnchantmentOfType<ArtfulEnchantment>())
          return;
        MeleeWeapon.defenseCooldown /= 2;
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.type == 2)
      {
        AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction = new AnimatedSprite.endOfAnimationBehavior(this.triggerClubFunction);
        if (!this.lastUser.IsLocalPlayer)
          endOfBehaviorFunction = (AnimatedSprite.endOfAnimationBehavior) null;
        this.lastUser.currentLocation.localSound("clubswipe");
        switch (this.lastUser.FacingDirection)
        {
          case 0:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(176, 40f, 8, endOfBehaviorFunction);
            this.Update(0, 0, this.lastUser);
            break;
          case 1:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(168, 40f, 8, endOfBehaviorFunction);
            this.Update(1, 0, this.lastUser);
            break;
          case 2:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(160, 40f, 8, endOfBehaviorFunction);
            this.Update(2, 0, this.lastUser);
            break;
          case 3:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(184, 40f, 8, endOfBehaviorFunction);
            this.Update(3, 0, this.lastUser);
            break;
        }
        this.beginSpecialMove(this.lastUser);
        if (this.lastUser.IsLocalPlayer)
          MeleeWeapon.clubCooldown = 6000;
        if (this.lastUser.professions.Contains(28))
          MeleeWeapon.clubCooldown /= 2;
        if (!this.hasEnchantmentOfType<ArtfulEnchantment>())
          return;
        MeleeWeapon.clubCooldown /= 2;
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.type != 1)
          return;
        MeleeWeapon.daggerHitsLeft = 4;
        this.quickStab(this.lastUser);
        if (this.lastUser.IsLocalPlayer)
          MeleeWeapon.daggerCooldown = 3000;
        if (this.lastUser.professions.Contains(28))
          MeleeWeapon.daggerCooldown /= 2;
        if (!this.hasEnchantmentOfType<ArtfulEnchantment>())
          return;
        MeleeWeapon.daggerCooldown /= 2;
      }
    }

    public void doSwipe(
      int type,
      Vector2 position,
      int facingDirection,
      float swipeSpeed,
      Farmer f)
    {
      if (f == null || f.CurrentTool != this)
        return;
      if (f.IsLocalPlayer)
      {
        f.TemporaryPassableTiles.Clear();
        f.currentLocation.lastTouchActionLocation = Vector2.Zero;
      }
      swipeSpeed *= 1.3f;
      switch (type)
      {
        case 2:
          if (f.CurrentTool == this)
          {
            switch (f.FacingDirection)
            {
              case 0:
                ((FarmerSprite) f.Sprite).animateOnce(248, swipeSpeed, 8);
                this.Update(0, 0, f);
                break;
              case 1:
                ((FarmerSprite) f.Sprite).animateOnce(240, swipeSpeed, 8);
                this.Update(1, 0, f);
                break;
              case 2:
                ((FarmerSprite) f.Sprite).animateOnce(232, swipeSpeed, 8);
                this.Update(2, 0, f);
                break;
              case 3:
                ((FarmerSprite) f.Sprite).animateOnce(256, swipeSpeed, 8);
                this.Update(3, 0, f);
                break;
            }
          }
          f.currentLocation.localSound("clubswipe");
          break;
        case 3:
          if (f.CurrentTool == this)
          {
            switch (f.FacingDirection)
            {
              case 0:
                ((FarmerSprite) f.Sprite).animateOnce(248, swipeSpeed, 6);
                this.Update(0, 0, f);
                break;
              case 1:
                ((FarmerSprite) f.Sprite).animateOnce(240, swipeSpeed, 6);
                this.Update(1, 0, f);
                break;
              case 2:
                ((FarmerSprite) f.Sprite).animateOnce(232, swipeSpeed, 6);
                this.Update(2, 0, f);
                break;
              case 3:
                ((FarmerSprite) f.Sprite).animateOnce(256, swipeSpeed, 6);
                this.Update(3, 0, f);
                break;
            }
          }
          else if (f.FacingDirection != 0)
          {
            int facingDirection1 = f.FacingDirection;
          }
          if (!f.ShouldHandleAnimationSound())
            break;
          f.currentLocation.localSound("swordswipe");
          break;
      }
    }

    public void setFarmerAnimating(Farmer who)
    {
      this.anotherClick = false;
      who.FarmerSprite.PauseForSingleAnimation = false;
      who.FarmerSprite.StopAnimation();
      this.hasBegunWeaponEndPause = false;
      this.swipeSpeed = (float) (400 - (int) (NetFieldBase<int, NetInt>) this.speed * 40 - who.addedSpeed * 40);
      this.swipeSpeed *= 1f - who.weaponSpeedModifier;
      if (who.IsLocalPlayer)
      {
        foreach (BaseEnchantment enchantment in this.enchantments)
        {
          if (enchantment is BaseWeaponEnchantment)
            (enchantment as BaseWeaponEnchantment).OnSwing(this, who);
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.type != 1)
      {
        this.doSwipe((int) (NetFieldBase<int, NetInt>) this.type, who.Position, who.FacingDirection, this.swipeSpeed / ((int) (NetFieldBase<int, NetInt>) this.type == 2 ? 5f : 8f), who);
        who.lastClick = Vector2.Zero;
        Vector2 toolLocation = who.GetToolLocation(true);
        this.DoDamage(who.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, who.FacingDirection, 1, who);
      }
      else
      {
        if (who.IsLocalPlayer)
          who.currentLocation.playSound("daggerswipe");
        this.swipeSpeed /= 4f;
        switch (who.FacingDirection)
        {
          case 0:
            ((FarmerSprite) who.Sprite).animateOnce(276, this.swipeSpeed, 2);
            this.Update(0, 0, who);
            break;
          case 1:
            ((FarmerSprite) who.Sprite).animateOnce(274, this.swipeSpeed, 2);
            this.Update(1, 0, who);
            break;
          case 2:
            ((FarmerSprite) who.Sprite).animateOnce(272, this.swipeSpeed, 2);
            this.Update(2, 0, who);
            break;
          case 3:
            ((FarmerSprite) who.Sprite).animateOnce(278, this.swipeSpeed, 2);
            this.Update(3, 0, who);
            break;
        }
        Vector2 toolLocation = who.GetToolLocation(true);
        this.DoDamage(who.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, who.FacingDirection, 1, who);
      }
      if (who.CurrentTool != null)
        return;
      who.completelyStopAnimatingOrDoingAction();
      who.forceCanMove();
    }

    public override void actionWhenBeingHeld(Farmer who) => base.actionWhenBeingHeld(who);

    public override void actionWhenStopBeingHeld(Farmer who)
    {
      who.UsingTool = false;
      this.anotherClick = false;
      base.actionWhenStopBeingHeld(who);
    }

    public override void endUsing(GameLocation location, Farmer who) => base.endUsing(location, who);

    public virtual void RecalculateAppliedForges(bool force = false)
    {
      if (this.enchantments.Count == 0 && !force)
        return;
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment.IsForge())
          enchantment.UnapplyTo((Item) this);
      }
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
      int initialParentTileIndex = this.InitialParentTileIndex;
      if (dictionary.ContainsKey(initialParentTileIndex))
      {
        string[] strArray = dictionary[initialParentTileIndex].Split('/');
        this.BaseName = strArray[0];
        this.minDamage.Value = Convert.ToInt32(strArray[2]);
        this.maxDamage.Value = Convert.ToInt32(strArray[3]);
        this.knockback.Value = (float) Convert.ToDouble(strArray[4], (IFormatProvider) CultureInfo.InvariantCulture);
        this.speed.Value = Convert.ToInt32(strArray[5]);
        this.addedPrecision.Value = Convert.ToInt32(strArray[6]);
        this.addedDefense.Value = Convert.ToInt32(strArray[7]);
        this.type.Set(Convert.ToInt32(strArray[8]));
        if ((int) (NetFieldBase<int, NetInt>) this.type == 0)
          this.type.Set(3);
        this.addedAreaOfEffect.Value = Convert.ToInt32(strArray[11]);
        this.critChance.Value = (float) Convert.ToDouble(strArray[12], (IFormatProvider) CultureInfo.InvariantCulture);
        this.critMultiplier.Value = (float) Convert.ToDouble(strArray[13], (IFormatProvider) CultureInfo.InvariantCulture);
      }
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment.IsForge())
          enchantment.ApplyTo((Item) this);
      }
    }

    public virtual void DoDamage(
      GameLocation location,
      int x,
      int y,
      int facingDirection,
      int power,
      Farmer who)
    {
      if (!who.IsLocalPlayer)
        return;
      this.isOnSpecial = false;
      if ((int) (NetFieldBase<int, NetInt>) this.type != 2)
        this.DoFunction(location, x, y, power, who);
      this.lastUser = who;
      Vector2 zero1 = Vector2.Zero;
      Vector2 zero2 = Vector2.Zero;
      Rectangle areaOfEffect = this.getAreaOfEffect(x, y, facingDirection, ref zero1, ref zero2, who.GetBoundingBox(), who.FarmerSprite.currentAnimationIndex);
      this.mostRecentArea = areaOfEffect;
      float num = (float) (NetFieldBase<float, NetFloat>) this.critChance;
      if ((int) (NetFieldBase<int, NetInt>) this.type == 1)
        num = (num + 0.005f) * 1.12f;
      if (location.damageMonster(areaOfEffect, (int) ((double) (int) (NetFieldBase<int, NetInt>) this.minDamage * (1.0 + (double) who.attackIncreaseModifier)), (int) ((double) (int) (NetFieldBase<int, NetInt>) this.maxDamage * (1.0 + (double) who.attackIncreaseModifier)), false, (float) (NetFieldBase<float, NetFloat>) this.knockback * (1f + who.knockbackModifier), (int) ((double) (int) (NetFieldBase<int, NetInt>) this.addedPrecision * (1.0 + (double) who.weaponPrecisionModifier)), num * (1f + who.critChanceModifier), (float) (NetFieldBase<float, NetFloat>) this.critMultiplier * (1f + who.critPowerModifier), (int) (NetFieldBase<int, NetInt>) this.type != 1 || !this.isOnSpecial, this.lastUser) && (int) (NetFieldBase<int, NetInt>) this.type == 2)
        location.playSound("clubhit");
      string cueName = "";
      location.projectiles.Filter((Func<Projectile, bool>) (projectile =>
      {
        if (areaOfEffect.Intersects(projectile.getBoundingBox()) && !projectile.ignoreMeleeAttacks.Value)
          projectile.behaviorOnCollisionWithOther(location);
        return !projectile.destroyMe;
      }));
      foreach (Vector2 removeDuplicate in Utility.removeDuplicates(Utility.getListOfTileLocationsForBordersOfNonTileRectangle(areaOfEffect)))
      {
        if (location.terrainFeatures.ContainsKey(removeDuplicate) && location.terrainFeatures[removeDuplicate].performToolAction((Tool) this, 0, removeDuplicate, location))
          location.terrainFeatures.Remove(removeDuplicate);
        if (location.objects.ContainsKey(removeDuplicate) && location.objects[removeDuplicate].performToolAction((Tool) this, location))
          location.objects.Remove(removeDuplicate);
        if (location.performToolAction((Tool) this, (int) removeDuplicate.X, (int) removeDuplicate.Y))
          break;
      }
      if (!cueName.Equals(""))
        Game1.playSound(cueName);
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
      if (who == null || !who.isRidingHorse())
        return;
      who.completelyStopAnimatingOrDoingAction();
    }

    public int getDrawnItemIndex() => this.appearance.Value < 0 ? this.InitialParentTileIndex : this.appearance.Value;

    public static Rectangle getSourceRect(int index) => Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, index, 16, 16);

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
      if (this.isScythe(this.IndexOfMenuItemView))
        return;
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(120, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Color color1 = Game1.textColor;
      if (this.hasEnchantmentOfType<RubyEnchantment>())
        color1 = new Color(0, 120, 120);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Damage", (object) this.minDamage, (object) this.maxDamage), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), color1 * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      if ((int) (NetFieldBase<int, NetInt>) this.speed != ((int) (NetFieldBase<int, NetInt>) this.type == 2 ? -8 : 0))
      {
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(130, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        bool flag = (int) (NetFieldBase<int, NetInt>) this.type == 2 && (int) (NetFieldBase<int, NetInt>) this.speed < -8 || (int) (NetFieldBase<int, NetInt>) this.type != 2 && (int) (NetFieldBase<int, NetInt>) this.speed < 0;
        Color color2 = Game1.textColor;
        if (this.hasEnchantmentOfType<EmeraldEnchantment>())
          color2 = new Color(0, 120, 120);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Speed", (object) ((((int) (NetFieldBase<int, NetInt>) this.type == 2 ? (int) (NetFieldBase<int, NetInt>) this.speed - -8 : (int) (NetFieldBase<int, NetInt>) this.speed) > 0 ? "+" : "") + (((int) (NetFieldBase<int, NetInt>) this.type == 2 ? (int) (NetFieldBase<int, NetInt>) this.speed - -8 : (int) (NetFieldBase<int, NetInt>) this.speed) / 2).ToString())), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), flag ? Color.DarkRed : color2 * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      if ((int) (NetFieldBase<int, NetInt>) this.addedDefense > 0)
      {
        Color color3 = Game1.textColor;
        if (this.hasEnchantmentOfType<TopazEnchantment>())
          color3 = new Color(0, 120, 120);
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) this.addedDefense), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), color3 * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      float num = (float) (NetFieldBase<float, NetFloat>) this.critChance;
      if ((int) (NetFieldBase<int, NetInt>) this.type == 1)
        num = (num + 0.005f) * 1.12f;
      if ((double) num / 0.02 >= 1.10000002384186)
      {
        Color color4 = Game1.textColor;
        if (this.hasEnchantmentOfType<AquamarineEnchantment>())
          color4 = new Color(0, 120, 120);
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(40, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", (object) (int) Math.Round(((double) num - 1.0 / 1000.0) / 0.02)), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), color4 * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      if (((double) (float) (NetFieldBase<float, NetFloat>) this.critMultiplier - 3.0) / 0.02 >= 1.0)
      {
        Color color5 = Game1.textColor;
        if (this.hasEnchantmentOfType<JadeEnchantment>())
          color5 = new Color(0, 120, 120);
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16), (float) (y + 16 + 4)), new Rectangle(160, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", (object) (int) (((double) (float) (NetFieldBase<float, NetFloat>) this.critMultiplier - 3.0) / 0.02)), font, new Vector2((float) (x + 16 + 44), (float) (y + 16 + 12)), color5 * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.knockback != (double) this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type))
      {
        Color color6 = Game1.textColor;
        if (this.hasEnchantmentOfType<AmethystEnchantment>())
          color6 = new Color(0, 120, 120);
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(70, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Weight", (object) (((double) (int) Math.Ceiling((double) Math.Abs((float) (NetFieldBase<float, NetFloat>) this.knockback - this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type)) * 10.0) > (double) this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type) ? "+" : "") + ((int) Math.Ceiling((double) Math.Abs((float) (NetFieldBase<float, NetFloat>) this.knockback - this.defaultKnockBackForThisType((int) (NetFieldBase<int, NetInt>) this.type)) * 10.0)).ToString())), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), color6 * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      if (this.enchantments.Count > 0 && this.enchantments[this.enchantments.Count - 1] is DiamondEnchantment)
      {
        Color color7 = new Color(0, 120, 120);
        int sub1 = this.GetMaxForges() - this.GetTotalForgeLevels();
        string text = sub1 != 1 ? Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", (object) sub1) : Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Singular", (object) sub1);
        Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2((float) (x + 16), (float) (y + 16 + 12)), color7 * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment.ShouldBeDisplayed())
        {
          Utility.drawWithShadow(spriteBatch, Game1.mouseCursors2, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle((int) sbyte.MaxValue, 35, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
          Utility.drawTextWithShadow(spriteBatch, BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), new Color(120, 0, 210) * 0.9f * alpha);
          y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
        }
      }
    }

    public override Point getExtraSpaceNeededForTooltipSpecialIcons(
      SpriteFont font,
      int minWidth,
      int horizontalBuffer,
      int startingHeight,
      StringBuilder descriptionText,
      string boldTitleText,
      int moneyAmountToDisplayAtBottom)
    {
      int num = 9999;
      Point tooltipSpecialIcons = new Point(0, 0);
      tooltipSpecialIcons.Y += Math.Max(60, (boldTitleText != null ? (int) ((double) Game1.dialogueFont.MeasureString(boldTitleText).Y + 16.0) : 0) + 32) + (int) font.MeasureString("T").Y + (moneyAmountToDisplayAtBottom > -1 ? (int) ((double) font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").Y + 4.0) : 0);
      tooltipSpecialIcons.Y += this.isScythe() ? 0 : this.getNumberOfDescriptionCategories() * 4 * 12;
      tooltipSpecialIcons.Y += (int) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y;
      tooltipSpecialIcons.X = (int) Math.Max((float) minWidth, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Damage", (object) num, (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Speed", (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", (object) num)).X + (float) horizontalBuffer, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Weight", (object) num)).X + (float) horizontalBuffer))))));
      if (this.enchantments.Count > 0 && (object) (this.enchantments[this.enchantments.Count - 1] as DiamondEnchantment) != null)
        tooltipSpecialIcons.X = (int) Math.Max((float) tooltipSpecialIcons.X, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", (object) this.GetMaxForges())).X);
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment.ShouldBeDisplayed())
          tooltipSpecialIcons.Y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      return tooltipSpecialIcons;
    }

    public virtual void drawDuringUse(
      int frameOfFarmerAnimation,
      int facingDirection,
      SpriteBatch spriteBatch,
      Vector2 playerPosition,
      Farmer f)
    {
      MeleeWeapon.drawDuringUse(frameOfFarmerAnimation, facingDirection, spriteBatch, playerPosition, f, MeleeWeapon.getSourceRect(this.getDrawnItemIndex()), (int) (NetFieldBase<int, NetInt>) this.type, this.isOnSpecial);
    }

    public override bool CanForge(Item item) => item is MeleeWeapon meleeWeapon && (NetFieldBase<int, NetInt>) meleeWeapon.type == this.type || base.CanForge(item);

    public override bool CanAddEnchantment(BaseEnchantment enchantment) => (!(enchantment is GalaxySoulEnchantment) || this.isGalaxyWeapon()) && base.CanAddEnchantment(enchantment);

    public bool isGalaxyWeapon() => this.InitialParentTileIndex == 4 || this.InitialParentTileIndex == 23 || this.InitialParentTileIndex == 29;

    public void transform(int newIndex)
    {
      this.CurrentParentTileIndex = newIndex;
      this.InitialParentTileIndex = newIndex;
      this.IndexOfMenuItemView = newIndex;
      this.appearance.Value = -1;
      this.RecalculateAppliedForges(true);
    }

    public override bool Forge(Item item, bool count_towards_stats = false)
    {
      if (this.isScythe())
        return false;
      if (!(item is MeleeWeapon meleeWeapon) || !((NetFieldBase<int, NetInt>) meleeWeapon.type == this.type))
        return base.Forge(item, count_towards_stats);
      this.appearance.Value = this.IndexOfMenuItemView = meleeWeapon.getDrawnItemIndex();
      return true;
    }

    public static void drawDuringUse(
      int frameOfFarmerAnimation,
      int facingDirection,
      SpriteBatch spriteBatch,
      Vector2 playerPosition,
      Farmer f,
      Rectangle sourceRect,
      int type,
      bool isOnSpecial)
    {
      if (type != 1)
      {
        if (isOnSpecial)
        {
          switch (type)
          {
            case 2:
              switch (facingDirection)
              {
                case 1:
                  switch (frameOfFarmerAnimation)
                  {
                    case 0:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X - 32.0 - 12.0), playerPosition.Y - 80f), new Rectangle?(sourceRect), Color.White, -3f * (float) Math.PI / 8f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 1:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, (float) ((double) playerPosition.Y - 64.0 - 48.0)), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 2:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 128.0 - 16.0), (float) ((double) playerPosition.Y - 64.0 - 12.0)), new Rectangle?(sourceRect), Color.White, 3f * (float) Math.PI / 8f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 3:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 72f, (float) ((double) playerPosition.Y - 64.0 + 16.0 - 32.0)), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 4:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f, (float) ((double) playerPosition.Y - 64.0 + 16.0 - 16.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 5:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 96.0 - 12.0), (float) ((double) playerPosition.Y - 64.0 + 16.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 6:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 96.0 - 16.0), (float) ((double) playerPosition.Y - 64.0 + 40.0 - 8.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 7:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 96.0 - 8.0), playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, 0.9817477f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    default:
                      return;
                  }
                case 3:
                  switch (frameOfFarmerAnimation)
                  {
                    case 0:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0 + 8.0), (float) ((double) playerPosition.Y - 56.0 - 64.0)), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 1:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -1.963495f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 2:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 12f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.748894f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 3:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X - 32.0 - 4.0), playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.356194f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 4:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X - 16.0 - 24.0), (float) ((double) playerPosition.Y + 64.0 + 12.0 - 64.0)), new Rectangle?(sourceRect), Color.White, 4.31969f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 5:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 20f, (float) ((double) playerPosition.Y + 64.0 + 40.0 - 64.0)), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 6:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y + 64.0 + 56.0)), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    case 7:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 8f, (float) ((double) playerPosition.Y + 64.0 + 64.0)), new Rectangle?(sourceRect), Color.White, 3.730641f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                      return;
                    default:
                      return;
                  }
                default:
                  switch (frameOfFarmerAnimation)
                  {
                    case 0:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 24f, (float) ((double) playerPosition.Y - 21.0 - 8.0 - 64.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                      break;
                    case 1:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 21.0 - 64.0 + 4.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                      break;
                    case 2:
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 21.0 + 20.0 - 64.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                      break;
                    case 3:
                      if (facingDirection == 2)
                      {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 + 8.0), playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                        break;
                      }
                      spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 21.0 + 32.0 - 64.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                      break;
                    case 4:
                      if (facingDirection == 2)
                      {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 + 8.0), playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                        break;
                      }
                      break;
                    case 5:
                      if (facingDirection == 2)
                      {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0), (float) ((double) playerPosition.Y + 64.0 - 20.0)), new Rectangle?(sourceRect), Color.White, 2.356194f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                        break;
                      }
                      break;
                    case 6:
                      if (facingDirection == 2)
                      {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0), (float) ((double) playerPosition.Y + 64.0 + 54.0)), new Rectangle?(sourceRect), Color.White, 2.356194f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                        break;
                      }
                      break;
                    case 7:
                      if (facingDirection == 2)
                      {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0), (float) ((double) playerPosition.Y + 64.0 + 58.0)), new Rectangle?(sourceRect), Color.White, 2.356194f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                        break;
                      }
                      break;
                  }
                  if (f.FacingDirection != 0)
                    return;
                  f.FarmerRenderer.draw(spriteBatch, f.FarmerSprite, f.FarmerSprite.SourceRect, f.getLocalPosition(Game1.viewport), new Vector2(0.0f, (float) (((double) f.yOffset + 128.0 - (double) (f.GetBoundingBox().Height / 2)) / 4.0 + 4.0)), Math.Max(0.0f, (float) ((double) f.getStandingY() / 10000.0 + 0.00989999994635582)), Color.White, 0.0f, f);
                  return;
              }
            case 3:
              switch (f.FacingDirection)
              {
                case 0:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 44f), new Rectangle?(sourceRect), Color.White, -1.767146f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 1) / 10000f));
                  return;
                case 1:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, -3f * (float) Math.PI / 16f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 1) / 10000f));
                  return;
                case 2:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 52.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, -5.105088f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 2) / 10000f));
                  return;
                case 3:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 56.0), playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, 3f * (float) Math.PI / 16f, new Vector2(15f, 15f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() + 1) / 10000f));
                  return;
                default:
                  return;
              }
          }
        }
        else
        {
          switch (facingDirection)
          {
            case 0:
              switch (frameOfFarmerAnimation)
              {
                case 0:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                case 1:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -1.570796f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                case 2:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -3f * (float) Math.PI / 8f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                case 3:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                case 4:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.0f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                case 5:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                case 6:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                case 7:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 44.0), playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -1.963495f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32 - 8) / 10000f));
                  return;
                default:
                  return;
              }
            case 1:
              switch (frameOfFarmerAnimation)
              {
                case 0:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 40f, (float) ((double) playerPosition.Y - 64.0 + 8.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 1) / 10000f));
                  return;
                case 1:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 56f, (float) ((double) playerPosition.Y - 64.0 + 28.0)), new Rectangle?(sourceRect), Color.White, 0.0f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 1) / 10000f));
                  return;
                case 2:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0), playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 1) / 10000f));
                  return;
                case 3:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0), playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, 1.570796f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 4:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 28.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 1.963495f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 5:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 48.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 6:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 48.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 7:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), (float) ((double) playerPosition.Y + 64.0 + 12.0)), new Rectangle?(sourceRect), Color.White, 1.963495f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                default:
                  return;
              }
            case 2:
              switch (frameOfFarmerAnimation)
              {
                case 0:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                case 1:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 52f, playerPosition.Y - 8f), new Rectangle?(sourceRect), Color.White, 1.570796f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                case 2:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 40f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 1.570796f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                case 3:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                case 4:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 8f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, 3.141593f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                case 5:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.534292f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                case 6:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.534292f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                case 7:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -5.105088f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
                  return;
                default:
                  return;
              }
            case 3:
              switch (frameOfFarmerAnimation)
              {
                case 0:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 64.0 - 16.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() - 1) / 10000f));
                  return;
                case 1:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 48f, (float) ((double) playerPosition.Y - 64.0 + 20.0)), new Rectangle?(sourceRect), Color.White, 0.0f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() - 1) / 10000f));
                  return;
                case 2:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X - 64.0 + 32.0), playerPosition.Y + 16f), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() - 1) / 10000f));
                  return;
                case 3:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 4f, playerPosition.Y + 44f), new Rectangle?(sourceRect), Color.White, -1.570796f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 4:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f), new Rectangle?(sourceRect), Color.White, -1.963495f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 5:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.356194f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 6:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.356194f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                case 7:
                  spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 44f, playerPosition.Y + 96f), new Rectangle?(sourceRect), Color.White, -5.105088f, MeleeWeapon.center, 4f, SpriteEffects.FlipVertically, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
                  return;
                default:
                  return;
              }
          }
        }
      }
      else
      {
        frameOfFarmerAnimation %= 2;
        switch (facingDirection)
        {
          case 0:
            if (frameOfFarmerAnimation != 0)
            {
              if (frameOfFarmerAnimation != 1)
                break;
              spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32) / 10000f));
              break;
            }
            spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0), playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() - 32) / 10000f));
            break;
          case 1:
            if (frameOfFarmerAnimation != 0)
            {
              if (frameOfFarmerAnimation != 1)
                break;
              spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
              break;
            }
            spriteBatch.Draw(Tool.weaponsTexture, new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
            break;
          case 2:
            if (frameOfFarmerAnimation != 0)
            {
              if (frameOfFarmerAnimation != 1)
                break;
              spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 21f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
              break;
            }
            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 12f), new Rectangle?(sourceRect), Color.White, 2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 32) / 10000f));
            break;
          case 3:
            if (frameOfFarmerAnimation != 0)
            {
              if (frameOfFarmerAnimation != 1)
                break;
              spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 8f, playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, -2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
              break;
            }
            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 16f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, -2.356194f, MeleeWeapon.center, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 64) / 10000f));
            break;
        }
      }
    }
  }
}
