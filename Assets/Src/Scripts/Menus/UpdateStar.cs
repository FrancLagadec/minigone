using UnityEngine;
using UnityEngine.UI;

namespace YsoCorp {
    public class UpdateStar : YCBehaviour {

        private Text stars;

        void Awake() {
            stars = this.GetComponent<Text>();
        }

        void FixedUpdate() {
            stars.text = this.dataManager.GetStarNb().ToString();
        }
    }
}