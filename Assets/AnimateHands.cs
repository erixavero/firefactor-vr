using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{
    public InputActionProperty triggervall;
    public InputActionProperty gripvall;
    public Animator handanim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float trigger = triggervall.action.ReadValue<float>();
        float grip = gripvall.action.ReadValue<float>();

        handanim.SetFloat("Trigger",trigger);
        handanim.SetFloat("Grip",grip);
    }
}
