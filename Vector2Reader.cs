// Decompiled with JetBrains decompiler
// Type: StardewValley.Vector2Reader
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace StardewValley
{
  public class Vector2Reader : XmlSerializationReader
  {
    public Vector2 ReadVector2()
    {
      XmlReader reader = this.Reader;
      reader.ReadStartElement("Vector2");
      reader.ReadStartElement("X");
      float x = reader.ReadContentAsFloat();
      reader.ReadEndElement();
      reader.ReadStartElement("Y");
      float y = reader.ReadContentAsFloat();
      reader.ReadEndElement();
      reader.ReadEndElement();
      return new Vector2(x, y);
    }

    protected override void InitCallbacks()
    {
    }

    protected override void InitIDs()
    {
    }
  }
}
