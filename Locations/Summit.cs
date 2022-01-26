// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Summit
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace StardewValley.Locations
{
  public class Summit : GameLocation
  {
    private ICue wind;
    private float windGust;
    private float globalWind = -0.25f;
    [XmlIgnore]
    public bool isShowingEndSlideshow;

    public Summit()
    {
    }

    public Summit(string map, string name)
      : base(map, name)
    {
    }

    public override void checkForMusic(GameTime time)
    {
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (Game1.random.NextDouble() < 0.005 || (double) this.globalWind >= 1.0 || (double) this.globalWind <= 0.349999994039536)
        this.windGust = (double) this.globalWind >= 0.349999994039536 ? ((double) this.globalWind <= 0.75 ? (float) ((Game1.random.NextDouble() < 0.5 ? -1 : 1) * Game1.random.Next(4, 6)) / 2000f : (float) -Game1.random.Next(2, 6) / 2000f) : (float) Game1.random.Next(3, 6) / 2000f;
      if (this.wind != null)
      {
        this.globalWind += this.windGust;
        this.globalWind = Utility.Clamp(this.globalWind, -0.5f, 1f);
        this.wind.SetVariable("Volume", Math.Abs(this.globalWind) * 60f);
        this.wind.SetVariable("Frequency", this.globalWind * 100f);
        this.wind.SetVariable("Pitch", (float) (1200.0 + (double) Math.Abs(this.globalWind) * 1200.0));
      }
      base.UpdateWhenCurrentLocation(time);
      if (this.temporarySprites.Count == 0 && Game1.random.NextDouble() < (Game1.timeOfDay >= 1800 ? (!Game1.currentSeason.Equals("summer") || Game1.dayOfMonth != 20 ? 0.001 : 1.0) : 0.0006))
      {
        Rectangle sourceRect = Rectangle.Empty;
        Vector2 position = new Vector2((float) Game1.viewport.Width, (float) Game1.random.Next(10, Game1.viewport.Height / 2));
        float x = -4f;
        int numberOfLoops = 200;
        float animationInterval = 100f;
        if (Game1.timeOfDay < 1800)
        {
          if (Game1.currentSeason.Equals("spring") || Game1.currentSeason.Equals("fall"))
          {
            sourceRect = new Rectangle(640, 736, 16, 16);
            int num = Game1.random.Next(1, 4);
            x = -1f;
            for (int index = 0; index < num; ++index)
            {
              TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(80, 121), 4, 200, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * -20)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f
              };
              temporaryAnimatedSprite1.motion = new Vector2(-1f, 0.0f);
              this.temporarySprites.Add(temporaryAnimatedSprite1);
              TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(80, 121), 4, 200, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * 20)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f
              };
              temporaryAnimatedSprite2.motion = new Vector2(-1f, 0.0f);
              this.temporarySprites.Add(temporaryAnimatedSprite2);
            }
          }
          else if (Game1.currentSeason.Equals("summer"))
          {
            sourceRect = new Rectangle(640, 752 + (Game1.random.NextDouble() < 0.5 ? 16 : 0), 16, 16);
            x = -0.5f;
            animationInterval = 150f;
          }
          if (Game1.random.NextDouble() < 1.25)
          {
            string currentSeason = Game1.currentSeason;
            TemporaryAnimatedSprite temporaryAnimatedSprite;
            if (!(currentSeason == "spring"))
            {
              if (!(currentSeason == "summer"))
              {
                if (!(currentSeason == "fall"))
                {
                  if (currentSeason == "winter")
                    temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(104, 302, 26, 18), (float) Game1.random.Next(80, 121), 4, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      layerDepth = 0.0f,
                      pingPong = true
                    };
                  else
                    temporaryAnimatedSprite = new TemporaryAnimatedSprite();
                }
                else
                  temporaryAnimatedSprite = new TemporaryAnimatedSprite("TileSheets\\critters", new Rectangle(0, 64, 32, 32), (float) Game1.random.Next(60, 80), 5, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                  {
                    layerDepth = 0.0f,
                    pingPong = true
                  };
              }
              else
                temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(1, 165, 24, 21), (float) Game1.random.Next(60, 80), 6, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                {
                  layerDepth = 0.0f
                };
            }
            else
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(0, 302, 26, 18), (float) Game1.random.Next(80, 121), 4, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f,
                pingPong = true
              };
            temporaryAnimatedSprite.motion = new Vector2(-3f, 0.0f);
            this.temporarySprites.Add(temporaryAnimatedSprite);
          }
          else if (Game1.random.NextDouble() < 0.15 && Game1.stats.getStat("childrenTurnedToDoves") > 1U)
          {
            for (int index = 0; (long) index < (long) Game1.stats.getStat("childrenTurnedToDoves"); ++index)
            {
              sourceRect = Rectangle.Empty;
              TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(388, 1894, 24, 21), (float) Game1.random.Next(80, 121), 6, 200, position + new Vector2((float) ((index + 1) * (Game1.random.Next(25, 27) * 4)), (float) (Game1.random.Next(-32, 33) * 4)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f
              };
              temporaryAnimatedSprite.motion = new Vector2(-3f, 0.0f);
              this.temporarySprites.Add(temporaryAnimatedSprite);
            }
          }
          if (Game1.MasterPlayer.eventsSeen.Contains(571102) && Game1.random.NextDouble() < 0.1)
          {
            sourceRect = Rectangle.Empty;
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(222, 1890, 20, 9), 30f, 2, 99900, position, false, false, 0.01f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f, true)
            {
              yPeriodic = true,
              yPeriodicLoopTime = 4000f,
              yPeriodicRange = 8f,
              layerDepth = 0.0f
            };
            temporaryAnimatedSprite.motion = new Vector2(-3f, 0.0f);
            this.temporarySprites.Add(temporaryAnimatedSprite);
          }
          if (Game1.MasterPlayer.eventsSeen.Contains(10) && Game1.random.NextDouble() < 0.05)
          {
            sourceRect = Rectangle.Empty;
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(206, 1827, 15, 25), 30f, 4, 99900, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              rotation = -1.047198f,
              layerDepth = 0.0f
            };
            temporaryAnimatedSprite.motion = new Vector2(-4f, -0.5f);
            this.temporarySprites.Add(temporaryAnimatedSprite);
          }
        }
        else if (Game1.timeOfDay >= 1900)
        {
          sourceRect = new Rectangle(640, 816, 16, 16);
          x = -2f;
          numberOfLoops = 0;
          position.X -= (float) Game1.random.Next(64, Game1.viewport.Width);
          if (Game1.currentSeason.Equals("summer") && Game1.dayOfMonth == 20)
          {
            int num = Game1.random.Next(3);
            for (int index = 0; index < num; ++index)
            {
              TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(80, 121), Game1.currentSeason.Equals("winter") ? 2 : 4, numberOfLoops, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f
              };
              temporaryAnimatedSprite.motion = new Vector2(x, 0.0f);
              this.temporarySprites.Add(temporaryAnimatedSprite);
              position.X -= (float) Game1.random.Next(64, Game1.viewport.Width);
              position.Y = (float) Game1.random.Next(0, 200);
            }
          }
          else if (Game1.currentSeason.Equals("winter") && Game1.timeOfDay >= 1700 && Game1.random.NextDouble() < 0.1)
          {
            sourceRect = new Rectangle(640, 800, 32, 16);
            numberOfLoops = 1000;
            position.X = (float) Game1.viewport.Width;
          }
          else if (Game1.currentSeason.Equals("winter"))
            sourceRect = Rectangle.Empty;
        }
        if (Game1.timeOfDay >= 2200 && !Game1.currentSeason.Equals("winter") && Game1.currentSeason.Equals("summer") && Game1.dayOfMonth == 20 && Game1.random.NextDouble() < 0.05)
        {
          sourceRect = new Rectangle(640, 784, 16, 16);
          numberOfLoops = 200;
          position.X = (float) Game1.viewport.Width;
          x = -3f;
        }
        if (!sourceRect.Equals(Rectangle.Empty) && Game1.viewport.X > -10000)
        {
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, animationInterval, Game1.currentSeason.Equals("winter") ? 2 : 4, numberOfLoops, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            layerDepth = 0.0f
          };
          temporaryAnimatedSprite.motion = new Vector2(x, 0.0f);
          this.temporarySprites.Add(temporaryAnimatedSprite);
        }
      }
      if (Game1.viewport.X > -10000)
      {
        foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
        {
          temporarySprite.position.Y -= (float) (((double) Game1.viewport.Y - (double) Game1.previousViewportPosition.Y) / 8.0);
          temporarySprite.drawAboveAlwaysFront = true;
        }
      }
      if (Game1.eventUp)
      {
        foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
        {
          if (temporarySprite.attachedCharacter != null)
            temporarySprite.attachedCharacter.animateInFacingDirection(time);
        }
      }
      else
        this.isShowingEndSlideshow = false;
    }

    public override void cleanupBeforePlayerExit()
    {
      this.isShowingEndSlideshow = false;
      base.cleanupBeforePlayerExit();
      Game1.background = (Background) null;
      Game1.displayHUD = true;
      if (this.wind == null)
        return;
      this.wind.Stop(AudioStopOptions.Immediate);
    }

    protected override void resetLocalState()
    {
      this.isShowingEndSlideshow = false;
      this.isOutdoors.Value = false;
      base.resetLocalState();
      Game1.background = new Background();
      this.temporarySprites.Clear();
      Game1.displayHUD = false;
      Game1.changeMusicTrack("winter_day_ambient", true, Game1.MusicContext.SubLocation);
      this.wind = Game1.soundBank.GetCue("wind");
      this.wind.Play();
      this.globalWind = 0.0f;
      this.windGust = 1f / 1000f;
      if (Game1.player.mailReceived.Contains("Summit_event") || !Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") || !(this.getSummitEvent() != ""))
        return;
      if (!Game1.player.songsHeard.Contains("end_credits"))
        Game1.player.songsHeard.Add("end_credits");
      Game1.player.mailReceived.Add("Summit_event");
      this.startEvent(new StardewValley.Event(this.getSummitEvent()));
    }

    public string GetSummitDialogue(string file, string key)
    {
      string path = "Data\\" + file + ":" + key;
      return Game1.player.getSpouse() != null && Game1.player.getSpouse().Name == "Penny" ? Game1.content.LoadString(path, (object) "요") : Game1.content.LoadString(path, (object) "");
    }

    private string getSummitEvent()
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        stringBuilder.Append("winter_day_ambient/-1000 -1000/farmer 9 23 0 ");
        if (Game1.player.isMarried() && Game1.player.getSpouse() != null && Game1.player.getSpouse().Name != "Krobus")
          stringBuilder.Append(Game1.player.getSpouse().Name + " 11 13 0/skippable/viewport 10 17 clamp true/pause 2000/viewport move 0 -1 4000/move farmer 0 -10 0/move farmer 1 0 0/pause 2000/speak " + Game1.player.getSpouse().Name + " \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro_Spouse") + "\"/viewport move 0 -1 4000/pause 5000/speak " + Game1.player.getSpouse().Name + " \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro2_Spouse" + (this.sayGrufferSummitIntro(Game1.player.getSpouse()) ? "_Gruff" : "")) + "\"/pause 400/emote farmer 56/pause 2000/speak " + Game1.player.getSpouse().Name + " \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue1_Spouse") + "\"/pause 2000/faceDirection " + Game1.player.getSpouse().Name + " 3/faceDirection farmer 1/pause 1000/speak " + Game1.player.getSpouse().Name + " \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue2_Spouse") + "\"/pause 2000/faceDirection " + Game1.player.getSpouse().Name + " 0/faceDirection farmer 0/pause 2000/speak " + Game1.player.getSpouse().Name + " \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue3_" + Game1.player.getSpouse().Name) + "\"/emote farmer 20/pause 500/faceDirection farmer 1/faceDirection " + Game1.player.getSpouse().Name + " 3/pause 1500/animate farmer false true 100 101/showKissFrame " + Game1.player.getSpouse().Name + "/playSound dwop/positionOffset farmer 8 0/positionOffset " + Game1.player.getSpouse().Name + " -4 0/specificTemporarySprite heart 11 12/pause 10");
        else if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
          stringBuilder.Append("Morris 11 13 0/skippable/viewport 10 17 clamp true/pause 2000/viewport move 0 -1 4000/move farmer 0 -10 0/pause 2000/speak Morris \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro_Morris") + "\"/viewport move 0 -1 4000/pause 5000/speak Morris \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue1_Morris") + "\"/pause 2000/faceDirection Morris 3/speak Morris \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue2_Morris") + "\"/pause 2000/faceDirection Morris 0/speak Morris \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Outro_Morris") + "\"/emote farmer 20/pause 10");
        else
          stringBuilder.Append("Lewis 11 13 0/skippable/viewport 10 17 clamp true/pause 2000/viewport move 0 -1 4000/move farmer 0 -10 0/pause 2000/speak Lewis \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro_Lewis") + "\"/viewport move 0 -1 4000/pause 5000/speak Lewis \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue1_Lewis") + "\"/pause 2000/faceDirection Lewis 3/speak Lewis \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue2_Lewis") + "\"/pause 2000/faceDirection Lewis 0/speak Lewis \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Outro_Lewis") + "\"/pause 10");
        int num = 35000;
        if (Game1.player.mailReceived.Contains("Broken_Capsule"))
          num += 8000;
        if (Game1.player.totalMoneyEarned >= 100000000U)
          num += 8000;
        if (Game1.year <= 2)
          num += 8000;
        stringBuilder.Append("/playMusic moonlightJellies/pause 2000/specificTemporarySprite krobusraven/viewport move 0 -1 12000/pause 10/pause " + num.ToString() + "/pause 2000/playMusic none/viewport move 0 -1 5000/fade/playMusic end_credits/viewport -8000 -8000 true/removeTemporarySprites/specificTemporarySprite getEndSlideshow/pause 1000/playMusic none/pause 500");
        stringBuilder.Append("/playMusic grandpas_theme/pause 2000/fade/viewport -3000 -2000/specificTemporarySprite doneWithSlideShow/removeTemporarySprites/pause 3000/addTemporaryActor MrQi 16 32 -998 -1000 2 true/addTemporaryActor Grandpa 1 1 -100 -100 2 true/specificTemporarySprite grandpaSpirit/viewport -1000 -1000 true/pause 6000/spriteText 3 \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage") + " \"/spriteText 3 \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage2") + " \"/spriteText 3 \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage3") + " \"/spriteText 3 \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage4") + " \"/spriteText 7 \"" + this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage5") + " \"/pause 400/playSound dwop/showFrame MrQi 1/pause 100/showFrame MrQi 2/pause 100/showFrame MrQi 3/pause 400/specificTemporarySprite grandpaThumbsUp/pause 10000/end");
      }
      catch (Exception ex)
      {
        return "";
      }
      return stringBuilder.ToString();
    }

    public string getEndSlideshow()
    {
      StringBuilder stringBuilder = new StringBuilder();
      Dictionary<string, string> dictionary1 = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      int num1 = 0;
      foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
      {
        try
        {
          if (!(keyValuePair.Key == "Marlon"))
          {
            if (!(keyValuePair.Key == "Krobus"))
            {
              if (!(keyValuePair.Key == "Dwarf"))
              {
                if (!(keyValuePair.Key == "Sandy"))
                {
                  if (!(keyValuePair.Key == "Wizard"))
                  {
                    string str = keyValuePair.Key;
                    if (keyValuePair.Key == "Leo")
                      str = "ParrotBoy";
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\" + str, new Rectangle(0, 96, 16, 32), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.4f), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num1
                    });
                    num1 += 500;
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 2000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 488), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 2000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 976), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 2000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 1464), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 2000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(324, 1936, 12, 20), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.400000005960464 + 192.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 14000,
        startSound = "dogWhining"
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(43, 80, 51, 56), 90f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-1f, -4f),
        delayBeforeAnimationStart = 27000,
        startSound = "trashbear",
        drawAboveAlwaysFront = true
      });
      stringBuilder.Append("pause 10/spriteText 5 \"" + Utility.loadStringShort("UI", "EndCredit_Neighbors") + " \"/pause 30000/");
      int num2 = num1 + 4000;
      int num3 = num2;
      foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
      {
        if (keyValuePair.Key == "Krobus" || keyValuePair.Key == "Dwarf" || keyValuePair.Key == "Sandy" || keyValuePair.Key == "Wizard")
        {
          int height = 32;
          if (keyValuePair.Key == "Krobus" || keyValuePair.Key == "Dwarf")
            height = 24;
          this.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\" + keyValuePair.Key, new Rectangle(0, height * 3, 16, height), 120f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.4f + (float) ((32 - height) * 4)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num2
          });
          num2 += 500;
        }
      }
      int num4 = num2 + 5000;
      stringBuilder.Append("spriteText 4 \"" + Utility.loadStringShort("UI", "EndCredit_Animals") + " \"/pause " + (num4 - num3 + 22000).ToString());
      int num5 = num4;
      foreach (KeyValuePair<string, string> keyValuePair in Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals"))
      {
        if (!(keyValuePair.Key == "Hog") && !(keyValuePair.Key == "Brown Cow"))
        {
          int int32_1 = Convert.ToInt32(keyValuePair.Value.Split('/')[16]);
          int int32_2 = Convert.ToInt32(keyValuePair.Value.Split('/')[17]);
          int num6 = 0;
          this.TemporarySprites.Add(new TemporaryAnimatedSprite("Animals\\" + keyValuePair.Key, new Rectangle(0, int32_2, int32_1, int32_2), 120f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) (int) ((double) Game1.viewport.Height * 0.5 - (double) (int32_2 * 4))), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num4
          });
          int num7 = num6 + int32_1 * 4;
          int num8 = int32_1 > 16 ? 4 : 0;
          try
          {
            string str = "Baby" + keyValuePair.Key;
            if (keyValuePair.Key == "Duck")
              str = "BabyWhite Chicken";
            else if (keyValuePair.Key == "Dinosaur")
              str = "Dinosaur";
            Game1.content.Load<Texture2D>("Animals\\" + str);
            this.TemporarySprites.Add(new TemporaryAnimatedSprite("Animals\\" + str, new Rectangle(0, int32_2, int32_1, int32_2), 90f, 4, 999999, new Vector2((float) (Game1.viewport.Width + (int32_1 + 2 + num8) * 4), (float) (int) ((double) Game1.viewport.Height * 0.5 - (double) (int32_2 * 4))), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = num4
            });
            this.TemporarySprites.Add(new TemporaryAnimatedSprite("Animals\\" + str, new Rectangle(0, int32_2, int32_1, int32_2), 90f, 4, 999999, new Vector2((float) (Game1.viewport.Width + (int32_1 + 2 + num8) * 2 * 4), (float) (int) ((double) Game1.viewport.Height * 0.5 - (double) (int32_2 * 4))), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = num4
            });
            num7 += (int32_1 + 2 + num8) * 4 * 2;
          }
          catch (Exception ex)
          {
          }
          this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(0, int32_2, int32_1, int32_2), 120f, 1, 999999, new Vector2((float) (Game1.viewport.Width + num7 / 2) - Game1.dialogueFont.MeasureString(keyValuePair.Value.Split('/')[25]).X / 2f, (float) (int) ((double) Game1.viewport.Height * 0.5 + 12.0)), false, true, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num4,
            text = keyValuePair.Value.Split('/')[25]
          });
          num4 += 2000 + num8 * 300;
        }
      }
      if (Game1.player.catPerson)
        this.TemporarySprites.Add(new TemporaryAnimatedSprite("Animals\\cat" + (Game1.player.whichPetBreed != 0 ? Game1.player.whichPetBreed.ToString() ?? "" : ""), new Rectangle(0, 96, 32, 32), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 320.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
        {
          motion = new Vector2(-4f, 0.0f),
          delayBeforeAnimationStart = 38000,
          startSound = "cat"
        });
      else
        this.TemporarySprites.Add(new TemporaryAnimatedSprite("Animals\\dog" + (Game1.player.whichPetBreed != 0 ? Game1.player.whichPetBreed.ToString() ?? "" : ""), new Rectangle(0, 256, 32, 32), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 320.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
        {
          motion = new Vector2(-5f, 0.0f),
          delayBeforeAnimationStart = 38000,
          startSound = "dog_bark",
          pingPong = true
        });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(64, 192, 32, 32), 90f, 6, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 128.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 45000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128, 160, 32, 32), 90f, 6, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 128.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 47000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128, 224, 32, 32), 90f, 6, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 128.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 48000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(32, 160, 32, 32), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 320.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 49000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(32, 160, 32, 32), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 288.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 49500,
        pingPong = true
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(34, 98, 32, 32), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 50000,
        pingPong = true
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 32, 32, 32), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 50500,
        pingPong = true
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128, 96, 16, 16), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 55000,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(192, 96, 16, 16), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.399993896484)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 55300,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(256, 96, 16, 16), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 345.600006103516)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 55600,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 128, 16, 16), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 57000,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(48, 144, 16, 16), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.399993896484)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 57300,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(96, 144, 16, 16), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 345.600006103516)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 57600,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(192, 288, 16, 16), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 345.600006103516)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 58000,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128, 288, 16, 16), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.399993896484)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 58300,
        pingPong = true,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 3000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 224, 16, 16), 90f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 64.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 54000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 240, 16, 16), 90f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 64.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 55000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(67, 190, 24, 51), 90f, 3, 999999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, -4f),
        delayBeforeAnimationStart = 68000,
        rotation = -0.1963495f,
        pingPong = true,
        drawAboveAlwaysFront = true
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(0, 0, 57, 70), 150f, 2, 999999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, -4f),
        delayBeforeAnimationStart = 69000,
        rotation = -0.1963495f,
        drawAboveAlwaysFront = true
      });
      stringBuilder.Append("/spriteText 1 \"" + Utility.loadStringShort("UI", "EndCredit_Fish") + " \"/pause " + (num4 - num5 + 18000).ToString());
      int num9 = num4 + 6000;
      int num10 = num9;
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 70000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 86000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 91000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(140, 78, 28, 38), 250f, 2, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 152.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 102000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 75000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", new Rectangle(0, 287, 47, 14), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 56.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 82000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", new Rectangle(0, 287, 47, 14), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 56.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 80000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", new Rectangle(0, 287, 47, 14), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 56.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 84000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(132, 20, 8, 8), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 48.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 81500,
        yPeriodic = true,
        yPeriodicRange = 21f,
        yPeriodicLoopTime = 5000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(140, 20, 8, 8), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 48.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 83500,
        yPeriodic = true,
        yPeriodicRange = 21f,
        yPeriodicLoopTime = 5000f
      });
      Dictionary<int, string> dictionary2 = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
      Dictionary<int, string> dictionary3 = Game1.content.Load<Dictionary<int, string>>("Data\\AquariumFish");
      int num11 = 0;
      foreach (KeyValuePair<int, string> keyValuePair in dictionary2)
      {
        try
        {
          int int32 = Convert.ToInt32(dictionary3[keyValuePair.Key].Split('/')[0]);
          Rectangle sourceRect = new Rectangle(24 * int32 % 480, 24 * int32 / 480 * 48, 24, 24);
          float x = Game1.dialogueFont.MeasureString(Game1.IsEnglish() ? keyValuePair.Value.Split('/')[0] : keyValuePair.Value.Split('/')[13]).X;
          this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", sourceRect, 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 192), (float) (int) ((double) Game1.viewport.Height * 0.529999971389771 - (double) (num11 * 64) * 2.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num9,
            yPeriodic = true,
            yPeriodicLoopTime = (float) Game1.random.Next(1500, 2100),
            yPeriodicRange = 4f
          });
          this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", sourceRect, 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 192 + 48) - x / 2f, (float) (int) ((double) Game1.viewport.Height * 0.529999971389771 - (double) (num11 * 64) * 2.0 + 64.0 + 16.0)), false, true, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num9,
            text = Game1.IsEnglish() ? keyValuePair.Value.Split('/')[0] : keyValuePair.Value.Split('/')[13]
          });
          ++num11;
          if (num11 == 4)
          {
            num9 += 2000;
            num11 = 0;
          }
        }
        catch (Exception ex)
        {
        }
      }
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\projectiles", new Rectangle(64, 0, 16, 16), 909f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-6f, 0.0f),
        delayBeforeAnimationStart = 123000,
        rotationChange = -0.1f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\projectiles", new Rectangle(64, 0, 16, 16), 909f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 339.200012207031)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-6f, 0.0f),
        delayBeforeAnimationStart = 123300,
        rotationChange = -0.1f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 640, 69), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 108000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 640, 69), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 2564), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 108000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 640, 69), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 5128), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 108000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 300, 69), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 7692), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 108000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 0, 31, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 110000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(65, 0, 31, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 115000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(96, 90, 31, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 118000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 176, 104, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 121000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(32, 320, 32, 23), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 92.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 124000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(31, 58, 67, 23), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 92.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 127000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 98, 32, 23), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 92.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 132000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(49, 131, 47, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 137000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 0, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 113000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 20, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 116000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 40, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 119000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 60, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 126000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 120, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 129000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 100, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 134000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 120, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 139000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\upperCavePlants", new Rectangle(0, 0, 48, 21), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 84.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 142000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\upperCavePlants", new Rectangle(96, 0, 48, 21), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 84.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 146000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(2, 123, 19, 24), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 145000,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 2500f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(2, 123, 19, 24), 100f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.399993896484)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 142500,
        yPeriodic = true,
        yPeriodicRange = 8f,
        yPeriodicLoopTime = 2000f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 0, 31, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 149000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(65, 0, 31, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 151000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(96, 90, 31, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 154000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 176, 104, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 156000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 0, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 155000
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 20, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 152500
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 40, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 158000
      });
      if (Game1.player.favoriteThing.Value != null && Game1.player.favoriteThing.Value.ToLower().Equals("concernedape"))
        this.TemporarySprites.Add(new TemporaryAnimatedSprite("Minigames\\Clouds", new Rectangle(210, 842, 138, 130), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 240.0)), false, false, 0.7f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
        {
          motion = new Vector2(-3f, 0.0f),
          delayBeforeAnimationStart = 160500,
          startSound = "discoverMineral"
        });
      stringBuilder.Append("/spriteText 2 \"" + Utility.loadStringShort("UI", "EndCredit_Monsters") + " \"/pause " + (num9 - num10 + 19000).ToString());
      int num12 = num9 + 6000;
      foreach (KeyValuePair<string, string> keyValuePair in Game1.content.Load<Dictionary<string, string>>("Data\\Monsters"))
      {
        if (!(keyValuePair.Key == "Fireball") && !(keyValuePair.Key == "Skeleton Warrior"))
        {
          int height1 = 16;
          int width1 = 16;
          int num13 = 0;
          int animationLength = 4;
          bool flag = false;
          int num14 = 0;
          Character character1 = (Character) null;
          if (keyValuePair.Key.Contains("Bat") || keyValuePair.Key.Contains("Ghost"))
            height1 = 24;
          switch (keyValuePair.Key)
          {
            case "Bat":
              Texture2D texture2D1 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Frost Bat");
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D1.Width, width1 * num13 / texture2D1.Width * height1, width1, height1), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height1 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character1,
                texture = texture2D1
              });
              Texture2D texture2D2 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Lava Bat");
              int height2 = 24;
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D2.Width, width1 * num13 / texture2D2.Width * height2, width1, height2), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 96f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height2 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character1,
                texture = texture2D2
              });
              Texture2D texture2D3 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Iridium Bat");
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D3.Width, width1 * num13 / texture2D3.Width * height2, width1, height2), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 288f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height2 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character1,
                texture = texture2D3
              });
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D3.Width, width1 * num13 / texture2D3.Width * height2, width1, height2), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 128 + width1 * 4 / 2) - Game1.dialogueFont.MeasureString(keyValuePair.Value.Split('/')[14]).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                text = Utility.loadStringShort("UI", "EndCredit_Bats")
              });
              num12 += 1500;
              continue;
            case "Big Slime":
              height1 = 32;
              width1 = 32;
              num14 = 64;
              character1 = (Character) new BigSlime(Vector2.Zero, 0);
              break;
            case "Blue Squid":
              width1 = 24;
              height1 = 24;
              animationLength = 5;
              break;
            case "Carbon Ghost":
            case "Dwarvish Sentry":
            case "Ghost":
            case "Putrid Ghost":
            case "Spiker":
              animationLength = 1;
              break;
            case "Cat":
            case "Crow":
            case "Frog":
            case "Frost Bat":
            case "Frost Jelly":
            case "Iridium Bat":
            case "Iridium Slime":
            case "Lava Bat":
            case "Royal Serpent":
            case "Shadow Brute":
            case "Shadow Shaman":
            case "Shadow Sniper":
            case "Sludge":
            case "Tiger Slime":
              continue;
            case "Duggy":
            case "Fly":
            case "Grub":
            case "Iridium Crab":
            case "Lava Crab":
            case "Magma Duggy":
            case "Rock Crab":
            case "Stone Golem":
            case "Wilderness Golem":
              height1 = 24;
              num13 = 4;
              break;
            case "Dust Spirit":
            case "False Magma Cap":
              height1 = 24;
              num13 = 0;
              break;
            case "Green Slime":
              Texture2D texture2D4 = (Texture2D) null;
              if (character1 == null)
                texture2D4 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Green Slime");
              int height3 = 32;
              int num15 = 4;
              Character character2 = (Character) new GreenSlime(Vector2.Zero, 0);
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D4.Width, width1 * num15 / texture2D4.Width * height3, width1, height3), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 - 64), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height3 * 4) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character2,
                texture = (Texture2D) null
              });
              Character character3 = (Character) new GreenSlime(Vector2.Zero, 41);
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D4.Width, width1 * num15 / texture2D4.Width * height3, width1, height3), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 96.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height3 * 4) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character3,
                texture = (Texture2D) null
              });
              Character character4 = (Character) new GreenSlime(Vector2.Zero, 81);
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D4.Width, width1 * num15 / texture2D4.Width * height3, width1, height3), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 288.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height3 * 4) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character4,
                texture = (Texture2D) null
              });
              Character character5 = (Character) new GreenSlime(Vector2.Zero, 121);
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D4.Width, width1 * num15 / texture2D4.Width * height3, width1, height3), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 240.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height3 * 4 * 2) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character5,
                texture = (Texture2D) null
              });
              Character character6 = (Character) new GreenSlime(Vector2.Zero, 0);
              (character6 as GreenSlime).makeTigerSlime();
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D4.Width, width1 * num15 / texture2D4.Width * height3, width1, height3), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 144.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height3 * 4 * 2) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character6,
                texture = (Texture2D) null
              });
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D4.Width, width1 * num15 / texture2D4.Width * height3, width1, height3), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 + width1 * 4 / 2) - Game1.dialogueFont.MeasureString(keyValuePair.Value.Split('/')[14]).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                text = Utility.loadStringShort("UI", "EndCredit_Slimes")
              });
              num12 += 1500;
              continue;
            case "Lava Lurk":
              num13 = 4;
              flag = true;
              break;
            case "Magma Sparker":
            case "Magma Sprite":
              animationLength = 7;
              num13 = 7;
              break;
            case "Mummy":
            case "Skeleton":
            case "Skeleton Mage":
              height1 = 32;
              num13 = 4;
              break;
            case "Pepper Rex":
              width1 = 32;
              height1 = 32;
              break;
            case "Serpent":
              width1 = 32;
              height1 = 32;
              animationLength = 5;
              break;
            case "Shadow Guy":
              int height4 = 32;
              int num16 = 4;
              Texture2D texture2D5 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Shadow Brute");
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D5.Width, width1 * num16 / texture2D5.Width * height4, width1, height4), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height4 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character1,
                texture = texture2D5
              });
              Texture2D texture2D6 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Shadow Shaman");
              int height5 = 24;
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D6.Width, width1 * num16 / texture2D6.Width * height5, width1, height5), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 96f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height5 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character1,
                texture = texture2D6
              });
              Texture2D texture2D7 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Shadow Sniper");
              int height6 = 32;
              int width2 = 32;
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width2 * num16 % texture2D7.Width, width2 * num16 / texture2D7.Width * height6, width2, height6), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 288f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height6 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                yPeriodic = animationLength == 1,
                yPeriodicRange = 16f,
                yPeriodicLoopTime = 3000f,
                attachedCharacter = character1,
                texture = texture2D7
              });
              this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width2 * num16 % texture2D7.Width, width2 * num16 / texture2D7.Width * height6, width2, height6), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 128 + width2 * 4 / 2) - Game1.dialogueFont.MeasureString(keyValuePair.Value.Split('/')[14]).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                delayBeforeAnimationStart = num12,
                text = Utility.loadStringShort("UI", "EndCredit_ShadowPeople")
              });
              num12 += 1500;
              continue;
            case "Spider":
              width1 = 32;
              height1 = 32;
              animationLength = 2;
              break;
          }
          try
          {
            Texture2D texture2D8 = character1 != null ? character1.Sprite.Texture : Game1.content.Load<Texture2D>("Characters\\Monsters\\" + keyValuePair.Key);
            this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D8.Width, width1 * num13 / texture2D8.Width * height1 + 1, width1, height1 - 1), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height1 * 4) - 16.0) + (float) num14), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = num12,
              yPeriodic = animationLength == 1,
              yPeriodicRange = 16f,
              yPeriodicLoopTime = 3000f,
              attachedCharacter = character1,
              texture = character1 == null ? texture2D8 : (Texture2D) null,
              pingPong = flag
            });
            this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D8.Width, width1 * num13 / texture2D8.Width * height1, width1, height1), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 + width1 * 4 / 2) - Game1.dialogueFont.MeasureString(Game1.parseText(keyValuePair.Value.Split('/')[14], Game1.dialogueFont, 256)).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = num12,
              text = Game1.parseText(keyValuePair.Value.Split('/')[14], Game1.dialogueFont, 256)
            });
            num12 += 1500;
          }
          catch (Exception ex)
          {
          }
        }
      }
      return stringBuilder.ToString();
    }

    private bool sayGrufferSummitIntro(NPC spouse)
    {
      string name = (string) (NetFieldBase<string, NetString>) spouse.name;
      if (name == "Harvey" || name == "Elliott")
        return false;
      return name == "Abigail" || name == "Maru" || spouse.Gender == 0;
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this.critters != null && Game1.farmEvent == null)
      {
        for (int index = 0; index < this.critters.Count; ++index)
          this.critters[index].drawAboveFrontLayer(b);
      }
      foreach (Character character in this.characters)
        character.drawAboveAlwaysFrontLayer(b);
      foreach (Projectile projectile in this.projectiles)
        projectile.draw(b);
      if (!Game1.eventUp)
        return;
      if (this.isShowingEndSlideshow)
      {
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 400.0), Game1.viewport.Width, 8), Utility.GetPrismaticColor());
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 412.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.8f);
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 432.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.6f);
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 468.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.4f);
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 536.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.2f);
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 240.0), Game1.viewport.Width, 8), Utility.GetPrismaticColor());
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 256.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.8f);
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 276.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.6f);
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 312.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.4f);
        b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 380.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.2f);
      }
      foreach (TemporaryAnimatedSprite temporarySprite in this.TemporarySprites)
      {
        if (temporarySprite.drawAboveAlwaysFront)
          temporarySprite.draw(b);
      }
    }

    public override void draw(SpriteBatch b) => base.draw(b);
  }
}
