```
namespace Item.Instance
{
    public class ItemInstance
    {
        //实例信息
        protected ItemInformation _information;
        protected int _count;
        protected string _guid;

        //公开属性
        public ItemInformation Information => _information;
        public int Count => _count;
        public string Guid => _guid;
        public bool IsStackable => _information.CanStack;
        public bool IsEmpty => _count <= 0;

        public ItemInstance(ItemInformation information, int count, string guid)
        {
            if (information == null)
            {
                Debug.LogError("物品信息为空");
            }
            _information = information;
            _count = Mathf.Clamp(count, 1, information.CanStack ? information.MaxStackCount : 1);
            _guid = guid;
        }

        public virtual ItemData ToData()
        {
            ItemData data = new ItemData
            {
                ItemId = _information.Id,
                Count = _count,
                Guid = _guid
            };
            return data;
        }
        //加载一些额外数据，比如子弹耐久
        public virtual void FromData(ItemData data)
        {

        }
        public bool TryAddCount(int count)
        {
            if (count <= 0)
            {
                Debug.LogWarning("TryAddCount参数错误");
                return false;
            }
            if (!IsStackable)
            {
                Debug.LogWarning("物品不可堆叠，无法增加数量");
                return false;
            }
            int prev = _count;
            _count = Math.Min(_count + count, _information.MaxStackCount);
            return _count > prev;
        }
        public bool TryReduceCount(int count)
        {
            if (count <= 0)
            {
                return false;
            }
            if (!IsStackable)
            {
                return false;
            }
            int prev = _count;
            _count = Math.Max(_count - count, 0);
            return _count < prev;
        }
        //将物品拆分为一个新的实例
        public ItemInstance Spilt(int spiltCount)
        {
            if (!IsStackable)
            {
                return null;
            }
            if (spiltCount <= 0 || spiltCount >= _count)
            {
                return null;
            }
            _count -= spiltCount;
            
            ItemInstance newInstance = new ItemInstance(_information, spiltCount, System.Guid.NewGuid().ToString());
            return newInstance;
        }
        //将另一个实例的物品合并到当前实例
        public int MergeFrom(ItemInstance other)
        {
            if (other == null) return 0;
            if (!IsStackable || !other.IsStackable) return 0;
            if (_information.Id != other.Information.Id) return 0;

            int space = _information.MaxStackCount - _count;
            if (space <= 0) return 0;

            int transfer = Math.Min(space, other._count);
            _count += transfer;
            other._count -= transfer;
            return transfer;
        }
    }

}
```
