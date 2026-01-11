using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float horizontalInput;
    public float speed;
    public float zRange;
    private float rotateYRange = 0.5f;
    public float rotateSpeed;
    public GameObject bowlingBall;
    public float bowlingBallSpeed;
    private CameraControl cameraControlScript;
    private Vector3 camOffset;
    private Animator playerAnim;
    private SpawnManager spawnManagerScript;
    private UIManager UIManagerScript;
    public bool spacePressed = false;
    private Rigidbody playerRb;
    

    void Start()
    {
        cameraControlScript = GameObject.Find("Main Camera").GetComponent<CameraControl>();
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        UIManagerScript = GameObject.Find("UI Manager").GetComponent<UIManager>();
        playerAnim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();


        //make bowling ball spawn at an offset so that camera transition between player and ball is smooth
        camOffset = cameraControlScript.playerOffset - cameraControlScript.ballOffset;
    }

    // Update is called once per frame
    void Update()
    {
        //if game is active, allow player control
        if (spawnManagerScript.isGameActive)
        {
            //if camera isn't on the scoreboard, allow player movement (when player exits scoreboard, can immediately move so feels nice and not restrictve)
            if (!cameraControlScript.camOnScores)
            {
                horizontalMovement();
                rotationalMovement();
            }

            //camBackOnPlayer indicates if the camera is approximately behind the player, aka cam is at the end of the transition (not midway transition) and on player
            //used Distance() to approximate equallness cause there was a delay between the two vector3's values making them equal after some time, not always.
            bool camBackOnPlayer = Vector3.Distance(cameraControlScript.transform.position,transform.position + cameraControlScript.playerOffset) < 0.1f;

            //throw bowling ball only if none exist currently (only can throw one ball at a time), and if cam is on the player (not mid transition)
            if (Input.GetKeyDown(KeyCode.Space) && !GameObject.FindGameObjectWithTag("Bowling Ball") && camBackOnPlayer && !spacePressed)
            {
                spacePressed=true;
                playerAnim.SetInteger("Animation_int",5);
            }
            //if entered moveforwardsequence, start bowling veloctiy bar UI + minigame
            else if(Input.GetKeyDown(KeyCode.Space) && !GameObject.FindGameObjectWithTag("Bowling Ball") && camBackOnPlayer && spacePressed)
            {
                //calculate percentage of bar full (0 to 1)
                float barPercent = UIManagerScript.velocityBarRectTransform.sizeDelta[1] / UIManagerScript.maxYFixed;

                //make the actual percent modifier range from 0.5 to 1 for speed balance
                float usedPercent = 0.5f + barPercent/2;

                //Debug.Log("barPercent: " + barPercent);
                //Debug.Log("usedPercent: "+ usedPercent);

                CreateAndMoveBall(usedPercent);
            }
            
            if (spacePressed)
            {
                MoveForwardSequence();
            }
        }
        
    }

    void horizontalMovement()
    {
        //don't allow player to move outside bowling lane
        if (transform.position.z < -zRange)
        {
            transform.position = new Vector3(transform.position.x,transform.position.y,-zRange);
        }

        if (transform.position.z > zRange)
        {
            transform.position = new Vector3(transform.position.x,transform.position.y,zRange);
        }
        
        horizontalInput = Input.GetAxis("Horizontal");
        
        //translate w/ respect to world, so can move left and right globally (not accounting for rotation)
        transform.Translate(Vector3.back * speed * horizontalInput *  Time.deltaTime,Space.World);

        //set moving animation for player
        if (horizontalInput!=0)
        {
            playerAnim.SetBool("Static_b",true);
            playerAnim.SetFloat("Speed_f",0.5f);
        }
        else
        {
            playerAnim.SetBool("Static_b",false);
            playerAnim.SetFloat("Speed_f",0f);
        }
    }

    void rotationalMovement()
    {
        //don't allow player to rotate past the rotation limits (approx 60 degrees)
        if (transform.rotation.y < -rotateYRange)
        {
            transform.rotation = new Quaternion(transform.rotation.x,-rotateYRange,transform.rotation.z,transform.rotation.w);
        }

        if (transform.rotation.y > rotateYRange)
        {
            transform.rotation = new Quaternion(transform.rotation.x,rotateYRange,transform.rotation.z,transform.rotation.w);
        }

        //allow player to rotate with Q and E buttons
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0,-rotateSpeed,0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0,rotateSpeed,0);
        }
    }

    private void MoveForwardSequence()
    {
        transform.Translate(Vector3.right * 2f *  Time.deltaTime,Space.World);
        playerAnim.speed=0.4f;
        playerAnim.SetBool("Static_b",true);
        playerAnim.SetFloat("Speed_f",0.5f);
    }

    private void CreateAndMoveBall(float percentModifier)
    {
        GameObject bowlingClone = Instantiate(bowlingBall,gameObject.transform.position + camOffset,bowlingBall.transform.rotation);
        Rigidbody bowlingRb = bowlingClone.GetComponent<Rigidbody>();

        //add force to ball, with a mulitplier from the percentage of bar filled
        bowlingRb.AddForce(transform.right * bowlingBallSpeed * percentModifier,ForceMode.Impulse);
        spacePressed = false;
    }

    //if player reaches the start of alley, force ball throw with mininum ball speed mulitplier of 0.5
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Alley Starting Point"))
        {
            CreateAndMoveBall(0.5f);
        }
    }
}
