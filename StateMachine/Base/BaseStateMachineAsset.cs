using FSM.Enums;
using System.Collections.Generic;
using UnityEngine;
namespace FSM.Base
{
    
    public class BaseStateMachineAsset : ScriptableObject 
    {
        public List<BaseTranslation> Translations;
        public BaseState InitialState;
        private Dictionary<StateType, List<BaseTranslation>> _map;
        public void OnValidate()
        {
            if(Translations != null&&Translations.Count>0)
            {
                BuildMap();
            }
        }
        public IReadOnlyList<BaseTranslation> GetTranslations(StateType type)
        {
            if(_map == null)
            {
                return null;
            }
            if(_map.TryGetValue(type,out var res))
            {
                return res.AsReadOnly();
            }
            return null;
        }
        private void BuildMap()
        {
            _map = new Dictionary<StateType, List<BaseTranslation>>();

            foreach (var translation in Translations)
            {
                if(!_map.ContainsKey(translation.From.Type))
                {
                    _map[translation.From.Type] = new List<BaseTranslation>();
                }
                _map[translation.From.Type].Add(translation);
            }
            foreach (var translations in _map.Values)
            {
                translations.Sort((a,b)=>b.Weight.CompareTo(a.Weight));
            }
        }
    }

}