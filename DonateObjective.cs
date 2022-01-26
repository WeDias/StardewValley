// Decompiled with JetBrains decompiler
// Type: StardewValley.DonateObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class DonateObjective : OrderObjective
  {
    [XmlElement("dropBox")]
    public NetString dropBox = new NetString();
    [XmlElement("dropBoxGameLocation")]
    public NetString dropBoxGameLocation = new NetString();
    [XmlElement("dropBoxTileLocation")]
    public NetVector2 dropBoxTileLocation = new NetVector2();
    [XmlElement("acceptableContextTagSets")]
    public NetStringList acceptableContextTagSets = new NetStringList();
    [XmlElement("minimumCapacity")]
    public NetInt minimumCapacity = new NetInt(-1);
    [XmlElement("confirmed")]
    public NetBool confirmed = new NetBool(false);

    public virtual string GetDropboxLocationName() => this.dropBoxGameLocation.Value == "Trailer" && Game1.MasterPlayer.hasOrWillReceiveMail("pamHouseUpgrade") ? "Trailer_Big" : this.dropBoxGameLocation.Value;

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      if (data.ContainsKey("AcceptedContextTags"))
        this.acceptableContextTagSets.Add(order.Parse(data["AcceptedContextTags"].Trim()));
      if (data.ContainsKey("DropBox"))
        this.dropBox.Value = order.Parse(data["DropBox"].Trim());
      if (data.ContainsKey("DropBoxGameLocation"))
        this.dropBoxGameLocation.Value = order.Parse(data["DropBoxGameLocation"].Trim());
      if (data.ContainsKey("DropBoxIndicatorLocation"))
      {
        string str = order.Parse(data["DropBoxIndicatorLocation"]);
        this.dropBoxTileLocation.Value = (Vector2) (NetFieldBase<Vector2, NetVector2>) new NetVector2(new Vector2((float) Convert.ToDouble(str.Split(' ')[0]), (float) Convert.ToDouble(str.Split(' ')[1])));
      }
      if (!data.ContainsKey("MinimumCapacity"))
        return;
      this.minimumCapacity.Value = int.Parse(order.Parse(data["MinimumCapacity"]));
    }

    public int GetAcceptCount(Item item, int stack_count) => this.IsValidItem(item) ? Math.Min(this.GetMaxCount() - this.GetCount(), stack_count) : 0;

    public override bool IsComplete() => base.IsComplete();

    public override void OnCompletion()
    {
      base.OnCompletion();
      if (!((NetFieldBase<string, NetString>) this.dropBoxGameLocation != (NetString) null))
        return;
      GameLocation locationFromName = Game1.getLocationFromName(this.GetDropboxLocationName());
      if (locationFromName == null)
        return;
      locationFromName.showDropboxIndicator = false;
    }

    public override bool CanComplete() => this.confirmed.Value;

    public virtual void Confirm()
    {
      if (this.GetCount() >= this.GetMaxCount())
        this.confirmed.Value = true;
      else
        this.confirmed.Value = false;
    }

    public override bool CanUncomplete() => true;

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.acceptableContextTagSets, (INetSerializable) this.dropBox, (INetSerializable) this.dropBoxGameLocation, (INetSerializable) this.dropBoxTileLocation, (INetSerializable) this.minimumCapacity, (INetSerializable) this.confirmed);
      this.confirmed.fieldChangeVisibleEvent += new NetFieldBase<bool, NetBool>.FieldChange(this.OnConfirmed);
    }

    protected void OnConfirmed(NetBool field, bool oldValue, bool newValue)
    {
      if (Utility.ShouldIgnoreValueChangeCallback())
        return;
      this.CheckCompletion();
    }

    public virtual bool IsValidItem(Item item)
    {
      if (item == null)
        return false;
      foreach (string acceptableContextTagSet in (NetList<string, NetString>) this.acceptableContextTagSets)
      {
        bool flag1 = false;
        foreach (string str1 in acceptableContextTagSet.Split(','))
        {
          bool flag2 = false;
          foreach (string str2 in str1.Split('/'))
          {
            if (item.HasContextTag(str2.Trim()))
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
            flag1 = true;
        }
        if (!flag1)
          return true;
      }
      return false;
    }
  }
}
