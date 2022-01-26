// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.LeftRightClickSpamInputSimulator
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Util
{
  public class LeftRightClickSpamInputSimulator : IInputSimulator
  {
    private bool leftClickThisFrame;

    public void SimulateInput(
      ref bool actionButtonPressed,
      ref bool switchToolButtonPressed,
      ref bool useToolButtonPressed,
      ref bool useToolButtonReleased,
      ref bool addItemToInventoryButtonPressed,
      ref bool cancelButtonPressed,
      ref bool moveUpPressed,
      ref bool moveRightPressed,
      ref bool moveLeftPressed,
      ref bool moveDownPressed,
      ref bool moveUpReleased,
      ref bool moveRightReleased,
      ref bool moveLeftReleased,
      ref bool moveDownReleased,
      ref bool moveUpHeld,
      ref bool moveRightHeld,
      ref bool moveLeftHeld,
      ref bool moveDownHeld)
    {
      useToolButtonPressed = this.leftClickThisFrame;
      useToolButtonReleased = !this.leftClickThisFrame;
      actionButtonPressed = !this.leftClickThisFrame;
      this.leftClickThisFrame = !this.leftClickThisFrame;
    }
  }
}
