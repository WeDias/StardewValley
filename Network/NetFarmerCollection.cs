// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetFarmerCollection
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public class NetFarmerCollection : 
    INetObject<NetFields>,
    ICollection<Farmer>,
    IEnumerable<Farmer>,
    IEnumerable
  {
    private List<Farmer> farmers = new List<Farmer>();
    private NetLongDictionary<bool, NetBool> uids = new NetLongDictionary<bool, NetBool>();

    public NetFields NetFields { get; } = new NetFields();

    public int Count => this.farmers.Count;

    public bool IsReadOnly => false;

    public event NetFarmerCollection.FarmerEvent FarmerAdded;

    public event NetFarmerCollection.FarmerEvent FarmerRemoved;

    public NetFarmerCollection()
    {
      this.NetFields.AddField((INetSerializable) this.uids);
      this.uids.OnValueAdded += (NetDictionary<long, bool, NetBool, SerializableDictionary<long, bool>, NetLongDictionary<bool, NetBool>>.ContentsChangeEvent) ((uid, _) =>
      {
        Farmer farmer = this.getFarmer(uid);
        if (farmer == null || this.farmers.Contains(farmer))
          return;
        this.farmers.Add(farmer);
        if (this.FarmerAdded == null)
          return;
        this.FarmerAdded(farmer);
      });
      this.uids.OnValueRemoved += (NetDictionary<long, bool, NetBool, SerializableDictionary<long, bool>, NetLongDictionary<bool, NetBool>>.ContentsChangeEvent) ((uid, _) =>
      {
        Farmer farmer = this.getFarmer(uid);
        if (farmer == null)
          return;
        this.farmers.Remove(farmer);
        if (this.FarmerRemoved == null)
          return;
        this.FarmerRemoved(farmer);
      });
    }

    private static bool playerIsOnline(long uid)
    {
      if (Game1.player.UniqueMultiplayerID == uid || (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && Game1.serverHost.Value.UniqueMultiplayerID == uid)
        return true;
      return Game1.otherFarmers.ContainsKey(uid) && !Game1.multiplayer.isDisconnecting(uid);
    }

    public bool RetainOnlinePlayers()
    {
      int num = this.uids.Count();
      if (num == 0)
        return false;
      this.uids.Filter((Func<KeyValuePair<long, bool>, bool>) (x => NetFarmerCollection.playerIsOnline(x.Key)));
      this.farmers.Clear();
      foreach (long key in this.uids.Keys)
      {
        Farmer farmer = this.getFarmer(key);
        if (farmer != null)
          this.farmers.Add(farmer);
      }
      return this.uids.Count() < num;
    }

    private Farmer getFarmer(long uid)
    {
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.UniqueMultiplayerID == uid)
          return onlineFarmer;
      }
      return (Farmer) null;
    }

    public void Add(Farmer item)
    {
      this.farmers.Add(item);
      if (this.uids.ContainsKey((long) item.uniqueMultiplayerID))
        return;
      this.uids.Add(item.UniqueMultiplayerID, true);
    }

    public void Clear()
    {
      this.farmers.Clear();
      this.uids.Clear();
    }

    public bool Contains(Farmer item) => this.farmers.Contains(item);

    public void CopyTo(Farmer[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException();
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException();
      if (this.Count - arrayIndex > array.Length)
        throw new ArgumentException();
      foreach (Farmer farmer in this)
        array[arrayIndex++] = farmer;
    }

    public bool Remove(Farmer item)
    {
      this.uids.Remove((long) item.uniqueMultiplayerID);
      return this.farmers.Remove(item);
    }

    public IEnumerator<Farmer> GetEnumerator() => (IEnumerator<Farmer>) this.farmers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public delegate void FarmerEvent(Farmer f);
  }
}
