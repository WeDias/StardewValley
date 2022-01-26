// Decompiled with JetBrains decompiler
// Type: StardewValley.IAnimalLocation
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;

namespace StardewValley
{
  public interface IAnimalLocation
  {
    NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> Animals { get; }

    bool CheckPetAnimal(Vector2 position, Farmer who);

    bool CheckPetAnimal(Rectangle rect, Farmer who);

    bool CheckInspectAnimal(Vector2 position, Farmer who);

    bool CheckInspectAnimal(Rectangle rect, Farmer who);
  }
}
