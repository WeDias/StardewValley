// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GalaxyNetHelper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Galaxy.Api;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewValley.SDKs
{
  public class GalaxyNetHelper : SDKNetHelper
  {
    public const string GalaxyConnectionStringPrefix = "-connect-lobby-";
    public const string SteamConnectionStringPrefix = "+connect_lobby";
    protected GalaxyID lobbyRequested;
    private GalaxyNetHelper.LobbyEnteredListener lobbyEntered;
    private GalaxyNetHelper.GameJoinRequestedListener lobbyJoinRequested;
    private GalaxyNetHelper.LobbyDataListener lobbyDataListener;
    private GalaxyNetHelper.RichPresenceListener richPresenceListener;
    private List<LobbyUpdateListener> lobbyUpdateListeners = new List<LobbyUpdateListener>();

    public GalaxyNetHelper()
    {
      this.lobbyRequested = this.getStartupLobby();
      this.lobbyJoinRequested = new GalaxyNetHelper.GameJoinRequestedListener(new Action<GalaxyID, string>(this.onLobbyJoinRequested));
      this.lobbyEntered = new GalaxyNetHelper.LobbyEnteredListener(new Action<GalaxyID, LobbyEnterResult>(this.onLobbyEntered));
      this.lobbyDataListener = new GalaxyNetHelper.LobbyDataListener(new Action<GalaxyID, GalaxyID>(this.onLobbyDataUpdated));
      this.richPresenceListener = new GalaxyNetHelper.RichPresenceListener(new Action<GalaxyID>(this.onRichPresenceUpdated));
      if (!(this.lobbyRequested != (GalaxyID) null))
        return;
      Game1.multiplayer.inviteAccepted();
    }

    public virtual string GetUserID() => Convert.ToString(GalaxyInstance.User().GetGalaxyID().ToUint64());

    protected virtual Client createClient(GalaxyID lobby) => Game1.multiplayer.InitClient((Client) new GalaxyNetClient(lobby));

    public Client CreateClient(object lobby) => this.createClient(new GalaxyID((ulong) lobby));

    public virtual Server CreateServer(IGameServer gameServer) => Game1.multiplayer.InitServer((Server) new GalaxyNetServer(gameServer));

    protected GalaxyID parseConnectionString(string connectionString)
    {
      if (connectionString == null)
        return (GalaxyID) null;
      if (connectionString.StartsWith("-connect-lobby-"))
        return new GalaxyID(Convert.ToUInt64(connectionString.Substring("-connect-lobby-".Length)));
      return connectionString.StartsWith("+connect_lobby ") ? new GalaxyID(Convert.ToUInt64(connectionString.Substring("+connect_lobby".Length + 1))) : (GalaxyID) null;
    }

    protected virtual GalaxyID getStartupLobby()
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      for (int index = 0; index < commandLineArgs.Length; ++index)
      {
        if (commandLineArgs[index].StartsWith("-connect-lobby-"))
          return this.parseConnectionString(commandLineArgs[index]);
      }
      return (GalaxyID) null;
    }

    public Client GetRequestedClient() => this.lobbyRequested != (GalaxyID) null ? this.createClient(this.lobbyRequested) : (Client) null;

    public void AddLobbyUpdateListener(LobbyUpdateListener listener) => this.lobbyUpdateListeners.Add(listener);

    public void RemoveLobbyUpdateListener(LobbyUpdateListener listener) => this.lobbyUpdateListeners.Remove(listener);

    public virtual void RequestFriendLobbyData()
    {
      uint friendCount = GalaxyInstance.Friends().GetFriendCount();
      for (uint index = 0; index < friendCount; ++index)
      {
        GalaxyID friendByIndex = GalaxyInstance.Friends().GetFriendByIndex(index);
        GalaxyInstance.Friends().RequestRichPresence(friendByIndex);
      }
    }

    private void onRichPresenceUpdated(GalaxyID userID)
    {
      GalaxyID connectionString = this.parseConnectionString(GalaxyInstance.Friends().GetRichPresence("connect", userID));
      if (!(connectionString != (GalaxyID) null))
        return;
      GalaxyInstance.Matchmaking().RequestLobbyData(connectionString);
    }

    private void onLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
    {
      foreach (LobbyUpdateListener lobbyUpdateListener in this.lobbyUpdateListeners)
        lobbyUpdateListener.OnLobbyUpdate((object) lobbyID.ToUint64());
    }

    public virtual string GetLobbyData(object lobby, string key) => GalaxyInstance.Matchmaking().GetLobbyData(new GalaxyID((ulong) lobby), key);

    public virtual string GetLobbyOwnerName(object lobbyId)
    {
      GalaxyID lobbyID = new GalaxyID((ulong) lobbyId);
      GalaxyID lobbyOwner = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
      return GalaxyInstance.Friends().GetFriendPersonaName(lobbyOwner);
    }

    protected virtual void onLobbyEntered(GalaxyID lobby_id, LobbyEnterResult result)
    {
    }

    private void onLobbyJoinRequested(GalaxyID userID, string connectionString)
    {
      this.lobbyRequested = this.parseConnectionString(connectionString);
      if (!(this.lobbyRequested != (GalaxyID) null))
        return;
      Game1.multiplayer.inviteAccepted();
    }

    public bool SupportsInviteCodes() => true;

    public object GetLobbyFromInviteCode(string inviteCode)
    {
      ulong num = 0;
      try
      {
        num = Base36.Decode(inviteCode);
      }
      catch (FormatException ex)
      {
      }
      return num != 0UL && num >> 56 == 0UL ? (object) GalaxyID.FromRealID(GalaxyID.IDType.ID_TYPE_LOBBY, num).ToUint64() : (object) null;
    }

    public virtual void ShowInviteDialog(object lobby) => GalaxyInstance.Friends().ShowOverlayInviteDialog("-connect-lobby-" + Convert.ToString((ulong) lobby));

    public void MutePlayer(string userId, bool mute)
    {
    }

    public bool IsPlayerMuted(string userId) => false;

    public void ShowProfile(string userId)
    {
    }

    private class LobbyEnteredListener : ILobbyEnteredListener
    {
      private Action<GalaxyID, LobbyEnterResult> callback;

      public LobbyEnteredListener(Action<GalaxyID, LobbyEnterResult> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyEntered.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnLobbyEntered(GalaxyID lobby_id, LobbyEnterResult result)
      {
        if (this.callback == null)
          return;
        this.callback(lobby_id, result);
      }
    }

    private class GameJoinRequestedListener : IGameJoinRequestedListener
    {
      private Action<GalaxyID, string> callback;

      public GameJoinRequestedListener(Action<GalaxyID, string> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerGameJoinRequested.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnGameJoinRequested(GalaxyID lobbyID, string result)
      {
        if (this.callback == null)
          return;
        this.callback(lobbyID, result);
      }
    }

    private class LobbyDataListener : ILobbyDataListener
    {
      private Action<GalaxyID, GalaxyID> callback;

      public LobbyDataListener(Action<GalaxyID, GalaxyID> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyData.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
      {
        if (this.callback == null)
          return;
        this.callback(lobbyID, memberID);
      }
    }

    private class RichPresenceListener : IRichPresenceListener
    {
      private Action<GalaxyID> callback;

      public RichPresenceListener(Action<GalaxyID> callback)
      {
        this.callback = callback;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerRichPresence.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnRichPresenceUpdated(GalaxyID userID)
      {
        if (this.callback == null)
          return;
        this.callback(userID);
      }
    }
  }
}
