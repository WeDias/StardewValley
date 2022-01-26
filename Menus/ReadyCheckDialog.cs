// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ReadyCheckDialog
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley.Menus
{
  public class ReadyCheckDialog : ConfirmationDialog
  {
    public string checkName;
    private bool allowCancel;

    public ReadyCheckDialog(
      string checkName,
      bool allowCancel,
      ConfirmationDialog.behavior onConfirm = null,
      ConfirmationDialog.behavior onCancel = null)
      : base(Game1.content.LoadString("Strings\\UI:ReadyCheck", (object) "N", (object) "M"), onConfirm, onCancel)
    {
      this.checkName = checkName;
      this.allowCancel = allowCancel;
      this.okButton.visible = false;
      this.cancelButton.visible = this.isCancelable();
      this.updateMessage();
      this.exitFunction = (IClickableMenu.onExit) (() => this.closeDialog(Game1.player));
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public bool isCancelable() => this.allowCancel && Game1.player.team.IsReadyCheckCancelable(this.checkName);

    public override bool readyToClose() => this.isCancelable();

    public override void closeDialog(Farmer who)
    {
      base.closeDialog(who);
      if (!this.isCancelable())
        return;
      Game1.player.team.SetLocalReady(this.checkName, false);
    }

    private void updateMessage()
    {
      int numberReady = Game1.player.team.GetNumberReady(this.checkName);
      int numberRequired = Game1.player.team.GetNumberRequired(this.checkName);
      this.message = Game1.content.LoadString("Strings\\UI:ReadyCheck", (object) numberReady, (object) numberRequired);
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this.cancelButton.visible = this.isCancelable();
      this.updateMessage();
      Game1.player.team.SetLocalReady(this.checkName, true);
      if (!Game1.player.team.IsReady(this.checkName))
        return;
      this.confirm();
    }
  }
}
