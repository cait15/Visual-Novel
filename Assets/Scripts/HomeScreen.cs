using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class HomeScreenUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject howToPlayPanel;
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private RectTransform panelHowToPlay;
    public GameObject npoPanel;
    [SerializeField] private RectTransform panelnPO;
    [SerializeField] private float scaleDuration = 0.4f;
    [SerializeField] private RectTransform panelHome;
    public Vector3 originalPanelScale;
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private float popAnimationTime = 0.5f;
    
    
    public GameObject oPTIONSpanel;
    [SerializeField] private RectTransform panelOptions;


    private void Start()
    {
        originalPanelScale = panelHome.localScale;
        AnimateButtons();
    }
    
    private void AnimateButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform b = buttons[i];
            b.localScale = Vector3.zero;

            LeanTween.scale(b, Vector3.one * 0.3f, popAnimationTime)
                .setEaseOutBack()
                .setDelay(i * delay);
        }
    }

    // === Called by Play Button ===
    public void OnPlayButton()
    {
        SceneManager.LoadScene("Openiing cutscene"); 
    }
    
    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Application Quit"); 
    }
    
    public void ShowHowTOPlay()
    {
        howToPlayPanel.SetActive(true);
        panelHowToPlay.localScale = Vector3.zero;

        LeanTween.scale( panelHowToPlay, originalPanelScale, scaleDuration)
            .setEaseOutBack();
    }
    public void HideHowToPlay()
    {
        LeanTween.scale( panelHowToPlay, Vector3.zero, scaleDuration)
            .setEaseInBack();
    }
    public void ShownPO()
    {
        npoPanel.SetActive(true);
        panelnPO.localScale = Vector3.zero;

        LeanTween.scale(  npoPanel, originalPanelScale, scaleDuration)
            .setEaseOutBack();
    }
    public void HidenPO()
    {
        LeanTween.scale(  npoPanel, Vector3.zero, scaleDuration)
            .setEaseInBack();
    }
    
    public void ShownOptions()
    {
        oPTIONSpanel.SetActive(true);
        panelOptions.localScale = Vector3.zero;

        LeanTween.scale(  oPTIONSpanel, originalPanelScale, scaleDuration)
            .setEaseOutBack();
    }
    public void HidenOptions()
    {
        LeanTween.scale(  oPTIONSpanel, Vector3.zero, scaleDuration)
            .setEaseInBack();
    }
}