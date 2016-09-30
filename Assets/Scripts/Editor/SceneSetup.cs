using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoad]
public class SceneSetup {

	static SceneSetup() {
		EditorApplication.hierarchyWindowChanged += freshInitialize;
	}

	private static void setup() {
		// create player
		Player player = createPrefab("Assets/Prefabs/Player.prefab").GetComponent<Player>();

		// create alien
		player.entity = createPrefab("Assets/Prefabs/Alien.prefab").GetComponent<Alien>();

		// set lighting mode
		Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
	}

	private static void freshInitialize() {
		// check that the level is fresh
		GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
		if (gameObjects.Length != 1)
			return;
		List<GameObject> objectList = new List<GameObject>(gameObjects);
		if (objectList.Find(go => go.name == "Main Camera") == null)
			return;

		// delete default objects
		foreach (GameObject ob in gameObjects)
			GameObject.DestroyImmediate(ob);

		// create prefabs and do configuration
		setup();
	}

	private static GameObject createPrefab(string assetPath, Transform parent=null) {
		GameObject prefab = PrefabUtility.InstantiatePrefab(
			AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)) as GameObject;
		prefab.transform.parent = parent;
		return prefab;
	}
}
