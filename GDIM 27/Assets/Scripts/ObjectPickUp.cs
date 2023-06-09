using Sounds;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

namespace Sounds
{
    public class ObjectPickUp : MonoBehaviour
    {
        FirstPersonController firstPersonController;

        [SerializeField] private GameObject item;
        [SerializeField] private GameObject tempHold;
        [SerializeField] private FMODUnity.StudioEventEmitter dropEmitter;
        [SerializeField] private bool isHolding = false;
        [SerializeField] private bool isThrown = false;
        [SerializeField] private bool isStandingOn = false;
        [SerializeField] private float throwForce;
        [SerializeField] private float soundRange;
        public bool startGameAttenuation = false;
        private bool waited = false;
        private float timer = 0;
        private float waitTime = 1;
        [SerializeField] private Hiding hide;
        [SerializeField] private GameObject normalReticle;
        [SerializeField] private GameObject grabReticle;
        [SerializeField] private GameObject throwReticle;
        

        private float distance;
        private Vector3 objectPos;
       
        void Update()
        {
            if (!waited) // controls sound at the very beginning
            {
                waitALittle();
            }


            Rigidbody body = item.GetComponent<Rigidbody>();

            distance = Vector3.Distance(item.transform.position, tempHold.transform.position);

            //checks distance of player from object
            if (distance >= 3.5)
            {
                isHolding = false;
            }

            //check if player is holding object
            if (isHolding == true)
            {
                isThrown = false;
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                item.transform.SetParent(tempHold.transform);
                grabReticle.SetActive(false);
                normalReticle.SetActive(false);
                throwReticle.SetActive(true);

                //throws object
                if (Input.GetMouseButtonDown(1))
                {
                    body.AddForce(tempHold.transform.forward * throwForce);
                    isHolding = false;
                    isThrown = true;
                }
            }
            else
            {
                objectPos = item.transform.position;
                item.transform.SetParent(null);
                item.transform.position = objectPos;
                body.useGravity = true;
            }
        }

        public void MakeASound()
        {
            // doesn't allow for sounds to overlap
            if (dropEmitter.IsPlaying())
            {
                return;
            }

            dropEmitter.Play();
            // Debug.Log("Make a sound!");

            var sound = new ObjectSound(transform.position, soundRange);

            ObjectSoundManager.MakeSound(sound);
        }

        void OnMouseDown()
        {
            if (isStandingOn == false)
            {
                Rigidbody body = item.GetComponent<Rigidbody>();

                if (distance <= 3.5 && hide != null && !hide.isHidden && !hide.allowed)
                {
                    isHolding = true;
                    body.useGravity = false;
                    body.detectCollisions = true;
                    grabReticle.SetActive(false);
                    normalReticle.SetActive(false);
                    throwReticle.SetActive(true);
                }
            }
        }
        void OnMouseUp()
        {
            if (hide != null && !hide.allowed)
            {
                isHolding = false;
                grabReticle.SetActive(false);
                normalReticle.SetActive(true);
                throwReticle.SetActive(false);
            }
        }

        void OnMouseOver()
        {
            if (isStandingOn == false)
            {
                Rigidbody body = item.GetComponent<Rigidbody>();

                if (distance > 3.5)
                {
                    grabReticle.SetActive(false);
                    normalReticle.SetActive(true);
                    throwReticle.SetActive(false);
                }
                else
                {
                    if (hide != null && distance <= 3.5 && !hide.isHidden && !hide.allowed)
                    {
                        body.useGravity = false;
                        body.detectCollisions = true;
                        grabReticle.SetActive(true);
                        normalReticle.SetActive(false);
                        throwReticle.SetActive(false);
                    }
                }
                
            }
        }

        private void OnMouseExit()
        {
            if (hide != null && !hide.allowed)
            {
                grabReticle.SetActive(false);
                normalReticle.SetActive(true);
                throwReticle.SetActive(false);
            }
            else
            {
                normalReticle.SetActive(false);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!startGameAttenuation)
            {   
                return; // ignores the first collision at the start of the game
            }

            if (collision.gameObject.tag == "Player")
            {
                isStandingOn = true; // make no sound
            }
            else if (collision.gameObject.tag == "Mascot")
            {
                // make no sound
            }
            else
            {
                if (isThrown == true)
                {
                    isThrown = false; // edited by dare to make sound at all collisions
                }
                if (startGameAttenuation)
                {
                    MakeASound();
                }
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                isStandingOn = false;
            }
        }

        private void waitALittle() // turns on sound after a little
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                waited = true;
                startGameAttenuation = true;
            }
        }
    }
}
