using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    [CreateAssetMenu(fileName = "stats", menuName = "tutoriales/char stats")]
    public class CharStats : ScriptableObject
    {
        public float speed = 200;
        public float runningSpeedIncrement = 1.5f; 
        public float rotationSpeed = 25; 
        public float jumpForce = 200;
        public float minAngle = -70; 
        public float maxAngle = 70;
        public float maxFallSpeed = 50;
        public float cameraSpeed = 24;

        public float crouchHeightOffset = -.25f; 
        public float crouchPosOffset = -.25f;

        public float MaxLife = 100;
    }
}
