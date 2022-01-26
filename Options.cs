// Decompiled with JetBrains decompiler
// Type: StardewValley.Options
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace StardewValley
{
  public class Options
  {
    public const float minZoom = 0.75f;
    public const float maxZoom = 2f;
    public const float minUIZoom = 0.75f;
    public const float maxUIZoom = 1.5f;
    public const int toggleAutoRun = 0;
    public const int musicVolume = 1;
    public const int soundVolume = 2;
    public const int toggleDialogueTypingSounds = 3;
    public const int toggleFullscreen = 4;
    public const int toggleWindowedOrTrueFullscreen = 5;
    public const int screenResolution = 6;
    public const int showPortraitsToggle = 7;
    public const int showMerchantPortraitsToggle = 8;
    public const int menuBG = 9;
    public const int toggleFootsteps = 10;
    public const int alwaysShowToolHitLocationToggle = 11;
    public const int hideToolHitLocationWhenInMotionToggle = 12;
    public const int windowMode = 13;
    public const int pauseWhenUnfocused = 14;
    public const int pinToolbar = 15;
    public const int toggleRumble = 16;
    public const int ambientOnly = 17;
    public const int zoom = 18;
    public const int zoomButtonsToggle = 19;
    public const int ambientVolume = 20;
    public const int footstepVolume = 21;
    public const int invertScrollDirectionToggle = 22;
    public const int snowTransparencyToggle = 23;
    public const int screenFlashToggle = 24;
    public const int lightingQualityToggle = 25;
    public const int toggleHardwareCursor = 26;
    public const int toggleShowPlacementTileGamepad = 27;
    public const int stowingModeSelect = 28;
    public const int toggleSnappyMenus = 29;
    public const int toggleIPConnections = 30;
    public const int serverMode = 31;
    public const int toggleFarmhandCreation = 32;
    public const int toggleShowAdvancedCraftingInformation = 34;
    public const int toggleMPReadyStatus = 35;
    public const int mapScreenshot = 36;
    public const int toggleVsync = 37;
    public const int gamepadModeSelect = 38;
    public const int uiScaleSlider = 39;
    public const int moveBuildingPermissions = 40;
    public const int slingshotModeSelect = 41;
    public const int biteChime = 42;
    public const int toggleMuteAnimalSounds = 43;
    public const int input_actionButton = 7;
    public const int input_cancelButton = 9;
    public const int input_useToolButton = 10;
    public const int input_moveUpButton = 11;
    public const int input_moveRightButton = 12;
    public const int input_moveDownButton = 13;
    public const int input_moveLeftButton = 14;
    public const int input_menuButton = 15;
    public const int input_runButton = 16;
    public const int input_chatButton = 17;
    public const int input_journalButton = 18;
    public const int input_mapButton = 19;
    public const int input_slot1 = 20;
    public const int input_slot2 = 21;
    public const int input_slot3 = 22;
    public const int input_slot4 = 23;
    public const int input_slot5 = 24;
    public const int input_slot6 = 25;
    public const int input_slot7 = 26;
    public const int input_slot8 = 27;
    public const int input_slot9 = 28;
    public const int input_slot10 = 29;
    public const int input_slot11 = 30;
    public const int input_slot12 = 31;
    public const int input_toolbarSwap = 32;
    public const int input_emoteButton = 33;
    public const int checkBoxOption = 1;
    public const int sliderOption = 2;
    public const int dropDownOption = 3;
    public const float defaultZoomLevel = 1f;
    public const int defaultLightingQuality = 32;
    public const float defaultSplitScreenZoomLevel = 1f;
    public bool autoRun;
    public bool dialogueTyping;
    public bool showPortraits;
    public bool showMerchantPortraits;
    public bool showMenuBackground;
    public bool playFootstepSounds;
    public bool alwaysShowToolHitLocation;
    public bool hideToolHitLocationWhenInMotion;
    public bool pauseWhenOutOfFocus;
    public bool pinToolbarToggle;
    public bool mouseControls;
    public bool keyboardControls;
    public bool gamepadControls;
    public bool rumble;
    public bool ambientOnlyToggle;
    public bool zoomButtons;
    public bool invertScrollDirection;
    public bool screenFlash;
    public bool showPlacementTileForGamepad;
    public bool snappyMenus;
    public bool showAdvancedCraftingInformation;
    public bool showMPEndOfNightReadyStatus;
    public bool muteAnimalSounds;
    public bool vsyncEnabled;
    public bool fullscreen;
    public bool windowedBorderlessFullscreen;
    [DontLoadDefaultSetting]
    public bool ipConnectionsEnabled;
    [DontLoadDefaultSetting]
    public bool enableServer;
    [DontLoadDefaultSetting]
    public bool enableFarmhandCreation;
    protected bool _hardwareCursor;
    public Options.ItemStowingModes stowingMode;
    [DontLoadDefaultSetting]
    public Options.GamepadModes gamepadMode;
    public bool useLegacySlingshotFiring;
    public float musicVolumeLevel;
    public float soundVolumeLevel;
    public float footstepVolumeLevel;
    public float ambientVolumeLevel;
    public float snowTransparency;
    [XmlIgnore]
    public float baseZoomLevel = 1f;
    [DontLoadDefaultSetting]
    [XmlElement("zoomLevel")]
    public float singlePlayerBaseZoomLevel = 1f;
    [DontLoadDefaultSetting]
    public float localCoopBaseZoomLevel = 1f;
    [DontLoadDefaultSetting]
    [XmlElement("uiScale")]
    public float singlePlayerDesiredUIScale = -1f;
    [DontLoadDefaultSetting]
    public float localCoopDesiredUIScale = 1.5f;
    [XmlIgnore]
    public float baseUIScale = 1f;
    public int preferredResolutionX;
    public int preferredResolutionY;
    public int lightingQuality;
    [DontLoadDefaultSetting]
    public ServerPrivacy serverPrivacy = ServerPrivacy.FriendsOnly;
    public InputButton[] actionButton = new InputButton[2]
    {
      new InputButton(Keys.X),
      new InputButton(false)
    };
    public InputButton[] cancelButton = new InputButton[1]
    {
      new InputButton(Keys.V)
    };
    public InputButton[] useToolButton = new InputButton[2]
    {
      new InputButton(Keys.C),
      new InputButton(true)
    };
    public InputButton[] moveUpButton = new InputButton[1]
    {
      new InputButton(Keys.W)
    };
    public InputButton[] moveRightButton = new InputButton[1]
    {
      new InputButton(Keys.D)
    };
    public InputButton[] moveDownButton = new InputButton[1]
    {
      new InputButton(Keys.S)
    };
    public InputButton[] moveLeftButton = new InputButton[1]
    {
      new InputButton(Keys.A)
    };
    public InputButton[] menuButton = new InputButton[2]
    {
      new InputButton(Keys.E),
      new InputButton(Keys.Escape)
    };
    public InputButton[] runButton = new InputButton[1]
    {
      new InputButton(Keys.LeftShift)
    };
    public InputButton[] tmpKeyToReplace = new InputButton[1]
    {
      new InputButton(Keys.None)
    };
    public InputButton[] chatButton = new InputButton[2]
    {
      new InputButton(Keys.T),
      new InputButton(Keys.OemQuestion)
    };
    public InputButton[] mapButton = new InputButton[1]
    {
      new InputButton(Keys.M)
    };
    public InputButton[] journalButton = new InputButton[1]
    {
      new InputButton(Keys.F)
    };
    public InputButton[] inventorySlot1 = new InputButton[1]
    {
      new InputButton(Keys.D1)
    };
    public InputButton[] inventorySlot2 = new InputButton[1]
    {
      new InputButton(Keys.D2)
    };
    public InputButton[] inventorySlot3 = new InputButton[1]
    {
      new InputButton(Keys.D3)
    };
    public InputButton[] inventorySlot4 = new InputButton[1]
    {
      new InputButton(Keys.D4)
    };
    public InputButton[] inventorySlot5 = new InputButton[1]
    {
      new InputButton(Keys.D5)
    };
    public InputButton[] inventorySlot6 = new InputButton[1]
    {
      new InputButton(Keys.D6)
    };
    public InputButton[] inventorySlot7 = new InputButton[1]
    {
      new InputButton(Keys.D7)
    };
    public InputButton[] inventorySlot8 = new InputButton[1]
    {
      new InputButton(Keys.D8)
    };
    public InputButton[] inventorySlot9 = new InputButton[1]
    {
      new InputButton(Keys.D9)
    };
    public InputButton[] inventorySlot10 = new InputButton[1]
    {
      new InputButton(Keys.D0)
    };
    public InputButton[] inventorySlot11 = new InputButton[1]
    {
      new InputButton(Keys.OemMinus)
    };
    public InputButton[] inventorySlot12 = new InputButton[1]
    {
      new InputButton(Keys.OemPlus)
    };
    public InputButton[] toolbarSwap = new InputButton[1]
    {
      new InputButton(Keys.Tab)
    };
    public InputButton[] emoteButton = new InputButton[1]
    {
      new InputButton(Keys.Y)
    };
    [XmlIgnore]
    public bool optionsDirty;
    [XmlIgnore]
    private XmlSerializer defaultSettingsSerializer = new XmlSerializer(typeof (Options));
    private int appliedLightingQuality = -1;

    public bool hardwareCursor
    {
      get => !LocalMultiplayer.IsLocalMultiplayer() && this._hardwareCursor;
      set => this._hardwareCursor = value;
    }

    [XmlIgnore]
    public float zoomLevel => Game1.game1.takingMapScreenshot ? this.baseZoomLevel : this.baseZoomLevel * Game1.game1.zoomModifier;

    [XmlIgnore]
    public float desiredBaseZoomLevel
    {
      get => LocalMultiplayer.IsLocalMultiplayer() || !Game1.game1.IsMainInstance ? this.localCoopBaseZoomLevel : this.singlePlayerBaseZoomLevel;
      set
      {
        if (LocalMultiplayer.IsLocalMultiplayer() || !Game1.game1.IsMainInstance)
          this.localCoopBaseZoomLevel = value;
        else
          this.singlePlayerBaseZoomLevel = value;
      }
    }

    [XmlIgnore]
    public float desiredUIScale
    {
      get
      {
        if (Game1.gameMode != (byte) 3)
          return 1f;
        return LocalMultiplayer.IsLocalMultiplayer() || !Game1.game1.IsMainInstance ? this.localCoopDesiredUIScale : this.singlePlayerDesiredUIScale;
      }
      set
      {
        if (Game1.gameMode != (byte) 3)
          return;
        if (LocalMultiplayer.IsLocalMultiplayer() || !Game1.game1.IsMainInstance)
          this.localCoopDesiredUIScale = value;
        else
          this.singlePlayerDesiredUIScale = value;
      }
    }

    [XmlIgnore]
    public float uiScale => this.baseUIScale * Game1.game1.zoomModifier;

    public bool allowStowing => this.stowingMode != Options.ItemStowingModes.Off && (this.stowingMode != Options.ItemStowingModes.GamepadOnly || this.gamepadControls);

    public Options() => this.setToDefaults();

    public virtual void LoadDefaultOptions()
    {
      if (!Game1.game1.IsMainInstance)
        return;
      Options options = (Options) null;
      string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "default_options"));
      XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
      try
      {
        using (FileStream fileStream = File.Open(path, FileMode.Open))
          options = this.defaultSettingsSerializer.Deserialize((Stream) fileStream) as Options;
      }
      catch (Exception ex)
      {
      }
      if (options == null)
        return;
      Type type = typeof (Options);
      foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
      {
        if (field.GetCustomAttributes(typeof (DontLoadDefaultSetting), true).Length == 0 && field.GetCustomAttributes(typeof (XmlIgnoreAttribute), true).Length == 0)
          field.SetValue((object) this, field.GetValue((object) options));
      }
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
      {
        if (property.GetCustomAttributes(typeof (DontLoadDefaultSetting), true).Length == 0 && property.GetCustomAttributes(typeof (XmlIgnoreAttribute), true).Length == 0 && property.GetSetMethod() != (MethodInfo) null && property.GetGetMethod() != (MethodInfo) null)
          property.SetValue((object) this, property.GetValue((object) options, (object[]) null), (object[]) null);
      }
    }

    public virtual void SaveDefaultOptions()
    {
      this.optionsDirty = false;
      if (!Game1.game1.IsMainInstance)
        return;
      string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "default_options"));
      XmlWriterSettings settings = new XmlWriterSettings();
      try
      {
        using (FileStream output = File.Open(path, FileMode.Create))
        {
          using (XmlWriter xmlWriter = XmlWriter.Create((Stream) output, settings))
          {
            xmlWriter.WriteStartDocument();
            this.defaultSettingsSerializer.Serialize(xmlWriter, (object) Game1.options);
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
          }
        }
      }
      catch (Exception ex)
      {
      }
    }

    public void platformClampValues()
    {
    }

    public bool SnappyMenus => this.snappyMenus && this.gamepadControls && Game1.input.GetMouseState().LeftButton != ButtonState.Pressed && Game1.input.GetMouseState().RightButton != ButtonState.Pressed;

    public Keys getFirstKeyboardKeyFromInputButtonList(InputButton[] inputButton)
    {
      for (int index = 0; index < inputButton.Length; ++index)
      {
        ref InputButton local = ref inputButton[index];
        if (inputButton[index].key != Keys.None)
          return inputButton[index].key;
      }
      return Keys.None;
    }

    public void reApplySetOptions()
    {
      this.platformClampValues();
      if (this.lightingQuality != this.appliedLightingQuality)
      {
        Program.gamePtr.refreshWindowSettings();
        this.appliedLightingQuality = this.lightingQuality;
      }
      Program.gamePtr.IsMouseVisible = this.hardwareCursor;
    }

    public void setToDefaults()
    {
      this.playFootstepSounds = true;
      this.showMenuBackground = false;
      this.showMerchantPortraits = true;
      this.showPortraits = true;
      this.autoRun = true;
      this.alwaysShowToolHitLocation = false;
      this.hideToolHitLocationWhenInMotion = true;
      this.dialogueTyping = true;
      this.rumble = true;
      this.fullscreen = false;
      this.pinToolbarToggle = false;
      this.baseZoomLevel = 1f;
      this.localCoopBaseZoomLevel = 1f;
      if (Game1.options == this)
        Game1.forceSnapOnNextViewportUpdate = true;
      this.zoomButtons = false;
      this.pauseWhenOutOfFocus = true;
      this.screenFlash = true;
      this.snowTransparency = 1f;
      this.invertScrollDirection = false;
      this.ambientOnlyToggle = false;
      this.showAdvancedCraftingInformation = false;
      this.stowingMode = Options.ItemStowingModes.Off;
      this.useLegacySlingshotFiring = false;
      this.gamepadMode = Options.GamepadModes.Auto;
      this.windowedBorderlessFullscreen = true;
      this.showPlacementTileForGamepad = true;
      this.lightingQuality = 32;
      this.hardwareCursor = false;
      this.musicVolumeLevel = 0.75f;
      this.ambientVolumeLevel = 0.75f;
      this.footstepVolumeLevel = 0.9f;
      this.soundVolumeLevel = 1f;
      this.preferredResolutionX = Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes.Last<DisplayMode>().Width;
      this.preferredResolutionY = Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes.Last<DisplayMode>().Height;
      this.vsyncEnabled = true;
      GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
      this.snappyMenus = true;
      this.ipConnectionsEnabled = true;
      this.enableServer = true;
      this.serverPrivacy = ServerPrivacy.FriendsOnly;
      this.enableFarmhandCreation = true;
      this.showMPEndOfNightReadyStatus = false;
      this.muteAnimalSounds = false;
    }

    public void setControlsToDefault()
    {
      this.actionButton = new InputButton[2]
      {
        new InputButton(Keys.X),
        new InputButton(false)
      };
      this.cancelButton = new InputButton[1]
      {
        new InputButton(Keys.V)
      };
      this.useToolButton = new InputButton[2]
      {
        new InputButton(Keys.C),
        new InputButton(true)
      };
      this.moveUpButton = new InputButton[1]
      {
        new InputButton(Keys.W)
      };
      this.moveRightButton = new InputButton[1]
      {
        new InputButton(Keys.D)
      };
      this.moveDownButton = new InputButton[1]
      {
        new InputButton(Keys.S)
      };
      this.moveLeftButton = new InputButton[1]
      {
        new InputButton(Keys.A)
      };
      this.menuButton = new InputButton[2]
      {
        new InputButton(Keys.E),
        new InputButton(Keys.Escape)
      };
      this.runButton = new InputButton[1]
      {
        new InputButton(Keys.LeftShift)
      };
      this.tmpKeyToReplace = new InputButton[1]
      {
        new InputButton(Keys.None)
      };
      this.chatButton = new InputButton[2]
      {
        new InputButton(Keys.T),
        new InputButton(Keys.OemQuestion)
      };
      this.mapButton = new InputButton[1]
      {
        new InputButton(Keys.M)
      };
      this.journalButton = new InputButton[1]
      {
        new InputButton(Keys.F)
      };
      this.inventorySlot1 = new InputButton[1]
      {
        new InputButton(Keys.D1)
      };
      this.inventorySlot2 = new InputButton[1]
      {
        new InputButton(Keys.D2)
      };
      this.inventorySlot3 = new InputButton[1]
      {
        new InputButton(Keys.D3)
      };
      this.inventorySlot4 = new InputButton[1]
      {
        new InputButton(Keys.D4)
      };
      this.inventorySlot5 = new InputButton[1]
      {
        new InputButton(Keys.D5)
      };
      this.inventorySlot6 = new InputButton[1]
      {
        new InputButton(Keys.D6)
      };
      this.inventorySlot7 = new InputButton[1]
      {
        new InputButton(Keys.D7)
      };
      this.inventorySlot8 = new InputButton[1]
      {
        new InputButton(Keys.D8)
      };
      this.inventorySlot9 = new InputButton[1]
      {
        new InputButton(Keys.D9)
      };
      this.inventorySlot10 = new InputButton[1]
      {
        new InputButton(Keys.D0)
      };
      this.inventorySlot11 = new InputButton[1]
      {
        new InputButton(Keys.OemMinus)
      };
      this.inventorySlot12 = new InputButton[1]
      {
        new InputButton(Keys.OemPlus)
      };
      this.emoteButton = new InputButton[1]
      {
        new InputButton(Keys.Y)
      };
      this.toolbarSwap = new InputButton[1]
      {
        new InputButton(Keys.Tab)
      };
    }

    public string getNameOfOptionFromIndex(int index)
    {
      switch (index)
      {
        case 0:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4556");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4557");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4558");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4559");
        case 4:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4560");
        case 5:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4561");
        case 6:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4562");
        default:
          return "";
      }
    }

    public int whatTypeOfOption(int index)
    {
      switch (index)
      {
        case 1:
        case 2:
          return 2;
        case 6:
          return 3;
        default:
          return 1;
      }
    }

    public void changeCheckBoxOption(int which, bool value)
    {
      switch (which)
      {
        case 0:
          this.autoRun = value;
          Game1.player.setRunning(this.autoRun);
          break;
        case 3:
          this.dialogueTyping = value;
          break;
        case 7:
          this.showPortraits = value;
          break;
        case 8:
          this.showMerchantPortraits = value;
          break;
        case 9:
          this.showMenuBackground = value;
          break;
        case 10:
          this.playFootstepSounds = value;
          break;
        case 11:
          this.alwaysShowToolHitLocation = value;
          break;
        case 12:
          this.hideToolHitLocationWhenInMotion = value;
          break;
        case 14:
          this.pauseWhenOutOfFocus = value;
          break;
        case 15:
          this.pinToolbarToggle = value;
          break;
        case 16:
          this.rumble = value;
          break;
        case 17:
          this.ambientOnlyToggle = value;
          break;
        case 19:
          this.zoomButtons = value;
          break;
        case 22:
          this.invertScrollDirection = value;
          break;
        case 24:
          this.screenFlash = value;
          break;
        case 26:
          this.hardwareCursor = value;
          Program.gamePtr.IsMouseVisible = this.hardwareCursor;
          break;
        case 27:
          this.showPlacementTileForGamepad = value;
          break;
        case 29:
          this.snappyMenus = value;
          break;
        case 30:
          this.ipConnectionsEnabled = value;
          break;
        case 32:
          this.enableFarmhandCreation = value;
          IGameServer server = Game1.server;
          if (server != null)
          {
            server.updateLobbyData();
            break;
          }
          break;
        case 34:
          this.showAdvancedCraftingInformation = value;
          break;
        case 35:
          this.showMPEndOfNightReadyStatus = value;
          break;
        case 37:
          this.vsyncEnabled = value;
          GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
          break;
        case 43:
          this.muteAnimalSounds = value;
          break;
      }
      this.optionsDirty = true;
    }

    public void changeSliderOption(int which, int value)
    {
      switch (which)
      {
        case 1:
          this.musicVolumeLevel = (float) value / 100f;
          Game1.musicCategory.SetVolume(this.musicVolumeLevel);
          Game1.musicPlayerVolume = this.musicVolumeLevel;
          break;
        case 2:
          this.soundVolumeLevel = (float) value / 100f;
          Game1.soundCategory.SetVolume(this.soundVolumeLevel);
          break;
        case 18:
          int num1 = (int) ((double) this.desiredBaseZoomLevel * 100.0);
          int num2 = num1;
          int num3 = (int) ((double) value * 100.0);
          if (num3 >= num1 + 10 || num3 >= 100)
            num1 = Math.Min(100, num1 + 10);
          else if (num3 <= num1 - 10 || num3 <= 50)
            num1 = Math.Max(50, num1 - 10);
          if (num1 != num2)
          {
            this.desiredBaseZoomLevel = (float) num1 / 100f;
            Game1.forceSnapOnNextViewportUpdate = true;
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4563") + this.zoomLevel.ToString());
            break;
          }
          break;
        case 20:
          this.ambientVolumeLevel = (float) value / 100f;
          Game1.ambientCategory.SetVolume(this.ambientVolumeLevel);
          Game1.ambientPlayerVolume = this.ambientVolumeLevel;
          break;
        case 21:
          this.footstepVolumeLevel = (float) value / 100f;
          Game1.footstepCategory.SetVolume(this.footstepVolumeLevel);
          break;
        case 23:
          this.snowTransparency = (float) value / 100f;
          break;
        case 39:
          int num4 = (int) ((double) this.desiredUIScale * 100.0);
          int num5 = (int) ((double) value * 100.0);
          if (num5 >= num4 + 10 || num5 >= 100)
            num4 = Math.Min(100, num4 + 10);
          else if (num5 <= num4 - 10 || num5 <= 50)
            num4 = Math.Max(50, num4 - 10);
          this.desiredUIScale = (float) num4 / 100f;
          break;
      }
      this.optionsDirty = true;
    }

    public void setStowingMode(string setting)
    {
      if (setting == "off")
        this.stowingMode = Options.ItemStowingModes.Off;
      else if (setting == "gamepad")
      {
        this.stowingMode = Options.ItemStowingModes.GamepadOnly;
      }
      else
      {
        if (!(setting == "both"))
          return;
        this.stowingMode = Options.ItemStowingModes.Both;
      }
    }

    public void setSlingshotMode(string setting)
    {
      if (setting == "legacy")
        this.useLegacySlingshotFiring = true;
      else
        this.useLegacySlingshotFiring = false;
    }

    public void setBiteChime(string setting)
    {
      try
      {
        Game1.player.biteChime.Value = int.Parse(setting);
      }
      catch (Exception ex)
      {
        Game1.player.biteChime.Value = -1;
      }
    }

    public void setGamepadMode(string setting)
    {
      if (setting == "auto")
        this.gamepadMode = Options.GamepadModes.Auto;
      else if (setting == "force_on")
        this.gamepadMode = Options.GamepadModes.ForceOn;
      else if (setting == "force_off")
        this.gamepadMode = Options.GamepadModes.ForceOff;
      try
      {
        StartupPreferences startupPreferences = new StartupPreferences();
        startupPreferences.loadPreferences(false, false);
        startupPreferences.gamepadMode = this.gamepadMode;
        startupPreferences.savePreferences(false);
      }
      catch (Exception ex)
      {
      }
    }

    public void setMoveBuildingPermissions(string setting)
    {
      if (setting == "off")
        Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.Off;
      if (setting == "on")
        Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.On;
      if (!(setting == "owned"))
        return;
      Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.OwnedBuildings;
    }

    public void setServerMode(string setting)
    {
      if (setting == "offline")
      {
        this.enableServer = false;
        Game1.multiplayer.Disconnect(Multiplayer.DisconnectType.ServerOfflineMode);
      }
      else
      {
        if (setting == "friends")
          this.serverPrivacy = ServerPrivacy.FriendsOnly;
        else if (setting == "invite")
          this.serverPrivacy = ServerPrivacy.InviteOnly;
        if (Game1.server == null && Game1.client == null)
        {
          this.enableServer = true;
          Game1.multiplayer.StartServer();
        }
        else
        {
          if (Game1.server == null)
            return;
          this.enableServer = true;
          Game1.server.setPrivacy(this.serverPrivacy);
        }
      }
    }

    public void setWindowedOption(string setting)
    {
      if (!(setting == "Windowed"))
      {
        if (!(setting == "Fullscreen"))
        {
          if (!(setting == "Windowed Borderless"))
            return;
          this.setWindowedOption(0);
        }
        else
          this.setWindowedOption(2);
      }
      else
        this.setWindowedOption(1);
    }

    public void setWindowedOption(int setting)
    {
      this.windowedBorderlessFullscreen = this.isCurrentlyWindowedBorderless();
      this.fullscreen = !this.windowedBorderlessFullscreen && Game1.graphics.IsFullScreen;
      int num = -1;
      switch (setting)
      {
        case 0:
          if (!this.windowedBorderlessFullscreen)
          {
            this.windowedBorderlessFullscreen = true;
            Game1.toggleFullscreen();
            this.fullscreen = false;
          }
          num = 0;
          break;
        case 1:
          if (Game1.graphics.IsFullScreen && !this.windowedBorderlessFullscreen)
          {
            this.fullscreen = false;
            Game1.toggleNonBorderlessWindowedFullscreen();
            this.windowedBorderlessFullscreen = false;
          }
          else if (this.windowedBorderlessFullscreen)
          {
            this.fullscreen = false;
            this.windowedBorderlessFullscreen = false;
            Game1.toggleFullscreen();
          }
          num = 1;
          break;
        case 2:
          if (this.windowedBorderlessFullscreen)
          {
            this.fullscreen = true;
            this.windowedBorderlessFullscreen = false;
            Game1.toggleFullscreen();
          }
          else if (!Game1.graphics.IsFullScreen)
          {
            this.fullscreen = true;
            this.windowedBorderlessFullscreen = false;
            Game1.toggleNonBorderlessWindowedFullscreen();
            this.hardwareCursor = false;
            Program.gamePtr.IsMouseVisible = false;
          }
          num = 2;
          break;
      }
      int gameMode = (int) Game1.gameMode;
      try
      {
        StartupPreferences startupPreferences = new StartupPreferences();
        startupPreferences.loadPreferences(false, false);
        startupPreferences.windowMode = num;
        startupPreferences.fullscreenResolutionX = this.preferredResolutionX;
        startupPreferences.fullscreenResolutionY = this.preferredResolutionY;
        startupPreferences.displayIndex = GameRunner.instance.Window.GetDisplayIndex();
        startupPreferences.savePreferences(false);
      }
      catch (Exception ex)
      {
      }
    }

    public void changeDropDownOption(int which, string value)
    {
      switch (which)
      {
        case 6:
          Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
          Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width, Game1.uiViewport.Height);
          string str = value;
          int int32_1 = Convert.ToInt32(str.Split(' ')[0]);
          int int32_2 = Convert.ToInt32(str.Split(' ')[2]);
          this.preferredResolutionX = int32_1;
          this.preferredResolutionY = int32_2;
          Game1.graphics.PreferredBackBufferWidth = int32_1;
          Game1.graphics.PreferredBackBufferHeight = int32_2;
          if (!this.isCurrentlyWindowed())
          {
            try
            {
              StartupPreferences startupPreferences = new StartupPreferences();
              startupPreferences.loadPreferences(false, false);
              startupPreferences.fullscreenResolutionX = this.preferredResolutionX;
              startupPreferences.fullscreenResolutionY = this.preferredResolutionY;
              startupPreferences.savePreferences(false);
            }
            catch (Exception ex)
            {
            }
          }
          Game1.graphics.ApplyChanges();
          GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
          break;
        case 13:
          this.setWindowedOption(value);
          break;
        case 18:
          this.desiredBaseZoomLevel = (float) Convert.ToInt32(value.Replace("%", "")) / 100f;
          Game1.forceSnapOnNextViewportUpdate = true;
          if (Game1.debrisWeather != null)
            Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
          Game1.randomizeRainPositions();
          break;
        case 25:
          if (!(value == "Lowest"))
          {
            if (!(value == "Low"))
            {
              if (!(value == "Med."))
              {
                if (!(value == "High"))
                {
                  if (value == "Ultra")
                    this.lightingQuality = 8;
                }
                else
                  this.lightingQuality = 16;
              }
              else
                this.lightingQuality = 32;
            }
            else
              this.lightingQuality = 64;
          }
          else
            this.lightingQuality = 128;
          Game1.overrideGameMenuReset = true;
          Program.gamePtr.refreshWindowSettings();
          Game1.overrideGameMenuReset = false;
          break;
        case 28:
          this.setStowingMode(value);
          break;
        case 31:
          this.setServerMode(value);
          break;
        case 38:
          this.setGamepadMode(value);
          break;
        case 39:
          this.desiredUIScale = (float) Convert.ToInt32(value.Replace("%", "")) / 100f;
          break;
        case 40:
          this.setMoveBuildingPermissions(value);
          break;
        case 41:
          this.setSlingshotMode(value);
          break;
        case 42:
          this.setBiteChime(value);
          Game1.player.PlayFishBiteChime();
          break;
      }
      this.optionsDirty = true;
    }

    public bool isKeyInUse(Keys key)
    {
      foreach (InputButton allUsedInputButton in this.getAllUsedInputButtons())
      {
        if (allUsedInputButton.key == key)
          return true;
      }
      return false;
    }

    public List<InputButton> getAllUsedInputButtons()
    {
      List<InputButton> usedInputButtons = new List<InputButton>();
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.useToolButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.actionButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.moveUpButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.moveRightButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.moveDownButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.moveLeftButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.runButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.menuButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.journalButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.mapButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.chatButton);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot1);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot2);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot3);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot4);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot5);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot6);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot7);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot8);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot9);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot10);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot11);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.inventorySlot12);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.toolbarSwap);
      usedInputButtons.AddRange((IEnumerable<InputButton>) this.emoteButton);
      return usedInputButtons;
    }

    public void setCheckBoxToProperValue(OptionsCheckbox checkbox)
    {
      switch (checkbox.whichOption)
      {
        case 0:
          checkbox.isChecked = this.autoRun;
          break;
        case 3:
          checkbox.isChecked = this.dialogueTyping;
          break;
        case 4:
          this.fullscreen = Game1.graphics.IsFullScreen || this.windowedBorderlessFullscreen;
          checkbox.isChecked = this.fullscreen;
          break;
        case 5:
          checkbox.isChecked = this.windowedBorderlessFullscreen;
          checkbox.greyedOut = !this.fullscreen;
          break;
        case 7:
          checkbox.isChecked = this.showPortraits;
          break;
        case 8:
          checkbox.isChecked = this.showMerchantPortraits;
          break;
        case 9:
          checkbox.isChecked = this.showMenuBackground;
          break;
        case 10:
          checkbox.isChecked = this.playFootstepSounds;
          break;
        case 11:
          checkbox.isChecked = this.alwaysShowToolHitLocation;
          break;
        case 12:
          checkbox.isChecked = this.hideToolHitLocationWhenInMotion;
          break;
        case 14:
          checkbox.isChecked = this.pauseWhenOutOfFocus;
          break;
        case 15:
          checkbox.isChecked = this.pinToolbarToggle;
          break;
        case 16:
          checkbox.isChecked = this.rumble;
          checkbox.greyedOut = !this.gamepadControls;
          break;
        case 17:
          checkbox.isChecked = this.ambientOnlyToggle;
          break;
        case 19:
          checkbox.isChecked = this.zoomButtons;
          break;
        case 22:
          checkbox.isChecked = this.invertScrollDirection;
          break;
        case 24:
          checkbox.isChecked = this.screenFlash;
          break;
        case 26:
          checkbox.isChecked = this._hardwareCursor;
          checkbox.greyedOut = this.fullscreen;
          break;
        case 27:
          checkbox.isChecked = this.showPlacementTileForGamepad;
          checkbox.greyedOut = !this.gamepadControls;
          break;
        case 29:
          checkbox.isChecked = this.snappyMenus;
          break;
        case 30:
          checkbox.isChecked = this.ipConnectionsEnabled;
          break;
        case 32:
          checkbox.isChecked = this.enableFarmhandCreation;
          break;
        case 34:
          checkbox.isChecked = this.showAdvancedCraftingInformation;
          break;
        case 35:
          checkbox.isChecked = this.showMPEndOfNightReadyStatus;
          break;
        case 37:
          checkbox.isChecked = this.vsyncEnabled;
          break;
        case 43:
          checkbox.isChecked = this.muteAnimalSounds;
          break;
      }
    }

    public void setPlusMinusToProperValue(OptionsPlusMinus plusMinus)
    {
      switch (plusMinus.whichOption)
      {
        case 18:
          string str1 = Math.Round((double) this.desiredBaseZoomLevel * 100.0).ToString() + "%";
          for (int index = 0; index < plusMinus.options.Count; ++index)
          {
            if (plusMinus.options[index].Equals(str1))
            {
              plusMinus.selected = index;
              break;
            }
          }
          break;
        case 25:
          string str2 = "";
          switch (this.lightingQuality)
          {
            case 8:
              str2 = "Ultra";
              break;
            case 16:
              str2 = "High";
              break;
            case 32:
              str2 = "Med.";
              break;
            case 64:
              str2 = "Low";
              break;
            case 128:
              str2 = "Lowest";
              break;
          }
          for (int index = 0; index < plusMinus.options.Count; ++index)
          {
            if (plusMinus.options[index].Equals(str2))
            {
              plusMinus.selected = index;
              break;
            }
          }
          break;
        case 39:
          string str3 = Math.Round((double) this.desiredUIScale * 100.0).ToString() + "%";
          for (int index = 0; index < plusMinus.options.Count; ++index)
          {
            if (plusMinus.options[index].Equals(str3))
            {
              plusMinus.selected = index;
              break;
            }
          }
          break;
      }
    }

    public void setSliderToProperValue(OptionsSlider slider)
    {
      switch (slider.whichOption)
      {
        case 1:
          slider.value = (int) ((double) this.musicVolumeLevel * 100.0);
          break;
        case 2:
          slider.value = (int) ((double) this.soundVolumeLevel * 100.0);
          break;
        case 18:
          slider.value = (int) ((double) this.desiredBaseZoomLevel * 100.0);
          break;
        case 20:
          slider.value = (int) ((double) this.ambientVolumeLevel * 100.0);
          break;
        case 21:
          slider.value = (int) ((double) this.footstepVolumeLevel * 100.0);
          break;
        case 23:
          slider.value = (int) ((double) this.snowTransparency * 100.0);
          break;
        case 39:
          slider.value = (int) ((double) this.desiredUIScale * 100.0);
          break;
      }
    }

    public bool doesInputListContain(InputButton[] list, Keys key)
    {
      for (int index = 0; index < list.Length; ++index)
      {
        if (list[index].key == key)
          return true;
      }
      return false;
    }

    public void changeInputListenerValue(int whichListener, Keys key)
    {
      switch (whichListener)
      {
        case 7:
          this.actionButton[0] = new InputButton(key);
          break;
        case 10:
          this.useToolButton[0] = new InputButton(key);
          break;
        case 11:
          this.moveUpButton[0] = new InputButton(key);
          break;
        case 12:
          this.moveRightButton[0] = new InputButton(key);
          break;
        case 13:
          this.moveDownButton[0] = new InputButton(key);
          break;
        case 14:
          this.moveLeftButton[0] = new InputButton(key);
          break;
        case 15:
          this.menuButton[0] = new InputButton(key);
          break;
        case 16:
          this.runButton[0] = new InputButton(key);
          break;
        case 17:
          this.chatButton[0] = new InputButton(key);
          break;
        case 18:
          this.journalButton[0] = new InputButton(key);
          break;
        case 19:
          this.mapButton[0] = new InputButton(key);
          break;
        case 20:
          this.inventorySlot1[0] = new InputButton(key);
          break;
        case 21:
          this.inventorySlot2[0] = new InputButton(key);
          break;
        case 22:
          this.inventorySlot3[0] = new InputButton(key);
          break;
        case 23:
          this.inventorySlot4[0] = new InputButton(key);
          break;
        case 24:
          this.inventorySlot5[0] = new InputButton(key);
          break;
        case 25:
          this.inventorySlot6[0] = new InputButton(key);
          break;
        case 26:
          this.inventorySlot7[0] = new InputButton(key);
          break;
        case 27:
          this.inventorySlot8[0] = new InputButton(key);
          break;
        case 28:
          this.inventorySlot9[0] = new InputButton(key);
          break;
        case 29:
          this.inventorySlot10[0] = new InputButton(key);
          break;
        case 30:
          this.inventorySlot11[0] = new InputButton(key);
          break;
        case 31:
          this.inventorySlot12[0] = new InputButton(key);
          break;
        case 32:
          this.toolbarSwap[0] = new InputButton(key);
          break;
        case 33:
          this.emoteButton[0] = new InputButton(key);
          break;
      }
      this.optionsDirty = true;
    }

    public void setInputListenerToProperValue(OptionsInputListener inputListener)
    {
      inputListener.buttonNames.Clear();
      switch (inputListener.whichOption)
      {
        case 7:
          foreach (InputButton inputButton in this.actionButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 10:
          foreach (InputButton inputButton in this.useToolButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 11:
          foreach (InputButton inputButton in this.moveUpButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 12:
          foreach (InputButton inputButton in this.moveRightButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 13:
          foreach (InputButton inputButton in this.moveDownButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 14:
          foreach (InputButton inputButton in this.moveLeftButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 15:
          foreach (InputButton inputButton in this.menuButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 16:
          foreach (InputButton inputButton in this.runButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 17:
          foreach (InputButton inputButton in this.chatButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 18:
          foreach (InputButton inputButton in this.journalButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 19:
          foreach (InputButton inputButton in this.mapButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 20:
          foreach (InputButton inputButton in this.inventorySlot1)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 21:
          foreach (InputButton inputButton in this.inventorySlot2)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 22:
          foreach (InputButton inputButton in this.inventorySlot3)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 23:
          foreach (InputButton inputButton in this.inventorySlot4)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 24:
          foreach (InputButton inputButton in this.inventorySlot5)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 25:
          foreach (InputButton inputButton in this.inventorySlot6)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 26:
          foreach (InputButton inputButton in this.inventorySlot7)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 27:
          foreach (InputButton inputButton in this.inventorySlot8)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 28:
          foreach (InputButton inputButton in this.inventorySlot9)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 29:
          foreach (InputButton inputButton in this.inventorySlot10)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 30:
          foreach (InputButton inputButton in this.inventorySlot11)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 31:
          foreach (InputButton inputButton in this.inventorySlot12)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 32:
          foreach (InputButton inputButton in this.toolbarSwap)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
        case 33:
          foreach (InputButton inputButton in this.emoteButton)
            inputListener.buttonNames.Add(inputButton.ToString());
          break;
      }
    }

    public void setDropDownToProperValue(OptionsDropDown dropDown)
    {
      switch (dropDown.whichOption)
      {
        case 6:
          try
          {
            StartupPreferences startupPreferences = new StartupPreferences();
            startupPreferences.loadPreferences(false, false);
            if (startupPreferences.fullscreenResolutionX != 0)
            {
              this.preferredResolutionX = startupPreferences.fullscreenResolutionX;
              this.preferredResolutionY = startupPreferences.fullscreenResolutionY;
            }
          }
          catch (Exception ex)
          {
          }
          int num1 = 0;
          foreach (DisplayMode supportedDisplayMode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
          {
            if (supportedDisplayMode.Width >= 1280)
            {
              List<string> dropDownOptions = dropDown.dropDownOptions;
              int num2 = supportedDisplayMode.Width;
              string str1 = num2.ToString();
              num2 = supportedDisplayMode.Height;
              string str2 = num2.ToString();
              string str3 = str1 + " x " + str2;
              dropDownOptions.Add(str3);
              List<string> downDisplayOptions = dropDown.dropDownDisplayOptions;
              num2 = supportedDisplayMode.Width;
              string str4 = num2.ToString();
              num2 = supportedDisplayMode.Height;
              string str5 = num2.ToString();
              string str6 = str4 + " x " + str5;
              downDisplayOptions.Add(str6);
              if (supportedDisplayMode.Width == this.preferredResolutionX && supportedDisplayMode.Height == this.preferredResolutionY)
                dropDown.selectedOption = num1;
              ++num1;
            }
          }
          dropDown.greyedOut = !this.fullscreen || this.windowedBorderlessFullscreen;
          break;
        case 13:
          this.windowedBorderlessFullscreen = this.isCurrentlyWindowedBorderless();
          this.fullscreen = Game1.graphics.IsFullScreen && !this.windowedBorderlessFullscreen;
          dropDown.dropDownOptions.Add("Windowed");
          if (!this.windowedBorderlessFullscreen)
            dropDown.dropDownOptions.Add("Fullscreen");
          if (!this.fullscreen)
            dropDown.dropDownOptions.Add("Windowed Borderless");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4564"));
          if (!this.windowedBorderlessFullscreen)
            dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4560"));
          if (!this.fullscreen)
            dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Options.cs.4561"));
          if (Game1.graphics.IsFullScreen || this.windowedBorderlessFullscreen)
          {
            dropDown.selectedOption = 1;
            break;
          }
          dropDown.selectedOption = 0;
          break;
        case 28:
          dropDown.dropDownOptions.Add("off");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_StowingMode_Off"));
          dropDown.dropDownOptions.Add("gamepad");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_StowingMode_GamepadOnly"));
          dropDown.dropDownOptions.Add("both");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_StowingMode_On"));
          if (this.stowingMode == Options.ItemStowingModes.Off)
          {
            dropDown.selectedOption = 0;
            break;
          }
          if (this.stowingMode == Options.ItemStowingModes.GamepadOnly)
          {
            dropDown.selectedOption = 1;
            break;
          }
          if (this.stowingMode != Options.ItemStowingModes.Both)
            break;
          dropDown.selectedOption = 2;
          break;
        case 31:
          dropDown.dropDownOptions.Add("offline");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:GameMenu_ServerMode_Offline"));
          if (Program.sdk.Networking != null)
          {
            dropDown.dropDownOptions.Add("friends");
            dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:GameMenu_ServerMode_FriendsOnly"));
            dropDown.dropDownOptions.Add("invite");
            dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:GameMenu_ServerMode_InviteOnly"));
          }
          else
          {
            dropDown.dropDownOptions.Add("online");
            dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:GameMenu_ServerMode_Online"));
          }
          if (Game1.server == null)
            dropDown.selectedOption = 0;
          else if (Program.sdk.Networking != null)
          {
            if (this.serverPrivacy == ServerPrivacy.FriendsOnly)
              dropDown.selectedOption = 1;
            else if (this.serverPrivacy == ServerPrivacy.InviteOnly)
              dropDown.selectedOption = 2;
          }
          else
            dropDown.selectedOption = 1;
          Console.WriteLine("setDropDownToProperValue( serverMode, {0} ) called.", (object) dropDown.dropDownOptions[dropDown.selectedOption]);
          break;
        case 38:
          try
          {
            StartupPreferences startupPreferences = new StartupPreferences();
            startupPreferences.loadPreferences(false, false);
            this.gamepadMode = startupPreferences.gamepadMode;
          }
          catch (Exception ex)
          {
          }
          dropDown.dropDownOptions.Add("auto");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_GamepadMode_Auto"));
          dropDown.dropDownOptions.Add("force_on");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_GamepadMode_ForceOn"));
          dropDown.dropDownOptions.Add("force_off");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_GamepadMode_ForceOff"));
          if (this.gamepadMode == Options.GamepadModes.Auto)
          {
            dropDown.selectedOption = 0;
            break;
          }
          if (this.gamepadMode == Options.GamepadModes.ForceOn)
          {
            dropDown.selectedOption = 1;
            break;
          }
          if (this.gamepadMode != Options.GamepadModes.ForceOff)
            break;
          dropDown.selectedOption = 2;
          break;
        case 40:
          dropDown.dropDownOptions.Add("on");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions_On"));
          dropDown.dropDownOptions.Add("owned");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions_Owned"));
          dropDown.dropDownOptions.Add("off");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions_Off"));
          if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On)
            dropDown.selectedOption = 0;
          if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings)
            dropDown.selectedOption = 1;
          if (Game1.player.team.farmhandsCanMoveBuildings.Value != FarmerTeam.RemoteBuildingPermissions.Off)
            break;
          dropDown.selectedOption = 2;
          break;
        case 41:
          dropDown.dropDownOptions.Add("hold");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_SlingshotMode_Hold"));
          dropDown.dropDownOptions.Add("legacy");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\UI:Options_SlingshotMode_Pull"));
          if (this.useLegacySlingshotFiring)
          {
            dropDown.selectedOption = 1;
            break;
          }
          dropDown.selectedOption = 0;
          break;
        case 42:
          dropDown.dropDownOptions.Add("-1");
          dropDown.dropDownDisplayOptions.Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:BiteChime_Default"));
          for (int index = 0; index <= 3; ++index)
          {
            dropDown.dropDownOptions.Add(index.ToString());
            dropDown.dropDownDisplayOptions.Add((index + 1).ToString());
          }
          dropDown.selectedOption = Game1.player.biteChime.Value + 1;
          break;
      }
    }

    public bool isCurrentlyWindowedBorderless() => Game1.graphics.IsFullScreen && !Game1.graphics.HardwareModeSwitch;

    public bool isCurrentlyFullscreen() => Game1.graphics.IsFullScreen && Game1.graphics.HardwareModeSwitch;

    public bool isCurrentlyWindowed() => !this.isCurrentlyWindowedBorderless() && !this.isCurrentlyFullscreen();

    public enum ItemStowingModes
    {
      Off,
      GamepadOnly,
      Both,
    }

    public enum GamepadModes
    {
      Auto,
      ForceOn,
      ForceOff,
    }
  }
}
