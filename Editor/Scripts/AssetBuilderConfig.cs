#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.AssetBuilder
{
    public class AssetBuilderConfig : ScriptableObject
    {
        public AssetBuilderResolver resolver;
        public AddressableAssetSettings addressableAssetSettings;
        public AddressableAssetGroupTemplate addressableAssetGroupTemplate;
        [SerializeField]
        private List<Group> groups = new List<Group>();

        public IReadOnlyList<Group> Groups
        {
            get => groups;
        }

        public bool ContainsGroup(string groupName)
        {
            return Groups.FirstOrDefault(group => group.groupName == groupName) != null;
        }

        public bool AddGroup(Group group)
        {
            if (ContainsGroup(group.groupName))
                return false;
            groups.Add(group);
            return true;
        }

        public void RemoveGroup(Group group)
        {
            if (!groups.Contains(group))
                return;
            groups.Remove(group);
        }

        public bool RenameGroup(Group group, string newName)
        {
            if (group.groupName == newName)
                return false;
            if (newName.Contains("/"))
                return false;
            group.groupName = newName;
            return true;
        }

        public bool AddFolder(Group group, Folder folder)
        {
            if (!groups.Contains(group))
                return false;
            if (group.folders.Contains(folder))
                return false;
            group.folders.Add(folder);
            return true;
        }

        public bool RemoveFolder(Group group, Folder folder)
        {
            if (!groups.Contains(group))
                return false;
            if (!group.folders.Contains(folder))
                return false;
            group.folders.Remove(folder);
            return true;
        }

        public bool Clean()
        {
            groups.RemoveAll(item => item == null);
            foreach (var group in groups)
            {
                group.folders.RemoveAll(item => (item == null || item.folder == null));
            }
            return true;
        }

        public void Build()
        {
            HashSet<UnityObject> assetBuffer = new HashSet<UnityObject>();
            foreach (var group in groups)
            {
                BuildGroup(group, assetBuffer);
            }
        }

        public void BuildGroup(Group group)
        {
            BuildGroup(group, new HashSet<UnityObject>());
        }

        public void BuildGroup(Group group, HashSet<UnityObject> assetBuffer)
        {
            var addressableGroup = GetGroup(group.groupName);
            foreach (var folder in group.folders)
            {
                string[] guids = AssetDatabase.FindAssets($"t:{group.assetType}", new string[] { AssetDatabase.GetAssetPath(folder.folder) });
                foreach (var asset in resolver.GetAssets(group, folder))
                {
                    assetBuffer.Add(asset);
                }
                var entries = AddToGroup(addressableGroup, group, folder, assetBuffer.ToArray());
                foreach (var entry in entries)
                {
                    foreach (var label in group.labels)
                    {
                        entry.SetLabel(label, true);
                    }
                    foreach (var label in folder.labels)
                    {
                        entry.SetLabel(label, true);
                    }
                }
            }
            assetBuffer.Clear();
        }

        /// <summary> 获取/创建组 </summary>
        private AddressableAssetGroup GetGroup(string groupName)
        {
            // 检测Group中有没有文件夹命名的组，有就将文件添加到Groups，没有就创建
            AddressableAssetGroup accessionGroup = addressableAssetSettings.groups.Where(group => { return group != null && group.name == groupName; }).FirstOrDefault();
            if (accessionGroup == null)
            {
                // 后两个参数，1.拷贝现有的（Assets中的），2.创造全新的（参数默认）
                accessionGroup = addressableAssetSettings.CreateGroup(groupName, false, false, false, addressableAssetGroupTemplate.SchemaObjects);
            }
            return accessionGroup;
        }

        private List<AddressableAssetEntry> AddToGroup(AddressableAssetGroup targetGroup, Group group, Folder folder, IList<UnityObject> assets)
        {
            List<AddressableAssetEntry> addressableAssetEntries = new List<AddressableAssetEntry>(targetGroup.entries);

            foreach (var entry in addressableAssetEntries)
            {
                if (entry.TargetAsset == null)
                    targetGroup.RemoveAssetEntry(entry);
            }

            foreach (var asset in assets)
            {
                var assetEntry = addressableAssetEntries.FirstOrDefault(item => item.TargetAsset == asset);

                if (assetEntry == null)
                {
                    var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
                    assetEntry = targetGroup.Settings.CreateOrMoveEntry(guid, targetGroup, false);
                }

                if (assetEntry != null)
                {
                    assetEntry.address = resolver.GetAssetKey(group, folder, asset);
                    addressableAssetEntries.Add(assetEntry);
                }
            }
            return addressableAssetEntries;
        }

        [Serializable]
        public class Group
        {
            public string groupName;
            public string assetType;
            public string pattern;
            public List<string> labels = new List<string>();
            public List<Folder> folders = new List<Folder>();

            public Group(string groupName, string assetType)
            {
                this.groupName = groupName;
                this.assetType = assetType;
            }
        }

        [Serializable]
        public class Folder
        {
            public UnityObject folder;
            public string pattern;
            public List<string> labels = new List<string>();
        }

        public enum AssetType
        {
            Prefab = 0,
            AudioClip = 1,
        }
    }
}