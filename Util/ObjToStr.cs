// Decompiled with JetBrains decompiler
// Type: ObjToStr
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Sickhead.Engine.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public static class ObjToStr
{
  private static readonly StringBuilder _stringBuilder = new StringBuilder();
  private static readonly Dictionary<Type, ObjToStr.ToStringDescription> _cache = new Dictionary<Type, ObjToStr.ToStringDescription>();

  public static string Format(object obj, ObjToStr.Style style)
  {
    Type type1 = obj.GetType();
    ObjToStr._cache.Clear();
    ObjToStr.ToStringDescription stringDescription;
    if (!ObjToStr._cache.TryGetValue(obj.GetType(), out stringDescription))
    {
      stringDescription = new ObjToStr.ToStringDescription()
      {
        Type = type1,
        Members = new List<ObjToStr.ToStringMember>()
      };
      ObjToStr._cache.Add(type1, stringDescription);
      BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      foreach (FieldInfo field in type1.GetFields(bindingAttr))
      {
        ObjToStr.ToStringMember toStringMember = new ObjToStr.ToStringMember()
        {
          Member = (MemberInfo) field,
          Name = field.Name
        };
        Type dataType = field.GetDataType();
        if (dataType == typeof (string))
          toStringMember.Format = "\"{0}\"";
        int num = field.GetDataType().IsArray ? 1 : 0;
        if (dataType.HasElementType)
          toStringMember.Format = "{1}[{2}] {0}";
        stringDescription.Members.Add(toStringMember);
      }
      stringDescription.Members.Sort(new Comparison<ObjToStr.ToStringMember>(ObjToStr.CompareToStringMembers));
    }
    lock (ObjToStr._stringBuilder)
    {
      ObjToStr._stringBuilder.Clear();
      if (style.ShowRootObjectType)
      {
        ObjToStr._stringBuilder.Append(stringDescription.Type.Name);
        ObjToStr._stringBuilder.Append(style.ObjectDelimiter);
      }
      for (int index = 0; index < stringDescription.Members.Count; ++index)
      {
        ObjToStr.ToStringMember member = stringDescription.Members[index];
        Type dataType = member.Member.GetDataType();
        object obj1 = member.Member.GetValue(obj);
        ObjToStr._stringBuilder.Append(dataType.Name);
        ObjToStr._stringBuilder.Append(" ");
        ObjToStr._stringBuilder.Append(member.Name);
        ObjToStr._stringBuilder.Append(style.MemberNameValueDelimiter);
        if (obj1 == null)
        {
          ObjToStr._stringBuilder.Append("null");
        }
        else
        {
          Type type2 = obj1.GetType();
          if (type2.HasElementType)
          {
            Type elementType = type2.GetElementType();
            string str = "?";
            ObjToStr._stringBuilder.AppendFormat(member.Format, obj1, (object) elementType, (object) str);
          }
          else
            ObjToStr._stringBuilder.AppendFormat(member.Format, obj1);
        }
        if (index != stringDescription.Members.Count - 1)
          ObjToStr._stringBuilder.Append(style.MemberDelimiter);
      }
      return ObjToStr._stringBuilder.ToString();
    }
  }

  private static int CompareToStringMembers(ObjToStr.ToStringMember a, ObjToStr.ToStringMember b) => a.Name.CompareTo(b.Name);

  private struct ToStringDescription
  {
    public Type Type;
    public List<ObjToStr.ToStringMember> Members;
  }

  private struct ToStringMember
  {
    public MemberInfo Member;
    private string _name;
    private string _format;

    public string Name
    {
      get => !string.IsNullOrEmpty(this._name) ? this._name : this.Member.Name;
      set => this._name = value;
    }

    public string Format
    {
      get => !string.IsNullOrEmpty(this._format) ? this._format : "{0}";
      set => this._format = value;
    }
  }

  public class Style
  {
    public bool ShowRootObjectType;
    public string ObjectDelimiter;
    public string MemberDelimiter;
    public string MemberNameValueDelimiter;
    public bool TrailingNewline;
    public static ObjToStr.Style TypeAndMembersSingleLine = new ObjToStr.Style()
    {
      ShowRootObjectType = true,
      ObjectDelimiter = ":",
      MemberDelimiter = ",",
      MemberNameValueDelimiter = "="
    };
    public static ObjToStr.Style MembersOnlyMultiline = new ObjToStr.Style()
    {
      ShowRootObjectType = false,
      ObjectDelimiter = "",
      MemberDelimiter = "\n",
      MemberNameValueDelimiter = "="
    };

    public Style()
    {
      this.ShowRootObjectType = true;
      this.ObjectDelimiter = ":";
      this.MemberDelimiter = ",";
      this.MemberNameValueDelimiter = "=";
    }
  }
}
