using PacketLib.PacketField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.Game.User
{
    public partial class User
    {
        public Packet_User Get_Packet_User()
        {
            var packet = new Packet_User();
            packet.UID = UserInfo.Id.ToString();
            packet.NickName = UserInfo.NickName;
            packet.Level = UserInfo.Level;
            packet.Exp = UserInfo.Exp;

            packet.Character = Get_Packet_Character();
            packet.Wallet = Get_Packet_Wallet();
            packet.Battle = Get_Packet_Battle();

            return packet;
        }

        public Packet_Character Get_Packet_Character()
        {
            var character = new Packet_Character();
            character.UID = UserInfo.Characters.Id.ToString();
            character.Level = UserInfo.Characters.Level;
            character.Exp = UserInfo.Characters.Exp;

            return character;
        }

        public Packet_Wallet Get_Packet_Wallet()
        {
            var wallet = new Packet_Wallet();
            wallet.Gem = UserInfo.Wallet.Gem;
            wallet.Money = UserInfo.Wallet.Money;

            return wallet;
        }

        public Packet_BattleInfo Get_Packet_Battle()
        {
            var battle = new Packet_BattleInfo();
            battle.Grade = BattleInfo.BattleGrade;
            battle.WinCount = BattleInfo.WinCount;
            battle.LoseCount = BattleInfo.LoseCount;

            return battle;
        }
    }
}
