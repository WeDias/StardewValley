// Decompiled with JetBrains decompiler
// Type: StardewValley.InteriorDoor
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System.IO;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley
{
  public class InteriorDoor : NetField<bool, InteriorDoor>
  {
    public GameLocation Location;
    public Point Position;
    public TemporaryAnimatedSprite Sprite;
    public Tile Tile;

    public InteriorDoor()
    {
    }

    public InteriorDoor(GameLocation location, Point position)
      : this()
    {
      this.Location = location;
      this.Position = position;
    }

    public override void Set(bool newValue)
    {
      if (newValue == this.value)
        return;
      this.cleanSet(newValue);
      this.MarkDirty();
    }

    protected override void ReadDelta(BinaryReader reader, NetVersion version)
    {
      bool newValue = reader.ReadBoolean();
      if (!version.IsPriorityOver(this.ChangeVersion))
        return;
      this.setInterpolationTarget(newValue);
    }

    protected override void WriteDelta(BinaryWriter writer) => writer.Write(this.targetValue);

    public void ResetLocalState()
    {
      int x = this.Position.X;
      int y = this.Position.Y;
      xTile.Dimensions.Location location = new xTile.Dimensions.Location(x, y);
      Layer layer1 = this.Location.Map.GetLayer("Buildings");
      Layer layer2 = this.Location.Map.GetLayer("Back");
      if (this.Tile == null)
        this.Tile = layer1.Tiles[location];
      if (this.Tile == null)
        return;
      PropertyValue propertyValue;
      if (this.Tile.Properties.TryGetValue("Action", out propertyValue) && propertyValue != null && propertyValue.ToString().Contains("Door") && propertyValue.ToString().Split(' ').Length > 1 && layer2.Tiles[location] != null && !layer2.Tiles[location].Properties.ContainsKey("TouchAction"))
        layer2.Tiles[location].Properties.Add("TouchAction", new PropertyValue("Door " + propertyValue.ToString().Substring(propertyValue.ToString().IndexOf(' ') + 1)));
      Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle();
      bool flipped = false;
      switch (this.Tile.TileIndex)
      {
        case 120:
          sourceRect = new Microsoft.Xna.Framework.Rectangle(512, 144, 16, 48);
          break;
        case 824:
          sourceRect = new Microsoft.Xna.Framework.Rectangle(640, 144, 16, 48);
          break;
        case 825:
          sourceRect = new Microsoft.Xna.Framework.Rectangle(640, 144, 16, 48);
          flipped = true;
          break;
        case 838:
          sourceRect = new Microsoft.Xna.Framework.Rectangle(576, 144, 16, 48);
          if (x == 10 && y == 5)
          {
            flipped = true;
            break;
          }
          break;
      }
      this.Sprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 100f, 4, 1, new Vector2((float) x, (float) (y - 2)) * 64f, false, flipped, (float) ((y + 1) * 64 - 12) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        holdLastFrame = true,
        paused = true
      };
      if (!this.Value)
        return;
      this.Sprite.paused = false;
      this.Sprite.resetEnd();
    }

    public virtual void ApplyMapModifications()
    {
      if (this.Value)
        this.openDoorTiles();
      else
        this.closeDoorTiles();
    }

    public void CleanUpLocalState() => this.closeDoorTiles();

    private void closeDoorSprite()
    {
      this.Sprite.reset();
      this.Sprite.paused = true;
    }

    private void openDoorSprite() => this.Sprite.paused = false;

    private void openDoorTiles()
    {
      this.Location.setTileProperty(this.Position.X, this.Position.Y, "Back", "TemporaryBarrier", "T");
      this.Location.removeTile(this.Position.X, this.Position.Y, "Buildings");
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => this.Location.removeTileProperty(this.Position.X, this.Position.Y, "Back", "TemporaryBarrier")), 400);
      this.Location.removeTile(this.Position.X, this.Position.Y - 1, "Front");
      this.Location.removeTile(this.Position.X, this.Position.Y - 2, "Front");
    }

    private void closeDoorTiles()
    {
      xTile.Dimensions.Location location = new xTile.Dimensions.Location(this.Position.X, this.Position.Y);
      Map map = this.Location.Map;
      if (map == null || this.Tile == null)
        return;
      map.GetLayer("Buildings").Tiles[location] = this.Tile;
      this.Location.removeTileProperty(this.Position.X, this.Position.Y, "Back", "TemporaryBarrier");
      --location.Y;
      map.GetLayer("Front").Tiles[location] = (Tile) new StaticTile(map.GetLayer("Front"), this.Tile.TileSheet, BlendMode.Alpha, this.Tile.TileIndex - this.Tile.TileSheet.SheetWidth);
      --location.Y;
      map.GetLayer("Front").Tiles[location] = (Tile) new StaticTile(map.GetLayer("Front"), this.Tile.TileSheet, BlendMode.Alpha, this.Tile.TileIndex - this.Tile.TileSheet.SheetWidth * 2);
    }

    public void Update(GameTime time)
    {
      if (this.Sprite == null)
        return;
      if (this.Value && this.Sprite.paused)
      {
        this.openDoorSprite();
        this.openDoorTiles();
      }
      else if (!this.Value && !this.Sprite.paused)
      {
        this.closeDoorSprite();
        this.closeDoorTiles();
      }
      this.Sprite.update(time);
    }

    public void Draw(SpriteBatch b)
    {
      if (this.Sprite == null)
        return;
      this.Sprite.draw(b);
    }
  }
}
