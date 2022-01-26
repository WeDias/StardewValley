// Decompiled with JetBrains decompiler
// Type: LocationWeather
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;

public class LocationWeather : INetObject<NetFields>
{
  public readonly NetInt weatherForTomorrow = new NetInt();
  public readonly NetBool isRaining = new NetBool();
  public readonly NetBool isSnowing = new NetBool();
  public readonly NetBool isLightning = new NetBool();
  public readonly NetBool isDebrisWeather = new NetBool();

  public NetFields NetFields { get; } = new NetFields();

  public LocationWeather() => this.NetFields.AddFields((INetSerializable) this.isRaining, (INetSerializable) this.isSnowing, (INetSerializable) this.isLightning, (INetSerializable) this.isDebrisWeather, (INetSerializable) this.weatherForTomorrow);

  public void InitializeDayWeather()
  {
    this.isRaining.Value = false;
    this.isSnowing.Value = false;
    this.isLightning.Value = false;
    this.isDebrisWeather.Value = false;
  }

  public void CopyFrom(LocationWeather other)
  {
    this.isRaining.Value = other.isRaining.Value;
    this.isSnowing.Value = other.isSnowing.Value;
    this.isLightning.Value = other.isLightning.Value;
    this.isDebrisWeather.Value = other.isDebrisWeather.Value;
    this.weatherForTomorrow.Value = other.weatherForTomorrow.Value;
  }
}
