using Data.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
namespace Data.Repo
{
    //数据容器
    public class DataRepo : IDataRepo
    {
        private Dictionary<Type, IReadOnlyList<IDatable>> _roDic = new();
        private Dictionary<Type, IList> _woDic = new();

        //反射的字典
        private static Dictionary<Type, FieldInfo> s_fields;

        static DataRepo()
        {
            s_fields = typeof(SaveBlob).//取到类为SaveBlob
                GetFields(BindingFlags.Public | BindingFlags.Instance)//获取实例和公开字段
                .Where(f => f.GetCustomAttribute<DataListAttribute>() != null)//挑选有对应标记的
                .ToDictionary(f => f.FieldType.GetGenericArguments()[0]);//列表里装的元素类型作为字典 Key，把字段信息缓存起来
        }

        /* ---------- IDataRepo ---------- */
        public IReadOnlyList<T> GetRoList<T>() where T : class, IDatable
        {
            return _roDic.TryGetValue(typeof(T), out var ro) ? ro as IReadOnlyList<T> : null;
        }

        public  List<T> GetWoList<T>() where T : class, IDatable
        {
            var type = typeof(T);

            if (!_woDic.TryGetValue(type, out var wo) || wo == null)
            {
                // 新建泛型 List<T>
                wo = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                _woDic[type] = wo;
            }
            // 安全强制转换 - 这里我们确定 wo 就是 List<T>
            return  (List<T>)wo;
        }
        public void Initialize(SaveBlob blob)
        {
            _woDic.Clear();
            _roDic.Clear();

            foreach (var (type, field) in s_fields)//利用反射完的字典添加列表
            {
                // 1. 保证存档字段非 null
                var blobList = field.GetValue(blob) as IList
                             ?? Activator.CreateInstance(field.FieldType) as IList;
                field.SetValue(blob, blobList);//防止空引用

                // 2. 创建运行时 List<T>
                var wo = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                if (blobList != null && blobList.Count > 0)
                {
                    wo.AddRange(blobList);
                }

                // 3. 只读视图，强制转换为IDatable
                var ro = wo.Cast<IDatable>().ToList().AsReadOnly();

                // 4. 登记到字典里
                _woDic[type] = wo;
                _roDic[type] = ro;
            }
        }

        public void SaveBlobData(SaveBlob blob)
        {
            foreach (var (elementType, runtimeList) in _woDic)
            {
                // 通过前面缓存的字段信息直接回写
                if (s_fields.TryGetValue(elementType, out var field))
                {
                    field.SetValue(blob, runtimeList);   // runtimeList 就是 List<RoomData>、List<ProfileData> ...
                }
            }
        }
        public T GetRoSingle<T>() where T : class, IDatable
        {
            var list = GetRoList<T>();
            return list?.Count > 0 ? list[0] : null;
        }

        public T GetWoSingle<T>() where T : class, IDatable
        {
            var list = GetWoList<T>();
            return list?.Count > 0 ? list[0] : null;
        }
    }
}
