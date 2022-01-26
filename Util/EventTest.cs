// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.EventTest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Util
{
  public class EventTest
  {
    private int currentEventIndex;
    private int currentLocationIndex;
    private int aButtonTimer;
    private List<string> specificEventsToDo = new List<string>();
    private bool doingSpecifics;

    public EventTest(string startingLocationName = "", int startingEventIndex = 0)
    {
      this.currentLocationIndex = 0;
      if (startingLocationName.Length > 0)
      {
        for (int index = 0; index < Game1.locations.Count<GameLocation>(); ++index)
        {
          if (Game1.locations[index].Name.Equals(startingLocationName))
          {
            this.currentLocationIndex = index;
            break;
          }
        }
      }
      this.currentEventIndex = startingEventIndex;
    }

    public EventTest(string[] whichEvents)
    {
      for (int index = 1; index < ((IEnumerable<string>) whichEvents).Count<string>(); index += 2)
        this.specificEventsToDo.Add(whichEvents[index] + " " + whichEvents[index + 1]);
      this.doingSpecifics = true;
      this.currentLocationIndex = -1;
    }

    public void update()
    {
      if (!Game1.eventUp && !Game1.fadeToBlack)
      {
        if (this.currentLocationIndex >= Game1.locations.Count)
          return;
        if (this.doingSpecifics && this.currentLocationIndex == -1)
        {
          if (this.specificEventsToDo.Count == 0)
            return;
          for (int index1 = 0; index1 < Game1.locations.Count<GameLocation>(); ++index1)
          {
            if (Game1.locations[index1].Name.Equals(this.specificEventsToDo.Last<string>().Split(' ')[0]))
            {
              this.currentLocationIndex = index1;
              Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + Game1.locations[index1].Name);
              for (int index2 = 0; index2 < source.Count; ++index2)
              {
                int result = -1;
                if (int.TryParse(source.ElementAt<KeyValuePair<string, string>>(index2).Key.Split('/')[0], out result) && result == Convert.ToInt32(this.specificEventsToDo.Last<string>().Split(' ')[1]))
                {
                  this.currentEventIndex = index2;
                  break;
                }
              }
              this.specificEventsToDo.Remove(this.specificEventsToDo.Last<string>());
              break;
            }
          }
        }
        GameLocation location = Game1.locations[this.currentLocationIndex];
        if (location.currentEvent != null)
          return;
        string locationName = (string) (NetFieldBase<string, NetString>) location.name;
        if (locationName == "Pool")
          locationName = "BathHouse_Pool";
        bool flag = true;
        try
        {
          Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName);
        }
        catch (Exception ex)
        {
          flag = false;
        }
        if (flag && this.currentEventIndex < Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).Count && Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).ElementAt<KeyValuePair<string, string>>(this.currentEventIndex).Key.Contains('/') && !Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).ElementAt<KeyValuePair<string, string>>(this.currentEventIndex).Value.Equals("null"))
        {
          if (Game1.currentLocation.Name.Equals(locationName))
          {
            Game1.eventUp = true;
            Game1.currentLocation.currentEvent = new StardewValley.Event(Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).ElementAt<KeyValuePair<string, string>>(this.currentEventIndex).Value);
          }
          else
          {
            LocationRequest locationRequest = Game1.getLocationRequest(locationName);
            int i = this.currentEventIndex;
            locationRequest.OnLoad += (LocationRequest.Callback) (() => Game1.currentLocation.currentEvent = new StardewValley.Event(Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).ElementAt<KeyValuePair<string, string>>(i).Value));
            Game1.warpFarmer(locationRequest, 8, 8, Game1.player.FacingDirection);
          }
        }
        ++this.currentEventIndex;
        if (!flag || this.currentEventIndex >= Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).Count)
        {
          this.currentEventIndex = 0;
          ++this.currentLocationIndex;
        }
        if (!this.doingSpecifics)
          return;
        this.currentLocationIndex = -1;
      }
      else
      {
        this.aButtonTimer -= (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
        if (this.aButtonTimer >= 0)
          return;
        this.aButtonTimer = 100;
        if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox))
          return;
        (Game1.activeClickableMenu as DialogueBox).performHoverAction(Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height - 64 - Game1.random.Next(300));
        DialogueBox activeClickableMenu = Game1.activeClickableMenu as DialogueBox;
        Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
        int x = viewport.Width / 2;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int y = viewport.Height - 64 - Game1.random.Next(300);
        activeClickableMenu.receiveLeftClick(x, y, true);
      }
    }
  }
}
