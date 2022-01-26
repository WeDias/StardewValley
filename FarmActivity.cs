// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmActivity
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class FarmActivity
  {
    protected NPC _character;
    public Vector2 activityPosition;
    public int activityDirection = 2;
    public float weight = 1f;
    protected float _age;
    protected bool _performingActivity;

    public virtual FarmActivity Initialize(NPC character, float activity_weight = 1f)
    {
      this._character = character;
      this.weight = activity_weight;
      return this;
    }

    public virtual bool AttemptActivity(Farm farm) => this._AttemptActivity(farm);

    protected virtual bool _AttemptActivity(Farm farm) => false;

    public bool Update(GameTime time)
    {
      this._age += (float) time.ElapsedGameTime.TotalSeconds;
      return this._Update(time);
    }

    protected virtual bool _Update(GameTime time) => (double) this._age >= 10.0;

    public bool IsPerformingActivity() => this._performingActivity;

    public void BeginActivity()
    {
      this._character.faceDirection(this.activityDirection);
      this._age = 0.0f;
      this._performingActivity = true;
      this._BeginActivity();
    }

    protected virtual void _BeginActivity()
    {
    }

    public void EndActivity()
    {
      this._performingActivity = false;
      this._EndActivity();
    }

    protected virtual void _EndActivity()
    {
    }

    public virtual bool IsTileBlockedFromSight(Vector2 tile) => false;

    public Rectangle GetFarmBounds(Farm farm) => new Rectangle(0, 0, farm.map.Layers[0].LayerWidth, farm.map.Layers[0].LayerHeight);

    public Object GetRandomObject(Farm farm, Func<Object, bool> validator = null)
    {
      List<Object> list = new List<Object>();
      foreach (Vector2 key in farm.objects.Keys)
      {
        Object @object = farm.objects[key];
        if (@object != null && (validator == null || validator(@object)))
          list.Add(@object);
      }
      return Utility.GetRandom<Object>(list);
    }

    public TerrainFeature GetRandomTerrainFeature(
      Farm farm,
      Func<TerrainFeature, bool> validator = null)
    {
      List<TerrainFeature> list = new List<TerrainFeature>();
      foreach (Vector2 key in farm.terrainFeatures.Keys)
      {
        TerrainFeature terrainFeature = farm.terrainFeatures[key];
        if (terrainFeature != null && (validator == null || validator(terrainFeature)))
          list.Add(terrainFeature);
      }
      return Utility.GetRandom<TerrainFeature>(list);
    }

    public HoeDirt GetRandomCrop(Farm farm, Func<Crop, bool> validator = null)
    {
      List<HoeDirt> list = new List<HoeDirt>();
      foreach (Vector2 key in farm.terrainFeatures.Keys)
      {
        if (farm.terrainFeatures[key] is HoeDirt terrainFeature && terrainFeature.crop != null && (validator == null || validator(terrainFeature.crop)))
          list.Add(terrainFeature);
      }
      return Utility.GetRandom<HoeDirt>(list);
    }

    public Vector2 GetNearbyTile(Farm farm, Vector2 tile) => Utility.getRandomAdjacentOpenTile(tile, (GameLocation) farm);
  }
}
