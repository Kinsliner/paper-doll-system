
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.Tool
{
    [System.Serializable]
    public class UICollectorKeys
    {
        public int id;
        public string guid;
        public List<string> keys = new List<string>();
    }

    public class UICollectorSetting : ScriptableObject
    {
        public List<string> groupNames = new List<string>();
        public List<string> keys = new List<string>();

        public int IncreaseCollectorID;

        public List<UICollectorKeys> collectorKeys = new List<UICollectorKeys>();

        public int GetNewCollectorID()
        {
            return IncreaseCollectorID++;
        }

        public void Refresh()
        {
            keys.Clear();
            foreach (var collector in collectorKeys)
            {
                AddKeys(collector.keys);
            }
        }

        private void AddKeys(List<string> newKeys)
        {
            foreach (var key in newKeys)
            {
                if (keys.Contains(key) == false)
                {
                    keys.Add(key);
                }
            }
        }

        public void UpdateKeys(int collectorID, string guid, List<string> newKeys)
        {
            UICollectorKeys collector = collectorKeys.Find(p => p.id == collectorID);
            if (collector != null)
            {
                collector.guid = guid;
            }
            
            RefreshKeys(guid, newKeys);
        }

        public void RefreshKeys(string guid, List<string> newKeys)
        {
            if (collectorKeys.Exists(p => p.guid == guid) == false)
            {
                UICollectorKeys newCollector = new UICollectorKeys()
                {
                    id = -1,
                    guid = guid,
                    keys = new List<string>(newKeys)
                };

                collectorKeys.Add(newCollector);
            }
            else
            {
                UICollectorKeys collector = collectorKeys.Find(p => p.guid == guid);
                collector.keys.Clear();
                newKeys.ForEach((p) =>
                {
                    collector.keys.Add(p);
                });
            }
        }

        public void AddKeys(string guid, List<string> newKeys)
        {
            if (collectorKeys.Exists(p => p.guid == guid) == false)
            {
                UICollectorKeys newCollector = new UICollectorKeys()
                {
                    id = -1,
                    guid = guid,
                    keys = new List<string>(newKeys)
                };

                collectorKeys.Add(newCollector);
            }
            else
            {
                UICollectorKeys collector = collectorKeys.Find(p => p.guid == guid);
                newKeys.ForEach((k) =>
                {
                    if (collector.keys.Contains(k) == false)
                        collector.keys.Add(k);
                });
            }
        }

        public void RemoveKeys(string guid)
        {
            UICollectorKeys collector = collectorKeys.Find(p => p.guid == guid);
            if (collector != null)
            {
                collectorKeys.Remove(collector);
            }
        }

        public void AddGroup(string group)
        {
            if (groupNames.Contains(group) == false)
            {
                groupNames.Add(group);
            }
        }

        public void RemoveGroup(string group)
        {
            if (groupNames.Contains(group))
            {
                groupNames.Remove(group);
            }
        }

        public void Merge(UICollectorSetting otherSetting)
        {
            foreach (var group in otherSetting.groupNames)
            {
                AddGroup(group);
            }

            foreach (var collectorKey in otherSetting.collectorKeys)
            {
                AddKeys(collectorKey.guid, collectorKey.keys);
            }

            Refresh();
        }
    }
}

