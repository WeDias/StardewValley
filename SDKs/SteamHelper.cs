// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.SteamHelper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Galaxy.Api;
using StardewValley.Menus;
using Steamworks;
using System;

namespace StardewValley.SDKs
{
  public class SteamHelper : SDKHelper
  {
    private Steamworks.Callback<GameOverlayActivated_t> gameOverlayActivated;
    private CallResult<EncryptedAppTicketResponse_t> encryptedAppTicketResponse;
    private Steamworks.Callback<GamepadTextInputDismissed_t> gamepadTextInputDismissed;
    private GalaxyHelper.AuthListener galaxyAuthListener;
    private GalaxyHelper.OperationalStateChangeListener galaxyStateChangeListener;
    public bool active;
    private SDKNetHelper networking;
    private TextBox _keyboardTextBox;
    protected bool _runningOnSteamDeck;

    public SDKNetHelper Networking => this.networking;

    public bool ConnectionFinished { get; private set; }

    public int ConnectionProgress { get; private set; }

    public string Name { get; } = "Steam";

    public void EarlyInitialize()
    {
    }

    public virtual bool IsRunningOnSteamDeck() => this._runningOnSteamDeck;

    public void Initialize()
    {
      try
      {
        this.active = SteamAPI.Init();
        Console.WriteLine("Steam logged on: " + SteamUser.BLoggedOn().ToString());
        if (this.active)
        {
          this._runningOnSteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();
          Console.WriteLine("Initializing GalaxySDK");
          GalaxyInstance.Init(new InitParams("48767653913349277", "58be5c2e55d7f535cf8c4b6bbc09d185de90b152c8c42703cc13502465f0d04a", "."));
          this.encryptedAppTicketResponse = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.onEncryptedAppTicketResponse));
          this.galaxyAuthListener = new GalaxyHelper.AuthListener(new Action(this.onGalaxyAuthSuccess), new Action<IAuthListener.FailureReason>(this.onGalaxyAuthFailure), new Action(this.onGalaxyAuthLost));
          this.galaxyStateChangeListener = new GalaxyHelper.OperationalStateChangeListener(new Action<uint>(this.onGalaxyStateChange));
          Console.WriteLine("Requesting Steam app ticket");
          this.encryptedAppTicketResponse.Set(SteamUser.RequestEncryptedAppTicket(new byte[0], 0));
          ++this.ConnectionProgress;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        this.active = false;
        this.ConnectionFinished = true;
      }
      if (!this.active)
        return;
      this.gameOverlayActivated = Steamworks.Callback<GameOverlayActivated_t>.Create(new Steamworks.Callback<GameOverlayActivated_t>.DispatchDelegate(this.onGameOverlayActivated));
      this.gamepadTextInputDismissed = Steamworks.Callback<GamepadTextInputDismissed_t>.Create(new Steamworks.Callback<GamepadTextInputDismissed_t>.DispatchDelegate(this.OnKeyboardDismissed));
    }

    public void CancelKeyboard() => this._keyboardTextBox = (TextBox) null;

    public void ShowKeyboard(TextBox text_box)
    {
      this._keyboardTextBox = text_box;
      SteamUtils.ShowGamepadTextInput(text_box.PasswordBox ? EGamepadTextInputMode.k_EGamepadTextInputModePassword : EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, "", text_box.textLimit < 0 ? 100U : (uint) text_box.textLimit, text_box.Text);
    }

    public void OnKeyboardDismissed(GamepadTextInputDismissed_t callback)
    {
      if (this._keyboardTextBox == null)
        return;
      if (!callback.m_bSubmitted)
      {
        this._keyboardTextBox = (TextBox) null;
      }
      else
      {
        string pchText = "";
        uint gamepadTextLength = SteamUtils.GetEnteredGamepadTextLength();
        if (!SteamUtils.GetEnteredGamepadTextInput(out pchText, gamepadTextLength))
        {
          this._keyboardTextBox = (TextBox) null;
        }
        else
        {
          this._keyboardTextBox.RecieveTextInput(pchText);
          this._keyboardTextBox = (TextBox) null;
        }
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
      this.networking = (SDKNetHelper) new SteamNetHelper();
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

    private void onEncryptedAppTicketResponse(EncryptedAppTicketResponse_t response, bool ioFailure)
    {
      if (response.m_eResult == EResult.k_EResultOK)
      {
        byte[] numArray = new byte[1024];
        uint pcbTicket;
        SteamUser.GetEncryptedAppTicket(numArray, 1024, out pcbTicket);
        Console.WriteLine("Signing into GalaxySDK");
        GalaxyInstance.User().SignInSteam(numArray, pcbTicket, SteamFriends.GetPersonaName());
        ++this.ConnectionProgress;
      }
      else
      {
        Console.WriteLine("Failed to retrieve encrypted app ticket: " + response.m_eResult.ToString() + ", " + ioFailure.ToString());
        this.ConnectionFinished = true;
      }
    }

    private void onGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
      if (!this.active)
        return;
      if (pCallback.m_bActive != (byte) 0)
        Game1.paused = !Game1.IsMultiplayer;
      else
        Game1.paused = false;
    }

    public void GetAchievement(string achieve)
    {
      if (!this.active || !SteamAPI.IsSteamRunning())
        return;
      if (achieve.Equals("0"))
        achieve = "a0";
      try
      {
        SteamUserStats.SetAchievement(achieve);
        SteamUserStats.StoreStats();
      }
      catch (Exception ex)
      {
      }
    }

    public void ResetAchievements()
    {
      if (!this.active)
        return;
      if (!SteamAPI.IsSteamRunning())
        return;
      try
      {
        SteamUserStats.ResetAllStats(true);
      }
      catch (Exception ex)
      {
      }
    }

    public void Update()
    {
      if (this.active)
      {
        SteamAPI.RunCallbacks();
        GalaxyInstance.ProcessData();
      }
      Game1.game1.IsMouseVisible = Game1.paused || Game1.options.hardwareCursor;
    }

    public void Shutdown() => SteamAPI.Shutdown();

    public void DebugInfo()
    {
      if (SteamAPI.IsSteamRunning())
      {
        Game1.debugOutput = "steam is running";
        if (!SteamUser.BLoggedOn())
          return;
        Game1.debugOutput += ", user logged on";
      }
      else
      {
        Game1.debugOutput = "steam is not running";
        SteamAPI.Init();
      }
    }

    public string FilterDirtyWords(string words) => words;

    public bool HasOverlay => false;

    public bool IsJapaneseRegionRelease => false;

    public bool IsEnterButtonAssignmentFlipped => false;
  }
}
