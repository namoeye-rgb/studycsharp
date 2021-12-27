using FluentScheduler;

namespace GameServer.Scheduler
{
    public class Schedule_BattleInfo : Registry
    {
        public Schedule_BattleInfo()
        {
            Schedule<Job_BattleInfo>().ToRunNow().AndEvery(5).Seconds();
        }
    }
}
