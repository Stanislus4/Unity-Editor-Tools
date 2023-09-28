using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
public class SetParentEditorWindow : EditorWindow
{
	public GameObject targetParent;

	[MenuItem("Tools/Editor Tools/Set Parent")]
	public static void ShowWindow()
	{
		GetWindow<SetParentEditorWindow>("Set Parent");
	}

	private void OnGUI()
	{
		GUILayout.Label("Set Parent", EditorStyles.boldLabel);

		targetParent = EditorGUILayout.ObjectField("Target Parent:", targetParent, typeof(GameObject), true) as GameObject;

		if (GUILayout.Button("Set Parent"))
		{
			Transform[] selectedTransforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable);

			if (selectedTransforms.Length == 0)
			{
				Debug.LogWarning("No GameObject selected. Please select a GameObject in the Hierarchy.");
				return;
			}

			foreach (Transform selectedTransform in selectedTransforms)
			{
				Undo.RecordObject(selectedTransform, "Set Parent");
				selectedTransform.SetParent(targetParent != null ? targetParent.transform : null);
			}
		}
	}
}

public class ReplaceWithPrefab : EditorWindow
{
	private GameObject prefab;

	[MenuItem("Tools/Editor Tools/Replace Selected Objects With Prefab")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(ReplaceWithPrefab));
	}

	private void OnGUI()
	{
		prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

		if (GUILayout.Button("Replace"))
		{
			ReplaceSelectedObjects();
		}
	}

	private void ReplaceSelectedObjects()
	{
		if (Selection.gameObjects.Length == 0)
		{
			Debug.LogWarning("No objects selected.");
			return;
		}

		foreach (var selectedObject in Selection.gameObjects)
		{
			GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			newObject.transform.parent = selectedObject.transform.parent;
			newObject.transform.position = selectedObject.transform.position;
			newObject.transform.rotation = selectedObject.transform.rotation;
			newObject.transform.localScale = selectedObject.transform.localScale;
			Undo.RegisterCreatedObjectUndo(newObject, "Replace with Prefab");
			Undo.DestroyObjectImmediate(selectedObject);
		}
	}
}

public class RenameObjectsFromMesh : ScriptableObject
{
	[MenuItem("Tools/Editor Tools/Rename Objects from MeshFilter")]
	private static void RenameObjects()
	{
		// Get all selected game objects
		GameObject[] selectedObjects = Selection.gameObjects;

		foreach (GameObject obj in selectedObjects)
		{
			// Get the MeshFilter component
			MeshFilter meshFilter = obj.GetComponent<MeshFilter>();

			if (meshFilter != null && meshFilter.sharedMesh != null)
			{
				// Rename the object using the mesh name
				obj.name = meshFilter.sharedMesh.name;
			}
		}
	}
}

public class AlphabeticalSort
{
	[MenuItem("Tools/Editor Tools/Sort Children By Name")]

	public static void SortChildrenByName()
	{
		foreach (Transform t in Selection.transforms)
		{
			List<Transform> children = t.Cast<Transform>().ToList();
			children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
			for (int i = 0; i < children.Count; ++i)
			{
				Undo.SetTransformParent(children[i], children[i].parent, "Sort Children");
				children[i].SetSiblingIndex(i);
			}
		}
	}

}

public class SetRandomRotationEditorWindow : EditorWindow
{
	private Vector3 randomRotationAxis;
	private bool randomX = false;
	private bool randomY = false;
	private bool randomZ = false;

	private float minAngleX = 0f;
	private float maxAngleX = 360f;
	private float minAngleY = 0f;
	private float maxAngleY = 360f;
	private float minAngleZ = 0f;
	private float maxAngleZ = 360f;

	[MenuItem("Tools/Editor Tools/Set Random Rotation")]
	private static void ShowWindow()
	{
		SetRandomRotationEditorWindow window = GetWindow<SetRandomRotationEditorWindow>("Set Random Rotation");
		window.minSize = new Vector2(250f, 250f);
	}

	private void OnGUI()
	{
		GUILayout.Label("Select Rotation Axis:", EditorStyles.boldLabel);

		randomX = EditorGUILayout.Toggle("X Axis", randomX);
		if (randomX)
		{
			minAngleX = EditorGUILayout.FloatField("Min Angle (X)", minAngleX);
			maxAngleX = EditorGUILayout.FloatField("Max Angle (X)", maxAngleX);
		}

		randomY = EditorGUILayout.Toggle("Y Axis", randomY);
		if (randomY)
		{
			minAngleY = EditorGUILayout.FloatField("Min Angle (Y)", minAngleY);
			maxAngleY = EditorGUILayout.FloatField("Max Angle (Y)", maxAngleY);
		}

		randomZ = EditorGUILayout.Toggle("Z Axis", randomZ);
		if (randomZ)
		{
			minAngleZ = EditorGUILayout.FloatField("Min Angle (Z)", minAngleZ);
			maxAngleZ = EditorGUILayout.FloatField("Max Angle (Z)", maxAngleZ);
		}

		if (GUILayout.Button("Apply Random Rotation"))
		{
			ApplyRandomRotationToSelectedObjects();
		}
	}

	private void ApplyRandomRotationToSelectedObjects()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		foreach (GameObject selectedObject in selectedObjects)
		{
			// Get the transform of the selected object and apply random rotation as needed.
			Transform selectedTransform = selectedObject.transform;

			float randomAngleX = randomX ? Random.Range(minAngleX, maxAngleX) : 0f;
			float randomAngleY = randomY ? Random.Range(minAngleY, maxAngleY) : 0f;
			float randomAngleZ = randomZ ? Random.Range(minAngleZ, maxAngleZ) : 0f;

			selectedTransform.Rotate(Vector3.right, randomAngleX);
			selectedTransform.Rotate(Vector3.up, randomAngleY);
			selectedTransform.Rotate(Vector3.forward, randomAngleZ);
		}

		Debug.Log("Random rotation applied to selected objects.");
	}
}

#endif