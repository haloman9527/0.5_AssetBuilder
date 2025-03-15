// using Atom;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text.RegularExpressions;
// using UnityEditor;
// using UnityEngine;
//
// using UnityObject = UnityEngine.Object;
//
// namespace Atom.AssetBuilder
// {
//     public partial class AssetBuilderResolver : ScriptableObject
//     {
//         [SerializeField]
//         List<AssetType> assetTypes = new List<AssetType>();
//     }
//
//     public partial class AssetBuilderResolver
//     {
//         [NonSerialized]
//         Dictionary<string, int> assetTypesMap = new Dictionary<string, int>();
//
//         public IReadOnlyDictionary<string, int> AssetTypesMap
//         {
//             get { return assetTypesMap; }
//         }
//
//         protected virtual void OnEnable()
//         {
//             RefreshAssetTypesMap();
//         }
//
//         // 设置资源的key
//         public virtual string GetAssetKey(AssetBuilderConfig.Group group, AssetBuilderConfig.Folder folder, UnityObject asset)
//         {
//             return asset.name;
//         }
//
//         public GUIContent GetAssetTypeIcon(string assetTypeName)
//         {
//             var label = (GUIContent)null;
//             if (!assetTypesMap.TryGetValue(assetTypeName, out var index))
//             {
//                 label = EditorGUIUtility.IconContent("console.erroricon");
//                 label.tooltip = $"未定义的资源类型:{assetTypeName}";
//                 return label;
//             }
//             var assetType = assetTypes[index];
//             if (assetType.overrideIcon != null)
//             {
//                 label = EditorGUIUtility.IconContent(assetType.name);
//                 label.image = assetType.overrideIcon;
//             }
//             if (label == null && !string.IsNullOrEmpty(assetType.builtInIcon))
//                 label = EditorGUIUtility.IconContent(assetType.builtInIcon, assetType.name);
//             if (label == null)
//                 label = EditorGUIUtility.TrTextContent(assetType.name);
//             label.tooltip = assetType.name;
//             return label;
//         }
//
//         // 返回所有资源类型
//         public IEnumerable<string> GetAssetTypes()
//         {
//             foreach (var pair in assetTypesMap)
//             {
//                 yield return pair.Key;
//             }
//         }
//
//         // 根据配置返回资源列表
//         public IEnumerable<UnityObject> GetAssets(AssetBuilderConfig.Group group, AssetBuilderConfig.Folder folder)
//         {
//             if (!assetTypesMap.TryGetValue(group.assetType, out var index))
//                 throw new Exception($"未定义的资源类型:{group.assetType}");
//
//             var assetType = assetTypes[index];
//             switch (assetType.filterMode)
//             {
//                 case FilterMode.AssetDatabase:
//                     {
//                         string path = AssetDatabase.GetAssetPath(folder.folder);
//                         string[] guids = AssetDatabase.FindAssets(assetType.filter, new string[] { path });
//                         foreach (var guid in guids)
//                         {
//                             string assetPath = AssetDatabase.GUIDToAssetPath(guid);
//
//                             // 文件正则匹配
//                             if (!string.IsNullOrEmpty(group.pattern))
//                             {
//                                 if (!Regex.IsMatch(assetPath, group.pattern))
//                                     continue;
//                             }
//
//                             // 文件正则匹配
//                             if (!string.IsNullOrEmpty(folder.pattern))
//                             {
//                                 if (!Regex.IsMatch(assetPath, folder.pattern))
//                                     continue;
//                             }
//
//                             var asset = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
//                             if (asset != null)
//                                 yield return asset;
//                         }
//                         break;
//                     }
//                 case FilterMode.Directory:
//                     {
//                         string path = AssetDatabase.GetAssetPath(folder.folder);
//                         DirectoryInfo directoryInfo = new DirectoryInfo(path);
//                         string[] exts = assetType.filter.Split('|');
//                         foreach (var ext in exts)
//                         {
//                             FileInfo[] fileInfos = directoryInfo.GetFiles($"*.{ext}", SearchOption.AllDirectories);
//                             foreach (var fileInfo in fileInfos)
//                             {
//                                 string assetPath = Util_Unity.ConvertToRelativePath(fileInfo.FullName);
//
//                                 // 文件正则匹配
//                                 if (!string.IsNullOrEmpty(group.pattern))
//                                 {
//                                     if (!Regex.IsMatch(assetPath, group.pattern))
//                                         continue;
//                                 }
//
//                                 // 文件正则匹配
//                                 if (!string.IsNullOrEmpty(folder.pattern))
//                                 {
//                                     if (!Regex.IsMatch(assetPath, folder.pattern))
//                                         continue;
//                                 }
//
//                                 var asset = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
//                                 if (asset != null)
//                                     yield return asset;
//                             }
//                         }
//                         break;
//                     }
//                 case FilterMode.Blend:
//                     {
//
//                         string path = AssetDatabase.GetAssetPath(folder.folder);
//                         string[] patterns = assetType.filter.Split('|');
//                         string[] guids = AssetDatabase.FindAssets(patterns[0], new string[] { path });
//                         foreach (var guid in guids)
//                         {
//                             string assetPath = AssetDatabase.GUIDToAssetPath(guid);
//
//                             // 文件正则匹配
//                             if (!string.IsNullOrEmpty(group.pattern))
//                             {
//                                 if (!Regex.IsMatch(assetPath, group.pattern))
//                                     continue;
//                             }
//
//                             // 文件正则匹配
//                             if (!string.IsNullOrEmpty(folder.pattern))
//                             {
//                                 if (!Regex.IsMatch(assetPath, folder.pattern))
//                                     continue;
//                             }
//
//                             var asset = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
//                             if (asset != null)
//                                 yield return asset;
//                         }
//
//                         DirectoryInfo directoryInfo = new DirectoryInfo(path);
//                         for (int i = 1; i < patterns.Length; i++)
//                         {
//                             var ext = patterns[i];
//                             FileInfo[] fileInfos = directoryInfo.GetFiles($"*.{ext}", SearchOption.AllDirectories);
//                             foreach (var fileInfo in fileInfos)
//                             {
//                                 string assetPath = Util_Unity.ConvertToRelativePath(fileInfo.FullName);
//
//                                 // 文件正则匹配
//                                 if (!string.IsNullOrEmpty(group.pattern))
//                                 {
//                                     if (!Regex.IsMatch(assetPath, group.pattern))
//                                         continue;
//                                 }
//
//                                 // 文件正则匹配
//                                 if (!string.IsNullOrEmpty(folder.pattern))
//                                 {
//                                     if (!Regex.IsMatch(assetPath, folder.pattern))
//                                         continue;
//                                 }
//
//                                 var asset = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
//                                 if (asset != null)
//                                     yield return asset;
//                             }
//                         }
//                         break;
//                     }
//                 default:
//                     break;
//             }
//             yield break;
//         }
//
//         private void RefreshAssetTypesMap()
//         {
//             if (assetTypes == null)
//                 assetTypes = new List<AssetType>();
//             assetTypesMap.Clear();
//             for (int i = 0; i < assetTypes.Count; i++)
//             {
//                 var assetType = assetTypes[i];
//                 if (string.IsNullOrEmpty(assetType.name))
//                     continue;
//                 assetTypesMap[assetType.name] = i;
//             }
//         }
//
//         protected void OnValidate()
//         {
//             RefreshAssetTypesMap();
//         }
//     }
//
//     [System.Serializable]
//     public class AssetType
//     {
//         public string name;
//         public string builtInIcon;
//         public Texture overrideIcon;
//         public string filter;
//         public FilterMode filterMode;
//     }
//
//     public enum FilterMode
//     {
//         AssetDatabase,
//         Directory,
//         Blend
//     }
// }
