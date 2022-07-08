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
                var handCube = collider.gameObject;
                var controller = handCube.GetComponentInParent<ActionBasedController>();
                controller.SendHapticImpulse(0.7f, 2);
                Debug.Log($"Sent vibration to {controller.name}");
            }
            if(gameObject.tag == "Cylinder")
            {
                GameController.LeftController.SendHapticImpulse(0.7f, 2);
                GameController.RightController.SendHapticImpulse(0.7f, 2);
                Debug.Log("Vibration sent to both controllers");
            }
        }
    }
}