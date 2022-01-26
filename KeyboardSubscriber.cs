// Decompiled with JetBrains decompiler
// Type: StardewValley.KeyboardDispatcher
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace StardewValley
{
  public class KeyboardDispatcher
  {
    protected string _enteredText;
    protected List<char> _commandInputs = new List<char>();
    protected List<Keys> _keysDown = new List<Keys>();
    protected List<char> _charsEntered = new List<char>();
    protected GameWindow _window;
    protected KeyboardState _oldKeyboardState;
    private IKeyboardSubscriber _subscriber;
    private string _pasteResult = "";

    public void Cleanup()
    {
      if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        this._window.TextInput -= new EventHandler<TextInputEventArgs>(this.Event_TextInput);
      }
      else
      {
        KeyboardInput.CharEntered -= new CharEnteredHandler(this.EventInput_CharEntered);
        KeyboardInput.KeyDown -= new KeyEventHandler(this.EventInput_KeyDown);
      }
      this._window = (GameWindow) null;
    }

    public KeyboardDispatcher(GameWindow window)
    {
      this._commandInputs = new List<char>();
      this._keysDown = new List<Keys>();
      this._charsEntered = new List<char>();
      this._window = window;
      if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        window.TextInput += new EventHandler<TextInputEventArgs>(this.Event_TextInput);
      }
      else
      {
        if (Game1.game1.IsMainInstance)
          KeyboardInput.Initialize(window);
        KeyboardInput.CharEntered += new CharEnteredHandler(this.EventInput_CharEntered);
        KeyboardInput.KeyDown += new KeyEventHandler(this.EventInput_KeyDown);
      }
    }

    private void Event_KeyDown(object sender, Keys key)
    {
      if (this._subscriber == null)
        return;
      if (key == Keys.Back)
        this._commandInputs.Add('\b');
      if (key == Keys.Enter)
        this._commandInputs.Add('\r');
      if (key == Keys.Tab)
        this._commandInputs.Add('\t');
      this._keysDown.Add(key);
    }

    private void Event_TextInput(object sender, TextInputEventArgs e)
    {
      if (this._subscriber == null)
        return;
      if (e.Key == Keys.Back)
        this._commandInputs.Add('\b');
      else if (e.Key == Keys.Enter)
        this._commandInputs.Add('\r');
      else if (e.Key == Keys.Tab)
      {
        this._commandInputs.Add('\t');
      }
      else
      {
        if (char.IsControl(e.Character))
          return;
        this._charsEntered.Add(e.Character);
      }
    }

    private void EventInput_KeyDown(object sender, KeyEventArgs e) => this._keysDown.Add(e.KeyCode);

    private void EventInput_CharEntered(object sender, CharacterEventArgs e)
    {
      if (this._subscriber == null)
        return;
      if (char.IsControl(e.Character))
      {
        if (e.Character == '\u0016')
        {
          Thread thread = new Thread(new ThreadStart(this.PasteThread));
          thread.SetApartmentState(ApartmentState.STA);
          thread.Start();
          thread.Join();
          this._enteredText = this._pasteResult;
        }
        else
          this._commandInputs.Add(e.Character);
      }
      else
        this._charsEntered.Add(e.Character);
    }

    public bool ShouldSuppress() => false;

    public void Discard()
    {
      this._enteredText = (string) null;
      this._charsEntered.Clear();
      this._commandInputs.Clear();
      this._keysDown.Clear();
    }

    public void Poll()
    {
      KeyboardState keyboardState = Game1.input.GetKeyboardState();
      bool flag = !RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl) : keyboardState.IsKeyDown(Keys.LeftWindows) || keyboardState.IsKeyDown(Keys.RightWindows);
      if (((!keyboardState.IsKeyDown(Keys.V) ? 0 : (!this._oldKeyboardState.IsKeyDown(Keys.V) ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      {
        string output = (string) null;
        DesktopClipboard.GetText(ref output);
        if (output != null)
          this._enteredText = output;
      }
      this._oldKeyboardState = keyboardState;
      if (this._enteredText != null)
      {
        if (this._subscriber != null && !this.ShouldSuppress())
          this._subscriber.RecieveTextInput(this._enteredText);
        this._enteredText = (string) null;
      }
      if (this._charsEntered.Count > 0)
      {
        if (this._subscriber != null && !this.ShouldSuppress())
        {
          foreach (char inputChar in this._charsEntered)
          {
            this._subscriber.RecieveTextInput(inputChar);
            if (this._subscriber == null)
              break;
          }
        }
        this._charsEntered.Clear();
      }
      if (this._commandInputs.Count > 0)
      {
        if (this._subscriber != null && !this.ShouldSuppress())
        {
          foreach (char commandInput in this._commandInputs)
          {
            this._subscriber.RecieveCommandInput(commandInput);
            if (this._subscriber == null)
              break;
          }
        }
        this._commandInputs.Clear();
      }
      if (this._keysDown.Count <= 0)
        return;
      if (this._subscriber != null && !this.ShouldSuppress())
      {
        foreach (Keys key in this._keysDown)
        {
          this._subscriber.RecieveSpecialInput(key);
          if (this._subscriber == null)
            break;
        }
      }
      this._keysDown.Clear();
    }

    public IKeyboardSubscriber Subscriber
    {
      get => this._subscriber;
      set
      {
        if (this._subscriber == value)
          return;
        if (this._subscriber != null)
          this._subscriber.Selected = false;
        this._subscriber = value;
        if (this._subscriber == null)
          return;
        this._subscriber.Selected = true;
      }
    }

    [STAThread]
    private void PasteThread() => this._pasteResult = "";
  }
}
