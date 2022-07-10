using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;

    private void Update()
    {
        var (leftVibration, rightVibration) = FeedbackManager.Instance.CurrentVibrationAmplitudes;
        textMesh.text = $"Vibration\nleft: {leftVibration}\nright: {rightVibration}";
    }
}
