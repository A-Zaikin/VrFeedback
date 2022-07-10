using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackActivator : MonoBehaviour
{
    [SerializeField] private FeedbackSource feedbackSource;
    [SerializeField] private bool onTriggerEnter;
    [SerializeField] private bool onTriggerExit;
    //private bool onGrab;
    //private bool onPointerEnter;
    [SerializeField] private bool onColliderStay;
    //private bool continuousManual;

    [SerializeField] public FeedbackSource CurrentFeedbackSource { get => feedbackSource; }

    private void Start()
    {
        //feedbackSource = this.gameObject.GetComponent<FeedbackSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(onTriggerEnter)
            feedbackSource.SendFeedback(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(onTriggerExit)
            feedbackSource.SendFeedback(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (onColliderStay)
            feedbackSource.SendFeedback(other);
    }

    
}
