// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Boots
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Boots : Item
  {
    [XmlElement("defenseBonus")]
    public readonly NetInt defenseBonus = new NetInt();
    [XmlElement("immunityBonus")]
    public readonly NetInt immunityBonus = new NetInt();
    [XmlElement("indexInTileSheet")]
    public readonly NetInt indexInTileSheet = new NetInt();
    [XmlElement("price")]
    public readonly NetInt price = new NetInt();
    [XmlElement("indexInColorSheet")]
    public readonly NetInt indexInColorSheet = new NetInt();
    [XmlElement("appliedBootSheetIndex")]
    public readonly NetInt appliedBootSheetIndex = new NetInt(-1);
    [XmlIgnore]
    public string displayName;
    [XmlIgnore]
    public string description;

    public Boots()
    {
      this.NetFields.AddFields((INetSerializable) this.defenseBonus, (INetSerializable) this.immunityBonus, (INetSerializable) this.indexInTileSheet, (INetSerializable) this.price, (INetSerializable) this.indexInColorSheet);
      this.Category = -97;
    }

    public Boots(int which)
      : this()
    {
      this.indexInTileSheet.Value = which;
      this.reloadData();
      this.Category = -97;
    }

    public virtual void reloadData()
    {
      string[] strArray = Game1.content.Load<Dictionary<int, string>>("Data\\Boots")[this.indexInTileSheet.Value].Split('/');
      this.Name = strArray[0];
      this.price.Value = Convert.ToInt32(strArray[2]);
      this.defenseBonus.Value = Convert.ToInt32(strArray[3]);
      this.immunityBonus.Value = Convert.ToInt32(strArray[4]);
      this.indexInColorSheet.Value = Convert.ToInt32(strArray[5]);
    }

    public void applyStats(Boots applied_boots)
    {
      this.reloadData();
      if (this.defenseBonus.Value == (int) (NetFieldBase<int, NetInt>) applied_boots.defenseBonus && this.immunityBonus.Value == (int) (NetFieldBase<int, NetInt>) applied_boots.immunityBonus)
        this.appliedBootSheetIndex.Value = -1;
      else
        this.appliedBootSheetIndex.Value = applied_boots.getStatsIndex();
      this.defenseBonus.Value = applied_boots.defenseBonus.Value;
      this.immunityBonus.Value = applied_boots.immunityBonus.Value;
      this.price.Value = applied_boots.price.Value;
      this.loadDisplayFields();
    }

    public virtual int getStatsIndex() => this.appliedBootSheetIndex.Value >= 0 ? this.appliedBootSheetIndex.Value : this.indexInTileSheet.Value;

    public override int salePrice() => (int) (NetFieldBase<int, NetInt>) this.defenseBonus * 100 + (int) (NetFieldBase<int, NetInt>) this.immunityBonus * 100;

    public virtual void onEquip()
    {
      Game1.player.resilience += (int) (NetFieldBase<int, NetInt>) this.defenseBonus;
      Game1.player.immunity += (int) (NetFieldBase<int, NetInt>) this.immunityBonus;
      Game1.player.changeShoeColor((int) (NetFieldBase<int, NetInt>) this.indexInColorSheet);
    }

    public virtual void onUnequip()
    {
      Game1.player.resilience -= (int) (NetFieldBase<int, NetInt>) this.defenseBonus;
      Game1.player.immunity -= (int) (NetFieldBase<int, NetInt>) this.immunityBonus;
      Game1.player.changeShoeColor(12);
    }

    public int getNumberOfDescriptionCategories() => (int) (NetFieldBase<int, NetInt>) this.immunityBonus > 0 && (int) (NetFieldBase<int, NetInt>) this.defenseBonus > 0 ? 2 : 1;

    public override void drawTooltip(
      SpriteBatch spriteBatch,
      ref int x,
      ref int y,
      SpriteFont font,
      float alpha,
      StringBuilder overrideText)
    {
      Utility.drawTextWithShadow(spriteBatch, Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth()), font, new Vector2((float) (x + 16), (float) (y + 16 + 4)), Game1.textColor);
      y += (int) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y;
      if ((int) (NetFieldBase<int, NetInt>) this.defenseBonus > 0)
      {
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) this.defenseBonus), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), Game1.textColor * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
      if ((int) (NetFieldBase<int, NetInt>) this.immunityBonus <= 0)
        return;
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y + 16 + 4)), new Rectangle(150, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", (object) this.immunityBonus), font, new Vector2((float) (x + 16 + 52), (float) (y + 16 + 12)), Game1.textColor * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }

    public override Point getExtraSpaceNeededForTooltipSpecialIcons(
      SpriteFont font,
      int minWidth,
      int horizontalBuffer,
      int startingHeight,
      StringBuilder descriptionText,
      string boldTitleText,
      int moneyAmountToDisplayAtBottom)
    {
      int sub1 = 9999;
      Point tooltipSpecialIcons = new Point(0, startingHeight);
      tooltipSpecialIcons.Y -= (int) font.MeasureString(descriptionText).Y;
      tooltipSpecialIcons.Y += (int) ((double) (this.getNumberOfDescriptionCategories() * 4 * 12) + (double) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y);
      tooltipSpecialIcons.X = (int) Math.Max((float) minWidth, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) sub1)).X + (float) horizontalBuffer, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", (object) sub1)).X + (float) horizontalBuffer));
      return tooltipSpecialIcons;
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
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.indexInTileSheet.Value, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
    }

    public override int maximumStackSize() => 1;

    public override int addToStack(Item stack) => 1;

    public override string getCategoryName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Boots.cs.12501");

    public override string getDescription()
    {
      if (this.description == null)
        this.loadDisplayFields();
      return Game1.parseText(this.description + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Boots.cs.12500", (object) ((int) (NetFieldBase<int, NetInt>) this.immunityBonus + (int) (NetFieldBase<int, NetInt>) this.defenseBonus)), Game1.smallFont, this.getDescriptionWidth());
    }

    public override bool isPlaceable() => false;

    [XmlIgnore]
    public override string DisplayName
    {
      get
      {
        if (this.displayName == null)
          this.loadDisplayFields();
        return this.displayName;
      }
      set => this.displayName = value;
    }

    [XmlIgnore]
    public override int Stack
    {
      get => 1;
      set
      {
      }
    }

    public override Item getOne()
    {
      Boots one = new Boots((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet);
      one.appliedBootSheetIndex.Value = this.appliedBootSheetIndex.Value;
      one.indexInColorSheet.Value = this.indexInColorSheet.Value;
      one.defenseBonus.Value = this.defenseBonus.Value;
      one.immunityBonus.Value = this.immunityBonus.Value;
      one.loadDisplayFields();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected virtual bool loadDisplayFields()
    {
      if (!((NetFieldBase<int, NetInt>) this.indexInTileSheet != (NetInt) null))
        return false;
      string[] strArray = Game1.content.Load<Dictionary<int, string>>("Data\\Boots")[(int) (NetFieldBase<int, NetInt>) this.indexInTileSheet].Split('/');
      this.displayName = this.Name;
      if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
        this.displayName = strArray[strArray.Length - 1];
      if (this.appliedBootSheetIndex.Value >= 0)
        this.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:CustomizedBootItemName", (object) this.DisplayName);
      this.description = strArray[1];
      return true;
    }
  }
}
