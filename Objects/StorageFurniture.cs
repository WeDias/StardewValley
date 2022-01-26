// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.StorageFurniture
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class StorageFurniture : Furniture
  {
    [XmlElement("heldItems")]
    public readonly NetObjectList<Item> heldItems = new NetObjectList<Item>();
    [XmlIgnore]
    public readonly NetMutex mutex = new NetMutex();

    public StorageFurniture()
    {
    }

    public StorageFurniture(int which, Vector2 tile, int initialRotations)
      : base(which, tile, initialRotations)
    {
    }

    public StorageFurniture(int which, Vector2 tile)
      : base(which, tile)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.heldItems, (INetSerializable) this.mutex.NetFields);
    }

    public override bool canBeRemoved(Farmer who) => !this.mutex.IsLocked() && base.canBeRemoved(who);

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return true;
      this.mutex.RequestLock((Action) (() => this.ShowMenu()));
      return true;
    }

    public virtual void ShowMenu() => this.ShowShopMenu();

    public virtual void ShowChestMenu()
    {
      ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) this.heldItems, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.GrabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.GrabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: ((Item) this), context: ((object) this));
      itemGrabMenu.behaviorBeforeCleanup = (Action<IClickableMenu>) (menu =>
      {
        this.mutex.ReleaseLock();
        this.OnMenuClose();
      });
      Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
      Game1.playSound("dwop");
    }

    public virtual void GrabItemFromInventory(Item item, Farmer who)
    {
      if (item.Stack == 0)
        item.Stack = 1;
      Item obj = this.AddItem(item);
      if (obj == null)
        who.removeItemFromInventory(item);
      else
        obj = who.addItemToInventory(obj);
      this.ClearNulls();
      int id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
      this.ShowChestMenu();
      (Game1.activeClickableMenu as ItemGrabMenu).heldItem = obj;
      if (id == -1)
        return;
      Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
      Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
    }

    public virtual bool HighlightItems(Item item) => InventoryMenu.highlightAllItems(item);

    public virtual void GrabItemFromChest(Item item, Farmer who)
    {
      if (!who.couldInventoryAcceptThisItem(item))
        return;
      this.heldItems.Remove(item);
      this.ClearNulls();
      this.ShowChestMenu();
    }

    public virtual void ClearNulls()
    {
      for (int index = this.heldItems.Count - 1; index >= 0; --index)
      {
        if (this.heldItems[index] == null)
          this.heldItems.RemoveAt(index);
      }
    }

    public virtual Item AddItem(Item item)
    {
      item.resetState();
      this.ClearNulls();
      for (int index = 0; index < this.heldItems.Count; ++index)
      {
        if (this.heldItems[index] != null && this.heldItems[index].canStackWith((ISalable) item))
        {
          item.Stack = this.heldItems[index].addToStack(item);
          if (item.Stack <= 0)
            return (Item) null;
        }
      }
      if (this.heldItems.Count >= 36)
        return item;
      this.heldItems.Add(item);
      return (Item) null;
    }

    public virtual void ShowShopMenu()
    {
      List<Item> list = this.heldItems.ToList<Item>();
      list.Sort(new Comparison<Item>(this.SortItems));
      Dictionary<ISalable, int[]> itemPriceAndStock = new Dictionary<ISalable, int[]>();
      foreach (Item key in list)
        itemPriceAndStock[(ISalable) key] = new int[2]
        {
          0,
          1
        };
      ShopMenu shopMenu = new ShopMenu(itemPriceAndStock, on_purchase: new Func<ISalable, Farmer, int, bool>(this.onDresserItemWithdrawn), on_sell: new Func<ISalable, bool>(this.onDresserItemDeposited), context: this.GetShopMenuContext());
      shopMenu.source = (object) this;
      shopMenu.behaviorBeforeCleanup = (Action<IClickableMenu>) (menu =>
      {
        this.mutex.ReleaseLock();
        this.OnMenuClose();
      });
      Game1.activeClickableMenu = (IClickableMenu) shopMenu;
    }

    public virtual void OnMenuClose()
    {
    }

    public virtual string GetShopMenuContext() => "Dresser";

    public override bool canBeTrashed() => this.heldItems.Count <= 0 && base.canBeTrashed();

    public override void DayUpdate(GameLocation location)
    {
      base.DayUpdate(location);
      this.mutex.ReleaseLock();
    }

    public override Item getOne()
    {
      StorageFurniture one = new StorageFurniture((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      one.drawPosition.Value = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition;
      one.defaultBoundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.defaultBoundingBox;
      one.boundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;
      one.currentRotation.Value = (int) (NetFieldBase<int, NetInt>) this.currentRotation - 1;
      one.isOn.Value = false;
      one.rotations.Value = (int) (NetFieldBase<int, NetInt>) this.rotations;
      one.rotate();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public virtual int SortItems(Item a, Item b)
    {
      if (a.Category != b.Category)
        return a.Category.CompareTo(b.Category);
      switch (a)
      {
        case Clothing _ when b is Clothing && (a as Clothing).clothesType.Value != (b as Clothing).clothesType.Value:
          return (a as Clothing).clothesType.Value.CompareTo((b as Clothing).clothesType.Value);
        case Hat _ when b is Hat:
          return (a as Hat).which.Value.CompareTo((b as Hat).which.Value);
        default:
          return a.ParentSheetIndex.CompareTo(b.ParentSheetIndex);
      }
    }

    public virtual bool onDresserItemWithdrawn(ISalable salable, Farmer who, int amount)
    {
      if (salable is Item)
        this.heldItems.Remove(salable as Item);
      return false;
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      this.mutex.Update(environment);
      base.updateWhenCurrentLocation(time, environment);
    }

    public virtual bool onDresserItemDeposited(ISalable deposited_salable)
    {
      if (deposited_salable is Item)
      {
        this.heldItems.Add(deposited_salable as Item);
        if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu)
        {
          Dictionary<ISalable, int[]> new_stock = new Dictionary<ISalable, int[]>();
          List<Item> list = this.heldItems.ToList<Item>();
          list.Sort(new Comparison<Item>(this.SortItems));
          foreach (Item key in list)
            new_stock[(ISalable) key] = new int[2]
            {
              0,
              1
            };
          (Game1.activeClickableMenu as ShopMenu).setItemPriceAndStock(new_stock);
          Game1.playSound("dwop");
          return true;
        }
      }
      return false;
    }
  }
}
