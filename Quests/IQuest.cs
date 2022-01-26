// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.IQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System.Collections.Generic;

namespace StardewValley.Quests
{
  public interface IQuest
  {
    string GetName();

    string GetDescription();

    List<string> GetObjectiveDescriptions();

    bool CanBeCancelled();

    void MarkAsViewed();

    bool ShouldDisplayAsNew();

    bool ShouldDisplayAsComplete();

    bool IsTimedQuest();

    int GetDaysLeft();

    bool IsHidden();

    bool HasReward();

    bool HasMoneyReward();

    int GetMoneyReward();

    void OnMoneyRewardClaimed();

    bool OnLeaveQuestPage();
  }
}
