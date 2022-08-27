using EE.Core;
using UnityEngine;

namespace EE.StateSystem {
    [System.Serializable]
    public static class HandUtils {
        public static Vector3 MirroredScale => new Vector3(-1, 1, 1);
        public const int FULLROTATION = 360;

        public static float FollowDirectionProvider(Vector2 angleTargetPosition, Vector2 rotationOrigin) {
            Vector2 aimDirection = (angleTargetPosition - rotationOrigin).normalized;
            return Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        }
    }

}
