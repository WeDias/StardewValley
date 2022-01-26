// Decompiled with JetBrains decompiler
// Type: StardewValley.Proposal
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Network;

namespace StardewValley
{
  public class Proposal : INetObject<NetFields>
  {
    public readonly NetFarmerRef sender = new NetFarmerRef();
    public readonly NetFarmerRef receiver = new NetFarmerRef();
    public readonly NetEnum<ProposalType> proposalType = new NetEnum<ProposalType>(ProposalType.Gift);
    public readonly NetEnum<ProposalResponse> response = new NetEnum<ProposalResponse>(ProposalResponse.None);
    public readonly NetString responseMessageKey = new NetString();
    public readonly NetRef<Item> gift = new NetRef<Item>();
    public readonly NetBool canceled = new NetBool();
    public readonly NetBool cancelConfirmed = new NetBool();

    public NetFields NetFields { get; } = new NetFields();

    public Proposal() => this.NetFields.AddFields((INetSerializable) this.sender.NetFields, (INetSerializable) this.receiver.NetFields, (INetSerializable) this.proposalType, (INetSerializable) this.response, (INetSerializable) this.responseMessageKey, (INetSerializable) this.gift, (INetSerializable) this.canceled, (INetSerializable) this.cancelConfirmed);
  }
}
