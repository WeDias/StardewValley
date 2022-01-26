// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.MiniJukebox
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Objects
{
  public class MiniJukebox : Object
  {
    private bool showNote;

    protected override void initNetFields() => base.initNetFields();

    public MiniJukebox()
    {
    }

    public MiniJukebox(Vector2 position)
      : base(position, 209)
    {
      this.Name = "Mini-Jukebox";
      this.type.Value = "Crafting";
      this.bigCraftable.Value = true;
      this.canBeSetDown.Value = true;
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return true;
      if (!who.currentLocation.IsFarm && !who.currentLocation.IsGreenhouse && !(who.currentLocation is Cellar))
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Mini_JukeBox_NotFarmPlay"));
      else if (who.currentLocation.IsOutdoors && Game1.isRaining)
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Mini_JukeBox_OutdoorRainy"));
      }
      else
      {
        List<string> list = Game1.player.songsHeard.Distinct<string>().ToList<string>();
        list.Insert(0, "turn_off");
        list.Add("random");
        Game1.activeClickableMenu = (IClickableMenu) new ChooseFromListMenu(list, new ChooseFromListMenu.actionOnChoosingListOption(this.OnSongChosen), true, who.currentLocation.miniJukeboxTrack.Value);
      }
      return true;
    }

    public void RegisterToLocation(GameLocation location) => location?.OnMiniJukeboxAdded();

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
    {
      environment?.OnMiniJukeboxRemoved();
      base.performRemoveAction(tileLocation, environment);
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (environment != null && environment.IsMiniJukeboxPlaying())
      {
        this.showNextIndex.Value = true;
        if (this.showNote)
        {
          this.showNote = false;
          for (int index = 0; index < 4; ++index)
            environment.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(516, 1916, 7, 10), 9999f, 1, 1, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(48), -80f), false, false, (float) (((double) this.tileLocation.Value.Y + 1.0) * 64.0 / 10000.0), 0.01f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              xPeriodic = true,
              xPeriodicLoopTime = 1200f,
              xPeriodicRange = 8f,
              motion = new Vector2((float) Game1.random.Next(-10, 10) / 100f, -1f),
              delayBeforeAnimationStart = 1200 + 300 * index
            });
        }
      }
      else
        this.showNextIndex.Value = false;
      base.updateWhenCurrentLocation(time, environment);
    }

    public void OnSongChosen(string selection)
    {
      if (Game1.player.currentLocation == null)
        return;
      if (selection == "turn_off")
      {
        Game1.player.currentLocation.miniJukeboxTrack.Value = "";
      }
      else
      {
        if (selection != (string) (NetFieldBase<string, NetString>) Game1.player.currentLocation.miniJukeboxTrack)
        {
          this.showNote = true;
          this.shakeTimer = 1000;
        }
        if (selection == "random")
          Game1.player.currentLocation.SelectRandomMiniJukeboxTrack();
        Game1.player.currentLocation.miniJukeboxTrack.Value = selection;
      }
    }
  }
}
