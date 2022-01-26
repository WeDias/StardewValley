// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AnimationPreviewTool
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StardewValley.Menus
{
  public class AnimationPreviewTool : IClickableMenu
  {
    public List<List<ClickableTextureComponent>> components;
    public Rectangle scrollView;
    public List<ClickableTextureComponent> animationButtons;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent hairLabel;
    public ClickableTextureComponent shirtLabel;
    public ClickableTextureComponent pantsLabel;
    public float scrollY;

    public AnimationPreviewTool()
      : base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + 64)
    {
      Game1.player.faceDirection(2);
      Game1.player.FarmerSprite.StopAnimation();
      FieldInfo[] fields = typeof (FarmerSprite).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      this.animationButtons = new List<ClickableTextureComponent>();
      foreach (FieldInfo fieldInfo in ((IEnumerable<FieldInfo>) fields).Where<FieldInfo>((Func<FieldInfo, bool>) (fi => fi.IsLiteral && !fi.IsInitOnly)))
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(0, 0, 200, 48), (Texture2D) null, new Rectangle(), 1f);
        textureComponent.myID = (int) fieldInfo.GetValue((object) null);
        textureComponent.name = fieldInfo.Name;
        this.animationButtons.Add(textureComponent);
      }
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent1.upNeighborID = -99998;
      textureComponent1.leftNeighborID = -99998;
      textureComponent1.rightNeighborID = -99998;
      textureComponent1.downNeighborID = -99998;
      this.okButton = textureComponent1;
      this.components = new List<List<ClickableTextureComponent>>();
      this.components.Add(new List<ClickableTextureComponent>((IEnumerable<ClickableTextureComponent>) new ClickableTextureComponent[1]
      {
        new ClickableTextureComponent("Hair Heading", new Rectangle(0, 0, 64, 16), "Hair", "", (Texture2D) null, new Rectangle(), 1f)
      }));
      this.hairLabel = new ClickableTextureComponent("Hair Label", new Rectangle(0, 0, 64, 64), "0", "", (Texture2D) null, new Rectangle(), 1f);
      List<List<ClickableTextureComponent>> components1 = this.components;
      ClickableTextureComponent[] collection1 = new ClickableTextureComponent[3];
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("Hair Style", new Rectangle(0, 0, 64, 64), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent2.myID = -1;
      collection1[0] = textureComponent2;
      collection1[1] = this.hairLabel;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("Hair Style", new Rectangle(0, 0, 64, 64), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent3.myID = 1;
      collection1[2] = textureComponent3;
      List<ClickableTextureComponent> textureComponentList1 = new List<ClickableTextureComponent>((IEnumerable<ClickableTextureComponent>) collection1);
      components1.Add(textureComponentList1);
      this.components.Add(new List<ClickableTextureComponent>((IEnumerable<ClickableTextureComponent>) new ClickableTextureComponent[1]
      {
        new ClickableTextureComponent("Shirt Heading", new Rectangle(0, 0, 64, 16), "Shirt", "", (Texture2D) null, new Rectangle(), 1f)
      }));
      this.shirtLabel = new ClickableTextureComponent("Shirt Label", new Rectangle(0, 0, 64, 64), "0", "", (Texture2D) null, new Rectangle(), 1f);
      List<List<ClickableTextureComponent>> components2 = this.components;
      ClickableTextureComponent[] collection2 = new ClickableTextureComponent[3];
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("Shirt Style", new Rectangle(0, 0, 64, 64), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent4.myID = -1;
      collection2[0] = textureComponent4;
      collection2[1] = this.shirtLabel;
      ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Shirt Style", new Rectangle(0, 0, 64, 64), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent5.myID = 1;
      collection2[2] = textureComponent5;
      List<ClickableTextureComponent> textureComponentList2 = new List<ClickableTextureComponent>((IEnumerable<ClickableTextureComponent>) collection2);
      components2.Add(textureComponentList2);
      this.components.Add(new List<ClickableTextureComponent>((IEnumerable<ClickableTextureComponent>) new ClickableTextureComponent[1]
      {
        new ClickableTextureComponent("Pants Heading", new Rectangle(0, 0, 64, 16), "Pants", "", (Texture2D) null, new Rectangle(), 1f)
      }));
      this.pantsLabel = new ClickableTextureComponent("Pants Label", new Rectangle(0, 0, 64, 64), "0", "", (Texture2D) null, new Rectangle(), 1f);
      List<List<ClickableTextureComponent>> components3 = this.components;
      ClickableTextureComponent[] collection3 = new ClickableTextureComponent[3];
      ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Pants Style", new Rectangle(0, 0, 64, 64), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent6.myID = -1;
      collection3[0] = textureComponent6;
      collection3[1] = this.pantsLabel;
      ClickableTextureComponent textureComponent7 = new ClickableTextureComponent("Pants Style", new Rectangle(0, 0, 64, 64), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent7.myID = 1;
      collection3[2] = textureComponent7;
      List<ClickableTextureComponent> textureComponentList3 = new List<ClickableTextureComponent>((IEnumerable<ClickableTextureComponent>) collection3);
      components3.Add(textureComponentList3);
      this.components.Add(new List<ClickableTextureComponent>((IEnumerable<ClickableTextureComponent>) new ClickableTextureComponent[1]
      {
        new ClickableTextureComponent("Toggle Gender", new Rectangle(0, 0, 64, 64), "Toggle Gender", "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 25), 1f)
      }));
      this.RepositionElements();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;
      this.RepositionElements();
    }

    public void SwitchShirt(int direction)
    {
      Game1.player.changeShirt(Game1.player.shirt.Value + direction);
      this.UpdateLabels();
    }

    public void SwitchHair(int direction)
    {
      Game1.player.changeHairStyle(Game1.player.hair.Value + direction);
      this.UpdateLabels();
    }

    public void SwitchPants(int direction)
    {
      Game1.player.changePantStyle(Game1.player.pants.Value + direction);
      this.UpdateLabels();
    }

    private void RepositionElements()
    {
      this.scrollView = new Rectangle(this.xPositionOnScreen + 320, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 250, 500);
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
      int num1 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 200;
      foreach (List<ClickableTextureComponent> component in this.components)
      {
        int num2 = this.xPositionOnScreen + 70;
        int val2 = 0;
        foreach (ClickableTextureComponent textureComponent in component)
        {
          textureComponent.bounds.X = num2;
          textureComponent.bounds.Y = num1;
          num2 += textureComponent.bounds.Width + 8;
          val2 = Math.Max(textureComponent.bounds.Height, val2);
        }
        num1 += val2 + 8;
      }
      this.RepositionScrollElements();
      this.UpdateLabels();
    }

    public void UpdateLabels()
    {
      ClickableTextureComponent pantsLabel = this.pantsLabel;
      int num = Game1.player.GetPantsIndex();
      string str1 = num.ToString() ?? "";
      pantsLabel.label = str1;
      ClickableTextureComponent shirtLabel = this.shirtLabel;
      num = Game1.player.GetShirtIndex();
      string str2 = num.ToString() ?? "";
      shirtLabel.label = str2;
      ClickableTextureComponent hairLabel = this.hairLabel;
      num = Game1.player.getHair();
      string str3 = num.ToString() ?? "";
      hairLabel.label = str3;
    }

    public void RepositionScrollElements()
    {
      int scrollY = (int) this.scrollY;
      if ((double) this.scrollY > 0.0)
        this.scrollY = 0.0f;
      foreach (ClickableTextureComponent animationButton in this.animationButtons)
      {
        animationButton.bounds.X = this.scrollView.X;
        animationButton.bounds.Y = this.scrollView.Y + scrollY;
        animationButton.bounds.Width = this.scrollView.Width;
        scrollY += animationButton.bounds.Height;
        if (this.scrollView.Intersects(animationButton.bounds))
          animationButton.visible = true;
        else
          animationButton.visible = false;
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
      foreach (ClickableTextureComponent animationButton in this.animationButtons)
      {
        if (animationButton.bounds.Contains(x, y) && this.scrollView.Contains(x, y))
        {
          if (animationButton.name.Contains("Left"))
            Game1.player.faceDirection(3);
          else if (animationButton.name.Contains("Right"))
            Game1.player.faceDirection(1);
          else if (animationButton.name.Contains("Up"))
            Game1.player.faceDirection(0);
          else
            Game1.player.faceDirection(2);
          Game1.player.completelyStopAnimatingOrDoingAction();
          Game1.player.animateOnce(animationButton.myID);
        }
      }
      foreach (List<ClickableTextureComponent> component in this.components)
      {
        foreach (ClickableTextureComponent textureComponent in component)
        {
          if (textureComponent.containsPoint(x, y))
          {
            if (textureComponent.name == "Shirt Style")
              this.SwitchShirt(textureComponent.myID);
            else if (textureComponent.name == "Pants Style")
              this.SwitchPants(textureComponent.myID);
            else if (textureComponent.name == "Hair Style")
              this.SwitchHair(textureComponent.myID);
            else if (textureComponent.name == "Toggle Gender")
              Game1.player.changeGender(!Game1.player.isMale.Value);
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
    }

    public bool canLeaveMenu() => true;

    public override void draw(SpriteBatch b)
    {
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
      b.Draw(Game1.daybg, new Vector2((float) (this.xPositionOnScreen + 64 + 42 - 2), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16)), Color.White);
      Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2((float) (this.xPositionOnScreen - 2 + 42 + 128 - 32), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth - 16 + IClickableMenu.spaceToClearTopBorder + 32)), Vector2.Zero, 0.8f, Color.White, 0.0f, 1f, Game1.player);
      b.End();
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
      b.GraphicsDevice.ScissorRectangle = this.scrollView;
      foreach (ClickableTextureComponent animationButton in this.animationButtons)
      {
        if (animationButton.visible)
        {
          Game1.DrawBox(animationButton.bounds.X, animationButton.bounds.Y, animationButton.bounds.Width, animationButton.bounds.Height);
          Utility.drawTextWithShadow(b, animationButton.name, Game1.smallFont, new Vector2((float) animationButton.bounds.X, (float) animationButton.bounds.Y), Color.Black);
        }
      }
      b.End();
      b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      foreach (List<ClickableTextureComponent> component in this.components)
      {
        foreach (ClickableTextureComponent textureComponent in component)
          textureComponent.draw(b);
      }
      this.okButton.draw(b);
      this.drawMouse(b);
    }

    public override void update(GameTime time)
    {
    }
  }
}
