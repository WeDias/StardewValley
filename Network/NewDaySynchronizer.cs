// Decompiled with JetBrains decompiler
// Type: StardewValley.NewDaySynchronizer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Network;
using System.Collections.Generic;
using System.Threading;

namespace StardewValley
{
  public class NewDaySynchronizer : NetSynchronizer
  {
    public void start()
    {
      Game1.multiplayer.UpdateEarly();
      if (!Game1.IsServer)
        return;
      this.sendVar<NetBool, bool>("started", true);
    }

    public bool readyForFinish()
    {
      Game1.player.team.SetLocalReady("wakeup", true);
      Game1.player.team.Update();
      Game1.multiplayer.UpdateLate();
      Game1.multiplayer.UpdateEarly();
      return Game1.player.team.IsReady("wakeup");
    }

    public int numReadyForFinish() => Game1.player.team.GetNumberReady("wakeup");

    public bool readyForSave()
    {
      Game1.player.team.SetLocalReady("ready_for_save", true);
      Game1.player.team.Update();
      Game1.multiplayer.UpdateLate();
      Game1.multiplayer.UpdateEarly();
      return Game1.player.team.IsReady("ready_for_save");
    }

    public int numReadyForSave() => Game1.player.team.GetNumberReady("ready_for_save");

    public void finish()
    {
      if (Game1.IsServer)
        this.sendVar<NetBool, bool>("finished", true);
      Game1.multiplayer.UpdateLate();
    }

    public bool hasFinished() => this.hasVar("finished");

    public void flagSaved()
    {
      if (Game1.IsServer)
        this.sendVar<NetBool, bool>("saved", true);
      Game1.multiplayer.UpdateLate();
    }

    public bool hasSaved() => this.hasVar("saved");

    public void waitForFinish()
    {
      if (!Game1.IsClient)
        return;
      this.waitForVar<NetBool, bool>("finished");
    }

    public override void processMessages()
    {
      Game1.multiplayer.UpdateLate();
      Thread.Sleep(16);
      Program.sdk.Update();
      Game1.multiplayer.UpdateEarly();
    }

    protected override void sendMessage(params object[] data)
    {
      OutgoingMessage message = new OutgoingMessage((byte) 14, Game1.player, data);
      if (Game1.IsServer)
      {
        foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
          Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
      }
      else
      {
        if (!Game1.IsClient)
          return;
        Game1.client.sendMessage(message);
      }
    }
  }
}
