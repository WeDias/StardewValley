// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Mill
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Buildings
{
  public class Mill : Building
  {
    [XmlElement("input")]
    public readonly NetRef<Chest> input = new NetRef<Chest>();
    [XmlElement("output")]
    public readonly NetRef<Chest> output = new NetRef<Chest>();
    private bool hasLoadedToday;
    private Rectangle baseSourceRect = new Rectangle(0, 0, 64, 128);

    public Mill(BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
      this.input.Value = new Chest(true);
      this.output.Value = new Chest(true);
    }

    public Mill()
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.input, (INetSerializable) this.output);
    }

    public override Rectangle getSourceRectForMenu() => new Rectangle(0, 0, 64, this.texture.Value.Bounds.Height);

    public override void load() => base.load();

    public override bool doAction(Vector2 tileLocation, Farmer who)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0)
      {
        if ((double) tileLocation.X == (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + 1) && (double) tileLocation.Y == (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1))
        {
          if (who != null && who.ActiveObject != null)
          {
            bool flag = false;
            switch ((int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex)
            {
              case 262:
              case 271:
              case 284:
                flag = true;
                break;
            }
            if (!flag)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantMill"));
              return false;
            }
            StardewValley.Object activeObject = who.ActiveObject;
            who.ActiveObject = (StardewValley.Object) null;
            StardewValley.Object thisInventoryList = (StardewValley.Object) Utility.addItemToThisInventoryList((Item) activeObject, (IList<Item>) this.input.Value.items, 36);
            if (thisInventoryList != null)
              who.ActiveObject = thisInventoryList;
            this.hasLoadedToday = true;
            Game1.playSound("Ship");
            if (who.ActiveObject != null)
              Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:MillFull"));
          }
        }
        else if ((double) tileLocation.X == (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + 3) && (double) tileLocation.Y == (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1))
        {
          Utility.CollectSingleItemOrShowChestMenu(this.output.Value, (object) this);
          return true;
        }
      }
      return base.doAction(tileLocation, who);
    }

    public override void dayUpdate(int dayOfMonth)
    {
      this.hasLoadedToday = false;
      for (int index = this.input.Value.items.Count - 1; index >= 0; --index)
      {
        if (this.input.Value.items[index] != null)
        {
          Item i = (Item) null;
          switch ((int) (NetFieldBase<int, NetInt>) this.input.Value.items[index].parentSheetIndex)
          {
            case 245:
            case 246:
            case 423:
              i = this.input.Value.items[index];
              break;
            case 262:
              i = (Item) new StardewValley.Object(246, this.input.Value.items[index].Stack);
              break;
            case 271:
              i = (Item) new StardewValley.Object(423, this.input.Value.items[index].Stack);
              break;
            case 284:
              i = (Item) new StardewValley.Object(245, 3 * this.input.Value.items[index].Stack);
              break;
          }
          if (i != null && Utility.canItemBeAddedToThisInventoryList(i, (IList<Item>) this.output.Value.items, 36))
            this.input.Value.items[index] = Utility.addItemToThisInventoryList(i, (IList<Item>) this.output.Value.items, 36);
        }
      }
      base.dayUpdate(dayOfMonth);
    }

    public override List<Item> GetAdditionalItemsToCheckBeforeDemolish() => new List<Item>((IEnumerable<Item>) this.output.Value.items);

    public override void drawInMenu(SpriteBatch b, int x, int y)
    {
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Rectangle?(this.getSourceRectForMenu()), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.89f);
      b.Draw(this.texture.Value, new Vector2((float) (x + 32), (float) (y + 4)), new Rectangle?(new Rectangle(64 + (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 % 160, (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 / 160 * 32, 32, 32)), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.9f);
    }

    public override void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
      {
        this.drawInConstruction(b);
      }
      else
      {
        this.drawShadow(b);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Rectangle?(this.baseSourceRect), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) this.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64) / 10000f);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 32), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 + 4))), new Rectangle?(new Rectangle(64 + (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 % 160, (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 / 160 * 32, 32, 32)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) this.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000f);
        if (this.hasLoadedToday)
          b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 52), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 + 276))), new Rectangle?(new Rectangle(64 + (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 700 / 100 * 21, 72, 21, 8)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) this.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000f);
        if (this.output.Value.items.Count <= 0 || this.output.Value.items[0] == null || (int) (NetFieldBase<int, NetInt>) this.output.Value.items[0].parentSheetIndex != 245 && (int) (NetFieldBase<int, NetInt>) this.output.Value.items[0].parentSheetIndex != 246 && (int) (NetFieldBase<int, NetInt>) this.output.Value.items[0].parentSheetIndex != 423)
          return;
        float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 192), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - 96) + num)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64) / 10000.0 + 9.99999997475243E-07 + (double) (int) (NetFieldBase<int, NetInt>) this.tileX / 10000.0));
        b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 192 + 32 + 4), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - 64 + 8) + num)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.output.Value.items[0].parentSheetIndex, 16, 16)), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64) / 10000.0 + 9.99999974737875E-06 + (double) (int) (NetFieldBase<int, NetInt>) this.tileX / 10000.0));
      }
    }
  }
}
