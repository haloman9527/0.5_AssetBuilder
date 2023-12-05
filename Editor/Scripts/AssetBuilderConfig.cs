// #region 注 释
// /***
//  *
//  *  Title:
//  *  
//  *  Description:
//  *  
//  *  Date:
//  *  Version:
//  *  Writer: 半只龙虾人
//  *  Github: https://github.com/HalfLobsterMan
//  *  Blog: https://www.mindgear.net/
//  *
//  */
// #endregion
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEditor.AddressableAssets.Settings;
// using UnityEngine;
//
// using UnityObject = UnityEngine.Object;
//
// namespace CZToolKit.AssetBuilder
// {
//     public partial class AssetBuilderConfig : ScriptableObject
//     {
//         #region Define
//         [Serializable]
//         public partial class Group
//         {
//             public string groupName;
//             public string assetType;
//             public string pattern;
//             public List<string> labels = new List<string>();
//             public List<Folder> folders = new List<Folder>();
//         }
//
//         [Serializable]
//         public partial class Folder
//         {
//             public UnityObject folder;
//             public string pattern;
//             public List<string> labels = new List<string>();
//         }
//
//         public enum AssetType
//         {
//             Prefab = 0,
//             AudioClip = 1,
//         }
//         #endregion
//
//         #region Field
//         public AssetBuilderResolver resolver;
//         public AddressableAssetSettings addressableAssetSettings;
//         public AddressableAssetGroupTemplate addressableAssetGroupTemplate;
//         [SerializeField] private List<Group> groups = new List<Group>();
//         #endregion
//     }
//
//     public partial class AssetBuilderConfig
//     {
//         #region Define
//         public partial class Group
//         {
//             public Group(string groupName, string assetType)
//             {
//                 this.groupName = groupName;
//                 this.assetType = assetType;
//             }
//         }
//
//         public partial class Folder
//         {
//
//         }
//         #endregion
//
//         #region Field
//         [NonSerialized] Dictionary<string, Group> groupsMap;
//         #endregion
//
//         #region Property
//         public IReadOnlyList<Group> Groups
//         {
//             get { return groups; }
//         }
//         #endregion
//
//         #region Unity
//         private void OnEnable()
//         {
//             groupsMap = new Dictionary<string, Group>();
//             foreach (var group in groups)
//             {
//                 if (group == null)
//                     continue;
//                 groupsMap[group.groupName] = group;
//             }
//         }
//         #endregion
//
//         #region Public
//         public bool ContainsGroup(string groupName)
//         {
//             return groupsMap.ContainsKey(groupName);
//         }
//
//         public bool AddGroup(Group group)
//         {
//             if (groupsMap.ContainsKey(group.groupName))
//                 return false;
//             groups.Add(group);
//             groupsMap[group.groupName] = group;
//             return true;
//         }
//
//         public void RemoveGroup(Group group)
//         {
//             if (!groups.Contains(group))
//                 return;
//             groups.Remove(group);
//             groupsMap.Remove(group.groupName);
//         }
//
//         public bool RenameGroup(string oldName, string newName)
//         {
//             if (!groupsMap.TryGetValue(oldName, out var group))
//                 return false;
//             if (group.groupName == newName)
//                 return false;
//             if (newName.Contains("/"))
//                 return false;
//             group.groupName = newName;
//             groupsMap.Remove(oldName);
//             groupsMap[newName] = group;
//             return true;
//         }
//
//         public bool AddFolder(Group group, Folder folder)
//         {
//             if (!groups.Contains(group))
//                 return false;
//             if (group.folders.Contains(folder))
//                 return false;
//             group.folders.Add(folder);
//             return true;
//         }
//
//         public bool RemoveFolder(Group group, Folder folder)
//         {
//             if (!groups.Contains(group))
//                 return false;
//             if (!group.folders.Contains(folder))
//                 return false;
//             group.folders.Remove(folder);
//             return true;
//         }
//
//         public bool Clean()
//         {
//             groups.RemoveAll(item => item == null);
//             groupsMap.Clear();
//             foreach (var group in groups)
//             {
//                 group.folders.RemoveAll(item => (item == null || item.folder == null));
//                 groupsMap[group.groupName] = group;
//             }
//             return true;
//         }
//
//         public void Build()
//         {
//             HashSet<UnityObject> assetBuffer = new HashSet<UnityObject>();
//             foreach (var group in groups)
//             {
//                 BuildGroup(group, assetBuffer);
//             }
//             AssetDatabase.SaveAssets();
//         }
//
//         public void BuildGroups(List<Group> groups)
//         {
//             HashSet<UnityObject> assetBuffer = new HashSet<UnityObject>();
//             foreach (var group in groups)
//             {
//                 BuildGroup(group, assetBuffer);
//             }
//             AssetDatabase.SaveAssets();
//         }
//
//         public void BuildGroup(Group group)
//         {
//             BuildGroup(group, new HashSet<UnityObject>());
//             AssetDatabase.SaveAssets();
//         }
//
//         public void BuildGroup(Group group, HashSet<UnityObject> assetBuffer)
//         {
//             var addressableGroup = GetGroup(group.groupName);
//             foreach (var folder in group.folders)
//             {
//                 if (null == folder.folder)
//                     continue;
//                 string[] guids = AssetDatabase.FindAssets($"t:{group.assetType}", new string[] { AssetDatabase.GetAssetPath(folder.folder) });
//                 foreach (var asset in resolver.GetAssets(group, folder))
//                 {
//                     assetBuffer.Add(asset);
//                 }
//                 var entries = AddToGroup(addressableGroup, group, folder, assetBuffer.ToArray());
//                 foreach (var entry in entries)
//                 {
//                     foreach (var label in group.labels)
//                     {
//                         entry.SetLabel(label, true);
//                     }
//                     foreach (var label in folder.labels)
//                     {
//                         entry.SetLabel(label, true);
//                     }
//                 }
//             }
//             assetBuffer.Clear();
//         }
//         #endregion
//
//         #region Private
//         /// <summary> 获取/创建组 </summary>
//         private AddressableAssetGroup GetGroup(string groupName)
//         {
//             // 检测Group中有没有文件夹命名的组，有就将文件添加到Groups，没有就创建
//             AddressableAssetGroup accessionGroup = addressableAssetSettings.groups.Where(group => { return group != null && group.name == groupName; }).FirstOrDefault();
//             if (accessionGroup == null)
//             {
//                 // 后两个参数，1.拷贝现有的（Assets中的），2.创造全新的（参数默认）
//                 accessionGroup = addressableAssetSettings.CreateGroup(groupName, false, false, false, addressableAssetGroupTemplate.SchemaObjects);
//             }
//             return accessionGroup;
//         }
//
//         private List<AddressableAssetEntry> AddToGroup(AddressableAssetGroup targetGroup, Group group, Folder folder, IList<UnityObject> assets)
//         {
//             List<AddressableAssetEntry> addressableAssetEntries = new List<AddressableAssetEntry>(targetGroup.entries);
//
//             foreach (var entry in addressableAssetEntries)
//             {
//                 if (entry.TargetAsset == null)
//                     targetGroup.RemoveAssetEntry(entry);
//             }
//
//             foreach (var asset in assets)
//             {
//                 var assetEntry = addressableAssetEntries.FirstOrDefault(item => item.TargetAsset == asset);
//
//                 if (assetEntry == null)
//                 {
//                     var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
//                     assetEntry = targetGroup.Settings.CreateOrMoveEntry(guid, targetGroup, false);
//                 }
//
//                 if (assetEntry != null)
//                 {
//                     assetEntry.address = resolver.GetAssetKey(group, folder, asset);
//                     addressableAssetEntries.Add(assetEntry);
//                 }
//             }
//             return addressableAssetEntries;
//         }
//         #endregion
//     }
// }