// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MapPage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class MapPage : IClickableMenu
  {
    public const int region_desert = 1001;
    public const int region_farm = 1002;
    public const int region_backwoods = 1003;
    public const int region_busstop = 1004;
    public const int region_wizardtower = 1005;
    public const int region_marnieranch = 1006;
    public const int region_leahcottage = 1007;
    public const int region_samhouse = 1008;
    public const int region_haleyhouse = 1009;
    public const int region_townsquare = 1010;
    public const int region_harveyclinic = 1011;
    public const int region_generalstore = 1012;
    public const int region_blacksmith = 1013;
    public const int region_saloon = 1014;
    public const int region_manor = 1015;
    public const int region_museum = 1016;
    public const int region_elliottcabin = 1017;
    public const int region_sewer = 1018;
    public const int region_graveyard = 1019;
    public const int region_trailer = 1020;
    public const int region_alexhouse = 1021;
    public const int region_sciencehouse = 1022;
    public const int region_tent = 1023;
    public const int region_mines = 1024;
    public const int region_adventureguild = 1025;
    public const int region_quarry = 1026;
    public const int region_jojamart = 1027;
    public const int region_fishshop = 1028;
    public const int region_spa = 1029;
    public const int region_secretwoods = 1030;
    public const int region_ruinedhouse = 1031;
    public const int region_communitycenter = 1032;
    public const int region_sewerpipe = 1033;
    public const int region_railroad = 1034;
    public const int region_island = 1035;
    private string descriptionText = "";
    private string hoverText = "";
    private string playerLocationName;
    private Texture2D map;
    private int mapX;
    private int mapY;
    public List<ClickableComponent> points = new List<ClickableComponent>();
    private bool drawPamHouseUpgrade;
    private bool drawMovieTheaterJoja;
    private bool drawMovieTheater;
    private bool drawIsland;

    public MapPage(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
      this.map = Game1.content.Load<Texture2D>("LooseSprites\\map");
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.map.Bounds.Width * 4, 720);
      this.drawPamHouseUpgrade = Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade");
      this.drawMovieTheaterJoja = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja");
      this.drawMovieTheater = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater");
      this.mapX = (int) centeringOnScreen.X;
      this.mapY = (int) centeringOnScreen.Y;
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX, this.mapY, 292, 152), Game1.MasterPlayer.mailReceived.Contains("ccVault") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062") : "???")
      {
        myID = 1001,
        rightNeighborID = 1003,
        downNeighborID = 1030
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 324, this.mapY + 252, 188, 132), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11064", (object) Game1.MasterPlayer.farmName))
      {
        myID = 1002,
        leftNeighborID = 1005,
        upNeighborID = 1003,
        rightNeighborID = 1004,
        downNeighborID = 1006
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 360, this.mapY + 96, 188, 132), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11065"))
      {
        myID = 1003,
        downNeighborID = 1002,
        leftNeighborID = 1001,
        rightNeighborID = 1022,
        upNeighborID = 1029
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 516, this.mapY + 224, 76, 100), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11066"))
      {
        myID = 1004,
        leftNeighborID = 1002,
        upNeighborID = 1003,
        downNeighborID = 1006,
        rightNeighborID = 1011
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 196, this.mapY + 352, 36, 76), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11067"))
      {
        myID = 1005,
        upNeighborID = 1001,
        downNeighborID = 1031,
        rightNeighborID = 1006,
        leftNeighborID = 1030
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 420, this.mapY + 392, 76, 40), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11068") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11069"))
      {
        myID = 1006,
        leftNeighborID = 1005,
        downNeighborID = 1007,
        upNeighborID = 1002,
        rightNeighborID = 1008
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 452, this.mapY + 436, 32, 24), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11070"))
      {
        myID = 1007,
        upNeighborID = 1006,
        downNeighborID = 1033,
        leftNeighborID = 1005,
        rightNeighborID = 1008
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 612, this.mapY + 396, 36, 52), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11071") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11072"))
      {
        myID = 1008,
        leftNeighborID = 1006,
        upNeighborID = 1010,
        rightNeighborID = 1009
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 652, this.mapY + 408, 40, 36), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11073") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11074"))
      {
        myID = 1009,
        leftNeighborID = 1008,
        upNeighborID = 1010,
        rightNeighborID = 1018
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 672, this.mapY + 340, 44, 60), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11075"))
      {
        myID = 1010,
        leftNeighborID = 1008,
        downNeighborID = 1009,
        rightNeighborID = 1014,
        upNeighborID = 1011
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 680, this.mapY + 304, 16, 32), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11076") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11077"))
      {
        myID = 1011,
        leftNeighborID = 1004,
        rightNeighborID = 1012,
        downNeighborID = 1010,
        upNeighborID = 1032
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 696, this.mapY + 296, 28, 40), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11078") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11079") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11080"))
      {
        myID = 1012,
        leftNeighborID = 1011,
        downNeighborID = 1014,
        rightNeighborID = 1021,
        upNeighborID = 1032
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 852, this.mapY + 388, 80, 36), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11081") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11082"))
      {
        myID = 1013,
        upNeighborID = 1027,
        rightNeighborID = 1016,
        downNeighborID = 1017,
        leftNeighborID = 1015
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 716, this.mapY + 352, 28, 40), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11083") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11084"))
      {
        myID = 1014,
        leftNeighborID = 1010,
        rightNeighborID = 1020,
        downNeighborID = 1019,
        upNeighborID = 1012
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 768, this.mapY + 388, 44, 56), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11085"))
      {
        myID = 1015,
        leftNeighborID = 1019,
        upNeighborID = 1020,
        rightNeighborID = 1013,
        downNeighborID = 1017
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 892, this.mapY + 416, 32, 28), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11086") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11087"))
      {
        myID = 1016,
        downNeighborID = 1017,
        leftNeighborID = 1013,
        upNeighborID = 1027,
        rightNeighborID = -1
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 824, this.mapY + 564, 28, 20), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11088"))
      {
        myID = 1017,
        downNeighborID = 1028,
        upNeighborID = 1015,
        rightNeighborID = -1
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 696, this.mapY + 448, 24, 20), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11089"))
      {
        myID = 1018,
        downNeighborID = 1017,
        rightNeighborID = 1019,
        upNeighborID = 1014,
        leftNeighborID = 1009
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 724, this.mapY + 424, 40, 32), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11090"))
      {
        myID = 1019,
        leftNeighborID = 1018,
        upNeighborID = 1014,
        rightNeighborID = 1015,
        downNeighborID = 1017
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 780, this.mapY + 360, 24, 20), Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.PamHouse") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.PamHouseHomeOf") : Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11091"))
      {
        myID = 1020,
        upNeighborID = 1021,
        leftNeighborID = 1014,
        downNeighborID = 1015,
        rightNeighborID = 1027
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 748, this.mapY + 316, 36, 36), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11092") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11093"))
      {
        myID = 1021,
        rightNeighborID = 1027,
        downNeighborID = 1020,
        leftNeighborID = 1012,
        upNeighborID = 1032
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 732, this.mapY + 148, 48, 32), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11094") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11095") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11096"))
      {
        myID = 1022,
        downNeighborID = 1032,
        leftNeighborID = 1003,
        upNeighborID = 1034,
        rightNeighborID = 1023
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 784, this.mapY + 128, 12, 16), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11097"))
      {
        myID = 1023,
        leftNeighborID = 1034,
        downNeighborID = 1022,
        rightNeighborID = 1024
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 880, this.mapY + 96, 16, 24), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11098"))
      {
        myID = 1024,
        leftNeighborID = 1023,
        rightNeighborID = 1025,
        downNeighborID = 1027
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 900, this.mapY + 108, 32, 36), Game1.stats.DaysPlayed >= 5U ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11099") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11100") : "???")
      {
        myID = 1025,
        leftNeighborID = 1024,
        rightNeighborID = 1026,
        downNeighborID = 1027
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 968, this.mapY + 116, 88, 76), Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11103") : "???")
      {
        myID = 1026,
        leftNeighborID = 1025,
        downNeighborID = 1027
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 872, this.mapY + 280, 52, 52), !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater") || Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja") ? (!Utility.HasAnyPlayerSeenEvent(191393) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11105") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11106") : Game1.content.LoadString("Strings\\StringsFromCSFiles:AbandonedJojaMart")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:MovieTheater_Map") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MovieTheater_Hours"))
      {
        myID = 1027,
        upNeighborID = 1025,
        leftNeighborID = 1021,
        downNeighborID = 1013
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 844, this.mapY + 608, 36, 40), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11107") + Environment.NewLine + (Game1.player.mailReceived.Contains("willyHours") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11108_newHours") : Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11108")))
      {
        myID = 1028,
        upNeighborID = 1017,
        rightNeighborID = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("Visited_Island") ? 1035 : -1
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 576, this.mapY + 60, 48, 36), Game1.isLocationAccessible("Railroad") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11110") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11111") : "???")
      {
        myID = 1029,
        rightNeighborID = 1034,
        downNeighborID = 1003,
        leftNeighborID = 1001
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX, this.mapY + 272, 196, 176), Game1.player.mailReceived.Contains("beenToWoods") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11114") : "???")
      {
        myID = 1030,
        upNeighborID = 1001,
        rightNeighborID = 1005
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 260, this.mapY + 572, 20, 20), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11116"))
      {
        myID = 1031,
        rightNeighborID = 1033,
        upNeighborID = 1005
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 692, this.mapY + 204, 44, 36), !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater") || !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11117") : Game1.content.LoadString("Strings\\StringsFromCSFiles:MovieTheater_Map") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MovieTheater_Hours"))
      {
        myID = 1032,
        downNeighborID = 1012,
        upNeighborID = 1022,
        leftNeighborID = 1004
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 380, this.mapY + 596, 24, 32), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11118"))
      {
        myID = 1033,
        leftNeighborID = 1031,
        rightNeighborID = 1017,
        upNeighborID = 1007
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 644, this.mapY + 64, 16, 8), Game1.isLocationAccessible("Railroad") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11119") : "???")
      {
        myID = 1034,
        leftNeighborID = 1029,
        rightNeighborID = 1023,
        downNeighborID = 1022
      });
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 728, this.mapY + 652, 28, 28), Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11122")));
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("Visited_Island"))
        return;
      this.drawIsland = true;
      this.points.Add(new ClickableComponent(new Rectangle(this.mapX + 1040, this.mapY + 600, 160, 120), Game1.content.LoadString("Strings\\StringsFromCSFiles:IslandName"))
      {
        myID = 1035,
        downNeighborID = -1,
        upNeighborID = 1013,
        leftNeighborID = 1028
      });
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(1002);
      this.snapCursorToCurrentSnappedComponent();
    }

    public Vector2 getPlayerMapPosition(Farmer player)
    {
      Vector2 playerMapPosition = new Vector2(-999f, -999f);
      if (player.currentLocation == null)
        return playerMapPosition;
      string str1 = player.currentLocation.Name;
      if (str1.StartsWith("UndergroundMine") || str1 == "Mine")
      {
        str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11098");
        if (player.currentLocation is MineShaft && (player.currentLocation as MineShaft).mineLevel > 120 && (player.currentLocation as MineShaft).mineLevel != 77377)
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062");
      }
      if (player.currentLocation is IslandLocation)
        str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:IslandName");
      switch (player.currentLocation.Name)
      {
        case "AdventureGuild":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11099");
          break;
        case "AnimalShop":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11068");
          break;
        case "ArchaeologyHouse":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11086");
          break;
        case "BathHouse_Entry":
        case "BathHouse_MensLocker":
        case "BathHouse_Pool":
        case "BathHouse_WomensLocker":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11110") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11111");
          break;
        case "Club":
        case "Desert":
        case "SandyHouse":
        case "SandyShop":
        case "SkullCave":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062");
          break;
        case "CommunityCenter":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11117");
          break;
        case "ElliottHouse":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11088");
          break;
        case "FishShop":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11107");
          break;
        case "HarveyRoom":
        case "Hospital":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11076") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11077");
          break;
        case "JoshHouse":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11092") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11093");
          break;
        case "ManorHouse":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11085");
          break;
        case "Railroad":
        case "WitchWarpCave":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11119");
          break;
        case "ScienceHouse":
        case "SebastianRoom":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11094") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11095");
          break;
        case "SeedShop":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11078") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11079") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11080");
          break;
        case "Temp":
          if (player.currentLocation.Map.Id.Contains("Town"))
          {
            str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
            break;
          }
          break;
        case "Trailer_Big":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.PamHouse");
          break;
        case "WizardHouse":
        case "WizardHouseBasement":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11067");
          break;
        case "Woods":
          str1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11114");
          break;
      }
      foreach (ClickableComponent point in this.points)
      {
        string str2 = point.name.Replace(" ", "");
        int length1 = point.name.IndexOf(Environment.NewLine);
        int length2 = str2.IndexOf(Environment.NewLine);
        string str3 = str1.Substring(0, str1.Contains(Environment.NewLine) ? str1.IndexOf(Environment.NewLine) : str1.Length);
        if (point.name.Equals(str1) || str2.Equals(str1) || point.name.Contains(Environment.NewLine) && (point.name.Substring(0, length1).Equals(str3) || str2.Substring(0, length2).Equals(str3)))
        {
          playerMapPosition = new Vector2((float) point.bounds.Center.X, (float) point.bounds.Center.Y);
          if (player.IsLocalPlayer)
            this.playerLocationName = point.name.Contains(Environment.NewLine) ? point.name.Substring(0, point.name.IndexOf(Environment.NewLine)) : point.name;
          return playerMapPosition;
        }
      }
      int tileX = player.getTileX();
      int tileY = player.getTileY();
      switch ((string) (NetFieldBase<string, NetString>) player.currentLocation.name)
      {
        case "Backwoods":
        case "Tunnel":
          playerMapPosition = new Vector2((float) (this.mapX + 436), (float) (this.mapY + 188));
          if (player.IsLocalPlayer)
          {
            this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11180");
            break;
          }
          break;
        case "Barn":
        case "Big Barn":
        case "Big Coop":
        case "Big Shed":
        case "Cabin":
        case "Coop":
        case "Deluxe Barn":
        case "Deluxe Coop":
        case "Farm":
        case "FarmCave":
        case "FarmHouse":
        case "Greenhouse":
        case "Shed":
        case "Slime Hutch":
          playerMapPosition = new Vector2((float) (this.mapX + 384), (float) (this.mapY + 288));
          if (player.IsLocalPlayer)
          {
            this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11064", (object) player.farmName);
            break;
          }
          break;
        case "Beach":
          if (player.IsLocalPlayer)
            this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11174");
          playerMapPosition = new Vector2((float) (this.mapX + 808), (float) (this.mapY + 564));
          break;
        case "Forest":
          if (tileY > 51)
          {
            if (player.IsLocalPlayer)
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11186");
            playerMapPosition = new Vector2((float) (this.mapX + 280), (float) (this.mapY + 540));
            break;
          }
          if (tileX < 58)
          {
            if (player.IsLocalPlayer)
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11186");
            playerMapPosition = new Vector2((float) (this.mapX + 252), (float) (this.mapY + 416));
            break;
          }
          if (player.IsLocalPlayer)
            this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11188");
          playerMapPosition = new Vector2((float) (this.mapX + 436), (float) (this.mapY + 428));
          break;
        case "Mountain":
          if (tileX < 38)
          {
            if (player.IsLocalPlayer)
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11176");
            playerMapPosition = new Vector2((float) (this.mapX + 740), (float) (this.mapY + 144));
            break;
          }
          if (tileX < 96)
          {
            if (player.IsLocalPlayer)
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11177");
            playerMapPosition = new Vector2((float) (this.mapX + 880), (float) (this.mapY + 152));
            break;
          }
          if (player.IsLocalPlayer)
            this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11178");
          playerMapPosition = new Vector2((float) (this.mapX + 1012), (float) (this.mapY + 160));
          break;
        case "Saloon":
          if (player.IsLocalPlayer)
          {
            this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11172");
            break;
          }
          break;
        case "Temp":
          if (player.currentLocation.Map.Id.Contains("Town"))
          {
            if (tileX > 84 && tileY < 68)
            {
              playerMapPosition = new Vector2((float) (this.mapX + 900), (float) (this.mapY + 324));
              if (player.IsLocalPlayer)
              {
                this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
                break;
              }
              break;
            }
            if (tileX > 80 && tileY >= 68)
            {
              playerMapPosition = new Vector2((float) (this.mapX + 880), (float) (this.mapY + 432));
              if (player.IsLocalPlayer)
              {
                this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
                break;
              }
              break;
            }
            if (tileY <= 42)
            {
              playerMapPosition = new Vector2((float) (this.mapX + 712), (float) (this.mapY + 256));
              if (player.IsLocalPlayer)
              {
                this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
                break;
              }
              break;
            }
            if (tileY > 42 && tileY < 76)
            {
              playerMapPosition = new Vector2((float) (this.mapX + 700), (float) (this.mapY + 352));
              if (player.IsLocalPlayer)
              {
                this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
                break;
              }
              break;
            }
            playerMapPosition = new Vector2((float) (this.mapX + 728), (float) (this.mapY + 436));
            if (player.IsLocalPlayer)
            {
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
              break;
            }
            break;
          }
          break;
        case "Town":
          if (tileX > 84 && tileY < 68)
          {
            playerMapPosition = new Vector2((float) (this.mapX + 900), (float) (this.mapY + 324));
            if (player.IsLocalPlayer)
            {
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
              break;
            }
            break;
          }
          if (tileX > 80 && tileY >= 68)
          {
            playerMapPosition = new Vector2((float) (this.mapX + 880), (float) (this.mapY + 432));
            if (player.IsLocalPlayer)
            {
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
              break;
            }
            break;
          }
          if (tileY <= 42)
          {
            playerMapPosition = new Vector2((float) (this.mapX + 712), (float) (this.mapY + 256));
            if (player.IsLocalPlayer)
            {
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
              break;
            }
            break;
          }
          if (tileY > 42 && tileY < 76)
          {
            playerMapPosition = new Vector2((float) (this.mapX + 700), (float) (this.mapY + 352));
            if (player.IsLocalPlayer)
            {
              this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
              break;
            }
            break;
          }
          playerMapPosition = new Vector2((float) (this.mapX + 728), (float) (this.mapY + 436));
          if (player.IsLocalPlayer)
          {
            this.playerLocationName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190");
            break;
          }
          break;
      }
      return playerMapPosition;
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);
      if (!Game1.options.doesInputListContain(Game1.options.mapButton, key))
        return;
      this.exitThisMenu();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      foreach (ClickableComponent point in this.points)
      {
        if (point.containsPoint(x, y) && point.name == "Lonely Stone")
          Game1.playSound("stoneCrack");
      }
      if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is GameMenu))
        return;
      (Game1.activeClickableMenu as GameMenu).changeTab((Game1.activeClickableMenu as GameMenu).lastOpenedNonMapTab);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.descriptionText = "";
      this.hoverText = "";
      foreach (ClickableComponent point in this.points)
      {
        if (point.containsPoint(x, y))
        {
          this.hoverText = point.name;
          break;
        }
      }
    }

    protected virtual void drawMiniPortraits(SpriteBatch b)
    {
      Dictionary<Vector2, int> dictionary = new Dictionary<Vector2, int>();
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        Vector2 key = this.getPlayerMapPosition(onlineFarmer) - new Vector2(32f, 32f);
        int num = 0;
        dictionary.TryGetValue(key, out num);
        dictionary[key] = num + 1;
        Vector2 position = key + new Vector2((float) (48 * (num % 2)), (float) (48 * (num / 2)));
        onlineFarmer.FarmerRenderer.drawMiniPortrat(b, position, 0.00011f, 4f, 2, onlineFarmer);
      }
    }

    public override void draw(SpriteBatch b)
    {
      float y1 = (float) (this.yPositionOnScreen + this.height + 32 + 16);
      float num = y1 + 80f;
      if ((double) num > (double) Game1.uiViewport.Height)
        y1 -= num - (float) Game1.uiViewport.Height;
      int y2 = this.mapY - 96;
      int mapY = this.mapY;
      Game1.drawDialogueBox(this.mapX - 32, y2, (this.map.Bounds.Width + 16) * 4, 848, false, true);
      b.Draw(this.map, new Vector2((float) this.mapX, (float) mapY), new Rectangle?(new Rectangle(0, 0, 300, 180)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.86f);
      switch (Game1.whichFarm)
      {
        case 1:
          b.Draw(this.map, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(new Rectangle(0, 180, 131, 61)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
          break;
        case 2:
          b.Draw(this.map, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(new Rectangle(131, 180, 131, 61)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
          break;
        case 3:
          b.Draw(this.map, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(new Rectangle(0, 241, 131, 61)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
          break;
        case 4:
          b.Draw(this.map, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(new Rectangle(131, 241, 131, 61)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
          break;
        case 5:
          b.Draw(this.map, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(new Rectangle(0, 302, 131, 61)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
          break;
        case 6:
          b.Draw(this.map, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(new Rectangle(131, 302, 131, 61)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
          break;
        case 7:
          if (Game1.whichModFarm != null && Game1.whichModFarm.WorldMapTexture != null)
          {
            Texture2D texture = Game1.content.Load<Texture2D>(Game1.whichModFarm.WorldMapTexture);
            b.Draw(texture, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
            break;
          }
          b.Draw(this.map, new Vector2((float) this.mapX, (float) (mapY + 172)), new Rectangle?(new Rectangle(0, 180, 131, 61)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
          break;
      }
      if (this.drawPamHouseUpgrade)
        b.Draw(this.map, new Vector2((float) (this.mapX + 780), (float) (this.mapY + 348)), new Rectangle?(new Rectangle(263, 181, 8, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
      if (this.drawMovieTheater)
        b.Draw(this.map, new Vector2((float) (this.mapX + 852), (float) (this.mapY + 280)), new Rectangle?(new Rectangle(271, 181, 29, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
      if (this.drawMovieTheaterJoja)
        b.Draw(this.map, new Vector2((float) (this.mapX + 684), (float) (this.mapY + 192)), new Rectangle?(new Rectangle(276, 181, 13, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
      if (this.drawIsland)
        b.Draw(this.map, new Vector2((float) (this.mapX + 1040), (float) (this.mapY + 600)), new Rectangle?(new Rectangle(208, 363, 40, 30)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.861f);
      this.drawMiniPortraits(b);
      if (this.playerLocationName != null)
        SpriteText.drawStringWithScrollCenteredAt(b, this.playerLocationName, this.xPositionOnScreen + this.width / 2, (int) y1);
      if (this.hoverText.Equals(""))
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
  }
}
