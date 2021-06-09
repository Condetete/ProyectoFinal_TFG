using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales { 
public class BodyPart : MonoBehaviour
{

        [HideInInspector]
        public CharControllers character;

        public string BodyName;
        public float Multiplier;
        public float LatDamage;
        public bool debug;

        public void TakeHit(float damage)
        {
            LatDamage = damage * Multiplier;
            this.character.TakeDamage(LatDamage);
            if (debug)
                Debug.Log(damage + "*" + Multiplier + "=" + LatDamage);

        }
  
}
}