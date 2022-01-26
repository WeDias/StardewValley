// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChatSnippet
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Menus
{
  public class ChatSnippet
  {
    public string message;
    public int emojiIndex = -1;
    public float myLength;

    public ChatSnippet(string message, LocalizedContentManager.LanguageCode language)
    {
      this.message = message;
      this.myLength = ChatBox.messageFont(language).MeasureString(message).X;
    }

    public ChatSnippet(int emojiIndex)
    {
      this.emojiIndex = emojiIndex;
      this.myLength = 40f;
    }
  }
}
