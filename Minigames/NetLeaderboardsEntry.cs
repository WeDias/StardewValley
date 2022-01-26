// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.NetLeaderboardsEntry
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;

namespace StardewValley.Minigames
{
  public class NetLeaderboardsEntry : INetObject<NetFields>
  {
    public readonly NetString name = new NetString("");
    public readonly NetInt score = new NetInt(0);

    public NetFields NetFields { get; } = new NetFields();

    public void InitNetFields() => this.NetFields.AddFields((INetSerializable) this.name, (INetSerializable) this.score);

    public NetLeaderboardsEntry() => this.InitNetFields();

    public NetLeaderboardsEntry(string new_name, int new_score)
    {
      this.InitNetFields();
      this.name.Value = new_name;
      this.score.Value = new_score;
    }
  }
}
