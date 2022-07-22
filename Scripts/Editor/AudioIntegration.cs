using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// This class is responsible for integrating audio work from multiple developers.
/// </summary>
public class AudioIntegration : MonoBehaviour {

    /// <summary>
    /// The text file where errors will be logged.
    /// </summary>
    private static readonly string copySfxSourcesErrorsFilePath = Application.dataPath + "/Logs/Copy SFX Source Errors.txt";

    /// <summary>
    /// Copies Sound FX Source prefab instances from other scenes' hierarchies to the current scene's hierarchy.
    /// </summary>
    /// <remarks>
    /// The placement path of a copied object is maintained only if an identically named hierarchy of objects can be found in the current scene.
    /// If one cannot be found, the copied object is left as a child object of the root-level GameObject <c>Duplicated SFX Sources from {scene name}</c> in the current scene.
    /// </remarks>
    [MenuItem("Audio/Copy Sound Sources From Other Scenes to the Current Active Scene")]
    private static void MergeSoundSourcesIntoActiveScene() {
        Scene targetScene = SceneManager.GetActiveScene();

        // check if the user is sure about executing this action
        if (!EditorUtility.DisplayDialog(
            "Copying SFX Source Objects to Active Scene",
            $"SFX Source objects are about to be copied from other scenes to the current active scene ({targetScene.name}). Are you sure you want to do this?",
            "Yes", "No")) {
            return;
        }

        // create/reset the error log file
        System.IO.File.WriteAllText(copySfxSourcesErrorsFilePath, "");

        // the scenes must be included in the project's build settings
        if (SceneManager.sceneCountInBuildSettings == 0) {
            Debug.LogWarning($"There are no scenes in build settings!");
            return;
        }

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            Scene scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path, OpenSceneMode.Additive);

            // we ignore the target scene; the names of any other scenes that should be ignored can be added here
            if (scene.name == targetScene.name) {
                continue;
            }

            SceneManager.SetActiveScene(scene);

            // find all SFX Source objects in the scene and store them in a dictionary with their paths as the keys
            Dictionary<string[], SoundFxSource> pathToSfxSourceDict = new Dictionary<string[], SoundFxSource>();
            FindSoundFxSourcesInScene(scene.GetRootGameObjects(), pathToSfxSourceDict);

            // select all the SFX Source objects in the scene hierarchy
            Selection.objects = new List<SoundFxSource>(pathToSfxSourceDict.Values).ConvertAll<Object>(sfxSource => sfxSource.gameObject).ToArray();
            if (Selection.objects.Length == 0) {
                EditorSceneManager.CloseScene(scene, true);
                continue;
            }

            // create an empty root GameObject to store and move over the duplicated SFX Source objects
            GameObject toMove = new GameObject($"Duplicated SFX Sources from {scene.name}");

            // duplicate the SFX Source objects in the scene hierarchy, including their prefab overrides and child objects
            EditorUtil.GetProjectHierarchyWindow().SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));

            // the selected objects are now the duplicated objects; make them the children of the mover
            System.Array.ForEach(Selection.gameObjects, duplicatedSfxSourceObj => duplicatedSfxSourceObj.transform.parent = toMove.transform);

            SceneManager.MoveGameObjectToScene(toMove, targetScene);

            PlaceDuplicatedSfxSourceObjsInPaths(targetScene, EditorUtil.GetChildrenOfGameObject(toMove), pathToSfxSourceDict);

            if (toMove.transform.childCount == 0) {
                // all duplicated SFX Source objects placed in target paths successfully
                DestroyImmediate(toMove, true);
            }

            EditorSceneManager.SaveScene(targetScene, targetScene.path);
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    /// <summary>
    /// Finds all objects with the SoundFxSource component in the given array of GameObjects.
    /// Each GameObject can have zero or more child GameObjects.
    /// </summary>
    /// <param name="objects">The array of GameObjects to search.</param>
    /// <param name="pathToSfxSourceDict">The dictionary in which to store the found objects and their paths.</param>
    private static void FindSoundFxSourcesInScene(GameObject[] objects, Dictionary<string[], SoundFxSource> pathToSfxSourceDict) {
        foreach (GameObject obj in objects) {
            if (obj.GetComponent<SoundFxSource>()) {
                pathToSfxSourceDict.Add(EditorUtil.GetPathToObject(obj), obj.GetComponent<SoundFxSource>());
            }

            // continue search with object's children
            FindSoundFxSourcesInScene(EditorUtil.GetChildrenOfGameObject(obj), pathToSfxSourceDict);
        }
    }

    /// <summary>
    /// Places the given duplicated Sound FX Source objects into the given target scene.
    /// </summary>
    /// <param name="targetScene">The scene to place the objects in.</param>
    /// <param name="duplicatedSfxSourceObjects">The objects to place.</param>
    /// <param name="pathToSfxSourceDict">The dictionary containing the intended paths of the objects.</param>
    private static void PlaceDuplicatedSfxSourceObjsInPaths(
        Scene targetScene, GameObject[] duplicatedSfxSourceObjects, Dictionary<string[], SoundFxSource> pathToSfxSourceDict) {

        List<string[]> pathList = new List<string[]>(pathToSfxSourceDict.Keys);
        bool areErrorsPresent = false;
        for (int j = 0; j < duplicatedSfxSourceObjects.Length; j++) {
            // find the target parents of the duplicated SFX Source objects based on the paths from the original scene
            string[] path = pathList[j];
            GameObject parent = EditorUtil.FindTargetObjectInScene(targetScene.GetRootGameObjects(), 0, path);
            GameObject duplicatedObj = duplicatedSfxSourceObjects[j];

            duplicatedObj.name = pathToSfxSourceDict[path].name;

            if (parent == null) {
                // can't find an appropriate target object
                areErrorsPresent = true;
                duplicatedObj.name = "Parent Not Found for: " + duplicatedObj.name;
                System.IO.File.AppendAllText(
                    copySfxSourcesErrorsFilePath,
                    $"\n{duplicatedObj.name}\nAttempted Path: {EditorUtil.ConvertPathArrToString(path)}\n");
                continue;
            }

            GameObject potentialExistingSfxSourceChildOnParent = System.Array.Find(
                EditorUtil.GetChildrenOfGameObject(parent),
                child => child.name == duplicatedObj.name);

            if (potentialExistingSfxSourceChildOnParent != null && potentialExistingSfxSourceChildOnParent.GetComponent<SoundFxSource>()) {
                // target parent object already has a child object that matches the duplicated
                areErrorsPresent = true;
                duplicatedObj.name = "Existing SFX Source Found for: " + duplicatedObj.name;
                System.IO.File.AppendAllText(
                    copySfxSourcesErrorsFilePath,
                    $"\n{duplicatedObj.name}\nExisting Object: {EditorUtil.ConvertPathArrToString(path)}/{potentialExistingSfxSourceChildOnParent.name}\n");
                continue;
            }

            duplicatedObj.transform.parent = parent.transform;
        }

        if (areErrorsPresent) {
            Debug.LogWarning($"Errors encountered during SFX Source copying. See log file at:\n{copySfxSourcesErrorsFilePath}");
        }
    }
}
