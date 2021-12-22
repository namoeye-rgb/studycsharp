using LobbyServer.DB.Model;
using PacketLib.PacketField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.DB.Packet_ExtendMethod
{
    public static class ConvertPacket
    {
        public static Packet_Character Get_Packet_Character(this FD_Character _model)
        {
            var character = new Packet_Character
            {
                UID = _model.Id.ToString(),
                Level = _model.Level,
                Exp = _model.Exp
            };

            return character;
        }

        public static Packet_Mail Get_Packet_Mail(this FD_Mail _model)
        {
            var packet = new Packet_Mail
            {
                MailUID = _model.Id.ToString(),
                RemainTime = _model.RemainTime,
                MailTID = (ushort)_model.MailTID,
                MailType = _model.Type,
            };

            packet.CustomInfo = Get_Packet_Mail_CustomInfo(_model.CustomInfo);
            packet.RewardInfo = Get_Packet_Mail_Reward(_model.Reward);

            return packet;
        }

        public static Packet_Mail_Reward Get_Packet_Mail_Reward(this FD_Reward _model)
        {
            var packet_reward = new Packet_Mail_Reward
            {
                ItemList = new List<Packet_RewardItemBase>(),
                PriceList = new List<Packet_Price>()
            };

            foreach (var data in _model.ItemList)
            {
                var packet = new Packet_RewardItemBase
                {
                    ItemType = data.Type,
                    ItemTID = data.ItemTID,
                    Count = (ushort)data.Count
                };

                packet_reward.ItemList.Add(packet);
            }

            foreach (var data in _model.PriceList)
            {
                var packet = new Packet_Price
                {
                    PriceType = data.Type,
                    PriceValue = data.Value
                };

                packet_reward.PriceList.Add(packet);
            }

            return packet_reward;
        }

        public static Packet_Mail_CustomInfo Get_Packet_Mail_CustomInfo(this FD_MailCustomInfo _model)
        {
            if(_model == null)
            {
                return null;
            }

            var packet = new Packet_Mail_CustomInfo
            {
                Message = _model.Message,
                Title = _model.Title,
            };

            return packet;
        }
    }
}
