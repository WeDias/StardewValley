// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Sign
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Sign : StardewValley.Object
  {
    public const int OBJECT = 1;
    public const int HAT = 2;
    public const int BIG_OBJECT = 3;
    public const int RING = 4;
    public const int FURNITURE = 5;
    [XmlElement("displayItem")]
    public readonly NetRef<Item> displayItem = new NetRef<Item>();
    [XmlElement("displayType")]
    public readonly NetInt displayType = new NetInt();

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.displayItem, (INetSerializable) this.displayType);
    }

    public Sign()
    {
    }

    public Sign(Vector2 tile, int which)
      : base(tile, which)
    {
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return who.CurrentItem != null;
      Item currentItem = who.CurrentItem;
      if (currentItem == null)
        return false;
      if (who.isMoving())
        Game1.haltAfterCheck = false;
      this.displayItem.Value = currentItem.getOne();
      Game1.playSound("coin");
      this.displayType.Value = 1;
      if (this.displayItem.Value is Hat)
        this.displayType.Value = 2;
      else if (this.displayItem.Value is Ring)
        this.displayType.Value = 4;
      else if (this.displayItem.Value is Furniture)
        this.displayType.Value = 5;
      else if (this.displayItem.Value is StardewValley.Object)
        this.displayType.Value = (bool) (NetFieldBase<bool, NetBool>) (this.displayItem.Value as StardewValley.Object).bigCraftable ? 3 : 1;
      return true;
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      base.draw(spriteBatch, x, y, alpha);
      if (this.displayItem.Value == null)
        return;
      switch (this.displayType.Value)
      {
        case 1:
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64) + 2f, (float) (y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 9.99999974737875E-06), StackDrawType.Hide, Color.Black, false);
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64) + 2f, (float) (y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 1.99999994947575E-05), StackDrawType.Hide, Color.White, false);
          break;
        case 2:
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64) + 1f, (float) (y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 9.99999974737875E-06), StackDrawType.Hide, Color.Black, false);
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64) + 1f, (float) (y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 1.99999994947575E-05), StackDrawType.Hide, Color.White, false);
          break;
        case 3:
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 9.99999974737875E-06), StackDrawType.Hide, Color.White, false);
          break;
        case 4:
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64) - 1f, (float) (y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 9.99999974737875E-06), StackDrawType.Hide, Color.Black, false);
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64) - 1f, (float) (y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 1.99999994947575E-05), StackDrawType.Hide, Color.White, false);
          break;
        case 5:
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 9.99999974737875E-06), StackDrawType.Hide, Color.Black, false);
          this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (double) x * 9.99999974737875E-06 + 1.99999994947575E-05), StackDrawType.Hide, Color.White, false);
          break;
      }
    }
  }
}
