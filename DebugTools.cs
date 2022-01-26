// Decompiled with JetBrains decompiler
// Type: StardewValley.DebugTools
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;

namespace StardewValley
{
  public static class DebugTools
  {
    private static int _mainThreadId;
    private const string CommentFormat = "#----------------------------------------------------------------------------#";
    public static DebugMetricsComponent _metrics;
    private static bool _noFpsCap;

    public static string FormatDivider(string label = null)
    {
      if (string.IsNullOrEmpty(label))
        return "#----------------------------------------------------------------------------#";
      label = " " + label + " ";
      int length = "#----------------------------------------------------------------------------#".Length / 2 - label.Length / 2;
      int startIndex = length + label.Length;
      return "#----------------------------------------------------------------------------#".Substring(0, length) + label + "#----------------------------------------------------------------------------#".Substring(startIndex);
    }

    [Conditional("VALIDATE_MAIN_THREAD_ENABLED")]
    public static void ValidateIsMainThread(bool req)
    {
      if (Thread.CurrentThread.ManagedThreadId == DebugTools._mainThreadId == req)
        return;
      Console.WriteLine(DebugTools.FormatDivider("ERROR: CODE EXECUTED ON UNSAFE THREAD!"));
      Debugger.Break();
      Environment.Exit(666);
    }

    public static bool IsMainThread() => Thread.CurrentThread.ManagedThreadId == DebugTools._mainThreadId;

    public static void Assert(bool expression, string failureMessage)
    {
      if (expression)
        return;
      Console.WriteLine(failureMessage);
    }

    public static void GameConstructed(Game game) => DebugTools._mainThreadId = Thread.CurrentThread.ManagedThreadId;

    public static void GameLoadContent(Game game)
    {
    }

    public static void BeforeGameInitialize(Game game) => DebugTools.ApplyNoFpsCap(DebugTools._noFpsCap);

    public static void BeforeGameUpdate(Game1 game, ref GameTime gameTime)
    {
      if (Program.releaseBuild)
        return;
      DebugTools.CheckInput(game);
      if (!DebugTools._noFpsCap)
        return;
      gameTime = new GameTime(gameTime.TotalGameTime, game.TargetElapsedTime, gameTime.IsRunningSlowly);
    }

    public static void BeforeGameDraw(Game1 game, ref GameTime time)
    {
      if (!DebugTools._noFpsCap)
        return;
      time = new GameTime(time.TotalGameTime, game.TargetElapsedTime, time.IsRunningSlowly);
    }

    private static void CheckInput(Game1 game)
    {
      GamePadState gamePadState = Game1.input.GetGamePadState();
      if (Game1.IsPressEvent(ref gamePadState, Buttons.LeftStick))
      {
        if (DebugTools._metrics != null)
          DebugTools._metrics.Visible = !DebugTools._metrics.Visible;
        Console.WriteLine("Toggling Metrics ({0})", DebugTools._metrics == null ? (object) "[null]" : (object) DebugTools._metrics.Visible.ToString());
      }
      if (!Game1.IsPressEvent(ref gamePadState, Buttons.RightStick) || !gamePadState.IsButtonDown(Buttons.LeftStick))
        return;
      DebugTools._noFpsCap = !DebugTools._noFpsCap;
      DebugTools.ApplyNoFpsCap(DebugTools._noFpsCap);
    }

    private static void ApplyNoFpsCap(bool nocap) => Console.WriteLine("NoFpsCap: This feature is not available");
  }
}
