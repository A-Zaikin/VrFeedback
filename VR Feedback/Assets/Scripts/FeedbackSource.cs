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
    [SerializeField] private float discreteFunctionStep;

    private List<ContinuousVibration> continuousVibrations;

    public Mode CurrentMode { get => mode; }
    

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
            if (continuousMode == ContinuousMode.Constant)
            {
                continuousCoroutine = StartCoroutine(ConstantCoroutine(controller));
            }
            if (continuousMode == ContinuousMode.SineWave)
            {
                continuousCoroutine = StartCoroutine(SineWaveCoroutine(controller));
            }
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
            }
        }
        return id;
    }

    public void EndContinuousFeedback(int id)
    {
        var coroutineToStop = continuousVibrations.Find(vibration => vibration.id == id);
        continuousVibrations.Remove(coroutineToStop);
        StopCoroutine(coroutineToStop.coroutine);
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
        for (var x = 0f; x < Mathf.PI; x += Mathf.PI * discreteFunctionStep / duration)
        {
            FeedbackManager.Instance.SendHapticImpulse(controller, Mathf.Sin(x) * amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private IEnumerator ConstantCoroutine(FeedbackManager.Controllers controller)
    {
        while(true)
        {
            FeedbackManager.Instance.SendHapticImpulse(controller, amplitude, discreteFunctionStep);
            yield return new WaitForSecondsRealtime(discreteFunctionStep);
        }
    }

    private IEnumerator SineWaveCoroutine(FeedbackManager.Controllers controller)
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

    //private void OnTriggerEnter(Collider collider)
    //{
    //    SendFeedback(collider);
    //}

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
