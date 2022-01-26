// Decompiled with JetBrains decompiler
// Type: StardewValley.InputState
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class InputState
  {
    protected Point _simulatedMousePosition = Point.Zero;
    protected List<Keys> _ignoredKeys = new List<Keys>();
    protected List<Keys> _pressedKeys = new List<Keys>();
    protected KeyboardState? _keyState;
    protected int _lastKeyStateTick = -1;
    protected KeyboardState _currentKeyboardState;
    protected MouseState _currentMouseState;
    protected GamePadState _currentGamepadState;

    public virtual void UpdateStates()
    {
      this._currentKeyboardState = Keyboard.GetState();
      this._currentMouseState = Mouse.GetState();
      if (Game1.playerOneIndex >= PlayerIndex.One)
        this._currentGamepadState = GamePad.GetState(Game1.playerOneIndex);
      else
        this._currentGamepadState = new GamePadState();
    }

    public virtual void Update()
    {
    }

    public virtual void IgnoreKeys(Keys[] keys)
    {
      if (keys.Length == 0)
        return;
      this._ignoredKeys.AddRange((IEnumerable<Keys>) keys);
      string str = "";
      foreach (Keys key in keys)
        str = str + key.ToString() + " ";
      Console.WriteLine("Ignoring keys: " + str.Trim());
    }

    public virtual KeyboardState GetKeyboardState()
    {
      if (!Game1.game1.IsMainInstance || !Game1.game1.HasKeyboardFocus())
        return new KeyboardState();
      if (this._lastKeyStateTick != Game1.ticks || !this._keyState.HasValue)
      {
        if (this._ignoredKeys.Count == 0)
        {
          this._keyState = new KeyboardState?(this._currentKeyboardState);
        }
        else
        {
          this._pressedKeys.Clear();
          this._pressedKeys.AddRange((IEnumerable<Keys>) this._currentKeyboardState.GetPressedKeys());
          for (int index = 0; index < this._ignoredKeys.Count; ++index)
          {
            if (!this._pressedKeys.Contains(this._ignoredKeys[index]))
            {
              this._ignoredKeys.RemoveAt(index);
              --index;
            }
          }
          for (int index = 0; index < this._pressedKeys.Count; ++index)
          {
            if (this._ignoredKeys.Contains(this._pressedKeys[index]))
            {
              this._pressedKeys.RemoveAt(index);
              --index;
            }
          }
          this._keyState = new KeyboardState?(new KeyboardState(this._pressedKeys.ToArray()));
        }
        this._lastKeyStateTick = Game1.ticks;
      }
      return this._keyState.Value;
    }

    public virtual GamePadState GetGamePadState() => Game1.options.gamepadMode == Options.GamepadModes.ForceOff || Game1.playerOneIndex == ~PlayerIndex.One ? new GamePadState() : this._currentGamepadState;

    public virtual MouseState GetMouseState() => !Game1.game1.IsMainInstance ? new MouseState(this._simulatedMousePosition.X, this._simulatedMousePosition.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released) : this._currentMouseState;

    public virtual void SetMousePosition(int x, int y)
    {
      if (!Game1.game1.IsMainInstance)
      {
        this._simulatedMousePosition.X = x;
        this._simulatedMousePosition.Y = y;
      }
      else
      {
        Mouse.SetPosition(x, y);
        this._currentMouseState = new MouseState(x, y, this._currentMouseState.ScrollWheelValue, this._currentMouseState.LeftButton, this._currentMouseState.MiddleButton, this._currentMouseState.RightButton, this._currentMouseState.XButton1, this._currentMouseState.XButton2);
      }
    }
  }
}
