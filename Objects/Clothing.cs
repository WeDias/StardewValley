// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Clothing
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Clothing : Item
  {
    public const int SHIRT_SHEET_WIDTH = 128;
    [XmlElement("price")]
    public readonly NetInt price = new NetInt();
    [XmlElement("indexInTileSheet")]
    public readonly NetInt indexInTileSheetMale = new NetInt();
    [XmlElement("indexInTileSheetFemale")]
    public readonly NetInt indexInTileSheetFemale = new NetInt();
    [XmlIgnore]
    public string description;
    [XmlIgnore]
    public string displayName;
    [XmlElement("clothesType")]
    public readonly NetInt clothesType = new NetInt();
    [XmlElement("dyeable")]
    public readonly NetBool dyeable = new NetBool(false);
    [XmlElement("clothesColor")]
    public readonly NetColor clothesColor = new NetColor(new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
    [XmlElement("otherData")]
    public readonly NetString otherData = new NetString("");
    protected List<string> _otherDataList;
    [XmlElement("isPrismatic")]
    public readonly NetBool isPrismatic = new NetBool(false);
    [XmlIgnore]
    protected bool _loadedData;
    protected static int _maxShirtValue = -1;
    protected static int _maxPantsValue = -1;

    public int Price
    {
      set => this.price.Value = value;
      get => this.price.Value;
    }

    public Clothing()
    {
      this.Category = -100;
      this.NetFields.AddFields((INetSerializable) this.price, (INetSerializable) this.indexInTileSheetMale, (INetSerializable) this.indexInTileSheetFemale, (INetSerializable) this.clothesType, (INetSerializable) this.dyeable, (INetSerializable) this.clothesColor, (INetSerializable) this.otherData, (INetSerializable) this.isPrismatic);
    }

    public static int GetMaxShirtValue()
    {
      if (Clothing._maxShirtValue < 0)
      {
        foreach (string str in (IEnumerable<string>) Game1.clothingInformation.Values)
        {
          if (str != null)
          {
            string[] strArray = str.Split('/');
            if (strArray.Length >= 9 && !(strArray[8] != "Shirt"))
            {
              int num1 = int.Parse(strArray[3]);
              int num2 = int.Parse(strArray[4]);
              if (Clothing._maxShirtValue < num1)
                Clothing._maxShirtValue = num1;
              if (Clothing._maxShirtValue < num2)
                Clothing._maxShirtValue = num2;
            }
          }
        }
      }
      return Clothing._maxShirtValue;
    }

    public static int GetMaxPantsValue()
    {
      if (Clothing._maxPantsValue < 0)
      {
        foreach (string str in (IEnumerable<string>) Game1.clothingInformation.Values)
        {
          if (str != null)
          {
            string[] strArray = str.Split('/');
            if (strArray.Length >= 9 && !(strArray[8] != "Pants"))
            {
              int num1 = int.Parse(strArray[3]);
              int num2 = int.Parse(strArray[4]);
              if (Clothing._maxPantsValue < num1)
                Clothing._maxPantsValue = num1;
              if (Clothing._maxPantsValue < num1)
                Clothing._maxPantsValue = num2;
            }
          }
        }
      }
      return Clothing._maxPantsValue;
    }

    public virtual List<string> GetOtherData()
    {
      if (this.otherData.Value == null)
        return new List<string>();
      if (this._otherDataList == null)
      {
        this._otherDataList = new List<string>((IEnumerable<string>) this.otherData.Value.Split(','));
        for (int index = 0; index < this._otherDataList.Count; ++index)
        {
          if (this._otherDataList[index].Trim() == "")
          {
            this._otherDataList.RemoveAt(index);
            --index;
          }
        }
      }
      return this._otherDataList;
    }

    public Clothing(int item_index)
      : this()
    {
      this.Name = nameof (Clothing);
      this.Category = -100;
      this.ParentSheetIndex = item_index;
      this.LoadData(true);
    }

    public virtual void LoadData(bool initialize_color = false)
    {
      if (this._loadedData)
        return;
      int parentSheetIndex = this.ParentSheetIndex;
      this.Category = -100;
      if (Game1.clothingInformation.ContainsKey(parentSheetIndex))
      {
        string[] strArray1 = Game1.clothingInformation[parentSheetIndex].Split('/');
        this.Name = strArray1[0];
        this.price.Value = Convert.ToInt32(strArray1[5]);
        this.indexInTileSheetMale.Value = Convert.ToInt32(strArray1[3]);
        this.indexInTileSheetFemale.Value = Convert.ToInt32(strArray1[4]);
        this.dyeable.Value = Convert.ToBoolean(strArray1[7]);
        if (initialize_color)
        {
          string[] strArray2 = strArray1[6].Split(' ');
          this.clothesColor.Value = new Color(Convert.ToInt32(strArray2[0]), Convert.ToInt32(strArray2[1]), Convert.ToInt32(strArray2[2]));
        }
        this.displayName = strArray1[1];
        this.description = strArray1[2];
        string str = strArray1[8].ToLower().Trim();
        if (!(str == "pants"))
        {
          if (!(str == "shirt"))
          {
            if (str == "accessory")
              this.clothesType.Set(2);
          }
          else
            this.clothesType.Set(0);
        }
        else
          this.clothesType.Set(1);
        if (strArray1.Length >= 10)
          this.otherData.Set(strArray1[9]);
        else
          this.otherData.Set("");
        if (this.GetOtherData().Contains("Prismatic"))
          this.isPrismatic.Set(true);
      }
      else
      {
        this.ParentSheetIndex = parentSheetIndex;
        string[] strArray = Game1.clothingInformation[-1].Split('/');
        this.clothesType.Set(1);
        if (parentSheetIndex >= 1000)
        {
          strArray = Game1.clothingInformation[-2].Split('/');
          this.clothesType.Set(0);
          parentSheetIndex -= 1000;
        }
        if (initialize_color)
          this.clothesColor.Set(new Color(1f, 1f, 1f));
        if (this.clothesType.Value == 1)
          this.dyeable.Set(true);
        else
          this.dyeable.Set(false);
        this.displayName = strArray[1];
        this.description = strArray[2];
        this.indexInTileSheetMale.Set(parentSheetIndex);
        this.indexInTileSheetFemale.Set(-1);
      }
      if (this.dyeable.Value)
        this.description = this.description + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Clothes_Dyeable");
      this._loadedData = true;
    }

    public override string getCategoryName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:category_clothes");

    public override int salePrice() => (int) (NetFieldBase<int, NetInt>) this.price;

    public virtual void Dye(Color color, float strength = 0.5f)
    {
      if (!this.dyeable.Value)
        return;
      Color color1 = this.clothesColor.Value;
      this.clothesColor.Value = new Color(Utility.MoveTowards((float) color1.R / (float) byte.MaxValue, (float) color.R / (float) byte.MaxValue, strength), Utility.MoveTowards((float) color1.G / (float) byte.MaxValue, (float) color.G / (float) byte.MaxValue, strength), Utility.MoveTowards((float) color1.B / (float) byte.MaxValue, (float) color.B / (float) byte.MaxValue, strength), Utility.MoveTowards((float) color1.A / (float) byte.MaxValue, (float) color.A / (float) byte.MaxValue, strength));
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      Color a = (Color) (NetFieldBase<Color, NetColor>) this.clothesColor;
      if (this.isPrismatic.Value)
        a = Utility.GetPrismaticColor();
      if (this.clothesType.Value == 0)
      {
        float num = 1E-07f;
        if ((double) layerDepth >= 1.0 - (double) num)
          layerDepth = 1f - num;
        spriteBatch.Draw(FarmerRenderer.shirtsTexture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(this.indexInTileSheetMale.Value * 8 % 128, this.indexInTileSheetMale.Value * 8 / 128 * 32, 8, 8)), color * transparency, 0.0f, new Vector2(4f, 4f), scaleSize * 4f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(FarmerRenderer.shirtsTexture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(this.indexInTileSheetMale.Value * 8 % 128 + 128, this.indexInTileSheetMale.Value * 8 / 128 * 32, 8, 8)), Utility.MultiplyColor(a, color) * transparency, 0.0f, new Vector2(4f, 4f), scaleSize * 4f, SpriteEffects.None, layerDepth + num);
      }
      else
      {
        if (this.clothesType.Value != 1)
          return;
        spriteBatch.Draw(FarmerRenderer.pantsTexture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(192 * (this.indexInTileSheetMale.Value % (FarmerRenderer.pantsTexture.Width / 192)), 688 * (this.indexInTileSheetMale.Value / (FarmerRenderer.pantsTexture.Width / 192)) + 672, 16, 16)), Utility.MultiplyColor(a, color) * transparency, 0.0f, new Vector2(8f, 8f), scaleSize * 4f, SpriteEffects.None, layerDepth);
      }
    }

    public override int maximumStackSize() => 1;

    public override int addToStack(Item stack) => 1;

    public override string getDescription()
    {
      if (!this._loadedData)
        this.LoadData();
      return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
    }

    public override bool isPlaceable() => false;

    [XmlIgnore]
    public override string DisplayName
    {
      get
      {
        if (!this._loadedData)
          this.LoadData();
        return this.displayName;
      }
      set => this.displayName = value;
    }

    [XmlIgnore]
    public override int Stack
    {
      get => 1;
      set
      {
      }
    }

    public override Item getOne()
    {
      Clothing one = new Clothing(this.ParentSheetIndex);
      one.clothesColor.Value = this.clothesColor.Value;
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public enum ClothesType
    {
      SHIRT,
      PANTS,
      ACCESSORY,
    }
  }
}
