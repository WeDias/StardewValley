// Decompiled with JetBrains decompiler
// Type: StardewValley.PurchaseableKeyItem
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class PurchaseableKeyItem : ISalable
  {
    protected string _displayName = "";
    protected string _name = "";
    protected string _description = "";
    protected int _price;
    protected int _id;
    protected List<string> _tags;
    protected Action<Farmer> _onPurchase;

    public string DisplayName => this._displayName;

    public int id => this._id;

    public List<string> tags => this._tags;

    public PurchaseableKeyItem(
      string display_name,
      string display_description,
      int parent_sheet_index,
      Action<Farmer> on_purchase = null)
    {
      this._id = parent_sheet_index;
      this._name = display_name;
      this._displayName = display_name;
      this._description = display_description;
      this._onPurchase = on_purchase;
    }

    public void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((float) (int) (32.0 * (double) scaleSize), (float) (int) (32.0 * (double) scaleSize)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this._id, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
    }

    public bool ShouldDrawIcon() => true;

    public string Name => this._name;

    public string getDescription() => this._description;

    public int maximumStackSize() => 1;

    public int addToStack(Item stack) => 1;

    public bool canStackWith(ISalable other) => false;

    public int Stack
    {
      get => 1;
      set
      {
      }
    }

    public int salePrice() => this._price;

    public bool actionWhenPurchased()
    {
      if (this._onPurchase != null)
        this._onPurchase(Game1.player);
      return true;
    }

    public bool CanBuyItem(Farmer farmer) => true;

    public bool IsInfiniteStock() => false;

    public ISalable GetSalableInstance() => (ISalable) this;
  }
}
