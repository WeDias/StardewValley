// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.PerchingBirds
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.BellsAndWhistles
{
  public class PerchingBirds
  {
    public const int BIRD_STARTLE_DISTANCE = 200;
    [XmlIgnore]
    public List<Bird> _birds = new List<Bird>();
    [XmlIgnore]
    protected Point[] _birdLocations;
    protected Point[] _birdRoostLocations;
    [XmlIgnore]
    public Dictionary<Point, Bird> _birdPointOccupancy;
    public bool roosting;
    protected Texture2D _birdSheet;
    protected int _birdWidth;
    protected int _birdHeight;
    protected int _flapFrames = 2;
    protected Vector2 _birdOrigin;
    public int peckDuration = 5;
    public float birdSpeed = 5f;

    public PerchingBirds(
      Texture2D bird_texture,
      int flap_frames,
      int width,
      int height,
      Vector2 origin,
      Point[] perch_locations,
      Point[] roost_locations)
    {
      this._birdSheet = bird_texture;
      this._birdWidth = width;
      this._birdHeight = height;
      this._birdOrigin = origin;
      this._flapFrames = flap_frames;
      this._birdPointOccupancy = new Dictionary<Point, Bird>();
      this._birdLocations = perch_locations;
      this._birdRoostLocations = roost_locations;
      this.ResetLocalState();
    }

    public int GetBirdWidth() => this._birdWidth;

    public int GetBirdHeight() => this._birdHeight;

    public Vector2 GetBirdOrigin() => this._birdOrigin;

    public Texture2D GetTexture() => this._birdSheet;

    public Point GetFreeBirdPoint(Bird bird = null, int clearance = 200)
    {
      List<Point> list = new List<Point>();
      foreach (Point currentBirdLocation in this.GetCurrentBirdLocationList())
      {
        if (this._birdPointOccupancy[currentBirdLocation] == null)
        {
          bool flag = false;
          if (bird != null)
          {
            foreach (Farmer farmer in Game1.currentLocation.farmers)
            {
              if ((double) Utility.distance(farmer.position.X, (float) (currentBirdLocation.X * 64) + 32f, farmer.position.Y, (float) (currentBirdLocation.Y * 64) + 32f) < 200.0)
                flag = true;
            }
          }
          if (!flag)
            list.Add(currentBirdLocation);
        }
      }
      return Utility.GetRandom<Point>(list);
    }

    public void ReserveBirdPoint(Bird bird, Point point)
    {
      if (this._birdPointOccupancy.ContainsKey(bird.endPosition))
        this._birdPointOccupancy[bird.endPosition] = (Bird) null;
      if (!this._birdPointOccupancy.ContainsKey(point))
        return;
      this._birdPointOccupancy[point] = bird;
    }

    public bool ShouldBirdsRoost() => this.roosting;

    public Point[] GetCurrentBirdLocationList() => this.ShouldBirdsRoost() ? this._birdRoostLocations : this._birdLocations;

    public virtual void Update(GameTime time)
    {
      for (int index = 0; index < this._birds.Count; ++index)
        this._birds[index].Update(time);
    }

    public virtual void Draw(SpriteBatch b)
    {
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      for (int index = 0; index < this._birds.Count; ++index)
        this._birds[index].Draw(b);
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }

    public virtual void ResetLocalState()
    {
      this._birds.Clear();
      this._birdPointOccupancy = new Dictionary<Point, Bird>();
      foreach (Point birdLocation in this._birdLocations)
        this._birdPointOccupancy[birdLocation] = (Bird) null;
      foreach (Point birdRoostLocation in this._birdRoostLocations)
        this._birdPointOccupancy[birdRoostLocation] = (Bird) null;
    }

    public virtual void AddBird(int bird_type)
    {
      Bird bird = new Bird(this.GetFreeBirdPoint(), this, bird_type, this._flapFrames);
      this._birds.Add(bird);
      this.ReserveBirdPoint(bird, bird.endPosition);
    }
  }
}
