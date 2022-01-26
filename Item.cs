// Decompiled with JetBrains decompiler
// Type: StardewValley.Item
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace StardewValley
{
  [XmlInclude(typeof (ModDataDictionary))]
  [XmlInclude(typeof (Object))]
  [XmlInclude(typeof (Tool))]
  [InstanceStatics]
  public abstract class Item : IComparable, INetObject<NetFields>, ISalable
  {
    public bool isLostItem;
    [XmlElement("specialVariable")]
    private readonly NetInt specialVariable = new NetInt();
    [XmlElement("category")]
    public readonly NetInt category = new NetInt();
    [XmlElement("hasBeenInInventory")]
    public readonly NetBool hasbeenInInventory = new NetBool();
    private HashSet<string> _contextTags;
    protected bool _contextTagsDirty;
    /// <summary>
    /// Used for modders to store metadata to this object. This data is synchronized in multiplayer and saved to the save data.
    /// </summary>
    [XmlIgnore]
    public ModDataDictionary modData = new ModDataDictionary();
    [XmlElement("name")]
    public readonly NetString netName = new NetString();
    [XmlElement("parentSheetIndex")]
    public readonly NetInt parentSheetIndex = new NetInt();
    public bool specialItem;

    /// <summary>Get the mod populated metadata as it will be serialized for game saving. Identical to <see cref="F:StardewValley.Item.modData" /> except returns null during save if it is empty. It is strongly recommended to use <see cref="F:StardewValley.Item.modData" /> instead.</summary>
    [XmlElement("modData")]
    public ModDataDictionary modDataForSerialization
    {
      get => this.modData.GetForSerialization();
      set => this.modData.SetFromSerialization(value);
    }

    public int SpecialVariable
    {
      get => (int) (NetFieldBase<int, NetInt>) this.specialVariable;
      set => this.specialVariable.Set(value);
    }

    [XmlIgnore]
    public int Category
    {
      get => (int) (NetFieldBase<int, NetInt>) this.category;
      set => this.category.Set(value);
    }

    [XmlIgnore]
    public bool HasBeenInInventory
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.hasbeenInInventory;
      set => this.hasbeenInInventory.Set(value);
    }

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public bool IsInfiniteStock() => this.isLostItem;

    public void MarkContextTagsDirty() => this._contextTagsDirty = true;

    public List<string> GetContextTagList() => this.GetContextTags().ToList<string>();

    public HashSet<string> GetContextTags()
    {
      if (this._contextTags == null || this._contextTagsDirty)
        this._GenerateContextTags();
      return this._contextTags;
    }

    public bool HasContextTag(string tag)
    {
      bool flag = true;
      if (tag.Length > 1 && tag[0] == '!')
      {
        tag = tag.Substring(1);
        flag = false;
      }
      return this.GetContextTags().Contains(tag) == flag;
    }

    protected void _GenerateContextTags()
    {
      this._contextTagsDirty = false;
      this._contextTags = new HashSet<string>();
      string str1 = "id_" + Utility.getStandardDescriptionFromItem(this, 0, '_');
      string lower = str1.Substring(0, str1.Length - 2).ToLower();
      this._contextTags.Add(lower);
      if (this.Name != null && Game1.objectContextTags.ContainsKey(this.Name))
      {
        foreach (string str2 in Game1.objectContextTags[this.Name].Split(','))
        {
          string str3 = str2.Trim();
          if (str3.Length > 0)
            this._contextTags.Add(str3);
        }
      }
      else if (this.Name != null && Game1.objectContextTags.ContainsKey(lower))
      {
        foreach (string str4 in Game1.objectContextTags[lower].Split(','))
        {
          string str5 = str4.Trim();
          if (str5.Length > 0)
            this._contextTags.Add(str5);
        }
      }
      this._PopulateContextTags(this._contextTags);
    }

    protected virtual void _PopulateContextTags(HashSet<string> tags)
    {
      if (this.Name != null)
        tags.Add("item_" + this.SanitizeContextTag(this.Name));
      switch (this.category.Value)
      {
        case -100:
          tags.Add("category_clothing");
          break;
        case -99:
          tags.Add("category_tool");
          break;
        case -98:
          tags.Add("category_weapon");
          break;
        case -97:
          tags.Add("category_boots");
          break;
        case -96:
          tags.Add("category_ring");
          break;
        case -95:
          tags.Add("category_hat");
          break;
        case -81:
          tags.Add("category_greens");
          break;
        case -80:
          tags.Add("category_flowers");
          break;
        case -79:
          tags.Add("category_fruits");
          break;
        case -75:
          tags.Add("category_vegetable");
          break;
        case -74:
          tags.Add("category_seeds");
          break;
        case -29:
          tags.Add("category_equipment");
          break;
        case -28:
          tags.Add("category_monster_loot");
          break;
        case -27:
          tags.Add("category_syrup");
          break;
        case -26:
          tags.Add("category_artisan_goods");
          break;
        case -25:
          tags.Add("category_ingredients");
          break;
        case -24:
          tags.Add("category_furniture");
          break;
        case -23:
          tags.Add("category_sell_at_fish_shop");
          break;
        case -22:
          tags.Add("category_tackle");
          break;
        case -21:
          tags.Add("category_bait");
          break;
        case -20:
          tags.Add("category_junk");
          break;
        case -19:
          tags.Add("category_fertilizer");
          break;
        case -18:
          tags.Add("category_sell_at_pierres_and_marnies");
          break;
        case -17:
          tags.Add("category_sell_at_pierres");
          break;
        case -16:
          tags.Add("category_building_resources");
          break;
        case -15:
          tags.Add("category_metal_resources");
          break;
        case -14:
          tags.Add("category_meat");
          break;
        case -12:
          tags.Add("category_minerals");
          break;
        case -9:
          tags.Add("category_big_craftable");
          break;
        case -8:
          tags.Add("category_crafting");
          break;
        case -7:
          tags.Add("category_cooking");
          break;
        case -6:
          tags.Add("category_milk");
          break;
        case -5:
          tags.Add("category_egg");
          break;
        case -4:
          tags.Add("category_fish");
          this._PopulateFishContextTags(tags);
          break;
        case -2:
          tags.Add("category_gem");
          break;
      }
    }

    protected void _PopulateFishContextTags(HashSet<string> tags)
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
      if (!dictionary.ContainsKey(this.ParentSheetIndex))
        return;
      string[] strArray = dictionary[this.ParentSheetIndex].Split('/');
      if (strArray[1] == "trap")
      {
        tags.Add("fish_trap_location_" + strArray[4]);
      }
      else
      {
        tags.Add("fish_motion_" + strArray[2]);
        int int32 = Convert.ToInt32(strArray[1]);
        if (int32 <= 33)
          tags.Add("fish_difficulty_easy");
        else if (int32 <= 66)
          tags.Add("fish_difficulty_medium");
        else if (int32 <= 100)
          tags.Add("fish_difficulty_hard");
        else
          tags.Add("fish_difficulty_extremely_hard");
        tags.Add("fish_favor_weather_" + strArray[7]);
      }
    }

    public string SanitizeContextTag(string tag) => tag.Trim().ToLower().Replace(' ', '_').Replace("'", "");

    protected Item()
    {
      this.NetFields.AddFields((INetSerializable) this.specialVariable, (INetSerializable) this.category, (INetSerializable) this.netName, (INetSerializable) this.parentSheetIndex, (INetSerializable) this.hasbeenInInventory);
      this.NetFields.AddField((INetSerializable) this.modData);
      this.parentSheetIndex.Value = -1;
    }

    public virtual bool ShouldSerializeparentSheetIndex() => this.parentSheetIndex.Value != -1;

    [XmlIgnore]
    public int ParentSheetIndex
    {
      get => (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex;
      set => this.parentSheetIndex.Value = value;
    }

    public virtual void drawTooltip(
      SpriteBatch spriteBatch,
      ref int x,
      ref int y,
      SpriteFont font,
      float alpha,
      StringBuilder overrideText)
    {
      if (overrideText == null || overrideText.Length == 0 || overrideText.Length == 1 && overrideText[0] == ' ')
        return;
      spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16), (float) (y + 16 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
      spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16), (float) (y + 16 + 4)) + new Vector2(0.0f, 2f), Game1.textShadowColor * alpha);
      spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16), (float) (y + 16 + 4)) + new Vector2(2f, 0.0f), Game1.textShadowColor * alpha);
      spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16), (float) (y + 16 + 4)), Game1.textColor * 0.9f * alpha);
      y += (int) font.MeasureString(overrideText).Y + 4;
    }

    public virtual string[] ModifyItemBuffs(string[] buffs) => buffs;

    public virtual Point getExtraSpaceNeededForTooltipSpecialIcons(
      SpriteFont font,
      int minWidth,
      int horizontalBuffer,
      int startingHeight,
      StringBuilder descriptionText,
      string boldTitleText,
      int moneyAmountToDisplayAtBottom)
    {
      return Point.Zero;
    }

    public bool ShouldDrawIcon() => true;

    public abstract void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow);

    public void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber)
    {
      this.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, Color.White, true);
    }

    public void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth)
    {
      this.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, StackDrawType.Draw, Color.White, true);
    }

    public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize) => this.drawInMenu(spriteBatch, location, scaleSize, 1f, 0.9f, StackDrawType.Draw, Color.White, true);

    public abstract int maximumStackSize();

    public abstract int addToStack(Item stack);

    public abstract string getDescription();

    public abstract bool isPlaceable();

    public virtual int salePrice() => -1;

    public virtual bool canBeTrashed()
    {
      if (this.specialItem)
        return false;
      return !(this is Tool) || this is MeleeWeapon && !(this as MeleeWeapon).isScythe() || this is FishingRod || this is Pan || this is Slingshot;
    }

    public virtual bool canBePlacedInWater() => false;

    public virtual bool actionWhenPurchased()
    {
      if (!this.isLostItem)
        return false;
      Game1.player.itemsLostLastDeath.Clear();
      this.isLostItem = false;
      Game1.player.recoveredItem = this;
      Game1.player.mailReceived.Remove("MarlonRecovery");
      Game1.addMailForTomorrow("MarlonRecovery");
      Game1.playSound("newArtifact");
      Game1.exitActiveMenu();
      bool flag = this.Stack > 1;
      Game1.drawDialogue(Game1.getCharacterFromName("Marlon"), Game1.content.LoadString(flag ? "Strings\\StringsFromCSFiles:ItemRecovery_Engaged_Stack" : "Strings\\StringsFromCSFiles:ItemRecovery_Engaged", (object) Lexicon.makePlural(this.DisplayName, !flag)));
      return true;
    }

    public virtual bool CanBuyItem(Farmer who) => Game1.player.couldInventoryAcceptThisItem(this);

    public virtual bool canBeDropped() => true;

    public virtual void actionWhenBeingHeld(Farmer who)
    {
    }

    public virtual void actionWhenStopBeingHeld(Farmer who)
    {
    }

    public int getRemainingStackSpace() => this.maximumStackSize() - this.Stack;

    public virtual int healthRecoveredOnConsumption() => 0;

    public virtual int staminaRecoveredOnConsumption() => 0;

    public virtual string getHoverBoxText(Item hoveredItem) => (string) null;

    public virtual bool canBeGivenAsGift() => false;

    public virtual void drawAttachments(SpriteBatch b, int x, int y)
    {
    }

    public virtual bool canBePlacedHere(GameLocation l, Vector2 tile) => false;

    public virtual int attachmentSlots() => 0;

    public virtual string getCategoryName() => this is Boots ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Item.cs.3829") : "";

    public virtual Color getCategoryColor() => Color.Black;

    public virtual bool canStackWith(ISalable other) => other != null && (other is Object && this is Object || other is ColoredObject && this is ColoredObject) && !((other as Object).orderData.Value != (this as Object).orderData.Value) && (!(this is ColoredObject) || !(other is ColoredObject) || (this as ColoredObject).color.Value.Equals((other as ColoredObject).color.Value)) && this.maximumStackSize() > 1 && other.maximumStackSize() > 1 && (this as Object).ParentSheetIndex == (other as Object).ParentSheetIndex && (this as Object).bigCraftable.Value == (other as Object).bigCraftable.Value && (this as Object).quality.Value == (other as Object).quality.Value && this.Name.Equals(other.Name);

    public virtual string checkForSpecialItemHoldUpMeessage() => (string) null;

    public abstract string DisplayName { get; set; }

    public virtual string Name
    {
      get => this.netName.Value;
      set => this.netName.Value = value;
    }

    public abstract int Stack { get; set; }

    public abstract Item getOne();

    public virtual void _GetOneFrom(Item source)
    {
      this.modData.Clear();
      foreach (string key in source.modData.Keys)
        this.modData[key] = source.modData[key];
    }

    public ISalable GetSalableInstance() => (ISalable) this.getOne();

    public virtual int CompareTo(object obj)
    {
      if (!(obj is Item))
        return 0;
      if ((obj as Item).Category != this.Category)
        return (obj as Item).getCategorySortValue() - this.getCategorySortValue();
      if ((obj as Item).Name.Equals(this.Name) && (obj as Item).ParentSheetIndex == this.ParentSheetIndex)
      {
        if (obj is Object && this is Object)
        {
          int num = (obj as Object).Quality.CompareTo((this as Object).Quality);
          if (num != 0)
            return num;
        }
        return this.Stack - (obj as Item).Stack;
      }
      return this is Object && obj is Object ? string.Compare((string) (NetFieldBase<string, NetString>) (this as Object).type + this.Name, (string) (NetFieldBase<string, NetString>) (obj as Object).type + (obj as Item).Name) : string.Compare(this.Name, (obj as Item).Name);
    }

    public int getCategorySortValue() => this.Category == -100 ? -94 : this.Category;

    protected virtual int getDescriptionWidth()
    {
      int val1 = 272;
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
        val1 = 384;
      return Math.Max(val1, (int) Game1.dialogueFont.MeasureString(this.DisplayName == null ? "" : this.DisplayName).X);
    }

    public virtual void resetState()
    {
    }
  }
}
