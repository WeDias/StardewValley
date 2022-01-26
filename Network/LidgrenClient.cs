// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.LidgrenClient
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Lidgren.Network;
using System;
using System.IO;

namespace StardewValley.Network
{
  public class LidgrenClient : Client
  {
    private string address;
    public NetClient client;
    private bool serverDiscovered;
    private int maxRetryAttempts;
    private int retryMs = 10000;
    private double lastAttemptMs;
    private int retryAttempts;
    private float lastLatencyMs;

    public LidgrenClient(string address) => this.address = address;

    public override string getUserID() => "";

    public override float GetPingToHost() => this.lastLatencyMs / 2f;

    protected override string getHostUserName() => this.client.ServerConnection.RemoteEndPoint.Address.ToString();

    protected override void connectImpl()
    {
      NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
      config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
      config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
      config.ConnectionTimeout = 30f;
      config.PingInterval = 5f;
      config.MaximumTransmissionUnit = 1200;
      this.client = new NetClient(config);
      this.client.Start();
      this.attemptConnection();
    }

    private void attemptConnection()
    {
      int serverPort = 24642;
      if (this.address.Contains(":"))
      {
        string[] strArray = this.address.Split(':');
        this.address = strArray[0];
        serverPort = Convert.ToInt32(strArray[1]);
      }
      this.client.DiscoverKnownPeer(this.address, serverPort);
      this.lastAttemptMs = DateTime.Now.TimeOfDay.TotalMilliseconds;
    }

    public override void disconnect(bool neatly = true)
    {
      if (this.client == null)
        return;
      if (this.client.ConnectionStatus != NetConnectionStatus.Disconnected && this.client.ConnectionStatus != NetConnectionStatus.Disconnecting)
      {
        if (neatly)
          this.sendMessage(new OutgoingMessage((byte) 19, Game1.player, Array.Empty<object>()));
        this.client.FlushSendQueue();
        this.client.Disconnect("");
        this.client.FlushSendQueue();
      }
      this.connectionMessage = (string) null;
    }

    protected virtual bool validateProtocol(string version) => version == "1.5.5";

    protected override void receiveMessagesImpl()
    {
      DateTime now;
      if (this.client != null && !this.serverDiscovered)
      {
        now = DateTime.Now;
        if (now.TimeOfDay.TotalMilliseconds >= this.lastAttemptMs + (double) this.retryMs && this.retryAttempts < this.maxRetryAttempts)
        {
          this.attemptConnection();
          ++this.retryAttempts;
        }
      }
      NetIncomingMessage netIncomingMessage;
      while ((netIncomingMessage = this.client.ReadMessage()) != null)
      {
        switch (netIncomingMessage.MessageType)
        {
          case NetIncomingMessageType.StatusChanged:
            this.statusChanged(netIncomingMessage);
            continue;
          case NetIncomingMessageType.Data:
            this.parseDataMessageFromServer(netIncomingMessage);
            continue;
          case NetIncomingMessageType.DiscoveryResponse:
            if (!this.serverDiscovered)
            {
              Console.WriteLine("Found server at " + netIncomingMessage.SenderEndPoint?.ToString());
              if (this.validateProtocol(netIncomingMessage.ReadString()))
              {
                this.serverName = netIncomingMessage.ReadString();
                this.receiveHandshake(netIncomingMessage);
                this.serverDiscovered = true;
                continue;
              }
              this.connectionMessage = Game1.content.LoadString("Strings\\UI:CoopMenu_FailedProtocolVersion");
              this.client.Disconnect("");
              continue;
            }
            continue;
          case NetIncomingMessageType.DebugMessage:
          case NetIncomingMessageType.WarningMessage:
          case NetIncomingMessageType.ErrorMessage:
            string str = netIncomingMessage.ReadString();
            Console.WriteLine("{0}: {1}", (object) netIncomingMessage.MessageType, (object) str);
            Game1.debugOutput = str;
            continue;
          case NetIncomingMessageType.ConnectionLatencyUpdated:
            this.readLatency(netIncomingMessage);
            continue;
          default:
            continue;
        }
      }
      if (this.client.ServerConnection == null)
        return;
      now = DateTime.Now;
      if (now.Second % 2 != 0)
        return;
      Game1.debugOutput = "Ping: " + (this.client.ServerConnection.AverageRoundtripTime * 1000f).ToString() + "ms";
    }

    private void readLatency(NetIncomingMessage msg) => this.lastLatencyMs = msg.ReadFloat() * 1000f;

    private void receiveHandshake(NetIncomingMessage msg) => this.client.Connect(msg.SenderEndPoint.Address.ToString(), msg.SenderEndPoint.Port);

    private void statusChanged(NetIncomingMessage message)
    {
      NetConnectionStatus status = (NetConnectionStatus) message.ReadByte();
      switch (status)
      {
        case NetConnectionStatus.Disconnecting:
        case NetConnectionStatus.Disconnected:
          string message1 = message.ReadString();
          this.clientRemotelyDisconnected(status, message1);
          break;
      }
    }

    private void clientRemotelyDisconnected(NetConnectionStatus status, string message)
    {
      this.timedOut = true;
      if (status == NetConnectionStatus.Disconnected)
      {
        if (message == Multiplayer.kicked)
          this.pendingDisconnect = Multiplayer.DisconnectType.Kicked;
        else
          this.pendingDisconnect = Multiplayer.DisconnectType.LidgrenTimeout;
      }
      else
        this.pendingDisconnect = Multiplayer.DisconnectType.LidgrenDisconnect_Unknown;
    }

    public override void sendMessage(OutgoingMessage message)
    {
      NetOutgoingMessage message1 = this.client.CreateMessage();
      using (NetBufferWriteStream output = new NetBufferWriteStream((NetBuffer) message1))
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
          message.Write(writer);
      }
      int num = (int) this.client.SendMessage(message1, NetDeliveryMethod.ReliableOrdered);
      if (this.bandwidthLogger == null)
        return;
      this.bandwidthLogger.RecordBytesUp((long) message1.LengthBytes);
    }

    private void parseDataMessageFromServer(NetIncomingMessage dataMsg)
    {
      if (this.bandwidthLogger != null)
        this.bandwidthLogger.RecordBytesDown((long) dataMsg.LengthBytes);
      using (IncomingMessage message = new IncomingMessage())
      {
        using (NetBufferReadStream input = new NetBufferReadStream((NetBuffer) dataMsg))
        {
          using (BinaryReader reader = new BinaryReader((Stream) input))
          {
            while ((long) dataMsg.LengthBits - dataMsg.Position >= 8L)
            {
              message.Read(reader);
              this.processIncomingMessage(message);
            }
          }
        }
      }
    }
  }
}
