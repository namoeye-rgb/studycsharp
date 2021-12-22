using GameCommon.Enum;
using LobbyServer.DB.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.Lobby
{
    public partial class User
    {

        public void AddStackItem(int _itemTID, ushort _count)
        {
            var stackItemFind = DB_UserInfo.StackItemInventroy.StackItems.Find(x => x.ItemTID == _itemTID);

            if (stackItemFind == null)
            {
                DB_UserInfo.StackItemInventroy.StackItems.Add(new FD_StackItem
                {
                    ItemTID = _itemTID,
                    Count = _count,
                });
                return;
            }

            stackItemFind.Count += _count;
        }

        public void AddUniqueItem(string _uid, int _itemTID, int _grade, int _exp)
        {
            DB_UserInfo.UniqueItemInventroy.UniqueItems.Add(new FD_UniqueItem
            {
                Id = ObjectId.Parse(_uid),
                ItemTID = _itemTID,
                Grade = _grade,
                Exp = _exp,
            });
        }

        public ERROR_CODE DeleteUniqueItem(string _uid, out FD_UniqueItem _deleteItem)
        {
            _deleteItem = null;
            if (ObjectId.TryParse(_uid, out ObjectId itemUID) == false)
            {
                return ERROR_CODE.INVALID_ITEM_UID;
            }

            var items = DB_UserInfo.UniqueItemInventroy.UniqueItems;

            int findItemIdx = -1;
            for (var i = 0; i < items.Count; ++i)
            {
                var item = items[i];
                if (item.Id == itemUID)
                {
                    _deleteItem = items[i];
                    findItemIdx = i;
                    break;
                }
            }

            if (_deleteItem == null)
            {
                return ERROR_CODE.NOT_FOUND_ITEM;
            }
            else
            {
                items.RemoveAt(findItemIdx);
            }

            return ERROR_CODE.SUCEESS;
        }
    }
}
