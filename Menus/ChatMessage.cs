// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChatMessage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StardewValley.Menus
{
  public class ChatMessage
  {
    public List<ChatSnippet> message = new List<ChatSnippet>();
    public int timeLeftToDisplay;
    public int verticalSize;
    public float alpha = 1f;
    public Color color;
    public LocalizedContentManager.LanguageCode language;

    public void parseMessageForEmoji(string messagePlaintext)
    {
      if (messagePlaintext == null)
        return;
      StringBuilder sb = new StringBuilder();
      for (int index = 0; index < messagePlaintext.Count<char>(); ++index)
      {
        if (messagePlaintext[index] == '[')
        {
          if (sb.Length > 0)
            this.breakNewLines(sb);
          sb.Clear();
          int num1 = messagePlaintext.IndexOf(']', index);
          int num2 = -1;
          if (index + 1 < messagePlaintext.Count<char>())
            num2 = messagePlaintext.IndexOf('[', index + 1);
          if (num1 != -1 && (num2 == -1 || num2 > num1))
          {
            string str = messagePlaintext.Substring(index + 1, num1 - index - 1);
            int result = -1;
            if (int.TryParse(str, out result))
            {
              if (result < EmojiMenu.totalEmojis)
                this.message.Add(new ChatSnippet(result));
            }
            else
            {
              switch (str)
              {
                case "aqua":
                case "blue":
                case "brown":
                case "cream":
                case "gray":
                case "green":
                case "jade":
                case "jungle":
                case "orange":
                case "peach":
                case "pink":
                case "plum":
                case "purple":
                case "red":
                case "salmon":
                case "yellow":
                case "yellowgreen":
                  if (this.color.Equals(Color.White))
                  {
                    this.color = ChatMessage.getColorFromName(str);
                    break;
                  }
                  break;
                default:
                  sb.Append("[");
                  sb.Append(str);
                  sb.Append("]");
                  break;
              }
            }
            index = num1;
          }
          else
            sb.Append("[");
        }
        else
          sb.Append(messagePlaintext[index]);
      }
      if (sb.Length <= 0)
        return;
      this.breakNewLines(sb);
    }

    public static Color getColorFromName(string name)
    {
      switch (name)
      {
        case "aqua":
          return Color.MediumTurquoise;
        case "blue":
          return Color.DodgerBlue;
        case "brown":
          return new Color(160, 80, 30);
        case "cream":
          return new Color((int) byte.MaxValue, (int) byte.MaxValue, 180);
        case "gray":
          return Color.Gray;
        case "green":
          return new Color(0, 180, 10);
        case "jade":
          return new Color(50, 230, 150);
        case "jungle":
          return Color.SeaGreen;
        case "orange":
          return new Color((int) byte.MaxValue, 100, 0);
        case "peach":
          return new Color((int) byte.MaxValue, 180, 120);
        case "pink":
          return Color.HotPink;
        case "plum":
          return new Color(190, 0, 190);
        case "purple":
          return new Color(138, 43, 250);
        case "red":
          return new Color(220, 20, 20);
        case "salmon":
          return Color.Salmon;
        case "yellow":
          return new Color(240, 200, 0);
        case "yellowgreen":
          return new Color(182, 214, 0);
        default:
          return Color.White;
      }
    }

    private void breakNewLines(StringBuilder sb)
    {
      string[] strArray = sb.ToString().Split(new string[1]
      {
        Environment.NewLine
      }, StringSplitOptions.None);
      for (int index = 0; index < strArray.Length; ++index)
      {
        this.message.Add(new ChatSnippet(strArray[index], this.language));
        if (index != strArray.Length - 1)
          this.message.Add(new ChatSnippet(Environment.NewLine, this.language));
      }
    }

    public static string makeMessagePlaintext(
      List<ChatSnippet> message,
      bool include_color_information)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ChatSnippet chatSnippet in message)
      {
        if (chatSnippet.message != null)
          stringBuilder.Append(chatSnippet.message);
        else if (chatSnippet.emojiIndex != -1)
          stringBuilder.Append("[" + chatSnippet.emojiIndex.ToString() + "]");
      }
      if (include_color_information && Game1.player.defaultChatColor != null && !ChatMessage.getColorFromName(Game1.player.defaultChatColor).Equals(Color.White))
      {
        stringBuilder.Append(" [");
        stringBuilder.Append(Game1.player.defaultChatColor);
        stringBuilder.Append("]");
      }
      return stringBuilder.ToString();
    }

    public void draw(SpriteBatch b, int x, int y)
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      for (int index = 0; index < this.message.Count; ++index)
      {
        if (this.message[index].emojiIndex != -1)
          b.Draw(ChatBox.emojiTexture, new Vector2((float) ((double) x + (double) num1 + 1.0), (float) ((double) y + (double) num2 - 4.0)), new Rectangle?(new Rectangle(this.message[index].emojiIndex * 9 % ChatBox.emojiTexture.Width, this.message[index].emojiIndex * 9 / ChatBox.emojiTexture.Width * 9, 9, 9)), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        else if (this.message[index].message != null)
        {
          if (this.message[index].message.Equals(Environment.NewLine))
          {
            num1 = 0.0f;
            num2 += ChatBox.messageFont(this.language).MeasureString("(").Y;
          }
          else
            b.DrawString(ChatBox.messageFont(this.language), this.message[index].message, new Vector2((float) x + num1, (float) y + num2), this.color * this.alpha, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        }
        num1 += this.message[index].myLength;
        if ((double) num1 >= 888.0)
        {
          num1 = 0.0f;
          num2 += ChatBox.messageFont(this.language).MeasureString("(").Y;
          if (this.message.Count > index + 1 && this.message[index + 1].message != null && this.message[index + 1].message.Equals(Environment.NewLine))
            ++index;
        }
      }
    }
  }
}
