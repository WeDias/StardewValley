// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GalaxyNetClient
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Galaxy.Api;
using StardewValley.Network;
using System;
using System.IO;
using System.Linq;

namespace StardewValley.SDKs
{
  public class GalaxyNetClient : Client
  {
    private GalaxyID lobbyId;
    protected GalaxySocket client;
    private GalaxyID serverId;
    private float lastPingMs;

    public GalaxyNetClient(GalaxyID lobbyId) => this.lobbyId = lobbyId;

    public override string getUserID() => Convert.ToString(GalaxyInstance.User().GetGalaxyID().ToUint64());

    protected override string getHostUserName() => GalaxyInstance.Friends().GetFriendPersonaName(this.serverId);

    public override float GetPingToHost() => this.lastPingMs;

    protected override void connectImpl()
    {
      this.client = new GalaxySocket("1.5.5");
      GalaxyInstance.User().GetGalaxyID();
      this.client.JoinLobby(this.lobbyId, new Action<string>(this.onReceiveError));
    }

    public override void disconnect(bool neatly = true)
    {
      if (this.client == null)
        return;
      Console.WriteLine("Disconnecting from server {0}", (object) this.lobbyId);
      this.client.Close();
      this.client = (GalaxySocket) null;
      this.connectionMessage = (string) null;
    }

    protected override void receiveMessagesImpl()
    {
      if (this.client == null || !this.client.Connected)
        return;
      if (this.client.Connected && this.serverId == (GalaxyID) null)
        this.serverId = this.client.LobbyOwner;
      this.client.Receive(new Action<GalaxyID>(this.onReceiveConnection), new Action<GalaxyID, Stream>(this.onReceiveMessage), new Action<GalaxyID>(this.onReceiveDisconnect), new Action<string>(this.onReceiveError));
      if (this.client == null)
        return;
      this.client.Heartbeat(Enumerable.Repeat<GalaxyID>(this.serverId, 1));
      this.lastPingMs = (float) this.client.GetPingWith(this.serverId);
      if ((double) this.lastPingMs <= 30000.0)
        return;
      this.timedOut = true;
      this.pendingDisconnect = Multiplayer.DisconnectType.GalaxyTimeout;
      this.disconnect(true);
    }

    protected virtual void onReceiveConnection(GalaxyID peer)
    {
    }

    protected virtual void onReceiveMessage(GalaxyID peer, Stream messageStream)
    {
      if (peer != this.serverId)
        return;
      if (this.bandwidthLogger != null)
        this.bandwidthLogger.RecordBytesDown(messageStream.Length);
      using (IncomingMessage message = new IncomingMessage())
      {
        using (BinaryReader reader = new BinaryReader(messageStream))
        {
          message.Read(reader);
          this.processIncomingMessage(message);
        }
      }
    }

    protected virtual void onReceiveDisconnect(GalaxyID peer)
    {
      if (peer != this.serverId)
      {
        Game1.multiplayer.playerDisconnected((long) peer.ToUint64());
      }
      else
      {
        this.timedOut = true;
        this.pendingDisconnect = Multiplayer.DisconnectType.HostLeft;
      }
    }

    protected virtual void onReceiveError(string message) => this.connectionMessage = message;

    public override void sendMessage(OutgoingMessage message)
    {
      if (this.client == null || !this.client.Connected || this.serverId == (GalaxyID) null)
        return;
      if (this.bandwidthLogger != null)
      {
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) output))
          {
            message.Write(writer);
            output.Seek(0L, SeekOrigin.Begin);
            byte[] array = output.ToArray();
            this.client.Send(this.serverId, array);
            this.bandwidthLogger.RecordBytesUp((long) array.Length);
          }
        }
      }
      else
        this.client.Send(this.serverId, message);
    }
  }
}
