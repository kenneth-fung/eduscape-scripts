using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This class contains a number of utility methods for other classes that execute in the editor.
/// </summary>
public static class EditorUtil {

    /// <summary>
    /// Converts the given string array to a single string path.
    /// </summary>
    /// <param name="path">The string array to convert.</param>
    /// <returns>The single string path, with each level separated by a slash.</returns>
    public static string ConvertPathArrToString(string[] path) {
        return string.Join("/", path);
    }

    /// <summary>
    /// Returns a string array containing the names of the given object's components.
    /// </summary>
    /// <param name="obj">The GameObject whose components' names are required.</param>
    public static string[] GetGameObjectComponentNames(GameObject obj) {
        Component[] components = obj.GetComponents<Component>();
        return System.Array.ConvertAll(components, component => component.GetType().Name);
    }

    /// <summary>
    /// Returns the project's hierarchy window.
    /// </summary>
    /// <returns>The hierarchy window as an EditorWindow.</returns>
    public static EditorWindow GetProjectHierarchyWindow() {
        System.Type hierarchyType = System.Type.GetType("UnityEditor.SceneHierarchyWindow, UnityEditor");
        return (EditorWindow)Resources.FindObjectsOfTypeAll(hierarchyType)[0];
    }

    /// <summary>
    /// Returns a list of the given object's (direct) child objects.
    /// </summary>
    /// <param name="obj">The GameObject whose direct children are required.</param>
    /// <returns>A GameObject array containing the child objects.</returns>
    public static GameObject[] GetChildrenOfGameObject(GameObject obj) {
        GameObject[] children = new GameObject[obj.transform.childCount];
        for (int i = 0; i < children.Length; i++) {
            children[i] = obj.transform.GetChild(i).gameObject;
        }

        return children;
    }

    /// <summary>
    /// Returns the path to an object.
    /// </summary>
    /// <param name="obj">The GameObject whose path is required.</param>
    /// <returns>The path in a string array, starting with the parent at the root level.</returns>
    public static string[] GetPathToObject(GameObject obj) {
        List<string> pathToObj = new List<string>();

        Transform parent = obj.transform.parent;
        while (parent != null) {
            pathToObj.Insert(0, parent.name);
            parent = parent.transform.parent;
        }

        return pathToObj.ToArray();
    }

    /// <summary>
    /// Returns the GameObject at the end of the given target path.
    /// </summary>
    /// <param name="objectsOnThisLevel">The objects on the current level of the hierarchy.</param>
    /// <param name="levelInHierarchy">The current level in the hierarchy.</param>
    /// <param name="targetPath">The path to the target object.</param>
    /// <returns>The found GameObject.</returns>
    public static GameObject FindTargetObjectInScene(GameObject[] objectsOnThisLevel, int levelInHierarchy, string[] targetPath) {
        bool isEndOfPath = levelInHierarchy >= targetPath.Length - 1;
        foreach (GameObject obj in objectsOnThisLevel) {
            if (obj.name == targetPath[levelInHierarchy]) {
                if (isEndOfPath) {
                    // the current level in the hierarchy matches the target object's level
                    return obj;
                }

                // continue search with object's children in next level
                return FindTargetObjectInScene(GetChildrenOfGameObject(obj), levelInHierarchy + 1, targetPath);
            }
        }

        Debug.Log($"GameObject {targetPath[targetPath.Length - 1]} not found among the given objects.");
        return null;
    }
}
