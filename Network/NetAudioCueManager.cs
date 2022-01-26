// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetAudioCueManager
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using Netcode;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public class NetAudioCueManager
  {
    private Dictionary<string, ICue> playingCues = new Dictionary<string, ICue>();
    private List<string> cuesToStop = new List<string>();

    public virtual void Update(GameLocation currentLocation)
    {
      NetDictionary<string, bool, NetBool, SerializableDictionary<string, bool>, NetStringDictionary<bool, NetBool>>.KeysCollection activeCues = currentLocation.netAudio.ActiveCues;
      foreach (string str in activeCues)
      {
        if (!this.playingCues.ContainsKey(str))
        {
          this.playingCues[str] = Game1.soundBank.GetCue(str);
          this.playingCues[str].Play();
        }
      }
      foreach (KeyValuePair<string, ICue> playingCue in this.playingCues)
      {
        string key = playingCue.Key;
        if (!activeCues.Contains(key))
          this.cuesToStop.Add(key);
      }
      foreach (string key in this.cuesToStop)
      {
        this.playingCues[key].Stop(AudioStopOptions.AsAuthored);
        this.playingCues.Remove(key);
      }
      this.cuesToStop.Clear();
    }

    public void StopAll()
    {
      foreach (ICue cue in this.playingCues.Values)
        cue.Stop(AudioStopOptions.Immediate);
      this.playingCues.Clear();
    }
  }
}
