// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChatTextBox
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class ChatTextBox : TextBox
  {
    public IClickableMenu parentMenu;
    public List<ChatSnippet> finalText = new List<ChatSnippet>();
    public float currentWidth;

    public ChatTextBox(
      Texture2D textBoxTexture,
      Texture2D caretTexture,
      SpriteFont font,
      Color textColor)
      : base(textBoxTexture, caretTexture, font, textColor)
    {
    }

    public void reset()
    {
      this.currentWidth = 0.0f;
      this.finalText.Clear();
    }

    public void setText(string text)
    {
      this.reset();
      this.RecieveTextInput(text);
    }

    public override void RecieveTextInput(string text)
    {
      if (this.finalText.Count == 0)
        this.finalText.Add(new ChatSnippet("", LocalizedContentManager.CurrentLanguageCode));
      if ((double) this.currentWidth + (double) ChatBox.messageFont(LocalizedContentManager.CurrentLanguageCode).MeasureString(text).X >= (double) (this.Width - 16))
        return;
      if (this.finalText.Last<ChatSnippet>().message != null)
        this.finalText.Last<ChatSnippet>().message += text;
      else
        this.finalText.Add(new ChatSnippet(text, LocalizedContentManager.CurrentLanguageCode));
      this.updateWidth();
    }

    public override void RecieveTextInput(char inputChar) => this.RecieveTextInput(inputChar.ToString() ?? "");

    public override void RecieveCommandInput(char command)
    {
      if (this.Selected && command == '\b')
        this.backspace();
      else
        base.RecieveCommandInput(command);
    }

    public void backspace()
    {
      if (this.finalText.Count > 0)
      {
        if (this.finalText.Last<ChatSnippet>().message != null)
        {
          if (this.finalText.Last<ChatSnippet>().message.Length > 1)
            this.finalText.Last<ChatSnippet>().message = this.finalText.Last<ChatSnippet>().message.Remove(this.finalText.Last<ChatSnippet>().message.Length - 1);
          else
            this.finalText.RemoveAt(this.finalText.Count - 1);
        }
        else if (this.finalText.Last<ChatSnippet>().emojiIndex != -1)
          this.finalText.RemoveAt(this.finalText.Count - 1);
      }
      this.updateWidth();
    }

    public void receiveEmoji(int emoji)
    {
      if ((double) this.currentWidth + 40.0 > (double) (this.Width - 16))
        return;
      this.finalText.Add(new ChatSnippet(emoji));
      this.updateWidth();
    }

    public void updateWidth()
    {
      this.currentWidth = 0.0f;
      foreach (ChatSnippet chatSnippet in this.finalText)
      {
        if (chatSnippet.message != null)
          chatSnippet.myLength = ChatBox.messageFont(LocalizedContentManager.CurrentLanguageCode).MeasureString(chatSnippet.message).X;
        this.currentWidth += chatSnippet.myLength;
      }
    }

    public override void Draw(SpriteBatch spriteBatch, bool drawShadow = true)
    {
      bool flag = Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 >= 500.0;
      if (this._textBoxTexture != null)
      {
        spriteBatch.Draw(this._textBoxTexture, new Rectangle(this.X, this.Y, 16, this.Height), new Rectangle?(new Rectangle(0, 0, 16, this.Height)), Color.White);
        spriteBatch.Draw(this._textBoxTexture, new Rectangle(this.X + 16, this.Y, this.Width - 32, this.Height), new Rectangle?(new Rectangle(16, 0, 4, this.Height)), Color.White);
        spriteBatch.Draw(this._textBoxTexture, new Rectangle(this.X + this.Width - 16, this.Y, 16, this.Height), new Rectangle?(new Rectangle(this._textBoxTexture.Bounds.Width - 16, 0, 16, this.Height)), Color.White);
      }
      else
        Game1.drawDialogueBox(this.X - 32, this.Y - 112 + 10, this.Width + 80, this.Height, false, true);
      if (flag && this.Selected)
        spriteBatch.Draw(Game1.staminaRect, new Rectangle(this.X + 16 + (int) this.currentWidth - 2, this.Y + 8, 4, 32), this._textColor);
      float num = 0.0f;
      for (int index = 0; index < this.finalText.Count; ++index)
      {
        if (this.finalText[index].emojiIndex != -1)
          spriteBatch.Draw(ChatBox.emojiTexture, new Vector2((float) ((double) this.X + (double) num + 12.0), (float) (this.Y + 12)), new Rectangle?(new Rectangle(this.finalText[index].emojiIndex * 9 % ChatBox.emojiTexture.Width, this.finalText[index].emojiIndex * 9 / ChatBox.emojiTexture.Width * 9, 9, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        else if (this.finalText[index].message != null)
          spriteBatch.DrawString(ChatBox.messageFont(LocalizedContentManager.CurrentLanguageCode), this.finalText[index].message, new Vector2((float) ((double) this.X + (double) num + 12.0), (float) (this.Y + 12)), ChatMessage.getColorFromName(Game1.player.defaultChatColor), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        num += this.finalText[index].myLength;
      }
    }
  }
}
