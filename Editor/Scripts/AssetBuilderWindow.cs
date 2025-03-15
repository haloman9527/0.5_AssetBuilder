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
// using UnityEditor.IMGUI.Controls;
// using System.IO;
// using System.Collections.Generic;
//
// namespace Atom.AssetBuilder
// {
//     public partial class AssetBuilderWindow : EditorWindow
//     {
//         AssetBuilderTreeView treeView;
//         TreeViewState treeViewState = new TreeViewState();
//         SearchField searchField;
//         string searchText = "";
//
//         private void OnEnable()
//         {
//             titleContent = new GUIContent("Asset Builder");
//             searchField = new SearchField();
//             Undo.undoRedoPerformed += Refresh;
//             Refresh();
//         }
//
//         private void OnDisable()
//         {
//             AssetDatabase.SaveAssets();
//             Undo.undoRedoPerformed -= Refresh;
//         }
//
//         private void OnProjectChange()
//         {
//             Refresh();
//         }
//
//         private void Refresh()
//         {
//             if (treeView == null || treeView.Config == null)
//             {
//                 string[] guids = AssetDatabase.FindAssets($"t:{nameof(AssetBuilderConfig)}");
//                 if (guids.Length > 0)
//                 {
//                     AssetBuilderConfig config = null;
//                     foreach (var item in guids)
//                     {
//                         var path = AssetDatabase.GUIDToAssetPath(item);
//                         config = AssetDatabase.LoadAssetAtPath<AssetBuilderConfig>(path);
//                         if (null != config)
//                             break;
//                     }
//                     if (null != config)
//                     {
//                         treeView = new AssetBuilderTreeView(treeViewState, config);
//                     }
//                 }
//             }
//             if (treeView != null && treeView.Config != null)
//                 treeView.Refresh();
//         }
//
//         public void OnGUI()
//         {
//             if (treeView == null || treeView.Config == null)
//             {
//                 GUILayout.FlexibleSpace();
//                 if (GUILayout.Button("创建Asset Builder Config", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
//                 {
//                     string path = EditorUtility.SaveFilePanelInProject("创建配置文件", "AssetBuilderConfig", "Asset", "");
//                     if (!string.IsNullOrEmpty(path))
//                     {
//                         AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AssetBuilderConfig>(), path);
//                         GUIUtility.ExitGUI();
//                     }
//                 }
//                 GUILayout.FlexibleSpace();
//                 return;
//             }
//
//             OnToolbarGUI();
//
//             if (treeView.Config.resolver == null)
//             {
//                 GUILayout.FlexibleSpace();
//                 GUILayout.Box("需要设置Setting -> Resolver", "AC BoldHeader", GUILayout.ExpandWidth(true), GUILayout.Height(30));
//                 GUILayout.FlexibleSpace();
//                 return;
//             }
//
//             if (treeView.Config.addressableAssetSettings == null)
//             {
//                 GUILayout.FlexibleSpace();
//                 GUILayout.Box("需要设置Setting -> Addressable Asset Setting", "AC BoldHeader", GUILayout.ExpandWidth(true), GUILayout.Height(30));
//                 GUILayout.FlexibleSpace();
//                 return;
//             }
//
//             if (treeView.Config.addressableAssetGroupTemplate == null)
//             {
//                 GUILayout.FlexibleSpace();
//                 GUILayout.Box("需要设置Setting -> Group Template", "AC BoldHeader", GUILayout.ExpandWidth(true), GUILayout.Height(30));
//                 GUILayout.FlexibleSpace();
//                 return;
//             }
//
//             OnTreeViewGUI();
//         }
//
//         private void OnToolbarGUI()
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUI.BeginDisabledGroup(treeView.Config.resolver == null);
//             var createGroupBtnRect = GUILayoutUtility.GetRect(EditorGUIUtility.TrTextContent("Create Group"), EditorStyles.toolbarDropDown);
//             if (GUI.Button(createGroupBtnRect, EditorGUIUtility.TrTextContent("Create Group"), EditorStyles.toolbarDropDown))
//             {
//                 var menu = new GenericMenu();
//                 foreach (var assetType in treeView.Config.resolver.GetAssetTypes())
//                 {
//                     menu.AddItem(EditorGUIUtility.TrTextContent(assetType), false, () =>
//                     {
//                         int index = 0;
//                         string name = $"New Group({index})";
//                         while (treeView.Config.ContainsGroup(name))
//                         {
//                             index++;
//                             name = $"New Group({index})";
//                         }
//                         var newGroup = new AssetBuilderConfig.Group(name, assetType);
//                         Undo.RegisterCompleteObjectUndo(treeView.Config, $"Add Group {newGroup.groupName}");
//                         treeView.Config.AddGroup(newGroup);
//                         EditorUtility.SetDirty(treeView.Config);
//                         treeView.AddGroup(newGroup);
//                     });
//                 }
//                 menu.DropDown(createGroupBtnRect);
//             }
//             EditorGUI.EndDisabledGroup();
//             GUILayout.FlexibleSpace();
//             if (GUILayout.Button(EditorGUIUtility.TrTextContent("?", "文档"), EditorStyles.toolbarButton, GUILayout.Width(25)))
//             {
//                 var config = Resources.Load("AssetBuilder/AssetBuilderConfig");
//                 var path = AssetDatabase.GetAssetPath(config);
//                 var folder = Path.GetDirectoryName(path);
//                 Application.OpenURL(Path.Combine(folder, "AssetBuilderDoc.docx"));
//             }
//             if (GUILayout.Button(EditorGUIUtility.TrTextContent("Groups"), EditorStyles.toolbarButton))
//             {
//                 EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
//             }
//             Rect settingRect = GUILayoutUtility.GetRect(EditorGUIUtility.TrTextContent("Setting"), EditorStyles.toolbarButton);
//             if (GUI.Button(settingRect, EditorGUIUtility.TrTextContent("Setting"), EditorStyles.toolbarButton))
//             {
//                 PopupWindow.Show(settingRect, new AssetBuilderSettingView(treeView.Config));
//             }
//
//             Rect buildRect = GUILayoutUtility.GetRect(EditorGUIUtility.TrTextContent("Build"), EditorStyles.toolbarDropDown);
//             if (GUI.Button(buildRect, EditorGUIUtility.TrTextContent("Build"), EditorStyles.toolbarDropDown))
//             {
//                 var menu = new GenericMenu();
//                 foreach (var assetType in treeView.Config.resolver.GetAssetTypes())
//                 {
//                     menu.AddItem(new GUIContent($"Build/{assetType}"), false, () =>
//                     {
//                         var groups = new List<AssetBuilderConfig.Group>();
//                         foreach (var group in treeView.Config.Groups)
//                         {
//                             if (group.assetType == assetType)
//                             {
//                                 groups.Add(group);
//                             }
//                         }
//                         treeView.Config.BuildGroups(groups);
//                     });
//                 }
//                 menu.AddItem(new GUIContent("Build All"), false, () =>
//                 {
//                     treeView.Config.Build();
//                 });
//                 menu.AddSeparator("");
//                 menu.AddItem(new GUIContent("Clean"), false, () =>
//                 {
//                     Undo.RegisterCompleteObjectUndo(this, "Clean");
//                     treeView.Config.Clean();
//                     EditorUtility.SetDirty(this);
//                     treeView.Refresh();
//                 });
//                 menu.DropDown(buildRect);
//             }
//
//             string text = searchField.OnToolbarGUI(EditorGUILayout.GetControlRect(GUILayout.Width(250)), searchText);
//             if (searchText != text)
//             {
//                 searchText = text;
//                 treeView.searchString = searchText;
//             }
//             if (searchField.HasFocus())
//             {
//                 if (Event.current.type == EventType.KeyUp)
//                 {
//                     switch (Event.current.keyCode)
//                     {
//                         case KeyCode.Return:
//                         case KeyCode.KeypadEnter:
//                             treeView.SetFocusAndEnsureSelectedItem();
//                             Event.current.Use();
//                             break;
//                     }
//                 }
//             }
//
//             EditorGUILayout.EndHorizontal();
//
//         }
//
//         private void OnTreeViewGUI()
//         {
//             Rect treeViewRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//             treeView.OnGUI(treeViewRect);
//         }
//
//         #region Static
//         [MenuItem("Tools/Atom/Asset Builder")]
//         public static void Open()
//         {
//             GetWindow<AssetBuilderWindow>().Close();
//             GetWindow<AssetBuilderWindow>();
//         }
//         #endregion
//     }
// }