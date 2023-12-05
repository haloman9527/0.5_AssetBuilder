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
// using UnityEditor;
// using UnityEngine;
// using UnityEditorInternal;
// using System.Collections.Generic;
// using UnityEditor.AddressableAssets.GUI;
// using UnityEditor.AddressableAssets.Settings;
// using CZToolKitEditor.IMGUI.Controls;
//
// namespace CZToolKit.AssetBuilder
// {
//     public class LabelsSettingView : PopupWindowContent
//     {
//         ReorderableList labelsList;
//         AddressableAssetSettings settings;
//         AssetBuilderConfig config;
//         List<string> labels;
//         public HashSet<int> removeCache = new HashSet<int>();
//
//         public LabelsSettingView(AssetBuilderConfig config, List<string> labels, AddressableAssetSettings settings)
//         {
//             this.settings = settings;
//             this.config = config;
//             this.labels = labels;
//             labelsList = new ReorderableList(labels, typeof(string), false, true, true, false);
//             labelsList.drawHeaderCallback = (a) =>
//             {
//                 GUI.Label(a, "Labels");
//             };
//             labelsList.onAddCallback = (a) =>
//             {
//                 Undo.RegisterCompleteObjectUndo(config, "Add Label");
//                 labels.Add("");
//                 EditorUtility.SetDirty(config);
//             };
//             labelsList.drawElementCallback = (a, b, c, d) =>
//             {
//                 a.y += 1;
//                 var dropDownRect = a;
//                 dropDownRect.xMax -= 25;
//                 if (GUI.Button(dropDownRect, labels[b], EditorStyles.toolbarDropDown))
//                 {
//                     CZAdvancedDropDown dropDown = new CZAdvancedDropDown();
//                     foreach (var label in settings.GetLabels())
//                     {
//                         if (labels.Contains(label))
//                             continue;
//                         var item = dropDown.Add(label);
//                     }
//                     dropDown.onItemSelected += item =>
//                     {
//                         Undo.RegisterCompleteObjectUndo(config, "Change Label");
//                         labels[b] = item.name;
//                         EditorUtility.SetDirty(config);
//                         editorWindow.Repaint();
//                     };
//                     dropDown.Show(a, 300);
//                 }
//                 var removeButtonRect = a;
//                 removeButtonRect.xMin = removeButtonRect.xMax - 25;
//                 if (GUI.Button(removeButtonRect, EditorGUIUtility.IconContent("winbtn_mac_close_a"), EditorStyles.toolbarButton))
//                 {
//                     removeCache.Add(b);
//                 }
//             };
//         }
//
//         public override void OnOpen()
//         {
//             base.OnOpen();
//             Undo.undoRedoPerformed += editorWindow.Repaint;
//         }
//
//         public override void OnClose()
//         {
//             base.OnClose();
//             Undo.undoRedoPerformed -= editorWindow.Repaint;
//         }
//
//         public override Vector2 GetWindowSize()
//         {
//             return new Vector2(300, labelsList.GetHeight() + 20);
//         }
//
//         public override void OnGUI(Rect rect)
//         {
//             labelsList.DoLayoutList();
//             if (removeCache.Count > 0)
//             {
//                 Undo.RegisterCompleteObjectUndo(config, "Remove Label");
//                 foreach (var index in removeCache)
//                 {
//                     labels.RemoveAt(index);
//                 }
//                 EditorUtility.SetDirty(config);
//                 removeCache.Clear();
//                 editorWindow.Repaint();
//             }
//             if (GUILayout.Button("Labels Managed"))
//                 EditorWindow.GetWindow<LabelWindow>(true).Intialize(settings);
//         }
//     }
// }
