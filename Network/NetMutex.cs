// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetMutex
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Network
{
  public class NetMutex : INetObject<NetFields>
  {
    public const long NoOwner = -1;
    private long prevOwner = -1;
    private readonly NetLong owner = new NetLong(-1L);
    private readonly NetEvent1Field<long, NetLong> lockRequest = new NetEvent1Field<long, NetLong>();
    private Action onLockAcquired;
    private Action onLockFailed;

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public NetMutex()
    {
      this.NetFields.AddFields((INetSerializable) this.owner, (INetSerializable) this.lockRequest);
      this.lockRequest.InterpolationWait = false;
      this.owner.InterpolationWait = false;
      this.lockRequest.onEvent += (AbstractNetEvent1<long>.Event) (playerId =>
      {
        if (!Game1.IsMasterGame || this.owner.Value != -1L && this.owner.Value != playerId)
          return;
        this.owner.Value = playerId;
        this.owner.MarkDirty();
      });
    }

    public void RequestLock(Action acquired = null, Action failed = null)
    {
      if (this.owner.Value == Game1.player.UniqueMultiplayerID)
      {
        if (acquired == null)
          return;
        acquired();
      }
      else if (this.owner.Value != -1L)
      {
        if (failed == null)
          return;
        failed();
      }
      else
      {
        this.lockRequest.Fire(Game1.player.UniqueMultiplayerID);
        this.onLockAcquired = acquired;
        this.onLockFailed = failed;
      }
    }

    public void ReleaseLock()
    {
      this.owner.Value = -1L;
      this.onLockFailed = (Action) null;
      this.onLockAcquired = (Action) null;
    }

    public bool IsLocked() => this.owner.Value != -1L;

    public bool IsLockHeld() => this.owner.Value == Game1.player.UniqueMultiplayerID;

    public void Update(GameLocation location) => this.Update(location.farmers);

    public void Update(FarmerCollection farmers)
    {
      this.lockRequest.Poll();
      if (this.owner.Value != this.prevOwner)
      {
        if (this.owner.Value == Game1.player.UniqueMultiplayerID && this.onLockAcquired != null)
          this.onLockAcquired();
        if (this.owner.Value != Game1.player.UniqueMultiplayerID && this.onLockFailed != null)
          this.onLockFailed();
        this.onLockAcquired = (Action) null;
        this.onLockFailed = (Action) null;
        this.prevOwner = this.owner.Value;
      }
      if (!Game1.IsMasterGame || this.owner.Value == -1L || farmers.FirstOrDefault<Farmer>((Func<Farmer, bool>) (f => f.UniqueMultiplayerID == this.owner.Value && f.locationBeforeForcedEvent.Value == null)) != null)
        return;
      this.ReleaseLock();
    }
  }
}
