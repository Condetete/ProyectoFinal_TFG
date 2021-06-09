using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales { 
public class RagdollController : MonoBehaviour
{
        Animator anim;
        Rigidbody[] bones;
        Rigidbody body;
        CharControllers character;
        public  bool _Debug;


        public List<HitMultiplier> hitStats;

        private void Start()
        {

            anim = GetComponent<Animator>();
            bones = GetComponentsInChildren<Rigidbody>();
            body = GetComponentInParent<Rigidbody>();
            character = GetComponentInParent<CharControllers>();
            SetUp();



        }

        public void SetUp()
        {
            LayerMask l = LayerMask.NameToLayer("hitbox");

            foreach(Rigidbody b in bones)
            {
                b.collisionDetectionMode = CollisionDetectionMode.Continuous;
                b.gameObject.layer = l;

                BodyPart p = b.gameObject.AddComponent<BodyPart>();

                p.debug = this._Debug;
                p.character = character;
                string bName = b.gameObject.name.ToLower();

                foreach (HitMultiplier hit in hitStats)
                {
                    if (bName.Contains(hit.BoneName))
                    {
                        p.Multiplier = hit.Value;
                        p.BodyName = hit.BoneName;
                        break;
                    }
                   
                }

            }

            Active(false);
        }

        public void Active (bool state)
        {
            foreach(Rigidbody b in bones)
            {
                Collider c = b.GetComponent<Collider>();
                if(b.useGravity != state)
                {
                    c.isTrigger = !state;
                    b.isKinematic = !state;
                    b.useGravity = state;
                    b.velocity = body.velocity;

                }
            }

            anim.enabled = !state;
            body.useGravity = !state;
            body.detectCollisions = !state;
            body.isKinematic = state;

        }

    }


    [System.Serializable]
    public class HitMultiplier
    {
        public string BoneName = "head";
        public float Value=1;
    }
}