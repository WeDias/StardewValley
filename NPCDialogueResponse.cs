// Decompiled with JetBrains decompiler
// Type: StardewValley.NPCDialogueResponse
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley
{
  public class NPCDialogueResponse : Response
  {
    public int friendshipChange;
    public int id;

    public NPCDialogueResponse(
      int id,
      int friendshipChange,
      string keyToNPCresponse,
      string responseText)
      : base(keyToNPCresponse, responseText)
    {
      this.friendshipChange = friendshipChange;
      this.id = id;
    }
  }
}
