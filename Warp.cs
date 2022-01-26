// Decompiled with JetBrains decompiler
// Type: StardewValley.Warp
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Xml.Serialization;

namespace StardewValley
{
  public class Warp : INetObject<NetFields>
  {
    [XmlElement("x")]
    private readonly NetInt x = new NetInt();
    [XmlElement("y")]
    private readonly NetInt y = new NetInt();
    [XmlElement("targetX")]
    private readonly NetInt targetX = new NetInt();
    [XmlElement("targetY")]
    private readonly NetInt targetY = new NetInt();
    [XmlElement("flipFarmer")]
    public readonly NetBool flipFarmer = new NetBool();
    [XmlElement("targetName")]
    private readonly NetString targetName = new NetString();
    [XmlElement("npcOnly")]
    public readonly NetBool npcOnly = new NetBool();

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public int X => (int) (NetFieldBase<int, NetInt>) this.x;

    public int Y => (int) (NetFieldBase<int, NetInt>) this.y;

    public int TargetX
    {
      get => (int) (NetFieldBase<int, NetInt>) this.targetX;
      set => this.targetX.Value = value;
    }

    public int TargetY
    {
      get => (int) (NetFieldBase<int, NetInt>) this.targetY;
      set => this.targetY.Value = value;
    }

    public string TargetName
    {
      get => (string) (NetFieldBase<string, NetString>) this.targetName;
      set => this.targetName.Value = value;
    }

    public Warp() => this.NetFields.AddFields((INetSerializable) this.x, (INetSerializable) this.y, (INetSerializable) this.targetX, (INetSerializable) this.targetY, (INetSerializable) this.targetName, (INetSerializable) this.flipFarmer, (INetSerializable) this.npcOnly);

    public Warp(
      int x,
      int y,
      string targetName,
      int targetX,
      int targetY,
      bool flipFarmer,
      bool npcOnly = false)
      : this()
    {
      this.x.Value = x;
      this.y.Value = y;
      this.targetX.Value = targetX;
      this.targetY.Value = targetY;
      this.targetName.Value = targetName;
      this.flipFarmer.Value = flipFarmer;
      this.npcOnly.Value = npcOnly;
    }
  }
}
