// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SpecialCurrencyDisplay
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class SpecialCurrencyDisplay
  {
    protected MoneyDial _moneyDial;
    protected float currentPosition;
    protected SpecialCurrencyDisplay.CurrencyDisplayType _currentDisplayedCurrency;
    public Dictionary<string, SpecialCurrencyDisplay.CurrencyDisplayType> registeredCurrencyDisplays;
    public float timeToLive;
    public Action<SpriteBatch, Vector2> drawSprite;
    public SpecialCurrencyDisplay.CurrencyDisplayType forcedOnscreenCurrencyType;

    public SpecialCurrencyDisplay()
    {
      this._moneyDial = new MoneyDial(3);
      this._moneyDial.onPlaySound = (Action<int>) null;
      this.drawSprite = (Action<SpriteBatch, Vector2>) null;
      this.registeredCurrencyDisplays = new Dictionary<string, SpecialCurrencyDisplay.CurrencyDisplayType>();
    }

    public virtual void Register(
      string key,
      NetIntDelta net_int_delta,
      Action<int> sound_function = null,
      Action<SpriteBatch, Vector2> draw_function = null)
    {
      if (this.registeredCurrencyDisplays.ContainsKey(key))
        this.Unregister(key);
      this.registeredCurrencyDisplays[key] = new SpecialCurrencyDisplay.CurrencyDisplayType()
      {
        key = key,
        netIntDelta = net_int_delta,
        playSound = sound_function,
        drawSprite = draw_function
      };
      this.registeredCurrencyDisplays[key].netIntDelta.fieldChangeVisibleEvent += new NetFieldBase<int, NetIntDelta>.FieldChange(this.OnCurrencyChange);
    }

    public virtual void ShowCurrency(string currency_type)
    {
      if (currency_type == null || !this.registeredCurrencyDisplays.ContainsKey(currency_type))
      {
        this.forcedOnscreenCurrencyType = (SpecialCurrencyDisplay.CurrencyDisplayType) null;
      }
      else
      {
        this.forcedOnscreenCurrencyType = this.registeredCurrencyDisplays[currency_type];
        this.SetDisplayedCurrency(this.forcedOnscreenCurrencyType);
      }
    }

    public virtual void OnCurrencyChange(NetIntDelta field, int old_value, int new_value)
    {
      if (Game1.gameMode != (byte) 3)
        return;
      string key1 = (string) null;
      foreach (string key2 in this.registeredCurrencyDisplays.Keys)
      {
        if ((NetFieldBase<int, NetIntDelta>) this.registeredCurrencyDisplays[key2].netIntDelta == field)
        {
          key1 = key2;
          break;
        }
      }
      if (key1 == null)
        return;
      this.SetDisplayedCurrency(key1);
      if (this._currentDisplayedCurrency != null)
      {
        this._moneyDial.currentValue = old_value;
        if (this._moneyDial.onPlaySound != null)
          this._moneyDial.onPlaySound(new_value - old_value);
      }
      this.timeToLive = 5f;
    }

    public virtual void SetDisplayedCurrency(
      SpecialCurrencyDisplay.CurrencyDisplayType currency_type)
    {
      if (currency_type == this._currentDisplayedCurrency || this.forcedOnscreenCurrencyType != null && this.forcedOnscreenCurrencyType != currency_type)
        return;
      this._moneyDial.onPlaySound = (Action<int>) null;
      this.drawSprite = (Action<SpriteBatch, Vector2>) null;
      this._currentDisplayedCurrency = currency_type;
      if (currency_type == null)
        return;
      this._moneyDial.currentValue = this._currentDisplayedCurrency.netIntDelta.Value;
      this._moneyDial.previousTargetValue = this._moneyDial.currentValue;
      this._moneyDial.onPlaySound = currency_type.playSound == null ? new Action<int>(this.DefaultPlaySound) : currency_type.playSound;
      if (currency_type.drawSprite != null)
        this.drawSprite = currency_type.drawSprite;
      else
        this.drawSprite = new Action<SpriteBatch, Vector2>(this.DefaultDrawSprite);
    }

    public virtual void SetDisplayedCurrency(string key)
    {
      if (!this.registeredCurrencyDisplays.ContainsKey(key))
        return;
      this.SetDisplayedCurrency(this.registeredCurrencyDisplays[key]);
    }

    public virtual void Unregister(string key)
    {
      if (!this.registeredCurrencyDisplays.ContainsKey(key))
        return;
      if (this._currentDisplayedCurrency == this.registeredCurrencyDisplays[key])
        this.SetDisplayedCurrency((SpecialCurrencyDisplay.CurrencyDisplayType) null);
      this.registeredCurrencyDisplays[key].netIntDelta.fieldChangeVisibleEvent -= new NetFieldBase<int, NetIntDelta>.FieldChange(this.OnCurrencyChange);
      this.registeredCurrencyDisplays.Remove(key);
    }

    public virtual void Cleanup()
    {
      foreach (string key in new List<string>((IEnumerable<string>) this.registeredCurrencyDisplays.Keys))
        this.Unregister(key);
    }

    public virtual void DefaultDrawSprite(SpriteBatch b, Vector2 position)
    {
      if (this._currentDisplayedCurrency == null)
        return;
      if (this._currentDisplayedCurrency.key == "walnuts")
      {
        b.Draw(Game1.objectSpriteSheet, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
      }
      else
      {
        if (!(this._currentDisplayedCurrency.key == "qiGems"))
          return;
        b.Draw(Game1.objectSpriteSheet, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
      }
    }

    public virtual void DefaultPlaySound(int direction)
    {
      if (this._currentDisplayedCurrency == null)
        return;
      if (direction < 0 && this._currentDisplayedCurrency.key == "walnuts")
        Game1.playSound("goldenWalnut");
      if (direction <= 0 || !(this._currentDisplayedCurrency.key == "walnuts"))
        return;
      Game1.playSound("goldenWalnut");
    }

    public virtual void Update(GameTime time)
    {
      if ((double) this.timeToLive > 0.0)
      {
        this.timeToLive -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.timeToLive < 0.0)
          this.timeToLive = 0.0f;
      }
      if ((double) this.timeToLive > 0.0 || this.forcedOnscreenCurrencyType != null)
        this.currentPosition += (float) (time.ElapsedGameTime.TotalSeconds / 0.5);
      else
        this.currentPosition -= (float) (time.ElapsedGameTime.TotalSeconds / 0.5);
      this.currentPosition = Utility.Clamp(this.currentPosition, 0.0f, 1f);
    }

    public Vector2 GetUpperLeft() => new Vector2(16f, (float) ((int) Utility.Lerp(-26f, 0.0f, this.currentPosition) * 4));

    public virtual void Draw(SpriteBatch b)
    {
      if (this._currentDisplayedCurrency == null || (double) this.currentPosition <= 0.0)
        return;
      Vector2 upperLeft = this.GetUpperLeft();
      b.Draw(Game1.mouseCursors2, upperLeft, new Rectangle?(new Rectangle(48, 176, 52, 26)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
      int previousTargetValue = this._currentDisplayedCurrency.netIntDelta.Value;
      if ((double) this.currentPosition < 0.5)
        previousTargetValue = this._moneyDial.previousTargetValue;
      this._moneyDial.draw(b, upperLeft + new Vector2(108f, 40f), previousTargetValue);
      if (this.drawSprite == null)
        return;
      this.drawSprite(b, upperLeft + new Vector2(4f, 6f) * 4f);
    }

    public class CurrencyDisplayType
    {
      public string key;
      public NetIntDelta netIntDelta;
      public Action<int> playSound;
      public Action<SpriteBatch, Vector2> drawSprite;
    }
  }
}
