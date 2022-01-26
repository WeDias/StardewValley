// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Bimap`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public class Bimap<L, R> : IEnumerable<KeyValuePair<L, R>>, IEnumerable
  {
    private Dictionary<L, R> leftToRight = new Dictionary<L, R>();
    private Dictionary<R, L> rightToLeft = new Dictionary<R, L>();

    public R this[L l]
    {
      get => this.leftToRight[l];
      set
      {
        if (this.leftToRight.ContainsKey(l))
          this.rightToLeft.Remove(this.leftToRight[l]);
        if (this.rightToLeft.ContainsKey(value))
          this.leftToRight.Remove(this.rightToLeft[value]);
        this.leftToRight[l] = value;
        this.rightToLeft[value] = l;
      }
    }

    public L this[R r]
    {
      get => this.rightToLeft[r];
      set
      {
        if (this.rightToLeft.ContainsKey(r))
          this.leftToRight.Remove(this.rightToLeft[r]);
        if (this.leftToRight.ContainsKey(value))
          this.rightToLeft.Remove(this.leftToRight[value]);
        this.rightToLeft[r] = value;
        this.leftToRight[value] = r;
      }
    }

    public ICollection<L> LeftValues => (ICollection<L>) this.leftToRight.Keys;

    public ICollection<R> RightValues => (ICollection<R>) this.rightToLeft.Keys;

    public int Count => this.rightToLeft.Count;

    public void Clear()
    {
      this.leftToRight.Clear();
      this.rightToLeft.Clear();
    }

    public void Add(L l, R r)
    {
      if (this.leftToRight.ContainsKey(l) || this.rightToLeft.ContainsKey(r))
        throw new ArgumentException();
      this.leftToRight.Add(l, r);
      this.rightToLeft.Add(r, l);
    }

    public bool ContainsLeft(L l) => this.leftToRight.ContainsKey(l);

    public bool ContainsRight(R r) => this.rightToLeft.ContainsKey(r);

    public void RemoveLeft(L l)
    {
      if (this.leftToRight.ContainsKey(l))
        this.rightToLeft.Remove(this.leftToRight[l]);
      this.leftToRight.Remove(l);
    }

    public void RemoveRight(R r)
    {
      if (this.rightToLeft.ContainsKey(r))
        this.leftToRight.Remove(this.rightToLeft[r]);
      this.rightToLeft.Remove(r);
    }

    public L GetLeft(R r) => this.rightToLeft[r];

    public R GetRight(L l) => this.leftToRight[l];

    public IEnumerator<KeyValuePair<L, R>> GetEnumerator() => (IEnumerator<KeyValuePair<L, R>>) this.leftToRight.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
