using UnityEngine;

public class BowlingBallControl : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] pinHit; //is audioclip for oneshot
    private AudioSource ballRolling; //is audiosource to play continously
    private Rigidbody ballRb;
    public bool isBallFrozen=false;
    public bool isBallPastPins=false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        //require 2 audio sources for handling each audiosource/clip
        audioSource = GetComponents<AudioSource>()[0]; //use this audiosource to play the oneshot AudioClip of pinHit
        ballRolling = GetComponents<AudioSource>()[1]; //use this audiosource to continously play the ballRolling audio
    }

    void OnCollisionEnter(Collision collision)
    {
        //if bowling ball is touching the ground, play the rolling sfx
        if (!ballRolling.isPlaying && collision.gameObject.CompareTag("Ground"))
        {
            ballRolling.Play();
        }
        //if bowling ball hits a pin, play oneshot of pinHit
        else if (collision.gameObject.CompareTag("Bowling Pin"))
        {
            int index = Random.Range(0,5);
            audioSource.PlayOneShot(pinHit[index]);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        ballRolling.Stop(); //if ball isnt touching ground, stop playing sfx
    }

    void OnTriggerEnter(Collider other)
    {   
        //if collides with the freeze layer, freeze the pins movements so they don't move endlessely
        if (other.CompareTag("Freeze Layer"))
        {
            ballRb.constraints = RigidbodyConstraints.FreezeAll;
            isBallFrozen = true;
        }
        else if (other.CompareTag("Past Pins Point"))
        {
            isBallPastPins=true;

            //if past pins, set angular damping on ball to high so it will stop moving faster, allowing resets to happen faster
            ballRb.angularDamping = 10;
        }
    }
}
