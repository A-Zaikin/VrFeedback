using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FeedbackSource))]
[CanEditMultipleObjects]
public class FeedbackSourceEditor : Editor
{
    SerializedProperty modeProperty;
    SerializedProperty impulseMode;
    SerializedProperty continuosMode;
    SerializedProperty amplitudeRollOffOverDistance;
    SerializedProperty continuosModeFrequency;
    SerializedProperty amplitudeRollOffOverVelocity;
    SerializedProperty distanceRollOffCoefficient;
    SerializedProperty velocityRollOffCoefficient;

    SerializedProperty amplitude;
    SerializedProperty duration;
    SerializedProperty targetController;
    bool modeFoldoutGroup;

    void OnEnable()
    {
        modeProperty = serializedObject.FindProperty("mode");
        impulseMode = serializedObject.FindProperty("impulseMode");
        continuosMode = serializedObject.FindProperty("continuosMode");
        amplitudeRollOffOverDistance = serializedObject.FindProperty("amplitudeRollOffOverDistance");
        continuosModeFrequency = serializedObject.FindProperty("continuosModeFrequency");
        amplitudeRollOffOverVelocity = serializedObject.FindProperty("amplitudeRollOffOverVelocity");
        distanceRollOffCoefficient = serializedObject.FindProperty("distanceRollOffCoefficient");
        velocityRollOffCoefficient = serializedObject.FindProperty("velocityRollOffCoefficient");
        amplitude = serializedObject.FindProperty("amplitude");
        duration = serializedObject.FindProperty("duration");
        targetController = serializedObject.FindProperty("targetController");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(amplitude);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(targetController);

        modeFoldoutGroup = EditorGUILayout.Foldout(modeFoldoutGroup, "Mode");
        if (modeFoldoutGroup)
        {
            var level = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(modeProperty);
            if (modeProperty.intValue == (int)FeedbackSource.Mode.Continuos)
            {
                EditorGUILayout.PropertyField(continuosMode);
                if (continuosMode.intValue != (int)FeedbackSource.ContinuosMode.Constant)
                    EditorGUILayout.PropertyField(continuosModeFrequency);

            }
            if (modeProperty.intValue == (int)FeedbackSource.Mode.Impulse)
            {
                EditorGUILayout.PropertyField(impulseMode);
            }
            EditorGUI.indentLevel = level;
        }

        //EditorGUILayout.BeginFoldoutHeaderGroup(true, "Mode");
        //EditorGUILayout.PropertyField(modeProperty);
        //if(modeProperty.intValue == (int)FeedbackSource.Mode.Continuos)
        //{
        //    EditorGUILayout.PropertyField(continuosMode);
        //    if (continuosMode.intValue != (int)FeedbackSource.ContinuosMode.Constant)
        //        EditorGUILayout.PropertyField(continuosModeFrequency);

        //}
        //if(modeProperty.intValue == (int)FeedbackSource.Mode.Impulse)
        //{
        //    EditorGUILayout.PropertyField(impulseMode);
        //}
        //EditorGUILayout.EndFoldoutHeaderGroup();


        EditorGUILayout.PropertyField(amplitudeRollOffOverDistance);
        if(amplitudeRollOffOverDistance.intValue != (int)FeedbackSource.AmplitudeRollOffOverDistance.None)
        {
            EditorGUILayout.PropertyField(distanceRollOffCoefficient);
        }
        EditorGUILayout.PropertyField(amplitudeRollOffOverVelocity);
        if(amplitudeRollOffOverVelocity.intValue != (int)FeedbackSource.AmplitudeRollOffOverVelocity.None)
        {
            EditorGUILayout.PropertyField (velocityRollOffCoefficient);
        }
        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }
}
