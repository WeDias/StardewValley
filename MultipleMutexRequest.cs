// Decompiled with JetBrains decompiler
// Type: StardewValley.MultipleMutexRequest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class MultipleMutexRequest
  {
    protected int _reportedCount;
    protected List<NetMutex> _acquiredLocks;
    protected List<NetMutex> _mutexList;
    protected Action _onSuccess;
    protected Action _onFailure;

    public MultipleMutexRequest(
      List<NetMutex> mutexes,
      Action success_callback = null,
      Action failure_callback = null)
    {
      this._onSuccess = success_callback;
      this._onFailure = failure_callback;
      this._acquiredLocks = new List<NetMutex>();
      this._mutexList = new List<NetMutex>((IEnumerable<NetMutex>) mutexes);
      this._RequestMutexes();
    }

    public MultipleMutexRequest(
      NetMutex[] mutexes,
      Action success_callback = null,
      Action failure_callback = null)
    {
      this._onSuccess = success_callback;
      this._onFailure = failure_callback;
      this._acquiredLocks = new List<NetMutex>();
      this._mutexList = new List<NetMutex>((IEnumerable<NetMutex>) mutexes);
      this._RequestMutexes();
    }

    protected void _RequestMutexes()
    {
      if (this._mutexList == null)
      {
        if (this._onFailure == null)
          return;
        this._onFailure();
      }
      else if (this._mutexList.Count == 0)
      {
        if (this._onSuccess == null)
          return;
        this._onSuccess();
      }
      else
      {
        for (int index = 0; index < this._mutexList.Count; ++index)
        {
          if (this._mutexList[index].IsLocked())
          {
            if (this._onFailure == null)
              return;
            this._onFailure();
            return;
          }
        }
        for (int index = 0; index < this._mutexList.Count; ++index)
        {
          NetMutex mutex = this._mutexList[index];
          mutex.RequestLock((Action) (() => this._OnLockAcquired(mutex)), (Action) (() => this._OnLockFailed(mutex)));
        }
      }
    }

    protected void _OnLockAcquired(NetMutex mutex)
    {
      ++this._reportedCount;
      this._acquiredLocks.Add(mutex);
      if (this._reportedCount < this._mutexList.Count)
        return;
      this._Finalize();
    }

    protected void _OnLockFailed(NetMutex mutex)
    {
      ++this._reportedCount;
      if (this._reportedCount < this._mutexList.Count)
        return;
      this._Finalize();
    }

    protected void _Finalize()
    {
      if (this._acquiredLocks.Count < this._mutexList.Count)
      {
        this.ReleaseLocks();
        this._onFailure();
      }
      else
        this._onSuccess();
    }

    public void ReleaseLocks()
    {
      for (int index = 0; index < this._acquiredLocks.Count; ++index)
        this._acquiredLocks[index].ReleaseLock();
      this._acquiredLocks.Clear();
    }
  }
}
