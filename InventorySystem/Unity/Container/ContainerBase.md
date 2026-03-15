```
namespace Inventory.Container
{
    public abstract class InventoryContainerBase
    {
        public int Capacity => _items.Length;
        public IReadOnlyList<ItemInstance> Items => Array.AsReadOnly(_items);
        public abstract ContainerType ContainerType { get; }
        [Inject]
        protected SignalBus _bus;

        protected ItemInstance[] _items;

        protected int _defaultCapacity;
        protected int _addOnceCapacity;
        protected int _maxCapacity;
        public virtual void Initialize(int def, int step, int max, List<ItemData> datas = null)
        {
            _defaultCapacity = def;
            _addOnceCapacity = step;
            _maxCapacity = max;
            _items = new ItemInstance[_defaultCapacity];
        }

        // 实例级 CRUD
        public virtual void Clear(int index)
        {
            ThrowIfOutOfRange(index);
            if (_items[index] != null)
            {
                _items[index] = null;
                NotifyChange(index, null);
            }
        }
        public virtual bool SetInstance(int index, ItemInstance instance)
        {
            ThrowIfOutOfRange(index);
            if (!CanAccept(instance))
            {
                return false;
            }
            if (ReferenceEquals(instance, _items[index]))
            {
                return false;
            }
            _items[index] = instance;
            NotifyChange(index, instance);
            return true;
        }
        public bool SwapInstance(int indexA, int indexB)
        {
            ThrowIfOutOfRange(indexA);
            ThrowIfOutOfRange(indexB);
            if (_items[indexA] == null && _items[indexB] == null)
            {
                return false;
            }
            else
            {
                ItemInstance temp = _items[indexA];
                _items[indexA] = _items[indexB];
                _items[indexB] = temp;
                NotifyChange(indexA, _items[indexA]);
                NotifyChange(indexB, _items[indexB]);
                return true;
            }
        }
        public virtual ItemInstance GetInstance(int index)
        {
            ThrowIfOutOfRange(index);
            if (_items[index] != null)
            {
                return _items[index];
            }
            else
            {
                return null;
            }
        }
        // 查询
        public virtual bool TryAddInstance(ItemInstance instance)
        {
            if (!CanAccept(instance) || instance?.IsEmpty != false)
                return false;

            // 可堆叠：先合并到现有
            if (instance.IsStackable)
            {
                for (int i = 0; i < Capacity && !instance.IsEmpty; i++)
                {
                    var existing = _items[i];
                    if (existing == null) continue;

                    if (existing.MergeFrom(instance) > 0)
                        NotifyChange(i, existing);
                }
            }

            // 还有剩余，找空位
            if (!instance.IsEmpty)
            {
                for (int i = 0; i < Capacity; i++)
                {
                    if (_items[i] != null) continue;

                    _items[i] = instance;
                    NotifyChange(i, instance);
                    return true;
                }
            }

            // 返回是否全部放入（instance.IsEmpty 表示全放进去了）
            return instance.IsEmpty;
        }
        public virtual bool TryRemoveInstance(int index)
        {
            ThrowIfOutOfRange(index);
            if (_items[index] == null)
            {
                return false;
            }
            else
            {
                _items[index] = null;
                NotifyChange(index, null);
                return true;
            }
        }
        public virtual bool TryReduceInstance(int index, int count = 1)
        {
            ThrowIfOutOfRange(index);
            if (_items[index] == null)
            {
                return false;
            }
            ItemInstance cur = _items[index];
            if ((cur.IsStackable && count > cur.Count)
                || (!cur.IsStackable && count > 1))
            {
                return false;
            }
            else
            {
                cur.TryReduceCount(count);
                if (cur.Count == 0)
                {
                    _items[index] = null;
                    NotifyChange(index, null);
                }
                else
                {
                    NotifyChange(index, cur);
                }
                return true;
            }
        }
        public bool IsEmpty(int index)
        {
            ThrowIfOutOfRange(index);
            return _items[index] == null;
        }
        public int IndexOf(ItemInstance instance)
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (ReferenceEquals(instance, _items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual bool ExpandCapacity()
        {
            if (Capacity >= _maxCapacity)
            {
                return false;
            }
            else
            {
                var newArr = new ItemInstance[Capacity + _addOnceCapacity];
                Array.Copy(_items, newArr, _items.Length);
                _items = newArr;              // 原子替换引用
                _bus.Fire(new InventoryContainerCapacityChangedSignal(ContainerType, Capacity));
                return true;
            }
        }
        protected void ThrowIfOutOfRange(int index)
        {
            if ((uint)index >= (uint)Capacity)
                throw new IndexOutOfRangeException($"Container {ContainerType} index {index} out of range [0,{Capacity}).");
        }
        public abstract bool CanAccept(ItemInstance instance);
        protected virtual void NotifyChange(int idx, ItemInstance newItem) =>
   _bus.Fire(new InventoryItemChangedSignal(ContainerType, idx, newItem));
    }
}
```
