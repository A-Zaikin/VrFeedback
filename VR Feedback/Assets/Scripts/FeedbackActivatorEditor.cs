using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(FeedbackActivator))]
[CanEditMultipleObjects]
public class FeedbackActivatorEditor : Editor
{
    SerializedProperty feedbackSource;
    SerializedProperty onTriggerEnter;
    SerializedProperty onTriggerExit;
    SerializedProperty onColliderStay;
    private bool impulseToggle;
    private bool continuousToggle;


    void OnEnable()
    {
        feedbackSource = serializedObject.FindProperty("feedbackSource");
        onTriggerEnter = serializedObject.FindProperty("onTriggerEnter");
        onTriggerExit = serializedObject.FindProperty("onTriggerExit");
        onColliderStay = serializedObject.FindProperty("onColliderStay");
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(feedbackSource);

        if (((FeedbackActivator)target).CurrentFeedbackSource != null)
        {
            impulseToggle = ((FeedbackActivator)target).CurrentFeedbackSource.CurrentMode == FeedbackSource.Mode.Impulse;
            continuousToggle = ((FeedbackActivator)target).CurrentFeedbackSource.CurrentMode == FeedbackSource.Mode.Continuous;
        }
        

        
        impulseToggle = EditorGUILayout.BeginToggleGroup("Impulse Mode", impulseToggle);
        if (impulseToggle)
        {
            EditorGUILayout.PropertyField(onTriggerEnter);
            EditorGUILayout.PropertyField(onTriggerExit);
        }
        EditorGUILayout.EndToggleGroup();

        continuousToggle = EditorGUILayout.BeginToggleGroup("Continuous Mode", continuousToggle);
        if (continuousToggle)
        {
            EditorGUILayout.PropertyField(onColliderStay);
        }
        EditorGUILayout.EndToggleGroup();
        

        serializedObject.ApplyModifiedProperties();
        
    }
}
