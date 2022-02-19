using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;

namespace YsoCorp {

    public class Player : Movable {

        private static float LEFT_LIMIT = -2f;
        private static float RIGHT_LIMIT = 2f;
        private static float SPEED_ROTATION = 25f;
        private static float SPEED_ACCELERATION = 0.5f;
        private static float SPEED = 4f;
        private static float SPEED_SPRINT = 8f;
        private static float ROTATION_SENSITIVITY = 0.2f;
        private static float MOVE_SENSITIVITY = 0.01f; 
        //private static float MAX_ANGLE = 35f;
        private static float MAX_ANGLE = 60f;

        public bool movementsWithRotation;
        public bool preventFall;
        public int nbStar;
        private bool _isMoving;
        private bool _isSprinting;
        private float _sprintTime;
        private Vector3 _slideMove;
        private Animator _animator;
        private Quaternion _rotation;
        private Rigidbody _rigidbody;
        private ParticleSystem _sprintSmoke;
        
        private RagdollBehaviour _ragdollBehviour;
        private TweenerCore<float, float, FloatOptions> _rotationTween;
        public SkinnedMeshRenderer _skinCharacter;
        public SkinnedMeshRenderer _skinRagdoll;
        public Material[] _skinTab;

        public bool isAlive { get; protected set; }
        public float speed { get; private set; }

        protected override void Awake() {
            this._rigidbody = this.GetComponent<Rigidbody>();
            this._animator = this.GetComponentInChildren<Animator>();
            this._ragdollBehviour = this.GetComponent<RagdollBehaviour>();
            this._sprintSmoke = this.GetComponentInChildren<ParticleSystem>();
            this.isAlive = true;
            this.nbStar = 0;
            this.StopSprint();
            this.UpdateSkin();

            this.game.onStateChanged += this.Launch;
        }

        private void Launch(Game.States states) {
            if (states == Game.States.Playing) {
                this._isMoving = true;
                this.nbStar = 0;
                this._animator?.SetBool("Moving", true);
            } else if (states == Game.States.Win) {
                this._isMoving = false;
                this._animator?.SetBool("Moving", false);
            } else if (states == Game.States.Lose) {
                this._isMoving = false;
            }
            
            this.StopSprint();
        }

        public void Reset() {
            this.player.UpdateSkin();
            this.isAlive = true;
            this._isMoving = false;
            if (this._animator != null) {
                this._animator.enabled = true;
                this._animator.SetBool("Moving", false);
            }

            Transform spot = this.game.map.GetStartingPos();
            this.transform.position = spot.position;
            this.transform.rotation = spot.rotation;
            this._rigidbody.velocity = Vector3.zero;
            this._ragdollBehviour?.Reset();
            this.cam.Follow(this.transform);

            this._rotation = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0);
        }

        public void Die(Transform killer) {
            if (this.isAlive)
                this.isAlive = false;
            else
                return;

            StopSprint();

            if (this._ragdollBehviour != null) {
                this._ragdollBehviour.EnableRagdoll(killer);
                this.cam.Follow(this._ragdollBehviour.hips);
            }
        }

        public void AddStar() { this.nbStar += 1; }

        public void StoreStar() {
            if (nbStar > 0 && this.dataManager.StarInLevelIsTaken(this.dataManager.GetLevel()) == false)
                    this.dataManager.updateStarOnLevel(this.dataManager.GetLevel());
        }

        public void UpdateSkin() {
            int index = this.dataManager.GetCurrentSkin();

            if (index > _skinTab.Length)
                return;
            
            _skinCharacter.material = _skinTab[index];
            _skinRagdoll.material = _skinTab[index];
        }

        private void FixedUpdate() {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            if (this._isSprinting) {
                if (this._sprintTime <= 0)
                    this.StopSprint();
                else
                    this._sprintTime -= Time.deltaTime;
            }

            if (this._isMoving == true) {
                this.speed += SPEED_ACCELERATION;
            } else {
                this.speed -= SPEED_ACCELERATION * 3f;
            }            

            this.speed = Mathf.Clamp(this.speed, 0, (_isSprinting ? SPEED_SPRINT: SPEED));
            if (this.speed != 0) {
                this._rigidbody.MovePosition(this._rigidbody.position + this.transform.forward * this.speed * Time.fixedDeltaTime + this._slideMove);
                this._rigidbody.MoveRotation(Quaternion.RotateTowards(this._rigidbody.rotation, this._rotation, SPEED_ROTATION));
                this._slideMove = Vector3.zero;

                if (this.preventFall == true) {
                    this.BlockPlayerFromFalling();
                }
            }
        }

        public void StartSprint() {
            if (this._isSprinting)
                return;
            
            this._isSprinting = true;
            this._sprintTime = 1f;
            this._sprintSmoke.gameObject.SetActive(true);
            this._animator.SetBool("Sprinting", true);
        }

        public void StopSprint() {
            this._isSprinting = false;
            this._sprintTime = 0f;
            this._sprintSmoke.gameObject.SetActive(false);
            this._animator.SetBool("Sprinting", false);
        }

        private void BlockPlayerFromFalling() {
            this._rigidbody.position = new Vector3(Mathf.Clamp(this._rigidbody.position.x, LEFT_LIMIT, RIGHT_LIMIT), this._rigidbody.position.y, this._rigidbody.position.z);
        }

        public override void GesturePanDown() {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            this._rotationTween.Kill();

            this._isMoving = true;
        }

        public override void GesturePanDeltaX(float deltaX) {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            if (this.movementsWithRotation == true) {
                this.RotateClamped(deltaX);
            } else {
                this.Slide(deltaX);
            }
        }

        private void Slide(float deltaX) {
            this._slideMove = new Vector3(deltaX * MOVE_SENSITIVITY, 0 , 0);
        }

        public override void GesturePanUp() {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            this.ResetRotation();
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