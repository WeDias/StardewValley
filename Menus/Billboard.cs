// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.Billboard
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class Billboard : IClickableMenu
  {
    private Texture2D billboardTexture;
    public const int basewidth = 338;
    public const int baseWidth_calendar = 301;
    public const int baseheight = 198;
    private bool dailyQuestBoard;
    public ClickableComponent acceptQuestButton;
    public List<ClickableTextureComponent> calendarDays;
    private string hoverText = "";
    private string nightMarketLocalized;
    private string wizardBirthdayLocalized;
    protected Dictionary<ClickableTextureComponent, List<string>> _upcomingWeddings;

    public Billboard(bool dailyQuest = false)
      : base(0, 0, 0, 0, true)
    {
      this._upcomingWeddings = new Dictionary<ClickableTextureComponent, List<string>>();
      if (!Game1.player.hasOrWillReceiveMail("checkedBulletinOnce"))
      {
        Game1.player.mailReceived.Add("checkedBulletinOnce");
        (Game1.getLocationFromName("Town") as Town).checkedBoard();
      }
      this.dailyQuestBoard = dailyQuest;
      this.billboardTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Billboard");
      this.width = (dailyQuest ? 338 : 301) * 4;
      this.height = 792;
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
      this.xPositionOnScreen = (int) centeringOnScreen.X;
      this.yPositionOnScreen = (int) centeringOnScreen.Y;
      this.acceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128, this.yPositionOnScreen + this.height - 128, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
      {
        myID = 0
      };
      this.UpdateDailyQuestButton();
      this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
      Game1.playSound("bigSelect");
      if (!dailyQuest)
      {
        this.calendarDays = new List<ClickableTextureComponent>();
        Dictionary<int, NPC> dictionary = new Dictionary<int, NPC>();
        foreach (NPC allCharacter in Utility.getAllCharacters())
        {
          if (allCharacter.isVillager() && allCharacter.Birthday_Season != null && allCharacter.Birthday_Season.Equals(Game1.currentSeason) && !dictionary.ContainsKey(allCharacter.Birthday_Day) && (Game1.player.friendshipData.ContainsKey(allCharacter.Name) || !allCharacter.Name.Equals("Dwarf") && !allCharacter.Name.Equals("Sandy") && !allCharacter.Name.Equals("Krobus")))
            dictionary.Add(allCharacter.Birthday_Day, allCharacter);
        }
        this.nightMarketLocalized = Game1.content.LoadString("Strings\\UI:Billboard_NightMarket");
        this.wizardBirthdayLocalized = Game1.content.LoadString("Strings\\UI:Billboard_Birthday", (object) Game1.getCharacterFromName("Wizard").displayName);
        for (int index = 1; index <= 28; ++index)
        {
          string str1 = "";
          string hoverText = "";
          NPC npc = dictionary.ContainsKey(index) ? dictionary[index] : (NPC) null;
          if (Utility.isFestivalDay(index, Game1.currentSeason))
          {
            str1 = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + index.ToString())["name"];
          }
          else
          {
            if (npc != null)
              hoverText = npc.displayName.Last<char>() == 's' || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de && (npc.displayName.Last<char>() == 'x' || npc.displayName.Last<char>() == 'ß' || npc.displayName.Last<char>() == 'z') ? Game1.content.LoadString("Strings\\UI:Billboard_SBirthday", (object) npc.displayName) : Game1.content.LoadString("Strings\\UI:Billboard_Birthday", (object) npc.displayName);
            if (Game1.currentSeason.Equals("winter") && index >= 15 && index <= 17)
              str1 = this.nightMarketLocalized;
          }
          Texture2D texture = (Texture2D) null;
          if (npc != null)
          {
            try
            {
              texture = Game1.content.Load<Texture2D>("Characters\\" + npc.getTextureName());
            }
            catch (Exception ex)
            {
              texture = npc.Sprite.Texture;
            }
          }
          ClickableTextureComponent textureComponent = new ClickableTextureComponent(str1, new Rectangle(this.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, this.yPositionOnScreen + 200 + (index - 1) / 7 * 32 * 4, 124, 124), str1, hoverText, texture, npc != null ? new Rectangle(0, 0, 16, 24) : Rectangle.Empty, 1f);
          textureComponent.myID = index;
          textureComponent.rightNeighborID = index % 7 != 0 ? index + 1 : -1;
          textureComponent.leftNeighborID = index % 7 != 1 ? index - 1 : -1;
          textureComponent.downNeighborID = index + 7;
          textureComponent.upNeighborID = index > 7 ? index - 7 : -1;
          ClickableTextureComponent key = textureComponent;
          HashSet<Farmer> farmerSet = new HashSet<Farmer>();
          foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
          {
            if (!farmerSet.Contains(onlineFarmer) && onlineFarmer.isEngaged() && !onlineFarmer.hasCurrentOrPendingRoommate())
            {
              string str2 = (string) null;
              WorldDate worldDate = (WorldDate) null;
              if (Game1.getCharacterFromName(onlineFarmer.spouse) != null)
              {
                worldDate = onlineFarmer.friendshipData[onlineFarmer.spouse].WeddingDate;
                str2 = Game1.getCharacterFromName(onlineFarmer.spouse).displayName;
              }
              else
              {
                long? spouse = onlineFarmer.team.GetSpouse((long) onlineFarmer.uniqueMultiplayerID);
                if (spouse.HasValue)
                {
                  Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(spouse.Value);
                  if (farmerMaybeOffline != null && Game1.getOnlineFarmers().Contains(farmerMaybeOffline))
                  {
                    worldDate = onlineFarmer.team.GetFriendship((long) onlineFarmer.uniqueMultiplayerID, spouse.Value).WeddingDate;
                    farmerSet.Add(farmerMaybeOffline);
                    str2 = farmerMaybeOffline.Name;
                  }
                }
              }
              if (!(worldDate == (WorldDate) null))
              {
                if (worldDate.TotalDays < Game1.Date.TotalDays)
                {
                  worldDate = new WorldDate(Game1.Date);
                  ++worldDate.TotalDays;
                }
                if (worldDate != (WorldDate) null && worldDate.TotalDays >= Game1.Date.TotalDays && Utility.getSeasonNumber(Game1.currentSeason) == worldDate.SeasonIndex && index == worldDate.DayOfMonth)
                {
                  if (!this._upcomingWeddings.ContainsKey(key))
                    this._upcomingWeddings[key] = new List<string>();
                  farmerSet.Add(onlineFarmer);
                  this._upcomingWeddings[key].Add(onlineFarmer.Name);
                  this._upcomingWeddings[key].Add(str2);
                }
              }
            }
          }
          this.calendarDays.Add(key);
        }
      }
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(this.dailyQuestBoard ? 0 : 1);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      Game1.activeClickableMenu = (IClickableMenu) new Billboard(this.dailyQuestBoard);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      Game1.playSound("bigDeSelect");
      this.exitThisMenu();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      if (!this.acceptQuestButton.visible || !this.acceptQuestButton.containsPoint(x, y))
        return;
      Game1.playSound("newArtifact");
      Game1.questOfTheDay.dailyQuest.Value = true;
      Game1.questOfTheDay.dayQuestAccepted.Value = Game1.Date.TotalDays;
      Game1.questOfTheDay.accepted.Value = true;
      Game1.questOfTheDay.canBeCancelled.Value = true;
      Game1.questOfTheDay.daysLeft.Value = 2;
      Game1.player.questLog.Add(Game1.questOfTheDay);
      Game1.player.acceptedDailyQuest.Set(true);
      this.UpdateDailyQuestButton();
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.hoverText = "";
      if (this.dailyQuestBoard && Game1.questOfTheDay != null && !(bool) (NetFieldBase<bool, NetBool>) Game1.questOfTheDay.accepted)
      {
        float scale = this.acceptQuestButton.scale;
        this.acceptQuestButton.scale = this.acceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
        if ((double) this.acceptQuestButton.scale > (double) scale)
          Game1.playSound("Cowboy_gunshot");
      }
      if (this.calendarDays == null)
        return;
      foreach (ClickableTextureComponent calendarDay in this.calendarDays)
      {
        if (calendarDay.bounds.Contains(x, y))
        {
          this.hoverText = calendarDay.hoverText.Length <= 0 ? calendarDay.label : calendarDay.hoverText;
          if (calendarDay.hoverText.Equals(this.wizardBirthdayLocalized))
          {
            ClickableTextureComponent textureComponent = calendarDay;
            textureComponent.hoverText = textureComponent.hoverText + Environment.NewLine + this.nightMarketLocalized;
          }
          if (this._upcomingWeddings.ContainsKey(calendarDay))
          {
            for (int index = 0; index < this._upcomingWeddings[calendarDay].Count / 2; ++index)
              this.hoverText = this.hoverText + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Calendar_Wedding", (object) this._upcomingWeddings[calendarDay][index * 2], (object) this._upcomingWeddings[calendarDay][index * 2 + 1]);
          }
          this.hoverText = this.hoverText.Trim();
        }
      }
    }

    public override void draw(SpriteBatch b)
    {
      bool flag = false;
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      b.Draw(this.billboardTexture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(this.dailyQuestBoard ? new Rectangle(0, 0, 338, 198) : new Rectangle(0, 198, 301, 198)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      if (!this.dailyQuestBoard)
      {
        b.DrawString(Game1.dialogueFont, Utility.getSeasonNameFromNumber(Utility.getSeasonNumber(Game1.currentSeason)), new Vector2((float) (this.xPositionOnScreen + 160), (float) (this.yPositionOnScreen + 80)), Game1.textColor);
        b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:Billboard_Year", (object) Game1.year), new Vector2((float) (this.xPositionOnScreen + 448), (float) (this.yPositionOnScreen + 80)), Game1.textColor);
        for (int index = 0; index < this.calendarDays.Count; ++index)
        {
          if (this.calendarDays[index].name.Length > 0)
          {
            if (this.calendarDays[index].name.Equals(this.nightMarketLocalized))
              Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.calendarDays[index].bounds.X + 12), (float) (this.calendarDays[index].bounds.Y + 60) - Game1.dialogueButtonScale / 2f), new Rectangle(346, 392, 8, 8), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
            else
              Utility.drawWithShadow(b, this.billboardTexture, new Vector2((float) (this.calendarDays[index].bounds.X + 40), (float) (this.calendarDays[index].bounds.Y + 56) - Game1.dialogueButtonScale / 2f), new Rectangle(1 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0 / 100.0) * 14, 398, 14, 12), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
          }
          if (this.calendarDays[index].hoverText.Length > 0)
            b.Draw(this.calendarDays[index].texture, new Vector2((float) (this.calendarDays[index].bounds.X + 48), (float) (this.calendarDays[index].bounds.Y + 28)), new Rectangle?(this.calendarDays[index].sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          if (this._upcomingWeddings.ContainsKey(this.calendarDays[index]))
          {
            foreach (string str in this._upcomingWeddings[this.calendarDays[index]])
              b.Draw(Game1.mouseCursors2, new Vector2((float) (this.calendarDays[index].bounds.Right - 56), (float) (this.calendarDays[index].bounds.Top - 12)), new Rectangle?(new Rectangle(112, 32, 16, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          }
          if (Game1.dayOfMonth > index + 1)
            b.Draw(Game1.staminaRect, this.calendarDays[index].bounds, Color.Gray * 0.25f);
          else if (Game1.dayOfMonth == index + 1)
          {
            int num = (int) (4.0 * (double) Game1.dialogueButtonScale / 8.0);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), this.calendarDays[index].bounds.X - num, this.calendarDays[index].bounds.Y - num, this.calendarDays[index].bounds.Width + num * 2, this.calendarDays[index].bounds.Height + num * 2, Color.Blue, 4f, false);
          }
        }
      }
      else
      {
        if (Game1.options.SnappyMenus)
          flag = true;
        if (Game1.questOfTheDay == null || Game1.questOfTheDay.currentObjective == null || Game1.questOfTheDay.currentObjective.Length == 0)
        {
          b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:Billboard_NothingPosted"), new Vector2((float) (this.xPositionOnScreen + 384), (float) (this.yPositionOnScreen + 320)), Game1.textColor);
        }
        else
        {
          SpriteFont spriteFont = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont;
          string text = Game1.parseText(Game1.questOfTheDay.questDescription, spriteFont, 640);
          Utility.drawTextWithShadow(b, text, spriteFont, new Vector2((float) (this.xPositionOnScreen + 320 + 32), (float) (this.yPositionOnScreen + 256)), Game1.textColor, shadowIntensity: 0.5f);
          if (this.acceptQuestButton.visible)
          {
            flag = false;
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.acceptQuestButton.bounds.X, this.acceptQuestButton.bounds.Y, this.acceptQuestButton.bounds.Width, this.acceptQuestButton.bounds.Height, (double) this.acceptQuestButton.scale > 1.0 ? Color.LightPink : Color.White, 4f * this.acceptQuestButton.scale);
            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2((float) (this.acceptQuestButton.bounds.X + 12), (float) (this.acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12))), Game1.textColor);
          }
        }
      }
      base.draw(b);
      if (flag)
        return;
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
      if (this.hoverText.Length <= 0)
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
    }

    public void UpdateDailyQuestButton()
    {
      if (this.acceptQuestButton == null)
        return;
      if (!this.dailyQuestBoard)
        this.acceptQuestButton.visible = false;
      else
        this.acceptQuestButton.visible = Game1.CanAcceptDailyQuest();
    }
  }
}
