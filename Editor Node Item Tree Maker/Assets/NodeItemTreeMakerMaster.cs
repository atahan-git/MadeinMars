using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nova.NodeItemTree {
    public class NodeItemTreeMakerMaster : MonoBehaviour {
        public static NodeItemTreeMakerMaster s;

        public RecipeSet myRecipeSet;

        public RectTransform ItemsParent;
        public GameObject ItemListItemPrefab;

        public RectTransform ItemNodeParent;
        public GameObject ItemNodePrefab;


        private void Awake () {
            s = this;
        }

        private void Start () {
            foreach (ItemSet myItemSet in myRecipeSet.myItemSets) {
                foreach (Item myItem in myItemSet.items) {
                    GameObject itemListItem = Instantiate(ItemListItemPrefab, ItemsParent);
                    itemListItem.GetComponent<DragMe>().myItem = myItem;
                    itemListItem.transform.GetChild(0).GetComponent<Image>().sprite = myItem.mySprite;
                    itemListItem.transform.GetChild(1).GetComponent<Text>().text = myItem.name;
                }
            }
        }


        public void CreateItemNodeAtPosition (PointerEventData data) {
            var originalObj = data.pointerDrag;
            if (originalObj == null)
                return;

            var dragMe = originalObj.GetComponent<DragMe>();
            if (dragMe == null)
                return;

            var myItem = dragMe.myItem;
            if (myItem == null)
                return;

            
        }

        private void Update () {
            
        }
    }

}
