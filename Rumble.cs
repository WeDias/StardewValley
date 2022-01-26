// Decompiled with JetBrains decompiler
// Type: StardewValley.Rumble
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StardewValley
{
  [InstanceStatics]
  public static class Rumble
  {
    private static float rumbleStrength;
    private static float rumbleTimerMax;
    private static float rumbleTimerCurrent;
    private static float rumbleDuringFade;
    private static float maxRumbleDuringFade;
    private static bool isRumbling;
    private static bool fade;

    public static void update(float milliseconds)
    {
      float num = 0.0f;
      if (Rumble.isRumbling)
      {
        num = Rumble.rumbleStrength;
        Rumble.rumbleTimerCurrent += milliseconds;
        if ((double) Rumble.rumbleTimerCurrent > (double) Rumble.rumbleTimerMax)
          num = 0.0f;
        else if (Rumble.fade)
        {
          if ((double) Rumble.rumbleTimerCurrent > (double) Rumble.rumbleTimerMax - 1000.0)
            Rumble.rumbleDuringFade = Utility.Lerp(Rumble.maxRumbleDuringFade, 0.0f, (float) (((double) Rumble.rumbleTimerCurrent - ((double) Rumble.rumbleTimerMax - 1000.0)) / 1000.0));
          num = Rumble.rumbleDuringFade;
        }
      }
      if ((double) num <= 0.0)
      {
        num = 0.0f;
        Rumble.isRumbling = false;
      }
      if ((double) num > 1.0)
        num = 1f;
      if (!Game1.options.gamepadControls || !Game1.options.rumble)
        num = 0.0f;
      if (Game1.playerOneIndex == ~PlayerIndex.One)
        return;
      GamePad.SetVibration(Game1.playerOneIndex, num, num);
    }

    public static void stopRumbling()
    {
      Rumble.rumbleStrength = 0.0f;
      Rumble.isRumbling = false;
    }

    public static void rumble(float leftPower, float rightPower, float milliseconds) => Rumble.rumble(leftPower, milliseconds);

    public static void rumble(float power, float milliseconds)
    {
      if (Rumble.isRumbling || !Game1.options.gamepadControls || !Game1.options.rumble)
        return;
      Rumble.fade = false;
      Rumble.rumbleTimerCurrent = 0.0f;
      Rumble.rumbleTimerMax = milliseconds;
      Rumble.isRumbling = true;
      Rumble.rumbleStrength = power;
    }

    public static void rumbleAndFade(float power, float milliseconds)
    {
      if (Rumble.isRumbling || !Game1.options.gamepadControls || !Game1.options.rumble)
        return;
      Rumble.rumbleTimerCurrent = 0.0f;
      Rumble.rumbleTimerMax = milliseconds;
      Rumble.isRumbling = true;
      Rumble.fade = true;
      Rumble.rumbleDuringFade = power;
      Rumble.maxRumbleDuringFade = power;
      Rumble.rumbleStrength = power;
    }
  }
}
