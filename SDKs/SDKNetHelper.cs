// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.SDKNetHelper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.Network;

namespace StardewValley.SDKs
{
  public interface SDKNetHelper
  {
    string GetUserID();

    Client CreateClient(object lobby);

    Client GetRequestedClient();

    Server CreateServer(IGameServer gameServer);

    void AddLobbyUpdateListener(LobbyUpdateListener listener);

    void RemoveLobbyUpdateListener(LobbyUpdateListener listener);

    void RequestFriendLobbyData();

    string GetLobbyData(object lobby, string key);

    string GetLobbyOwnerName(object lobby);

    bool SupportsInviteCodes();

    object GetLobbyFromInviteCode(string inviteCode);

    void ShowInviteDialog(object lobby);

    void MutePlayer(string userId, bool mute);

    bool IsPlayerMuted(string userId);

    void ShowProfile(string userId);
  }
}
