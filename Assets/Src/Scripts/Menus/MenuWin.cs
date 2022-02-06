using UnityEngine;
using UnityEngine.UI;


namespace YsoCorp {

    public  class MenuWin : AMenu {

        public Button bRetry;
        public Button bNext;
        public GameObject star;

        void Start() {
            this.bRetry.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    if (this.dataManager.GetLevel() < 30)
                        this.dataManager.PrevLevel();
                    this.game.state = Game.States.Home;
                });
            });

            this.bNext.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    this.game.state = Game.States.Home;
                });
            });
        }

        void OnEnable() {
            if (star == null)
                return;
            
            if (this.player.nbStar > 0)
                star.SetActive(true);
            else
                star.SetActive(false);
        }

    }

}
