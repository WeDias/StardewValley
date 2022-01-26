// Decompiled with JetBrains decompiler
// Type: StardewValley.NameSelect
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  [InstanceStatics]
  public class NameSelect
  {
    public const int maxNameLength = 9;
    public const int charactersPerRow = 15;
    public static string name = "";
    private static int selection = 0;
    private static List<char> namingCharacters;

    public static void load()
    {
      NameSelect.namingCharacters = new List<char>();
      for (int index1 = 0; index1 < 25; index1 += 5)
      {
        for (int index2 = 0; index2 < 5; ++index2)
          NameSelect.namingCharacters.Add((char) (97 + index1 + index2));
        for (int index3 = 0; index3 < 5; ++index3)
          NameSelect.namingCharacters.Add((char) (65 + index1 + index3));
        if (index1 < 10)
        {
          for (int index4 = 0; index4 < 5; ++index4)
            NameSelect.namingCharacters.Add((char) (48 + index1 + index4));
        }
        else if (index1 < 15)
        {
          NameSelect.namingCharacters.Add('?');
          NameSelect.namingCharacters.Add('$');
          NameSelect.namingCharacters.Add('\'');
          NameSelect.namingCharacters.Add('#');
          NameSelect.namingCharacters.Add('[');
        }
        else if (index1 < 20)
        {
          NameSelect.namingCharacters.Add('-');
          NameSelect.namingCharacters.Add('=');
          NameSelect.namingCharacters.Add('~');
          NameSelect.namingCharacters.Add('&');
          NameSelect.namingCharacters.Add('!');
        }
        else
        {
          NameSelect.namingCharacters.Add('Z');
          NameSelect.namingCharacters.Add('z');
          NameSelect.namingCharacters.Add('<');
          NameSelect.namingCharacters.Add('"');
          NameSelect.namingCharacters.Add(']');
        }
      }
    }

    public static void draw()
    {
      int val1_1 = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Width - Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Width % 64;
      Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int width1 = viewport1.Width;
      viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int num1 = viewport1.Width % 64;
      int val2_1 = width1 - num1 - 128;
      int width2 = Math.Min(val1_1, val2_1);
      int val1_2 = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Height - Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Height % 64;
      Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      int height1 = viewport2.Height;
      viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      int num2 = viewport2.Height % 64;
      int val2_2 = height1 - num2 - 64;
      int height2 = Math.Min(val1_2, val2_2);
      int x1 = Game1.graphics.GraphicsDevice.Viewport.Width / 2 - width2 / 2;
      int y = Game1.graphics.GraphicsDevice.Viewport.Height / 2 - height2 / 2;
      int num3 = (width2 - 128) / 15;
      int num4 = (height2 - 256) / 5;
      Game1.drawDialogueBox(x1, y, width2, height2, false, true);
      string text1 = "";
      string nameSelectType = Game1.nameSelectType;
      if (!(nameSelectType == "samBand"))
      {
        if (nameSelectType == "Animal" || nameSelectType == "playerName" || nameSelectType == "coopDwellerBorn")
          text1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:NameSelect.cs.3860");
      }
      else
        text1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:NameSelect.cs.3856");
      Game1.spriteBatch.DrawString(Game1.dialogueFont, text1, new Vector2((float) (x1 + 128), (float) (y + 128)), Game1.textColor);
      int x2 = (int) Game1.dialogueFont.MeasureString(text1).X;
      string text2 = "";
      char namingCharacter;
      for (int index = 0; index < 9; ++index)
      {
        if (NameSelect.name.Length > index)
        {
          SpriteBatch spriteBatch = Game1.spriteBatch;
          SpriteFont dialogueFont1 = Game1.dialogueFont;
          namingCharacter = NameSelect.name[index];
          string text3 = namingCharacter.ToString() ?? "";
          double num5 = (double) (x1 + 128 + x2) + (double) Game1.dialogueFont.MeasureString(text2).X;
          double x3 = (double) Game1.dialogueFont.MeasureString("_").X;
          SpriteFont dialogueFont2 = Game1.dialogueFont;
          namingCharacter = NameSelect.name[index];
          string text4 = namingCharacter.ToString() ?? "";
          double x4 = (double) dialogueFont2.MeasureString(text4).X;
          double num6 = (x3 - x4) / 2.0;
          Vector2 position = new Vector2((float) (num5 + num6 - 2.0), (float) (y + 128 - 6));
          Color textColor = Game1.textColor;
          spriteBatch.DrawString(dialogueFont1, text3, position, textColor);
        }
        text2 += "_ ";
      }
      Game1.spriteBatch.DrawString(Game1.dialogueFont, "_ _ _ _ _ _ _ _ _", new Vector2((float) (x1 + 128 + x2), (float) (y + 128)), Game1.textColor);
      Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:NameSelect.cs.3864"), new Vector2((float) (x1 + width2 - 192), (float) (y + height2 - 96)), Game1.textColor);
      for (int index1 = 0; index1 < 5; ++index1)
      {
        int num7 = 0;
        for (int index2 = 0; index2 < 15; ++index2)
        {
          if (index2 != 0 && index2 % 5 == 0)
            num7 += num3 / 3;
          SpriteBatch spriteBatch = Game1.spriteBatch;
          SpriteFont dialogueFont = Game1.dialogueFont;
          namingCharacter = NameSelect.namingCharacters[index1 * 15 + index2];
          string text5 = namingCharacter.ToString() ?? "";
          Vector2 position = new Vector2((float) (num7 + x1 + 64 + num3 * index2), (float) (y + 192 + num4 * index1));
          Color textColor = Game1.textColor;
          spriteBatch.DrawString(dialogueFont, text5, position, textColor);
          if (NameSelect.selection == index1 * 15 + index2)
            Game1.spriteBatch.Draw(Game1.objectSpriteSheet, new Vector2((float) (num7 + x1 + num3 * index2 - 6), (float) (y + 192 + num4 * index1 - 8)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 26)), Color.White);
        }
      }
      if (NameSelect.selection != -1)
        return;
      Game1.spriteBatch.Draw(Game1.objectSpriteSheet, new Vector2((float) (x1 + width2 - 192 - 64 - 6), (float) (y + height2 - 96 - 8)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 26)), Color.White);
    }

    public static bool select()
    {
      if (NameSelect.selection == -1)
      {
        if (NameSelect.name.Length > 0)
          return true;
      }
      else if (NameSelect.name.Length < 9)
      {
        NameSelect.name += NameSelect.namingCharacters[NameSelect.selection].ToString();
        Game1.playSound("smallSelect");
      }
      return false;
    }

    public static void startButton()
    {
      if (NameSelect.name.Length <= 0)
        return;
      NameSelect.selection = -1;
      Game1.playSound("smallSelect");
    }

    public static bool isOnDone() => NameSelect.selection == -1;

    public static void backspace()
    {
      if (NameSelect.name.Length <= 0)
        return;
      NameSelect.name = NameSelect.name.Remove(NameSelect.name.Length - 1);
      Game1.playSound("toolSwap");
    }

    public static bool cancel()
    {
      if ((Game1.nameSelectType.Equals("samBand") || Game1.nameSelectType.Equals("coopDwellerBorn")) && NameSelect.name.Length > 0)
      {
        Game1.playSound("toolSwap");
        NameSelect.name = NameSelect.name.Remove(NameSelect.name.Length - 1);
        return false;
      }
      NameSelect.selection = 0;
      NameSelect.name = "";
      return true;
    }

    public static void moveSelection(int direction)
    {
      Game1.playSound("toolSwap");
      if (direction.Equals(0))
      {
        if (NameSelect.selection == -1)
          NameSelect.selection = NameSelect.namingCharacters.Count - 2;
        else if (NameSelect.selection - 15 < 0)
          NameSelect.selection = NameSelect.namingCharacters.Count - 15 + NameSelect.selection;
        else
          NameSelect.selection -= 15;
      }
      else if (direction.Equals(1))
      {
        ++NameSelect.selection;
        if (NameSelect.selection % 15 != 0)
          return;
        NameSelect.selection -= 15;
      }
      else if (direction.Equals(2))
      {
        if (NameSelect.selection >= NameSelect.namingCharacters.Count - 2)
        {
          NameSelect.selection = -1;
        }
        else
        {
          NameSelect.selection += 15;
          if (NameSelect.selection < NameSelect.namingCharacters.Count)
            return;
          NameSelect.selection -= NameSelect.namingCharacters.Count;
        }
      }
      else
      {
        if (!direction.Equals(3))
          return;
        if (NameSelect.selection % 15 == 0)
          NameSelect.selection += 14;
        else
          --NameSelect.selection;
      }
    }
  }
}
