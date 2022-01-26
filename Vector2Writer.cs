// Decompiled with JetBrains decompiler
// Type: StardewValley.Vector2Writer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace StardewValley
{
  public class Vector2Writer : XmlSerializationWriter
  {
    public void WriteVector2(Vector2 vec)
    {
      XmlWriter writer = this.Writer;
      writer.WriteStartElement("Vector2");
      writer.WriteStartElement("X");
      writer.WriteValue(vec.X);
      writer.WriteEndElement();
      writer.WriteStartElement("Y");
      writer.WriteValue(vec.Y);
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    protected override void InitCallbacks()
    {
    }
  }
}
