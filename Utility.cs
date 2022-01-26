// Decompiled with JetBrains decompiler
// Type: StardewValley.Utility
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using xTile.Dimensions;

namespace StardewValley
{
  public class Utility
  {
    public static readonly char[] CharSpace = new char[1]
    {
      ' '
    };
    public static Microsoft.Xna.Framework.Color[] PRISMATIC_COLORS = new Microsoft.Xna.Framework.Color[6]
    {
      Microsoft.Xna.Framework.Color.Red,
      new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 120, 0),
      new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 217, 0),
      Microsoft.Xna.Framework.Color.Lime,
      Microsoft.Xna.Framework.Color.Cyan,
      Microsoft.Xna.Framework.Color.Violet
    };
    public static List<VertexPositionColor[]> straightLineVertex = new List<VertexPositionColor[]>()
    {
      new VertexPositionColor[2],
      new VertexPositionColor[2],
      new VertexPositionColor[2],
      new VertexPositionColor[2]
    };
    private static readonly ListPool<NPC> _pool = new ListPool<NPC>();
    public static readonly Vector2[] DirectionsTileVectors = new Vector2[4]
    {
      new Vector2(0.0f, -1f),
      new Vector2(1f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(-1f, 0.0f)
    };
    public static readonly RasterizerState ScissorEnabled = new RasterizerState()
    {
      ScissorTestEnable = true
    };

    public static Microsoft.Xna.Framework.Rectangle controllerMapSourceRect(
      Microsoft.Xna.Framework.Rectangle xboxSourceRect)
    {
      return xboxSourceRect;
    }

    public static char getRandomSlotCharacter() => Utility.getRandomSlotCharacter('o');

    public static List<Vector2> removeDuplicates(List<Vector2> list)
    {
      for (int index1 = 0; index1 < list.Count; ++index1)
      {
        for (int index2 = list.Count - 1; index2 >= 0; --index2)
        {
          if (index2 != index1 && list[index1].Equals(list[index2]))
            list.RemoveAt(index2);
        }
      }
      return list;
    }

    public static IEnumerable<int> GetHorseWarpRestrictionsForFarmer(Farmer who)
    {
      if (who.horseName.Value == null)
        yield return 1;
      GameLocation location = who.currentLocation;
      if (!location.IsOutdoors)
        yield return 2;
      if (location.isCollidingPosition(new Microsoft.Xna.Framework.Rectangle(who.getTileX() * 64, who.getTileY() * 64, 128, 64), Game1.viewport, true, 0, false, (Character) who))
        yield return 3;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.mount != null && onlineFarmer.mount.getOwner() == who)
          yield return 4;
      }
    }

    public static Microsoft.Xna.Framework.Rectangle ConstrainScissorRectToScreen(
      Microsoft.Xna.Framework.Rectangle scissor_rect)
    {
      if (scissor_rect.Top < 0)
      {
        int num = -scissor_rect.Top;
        scissor_rect.Height -= num;
        scissor_rect.Y += num;
      }
      if (scissor_rect.Bottom > Game1.viewport.Height)
      {
        int num = scissor_rect.Bottom - Game1.viewport.Height;
        scissor_rect.Height -= num;
      }
      if (scissor_rect.Left < 0)
      {
        int num = -scissor_rect.Left;
        scissor_rect.Width -= num;
        scissor_rect.X += num;
      }
      if (scissor_rect.Right > Game1.viewport.Width)
      {
        int num = scissor_rect.Right - Game1.viewport.Width;
        scissor_rect.Width -= num;
      }
      return scissor_rect;
    }

    public static void RecordAnimalProduce(FarmAnimal animal, int produce)
    {
      if (animal.type.Contains("Cow"))
        ++Game1.stats.CowMilkProduced;
      else if (animal.type.Contains("Sheep"))
      {
        ++Game1.stats.SheepWoolProduced;
      }
      else
      {
        if (!animal.type.Contains("Goat"))
          return;
        ++Game1.stats.GoatMilkProduced;
      }
    }

    public static Point Vector2ToPoint(Vector2 v) => new Point((int) v.X, (int) v.Y);

    public static Vector2 PointToVector2(Point p) => new Vector2((float) p.X, (float) p.Y);

    public static int getStartTimeOfFestival() => Game1.weatherIcon == 1 ? Convert.ToInt32(Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth.ToString())["conditions"].Split('/')[1].Split(' ')[0]) : -1;

    public static bool doesMasterPlayerHaveMailReceivedButNotMailForTomorrow(string mailID) => (Game1.MasterPlayer.mailReceived.Contains(mailID) || Game1.MasterPlayer.mailReceived.Contains(mailID + "%&NL&%")) && !Game1.MasterPlayer.mailForTomorrow.Contains(mailID) && !Game1.MasterPlayer.mailForTomorrow.Contains(mailID + "%&NL&%");

    public static bool isFestivalDay(int day, string season)
    {
      string key = season + day.ToString();
      return Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\FestivalDates").ContainsKey(key);
    }

    public static void ForAllLocations(Action<GameLocation> action)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        action(location);
        if (location is BuildableGameLocation)
        {
          foreach (Building building in (location as BuildableGameLocation).buildings)
          {
            if (building.indoors.Value != null)
              action(building.indoors.Value);
          }
        }
      }
    }

    public static int getNumObjectsOfIndexWithinRectangle(
      Microsoft.Xna.Framework.Rectangle r,
      int[] indexes,
      GameLocation location)
    {
      int indexWithinRectangle = 0;
      Vector2 zero = Vector2.Zero;
      for (int y = r.Y; y < r.Bottom + 1; ++y)
      {
        zero.Y = (float) y;
        for (int x = r.X; x < r.Right + 1; ++x)
        {
          zero.X = (float) x;
          if (location.objects.ContainsKey(zero))
          {
            for (int index = 0; index < indexes.Length; ++index)
            {
              if (indexes[index] == (int) (NetFieldBase<int, NetInt>) location.objects[zero].parentSheetIndex || indexes[index] == -1)
              {
                ++indexWithinRectangle;
                break;
              }
            }
          }
        }
      }
      return indexWithinRectangle;
    }

    public static string fuzzySearch(string query, List<string> word_bank)
    {
      foreach (string str in word_bank)
      {
        if (query.Trim() == str.Trim())
          return str;
      }
      foreach (string term in word_bank)
      {
        if (Utility._formatForFuzzySearch(query) == Utility._formatForFuzzySearch(term))
          return term;
      }
      foreach (string term in word_bank)
      {
        if (Utility._formatForFuzzySearch(term).StartsWith(Utility._formatForFuzzySearch(query)))
          return term;
      }
      foreach (string term in word_bank)
      {
        if (Utility._formatForFuzzySearch(term).Contains(Utility._formatForFuzzySearch(query)))
          return term;
      }
      return (string) null;
    }

    protected static string _formatForFuzzySearch(string term)
    {
      string str = term.Trim().ToLowerInvariant().Replace(" ", "").Replace("(", "").Replace(")", "").Replace("'", "").Replace(".", "").Replace("!", "").Replace("?", "").Replace("-", "");
      return str.Length == 0 ? term.Trim().ToLowerInvariant().Replace(" ", "") : str;
    }

    public static Item fuzzyItemSearch(string query, int stack_count = 1)
    {
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      foreach (int key1 in (IEnumerable<int>) Game1.objectInformation.Keys)
      {
        string str = Game1.objectInformation[key1];
        if (str != null)
        {
          string[] strArray = str.Split('/');
          string key2 = strArray[0];
          if (!(key2 == "Stone") || key1 == 390)
            dictionary1[key2] = !(strArray[3] == "Ring") ? "O " + key1.ToString() + " " + stack_count.ToString() : "R " + key1.ToString() + " " + stack_count.ToString();
        }
      }
      foreach (int key3 in (IEnumerable<int>) Game1.bigCraftablesInformation.Keys)
      {
        string str = Game1.bigCraftablesInformation[key3];
        if (str != null)
        {
          string key4 = str.Substring(0, str.IndexOf('/'));
          dictionary1[key4] = "BO " + key3.ToString() + " " + stack_count.ToString();
        }
      }
      Dictionary<int, string> dictionary2 = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
      foreach (int key5 in dictionary2.Keys)
      {
        string str = dictionary2[key5];
        if (str != null)
        {
          string key6 = str.Substring(0, str.IndexOf('/'));
          dictionary1[key6] = "F " + key5.ToString() + " " + stack_count.ToString();
        }
      }
      Dictionary<int, string> dictionary3 = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
      foreach (int key7 in dictionary3.Keys)
      {
        string str = dictionary3[key7];
        if (str != null)
        {
          string key8 = str.Substring(0, str.IndexOf('/'));
          dictionary1[key8] = "W " + key7.ToString() + " " + stack_count.ToString();
        }
      }
      Dictionary<int, string> dictionary4 = Game1.content.Load<Dictionary<int, string>>("Data\\Boots");
      foreach (int key9 in dictionary4.Keys)
      {
        string str = dictionary4[key9];
        if (str != null)
        {
          string key10 = str.Substring(0, str.IndexOf('/'));
          dictionary1[key10] = "B " + key9.ToString() + " " + stack_count.ToString();
        }
      }
      Dictionary<int, string> dictionary5 = Game1.content.Load<Dictionary<int, string>>("Data\\hats");
      foreach (int key11 in dictionary5.Keys)
      {
        string str = dictionary5[key11];
        if (str != null)
        {
          string key12 = str.Substring(0, str.IndexOf('/'));
          dictionary1[key12] = "H " + key11.ToString() + " " + stack_count.ToString();
        }
      }
      foreach (int key13 in (IEnumerable<int>) Game1.clothingInformation.Keys)
      {
        string str = Game1.clothingInformation[key13];
        if (str != null)
        {
          string key14 = str.Substring(0, str.IndexOf('/'));
          dictionary1[key14] = "C " + key13.ToString() + " " + stack_count.ToString();
        }
      }
      string key = Utility.fuzzySearch(query, dictionary1.Keys.ToList<string>());
      return key != null ? Utility.getItemFromStandardTextDescription(dictionary1[key], (Farmer) null) : (Item) null;
    }

    public static GameLocation fuzzyLocationSearch(string query)
    {
      Dictionary<string, GameLocation> dictionary = new Dictionary<string, GameLocation>();
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        dictionary[location.NameOrUniqueName] = location;
        if (location is BuildableGameLocation)
        {
          foreach (Building building in (location as BuildableGameLocation).buildings)
          {
            if (building.indoors.Value != null)
              dictionary[building.indoors.Value.NameOrUniqueName] = building.indoors.Value;
          }
        }
      }
      string key = Utility.fuzzySearch(query, dictionary.Keys.ToList<string>());
      return key != null ? dictionary[key] : (GameLocation) null;
    }

    public static string AOrAn(string text)
    {
      if (text != null && text.Length > 0)
      {
        switch (text.ToLowerInvariant()[0])
        {
          case 'a':
          case 'e':
          case 'i':
          case 'o':
          case 'u':
            return LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.hu ? "az" : "an";
        }
      }
      return "a";
    }

    public static void getDefaultWarpLocation(string location_name, ref int x, ref int y)
    {
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(location_name))
      {
        case 10819997:
          if (!(location_name == "LeoTreeHouse"))
            return;
          x = 3;
          y = 4;
          return;
        case 144182059:
          if (!(location_name == "WizardHouseBasement"))
            return;
          x = 4;
          y = 4;
          return;
        case 263498407:
          if (!(location_name == "BathHouse_Pool"))
            return;
          x = 13;
          y = 5;
          return;
        case 278567071:
          if (!(location_name == "HarveyRoom"))
            return;
          x = 6;
          y = 11;
          return;
        case 437214172:
          if (!(location_name == "Desert"))
            return;
          x = 35;
          y = 43;
          return;
        case 486972589:
          if (!(location_name == "BoatTunnel"))
            return;
          x = 6;
          y = 11;
          return;
        case 524243468:
          if (!(location_name == "BugLand"))
            return;
          x = 14;
          y = 52;
          return;
        case 636013712:
          if (!(location_name == "HaleyHouse"))
            return;
          x = 2;
          y = 23;
          return;
        case 708811392:
          if (!(location_name == "Tent"))
            return;
          x = 2;
          y = 4;
          return;
        case 720888915:
          if (!(location_name == "JojaMart"))
            return;
          x = 13;
          y = 28;
          return;
        case 746089795:
          if (!(location_name == "ScienceHouse"))
            return;
          x = 8;
          y = 20;
          return;
        case 807500499:
          if (!(location_name == "Hospital"))
            return;
          x = 10;
          y = 18;
          return;
        case 971922314:
          if (!(location_name == "Barn2"))
            return;
          break;
        case 988699933:
          if (!(location_name == "Barn3"))
            return;
          break;
        case 1158526071:
          if (!(location_name == "Submarine"))
            return;
          x = 14;
          y = 14;
          return;
        case 1167876998:
          if (!(location_name == "ManorHouse"))
            return;
          x = 4;
          y = 10;
          return;
        case 1253908523:
          if (!(location_name == "JoshHouse"))
            return;
          x = 9;
          y = 20;
          return;
        case 1367472567:
          if (!(location_name == "Blacksmith"))
            return;
          x = 3;
          y = 15;
          return;
        case 1421832993:
          if (!(location_name == "Caldera"))
            return;
          x = 21;
          y = 30;
          return;
        case 1428365440:
          if (!(location_name == "SeedShop"))
            return;
          x = 4;
          y = 19;
          return;
        case 1446049731:
          if (!(location_name == "CommunityCenter"))
            return;
          x = 32;
          y = 13;
          return;
        case 1553824068:
          if (!(location_name == "MermaidHouse"))
            return;
          x = 4;
          y = 9;
          return;
        case 1659169913:
          if (!(location_name == "IslandEast"))
            return;
          x = 21;
          y = 37;
          return;
        case 1667813495:
          if (!(location_name == "Tunnel"))
            return;
          x = 17;
          y = 7;
          return;
        case 1684694008:
          if (!(location_name == "Coop"))
            return;
          goto label_186;
        case 1695005214:
          if (!(location_name == "Sunroom"))
            return;
          x = 5;
          y = 12;
          return;
        case 1750581136:
          if (!(location_name == "IslandFieldOffice"))
            return;
          x = 8;
          y = 8;
          return;
        case 1755079725:
          if (!(location_name == "IslandFarmCave"))
            return;
          x = 4;
          y = 10;
          return;
        case 1840909614:
          if (!(location_name == "SandyHouse"))
            return;
          x = 2;
          y = 7;
          return;
        case 1919215024:
          if (!(location_name == "ElliottHouse"))
            return;
          x = 3;
          y = 9;
          return;
        case 1940048251:
          if (!(location_name == "IslandWest"))
            return;
          x = 77;
          y = 40;
          return;
        case 2028543928:
          if (!(location_name == "Backwoods"))
            return;
          x = 18;
          y = 18;
          return;
        case 2045747172:
          if (!(location_name == "IslandFarmHouse"))
            return;
          x = 14;
          y = 17;
          return;
        case 2151182681:
          if (!(location_name == "Farm"))
            return;
          x = 64;
          y = 15;
          return;
        case 2229837505:
          if (!(location_name == "Sewer"))
            return;
          x = 31;
          y = 18;
          return;
        case 2233558176:
          if (!(location_name == "Greenhouse"))
            return;
          x = 9;
          y = 8;
          return;
        case 2273535120:
          if (!(location_name == "IslandSecret"))
            return;
          x = 80;
          y = 68;
          return;
        case 2478616111:
          if (!(location_name == "BathHouse_Entry"))
            return;
          x = 5;
          y = 8;
          return;
        case 2503779456:
          if (!(location_name == "Forest"))
            return;
          x = 27;
          y = 12;
          return;
        case 2563511424:
          if (!(location_name == "Trailer"))
            return;
          x = 12;
          y = 9;
          return;
        case 2680503661:
          if (!(location_name == "AbandonedJojaMart"))
            return;
          x = 9;
          y = 12;
          return;
        case 2706464810:
          if (!(location_name == "WizardHouse"))
            return;
          x = 6;
          y = 18;
          return;
        case 2708986271:
          if (!(location_name == "ArchaeologyHouse"))
            return;
          x = 3;
          y = 10;
          return;
        case 2769858145:
          if (!(location_name == "Trailer_Big"))
            return;
          x = 13;
          y = 23;
          return;
        case 2841403676:
          if (!(location_name == "WitchSwamp"))
            return;
          x = 20;
          y = 30;
          return;
        case 2844260897:
          if (!(location_name == "Woods"))
            return;
          x = 8;
          y = 9;
          return;
        case 2880011553:
          if (!(location_name == "IslandSouthEastCave"))
            return;
          x = 2;
          y = 7;
          return;
        case 2880093601:
          if (!(location_name == "QiNutRoom"))
            return;
          x = 7;
          y = 7;
          return;
        case 2909376585:
          if (!(location_name == "Saloon"))
            return;
          x = 18;
          y = 20;
          return;
        case 2967988020:
          if (!(location_name == "MovieTheater"))
            return;
          x = 8;
          y = 9;
          return;
        case 3006626703:
          if (!(location_name == "FishShop"))
            return;
          x = 6;
          y = 6;
          return;
        case 3014964069:
          if (!(location_name == "Town"))
            return;
          x = 29;
          y = 67;
          return;
        case 3030632101:
          if (!(location_name == "LeahHouse"))
            return;
          x = 7;
          y = 9;
          return;
        case 3059129661:
          if (!(location_name == "IslandSouth"))
            return;
          x = 21;
          y = 43;
          return;
        case 3064172445:
          if (!(location_name == "IslandHut"))
            return;
          x = 7;
          y = 11;
          return;
        case 3095702198:
          if (!(location_name == "AdventureGuild"))
            return;
          x = 4;
          y = 13;
          return;
        case 3183088828:
          if (!(location_name == "Barn"))
            return;
          break;
        case 3188665151:
          if (!(location_name == "Railroad"))
            return;
          x = 29;
          y = 58;
          return;
        case 3223558537:
          if (!(location_name == "BusStop"))
            return;
          x = 14;
          y = 23;
          return;
        case 3308967874:
          if (!(location_name == "Mountain"))
            return;
          x = 40;
          y = 13;
          return;
        case 3333348840:
          if (!(location_name == "Beach"))
            return;
          x = 39;
          y = 1;
          return;
        case 3354701620:
          if (!(location_name == "IslandSouthEast"))
            return;
          x = 0;
          y = 28;
          return;
        case 3647688262:
          if (!(location_name == "BathHouse_WomensLocker"))
            return;
          x = 2;
          y = 14;
          return;
        case 3653788295:
          if (!(location_name == "SkullCave"))
            return;
          x = 3;
          y = 4;
          return;
        case 3703500451:
          if (!(location_name == "SlimeHutch"))
            return;
          x = 8;
          y = 18;
          return;
        case 3715831550:
          if (!(location_name == "Coop2"))
            return;
          goto label_186;
        case 3732609169:
          if (!(location_name == "Coop3"))
            return;
          goto label_186;
        case 3755589785:
          if (!(location_name == "WitchHut"))
            return;
          x = 7;
          y = 14;
          return;
        case 3760230863:
          if (!(location_name == "IslandNorth"))
            return;
          x = 36;
          y = 89;
          return;
        case 3848897750:
          if (!(location_name == "Mine"))
            return;
          x = 13;
          y = 10;
          return;
        case 3924195856:
          if (!(location_name == "BathHouse_MensLocker"))
            return;
          x = 15;
          y = 16;
          return;
        case 3962580640:
          if (!(location_name == "SamHouse"))
            return;
          x = 4;
          y = 15;
          return;
        case 3978811393:
          if (!(location_name == "AnimalShop"))
            return;
          x = 12;
          y = 16;
          return;
        case 3979572909:
          if (!(location_name == "Club"))
            return;
          x = 8;
          y = 11;
          return;
        case 4002456539:
          if (!(location_name == "WitchWarpCave"))
            return;
          x = 4;
          y = 8;
          return;
        case 4156484379:
          if (!(location_name == "IslandShrine"))
            return;
          x = 16;
          y = 28;
          return;
        default:
          return;
      }
      x = 11;
      y = 13;
      return;
label_186:
      x = 2;
      y = 8;
    }

    public static FarmAnimal fuzzyAnimalSearch(string query)
    {
      List<FarmAnimal> farmAnimalList = new List<FarmAnimal>();
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (location is IAnimalLocation)
          farmAnimalList.AddRange((IEnumerable<FarmAnimal>) (location as IAnimalLocation).Animals.Values);
        if (location is BuildableGameLocation)
        {
          foreach (Building building in (location as BuildableGameLocation).buildings)
          {
            if (building.indoors.Value != null && building.indoors.Value is IAnimalLocation)
              farmAnimalList.AddRange((IEnumerable<FarmAnimal>) (building.indoors.Value as IAnimalLocation).Animals.Values);
          }
        }
      }
      Dictionary<string, FarmAnimal> dictionary = new Dictionary<string, FarmAnimal>();
      foreach (FarmAnimal farmAnimal in farmAnimalList)
        dictionary[farmAnimal.Name] = farmAnimal;
      string key = Utility.fuzzySearch(query, dictionary.Keys.ToList<string>());
      return key != null ? dictionary[key] : (FarmAnimal) null;
    }

    public static NPC fuzzyCharacterSearch(string query, bool must_be_villager = true)
    {
      List<NPC> list = new List<NPC>();
      Utility.getAllCharacters(list);
      Dictionary<string, NPC> dictionary = new Dictionary<string, NPC>();
      foreach (NPC npc in list)
      {
        if (!must_be_villager || npc.isVillager())
          dictionary[npc.Name] = npc;
      }
      string key = Utility.fuzzySearch(query, dictionary.Keys.ToList<string>());
      return key != null ? dictionary[key] : (NPC) null;
    }

    public static Microsoft.Xna.Framework.Color GetPrismaticColor(
      int offset = 0,
      float speedMultiplier = 1f)
    {
      float num = 1500f;
      int index1 = ((int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds * (double) speedMultiplier / (double) num) + offset) % Utility.PRISMATIC_COLORS.Length;
      int index2 = (index1 + 1) % Utility.PRISMATIC_COLORS.Length;
      float t = (float) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds * (double) speedMultiplier / (double) num % 1.0);
      return new Microsoft.Xna.Framework.Color()
      {
        R = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].R / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].R / (float) byte.MaxValue, t) * (double) byte.MaxValue),
        G = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].G / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].G / (float) byte.MaxValue, t) * (double) byte.MaxValue),
        B = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].B / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].B / (float) byte.MaxValue, t) * (double) byte.MaxValue),
        A = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].A / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].A / (float) byte.MaxValue, t) * (double) byte.MaxValue)
      };
    }

    public static Microsoft.Xna.Framework.Color Get2PhaseColor(
      Microsoft.Xna.Framework.Color color1,
      Microsoft.Xna.Framework.Color color2,
      int offset = 0,
      float speedMultiplier = 1f,
      float timeOffset = 0.0f)
    {
      float num1 = 1500f;
      TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
      int num2 = ((int) ((double) ((float) totalGameTime.TotalMilliseconds + timeOffset) * (double) speedMultiplier / (double) num1) + offset) % 2;
      int num3 = (num2 + 1) % 2;
      totalGameTime = Game1.currentGameTime.TotalGameTime;
      float t = (float) ((double) ((float) totalGameTime.TotalMilliseconds + timeOffset) * (double) speedMultiplier / (double) num1 % 1.0);
      Microsoft.Xna.Framework.Color color3 = new Microsoft.Xna.Framework.Color();
      Microsoft.Xna.Framework.Color color4 = num2 == 0 ? color1 : color2;
      Microsoft.Xna.Framework.Color color5 = num2 == 0 ? color2 : color1;
      color3.R = (byte) ((double) Utility.Lerp((float) color4.R / (float) byte.MaxValue, (float) color5.R / (float) byte.MaxValue, t) * (double) byte.MaxValue);
      color3.G = (byte) ((double) Utility.Lerp((float) color4.G / (float) byte.MaxValue, (float) color5.G / (float) byte.MaxValue, t) * (double) byte.MaxValue);
      color3.B = (byte) ((double) Utility.Lerp((float) color4.B / (float) byte.MaxValue, (float) color5.B / (float) byte.MaxValue, t) * (double) byte.MaxValue);
      color3.A = (byte) ((double) Utility.Lerp((float) color4.A / (float) byte.MaxValue, (float) color5.A / (float) byte.MaxValue, t) * (double) byte.MaxValue);
      return color3;
    }

    public static bool IsNormalObjectAtParentSheetIndex(Item item, int index) => item != null && !(item.GetType() != typeof (Object)) && !(item as Object).bigCraftable.Value && item.ParentSheetIndex == index;

    public static bool isObjectOffLimitsForSale(int index)
    {
      switch (index)
      {
        case 69:
        case 73:
        case 79:
        case 91:
        case 158:
        case 159:
        case 160:
        case 161:
        case 162:
        case 163:
        case 261:
        case 277:
        case 279:
        case 289:
        case 292:
        case 305:
        case 308:
        case 326:
        case 341:
        case 413:
        case 417:
        case 437:
        case 439:
        case 447:
        case 454:
        case 460:
        case 645:
        case 680:
        case 681:
        case 682:
        case 688:
        case 689:
        case 690:
        case 774:
        case 775:
        case 797:
        case 798:
        case 799:
        case 800:
        case 801:
        case 802:
        case 803:
        case 807:
        case 812:
          return true;
        default:
          return false;
      }
    }

    public static Microsoft.Xna.Framework.Rectangle xTileToMicrosoftRectangle(
      xTile.Dimensions.Rectangle rect)
    {
      return new Microsoft.Xna.Framework.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static Microsoft.Xna.Framework.Rectangle getSafeArea()
    {
      Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
      if (Game1.game1.GraphicsDevice.GetRenderTargets().Length == 0)
      {
        float num = 1f / Game1.options.zoomLevel;
        if (Game1.uiMode)
          num = 1f / Game1.options.uiScale;
        titleSafeArea.X = (int) ((double) titleSafeArea.X * (double) num);
        titleSafeArea.Y = (int) ((double) titleSafeArea.Y * (double) num);
        titleSafeArea.Width = (int) ((double) titleSafeArea.Width * (double) num);
        titleSafeArea.Height = (int) ((double) titleSafeArea.Height * (double) num);
      }
      return titleSafeArea;
    }

    /// <summary>
    /// Return the adjusted renderPos such that bounds implied by renderSize
    /// is within the TitleSafeArea.
    /// 
    /// If it already is, renderPos is returned unmodified.
    /// </summary>
    public static Vector2 makeSafe(Vector2 renderPos, Vector2 renderSize)
    {
      Utility.getSafeArea();
      int x1 = (int) renderPos.X;
      int y1 = (int) renderPos.Y;
      int x2 = (int) renderSize.X;
      int y2 = (int) renderSize.Y;
      Utility.makeSafe(ref x1, ref y1, x2, y2);
      return new Vector2((float) x1, (float) y1);
    }

    public static void makeSafe(ref Vector2 position, int width, int height)
    {
      int x = (int) position.X;
      int y = (int) position.Y;
      Utility.makeSafe(ref x, ref y, width, height);
      position.X = (float) x;
      position.Y = (float) y;
    }

    public static void makeSafe(ref Microsoft.Xna.Framework.Rectangle bounds) => Utility.makeSafe(ref bounds.X, ref bounds.Y, bounds.Width, bounds.Height);

    public static void makeSafe(ref int x, ref int y, int width, int height)
    {
      Microsoft.Xna.Framework.Rectangle safeArea = Utility.getSafeArea();
      if (x < safeArea.Left)
        x = safeArea.Left;
      if (y < safeArea.Top)
        y = safeArea.Top;
      if (x + width > safeArea.Right)
        x = safeArea.Right - width;
      if (y + height <= safeArea.Bottom)
        return;
      y = safeArea.Bottom - height;
    }

    internal static void makeSafeY(ref int y, int height)
    {
      Vector2 vector2 = Utility.makeSafe(new Vector2(0.0f, (float) y), new Vector2(0.0f, (float) height));
      y = (int) vector2.Y;
    }

    public static int makeSafeMarginX(int marginx)
    {
      Viewport viewport = Game1.game1.GraphicsDevice.Viewport;
      Microsoft.Xna.Framework.Rectangle safeArea = Utility.getSafeArea();
      if (safeArea.Left > viewport.Bounds.Left)
        marginx = safeArea.Left;
      int num = safeArea.Right - viewport.Bounds.Right;
      if (num > marginx)
        marginx = num;
      return marginx;
    }

    public static int makeSafeMarginY(int marginy)
    {
      Viewport viewport = Game1.game1.GraphicsDevice.Viewport;
      Microsoft.Xna.Framework.Rectangle safeArea = Utility.getSafeArea();
      int num1 = safeArea.Top - viewport.Bounds.Top;
      if (num1 > marginy)
        marginy = num1;
      int num2 = viewport.Bounds.Bottom - safeArea.Bottom;
      if (num2 > marginy)
        marginy = num2;
      return marginy;
    }

    public static bool onTravelingMerchantShopPurchase(ISalable item, Farmer farmer, int amount)
    {
      Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.TravelingMerchant, item, amount);
      return false;
    }

    private static Dictionary<ISalable, int[]> generateLocalTravelingMerchantStock(
      int seed)
    {
      Dictionary<ISalable, int[]> stock1 = new Dictionary<ISalable, int[]>();
      HashSet<int> stockIndices1 = new HashSet<int>();
      Random r = new Random(seed);
      bool flag = false;
      if (Game1.netWorldState.Value.VisitsUntilY1Guarantee == 0)
        flag = true;
      for (int index = 0; index < 10; ++index)
      {
        int num = r.Next(2, 790);
        Dictionary<ISalable, int[]> stock2;
        HashSet<int> stockIndices2;
        Object objectToAdd;
        int[] listing;
        do
        {
          string[] strArray;
          do
          {
            do
            {
              num = (num + 1) % 790;
            }
            while (!Game1.objectInformation.ContainsKey(num) || Utility.isObjectOffLimitsForSale(num));
            if (num == 266 || num == 485)
              flag = false;
            strArray = Game1.objectInformation[num].Split('/');
          }
          while (!strArray[3].Contains('-') || Convert.ToInt32(strArray[1]) <= 0 || strArray[3].Contains("-13") || strArray[3].Equals("Quest") || strArray[0].Equals("Weeds") || strArray[3].Contains("Minerals") || strArray[3].Contains("Arch"));
          stock2 = stock1;
          stockIndices2 = stockIndices1;
          objectToAdd = new Object(num, 1);
          listing = new int[2]
          {
            Math.Max(r.Next(1, 11) * 100, Convert.ToInt32(strArray[1]) * r.Next(3, 6)),
            r.NextDouble() < 0.1 ? 5 : 1
          };
        }
        while (!Utility.addToStock(stock2, stockIndices2, objectToAdd, listing));
      }
      if (flag)
      {
        string[] strArray = Game1.objectInformation[485].Split('/');
        Utility.addToStock(stock1, stockIndices1, new Object(485, 1), new int[2]
        {
          Math.Max(r.Next(1, 11) * 100, Convert.ToInt32(strArray[1]) * r.Next(3, 6)),
          r.NextDouble() < 0.1 ? 5 : 1
        });
      }
      Utility.addToStock(stock1, stockIndices1, (Object) Utility.getRandomFurniture(r, (Dictionary<ISalable, int[]>) null, upperIndexBound: 1613), new int[2]
      {
        r.Next(1, 11) * 250,
        1
      });
      if (Utility.getSeasonNumber(Game1.currentSeason) < 2)
        Utility.addToStock(stock1, stockIndices1, new Object(347, 1), new int[2]
        {
          1000,
          r.NextDouble() < 0.1 ? 5 : 1
        });
      else if (r.NextDouble() < 0.4)
        Utility.addToStock(stock1, stockIndices1, new Object(Vector2.Zero, 136), new int[2]
        {
          4000,
          1
        });
      if (r.NextDouble() < 0.25)
        Utility.addToStock(stock1, stockIndices1, new Object(433, 1), new int[2]
        {
          2500,
          1
        });
      return stock1;
    }

    public static Dictionary<ISalable, int[]> getTravelingMerchantStock(int seed)
    {
      Dictionary<ISalable, int[]> travelingMerchantStock = Utility.generateLocalTravelingMerchantStock(seed);
      Game1.player.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(SynchronizedShopStock.SynchedShop.TravelingMerchant, travelingMerchantStock);
      if (Game1.IsMultiplayer && !Game1.player.craftingRecipes.ContainsKey("Wedding Ring"))
      {
        Object key = new Object(801, 1, true);
        travelingMerchantStock.Add((ISalable) key, new int[2]
        {
          500,
          1
        });
      }
      return travelingMerchantStock;
    }

    private static bool addToStock(
      Dictionary<ISalable, int[]> stock,
      HashSet<int> stockIndices,
      Object objectToAdd,
      int[] listing)
    {
      int parentSheetIndex = objectToAdd.ParentSheetIndex;
      if (stockIndices.Contains(parentSheetIndex))
        return false;
      stock.Add((ISalable) objectToAdd, listing);
      stockIndices.Add(parentSheetIndex);
      return true;
    }

    public static Dictionary<ISalable, int[]> getDwarfShopStock()
    {
      Dictionary<ISalable, int[]> dwarfShopStock = new Dictionary<ISalable, int[]>();
      dwarfShopStock.Add((ISalable) new Object(773, 1), new int[2]
      {
        2000,
        int.MaxValue
      });
      dwarfShopStock.Add((ISalable) new Object(772, 1), new int[2]
      {
        3000,
        int.MaxValue
      });
      dwarfShopStock.Add((ISalable) new Object(286, 1), new int[2]
      {
        300,
        int.MaxValue
      });
      dwarfShopStock.Add((ISalable) new Object(287, 1), new int[2]
      {
        600,
        int.MaxValue
      });
      dwarfShopStock.Add((ISalable) new Object(288, 1), new int[2]
      {
        1000,
        int.MaxValue
      });
      dwarfShopStock.Add((ISalable) new Object(243, 1), new int[2]
      {
        1000,
        int.MaxValue
      });
      dwarfShopStock.Add((ISalable) new Object(Vector2.Zero, 138), new int[2]
      {
        2500,
        int.MaxValue
      });
      dwarfShopStock.Add((ISalable) new Object(Vector2.Zero, 32), new int[2]
      {
        200,
        int.MaxValue
      });
      if (!Game1.player.craftingRecipes.ContainsKey("Weathered Floor"))
        dwarfShopStock.Add((ISalable) new Object(331, 1, true), new int[2]
        {
          500,
          1
        });
      return dwarfShopStock;
    }

    public static Dictionary<ISalable, int[]> getHospitalStock() => new Dictionary<ISalable, int[]>()
    {
      {
        (ISalable) new Object(349, 1),
        new int[2]{ 1000, int.MaxValue }
      },
      {
        (ISalable) new Object(351, 1),
        new int[2]{ 1000, int.MaxValue }
      }
    };

    public static int CompareGameVersions(
      string version,
      string other_version,
      bool ignore_platform_specific = false)
    {
      string[] strArray1 = version.Split('.');
      string[] strArray2 = other_version.Split('.');
      for (int index = 0; index < Math.Max(strArray1.Length, strArray2.Length); ++index)
      {
        float result1 = 0.0f;
        float result2 = 0.0f;
        if (index < strArray1.Length)
          float.TryParse(strArray1[index], out result1);
        if (index < strArray2.Length)
          float.TryParse(strArray2[index], out result2);
        if ((double) result1 != (double) result2 || index == 2 & ignore_platform_specific)
          return result1.CompareTo(result2);
      }
      return 0;
    }

    public static float getFarmerItemsShippedPercent(Farmer who = null)
    {
      if (who == null)
        who = Game1.player;
      int num1 = 0;
      int num2 = 0;
      foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) Game1.objectInformation)
      {
        string str = keyValuePair.Value.Split('/')[3];
        if (!str.Contains("Arch") && !str.Contains("Fish") && !str.Contains("Mineral") && !str.Substring(str.Length - 3).Equals("-2") && !str.Contains("Cooking") && !str.Substring(str.Length - 3).Equals("-7") && Object.isPotentialBasicShippedCategory(keyValuePair.Key, str.Substring(str.Length - 3)))
        {
          ++num2;
          if (who.basicShipped.ContainsKey(keyValuePair.Key))
            ++num1;
        }
      }
      return (float) num1 / (float) num2;
    }

    public static bool hasFarmerShippedAllItems() => (double) Utility.getFarmerItemsShippedPercent() >= 1.0;

    public static Dictionary<ISalable, int[]> getQiShopStock() => new Dictionary<ISalable, int[]>()
    {
      {
        (ISalable) new Furniture(1552, Vector2.Zero),
        new int[2]{ 5000, int.MaxValue }
      },
      {
        (ISalable) new Furniture(1545, Vector2.Zero),
        new int[2]{ 4000, int.MaxValue }
      },
      {
        (ISalable) new Furniture(1563, Vector2.Zero),
        new int[2]{ 4000, int.MaxValue }
      },
      {
        (ISalable) new Furniture(1561, Vector2.Zero),
        new int[2]{ 3000, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Objects.Hat(2),
        new int[2]{ 8000, int.MaxValue }
      },
      {
        (ISalable) new Object(Vector2.Zero, 126),
        new int[2]{ 10000, int.MaxValue }
      },
      {
        (ISalable) new Object(298, 1),
        new int[2]{ 100, int.MaxValue }
      },
      {
        (ISalable) new Object(703, 1),
        new int[2]{ 1000, int.MaxValue }
      },
      {
        (ISalable) new Object(688, 1),
        new int[2]{ 500, int.MaxValue }
      },
      {
        (ISalable) new BedFurniture(2192, Vector2.Zero),
        new int[2]{ 8000, int.MaxValue }
      }
    };

    public static Dictionary<ISalable, int[]> getJojaStock()
    {
      Dictionary<ISalable, int[]> jojaStock = new Dictionary<ISalable, int[]>();
      if (Game1.MasterPlayer.eventsSeen.Contains(502261))
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 272), new int[2]
        {
          50000,
          int.MaxValue
        });
      jojaStock.Add((ISalable) new Object(Vector2.Zero, 167, int.MaxValue), new int[2]
      {
        75,
        int.MaxValue
      });
      Dictionary<ISalable, int[]> dictionary1 = jojaStock;
      Wallpaper key1 = new Wallpaper(21);
      key1.Stack = int.MaxValue;
      int[] numArray1 = new int[2]{ 20, int.MaxValue };
      dictionary1.Add((ISalable) key1, numArray1);
      Dictionary<ISalable, int[]> dictionary2 = jojaStock;
      Furniture key2 = new Furniture(1609, Vector2.Zero);
      key2.Stack = int.MaxValue;
      int[] numArray2 = new int[2]{ 500, int.MaxValue };
      dictionary2.Add((ISalable) key2, numArray2);
      float num = (Game1.player.hasOrWillReceiveMail("JojaMember") ? 2f : 2.5f) * Game1.MasterPlayer.difficultyModifier;
      if (Game1.currentSeason.Equals("spring"))
      {
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 472, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[472].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 473, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[473].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 474, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[474].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 475, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[475].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 427, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[427].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 429, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[429].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 477, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[477].Split('/')[1]) * (double) num),
          int.MaxValue
        });
      }
      if (Game1.currentSeason.Equals("summer"))
      {
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 480, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[480].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 482, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[482].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 483, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[483].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 484, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[484].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 479, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[479].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 302, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[302].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 453, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[453].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 455, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[455].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(431, int.MaxValue, price: 100), new int[2]
        {
          (int) (50.0 * (double) num),
          int.MaxValue
        });
      }
      if (Game1.currentSeason.Equals("fall"))
      {
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 487, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[487].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 488, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[488].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 483, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[483].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 490, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[490].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 299, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[299].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 301, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[301].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 492, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[492].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 491, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[491].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 493, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[493].Split('/')[1]) * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(431, int.MaxValue, price: 100), new int[2]
        {
          (int) (50.0 * (double) num),
          int.MaxValue
        });
        jojaStock.Add((ISalable) new Object(Vector2.Zero, 425, int.MaxValue), new int[2]
        {
          (int) ((double) Convert.ToInt32(Game1.objectInformation[425].Split('/')[1]) * (double) num),
          int.MaxValue
        });
      }
      jojaStock.Add((ISalable) new Object(Vector2.Zero, 297, int.MaxValue), new int[2]
      {
        (int) ((double) Convert.ToInt32(Game1.objectInformation[297].Split('/')[1]) * (double) num),
        int.MaxValue
      });
      jojaStock.Add((ISalable) new Object(Vector2.Zero, 245, int.MaxValue), new int[2]
      {
        (int) ((double) Convert.ToInt32(Game1.objectInformation[245].Split('/')[1]) * (double) num),
        int.MaxValue
      });
      jojaStock.Add((ISalable) new Object(Vector2.Zero, 246, int.MaxValue), new int[2]
      {
        (int) ((double) Convert.ToInt32(Game1.objectInformation[246].Split('/')[1]) * (double) num),
        int.MaxValue
      });
      jojaStock.Add((ISalable) new Object(Vector2.Zero, 423, int.MaxValue), new int[2]
      {
        (int) ((double) Convert.ToInt32(Game1.objectInformation[423].Split('/')[1]) * (double) num),
        int.MaxValue
      });
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + 1);
      int which = random.Next(112);
      if (which == 21)
        which = 22;
      Dictionary<ISalable, int[]> dictionary3 = jojaStock;
      Wallpaper key3 = new Wallpaper(which);
      key3.Stack = int.MaxValue;
      int[] numArray3 = new int[2]{ 250, int.MaxValue };
      dictionary3.Add((ISalable) key3, numArray3);
      Dictionary<ISalable, int[]> dictionary4 = jojaStock;
      Wallpaper key4 = new Wallpaper(random.Next(40), true);
      key4.Stack = int.MaxValue;
      int[] numArray4 = new int[2]{ 250, int.MaxValue };
      dictionary4.Add((ISalable) key4, numArray4);
      return jojaStock;
    }

    public static Dictionary<ISalable, int[]> getHatStock()
    {
      Dictionary<ISalable, int[]> hatStock = new Dictionary<ISalable, int[]>();
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
      bool flag = true;
      foreach (KeyValuePair<int, string> keyValuePair in dictionary)
      {
        if (Game1.player.achievements.Contains(keyValuePair.Key))
          hatStock.Add((ISalable) new StardewValley.Objects.Hat(Convert.ToInt32(keyValuePair.Value.Split('^')[4])), new int[2]
          {
            1000,
            int.MaxValue
          });
        else
          flag = false;
      }
      if (Game1.player.mailReceived.Contains("Egg Festival"))
        hatStock.Add((ISalable) new StardewValley.Objects.Hat(4), new int[2]
        {
          1000,
          int.MaxValue
        });
      if (Game1.player.mailReceived.Contains("Ice Festival"))
        hatStock.Add((ISalable) new StardewValley.Objects.Hat(17), new int[2]
        {
          1000,
          int.MaxValue
        });
      if (Game1.player.achievements.Contains(17))
        hatStock.Add((ISalable) new StardewValley.Objects.Hat(61), new int[2]
        {
          1000,
          int.MaxValue
        });
      if (flag)
        hatStock.Add((ISalable) new StardewValley.Objects.Hat(64), new int[2]
        {
          1000,
          int.MaxValue
        });
      return hatStock;
    }

    public static NPC getTodaysBirthdayNPC(string season, int day)
    {
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (allCharacter.isBirthday(season, day))
          return allCharacter;
      }
      return (NPC) null;
    }

    public static bool highlightEdibleItems(Item i) => i is Object && (int) (NetFieldBase<int, NetInt>) (i as Object).edibility != -300;

    public static T GetRandom<T>(List<T> list, Random random = null)
    {
      if (list == null || list.Count == 0)
        return default (T);
      if (random == null)
        random = Game1.random;
      return list[random.Next(list.Count)];
    }

    public static int getRandomSingleTileFurniture(Random r)
    {
      switch (r.Next(3))
      {
        case 0:
          return r.Next(10) * 3;
        case 1:
          return r.Next(1376, 1391);
        case 2:
          return r.Next(7) * 2 + 1391;
        default:
          return 0;
      }
    }

    public static void improveFriendshipWithEveryoneInRegion(Farmer who, int amount, int region)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        foreach (NPC character in location.characters)
        {
          if (character.homeRegion == region && who.friendshipData.ContainsKey(character.Name))
            who.changeFriendship(amount, character);
        }
      }
    }

    public static Item getGiftFromNPC(NPC who)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + Game1.year + Game1.dayOfMonth + Utility.getSeasonNumber(Game1.currentSeason) + who.getTileX());
      List<Item> objList = new List<Item>();
      string name = who.Name;
      if (!(name == "Clint"))
      {
        if (!(name == "Marnie"))
        {
          if (!(name == "Robin"))
          {
            if (!(name == "Willy"))
            {
              if (name == "Evelyn")
                objList.Add((Item) new Object(223, 1));
              else if (who.Age == 2)
              {
                objList.Add((Item) new Object(330, 1));
                objList.Add((Item) new Object(103, 1));
                objList.Add((Item) new Object(394, 1));
                objList.Add((Item) new Object(random.Next(535, 538), 1));
              }
              else
              {
                objList.Add((Item) new Object(608, 1));
                objList.Add((Item) new Object(651, 1));
                objList.Add((Item) new Object(611, 1));
                objList.Add((Item) new Ring(517));
                objList.Add((Item) new Object(466, 10));
                objList.Add((Item) new Object(422, 1));
                objList.Add((Item) new Object(392, 1));
                objList.Add((Item) new Object(348, 1));
                objList.Add((Item) new Object(346, 1));
                objList.Add((Item) new Object(341, 1));
                objList.Add((Item) new Object(221, 1));
                objList.Add((Item) new Object(64, 1));
                objList.Add((Item) new Object(60, 1));
                objList.Add((Item) new Object(70, 1));
              }
            }
            else
            {
              objList.Add((Item) new Object(690, 25));
              objList.Add((Item) new Object(687, 1));
              objList.Add((Item) new Object(703, 1));
            }
          }
          else
          {
            objList.Add((Item) new Object(388, 99));
            objList.Add((Item) new Object(390, 50));
            objList.Add((Item) new Object(709, 25));
          }
        }
        else
          objList.Add((Item) new Object(176, 12));
      }
      else
      {
        objList.Add((Item) new Object(337, 1));
        objList.Add((Item) new Object(336, 5));
        objList.Add((Item) new Object(random.Next(535, 538), 5));
      }
      return objList[random.Next(objList.Count)];
    }

    public static NPC getTopRomanticInterest(Farmer who)
    {
      NPC romanticInterest = (NPC) null;
      int num = -1;
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (who.friendshipData.ContainsKey(allCharacter.Name) && (bool) (NetFieldBase<bool, NetBool>) allCharacter.datable && who.getFriendshipLevelForNPC(allCharacter.Name) > num)
        {
          romanticInterest = allCharacter;
          num = who.getFriendshipLevelForNPC(allCharacter.Name);
        }
      }
      return romanticInterest;
    }

    public static Microsoft.Xna.Framework.Color getRandomRainbowColor(Random r = null)
    {
      switch (r == null ? Game1.random.Next(8) : r.Next(8))
      {
        case 0:
          return Microsoft.Xna.Framework.Color.Red;
        case 1:
          return Microsoft.Xna.Framework.Color.Orange;
        case 2:
          return Microsoft.Xna.Framework.Color.Yellow;
        case 3:
          return Microsoft.Xna.Framework.Color.Lime;
        case 4:
          return Microsoft.Xna.Framework.Color.Cyan;
        case 5:
          return new Microsoft.Xna.Framework.Color(0, 100, (int) byte.MaxValue);
        case 6:
          return new Microsoft.Xna.Framework.Color(152, 96, (int) byte.MaxValue);
        case 7:
          return new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 100, (int) byte.MaxValue);
        default:
          return Microsoft.Xna.Framework.Color.White;
      }
    }

    public static NPC getTopNonRomanticInterest(Farmer who)
    {
      NPC romanticInterest = (NPC) null;
      int num = -1;
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (who.friendshipData.ContainsKey(allCharacter.Name) && !(bool) (NetFieldBase<bool, NetBool>) allCharacter.datable && who.getFriendshipLevelForNPC(allCharacter.Name) > num)
        {
          romanticInterest = allCharacter;
          num = who.getFriendshipLevelForNPC(allCharacter.Name);
        }
      }
      return romanticInterest;
    }

    public static int getHighestSkill(Farmer who)
    {
      int num = 0;
      int highestSkill = 0;
      for (int index = 0; index < who.experiencePoints.Length; ++index)
      {
        if (who.experiencePoints[index] > num)
          highestSkill = index;
      }
      return highestSkill;
    }

    public static int getNumberOfFriendsWithinThisRange(
      Farmer who,
      int minFriendshipPoints,
      int maxFriendshipPoints,
      bool romanceOnly = false)
    {
      int friendsWithinThisRange = 0;
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        int? friendshipLevelForNpc = who.tryGetFriendshipLevelForNPC(allCharacter.Name);
        if (friendshipLevelForNpc.HasValue && friendshipLevelForNpc.Value >= minFriendshipPoints && friendshipLevelForNpc.Value <= maxFriendshipPoints && (!romanceOnly || (bool) (NetFieldBase<bool, NetBool>) allCharacter.datable))
          ++friendsWithinThisRange;
      }
      return friendsWithinThisRange;
    }

    public static bool highlightLuauSoupItems(Item i)
    {
      if (!(i is Object))
        return false;
      return (int) (NetFieldBase<int, NetInt>) (i as Object).edibility != -300 && (i as Object).Category != -7 || Utility.IsNormalObjectAtParentSheetIndex(i, 789) || Utility.IsNormalObjectAtParentSheetIndex(i, 71);
    }

    public static bool highlightEdibleNonCookingItems(Item i) => i is Object && (int) (NetFieldBase<int, NetInt>) (i as Object).edibility != -300 && (i as Object).Category != -7;

    public static bool highlightSmallObjects(Item i) => i is Object && !(bool) (NetFieldBase<bool, NetBool>) (i as Object).bigCraftable;

    public static bool highlightSantaObjects(Item i) => i.canBeTrashed() && i.canBeGivenAsGift() && Utility.highlightSmallObjects(i);

    public static bool highlightShippableObjects(Item i) => i is Object && (i as Object).canBeShipped();

    public static Farmer getFarmerFromFarmerNumberString(string s, Farmer defaultFarmer) => s.Equals("farmer") || !s.StartsWith("farmer") ? defaultFarmer : Utility.getFarmerFromFarmerNumber(Convert.ToInt32(s[s.Length - 1].ToString() ?? ""));

    public static int getFarmerNumberFromFarmer(Farmer who)
    {
      for (int number = 1; number <= Game1.CurrentPlayerLimit; ++number)
      {
        if (Utility.getFarmerFromFarmerNumber(number).UniqueMultiplayerID == who.UniqueMultiplayerID)
          return number;
      }
      return -1;
    }

    public static Farmer getFarmerFromFarmerNumber(int number)
    {
      if (!Game1.IsMultiplayer)
        return number == 1 ? Game1.player : (Farmer) null;
      if (number <= 1 && (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null)
        return (Farmer) (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost;
      return number <= Game1.numberOfPlayers() ? Game1.otherFarmers.Values.Where<Farmer>((Func<Farmer, bool>) (f => f != Game1.serverHost.Value)).OrderBy<Farmer, long>((Func<Farmer, long>) (f => f.UniqueMultiplayerID)).ElementAt<Farmer>(number - 2) : (Farmer) null;
    }

    public static string getLoveInterest(string who)
    {
      switch (who)
      {
        case "Abigail":
          return "Sebastian";
        case "Alex":
          return "Haley";
        case "Elliott":
          return "Leah";
        case "Emily":
          return "Shane";
        case "Haley":
          return "Alex";
        case "Harvey":
          return "Maru";
        case "Leah":
          return "Elliott";
        case "Maru":
          return "Harvey";
        case "Penny":
          return "Sam";
        case "Sam":
          return "Penny";
        case "Sebastian":
          return "Abigail";
        case "Shane":
          return "Emily";
        default:
          return "";
      }
    }

    public static Dictionary<ISalable, int[]> getFishShopStock(Farmer who)
    {
      Dictionary<ISalable, int[]> fishShopStock = new Dictionary<ISalable, int[]>();
      fishShopStock.Add((ISalable) new Object(219, 1), new int[2]
      {
        250,
        int.MaxValue
      });
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 2)
        fishShopStock.Add((ISalable) new Object(685, 1), new int[2]
        {
          5,
          int.MaxValue
        });
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 3)
        fishShopStock.Add((ISalable) new Object(710, 1), new int[2]
        {
          1500,
          int.MaxValue
        });
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 6)
      {
        fishShopStock.Add((ISalable) new Object(686, 1), new int[2]
        {
          500,
          int.MaxValue
        });
        fishShopStock.Add((ISalable) new Object(694, 1), new int[2]
        {
          500,
          int.MaxValue
        });
        fishShopStock.Add((ISalable) new Object(692, 1), new int[2]
        {
          200,
          int.MaxValue
        });
      }
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 7)
      {
        fishShopStock.Add((ISalable) new Object(693, 1), new int[2]
        {
          750,
          int.MaxValue
        });
        fishShopStock.Add((ISalable) new Object(695, 1), new int[2]
        {
          750,
          int.MaxValue
        });
      }
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 8)
      {
        fishShopStock.Add((ISalable) new Object(691, 1), new int[2]
        {
          1000,
          int.MaxValue
        });
        fishShopStock.Add((ISalable) new Object(687, 1), new int[2]
        {
          1000,
          int.MaxValue
        });
      }
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 9)
        fishShopStock.Add((ISalable) new Object(703, 1), new int[2]
        {
          1000,
          int.MaxValue
        });
      fishShopStock.Add((ISalable) new FishingRod(0), new int[2]
      {
        500,
        int.MaxValue
      });
      fishShopStock.Add((ISalable) new FishingRod(1), new int[2]
      {
        25,
        int.MaxValue
      });
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 2)
        fishShopStock.Add((ISalable) new FishingRod(2), new int[2]
        {
          1800,
          int.MaxValue
        });
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.fishingLevel >= 6)
        fishShopStock.Add((ISalable) new FishingRod(3), new int[2]
        {
          7500,
          int.MaxValue
        });
      if (Game1.MasterPlayer.mailReceived.Contains("ccFishTank"))
        fishShopStock.Add((ISalable) new Pan(), new int[2]
        {
          2500,
          int.MaxValue
        });
      fishShopStock.Add((ISalable) new FishTankFurniture(2304, Vector2.Zero), new int[2]
      {
        2000,
        int.MaxValue
      });
      fishShopStock.Add((ISalable) new FishTankFurniture(2322, Vector2.Zero), new int[2]
      {
        500,
        int.MaxValue
      });
      if (Game1.player.mailReceived.Contains("WillyTropicalFish"))
        fishShopStock.Add((ISalable) new FishTankFurniture(2312, Vector2.Zero), new int[2]
        {
          5000,
          int.MaxValue
        });
      fishShopStock.Add((ISalable) new BedFurniture(2502, Vector2.Zero), new int[2]
      {
        25000,
        int.MaxValue
      });
      GameLocation locationFromName = Game1.getLocationFromName("FishShop");
      if (locationFromName is ShopLocation)
      {
        foreach (Item key in (NetList<Item, NetRef<Item>>) (locationFromName as ShopLocation).itemsFromPlayerToSell)
        {
          if (key.Stack > 0)
          {
            int num = key.salePrice();
            if (key is Object)
              num = (key as Object).sellToStorePrice();
            fishShopStock.Add((ISalable) key, new int[2]
            {
              num,
              key.Stack
            });
          }
        }
      }
      return fishShopStock;
    }

    public static string ParseGiftReveals(string str)
    {
      try
      {
        while (str.Contains("%revealtaste"))
        {
          int startIndex1 = str.IndexOf("%revealtaste");
          int startIndex2 = startIndex1 + "%revealtaste".Length;
          int index = startIndex1 + 1;
          if (index >= str.Length)
            index = str.Length - 1;
          while (index < str.Length && (str[index] < '0' || str[index] > '9'))
            ++index;
          string name = str.Substring(startIndex2, index - startIndex2);
          int startIndex3 = index;
          while (index < str.Length && str[index] >= '0' && str[index] <= '9')
            ++index;
          int parent_sheet_index = int.Parse(str.Substring(startIndex3, index - startIndex3));
          str = str.Remove(startIndex1, index - startIndex1);
          NPC characterFromName = Game1.getCharacterFromName(name);
          if (characterFromName != null)
            Game1.player.revealGiftTaste(characterFromName, parent_sheet_index);
        }
      }
      catch (Exception ex)
      {
      }
      return str;
    }

    public static void Shuffle<T>(Random rng, List<T> list)
    {
      int count = list.Count;
      while (count > 1)
      {
        int index = rng.Next(count--);
        T obj = list[count];
        list[count] = list[index];
        list[index] = obj;
      }
    }

    public static void Shuffle<T>(Random rng, T[] array)
    {
      int length = array.Length;
      while (length > 1)
      {
        int index = rng.Next(length--);
        T obj = array[length];
        array[length] = array[index];
        array[index] = obj;
      }
    }

    public static int getSeasonNumber(string whichSeason)
    {
      if (whichSeason.Equals("spring", StringComparison.OrdinalIgnoreCase))
        return 0;
      if (whichSeason.Equals("summer", StringComparison.OrdinalIgnoreCase))
        return 1;
      if (whichSeason.Equals("autumn", StringComparison.OrdinalIgnoreCase) || whichSeason.Equals("fall", StringComparison.OrdinalIgnoreCase))
        return 2;
      return whichSeason.Equals("winter", StringComparison.OrdinalIgnoreCase) ? 3 : -1;
    }

    public static char getRandomSlotCharacter(char current)
    {
      char randomSlotCharacter = 'o';
      while (randomSlotCharacter == 'o' || (int) randomSlotCharacter == (int) current)
      {
        switch (Game1.random.Next(8))
        {
          case 0:
            randomSlotCharacter = '=';
            continue;
          case 1:
            randomSlotCharacter = '\\';
            continue;
          case 2:
            randomSlotCharacter = ']';
            continue;
          case 3:
            randomSlotCharacter = '[';
            continue;
          case 4:
            randomSlotCharacter = '<';
            continue;
          case 5:
            randomSlotCharacter = '*';
            continue;
          case 6:
            randomSlotCharacter = '$';
            continue;
          case 7:
            randomSlotCharacter = '}';
            continue;
          default:
            continue;
        }
      }
      return randomSlotCharacter;
    }

    /// <summary>
    /// uses Game1.random so this will not be the same each time it's called in the same context.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public static List<Vector2> getPositionsInClusterAroundThisTile(
      Vector2 startTile,
      int number)
    {
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      List<Vector2> clusterAroundThisTile = new List<Vector2>();
      Vector2 vector2_1 = startTile;
      vector2Queue.Enqueue(vector2_1);
      while (clusterAroundThisTile.Count < number)
      {
        Vector2 vector2_2 = vector2Queue.Dequeue();
        clusterAroundThisTile.Add(vector2_2);
        if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X + 1f, vector2_2.Y)))
          vector2Queue.Enqueue(new Vector2(vector2_2.X + 1f, vector2_2.Y));
        if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X - 1f, vector2_2.Y)))
          vector2Queue.Enqueue(new Vector2(vector2_2.X - 1f, vector2_2.Y));
        if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X, vector2_2.Y + 1f)))
          vector2Queue.Enqueue(new Vector2(vector2_2.X, vector2_2.Y + 1f));
        if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X, vector2_2.Y - 1f)))
          vector2Queue.Enqueue(new Vector2(vector2_2.X, vector2_2.Y - 1f));
      }
      return clusterAroundThisTile;
    }

    public static bool doesPointHaveLineOfSightInMine(
      GameLocation mine,
      Vector2 start,
      Vector2 end,
      int visionDistance)
    {
      if ((double) Vector2.Distance(start, end) > (double) visionDistance)
        return false;
      foreach (Point p in Utility.GetPointsOnLine((int) start.X, (int) start.Y, (int) end.X, (int) end.Y))
      {
        if (mine.getTileIndexAt(p, "Buildings") != -1)
          return false;
      }
      return true;
    }

    public static void addSprinklesToLocation(
      GameLocation l,
      int sourceXTile,
      int sourceYTile,
      int tilesWide,
      int tilesHigh,
      int totalSprinkleDuration,
      int millisecondsBetweenSprinkles,
      Microsoft.Xna.Framework.Color sprinkleColor,
      string sound = null,
      bool motionTowardCenter = false)
    {
      Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(sourceXTile - tilesWide / 2, sourceYTile - tilesHigh / 2, tilesWide, tilesHigh);
      Random random = new Random();
      int num = totalSprinkleDuration / millisecondsBetweenSprinkles;
      for (int index = 0; index < num; ++index)
      {
        Vector2 vector2 = Utility.getRandomPositionInThisRectangle(r, random) * 64f;
        l.temporarySprites.Add(new TemporaryAnimatedSprite(random.Next(10, 12), vector2, sprinkleColor, animationInterval: 50f)
        {
          layerDepth = 1f,
          delayBeforeAnimationStart = millisecondsBetweenSprinkles * index,
          interval = 100f,
          startSound = sound,
          motion = motionTowardCenter ? Utility.getVelocityTowardPoint(vector2, new Vector2((float) sourceXTile, (float) sourceYTile) * 64f, Vector2.Distance(new Vector2((float) sourceXTile, (float) sourceYTile) * 64f, vector2) / 64f) : Vector2.Zero,
          xStopCoordinate = sourceXTile,
          yStopCoordinate = sourceYTile
        });
      }
    }

    public static List<TemporaryAnimatedSprite> getStarsAndSpirals(
      GameLocation l,
      int sourceXTile,
      int sourceYTile,
      int tilesWide,
      int tilesHigh,
      int totalSprinkleDuration,
      int millisecondsBetweenSprinkles,
      Microsoft.Xna.Framework.Color sprinkleColor,
      string sound = null,
      bool motionTowardCenter = false)
    {
      Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(sourceXTile - tilesWide / 2, sourceYTile - tilesHigh / 2, tilesWide, tilesHigh);
      Random random = new Random();
      int num = totalSprinkleDuration / millisecondsBetweenSprinkles;
      List<TemporaryAnimatedSprite> starsAndSpirals = new List<TemporaryAnimatedSprite>();
      for (int index = 0; index < num; ++index)
      {
        Vector2 position = Utility.getRandomPositionInThisRectangle(r, random) * 64f;
        starsAndSpirals.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", random.NextDouble() < 0.5 ? new Microsoft.Xna.Framework.Rectangle(359, 1437, 14, 14) : new Microsoft.Xna.Framework.Rectangle(377, 1438, 9, 9), position, false, 0.01f, sprinkleColor)
        {
          xPeriodic = true,
          xPeriodicLoopTime = (float) random.Next(2000, 3000),
          xPeriodicRange = (float) random.Next(-64, 64),
          motion = new Vector2(0.0f, -2f),
          rotationChange = 3.141593f / (float) random.Next(4, 64),
          delayBeforeAnimationStart = millisecondsBetweenSprinkles * index,
          layerDepth = 1f,
          scaleChange = 0.04f,
          scaleChangeChange = -0.0008f,
          scale = 4f
        });
      }
      return starsAndSpirals;
    }

    public static void addStarsAndSpirals(
      GameLocation l,
      int sourceXTile,
      int sourceYTile,
      int tilesWide,
      int tilesHigh,
      int totalSprinkleDuration,
      int millisecondsBetweenSprinkles,
      Microsoft.Xna.Framework.Color sprinkleColor,
      string sound = null,
      bool motionTowardCenter = false)
    {
      l.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.getStarsAndSpirals(l, sourceXTile, sourceYTile, tilesWide, tilesHigh, totalSprinkleDuration, millisecondsBetweenSprinkles, sprinkleColor, sound, motionTowardCenter));
    }

    public static Vector2 snapDrawPosition(Vector2 draw_position) => new Vector2((float) (int) draw_position.X, (float) (int) draw_position.Y);

    public static Vector2 clampToTile(Vector2 nonTileLocation)
    {
      nonTileLocation.X -= nonTileLocation.X % 64f;
      nonTileLocation.Y -= nonTileLocation.Y % 64f;
      return nonTileLocation;
    }

    public static float distance(float x1, float x2, float y1, float y2) => (float) Math.Sqrt(((double) x2 - (double) x1) * ((double) x2 - (double) x1) + ((double) y2 - (double) y1) * ((double) y2 - (double) y1));

    public static void facePlayerEndBehavior(Character c, GameLocation location)
    {
      Character character = c;
      Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
      double x = (double) boundingBox.Center.X;
      boundingBox = Game1.player.GetBoundingBox();
      double y = (double) boundingBox.Center.Y;
      Vector2 target = new Vector2((float) x, (float) y);
      character.faceGeneralDirection(target, 0, false, false);
    }

    public static bool couldSeePlayerInPeripheralVision(Farmer player, Character c)
    {
      switch (c.FacingDirection)
      {
        case 0:
          if (player.GetBoundingBox().Center.Y < c.GetBoundingBox().Center.Y + 32)
            return true;
          break;
        case 1:
          if (player.GetBoundingBox().Center.X > c.GetBoundingBox().Center.X - 32)
            return true;
          break;
        case 2:
          if (player.GetBoundingBox().Center.Y > c.GetBoundingBox().Center.Y - 32)
            return true;
          break;
        case 3:
          if (player.GetBoundingBox().Center.X < c.GetBoundingBox().Center.X + 32)
            return true;
          break;
      }
      return false;
    }

    public static List<Microsoft.Xna.Framework.Rectangle> divideThisRectangleIntoQuarters(
      Microsoft.Xna.Framework.Rectangle rect)
    {
      return new List<Microsoft.Xna.Framework.Rectangle>()
      {
        new Microsoft.Xna.Framework.Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height / 2),
        new Microsoft.Xna.Framework.Rectangle(rect.X + rect.Width / 2, rect.Y, rect.Width / 2, rect.Height / 2),
        new Microsoft.Xna.Framework.Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2),
        new Microsoft.Xna.Framework.Rectangle(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2)
      };
    }

    public static Item getUncommonItemForThisMineLevel(int level, Point location)
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
      List<int> source = new List<int>();
      int num1 = -1;
      int num2 = -1;
      int num3 = 12;
      Random random = new Random(location.X * 1000 + location.Y + (int) Game1.uniqueIDForThisGame + level);
      foreach (KeyValuePair<int, string> keyValuePair in dictionary)
      {
        if (Game1.CurrentMineLevel >= Convert.ToInt32(keyValuePair.Value.Split('/')[10]) && Convert.ToInt32(keyValuePair.Value.Split('/')[9]) != -1)
        {
          int int32 = Convert.ToInt32(keyValuePair.Value.Split('/')[9]);
          if (num1 == -1 || num2 > Math.Abs(Game1.CurrentMineLevel - int32))
          {
            num1 = keyValuePair.Key;
            num2 = Convert.ToInt32(keyValuePair.Value.Split('/')[9]);
          }
          double num4 = Math.Pow(Math.E, -Math.Pow((double) (Game1.CurrentMineLevel - int32), 2.0) / (double) (2 * (num3 * num3)));
          if (random.NextDouble() < num4)
            source.Add(keyValuePair.Key);
        }
      }
      source.Add(num1);
      return (Item) new MeleeWeapon(source.ElementAt<int>(random.Next(source.Count)));
    }

    public static IEnumerable<Point> GetPointsOnLine(
      int x0,
      int y0,
      int x1,
      int y1)
    {
      return Utility.GetPointsOnLine(x0, y0, x1, y1, false);
    }

    public static List<Vector2> getBorderOfThisRectangle(Microsoft.Xna.Framework.Rectangle r)
    {
      List<Vector2> borderOfThisRectangle = new List<Vector2>();
      for (int x = r.X; x < r.Right; ++x)
        borderOfThisRectangle.Add(new Vector2((float) x, (float) r.Y));
      for (int y = r.Y + 1; y < r.Bottom; ++y)
        borderOfThisRectangle.Add(new Vector2((float) (r.Right - 1), (float) y));
      for (int x = r.Right - 2; x >= r.X; --x)
        borderOfThisRectangle.Add(new Vector2((float) x, (float) (r.Bottom - 1)));
      for (int y = r.Bottom - 2; y >= r.Y + 1; --y)
        borderOfThisRectangle.Add(new Vector2((float) r.X, (float) y));
      return borderOfThisRectangle;
    }

    public static Point getTranslatedPoint(Point p, int direction, int movementAmount)
    {
      switch (direction)
      {
        case 0:
          return new Point(p.X, p.Y - movementAmount);
        case 1:
          return new Point(p.X + movementAmount, p.Y);
        case 2:
          return new Point(p.X, p.Y + movementAmount);
        case 3:
          return new Point(p.X - movementAmount, p.Y);
        default:
          return p;
      }
    }

    public static Vector2 getTranslatedVector2(
      Vector2 p,
      int direction,
      float movementAmount)
    {
      switch (direction)
      {
        case 0:
          return new Vector2(p.X, p.Y - movementAmount);
        case 1:
          return new Vector2(p.X + movementAmount, p.Y);
        case 2:
          return new Vector2(p.X, p.Y + movementAmount);
        case 3:
          return new Vector2(p.X - movementAmount, p.Y);
        default:
          return p;
      }
    }

    public static IEnumerable<Point> GetPointsOnLine(
      int x0,
      int y0,
      int x1,
      int y1,
      bool ignoreSwap)
    {
      bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
      if (steep)
      {
        int num1 = x0;
        x0 = y0;
        y0 = num1;
        int num2 = x1;
        x1 = y1;
        y1 = num2;
      }
      if (!ignoreSwap && x0 > x1)
      {
        int num3 = x0;
        x0 = x1;
        x1 = num3;
        int num4 = y0;
        y0 = y1;
        y1 = num4;
      }
      int dx = x1 - x0;
      int dy = Math.Abs(y1 - y0);
      int error = dx / 2;
      int ystep = y0 < y1 ? 1 : -1;
      int y = y0;
      for (int x = x0; x <= x1; ++x)
      {
        yield return new Point(steep ? y : x, steep ? x : y);
        error -= dy;
        if (error < 0)
        {
          y += ystep;
          error += dx;
        }
      }
    }

    public static Vector2 getRandomAdjacentOpenTile(Vector2 tile, GameLocation location)
    {
      List<Vector2> adjacentTileLocations = Utility.getAdjacentTileLocations(tile);
      int num = 0;
      int index = Game1.random.Next(adjacentTileLocations.Count);
      Vector2 tileLocation;
      for (tileLocation = adjacentTileLocations[index]; num < 4 && (location.isTileOccupiedForPlacement(tileLocation) || !location.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y), Game1.viewport)); ++num)
      {
        index = (index + 1) % adjacentTileLocations.Count;
        tileLocation = adjacentTileLocations[index];
      }
      return num >= 4 ? Vector2.Zero : tileLocation;
    }

    public static int getObjectIndexFromSlotCharacter(char character)
    {
      switch (character)
      {
        case '$':
          return 398;
        case '*':
          return 176;
        case '<':
          return 400;
        case '=':
          return 72;
        case '[':
          return 276;
        case '\\':
          return 336;
        case ']':
          return 221;
        case '}':
          return 184;
        default:
          return 0;
      }
    }

    private static string farmerAccomplishments()
    {
      string str = Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5229") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5230");
      if (Game1.player.hasRustyKey)
        str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5235");
      if (Game1.player.achievements.Contains(71))
        str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5236");
      if (Game1.player.achievements.Contains(45))
        str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5237");
      if (str.Length > 115)
        str += "#$b#";
      if (Game1.player.achievements.Contains(63))
        str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5239");
      if (Game1.player.timesReachedMineBottom > 0)
        str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5240");
      return str + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5241", (object) (uint) ((int) Game1.player.totalMoneyEarned - (int) (Game1.player.totalMoneyEarned % 1000U)));
    }

    public static string getCreditsString() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5243") + Environment.NewLine + " " + Environment.NewLine + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5244") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5245") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5246") + Environment.NewLine + Environment.NewLine + "-Eric Barone" + Environment.NewLine + " " + Environment.NewLine + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5248") + Environment.NewLine + Environment.NewLine + "-Amber Hageman" + Environment.NewLine + "-Shane Waletzko" + Environment.NewLine + "-Fiddy, Nuns, Kappy &" + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5252") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5253") + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5254");

    public static string getStardewHeroCelebrationEventString(int finalFarmerScore)
    {
      string celebrationEventString;
      if (finalFarmerScore >= Game1.percentageToWinStardewHero)
      {
        string[] strArray = new string[54];
        strArray[0] = "title_day/-100 -100/farmer 18 20 1 rival 27 20 2";
        strArray[1] = Utility.getCelebrationPositionsForDatables(Game1.player.spouse);
        strArray[2] = Game1.player.spouse == null || Game1.player.isEngaged() ? "" : Game1.player.spouse + " 17 21 1 ";
        strArray[3] = "Lewis 22 19 2 Marnie 21 22 0 Caroline 24 22 0 Pierre 25 22 0 Gus 26 22 0 Clint 26 23 0 Emily 25 23 0 Shane 27 23 0 ";
        strArray[4] = !Game1.player.friendshipData.ContainsKey("Sandy") || Game1.player.friendshipData["Sandy"].Points <= 0 ? "" : "Sandy 24 23 0 ";
        strArray[5] = "George 21 23 0 Evelyn 20 23 0 Pam 19 23 0 Jodi 27 24 0 ";
        strArray[6] = Game1.getCharacterFromName("Kent") != null ? "Kent 26 24 0 " : "";
        strArray[7] = "Linus 24 24 0 Robin 21 24 0 Demetrius 20 24 0";
        strArray[8] = Game1.player.timesReachedMineBottom > 0 ? " Dwarf 19 24 0" : "";
        strArray[9] = "/addObject 18 19 ";
        strArray[10] = Game1.random.Next(313, 320).ToString();
        strArray[11] = "/addObject 19 19 ";
        strArray[12] = Game1.random.Next(313, 320).ToString();
        strArray[13] = "/addObject 20 19 ";
        strArray[14] = Game1.random.Next(313, 320).ToString();
        strArray[15] = "/addObject 25 19 ";
        strArray[16] = Game1.random.Next(313, 320).ToString();
        strArray[17] = "/addObject 26 19 ";
        int num = Game1.random.Next(313, 320);
        strArray[18] = num.ToString();
        strArray[19] = "/addObject 27 19 ";
        num = Game1.random.Next(313, 320);
        strArray[20] = num.ToString();
        strArray[21] = "/addObject 23 19 468/viewport 22 20 true/pause 4000/speak Lewis \"";
        strArray[22] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5256");
        strArray[23] = "\"/pause 400/faceDirection Lewis 3/pause 500/faceDirection Lewis 1/pause 600/faceDirection Lewis 2/speak Lewis \"";
        strArray[24] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5259");
        strArray[25] = "\"/pause 200/showRivalFrame 16/pause 600/speak Lewis \"";
        strArray[26] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5260");
        strArray[27] = "\"/pause 700/move Lewis 0 1 3/stopMusic/move Lewis -2 0 3/playMusic musicboxsong/faceDirection farmer 1/showRivalFrame 12/speak Lewis \"";
        strArray[28] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5263", (object) Utility.farmerAccomplishments());
        strArray[29] = "\"/pause 800/move Lewis 5 0 1/showRivalFrame 12/playMusic rival/pause 500/speak Lewis \"";
        strArray[30] = (bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5306") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5307");
        strArray[31] = "\"/pause 500/speak rival \"";
        strArray[32] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5308");
        strArray[33] = "\"/move rival 0 1 2/showRivalFrame 17/pause 500/speak rival \"";
        strArray[34] = (bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? (Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5310") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5311")) : (Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5312") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5313"));
        strArray[35] = "\"/pause 600/emote farmer 40/showRivalFrame 16/pause 900/move rival 0 -1 2/showRivalFrame 16/move Lewis -3 0 2/stopMusic/pause 500/speak Lewis \"";
        strArray[36] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5314");
        strArray[37] = "\"/stopMusic/move Lewis 0 -1 2/pause 600/faceDirection Lewis 1/pause 600/faceDirection Lewis 3/pause 600/faceDirection Lewis 2/speak Lewis \"";
        strArray[38] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5315");
        strArray[39] = "\"/pause 300/move rival -2 0 2/showRivalFrame 16/pause 1500/speak Lewis \"";
        strArray[40] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5316");
        strArray[41] = "\"/pause 500/showRivalFrame 18/pause 400/playMusic happy/emote farmer 16/move farmer 5 0 2/move Lewis 0 1 1/speak Lewis \"";
        strArray[42] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5317", (object) finalFarmerScore);
        strArray[43] = "\"/speak Emily \"";
        strArray[44] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5318");
        strArray[45] = "\"/speak Gus \"";
        strArray[46] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5319");
        strArray[47] = "\"/speak Pierre \"";
        strArray[48] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5320");
        strArray[49] = "\"/showRivalFrame 12/pause 500/speak rival \"";
        strArray[50] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5321");
        strArray[51] = "\"/speed rival 4/move rival 6 0 0/faceDirection farmer 1 true/speed rival 4/move rival 0 -10 1/warp rival -100 -100/move farmer 0 1 2/emote farmer 20/fade/viewport -1000 -1000/message \"";
        strArray[52] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5322", (object) Utility.getOtherFarmerNames()[0]);
        strArray[53] = "\"/end credits";
        celebrationEventString = string.Concat(strArray);
      }
      else
      {
        string[] strArray = new string[54];
        strArray[0] = "title_day/-100 -100/farmer 18 20 1 rival 27 20 2";
        strArray[1] = Utility.getCelebrationPositionsForDatables(Game1.player.spouse);
        strArray[2] = Game1.player.spouse == null || Game1.player.isEngaged() ? "" : Game1.player.spouse + " 17 21 1 ";
        strArray[3] = "Lewis 22 19 2 Marnie 21 22 0 Caroline 24 22 0 Pierre 25 22 0 Gus 26 22 0 Clint 26 23 0 Emily 25 23 0 Shane 27 23 0 ";
        strArray[4] = !Game1.player.friendshipData.ContainsKey("Sandy") || Game1.player.friendshipData["Sandy"].Points <= 0 ? "" : "Sandy 24 23 0 ";
        strArray[5] = "George 21 23 0 Evelyn 20 23 0 Pam 19 23 0 Jodi 27 24 0 ";
        strArray[6] = Game1.getCharacterFromName("Kent") != null ? "Kent 26 24 0 " : "";
        strArray[7] = "Linus 24 24 0 Robin 21 24 0 Demetrius 20 24 0";
        strArray[8] = Game1.player.timesReachedMineBottom > 0 ? " Dwarf 19 24 0" : "";
        strArray[9] = "/addObject 18 19 ";
        strArray[10] = Game1.random.Next(313, 320).ToString();
        strArray[11] = "/addObject 19 19 ";
        strArray[12] = Game1.random.Next(313, 320).ToString();
        strArray[13] = "/addObject 20 19 ";
        strArray[14] = Game1.random.Next(313, 320).ToString();
        strArray[15] = "/addObject 25 19 ";
        strArray[16] = Game1.random.Next(313, 320).ToString();
        strArray[17] = "/addObject 26 19 ";
        int num = Game1.random.Next(313, 320);
        strArray[18] = num.ToString();
        strArray[19] = "/addObject 27 19 ";
        num = Game1.random.Next(313, 320);
        strArray[20] = num.ToString();
        strArray[21] = "/addObject 23 19 468/viewport 22 20 true/pause 4000/speak Lewis \"";
        strArray[22] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5256");
        strArray[23] = "\"/pause 400/faceDirection Lewis 3/pause 500/faceDirection Lewis 1/pause 600/faceDirection Lewis 2/speak Lewis \"";
        strArray[24] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5259");
        strArray[25] = "\"/pause 200/showRivalFrame 16/pause 600/speak Lewis \"";
        strArray[26] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5260");
        strArray[27] = "\"/pause 700/move Lewis 0 1 3/stopMusic/move Lewis -2 0 3/playMusic musicboxsong/faceDirection farmer 1/showRivalFrame 12/speak Lewis \"";
        strArray[28] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5263", (object) Utility.farmerAccomplishments());
        strArray[29] = "\"/pause 800/move Lewis 5 0 1/showRivalFrame 12/playMusic rival/pause 500/speak Lewis \"";
        strArray[30] = (bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5306") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5307");
        strArray[31] = "\"/pause 500/speak rival \"";
        strArray[32] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5308");
        strArray[33] = "\"/move rival 0 1 2/showRivalFrame 17/pause 500/speak rival \"";
        strArray[34] = (bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? (Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5310") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5311")) : (Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5312") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5313"));
        strArray[35] = "\"/pause 600/emote farmer 40/showRivalFrame 16/pause 900/move rival 0 -1 2/showRivalFrame 16/move Lewis -3 0 2/stopMusic/pause 500/speak Lewis \"";
        strArray[36] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5314");
        strArray[37] = "\"/stopMusic/move Lewis 0 -1 2/pause 600/faceDirection Lewis 1/pause 600/faceDirection Lewis 3/pause 600/faceDirection Lewis 2/speak Lewis \"";
        strArray[38] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5315");
        strArray[39] = "\"/pause 300/move rival -2 0 2/showRivalFrame 16/pause 1500/speak Lewis \"";
        strArray[40] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5323");
        strArray[41] = "\"/pause 200/showFrame 32/move rival -2 0 2/showRivalFrame 19/pause 400/playSound death/emote farmer 28/speak Lewis \"";
        strArray[42] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5324", (object) (Game1.percentageToWinStardewHero - finalFarmerScore));
        strArray[43] = "\"/speak rival \"";
        strArray[44] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5325");
        strArray[45] = "\"/pause 600/faceDirection Lewis 3/speak Lewis \"";
        strArray[46] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5326");
        strArray[47] = "\"/speak Emily \"";
        strArray[48] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5327");
        strArray[49] = "\"/fade/viewport -1000 -1000/message \"";
        strArray[50] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5328", (object) finalFarmerScore);
        strArray[51] = Environment.NewLine;
        strArray[52] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5329");
        strArray[53] = "\"/end credits";
        celebrationEventString = string.Concat(strArray);
      }
      return celebrationEventString;
    }

    public static void CollectSingleItemOrShowChestMenu(Chest chest, object context = null)
    {
      int num = 0;
      Item obj = (Item) null;
      for (int index = 0; index < chest.items.Count; ++index)
      {
        if (chest.items[index] != null)
        {
          ++num;
          if (num == 1)
            obj = chest.items[index];
          if (num == 2)
          {
            obj = (Item) null;
            break;
          }
        }
      }
      if (num == 0)
        return;
      if (obj != null)
      {
        int stack = obj.Stack;
        if (Game1.player.addItemToInventory(obj) == null)
        {
          Game1.playSound("coin");
          chest.items.Remove(obj);
          chest.clearNulls();
          return;
        }
        if (obj.Stack != stack)
          Game1.playSound("coin");
      }
      Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, context: context);
    }

    public static bool CollectOrDrop(Item item, int direction)
    {
      if (item == null)
        return true;
      item = Game1.player.addItemToInventory(item);
      if (item == null)
        return true;
      if (direction != -1)
        Game1.createItemDebris(item, Game1.player.getStandingPosition(), direction);
      else
        Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
      return false;
    }

    public static bool CollectOrDrop(Item item) => Utility.CollectOrDrop(item, -1);

    public static void perpareDayForStardewCelebration(int finalFarmerScore)
    {
      bool flag = finalFarmerScore >= Game1.percentageToWinStardewHero;
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        foreach (NPC character in location.characters)
        {
          string masterDialogue = "";
          if (flag)
          {
            switch (Game1.random.Next(6))
            {
              case 0:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5348");
                break;
              case 1:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5349");
                break;
              case 2:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5350");
                break;
              case 3:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5351");
                break;
              case 4:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5352");
                break;
              case 5:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5353");
                break;
            }
            if (character.Name.Equals("Sebastian") || character.Name.Equals("Abigail"))
              masterDialogue = Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5356") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5357");
            else if (character.Name.Equals("George"))
              masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5360");
          }
          else
          {
            switch (Game1.random.Next(4))
            {
              case 0:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5361");
                break;
              case 1:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5362");
                break;
              case 2:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5363");
                break;
              case 3:
                masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5364");
                break;
            }
            if (character.Name.Equals("George"))
              masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5360");
          }
          character.CurrentDialogue.Push(new Dialogue(masterDialogue, character));
        }
      }
      if (!flag)
        return;
      Game1.player.stardewHero = true;
    }

    public static List<string> getExes(Farmer farmer)
    {
      List<string> exes = new List<string>();
      foreach (string key in farmer.friendshipData.Keys)
      {
        if (farmer.friendshipData[key].IsDivorced())
          exes.Add(key);
      }
      return exes;
    }

    public static string getCelebrationPositionsForDatables(List<string> people_to_exclude)
    {
      string positionsForDatables = " ";
      if (!people_to_exclude.Contains("Sam"))
        positionsForDatables += "Sam 25 65 0 ";
      if (!people_to_exclude.Contains("Sebastian"))
        positionsForDatables += "Sebastian 24 65 0 ";
      if (!people_to_exclude.Contains("Alex"))
        positionsForDatables += "Alex 25 69 0 ";
      if (!people_to_exclude.Contains("Harvey"))
        positionsForDatables += "Harvey 23 67 0 ";
      if (!people_to_exclude.Contains("Elliott"))
        positionsForDatables += "Elliott 32 65 0 ";
      if (!people_to_exclude.Contains("Haley"))
        positionsForDatables += "Haley 26 69 0 ";
      if (!people_to_exclude.Contains("Penny"))
        positionsForDatables += "Penny 23 66 0 ";
      if (!people_to_exclude.Contains("Maru"))
        positionsForDatables += "Maru 24 68 0 ";
      if (!people_to_exclude.Contains("Leah"))
        positionsForDatables += "Leah 33 65 0 ";
      if (!people_to_exclude.Contains("Abigail"))
        positionsForDatables += "Abigail 23 65 0 ";
      return positionsForDatables;
    }

    public static string getCelebrationPositionsForDatables(string personToExclude)
    {
      List<string> people_to_exclude = new List<string>();
      if (personToExclude != null)
        people_to_exclude.Add(personToExclude);
      return Utility.getCelebrationPositionsForDatables(people_to_exclude);
    }

    public static void fixAllAnimals()
    {
      if (!Game1.IsMasterGame)
        return;
      Farm farm = Game1.getFarm();
      foreach (Building building in farm.buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value is AnimalHouse)
        {
          foreach (long id in (NetList<long, NetLong>) (building.indoors.Value as AnimalHouse).animalsThatLiveHere)
          {
            FarmAnimal animal = Utility.getAnimal(id);
            if (animal != null)
            {
              animal.home = building;
              animal.homeLocation.Value = new Vector2((float) (int) (NetFieldBase<int, NetInt>) building.tileX, (float) (int) (NetFieldBase<int, NetInt>) building.tileY);
            }
          }
        }
      }
      List<FarmAnimal> farmAnimalList1 = new List<FarmAnimal>();
      foreach (FarmAnimal allFarmAnimal in farm.getAllFarmAnimals())
      {
        if (allFarmAnimal.home == null)
          farmAnimalList1.Add(allFarmAnimal);
      }
      foreach (FarmAnimal farmAnimal in farmAnimalList1)
      {
        NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection pairs;
        KeyValuePair<long, FarmAnimal> keyValuePair;
        foreach (Building building in farm.buildings)
        {
          if (building.indoors.Value != null && building.indoors.Value is AnimalHouse)
          {
            for (int index = (building.indoors.Value as AnimalHouse).animals.Count() - 1; index >= 0; --index)
            {
              pairs = (building.indoors.Value as AnimalHouse).animals.Pairs;
              keyValuePair = pairs.ElementAt(index);
              if (keyValuePair.Value.Equals((object) farmAnimal))
              {
                NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> animals = (building.indoors.Value as AnimalHouse).animals;
                pairs = (building.indoors.Value as AnimalHouse).animals.Pairs;
                keyValuePair = pairs.ElementAt(index);
                long key = keyValuePair.Key;
                animals.Remove(key);
              }
            }
          }
        }
        for (int index = farm.animals.Count() - 1; index >= 0; --index)
        {
          pairs = farm.animals.Pairs;
          keyValuePair = pairs.ElementAt(index);
          if (keyValuePair.Value.Equals((object) farmAnimal))
          {
            NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> animals = farm.animals;
            pairs = farm.animals.Pairs;
            keyValuePair = pairs.ElementAt(index);
            long key = keyValuePair.Key;
            animals.Remove(key);
          }
        }
      }
      foreach (Building building in farm.buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value is AnimalHouse)
        {
          for (int index = (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count - 1; index >= 0; --index)
          {
            if (Utility.getAnimal((building.indoors.Value as AnimalHouse).animalsThatLiveHere[index]).home != building)
              (building.indoors.Value as AnimalHouse).animalsThatLiveHere.RemoveAt(index);
          }
        }
      }
      foreach (FarmAnimal farmAnimal in farmAnimalList1)
      {
        foreach (Building building in farm.buildings)
        {
          if (building.buildingType.Contains((string) (NetFieldBase<string, NetString>) farmAnimal.buildingTypeILiveIn) && building.indoors.Value != null && building.indoors.Value is AnimalHouse && !(building.indoors.Value as AnimalHouse).isFull())
          {
            farmAnimal.home = building;
            farmAnimal.homeLocation.Value = new Vector2((float) (int) (NetFieldBase<int, NetInt>) building.tileX, (float) (int) (NetFieldBase<int, NetInt>) building.tileY);
            farmAnimal.setRandomPosition((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) farmAnimal.home.indoors);
            (farmAnimal.home.indoors.Value as AnimalHouse).animals.Add((long) farmAnimal.myID, farmAnimal);
            (farmAnimal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Add((long) farmAnimal.myID);
            break;
          }
        }
      }
      List<FarmAnimal> farmAnimalList2 = new List<FarmAnimal>();
      foreach (FarmAnimal farmAnimal in farmAnimalList1)
      {
        if (farmAnimal.home == null)
          farmAnimalList2.Add(farmAnimal);
      }
      foreach (FarmAnimal c in farmAnimalList2)
      {
        c.Position = Utility.recursiveFindOpenTileForCharacter((Character) c, (GameLocation) farm, new Vector2(40f, 40f), 200) * 64f;
        if (!farm.animals.ContainsKey((long) c.myID))
          farm.animals.Add((long) c.myID, c);
      }
    }

    public static Event getWeddingEvent(Farmer farmer)
    {
      List<string> exes = Utility.getExes(farmer);
      exes.Add(farmer.spouse);
      return new Event("sweet/-1000 -100/farmer 27 63 2 spouse 28 63 2" + Utility.getCelebrationPositionsForDatables(exes) + "Lewis 27 64 2 Marnie 26 65 0 Caroline 29 65 0 Pierre 30 65 0 Gus 31 65 0 Clint 31 66 0 " + (farmer.spouse.Contains("Emily") || exes.Contains("Emily") ? "" : "Emily 30 66 0 ") + (farmer.spouse.Contains("Shane") || exes.Contains("Shane") ? "" : "Shane 32 66 0 ") + (!farmer.friendshipData.ContainsKey("Sandy") || farmer.friendshipData["Sandy"].Points <= 0 ? "" : "Sandy 29 66 0 ") + "George 26 66 0 Evelyn 25 66 0 Pam 24 66 0 Jodi 32 67 0 " + (Game1.getCharacterFromName("Kent") != null ? "Kent 31 67 0 " : "") + "otherFarmers 29 69 0 Linus 29 67 0 Robin 25 67 0 Demetrius 26 67 0 Vincent 26 68 3 Jas 25 68 1" + (!farmer.friendshipData.ContainsKey("Dwarf") || farmer.friendshipData["Dwarf"].Points <= 0 ? "" : " Dwarf 30 67 0") + "/changeLocation Town/showFrame spouse 36/specificTemporarySprite wedding/viewport 27 64 true/pause 4000/speak Lewis \"" + (farmer.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5367", (object) Game1.dayOfMonth, (object) Game1.CurrentSeasonDisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5369", (object) Game1.dayOfMonth, (object) Game1.CurrentSeasonDisplayName)) + "\"/faceDirection farmer 1/showFrame spouse 37/pause 500/faceDirection Lewis 0/pause 2000/speak Lewis \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5370") + "\"/move Lewis 0 1 0/playMusic none/pause 1000/showFrame Lewis 20/speak Lewis \"" + (farmer.IsMale ? (Utility.isMale(farmer.spouse) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5371") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5373")) : (Utility.isMale(farmer.spouse) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5377") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5375"))) + "\"/pause 500/speak Lewis \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5379") + "\"/pause 1000/showFrame 101/showFrame spouse 38/specificTemporarySprite heart 28 62/playSound dwop/pause 2000/specificTemporarySprite wed/warp Marnie -2000 -2000/faceDirection farmer 2/showFrame spouse 36/faceDirection Pam 1 true/faceDirection Evelyn 3 true/faceDirection Pierre 3 true/faceDirection Caroline 1 true/animate Robin false true 500 20 21 20 22/animate Demetrius false true 500 24 25 24 26/move Lewis 0 3 3 true/move Caroline 0 -1 3 false/pause 4000/faceDirection farmer 1/showFrame spouse 37/globalFade/viewport -1000 -1000/pause 1000/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5381") + "\"/pause 500/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5383") + "\"/pause 4000/waitForOtherPlayers weddingEnd" + farmer.uniqueMultiplayerID?.ToString() + "/end wedding", -2, farmer);
    }

    public static Event getPlayerWeddingEvent(Farmer farmer, Farmer spouse)
    {
      List<string> exes = Utility.getExes(farmer);
      exes.AddRange((IEnumerable<string>) Utility.getExes(spouse));
      return new Event(("sweet/-1000 -100/farmer 27 63 2" + Utility.getCelebrationPositionsForDatables(exes) + "Lewis 27 64 2 Marnie 26 65 0 Caroline 29 65 0 Pierre 30 65 0 Gus 31 65 0 Clint 31 66 0 " + (exes.Contains("Emily") ? "" : "Emily 30 66 0 ") + (exes.Contains("Shane") ? "" : "Shane 32 66 0 ") + (!farmer.friendshipData.ContainsKey("Sandy") || farmer.friendshipData["Sandy"].Points <= 0 ? "" : "Sandy 29 66 0 ") + "George 26 66 0 Evelyn 25 66 0 Pam 24 66 0 Jodi 32 67 0 " + (Game1.getCharacterFromName("Kent") != null ? "Kent 31 67 0 " : "") + "otherFarmers 29 69 0 Linus 29 67 0 Robin 25 67 0 Demetrius 26 67 0 Vincent 26 68 3 Jas 25 68 1" + (!farmer.friendshipData.ContainsKey("Dwarf") || farmer.friendshipData["Dwarf"].Points <= 0 ? "" : " Dwarf 30 67 0") + "/changeLocation Town/faceDirection spouseFarmer 2/warp spouseFarmer 28 63/specificTemporarySprite wedding/viewport 27 64 true/pause 4000/speak Lewis \"" + (farmer.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5367", (object) Game1.dayOfMonth, (object) Game1.CurrentSeasonDisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5369", (object) Game1.dayOfMonth, (object) Game1.CurrentSeasonDisplayName)) + "\"/faceDirection farmer 1/faceDirection spouseFarmer 3/pause 500/faceDirection Lewis 0/pause 2000/speak Lewis \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5370") + "\"/move Lewis 0 1 0/playMusic none/pause 1000/showFrame Lewis 20/speak Lewis \"" + (farmer.IsMale ? (spouse.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5371") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5373")) : (spouse.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5377") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5375"))) + "\"/pause 500/speak Lewis \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5379") + "\"/pause 1000/showFrame 101/showFrame spouseFarmer 101/specificTemporarySprite heart 28 62/playSound dwop/pause 2000/specificTemporarySprite wed/warp Marnie -2000 -2000/faceDirection farmer 2/faceDirection spouseFarmer 2/faceDirection Pam 1 true/faceDirection Evelyn 3 true/faceDirection Pierre 3 true/faceDirection Caroline 1 true/animate Robin false true 500 20 21 20 22/animate Demetrius false true 500 24 25 24 26/move Lewis 0 3 3 true/move Caroline 0 -1 3 false/pause 4000/faceDirection farmer 1/showFrame spouseFarmer 3/globalFade/viewport -1000 -1000/pause 1000/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5381") + "\"/pause 500/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5383") + "\"/pause 4000/waitForOtherPlayers weddingEnd" + farmer.uniqueMultiplayerID?.ToString() + "/end wedding").Replace("spouseFarmer", nameof (farmer) + Utility.getFarmerNumberFromFarmer(spouse).ToString()), -2, farmer);
    }

    public static void drawTinyDigits(
      int toDraw,
      SpriteBatch b,
      Vector2 position,
      float scale,
      float layerDepth,
      Microsoft.Xna.Framework.Color c)
    {
      int x = 0;
      int num1 = toDraw;
      int num2 = 0;
      do
      {
        ++num2;
      }
      while ((toDraw /= 10) >= 1);
      int num3 = (int) Math.Pow(10.0, (double) (num2 - 1));
      bool flag = false;
      for (int index = 0; index < num2; ++index)
      {
        int num4 = num1 / num3 % 10;
        if (num4 > 0 || index == num2 - 1)
          flag = true;
        if (flag)
          b.Draw(Game1.mouseCursors, position + new Vector2((float) x, 0.0f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(368 + num4 * 5, 56, 5, 7)), c, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        x += (int) (5.0 * (double) scale) - 1;
        num3 /= 10;
      }
    }

    public static int getWidthOfTinyDigitString(int toDraw, float scale)
    {
      int num = 0;
      do
      {
        ++num;
      }
      while ((toDraw /= 10) >= 1);
      return (int) ((double) (num * 5) * (double) scale);
    }

    public static bool isMale(string who)
    {
      switch (who)
      {
        case "Abigail":
        case "Emily":
        case "Haley":
        case "Leah":
        case "Maru":
        case "Penny":
        case "Sandy":
          return false;
        default:
          return true;
      }
    }

    public static int GetMaximumHeartsForCharacter(Character character)
    {
      if (character == null)
        return 0;
      int heartsForCharacter = 10;
      if (character is NPC && (bool) (NetFieldBase<bool, NetBool>) ((NPC) character).datable)
        heartsForCharacter = 8;
      Friendship friendship = (Friendship) null;
      if (Game1.player.friendshipData.ContainsKey(character.Name))
        friendship = Game1.player.friendshipData[character.Name];
      if (friendship != null)
      {
        if (friendship.IsMarried())
          heartsForCharacter = 14;
        else if (friendship.IsDating())
          heartsForCharacter = 10;
      }
      return heartsForCharacter;
    }

    public static bool doesItemWithThisIndexExistAnywhere(int index, bool bigCraftable = false)
    {
      bool item_found = false;
      Utility.iterateAllItems((Action<Item>) (item =>
      {
        if (!(item is Object) || (bool) (NetFieldBase<bool, NetBool>) (item as Object).bigCraftable != bigCraftable || (int) (NetFieldBase<int, NetInt>) item.parentSheetIndex != index)
          return;
        item_found = true;
      }));
      return item_found;
    }

    public static int getSwordUpgradeLevel()
    {
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Game1.player.items)
      {
        if (obj != null && obj is Sword)
          return (int) (NetFieldBase<int, NetInt>) ((Tool) obj).upgradeLevel;
      }
      return 0;
    }

    public static bool tryToAddObjectToHome(Object o)
    {
      GameLocation locationFromName = Game1.getLocationFromName("FarmHouse");
      for (int x = locationFromName.map.GetLayer("Back").LayerWidth - 1; x >= 0; --x)
      {
        for (int y = locationFromName.map.GetLayer("Back").LayerHeight - 1; y >= 0; --y)
        {
          if (locationFromName.map.GetLayer("Back").Tiles[x, y] != null && locationFromName.dropObject(o, new Vector2((float) (x * 64), (float) (y * 64)), Game1.viewport, false))
          {
            if (o.ParentSheetIndex == 468)
            {
              Object @object = new Object(new Vector2((float) x, (float) y), 308, (string) null, true, true, false, false);
              @object.heldObject.Value = o;
              locationFromName.objects[new Vector2((float) x, (float) y)] = @object;
            }
            return true;
          }
        }
      }
      return false;
    }

    internal static void CollectGarbage(string filePath = "", int lineNumber = 0) => GC.Collect(0, GCCollectionMode.Forced);

    public static string InvokeSimpleReturnTypeMethod(
      object toBeCalled,
      string methodName,
      object[] parameters)
    {
      Type type = toBeCalled.GetType();
      try
      {
        return (string) type.InvokeMember(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, (Binder) null, toBeCalled, parameters) ?? "";
      }
      catch (Exception ex)
      {
        return Game1.parseText("Didn't work - " + ex.Message);
      }
    }

    public static List<int> possibleCropsAtThisTime(string season, bool firstWeek)
    {
      List<int> collection = (List<int>) null;
      List<int> intList = (List<int>) null;
      if (season.Equals("spring"))
      {
        collection = new List<int>() { 24, 192 };
        if (Game1.year > 1)
          collection.Add(250);
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
          collection.Add(248);
        intList = new List<int>() { 190, 188 };
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
          intList.Add(252);
        intList.AddRange((IEnumerable<int>) collection);
      }
      else if (season.Equals("summer"))
      {
        collection = new List<int>() { 264, 262, 260 };
        intList = new List<int>() { 254, 256 };
        if (Game1.year > 1)
          collection.Add(266);
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
          intList.AddRange((IEnumerable<int>) new int[2]
          {
            258,
            268
          });
        intList.AddRange((IEnumerable<int>) collection);
      }
      else if (season.Equals("fall"))
      {
        collection = new List<int>() { 272, 278 };
        intList = new List<int>() { 270, 276, 280 };
        if (Game1.year > 1)
          intList.Add(274);
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
        {
          collection.Add(284);
          intList.Add(282);
        }
        intList.AddRange((IEnumerable<int>) collection);
      }
      return firstWeek ? collection : intList;
    }

    public static int[] cropsOfTheWeek()
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) (Game1.stats.DaysPlayed / 29U));
      int[] numArray = new int[4];
      List<int> source1 = Utility.possibleCropsAtThisTime(Game1.currentSeason, true);
      List<int> source2 = Utility.possibleCropsAtThisTime(Game1.currentSeason, false);
      if (source1 != null)
      {
        numArray[0] = source1.ElementAt<int>(random.Next(source1.Count));
        for (int index = 1; index < 4; ++index)
        {
          numArray[index] = source2.ElementAt<int>(random.Next(source2.Count));
          while (numArray[index] == numArray[index - 1])
            numArray[index] = source2.ElementAt<int>(random.Next(source2.Count));
        }
      }
      return numArray;
    }

    public static float RandomFloat(float min, float max, Random random = null)
    {
      if (random == null)
        random = Game1.random;
      return Utility.Lerp(min, max, (float) random.NextDouble());
    }

    public static float Clamp(float value, float min, float max)
    {
      if ((double) max < (double) min)
      {
        double num = (double) min;
        min = max;
        max = (float) num;
      }
      if ((double) value < (double) min)
        value = min;
      if ((double) value > (double) max)
        value = max;
      return value;
    }

    public static Microsoft.Xna.Framework.Color MakeCompletelyOpaque(Microsoft.Xna.Framework.Color color)
    {
      if (color.A >= byte.MaxValue)
        return color;
      color.A = byte.MaxValue;
      return color;
    }

    public static int Clamp(int value, int min, int max)
    {
      if (max < min)
      {
        int num = min;
        min = max;
        max = num;
      }
      if (value < min)
        value = min;
      if (value > max)
        value = max;
      return value;
    }

    public static float Lerp(float a, float b, float t) => a + t * (b - a);

    public static float MoveTowards(float from, float to, float delta) => (double) Math.Abs(to - from) <= (double) delta ? to : from + (float) Math.Sign(to - from) * delta;

    public static Microsoft.Xna.Framework.Color MultiplyColor(Microsoft.Xna.Framework.Color a, Microsoft.Xna.Framework.Color b) => new Microsoft.Xna.Framework.Color((float) ((double) a.R / (double) byte.MaxValue * ((double) b.R / (double) byte.MaxValue)), (float) ((double) a.G / (double) byte.MaxValue * ((double) b.G / (double) byte.MaxValue)), (float) ((double) a.B / (double) byte.MaxValue * ((double) b.B / (double) byte.MaxValue)), (float) ((double) a.A / (double) byte.MaxValue * ((double) b.A / (double) byte.MaxValue)));

    public static int CalculateMinutesUntilMorning(int currentTime) => Utility.CalculateMinutesUntilMorning(currentTime, 1);

    public static int CalculateMinutesUntilMorning(int currentTime, int daysElapsed)
    {
      if (daysElapsed <= 0)
        return 0;
      --daysElapsed;
      return Utility.ConvertTimeToMinutes(2600) - Utility.ConvertTimeToMinutes(currentTime) + 400 + daysElapsed * 1600;
    }

    public static int CalculateMinutesBetweenTimes(int startTime, int endTime) => Utility.ConvertTimeToMinutes(endTime) - Utility.ConvertTimeToMinutes(startTime);

    public static int ModifyTime(int timestamp, int minutes_to_add)
    {
      timestamp = Utility.ConvertTimeToMinutes(timestamp);
      timestamp += minutes_to_add;
      return Utility.ConvertMinutesToTime(timestamp);
    }

    public static int ConvertMinutesToTime(int minutes) => minutes / 60 * 100 + minutes % 60;

    public static int ConvertTimeToMinutes(int time_stamp) => time_stamp / 100 * 60 + time_stamp % 100;

    public static int getSellToStorePriceOfItem(Item i, bool countStack = true) => i != null ? (i is Object ? (i as Object).sellToStorePrice() : i.salePrice() / 2) * (countStack ? i.Stack : 1) : 0;

    public static bool HasAnyPlayerSeenSecretNote(int note_number)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.secretNotesSeen.Contains(note_number))
          return true;
      }
      return false;
    }

    public static bool HasAnyPlayerSeenEvent(int event_number)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.eventsSeen.Contains(event_number))
          return true;
      }
      return false;
    }

    public static bool HaveAllPlayersSeenEvent(int event_number)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (!allFarmer.eventsSeen.Contains(event_number))
          return false;
      }
      return true;
    }

    public static List<string> GetAllPlayerUnlockedCookingRecipes()
    {
      List<string> unlockedCookingRecipes = new List<string>();
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (string key in allFarmer.cookingRecipes.Keys)
        {
          if (!unlockedCookingRecipes.Contains(key))
            unlockedCookingRecipes.Add(key);
        }
      }
      return unlockedCookingRecipes;
    }

    public static List<string> GetAllPlayerUnlockedCraftingRecipes()
    {
      List<string> unlockedCraftingRecipes = new List<string>();
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (string key in allFarmer.craftingRecipes.Keys)
        {
          if (!unlockedCraftingRecipes.Contains(key))
            unlockedCraftingRecipes.Add(key);
        }
      }
      return unlockedCraftingRecipes;
    }

    public static int GetAllPlayerFriendshipLevel(NPC npc)
    {
      int playerFriendshipLevel = -1;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.friendshipData.ContainsKey(npc.Name) && allFarmer.friendshipData[npc.Name].Points > playerFriendshipLevel)
          playerFriendshipLevel = allFarmer.friendshipData[npc.Name].Points;
      }
      return playerFriendshipLevel;
    }

    public static int GetAllPlayerReachedBottomOfMines()
    {
      int reachedBottomOfMines = 0;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.timesReachedMineBottom > reachedBottomOfMines)
          reachedBottomOfMines = allFarmer.timesReachedMineBottom;
      }
      return reachedBottomOfMines;
    }

    public static int GetAllPlayerDeepestMineLevel()
    {
      int deepestMineLevel = 0;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.deepestMineLevel > deepestMineLevel)
          deepestMineLevel = allFarmer.deepestMineLevel;
      }
      return deepestMineLevel;
    }

    public static int getRandomBasicSeasonalForageItem(string season, int randomSeedAddition = -1)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + randomSeedAddition);
      List<int> source = new List<int>();
      if (season.Equals("spring"))
        source.AddRange((IEnumerable<int>) new int[4]
        {
          16,
          18,
          20,
          22
        });
      else if (season.Equals("summer"))
        source.AddRange((IEnumerable<int>) new int[3]
        {
          396,
          398,
          402
        });
      else if (season.Equals("fall"))
        source.AddRange((IEnumerable<int>) new int[4]
        {
          404,
          406,
          408,
          410
        });
      else if (season.Equals("winter"))
        source.AddRange((IEnumerable<int>) new int[4]
        {
          412,
          414,
          416,
          418
        });
      return source.ElementAt<int>(random.Next(source.Count));
    }

    public static int getRandomPureSeasonalItem(string season, int randomSeedAddition)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + randomSeedAddition);
      List<int> source = new List<int>();
      if (season.Equals("spring"))
        source.AddRange((IEnumerable<int>) new int[15]
        {
          16,
          18,
          20,
          22,
          129,
          131,
          132,
          136,
          137,
          142,
          143,
          145,
          147,
          148,
          152
        });
      else if (season.Equals("summer"))
        source.AddRange((IEnumerable<int>) new int[16]
        {
          128,
          130,
          131,
          132,
          136,
          138,
          142,
          144,
          145,
          146,
          149,
          150,
          155,
          396,
          398,
          402
        });
      else if (season.Equals("fall"))
        source.AddRange((IEnumerable<int>) new int[17]
        {
          404,
          406,
          408,
          410,
          129,
          131,
          132,
          136,
          137,
          139,
          140,
          142,
          143,
          148,
          150,
          154,
          155
        });
      else if (season.Equals("winter"))
        source.AddRange((IEnumerable<int>) new int[17]
        {
          412,
          414,
          416,
          418,
          130,
          131,
          132,
          136,
          140,
          141,
          143,
          144,
          146,
          147,
          150,
          151,
          154
        });
      return source.ElementAt<int>(random.Next(source.Count));
    }

    public static int getRandomItemFromSeason(
      string season,
      int randomSeedAddition,
      bool forQuest,
      bool changeDaily = true)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (changeDaily ? (int) Game1.stats.DaysPlayed : 0) + randomSeedAddition);
      List<int> source = new List<int>()
      {
        68,
        66,
        78,
        80,
        86,
        152,
        167,
        153,
        420
      };
      List<string> stringList1 = new List<string>((IEnumerable<string>) Game1.player.craftingRecipes.Keys);
      List<string> stringList2 = new List<string>((IEnumerable<string>) Game1.player.cookingRecipes.Keys);
      if (forQuest)
      {
        stringList1 = Utility.GetAllPlayerUnlockedCraftingRecipes();
        stringList2 = Utility.GetAllPlayerUnlockedCookingRecipes();
      }
      if (forQuest && (MineShaft.lowestLevelReached > 40 || Utility.GetAllPlayerReachedBottomOfMines() >= 1) || !forQuest && (Game1.player.deepestMineLevel > 40 || Game1.player.timesReachedMineBottom >= 1))
        source.AddRange((IEnumerable<int>) new int[5]
        {
          62,
          70,
          72,
          84,
          422
        });
      if (forQuest && (MineShaft.lowestLevelReached > 80 || Utility.GetAllPlayerReachedBottomOfMines() >= 1) || !forQuest && (Game1.player.deepestMineLevel > 80 || Game1.player.timesReachedMineBottom >= 1))
        source.AddRange((IEnumerable<int>) new int[3]
        {
          64,
          60,
          82
        });
      if (Utility.doesAnyFarmerHaveMail("ccVault"))
        source.AddRange((IEnumerable<int>) new int[4]
        {
          88,
          90,
          164,
          165
        });
      if (stringList1.Contains("Furnace"))
        source.AddRange((IEnumerable<int>) new int[4]
        {
          334,
          335,
          336,
          338
        });
      if (stringList1.Contains("Quartz Globe"))
        source.Add(339);
      if (season.Equals("spring"))
        source.AddRange((IEnumerable<int>) new int[17]
        {
          16,
          18,
          20,
          22,
          129,
          131,
          132,
          136,
          137,
          142,
          143,
          145,
          147,
          148,
          152,
          167,
          267
        });
      else if (season.Equals("summer"))
        source.AddRange((IEnumerable<int>) new int[16]
        {
          128,
          130,
          132,
          136,
          138,
          142,
          144,
          145,
          146,
          149,
          150,
          155,
          396,
          398,
          402,
          267
        });
      else if (season.Equals("fall"))
        source.AddRange((IEnumerable<int>) new int[18]
        {
          404,
          406,
          408,
          410,
          129,
          131,
          132,
          136,
          137,
          139,
          140,
          142,
          143,
          148,
          150,
          154,
          155,
          269
        });
      else if (season.Equals("winter"))
        source.AddRange((IEnumerable<int>) new int[17]
        {
          412,
          414,
          416,
          418,
          130,
          131,
          132,
          136,
          140,
          141,
          144,
          146,
          147,
          150,
          151,
          154,
          269
        });
      if (forQuest)
      {
        foreach (string key in stringList2)
        {
          if (random.NextDouble() >= 0.4)
          {
            List<int> intList = Utility.possibleCropsAtThisTime(Game1.currentSeason, Game1.dayOfMonth <= 7);
            Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data//CookingRecipes");
            if (dictionary.ContainsKey(key))
            {
              string[] strArray = dictionary[key].Split('/')[0].Split(' ');
              bool flag = true;
              for (int index = 0; index < strArray.Length; ++index)
              {
                if (!source.Contains(Convert.ToInt32(strArray[index])) && !Utility.isCategoryIngredientAvailable(Convert.ToInt32(strArray[index])) && (intList == null || !intList.Contains(Convert.ToInt32(strArray[index]))))
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
                source.Add(Convert.ToInt32(dictionary[key].Split('/')[2]));
            }
          }
        }
      }
      return source.ElementAt<int>(random.Next(source.Count));
    }

    private static bool isCategoryIngredientAvailable(int category) => category < 0 && category != -5 && category != -6;

    public static int weatherDebrisOffsetForSeason(string season)
    {
      if (season == "spring")
        return 16;
      if (season == "summer")
        return 24;
      if (season == "fall")
        return 18;
      return season == "winter" ? 20 : 0;
    }

    public static Microsoft.Xna.Framework.Color getSkyColorForSeason(string season)
    {
      if (season == "spring")
        return new Microsoft.Xna.Framework.Color(92, 170, (int) byte.MaxValue);
      if (season == "summer")
        return new Microsoft.Xna.Framework.Color(24, 163, (int) byte.MaxValue);
      if (season == "fall")
        return new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 184, 151);
      return season == "winter" ? new Microsoft.Xna.Framework.Color(165, 207, (int) byte.MaxValue) : new Microsoft.Xna.Framework.Color(92, 170, (int) byte.MaxValue);
    }

    public static void farmerHeardSong(string trackName)
    {
      List<string> stringList = new List<string>();
      if (trackName.Equals("EarthMine"))
      {
        if (!Game1.player.songsHeard.Contains("Crystal Bells"))
          stringList.Add("Crystal Bells");
        if (!Game1.player.songsHeard.Contains("Cavern"))
          stringList.Add("Cavern");
        if (!Game1.player.songsHeard.Contains("Secret Gnomes"))
          stringList.Add("Secret Gnomes");
      }
      else if (trackName.Equals("FrostMine"))
      {
        if (!Game1.player.songsHeard.Contains("Cloth"))
          stringList.Add("Cloth");
        if (!Game1.player.songsHeard.Contains("Icicles"))
          stringList.Add("Icicles");
        if (!Game1.player.songsHeard.Contains("XOR"))
          stringList.Add("XOR");
      }
      else if (trackName.Equals("LavaMine"))
      {
        if (!Game1.player.songsHeard.Contains("Of Dwarves"))
          stringList.Add("Of Dwarves");
        if (!Game1.player.songsHeard.Contains("Near The Planet Core"))
          stringList.Add("Near The Planet Core");
        if (!Game1.player.songsHeard.Contains("Overcast"))
          stringList.Add("Overcast");
        if (!Game1.player.songsHeard.Contains("tribal"))
          stringList.Add("tribal");
      }
      else if (trackName.Equals("VolcanoMines"))
      {
        if (!Game1.player.songsHeard.Contains("VolcanoMines1"))
          stringList.Add("VolcanoMines1");
        if (!Game1.player.songsHeard.Contains("VolcanoMines2"))
          stringList.Add("VolcanoMines2");
      }
      else if (!trackName.Equals("none") && !trackName.Equals("rain"))
        stringList.Add(trackName);
      foreach (string str in stringList)
      {
        if (!Game1.player.songsHeard.Contains(str))
          Game1.player.songsHeard.Add(str);
      }
    }

    public static float getMaxedFriendshipPercent(Farmer who = null)
    {
      if (who == null)
        who = Game1.player;
      float num1 = 0.0f;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      foreach (KeyValuePair<string, Friendship> pair in who.friendshipData.Pairs)
      {
        if (dictionary.ContainsKey(pair.Key) && pair.Value.Points >= 250 * (dictionary[pair.Key].Split('/')[5] == "datable" ? 8 : 10))
          ++num1;
      }
      int num2 = dictionary.Count - 1;
      return num1 / (float) num2;
    }

    public static float getCookedRecipesPercent(Farmer who = null)
    {
      if (who == null)
        who = Game1.player;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
      float num = 0.0f;
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (who.cookingRecipes.ContainsKey(keyValuePair.Key))
        {
          int int32 = Convert.ToInt32(keyValuePair.Value.Split('/')[2].Split(' ')[0]);
          if (who.recipesCooked.ContainsKey(int32))
            ++num;
        }
      }
      return num / (float) dictionary.Count;
    }

    public static float getCraftedRecipesPercent(Farmer who = null)
    {
      if (who == null)
        who = Game1.player;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
      float num = 0.0f;
      foreach (string key in dictionary.Keys)
      {
        if (!(key == "Wedding Ring") && who.craftingRecipes.ContainsKey(key) && who.craftingRecipes[key] > 0)
          ++num;
      }
      return num / ((float) dictionary.Count - 1f);
    }

    public static float getFishCaughtPercent(Farmer who = null)
    {
      if (who == null)
        who = Game1.player;
      float num1 = 0.0f;
      float num2 = 0.0f;
      foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) Game1.objectInformation)
      {
        if (keyValuePair.Value.Split('/')[3].Contains("Fish") && (keyValuePair.Key < 167 || keyValuePair.Key > 172) && (keyValuePair.Key < 898 || keyValuePair.Key > 902))
        {
          ++num2;
          if (who.fishCaught.ContainsKey(keyValuePair.Key))
            ++num1;
        }
      }
      return num1 / num2;
    }

    public static KeyValuePair<Farmer, bool> GetFarmCompletion(
      Func<Farmer, bool> check)
    {
      if (check(Game1.player))
        return new KeyValuePair<Farmer, bool>(Game1.player, true);
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer != Game1.player && allFarmer.isCustomized.Value && check(allFarmer))
          return new KeyValuePair<Farmer, bool>(allFarmer, true);
      }
      return new KeyValuePair<Farmer, bool>(Game1.player, false);
    }

    public static KeyValuePair<Farmer, float> GetFarmCompletion(
      Func<Farmer, float> check)
    {
      Farmer key = Game1.player;
      float num1 = check(Game1.player);
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer != Game1.player && allFarmer.isCustomized.Value)
        {
          float num2 = check(allFarmer);
          if ((double) num2 > (double) num1)
          {
            key = allFarmer;
            num1 = num2;
          }
        }
      }
      return new KeyValuePair<Farmer, float>(key, num1);
    }

    public static float percentGameComplete()
    {
      float num1 = 0.0f;
      KeyValuePair<Farmer, float> farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getFarmerItemsShippedPercent(farmer)));
      double num2 = 0.0 + (double) farmCompletion1.Value * 15.0;
      float num3 = num1 + 15f;
      double num4 = (double) Math.Min((float) Utility.numObelisksOnFarm(), 4f);
      double num5 = num2 + num4;
      float num6 = num3 + 4f;
      double num7 = Game1.getFarm().isBuildingConstructed("Gold Clock") ? 10.0 : 0.0;
      double num8 = num5 + num7;
      float num9 = num6 + 10f;
      KeyValuePair<Farmer, bool> farmCompletion2 = Utility.GetFarmCompletion((Func<Farmer, bool>) (farmer => farmer.hasCompletedAllMonsterSlayerQuests.Value));
      double num10 = farmCompletion2.Value ? 10.0 : 0.0;
      double num11 = num8 + num10;
      float num12 = num9 + 10f;
      farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getMaxedFriendshipPercent(farmer)));
      double num13 = (double) farmCompletion1.Value * 11.0;
      double num14 = num11 + num13;
      float num15 = num12 + 11f;
      farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Math.Min((float) farmer.Level, 25f) / 25f));
      double num16 = (double) farmCompletion1.Value * 5.0;
      double num17 = num14 + num16;
      float num18 = num15 + 5f;
      farmCompletion2 = Utility.GetFarmCompletion((Func<Farmer, bool>) (farmer => Utility.foundAllStardrops(farmer)));
      double num19 = farmCompletion2.Value ? 10.0 : 0.0;
      double num20 = num17 + num19;
      float num21 = num18 + 10f;
      farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getCookedRecipesPercent(farmer)));
      double num22 = (double) farmCompletion1.Value * 10.0;
      double num23 = num20 + num22;
      float num24 = num21 + 10f;
      farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getCraftedRecipesPercent(farmer)));
      double num25 = (double) farmCompletion1.Value * 10.0;
      double num26 = num23 + num25;
      float num27 = num24 + 10f;
      farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getFishCaughtPercent(farmer)));
      double num28 = (double) farmCompletion1.Value * 10.0;
      double num29 = num26 + num28;
      float num30 = num27 + 10f;
      float val2 = 130f;
      double num31 = (double) Math.Min((float) (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.GoldenWalnutsFound, val2) / (double) val2 * 5.0;
      return (float) (num29 + num31) / (num30 + 5f);
    }

    public static int numObelisksOnFarm() => (Game1.getFarm().isBuildingConstructed("Water Obelisk") ? 1 : 0) + (Game1.getFarm().isBuildingConstructed("Earth Obelisk") ? 1 : 0) + (Game1.getFarm().isBuildingConstructed("Desert Obelisk") ? 1 : 0) + (Game1.getFarm().isBuildingConstructed("Island Obelisk") ? 1 : 0);

    public static bool IsDesertLocation(GameLocation location) => location.Name == "Desert" || location.Name == "SkullCave" || location.Name == "Club" || location.Name == "SandyHouse" || location.Name == "SandyShop";

    public static List<string> getOtherFarmerNames()
    {
      List<string> otherFarmerNames = new List<string>();
      Random random1 = new Random((int) Game1.uniqueIDForThisGame);
      Random random2 = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      string[] strArray1 = new string[33]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5499"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5500"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5501"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5502"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5503"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5504"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5505"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5506"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5507"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5508"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5509"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5510"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5511"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5512"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5513"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5514"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5515"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5516"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5517"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5518"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5519"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5520"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5521"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5522"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5523"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5524"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5525"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5526"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5527"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5528"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5529"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5530"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5531")
      };
      string[] strArray2 = new string[29]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5532"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5533"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5534"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5535"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5536"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5537"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5538"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5539"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5540"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5541"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5542"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5543"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5544"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5545"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5546"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5547"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5548"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5549"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5550"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5551"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5552"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5553"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5554"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5555"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5556"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5557"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5558"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5559"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5560")
      };
      string[] strArray3 = new string[17]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5561"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5562"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5563"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5564"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5565"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5566"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5567"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5568"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5569"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5570"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5571"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5572"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5573"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5574"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5575"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5576"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5577")
      };
      string[] strArray4 = new string[12]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5561"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5562"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5573"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5581"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5582"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5583"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5568"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5585"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5586"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5587"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5588"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5589")
      };
      string[] strArray5 = new string[28]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5590"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5591"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5592"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5593"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5594"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5595"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5596"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5597"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5598"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5599"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5600"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5601"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5602"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5603"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5604"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5605"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5606"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5607"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5608"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5609"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5610"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5611"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5612"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5613"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5614"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5615"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5616"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5617")
      };
      string[] strArray6 = new string[21]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5618"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5619"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5620"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5607"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5622"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5623"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5624"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5625"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5626"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5627"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5628"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5629"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5630"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5631"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5632"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5633"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5634"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5635"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5636"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5637"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5638")
      };
      string[] strArray7 = new string[9]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5639"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5640"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5641"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5642"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5643"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5644"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5645"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5646"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5647")
      };
      string[] strArray8 = new string[4]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5561"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5568"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5569"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5651")
      };
      string[] strArray9 = new string[4]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5561"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5568"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5585"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5655")
      };
      if (Game1.player.IsMale)
      {
        string str = strArray1[random1.Next(strArray1.Length)];
        for (int index = 0; index < 2; ++index)
        {
          while (otherFarmerNames.Contains(str) || Game1.player.Name.Equals(str))
            str = index != 0 ? strArray1[random2.Next(strArray1.Length)] : strArray1[random1.Next(strArray1.Length)];
          str = index != 0 ? strArray3[random2.Next(strArray3.Length)] + " " + str : strArray8[random1.Next(strArray8.Length)] + " " + str;
          otherFarmerNames.Add(str);
        }
      }
      else
      {
        string str = strArray2[random1.Next(strArray2.Length)];
        for (int index = 0; index < 2; ++index)
        {
          while (otherFarmerNames.Contains(str) || Game1.player.Name.Equals(str))
            str = index != 0 ? strArray2[random2.Next(strArray2.Length)] : strArray2[random1.Next(strArray2.Length)];
          str = index != 0 ? strArray4[random2.Next(strArray4.Length)] + " " + str : strArray9[random1.Next(strArray9.Length)] + " " + str;
          otherFarmerNames.Add(str);
        }
      }
      string str1;
      if (random2.NextDouble() < 0.5)
      {
        string str2 = strArray1[random2.Next(strArray1.Length)];
        while (Game1.player.Name.Equals(str2))
          str2 = strArray1[random2.Next(strArray1.Length)];
        str1 = random2.NextDouble() >= 0.5 ? str2 + " " + strArray7[random2.Next(strArray7.Length)] : strArray5[random2.Next(strArray5.Length)] + " " + str2;
      }
      else
      {
        string str3 = strArray2[random2.Next(strArray2.Length)];
        while (Game1.player.Name.Equals(str3))
          str3 = strArray2[random2.Next(strArray2.Length)];
        str1 = strArray6[random2.Next(strArray6.Length)] + " " + str3;
      }
      otherFarmerNames.Add(str1);
      return otherFarmerNames;
    }

    public static string getStardewHeroStandingsString()
    {
      string heroStandingsString = "";
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      List<string> otherFarmerNames = Utility.getOtherFarmerNames();
      int[] numArray = new int[otherFarmerNames.Count];
      numArray[0] = (int) ((double) Game1.stats.DaysPlayed / 208.0 * (double) Game1.percentageToWinStardewHero);
      numArray[1] = (int) ((double) numArray[0] * 0.75 + (double) random.Next(-5, 5));
      numArray[2] = Math.Max(0, numArray[1] / 2 + random.Next(-10, 0));
      if (Game1.stats.DaysPlayed < 30U)
        numArray[0] += 3;
      else if (Game1.stats.DaysPlayed < 60U)
        numArray[0] += 7;
      float num = Utility.percentGameComplete();
      bool flag = false;
      for (int index = 0; index < 3; ++index)
      {
        if ((double) num > (double) numArray[index] && !flag)
        {
          flag = true;
          heroStandingsString = heroStandingsString + Game1.player.getTitle() + " " + Game1.player.Name + " ....... " + num.ToString() + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5657") + Environment.NewLine;
        }
        heroStandingsString = heroStandingsString + otherFarmerNames[index] + " ....... " + numArray[index].ToString() + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5657") + Environment.NewLine;
      }
      if (!flag)
        heroStandingsString = heroStandingsString + Game1.player.getTitle() + " " + Game1.player.Name + " ....... " + num.ToString() + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5657");
      return heroStandingsString;
    }

    private static int cosmicFruitPercent() => Math.Max(0, (Game1.player.MaxStamina - 120) / 20);

    private static int minePercentage()
    {
      if (Game1.player.timesReachedMineBottom > 0)
        return 4;
      if (MineShaft.lowestLevelReached >= 80)
        return 2;
      return MineShaft.lowestLevelReached >= 40 ? 1 : 0;
    }

    private static int cookingPercent()
    {
      int num = 0;
      foreach (string key in Game1.player.cookingRecipes.Keys)
      {
        if (Game1.player.cookingRecipes[key] > 0)
          ++num;
      }
      return (int) ((double) (num / Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes").Count) * 3.0);
    }

    private static int craftingPercent()
    {
      int num = 0;
      foreach (string key in Game1.player.craftingRecipes.Keys)
      {
        if (Game1.player.craftingRecipes[key] > 0)
          ++num;
      }
      return (int) ((double) (num / Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes").Count) * 3.0);
    }

    private static int achievementsPercent() => (int) ((double) (Game1.player.achievements.Count / Game1.content.Load<Dictionary<int, string>>("Data\\achievements").Count) * 15.0);

    private static int itemsShippedPercent() => (int) ((double) Game1.player.basicShipped.Count() / 92.0 * 5.0);

    private static int artifactsPercent() => (int) ((double) Game1.player.archaeologyFound.Count() / 32.0 * 3.0);

    private static int fishPercent() => (int) ((double) Game1.player.fishCaught.Count() / 42.0 * 3.0);

    private static int upgradePercent()
    {
      int num1 = 0;
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Game1.player.items)
      {
        if (obj != null && obj is Tool && (obj.Name.Contains("Hoe") || obj.Name.Contains("Axe") || obj.Name.Contains("Pickaxe") || obj.Name.Contains("Can")) && (int) (NetFieldBase<int, NetInt>) ((Tool) obj).upgradeLevel == 4)
          ++num1;
      }
      int num2 = num1 + Game1.player.HouseUpgradeLevel + Game1.player.CoopUpgradeLevel + Game1.player.BarnUpgradeLevel;
      if (Game1.player.hasGreenhouse)
        ++num2;
      return num2;
    }

    private static int friendshipPercent()
    {
      int num = 0;
      foreach (string key in Game1.player.friendshipData.Keys)
        num += Game1.player.friendshipData[key].Points;
      return Math.Min(10, (int) ((double) num / 70000.0 * 10.0));
    }

    private static bool playerHasGalaxySword()
    {
      foreach (Item obj in (IEnumerable<Item>) Game1.player.Items)
      {
        if (obj != null && obj is Sword && obj.Name.Contains("Galaxy"))
          return true;
      }
      return false;
    }

    public static int getTrashReclamationPrice(Item i, Farmer f)
    {
      float num = 0.15f * (float) f.trashCanLevel;
      if (i.canBeTrashed() && !(i is Wallpaper) && !(i is Furniture))
      {
        switch (i)
        {
          case Object _ when !(bool) (NetFieldBase<bool, NetBool>) (i as Object).bigCraftable:
            return (int) ((double) i.Stack * ((double) (i as Object).sellToStorePrice() * (double) num));
          case MeleeWeapon _:
          case Ring _:
          case Boots _:
            return (int) ((double) i.Stack * ((double) (i.salePrice() / 2) * (double) num));
        }
      }
      return -1;
    }

    public static Quest getQuestOfTheDay()
    {
      if (Game1.stats.DaysPlayed <= 1U)
        return (Quest) null;
      double num = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed).NextDouble();
      return num >= 0.08 ? (num >= 0.18 || MineShaft.lowestLevelReached <= 0 || Game1.stats.DaysPlayed <= 5U ? (num >= 0.53 ? (num >= 0.6 ? (Quest) new ItemDeliveryQuest() : (Quest) new FishingQuest()) : (Quest) null) : (Quest) new SlayMonsterQuest()) : (Quest) new ResourceCollectionQuest();
    }

    public static Microsoft.Xna.Framework.Color getOppositeColor(Microsoft.Xna.Framework.Color color) => new Microsoft.Xna.Framework.Color((int) byte.MaxValue - (int) color.R, (int) byte.MaxValue - (int) color.G, (int) byte.MaxValue - (int) color.B);

    public static void drawLightningBolt(Vector2 strikePosition, GameLocation l)
    {
      Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(644, 1078, 37, 57);
      for (Vector2 position = strikePosition + new Vector2((float) (-sourceRect.Width * 4 / 2), (float) (-sourceRect.Height * 4)); (double) position.Y > (double) (-sourceRect.Height * 4); position.Y -= (float) (sourceRect.Height * 4))
        l.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 9999f, 1, 999, position, false, Game1.random.NextDouble() < 0.5, (float) (((double) strikePosition.Y + 32.0) / 10000.0 + 1.0 / 1000.0), 0.025f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          light = true,
          lightRadius = 2f,
          delayBeforeAnimationStart = 200,
          lightcolor = Microsoft.Xna.Framework.Color.Black
        });
    }

    public static string getDateStringFor(int currentDay, int currentSeason, int currentYear)
    {
      if (currentDay <= 0)
      {
        currentDay += 28;
        --currentSeason;
        if (currentSeason < 0)
        {
          currentSeason = 3;
          --currentYear;
        }
      }
      else if (currentDay > 28)
      {
        currentDay -= 28;
        ++currentSeason;
        if (currentSeason > 3)
        {
          currentSeason = 0;
          ++currentYear;
        }
      }
      return currentYear == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5677") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5678", (object) currentDay, LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es ? (object) Utility.getSeasonNameFromNumber(currentSeason).ToLower() : (object) Utility.getSeasonNameFromNumber(currentSeason), (object) currentYear);
    }

    public static string getDateString(int offset = 0)
    {
      int dayOfMonth = Game1.dayOfMonth;
      int seasonNumber = Utility.getSeasonNumber(Game1.currentSeason);
      int year = Game1.year;
      int num = offset;
      return Utility.getDateStringFor(dayOfMonth + num, seasonNumber, year);
    }

    public static string getYesterdaysDate() => Utility.getDateString(-1);

    public static string getSeasonNameFromNumber(int number)
    {
      switch (number)
      {
        case 0:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5680");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5681");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5682");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5683");
        default:
          return "";
      }
    }

    public static string getNumberEnding(int number)
    {
      if (number % 100 > 10 && number % 100 < 20)
        return "th";
      switch (number % 10)
      {
        case 0:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
          return "th";
        case 1:
          return "st";
        case 2:
          return "nd";
        case 3:
          return "rd";
        default:
          return "";
      }
    }

    public static void killAllStaticLoopingSoundCues()
    {
      if (Game1.soundBank != null)
      {
        if (Intro.roadNoise != null)
          Intro.roadNoise.Stop(AudioStopOptions.Immediate);
        if (Fly.buzz != null)
          Fly.buzz.Stop(AudioStopOptions.Immediate);
        if (Railroad.trainLoop != null)
          Railroad.trainLoop.Stop(AudioStopOptions.Immediate);
        if (BobberBar.reelSound != null)
          BobberBar.reelSound.Stop(AudioStopOptions.Immediate);
        if (BobberBar.unReelSound != null)
          BobberBar.unReelSound.Stop(AudioStopOptions.Immediate);
        if (FishingRod.reelSound != null)
          FishingRod.reelSound.Stop(AudioStopOptions.Immediate);
      }
      Game1.locationCues.StopAll();
    }

    public static void consolidateStacks(IList<Item> objects)
    {
      for (int index1 = 0; index1 < objects.Count; ++index1)
      {
        if (objects[index1] != null && objects[index1] is Object)
        {
          Object stack = objects[index1] as Object;
          for (int index2 = index1 + 1; index2 < objects.Count; ++index2)
          {
            if (objects[index2] != null && stack.canStackWith((ISalable) objects[index2]))
            {
              stack.Stack = objects[index2].addToStack((Item) stack);
              if (stack.Stack <= 0)
                break;
            }
          }
        }
      }
      for (int index = objects.Count - 1; index >= 0; --index)
      {
        if (objects[index] != null && objects[index].Stack <= 0)
          objects.RemoveAt(index);
      }
    }

    public static void performLightningUpdate(int time_of_day)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + time_of_day);
      if (random.NextDouble() < 0.125 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() / 100.0)
      {
        Farm.LightningStrikeEvent lightningStrikeEvent = new Farm.LightningStrikeEvent();
        lightningStrikeEvent.bigFlash = true;
        Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
        List<Vector2> source = new List<Vector2>();
        foreach (KeyValuePair<Vector2, Object> pair in locationFromName.objects.Pairs)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) pair.Value.bigCraftable && pair.Value.ParentSheetIndex == 9)
            source.Add(pair.Key);
        }
        if (source.Count > 0)
        {
          for (int index = 0; index < 2; ++index)
          {
            Vector2 key = source.ElementAt<Vector2>(random.Next(source.Count));
            if (locationFromName.objects[key].heldObject.Value == null)
            {
              locationFromName.objects[key].heldObject.Value = new Object(787, 1);
              locationFromName.objects[key].minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
              locationFromName.objects[key].shakeTimer = 1000;
              lightningStrikeEvent.createBolt = true;
              lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, 0.0f);
              locationFromName.lightningStrikeEvent.Fire(lightningStrikeEvent);
              return;
            }
          }
        }
        if (random.NextDouble() < 0.25 - Game1.player.team.AverageDailyLuck() - Game1.player.team.AverageLuckLevel() / 100.0)
        {
          try
          {
            KeyValuePair<Vector2, TerrainFeature> keyValuePair = locationFromName.terrainFeatures.Pairs.ElementAt(random.Next(locationFromName.terrainFeatures.Count()));
            if (!(keyValuePair.Value is FruitTree))
            {
              int num = !(keyValuePair.Value is HoeDirt) || (keyValuePair.Value as HoeDirt).crop == null ? 0 : (!(bool) (NetFieldBase<bool, NetBool>) (keyValuePair.Value as HoeDirt).crop.dead ? 1 : 0);
              if (keyValuePair.Value.performToolAction((Tool) null, 50, keyValuePair.Key, (GameLocation) locationFromName))
              {
                lightningStrikeEvent.destroyedTerrainFeature = true;
                lightningStrikeEvent.createBolt = true;
                locationFromName.terrainFeatures.Remove(keyValuePair.Key);
                lightningStrikeEvent.boltPosition = keyValuePair.Key * 64f + new Vector2(32f, (float) sbyte.MinValue);
              }
              if (num != 0)
              {
                if (keyValuePair.Value is HoeDirt)
                {
                  if ((keyValuePair.Value as HoeDirt).crop != null)
                  {
                    if ((bool) (NetFieldBase<bool, NetBool>) (keyValuePair.Value as HoeDirt).crop.dead)
                    {
                      lightningStrikeEvent.createBolt = true;
                      lightningStrikeEvent.boltPosition = keyValuePair.Key * 64f + new Vector2(32f, 0.0f);
                    }
                  }
                }
              }
            }
            else if (keyValuePair.Value is FruitTree)
            {
              (keyValuePair.Value as FruitTree).struckByLightningCountdown.Value = 4;
              (keyValuePair.Value as FruitTree).shake(keyValuePair.Key, true, (GameLocation) locationFromName);
              lightningStrikeEvent.createBolt = true;
              lightningStrikeEvent.boltPosition = keyValuePair.Key * 64f + new Vector2(32f, (float) sbyte.MinValue);
            }
          }
          catch (Exception ex)
          {
          }
        }
        locationFromName.lightningStrikeEvent.Fire(lightningStrikeEvent);
      }
      else
      {
        if (random.NextDouble() >= 0.1)
          return;
        (Game1.getLocationFromName("Farm") as Farm).lightningStrikeEvent.Fire(new Farm.LightningStrikeEvent()
        {
          smallFlash = true
        });
      }
    }

    public static void overnightLightning()
    {
      if (!Game1.IsMasterGame)
        return;
      int num = (2300 - Game1.timeOfDay) / 100;
      for (int index = 1; index <= num; ++index)
        Utility.performLightningUpdate(Game1.timeOfDay + index * 100);
    }

    public static List<Vector2> getAdjacentTileLocations(Vector2 tileLocation) => new List<Vector2>()
    {
      new Vector2(-1f, 0.0f) + tileLocation,
      new Vector2(1f, 0.0f) + tileLocation,
      new Vector2(0.0f, 1f) + tileLocation,
      new Vector2(0.0f, -1f) + tileLocation
    };

    public static List<Point> getAdjacentTilePoints(float xTile, float yTile)
    {
      List<Point> adjacentTilePoints = new List<Point>();
      int x = (int) xTile;
      int y = (int) yTile;
      adjacentTilePoints.Add(new Point(x - 1, y));
      adjacentTilePoints.Add(new Point(1 + x, y));
      adjacentTilePoints.Add(new Point(x, 1 + y));
      adjacentTilePoints.Add(new Point(x, y - 1));
      return adjacentTilePoints;
    }

    public static Vector2[] getAdjacentTileLocationsArray(Vector2 tileLocation) => new Vector2[4]
    {
      new Vector2(-1f, 0.0f) + tileLocation,
      new Vector2(1f, 0.0f) + tileLocation,
      new Vector2(0.0f, 1f) + tileLocation,
      new Vector2(0.0f, -1f) + tileLocation
    };

    public static Vector2[] getDiagonalTileLocationsArray(Vector2 tileLocation) => new Vector2[4]
    {
      new Vector2(-1f, -1f) + tileLocation,
      new Vector2(1f, 1f) + tileLocation,
      new Vector2(-1f, 1f) + tileLocation,
      new Vector2(1f, -1f) + tileLocation
    };

    public static Vector2[] getSurroundingTileLocationsArray(Vector2 tileLocation) => new Vector2[8]
    {
      new Vector2(-1f, 0.0f) + tileLocation,
      new Vector2(1f, 0.0f) + tileLocation,
      new Vector2(0.0f, 1f) + tileLocation,
      new Vector2(0.0f, -1f) + tileLocation,
      new Vector2(-1f, -1f) + tileLocation,
      new Vector2(1f, -1f) + tileLocation,
      new Vector2(1f, 1f) + tileLocation,
      new Vector2(-1f, 1f) + tileLocation
    };

    public static Crop findCloseFlower(GameLocation location, Vector2 startTileLocation) => Utility.findCloseFlower(location, startTileLocation, -1, (Func<Crop, bool>) null);

    public static Crop findCloseFlower(
      GameLocation location,
      Vector2 startTileLocation,
      int range = -1,
      Func<Crop, bool> additional_check = null)
    {
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      HashSet<Vector2> vector2Set = new HashSet<Vector2>();
      vector2Queue.Enqueue(startTileLocation);
      for (int index = 0; (range >= 0 || range < 0 && index <= 150) && vector2Queue.Count > 0; ++index)
      {
        Vector2 vector2 = vector2Queue.Dequeue();
        if (location.terrainFeatures.ContainsKey(vector2) && location.terrainFeatures[vector2] is HoeDirt && (location.terrainFeatures[vector2] as HoeDirt).crop != null && new Object((location.terrainFeatures[vector2] as HoeDirt).crop.indexOfHarvest.Value, 1).Category == -80 && (int) (NetFieldBase<int, NetInt>) (location.terrainFeatures[vector2] as HoeDirt).crop.currentPhase >= (location.terrainFeatures[vector2] as HoeDirt).crop.phaseDays.Count - 1 && !(bool) (NetFieldBase<bool, NetBool>) (location.terrainFeatures[vector2] as HoeDirt).crop.dead && (additional_check == null || additional_check((location.terrainFeatures[vector2] as HoeDirt).crop)))
          return (location.terrainFeatures[vector2] as HoeDirt).crop;
        foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
        {
          if (!vector2Set.Contains(adjacentTileLocation) && (range < 0 || (double) Math.Abs(adjacentTileLocation.X - startTileLocation.X) + (double) Math.Abs(adjacentTileLocation.Y - startTileLocation.Y) <= (double) range))
            vector2Queue.Enqueue(adjacentTileLocation);
        }
        vector2Set.Add(vector2);
      }
      return (Crop) null;
    }

    public static Point findCloseMatureCrop(Vector2 startTileLocation)
    {
      Queue<Vector2> source = new Queue<Vector2>();
      HashSet<Vector2> vector2Set = new HashSet<Vector2>();
      Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
      source.Enqueue(startTileLocation);
      for (int index = 0; index <= 40 && source.Count<Vector2>() > 0; ++index)
      {
        Vector2 vector2 = source.Dequeue();
        if (locationFromName.terrainFeatures.ContainsKey(vector2) && locationFromName.terrainFeatures[vector2] is HoeDirt && (locationFromName.terrainFeatures[vector2] as HoeDirt).crop != null && (locationFromName.terrainFeatures[vector2] as HoeDirt).readyForHarvest())
          return Utility.Vector2ToPoint(vector2);
        foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
        {
          if (!vector2Set.Contains(adjacentTileLocation))
            source.Enqueue(adjacentTileLocation);
        }
        vector2Set.Add(vector2);
      }
      return Point.Zero;
    }

    public static void recursiveFenceBuild(
      Vector2 position,
      int direction,
      GameLocation location,
      Random r)
    {
      if (r.NextDouble() < 0.04 || location.objects.ContainsKey(position) || !location.isTileLocationOpen(new Location((int) position.X, (int) position.Y)))
        return;
      location.objects.Add(position, (Object) new Fence(position, 1, false));
      int direction1 = direction;
      if (r.NextDouble() < 0.16)
        direction1 = r.Next(4);
      if (direction1 == (direction + 2) % 4)
        direction1 = (direction1 + 1) % 4;
      switch (direction)
      {
        case 0:
          Utility.recursiveFenceBuild(position + new Vector2(0.0f, -1f), direction1, location, r);
          break;
        case 1:
          Utility.recursiveFenceBuild(position + new Vector2(1f, 0.0f), direction1, location, r);
          break;
        case 2:
          Utility.recursiveFenceBuild(position + new Vector2(0.0f, 1f), direction1, location, r);
          break;
        case 3:
          Utility.recursiveFenceBuild(position + new Vector2(-1f, 0.0f), direction1, location, r);
          break;
      }
    }

    public static bool addAnimalToFarm(FarmAnimal animal)
    {
      if (animal == null || animal.Sprite == null)
        return false;
      foreach (Building building in ((BuildableGameLocation) Game1.getLocationFromName("Farm")).buildings)
      {
        if (building.buildingType.Contains((string) (NetFieldBase<string, NetString>) animal.buildingTypeILiveIn) && !((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors).isFull())
        {
          ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors).animals.Add((long) animal.myID, animal);
          ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors).animalsThatLiveHere.Add((long) animal.myID);
          animal.home = building;
          animal.setRandomPosition((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors);
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// "Standard" description is as follows:
    /// (Item type [Object (O), BigObject (BO), Weapon (W), Ring (R), Hat (H), Boot (B), Blueprint (BL), Big Object Blueprint(BBL)], follwed by item index, then stack amount)
    /// </summary>
    /// <returns>the described Item object</returns>
    public static Item getItemFromStandardTextDescription(
      string description,
      Farmer who,
      char delimiter = ' ')
    {
      string[] strArray = description.Split(delimiter);
      string str = strArray[0];
      int int32_1 = Convert.ToInt32(strArray[1]);
      int int32_2 = Convert.ToInt32(strArray[2]);
      Item obj = (Item) null;
      switch (str)
      {
        case "B":
        case "Boot":
          obj = (Item) new Boots(int32_1);
          break;
        case "BBL":
        case "BBl":
        case "BigBlueprint":
          obj = (Item) new Object(Vector2.Zero, int32_1, true);
          break;
        case "BL":
        case "Blueprint":
          obj = (Item) new Object(int32_1, 1, true);
          break;
        case "BO":
        case "BigObject":
          obj = (Item) new Object(Vector2.Zero, int32_1);
          break;
        case "C":
          obj = (Item) new Clothing(int32_1);
          break;
        case "F":
        case "Furniture":
          obj = (Item) Furniture.GetFurnitureInstance(int32_1, new Vector2?(Vector2.Zero));
          break;
        case "H":
        case "Hat":
          obj = (Item) new StardewValley.Objects.Hat(int32_1);
          break;
        case "O":
        case "Object":
          obj = (Item) new Object(int32_1, 1);
          break;
        case "R":
        case "Ring":
          obj = (Item) new Ring(int32_1);
          break;
        case "W":
        case "Weapon":
          obj = (Item) new MeleeWeapon(int32_1);
          break;
      }
      obj.Stack = int32_2;
      return who != null && obj is Object && (bool) (NetFieldBase<bool, NetBool>) (obj as Object).isRecipe && who.knowsRecipe(obj.Name) ? (Item) null : obj;
    }

    public static string getStandardDescriptionFromItem(Item item, int stack, char delimiter = ' ')
    {
      string str = "";
      int currentParentTileIndex = item.parentSheetIndex.Value;
      switch (item)
      {
        case Furniture _:
          str = "F";
          break;
        case Object _:
          Object @object = item as Object;
          str = !@object.bigCraftable.Value ? (!@object.IsRecipe ? "O" : "BL") : (!@object.IsRecipe ? "BO" : "BBL");
          break;
        case Ring _:
          str = "R";
          break;
        case Boots boots:
          str = "B";
          currentParentTileIndex = boots.indexInTileSheet.Value;
          break;
        case MeleeWeapon meleeWeapon:
          str = "W";
          currentParentTileIndex = meleeWeapon.CurrentParentTileIndex;
          break;
        case StardewValley.Objects.Hat hat:
          str = "H";
          currentParentTileIndex = hat.which.Value;
          break;
        case Clothing _:
          str = "C";
          break;
      }
      return str + delimiter.ToString() + currentParentTileIndex.ToString() + delimiter.ToString() + stack.ToString();
    }

    public static List<TemporaryAnimatedSprite> sparkleWithinArea(
      Microsoft.Xna.Framework.Rectangle bounds,
      int numberOfSparkles,
      Microsoft.Xna.Framework.Color sparkleColor,
      int delayBetweenSparkles = 100,
      int delayBeforeStarting = 0,
      string sparkleSound = "")
    {
      return Utility.getTemporarySpritesWithinArea(new int[2]
      {
        10,
        11
      }, bounds, numberOfSparkles, sparkleColor, delayBetweenSparkles, delayBeforeStarting, sparkleSound);
    }

    public static List<TemporaryAnimatedSprite> getTemporarySpritesWithinArea(
      int[] temporarySpriteRowNumbers,
      Microsoft.Xna.Framework.Rectangle bounds,
      int numberOfsprites,
      Microsoft.Xna.Framework.Color color,
      int delayBetweenSprites = 100,
      int delayBeforeStarting = 0,
      string sound = "")
    {
      List<TemporaryAnimatedSprite> spritesWithinArea = new List<TemporaryAnimatedSprite>();
      for (int index = 0; index < numberOfsprites; ++index)
        spritesWithinArea.Add(new TemporaryAnimatedSprite(temporarySpriteRowNumbers[Game1.random.Next(temporarySpriteRowNumbers.Length)], new Vector2((float) Game1.random.Next(bounds.X, bounds.Right), (float) Game1.random.Next(bounds.Y, bounds.Bottom)), color)
        {
          delayBeforeAnimationStart = delayBeforeStarting + delayBetweenSprites * index,
          startSound = sound.Length > 0 ? sound : (string) null
        });
      return spritesWithinArea;
    }

    public static Vector2 getAwayFromPlayerTrajectory(Microsoft.Xna.Framework.Rectangle monsterBox, Farmer who)
    {
      Vector2 playerTrajectory;
      ref Vector2 local = ref playerTrajectory;
      Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
      double x = (double) -(boundingBox.Center.X - monsterBox.Center.X);
      boundingBox = who.GetBoundingBox();
      double y = (double) (boundingBox.Center.Y - monsterBox.Center.Y);
      local = new Vector2((float) x, (float) y);
      if ((double) playerTrajectory.Length() <= 0.0)
      {
        if (who.FacingDirection == 3)
          playerTrajectory = new Vector2(-1f, 0.0f);
        else if (who.FacingDirection == 1)
          playerTrajectory = new Vector2(1f, 0.0f);
        else if (who.FacingDirection == 0)
          playerTrajectory = new Vector2(0.0f, 1f);
        else if (who.FacingDirection == 2)
          playerTrajectory = new Vector2(0.0f, -1f);
      }
      playerTrajectory.Normalize();
      playerTrajectory.X *= (float) (50 + Game1.random.Next(-20, 20));
      playerTrajectory.Y *= (float) (50 + Game1.random.Next(-20, 20));
      return playerTrajectory;
    }

    public static string getSongTitleFromCueName(string cueName)
    {
      switch (cueName.ToLower())
      {
        case "50s":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5724");
        case "abigailflute":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5726");
        case "abigailfluteduet":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5728");
        case "aerobics":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5730");
        case "breezy":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5752");
        case "buglevelloop":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_9");
        case "caldera":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:caldera");
        case "cavern":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5859");
        case "christmasTheme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5754");
        case "christmastheme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_7");
        case "cloth":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5756");
        case "cloudcountry":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5758");
        case "cowboy_boss":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5855");
        case "cowboy_outlawsong":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5851");
        case "cowboy_overworld":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5849");
        case "cowboy_singing":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5857");
        case "cowboy_undead":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5853");
        case "crane_game":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "crane_game_fast":
        case "junimokart":
        case "junimokart_ghostmusic":
        case "junimokart_mushroommusic":
        case "junimokart_slimemusic":
        case "junimokart_whalemusic":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "crystal bells":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5847");
        case "desolate":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5845");
        case "distantbanjo":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5843");
        case "echos":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5841");
        case "elliottpiano":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5839");
        case "emilydance":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_5");
        case "emilydream":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_6");
        case "emilytheme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_4");
        case "end_credits":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:EndCredits_SongName");
        case "event1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5835");
        case "event2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5837");
        case "fall1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5746");
        case "fall2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5748");
        case "fall3":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5750");
        case "fallfest":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5766");
        case "fieldofficetentmusic":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:fieldOfficeTentMusic");
        case "flowerdance":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5764");
        case "frogcave":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:FrogCave");
        case "grandpas_theme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5762");
        case "gusviolin":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5833");
        case "harveys_theme_jazz":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "heavy":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5794");
        case "honkytonky":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5831");
        case "icicles":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5768");
        case "islandmusic":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:IslandMusic");
        case "jaunty":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5829");
        case "jojaofficesoundscape":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5863");
        case "junimostarsong":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5825");
        case "kindadumbautumn":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5827");
        case "librarytheme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5823");
        case "maintheme":
        case "title_day":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5819");
        case "marlonstheme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5816");
        case "marnieshop":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5814");
        case "mermaidsong":
          return Game1.content.LoadString("strings\\StringsFromCSFiles:Dialogue.cs.718");
        case "moonlightjellies":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5812");
        case "movie_classic":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "movie_nature":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "movie_wumbus":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "movietheater":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "movietheaterafter":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "musicboxsong":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_8");
        case "near the planet core":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5738");
        case "night_market":
          return Game1.content.LoadString("Strings\\UI:Billboard_NightMarket");
        case "of dwarves":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5760");
        case "overcast":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5810");
        case "pirate_theme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PIRATE_THEME");
        case "pirate_theme(muffled)":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PIRATE_THEME_MUFFLED");
        case "playful":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5808");
        case "poppy":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5806");
        case "ragtime":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_1");
        case "random":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:JukeboxRandomTrack");
        case "sad_kid":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:sad_kid");
        case "sadpiano":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5804");
        case "saloon1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5802");
        case "sam_acoustic1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "sam_acoustic2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + cueName.ToLower());
        case "sampractice":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5800");
        case "secret gnomes":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5798");
        case "settlingin":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5796");
        case "shanetheme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_3");
        case "shimmeringbastion":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5792");
        case "spacemusic":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5790");
        case "spirits_eve":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5788");
        case "spring1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5718");
        case "spring2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5720");
        case "spring3":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5722");
        case "springtown":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5786");
        case "starshoot":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5784");
        case "submarine_song":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.721");
        case "summer1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5740");
        case "summer2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5742");
        case "summer3":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5744");
        case "sunroom":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:SunRoom");
        case "sweet":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5864_2");
        case "ticktock":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5782");
        case "tinymusicbox":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5780");
        case "title_night":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5821");
        case "tribal":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5778");
        case "turn_off":
          return Game1.content.LoadString("Strings\\UI:Mini_JukeBox_Off");
        case "volcanomines1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:VolcanoMines1");
        case "volcanomines2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:VolcanoMines2");
        case "wavy":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5861");
        case "wedding":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5776");
        case "winter1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5732");
        case "winter2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5734");
        case "winter3":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5736");
        case "wizardsong":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5772");
        case "woodstheme":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5774");
        case "xor":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5770");
        default:
          return cueName;
      }
    }

    public static bool isOffScreenEndFunction(
      PathNode currentNode,
      Point endPoint,
      GameLocation location,
      Character c)
    {
      return !Utility.isOnScreen(new Vector2((float) (currentNode.x * 64), (float) (currentNode.y * 64)), 32);
    }

    public static Vector2 getAwayFromPositionTrajectory(
      Microsoft.Xna.Framework.Rectangle monsterBox,
      Vector2 position)
    {
      double num1 = -((double) position.X - (double) monsterBox.Center.X);
      float num2 = position.Y - (float) monsterBox.Center.Y;
      float num3 = Math.Abs((float) num1) + Math.Abs(num2);
      if ((double) num3 < 1.0)
        num3 = 5f;
      return new Vector2((float) (num1 / (double) num3 * 20.0), (float) ((double) num2 / (double) num3 * 20.0));
    }

    public static bool tileWithinRadiusOfPlayer(int xTile, int yTile, int tileRadius, Farmer f)
    {
      Point point = new Point(xTile, yTile);
      Vector2 tileLocation = f.getTileLocation();
      return (double) Math.Abs((float) point.X - tileLocation.X) <= (double) tileRadius && (double) Math.Abs((float) point.Y - tileLocation.Y) <= (double) tileRadius;
    }

    public static bool withinRadiusOfPlayer(int x, int y, int tileRadius, Farmer f)
    {
      Point point = new Point(x / 64, y / 64);
      Vector2 tileLocation = f.getTileLocation();
      return (double) Math.Abs((float) point.X - tileLocation.X) <= (double) tileRadius && (double) Math.Abs((float) point.Y - tileLocation.Y) <= (double) tileRadius;
    }

    public static bool isThereAnObjectHereWhichAcceptsThisItem(
      GameLocation location,
      Item item,
      int x,
      int y)
    {
      if (item is Tool)
        return false;
      Vector2 vector2 = new Vector2((float) (x / 64), (float) (y / 64));
      if (location is BuildableGameLocation)
      {
        foreach (Building building in (location as BuildableGameLocation).buildings)
        {
          if (building.occupiesTile(vector2) && building.performActiveObjectDropInAction(Game1.player, true))
            return true;
        }
      }
      if (!location.Objects.ContainsKey(vector2) || location.objects[vector2].heldObject.Value != null)
        return false;
      location.objects[vector2].performObjectDropInAction(item, true, Game1.player);
      int num = location.objects[vector2].heldObject.Value != null ? 1 : 0;
      location.objects[vector2].heldObject.Value = (Object) null;
      return num != 0;
    }

    public static bool buyWallpaper()
    {
      if (Game1.player.Money < Game1.wallpaperPrice)
        return false;
      Game1.updateWallpaperInFarmHouse(Game1.currentWallpaper);
      Game1.farmerWallpaper = Game1.currentWallpaper;
      Game1.player.Money -= Game1.wallpaperPrice;
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5865"), Microsoft.Xna.Framework.Color.Green, 3500f));
      return true;
    }

    public static FarmAnimal getAnimal(long id)
    {
      if (Game1.getFarm().animals.ContainsKey(id))
        return Game1.getFarm().animals[id];
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.indoors.Value is AnimalHouse && (building.indoors.Value as AnimalHouse).animals.ContainsKey(id))
          return (building.indoors.Value as AnimalHouse).animals[id];
      }
      return (FarmAnimal) null;
    }

    public static bool buyFloor()
    {
      if (Game1.player.Money < Game1.floorPrice)
        return false;
      Game1.FarmerFloor = Game1.currentFloor;
      Game1.updateFloorInFarmHouse(Game1.currentFloor);
      Game1.player.Money -= Game1.floorPrice;
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5868"), Microsoft.Xna.Framework.Color.Green, 3500f));
      return true;
    }

    public static int numSilos()
    {
      int num = 0;
      foreach (Building building in (Game1.getLocationFromName("Farm") as Farm).buildings)
      {
        if (building.buildingType.Equals((object) "Silo") && (int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0)
          ++num;
      }
      return num;
    }

    public static Dictionary<ISalable, int[]> getCarpenterStock()
    {
      Dictionary<ISalable, int[]> stock1 = new Dictionary<ISalable, int[]>();
      Utility.AddStock(stock1, (Item) new Object(Vector2.Zero, 388, int.MaxValue));
      Utility.AddStock(stock1, (Item) new Object(Vector2.Zero, 390, int.MaxValue));
      Random r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      Utility.AddStock(stock1, (Item) new Furniture(1614, Vector2.Zero));
      Utility.AddStock(stock1, (Item) new Furniture(1616, Vector2.Zero));
      switch (Game1.dayOfMonth % 7)
      {
        case 0:
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 1296, 1391));
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 416, 537));
          break;
        case 1:
          Utility.AddStock(stock1, (Item) new Furniture(0, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(192, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(704, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1120, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1216, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1391, Vector2.Zero));
          break;
        case 2:
          Utility.AddStock(stock1, (Item) new Furniture(3, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(197, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(709, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1122, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1218, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1393, Vector2.Zero));
          break;
        case 3:
          Utility.AddStock(stock1, (Item) new Furniture(6, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(202, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(714, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1124, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1220, Vector2.Zero));
          Utility.AddStock(stock1, (Item) new Furniture(1395, Vector2.Zero));
          break;
        case 4:
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 1296, 1391));
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 1296, 1391));
          break;
        case 5:
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 1443, 1450));
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 288, 313));
          break;
        case 6:
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 1565, 1607));
          Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 12, 129));
          break;
      }
      Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1));
      Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1));
      while (r.NextDouble() < 0.25)
        Utility.AddStock(stock1, (Item) Utility.getRandomFurniture(r, stock1, 1673, 1815));
      Utility.AddStock(stock1, (Item) new Furniture(1402, Vector2.Zero));
      Dictionary<ISalable, int[]> stock2 = stock1;
      Object object1 = new Object(Vector2.Zero, 208);
      object1.Stack = int.MaxValue;
      Utility.AddStock(stock2, (Item) object1);
      if (Game1.currentSeason == "winter" || Game1.year >= 2)
      {
        Dictionary<ISalable, int[]> stock3 = stock1;
        Object object2 = new Object(Vector2.Zero, 211);
        object2.Stack = int.MaxValue;
        Utility.AddStock(stock3, (Item) object2);
      }
      if (Utility.getHomeOfFarmer(Game1.player).upgradeLevel > 0)
        Utility.AddStock(stock1, (Item) new Object(Vector2.Zero, 216));
      Utility.AddStock(stock1, (Item) new Object(Vector2.Zero, 214));
      Utility.AddStock(stock1, (Item) new TV(1466, Vector2.Zero));
      Utility.AddStock(stock1, (Item) new TV(1680, Vector2.Zero));
      if (Utility.getHomeOfFarmer(Game1.player).upgradeLevel > 0)
        Utility.AddStock(stock1, (Item) new TV(1468, Vector2.Zero));
      if (Utility.getHomeOfFarmer(Game1.player).upgradeLevel > 0)
        Utility.AddStock(stock1, (Item) new Furniture(1226, Vector2.Zero));
      Dictionary<ISalable, int[]> stock4 = stock1;
      Object object3 = new Object(Vector2.Zero, 200);
      object3.Stack = int.MaxValue;
      Utility.AddStock(stock4, (Item) object3);
      Dictionary<ISalable, int[]> stock5 = stock1;
      Object object4 = new Object(Vector2.Zero, 35);
      object4.Stack = int.MaxValue;
      Utility.AddStock(stock5, (Item) object4);
      Dictionary<ISalable, int[]> stock6 = stock1;
      Object object5 = new Object(Vector2.Zero, 46);
      object5.Stack = int.MaxValue;
      Utility.AddStock(stock6, (Item) object5);
      Utility.AddStock(stock1, (Item) new Furniture(1792, Vector2.Zero));
      Utility.AddStock(stock1, (Item) new Furniture(1794, Vector2.Zero));
      Utility.AddStock(stock1, (Item) new Furniture(1798, Vector2.Zero));
      if (Game1.player.eventsSeen.Contains(1053978))
        Utility.AddStock(stock1, (Item) new BedFurniture(2186, Vector2.Zero));
      Utility.AddStock(stock1, (Item) new BedFurniture(2048, Vector2.Zero), 250);
      if (Game1.player.HouseUpgradeLevel > 0)
        Utility.AddStock(stock1, (Item) new BedFurniture(2052, Vector2.Zero), 1000);
      if (Game1.player.HouseUpgradeLevel > 1)
        Utility.AddStock(stock1, (Item) new BedFurniture(2076, Vector2.Zero), 1000);
      if (!Game1.player.craftingRecipes.ContainsKey("Wooden Brazier"))
      {
        Dictionary<ISalable, int[]> stock7 = stock1;
        Torch torch = new Torch(Vector2.Zero, 143, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock7, (Item) torch);
      }
      else if (!Game1.player.craftingRecipes.ContainsKey("Stone Brazier"))
      {
        Dictionary<ISalable, int[]> stock8 = stock1;
        Torch torch = new Torch(Vector2.Zero, 144, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock8, (Item) torch);
      }
      else if (!Game1.player.craftingRecipes.ContainsKey("Barrel Brazier"))
      {
        Dictionary<ISalable, int[]> stock9 = stock1;
        Torch torch = new Torch(Vector2.Zero, 150, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock9, (Item) torch);
      }
      else if (!Game1.player.craftingRecipes.ContainsKey("Stump Brazier"))
      {
        Dictionary<ISalable, int[]> stock10 = stock1;
        Torch torch = new Torch(Vector2.Zero, 147, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock10, (Item) torch);
      }
      else if (!Game1.player.craftingRecipes.ContainsKey("Gold Brazier"))
      {
        Dictionary<ISalable, int[]> stock11 = stock1;
        Torch torch = new Torch(Vector2.Zero, 145, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock11, (Item) torch);
      }
      else if (!Game1.player.craftingRecipes.ContainsKey("Carved Brazier"))
      {
        Dictionary<ISalable, int[]> stock12 = stock1;
        Torch torch = new Torch(Vector2.Zero, 148, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock12, (Item) torch);
      }
      else if (!Game1.player.craftingRecipes.ContainsKey("Skull Brazier"))
      {
        Dictionary<ISalable, int[]> stock13 = stock1;
        Torch torch = new Torch(Vector2.Zero, 149, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock13, (Item) torch);
      }
      else if (!Game1.player.craftingRecipes.ContainsKey("Marble Brazier"))
      {
        Dictionary<ISalable, int[]> stock14 = stock1;
        Torch torch = new Torch(Vector2.Zero, 151, true);
        torch.IsRecipe = true;
        Utility.AddStock(stock14, (Item) torch);
      }
      if (!Game1.player.craftingRecipes.ContainsKey("Wood Lamp-post"))
        Utility.AddStock(stock1, (Item) new Object(Vector2.Zero, 152, true)
        {
          IsRecipe = true
        });
      if (!Game1.player.craftingRecipes.ContainsKey("Iron Lamp-post"))
        Utility.AddStock(stock1, (Item) new Object(Vector2.Zero, 153, true)
        {
          IsRecipe = true
        });
      if (!Game1.player.craftingRecipes.ContainsKey("Wood Floor"))
        Utility.AddStock(stock1, (Item) new Object(328, 1, true), 50);
      if (!Game1.player.craftingRecipes.ContainsKey("Rustic Plank Floor"))
        Utility.AddStock(stock1, (Item) new Object(840, 1, true), 100);
      if (!Game1.player.craftingRecipes.ContainsKey("Stone Floor"))
        Utility.AddStock(stock1, (Item) new Object(329, 1, true), 50);
      if (!Game1.player.craftingRecipes.ContainsKey("Brick Floor"))
        Utility.AddStock(stock1, (Item) new Object(293, 1, true), 250);
      if (!Game1.player.craftingRecipes.ContainsKey("Stone Walkway Floor"))
        Utility.AddStock(stock1, (Item) new Object(841, 1, true), 100);
      if (!Game1.player.craftingRecipes.ContainsKey("Stepping Stone Path"))
        Utility.AddStock(stock1, (Item) new Object(415, 1, true), 50);
      if (!Game1.player.craftingRecipes.ContainsKey("Straw Floor"))
        Utility.AddStock(stock1, (Item) new Object(401, 1, true), 100);
      if (!Game1.player.craftingRecipes.ContainsKey("Crystal Path"))
        Utility.AddStock(stock1, (Item) new Object(409, 1, true), 100);
      return stock1;
    }

    private static bool isFurnitureOffLimitsForSale(int index)
    {
      if (index <= 1764)
      {
        if (index <= 1468)
        {
          if (index <= 1226)
          {
            if (index <= 134)
            {
              if (index != 131 && index != 134)
                goto label_38;
            }
            else if ((uint) (index - 984) > 2U && index != 989 && index != 1226)
              goto label_38;
          }
          else if (index <= 1375)
          {
            if ((uint) (index - 1298) > 11U)
            {
              switch (index)
              {
                case 1371:
                case 1373:
                case 1375:
                  break;
                default:
                  goto label_38;
              }
            }
          }
          else if (index != 1402 && index != 1466 && index != 1468)
            goto label_38;
        }
        else if (index <= 1669)
        {
          if (index <= 1541)
          {
            if (index != 1471 && index != 1541)
              goto label_38;
          }
          else if (index != 1545 && index != 1554 && index != 1669)
            goto label_38;
        }
        else if (index <= 1687)
        {
          if (index != 1671 && index != 1680 && index != 1687)
            goto label_38;
        }
        else if (index != 1692 && index != 1733 && (uint) (index - 1760) > 4U)
          goto label_38;
      }
      else if (index <= 2393)
      {
        if (index <= 1918)
        {
          if (index <= 1854)
          {
            switch (index - 1796)
            {
              case 0:
              case 2:
              case 4:
              case 6:
                break;
              case 1:
              case 3:
              case 5:
                goto label_38;
              default:
                switch (index - 1838)
                {
                  case 0:
                  case 2:
                  case 4:
                  case 6:
                  case 8:
                  case 10:
                  case 12:
                  case 14:
                  case 16:
                    break;
                  default:
                    goto label_38;
                }
                break;
            }
          }
          else if (index != 1900 && index != 1902)
          {
            switch (index)
            {
              case 1907:
              case 1909:
              case 1914:
              case 1915:
              case 1916:
              case 1917:
              case 1918:
                break;
              default:
                goto label_38;
            }
          }
        }
        else if (index <= 1971)
        {
          if ((uint) (index - 1952) > 9U && index != 1971)
            goto label_38;
        }
        else if (index != 2186)
        {
          switch (index - 2326)
          {
            case 0:
            case 3:
            case 5:
            case 6:
            case 8:
              break;
            case 1:
            case 2:
            case 4:
            case 7:
              goto label_38;
            default:
              if (index == 2393)
                break;
              goto label_38;
          }
        }
      }
      else if (index <= 2502)
      {
        if (index <= 2400)
        {
          if (index != 2396 && index != 2400)
            goto label_38;
        }
        else
        {
          switch (index - 2418)
          {
            case 0:
            case 1:
            case 3:
            case 5:
            case 7:
            case 8:
            case 10:
              break;
            case 2:
            case 4:
            case 6:
            case 9:
              goto label_38;
            default:
              if (index == 2496 || index == 2502)
                break;
              goto label_38;
          }
        }
      }
      else if (index <= 2626)
      {
        if (index != 2508 && index != 2514 && (uint) (index - 2624) > 2U)
          goto label_38;
      }
      else if (index != 2653 && index != 2732 && index != 2814)
        goto label_38;
      return true;
label_38:
      return false;
    }

    private static Furniture getRandomFurniture(
      Random r,
      Dictionary<ISalable, int[]> stock,
      int lowerIndexBound = 0,
      int upperIndexBound = 1462)
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
      int num;
      do
      {
        num = r.Next(lowerIndexBound, upperIndexBound);
        if (stock != null)
        {
          foreach (Item key in stock.Keys)
          {
            if (key is Furniture && (int) (NetFieldBase<int, NetInt>) key.parentSheetIndex == num)
              num = -1;
          }
        }
      }
      while (Utility.isFurnitureOffLimitsForSale(num) || !dictionary.ContainsKey(num));
      Furniture randomFurniture = new Furniture(num, Vector2.Zero);
      randomFurniture.stack.Value = int.MaxValue;
      return randomFurniture;
    }

    public static Dictionary<ISalable, int[]> GetQiChallengeRewardStock(Farmer who)
    {
      Dictionary<ISalable, int[]> challengeRewardStock = new Dictionary<ISalable, int[]>();
      int num = int.MaxValue;
      Item key1 = (Item) new Object(Vector2.Zero, 256);
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotFirstJunimoChest"))
      {
        key1.Stack = 2;
        num = 1;
      }
      challengeRewardStock.Add((ISalable) key1, new int[4]
      {
        0,
        num,
        858,
        15 * key1.Stack
      });
      Item key2 = (Item) new Object(911, 1);
      challengeRewardStock.Add((ISalable) key2, new int[4]
      {
        0,
        int.MaxValue,
        858,
        50
      });
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotMissingStocklist"))
      {
        Item key3 = (Item) new Object(897, 1);
        (key3 as Object).questItem.Value = true;
        challengeRewardStock.Add((ISalable) key3, new int[4]
        {
          0,
          1,
          858,
          50
        });
      }
      Item key4 = (Item) new Object(Vector2.Zero, 275);
      challengeRewardStock.Add((ISalable) key4, new int[4]
      {
        0,
        int.MaxValue,
        858,
        10
      });
      Item key5 = (Item) new Object(913, 4);
      challengeRewardStock.Add((ISalable) key5, new int[4]
      {
        0,
        int.MaxValue,
        858,
        20
      });
      Item key6 = (Item) new Object(915, 4);
      challengeRewardStock.Add((ISalable) key6, new int[4]
      {
        0,
        int.MaxValue,
        858,
        20
      });
      Item key7 = (Item) new Object(Vector2.Zero, 265);
      challengeRewardStock.Add((ISalable) key7, new int[4]
      {
        0,
        int.MaxValue,
        858,
        20
      });
      if (!who.HasTownKey)
      {
        PurchaseableKeyItem key8 = new PurchaseableKeyItem(Game1.content.LoadString("Strings\\StringsFromCSFiles:KeyToTheTown"), Game1.content.LoadString("Strings\\StringsFromCSFiles:Key To The Town_desc"), 912, (Action<Farmer>) (farmer => farmer.HasTownKey = true));
        challengeRewardStock.Add((ISalable) key8, new int[4]
        {
          0,
          1,
          858,
          20
        });
      }
      Item key9 = (Item) new Object(896, 1);
      challengeRewardStock.Add((ISalable) key9, new int[4]
      {
        0,
        int.MaxValue,
        858,
        40
      });
      Item key10 = (Item) new Object(891, 1);
      challengeRewardStock.Add((ISalable) key10, new int[4]
      {
        0,
        int.MaxValue,
        858,
        5
      });
      Item key11 = (Item) new Object(908, 20);
      challengeRewardStock.Add((ISalable) key11, new int[4]
      {
        0,
        int.MaxValue,
        858,
        5
      });
      Item key12 = (Item) new Object(917, 10);
      challengeRewardStock.Add((ISalable) key12, new int[4]
      {
        0,
        int.MaxValue,
        858,
        10
      });
      Item key13 = (Item) new StardewValley.Objects.Hat(82);
      challengeRewardStock.Add((ISalable) key13, new int[4]
      {
        0,
        int.MaxValue,
        858,
        5
      });
      Item key14 = (Item) new FishTankFurniture(2400, Vector2.Zero);
      challengeRewardStock.Add((ISalable) key14, new int[4]
      {
        0,
        int.MaxValue,
        858,
        20
      });
      if (!who.craftingRecipes.ContainsKey("Heavy Tapper"))
      {
        Item key15 = (Item) new Object(Vector2.Zero, 264, true);
        challengeRewardStock.Add((ISalable) key15, new int[4]
        {
          0,
          1,
          858,
          20
        });
      }
      if (!who.craftingRecipes.ContainsKey("Hyper Speed-Gro"))
      {
        Item key16 = (Item) new Object(918, 1, true);
        challengeRewardStock.Add((ISalable) key16, new int[4]
        {
          0,
          1,
          858,
          30
        });
      }
      if (!who.craftingRecipes.ContainsKey("Deluxe Fertilizer"))
      {
        Item key17 = (Item) new Object(919, 1, true);
        challengeRewardStock.Add((ISalable) key17, new int[4]
        {
          0,
          1,
          858,
          20
        });
      }
      if (!who.craftingRecipes.ContainsKey("Hopper"))
      {
        Item key18 = (Item) new Object(Vector2.Zero, 275, true);
        challengeRewardStock.Add((ISalable) key18, new int[4]
        {
          0,
          1,
          858,
          50
        });
      }
      if (!who.craftingRecipes.ContainsKey("Magic Bait"))
      {
        Item key19 = (Item) new Object(908, 1, true);
        challengeRewardStock.Add((ISalable) key19, new int[4]
        {
          0,
          1,
          858,
          20
        });
      }
      if ((int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.GoldenWalnuts > 0 && Game1.player.hasOrWillReceiveMail("Island_FirstParrot") && Game1.player.hasOrWillReceiveMail("Island_Turtle") && Game1.player.hasOrWillReceiveMail("Island_UpgradeBridge") && Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse") && Game1.player.hasOrWillReceiveMail("Island_UpgradeParrotPlatform") && Game1.player.hasOrWillReceiveMail("Island_Resort") && Game1.player.hasOrWillReceiveMail("Island_UpgradeTrader") && Game1.player.hasOrWillReceiveMail("Island_W_Obelisk") && Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse_Mailbox") && Game1.player.hasOrWillReceiveMail("Island_VolcanoBridge") && Game1.player.hasOrWillReceiveMail("Island_VolcanoShortcutOut"))
      {
        Item key20 = (Item) new Object(858, 2);
        challengeRewardStock.Add((ISalable) key20, new int[4]
        {
          0,
          int.MaxValue,
          73,
          1
        });
      }
      Item key21 = (Item) new BedFurniture(2514, Vector2.Zero);
      challengeRewardStock.Add((ISalable) key21, new int[4]
      {
        0,
        int.MaxValue,
        858,
        50
      });
      if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
      {
        Item key22 = (Item) new Object(928, 1);
        challengeRewardStock.Add((ISalable) key22, new int[4]
        {
          0,
          int.MaxValue,
          858,
          100
        });
      }
      return challengeRewardStock;
    }

    public static Dictionary<ISalable, int[]> getAdventureRecoveryStock()
    {
      Dictionary<ISalable, int[]> adventureRecoveryStock = new Dictionary<ISalable, int[]>();
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Game1.player.itemsLostLastDeath)
      {
        if (obj != null)
        {
          obj.isLostItem = true;
          adventureRecoveryStock.Add((ISalable) obj, new int[2]
          {
            Utility.getSellToStorePriceOfItem(obj),
            obj.Stack
          });
        }
      }
      return adventureRecoveryStock;
    }

    public static Dictionary<ISalable, int[]> getAdventureShopStock()
    {
      Dictionary<ISalable, int[]> adventureShopStock = new Dictionary<ISalable, int[]>();
      int maxValue = int.MaxValue;
      adventureShopStock.Add((ISalable) new MeleeWeapon(12), new int[2]
      {
        250,
        maxValue
      });
      if (MineShaft.lowestLevelReached >= 15)
        adventureShopStock.Add((ISalable) new MeleeWeapon(17), new int[2]
        {
          500,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 20)
        adventureShopStock.Add((ISalable) new MeleeWeapon(1), new int[2]
        {
          750,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 25)
      {
        adventureShopStock.Add((ISalable) new MeleeWeapon(43), new int[2]
        {
          850,
          maxValue
        });
        adventureShopStock.Add((ISalable) new MeleeWeapon(44), new int[2]
        {
          1500,
          maxValue
        });
      }
      if (MineShaft.lowestLevelReached >= 40)
        adventureShopStock.Add((ISalable) new MeleeWeapon(27), new int[2]
        {
          2000,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 45)
        adventureShopStock.Add((ISalable) new MeleeWeapon(10), new int[2]
        {
          2000,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 55)
        adventureShopStock.Add((ISalable) new MeleeWeapon(7), new int[2]
        {
          4000,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 75)
        adventureShopStock.Add((ISalable) new MeleeWeapon(5), new int[2]
        {
          6000,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 90)
        adventureShopStock.Add((ISalable) new MeleeWeapon(50), new int[2]
        {
          9000,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 120)
        adventureShopStock.Add((ISalable) new MeleeWeapon(9), new int[2]
        {
          25000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("galaxySword"))
      {
        adventureShopStock.Add((ISalable) new MeleeWeapon(4), new int[2]
        {
          50000,
          maxValue
        });
        adventureShopStock.Add((ISalable) new MeleeWeapon(23), new int[2]
        {
          35000,
          maxValue
        });
        adventureShopStock.Add((ISalable) new MeleeWeapon(29), new int[2]
        {
          75000,
          maxValue
        });
      }
      adventureShopStock.Add((ISalable) new Boots(504), new int[2]
      {
        500,
        maxValue
      });
      if (MineShaft.lowestLevelReached >= 10)
        adventureShopStock.Add((ISalable) new Boots(506), new int[2]
        {
          500,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 50)
        adventureShopStock.Add((ISalable) new Boots(509), new int[2]
        {
          750,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 40)
        adventureShopStock.Add((ISalable) new Boots(508), new int[2]
        {
          1250,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 80)
      {
        adventureShopStock.Add((ISalable) new Boots(512), new int[2]
        {
          2000,
          maxValue
        });
        adventureShopStock.Add((ISalable) new Boots(511), new int[2]
        {
          2500,
          maxValue
        });
      }
      if (MineShaft.lowestLevelReached >= 110)
        adventureShopStock.Add((ISalable) new Boots(514), new int[2]
        {
          5000,
          maxValue
        });
      adventureShopStock.Add((ISalable) new Ring(529), new int[2]
      {
        1000,
        maxValue
      });
      adventureShopStock.Add((ISalable) new Ring(530), new int[2]
      {
        1000,
        maxValue
      });
      if (MineShaft.lowestLevelReached >= 40)
      {
        adventureShopStock.Add((ISalable) new Ring(531), new int[2]
        {
          2500,
          maxValue
        });
        adventureShopStock.Add((ISalable) new Ring(532), new int[2]
        {
          2500,
          maxValue
        });
      }
      if (MineShaft.lowestLevelReached >= 80)
      {
        adventureShopStock.Add((ISalable) new Ring(533), new int[2]
        {
          5000,
          maxValue
        });
        adventureShopStock.Add((ISalable) new Ring(534), new int[2]
        {
          5000,
          maxValue
        });
      }
      int lowestLevelReached = MineShaft.lowestLevelReached;
      if (MineShaft.lowestLevelReached >= 40)
        adventureShopStock.Add((ISalable) new Slingshot(32), new int[2]
        {
          500,
          maxValue
        });
      if (MineShaft.lowestLevelReached >= 70)
        adventureShopStock.Add((ISalable) new Slingshot(33), new int[2]
        {
          1000,
          maxValue
        });
      if (Game1.player.craftingRecipes.ContainsKey("Explosive Ammo"))
        adventureShopStock.Add((ISalable) new Object(441, int.MaxValue), new int[2]
        {
          300,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring"))
        adventureShopStock.Add((ISalable) new Ring(520), new int[2]
        {
          25000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Savage Ring"))
        adventureShopStock.Add((ISalable) new Ring(523), new int[2]
        {
          25000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Burglar's Ring"))
        adventureShopStock.Add((ISalable) new Ring(526), new int[2]
        {
          20000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Vampire Ring"))
        adventureShopStock.Add((ISalable) new Ring(522), new int[2]
        {
          15000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Crabshell Ring"))
        adventureShopStock.Add((ISalable) new Ring(810), new int[2]
        {
          15000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Napalm Ring"))
        adventureShopStock.Add((ISalable) new Ring(811), new int[2]
        {
          30000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Skeleton Mask"))
        adventureShopStock.Add((ISalable) new StardewValley.Objects.Hat(8), new int[2]
        {
          20000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Hard Hat"))
        adventureShopStock.Add((ISalable) new StardewValley.Objects.Hat(27), new int[2]
        {
          20000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Arcane Hat"))
        adventureShopStock.Add((ISalable) new StardewValley.Objects.Hat(60), new int[2]
        {
          20000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Knight's Helmet"))
        adventureShopStock.Add((ISalable) new StardewValley.Objects.Hat(50), new int[2]
        {
          20000,
          maxValue
        });
      if (Game1.player.mailReceived.Contains("Gil_Insect Head"))
        adventureShopStock.Add((ISalable) new MeleeWeapon(13), new int[2]
        {
          10000,
          maxValue
        });
      return adventureShopStock;
    }

    public static void AddStock(
      Dictionary<ISalable, int[]> stock,
      Item obj,
      int buyPrice = -1,
      int limitedQuantity = -1)
    {
      int num1 = 2 * buyPrice;
      if (buyPrice == -1)
        num1 = obj.salePrice();
      int num2 = int.MaxValue;
      if (obj is Object && (obj as Object).IsRecipe)
        num2 = 1;
      else if (limitedQuantity != -1)
        num2 = limitedQuantity;
      stock.Add((ISalable) obj, new int[2]
      {
        num1,
        num2
      });
    }

    public static Dictionary<ISalable, int[]> getSaloonStock()
    {
      Dictionary<ISalable, int[]> saloonStock = new Dictionary<ISalable, int[]>();
      Utility.AddStock(saloonStock, (Item) new Object(Vector2.Zero, 346, int.MaxValue));
      Utility.AddStock(saloonStock, (Item) new Object(Vector2.Zero, 196, int.MaxValue));
      Utility.AddStock(saloonStock, (Item) new Object(Vector2.Zero, 216, int.MaxValue));
      Utility.AddStock(saloonStock, (Item) new Object(Vector2.Zero, 224, int.MaxValue));
      Utility.AddStock(saloonStock, (Item) new Object(Vector2.Zero, 206, int.MaxValue));
      Utility.AddStock(saloonStock, (Item) new Object(Vector2.Zero, 395, int.MaxValue));
      if (!Game1.player.cookingRecipes.ContainsKey("Hashbrowns"))
        Utility.AddStock(saloonStock, (Item) new Object(210, 1, true), 25);
      if (!Game1.player.cookingRecipes.ContainsKey("Omelet"))
        Utility.AddStock(saloonStock, (Item) new Object(195, 1, true), 50);
      if (!Game1.player.cookingRecipes.ContainsKey("Pancakes"))
        Utility.AddStock(saloonStock, (Item) new Object(211, 1, true), 50);
      if (!Game1.player.cookingRecipes.ContainsKey("Bread"))
        Utility.AddStock(saloonStock, (Item) new Object(216, 1, true), 50);
      if (!Game1.player.cookingRecipes.ContainsKey("Tortilla"))
        Utility.AddStock(saloonStock, (Item) new Object(229, 1, true), 50);
      if (!Game1.player.cookingRecipes.ContainsKey("Pizza"))
        Utility.AddStock(saloonStock, (Item) new Object(206, 1, true), 75);
      if (!Game1.player.cookingRecipes.ContainsKey("Maki Roll"))
        Utility.AddStock(saloonStock, (Item) new Object(228, 1, true), 150);
      if (!Game1.player.cookingRecipes.ContainsKey("Cookies") && Game1.player.eventsSeen.Contains(19))
        Utility.AddStock(saloonStock, (Item) new Object(223, 1, true)
        {
          name = "Cookies"
        }, 150);
      if (!Game1.player.cookingRecipes.ContainsKey("Triple Shot Espresso"))
        Utility.AddStock(saloonStock, (Item) new Object(253, 1, true), 2500);
      if ((int) (NetFieldBase<int, NetInt>) Game1.dishOfTheDay.stack > 0 && !((IEnumerable<int>) Utility.getForbiddenDishesOfTheDay()).Contains<int>(Game1.dishOfTheDay.ParentSheetIndex))
        Utility.AddStock(saloonStock, (Item) (Game1.dishOfTheDay.getOne() as Object), Game1.dishOfTheDay.Price, (int) (NetFieldBase<int, NetInt>) Game1.dishOfTheDay.stack);
      Game1.player.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(SynchronizedShopStock.SynchedShop.Saloon, saloonStock);
      if (Game1.player.activeDialogueEvents.ContainsKey("willyCrabs"))
        Utility.AddStock(saloonStock, (Item) new Object(Vector2.Zero, 732, int.MaxValue));
      return saloonStock;
    }

    public static int[] getForbiddenDishesOfTheDay() => new int[7]
    {
      346,
      196,
      216,
      224,
      206,
      395,
      217
    };

    public static bool removeLightSource(int identifier)
    {
      bool flag = false;
      for (int index = Game1.currentLightSources.Count - 1; index >= 0; --index)
      {
        if ((int) (NetFieldBase<int, NetInt>) Game1.currentLightSources.ElementAt<LightSource>(index).identifier == identifier)
        {
          Game1.currentLightSources.Remove(Game1.currentLightSources.ElementAt<LightSource>(index));
          flag = true;
        }
      }
      return flag;
    }

    public static Horse findHorseForPlayer(long uid)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        foreach (NPC character in location.characters)
        {
          if (character is Horse horseForPlayer && (long) horseForPlayer.ownerId == uid)
            return horseForPlayer;
        }
      }
      return (Horse) null;
    }

    public static Horse findHorse(Guid horseId)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        foreach (NPC character in location.characters)
        {
          if (character is Horse horse && horse.HorseId == horseId)
            return horse;
        }
      }
      return (Horse) null;
    }

    public static void addDirtPuffs(
      GameLocation location,
      int tileX,
      int tileY,
      int tilesWide,
      int tilesHigh,
      int number = 5)
    {
      for (int x = tileX; x < tileX + tilesWide; ++x)
      {
        for (int y = tileY; y < tileY + tilesHigh; ++y)
        {
          for (int index = 0; index < number; ++index)
            location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.random.NextDouble() < 0.5 ? 46 : 12, new Vector2((float) x, (float) y) * 64f + new Vector2((float) Game1.random.Next(-16, 32), (float) Game1.random.Next(-16, 32)), Microsoft.Xna.Framework.Color.White, 10, Game1.random.NextDouble() < 0.5)
            {
              delayBeforeAnimationStart = Math.Max(0, Game1.random.Next(-200, 400)),
              motion = new Vector2(0.0f, -1f),
              interval = (float) Game1.random.Next(50, 80)
            });
          location.temporarySprites.Add(new TemporaryAnimatedSprite(14, new Vector2((float) x, (float) y) * 64f + new Vector2((float) Game1.random.Next(-16, 32), (float) Game1.random.Next(-16, 32)), Microsoft.Xna.Framework.Color.White, 10, Game1.random.NextDouble() < 0.5));
        }
      }
    }

    public static void addSmokePuff(
      GameLocation l,
      Vector2 v,
      int delay = 0,
      float baseScale = 2f,
      float scaleChange = 0.02f,
      float alpha = 0.75f,
      float alphaFade = 0.002f)
    {
      l.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), v, false, alphaFade, Microsoft.Xna.Framework.Color.Gray)
      {
        alpha = alpha,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2(1f / 500f, 0.0f),
        interval = 99999f,
        layerDepth = 1f,
        scale = baseScale,
        scaleChange = scaleChange,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
        delayBeforeAnimationStart = delay
      });
    }

    public static LightSource getLightSource(int identifier)
    {
      foreach (LightSource currentLightSource in Game1.currentLightSources)
      {
        if ((int) (NetFieldBase<int, NetInt>) currentLightSource.identifier == identifier)
          return currentLightSource;
      }
      return (LightSource) null;
    }

    public static Dictionary<ISalable, int[]> getAllWallpapersAndFloorsForFree()
    {
      List<ModWallpaperOrFlooring> wallpaperOrFlooringList = Game1.content.Load<List<ModWallpaperOrFlooring>>("Data\\AdditionalWallpaperFlooring");
      Dictionary<ISalable, int[]> andFloorsForFree = new Dictionary<ISalable, int[]>();
      for (int which = 0; which < 112; ++which)
      {
        Dictionary<ISalable, int[]> dictionary = andFloorsForFree;
        Wallpaper key = new Wallpaper(which);
        key.Stack = int.MaxValue;
        int[] numArray = new int[2]{ 0, int.MaxValue };
        dictionary.Add((ISalable) key, numArray);
      }
      foreach (ModWallpaperOrFlooring wallpaperOrFlooring in wallpaperOrFlooringList)
      {
        for (int which = 0; which < wallpaperOrFlooring.Count; ++which)
        {
          if (!wallpaperOrFlooring.IsFlooring)
          {
            Wallpaper wallpaper = new Wallpaper(wallpaperOrFlooring.ID, which);
            wallpaper.Stack = int.MaxValue;
            Wallpaper key = wallpaper;
            andFloorsForFree.Add((ISalable) key, new int[2]
            {
              0,
              int.MaxValue
            });
          }
        }
      }
      for (int which = 0; which < 56; ++which)
      {
        Dictionary<ISalable, int[]> dictionary = andFloorsForFree;
        Wallpaper key = new Wallpaper(which, true);
        key.Stack = int.MaxValue;
        int[] numArray = new int[2]{ 0, int.MaxValue };
        dictionary.Add((ISalable) key, numArray);
      }
      foreach (ModWallpaperOrFlooring wallpaperOrFlooring in wallpaperOrFlooringList)
      {
        for (int which = 0; which < wallpaperOrFlooring.Count; ++which)
        {
          if (wallpaperOrFlooring.IsFlooring)
          {
            Wallpaper wallpaper = new Wallpaper(wallpaperOrFlooring.ID, which);
            wallpaper.Stack = int.MaxValue;
            Wallpaper key = wallpaper;
            andFloorsForFree.Add((ISalable) key, new int[2]
            {
              0,
              int.MaxValue
            });
          }
        }
      }
      return andFloorsForFree;
    }

    public static Dictionary<ISalable, int[]> getAllFurnituresForFree()
    {
      Dictionary<ISalable, int[]> furnituresForFree = new Dictionary<ISalable, int[]>();
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
      List<Furniture> furnitureList = new List<Furniture>();
      foreach (KeyValuePair<int, string> keyValuePair in dictionary)
      {
        if (!Utility.isFurnitureOffLimitsForSale(keyValuePair.Key))
          furnitureList.Add(Furniture.GetFurnitureInstance(keyValuePair.Key));
      }
      furnitureList.Sort(new Comparison<Furniture>(Utility.SortAllFurnitures));
      foreach (Furniture key in furnitureList)
        furnituresForFree.Add((ISalable) key, new int[2]
        {
          0,
          int.MaxValue
        });
      furnituresForFree.Add((ISalable) new Furniture(1402, Vector2.Zero), new int[2]
      {
        0,
        int.MaxValue
      });
      furnituresForFree.Add((ISalable) new TV(1680, Vector2.Zero), new int[2]
      {
        0,
        int.MaxValue
      });
      furnituresForFree.Add((ISalable) new TV(1466, Vector2.Zero), new int[2]
      {
        0,
        int.MaxValue
      });
      furnituresForFree.Add((ISalable) new TV(1468, Vector2.Zero), new int[2]
      {
        0,
        int.MaxValue
      });
      return furnituresForFree;
    }

    public static int SortAllFurnitures(Furniture a, Furniture b)
    {
      if ((NetFieldBase<int, NetInt>) a.furniture_type != b.furniture_type)
        return a.furniture_type.Value.CompareTo(b.furniture_type.Value);
      if ((int) (NetFieldBase<int, NetInt>) a.furniture_type == 12 && (int) (NetFieldBase<int, NetInt>) b.furniture_type == 12)
      {
        int num1 = a.Name.StartsWith("Floor Divider ") ? 1 : 0;
        bool flag = b.Name.StartsWith("Floor Divider ");
        int num2 = flag ? 1 : 0;
        if (num1 != num2)
          return flag ? -1 : 1;
      }
      return a.ParentSheetIndex.CompareTo(b.ParentSheetIndex);
    }

    public static bool doesAnyFarmerHaveOrWillReceiveMail(string id)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.hasOrWillReceiveMail(id))
          return true;
      }
      return false;
    }

    public static string loadStringShort(string fileWithinStringsFolder, string key) => Game1.content.LoadString("Strings\\" + fileWithinStringsFolder + ":" + key);

    public static string loadStringDataShort(string fileWithinStringsFolder, string key) => Game1.content.LoadString("Data\\" + fileWithinStringsFolder + ":" + key);

    public static bool doesAnyFarmerHaveMail(string id)
    {
      if (Game1.player.mailReceived.Contains(id))
        return true;
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      {
        if (farmer.mailReceived.Contains(id))
          return true;
      }
      return false;
    }

    public static FarmEvent pickFarmEvent() => Game1.hooks.OnUtility_PickFarmEvent((Func<FarmEvent>) (() =>
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      if (Game1.weddingToday)
        return (FarmEvent) null;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        Friendship spouseFriendship = onlineFarmer.GetSpouseFriendship();
        if (spouseFriendship != null && spouseFriendship.IsMarried() && spouseFriendship.WeddingDate == Game1.Date)
          return (FarmEvent) null;
      }
      if (Game1.stats.DaysPlayed == 31U)
        return (FarmEvent) new SoundInTheNightEvent(4);
      if (Game1.MasterPlayer.mailForTomorrow.Contains("leoMoved%&NL&%") || Game1.MasterPlayer.mailForTomorrow.Contains("leoMoved"))
        return (FarmEvent) new WorldChangeEvent(14);
      if (Game1.player.mailForTomorrow.Contains("jojaPantry%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaPantry"))
        return (FarmEvent) new WorldChangeEvent(0);
      if (Game1.player.mailForTomorrow.Contains("ccPantry%&NL&%") || Game1.player.mailForTomorrow.Contains("ccPantry"))
        return (FarmEvent) new WorldChangeEvent(1);
      if (Game1.player.mailForTomorrow.Contains("jojaVault%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaVault"))
        return (FarmEvent) new WorldChangeEvent(6);
      if (Game1.player.mailForTomorrow.Contains("ccVault%&NL&%") || Game1.player.mailForTomorrow.Contains("ccVault"))
        return (FarmEvent) new WorldChangeEvent(7);
      if (Game1.player.mailForTomorrow.Contains("jojaBoilerRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaBoilerRoom"))
        return (FarmEvent) new WorldChangeEvent(2);
      if (Game1.player.mailForTomorrow.Contains("ccBoilerRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("ccBoilerRoom"))
        return (FarmEvent) new WorldChangeEvent(3);
      if (Game1.player.mailForTomorrow.Contains("jojaCraftsRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaCraftsRoom"))
        return (FarmEvent) new WorldChangeEvent(4);
      if (Game1.player.mailForTomorrow.Contains("ccCraftsRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("ccCraftsRoom"))
        return (FarmEvent) new WorldChangeEvent(5);
      if (Game1.player.mailForTomorrow.Contains("jojaFishTank%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaFishTank"))
        return (FarmEvent) new WorldChangeEvent(8);
      if (Game1.player.mailForTomorrow.Contains("ccFishTank%&NL&%") || Game1.player.mailForTomorrow.Contains("ccFishTank"))
        return (FarmEvent) new WorldChangeEvent(9);
      if (Game1.player.mailForTomorrow.Contains("ccMovieTheaterJoja%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaMovieTheater"))
        return (FarmEvent) new WorldChangeEvent(10);
      if (Game1.player.mailForTomorrow.Contains("ccMovieTheater%&NL&%") || Game1.player.mailForTomorrow.Contains("ccMovieTheater"))
        return (FarmEvent) new WorldChangeEvent(11);
      if (Game1.MasterPlayer.eventsSeen.Contains(191393) && (Game1.isRaining || Game1.isLightning) && !Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible") && !Game1.MasterPlayer.mailReceived.Contains("ccMovieTheater"))
        return (FarmEvent) new WorldChangeEvent(12);
      if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatTicketMachine") && Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull") && Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor") && !Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed"))
        return (FarmEvent) new WorldChangeEvent(13);
      if (random.NextDouble() < 0.01 && !Game1.currentSeason.Equals("winter"))
        return (FarmEvent) new FairyEvent();
      if (random.NextDouble() < 0.01)
        return (FarmEvent) new WitchEvent();
      if (random.NextDouble() < 0.01)
        return (FarmEvent) new SoundInTheNightEvent(1);
      if (random.NextDouble() < 0.005)
        return (FarmEvent) new SoundInTheNightEvent(3);
      if (random.NextDouble() >= 0.008 || Game1.year <= 1 || Game1.MasterPlayer.mailReceived.Contains("Got_Capsule"))
        return (FarmEvent) null;
      Game1.MasterPlayer.mailReceived.Add("Got_Capsule");
      return (FarmEvent) new SoundInTheNightEvent(0);
    }));

    public static bool hasFinishedJojaRoute()
    {
      bool flag = false;
      if (Game1.player.mailReceived.Contains("jojaVault"))
        flag = true;
      else if (!Game1.player.mailReceived.Contains("ccVault"))
        return false;
      if (Game1.player.mailReceived.Contains("jojaPantry"))
        flag = true;
      else if (!Game1.player.mailReceived.Contains("ccPantry"))
        return false;
      if (Game1.player.mailReceived.Contains("jojaBoilerRoom"))
        flag = true;
      else if (!Game1.player.mailReceived.Contains("ccBoilerRoom"))
        return false;
      if (Game1.player.mailReceived.Contains("jojaCraftsRoom"))
        flag = true;
      else if (!Game1.player.mailReceived.Contains("ccCraftsRoom"))
        return false;
      if (Game1.player.mailReceived.Contains("jojaFishTank"))
        flag = true;
      else if (!Game1.player.mailReceived.Contains("ccFishTank"))
        return false;
      return flag || Game1.player.mailReceived.Contains("JojaMember");
    }

    public static FarmEvent pickPersonalFarmEvent()
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 ^ 470124797 + (int) Game1.player.UniqueMultiplayerID);
      if (Game1.weddingToday)
        return (FarmEvent) null;
      if (Game1.player.isMarried() && Game1.player.GetSpouseFriendship().DaysUntilBirthing <= 0 && Game1.player.GetSpouseFriendship().NextBirthingDate != (WorldDate) null)
      {
        if (Game1.player.spouse != null)
          return (FarmEvent) new BirthingEvent();
        long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
        if (Game1.otherFarmers.ContainsKey(key))
          return (FarmEvent) new PlayerCoupleBirthingEvent();
      }
      else
      {
        if (Game1.player.isMarried() && Game1.player.spouse != null && Game1.getCharacterFromName(Game1.player.spouse).canGetPregnant() && Game1.player.currentLocation == Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) Game1.player.homeLocation) && random.NextDouble() < 0.05)
          return (FarmEvent) new QuestionEvent(1);
        if (Game1.player.isMarried())
        {
          long? spouse = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID);
          if (spouse.HasValue && Game1.player.GetSpouseFriendship().NextBirthingDate == (WorldDate) null && random.NextDouble() < 0.05)
          {
            spouse = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID);
            long key = spouse.Value;
            if (Game1.otherFarmers.ContainsKey(key))
            {
              Farmer otherFarmer = Game1.otherFarmers[key];
              if (otherFarmer.currentLocation == Game1.player.currentLocation && (otherFarmer.currentLocation == Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) otherFarmer.homeLocation) || otherFarmer.currentLocation == Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) Game1.player.homeLocation)) && Utility.playersCanGetPregnantHere(otherFarmer.currentLocation as FarmHouse))
                return (FarmEvent) new QuestionEvent(3);
            }
          }
        }
      }
      return random.NextDouble() < 0.5 ? (FarmEvent) new QuestionEvent(2) : (FarmEvent) new SoundInTheNightEvent(2);
    }

    private static bool playersCanGetPregnantHere(FarmHouse farmHouse)
    {
      List<Child> children = farmHouse.getChildren();
      if (farmHouse.cribStyle.Value <= 0 || farmHouse.getChildrenCount() >= 2 || farmHouse.upgradeLevel < 2 || children.Count >= 2)
        return false;
      return children.Count == 0 || children[0].Age > 2;
    }

    public static string capitalizeFirstLetter(string s) => s == null || s.Length < 1 ? "" : s[0].ToString().ToUpper() + (s.Length > 1 ? s.Substring(1) : "");

    public static Dictionary<ISalable, int[]> getBlacksmithStock() => new Dictionary<ISalable, int[]>()
    {
      {
        (ISalable) new Object(Vector2.Zero, 378, int.MaxValue),
        new int[2]{ Game1.year > 1 ? 150 : 75, int.MaxValue }
      },
      {
        (ISalable) new Object(Vector2.Zero, 380, int.MaxValue),
        new int[2]{ Game1.year > 1 ? 250 : 150, int.MaxValue }
      },
      {
        (ISalable) new Object(Vector2.Zero, 382, int.MaxValue),
        new int[2]{ Game1.year > 1 ? 250 : 150, int.MaxValue }
      },
      {
        (ISalable) new Object(Vector2.Zero, 384, int.MaxValue),
        new int[2]{ Game1.year > 1 ? 750 : 400, int.MaxValue }
      }
    };

    public static bool alreadyHasLightSourceWithThisID(int identifier)
    {
      foreach (LightSource currentLightSource in Game1.currentLightSources)
      {
        if ((int) (NetFieldBase<int, NetInt>) currentLightSource.identifier == identifier)
          return true;
      }
      return false;
    }

    public static void repositionLightSource(int identifier, Vector2 position)
    {
      foreach (LightSource currentLightSource in Game1.currentLightSources)
      {
        if ((int) (NetFieldBase<int, NetInt>) currentLightSource.identifier == identifier)
          currentLightSource.position.Value = position;
      }
    }

    public static Dictionary<ISalable, int[]> getAnimalShopStock()
    {
      Dictionary<ISalable, int[]> animalShopStock = new Dictionary<ISalable, int[]>();
      animalShopStock.Add((ISalable) new Object(178, 1), new int[2]
      {
        50,
        int.MaxValue
      });
      Object key = new Object(Vector2.Zero, 104);
      key.price.Value = 2000;
      key.Stack = 1;
      animalShopStock.Add((ISalable) key, new int[2]
      {
        2000,
        int.MaxValue
      });
      if (Game1.player.hasItemWithNameThatContains("Milk Pail") == null)
        animalShopStock.Add((ISalable) new MilkPail(), new int[2]
        {
          1000,
          1
        });
      if (Game1.player.hasItemWithNameThatContains("Shears") == null)
        animalShopStock.Add((ISalable) new Shears(), new int[2]
        {
          1000,
          1
        });
      if ((int) (NetFieldBase<int, NetInt>) Game1.player.farmingLevel >= 10)
        animalShopStock.Add((ISalable) new Object(Vector2.Zero, 165), new int[2]
        {
          25000,
          int.MaxValue
        });
      animalShopStock.Add((ISalable) new Object(Vector2.Zero, 45), new int[2]
      {
        250,
        int.MaxValue
      });
      if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
        animalShopStock.Add((ISalable) new Object(928, 1), new int[2]
        {
          100000,
          int.MaxValue
        });
      return animalShopStock;
    }

    public static bool areThereAnyOtherAnimalsWithThisName(string name)
    {
      Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
      foreach (Building building in locationFromName.buildings)
      {
        if (building.indoors.Value is AnimalHouse)
        {
          foreach (FarmAnimal farmAnimal in (building.indoors.Value as AnimalHouse).animals.Values)
          {
            if (farmAnimal.displayName != null && farmAnimal.displayName.Equals(name))
              return true;
          }
        }
      }
      foreach (FarmAnimal farmAnimal in locationFromName.animals.Values)
      {
        if (farmAnimal.displayName != null && farmAnimal.displayName.Equals(name))
          return true;
      }
      return false;
    }

    public static string getNumberWithCommas(int number)
    {
      StringBuilder stringBuilder = new StringBuilder(number.ToString() ?? "");
      string str = ",";
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt)
        str = ".";
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.mod && LocalizedContentManager.CurrentModLanguage != null)
        str = LocalizedContentManager.CurrentModLanguage.NumberComma;
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru)
        str = " ";
      for (int index = stringBuilder.Length - 4; index >= 0; index -= 3)
        stringBuilder.Insert(index + 1, str);
      return stringBuilder.ToString();
    }

    public static List<Object> getPurchaseAnimalStock()
    {
      List<Object> purchaseAnimalStock = new List<Object>();
      Object object1 = new Object(100, 1, price: 400);
      object1.Name = "Chicken";
      object1.Type = Game1.getFarm().isBuildingConstructed("Coop") || Game1.getFarm().isBuildingConstructed("Deluxe Coop") || Game1.getFarm().isBuildingConstructed("Big Coop") ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5926");
      object1.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5922");
      purchaseAnimalStock.Add(object1);
      Object object2 = new Object(100, 1, price: 750);
      object2.Name = "Dairy Cow";
      object2.Type = Game1.getFarm().isBuildingConstructed("Barn") || Game1.getFarm().isBuildingConstructed("Deluxe Barn") || Game1.getFarm().isBuildingConstructed("Big Barn") ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5931");
      object2.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5927");
      purchaseAnimalStock.Add(object2);
      Object object3 = new Object(100, 1, price: 2000);
      object3.Name = "Goat";
      object3.Type = Game1.getFarm().isBuildingConstructed("Big Barn") || Game1.getFarm().isBuildingConstructed("Deluxe Barn") ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5936");
      object3.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5933");
      purchaseAnimalStock.Add(object3);
      Object object4 = new Object(100, 1, price: 600);
      object4.Name = "Duck";
      object4.Type = Game1.getFarm().isBuildingConstructed("Big Coop") || Game1.getFarm().isBuildingConstructed("Deluxe Coop") ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5940");
      object4.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5937");
      purchaseAnimalStock.Add(object4);
      Object object5 = new Object(100, 1, price: 4000);
      object5.Name = "Sheep";
      object5.Type = Game1.getFarm().isBuildingConstructed("Deluxe Barn") ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5944");
      object5.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5942");
      purchaseAnimalStock.Add(object5);
      Object object6 = new Object(100, 1, price: 4000);
      object6.Name = "Rabbit";
      object6.Type = Game1.getFarm().isBuildingConstructed("Deluxe Coop") ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5947");
      object6.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5945");
      purchaseAnimalStock.Add(object6);
      Object object7 = new Object(100, 1, price: 8000);
      object7.Name = "Pig";
      object7.Type = Game1.getFarm().isBuildingConstructed("Deluxe Barn") ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5950");
      object7.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5948");
      purchaseAnimalStock.Add(object7);
      return purchaseAnimalStock;
    }

    public static void FixChildNameCollisions()
    {
      List<NPC> list = new List<NPC>();
      Utility.getAllCharacters(list);
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      foreach (NPC npc1 in list)
      {
        if (npc1 is Child)
        {
          string name1 = npc1.Name;
          string name2 = npc1.Name;
          bool flag;
          do
          {
            flag = false;
            if (dictionary.ContainsKey(name2))
            {
              name2 += " ";
              flag = true;
            }
            else
            {
              foreach (NPC npc2 in list)
              {
                if (npc2 != npc1 && npc2.name.Equals((object) name2))
                {
                  name2 += " ";
                  flag = true;
                }
              }
            }
          }
          while (flag);
          if (name2 != npc1.Name)
          {
            npc1.Name = name2;
            npc1.displayName = (string) null;
            string displayName = npc1.displayName;
            foreach (Farmer allFarmer in Game1.getAllFarmers())
            {
              if (allFarmer.friendshipData != null && allFarmer.friendshipData.ContainsKey(name1))
              {
                allFarmer.friendshipData[name2] = allFarmer.friendshipData[name1];
                allFarmer.friendshipData.Remove(name1);
              }
            }
          }
        }
      }
    }

    public static List<Item> getShopStock(bool Pierres)
    {
      List<Item> shopStock = new List<Item>();
      if (Pierres)
      {
        if (Game1.currentSeason.Equals("spring"))
        {
          shopStock.Add((Item) new Object(Vector2.Zero, 472, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 473, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 474, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 475, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 427, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 429, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 477, int.MaxValue));
          shopStock.Add((Item) new Object(628, int.MaxValue, price: 1700));
          shopStock.Add((Item) new Object(629, int.MaxValue, price: 1000));
          if (Game1.year > 1)
            shopStock.Add((Item) new Object(Vector2.Zero, 476, int.MaxValue));
        }
        if (Game1.currentSeason.Equals("summer"))
        {
          shopStock.Add((Item) new Object(Vector2.Zero, 480, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 482, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 483, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 484, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 479, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 302, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 453, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 455, int.MaxValue));
          shopStock.Add((Item) new Object(630, int.MaxValue, price: 2000));
          shopStock.Add((Item) new Object(631, int.MaxValue, price: 3000));
          if (Game1.year > 1)
            shopStock.Add((Item) new Object(Vector2.Zero, 485, int.MaxValue));
        }
        if (Game1.currentSeason.Equals("fall"))
        {
          shopStock.Add((Item) new Object(Vector2.Zero, 487, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 488, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 490, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 299, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 301, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 492, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 491, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 493, int.MaxValue));
          shopStock.Add((Item) new Object(431, int.MaxValue, price: 100));
          shopStock.Add((Item) new Object(Vector2.Zero, 425, int.MaxValue));
          shopStock.Add((Item) new Object(632, int.MaxValue, price: 3000));
          shopStock.Add((Item) new Object(633, int.MaxValue, price: 2000));
          if (Game1.year > 1)
            shopStock.Add((Item) new Object(Vector2.Zero, 489, int.MaxValue));
        }
        shopStock.Add((Item) new Object(Vector2.Zero, 297, int.MaxValue));
        shopStock.Add((Item) new Object(Vector2.Zero, 245, int.MaxValue));
        shopStock.Add((Item) new Object(Vector2.Zero, 246, int.MaxValue));
        shopStock.Add((Item) new Object(Vector2.Zero, 423, int.MaxValue));
        Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
        List<Item> objList1 = shopStock;
        Wallpaper wallpaper1 = new Wallpaper(random.Next(112));
        wallpaper1.Stack = int.MaxValue;
        objList1.Add((Item) wallpaper1);
        List<Item> objList2 = shopStock;
        Wallpaper wallpaper2 = new Wallpaper(random.Next(40), true);
        wallpaper2.Stack = int.MaxValue;
        objList2.Add((Item) wallpaper2);
        List<Item> objList3 = shopStock;
        Clothing clothing = new Clothing(1000 + random.Next(128));
        clothing.Stack = int.MaxValue;
        clothing.Price = 1000;
        objList3.Add((Item) clothing);
        if (Game1.player.achievements.Contains(38))
          shopStock.Add((Item) new Object(Vector2.Zero, 458, int.MaxValue));
      }
      else
      {
        if (Game1.currentSeason.Equals("spring"))
          shopStock.Add((Item) new Object(Vector2.Zero, 478, int.MaxValue));
        if (Game1.currentSeason.Equals("summer"))
        {
          shopStock.Add((Item) new Object(Vector2.Zero, 486, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 481, int.MaxValue));
        }
        if (Game1.currentSeason.Equals("fall"))
        {
          shopStock.Add((Item) new Object(Vector2.Zero, 493, int.MaxValue));
          shopStock.Add((Item) new Object(Vector2.Zero, 494, int.MaxValue));
        }
        shopStock.Add((Item) new Object(Vector2.Zero, 88, int.MaxValue));
        shopStock.Add((Item) new Object(Vector2.Zero, 90, int.MaxValue));
      }
      return shopStock;
    }

    public static Vector2 getCornersOfThisRectangle(ref Microsoft.Xna.Framework.Rectangle r, int corner)
    {
      switch (corner)
      {
        case 1:
          return new Vector2((float) (r.Right - 1), (float) r.Y);
        case 2:
          return new Vector2((float) (r.Right - 1), (float) (r.Bottom - 1));
        case 3:
          return new Vector2((float) r.X, (float) (r.Bottom - 1));
        default:
          return new Vector2((float) r.X, (float) r.Y);
      }
    }

    private static int priceForToolUpgradeLevel(int level)
    {
      switch (level)
      {
        case 1:
          return 2000;
        case 2:
          return 5000;
        case 3:
          return 10000;
        case 4:
          return 25000;
        default:
          return 2000;
      }
    }

    private static int indexOfExtraMaterialForToolUpgrade(int level)
    {
      switch (level)
      {
        case 1:
          return 334;
        case 2:
          return 335;
        case 3:
          return 336;
        case 4:
          return 337;
        default:
          return 334;
      }
    }

    public static Dictionary<ISalable, int[]> getBlacksmithUpgradeStock(Farmer who)
    {
      Dictionary<ISalable, int[]> blacksmithUpgradeStock = new Dictionary<ISalable, int[]>();
      Tool toolFromName1 = who.getToolFromName("Axe");
      Tool toolFromName2 = who.getToolFromName("Watering Can");
      Tool toolFromName3 = who.getToolFromName("Pickaxe");
      Tool toolFromName4 = who.getToolFromName("Hoe");
      if (toolFromName1 != null && (int) (NetFieldBase<int, NetInt>) toolFromName1.upgradeLevel < 4)
      {
        Tool key = (Tool) new Axe();
        key.UpgradeLevel = (int) (NetFieldBase<int, NetInt>) toolFromName1.upgradeLevel + 1;
        blacksmithUpgradeStock.Add((ISalable) key, new int[3]
        {
          Utility.priceForToolUpgradeLevel(key.UpgradeLevel),
          1,
          Utility.indexOfExtraMaterialForToolUpgrade((int) (NetFieldBase<int, NetInt>) key.upgradeLevel)
        });
      }
      if (toolFromName2 != null && (int) (NetFieldBase<int, NetInt>) toolFromName2.upgradeLevel < 4)
      {
        Tool key = (Tool) new WateringCan();
        key.UpgradeLevel = (int) (NetFieldBase<int, NetInt>) toolFromName2.upgradeLevel + 1;
        blacksmithUpgradeStock.Add((ISalable) key, new int[3]
        {
          Utility.priceForToolUpgradeLevel(key.UpgradeLevel),
          1,
          Utility.indexOfExtraMaterialForToolUpgrade((int) (NetFieldBase<int, NetInt>) key.upgradeLevel)
        });
      }
      if (toolFromName3 != null && (int) (NetFieldBase<int, NetInt>) toolFromName3.upgradeLevel < 4)
      {
        Tool key = (Tool) new Pickaxe();
        key.UpgradeLevel = (int) (NetFieldBase<int, NetInt>) toolFromName3.upgradeLevel + 1;
        blacksmithUpgradeStock.Add((ISalable) key, new int[3]
        {
          Utility.priceForToolUpgradeLevel(key.UpgradeLevel),
          1,
          Utility.indexOfExtraMaterialForToolUpgrade((int) (NetFieldBase<int, NetInt>) key.upgradeLevel)
        });
      }
      if (toolFromName4 != null && (int) (NetFieldBase<int, NetInt>) toolFromName4.upgradeLevel < 4)
      {
        Tool key = (Tool) new Hoe();
        key.UpgradeLevel = (int) (NetFieldBase<int, NetInt>) toolFromName4.upgradeLevel + 1;
        blacksmithUpgradeStock.Add((ISalable) key, new int[3]
        {
          Utility.priceForToolUpgradeLevel(key.UpgradeLevel),
          1,
          Utility.indexOfExtraMaterialForToolUpgrade((int) (NetFieldBase<int, NetInt>) key.upgradeLevel)
        });
      }
      if (who.trashCanLevel < 4)
      {
        string name = "";
        switch (who.trashCanLevel + 1)
        {
          case 1:
            name = Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14299", (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:TrashCan"));
            break;
          case 2:
            name = Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14300", (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:TrashCan"));
            break;
          case 3:
            name = Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14301", (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:TrashCan"));
            break;
          case 4:
            name = Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14302", (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:TrashCan"));
            break;
        }
        Tool key = (Tool) new GenericTool(name, Game1.content.LoadString("Strings\\StringsFromCSFiles:TrashCan_Description", (object) (((who.trashCanLevel + 1) * 15).ToString() ?? "")), who.trashCanLevel + 1, 13 + who.trashCanLevel, 13 + who.trashCanLevel);
        blacksmithUpgradeStock.Add((ISalable) key, new int[3]
        {
          Utility.priceForToolUpgradeLevel(who.trashCanLevel + 1) / 2,
          1,
          Utility.indexOfExtraMaterialForToolUpgrade(who.trashCanLevel + 1)
        });
      }
      return blacksmithUpgradeStock;
    }

    public static Vector2 GetCurvePoint(
      float t,
      Vector2 p0,
      Vector2 p1,
      Vector2 p2,
      Vector2 p3)
    {
      float num1 = (float) (3.0 * ((double) p1.X - (double) p0.X));
      float num2 = (float) (3.0 * ((double) p1.Y - (double) p0.Y));
      float num3 = (float) (3.0 * ((double) p2.X - (double) p1.X)) - num1;
      float num4 = (float) (3.0 * ((double) p2.Y - (double) p1.Y)) - num2;
      double num5 = (double) p3.X - (double) p0.X - (double) num1 - (double) num3;
      float num6 = p3.Y - p0.Y - num2 - num4;
      float num7 = t * t * t;
      float num8 = t * t;
      double num9 = (double) num7;
      return new Vector2((float) (num5 * num9 + (double) num3 * (double) num8 + (double) num1 * (double) t) + p0.X, (float) ((double) num6 * (double) num7 + (double) num4 * (double) num8 + (double) num2 * (double) t) + p0.Y);
    }

    public static GameLocation getGameLocationOfCharacter(NPC n) => n.currentLocation;

    public static int[] parseStringToIntArray(string s, char delimiter = ' ')
    {
      string[] strArray = s.Split(delimiter);
      int[] stringToIntArray = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        stringToIntArray[index] = Convert.ToInt32(strArray[index]);
      return stringToIntArray;
    }

    public static void drawLineWithScreenCoordinates(
      int x1,
      int y1,
      int x2,
      int y2,
      SpriteBatch b,
      Microsoft.Xna.Framework.Color color1,
      float layerDepth = 1f)
    {
      Vector2 vector2_1 = new Vector2((float) x2, (float) y2);
      Vector2 vector2_2 = new Vector2((float) x1, (float) y1);
      Vector2 vector2_3 = vector2_2;
      Vector2 vector2_4 = vector2_1 - vector2_3;
      float rotation = (float) Math.Atan2((double) vector2_4.Y, (double) vector2_4.X);
      b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int) vector2_2.X, (int) vector2_2.Y, (int) vector2_4.Length(), 1), new Microsoft.Xna.Framework.Rectangle?(), color1, rotation, new Vector2(0.0f, 0.0f), SpriteEffects.None, layerDepth);
      b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int) vector2_2.X, (int) vector2_2.Y + 1, (int) vector2_4.Length(), 1), new Microsoft.Xna.Framework.Rectangle?(), color1, rotation, new Vector2(0.0f, 0.0f), SpriteEffects.None, layerDepth);
    }

    public static string getRandomNonLoopingSong()
    {
      switch (Game1.random.Next(7))
      {
        case 0:
          return "springsongs";
        case 1:
          return "summersongs";
        case 2:
          return "fallsongs";
        case 3:
          return "wintersongs";
        case 4:
          return "EarthMine";
        case 5:
          return "FrostMine";
        case 6:
          return "LavaMine";
        default:
          return "fallsongs";
      }
    }

    public static Farmer isThereAFarmerWithinDistance(
      Vector2 tileLocation,
      int tilesAway,
      GameLocation location)
    {
      foreach (Farmer farmer in location.farmers)
      {
        if ((double) Math.Abs(tileLocation.X - farmer.getTileLocation().X) <= (double) tilesAway && (double) Math.Abs(tileLocation.Y - farmer.getTileLocation().Y) <= (double) tilesAway)
          return farmer;
      }
      return (Farmer) null;
    }

    public static Character isThereAFarmerOrCharacterWithinDistance(
      Vector2 tileLocation,
      int tilesAway,
      GameLocation environment)
    {
      foreach (Character character in environment.characters)
      {
        if ((double) Vector2.Distance(character.getTileLocation(), tileLocation) <= (double) tilesAway)
          return character;
      }
      return (Character) Utility.isThereAFarmerWithinDistance(tileLocation, tilesAway, environment);
    }

    public static Microsoft.Xna.Framework.Color getRedToGreenLerpColor(float power) => new Microsoft.Xna.Framework.Color((double) power <= 0.5 ? (int) byte.MaxValue : (int) ((1.0 - (double) power) * 2.0 * (double) byte.MaxValue), (int) Math.Min((float) byte.MaxValue, (float) ((double) power * 2.0 * (double) byte.MaxValue)), 0);

    public static FarmHouse getHomeOfFarmer(Farmer who) => Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) who.homeLocation) as FarmHouse;

    public static Vector2 getRandomPositionOnScreen() => new Vector2((float) Game1.random.Next(Game1.viewport.Width), (float) Game1.random.Next(Game1.viewport.Height));

    public static Vector2 getRandomPositionOnScreenNotOnMap()
    {
      Vector2 vector2 = Vector2.Zero;
      int num;
      for (num = 0; num < 30 && (vector2.Equals(Vector2.Zero) || Game1.currentLocation.isTileOnMap((vector2 + new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y)) / 64f)); ++num)
        vector2 = Utility.getRandomPositionOnScreen();
      return num >= 30 ? new Vector2(-1000f, -1000f) : vector2;
    }

    public static Microsoft.Xna.Framework.Rectangle getRectangleCenteredAt(
      Vector2 v,
      int size)
    {
      return new Microsoft.Xna.Framework.Rectangle((int) v.X - size / 2, (int) v.Y - size / 2, size, size);
    }

    public static bool checkForCharacterInteractionAtTile(Vector2 tileLocation, Farmer who)
    {
      NPC npc = Game1.currentLocation.isCharacterAtTile(tileLocation);
      if (npc == null || npc.IsMonster || npc.IsInvisible)
        return false;
      if (Game1.currentLocation is MovieTheater)
        Game1.mouseCursor = 4;
      else if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !who.isRidingHorse() && npc.isVillager() && (who.friendshipData.ContainsKey(npc.Name) && who.friendshipData[npc.Name].GiftsToday != 1 || Game1.NPCGiftTastes.ContainsKey(npc.Name)) && !Game1.eventUp)
        Game1.mouseCursor = 3;
      else if (npc.canTalk() && (npc.CurrentDialogue != null && npc.CurrentDialogue.Count > 0 || Game1.player.spouse != null && npc.Name != null && npc.Name == Game1.player.spouse && npc.shouldSayMarriageDialogue.Value && npc.currentMarriageDialogue != null && npc.currentMarriageDialogue.Count > 0 || npc.hasTemporaryMessageAvailable() || who.hasClubCard && npc.Name.Equals("Bouncer") && who.IsLocalPlayer || npc.Name.Equals("Henchman") && npc.currentLocation.Name.Equals("WitchSwamp") && !who.hasOrWillReceiveMail("henchmanGone")) && !npc.isOnSilentTemporaryMessage())
        Game1.mouseCursor = 4;
      if (Game1.eventUp && Game1.CurrentEvent != null && !Game1.CurrentEvent.playerControlSequence)
        Game1.mouseCursor = 0;
      Game1.currentLocation.checkForSpecialCharacterIconAtThisTile(tileLocation);
      if (Game1.mouseCursor == 3 || Game1.mouseCursor == 4)
        Game1.mouseCursorTransparency = !Utility.tileWithinRadiusOfPlayer((int) tileLocation.X, (int) tileLocation.Y, 1, who) ? 0.5f : 1f;
      return true;
    }

    public static bool canGrabSomethingFromHere(int x, int y, Farmer who)
    {
      if (Game1.currentLocation == null)
        return false;
      Vector2 vector2 = new Vector2((float) (x / 64), (float) (y / 64));
      if (Game1.currentLocation.isObjectAt(x, y))
        Game1.currentLocation.getObjectAt(x, y).hoverAction();
      if (Utility.checkForCharacterInteractionAtTile(vector2, who) || Utility.checkForCharacterInteractionAtTile(vector2 + new Vector2(0.0f, 1f), who) || !who.IsLocalPlayer || who.onBridge.Value)
        return false;
      if (Game1.currentLocation != null)
      {
        foreach (Furniture furniture in Game1.currentLocation.furniture)
        {
          if (furniture.getBoundingBox(furniture.TileLocation).Contains(Utility.Vector2ToPoint(vector2 * 64f)) && furniture.Name.Contains("Table") && furniture.heldObject.Value != null)
            return true;
        }
      }
      if (Game1.currentLocation.Objects.ContainsKey(vector2))
      {
        if ((bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.Objects[vector2].readyForHarvest || (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.Objects[vector2].isSpawnedObject || Game1.currentLocation.Objects[vector2] is IndoorPot && (Game1.currentLocation.Objects[vector2] as IndoorPot).hoeDirt.Value.readyForHarvest())
        {
          Game1.mouseCursor = 6;
          if (Utility.withinRadiusOfPlayer(x, y, 1, who))
            return true;
          Game1.mouseCursorTransparency = 0.5f;
          return false;
        }
      }
      else if (Game1.currentLocation.terrainFeatures.ContainsKey(vector2) && Game1.currentLocation.terrainFeatures[vector2] is HoeDirt && ((HoeDirt) Game1.currentLocation.terrainFeatures[vector2]).readyForHarvest())
      {
        Game1.mouseCursor = 6;
        if (Utility.withinRadiusOfPlayer(x, y, 1, who))
          return true;
        Game1.mouseCursorTransparency = 0.5f;
        return false;
      }
      return false;
    }

    public static Microsoft.Xna.Framework.Rectangle getSourceRectWithinRectangularRegion(
      int regionX,
      int regionY,
      int regionWidth,
      int sourceIndex,
      int sourceWidth,
      int sourceHeight)
    {
      int num = regionWidth / sourceWidth;
      return new Microsoft.Xna.Framework.Rectangle(regionX + sourceIndex % num * sourceWidth, regionY + sourceIndex / num * sourceHeight, sourceWidth, sourceHeight);
    }

    public static void drawWithShadow(
      SpriteBatch b,
      Texture2D texture,
      Vector2 position,
      Microsoft.Xna.Framework.Rectangle sourceRect,
      Microsoft.Xna.Framework.Color color,
      float rotation,
      Vector2 origin,
      float scale = -1f,
      bool flipped = false,
      float layerDepth = -1f,
      int horizontalShadowOffset = -1,
      int verticalShadowOffset = -1,
      float shadowIntensity = 0.35f)
    {
      if ((double) scale == -1.0)
        scale = 4f;
      if ((double) layerDepth == -1.0)
        layerDepth = position.Y / 10000f;
      if (horizontalShadowOffset == -1)
        horizontalShadowOffset = -4;
      if (verticalShadowOffset == -1)
        verticalShadowOffset = 4;
      b.Draw(texture, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Microsoft.Xna.Framework.Color.Black * shadowIntensity, rotation, origin, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth - 0.0001f);
      b.Draw(texture, position, new Microsoft.Xna.Framework.Rectangle?(sourceRect), color, rotation, origin, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
    }

    public static void drawTextWithShadow(
      SpriteBatch b,
      StringBuilder text,
      SpriteFont font,
      Vector2 position,
      Microsoft.Xna.Framework.Color color,
      float scale = 1f,
      float layerDepth = -1f,
      int horizontalShadowOffset = -1,
      int verticalShadowOffset = -1,
      float shadowIntensity = 1f,
      int numShadows = 3)
    {
      if ((double) layerDepth == -1.0)
        layerDepth = position.Y / 10000f;
      bool flag = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de;
      if (horizontalShadowOffset == -1)
        horizontalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? -2 : -3;
      if (verticalShadowOffset == -1)
        verticalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? 2 : 3;
      if (text == null)
        throw new ArgumentNullException(nameof (text));
      b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), new Microsoft.Xna.Framework.Color(221, 148, 84) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
      if (numShadows == 2)
        b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, 0.0f), new Microsoft.Xna.Framework.Color(221, 148, 84) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
      if (numShadows == 3)
        b.DrawString(font, text, position + new Vector2(0.0f, (float) verticalShadowOffset), new Microsoft.Xna.Framework.Color(221, 148, 84) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
      b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    }

    public static void drawTextWithShadow(
      SpriteBatch b,
      string text,
      SpriteFont font,
      Vector2 position,
      Microsoft.Xna.Framework.Color color,
      float scale = 1f,
      float layerDepth = -1f,
      int horizontalShadowOffset = -1,
      int verticalShadowOffset = -1,
      float shadowIntensity = 1f,
      int numShadows = 3)
    {
      if ((double) layerDepth == -1.0)
        layerDepth = position.Y / 10000f;
      bool flag = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ko;
      if (horizontalShadowOffset == -1)
        horizontalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? -2 : -3;
      if (verticalShadowOffset == -1)
        verticalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? 2 : 3;
      if (text == null)
        text = "";
      b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), new Microsoft.Xna.Framework.Color(221, 148, 84) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
      if (numShadows == 2)
        b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, 0.0f), new Microsoft.Xna.Framework.Color(221, 148, 84) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
      if (numShadows == 3)
        b.DrawString(font, text, position + new Vector2(0.0f, (float) verticalShadowOffset), new Microsoft.Xna.Framework.Color(221, 148, 84) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
      b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    }

    public static void drawTextWithColoredShadow(
      SpriteBatch b,
      string text,
      SpriteFont font,
      Vector2 position,
      Microsoft.Xna.Framework.Color color,
      Microsoft.Xna.Framework.Color shadowColor,
      float scale = 1f,
      float layerDepth = -1f,
      int horizontalShadowOffset = -1,
      int verticalShadowOffset = -1,
      int numShadows = 3)
    {
      if ((double) layerDepth == -1.0)
        layerDepth = position.Y / 10000f;
      bool flag = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de;
      if (horizontalShadowOffset == -1)
        horizontalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? -2 : -3;
      if (verticalShadowOffset == -1)
        verticalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? 2 : 3;
      if (text == null)
        text = "";
      b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), shadowColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
      if (numShadows == 2)
        b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, 0.0f), shadowColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
      if (numShadows == 3)
        b.DrawString(font, text, position + new Vector2(0.0f, (float) verticalShadowOffset), shadowColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
      b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    }

    public static void drawBoldText(
      SpriteBatch b,
      string text,
      SpriteFont font,
      Vector2 position,
      Microsoft.Xna.Framework.Color color,
      float scale = 1f,
      float layerDepth = -1f,
      int boldnessOffset = 1)
    {
      if ((double) layerDepth == -1.0)
        layerDepth = position.Y / 10000f;
      b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
      b.DrawString(font, text, position + new Vector2((float) boldnessOffset, 0.0f), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
      b.DrawString(font, text, position + new Vector2((float) boldnessOffset, (float) boldnessOffset), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
      b.DrawString(font, text, position + new Vector2(0.0f, (float) boldnessOffset), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    }

    protected static bool _HasNonMousePlacementLeeway(int x, int y, Item item, Farmer f)
    {
      if (!Game1.isCheckingNonMousePlacement)
        return false;
      Point tileLocationPoint = f.getTileLocationPoint();
      if (!Utility.withinRadiusOfPlayer(x, y, 2, f))
        return false;
      if (item is Object && item.Category == -74)
        return true;
      foreach (Point point in Utility.GetPointsOnLine(tileLocationPoint.X, tileLocationPoint.Y, x / 64, y / 64))
      {
        if (!(point == tileLocationPoint) && !item.canBePlacedHere(f.currentLocation, new Vector2((float) point.X, (float) point.Y)))
          return false;
      }
      return true;
    }

    public static bool isPlacementForbiddenHere(GameLocation location) => location == null || Utility.isPlacementForbiddenHere((string) (NetFieldBase<string, NetString>) location.name);

    public static bool isPlacementForbiddenHere(string location_name) => location_name == "AbandonedJojaMart" || location_name == "BeachNightMarket";

    public static void transferPlacedObjectsFromOneLocationToAnother(
      GameLocation source,
      GameLocation destination,
      Vector2? overflow_chest_position = null,
      GameLocation overflow_chest_location = null)
    {
      if (source == null)
        return;
      List<Item> overflow_items = new List<Item>();
      foreach (Vector2 vector2 in new List<Vector2>((IEnumerable<Vector2>) source.objects.Keys))
      {
        if (source.objects[vector2] != null)
        {
          Object @object = source.objects[vector2];
          bool flag = true;
          if (destination == null)
            flag = false;
          if (flag && destination.objects.ContainsKey(vector2))
            flag = false;
          if (flag && !destination.isTileLocationTotallyClearAndPlaceable(vector2))
            flag = false;
          source.objects.Remove(vector2);
          if (flag && destination != null)
          {
            destination.objects[vector2] = @object;
          }
          else
          {
            overflow_items.Add((Item) @object);
            if (@object is Chest)
            {
              Chest chest = @object as Chest;
              List<Item> objList = new List<Item>((IEnumerable<Item>) chest.items);
              chest.items.Clear();
              foreach (Item obj in objList)
              {
                if (obj != null)
                  overflow_items.Add(obj);
              }
            }
          }
        }
      }
      if (!overflow_chest_position.HasValue)
        return;
      if (overflow_chest_location != null)
      {
        Utility.createOverflowChest(overflow_chest_location, overflow_chest_position.Value, overflow_items);
      }
      else
      {
        if (destination == null)
          return;
        Utility.createOverflowChest(destination, overflow_chest_position.Value, overflow_items);
      }
    }

    public static void createOverflowChest(
      GameLocation destination,
      Vector2 overflow_chest_location,
      List<Item> overflow_items)
    {
      List<Chest> chestList = new List<Chest>();
      foreach (Item overflowItem in overflow_items)
      {
        if (chestList.Count == 0)
          chestList.Add(new Chest(true));
        bool flag = false;
        foreach (Chest chest in chestList)
        {
          if (chest.addItem(overflowItem) == null)
            flag = true;
        }
        if (!flag)
        {
          Chest chest = new Chest(true);
          chest.addItem(overflowItem);
          chestList.Add(chest);
        }
      }
      for (int index = 0; index < chestList.Count; ++index)
      {
        Chest o = chestList[index];
        Vector2 tileLocation = overflow_chest_location;
        Utility._placeOverflowChestInNearbySpace(destination, tileLocation, (Object) o);
      }
    }

    protected static void _placeOverflowChestInNearbySpace(
      GameLocation location,
      Vector2 tileLocation,
      Object o)
    {
      if (o == null || tileLocation.Equals(Vector2.Zero))
        return;
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      HashSet<Vector2> vector2Set = new HashSet<Vector2>();
      vector2Queue.Enqueue(tileLocation);
      Vector2 vector2 = Vector2.Zero;
      for (; num < 100; ++num)
      {
        vector2 = vector2Queue.Dequeue();
        if (location.isTileOccupiedForPlacement(vector2) || !location.isTileLocationTotallyClearAndPlaceable(vector2) || location.isOpenWater((int) vector2.X, (int) vector2.Y))
        {
          vector2Set.Add(vector2);
          foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
          {
            if (!vector2Set.Contains(adjacentTileLocation))
              vector2Queue.Enqueue(adjacentTileLocation);
          }
        }
        else
          break;
      }
      if (vector2.Equals(Vector2.Zero) || location.isTileOccupiedForPlacement(vector2) || location.isOpenWater((int) vector2.X, (int) vector2.Y) || !location.isTileLocationTotallyClearAndPlaceable(vector2))
        return;
      o.tileLocation.Value = vector2;
      location.objects.Add(vector2, o);
    }

    public static bool isWithinTileWithLeeway(int x, int y, Item item, Farmer f) => Utility.withinRadiusOfPlayer(x, y, 1, f) || Utility._HasNonMousePlacementLeeway(x, y, item, f);

    public static bool playerCanPlaceItemHere(
      GameLocation location,
      Item item,
      int x,
      int y,
      Farmer f)
    {
      if (Utility.isPlacementForbiddenHere(location) || item == null || item is Tool || Game1.eventUp || (bool) (NetFieldBase<bool, NetBool>) f.bathingClothes || f.onBridge.Value || !Utility.isWithinTileWithLeeway(x, y, item, f) && (!(item is Wallpaper) || !(location is DecoratableLocation)) && (!(item is Furniture) || !location.CanPlaceThisFurnitureHere(item as Furniture)))
        return false;
      if (item is Furniture)
      {
        Furniture furniture = item as Furniture;
        if (!location.CanFreePlaceFurniture() && !furniture.IsCloseEnoughToFarmer(f, new int?(x / 64), new int?(y / 64)))
          return false;
      }
      Vector2 vector2 = new Vector2((float) (x / 64), (float) (y / 64));
      Object objectAtTile = location.getObjectAtTile((int) vector2.X, (int) vector2.Y);
      if (objectAtTile != null && objectAtTile is Fence && (objectAtTile as Fence).CanRepairWithThisItem(item))
        return true;
      if (item.canBePlacedHere(location, vector2))
      {
        if (item is Wallpaper)
          return true;
        if (!((Object) item).isPassable())
        {
          foreach (Character farmer in location.farmers)
          {
            if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int) vector2.X * 64, (int) vector2.Y * 64, 64, 64)))
              return false;
          }
        }
        if (Utility.itemCanBePlaced(location, vector2, item) || Utility.isViableSeedSpot(location, vector2, item))
          return true;
      }
      return false;
    }

    public static int GetDoubleWideVersionOfBed(int bed_index) => bed_index + 4;

    private static bool itemCanBePlaced(GameLocation location, Vector2 tileLocation, Item item)
    {
      if (!location.isTilePlaceable(tileLocation, item) || !item.isPlaceable() || item.Category == -74 && (!(item is Object) || !(item as Object).isSapling()))
        return false;
      return ((Object) item).isPassable() || !new Microsoft.Xna.Framework.Rectangle((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.Y * 64.0), 64, 64).Intersects(Game1.player.GetBoundingBox());
    }

    public static bool isViableSeedSpot(GameLocation location, Vector2 tileLocation, Item item)
    {
      if (item.Category != -74)
        return false;
      if (location.terrainFeatures.ContainsKey(tileLocation) && location.terrainFeatures[tileLocation] is HoeDirt && ((HoeDirt) location.terrainFeatures[tileLocation]).canPlantThisSeedHere((item as Object).ParentSheetIndex, (int) tileLocation.X, (int) tileLocation.Y) || location.objects.ContainsKey(tileLocation) && location.objects[tileLocation] is IndoorPot && (location.objects[tileLocation] as IndoorPot).hoeDirt.Value.canPlantThisSeedHere((item as Object).ParentSheetIndex, (int) tileLocation.X, (int) tileLocation.Y) && (item as Object).ParentSheetIndex != 499)
        return true;
      return (location.isTileHoeDirt(tileLocation) || !location.terrainFeatures.ContainsKey(tileLocation)) && Object.isWildTreeSeed((int) (NetFieldBase<int, NetInt>) item.parentSheetIndex);
    }

    public static int getDirectionFromChange(Vector2 current, Vector2 previous, bool yBias = false)
    {
      if (!yBias && (double) current.X > (double) previous.X)
        return 1;
      if (!yBias && (double) current.X < (double) previous.X)
        return 3;
      if ((double) current.Y > (double) previous.Y)
        return 2;
      if ((double) current.Y < (double) previous.Y)
        return 0;
      if ((double) current.X > (double) previous.X)
        return 1;
      return (double) current.X < (double) previous.X ? 3 : -1;
    }

    public static bool doesRectangleIntersectTile(Microsoft.Xna.Framework.Rectangle r, int tileX, int tileY)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileX * 64, tileY * 64, 64, 64);
      return r.Intersects(rectangle);
    }

    public static List<NPC> getPooledList()
    {
      lock (Utility._pool)
        return Utility._pool.Get();
    }

    public static bool IsHospitalVisitDay(string character_name)
    {
      try
      {
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + character_name);
        string key = Game1.currentSeason + "_" + Game1.dayOfMonth.ToString();
        if (dictionary.ContainsKey(key))
        {
          if (dictionary[key].Contains("Hospital"))
            return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static void returnPooledList(List<NPC> list)
    {
      lock (Utility._pool)
        Utility._pool.Return(list);
    }

    public static List<NPC> getAllCharacters(List<NPC> list)
    {
      list.AddRange((IEnumerable<NPC>) Game1.currentLocation.characters);
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (!location.Equals(Game1.currentLocation))
          list.AddRange((IEnumerable<NPC>) location.characters);
      }
      Farm farm = Game1.getFarm();
      if (farm != null)
      {
        foreach (Building building in farm.buildings)
        {
          if (building.indoors.Value != null)
          {
            foreach (Character character in building.indoors.Value.characters)
              character.currentLocation = (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors;
            list.AddRange((IEnumerable<NPC>) building.indoors.Value.characters);
          }
        }
      }
      return list;
    }

    public static DisposableList<NPC> getAllCharacters()
    {
      List<NPC> list;
      lock (Utility._pool)
        list = Utility._pool.Get();
      Utility.getAllCharacters(list);
      return new DisposableList<NPC>(list, Utility._pool);
    }

    private static void _recursiveIterateItem(Item i, Action<Item> action)
    {
      if (i == null)
        return;
      if (i is Object)
      {
        Object @object = i as Object;
        if (@object is StorageFurniture)
        {
          foreach (Item heldItem in (NetList<Item, NetRef<Item>>) (@object as StorageFurniture).heldItems)
          {
            if (heldItem != null)
              Utility._recursiveIterateItem(heldItem, action);
          }
        }
        if (@object is Chest)
        {
          foreach (Item i1 in (NetList<Item, NetRef<Item>>) (@object as Chest).items)
          {
            if (i1 != null)
              Utility._recursiveIterateItem(i1, action);
          }
        }
        if (@object.heldObject.Value != null)
          Utility._recursiveIterateItem((Item) (Object) (NetFieldBase<Object, NetRef<Object>>) @object.heldObject, action);
      }
      action(i);
    }

    protected static void _recursiveIterateLocation(GameLocation l, Action<Item> action)
    {
      if (l == null)
        return;
      if (l != null)
      {
        foreach (Item i in l.furniture)
          Utility._recursiveIterateItem(i, action);
      }
      if (l is IslandFarmHouse)
      {
        foreach (Item i in (NetList<Item, NetRef<Item>>) (l as IslandFarmHouse).fridge.Value.items)
        {
          if (i != null)
            Utility._recursiveIterateItem(i, action);
        }
      }
      if (l is FarmHouse)
      {
        foreach (Item i in (NetList<Item, NetRef<Item>>) (l as FarmHouse).fridge.Value.items)
        {
          if (i != null)
            Utility._recursiveIterateItem(i, action);
        }
      }
      foreach (Character character in l.characters)
      {
        if (character is Child && (character as Child).hat.Value != null)
          Utility._recursiveIterateItem((Item) (character as Child).hat.Value, action);
        if (character is Horse && (character as Horse).hat.Value != null)
          Utility._recursiveIterateItem((Item) (character as Horse).hat.Value, action);
      }
      if (l is BuildableGameLocation)
      {
        foreach (Building building in (l as BuildableGameLocation).buildings)
        {
          if (building.indoors.Value != null)
            Utility._recursiveIterateLocation(building.indoors.Value, action);
          if (building is Mill)
          {
            foreach (Item i in (NetList<Item, NetRef<Item>>) (building as Mill).output.Value.items)
            {
              if (i != null)
                Utility._recursiveIterateItem(i, action);
            }
          }
          else if (building is JunimoHut)
          {
            foreach (Item i in (NetList<Item, NetRef<Item>>) (building as JunimoHut).output.Value.items)
            {
              if (i != null)
                Utility._recursiveIterateItem(i, action);
            }
          }
        }
      }
      foreach (Item i in l.objects.Values)
        Utility._recursiveIterateItem(i, action);
      foreach (Debris debri in l.debris)
      {
        if (debri.item != null)
          Utility._recursiveIterateItem(debri.item, action);
      }
    }

    public static Item PerformSpecialItemPlaceReplacement(Item placedItem)
    {
      if (placedItem != null && placedItem is Pan)
        return (Item) new StardewValley.Objects.Hat(71);
      return placedItem != null && placedItem is Object && (int) (NetFieldBase<int, NetInt>) (placedItem as Object).parentSheetIndex == 71 ? (Item) new Clothing(15) : placedItem;
    }

    public static Item PerformSpecialItemGrabReplacement(Item heldItem)
    {
      if (heldItem != null && heldItem is Clothing && (int) (NetFieldBase<int, NetInt>) (heldItem as Clothing).parentSheetIndex == 15)
      {
        heldItem = (Item) new Object(71, 1);
        Object @object = heldItem as Object;
        @object.questItem.Value = true;
        @object.questId.Value = 102;
      }
      if (heldItem != null && heldItem is StardewValley.Objects.Hat && (int) (NetFieldBase<int, NetInt>) (heldItem as StardewValley.Objects.Hat).which == 71)
        heldItem = (Item) new Pan();
      return heldItem;
    }

    public static void iterateAllItemsHere(GameLocation location, Action<Item> action) => Utility._recursiveIterateLocation(location, action);

    public static void iterateAllItems(Action<Item> action)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        Utility._recursiveIterateLocation(location, action);
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (Item i in (IEnumerable<Item>) allFarmer.Items)
          Utility._recursiveIterateItem(i, action);
        Utility._recursiveIterateItem((Item) allFarmer.shirtItem.Value, action);
        Utility._recursiveIterateItem((Item) allFarmer.pantsItem.Value, action);
        Utility._recursiveIterateItem((Item) allFarmer.boots.Value, action);
        Utility._recursiveIterateItem((Item) allFarmer.hat.Value, action);
        Utility._recursiveIterateItem((Item) allFarmer.leftRing.Value, action);
        Utility._recursiveIterateItem((Item) allFarmer.rightRing.Value, action);
        foreach (Item i in (NetList<Item, NetRef<Item>>) allFarmer.itemsLostLastDeath)
          Utility._recursiveIterateItem(i, action);
        if (allFarmer.recoveredItem != null)
          Utility._recursiveIterateItem(allFarmer.recoveredItem, action);
      }
      foreach (Item returnedDonation in Game1.player.team.returnedDonations)
      {
        if (returnedDonation != null)
          action(returnedDonation);
      }
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Game1.player.team.junimoChest)
      {
        if (obj != null)
          action(obj);
      }
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        foreach (Item donatedItem in specialOrder.donatedItems)
        {
          if (donatedItem != null)
            action(donatedItem);
        }
      }
    }

    public static void iterateChestsAndStorage(Action<Item> action)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        foreach (Object @object in location.objects.Values)
        {
          if (@object is Chest)
          {
            foreach (Item obj in (NetList<Item, NetRef<Item>>) (@object as Chest).items)
            {
              if (obj != null)
                action(obj);
            }
          }
          if (@object.heldObject.Value != null && @object.heldObject.Value is Chest)
          {
            foreach (Item obj in (NetList<Item, NetRef<Item>>) (@object.heldObject.Value as Chest).items)
            {
              if (obj != null)
                action(obj);
            }
          }
        }
        if (location is FarmHouse)
        {
          foreach (Item obj in (NetList<Item, NetRef<Item>>) (location as FarmHouse).fridge.Value.items)
          {
            if (obj != null)
              action(obj);
          }
        }
        else if (location is IslandFarmHouse)
        {
          foreach (Item obj in (NetList<Item, NetRef<Item>>) (location as IslandFarmHouse).fridge.Value.items)
          {
            if (obj != null)
              action(obj);
          }
        }
        if (location != null)
        {
          foreach (Furniture furniture in location.furniture)
          {
            if (furniture is StorageFurniture)
            {
              foreach (Item heldItem in (NetList<Item, NetRef<Item>>) (furniture as StorageFurniture).heldItems)
              {
                if (heldItem != null)
                  action(heldItem);
              }
            }
          }
        }
        if (location is BuildableGameLocation)
        {
          foreach (Building building in (location as BuildableGameLocation).buildings)
          {
            if (building.indoors.Value != null)
            {
              foreach (Object @object in building.indoors.Value.objects.Values)
              {
                if (@object is Chest)
                {
                  foreach (Item obj in (NetList<Item, NetRef<Item>>) (@object as Chest).items)
                  {
                    if (obj != null)
                      action(obj);
                  }
                }
                if (@object.heldObject.Value != null && @object.heldObject.Value is Chest)
                {
                  foreach (Item obj in (NetList<Item, NetRef<Item>>) (@object.heldObject.Value as Chest).items)
                  {
                    if (obj != null)
                      action(obj);
                  }
                }
              }
              if (building.indoors.Value != null)
              {
                foreach (Furniture furniture in building.indoors.Value.furniture)
                {
                  if (furniture is StorageFurniture)
                  {
                    foreach (Item heldItem in (NetList<Item, NetRef<Item>>) (furniture as StorageFurniture).heldItems)
                    {
                      if (heldItem != null)
                        action(heldItem);
                    }
                  }
                }
              }
            }
            else if (building is Mill)
            {
              foreach (Item obj in (NetList<Item, NetRef<Item>>) (building as Mill).output.Value.items)
              {
                if (obj != null)
                  action(obj);
              }
            }
            else if (building is JunimoHut)
            {
              foreach (Item obj in (NetList<Item, NetRef<Item>>) (building as JunimoHut).output.Value.items)
              {
                if (obj != null)
                  action(obj);
              }
            }
          }
        }
      }
      foreach (Item returnedDonation in Game1.player.team.returnedDonations)
      {
        if (returnedDonation != null)
          action(returnedDonation);
      }
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Game1.player.team.junimoChest)
      {
        if (obj != null)
          action(obj);
      }
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        foreach (Item donatedItem in specialOrder.donatedItems)
        {
          if (donatedItem != null)
            action(donatedItem);
        }
      }
    }

    public static Item removeItemFromInventory(int whichItemIndex, IList<Item> items)
    {
      if (whichItemIndex < 0 || whichItemIndex >= items.Count || items[whichItemIndex] == null)
        return (Item) null;
      Item obj = items[whichItemIndex];
      if (whichItemIndex == Game1.player.CurrentToolIndex && items.Equals((object) Game1.player.items) && obj != null)
        obj.actionWhenStopBeingHeld(Game1.player);
      items[whichItemIndex] = (Item) null;
      return obj;
    }

    public static void iterateAllCrops(Action<Crop> action)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        Utility._recursiveIterateLocationCrops(location, action);
    }

    protected static void _recursiveIterateLocationCrops(GameLocation l, Action<Crop> action)
    {
      if (l == null)
        return;
      if (l is BuildableGameLocation)
      {
        foreach (Building building in (l as BuildableGameLocation).buildings)
        {
          if (building.indoors.Value != null)
            Utility._recursiveIterateLocationCrops(building.indoors.Value, action);
        }
      }
      foreach (TerrainFeature terrainFeature in l.terrainFeatures.Values)
      {
        if (terrainFeature is HoeDirt && (terrainFeature as HoeDirt).crop != null)
          action((terrainFeature as HoeDirt).crop);
      }
      foreach (Object @object in l.objects.Values)
      {
        if (@object is IndoorPot && (@object as IndoorPot).hoeDirt.Value != null && (@object as IndoorPot).hoeDirt.Value.crop != null)
          action((@object as IndoorPot).hoeDirt.Value.crop);
      }
    }

    public static void checkItemFirstInventoryAdd(Item item)
    {
      if (!(item is Object) || item.HasBeenInInventory)
        return;
      if (!(item is Furniture) && !(bool) (NetFieldBase<bool, NetBool>) (item as Object).bigCraftable && !(bool) (NetFieldBase<bool, NetBool>) (item as Object).hasBeenPickedUpByFarmer)
        Game1.player.checkForQuestComplete((NPC) null, (int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex, (int) (NetFieldBase<int, NetInt>) (item as Object).stack, item, (string) null, 9);
      if (Game1.player.team.specialOrders != null)
      {
        foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
        {
          if (specialOrder.onItemCollected != null)
            specialOrder.onItemCollected(Game1.player, item);
        }
      }
      item.HasBeenInInventory = true;
      (item as Object).hasBeenPickedUpByFarmer.Value = true;
      if ((bool) (NetFieldBase<bool, NetBool>) (item as Object).questItem)
      {
        if (Utility.IsNormalObjectAtParentSheetIndex(item, 875) && !Game1.MasterPlayer.hasOrWillReceiveMail("ectoplasmDrop") && Game1.player.team.SpecialOrderActive("Wizard"))
          Game1.addMailForTomorrow("ectoplasmDrop", true, true);
        else if (Utility.IsNormalObjectAtParentSheetIndex(item, 876) && !Game1.MasterPlayer.hasOrWillReceiveMail("prismaticJellyDrop") && Game1.player.team.SpecialOrderActive("Wizard2"))
          Game1.addMailForTomorrow("prismaticJellyDrop", true, true);
        if (!Utility.IsNormalObjectAtParentSheetIndex(item, 897) || Game1.MasterPlayer.hasOrWillReceiveMail("gotMissingStocklist"))
          return;
        Game1.addMailForTomorrow("gotMissingStocklist", true, true);
      }
      else
      {
        if (item is Object && (item as Object).bigCraftable.Value && item.ParentSheetIndex == 256 && !Game1.MasterPlayer.hasOrWillReceiveMail("gotFirstJunimoChest"))
          Game1.addMailForTomorrow("gotFirstJunimoChest", true, true);
        if (Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex))
        {
          switch ((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex)
          {
            case 72:
              ++Game1.stats.DiamondsFound;
              break;
            case 74:
              ++Game1.stats.PrismaticShardsFound;
              break;
            case 102:
              ++Game1.stats.NotesFound;
              break;
            case 378:
              if (!Game1.player.hasOrWillReceiveMail("copperFound"))
              {
                Game1.addMailForTomorrow("copperFound", true);
                break;
              }
              break;
            case 390:
              ++Game1.stats.StoneGathered;
              if (Game1.stats.StoneGathered >= 100U && !Game1.player.hasOrWillReceiveMail("robinWell"))
              {
                Game1.addMailForTomorrow("robinWell");
                break;
              }
              break;
            case 428:
              if (!Game1.player.hasOrWillReceiveMail("clothFound"))
              {
                Game1.addMailForTomorrow("clothFound", true);
                break;
              }
              break;
            case 535:
              if (Game1.activeClickableMenu == null && !Game1.player.hasOrWillReceiveMail("geodeFound"))
              {
                Game1.player.mailReceived.Add("geodeFound");
                Game1.player.holdUpItemThenMessage(item);
                break;
              }
              break;
          }
        }
        else if (item is Object && (item as Object).bigCraftable.Value && (item as Object).ParentSheetIndex == 248)
          ++Game1.netWorldState.Value.MiniShippingBinsObtained.Value;
        if (!(item is Object))
          return;
        Game1.player.checkForQuestComplete((NPC) null, (int) (NetFieldBase<int, NetInt>) item.parentSheetIndex, item.Stack, item, "", 10);
      }
    }

    public static NPC getRandomTownNPC() => Utility.getRandomTownNPC(Game1.random);

    public static NPC getRandomTownNPC(Random r)
    {
      Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      int index = r.Next(source.Count);
      NPC characterFromName;
      for (characterFromName = Game1.getCharacterFromName(source.ElementAt<KeyValuePair<string, string>>(index).Key); source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals("Wizard") || source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals("Krobus") || source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals("Sandy") || source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals("Dwarf") || source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals("Marlon") || source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals("Leo") && !Game1.MasterPlayer.mailReceived.Contains("addedParrotBoy") || characterFromName == null; characterFromName = Game1.getCharacterFromName(source.ElementAt<KeyValuePair<string, string>>(index).Key))
        index = r.Next(source.Count);
      return characterFromName;
    }

    public static NPC getTownNPCByGiftTasteIndex(int index)
    {
      Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      NPC characterFromName = Game1.getCharacterFromName(source.ElementAt<KeyValuePair<string, string>>(index).Key);
      int index1 = (index += 10) % 25;
      while (characterFromName == null)
      {
        characterFromName = Game1.getCharacterFromName(source.ElementAt<KeyValuePair<string, string>>(index1).Key);
        index1 = (index1 + 1) % 30;
      }
      return characterFromName;
    }

    public static bool foundAllStardrops(Farmer who = null)
    {
      if (who == null)
        who = Game1.player;
      if (who.mailReceived.Contains("gotMaxStamina"))
        return true;
      return who.hasOrWillReceiveMail("CF_Fair") && who.hasOrWillReceiveMail("CF_Fish") && who.hasOrWillReceiveMail("CF_Mines") && who.hasOrWillReceiveMail("CF_Sewer") && who.hasOrWillReceiveMail("museumComplete") && who.hasOrWillReceiveMail("CF_Spouse") && who.hasOrWillReceiveMail("CF_Statue");
    }

    /// <summary>
    /// Can range from 0 to 21.
    /// 
    ///    if (points &gt;= 12) 4
    ///     if (points &gt;= 8) 3
    ///   if (points &gt;= 4)  2
    ///    else 1
    /// those are the number of candles that will be light on grandpa's shrine.
    /// </summary>
    /// <returns></returns>
    public static int getGrandpaScore()
    {
      int grandpaScore = 0;
      if (Game1.player.totalMoneyEarned >= 50000U)
        ++grandpaScore;
      if (Game1.player.totalMoneyEarned >= 100000U)
        ++grandpaScore;
      if (Game1.player.totalMoneyEarned >= 200000U)
        ++grandpaScore;
      if (Game1.player.totalMoneyEarned >= 300000U)
        ++grandpaScore;
      if (Game1.player.totalMoneyEarned >= 500000U)
        ++grandpaScore;
      if (Game1.player.totalMoneyEarned >= 1000000U)
        grandpaScore += 2;
      if (Game1.player.achievements.Contains(5))
        ++grandpaScore;
      if (Game1.player.hasSkullKey)
        ++grandpaScore;
      int num = Game1.isLocationAccessible("CommunityCenter") ? 1 : 0;
      if (num != 0 || Game1.player.hasCompletedCommunityCenter())
        ++grandpaScore;
      if (num != 0)
        grandpaScore += 2;
      if (Game1.player.isMarried() && Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 2)
        ++grandpaScore;
      if (Game1.player.hasRustyKey)
        ++grandpaScore;
      if (Game1.player.achievements.Contains(26))
        ++grandpaScore;
      if (Game1.player.achievements.Contains(34))
        ++grandpaScore;
      int friendsWithinThisRange = Utility.getNumberOfFriendsWithinThisRange(Game1.player, 1975, 999999);
      if (friendsWithinThisRange >= 5)
        ++grandpaScore;
      if (friendsWithinThisRange >= 10)
        ++grandpaScore;
      int level = Game1.player.Level;
      if (level >= 15)
        ++grandpaScore;
      if (level >= 25)
        ++grandpaScore;
      string petName = Game1.player.getPetName();
      if (petName != null)
      {
        Pet characterFromName = Game1.getCharacterFromName<Pet>(petName, false);
        if (characterFromName != null && (int) (NetFieldBase<int, NetInt>) characterFromName.friendshipTowardFarmer >= 999)
          ++grandpaScore;
      }
      return grandpaScore;
    }

    public static int getGrandpaCandlesFromScore(int score)
    {
      if (score >= 12)
        return 4;
      if (score >= 8)
        return 3;
      return score >= 4 ? 2 : 1;
    }

    public static bool canItemBeAddedToThisInventoryList(
      Item i,
      IList<Item> list,
      int listMaxSpace = -1)
    {
      if (listMaxSpace != -1 && list.Count < listMaxSpace)
        return true;
      int stack = i.Stack;
      foreach (Item obj in (IEnumerable<Item>) list)
      {
        if (obj == null)
          return true;
        if (obj.canStackWith((ISalable) i) && obj.getRemainingStackSpace() > 0)
        {
          stack -= obj.getRemainingStackSpace();
          if (stack <= 0)
            return true;
        }
      }
      return false;
    }

    public static Item addItemToThisInventoryList(Item i, IList<Item> list, int listMaxSpace = -1)
    {
      if (i.Stack == 0)
        i.Stack = 1;
      foreach (Item obj in (IEnumerable<Item>) list)
      {
        if (obj != null && obj.canStackWith((ISalable) i) && obj.getRemainingStackSpace() > 0)
        {
          if (i is Object)
            (i as Object).stack.Value = obj.addToStack(i);
          else
            i.Stack = obj.addToStack(i);
          if (i.Stack <= 0)
            return (Item) null;
        }
      }
      for (int index = list.Count - 1; index >= 0; --index)
      {
        if (list[index] == null)
        {
          if (i.Stack > i.maximumStackSize())
          {
            list[index] = i.getOne();
            list[index].Stack = i.maximumStackSize();
            if (i is Object)
              (i as Object).stack.Value -= i.maximumStackSize();
            else
              i.Stack -= i.maximumStackSize();
          }
          else
          {
            list[index] = i;
            return (Item) null;
          }
        }
      }
      while (listMaxSpace != -1 && list.Count < listMaxSpace)
      {
        if (i.Stack > i.maximumStackSize())
        {
          Item one = i.getOne();
          one.Stack = i.maximumStackSize();
          if (i is Object)
            (i as Object).stack.Value -= i.maximumStackSize();
          else
            i.Stack -= i.maximumStackSize();
          list.Add(one);
        }
        else
        {
          list.Add(i);
          return (Item) null;
        }
      }
      return i;
    }

    public static Item addItemToInventory(
      Item item,
      int position,
      IList<Item> items,
      ItemGrabMenu.behaviorOnItemSelect onAddFunction = null)
    {
      if (items.Equals((object) Game1.player.items) && item is Object && (item as Object).specialItem)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) (item as Object).bigCraftable)
        {
          if (!Game1.player.specialBigCraftables.Contains((bool) (NetFieldBase<bool, NetBool>) (item as Object).isRecipe ? -(int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex : (int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex))
            Game1.player.specialBigCraftables.Add((bool) (NetFieldBase<bool, NetBool>) (item as Object).isRecipe ? -(int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex : (int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
        }
        else if (!Game1.player.specialItems.Contains((bool) (NetFieldBase<bool, NetBool>) (item as Object).isRecipe ? -(int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex : (int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex))
          Game1.player.specialItems.Add((bool) (NetFieldBase<bool, NetBool>) (item as Object).isRecipe ? -(int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex : (int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
      }
      if (position < 0 || position >= items.Count)
        return item;
      if (items[position] == null)
      {
        items[position] = item;
        Utility.checkItemFirstInventoryAdd(item);
        if (onAddFunction != null)
          onAddFunction(item, (Farmer) null);
        return (Item) null;
      }
      if (items[position].maximumStackSize() != -1 && items[position].Name.Equals(item.Name) && (!(item is Object) || !(items[position] is Object) || (NetFieldBase<int, NetInt>) (item as Object).quality == (items[position] as Object).quality && (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex == (items[position] as Object).parentSheetIndex) && item.canStackWith((ISalable) items[position]))
      {
        Utility.checkItemFirstInventoryAdd(item);
        int stack = items[position].addToStack(item);
        if (stack <= 0)
          return (Item) null;
        item.Stack = stack;
        if (onAddFunction != null)
          onAddFunction(item, (Farmer) null);
        return item;
      }
      Item inventory = items[position];
      if (position == Game1.player.CurrentToolIndex && items.Equals((object) Game1.player.items) && inventory != null)
      {
        inventory.actionWhenStopBeingHeld(Game1.player);
        item.actionWhenBeingHeld(Game1.player);
      }
      Utility.checkItemFirstInventoryAdd(item);
      items[position] = item;
      if (onAddFunction != null)
        onAddFunction(item, (Farmer) null);
      return inventory;
    }

    public static bool spawnObjectAround(
      Vector2 tileLocation,
      Object o,
      GameLocation l,
      bool playSound = true,
      Action<Object> modifyObject = null)
    {
      if (o == null || l == null || tileLocation.Equals(Vector2.Zero))
        return false;
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      HashSet<Vector2> vector2Set = new HashSet<Vector2>();
      vector2Queue.Enqueue(tileLocation);
      Vector2 vector2_1 = Vector2.Zero;
      for (; num < 100; ++num)
      {
        vector2_1 = vector2Queue.Dequeue();
        if (l.isTileOccupiedForPlacement(vector2_1) || l.isOpenWater((int) vector2_1.X, (int) vector2_1.Y))
        {
          vector2Set.Add(vector2_1);
          foreach (Vector2 vector2_2 in Utility.getAdjacentTileLocations(vector2_1).OrderBy<Vector2, Guid>((Func<Vector2, Guid>) (a => Guid.NewGuid())).ToList<Vector2>())
          {
            if (!vector2Set.Contains(vector2_2))
              vector2Queue.Enqueue(vector2_2);
          }
        }
        else
          break;
      }
      o.isSpawnedObject.Value = true;
      o.canBeGrabbed.Value = true;
      o.tileLocation.Value = vector2_1;
      if (modifyObject != null)
        modifyObject(o);
      if (vector2_1.Equals(Vector2.Zero) || l.isTileOccupiedForPlacement(vector2_1) || l.isOpenWater((int) vector2_1.X, (int) vector2_1.Y))
        return false;
      l.objects.Add(vector2_1, o);
      if (playSound)
        l.playSound("coin");
      if (l.Equals(Game1.currentLocation))
        l.temporarySprites.Add(new TemporaryAnimatedSprite(5, vector2_1 * 64f, Microsoft.Xna.Framework.Color.White));
      return true;
    }

    public static bool IsGeode(Item item, bool disallow_special_geodes = false)
    {
      if (item == null || !Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex))
        return false;
      int parentSheetIndex = (int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex;
      switch (parentSheetIndex)
      {
        case 275:
        case 791:
          return !disallow_special_geodes;
        default:
          try
          {
            if (Game1.objectInformation.ContainsKey(parentSheetIndex))
            {
              string[] strArray1 = Game1.objectInformation[parentSheetIndex].Split('/');
              if (strArray1.Length > 6)
              {
                string[] strArray2 = strArray1[6].Split(' ');
                return strArray2 != null && strArray2.Length != 0 && int.TryParse(strArray2[0], out int _);
              }
            }
          }
          catch (Exception ex)
          {
          }
          return false;
      }
    }

    public static Item getTreasureFromGeode(Item geode)
    {
      bool flag = Utility.IsGeode(geode);
      if (flag)
      {
        try
        {
          Random random = new Random((int) Game1.stats.GeodesCracked + (int) Game1.uniqueIDForThisGame / 2);
          int num1 = random.Next(1, 10);
          for (int index = 0; index < num1; ++index)
            random.NextDouble();
          int num2 = random.Next(1, 10);
          for (int index = 0; index < num2; ++index)
            random.NextDouble();
          int parentSheetIndex1 = (int) (NetFieldBase<int, NetInt>) (geode as Object).parentSheetIndex;
          if (random.NextDouble() <= 0.1 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            return (Item) new Object(890, random.NextDouble() < 0.25 ? 5 : 1);
          switch (parentSheetIndex1)
          {
            case 275:
              string[] strArray = Game1.objectInformation[parentSheetIndex1].Split('/')[6].Split(' ');
              int int32 = Convert.ToInt32(strArray[random.Next(strArray.Length)]);
              return parentSheetIndex1 == 749 && random.NextDouble() < 0.008 && (int) Game1.stats.GeodesCracked > 15 ? (Item) new Object(74, 1) : (Item) new Object(int32, 1);
            case 791:
              if (random.NextDouble() < 0.05 && !Game1.player.hasOrWillReceiveMail("goldenCoconutHat"))
              {
                Game1.player.mailReceived.Add("goldenCoconutHat");
                return (Item) new StardewValley.Objects.Hat(75);
              }
              switch (random.Next(7))
              {
                case 0:
                  return (Item) new Object(69, 1);
                case 1:
                  return (Item) new Object(835, 1);
                case 2:
                  return (Item) new Object(833, 5);
                case 3:
                  return (Item) new Object(831, 5);
                case 4:
                  return (Item) new Object(820, 1);
                case 5:
                  return (Item) new Object(292, 1);
                case 6:
                  return (Item) new Object(386, 5);
              }
              break;
            default:
              if (random.NextDouble() < 0.5)
              {
                int initialStack = random.Next(3) * 2 + 1;
                if (random.NextDouble() < 0.1)
                  initialStack = 10;
                if (random.NextDouble() < 0.01)
                  initialStack = 20;
                if (random.NextDouble() < 0.5)
                {
                  switch (random.Next(4))
                  {
                    case 0:
                    case 1:
                      return (Item) new Object(390, initialStack);
                    case 2:
                      return (Item) new Object(330, 1);
                    case 3:
                      int parentSheetIndex2;
                      switch (parentSheetIndex1)
                      {
                        case 535:
                          parentSheetIndex2 = 86;
                          break;
                        case 536:
                          parentSheetIndex2 = 84;
                          break;
                        case 749:
                          return (Item) new Object(82 + random.Next(3) * 2, 1);
                        default:
                          parentSheetIndex2 = 82;
                          break;
                      }
                      return (Item) new Object(parentSheetIndex2, 1);
                  }
                }
                else
                {
                  switch (parentSheetIndex1)
                  {
                    case 535:
                      switch (random.Next(3))
                      {
                        case 0:
                          return (Item) new Object(378, initialStack);
                        case 1:
                          return (Item) new Object(Game1.player.deepestMineLevel > 25 ? 380 : 378, initialStack);
                        case 2:
                          return (Item) new Object(382, initialStack);
                      }
                      break;
                    case 536:
                      switch (random.Next(4))
                      {
                        case 0:
                          return (Item) new Object(378, initialStack);
                        case 1:
                          return (Item) new Object(380, initialStack);
                        case 2:
                          return (Item) new Object(382, initialStack);
                        case 3:
                          return (Item) new Object(Game1.player.deepestMineLevel > 75 ? 384 : 380, initialStack);
                      }
                      break;
                    default:
                      switch (random.Next(5))
                      {
                        case 0:
                          return (Item) new Object(378, initialStack);
                        case 1:
                          return (Item) new Object(380, initialStack);
                        case 2:
                          return (Item) new Object(382, initialStack);
                        case 3:
                          return (Item) new Object(384, initialStack);
                        case 4:
                          return (Item) new Object(386, initialStack / 2 + 1);
                      }
                      break;
                  }
                }
              }
              else
                goto case 275;
              break;
          }
          return (Item) new Object(Vector2.Zero, 390, 1);
        }
        catch (Exception ex)
        {
        }
      }
      return flag ? (Item) new Object(Vector2.Zero, 390, 1) : (Item) null;
    }

    public static Vector2 snapToInt(Vector2 v)
    {
      v.X = (float) (int) v.X;
      v.Y = (float) (int) v.Y;
      return v;
    }

    public static Vector2 GetNearbyValidPlacementPosition(
      Farmer who,
      GameLocation location,
      Item item,
      int x,
      int y)
    {
      if (!Game1.isCheckingNonMousePlacement)
        return new Vector2((float) x, (float) y);
      int num1 = 1;
      int num2 = 1;
      Point point = new Point();
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, num1 * 64, num2 * 64);
      if (item is Furniture)
      {
        Furniture furniture = item as Furniture;
        num1 = furniture.getTilesWide();
        num2 = furniture.getTilesHigh();
        rectangle.Width = furniture.boundingBox.Value.Width;
        rectangle.Height = furniture.boundingBox.Value.Height;
      }
      switch (who.FacingDirection)
      {
        case 0:
          point.X = 0;
          point.Y = -1;
          y -= (num2 - 1) * 64;
          break;
        case 1:
          point.X = 1;
          point.Y = 0;
          break;
        case 2:
          point.X = 0;
          point.Y = 1;
          break;
        case 3:
          point.X = -1;
          point.Y = 0;
          x -= (num1 - 1) * 64;
          break;
      }
      int num3 = 2;
      if (item is Object && (item as Object).isPassable() && ((item as Object).Category == -74 || (item as Object).isSapling() || (int) (NetFieldBase<int, NetInt>) (item as Object).category == -19))
      {
        x = (int) who.GetToolLocation().X / 64 * 64;
        y = (int) who.GetToolLocation().Y / 64 * 64;
        point.X = who.getTileX() - x / 64;
        point.Y = who.getTileY() - y / 64;
        int num4 = (int) Math.Sqrt(Math.Pow((double) point.X, 2.0) + Math.Pow((double) point.Y, 2.0));
        if (num4 > 0)
        {
          point.X /= num4;
          point.Y /= num4;
        }
        num3 = num4 + 1;
      }
      bool flag = item is Object && (item as Object).isPassable();
      x = x / 64 * 64;
      y = y / 64 * 64;
      for (int index = 0; index < num3; ++index)
      {
        int x1 = x + point.X * index * 64;
        int y1 = y + point.Y * index * 64;
        rectangle.X = x1;
        rectangle.Y = y1;
        if (!who.GetBoundingBox().Intersects(rectangle) && !flag || Utility.playerCanPlaceItemHere(location, item, x1, y1, who))
          return new Vector2((float) x1, (float) y1);
      }
      return new Vector2((float) x, (float) y);
    }

    public static bool tryToPlaceItem(GameLocation location, Item item, int x, int y)
    {
      if (item == null || item is Tool)
        return false;
      Vector2 key = new Vector2((float) (x / 64), (float) (y / 64));
      if (Utility.playerCanPlaceItemHere(location, item, x, y, Game1.player))
      {
        if (item is Furniture)
          Game1.player.ActiveObject = (Object) null;
        if (((Object) item).placementAction(location, x, y, Game1.player))
        {
          Game1.player.reduceActiveItemByOne();
        }
        else
        {
          switch (item)
          {
            case Furniture _:
              Game1.player.ActiveObject = (Object) (item as Furniture);
              break;
            case Wallpaper _:
              return false;
          }
        }
        return true;
      }
      if (Utility.isPlacementForbiddenHere(location) && item != null && item.isPlaceable())
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
      else if (item is Furniture)
      {
        switch ((item as Furniture).GetAdditionalFurniturePlacementStatus(location, x, y, Game1.player))
        {
          case 1:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12629"));
            break;
          case 2:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12632"));
            break;
          case 3:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12633"));
            break;
          case 4:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12632"));
            break;
        }
      }
      if (item.Category == -19 && location.terrainFeatures.ContainsKey(key) && location.terrainFeatures[key] is HoeDirt)
      {
        HoeDirt terrainFeature = location.terrainFeatures[key] as HoeDirt;
        if ((int) (NetFieldBase<int, NetInt>) (location.terrainFeatures[key] as HoeDirt).fertilizer != 0)
        {
          if ((NetFieldBase<int, NetInt>) (location.terrainFeatures[key] as HoeDirt).fertilizer != item.parentSheetIndex)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916-2"));
          return false;
        }
        if (((int) (NetFieldBase<int, NetInt>) item.parentSheetIndex == 368 || (int) (NetFieldBase<int, NetInt>) item.parentSheetIndex == 368) && terrainFeature.crop != null && (int) (NetFieldBase<int, NetInt>) terrainFeature.crop.currentPhase != 0)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916"));
          return false;
        }
      }
      return false;
    }

    public static int showLanternBar()
    {
      foreach (Item obj in (IEnumerable<Item>) Game1.player.Items)
      {
        if (obj != null && obj is Lantern && ((Lantern) obj).on)
          return ((Lantern) obj).fuelLeft;
      }
      return -1;
    }

    public static void plantCrops(
      GameLocation farm,
      int seedType,
      int x,
      int y,
      int width,
      int height,
      int daysOld)
    {
      for (int x1 = x; x1 < x + width; ++x1)
      {
        for (int y1 = y; y1 < y + height; ++y1)
        {
          Vector2 vector2 = new Vector2((float) x1, (float) y1);
          farm.makeHoeDirt(vector2);
          if (farm.terrainFeatures.ContainsKey(vector2) && farm.terrainFeatures[vector2] is HoeDirt)
            ((HoeDirt) farm.terrainFeatures[vector2]).crop = new Crop(seedType, x, y);
        }
      }
    }

    public static bool pointInRectangles(List<Microsoft.Xna.Framework.Rectangle> rectangles, int x, int y)
    {
      foreach (Microsoft.Xna.Framework.Rectangle rectangle in rectangles)
      {
        if (rectangle.Contains(x, y))
          return true;
      }
      return false;
    }

    public static Keys mapGamePadButtonToKey(Buttons b)
    {
      switch (b)
      {
        case Buttons.DPadUp:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton);
        case Buttons.DPadDown:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton);
        case Buttons.DPadLeft:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton);
        case Buttons.DPadRight:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton);
        case Buttons.Start:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
        case Buttons.Back:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.journalButton);
        case Buttons.A:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.actionButton);
        case Buttons.B:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
        case Buttons.X:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.useToolButton);
        case Buttons.Y:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
        case Buttons.LeftThumbstickLeft:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton);
        case Buttons.LeftThumbstickUp:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton);
        case Buttons.LeftThumbstickDown:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton);
        case Buttons.LeftThumbstickRight:
          return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton);
        default:
          return Keys.None;
      }
    }

    public static ButtonCollection getPressedButtons(
      GamePadState padState,
      GamePadState oldPadState)
    {
      return new ButtonCollection(ref padState, ref oldPadState);
    }

    public static bool thumbstickIsInDirection(int direction, GamePadState padState)
    {
      if (Game1.currentMinigame != null)
        return true;
      GamePadThumbSticks thumbSticks;
      if (direction == 0)
      {
        thumbSticks = padState.ThumbSticks;
        double num = (double) Math.Abs(thumbSticks.Left.X);
        thumbSticks = padState.ThumbSticks;
        double y = (double) thumbSticks.Left.Y;
        if (num < y)
          return true;
      }
      if (direction == 1)
      {
        thumbSticks = padState.ThumbSticks;
        double x = (double) thumbSticks.Left.X;
        thumbSticks = padState.ThumbSticks;
        double num = (double) Math.Abs(thumbSticks.Left.Y);
        if (x > num)
          return true;
      }
      if (direction == 2)
      {
        thumbSticks = padState.ThumbSticks;
        double num1 = (double) Math.Abs(thumbSticks.Left.X);
        thumbSticks = padState.ThumbSticks;
        double num2 = (double) Math.Abs(thumbSticks.Left.Y);
        if (num1 < num2)
          return true;
      }
      if (direction == 3)
      {
        thumbSticks = padState.ThumbSticks;
        double num3 = (double) Math.Abs(thumbSticks.Left.X);
        thumbSticks = padState.ThumbSticks;
        double num4 = (double) Math.Abs(thumbSticks.Left.Y);
        if (num3 > num4)
          return true;
      }
      return false;
    }

    public static ButtonCollection getHeldButtons(GamePadState padState) => new ButtonCollection(ref padState);

    /// <summary>return true if music becomes muted</summary>
    /// <returns></returns>
    public static bool toggleMuteMusic()
    {
      if (Game1.soundBank != null)
      {
        if ((double) Game1.options.musicVolumeLevel != 0.0)
        {
          Utility.disableMusic();
          return true;
        }
        Utility.enableMusic();
      }
      return false;
    }

    public static void enableMusic()
    {
      if (Game1.soundBank == null)
        return;
      Game1.options.musicVolumeLevel = 0.75f;
      Game1.musicCategory.SetVolume(0.75f);
      Game1.musicPlayerVolume = 0.75f;
      Game1.options.ambientVolumeLevel = 0.75f;
      Game1.ambientCategory.SetVolume(0.75f);
      Game1.ambientPlayerVolume = 0.75f;
    }

    public static void disableMusic()
    {
      if (Game1.soundBank == null)
        return;
      Game1.options.musicVolumeLevel = 0.0f;
      Game1.musicCategory.SetVolume(0.0f);
      Game1.options.ambientVolumeLevel = 0.0f;
      Game1.ambientCategory.SetVolume(0.0f);
      Game1.ambientPlayerVolume = 0.0f;
      Game1.musicPlayerVolume = 0.0f;
    }

    public static Vector2 getVelocityTowardPlayer(
      Point startingPoint,
      float speed,
      Farmer f)
    {
      return Utility.getVelocityTowardPoint(startingPoint, new Vector2((float) f.GetBoundingBox().X, (float) f.GetBoundingBox().Y), speed);
    }

    public static string getHoursMinutesStringFromMilliseconds(ulong milliseconds)
    {
      ulong num = milliseconds / 3600000UL;
      string str1 = num.ToString();
      string str2 = milliseconds % 3600000UL / 60000UL < 10UL ? "0" : "";
      num = milliseconds % 3600000UL / 60000UL;
      string str3 = num.ToString();
      return str1 + ":" + str2 + str3;
    }

    public static string getMinutesSecondsStringFromMilliseconds(int milliseconds)
    {
      int num = milliseconds / 60000;
      string str1 = num.ToString();
      string str2 = milliseconds % 60000 / 1000 < 10 ? "0" : "";
      num = milliseconds % 60000 / 1000;
      string str3 = num.ToString();
      return str1 + ":" + str2 + str3;
    }

    public static Vector2 getVelocityTowardPoint(
      Vector2 startingPoint,
      Vector2 endingPoint,
      float speed)
    {
      double x1 = (double) endingPoint.X - (double) startingPoint.X;
      double x2 = (double) endingPoint.Y - (double) startingPoint.Y;
      if (Math.Abs(x1) < 0.1 && Math.Abs(x2) < 0.1)
        return new Vector2(0.0f, 0.0f);
      double num1 = Math.Sqrt(Math.Pow(x1, 2.0) + Math.Pow(x2, 2.0));
      double num2 = x1 / num1;
      double num3 = x2 / num1;
      return new Vector2((float) num2 * speed, (float) num3 * speed);
    }

    public static Vector2 getVelocityTowardPoint(
      Point startingPoint,
      Vector2 endingPoint,
      float speed)
    {
      return Utility.getVelocityTowardPoint(new Vector2((float) startingPoint.X, (float) startingPoint.Y), endingPoint, speed);
    }

    public static Vector2 getRandomPositionInThisRectangle(Microsoft.Xna.Framework.Rectangle r, Random random) => new Vector2((float) random.Next(r.X, r.X + r.Width), (float) random.Next(r.Y, r.Y + r.Height));

    public static Vector2 getTopLeftPositionForCenteringOnScreen(
      xTile.Dimensions.Rectangle viewport,
      int width,
      int height,
      int xOffset = 0,
      int yOffset = 0)
    {
      return new Vector2((float) (viewport.Width / 2 - width / 2 + xOffset), (float) (viewport.Height / 2 - height / 2 + yOffset));
    }

    public static Vector2 getTopLeftPositionForCenteringOnScreen(
      int width,
      int height,
      int xOffset = 0,
      int yOffset = 0)
    {
      return Utility.getTopLeftPositionForCenteringOnScreen(Game1.uiViewport, width, height, xOffset, yOffset);
    }

    public static void recursiveFindPositionForCharacter(
      NPC c,
      GameLocation l,
      Vector2 tileLocation,
      int maxIterations)
    {
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      vector2Queue.Enqueue(tileLocation);
      List<Vector2> vector2List = new List<Vector2>();
      for (; num < maxIterations && vector2Queue.Count > 0; ++num)
      {
        Vector2 vector2 = vector2Queue.Dequeue();
        vector2List.Add(vector2);
        c.Position = new Vector2((float) ((double) vector2.X * 64.0 + 32.0) - (float) (c.GetBoundingBox().Width / 2), vector2.Y * 64f - (float) c.GetBoundingBox().Height);
        if (!l.isCollidingPosition(c.GetBoundingBox(), Game1.viewport, false, 0, false, (Character) c, true))
        {
          if (l.characters.Contains(c))
            break;
          l.characters.Add(c);
          c.currentLocation = l;
          break;
        }
        foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
        {
          if (!vector2List.Contains(vector2 + directionsTileVector))
            vector2Queue.Enqueue(vector2 + directionsTileVector);
        }
      }
    }

    public static Vector2 recursiveFindOpenTileForCharacter(
      Character c,
      GameLocation l,
      Vector2 tileLocation,
      int maxIterations,
      bool allowOffMap = true)
    {
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      vector2Queue.Enqueue(tileLocation);
      List<Vector2> vector2List = new List<Vector2>();
      Vector2 position1 = c.Position;
      for (; num < maxIterations && vector2Queue.Count > 0; ++num)
      {
        Vector2 position2 = vector2Queue.Dequeue();
        vector2List.Add(position2);
        c.Position = new Vector2((float) ((double) position2.X * 64.0 + 32.0) - (float) (c.GetBoundingBox().Width / 2), (float) ((double) position2.Y * 64.0 + 4.0));
        if (!l.isCollidingPosition(c.GetBoundingBox(), Game1.viewport, c is Farmer, 0, false, c, true) && (allowOffMap || l.isTileOnMap(position2)))
        {
          c.Position = position1;
          return position2;
        }
        foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
        {
          if (!vector2List.Contains(position2 + directionsTileVector))
            vector2Queue.Enqueue(position2 + directionsTileVector);
        }
      }
      c.Position = position1;
      return Vector2.Zero;
    }

    public static List<Vector2> recursiveFindOpenTiles(
      GameLocation l,
      Vector2 tileLocation,
      int maxOpenTilesToFind = 24,
      int maxIterations = 50)
    {
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      vector2Queue.Enqueue(tileLocation);
      List<Vector2> vector2List = new List<Vector2>();
      List<Vector2> openTiles;
      for (openTiles = new List<Vector2>(); num < maxIterations && vector2Queue.Count > 0 && openTiles.Count < maxOpenTilesToFind; ++num)
      {
        Vector2 v = vector2Queue.Dequeue();
        vector2List.Add(v);
        if (l.isTileLocationTotallyClearAndPlaceable(v))
          openTiles.Add(v);
        foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
        {
          if (!vector2List.Contains(v + directionsTileVector))
            vector2Queue.Enqueue(v + directionsTileVector);
        }
      }
      return openTiles;
    }

    public static void spreadAnimalsAround(Building b, Farm environment)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    public static void spreadAnimalsAround(
      Building b,
      Farm environment,
      List<FarmAnimal> animalsList)
    {
      if (b.indoors.Value == null || !(b.indoors.Value is AnimalHouse))
        return;
      Queue<FarmAnimal> farmAnimalQueue = new Queue<FarmAnimal>((IEnumerable<FarmAnimal>) animalsList);
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      vector2Queue.Enqueue(new Vector2((float) ((int) (NetFieldBase<int, NetInt>) b.tileX + b.animalDoor.X), (float) ((int) (NetFieldBase<int, NetInt>) b.tileY + b.animalDoor.Y + 1)));
      for (; farmAnimalQueue.Count > 0 && num < 40 && vector2Queue.Count > 0; ++num)
      {
        Vector2 vector2 = vector2Queue.Dequeue();
        farmAnimalQueue.Peek().Position = new Vector2((float) ((double) vector2.X * 64.0 + 32.0) - (float) (farmAnimalQueue.Peek().GetBoundingBox().Width / 2), (float) ((double) vector2.Y * 64.0 - 32.0) - (float) (farmAnimalQueue.Peek().GetBoundingBox().Height / 2));
        if (!environment.isCollidingPosition(farmAnimalQueue.Peek().GetBoundingBox(), Game1.viewport, false, 0, false, (Character) farmAnimalQueue.Peek(), true, false, false))
        {
          FarmAnimal farmAnimal = farmAnimalQueue.Dequeue();
          environment.animals.Add((long) farmAnimal.myID, farmAnimal);
        }
        if (farmAnimalQueue.Count > 0)
        {
          foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
          {
            farmAnimalQueue.Peek().Position = new Vector2((float) (((double) vector2.X + (double) directionsTileVector.X) * 64.0 + 32.0) - (float) (farmAnimalQueue.Peek().GetBoundingBox().Width / 2), (float) (((double) vector2.Y + (double) directionsTileVector.Y) * 64.0 - 32.0) - (float) (farmAnimalQueue.Peek().GetBoundingBox().Height / 2));
            if (!environment.isCollidingPosition(farmAnimalQueue.Peek().GetBoundingBox(), Game1.viewport, false, 0, false, (Character) farmAnimalQueue.Peek(), true, false, false))
              vector2Queue.Enqueue(vector2 + directionsTileVector);
          }
        }
      }
    }

    public static bool[] horizontalOrVerticalCollisionDirections(
      Microsoft.Xna.Framework.Rectangle boundingBox,
      bool projectile = false)
    {
      return Utility.horizontalOrVerticalCollisionDirections(boundingBox, (Character) null, projectile);
    }

    public static Point findTile(GameLocation location, int tileIndex, string layer)
    {
      for (int y = 0; y < location.map.GetLayer(layer).LayerHeight; ++y)
      {
        for (int x = 0; x < location.map.GetLayer(layer).LayerWidth; ++x)
        {
          if (location.getTileIndexAt(x, y, layer) == tileIndex)
            return new Point(x, y);
        }
      }
      return new Point(-1, -1);
    }

    public static bool[] horizontalOrVerticalCollisionDirections(
      Microsoft.Xna.Framework.Rectangle boundingBox,
      Character c,
      bool projectile = false)
    {
      bool[] flagArray = new bool[2];
      Microsoft.Xna.Framework.Rectangle position = new Microsoft.Xna.Framework.Rectangle(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
      position.Width = 1;
      position.X = boundingBox.Center.X;
      if (c != null)
      {
        if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
          flagArray[1] = true;
      }
      else if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
        flagArray[1] = true;
      position.Width = boundingBox.Width;
      position.X = boundingBox.X;
      position.Height = 1;
      position.Y = boundingBox.Center.Y;
      if (c != null)
      {
        if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
          flagArray[0] = true;
      }
      else if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
        flagArray[0] = true;
      return flagArray;
    }

    public static Microsoft.Xna.Framework.Color getBlendedColor(Microsoft.Xna.Framework.Color c1, Microsoft.Xna.Framework.Color c2) => new Microsoft.Xna.Framework.Color(Game1.random.NextDouble() < 0.5 ? (int) Math.Max(c1.R, c2.R) : ((int) c1.R + (int) c2.R) / 2, Game1.random.NextDouble() < 0.5 ? (int) Math.Max(c1.G, c2.G) : ((int) c1.G + (int) c2.G) / 2, Game1.random.NextDouble() < 0.5 ? (int) Math.Max(c1.B, c2.B) : ((int) c1.B + (int) c2.B) / 2);

    public static Character checkForCharacterWithinArea(
      Type kindOfCharacter,
      Vector2 positionToAvoid,
      GameLocation location,
      Microsoft.Xna.Framework.Rectangle area)
    {
      foreach (NPC character in location.characters)
      {
        if (character.GetType().Equals(kindOfCharacter) && character.GetBoundingBox().Intersects(area) && !character.Position.Equals(positionToAvoid))
          return (Character) character;
      }
      return (Character) null;
    }

    public static int getNumberOfCharactersInRadius(GameLocation l, Point position, int tileRadius)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(position.X - tileRadius * 64, position.Y - tileRadius * 64, (tileRadius * 2 + 1) * 64, (tileRadius * 2 + 1) * 64);
      int charactersInRadius = 0;
      foreach (NPC character in l.characters)
      {
        if (rectangle.Contains(Utility.Vector2ToPoint(character.Position)))
          ++charactersInRadius;
      }
      return charactersInRadius;
    }

    public static List<Vector2> getListOfTileLocationsForBordersOfNonTileRectangle(
      Microsoft.Xna.Framework.Rectangle rectangle)
    {
      return new List<Vector2>()
      {
        new Vector2((float) (rectangle.Left / 64), (float) (rectangle.Top / 64)),
        new Vector2((float) (rectangle.Right / 64), (float) (rectangle.Top / 64)),
        new Vector2((float) (rectangle.Left / 64), (float) (rectangle.Bottom / 64)),
        new Vector2((float) (rectangle.Right / 64), (float) (rectangle.Bottom / 64)),
        new Vector2((float) (rectangle.Left / 64), (float) (rectangle.Center.Y / 64)),
        new Vector2((float) (rectangle.Right / 64), (float) (rectangle.Center.Y / 64)),
        new Vector2((float) (rectangle.Center.X / 64), (float) (rectangle.Bottom / 64)),
        new Vector2((float) (rectangle.Center.X / 64), (float) (rectangle.Top / 64)),
        new Vector2((float) (rectangle.Center.X / 64), (float) (rectangle.Center.Y / 64))
      };
    }

    public static void makeTemporarySpriteJuicier(
      TemporaryAnimatedSprite t,
      GameLocation l,
      int numAddOns = 4,
      int xRange = 64,
      int yRange = 64)
    {
      t.position.Y -= 8f;
      l.temporarySprites.Add(t);
      for (int index = 0; index < numAddOns; ++index)
      {
        TemporaryAnimatedSprite clone = t.getClone();
        clone.delayBeforeAnimationStart = index * 100;
        clone.position += new Vector2((float) Game1.random.Next(-xRange / 2, xRange / 2 + 1), (float) Game1.random.Next(-yRange / 2, yRange / 2 + 1));
        l.temporarySprites.Add(clone);
      }
    }

    public static void recursiveObjectPlacement(
      Object o,
      int tileX,
      int tileY,
      double growthRate,
      double decay,
      GameLocation location,
      string terrainToExclude = "",
      int objectIndexAddRange = 0,
      double failChance = 0.0,
      int objectIndeAddRangeMultiplier = 1)
    {
      if (!location.isTileLocationOpen(new Location(tileX, tileY)) || location.isTileOccupied(new Vector2((float) tileX, (float) tileY)) || location.getTileIndexAt(tileX, tileY, "Back") == -1 || !terrainToExclude.Equals("") && (location.doesTileHaveProperty(tileX, tileY, "Type", "Back") == null || location.doesTileHaveProperty(tileX, tileY, "Type", "Back").Equals(terrainToExclude)))
        return;
      Vector2 vector2 = new Vector2((float) tileX, (float) tileY);
      if (Game1.random.NextDouble() > failChance * 2.0)
      {
        if (o is ColoredObject)
        {
          OverlaidDictionary objects = location.objects;
          Vector2 key = vector2;
          ColoredObject coloredObject = new ColoredObject((int) (NetFieldBase<int, NetInt>) o.parentSheetIndex + Game1.random.Next(objectIndexAddRange + 1) * objectIndeAddRangeMultiplier, 1, (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) (o as ColoredObject).color);
          coloredObject.Fragility = (int) (NetFieldBase<int, NetInt>) o.fragility;
          coloredObject.MinutesUntilReady = (int) (NetFieldBase<int, NetIntDelta>) o.minutesUntilReady;
          coloredObject.Name = o.name;
          coloredObject.CanBeSetDown = o.CanBeSetDown;
          coloredObject.CanBeGrabbed = o.CanBeGrabbed;
          coloredObject.IsSpawnedObject = o.IsSpawnedObject;
          coloredObject.TileLocation = vector2;
          coloredObject.ColorSameIndexAsParentSheetIndex = (o as ColoredObject).ColorSameIndexAsParentSheetIndex;
          objects.Add(key, (Object) coloredObject);
        }
        else
          location.objects.Add(vector2, new Object(vector2, (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex + Game1.random.Next(objectIndexAddRange + 1) * objectIndeAddRangeMultiplier, o.name, (bool) (NetFieldBase<bool, NetBool>) o.canBeSetDown, (bool) (NetFieldBase<bool, NetBool>) o.canBeGrabbed, (bool) (NetFieldBase<bool, NetBool>) o.isHoedirt, (bool) (NetFieldBase<bool, NetBool>) o.isSpawnedObject)
          {
            Fragility = (int) (NetFieldBase<int, NetInt>) o.fragility,
            MinutesUntilReady = (int) (NetFieldBase<int, NetIntDelta>) o.minutesUntilReady
          });
      }
      growthRate -= decay;
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveObjectPlacement(o, tileX + 1, tileY, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveObjectPlacement(o, tileX - 1, tileY, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveObjectPlacement(o, tileX, tileY + 1, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier);
      if (Game1.random.NextDouble() >= growthRate)
        return;
      Utility.recursiveObjectPlacement(o, tileX, tileY - 1, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier);
    }

    public static void recursiveFarmGrassPlacement(
      int tileX,
      int tileY,
      double growthRate,
      double decay,
      GameLocation farm)
    {
      if (!farm.isTileLocationOpen(new Location(tileX, tileY)) || farm.isTileOccupied(new Vector2((float) tileX, (float) tileY)) || farm.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") == null)
        return;
      Vector2 key = new Vector2((float) tileX, (float) tileY);
      if (Game1.random.NextDouble() < 0.05)
        farm.objects.Add(new Vector2((float) tileX, (float) tileY), new Object(new Vector2((float) tileX, (float) tileY), Game1.random.NextDouble() < 0.5 ? 674 : 675, 1));
      else
        farm.terrainFeatures.Add(key, (TerrainFeature) new Grass(1, 4 - (int) ((1.0 - growthRate) * 4.0)));
      growthRate -= decay;
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveFarmGrassPlacement(tileX + 1, tileY, growthRate, decay, farm);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveFarmGrassPlacement(tileX - 1, tileY, growthRate, decay, farm);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveFarmGrassPlacement(tileX, tileY + 1, growthRate, decay, farm);
      if (Game1.random.NextDouble() >= growthRate)
        return;
      Utility.recursiveFarmGrassPlacement(tileX, tileY - 1, growthRate, decay, farm);
    }

    public static void recursiveTreePlacement(
      int tileX,
      int tileY,
      double growthRate,
      int growthStage,
      double skipChance,
      GameLocation l,
      Microsoft.Xna.Framework.Rectangle clearPatch,
      bool sparse)
    {
      if (clearPatch.Contains(tileX, tileY))
        return;
      Vector2 vector2 = new Vector2((float) tileX, (float) tileY);
      if (l.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Diggable", "Back") == null || l.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "NoSpawn", "Back") != null || !l.isTileLocationOpen(new Location((int) vector2.X, (int) vector2.Y)) || l.isTileOccupied(vector2) || sparse && (l.isTileOccupied(new Vector2((float) tileX, (float) (tileY - 1))) || l.isTileOccupied(new Vector2((float) tileX, (float) (tileY + 1))) || l.isTileOccupied(new Vector2((float) (tileX + 1), (float) tileY)) || l.isTileOccupied(new Vector2((float) (tileX - 1), (float) tileY)) || l.isTileOccupied(new Vector2((float) (tileX + 1), (float) (tileY + 1)))))
        return;
      if (Game1.random.NextDouble() > skipChance)
      {
        if (sparse && (double) vector2.X < 70.0 && ((double) vector2.X < 48.0 || (double) vector2.Y > 26.0) && Game1.random.NextDouble() < 0.07)
          (l as Farm).resourceClumps.Add(new ResourceClump(Game1.random.NextDouble() < 0.5 ? 672 : (Game1.random.NextDouble() < 0.5 ? 600 : 602), 2, 2, vector2));
        else
          l.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(Game1.random.Next(1, 4), growthStage < 5 ? Game1.random.Next(5) : 5));
        growthRate -= 0.05;
      }
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveTreePlacement(tileX + Game1.random.Next(1, 3), tileY, growthRate, growthStage, skipChance, l, clearPatch, sparse);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveTreePlacement(tileX - Game1.random.Next(1, 3), tileY, growthRate, growthStage, skipChance, l, clearPatch, sparse);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveTreePlacement(tileX, tileY + Game1.random.Next(1, 3), growthRate, growthStage, skipChance, l, clearPatch, sparse);
      if (Game1.random.NextDouble() >= growthRate)
        return;
      Utility.recursiveTreePlacement(tileX, tileY - Game1.random.Next(1, 3), growthRate, growthStage, skipChance, l, clearPatch, sparse);
    }

    public static void recursiveRemoveTerrainFeatures(
      int tileX,
      int tileY,
      double growthRate,
      double decay,
      GameLocation l)
    {
      Vector2 key = new Vector2((float) tileX, (float) tileY);
      l.terrainFeatures.Remove(key);
      growthRate -= decay;
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveRemoveTerrainFeatures(tileX + 1, tileY, growthRate, decay, l);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveRemoveTerrainFeatures(tileX - 1, tileY, growthRate, decay, l);
      if (Game1.random.NextDouble() < growthRate)
        Utility.recursiveRemoveTerrainFeatures(tileX, tileY + 1, growthRate, decay, l);
      if (Game1.random.NextDouble() >= growthRate)
        return;
      Utility.recursiveRemoveTerrainFeatures(tileX, tileY - 1, growthRate, decay, l);
    }

    public static IEnumerator<int> generateNewFarm(bool skipFarmGeneration) => Utility.generateNewFarm(skipFarmGeneration, true);

    public static IEnumerator<int> generateNewFarm(
      bool skipFarmGeneration,
      bool loadForNewGame)
    {
      Game1.fadeToBlack = false;
      Game1.fadeToBlackAlpha = 1f;
      Game1.debrisWeather.Clear();
      Game1.viewport.X = -9999;
      Game1.changeMusicTrack("none");
      if (loadForNewGame)
        Game1.loadForNewGame();
      Game1.currentLocation = Game1.getLocationFromName("Farmhouse");
      Game1.currentLocation.currentEvent = new Event("none/-600 -600/farmer 4 8 2/warp farmer 4 8/end beginGame");
      Game1.gameMode = (byte) 2;
      yield return 100;
    }

    public static bool isOnScreen(Vector2 positionNonTile, int acceptableDistanceFromScreen)
    {
      positionNonTile.X -= (float) Game1.viewport.X;
      positionNonTile.Y -= (float) Game1.viewport.Y;
      return (double) positionNonTile.X > (double) -acceptableDistanceFromScreen && (double) positionNonTile.X < (double) (Game1.viewport.Width + acceptableDistanceFromScreen) && (double) positionNonTile.Y > (double) -acceptableDistanceFromScreen && (double) positionNonTile.Y < (double) (Game1.viewport.Height + acceptableDistanceFromScreen);
    }

    public static bool isOnScreen(
      Point positionTile,
      int acceptableDistanceFromScreenNonTile,
      GameLocation location = null)
    {
      return (location == null || location.Equals(Game1.currentLocation)) && positionTile.X * 64 > Game1.viewport.X - acceptableDistanceFromScreenNonTile && positionTile.X * 64 < Game1.viewport.X + Game1.viewport.Width + acceptableDistanceFromScreenNonTile && positionTile.Y * 64 > Game1.viewport.Y - acceptableDistanceFromScreenNonTile && positionTile.Y * 64 < Game1.viewport.Y + Game1.viewport.Height + acceptableDistanceFromScreenNonTile;
    }

    public static void createPotteryTreasure(int tileX, int tileY)
    {
    }

    public static void clearObjectsInArea(Microsoft.Xna.Framework.Rectangle r, GameLocation l)
    {
      for (int left = r.Left; left < r.Right; left += 64)
      {
        for (int top = r.Top; top < r.Bottom; top += 64)
          l.removeEverythingFromThisTile(left / 64, top / 64);
      }
    }

    public static void trashItem(Item item)
    {
      if (item is Object && Game1.player.specialItems.Contains((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex))
        Game1.player.specialItems.Remove((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
      if (Utility.getTrashReclamationPrice(item, Game1.player) > 0)
        Game1.player.Money += Utility.getTrashReclamationPrice(item, Game1.player);
      Game1.playSound("trashcan");
    }

    public static FarmAnimal GetBestHarvestableFarmAnimal(
      IEnumerable<FarmAnimal> animals,
      Tool tool,
      Microsoft.Xna.Framework.Rectangle toolRect)
    {
      FarmAnimal harvestableFarmAnimal = (FarmAnimal) null;
      foreach (FarmAnimal animal in animals)
      {
        if (animal.GetHarvestBoundingBox().Intersects(toolRect))
        {
          if (animal.toolUsedForHarvest.Equals((object) tool.BaseName) && (int) (NetFieldBase<int, NetInt>) animal.currentProduce > 0 && (int) (NetFieldBase<int, NetInt>) animal.age >= (int) (byte) (NetFieldBase<byte, NetByte>) animal.ageWhenMature)
            return animal;
          harvestableFarmAnimal = animal;
        }
      }
      return harvestableFarmAnimal;
    }

    public static void recolorDialogueAndMenu(string theme)
    {
      Microsoft.Xna.Framework.Color color1 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color2 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color3 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color4 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color5 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color6 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color7 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color8 = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color9 = Microsoft.Xna.Framework.Color.White;
      switch (theme)
      {
        case "Basic":
          color1 = new Microsoft.Xna.Framework.Color(47, 46, 36);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color(220, 215, 194);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Biomes":
          color1 = new Microsoft.Xna.Framework.Color(17, 36, 0);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color(192, (int) byte.MaxValue, 183);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Bombs Away":
          color1 = new Microsoft.Xna.Framework.Color(50, 20, 0);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          Microsoft.Xna.Framework.Color tan = Microsoft.Xna.Framework.Color.Tan;
          color6 = new Microsoft.Xna.Framework.Color((int) color4.R + 30, (int) color4.G + 30, (int) color4.B + 30);
          color7 = new Microsoft.Xna.Framework.Color(192, 167, 143);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Duchess":
          color1 = new Microsoft.Xna.Framework.Color(69, 45, 0);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 30);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 20);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 20);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 10);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 10);
          color7 = new Microsoft.Xna.Framework.Color(227, 221, 174);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Earthy":
          color1 = new Microsoft.Xna.Framework.Color(44, 35, 0);
          color2 = new Microsoft.Xna.Framework.Color(115, 147, 102);
          color3 = new Microsoft.Xna.Framework.Color(91, 65, 0);
          color4 = new Microsoft.Xna.Framework.Color(122, 83, 0);
          color5 = new Microsoft.Xna.Framework.Color(179, 181, 125);
          color6 = new Microsoft.Xna.Framework.Color(144, 96, 0);
          color7 = new Microsoft.Xna.Framework.Color(234, 227, 190);
          color8 = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 227);
          color9 = new Microsoft.Xna.Framework.Color(193, 187, 156);
          break;
        case "Ghosts N' Goblins":
          color1 = new Microsoft.Xna.Framework.Color(55, 0, 0);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color(196, 197, 230);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Outer Space":
          color1 = new Microsoft.Xna.Framework.Color(20, 20, 20);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color(194, 189, 202);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Polynomial":
          color1 = new Microsoft.Xna.Framework.Color(60, 60, 60);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color6 = new Microsoft.Xna.Framework.Color(254, 254, 254);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 30, (int) color4.G + 30, (int) color4.B + 30);
          color7 = new Microsoft.Xna.Framework.Color(225, 225, 225);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Skyscape":
          color1 = new Microsoft.Xna.Framework.Color(15, 31, 57);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color(206, 237, 254);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Sports":
          color1 = new Microsoft.Xna.Framework.Color(110, 45, 0);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 214, 168);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Sweeties":
          color1 = new Microsoft.Xna.Framework.Color(120, 60, 60);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 213, 227);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
        case "Wasteland":
          color1 = new Microsoft.Xna.Framework.Color(14, 12, 10);
          color2 = new Microsoft.Xna.Framework.Color((int) color1.R + 60, (int) color1.G + 60, (int) color1.B + 60);
          color3 = new Microsoft.Xna.Framework.Color((int) color2.R + 30, (int) color2.G + 30, (int) color2.B + 30);
          color4 = new Microsoft.Xna.Framework.Color((int) color3.R + 30, (int) color3.G + 30, (int) color3.B + 30);
          color5 = new Microsoft.Xna.Framework.Color((int) color4.R + 15, (int) color4.G + 15, (int) color4.B + 15);
          color6 = new Microsoft.Xna.Framework.Color((int) color5.R + 15, (int) color5.G + 15, (int) color5.B + 15);
          color7 = new Microsoft.Xna.Framework.Color(185, 178, 165);
          color8 = new Microsoft.Xna.Framework.Color(Math.Min((int) byte.MaxValue, (int) color7.R + 30), Math.Min((int) byte.MaxValue, (int) color7.G + 30), Math.Min((int) byte.MaxValue, (int) color7.B + 30));
          color9 = new Microsoft.Xna.Framework.Color((int) color7.R - 30, (int) color7.G - 30, (int) color7.B - 30);
          break;
      }
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 15633, (int) color1.R, (int) color1.G, (int) color1.B);
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 15645, (int) color6.R, (int) color6.G, (int) color6.B);
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 15649, (int) color4.R, (int) color4.G, (int) color4.B);
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 15641, (int) color4.R, (int) color4.G, (int) color4.B);
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 15637, (int) color3.R, (int) color3.G, (int) color3.B);
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 15666, (int) color7.R, (int) color7.G, (int) color7.B);
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 40577, (int) color8.R, (int) color8.G, (int) color8.B);
      Game1.menuTexture = ColorChanger.swapColor(Game1.menuTexture, 40637, (int) color9.R, (int) color9.G, (int) color9.B);
      Game1.toolIconBox = ColorChanger.swapColor(Game1.toolIconBox, 1760, (int) color1.R, (int) color1.G, (int) color1.B);
      Game1.toolIconBox = ColorChanger.swapColor(Game1.toolIconBox, 1764, (int) color3.R, (int) color3.G, (int) color3.B);
      Game1.toolIconBox = ColorChanger.swapColor(Game1.toolIconBox, 1768, (int) color4.R, (int) color4.G, (int) color4.B);
      Game1.toolIconBox = ColorChanger.swapColor(Game1.toolIconBox, 1841, (int) color6.R, (int) color6.G, (int) color6.B);
      Game1.toolIconBox = ColorChanger.swapColor(Game1.toolIconBox, 1792, (int) color7.R, (int) color7.G, (int) color7.B);
      Game1.toolIconBox = ColorChanger.swapColor(Game1.toolIconBox, 1834, (int) color8.R, (int) color8.G, (int) color8.B);
      Game1.toolIconBox = ColorChanger.swapColor(Game1.toolIconBox, 1773, (int) color9.R, (int) color9.G, (int) color9.B);
    }

    public static long RandomLong(Random r = null)
    {
      if (r == null)
        r = new Random();
      byte[] buffer = new byte[8];
      r.NextBytes(buffer);
      return BitConverter.ToInt64(buffer, 0);
    }

    public static ulong NewUniqueIdForThisGame() => (ulong) (long) (DateTime.UtcNow - new DateTime(2012, 6, 22)).TotalSeconds;

    public static string FilterDirtyWords(string words) => Program.sdk.FilterDirtyWords(words);

    /// <summary>
    /// This is used to filter out special characters from user entered
    /// names to avoid crashes and other bugs in Dialogue.cs parsing.
    /// 
    /// The characters are replaced with spaces.
    /// </summary>
    public static string FilterUserName(string name) => name;

    public static bool IsHorizontalDirection(int direction) => direction == 3 || direction == 1;

    public static bool IsVerticalDirection(int direction) => direction == 0 || direction == 2;

    public static Microsoft.Xna.Framework.Rectangle ExpandRectangle(
      Microsoft.Xna.Framework.Rectangle rect,
      int pixels)
    {
      rect.Height += 2 * pixels;
      rect.Width += 2 * pixels;
      rect.X -= pixels;
      rect.Y -= pixels;
      return rect;
    }

    public static Microsoft.Xna.Framework.Rectangle ExpandRectangle(
      Microsoft.Xna.Framework.Rectangle rect,
      int facingDirection,
      int pixels)
    {
      switch (facingDirection)
      {
        case 0:
          rect.Height += pixels;
          rect.Y -= pixels;
          break;
        case 1:
          rect.Width += pixels;
          break;
        case 2:
          rect.Height += pixels;
          break;
        case 3:
          rect.Width += pixels;
          rect.X -= pixels;
          break;
      }
      return rect;
    }

    public static int GetOppositeFacingDirection(int facingDirection)
    {
      switch (facingDirection)
      {
        case 0:
          return 2;
        case 1:
          return 3;
        case 2:
          return 0;
        case 3:
          return 1;
        default:
          return 0;
      }
    }

    public static void RGBtoHSL(int r, int g, int b, out double h, out double s, out double l)
    {
      double num1 = (double) r / (double) byte.MaxValue;
      double num2 = (double) g / (double) byte.MaxValue;
      double num3 = (double) b / (double) byte.MaxValue;
      double num4 = num1;
      if (num4 < num2)
        num4 = num2;
      if (num4 < num3)
        num4 = num3;
      double num5 = num1;
      if (num5 > num2)
        num5 = num2;
      if (num5 > num3)
        num5 = num3;
      double num6 = num4 - num5;
      l = (num4 + num5) / 2.0;
      if (Math.Abs(num6) < 1E-05)
      {
        s = 0.0;
        h = 0.0;
      }
      else
      {
        s = l > 0.5 ? num6 / (2.0 - num4 - num5) : num6 / (num4 + num5);
        double num7 = (num4 - num1) / num6;
        double num8 = (num4 - num2) / num6;
        double num9 = (num4 - num3) / num6;
        h = num1 != num4 ? (num2 != num4 ? 4.0 + num8 - num7 : 2.0 + num7 - num9) : num9 - num8;
        h *= 60.0;
        if (h >= 0.0)
          return;
        h += 360.0;
      }
    }

    public static void HSLtoRGB(double h, double s, double l, out int r, out int g, out int b)
    {
      double q2 = l > 0.5 ? l + s - l * s : l * (1.0 + s);
      double q1 = 2.0 * l - q2;
      double num1;
      double num2;
      double num3;
      if (s == 0.0)
      {
        num1 = l;
        num2 = l;
        num3 = l;
      }
      else
      {
        num1 = Utility.QQHtoRGB(q1, q2, h + 120.0);
        num2 = Utility.QQHtoRGB(q1, q2, h);
        num3 = Utility.QQHtoRGB(q1, q2, h - 120.0);
      }
      r = (int) (num1 * (double) byte.MaxValue);
      g = (int) (num2 * (double) byte.MaxValue);
      b = (int) (num3 * (double) byte.MaxValue);
    }

    private static double QQHtoRGB(double q1, double q2, double hue)
    {
      if (hue > 360.0)
        hue -= 360.0;
      else if (hue < 0.0)
        hue += 360.0;
      if (hue < 60.0)
        return q1 + (q2 - q1) * hue / 60.0;
      if (hue < 180.0)
        return q2;
      return hue < 240.0 ? q1 + (q2 - q1) * (240.0 - hue) / 60.0 : q1;
    }

    public static float ModifyCoordinateFromUIScale(float coordinate) => coordinate * Game1.options.uiScale / Game1.options.zoomLevel;

    public static Vector2 ModifyCoordinatesFromUIScale(Vector2 coordinates) => coordinates * Game1.options.uiScale / Game1.options.zoomLevel;

    public static float ModifyCoordinateForUIScale(float coordinate) => coordinate / Game1.options.uiScale * Game1.options.zoomLevel;

    public static Vector2 ModifyCoordinatesForUIScale(Vector2 coordinates) => coordinates / Game1.options.uiScale * Game1.options.zoomLevel;

    public static bool ShouldIgnoreValueChangeCallback() => Game1.gameMode != (byte) 3 || Game1.client != null && !Game1.client.readyToPlay || Game1.client != null && Game1.locationRequest != null;
  }
}
