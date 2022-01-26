// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.Bundle
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class Bundle : ClickableComponent
  {
    public const float shakeRate = 0.01570796f;
    public const float shakeDecayRate = 0.003067962f;
    public const int Color_Green = 0;
    public const int Color_Purple = 1;
    public const int Color_Orange = 2;
    public const int Color_Yellow = 3;
    public const int Color_Red = 4;
    public const int Color_Blue = 5;
    public const int Color_Teal = 6;
    public const float DefaultShakeForce = 0.07363108f;
    public string rewardDescription;
    public List<BundleIngredientDescription> ingredients;
    public int bundleColor;
    public int numberOfIngredientSlots;
    public int bundleIndex;
    public int completionTimer;
    public bool complete;
    public bool depositsAllowed = true;
    public Texture2D bundleTextureOverride;
    public int bundleTextureIndexOverride = -1;
    public TemporaryAnimatedSprite sprite;
    private float maxShake;
    private bool shakeLeft;

    public Bundle(
      int bundleIndex,
      string rawBundleInfo,
      bool[] completedIngredientsList,
      Point position,
      string textureName,
      JunimoNoteMenu menu)
      : base(new Rectangle(position.X, position.Y, 64, 64), "")
    {
      if (menu.fromGameMenu)
        this.depositsAllowed = false;
      this.bundleIndex = bundleIndex;
      string[] strArray1 = rawBundleInfo.Split('/');
      this.name = strArray1[0];
      this.label = strArray1[0];
      if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
        this.label = strArray1[strArray1.Length - 1];
      this.rewardDescription = strArray1[1];
      if (strArray1.Length > 5 && (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en || strArray1.Length > 6))
      {
        string s = strArray1[5];
        try
        {
          if (s.IndexOf(':') >= 0)
          {
            string[] strArray2 = s.Split(':');
            this.bundleTextureOverride = Game1.content.Load<Texture2D>(strArray2[0]);
            this.bundleTextureIndexOverride = int.Parse(strArray2[1]);
          }
          else
            this.bundleTextureIndexOverride = int.Parse(s);
        }
        catch (Exception ex)
        {
          this.bundleTextureOverride = (Texture2D) null;
          this.bundleTextureIndexOverride = -1;
        }
      }
      string[] strArray3 = strArray1[2].Split(' ');
      this.complete = true;
      this.ingredients = new List<BundleIngredientDescription>();
      int num1 = 0;
      for (int index = 0; index < strArray3.Length; index += 3)
      {
        this.ingredients.Add(new BundleIngredientDescription(Convert.ToInt32(strArray3[index]), Convert.ToInt32(strArray3[index + 1]), Convert.ToInt32(strArray3[index + 2]), completedIngredientsList[index / 3]));
        if (!completedIngredientsList[index / 3])
          this.complete = false;
        else
          ++num1;
      }
      this.bundleColor = Convert.ToInt32(strArray1[3]);
      int num2 = 4;
      if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
        num2 = 5;
      this.numberOfIngredientSlots = strArray1.Length > num2 ? Convert.ToInt32(strArray1[4]) : this.ingredients.Count<BundleIngredientDescription>();
      if (num1 >= this.numberOfIngredientSlots)
        this.complete = true;
      this.sprite = new TemporaryAnimatedSprite(textureName, new Rectangle(this.bundleColor * 256 % 512, 244 + this.bundleColor * 256 / 512 * 16, 16, 16), 70f, 3, 99999, new Vector2((float) this.bounds.X, (float) this.bounds.Y), false, false, 0.8f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        pingPong = true
      };
      this.sprite.paused = true;
      this.sprite.sourceRect.X += this.sprite.sourceRect.Width;
      if (this.name.ToLower().Contains(Game1.currentSeason) && !this.complete)
        this.shake();
      if (!this.complete)
        return;
      this.completionAnimation(menu, false);
    }

    public Item getReward() => Utility.getItemFromStandardTextDescription(this.rewardDescription, Game1.player);

    public void shake(float force = 0.07363108f)
    {
      if (!this.sprite.paused)
        return;
      this.maxShake = force;
    }

    public void shake(int extraInfo)
    {
      this.maxShake = 3f * (float) Math.PI / 128f;
      if (extraInfo != 1)
        return;
      Game1.playSound("leafrustle");
      JunimoNoteMenu.tempSprites.Add(new TemporaryAnimatedSprite(50, this.sprite.position, Bundle.getColorFromColorIndex(this.bundleColor))
      {
        motion = new Vector2(-1f, 0.5f),
        acceleration = new Vector2(0.0f, 0.02f)
      });
      ++JunimoNoteMenu.tempSprites.Last<TemporaryAnimatedSprite>().sourceRect.Y;
      --JunimoNoteMenu.tempSprites.Last<TemporaryAnimatedSprite>().sourceRect.Height;
      JunimoNoteMenu.tempSprites.Add(new TemporaryAnimatedSprite(50, this.sprite.position, Bundle.getColorFromColorIndex(this.bundleColor))
      {
        motion = new Vector2(1f, 0.5f),
        acceleration = new Vector2(0.0f, 0.02f),
        flipped = true,
        delayBeforeAnimationStart = 50
      });
      ++JunimoNoteMenu.tempSprites.Last<TemporaryAnimatedSprite>().sourceRect.Y;
      --JunimoNoteMenu.tempSprites.Last<TemporaryAnimatedSprite>().sourceRect.Height;
    }

    public void shakeAndAllowClicking(int extraInfo)
    {
      this.maxShake = 3f * (float) Math.PI / 128f;
      JunimoNoteMenu.canClick = true;
    }

    public void tryHoverAction(int x, int y)
    {
      if (this.bounds.Contains(x, y) && !this.complete)
      {
        this.sprite.paused = false;
        JunimoNoteMenu.hoverText = Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", (object) this.label);
      }
      else
      {
        if (this.complete)
          return;
        this.sprite.reset();
        this.sprite.sourceRect.X += this.sprite.sourceRect.Width;
        this.sprite.paused = true;
      }
    }

    public bool IsValidItemForThisIngredientDescription(
      Item item,
      BundleIngredientDescription ingredient)
    {
      if (!(item is StardewValley.Object))
        return false;
      StardewValley.Object @object = item as StardewValley.Object;
      if (!ingredient.completed && ingredient.quality <= (int) (NetFieldBase<int, NetInt>) @object.quality)
      {
        if (ingredient.index < 0)
        {
          if (item.Category == ingredient.index)
            return true;
        }
        else if (ingredient.index == (int) (NetFieldBase<int, NetInt>) item.parentSheetIndex)
          return true;
      }
      return false;
    }

    public int GetBundleIngredientDescriptionIndexForItem(Item item)
    {
      if (!(item is StardewValley.Object))
        return -1;
      StardewValley.Object @object = item as StardewValley.Object;
      for (int index = 0; index < this.ingredients.Count; ++index)
      {
        if (this.IsValidItemForThisIngredientDescription((Item) @object, this.ingredients[index]))
          return index;
      }
      return -1;
    }

    public bool canAcceptThisItem(Item item, ClickableTextureComponent slot) => this.canAcceptThisItem(item, slot, false);

    public bool canAcceptThisItem(
      Item item,
      ClickableTextureComponent slot,
      bool ignore_stack_count = false)
    {
      if (!this.depositsAllowed || !(item is StardewValley.Object))
        return false;
      StardewValley.Object @object = item as StardewValley.Object;
      for (int index = 0; index < this.ingredients.Count; ++index)
      {
        if (this.IsValidItemForThisIngredientDescription((Item) @object, this.ingredients[index]) && (ignore_stack_count || this.ingredients[index].stack <= item.Stack) && (slot == null || slot.item == null))
          return true;
      }
      return false;
    }

    public Item tryToDepositThisItem(
      Item item,
      ClickableTextureComponent slot,
      string noteTextureName)
    {
      if (!this.depositsAllowed)
      {
        if (Game1.player.hasCompletedCommunityCenter())
          Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtAJM"));
        else
          Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtCC"));
        return item;
      }
      if (!(item is StardewValley.Object) || item is Furniture)
        return item;
      for (int index = 0; index < this.ingredients.Count; ++index)
      {
        if (this.IsValidItemForThisIngredientDescription(item, this.ingredients[index]) && slot.item == null)
        {
          item.Stack -= this.ingredients[index].stack;
          this.ingredients[index] = new BundleIngredientDescription(this.ingredients[index].index, this.ingredients[index].stack, this.ingredients[index].quality, true);
          this.ingredientDepositAnimation(slot, noteTextureName);
          int num = this.ingredients[index].index;
          if (num < 0)
            num = JunimoNoteMenu.GetObjectOrCategoryIndex(num);
          slot.item = (Item) new StardewValley.Object(num, this.ingredients[index].stack, quality: this.ingredients[index].quality);
          Game1.playSound("newArtifact");
          (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundles.FieldDict[this.bundleIndex][index] = true;
          slot.sourceRect.X = 512;
          slot.sourceRect.Y = 244;
          Game1.multiplayer.globalChatInfoMessage("BundleDonate", Game1.player.displayName, slot.item.DisplayName);
          break;
        }
      }
      return item.Stack > 0 ? item : (Item) null;
    }

    public bool couldThisItemBeDeposited(Item item)
    {
      if (!(item is StardewValley.Object) || item is Furniture)
        return false;
      for (int index = 0; index < this.ingredients.Count; ++index)
      {
        if (!this.IsValidItemForThisIngredientDescription(item, this.ingredients[index]))
          return true;
      }
      return false;
    }

    public void ingredientDepositAnimation(
      ClickableTextureComponent slot,
      string noteTextureName,
      bool skipAnimation = false)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(noteTextureName, new Rectangle(530, 244, 18, 18), 50f, 6, 1, new Vector2((float) slot.bounds.X, (float) slot.bounds.Y), false, false, 0.88f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        holdLastFrame = true,
        endSound = "cowboy_monsterhit"
      };
      if (skipAnimation)
      {
        temporaryAnimatedSprite.sourceRect.Offset(temporaryAnimatedSprite.sourceRect.Width * 5, 0);
        temporaryAnimatedSprite.sourceRectStartingPos = new Vector2((float) temporaryAnimatedSprite.sourceRect.X, (float) temporaryAnimatedSprite.sourceRect.Y);
        temporaryAnimatedSprite.animationLength = 1;
      }
      JunimoNoteMenu.tempSprites.Add(temporaryAnimatedSprite);
    }

    public bool canBeClicked() => !this.complete;

    public void completionAnimation(JunimoNoteMenu menu, bool playSound = true, int delay = 0)
    {
      if (delay <= 0)
        this.completionAnimation(playSound);
      else
        this.completionTimer = delay;
    }

    private void completionAnimation(bool playSound = true)
    {
      if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is JunimoNoteMenu)
        (Game1.activeClickableMenu as JunimoNoteMenu).takeDownBundleSpecificPage();
      this.sprite.pingPong = false;
      this.sprite.paused = false;
      this.sprite.sourceRect.X = (int) this.sprite.sourceRectStartingPos.X;
      this.sprite.sourceRect.X += this.sprite.sourceRect.Width;
      this.sprite.animationLength = 15;
      this.sprite.interval = 50f;
      this.sprite.totalNumberOfLoops = 0;
      this.sprite.holdLastFrame = true;
      this.sprite.endFunction = new TemporaryAnimatedSprite.endBehavior(this.shake);
      this.sprite.extraInfoForEndBehavior = 1;
      if (this.complete)
      {
        this.sprite.sourceRect.X += this.sprite.sourceRect.Width * 14;
        this.sprite.sourceRectStartingPos = new Vector2((float) this.sprite.sourceRect.X, (float) this.sprite.sourceRect.Y);
        this.sprite.currentParentTileIndex = 14;
        this.sprite.interval = 0.0f;
        this.sprite.animationLength = 1;
        this.sprite.extraInfoForEndBehavior = 0;
      }
      else
      {
        if (playSound)
          Game1.playSound("dwop");
        this.bounds.Inflate(64, 64);
        JunimoNoteMenu.tempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(this.bounds, 8, Bundle.getColorFromColorIndex(this.bundleColor) * 0.5f));
        this.bounds.Inflate(-64, -64);
      }
      this.complete = true;
    }

    public void update(GameTime time)
    {
      this.sprite.update(time);
      if (this.completionTimer > 0 && JunimoNoteMenu.screenSwipe == null)
      {
        this.completionTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.completionTimer <= 0)
          this.completionAnimation();
      }
      if (Game1.random.NextDouble() < 0.005 && (this.complete || this.name.ToLower().Contains(Game1.currentSeason)))
        this.shake();
      if ((double) this.maxShake > 0.0)
      {
        if (this.shakeLeft)
        {
          this.sprite.rotation -= (float) Math.PI / 200f;
          if ((double) this.sprite.rotation <= -(double) this.maxShake)
            this.shakeLeft = false;
        }
        else
        {
          this.sprite.rotation += (float) Math.PI / 200f;
          if ((double) this.sprite.rotation >= (double) this.maxShake)
            this.shakeLeft = true;
        }
      }
      if ((double) this.maxShake <= 0.0)
        return;
      this.maxShake = Math.Max(0.0f, this.maxShake - 0.0007669904f);
    }

    public void draw(SpriteBatch b) => this.sprite.draw(b, true);

    public static Color getColorFromColorIndex(int color)
    {
      switch (color)
      {
        case 0:
          return Color.Lime;
        case 1:
          return Color.DeepPink;
        case 2:
          return Color.Orange;
        case 3:
          return Color.Orange;
        case 4:
          return Color.Red;
        case 5:
          return Color.LightBlue;
        case 6:
          return Color.Cyan;
        default:
          return Color.Lime;
      }
    }
  }
}
