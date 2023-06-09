using Sounds;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

namespace Sounds
{
    public class ObjectPickUp_Josh1 : MonoBehaviour
    {
        FirstPersonController firstPersonController;

        [SerializeField] private GameObject item;
        [SerializeField] private GameObject tempHold;
        [SerializeField] private AudioSource source = null;
        [SerializeField] private bool isHolding = false;
        [SerializeField] private bool isThrown = false;
        [SerializeField] private bool isStandingOn = false;
        [SerializeField] private float throwForce;
        [SerializeField] private float soundRange;
        private GameObject normalReticle;
        private GameObject grabReticle;
        private GameObject throwReticle;
        

        // public bool grabbable = false;
        private float distance;
        private Vector3 objectPos;
        private bool afterStart = false;

        private void Start()
        {
            normalReticle = GameObject.Find("Reticle D");
            grabReticle = GameObject.Find("Reticle G");
            throwReticle = GameObject.Find("Reticle T");
            afterStart = true;
        }
        void Update()
        {
            if (afterStart)
            {
                grabReticle.SetActive(false);
                normalReticle.SetActive(true);
                throwReticle.SetActive(false);
                afterStart = false;
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
            if (source.isPlaying)
            {
                return;
            }

            source.Play();

            var sound = new ObjectSound(transform.position, soundRange);

            ObjectSoundManager.MakeSound(sound);
        }

        void OnMouseDown()
        {
            if (isStandingOn == false)
            {
                Rigidbody body = item.GetComponent<Rigidbody>();

                if (distance <= 3.5)
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
            isHolding = false;
            //grabbable = false;
            grabReticle.SetActive(false);
            normalReticle.SetActive(true);
            throwReticle.SetActive(false);
        }

        void OnMouseOver()
        {
            if (isStandingOn == false)
            {
                Rigidbody body = item.GetComponent<Rigidbody>();

                if (distance <= 3.5)
                {
                    body.useGravity = false;
                    body.detectCollisions = true;
                    grabReticle.SetActive(true);
                    normalReticle.SetActive(false);
                    throwReticle.SetActive(false);
                }
            }
        }

        private void OnMouseExit()
        {
            grabReticle.SetActive(false);
            normalReticle.SetActive(true);
            throwReticle.SetActive(false);
        }


        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                isStandingOn = true;
            }
            else if (collision.gameObject.tag == "Mascot")
            {
                /// make no sound
            }
            else
            {
                if (isThrown == true)
                {
                    MakeASound();
                    isThrown = false;
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
    }
}
