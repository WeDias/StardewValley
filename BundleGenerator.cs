// Decompiled with JetBrains decompiler
// Type: StardewValley.BundleGenerator
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StardewValley
{
  public class BundleGenerator
  {
    public List<RandomBundleData> randomBundleData;
    public Dictionary<string, string> bundleData;
    public Dictionary<string, int> itemNameLookup;
    public Random random;

    public Dictionary<string, string> Generate(string bundle_data_path, Random rng)
    {
      this.random = rng;
      this.randomBundleData = Game1.content.Load<List<RandomBundleData>>(bundle_data_path);
      this.bundleData = new Dictionary<string, string>();
      Dictionary<string, string> dictionary1 = Game1.content.LoadBase<Dictionary<string, string>>("Data\\Bundles");
      foreach (string key in dictionary1.Keys)
        this.bundleData[key] = dictionary1[key];
      foreach (RandomBundleData randomBundleData in this.randomBundleData)
      {
        List<int> intList = new List<int>();
        string[] strArray1 = randomBundleData.Keys.Trim().Split(' ');
        Dictionary<int, BundleData> dictionary2 = new Dictionary<int, BundleData>();
        foreach (string s in strArray1)
          intList.Add(int.Parse(s));
        BundleSetData random1 = Utility.GetRandom<BundleSetData>(randomBundleData.BundleSets, this.random);
        if (random1 != null)
        {
          foreach (BundleData bundle in random1.Bundles)
            dictionary2[bundle.Index] = bundle;
        }
        List<BundleData> bundleDataList = new List<BundleData>();
        foreach (BundleData bundle in randomBundleData.Bundles)
          bundleDataList.Add(bundle);
        for (int key = 0; key < intList.Count; ++key)
        {
          if (!dictionary2.ContainsKey(key))
          {
            List<BundleData> list = new List<BundleData>();
            foreach (BundleData bundleData in bundleDataList)
            {
              if (bundleData.Index == key)
                list.Add(bundleData);
            }
            if (list.Count > 0)
            {
              BundleData random2 = Utility.GetRandom<BundleData>(list, this.random);
              bundleDataList.Remove(random2);
              dictionary2[key] = random2;
            }
            else
            {
              foreach (BundleData bundleData in bundleDataList)
              {
                if (bundleData.Index == -1)
                  list.Add(bundleData);
              }
              if (list.Count > 0)
              {
                BundleData random3 = Utility.GetRandom<BundleData>(list, this.random);
                bundleDataList.Remove(random3);
                dictionary2[key] = random3;
              }
            }
          }
        }
        foreach (int key in dictionary2.Keys)
        {
          BundleData bundleData = dictionary2[key];
          StringBuilder builder = new StringBuilder();
          builder.Append(bundleData.Name);
          builder.Append("/");
          string str = bundleData.Reward;
          if (str.Length > 0)
          {
            try
            {
              if (char.IsDigit(str[0]))
              {
                string[] strArray2 = str.Split(' ');
                int stack_count = int.Parse(strArray2[0]);
                Item obj = Utility.fuzzyItemSearch(string.Join(" ", strArray2, 1, strArray2.Length - 1), stack_count);
                if (obj != null)
                  str = Utility.getStandardDescriptionFromItem(obj, obj.Stack);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine("ERROR: Malformed reward string in bundle: " + str);
              str = bundleData.Reward;
            }
          }
          builder.Append(str);
          builder.Append("/");
          int color = 0;
          if (bundleData.Color == "Red")
            color = 4;
          else if (bundleData.Color == "Blue")
            color = 5;
          else if (bundleData.Color == "Green")
            color = 0;
          else if (bundleData.Color == "Orange")
            color = 2;
          else if (bundleData.Color == "Purple")
            color = 1;
          else if (bundleData.Color == "Teal")
            color = 6;
          else if (bundleData.Color == "Yellow")
            color = 3;
          this.ParseItemList(builder, bundleData.Items, bundleData.Pick, bundleData.RequiredItems, color);
          builder.Append("/");
          builder.Append(bundleData.Sprite);
          this.bundleData[randomBundleData.AreaName + "/" + intList[key].ToString()] = builder.ToString();
        }
      }
      return this.bundleData;
    }

    public string ParseRandomTags(string data)
    {
      int startIndex;
      do
      {
        startIndex = data.LastIndexOf('[');
        if (startIndex >= 0)
        {
          int num = data.IndexOf(']', startIndex);
          if (num == -1)
            return data;
          string random = Utility.GetRandom<string>(new List<string>((IEnumerable<string>) data.Substring(startIndex + 1, num - startIndex - 1).Split('|')), this.random);
          data = data.Remove(startIndex, num - startIndex + 1);
          data = data.Insert(startIndex, random);
        }
      }
      while (startIndex >= 0);
      return data;
    }

    public Item ParseItemString(string item_string)
    {
      string[] strArray = item_string.Trim().Split(' ');
      int index = 0;
      int initialStack = int.Parse(strArray[index]);
      int startIndex = index + 1;
      int num = 0;
      if (strArray[startIndex] == "NQ")
      {
        num = 0;
        ++startIndex;
      }
      else if (strArray[startIndex] == "SQ")
      {
        num = 1;
        ++startIndex;
      }
      else if (strArray[startIndex] == "GQ")
      {
        num = 2;
        ++startIndex;
      }
      else if (strArray[startIndex] == "IQ")
      {
        num = 3;
        ++startIndex;
      }
      string str = string.Join(" ", strArray, startIndex, strArray.Length - startIndex);
      if (char.IsDigit(str[0]))
      {
        Object itemString = new Object(int.Parse(str), initialStack);
        (itemString as Object).Quality = num;
        return (Item) itemString;
      }
      Item itemString1 = (Item) null;
      if (str.ToLowerInvariant().EndsWith("category"))
      {
        try
        {
          FieldInfo field = typeof (Object).GetField(str);
          if (field != (FieldInfo) null)
            itemString1 = (Item) new Object(Vector2.Zero, (int) field.GetValue((object) null), 1);
        }
        catch (Exception ex)
        {
        }
      }
      if (itemString1 == null)
      {
        itemString1 = Utility.fuzzyItemSearch(str);
        if (itemString1 is Object)
          (itemString1 as Object).Quality = num;
      }
      if (itemString1 == null)
        throw new Exception("Invalid item name '" + str + "' encountered while generating a bundle.");
      itemString1.Stack = initialStack;
      return itemString1;
    }

    public void ParseItemList(
      StringBuilder builder,
      string item_list,
      int pick_count,
      int required_items,
      int color)
    {
      item_list = this.ParseRandomTags(item_list);
      string[] strArray = item_list.Split(',');
      List<string> stringList = new List<string>();
      for (int index = 0; index < strArray.Length; ++index)
      {
        Item itemString = this.ParseItemString(strArray[index]);
        stringList.Add(itemString.ParentSheetIndex.ToString() + " " + itemString.Stack.ToString() + " " + (itemString as Object).Quality.ToString());
      }
      if (pick_count < 0)
        pick_count = stringList.Count;
      if (required_items < 0)
        required_items = pick_count;
      while (stringList.Count > pick_count)
      {
        int index = this.random.Next(stringList.Count);
        stringList.RemoveAt(index);
      }
      for (int index = 0; index < stringList.Count; ++index)
      {
        builder.Append(stringList[index]);
        if (index < stringList.Count - 1)
          builder.Append(" ");
      }
      builder.Append("/");
      builder.Append(color);
      builder.Append("/");
      builder.Append(required_items);
    }
  }
}
