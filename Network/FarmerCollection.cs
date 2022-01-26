// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.FarmerCollection
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public class FarmerCollection : IEnumerable<Farmer>, IEnumerable
  {
    private GameLocation _locationFilter;

    public FarmerCollection(GameLocation locationFilter = null) => this._locationFilter = locationFilter;

    public int Count
    {
      get
      {
        int count = 0;
        foreach (Farmer farmer in this)
          ++count;
        return count;
      }
    }

    public bool Any()
    {
      using (FarmerCollection.Enumerator enumerator = this.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          Farmer current = enumerator.Current;
          return true;
        }
      }
      return false;
    }

    public bool Contains(Farmer farmer)
    {
      foreach (Farmer farmer1 in this)
      {
        if (farmer1 == farmer)
          return true;
      }
      return false;
    }

    public FarmerCollection.Enumerator GetEnumerator() => new FarmerCollection.Enumerator(this._locationFilter);

    IEnumerator<Farmer> IEnumerable<Farmer>.GetEnumerator() => (IEnumerator<Farmer>) new FarmerCollection.Enumerator(this._locationFilter);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new FarmerCollection.Enumerator(this._locationFilter);

    public struct Enumerator : IEnumerator<Farmer>, IEnumerator, IDisposable
    {
      private GameLocation _locationFilter;
      private Dictionary<long, NetRoot<Farmer>>.Enumerator _enumerator;
      private Farmer _player;
      private Farmer _current;
      private int _done;

      public Enumerator(GameLocation locationFilter)
      {
        this._locationFilter = locationFilter;
        this._player = Game1.player;
        this._enumerator = Game1.otherFarmers.Roots.GetEnumerator();
        this._current = (Farmer) null;
        this._done = 2;
      }

      public bool MoveNext()
      {
        if (this._done == 2)
        {
          this._done = 1;
          if (this._locationFilter == null || this._player.currentLocation != null && this._locationFilter.Equals(this._player.currentLocation))
          {
            this._current = this._player;
            return true;
          }
        }
        while (this._enumerator.MoveNext())
        {
          Farmer farmer = this._enumerator.Current.Value.Value;
          if (farmer != this._player && (this._locationFilter == null || farmer.currentLocation != null && this._locationFilter.Equals(farmer.currentLocation)))
          {
            this._current = farmer;
            return true;
          }
        }
        this._done = 0;
        this._current = (Farmer) null;
        return false;
      }

      public Farmer Current => this._current;

      public void Dispose()
      {
      }

      object IEnumerator.Current
      {
        get
        {
          if (this._done == 0)
            throw new InvalidOperationException();
          return (object) this._current;
        }
      }

      void IEnumerator.Reset()
      {
        this._player = Game1.player;
        this._enumerator = Game1.otherFarmers.Roots.GetEnumerator();
        this._current = (Farmer) null;
        this._done = 2;
      }
    }
  }
}
