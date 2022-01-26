// Decompiled with JetBrains decompiler
// Type: StardewValley.StartMovieEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.IO;

namespace StardewValley
{
  public class StartMovieEvent : NetEventArg
  {
    public long uid;
    public List<List<Character>> playerGroups;
    public List<List<Character>> npcGroups;

    public StartMovieEvent()
    {
    }

    public StartMovieEvent(
      long farmer_uid,
      List<List<Character>> player_groups,
      List<List<Character>> npc_groups)
    {
      this.uid = farmer_uid;
      this.playerGroups = player_groups;
      this.npcGroups = npc_groups;
    }

    public void Read(BinaryReader reader)
    {
      this.uid = reader.ReadInt64();
      this.playerGroups = this.ReadCharacterList(reader);
      this.npcGroups = this.ReadCharacterList(reader);
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.uid);
      this.WriteCharacterList(writer, this.playerGroups);
      this.WriteCharacterList(writer, this.npcGroups);
    }

    public List<List<Character>> ReadCharacterList(BinaryReader reader)
    {
      List<List<Character>> characterListList = new List<List<Character>>();
      int num1 = reader.ReadInt32();
      for (int index1 = 0; index1 < num1; ++index1)
      {
        List<Character> characterList = new List<Character>();
        int num2 = reader.ReadInt32();
        for (int index2 = 0; index2 < num2; ++index2)
        {
          Character character = reader.ReadInt32() != 1 ? (Character) Game1.getCharacterFromName(reader.ReadString()) : (Character) Game1.getFarmer(reader.ReadInt64());
          characterList.Add(character);
        }
        characterListList.Add(characterList);
      }
      return characterListList;
    }

    public void WriteCharacterList(BinaryWriter writer, List<List<Character>> group_list)
    {
      writer.Write(group_list.Count);
      foreach (List<Character> group in group_list)
      {
        writer.Write(group.Count);
        foreach (Character character in group)
        {
          if (character is Farmer)
          {
            writer.Write(1);
            writer.Write((character as Farmer).UniqueMultiplayerID);
          }
          else
          {
            writer.Write(0);
            writer.Write(character.Name);
          }
        }
      }
    }
  }
}
