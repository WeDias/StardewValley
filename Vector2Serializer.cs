// Decompiled with JetBrains decompiler
// Type: StardewValley.Vector2Serializer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace StardewValley
{
  public class Vector2Serializer : XmlSerializer
  {
    private Vector2Reader _reader = new Vector2Reader();
    private Vector2Writer _writer = new Vector2Writer();

    protected override XmlSerializationReader CreateReader() => (XmlSerializationReader) this._reader;

    protected override XmlSerializationWriter CreateWriter() => (XmlSerializationWriter) this._writer;

    public override bool CanDeserialize(XmlReader xmlReader) => xmlReader.IsStartElement("Vector2");

    protected override void Serialize(object o, XmlSerializationWriter writer) => this._writer.WriteVector2((Vector2) o);

    protected override object Deserialize(XmlSerializationReader reader) => (object) this._reader.ReadVector2();
  }
}
