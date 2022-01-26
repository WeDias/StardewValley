// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GalaxySocket
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Galaxy.Api;
using StardewValley.Network;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewValley.SDKs
{
  public class GalaxySocket
  {
    public const long Timeout = 30000;
    private const int SendMaxPacketSize = 1100;
    private const int ReceiveMaxPacketSize = 1300;
    private const long RecreateLobbyDelay = 20000;
    private const long HeartbeatDelay = 15000;
    private const byte HeartbeatMessage = 255;
    public bool isRecreatedLobby;
    public bool isFirstRecreateAttempt;
    private GalaxyID selfId;
    private GalaxyID connectingLobbyID;
    private GalaxyID lobby;
    private GalaxyID lobbyOwner;
    private GalaxySocket.GalaxyLobbyEnteredListener galaxyLobbyEnterCallback;
    private GalaxySocket.GalaxyLobbyCreatedListener galaxyLobbyCreatedCallback;
    private GalaxySocket.GalaxyLobbyLeftListener galaxyLobbyLeftCallback;
    private string protocolVersion;
    private Dictionary<string, string> lobbyData = new Dictionary<string, string>();
    private ServerPrivacy privacy;
    private uint memberLimit;
    private long recreateTimer;
    private long heartbeatTimer;
    private Dictionary<ulong, GalaxyID> connections = new Dictionary<ulong, GalaxyID>();
    private HashSet<ulong> ghosts = new HashSet<ulong>();
    private Dictionary<ulong, MemoryStream> incompletePackets = new Dictionary<ulong, MemoryStream>();
    private Dictionary<ulong, long> lastMessageTime = new Dictionary<ulong, long>();
    private CSteamID? steamLobby;
    private Steamworks.Callback<LobbyEnter_t> steamLobbyEnterCallback;

    public int ConnectionCount => this.connections.Count;

    public IEnumerable<GalaxyID> Connections => (IEnumerable<GalaxyID>) this.connections.Values;

    public bool Connected => this.lobby != (GalaxyID) null;

    public GalaxyID LobbyOwner => this.lobbyOwner;

    public GalaxyID Lobby => this.lobby;

    public ulong? InviteDialogLobby => !this.steamLobby.HasValue ? new ulong?() : new ulong?(this.steamLobby.Value.m_SteamID);

    public GalaxySocket(string protocolVersion)
    {
      this.protocolVersion = protocolVersion;
      this.lobbyData[nameof (protocolVersion)] = protocolVersion;
      this.selfId = GalaxyInstance.User().GetGalaxyID();
      this.galaxyLobbyEnterCallback = new GalaxySocket.GalaxyLobbyEnteredListener(new Action<GalaxyID, LobbyEnterResult>(this.onGalaxyLobbyEnter));
      this.galaxyLobbyCreatedCallback = new GalaxySocket.GalaxyLobbyCreatedListener(new Action<GalaxyID, LobbyCreateResult>(this.onGalaxyLobbyCreated));
    }

    public string GetInviteCode() => this.lobby == (GalaxyID) null ? (string) null : Base36.Encode(this.lobby.GetRealID());

    private string getConnectionString() => this.lobby == (GalaxyID) null ? "" : "-connect-lobby-" + this.lobby.ToUint64().ToString();

    private long getTimeNow() => DateTime.Now.Ticks / 10000L;

    public long GetPingWith(GalaxyID peer)
    {
      long num = 0;
      this.lastMessageTime.TryGetValue(peer.ToUint64(), out num);
      if (num == 0L)
        return 0;
      return this.getTimeNow() - num > 30000L ? long.MaxValue : (long) GalaxyInstance.Networking().GetPingWith(peer);
    }

    private LobbyType privacyToLobbyType(ServerPrivacy privacy)
    {
      if (privacy == ServerPrivacy.InviteOnly)
        return LobbyType.LOBBY_TYPE_PRIVATE;
      if (privacy == ServerPrivacy.FriendsOnly)
        return LobbyType.LOBBY_TYPE_FRIENDS_ONLY;
      if (privacy == ServerPrivacy.Public)
        return LobbyType.LOBBY_TYPE_PUBLIC;
      throw new ArgumentException(Convert.ToString((object) privacy));
    }

    private ELobbyType privacyToSteamLobbyType(ServerPrivacy privacy)
    {
      if (privacy == ServerPrivacy.InviteOnly)
        return ELobbyType.k_ELobbyTypePrivate;
      if (privacy == ServerPrivacy.FriendsOnly)
        return ELobbyType.k_ELobbyTypeFriendsOnly;
      if (privacy == ServerPrivacy.Public)
        return ELobbyType.k_ELobbyTypePublic;
      throw new ArgumentException(Convert.ToString((object) privacy));
    }

    public void SetPrivacy(ServerPrivacy privacy)
    {
      this.privacy = privacy;
      this.updateLobbyPrivacy();
    }

    public void CreateLobby(ServerPrivacy privacy, uint memberLimit)
    {
      this.privacy = privacy;
      this.memberLimit = memberLimit;
      this.lobbyOwner = this.selfId;
      this.isRecreatedLobby = false;
      this.tryCreateLobby();
    }

    private void tryCreateLobby()
    {
      Console.WriteLine("Creating lobby...");
      if (this.galaxyLobbyLeftCallback != null)
      {
        this.galaxyLobbyLeftCallback.Dispose();
        this.galaxyLobbyLeftCallback = (GalaxySocket.GalaxyLobbyLeftListener) null;
      }
      this.galaxyLobbyLeftCallback = new GalaxySocket.GalaxyLobbyLeftListener(new Action<GalaxyID, ILobbyLeftListener.LobbyLeaveReason>(this.onGalaxyLobbyLeft));
      GalaxyInstance.Matchmaking().CreateLobby(this.privacyToLobbyType(this.privacy), this.memberLimit, true, LobbyTopologyType.LOBBY_TOPOLOGY_TYPE_STAR);
      this.recreateTimer = 0L;
    }

    public void JoinLobby(GalaxyID lobbyId, Action<string> onError)
    {
      try
      {
        this.connectingLobbyID = lobbyId;
        GalaxyInstance.Matchmaking().JoinLobby(this.connectingLobbyID);
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        string str1 = Game1.content.LoadString("Strings\\UI:CoopMenu_Failed");
        string str2 = !ex.Message.EndsWith("already joined this lobby") ? str1 + " (" + ex.Message + ")" : str1 + " (already connected)";
        onError(str2);
        this.Close();
      }
    }

    public void SetLobbyData(string key, string value)
    {
      this.lobbyData[key] = value;
      if (!(this.lobby != (GalaxyID) null))
        return;
      GalaxyInstance.Matchmaking().SetLobbyData(this.lobby, key, value);
    }

    private void updateLobbyPrivacy()
    {
      if (this.lobbyOwner != this.selfId)
        return;
      if (this.lobby != (GalaxyID) null)
        GalaxyInstance.Matchmaking().SetLobbyType(this.lobby, this.privacyToLobbyType(this.privacy));
      if (this.lobby == (GalaxyID) null)
      {
        if (!this.steamLobby.HasValue)
          return;
        SteamMatchmaking.LeaveLobby(this.steamLobby.Value);
      }
      else if (!this.steamLobby.HasValue)
      {
        if (this.steamLobbyEnterCallback == null)
          this.steamLobbyEnterCallback = Steamworks.Callback<LobbyEnter_t>.Create(new Steamworks.Callback<LobbyEnter_t>.DispatchDelegate(this.onSteamLobbyEnter));
        SteamMatchmaking.CreateLobby(this.privacyToSteamLobbyType(this.privacy), (int) this.memberLimit);
      }
      else
      {
        SteamMatchmaking.SetLobbyType(this.steamLobby.Value, this.privacyToSteamLobbyType(this.privacy));
        SteamMatchmaking.SetLobbyData(this.steamLobby.Value, "connect", this.getConnectionString());
      }
    }

    private void onGalaxyLobbyCreated(GalaxyID lobbyID, LobbyCreateResult result)
    {
      if (result != LobbyCreateResult.LOBBY_CREATE_RESULT_ERROR)
        return;
      Console.WriteLine("Failed to create lobby.");
      if (Game1.chatBox != null && this.isFirstRecreateAttempt)
      {
        if (this.isRecreatedLobby)
          Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyCreateFail"));
        else
          Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyCreateFail"));
      }
      this.recreateTimer = this.getTimeNow() + 20000L;
      this.isRecreatedLobby = true;
      this.isFirstRecreateAttempt = false;
    }

    private void onGalaxyLobbyLeft(
      GalaxyID lobbyID,
      ILobbyLeftListener.LobbyLeaveReason leaveReason)
    {
      if (leaveReason != ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT)
        Program.WriteLog(Program.LogType.Disconnect, "Forcibly left Galaxy lobby at " + DateTime.Now.ToLongTimeString() + " - " + leaveReason.ToString(), true);
      if (Game1.chatBox != null)
      {
        string sub1 = "";
        switch (leaveReason)
        {
          case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT:
            sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_UserLeft");
            break;
          case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_LOBBY_CLOSED:
            sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_LobbyClosed");
            break;
          case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_CONNECTION_LOST:
            sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_ConnectionLost");
            break;
        }
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyLost", (object) sub1).Trim());
      }
      Console.WriteLine("Left lobby {0} - leaveReason: {1}", (object) lobbyID.ToUint64(), (object) leaveReason);
      this.lobby = (GalaxyID) null;
      this.recreateTimer = this.getTimeNow() + 20000L;
      this.isRecreatedLobby = true;
      this.isFirstRecreateAttempt = true;
    }

    private void onGalaxyLobbyEnter(GalaxyID lobbyID, LobbyEnterResult result)
    {
      this.connectingLobbyID = (GalaxyID) null;
      if (result != LobbyEnterResult.LOBBY_ENTER_RESULT_SUCCESS)
        return;
      Console.WriteLine("Lobby entered: {0}", (object) lobbyID.ToUint64());
      this.lobby = lobbyID;
      this.lobbyOwner = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
      if (Game1.chatBox != null)
      {
        string sub1 = "";
        if (Program.sdk.Networking != null && Program.sdk.Networking.SupportsInviteCodes())
          sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyJoined_InviteCode", (object) this.GetInviteCode());
        if (this.isRecreatedLobby)
          Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyRecreated", (object) sub1).Trim());
        else
          Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyJoined", (object) sub1).Trim());
      }
      if (!(this.lobbyOwner == this.selfId))
        return;
      foreach (KeyValuePair<string, string> keyValuePair in this.lobbyData)
        GalaxyInstance.Matchmaking().SetLobbyData(this.lobby, keyValuePair.Key, keyValuePair.Value);
      this.updateLobbyPrivacy();
    }

    private void onSteamLobbyEnter(LobbyEnter_t pCallback)
    {
      if (pCallback.m_EChatRoomEnterResponse != 1U)
        return;
      Console.WriteLine("Steam lobby entered: {0}", (object) pCallback.m_ulSteamIDLobby);
      this.steamLobbyEnterCallback.Unregister();
      this.steamLobbyEnterCallback = (Steamworks.Callback<LobbyEnter_t>) null;
      this.steamLobby = new CSteamID?(new CSteamID(pCallback.m_ulSteamIDLobby));
      if (!(SteamMatchmaking.GetLobbyOwner(this.steamLobby.Value) == SteamUser.GetSteamID()))
        return;
      SteamMatchmaking.SetLobbyType(this.steamLobby.Value, this.privacyToSteamLobbyType(this.privacy));
      SteamMatchmaking.SetLobbyData(this.steamLobby.Value, "connect", this.getConnectionString());
    }

    public IEnumerable<GalaxyID> LobbyMembers()
    {
      if (!(this.lobby == (GalaxyID) null))
      {
        uint lobby_members_count = 0;
        try
        {
          lobby_members_count = GalaxyInstance.Matchmaking().GetNumLobbyMembers(this.lobby);
        }
        catch (Exception ex)
        {
          yield break;
        }
        for (uint i = 0; i < lobby_members_count; ++i)
        {
          GalaxyID lobbyMemberByIndex = GalaxyInstance.Matchmaking().GetLobbyMemberByIndex(this.lobby, i);
          if (!(lobbyMemberByIndex == this.selfId) && !this.ghosts.Contains(lobbyMemberByIndex.ToUint64()))
            yield return lobbyMemberByIndex;
        }
      }
    }

    private bool lobbyContains(GalaxyID user)
    {
      foreach (GalaxyID lobbyMember in this.LobbyMembers())
      {
        if (user == lobbyMember || this.ghosts.Contains(lobbyMember.ToUint64()))
          return true;
      }
      return false;
    }

    private void close(GalaxyID peer)
    {
      this.connections.Remove(peer.ToUint64());
      this.incompletePackets.Remove(peer.ToUint64());
    }

    public void Kick(GalaxyID user) => this.ghosts.Add(user.ToUint64());

    public void Close()
    {
      if (this.connectingLobbyID != (GalaxyID) null)
      {
        GalaxyInstance.Matchmaking().LeaveLobby(this.connectingLobbyID);
        this.connectingLobbyID = (GalaxyID) null;
      }
      if (this.lobby != (GalaxyID) null)
      {
        while (this.ConnectionCount > 0)
          this.close(this.Connections.First<GalaxyID>());
        GalaxyInstance.Matchmaking().LeaveLobby(this.lobby);
        this.lobby = (GalaxyID) null;
      }
      this.updateLobbyPrivacy();
      try
      {
        this.galaxyLobbyEnterCallback.Dispose();
      }
      catch (Exception ex)
      {
      }
      try
      {
        this.galaxyLobbyCreatedCallback.Dispose();
      }
      catch (Exception ex)
      {
      }
      if (this.galaxyLobbyLeftCallback == null)
        return;
      this.galaxyLobbyLeftCallback.Dispose();
    }

    public void Receive(
      Action<GalaxyID> onConnection,
      Action<GalaxyID, Stream> onMessage,
      Action<GalaxyID> onDisconnect,
      Action<string> onError)
    {
      long timeNow = this.getTimeNow();
      if (this.lobby == (GalaxyID) null)
      {
        if (this.lobbyOwner == this.selfId && this.recreateTimer > 0L && this.recreateTimer <= timeNow)
        {
          this.recreateTimer = 0L;
          this.tryCreateLobby();
        }
        this.DisconnectPeers(onDisconnect);
      }
      else
      {
        try
        {
          string lobbyData = GalaxyInstance.Matchmaking().GetLobbyData(this.lobby, "protocolVersion");
          if (lobbyData != "")
          {
            if (lobbyData != this.protocolVersion)
            {
              onError(Game1.content.LoadString("Strings\\UI:CoopMenu_FailedProtocolVersion"));
              this.Close();
              return;
            }
          }
        }
        catch (Exception ex)
        {
        }
        foreach (GalaxyID lobbyMember in this.LobbyMembers())
        {
          if (!this.connections.ContainsKey(lobbyMember.ToUint64()) && !this.ghosts.Contains(lobbyMember.ToUint64()))
          {
            this.connections.Add(lobbyMember.ToUint64(), lobbyMember);
            onConnection(lobbyMember);
          }
        }
        this.ghosts.IntersectWith(this.LobbyMembers().Select<GalaxyID, ulong>((Func<GalaxyID, ulong>) (peer => peer.ToUint64())));
        byte[] numArray = new byte[1300];
        uint outMsgSize = 1300;
        GalaxyID outGalaxyID = new GalaxyID();
        while (GalaxyInstance.Networking().ReadP2PPacket(numArray, (uint) numArray.Length, ref outMsgSize, ref outGalaxyID))
        {
          this.lastMessageTime[outGalaxyID.ToUint64()] = timeNow;
          if (this.connections.ContainsKey(outGalaxyID.ToUint64()) && numArray[0] != byte.MaxValue)
          {
            bool flag = numArray[0] == (byte) 1;
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(numArray, 4, (int) outMsgSize - 4);
            if (this.incompletePackets.ContainsKey(outGalaxyID.ToUint64()))
            {
              memoryStream.Position = 0L;
              memoryStream.CopyTo((Stream) this.incompletePackets[outGalaxyID.ToUint64()]);
              if (!flag)
              {
                MemoryStream incompletePacket = this.incompletePackets[outGalaxyID.ToUint64()];
                this.incompletePackets.Remove(outGalaxyID.ToUint64());
                incompletePacket.Position = 0L;
                onMessage(outGalaxyID, (Stream) incompletePacket);
              }
            }
            else if (flag)
            {
              memoryStream.Position = memoryStream.Length;
              this.incompletePackets[outGalaxyID.ToUint64()] = memoryStream;
            }
            else
            {
              memoryStream.Position = 0L;
              onMessage(outGalaxyID, (Stream) memoryStream);
            }
          }
        }
        this.DisconnectPeers(onDisconnect);
      }
    }

    public virtual void DisconnectPeers(Action<GalaxyID> onDisconnect)
    {
      List<GalaxyID> galaxyIdList = new List<GalaxyID>();
      foreach (GalaxyID user in this.connections.Values)
      {
        if (this.lobby == (GalaxyID) null || !this.lobbyContains(user) || this.ghosts.Contains(user.ToUint64()))
          galaxyIdList.Add(user);
      }
      foreach (GalaxyID peer in galaxyIdList)
      {
        onDisconnect(peer);
        this.close(peer);
      }
    }

    public void Heartbeat(IEnumerable<GalaxyID> peers)
    {
      long timeNow = this.getTimeNow();
      if (this.heartbeatTimer > timeNow)
        return;
      this.heartbeatTimer = timeNow + 15000L;
      byte[] data = new byte[1]{ byte.MaxValue };
      foreach (GalaxyID peer in peers)
        GalaxyInstance.Networking().SendP2PPacket(peer, data, (uint) data.Length, P2PSendType.P2P_SEND_RELIABLE);
    }

    public void Send(GalaxyID peer, byte[] data)
    {
      if (!this.connections.ContainsKey(peer.ToUint64()))
        return;
      if (data.Length <= 1100)
      {
        byte[] data1 = new byte[data.Length + 4];
        data.CopyTo((Array) data1, 4);
        GalaxyInstance.Networking().SendP2PPacket(peer, data1, (uint) data1.Length, P2PSendType.P2P_SEND_RELIABLE);
      }
      else
      {
        int num = 1096;
        int srcOffset = 0;
        byte[] numArray = new byte[1100];
        numArray[0] = (byte) 1;
        while (srcOffset < data.Length)
        {
          int count = num;
          if (srcOffset + num >= data.Length)
          {
            numArray[0] = (byte) 0;
            count = data.Length - srcOffset;
          }
          Buffer.BlockCopy((Array) data, srcOffset, (Array) numArray, 4, count);
          srcOffset += count;
          GalaxyInstance.Networking().SendP2PPacket(peer, numArray, (uint) (count + 4), P2PSendType.P2P_SEND_RELIABLE);
        }
      }
    }

    public void Send(GalaxyID peer, OutgoingMessage message)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
        {
          message.Write(writer);
          output.Seek(0L, SeekOrigin.Begin);
          this.Send(peer, output.ToArray());
        }
      }
    }

    private class GalaxyLobbyCreatedListener : ILobbyCreatedListener
    {
      private Action<GalaxyID, LobbyCreateResult> callback;

      public GalaxyLobbyCreatedListener(Action<GalaxyID, LobbyCreateResult> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyCreated.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnLobbyCreated(GalaxyID lobbyID, LobbyCreateResult result) => this.callback(lobbyID, result);

      public override void Dispose()
      {
        GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyCreated.GetListenerType(), (IGalaxyListener) this);
        base.Dispose();
      }
    }

    private class GalaxyLobbyEnteredListener : ILobbyEnteredListener
    {
      private Action<GalaxyID, LobbyEnterResult> callback;

      public GalaxyLobbyEnteredListener(Action<GalaxyID, LobbyEnterResult> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyEntered.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnLobbyEntered(GalaxyID lobbyID, LobbyEnterResult result) => this.callback(lobbyID, result);

      public override void Dispose()
      {
        GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyEntered.GetListenerType(), (IGalaxyListener) this);
        base.Dispose();
      }
    }

    private class GalaxyLobbyLeftListener : ILobbyLeftListener
    {
      private Action<GalaxyID, ILobbyLeftListener.LobbyLeaveReason> callback;

      public GalaxyLobbyLeftListener(
        Action<GalaxyID, ILobbyLeftListener.LobbyLeaveReason> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyLeft.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnLobbyLeft(
        GalaxyID lobbyID,
        ILobbyLeftListener.LobbyLeaveReason leaveReason)
      {
        this.callback(lobbyID, leaveReason);
      }

      public override void Dispose()
      {
        GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyLeft.GetListenerType(), (IGalaxyListener) this);
        base.Dispose();
      }
    }
  }
}
