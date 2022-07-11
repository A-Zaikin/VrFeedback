using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackActivator : MonoBehaviour
{
    [SerializeField] private FeedbackSource feedbackSource;
    [SerializeField] private bool onTriggerEnter;
    [SerializeField] private bool onTriggerExit;
    [SerializeField] private bool onColliderStay;

    [SerializeField] public FeedbackSource CurrentFeedbackSource { get => feedbackSource; }


    private void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter)
        {
            feedbackSource.SendFeedback(other);
        }
        if (onColliderStay)
        {
            feedbackSource.StartContinuousFeedback(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (onTriggerExit)
            feedbackSource.SendFeedback(other);
        if (onColliderStay)
        {
            feedbackSource.EndContinuousFeedback(other);
        }
    }

}
