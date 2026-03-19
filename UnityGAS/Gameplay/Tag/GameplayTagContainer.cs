using System.Collections.Generic;

namespace Gameplay.Tag
{
    public class GameplayTagContainer 
    {
        private readonly HashSet<GameplayTag> _tags= new HashSet<GameplayTag>();
        public bool  HasTag(GameplayTag tag)
        {
            return _tags.Contains(tag);
        }
        public bool HasAny(List<GameplayTag> tags)
        {
            foreach (GameplayTag tag in tags)
            {
                if(!_tags.Contains(tag))
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasAll(List<GameplayTag> tags)
        {
            foreach (GameplayTag tag in tags)
            {
                if(!_tags.Contains(tag))
                {
                    return false;
                }
            }
            return true;
        }
        public bool AddTag(GameplayTag tag)
        {
            return _tags.Add(tag);
        }
        public bool RemoveTag(GameplayTag tag)
        {
            return _tags.Remove(tag);
        }
        public void Clear()
        {
            _tags.Clear();
        }
        public IReadOnlyList<GameplayTag> GetTags()
        {
            List<GameplayTag> temp= new List<GameplayTag>(_tags);
            return temp.AsReadOnly();
        }
    }
}
