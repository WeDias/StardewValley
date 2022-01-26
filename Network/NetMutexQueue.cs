// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetMutexQueue`1
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Network
{
  public class NetMutexQueue<T> : INetObject<NetFields>
  {
    private readonly NetLongDictionary<bool, NetBool> requests = new NetLongDictionary<bool, NetBool>();
    private readonly NetLong currentOwner = new NetLong();
    private readonly List<T> localJobs = new List<T>();
    [XmlIgnore]
    public Action<T> Processor = (Action<T>) (x => { });

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public NetMutexQueue()
    {
      this.NetFields.AddFields((INetSerializable) this.requests, (INetSerializable) this.currentOwner);
      this.requests.InterpolationWait = false;
      this.currentOwner.InterpolationWait = false;
    }

    public void Add(T job) => this.localJobs.Add(job);

    public bool Contains(T job) => this.localJobs.Contains(job);

    public void Clear() => this.localJobs.Clear();

    public void Update(GameLocation location)
    {
      FarmerCollection farmers = location.farmers;
      if (farmers.Contains(Game1.player) && this.localJobs.Count > 0)
        this.requests[Game1.player.UniqueMultiplayerID] = true;
      else
        this.requests.Remove(Game1.player.UniqueMultiplayerID);
      if (Game1.IsMasterGame)
      {
        this.requests.Filter((Func<KeyValuePair<long, bool>, bool>) (kv => farmers.FirstOrDefault<Farmer>((Func<Farmer, bool>) (f => f.UniqueMultiplayerID == kv.Key)) != null));
        if (!this.requests.ContainsKey((long) this.currentOwner))
          this.currentOwner.Value = -1L;
      }
      if ((long) this.currentOwner == Game1.player.UniqueMultiplayerID)
      {
        foreach (T localJob in this.localJobs)
          this.Processor(localJob);
        this.localJobs.Clear();
        this.requests.Remove(Game1.player.UniqueMultiplayerID);
        this.currentOwner.Value = -1L;
      }
      if (!Game1.IsMasterGame || (long) this.currentOwner != -1L || this.requests.Count() <= 0)
        return;
      this.currentOwner.Value = this.requests.Keys.ElementAt<long>(Game1.random.Next(this.requests.Count()));
    }
  }
}
