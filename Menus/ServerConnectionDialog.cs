// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ServerConnectionDialog
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley.Menus
{
  public class ServerConnectionDialog : ConfirmationDialog
  {
    public ServerConnectionDialog(
      ConfirmationDialog.behavior onConfirm = null,
      ConfirmationDialog.behavior onCancel = null)
      : base(Game1.content.LoadString("Strings\\UI:CoopMenu_Connecting"), onConfirm, onCancel)
    {
      this.okButton.visible = false;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (Game1.server == null || !Game1.server.connected())
        return;
      this.confirm();
    }
  }
}
