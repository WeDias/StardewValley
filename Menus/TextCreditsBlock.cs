// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TextCreditsBlock
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  internal class TextCreditsBlock : ICreditsBlock
  {
    private string text;
    private int color;
    private bool renderNameInEnglish;

    public TextCreditsBlock(string rawtext)
    {
      string[] source = rawtext.Split(']');
      if (((IEnumerable<string>) source).Count<string>() > 1)
      {
        this.text = source[1];
        this.color = Convert.ToInt32(source[0].Substring(1));
      }
      else
      {
        this.text = source[0];
        this.color = 4;
      }
      if (!SpriteText.IsMissingCharacters(rawtext))
        return;
      this.renderNameInEnglish = true;
    }

    public override void draw(int topLeftX, int topLeftY, int widthToOccupy, SpriteBatch b)
    {
      if (this.renderNameInEnglish)
      {
        int num1 = this.text.IndexOf('(');
        if (num1 != -1 && num1 > 0)
        {
          string s1 = this.text.Substring(0, num1);
          string s2 = this.text.Substring(num1);
          SpriteText.forceEnglishFont = true;
          int num2 = (int) ((double) SpriteText.getWidthOfString(s1) / (double) SpriteText.fontPixelZoom * 3.0);
          SpriteText.drawString(b, s1, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: this.color);
          SpriteText.forceEnglishFont = false;
          SpriteText.drawString(b, s2, topLeftX + num2, topLeftY, height: 99999, color: this.color);
        }
        else
        {
          SpriteText.forceEnglishFont = true;
          SpriteText.drawString(b, this.text, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: this.color);
          SpriteText.forceEnglishFont = false;
        }
      }
      else
        SpriteText.drawString(b, this.text, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: this.color);
    }

    public override int getHeight(int maxWidth) => !(this.text == "") ? SpriteText.getHeightOfString(this.text, maxWidth) : 64;
  }
}
