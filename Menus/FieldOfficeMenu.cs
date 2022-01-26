// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.FieldOfficeMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class FieldOfficeMenu : MenuWithInventory
  {
    private Texture2D fieldOfficeMenuTexture;
    private IslandFieldOffice office;
    private bool madeADonation;
    private bool gotReward;
    public List<ClickableComponent> pieceHolders = new List<ClickableComponent>();
    private float bearTimer;
    private float snakeTimer;
    private float batTimer;
    private float frogTimer;

    public FieldOfficeMenu(IslandFieldOffice office)
      : base(new InventoryMenu.highlightThisItem(FieldOfficeMenu.highlightBones), true, true, 16, 132)
    {
      FieldOfficeMenu fieldOfficeMenu = this;
      this.office = office;
      this.fieldOfficeMenuTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\FieldOfficeDonationMenu");
      Point point = new Point(this.xPositionOnScreen + 32, this.yPositionOnScreen + 96);
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 76, point.Y + 180, 64, 64), office.piecesDonated[0] ? (Item) new StardewValley.Object(823, 1) : (Item) null)
      {
        label = "823"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 144, point.Y + 180, 64, 64), office.piecesDonated[1] ? (Item) new StardewValley.Object(824, 1) : (Item) null)
      {
        label = "824"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 212, point.Y + 180, 64, 64), office.piecesDonated[2] ? (Item) new StardewValley.Object(823, 1) : (Item) null)
      {
        label = "823"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 76, point.Y + 112, 64, 64), office.piecesDonated[3] ? (Item) new StardewValley.Object(822, 1) : (Item) null)
      {
        label = "822"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 144, point.Y + 112, 64, 64), office.piecesDonated[4] ? (Item) new StardewValley.Object(821, 1) : (Item) null)
      {
        label = "821"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 212, point.Y + 112, 64, 64), office.piecesDonated[5] ? (Item) new StardewValley.Object(820, 1) : (Item) null)
      {
        label = "820"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 412, point.Y + 48, 64, 64), office.piecesDonated[6] ? (Item) new StardewValley.Object(826, 1) : (Item) null)
      {
        label = "826"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 412, point.Y + 128, 64, 64), office.piecesDonated[7] ? (Item) new StardewValley.Object(826, 1) : (Item) null)
      {
        label = "826"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 412, point.Y + 208, 64, 64), office.piecesDonated[8] ? (Item) new StardewValley.Object(825, 1) : (Item) null)
      {
        label = "825"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 616, point.Y + 36, 64, 64), office.piecesDonated[9] ? (Item) new StardewValley.Object(827, 1) : (Item) null)
      {
        label = "827"
      });
      this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 624, point.Y + 156, 64, 64), office.piecesDonated[10] ? (Item) new StardewValley.Object(828, 1) : (Item) null)
      {
        label = "828"
      });
      if (Game1.activeClickableMenu == null)
        Game1.playSound("bigSelect");
      for (int index = 0; index < this.pieceHolders.Count; ++index)
      {
        ClickableComponent pieceHolder = this.pieceHolders[index];
        int num1;
        int num2 = num1 = -99998;
        pieceHolder.leftNeighborID = num1;
        int num3;
        int num4 = num3 = num2;
        pieceHolder.rightNeighborID = num3;
        int num5;
        int num6 = num5 = num4;
        pieceHolder.downNeighborID = num5;
        pieceHolder.upNeighborID = num6;
        pieceHolder.myID = 1000 + index;
      }
      foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
        clickableComponent.upNeighborID = -99998;
      foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
      {
        clickableComponent.rightNeighborID = 4857;
        clickableComponent.rightNeighborImmutable = true;
      }
      this.populateClickableComponentList();
      if (Game1.options.SnappyMenus)
        this.snapToDefaultClickableComponent();
      this.trashCan.leftNeighborID = this.okButton.leftNeighborID = 11;
      this.exitFunction = (IClickableMenu.onExit) (() =>
      {
        if (!fieldOfficeMenu.madeADonation)
          return;
        string str = fieldOfficeMenu.gotReward ? "#$b#" + Game1.content.LoadString("Strings\\Locations:FieldOfficeDonated_Reward") : "";
        Game1.drawDialogue(office.getSafariGuy(), Game1.content.LoadString("Strings\\Locations:FieldOfficeDonated_" + Game1.random.Next(4).ToString()) + str);
        if (!fieldOfficeMenu.gotReward)
          return;
        Game1.multiplayer.globalChatInfoMessage("FieldOfficeCompleteSet", Game1.player.Name);
      });
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return (b.myID != 5948 || b.myID == 4857) && base.IsAutomaticSnapValid(direction, a, b);
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public static bool highlightBones(Item i)
    {
      if (i != null)
      {
        IslandFieldOffice locationFromName = (IslandFieldOffice) Game1.getLocationFromName("IslandFieldOffice");
        switch ((int) (NetFieldBase<int, NetInt>) i.parentSheetIndex)
        {
          case 820:
            if (!locationFromName.piecesDonated[5])
              return true;
            break;
          case 821:
            if (!locationFromName.piecesDonated[4])
              return true;
            break;
          case 822:
            if (!locationFromName.piecesDonated[3])
              return true;
            break;
          case 823:
            if (!locationFromName.piecesDonated[0] || !locationFromName.piecesDonated[2])
              return true;
            break;
          case 824:
            if (!locationFromName.piecesDonated[1])
              return true;
            break;
          case 825:
            if (!locationFromName.piecesDonated[8])
              return true;
            break;
          case 826:
            if (!locationFromName.piecesDonated[7] || !locationFromName.piecesDonated[6])
              return true;
            break;
          case 827:
            if (!locationFromName.piecesDonated[9])
              return true;
            break;
          case 828:
            if (!locationFromName.piecesDonated[10])
              return true;
            break;
        }
      }
      return false;
    }

    public static int getPieceIndexForDonationItem(int itemIndex)
    {
      switch (itemIndex)
      {
        case 820:
          return 5;
        case 821:
          return 4;
        case 822:
          return 3;
        case 823:
          return 0;
        case 824:
          return 1;
        case 825:
          return 8;
        case 826:
          return 7;
        case 827:
          return 9;
        case 828:
          return 10;
        default:
          return -1;
      }
    }

    public static int getDonationPieceIndexNeededForSpot(int donationSpotIndex)
    {
      switch (donationSpotIndex)
      {
        case 0:
        case 2:
          return 823;
        case 1:
          return 824;
        case 3:
          return 822;
        case 4:
          return 821;
        case 5:
          return 820;
        case 6:
        case 7:
          return 826;
        case 8:
          return 825;
        case 9:
          return 827;
        case 10:
          return 828;
        default:
          return -1;
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      if (this.heldItem == null)
        return;
      int indexForDonationItem = FieldOfficeMenu.getPieceIndexForDonationItem((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex);
      if (indexForDonationItem == -1)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex == 823)
      {
        if (this.donate(0, x, y))
          return;
        this.donate(2, x, y);
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex == 826)
      {
        if (this.donate(7, x, y))
          return;
        this.donate(6, x, y);
      }
      else
        this.donate(indexForDonationItem, x, y);
    }

    protected override void cleanupBeforeExit()
    {
      base.cleanupBeforeExit();
      if (this.office == null || !this.office.isRangeAllTrue(0, 11) || !this.office.plantsRestoredRight.Value || !this.office.plantsRestoredLeft.Value || Game1.player.hasOrWillReceiveMail("fieldOfficeFinale"))
        return;
      this.office.triggerFinaleCutscene();
    }

    private bool donate(int index, int x, int y)
    {
      if (!this.pieceHolders[index].containsPoint(x, y) || this.pieceHolders[index].item != null)
        return false;
      this.pieceHolders[index].item = (Item) new StardewValley.Object((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex, 1);
      --this.heldItem.Stack;
      if (this.heldItem.Stack <= 0)
        this.heldItem = (Item) null;
      Game1.playSound("newArtifact");
      this.checkForSetFinish();
      this.gotReward = this.office.donatePiece(index);
      Game1.multiplayer.globalChatInfoMessage("FieldOfficeDonation", Game1.player.Name, this.pieceHolders[index].item.DisplayName);
      this.madeADonation = true;
      return true;
    }

    public void checkForSetFinish()
    {
      if (!this.office.centerSkeletonRestored.Value && this.pieceHolders[0].item != null && this.pieceHolders[1].item != null && this.pieceHolders[2].item != null && this.pieceHolders[3].item != null && this.pieceHolders[4].item != null && this.pieceHolders[5].item != null)
        DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
        {
          this.bearTimer = 500f;
          Game1.playSound("camel");
        }), 700);
      if (!this.office.snakeRestored.Value && this.pieceHolders[6].item != null && this.pieceHolders[7].item != null && this.pieceHolders[8].item != null)
        DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
        {
          this.snakeTimer = 1500f;
          Game1.playSound("steam");
        }), 700);
      if (!this.office.batRestored.Value && this.pieceHolders[9].item != null)
        DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
        {
          this.batTimer = 1500f;
          Game1.playSound("batScreech");
        }), 700);
      if (this.office.frogRestored.Value || this.pieceHolders[10].item == null)
        return;
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
      {
        this.frogTimer = 1000f;
        Game1.playSound("croak");
      }), 700);
    }

    public override void update(GameTime time)
    {
      base.update(time);
      TimeSpan elapsedGameTime;
      if ((double) this.bearTimer > 0.0)
      {
        double bearTimer = (double) this.bearTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.bearTimer = (float) (bearTimer - totalMilliseconds);
      }
      if ((double) this.snakeTimer > 0.0)
      {
        double snakeTimer = (double) this.snakeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.snakeTimer = (float) (snakeTimer - totalMilliseconds);
      }
      if ((double) this.batTimer > 0.0)
      {
        double batTimer = (double) this.batTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.batTimer = (float) (batTimer - totalMilliseconds);
      }
      if ((double) this.frogTimer <= 0.0)
        return;
      double frogTimer = (double) this.frogTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds1 = elapsedGameTime.TotalMilliseconds;
      this.frogTimer = (float) (frogTimer - totalMilliseconds1);
    }

    public override void draw(SpriteBatch b)
    {
      this.draw(b, drawDescriptionArea: false, red: 0, green: 80, blue: 80);
      b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32), (float) (this.yPositionOnScreen + 96)), new Rectangle?(new Rectangle(0, 0, 204, 80)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + this.width - 160), (float) (this.yPositionOnScreen + 108) + ((double) this.batTimer > 0.0 ? (float) (Math.Sin((1500.0 - (double) this.batTimer) / 80.0) * 64.0 / 4.0) : 0.0f)), new Rectangle?(new Rectangle(68, 84, 30, 20)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      foreach (ClickableComponent pieceHolder in this.pieceHolders)
      {
        if (pieceHolder.item != null)
          pieceHolder.item.drawInMenu(b, Utility.PointToVector2(pieceHolder.bounds.Location), 1f);
      }
      if ((double) this.bearTimer > 0.0)
        b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32 + 240), (float) (this.yPositionOnScreen + 96 + 36)), new Rectangle?(new Rectangle(0, 81, 37, 29)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      else if ((double) this.snakeTimer > 0.0 && (double) this.snakeTimer / 300.0 % 2.0 != 0.0)
        b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32 + 484), (float) (this.yPositionOnScreen + 96 + 232)), new Rectangle?(new Rectangle(47, 84, 19, 19)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      else if ((double) this.frogTimer > 0.0)
        b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32 + 708), (float) (this.yPositionOnScreen + 96 + 140)), new Rectangle?(new Rectangle(100, 89, 18, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      if (this.heldItem != null)
      {
        int indexForDonationItem = FieldOfficeMenu.getPieceIndexForDonationItem((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex);
        if (indexForDonationItem != -1)
          this.drawHighlightedSquare(indexForDonationItem, b);
      }
      this.drawMouse(b);
      if (this.heldItem == null)
        return;
      this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16), (float) (Game1.getOldMouseY() + 16)), 1f);
    }

    private void drawHighlightedSquare(int index, SpriteBatch b)
    {
      Rectangle rectangle = new Rectangle();
      switch ((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex)
      {
        case 820:
        case 821:
        case 822:
        case 823:
        case 824:
          rectangle = new Rectangle(119, 86, 18, 18);
          break;
        case 825:
        case 826:
          rectangle = new Rectangle(138, 86, 18, 18);
          break;
        case 827:
          rectangle = new Rectangle(157, 86, 18, 18);
          break;
        case 828:
          rectangle = new Rectangle(176, 86, 18, 18);
          break;
      }
      if (this.pieceHolders[index].item == null)
        b.Draw(this.fieldOfficeMenuTexture, Utility.PointToVector2(this.pieceHolders[index].bounds.Location) + new Vector2(-1f, -1f) * 4f, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      if ((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex == 823 && index == 0 && this.pieceHolders[2].item == null)
        b.Draw(this.fieldOfficeMenuTexture, Utility.PointToVector2(this.pieceHolders[2].bounds.Location) + new Vector2(-1f, -1f) * 4f, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      if ((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex != 826 || index != 7 || this.pieceHolders[6].item != null)
        return;
      b.Draw(this.fieldOfficeMenuTexture, Utility.PointToVector2(this.pieceHolders[6].bounds.Location) + new Vector2(-1f, -1f) * 4f, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    }
  }
}
