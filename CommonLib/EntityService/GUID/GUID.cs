using System;
using System.Diagnostics;
using System.Threading;

namespace EntityService
{
    /// <summary> GloballyUniqueIdentifierDefinition </summary>
    public class GUID
    {
        public static readonly GUID Empty = new GUID(0);
        internal GUID() { }
        internal GUID(ulong v) { Value = v; }
        public ulong Value { get; }
        // timestamp[63, 22], uniquifier[21, 10], machineId[9, 0]
        public ushort MachineId {
            get {
                //const ulong filter = 0b000000000000000000000000000000000000000000_000000000000_1111111111; // C# 7.0
                const ulong filter = (1UL << 10) - 1;
                ulong value = filter & Value;
                return (ushort)(value);
            }
        }

        public ushort Uniquifier {
            get {
                //const ulong filter = 0b000000000000000000000000000000000000000000_111111111111_0000000000; // C# 7.0
                const ulong filter = ((1UL << 12) - 1) << 10;
                ulong value = filter & Value;
                return (ushort)(value >> 10);
            }
        }

        public DateTime TimeStamp {
            get {
                //const ulong filter = 0b111111111111111111111111111111111111111111_000000000000_0000000000; // C# 7.0
                const ulong filter = ((1UL << 42) - 1) << 22;
                ulong value = filter & Value;
                long ticks = (long)(value >> 22) * TimeSpan.TicksPerMillisecond;
                return GUIDGen.TimeOrigin.AddTicks(ticks);
            }
        }

        public override bool Equals(object obj)
        {
            var g = obj as GUID;
            if (g == null) {
                return false;
            }

            return Value == g.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        //  c#에서 연산자를 재정의할 때에는 Equals와 GetHashCode 도 같이 맞춰줘야 한다
        public static bool operator ==(GUID a, GUID b)
        {
            if (a?.Value == b?.Value) {
                return true;
            }

            return false;
        }

        public static bool operator !=(GUID a, GUID b)
        {
            if (a?.Value != b?.Value) {
                return true;
            }

            return false;
        }
    }

    public static class GUIDGen
    {
        private static ushort m_uniquifier;
        private static ushort m_machineId;
        private static ulong m_oldTimeStamp;
        private static readonly object m_locker = new object();

        public static DateTime TimeOrigin { get; private set; }
        //  starttime 기준으로 timestamp를 계산하는데, 이것이 바뀌면 안되므로 항상 2015년 7월 14일 한국 시각 0시를 기준으로 계산한다
        public static void Init(ushort machineId)
        {
            Debug.Assert(machineId < 1024);
            TimeOrigin = new DateTime(2015, 7, 14);
            m_machineId = machineId;
        }

        public static GUID CreateGUID()
        {
            lock (m_locker) {
                return CreateGUID(DateTime.Now);
            }
        }

        private static GUID CreateGUID(DateTime now)
        {
            ulong timeStamp = (ulong)((now - TimeOrigin).Ticks / TimeSpan.TicksPerMillisecond);
            const long timeStampMax = (1L << 42) - 1;   // 42bit 최대값
            Debug.Assert(timeStamp < timeStampMax);

            if (timeStamp != m_oldTimeStamp) { // timestamp reset
                m_uniquifier = 0;
                m_oldTimeStamp = timeStamp;
            }

            if (m_uniquifier > 4095) {
                Thread.Sleep(1);
                return CreateGUID();
            }

            // high(63) ------------------------------------------ low(0)
            // timestamp [63, 22], uniquifier [21, 10], machine id [9, 0]
            ulong timestamp_bits = timeStamp << 22;
            ulong uniquifier_bits = (ulong)m_uniquifier << 10;
            ulong machineId_bits = (ulong)m_machineId << 0;

            m_uniquifier++;
            var newGuid = timestamp_bits | uniquifier_bits | machineId_bits;
            return new GUID(newGuid);
        }
    }
}
