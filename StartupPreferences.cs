// Decompiled with JetBrains decompiler
// Type: StardewValley.StartupPreferences
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StardewValley
{
  public class StartupPreferences
  {
    public const int windowed_borderless = 0;
    public const int windowed = 1;
    public const int fullscreen = 2;
    private static readonly string _filename = "startup_preferences";
    public static XmlSerializer serializer = new XmlSerializer(typeof (StartupPreferences));
    public bool startMuted;
    public bool levelTenFishing;
    public bool levelTenMining;
    public bool levelTenForaging;
    public bool levelTenCombat;
    public bool skipWindowPreparation;
    public bool sawAdvancedCharacterCreationIndicator;
    public int timesPlayed;
    public int windowMode;
    public int displayIndex = -1;
    public Options.GamepadModes gamepadMode;
    public int playerLimit = -1;
    public int fullscreenResolutionX;
    public int fullscreenResolutionY;
    public string lastEnteredIP = "";
    public string languageCode;
    public Options clientOptions = new Options();
    [XmlIgnore]
    public bool isLoaded;
    private bool _isBusy;
    private bool _pendingApplyLanguage;
    private Task _task;

    [XmlIgnore]
    public bool IsBusy
    {
      get
      {
        lock (this)
        {
          if (!this._isBusy)
            return false;
          if (this._task == null)
            throw new Exception("StartupPreferences.IsBusy; was busy but task is null?");
          if (this._task.IsFaulted)
          {
            Exception baseException = this._task.Exception.GetBaseException();
            Console.WriteLine("StartupPreferences._task failed with an exception");
            Console.WriteLine((object) baseException);
            throw baseException;
          }
          if (this._task.IsCompleted)
          {
            this._task = (Task) null;
            this._isBusy = false;
            if (this._pendingApplyLanguage)
              this._SetLanguageFromCode(this.languageCode);
          }
          return this._isBusy;
        }
      }
    }

    private void Init()
    {
      this.isLoaded = false;
      this.ensureFolderStructureExists();
    }

    public void OnLanguageChange(LocalizedContentManager.LanguageCode code)
    {
      string id = code.ToString();
      if (code == LocalizedContentManager.LanguageCode.mod && LocalizedContentManager.CurrentModLanguage != null)
        id = LocalizedContentManager.CurrentModLanguage.ID;
      if (!this.isLoaded || !(this.languageCode != id))
        return;
      this.savePreferences(false, true);
    }

    private void ensureFolderStructureExists()
    {
      FileInfo fileInfo = new FileInfo(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "placeholder")));
      if (!fileInfo.Directory.Exists)
        fileInfo.Directory.Create();
    }

    public void savePreferences(bool async, bool update_language_from_ingame_language = false)
    {
      lock (this)
      {
        if (update_language_from_ingame_language)
          this.languageCode = LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.mod ? LocalizedContentManager.CurrentLanguageCode.ToString() : LocalizedContentManager.CurrentModLanguage.ID;
        Console.WriteLine("savePreferences(); async={0}, languageCode={1}", (object) async, (object) this.languageCode);
        try
        {
          this._savePreferences();
        }
        catch (Exception ex)
        {
          Exception baseException = ex.GetBaseException();
          Console.WriteLine("StartupPreferences._task failed with an exception");
          Console.WriteLine((object) baseException.GetType());
          Console.WriteLine(baseException.Message);
          Console.WriteLine(baseException.StackTrace);
          throw ex;
        }
      }
    }

    private void _savePreferences()
    {
      string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), StartupPreferences._filename);
      try
      {
        this.ensureFolderStructureExists();
        if (File.Exists(path))
          File.Delete(path);
        using (FileStream fileStream = File.Create(path))
          this.writeSettings((Stream) fileStream);
      }
      catch (Exception ex)
      {
        Game1.debugOutput = Game1.parseText(ex.Message);
      }
    }

    private long writeSettings(Stream stream)
    {
      using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings()
      {
        CloseOutput = true
      }))
      {
        xmlWriter.WriteStartDocument();
        StartupPreferences.serializer.Serialize(xmlWriter, (object) this);
        xmlWriter.WriteEndDocument();
        xmlWriter.Flush();
        return stream.Length;
      }
    }

    public void loadPreferences(bool async, bool applyLanguage)
    {
      lock (this)
      {
        this._pendingApplyLanguage = applyLanguage;
        Console.WriteLine("loadPreferences(); begin - languageCode={0}", (object) this.languageCode);
        this.Init();
        try
        {
          this._loadPreferences();
        }
        catch (Exception ex)
        {
          Exception baseException = this._task.Exception.GetBaseException();
          Console.WriteLine("StartupPreferences._task failed with an exception");
          Console.WriteLine((object) baseException.GetType());
          Console.WriteLine(baseException.Message);
          Console.WriteLine(baseException.StackTrace);
          throw baseException;
        }
        if (!applyLanguage)
          return;
        this._SetLanguageFromCode(this.languageCode);
      }
    }

    protected virtual void _SetLanguageFromCode(string language_code_string)
    {
      List<ModLanguage> modLanguageList = Game1.content.Load<List<ModLanguage>>("Data\\AdditionalLanguages");
      bool flag = false;
      if (modLanguageList != null)
      {
        foreach (ModLanguage new_mod_language in modLanguageList)
        {
          if (new_mod_language.ID == language_code_string)
          {
            LocalizedContentManager.SetModLanguage(new_mod_language);
            flag = true;
            break;
          }
        }
      }
      if (flag)
        return;
      LocalizedContentManager.LanguageCode result = LocalizedContentManager.LanguageCode.en;
      if (Enum.TryParse<LocalizedContentManager.LanguageCode>(language_code_string, out result) && result != LocalizedContentManager.LanguageCode.mod)
        LocalizedContentManager.CurrentLanguageCode = result;
      else
        LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.GetDefaultLanguageCode();
    }

    private void _loadPreferences()
    {
      string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), StartupPreferences._filename);
      if (!File.Exists(path))
      {
        Console.WriteLine("path '{0}' did not exist and will be created", (object) path);
        try
        {
          this.languageCode = LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.mod ? LocalizedContentManager.CurrentLanguageCode.ToString() : LocalizedContentManager.CurrentModLanguage.ID;
          using (FileStream fileStream = File.Create(path))
            this.writeSettings((Stream) fileStream);
        }
        catch (Exception ex)
        {
          Console.WriteLine("_loadPreferences; exception occured trying to create/write: {0}", (object) ex);
          Game1.debugOutput = Game1.parseText(ex.Message);
          return;
        }
      }
      try
      {
        using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
          this.readSettings((Stream) fileStream);
        this.isLoaded = true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("_loadPreferences; exception occured trying open/read: {0}", (object) ex);
        Game1.debugOutput = Game1.parseText(ex.Message);
      }
    }

    private void readSettings(Stream stream)
    {
      StartupPreferences startupPreferences = (StartupPreferences) StartupPreferences.serializer.Deserialize(stream);
      this.startMuted = startupPreferences.startMuted;
      this.timesPlayed = startupPreferences.timesPlayed + 1;
      this.levelTenCombat = startupPreferences.levelTenCombat;
      this.levelTenFishing = startupPreferences.levelTenFishing;
      this.levelTenForaging = startupPreferences.levelTenForaging;
      this.levelTenMining = startupPreferences.levelTenMining;
      this.skipWindowPreparation = startupPreferences.skipWindowPreparation;
      this.windowMode = startupPreferences.windowMode;
      this.displayIndex = startupPreferences.displayIndex;
      this.playerLimit = startupPreferences.playerLimit;
      this.gamepadMode = startupPreferences.gamepadMode;
      this.fullscreenResolutionX = startupPreferences.fullscreenResolutionX;
      this.fullscreenResolutionY = startupPreferences.fullscreenResolutionY;
      this.lastEnteredIP = startupPreferences.lastEnteredIP;
      this.languageCode = startupPreferences.languageCode;
      this.clientOptions = startupPreferences.clientOptions;
    }
  }
}
