﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace tutoriales { 
public class VolumenCode : MonoBehaviour
{

        public Slider slider;
        public float sliderValue;
        public Image imageMute;

    void Start()
    {
            slider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
            AudioListener.volume = slider.value;
            RevisarSiEstoyMute();
        
    }

    public void ChangeSlider(float valor)
        {
            sliderValue = valor;
            PlayerPrefs.SetFloat("VolumenAudio", sliderValue);
            AudioListener.volume = slider.value;
            RevisarSiEstoyMute();
        }

        public void RevisarSiEstoyMute()
        {
            if (sliderValue == 0)
            {
                imageMute.enabled = true;

            }
            else
            {
                imageMute.enabled = false;
            }
        }
    }
}