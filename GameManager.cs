using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private GameObject[] noteSpawns;
    private int[] alreadySpawned;
    private int notesSpawned;
    public GameObject notePrefab;
    public int notesCollected;
    public int notesMax;
    public GameObject crosshairHand;
    public TextMeshProUGUI notesText;
    public TextMeshProUGUI debugText;
    public TextMeshProUGUI fpsText;
    public GameObject ratingGameAlert;
    public GameObject ratingGameAlertMenu;
    public GameObject pauseMenuObj;
    public GameObject settingsMenuObj;
    public bool hide_ui;
    public static GameManager instance;
    private float deltaTime = 0.0f;
    public TextMeshProUGUI itemNotif;
    public GameObject invisibleOverlay;
    public TextMeshProUGUI journalText;
    public TextMeshProUGUI interactText;
    public GameObject journalPanel;
    public TextMeshProUGUI journalNotify;
    public bool journalSound = false;

    [Header("Настройки дневной сцены")]
    public Material skyboxDay;
    public Light lightning;
    //public PostProcessVolume postProcessVolume; 

    [Header("Настройка спавна особых предметов")]
    public GameObject[] items;
    private GameObject[] itemSpawns;
    public int spawnedItemID;

    [Header("Настройка монстра")]
    public int monsterType = -1;
    public bool monsterFinded;

    [Header("Предмет в руке")]
    public bool itemFinded = false;
    public bool itemInHand;
    public int itemInHandID;
    public bool itemUsable;

    [Header("Щиток")]
    public GameObject[] electropanel_spawns;
    public GameObject electropanel;
    public bool electric_enabled = true;

    public float gameTime;
    //0 - обыч
    //1 - агр
    //2 - хищ
    //3 - тихий
    //4 - призрак
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        monsterType = UnityEngine.Random.Range(0,12);

        GlobalFunc.LoadSettings();
        noteSpawns = GameObject.FindGameObjectsWithTag("NodePoint");
        itemSpawns = GameObject.FindGameObjectsWithTag("ItemPoint");
        electropanel_spawns = GameObject.FindGameObjectsWithTag("ElectropanelPoint");
        alreadySpawned = new int[noteSpawns.Length];

        if(!Globals.rankedGame) {
            ratingGameAlert.SetActive(false);
            ratingGameAlertMenu.SetActive(false);
        }
        else if(Globals.rankedGame) {
            ratingGameAlert.SetActive(true);
            ratingGameAlertMenu.SetActive(true);
        }
        Globals.pauseOpened = false;
        pauseMenuObj.SetActive(false);
        settingsMenuObj.SetActive(false);

        //настройка освещения
        if(Globals.difficultyID == 0)
        {
            RenderSettings.skybox = skyboxDay;
            Debug.Log("Безопасно, скайбокс: " + skyboxDay + " | рендер скайбокс: " + RenderSettings.skybox);
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.grey;
            RenderSettings.fogDensity = 0.015f;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(109f / 255f, 109f / 255f, 109f / 255f);
            lightning.enabled = true;
        }
        if(Globals.difficultyID > 0)
        {
            RenderSettings.skybox = null;
            RenderSettings.fog = false;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;
            lightning.enabled = false;
        }

        //спавн особого предмета
        int randompointitem = UnityEngine.Random.Range(0, itemSpawns.Length);
        spawnedItemID = UnityEngine.Random.Range(0, items.Length);
        GameObject item = Instantiate(items[spawnedItemID], itemSpawns[randompointitem].transform.position, Quaternion.identity);
        if(spawnedItemID==3) item.transform.rotation = Quaternion.Euler(90,0,0);
        Debug.Log("Spawned Item ID: " + spawnedItemID + " | on point: " + randompointitem + "/" + itemSpawns.Length);
        spawnedItemID += 1;

        Debug.Log("Difficulty ID: " + Globals.difficultyID);

        if(Globals.difficultyID < 3) electric_enabled = true;
        if(Globals.difficultyID >= 3) electric_enabled = false;

        //спавн электрощитка
        if(electropanel_spawns.Length != 0)
        {
            int randompointpanel = UnityEngine.Random.Range(0, electropanel_spawns.Length);
            GameObject panel = Instantiate(electropanel, electropanel_spawns[randompointpanel].transform.position, Quaternion.identity);
            panel.transform.rotation = electropanel_spawns[randompointpanel].transform.rotation;
        }
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        journalNotify.text = "[" + PlayerPrefs.GetString("journal") + "] - Журнал";

        if(PlayerPrefs.GetString("interact") == "Mouse0") interactText.text = "[ЛКМ] - Взаимодействие";
        else if(PlayerPrefs.GetString("interact") == "Mouse1") interactText.text = "[ПКМ] - Взаимодействие";
        else interactText.text = "[" + PlayerPrefs.GetString("interact") + "] - Взаимодействие";

        if(PlayerPrefs.GetString("use") == "Mouse0") itemNotif.text = "[ЛКМ] - Использовать";
        else if(PlayerPrefs.GetString("use") == "Mouse1") itemNotif.text = "[ПКМ] - Использовать";
        else itemNotif.text = "[" + PlayerPrefs.GetString("use") + "] - Использовать";

        if(debugText.gameObject.activeInHierarchy) debugText.text = MonsterAI.monster.playerVisibleTimer.ToString("F2");
        if(notesCollected < notesMax) notesText.text = "Записок собрано: " + notesCollected + "/" + notesMax;
        if(notesCollected >= notesMax) notesText.text = "Все записки собраны";

        if(notesSpawned < notesMax)
        {
            int randompoint = UnityEngine.Random.Range(0, noteSpawns.Length);
            if(alreadySpawned[randompoint] == 0)
            {
                GameObject note = Instantiate(notePrefab, noteSpawns[randompoint].transform.position, Quaternion.identity);
                alreadySpawned[randompoint] = 1;
                notesSpawned += 1;
            }
        }

        // меню паузы
        if(Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("pause"))))
        {
            Globals.pauseOpened = !Globals.pauseOpened;
            pauseMenuObj.SetActive(true);
            settingsMenuObj.SetActive(false);
            journalPanel.SetActive(false);
        }

        // меню паузы
        if(Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("journal"))))
        {
            UpdateJournalText();
            Globals.pauseOpened = !Globals.pauseOpened;
            pauseMenuObj.SetActive(false);
            settingsMenuObj.SetActive(false);
            journalPanel.SetActive(true);
        }

        if(Globals.pauseOpened)
        {
            if(!settingsMenuObj.activeInHierarchy && !journalPanel.activeInHierarchy) pauseMenuObj.SetActive(true);
            if(settingsMenuObj.activeInHierarchy || journalPanel.activeInHierarchy) pauseMenuObj.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }

        if(!Globals.pauseOpened)
        {
            pauseMenuObj.SetActive(false);
            settingsMenuObj.SetActive(false);
            journalPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        // скрыть интерфейс
        if(hide_ui)
        {
            crosshairHand.SetActive(false);
            notesText.gameObject.SetActive(false);
            journalNotify.gameObject.SetActive(false);
            itemNotif.gameObject.SetActive(false);
            if(Globals.rankedGame) ratingGameAlert.SetActive(false);
        }
        else
        {
            notesText.gameObject.SetActive(true);
            journalNotify.gameObject.SetActive(true);
            if(Globals.rankedGame) ratingGameAlert.SetActive(true);
        }

        if(Globals.postprocessing == 0) return;//postProcessVolume.enabled = false;
        else if(Globals.postprocessing == 1) return;//postProcessVolume.enabled = true;

        if(Globals.fpscounter == 1)
        {
            fpsText.gameObject.SetActive(true);
            // Вычисляем время между кадрами
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            // Вычисляем количество кадров в секунду (FPS)
            float fps = 1.0f / deltaTime;

            // Обновляем текст в UI элементе
            fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
        }
        if(Globals.fpscounter == 0) fpsText.gameObject.SetActive(false);

        if(!itemInHand || !itemUsable || hide_ui) itemNotif.gameObject.SetActive(false);
        if(itemInHand && itemUsable && !hide_ui) itemNotif.gameObject.SetActive(true);
    }

    public void MenuClose()
    {
        Globals.pauseOpened = false;
    }

    public void ExitGame()
    {
        AddGameToHistory(false);
        SceneManager.LoadScene("Menu");
        Globals.pauseOpened = false;
    }

    public void UpdateJournalText()
    {
        journalSound = true;
        string text = "";
        
        text += "Журнал. Сведения и информация";
        if(!itemFinded) text += "\n\nОсобый предмет: ???";

        if(itemFinded) text += "\n\nОсобый предмет: "+Globals.itemName[spawnedItemID-1]+"\n\n"+Globals.itemDesc[spawnedItemID-1];

        if(!monsterFinded)
        {
            if(Globals.difficultyID == 0)
            {
                text += "\n\n\nТип монстра: Отсутствует";
            }
            if(Globals.difficultyID != 0)
            {
                text += "\n\n\nТип монстра: ???";
                text += "\nОсобенности монстра: ???";
            }
        }
        if(monsterFinded)
        {
            text += "\n\n\nТип монстра: ";
            text += Globals.monsterTypeName[monsterType];
            text += "\nОсобенности монстра: \n";
            text += Globals.monsterTypeDesc[monsterType];
        }

        journalText.text = text;
    }

    public void AddGameToHistory(bool _isWin)
    {
        if (GameHistoryManager.Instance != null)
        {
            DateTime datenow = DateTime.Now;

            GameInfo newGame = new GameInfo
            {
                MapID = Globals.curMapID,
                DifficultyID = Globals.difficultyID,
                NotesCollected = notesCollected,
                NotesOnMap = notesMax,
                SpecialItem = spawnedItemID,
                MonsterType = monsterType,
                IsWin = _isWin,
                IsRating = Globals.rankedGame,
                rankID = Globals.rankedLevel,
                fullRating = Globals.fullRankedPoints,
                matchTime = gameTime,
                date = datenow.ToString("yyyy.MM.dd"),
                time = datenow.ToString("HH:mm"),
                itemFinded = itemFinded,
                monsterFinded = monsterFinded
            };

            GameHistoryManager.Instance.AddGame(newGame);
        }

        else Debug.Log("Нет скрипта GameHistoryManager.Instance");
    }
}
