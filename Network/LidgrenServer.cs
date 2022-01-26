// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.LidgrenServer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace StardewValley.Network
{
  public class LidgrenServer : Server
  {
    public const int defaultPort = 24642;
    public NetServer server;
    private HashSet<NetConnection> introductionsSent = new HashSet<NetConnection>();
    protected Bimap<long, NetConnection> peers = new Bimap<long, NetConnection>();

    public override int connectionsCount => this.server == null ? 0 : this.server.ConnectionsCount;

    public LidgrenServer(IGameServer gameServer)
      : base(gameServer)
    {
    }

    public override bool isConnectionActive(string connectionID)
    {
      foreach (NetConnection connection in this.server.Connections)
      {
        if (this.getConnectionId(connection) == connectionID && connection.Status == NetConnectionStatus.Connected)
          return true;
      }
      return false;
    }

    public override string getUserId(long farmerId) => !this.peers.ContainsLeft(farmerId) ? (string) null : this.peers[farmerId].RemoteEndPoint.Address.ToString();

    public override bool hasUserId(string userId)
    {
      foreach (NetConnection rightValue in (IEnumerable<NetConnection>) this.peers.RightValues)
      {
        if (rightValue.RemoteEndPoint.Address.ToString().Equals(userId))
          return true;
      }
      return false;
    }

    public override string getUserName(long farmerId) => !this.peers.ContainsLeft(farmerId) ? (string) null : this.peers[farmerId].RemoteEndPoint.Address.ToString();

    public override float getPingToClient(long farmerId) => !this.peers.ContainsLeft(farmerId) ? -1f : (float) ((double) this.peers[farmerId].AverageRoundtripTime / 2.0 * 1000.0);

    public override void setPrivacy(ServerPrivacy privacy)
    {
    }

    public override bool canAcceptIPConnections() => true;

    public override bool connected() => this.server != null;

    public override void initialize()
    {
      Console.WriteLine("Starting LAN server");
      NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
      config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
      config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
      config.Port = 24642;
      config.ConnectionTimeout = 30f;
      config.PingInterval = 5f;
      config.MaximumConnections = Game1.multiplayer.playerLimit * 2;
      config.MaximumTransmissionUnit = 1200;
      this.server = new NetServer(config);
      this.server.Start();
    }

    public override void stopServer()
    {
      Console.WriteLine("Stopping LAN server");
      this.server.Shutdown("Server shutting down...");
      this.server.FlushSendQueue();
      this.introductionsSent.Clear();
      this.peers.Clear();
    }

    public static bool IsLocal(string host_name_or_address)
    {
      if (string.IsNullOrEmpty(host_name_or_address))
        return false;
      try
      {
        IPAddress[] hostAddresses = Dns.GetHostAddresses(host_name_or_address);
        IPAddress[] local_ips = Dns.GetHostAddresses(Dns.GetHostName());
        Func<IPAddress, bool> predicate = (Func<IPAddress, bool>) (host_ip => IPAddress.IsLoopback(host_ip) || ((IEnumerable<IPAddress>) local_ips).Contains<IPAddress>(host_ip));
        return ((IEnumerable<IPAddress>) hostAddresses).Any<IPAddress>(predicate);
      }
      catch
      {
        return false;
      }
    }

    public override void receiveMessages()
    {
      NetIncomingMessage netIncomingMessage;
      while ((netIncomingMessage = this.server.ReadMessage()) != null)
      {
        if (this.bandwidthLogger != null)
          this.bandwidthLogger.RecordBytesDown((long) netIncomingMessage.LengthBytes);
        switch (netIncomingMessage.MessageType)
        {
          case NetIncomingMessageType.StatusChanged:
            this.statusChanged(netIncomingMessage);
            break;
          case NetIncomingMessageType.ConnectionApproval:
            if (Game1.options.ipConnectionsEnabled || this.gameServer.IsLocalMultiplayerInitiatedServer())
            {
              netIncomingMessage.SenderConnection.Approve();
              break;
            }
            netIncomingMessage.SenderConnection.Deny();
            break;
          case NetIncomingMessageType.Data:
            this.parseDataMessageFromClient(netIncomingMessage);
            break;
          case NetIncomingMessageType.DiscoveryRequest:
            if ((Game1.options.ipConnectionsEnabled || this.gameServer.IsLocalMultiplayerInitiatedServer()) && (!this.gameServer.IsLocalMultiplayerInitiatedServer() || LidgrenServer.IsLocal(netIncomingMessage.SenderEndPoint.Address.ToString())) && !this.gameServer.isUserBanned(netIncomingMessage.SenderEndPoint.Address.ToString()))
            {
              this.sendVersionInfo(netIncomingMessage);
              break;
            }
            break;
          case NetIncomingMessageType.DebugMessage:
          case NetIncomingMessageType.WarningMessage:
          case NetIncomingMessageType.ErrorMessage:
            string str = netIncomingMessage.ReadString();
            Console.WriteLine("{0}: {1}", (object) netIncomingMessage.MessageType, (object) str);
            Game1.debugOutput = str;
            break;
          default:
            Game1.debugOutput = netIncomingMessage.ToString();
            break;
        }
        this.server.Recycle(netIncomingMessage);
      }
      foreach (NetConnection connection in this.server.Connections)
      {
        NetConnection conn = connection;
        if (conn.Status == NetConnectionStatus.Connected && !this.introductionsSent.Contains(conn))
        {
          if (!this.gameServer.whenGameAvailable((Action) (() => this.gameServer.sendAvailableFarmhands("", (Action<OutgoingMessage>) (msg => this.sendMessage(conn, msg)))), (Func<bool>) (() => Game1.gameMode != (byte) 6)))
          {
            Console.WriteLine("Postponing introduction message");
            this.sendMessage(conn, new OutgoingMessage((byte) 11, Game1.player, new object[1]
            {
              (object) "Strings\\UI:Client_WaitForHostLoad"
            }));
          }
          this.introductionsSent.Add(conn);
        }
      }
      if (this.bandwidthLogger == null)
        return;
      this.bandwidthLogger.Update();
    }

    private void sendVersionInfo(NetIncomingMessage message)
    {
      NetOutgoingMessage message1 = this.server.CreateMessage();
      message1.Write("1.5.5");
      message1.Write("StardewValley");
      this.server.SendDiscoveryResponse(message1, message.SenderEndPoint);
      if (this.bandwidthLogger == null)
        return;
      this.bandwidthLogger.RecordBytesUp((long) message1.LengthBytes);
    }

    private void statusChanged(NetIncomingMessage message)
    {
      switch ((NetConnectionStatus) message.ReadByte())
      {
        case NetConnectionStatus.Connected:
          this.onConnect(this.getConnectionId(message.SenderConnection));
          break;
        case NetConnectionStatus.Disconnecting:
        case NetConnectionStatus.Disconnected:
          this.onDisconnect(this.getConnectionId(message.SenderConnection));
          if (!this.peers.ContainsRight(message.SenderConnection))
            break;
          this.playerDisconnected(this.peers[message.SenderConnection]);
          break;
      }
    }

    public override void kick(long disconnectee)
    {
      base.kick(disconnectee);
      if (!this.peers.ContainsLeft(disconnectee))
        return;
      this.peers[disconnectee].Disconnect(Multiplayer.kicked);
      this.server.FlushSendQueue();
      this.playerDisconnected(disconnectee);
    }

    public override void playerDisconnected(long disconnectee)
    {
      base.playerDisconnected(disconnectee);
      this.introductionsSent.Remove(this.peers[disconnectee]);
      this.peers.RemoveLeft(disconnectee);
    }

    protected virtual void parseDataMessageFromClient(NetIncomingMessage dataMsg)
    {
      NetConnection peer = dataMsg.SenderConnection;
      using (IncomingMessage message = new IncomingMessage())
      {
        using (NetBufferReadStream input = new NetBufferReadStream((NetBuffer) dataMsg))
        {
          using (BinaryReader reader = new BinaryReader((Stream) input))
          {
            while ((long) dataMsg.LengthBits - dataMsg.Position >= 8L)
            {
              message.Read(reader);
              if (this.peers.ContainsLeft(message.FarmerID) && this.peers[message.FarmerID] == peer)
                this.gameServer.processIncomingMessage(message);
              else if (message.MessageType == (byte) 2)
              {
                NetFarmerRoot farmer = Game1.multiplayer.readFarmer(message.Reader);
                this.gameServer.checkFarmhandRequest("", this.getConnectionId(dataMsg.SenderConnection), farmer, closure_0 ?? (closure_0 = (Action<OutgoingMessage>) (msg => this.sendMessage(peer, msg))), (Action) (() => this.peers[farmer.Value.UniqueMultiplayerID] = peer));
              }
            }
          }
        }
      }
    }

    public string getConnectionId(NetConnection connection) => "L_" + connection.RemoteUniqueIdentifier.ToString();

    public override void sendMessage(long peerId, OutgoingMessage message)
    {
      if (!this.peers.ContainsLeft(peerId))
        return;
      this.sendMessage(this.peers[peerId], message);
    }

    protected virtual void sendMessage(NetConnection connection, OutgoingMessage message)
    {
      NetOutgoingMessage message1 = this.server.CreateMessage();
      using (NetBufferWriteStream output = new NetBufferWriteStream((NetBuffer) message1))
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
          message.Write(writer);
      }
      int num = (int) this.server.SendMessage(message1, connection, NetDeliveryMethod.ReliableOrdered);
      if (this.bandwidthLogger == null)
        return;
      this.bandwidthLogger.RecordBytesUp((long) message1.LengthBytes);
    }

    public override void setLobbyData(string key, string value)
    {
    }
  }
}
