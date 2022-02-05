using UnityEngine;

namespace YsoCorp {

    public class Star : YCBehaviour {

        private Vector3 rotation = new Vector3(0f, 5f, 0f);
        private void FixedUpdate() {
            //this._rigidbody.MoveRotation(Quaternion.RotateTowards(this._rigidbody.rotation, this._rotation, SPEED_ROTATION));
            this.transform.Rotate(rotation, Space.World);
        }
    }

}
