// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Bus
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class Bus : GameLocation
  {
    private bool talkedToKid;
    private bool talkedToWoman;
    private bool talkedToMan;
    private bool foundChange;
    private bool talkedToOldLady;
    private bool talkedToHaley;
    private int timesBagAttempt;

    public Bus()
    {
    }

    public Bus(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
      if (tile == null || Game1.year != 1 || Game1.dayOfMonth != 0 || !who.IsMainPlayer)
        return base.checkAction(tileLocation, viewport, who);
      switch (tile.TileIndex)
      {
        case 221:
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7130"));
          break;
        case 225:
          if (!this.talkedToHaley)
            Game1.drawDialogue(Game1.getCharacterFromName("Haley"), Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7152")));
          else
            Game1.drawDialogue(Game1.getCharacterFromName("Haley"), Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7154")));
          this.talkedToHaley = true;
          break;
        case 229:
          if (!this.talkedToMan)
          {
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7134")));
            break;
          }
          break;
        case 232:
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7135")));
          break;
        case 233:
          if (!this.talkedToOldLady)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7136")));
          else
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7137")));
          this.talkedToOldLady = true;
          break;
        case 236:
          switch (this.timesBagAttempt)
          {
            case 0:
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7140")));
              break;
            case 1:
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7141")));
              break;
            case 2:
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7142")));
              break;
            case 3:
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7143")));
              break;
            case 4:
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7144")));
              break;
            case 5:
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7145")));
              break;
            case 10:
              Game1.multipleDialogues(new string[2]
              {
                Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7146")),
                Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7147"))
              });
              ++Game1.player.Money;
              break;
            default:
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7148")));
              break;
          }
          ++this.timesBagAttempt;
          break;
        case 238:
          if (this.talkedToHaley)
          {
            this.busEvent();
            break;
          }
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7150")));
          break;
        case 265:
          Game1.drawObjectDialogue(Game1.player.IsMale || this.talkedToKid ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7127") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7128"));
          this.talkedToKid = true;
          break;
        case 266:
          if (Game1.player.IsMale)
          {
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7129")));
            break;
          }
          break;
        case 270:
          if (!this.talkedToWoman)
            Game1.multipleDialogues(new string[2]
            {
              Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7131")),
              Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7132"))
            });
          else
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7133")));
          this.talkedToWoman = true;
          break;
        case 274:
          if (!this.foundChange)
          {
            Game1.player.Money += 20;
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7138")));
            break;
          }
          this.foundChange = true;
          break;
        case 278:
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7139"));
          break;
        case 459:
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7149")));
          break;
      }
      return true;
    }

    public void busEvent()
    {
      this.characters.Add(new NPC(new AnimatedSprite("Characters\\Dobson", 0, 64, 128), new Vector2(-64000f, 128f), nameof (Bus), 0, "Dobson", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Dobson")));
      this.currentEvent = new Event("none/10 4/farmer 18 5 0 Dobson -100 -100 2/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7156") + "\"/pause 500/faceDirection farmer 3/pause 1000/playSound doorClose/warp Dobson 1 4/pause 500/speak Dobson \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7157") + "\"/move Dobson 3 0 1/speak Dobson \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7158") + "\"/move Dobson 5 0 1/pause 500/faceDirection Dobson 0/speak Dobson \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7159") + "\"/pause 500/faceDirection Dobson 3/speak Dobson \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7160") + "\"/pause 800/move Dobson 2 0 0/pause 800/showFrame Dobson 16/pause 800/speak Dobson \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7161") + "\"/showFrame Dobson 8/pause 400/move Dobson 5 0 1/pause 1000/showFrame Dobson 17/pause 1000/showFrame Dobson 4/pause 1000/faceDirection Dobson 3/speak Dobson \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7162") + "\"/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7163") + "\"/pause 800/changeMapTile Buildings 9 1 119/changeMapTile Buildings 9 2 141/pause 400/changeMapTile Buildings 9 1 185/changeMapTile Buildings 9 2 207/pause 400/changeMapTile Buildings 9 1 119/changeMapTile Buildings 9 2 141/pause 300/changeMapTile Buildings 9 1 119/changeMapTile Buildings 9 2 141/pause 400/changeMapTile Buildings 9 1 185/changeMapTile Buildings 9 2 207/pause 400/changeMapTile Buildings 9 1 119/changeMapTile Buildings 9 2 141/pause 300/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Bus.cs.7164") + "\"/pause 400/faceDirection farmer 0/pause 500/faceDirection Dobson 0/pause 1000/end busIntro");
      Game1.eventUp = true;
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.map.Update((long) time.ElapsedGameTime.Milliseconds);
      if (this.currentEvent == null)
        return;
      this.currentEvent.checkForNextCommand((GameLocation) this, time);
    }
  }
}
