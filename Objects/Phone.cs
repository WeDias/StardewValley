// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Phone
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.Objects
{
  [InstanceStatics]
  public class Phone : StardewValley.Object
  {
    public const int RING_DURATION = 600;
    public const int RING_CYCLE_TIME = 1800;
    public static Random r;
    protected static bool _phoneSoundPlayed = false;
    public static int ringingTimer;
    public static int whichPhoneCall = -1;
    public static long lastRunTick = -1;
    public static long lastMinutesElapsedTick = -1;
    public static int intervalsToRing = 0;

    public Phone()
    {
    }

    public Phone(Vector2 position)
      : base(position, 214)
    {
      this.Name = "Telephone";
      this.type.Value = "Crafting";
      this.bigCraftable.Value = true;
      this.canBeSetDown.Value = true;
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return true;
      if (Phone.CanHearCall(Phone.whichPhoneCall) && Phone.whichPhoneCall >= 0)
      {
        int which_call = Phone.whichPhoneCall;
        if (Phone._phoneSoundPlayed)
        {
          Game1.soundBank.GetCue("phone").Stop(AudioStopOptions.Immediate);
          Phone._phoneSoundPlayed = false;
        }
        Game1.playSound("openBox");
        Game1.player.freezePause = 500;
        DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
        {
          switch (which_call)
          {
            case 0:
              Game1.drawDialogue(Game1.getCharacterFromName("Vincent"), Game1.content.LoadString("Strings\\Characters:Phone_Ring_Vincent"));
              break;
            case 1:
              Game1.drawDialogue(Game1.getCharacterFromName("Lewis"), Game1.content.LoadString("Strings\\Characters:Phone_Ring_Lewis"));
              break;
            case 2:
              Game1.drawDialogue(Game1.getCharacterFromName("Pierre"), Game1.content.LoadString("Strings\\Characters:Phone_Ring_Pierre"));
              break;
            case 3:
              Game1.multipleDialogues(Game1.content.LoadString("Strings\\Characters:Phone_Ring_Foreign").Split('#'));
              break;
            case 4:
              Game1.drawDialogue(new NPC(new AnimatedSprite("Characters\\Bear", 0, 32, 32), Vector2.Zero, "", 0, "Bear", (Dictionary<int, int[]>) null, Game1.temporaryContent.Load<Texture2D>("Portraits\\Bear"), false), Game1.content.LoadString("Strings\\Characters:Phone_Ring_Bear"));
              break;
            case 5:
              Game1.multipleDialogues(Game1.content.LoadString("Strings\\Characters:Phone_Ring_HatMouse").Split('#'));
              break;
            case 6:
              Game1.multipleDialogues(Game1.content.LoadString("Strings\\Characters:Phone_Ring_Cursed").Split('#'));
              break;
            case 7:
              Game1.multipleDialogues(Game1.content.LoadString("Strings\\Characters:Phone_Ring_RoboCaller").Split('#'));
              break;
          }
          Game1.player.callsReceived[which_call] = 1;
        }), 500);
      }
      else
        Game1.game1.ShowTelephoneMenu();
      Phone.ringingTimer = 0;
      Phone.whichPhoneCall = -1;
      return true;
    }

    public static bool CanHearCall(int which_phone_call) => (which_phone_call != 4 || Game1.player.eventsSeen.Contains(2120303)) && (which_phone_call != 6 || Game1.player.mailReceived.Contains("cursed_doll"));

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment) => base.performRemoveAction(tileLocation, environment);

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (environment != Game1.currentLocation)
        return;
      if ((long) Game1.ticks != Phone.lastRunTick)
      {
        if (Game1.eventUp)
          return;
        Phone.lastRunTick = (long) Game1.ticks;
        if (Phone.whichPhoneCall >= 0 && Phone.CanHearCall(Phone.whichPhoneCall) && Game1.shouldTimePass())
        {
          if (Phone.ringingTimer == 0)
          {
            Game1.playSound("phone");
            Phone._phoneSoundPlayed = true;
          }
          Phone.ringingTimer += (int) time.ElapsedGameTime.TotalMilliseconds;
          if (Phone.ringingTimer >= 1800)
          {
            Phone.ringingTimer = 0;
            Phone._phoneSoundPlayed = false;
          }
        }
      }
      base.updateWhenCurrentLocation(time, environment);
    }

    public override void DayUpdate(GameLocation location)
    {
      base.DayUpdate(location);
      Phone.r = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      Phone._phoneSoundPlayed = false;
      Phone.ringingTimer = 0;
      Phone.whichPhoneCall = -1;
      Phone.intervalsToRing = 0;
    }

    public override bool minutesElapsed(int minutes, GameLocation environment)
    {
      if (!Game1.IsMasterGame)
        return false;
      if (Phone.lastMinutesElapsedTick != (long) Game1.ticks)
      {
        Phone.lastMinutesElapsedTick = (long) Game1.ticks;
        if (Phone.intervalsToRing == 0)
        {
          if (Phone.r == null)
            Phone.r = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
          if (Phone.r.NextDouble() < 0.01)
          {
            int num = Phone.r.Next(8);
            if (Game1.timeOfDay < 1800 || num == 5)
            {
              Phone.intervalsToRing = 3;
              Game1.player.team.ringPhoneEvent.Fire(num);
            }
          }
        }
        else
        {
          --Phone.intervalsToRing;
          if (Phone.intervalsToRing <= 0)
            Game1.player.team.ringPhoneEvent.Fire(-1);
        }
      }
      return base.minutesElapsed(minutes, environment);
    }

    public static void Ring(int which_call)
    {
      if (which_call < 0)
      {
        Phone.whichPhoneCall = -1;
        Phone.ringingTimer = 0;
        if (!Phone._phoneSoundPlayed)
          return;
        Game1.soundBank.GetCue("phone").Stop(AudioStopOptions.Immediate);
        Phone._phoneSoundPlayed = false;
      }
      else
      {
        if (Game1.player.callsReceived.ContainsKey(which_call))
          return;
        Phone.whichPhoneCall = which_call;
        Phone.ringingTimer = 0;
        Phone._phoneSoundPlayed = false;
      }
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      base.draw(spriteBatch, x, y, alpha);
      bool flag = Phone.ringingTimer > 0 && Phone.ringingTimer < 600;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64)));
      Rectangle destinationRectangle = new Rectangle((int) local.X + (flag || this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) local.Y + (flag || this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), 64, 128);
      float layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 - 20) / 10000f) + (float) x * 1E-05f;
      spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(this.ParentSheetIndex + 1)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
    }

    public enum PhoneCalls
    {
      NONE = -1, // 0xFFFFFFFF
      Vincent = 0,
      Lewis = 1,
      Pierre = 2,
      Foreign = 3,
      Bear = 4,
      Hat = 5,
      Curse = 6,
      Robo = 7,
      MAX = 8,
    }
  }
}
