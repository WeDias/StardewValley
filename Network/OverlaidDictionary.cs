// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.OverlaidDictionary
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public sealed class OverlaidDictionary : 
    IEnumerable<SerializableDictionary<Vector2, StardewValley.Object>>,
    IEnumerable
  {
    private NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>> baseDict;
    private Dictionary<Vector2, StardewValley.Object> overlayDict;

    public StardewValley.Object this[Vector2 key]
    {
      get => this.overlayDict.ContainsKey(key) ? this.overlayDict[key] : this.baseDict[key];
      set => this.baseDict[key] = value;
    }

    public OverlaidDictionary.KeysCollection Keys => new OverlaidDictionary.KeysCollection(this);

    public OverlaidDictionary.ValuesCollection Values => new OverlaidDictionary.ValuesCollection(this);

    public OverlaidDictionary.PairsCollection Pairs => new OverlaidDictionary.PairsCollection(this);

    public void SetEqualityComparer(
      IEqualityComparer<Vector2> comparer,
      ref NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>> base_dict,
      ref Dictionary<Vector2, StardewValley.Object> overlay_dict)
    {
      this.baseDict.SetEqualityComparer(comparer);
      this.overlayDict = new Dictionary<Vector2, StardewValley.Object>((IDictionary<Vector2, StardewValley.Object>) this.overlayDict);
      base_dict = this.baseDict;
      overlay_dict = this.overlayDict;
    }

    public OverlaidDictionary(
      NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>> baseDict,
      Dictionary<Vector2, StardewValley.Object> overlayDict)
    {
      this.baseDict = baseDict;
      this.overlayDict = overlayDict;
    }

    public int Count() => this.Keys.Count();

    public void Add(Vector2 key, StardewValley.Object value) => this.baseDict.Add(key, value);

    public void Clear()
    {
      this.baseDict.Clear();
      this.overlayDict.Clear();
    }

    public bool ContainsKey(Vector2 key) => this.overlayDict.ContainsKey(key) || this.baseDict.ContainsKey(key);

    public bool Remove(Vector2 key) => this.overlayDict.ContainsKey(key) ? this.overlayDict.Remove(key) : this.baseDict.Remove(key);

    public bool TryGetValue(Vector2 key, out StardewValley.Object value) => this.overlayDict.TryGetValue(key, out value) || this.baseDict.TryGetValue(key, out value);

    public IEnumerator<SerializableDictionary<Vector2, StardewValley.Object>> GetEnumerator() => this.baseDict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.baseDict.GetEnumerator();

    public void Add(SerializableDictionary<Vector2, StardewValley.Object> dict)
    {
      foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in (Dictionary<Vector2, StardewValley.Object>) dict)
        this.baseDict.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public struct ValuesCollection : IEnumerable<StardewValley.Object>, IEnumerable
    {
      private OverlaidDictionary _dict;

      public ValuesCollection(OverlaidDictionary dict) => this._dict = dict;

      public OverlaidDictionary.ValuesCollection.Enumerator GetEnumerator() => new OverlaidDictionary.ValuesCollection.Enumerator(this._dict);

      IEnumerator<StardewValley.Object> IEnumerable<StardewValley.Object>.GetEnumerator() => (IEnumerator<StardewValley.Object>) new OverlaidDictionary.ValuesCollection.Enumerator(this._dict);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new OverlaidDictionary.ValuesCollection.Enumerator(this._dict);

      public struct Enumerator : IEnumerator<StardewValley.Object>, IEnumerator, IDisposable
      {
        private readonly OverlaidDictionary _dict;
        private NetDictionary<Vector2, StardewValley.Object, NetRef<StardewValley.Object>, SerializableDictionary<Vector2, StardewValley.Object>, NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>>>.ValuesCollection.Enumerator _base;
        private Dictionary<Vector2, StardewValley.Object>.Enumerator _overlay;
        private StardewValley.Object _current;
        private bool _done;

        public Enumerator(OverlaidDictionary dict)
        {
          this._dict = dict;
          this._base = this._dict.baseDict.Values.GetEnumerator();
          this._overlay = this._dict.overlayDict.GetEnumerator();
          this._current = (StardewValley.Object) null;
          this._done = false;
        }

        public bool MoveNext()
        {
          if (this._base.MoveNext())
          {
            this._current = this._base.Current;
            return true;
          }
          if (this._overlay.MoveNext())
          {
            this._current = this._overlay.Current.Value;
            return true;
          }
          this._done = true;
          this._current = (StardewValley.Object) null;
          return false;
        }

        public StardewValley.Object Current => this._current;

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
          get
          {
            if (this._done)
              throw new InvalidOperationException();
            return (object) this._current;
          }
        }

        void IEnumerator.Reset()
        {
          this._base = this._dict.baseDict.Values.GetEnumerator();
          this._overlay = this._dict.overlayDict.GetEnumerator();
          this._current = (StardewValley.Object) null;
          this._done = false;
        }
      }
    }

    public struct KeysCollection : IEnumerable<Vector2>, IEnumerable
    {
      private OverlaidDictionary _dict;

      public KeysCollection(OverlaidDictionary dict) => this._dict = dict;

      public int Count()
      {
        int num = 0;
        foreach (Vector2 vector2 in this)
          ++num;
        return num;
      }

      public OverlaidDictionary.KeysCollection.Enumerator GetEnumerator() => new OverlaidDictionary.KeysCollection.Enumerator(this._dict);

      IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator() => (IEnumerator<Vector2>) new OverlaidDictionary.KeysCollection.Enumerator(this._dict);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new OverlaidDictionary.KeysCollection.Enumerator(this._dict);

      public struct Enumerator : IEnumerator<Vector2>, IEnumerator, IDisposable
      {
        private readonly OverlaidDictionary _dict;
        private NetDictionary<Vector2, StardewValley.Object, NetRef<StardewValley.Object>, SerializableDictionary<Vector2, StardewValley.Object>, NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>>>.KeysCollection.Enumerator _base;
        private Dictionary<Vector2, StardewValley.Object>.Enumerator _overlay;
        private Vector2 _current;
        private bool _done;

        public Enumerator(OverlaidDictionary dict)
        {
          this._dict = dict;
          this._base = this._dict.baseDict.Keys.GetEnumerator();
          this._overlay = this._dict.overlayDict.GetEnumerator();
          this._current = Vector2.Zero;
          this._done = false;
        }

        public bool MoveNext()
        {
          if (this._base.MoveNext())
          {
            this._current = this._base.Current;
            return true;
          }
          while (this._overlay.MoveNext())
          {
            if (!this._dict.baseDict.ContainsKey(this._overlay.Current.Key))
            {
              this._current = this._overlay.Current.Key;
              return true;
            }
          }
          this._done = true;
          this._current = Vector2.Zero;
          return false;
        }

        public Vector2 Current => this._current;

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
          get
          {
            if (this._done)
              throw new InvalidOperationException();
            return (object) this._current;
          }
        }

        void IEnumerator.Reset()
        {
          this._base = this._dict.baseDict.Keys.GetEnumerator();
          this._overlay = this._dict.overlayDict.GetEnumerator();
          this._current = Vector2.Zero;
          this._done = false;
        }
      }
    }

    public struct PairsCollection : IEnumerable<KeyValuePair<Vector2, StardewValley.Object>>, IEnumerable
    {
      private OverlaidDictionary _dict;

      public PairsCollection(OverlaidDictionary dict) => this._dict = dict;

      public KeyValuePair<Vector2, StardewValley.Object> ElementAt(
        int index)
      {
        int num = 0;
        foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in this)
        {
          if (num == index)
            return keyValuePair;
          ++num;
        }
        throw new ArgumentOutOfRangeException();
      }

      public OverlaidDictionary.PairsCollection.Enumerator GetEnumerator() => new OverlaidDictionary.PairsCollection.Enumerator(this._dict);

      IEnumerator<KeyValuePair<Vector2, StardewValley.Object>> IEnumerable<KeyValuePair<Vector2, StardewValley.Object>>.GetEnumerator() => (IEnumerator<KeyValuePair<Vector2, StardewValley.Object>>) new OverlaidDictionary.PairsCollection.Enumerator(this._dict);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new OverlaidDictionary.PairsCollection.Enumerator(this._dict);

      public struct Enumerator : IEnumerator<KeyValuePair<Vector2, StardewValley.Object>>, IEnumerator, IDisposable
      {
        private readonly OverlaidDictionary _dict;
        private NetDictionary<Vector2, StardewValley.Object, NetRef<StardewValley.Object>, SerializableDictionary<Vector2, StardewValley.Object>, NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>>>.PairsCollection.Enumerator _base;
        private Dictionary<Vector2, StardewValley.Object>.Enumerator _overlay;
        private KeyValuePair<Vector2, StardewValley.Object> _current;
        private bool _done;

        public Enumerator(OverlaidDictionary dict)
        {
          this._dict = dict;
          this._base = this._dict.baseDict.Pairs.GetEnumerator();
          this._overlay = this._dict.overlayDict.GetEnumerator();
          this._current = new KeyValuePair<Vector2, StardewValley.Object>();
          this._done = false;
        }

        public bool MoveNext()
        {
          while (this._base.MoveNext())
          {
            KeyValuePair<Vector2, StardewValley.Object> current = this._base.Current;
            if (!this._dict.overlayDict.ContainsKey(current.Key))
            {
              this._current = new KeyValuePair<Vector2, StardewValley.Object>(current.Key, current.Value);
              return true;
            }
          }
          if (this._overlay.MoveNext())
          {
            KeyValuePair<Vector2, StardewValley.Object> current = this._overlay.Current;
            this._current = new KeyValuePair<Vector2, StardewValley.Object>(current.Key, current.Value);
            return true;
          }
          this._done = true;
          this._current = new KeyValuePair<Vector2, StardewValley.Object>();
          return false;
        }

        public KeyValuePair<Vector2, StardewValley.Object> Current => this._current;

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
          get
          {
            if (this._done)
              throw new InvalidOperationException();
            return (object) this._current;
          }
        }

        void IEnumerator.Reset()
        {
          this._base = this._dict.baseDict.Pairs.GetEnumerator();
          this._overlay = this._dict.overlayDict.GetEnumerator();
          this._current = new KeyValuePair<Vector2, StardewValley.Object>();
          this._done = false;
        }
      }
    }
  }
}
