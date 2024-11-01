using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float smooth = 8;
    public float swayMultiplier = 4;
    float mouseX;
    float mouseY;
    public bool cutscene;

    private void Update()
    {
        if(!Globals.pauseOpened && !cutscene)
        {
            //get mouse input
            mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier * (Globals.sensitivity / 108f);
            mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier * (Globals.sensitivity / 108f);
        }
        if(Globals.pauseOpened && !cutscene)
        {
            //get mouse input
            mouseX = 0;
            mouseY = 0;
        }

        //calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        //rotate
    
        if(!cutscene)transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
