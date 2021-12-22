using FluentScheduler;
using GameServer.Net;
using PacketLib.Message;

namespace GameServer.Scheduler
{
    public class Job_BattleInfo : IJob
    {
        private PK_G2S_BATTLE_ROOM_INFO packet = new PK_G2S_BATTLE_ROOM_INFO();

        public void Execute()
        {
            if (S2SClient.Instance.IsAvailable() == false) {
                return;
            }

            //TODO : 룸 매니저나 아무튼 배틀 룸 관련된 갯수와 얻어와야함
            packet.RoomCount = 10;
            packet.TotalUsers = 1000;

            S2SClient.Instance.SendPacket(packet);
        }
    }
}
