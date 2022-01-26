// Decompiled with JetBrains decompiler
// Type: StardewValley.MovieConcession
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Movies;
using System.Collections.Generic;

namespace StardewValley
{
  public class MovieConcession : ISalable
  {
    protected string _displayName = "";
    protected string _name = "";
    protected string _description = "";
    protected int _price;
    protected int _id;
    protected List<string> _tags;

    public string DisplayName => this._displayName;

    public int id => this._id;

    public List<string> tags => this._tags;

    public MovieConcession(ConcessionItemData data)
    {
      this._id = data.ID;
      this._name = data.Name;
      this._displayName = data.DisplayName;
      this._description = data.Description;
      this._price = data.Price;
      this._tags = data.ItemTags;
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
      if (this._id != 590 & drawShadow)
      {
        SpriteBatch spriteBatch1 = spriteBatch;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = location + new Vector2(32f, 48f);
        Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
        Color color1 = color * 0.5f;
        Rectangle bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        double layerDepth1 = (double) layerDepth - 9.99999974737875E-05;
        spriteBatch1.Draw(shadowTexture, position, sourceRectangle, color1, 0.0f, origin, 3f, SpriteEffects.None, (float) layerDepth1);
      }
      spriteBatch.Draw(Game1.concessionsSpriteSheet, location + new Vector2((float) (int) (32.0 * (double) scaleSize), (float) (int) (32.0 * (double) scaleSize)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.concessionsSpriteSheet, this._id, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
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

    public bool actionWhenPurchased() => true;

    public bool CanBuyItem(Farmer farmer) => true;

    public bool IsInfiniteStock() => true;

    public ISalable GetSalableInstance() => (ISalable) this;
  }
}
