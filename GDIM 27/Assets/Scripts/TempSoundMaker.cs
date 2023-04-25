using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sounds
{
    public class TempSoundMaker : MonoBehaviour
    {
        [SerializeField] private AudioSource source = null;
        [SerializeField] private float soundRange = 25f;

        private void OnMouseDown()
        {
            // doesn't allow for sounds to overlap
            if (source.isPlaying)
            {
                return;
            }

            source.Play();

            var sound = new ObjectSound(transform.position, soundRange);

            //ObjectSoundManager.MakeSound(sound);
        }
    }
}
