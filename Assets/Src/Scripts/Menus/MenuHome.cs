using UnityEngine;
using UnityEngine.UI;

namespace YsoCorp {

    public  class MenuHome : AMenu {

        public Button bPlay;
        public Button bLevels;
        public Button bSetting;
        public Button bShop;
        public Button bRemoveAds;

        public GameObject gridLayout;
        public Button bClose;

        public Button bBack;
        public GameObject gridShop;
        public Button[] Purchase;
        public Button[] Select;

        public Text levelIndex;

        void Start() {
            this.bPlay.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    this.game.state = Game.States.Playing;
                });
            });

            this.bLevels.onClick.AddListener(() => {
                this.gridLayout.transform.parent.gameObject.SetActive(true);
            });

            this.bShop.onClick.AddListener(() => {
                this.gridShop.transform.parent.gameObject.SetActive(true);
            });

            this.bSetting.onClick.AddListener(() => {
                this.ycManager.settingManager.Show();
            });

            this.bRemoveAds.onClick.AddListener(() => {
                this.ycManager.inAppManager.BuyProductIDAdsRemove();
            });

            foreach(Transform child in gridLayout.transform) {
                child.GetComponent<Button>().onClick.AddListener(() => {
                    this.dataManager.SetLevel(child.GetSiblingIndex() + 1);
                });
            }

            this.bClose.onClick.AddListener(() => {
                this.gridLayout.transform.parent.gameObject.SetActive(false);
            });

            this.bBack.onClick.AddListener(() => {
                this.gridShop.transform.parent.gameObject.SetActive(false);
            });

            foreach (Button BPurchase in Purchase) {
                BPurchase.onClick.AddListener(() => {
                    this.PurchaseItem(BPurchase);
                });
            }

            foreach (Button BSelect in Select) {
                BSelect.onClick.AddListener(() => {
                    this.SelectItem(BSelect);
                });
            }
        }

        void FixedUpdate() {
            levelIndex.text =  "Level " + this.dataManager.GetLevel();
        }

        void OnEnable() {
            Transform tmpStar;

            foreach(Transform child in gridLayout.transform) {
                if (child.GetSiblingIndex() < this.dataManager.GetLevelMax()) {
                    child.gameObject.SetActive(true);
                    tmpStar = child.GetChild(0);
                    if (this.dataManager.StarInLevelIsTaken(child.GetSiblingIndex()))
                        tmpStar.gameObject.SetActive(true);
                    else
                        tmpStar.gameObject.SetActive(false);
                } else
                    child.gameObject.SetActive(false);
            }

            Transform tmpItem;
            foreach(Button BPurchase in Purchase) {
                tmpItem = BPurchase.transform.parent;
                if (this.dataManager.ItemInShopIsPurchased(tmpItem.GetSiblingIndex())) {
                    BPurchase.gameObject.SetActive(false);
                    Select[tmpItem.GetSiblingIndex()].gameObject.SetActive(true);
                } else {
                    BPurchase.gameObject.SetActive(true);
                    Select[tmpItem.GetSiblingIndex()].gameObject.SetActive(false);
                }
            }
        }

        void PurchaseItem(Button BPurchase) {
            
            if (this.dataManager.GetStarNb() < 5)
                return;
            
            int index = BPurchase.transform.parent.GetSiblingIndex();
            Debug.Log("Purchase button " + index);

            this.dataManager.updateShop(index);
            if (this.dataManager.ItemInShopIsPurchased(index)) {
                this.dataManager.SetStarNb(this.dataManager.GetStarNb() - 5);
                BPurchase.gameObject.SetActive(false);
                Select[index].gameObject.SetActive(true);

                if (index < 5) {
                    this.dataManager.SetCurrentSkin(index);
                    this.player.UpdateSkin();
                }
            }
        }

        void SelectItem(Button BSelect) {

            int index = BSelect.transform.parent.GetSiblingIndex();
            Debug.Log("Select button " + index);

            if (index >= 5)
                return;
            this.dataManager.SetCurrentSkin(index);
            this.player.UpdateSkin();
        }
    }

}
