using UnityEngine;

public class FeedbackSource : MonoBehaviour
{
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
}
