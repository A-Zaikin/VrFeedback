using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VrFeedback
{
    public class VibrationController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            if(gameObject.tag == "Sphere")
            {
                FeedbackManager.Instance.SendHapticImpulseToCollider(collider, 0.5f, 0.5f);
            }
            if(gameObject.tag == "Cylinder")
            {
                FeedbackManager.Instance.SendHapticImpulse(FeedbackManager.Controllers.Both, 0.7f, 2);
            }
        }
    }
}