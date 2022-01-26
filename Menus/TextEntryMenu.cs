// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TextEntryMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class TextEntryMenu : IClickableMenu
  {
    public const int borderSpace = 4;
    public const int buttonSize = 16;
    public const int windowWidth = 168;
    public const int windowHeight = 88;
    public string[][] letterMaps = new string[3][]
    {
      new string[4]
      {
        "1234567890",
        "qwertyuiop",
        "asdfghjkl'",
        "zxcvbnm,.?"
      },
      new string[4]
      {
        "!@#$%^&*()",
        "QWERTYUIOP",
        "ASDFGHJKL\"",
        "ZXCVBNM,.?"
      },
      new string[4]
      {
        "&%#|~$£~/\\",
        "-+=<>:;'\"`",
        "()[]{}.^°ñ",
        "áéíóúü¡!¿?"
      }
    };
    public List<ClickableTextureComponent> keys = new List<ClickableTextureComponent>();
    public ClickableTextureComponent backspaceButton;
    public ClickableTextureComponent spaceButton;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent upperCaseButton;
    public ClickableTextureComponent symbolsButton;
    protected int _lettersPerRow;
    protected TextBox _target;
    public int _currentKeyboard;

    public override void receiveGamePadButton(Buttons b)
    {
      switch (b)
      {
        case Buttons.Start:
          this.OnSubmit();
          break;
        case Buttons.B:
          this.Close();
          break;
        case Buttons.X:
          this.OnBackSpace();
          break;
        case Buttons.Y:
          this.OnSpaceBar();
          break;
        default:
          base.receiveGamePadButton(b);
          break;
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      if (key == Keys.Escape)
        this.Close();
      base.receiveKeyPress(key);
    }

    public TextEntryMenu(TextBox target)
      : base((int) Utility.getTopLeftPositionForCenteringOnScreen(672, 352).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(672, 352).Y, 672, 352)
    {
      this._target = target;
      this._lettersPerRow = this.letterMaps[0][0].Length;
      for (int index1 = 0; index1 < this.letterMaps[0].Length; ++index1)
      {
        for (int index2 = 0; index2 < this._lettersPerRow; ++index2)
        {
          ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(0, 0, 1024, 1024), Game1.mouseCursors2, new Rectangle(32, 176, 16, 16), 4f);
          textureComponent.myID = index1 * this._lettersPerRow + index2;
          textureComponent.leftNeighborID = -99998;
          textureComponent.rightNeighborID = -99998;
          textureComponent.upNeighborID = -99998;
          textureComponent.downNeighborID = -99998;
          if (index1 == this.letterMaps[0].Length - 1)
          {
            if (index2 >= 2 && index2 <= this._lettersPerRow - 4)
            {
              textureComponent.downNeighborID = 99991;
              textureComponent.downNeighborImmutable = true;
            }
            if (index2 >= this._lettersPerRow - 3 && index2 <= this._lettersPerRow - 2)
            {
              textureComponent.downNeighborID = 99990;
              textureComponent.downNeighborImmutable = true;
            }
          }
          this.keys.Add(textureComponent);
        }
      }
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(0, 0, 128, 64), Game1.mouseCursors2, new Rectangle(32, 144, 32, 16), 4f);
      textureComponent1.myID = 99990;
      textureComponent1.leftNeighborID = -99998;
      textureComponent1.rightNeighborID = -99998;
      textureComponent1.upNeighborID = -99998;
      textureComponent1.downNeighborID = -99998;
      this.backspaceButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(0, 0, 320, 64), Game1.mouseCursors2, new Rectangle(0, 160, 80, 16), 4f);
      textureComponent2.myID = 99991;
      textureComponent2.leftNeighborID = -99998;
      textureComponent2.rightNeighborID = -99998;
      textureComponent2.upNeighborID = -99998;
      textureComponent2.downNeighborID = -99998;
      this.spaceButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(64, 144, 16, 16), 4f);
      textureComponent3.myID = 99992;
      textureComponent3.leftNeighborID = -99998;
      textureComponent3.rightNeighborID = -99998;
      textureComponent3.upNeighborID = -99998;
      textureComponent3.downNeighborID = -99998;
      this.okButton = textureComponent3;
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(0, 144, 16, 16), 4f);
      textureComponent4.myID = 99993;
      textureComponent4.leftNeighborID = -99998;
      textureComponent4.rightNeighborID = -99998;
      textureComponent4.upNeighborID = -99998;
      textureComponent4.downNeighborID = -99998;
      this.upperCaseButton = textureComponent4;
      ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(16, 144, 16, 16), 4f);
      textureComponent5.myID = 99994;
      textureComponent5.leftNeighborID = -99998;
      textureComponent5.rightNeighborID = -99998;
      textureComponent5.upNeighborID = -99998;
      textureComponent5.downNeighborID = -99998;
      this.symbolsButton = textureComponent5;
      this.ShowKeyboard(0, false);
      this.RepositionElements();
      this.populateClickableComponentList();
      if (Game1.options.SnappyMenus)
        this.snapToDefaultClickableComponent();
      Game1.playSound("bigSelect");
    }

    public override bool readyToClose() => false;

    public void ShowKeyboard(int index, bool play_sound = true)
    {
      this._currentKeyboard = index;
      int index1 = 0;
      foreach (string str in this.letterMaps[index])
      {
        foreach (char ch in str)
        {
          this.keys[index1].name = ch.ToString() ?? "";
          ++index1;
        }
      }
      this.upperCaseButton.sourceRect = new Rectangle(0, 144, 16, 16);
      this.symbolsButton.sourceRect = new Rectangle(16, 144, 16, 16);
      if (this._currentKeyboard == 1)
        this.upperCaseButton.sourceRect = new Rectangle(0, 176, 16, 16);
      else if (this._currentKeyboard == 2)
        this.symbolsButton.sourceRect = new Rectangle(16, 176, 16, 16);
      if (!play_sound)
        return;
      Game1.playSound("button1");
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(this._lettersPerRow);
      this.snapCursorToCurrentSnappedComponent();
    }

    public void RepositionElements()
    {
      this.xPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(672, 352).X;
      this.yPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(672, 256).Y;
      for (int index1 = 0; index1 < this.keys.Count / this._lettersPerRow; ++index1)
      {
        for (int index2 = 0; index2 < this._lettersPerRow; ++index2)
          this.keys[index2 + index1 * this._lettersPerRow].bounds = new Rectangle(this.xPositionOnScreen + 16 + index2 * 16 * 4, this.yPositionOnScreen + 16 + index1 * 16 * 4, 64, 64);
      }
      this.upperCaseButton.bounds = new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + 256, this.upperCaseButton.bounds.Width, this.upperCaseButton.bounds.Height);
      this.symbolsButton.bounds = new Rectangle(this.xPositionOnScreen + 16 + 64, this.yPositionOnScreen + 16 + 256, this.symbolsButton.bounds.Width, this.symbolsButton.bounds.Height);
      this.backspaceButton.bounds = new Rectangle(this.xPositionOnScreen + 16 + 448, this.yPositionOnScreen + 16 + 256, this.backspaceButton.bounds.Width, this.backspaceButton.bounds.Height);
      this.spaceButton.bounds = new Rectangle(this.xPositionOnScreen + 16 + 128, this.yPositionOnScreen + 16 + 256, this.spaceButton.bounds.Width, this.spaceButton.bounds.Height);
      this.okButton.bounds = new Rectangle(this.xPositionOnScreen + 16 + 576, this.yPositionOnScreen + 16 + 256, this.okButton.bounds.Width, this.okButton.bounds.Height);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.RepositionElements();
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      foreach (ClickableTextureComponent key in this.keys)
        key.tryHover(x, y);
      this.spaceButton.tryHover(x, y);
      this.backspaceButton.tryHover(x, y);
      this.okButton.tryHover(x, y);
      this.symbolsButton.tryHover(x, y);
      this.upperCaseButton.tryHover(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      foreach (ClickableTextureComponent key in this.keys)
      {
        if (key.containsPoint(x, y))
          this.OnLetter(key.name);
      }
      if (this.okButton.containsPoint(x, y))
      {
        this.OnSubmit();
      }
      else
      {
        if (this.spaceButton.containsPoint(x, y))
          this.OnSpaceBar();
        if (this.upperCaseButton.containsPoint(x, y))
        {
          if (this._currentKeyboard != 1)
            this.ShowKeyboard(1);
          else
            this.ShowKeyboard(0);
        }
        if (this.symbolsButton.containsPoint(x, y))
        {
          if (this._currentKeyboard != 2)
            this.ShowKeyboard(2);
          else
            this.ShowKeyboard(0);
        }
        if (!this.backspaceButton.containsPoint(x, y))
          return;
        this.OnBackSpace();
      }
    }

    public void OnSubmit()
    {
      this._target.RecieveCommandInput('\r');
      this.Close();
    }

    public void OnSpaceBar() => this._target.RecieveTextInput(' ');

    public void OnBackSpace() => this._target.RecieveCommandInput('\b');

    public void OnLetter(string letter)
    {
      if (letter.Length <= 0)
        return;
      this._target.RecieveTextInput(letter[0]);
    }

    public void Close()
    {
      Game1.playSound("bigDeSelect");
      Game1.closeTextEntry();
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.8f);
      Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
      foreach (ClickableTextureComponent key in this.keys)
      {
        key.draw(b);
        Vector2 vector2 = Game1.dialogueFont.MeasureString(key.name);
        b.DrawString(Game1.dialogueFont, key.name, Utility.snapDrawPosition(new Vector2((float) key.bounds.Center.X - vector2.X / 2f, (float) key.bounds.Center.Y - vector2.Y / 2f)), Color.Black);
      }
      this.backspaceButton.draw(b);
      this.okButton.draw(b);
      this.spaceButton.draw(b);
      this.symbolsButton.draw(b);
      this.upperCaseButton.draw(b);
      if (this._target != null)
      {
        int x = this._target.X;
        int y = this._target.Y;
        this._target.X = (int) Utility.getTopLeftPositionForCenteringOnScreen(this._target.Width, this._target.Height * 4).X;
        this._target.Y = this.yPositionOnScreen - 96;
        this._target.Draw(b);
        this._target.X = x;
        this._target.Y = y;
      }
      base.draw(b);
      this.drawMouse(b, true);
    }

    public override void update(GameTime time)
    {
      if (this._target == null || !this._target.Selected)
        this.Close();
      if (Game1.input.GetGamePadState().IsButtonDown(Buttons.LeftStick) && !Game1.oldPadState.IsButtonDown(Buttons.LeftStick))
      {
        if (this._currentKeyboard != 1)
          this.ShowKeyboard(1);
        else
          this.ShowKeyboard(0);
      }
      if (!Game1.input.GetGamePadState().IsButtonDown(Buttons.RightStick) || Game1.oldPadState.IsButtonDown(Buttons.RightStick))
        return;
      if (this._currentKeyboard != 2)
        this.ShowKeyboard(2);
      else
        this.ShowKeyboard(0);
    }
  }
}
