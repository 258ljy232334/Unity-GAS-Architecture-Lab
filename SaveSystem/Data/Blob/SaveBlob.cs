using System.Collections.Generic;

namespace Data.Save
{
    [System.Serializable]
    public class SaveBlob
    {
        //打好反射标记
        [DataList]
        public List<VersionData> _version = new List<VersionData>();
        [DataList]
        public List<RoomData> _rooms = new();
        [DataList]
        public List<ProfileData> _profiles = new();
        [DataList]
        public List<ItemData> _items = new();
    }
}
