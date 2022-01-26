// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.QuestionEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace StardewValley.Events
{
  public class QuestionEvent : FarmEvent, INetObject<NetFields>
  {
    public const int pregnancyQuestion = 1;
    public const int barnBirth = 2;
    public const int playerPregnancyQuestion = 3;
    private int whichQuestion;
    private AnimalHouse animalHouse;
    public FarmAnimal animal;
    public bool forceProceed;

    public NetFields NetFields { get; } = new NetFields();

    public QuestionEvent(int whichQuestion) => this.whichQuestion = whichQuestion;

    public bool setUp()
    {
      switch (this.whichQuestion)
      {
        case 1:
          Response[] answerChoices1 = new Response[2]
          {
            new Response("Yes", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_Yes")),
            new Response("Not", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_No"))
          };
          if (!Game1.getCharacterFromName(Game1.player.spouse).isGaySpouse())
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HaveBabyQuestion", (object) Game1.player.Name), answerChoices1, new GameLocation.afterQuestionBehavior(this.answerPregnancyQuestion), Game1.getCharacterFromName(Game1.player.spouse));
          else
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HaveBabyQuestion_Adoption", (object) Game1.player.Name), answerChoices1, new GameLocation.afterQuestionBehavior(this.answerPregnancyQuestion), Game1.getCharacterFromName(Game1.player.spouse));
          Game1.messagePause = true;
          return false;
        case 2:
          FarmAnimal farmAnimal = (FarmAnimal) null;
          foreach (Building building in Game1.getFarm().buildings)
          {
            if ((building.owner.Equals((object) Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && building.buildingType.Contains("Barn") && !building.buildingType.Equals((object) "Barn") && !(building.indoors.Value as AnimalHouse).isFull() && Game1.random.NextDouble() < (double) (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * 0.0055)
            {
              farmAnimal = Utility.getAnimal((building.indoors.Value as AnimalHouse).animalsThatLiveHere[Game1.random.Next((building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count)]);
              this.animalHouse = building.indoors.Value as AnimalHouse;
              break;
            }
          }
          if (farmAnimal != null && !farmAnimal.isBaby() && (bool) (NetFieldBase<bool, NetBool>) farmAnimal.allowReproduction && farmAnimal.CanHavePregnancy())
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:AnimalBirth", (object) farmAnimal.displayName, (object) farmAnimal.shortDisplayType()));
            Game1.messagePause = true;
            this.animal = farmAnimal;
            return false;
          }
          break;
        case 3:
          Response[] answerChoices2 = new Response[2]
          {
            new Response("Yes", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_Yes")),
            new Response("Not", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_No"))
          };
          long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
          Farmer otherFarmer = Game1.otherFarmers[key];
          if (otherFarmer.IsMale != Game1.player.IsMale)
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HavePlayerBabyQuestion", (object) otherFarmer.Name), answerChoices2, new GameLocation.afterQuestionBehavior(this.answerPlayerPregnancyQuestion));
          else
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HavePlayerBabyQuestion_Adoption", (object) otherFarmer.Name), answerChoices2, new GameLocation.afterQuestionBehavior(this.answerPlayerPregnancyQuestion));
          Game1.messagePause = true;
          return false;
      }
      return true;
    }

    private void answerPregnancyQuestion(Farmer who, string answer)
    {
      if (!answer.Equals("Yes"))
        return;
      WorldDate worldDate = new WorldDate(Game1.Date);
      worldDate.TotalDays += 14;
      who.GetSpouseFriendship().NextBirthingDate = worldDate;
      Game1.getCharacterFromName(who.spouse).isGaySpouse();
    }

    private void answerPlayerPregnancyQuestion(Farmer who, string answer)
    {
      if (!answer.Equals("Yes"))
        return;
      long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
      Game1.player.team.SendProposal(Game1.otherFarmers[key], ProposalType.Baby);
    }

    public bool tickUpdate(GameTime time)
    {
      if (this.forceProceed)
        return true;
      if (this.whichQuestion != 2 || Game1.dialogueUp)
        return !Game1.dialogueUp;
      if (Game1.activeClickableMenu == null)
        Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.animalHouse.addNewHatchedAnimal), this.animal != null ? Game1.content.LoadString("Strings\\Events:AnimalNamingTitle", (object) this.animal.displayType) : Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestionEvent.cs.6692"));
      return false;
    }

    public void draw(SpriteBatch b)
    {
    }

    public void drawAboveEverything(SpriteBatch b)
    {
    }

    public void makeChangesToLocation() => Game1.messagePause = false;
  }
}
