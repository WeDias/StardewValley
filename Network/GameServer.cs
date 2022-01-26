// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.GameServer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Network
{
  public class GameServer : IGameServer, IBandwidthMonitor
  {
    protected List<Server> servers = new List<Server>();
    private Dictionary<Action, Func<bool>> pendingGameAvailableActions = new Dictionary<Action, Func<bool>>();
    protected Dictionary<string, Action> _pendingFarmhandSelections = new Dictionary<string, Action>();
    private List<Action> completedPendingActions = new List<Action>();
    private List<string> bannedUsers = new List<string>();
    protected bool _wasConnected;
    protected bool _isLocalMultiplayerInitiatedServer;

    public GameServer(bool local_multiplayer = false)
    {
      this.servers.Add(Game1.multiplayer.InitServer((Server) new LidgrenServer((IGameServer) this)));
      this._isLocalMultiplayerInitiatedServer = local_multiplayer;
      if (this._isLocalMultiplayerInitiatedServer || Program.sdk.Networking == null)
        return;
      this.servers.Add(Program.sdk.Networking.CreateServer((IGameServer) this));
    }

    public int connectionsCount => this.servers.Sum<Server>((Func<Server, int>) (s => s.connectionsCount));

    public bool isConnectionActive(string connectionId)
    {
      foreach (Server server in this.servers)
      {
        if (server.isConnectionActive(connectionId))
          return true;
      }
      return false;
    }

    public virtual void onConnect(string connectionID) => this.UpdateLocalOnlyFlag();

    public virtual void onDisconnect(string connectionID)
    {
      if (this._pendingFarmhandSelections.ContainsKey(connectionID))
      {
        Console.WriteLine("Removed pending farmhand selection for invalidated connection " + connectionID);
        if (this.pendingGameAvailableActions.ContainsKey(this._pendingFarmhandSelections[connectionID]))
          this.pendingGameAvailableActions.Remove(this._pendingFarmhandSelections[connectionID]);
        this._pendingFarmhandSelections.Remove(connectionID);
      }
      this.UpdateLocalOnlyFlag();
    }

    public bool IsLocalMultiplayerInitiatedServer() => this._isLocalMultiplayerInitiatedServer;

    public virtual void UpdateLocalOnlyFlag()
    {
      if (!Game1.game1.IsMainInstance)
        return;
      bool flag = true;
      HashSet<long> local_clients = new HashSet<long>();
      GameRunner.instance.ExecuteForInstances((Action<Game1>) (instance =>
      {
        Client client = Game1.client;
        if (client == null && Game1.activeClickableMenu is FarmhandMenu)
          client = (Game1.activeClickableMenu as FarmhandMenu).client;
        if (!(client is LidgrenClient))
          return;
        local_clients.Add((client as LidgrenClient).client.UniqueIdentifier);
      }));
      foreach (Server server in this.servers)
      {
        if (server is LidgrenServer)
        {
          foreach (NetConnection connection in (server as LidgrenServer).server.Connections)
          {
            if (!local_clients.Contains(connection.RemoteUniqueIdentifier))
            {
              flag = false;
              break;
            }
          }
        }
        else if (server.connectionsCount > 0)
        {
          flag = false;
          break;
        }
        if (!flag)
          break;
      }
      if (Game1.hasLocalClientsOnly == flag)
        return;
      Game1.hasLocalClientsOnly = flag;
      if (Game1.hasLocalClientsOnly)
        Console.WriteLine("Game has only local clients.");
      else
        Console.WriteLine("Game has remote clients.");
    }

    public string getInviteCode()
    {
      foreach (Server server in this.servers)
      {
        string inviteCode = server.getInviteCode();
        if (inviteCode != null)
          return inviteCode;
      }
      return (string) null;
    }

    public string getUserName(long farmerId)
    {
      foreach (Server server in this.servers)
      {
        string userName = server.getUserName(farmerId);
        if (userName != null)
          return userName;
      }
      return (string) null;
    }

    public float getPingToClient(long farmerId)
    {
      foreach (Server server in this.servers)
      {
        if ((double) server.getPingToClient(farmerId) != -1.0)
          return server.getPingToClient(farmerId);
      }
      return -1f;
    }

    protected void initialize()
    {
      foreach (Server server in this.servers)
        server.initialize();
      this.whenGameAvailable(new Action(this.updateLobbyData), (Func<bool>) null);
    }

    public void setPrivacy(ServerPrivacy privacy)
    {
      foreach (Server server in this.servers)
        server.setPrivacy(privacy);
      if (!((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState != (NetRef<IWorldState>) null) || Game1.netWorldState.Value == null)
        return;
      Game1.netWorldState.Value.ServerPrivacy = privacy;
    }

    public void stopServer()
    {
      if (Game1.chatBox != null)
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_DisablingServer"));
      foreach (Server server in this.servers)
        server.stopServer();
    }

    public void receiveMessages()
    {
      foreach (Server server in this.servers)
        server.receiveMessages();
      this.completedPendingActions.Clear();
      foreach (Action key in this.pendingGameAvailableActions.Keys)
      {
        if (this.pendingGameAvailableActions[key]())
        {
          key();
          this.completedPendingActions.Add(key);
        }
      }
      foreach (Action completedPendingAction in this.completedPendingActions)
        this.pendingGameAvailableActions.Remove(completedPendingAction);
      this.completedPendingActions.Clear();
      if (Game1.chatBox == null)
        return;
      bool flag = this.anyServerConnected();
      if (this._wasConnected == flag)
        return;
      this._wasConnected = flag;
      if (!this._wasConnected)
        return;
      Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_StartingServer"));
    }

    public void sendMessage(long peerId, OutgoingMessage message)
    {
      foreach (Server server in this.servers)
        server.sendMessage(peerId, message);
    }

    public bool canAcceptIPConnections() => this.servers.Select<Server, bool>((Func<Server, bool>) (s => s.canAcceptIPConnections())).Aggregate<bool, bool>(false, (Func<bool, bool, bool>) ((a, b) => a | b));

    public bool canOfferInvite() => this.servers.Select<Server, bool>((Func<Server, bool>) (s => s.canOfferInvite())).Aggregate<bool, bool>(false, (Func<bool, bool, bool>) ((a, b) => a | b));

    public void offerInvite()
    {
      foreach (Server server in this.servers)
      {
        if (server.canOfferInvite())
          server.offerInvite();
      }
    }

    public bool anyServerConnected()
    {
      foreach (Server server in this.servers)
      {
        if (server.connected())
          return true;
      }
      return false;
    }

    public bool connected()
    {
      foreach (Server server in this.servers)
      {
        if (!server.connected())
          return false;
      }
      return true;
    }

    public void sendMessage(
      long peerId,
      byte messageType,
      Farmer sourceFarmer,
      params object[] data)
    {
      this.sendMessage(peerId, new OutgoingMessage(messageType, sourceFarmer, data));
    }

    public void sendMessages()
    {
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      {
        foreach (OutgoingMessage message in (IEnumerable<OutgoingMessage>) farmer.messageQueue)
          this.sendMessage(farmer.UniqueMultiplayerID, message);
        farmer.messageQueue.Clear();
      }
    }

    public void startServer()
    {
      this._wasConnected = false;
      Console.WriteLine("Starting server. Protocol version: 1.5.5");
      this.initialize();
      if ((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState == (NetRef<IWorldState>) null)
        Game1.netWorldState = new NetRoot<IWorldState>((IWorldState) new NetWorldState());
      Game1.netWorldState.Clock.InterpolationTicks = 0;
      Game1.netWorldState.Value.UpdateFromGame1();
    }

    public void initializeHost()
    {
      if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null)
        Game1.serverHost = new NetFarmerRoot();
      Game1.serverHost.Value = Game1.player;
      foreach (Server server in this.servers)
      {
        if (server.PopulatePlatformData(Game1.player))
          break;
      }
      Game1.serverHost.MarkClean();
      Game1.serverHost.Clock.InterpolationTicks = Game1.multiplayer.defaultInterpolationTicks;
    }

    public void sendServerIntroduction(long peer)
    {
      this.sendMessage(peer, new OutgoingMessage((byte) 1, Game1.serverHost.Value, new object[3]
      {
        (object) Game1.multiplayer.writeObjectFullBytes<Farmer>((NetRoot<Farmer>) Game1.serverHost, new long?(peer)),
        (object) Game1.multiplayer.writeObjectFullBytes<FarmerTeam>(Game1.player.teamRoot, new long?(peer)),
        (object) Game1.multiplayer.writeObjectFullBytes<IWorldState>(Game1.netWorldState, new long?(peer))
      }));
      foreach (KeyValuePair<long, NetRoot<Farmer>> root in Game1.otherFarmers.Roots)
      {
        if (root.Key != Game1.player.UniqueMultiplayerID && root.Key != peer)
          this.sendMessage(peer, new OutgoingMessage((byte) 2, root.Value.Value, new object[2]
          {
            (object) this.getUserName(root.Value.Value.UniqueMultiplayerID),
            (object) Game1.multiplayer.writeObjectFullBytes<Farmer>(root.Value, new long?(peer))
          }));
      }
    }

    public void kick(long disconnectee)
    {
      foreach (Server server in this.servers)
        server.kick(disconnectee);
    }

    public string ban(long farmerId)
    {
      string key = (string) null;
      foreach (Server server in this.servers)
      {
        key = server.getUserId(farmerId);
        if (key != null)
          break;
      }
      if (key == null || Game1.bannedUsers.ContainsKey(key))
        return (string) null;
      string str = Game1.multiplayer.getUserName(farmerId);
      if (str == "" || str == key)
        str = (string) null;
      Game1.bannedUsers.Add(key, str);
      this.kick(farmerId);
      return key;
    }

    public void playerDisconnected(long disconnectee)
    {
      Farmer sourceFarmer = (Farmer) null;
      Game1.otherFarmers.TryGetValue(disconnectee, out sourceFarmer);
      Game1.multiplayer.playerDisconnected(disconnectee);
      if (sourceFarmer == null)
        return;
      OutgoingMessage message = new OutgoingMessage((byte) 19, sourceFarmer, Array.Empty<object>());
      foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
      {
        if (key != disconnectee)
          this.sendMessage(key, message);
      }
    }

    public bool isGameAvailable()
    {
      bool flag1 = Game1.currentMinigame is Intro || Game1.Date.DayOfMonth == 0;
      bool flag2 = Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding;
      bool flag3 = Game1.newDaySync != null && !Game1.newDaySync.hasFinished();
      bool flag4 = Game1.player.team.demolishLock.IsLocked();
      return !Game1.isFestival() && !flag2 && !flag1 && !flag3 && !flag4 && Game1.weddingsToday.Count == 0 && Game1.gameMode != (byte) 6;
    }

    public bool whenGameAvailable(Action action, Func<bool> customAvailabilityCheck = null)
    {
      Func<bool> func = customAvailabilityCheck != null ? customAvailabilityCheck : new Func<bool>(this.isGameAvailable);
      if (func())
      {
        action();
        return true;
      }
      this.pendingGameAvailableActions.Add(action, func);
      return false;
    }

    private void rejectFarmhandRequest(
      string userID,
      NetFarmerRoot farmer,
      Action<OutgoingMessage> sendMessage)
    {
      this.sendAvailableFarmhands(userID, sendMessage);
      Console.WriteLine("Rejected request for farmhand " + (farmer.Value != null ? farmer.Value.UniqueMultiplayerID.ToString() : "???"));
    }

    private IEnumerable<Cabin> cabins()
    {
      if (Game1.getFarm() != null)
      {
        foreach (Building building in Game1.getFarm().buildings)
        {
          if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0 && building.indoors.Value is Cabin)
            yield return building.indoors.Value as Cabin;
        }
      }
    }

    public bool isUserBanned(string userID) => Game1.bannedUsers.ContainsKey(userID);

    private bool authCheck(string userID, Farmer farmhand)
    {
      if (!Game1.options.enableFarmhandCreation && !this.IsLocalMultiplayerInitiatedServer() && !(bool) (NetFieldBase<bool, NetBool>) farmhand.isCustomized)
        return false;
      return userID == "" || farmhand.userID.Value == "" || farmhand.userID.Value == userID;
    }

    private Cabin findCabin(Farmer farmhand)
    {
      foreach (Cabin cabin in this.cabins())
      {
        if (cabin.getFarmhand().Value.UniqueMultiplayerID == farmhand.UniqueMultiplayerID)
          return cabin;
      }
      return (Cabin) null;
    }

    private Farmer findOriginalFarmhand(Farmer farmhand) => this.findCabin(farmhand)?.getFarmhand().Value;

    public void checkFarmhandRequest(
      string userID,
      string connectionID,
      NetFarmerRoot farmer,
      Action<OutgoingMessage> sendMessage,
      Action approve)
    {
      if (farmer.Value == null)
      {
        this.rejectFarmhandRequest(userID, farmer, sendMessage);
      }
      else
      {
        long id = farmer.Value.UniqueMultiplayerID;
        Action action = (Action) (() =>
        {
          if (this._pendingFarmhandSelections.ContainsKey(connectionID))
            this._pendingFarmhandSelections.Remove(connectionID);
          Farmer originalFarmhand = this.findOriginalFarmhand(farmer.Value);
          if (!this.isConnectionActive(connectionID))
            Console.WriteLine("Rejected request for connection ID " + connectionID + ": Connection not active.");
          else if (originalFarmhand == null)
          {
            Console.WriteLine("Rejected request for farmhand " + id.ToString() + ": doesn't exist");
            this.rejectFarmhandRequest(userID, farmer, sendMessage);
          }
          else if (!this.authCheck(userID, originalFarmhand))
          {
            Console.WriteLine("Rejected request for farmhand " + id.ToString() + ": authorization failure " + userID + " " + originalFarmhand.userID.Value);
            this.rejectFarmhandRequest(userID, farmer, sendMessage);
          }
          else if (Game1.otherFarmers.ContainsKey(id) && !Game1.multiplayer.isDisconnecting(id) || Game1.serverHost.Value.UniqueMultiplayerID == id)
          {
            Console.WriteLine("Rejected request for farmhand " + id.ToString() + ": already in use");
            this.rejectFarmhandRequest(userID, farmer, sendMessage);
          }
          else if (this.findCabin(farmer.Value).isInventoryOpen())
          {
            Console.WriteLine("Rejected request for farmhand " + id.ToString() + ": inventory in use");
            this.rejectFarmhandRequest(userID, farmer, sendMessage);
          }
          else
          {
            Console.WriteLine("Approved request for farmhand " + id.ToString());
            approve();
            Game1.updateCellarAssignments();
            Game1.multiplayer.addPlayer(farmer);
            Game1.multiplayer.broadcastPlayerIntroduction(farmer);
            this.sendLocation(id, (GameLocation) Game1.getFarm());
            this.sendLocation(id, Game1.getLocationFromName("FarmHouse"));
            this.sendLocation(id, Game1.getLocationFromName("Greenhouse"));
            if ((NetFieldBase<string, NetString>) farmer.Value.lastSleepLocation != (NetString) null)
            {
              GameLocation locationFromName = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) farmer.Value.lastSleepLocation);
              if (locationFromName != null && Game1.isLocationAccessible(locationFromName.Name) && !Game1.multiplayer.isAlwaysActiveLocation(locationFromName))
                this.sendLocation(id, locationFromName, true);
            }
            this.sendServerIntroduction(id);
            this.updateLobbyData();
          }
        });
        if (this.whenGameAvailable(action, (Func<bool>) null))
          return;
        this._pendingFarmhandSelections[connectionID] = action;
        Console.WriteLine("Postponing request for farmhand " + id.ToString() + " from connection: " + connectionID);
        sendMessage(new OutgoingMessage((byte) 11, Game1.player, new object[1]
        {
          (object) "Strings\\UI:Client_WaitForHostAvailability"
        }));
      }
    }

    public void sendAvailableFarmhands(string userID, Action<OutgoingMessage> sendMessage)
    {
      List<NetRef<Farmer>> netRefList = new List<NetRef<Farmer>>();
      Game1.getFarm();
      foreach (Cabin cabin in this.cabins())
      {
        NetRef<Farmer> farmhand = cabin.getFarmhand();
        if ((!farmhand.Value.isActive() || Game1.multiplayer.isDisconnecting(farmhand.Value.UniqueMultiplayerID)) && !cabin.isInventoryOpen())
          netRefList.Add(farmhand);
      }
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
        {
          writer.Write(Game1.year);
          writer.Write(Utility.getSeasonNumber(Game1.currentSeason));
          writer.Write(Game1.dayOfMonth);
          writer.Write((byte) netRefList.Count);
          foreach (NetRef<Farmer> netRef in netRefList)
          {
            try
            {
              netRef.Serializer = SaveGame.farmerSerializer;
              netRef.WriteFull(writer);
            }
            finally
            {
              netRef.Serializer = (XmlSerializer) null;
            }
          }
          output.Seek(0L, SeekOrigin.Begin);
          sendMessage(new OutgoingMessage((byte) 9, Game1.player, new object[1]
          {
            (object) output.ToArray()
          }));
        }
      }
    }

    public T GetServer<T>() where T : Server
    {
      foreach (Server server in this.servers)
      {
        if (server is T)
          return server as T;
      }
      return default (T);
    }

    private void sendLocation(long peer, GameLocation location, bool force_current = false) => this.sendMessage(peer, (byte) 3, Game1.serverHost.Value, new object[2]
    {
      (object) force_current,
      (object) Game1.multiplayer.writeObjectFullBytes<GameLocation>(Game1.multiplayer.locationRoot(location), new long?(peer))
    });

    private void warpFarmer(Farmer farmer, short x, short y, string name, bool isStructure)
    {
      GameLocation locationFromName = Game1.getLocationFromName(name, isStructure);
      if (Game1.IsMasterGame)
        locationFromName.hostSetup();
      farmer.currentLocation = locationFromName;
      farmer.Position = new Vector2((float) ((int) x * 64), (float) ((int) y * 64 - (farmer.Sprite.getHeight() - 32) + 16));
      this.sendLocation(farmer.UniqueMultiplayerID, locationFromName);
    }

    public void processIncomingMessage(IncomingMessage message)
    {
      switch (message.MessageType)
      {
        case 2:
          message.Reader.ReadString();
          Game1.multiplayer.processIncomingMessage(message);
          break;
        case 5:
          short x = message.Reader.ReadInt16();
          short y = message.Reader.ReadInt16();
          string name = message.Reader.ReadString();
          bool isStructure = message.Reader.ReadByte() == (byte) 1;
          this.warpFarmer(message.SourceFarmer, x, y, name, isStructure);
          break;
        case 10:
          long peerID = message.Reader.ReadInt64();
          message.Reader.BaseStream.Position -= 8L;
          if (peerID == Multiplayer.AllPlayers || peerID == Game1.player.UniqueMultiplayerID)
            Game1.multiplayer.processIncomingMessage(message);
          this.rebroadcastClientMessage(message, peerID);
          break;
        default:
          Game1.multiplayer.processIncomingMessage(message);
          break;
      }
      if (!Game1.multiplayer.isClientBroadcastType(message.MessageType))
        return;
      this.rebroadcastClientMessage(message, Multiplayer.AllPlayers);
    }

    private void rebroadcastClientMessage(IncomingMessage message, long peerID)
    {
      OutgoingMessage message1 = new OutgoingMessage(message);
      foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
      {
        if (key != message.FarmerID && (peerID == Multiplayer.AllPlayers || key == peerID))
          this.sendMessage(key, message1);
      }
    }

    private void setLobbyData(string key, string value)
    {
      foreach (Server server in this.servers)
        server.setLobbyData(key, value);
    }

    private bool unclaimedFarmhandsExist()
    {
      foreach (Cabin cabin in this.cabins())
      {
        if (cabin.farmhand.Value == null || cabin.farmhand.Value.userID.Value == "")
          return true;
      }
      return false;
    }

    public void updateLobbyData()
    {
      this.setLobbyData("farmName", Game1.player.farmName.Value);
      this.setLobbyData("farmType", Convert.ToString(Game1.whichFarm));
      if (Game1.whichFarm == 7)
        this.setLobbyData("modFarmType", Game1.GetFarmTypeID());
      else
        this.setLobbyData("modFarmType", "");
      this.setLobbyData("date", Convert.ToString(new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth).TotalDays));
      this.setLobbyData("farmhands", string.Join(",", Game1.getAllFarmhands().Select<Farmer, string>((Func<Farmer, string>) (farmhand => farmhand.userID.Value)).Where<string>((Func<string, bool>) (user => user != ""))));
      this.setLobbyData("newFarmhands", Convert.ToString(Game1.options.enableFarmhandCreation && this.unclaimedFarmhandsExist()));
    }

    public BandwidthLogger BandwidthLogger
    {
      get
      {
        foreach (Server server in this.servers)
        {
          if (server.connectionsCount > 0)
            return server.BandwidthLogger;
        }
        return (BandwidthLogger) null;
      }
    }

    public bool LogBandwidth
    {
      get
      {
        foreach (Server server in this.servers)
        {
          if (server.connectionsCount > 0)
            return server.LogBandwidth;
        }
        return false;
      }
      set
      {
        foreach (Server server in this.servers)
        {
          if (server.connectionsCount > 0)
          {
            server.LogBandwidth = value;
            break;
          }
        }
      }
    }
  }
}
