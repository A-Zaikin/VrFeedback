using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private Collider leftHandCollider;
    [SerializeField] private ActionBasedController rightController;
    [SerializeField] private Collider rightHandCollider;

    [SerializeField] private float sensitivity;
    [SerializeField] private int framesPerHapticCall;
    [SerializeField] private float msBetweenHapticCalls;
    [SerializeField] private float discreteFunctionStep;

    private List<Vibration> currentLeftVibrations;
    private List<Vibration> currentRightVibrations;
    private int framesSinceLastHapticCall;

    public static FeedbackManager Instance { get; private set; }

    public (float, float) CurrentVibrationAmplitudes { get; private set; }

    public float DiscreteFunctionStep { get => discreteFunctionStep; }

    public void SendHapticImpulse(Controllers controller, float amplitude, float duration)
    {
        var newVibration = new Vibration(amplitude, duration, Time.time);
        if (controller == Controllers.Left || controller == Controllers.Both)
        {
            currentLeftVibrations.Add(newVibration);
        }
        if (controller == Controllers.Right || controller == Controllers.Both)
        {
            currentRightVibrations.Add(newVibration);
        }
    }

    public bool TryGetController(Collider collider, out Controllers controller)
    {
        controller = Controllers.Left;
        if (collider == leftHandCollider)
        {
            return true;
        }
        else if (collider == rightHandCollider)
        {
            controller = Controllers.Right;
            return true;
        }
        return false;
    }

    public Vector3 GetPosition(Controllers controller)
    {
        if (controller == Controllers.Left)
        {
            return leftHandCollider.transform.position;
        }
        else
        {
            return rightHandCollider.transform.position;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentLeftVibrations = new List<Vibration>();
        currentRightVibrations = new List<Vibration>();
    }

    private void Update()
    {
        framesSinceLastHapticCall++;
        if (framesSinceLastHapticCall >= framesPerHapticCall)
        {
            RemoveEndedVibrations(currentLeftVibrations);
            RemoveEndedVibrations(currentRightVibrations);

            //var currentLeftVibrations = this.currentLeftVibrations.ToList();
            //var currentRightVibrations = this.currentRightVibrations.ToList();

            var duration = msBetweenHapticCalls < 0
                ? Time.deltaTime * framesPerHapticCall
                : msBetweenHapticCalls / 1000;
            var leftMaxAmplitude = currentLeftVibrations.Count > 0
                ? currentLeftVibrations.Max(vibration => vibration.Amplitude)
                : 0;
            var rightMaxAmplitude = currentRightVibrations.Count > 0
                ? currentRightVibrations.Max(vibration => vibration.Amplitude)
                : 0;
            leftMaxAmplitude = Mathf.Min(leftMaxAmplitude * sensitivity, 1);
            rightMaxAmplitude = Mathf.Min(rightMaxAmplitude * sensitivity, 1);
            CurrentVibrationAmplitudes = (leftMaxAmplitude, rightMaxAmplitude);

            leftController.SendHapticImpulse(leftMaxAmplitude, duration);
            rightController.SendHapticImpulse(rightMaxAmplitude, duration);

            framesSinceLastHapticCall = 0;
        }


        void RemoveEndedVibrations(List<Vibration> currentVibrations)
        {
            for (var i = 0; i < currentVibrations.Count; i++)
            {
                if (Time.time - currentVibrations[i].StartTime > currentVibrations[i].Duration)
                {
                    currentVibrations.Remove(currentVibrations[i]);
                }
            }
        }
    }

    public enum Controllers
    {
        None,
        Left,
        Right,
        Both
    }

    private struct Vibration
    {
        public float Amplitude;
        public float Duration;
        public float StartTime;

        public Vibration(float amplitude, float duration, float startTime)
        {
            Amplitude = amplitude;
            Duration = duration;
            StartTime = startTime;
        }
    }
}
