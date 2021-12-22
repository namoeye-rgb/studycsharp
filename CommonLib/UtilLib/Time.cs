using System;

namespace UtilLib
{
    public class Time
    {
        private static readonly DateTime init = DateTime.UtcNow;
        private DateTime prev;

        // ----------------------------------------

        public void Init()
        {
            prev = DateTime.UtcNow;
        }

        public TimeSpan Update()
        {
            var now = DateTime.UtcNow;
            var delta = now - prev;
            prev = now;

            return delta;
        }

        public static double GetAppTimeMS()
        {
            var app = DateTime.UtcNow - init;
            return app.TotalMilliseconds;
        }
    }

    /// <summary>
    ///     dt를 이용한 Update 루프 안에서 dt를 기준으로 지나간 시간을 측정하고 일정 시간이 흘렀는지 확인하기 위한 코드입니다.
    /// </summary>
    public class Clock
    {
        public Clock(double tickTime = 1)
        {
            m_tickTime = tickTime;
            Reset();
        }

        /// <summary> 마지막으로 Tick이 발생한 시점 </summary>
        private double m_prevTick;
        /// <summary> 다음에 Tick이 발생해야 할 시점 </summary>
        private double m_nextTick;
        /// <summary> 일회용 딜레이 </summary>
        private double m_tempTick;
        /// <summary> 연속 된 두 Tick 사이의 일정한 시간차. 혹은 Tick 1회가 소비하는 시간 </summary>
        private double m_tickTime;

        /// <summary> Tick 1회가 소비하는 시간입니다. 즉, 0.5일 경우 1초에 2회 Tick 합니다. [sec] </summary>
        public double TickTime {
            get { return m_tickTime; }
            set { m_tickTime = value; m_nextTick = m_prevTick + value; }
        }

        /// <summary> 리셋 이후 흐른 시간. </summary>
        public double TimeSinceReset { get; private set; }

        /// <summary> 시계를 직접 설정하거나 값을 구합니다. </summary>
        public double ClockTime { get { return TimeSinceReset; } set { TimeSinceReset = value; } }

        /// <summary>이 시계를 일정 시간 앞으로 감습니다. </summary>
        /// <param name="dt">이 시계를 dt 만큼 앞으로 감습니다.[sec]</param>
        /// <param name="elapsed">지난번 틱으로 부터 흐른 시간을 반환합니다. </param>
        /// <returns>
        ///     지난번 Tick이후 새로운 Tick이 발생했는지 안했는지 여부를 반환합니다.
        ///     elapsed에는 항상 지난번 Tick으로 부터 현재까지 흐른 시간이 저장됩니다.
        /// </returns>
        public bool Update(double dt, out double elapsed)
        {
            // Update Clock
            TimeSinceReset += dt;

            // Caclulated Elapsed Time From Last Tick
            elapsed = TimeSinceReset - m_prevTick;

            // Clock will not tick if TickTime is zero
            if (Math.Abs(m_tickTime) <= double.Epsilon) {
                return false;
            }

            // Clock has reached TickTime. Generate Tick
            if (TimeSinceReset > m_nextTick) {
                m_prevTick = TimeSinceReset;
                m_nextTick = TimeSinceReset + m_tickTime;
                return true;
            }

            return false;
        }

        /// <summary> 이 시계를 일정 시간 앞으로 감습니다. </summary>
        /// <param name="dt">이 시계를 dt 만큼 앞으로 감습니다.[sec]</param>
        /// <returns> 지난번 Tick이후 새로운 Tick이 발생했는지 안했는지 여부를 반환합니다. </returns>
        public bool Update(double dt)
        {
            // Update Clock
            TimeSinceReset += dt;

            // Clock has reached TickTime. Generate Tick
            if (TimeSinceReset > m_nextTick + m_tempTick) {
                m_prevTick = TimeSinceReset;
                m_nextTick = TimeSinceReset + m_tickTime;
                m_tempTick = 0;
                return true;
            }

            return false;
        }

        /// <summary> dt 만큼의 시간이 흐른 후 다음 Tick이 발생할지 안할지를 검사합니다. </summary>
        public bool IsTickReached(double dt)
        {
            return TimeSinceReset + dt > m_nextTick;
        }

        /// <summary> 내부 시계를 0으로 되돌리고 Tick또한 0에서 이미 발생했다고 설정합니다. </summary>
        public void Reset()
        {
            TimeSinceReset = 0;
            m_prevTick = 0;
            m_nextTick = TickTime;
            m_tempTick = 0;
        }

        public void AddTempDelay(double tickTime)
        {
            m_tempTick = tickTime;
        }
    }
}
