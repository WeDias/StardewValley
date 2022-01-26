// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.SynchronizedShopStock
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewValley.Util
{
  public class SynchronizedShopStock : INetObject<NetFields>
  {
    private readonly NetIntDictionary<int, NetInt> lastDayUpdated = new NetIntDictionary<int, NetInt>();
    private readonly NetStringDictionary<int, NetInt> sharedKrobusStock = new NetStringDictionary<int, NetInt>();
    private readonly NetStringDictionary<int, NetInt> sharedSandyStock = new NetStringDictionary<int, NetInt>();
    private readonly NetStringDictionary<int, NetInt> sharedTravelingMerchantStock = new NetStringDictionary<int, NetInt>();
    private readonly NetStringDictionary<int, NetInt> sharedSaloonStock = new NetStringDictionary<int, NetInt>();

    public NetFields NetFields { get; } = new NetFields();

    public SynchronizedShopStock() => this.initNetFields();

    private void initNetFields() => this.NetFields.AddFields((INetSerializable) this.lastDayUpdated, (INetSerializable) this.sharedKrobusStock, (INetSerializable) this.sharedSandyStock, (INetSerializable) this.sharedTravelingMerchantStock, (INetSerializable) this.sharedSaloonStock);

    private NetStringDictionary<int, NetInt> getSharedStock(
      SynchronizedShopStock.SynchedShop shop)
    {
      switch (shop)
      {
        case SynchronizedShopStock.SynchedShop.Krobus:
          return this.sharedKrobusStock;
        case SynchronizedShopStock.SynchedShop.TravelingMerchant:
          return this.sharedTravelingMerchantStock;
        case SynchronizedShopStock.SynchedShop.Sandy:
          return this.sharedSandyStock;
        case SynchronizedShopStock.SynchedShop.Saloon:
          return this.sharedSaloonStock;
        default:
          Console.WriteLine("Tried to get shared stock for invalid shop " + shop.ToString());
          return (NetStringDictionary<int, NetInt>) null;
      }
    }

    private int getLastDayUpdated(SynchronizedShopStock.SynchedShop shop)
    {
      if (!this.lastDayUpdated.ContainsKey((int) shop))
        this.lastDayUpdated[(int) shop] = -1;
      return this.lastDayUpdated[(int) shop];
    }

    private int setLastDayUpdated(SynchronizedShopStock.SynchedShop shop, int value)
    {
      if (!this.lastDayUpdated.ContainsKey((int) shop))
        this.lastDayUpdated[(int) shop] = 0;
      return this.lastDayUpdated[(int) shop] = value;
    }

    public void OnItemPurchased(SynchronizedShopStock.SynchedShop shop, ISalable item, int amount)
    {
      NetStringDictionary<int, NetInt> sharedStock = this.getSharedStock(shop);
      string descriptionFromItem = Utility.getStandardDescriptionFromItem(item as Item, 1);
      if (!sharedStock.ContainsKey(descriptionFromItem) || sharedStock[descriptionFromItem] == int.MaxValue || item is StardewValley.Object && (item as StardewValley.Object).IsRecipe)
        return;
      sharedStock[descriptionFromItem] -= amount;
    }

    public void UpdateLocalStockWithSyncedQuanitities(
      SynchronizedShopStock.SynchedShop shop,
      Dictionary<ISalable, int[]> localStock,
      Dictionary<string, Func<bool>> conditionalItemFilters = null)
    {
      List<Item> objList = new List<Item>();
      NetStringDictionary<int, NetInt> sharedStock = this.getSharedStock(shop);
      if (this.getLastDayUpdated(shop) != Game1.Date.TotalDays)
      {
        this.setLastDayUpdated(shop, Game1.Date.TotalDays);
        sharedStock.Clear();
        foreach (Item key in localStock.Keys)
        {
          string descriptionFromItem = Utility.getStandardDescriptionFromItem(key, 1);
          sharedStock.Add(descriptionFromItem, localStock[(ISalable) key][1]);
          if (sharedStock[descriptionFromItem] != int.MaxValue)
            key.Stack = sharedStock[descriptionFromItem];
        }
      }
      else
      {
        objList.Clear();
        foreach (Item key in localStock.Keys)
        {
          string descriptionFromItem = Utility.getStandardDescriptionFromItem(key, 1);
          if (sharedStock.ContainsKey(descriptionFromItem) && sharedStock[descriptionFromItem] > 0)
          {
            localStock[(ISalable) key][1] = sharedStock[descriptionFromItem];
            if (sharedStock[descriptionFromItem] != int.MaxValue)
              key.Stack = sharedStock[descriptionFromItem];
          }
          else
            objList.Add(key);
        }
        foreach (Item key in objList)
          localStock.Remove((ISalable) key);
      }
      objList.Clear();
      if (conditionalItemFilters == null)
        return;
      foreach (Item key in localStock.Keys)
      {
        string descriptionFromItem = Utility.getStandardDescriptionFromItem(key, 1);
        if (conditionalItemFilters.ContainsKey(descriptionFromItem) && !conditionalItemFilters[descriptionFromItem]())
          objList.Add(key);
      }
      foreach (Item key in objList)
        localStock.Remove((ISalable) key);
    }

    public enum SynchedShop
    {
      Krobus,
      TravelingMerchant,
      Sandy,
      Saloon,
    }
  }
}
