// Decompiled with JetBrains decompiler
// Type: StardewValley.BluePrint
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class BluePrint
  {
    public string name;
    public int woodRequired;
    public int stoneRequired;
    public int copperRequired;
    public int IronRequired;
    public int GoldRequired;
    public int IridiumRequired;
    public int tilesWidth;
    public int tilesHeight;
    public int maxOccupants;
    public int moneyRequired;
    public int daysToConstruct;
    public Point humanDoor;
    public Point animalDoor;
    public string mapToWarpTo;
    public string description;
    public string blueprintType;
    public string nameOfBuildingToUpgrade;
    public string actionBehavior;
    public string displayName;
    public readonly string textureName;
    public readonly Texture2D texture;
    public List<string> namesOfOkayBuildingLocations = new List<string>();
    public Rectangle sourceRectForMenuView;
    public Dictionary<int, int> itemsRequired = new Dictionary<int, int>();
    public bool canBuildOnCurrentMap;
    public bool magical;
    public List<Point> additionalPlacementTiles = new List<Point>();

    public BluePrint(string name)
    {
      this.name = name;
      if (name.Equals("Info Tool"))
      {
        this.textureName = "LooseSprites\\Cursors";
        this.displayName = name;
        this.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:BluePrint.cs.1");
        this.sourceRectForMenuView = new Rectangle(576, 0, 64, 64);
      }
      else
      {
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Blueprints");
        string str1 = (string) null;
        string key = name;
        ref string local = ref str1;
        dictionary.TryGetValue(key, out local);
        if (str1 != null)
        {
          string[] strArray1 = str1.Split('/');
          if (strArray1[0].Equals("animal"))
          {
            try
            {
              this.textureName = "Animals\\" + (name.Equals("Chicken") ? "White Chicken" : name);
            }
            catch (Exception ex)
            {
              Game1.debugOutput = "Blueprint loaded with no texture!";
            }
            this.moneyRequired = Convert.ToInt32(strArray1[1]);
            this.sourceRectForMenuView = new Rectangle(0, 0, Convert.ToInt32(strArray1[2]), Convert.ToInt32(strArray1[3]));
            this.blueprintType = "Animals";
            this.tilesWidth = 1;
            this.tilesHeight = 1;
            this.displayName = strArray1[4];
            this.description = strArray1[5];
            this.humanDoor = new Point(-1, -1);
            this.animalDoor = new Point(-1, -1);
          }
          else
          {
            this.textureName = "Buildings\\" + name;
            string[] strArray2 = strArray1[0].Split(' ');
            for (int index = 0; index < strArray2.Length; index += 2)
            {
              if (!strArray2[index].Equals(""))
                this.itemsRequired.Add(Convert.ToInt32(strArray2[index]), Convert.ToInt32(strArray2[index + 1]));
            }
            this.tilesWidth = Convert.ToInt32(strArray1[1]);
            this.tilesHeight = Convert.ToInt32(strArray1[2]);
            this.humanDoor = new Point(Convert.ToInt32(strArray1[3]), Convert.ToInt32(strArray1[4]));
            this.animalDoor = new Point(Convert.ToInt32(strArray1[5]), Convert.ToInt32(strArray1[6]));
            this.mapToWarpTo = strArray1[7];
            this.displayName = strArray1[8];
            this.description = strArray1[9];
            this.blueprintType = strArray1[10];
            if (this.blueprintType.Equals("Upgrades"))
              this.nameOfBuildingToUpgrade = strArray1[11];
            this.sourceRectForMenuView = new Rectangle(0, 0, Convert.ToInt32(strArray1[12]), Convert.ToInt32(strArray1[13]));
            this.maxOccupants = Convert.ToInt32(strArray1[14]);
            this.actionBehavior = strArray1[15];
            foreach (string str2 in strArray1[16].Split(' '))
              this.namesOfOkayBuildingLocations.Add(str2);
            int num = 17;
            if (strArray1.Length > num)
              this.moneyRequired = Convert.ToInt32(strArray1[17]);
            if (strArray1.Length > num + 1)
              this.magical = Convert.ToBoolean(strArray1[18]);
            this.daysToConstruct = strArray1.Length <= num + 2 ? 2 : Convert.ToInt32(strArray1[19]);
            if (strArray1.Length > num + 3)
            {
              string str3 = strArray1[20];
              this.additionalPlacementTiles.Clear();
              string[] strArray3 = str3.Split(' ');
              for (int index = 0; index < strArray3.Length / 2; ++index)
                this.additionalPlacementTiles.Add(new Point(Convert.ToInt32(strArray3[index * 2]), Convert.ToInt32(strArray3[index * 2 + 1])));
            }
          }
        }
      }
      try
      {
        this.texture = Game1.content.Load<Texture2D>(this.textureName);
      }
      catch (Exception ex)
      {
      }
    }

    public void consumeResources()
    {
      foreach (KeyValuePair<int, int> keyValuePair in this.itemsRequired)
        Game1.player.consumeObject(keyValuePair.Key, keyValuePair.Value);
      Game1.player.Money -= this.moneyRequired;
    }

    public int getTileSheetIndexForStructurePlacementTile(int x, int y)
    {
      if (x == this.humanDoor.X && y == this.humanDoor.Y)
        return 2;
      return x == this.animalDoor.X && y == this.animalDoor.Y ? 4 : 0;
    }

    public bool isUpgrade() => this.nameOfBuildingToUpgrade != null && this.nameOfBuildingToUpgrade.Length > 0;

    public bool doesFarmerHaveEnoughResourcesToBuild()
    {
      if (this.moneyRequired < 0)
        return false;
      foreach (KeyValuePair<int, int> keyValuePair in this.itemsRequired)
      {
        if (!Game1.player.hasItemInInventory(keyValuePair.Key, keyValuePair.Value))
          return false;
      }
      return Game1.player.Money >= this.moneyRequired;
    }

    public void drawDescription(SpriteBatch b, int x, int y, int width)
    {
      b.DrawString(Game1.smallFont, this.name, new Vector2((float) x, (float) y), Game1.textColor);
      string text1 = Game1.parseText(this.description, Game1.smallFont, width);
      b.DrawString(Game1.smallFont, text1, new Vector2((float) x, (float) y + Game1.smallFont.MeasureString(this.name).Y), Game1.textColor * 0.75f);
      int y1 = (int) ((double) y + (double) Game1.smallFont.MeasureString(this.name).Y + (double) Game1.smallFont.MeasureString(text1).Y);
      foreach (KeyValuePair<int, int> keyValuePair in this.itemsRequired)
      {
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (x + 8), (float) y1), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, keyValuePair.Key, 16, 16)), Color.White, 0.0f, new Vector2(6f, 3f), 2f, SpriteEffects.None, 0.999f);
        Color color = Game1.player.hasItemInInventory(keyValuePair.Key, keyValuePair.Value) ? Color.DarkGreen : Color.DarkRed;
        int toDraw = keyValuePair.Value;
        SpriteBatch b1 = b;
        double num1 = (double) (x + 32);
        SpriteFont tinyFont1 = Game1.tinyFont;
        int num2 = keyValuePair.Value;
        string text2 = num2.ToString() ?? "";
        double x1 = (double) tinyFont1.MeasureString(text2).X;
        double x2 = num1 - x1;
        double num3 = (double) (y1 + 32);
        SpriteFont tinyFont2 = Game1.tinyFont;
        num2 = keyValuePair.Value;
        string text3 = num2.ToString() ?? "";
        double y2 = (double) tinyFont2.MeasureString(text3).Y;
        double y3 = num3 - y2;
        Vector2 position = new Vector2((float) x2, (float) y3);
        Color antiqueWhite = Color.AntiqueWhite;
        Utility.drawTinyDigits(toDraw, b1, position, 1f, 0.9f, antiqueWhite);
        b.DrawString(Game1.smallFont, Game1.objectInformation[keyValuePair.Key].Split('/')[4], new Vector2((float) (x + 32 + 16), (float) y1), color);
        y1 += (int) Game1.smallFont.MeasureString("P").Y;
      }
      if (this.moneyRequired <= 0)
        return;
      b.Draw(Game1.debrisSpriteSheet, new Vector2((float) x, (float) y1), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8)), Color.White, 0.0f, new Vector2(24f, 11f), 0.5f, SpriteEffects.None, 0.999f);
      Color color1 = Game1.player.Money >= this.moneyRequired ? Color.DarkGreen : Color.DarkRed;
      b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.moneyRequired), new Vector2((float) (x + 16 + 8), (float) y1), color1);
      int num = y1 + (int) Game1.smallFont.MeasureString(this.moneyRequired.ToString() ?? "").Y;
    }
  }
}
