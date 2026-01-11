using UnityEngine;

public class BowlingPinControl : MonoBehaviour
{
    private Rigidbody pinRb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pinRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Sleep();
    }

    void OnTriggerEnter(Collider other)
    {   
        //if collides with the freeze layer, freeze the pins movements so they don't move endlessely
        if (other.CompareTag("Freeze Layer"))
        {
            pinRb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    //stop bowling pins from infinetly rotating and moving by sleeping (stop calculating physics and setting velocities to zero) them when their velocities get low
    void Sleep()
    {
        if (pinRb.linearVelocity.magnitude < 0.5f || pinRb.angularVelocity.magnitude < 0.5f)
        {
            pinRb.Sleep();
        }
        else
        {
            pinRb.WakeUp();
        }
    }
}
