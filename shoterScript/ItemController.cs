using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{
    public class ItemController : MonoBehaviour
    {
        public ItemStats Stats;

        internal Transform mTransform;
        private Rigidbody rg;
        private SphereCollider collider;
        protected MeshRenderer[] meshes;


        private void Start()
        {
            mTransform = this.transform;

            rg = GetComponent<Rigidbody>();
            collider = GetComponent<SphereCollider>();
            meshes=GetComponentsInChildren<MeshRenderer>();
            Initialize();
        }


        public void Show()
        {
            foreach(MeshRenderer mesh in meshes)
            {
                mesh.enabled = true;

            }
        }

        public void Hide()
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.enabled = false;

            }
        }
        protected virtual void Initialize()
        {

        }

        public virtual void Use(CharControllers character)
        {
            if (Stats.ShowOnUse)
            {
                this.Show();
            }
        }

        public void Take(Transform slotPosition)
        {
            if (slotPosition !=null)
            {
                mTransform.parent = slotPosition;
            }
            
            mTransform.localPosition = Vector3.zero;
            mTransform.localRotation = Quaternion.identity;
            rg.detectCollisions = false;
            rg.isKinematic = true;
            collider.enabled = false;

            if (Stats.HideOnTake)
            {
                this.Hide();
            }

           

        }

        public void Drop()
        {
            mTransform.parent = null;
            
            rg.detectCollisions = true;
            rg.isKinematic = false;
            collider.enabled = true;
            RaycastHit hit;
            if(Physics.Raycast(mTransform.position, Vector3.down,out hit))
            {
                mTransform.position = hit.point;
            }
            if (Stats.HideOnTake)
            {
                this.Show();
            }
        }
    }
}