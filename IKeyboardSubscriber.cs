// Decompiled with JetBrains decompiler
// Type: StardewValley.IKeyboardSubscriber
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Input;

namespace StardewValley
{
  public interface IKeyboardSubscriber
  {
    void RecieveTextInput(char inputChar);

    void RecieveTextInput(string text);

    void RecieveCommandInput(char command);

    void RecieveSpecialInput(Keys key);

    bool Selected { get; set; }
  }
}
