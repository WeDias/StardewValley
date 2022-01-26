// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Hat
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
  public class Hat : Item
  {
    public const int widthOfTileSheetSquare = 20;
    public const int heightOfTileSheetSquare = 20;
    [XmlElement("which")]
    public readonly NetInt which = new NetInt();
    [XmlElement("skipHairDraw")]
    public bool skipHairDraw;
    [XmlElement("ignoreHairstyleOffset")]
    public readonly NetBool ignoreHairstyleOffset = new NetBool();
    [XmlElement("hairDrawType")]
    public readonly NetInt hairDrawType = new NetInt();
    [XmlElement("isPrismatic")]
    public readonly NetBool isPrismatic = new NetBool(false);
    [XmlIgnore]
    protected int _isMask = -1;
    [XmlIgnore]
    public string displayName;
    [XmlIgnore]
    public string description;

    [XmlIgnore]
    public bool isMask
    {
      get
      {
        if (this._isMask == -1)
        {
          this._isMask = !this.Name.Contains("Mask") ? 0 : 1;
          if ((int) (NetFieldBase<int, NetInt>) this.hairDrawType == 2)
            this._isMask = 0;
        }
        return this._isMask == 1;
      }
    }

    public Hat()
    {
      this.NetFields.AddFields((INetSerializable) this.which, (INetSerializable) this.ignoreHairstyleOffset, (INetSerializable) this.hairDrawType, (INetSerializable) this.isPrismatic);
      this.load((int) (NetFieldBase<int, NetInt>) this.which);
    }

    public void load(int which)
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\hats");
      if (!dictionary.ContainsKey(which))
        which = 0;
      string[] strArray = dictionary[which].Split('/');
      this.Name = strArray[0];
      if (strArray[2] == "hide")
        this.hairDrawType.Set(2);
      else if (Convert.ToBoolean(strArray[2]))
        this.hairDrawType.Set(0);
      else
        this.hairDrawType.Set(1);
      if (this.skipHairDraw)
      {
        this.skipHairDraw = false;
        this.hairDrawType.Set(0);
      }
      if (strArray.Length > 4)
      {
        foreach (string str in strArray[4].Split(' '))
        {
          if (str == "Prismatic")
            this.isPrismatic.Value = true;
        }
      }
      this.ignoreHairstyleOffset.Value = Convert.ToBoolean(strArray[3]);
      this.Category = -95;
    }

    public Hat(int which)
      : this()
    {
      this.which.Value = which;
      this.load(which);
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
      float num = scaleSize;
      scaleSize *= 0.75f;
      spriteBatch.Draw(FarmerRenderer.hatsTexture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.which * 20 % FarmerRenderer.hatsTexture.Width, (int) (NetFieldBase<int, NetInt>) this.which * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20)), (bool) (NetFieldBase<bool, NetBool>) this.isPrismatic ? Utility.GetPrismaticColor() * transparency : color * transparency, 0.0f, new Vector2(10f, 10f), 4f * scaleSize, SpriteEffects.None, layerDepth);
      if (((drawStackNumber != StackDrawType.Draw || this.maximumStackSize() <= 1 || this.Stack <= 1) && drawStackNumber != StackDrawType.Draw_OneInclusive || (double) scaleSize <= 0.3 ? 0 : (this.Stack != int.MaxValue ? 1 : 0)) == 0)
        return;
      Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float) (64 - Utility.getWidthOfTinyDigitString(this.Stack, 3f * num)) + 3f * num, (float) (64.0 - 18.0 * (double) num + 2.0)), 3f * num, 1f, color);
    }

    public void draw(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      int direction)
    {
      switch (direction)
      {
        case 0:
          direction = 3;
          break;
        case 2:
          direction = 0;
          break;
        case 3:
          direction = 2;
          break;
      }
      spriteBatch.Draw(FarmerRenderer.hatsTexture, location + new Vector2(10f, 10f), new Rectangle?(new Rectangle((int) (NetFieldBase<int, NetInt>) this.which * 20 % FarmerRenderer.hatsTexture.Width, (int) (NetFieldBase<int, NetInt>) this.which * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4 + direction * 20, 20, 20)), (bool) (NetFieldBase<bool, NetBool>) this.isPrismatic ? Utility.GetPrismaticColor() * transparency : Color.White * transparency, 0.0f, new Vector2(3f, 3f), 3f * scaleSize, SpriteEffects.None, layerDepth);
    }

    public override string getDescription()
    {
      if (this.description == null)
        this.loadDisplayFields();
      return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
    }

    public override int maximumStackSize() => 1;

    public override int addToStack(Item stack) => 1;

    public override bool isPlaceable() => false;

    [XmlIgnore]
    public override string DisplayName
    {
      get
      {
        if (this.displayName == null)
          this.loadDisplayFields();
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
      Hat one = new Hat((int) (NetFieldBase<int, NetInt>) this.which);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    private bool loadDisplayFields()
    {
      if (this.Name != null)
      {
        foreach (KeyValuePair<int, string> keyValuePair in Game1.content.Load<Dictionary<int, string>>("Data\\hats"))
        {
          string[] strArray = keyValuePair.Value.Split('/');
          if (strArray[0] == this.Name)
          {
            this.displayName = this.Name;
            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
              this.displayName = strArray[strArray.Length - 1];
            this.description = strArray[1];
            return true;
          }
        }
      }
      return false;
    }

    public enum HairDrawType
    {
      DrawFullHair,
      DrawObscuredHair,
      HideHair,
    }
  }
}
