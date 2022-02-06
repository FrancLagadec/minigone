using UnityEngine;
using UnityEngine.UI;

namespace YsoCorp {

    public class MenuGame : AMenu {

        public Button bBack;
        public Button bSprint;
        public Joystick joystick;
        public GameObject star;

        void Start() {
            this.bBack.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    this.game.state = Game.States.Home;
                });
            });

            this.bSprint.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    this.player.StartSprint();
                    this.bSprint.gameObject.SetActive(false);
                });
            });
        }

        void OnEnable() {
            this.bSprint.gameObject.SetActive(this.dataManager.ItemInShopIsPurchased(5));
            if (star != null)
                star.SetActive(this.player.nbStar > 0);
        }

        void FixedUpdate() {
            if (star != null)
                star.SetActive(this.player.nbStar > 0);
        }
    }

}
