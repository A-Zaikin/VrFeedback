using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GameController : MonoBehaviour 
{
    public static ActionBasedController LeftController;
    public static ActionBasedController RightController;

    public ActionBasedController leftController;
    public ActionBasedController rightController;

    private void Start()
    {
        LeftController = leftController;
        RightController = rightController;
    }
}
