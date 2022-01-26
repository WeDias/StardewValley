// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.BotchedNetField`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace StardewValley.Characters
{
  public class BotchedNetField<T, TNet> : IXmlSerializable where TNet : NetField<T, TNet>
  {
    [XmlIgnore]
    public TNet netField;

    public BotchedNetField()
    {
    }

    public BotchedNetField(TNet net_field) => this.netField = net_field;

    public T Value
    {
      get => this.netField.Value;
      set => this.netField.Value = value;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void WriteXml(XmlWriter writer) => writer.WriteValue((object) this.netField.Value);

    protected virtual object _ParseValue(XmlReader reader) => (object) null;

    public void ReadXml(XmlReader reader)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num != 0)
        return;
      if (reader.Value != null && reader.Value != "")
      {
        this.netField.Value = (T) this._ParseValue(reader);
      }
      else
      {
        XmlReader reader1 = reader.ReadSubtree();
        if (reader1 != null)
        {
          int content = (int) reader1.MoveToContent();
          reader1.Read();
          this.netField.Value = (T) this._ParseValue(reader1);
          while (reader.NodeType != XmlNodeType.EndElement)
            reader.Read();
          reader.ReadEndElement();
        }
      }
      while (reader.NodeType != XmlNodeType.EndElement)
        reader.Read();
      reader.ReadEndElement();
    }
  }
}
