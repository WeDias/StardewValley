// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.InviteCodeDialog
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley.Menus
{
  public class InviteCodeDialog : ConfirmationDialog
  {
    private string code;

    public InviteCodeDialog(string code, ConfirmationDialog.behavior onClose)
      : base(Game1.content.LoadString("Strings\\UI:Server_InviteCode", (object) code), onClose, onClose)
    {
      this.code = code;
      if (!DesktopClipboard.IsAvailable)
      {
        this.cancelButton.visible = false;
      }
      else
      {
        this.onCancel = new ConfirmationDialog.behavior(this.copyCode);
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64, 64), (string) null, (string) null, Game1.mouseCursors, new Rectangle(274, 284, 16, 16), 4f);
        textureComponent.myID = 102;
        textureComponent.leftNeighborID = 101;
        this.cancelButton = textureComponent;
      }
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.currentlySnappedComponent = this.getComponentWithID(101);
      this.snapCursorToCurrentSnappedComponent();
    }

    protected void copyCode(Farmer who)
    {
      if (DesktopClipboard.SetText(this.code))
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Server_InviteCode_Copied")));
      else
        Game1.showRedMessageUsingLoadString("Strings\\UI:Server_InviteCode_CopyFailed");
    }
  }
}
