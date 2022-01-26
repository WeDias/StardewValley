// Decompiled with JetBrains decompiler
// Type: StardewValley.NutDropRequest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System.IO;

namespace StardewValley
{
  public class NutDropRequest : NetEventArg
  {
    public string key;
    public string locationName;
    public Point position;
    public int limit = 1;
    public int rewardAmount = 1;

    public NutDropRequest()
    {
    }

    public NutDropRequest(
      string key,
      string location_name,
      Point position,
      int limit,
      int reward_amount)
    {
      this.key = key;
      this.locationName = location_name != null ? location_name : "null";
      this.position = position;
      this.limit = limit;
      this.rewardAmount = reward_amount;
    }

    public void Read(BinaryReader reader)
    {
      this.key = reader.ReadString();
      this.locationName = reader.ReadString();
      this.position.X = reader.ReadInt32();
      this.position.Y = reader.ReadInt32();
      this.limit = reader.ReadInt32();
      this.rewardAmount = reader.ReadInt32();
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.key);
      writer.Write(this.locationName);
      writer.Write(this.position.X);
      writer.Write(this.position.Y);
      writer.Write(this.limit);
      writer.Write(this.rewardAmount);
    }
  }
}
