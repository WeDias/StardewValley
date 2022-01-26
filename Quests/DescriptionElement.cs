// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.DescriptionElement
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class DescriptionElement : INetObject<NetFields>
  {
    public static XmlSerializer serializer = new XmlSerializer(typeof (DescriptionElement), new Type[3]
    {
      typeof (Monster),
      typeof (NPC),
      typeof (StardewValley.Object)
    });
    public string xmlKey;
    public List<object> param;

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public static implicit operator DescriptionElement(string key) => new DescriptionElement(key);

    public DescriptionElement()
    {
      this.xmlKey = string.Empty;
      this.param = new List<object>();
    }

    public DescriptionElement(string key)
    {
      this.xmlKey = key;
      this.param = new List<object>();
    }

    public DescriptionElement(string key, object param1)
    {
      this.xmlKey = key;
      this.param = new List<object>();
      this.param.Add(param1);
    }

    public DescriptionElement(string key, List<object> paramlist)
    {
      this.xmlKey = key;
      this.param = new List<object>();
      foreach (object obj in paramlist)
        this.param.Add(obj);
    }

    public DescriptionElement(string key, object param1, object param2)
    {
      this.xmlKey = key;
      this.param = new List<object>();
      this.param.Add(param1);
      this.param.Add(param2);
    }

    public DescriptionElement(string key, object param1, object param2, object param3)
    {
      this.xmlKey = key;
      this.param = new List<object>();
      this.param.Add(param1);
      this.param.Add(param2);
      this.param.Add(param3);
    }

    public string loadDescriptionElement()
    {
      DescriptionElement descriptionElement1 = new DescriptionElement(this.xmlKey, this.param);
      for (int index = 0; index < descriptionElement1.param.Count; ++index)
      {
        if (descriptionElement1.param[index] is DescriptionElement)
        {
          DescriptionElement descriptionElement2 = descriptionElement1.param[index] as DescriptionElement;
          descriptionElement1.param[index] = (object) descriptionElement2.loadDescriptionElement();
        }
        if (descriptionElement1.param[index] is StardewValley.Object)
        {
          string str;
          Game1.objectInformation.TryGetValue((int) (NetFieldBase<int, NetInt>) (descriptionElement1.param[index] as StardewValley.Object).parentSheetIndex, out str);
          descriptionElement1.param[index] = (object) str.Split('/')[4];
        }
        if (descriptionElement1.param[index] is Monster)
        {
          DescriptionElement descriptionElement3;
          if ((descriptionElement1.param[index] as Monster).name.Equals((object) "Frost Jelly"))
          {
            descriptionElement3 = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13772");
            descriptionElement1.param[index] = (object) descriptionElement3.loadDescriptionElement();
          }
          else
          {
            descriptionElement3 = new DescriptionElement("Data\\Monsters:" + (string) (NetFieldBase<string, NetString>) (descriptionElement1.param[index] as Monster).name);
            descriptionElement1.param[index] = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? (object) (((IEnumerable<string>) descriptionElement3.loadDescriptionElement().Split('/')).Last<string>() + "s") : (object) ((IEnumerable<string>) descriptionElement3.loadDescriptionElement().Split('/')).Last<string>();
          }
          descriptionElement1.param[index] = (object) ((IEnumerable<string>) descriptionElement3.loadDescriptionElement().Split('/')).Last<string>();
        }
        if (descriptionElement1.param[index] is NPC)
        {
          DescriptionElement descriptionElement4 = new DescriptionElement("Data\\NPCDispositions:" + (string) (NetFieldBase<string, NetString>) (descriptionElement1.param[index] as NPC).name);
          descriptionElement1.param[index] = (object) ((IEnumerable<string>) descriptionElement4.loadDescriptionElement().Split('/')).Last<string>();
        }
      }
      if (descriptionElement1.xmlKey == "")
        return string.Empty;
      string str1;
      switch (descriptionElement1.param.Count)
      {
        case 1:
          str1 = Game1.content.LoadString(descriptionElement1.xmlKey, descriptionElement1.param[0]);
          break;
        case 2:
          str1 = Game1.content.LoadString(descriptionElement1.xmlKey, descriptionElement1.param[0], descriptionElement1.param[1]);
          break;
        case 3:
          str1 = Game1.content.LoadString(descriptionElement1.xmlKey, descriptionElement1.param[0], descriptionElement1.param[1], descriptionElement1.param[2]);
          break;
        case 4:
          str1 = Game1.content.LoadString(descriptionElement1.xmlKey, descriptionElement1.param[0], descriptionElement1.param[1], descriptionElement1.param[2], descriptionElement1.param[3]);
          break;
        default:
          str1 = Game1.content.LoadString(descriptionElement1.xmlKey);
          if (this.xmlKey.Contains("Dialogue.cs.7") || this.xmlKey.Contains("Dialogue.cs.8"))
          {
            string str2 = Game1.content.LoadString(descriptionElement1.xmlKey).Replace("/", " ");
            str1 = str2[0] == ' ' ? str2.Substring(1) : str2;
            break;
          }
          break;
      }
      return str1;
    }
  }
}
