// Decompiled with JetBrains decompiler
// Type: StardewValley.Program
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.SDKs;
using System;
using System.IO;
using System.Text;

namespace StardewValley
{
  public static class Program
  {
    public const int build_steam = 0;
    public const int build_gog = 1;
    public const int build_rail = 2;
    public const int build_gdk = 3;
    public static bool GameTesterMode = false;
    public static bool releaseBuild = true;
    public const int buildType = 0;
    private static SDKHelper _sdk;
    public static Game1 gamePtr;
    public static bool handlingException;
    public static bool hasTriedToPrintLog;
    public static bool successfullyPrintedLog;

    internal static SDKHelper sdk
    {
      get
      {
        if (Program._sdk == null)
          Program._sdk = (SDKHelper) new SteamHelper();
        return Program._sdk;
      }
    }

    /// <summary>The main entry point for the application.</summary>
    public static void Main(string[] args)
    {
      Program.GameTesterMode = true;
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.handleException);
      using (GameRunner gameRunner = new GameRunner())
      {
        GameRunner.instance = gameRunner;
        gameRunner.Run();
      }
    }

    public static string WriteLog(Program.LogType logType, string message, bool append = false)
    {
      string path2_1;
      string path2_2;
      if (logType != Program.LogType.Error && logType == Program.LogType.Disconnect)
      {
        path2_1 = "DisconnectLogs";
        string[] strArray = new string[6]
        {
          Game1.player != null ? Game1.player.Name : "NullPlayer",
          "_",
          null,
          null,
          null,
          null
        };
        DateTime now = DateTime.Now;
        strArray[2] = now.Month.ToString();
        strArray[3] = "-";
        now = DateTime.Now;
        strArray[4] = now.Day.ToString();
        strArray[5] = ".txt";
        path2_2 = string.Concat(strArray);
      }
      else
      {
        path2_1 = "ErrorLogs";
        path2_2 = (Game1.player != null ? Game1.player.Name : "NullPlayer") + "_" + Game1.uniqueIDForThisGame.ToString() + "_" + (Game1.player != null ? (int) Game1.player.millisecondsPlayed : Game1.random.Next(999999)).ToString() + ".txt";
      }
      int folder = Environment.OSVersion.Platform != PlatformID.Unix ? 26 : 28;
      string path = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder) folder), "StardewValley"), path2_1), path2_2);
      FileInfo fileInfo = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder) folder), "StardewValley"), path2_1), "asdfasdf"));
      if (!fileInfo.Directory.Exists)
        fileInfo.Directory.Create();
      if (append)
      {
        if (!File.Exists(path))
          File.CreateText(path);
        try
        {
          File.AppendAllText(path, message + Environment.NewLine);
          return path;
        }
        catch (Exception ex)
        {
          return (string) null;
        }
      }
      else
      {
        if (File.Exists(path))
          File.Delete(path);
        try
        {
          File.WriteAllText(path, message);
          return path;
        }
        catch (Exception ex)
        {
          return (string) null;
        }
      }
    }

    public static void AppendDiagnostics(StringBuilder sb)
    {
      sb.AppendLine("Game Version: " + Game1.GetVersionString());
      try
      {
        if (Program.sdk != null)
          sb.AppendLine("SDK Helper: " + Program.sdk.GetType().Name);
        sb.AppendLine("Game Language: " + LocalizedContentManager.CurrentLanguageCode.ToString());
        try
        {
          sb.AppendLine("GPU: " + Game1.graphics.GraphicsDevice.Adapter.Description);
        }
        catch (Exception ex)
        {
          sb.AppendLine("GPU: Could not detect.");
        }
        sb.AppendLine("OS: " + Environment.OSVersion.Platform.ToString() + " " + Environment.OSVersion.VersionString);
        if (GameRunner.instance != null && GameRunner.instance.GetType().FullName.StartsWith("StardewModdingAPI."))
          sb.AppendLine("Running SMAPI");
        if (Game1.IsMultiplayer)
        {
          if (LocalMultiplayer.IsLocalMultiplayer())
            sb.AppendLine("Multiplayer (Split Screen)");
          else if (Game1.IsMasterGame)
            sb.AppendLine("Multiplayer (Host)");
          else
            sb.AppendLine("Multiplayer (Client)");
        }
        if (Game1.options.gamepadControls)
          sb.AppendLine("Playing on Controller");
        sb.AppendLine("In-game Date: " + Game1.currentSeason + " " + Game1.dayOfMonth.ToString() + " Y" + Game1.year.ToString() + " Time of Day: " + Game1.timeOfDay.ToString());
        sb.AppendLine("Game Location: " + (Game1.currentLocation == null ? "null" : Game1.currentLocation.NameOrUniqueName));
      }
      catch (Exception ex)
      {
      }
    }

    public static void handleException(object sender, UnhandledExceptionEventArgs args)
    {
      if (Program.handlingException || !Program.GameTesterMode)
        return;
      Game1.gameMode = (byte) 11;
      Program.handlingException = true;
      StringBuilder sb = new StringBuilder();
      if (args != null)
      {
        Exception exceptionObject = (Exception) args.ExceptionObject;
        sb.AppendLine("Message: " + exceptionObject.Message);
        sb.AppendLine("InnerException: " + exceptionObject.InnerException?.ToString());
        sb.AppendLine("Stack Trace: " + exceptionObject.StackTrace);
        sb.AppendLine("");
      }
      Program.AppendDiagnostics(sb);
      Game1.errorMessage = sb.ToString();
      long num = DateTime.Now.Ticks / 10000L + 25000L;
      if (!Program.hasTriedToPrintLog)
      {
        Program.hasTriedToPrintLog = true;
        string str = Program.WriteLog(Program.LogType.Error, Game1.errorMessage);
        if (str != null)
        {
          Program.successfullyPrintedLog = true;
          Game1.errorMessage = "(Error Report created at " + str + ")" + Environment.NewLine + Game1.errorMessage;
        }
      }
      if (args == null)
        return;
      Game1.gameMode = (byte) 3;
    }

    public enum LogType
    {
      Error,
      Disconnect,
    }
  }
}
