// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SaveGameMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StardewValley.Menus
{
  public class SaveGameMenu : IClickableMenu, IDisposable
  {
    private Stopwatch stopwatch;
    private IEnumerator<int> loader;
    private int completePause = -1;
    public bool quit;
    public bool hasDrawn;
    private SparklingText saveText;
    private int margin = 500;
    private StringBuilder _stringBuilder = new StringBuilder();
    private float _ellipsisDelay = 0.5f;
    private int _ellipsisCount;
    protected bool _hasSentFarmhandData;

    public SaveGameMenu()
    {
      this.saveText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGameMenu.cs.11378"), Color.LimeGreen, Color.Black * (1f / 1000f), millisecondsDuration: 1500, amplitude: 32);
      this._hasSentFarmhandData = false;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void complete()
    {
      Game1.playSound("money");
      this.completePause = 1500;
      this.loader = (IEnumerator<int>) null;
      Game1.game1.IsSaving = false;
      if (!Game1.IsMasterGame || Game1.newDaySync == null || Game1.newDaySync.hasSaved())
        return;
      Game1.newDaySync.flagSaved();
    }

    public override bool readyToClose() => false;

    public override void update(GameTime time)
    {
      if (this.quit)
      {
        if (!Game1.activeClickableMenu.Equals((object) this) || !Game1.PollForEndOfNewDaySync())
          return;
        Game1.exitActiveMenu();
      }
      else
      {
        base.update(time);
        if (Game1.client != null && Game1.client.timedOut)
        {
          this.quit = true;
          if (!Game1.activeClickableMenu.Equals((object) this))
            return;
          Game1.exitActiveMenu();
        }
        else
        {
          double ellipsisDelay = (double) this._ellipsisDelay;
          TimeSpan elapsedGameTime = time.ElapsedGameTime;
          double totalSeconds = elapsedGameTime.TotalSeconds;
          this._ellipsisDelay = (float) (ellipsisDelay - totalSeconds);
          if ((double) this._ellipsisDelay <= 0.0)
          {
            this._ellipsisDelay += 0.75f;
            ++this._ellipsisCount;
            if (this._ellipsisCount > 3)
              this._ellipsisCount = 1;
          }
          if (this.loader != null)
          {
            this.loader.MoveNext();
            if (this.loader.Current >= 100)
            {
              int margin = this.margin;
              elapsedGameTime = time.ElapsedGameTime;
              int milliseconds = elapsedGameTime.Milliseconds;
              this.margin = margin - milliseconds;
              if (this.margin <= 0)
                this.complete();
            }
          }
          else if (this.hasDrawn && this.completePause == -1)
          {
            if (Game1.IsMasterGame)
            {
              if (Game1.saveOnNewDay)
              {
                Game1.player.team.endOfNightStatus.UpdateState("ready");
                if (Game1.newDaySync.readyForSave())
                {
                  Game1.multiplayer.saveFarmhands();
                  Game1.game1.IsSaving = true;
                  this.loader = SaveGame.Save();
                }
              }
              else
              {
                this.margin = -1;
                if (Game1.newDaySync.readyForSave())
                {
                  Game1.game1.IsSaving = true;
                  this.complete();
                }
              }
            }
            else
            {
              if (LocalMultiplayer.IsLocalMultiplayer())
                LocalMultiplayer.SaveOptions();
              if (!this._hasSentFarmhandData)
              {
                this._hasSentFarmhandData = true;
                Game1.multiplayer.sendFarmhand();
              }
              Game1.multiplayer.UpdateLate();
              Program.sdk.Update();
              Game1.multiplayer.UpdateEarly();
              Game1.newDaySync.readyForSave();
              Game1.player.team.endOfNightStatus.UpdateState("ready");
              if (Game1.newDaySync.hasSaved())
              {
                SaveGameMenu.saveClientOptions();
                this.complete();
              }
            }
          }
          if (this.completePause < 0)
            return;
          int completePause = this.completePause;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds1 = elapsedGameTime.Milliseconds;
          this.completePause = completePause - milliseconds1;
          this.saveText.update(time);
          if (this.completePause >= 0)
            return;
          this.quit = true;
          this.completePause = -9999;
        }
      }
    }

    private static void saveClientOptions()
    {
      StartupPreferences startupPreferences = new StartupPreferences();
      startupPreferences.loadPreferences(false, false);
      startupPreferences.clientOptions = Game1.options;
      startupPreferences.savePreferences(false);
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      Vector2 vector2 = Utility.makeSafe(new Vector2(64f, (float) (Game1.uiViewport.Height - 64)), new Vector2(64f, 64f));
      bool flag = false;
      if (this.completePause >= 0)
      {
        if (Game1.saveOnNewDay)
          this.saveText.draw(b, vector2);
      }
      else if (this.margin < 0 || Game1.IsClient)
      {
        if (Game1.IsMultiplayer)
        {
          this._stringBuilder.Clear();
          this._stringBuilder.Append(Game1.content.LoadString("Strings\\UI:ReadyCheck", (object) Game1.newDaySync.numReadyForSave(), (object) Game1.getOnlineFarmers().Count));
          for (int index = 0; index < this._ellipsisCount; ++index)
            this._stringBuilder.Append(".");
          b.DrawString(Game1.dialogueFont, this._stringBuilder, vector2, Color.White);
          flag = true;
        }
      }
      else if (!Game1.IsMultiplayer)
      {
        this._stringBuilder.Clear();
        this._stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGameMenu.cs.11381"));
        for (int index = 0; index < this._ellipsisCount; ++index)
          this._stringBuilder.Append(".");
        b.DrawString(Game1.dialogueFont, this._stringBuilder, vector2, Color.White);
      }
      else
      {
        this._stringBuilder.Clear();
        this._stringBuilder.Append(Game1.content.LoadString("Strings\\UI:ReadyCheck", (object) Game1.newDaySync.numReadyForSave(), (object) Game1.getOnlineFarmers().Count));
        for (int index = 0; index < this._ellipsisCount; ++index)
          this._stringBuilder.Append(".");
        b.DrawString(Game1.dialogueFont, this._stringBuilder, vector2, Color.White);
        flag = true;
      }
      if (this.completePause > 0)
        flag = false;
      if (Game1.newDaySync != null && Game1.newDaySync.hasSaved())
        flag = false;
      if (Game1.IsMultiplayer & flag && Game1.options.showMPEndOfNightReadyStatus)
        Game1.player.team.endOfNightStatus.Draw(b, vector2 + new Vector2(0.0f, -32f), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
      this.hasDrawn = true;
    }

    public void Dispose() => Game1.game1.IsSaving = false;
  }
}
