using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public Renderer talkieLightRenderer; 
    public AudioSource walkieAudio;
    public GameObject startPanel; 
    public GameObject dedPanel; 
    public GameObject wonPanel; 
    public FirstPersonLook lookScript;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    public GameObject[] PickUps;

    private int score = 0;
    private float gameTimer = 0f;
    private float changeTrashTagTimer = 0f;

    private Material talkieLightMat;
    private AudioSource audioSource;


    void Start()
    {
        Time.timeScale = 0f; //paused game
        startPanel.SetActive(true);
        dedPanel.SetActive(false);
        wonPanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();

        lookScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        score = 0;
        scoreText.text = "";
        talkieLightMat = talkieLightRenderer.material;
        talkieLightMat.EnableKeyword("_EMISSION");
        TalkieSetEmission(-10f);
    }

    void Update()
    {
        gameTimer += Time.deltaTime;
        changeTrashTagTimer += Time.deltaTime;

        //Skip evil tresh check for first 8 seconds
        if (gameTimer < 10f && gameTimer > 4.8f) return;

        //tag change every 10s
        if (changeTrashTagTimer >= 15f)
        {
            ChangeEvilTrashTags();
            changeTrashTagTimer = 0f; //Reset timer
        }

        bool nearEvilTrash = false;

        foreach (GameObject obj in PickUps)
        {
            if (obj != null && obj.CompareTag("evil-trash"))
            {
                float dist = Vector3.Distance(player.position, obj.transform.position);
                if (dist < 2f) {
                    nearEvilTrash = true;
                    AudioSource src = obj.GetComponent<AudioSource>(); //spooky tune
                    if (src != null && !src.isPlaying) {
                        src.Play();
                    }
                    break;
                }
            }
        }

        if (nearEvilTrash)
        {
            talkieLightMat.SetColor("_EmissionColor", Color.red * Mathf.LinearToGammaSpace(5f));
        } 
        else {
            talkieLightMat.SetColor("_EmissionColor", Color.white * Mathf.LinearToGammaSpace(-10f));
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        startPanel.SetActive(false);
        lookScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        score = 0;
        scoreText.text = score + " / 10";

        StartCoroutine(ChangeEmissionAfterDelay(5f)); //talkie goes on after 5s
    }

    void TalkieSetEmission(float intensity)
    {
        Color baseColor = Color.white; 
        talkieLightMat.SetColor("_EmissionColor", baseColor * Mathf.LinearToGammaSpace(intensity));
    }

    System.Collections.IEnumerator ChangeEmissionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;

        while (elapsed < 1f) //transition for 1s
        {
            float t = elapsed / 1f;
            float currentIntensity = Mathf.Lerp(-10f, 5f, t); //-10 to 3 intensity
            TalkieSetEmission(currentIntensity);
            elapsed += Time.deltaTime;
            yield return null;
        }

        TalkieSetEmission(5f);
            
        if (walkieAudio != null && !walkieAudio.isPlaying)
        {
            walkieAudio.Play();
        }
    }

    public void addScore(){
        score += 1;
        scoreText.text = score + " / 10";

        if (score >= 10) {
            scoreText.text = "";
            audioSource.Stop(); //stop bg music only
            lookScript.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            timerText.text = "Finished in - " + Mathf.RoundToInt(gameTimer) + " sec";
            wonPanel.SetActive(true);
        }
    }

    public void Died(){
        audioSource.Stop();
        scoreText.text = "";
        lookScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        dedPanel.SetActive(true);
    }


    public void RestartGame()
    {
        scoreText.text = "";
        Time.timeScale = 1f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 0f;
        Application.Quit();
    }


    void ChangeEvilTrashTags() {
        Debug.Log("Trash bout to get retaged");
        foreach (GameObject obj in PickUps)
        {
            if (obj != null && obj.CompareTag("evil-trash")) //all to trash
            {
                obj.tag = "trash";
            }
        }

        //Randomly select 3 to mek them evil-trash
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, PickUps.Length);
            if (PickUps[randomIndex] != null && PickUps[randomIndex].CompareTag("trash"))
            {
                PickUps[randomIndex].tag = "evil-trash"; 
            }
        }
    }
}