// Decompiled with JetBrains decompiler
// Type: StardewValley.MoneyMadeScreen
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley
{
  public class MoneyMadeScreen
  {
    private const int timeToDisplayEachItem = 200;
    private Dictionary<ShippedItem, int> shippingItems = new Dictionary<ShippedItem, int>();
    public bool complete;
    public bool canProceed;
    public bool throbUp;
    public bool day;
    private int currentItemIndex;
    private int timeOnCurrentItem;
    private int total;
    private float starScale = 1f;

    public MoneyMadeScreen(List<Object> shippingItems, int timeOfDay)
    {
      if (timeOfDay < 2000)
        this.day = true;
      int randomItemFromSeason = Utility.getRandomItemFromSeason(Game1.currentSeason, 0, false);
      int num1 = Game1.cropsOfTheWeek[(Game1.dayOfMonth - 1) / 7];
      foreach (Object shippingItem in shippingItems)
      {
        ShippedItem key = new ShippedItem(shippingItem.ParentSheetIndex, shippingItem.Price, shippingItem.name);
        int num2 = shippingItem.Price * shippingItem.Stack;
        if (shippingItem.ParentSheetIndex == randomItemFromSeason)
          num2 = (int) ((double) num2 * 1.20000004768372);
        if (shippingItem.ParentSheetIndex == num1)
          num2 = (int) ((double) num2 * 1.10000002384186);
        if (shippingItem.Name.Contains("="))
          num2 += num2 / 2;
        int num3 = num2 - num2 % 5;
        if (this.shippingItems.ContainsKey(key))
          this.shippingItems[key]++;
        else
          this.shippingItems.Add(key, shippingItem.Stack);
        this.total += num3;
      }
    }

    public void update(int milliseconds, bool keyDown)
    {
      if (!this.complete)
      {
        this.timeOnCurrentItem += keyDown ? milliseconds * 2 : milliseconds;
        if (this.timeOnCurrentItem >= 200)
        {
          ++this.currentItemIndex;
          Game1.playSound("shiny4");
          this.timeOnCurrentItem = 0;
          if (this.currentItemIndex == this.shippingItems.Count)
            this.complete = true;
        }
      }
      else
      {
        this.timeOnCurrentItem += keyDown ? milliseconds * 2 : milliseconds;
        if (this.timeOnCurrentItem >= 1000)
          this.canProceed = true;
      }
      if (this.throbUp)
        this.starScale += 0.01f;
      else
        this.starScale -= 0.01f;
      if ((double) this.starScale >= 1.20000004768372)
      {
        this.throbUp = false;
      }
      else
      {
        if ((double) this.starScale > 1.0)
          return;
        this.throbUp = true;
      }
    }

    public void draw(GameTime gametime)
    {
      if (this.day)
        Game1.graphics.GraphicsDevice.Clear(Utility.getSkyColorForSeason(Game1.currentSeason));
      Game1.drawTitleScreenBackground(gametime, this.day ? "_day" : "_night", Utility.weatherDebrisOffsetForSeason(Game1.currentSeason));
      int height = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Height - 128;
      int x = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().X + Game1.graphics.GraphicsDevice.Viewport.Width / 2 - (int) ((double) ((this.shippingItems.Count / (height / 64 - 4) + 1) * 64) * 3.0);
      int y = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Y + 64;
      int width = (int) ((double) ((this.shippingItems.Count / (height / 64 - 4) + 1) * 64) * 6.0);
      Game1.drawDialogueBox(x, y, width, height, false, true);
      int num = height - 192;
      Point point = new Point(x + 64, y + 64);
      for (int index = 0; index < this.currentItemIndex; ++index)
      {
        Game1.spriteBatch.Draw(Game1.objectSpriteSheet, new Vector2((float) (point.X + index * 64 / (num - 128) * 64 * 4 + 32), (float) (index * 64 % (num - 128) - index * 64 % (num - 128) % 64 + Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Y + 192 + 32)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.shippingItems.Keys.ElementAt<ShippedItem>(index).index)), Color.White, 0.0f, new Vector2(32f, 32f), this.shippingItems.Keys.ElementAt<ShippedItem>(index).name.Contains("=") ? this.starScale : 1f, SpriteEffects.None, 0.999999f);
        Game1.spriteBatch.DrawString(Game1.dialogueFont, "x" + this.shippingItems[this.shippingItems.Keys.ElementAt<ShippedItem>(index)].ToString() + " : " + (this.shippingItems.Keys.ElementAt<ShippedItem>(index).price * this.shippingItems[this.shippingItems.Keys.ElementAt<ShippedItem>(index)]).ToString() + "g", new Vector2((float) (point.X + index * 64 / (num - 128) * 64 * 4 + 64), (float) ((double) (index * 64 % (num - 128) - index * 64 % (num - 128) % 64 + 32) - (double) Game1.dialogueFont.MeasureString("9").Y / 2.0 + (double) Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Y + 192.0)), Game1.textColor);
      }
      if (!this.complete)
        return;
      Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:MoneyMadeScreen.cs.3854", (object) this.total), new Vector2((float) (x + width - 64) - Game1.dialogueFont.MeasureString("Total: " + this.total.ToString()).X, (float) (Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 160)), Game1.textColor);
    }
  }
}
