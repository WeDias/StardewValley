// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Cellar
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace StardewValley.Locations
{
  public class Cellar : GameLocation
  {
    public readonly NetLong ownerUID = new NetLong(0L);

    public Cellar()
    {
    }

    public Cellar(string mapPath, string name)
      : base(mapPath, name)
    {
      this.setUpAgingBoards();
    }

    public override void DayUpdate(int dayOfMonth) => base.DayUpdate(dayOfMonth);

    public void setUpAgingBoards()
    {
      for (int x = 6; x < 17; ++x)
      {
        Vector2 vector2 = new Vector2((float) x, 8f);
        if (!this.objects.ContainsKey(vector2))
          this.objects.Add(vector2, (Object) new Cask(vector2));
        vector2 = new Vector2((float) x, 10f);
        if (!this.objects.ContainsKey(vector2))
          this.objects.Add(vector2, (Object) new Cask(vector2));
        vector2 = new Vector2((float) x, 12f);
        if (!this.objects.ContainsKey(vector2))
          this.objects.Add(vector2, (Object) new Cask(vector2));
      }
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      string str = "Farmhouse";
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value is Cabin)
        {
          Cabin cabin = building.indoors.Value as Cabin;
          if (cabin.GetCellarName() == this.Name)
          {
            str = cabin.NameOrUniqueName;
            break;
          }
        }
      }
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
        warp.TargetName = str;
    }

    public override void updateWarps() => base.updateWarps();
  }
}
