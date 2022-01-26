// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LanguageSelectionMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class LanguageSelectionMenu : IClickableMenu
  {
    public new static int width = 500;
    public new static int height = 728;
    private Texture2D texture;
    protected int _currentPage;
    protected int _pageCount = 1;
    public List<ClickableComponent> languages = new List<ClickableComponent>();
    public List<ModLanguage> modLanguageData;
    public Dictionary<string, ModLanguage> modLanguageLookup = new Dictionary<string, ModLanguage>();
    public List<string> languageList = new List<string>();
    public ClickableTextureComponent nextPageButton;
    public ClickableTextureComponent previousPageButton;

    public LanguageSelectionMenu()
    {
      this.texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\LanguageButtons");
      List<ModLanguage> modLanguageList = Game1.content.Load<List<ModLanguage>>("Data\\AdditionalLanguages");
      this.languageList.Clear();
      this.modLanguageLookup.Clear();
      this.languageList.AddRange((IEnumerable<string>) new string[12]
      {
        "English",
        "Russian",
        "Chinese",
        "German",
        "Portuguese",
        "French",
        "Spanish",
        "Japanese",
        "Korean",
        "Italian",
        "Turkish",
        "Hungarian"
      });
      if (modLanguageList != null)
      {
        foreach (ModLanguage modLanguage in modLanguageList)
        {
          this.languageList.Add("ModLanguage_" + modLanguage.ID);
          this.modLanguageLookup["ModLanguage_" + modLanguage.ID] = modLanguage;
        }
      }
      this._pageCount = (int) Math.Floor((double) (this.languageList.Count - 1) / 12.0) + 1;
      this.SetupButtons();
    }

    private void SetupButtons()
    {
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen((int) ((double) LanguageSelectionMenu.width * 2.5), LanguageSelectionMenu.height);
      this.languages.Clear();
      int height = 83;
      int index = 12 * this._currentPage;
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 64, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 6 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 0,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 448, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 6 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 3,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 832, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 6 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 6,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 64, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 5 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 1,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 448, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 5 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 4,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 832, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 5 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 7,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 64, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 4 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 2,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 448, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 4 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 5,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 832, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 4 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 8,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 64, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 3 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 9,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 448, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 3 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 10,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++index;
      }
      if (index < this.languageList.Count)
      {
        this.languages.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 832, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * 3 - 16, LanguageSelectionMenu.width - 128, height), this.languageList[index], (string) null)
        {
          myID = 11,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        int num = index + 1;
      }
      this.previousPageButton = (ClickableTextureComponent) null;
      if (this._currentPage > 0)
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle((int) centeringOnScreen.X + 4, (int) centeringOnScreen.Y + LanguageSelectionMenu.height / 2 - 25, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
        textureComponent.myID = 554;
        textureComponent.downNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.rightNeighborID = -99998;
        textureComponent.upNeighborID = -99998;
        this.previousPageButton = textureComponent;
      }
      this.nextPageButton = (ClickableTextureComponent) null;
      if (this._currentPage < this._pageCount - 1)
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle((int) ((double) centeringOnScreen.X + (double) LanguageSelectionMenu.width * 2.5) - 32, (int) centeringOnScreen.Y + LanguageSelectionMenu.height / 2 - 25, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
        textureComponent.myID = 555;
        textureComponent.downNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.rightNeighborID = -99998;
        textureComponent.upNeighborID = -99998;
        this.nextPageButton = textureComponent;
      }
      if (!Game1.options.SnappyMenus)
        return;
      int id = this.currentlySnappedComponent != null ? this.currentlySnappedComponent.myID : 0;
      this.populateClickableComponentList();
      this.currentlySnappedComponent = this.getComponentWithID(id);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      if (this.nextPageButton != null && this.nextPageButton.containsPoint(x, y))
      {
        Game1.playSound("shwip");
        ++this._currentPage;
        this.SetupButtons();
      }
      else if (this.previousPageButton != null && this.previousPageButton.containsPoint(x, y))
      {
        Game1.playSound("shwip");
        --this._currentPage;
        this.SetupButtons();
      }
      else
      {
        foreach (ClickableComponent language in this.languages)
        {
          if (language.containsPoint(x, y))
          {
            Game1.playSound("select");
            bool flag = true;
            if (language.name.StartsWith("ModLanguage_"))
            {
              flag = true;
              LocalizedContentManager.SetModLanguage(this.modLanguageLookup[language.name]);
            }
            else
            {
              switch (language.name)
              {
                case "Chinese":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.zh;
                  break;
                case "English":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.en;
                  break;
                case "French":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.fr;
                  break;
                case "German":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.de;
                  break;
                case "Hungarian":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.hu;
                  break;
                case "Italian":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.it;
                  break;
                case "Japanese":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.ja;
                  break;
                case "Korean":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.ko;
                  break;
                case "Portuguese":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.pt;
                  break;
                case "Russian":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.ru;
                  break;
                case "Spanish":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.es;
                  break;
                case "Turkish":
                  LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.tr;
                  break;
                default:
                  flag = false;
                  break;
              }
            }
            if (Game1.options.SnappyMenus)
            {
              Game1.activeClickableMenu.setCurrentlySnappedComponentTo(81118);
              Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
            if (flag)
            {
              this.ApplyLanguageChange();
              this.exitThisMenu();
            }
          }
        }
        this.isWithinBounds(x, y);
      }
    }

    public virtual void ApplyLanguageChange()
    {
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      foreach (ClickableComponent language in this.languages)
      {
        if (language.containsPoint(x, y))
        {
          if (language.label == null)
          {
            Game1.playSound("Cowboy_Footstep");
            language.label = "hovered";
          }
        }
        else
          language.label = (string) null;
      }
      if (this.previousPageButton != null)
        this.previousPageButton.tryHover(x, y);
      if (this.nextPageButton == null)
        return;
      this.nextPageButton.tryHover(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void draw(SpriteBatch b)
    {
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen((int) ((double) LanguageSelectionMenu.width * 2.5), LanguageSelectionMenu.height);
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(473, 36, 24, 24), (int) centeringOnScreen.X + 32, (int) centeringOnScreen.Y + 156, (int) ((double) LanguageSelectionMenu.width * 2.54999995231628) - 64, LanguageSelectionMenu.height / 2 + 25, Color.White, 4f);
      foreach (ClickableComponent language in this.languages)
      {
        int num = 0;
        switch (language.name)
        {
          case "Chinese":
            num = 4;
            break;
          case "English":
            num = 0;
            break;
          case "French":
            num = 7;
            break;
          case "German":
            num = 6;
            break;
          case "Hungarian":
            num = 11;
            break;
          case "Italian":
            num = 10;
            break;
          case "Japanese":
            num = 5;
            break;
          case "Korean":
            num = 8;
            break;
          case "Portuguese":
            num = 2;
            break;
          case "Russian":
            num = 3;
            break;
          case "Spanish":
            num = 1;
            break;
          case "Turkish":
            num = 9;
            break;
        }
        if (this.modLanguageLookup.ContainsKey(language.name))
        {
          Texture2D texture = Game1.temporaryContent.Load<Texture2D>(this.modLanguageLookup[language.name].ButtonTexture);
          Rectangle rectangle = new Rectangle(0, 0, 174, 39);
          if (language.label != null)
            rectangle.Y += 39;
          b.Draw(texture, language.bounds, new Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, 0.0f);
        }
        else
        {
          int y = (num <= 6 ? num * 78 : (num - 7) * 78) + (language.label != null ? 39 : 0);
          int x = num > 6 ? 174 : 0;
          b.Draw(this.texture, language.bounds, new Rectangle?(new Rectangle(x, y, 174, 40)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, 0.0f);
        }
      }
      if (this.previousPageButton != null)
        this.previousPageButton.draw(b);
      if (this.nextPageButton != null)
        this.nextPageButton.draw(b);
      if (Game1.activeClickableMenu != this)
        return;
      this.drawMouse(b);
    }

    public override bool readyToClose() => true;

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.SetupButtons();
    }
  }
}
