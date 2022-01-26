// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.PendingProposalDialog
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;

namespace StardewValley.Menus
{
  public class PendingProposalDialog : ConfirmationDialog
  {
    public PendingProposalDialog()
      : base(Game1.content.LoadString("Strings\\UI:PendingProposal"), (ConfirmationDialog.behavior) null)
    {
      this.okButton.visible = false;
      this.onCancel = new ConfirmationDialog.behavior(this.cancelProposal);
      this.setCancelable(true);
    }

    public void cancelProposal(Farmer who)
    {
      Proposal outgoingProposal = Game1.player.team.GetOutgoingProposal();
      if (outgoingProposal == null || outgoingProposal.receiver.Value == null || !outgoingProposal.receiver.Value.isActive())
        return;
      outgoingProposal.canceled.Value = true;
      this.message = Game1.content.LoadString("Strings\\UI:PendingProposal_Canceling");
      this.setCancelable(false);
    }

    public void setCancelable(bool cancelable)
    {
      this.cancelButton.visible = cancelable;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override bool readyToClose() => false;

    private bool consumesItem(ProposalType pt) => pt == ProposalType.Gift || pt == ProposalType.Marriage;

    public override void update(GameTime time)
    {
      base.update(time);
      Proposal outgoingProposal = Game1.player.team.GetOutgoingProposal();
      if (outgoingProposal == null || outgoingProposal.receiver.Value == null || !outgoingProposal.receiver.Value.isActive())
      {
        Game1.player.team.RemoveOutgoingProposal();
        this.closeDialog(Game1.player);
      }
      else if (outgoingProposal.cancelConfirmed.Value && outgoingProposal.response.Value != ProposalResponse.Accepted)
      {
        Game1.player.team.RemoveOutgoingProposal();
        this.closeDialog(Game1.player);
      }
      else
      {
        if (outgoingProposal.response.Value == ProposalResponse.None)
          return;
        if (outgoingProposal.response.Value == ProposalResponse.Accepted)
        {
          if (this.consumesItem((ProposalType) (NetFieldBase<ProposalType, NetEnum<ProposalType>>) outgoingProposal.proposalType))
            Game1.player.reduceActiveItemByOne();
          if (outgoingProposal.proposalType.Value == ProposalType.Dance)
            Game1.player.dancePartner.Value = (Character) outgoingProposal.receiver.Value;
          outgoingProposal.receiver.Value.doEmote(20);
        }
        Game1.player.team.RemoveOutgoingProposal();
        this.closeDialog(Game1.player);
        if (outgoingProposal.responseMessageKey.Value == null)
          return;
        Game1.drawObjectDialogue(Game1.content.LoadString(outgoingProposal.responseMessageKey.Value, (object) outgoingProposal.receiver.Value.Name));
      }
    }
  }
}
