// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.Slots
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StardewValley.Minigames
{
  public class Slots : IMinigame
  {
    public const float slotTurnRate = 0.008f;
    public const int numberOfIcons = 8;
    public const int defaultBet = 10;
    private string coinBuffer;
    private List<float> slots;
    private List<float> slotResults;
    private ClickableComponent spinButton10;
    private ClickableComponent spinButton100;
    private ClickableComponent doneButton;
    private bool spinning;
    private bool showResult;
    private float payoutModifier;
    private int currentBet;
    private int spinsCount;
    private int slotsFinished;
    private int endTimer;
    public ClickableComponent currentlySnappedComponent;

    public Slots(int toBet = -1, bool highStakes = false)
    {
      string str;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ru:
          str = "     ";
          break;
        case LocalizedContentManager.LanguageCode.zh:
          str = "　　";
          break;
        default:
          str = "  ";
          break;
      }
      this.coinBuffer = str;
      this.currentBet = toBet;
      if (this.currentBet == -1)
        this.currentBet = 10;
      this.slots = new List<float>();
      this.slots.Add(0.0f);
      this.slots.Add(0.0f);
      this.slots.Add(0.0f);
      this.slotResults = new List<float>();
      this.slotResults.Add(0.0f);
      this.slotResults.Add(0.0f);
      this.slotResults.Add(0.0f);
      Game1.playSound("newArtifact");
      this.setSlotResults(this.slots);
      Vector2 centeringOnScreen1 = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 104, 52, -16, 32);
      this.spinButton10 = new ClickableComponent(new Rectangle((int) centeringOnScreen1.X, (int) centeringOnScreen1.Y, 104, 52), Game1.content.LoadString("Strings\\StringsFromCSFiles:Slots.cs.12117"));
      Vector2 centeringOnScreen2 = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 124, 52, -16, 96);
      this.spinButton100 = new ClickableComponent(new Rectangle((int) centeringOnScreen2.X, (int) centeringOnScreen2.Y, 124, 52), Game1.content.LoadString("Strings\\StringsFromCSFiles:Slots.cs.12118"));
      Vector2 centeringOnScreen3 = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 96, 52, -16, 160);
      this.doneButton = new ClickableComponent(new Rectangle((int) centeringOnScreen3.X, (int) centeringOnScreen3.Y, 96, 52), Game1.content.LoadString("Strings\\StringsFromCSFiles:NameSelect.cs.3864"));
      if (!Game1.isAnyGamePadButtonBeingPressed())
        return;
      Game1.setMousePosition(this.spinButton10.bounds.Center);
      if (!Game1.options.SnappyMenus)
        return;
      this.currentlySnappedComponent = this.spinButton10;
    }

    public void setSlotResults(List<float> toSet)
    {
      double num1 = Game1.random.NextDouble();
      double num2 = 1.0 + Game1.player.DailyLuck * 2.0 + (double) Game1.player.LuckLevel * 0.08;
      if (num1 < 0.001 * num2)
      {
        this.set(toSet, 5);
        this.payoutModifier = 2500f;
      }
      else if (num1 < 1.0 / 625.0 * num2)
      {
        this.set(toSet, 6);
        this.payoutModifier = 1000f;
      }
      else if (num1 < 1.0 / 400.0 * num2)
      {
        this.set(toSet, 7);
        this.payoutModifier = 500f;
      }
      else if (num1 < 0.005 * num2)
      {
        this.set(toSet, 4);
        this.payoutModifier = 200f;
      }
      else if (num1 < 0.007 * num2)
      {
        this.set(toSet, 3);
        this.payoutModifier = 120f;
      }
      else if (num1 < 0.01 * num2)
      {
        this.set(toSet, 2);
        this.payoutModifier = 80f;
      }
      else if (num1 < 0.02 * num2)
      {
        this.set(toSet, 1);
        this.payoutModifier = 30f;
      }
      else if (num1 < 0.12 * num2)
      {
        int num3 = Game1.random.Next(3);
        for (int index = 0; index < 3; ++index)
          toSet[index] = index == num3 ? (float) Game1.random.Next(7) : 7f;
        this.payoutModifier = 3f;
      }
      else if (num1 < 0.2 * num2)
      {
        this.set(toSet, 0);
        this.payoutModifier = 5f;
      }
      else if (num1 < 0.4 * num2)
      {
        int num4 = Game1.random.Next(3);
        for (int index = 0; index < 3; ++index)
          toSet[index] = index == num4 ? 7f : (float) Game1.random.Next(7);
        this.payoutModifier = 2f;
      }
      else
      {
        this.payoutModifier = 0.0f;
        int[] numArray = new int[8];
        for (int index1 = 0; index1 < 3; ++index1)
        {
          int index2 = Game1.random.Next(6);
          while (numArray[index2] > 1)
            index2 = Game1.random.Next(6);
          toSet[index1] = (float) index2;
          ++numArray[index2];
        }
      }
    }

    private void set(List<float> toSet, int number)
    {
      toSet[0] = (float) number;
      toSet[1] = (float) number;
      toSet[2] = (float) number;
    }

    public bool tick(GameTime time)
    {
      TimeSpan elapsedGameTime;
      if (this.spinning && this.endTimer <= 0)
      {
        for (int slotsFinished = this.slotsFinished; slotsFinished < this.slots.Count; ++slotsFinished)
        {
          float slot = this.slots[slotsFinished];
          List<float> slots = this.slots;
          int index1 = slotsFinished;
          List<float> floatList = slots;
          int index2 = index1;
          double num1 = (double) slots[index1];
          elapsedGameTime = time.ElapsedGameTime;
          double num2 = (double) elapsedGameTime.Milliseconds * 0.00800000037997961 * (1.0 - (double) slotsFinished * 0.0500000007450581);
          double num3 = num1 + num2;
          floatList[index2] = (float) num3;
          this.slots[slotsFinished] %= 8f;
          if (slotsFinished == 2)
          {
            if ((double) slot % (0.25 + (double) this.slotsFinished * 0.5) > (double) this.slots[slotsFinished] % (0.25 + (double) this.slotsFinished * 0.5))
              Game1.playSound("shiny4");
            if ((double) slot > (double) this.slots[slotsFinished])
              ++this.spinsCount;
          }
          if (this.spinsCount > 0 && slotsFinished == this.slotsFinished)
          {
            double num4 = (double) Math.Abs(this.slots[slotsFinished] - this.slotResults[slotsFinished]);
            elapsedGameTime = time.ElapsedGameTime;
            double num5 = (double) elapsedGameTime.Milliseconds * 0.00800000037997961;
            if (num4 <= num5)
            {
              this.slots[slotsFinished] = this.slotResults[slotsFinished];
              ++this.slotsFinished;
              --this.spinsCount;
              Game1.playSound("Cowboy_gunshot");
            }
          }
        }
        if (this.slotsFinished >= 3)
          this.endTimer = (double) this.payoutModifier == 0.0 ? 600 : 1000;
      }
      if (this.endTimer > 0)
      {
        int endTimer = this.endTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.endTimer = endTimer - milliseconds;
        if (this.endTimer <= 0)
        {
          this.spinning = false;
          this.spinsCount = 0;
          this.slotsFinished = 0;
          if ((double) this.payoutModifier > 0.0)
          {
            this.showResult = true;
            Game1.playSound((double) this.payoutModifier >= 5.0 ? ((double) this.payoutModifier >= 10.0 ? "reward" : "money") : "newArtifact");
          }
          else
            Game1.playSound("breathout");
          Game1.player.clubCoins += (int) ((double) this.currentBet * (double) this.payoutModifier);
          if ((double) this.payoutModifier == 2500.0)
            Game1.multiplayer.globalChatInfoMessage("Jackpot", Game1.player.Name);
        }
      }
      this.spinButton10.scale = this.spinning || !this.spinButton10.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1f : 1.05f;
      this.spinButton100.scale = this.spinning || !this.spinButton100.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1f : 1.05f;
      this.doneButton.scale = this.spinning || !this.doneButton.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1f : 1.05f;
      return false;
    }

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!this.spinning && Game1.player.clubCoins >= 10 && this.spinButton10.bounds.Contains(x, y))
      {
        ++Club.timesPlayedSlots;
        this.setSlotResults(this.slotResults);
        this.spinning = true;
        Game1.playSound("bigSelect");
        this.currentBet = 10;
        this.slotsFinished = 0;
        this.spinsCount = 0;
        this.showResult = false;
        Game1.player.clubCoins -= 10;
      }
      if (!this.spinning && Game1.player.clubCoins >= 100 && this.spinButton100.bounds.Contains(x, y))
      {
        ++Club.timesPlayedSlots;
        this.setSlotResults(this.slotResults);
        Game1.playSound("bigSelect");
        this.spinning = true;
        this.slotsFinished = 0;
        this.spinsCount = 0;
        this.showResult = false;
        this.currentBet = 100;
        Game1.player.clubCoins -= 100;
      }
      if (this.spinning || !this.doneButton.bounds.Contains(x, y))
        return;
      Game1.playSound("bigDeSelect");
      Game1.currentMinigame = (IMinigame) null;
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public void receiveKeyPress(Keys k)
    {
      if (!this.spinning && (k.Equals((object) Keys.Escape) || Game1.options.doesInputListContain(Game1.options.menuButton, k)))
      {
        this.unload();
        Game1.playSound("bigDeSelect");
        Game1.currentMinigame = (IMinigame) null;
      }
      else
      {
        if (this.spinning || this.currentlySnappedComponent == null)
          return;
        if (Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
        {
          if (this.currentlySnappedComponent.Equals((object) this.spinButton10))
          {
            this.currentlySnappedComponent = this.spinButton100;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
          }
          else
          {
            if (!this.currentlySnappedComponent.Equals((object) this.spinButton100))
              return;
            this.currentlySnappedComponent = this.doneButton;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
          }
        }
        else
        {
          if (!Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
            return;
          if (this.currentlySnappedComponent.Equals((object) this.doneButton))
          {
            this.currentlySnappedComponent = this.spinButton100;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
          }
          else
          {
            if (!this.currentlySnappedComponent.Equals((object) this.spinButton100))
              return;
            this.currentlySnappedComponent = this.spinButton10;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
          }
        }
      }
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public int getIconIndex(int index)
    {
      switch (index)
      {
        case 0:
          return 24;
        case 1:
          return 186;
        case 2:
          return 138;
        case 3:
          return 392;
        case 4:
          return 254;
        case 5:
          return 434;
        case 6:
          return 72;
        case 7:
          return 638;
        default:
          return 24;
      }
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      SpriteBatch spriteBatch1 = b;
      Texture2D staminaRect = Game1.staminaRect;
      Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int width = viewport1.Width;
      viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int height = viewport1.Height;
      Rectangle destinationRectangle = new Rectangle(0, 0, width, height);
      Color color = new Color(38, 0, 7);
      spriteBatch1.Draw(staminaRect, destinationRectangle, color);
      b.Draw(Game1.mouseCursors, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 228, 52, yOffset: -256), new Rectangle?(new Rectangle(441, 424, 57, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      for (int index = 0; index < 3; ++index)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 112 + index * 26 * 4), (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 128)), new Rectangle?(new Rectangle(306, 320, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        float num = (float) (((double) this.slots[index] + 1.0) % 8.0);
        int iconIndex1 = this.getIconIndex(((int) num + 8 - 1) % 8);
        int iconIndex2 = this.getIconIndex((iconIndex1 + 1) % 8);
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 112 + index * 26 * 4), (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 128)) - new Vector2(0.0f, (float) (-64.0 * ((double) num % 1.0))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, iconIndex1, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        SpriteBatch spriteBatch2 = b;
        Texture2D objectSpriteSheet = Game1.objectSpriteSheet;
        Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
        double x = (double) (viewport2.Width / 2 - 112 + index * 26 * 4);
        viewport2 = Game1.graphics.GraphicsDevice.Viewport;
        double y = (double) (viewport2.Height / 2 - 128);
        Vector2 position = new Vector2((float) x, (float) y) - new Vector2(0.0f, (float) (64.0 - 64.0 * ((double) num % 1.0)));
        Rectangle? sourceRectangle = new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, iconIndex2, 16, 16));
        Color white = Color.White;
        Vector2 zero = Vector2.Zero;
        spriteBatch2.Draw(objectSpriteSheet, position, sourceRectangle, white, 0.0f, zero, 4f, SpriteEffects.None, 0.99f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 132 + index * 26 * 4), (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 192)), new Rectangle?(new Rectangle(415, 385, 26, 48)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      }
      if (this.showResult)
        SpriteText.drawString(b, "+" + (this.payoutModifier * (float) this.currentBet).ToString(), Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 372, this.spinButton10.bounds.Y - 64 + 8, 9999, height: 9999, layerDepth: 1f, color: 4);
      b.Draw(Game1.mouseCursors, new Vector2((float) this.spinButton10.bounds.X, (float) this.spinButton10.bounds.Y), new Rectangle?(new Rectangle(441, 385, 26, 13)), Color.White * (this.spinning || Game1.player.clubCoins < 10 ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f * this.spinButton10.scale, SpriteEffects.None, 0.99f);
      b.Draw(Game1.mouseCursors, new Vector2((float) this.spinButton100.bounds.X, (float) this.spinButton100.bounds.Y), new Rectangle?(new Rectangle(441, 398, 31, 13)), Color.White * (this.spinning || Game1.player.clubCoins < 100 ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f * this.spinButton100.scale, SpriteEffects.None, 0.99f);
      b.Draw(Game1.mouseCursors, new Vector2((float) this.doneButton.bounds.X, (float) this.doneButton.bounds.Y), new Rectangle?(new Rectangle(441, 411, 24, 13)), Color.White * (!this.spinning ? 1f : 0.5f), 0.0f, Vector2.Zero, 4f * this.doneButton.scale, SpriteEffects.None, 0.99f);
      SpriteBatch b1 = b;
      string s = this.coinBuffer + Game1.player.clubCoins.ToString();
      int x1 = Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 376;
      Viewport viewport3 = Game1.graphics.GraphicsDevice.Viewport;
      int y1 = viewport3.Height / 2 - 120;
      SpriteText.drawStringWithScrollBackground(b1, s, x1, y1);
      SpriteBatch b2 = b;
      Texture2D mouseCursors = Game1.mouseCursors;
      viewport3 = Game1.graphics.GraphicsDevice.Viewport;
      double x2 = (double) (viewport3.Width / 2 - 376 + 4);
      viewport3 = Game1.graphics.GraphicsDevice.Viewport;
      double y2 = (double) (viewport3.Height / 2 - 120 + 4);
      Vector2 position1 = new Vector2((float) x2, (float) y2);
      Rectangle sourceRect = new Rectangle(211, 373, 9, 10);
      Color white1 = Color.White;
      Vector2 zero1 = Vector2.Zero;
      Utility.drawWithShadow(b2, mouseCursors, position1, sourceRect, white1, 0.0f, zero1, 4f, layerDepth: 1f);
      Vector2 vector2;
      ref Vector2 local = ref vector2;
      viewport3 = Game1.graphics.GraphicsDevice.Viewport;
      double x3 = (double) (viewport3.Width / 2 + 200);
      viewport3 = Game1.graphics.GraphicsDevice.Viewport;
      double y3 = (double) (viewport3.Height / 2 - 352);
      local = new Vector2((float) x3, (float) y3);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), (int) vector2.X, (int) vector2.Y, 384, 704, Color.White, 4f);
      b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(8f, 8f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(7), 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      SpriteText.drawString(b, "x2", (int) vector2.X + 192 + 16, (int) vector2.Y + 24, 9999, height: 99999, color: 4);
      b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(8f, 76f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(7), 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(76f, 76f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(7), 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      SpriteText.drawString(b, "x3", (int) vector2.X + 192 + 16, (int) vector2.Y + 68 + 24, 9999, height: 99999, color: 4);
      for (int index1 = 0; index1 < 8; ++index1)
      {
        int index2 = index1;
        if (index1 == 5)
          index2 = 7;
        else if (index1 == 7)
          index2 = 5;
        b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(8f, (float) (8 + (index1 + 2) * 68)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(index2), 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(76f, (float) (8 + (index1 + 2) * 68)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(index2), 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(144f, (float) (8 + (index1 + 2) * 68)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(index2), 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        int num = 0;
        switch (index1)
        {
          case 0:
            num = 5;
            break;
          case 1:
            num = 30;
            break;
          case 2:
            num = 80;
            break;
          case 3:
            num = 120;
            break;
          case 4:
            num = 200;
            break;
          case 5:
            num = 500;
            break;
          case 6:
            num = 1000;
            break;
          case 7:
            num = 2500;
            break;
        }
        SpriteText.drawString(b, "x" + num.ToString(), (int) vector2.X + 192 + 16, (int) vector2.Y + (index1 + 2) * 68 + 24, 9999, height: 99999, color: 4);
      }
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), (int) vector2.X - 640, (int) vector2.Y, 1024, 704, Color.Red, 4f, false);
      for (int index = 1; index < 8; ++index)
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), (int) vector2.X - 640 - 4 * index, (int) vector2.Y - 4 * index, 1024 + 8 * index, 704 + 8 * index, Color.Red * (float) (1.0 - (double) index * 0.150000005960464), 4f, false);
      for (int index = 0; index < 17; ++index)
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(147, 472, 3, 3), (int) vector2.X - 640 + 8, (int) vector2.Y + index * 4 * 3 + 12, (int) (608.0 - (double) (index * 64) * 1.20000004768372 + (double) (index * index * 4) * 0.699999988079071), 4, new Color(index * 25, index > 8 ? index * 10 : 0, (int) byte.MaxValue - index * 25), 4f, false);
      if (!Game1.options.hardwareCursor)
        b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
      b.End();
    }

    public void changeScreenSize()
    {
    }

    public void unload()
    {
    }

    public void receiveEventPoke(int data)
    {
    }

    public string minigameId() => nameof (Slots);

    public bool doMainGameUpdates() => false;

    public bool forceQuit()
    {
      if (this.spinning)
        Game1.player.clubCoins += this.currentBet;
      this.unload();
      return true;
    }
  }
}
