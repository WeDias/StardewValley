// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.PlayerCoupleBirthingEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StardewValley.Events
{
  public class PlayerCoupleBirthingEvent : FarmEvent, INetObject<NetFields>
  {
    private int behavior;
    private int timer;
    private string soundName;
    private string message;
    private string babyName;
    private bool playedSound;
    private bool showedMessage;
    private bool isMale;
    private bool getBabyName;
    private bool naming;
    private Vector2 targetLocation;
    private TextBox babyNameBox;
    private ClickableTextureComponent okButton;
    private FarmHouse farmHouse;
    private long spouseID;
    private Farmer spouse;
    private bool isPlayersTurn;
    private Child child;

    public NetFields NetFields { get; } = new NetFields();

    public PlayerCoupleBirthingEvent()
    {
      this.spouseID = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
      Game1.otherFarmers.TryGetValue(this.spouseID, out this.spouse);
      this.farmHouse = this.chooseHome();
      if (this.farmHouse.getChildren().Count < 1)
        return;
      Game1.getSteamAchievement("Achievement_FullHouse");
    }

    private bool isSuitableHome(FarmHouse home) => home.getChildrenCount() < 2 && home.upgradeLevel >= 2;

    private FarmHouse chooseHome()
    {
      List<Farmer> farmerList = new List<Farmer>();
      farmerList.Add(Game1.player);
      farmerList.Add(this.spouse);
      farmerList.Sort((Comparison<Farmer>) ((p1, p2) => p1.UniqueMultiplayerID.CompareTo(p2.UniqueMultiplayerID)));
      foreach (Farmer farmer in farmerList)
      {
        if (Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) farmer.homeLocation) is FarmHouse locationFromName && locationFromName == farmer.currentLocation && this.isSuitableHome(locationFromName))
          return locationFromName;
      }
      foreach (Farmer farmer in farmerList)
      {
        if (Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) farmer.homeLocation) is FarmHouse locationFromName && this.isSuitableHome(locationFromName))
          return locationFromName;
      }
      return Game1.player.currentLocation as FarmHouse;
    }

    public bool setUp()
    {
      if (this.spouse == null || this.farmHouse == null)
        return true;
      Random random = new Random((int) Game1.uniqueIDForThisGame ^ Game1.Date.TotalDays);
      Game1.player.CanMove = false;
      this.isMale = this.farmHouse.getChildrenCount() != 0 ? this.farmHouse.getChildren()[0].Gender == 1 : random.NextDouble() < 0.5;
      this.isPlayersTurn = Game1.player.GetSpouseFriendship().Proposer != Game1.player.UniqueMultiplayerID == (this.farmHouse.getChildrenCount() % 2 == 0);
      this.message = this.spouse.IsMale != Game1.player.IsMale ? (!this.spouse.IsMale ? Game1.content.LoadString("Strings\\Events:BirthMessage_SpouseMother", (object) Lexicon.getGenderedChildTerm(this.isMale), (object) this.spouse.Name) : Game1.content.LoadString("Strings\\Events:BirthMessage_PlayerMother", (object) Lexicon.getGenderedChildTerm(this.isMale))) : Game1.content.LoadString("Strings\\Events:BirthMessage_Adoption", (object) Lexicon.getGenderedChildTerm(this.isMale));
      return false;
    }

    public void returnBabyName(string name)
    {
      this.babyName = name;
      Game1.exitActiveMenu();
    }

    public void afterMessage()
    {
      if (this.isPlayersTurn)
      {
        this.getBabyName = true;
        double num = (this.spouse.hasDarkSkin() ? 0.5 : 0.0) + (Game1.player.hasDarkSkin() ? 0.5 : 0.0);
        this.farmHouse.characters.Add((NPC) (this.child = new Child("Baby", this.isMale, new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed).NextDouble() < num, Game1.player)));
        this.child.Age = 0;
        this.child.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
        Game1.player.GetSpouseFriendship().NextBirthingDate = (WorldDate) null;
      }
      else
      {
        Game1.afterDialogues = (Game1.afterFadeFunction) (() => this.getBabyName = true);
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:BirthMessage_SpouseNaming_" + (this.isMale ? "Male" : "Female"), (object) this.spouse.Name));
      }
    }

    public bool tickUpdate(GameTime time)
    {
      Game1.player.CanMove = false;
      this.timer += time.ElapsedGameTime.Milliseconds;
      Game1.fadeToBlackAlpha = 1f;
      if (this.timer > 1500 && !this.playedSound && !this.getBabyName)
      {
        if (this.soundName != null && !this.soundName.Equals(""))
        {
          Game1.playSound(this.soundName);
          this.playedSound = true;
        }
        if (!this.playedSound && this.message != null && !Game1.dialogueUp && Game1.activeClickableMenu == null)
        {
          Game1.drawObjectDialogue(this.message);
          Game1.afterDialogues = new Game1.afterFadeFunction(this.afterMessage);
        }
      }
      else if (this.getBabyName)
      {
        if (!this.isPlayersTurn)
        {
          Game1.globalFadeToClear();
          return true;
        }
        if (!this.naming)
        {
          Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.returnBabyName), Game1.content.LoadString(this.isMale ? "Strings\\Events:BabyNamingTitle_Male" : "Strings\\Events:BabyNamingTitle_Female"), "");
          this.naming = true;
        }
        if (this.babyName != null && this.babyName != "" && this.babyName.Length > 0)
        {
          string babyName = this.babyName;
          DisposableList<NPC> allCharacters = Utility.getAllCharacters();
          bool flag;
          do
          {
            flag = false;
            foreach (Character character in allCharacters)
            {
              if (character.name.Equals((object) babyName))
              {
                babyName += " ";
                flag = true;
                break;
              }
            }
          }
          while (flag);
          this.child.Name = babyName;
          Game1.playSound("smallSelect");
          if (Game1.keyboardDispatcher != null)
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
          Game1.globalFadeToClear();
          return true;
        }
      }
      return false;
    }

    public void draw(SpriteBatch b)
    {
    }

    public void makeChangesToLocation()
    {
    }

    public void drawAboveEverything(SpriteBatch b)
    {
    }
  }
}
