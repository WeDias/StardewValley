// Decompiled with JetBrains decompiler
// Type: StardewValley.BuildingPaintColor
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Xml.Serialization;

namespace StardewValley
{
  public class BuildingPaintColor : INetObject<NetFields>
  {
    public NetString ColorName = new NetString();
    public NetBool Color1Default = new NetBool(true);
    public NetInt Color1Hue = new NetInt();
    public NetInt Color1Saturation = new NetInt();
    public NetInt Color1Lightness = new NetInt();
    public NetBool Color2Default = new NetBool(true);
    public NetInt Color2Hue = new NetInt();
    public NetInt Color2Saturation = new NetInt();
    public NetInt Color2Lightness = new NetInt();
    public NetBool Color3Default = new NetBool(true);
    public NetInt Color3Hue = new NetInt();
    public NetInt Color3Saturation = new NetInt();
    public NetInt Color3Lightness = new NetInt();
    protected bool _dirty;

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public BuildingPaintColor()
    {
      this.NetFields.AddFields((INetSerializable) this.ColorName, (INetSerializable) this.Color1Default, (INetSerializable) this.Color2Default, (INetSerializable) this.Color3Default, (INetSerializable) this.Color1Hue, (INetSerializable) this.Color1Saturation, (INetSerializable) this.Color1Lightness, (INetSerializable) this.Color2Hue, (INetSerializable) this.Color2Saturation, (INetSerializable) this.Color2Lightness, (INetSerializable) this.Color3Hue, (INetSerializable) this.Color3Saturation, (INetSerializable) this.Color3Lightness);
      this.Color1Default.fieldChangeVisibleEvent += new NetFieldBase<bool, NetBool>.FieldChange(this.OnDefaultFlagChanged);
      this.Color2Default.fieldChangeVisibleEvent += new NetFieldBase<bool, NetBool>.FieldChange(this.OnDefaultFlagChanged);
      this.Color3Default.fieldChangeVisibleEvent += new NetFieldBase<bool, NetBool>.FieldChange(this.OnDefaultFlagChanged);
      this.Color1Hue.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color1Saturation.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color1Lightness.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color2Hue.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color2Saturation.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color2Lightness.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color3Hue.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color3Saturation.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
      this.Color3Lightness.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnColorChanged);
    }

    public virtual void OnDefaultFlagChanged(NetBool field, bool old_value, bool new_value) => this._dirty = true;

    public virtual void OnColorChanged(NetInt field, int old_value, int new_value) => this._dirty = true;

    public virtual void Poll(Action apply)
    {
      if (!this._dirty)
        return;
      if (apply != null)
        apply();
      this._dirty = false;
    }

    public bool IsDirty() => this._dirty;

    public bool RequiresRecolor() => !this.Color1Default.Value || !this.Color2Default.Value || !this.Color3Default.Value;
  }
}
