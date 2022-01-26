// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.FishTankFurniture
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class FishTankFurniture : StorageFurniture
  {
    public const int TANK_DEPTH = 10;
    public const int FLOOR_DECORATION_OFFSET = 4;
    public const int TANK_SORT_REGION = 20;
    [XmlIgnore]
    public List<Vector4> bubbles = new List<Vector4>();
    [XmlIgnore]
    public List<TankFish> tankFish = new List<TankFish>();
    [XmlIgnore]
    public NetEvent0 refreshFishEvent = new NetEvent0();
    [XmlIgnore]
    public bool fishDirty = true;
    [XmlIgnore]
    private Texture2D _aquariumTexture;
    [XmlIgnore]
    public List<KeyValuePair<Rectangle, Vector2>?> floorDecorations = new List<KeyValuePair<Rectangle, Vector2>?>();
    [XmlIgnore]
    public List<Vector2> decorationSlots = new List<Vector2>();
    [XmlIgnore]
    public List<int> floorDecorationIndices = new List<int>();
    public NetInt generationSeed = new NetInt();
    [XmlIgnore]
    public Item localDepositedItem;
    [XmlIgnore]
    protected int _currentDecorationIndex;
    protected Dictionary<Item, TankFish> _fishLookup = new Dictionary<Item, TankFish>();

    public FishTankFurniture() => this.generationSeed.Value = Game1.random.Next();

    public FishTankFurniture(int which, Vector2 tile, int initialRotations)
      : base(which, tile, initialRotations)
    {
      this.generationSeed.Value = Game1.random.Next();
    }

    public FishTankFurniture(int which, Vector2 tile)
      : base(which, tile)
    {
      this.generationSeed.Value = Game1.random.Next();
    }

    public override void resetOnPlayerEntry(GameLocation environment, bool dropDown = false)
    {
      base.resetOnPlayerEntry(environment, dropDown);
      this.ResetFish();
      this.UpdateFish();
    }

    public virtual void ResetFish()
    {
      this.bubbles.Clear();
      this.tankFish.Clear();
      this._fishLookup.Clear();
      this.UpdateFish();
    }

    public Texture2D GetAquariumTexture()
    {
      if (this._aquariumTexture == null)
        this._aquariumTexture = Game1.content.Load<Texture2D>("LooseSprites\\AquariumFish");
      return this._aquariumTexture;
    }

    protected override void initNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.generationSeed, (INetSerializable) this.refreshFishEvent);
      this.refreshFishEvent.onEvent += new NetEvent0.Event(this.UpdateDecorAndFish);
      base.initNetFields();
    }

    public override Item getOne()
    {
      FishTankFurniture one = new FishTankFurniture((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      one.drawPosition.Value = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition;
      one.defaultBoundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.defaultBoundingBox;
      one.boundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;
      one.currentRotation.Value = (int) (NetFieldBase<int, NetInt>) this.currentRotation - 1;
      one.isOn.Value = false;
      one.rotations.Value = (int) (NetFieldBase<int, NetInt>) this.rotations;
      one.rotate();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public virtual int GetCapacityForCategory(FishTankFurniture.FishTankCategories category)
    {
      int tilesWide = this.getTilesWide();
      switch (category)
      {
        case FishTankFurniture.FishTankCategories.Swim:
          return tilesWide - 1;
        case FishTankFurniture.FishTankCategories.Ground:
          return tilesWide - 1;
        case FishTankFurniture.FishTankCategories.Decoration:
          return tilesWide <= 2 ? 1 : -1;
        default:
          return 0;
      }
    }

    public FishTankFurniture.FishTankCategories GetCategoryFromItem(Item item)
    {
      Dictionary<int, string> aquariumData = this.GetAquariumData();
      if (!this.CanBeDeposited(item))
        return FishTankFurniture.FishTankCategories.None;
      if (!aquariumData.ContainsKey(item.ParentSheetIndex))
        return FishTankFurniture.FishTankCategories.Decoration;
      string str = aquariumData[item.ParentSheetIndex].Split('/')[1];
      return str == "crawl" || str == "ground" || str == "front_crawl" || str == "static" ? FishTankFurniture.FishTankCategories.Ground : FishTankFurniture.FishTankCategories.Swim;
    }

    public bool HasRoomForThisItem(Item item)
    {
      if (!this.CanBeDeposited(item))
        return false;
      FishTankFurniture.FishTankCategories categoryFromItem = this.GetCategoryFromItem(item);
      int num1 = this.GetCapacityForCategory(categoryFromItem);
      if (item is Hat)
        num1 = 999;
      if (num1 < 0)
      {
        foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
        {
          if (heldItem != null && heldItem.ParentSheetIndex == item.ParentSheetIndex)
            return false;
        }
        return true;
      }
      int num2 = 0;
      foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
      {
        if (heldItem != null)
        {
          if (this.GetCategoryFromItem(heldItem) == categoryFromItem)
            ++num2;
          if (num2 >= num1)
            return false;
        }
      }
      return true;
    }

    public override string GetShopMenuContext() => "FishTank";

    public override void ShowMenu() => this.ShowShopMenu();

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity || this.mutex.IsLocked())
        return true;
      if ((who.ActiveObject != null || who.CurrentItem != null && who.CurrentItem is Hat) && this.localDepositedItem == null && this.CanBeDeposited(who.CurrentItem))
      {
        if (!this.HasRoomForThisItem(who.CurrentItem))
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishTank_Full"));
          return true;
        }
        GameLocation location = who.currentLocation;
        this.localDepositedItem = who.CurrentItem.getOne();
        --who.CurrentItem.Stack;
        if (who.CurrentItem.Stack <= 0 || who.CurrentItem is Hat)
        {
          who.removeItemFromInventory(who.CurrentItem);
          who.showNotCarrying();
        }
        this.mutex.RequestLock((Action) (() =>
        {
          location.playSound("dropItemInWater");
          this.heldItems.Add(this.localDepositedItem);
          this.localDepositedItem = (Item) null;
          this.refreshFishEvent.Fire();
          this.mutex.ReleaseLock();
        }), (Action) (() =>
        {
          this.localDepositedItem = who.addItemToInventory(this.localDepositedItem);
          if (this.localDepositedItem != null)
            Game1.createItemDebris(this.localDepositedItem, new Vector2((float) ((double) this.TileLocation.X + (double) this.getTilesWide() / 2.0 + 0.5), this.TileLocation.Y + 0.5f) * 64f, -1, location);
          this.localDepositedItem = (Item) null;
        }));
        return true;
      }
      this.mutex.RequestLock((Action) (() => this.ShowMenu()));
      return true;
    }

    public virtual bool CanBeDeposited(Item item)
    {
      if (item == null || !(item is Hat) && !Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex))
        return false;
      if (item.ParentSheetIndex == 152 || item.ParentSheetIndex == 393 || item.ParentSheetIndex == 390)
        return true;
      if (item is Hat)
      {
        int num1 = 0;
        int num2 = 0;
        foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
        {
          if (heldItem is Hat)
            ++num2;
          else if (heldItem is StardewValley.Object && (int) (NetFieldBase<int, NetInt>) heldItem.parentSheetIndex == 397)
            ++num1;
        }
        return num2 < num1;
      }
      return this.GetAquariumData().ContainsKey(item.ParentSheetIndex);
    }

    public override void DayUpdate(GameLocation location)
    {
      this.ResetFish();
      base.DayUpdate(location);
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (Game1.currentLocation == environment)
      {
        if (this.fishDirty)
        {
          this.fishDirty = false;
          this.UpdateDecorAndFish();
        }
        foreach (TankFish tankFish in this.tankFish)
          tankFish.Update(time);
        for (int index = 0; index < this.bubbles.Count; ++index)
        {
          Vector4 bubble = this.bubbles[index];
          bubble.W += 0.05f;
          if ((double) bubble.W > 1.0)
            bubble.W = 1f;
          bubble.Y += bubble.W;
          this.bubbles[index] = bubble;
          if ((double) bubble.Y >= (double) this.GetTankBounds().Height)
          {
            this.bubbles.RemoveAt(index);
            --index;
          }
        }
      }
      base.updateWhenCurrentLocation(time, environment);
      this.refreshFishEvent.Poll();
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      this.generationSeed.Value = Game1.random.Next();
      this.fishDirty = true;
      return base.placementAction(location, x, y, who);
    }

    public Dictionary<int, string> GetAquariumData() => Game1.content.Load<Dictionary<int, string>>("Data\\AquariumFish");

    public override bool onDresserItemWithdrawn(ISalable salable, Farmer who, int amount)
    {
      int num = base.onDresserItemWithdrawn(salable, who, amount) ? 1 : 0;
      this.refreshFishEvent.Fire();
      return num != 0;
    }

    public virtual void UpdateFish()
    {
      List<Item> objList1 = new List<Item>();
      Dictionary<int, string> aquariumData = this.GetAquariumData();
      foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
      {
        if (heldItem != null && Utility.IsNormalObjectAtParentSheetIndex(heldItem, heldItem.ParentSheetIndex) && aquariumData.ContainsKey(heldItem.ParentSheetIndex))
          objList1.Add(heldItem);
      }
      List<Item> objList2 = new List<Item>();
      foreach (Item key in this._fishLookup.Keys)
      {
        if (!this.heldItems.Contains(key))
          objList2.Add(key);
      }
      for (int index = 0; index < objList1.Count; ++index)
      {
        Item key = objList1[index];
        if (!this._fishLookup.ContainsKey(key))
        {
          TankFish tankFish = new TankFish(this, key);
          this.tankFish.Add(tankFish);
          this._fishLookup[key] = tankFish;
        }
      }
      foreach (Item key in objList2)
      {
        this.tankFish.Remove(this._fishLookup[key]);
        this.heldItems.Remove(key);
      }
    }

    public virtual void UpdateDecorAndFish()
    {
      Random rng = new Random(this.generationSeed.Value);
      this.UpdateFish();
      this.decorationSlots.Clear();
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < this.getTilesWide(); ++index2)
        {
          Vector2 vector2 = new Vector2();
          if (index1 % 2 == 0)
          {
            if (index2 != this.getTilesWide() - 1)
              vector2.X = (float) (16 + index2 * 16);
            else
              continue;
          }
          else
            vector2.X = (float) (8 + index2 * 16);
          vector2.Y = 4f;
          vector2.Y += 3.333333f * (float) index1;
          this.decorationSlots.Add(vector2);
        }
      }
      this.floorDecorationIndices.Clear();
      this.floorDecorations.Clear();
      this._currentDecorationIndex = 0;
      for (int index = 0; index < this.decorationSlots.Count; ++index)
      {
        this.floorDecorationIndices.Add(index);
        this.floorDecorations.Add(new KeyValuePair<Rectangle, Vector2>?());
      }
      Utility.Shuffle<int>(rng, this.floorDecorationIndices);
      Random random1 = new Random(rng.Next());
      bool flag1 = this.GetItemCount(393) > 0;
      for (int index = 0; index < 1; ++index)
      {
        if (flag1)
          this.AddFloorDecoration(new Rectangle(16 * random1.Next(0, 5), 256, 16, 16));
        else
          this._AdvanceDecorationIndex();
      }
      Random random2 = new Random(rng.Next());
      bool flag2 = this.GetItemCount(152) > 0;
      for (int index = 0; index < 4; ++index)
      {
        if (flag2)
          this.AddFloorDecoration(new Rectangle(16 * random2.Next(0, 3), 288, 16, 16));
        else
          this._AdvanceDecorationIndex();
      }
      Random random3 = new Random(rng.Next());
      bool flag3 = this.GetItemCount(390) > 0;
      for (int index = 0; index < 2; ++index)
      {
        if (flag3)
          this.AddFloorDecoration(new Rectangle(16 * random3.Next(0, 3), 272, 16, 16));
        else
          this._AdvanceDecorationIndex();
      }
    }

    public virtual void AddFloorDecoration(Rectangle source_rect)
    {
      if (this._currentDecorationIndex == -1)
        return;
      int floorDecorationIndex = this.floorDecorationIndices[this._currentDecorationIndex];
      this._AdvanceDecorationIndex();
      int x = (int) this.decorationSlots[floorDecorationIndex].X;
      int y = (int) this.decorationSlots[floorDecorationIndex].Y;
      if (x < source_rect.Width / 2)
        x = source_rect.Width / 2;
      if (x > this.GetTankBounds().Width / 4 - source_rect.Width / 2)
        x = this.GetTankBounds().Width / 4 - source_rect.Width / 2;
      KeyValuePair<Rectangle, Vector2> keyValuePair = new KeyValuePair<Rectangle, Vector2>(source_rect, new Vector2((float) x, (float) y));
      this.floorDecorations[floorDecorationIndex] = new KeyValuePair<Rectangle, Vector2>?(keyValuePair);
    }

    protected virtual void _AdvanceDecorationIndex()
    {
      for (int index = 0; index < this.decorationSlots.Count; ++index)
      {
        ++this._currentDecorationIndex;
        if (this._currentDecorationIndex >= this.decorationSlots.Count)
          this._currentDecorationIndex = 0;
        if (!this.floorDecorations[this.floorDecorationIndices[this._currentDecorationIndex]].HasValue)
          return;
      }
      this._currentDecorationIndex = 1;
    }

    public override void OnMenuClose()
    {
      this.refreshFishEvent.Fire();
      base.OnMenuClose();
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
    }

    public Vector2 GetFishSortRegion() => new Vector2(this.GetBaseDrawLayer() + 1E-06f, this.GetGlassDrawLayer() - 1E-06f);

    public float GetGlassDrawLayer() => this.GetBaseDrawLayer() + 0.0001f;

    public float GetBaseDrawLayer() => (int) (NetFieldBase<int, NetInt>) this.furniture_type != 12 ? (float) (this.boundingBox.Value.Bottom - ((int) (NetFieldBase<int, NetInt>) this.furniture_type == 6 || (int) (NetFieldBase<int, NetInt>) this.furniture_type == 13 ? 48 : 8)) / 10000f : 2E-09f;

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      Vector2 vector2_1 = Vector2.Zero;
      if (this.isTemporarilyInvisible)
        return;
      Vector2 vector2_2 = this.drawPosition.Value;
      if (!Furniture.isDrawingLocationFurniture)
      {
        vector2_2 = new Vector2((float) x, (float) y) * 64f;
        vector2_2.Y -= (float) (this.sourceRect.Height * 4 - this.boundingBox.Height);
      }
      if (this.shakeTimer > 0)
        vector2_1 = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, vector2_2 + vector2_1), new Rectangle?(new Rectangle(this.sourceRect.Value.X + this.sourceRect.Value.Width, this.sourceRect.Value.Y, this.sourceRect.Value.Width, this.sourceRect.Value.Height)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.GetGlassDrawLayer());
      if (Furniture.isDrawingLocationFurniture)
      {
        int num1 = 0;
        for (int index = 0; index < this.tankFish.Count; ++index)
        {
          TankFish tankFish = this.tankFish[index];
          float draw_layer = Utility.Lerp(this.GetFishSortRegion().Y, this.GetFishSortRegion().X, tankFish.zPosition / 20f) + 1E-07f * (float) index;
          tankFish.Draw(spriteBatch, alpha, draw_layer);
          if (tankFish.fishIndex == 86)
          {
            int num2 = 0;
            foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
            {
              if (heldItem is Hat)
              {
                if (num2 == num1)
                {
                  heldItem.drawInMenu(spriteBatch, Game1.GlobalToLocal(tankFish.GetWorldPosition() + new Vector2((float) ((tankFish.facingLeft ? -4 : 0) - 30), -55f)), 0.75f, 1f, draw_layer + 1E-08f, StackDrawType.Hide);
                  ++num1;
                  break;
                }
                ++num2;
              }
            }
          }
        }
        Rectangle tankBounds;
        for (int index = 0; index < this.floorDecorations.Count; ++index)
        {
          KeyValuePair<Rectangle, Vector2>? floorDecoration = this.floorDecorations[index];
          if (floorDecoration.HasValue)
          {
            floorDecoration = this.floorDecorations[index];
            KeyValuePair<Rectangle, Vector2> keyValuePair = floorDecoration.Value;
            Vector2 vector2_3 = keyValuePair.Value;
            Rectangle key = keyValuePair.Key;
            float num3 = Utility.Lerp(this.GetFishSortRegion().Y, this.GetFishSortRegion().X, vector2_3.Y / 20f) - 1E-06f;
            SpriteBatch spriteBatch1 = spriteBatch;
            Texture2D aquariumTexture = this.GetAquariumTexture();
            tankBounds = this.GetTankBounds();
            double x1 = (double) tankBounds.Left + (double) vector2_3.X * 4.0;
            tankBounds = this.GetTankBounds();
            double y1 = (double) (tankBounds.Bottom - 4) - (double) vector2_3.Y * 4.0;
            Vector2 local = Game1.GlobalToLocal(new Vector2((float) x1, (float) y1));
            Rectangle? sourceRectangle = new Rectangle?(key);
            Color color = Color.White * alpha;
            Vector2 origin = new Vector2((float) (key.Width / 2), (float) (key.Height - 4));
            double layerDepth = (double) num3;
            spriteBatch1.Draw(aquariumTexture, local, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
          }
        }
        foreach (Vector4 bubble in this.bubbles)
        {
          float num4 = Utility.Lerp(this.GetFishSortRegion().Y, this.GetFishSortRegion().X, bubble.Z / 20f) - 1E-06f;
          SpriteBatch spriteBatch2 = spriteBatch;
          Texture2D aquariumTexture = this.GetAquariumTexture();
          tankBounds = this.GetTankBounds();
          double x2 = (double) tankBounds.Left + (double) bubble.X;
          tankBounds = this.GetTankBounds();
          double y2 = (double) (tankBounds.Bottom - 4) - (double) bubble.Y - (double) bubble.Z * 4.0;
          Vector2 local = Game1.GlobalToLocal(new Vector2((float) x2, (float) y2));
          Rectangle? sourceRectangle = new Rectangle?(new Rectangle(0, 240, 16, 16));
          Color color = Color.White * alpha;
          Vector2 origin = new Vector2(8f, 8f);
          double scale = 4.0 * (double) bubble.W;
          double layerDepth = (double) num4;
          spriteBatch2.Draw(aquariumTexture, local, sourceRectangle, color, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
        }
      }
      base.draw(spriteBatch, x, y, alpha);
    }

    public int GetItemCount(int parent_sheet_index)
    {
      int itemCount = 0;
      foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
      {
        if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, parent_sheet_index))
          itemCount += heldItem.Stack;
      }
      return itemCount;
    }

    public virtual Rectangle GetTankBounds()
    {
      int num1 = this.defaultSourceRect.Value.Height / 16;
      int num2 = this.defaultSourceRect.Value.Width / 16;
      Rectangle tankBounds = new Rectangle((int) this.TileLocation.X * 64, (int) (((double) this.TileLocation.Y - (double) this.getTilesHigh() - 1.0) * 64.0), num2 * 64, num1 * 64);
      tankBounds.X += 4;
      tankBounds.Width -= 8;
      tankBounds.Height -= 28;
      tankBounds.Y += 64;
      tankBounds.Height -= 64;
      return tankBounds;
    }

    public enum FishTankCategories
    {
      None,
      Swim,
      Ground,
      Decoration,
    }
  }
}
