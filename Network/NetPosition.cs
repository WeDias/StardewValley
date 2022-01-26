// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetPosition
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;

namespace StardewValley.Network
{
  public sealed class NetPosition : NetPausableField<Vector2, NetVector2, NetVector2>
  {
    private const float SmoothingFudge = 0.8f;
    private const ushort DefaultDeltaAggregateTicks = 0;
    public bool ExtrapolationEnabled;
    public readonly NetBool moving = new NetBool().Interpolated(false, false);

    public float X
    {
      get => this.Get().X;
      set => this.Set(new Vector2(value, this.Y));
    }

    public float Y
    {
      get => this.Get().Y;
      set => this.Set(new Vector2(this.X, value));
    }

    public NetPosition()
      : base(new NetVector2().Interpolated(true, true))
    {
    }

    public NetPosition(NetVector2 field)
      : base(field)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.moving);
      this.NetFields.DeltaAggregateTicks = (ushort) 0;
      this.Field.fieldChangeEvent += (NetFieldBase<Vector2, NetVector2>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!this.IsMaster())
          return;
        this.moving.Value = true;
      });
      this.moving.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (this.IsMaster())
          return;
        this.Field.ExtrapolationEnabled = newValue && this.ExtrapolationEnabled;
      });
    }

    protected bool IsMaster() => this.NetFields.Root != null && this.NetFields.Root.Clock.LocalId == 0;

    public override Vector2 Get()
    {
      if (Game1.HostPaused)
        this.Field.CancelInterpolation();
      return base.Get();
    }

    public Vector2 CurrentInterpolationDirection() => this.Paused ? Vector2.Zero : this.Field.CurrentInterpolationDirection();

    public float CurrentInterpolationSpeed() => this.Paused ? 0.0f : this.Field.CurrentInterpolationSpeed();

    public void UpdateExtrapolation(float extrapolationSpeed)
    {
      this.NetFields.DeltaAggregateTicks = this.NetFields.Root != null ? (ushort) ((double) this.NetFields.Root.Clock.InterpolationTicks * 0.800000011920929) : (ushort) 0;
      this.ExtrapolationEnabled = true;
      this.Field.ExtrapolationSpeed = extrapolationSpeed;
      if (!this.IsMaster())
        return;
      this.moving.Value = false;
    }
  }
}
