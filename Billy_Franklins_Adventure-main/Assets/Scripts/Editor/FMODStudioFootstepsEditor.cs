
//using UnityEditor;

//[CustomEditor(typeof(FMODStudioMaterialSetter))]
//public class FMODStudioFootstepsEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        var MS = target as FMODStudioMaterialSetter;
//        var fpf = FindObjectOfType<FMODStudioFootstepScript>();

//        MS.MaterialValue = EditorGUILayout.Popup("Set Material As", MS.MaterialValue, fpf.MaterialTypes);
//    }
//}

//[CustomEditor(typeof(FMODStudioFootstepScript))]
//public class FMODStudioFootstepsEditorTwo : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        var FPF = target as FMODStudioFootstepScript;

//        FPF.DefulatMaterialValue = EditorGUILayout.Popup("Set Default Material As", FPF.DefulatMaterialValue, FPF.MaterialTypes);
//    }
//}

