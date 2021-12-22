using System;
using System.Runtime.Serialization;

namespace EntityService {
    public enum ValueType {
        Invalid,
        Number,
        String,
        // ObjectType - Composite
        IntRange,
        DoubleRange,
        Vector2,
        Vector3,
        // ObjectType - Array
        StringArray,
        IntArray,
        FloatArray,
        DoubleArray,
        Vector2Array,
        Vector3Array,
    }

    [Serializable]
    public struct ESProperty : ISerializable {
        public readonly ValueType Type;
        private readonly float numberValue;
        private readonly string stringValue;

        public static readonly ESProperty Empty = new ESProperty();
        public static bool IsEmpty(ESProperty property)
        {
            return property.Type == ValueType.Invalid;
        }

        public ESProperty(ValueType type = ValueType.Invalid)
        {
            Type = type;
            numberValue = 0;
            stringValue = "";
        }

        public ESProperty(bool rawValue)
        {
            Type = ValueType.String;
            numberValue = 0;
            stringValue = rawValue.ToString();
        }

        public ESProperty(int rawValue)
        {
            numberValue = rawValue;
            stringValue = null;
            Type = ValueType.Number;
        }

        public ESProperty(float rawValue)
        {
            numberValue = rawValue;
            stringValue = null;
            Type = ValueType.String;
        }
        public ESProperty(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue)) {
                throw new NullReferenceException();
            }

            if (float.TryParse(rawValue, out float outNumber)) {
                numberValue = outNumber;
                stringValue = rawValue;
                Type = ValueType.Number;
            } else {
                stringValue = rawValue;
                numberValue = 0;
                Type = ValueType.String;
            }
        }

        public int GetInt()
        {
            return (int)numberValue;
        }

        public float GetFloat()
        {
            return numberValue;
        }

        public string GetString()
        {
            // ToString("R") 설명 https://docs.microsoft.com/ko-kr/dotnet/standard/base-types/standard-numeric-format-strings
            return stringValue ?? numberValue.ToString("R");
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ValueType), Type);
            info.AddValue(nameof(numberValue), numberValue);
            info.AddValue(nameof(stringValue), stringValue);
        }
    }
}