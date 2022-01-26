// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.EmoteSelector
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StardewValley.Menus
{
  public class EmoteSelector : IClickableMenu
  {
    public Rectangle scrollView;
    public List<ClickableTextureComponent> emoteButtons;
    public ClickableTextureComponent okButton;
    public float scrollY;
    public int emoteIndex;
    protected ClickableTextureComponent _selectedEmote;
    protected ClickableTextureComponent _hoveredEmote;
    protected Texture2D emoteTexture;

    public EmoteSelector(int emote_index, string selected_emote = "")
      : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + 64)
    {
      this.emoteTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\EmoteMenu");
      Game1.playSound("shwip");
      this.emoteIndex = emote_index;
      Game1.player.faceDirection(2);
      Game1.player.FarmerSprite.StopAnimation();
      typeof (FarmerSprite).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      this.emoteButtons = new List<ClickableTextureComponent>();
      this.currentlySnappedComponent = (ClickableComponent) null;
      for (int emote_index1 = 0; emote_index1 < Farmer.EMOTES.Length; ++emote_index1)
      {
        Farmer.EmoteType emoteType = Farmer.EMOTES[emote_index1];
        if (!emoteType.hidden || Game1.player.performedEmotes.ContainsKey(emoteType.emoteString))
        {
          ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(0, 0, 80, 68), this.emoteTexture, EmoteMenu.GetEmoteNonBubbleSpriteRect(emote_index1), 4f, true);
          textureComponent1.leftNeighborID = -99998;
          textureComponent1.rightNeighborID = -99998;
          textureComponent1.upNeighborID = -99998;
          textureComponent1.downNeighborID = -99998;
          textureComponent1.myID = emote_index1;
          ClickableTextureComponent textureComponent2 = textureComponent1;
          textureComponent2.label = emoteType.displayName;
          textureComponent2.name = emoteType.emoteString;
          textureComponent2.drawLabelWithShadow = true;
          textureComponent2.hoverText = emoteType.animationFrames != null ? "animated" : "";
          this.emoteButtons.Add(textureComponent2);
          if (this.currentlySnappedComponent == null)
            this.currentlySnappedComponent = (ClickableComponent) textureComponent2;
          if (selected_emote != "" && selected_emote == textureComponent2.name)
          {
            this.currentlySnappedComponent = (ClickableComponent) textureComponent2;
            this._selectedEmote = textureComponent2;
          }
        }
      }
      ClickableTextureComponent textureComponent = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      textureComponent.myID = 1000;
      textureComponent.drawShadow = true;
      this.okButton = textureComponent;
      this.RepositionElements();
      this.populateClickableComponentList();
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      this.snapToDefaultClickableComponent();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;
      this.RepositionElements();
    }

    public override void performHoverAction(int x, int y)
    {
      ClickableTextureComponent hoveredEmote = this._hoveredEmote;
      this._hoveredEmote = (ClickableTextureComponent) null;
      this.okButton.tryHover(x, y);
      foreach (ClickableTextureComponent emoteButton in this.emoteButtons)
      {
        int width = emoteButton.bounds.Width;
        emoteButton.bounds.Width = this.scrollView.Width / 3;
        emoteButton.tryHover(x, y);
        if (emoteButton != this._selectedEmote && emoteButton.bounds.Contains(x, y) && this.scrollView.Contains(x, y))
          this._hoveredEmote = emoteButton;
        emoteButton.bounds.Width = width;
      }
      if (this._hoveredEmote == null || this._hoveredEmote == hoveredEmote)
        return;
      Game1.playSound("shiny4");
    }

    private void RepositionElements()
    {
      this.scrollView = new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 4, this.width - 128, this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder - 64 + 8);
      this.RepositionScrollElements();
    }

    public void RepositionScrollElements()
    {
      int num1 = (int) this.scrollY + 4;
      if ((double) this.scrollY > 0.0)
        this.scrollY = 0.0f;
      int num2 = 8;
      foreach (ClickableTextureComponent emoteButton in this.emoteButtons)
      {
        emoteButton.bounds.X = this.scrollView.X + num2;
        emoteButton.bounds.Y = this.scrollView.Y + num1;
        if (emoteButton.bounds.Bottom > this.scrollView.Bottom)
        {
          num1 = 4;
          num2 += this.scrollView.Width / 3;
          emoteButton.bounds.X = this.scrollView.X + num2;
          emoteButton.bounds.Y = this.scrollView.Y + num1;
        }
        num1 += emoteButton.bounds.Height;
        if (this.scrollView.Intersects(emoteButton.bounds))
          emoteButton.visible = true;
        else
          emoteButton.visible = false;
      }
    }

    public override void snapToDefaultClickableComponent() => this.snapCursorToCurrentSnappedComponent();

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      foreach (ClickableTextureComponent emoteButton in this.emoteButtons)
      {
        int width = emoteButton.bounds.Width;
        emoteButton.bounds.Width = this.scrollView.Width / 3;
        if (emoteButton.bounds.Contains(x, y) && this.scrollView.Contains(x, y))
        {
          emoteButton.bounds.Width = width;
          if (this.emoteIndex < Game1.player.GetEmoteFavorites().Count)
            Game1.player.GetEmoteFavorites()[this.emoteIndex] = emoteButton.name;
          this.exitThisMenu(false);
          Game1.playSound("drumkit6");
          if (Game1.options.gamepadControls)
            return;
          Game1.emoteMenu = new EmoteMenu();
          return;
        }
        emoteButton.bounds.Width = width;
      }
      if (!this.okButton.containsPoint(x, y))
        return;
      this.exitThisMenu();
    }

    public bool canLeaveMenu() => true;

    public override void draw(SpriteBatch b)
    {
      bool ignoreTitleSafe = true;
      IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), this.xPositionOnScreen - 128 - 8, this.yPositionOnScreen + 128 - 8, 192, 164, Color.White, drawShadow: false);
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, ignoreTitleSafe: ignoreTitleSafe);
      foreach (ClickableTextureComponent emoteButton in this.emoteButtons)
      {
        if (emoteButton == this.currentlySnappedComponent && Game1.options.gamepadControls && emoteButton != this._selectedEmote && emoteButton == this._hoveredEmote)
        {
          IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(64, 320, 60, 60), emoteButton.bounds.X + 64 + 8, emoteButton.bounds.Y + 8, this.scrollView.Width / 3 - 64 - 16, emoteButton.bounds.Height - 16, Color.White, drawShadow: false);
          Utility.drawWithShadow(b, this.emoteTexture, emoteButton.getVector2() - new Vector2(4f, 4f), new Rectangle(83, 0, 18, 18), Color.White, 0.0f, Vector2.Zero, 4f);
        }
        emoteButton.draw(b, Color.White * (emoteButton == this._selectedEmote ? 0.4f : 1f), 0.87f);
        if (emoteButton != this._selectedEmote && emoteButton.hoverText != "" && Game1.currentGameTime.TotalGameTime.Milliseconds % 500 < 250)
          b.Draw(emoteButton.texture, emoteButton.getVector2(), new Rectangle?(new Rectangle(emoteButton.sourceRect.X + 80, emoteButton.sourceRect.Y, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      }
      if (this._selectedEmote != null)
      {
        for (int index = 0; index < 8; ++index)
        {
          float num = Utility.Lerp(0.0f, 6.283185f, (float) index / 8f);
          Vector2 zero = Vector2.Zero with
          {
            X = (float) (int) ((double) (this.xPositionOnScreen - 64 + (int) (Math.Cos((double) num) * 12.0) * 4) - 3.5),
            Y = (float) (int) ((double) (this.yPositionOnScreen + 192 + (int) (-Math.Sin((double) num) * 12.0) * 4) - 3.5)
          };
          Utility.drawWithShadow(b, this.emoteTexture, zero, new Rectangle(64 + (index == this.emoteIndex ? 8 : 0), 48, 8, 8), Color.White, 0.0f, Vector2.Zero);
        }
      }
      this.okButton.draw(b);
      this.drawMouse(b);
    }

    public override void update(GameTime time) => base.update(time);

    protected override void cleanupBeforeExit()
    {
      base.cleanupBeforeExit();
      Game1.player.noMovementPause = Math.Max(Game1.player.noMovementPause, 200);
    }
  }
}
