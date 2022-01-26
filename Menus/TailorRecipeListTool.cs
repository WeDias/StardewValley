// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TailorRecipeListTool
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.GameData.Crafting;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class TailorRecipeListTool : IClickableMenu
  {
    public Rectangle scrollView;
    public List<ClickableTextureComponent> recipeComponents;
    public ClickableTextureComponent okButton;
    public float scrollY;
    public Dictionary<string, KeyValuePair<Item, Item>> _recipeLookup;
    public Item hoveredItem;
    public string metadata = "";
    public Dictionary<string, string> _recipeMetadata;
    public Dictionary<string, Color> _recipeColors;

    public TailorRecipeListTool()
      : base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + 64)
    {
      TailoringMenu tailoringMenu = new TailoringMenu();
      Game1.player.faceDirection(2);
      Game1.player.FarmerSprite.StopAnimation();
      this.recipeComponents = new List<ClickableTextureComponent>();
      this._recipeLookup = new Dictionary<string, KeyValuePair<Item, Item>>();
      this._recipeMetadata = new Dictionary<string, string>();
      this._recipeColors = new Dictionary<string, Color>();
      Item left_item = (Item) new StardewValley.Object(Vector2.Zero, 428, 1);
      foreach (int key in (IEnumerable<int>) Game1.objectInformation.Keys)
      {
        Item obj1 = (Item) new StardewValley.Object(Vector2.Zero, key, 1);
        if (!obj1.Name.Contains("Seeds") && !obj1.Name.Contains("Floor") && !obj1.Name.Equals("Stone") && !obj1.Name.Contains("Weeds") && !obj1.Name.Equals("Lumber") && !obj1.Name.Contains("Fence") && !obj1.Name.Equals("Gate") && !obj1.Name.Contains("Starter") && !obj1.Name.Contains("Twig") && !obj1.Name.Equals("Secret Note") && !obj1.Name.Contains("Guide") && !obj1.Name.Contains("Path") && !obj1.Name.Contains("Ring") && (int) (NetFieldBase<int, NetInt>) obj1.category != -22 && !obj1.Name.Contains("Sapling"))
        {
          Item obj2 = tailoringMenu.CraftItem(left_item, obj1);
          TailorItemRecipe recipeForItems = tailoringMenu.GetRecipeForItems(left_item, obj1);
          KeyValuePair<Item, Item> keyValuePair = new KeyValuePair<Item, Item>(obj1, obj2);
          this._recipeLookup[Utility.getStandardDescriptionFromItem(obj1, 1)] = keyValuePair;
          string str = "";
          Color? dyeColor = TailoringMenu.GetDyeColor(obj1);
          if (dyeColor.HasValue)
            this._recipeColors[Utility.getStandardDescriptionFromItem(obj1, 1)] = dyeColor.Value;
          if (recipeForItems != null)
          {
            str = "clothes id: " + recipeForItems.CraftedItemID.ToString() + " from ";
            foreach (string secondItemTag in recipeForItems.SecondItemTags)
              str = str + secondItemTag + " ";
            str.Trim();
          }
          this._recipeMetadata[Utility.getStandardDescriptionFromItem(obj1, 1)] = str;
          ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), (Texture2D) null, new Rectangle(), 1f);
          textureComponent.myID = 0;
          textureComponent.name = Utility.getStandardDescriptionFromItem(obj1, 1);
          textureComponent.label = obj1.DisplayName;
          this.recipeComponents.Add(textureComponent);
        }
      }
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent1.upNeighborID = -99998;
      textureComponent1.leftNeighborID = -99998;
      textureComponent1.rightNeighborID = -99998;
      textureComponent1.downNeighborID = -99998;
      this.okButton = textureComponent1;
      this.RepositionElements();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;
      this.RepositionElements();
    }

    private void RepositionElements()
    {
      this.scrollView = new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, this.width - IClickableMenu.borderWidth, 500);
      if (this.scrollView.Left < Game1.graphics.GraphicsDevice.ScissorRectangle.Left)
      {
        int num = Game1.graphics.GraphicsDevice.ScissorRectangle.Left - this.scrollView.Left;
        this.scrollView.X += num;
        this.scrollView.Width -= num;
      }
      if (this.scrollView.Right > Game1.graphics.GraphicsDevice.ScissorRectangle.Right)
      {
        int num = this.scrollView.Right - Game1.graphics.GraphicsDevice.ScissorRectangle.Right;
        this.scrollView.X -= num;
        this.scrollView.Width -= num;
      }
      if (this.scrollView.Top < Game1.graphics.GraphicsDevice.ScissorRectangle.Top)
      {
        int num = Game1.graphics.GraphicsDevice.ScissorRectangle.Top - this.scrollView.Top;
        this.scrollView.Y += num;
        this.scrollView.Width -= num;
      }
      if (this.scrollView.Bottom > Game1.graphics.GraphicsDevice.ScissorRectangle.Bottom)
      {
        int num = this.scrollView.Bottom - Game1.graphics.GraphicsDevice.ScissorRectangle.Bottom;
        this.scrollView.Y -= num;
        this.scrollView.Width -= num;
      }
      this.RepositionScrollElements();
    }

    public void RepositionScrollElements()
    {
      int scrollY = (int) this.scrollY;
      if ((double) this.scrollY > 0.0)
        this.scrollY = 0.0f;
      foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
      {
        recipeComponent.bounds.X = this.scrollView.X;
        recipeComponent.bounds.Y = this.scrollView.Y + scrollY;
        scrollY += recipeComponent.bounds.Height;
        if (this.scrollView.Intersects(recipeComponent.bounds))
          recipeComponent.visible = true;
        else
          recipeComponent.visible = false;
      }
    }

    public override void snapToDefaultClickableComponent() => this.snapCursorToCurrentSnappedComponent();

    public override void gamePadButtonHeld(Buttons b) => base.gamePadButtonHeld(b);

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      ClickableComponent snappedComponent = this.currentlySnappedComponent;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
      {
        if (recipeComponent.bounds.Contains(x, y))
        {
          if (this.scrollView.Contains(x, y))
          {
            try
            {
              int int32 = Convert.ToInt32(this._recipeMetadata[recipeComponent.name].Split(' ')[2]);
              if (int32 >= 2000)
              {
                Game1.player.addItemToInventoryBool((Item) new Hat(int32 - 2000));
              }
              else
              {
                Clothing clothing = new Clothing(int32);
                if (this._recipeColors.ContainsKey(recipeComponent.name))
                  clothing.Dye(this._recipeColors[recipeComponent.name], 1f);
                Game1.player.addItemToInventoryBool((Item) clothing);
              }
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      if (!this.okButton.containsPoint(x, y))
        return;
      this.exitThisMenu();
    }

    public override void leftClickHeld(int x, int y)
    {
    }

    public override void releaseLeftClick(int x, int y)
    {
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void receiveKeyPress(Keys key)
    {
    }

    public override void receiveScrollWheelAction(int direction)
    {
      this.scrollY += (float) direction;
      this.RepositionScrollElements();
      base.receiveScrollWheelAction(direction);
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoveredItem = (Item) null;
      this.metadata = "";
      foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
      {
        if (recipeComponent.containsPoint(x, y))
        {
          this.hoveredItem = this._recipeLookup[recipeComponent.name].Value;
          this.metadata = this._recipeMetadata[recipeComponent.name];
        }
      }
    }

    public bool canLeaveMenu() => true;

    public override void draw(SpriteBatch b)
    {
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
      b.End();
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
      b.GraphicsDevice.ScissorRectangle = this.scrollView;
      foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
      {
        if (recipeComponent.visible)
        {
          this.drawHorizontalPartition(b, recipeComponent.bounds.Bottom - 32, true);
          KeyValuePair<Item, Item> keyValuePair = this._recipeLookup[recipeComponent.name];
          recipeComponent.draw(b);
          keyValuePair.Key.drawInMenu(b, new Vector2((float) recipeComponent.bounds.X, (float) recipeComponent.bounds.Y), 1f);
          if (this._recipeColors.ContainsKey(recipeComponent.name))
          {
            int num = 24;
            b.Draw(Game1.staminaRect, new Rectangle(this.scrollView.Left + this.scrollView.Width / 2 - num / 2, recipeComponent.bounds.Center.Y - num / 2, num, num), this._recipeColors[recipeComponent.name]);
          }
          if (keyValuePair.Value != null)
            keyValuePair.Value.drawInMenu(b, new Vector2((float) (this.scrollView.Left + this.scrollView.Width - 128), (float) recipeComponent.bounds.Y), 1f);
        }
      }
      b.End();
      b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      this.okButton.draw(b);
      this.drawMouse(b);
      if (this.hoveredItem == null)
        return;
      Utility.drawTextWithShadow(b, this.metadata, Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen + this.height - 64)), Color.Black);
      if (Game1.oldKBState.IsKeyDown(Keys.LeftShift))
        return;
      IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem);
    }

    public override void update(GameTime time)
    {
    }
  }
}
