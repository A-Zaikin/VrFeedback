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

    [SerializeField] private int framesPerHapticCall;
    [SerializeField] private float timeBetweenHapticCalls;

    private List<Vibration> currentLeftVibrations;
    private List<Vibration> currentRightVibrations;
    private int framesSinceLastHapticCall;

    public static FeedbackManager Instance { get; private set; }

    public (float, float) CurrentVibrationAmplitudes { get; private set; }

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

    public void SendHapticImpulseToCollider(Collider collider, float amplitude, float duration)
    {
        var newVibration = new Vibration(amplitude, duration, Time.time);
        if (collider == leftHandCollider)
        {
            currentLeftVibrations.Add(newVibration);
        }
        if (collider == rightHandCollider)
        {
            currentRightVibrations.Add(newVibration);
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
        if (framesSinceLastHapticCall >= framesPerHapticCall || framesPerHapticCall == 0)
        {
            RemoveEndedVibrations(currentLeftVibrations);
            RemoveEndedVibrations(currentRightVibrations);

            //var currentLeftVibrations = this.currentLeftVibrations.ToList();
            //var currentRightVibrations = this.currentRightVibrations.ToList();

            var duration = framesPerHapticCall == 0
                ? timeBetweenHapticCalls / 1000
                : Time.deltaTime * framesPerHapticCall;
            var leftMaxAmplitude = currentLeftVibrations.Count > 0
                ? currentLeftVibrations.Max(vibration => vibration.Amplitude)
                : 0;
            var rightMaxAmplitude = currentRightVibrations.Count > 0
                ? currentRightVibrations.Max(vibration => vibration.Amplitude)
                : 0;
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
