// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.EmoteMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class EmoteMenu : IClickableMenu
  {
    public Texture2D menuBackgroundTexture;
    public List<string> emotes;
    protected Point _mouseStartPosition;
    public bool _hasSelectedEmote;
    protected List<ClickableTextureComponent> _emoteButtons;
    protected string _selectedEmote;
    protected int _selectedIndex = -1;
    protected int _oldSelection;
    protected int _selectedTime;
    protected float _alpha;
    protected int _menuCloseGracePeriod = -1;
    protected int _age;
    public bool gamepadMode;
    protected int _expandTime = 200;
    protected int _expandedButtonRadius = 24;
    protected int _buttonRadius;
    public Vector2 _oldDrawPosition;

    public EmoteMenu()
    {
      this.menuBackgroundTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\EmoteMenu");
      this.width = 256;
      this.height = 256;
      this.xPositionOnScreen = (int) ((double) (Game1.viewport.Width / 2) - (double) this.width / 2.0);
      this.yPositionOnScreen = (int) ((double) (Game1.viewport.Height / 2) - (double) this.height / 2.0);
      this.emotes = new List<string>();
      foreach (string emoteFavorite in (NetList<string, NetString>) Game1.player.GetEmoteFavorites())
        this.emotes.Add(emoteFavorite);
      this._mouseStartPosition = Game1.getMousePosition(false);
      this._alpha = 0.0f;
      this._menuCloseGracePeriod = 300;
      this._CreateEmoteButtons();
      this._SnapToPlayerPosition();
    }

    protected void _CreateEmoteButtons()
    {
      this._emoteButtons = new List<ClickableTextureComponent>();
      for (int index1 = 0; index1 < this.emotes.Count; ++index1)
      {
        int emote_index = -1;
        for (int index2 = 0; index2 < ((IEnumerable<Farmer.EmoteType>) Farmer.EMOTES).Count<Farmer.EmoteType>(); ++index2)
        {
          if (Farmer.EMOTES[index2].emoteString == this.emotes[index1])
          {
            emote_index = index2;
            break;
          }
        }
        this._emoteButtons.Add(new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), this.menuBackgroundTexture, EmoteMenu.GetEmoteNonBubbleSpriteRect(emote_index), 4f));
      }
      this._RepositionButtons();
    }

    public static Rectangle GetEmoteSpriteRect(int emote_index) => emote_index <= 0 ? new Rectangle(48, 0, 16, 16) : new Rectangle(emote_index % 4 * 16 + 48, emote_index / 4 * 16, 16, 16);

    public static Rectangle GetEmoteNonBubbleSpriteRect(int emote_index) => new Rectangle(emote_index % 4 * 16, emote_index / 4 * 16, 16, 16);

    public override void applyMovementKey(int direction)
    {
    }

    protected override void cleanupBeforeExit()
    {
      if (Game1.emoteMenu != null)
        Game1.emoteMenu = (EmoteMenu) null;
      Game1.oldMouseState = Game1.input.GetMouseState();
      base.cleanupBeforeExit();
    }

    public override void performHoverAction(int x, int y)
    {
      x = (int) Utility.ModifyCoordinateFromUIScale((float) x);
      y = (int) Utility.ModifyCoordinateFromUIScale((float) y);
      if (this.gamepadMode)
        return;
      for (int index = 0; index < this._emoteButtons.Count; ++index)
      {
        if (this._emoteButtons[index].containsPoint(x, y))
        {
          this._selectedEmote = this.emotes[index];
          this._selectedIndex = index;
          if (this._selectedIndex == this._oldSelection)
            return;
          this._selectedTime = 0;
          return;
        }
      }
      this._selectedEmote = (string) null;
      this._selectedIndex = -1;
    }

    protected void _RepositionButtons()
    {
      for (int index = 0; index < this._emoteButtons.Count; ++index)
      {
        ClickableTextureComponent emoteButton = this._emoteButtons[index];
        float num = Utility.Lerp(0.0f, 6.283185f, (float) index / (float) this._emoteButtons.Count);
        emoteButton.bounds.X = (int) ((double) (this.xPositionOnScreen + this.width / 2 + (int) (Math.Cos((double) num) * (double) this._buttonRadius) * 4) - (double) emoteButton.bounds.Width / 2.0);
        emoteButton.bounds.Y = (int) ((double) (this.yPositionOnScreen + this.height / 2 + (int) (-Math.Sin((double) num) * (double) this._buttonRadius) * 4) - (double) emoteButton.bounds.Height / 2.0);
      }
    }

    protected void _SnapToPlayerPosition()
    {
      if (Game1.player == null)
        return;
      Vector2 vector2 = Game1.player.getLocalPosition(Game1.viewport) + new Vector2((float) -this.width / 2f, (float) -this.height / 2f);
      this.xPositionOnScreen = (int) vector2.X + 32;
      this.yPositionOnScreen = (int) vector2.Y - 64;
      if (this.xPositionOnScreen + this.width > Game1.viewport.Width)
        this.xPositionOnScreen -= this.xPositionOnScreen + this.width - Game1.viewport.Width;
      if (this.xPositionOnScreen < 0)
        this.xPositionOnScreen -= this.xPositionOnScreen;
      if (this.yPositionOnScreen + this.height > Game1.viewport.Height)
        this.yPositionOnScreen -= this.yPositionOnScreen + this.height - Game1.viewport.Height;
      if (this.yPositionOnScreen < 0)
        this.yPositionOnScreen -= this.yPositionOnScreen;
      this._RepositionButtons();
    }

    public override void update(GameTime time)
    {
      this._age += time.ElapsedGameTime.Milliseconds;
      if (this._age > this._expandTime)
        this._age = this._expandTime;
      if (!this.gamepadMode && Game1.options.gamepadControls && ((double) Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.X) > 0.5 || (double) Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.Y) > 0.5))
        this.gamepadMode = true;
      this._alpha = (float) this._age / (float) this._expandTime;
      this._buttonRadius = (int) ((double) this._age / (double) this._expandTime * (double) this._expandedButtonRadius);
      this._SnapToPlayerPosition();
      Vector2 vector2_1 = new Vector2();
      if (this.gamepadMode)
      {
        this._mouseStartPosition = Game1.getMousePosition(false);
        if ((double) Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.X) > 0.5 || (double) Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.Y) > 0.5)
        {
          this._hasSelectedEmote = true;
          vector2_1 = new Vector2(Game1.input.GetGamePadState().ThumbSticks.Right.X, Game1.input.GetGamePadState().ThumbSticks.Right.Y);
          vector2_1.Y *= -1f;
          vector2_1.Normalize();
          float num1 = -1f;
          for (int index = 0; index < this._emoteButtons.Count; ++index)
          {
            Vector2 vector2_2 = new Vector2((float) this._emoteButtons[index].bounds.Center.X - ((float) this.xPositionOnScreen + (float) this.width / 2f), (float) this._emoteButtons[index].bounds.Center.Y - ((float) this.yPositionOnScreen + (float) this.height / 2f));
            float num2 = Vector2.Dot(vector2_1, vector2_2);
            if ((double) num2 > (double) num1)
            {
              num1 = num2;
              this._selectedEmote = this.emotes[index];
              this._selectedIndex = index;
            }
          }
          this._menuCloseGracePeriod = 100;
          if (Game1.input.GetGamePadState().IsButtonDown(Buttons.Back) && this._selectedIndex >= 0)
          {
            Game1.activeClickableMenu = (IClickableMenu) new EmoteSelector(this._selectedIndex, this.emotes[this._selectedIndex]);
            this.exitThisMenuNoSound();
            return;
          }
        }
        else
        {
          if (Game1.input.GetGamePadState().IsButtonDown(Buttons.RightStick) && this._menuCloseGracePeriod < 100)
            this._menuCloseGracePeriod = 100;
          if (this._menuCloseGracePeriod >= 0)
            this._menuCloseGracePeriod -= time.ElapsedGameTime.Milliseconds;
          if (this._menuCloseGracePeriod <= 0 && !Game1.input.GetGamePadState().IsButtonDown(Buttons.RightStick))
            this.ConfirmSelection();
        }
      }
      for (int index = 0; index < this._emoteButtons.Count; ++index)
      {
        if ((double) this._emoteButtons[index].scale > 4.0)
          this._emoteButtons[index].scale = Utility.MoveTowards(this._emoteButtons[index].scale, 4f, (float) ((double) time.ElapsedGameTime.Milliseconds / 1000.0 * 10.0));
      }
      if (this._selectedEmote != null && this._selectedIndex > -1)
        this._emoteButtons[this._selectedIndex].scale = 5f;
      if (this._oldSelection != this._selectedIndex)
      {
        this._oldSelection = this._selectedIndex;
        this._selectedTime = 0;
      }
      this._selectedTime += time.ElapsedGameTime.Milliseconds;
      base.update(time);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      x = (int) Utility.ModifyCoordinateFromUIScale((float) x);
      y = (int) Utility.ModifyCoordinateFromUIScale((float) y);
      for (int index = 0; index < this._emoteButtons.Count; ++index)
      {
        if (this._emoteButtons[index].containsPoint(x, y) && Game1.activeClickableMenu == null)
        {
          Game1.activeClickableMenu = (IClickableMenu) new EmoteSelector(index, this.emotes[index]);
          this.exitThisMenuNoSound();
          return;
        }
      }
      base.receiveLeftClick(x, y, playSound);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      x = (int) Utility.ModifyCoordinateFromUIScale((float) x);
      y = (int) Utility.ModifyCoordinateFromUIScale((float) y);
      this.ConfirmSelection();
      base.receiveLeftClick(x, y, playSound);
    }

    public void ConfirmSelection()
    {
      if (this._selectedEmote != null)
        Game1.chatBox.textBoxEnter("/emote " + this._selectedEmote);
      this.exitThisMenu(false);
    }

    public override void draw(SpriteBatch b)
    {
      Game1.StartWorldDrawInUI(b);
      Color white = Color.White with
      {
        A = (byte) Utility.Lerp(0.0f, (float) byte.MaxValue, this._alpha)
      };
      foreach (ClickableTextureComponent emoteButton in this._emoteButtons)
        emoteButton.draw(b, white, 0.86f);
      if (this._selectedEmote != null)
      {
        foreach (Farmer.EmoteType emoteType in Farmer.EMOTES)
        {
          if (emoteType.emoteString == this._selectedEmote)
          {
            SpriteText.drawStringWithScrollCenteredAt(b, emoteType.displayName, this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height);
            break;
          }
        }
      }
      if (this._selectedIndex >= 0 && this._selectedTime >= 250)
      {
        Vector2 position = Utility.PointToVector2(this._emoteButtons[this._selectedIndex].bounds.Center);
        position.X += 16f;
        if (!this.gamepadMode)
        {
          position = Utility.PointToVector2(Game1.getMousePosition(false)) + new Vector2(32f, 32f);
          b.Draw(this.menuBackgroundTexture, position, new Rectangle?(new Rectangle(64, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.99f);
        }
        else
          b.Draw(Game1.controllerMaps, position, new Rectangle?(Utility.controllerMapSourceRect(new Rectangle(625, 260, 28, 28))), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        position.X += 32f;
        b.Draw(this.menuBackgroundTexture, position, new Rectangle?(new Rectangle(64, 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.99f);
      }
      Game1.EndWorldDrawInUI(b);
    }
  }
}
