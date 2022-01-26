// Decompiled with JetBrains decompiler
// Type: StardewValley.DisposableList`1
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Collections.Generic;

namespace StardewValley
{
  public struct DisposableList<T>
  {
    private readonly ListPool<T> _pool;
    private readonly List<T> _list;

    public DisposableList(List<T> list, ListPool<T> pool)
    {
      this._list = list;
      this._pool = pool;
    }

    public DisposableList<T>.Enumerator GetEnumerator() => new DisposableList<T>.Enumerator(this);

    public struct Enumerator : IDisposable
    {
      private readonly DisposableList<T> _parent;
      private int _index;

      public Enumerator(DisposableList<T> parent)
      {
        this._parent = parent;
        this._index = 0;
      }

      public T Current
      {
        get
        {
          if (this._parent._list == null || this._index == 0)
            throw new InvalidOperationException();
          return this._parent._list[this._index - 1];
        }
      }

      public bool MoveNext()
      {
        ++this._index;
        return this._parent._list != null && this._parent._list.Count >= this._index;
      }

      public void Reset() => this._index = 0;

      public void Dispose()
      {
        lock (this._parent._pool)
          this._parent._pool.Return(this._parent._list);
      }
    }
  }
}
