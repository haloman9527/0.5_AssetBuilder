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
// using Jiange;
// using System.Collections.Generic;
// using JiangeEditor.IMGUI.Controls;
// using UnityEditor;
// using UnityEngine;
// using UnityEditor.IMGUI.Controls;
//
// namespace Jiange.AssetBuilder
// {
//     public partial class AssetBuilderWindow
//     {
//         public class AssetBuilderTreeView : CZTreeView
//         {
//             public AssetBuilderConfig Config { get; private set; }
//
//             public AssetBuilderTreeView(TreeViewState state, AssetBuilderConfig config) : base(state)
//             {
//                 this.Config = config;
//                 var treeViewHeaderColumns = new MultiColumnHeaderState.Column[]
//                 {
//                     new MultiColumnHeaderState.Column(){headerContent = new GUIContent("Info"), headerTextAlignment = TextAlignment.Center,canSort = false, allowToggleVisibility = false, width = 30, minWidth = 30, maxWidth = 30 },
//                     new MultiColumnHeaderState.Column(){headerContent = new GUIContent("Group \\ Folder"), headerTextAlignment = TextAlignment.Left, allowToggleVisibility = false, autoResize = true, width = 300, minWidth = 150 },
//                     new MultiColumnHeaderState.Column(){headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Asset Type"), headerTextAlignment = TextAlignment.Center, allowToggleVisibility = false, autoResize = false, width = 30, minWidth = 30 },
//                     new MultiColumnHeaderState.Column(){headerContent = new GUIContent("Labels"), headerTextAlignment = TextAlignment.Left, canSort = false, allowToggleVisibility = false, autoResize = true, width = 150, minWidth = 150 },
//                     new MultiColumnHeaderState.Column(){headerContent = new GUIContent("Pattern"), headerTextAlignment = TextAlignment.Left, canSort = false, allowToggleVisibility = false, autoResize = true, width = 150, minWidth = 150 },
//                 };
//
//                 var treeViewHeaderState = new MultiColumnHeaderState(treeViewHeaderColumns);
//                 this.multiColumnHeader = new MultiColumnHeader(treeViewHeaderState);
//                 multiColumnHeader.canSort = true;
//                 multiColumnHeader.sortedColumnIndex = 1;
//                 multiColumnHeader.GetColumn(1).sortedAscending = true;
//                 multiColumnHeader.sortingChanged += header => Reload();
//
//                 this.columnIndexForTreeFoldouts = 1;
//                 ShowBoder = true;
//                 RowHeight = 25;
//                 Refresh();
//                 this.multiColumnHeader.ResizeToFit();
//             }
//
//             protected override TreeViewItem BuildRoot()
//             {
//                 var root = base.BuildRoot();
//                 Sort();
//                 return root;
//             }
//
//             protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
//             {
//                 if (base.DoesItemMatchSearch(item, search))
//                     return true;
//                 if (item.displayName.ToLower().Contains(search.ToLower()))
//                     return true;
//                 return false;
//             }
//
//             public void Sort()
//             {
//                 // SortGroups
//                 {
//                     var header = this.multiColumnHeader;
//                     switch (header.sortedColumnIndex)
//                     {
//                         case 1:
//                             {
//                                 if (header.GetColumn(1).sortedAscending)
//                                     (RootItem as CZTreeViewItem).children.QuickSort((a, b) => { return a.displayName.CompareTo(b.displayName); });
//                                 else
//                                     (RootItem as CZTreeViewItem).children.QuickSort((a, b) => { return -a.displayName.CompareTo(b.displayName); });
//                             }
//                             break;
//                         case 2:
//                             {
//                                 if (header.GetColumn(2).sortedAscending)
//                                 {
//                                     (RootItem as CZTreeViewItem).children.QuickSort((a, b) =>
//                                     {
//                                         var groupA = (a as CZTreeViewItem).userData as AssetBuilderConfig.Group;
//                                         var groupB = (b as CZTreeViewItem).userData as AssetBuilderConfig.Group;
//                                         return groupA.assetType.CompareTo(groupB.assetType);
//                                     });
//                                 }
//                                 else
//                                 {
//                                     (RootItem as CZTreeViewItem).children.QuickSort((a, b) =>
//                                     {
//                                         var groupA = (a as CZTreeViewItem).userData as AssetBuilderConfig.Group;
//                                         var groupB = (b as CZTreeViewItem).userData as AssetBuilderConfig.Group;
//                                         return -groupA.assetType.CompareTo(groupB.assetType);
//                                     });
//                                 }
//                             }
//                             break;
//                     }
//                 }
//
//                 // SortFolders
//                 {
//                     foreach (var groupItem in (RootItem as CZTreeViewItem).children)
//                     {
//                         (groupItem as CZTreeViewItem).children.QuickSort((a, b) => { return a.displayName.CompareTo(b.displayName); });
//                     }
//                 }
//             }
//
//             protected override void RowGUI(RowGUIArgs args)
//             {
//                 CZTreeViewItem item = args.item as CZTreeViewItem;
//                 if (item == null || item.userData == null)
//                 {
//                     base.RowGUI(args);
//                     return;
//                 }
//
//                 var group = item.userData as AssetBuilderConfig.Group;
//                 if (null != group)
//                 {
//                     var infoRect = args.GetCellRect(0);
//                     if (GUI.Button(infoRect, "...", EditorStyles.label))
//                     {
//                         //Debug.Log("ShowInfo");
//                     }
//
//                     args.rowRect = args.GetCellRect(1);
//                     base.RowGUI(args);
//
//                     GUI.Label(args.GetCellRect(2), Config.resolver.GetAssetTypeIcon(group.assetType), "CenteredLabel");
//
//                     Rect labelsRect = args.GetCellRect(3);
//                     string labels = string.Join(",", group.labels);
//                     labelsRect.yMin += 3;
//                     if (EditorGUI.DropdownButton(labelsRect, EditorGUIUtility.TrTextContent(labels), FocusType.Passive, EditorStyles.toolbarDropDown))
//                     {
//                         PopupWindow.Show(labelsRect, new LabelsSettingView(Config, group.labels, Config.addressableAssetSettings));
//                     }
//
//                     EditorGUI.BeginChangeCheck();
//                     var patternRect = args.GetCellRect(4);
//                     patternRect.yMin += 2;
//                     patternRect.yMax -= 2;
//                     string newPattern = EditorGUI.TextField(patternRect, group.pattern);
//                     if (EditorGUI.EndChangeCheck())
//                     {
//                         Undo.RegisterCompleteObjectUndo(Config, $"Change {group.groupName} Pattern");
//                         group.pattern = newPattern;
//                         EditorUtility.SetDirty(Config);
//                     }
//                     if (string.IsNullOrEmpty(newPattern))
//                     {
//                         EditorGUI.BeginDisabledGroup(true);
//                         GUI.Label(patternRect, "Regex...");
//                         EditorGUI.EndDisabledGroup();
//                     }
//                 }
//                 else if (item.userData is AssetBuilderConfig.Folder folder)
//                 {
//                     var folderItem = item as FolderTreeViewItem;
//                     group = (item.parent as CZTreeViewItem).userData as AssetBuilderConfig.Group;
//
//                     var infoRect = args.GetCellRect(0);
//                     if (GUI.Button(infoRect, "...", EditorStyles.label))
//                     {
//                         Debug.Log("ShowInfo");
//                     }
//
//                     var pathRect = args.GetCellRect(1);
//                     pathRect.xMin += item.depth * depthIndentWidth;
//                     if (folder.folder == null)
//                         EditorGUI.DrawRect(args.rowRect, new Color(1, 0, 0, 0.2f));
//                     else
//                         EditorGUI.DrawRect(args.rowRect, new Color(0, 0, 0, 0.2f));
//                     string path = folderItem.path;
//                     if (!hasSearch)
//                         pathRect.xMin += this.depthIndentWidth;
//                     if (folder.folder == null)
//                         GUI.Label(pathRect, GUIHelper.TextContent(item.displayName, item.icon));
//                     else
//                         GUI.Label(pathRect, GUIHelper.TextContent(path, item.icon));
//
//                     GUI.Label(args.GetCellRect(2), Config.resolver.GetAssetTypeIcon(group.assetType), "CenteredLabel");
//
//                     Rect labelsRect = args.GetCellRect(3);
//                     string labels = string.Join(",", folder.labels);
//                     labelsRect.yMin += 3;
//                     if (EditorGUI.DropdownButton(labelsRect, EditorGUIUtility.TrTextContent(labels), FocusType.Passive, EditorStyles.toolbarDropDown))
//                     {
//                         PopupWindow.Show(labelsRect, new LabelsSettingView(Config, folder.labels, Config.addressableAssetSettings));
//                     }
//
//                     EditorGUI.BeginChangeCheck();
//                     var patternRect = args.GetCellRect(4);
//                     patternRect.yMin += 2;
//                     patternRect.yMax -= 2;
//                     string newPattern = EditorGUI.TextField(patternRect, folder.pattern);
//                     if (EditorGUI.EndChangeCheck())
//                     {
//                         Undo.RegisterCompleteObjectUndo(Config, $"Change {group.groupName} Pattern");
//                         folder.pattern = newPattern;
//                         EditorUtility.SetDirty(Config);
//                     }
//                     if (string.IsNullOrEmpty(newPattern))
//                     {
//                         EditorGUI.BeginDisabledGroup(true);
//                         GUI.Label(patternRect, "Regex...");
//                         EditorGUI.EndDisabledGroup();
//                     }
//                 }
//             }
//
//             protected override void ContextClickedItem(int id)
//             {
//                 var item = FindItem(id);
//                 GenericMenu menu = new GenericMenu();
//                 if (item.userData is AssetBuilderConfig.Group group)
//                 {
//                     menu.AddItem(new GUIContent("Build Group"), false, () =>
//                     {
//                         var groups = new List<AssetBuilderConfig.Group>();
//                         foreach (var selectionID in GetSelection())
//                         {
//                             var selection = FindItem(selectionID);
//                             if (selection.userData is AssetBuilderConfig.Group tempGroup)
//                                 groups.Add(tempGroup);
//                         }
//                         Config.BuildGroups(groups);
//                     });
//                     menu.AddSeparator("");
//                     menu.AddItem(new GUIContent("Rename"), false, () =>
//                     {
//                         this.BeginRename(item);
//                     });
//                 }
//                 menu.AddSeparator("");
//                 menu.AddItem(new GUIContent("Delete"), false, () =>
//                 {
//                     DeleteSelection();
//                 });
//                 menu.ShowAsContext();
//                 base.ContextClickedItem(id);
//             }
//
//             protected override bool CanMultiSelect(TreeViewItem item)
//             {
//                 return true;
//             }
//
//             protected override bool CanRename(TreeViewItem item)
//             {
//                 if (item is CZTreeViewItem mItem && mItem.userData is AssetBuilderConfig.Group)
//                     return true;
//                 return base.CanRename(item);
//             }
//
//             protected override void RenameEnded(RenameEndedArgs args)
//             {
//                 if (!args.acceptedRename)
//                     return;
//                 var item = FindItem(args.itemID);
//                 if (item == null)
//                     return;
//                 if (item.userData is AssetBuilderConfig.Group group)
//                 {
//                     Undo.RegisterCompleteObjectUndo(Config, "Rename Group");
//                     var successed = Config.RenameGroup(group.groupName, args.newName);
//                     EditorUtility.SetDirty(Config);
//                     if (successed)
//                     {
//                         item.displayName = group.groupName;
//                         Refresh();
//                     }
//                 }
//             }
//
//             protected override bool CanStartDrag(CanStartDragArgs args)
//             {
//                 return true;
//             }
//
//             protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
//             {
//                 DragAndDrop.SetGenericData("AAAAA", args.draggedItemIDs);
//                 DragAndDrop.StartDrag("AAAAA");
//             }
//
//             protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
//             {
//                 if (DragAndDrop.GetGenericData("AAAAA") is IList<int> itemIDs)
//                 {
//                     if (args.performDrop)
//                     {
//                         if (args.parentItem is CZTreeViewItem treeViewItem && treeViewItem.userData is AssetBuilderConfig.Group group)
//                         {
//                             Undo.RegisterCompleteObjectUndo(Config, "Move Folders");
//                             foreach (var id in itemIDs)
//                             {
//                                 if (!(FindItem(id) is FolderTreeViewItem folderItem))
//                                     continue;
//                                 if (!(folderItem.userData is AssetBuilderConfig.Folder folder))
//                                     continue;
//                                 var oldGroup = (folderItem.parent as CZTreeViewItem).userData as AssetBuilderConfig.Group;
//                                 Config.RemoveFolder(oldGroup, folder);
//                                 Config.AddFolder(group, folder);
//                                 EditorUtility.SetDirty(Config);
//                             }
//                             Refresh();
//                         }
//                     }
//                     return DragAndDropVisualMode.Generic;
//                 }
//                 if (DragAndDrop.objectReferences.Length > 0)
//                 {
//                     if (args.performDrop)
//                     {
//                         if (args.parentItem is CZTreeViewItem treeViewItem && treeViewItem.userData is AssetBuilderConfig.Group group)
//                         {
//                             Undo.RegisterCompleteObjectUndo(Config, "Add Folders");
//                             foreach (var obj in DragAndDrop.objectReferences)
//                             {
//                                 if (!AssetDatabase.IsMainAsset(obj))
//                                     continue;
//                                 string path = AssetDatabase.GetAssetPath(obj);
//                                 if (AssetDatabase.IsValidFolder(path))
//                                 {
//                                     Config.AddFolder(group, new AssetBuilderConfig.Folder() { folder = obj });
//                                     EditorUtility.SetDirty(Config);
//                                 }
//                             }
//                             Refresh();
//                         }
//                     }
//                     return DragAndDropVisualMode.Generic;
//                 }
//                 return base.HandleDragAndDrop(args);
//             }
//
//             protected override void KeyEvent()
//             {
//                 base.KeyEvent();
//                 var currentEvent = Event.current;
//                 if (currentEvent.type == EventType.KeyDown)
//                 {
//                     switch (currentEvent.keyCode)
//                     {
//                         case KeyCode.Delete:
//                             {
//                                 if (HasSelection())
//                                 {
//                                     DeleteSelection();
//                                     currentEvent.Use();
//                                 }
//                                 break;
//                             }
//                     }
//                 }
//             }
//
//             protected override void DoubleClickedItem(int id)
//             {
//                 base.DoubleClickedItem(id);
//                 var item = FindItem(id);
//                 if (item == null)
//                     return;
//                 if (item.userData is AssetBuilderConfig.Folder folder && folder.folder != null)
//                 {
//                     EditorGUIUtility.PingObject(folder.folder);
//                     Selection.activeObject = folder.folder;
//                 }
//             }
//
//             public void Refresh()
//             {
//                 Clear();
//
//                 foreach (var group in Config.Groups)
//                 {
//                     AddMenuItem(group.groupName, new CZTreeViewItem() { userData = group });
//                     foreach (var folder in group.folders)
//                     {
//                         if (folder == null)
//                             continue;
//                         if (folder.folder == null)
//                             AddMenuItem(group.groupName + "/Missing", new FolderTreeViewItem() { userData = folder, path = "Missing", icon = EditorGUIUtility.FindTexture("Folder Icon") });
//                         else
//                             AddMenuItem(group.groupName + "/" + folder.folder.name, new FolderTreeViewItem() { userData = folder, path = AssetDatabase.GetAssetPath(folder.folder), icon = EditorGUIUtility.FindTexture("Folder Icon") });
//                     }
//                 }
//                 Reload();
//             }
//
//             public void AddGroup(AssetBuilderConfig.Group group)
//             {
//                 var item = new CZTreeViewItem() { userData = group };
//                 AddMenuItem(group.groupName, item);
//                 SetSelection(new int[] { item.id });
//                 Reload();
//                 FrameItem(item.id);
//             }
//
//             public void DeleteSelection()
//             {
//                 bool deleted = false;
//                 foreach (var selectionID in GetSelection())
//                 {
//                     var item = FindItem(selectionID);
//                     if (item == null)
//                         continue;
//                     if (item.userData is AssetBuilderConfig.Group group)
//                     {
//                         Undo.RegisterCompleteObjectUndo(Config, "Remove Group");
//                         Config.RemoveGroup(group);
//                         EditorUtility.SetDirty(Config);
//                         deleted = true;
//                     }
//                     else if (item.userData is AssetBuilderConfig.Folder folder)
//                     {
//                         var parent = item.parent as CZTreeViewItem;
//                         var tempGroup = parent.userData as AssetBuilderConfig.Group;
//                         Undo.RegisterCompleteObjectUndo(Config, "Remove Folder");
//                         Config.RemoveFolder(tempGroup, folder);
//                         EditorUtility.SetDirty(Config);
//                         deleted = true;
//                     }
//                 }
//                 if (deleted)
//                     Refresh();
//             }
//         }
//
//         public class FolderTreeViewItem : CZTreeViewItem
//         {
//             public string path;
//         }
//     }
// }