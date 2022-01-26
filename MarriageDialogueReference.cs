// Decompiled with JetBrains decompiler
// Type: StardewValley.MarriageDialogueReference
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley
{
  public class MarriageDialogueReference : 
    INetObject<NetFields>,
    IEquatable<MarriageDialogueReference>
  {
    public const string ENDEARMENT_TOKEN = "%endearment";
    public const string ENDEARMENT_TOKEN_LOWER = "%endearmentlower";
    private readonly NetString _dialogueFile = new NetString("");
    private readonly NetString _dialogueKey = new NetString("");
    private readonly NetBool _isGendered = new NetBool(false);
    private readonly NetStringList _substitutions = new NetStringList();

    public NetFields NetFields { get; } = new NetFields();

    public MarriageDialogueReference() => this.NetFields.AddFields((INetSerializable) this._dialogueFile, (INetSerializable) this._dialogueKey, (INetSerializable) this._isGendered, (INetSerializable) this._substitutions);

    public MarriageDialogueReference(
      string dialogue_file,
      string dialogue_key,
      bool gendered = false,
      params string[] substitutions)
    {
      this._dialogueFile.Value = dialogue_file;
      this._dialogueKey.Value = dialogue_key;
      this._isGendered.Value = (bool) (NetFieldBase<bool, NetBool>) this._isGendered;
      if (substitutions.Length != 0)
        this._substitutions.AddRange((IEnumerable<string>) substitutions);
      this.NetFields.AddFields((INetSerializable) this._dialogueFile, (INetSerializable) this._dialogueKey, (INetSerializable) this._isGendered, (INetSerializable) this._substitutions);
    }

    public string GetText() => "";

    public bool IsItemGrabDialogue(NPC n) => this.GetDialogue(n).isItemGrabDialogue();

    protected string _ReplaceTokens(string text, NPC n)
    {
      text = text.Replace("%endearmentlower", n.getTermOfSpousalEndearment().ToLower());
      text = text.Replace("%endearment", n.getTermOfSpousalEndearment());
      return text;
    }

    public Dialogue GetDialogue(NPC n)
    {
      if (this._dialogueFile.Value.Contains("Marriage"))
        return new Dialogue(this._ReplaceTokens(n.tryToGetMarriageSpecificDialogueElseReturnDefault(this._dialogueKey.Value), n), n)
        {
          removeOnNextMove = true
        };
      if (this._isGendered.Value)
        return new Dialogue(this._ReplaceTokens(Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) n.gender, this._dialogueFile.Value + ":" + this._dialogueKey.Value, (object) this._substitutions), n), n)
        {
          removeOnNextMove = true
        };
      return new Dialogue(this._ReplaceTokens(Game1.content.LoadString(this._dialogueFile.Value + ":" + this._dialogueKey.Value, (object) this._substitutions), n), n)
      {
        removeOnNextMove = true
      };
    }

    public string DialogueFile => this._dialogueFile.Value;

    public string DialogueKey => this._dialogueKey.Value;

    public bool IsGendered => this._isGendered.Value;

    public string[] Substitutions => this._substitutions.ToArray<string>();

    public bool Equals(MarriageDialogueReference other) => object.Equals((object) this._dialogueFile.Value, (object) other._dialogueFile.Value) && object.Equals((object) this._dialogueKey.Value, (object) other._dialogueKey.Value) && object.Equals((object) this._isGendered.Value, (object) other._isGendered.Value) && this._substitutions.SequenceEqual<string>((IEnumerable<string>) other._substitutions);

    public override bool Equals(object obj) => obj is MarriageDialogueReference && this.Equals(obj as MarriageDialogueReference);

    public override int GetHashCode()
    {
      int hashCode = ((13 * 7 + (this._dialogueFile.Value == null ? 0 : this._dialogueFile.Value.GetHashCode())) * 7 + (this._dialogueKey.Value == null ? 0 : this._dialogueFile.Value.GetHashCode())) * 7 + (this._isGendered.Value ? 0 : 1);
      foreach (string substitution in (NetList<string, NetString>) this._substitutions)
        hashCode = hashCode * 7 + substitution.GetHashCode();
      return hashCode;
    }
  }
}
