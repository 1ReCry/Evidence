using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightWiggle : MonoBehaviour
{
    public GameObject flashlightObj;
    public GameObject flashlightOriginal;
    private PlayerController pcontroller;

    [Header("Настройки покачивания")]
    public float lerpSpeedWalk = 2f;
    public float lerpSpeedRun = 2f;
    public float y_wiggle_decrease_speed = 10f;
    public Vector2 random_walk_x_angle;
    public Vector2 random_walk_up;
    public Vector2 random_run_x_angle;
    public Vector2 random_run_up;
    public float run_angle_y;

    private Vector3 currentRotation;
    private float flashlight_x_rot;
    private float flashlight_y_rot;
    private bool wiggle_state_is_positive;
    private float y_wiggle;

    void Start()
    {
        currentRotation = flashlightObj.transform.localEulerAngles;
        pcontroller = GetComponent<PlayerController>();
    }

    void Update()
    {
        y_wiggle = Mathf.Lerp(y_wiggle, 0, y_wiggle_decrease_speed);
        if(pcontroller.isMoving) LerpFlashlightPos();
        if(!pcontroller.isMoving) LerpFlashlightPosToDefault();

        if(!pcontroller.sprinting)
        {
            currentRotation = Vector3.Lerp(currentRotation, new Vector3(0, 0f, 0f), Time.deltaTime * lerpSpeedWalk);
            flashlightOriginal.transform.localEulerAngles = currentRotation;
        }
        if(pcontroller.isMoving && pcontroller.sprinting)
        {
            currentRotation = Vector3.Lerp(currentRotation, new Vector3(run_angle_y, 0f, 0f), Time.deltaTime * lerpSpeedWalk);
            flashlightOriginal.transform.localEulerAngles = currentRotation;
        }
    }

    void LerpFlashlightPos()
    {
        if(!pcontroller.sprinting)
        {
            Vector3 targetRotation = new Vector3(y_wiggle, flashlight_x_rot, 0f);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * lerpSpeedWalk);
            flashlightObj.transform.localEulerAngles = currentRotation;
        }
        if(pcontroller.sprinting && pcontroller.isMoving)
        {
            Vector3 targetRotation = new Vector3(y_wiggle, flashlight_x_rot, 0f);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * lerpSpeedRun);
            flashlightObj.transform.localEulerAngles = currentRotation;
        }
    }
    void LerpFlashlightPosToDefault()
    {
        y_wiggle = 0;
        flashlight_x_rot = 0;
        flashlight_y_rot = 0;
        Vector3 targetRotation = new Vector3(0, 0, 0f);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * lerpSpeedWalk);
        flashlightObj.transform.localEulerAngles = currentRotation;
    }

    public void FlashlightRandomWiggle()
    {
        if(!pcontroller.sprinting)
        {
            y_wiggle += Random.Range(random_walk_up.x, random_walk_up.y);

            if(wiggle_state_is_positive)
            {
                wiggle_state_is_positive = false;
                flashlight_x_rot = Random.Range(random_walk_x_angle.x, random_walk_x_angle.y);
            }

            else if(!wiggle_state_is_positive)
            {
                wiggle_state_is_positive = true;
                flashlight_x_rot = Random.Range(-random_walk_x_angle.x, -random_walk_x_angle.y);
            }
        }

        if(pcontroller.sprinting)
        {
            y_wiggle += Random.Range(random_run_up.x, random_run_up.y);

            if(wiggle_state_is_positive)
            {
                wiggle_state_is_positive = false;
                flashlight_x_rot = Random.Range(random_run_x_angle.x, random_run_x_angle.y);
            }

            else if(!wiggle_state_is_positive)
            {
                wiggle_state_is_positive = true;
                flashlight_x_rot = Random.Range(-random_run_x_angle.x, -random_run_x_angle.y);
            }
        }

        //Debug.Log("Flashlight Wiggle: \nwiggle_state_is_positive: " + wiggle_state_is_positive + " | flashlight_x_rot: " + flashlight_x_rot + " | y_wiggle: " + y_wiggle);
    }
}
