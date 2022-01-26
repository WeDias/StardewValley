// Decompiled with JetBrains decompiler
// Type: StardewValley.ModDataDictionary
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Network;

namespace StardewValley
{
  public class ModDataDictionary : NetStringDictionary<string, NetString>
  {
    public ModDataDictionary() => this.InterpolationWait = false;

    public virtual void SetFromSerialization(ModDataDictionary source)
    {
      this.Clear();
      if (source == null)
        return;
      foreach (string key in source.Keys)
        this[key] = source[key];
    }

    public ModDataDictionary GetForSerialization() => Game1.game1 != null && Game1.game1.IsSaving && this.Count() == 0 ? (ModDataDictionary) null : this;
  }
}
