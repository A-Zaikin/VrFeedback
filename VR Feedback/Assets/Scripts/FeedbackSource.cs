using UnityEngine;
using System.Collections.Generic;
using System;

public class FeedbackSource : MonoBehaviour
{
    [SerializeField] private Mode mode;
    [SerializeField] private ImpulseMode impulseMode;
    [SerializeField] private ContinuosMode continuosMode;
    [SerializeField] private AmplitudeRollOffOverDistance amplitudeRollOffOverDistance;
    [SerializeField] private float continuosModeFrequency;
    [SerializeField] private AmplitudeRollOffOverVelocity amplitudeRollOffOverVelocity;
    [SerializeField] private float distanceRollOffCoefficient;
    [SerializeField] private float velocityRollOffCoefficient;

    
    [SerializeField] private float amplitude;
    [SerializeField] private float duration;
    [SerializeField] private TargetControllers targetController;

    public void SendFeedback()
    {
        if (targetController != TargetControllers.BasedOnCollider) 
        {
            var controller = targetController switch
            {
                TargetControllers.Left => FeedbackManager.Controllers.Left,
                TargetControllers.Right => FeedbackManager.Controllers.Right,
                TargetControllers.Both => FeedbackManager.Controllers.Both,
            };
            FeedbackManager.Instance.SendHapticImpulse(controller, amplitude, duration);
        }
        //if(mode == Mode.Impulse)
        //{
        //    SendImpulseFeedback();
        //}

        //if(mode == Mode.Continuos)
        //{
        //    SendContinuousFeedback();
        //}
    }

    

    private void OnTriggerEnter(Collider collider)
    {
        if (targetController == TargetControllers.BasedOnCollider)
        {
            FeedbackManager.Instance.SendHapticImpulseToCollider(collider, amplitude, duration);
        }
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
        BellCurve
    }

    public enum ContinuosMode
    {
        Constant,
        SineWave,
        Square
    }

    public enum AmplitudeRollOffOverDistance
    {
        None,
        Logarithmic,
        Linear
    }

    public enum AmplitudeRollOffOverVelocity
    {
        None,
        Linear
    }
}
