// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.CombinedRing
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Menus;
using StardewValley.Monsters;
using System;

namespace StardewValley.Objects
{
  public class CombinedRing : Ring
  {
    public NetList<Ring, NetRef<Ring>> combinedRings = new NetList<Ring, NetRef<Ring>>();

    public CombinedRing() => this.NetFields.AddField((INetSerializable) this.combinedRings);

    public CombinedRing(int parent_sheet_index)
      : base(880)
    {
      this.NetFields.AddField((INetSerializable) this.combinedRings);
    }

    public virtual void UpdateDescription() => this.loadDisplayFields();

    protected override bool loadDisplayFields()
    {
      base.loadDisplayFields();
      this.description = "";
      foreach (Ring combinedRing in this.combinedRings)
      {
        combinedRing.getDescription();
        this.description = this.description + combinedRing.description + "\n\n";
      }
      this.description = this.description.Trim();
      return true;
    }

    public override bool GetsEffectOfRing(int ring_index)
    {
      foreach (Ring combinedRing in this.combinedRings)
      {
        if (combinedRing.GetsEffectOfRing(ring_index))
          return true;
      }
      return base.GetsEffectOfRing(ring_index);
    }

    public override Item getOne()
    {
      CombinedRing one = new CombinedRing((int) (NetFieldBase<int, NetInt>) this.indexInTileSheet);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public override void _GetOneFrom(Item source)
    {
      this.combinedRings.Clear();
      foreach (Item combinedRing in (source as CombinedRing).combinedRings)
        this.combinedRings.Add(combinedRing.getOne() as Ring);
      this.loadDisplayFields();
      base._GetOneFrom(source);
    }

    public override int GetEffectsOfRingMultiplier(int ring_index)
    {
      int ofRingMultiplier = 0;
      foreach (Ring combinedRing in this.combinedRings)
        ofRingMultiplier += combinedRing.GetEffectsOfRingMultiplier(ring_index);
      return ofRingMultiplier;
    }

    public override void onDayUpdate(Farmer who, GameLocation location)
    {
      foreach (Ring combinedRing in this.combinedRings)
        combinedRing.onDayUpdate(who, location);
      base.onDayUpdate(who, location);
    }

    public override void onEquip(Farmer who, GameLocation location)
    {
      foreach (Ring combinedRing in this.combinedRings)
        combinedRing.onEquip(who, location);
      base.onEquip(who, location);
    }

    public override void onLeaveLocation(Farmer who, GameLocation environment)
    {
      foreach (Ring combinedRing in this.combinedRings)
        combinedRing.onLeaveLocation(who, environment);
      base.onLeaveLocation(who, environment);
    }

    public override void onMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
      foreach (Ring combinedRing in this.combinedRings)
        combinedRing.onMonsterSlay(m, location, who);
      base.onMonsterSlay(m, location, who);
    }

    public override void onUnequip(Farmer who, GameLocation location)
    {
      foreach (Ring combinedRing in this.combinedRings)
        combinedRing.onUnequip(who, location);
      base.onUnequip(who, location);
    }

    public override void onNewLocation(Farmer who, GameLocation environment)
    {
      foreach (Ring combinedRing in this.combinedRings)
        combinedRing.onNewLocation(who, environment);
      base.onNewLocation(who, environment);
    }

    public void FixCombinedRing()
    {
      if (this.ParentSheetIndex == 880)
        return;
      string[] strArray = Game1.objectInformation[880].Split('/');
      this.Category = -96;
      this.Name = strArray[0];
      this.price.Value = Convert.ToInt32(strArray[1]);
      this.indexInTileSheet.Value = 880;
      this.ParentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.indexInTileSheet;
      this.loadDisplayFields();
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
      if (this.combinedRings.Count >= 2)
      {
        float num = scaleSize;
        scaleSize = 1f;
        location.Y -= (float) (((double) num - 1.0) * 32.0);
        Rectangle standardTileSheet1 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.combinedRings[0].indexInTileSheet, 16, 16);
        standardTileSheet1.X += 5;
        standardTileSheet1.Y += 7;
        standardTileSheet1.Width = 4;
        standardTileSheet1.Height = 6;
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(-12f, 8f) * scaleSize, new Rectangle?(standardTileSheet1), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
        ++standardTileSheet1.X;
        standardTileSheet1.Y += 4;
        standardTileSheet1.Width = 3;
        standardTileSheet1.Height = 1;
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(-8f, 4f) * scaleSize, new Rectangle?(standardTileSheet1), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
        Rectangle standardTileSheet2 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.combinedRings[1].indexInTileSheet, 16, 16);
        standardTileSheet2.X += 9;
        standardTileSheet2.Y += 7;
        standardTileSheet2.Width = 4;
        standardTileSheet2.Height = 6;
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 8f) * scaleSize, new Rectangle?(standardTileSheet2), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
        standardTileSheet2.Y += 4;
        standardTileSheet2.Width = 3;
        standardTileSheet2.Height = 1;
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 4f) * scaleSize, new Rectangle?(standardTileSheet2), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
        Color? dyeColor1 = TailoringMenu.GetDyeColor((Item) this.combinedRings[0]);
        Color? dyeColor2 = TailoringMenu.GetDyeColor((Item) this.combinedRings[1]);
        Color red = Color.Red;
        Color blue = Color.Blue;
        if (dyeColor1.HasValue)
          red = dyeColor1.Value;
        if (dyeColor2.HasValue)
          blue = dyeColor2.Value;
        base.drawInMenu(spriteBatch, location + new Vector2(-5f, -1f), scaleSize, transparency, layerDepth, drawStackNumber, Utility.Get2PhaseColor(red, blue), drawShadow);
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(13f, 35f) * scaleSize, new Rectangle?(new Rectangle(263, 579, 4, 2)), Utility.Get2PhaseColor(red, blue, timeOffset: 1125f) * transparency, -1.570796f, new Vector2(2f, 1.5f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(49f, 35f) * scaleSize, new Rectangle?(new Rectangle(263, 579, 4, 2)), Utility.Get2PhaseColor(red, blue, timeOffset: 375f) * transparency, 1.570796f, new Vector2(2f, 1.5f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(31f, 53f) * scaleSize, new Rectangle?(new Rectangle(263, 579, 4, 2)), Utility.Get2PhaseColor(red, blue, timeOffset: 750f) * transparency, 3.141593f, new Vector2(2f, 1.5f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      }
      else
        base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
    }

    public override void update(GameTime time, GameLocation environment, Farmer who)
    {
      foreach (Ring combinedRing in this.combinedRings)
        combinedRing.update(time, environment, who);
      base.update(time, environment, who);
    }
  }
}
