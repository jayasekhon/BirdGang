using UnityEngine;
using UnityEditor;
public class ReplaceWithPrefab : EditorWindow
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 scale = new Vector3(.12f, .2f, .12f);
    [MenuItem("Tools/Replace With Prefab")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ReplaceWithPrefab>();
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        scale = EditorGUILayout.Vector3Field("BaseScale", scale);
        var newScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
        if (GUILayout.Button("Replace"))
        {
            var selection = Selection.gameObjects;
            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];
                var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
                GameObject newObject;
                if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                }
                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }
                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                var s = selected.transform.localScale;
                s.Scale(newScale);
                newObject.transform.localScale = s;
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                Undo.DestroyObjectImmediate(selected);
            }
        }
        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}
