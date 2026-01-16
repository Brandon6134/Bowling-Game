using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public GameObject velocityBar;
    public GameObject velocityBarOutline;
    public RectTransform velocityBarRectTransform;
    public TextMeshProUGUI scoreAnnouncementText;
    private SpawnManager spawnManagerScript;
    private PlayerController playerControllerScript;
    private AudioSource audioSource;
    public AudioClip[] announcePinsHitSFX;
    public TextMeshProUGUI ballSpeedText;
    public TextMeshProUGUI helpText;
    public TextMeshProUGUI torqueSpeedText;
    public GameObject bowlingSpeedIcon;
    public GameObject[] spinUI;
    private float t=0;
    public float tBar=0;
    private int minA = 0;
    private int maxA = 1;
    private float tMod = 3f;
    private bool fadeText = false;
    private bool pause = false;
    public float minYFixed = 0f;
    public float maxYFixed = 700f;
    public float minY = 0f;
    public float maxY = 700f;
    public bool stopMovingVelocityBar = false;
    private float minSpinX = 100f+960f;
    private float maxSpinX = 930f+960f;
    public float speedOfSpinIndicator;
    public Vector3 spinIndicatorBasePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        velocityBar = GameObject.FindGameObjectWithTag("Velocity Bar");
        Image velocityBarImage = velocityBar.GetComponent<Image>();
        velocityBarImage.color = Color.green;
        
        velocityBarRectTransform = velocityBar.GetComponent<RectTransform>();
        velocityBar.SetActive(false);

        velocityBarOutline = GameObject.FindGameObjectWithTag("Velocity Bar Outline");
        velocityBarOutline.SetActive(false);
        
        ballSpeedText.enabled = false;
        torqueSpeedText.enabled=false;
        bowlingSpeedIcon.SetActive(false);

        helpText.outlineColor = Color.black;
        helpText.outlineWidth = 0.15f;
        helpText.enabled = false;

        spinIndicatorBasePosition = spinUI[0].transform.position;

        //set all spinUI objects inactive
        foreach (GameObject obj in spinUI)
        {
            obj.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //if space is pressed down, velocity bar should be moving still, and game isnt paused
        if (playerControllerScript.spacePressed && !stopMovingVelocityBar && Time.deltaTime!=0)
        {
            (tBar, minY, maxY) = VelocityBarChange(velocityBarRectTransform, tBar, minY, maxY);
            SpinGaugeChange(spinUI,minSpinX,maxSpinX);
        }
        
        FadeOutAndStop();
    }

    public (float,float,float) VelocityBarChange(RectTransform rect, float tBar, float minY, float maxY)
    {
        velocityBar.SetActive(true);
        velocityBarOutline.SetActive(true);
        bowlingSpeedIcon.SetActive(true);

        //change rectangle height
        rect.sizeDelta = new (100, Mathf.Lerp(minY,maxY,tBar));

        //increase the t value over time, so when this function is called again in Update() Mathf.Lerp returns higher pos values, eventually reaching end pos
        tBar+=1.1f*Time.deltaTime;

        //if this is true, then bar has went up and down once already, so thus stop moving the velocity bar.
        if (tBar>=1 && minY > maxY)
        {
            stopMovingVelocityBar = true;
        }
        //if t=1 then bar has reached max or min size, thus switch the min and max so it starts increasing or decreasing size appropriately
        else if (tBar>=1)
        {
            (minY,maxY) = (maxY,minY);
            tBar=0f;
        }

        return (tBar,minY,maxY);
    }

    public float SpinGaugeChange(GameObject[] objarray, float min, float max)
    {
        foreach (GameObject obj in objarray)
        {
            obj.SetActive(true);
        }

        //don't allow player to move outside bowling lane
        if (objarray[0].transform.position.x < min)
        {
            objarray[0].transform.position = new Vector3(min,objarray[0].transform.position.y,objarray[0].transform.position.z);
        }

        if (objarray[0].transform.position.x > max)
        {
            objarray[0].transform.position = new Vector3(max,objarray[0].transform.position.y,objarray[0].transform.position.z);
        }
        
        float horizontalInput = Input.GetAxis("Horizontal");
        
        //translate green indicator left and right based on user horizontal input
        objarray[0].transform.Translate(Vector3.left * speedOfSpinIndicator * horizontalInput *  Time.deltaTime);


        return max;
    }

    //at the end of each round, announce your score e.g. "9 pins" or "Spare" and play the corresponding sfx with it
    public (bool,bool) AnnounceScore(int pinsHit, bool isSpare, bool isStrike)
    {
        string text="";

        if (isStrike)
        {
            text = "Strike!";
            audioSource.PlayOneShot(announcePinsHitSFX[2],1.25f);
            isStrike=false;
        }
        else if (isSpare)
        {
            text = "Spare!";
            audioSource.PlayOneShot(announcePinsHitSFX[1],1.25f);
            isSpare=false;
        }
        else if (pinsHit==0)
        {
            text = "Miss..";
            audioSource.PlayOneShot(announcePinsHitSFX[3],1.25f);
        }
        else
        {
            text = pinsHit.ToString() + " pins";
            audioSource.PlayOneShot(announcePinsHitSFX[0],1.25f);
        }

        scoreAnnouncementText.text = text;
        scoreAnnouncementText.outlineColor = Color.black;
        scoreAnnouncementText.outlineWidth = 0.15f;

        fadeText=true;

        return (isSpare,isStrike);
    }

    //fade's the given text's .a property and returns that and the t value
    public (Color,float) FadeText(TextMeshProUGUI text, float t, int min, int max,float tModifier)
    {
        //Debug.Log("fading text! t: " + t);
        Color c = text.color;
        c.a = Mathf.Lerp(min,max,t);

        //increase the t value over time
        t+=tModifier*Time.deltaTime;

        return (c,t);
    }

    private void FadeOutAndStop()
    {
        
        //if text is opaque (a>=1) and t>=1, then start the fadeout by switching min and max values and resetting t=0
        if (scoreAnnouncementText.color.a >= 1f && t>=1 && !pause )
        {
            //set pause to true, so the fadeout timer func is only called once
            pause = true;
            StartCoroutine(FadeOutScoreDisplayTimer(1f));
        }

        //if text is inivisible (a<=0) and the min and max values have been switched (has faded in and out already), stop the fade calls
        else if (scoreAnnouncementText.color.a <= 0f && minA > maxA && fadeText)
        {
            //Debug.Log("stop fade");
            fadeText = false;
            
            //reset t and set max and min to original values
            t=0;
            (maxA,minA) = (minA,maxA);
            pause = false;
        }

        //if we want to fadeText in or out, call func repeatedly in update() to adjust .a and t values to transition opacity levels
        if (fadeText)
        {
            (scoreAnnouncementText.color,t) = FadeText(scoreAnnouncementText,t,minA,maxA,tMod);
        }
    }

    //a timer that waits seconds, then sets certain variables to new values to begin announce score text to fade out
    IEnumerator FadeOutScoreDisplayTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        //Debug.Log("start fadeOut");
        (maxA,minA) = (minA,maxA);
        t=0;

    }
}
