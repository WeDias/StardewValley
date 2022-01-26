// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.OutgoingMessage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace StardewValley.Network
{
  public struct OutgoingMessage
  {
    private byte messageType;
    private long farmerID;
    private object[] data;

    public byte MessageType => this.messageType;

    public long FarmerID => this.farmerID;

    public Farmer SourceFarmer => Game1.getFarmer(this.farmerID);

    public ReadOnlyCollection<object> Data => Array.AsReadOnly<object>(this.data);

    public OutgoingMessage(byte messageType, long farmerID, params object[] data)
    {
      this.messageType = messageType;
      this.farmerID = farmerID;
      this.data = data;
    }

    public OutgoingMessage(byte messageType, Farmer sourceFarmer, params object[] data)
      : this(messageType, sourceFarmer.UniqueMultiplayerID, data)
    {
    }

    public OutgoingMessage(IncomingMessage message)
      : this(message.MessageType, message.FarmerID, new object[1]
      {
        (object) message.Data
      })
    {
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.messageType);
      writer.Write(this.farmerID);
      object[] data = this.data;
      writer.WriteSkippable((Action) (() =>
      {
        foreach (object obj in data)
        {
          switch (obj)
          {
            case Vector2 _:
              writer.Write(((Vector2) obj).X);
              writer.Write(((Vector2) obj).Y);
              break;
            case Guid guid2:
              writer.Write(guid2.ToByteArray());
              break;
            case byte[] _:
              writer.Write((byte[]) obj);
              break;
            case bool flag2:
              writer.Write(flag2 ? (byte) 1 : (byte) 0);
              break;
            case byte num6:
              writer.Write(num6);
              break;
            case int num7:
              writer.Write(num7);
              break;
            case short num8:
              writer.Write(num8);
              break;
            case float num9:
              writer.Write(num9);
              break;
            case long num10:
              writer.Write(num10);
              break;
            case string _:
              writer.Write((string) obj);
              break;
            case string[] _:
              string[] strArray = (string[]) obj;
              writer.Write((byte) strArray.Length);
              for (int index = 0; index < strArray.Length; ++index)
                writer.Write(strArray[index]);
              break;
            case IConvertible _:
              if (obj.GetType().IsValueType)
              {
                writer.WriteEnum(obj);
                break;
              }
              goto default;
            default:
              throw new InvalidDataException();
          }
        }
      }));
    }
  }
}
