// Decompiled with JetBrains decompiler
// Type: StardewValley.HUDMessage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

namespace StardewValley
{
  public class HUDMessage
  {
    public const float defaultTime = 3500f;
    public const int achievement_type = 1;
    public const int newQuest_type = 2;
    public const int error_type = 3;
    public const int stamina_type = 4;
    public const int health_type = 5;
    public const int screenshot_type = 6;
    public string message;
    public string type;
    public Color color;
    public float timeLeft;
    public float transparency = 1f;
    public int number = -1;
    public int whatType;
    public bool add;
    public bool achievement;
    public bool fadeIn;
    public bool noIcon;
    private Item messageSubject;

    public string Message
    {
      get
      {
        if (this.type == null)
          return this.message;
        return this.type.Equals("Money") ? (this.add ? "+ " : "- ") + this.number.ToString() + "g" : (this.add ? "+ " : "- ") + this.number.ToString() + " " + this.type;
      }
      set => this.message = value;
    }

    public HUDMessage(string message)
    {
      this.message = message;
      this.color = Color.SeaGreen;
      this.timeLeft = 3500f;
    }

    public HUDMessage(string message, bool achievement)
    {
      if (!achievement)
        return;
      this.message = Game1.content.LoadString("Strings\\StringsFromCSFiles:HUDMessage.cs.3824") + message;
      this.color = Color.OrangeRed;
      this.timeLeft = 5250f;
      this.achievement = true;
      this.whatType = 1;
    }

    public HUDMessage(string message, int whatType)
    {
      this.message = message;
      this.color = Color.OrangeRed;
      this.timeLeft = 5250f;
      this.achievement = true;
      this.whatType = whatType;
    }

    public HUDMessage(string type, int number, bool add, Color color, Item messageSubject = null)
    {
      this.type = type;
      this.add = add;
      this.color = color;
      this.timeLeft = 3500f;
      this.number = number;
      this.messageSubject = messageSubject;
    }

    public HUDMessage(string message, Color color, float timeLeft)
      : this(message, color, timeLeft, false)
    {
    }

    /// <summary>constructor for simpmle message box no icon.</summary>
    /// <param name="message"></param>
    /// <param name="textColor"></param>
    public HUDMessage(string message, string leaveMeNull)
    {
      this.message = Game1.parseText(message, Game1.dialogueFont, 384);
      this.timeLeft = 3500f;
      this.color = Game1.textColor;
      this.noIcon = true;
    }

    public HUDMessage(string message, Color color, float timeLeft, bool fadeIn)
    {
      this.message = message;
      this.color = color;
      this.timeLeft = timeLeft;
      this.fadeIn = fadeIn;
      if (!fadeIn)
        return;
      this.transparency = 0.0f;
    }

    public virtual bool update(GameTime time)
    {
      this.timeLeft -= (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.timeLeft < 0.0)
      {
        this.transparency -= 0.02f;
        if ((double) this.transparency < 0.0)
          return true;
      }
      else if (this.fadeIn)
        this.transparency = Math.Min(this.transparency + 0.02f, 1f);
      return false;
    }

    public virtual void draw(SpriteBatch b, int i)
    {
      Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
      if (this.noIcon)
      {
        int overrideX = titleSafeArea.Left + 16;
        int overrideY = (Game1.uiViewport.Width < 1400 ? -64 : 0) + titleSafeArea.Bottom - (i + 1) * 64 * 7 / 4 - 21 - (int) Game1.dialogueFont.MeasureString(this.message).Y;
        IClickableMenu.drawHoverText(b, this.message, Game1.dialogueFont, overrideX: overrideX, overrideY: overrideY, alpha: this.transparency);
      }
      else
      {
        Vector2 vector2 = new Vector2((float) (titleSafeArea.Left + 16), (float) (titleSafeArea.Bottom - (i + 1) * 64 * 7 / 4 - 64));
        if (Game1.isOutdoorMapSmallerThanViewport())
          vector2.X = (float) Math.Max(titleSafeArea.Left + 16, -Game1.uiViewport.X + 16);
        if (Game1.uiViewport.Width < 1400)
          vector2.Y -= 48f;
        b.Draw(Game1.mouseCursors, vector2, new Rectangle?(this.messageSubject == null || !(this.messageSubject is Object) || (this.messageSubject as Object).sellToStorePrice() <= 500 ? new Rectangle(293, 360, 26, 24) : new Rectangle(163, 399, 26, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        float x = Game1.smallFont.MeasureString(this.messageSubject == null || this.messageSubject.DisplayName == null ? (this.message == null ? "" : this.message) : this.messageSubject.DisplayName).X;
        b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f, vector2.Y), new Rectangle?(new Rectangle(319, 360, 1, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, new Vector2(x, 4f), SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f + x, vector2.Y), new Rectangle?(new Rectangle(323, 360, 6, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        vector2.X += 16f;
        vector2.Y += 16f;
        if (this.messageSubject == null)
        {
          switch (this.whatType)
          {
            case 1:
              b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(294, 392, 16, 16)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
              break;
            case 2:
              b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(403, 496, 5, 14)), Color.White * this.transparency, 0.0f, new Vector2(3f, 7f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
              break;
            case 3:
              b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(268, 470, 16, 16)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
              break;
            case 4:
              b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(0, 411, 16, 16)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
              break;
            case 5:
              b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(16, 411, 16, 16)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
              break;
            case 6:
              b.Draw(Game1.mouseCursors2, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(96, 32, 16, 16)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
              break;
          }
        }
        else
          this.messageSubject.drawInMenu(b, vector2, 1f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), this.transparency, 1f, StackDrawType.Hide);
        vector2.X += 51f;
        vector2.Y += 51f;
        if (this.number > 1)
          Utility.drawTinyDigits(this.number, b, vector2, 3f, 1f, Color.White * this.transparency);
        vector2.X += 32f;
        vector2.Y -= 33f;
        Utility.drawTextWithShadow(b, this.messageSubject == null ? this.message : this.messageSubject.DisplayName, Game1.smallFont, vector2, Game1.textColor * this.transparency, layerDepth: 1f, shadowIntensity: this.transparency);
      }
    }
  }
}
