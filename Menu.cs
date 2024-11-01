using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class Menu : MonoBehaviour
{
    
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject profilePanel;
    public GameObject gamePanel;
    public GameObject ratingGamePanel;
    public GameObject ratingGameRanksPanel;
    public GameObject changelogPanel;
    public GameObject resetAcceptPanel;
    public GameObject resetAcceptPanel2;
    public GameObject resetAcceptPanel3;
    public GameObject getSealPanel;
    public GameObject gameHistoryPanel;
    public GameObject bookPanel;
    public GameObject soundSpawner;
    public GameObject gameStatsPanel;
    public GameObject journalPanel;

    public TextMeshProUGUI statistics;
    public TextMeshProUGUI selected_map;
    public TextMeshProUGUI selected_difficulty;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI fpstext;
    public Slider xpBar;
    public Slider xpBarMenu;
    public TextMeshProUGUI levelTextMenu;
    public Slider rankedBar;
    public TextMeshProUGUI rankName;
    public TextMeshProUGUI rankProgress;
    public Image rankIcon;
    public Image rankedBarFillImage;
    public Image rankIconMenu;
    public TextMeshProUGUI rankNameMenu;
    public Slider fullRankBar;
    public TextMeshProUGUI rankPeracentageText;
    public TextMeshProUGUI rankPointsText;
    public TextMeshProUGUI difficultyDesc;
    public TextMeshProUGUI bookText;
    public TextMeshProUGUI journalTextMonster;
    public TextMeshProUGUI journalTextItem;
    public TMP_InputField delete_confirm_field;
    public Animator fade_animator;
    public TextMeshProUGUI maxRankpointsText;

    //public PostProcessVolume postProcessVolume; 

    private bool statisticLoaded = false;
    private float gametime_minutes = 0;
    private float gametime_hours = 0;
    private float deltaTime;

    [Header("Спрайты званий")]
    public Sprite rankfree;
    public Sprite rank0;
    public Sprite rank1;
    public Sprite rank2;
    public Sprite rank3;
    public Sprite rank4;

    [Header("Печать архонта и количества")]
    public GameObject archon_seal_main;
    public GameObject archon_seal2;
    public GameObject archon_seal3;
    public GameObject archon_seal4;
    public GameObject archon_seal5;
    public TextMeshProUGUI archon_seal_num_text;
    public GameObject get_seal_button;

    [Header("История игр")]
    public GameObject gameHistoryPrefab;
    public Transform contentParent;

    [Header("История конкретной игры")]
    public Image stats_map_icon;
    public Image stats_difficulty_icon;
    public TextMeshProUGUI stats_map_name;
    public TextMeshProUGUI stats_difficulty_name;
    public TextMeshProUGUI stats_statistics;
    public int selectedGame;

    [Header("Спрайты")]
    public Sprite map0;
    public Sprite map1;
    public Sprite map2;
    public Sprite map3;

    private int selectedMonsterType = 0;
    private int selectedItem = 0;

    void Start()
    {
        GlobalFunc.LoadSettings();
        Cursor.lockState = CursorLockMode.None;

        OpenMainMenu();
        Globals.UpdateLevel();

        ResetBookText();
    }

    public void AddMonsterTypeSelected(int num)
    {
        if(selectedMonsterType < Globals.monsterTypeName.Length-1 && num > 0)
        selectedMonsterType += num;

        if(selectedMonsterType > 0 && num < 0)
        selectedMonsterType += num;

        UpdateJournalText();
    }

    public void AddItemSelected(int num)
    {
        if(selectedItem < Globals.itemName.Length-1 && num > 0)
        selectedItem += num;

        if(selectedItem > 0 && num < 0)
        selectedItem += num;

        UpdateJournalText();
    }

    public void UpdateJournalText()
    {
        string text = "";

        text += "Всё о типах монстров.";
        text += "\n\nПро тип монстра \""+Globals.monsterTypeName[selectedMonsterType]+"\":";
        text += "\n\n"+Globals.monsterTypeDesc[selectedMonsterType];

        journalTextMonster.text = text;

        string text2 = "";

        text2 += "Всё о предметах.";
        text2 += "\n\nПро предмет \""+Globals.itemName[selectedItem]+"\":";
        text2 += "\n\n"+Globals.itemDesc[selectedItem];

        journalTextItem.text = text2;
    }

    void Update()
    {
        SetStatsText();
        //DeleteProgress()

        if(Globals.maxRankedPoints < Globals.fullRankedPoints)
        {
            Globals.maxRankedPoints = Globals.fullRankedPoints;
        }
        maxRankpointsText.text = Globals.maxRankedPoints+"";
        
        if(Globals.controlsNotSetted)
        {
            GlobalFunc.ResetControls();
            Globals.controlsNotSetted = false;
        }

        if(Globals.fullRankedPoints < 0 || Globals.rankedLevel < 0)
        {
            Globals.rankedPoints = 0;
            Globals.fullRankedPoints = 0;
            Globals.rankedLevel = 0;
        }

        if(SaveLoad.isDataLoaded)
        {
            if(!statisticLoaded)
            {
                InGameTimeCalculate();
                SetStatsText();
                statisticLoaded = true;
                LoadGameHistory();
            }
        }

        xpBarMenu.value = Globals.xp;
        xpBarMenu.maxValue = Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level);
        levelTextMenu.text = Globals.level+" уровень";

        if(Globals.selectedMapUIName == "") selected_map.text = "<size=18>[Выберите карту]";
        if(Globals.selectedMapUIName != "") selected_map.text = Globals.selectedMapUIName;
        
        if(Globals.difficultyID == -1) selected_difficulty.text = "[Выберите сложность]";
        if(Globals.difficultyID == 0) selected_difficulty.text = "Безопасно";
        if(Globals.difficultyID == 1) selected_difficulty.text = "Новичок";
        if(Globals.difficultyID == 2) selected_difficulty.text = "Профессионал";
        if(Globals.difficultyID == 3) selected_difficulty.text = "Эксперт";
        if(Globals.difficultyID == 4) selected_difficulty.text = "Мастер";

        levelText.text = "Уровень:  " + Globals.level + " |  " + Globals.xp + "/" + (Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level)) + " xp";
        xpBar.value = Globals.xp;
        xpBar.maxValue = Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level);

        DifficultDescText();

        UpdateRanksUI();

        if(Globals.postprocessing == 0) return;//postProcessVolume.enabled = false;
        else if(Globals.postprocessing == 1) return;//postProcessVolume.enabled = true;

        if(Globals.fpscounter == 1)
        {
            fpstext.gameObject.SetActive(true);
            // Вычисляем время между кадрами
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            // Вычисляем количество кадров в секунду (FPS)
            float fps = 1.0f / deltaTime;

            // Обновляем текст в UI элементе
            fpstext.text = Mathf.Ceil(fps).ToString() + " FPS";
        }
        if(Globals.fpscounter == 0) fpstext.gameObject.SetActive(false);

        if(Input.GetKeyDown(KeyCode.LeftArrow) && Application.isEditor)
        {
            Globals.rankedPoints -= 200;
            Globals.fullRankedPoints -= 200;
            Globals.UpdateLevel();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) && Application.isEditor)
        {
            Globals.rankedPoints += 200;
            Globals.fullRankedPoints += 200;
            Globals.UpdateLevel();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) && Application.isEditor)
        {
            Globals.xp += 20;
            Globals.UpdateLevel();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow) && Application.isEditor)
        {
            Globals.xp -= 20;
            Globals.UpdateLevel();
        }
        if(Input.GetKeyDown(KeyCode.RightAlt) && Application.isEditor)
        {
            RestoreProgress();
            Globals.UpdateLevel();
        }

        if(Globals.archonSeals <= 0)
        {
            archon_seal_main.SetActive(false);
        }
        if(Globals.archonSeals == 1)
        {
            archon_seal_main.SetActive(true);
            archon_seal2.SetActive(false);
            archon_seal3.SetActive(false);
            archon_seal4.SetActive(false);
            archon_seal5.SetActive(false);
        }
        if(Globals.archonSeals == 2)
        {
            archon_seal_main.SetActive(true);
            archon_seal2.SetActive(true);
            archon_seal3.SetActive(false);
            archon_seal4.SetActive(false);
            archon_seal5.SetActive(false);
        }
        if(Globals.archonSeals == 3)
        {
            archon_seal_main.SetActive(true);
            archon_seal2.SetActive(true);
            archon_seal3.SetActive(true);
            archon_seal4.SetActive(false);
            archon_seal5.SetActive(false);
        }
        if(Globals.archonSeals == 4)
        {
            archon_seal_main.SetActive(true);
            archon_seal2.SetActive(true);
            archon_seal3.SetActive(true);
            archon_seal4.SetActive(true);
            archon_seal5.SetActive(false);
        }
        if(Globals.archonSeals == 5)
        {
            archon_seal_main.SetActive(true);
            archon_seal2.SetActive(true);
            archon_seal3.SetActive(true);
            archon_seal4.SetActive(true);
            archon_seal5.SetActive(true);
        }
        if(Globals.archonSeals > 5)
        {
            archon_seal_main.SetActive(true);
            archon_seal2.SetActive(true);
            archon_seal3.SetActive(true);
            archon_seal4.SetActive(true);
            archon_seal5.SetActive(true);
        }
        if(Globals.archonSeals <= 1) archon_seal_num_text.text = "";
        if(Globals.archonSeals > 1) archon_seal_num_text.text = "x"+Globals.archonSeals;

        if(Globals.fullRankedPoints >= 2400) get_seal_button.SetActive(true);
        else get_seal_button.SetActive(false);

        if(Input.GetKeyDown(KeyCode.Tab) && Application.isEditor)
        {
            Debug.Log("Data: | RankID: " + Globals.rankedLevel + " | RankPoints: " + Globals.rankedPoints + " | RankFullPoints: " + Globals.fullRankedPoints);
        }
    }

    public void StartGame()
    {
        if(Globals.selectedMapSceneName != "" && Globals.difficultyID >= 0)
        {
            Globals.gamesPlayed += 1;
            Globals.rankedGame = false;
            Globals.gameOver = false;
            Globals.pauseOpened = false;
            SceneManager.LoadScene(Globals.selectedMapSceneName);
        }
    }

    public void SetSelectedMap(int id)
    {
        Globals.curMapID = id;
    }

    public void StartRatingGame()
    {
        // Новичок
        if(Globals.rankedLevel >= 0 && Globals.rankedLevel <= 2)
        {
            Globals.difficultyID = 1;
        } 

        // Профессионал
        if(Globals.rankedLevel >= 3 && Globals.rankedLevel <= 5)
        {
            Globals.difficultyID = 2;
        } 
        
        // Эксперт
        if(Globals.rankedLevel >= 6 && Globals.rankedLevel <= 8)
        {
            Globals.difficultyID = 3;
        } 

        // Мастер
        if(Globals.rankedLevel >= 9)
        {
            Globals.difficultyID = 4;
        } 

        int mapID = Random.Range(1,5);

        Globals.rankedGame = true;
        Globals.gameOver = false;
        Globals.gamesPlayed += 1;
        Globals.pauseOpened = false;

        Globals.curMapID = mapID-1;

        if(mapID==1) SceneManager.LoadScene("Map1");
        if(mapID==2) SceneManager.LoadScene("Map2");
        if(mapID==3) SceneManager.LoadScene("Map3");
        if(mapID==4) SceneManager.LoadScene("Map4");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void OpenMainMenu()
    {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        profilePanel.SetActive(false);
        gamePanel.SetActive(false);
        ratingGamePanel.SetActive(false);
        ratingGameRanksPanel.SetActive(false);
        changelogPanel.SetActive(false);
        resetAcceptPanel.SetActive(false);
        resetAcceptPanel2.SetActive(false);
        bookPanel.SetActive(false);
        resetAcceptPanel3.SetActive(false);
        getSealPanel.SetActive(false);
        gameHistoryPanel.SetActive(false);
        gameStatsPanel.SetActive(false);
        journalPanel.SetActive(false);
    }

    void SetStatsText()
    {
        string text = "";

        text += "<size=15>Проведено в игре: " + gametime_hours + " ч. " + gametime_minutes + " мин.";
        text += "\n\n<size=19>Игр сыграно: " + Globals.gamesPlayed;
        text += "\n<size=19>Игр выиграно: " + Globals.gameCompletesCount;
        text += "\n\n<size=19>По сложностям: ";
        text += "\n<size=16>Безопасно: " + Globals.gameCompletesDifficulty0;
        text += "\n<size=16>Нормально: " + Globals.gameCompletesDifficulty1;
        text += "\n<size=16>Профессионал: " + Globals.gameCompletesDifficulty2;
        text += "\n<size=16>Эксперт: " + Globals.gameCompletesDifficulty3;
        text += "\n<size=16>Мастер: " + Globals.gameCompletesDifficulty4;
        text += "\n\n<size=19>Уровень: " + Globals.level;
        text += "\n<size=19>Опыт: " + Globals.xp + "/" + (Globals.xpPerLevel + (Globals.xpAddPerLevel * Globals.level));
        text += "\n<size=19>Рейтинг: " + Globals.fullRankedPoints + " | " + Globals.maxRankedPoints;
        text += "\n<size=19>Печатей: " + Globals.archonSeals;
        text += "\n\n<size=19>Собрано записок: " + Globals.allGameNotesCollected;
        text += "\n<size=19>Встреч с монстром: " + Globals.monsterMeetings;
        text += "\n<size=15>Убежал от преследования: " + Globals.monsterEscaped + " раз";
        text += "\n<size=15>Собрано особых предметов: " + Globals.allItemsCollected;

        statistics.text = text;
    }

    void InGameTimeCalculate()
    {
        //время в игре расчёт
        float gametime = Globals.inGameTime;
        Debug.Log("gametime: "+gametime);
        while(gametime >= 3600)
        {
            gametime -= 3600;
            gametime_hours += 1;
        }
        gametime_minutes = Mathf.Round(gametime / 60);
    }

    public void UpdateRanksUI()
    {
        if(Globals.rankedLevel == 0) rankName.text = "Новичок I";
        if(Globals.rankedLevel == 1) rankName.text = "Новичок II";
        if(Globals.rankedLevel == 2) rankName.text = "Новичок III";
        if(Globals.rankedLevel == 3) rankName.text = "Профессионал I";
        if(Globals.rankedLevel == 4) rankName.text = "Профессионал II";
        if(Globals.rankedLevel == 5) rankName.text = "Профессионал III";
        if(Globals.rankedLevel == 6) rankName.text = "Эксперт I";
        if(Globals.rankedLevel == 7) rankName.text = "Эксперт II";
        if(Globals.rankedLevel == 8) rankName.text = "Эксперт III";
        if(Globals.rankedLevel == 9) rankName.text = "Мастер I";
        if(Globals.rankedLevel == 10) rankName.text = "Мастер II";
        if(Globals.rankedLevel == 11) rankName.text = "Мастер III";
        if(Globals.rankedLevel > 11) rankName.text = "Архонт";
        rankNameMenu.text = rankName.text;

        if(Globals.rankedLevel <= 11) rankProgress.text = Globals.fullRankedPoints + "/" + (Globals.rankedLevel+1) * Globals.rpPerLevel;
        if(Globals.rankedLevel > 11) rankProgress.text = Globals.fullRankedPoints + "";

        if(Globals.rankedLevel <= 11) rankedBar.value = Globals.rankedPoints;
        if(Globals.rankedLevel > 11) rankedBar.value = 999;

        fullRankBar.value = Globals.fullRankedPoints;
        fullRankBar.maxValue = 2400;
        rankPeracentageText.text = Mathf.RoundToInt(Globals.fullRankedPoints / 2400f * 100f).ToString() + "%";
        rankPointsText.text = Globals.fullRankedPoints + " / 2400";

        // Новичок
        if(Globals.rankedLevel >= 0 && Globals.rankedLevel <= 2)
        {
            rankedBarFillImage.color =  new Color(189f / 255f, 127f / 255f, 73f / 255f);
            rankedBar.maxValue = Globals.rpPerLevel;
            rankIcon.sprite = rank0;
            rankIconMenu.sprite = rank0;
        } 

        // Профессионал
        if(Globals.rankedLevel >= 3 && Globals.rankedLevel <= 5)
        {
            rankedBarFillImage.color =new Color(220f / 255f, 220f / 255f, 220f / 255f);
            rankedBar.maxValue = Globals.rpPerLevel;
            rankIcon.sprite = rank1;
            rankIconMenu.sprite = rank1;
        } 
        
        // Эксперт
        if(Globals.rankedLevel >= 6 && Globals.rankedLevel <= 8)
        {
            rankedBarFillImage.color =  new Color(121f / 255f, 78f / 255f, 154f / 255f);
            rankedBar.maxValue = Globals.rpPerLevel;
            rankIcon.sprite = rank2;
            rankIconMenu.sprite = rank2;
        } 

        // Мастер
        if(Globals.rankedLevel >= 9 && Globals.rankedLevel <= 11)
        {
            rankedBarFillImage.color =   new Color(167f / 255f, 60f / 255f, 51f / 255f);
            rankedBar.maxValue = Globals.rpPerLevel;
            rankIcon.sprite = rank3;
            rankIconMenu.sprite = rank3;
        } 

        // Архонт
        if(Globals.rankedLevel > 11)
        {
            rankedBarFillImage.color = new Color(94f / 255f, 127f / 255f, 153f / 255f);
            rankedBar.maxValue = Globals.rpPerLevel;
            rankIcon.sprite = rank4;
            rankIconMenu.sprite = rank4;
        } 
    }

    public void ButtonSound()
    {
        GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/button", 1f, 1f, true);
    }

    public void ButtonSound2()
    {
        GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/click", 1f, 1f, true);
    }

    public void NodeSound()
    {
        GlobalFunc.CreateSound(soundSpawner, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/note_collect", 1f, 1.1f, true);
    }

    public void DeleteProgress()
    {
        Globals.rankedPoints = 0;
        Globals.gamesPlayed = 0;
        Globals.level = 0;
        Globals.xp = 0;
        Globals.rankedLevel = 0;
        Globals.fullRankedPoints = 0;

        Globals.gameCompletesCount = 0;
        Globals.gameCompletesDifficulty0 = 0;
        Globals.gameCompletesDifficulty1 = 0;
        Globals.gameCompletesDifficulty2 = 0;
        Globals.gameCompletesDifficulty3 = 0;
        Globals.gameCompletesDifficulty4 = 0;
        
        Globals.monsterEscaped = 0;
        Globals.monsterMeetings = 0;
        Globals.allGameNotesCollected = 0;
        Globals.allItemsCollected = 0;
        Globals.archonSeals = 0;

        GameHistoryManager.Instance.gameHistory = new List<GameInfo>();
        Globals.SaveGamesData();

        Globals.SaveData();
        Application.Quit();
    }

    public void RestoreProgress()
    {
        Globals.inGameTime = 10140;
        Globals.rankedPoints = 0;
        Globals.gamesPlayed = 53;
        Globals.level = 12;
        Globals.xp = 105;
        Globals.rankedLevel = 10;
        Globals.fullRankedPoints = 2000;

        Globals.gameCompletesCount = 49;
        Globals.gameCompletesDifficulty0 = 2;
        Globals.gameCompletesDifficulty1 = 7;
        Globals.gameCompletesDifficulty2 = 6;
        Globals.gameCompletesDifficulty3 = 10;
        Globals.gameCompletesDifficulty4 = 2;
        
        Globals.monsterEscaped = 124;
        Globals.monsterMeetings = 157;
        Globals.allGameNotesCollected = 187;
        Globals.allItemsCollected = 29;
        Globals.archonSeals = 0;

        GameHistoryManager.Instance.gameHistory = new List<GameInfo>();

        Globals.SaveGamesData();
        Globals.SaveData();
        Application.Quit();
    }

    public void DifficultDescText()
    {
        if(Globals.difficultyID < 0) difficultyDesc.text = "";

        if(Globals.difficultyID == 0) 
        {
            string text = "";

            text += "- Монстр отсутствует";
            text += "\n- Дневное время суток";

            difficultyDesc.text = text;
        }

        if(Globals.difficultyID == 1) 
        {
            string text = "";

            text += "- Монстр движется с нормальной скоростью";
            text += "\n- Почти не чувствует игрока за стенами";
            text += "\n- Мигает обычно";
            text += "\n+35 xp за прохождение";

            difficultyDesc.text = text;
        }

        if(Globals.difficultyID == 2) 
        {
            string text = "";

            text += "- Монстр движется с повышенной скоростью";
            text += "\n- Чувствует игрока за стенами не долго";
            text += "\n- Мигает дольше";
            text += "\n+50 xp за прохождение";

            difficultyDesc.text = text;
        }

        if(Globals.difficultyID == 3) 
        {
            string text = "";

            text += "- Монстр движется с высокой скоростью";
            text += "\n- Мигает ещё дольше";
            text += "\n- Чувствует игрока за стенами не слишком долго";
            text += "\n- Не издаёт звука дыхания";
            text += "\n+75 xp за прохождение";

            difficultyDesc.text = text;
        }

        if(Globals.difficultyID == 4) 
        {
            string text = "";

            text += "- Монстр движется с очень высокой скоростью";
            text += "\n- Мигает очень долго";
            text += "\n- Чувствует игрока за стенами очень долго";
            text += "\n- Не издаёт звука дыхания";
            text += "\n- Фонарик разбит и светит хуже";
            text += "\n+110 xp за прохождение";

            difficultyDesc.text = text;
        }
    }

    public void SetBookPage(int id)
    {
        if(id==1) bookText.text = "<size=28>Цель игры<size=21>\n\nВаша задача — найти на карте 5 записок и добраться до выхода, чтобы сбежать. Однако это будет не так просто, потому что по карте есть опасный монстр, который не даст вам легко выполнить свою миссию. Кстати, если вы не можете найти выход с карты, то побегайте по ней в безопасном режиме, там легче ориентироватся.";
        if(id==2) bookText.text = "<size=28>Как ведёт себя монстр<size=21>\n\nМонстр появляется на карте неожиданно, телепортируясь в случайные точки. Он остается там неподвижным, поджидая, что вы случайно на него наткнетесь. Если вы не окажетесь рядом с монстром в течение определённого времени (около 15 секунд, в зависимости от уровня сложности), он снова телепортируется на новое место, ожидая своего шанса вас поймать.\n\nЕсли монстр увидит вас, он сразу начнет преследование и будет неотступно следовать за вами, ориентируясь на ваше последнее местоположение. Пока вы находитесь в его поле зрения, он продолжает бежать за вами, и единственный способ сбежать — скрыться из его вида за препятствиями (правило двух углов: забегите за короткий промежуток времени за 2 угла, монстр потеряет вас на первом углу когда вы уже будете за вторым)\n\nЕсли вам удастся скрыться на 7-8 секунд (зависит от сложности), монстр потеряет вас и снова телепортируется в случайную точку, где снова будет стоять в ожидании. Однако не стоит терять бдительность — монстр может появиться в любом месте, и вы можете наткнуться на него в любой момент!";
        if(id==3) bookText.text = "<size=26>Чувство последнего следа<size=21>\n\nКогда монстр вас заметит, он сразу начнёт погоню, направляясь туда, где видел вас в последний раз. Если вы скроетесь за препятствием, например, за стеной, и при этом ваш фонарик будет включён, монстр на некоторое время всё равно продолжит двигаться в вашу сторону, как будто он знает, куда вы пошли.\n\nОднако, если вы выключите фонарик, монстр сразу потеряет след, так что это может спасти вас в критический момент.\n\nНа более высоких уровнях сложности монстр дольше ориентируется в вашем направлении, даже если вы вышли из его поля зрения, поэтому будьте особенно осторожны и быстро принимайте решения.";
        if(id==4) bookText.text = "<size=26>Особые предметы<size=21>\n\nПри старте игры на карте всегда присутствует 1 особый предмет. Обычно он лежит в необычных местах, отличающихся от мест для записок. Разные предметы при использовании дают разные эффекты. Что бы узнать что делает предмет, вам нужно найти и взять его, после чего в журнале появится информация об имени предмета и что он делает.";
        if(id==5) bookText.text = "<size=26>Типы монстра<size=21>\n\nКаждую игру на карте появляется монстр случайного типа. Типы монстра меняют его характеристики. Каждую игру вам придется адаптироватся к типу монстра, так как с каждым типом лучше действовать по разному для эффективности. Что бы узнать какой тип монстра вам попался, вам нужно хотя бы 1 раз встретить его, после чего в журнале появится информация о нём.";
    }

    public void ResetBookText()
    {
        bookText.text = "[Выберите раздел]";
    }

    public void GetArchonSeal()
    {
        Globals.archonSeals += 1;
        Globals.rankedLevel = 0;
        Globals.rankedPoints = 0;
        Globals.fullRankedPoints = 0;
        Globals.SaveData();
        SceneManager.LoadScene("GetArchonSeal");
    }

    public void CheckForDeleteProgress()
    {
        if(delete_confirm_field.text == "i want delete my progress now")
        {
            DeleteProgress();
        }
    }

    public void StartFade()
    {
        fade_animator.SetTrigger("fade");
    }

    void LoadGameHistory()
    {
        foreach (GameInfo game in GameHistoryManager.Instance.gameHistory)
        {
            GameObject gameObject = Instantiate(gameHistoryPrefab, contentParent);

            GameStatsButton gamebuttonscript = gameObject.GetComponent<GameStatsButton>();

            int gameIndex = GameHistoryManager.Instance.gameHistory.IndexOf(game);

            gamebuttonscript.gameID = gameIndex;
            
            if(game.IsRating)
            {
                gameObject.transform.Find("IsRatingGame").gameObject.SetActive(true);
                if(game.IsWin)
                {
                    gameObject.transform.Find("ResultRating").GetComponent<TextMeshProUGUI>().text = "+120 рейтинга";
                    gameObject.transform.Find("ResultRating").GetComponent<TextMeshProUGUI>().color = Color.green;
                } 
                if(!game.IsWin)
                {
                    gameObject.transform.Find("ResultRating").GetComponent<TextMeshProUGUI>().text = "-40 рейтинга";
                    gameObject.transform.Find("ResultRating").GetComponent<TextMeshProUGUI>().color = Color.red;
                } 
            }
            if(!game.IsRating)
            {
                gameObject.transform.Find("IsRatingGame").gameObject.SetActive(false);
                gameObject.transform.Find("ResultRating").gameObject.SetActive(false);
            }

            // Настройте элементы UI внутри gameObject
            if(game.MapID == 0) 
            {
                gameObject.transform.Find("MapName").GetComponent<TextMeshProUGUI>().text = "Старая Усадьба";
                gameObject.transform.Find("MapIcon").GetComponent<Image>().sprite = map0;
            }
            if(game.MapID == 1) 
            {
                gameObject.transform.Find("MapName").GetComponent<TextMeshProUGUI>().text = "Квартира 312";
                gameObject.transform.Find("MapIcon").GetComponent<Image>().sprite = map1;
            }
            if(game.MapID == 2) 
            {
                gameObject.transform.Find("MapName").GetComponent<TextMeshProUGUI>().text = "Квартира 115";
                gameObject.transform.Find("MapIcon").GetComponent<Image>().sprite = map2;
            }
            if(game.MapID == 3) 
            {
                gameObject.transform.Find("MapName").GetComponent<TextMeshProUGUI>().text = "Стройка";
                gameObject.transform.Find("MapIcon").GetComponent<Image>().sprite = map3;
            }

            if(game.DifficultyID == 0) 
            {
                gameObject.transform.Find("DifficultyIcon").GetComponent<Image>().sprite = rankfree;
                gameObject.transform.Find("DifficultyName").GetComponent<TextMeshProUGUI>().text = "Безопасно";
            }
            if(game.DifficultyID == 1) 
            {
                gameObject.transform.Find("DifficultyIcon").GetComponent<Image>().sprite = rank0;
                gameObject.transform.Find("DifficultyName").GetComponent<TextMeshProUGUI>().text = "Новичок";
            }
            if(game.DifficultyID == 2) 
            {
                gameObject.transform.Find("DifficultyIcon").GetComponent<Image>().sprite = rank1;
                gameObject.transform.Find("DifficultyName").GetComponent<TextMeshProUGUI>().text = "Профессионал";
            }
            if(game.DifficultyID == 3) 
            {
                gameObject.transform.Find("DifficultyIcon").GetComponent<Image>().sprite = rank2;
                gameObject.transform.Find("DifficultyName").GetComponent<TextMeshProUGUI>().text = "Эксперт";
            }
            if(game.DifficultyID == 4) 
            {
                gameObject.transform.Find("DifficultyIcon").GetComponent<Image>().sprite = rank3;
                gameObject.transform.Find("DifficultyName").GetComponent<TextMeshProUGUI>().text = "Мастер";
            }

            gameObject.transform.Find("NotesCollected").GetComponent<TextMeshProUGUI>().text = game.NotesCollected+"/"+game.NotesOnMap;

            float matchTimeValue = game.matchTime;
            int matchTimeMinutes = 0;
            while(matchTimeValue >= 60)
            {
                matchTimeValue -= 60;
                matchTimeMinutes += 1;
            }

            string timetext = "";
            if(Mathf.RoundToInt(matchTimeValue) < 10) timetext = matchTimeMinutes+":0"+Mathf.RoundToInt(matchTimeValue);
            if(Mathf.RoundToInt(matchTimeValue) >= 10) timetext = matchTimeMinutes+":"+Mathf.RoundToInt(matchTimeValue);
            
            //gameObject.transform.Find("MatchTime").GetComponent<TextMeshProUGUI>().text = timetext;
            
            string datetext = game.date + " " + game.time;
            gameObject.transform.Find("MatchTime").GetComponent<TextMeshProUGUI>().text = datetext;

            gameObject.transform.Find("NotesCollected").GetComponent<TextMeshProUGUI>().text = game.NotesCollected+"/"+game.NotesOnMap;

            if(game.SpecialItem == 1) gameObject.transform.Find("SpecialItem").GetComponent<TextMeshProUGUI>().text = "Кукла Вуду";
            if(game.SpecialItem == 2) gameObject.transform.Find("SpecialItem").GetComponent<TextMeshProUGUI>().text = "Благовоние";
            if(game.SpecialItem == 3) gameObject.transform.Find("SpecialItem").GetComponent<TextMeshProUGUI>().text = "Зеркало";
            if(game.SpecialItem == 4) gameObject.transform.Find("SpecialItem").GetComponent<TextMeshProUGUI>().text = "Шапка Невидимка";
            if(game.SpecialItem == 5) gameObject.transform.Find("SpecialItem").GetComponent<TextMeshProUGUI>().text = "Адреналин";
            if(game.SpecialItem == 6) gameObject.transform.Find("SpecialItem").GetComponent<TextMeshProUGUI>().text = "Таблетки";
            if(game.SpecialItem == 7) gameObject.transform.Find("SpecialItem").GetComponent<TextMeshProUGUI>().text = "Фонарь";

            gameObject.transform.Find("MonsterType").GetComponent<TextMeshProUGUI>().text = Globals.monsterTypeName[game.MonsterType];

            if(game.IsWin)
            {
                gameObject.transform.Find("Result").GetComponent<TextMeshProUGUI>().text = "Победа";
                gameObject.transform.Find("Result").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            if(!game.IsWin)
            {
                gameObject.transform.Find("Result").GetComponent<TextMeshProUGUI>().text = "Поражение";
                gameObject.transform.Find("Result").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    public void UpdateGameStats()
    {
        GameInfo selGameInfo = GameHistoryManager.Instance.gameHistory[selectedGame];

        if(selGameInfo.MapID == 0) 
        {
            stats_map_icon.sprite = map0;
            stats_map_name.text = "Старая Усадьба";
        }
        if(selGameInfo.MapID == 1) 
        {
            stats_map_icon.sprite = map1;
            stats_map_name.text = "Квартира 312";
        }
        if(selGameInfo.MapID == 2) 
        {
            stats_map_icon.sprite = map2;
            stats_map_name.text = "Квартира 115";
        }
        if(selGameInfo.MapID == 3) 
        {
            stats_map_icon.sprite = map3;
            stats_map_name.text = "Стройка";
        }

        if(selGameInfo.DifficultyID == 0) 
        {
            stats_difficulty_icon.sprite = rankfree;
            stats_difficulty_name.text = "Безопасно";
        }
        if(selGameInfo.DifficultyID == 1) 
        {
            stats_difficulty_icon.sprite = rank0;
            stats_difficulty_name.text = "Новичок";
        }
        if(selGameInfo.DifficultyID == 2) 
        {
            stats_difficulty_icon.sprite = rank1;
            stats_difficulty_name.text = "Профессионал";
        }
        if(selGameInfo.DifficultyID == 3) 
        {
            stats_difficulty_icon.sprite = rank2;
            stats_difficulty_name.text = "Эксперт";
        }
        if(selGameInfo.DifficultyID == 4) 
        {
            stats_difficulty_icon.sprite = rank3;
            stats_difficulty_name.text = "Мастер";
        }

        string text = "";
        text += "Игра #" + (selectedGame+1);
        if(selGameInfo.IsRating) text += "\nРейтинг: " + (selGameInfo.fullRating+40);
        if(selGameInfo.IsRating && selGameInfo.IsWin) text += "  Победа (+120 рейтинга)";
        if(selGameInfo.IsRating && !selGameInfo.IsWin) text += "  Поражение (-40 рейтинга)";

        float gameTimeValue = selGameInfo.matchTime;
        float gameTimeMinutes = 0;
        while(gameTimeValue >= 60)
        {
            gameTimeValue -= 60;
            gameTimeMinutes += 1;
        }

        text += "\nСобрано записок: " + selGameInfo.NotesCollected + "/" + selGameInfo.NotesOnMap;

        if(selGameInfo.itemFinded) text += "\nПредмет был найден";
        if(!selGameInfo.itemFinded) text += "\nПредмет не был найден";

        if(selGameInfo.monsterFinded) text += "\nМонстр был найден";
        if(!selGameInfo.monsterFinded) text += "\nМонстр не был найден";

        text += "\n\nВремя матча: " + gameTimeMinutes + " мин. " + Mathf.RoundToInt(gameTimeValue) + " сек.";

        stats_statistics.text = text;
    }

    public void OpenMatchStats()
    {
        OpenMainMenu();
        menuPanel.SetActive(false);

        gameStatsPanel.SetActive(true);
    }
}
