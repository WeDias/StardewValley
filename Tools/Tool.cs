// Decompiled with JetBrains decompiler
// Type: StardewValley.Tool
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace StardewValley
{
  [XmlInclude(typeof (MagnifyingGlass))]
  [XmlInclude(typeof (Shears))]
  [XmlInclude(typeof (MilkPail))]
  [XmlInclude(typeof (Axe))]
  [XmlInclude(typeof (Wand))]
  [XmlInclude(typeof (Hoe))]
  [XmlInclude(typeof (FishingRod))]
  [XmlInclude(typeof (MeleeWeapon))]
  [XmlInclude(typeof (Pan))]
  [XmlInclude(typeof (Pickaxe))]
  [XmlInclude(typeof (WateringCan))]
  [XmlInclude(typeof (Slingshot))]
  [XmlInclude(typeof (GenericTool))]
  public abstract class Tool : Item
  {
    public const int standardStaminaReduction = 2;
    public const int nonUpgradeable = -1;
    public const int stone = 0;
    public const int copper = 1;
    public const int steel = 2;
    public const int gold = 3;
    public const int iridium = 4;
    public const int parsnipSpriteIndex = 0;
    public const int hoeSpriteIndex = 21;
    public const int hammerSpriteIndex = 105;
    public const int axeSpriteIndex = 189;
    public const int wateringCanSpriteIndex = 273;
    public const int fishingRodSpriteIndex = 8;
    public const int batteredSwordSpriteIndex = 67;
    public const int axeMenuIndex = 215;
    public const int hoeMenuIndex = 47;
    public const int pickAxeMenuIndex = 131;
    public const int wateringCanMenuIndex = 296;
    public const int startOfNegativeWeaponIndex = -10000;
    public const string weaponsTextureName = "TileSheets\\weapons";
    public static Texture2D weaponsTexture;
    [XmlElement("initialParentTileIndex")]
    public readonly NetInt initialParentTileIndex = new NetInt();
    [XmlElement("currentParentTileIndex")]
    public readonly NetInt currentParentTileIndex = new NetInt();
    [XmlElement("indexOfMenuItemView")]
    public readonly NetInt indexOfMenuItemView = new NetInt();
    [XmlElement("stackable")]
    public readonly NetBool stackable = new NetBool();
    [XmlElement("instantUse")]
    public readonly NetBool instantUse = new NetBool();
    [XmlElement("isEfficient")]
    public readonly NetBool isEfficient = new NetBool();
    [XmlElement("animationSpeedModifier")]
    public readonly NetFloat animationSpeedModifier = new NetFloat(1f);
    [XmlIgnore]
    private string _description;
    public static Color copperColor = new Color(198, 108, 43);
    public static Color steelColor = new Color(197, 226, 222);
    public static Color goldColor = new Color(248, (int) byte.MaxValue, 73);
    public static Color iridiumColor = new Color(144, 135, 181);
    [XmlElement("upgradeLevel")]
    public readonly NetInt upgradeLevel = new NetInt();
    [XmlElement("numAttachmentSlots")]
    public readonly NetInt numAttachmentSlots = new NetInt();
    protected Farmer lastUser;
    public readonly NetObjectArray<Object> attachments = new NetObjectArray<Object>();
    [XmlIgnore]
    protected string displayName;
    [XmlElement("enchantments")]
    public readonly NetList<BaseEnchantment, NetRef<BaseEnchantment>> enchantments = new NetList<BaseEnchantment, NetRef<BaseEnchantment>>();
    [XmlElement("previousEnchantments")]
    public readonly NetStringList previousEnchantments = new NetStringList();

    [XmlIgnore]
    public string description
    {
      get
      {
        if (this._description == null)
          this._description = this.loadDescription();
        return this._description;
      }
      set => this._description = value;
    }

    public string BaseName
    {
      get => (string) (NetFieldBase<string, NetString>) this.netName;
      set => this.netName.Set(value);
    }

    [XmlIgnore]
    public override string DisplayName
    {
      get
      {
        this.displayName = this.loadDisplayName();
        switch ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel)
        {
          case 1:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14299", (object) this.displayName);
          case 2:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14300", (object) this.displayName);
          case 3:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14301", (object) this.displayName);
          case 4:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14302", (object) this.displayName);
          default:
            return this.displayName;
        }
      }
      set => this.displayName = value;
    }

    public override string Name
    {
      get
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel)
        {
          case 1:
            return "Copper " + this.BaseName;
          case 2:
            return "Steel " + this.BaseName;
          case 3:
            return "Gold " + this.BaseName;
          case 4:
            return "Iridium " + this.BaseName;
          default:
            return this.BaseName;
        }
      }
      set => this.BaseName = value;
    }

    public override int Stack
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.stackable ? ((StardewValley.Tools.Stackable) this).NumberInStack : 1;
      set
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.stackable)
          return;
        this.Stack = Math.Min(Math.Max(0, value), this.maximumStackSize());
      }
    }

    public string Description => this.description;

    [XmlIgnore]
    public int CurrentParentTileIndex
    {
      get => (int) (NetFieldBase<int, NetInt>) this.currentParentTileIndex;
      set => this.currentParentTileIndex.Set(value);
    }

    public int InitialParentTileIndex
    {
      get => (int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex;
      set => this.initialParentTileIndex.Set(value);
    }

    public int IndexOfMenuItemView
    {
      get => (int) (NetFieldBase<int, NetInt>) this.indexOfMenuItemView;
      set => this.indexOfMenuItemView.Set(value);
    }

    [XmlIgnore]
    public int UpgradeLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.upgradeLevel;
      set
      {
        this.upgradeLevel.Value = value;
        this.setNewTileIndexForUpgradeLevel();
      }
    }

    public bool InstantUse
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.instantUse;
      set => this.instantUse.Value = value;
    }

    public bool IsEfficient
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isEfficient;
      set => this.isEfficient.Value = value;
    }

    public float AnimationSpeedModifier
    {
      get => (float) (NetFieldBase<float, NetFloat>) this.animationSpeedModifier;
      set => this.animationSpeedModifier.Value = value;
    }

    public bool Stackable
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.stackable;
      set => this.stackable.Value = value;
    }

    public Tool()
    {
      this.initNetFields();
      this.Category = -99;
    }

    public Tool(
      string name,
      int upgradeLevel,
      int initialParentTileIndex,
      int indexOfMenuItemView,
      bool stackable,
      int numAttachmentSlots = 0)
      : this()
    {
      this.BaseName = name;
      this.initialParentTileIndex.Value = initialParentTileIndex;
      this.IndexOfMenuItemView = indexOfMenuItemView;
      this.Stackable = stackable;
      this.currentParentTileIndex.Value = initialParentTileIndex;
      this.numAttachmentSlots.Value = numAttachmentSlots;
      if (numAttachmentSlots > 0)
        this.attachments.SetCount(numAttachmentSlots);
      this.Category = -99;
    }

    protected virtual void initNetFields() => this.NetFields.AddFields((INetSerializable) this.initialParentTileIndex, (INetSerializable) this.currentParentTileIndex, (INetSerializable) this.indexOfMenuItemView, (INetSerializable) this.stackable, (INetSerializable) this.instantUse, (INetSerializable) this.upgradeLevel, (INetSerializable) this.numAttachmentSlots, (INetSerializable) this.attachments, (INetSerializable) this.enchantments, (INetSerializable) this.isEfficient, (INetSerializable) this.animationSpeedModifier, (INetSerializable) this.previousEnchantments);

    protected abstract string loadDisplayName();

    protected abstract string loadDescription();

    public override string getCategoryName() => this is MeleeWeapon && !(this as MeleeWeapon).isScythe(this.IndexOfMenuItemView) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14303", (object) (this as MeleeWeapon).getItemLevel(), (int) (NetFieldBase<int, NetInt>) (this as MeleeWeapon).type == 1 ? (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14304") : ((int) (NetFieldBase<int, NetInt>) (this as MeleeWeapon).type == 2 ? (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14305") : (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14306"))) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14307");

    public override Color getCategoryColor() => Color.DarkSlateGray;

    public virtual void draw(SpriteBatch b)
    {
      if (this.lastUser == null || this.lastUser.toolPower <= 0 || !this.lastUser.canReleaseTool)
        return;
      foreach (Vector2 vector2 in this.tilesAffected(this.lastUser.GetToolLocation() / 64f, this.lastUser.toolPower, this.lastUser))
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) ((int) vector2.X * 64), (float) ((int) vector2.Y * 64))), new Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
    }

    public override void drawTooltip(
      SpriteBatch spriteBatch,
      ref int x,
      ref int y,
      SpriteFont font,
      float alpha,
      StringBuilder overrideText)
    {
      base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);
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
      Point tooltipSpecialIcons = base.getExtraSpaceNeededForTooltipSpecialIcons(font, minWidth, horizontalBuffer, startingHeight, descriptionText, boldTitleText, moneyAmountToDisplayAtBottom) with
      {
        Y = startingHeight
      };
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment.ShouldBeDisplayed())
          tooltipSpecialIcons.Y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      return tooltipSpecialIcons;
    }

    public virtual void tickUpdate(GameTime time, Farmer who)
    {
    }

    public bool isHeavyHitter()
    {
      switch (this)
      {
        case MeleeWeapon _:
        case Hoe _:
        case Axe _:
          return true;
        default:
          return this is Pickaxe;
      }
    }

    public void Update(int direction, int farmerMotionFrame, Farmer who)
    {
      int num = 0;
      if (this is WateringCan)
      {
        switch (direction)
        {
          case 0:
            num = 4;
            break;
          case 1:
            num = 2;
            break;
          case 2:
            num = 0;
            break;
          case 3:
            num = 2;
            break;
        }
      }
      else if (this is FishingRod)
      {
        switch (direction)
        {
          case 0:
            num = 3;
            break;
          case 1:
            num = 0;
            break;
          case 3:
            num = 0;
            break;
        }
      }
      else
      {
        switch (direction)
        {
          case 0:
            num = 3;
            break;
          case 1:
            num = 2;
            break;
          case 3:
            num = 2;
            break;
        }
      }
      if (!this.Name.Equals("Watering Can"))
      {
        if (farmerMotionFrame < 1)
          this.CurrentParentTileIndex = this.InitialParentTileIndex;
        else if (who.FacingDirection == 0 || who.FacingDirection == 2 && farmerMotionFrame >= 2)
          this.CurrentParentTileIndex = this.InitialParentTileIndex + 1;
      }
      else
        this.CurrentParentTileIndex = farmerMotionFrame < 5 || direction == 0 ? this.InitialParentTileIndex : this.InitialParentTileIndex + 1;
      this.CurrentParentTileIndex += num;
    }

    public override int attachmentSlots() => (int) (NetFieldBase<int, NetInt>) this.numAttachmentSlots;

    public Farmer getLastFarmerToUse() => this.lastUser;

    public virtual void leftClick(Farmer who)
    {
    }

    public virtual void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      this.lastUser = who;
      Game1.recentMultiplayerRandom = new Random((int) (short) Game1.random.Next((int) short.MinValue, 32768));
      ToolFactory.getIndexFromTool(this);
      if (this.isHeavyHitter() && !(this is MeleeWeapon))
      {
        Rumble.rumble(0.1f + (float) (Game1.random.NextDouble() / 4.0), (float) (100 + Game1.random.Next(50)));
        location.damageMonster(new Rectangle(x - 32, y - 32, 64, 64), (int) (NetFieldBase<int, NetInt>) this.upgradeLevel + 1, ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel + 1) * 3, false, who);
      }
      if (!(this is MeleeWeapon) || who.UsingTool && Game1.mouseClickPolling < 50 && (int) (NetFieldBase<int, NetInt>) (this as MeleeWeapon).type != 1 && (this as MeleeWeapon).InitialParentTileIndex != 47 && MeleeWeapon.timedHitTimer <= 0 && who.FarmerSprite.currentAnimationIndex == 5 && (double) who.FarmerSprite.timer < (double) who.FarmerSprite.interval / 4.0)
        return;
      if ((int) (NetFieldBase<int, NetInt>) (this as MeleeWeapon).type == 2 && (this as MeleeWeapon).isOnSpecial)
      {
        (this as MeleeWeapon).triggerClubFunction(who);
      }
      else
      {
        if (who.FarmerSprite.currentAnimationIndex <= 0)
          return;
        MeleeWeapon.timedHitTimer = 500;
      }
    }

    public virtual void endUsing(GameLocation location, Farmer who)
    {
      who.stopJittering();
      who.canReleaseTool = false;
      int num = (double) who.Stamina <= 0.0 ? 2 : 1;
      if (Game1.isAnyGamePadButtonBeingPressed() || !who.IsLocalPlayer)
        who.lastClick = who.GetToolLocation();
      if (this.Name.Equals("Seeds"))
      {
        switch (who.FacingDirection)
        {
          case 0:
            ((FarmerSprite) who.Sprite).animateOnce(208, 150f, 4);
            break;
          case 1:
            ((FarmerSprite) who.Sprite).animateOnce(204, 150f, 4);
            break;
          case 2:
            ((FarmerSprite) who.Sprite).animateOnce(200, 150f, 4);
            break;
          case 3:
            ((FarmerSprite) who.Sprite).animateOnce(212, 150f, 4);
            break;
        }
      }
      else
      {
        switch (this)
        {
          case WateringCan _:
            if ((this as WateringCan).WaterLeft > 0 && who.ShouldHandleAnimationSound())
              who.currentLocation.localSound("wateringCan");
            switch (who.FacingDirection)
            {
              case 0:
                ((FarmerSprite) who.Sprite).animateOnce(180, 125f * (float) num, 3);
                return;
              case 1:
                ((FarmerSprite) who.Sprite).animateOnce(172, 125f * (float) num, 3);
                return;
              case 2:
                ((FarmerSprite) who.Sprite).animateOnce(164, 125f * (float) num, 3);
                return;
              case 3:
                ((FarmerSprite) who.Sprite).animateOnce(188, 125f * (float) num, 3);
                return;
              default:
                return;
            }
          case FishingRod _ when who.IsLocalPlayer && Game1.activeClickableMenu == null:
            if ((this as FishingRod).hit)
              break;
            this.DoFunction(who.currentLocation, (int) who.lastClick.X, (int) who.lastClick.Y, 1, who);
            break;
          case MeleeWeapon _:
          case Pan _:
          case Shears _:
          case MilkPail _:
          case Slingshot _:
            break;
          default:
            switch (who.FacingDirection)
            {
              case 0:
                ((FarmerSprite) who.Sprite).animateOnce(176, 60f * (float) num, 8);
                return;
              case 1:
                ((FarmerSprite) who.Sprite).animateOnce(168, 60f * (float) num, 8);
                return;
              case 2:
                ((FarmerSprite) who.Sprite).animateOnce(160, 60f * (float) num, 8);
                return;
              case 3:
                ((FarmerSprite) who.Sprite).animateOnce(184, 60f * (float) num, 8);
                return;
              default:
                return;
            }
        }
      }
    }

    public virtual bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      this.lastUser = who;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.instantUse)
      {
        who.Halt();
        this.Update(who.FacingDirection, 0, who);
        if (!(this is FishingRod) && (int) (NetFieldBase<int, NetInt>) this.upgradeLevel <= 0 && !(this is MeleeWeapon) || this is Pickaxe)
        {
          who.EndUsingTool();
          return true;
        }
      }
      if (this.Name.Equals("Wand"))
      {
        if (((Wand) this).charged)
        {
          Game1.toolAnimationDone(who);
          who.canReleaseTool = false;
          if (!who.IsLocalPlayer || !Game1.fadeToBlack)
          {
            who.CanMove = true;
            who.UsingTool = false;
          }
        }
        else
        {
          if (who.IsLocalPlayer)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3180")));
          who.UsingTool = false;
          who.canReleaseTool = false;
        }
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.instantUse)
      {
        Game1.toolAnimationDone(who);
        who.canReleaseTool = false;
        who.UsingTool = false;
      }
      else if (this.Name.Equals("Seeds"))
      {
        switch (who.FacingDirection)
        {
          case 0:
            who.Sprite.currentFrame = 208;
            this.Update(0, 0, who);
            break;
          case 1:
            who.Sprite.currentFrame = 204;
            this.Update(1, 0, who);
            break;
          case 2:
            who.Sprite.currentFrame = 200;
            this.Update(2, 0, who);
            break;
          case 3:
            who.Sprite.currentFrame = 212;
            this.Update(3, 0, who);
            break;
        }
      }
      else
      {
        switch (this)
        {
          case WateringCan _ when location.CanRefillWateringCanOnTile((int) who.GetToolLocation().X / 64, (int) who.GetToolLocation().Y / 64):
            switch (who.FacingDirection)
            {
              case 0:
                ((FarmerSprite) who.Sprite).animateOnce(182, 250f, 2);
                this.Update(0, 1, who);
                break;
              case 1:
                ((FarmerSprite) who.Sprite).animateOnce(174, 250f, 2);
                this.Update(1, 0, who);
                break;
              case 2:
                ((FarmerSprite) who.Sprite).animateOnce(166, 250f, 2);
                this.Update(2, 1, who);
                break;
              case 3:
                ((FarmerSprite) who.Sprite).animateOnce(190, 250f, 2);
                this.Update(3, 0, who);
                break;
            }
            who.canReleaseTool = false;
            break;
          case WateringCan _ when ((WateringCan) this).WaterLeft <= 0:
            Game1.toolAnimationDone(who);
            who.CanMove = true;
            who.canReleaseTool = false;
            break;
          case WateringCan _:
            who.jitterStrength = 0.25f;
            switch (who.FacingDirection)
            {
              case 0:
                who.FarmerSprite.setCurrentFrame(180);
                this.Update(0, 0, who);
                break;
              case 1:
                who.FarmerSprite.setCurrentFrame(172);
                this.Update(1, 0, who);
                break;
              case 2:
                who.FarmerSprite.setCurrentFrame(164);
                this.Update(2, 0, who);
                break;
              case 3:
                who.FarmerSprite.setCurrentFrame(188);
                this.Update(3, 0, who);
                break;
            }
            break;
          case FishingRod _:
            switch (who.FacingDirection)
            {
              case 0:
                ((FarmerSprite) who.Sprite).animateOnce(295, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
                this.Update(0, 0, who);
                break;
              case 1:
                ((FarmerSprite) who.Sprite).animateOnce(296, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
                this.Update(1, 0, who);
                break;
              case 2:
                ((FarmerSprite) who.Sprite).animateOnce(297, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
                this.Update(2, 0, who);
                break;
              case 3:
                ((FarmerSprite) who.Sprite).animateOnce(298, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
                this.Update(3, 0, who);
                break;
            }
            who.canReleaseTool = false;
            break;
          case MeleeWeapon _:
            ((MeleeWeapon) this).setFarmerAnimating(who);
            break;
          default:
            switch (who.FacingDirection)
            {
              case 0:
                who.FarmerSprite.setCurrentFrame(176);
                this.Update(0, 0, who);
                break;
              case 1:
                who.FarmerSprite.setCurrentFrame(168);
                this.Update(1, 0, who);
                break;
              case 2:
                who.FarmerSprite.setCurrentFrame(160);
                this.Update(2, 0, who);
                break;
              case 3:
                who.FarmerSprite.setCurrentFrame(184);
                this.Update(3, 0, who);
                break;
            }
            break;
        }
      }
      return false;
    }

    public virtual bool onRelease(GameLocation location, int x, int y, Farmer who) => false;

    public override bool canBeDropped() => false;

    public virtual bool canThisBeAttached(Object o)
    {
      if (this.attachments != null)
      {
        for (int index = 0; index < this.attachments.Length; ++index)
        {
          if (this.attachments[index] == null)
            return true;
        }
      }
      return false;
    }

    public virtual Object attach(Object o)
    {
      for (int index = 0; index < this.attachments.Length; ++index)
      {
        if (this.attachments[index] == null)
        {
          this.attachments[index] = o;
          Game1.playSound("button1");
          return (Object) null;
        }
      }
      return o;
    }

    public void colorTool(int level)
    {
      int targetColorIndex1 = 0;
      int startPixelIndex = 0;
      string str = ((IEnumerable<string>) this.BaseName.Split(' ')).Last<string>();
      if (!(str == "Hoe"))
      {
        if (!(str == "Pickaxe"))
        {
          if (!(str == "Axe"))
          {
            if (str == "Can")
            {
              targetColorIndex1 = 168713;
              startPixelIndex = 163840;
            }
          }
          else
          {
            targetColorIndex1 = 134681;
            startPixelIndex = 131072;
          }
        }
        else
        {
          targetColorIndex1 = 100749;
          startPixelIndex = 98304;
        }
      }
      else
      {
        targetColorIndex1 = 69129;
        startPixelIndex = 65536;
      }
      int r1 = 0;
      int g1 = 0;
      int b1 = 0;
      switch (level)
      {
        case 1:
          r1 = 198;
          g1 = 108;
          b1 = 43;
          break;
        case 2:
          r1 = 197;
          g1 = 226;
          b1 = 222;
          break;
        case 3:
          r1 = 248;
          g1 = (int) byte.MaxValue;
          b1 = 73;
          break;
        case 4:
          r1 = 144;
          g1 = 135;
          b1 = 181;
          break;
      }
      if (startPixelIndex <= 0 || level <= 0)
        return;
      if (this.BaseName.Contains("Can"))
        ColorChanger.swapColor(Game1.toolSpriteSheet, targetColorIndex1 + 36, r1 * 5 / 4, g1 * 5 / 4, b1 * 5 / 4, startPixelIndex, startPixelIndex + 32768);
      ColorChanger.swapColor(Game1.toolSpriteSheet, targetColorIndex1 + 8, r1, g1, b1, startPixelIndex, startPixelIndex + 32768);
      ColorChanger.swapColor(Game1.toolSpriteSheet, targetColorIndex1 + 4, r1 * 3 / 4, g1 * 3 / 4, b1 * 3 / 4, startPixelIndex, startPixelIndex + 32768);
      ColorChanger.swapColor(Game1.toolSpriteSheet, targetColorIndex1, r1 * 3 / 8, g1 * 3 / 8, b1 * 3 / 8, startPixelIndex, startPixelIndex + 32768);
    }

    public virtual void actionWhenClaimed()
    {
      if (!(this is GenericTool))
        return;
      switch ((int) (NetFieldBase<int, NetInt>) this.indexOfMenuItemView)
      {
        case 13:
        case 14:
        case 15:
        case 16:
          ++Game1.player.trashCanLevel;
          break;
      }
    }

    public override bool CanBuyItem(Farmer who)
    {
      if (Game1.player.toolBeingUpgraded.Value == null)
      {
        switch (this)
        {
          case Axe _:
          case Pickaxe _:
          case Hoe _:
          case WateringCan _:
label_3:
            return true;
          case GenericTool _:
            if ((int) (NetFieldBase<int, NetInt>) this.indexOfMenuItemView < 13 || (int) (NetFieldBase<int, NetInt>) this.indexOfMenuItemView > 16)
              break;
            goto label_3;
        }
      }
      return base.CanBuyItem(who);
    }

    public override bool actionWhenPurchased()
    {
      if (Game1.player.toolBeingUpgraded.Value == null)
      {
        switch (this)
        {
          case Axe _:
          case Pickaxe _:
          case Hoe _:
          case WateringCan _:
            Tool toolFromName = Game1.player.getToolFromName(this.BaseName);
            ++toolFromName.UpgradeLevel;
            Game1.player.removeItemFromInventory((Item) toolFromName);
            Game1.player.toolBeingUpgraded.Value = toolFromName;
            Game1.player.daysLeftForToolUpgrade.Value = 2;
            Game1.playSound("parry");
            Game1.exitActiveMenu();
            Game1.drawDialogue(Game1.getCharacterFromName("Clint"), Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
            return true;
          case GenericTool _:
            switch ((int) (NetFieldBase<int, NetInt>) this.indexOfMenuItemView)
            {
              case 13:
              case 14:
              case 15:
              case 16:
                Game1.player.toolBeingUpgraded.Value = this;
                Game1.player.daysLeftForToolUpgrade.Value = 2;
                Game1.playSound("parry");
                Game1.exitActiveMenu();
                Game1.drawDialogue(Game1.getCharacterFromName("Clint"), Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
                return true;
            }
            break;
        }
      }
      return base.actionWhenPurchased();
    }

    protected List<Vector2> tilesAffected(Vector2 tileLocation, int power, Farmer who)
    {
      ++power;
      List<Vector2> vector2List = new List<Vector2>();
      vector2List.Add(tileLocation);
      Vector2 vector2 = Vector2.Zero;
      if (who.FacingDirection == 0)
      {
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X, tileLocation.Y - 2f);
        }
        else
        {
          if (power >= 2)
          {
            vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
            vector2List.Add(tileLocation + new Vector2(0.0f, -2f));
          }
          if (power >= 3)
          {
            vector2List.Add(tileLocation + new Vector2(0.0f, -3f));
            vector2List.Add(tileLocation + new Vector2(0.0f, -4f));
          }
          if (power >= 4)
          {
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.Add(tileLocation + new Vector2(1f, -2f));
            vector2List.Add(tileLocation + new Vector2(1f, -1f));
            vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
            vector2List.Add(tileLocation + new Vector2(-1f, -2f));
            vector2List.Add(tileLocation + new Vector2(-1f, -1f));
            vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
          }
          if (power >= 5)
          {
            for (int index = vector2List.Count - 1; index >= 0; --index)
              vector2List.Add(vector2List[index] + new Vector2(0.0f, -3f));
          }
        }
      }
      else if (who.FacingDirection == 1)
      {
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X + 2f, tileLocation.Y);
        }
        else
        {
          if (power >= 2)
          {
            vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
            vector2List.Add(tileLocation + new Vector2(2f, 0.0f));
          }
          if (power >= 3)
          {
            vector2List.Add(tileLocation + new Vector2(3f, 0.0f));
            vector2List.Add(tileLocation + new Vector2(4f, 0.0f));
          }
          if (power >= 4)
          {
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
            vector2List.Add(tileLocation + new Vector2(1f, -1f));
            vector2List.Add(tileLocation + new Vector2(2f, -1f));
            vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
            vector2List.Add(tileLocation + new Vector2(1f, 1f));
            vector2List.Add(tileLocation + new Vector2(2f, 1f));
          }
          if (power >= 5)
          {
            for (int index = vector2List.Count - 1; index >= 0; --index)
              vector2List.Add(vector2List[index] + new Vector2(3f, 0.0f));
          }
        }
      }
      else if (who.FacingDirection == 2)
      {
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X, tileLocation.Y + 2f);
        }
        else
        {
          if (power >= 2)
          {
            vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
            vector2List.Add(tileLocation + new Vector2(0.0f, 2f));
          }
          if (power >= 3)
          {
            vector2List.Add(tileLocation + new Vector2(0.0f, 3f));
            vector2List.Add(tileLocation + new Vector2(0.0f, 4f));
          }
          if (power >= 4)
          {
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.Add(tileLocation + new Vector2(1f, 2f));
            vector2List.Add(tileLocation + new Vector2(1f, 1f));
            vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
            vector2List.Add(tileLocation + new Vector2(-1f, 2f));
            vector2List.Add(tileLocation + new Vector2(-1f, 1f));
            vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
          }
          if (power >= 5)
          {
            for (int index = vector2List.Count - 1; index >= 0; --index)
              vector2List.Add(vector2List[index] + new Vector2(0.0f, 3f));
          }
        }
      }
      else if (who.FacingDirection == 3)
      {
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X - 2f, tileLocation.Y);
        }
        else
        {
          if (power >= 2)
          {
            vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
            vector2List.Add(tileLocation + new Vector2(-2f, 0.0f));
          }
          if (power >= 3)
          {
            vector2List.Add(tileLocation + new Vector2(-3f, 0.0f));
            vector2List.Add(tileLocation + new Vector2(-4f, 0.0f));
          }
          if (power >= 4)
          {
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.RemoveAt(vector2List.Count - 1);
            vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
            vector2List.Add(tileLocation + new Vector2(-1f, -1f));
            vector2List.Add(tileLocation + new Vector2(-2f, -1f));
            vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
            vector2List.Add(tileLocation + new Vector2(-1f, 1f));
            vector2List.Add(tileLocation + new Vector2(-2f, 1f));
          }
          if (power >= 5)
          {
            for (int index = vector2List.Count - 1; index >= 0; --index)
              vector2List.Add(vector2List[index] + new Vector2(-3f, 0.0f));
          }
        }
      }
      if (power >= 6)
      {
        vector2List.Clear();
        for (int x = (int) vector2.X - 2; (double) x <= (double) vector2.X + 2.0; ++x)
        {
          for (int y = (int) vector2.Y - 2; (double) y <= (double) vector2.Y + 2.0; ++y)
            vector2List.Add(new Vector2((float) x, (float) y));
        }
      }
      return vector2List;
    }

    public virtual bool doesShowTileLocationMarker() => true;

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
      spriteBatch.Draw(Game1.toolSpriteSheet, location + new Vector2(32f, 32f), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, this.IndexOfMenuItemView)), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.stackable)
        return;
      Game1.drawWithBorder(((StardewValley.Tools.Stackable) this).NumberInStack.ToString() ?? "", Color.Black, Color.White, location + new Vector2(64f - Game1.dialogueFont.MeasureString(((StardewValley.Tools.Stackable) this).NumberInStack.ToString() ?? "").X, (float) (64.0 - (double) Game1.dialogueFont.MeasureString(((StardewValley.Tools.Stackable) this).NumberInStack.ToString() ?? "").Y * 3.0 / 4.0)), 0.0f, 0.5f, 1f);
    }

    public override bool isPlaceable() => false;

    public override int maximumStackSize() => (bool) (NetFieldBase<bool, NetBool>) this.stackable ? 99 : -1;

    public virtual void setNewTileIndexForUpgradeLevel()
    {
      switch (this)
      {
        case MeleeWeapon _:
          break;
        case MagnifyingGlass _:
          break;
        case MilkPail _:
          break;
        case Shears _:
          break;
        case Pan _:
          break;
        case Slingshot _:
          break;
        case Wand _:
          break;
        default:
          int num1 = 21;
          switch (this)
          {
            case FishingRod _:
              this.InitialParentTileIndex = 8 + (int) (NetFieldBase<int, NetInt>) this.upgradeLevel;
              this.CurrentParentTileIndex = this.InitialParentTileIndex;
              this.IndexOfMenuItemView = this.InitialParentTileIndex;
              return;
            case Axe _:
              num1 = 189;
              break;
            case Hoe _:
              num1 = 21;
              break;
            case Pickaxe _:
              num1 = 105;
              break;
            case WateringCan _:
              num1 = 273;
              break;
          }
          int num2 = num1 + (int) (NetFieldBase<int, NetInt>) this.upgradeLevel * 7;
          if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel > 2)
            num2 += 21;
          this.InitialParentTileIndex = num2;
          this.CurrentParentTileIndex = this.InitialParentTileIndex;
          this.IndexOfMenuItemView = this.InitialParentTileIndex + (this is WateringCan ? 2 : 5) + 21;
          break;
      }
    }

    public override int addToStack(Item stack)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.stackable)
        return stack.Stack;
      ((StardewValley.Tools.Stackable) this).NumberInStack += stack.Stack;
      if (((StardewValley.Tools.Stackable) this).NumberInStack <= 99)
        return 0;
      int stack1 = ((StardewValley.Tools.Stackable) this).NumberInStack - 99;
      ((StardewValley.Tools.Stackable) this).NumberInStack = 99;
      return stack1;
    }

    public override string getDescription() => Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());

    public virtual void ClearEnchantments()
    {
      for (int index = this.enchantments.Count - 1; index >= 0; --index)
        this.enchantments[index].UnapplyTo((Item) this);
      this.enchantments.Clear();
    }

    public virtual int GetMaxForges() => 0;

    public int GetSecondaryEnchantmentCount()
    {
      int enchantmentCount = 0;
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment != null && enchantment.IsSecondaryEnchantment())
          ++enchantmentCount;
      }
      return enchantmentCount;
    }

    public virtual bool CanAddEnchantment(BaseEnchantment enchantment)
    {
      if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
        return true;
      if (this.GetTotalForgeLevels() >= this.GetMaxForges() && !enchantment.IsSecondaryEnchantment() || enchantment == null)
        return false;
      foreach (BaseEnchantment enchantment1 in this.enchantments)
      {
        if (enchantment.GetType() == enchantment1.GetType())
          return enchantment1.GetMaximumLevel() < 0 || enchantment1.GetLevel() < enchantment1.GetMaximumLevel();
      }
      return true;
    }

    public virtual void CopyEnchantments(Tool source, Tool destination)
    {
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        destination.enchantments.Add(enchantment.GetOne());
        enchantment.GetOne().ApplyTo((Item) destination);
      }
      destination.previousEnchantments.Clear();
      destination.previousEnchantments.AddRange((IEnumerable<string>) source.previousEnchantments);
    }

    public int GetTotalForgeLevels(bool for_unforge = false)
    {
      int totalForgeLevels = 0;
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment is DiamondEnchantment)
        {
          if (for_unforge)
            return totalForgeLevels;
        }
        else if (enchantment.IsForge())
          totalForgeLevels += enchantment.GetLevel();
      }
      return totalForgeLevels;
    }

    public virtual bool AddEnchantment(BaseEnchantment enchantment)
    {
      if (enchantment == null)
        return false;
      if (this is MeleeWeapon && (enchantment.IsForge() || enchantment.IsSecondaryEnchantment()))
      {
        foreach (BaseEnchantment enchantment1 in this.enchantments)
        {
          if (enchantment.GetType() == enchantment1.GetType())
          {
            if (enchantment1.GetMaximumLevel() >= 0 && enchantment1.GetLevel() >= enchantment1.GetMaximumLevel())
              return false;
            enchantment1.SetLevel((Item) this, enchantment1.GetLevel() + 1);
            return true;
          }
        }
        this.enchantments.Add(enchantment);
        enchantment.ApplyTo((Item) this, this.lastUser);
        return true;
      }
      for (int index = this.enchantments.Count - 1; index >= 0; --index)
      {
        if (!this.enchantments[index].IsForge() && !this.enchantments[index].IsSecondaryEnchantment())
        {
          this.enchantments.ElementAt<BaseEnchantment>(index).UnapplyTo((Item) this);
          this.enchantments.RemoveAt(index);
        }
      }
      this.enchantments.Add(enchantment);
      enchantment.ApplyTo((Item) this, this.lastUser);
      return true;
    }

    public bool hasEnchantmentOfType<T>()
    {
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment is T)
          return true;
      }
      return false;
    }

    public virtual void RemoveEnchantment(BaseEnchantment enchantment)
    {
      if (enchantment == null)
        return;
      this.enchantments.Remove(enchantment);
      enchantment.UnapplyTo((Item) this, this.lastUser);
    }

    public override void actionWhenBeingHeld(Farmer who)
    {
      base.actionWhenBeingHeld(who);
      if (!who.IsLocalPlayer)
        return;
      foreach (BaseEnchantment enchantment in this.enchantments)
        enchantment.OnEquip(who);
    }

    public override void actionWhenStopBeingHeld(Farmer who)
    {
      base.actionWhenStopBeingHeld(who);
      if (who.UsingTool)
      {
        who.UsingTool = false;
        if (who.FarmerSprite.PauseForSingleAnimation)
          who.FarmerSprite.PauseForSingleAnimation = false;
      }
      if (!who.IsLocalPlayer)
        return;
      foreach (BaseEnchantment enchantment in this.enchantments)
        enchantment.OnUnequip(who);
    }

    public virtual bool CanUseOnStandingTile() => false;

    public virtual bool CanForge(Item item)
    {
      BaseEnchantment enchantmentFromItem = BaseEnchantment.GetEnchantmentFromItem((Item) this, item);
      return enchantmentFromItem != null && this.CanAddEnchantment(enchantmentFromItem);
    }

    public T GetEnchantmentOfType<T>() where T : BaseEnchantment
    {
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment.GetType() == typeof (T))
          return enchantment as T;
      }
      return default (T);
    }

    public int GetEnchantmentLevel<T>() where T : BaseEnchantment
    {
      int enchantmentLevel = 0;
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment.GetType() == typeof (T))
          enchantmentLevel += enchantment.GetLevel();
      }
      return enchantmentLevel;
    }

    public virtual bool Forge(Item item, bool count_towards_stats = false)
    {
      BaseEnchantment enchantmentFromItem = BaseEnchantment.GetEnchantmentFromItem((Item) this, item);
      if (enchantmentFromItem == null || !this.AddEnchantment(enchantmentFromItem))
        return false;
      if (enchantmentFromItem is DiamondEnchantment)
      {
        int num1 = this.GetMaxForges() - this.GetTotalForgeLevels();
        List<int> intList = new List<int>();
        if (!this.hasEnchantmentOfType<EmeraldEnchantment>())
          intList.Add(0);
        if (!this.hasEnchantmentOfType<AquamarineEnchantment>())
          intList.Add(1);
        if (!this.hasEnchantmentOfType<RubyEnchantment>())
          intList.Add(2);
        if (!this.hasEnchantmentOfType<AmethystEnchantment>())
          intList.Add(3);
        if (!this.hasEnchantmentOfType<TopazEnchantment>())
          intList.Add(4);
        if (!this.hasEnchantmentOfType<JadeEnchantment>())
          intList.Add(5);
        for (int index1 = 0; index1 < num1 && intList.Count != 0; ++index1)
        {
          int index2 = Game1.random.Next(intList.Count);
          int num2 = intList[index2];
          intList.RemoveAt(index2);
          switch (num2)
          {
            case 0:
              this.AddEnchantment((BaseEnchantment) new EmeraldEnchantment());
              break;
            case 1:
              this.AddEnchantment((BaseEnchantment) new AquamarineEnchantment());
              break;
            case 2:
              this.AddEnchantment((BaseEnchantment) new RubyEnchantment());
              break;
            case 3:
              this.AddEnchantment((BaseEnchantment) new AmethystEnchantment());
              break;
            case 4:
              this.AddEnchantment((BaseEnchantment) new TopazEnchantment());
              break;
            case 5:
              this.AddEnchantment((BaseEnchantment) new JadeEnchantment());
              break;
          }
        }
      }
      else if (enchantmentFromItem is GalaxySoulEnchantment && this is MeleeWeapon && (this as MeleeWeapon).isGalaxyWeapon() && (this as MeleeWeapon).GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3)
      {
        int initialParentTileIndex = (this as MeleeWeapon).InitialParentTileIndex;
        int newIndex = -1;
        switch (initialParentTileIndex)
        {
          case 4:
            newIndex = 62;
            break;
          case 23:
            newIndex = 64;
            break;
          case 29:
            newIndex = 63;
            break;
        }
        if (newIndex != -1)
        {
          (this as MeleeWeapon).transform(newIndex);
          if (count_towards_stats)
          {
            DelayedAction.playSoundAfterDelay("discoverMineral", 400);
            Game1.multiplayer.globalChatInfoMessage("InfinityWeapon", (string) (NetFieldBase<string, NetString>) Game1.player.name, this.DisplayName);
          }
        }
        GalaxySoulEnchantment enchantmentOfType = this.GetEnchantmentOfType<GalaxySoulEnchantment>();
        if (enchantmentOfType != null)
          this.RemoveEnchantment((BaseEnchantment) enchantmentOfType);
      }
      if (count_towards_stats && !enchantmentFromItem.IsForge())
      {
        this.previousEnchantments.Insert(0, enchantmentFromItem.GetName());
        while (this.previousEnchantments.Count > 2)
          this.previousEnchantments.RemoveAt(this.previousEnchantments.Count - 1);
        Game1.stats.incrementStat("timesEnchanted", 1);
      }
      return true;
    }
  }
}
