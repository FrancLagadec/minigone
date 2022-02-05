using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;

namespace YsoCorp {

    public class Ennemy : YCBehaviour {

        private static float LEFT_LIMIT = -2f;
        private static float RIGHT_LIMIT = 2f;
        private static float SPEED_ROTATION = 25f;
        private static float SPEED_ACCELERATION = 0.5f;
        //private static float SPEED = 4f;
        private static float SPEED = 5f;
        private static float ROTATION_SENSITIVITY = 0.2f;
        private static float MAX_ANGLE = 35f;

        public bool movementsWithRotation;
        public bool preventFall;

        private bool _isMoving;

        private Transform _playerPos;
        private Vector3 _slideMove;
        private Animator _animator;
        private Quaternion _rotation;
        private Rigidbody _rigidbody;
        private RagdollBehaviour _ragdollBehviour;
        private CapsuleCollider _capsuleCollider;
        private TweenerCore<float, float, FloatOptions> _rotationTween;

        public bool isAlive { get; protected set; }
        public float speed { get; private set; }

       protected override void Awake() {
            this._rigidbody = this.GetComponent<Rigidbody>();
            this._animator = this.GetComponentInChildren<Animator>();
            this._rotation = Quaternion.Euler(0f, 180f, 0f);;
            this._ragdollBehviour = this.GetComponent<RagdollBehaviour>();
            this._capsuleCollider = this.GetComponent<CapsuleCollider>();
            this.isAlive = true;
        }

        private void FixedUpdate() {

            if (this.game.state != Game.States.Playing && this._isMoving == true) {
                this._isMoving = false;
                this._animator?.SetBool("Moving", false);
            }

            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            if (this._isMoving == false) {
                this._isMoving = true;
                this._animator?.SetBool("Moving", true);
            }
            
            this.speed += SPEED_ACCELERATION;
            this.speed = Mathf.Clamp(this.speed, 0, SPEED);
            if (this.speed != 0) {
                Vector3 dir = (this.player.transform.position - this._rigidbody.position).normalized;
                this._rigidbody.MovePosition(this._rigidbody.position + dir * this.speed * Time.fixedDeltaTime);
                this._rigidbody.transform.forward = dir;
                this._rigidbody.MoveRotation(Quaternion.RotateTowards(this._rigidbody.rotation, this._rotation, SPEED_ROTATION));
                this._slideMove = Vector3.zero;

                if (this.preventFall == true)
                    this.BlockPlayerFromFalling();
                
                if (Vector3.Distance(this._rigidbody.position, this.player.transform.position) < 1.5f) {
                    Tackle();
                }
            }
        }

        public void Tackle() {
            if (!this.isAlive)
                return;
            
            this.isAlive = false;
            this._rigidbody.useGravity = false;
            this._capsuleCollider.enabled = false;

            if (this._ragdollBehviour != null)
                this._ragdollBehviour.EnableRagdoll(this.player.transform);
        }

        private void BlockPlayerFromFalling() {
            this._rigidbody.position = new Vector3(Mathf.Clamp(this._rigidbody.position.x, LEFT_LIMIT, RIGHT_LIMIT), this._rigidbody.position.y, this._rigidbody.position.z);
        }

        private void ResetRotation() {
            float duration = this._rotation.y / 40;
            this._rotationTween.Kill();

            this._rotationTween = DOTween.To(
                () => this._rotation.y,
                (value) => this._rotation = Quaternion.Euler(0f, value, 0),
                0, duration).SetEase(Ease.Linear);
        }

        public void RotateClamped(float deltaX) {
            float eulerAngle = transform.localEulerAngles.y;
            
            eulerAngle = (eulerAngle > 180) ? eulerAngle - 360 : eulerAngle;
            
            float angle = eulerAngle + deltaX * ROTATION_SENSITIVITY;
            float clampedAngle = Mathf.Clamp(angle, -MAX_ANGLE, MAX_ANGLE);

            this._rotation = Quaternion.Euler(0f, clampedAngle, 0);
        }

    }

}