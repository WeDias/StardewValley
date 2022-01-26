// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Server
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace StardewValley.Network
{
  public abstract class Server : IBandwidthMonitor
  {
    protected IGameServer gameServer;
    protected BandwidthLogger bandwidthLogger;

    public Server(IGameServer gameServer) => this.gameServer = gameServer;

    public abstract int connectionsCount { get; }

    public abstract void initialize();

    public abstract void setPrivacy(ServerPrivacy privacy);

    public abstract void stopServer();

    public abstract void receiveMessages();

    public abstract void sendMessage(long peerId, OutgoingMessage message);

    public abstract bool connected();

    public virtual bool canAcceptIPConnections() => false;

    public virtual bool canOfferInvite() => false;

    public virtual void offerInvite()
    {
    }

    public virtual string getInviteCode() => (string) null;

    public virtual bool PopulatePlatformData(Farmer farmer) => false;

    public virtual string getUserId(long farmerId) => (string) null;

    public virtual bool hasUserId(string userId) => false;

    public virtual float getPingToClient(long farmerId) => 0.0f;

    public virtual bool isConnectionActive(string connectionId) => throw new NotImplementedException();

    public virtual void onConnect(string connectionId) => this.gameServer.onConnect(connectionId);

    public virtual void onDisconnect(string connectionId) => this.gameServer.onDisconnect(connectionId);

    public abstract string getUserName(long farmerId);

    public abstract void setLobbyData(string key, string value);

    public virtual void kick(long disconnectee)
    {
    }

    public virtual void playerDisconnected(long disconnectee) => this.gameServer.playerDisconnected(disconnectee);

    public bool LogBandwidth
    {
      get => this.bandwidthLogger != null;
      set
      {
        if (value)
          this.bandwidthLogger = new BandwidthLogger();
        else
          this.bandwidthLogger = (BandwidthLogger) null;
      }
    }

    public BandwidthLogger BandwidthLogger => this.bandwidthLogger;
  }
}
