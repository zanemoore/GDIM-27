using UnityEngine;

namespace Sounds
{
    public static class ObjectSoundManager
    {
        public static void MakeSound(ObjectSound sound)
        {
            Collider[] collider = Physics.OverlapSphere(sound.position, sound.range);

            for (int i = 0; i < collider.Length; i++)
            {
                if (collider[i].TryGetComponent(out MascotHearing hear))
                {
                    hear.RespondToSound(sound);
                }
            }
        }
    }
}
