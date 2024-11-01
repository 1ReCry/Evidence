using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private GameManager manager;

    [Header("Привязки")]
    public PlayerController pcontroller;
    public GameObject item_voodoo;
    public GameObject item_smudge;
    public GameObject item_mirror;
    public GameObject item_hat;
    public GameObject item_adrenaline;
    public GameObject item_pills;
    public GameObject item_lantern;

    [Header("Текущая рука")]
    public int currentItemID;
    public bool itemUsable;

    //другое
    [HideInInspector]
    public bool updateItem;

    void Start()
    {
        manager = GameManager.instance;
        currentItemID = 0;
        updateItem = true;
    }

    void Update()
    {
        if(currentItemID == 0) manager.itemInHand = false;
        if(currentItemID > 0) manager.itemInHand = true;

        if(updateItem)
        {
            updateItem = false;
            if(currentItemID != 0) GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Items/item_pickup", 0.6f, Random.Range(1.02f, 1.06f), true);
            SetItemGameobj(currentItemID);
        }

        if(Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("use"))) && !Globals.pauseOpened && itemUsable)
        {
            UseItem();
        }
    }

    void SetItemGameobj(int id)
    {
        // деактивируем все предметы
        item_voodoo.SetActive(false);
        item_smudge.SetActive(false);
        item_mirror.SetActive(false);
        item_hat.SetActive(false);
        item_adrenaline.SetActive(false);
        item_pills.SetActive(false);
        item_lantern.SetActive(false);

        // активируем 1 нужный по ID
        if(id == 1) item_voodoo.SetActive(true);
        if(id == 2) item_smudge.SetActive(true);
        if(id == 3) item_mirror.SetActive(true);
        if(id == 4) item_hat.SetActive(true);
        if(id == 5) item_adrenaline.SetActive(true);
        if(id == 6) item_pills.SetActive(true);
        if(id == 7) item_lantern.SetActive(true);
    }

    void UseItem()
    {
        // кукла вуду
        if(currentItemID == 1)
        {
            if(Globals.difficultyID != 0) 
            {
                int randompoint = Random.Range(0, MonsterAI.monster.monsterPoints.Length);                  
                MonsterAI.monster.TeleportToPoint(randompoint);
            }
            GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Items/clotch_use", 1.1f, Random.Range(0.96f, 1.03f), true);
        }

        // благовоние
        if(currentItemID == 2)
        {
            if(Globals.difficultyID != 0) MonsterAI.monster.speedMultiplier = 0.25f;
            if(Globals.difficultyID != 0) MonsterAI.monster.speedMultiplierEffectTimer = 4.5f;
            GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Items/fire_use", 1f, Random.Range(0.96f, 1.03f), true);
        }

        // зеркало
        if(currentItemID == 3)
        {
            pcontroller.undying_effect = 0.5f;
            pcontroller.invisible_effect = 0.75f;

            pcontroller.fov_effect_timer = 0.1f;
            pcontroller.fovMultiplier = 2f;

            if(Globals.difficultyID != 0) 
            {
                Vector3 monsterpos = MonsterAI.monster.gameObject.transform.position;
                MonsterAI.monster.gameObject.transform.position = pcontroller.gameObject.transform.position;
                pcontroller.PosToMonster(monsterpos);
                pcontroller.posToMonsterTimer = 0.2f;
                MonsterAI.monster.ResetAgr();
            }
            GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Items/magic_use", 1f, Random.Range(0.96f, 1.03f), true);
        }

        // шапка невидимка
        if(currentItemID == 4)
        {
            pcontroller.undying_effect = 7f;
            pcontroller.invisible_effect = 7f;

            pcontroller.fov_effect_timer = 7f;
            pcontroller.fovMultiplier = 0.88f;

            GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Items/stels_use", 0.85f, Random.Range(0.96f, 1.03f), true);
        }

        // адреналин
        if(currentItemID == 5)
        {
            pcontroller.speed_effect_timer = 8f;
            pcontroller.speed_multiplier_byeffect = 1.35f;

            pcontroller.fov_effect_timer = 8f;
            pcontroller.fovMultiplier = 1.15f;

            GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Items/ukol_use", 1.5f, Random.Range(0.96f, 1.03f), true);
        }

        // таблетки
        if(currentItemID == 6)
        {
            pcontroller.staminaValue = pcontroller.staminaMax;

            GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/Items/eat_use", 1.1f, Random.Range(0.96f, 1.03f), true);
        }

        // фонарь
        if(currentItemID == 7)
        {
            //без применения
        }

        // сброс предмета
        currentItemID = 0;
        updateItem = true;
    }
}
