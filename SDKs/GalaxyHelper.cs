// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GalaxyHelper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Galaxy.Api;
using System;

namespace StardewValley.SDKs
{
  public class GalaxyHelper : SDKHelper
  {
    public const string ClientID = "48767653913349277";
    public const string ClientSecret = "58be5c2e55d7f535cf8c4b6bbc09d185de90b152c8c42703cc13502465f0d04a";
    public bool active;
    private GalaxyHelper.AuthListener authListener;
    private GalaxyHelper.OperationalStateChangeListener stateChangeListener;
    private GalaxyNetHelper networking;

    public string Name { get; } = "Galaxy";

    public bool ConnectionFinished { get; private set; }

    public int ConnectionProgress { get; private set; }

    public SDKNetHelper Networking => (SDKNetHelper) this.networking;

    public bool HasOverlay => false;

    public void EarlyInitialize()
    {
    }

    public void Initialize()
    {
      try
      {
        GalaxyInstance.Init(new InitParams("48767653913349277", "58be5c2e55d7f535cf8c4b6bbc09d185de90b152c8c42703cc13502465f0d04a"));
        this.authListener = new GalaxyHelper.AuthListener(new Action(this.onGalaxyAuthSuccess), new Action<IAuthListener.FailureReason>(this.onGalaxyAuthFailure), new Action(this.onGalaxyAuthLost));
        this.stateChangeListener = new GalaxyHelper.OperationalStateChangeListener(new Action<uint>(this.onGalaxyStateChange));
        GalaxyInstance.User().SignInGalaxy(true);
        this.active = true;
        ++this.ConnectionProgress;
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        this.ConnectionFinished = true;
      }
    }

    private void onGalaxyStateChange(uint operationalState)
    {
      if (this.networking != null)
        return;
      if (((int) operationalState & 1) != 0)
      {
        Console.WriteLine("Galaxy signed in");
        ++this.ConnectionProgress;
      }
      if (((int) operationalState & 2) == 0)
        return;
      Console.WriteLine("Galaxy logged on");
      this.networking = new GalaxyNetHelper();
      ++this.ConnectionProgress;
      this.ConnectionFinished = true;
    }

    private void onGalaxyAuthSuccess()
    {
      Console.WriteLine("Galaxy auth success");
      ++this.ConnectionProgress;
    }

    private void onGalaxyAuthFailure(IAuthListener.FailureReason reason)
    {
      Console.WriteLine("Galaxy auth failure: " + reason.ToString());
      this.ConnectionFinished = true;
    }

    private void onGalaxyAuthLost()
    {
      Console.WriteLine("Galaxy auth lost");
      this.ConnectionFinished = true;
    }

    public void GetAchievement(string achieve)
    {
    }

    public void ResetAchievements()
    {
      if (!this.active)
        return;
      GalaxyInstance.Stats().ResetStatsAndAchievements();
    }

    public void Update()
    {
      if (!this.active)
        return;
      GalaxyInstance.ProcessData();
    }

    public void Shutdown()
    {
    }

    public void DebugInfo()
    {
    }

    public string FilterDirtyWords(string words) => words;

    public bool IsJapaneseRegionRelease => false;

    public bool IsEnterButtonAssignmentFlipped => false;

    public class AuthListener : IAuthListener
    {
      public Action OnSuccess;
      public Action<IAuthListener.FailureReason> OnFailure;
      public Action OnLost;

      public AuthListener(Action success, Action<IAuthListener.FailureReason> failure, Action lost)
      {
        this.OnSuccess = success;
        this.OnFailure = failure;
        this.OnLost = lost;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerAuth.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnAuthSuccess()
      {
        if (this.OnSuccess == null)
          return;
        this.OnSuccess();
      }

      public override void OnAuthFailure(IAuthListener.FailureReason reason)
      {
        if (this.OnFailure == null)
          return;
        this.OnFailure(reason);
      }

      public override void OnAuthLost()
      {
        if (this.OnLost == null)
          return;
        this.OnLost();
      }
    }

    public class OperationalStateChangeListener : IOperationalStateChangeListener
    {
      public Action<uint> OnStateChanged;

      public OperationalStateChangeListener(Action<uint> stateChanged)
      {
        this.OnStateChanged = stateChanged;
        GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerOperationalStateChange.GetListenerType(), (IGalaxyListener) this);
      }

      public override void OnOperationalStateChanged(uint operationalState)
      {
        if (this.OnStateChanged == null)
          return;
        this.OnStateChanged(operationalState);
      }
    }
  }
}
