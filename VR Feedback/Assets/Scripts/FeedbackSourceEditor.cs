using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FeedbackSource))]
[CanEditMultipleObjects]
public class FeedbackSourceEditor : Editor
{
    SerializedProperty modeProperty;
    SerializedProperty impulseMode;
    SerializedProperty continuousMode;
    SerializedProperty amplitudeOverDistance;
    SerializedProperty continuousModeFrequency;
    SerializedProperty amplitudeOverVelocity;
    SerializedProperty distanceRollOffCoefficient;
    SerializedProperty velocityRollOffCoefficient;

    SerializedProperty amplitude;
    SerializedProperty duration;
    SerializedProperty targetController;
    SerializedProperty discreteFunctionStep;
    bool modeFoldoutGroup = true;

    void OnEnable()
    {
        modeProperty = serializedObject.FindProperty("mode");
        impulseMode = serializedObject.FindProperty("impulseMode");
        continuousMode = serializedObject.FindProperty("continuousMode");
        amplitudeOverDistance = serializedObject.FindProperty("amplitudeOverDistance");
        continuousModeFrequency = serializedObject.FindProperty("continuousModeFrequency");
        amplitudeOverVelocity = serializedObject.FindProperty("amplitudeOverVelocity");
        distanceRollOffCoefficient = serializedObject.FindProperty("distanceRollOffCoefficient");
        velocityRollOffCoefficient = serializedObject.FindProperty("velocityRollOffCoefficient");
        amplitude = serializedObject.FindProperty("amplitude");
        duration = serializedObject.FindProperty("duration");
        targetController = serializedObject.FindProperty("targetController");
        discreteFunctionStep = serializedObject.FindProperty("discreteFunctionStep");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(amplitude);
        EditorGUILayout.PropertyField(targetController);
        EditorGUILayout.PropertyField(discreteFunctionStep);

        modeFoldoutGroup = EditorGUILayout.Foldout(modeFoldoutGroup, "Mode");
        if (modeFoldoutGroup)
        {
            var level = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(modeProperty);
            if (modeProperty.intValue == (int)FeedbackSource.Mode.Continuous)
            {
                EditorGUILayout.PropertyField(continuousMode);
                if (continuousMode.intValue != (int)FeedbackSource.ContinuousMode.Constant)
                    EditorGUILayout.PropertyField(continuousModeFrequency);

            }
            if (modeProperty.intValue == (int)FeedbackSource.Mode.Impulse)
            {
                EditorGUILayout.PropertyField(duration);
                EditorGUILayout.PropertyField(impulseMode);
            }
            EditorGUI.indentLevel = level;
        }


        EditorGUILayout.PropertyField(amplitudeOverDistance);
        if (amplitudeOverDistance.intValue != (int)FeedbackSource.AmplitudeOverDistance.None)
        {
            EditorGUILayout.PropertyField(distanceRollOffCoefficient);
        }
        EditorGUILayout.PropertyField(amplitudeOverVelocity);
        if (amplitudeOverVelocity.intValue != (int)FeedbackSource.AmplitudeOverVelocity.None)
        {
            EditorGUILayout.PropertyField(velocityRollOffCoefficient);
        }
        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }
}
