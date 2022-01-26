// Decompiled with JetBrains decompiler
// Type: StardewValley.LightSource
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;

namespace StardewValley
{
  public class LightSource : INetObject<NetFields>
  {
    public const int lantern = 1;
    public const int windowLight = 2;
    public const int sconceLight = 4;
    public const int cauldronLight = 5;
    public const int indoorWindowLight = 6;
    public const int projectorLight = 7;
    public const int fishTankLight = 8;
    public const int maxLightsOnScreenBeforeReduction = 8;
    public const float reductionPerExtraLightSource = 0.03f;
    public const int playerLantern = -85736;
    public readonly NetInt textureIndex = new NetInt().Interpolated(false, false);
    public Texture2D lightTexture;
    public readonly NetVector2 position = new NetVector2().Interpolated(true, true);
    public readonly NetColor color = new NetColor();
    public readonly NetFloat radius = new NetFloat();
    public readonly NetInt identifier = new NetInt();
    public readonly NetEnum<LightSource.LightContext> lightContext = new NetEnum<LightSource.LightContext>();
    public readonly NetLong playerID = new NetLong(0L).Interpolated(false, false);

    public int Identifier
    {
      get => this.identifier.Value;
      set => this.identifier.Value = value;
    }

    public long PlayerID
    {
      get => this.playerID.Value;
      set => this.playerID.Value = value;
    }

    public NetFields NetFields { get; } = new NetFields();

    public LightSource()
    {
      this.NetFields.AddFields((INetSerializable) this.textureIndex, (INetSerializable) this.position, (INetSerializable) this.color, (INetSerializable) this.radius, (INetSerializable) this.identifier, (INetSerializable) this.lightContext, (INetSerializable) this.playerID);
      this.textureIndex.fieldChangeEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, oldValue, newValue) => this.loadTextureFromConstantValue(newValue));
    }

    public LightSource(
      int textureIndex,
      Vector2 position,
      float radius,
      Color color,
      LightSource.LightContext light_context = LightSource.LightContext.None,
      long playerID = 0)
      : this()
    {
      this.textureIndex.Value = textureIndex;
      this.position.Value = position;
      this.radius.Value = radius;
      this.color.Value = color;
      this.lightContext.Value = light_context;
      this.playerID.Value = playerID;
    }

    public LightSource(
      int textureIndex,
      Vector2 position,
      float radius,
      Color color,
      int identifier,
      LightSource.LightContext light_context = LightSource.LightContext.None,
      long playerID = 0)
      : this()
    {
      this.textureIndex.Value = textureIndex;
      this.position.Value = position;
      this.radius.Value = radius;
      this.color.Value = color;
      this.identifier.Value = identifier;
      this.lightContext.Value = light_context;
      this.playerID.Value = playerID;
    }

    public LightSource(
      int textureIndex,
      Vector2 position,
      float radius,
      LightSource.LightContext light_context = LightSource.LightContext.None,
      long playerID = 0)
      : this()
    {
      this.textureIndex.Value = textureIndex;
      this.position.Value = position;
      this.radius.Value = radius;
      this.color.Value = Color.Black;
      this.lightContext.Value = light_context;
      this.playerID.Value = playerID;
    }

    private void loadTextureFromConstantValue(int value)
    {
      switch (value)
      {
        case 1:
          this.lightTexture = Game1.lantern;
          break;
        case 2:
          this.lightTexture = Game1.windowLight;
          break;
        case 4:
          this.lightTexture = Game1.sconceLight;
          break;
        case 5:
          this.lightTexture = Game1.cauldronLight;
          break;
        case 6:
          this.lightTexture = Game1.indoorWindowLight;
          break;
        case 7:
          this.lightTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Lighting\\projectorLight");
          break;
        case 8:
          this.lightTexture = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\fishTankLight");
          break;
      }
    }

    public LightSource Clone() => new LightSource((int) (NetFieldBase<int, NetInt>) this.textureIndex, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.position, (float) (NetFieldBase<float, NetFloat>) this.radius, (Color) (NetFieldBase<Color, NetColor>) this.color, (int) (NetFieldBase<int, NetInt>) this.identifier, this.lightContext.Value, this.playerID.Value);

    public enum LightContext
    {
      None,
      MapLight,
      WindowLight,
    }
  }
}
