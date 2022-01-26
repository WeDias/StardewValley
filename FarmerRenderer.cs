// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmerRenderer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  [InstanceStatics]
  public class FarmerRenderer : INetObject<NetFields>
  {
    public const int sleeveDarkestColorIndex = 256;
    public const int skinDarkestColorIndex = 260;
    public const int shoeDarkestColorIndex = 268;
    public const int eyeLightestColorIndex = 276;
    public const int accessoryDrawBelowHairThreshold = 8;
    public const int accessoryFacialHairThreshold = 6;
    protected bool _sickFrame;
    public static bool isDrawingForUI = false;
    public const int pantsOffset = 288;
    public const int armOffset = 96;
    public const int secondaryArmOffset = 192;
    public const int shirtXOffset = 16;
    public const int shirtYOffset = 56;
    public static int[] featureYOffsetPerFrame = new int[126]
    {
      1,
      2,
      2,
      0,
      5,
      6,
      1,
      2,
      2,
      1,
      0,
      2,
      0,
      1,
      1,
      0,
      2,
      2,
      3,
      3,
      2,
      2,
      1,
      1,
      0,
      0,
      2,
      2,
      4,
      4,
      0,
      0,
      1,
      2,
      1,
      1,
      1,
      1,
      0,
      0,
      1,
      1,
      1,
      0,
      0,
      -2,
      -1,
      1,
      1,
      0,
      -1,
      -2,
      -1,
      -1,
      5,
      4,
      0,
      0,
      3,
      2,
      -1,
      0,
      4,
      2,
      0,
      0,
      2,
      1,
      0,
      -1,
      1,
      -2,
      0,
      0,
      1,
      1,
      1,
      1,
      1,
      1,
      0,
      0,
      0,
      0,
      1,
      -1,
      -1,
      -1,
      -1,
      1,
      1,
      0,
      0,
      0,
      0,
      4,
      1,
      0,
      1,
      2,
      1,
      0,
      1,
      0,
      1,
      2,
      -3,
      -4,
      -1,
      0,
      0,
      2,
      1,
      -4,
      -1,
      0,
      0,
      -3,
      0,
      0,
      -1,
      0,
      0,
      2,
      1,
      1
    };
    public static int[] featureXOffsetPerFrame = new int[126]
    {
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      -1,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      -1,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      -1,
      -1,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      4,
      0,
      0,
      0,
      0,
      -1,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      -1,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0
    };
    public static int[] hairstyleHatOffset = new int[16]
    {
      0,
      0,
      0,
      4,
      0,
      0,
      3,
      0,
      4,
      0,
      0,
      0,
      0,
      0,
      0,
      0
    };
    public static Texture2D hairStylesTexture;
    public static Texture2D shirtsTexture;
    public static Texture2D hatsTexture;
    public static Texture2D accessoriesTexture;
    public static Texture2D pantsTexture;
    protected static Dictionary<string, Dictionary<int, List<int>>> _recolorOffsets;
    [XmlElement("textureName")]
    public readonly NetString textureName = new NetString();
    [XmlIgnore]
    private LocalizedContentManager farmerTextureManager;
    [XmlIgnore]
    private Texture2D baseTexture;
    [XmlElement("heightOffset")]
    public readonly NetInt heightOffset = new NetInt(0);
    [XmlIgnore]
    private readonly NetColor eyes = new NetColor();
    [XmlIgnore]
    private readonly NetInt skin = new NetInt();
    [XmlIgnore]
    private readonly NetInt shoes = new NetInt();
    [XmlIgnore]
    private readonly NetInt shirt = new NetInt();
    [XmlIgnore]
    private readonly NetInt pants = new NetInt();
    protected bool _spriteDirty;
    protected bool _baseTextureDirty;
    protected bool _eyesDirty;
    protected bool _skinDirty;
    protected bool _shoesDirty;
    protected bool _shirtDirty;
    protected bool _pantsDirty;
    private Rectangle shirtSourceRect;
    private Rectangle hairstyleSourceRect;
    private Rectangle hatSourceRect;
    private Rectangle accessorySourceRect;
    private Vector2 rotationAdjustment;
    private Vector2 positionOffset;

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public FarmerRenderer()
    {
      this.NetFields.AddFields((INetSerializable) this.textureName, (INetSerializable) this.heightOffset, (INetSerializable) this.eyes, (INetSerializable) this.skin, (INetSerializable) this.shoes, (INetSerializable) this.shirt, (INetSerializable) this.pants);
      this.farmerTextureManager = Game1.content.CreateTemporary();
      this.textureName.fieldChangeVisibleEvent += (NetFieldBase<string, NetString>.FieldChange) ((field, old_value, new_value) =>
      {
        this._spriteDirty = true;
        this._baseTextureDirty = true;
      });
      this.eyes.fieldChangeVisibleEvent += (NetFieldBase<Color, NetColor>.FieldChange) ((field, old_value, new_value) =>
      {
        this._spriteDirty = true;
        this._eyesDirty = true;
      });
      this.skin.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) =>
      {
        this._spriteDirty = true;
        this._skinDirty = true;
        this._shirtDirty = true;
      });
      this.shoes.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) =>
      {
        this._spriteDirty = true;
        this._shoesDirty = true;
      });
      this.shirt.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) =>
      {
        this._spriteDirty = true;
        this._shirtDirty = true;
      });
      this.pants.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) =>
      {
        this._spriteDirty = true;
        this._pantsDirty = true;
      });
      this._spriteDirty = true;
      this._baseTextureDirty = true;
    }

    public FarmerRenderer(string textureName, Farmer farmer)
      : this()
    {
      this.eyes.Set(farmer.newEyeColor.Value);
      this.textureName.Set(textureName);
      this._spriteDirty = true;
      this._baseTextureDirty = true;
    }

    private void executeRecolorActions(Farmer farmer)
    {
      if (!this._spriteDirty)
        return;
      this._spriteDirty = false;
      if (this._baseTextureDirty)
      {
        this._baseTextureDirty = false;
        this.textureChanged();
        this._eyesDirty = true;
        this._shoesDirty = true;
        this._pantsDirty = true;
        this._skinDirty = true;
        this._shirtDirty = true;
      }
      if (FarmerRenderer._recolorOffsets == null)
        FarmerRenderer._recolorOffsets = new Dictionary<string, Dictionary<int, List<int>>>();
      if (!FarmerRenderer._recolorOffsets.ContainsKey((string) (NetFieldBase<string, NetString>) this.textureName))
      {
        FarmerRenderer._recolorOffsets[this.textureName.Value] = new Dictionary<int, List<int>>();
        Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>(this.textureName.Value);
        Color[] colorArray = new Color[texture2D.Width * texture2D.Height];
        texture2D.GetData<Color>(colorArray);
        this._GeneratePixelIndices(256, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(257, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(258, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(268, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(269, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(270, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(271, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(260, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(261, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(262, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(276, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
        this._GeneratePixelIndices(277, (string) (NetFieldBase<string, NetString>) this.textureName, colorArray);
      }
      Color[] colorArray1 = new Color[this.baseTexture.Width * this.baseTexture.Height];
      this.baseTexture.GetData<Color>(colorArray1);
      if (this._eyesDirty)
      {
        this._eyesDirty = false;
        this.ApplyEyeColor((string) (NetFieldBase<string, NetString>) this.textureName, colorArray1);
      }
      if (this._skinDirty)
      {
        this._skinDirty = false;
        this.ApplySkinColor((string) (NetFieldBase<string, NetString>) this.textureName, colorArray1);
      }
      if (this._shoesDirty)
      {
        this._shoesDirty = false;
        this.ApplyShoeColor((string) (NetFieldBase<string, NetString>) this.textureName, colorArray1);
      }
      if (this._shirtDirty)
      {
        this._shirtDirty = false;
        this.ApplySleeveColor((string) (NetFieldBase<string, NetString>) this.textureName, colorArray1, farmer);
      }
      if (this._pantsDirty)
        this._pantsDirty = false;
      this.baseTexture.SetData<Color>(colorArray1);
    }

    protected void _GeneratePixelIndices(
      int source_color_index,
      string texture_name,
      Color[] pixels)
    {
      Color pixel = pixels[source_color_index];
      List<int> intList = new List<int>();
      for (int index = 0; index < pixels.Length; ++index)
      {
        if ((int) pixels[index].PackedValue == (int) pixel.PackedValue)
          intList.Add(index);
      }
      FarmerRenderer._recolorOffsets[texture_name][source_color_index] = intList;
    }

    public void unload()
    {
      this.farmerTextureManager.Unload();
      this.farmerTextureManager.Dispose();
    }

    private void textureChanged()
    {
      if (this.baseTexture != null)
      {
        this.baseTexture.Dispose();
        this.baseTexture = (Texture2D) null;
      }
      Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>(this.textureName.Value);
      this.baseTexture = new Texture2D(Game1.graphics.GraphicsDevice, texture2D.Width, texture2D.Height);
      Color[] data = new Color[texture2D.Width * texture2D.Height];
      texture2D.GetData<Color>(data, 0, data.Length);
      this.baseTexture.SetData<Color>(data);
    }

    public void recolorEyes(Color lightestColor) => this.eyes.Set(lightestColor);

    private void ApplyEyeColor(string texture_name, Color[] pixels)
    {
      Color color1 = this.eyes.Value;
      Color color2 = FarmerRenderer.changeBrightness(color1, -75);
      if (color1.Equals(color2))
        color1.B += (byte) 10;
      this._SwapColor(texture_name, pixels, 276, color1);
      this._SwapColor(texture_name, pixels, 277, color2);
    }

    private void _SwapColor(string texture_name, Color[] pixels, int color_index, Color color)
    {
      List<int> intList = FarmerRenderer._recolorOffsets[texture_name][color_index];
      for (int index = 0; index < intList.Count; ++index)
        pixels[intList[index]] = color;
    }

    public void recolorShoes(int which) => this.shoes.Set(which);

    private void ApplyShoeColor(string texture_name, Color[] pixels)
    {
      int num = this.shoes.Value;
      Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>("Characters\\Farmer\\shoeColors");
      Color[] data = new Color[texture2D.Width * texture2D.Height];
      texture2D.GetData<Color>(data);
      Color color1 = data[num * 4 % (texture2D.Height * 4)];
      Color color2 = data[num * 4 % (texture2D.Height * 4) + 1];
      Color color3 = data[num * 4 % (texture2D.Height * 4) + 2];
      Color color4 = data[num * 4 % (texture2D.Height * 4) + 3];
      this._SwapColor(texture_name, pixels, 268, color1);
      this._SwapColor(texture_name, pixels, 269, color2);
      this._SwapColor(texture_name, pixels, 270, color3);
      this._SwapColor(texture_name, pixels, 271, color4);
    }

    public int recolorSkin(int which, bool force = false)
    {
      if (force)
        this.skin.Value = -1;
      this.skin.Set(which);
      return which;
    }

    private void ApplySkinColor(string texture_name, Color[] pixels)
    {
      int num = this.skin.Value;
      Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>("Characters\\Farmer\\skinColors");
      Color[] data = new Color[texture2D.Width * texture2D.Height];
      if (num < 0)
        num = texture2D.Height - 1;
      if (num > texture2D.Height - 1)
        num = 0;
      texture2D.GetData<Color>(data);
      Color color1 = data[num * 3 % (texture2D.Height * 3)];
      Color color2 = data[num * 3 % (texture2D.Height * 3) + 1];
      Color color3 = data[num * 3 % (texture2D.Height * 3) + 2];
      this._SwapColor(texture_name, pixels, 260, color1);
      this._SwapColor(texture_name, pixels, 261, color2);
      this._SwapColor(texture_name, pixels, 262, color3);
    }

    public void changeShirt(int whichShirt) => this.shirt.Set(whichShirt);

    public void changePants(int whichPants) => this.pants.Set(whichPants);

    public void MarkSpriteDirty()
    {
      this._spriteDirty = true;
      this._shirtDirty = true;
      this._pantsDirty = true;
      this._eyesDirty = true;
      this._shoesDirty = true;
      this._baseTextureDirty = true;
    }

    public void ApplySleeveColor(string texture_name, Color[] pixels, Farmer who)
    {
      Color[] data1 = new Color[FarmerRenderer.shirtsTexture.Bounds.Width * FarmerRenderer.shirtsTexture.Bounds.Height];
      FarmerRenderer.shirtsTexture.GetData<Color>(data1);
      int index1 = this.ClampShirt(who.GetShirtIndex()) * 8 / 128 * 32 * FarmerRenderer.shirtsTexture.Bounds.Width + this.ClampShirt(who.GetShirtIndex()) * 8 % 128 + FarmerRenderer.shirtsTexture.Width * 4;
      int index2 = index1 + 128;
      Color color1 = Color.White;
      if (who.GetShirtExtraData().Contains("Sleeveless") || index1 >= data1.Length)
      {
        Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>("Characters\\Farmer\\skinColors");
        Color[] data2 = new Color[texture2D.Width * texture2D.Height];
        int num = this.skin.Value;
        if (num < 0)
          num = texture2D.Height - 1;
        if (num > texture2D.Height - 1)
          num = 0;
        texture2D.GetData<Color>(data2);
        Color pixel1 = data2[num * 3 % (texture2D.Height * 3)];
        Color pixel2 = data2[num * 3 % (texture2D.Height * 3) + 1];
        Color pixel3 = data2[num * 3 % (texture2D.Height * 3) + 2];
        if (this._sickFrame)
        {
          pixel1 = pixels[260 + this.baseTexture.Width];
          pixel2 = pixels[261 + this.baseTexture.Width];
          pixel3 = pixels[262 + this.baseTexture.Width];
        }
        color1 = pixel1;
        this._SwapColor(texture_name, pixels, 256, pixel1);
        this._SwapColor(texture_name, pixels, 257, pixel2);
        this._SwapColor(texture_name, pixels, 258, pixel3);
      }
      else
      {
        Color color2 = Utility.MakeCompletelyOpaque(who.GetShirtColor());
        Color color3 = data1[index2];
        Color b = color2;
        if (color3.A < byte.MaxValue)
        {
          color3 = data1[index1];
          b = Color.White;
        }
        color3 = Utility.MultiplyColor(color3, b);
        this._SwapColor(texture_name, pixels, 256, color3);
        color3 = data1[index2 - FarmerRenderer.shirtsTexture.Width];
        if (color3.A < byte.MaxValue)
        {
          color3 = data1[index1 - FarmerRenderer.shirtsTexture.Width];
          b = Color.White;
        }
        color3 = Utility.MultiplyColor(color3, b);
        this._SwapColor(texture_name, pixels, 257, color3);
        color3 = data1[index2 - FarmerRenderer.shirtsTexture.Width * 2];
        if (color3.A < byte.MaxValue)
        {
          color3 = data1[index1 - FarmerRenderer.shirtsTexture.Width * 2];
          b = Color.White;
        }
        color3 = Utility.MultiplyColor(color3, b);
        this._SwapColor(texture_name, pixels, 258, color3);
      }
    }

    private static Color changeBrightness(Color c, int brightness)
    {
      c.R = (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) c.R + brightness));
      c.G = (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) c.G + brightness));
      c.B = (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) c.B + (brightness > 0 ? brightness * 5 / 6 : brightness * 8 / 7)));
      return c;
    }

    public void draw(
      SpriteBatch b,
      Farmer who,
      int whichFrame,
      Vector2 position,
      float layerDepth = 1f,
      bool flip = false)
    {
      who.FarmerSprite.setCurrentSingleFrame(whichFrame, flip: flip);
      this.draw(b, who.FarmerSprite, who.FarmerSprite.SourceRect, position, Vector2.Zero, layerDepth, Color.White, 0.0f, who);
    }

    public void draw(
      SpriteBatch b,
      FarmerSprite farmerSprite,
      Rectangle sourceRect,
      Vector2 position,
      Vector2 origin,
      float layerDepth,
      Color overrideColor,
      float rotation,
      Farmer who)
    {
      this.draw(b, farmerSprite.CurrentAnimationFrame, farmerSprite.CurrentFrame, sourceRect, position, origin, layerDepth, overrideColor, rotation, 1f, who);
    }

    public int ClampShirt(int shirt_value) => shirt_value > Clothing.GetMaxShirtValue() || shirt_value < 0 ? 0 : shirt_value;

    public int ClampPants(int pants_value) => pants_value > Clothing.GetMaxPantsValue() || pants_value < 0 ? 0 : pants_value;

    public void drawMiniPortrat(
      SpriteBatch b,
      Vector2 position,
      float layerDepth,
      float scale,
      int facingDirection,
      Farmer who)
    {
      int hair = who.getHair(true);
      HairStyleMetadata hairStyleMetadata = Farmer.GetHairStyleMetadata(who.hair.Value);
      this.executeRecolorActions(who);
      facingDirection = 2;
      bool flag = false;
      int y = 0;
      int num1 = 0;
      Texture2D texture = FarmerRenderer.hairStylesTexture;
      this.hairstyleSourceRect = new Rectangle(hair * 16 % FarmerRenderer.hairStylesTexture.Width, hair * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 15);
      if (hairStyleMetadata != null)
      {
        texture = hairStyleMetadata.texture;
        this.hairstyleSourceRect = new Rectangle(hairStyleMetadata.tileX * 16, hairStyleMetadata.tileY * 16, 16, 15);
      }
      switch (facingDirection)
      {
        case 0:
          y = 64;
          this.hairstyleSourceRect.Offset(0, 64);
          num1 = FarmerRenderer.featureYOffsetPerFrame[12];
          break;
        case 1:
          y = 32;
          this.hairstyleSourceRect.Offset(0, 32);
          num1 = FarmerRenderer.featureYOffsetPerFrame[6];
          break;
        case 2:
          y = 0;
          this.hairstyleSourceRect.Offset(0, 0);
          num1 = FarmerRenderer.featureYOffsetPerFrame[0];
          break;
        case 3:
          if (hairStyleMetadata != null && hairStyleMetadata.usesUniqueLeftSprite)
          {
            flag = false;
            y = 96;
          }
          else
          {
            flag = true;
            y = 32;
          }
          this.hairstyleSourceRect.Offset(0, 32);
          num1 = FarmerRenderer.featureYOffsetPerFrame[6];
          break;
      }
      b.Draw(this.baseTexture, position, new Rectangle?(new Rectangle(0, y, 16, (bool) (NetFieldBase<bool, NetBool>) who.isMale ? 15 : 16)), Color.White, 0.0f, Vector2.Zero, scale, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
      int num2 = Game1.isUsingBackToFrontSorting ? -1 : 1;
      b.Draw(texture, position + new Vector2(0.0f, (float) (num1 * 4 + (!who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair < 16 ? (who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair >= 16 ? 0 : 4) : -4))) * scale / 4f, new Rectangle?(this.hairstyleSourceRect), (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor, 0.0f, Vector2.Zero, scale, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 1.1E-07f * (float) num2);
    }

    public void draw(
      SpriteBatch b,
      FarmerSprite.AnimationFrame animationFrame,
      int currentFrame,
      Rectangle sourceRect,
      Vector2 position,
      Vector2 origin,
      float layerDepth,
      Color overrideColor,
      float rotation,
      float scale,
      Farmer who)
    {
      this.draw(b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, who.FacingDirection, overrideColor, rotation, scale, who);
    }

    public void drawHairAndAccesories(
      SpriteBatch b,
      int facingDirection,
      Farmer who,
      Vector2 position,
      Vector2 origin,
      float scale,
      int currentFrame,
      float rotation,
      Color overrideColor,
      float layerDepth)
    {
      int hair_index = who.getHair();
      HairStyleMetadata hairStyleMetadata = Farmer.GetHairStyleMetadata(who.hair.Value);
      if (who != null && who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1 && hairStyleMetadata != null && hairStyleMetadata.coveredIndex != -1)
      {
        hair_index = hairStyleMetadata.coveredIndex;
        hairStyleMetadata = Farmer.GetHairStyleMetadata(hair_index);
      }
      this.executeRecolorActions(who);
      this.shirtSourceRect = new Rectangle(this.ClampShirt(who.GetShirtIndex()) * 8 % 128, this.ClampShirt(who.GetShirtIndex()) * 8 / 128 * 32, 8, 8);
      Texture2D texture = FarmerRenderer.hairStylesTexture;
      this.hairstyleSourceRect = new Rectangle(hair_index * 16 % FarmerRenderer.hairStylesTexture.Width, hair_index * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 32);
      if (hairStyleMetadata != null)
      {
        texture = hairStyleMetadata.texture;
        this.hairstyleSourceRect = new Rectangle(hairStyleMetadata.tileX * 16, hairStyleMetadata.tileY * 16, 16, 32);
      }
      if ((int) (NetFieldBase<int, NetInt>) who.accessory >= 0)
        this.accessorySourceRect = new Rectangle((int) (NetFieldBase<int, NetInt>) who.accessory * 16 % FarmerRenderer.accessoriesTexture.Width, (int) (NetFieldBase<int, NetInt>) who.accessory * 16 / FarmerRenderer.accessoriesTexture.Width * 32, 16, 16);
      if (who.hat.Value != null)
        this.hatSourceRect = new Rectangle(20 * (int) (NetFieldBase<int, NetInt>) who.hat.Value.which % FarmerRenderer.hatsTexture.Width, 20 * (int) (NetFieldBase<int, NetInt>) who.hat.Value.which / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20);
      Rectangle shirtSourceRect1 = this.shirtSourceRect;
      float num1 = 1E-07f;
      float num2 = 2.2E-05f;
      switch (facingDirection)
      {
        case 0:
          this.shirtSourceRect.Offset(0, 24);
          this.hairstyleSourceRect.Offset(0, 64);
          Rectangle shirtSourceRect2 = this.shirtSourceRect;
          shirtSourceRect2.Offset(128, 0);
          if (who.hat.Value != null)
            this.hatSourceRect.Offset(0, 60);
          if (!(bool) (NetFieldBase<bool, NetBool>) who.bathingClothes)
          {
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + new Vector2(16f * scale + (float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float) (int) (NetFieldBase<int, NetInt>) this.heightOffset * scale), new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.8E-07f);
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + new Vector2(16f * scale + (float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float) (int) (NetFieldBase<int, NetInt>) this.heightOffset * scale), new Rectangle?(shirtSourceRect2), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.8E-07f + num1);
          }
          b.Draw(texture, position + origin + this.positionOffset + new Vector2((float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + (!who.IsMale || hair_index < 16 ? (who.IsMale || hair_index >= 16 ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + num2);
          break;
        case 1:
          this.shirtSourceRect.Offset(0, 8);
          this.hairstyleSourceRect.Offset(0, 32);
          Rectangle shirtSourceRect3 = this.shirtSourceRect;
          shirtSourceRect3.Offset(128, 0);
          if ((int) (NetFieldBase<int, NetInt>) who.accessory >= 0)
            this.accessorySourceRect.Offset(0, 16);
          if (who.hat.Value != null)
            this.hatSourceRect.Offset(0, 20);
          if ((double) rotation == -0.0981747731566429)
          {
            this.rotationAdjustment.X = 6f;
            this.rotationAdjustment.Y = -2f;
          }
          else if ((double) rotation == 0.0981747731566429)
          {
            this.rotationAdjustment.X = -6f;
            this.rotationAdjustment.Y = 1f;
          }
          if (!(bool) (NetFieldBase<bool, NetBool>) who.bathingClothes)
          {
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2(16f * scale + (float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (56.0 * (double) scale + (double) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (double) (int) (NetFieldBase<int, NetInt>) this.heightOffset * (double) scale)), new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, (float) (4.0 * (double) scale + ((double) rotation != 0.0 ? 0.0 : 0.0)), SpriteEffects.None, layerDepth + 1.8E-07f);
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2(16f * scale + (float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (56.0 * (double) scale + (double) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (double) (int) (NetFieldBase<int, NetInt>) this.heightOffset * (double) scale)), new Rectangle?(shirtSourceRect3), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, (float) (4.0 * (double) scale + ((double) rotation != 0.0 ? 0.0 : 0.0)), SpriteEffects.None, layerDepth + 1.8E-07f + num1);
          }
          if ((int) (NetFieldBase<int, NetInt>) who.accessory >= 0)
            b.Draw(FarmerRenderer.accessoriesTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset)), new Rectangle?(this.accessorySourceRect), !overrideColor.Equals(Color.White) || (int) (NetFieldBase<int, NetInt>) who.accessory >= 6 ? overrideColor : (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor, rotation, origin, (float) (4.0 * (double) scale + ((double) rotation != 0.0 ? 0.0 : 0.0)), SpriteEffects.None, layerDepth + ((int) (NetFieldBase<int, NetInt>) who.accessory < 8 ? 1.9E-05f : 2.9E-05f));
          b.Draw(texture, position + origin + this.positionOffset + new Vector2((float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (!who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair < 16 ? (who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair >= 16 ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + num2);
          break;
        case 2:
          Rectangle shirtSourceRect4 = this.shirtSourceRect;
          shirtSourceRect4.Offset(128, 0);
          if (!(bool) (NetFieldBase<bool, NetBool>) who.bathingClothes)
          {
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + new Vector2((float) (16 + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) ((double) (56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (double) (int) (NetFieldBase<int, NetInt>) this.heightOffset * (double) scale - (who.IsMale ? 0.0 : 0.0))), new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.5E-07f);
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + new Vector2((float) (16 + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) ((double) (56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (double) (int) (NetFieldBase<int, NetInt>) this.heightOffset * (double) scale - (who.IsMale ? 0.0 : 0.0))), new Rectangle?(shirtSourceRect4), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.5E-07f + num1);
          }
          if ((int) (NetFieldBase<int, NetInt>) who.accessory >= 0)
            b.Draw(FarmerRenderer.accessoriesTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (8 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset - 4)), new Rectangle?(this.accessorySourceRect), !overrideColor.Equals(Color.White) || (int) (NetFieldBase<int, NetInt>) who.accessory >= 6 ? overrideColor : (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor, rotation, origin, (float) (4.0 * (double) scale + ((double) rotation != 0.0 ? 0.0 : 0.0)), SpriteEffects.None, layerDepth + ((int) (NetFieldBase<int, NetInt>) who.accessory < 8 ? 1.9E-05f : 2.9E-05f));
          b.Draw(texture, position + origin + this.positionOffset + new Vector2((float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (!who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair < 16 ? (who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair >= 16 ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + num2);
          break;
        case 3:
          bool flag = true;
          this.shirtSourceRect.Offset(0, 16);
          Rectangle shirtSourceRect5 = this.shirtSourceRect;
          shirtSourceRect5.Offset(128, 0);
          if ((int) (NetFieldBase<int, NetInt>) who.accessory >= 0)
            this.accessorySourceRect.Offset(0, 16);
          if (hairStyleMetadata != null && hairStyleMetadata.usesUniqueLeftSprite)
          {
            flag = false;
            this.hairstyleSourceRect.Offset(0, 96);
          }
          else
            this.hairstyleSourceRect.Offset(0, 32);
          if (who.hat.Value != null)
            this.hatSourceRect.Offset(0, 40);
          if ((double) rotation == -0.0981747731566429)
          {
            this.rotationAdjustment.X = 6f;
            this.rotationAdjustment.Y = -2f;
          }
          else if ((double) rotation == 0.0981747731566429)
          {
            this.rotationAdjustment.X = -5f;
            this.rotationAdjustment.Y = 1f;
          }
          if (!(bool) (NetFieldBase<bool, NetBool>) who.bathingClothes)
          {
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (16 - FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset)), new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, (float) (4.0 * (double) scale + ((double) rotation != 0.0 ? 0.0 : 0.0)), SpriteEffects.None, layerDepth + 1.5E-07f);
            b.Draw(FarmerRenderer.shirtsTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (16 - FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset)), new Rectangle?(shirtSourceRect5), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, (float) (4.0 * (double) scale + ((double) rotation != 0.0 ? 0.0 : 0.0)), SpriteEffects.None, layerDepth + 1.5E-07f + num1);
          }
          if ((int) (NetFieldBase<int, NetInt>) who.accessory >= 0)
            b.Draw(FarmerRenderer.accessoriesTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset)), new Rectangle?(this.accessorySourceRect), !overrideColor.Equals(Color.White) || (int) (NetFieldBase<int, NetInt>) who.accessory >= 6 ? overrideColor : (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor, rotation, origin, (float) (4.0 * (double) scale + ((double) rotation != 0.0 ? 0.0 : 0.0)), SpriteEffects.FlipHorizontally, layerDepth + ((int) (NetFieldBase<int, NetInt>) who.accessory < 8 ? 1.9E-05f : 2.9E-05f));
          b.Draw(texture, position + origin + this.positionOffset + new Vector2((float) (-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (!who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair < 16 ? (who.IsMale || (int) (NetFieldBase<int, NetInt>) who.hair >= 16 ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? (Color) (NetFieldBase<Color, NetColor>) who.hairstyleColor : overrideColor, rotation, origin, 4f * scale, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + num2);
          break;
      }
      if (who.hat.Value == null || (bool) (NetFieldBase<bool, NetBool>) who.bathingClothes)
        return;
      bool flip = who.FarmerSprite.CurrentAnimationFrame.flip;
      float num3 = 3.9E-05f;
      if (who.hat.Value.isMask && facingDirection == 0)
      {
        Rectangle hatSourceRect = this.hatSourceRect;
        hatSourceRect.Height -= 11;
        hatSourceRect.Y += 11;
        b.Draw(FarmerRenderer.hatsTexture, position + origin + this.positionOffset + new Vector2(0.0f, 44f) + new Vector2((float) ((flip ? -1 : 1) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4 - 8), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 - 16 + ((bool) (NetFieldBase<bool, NetBool>) who.hat.Value.ignoreHairstyleOffset ? 0 : FarmerRenderer.hairstyleHatOffset[(int) (NetFieldBase<int, NetInt>) who.hair % 16]) + 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset)), new Rectangle?(hatSourceRect), Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + num3);
        hatSourceRect = this.hatSourceRect with
        {
          Height = 11
        };
        float num4 = -1E-06f;
        b.Draw(FarmerRenderer.hatsTexture, position + origin + this.positionOffset + new Vector2((float) ((flip ? -1 : 1) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4 - 8), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 - 16 + ((bool) (NetFieldBase<bool, NetBool>) who.hat.Value.ignoreHairstyleOffset ? 0 : FarmerRenderer.hairstyleHatOffset[(int) (NetFieldBase<int, NetInt>) who.hair % 16]) + 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset)), new Rectangle?(hatSourceRect), (bool) (NetFieldBase<bool, NetBool>) who.hat.Value.isPrismatic ? Utility.GetPrismaticColor() : Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + num4);
      }
      else
        b.Draw(FarmerRenderer.hatsTexture, position + origin + this.positionOffset + new Vector2((float) ((flip ? -1 : 1) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4 - 8), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 - 16 + ((bool) (NetFieldBase<bool, NetBool>) who.hat.Value.ignoreHairstyleOffset ? 0 : FarmerRenderer.hairstyleHatOffset[(int) (NetFieldBase<int, NetInt>) who.hair % 16]) + 4 + (int) (NetFieldBase<int, NetInt>) this.heightOffset)), new Rectangle?(this.hatSourceRect), (bool) (NetFieldBase<bool, NetBool>) who.hat.Value.isPrismatic ? Utility.GetPrismaticColor() : Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + num3);
    }

    public void draw(
      SpriteBatch b,
      FarmerSprite.AnimationFrame animationFrame,
      int currentFrame,
      Rectangle sourceRect,
      Vector2 position,
      Vector2 origin,
      float layerDepth,
      int facingDirection,
      Color overrideColor,
      float rotation,
      float scale,
      Farmer who)
    {
      bool flag = currentFrame == 104 || currentFrame == 105;
      if (this._sickFrame != flag)
      {
        this._sickFrame = flag;
        this._shirtDirty = true;
        this._spriteDirty = true;
      }
      this.executeRecolorActions(who);
      position = new Vector2((float) Math.Floor((double) position.X), (float) Math.Floor((double) position.Y));
      this.rotationAdjustment = Vector2.Zero;
      this.positionOffset.Y = (float) (animationFrame.positionOffset * 4);
      this.positionOffset.X = (float) (animationFrame.xOffset * 4);
      if (!FarmerRenderer.isDrawingForUI && (bool) (NetFieldBase<bool, NetBool>) who.swimming)
      {
        sourceRect.Height /= 2;
        sourceRect.Height -= (int) who.yOffset / 4;
        position.Y += 64f;
      }
      if (facingDirection == 3 || facingDirection == 1)
        facingDirection = animationFrame.flip ? 3 : 1;
      b.Draw(this.baseTexture, position + origin + this.positionOffset, new Rectangle?(sourceRect), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
      if (!FarmerRenderer.isDrawingForUI && (bool) (NetFieldBase<bool, NetBool>) who.swimming)
      {
        if (who.currentEyes != 0 && who.FacingDirection != 0 && (Game1.timeOfDay < 2600 || who.isInBed.Value && who.timeWentToBed.Value != 0) && (!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool || who.UsingTool && who.CurrentTool is FishingRod))
        {
          b.Draw(this.baseTexture, position + origin + this.positionOffset + new Vector2((float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4 + 20 + (who.FacingDirection == 1 ? 12 : (who.FacingDirection == 3 ? 4 : 0))), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 40)), new Rectangle?(new Rectangle(5, 16, who.FacingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 5E-08f);
          b.Draw(this.baseTexture, position + origin + this.positionOffset + new Vector2((float) (FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4 + 20 + (who.FacingDirection == 1 ? 12 : (who.FacingDirection == 3 ? 4 : 0))), (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 40)), new Rectangle?(new Rectangle(264 + (who.FacingDirection == 3 ? 4 : 0), 2 + (who.currentEyes - 1) * 2, who.FacingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.2E-07f);
        }
        this.drawHairAndAccesories(b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
        b.Draw(Game1.staminaRect, new Rectangle((int) position.X + (int) who.yOffset + 8, (int) position.Y - 128 + sourceRect.Height * 4 + (int) origin.Y - (int) who.yOffset, sourceRect.Width * 4 - (int) who.yOffset * 2 - 16, 4), new Rectangle?(Game1.staminaRect.Bounds), Color.White * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth + 1f / 1000f);
      }
      else
      {
        Rectangle rectangle = new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
        rectangle.X += this.ClampPants(who.GetPantsIndex()) % 10 * 192;
        rectangle.Y += this.ClampPants(who.GetPantsIndex()) / 10 * 688;
        if (!who.IsMale)
          rectangle.X += 96;
        b.Draw(FarmerRenderer.pantsTexture, position + origin + this.positionOffset, new Rectangle?(rectangle), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (who.FarmerSprite.CurrentAnimationFrame.frame == 5 ? 0.00092f : 9.2E-08f));
        sourceRect.Offset(288, 0);
        if (who.currentEyes != 0 && facingDirection != 0 && (Game1.timeOfDay < 2600 || who.isInBed.Value && who.timeWentToBed.Value != 0) && (!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool || who.UsingTool && who.CurrentTool is FishingRod) && (!who.UsingTool || !(who.CurrentTool is FishingRod currentTool1) || currentTool1.isFishing))
        {
          int num1 = 5;
          int num2 = animationFrame.flip ? num1 - FarmerRenderer.featureXOffsetPerFrame[currentFrame] : num1 + FarmerRenderer.featureXOffsetPerFrame[currentFrame];
          switch (facingDirection)
          {
            case 1:
              num2 += 3;
              break;
            case 3:
              ++num2;
              break;
          }
          int x = num2 * 4;
          b.Draw(this.baseTexture, position + origin + this.positionOffset + new Vector2((float) x, (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (!who.IsMale || who.FacingDirection == 2 ? 40 : 36))), new Rectangle?(new Rectangle(5, 16, facingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 5E-08f);
          b.Draw(this.baseTexture, position + origin + this.positionOffset + new Vector2((float) x, (float) (FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (who.FacingDirection == 1 || who.FacingDirection == 3 ? 40 : 44))), new Rectangle?(new Rectangle(264 + (facingDirection == 3 ? 4 : 0), 2 + (who.currentEyes - 1) * 2, facingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.2E-07f);
        }
        this.drawHairAndAccesories(b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
        float num3 = 4.9E-05f;
        if (facingDirection == 0)
          num3 = -1E-07f;
        sourceRect.Offset((animationFrame.secondaryArm ? 192 : 96) - 288, 0);
        b.Draw(this.baseTexture, position + origin + this.positionOffset + who.armOffset, new Rectangle?(sourceRect), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + num3);
        if (!who.usingSlingshot || !(who.CurrentTool is Slingshot))
          return;
        Slingshot currentTool2 = who.CurrentTool as Slingshot;
        Point point = Utility.Vector2ToPoint(currentTool2.AdjustForHeight(Utility.PointToVector2(currentTool2.aimPos.Value)));
        int x1 = point.X;
        int y = point.Y;
        int backArmDistance = currentTool2.GetBackArmDistance(who);
        Vector2 shootOrigin = currentTool2.GetShootOrigin(who);
        float rotation1 = (float) Math.Atan2((double) y - (double) shootOrigin.Y, (double) x1 - (double) shootOrigin.X) + 3.141593f;
        if (!Game1.options.useLegacySlingshotFiring)
        {
          rotation1 -= 3.141593f;
          if ((double) rotation1 < 0.0)
            rotation1 += 6.283185f;
        }
        switch (facingDirection)
        {
          case 0:
            b.Draw(this.baseTexture, position + new Vector2((float) (4.0 + (double) rotation1 * 8.0), -44f), new Rectangle?(new Rectangle(173, 238, 9, 14)), Color.White, 0.0f, new Vector2(4f, 11f), 4f * scale, SpriteEffects.None, layerDepth + (facingDirection != 0 ? 5.9E-05f : -0.0005f));
            break;
          case 1:
            b.Draw(this.baseTexture, position + new Vector2((float) (52 - backArmDistance), -32f), new Rectangle?(new Rectangle(147, 237, 10, 4)), Color.White, 0.0f, new Vector2(8f, 3f), 4f * scale, SpriteEffects.None, layerDepth + (facingDirection != 0 ? 5.9E-05f : 0.0f));
            b.Draw(this.baseTexture, position + new Vector2(36f, -44f), new Rectangle?(new Rectangle(156, 244, 9, 10)), Color.White, rotation1, new Vector2(0.0f, 3f), 4f * scale, SpriteEffects.None, layerDepth + (facingDirection != 0 ? 1E-08f : 0.0f));
            int num4 = (int) (Math.Cos((double) rotation1 + 1.57079637050629) * (double) (20 - backArmDistance - 8) - Math.Sin((double) rotation1 + 1.57079637050629) * -68.0);
            int num5 = (int) (Math.Sin((double) rotation1 + 1.57079637050629) * (double) (20 - backArmDistance - 8) + Math.Cos((double) rotation1 + 1.57079637050629) * -68.0);
            Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 52.0 - (double) backArmDistance), (int) ((double) position.Y - 32.0 - 4.0), (int) ((double) position.X + 32.0 + (double) (num4 / 2)), (int) ((double) position.Y - 32.0 - 12.0 + (double) (num5 / 2)), b, Color.White);
            break;
          case 2:
            b.Draw(this.baseTexture, position + new Vector2(4f, (float) (-32 - backArmDistance / 2)), new Rectangle?(new Rectangle(148, 244, 4, 4)), Color.White, 0.0f, Vector2.Zero, 4f * scale, SpriteEffects.None, layerDepth + (facingDirection != 0 ? 5.9E-05f : 0.0f));
            Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 16.0), (int) ((double) position.Y - 28.0 - (double) (backArmDistance / 2)), (int) ((double) position.X + 44.0 - (double) rotation1 * 10.0), (int) ((double) position.Y - 16.0 - 8.0), b, Color.White);
            Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 16.0), (int) ((double) position.Y - 28.0 - (double) (backArmDistance / 2)), (int) ((double) position.X + 56.0 - (double) rotation1 * 10.0), (int) ((double) position.Y - 16.0 - 8.0), b, Color.White);
            b.Draw(this.baseTexture, position + new Vector2((float) (44.0 - (double) rotation1 * 10.0), -16f), new Rectangle?(new Rectangle(167, 235, 7, 9)), Color.White, 0.0f, new Vector2(3f, 5f), 4f * scale, SpriteEffects.None, layerDepth + (facingDirection != 0 ? 5.9E-05f : 0.0f));
            break;
          case 3:
            b.Draw(this.baseTexture, position + new Vector2((float) (40 + backArmDistance), -32f), new Rectangle?(new Rectangle(147, 237, 10, 4)), Color.White, 0.0f, new Vector2(9f, 4f), 4f * scale, SpriteEffects.FlipHorizontally, layerDepth + (facingDirection != 0 ? 5.9E-05f : 0.0f));
            b.Draw(this.baseTexture, position + new Vector2(24f, -40f), new Rectangle?(new Rectangle(156, 244, 9, 10)), Color.White, rotation1 + 3.141593f, new Vector2(8f, 3f), 4f * scale, SpriteEffects.FlipHorizontally, layerDepth + (facingDirection != 0 ? 1E-08f : 0.0f));
            int num6 = (int) (Math.Cos((double) rotation1 + 1.25663709640503) * (double) (20 + backArmDistance - 8) - Math.Sin((double) rotation1 + 1.25663709640503) * -68.0);
            int num7 = (int) (Math.Sin((double) rotation1 + 1.25663709640503) * (double) (20 + backArmDistance - 8) + Math.Cos((double) rotation1 + 1.25663709640503) * -68.0);
            Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 4.0 + (double) backArmDistance), (int) ((double) position.Y - 32.0 - 8.0), (int) ((double) position.X + 26.0 + (double) num6 * 4.0 / 10.0), (int) ((double) position.Y - 32.0 - 8.0 + (double) num7 * 4.0 / 10.0), b, Color.White);
            break;
        }
      }
    }
  }
}
