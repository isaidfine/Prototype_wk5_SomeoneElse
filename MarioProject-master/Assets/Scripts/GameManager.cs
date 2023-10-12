using System;
using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject mask;
    public GameObject mask1;
    public GameObject mask2;
    public MarioBlock blockScript; // MarioBlock的引用
    private Vector3 playerStartPosition = new Vector3(-2.64f, -3.58f, -1);
    private PlayerCharacter playerCharacter;
    public int level = 2;
    public int year = 2;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI yearText;
    public TextMeshProUGUI statsText;
    
    public AudioClip levelSound;
    private AudioSource audioSource;

    private void Awake()
    {
        player.transform.position = playerStartPosition;
        playerCharacter = player.GetComponent<PlayerCharacter>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateUI();
        if (level < 27)
        {
            playerCharacter.curSpeed = 0.84f + level * 0.08f;
            playerCharacter.jumpSpeed = 5.68f + level * 0.16f;
        }

        if (level >= 27)
        {
            if (year < 35)
            {
                playerCharacter.curSpeed = 0.84f + level * 0.08f - (year - 27) * 0.08f;
                playerCharacter.jumpSpeed = 5.68f + level * 0.16f - (year - 27) * 0.16f;
            }

            if (year >= 35)
            {
                playerCharacter.curSpeed = 0.84f + level * 0.08f - 8 * 0.08f -(year - 35) * 0.16f;
                playerCharacter.jumpSpeed = 5.68f + level * 0.16f - 8 * 0.16f - (year - 35) * 0.32f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && gameObject.tag == "EndCollider")
        {
            Debug.Log("End Collider");
            year++;
            StartCoroutine(Transition());
        }
    }

    private IEnumerator Transition()
    {
        if (level < 27)
        {
            mask.SetActive(true);
            audioSource.PlayOneShot(levelSound);
            yield return new WaitForSeconds(0.5f);
            mask.SetActive(false); 
            ResetLevel();
        }

        if (level >= 27)
        {
            if (year < 35)
            {
                mask1.SetActive(true);
                audioSource.PlayOneShot(levelSound);
                yield return new WaitForSeconds(0.5f);
                mask1.SetActive(false); 
                ResetLevel();
            }

            if (year >= 35)
            {
                mask2.SetActive(true);
                audioSource.PlayOneShot(levelSound);
                yield return new WaitForSeconds(0.5f);
                mask2.SetActive(false); 
                ResetLevel();
            }
        }
    }
    

    private void ResetLevel()
    {
        player.transform.position = playerStartPosition;
        blockScript.ResetBlock();
    }

    private void UpdateUI()
    {
        levelText.text = "LV" + level;
        yearText.text = "Year\n" + year;
        statsText.text = "Move Spd: " + playerCharacter.curSpeed + "\nJump Spd: " + playerCharacter.jumpSpeed;

    }
}