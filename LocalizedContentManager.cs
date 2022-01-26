// Decompiled with JetBrains decompiler
// Type: StardewValley.LocalizedContentManager
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Content;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace StardewValley
{
  public class LocalizedContentManager : ContentManager
  {
    public const string genderDialogueSplitCharacter = "¦";
    private static LocalizedContentManager.LanguageCode _currentLangCode = LocalizedContentManager.GetDefaultLanguageCode();
    private static ModLanguage _currentModLanguage = (ModLanguage) null;
    public CultureInfo CurrentCulture;
    protected static StringBuilder _timeFormatStringBuilder = new StringBuilder();
    public static readonly Dictionary<string, string> localizedAssetNames = new Dictionary<string, string>();

    public static event LocalizedContentManager.LanguageChangedHandler OnLanguageChange;

    public static LocalizedContentManager.LanguageCode GetDefaultLanguageCode() => LocalizedContentManager.LanguageCode.en;

    public static LocalizedContentManager.LanguageCode CurrentLanguageCode
    {
      get => LocalizedContentManager._currentLangCode;
      set
      {
        if (LocalizedContentManager._currentLangCode == value)
          return;
        LocalizedContentManager.LanguageCode currentLangCode = LocalizedContentManager._currentLangCode;
        LocalizedContentManager._currentLangCode = value;
        if (LocalizedContentManager._currentLangCode != LocalizedContentManager.LanguageCode.mod)
          LocalizedContentManager._currentModLanguage = (ModLanguage) null;
        Console.WriteLine("LocalizedContentManager.CurrentLanguageCode CHANGING from '{0}' to '{1}'", (object) currentLangCode, (object) LocalizedContentManager._currentLangCode);
        LocalizedContentManager.LanguageChangedHandler onLanguageChange = LocalizedContentManager.OnLanguageChange;
        if (onLanguageChange != null)
          onLanguageChange(LocalizedContentManager._currentLangCode);
        Console.WriteLine("LocalizedContentManager.CurrentLanguageCode CHANGED from '{0}' to '{1}'", (object) currentLangCode, (object) LocalizedContentManager._currentLangCode);
      }
    }

    public static bool CurrentLanguageLatin
    {
      get
      {
        switch (LocalizedContentManager.CurrentLanguageCode)
        {
          case LocalizedContentManager.LanguageCode.en:
          case LocalizedContentManager.LanguageCode.pt:
          case LocalizedContentManager.LanguageCode.es:
          case LocalizedContentManager.LanguageCode.de:
          case LocalizedContentManager.LanguageCode.fr:
          case LocalizedContentManager.LanguageCode.it:
          case LocalizedContentManager.LanguageCode.tr:
          case LocalizedContentManager.LanguageCode.hu:
            return true;
          case LocalizedContentManager.LanguageCode.mod:
            return LocalizedContentManager._currentModLanguage.UseLatinFont;
          default:
            return false;
        }
      }
    }

    public LocalizedContentManager(
      IServiceProvider serviceProvider,
      string rootDirectory,
      CultureInfo currentCulture)
      : base(serviceProvider, rootDirectory)
    {
      this.CurrentCulture = currentCulture;
    }

    public LocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory)
      : this(serviceProvider, rootDirectory, Thread.CurrentThread.CurrentUICulture)
    {
    }

    public static ModLanguage CurrentModLanguage => LocalizedContentManager._currentModLanguage;

    protected static bool _IsStringAt(string source, string string_to_find, int index)
    {
      for (int index1 = 0; index1 < string_to_find.Length; ++index1)
      {
        int index2 = index + index1;
        if (index2 >= source.Length || (int) source[index2] != (int) string_to_find[index1])
          return false;
      }
      return true;
    }

    public static StringBuilder FormatTimeString(int time, string format)
    {
      LocalizedContentManager._timeFormatStringBuilder.Clear();
      int index1 = -1;
      for (int index2 = 0; index2 < format.Length; ++index2)
      {
        char ch = format[index2];
        switch (ch)
        {
          case '[':
            if (index1 < 0)
            {
              index1 = index2;
              break;
            }
            for (int index3 = index1; index3 <= index2; ++index3)
              LocalizedContentManager._timeFormatStringBuilder.Append(format[index3]);
            index1 = index2;
            break;
          case ']':
            if (index1 >= 0)
            {
              int num;
              if (LocalizedContentManager._IsStringAt(format, "[HOURS_12]", index1))
              {
                StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
                string str;
                if (time / 100 % 12 != 0)
                {
                  num = time / 100 % 12;
                  str = num.ToString();
                }
                else
                  str = "12";
                formatStringBuilder.Append(str);
              }
              else if (LocalizedContentManager._IsStringAt(format, "[HOURS_12_0]", index1))
              {
                StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
                string str;
                if (time / 100 % 12 != 0)
                {
                  num = time / 100 % 12;
                  str = num.ToString();
                }
                else
                  str = "0";
                formatStringBuilder.Append(str);
              }
              else if (LocalizedContentManager._IsStringAt(format, "[HOURS_24]", index1))
                LocalizedContentManager._timeFormatStringBuilder.Append(time / 100 % 24);
              else if (LocalizedContentManager._IsStringAt(format, "[HOURS_24_00]", index1))
              {
                StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
                num = time / 100 % 24;
                string str = num.ToString("00");
                formatStringBuilder.Append(str);
              }
              else if (LocalizedContentManager._IsStringAt(format, "[MINUTES]", index1))
              {
                StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
                num = time % 100;
                string str = num.ToString("00");
                formatStringBuilder.Append(str);
              }
              else if (LocalizedContentManager._IsStringAt(format, "[AM_PM]", index1))
              {
                if (time < 1200 || time >= 2400)
                  LocalizedContentManager._timeFormatStringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370"));
                else
                  LocalizedContentManager._timeFormatStringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371"));
              }
              else
              {
                for (int index4 = index1; index4 <= index2; ++index4)
                  LocalizedContentManager._timeFormatStringBuilder.Append(format[index4]);
              }
              index1 = -1;
              break;
            }
            goto default;
          default:
            if (index1 < 0)
            {
              LocalizedContentManager._timeFormatStringBuilder.Append(ch);
              break;
            }
            break;
        }
      }
      return LocalizedContentManager._timeFormatStringBuilder;
    }

    public static void SetModLanguage(ModLanguage new_mod_language)
    {
      if (new_mod_language == LocalizedContentManager._currentModLanguage)
        return;
      LocalizedContentManager._currentModLanguage = new_mod_language;
      LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.mod;
    }

    public virtual T LoadBase<T>(string assetName) => base.Load<T>(assetName);

    public override T Load<T>(string assetName) => this.Load<T>(assetName, LocalizedContentManager.CurrentLanguageCode);

    public virtual T Load<T>(string assetName, LocalizedContentManager.LanguageCode language)
    {
      if (language == LocalizedContentManager.LanguageCode.en)
        return base.Load<T>(assetName);
      if (!LocalizedContentManager.localizedAssetNames.TryGetValue(assetName, out string _))
      {
        string assetName1 = assetName + "." + this.LanguageCodeString(language);
        try
        {
          base.Load<T>(assetName1);
          LocalizedContentManager.localizedAssetNames[assetName] = assetName1;
        }
        catch (ContentLoadException ex1)
        {
          string assetName2 = assetName + "_international";
          try
          {
            base.Load<T>(assetName2);
            LocalizedContentManager.localizedAssetNames[assetName] = assetName2;
          }
          catch (ContentLoadException ex2)
          {
            LocalizedContentManager.localizedAssetNames[assetName] = assetName;
          }
        }
      }
      return base.Load<T>(LocalizedContentManager.localizedAssetNames[assetName]);
    }

    public string LanguageCodeString(LocalizedContentManager.LanguageCode code)
    {
      string str = "";
      switch (code)
      {
        case LocalizedContentManager.LanguageCode.ja:
          str = "ja-JP";
          break;
        case LocalizedContentManager.LanguageCode.ru:
          str = "ru-RU";
          break;
        case LocalizedContentManager.LanguageCode.zh:
          str = "zh-CN";
          break;
        case LocalizedContentManager.LanguageCode.pt:
          str = "pt-BR";
          break;
        case LocalizedContentManager.LanguageCode.es:
          str = "es-ES";
          break;
        case LocalizedContentManager.LanguageCode.de:
          str = "de-DE";
          break;
        case LocalizedContentManager.LanguageCode.th:
          str = "th-TH";
          break;
        case LocalizedContentManager.LanguageCode.fr:
          str = "fr-FR";
          break;
        case LocalizedContentManager.LanguageCode.ko:
          str = "ko-KR";
          break;
        case LocalizedContentManager.LanguageCode.it:
          str = "it-IT";
          break;
        case LocalizedContentManager.LanguageCode.tr:
          str = "tr-TR";
          break;
        case LocalizedContentManager.LanguageCode.hu:
          str = "hu-HU";
          break;
        case LocalizedContentManager.LanguageCode.mod:
          str = LocalizedContentManager._currentModLanguage.LanguageCode;
          break;
      }
      return str;
    }

    public LocalizedContentManager.LanguageCode GetCurrentLanguage() => LocalizedContentManager.CurrentLanguageCode;

    private string GetString(Dictionary<string, string> strings, string key)
    {
      string str;
      return strings.TryGetValue(key + ".desktop", out str) ? str : strings[key];
    }

    public virtual string LoadStringReturnNullIfNotFound(string path)
    {
      string str = this.LoadString(path);
      return !str.Equals(path) ? str : (string) null;
    }

    public virtual string LoadString(string path)
    {
      string assetName;
      string key;
      this.parseStringPath(path, out assetName, out key);
      Dictionary<string, string> strings = this.Load<Dictionary<string, string>>(assetName);
      if (strings == null || !strings.ContainsKey(key))
        return this.LoadBaseString(path);
      string str = this.GetString(strings, key);
      if (str.Contains("¦"))
        str = !Game1.player.IsMale ? str.Substring(str.IndexOf("¦") + 1) : str.Substring(0, str.IndexOf("¦"));
      return str;
    }

    public virtual bool ShouldUseGenderedCharacterTranslations()
    {
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.pt:
          return true;
        case LocalizedContentManager.LanguageCode.mod:
          if (LocalizedContentManager.CurrentModLanguage != null)
            return LocalizedContentManager.CurrentModLanguage.UseGenderedCharacterTranslations;
          break;
      }
      return false;
    }

    public virtual string LoadString(string path, object sub1)
    {
      string format = this.LoadString(path);
      try
      {
        return string.Format(format, sub1);
      }
      catch (Exception ex)
      {
      }
      return format;
    }

    public virtual string LoadString(string path, object sub1, object sub2)
    {
      string format = this.LoadString(path);
      try
      {
        return string.Format(format, sub1, sub2);
      }
      catch (Exception ex)
      {
      }
      return format;
    }

    public virtual string LoadString(string path, object sub1, object sub2, object sub3)
    {
      string format = this.LoadString(path);
      try
      {
        return string.Format(format, sub1, sub2, sub3);
      }
      catch (Exception ex)
      {
      }
      return format;
    }

    public virtual string LoadString(string path, params object[] substitutions)
    {
      string format = this.LoadString(path);
      if (substitutions.Length != 0)
      {
        try
        {
          return string.Format(format, substitutions);
        }
        catch (Exception ex)
        {
        }
      }
      return format;
    }

    public virtual string LoadBaseString(string path)
    {
      string assetName;
      string key;
      this.parseStringPath(path, out assetName, out key);
      Dictionary<string, string> strings = base.Load<Dictionary<string, string>>(assetName);
      return strings != null && strings.ContainsKey(key) ? this.GetString(strings, key) : path;
    }

    private void parseStringPath(string path, out string assetName, out string key)
    {
      int length = path.IndexOf(':');
      assetName = length != -1 ? path.Substring(0, length) : throw new ContentLoadException("Unable to parse string path: " + path);
      key = path.Substring(length + 1, path.Length - length - 1);
    }

    public virtual LocalizedContentManager CreateTemporary() => new LocalizedContentManager(this.ServiceProvider, this.RootDirectory, this.CurrentCulture);

    public delegate void LanguageChangedHandler(LocalizedContentManager.LanguageCode code);

    public enum LanguageCode
    {
      en,
      ja,
      ru,
      zh,
      pt,
      es,
      de,
      th,
      fr,
      ko,
      it,
      tr,
      hu,
      mod,
    }
  }
}
