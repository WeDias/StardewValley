// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChatBox
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StardewValley.Menus
{
  public class ChatBox : IClickableMenu
  {
    public const int chatMessage = 0;
    public const int errorMessage = 1;
    public const int userNotificationMessage = 2;
    public const int privateMessage = 3;
    public const int defaultMaxMessages = 10;
    public const int timeToDisplayMessages = 600;
    public const int chatboxWidth = 896;
    public const int chatboxHeight = 56;
    public const int region_chatBox = 101;
    public const int region_emojiButton = 102;
    public ChatTextBox chatBox;
    public ClickableComponent chatBoxCC;
    private TextBoxEvent e;
    private TextBoxEvent e_backspace;
    private List<ChatMessage> messages = new List<ChatMessage>();
    private KeyboardState oldKBState;
    private List<string> cheatHistory = new List<string>();
    private int cheatHistoryPosition = -1;
    public int maxMessages = 10;
    public static Texture2D emojiTexture;
    public ClickableTextureComponent emojiMenuIcon;
    public EmojiMenu emojiMenu;
    public bool choosingEmoji;
    public bool enableCheats;
    private long lastReceivedPrivateMessagePlayerId;

    public ChatBox()
    {
      this.enableCheats = !Program.releaseBuild;
      Texture2D texture2D = Game1.content.Load<Texture2D>("LooseSprites\\chatBox");
      this.chatBox = new ChatTextBox(texture2D, (Texture2D) null, Game1.smallFont, Color.White);
      this.e = new TextBoxEvent(this.textBoxEnter);
      this.chatBox.OnEnterPressed += this.e;
      this.chatBox.TitleText = "Chat";
      this.chatBoxCC = new ClickableComponent(new Rectangle(this.chatBox.X, this.chatBox.Y, this.chatBox.Width, this.chatBox.Height), "")
      {
        myID = 101
      };
      Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) this.chatBox;
      ChatBox.emojiTexture = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(0, 0, 40, 36), ChatBox.emojiTexture, new Rectangle(0, 0, 9, 9), 4f);
      textureComponent.myID = 102;
      textureComponent.leftNeighborID = 101;
      this.emojiMenuIcon = textureComponent;
      this.emojiMenu = new EmojiMenu(this, ChatBox.emojiTexture, texture2D);
      this.chatBoxCC.rightNeighborID = 102;
      this.updatePosition();
      this.chatBox.Selected = false;
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(101);
      this.snapCursorToCurrentSnappedComponent();
    }

    private void updatePosition()
    {
      this.chatBox.Width = 896;
      this.chatBox.Height = 56;
      this.width = this.chatBox.Width;
      this.height = this.chatBox.Height;
      this.xPositionOnScreen = 0;
      this.yPositionOnScreen = Game1.uiViewport.Height - this.chatBox.Height;
      Utility.makeSafe(ref this.xPositionOnScreen, ref this.yPositionOnScreen, this.chatBox.Width, this.chatBox.Height);
      this.chatBox.X = this.xPositionOnScreen;
      this.chatBox.Y = this.yPositionOnScreen;
      this.chatBoxCC.bounds = new Rectangle(this.chatBox.X, this.chatBox.Y, this.chatBox.Width, this.chatBox.Height);
      this.emojiMenuIcon.bounds.Y = this.chatBox.Y + 8;
      this.emojiMenuIcon.bounds.X = this.chatBox.Width - this.emojiMenuIcon.bounds.Width - 8;
      if (this.emojiMenu == null)
        return;
      this.emojiMenu.xPositionOnScreen = this.emojiMenuIcon.bounds.Center.X - 146;
      this.emojiMenu.yPositionOnScreen = this.emojiMenuIcon.bounds.Y - 248;
    }

    public virtual void textBoxEnter(string text_to_send)
    {
      if (text_to_send.Length < 1)
        return;
      if (text_to_send[0] == '/' && text_to_send.Split(' ')[0].Length > 1)
      {
        this.runCommand(text_to_send.Substring(1));
      }
      else
      {
        text_to_send = Program.sdk.FilterDirtyWords(text_to_send);
        Game1.multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, text_to_send, Multiplayer.AllPlayers);
        this.receiveChatMessage(Game1.player.UniqueMultiplayerID, 0, LocalizedContentManager.CurrentLanguageCode, text_to_send);
      }
    }

    public virtual void textBoxEnter(TextBox sender)
    {
      if (sender is ChatTextBox)
      {
        ChatTextBox chatTextBox = sender as ChatTextBox;
        if (chatTextBox.finalText.Count > 0)
        {
          bool include_color_information = true;
          if (chatTextBox.finalText[0].message != null && chatTextBox.finalText[0].message.Length > 0 && chatTextBox.finalText[0].message.ToString()[0] == '/' && chatTextBox.finalText[0].message.Split(' ')[0].Length > 1)
            include_color_information = false;
          if (chatTextBox.finalText.Count != 1 || (chatTextBox.finalText[0].message != null || chatTextBox.finalText[0].emojiIndex != -1) && (chatTextBox.finalText[0].message == null || chatTextBox.finalText[0].message.Trim().Length != 0))
            this.textBoxEnter(ChatMessage.makeMessagePlaintext(chatTextBox.finalText, include_color_information));
        }
        chatTextBox.reset();
        this.cheatHistoryPosition = -1;
      }
      sender.Text = "";
      this.clickAway();
    }

    public virtual void addInfoMessage(string message) => this.receiveChatMessage(0L, 2, LocalizedContentManager.CurrentLanguageCode, message);

    public virtual void globalInfoMessage(string messageKey, params string[] args)
    {
      if (Game1.IsMultiplayer)
        Game1.multiplayer.globalChatInfoMessage(messageKey, args);
      else
        this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_" + messageKey, (object[]) args));
    }

    public virtual void addErrorMessage(string message) => this.receiveChatMessage(0L, 1, LocalizedContentManager.CurrentLanguageCode, message);

    public virtual void listPlayers(bool otherPlayersOnly = false)
    {
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_UserList"));
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (!otherPlayersOnly || onlineFarmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_UserListUser", (object) ChatBox.formattedUserNameLong(onlineFarmer)));
      }
    }

    public virtual void showHelp()
    {
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_Help"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpClear", (object) "clear"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpList", (object) "list"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpColor", (object) "color"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpColorList", (object) "color-list"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpPause", (object) "pause"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpResume", (object) "resume"));
      if (Game1.IsMultiplayer)
      {
        this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpMessage", (object) "message"));
        this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpReply", (object) "reply"));
      }
      if (!Game1.IsServer)
        return;
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpKick", (object) "kick"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpBan", (object) "ban"));
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpUnban", (object) "unban"));
    }

    protected virtual void runCommand(string command)
    {
      string[] source = command.Split(' ');
      switch (source[0])
      {
        case "ConcernedApe":
        case "ape":
        case "ca":
        case "concernedape":
          if (!Game1.player.mailReceived.Contains("apeChat1"))
          {
            Game1.player.mailReceived.Add("apeChat1");
            this.addMessage(Game1.content.LoadString("Strings\\UI:Chat_ConcernedApe"), new Color(104, 214, (int) byte.MaxValue));
            break;
          }
          this.addMessage(Game1.content.LoadString("Strings\\UI:Chat_ConcernedApe2"), Color.Yellow);
          break;
        case "ban":
          if (!Game1.IsMultiplayer || !Game1.IsServer)
            break;
          this.banPlayer(command);
          break;
        case "cheat":
        case "cheats":
        case "debug":
        case "freegold":
        case "imacheat":
        case "rosebud":
        case "showmethemoney":
          this.addMessage(Game1.content.LoadString("Strings\\UI:Chat_ConcernedApeNiceTry"), new Color(104, 214, (int) byte.MaxValue));
          break;
        case "clear":
          this.messages.Clear();
          break;
        case "color":
          if (source.Length <= 1)
            break;
          Game1.player.defaultChatColor = source[1];
          break;
        case "color-list":
          this.addMessage("white, red, blue, green, jade, yellowgreen, pink, purple, yellow, orange, brown, gray, cream, salmon, peach, aqua, jungle, plum", Color.White);
          break;
        case "dm":
        case "message":
        case "pm":
        case "whisper":
          this.sendPrivateMessage(command);
          break;
        case "e":
        case "emote":
          if (!Game1.player.CanEmote())
            break;
          bool flag = false;
          if (((IEnumerable<string>) source).Count<string>() > 1)
          {
            string str = source[1];
            string emote_type = str.Substring(0, Math.Min(str.Length, 16));
            emote_type.Trim();
            emote_type.ToLower();
            for (int index = 0; index < Farmer.EMOTES.Length; ++index)
            {
              if (emote_type == Farmer.EMOTES[index].emoteString)
              {
                flag = true;
                break;
              }
            }
            if (flag)
              Game1.player.netDoEmote(emote_type);
          }
          if (flag)
            break;
          string message = "";
          for (int index = 0; index < Farmer.EMOTES.Length; ++index)
          {
            if (!Farmer.EMOTES[index].hidden)
            {
              message += Farmer.EMOTES[index].emoteString;
              if (index < Farmer.EMOTES.Length - 1)
                message += ", ";
            }
          }
          this.addMessage(message, Color.White);
          break;
        case "fixweapons":
          Game1.applySaveFix(SaveGame.SaveFixes.ResetForges);
          this.addMessage("Reset forged weapon attributes.", Color.White);
          break;
        case "h":
        case "help":
          this.showHelp();
          break;
        case "kick":
          if (!Game1.IsMultiplayer || !Game1.IsServer)
            break;
          this.kickPlayer(command);
          break;
        case "list":
        case "players":
        case "users":
          this.listPlayers();
          break;
        case "mapscreenshot":
          if (!Game1.game1.CanTakeScreenshots())
            break;
          int result = 25;
          string screenshot_name = (string) null;
          if (((IEnumerable<string>) source).Count<string>() > 2 && !int.TryParse(source[2], out result))
            result = 25;
          if (((IEnumerable<string>) source).Count<string>() > 1)
            screenshot_name = source[1];
          if (result <= 10)
            result = 10;
          string mapScreenshot = Game1.game1.takeMapScreenshot(new float?((float) result / 100f), screenshot_name, (Action) null);
          if (mapScreenshot != null)
          {
            this.addMessage("Wrote '" + mapScreenshot + "'.", Color.White);
            break;
          }
          this.addMessage("Failed.", Color.Red);
          break;
        case "mbp":
        case "movebuildingpermission":
        case "movepermission":
          if (!Game1.IsMasterGame)
            break;
          if (((IEnumerable<string>) source).Count<string>() > 1)
          {
            string str = source[1];
            if (str == "off")
              Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.Off;
            else if (str == "owned")
              Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.OwnedBuildings;
            else if (str == "on")
              Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.On;
            this.addMessage("movebuildingpermission " + Game1.player.team.farmhandsCanMoveBuildings.Value.ToString(), Color.White);
            break;
          }
          this.addMessage("off, owned, on", Color.White);
          break;
        case "money":
          if (this.enableCheats)
          {
            this.cheat(command);
            break;
          }
          this.addMessage(Game1.content.LoadString("Strings\\UI:Chat_ConcernedApeNiceTry"), new Color(104, 214, (int) byte.MaxValue));
          break;
        case "pause":
          if (!Game1.IsMasterGame)
          {
            this.addErrorMessage(Game1.content.LoadString("Strings\\UI:Chat_HostOnlyCommand"));
            break;
          }
          Game1.netWorldState.Value.IsPaused = !Game1.netWorldState.Value.IsPaused;
          if (Game1.netWorldState.Value.IsPaused)
          {
            this.globalInfoMessage("Paused");
            break;
          }
          this.globalInfoMessage("Resumed");
          break;
        case "ping":
          if (!Game1.IsMultiplayer)
            break;
          StringBuilder stringBuilder = new StringBuilder();
          if (Game1.IsServer)
          {
            using (NetRootDictionary<long, Farmer>.Enumerator enumerator = Game1.otherFarmers.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<long, Farmer> current = enumerator.Current;
                stringBuilder.Clear();
                stringBuilder.AppendFormat("Ping({0}) {1}ms ", (object) current.Value.Name, (object) (int) Game1.server.getPingToClient(current.Key));
                this.addMessage(stringBuilder.ToString(), Color.White);
              }
              break;
            }
          }
          else
          {
            stringBuilder.AppendFormat("Ping: {0}ms", (object) (int) Game1.client.GetPingToHost());
            this.addMessage(stringBuilder.ToString(), Color.White);
            break;
          }
        case "printdiag":
          StringBuilder sb = new StringBuilder();
          Program.AppendDiagnostics(sb);
          this.addInfoMessage(sb.ToString());
          Console.WriteLine(sb.ToString());
          break;
        case "qi":
          if (!Game1.player.mailReceived.Contains("QiChat1"))
          {
            Game1.player.mailReceived.Add("QiChat1");
            this.addMessage(Game1.content.LoadString("Strings\\UI:Chat_Qi1"), new Color(100, 50, (int) byte.MaxValue));
            break;
          }
          if (Game1.player.mailReceived.Contains("QiChat2"))
            break;
          Game1.player.mailReceived.Add("QiChat2");
          this.addMessage(Game1.content.LoadString("Strings\\UI:Chat_Qi2"), new Color(100, 50, (int) byte.MaxValue));
          this.addMessage(Game1.content.LoadString("Strings\\UI:Chat_Qi3"), Color.Yellow);
          break;
        case "r":
        case "reply":
          this.replyPrivateMessage(command);
          break;
        case "recountnuts":
          Game1.game1.RecountWalnuts();
          break;
        case "resetisland":
          Game1.game1.ResetIslandLocations();
          break;
        case "resume":
          if (!Game1.IsMasterGame)
          {
            this.addErrorMessage(Game1.content.LoadString("Strings\\UI:Chat_HostOnlyCommand"));
            break;
          }
          if (!Game1.netWorldState.Value.IsPaused)
            break;
          Game1.netWorldState.Value.IsPaused = false;
          this.globalInfoMessage("Resumed");
          break;
        case "sleepannouncemode":
          if (!Game1.IsMasterGame)
            break;
          if (((IEnumerable<string>) source).Count<string>() > 1)
          {
            string str = source[1];
            if (str == "all")
              Game1.player.team.sleepAnnounceMode.Value = FarmerTeam.SleepAnnounceModes.All;
            else if (str == "first")
              Game1.player.team.sleepAnnounceMode.Value = FarmerTeam.SleepAnnounceModes.First;
            else if (str == "off")
              Game1.player.team.sleepAnnounceMode.Value = FarmerTeam.SleepAnnounceModes.Off;
          }
          Game1.multiplayer.globalChatInfoMessage("SleepAnnounceModeSet", Game1.content.LoadString("Strings\\UI:SleepAnnounceMode_" + Game1.player.team.sleepAnnounceMode.Value.ToString()));
          break;
        case "unban":
          if (!Game1.IsServer)
            break;
          this.unbanPlayer(command);
          break;
        case "unbanAll":
        case "unbanall":
          if (!Game1.IsServer)
            break;
          if (Game1.bannedUsers.Count == 0)
          {
            this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_BannedPlayersList_None"));
            break;
          }
          this.unbanAll();
          break;
        default:
          if (!this.enableCheats && !Game1.isRunningMacro)
            break;
          this.cheat(command);
          break;
      }
    }

    public virtual void cheat(string command)
    {
      Game1.debugOutput = (string) null;
      this.addInfoMessage("/" + command);
      if (!Game1.isRunningMacro)
        this.cheatHistory.Insert(0, "/" + command);
      if (Game1.game1.parseDebugInput(command))
      {
        if (Game1.debugOutput == null || !(Game1.debugOutput != ""))
          return;
        this.addInfoMessage(Game1.debugOutput);
      }
      else if (Game1.debugOutput != null && Game1.debugOutput != "")
        this.addErrorMessage(Game1.debugOutput);
      else
        this.addErrorMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:ChatBox.cs.10261") + " " + command.Split(' ')[0]);
    }

    private void replyPrivateMessage(string command)
    {
      if (!Game1.IsMultiplayer)
        return;
      if (this.lastReceivedPrivateMessagePlayerId == 0L)
      {
        this.addErrorMessage(Game1.content.LoadString("Strings\\UI:Chat_NoPlayerToReplyTo"));
      }
      else
      {
        bool flag = !Game1.otherFarmers.ContainsKey(this.lastReceivedPrivateMessagePlayerId);
        if (!flag)
          flag = !Game1.otherFarmers[this.lastReceivedPrivateMessagePlayerId].isActive();
        if (flag)
        {
          this.addErrorMessage(Game1.content.LoadString("Strings\\UI:Chat_CouldNotReply"));
        }
        else
        {
          string[] strArray = command.Split(' ');
          if (strArray.Length <= 1)
            return;
          string words = "";
          for (int index = 1; index < strArray.Length; ++index)
          {
            words += strArray[index];
            if (index < strArray.Length - 1)
              words += " ";
          }
          string message = Program.sdk.FilterDirtyWords(words);
          Game1.multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, message, this.lastReceivedPrivateMessagePlayerId);
          this.receiveChatMessage(Game1.player.UniqueMultiplayerID, 3, LocalizedContentManager.CurrentLanguageCode, message);
        }
      }
    }

    private void kickPlayer(string command)
    {
      int matchingIndex = 0;
      Farmer matchingFarmer = this.findMatchingFarmer(command, ref matchingIndex, true);
      if (matchingFarmer != null)
      {
        Game1.server.kick(matchingFarmer.UniqueMultiplayerID);
      }
      else
      {
        this.addErrorMessage(Game1.content.LoadString("Strings\\UI:Chat_NoPlayerWithThatName"));
        this.listPlayers(true);
      }
    }

    private void banPlayer(string command)
    {
      int matchingIndex = 0;
      Farmer matchingFarmer = this.findMatchingFarmer(command, ref matchingIndex, true);
      if (matchingFarmer != null)
      {
        string key = Game1.server.ban(matchingFarmer.UniqueMultiplayerID);
        if (key == null || !Game1.bannedUsers.ContainsKey(key))
        {
          this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_BannedPlayerFailed"));
        }
        else
        {
          string bannedUser = Game1.bannedUsers[key];
          string sub1 = bannedUser != null ? bannedUser + " (" + key + ")" : key;
          this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_BannedPlayer", (object) sub1));
        }
      }
      else
      {
        this.addErrorMessage(Game1.content.LoadString("Strings\\UI:Chat_NoPlayerWithThatName"));
        this.listPlayers(true);
      }
    }

    private void unbanAll()
    {
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_UnbannedAllPlayers"));
      Game1.bannedUsers.Clear();
    }

    private void unbanPlayer(string command)
    {
      if (Game1.bannedUsers.Count == 0)
      {
        this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_BannedPlayersList_None"));
      }
      else
      {
        bool flag = false;
        string[] strArray = command.Split(' ');
        if (strArray.Length > 1)
        {
          string key1 = strArray[1];
          string key2 = (string) null;
          if (Game1.bannedUsers.ContainsKey(key1))
          {
            key2 = key1;
          }
          else
          {
            foreach (KeyValuePair<string, string> bannedUser in (Dictionary<string, string>) Game1.bannedUsers)
            {
              if (bannedUser.Value == key1)
              {
                key2 = bannedUser.Key;
                break;
              }
            }
          }
          if (key2 != null)
          {
            string sub1 = Game1.bannedUsers[key2] == null ? key2 : Game1.bannedUsers[key2] + " (" + key2 + ")";
            this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_UnbannedPlayer", (object) sub1));
            Game1.bannedUsers.Remove(key2);
          }
          else
          {
            flag = true;
            this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_UnbanPlayer_NotFound"));
          }
        }
        else
          flag = true;
        if (!flag)
          return;
        this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_BannedPlayersList"));
        foreach (KeyValuePair<string, string> bannedUser in (Dictionary<string, string>) Game1.bannedUsers)
        {
          string message = "- " + bannedUser.Key;
          if (bannedUser.Value != null)
            message = "- " + bannedUser.Value + " (" + bannedUser.Key + ")";
          this.addInfoMessage(message);
        }
      }
    }

    private Farmer findMatchingFarmer(
      string command,
      ref int matchingIndex,
      bool allowMatchingByUserName = false)
    {
      string[] strArray1 = command.Split(' ');
      Farmer matchingFarmer = (Farmer) null;
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      {
        string[] strArray2 = farmer.displayName.Split(' ');
        bool flag1 = true;
        int index1;
        for (index1 = 0; index1 < strArray2.Length; ++index1)
        {
          if (strArray1.Length > index1 + 1)
          {
            if (strArray1[index1 + 1].ToLowerInvariant() != strArray2[index1].ToLowerInvariant())
            {
              flag1 = false;
              break;
            }
          }
          else
          {
            flag1 = false;
            break;
          }
        }
        if (flag1)
        {
          matchingFarmer = farmer;
          matchingIndex = index1;
          break;
        }
        if (allowMatchingByUserName)
        {
          bool flag2 = true;
          string[] strArray3 = Game1.multiplayer.getUserName(farmer.UniqueMultiplayerID).Split(' ');
          int index2;
          for (index2 = 0; index2 < strArray3.Length; ++index2)
          {
            if (strArray1.Length > index2 + 1)
            {
              if (strArray1[index2 + 1].ToLowerInvariant() != strArray3[index2].ToLowerInvariant())
              {
                flag2 = false;
                break;
              }
            }
            else
            {
              flag2 = false;
              break;
            }
          }
          if (flag2)
          {
            matchingFarmer = farmer;
            matchingIndex = index2;
            break;
          }
        }
      }
      return matchingFarmer;
    }

    private void sendPrivateMessage(string command)
    {
      if (!Game1.IsMultiplayer)
        return;
      string[] strArray = command.Split(' ');
      int matchingIndex = 0;
      Farmer matchingFarmer = this.findMatchingFarmer(command, ref matchingIndex);
      if (matchingFarmer == null)
      {
        this.addErrorMessage(Game1.content.LoadString("Strings\\UI:Chat_NoPlayerWithThatName"));
      }
      else
      {
        string words = "";
        for (int index = matchingIndex + 1; index < strArray.Length; ++index)
        {
          words += strArray[index];
          if (index < strArray.Length - 1)
            words += " ";
        }
        string message = Program.sdk.FilterDirtyWords(words);
        Game1.multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, message, matchingFarmer.UniqueMultiplayerID);
        this.receiveChatMessage(Game1.player.UniqueMultiplayerID, 3, LocalizedContentManager.CurrentLanguageCode, message);
      }
    }

    public bool isActive() => this.chatBox.Selected;

    public void activate()
    {
      this.chatBox.Selected = true;
      this.setText("");
    }

    public override void clickAway()
    {
      base.clickAway();
      if (this.choosingEmoji && this.emojiMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) && !Game1.input.GetKeyboardState().IsKeyDown(Keys.Escape))
        return;
      int num = this.chatBox.Selected ? 1 : 0;
      this.chatBox.Selected = false;
      this.choosingEmoji = false;
      this.setText("");
      this.cheatHistoryPosition = -1;
      if (num == 0)
        return;
      Game1.oldKBState = Game1.GetKeyboardState();
    }

    public override bool isWithinBounds(int x, int y)
    {
      if (x - this.xPositionOnScreen < this.width && x - this.xPositionOnScreen >= 0 && y - this.yPositionOnScreen < this.height && y - this.yPositionOnScreen >= -this.getOldMessagesBoxHeight())
        return true;
      return this.choosingEmoji && this.emojiMenu.isWithinBounds(x, y);
    }

    public virtual void setText(string text) => this.chatBox.setText(text);

    public override void receiveKeyPress(Keys key)
    {
      switch (key)
      {
        case Keys.Up:
          if (this.cheatHistoryPosition < this.cheatHistory.Count - 1)
          {
            ++this.cheatHistoryPosition;
            this.chatBox.setText(this.cheatHistory[this.cheatHistoryPosition]);
            break;
          }
          break;
        case Keys.Down:
          if (this.cheatHistoryPosition > 0)
          {
            --this.cheatHistoryPosition;
            this.chatBox.setText(this.cheatHistory[this.cheatHistoryPosition]);
            break;
          }
          break;
      }
      if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key) || Game1.options.doesInputListContain(Game1.options.moveRightButton, key) || Game1.options.doesInputListContain(Game1.options.moveDownButton, key) || Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
        return;
      base.receiveKeyPress(key);
    }

    public override bool readyToClose() => false;

    public override void receiveGamePadButton(Buttons b)
    {
    }

    public bool isHoveringOverClickable(int x, int y) => this.emojiMenuIcon.containsPoint(x, y) || this.choosingEmoji && this.emojiMenu.isWithinBounds(x, y);

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!this.chatBox.Selected)
        return;
      if (this.emojiMenuIcon.containsPoint(x, y))
      {
        this.choosingEmoji = !this.choosingEmoji;
        Game1.playSound("shwip");
        this.emojiMenuIcon.scale = 4f;
      }
      else if (this.choosingEmoji && this.emojiMenu.isWithinBounds(x, y))
      {
        this.emojiMenu.leftClick(x, y, this);
      }
      else
      {
        this.chatBox.Update();
        if (this.choosingEmoji)
        {
          this.choosingEmoji = false;
          this.emojiMenuIcon.scale = 4f;
        }
        if (!this.isWithinBounds(x, y))
          return;
        this.chatBox.Selected = true;
      }
    }

    public static string formattedUserName(Farmer farmer)
    {
      string str = farmer.Name;
      if (str == null || str.Trim() == "")
        str = Game1.content.LoadString("Strings\\UI:Chat_PlayerJoinedNewName");
      return str;
    }

    public static string formattedUserNameLong(Farmer farmer)
    {
      string sub1 = ChatBox.formattedUserName(farmer);
      return Game1.content.LoadString("Strings\\UI:Chat_PlayerName", (object) sub1, (object) Game1.multiplayer.getUserName(farmer.UniqueMultiplayerID));
    }

    private string formatMessage(long sourceFarmer, int chatKind, string message)
    {
      string sub1 = Game1.content.LoadString("Strings\\UI:Chat_UnknownUserName");
      Farmer farmer = (Farmer) null;
      if (sourceFarmer == Game1.player.UniqueMultiplayerID)
        farmer = Game1.player;
      if (Game1.otherFarmers.ContainsKey(sourceFarmer))
        farmer = Game1.otherFarmers[sourceFarmer];
      if (farmer != null)
        sub1 = ChatBox.formattedUserName(farmer);
      switch (chatKind)
      {
        case 0:
          return Game1.content.LoadString("Strings\\UI:Chat_ChatMessageFormat", (object) sub1, (object) message);
        case 2:
          return Game1.content.LoadString("Strings\\UI:Chat_UserNotificationMessageFormat", (object) message);
        case 3:
          return Game1.content.LoadString("Strings\\UI:Chat_PrivateMessageFormat", (object) sub1, (object) message);
        default:
          return Game1.content.LoadString("Strings\\UI:Chat_ErrorMessageFormat", (object) message);
      }
    }

    protected virtual Color messageColor(int chatKind)
    {
      switch (chatKind)
      {
        case 0:
          return this.chatBox.TextColor;
        case 2:
          return Color.Yellow;
        case 3:
          return Color.DarkCyan;
        default:
          return Color.Red;
      }
    }

    public virtual void receiveChatMessage(
      long sourceFarmer,
      int chatKind,
      LocalizedContentManager.LanguageCode language,
      string message)
    {
      string text1 = this.formatMessage(sourceFarmer, chatKind, message);
      ChatMessage chatMessage = new ChatMessage();
      SpriteFont font = this.chatBox.Font;
      int width = this.chatBox.Width - 16;
      string text2 = Game1.parseText(text1, font, width);
      chatMessage.timeLeftToDisplay = 600;
      chatMessage.verticalSize = (int) this.chatBox.Font.MeasureString(text2).Y + 4;
      chatMessage.color = this.messageColor(chatKind);
      chatMessage.language = language;
      chatMessage.parseMessageForEmoji(text2);
      this.messages.Add(chatMessage);
      if (this.messages.Count > this.maxMessages)
        this.messages.RemoveAt(0);
      if (chatKind != 3 || sourceFarmer == Game1.player.UniqueMultiplayerID)
        return;
      this.lastReceivedPrivateMessagePlayerId = sourceFarmer;
    }

    public virtual void addMessage(string message, Color color)
    {
      ChatMessage chatMessage = new ChatMessage();
      string text = Game1.parseText(message, this.chatBox.Font, this.chatBox.Width - 8);
      chatMessage.timeLeftToDisplay = 600;
      chatMessage.verticalSize = (int) this.chatBox.Font.MeasureString(text).Y + 4;
      chatMessage.color = color;
      chatMessage.language = LocalizedContentManager.CurrentLanguageCode;
      chatMessage.parseMessageForEmoji(text);
      this.messages.Add(chatMessage);
      if (this.messages.Count <= this.maxMessages)
        return;
      this.messages.RemoveAt(0);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.emojiMenuIcon.tryHover(x, y, 1f);
      this.emojiMenuIcon.tryHover(x, y, 1f);
    }

    public override void update(GameTime time)
    {
      KeyboardState keyboardState = Game1.input.GetKeyboardState();
      foreach (Keys pressedKey in keyboardState.GetPressedKeys())
      {
        if (!this.oldKBState.IsKeyDown(pressedKey))
          this.receiveKeyPress(pressedKey);
      }
      this.oldKBState = keyboardState;
      for (int index = 0; index < this.messages.Count; ++index)
      {
        if (this.messages[index].timeLeftToDisplay > 0)
          --this.messages[index].timeLeftToDisplay;
        if (this.messages[index].timeLeftToDisplay < 75)
          this.messages[index].alpha = (float) this.messages[index].timeLeftToDisplay / 75f;
      }
      if (this.chatBox.Selected)
      {
        foreach (ChatMessage message in this.messages)
          message.alpha = 1f;
      }
      this.emojiMenuIcon.tryHover(0, 0, 1f);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      if (!this.choosingEmoji)
        return;
      this.emojiMenu.receiveScrollWheelAction(direction);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => this.updatePosition();

    public static SpriteFont messageFont(LocalizedContentManager.LanguageCode language) => Game1.content.Load<SpriteFont>("Fonts\\SmallFont", language);

    public int getOldMessagesBoxHeight()
    {
      int messagesBoxHeight = 20;
      for (int index = this.messages.Count - 1; index >= 0; --index)
      {
        ChatMessage message = this.messages[index];
        if (this.chatBox.Selected || (double) message.alpha > 0.00999999977648258)
          messagesBoxHeight += message.verticalSize;
      }
      return messagesBoxHeight;
    }

    public override void draw(SpriteBatch b)
    {
      int num1 = 0;
      bool flag = false;
      for (int index = this.messages.Count - 1; index >= 0; --index)
      {
        ChatMessage message = this.messages[index];
        if (this.chatBox.Selected || (double) message.alpha > 0.00999999977648258)
        {
          num1 += message.verticalSize;
          flag = true;
        }
      }
      if (flag)
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(301, 288, 15, 15), this.xPositionOnScreen, this.yPositionOnScreen - num1 - 20 + (this.chatBox.Selected ? 0 : this.chatBox.Height), this.chatBox.Width, num1 + 20, Color.White, 4f, false);
      int num2 = 0;
      for (int index = this.messages.Count - 1; index >= 0; --index)
      {
        ChatMessage message = this.messages[index];
        num2 += message.verticalSize;
        message.draw(b, this.xPositionOnScreen + 12, this.yPositionOnScreen - num2 - 8 + (this.chatBox.Selected ? 0 : this.chatBox.Height));
      }
      if (!this.chatBox.Selected)
        return;
      this.chatBox.Draw(b, false);
      this.emojiMenuIcon.draw(b, Color.White, 0.99f);
      if (this.choosingEmoji)
        this.emojiMenu.draw(b);
      if (!this.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) || Game1.options.hardwareCursor)
        return;
      Game1.mouseCursor = Game1.options.gamepadControls ? 44 : 0;
    }
  }
}
