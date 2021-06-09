using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tutoriales { 
public class StatsViewer : MonoBehaviour
{

        static public StatsViewer Viewer;


        Slider[] sliders;



        private void Start()
        {
            Viewer = this;
            sliders = GetComponentsInChildren<Slider>();
        }

        public void Add(Stat stat)
        {
            stat.StatChanged += new StatEvent(OnStatChange);
            OnStatChange(stat);
        }


        //percentaje
        public void OnStatChange(Stat stat)
        {
            foreach (Slider sl in sliders)
            {
                if ( sl.name.Contains(stat.Name))
                {
                    sl.value = stat.Percentaje();
                }
            }
        }



    }
}