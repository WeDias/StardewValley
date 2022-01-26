// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.BuffsDisplay
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class BuffsDisplay : IClickableMenu
  {
    public const int fullnessLength = 180000;
    public const int quenchedLength = 60000;
    private Dictionary<ClickableTextureComponent, Buff> buffs = new Dictionary<ClickableTextureComponent, Buff>();
    public Buff food;
    public Buff drink;
    public List<Buff> otherBuffs = new List<Buff>();
    public int fullnessLeft;
    public int quenchedLeft;
    public string hoverText = "";

    public BuffsDisplay() => this.updatePosition();

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    private void updatePosition()
    {
      Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
      int num1 = 288;
      int num2 = 64;
      int num3 = titleSafeArea.Right - 320 - 32 - this.width;
      int num4 = titleSafeArea.Top + 8;
      this.xPositionOnScreen = num3;
      this.yPositionOnScreen = num4;
      this.width = num1;
      this.height = num2;
      this.syncIcons();
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
      {
        if (buff.Key.containsPoint(x, y))
        {
          this.hoverText = buff.Key.hoverText + Environment.NewLine + buff.Value.getTimeLeft();
          buff.Key.scale = Math.Min(buff.Key.baseScale + 0.1f, buff.Key.scale + 0.02f);
          break;
        }
      }
    }

    public void arrangeTheseComponentsInThisRectangle(
      int rectangleX,
      int rectangleY,
      int rectangleWidthInComponentWidthUnits,
      int componentWidth,
      int componentHeight,
      int buffer,
      bool rightToLeft)
    {
      int num1 = 0;
      int num2 = 0;
      foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
      {
        ClickableTextureComponent key = buff.Key;
        if (rightToLeft)
          key.bounds = new Rectangle(rectangleX + rectangleWidthInComponentWidthUnits * componentWidth - (num1 + 1) * (componentWidth + buffer), rectangleY + num2 * (componentHeight + buffer), componentWidth, componentHeight);
        else
          key.bounds = new Rectangle(rectangleX + num1 * (componentWidth + buffer), rectangleY + num2 * (componentHeight + buffer), componentWidth, componentHeight);
        ++num1;
        if (num1 > rectangleWidthInComponentWidthUnits)
        {
          ++num2;
          num1 %= rectangleWidthInComponentWidthUnits;
        }
      }
    }

    public void syncIcons()
    {
      this.buffs.Clear();
      if (this.food != null)
      {
        foreach (ClickableTextureComponent clickableComponent in this.food.getClickableComponents())
          this.buffs.Add(clickableComponent, this.food);
      }
      if (this.drink != null)
      {
        foreach (ClickableTextureComponent clickableComponent in this.drink.getClickableComponents())
          this.buffs.Add(clickableComponent, this.drink);
      }
      foreach (Buff otherBuff in this.otherBuffs)
      {
        foreach (ClickableTextureComponent clickableComponent in otherBuff.getClickableComponents())
          this.buffs.Add(clickableComponent, otherBuff);
      }
      this.arrangeTheseComponentsInThisRectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width / 64, 64, 64, 8, true);
    }

    public bool hasBuff(int which) => Game1.player.hasBuff(which);

    public bool tryToAddFoodBuff(Buff b, int duration)
    {
      if (b.source.Equals("Squid Ink Ravioli"))
        this.addOtherBuff(new Buff(28));
      if (b.total <= 0 || this.fullnessLeft > 0)
        return false;
      if (this.food != null)
        this.food.removeBuff();
      this.food = b;
      this.food.addBuff();
      this.syncIcons();
      return true;
    }

    public bool tryToAddDrinkBuff(Buff b)
    {
      if (b.source.Contains("Beer") || b.source.Contains("Wine") || b.source.Contains("Mead") || b.source.Contains("Pale Ale"))
        this.addOtherBuff(new Buff(17));
      else if (b.source.Equals("Oil of Garlic"))
        this.addOtherBuff(new Buff(23));
      else if (b.source.Equals("Life Elixir"))
        Game1.player.health = Game1.player.maxHealth;
      else if (b.source.Equals("Muscle Remedy"))
        Game1.player.exhausted.Value = false;
      if (b.total <= 0 || this.quenchedLeft > 0)
        return false;
      if (this.drink != null)
        this.drink.removeBuff();
      this.drink = b;
      this.drink.addBuff();
      this.syncIcons();
      return true;
    }

    public bool removeOtherBuff(int which)
    {
      bool flag = false;
      for (int index = 0; index < this.otherBuffs.Count; ++index)
      {
        Buff otherBuff = this.otherBuffs[index];
        if (which == otherBuff.which)
        {
          otherBuff.removeBuff();
          this.otherBuffs.RemoveAt(index);
          flag = true;
        }
      }
      if (flag)
        this.syncIcons();
      return flag;
    }

    public bool addOtherBuff(Buff buff)
    {
      if (buff.which != -1)
      {
        foreach (KeyValuePair<ClickableTextureComponent, Buff> buff1 in this.buffs)
        {
          if (buff.which == buff1.Value.which)
          {
            buff1.Value.millisecondsDuration = buff.millisecondsDuration;
            buff1.Key.scale = buff1.Key.baseScale + 0.2f;
            return false;
          }
        }
      }
      this.otherBuffs.Add(buff);
      buff.addBuff();
      this.syncIcons();
      return true;
    }

    public new void update(GameTime time)
    {
      if (!Game1.wasMouseVisibleThisFrame)
        this.hoverText = "";
      if (this.food != null && this.food.update(time))
      {
        this.food.removeBuff();
        this.food = (Buff) null;
        this.syncIcons();
      }
      if (this.drink != null && this.drink.update(time))
      {
        this.drink.removeBuff();
        this.drink = (Buff) null;
        this.syncIcons();
      }
      for (int index = this.otherBuffs.Count - 1; index >= 0; --index)
      {
        if (this.otherBuffs[index].update(time))
        {
          this.otherBuffs[index].removeBuff();
          this.otherBuffs.RemoveAt(index);
          this.syncIcons();
        }
      }
      foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
      {
        ClickableTextureComponent key = buff.Key;
        key.scale = Math.Max(key.baseScale, key.scale - 0.01f);
        if (!buff.Value.alreadyUpdatedIconAlpha && (double) buff.Value.millisecondsDuration < (double) Math.Min(10000f, (float) buff.Value.totalMillisecondsDuration / 10f))
        {
          buff.Value.displayAlphaTimer += (float) (Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / ((double) buff.Value.millisecondsDuration < (double) Math.Min(2000f, (float) buff.Value.totalMillisecondsDuration / 20f) ? 1.0 : 2.0));
          buff.Value.alreadyUpdatedIconAlpha = true;
        }
      }
    }

    public void clearAllBuffs()
    {
      this.otherBuffs.Clear();
      if (this.food != null)
      {
        this.food.removeBuff();
        this.food = (Buff) null;
      }
      if (this.drink != null)
      {
        this.drink.removeBuff();
        this.drink = (Buff) null;
      }
      this.buffs.Clear();
    }

    public override void draw(SpriteBatch b)
    {
      this.updatePosition();
      foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
      {
        buff.Key.draw(b, Color.White * ((double) buff.Value.displayAlphaTimer > 0.0 ? (float) ((Math.Cos((double) buff.Value.displayAlphaTimer / 100.0) + 3.0) / 4.0) : 1f), 0.8f);
        buff.Value.alreadyUpdatedIconAlpha = false;
      }
      if (this.hoverText.Length == 0 || !this.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
  }
}
