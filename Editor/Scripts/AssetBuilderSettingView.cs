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
//  *  Github: https://github.com/haloman9527
//  *  Blog: https://www.haloman.net/
//  *
//  */
// #endregion
// using UnityEditor;
// using UnityEngine;
// using UnityEditor.AddressableAssets.Settings;
//
// namespace CZToolKit.AssetBuilder
// {
//     public class AssetBuilderSettingView : PopupWindowContent
//     {
//         private AssetBuilderConfig config;
//
//         public AssetBuilderSettingView(AssetBuilderConfig config)
//         {
//             this.config = config;
//         }
//
//         public override Vector2 GetWindowSize()
//         {
//             return new Vector2(300, 100);
//         }
//
//         public override void OnGUI(Rect rect)
//         {
//             EditorGUI.BeginChangeCheck();
//             EditorGUI.BeginDisabledGroup(true);
//             EditorGUILayout.ObjectField("AssetBuilder Config", config, typeof(AssetBuilderConfig), false);
//             EditorGUI.EndDisabledGroup();
//             config.resolver = EditorGUILayout.ObjectField("Resolver", config.resolver, typeof(AssetBuilderResolver), false) as AssetBuilderResolver;
//             config.addressableAssetSettings = EditorGUILayout.ObjectField("Addressable Settings", config.addressableAssetSettings, typeof(AddressableAssetSettings), false) as AddressableAssetSettings;
//             config.addressableAssetGroupTemplate = EditorGUILayout.ObjectField("Group Template", config.addressableAssetGroupTemplate, typeof(AddressableAssetGroupTemplate), false) as AddressableAssetGroupTemplate;
//             if (EditorGUI.EndChangeCheck())
//             {
//                 Undo.RegisterCompleteObjectUndo(config, "Change Setting");
//                 EditorUtility.SetDirty(config);
//             }
//         }
//     }
// }
