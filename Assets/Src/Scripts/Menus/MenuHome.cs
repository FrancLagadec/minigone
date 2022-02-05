using UnityEngine;
using UnityEngine.UI;

namespace YsoCorp {

    public  class MenuHome : AMenu {

        public Button bPlay;
        public Button bLevels;
        public Button bSetting;
        public Button bRemoveAds;
        public GameObject gridLayout;
        public Button bClose;


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
        }

        void FixedUpdate() {
            levelIndex.text =  "Level " + this.dataManager.GetLevel();
        }

        void OnEnable() {
            foreach(Transform child in gridLayout.transform) {
                if (child.GetSiblingIndex() < this.dataManager.GetLevelMax())
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
    }

}
