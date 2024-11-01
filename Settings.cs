using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Settings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;
    public Toggle postproc_toggle;
    public Toggle fps_toggle;

    void Start()
    {
        // ЗАГРУЗКА ВСЕХ НАСТРОЕК ИЗ ПРЕФОВ ТУТ! :)
        GlobalFunc.LoadSettings();
        sensitivitySlider.value = Globals.sensitivity;
        sensitivityText.text = (Globals.sensitivity/55).ToString("F2");
        if(Globals.postprocessing == 0) postproc_toggle.isOn = false;
        if(Globals.postprocessing == 1) postproc_toggle.isOn = true;
        if(Globals.fpscounter == 0) fps_toggle.isOn = false;
        if(Globals.fpscounter == 1) fps_toggle.isOn = true;
    }

    public void OnSensitivitySlderChanged(Slider slider)
    {
        Globals.sensitivity = slider.value;
        PlayerPrefs.SetFloat("sensitivity", Globals.sensitivity);
        sensitivityText.text = (Globals.sensitivity/55).ToString("F2");
    }

    public void OnPostprocessEnabledChanged(Toggle toggle)
    {
        if(toggle.isOn) Globals.postprocessing = 1;
        else if(!toggle.isOn) Globals.postprocessing = 0;
        PlayerPrefs.SetInt("postprocessing", Globals.postprocessing);
    }

    public void OnFPScounterChanged(Toggle toggle)
    {
        if(toggle.isOn) Globals.fpscounter = 1;
        else if(!toggle.isOn) Globals.fpscounter = 0;
        PlayerPrefs.SetInt("fpscounter", Globals.fpscounter);
    }

    public void ResetControls()
    {
        GlobalFunc.ResetControls();
    }
}
