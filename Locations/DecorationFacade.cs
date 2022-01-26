// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.DecorationFacade
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Locations
{
  public class DecorationFacade : SerializationCollectionFacade<int>
  {
    public readonly NetIntDictionary<int, NetInt> Field = new NetIntDictionary<int, NetInt>();
    private List<Action> pendingChanges = new List<Action>();
    [NonInstancedStatic]
    public static bool warnedDeprecated;

    public event DecorationFacade.ChangeEvent OnChange;

    public int this[int whichRoom]
    {
      get
      {
        this.WarnDeprecation();
        return this.Field.ContainsKey(whichRoom) ? this.Field[whichRoom] : 0;
      }
      set
      {
        this.WarnDeprecation();
        this.Field[whichRoom] = value;
      }
    }

    public int Count => this.Field.Count() == 0 ? 0 : this.Field.Keys.Max() + 1;

    public DecorationFacade()
    {
      this.Field.InterpolationWait = false;
      this.Field.OnValueAdded += (NetDictionary<int, int, NetInt, SerializableDictionary<int, int>, NetIntDictionary<int, NetInt>>.ContentsChangeEvent) ((whichRoom, which) =>
      {
        this.Field.InterpolationWait = false;
        this.Field.FieldDict[whichRoom].fieldChangeEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, oldValue, newValue) => this.changed(whichRoom, newValue));
        this.changed(whichRoom, which);
      });
    }

    private void changed(int whichRoom, int which) => this.pendingChanges.Add((Action) (() =>
    {
      if (this.OnChange == null)
        return;
      this.OnChange(whichRoom, which);
    }));

    protected override List<int> Serialize()
    {
      List<int> intList = new List<int>();
      while (intList.Count < this.Count)
        intList.Add(0);
      foreach (KeyValuePair<int, int> pair in this.Field.Pairs)
        intList[pair.Key] = pair.Value;
      return intList;
    }

    protected override void DeserializeAdd(int serialValue) => this.Field[this.Count] = serialValue;

    public void Set(DecorationFacade other) => this.Field.Set((IEnumerable<KeyValuePair<int, int>>) other.Field.Pairs);

    public void SetCountAtLeast(int targetCount)
    {
      while (this.Count < targetCount)
        this[this.Count] = 0;
    }

    public void Update()
    {
      foreach (Action pendingChange in this.pendingChanges)
        pendingChange();
      this.pendingChanges.Clear();
    }

    public virtual void WarnDeprecation()
    {
      if (Game1.gameMode == (byte) 6 || DecorationFacade.warnedDeprecated)
        return;
      DecorationFacade.warnedDeprecated = true;
      Console.WriteLine("WARNING: DecorationFacade/DecoratableLocation.wallPaper and floor are deprecated. Use wallpaperIDs, appliedWallpaper, wallPaperTiles/floorIDs, appliedFloor, and floorTiles instead.");
    }

    public delegate void ChangeEvent(int whichRoom, int which);
  }
}
