using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UINavigation.Editor
{
    //[CustomEditor(typeof(UINavigationComponent))]
    //public class UINavigationComponentEditor : UnityEditor.Editor
    //{
    //    private VisualElement root;

    //    private VisualTreeAsset visualTree;
    //    private VisualTreeAsset clickListElementTree;

    //    private void OnEnable()
    //    {
    //        root = new VisualElement();

    //        visualTree = Resources.Load<VisualTreeAsset>("UINavComEditor.uxml");
    //        clickListElementTree = Resources.Load<VisualTreeAsset>("ClickEvtEle_UINavComEditor.uxml");
    //    }

    //    public override VisualElement CreateInspectorGUI()
    //    {
    //        root.Clear();

    //        root = visualTree.CloneTree();

    //        return root;
    //    }

    //    private void InitData()
    //    {
    //        UINavigationComponent component = target as UINavigationComponent;

    //        var sourceTreeField = root.Q<ObjectField>(name:"sourcetree-field");
    //        sourceTreeField.BindProperty(serializedObject.FindProperty("navigationGraph"));

    //        var addButton = root.Q<Button>(name: "add-button");
    //        var clickEventContainer = root.Q(name: "clickevent-container");

    //        foreach()
    //    }
    //} 
}
