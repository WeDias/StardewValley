// Decompiled with JetBrains decompiler
// Type: StardewValley.CraftingRecipe
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley
{
  public class CraftingRecipe
  {
    public const int wild_seed_special_category = -777;
    public string name;
    public string DisplayName;
    public string description;
    public static Dictionary<string, string> craftingRecipes;
    public static Dictionary<string, string> cookingRecipes;
    public Dictionary<int, int> recipeList = new Dictionary<int, int>();
    public List<int> itemToProduce = new List<int>();
    public bool bigCraftable;
    public bool isCookingRecipe;
    public int timesCrafted;
    public int numberProducedPerCraft;
    public string itemType;

    public string ItemType
    {
      get
      {
        if (this.itemType != null && !(this.itemType == ""))
          return this.itemType;
        return !this.bigCraftable ? "O" : "BO";
      }
    }

    public static void InitShared()
    {
      CraftingRecipe.craftingRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
      CraftingRecipe.cookingRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
    }

    public CraftingRecipe(string name)
      : this(name, CraftingRecipe.cookingRecipes.ContainsKey(name))
    {
    }

    public CraftingRecipe(string name, bool isCookingRecipe)
    {
      this.isCookingRecipe = isCookingRecipe;
      this.name = name;
      string str = !isCookingRecipe || !CraftingRecipe.cookingRecipes.ContainsKey(name) ? (CraftingRecipe.craftingRecipes.ContainsKey(name) ? CraftingRecipe.craftingRecipes[name] : (string) null) : CraftingRecipe.cookingRecipes[name];
      if (str == null)
      {
        this.name = "Torch";
        name = "Torch";
        str = CraftingRecipe.craftingRecipes[name];
      }
      string[] strArray1 = str.Split('/');
      string[] strArray2 = strArray1[0].Split(' ');
      for (int index = 0; index < strArray2.Length; index += 2)
        this.recipeList.Add(Convert.ToInt32(strArray2[index]), Convert.ToInt32(strArray2[index + 1]));
      string[] strArray3 = strArray1[2].Split(' ');
      for (int index = 0; index < strArray3.Length; index += 2)
      {
        this.itemToProduce.Add(Convert.ToInt32(strArray3[index]));
        this.numberProducedPerCraft = strArray3.Length > 1 ? Convert.ToInt32(strArray3[index + 1]) : 1;
      }
      if (!isCookingRecipe)
      {
        if (strArray1[3] == "true")
        {
          this.itemType = "BO";
          this.bigCraftable = true;
        }
        else
          this.itemType = !(strArray1[3] == "false") ? strArray1[3] : "O";
      }
      try
      {
        this.description = this.bigCraftable ? Game1.bigCraftablesInformation[this.itemToProduce[0]].Split('/')[4] : Game1.objectInformation[this.itemToProduce[0]].Split('/')[5];
      }
      catch (Exception ex)
      {
        this.description = "";
      }
      this.timesCrafted = Game1.player.craftingRecipes.ContainsKey(name) ? Game1.player.craftingRecipes[name] : 0;
      if (name.Equals("Crab Pot") && Game1.player.professions.Contains(7))
      {
        this.recipeList = new Dictionary<int, int>();
        this.recipeList.Add(388, 25);
        this.recipeList.Add(334, 2);
      }
      if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
        this.DisplayName = strArray1[strArray1.Length - 1];
      else
        this.DisplayName = name;
    }

    public int getIndexOfMenuView() => this.itemToProduce.Count <= 0 ? -1 : this.itemToProduce[0];

    public virtual bool doesFarmerHaveIngredientsInInventory(IList<Item> extraToCheck = null)
    {
      foreach (KeyValuePair<int, int> recipe in this.recipeList)
      {
        int num = recipe.Value - Game1.player.getItemCount(recipe.Key, 5);
        if (num > 0 && (extraToCheck == null || num - Game1.player.getItemCountInList(extraToCheck, recipe.Key, 5) > 0))
          return false;
      }
      return true;
    }

    public virtual void drawMenuView(SpriteBatch b, int x, int y, float layerDepth = 0.88f, bool shadow = true)
    {
      if (this.bigCraftable)
        Utility.drawWithShadow(b, Game1.bigCraftableSpriteSheet, new Vector2((float) x, (float) y), Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.getIndexOfMenuView(), 16, 32), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: layerDepth);
      else
        Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float) x, (float) y), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIndexOfMenuView(), 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: layerDepth);
    }

    public virtual Item createItem()
    {
      int num = this.itemToProduce.ElementAt<int>(Game1.random.Next(this.itemToProduce.Count));
      if (this.bigCraftable)
        return this.name.Equals("Chest") ? (Item) new Chest(true) : (Item) new Object(Vector2.Zero, num);
      if (this.name.Equals("Torch"))
        return (Item) new Torch(Vector2.Zero, this.numberProducedPerCraft);
      if (num >= 516 && num <= 534 || num == 801)
        return (Item) new Ring(num);
      Item standardTextDescription = Utility.getItemFromStandardTextDescription(this.ItemType + " " + num.ToString() + " " + this.numberProducedPerCraft.ToString(), Game1.player);
      if (this.isCookingRecipe && standardTextDescription is Object && Game1.player.team.SpecialOrderRuleActive("QI_COOKING"))
      {
        (standardTextDescription as Object).orderData.Value = "QI_COOKING";
        standardTextDescription.MarkContextTagsDirty();
      }
      return standardTextDescription;
    }

    public static bool isThereSpecialIngredientRule(
      Object potentialIngredient,
      int requiredIngredient)
    {
      return requiredIngredient == -777 && ((int) (NetFieldBase<int, NetInt>) potentialIngredient.parentSheetIndex == 495 || (int) (NetFieldBase<int, NetInt>) potentialIngredient.parentSheetIndex == 496 || (int) (NetFieldBase<int, NetInt>) potentialIngredient.parentSheetIndex == 497 || (int) (NetFieldBase<int, NetInt>) potentialIngredient.parentSheetIndex == 498);
    }

    public void consumeIngredients(List<Chest> additional_materials)
    {
      for (int index1 = this.recipeList.Count - 1; index1 >= 0; --index1)
      {
        int recipe = this.recipeList[this.recipeList.Keys.ElementAt<int>(index1)];
        bool flag1 = false;
        for (int index2 = Game1.player.items.Count - 1; index2 >= 0; --index2)
        {
          if (Game1.player.items[index2] != null && Game1.player.items[index2] is Object && !(bool) (NetFieldBase<bool, NetBool>) (Game1.player.items[index2] as Object).bigCraftable && ((int) (NetFieldBase<int, NetInt>) Game1.player.items[index2].parentSheetIndex == this.recipeList.Keys.ElementAt<int>(index1) || Game1.player.items[index2].Category == this.recipeList.Keys.ElementAt<int>(index1) || CraftingRecipe.isThereSpecialIngredientRule((Object) Game1.player.items[index2], this.recipeList.Keys.ElementAt<int>(index1))))
          {
            int num = recipe;
            recipe -= Game1.player.items[index2].Stack;
            Game1.player.items[index2].Stack -= num;
            if (Game1.player.items[index2].Stack <= 0)
              Game1.player.items[index2] = (Item) null;
            if (recipe <= 0)
            {
              flag1 = true;
              break;
            }
          }
        }
        if (additional_materials != null && !flag1)
        {
          for (int index3 = 0; index3 < additional_materials.Count; ++index3)
          {
            Chest additionalMaterial = additional_materials[index3];
            if (additionalMaterial != null)
            {
              bool flag2 = false;
              for (int index4 = additionalMaterial.items.Count - 1; index4 >= 0; --index4)
              {
                if (additionalMaterial.items[index4] != null && additionalMaterial.items[index4] is Object && ((int) (NetFieldBase<int, NetInt>) additionalMaterial.items[index4].parentSheetIndex == this.recipeList.Keys.ElementAt<int>(index1) || additionalMaterial.items[index4].Category == this.recipeList.Keys.ElementAt<int>(index1) || CraftingRecipe.isThereSpecialIngredientRule((Object) additionalMaterial.items[index4], this.recipeList.Keys.ElementAt<int>(index1))))
                {
                  int num = Math.Min(recipe, additionalMaterial.items[index4].Stack);
                  recipe -= num;
                  additionalMaterial.items[index4].Stack -= num;
                  if (additionalMaterial.items[index4].Stack <= 0)
                  {
                    additionalMaterial.items[index4] = (Item) null;
                    flag2 = true;
                  }
                  if (recipe <= 0)
                    break;
                }
              }
              if (flag2)
                additionalMaterial.clearNulls();
              if (recipe <= 0)
                break;
            }
          }
        }
      }
    }

    public static bool DoesFarmerHaveAdditionalIngredientsInInventory(
      List<KeyValuePair<int, int>> additional_recipe_items,
      IList<Item> extraToCheck = null)
    {
      foreach (KeyValuePair<int, int> additionalRecipeItem in additional_recipe_items)
      {
        int num = additionalRecipeItem.Value - Game1.player.getItemCount(additionalRecipeItem.Key, 5);
        if (num > 0 && (extraToCheck == null || num - Game1.player.getItemCountInList(extraToCheck, additionalRecipeItem.Key, 5) > 0))
          return false;
      }
      return true;
    }

    public static void ConsumeAdditionalIngredients(
      List<KeyValuePair<int, int>> additional_recipe_items,
      List<Chest> additional_materials)
    {
      for (int index1 = additional_recipe_items.Count - 1; index1 >= 0; --index1)
      {
        KeyValuePair<int, int> additionalRecipeItem = additional_recipe_items[index1];
        int key = additionalRecipeItem.Key;
        additionalRecipeItem = additional_recipe_items[index1];
        int val1 = additionalRecipeItem.Value;
        bool flag1 = false;
        for (int index2 = Game1.player.items.Count - 1; index2 >= 0; --index2)
        {
          if (Game1.player.items[index2] != null && Game1.player.items[index2] is Object && !(bool) (NetFieldBase<bool, NetBool>) (Game1.player.items[index2] as Object).bigCraftable && ((int) (NetFieldBase<int, NetInt>) Game1.player.items[index2].parentSheetIndex == key || Game1.player.items[index2].Category == key || CraftingRecipe.isThereSpecialIngredientRule((Object) Game1.player.items[index2], key)))
          {
            int num = val1;
            val1 -= Game1.player.items[index2].Stack;
            Game1.player.items[index2].Stack -= num;
            if (Game1.player.items[index2].Stack <= 0)
              Game1.player.items[index2] = (Item) null;
            if (val1 <= 0)
            {
              flag1 = true;
              break;
            }
          }
        }
        if (additional_materials != null && !flag1)
        {
          for (int index3 = 0; index3 < additional_materials.Count; ++index3)
          {
            Chest additionalMaterial = additional_materials[index3];
            if (additionalMaterial != null)
            {
              bool flag2 = false;
              for (int index4 = additionalMaterial.items.Count - 1; index4 >= 0; --index4)
              {
                if (additionalMaterial.items[index4] != null && additionalMaterial.items[index4] is Object && ((int) (NetFieldBase<int, NetInt>) additionalMaterial.items[index4].parentSheetIndex == key || additionalMaterial.items[index4].Category == key || CraftingRecipe.isThereSpecialIngredientRule((Object) additionalMaterial.items[index4], key)))
                {
                  int num = Math.Min(val1, additionalMaterial.items[index4].Stack);
                  val1 -= num;
                  additionalMaterial.items[index4].Stack -= num;
                  if (additionalMaterial.items[index4].Stack <= 0)
                  {
                    additionalMaterial.items[index4] = (Item) null;
                    flag2 = true;
                  }
                  if (val1 <= 0)
                    break;
                }
              }
              if (flag2)
                additionalMaterial.clearNulls();
              if (val1 <= 0)
                break;
            }
          }
        }
      }
    }

    public virtual int getCraftableCount(IList<Chest> additional_material_chests)
    {
      List<Item> additional_materials = new List<Item>();
      if (additional_material_chests != null)
      {
        for (int index = 0; index < additional_material_chests.Count; ++index)
          additional_materials.AddRange((IEnumerable<Item>) additional_material_chests[index].items);
      }
      return this.getCraftableCount((IList<Item>) additional_materials);
    }

    public int getCraftableCount(IList<Item> additional_materials)
    {
      int craftableCount = -1;
      for (int index1 = this.recipeList.Count - 1; index1 >= 0; --index1)
      {
        int num1 = 0;
        int recipe = this.recipeList[this.recipeList.Keys.ElementAt<int>(index1)];
        for (int index2 = Game1.player.items.Count - 1; index2 >= 0; --index2)
        {
          if (Game1.player.items[index2] != null && Game1.player.items[index2] is Object && !(bool) (NetFieldBase<bool, NetBool>) (Game1.player.items[index2] as Object).bigCraftable && ((int) (NetFieldBase<int, NetInt>) Game1.player.items[index2].parentSheetIndex == this.recipeList.Keys.ElementAt<int>(index1) || Game1.player.items[index2].Category == this.recipeList.Keys.ElementAt<int>(index1) || CraftingRecipe.isThereSpecialIngredientRule((Object) Game1.player.items[index2], this.recipeList.Keys.ElementAt<int>(index1))))
            num1 += Game1.player.items[index2].Stack;
        }
        if (additional_materials != null)
        {
          for (int index3 = 0; index3 < additional_materials.Count; ++index3)
          {
            Item additionalMaterial = additional_materials[index3];
            if (additionalMaterial != null && additionalMaterial is Object && (Utility.IsNormalObjectAtParentSheetIndex(additionalMaterial, additionalMaterial.ParentSheetIndex) && (int) (NetFieldBase<int, NetInt>) additionalMaterial.parentSheetIndex == this.recipeList.Keys.ElementAt<int>(index1) || additionalMaterial.Category == this.recipeList.Keys.ElementAt<int>(index1) || CraftingRecipe.isThereSpecialIngredientRule((Object) additionalMaterial, this.recipeList.Keys.ElementAt<int>(index1))))
              num1 += additionalMaterial.Stack;
          }
        }
        int num2 = num1 / recipe;
        if (num2 < craftableCount || craftableCount == -1)
          craftableCount = num2;
      }
      return craftableCount;
    }

    public virtual string getCraftCountText()
    {
      if (this.isCookingRecipe)
      {
        if (Game1.player.recipesCooked.ContainsKey(this.getIndexOfMenuView()) && Game1.player.recipesCooked[this.getIndexOfMenuView()] > 0)
          return Game1.content.LoadString("Strings\\UI:Collections_Description_RecipesCooked", (object) Game1.player.recipesCooked[this.getIndexOfMenuView()]);
      }
      else if (Game1.player.craftingRecipes.ContainsKey(this.name) && Game1.player.craftingRecipes[this.name] > 0)
        return Game1.content.LoadString("Strings\\UI:Crafting_NumberCrafted", (object) Game1.player.craftingRecipes[this.name]);
      return (string) null;
    }

    public int getDescriptionHeight(int width) => (int) ((double) Game1.smallFont.MeasureString(Game1.parseText(this.description, Game1.smallFont, width)).Y + (double) (this.getNumberOfIngredients() * 36) + (double) (int) Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567")).Y + 21.0);

    public virtual void drawRecipeDescription(
      SpriteBatch b,
      Vector2 position,
      int width,
      IList<Item> additional_crafting_items)
    {
      int num1 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 8 : 0;
      b.Draw(Game1.staminaRect, new Rectangle((int) ((double) position.X + 8.0), (int) ((double) position.Y + 32.0 + (double) Game1.smallFont.MeasureString("Ing!").Y) - 4 - 2 - (int) ((double) num1 * 1.5), width - 32, 2), Game1.textColor * 0.35f);
      Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"), Game1.smallFont, position + new Vector2(8f, 28f), Game1.textColor * 0.75f);
      for (int index = 0; index < this.recipeList.Count; ++index)
      {
        int num2 = this.recipeList.Values.ElementAt<int>(index);
        int item_index = this.recipeList.Keys.ElementAt<int>(index);
        int itemCount = Game1.player.getItemCount(item_index, 8);
        int num3 = 0;
        int num4 = num2 - itemCount;
        if (additional_crafting_items != null)
        {
          num3 = Game1.player.getItemCountInList(additional_crafting_items, item_index, 8);
          if (num4 > 0)
            num4 -= num3;
        }
        string nameFromIndex = this.getNameFromIndex(this.recipeList.Keys.ElementAt<int>(index));
        Color color1 = num4 <= 0 ? Game1.textColor : Color.Red;
        b.Draw(Game1.objectSpriteSheet, new Vector2(position.X, position.Y + 64f + (float) (index * 64 / 2) + (float) (index * 4)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getSpriteIndexFromRawIndex(this.recipeList.Keys.ElementAt<int>(index)), 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
        int toDraw = this.recipeList.Values.ElementAt<int>(index);
        SpriteBatch b1 = b;
        double num5 = (double) position.X + 32.0;
        SpriteFont tinyFont = Game1.tinyFont;
        int num6 = this.recipeList.Values.ElementAt<int>(index);
        string text1 = num6.ToString() ?? "";
        double x = (double) tinyFont.MeasureString(text1).X;
        Vector2 position1 = new Vector2((float) (num5 - x), (float) ((double) position.Y + 64.0 + (double) (index * 64 / 2) + (double) (index * 4) + 21.0));
        Color antiqueWhite = Color.AntiqueWhite;
        Utility.drawTinyDigits(toDraw, b1, position1, 2f, 0.87f, antiqueWhite);
        Vector2 position2 = new Vector2((float) ((double) position.X + 32.0 + 8.0), (float) ((double) position.Y + 64.0 + (double) (index * 64 / 2) + (double) (index * 4) + 4.0));
        Utility.drawTextWithShadow(b, nameFromIndex, Game1.smallFont, position2, color1);
        if (Game1.options.showAdvancedCraftingInformation)
        {
          position2.X = (float) ((double) position.X + (double) width - 40.0);
          b.Draw(Game1.mouseCursors, new Rectangle((int) position2.X, (int) position2.Y + 2, 22, 26), new Rectangle?(new Rectangle(268, 1436, 11, 13)), Color.White);
          SpriteBatch b2 = b;
          num6 = itemCount + num3;
          string text2 = num6.ToString() ?? "";
          SpriteFont smallFont1 = Game1.smallFont;
          Vector2 vector2_1 = position2;
          SpriteFont smallFont2 = Game1.smallFont;
          num6 = itemCount + num3;
          string text3 = num6.ToString() + " ";
          Vector2 vector2_2 = new Vector2(smallFont2.MeasureString(text3).X, 0.0f);
          Vector2 position3 = vector2_1 - vector2_2;
          Color color2 = color1;
          Utility.drawTextWithShadow(b2, text2, smallFont1, position3, color2);
        }
      }
      b.Draw(Game1.staminaRect, new Rectangle((int) position.X + 8, (int) position.Y + num1 + 64 + 4 + this.recipeList.Count * 36, width - 32, 2), Game1.textColor * 0.35f);
      Utility.drawTextWithShadow(b, Game1.parseText(this.description, Game1.smallFont, width - 8), Game1.smallFont, position + new Vector2(0.0f, (float) (76 + this.recipeList.Count * 36 + num1)), Game1.textColor * 0.75f);
    }

    public virtual int getNumberOfIngredients() => this.recipeList.Count;

    public int getSpriteIndexFromRawIndex(int index)
    {
      switch (index)
      {
        case -777:
          return 495;
        case -6:
          return 184;
        case -5:
          return 176;
        case -4:
          return 145;
        case -3:
          return 24;
        case -2:
          return 80;
        case -1:
          return 20;
        default:
          return index;
      }
    }

    public string getNameFromIndex(int index)
    {
      if (index < 0)
      {
        switch (index)
        {
          case -777:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.574");
          case -6:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
          case -5:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
          case -4:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
          case -3:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");
          case -2:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
          case -1:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568");
          default:
            return "???";
        }
      }
      else
      {
        string nameFromIndex = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.575");
        if (Game1.objectInformation.ContainsKey(index))
          nameFromIndex = Game1.objectInformation[index].Split('/')[4];
        return nameFromIndex;
      }
    }
  }
}
