// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.NullSDKHelper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.SDKs
{
  public class NullSDKHelper : SDKHelper
  {
    public bool IsEnterButtonAssignmentFlipped => false;

    public bool IsJapaneseRegionRelease => false;

    public void EarlyInitialize()
    {
    }

    public void Initialize()
    {
    }

    public void GetAchievement(string achieve)
    {
    }

    public void ResetAchievements()
    {
    }

    public void Update()
    {
    }

    public void Shutdown()
    {
    }

    public void DebugInfo()
    {
    }

    public string FilterDirtyWords(string words) => words;

    public virtual string Name { get; } = "?";

    public SDKNetHelper Networking { get; }

    public bool ConnectionFinished { get; } = true;

    public int ConnectionProgress { get; }

    public bool HasOverlay => false;
  }
}
