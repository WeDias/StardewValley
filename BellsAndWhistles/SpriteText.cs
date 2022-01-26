// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.SpriteText
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using BmFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.BellsAndWhistles
{
  public class SpriteText
  {
    public const int scrollStyle_scroll = 0;
    public const int scrollStyle_speechBubble = 1;
    public const int scrollStyle_darkMetal = 2;
    public const int maxCharacter = 999999;
    public const int maxHeight = 999999;
    public const int characterWidth = 8;
    public const int characterHeight = 16;
    public const int horizontalSpaceBetweenCharacters = 0;
    public const int verticalSpaceBetweenCharacters = 2;
    public const char newLine = '^';
    public static float fontPixelZoom = 3f;
    public static float shadowAlpha = 0.15f;
    private static Dictionary<char, FontChar> _characterMap;
    private static FontFile FontFile = (FontFile) null;
    private static List<Texture2D> fontPages = (List<Texture2D>) null;
    public static Texture2D spriteTexture;
    public static Texture2D coloredTexture;
    public const int color_Black = 0;
    public const int color_Blue = 1;
    public const int color_Red = 2;
    public const int color_Purple = 3;
    public const int color_White = 4;
    public const int color_Orange = 5;
    public const int color_Green = 6;
    public const int color_Cyan = 7;
    public const int color_Gray = 8;
    public static bool forceEnglishFont = false;

    public static void drawStringHorizontallyCenteredAt(
      SpriteBatch b,
      string s,
      int x,
      int y,
      int characterPosition = 999999,
      int width = -1,
      int height = 999999,
      float alpha = 1f,
      float layerDepth = 0.88f,
      bool junimoText = false,
      int color = -1,
      int maxWidth = 99999)
    {
      SpriteText.drawString(b, s, x - SpriteText.getWidthOfString(s, maxWidth) / 2, y, characterPosition, width, height, alpha, layerDepth, junimoText, color: color);
    }

    public static int getWidthOfString(string s, int widthConstraint = 999999)
    {
      SpriteText.setUpCharacterMap();
      int val1 = 0;
      int val2 = 0;
      for (int index = 0; index < s.Length; ++index)
      {
        if (!LocalizedContentManager.CurrentLanguageLatin && !SpriteText.forceEnglishFont)
        {
          FontChar fontChar;
          if (SpriteText._characterMap.TryGetValue(s[index], out fontChar))
            val1 += fontChar.XAdvance;
          val2 = Math.Max(val1, val2);
          if (s[index] == '^' || (double) val1 * (double) SpriteText.fontPixelZoom > (double) widthConstraint)
            val1 = 0;
        }
        else
        {
          val1 += 8 + SpriteText.getWidthOffsetForChar(s[index]);
          if (index > 0)
            val1 += SpriteText.getWidthOffsetForChar(s[Math.Max(0, index - 1)]);
          val2 = Math.Max(val1, val2);
          float num = (float) SpriteText.positionOfNextSpace(s, index, (int) ((double) val1 * (double) SpriteText.fontPixelZoom), 0);
          if (s[index] == '^' || (double) val1 * (double) SpriteText.fontPixelZoom >= (double) widthConstraint || (double) num >= (double) widthConstraint)
            val1 = 0;
        }
      }
      return (int) ((double) val2 * (double) SpriteText.fontPixelZoom);
    }

    public static bool IsMissingCharacters(string text)
    {
      SpriteText.setUpCharacterMap();
      if (!LocalizedContentManager.CurrentLanguageLatin && !SpriteText.forceEnglishFont)
      {
        for (int index = 0; index < text.Length; ++index)
        {
          if (!SpriteText._characterMap.ContainsKey(text[index]))
            return true;
        }
      }
      return false;
    }

    public static int getHeightOfString(string s, int widthConstraint = 999999)
    {
      if (s.Length == 0)
        return 0;
      Vector2 vector2 = new Vector2();
      int accumulatedHorizontalSpaceBetweenCharacters = 0;
      s = s.Replace(Environment.NewLine, "");
      SpriteText.setUpCharacterMap();
      if (!LocalizedContentManager.CurrentLanguageLatin && !SpriteText.forceEnglishFont)
      {
        for (int index = 0; index < s.Length; ++index)
        {
          if (s[index] == '^')
          {
            vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
            vector2.X = 0.0f;
          }
          else
          {
            if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= widthConstraint)
            {
              vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
              accumulatedHorizontalSpaceBetweenCharacters = 0;
              vector2.X = 0.0f;
            }
            FontChar fontChar;
            if (SpriteText._characterMap.TryGetValue(s[index], out fontChar))
              vector2.X += (float) fontChar.XAdvance * SpriteText.fontPixelZoom;
          }
        }
        return (int) ((double) vector2.Y + (double) (SpriteText.FontFile.Common.LineHeight + 2) * (double) SpriteText.fontPixelZoom);
      }
      for (int index = 0; index < s.Length; ++index)
      {
        if (s[index] == '^')
        {
          vector2.Y += 18f * SpriteText.fontPixelZoom;
          vector2.X = 0.0f;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
        }
        else
        {
          if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= widthConstraint)
          {
            vector2.Y += 18f * SpriteText.fontPixelZoom;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
            vector2.X = 0.0f;
          }
          vector2.X += (float) (8.0 * (double) SpriteText.fontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) SpriteText.getWidthOffsetForChar(s[index]) * (double) SpriteText.fontPixelZoom);
          if (index > 0)
            vector2.X += (float) SpriteText.getWidthOffsetForChar(s[index - 1]) * SpriteText.fontPixelZoom;
          accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.fontPixelZoom);
        }
      }
      return (int) ((double) vector2.Y + 16.0 * (double) SpriteText.fontPixelZoom);
    }

    public static Color getColorFromIndex(int index)
    {
      switch (index)
      {
        case -1:
          return LocalizedContentManager.CurrentLanguageLatin ? Color.White : new Color(86, 22, 12);
        case 1:
          return Color.SkyBlue;
        case 2:
          return Color.Red;
        case 3:
          return new Color(110, 43, (int) byte.MaxValue);
        case 4:
          return Color.White;
        case 5:
          return Color.OrangeRed;
        case 6:
          return Color.LimeGreen;
        case 7:
          return Color.Cyan;
        case 8:
          return new Color(60, 60, 60);
        default:
          return Color.Black;
      }
    }

    public static string getSubstringBeyondHeight(string s, int width, int height)
    {
      Vector2 vector2 = new Vector2();
      int accumulatedHorizontalSpaceBetweenCharacters = 0;
      s = s.Replace(Environment.NewLine, "");
      SpriteText.setUpCharacterMap();
      if (!LocalizedContentManager.CurrentLanguageLatin)
      {
        for (int index = 0; index < s.Length; ++index)
        {
          if (s[index] == '^')
          {
            vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
            vector2.X = 0.0f;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
          }
          else
          {
            FontChar fontChar;
            if (SpriteText._characterMap.TryGetValue(s[index], out fontChar))
            {
              if (index > 0)
                vector2.X += (float) fontChar.XAdvance * SpriteText.fontPixelZoom;
              if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
              {
                vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
                accumulatedHorizontalSpaceBetweenCharacters = 0;
                vector2.X = 0.0f;
              }
            }
            if ((double) vector2.Y >= (double) height - (double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.fontPixelZoom * 2.0)
              return s.Substring(SpriteText.getLastSpace(s, index));
          }
        }
        return "";
      }
      for (int index = 0; index < s.Length; ++index)
      {
        if (s[index] == '^')
        {
          vector2.Y += 18f * SpriteText.fontPixelZoom;
          vector2.X = 0.0f;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
        }
        else
        {
          if (index > 0)
            vector2.X += (float) (8.0 * (double) SpriteText.fontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) (SpriteText.getWidthOffsetForChar(s[index]) + SpriteText.getWidthOffsetForChar(s[index - 1])) * (double) SpriteText.fontPixelZoom);
          accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.fontPixelZoom);
          if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
          {
            vector2.Y += 18f * SpriteText.fontPixelZoom;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
            vector2.X = 0.0f;
          }
          if ((double) vector2.Y >= (double) height - 16.0 * (double) SpriteText.fontPixelZoom * 2.0)
            return s.Substring(SpriteText.getLastSpace(s, index));
        }
      }
      return "";
    }

    public static int getIndexOfSubstringBeyondHeight(string s, int width, int height)
    {
      Vector2 vector2 = new Vector2();
      int accumulatedHorizontalSpaceBetweenCharacters = 0;
      s = s.Replace(Environment.NewLine, "");
      SpriteText.setUpCharacterMap();
      if (!LocalizedContentManager.CurrentLanguageLatin)
      {
        for (int index = 0; index < s.Length; ++index)
        {
          if (s[index] == '^')
          {
            vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
            vector2.X = 0.0f;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
          }
          else
          {
            FontChar fontChar;
            if (SpriteText._characterMap.TryGetValue(s[index], out fontChar))
            {
              if (index > 0)
                vector2.X += (float) fontChar.XAdvance * SpriteText.fontPixelZoom;
              if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
              {
                vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
                accumulatedHorizontalSpaceBetweenCharacters = 0;
                vector2.X = 0.0f;
              }
            }
            if ((double) vector2.Y >= (double) height - (double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.fontPixelZoom * 2.0)
              return index - 1;
          }
        }
        return s.Length - 1;
      }
      for (int index = 0; index < s.Length; ++index)
      {
        if (s[index] == '^')
        {
          vector2.Y += 18f * SpriteText.fontPixelZoom;
          vector2.X = 0.0f;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
        }
        else
        {
          if (index > 0)
            vector2.X += (float) (8.0 * (double) SpriteText.fontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) (SpriteText.getWidthOffsetForChar(s[index]) + SpriteText.getWidthOffsetForChar(s[index - 1])) * (double) SpriteText.fontPixelZoom);
          accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.fontPixelZoom);
          if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
          {
            vector2.Y += 18f * SpriteText.fontPixelZoom;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
            vector2.X = 0.0f;
          }
          if ((double) vector2.Y >= (double) height - 16.0 * (double) SpriteText.fontPixelZoom)
            return index - 1;
        }
      }
      return s.Length - 1;
    }

    public static List<string> getStringBrokenIntoSectionsOfHeight(
      string s,
      int width,
      int height)
    {
      List<string> source = new List<string>();
      for (; s.Length > 0; s = s.Substring(source.Last<string>().Length))
      {
        string thisHeightCutoff = SpriteText.getStringPreviousToThisHeightCutoff(s, width, height);
        if (thisHeightCutoff.Length > 0)
          source.Add(thisHeightCutoff);
        else
          break;
      }
      return source;
    }

    public static string getStringPreviousToThisHeightCutoff(string s, int width, int height) => s.Substring(0, SpriteText.getIndexOfSubstringBeyondHeight(s, width, height) + 1);

    private static int getLastSpace(string s, int startIndex)
    {
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.th)
        return startIndex;
      for (int index = startIndex; index >= 0; --index)
      {
        if (s[index] == ' ')
          return index;
      }
      return startIndex;
    }

    public static int getWidthOffsetForChar(char c)
    {
      switch (c)
      {
        case '!':
        case 'j':
        case 'l':
        case '¡':
          return -1;
        case '$':
          return 1;
        case ',':
        case '.':
          return -2;
        case '^':
          return -8;
        case 'i':
        case 'ì':
        case 'í':
        case 'î':
        case 'ï':
        case 'ı':
          return -1;
        default:
          return 0;
      }
    }

    public static void drawStringWithScrollCenteredAt(
      SpriteBatch b,
      string s,
      int x,
      int y,
      int width,
      float alpha = 1f,
      int color = -1,
      int scrollType = 0,
      float layerDepth = 0.88f,
      bool junimoText = false)
    {
      SpriteText.drawString(b, s, x - width / 2, y, width: width, alpha: alpha, layerDepth: layerDepth, junimoText: junimoText, drawBGScroll: scrollType, color: color, scroll_text_alignment: SpriteText.ScrollTextAlignment.Center);
    }

    public static void drawStringWithScrollCenteredAt(
      SpriteBatch b,
      string s,
      int x,
      int y,
      string placeHolderWidthText = "",
      float alpha = 1f,
      int color = -1,
      int scrollType = 0,
      float layerDepth = 0.88f,
      bool junimoText = false)
    {
      SpriteText.drawString(b, s, x - SpriteText.getWidthOfString(placeHolderWidthText.Length > 0 ? placeHolderWidthText : s) / 2, y, alpha: alpha, layerDepth: layerDepth, junimoText: junimoText, drawBGScroll: scrollType, placeHolderScrollWidthText: placeHolderWidthText, color: color, scroll_text_alignment: SpriteText.ScrollTextAlignment.Center);
    }

    public static void drawStringWithScrollBackground(
      SpriteBatch b,
      string s,
      int x,
      int y,
      string placeHolderWidthText = "",
      float alpha = 1f,
      int color = -1,
      SpriteText.ScrollTextAlignment scroll_text_alignment = SpriteText.ScrollTextAlignment.Left)
    {
      SpriteText.drawString(b, s, x, y, alpha: alpha, drawBGScroll: 0, placeHolderScrollWidthText: placeHolderWidthText, color: color, scroll_text_alignment: scroll_text_alignment);
    }

    private static FontFile loadFont(string assetName) => FontLoader.Parse(Game1.content.Load<XmlSource>(assetName).Source);

    private static void setUpCharacterMap()
    {
      if (!LocalizedContentManager.CurrentLanguageLatin && SpriteText._characterMap == null)
      {
        SpriteText._characterMap = new Dictionary<char, FontChar>();
        SpriteText.fontPages = new List<Texture2D>();
        switch (LocalizedContentManager.CurrentLanguageCode)
        {
          case LocalizedContentManager.LanguageCode.ja:
            SpriteText.FontFile = SpriteText.loadFont("Fonts\\Japanese");
            SpriteText.fontPixelZoom = 1.75f;
            break;
          case LocalizedContentManager.LanguageCode.ru:
            SpriteText.FontFile = SpriteText.loadFont("Fonts\\Russian");
            SpriteText.fontPixelZoom = 3f;
            break;
          case LocalizedContentManager.LanguageCode.zh:
            SpriteText.FontFile = SpriteText.loadFont("Fonts\\Chinese");
            SpriteText.fontPixelZoom = 1.5f;
            break;
          case LocalizedContentManager.LanguageCode.th:
            SpriteText.FontFile = SpriteText.loadFont("Fonts\\Thai");
            SpriteText.fontPixelZoom = 1.5f;
            break;
          case LocalizedContentManager.LanguageCode.ko:
            SpriteText.FontFile = SpriteText.loadFont("Fonts\\Korean");
            SpriteText.fontPixelZoom = 1.5f;
            break;
          case LocalizedContentManager.LanguageCode.mod:
            SpriteText.FontFile = SpriteText.loadFont(LocalizedContentManager.CurrentModLanguage.FontFile);
            SpriteText.fontPixelZoom = LocalizedContentManager.CurrentModLanguage.FontPixelZoom;
            break;
        }
        foreach (FontChar fontChar in SpriteText.FontFile.Chars)
        {
          char id = (char) fontChar.ID;
          SpriteText._characterMap.Add(id, fontChar);
        }
        foreach (FontPage page in SpriteText.FontFile.Pages)
          SpriteText.fontPages.Add(Game1.content.Load<Texture2D>("Fonts\\" + page.File));
        LocalizedContentManager.OnLanguageChange += new LocalizedContentManager.LanguageChangedHandler(SpriteText.OnLanguageChange);
      }
      else
      {
        if (!LocalizedContentManager.CurrentLanguageLatin || (double) SpriteText.fontPixelZoom >= 3.0)
          return;
        SpriteText.fontPixelZoom = 3f;
      }
    }

    public static void drawString(
      SpriteBatch b,
      string s,
      int x,
      int y,
      int characterPosition = 999999,
      int width = -1,
      int height = 999999,
      float alpha = 1f,
      float layerDepth = 0.88f,
      bool junimoText = false,
      int drawBGScroll = -1,
      string placeHolderScrollWidthText = "",
      int color = -1,
      SpriteText.ScrollTextAlignment scroll_text_alignment = SpriteText.ScrollTextAlignment.Left)
    {
      SpriteText.setUpCharacterMap();
      bool flag1 = true;
      if (width == -1)
      {
        flag1 = false;
        width = Game1.graphics.GraphicsDevice.Viewport.Width - x;
        if (drawBGScroll == 1)
          width = SpriteText.getWidthOfString(s) * 2;
      }
      if ((double) SpriteText.fontPixelZoom < 4.0 && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.ko)
        y += (int) ((4.0 - (double) SpriteText.fontPixelZoom) * 4.0);
      Vector2 vector2_1 = new Vector2((float) x, (float) y);
      int accumulatedHorizontalSpaceBetweenCharacters = 0;
      if (drawBGScroll != 1)
      {
        if ((double) vector2_1.X + (double) width > (double) (Game1.graphics.GraphicsDevice.Viewport.Width - 4))
          vector2_1.X = (float) (Game1.graphics.GraphicsDevice.Viewport.Width - width - 4);
        if ((double) vector2_1.X < 0.0)
          vector2_1.X = 0.0f;
      }
      if (drawBGScroll == 0 || drawBGScroll == 2)
      {
        int x1 = SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s);
        if (flag1)
          x1 = width;
        switch (drawBGScroll)
        {
          case 0:
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-12f, -3f) * 4f, new Rectangle?(new Rectangle(325, 318, 12, 18)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(337, 318, 1, 18)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float) x1, 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float) x1, -12f), new Rectangle?(new Rectangle(338, 318, 12, 18)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            break;
          case 2:
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-3f, -3f) * 4f, new Rectangle?(new Rectangle(327, 281, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(330, 281, 1, 17)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float) (x1 + 4), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float) (x1 + 4), -12f), new Rectangle?(new Rectangle(333, 281, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            break;
        }
        switch (scroll_text_alignment)
        {
          case SpriteText.ScrollTextAlignment.Center:
            x += (x1 - SpriteText.getWidthOfString(s)) / 2;
            vector2_1.X = (float) x;
            break;
          case SpriteText.ScrollTextAlignment.Right:
            x += x1 - SpriteText.getWidthOfString(s);
            vector2_1.X = (float) x;
            break;
        }
        vector2_1.Y += (float) ((4.0 - (double) SpriteText.fontPixelZoom) * 4.0);
      }
      else if (drawBGScroll == 1)
      {
        int widthOfString = SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s);
        Vector2 vector2_2 = vector2_1;
        if (Game1.currentLocation != null && Game1.currentLocation.map != null && Game1.currentLocation.map.Layers[0] != null)
        {
          int num1 = -Game1.viewport.X + 28;
          int num2 = -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 28;
          if ((double) vector2_1.X < (double) num1)
            vector2_1.X = (float) num1;
          if ((double) vector2_1.X + (double) widthOfString > (double) num2)
            vector2_1.X = (float) (num2 - widthOfString);
          vector2_2.X += (float) (widthOfString / 2);
          if ((double) vector2_2.X < (double) vector2_1.X)
            vector2_1.X += vector2_2.X - vector2_1.X;
          if ((double) vector2_2.X > (double) vector2_1.X + (double) widthOfString - 24.0)
            vector2_1.X += vector2_2.X - (float) ((double) vector2_1.X + (double) widthOfString - 24.0);
          vector2_2.X = Utility.Clamp(vector2_2.X, vector2_1.X, (float) ((double) vector2_1.X + (double) widthOfString - 24.0));
        }
        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-7f, -3f) * 4f, new Rectangle?(new Rectangle(324, 299, 7, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(331, 299, 1, 17)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float) SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float) widthOfString, -12f), new Rectangle?(new Rectangle(332, 299, 7, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
        b.Draw(Game1.mouseCursors, vector2_2 + new Vector2(0.0f, 52f), new Rectangle?(new Rectangle(341, 308, 6, 5)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.0001f);
        x = (int) vector2_1.X;
        if (placeHolderScrollWidthText.Length > 0)
        {
          x += SpriteText.getWidthOfString(placeHolderScrollWidthText) / 2 - SpriteText.getWidthOfString(s) / 2;
          vector2_1.X = (float) x;
        }
        vector2_1.Y += (float) ((4.0 - (double) SpriteText.fontPixelZoom) * 4.0);
      }
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
        vector2_1.Y -= 8f;
      s = s.Replace(Environment.NewLine, "");
      if (!junimoText)
      {
        switch (LocalizedContentManager.CurrentLanguageCode)
        {
          case LocalizedContentManager.LanguageCode.ja:
          case LocalizedContentManager.LanguageCode.zh:
          case LocalizedContentManager.LanguageCode.th:
            vector2_1.Y -= (float) ((4.0 - (double) SpriteText.fontPixelZoom) * 4.0);
            break;
          case LocalizedContentManager.LanguageCode.mod:
            if (!LocalizedContentManager.CurrentModLanguage.FontApplyYOffset)
              break;
            goto case LocalizedContentManager.LanguageCode.ja;
        }
      }
      s = s.Replace('♡', '<');
      for (int index = 0; index < Math.Min(s.Length, characterPosition); ++index)
      {
        if (((LocalizedContentManager.CurrentLanguageLatin ? 1 : (SpriteText.IsSpecialCharacter(s[index]) ? 1 : 0)) | (junimoText ? 1 : 0)) != 0 || SpriteText.forceEnglishFont)
        {
          float fontPixelZoom = SpriteText.fontPixelZoom;
          if (SpriteText.IsSpecialCharacter(s[index]) | junimoText || SpriteText.forceEnglishFont)
            SpriteText.fontPixelZoom = 3f;
          if (s[index] == '^')
          {
            vector2_1.Y += 18f * SpriteText.fontPixelZoom;
            vector2_1.X = (float) x;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
            SpriteText.fontPixelZoom = fontPixelZoom;
          }
          else
          {
            accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.fontPixelZoom);
            bool flag2 = char.IsUpper(s[index]) || s[index] == 'ß';
            Vector2 vector2_3 = new Vector2(0.0f, (float) ((!junimoText & flag2 ? -3 : 0) - 1));
            if (s[index] == 'Ç')
              vector2_3.Y += 2f;
            if (SpriteText.positionOfNextSpace(s, index, (int) vector2_1.X - x, accumulatedHorizontalSpaceBetweenCharacters) >= width)
            {
              vector2_1.Y += 18f * SpriteText.fontPixelZoom;
              accumulatedHorizontalSpaceBetweenCharacters = 0;
              vector2_1.X = (float) x;
              if (s[index] == ' ')
              {
                SpriteText.fontPixelZoom = fontPixelZoom;
                continue;
              }
            }
            b.Draw(color != -1 ? SpriteText.coloredTexture : SpriteText.spriteTexture, vector2_1 + vector2_3 * SpriteText.fontPixelZoom, new Rectangle?(SpriteText.getSourceRectForChar(s[index], junimoText)), (SpriteText.IsSpecialCharacter(s[index]) | junimoText ? Color.White : SpriteText.getColorFromIndex(color)) * alpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
            if (index < s.Length - 1)
              vector2_1.X += (float) (8.0 * (double) SpriteText.fontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) SpriteText.getWidthOffsetForChar(s[index + 1]) * (double) SpriteText.fontPixelZoom);
            if (s[index] != '^')
              vector2_1.X += (float) SpriteText.getWidthOffsetForChar(s[index]) * SpriteText.fontPixelZoom;
            SpriteText.fontPixelZoom = fontPixelZoom;
          }
        }
        else if (s[index] == '^')
        {
          vector2_1.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
          vector2_1.X = (float) x;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
        }
        else
        {
          if (index > 0 && SpriteText.IsSpecialCharacter(s[index - 1]))
            vector2_1.X += 24f;
          FontChar fontChar;
          if (SpriteText._characterMap.TryGetValue(s[index], out fontChar))
          {
            Rectangle rectangle = new Rectangle(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height);
            Texture2D fontPage = SpriteText.fontPages[fontChar.Page];
            if (SpriteText.positionOfNextSpace(s, index, (int) vector2_1.X, accumulatedHorizontalSpaceBetweenCharacters) >= x + width - 4)
            {
              vector2_1.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
              accumulatedHorizontalSpaceBetweenCharacters = 0;
              vector2_1.X = (float) x;
            }
            Vector2 position = new Vector2(vector2_1.X + (float) fontChar.XOffset * SpriteText.fontPixelZoom, vector2_1.Y + (float) fontChar.YOffset * SpriteText.fontPixelZoom);
            if (drawBGScroll != -1 && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
              position.Y -= 8f;
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru)
            {
              Vector2 vector2_4 = new Vector2(-1f, 1f) * SpriteText.fontPixelZoom;
              b.Draw(fontPage, position + vector2_4, new Rectangle?(rectangle), SpriteText.getColorFromIndex(color) * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
              b.Draw(fontPage, position + new Vector2(0.0f, vector2_4.Y), new Rectangle?(rectangle), SpriteText.getColorFromIndex(color) * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
              b.Draw(fontPage, position + new Vector2(vector2_4.X, 0.0f), new Rectangle?(rectangle), SpriteText.getColorFromIndex(color) * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
            }
            b.Draw(fontPage, position, new Rectangle?(rectangle), SpriteText.getColorFromIndex(color) * alpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
            vector2_1.X += (float) fontChar.XAdvance * SpriteText.fontPixelZoom;
          }
        }
      }
    }

    private static bool IsSpecialCharacter(char c) => c.Equals('<') || c.Equals('=') || c.Equals('>') || c.Equals('@') || c.Equals('$') || c.Equals('`') || c.Equals('+');

    private static void OnLanguageChange(LocalizedContentManager.LanguageCode code)
    {
      if (SpriteText._characterMap != null)
        SpriteText._characterMap.Clear();
      else
        SpriteText._characterMap = new Dictionary<char, FontChar>();
      if (SpriteText.fontPages != null)
        SpriteText.fontPages.Clear();
      else
        SpriteText.fontPages = new List<Texture2D>();
      switch (code)
      {
        case LocalizedContentManager.LanguageCode.ja:
          SpriteText.FontFile = SpriteText.loadFont("Fonts\\Japanese");
          SpriteText.fontPixelZoom = 1.75f;
          break;
        case LocalizedContentManager.LanguageCode.ru:
          SpriteText.FontFile = SpriteText.loadFont("Fonts\\Russian");
          SpriteText.fontPixelZoom = 3f;
          break;
        case LocalizedContentManager.LanguageCode.zh:
          SpriteText.FontFile = SpriteText.loadFont("Fonts\\Chinese");
          SpriteText.fontPixelZoom = 1.5f;
          break;
        case LocalizedContentManager.LanguageCode.th:
          SpriteText.FontFile = SpriteText.loadFont("Fonts\\Thai");
          SpriteText.fontPixelZoom = 1.5f;
          break;
        case LocalizedContentManager.LanguageCode.ko:
          SpriteText.FontFile = SpriteText.loadFont("Fonts\\Korean");
          SpriteText.fontPixelZoom = 1.5f;
          break;
      }
      foreach (FontChar fontChar in SpriteText.FontFile.Chars)
      {
        char id = (char) fontChar.ID;
        SpriteText._characterMap.Add(id, fontChar);
      }
      foreach (FontPage page in SpriteText.FontFile.Pages)
        SpriteText.fontPages.Add(Game1.content.Load<Texture2D>("Fonts\\" + page.File));
    }

    public static int positionOfNextSpace(
      string s,
      int index,
      int currentXPosition,
      int accumulatedHorizontalSpaceBetweenCharacters)
    {
      SpriteText.setUpCharacterMap();
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ja:
          FontChar fontChar1;
          return SpriteText._characterMap.TryGetValue(s[index], out fontChar1) ? currentXPosition + (int) ((double) fontChar1.XAdvance * (double) SpriteText.fontPixelZoom) : currentXPosition + (int) ((double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.fontPixelZoom);
        case LocalizedContentManager.LanguageCode.zh:
        case LocalizedContentManager.LanguageCode.th:
          FontChar fontChar2;
          return SpriteText._characterMap.TryGetValue(s[index], out fontChar2) ? currentXPosition + (int) ((double) fontChar2.XAdvance * (double) SpriteText.fontPixelZoom) : currentXPosition + (int) ((double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.fontPixelZoom);
        default:
          for (int index1 = index; index1 < s.Length; ++index1)
          {
            if (!LocalizedContentManager.CurrentLanguageLatin)
            {
              if (s[index1] == ' ' || s[index1] == '^')
                return currentXPosition;
              FontChar fontChar3;
              if (SpriteText._characterMap.TryGetValue(s[index1], out fontChar3))
                currentXPosition += (int) ((double) fontChar3.XAdvance * (double) SpriteText.fontPixelZoom);
              else
                currentXPosition += (int) ((double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.fontPixelZoom);
            }
            else
            {
              if (s[index1] == ' ' || s[index1] == '^')
                return currentXPosition;
              currentXPosition += (int) (8.0 * (double) SpriteText.fontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) (SpriteText.getWidthOffsetForChar(s[index1]) + SpriteText.getWidthOffsetForChar(s[Math.Max(0, index1 - 1)])) * (double) SpriteText.fontPixelZoom);
              accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.fontPixelZoom);
            }
          }
          return currentXPosition;
      }
    }

    private static Rectangle getSourceRectForChar(char c, bool junimoText)
    {
      int num = (int) c - 32;
      switch (c)
      {
        case 'Ğ':
          num = 102;
          break;
        case 'ğ':
          num = 103;
          break;
        case 'İ':
          num = 98;
          break;
        case 'ı':
          num = 99;
          break;
        case 'Ő':
          num = 105;
          break;
        case 'ő':
          num = 106;
          break;
        case 'Œ':
          num = 96;
          break;
        case 'œ':
          num = 97;
          break;
        case 'Ş':
          num = 100;
          break;
        case 'ş':
          num = 101;
          break;
        case 'Ű':
          num = 107;
          break;
        case 'ű':
          num = 108;
          break;
        case '’':
          num = 104;
          break;
      }
      return new Rectangle(num * 8 % SpriteText.spriteTexture.Width, num * 8 / SpriteText.spriteTexture.Width * 16 + (junimoText ? 224 : 0), 8, 16);
    }

    public enum ScrollTextAlignment
    {
      Left,
      Center,
      Right,
    }
  }
}
