// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetPausableField`3
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;

namespace StardewValley.Network
{
  public class NetPausableField<T, TField, TBaseField> : INetObject<NetFields>
    where TField : TBaseField, new()
    where TBaseField : NetFieldBase<T, TBaseField>, new()
  {
    private bool paused;
    public readonly TField Field;
    private readonly NetEvent1Field<bool, NetBool> pauseEvent = new NetEvent1Field<bool, NetBool>();

    public T Value
    {
      get => this.Get();
      set => this.Set(value);
    }

    public bool Paused
    {
      get
      {
        this.pauseEvent.Poll();
        return this.paused;
      }
      set
      {
        if (value == this.paused)
          return;
        this.pauseEvent.Fire(value);
        this.pauseEvent.Poll();
      }
    }

    public NetFields NetFields { get; } = new NetFields();

    public NetPausableField(TField field)
    {
      this.Field = field;
      this.initNetFields();
    }

    protected virtual void initNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.Field, (INetSerializable) this.pauseEvent);
      this.pauseEvent.onEvent += (AbstractNetEvent1<bool>.Event) (newPauseValue => this.paused = newPauseValue);
    }

    public NetPausableField()
      : this(new TField())
    {
    }

    public virtual T Get()
    {
      if (this.Paused)
        this.Field.CancelInterpolation();
      return this.Field.Get();
    }

    public void Set(T value) => this.Field.Set(value);

    public bool IsPausePending() => this.pauseEvent.HasPendingEvent((Predicate<bool>) (p => p));

    public bool IsInterpolating() => this.Field.IsInterpolating() && !this.Paused;

    public static implicit operator T(NetPausableField<T, TField, TBaseField> field) => field.Get();
  }
}
