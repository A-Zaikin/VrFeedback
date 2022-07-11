using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private List<ContinuousVibration> continuousVibrations;

    public Mode CurrentMode { get => mode; }

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
                StartCoroutine(SingleSineCurveCoroutine(controller));
            }
        }


    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    public int StartContinuousFeedback(Collider collider = null)
    {
        var controller = GetControllers(collider);
        var vibrationsList = continuousVibrations.ToList();
        var vibration = new ContinuousVibration();
        int id = 0;

        if (mode == Mode.Continuous)
        {
            Coroutine continuousCoroutine = continuousMode == ContinuousMode.Constant
                ? StartCoroutine(ConstantCoroutine(controller))
                : StartCoroutine(SineWaveCoroutine(controller));

            vibration.coroutine = continuousCoroutine;
            vibration.collider = collider;
        }
        var idFound = false;
        while (!idFound)
        {
            var idbuffer = Random.Range(0, 1000);
            if (!vibrationsList.Any(vibration => vibration.id == idbuffer))
            {
                id = idbuffer;
                vibration.id = id;
                idFound = true;
            }
        }
        continuousVibrations.Add(vibration);
        return id;
    }

    public void EndContinuousFeedback(int id)
    {
        var coroutineToStop = continuousVibrations.Find(vibration => vibration.id == id);
        continuousVibrations.Remove(coroutineToStop);
        StopCoroutine(coroutineToStop.coroutine);
    }

    public void EndContinuousFeedback(Collider collider)
    {
        var coroutinesToStop = continuousVibrations.FindAll(vibration => vibration.collider == collider);
        foreach (var coroutineToStop in coroutinesToStop)
        {
            continuousVibrations.Remove(coroutineToStop);
            StopCoroutine(coroutineToStop.coroutine);
        }
    }

    private FeedbackManager.Controllers GetControllers(Collider collider = null)
    {
        FeedbackManager.Controllers controller;
        if (targetController == TargetControllers.BasedOnCollider && collider != null)
        {
            if (!FeedbackManager.Instance.TryGetController(collider, out var recievedController))
            {
                return FeedbackManager.Controllers.None;
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
        return controller;
    }

    private IEnumerator SingleSineCurveCoroutine(FeedbackManager.Controllers controller)
    {
        var discreteFunctionStep = FeedbackManager.Instance.DiscreteFunctionStep;
        for (var x = 0f; x < Mathf.PI; x += Mathf.PI * discreteFunctionStep / duration)
        {
            var amplitude = GetDistanceCoefficient(controller) * Mathf.Sin(x) * this.amplitude;
            FeedbackManager.Instance.SendHapticImpulse(controller, amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private IEnumerator ConstantCoroutine(FeedbackManager.Controllers controller)
    {
        var discreteFunctionStep = FeedbackManager.Instance.DiscreteFunctionStep;
        while (true)
        {
            FeedbackManager.Instance.SendHapticImpulse(controller,
                GetDistanceCoefficient(controller) * amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private IEnumerator SineWaveCoroutine(FeedbackManager.Controllers controller)
    {
        var x = 0f;
        var discreteFunctionStep = FeedbackManager.Instance.DiscreteFunctionStep;
        while (true)
        {
            x += (Mathf.PI * discreteFunctionStep / continuousModeFrequency) % Mathf.PI;
            var amplitude = this.amplitude * (Mathf.Sin(x) / 2 + 0.5f) * GetDistanceCoefficient(controller);
            FeedbackManager.Instance.SendHapticImpulse(controller, amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private float GetDistanceCoefficient(FeedbackManager.Controllers controller)
    {
        var distanceToController = Vector3.Distance(transform.position,
            FeedbackManager.Instance.GetPosition(controller));
        var distanceCoefficient = amplitudeOverDistance == AmplitudeOverDistance.Linear
            ? distanceRollOffCoefficient / distanceToController
            : 1;
        distanceCoefficient = Mathf.Min(distanceCoefficient, 1);
        return distanceCoefficient;
    }

    private void Awake()
    {
        continuousVibrations = new List<ContinuousVibration>();
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
        Continuous
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

    public class ContinuousVibration
    {
        public Coroutine coroutine;
        public int id;
        public Collider collider;

        public ContinuousVibration()
        {

        }

        public ContinuousVibration(Coroutine coroutine, int id, Collider collider)
        {
            this.coroutine = coroutine;
            this.id = id;
            this.collider = collider;
        }

    }
}
