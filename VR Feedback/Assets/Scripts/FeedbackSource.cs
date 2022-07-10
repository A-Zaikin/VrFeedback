using System.Collections;

using UnityEngine;

public class FeedbackSource : MonoBehaviour
{
    [SerializeField] private Mode mode;
    [SerializeField] private ImpulseMode impulseMode;
    [SerializeField] private ContinuousMode continuousMode;
    [SerializeField] private float continuousModeFrequency;

    [SerializeField] private AmplitudeOverDistance amplitudeOverDistance;
    [SerializeField] private float distanceRollOffCoefficient;
    [SerializeField] private AmplitudeOverVelocity amplitudeOverVelocity;
    [SerializeField] private float velocityRollOffCoefficient;
    
    [SerializeField] private float amplitude;
    [SerializeField] private float duration;
    [SerializeField] private TargetControllers targetController;
    [SerializeField] private float discreteFunctionStep;

    private Coroutine continuousCoroutine;

    public void SendFeedback(Collider collider = null)
    {
        FeedbackManager.Controllers controller;
        if (targetController == TargetControllers.BasedOnCollider && collider != null)
        {
            if (!FeedbackManager.Instance.TryGetController(collider, out var recievedController))
            {
                return;
            }
            controller = recievedController;
        }
        else
        {
            controller = targetController switch
            {
                TargetControllers.Left => FeedbackManager.Controllers.Left,
                TargetControllers.Right => FeedbackManager.Controllers.Right,
                TargetControllers.Both => FeedbackManager.Controllers.Both,
            };
        }

        if (mode == Mode.Impulse)
        {
            if (impulseMode == ImpulseMode.Constant)
            {
                FeedbackManager.Instance.SendHapticImpulse(controller, amplitude, duration);
            }
            if (impulseMode == ImpulseMode.SineCurve)
            {
                StartCoroutine(SingleSineCurveCouroutine(controller));
            }
        }

        if (mode == Mode.Continuos)
        {
            if (continuousMode == ContinuousMode.Constant)
            {
                continuousCoroutine = StartCoroutine(ConstantCouroutine(controller));
            }
            if (continuousMode == ContinuousMode.SineWave)
            {
                continuousCoroutine = StartCoroutine(SineWaveCouroutine(controller));
            }
        }
    }

    private IEnumerator SingleSineCurveCouroutine(FeedbackManager.Controllers controller)
    {
        for (var x = 0f; x < Mathf.PI; x += Mathf.PI * discreteFunctionStep / duration)
        {
            FeedbackManager.Instance.SendHapticImpulse(controller, Mathf.Sin(x) * amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private IEnumerator ConstantCouroutine(FeedbackManager.Controllers controller)
    {
        while(true)
        {
            FeedbackManager.Instance.SendHapticImpulse(controller, amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private IEnumerator SineWaveCouroutine(FeedbackManager.Controllers controller)
    {
        var x = 0f;
        while (true)
        {
            x += (Mathf.PI * discreteFunctionStep / continuousModeFrequency) % Mathf.PI;
            var amplitude = this.amplitude * (Mathf.Sin(x) / 2 + 0.5f);
            FeedbackManager.Instance.SendHapticImpulse(controller, amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        SendFeedback(collider);
    }

    public enum TargetControllers
    {
        Left,
        Right,
        Both,
        BasedOnCollider
    }

    public enum Mode
    {
        Impulse,
        Continuos
    }

    public enum ImpulseMode
    {
        Constant,
        SineCurve
    }

    public enum ContinuousMode
    {
        Constant,
        SineWave,
        Square
    }

    public enum AmplitudeOverDistance
    {
        None,
        Logarithmic,
        Linear
    }

    public enum AmplitudeOverVelocity
    {
        None,
        Linear
    }
}
