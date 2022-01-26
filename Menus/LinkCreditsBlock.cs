// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LinkCreditsBlock
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using System;
using System.Diagnostics;

namespace StardewValley.Menus
{
  internal class LinkCreditsBlock : ICreditsBlock
  {
    private string text;
    private string url;
    private bool currentlyHovered;

    public LinkCreditsBlock(string text, string url)
    {
      this.text = text;
      this.url = url;
    }

    public override void draw(int topLeftX, int topLeftY, int widthToOccupy, SpriteBatch b)
    {
      SpriteText.drawString(b, this.text, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: (this.currentlyHovered ? 6 : 7));
      this.currentlyHovered = false;
    }

    public override int getHeight(int maxWidth) => !(this.text == "") ? SpriteText.getHeightOfString(this.text, maxWidth) : 64;

    public override void hovered() => this.currentlyHovered = true;

    public override void clicked()
    {
      Game1.playSound("bigSelect");
      try
      {
        Process.Start(this.url);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
