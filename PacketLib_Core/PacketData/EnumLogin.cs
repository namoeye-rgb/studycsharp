// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Enum_Login.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Packet_Login {

  /// <summary>Holder for reflection information generated from Enum_Login.proto</summary>
  public static partial class EnumLoginReflection {

    #region Descriptor
    /// <summary>File descriptor for Enum_Login.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static EnumLoginReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChBFbnVtX0xvZ2luLnByb3RvEgZwYWNrZXQaH2dvb2dsZS9wcm90b2J1Zi90",
            "aW1lc3RhbXAucHJvdG8qPgoJTG9naW5UeXBlEg4KCkxPR0lOX05PTkUQABIQ",
            "CgxMT0dJTl9HT09HTEUQARIPCgtMT0dJTl9HVUVTVBACQg+qAgxQYWNrZXRf",
            "TG9naW5iBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.TimestampReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Packet_Login.LoginType), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum LoginType {
    [pbr::OriginalName("LOGIN_NONE")] LoginNone = 0,
    [pbr::OriginalName("LOGIN_GOOGLE")] LoginGoogle = 1,
    [pbr::OriginalName("LOGIN_GUEST")] LoginGuest = 2,
  }

  #endregion

}

#endregion Designer generated code
