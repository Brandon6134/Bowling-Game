using System.Runtime.CompilerServices;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject velocityBar;
    public RectTransform velocityBarRectTransform;
    private float t = 0f;
    private float minY = 0f;
    private float maxY = 700f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocityBar = GameObject.FindGameObjectWithTag("Velocity Bar");
        velocityBarRectTransform = velocityBar.GetComponent<RectTransform>();

        howToPlayPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (howToPlayPanel.activeInHierarchy)
        {
            (t,minY,maxY) = VelocityBarChange(velocityBarRectTransform,t,minY,maxY);
        }
    }

    public void LoadGameScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    public void HowToPlayButton()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void ReturnButton()
    {
        howToPlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    //simplified velocity bar func from UIManager, but it just goes up and down forever with no player input
    public (float,float,float) VelocityBarChange(RectTransform rect, float tBar, float minY, float maxY)
    {
        //change rectangle height
        rect.sizeDelta = new (100, Mathf.Lerp(minY,maxY,tBar));

        //increase the t value over time, so when this function is called again in Update() Mathf.Lerp returns higher pos values, eventually reaching end pos
        tBar+=1.1f*Time.deltaTime;

        //if t=1 then bar has reached max or min size, thus switch the min and max so it starts increasing or decreasing size appropriately
        if (tBar>=1)
        {
            (minY,maxY) = (maxY,minY);
            tBar=0f;
        }

        return (tBar,minY,maxY);
    }

    
}
