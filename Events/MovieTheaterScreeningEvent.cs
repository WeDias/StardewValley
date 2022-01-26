// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.MovieTheaterScreeningEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StardewValley.Events
{
  public class MovieTheaterScreeningEvent
  {
    public int currentResponse;
    public List<List<Character>> playerAndGuestAudienceGroups;
    public Dictionary<int, Character> _responseOrder = new Dictionary<int, Character>();
    protected Dictionary<Character, Character> _whiteListDependencyLookup;
    protected Dictionary<Character, string> _characterResponses;
    public MovieData movieData;
    protected List<Farmer> _farmers;
    protected Dictionary<Character, MovieConcession> _concessionsData;

    public StardewValley.Event getMovieEvent(
      string movieID,
      List<List<Character>> player_and_guest_audience_groups,
      List<List<Character>> npcOnlyAudienceGroups,
      Dictionary<Character, MovieConcession> concessions_data = null)
    {
      this._concessionsData = concessions_data;
      this._responseOrder = new Dictionary<int, Character>();
      this._whiteListDependencyLookup = new Dictionary<Character, Character>();
      this._characterResponses = new Dictionary<Character, string>();
      this.movieData = MovieTheater.GetMovieData()[movieID];
      this.playerAndGuestAudienceGroups = player_and_guest_audience_groups;
      this.currentResponse = 0;
      StringBuilder sb = new StringBuilder();
      Random theaterRandom = new Random((int) ((long) Game1.stats.DaysPlayed + (long) (Game1.uniqueIDForThisGame / 2UL)));
      sb.Append("movieScreenAmbience/-2000 -2000/");
      string str1 = "farmer" + Utility.getFarmerNumberFromFarmer(Game1.player).ToString();
      string str2 = "";
      foreach (List<Character> guestAudienceGroup in this.playerAndGuestAudienceGroups)
      {
        if (guestAudienceGroup.Contains((Character) Game1.player))
        {
          for (int index = 0; index < guestAudienceGroup.Count; ++index)
          {
            if (!(guestAudienceGroup[index] is Farmer))
              str2 = (string) (NetFieldBase<string, NetString>) guestAudienceGroup[index].name;
          }
        }
      }
      this._farmers = new List<Farmer>();
      foreach (List<Character> guestAudienceGroup in this.playerAndGuestAudienceGroups)
      {
        foreach (Character character in guestAudienceGroup)
        {
          if (character is Farmer && !((IEnumerable<Character>) this._farmers).Contains<Character>(character))
            this._farmers.Add(character as Farmer);
        }
      }
      List<Character> list = this.playerAndGuestAudienceGroups.SelectMany<List<Character>, Character>((Func<List<Character>, IEnumerable<Character>>) (x => (IEnumerable<Character>) x)).ToList<Character>();
      list.AddRange((IEnumerable<Character>) npcOnlyAudienceGroups.SelectMany<List<Character>, Character>((Func<List<Character>, IEnumerable<Character>>) (x => (IEnumerable<Character>) x)).ToList<Character>());
      bool flag1 = true;
      foreach (Character character in list)
      {
        if (character != null)
        {
          if (!flag1)
            sb.Append(" ");
          if (character is Farmer)
          {
            Farmer who = character as Farmer;
            sb.Append("farmer" + Utility.getFarmerNumberFromFarmer(who).ToString());
          }
          else if ((string) (NetFieldBase<string, NetString>) character.name == "Krobus")
            sb.Append("Krobus_Trenchcoat");
          else
            sb.Append((string) (NetFieldBase<string, NetString>) character.name);
          sb.Append(" -1000 -1000 0");
          flag1 = false;
        }
      }
      sb.Append("/changeToTemporaryMap MovieTheaterScreen false/specificTemporarySprite movieTheater_setup/ambientLight 0 0 0/");
      string[] strArray1 = new string[8];
      this.playerAndGuestAudienceGroups = this.playerAndGuestAudienceGroups.OrderBy<List<Character>, int>((Func<List<Character>, int>) (x => theaterRandom.Next())).ToList<List<Character>>();
      int num1 = theaterRandom.Next(8 - this.playerAndGuestAudienceGroups.SelectMany<List<Character>, Character>((Func<List<Character>, IEnumerable<Character>>) (x => (IEnumerable<Character>) x)).Count<Character>() + 1);
      int index1 = 0;
      int num2;
      for (int index2 = 0; index2 < 8; ++index2)
      {
        int num3 = (index2 + num1) % 8;
        if (this.playerAndGuestAudienceGroups[index1].Count == 2 && (num3 == 3 || num3 == 7))
        {
          ++index2;
          num3 = (num3 + 1) % 8;
        }
        for (int index3 = 0; index3 < this.playerAndGuestAudienceGroups[index1].Count && num3 + index3 < strArray1.Length; ++index3)
        {
          string[] strArray2 = strArray1;
          int index4 = num3 + index3;
          string str3;
          if (!(this.playerAndGuestAudienceGroups[index1][index3] is Farmer))
          {
            str3 = (string) (NetFieldBase<string, NetString>) this.playerAndGuestAudienceGroups[index1][index3].name;
          }
          else
          {
            num2 = Utility.getFarmerNumberFromFarmer(this.playerAndGuestAudienceGroups[index1][index3] as Farmer);
            str3 = "farmer" + num2.ToString();
          }
          strArray2[index4] = str3;
          if (index3 > 0)
            ++index2;
        }
        ++index1;
        if (index1 >= this.playerAndGuestAudienceGroups.Count)
          break;
      }
      string[] strArray3 = new string[6];
      for (int index5 = 0; index5 < npcOnlyAudienceGroups.Count; ++index5)
      {
        int num4 = theaterRandom.Next(3 - npcOnlyAudienceGroups[index5].Count + 1) + index5 * 3;
        for (int index6 = 0; index6 < npcOnlyAudienceGroups[index5].Count; ++index6)
          strArray3[num4 + index6] = (string) (NetFieldBase<string, NetString>) npcOnlyAudienceGroups[index5][index6].name;
      }
      int num5 = 0;
      int num6 = 0;
      for (int index7 = 0; index7 < strArray1.Length; ++index7)
      {
        if (strArray1[index7] != null && strArray1[index7] != "" && strArray1[index7] != str1 && strArray1[index7] != str2)
        {
          ++num5;
          if (num5 >= 2)
          {
            ++num6;
            Point seatTileFromIndex = this.getBackRowSeatTileFromIndex(index7);
            sb.Append("warp ").Append(strArray1[index7]).Append(" ").Append(seatTileFromIndex.X).Append(" ").Append(seatTileFromIndex.Y).Append("/positionOffset ").Append(strArray1[index7]).Append(" 0 -10/");
            if (num6 == 2)
            {
              num6 = 0;
              if (theaterRandom.NextDouble() < 0.5 && strArray1[index7] != str2 && strArray1[index7 - 1] != str2)
              {
                sb.Append("faceDirection " + strArray1[index7] + " 3 true/");
                sb.Append("faceDirection " + strArray1[index7 - 1] + " 1 true/");
              }
            }
          }
        }
      }
      int num7 = 0;
      int num8 = 0;
      for (int index8 = 0; index8 < strArray3.Length; ++index8)
      {
        if (strArray3[index8] != null && strArray3[index8] != "")
        {
          ++num7;
          if (num7 >= 2)
          {
            ++num8;
            Point seatTileFromIndex = this.getMidRowSeatTileFromIndex(index8);
            sb.Append("warp ").Append(strArray3[index8]).Append(" ").Append(seatTileFromIndex.X).Append(" ").Append(seatTileFromIndex.Y).Append("/positionOffset ").Append(strArray3[index8]).Append(" 0 -10/");
            if (num8 == 2)
            {
              num8 = 0;
              if (index8 != 3 && theaterRandom.NextDouble() < 0.5)
              {
                sb.Append("faceDirection " + strArray3[index8] + " 3 true/");
                sb.Append("faceDirection " + strArray3[index8 - 1] + " 1 true/");
              }
            }
          }
        }
      }
      Point point = new Point(1, 15);
      int num9 = 0;
      for (int index9 = 0; index9 < strArray1.Length; ++index9)
      {
        if (strArray1[index9] != null && strArray1[index9] != "" && strArray1[index9] != str1 && strArray1[index9] != str2)
        {
          Point seatTileFromIndex = this.getBackRowSeatTileFromIndex(index9);
          if (num9 == 1)
          {
            StringBuilder stringBuilder = sb.Append("warp ").Append(strArray1[index9]).Append(" ").Append(seatTileFromIndex.X - 1).Append(" 10").Append("/advancedMove ").Append(strArray1[index9]);
            num2 = 200;
            string str4 = " false 1 " + num2.ToString() + " 1 0 4 1000/";
            stringBuilder.Append(str4).Append("positionOffset ").Append(strArray1[index9]).Append(" 0 -10/");
          }
          else
            sb.Append("warp ").Append(strArray1[index9]).Append(" 1 12").Append("/advancedMove ").Append(strArray1[index9]).Append(" false 1 200 ").Append("0 -2 ").Append(seatTileFromIndex.X - 1).Append(" 0 4 1000/").Append("positionOffset ").Append(strArray1[index9]).Append(" 0 -10/");
          ++num9;
        }
        if (num9 >= 2)
          break;
      }
      int num10 = 0;
      for (int index10 = 0; index10 < strArray3.Length; ++index10)
      {
        if (strArray3[index10] != null && strArray3[index10] != "")
        {
          Point seatTileFromIndex = this.getMidRowSeatTileFromIndex(index10);
          if (num10 == 1)
          {
            StringBuilder stringBuilder = sb.Append("warp ").Append(strArray3[index10]).Append(" ").Append(seatTileFromIndex.X - 1).Append(" 8").Append("/advancedMove ").Append(strArray3[index10]);
            num2 = 400;
            string str5 = " false 1 " + num2.ToString() + " 1 0 4 1000/";
            stringBuilder.Append(str5);
          }
          else
            sb.Append("warp ").Append(strArray3[index10]).Append(" 2 9").Append("/advancedMove ").Append(strArray3[index10]).Append(" false 1 300 ").Append("0 -1 ").Append(seatTileFromIndex.X - 2).Append(" 0 4 1000/");
          ++num10;
        }
        if (num10 >= 2)
          break;
      }
      sb.Append("viewport 6 8 true/pause 500/");
      for (int index11 = 0; index11 < strArray1.Length; ++index11)
      {
        if (strArray1[index11] != null && strArray1[index11] != "")
        {
          Point seatTileFromIndex = this.getBackRowSeatTileFromIndex(index11);
          if (strArray1[index11] == str1 || strArray1[index11] == str2)
            sb.Append("warp ").Append(strArray1[index11]).Append(" ").Append(point.X).Append(" ").Append(point.Y).Append("/advancedMove ").Append(strArray1[index11]).Append(" false 0 -5 ").Append(seatTileFromIndex.X - point.X).Append(" 0 4 1000/").Append("pause ").Append(1000).Append("/");
        }
      }
      sb.Append("pause 3000/proceedPosition ").Append(str2).Append("/pause 1000");
      if (str2.Equals(""))
        sb.Append("/proceedPosition farmer");
      sb.Append("/waitForAllStationary/pause 100");
      foreach (Character c in list)
      {
        if (MovieTheaterScreeningEvent.getEventName(c) != str1 && MovieTheaterScreeningEvent.getEventName(c) != str2)
        {
          if (c is Farmer)
            sb.Append("/faceDirection ").Append(MovieTheaterScreeningEvent.getEventName(c)).Append(" 0 true/positionOffset ").Append(MovieTheaterScreeningEvent.getEventName(c)).Append(" 0 42 true");
          else
            sb.Append("/faceDirection ").Append(MovieTheaterScreeningEvent.getEventName(c)).Append(" 0 true/positionOffset ").Append(MovieTheaterScreeningEvent.getEventName(c)).Append(" 0 12 true");
          if (theaterRandom.NextDouble() < 0.2)
            sb.Append("/pause 100");
        }
      }
      sb.Append("/positionOffset ").Append(str1).Append(" 0 32/positionOffset ").Append(str2).Append(" 0 8/ambientLight 210 210 120 true/pause 500/viewport move 0 -1 4000/pause 5000");
      List<Character> characterList = new List<Character>();
      foreach (List<Character> guestAudienceGroup in this.playerAndGuestAudienceGroups)
      {
        foreach (Character character in guestAudienceGroup)
        {
          if (!(character is Farmer) && !characterList.Contains(character))
            characterList.Add(character);
        }
      }
      for (int index12 = 0; index12 < characterList.Count; ++index12)
      {
        int index13 = theaterRandom.Next(characterList.Count);
        Character character = characterList[index12];
        characterList[index12] = characterList[index13];
        characterList[index13] = character;
      }
      int key1 = 0;
      foreach (MovieScene scene in this.movieData.Scenes)
      {
        if (scene.ResponsePoint != null)
        {
          bool flag2 = false;
          for (int index14 = 0; index14 < characterList.Count; ++index14)
          {
            MovieCharacterReaction reactionsForCharacter = MovieTheater.GetReactionsForCharacter(characterList[index14] as NPC);
            if (reactionsForCharacter != null)
            {
              foreach (MovieReaction reaction in reactionsForCharacter.Reactions)
              {
                if (reaction.ShouldApplyToMovie(this.movieData, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(characterList[index14] as NPC)) && reaction.SpecialResponses != null && reaction.SpecialResponses.DuringMovie != null && (reaction.SpecialResponses.DuringMovie.ResponsePoint == scene.ResponsePoint || reaction.Whitelist.Count > 0))
                {
                  if (!this._whiteListDependencyLookup.ContainsKey(characterList[index14]))
                  {
                    this._responseOrder[key1] = characterList[index14];
                    if (reaction.Whitelist != null)
                    {
                      for (int index15 = 0; index15 < reaction.Whitelist.Count; ++index15)
                      {
                        Character characterFromName = (Character) Game1.getCharacterFromName(reaction.Whitelist[index15]);
                        if (characterFromName != null)
                        {
                          this._whiteListDependencyLookup[characterFromName] = characterList[index14];
                          foreach (int key2 in this._responseOrder.Keys)
                          {
                            if (this._responseOrder[key2] == characterFromName)
                              this._responseOrder.Remove(key2);
                          }
                        }
                      }
                    }
                  }
                  characterList.RemoveAt(index14);
                  --index14;
                  flag2 = true;
                  break;
                }
              }
              if (flag2)
                break;
            }
          }
          if (!flag2)
          {
            for (int index16 = 0; index16 < characterList.Count; ++index16)
            {
              MovieCharacterReaction reactionsForCharacter = MovieTheater.GetReactionsForCharacter(characterList[index16] as NPC);
              if (reactionsForCharacter != null)
              {
                foreach (MovieReaction reaction in reactionsForCharacter.Reactions)
                {
                  if (reaction.ShouldApplyToMovie(this.movieData, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(characterList[index16] as NPC)) && reaction.SpecialResponses != null && reaction.SpecialResponses.DuringMovie != null && reaction.SpecialResponses.DuringMovie.ResponsePoint == key1.ToString())
                  {
                    if (!this._whiteListDependencyLookup.ContainsKey(characterList[index16]))
                    {
                      this._responseOrder[key1] = characterList[index16];
                      if (reaction.Whitelist != null)
                      {
                        for (int index17 = 0; index17 < reaction.Whitelist.Count; ++index17)
                        {
                          Character characterFromName = (Character) Game1.getCharacterFromName(reaction.Whitelist[index17]);
                          if (characterFromName != null)
                          {
                            this._whiteListDependencyLookup[characterFromName] = characterList[index16];
                            foreach (int key3 in this._responseOrder.Keys)
                            {
                              if (this._responseOrder[key3] == characterFromName)
                                this._responseOrder.Remove(key3);
                            }
                          }
                        }
                      }
                    }
                    characterList.RemoveAt(index16);
                    --index16;
                    flag2 = true;
                    break;
                  }
                }
                if (flag2)
                  break;
              }
            }
          }
          ++key1;
        }
      }
      int key4 = 0;
      for (int index18 = 0; index18 < characterList.Count; ++index18)
      {
        if (!this._whiteListDependencyLookup.ContainsKey(characterList[index18]))
        {
          while (this._responseOrder.ContainsKey(key4))
            ++key4;
          this._responseOrder[key4] = characterList[index18];
          ++key4;
        }
      }
      foreach (MovieScene scene in this.movieData.Scenes)
        this._ParseScene(sb, scene);
      while (this.currentResponse < this._responseOrder.Count)
        this._ParseResponse(sb);
      sb.Append("/stopMusic");
      sb.Append("/fade/viewport -1000 -1000");
      sb.Append("/pause 500/message \"" + Game1.content.LoadString("Strings\\Locations:Theater_MovieEnd") + "\"/pause 500");
      sb.Append("/requestMovieEnd");
      Console.WriteLine(sb.ToString());
      return new StardewValley.Event(sb.ToString());
    }

    protected void _ParseScene(StringBuilder sb, MovieScene scene)
    {
      if (scene.Sound != "")
        sb.Append("/playSound " + scene.Sound);
      if (scene.Music != "")
        sb.Append("/playMusic " + scene.Music);
      if (scene.MessageDelay > 0)
        sb.Append("/pause " + scene.MessageDelay.ToString());
      if (scene.Image >= 0)
        sb.Append("/specificTemporarySprite movieTheater_screen " + this.movieData.SheetIndex.ToString() + " " + scene.Image.ToString() + " " + scene.Shake.ToString());
      if (scene.Script != "")
        sb.Append(scene.Script);
      if (scene.Text != "")
        sb.Append("/message \"" + scene.Text + "\"");
      if (scene.ResponsePoint == null)
        return;
      this._ParseResponse(sb, scene);
    }

    protected void _ParseResponse(StringBuilder sb, MovieScene scene = null)
    {
      if (this._responseOrder.ContainsKey(this.currentResponse))
      {
        sb.Append("/pause 500");
        Character character = this._responseOrder[this.currentResponse];
        bool ignoreScript = false;
        if (!this._whiteListDependencyLookup.ContainsKey(character))
        {
          MovieCharacterReaction reactionsForCharacter = MovieTheater.GetReactionsForCharacter(character as NPC);
          if (reactionsForCharacter != null)
          {
            foreach (MovieReaction reaction in reactionsForCharacter.Reactions)
            {
              if (reaction.ShouldApplyToMovie(this.movieData, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(character as NPC)) && reaction.SpecialResponses != null && reaction.SpecialResponses.DuringMovie != null && (reaction.SpecialResponses.DuringMovie.ResponsePoint == null || reaction.SpecialResponses.DuringMovie.ResponsePoint == "" || scene != null && reaction.SpecialResponses.DuringMovie.ResponsePoint == scene.ResponsePoint || reaction.SpecialResponses.DuringMovie.ResponsePoint == this.currentResponse.ToString() || reaction.Whitelist.Count > 0))
              {
                if (reaction.SpecialResponses.DuringMovie.Script != "")
                {
                  sb.Append(reaction.SpecialResponses.DuringMovie.Script);
                  ignoreScript = true;
                }
                if (reaction.SpecialResponses.DuringMovie.Text != "")
                {
                  sb.Append("/speak " + (string) (NetFieldBase<string, NetString>) character.name + " \"" + reaction.SpecialResponses.DuringMovie.Text + "\"");
                  break;
                }
                break;
              }
            }
          }
        }
        this._ParseCharacterResponse(sb, character, ignoreScript);
        foreach (Character key in this._whiteListDependencyLookup.Keys)
        {
          if (this._whiteListDependencyLookup[key] == character)
            this._ParseCharacterResponse(sb, key);
        }
      }
      ++this.currentResponse;
    }

    protected void _ParseCharacterResponse(
      StringBuilder sb,
      Character responding_character,
      bool ignoreScript = false)
    {
      string responseForMovie = MovieTheater.GetResponseForMovie(responding_character as NPC);
      if (this._whiteListDependencyLookup.ContainsKey(responding_character))
        responseForMovie = MovieTheater.GetResponseForMovie(this._whiteListDependencyLookup[responding_character] as NPC);
      if (!(responseForMovie == "love"))
      {
        if (!(responseForMovie == "like"))
        {
          if (responseForMovie == "dislike")
          {
            sb.Append("/friendship " + responding_character.Name + " " + 0.ToString());
            if (!ignoreScript)
              sb.Append("/playSound newArtifact/emote " + (string) (NetFieldBase<string, NetString>) responding_character.name + " " + 24.ToString() + "/message \"" + Game1.content.LoadString("Strings\\Characters:MovieTheater_DislikeMovie", (object) responding_character.displayName) + "\"");
          }
        }
        else
        {
          sb.Append("/friendship " + responding_character.Name + " " + 100.ToString());
          if (!ignoreScript)
            sb.Append("/playSound give_gift/emote " + (string) (NetFieldBase<string, NetString>) responding_character.name + " " + 56.ToString() + "/message \"" + Game1.content.LoadString("Strings\\Characters:MovieTheater_LikeMovie", (object) responding_character.displayName) + "\"");
        }
      }
      else
      {
        sb.Append("/friendship " + responding_character.Name + " " + 200.ToString());
        if (!ignoreScript)
          sb.Append("/playSound reward/emote " + (string) (NetFieldBase<string, NetString>) responding_character.name + " " + 20.ToString() + "/message \"" + Game1.content.LoadString("Strings\\Characters:MovieTheater_LoveMovie", (object) responding_character.displayName) + "\"");
      }
      if (this._concessionsData != null && this._concessionsData.ContainsKey(responding_character))
      {
        MovieConcession concession = this._concessionsData[responding_character];
        string tasteForCharacter = MovieTheater.GetConcessionTasteForCharacter(responding_character, concession);
        string str1 = "";
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
        if (dictionary.ContainsKey((string) (NetFieldBase<string, NetString>) responding_character.name))
        {
          string[] strArray = dictionary[(string) (NetFieldBase<string, NetString>) responding_character.name].Split('/');
          if (strArray[4] == "female")
            str1 = "_Female";
          else if (strArray[4] == "male")
            str1 = "_Male";
        }
        string str2 = "eat";
        if (concession.tags != null && concession.tags.Contains("Drink"))
          str2 = "gulp";
        if (!(tasteForCharacter == "love"))
        {
          if (!(tasteForCharacter == "like"))
          {
            if (tasteForCharacter == "dislike")
            {
              sb.Append("/friendship " + responding_character.Name + " " + 0.ToString());
              sb.Append("/playSound croak/pause 1000");
              sb.Append("/playSound newArtifact/emote " + (string) (NetFieldBase<string, NetString>) responding_character.name + " " + 40.ToString() + "/message \"" + Game1.content.LoadString("Strings\\Characters:MovieTheater_DislikeConcession" + str1, (object) responding_character.displayName, (object) concession.DisplayName) + "\"");
            }
          }
          else
          {
            sb.Append("/friendship " + responding_character.Name + " " + 25.ToString());
            sb.Append("/tossConcession " + responding_character.Name + " " + concession.id.ToString() + "/pause 1000");
            sb.Append("/playSound " + str2 + "/shake " + responding_character.Name + " 500/pause 1000");
            sb.Append("/playSound give_gift/emote " + (string) (NetFieldBase<string, NetString>) responding_character.name + " " + 56.ToString() + "/message \"" + Game1.content.LoadString("Strings\\Characters:MovieTheater_LikeConcession" + str1, (object) responding_character.displayName, (object) concession.DisplayName) + "\"");
          }
        }
        else
        {
          sb.Append("/friendship " + responding_character.Name + " " + 50.ToString());
          sb.Append("/tossConcession " + responding_character.Name + " " + concession.id.ToString() + "/pause 1000");
          sb.Append("/playSound " + str2 + "/shake " + responding_character.Name + " 500/pause 1000");
          sb.Append("/playSound reward/emote " + (string) (NetFieldBase<string, NetString>) responding_character.name + " " + 20.ToString() + "/message \"" + Game1.content.LoadString("Strings\\Characters:MovieTheater_LoveConcession" + str1, (object) responding_character.displayName, (object) concession.DisplayName) + "\"");
        }
      }
      this._characterResponses[responding_character] = responseForMovie;
    }

    public Dictionary<Character, string> GetCharacterResponses() => this._characterResponses;

    private static string getEventName(Character c) => c is Farmer ? "farmer" + Utility.getFarmerNumberFromFarmer(c as Farmer).ToString() : (string) (NetFieldBase<string, NetString>) c.name;

    private Point getBackRowSeatTileFromIndex(int index)
    {
      switch (index)
      {
        case 0:
          return new Point(2, 10);
        case 1:
          return new Point(3, 10);
        case 2:
          return new Point(4, 10);
        case 3:
          return new Point(5, 10);
        case 4:
          return new Point(8, 10);
        case 5:
          return new Point(9, 10);
        case 6:
          return new Point(10, 10);
        case 7:
          return new Point(11, 10);
        default:
          return new Point(4, 12);
      }
    }

    private Point getMidRowSeatTileFromIndex(int index)
    {
      switch (index)
      {
        case 0:
          return new Point(3, 8);
        case 1:
          return new Point(4, 8);
        case 2:
          return new Point(5, 8);
        case 3:
          return new Point(8, 8);
        case 4:
          return new Point(9, 8);
        case 5:
          return new Point(10, 8);
        default:
          return new Point(4, 12);
      }
    }
  }
}
