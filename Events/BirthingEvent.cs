// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.BirthingEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StardewValley.Events
{
  public class BirthingEvent : FarmEvent, INetObject<NetFields>
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

    public NetFields NetFields { get; } = new NetFields();

    public bool setUp()
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      NPC characterFromName = Game1.getCharacterFromName(Game1.player.spouse);
      Game1.player.CanMove = false;
      this.isMale = Game1.player.getNumberOfChildren() != 0 ? Game1.player.getChildren()[0].Gender == 1 : random.NextDouble() < 0.5;
      this.message = !characterFromName.isGaySpouse() ? (characterFromName.Gender != 0 ? Game1.content.LoadString("Strings\\Events:BirthMessage_SpouseMother", (object) Lexicon.getGenderedChildTerm(this.isMale), (object) characterFromName.displayName) : Game1.content.LoadString("Strings\\Events:BirthMessage_PlayerMother", (object) Lexicon.getGenderedChildTerm(this.isMale))) : Game1.content.LoadString("Strings\\Events:BirthMessage_Adoption", (object) Lexicon.getGenderedChildTerm(this.isMale));
      return false;
    }

    public void returnBabyName(string name)
    {
      this.babyName = name;
      Game1.exitActiveMenu();
    }

    public void afterMessage() => this.getBabyName = true;

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
        if (!this.naming)
        {
          Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.returnBabyName), Game1.content.LoadString(this.isMale ? "Strings\\Events:BabyNamingTitle_Male" : "Strings\\Events:BabyNamingTitle_Female"), "");
          this.naming = true;
        }
        if (this.babyName != null && this.babyName != "" && this.babyName.Length > 0)
        {
          double num = (Game1.player.spouse.Equals("Maru") ? 0.5 : 0.0) + (Game1.player.hasDarkSkin() ? 0.5 : 0.0);
          bool isDarkSkinned = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed).NextDouble() < num;
          string babyName = this.babyName;
          Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
          DisposableList<NPC> allCharacters = Utility.getAllCharacters();
          bool flag;
          do
          {
            flag = false;
            if (dictionary.ContainsKey(babyName))
            {
              babyName += " ";
              flag = true;
            }
            else
            {
              foreach (Character character in allCharacters)
              {
                if (character.name.Equals((object) babyName))
                {
                  babyName += " ";
                  flag = true;
                }
              }
            }
          }
          while (flag);
          Child baby = new Child(babyName, this.isMale, isDarkSkinned, Game1.player);
          baby.Age = 0;
          baby.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
          Utility.getHomeOfFarmer(Game1.player).characters.Add((NPC) baby);
          Game1.playSound("smallSelect");
          Game1.player.getSpouse().daysAfterLastBirth = 5;
          Game1.player.GetSpouseFriendship().NextBirthingDate = (WorldDate) null;
          if (Game1.player.getChildrenCount() == 2)
          {
            Game1.player.getSpouse().shouldSayMarriageDialogue.Value = true;
            Game1.player.getSpouse().currentMarriageDialogue.Insert(0, new MarriageDialogueReference("Data\\ExtraDialogue", "NewChild_SecondChild" + Game1.random.Next(1, 3).ToString(), true, Array.Empty<string>()));
            Game1.getSteamAchievement("Achievement_FullHouse");
          }
          else if (Game1.player.getSpouse().isGaySpouse())
            Game1.player.getSpouse().currentMarriageDialogue.Insert(0, new MarriageDialogueReference("Data\\ExtraDialogue", "NewChild_Adoption", true, new string[1]
            {
              this.babyName
            }));
          else
            Game1.player.getSpouse().currentMarriageDialogue.Insert(0, new MarriageDialogueReference("Data\\ExtraDialogue", "NewChild_FirstChild", true, new string[1]
            {
              this.babyName
            }));
          Game1.morningQueue.Enqueue((DelayedAction.delayedBehavior) (() => Game1.multiplayer.globalChatInfoMessage("Baby", Lexicon.capitalize(Game1.player.Name), Game1.player.spouse, Lexicon.getGenderedChildTerm(this.isMale), Lexicon.getPronoun(this.isMale), baby.displayName)));
          if (Game1.keyboardDispatcher != null)
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
          Game1.player.Position = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).GetPlayerBedSpot()) * 64f;
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
