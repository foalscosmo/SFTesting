using Car;
using MyEditor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CarBuilder))]
public class CarBuilderEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Build Car", EditorStyles.miniButton))
		{
			((CarBuilder)this.target).BuildCar();
		}
		if (GUILayout.Button("Destroy Car", EditorStyles.miniButton))
		{
			((CarBuilder)this.target).DestroyCar();
		}
		DrawDefaultInspector ();
	}
}

[CustomEditor(typeof(SWController))]
public class SWControllerEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		/*if (GUILayout.Button("Prepare Assets", EditorStyles.miniButton))
		{
			((SWController)this.target).PrepareAssets();
		}*/
		if (GUILayout.Button("Build For Shop", EditorStyles.miniButton))
		{
			((SWController)this.target).ShopTest();
		}
		DrawDefaultInspector ();
	}
}

[CustomEditor(typeof(SWBuilderForEditor))]
public class SWBuilderForEditorEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		/*if (GUILayout.Button("Prepare Assets", EditorStyles.miniButton))
		{
			((SWController)this.target).PrepareAssets();
		}*/
		if (GUILayout.Button("Build", EditorStyles.miniButton))
		{
			((SWBuilderForEditor)this.target).PrepareParts();
		}
		if (GUILayout.Button("Build All", EditorStyles.miniButton))
		{
			((SWBuilderForEditor)this.target).PrepareAllParts();
		}
		if (GUILayout.Button("ScreenShot", EditorStyles.miniButton))
		{
			((SWBuilderForEditor)this.target).TakeScreenShot();
		}
		DrawDefaultInspector ();
	}
}
