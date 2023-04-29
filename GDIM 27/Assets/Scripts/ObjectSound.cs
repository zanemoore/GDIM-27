using UnityEngine;

namespace Sounds
{
    public class ObjectSound
    {
        public ObjectSound(Vector3 objectPosition, float objectRange)
        {
            position = objectPosition;
            range = objectRange;
        }

        public readonly Vector3 position;           // the location of where the sound originated
        public readonly float range;                // the area of where the sound can be heard by the AI
    }
}
