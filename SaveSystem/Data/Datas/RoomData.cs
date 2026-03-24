using Data;
using System.Collections.Generic;

[System.Serializable]
public class RoomData :IDatable
{
    public int Id;
    public List<FurnitureData> FurnitureList;
    public bool HasUnlocked;
    public RoomData(int id)
    {
        Id = id;
        FurnitureList = new List<FurnitureData>();
        HasUnlocked = true;
    }
}
