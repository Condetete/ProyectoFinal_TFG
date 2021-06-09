using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{
    public class BulletController : MonoBehaviour
    {
       
        private float lifeTime = 10f;
        public float damage;
        float deltatime = 0f;

        Rigidbody rg;

        Transform mtransform;
        Vector3 lastPosition;

        public LayerMask hitboxMask;

       

        public void Initialize(float power,float damage,float lifeTime)
        {

            rg = GetComponent<Rigidbody>();
            mtransform = GetComponent<Transform>();

            rg.velocity = this.transform.forward * power;
            this.damage = damage;
            this.lifeTime = lifeTime;

            hitboxMask = LayerMask.NameToLayer("hitbox");
            lastPosition = mtransform.position;


         

        }
        private void FixedUpdate()
        {
            deltatime += Time.deltaTime;
            detectCollision();
            if (deltatime >= lifeTime)
                Destroy(this.gameObject);
        }


        //detectar las colisiones de las balas en el personaje
        private void detectCollision()
        {
            Vector3 newPos = mtransform.position;
            Vector3 dir = newPos - lastPosition;

            RaycastHit hit;
            if(Physics.Raycast(lastPosition, dir.normalized, out hit, dir.magnitude))
            {
                GameObject go = hit.collider.gameObject;

                if(go.layer == hitboxMask)
                {
                    BodyPart bp = go.GetComponent<BodyPart>();
                    if (bp != null)
                    {
                        bp.TakeHit(damage);
                        Debug.Log("Impacto en " + bp.BodyName);
                    }
                }
                else
                {
                    HolesPool.Pool.Impact(go.tag, hit.point, hit.normal);
                    Destroy(this.gameObject);

            }

            }
          

            lastPosition = newPos;
        }


    }
}