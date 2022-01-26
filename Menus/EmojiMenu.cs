// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.EmojiMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class EmojiMenu : IClickableMenu
  {
    public const int EMOJI_SIZE = 9;
    private Texture2D chatBoxTexture;
    private Texture2D emojiTexture;
    private ChatBox chatBox;
    private List<ClickableComponent> emojiSelectionButtons = new List<ClickableComponent>();
    private int pageStartIndex;
    private ClickableComponent upArrow;
    private ClickableComponent downArrow;
    private ClickableComponent sendArrow;
    public static int totalEmojis;

    public EmojiMenu(ChatBox chatBox, Texture2D emojiTexture, Texture2D chatBoxTexture)
    {
      this.chatBox = chatBox;
      this.chatBoxTexture = chatBoxTexture;
      this.emojiTexture = emojiTexture;
      this.width = 300;
      this.height = 248;
      for (int index1 = 0; index1 < 5; ++index1)
      {
        for (int index2 = 0; index2 < 6; ++index2)
          this.emojiSelectionButtons.Add(new ClickableComponent(new Rectangle(16 + index2 * 10 * 4, 16 + index1 * 10 * 4, 36, 36), (index2 + index1 * 6).ToString() ?? ""));
      }
      this.upArrow = new ClickableComponent(new Rectangle(256, 16, 32, 20), "");
      this.downArrow = new ClickableComponent(new Rectangle(256, 156, 32, 20), "");
      this.sendArrow = new ClickableComponent(new Rectangle(256, 188, 32, 32), "");
      EmojiMenu.totalEmojis = ChatBox.emojiTexture.Width / 9 * (ChatBox.emojiTexture.Height / 9);
    }

    public void leftClick(int x, int y, ChatBox cb)
    {
      if (!this.isWithinBounds(x, y))
        return;
      int x1 = x - this.xPositionOnScreen;
      int y1 = y - this.yPositionOnScreen;
      if (this.upArrow.containsPoint(x1, y1))
        this.upArrowPressed();
      else if (this.downArrow.containsPoint(x1, y1))
        this.downArrowPressed();
      else if (this.sendArrow.containsPoint(x1, y1) && (double) cb.chatBox.currentWidth > 0.0)
      {
        cb.textBoxEnter((TextBox) cb.chatBox);
        this.sendArrow.scale = 0.5f;
        Game1.playSound("shwip");
      }
      foreach (ClickableComponent emojiSelectionButton in this.emojiSelectionButtons)
      {
        if (emojiSelectionButton.containsPoint(x1, y1))
        {
          int emoji = this.pageStartIndex + int.Parse(emojiSelectionButton.name);
          cb.chatBox.receiveEmoji(emoji);
          Game1.playSound("coin");
          break;
        }
      }
    }

    private void upArrowPressed(int amountToScroll = 30)
    {
      if (this.pageStartIndex != 0)
        Game1.playSound("Cowboy_Footstep");
      this.pageStartIndex = Math.Max(0, this.pageStartIndex - amountToScroll);
      this.upArrow.scale = 0.75f;
    }

    private void downArrowPressed(int amountToScroll = 30)
    {
      if (this.pageStartIndex != EmojiMenu.totalEmojis - 30)
        Game1.playSound("Cowboy_Footstep");
      this.pageStartIndex = Math.Min(EmojiMenu.totalEmojis - 30, this.pageStartIndex + amountToScroll);
      this.downArrow.scale = 0.75f;
    }

    public override void receiveScrollWheelAction(int direction)
    {
      if (direction < 0)
      {
        this.downArrowPressed(6);
      }
      else
      {
        if (direction <= 0)
          return;
        this.upArrowPressed(6);
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(this.chatBoxTexture, new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), new Rectangle?(new Rectangle(0, 56, 300, 244)), Color.White);
      for (int index = 0; index < this.emojiSelectionButtons.Count; ++index)
        b.Draw(this.emojiTexture, new Vector2((float) (this.emojiSelectionButtons[index].bounds.X + this.xPositionOnScreen), (float) (this.emojiSelectionButtons[index].bounds.Y + this.yPositionOnScreen)), new Rectangle?(new Rectangle((this.pageStartIndex + index) * 9 % this.emojiTexture.Width, (this.pageStartIndex + index) * 9 / this.emojiTexture.Width * 9, 9, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
      if ((double) this.upArrow.scale < 1.0)
        this.upArrow.scale += 0.05f;
      if ((double) this.downArrow.scale < 1.0)
        this.downArrow.scale += 0.05f;
      if ((double) this.sendArrow.scale < 1.0)
        this.sendArrow.scale += 0.05f;
      b.Draw(this.chatBoxTexture, new Vector2((float) (this.upArrow.bounds.X + this.xPositionOnScreen + 16), (float) (this.upArrow.bounds.Y + this.yPositionOnScreen + 10)), new Rectangle?(new Rectangle(156, 300, 32, 20)), Color.White * (this.pageStartIndex == 0 ? 0.25f : 1f), 0.0f, new Vector2(16f, 10f), this.upArrow.scale, SpriteEffects.None, 0.9f);
      b.Draw(this.chatBoxTexture, new Vector2((float) (this.downArrow.bounds.X + this.xPositionOnScreen + 16), (float) (this.downArrow.bounds.Y + this.yPositionOnScreen + 10)), new Rectangle?(new Rectangle(192, 300, 32, 20)), Color.White * (this.pageStartIndex == EmojiMenu.totalEmojis - 30 ? 0.25f : 1f), 0.0f, new Vector2(16f, 10f), this.downArrow.scale, SpriteEffects.None, 0.9f);
      b.Draw(this.chatBoxTexture, new Vector2((float) (this.sendArrow.bounds.X + this.xPositionOnScreen + 16), (float) (this.sendArrow.bounds.Y + this.yPositionOnScreen + 10)), new Rectangle?(new Rectangle(116, 304, 28, 28)), Color.White * ((double) this.chatBox.chatBox.currentWidth > 0.0 ? 1f : 0.4f), 0.0f, new Vector2(14f, 16f), this.sendArrow.scale, SpriteEffects.None, 0.9f);
    }
  }
}
