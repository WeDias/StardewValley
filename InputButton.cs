// Decompiled with JetBrains decompiler
// Type: StardewValley.InputButton
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Input;

namespace StardewValley
{
  public struct InputButton
  {
    public Keys key;
    public bool mouseLeft;
    public bool mouseRight;

    public InputButton(Keys key)
    {
      this.key = key;
      this.mouseLeft = false;
      this.mouseRight = false;
    }

    public InputButton(bool mouseLeft)
    {
      this.key = Keys.None;
      this.mouseLeft = mouseLeft;
      this.mouseRight = !mouseLeft;
    }

    public override string ToString()
    {
      if (this.mouseLeft)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Left-Click");
      if (this.mouseRight)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Right-Click");
      switch (this.key)
      {
        case Keys.D0:
          return "0";
        case Keys.D1:
          return "1";
        case Keys.D2:
          return "2";
        case Keys.D3:
          return "3";
        case Keys.D4:
          return "4";
        case Keys.D5:
          return "5";
        case Keys.D6:
          return "6";
        case Keys.D7:
          return "7";
        case Keys.D8:
          return "8";
        case Keys.D9:
          return "9";
        default:
          string str = this.key.ToString().Replace("Oem", "");
          if (Game1.content.LoadString("Strings\\StringsFromCSFiles:" + this.key.ToString().Replace("Oem", "")) != "Strings\\StringsFromCSFiles:" + this.key.ToString().Replace("Oem", ""))
            str = Game1.content.LoadString("Strings\\StringsFromCSFiles:" + this.key.ToString().Replace("Oem", ""));
          return str;
      }
    }
  }
}
