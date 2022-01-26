// Decompiled with JetBrains decompiler
// Type: StardewValley.BaseEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  [XmlInclude(typeof (BaseEnchantment))]
  [XmlInclude(typeof (BaseWeaponEnchantment))]
  [XmlInclude(typeof (ArtfulEnchantment))]
  [XmlInclude(typeof (BugKillerEnchantment))]
  [XmlInclude(typeof (HaymakerEnchantment))]
  [XmlInclude(typeof (MagicEnchantment))]
  [XmlInclude(typeof (VampiricEnchantment))]
  [XmlInclude(typeof (CrusaderEnchantment))]
  [XmlInclude(typeof (ShearsEnchantment))]
  [XmlInclude(typeof (MilkPailEnchantment))]
  [XmlInclude(typeof (PanEnchantment))]
  [XmlInclude(typeof (WateringCanEnchantment))]
  [XmlInclude(typeof (AxeEnchantment))]
  [XmlInclude(typeof (HoeEnchantment))]
  [XmlInclude(typeof (PickaxeEnchantment))]
  [XmlInclude(typeof (SwiftToolEnchantment))]
  [XmlInclude(typeof (ReachingToolEnchantment))]
  [XmlInclude(typeof (BottomlessEnchantment))]
  [XmlInclude(typeof (ShavingEnchantment))]
  [XmlInclude(typeof (ArchaeologistEnchantment))]
  [XmlInclude(typeof (EfficientToolEnchantment))]
  [XmlInclude(typeof (PowerfulEnchantment))]
  [XmlInclude(typeof (GenerousEnchantment))]
  [XmlInclude(typeof (MasterEnchantment))]
  [XmlInclude(typeof (AutoHookEnchantment))]
  [XmlInclude(typeof (PreservingEnchantment))]
  [XmlInclude(typeof (AmethystEnchantment))]
  [XmlInclude(typeof (TopazEnchantment))]
  [XmlInclude(typeof (AquamarineEnchantment))]
  [XmlInclude(typeof (JadeEnchantment))]
  [XmlInclude(typeof (EmeraldEnchantment))]
  [XmlInclude(typeof (RubyEnchantment))]
  [XmlInclude(typeof (DiamondEnchantment))]
  [XmlInclude(typeof (GalaxySoulEnchantment))]
  public class BaseEnchantment : INetObject<NetFields>
  {
    [XmlIgnore]
    protected string _displayName;
    [XmlIgnore]
    protected bool _applied;
    [XmlIgnore]
    [InstancedStatic]
    public static bool hideEnchantmentName;
    protected static List<BaseEnchantment> _enchantments;
    protected readonly NetInt level = new NetInt(1);

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    [XmlElement("level")]
    public int Level
    {
      get => this.level.Value;
      set => this.level.Value = value;
    }

    public BaseEnchantment() => this.InitializeNetFields();

    public static BaseEnchantment GetEnchantmentFromItem(Item base_item, Item item)
    {
      if (base_item == null || base_item is MeleeWeapon && !(base_item as MeleeWeapon).isScythe())
      {
        if (base_item != null && base_item is MeleeWeapon && (base_item as MeleeWeapon).isGalaxyWeapon() && Utility.IsNormalObjectAtParentSheetIndex(item, 896))
          return (BaseEnchantment) new GalaxySoulEnchantment();
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 60))
          return (BaseEnchantment) new EmeraldEnchantment();
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 62))
          return (BaseEnchantment) new AquamarineEnchantment();
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 64))
          return (BaseEnchantment) new RubyEnchantment();
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 66))
          return (BaseEnchantment) new AmethystEnchantment();
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 68))
          return (BaseEnchantment) new TopazEnchantment();
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 70))
          return (BaseEnchantment) new JadeEnchantment();
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 72))
          return (BaseEnchantment) new DiamondEnchantment();
      }
      if (!Utility.IsNormalObjectAtParentSheetIndex(item, 74))
        return (BaseEnchantment) null;
      Random random = new Random((int) Game1.stats.getStat("timesEnchanted") + (int) Game1.uniqueIDForThisGame);
      return Utility.GetRandom<BaseEnchantment>(BaseEnchantment.GetAvailableEnchantmentsForItem(base_item as Tool), random);
    }

    public static List<BaseEnchantment> GetAvailableEnchantmentsForItem(Tool item)
    {
      List<BaseEnchantment> enchantmentsForItem = new List<BaseEnchantment>();
      if (item == null)
        return BaseEnchantment.GetAvailableEnchantments();
      List<BaseEnchantment> availableEnchantments = BaseEnchantment.GetAvailableEnchantments();
      HashSet<Type> typeSet = new HashSet<Type>();
      foreach (BaseEnchantment enchantment in item.enchantments)
        typeSet.Add(enchantment.GetType());
      foreach (BaseEnchantment baseEnchantment in availableEnchantments)
      {
        if (baseEnchantment.CanApplyTo((Item) item) && !typeSet.Contains(baseEnchantment.GetType()))
          enchantmentsForItem.Add(baseEnchantment);
      }
      foreach (string previousEnchantment in (NetList<string, NetString>) item.previousEnchantments)
      {
        if (enchantmentsForItem.Count > 1)
        {
          for (int index = 0; index < enchantmentsForItem.Count; ++index)
          {
            if (enchantmentsForItem[index].GetName() == previousEnchantment)
            {
              enchantmentsForItem.RemoveAt(index);
              break;
            }
          }
        }
        else
          break;
      }
      return enchantmentsForItem;
    }

    public static List<BaseEnchantment> GetAvailableEnchantments()
    {
      if (BaseEnchantment._enchantments == null)
      {
        BaseEnchantment._enchantments = new List<BaseEnchantment>();
        BaseEnchantment._enchantments.Add((BaseEnchantment) new ArtfulEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new BugKillerEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new VampiricEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new CrusaderEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new HaymakerEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new PowerfulEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new ReachingToolEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new ShavingEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new BottomlessEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new GenerousEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new ArchaeologistEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new MasterEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new AutoHookEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new PreservingEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new EfficientToolEnchantment());
        BaseEnchantment._enchantments.Add((BaseEnchantment) new SwiftToolEnchantment());
      }
      return BaseEnchantment._enchantments;
    }

    public virtual bool IsForge() => false;

    public virtual bool IsSecondaryEnchantment() => false;

    public virtual void InitializeNetFields() => this.NetFields.AddFields((INetSerializable) this.level);

    public void OnEquip(Farmer farmer)
    {
      if (this._applied)
        return;
      farmer.enchantments.Add(this);
      this._applied = true;
      this._OnEquip(farmer);
    }

    public void OnUnequip(Farmer farmer)
    {
      if (!this._applied)
        return;
      farmer.enchantments.Remove(this);
      this._applied = false;
      this._OnUnequip(farmer);
    }

    protected virtual void _OnEquip(Farmer who)
    {
    }

    protected virtual void _OnUnequip(Farmer who)
    {
    }

    public void OnCalculateDamage(
      Monster monster,
      GameLocation location,
      Farmer who,
      ref int amount)
    {
      this._OnDealDamage(monster, location, who, ref amount);
    }

    public void OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount) => this._OnDealDamage(monster, location, who, ref amount);

    protected virtual void _OnDealDamage(
      Monster monster,
      GameLocation location,
      Farmer who,
      ref int amount)
    {
    }

    public void OnMonsterSlay(Monster m, GameLocation location, Farmer who) => this._OnMonsterSlay(m, location, who);

    protected virtual void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
    }

    public void OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who) => this._OnCutWeed(tile_location, location, who);

    protected virtual void _OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who)
    {
    }

    public virtual BaseEnchantment GetOne()
    {
      BaseEnchantment instance = Activator.CreateInstance(this.GetType()) as BaseEnchantment;
      instance.level.Value = this.level.Value;
      return instance;
    }

    public int GetLevel() => this.level.Value;

    public void SetLevel(Item item, int new_level)
    {
      if (new_level < 1)
        new_level = 1;
      else if (this.GetMaximumLevel() >= 0 && new_level > this.GetMaximumLevel())
        new_level = this.GetMaximumLevel();
      if (this.level.Value == new_level)
        return;
      this.UnapplyTo(item);
      this.level.Value = new_level;
      this.ApplyTo(item);
    }

    public virtual int GetMaximumLevel() => -1;

    public void ApplyTo(Item item, Farmer farmer = null)
    {
      this._ApplyTo(item);
      if (!this.IsItemCurrentlyEquipped(item, farmer))
        return;
      this.OnEquip(farmer);
    }

    protected virtual void _ApplyTo(Item item)
    {
    }

    public bool IsItemCurrentlyEquipped(Item item, Farmer farmer) => farmer != null && this._IsCurrentlyEquipped(item, farmer);

    protected virtual bool _IsCurrentlyEquipped(Item item, Farmer farmer) => farmer.CurrentTool == item;

    public void UnapplyTo(Item item, Farmer farmer = null)
    {
      this._UnapplyTo(item);
      if (!this.IsItemCurrentlyEquipped(item, farmer))
        return;
      this.OnUnequip(farmer);
    }

    protected virtual void _UnapplyTo(Item item)
    {
    }

    public virtual bool CanApplyTo(Item item) => true;

    public string GetDisplayName()
    {
      if (this._displayName == null)
      {
        this._displayName = Game1.content.LoadStringReturnNullIfNotFound("Strings\\EnchantmentNames:" + this.GetName());
        if (this._displayName == null)
          this._displayName = this.GetName();
      }
      return this._displayName;
    }

    public virtual string GetName() => "Unknown Enchantment";

    public virtual bool ShouldBeDisplayed() => true;
  }
}
