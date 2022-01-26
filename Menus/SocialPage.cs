// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SocialPage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class SocialPage : IClickableMenu
  {
    public const int slotsOnPage = 5;
    public static readonly string[] defaultFriendships = new string[2]
    {
      "Robin",
      "Lewis"
    };
    private string descriptionText = "";
    private string hoverText = "";
    private ClickableTextureComponent upButton;
    private ClickableTextureComponent downButton;
    private ClickableTextureComponent scrollBar;
    private Rectangle scrollBarRunner;
    public List<object> names;
    private List<ClickableTextureComponent> sprites;
    private int slotPosition;
    private int numFarmers;
    private List<string> kidsNames = new List<string>();
    private Dictionary<string, string> npcNames;
    public List<ClickableTextureComponent> characterSlots;
    private bool scrolling;
    public Friendship emptyFriendship = new Friendship();

    public SocialPage(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
      foreach (string defaultFriendship in SocialPage.defaultFriendships)
      {
        if (!Game1.player.friendshipData.ContainsKey(defaultFriendship))
          Game1.player.friendshipData.Add(defaultFriendship, new Friendship());
      }
      this.characterSlots = new List<ClickableTextureComponent>();
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      this.npcNames = new Dictionary<string, string>();
      foreach (string key in Game1.player.friendshipData.Keys)
      {
        string str = key;
        if (dictionary.ContainsKey(key) && dictionary[key].Split('/').Length > 11)
          str = dictionary[key].Split('/')[11];
        if (!this.npcNames.ContainsKey(key))
          this.npcNames.Add(key, str);
      }
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (allCharacter.CanSocialize)
        {
          if (Game1.player.friendshipData.ContainsKey(allCharacter.Name) && allCharacter is Child)
          {
            this.kidsNames.Add(allCharacter.Name);
            this.npcNames[allCharacter.Name] = allCharacter.Name.Trim();
          }
          else if (Game1.player.friendshipData.ContainsKey(allCharacter.Name))
            this.npcNames[allCharacter.Name] = allCharacter.displayName;
          else
            this.npcNames[allCharacter.Name] = "???";
        }
      }
      this.names = new List<object>();
      this.sprites = new List<ClickableTextureComponent>();
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (((bool) (NetFieldBase<bool, NetBool>) allFarmer.isCustomized || allFarmer == Game1.MasterPlayer) && allFarmer != Game1.player)
        {
          this.names.Add((object) allFarmer.UniqueMultiplayerID);
          this.sprites.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.borderWidth + 4, 0, width, 64), (string) null, "", allFarmer.Sprite.Texture, Rectangle.Empty, 4f));
          ++this.numFarmers;
        }
      }
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) this.npcNames.OrderBy<KeyValuePair<string, string>, int>((Func<KeyValuePair<string, string>, int>) (p => -Game1.player.getFriendshipLevelForNPC(p.Key))))
      {
        NPC npc = (NPC) null;
        if (this.kidsNames.Contains(keyValuePair.Key))
          npc = (NPC) Game1.getCharacterFromName<Child>(keyValuePair.Key, false);
        else if (dictionary.ContainsKey(keyValuePair.Key))
        {
          string[] strArray = dictionary[keyValuePair.Key].Split('/')[10].Split(' ');
          string nameForCharacter = NPC.getTextureNameForCharacter(keyValuePair.Key);
          if (strArray.Length > 2)
            npc = new NPC(new AnimatedSprite("Characters\\" + nameForCharacter, 0, 16, 32), new Vector2((float) (Convert.ToInt32(strArray[1]) * 64), (float) (Convert.ToInt32(strArray[2]) * 64)), strArray[0], 0, keyValuePair.Key, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\" + nameForCharacter), false);
        }
        if (npc != null)
        {
          this.names.Add((object) npc.Name);
          this.sprites.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.borderWidth + 4, 0, width, 64), (string) null, "", npc.Sprite.Texture, npc.getMugShotSourceRect(), 4f));
        }
      }
      for (int index = 0; index < this.names.Count; ++index)
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth, 0, width - IClickableMenu.borderWidth * 2, this.rowPosition(1) - this.rowPosition(0)), (Texture2D) null, new Rectangle(0, 0, 0, 0), 4f);
        textureComponent.myID = index;
        textureComponent.downNeighborID = index + 1;
        textureComponent.upNeighborID = index - 1;
        if (textureComponent.upNeighborID < 0)
          textureComponent.upNeighborID = 12342;
        this.characterSlots.Add(textureComponent);
      }
      this.upButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width + 16, this.yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
      this.downButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width + 16, this.yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
      this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upButton.bounds.X + 12, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, this.scrollBar.bounds.Width, height - 128 - this.upButton.bounds.Height - 8);
      int num = 0;
      for (int index = 0; index < this.names.Count; ++index)
      {
        if (!(this.names[index] is long))
        {
          num = index;
          break;
        }
      }
      this.slotPosition = num;
      this.setScrollBarToCurrentIndex();
      this.updateSlots();
    }

    public static bool isRoommateOfAnyone(string name)
    {
      NPC characterFromName = Game1.getCharacterFromName(name);
      return characterFromName != null && characterFromName.isRoommate();
    }

    public static bool isDatable(string name)
    {
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      return dictionary.ContainsKey(name) && dictionary[name].Split('/')[5] == "datable";
    }

    public Friendship getFriendship(string name) => Game1.player.friendshipData.ContainsKey(name) ? Game1.player.friendshipData[name] : this.emptyFriendship;

    public override void snapToDefaultClickableComponent()
    {
      if (this.slotPosition < this.characterSlots.Count)
        this.currentlySnappedComponent = (ClickableComponent) this.characterSlots[this.slotPosition];
      this.snapCursorToCurrentSnappedComponent();
    }

    public int getGender(string name)
    {
      if (this.kidsNames.Contains(name))
        return Game1.getCharacterFromName<Child>(name, false).Gender;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      return !dictionary.ContainsKey(name) || !(dictionary[name].Split('/')[4] == "female") ? 0 : 1;
    }

    public bool isMarriedToAnyone(string name)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.spouse == name && allFarmer.isMarried())
          return true;
      }
      return false;
    }

    public void updateSlots()
    {
      for (int index = 0; index < this.characterSlots.Count; ++index)
        this.characterSlots[index].bounds.Y = this.rowPosition(index - 1);
      int num1 = 0;
      for (int slotPosition = this.slotPosition; slotPosition < this.slotPosition + 5; ++slotPosition)
      {
        if (this.sprites.Count > slotPosition)
        {
          int num2 = this.yPositionOnScreen + IClickableMenu.borderWidth + 32 + 112 * num1 + 32;
          this.sprites[slotPosition].bounds.Y = num2;
        }
        ++num1;
      }
      this.populateClickableComponentList();
      this.addTabsToClickableComponents();
    }

    public void addTabsToClickableComponents()
    {
      if (!(Game1.activeClickableMenu is GameMenu) || this.allClickableComponents.Contains((Game1.activeClickableMenu as GameMenu).tabs[0]))
        return;
      this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) (Game1.activeClickableMenu as GameMenu).tabs);
    }

    protected void _SelectSlot(ClickableComponent slot_component)
    {
      if (slot_component == null || !((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(slot_component))
        return;
      int num = this.characterSlots.IndexOf(slot_component as ClickableTextureComponent);
      this.currentlySnappedComponent = slot_component;
      if (num < this.slotPosition)
        this.slotPosition = num;
      else if (num >= this.slotPosition + 5)
        this.slotPosition = num - 5 + 1;
      this.setScrollBarToCurrentIndex();
      this.updateSlots();
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      this.snapCursorToCurrentSnappedComponent();
    }

    public void ConstrainSelectionToVisibleSlots()
    {
      if (!((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
        return;
      int index = this.characterSlots.IndexOf(this.currentlySnappedComponent as ClickableTextureComponent);
      if (index < this.slotPosition)
        index = this.slotPosition;
      else if (index >= this.slotPosition + 5)
        index = this.slotPosition + 5 - 1;
      this.currentlySnappedComponent = (ClickableComponent) this.characterSlots[index];
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void snapCursorToCurrentSnappedComponent()
    {
      if (this.currentlySnappedComponent != null && ((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
        Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 64, this.currentlySnappedComponent.bounds.Center.Y);
      else
        base.snapCursorToCurrentSnappedComponent();
    }

    public override void applyMovementKey(int direction)
    {
      base.applyMovementKey(direction);
      if (!((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
        return;
      this._SelectSlot(this.currentlySnappedComponent);
    }

    public override void leftClickHeld(int x, int y)
    {
      base.leftClickHeld(x, y);
      if (!this.scrolling)
        return;
      int y1 = this.scrollBar.bounds.Y;
      this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upButton.bounds.Height + 20));
      this.slotPosition = Math.Min(this.sprites.Count - 5, Math.Max(0, (int) ((double) this.sprites.Count * (double) ((float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height))));
      this.setScrollBarToCurrentIndex();
      int y2 = this.scrollBar.bounds.Y;
      if (y1 == y2)
        return;
      Game1.playSound("shiny4");
    }

    public override void releaseLeftClick(int x, int y)
    {
      base.releaseLeftClick(x, y);
      this.scrolling = false;
    }

    private void setScrollBarToCurrentIndex()
    {
      if (this.sprites.Count > 0)
      {
        this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.sprites.Count - 5 + 1) * this.slotPosition + this.upButton.bounds.Bottom + 4;
        if (this.slotPosition == this.sprites.Count - 5)
          this.scrollBar.bounds.Y = this.downButton.bounds.Y - this.scrollBar.bounds.Height - 4;
      }
      this.updateSlots();
    }

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && this.slotPosition > 0)
      {
        this.upArrowPressed();
        this.ConstrainSelectionToVisibleSlots();
        Game1.playSound("shiny4");
      }
      else
      {
        if (direction >= 0 || this.slotPosition >= Math.Max(0, this.sprites.Count - 5))
          return;
        this.downArrowPressed();
        this.ConstrainSelectionToVisibleSlots();
        Game1.playSound("shiny4");
      }
    }

    public void upArrowPressed()
    {
      --this.slotPosition;
      this.updateSlots();
      this.upButton.scale = 3.5f;
      this.setScrollBarToCurrentIndex();
    }

    public void downArrowPressed()
    {
      ++this.slotPosition;
      this.updateSlots();
      this.downButton.scale = 3.5f;
      this.setScrollBarToCurrentIndex();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.upButton.containsPoint(x, y) && this.slotPosition > 0)
      {
        this.upArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.downButton.containsPoint(x, y) && this.slotPosition < this.sprites.Count - 5)
      {
        this.downArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.scrollBar.containsPoint(x, y))
        this.scrolling = true;
      else if (!this.downButton.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
      {
        this.scrolling = true;
        this.leftClickHeld(x, y);
        this.releaseLeftClick(x, y);
      }
      else
      {
        for (int index1 = 0; index1 < this.characterSlots.Count; ++index1)
        {
          if (index1 >= this.slotPosition && index1 < this.slotPosition + 5 && this.characterSlots[index1].bounds.Contains(x, y))
          {
            bool flag = true;
            if (this.names[index1] is string)
            {
              Character characterFromName = (Character) Game1.getCharacterFromName((string) this.names[index1]);
              if (characterFromName != null && Game1.player.friendshipData.ContainsKey((string) (NetFieldBase<string, NetString>) characterFromName.name))
              {
                Game1.playSound("bigSelect");
                int cached_slot_position = this.slotPosition;
                ProfileMenu menu = new ProfileMenu(characterFromName);
                menu.exitFunction = (IClickableMenu.onExit) (() =>
                {
                  if (!(((GameMenu) (Game1.activeClickableMenu = (IClickableMenu) new GameMenu(2, playOpeningSound: false))).GetCurrentPage() is SocialPage currentPage2))
                    return;
                  Character character = menu.GetCharacter();
                  if (character == null)
                    return;
                  for (int index2 = 0; index2 < currentPage2.names.Count; ++index2)
                  {
                    if (currentPage2.names[index2] is string && character.Name == (string) currentPage2.names[index2])
                    {
                      currentPage2.slotPosition = cached_slot_position;
                      currentPage2._SelectSlot((ClickableComponent) currentPage2.characterSlots[index2]);
                      break;
                    }
                  }
                });
                Game1.activeClickableMenu = (IClickableMenu) menu;
                if (!Game1.options.SnappyMenus)
                  return;
                menu.snapToDefaultClickableComponent();
                return;
              }
            }
            if (flag)
            {
              Game1.playSound("shiny4");
              break;
            }
            break;
          }
        }
        this.slotPosition = Math.Max(0, Math.Min(this.sprites.Count - 5, this.slotPosition));
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.descriptionText = "";
      this.hoverText = "";
      this.upButton.tryHover(x, y);
      this.downButton.tryHover(x, y);
    }

    private bool isCharacterSlotClickable(int i)
    {
      if (this.names[i] is string)
      {
        Character characterFromName = (Character) Game1.getCharacterFromName((string) this.names[i]);
        if (characterFromName != null && Game1.player.friendshipData.ContainsKey((string) (NetFieldBase<string, NetString>) characterFromName.name))
          return true;
      }
      return false;
    }

    private void drawNPCSlot(SpriteBatch b, int i)
    {
      if ((!this.isCharacterSlotClickable(i) ? 0 : (this.characterSlots[i].bounds.Contains(Game1.getMouseX(), Game1.getMouseY()) ? 1 : 0)) != 0)
        b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth - 4, this.sprites[i].bounds.Y - 4, this.characterSlots[i].bounds.Width, this.characterSlots[i].bounds.Height - 12), Color.White * 0.25f);
      this.sprites[i].draw(b);
      string name = this.names[i] as string;
      int heartLevelForNpc = Game1.player.getFriendshipHeartLevelForNPC(name);
      bool flag1 = SocialPage.isDatable(name);
      Friendship friendship = this.getFriendship(name);
      bool flag2 = friendship.IsMarried();
      bool flag3 = flag2 && SocialPage.isRoommateOfAnyone(name);
      float y = Game1.smallFont.MeasureString("W").Y;
      float num = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? (float) (-(double) y / 2.0) : 0.0f;
      b.DrawString(Game1.dialogueFont, this.npcNames[name], new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 64 - 20 + 96) - Game1.dialogueFont.MeasureString(this.npcNames[name]).X / 2f, (float) ((double) (this.sprites[i].bounds.Y + 48) + (double) num - (flag1 ? 24.0 : 20.0))), Game1.textColor);
      for (int index = 0; index < Math.Max(Utility.GetMaximumHeartsForCharacter((Character) Game1.getCharacterFromName(name)), 10); ++index)
      {
        int x = index < heartLevelForNpc ? 211 : 218;
        if (flag1 && !friendship.IsDating() && !flag2 && index >= 8)
          x = 211;
        if (index < 10)
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 320 - 4 + index * 32), (float) (this.sprites[i].bounds.Y + 64 - 28)), new Rectangle?(new Rectangle(x, 428, 7, 6)), !flag1 || friendship.IsDating() || flag2 || index < 8 ? Color.White : Color.Black * 0.35f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
        else
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 320 - 4 + (index - 10) * 32), (float) (this.sprites[i].bounds.Y + 64)), new Rectangle?(new Rectangle(x, 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      }
      if (flag1 | flag3)
      {
        string text1 = !Game1.content.ShouldUseGenderedCharacterTranslations() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : (this.getGender(name) == 0 ? ((IEnumerable<string>) Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')).First<string>() : ((IEnumerable<string>) Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')).Last<string>());
        if (flag3)
          text1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Housemate");
        else if (flag2)
          text1 = this.getGender(name) == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637");
        else if (this.isMarriedToAnyone(name))
          text1 = this.getGender(name) == 0 ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC");
        else if (!Game1.player.isMarried() && friendship.IsDating())
          text1 = this.getGender(name) == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640");
        else if (this.getFriendship(name).IsDivorced())
          text1 = this.getGender(name) == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643");
        int width = (IClickableMenu.borderWidth * 3 + 128 - 40 + 192) / 2;
        string text2 = Game1.parseText(text1, Game1.smallFont, width);
        Vector2 vector2 = Game1.smallFont.MeasureString(text2);
        b.DrawString(Game1.smallFont, text2, new Vector2((float) (this.xPositionOnScreen + 192 + 8) - vector2.X / 2f, (float) this.sprites[i].bounds.Bottom - (vector2.Y - y)), Game1.textColor);
      }
      if (!this.getFriendship(name).IsMarried() && !this.kidsNames.Contains(name))
      {
        Utility.drawWithShadow(b, Game1.mouseCursors2, new Vector2((float) (this.xPositionOnScreen + 384 + 304), (float) (this.sprites[i].bounds.Y - 4)), new Rectangle(166, 174, 14, 12), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f, horizontalShadowOffset: 0, shadowIntensity: 0.2f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 384 + 296), (float) (this.sprites[i].bounds.Y + 32 + 20)), new Rectangle?(new Rectangle(227 + (this.getFriendship(name).GiftsThisWeek >= 2 ? 9 : 0), 425, 9, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 384 + 336), (float) (this.sprites[i].bounds.Y + 32 + 20)), new Rectangle?(new Rectangle(227 + (this.getFriendship(name).GiftsThisWeek >= 1 ? 9 : 0), 425, 9, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
        Utility.drawWithShadow(b, Game1.mouseCursors2, new Vector2((float) (this.xPositionOnScreen + 384 + 424), (float) this.sprites[i].bounds.Y), new Rectangle(180, 175, 13, 11), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f, horizontalShadowOffset: 0, shadowIntensity: 0.2f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 384 + 432), (float) (this.sprites[i].bounds.Y + 32 + 20)), new Rectangle?(new Rectangle(227 + (this.getFriendship(name).TalkedToToday ? 9 : 0), 425, 9, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      }
      if (flag2)
      {
        if (flag3 && !(name == "Krobus"))
          return;
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, flag3 ? 808 : 460, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
      }
      else
      {
        if (!friendship.IsDating())
          return;
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, flag3 ? 808 : 458, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
      }
    }

    private int rowPosition(int i)
    {
      int num1 = i - this.slotPosition;
      int num2 = 112;
      return this.yPositionOnScreen + IClickableMenu.borderWidth + 160 + 4 + num1 * num2;
    }

    private void drawFarmerSlot(SpriteBatch b, int i)
    {
      long name = (long) this.names[i];
      Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(name);
      if (farmerMaybeOffline == null)
        return;
      int num1 = farmerMaybeOffline.IsMale ? 0 : 1;
      ClickableTextureComponent sprite = this.sprites[i];
      int x = sprite.bounds.X;
      int y1 = sprite.bounds.Y;
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      Rectangle rectangle = scissorRectangle;
      rectangle.Height = Math.Min(rectangle.Bottom, this.rowPosition(i)) - rectangle.Y - 4;
      b.GraphicsDevice.ScissorRectangle = rectangle;
      FarmerRenderer.isDrawingForUI = true;
      try
      {
        farmerMaybeOffline.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame((bool) (NetFieldBase<bool, NetBool>) farmerMaybeOffline.bathingClothes ? 108 : 0, 0, false, false), (bool) (NetFieldBase<bool, NetBool>) farmerMaybeOffline.bathingClothes ? 108 : 0, new Rectangle(0, (bool) (NetFieldBase<bool, NetBool>) farmerMaybeOffline.bathingClothes ? 576 : 0, 16, 32), new Vector2((float) x, (float) y1), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, farmerMaybeOffline);
      }
      finally
      {
        b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      }
      FarmerRenderer.isDrawingForUI = false;
      Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, name);
      int num2 = friendship.IsMarried() ? 1 : 0;
      float y2 = Game1.smallFont.MeasureString("W").Y;
      float num3 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru ? (float) (-(double) y2 / 2.0) : 0.0f;
      b.DrawString(Game1.dialogueFont, farmerMaybeOffline.Name, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 96 - 20), (float) ((double) (this.sprites[i].bounds.Y + 48) + (double) num3 - 24.0)), Game1.textColor);
      string text1 = !Game1.content.ShouldUseGenderedCharacterTranslations() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : (num1 == 0 ? ((IEnumerable<string>) Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')).First<string>() : ((IEnumerable<string>) Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')).Last<string>());
      if (num2 != 0)
        text1 = num1 == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637");
      else if (farmerMaybeOffline.isMarried() && !farmerMaybeOffline.hasRoommate())
        text1 = num1 == 0 ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC");
      else if (!Game1.player.isMarried() && friendship.IsDating())
        text1 = num1 == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640");
      else if (friendship.IsDivorced())
        text1 = num1 == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643");
      int width = (IClickableMenu.borderWidth * 3 + 128 - 40 + 192) / 2;
      string text2 = Game1.parseText(text1, Game1.smallFont, width);
      Vector2 vector2 = Game1.smallFont.MeasureString(text2);
      b.DrawString(Game1.smallFont, text2, new Vector2((float) (this.xPositionOnScreen + 192 + 8) - vector2.X / 2f, (float) this.sprites[i].bounds.Bottom - (vector2.Y - y2)), Game1.textColor);
      if (num2 != 0)
      {
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 801, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
      }
      else
      {
        if (!friendship.IsDating())
          return;
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 458, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
      }
    }

    public override void draw(SpriteBatch b)
    {
      b.End();
      b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 128 + 4, true);
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 192 + 32 + 20, true);
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 320 + 36, true);
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 384 + 32 + 52, true);
      for (int slotPosition = this.slotPosition; slotPosition < this.slotPosition + 5; ++slotPosition)
      {
        if (slotPosition < this.sprites.Count)
        {
          if (this.names[slotPosition] is string)
            this.drawNPCSlot(b, slotPosition);
          else if (this.names[slotPosition] is long)
            this.drawFarmerSlot(b, slotPosition);
        }
      }
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      Rectangle rectangle = scissorRectangle with
      {
        Y = Math.Max(0, this.rowPosition(this.numFarmers - 1))
      };
      rectangle.Height -= rectangle.Y;
      if (rectangle.Height > 0)
      {
        b.GraphicsDevice.ScissorRectangle = rectangle;
        try
        {
          this.drawVerticalPartition(b, this.xPositionOnScreen + 256 + 12, true);
          this.drawVerticalPartition(b, this.xPositionOnScreen + 384 + 368, true);
          this.drawVerticalPartition(b, this.xPositionOnScreen + 256 + 12 + 352, true);
        }
        finally
        {
          b.GraphicsDevice.ScissorRectangle = scissorRectangle;
        }
      }
      this.upButton.draw(b);
      this.downButton.draw(b);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
      this.scrollBar.draw(b);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }
  }
}
