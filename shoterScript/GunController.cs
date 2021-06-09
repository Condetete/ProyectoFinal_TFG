using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{
    public class GunController : WeaponController
{

        Transform crosshair;

    public Transform ModelRoot;

    public Transform shootPoint;
    public Transform leftHandPosition;
    public Transform leftElbowPosition;

    public BulletController bulletPrefab;
    


        private bool showCrossHair = false;


        public AudioClip shotNoise;
        private AudioSource shotSource;

        //destello
        private ParticleSystem muzzle;

        private int ShootedBullets = 0;
        private float deltaTime = 0;
        public int ActualAmmo = 0;


        int fState = 0;
        public FireState FireState;

        public FireState FireSate
        {
            get
            {
                return (getGunStats().States.Count >= fState ?  getGunStats().States[fState]:new FireState() );

            }
        }
        public void NextFireState()
        {
            int state = fState + 1;
            fState = (state >= getGunStats().States.Count ? 0 : state);
        }


        protected override void Initialize()
        {
       
            crosshair = GetComponentInChildren<Canvas>().transform;
            crosshair.gameObject.SetActive(false);

            muzzle = shootPoint.GetComponentInChildren<ParticleSystem>();
            muzzle.gameObject.SetActive(false);
            muzzle.Stop();
            shotSource = shootPoint.GetComponent<AudioSource>();

            //se inicia con un cargador
            ActualAmmo = getGunStats().maxClip;
        }


        public GunStats getGunStats()
        {
            if (Stats is GunStats)
                return Stats as GunStats;
            GunStats defect = new GunStats();
            Stats = defect;
            return defect;

        }

   


        public override bool Attack (CharControllers character)
        {
            if (ActualAmmo<=0)
            {
                return false;
            }

            if (deltaTime > FireState.FireLapse() || ShootedBullets == 0)
            {
                deltaTime = 0;
                switch (FireState.mode)
                {
                    case FireMode.auto:
                        Shoot(character);
                        return true;
                    case FireMode.burst:
                        Shoot(character);
                        int rate = FireState.rate;
                        if (ShootedBullets >= rate)
                        {
                            ShootedBullets = 0;
                            return false;
                        }
                        return true;
                    case FireMode.unique:
                        if (ShootedBullets == 0)
                        {
                            Shoot(character);
                            ShootedBullets = 0;
                        }
                        return false;
                }

            }

            return true;
        }

        private void Shoot(CharControllers character)
        {
            BulletController bullet = Instantiate<BulletController>(bulletPrefab, shootPoint.position, shootPoint.rotation);
            GunStats gstats = getGunStats();
            bullet.Initialize(gstats.power, gstats.Damage, gstats.lifeTime);

            shotSource.PlayOneShot(shotNoise, GameConfiguration.EffectsLevel);
            muzzle.gameObject.SetActive(true);
            muzzle.Play(true);

           character.ik.UpdateRecoil(getGunStats().MaxRecoil, -character.moveAnim.x, getGunStats().ShootModifier);

            ActualAmmo--;
            ShootedBullets++;
        }



        public void ShowCrossHair()
        {
            crosshair.gameObject.SetActive(showCrossHair = !showCrossHair);
        }
        public void DrawCrossHair(Transform camera)
        {
            if (!showCrossHair)
            {
                ShowCrossHair();
            }

            Vector3 end = shootPoint.position + shootPoint.forward * getGunStats().Range;
           crosshair.position = Vector3.Lerp(end, camera.position, 0.9f);
            crosshair.rotation = camera.rotation;

            
            
         /* RaycastHit hit;
            if(Physics.Raycast(camera.position, camera.forward, out hit, getGunStats().Range))
            {

                ModelRoot.LookAt(2 * ModelRoot.position - hit.point);

            }
            else
            {
                Vector3 end = camera.position + camera.forward * getGunStats().Range;
                ModelRoot.LookAt(2 * ModelRoot.position - end);

            }*/
          
        }


        public override void Use(CharControllers character)
        {
            InventaryGroup g = character.inventary.GetGroup("Ammo");

            if (g != null)
            {
                ItemController it = g.GetItemByName(getGunStats().ammoName);
                if (it != null && it is ConsumableItem)
                {
                    ConsumableItem consumable = (it as ConsumableItem);

                    int lack = (this.getGunStats().maxClip - ActualAmmo);
                    if (lack <= 0)
                    {
                        return;
                    }
                    else
                    {
                        int getUnits = (consumable.Units < lack ? consumable.Units : lack);

                       // reloading = consumable;
                        //consumable.Use(character);

                        consumable.Units -= getUnits;
                        ActualAmmo += getUnits;


                        if (consumable.Units <= 0)
                        {
                            character.inventary.RemoveItem(consumable);
                            ConsumableItem.Destroy(consumable.gameObject);
                        }

                       /* int index = character.anim.GetLayerIndex(Stats.animLayer);
                        character.anim.Play(Stats.animation, index);
                        character.anim.SetLayerWeight(index, 1);
                        character.ik.DisableLeftHand = consumable.getConsumableStats().DisableLeftHand;
                        character.ik.DisableRightHand = consumable.getConsumableStats().DisableRightHands;

                        Debug.Log(Stats.animation);
                        Debug.Log(index);*/
                    }
                }
            }
            base.Use(character);
        }

    }
}

