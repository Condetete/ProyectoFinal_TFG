using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tutoriales { 
public class CharControllers : Photon.Pun.MonoBehaviourPun
{

        public bool Active = true;
        public bool Player = true;


        Transform tr; 
        Rigidbody rg;
        internal Animator anim;
        RagdollController ragdoll;

        CapsuleCollider coll;
        internal  IkHandler ik;
        internal InventaryController inventary;
        private bool notShooting = false;

        public Transform CameraShoulder;
        public Transform CameraHolder;
        public Transform LookAt;


        private Transform cam;
        private float rotY = 0f; 
        private float rotX = 0f;



        public Transform HandsPivot; 
        public Transform RightHand;
        public Transform RightElbow;
        public Transform RightHandRelax;
        public Transform RightElbowRelax;

        public CharStats Stats;
        Stat life;
        Stat shield;
        Stat armor;

        public float CurrentLife;

        internal States states=new States();
        private bool initialized = false;
     
        
        


        private Vector2 moveDelta;
        private Vector2 mouseDelta;
        internal Vector2 moveAnim;
        private float deltaT;

        public InputController _input;

        private void OnEnable()
        {
            if (!initialized)
            {
                Initialize();
            }
        }

        void Initialize()
        {
            tr = this.transform;

            rg = GetComponent<Rigidbody>();
            coll = GetComponent<CapsuleCollider>();
            anim = GetComponentInChildren<Animator>();
            ik = GetComponentInChildren<IkHandler>();
            inventary = GetComponent<InventaryController>();
            ragdoll = GetComponentInChildren<RagdollController>();



            inventary.Initialize(Player);

            ik.LookAtPosition = LookAt;
            ik.RightHandPosition = RightHand;
            ik.RightElbowPosition = RightElbow;
            ik.RightHandPosition_relax = RightHandRelax;
            ik.RightElbowPosition_relax = RightElbowRelax;
            cam = Camera.main.transform;

            life = new Stat("life", this.Stats.MaxLife, this.Stats.MaxLife);
            shield = new Stat("shield", this.Stats.MaxLife, 0);
            armor = new Stat("armor", this.Stats.MaxLife, 0);





            Active = true;

        }
        void OnGUI()
        {
            if (Player)
            {


                StatsViewer.Viewer.Add(life);
                StatsViewer.Viewer.Add(shield);
                StatsViewer.Viewer.Add(armor);

            }
        }





        void FixedUpdate()
        {

            if (Player)
            {
                PlayerControl();
                CameraControl();
            }
          
            {
               if (Player = photonView.IsMine)
                {
                    inventary.Initialize(Player);

                }
            }

            if (!Active)
            {
                return;
            }


            MoveControl();
            ItemsControl();
            AnimControl();

        }



        private void PlayerControl()
        {
            _input.Update();

            float deltaX = _input.CheckF("Horizontal");
            float deltaZ = _input.CheckF("Vertical");
            float mouseX = _input.CheckF("Mouse X");
            float mouseY = _input.CheckF("Mouse Y");
            states.Jumping = _input.Check("Jump");
            states.Crouching = _input.Check("Crouch");
            states.Running = _input.Check("Run");
            states.Interacting = _input.Check("Interact");
            states.Consuming = _input.Check("Consume") && states.OnGround;
            states.W1 = _input.Check("Weapon1");
            states.W2 = _input.Check("Weapon2");
            states.W3 = _input.Check("Weapon3");

            states.Reloading = _input.Check("Reloade") && !states.Running;
            states.FireMode = _input.Check("FireMode") && !states.Running;
            states.Aiming = _input.Check("Fire2") && !states.Running;
            bool shoot = _input.Check("Fire1") && !states.Running && !EventSystem.current.IsPointerOverGameObject();
            

            if (!shoot)
            {
                notShooting = true;
            }
            //saber si ha empezado a disparar
            states.Shooting = shoot && notShooting;

            moveDelta = new Vector2(deltaX, deltaZ);
            mouseDelta = new Vector2(mouseX, mouseY);
            deltaT = Time.deltaTime;


            if (Input.GetKeyDown(KeyCode.Y))
            {
                TakeDamage(30);
            }
        }

        private void MoveControl()
        {
            Vector3 side = Stats.speed * moveDelta.x * deltaT * tr.right;
            Vector3 forward = Stats.speed * moveDelta.y * deltaT * tr.forward;
            Vector3 endSpeed = side + forward;

            RaycastHit hit; states.OnGround = Physics.Raycast(this.tr.position, -tr.up, out hit, .2f);


            if (states.OnGround)
            {
                if (states.Crouching)
                    OnCrouch();
                else
                {
                    if (states.Running)
                        endSpeed *= Stats.runningSpeedIncrement;
                }
                if (states.Jumping)
                {
                    if (states.Crouch) OnCrouch();

                    else Jump();
                }
                Vector3 sp = rg.velocity;
                endSpeed.y = sp.y;
                rg.velocity = endSpeed;
            }
            else
            {
                if (states.Crouch)
                    OnCrouch();
            }
            moveAnim = moveDelta * (states.Running ? 2 : 1);
        }

        private void ItemsControl()
        {

            Collider[] checking = Physics.OverlapSphere(LookAt.position, 2f, LayerMask.GetMask("Item"));


            if (checking.Length > 0)
            {
                float near = 2f;
                Collider nearest = null;




                //Averiguar el mas cercano

                foreach (Collider c in checking)
                {
                    Vector3 collisionpos = c.ClosestPoint(LookAt.position);
                    float distance = Vector3.Distance(collisionpos, LookAt.position);

                    if (distance < near)
                    {
                        nearest = c;
                        near = distance;
                    }
                }


                if (nearest != null)
                {
                    ItemController item = nearest.GetComponent<ItemController>();
                    if (item != null)
                    {
                        if (Player)
                        {
                            inventary.ItemViewer.DrawItemViewer(item.Stats, item.mTransform, cam);
                        }

                        if (states.Interacting)
                        {
                            inventary.AddItem(item);
                        }
                    }

                }
                else
                {
                    if (this.Player)
                    {
                        inventary.ItemViewer.HideViewer();
                    }

                }
            }


            ItemController selectedWeapon = inventary.GetSelectedAt("Primary Weapon");

            if (selectedWeapon != null)
            {
                if (selectedWeapon is GunController)
                {

                    GunController GUN = (selectedWeapon as GunController);
                    ik.LeftHandPosition = GUN.leftHandPosition;
                    ik.LeftElbowPosition = GUN.leftElbowPosition;

                    GUN.DrawCrossHair(cam);
                    if (states.Shooting)
                    {
                        if (!GUN.Attack(this))
                        {
                            notShooting = false;
                        }
                    }

                    else if (states.Reloading)
                    {
                        GUN.Use(this);
                    }
                    else if (states.FireMode)
                    {
                        GUN.NextFireState();
                    }

                    //El ratón no estara bloqueado
                    Cursor.lockState = (Input.GetKey(KeyCode.Escape) ? CursorLockMode.None : CursorLockMode.Locked);
                }
            }

            if (states.Consuming)
            {
                ItemController SelectedMed = inventary.GetSelectedAt("Meds");
                if (SelectedMed != null)
                {
                    if (SelectedMed is ConsumableItem)
                    {
                        ConsumableItem consumable = SelectedMed as ConsumableItem;
                        consumable.Use(this);
                    }
                }
            }

            InventaryGroup weaponGroup = inventary.GetGroup("Primary Weapon");

            if (weaponGroup != null)
            {
                if (states.W1)
                {
                    weaponGroup.Select(0);
                }
                if (states.W2)
                {
                    weaponGroup.Select(1);
                }
                if (states.W3)
                {
                    weaponGroup.Select(2);
                }


            }



        }
        public void Jump()
        {
            rg.AddForce(tr.up * Stats.jumpForce);
        }
        public void OnCrouch()
        {
            states.Crouch = !states.Crouch;
            states.Crouching = false;
            float mult = (states.Crouch ? 1 : -1);
            coll.center = coll.center + new Vector3(0, Stats.crouchPosOffset, 0) * mult;
            coll.height += Stats.crouchHeightOffset * mult;
            CameraShoulder.position = CameraShoulder.position + new Vector3(0, Stats.crouchPosOffset, 0) * mult;
        }


        public void TakeDamage(float damage)
        {
            life.Value -= damage;
            if (life.Value <= 0)
            {
                ragdoll.Active(true);
                this.Active = false;
            }
        }

        internal void Consume(float effectStrength, string effectStat)
        {
            if (effectStat == "life")
            {
                if (life.Value + effectStrength <= life.Max)
                {
                    life.Value += effectStrength;
                }
                else
                {
                    life.Value = life.Max;
                }
            }
            else if (effectStat == "shield")
            {
                if (shield.Value + effectStrength <= shield.Max)
                {
                    shield.Value += effectStrength;
                }
                else
                {
                    shield.Value = shield.Max;
                }

            }
        }

        private void CameraControl()
        {
            rotY += mouseDelta.y * deltaT * Stats.rotationSpeed;
            float xrot = mouseDelta.x * deltaT * Stats.rotationSpeed;
            tr.Rotate(0, xrot, 0);
            rotY = Mathf.Clamp(rotY, Stats.minAngle, Stats.maxAngle);
            Quaternion localRotation = Quaternion.Euler(-rotY, 0, 0);
            CameraShoulder.localRotation = localRotation;
            cam.position = Vector3.Lerp(cam.position, CameraHolder.position, Stats.cameraSpeed * deltaT);
            cam.rotation = Quaternion.Lerp(cam.rotation, CameraHolder.rotation, Stats.cameraSpeed * deltaT);
        }
        private void AnimControl()
        {

            HandsPivot.position = anim.GetBoneTransform(HumanBodyBones.RightShoulder).position;
            Quaternion localRotation = Quaternion.Euler(-rotY, HandsPivot.localRotation.y, HandsPivot.localRotation.z);
            HandsPivot.localRotation = localRotation;

            anim.SetBool("ground", states.OnGround);
            anim.SetBool("crouch", states.Crouch);

            anim.SetFloat("X", moveAnim.x);
            anim.SetFloat("Y", moveAnim.y);

            ik.Aiming = this.states.Aiming;
            ik.Shooting = this.states.Shooting;
        }
    }


    //Añadir estados
    public class States
    {
        public bool OnGround = false;
        public bool Jumping = false;
        public bool Crouch = false;
        public bool Crouching = false;
        public bool Running = false;
        public bool Aiming = false;
        public bool Shooting = false;
        public bool Interacting = false;
        public bool Consuming = false;
        public bool W1 = false;
        public bool W2 = false;
        public bool W3 = false;

        public bool Reloading = false;
        public bool FireMode = false;
    }

}