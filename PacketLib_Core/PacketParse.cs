using CommonLib_Core;
using Google.Protobuf;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UtilLib;

namespace PacketLib_Core
{
    public class PacketConst
    {
        public const byte HEADER_SIZE = 6;
        public const byte ID_SIZE = 2;
        public const int BODY_SIZE = 4;
    }

    public class PacketHandler
    {
        private static Dictionary<short, Type> packetIDTypeMap = new Dictionary<short, Type>();
        private static Dictionary<Type, short> packetTypeIDMap = new Dictionary<Type, short>();
        private static Dictionary<short, MethodInfo> handlerFuncMap = new Dictionary<short, MethodInfo>();

        private ILogger logger;

        public void Initialize(ILogger logger, Assembly excuteAssembly, string packetHandlerClassName)
        {
            if (packetHandlerClassName.Length <= 0)
            {
                logger.Error("not found packetHandlerFuncName");
                return;
            }

            var assembles = Assembly.GetExecutingAssembly().GetTypes();
            var classTypes = assembles.Where(
                t => t.IsClass == true
                && t.IsPublic == true
               && t.IsSealed == true
               && t.Namespace.ToLower().Contains("packet")
               && (t.Name.ToLower().Contains("cs") || t.Name.ToLower().Contains("sc")));

            var enumFields = assembles
                .Where(t => t.IsEnum == true)
                .Where(t => t.Name.ToLower().Contains("packetid")).Select(s => s.GetFields()).First();
            var enumTypeMap = enumFields.Skip(1).ToDictionary(s => s.Name);

            foreach (var classType in classTypes)
            {
                if (enumTypeMap.ContainsKey(classType.Name) == false)
                {
                    logger.Error($"not found assembly classType not match enumtype, className : {classType.Name}");
                    continue;
                }

                var enumVal = enumTypeMap[classType.Name];
                var id = (short)Enum.Parse(enumVal.FieldType, enumVal.Name);

                packetIDTypeMap.Add(id, classType);
                packetTypeIDMap.Add(classType, id);
            }

            //HandlerMethods Find
            var currentAssemblies = excuteAssembly.GetTypes();
            var handlerMethods = currentAssemblies
                .Where(x => x.IsClass == true && x.Name.ToLower() == packetHandlerClassName.ToLower())
                .Select(s => s.GetMethods(BindingFlags.Public | BindingFlags.Static)).First();

            foreach (var method in handlerMethods)
            {
                var methodParams = method.GetParameters();

                foreach (var param in methodParams)
                {
                    var t = param.ParameterType;
                    if (t.IsInterface == true)
                    {
                        continue;
                    }

                    if (packetTypeIDMap.ContainsKey(t) == false)
                    {
                        logger.Error($"not found PacketTypeIDMap, type : {t}");
                        continue;
                    }
                    handlerFuncMap.Add(packetTypeIDMap[t], method);
                }
            }
        }

        private static short GetIDFromType(Type packetType)
        {
            if (packetTypeIDMap.ContainsKey(packetType) == false)
            {
                return 0;
            }

            return packetTypeIDMap[packetType];
        }

        public void CallPacketHandlerMethod(INetSession session, short id, byte[] buffer)
        {
            if (handlerFuncMap.ContainsKey(id) == false)
            {
                logger.Error($"not found handlerFuncMap id : {id}");
                return;
            }

            var type = packetIDTypeMap[id];

            handlerFuncMap[id].Invoke(null, new object[]
            {
                session,
                buffer
            });
        }

        public static byte[] GetPacketToBytes<T>(T packet) where T : IMessage
        {
            var t = packet.GetType();
            var id = GetIDFromType(t);
            var bodyBuffer = packet.ToByteArray();

            var idBuffer = BitConverter.GetBytes(id);
            var bodySizeBuffer = BitConverter.GetBytes(bodyBuffer.Length);

            var packetBuffer = new byte[PacketConst.HEADER_SIZE + bodyBuffer.Length];
            Array.Copy(idBuffer, 0, packetBuffer, 0, idBuffer.Length);
            Array.Copy(bodySizeBuffer, 0, packetBuffer, PacketConst.ID_SIZE, bodySizeBuffer.Length);
            Array.Copy(bodyBuffer, 0, packetBuffer, PacketConst.HEADER_SIZE, bodyBuffer.Length);

            return packetBuffer;
        }
    }


}
