// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GalaxyNetServer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Galaxy.Api;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewValley.SDKs
{
  public class GalaxyNetServer : Server
  {
    private GalaxyID host;
    protected GalaxySocket server;
    private GalaxyNetServer.GalaxyPersonaDataChangedListener galaxyPersonaDataChangedListener;
    protected Bimap<long, ulong> peers = new Bimap<long, ulong>();

    public GalaxyNetServer(IGameServer gameServer)
      : base(gameServer)
    {
    }

    public override int connectionsCount => this.server == null ? 0 : this.server.ConnectionCount;

    public override string getUserId(long farmerId) => !this.peers.ContainsLeft(farmerId) ? (string) null : this.peers[farmerId].ToString();

    public override bool hasUserId(string userId)
    {
      foreach (ulong rightValue in (IEnumerable<ulong>) this.peers.RightValues)
      {
        if (rightValue.ToString().Equals(userId))
          return true;
      }
      return false;
    }

    public override bool isConnectionActive(string connection_id)
    {
      foreach (GalaxyID connection in this.server.Connections)
      {
        if (this.getConnectionId(connection) == connection_id && connection.IsValid())
          return true;
      }
      return false;
    }

    public override string getUserName(long farmerId)
    {
      if (!this.peers.ContainsLeft(farmerId))
        return (string) null;
      GalaxyID userID = new GalaxyID(this.peers[farmerId]);
      return GalaxyInstance.Friends().GetFriendPersonaName(userID);
    }

    public override float getPingToClient(long farmerId) => !this.peers.ContainsLeft(farmerId) ? -1f : (float) this.server.GetPingWith(new GalaxyID(this.peers[farmerId]));

    public override void setPrivacy(ServerPrivacy privacy) => this.server.SetPrivacy(privacy);

    public override bool connected() => this.server.Connected;

    public override bool canOfferInvite() => this.server.Connected;

    public override void offerInvite()
    {
      if (!this.server.Connected || Program.sdk.Networking == null)
        return;
      ulong? inviteDialogLobby = this.server.InviteDialogLobby;
      if (!inviteDialogLobby.HasValue)
        return;
      SDKNetHelper networking = Program.sdk.Networking;
      inviteDialogLobby = this.server.InviteDialogLobby;
      // ISSUE: variable of a boxed type
      __Boxed<ulong> lobby = (ValueType) inviteDialogLobby.Value;
      networking.ShowInviteDialog((object) lobby);
    }

    public override string getInviteCode() => this.server.GetInviteCode();

    public override void initialize()
    {
      Console.WriteLine("Starting Galaxy server");
      this.host = GalaxyInstance.User().GetGalaxyID();
      this.galaxyPersonaDataChangedListener = new GalaxyNetServer.GalaxyPersonaDataChangedListener(new Action<GalaxyID, uint>(this.onPersonaDataChanged));
      this.server = new GalaxySocket("1.5.5");
      this.server.CreateLobby(Game1.options.serverPrivacy, (uint) (Game1.multiplayer.playerLimit * 2));
    }

    public override void stopServer()
    {
      Console.WriteLine("Stopping Galaxy server");
      this.server.Close();
      if (this.galaxyPersonaDataChangedListener == null)
        return;
      this.galaxyPersonaDataChangedListener.Dispose();
      this.galaxyPersonaDataChangedListener = (GalaxyNetServer.GalaxyPersonaDataChangedListener) null;
    }

    private void onPersonaDataChanged(GalaxyID userID, uint avatarCriteria)
    {
      if (!this.peers.ContainsRight(userID.ToUint64()))
        return;
      long left = this.peers.GetLeft(userID.ToUint64());
      Game1.multiplayer.broadcastUserName(left, GalaxyInstance.Friends().GetFriendPersonaName(userID));
    }

    public override void receiveMessages()
    {
      if (this.server == null)
        return;
      this.server.Receive(new Action<GalaxyID>(this.onReceiveConnection), new Action<GalaxyID, Stream>(this.onReceiveMessage), new Action<GalaxyID>(this.onReceiveDisconnect), new Action<string>(this.onReceiveError));
      this.server.Heartbeat(this.server.LobbyMembers());
      foreach (GalaxyID connection in this.server.Connections)
      {
        if (this.server.GetPingWith(connection) > 30000L)
          this.server.Kick(connection);
      }
      if (this.bandwidthLogger == null)
        return;
      this.bandwidthLogger.Update();
    }

    public override void kick(long disconnectee)
    {
      base.kick(disconnectee);
      if (!this.peers.ContainsLeft(disconnectee))
        return;
      GalaxyID galaxyId = new GalaxyID(this.peers[disconnectee]);
      this.server.Kick(galaxyId);
      this.sendMessage(galaxyId, new OutgoingMessage((byte) 23, Game1.player, (object[]) new StardewValley.Object[0]));
    }

    public string getConnectionId(GalaxyID peer) => "GN_" + Convert.ToString(peer.ToUint64());

    private string createUserID(GalaxyID peer) => Convert.ToString(peer.ToUint64());

    protected virtual void onReceiveConnection(GalaxyID peer)
    {
      if (this.gameServer.isUserBanned(((object) peer).ToString()))
        return;
      Console.WriteLine("{0} connected", (object) peer);
      this.onConnect(this.getConnectionId(peer));
      this.gameServer.sendAvailableFarmhands(this.createUserID(peer), (Action<OutgoingMessage>) (msg => this.sendMessage(peer, msg)));
    }

    protected virtual void onReceiveMessage(GalaxyID peer, Stream messageStream)
    {
      if (this.bandwidthLogger != null)
        this.bandwidthLogger.RecordBytesDown(messageStream.Length);
      using (IncomingMessage message = new IncomingMessage())
      {
        using (BinaryReader reader = new BinaryReader(messageStream))
        {
          message.Read(reader);
          if (this.peers.ContainsLeft(message.FarmerID) && (long) this.peers[message.FarmerID] == (long) peer.ToUint64())
          {
            this.gameServer.processIncomingMessage(message);
          }
          else
          {
            if (message.MessageType != (byte) 2)
              return;
            NetFarmerRoot farmer = Game1.multiplayer.readFarmer(message.Reader);
            GalaxyID capturedPeer = new GalaxyID(peer.ToUint64());
            this.gameServer.checkFarmhandRequest(this.createUserID(peer), this.getConnectionId(peer), farmer, (Action<OutgoingMessage>) (msg => this.sendMessage(capturedPeer, msg)), (Action) (() => this.peers[farmer.Value.UniqueMultiplayerID] = capturedPeer.ToUint64()));
          }
        }
      }
    }

    public virtual void onReceiveDisconnect(GalaxyID peer)
    {
      Console.WriteLine("{0} disconnected", (object) peer);
      this.onDisconnect(this.getConnectionId(peer));
      if (!this.peers.ContainsRight(peer.ToUint64()))
        return;
      this.playerDisconnected(this.peers[peer.ToUint64()]);
    }

    protected virtual void onReceiveError(string messageKey) => Console.WriteLine("Server error: " + Game1.content.LoadString(messageKey));

    public override void playerDisconnected(long disconnectee)
    {
      base.playerDisconnected(disconnectee);
      this.peers.RemoveLeft(disconnectee);
    }

    public override void sendMessage(long peerId, OutgoingMessage message)
    {
      if (!this.peers.ContainsLeft(peerId))
        return;
      this.sendMessage(new GalaxyID(this.peers[peerId]), message);
    }

    protected virtual void sendMessage(GalaxyID peer, OutgoingMessage message)
    {
      if (this.bandwidthLogger != null)
      {
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) output))
          {
            message.Write(writer);
            output.Seek(0L, SeekOrigin.Begin);
            byte[] array = output.ToArray();
            this.server.Send(peer, array);
            this.bandwidthLogger.RecordBytesUp((long) array.Length);
          }
        }
      }
      else
        this.server.Send(peer, message);
    }

    public override void setLobbyData(string key, string value) => this.server.SetLobbyData(key, value);

    private class GalaxyPersonaDataChangedListener : IPersonaDataChangedListener
    {
      private Action<GalaxyID, uint> callback;

      public GalaxyPersonaDataChangedListener(Action<GalaxyID, uint> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerPersonaDataChanged.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnPersonaDataChanged(GalaxyID userID, uint avatarCriteria) => this.callback(userID, avatarCriteria);

      public override void Dispose()
      {
        GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerPersonaDataChanged.GetListenerType(), (IGalaxyListener) this);
        base.Dispose();
      }
    }
  }
}
