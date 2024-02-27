using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    enum enDirection
    {
        North,
        East,
        West
    };

    public AudioClip[] soundFXClips;
    public TMP_Text distanceScoreText;
    public TMP_Text coinScoreText;
    public TMP_Text bestDistanceScoreText;
    public TMP_Text bestCoinScoreText;
    public GameObject deathMenu;
    public bool mobileEnabled;
    
    private CharacterController characterController;
    private Vector3 playerVector;
    private enDirection playerDirection = enDirection.North;
    private enDirection playerNextDirection = enDirection.North;
    private Animator anim;
    private BridgeSpawner bridgeSpawner;
    private AudioSource audioSource;
    private Gestures gestures;

    private int coinsCollected = 0;
    private int coinsCollectedBest;
    private int distanceRun = 0;
    private int distanceRunBest;
    private float playerStartSpeed = 10.0f;
    private float playerSpeed;
    private float gValue = 10.0f;
    private float translationFactor = 10.0f;
    private float translationFactorMobile = 5.0f;
    private float jumpForce = 1.5f;
    private float timer = 0;
    private float distance = 0;
    private bool canTurnRight = false;
    private bool canTurnLeft = false;
    private bool isDead = false;
    
    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerStartSpeed;
        characterController = this.GetComponent<CharacterController>();
        anim = this.GetComponent<Animator>();
        bridgeSpawner = GameObject.Find("BridgeManager").GetComponent<BridgeSpawner>();
        audioSource = this.GetComponent<AudioSource>();
        playerVector = new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime;
        deathMenu.SetActive(false);
        gestures = this.GetComponent<Gestures>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLogic();
        distanceScoreText.text = distanceRun.ToString();
        coinScoreText.text = "x" + coinsCollected.ToString();
        Debug.Log(Input.acceleration.x);
    }

    void PlayerLogic()
    {
        if (isDead)
        {
            return;
        }

        if (!characterController.enabled)
        {
            characterController.enabled = true;
        }

        timer += Time.deltaTime;
        
        playerSpeed += 0.2f * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && canTurnRight || gestures.swipeRight && canTurnRight)
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.East;
                    this.transform.rotation = Quaternion.Euler(000,090,000);
                    break;
                case enDirection.West:
                    playerNextDirection = enDirection.North;
                    this.transform.rotation = Quaternion.Euler(000,000,000);
                    break;
            }

            gestures.swipeRight = false;
            audioSource.PlayOneShot(soundFXClips[6], 1.0f);
        }
        else if (Input.GetKeyDown(KeyCode.F) && canTurnLeft || gestures.swipeLeft && canTurnLeft)
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.West;
                    this.transform.rotation = Quaternion.Euler(000,-090,000);
                    break;
                case enDirection.East:
                    playerNextDirection = enDirection.North;
                    this.transform.rotation = Quaternion.Euler(000,000,000);
                    break;
            }

            gestures.swipeLeft = false;
            audioSource.PlayOneShot(soundFXClips[6], 1.0f);
        }

        playerDirection = playerNextDirection;

        if (playerDirection == enDirection.North)
        {
            playerVector = Vector3.forward * playerSpeed * Time.deltaTime;
        }
        else if (playerDirection == enDirection.East)
        {
            playerVector = Vector3.right * playerSpeed * Time.deltaTime;
        }
        else if (playerDirection == enDirection.West)
        {
            playerVector = Vector3.left * playerSpeed * Time.deltaTime;
        }

        switch (playerDirection)
        {
            case enDirection.North:
                if (mobileEnabled)
                {
                    if (Input.acceleration.x < 0.1f && Input.acceleration.x > -0.1f)
                    {
                        playerVector.x = 0;
                    }
                    else if (Input.acceleration.x > 0.1f)
                    {
                        playerVector.x = Mathf.Lerp(Input.acceleration.x, 1, translationFactorMobile * Time.deltaTime);
                    }
                    else if (Input.acceleration.x < -0.1f)
                    {
                        playerVector.x = Mathf.Lerp(Input.acceleration.x, -1, translationFactorMobile * Time.deltaTime);
                    }
                }
                else
                {
                    playerVector.x = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                }
                break;
            case enDirection.East:
                if (mobileEnabled)
                {
                    if (Input.acceleration.x < 0.1f && Input.acceleration.x > -0.1f)
                    {
                        playerVector.z = 0;
                    }
                    else if (Input.acceleration.x > 0.1f)
                    {
                        playerVector.z = Mathf.Lerp(-Input.acceleration.x, -1, translationFactorMobile * Time.deltaTime);
                    }
                    else if (Input.acceleration.x < -0.1f)
                    {
                        playerVector.z = Mathf.Lerp(-Input.acceleration.x, 1, translationFactorMobile * Time.deltaTime);
                    }
                }
                else
                {
                    playerVector.z = -Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                }
                break;
            case enDirection.West:
                if (mobileEnabled)
                {
                    if (Input.acceleration.x < 0.1f && Input.acceleration.x > -0.1f)
                    {
                        playerVector.z = 0;
                    }
                    else if (Input.acceleration.x > 0.1f)
                    {
                        playerVector.z = Mathf.Lerp(Input.acceleration.x, 1, translationFactorMobile * Time.deltaTime);
                    }
                    else if (Input.acceleration.x < -0.1f)
                    {
                        playerVector.z = Mathf.Lerp(Input.acceleration.x, -1, translationFactorMobile * Time.deltaTime);
                    }
                }
                else
                {
                    playerVector.z = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || gestures.swipeDown)
        {
            DoSliding();
        }
        
        if (characterController.isGrounded)
        {
            playerVector.y = -0.2f;
        }
        else
        {
            playerVector.y -= (gValue * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded || gestures.swipeUp && characterController.isGrounded)
        {
            audioSource.PlayOneShot(soundFXClips[3], 0.4f);
            anim.SetTrigger("isJumping");
            playerVector.y = Mathf.Sqrt(jumpForce * gValue);
            gestures.swipeUp = false;
        }

        if (this.transform.position.y < -0.5f)
        {
            isDead = true;
            audioSource.PlayOneShot(soundFXClips[2], 0.4f);
            anim.SetTrigger("isTripping");
        }
        
        characterController.Move(playerVector);
        distance = playerSpeed * timer;
        distanceRun = (int)distance;
    }

    void DoSliding()
    {
        characterController.height = 1.0f;
        characterController.center = new Vector3(0, 0.5f, 0);
        characterController.radius = 0;
        StartCoroutine(ReEnableCC());
        audioSource.PlayOneShot(soundFXClips[5], 0.4f);
        anim.SetTrigger("isSliding");
        gestures.swipeDown = false;
    }

    IEnumerator ReEnableCC()
    {
        yield return new WaitForSeconds(0.5f);
        
        characterController.height = 2.0f;
        characterController.center = new Vector3(0, 1.0f, 0);
        characterController.radius = 0.2f;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "LCorner")
        {
            canTurnLeft = true;
        }
        else if (hit.gameObject.tag == "RCorner")
        {
            canTurnRight = true;
        }
        else
        {
            canTurnLeft = false;
            canTurnRight = false;
            gestures.swipeRight = false;
            gestures.swipeLeft = false;
        }

        if (hit.gameObject.tag == "Obstacle")
        {
            isDead = true;
            audioSource.PlayOneShot(soundFXClips[1], 0.4f);
            anim.SetTrigger("isTripping");
            SaveScore();
        }
    }

    private void OnGUI()
    {
        if (isDead)
        {
            deathMenu.SetActive(true);
        }
        
        /*
        GUI.Label(new Rect(10,10,100,20), "Coins: " + coinsCollected.ToString());
        GUI.Label(new Rect(10,40,150,20), "Coins Collected Best: " + PlayerPrefs.GetInt("highscoreC").ToString());
        GUI.Label(new Rect(10,70,100,20), "Distance: " + distanceRun.ToString());
        GUI.Label(new Rect(10,100,150,20), "Distance Run Best: " + PlayerPrefs.GetInt("highscoreD").ToString());
        */
        
    }

    public void DeathEvent()
    {
        deathMenu.SetActive(false);
        bestCoinScoreText.text = "";
        bestDistanceScoreText.text = "";
        characterController.enabled = false;
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(000, 000, 000);
        playerDirection = enDirection.North;
        playerNextDirection = enDirection.North;
        playerSpeed = playerStartSpeed;
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime;
        bridgeSpawner.CleanTheScene();
        coinsCollected = 0;
        timer = 0;
        anim.SetTrigger("isSpawned");
        isDead = false;
    }

    void FootStepEventA()
    {
        audioSource.PlayOneShot(soundFXClips[0], 0.4f);
    }
    
    void FootStepEventB()
    {
        audioSource.PlayOneShot(soundFXClips[0], 0.4f);
    }

    void JumpLandEvent()
    {
        audioSource.PlayOneShot(soundFXClips[4], 0.4f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Coin")
        {
            Destroy(col.gameObject);
            audioSource.PlayOneShot(soundFXClips[7], 0.4f);
            coinsCollected += 1;
        }
    }

    void SaveScore()
    {
        if (coinsCollected > coinsCollectedBest)
        {
            coinsCollectedBest = coinsCollected;
            PlayerPrefs.SetInt("highscoreC", coinsCollectedBest);
            PlayerPrefs.Save();
            bestCoinScoreText.text = "Wow! You've Achieved a New Best Coin Score of: " + coinsCollectedBest.ToString();
        }

        if (distanceRun > distanceRunBest)
        {
            distanceRunBest = distanceRun;
            PlayerPrefs.SetInt("highscoreD", distanceRunBest);
            PlayerPrefs.Save();
            bestDistanceScoreText.text = "Congrats! You've a New Best Running Score of: " + distanceRunBest.ToString() + "M";
        }
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
