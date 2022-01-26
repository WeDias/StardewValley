// Decompiled with JetBrains decompiler
// Type: StardewValley.OrderObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class OrderObjective : INetObject<NetFields>
  {
    [XmlIgnore]
    protected SpecialOrder _order;
    [XmlElement("currentCount")]
    public NetIntDelta currentCount = new NetIntDelta();
    [XmlElement("maxCount")]
    public NetInt maxCount = new NetInt(0);
    [XmlElement("description")]
    public NetString description = new NetString();
    [XmlIgnore]
    protected bool _complete;
    [XmlIgnore]
    protected bool _registered;
    [XmlElement("failOnCompletion")]
    public NetBool failOnCompletion = new NetBool(false);

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public OrderObjective() => this.InitializeNetFields();

    public virtual void OnFail()
    {
    }

    public virtual void InitializeNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.currentCount, (INetSerializable) this.maxCount, (INetSerializable) this.failOnCompletion, (INetSerializable) this.description);
      this.currentCount.fieldChangeVisibleEvent += new NetFieldBase<int, NetIntDelta>.FieldChange(this.OnCurrentCountChanged);
    }

    protected void OnCurrentCountChanged(NetIntDelta field, int oldValue, int newValue)
    {
      if (Utility.ShouldIgnoreValueChangeCallback())
        return;
      this.CheckCompletion();
    }

    public void Register(SpecialOrder new_order)
    {
      this._registered = true;
      this._order = new_order;
      this._Register();
      this.CheckCompletion(false);
    }

    protected virtual void _Register()
    {
    }

    public virtual void Unregister()
    {
      this._registered = false;
      this._Unregister();
      this._order = (SpecialOrder) null;
    }

    protected virtual void _Unregister()
    {
    }

    public virtual bool ShouldShowProgress() => true;

    public int GetCount() => (int) (NetFieldBase<int, NetIntDelta>) this.currentCount;

    public virtual void IncrementCount(int amount)
    {
      int new_count = this.GetCount() + amount;
      if (new_count < 0)
        new_count = 0;
      if (new_count > this.GetMaxCount())
        new_count = this.GetMaxCount();
      this.SetCount(new_count);
    }

    public virtual void SetCount(int new_count)
    {
      if (new_count > this.GetMaxCount())
        new_count = this.GetMaxCount();
      if (new_count == this.GetCount())
        return;
      this.currentCount.Value = new_count;
    }

    public int GetMaxCount() => (int) (NetFieldBase<int, NetInt>) this.maxCount;

    public virtual void OnCompletion()
    {
    }

    public virtual void CheckCompletion(bool play_sound = true)
    {
      if (!this._registered)
        return;
      bool flag = false;
      if (this.GetCount() >= this.GetMaxCount() && this.CanComplete())
      {
        if (!this._complete)
        {
          flag = true;
          this.OnCompletion();
        }
        this._complete = true;
      }
      else if (this.CanUncomplete() && this._complete)
        this._complete = false;
      if (this._order == null)
        return;
      this._order.CheckCompletion();
      if (!flag || this._order.questState.Value == SpecialOrder.QuestState.Complete || !play_sound)
        return;
      Game1.playSound("jingle1");
    }

    public virtual bool IsComplete() => this._complete;

    public virtual bool CanUncomplete() => false;

    public virtual bool CanComplete() => true;

    public virtual string GetDescription()
    {
      this.GetMaxCount();
      return (string) (NetFieldBase<string, NetString>) this.description;
    }

    public virtual void Load(SpecialOrder order, Dictionary<string, string> data)
    {
    }
  }
}
