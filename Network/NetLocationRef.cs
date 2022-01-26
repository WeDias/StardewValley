// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetLocationRef
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;

namespace StardewValley.Network
{
  public class NetLocationRef : INetObject<NetFields>
  {
    private readonly NetString locationName = new NetString();
    private readonly NetBool isStructure = new NetBool();
    protected GameLocation _gameLocation;
    protected bool _dirty = true;
    protected bool _usedLocalLocation;

    public GameLocation Value
    {
      get => this.Get();
      set => this.Set(value);
    }

    public NetFields NetFields { get; } = new NetFields();

    public NetLocationRef()
    {
      this.NetFields.AddFields((INetSerializable) this.locationName, (INetSerializable) this.isStructure);
      this.locationName.fieldChangeVisibleEvent += new NetFieldBase<string, NetString>.FieldChange(this.OnLocationNameChanged);
      this.isStructure.fieldChangeVisibleEvent += new NetFieldBase<bool, NetBool>.FieldChange(this.OnStructureValueChanged);
    }

    public virtual void OnLocationNameChanged(NetString field, string old_value, string new_value) => this._dirty = true;

    public virtual void OnStructureValueChanged(NetBool field, bool old_value, bool new_value) => this._dirty = true;

    public NetLocationRef(GameLocation value)
      : this()
    {
      this.Set(value);
    }

    public bool IsChanging() => this.locationName.IsChanging() || this.isStructure.IsChanging();

    public void Update() => this.ApplyChangesIfDirty();

    public void ApplyChangesIfDirty()
    {
      if (this._usedLocalLocation && this._gameLocation != Game1.currentLocation)
      {
        this._dirty = true;
        this._usedLocalLocation = false;
      }
      if (this._dirty)
      {
        this._gameLocation = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) this.locationName, (bool) (NetFieldBase<bool, NetBool>) this.isStructure);
        this._dirty = false;
      }
      if (this._usedLocalLocation || this._gameLocation == Game1.currentLocation || !this.IsCurrentlyViewedLocation())
        return;
      this._usedLocalLocation = true;
      this._gameLocation = Game1.currentLocation;
    }

    public GameLocation Get()
    {
      this.ApplyChangesIfDirty();
      return this._gameLocation;
    }

    public void Set(GameLocation location)
    {
      if (location == null)
      {
        this.isStructure.Value = false;
        this.locationName.Value = "";
      }
      else
      {
        this.isStructure.Value = (bool) (NetFieldBase<bool, NetBool>) location.isStructure;
        this.locationName.Value = (bool) (NetFieldBase<bool, NetBool>) location.isStructure ? (string) (NetFieldBase<string, NetString>) location.uniqueName : location.Name;
      }
      if (this.IsCurrentlyViewedLocation())
      {
        this._usedLocalLocation = true;
        this._gameLocation = Game1.currentLocation;
      }
      else
        this._gameLocation = location;
      if (this._gameLocation != null && this._gameLocation.isTemp())
        this._gameLocation = (GameLocation) null;
      this._dirty = false;
    }

    public bool IsCurrentlyViewedLocation() => Game1.currentLocation != null && this.locationName.Value == Game1.currentLocation.NameOrUniqueName;

    public static implicit operator GameLocation(NetLocationRef locationRef) => locationRef.Value;
  }
}
