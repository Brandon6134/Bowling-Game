using System.Runtime.CompilerServices;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject pauseButton;
    public GameObject velocityBar;
    public RectTransform velocityBarRectTransform;
    private float t = 0f;
    private float minY = 0f;
    private float maxY = 700f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //mainMenuPanel.SetActive(true);
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
        Time.timeScale = 1;
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

    public void PauseMenuButton()
    {
        mainMenuPanel.SetActive(true);
        pauseButton.SetActive(false);

        //pause game and its time
        Time.timeScale = 0;
    }

    public void ResumeButton()
    {
        mainMenuPanel.SetActive(false);
        pauseButton.SetActive(true);

        //unpause game and its time
        Time.timeScale = 1;
    }

    //simplified velocity bar func from UIManager, but it just goes up and down forever with no player input
    public (float,float,float) VelocityBarChange(RectTransform rect, float tBar, float minY, float maxY)
    {
        //change rectangle height
        rect.sizeDelta = new (100, Mathf.Lerp(minY,maxY,tBar));

        //increase t overtime, multiply by unscaledDeltaTime so even game is paused (time.timeScale=0) it still scales with time (exclusive for UI)
        tBar+=1.1f*Time.unscaledDeltaTime;

        //if t=1 then bar has reached max or min size, thus switch the min and max so it starts increasing or decreasing size appropriately
        if (tBar>=1)
        {
            (minY,maxY) = (maxY,minY);
            tBar=0f;
        }

        return (tBar,minY,maxY);
    }

    
}
