// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetAudio
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.IO;

namespace StardewValley.Network
{
  public class NetAudio : INetObject<NetFields>
  {
    private readonly NetEventBinary audioEvent = new NetEventBinary();
    private readonly NetStringDictionary<bool, NetBool> activeCues = new NetStringDictionary<bool, NetBool>();
    private GameLocation location;

    public NetFields NetFields { get; } = new NetFields();

    public NetDictionary<string, bool, NetBool, SerializableDictionary<string, bool>, NetStringDictionary<bool, NetBool>>.KeysCollection ActiveCues => this.activeCues.Keys;

    public NetAudio(GameLocation location)
    {
      this.location = location;
      this.NetFields.AddFields((INetSerializable) this.audioEvent, (INetSerializable) this.activeCues);
      this.audioEvent.AddReaderHandler(new Action<BinaryReader>(this.handleAudioEvent));
    }

    private void handleAudioEvent(BinaryReader reader) => this.PlayLocalAt(reader.ReadString(), reader.ReadVector2(), reader.ReadInt32(), (NetAudio.SoundContext) reader.ReadInt32());

    public void PlayLocalAt(
      string audioName,
      Vector2 position,
      int pitch = -1,
      NetAudio.SoundContext sound_context = NetAudio.SoundContext.Default)
    {
      if (!this.CanHear(position))
        return;
      this.PlayLocal(audioName, pitch, sound_context);
    }

    public void PlayLocal(string audioName, int pitch = -1, NetAudio.SoundContext sound_context = NetAudio.SoundContext.Default)
    {
      if (Game1.eventUp && sound_context == NetAudio.SoundContext.NPC || Game1.currentLocation != this.location)
        return;
      this._PlayAudio(audioName, pitch);
    }

    protected void _PlayAudio(string audioName, int pitch)
    {
      if (pitch == -1)
        Game1.playSound(audioName);
      else
        Game1.playSoundPitched(audioName, pitch);
    }

    public void Update() => this.audioEvent.Poll();

    public bool CanHear(Vector2 position) => position == Vector2.Zero || Utility.isOnScreen(position * 64f, 384);

    public bool CanShortcutPlay(Vector2 position, NetAudio.SoundContext sound_context)
    {
      if (!LocalMultiplayer.IsLocalMultiplayer(true) || Game1.eventUp && sound_context == NetAudio.SoundContext.NPC)
        return false;
      if ((this.location == null || this.location == Game1.currentLocation) && this.CanHear(position))
        return true;
      bool someone_can_hear = false;
      if (this.location == null)
        return true;
      foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
      {
        if (gameInstance.instanceGameLocation == this.location)
        {
          someone_can_hear = true;
          break;
        }
      }
      if (someone_can_hear && position != Vector2.Zero)
      {
        someone_can_hear = false;
        GameRunner.instance.ExecuteForInstances((Action<Game1>) (instance =>
        {
          if (someone_can_hear || this.location != Game1.currentLocation || !this.CanHear(position))
            return;
          someone_can_hear = true;
        }));
      }
      return someone_can_hear;
    }

    public void Play(string audioName, NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default)
    {
      if (this.CanShortcutPlay(Vector2.Zero, soundContext))
      {
        this._PlayAudio(audioName, -1);
      }
      else
      {
        this.audioEvent.Fire((NetEventBinary.ArgWriter) (writer =>
        {
          writer.Write(audioName);
          writer.WriteVector2(Vector2.Zero);
          writer.Write(-1);
          writer.Write((int) soundContext);
        }));
        this.audioEvent.Poll();
      }
    }

    public void PlayAt(string audioName, Vector2 position, NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default)
    {
      if (this.CanShortcutPlay(position, soundContext))
      {
        this._PlayAudio(audioName, -1);
      }
      else
      {
        this.audioEvent.Fire((NetEventBinary.ArgWriter) (writer =>
        {
          writer.Write(audioName);
          writer.WriteVector2(position);
          writer.Write(-1);
          writer.Write((int) soundContext);
        }));
        this.audioEvent.Poll();
      }
    }

    public void PlayPitched(
      string audioName,
      Vector2 position,
      int pitch,
      NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default)
    {
      if (this.CanShortcutPlay(Vector2.Zero, soundContext))
      {
        this._PlayAudio(audioName, pitch);
      }
      else
      {
        this.audioEvent.Fire((NetEventBinary.ArgWriter) (writer =>
        {
          writer.Write(audioName);
          writer.WriteVector2(position);
          writer.Write(pitch);
          writer.Write((int) soundContext);
        }));
        this.audioEvent.Poll();
      }
    }

    public void StartPlaying(string cueName) => this.activeCues[cueName] = false;

    public void StopPlaying(string cueName) => this.activeCues.Remove(cueName);

    public enum SoundContext
    {
      Default,
      NPC,
    }
  }
}
