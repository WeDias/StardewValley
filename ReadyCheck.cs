// Decompiled with JetBrains decompiler
// Type: StardewValley.ReadyCheck
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Menus;
using StardewValley.Network;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley
{
  internal class ReadyCheck : INetObject<NetFields>
  {
    private readonly NetString name = new NetString();
    private readonly NetFarmerCollection readyPlayers = new NetFarmerCollection();
    private readonly NetFarmerCollection setPlayers = new NetFarmerCollection();
    private readonly NetFarmerCollection requiredPlayers = new NetFarmerCollection();

    public NetFields NetFields { get; } = new NetFields();

    public string Name => (string) (NetFieldBase<string, NetString>) this.name;

    public ReadyCheck() => this.NetFields.AddFields((INetSerializable) this.name, (INetSerializable) this.readyPlayers.NetFields, (INetSerializable) this.setPlayers.NetFields);

    public ReadyCheck(string name)
      : this()
    {
      this.name.Value = name;
    }

    public void SetRequiredFarmers(IEnumerable<Farmer> required_farmers)
    {
      this.requiredPlayers.Clear();
      if (required_farmers == null)
        return;
      foreach (Farmer requiredFarmer in required_farmers)
        this.requiredPlayers.Add(requiredFarmer);
    }

    private IEnumerable<Farmer> GetRequiredPlayers()
    {
      if (this.requiredPlayers.Count != 0)
        return (IEnumerable<Farmer>) this.requiredPlayers;
      return this.setPlayers.Contains(Game1.player) ? this.readyPlayers.Intersect<Farmer>((IEnumerable<Farmer>) Game1.getOnlineFarmers()) : (IEnumerable<Farmer>) Game1.getOnlineFarmers();
    }

    private bool containsAllPlayers(NetFarmerCollection farmerSet)
    {
      foreach (Farmer requiredPlayer in this.GetRequiredPlayers())
      {
        if (!farmerSet.Contains(requiredPlayer) && !Game1.multiplayer.isDisconnecting(requiredPlayer))
          return false;
      }
      return true;
    }

    public bool IsOtherFarmerReady(Farmer farmer) => this.setPlayers.Contains(farmer);

    public bool IsCancelable() => !this.setPlayers.Contains(Game1.player);

    public bool IsReady() => this.containsAllPlayers(this.setPlayers);

    public int GetNumberReady() => this.readyPlayers.Count;

    public int GetNumberRequired()
    {
      int numberRequired = 0;
      foreach (Farmer requiredPlayer in this.GetRequiredPlayers())
        ++numberRequired;
      return numberRequired;
    }

    public void SetLocalReady(bool ready)
    {
      if (ready && !this.readyPlayers.Contains(Game1.player))
      {
        this.readyPlayers.Add(Game1.player);
      }
      else
      {
        if (ready || !this.readyPlayers.Contains(Game1.player))
          return;
        this.readyPlayers.Remove(Game1.player);
        this.setPlayers.Remove(Game1.player);
      }
    }

    public void Update()
    {
      if (this.readyPlayers.Contains(Game1.player) && !this.setPlayers.Contains(Game1.player))
      {
        switch (Game1.activeClickableMenu)
        {
          case SaveGameMenu _:
          case ShippingMenu _:
            goto label_4;
          case ReadyCheckDialog readyCheckDialog:
            if (!(readyCheckDialog.checkName != this.Name))
              goto label_4;
            else
              break;
        }
        this.SetLocalReady(false);
      }
label_4:
      if (this.requiredPlayers.RetainOnlinePlayers())
        this.setPlayers.Remove(Game1.player);
      if (this.readyPlayers.RetainOnlinePlayers())
        this.setPlayers.Remove(Game1.player);
      if (this.containsAllPlayers(this.readyPlayers))
      {
        if (this.setPlayers.Contains(Game1.player))
          return;
        this.setPlayers.Add(Game1.player);
      }
      else
      {
        if (!this.setPlayers.Contains(Game1.player))
          return;
        this.setPlayers.Remove(Game1.player);
      }
    }
  }
}
