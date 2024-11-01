using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndCutsceneManager : MonoBehaviour
{
    [Header("Основные переменные")]
    public int sceneID;
    public float sceneTimer;
    public GameObject scene0black;
    public GameObject scene1text;
    public GameObject scene2xp;
    public GameObject scene3ranked;
    public Animator sceneranked_animator;
    public Slider xpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI textSceneText;

    public Slider rpBar;
    public TextMeshProUGUI pointRankedText;
    public TextMeshProUGUI levelRankedNameText;
    public Image levelRankedIcon;
    public GameObject sliderFill;
    public Image sliderFillImage;

    public GameObject soundSpawner;

    [Header("Спрайты завний")]
    public Sprite rank0;
    public Sprite rank1;
    public Sprite rank2;
    public Sprite rank3;
    public Sprite rank4;

    [Header("Настраиваемые переменные")]
    public float waitTimerStartMax;
    public float waitTimerEndMax;
    public float waitTimerOneXpMax;
    public float waitTimerOneRpMax;

    private float waitTimerStart;
    private float waitTimerEnd;
    private float waitTimerOneXp;
    private float waitTimerOneRp;

    private float xpToAdd;
    private float localXP;
    private int localLevel;

    private float rpToAdd;
    private float localRP;
    private int localRankedLevel;

    void Start()
    {
        sliderFillImage = sliderFill.GetComponent<Image>();

        waitTimerEnd = 3;
        Globals.gameCompletesCount += 1;

        localXP = Globals.xp;
        localLevel = Globals.level;

        localRankedLevel = Globals.rankedLevel;
        localRP = Globals.rankedPoints + 40;

        if(!Globals.gameOver)
        {
            GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/escape_sound", 1.12f, Random.Range(0.97f,1.03f), true);
            if(Globals.rankedGame)
            {
                rpToAdd += 120;
                Globals.rankedPoints += 120 + 40;
                Globals.fullRankedPoints += 120 + 40;
            }

            if(Globals.difficultyID == 0)
            {
                Globals.gameCompletesDifficulty0 += 1;
            } 
            if(Globals.difficultyID == 1)
            {
                Globals.xp += 35;
                xpToAdd = 35;
                Globals.gameCompletesDifficulty1 += 1;
            } 
            if(Globals.difficultyID == 2)
            {
                Globals.xp += 50;
                xpToAdd = 50;
                Globals.gameCompletesDifficulty2 += 1;
            } 
            if(Globals.difficultyID == 3)
            {
                Globals.xp += 75;
                xpToAdd = 75;
                Globals.gameCompletesDifficulty3 += 1;
            } 
            if(Globals.difficultyID == 4)
            {
                Globals.xp += 110;
                xpToAdd = 110;
                Globals.gameCompletesDifficulty4 += 1;
            } 
        }

        if(Globals.gameOver)
        {
            if(Globals.difficultyID == 1) GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Monster/attack1", 1.1f, Random.Range(0.98f,1.02f), true);
            if(Globals.difficultyID == 2) GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Monster/attack2", 1.1f, Random.Range(0.98f,1.02f), true);
            if(Globals.difficultyID == 3) GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Monster/attack3", 1.2f, Random.Range(0.98f,1.02f), true);
            if(Globals.difficultyID == 4) GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Monster/attack4", 1.6f, Random.Range(0.98f,1.02f), true);
            if(Globals.rankedGame)
            {
                rpToAdd -= 40;
            }
        }

        Globals.SaveData();
        Globals.SaveGamesData();
    }

    void Update()
    {

        //черный экран
        if(sceneID == 0)
        {
            scene0black.SetActive(true);
            scene1text.SetActive(false);
            scene2xp.SetActive(false);
            scene3ranked.SetActive(false);
            if(waitTimerEnd > 0) waitTimerEnd -= Time.deltaTime;
            if(waitTimerEnd <= 0)  
            {
                waitTimerEnd = waitTimerEndMax;
                waitTimerStart = waitTimerStartMax;
                sceneID = 1;
                if(Globals.difficultyID != 4) GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/kicking", 0.8f, 1f, true);
                if(Globals.difficultyID == 4) GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/kicking2", 0.8f, 1f, true);
            }
        }
        
        // текст
        if(sceneID == 1)
        {
            scene0black.SetActive(false);
            scene1text.SetActive(true);
            scene2xp.SetActive(false);
            scene3ranked.SetActive(false);
            if(!Globals.gameOver) textSceneText.text = "Вы сбежали";
            if(Globals.gameOver) textSceneText.text = "Вы проиграли.";
            if(Globals.difficultyID == 4 && Globals.gameOver) textSceneText.color = Color.red;
            if(waitTimerEnd > 0) waitTimerEnd -= Time.deltaTime / 1.5f;
            if(waitTimerEnd <= 0)  
            {
                if(!Globals.gameOver)
                {
                    waitTimerEnd = waitTimerEndMax;
                    waitTimerStart = waitTimerStartMax;
                    sceneID = 2;
                    GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/click", 0.6f, 1.08f, true);
                }
                if(Globals.gameOver)
                {
                    if(Globals.rankedGame)
                    {
                        waitTimerEnd = waitTimerEndMax;
                        waitTimerStart = waitTimerStartMax;
                        sceneID = 3;
                        GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/click", 0.6f, 1.08f, true);
                    }
                    if(!Globals.rankedGame)
                    {
                        SceneManager.LoadScene("Menu");
                    }
                }
            }
        }

        // выдача XP
        if(sceneID == 2)
        {
            scene0black.SetActive(false);
            scene1text.SetActive(false);
            scene2xp.SetActive(true);
            scene3ranked.SetActive(false);

            xpBar.value = localXP;
            xpBar.maxValue = Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level);

            if(xpToAdd > 0) levelText.text = "<size=24>Уровень:  " + localLevel + "\n<size=18>" + localXP + "/" + (Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level)) + " xp (+" + xpToAdd + " xp)";
            if(xpToAdd <= 0) levelText.text = "<size=24>Уровень:  " + localLevel + "\n<size=18>" + localXP + "/" + (Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level));

            if(waitTimerStart > 0) waitTimerStart -= Time.deltaTime;

            if(waitTimerOneXp > 0) waitTimerOneXp -= Time.deltaTime;
            if(waitTimerStart <= 0 && waitTimerOneXp <= 0 && xpToAdd > 0)
            {
                waitTimerOneXp = waitTimerOneXpMax;
                xpToAdd -= 1;
                localXP += 1;
                GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/pip", 0.6f, 1.08f, true);
                if(localXP >= (Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level)))
                {
                    localLevel += 1;
                    localXP -= Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level);
                    GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/levelup2", 1f, 1f, true);
                }
            }

            if(waitTimerEnd > 0 && xpToAdd <= 0 && waitTimerStart <= 0) waitTimerEnd -= Time.deltaTime;
            if(waitTimerEnd <= 0 && !Globals.rankedGame)  
            {
                SceneManager.LoadScene("Menu");
            }
            if(waitTimerEnd <= 0 && Globals.rankedGame)  
            {
                GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/click", 0.6f, 1.08f, true);
                sceneID = 3;
                waitTimerEnd = waitTimerEndMax;
                waitTimerStart = waitTimerStartMax;
            }
        }

        // выдача рейтинга
        if(sceneID == 3)
        {
            scene0black.SetActive(false);
            scene1text.SetActive(false);
            scene2xp.SetActive(false);
            scene3ranked.SetActive(true);

            if(localRankedLevel <= 11) rpBar.value = localRP*(localRankedLevel+1);
            if(localRankedLevel > 11) rpBar.value = 999;

            if(Input.GetKeyDown(KeyCode.LeftAlt) && Application.isEditor) rpToAdd = 120;
            

            // Новичок
            if(localRankedLevel >= 0 && localRankedLevel <= 2)
            {
                sliderFillImage.color =  new Color(189f / 255f, 127f / 255f, 73f / 255f);
                rpBar.maxValue = Globals.rpPerLevel * (localRankedLevel+1);
                levelRankedIcon.sprite = rank0;
                if(localRankedLevel == 0) levelRankedNameText.text = "Новичок I";
                if(localRankedLevel == 1) levelRankedNameText.text = "Новичок II";
                if(localRankedLevel == 2) levelRankedNameText.text = "Новичок III";
            } 

            // Профессионал
            if(localRankedLevel >= 3 && localRankedLevel <= 5)
            {
                sliderFillImage.color =new Color(220f / 255f, 220f / 255f, 220f / 255f);
                rpBar.maxValue = Globals.rpPerLevel * (localRankedLevel+1);
                levelRankedIcon.sprite = rank1;
                if(localRankedLevel == 3) levelRankedNameText.text = "Профессионал I";
                if(localRankedLevel == 4) levelRankedNameText.text = "Профессионал II";
                if(localRankedLevel == 5) levelRankedNameText.text = "Профессионал III";
            } 
            
            // Эксперт
            if(localRankedLevel >= 6 && localRankedLevel <= 8)
            {
                sliderFillImage.color =  new Color(121f / 255f, 78f / 255f, 154f / 255f);
                rpBar.maxValue = Globals.rpPerLevel * (localRankedLevel+1);
                levelRankedIcon.sprite = rank2;
                if(localRankedLevel == 6) levelRankedNameText.text = "Эксперт I";
                if(localRankedLevel == 7) levelRankedNameText.text = "Эксперт II";
                if(localRankedLevel == 8) levelRankedNameText.text = "Эксперт III";
            } 

            // Мастер
            if(localRankedLevel >= 9 && localRankedLevel <= 11)
            {
                sliderFillImage.color =   new Color(167f / 255f, 60f / 255f, 51f / 255f);
                rpBar.maxValue = Globals.rpPerLevel * (localRankedLevel+1);
                levelRankedIcon.sprite = rank3;
                if(localRankedLevel == 9) levelRankedNameText.text = "Мастер I";
                if(localRankedLevel == 10) levelRankedNameText.text = "Мастер II";
                if(localRankedLevel == 11) levelRankedNameText.text = "Мастер III";
            } 

            // Архонт
            if(localRankedLevel > 11)
            {
                sliderFillImage.color = new Color(94f / 255f, 127f / 255f, 153f / 255f);
                rpBar.maxValue = 1;
                levelRankedIcon.sprite = rank4;
                levelRankedNameText.text = "Архонт";
            } 

            if(localRankedLevel <= 11)
            {
                if(rpToAdd > 0) pointRankedText.text = localRP+(Globals.rpPerLevel*localRankedLevel) + "/" + Globals.rpPerLevel*(localRankedLevel+1) + " (+" + rpToAdd +")";
                if(rpToAdd == 0) pointRankedText.text = localRP+(Globals.rpPerLevel*localRankedLevel) + "/" + Globals.rpPerLevel*(localRankedLevel+1);
                if(rpToAdd < 0) pointRankedText.text = localRP+(Globals.rpPerLevel*localRankedLevel) + "/" + Globals.rpPerLevel*(localRankedLevel+1) + " (" + rpToAdd +")";
            }
            if(localRankedLevel > 11)
            {
                if(rpToAdd > 0) pointRankedText.text = localRP+(Globals.rpPerLevel*localRankedLevel) + " (+" + rpToAdd +")";
                if(rpToAdd == 0) pointRankedText.text = localRP+(Globals.rpPerLevel*localRankedLevel) + "";
                if(rpToAdd < 0) pointRankedText.text = localRP+(Globals.rpPerLevel*localRankedLevel) + " (" + rpToAdd +")";
            }

            if(waitTimerStart > 0) waitTimerStart -= Time.deltaTime;

            if(waitTimerOneRp > 0) waitTimerOneRp -= Time.deltaTime;
            //добавление ранкед поинтов
            if(waitTimerStart <= 0 && waitTimerOneRp <= 0 && rpToAdd > 0)
            {
                waitTimerOneRp = waitTimerOneRpMax;
                rpToAdd -= 1;
                localRP += 1;
                GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/pip", 0.3f, 1f, true);
                if(localRP >= Globals.rpPerLevel && Globals.rankedLevel <= 11)
                {
                    localRankedLevel += 1;
                    localRP -= Globals.rpPerLevel;
                    sceneranked_animator.SetTrigger("rankUp");
                    GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/levelup", 1f, 1f, true);
                }
            }
            //снятие ранкед поинтов
            if(waitTimerStart <= 0 && waitTimerOneRp <= 0 && rpToAdd < 0)
            {
                waitTimerOneRp = waitTimerOneRpMax;
                rpToAdd += 1;
                localRP -= 1;
                GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/pip", 0.3f, 0.8f, true);
                if(localRP < 0)
                {
                    localRankedLevel -= 1;
                    localRP += Globals.rpPerLevel;
                    GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/leveldown", 1f, 1f, true);
                }
            }

            if(waitTimerEnd > 0 && rpToAdd == 0 && waitTimerStart <= 0) waitTimerEnd -= Time.deltaTime;
            if(waitTimerEnd <= 0)  
            {
                SceneManager.LoadScene("Menu");
            }
            if(waitTimerEnd <= 0)  
            {
                sceneID = 1;
                waitTimerEnd = waitTimerEndMax;
                waitTimerStart = waitTimerStartMax;
            }
        }
    }
}
