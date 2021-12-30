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
        public Dictionary<short, Type> PacketIDTypeMap = new Dictionary<short, Type>();
        public Dictionary<Type, short> PacketTypeIDMap = new Dictionary<Type, short>();

        public void Initialize(ILogger logger)
        {
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

                PacketIDTypeMap.Add(id, classType);
            }
        }
    }


}
