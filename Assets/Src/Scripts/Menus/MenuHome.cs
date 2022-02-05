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
        public GameObject gridShop;
        public Button bClose;

        public Button bBack;
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

            this.bShop.onClick.AddListener(() => {
                this.gridShop.transform.parent.gameObject.SetActive(true);
            });

            this.bBack.onClick.AddListener(() => {
                this.gridShop.transform.parent.gameObject.SetActive(false);
            });

            for (int i = 0; i < this.Purchase.Length; i++) {
                this.Purchase[i].onClick.AddListener(() => {
                    Debug.Log("Purchase Call button " + i);
                });
            };

            for (int i = 0; i < this.Select.Length; i++) {
                this.Select[i].onClick.AddListener(() => {
                    Debug.Log("Call button " + i);
                });
            };
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
        }
    }

}
