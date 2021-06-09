using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace tutoriales
{
    public class InventaryItemViewer : MonoBehaviour
    {
        InventaryGroupViewer inventaryGroupViewer;
        Image itemIcon;

        internal ItemController invItem;
        internal Transform mtransform;


        public Sprite defaultSprite;
        public Image countDown;
        public bool StartCountDown;

        //texto para cantidad de balas 
        public TextMeshProUGUI Text;


        public void Initialize(InventaryGroupViewer inventaryGroup)
        {
            this.mtransform = this.transform;
            this.inventaryGroupViewer = inventaryGroup;
            itemIcon = GetComponent<Image>();
        }

        public void SetItem(ItemController item)
        {
            this.invItem = item;
            itemIcon.sprite = (item != null ? item.Stats.Icon : defaultSprite);
           // Text.text = "";
             countDown.fillAmount = 0;
        }

        private void FixedUpdate()
        {
            if(invItem != null)
            {
                if (invItem is ConsumableItem)
                {

                    ConsumableItem it = (invItem as ConsumableItem);
                    countDown.fillAmount = it.GetActualUseState();

                    if(Text != null)
                    {
                        Text.text = it.Units + "/" + it.getConsumableStats().MaxUnits;
                    }

                    countDown.fillAmount = it.GetActualUseState();

                }
                else if(invItem is GunController)
                {
                    GunController it = (invItem as GunController);

                    if(Text != null)
                    {
                        Text.text = it.FireSate.mode + "\n"+  it.ActualAmmo+ "/" + it.getGunStats().maxClip;
                    }
                }
            }
        }

        public void OnclickSelect()
        {
            inventaryGroupViewer.thisGroup.Select(invItem);
        }

    }
}