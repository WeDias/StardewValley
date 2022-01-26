// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.SteamNetHelper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Galaxy.Api;
using Steamworks;
using System;
using System.Collections.Generic;

namespace StardewValley.SDKs
{
  internal class SteamNetHelper : GalaxyNetHelper
  {
    private Steamworks.Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    private Steamworks.Callback<LobbyEnter_t> lobbyEnterCallback;
    private Steamworks.Callback<LobbyDataUpdate_t> lobbyDataUpdateCallback;
    private Dictionary<ulong, CSteamID> lobbyOwners = new Dictionary<ulong, CSteamID>();

    public SteamNetHelper()
    {
      this.gameLobbyJoinRequested = Steamworks.Callback<GameLobbyJoinRequested_t>.Create(new Steamworks.Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.onGameLobbyJoinRequested));
      this.lobbyEnterCallback = Steamworks.Callback<LobbyEnter_t>.Create(new Steamworks.Callback<LobbyEnter_t>.DispatchDelegate(this.onLobbyEnter));
      this.lobbyDataUpdateCallback = Steamworks.Callback<LobbyDataUpdate_t>.Create(new Steamworks.Callback<LobbyDataUpdate_t>.DispatchDelegate(this.onLobbyDataUpdate));
    }

    public override void RequestFriendLobbyData()
    {
      EFriendFlags iFriendFlags = EFriendFlags.k_EFriendFlagImmediate;
      int friendCount = SteamFriends.GetFriendCount(iFriendFlags);
      for (int iFriend = 0; iFriend < friendCount; ++iFriend)
      {
        FriendGameInfo_t pFriendGameInfo;
        if (SteamFriends.GetFriendGamePlayed(SteamFriends.GetFriendByIndex(iFriend, iFriendFlags), out pFriendGameInfo) && !(pFriendGameInfo.m_gameID.AppID() != SteamUtils.GetAppID()))
          SteamMatchmaking.RequestLobbyData(pFriendGameInfo.m_steamIDLobby);
      }
    }

    public override void ShowInviteDialog(object lobby) => SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID((ulong) lobby));

    private void onLobbyDataUpdate(LobbyDataUpdate_t pCallback)
    {
      CSteamID steamIDLobby = new CSteamID(pCallback.m_ulSteamIDLobby);
      GalaxyID connectionString = this.parseConnectionString(SteamMatchmaking.GetLobbyData(steamIDLobby, "connect"));
      if (connectionString == (GalaxyID) null)
        return;
      this.lobbyOwners[connectionString.ToUint64()] = SteamMatchmaking.GetLobbyOwner(steamIDLobby);
      GalaxyInstance.Matchmaking().RequestLobbyData(connectionString);
    }

    protected override GalaxyID getStartupLobby()
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      for (int index = 0; index < commandLineArgs.Length; ++index)
      {
        if (commandLineArgs[index] == "+connect_lobby")
        {
          try
          {
            SteamMatchmaking.JoinLobby(new CSteamID(Convert.ToUInt64(commandLineArgs[index + 1])));
            return (GalaxyID) null;
          }
          catch (Exception ex)
          {
          }
        }
      }
      return (GalaxyID) null;
    }

    private void onGameLobbyJoinRequested(GameLobbyJoinRequested_t pCallback) => SteamMatchmaking.JoinLobby(pCallback.m_steamIDLobby);

    private void onLobbyEnter(LobbyEnter_t pCallback)
    {
      CSteamID steamIDLobby = new CSteamID(pCallback.m_ulSteamIDLobby);
      if (SteamMatchmaking.GetLobbyOwner(steamIDLobby) == SteamUser.GetSteamID())
        return;
      this.lobbyRequested = this.parseConnectionString(SteamMatchmaking.GetLobbyData(steamIDLobby, "connect"));
      SteamMatchmaking.LeaveLobby(steamIDLobby);
      if (!(this.lobbyRequested != (GalaxyID) null))
        return;
      Game1.multiplayer.inviteAccepted();
    }

    public override string GetLobbyOwnerName(object lobbyID) => this.lobbyOwners.ContainsKey((ulong) lobbyID) ? SteamFriends.GetFriendPersonaName(this.lobbyOwners[(ulong) lobbyID]) : "";
  }
}
