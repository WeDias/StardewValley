// Decompiled with JetBrains decompiler
// Type: StardewValley.InteriorDoorDictionary
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using xTile.ObjectModel;

namespace StardewValley
{
  public class InteriorDoorDictionary : NetPointDictionary<bool, InteriorDoor>
  {
    private GameLocation location;

    public InteriorDoorDictionary.DoorCollection Doors => new InteriorDoorDictionary.DoorCollection(this);

    public InteriorDoorDictionary(GameLocation location) => this.location = location;

    protected override void setFieldValue(InteriorDoor door, Point position, bool open)
    {
      door.Location = this.location;
      door.Position = position;
      base.setFieldValue(door, position, open);
    }

    public void ResetSharedState()
    {
      PropertyValue propertyValue;
      if ((bool) (NetFieldBase<bool, NetBool>) this.location.isOutdoors || !this.location.Map.Properties.TryGetValue("Doors", out propertyValue) || propertyValue == null)
        return;
      string[] strArray = propertyValue.ToString().Split(' ');
      for (int index = 0; index < strArray.Length; index += 4)
        this[new Point(Convert.ToInt32(strArray[index]), Convert.ToInt32(strArray[index + 1]))] = false;
    }

    public void ResetLocalState()
    {
      PropertyValue propertyValue;
      if ((bool) (NetFieldBase<bool, NetBool>) this.location.isOutdoors || !this.location.Map.Properties.TryGetValue("Doors", out propertyValue) || propertyValue == null)
        return;
      string[] strArray = propertyValue.ToString().Split(' ');
      for (int index = 0; index < strArray.Length; index += 4)
      {
        Point key = new Point(Convert.ToInt32(strArray[index]), Convert.ToInt32(strArray[index + 1]));
        if (this.ContainsKey(key))
        {
          InteriorDoor interiorDoor = this.FieldDict[key];
          interiorDoor.Location = this.location;
          interiorDoor.Position = key;
          interiorDoor.ResetLocalState();
        }
      }
    }

    public void MakeMapModifications()
    {
      foreach (InteriorDoor door in this.Doors)
        door.ApplyMapModifications();
    }

    public void CleanUpLocalState()
    {
      foreach (InteriorDoor door in this.Doors)
        door.CleanUpLocalState();
    }

    public void Update(GameTime time)
    {
      foreach (InteriorDoor door in this.Doors)
        door.Update(time);
    }

    public void Draw(SpriteBatch b)
    {
      foreach (InteriorDoor door in this.Doors)
        door.Draw(b);
    }

    public struct DoorCollection : IEnumerable<InteriorDoor>, IEnumerable
    {
      private InteriorDoorDictionary _dict;

      public DoorCollection(InteriorDoorDictionary dict) => this._dict = dict;

      public InteriorDoorDictionary.DoorCollection.Enumerator GetEnumerator() => new InteriorDoorDictionary.DoorCollection.Enumerator(this._dict);

      IEnumerator<InteriorDoor> IEnumerable<InteriorDoor>.GetEnumerator() => (IEnumerator<InteriorDoor>) new InteriorDoorDictionary.DoorCollection.Enumerator(this._dict);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new InteriorDoorDictionary.DoorCollection.Enumerator(this._dict);

      public struct Enumerator : IEnumerator<InteriorDoor>, IEnumerator, IDisposable
      {
        private readonly InteriorDoorDictionary _dict;
        private Dictionary<Point, InteriorDoor>.Enumerator _enumerator;
        private InteriorDoor _current;
        private bool _done;

        public Enumerator(InteriorDoorDictionary dict)
        {
          this._dict = dict;
          this._enumerator = this._dict.FieldDict.GetEnumerator();
          this._current = (InteriorDoor) null;
          this._done = false;
        }

        public bool MoveNext()
        {
          if (this._enumerator.MoveNext())
          {
            KeyValuePair<Point, InteriorDoor> current = this._enumerator.Current;
            this._current = current.Value;
            this._current.Location = this._dict.location;
            this._current.Position = current.Key;
            return true;
          }
          this._done = true;
          this._current = (InteriorDoor) null;
          return false;
        }

        public InteriorDoor Current => this._current;

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
          this._enumerator = this._dict.FieldDict.GetEnumerator();
          this._current = (InteriorDoor) null;
          this._done = false;
        }
      }
    }
  }
}
