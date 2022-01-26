// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.SDKHelper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.SDKs
{
  public interface SDKHelper
  {
    /// <summary>
    /// This property needs to be initialized to the correct value before Initialize(), so probably within EarlyInitialize().
    /// </summary>
    bool IsEnterButtonAssignmentFlipped { get; }

    /// <summary>
    /// This property needs to be initialized to the correct value before Initialize(), so probably within EarlyInitialize().
    /// </summary>
    bool IsJapaneseRegionRelease { get; }

    void EarlyInitialize();

    void Initialize();

    void Update();

    void Shutdown();

    void DebugInfo();

    void GetAchievement(string achieve);

    void ResetAchievements();

    string FilterDirtyWords(string words);

    string Name { get; }

    SDKNetHelper Networking { get; }

    bool ConnectionFinished { get; }

    int ConnectionProgress { get; }

    bool HasOverlay { get; }
  }
}
