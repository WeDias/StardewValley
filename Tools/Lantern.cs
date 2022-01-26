// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Lantern
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System.Linq;

namespace StardewValley.Tools
{
  public class Lantern : Tool
  {
    public const float baseRadius = 10f;
    public const int millisecondsPerFuelUnit = 6000;
    public const int maxFuel = 100;
    public int fuelLeft;
    private int fuelTimer;
    public bool on;

    public Lantern()
      : base(nameof (Lantern), 0, 74, 74, false)
    {
      this.UpgradeLevel = 0;
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
      this.InstantUse = true;
    }

    public override Item getOne()
    {
      Lantern one = new Lantern();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Lantern.cs.14115");

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Lantern.cs.14114");

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      this.on = !this.on;
      this.CurrentParentTileIndex = this.IndexOfMenuItemView;
      if (this.on)
      {
        Game1.currentLightSources.Add(new LightSource(1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), (float) (2.5 + (double) this.fuelLeft / 100.0 * 10.0 * 0.75), new Color(0, 131, (int) byte.MaxValue), -85736));
      }
      else
      {
        for (int index = Game1.currentLightSources.Count - 1; index >= 0; --index)
        {
          if ((int) (NetFieldBase<int, NetInt>) Game1.currentLightSources.ElementAt<LightSource>(index).identifier == -85736)
          {
            Game1.currentLightSources.Remove(Game1.currentLightSources.ElementAt<LightSource>(index));
            break;
          }
        }
      }
    }

    public override void tickUpdate(GameTime time, Farmer who)
    {
      if (this.on && this.fuelLeft > 0 && Game1.drawLighting)
      {
        this.fuelTimer += time.ElapsedGameTime.Milliseconds;
        if (this.fuelTimer > 6000)
        {
          --this.fuelLeft;
          this.fuelTimer = 0;
        }
        bool flag = false;
        foreach (LightSource currentLightSource in Game1.currentLightSources)
        {
          if ((int) (NetFieldBase<int, NetInt>) currentLightSource.identifier == -85736)
          {
            currentLightSource.position.Value = new Vector2(who.Position.X + 21f, who.Position.Y + 64f);
            flag = true;
            break;
          }
        }
        if (!flag)
          Game1.currentLightSources.Add(new LightSource(1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), (float) (2.5 + (double) this.fuelLeft / 100.0 * 10.0 * 0.75), new Color(0, 131, (int) byte.MaxValue), -85736));
      }
      if (!this.on || this.fuelLeft > 0)
        return;
      Utility.removeLightSource(1);
    }
  }
}
