// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldDate
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Xml.Serialization;

namespace StardewValley
{
  public class WorldDate : INetObject<NetFields>
  {
    public const int MonthsPerYear = 4;
    public const int DaysPerMonth = 28;
    private readonly NetInt year = new NetInt(1);
    private readonly NetInt seasonIndex = new NetInt(0);
    private readonly NetInt dayOfMonth = new NetInt(1);

    public int Year
    {
      get => this.year.Value;
      set => this.year.Value = value;
    }

    [XmlIgnore]
    public int SeasonIndex
    {
      get => this.seasonIndex.Value;
      internal set => this.seasonIndex.Value = value;
    }

    public int DayOfMonth
    {
      get => this.dayOfMonth.Value;
      set => this.dayOfMonth.Value = value;
    }

    public DayOfWeek DayOfWeek => (DayOfWeek) (this.DayOfMonth % 7);

    public string Season
    {
      get
      {
        switch (this.SeasonIndex)
        {
          case 0:
            return "spring";
          case 1:
            return "summer";
          case 2:
            return "fall";
          case 3:
            return "winter";
          default:
            throw new ArgumentException(Convert.ToString(this.SeasonIndex));
        }
      }
      set
      {
        if (!(value == "spring"))
        {
          if (!(value == "summer"))
          {
            if (!(value == "fall"))
            {
              if (!(value == "winter"))
                throw new ArgumentException(value);
              this.SeasonIndex = 3;
            }
            else
              this.SeasonIndex = 2;
          }
          else
            this.SeasonIndex = 1;
        }
        else
          this.SeasonIndex = 0;
      }
    }

    public int TotalDays
    {
      get => ((this.Year - 1) * 4 + this.SeasonIndex) * 28 + (this.DayOfMonth - 1);
      set
      {
        int num = value / 28;
        this.DayOfMonth = value % 28 + 1;
        this.SeasonIndex = num % 4;
        this.Year = num / 4 + 1;
      }
    }

    public int TotalWeeks => this.TotalDays / 7;

    public int TotalSundayWeeks => (this.TotalDays + 1) / 7;

    public NetFields NetFields { get; } = new NetFields();

    public WorldDate() => this.NetFields.AddFields((INetSerializable) this.year, (INetSerializable) this.seasonIndex, (INetSerializable) this.dayOfMonth);

    public WorldDate(WorldDate other)
      : this()
    {
      this.Year = other.Year;
      this.SeasonIndex = other.SeasonIndex;
      this.DayOfMonth = other.DayOfMonth;
    }

    public WorldDate(int year, string season, int dayOfMonth)
      : this()
    {
      this.Year = year;
      this.Season = season;
      this.DayOfMonth = dayOfMonth;
    }

    public string Localize() => Utility.getDateStringFor(this.DayOfMonth, this.SeasonIndex, this.Year);

    public override string ToString() => "Year " + this.Year.ToString() + ", " + this.Season + " " + this.DayOfMonth.ToString() + ", " + this.DayOfWeek.ToString();

    public static bool operator ==(WorldDate a, WorldDate b)
    {
      int? totalDays1 = a?.TotalDays;
      int? totalDays2 = b?.TotalDays;
      return totalDays1.GetValueOrDefault() == totalDays2.GetValueOrDefault() & totalDays1.HasValue == totalDays2.HasValue;
    }

    public static bool operator !=(WorldDate a, WorldDate b)
    {
      int? totalDays1 = a?.TotalDays;
      int? totalDays2 = b?.TotalDays;
      return !(totalDays1.GetValueOrDefault() == totalDays2.GetValueOrDefault() & totalDays1.HasValue == totalDays2.HasValue);
    }

    public static bool operator <(WorldDate a, WorldDate b)
    {
      int? totalDays1 = a?.TotalDays;
      int? totalDays2 = b?.TotalDays;
      return totalDays1.GetValueOrDefault() < totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
    }

    public static bool operator >(WorldDate a, WorldDate b)
    {
      int? totalDays1 = a?.TotalDays;
      int? totalDays2 = b?.TotalDays;
      return totalDays1.GetValueOrDefault() > totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
    }

    public static bool operator <=(WorldDate a, WorldDate b)
    {
      int? totalDays1 = a?.TotalDays;
      int? totalDays2 = b?.TotalDays;
      return totalDays1.GetValueOrDefault() <= totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
    }

    public static bool operator >=(WorldDate a, WorldDate b)
    {
      int? totalDays1 = a?.TotalDays;
      int? totalDays2 = b?.TotalDays;
      return totalDays1.GetValueOrDefault() >= totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
    }
  }
}
