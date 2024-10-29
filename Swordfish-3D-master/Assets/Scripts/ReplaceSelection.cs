using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ReplaceSelection : ScriptableWizard 
{
	static GameObject replacement = null;
	static bool keep = false;
 
	public GameObject ReplacementObject = null;
	public bool KeepOriginals = false;
 
	[MenuItem("GameObject/-Replace Selection...")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard(
			"Replace Selection", typeof(ReplaceSelection), "Replace");
	}
 
	public ReplaceSelection()
	{
		ReplacementObject = replacement;
		KeepOriginals = keep;
	}
 
	void OnWizardUpdate()
	{
		replacement = ReplacementObject;
		keep = KeepOriginals;
	}
 
	void OnWizardCreate()
	{
		if (replacement == null)
			return;
 
		Undo.RegisterSceneUndo("Replace Selection");
 
		Transform[] transforms = Selection.GetTransforms(
			SelectionMode.TopLevel | SelectionMode.Editable);
 
		foreach (Transform t in transforms)
		{
			GameObject g;
			var pref = EditorUtility.GetPrefabType(replacement);
			
			g = (GameObject)EditorUtility.InstantiatePrefab(replacement);
 
			Transform gTransform = g.transform;
			gTransform.parent = t.parent;
			g.name = replacement.name;
			gTransform.localPosition = t.localPosition;
			gTransform.localScale = t.localScale;
			gTransform.localRotation = t.localRotation;
		}
 
		if (!keep)
		{
			foreach (GameObject g in Selection.gameObjects)
			{
				GameObject.DestroyImmediate(g);
			}
		}
	}
}
#endif