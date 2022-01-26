// Decompiled with JetBrains decompiler
// Type: StardewValley.KeyboardInput
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.InteropServices;

namespace StardewValley
{
  public static class KeyboardInput
  {
    private static bool initialized;
    private static IntPtr prevWndProc;
    private static KeyboardInput.WndProc hookProcDelegate;
    private static IntPtr hIMC;
    private const int GWL_WNDPROC = -4;
    private const int WM_KEYDOWN = 256;
    private const int WM_KEYUP = 257;
    private const int WM_CHAR = 258;
    private const int WM_IME_SETCONTEXT = 641;
    private const int WM_INPUTLANGCHANGE = 81;
    private const int WM_GETDLGCODE = 135;
    private const int WM_IME_COMPOSITION = 271;
    private const int DLGC_WANTALLKEYS = 4;

    /// <summary>Event raised when a character has been entered.</summary>
    public static event CharEnteredHandler CharEntered;

    /// <summary>
    /// Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.
    /// </summary>
    public static event KeyEventHandler KeyDown;

    /// <summary>Event raised when a key has been released.</summary>
    public static event KeyEventHandler KeyUp;

    [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr ImmGetContext(IntPtr hWnd);

    [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr CallWindowProc(
      IntPtr lpPrevWndFunc,
      IntPtr hWnd,
      uint Msg,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    /// <summary>Initialize the TextInput with the given GameWindow.</summary>
    /// <param name="window">The XNA window to which text input should be linked.</param>
    public static void Initialize(GameWindow window)
    {
      if (KeyboardInput.initialized)
        throw new InvalidOperationException("TextInput.Initialize can only be called once!");
      KeyboardInput.hookProcDelegate = new KeyboardInput.WndProc(KeyboardInput.HookProc);
      KeyboardInput.prevWndProc = (IntPtr) KeyboardInput.SetWindowLong(window.Handle, -4, (int) Marshal.GetFunctionPointerForDelegate<KeyboardInput.WndProc>(KeyboardInput.hookProcDelegate));
      KeyboardInput.hIMC = KeyboardInput.ImmGetContext(window.Handle);
      KeyboardInput.initialized = true;
    }

    private static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
      IntPtr num = KeyboardInput.CallWindowProc(KeyboardInput.prevWndProc, hWnd, msg, wParam, lParam);
      switch (msg)
      {
        case 81:
          KeyboardInput.ImmAssociateContext(hWnd, KeyboardInput.hIMC);
          num = (IntPtr) 1;
          break;
        case 135:
          num = (IntPtr) (num.ToInt32() | 4);
          break;
        case 256:
          if (KeyboardInput.KeyDown != null)
          {
            KeyboardInput.KeyDown((object) null, new KeyEventArgs((Keys) (int) wParam));
            break;
          }
          break;
        case 257:
          if (KeyboardInput.KeyUp != null)
          {
            KeyboardInput.KeyUp((object) null, new KeyEventArgs((Keys) (int) wParam));
            break;
          }
          break;
        case 258:
          if (KeyboardInput.CharEntered != null)
          {
            KeyboardInput.CharEntered((object) null, new CharacterEventArgs((char) (int) wParam, lParam.ToInt32()));
            break;
          }
          break;
        case 641:
          if (wParam.ToInt32() == 1)
          {
            KeyboardInput.ImmAssociateContext(hWnd, KeyboardInput.hIMC);
            break;
          }
          break;
      }
      return num;
    }

    private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
  }
}
