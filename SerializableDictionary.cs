// Decompiled with JetBrains decompiler
// Type: StardewValley.SerializableDictionary`2
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace StardewValley
{
  [XmlRoot("dictionary")]
  public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
  {
    private static XmlSerializer _keySerializer = SaveGame.GetSerializer(typeof (TKey));
    private static XmlSerializer _valueSerializer = SaveGame.GetSerializer(typeof (TValue));

    public new void Add(TKey key, TValue value)
    {
      base.Add(key, value);
      this.OnCollectionChanged((object) this, new SerializableDictionary<TKey, TValue>.ChangeArgs(ChangeType.Add, key, value));
    }

    public new bool Remove(TKey key)
    {
      TValue v;
      if (!this.TryGetValue(key, out v))
        return false;
      base.Remove(key);
      this.OnCollectionChanged((object) this, new SerializableDictionary<TKey, TValue>.ChangeArgs(ChangeType.Remove, key, v));
      return true;
    }

    public new void Clear()
    {
      base.Clear();
      this.OnCollectionChanged((object) this, new SerializableDictionary<TKey, TValue>.ChangeArgs(ChangeType.Clear, default (TKey), default (TValue)));
    }

    public event SerializableDictionary<TKey, TValue>.ChangeCallback CollectionChanged;

    private void OnCollectionChanged(
      object sender,
      SerializableDictionary<TKey, TValue>.ChangeArgs args)
    {
      SerializableDictionary<TKey, TValue>.ChangeCallback collectionChanged = this.CollectionChanged;
      if (collectionChanged == null)
        return;
      collectionChanged(sender ?? (object) this, args);
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num != 0)
        return;
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        reader.ReadStartElement("item");
        reader.ReadStartElement("key");
        TKey key = (TKey) SerializableDictionary<TKey, TValue>._keySerializer.Deserialize(reader);
        reader.ReadEndElement();
        reader.ReadStartElement("value");
        TValue obj = (TValue) SerializableDictionary<TKey, TValue>._valueSerializer.Deserialize(reader);
        reader.ReadEndElement();
        base.Add(key, obj);
        reader.ReadEndElement();
        int content = (int) reader.MoveToContent();
      }
      reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
      foreach (TKey key in this.Keys)
      {
        writer.WriteStartElement("item");
        writer.WriteStartElement("key");
        SerializableDictionary<TKey, TValue>._keySerializer.Serialize(writer, (object) key);
        writer.WriteEndElement();
        writer.WriteStartElement("value");
        TValue o = this[key];
        SerializableDictionary<TKey, TValue>._valueSerializer.Serialize(writer, (object) o);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    public struct ChangeArgs
    {
      public readonly ChangeType Type;
      public readonly TKey Key;
      public readonly TValue Value;

      public ChangeArgs(ChangeType type, TKey k, TValue v)
      {
        this.Type = type;
        this.Key = k;
        this.Value = v;
      }
    }

    public delegate void ChangeCallback(
      object sender,
      SerializableDictionary<TKey, TValue>.ChangeArgs args);
  }
}
