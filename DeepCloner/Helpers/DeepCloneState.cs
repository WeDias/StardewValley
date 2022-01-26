// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.DeepCloneState
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Force.DeepCloner.Helpers
{
  internal class DeepCloneState
  {
    private DeepCloneState.MiniDictionary _loops;
    private readonly object[] _baseFromTo = new object[6];
    private int _idx;

    public object GetKnownRef(object from)
    {
      object[] baseFromTo = this._baseFromTo;
      if (from == baseFromTo[0])
        return baseFromTo[3];
      if (from == baseFromTo[1])
        return baseFromTo[4];
      if (from == baseFromTo[2])
        return baseFromTo[5];
      return this._loops == null ? (object) null : this._loops.FindEntry(from);
    }

    public void AddKnownRef(object from, object to)
    {
      if (this._idx < 3)
      {
        this._baseFromTo[this._idx] = from;
        this._baseFromTo[this._idx + 3] = to;
        ++this._idx;
      }
      else
      {
        if (this._loops == null)
          this._loops = new DeepCloneState.MiniDictionary();
        this._loops.Insert(from, to);
      }
    }

    private class CustomEqualityComparer : IEqualityComparer<object>, IEqualityComparer
    {
      bool IEqualityComparer<object>.Equals(object x, object y) => x == y;

      bool IEqualityComparer.Equals(object x, object y) => x == y;

      public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }

    private class MiniDictionary
    {
      private int[] _buckets;
      private DeepCloneState.MiniDictionary.Entry[] _entries;
      private int _count;
      private static readonly int[] _primes = new int[72]
      {
        3,
        7,
        11,
        17,
        23,
        29,
        37,
        47,
        59,
        71,
        89,
        107,
        131,
        163,
        197,
        239,
        293,
        353,
        431,
        521,
        631,
        761,
        919,
        1103,
        1327,
        1597,
        1931,
        2333,
        2801,
        3371,
        4049,
        4861,
        5839,
        7013,
        8419,
        10103,
        12143,
        14591,
        17519,
        21023,
        25229,
        30293,
        36353,
        43627,
        52361,
        62851,
        75431,
        90523,
        108631,
        130363,
        156437,
        187751,
        225307,
        270371,
        324449,
        389357,
        467237,
        560689,
        672827,
        807403,
        968897,
        1162687,
        1395263,
        1674319,
        2009191,
        2411033,
        2893249,
        3471899,
        4166287,
        4999559,
        5999471,
        7199369
      };

      public MiniDictionary()
        : this(5)
      {
      }

      public MiniDictionary(int capacity)
      {
        if (capacity <= 0)
          return;
        this.Initialize(capacity);
      }

      public object FindEntry(object key)
      {
        if (this._buckets != null)
        {
          int num = RuntimeHelpers.GetHashCode(key) & int.MaxValue;
          DeepCloneState.MiniDictionary.Entry[] entries = this._entries;
          for (int index = this._buckets[num % this._buckets.Length]; index >= 0; index = entries[index].Next)
          {
            if (entries[index].HashCode == num && entries[index].Key == key)
              return entries[index].Value;
          }
        }
        return (object) null;
      }

      private static int GetPrime(int min)
      {
        for (int index = 0; index < DeepCloneState.MiniDictionary._primes.Length; ++index)
        {
          int prime = DeepCloneState.MiniDictionary._primes[index];
          if (prime >= min)
            return prime;
        }
        for (int candidate = min | 1; candidate < int.MaxValue; candidate += 2)
        {
          if (DeepCloneState.MiniDictionary.IsPrime(candidate) && (candidate - 1) % 101 != 0)
            return candidate;
        }
        return min;
      }

      private static bool IsPrime(int candidate)
      {
        if ((candidate & 1) == 0)
          return candidate == 2;
        int num = (int) Math.Sqrt((double) candidate);
        for (int index = 3; index <= num; index += 2)
        {
          if (candidate % index == 0)
            return false;
        }
        return true;
      }

      private static int ExpandPrime(int oldSize)
      {
        int min = 2 * oldSize;
        return (uint) min > 2146435069U && 2146435069 > oldSize ? 2146435069 : DeepCloneState.MiniDictionary.GetPrime(min);
      }

      private void Initialize(int size)
      {
        this._buckets = new int[size];
        for (int index = 0; index < this._buckets.Length; ++index)
          this._buckets[index] = -1;
        this._entries = new DeepCloneState.MiniDictionary.Entry[size];
      }

      public void Insert(object key, object value)
      {
        if (this._buckets == null)
          this.Initialize(0);
        int num = RuntimeHelpers.GetHashCode(key) & int.MaxValue;
        int index = num % this._buckets.Length;
        DeepCloneState.MiniDictionary.Entry[] entries = this._entries;
        if (this._count == entries.Length)
        {
          this.Resize();
          entries = this._entries;
          index = num % this._buckets.Length;
        }
        int count = this._count;
        ++this._count;
        entries[count].HashCode = num;
        entries[count].Next = this._buckets[index];
        entries[count].Key = key;
        entries[count].Value = value;
        this._buckets[index] = count;
      }

      private void Resize() => this.Resize(DeepCloneState.MiniDictionary.ExpandPrime(this._count));

      private void Resize(int newSize)
      {
        int[] numArray = new int[newSize];
        for (int index = 0; index < numArray.Length; ++index)
          numArray[index] = -1;
        DeepCloneState.MiniDictionary.Entry[] destinationArray = new DeepCloneState.MiniDictionary.Entry[newSize];
        Array.Copy((Array) this._entries, 0, (Array) destinationArray, 0, this._count);
        for (int index1 = 0; index1 < this._count; ++index1)
        {
          if (destinationArray[index1].HashCode >= 0)
          {
            int index2 = destinationArray[index1].HashCode % newSize;
            destinationArray[index1].Next = numArray[index2];
            numArray[index2] = index1;
          }
        }
        this._buckets = numArray;
        this._entries = destinationArray;
      }

      private struct Entry
      {
        public int HashCode;
        public int Next;
        public object Key;
        public object Value;
      }
    }
  }
}
