using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.ShaderKeywordFilter;
using UnityEditor.UIElements;
using System.Reflection;


using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class ProjectBookmark : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Tools/ProjectBookmark #B")]
    public static void ShowExample()
    {
        ProjectBookmark wnd = GetWindow<ProjectBookmark>();
        wnd.titleContent = new GUIContent("ProjectBookmark");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        

        //ツールバーの設定
        var toolbar = new Toolbar();
        var btn1 = new ToolbarButton { text = "更新" };
        toolbar.Add(btn1);

        btn1.clicked += () =>
        {
            ReloadBookmark();
        };

        var btn2 = new ToolbarButton { text = "Help" };
        toolbar.Add(btn2);

        root.Add(toolbar);

        
        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        
        ReloadBookmark();
    }



    //サポート関数--------------------------------------------------------------------------------------------


    public VisualElement LoadAsset()
    {
        var packageBasePath = Regex.Match(AssetDatabase.GetAssetPath(m_VisualTreeAsset), "(.+" + Regex.Escape("/") + ").*?" + Regex.Escape(".") + ".*?$").Groups[1].Value;
        return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(packageBasePath + "BookmarkRow.uxml").Instantiate();
    }


    public void ReloadBookmark()
    {
        var scrollView = rootVisualElement.Query<ScrollView>().First();

        while(scrollView.childCount != 0)
        {
            scrollView.RemoveAt(0);
        }

        //Bookmarkdata data = LoadSetting();

        string[] targetguids = UnityEditor.AssetDatabase.FindAssets("t:BookmarkObject");
        foreach (var guid in targetguids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<BookmarkObject>(path);
            
            VisualElement element = LoadAsset();
            element.Query<Label>("BookmarkName").First().text = obj.BookmarkName;
            element.Query<Label>("BookmarkLink").First().text = path;

            element.Query("BookmarkRow").First().RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
            element.Query("BookmarkRow").First().RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
            element.Query("BookmarkRow").First().RegisterCallback<MouseDownEvent>((d) => 
            {
                BookmarkObject target= AssetDatabase.LoadAssetAtPath<BookmarkObject>(path);
                Selection.objects = new UnityEngine.Object[] { target };
                EditorGUIUtility.PingObject(target);
            });

            rootVisualElement.Query<ScrollView>().First().Add(element);
            
        }
    }

    void OnMouseEnterEvent(MouseEventBase<MouseEnterEvent> evt)
    {
        ((VisualElement)evt.target).style.backgroundColor = new StyleColor(new Color32(150, 150, 150, 255));
        
    }

    void OnMouseLeaveEvent(MouseEventBase<MouseLeaveEvent> evt)
    {
        ((VisualElement)evt.target).style.backgroundColor = new StyleColor(new Color32(97, 97, 97, 255));
    }




}
