using UnityEngine;

namespace YsoCorp {

    public class ObstacleCollision : YCBehaviour {

        private void OnCollisionEnter(Collision collision) {
            if (!this.player.isAlive)
                return;
            
            if (collision.transform.CompareTag("Obstacle") == true) {
                this.ycManager.soundManager.PlayEffect("hit2", 0.05f);
                this.KillPlayer(collision.transform);
            } else if (collision.transform.CompareTag("Star") == true) {
                this.ycManager.soundManager.PlayEffect("pickUp", 0.2f);
                this.player.AddStar();
                Destroy(collision.gameObject);
            } else if (collision.transform.CompareTag("Ennemy") == true) {
                this.ycManager.soundManager.PlayEffect("hit9", 0.05f);
                this.KillPlayer(collision.transform);
            }
        }

        private void OnTriggerEnter(Collider collision) {
            if (!this.player.isAlive)
                return;
            
            if (collision.transform.CompareTag("Obstacle") == true) {
                this.ycManager.soundManager.PlayEffect("hit2", 0.05f);
                this.KillPlayer(collision.transform);
            } else if (collision.transform.CompareTag("Ennemy") == true) {
                this.ycManager.soundManager.PlayEffect("hit9", 0.05f);
                this.KillPlayer(collision.transform);
            } else if (collision.transform.CompareTag("Star") == true) {
                this.ycManager.soundManager.PlayEffect("pickUp", 0.2f);
                this.player.AddStar();
                Destroy(collision.gameObject);
            } else if (collision.transform.CompareTag("Finish") == true) {
                this.ycManager.soundManager.PlayEffect("Applause", 0.05f);
                this.Finish();
            }
        }

        private void KillPlayer(Transform killer) {
            if (this.player.isAlive) {
                this.player.Die(killer);
                this.game.Lose();
            }
        }

        private void Finish() {
            this.player.StoreStar();
            this.game.Win();
        }

    }

}