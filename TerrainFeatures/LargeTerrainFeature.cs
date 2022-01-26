// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.LargeTerrainFeature
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  [XmlInclude(typeof (Bush))]
  public abstract class LargeTerrainFeature : TerrainFeature
  {
    [XmlElement("tilePosition")]
    public readonly NetVector2 tilePosition = new NetVector2();

    protected LargeTerrainFeature(bool needsTick)
      : base(needsTick)
    {
      this.NetFields.AddField((INetSerializable) this.tilePosition);
    }

    public Rectangle getBoundingBox() => this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tilePosition);

    public void dayUpdate(GameLocation l) => this.dayUpdate(l, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tilePosition);

    public bool tickUpdate(GameTime time, GameLocation location) => this.tickUpdate(time, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tilePosition, location);

    public void draw(SpriteBatch b) => this.draw(b, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tilePosition);
  }
}
