using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{
    public class Air : MonoBehaviour
{
        public CharControllers charPrefab;

        public Transform Plane;
        public Transform PlaneCam;
        public Transform PlaneCamRotation;
        private Transform cam;

        public float DistanceJump;
        public float Speed;
        public float Distance;
        public float rotationSpeed;

        public bool PrepareLaunch = false;
        public bool Launched = false;

        private void OnEnable()
        {
            Vector3 pos = new Vector3(Random.Range(0, 0), 0, Random.Range(0, 0));
            Plane.position = this.transform.position + pos * Distance;
            Plane.LookAt(this.transform.position);

            Rigidbody rg = Plane.GetComponent<Rigidbody>();
            rg.velocity = Plane.forward * Speed;

            cam = Camera.main.transform;
        }

        private void Update()
        {
            if (!Launched)
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                Vector2 mouseDelta = new Vector2(mouseX, mouseY);

                float xrot = mouseDelta.x * Time.deltaTime * rotationSpeed;
                PlaneCamRotation.Rotate(0, xrot, 0);

                cam.position = PlaneCam.position;
                cam.LookAt(Plane);

                bool distance = (Vector3.Distance(this.transform.position,Plane.position) < DistanceJump);

                if(distance)
                {
                    float jump = Input.GetAxis("Jump");
                    if (jump == 1)
                    {
                        Launched = true;

                        Photon.Pun.PhotonNetwork.Instantiate(charPrefab.name, Plane.position, Quaternion.identity);

                       // CharControllers player= Instantiate<CharControllers>(charPrefab);
                       // player.transform.position = ;
                    }

                }
            }
            
        }
    }

}
