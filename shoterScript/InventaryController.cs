using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{
    public class InventaryController : MonoBehaviour
    {

        public ItemViewer ItemViewer;
        public InventaryViewer inventaryViewer;
        public List<InventaryGroup> inventaryGroups;

        private Dictionary<string, InventaryGroup> mappedInventary = new Dictionary<string, InventaryGroup>();
        public void Initialize(bool player)
        {
            MapInventaryGroups();
            if (player)
            {
                ItemViewer = FindObjectOfType<ItemViewer>();
                inventaryViewer = FindObjectOfType<InventaryViewer>();
                inventaryViewer.Initialize(this);
            }
            else
            {
                ItemViewer = null;
          

            }
            
        }


        private void MapInventaryGroups()
        {
            mappedInventary.Clear();
            foreach (InventaryGroup g in inventaryGroups)
            {
                if (!mappedInventary.ContainsKey(g.SlotType))
                {
                    mappedInventary.Add(g.SlotType, g);
                }
            }
        }

        public bool AddItem(ItemController item)
        {
            if (mappedInventary.ContainsKey(item.Stats.SlotType))
            {
                return mappedInventary[item.Stats.SlotType].AddItem(item);
            }
            return false;
        }


        public ItemController GetSelectedAt(string group)
        {

            InventaryGroup inveGroup = GetGroup(group);
            if (inveGroup == null)
                return null;
            
          return  inveGroup.GetSelected();
    }


        public InventaryGroup GetGroup(string group)
        {
            if (!mappedInventary.ContainsKey(group))
                return null;
            return mappedInventary[group];
        }

        public bool RemoveItem(ItemController item)
        {
            if (mappedInventary.ContainsKey(item.Stats.SlotType))
            {
                return mappedInventary[item.Stats.SlotType].RemoveItem(item);
            }
            return false;
        }

    }

    

        [System.Serializable]
    public class InventaryGroup
    {
        public string SlotType;
        public Transform RealPosition;
        public int MaxCapacity=3;
        public bool ReplaceSelectOnMax = true;


        public ItemEvent AddedItem;
        public ItemEvent RemovedItem;

        private int selIndex;

        public int SelectedIndex
        {
            set { selIndex = value; }
            get { return selIndex; }
        }
        public List<ItemController> items;
        public ItemController GetSelected()
        {

            if (items.Count == 0)
                return null;

            if (selIndex < 0)
                return null;
            if (selIndex < items.Count) 
                return items[selIndex];
            return null;
           
        }
        public bool AddItem(ItemController item)
        {

            //stackear el objeto
            if (Stack(item))
            {
                return true;
            }

            if (items.Count >= MaxCapacity)
            {

                if (ReplaceSelectOnMax)
                {
                    ItemController ic =GetSelected();
                    ic.Drop();
                    items[selIndex] = item;
                    item.Take(RealPosition);

                    if (RemovedItem != null)
                        RemovedItem(ic);
                    if (AddedItem != null)
                        AddedItem(item);

                    return true;
                }
                return false;

            }
            
            items.Add(item);
            item.Take(RealPosition);
            if (AddedItem != null)
                AddedItem(item);


            return true;

        }

        //rellenar inventario
        public bool Stack (ItemController item)
        {
            if (item is ConsumableItem)
            {
                ConsumableItem consumable = (item as ConsumableItem);
                ItemController itContrl = GetItemByName(consumable.Stats.ItemName);

                if (itContrl !=null && itContrl is ConsumableItem )
                {
                    ConsumableItem alreadyInventary = (itContrl as ConsumableItem);

                    int max = alreadyInventary.getConsumableStats().MaxUnits;
                    int totalUnits = alreadyInventary.Units + consumable.Units;
                    int difference = (totalUnits - max);

                    //si la diferencia es positiva es que nos sobran si es negativo o igual a cero nos sobra espacio
                    alreadyInventary.Units = (difference > 0 ? max : totalUnits);
                    consumable.Units = (difference > 0 ? difference :0);

                    if (consumable.Units <=0)
                    {
                        ConsumableItem.Destroy(consumable.gameObject);
                        return true;
                    }
                }



            }
            return false;
        }

        public int GetIndex(ItemController itm)
        {
            return items.IndexOf(itm);

        }
        public void Select (ItemController itm)
        {
            int indx = GetIndex(itm);
            Select(indx);
           
           
        }

        public void Select(int indx) {
            
            
            if(indx != selIndex)
            {
                ItemController item = GetSelected();
                if(item != null)
                {
                    item.Hide();

                }
                SelectedIndex = indx;

                item = GetSelected();
                if (item != null)
                {
                    item.Show();

                }
            
        }
            
        }

        public bool RemoveItem(ItemController item)
        {

            if (items.Remove(item))
            {
                if (RemovedItem != null)
                    RemovedItem(item);
                return true;
            }
            return false;
           
        }

        public ItemController GetItemByName(string name)
        {
            foreach (ItemController it in items)
            {

                if (it.Stats.ItemName == name)
                {
                    return it;
                }

            }

            return null;
        }

    }


//Eventos para que los objetos se me añadan 

public delegate void ItemEvent(ItemController item);


}