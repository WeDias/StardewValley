// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.DiaryEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;

namespace StardewValley.Events
{
  public class DiaryEvent : FarmEvent, INetObject<NetFields>
  {
    public string NPCname;

    public NetFields NetFields { get; } = new NetFields();

    /// <summary>
    /// return true if the event wasn't able to set up and should be skipped
    /// </summary>
    public bool setUp()
    {
      if (Game1.player.isMarried())
        return true;
      foreach (string str1 in (NetList<string, NetString>) Game1.player.mailReceived)
      {
        if (str1.Contains("diary"))
        {
          string str2 = str1.Split('_')[1];
          if (!Game1.player.mailReceived.Contains("diary_" + str2 + "_finished"))
          {
            Convert.ToInt32(str2.Split('/')[1]);
            this.NPCname = str2.Split('/')[0];
            NPC characterFromName = Game1.getCharacterFromName(this.NPCname);
            int gender = characterFromName.Gender;
            Game1.player.mailReceived.Add("diary_" + str2 + "_finished");
            string text = (Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DiaryEvent.cs.6658") : Game1.content.LoadString("Strings\\StringsFromCSFiles:DiaryEvent.cs.6660")) + Environment.NewLine + Environment.NewLine + "-" + Utility.capitalizeFirstLetter(Game1.CurrentSeasonDisplayName) + " " + Game1.dayOfMonth.ToString() + "-" + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:DiaryEvent.cs.6664", (object) this.NPCname);
            Response[] answerChoices = new Response[3]
            {
              new Response("...We're", Game1.content.LoadString("Strings\\StringsFromCSFiles:DiaryEvent.cs.6667")),
              new Response("...I", (int) (NetFieldBase<int, NetInt>) characterFromName.gender == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DiaryEvent.cs.6669") : Game1.content.LoadString("Strings\\StringsFromCSFiles:DiaryEvent.cs.6670")),
              new Response("(Write", Game1.content.LoadString("Strings\\StringsFromCSFiles:DiaryEvent.cs.6672"))
            };
            Game1.currentLocation.createQuestionDialogue(Game1.parseText(text), answerChoices, "diary");
            Game1.messagePause = true;
            return false;
          }
        }
      }
      return true;
    }

    public bool tickUpdate(GameTime time) => !Game1.dialogueUp;

    public void draw(SpriteBatch b)
    {
    }

    public void makeChangesToLocation() => Game1.messagePause = false;

    public void drawAboveEverything(SpriteBatch b)
    {
    }
  }
}
