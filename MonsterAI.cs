using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private PlayerController[] players;
    public GameObject[] monsterPoints;
    private NavMeshAgent agent;
    public float detectionRange = 20f;
    private Vector3 lastKnownPosition;
    private bool playerInView;
    public float positionReachThreshold = 1f;

    public GameManager manager;

    public static MonsterAI monster;

    [Header("AI переменные")]
    [Header("Скорость (2.6 def)")]
    public float speed;
    private float defaultSpeed = 2.6f;
    private float speedMultiplierByType = 1f; // множитель скорости зависящий от типа монстра
    public float speedMultiplier = 1f; //множитель зависящий от внеш факторов
    public float speedMultiplierEffectTimer = 0f;
    [Header("Макс время проведенное афк на точке")]
    public float afk_on_point_timer_max;
    [Header("Макс время до телепорта если не видит игрока")]
    public float teleportDontSeePlayerTimerMax;
    [Header("Безопасное время при спавне")]
    public float spawnSafeTimeMax;
    [Header("Максимальное время виденья игрока за стеной после пропажи")]
    public float playerVisibleTimerMax;

    [Header("Настройки мигания (рандом от X до Y сек.)")]
    public Vector2 blink_visible_time;
    public Vector2 blink_invisible_time;

    [Header("Другие переменные")]
    public GameObject model;
    public GameObject model_mesh;
    public GameObject tp_sound_obj;
    public Transform RaycastStartPoint;
    public float flight_off_chance;

    // приватные AI
    private bool playerSeeAlready;
    private float afk_on_point_timer;
    private float teleportDontSeePlayerTimer;
    private float step_timer;
    private float step_timer_max;
    [HideInInspector]
    public float spawnSafeTimer;
    [HideInInspector]
    public bool blink_enabled;
    private int curPointID;
    private AudioSource[] sound;
    private CapsuleCollider col;
    private Animator animator;
    private Vector3 nav_safe_timer_pos;
    public float playerVisibleTimer;
    private PlayerController lastNearestPlayer;

    private bool teleport_sounds = true;
    private bool step_sounds = true;
    private bool breathing_sound = true;
    public bool heartbeat_player = true;
    public bool flashlightBlink_player = true;
    public AudioClip rakaiSound;

    private bool typeSelected = false;

    PlayerController nearestVisiblePlayer;

    void Start()
    {
        monster = this;
        agent = GetComponent<NavMeshAgent>();
        sound = GetComponents<AudioSource>();
        col = GetComponent<CapsuleCollider>();
        manager = GameManager.instance;
        animator = model.GetComponent<Animator>();
        monsterPoints = GameObject.FindGameObjectsWithTag("MonsterPoint");
        UpdatePlayerList();
        SetDifficulty(Globals.difficultyID);

        afk_on_point_timer = 1f;

        agent.speed = speed;
        step_timer_max = 0.45f / (speed/defaultSpeed);
        step_timer = step_timer_max;

        if(Globals.difficultyID == 4)
        {
            animator.SetBool("master_difficulty", true);

            int randanim = Random.Range(1,101);
            if(randanim <= 91) animator.SetInteger("master_anim", 0);
            if(randanim > 91) animator.SetInteger("master_anim", 1);
        } 
        else animator.SetBool("master_difficulty", false);
    }

    void FixedUpdate()
    {
        nearestVisiblePlayer = FindNearestVisiblePlayer();
    }

    void Update()
    {
        if(manager.monsterType != 0 && !typeSelected)
        {
            typeSelected = true;
            SetType();
        }
        agent.speed = speed * speedMultiplierByType * speedMultiplier;
        if(Input.GetKeyDown(KeyCode.LeftAlt) && Application.isEditor)
        {
            int randompoint = Random.Range(0, monsterPoints.Length);
            TeleportToPoint(randompoint);
        }

        if (nearestVisiblePlayer != null)
        {
            // Если монстр видит игрока, запоминаем его последнюю известную позицию Начинаем двигаться к игроку
            playerInView = true;
            if(!playerSeeAlready)
            {
                Globals.monsterMeetings += 1;
                playerSeeAlready = true;

                int randomch = Random.Range(0,101);
                if(randomch >= 95)
                {
                    manager.electric_enabled = false;
                }
            }
            teleportDontSeePlayerTimer = teleportDontSeePlayerTimerMax;
            playerVisibleTimer = playerVisibleTimerMax;
            agent.SetDestination(lastKnownPosition);
        }
        else if (playerInView)
        {
            agent.SetDestination(lastKnownPosition);
            // Проверяем, достигли ли последней известной позиции игрока
            if (Vector3.Distance(transform.position, lastKnownPosition) <= positionReachThreshold)
            {
                // Как только достигли последней точки, монстр перестает двигаться
                playerInView = false;
                agent.SetDestination(transform.position);
            }
        }
        if (nearestVisiblePlayer == null)
        {
            if(playerSeeAlready) teleportDontSeePlayerTimer -= Time.deltaTime;
            if(!playerSeeAlready) afk_on_point_timer -= Time.deltaTime;
            if(playerVisibleTimer > 0) playerVisibleTimer -= Time.deltaTime;
        }

        if(playerVisibleTimer > 0)
        {
            if(nearestVisiblePlayer != null) lastNearestPlayer = nearestVisiblePlayer;
            if(lastNearestPlayer != null){
                if(lastNearestPlayer.flight_enabled) lastKnownPosition = lastNearestPlayer.transform.position;
                if(!lastNearestPlayer.flight_enabled)
                {
                    lastKnownPosition = lastNearestPlayer.transform.position;
                    if(nearestVisiblePlayer != null) playerVisibleTimer = 0.02f;
                }
            } 
        }

        //тп на точку рандом после таймеров
        if(teleportDontSeePlayerTimer <= 0 && playerSeeAlready)
        {
            int randompoint = Random.Range(0, monsterPoints.Length);
            TeleportToPoint(randompoint);
        }
        if(afk_on_point_timer <= 0 && !playerSeeAlready)
        {
            int randompoint = Random.Range(0, monsterPoints.Length);
            TeleportToPoint(randompoint);
        }

        // звуки шагов
        step_timer_max = defaultSpeed / speed / speedMultiplierByType / 2;
        if (agent.velocity.magnitude > 0.1f)
        {
            step_timer -= Time.deltaTime;
        }
        else
        {
            step_timer = step_timer_max;
        }

        if(step_timer <= 0 && spawnSafeTimer <= 0)
        {
            string sound_name = "Audio/Sounds/Steps/step_" + Random.Range(1,11);
            if(step_sounds)GlobalFunc.CreateSound(this.gameObject, Resources.Load<GameObject>("Audio/MonsterStep"), sound_name, 0.8f, Random.Range(0.65f,0.7f), true);
            step_timer = step_timer_max;
        }

        if(!sound[0].isPlaying && playerSeeAlready && breathing_sound)
        {
            if(spawnSafeTimer <= 0)
            {
                sound[0].Play();

                //если ракай то не выполняем
                if(manager.monsterType == 8) return;

                if(Globals.difficultyID == 4 || Globals.difficultyID == 3) sound[0].volume = 0f;
                else sound[0].volume = 0.33f;
                sound[0].clip = Resources.Load<AudioClip>("Audio/Sounds/Monster/breathing2");
            }
        }
        if(sound[0].isPlaying && !playerSeeAlready)
        {
            sound[0].Stop();
        }


        //безопасное время при спавне
        if(spawnSafeTimer > 0)
        {
            spawnSafeTimer -= Time.deltaTime;
            transform.position = nav_safe_timer_pos;
            col.enabled = false;
        } 
        if(spawnSafeTimer <= 0)
        {
            col.enabled = true;
        }

        // аниматор и анимации
        if(agent.velocity.magnitude > 0.1f && spawnSafeTimer <= 0)
        {
            animator.SetBool("is_running", true);
        }
        if(agent.velocity.magnitude < 0.1f || spawnSafeTimer > 0)
        {
            animator.SetBool("is_running", false);
        }
        animator.SetFloat("run_speed", agent.speed);
        if(manager.monsterType != 8) sound[0].pitch = agent.speed / defaultSpeed;
        else sound[0].pitch = 1;

        //эффекты скорости
        if(speedMultiplier != 1f) speedMultiplierEffectTimer -= Time.deltaTime;
        if(speedMultiplierEffectTimer <= 0) speedMultiplier = 1f;

        //ревенант
        if(GameManager.instance.monsterType == 6)
        {
            if(nearestVisiblePlayer != null && nearestVisiblePlayer.flight_enabled) speedMultiplierByType = 2f;
            if(nearestVisiblePlayer == null || !nearestVisiblePlayer.flight_enabled) speedMultiplierByType = 0.97f;
        }

        //кошмар
        if(GameManager.instance.monsterType == 9)
        {
            if(nearestVisiblePlayer == null || nearestVisiblePlayer.flight_enabled) speedMultiplierByType = 0.97f;
            if(nearestVisiblePlayer != null && !nearestVisiblePlayer.flight_enabled) speedMultiplierByType = 2f;
        }
    }

    void UpdatePlayerList()
    {
        players = FindObjectsOfType<PlayerController>();
    }

    public void TeleportToPoint(int id)
    {
        int random_chance = Random.Range(0,101);
        Debug.Log("TeleportToPoint: " + id + "/" + monsterPoints.Length + " | sound chance: " + random_chance);

        //рандом звук телепорта
        if(random_chance < 50 && playerSeeAlready && teleport_sounds) GlobalFunc.CreateSound(this.gameObject, tp_sound_obj, "Audio/Sounds/Monster/tp_sound1", 1f, Random.Range(0.96f, 1.03f), false);
        if(random_chance >= 50 && random_chance < 99 && playerSeeAlready && teleport_sounds) GlobalFunc.CreateSound(this.gameObject, tp_sound_obj, "Audio/Sounds/Monster/tp_sound2", 1f, Random.Range(0.96f, 1.03f), false);
        if(random_chance >= 99 && playerSeeAlready && teleport_sounds) GlobalFunc.CreateSound(this.gameObject, tp_sound_obj, "Audio/Sounds/Monster/tp_sound3", 1f, Random.Range(0.96f, 1.03f), false);
        animator.Update(0);
        animator.Play("flying");

        NavMeshHit hit;
        // Находим ближайшую точку на NavMesh в радиусе maxDistance от targetPosition
        if (NavMesh.SamplePosition(monsterPoints[id].transform.position, out hit, 3f, NavMesh.AllAreas))
        {
            nav_safe_timer_pos = new Vector3(hit.position.x, hit.position.y + agent.height / 2, hit.position.z);
            agent.Warp(nav_safe_timer_pos); // Перемещаем агента на найденную позицию
        }
        curPointID = id;

        if(playerSeeAlready)
        {
            Globals.monsterEscaped += 1;
            int chance = Random.Range(0,100);
            Debug.Log("Lighter off chance: " + chance +"% / 100%");
            if(chance <= flight_off_chance)
            {
                PlayerController[] allPControllers = FindObjectsOfType<PlayerController>();
                foreach (PlayerController pcontroller in allPControllers)
                {
                    pcontroller.flight_enabled = false;
                    GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/flashlight_off", 1f, Random.Range(0.97f,1.03f), true);
                    GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/flashlight_broke", 1f, Random.Range(0.96f, 1.04f), false);
                }
            }
        }

        agent.SetDestination(transform.position);
        afk_on_point_timer = afk_on_point_timer_max;
        playerSeeAlready = false;
        lastKnownPosition = transform.position;
        playerVisibleTimer = 0;
        spawnSafeTimer = spawnSafeTimeMax;

        
    }

    // Метод для поиска ближайшего видимого игрока
    PlayerController FindNearestVisiblePlayer()
    {
        PlayerController nearestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (PlayerController player in players)
        {
            // Рассчитываем расстояние до каждого игрока
            float distanceToPlayer = Vector3.Distance(RaycastStartPoint.position, new Vector3(player.transform.position.x, player.transform.position.y+0.66f, player.transform.position.z));

            // Если игрок находится в пределах дальности обнаружения
            if (distanceToPlayer <= detectionRange)
            {
                // Направление луча от монстра к игроку
                Vector3 directionToPlayer = (new Vector3(player.transform.position.x, player.transform.position.y+0.66f, player.transform.position.z) - RaycastStartPoint.position).normalized;

                // Выпускаем луч от монстра к игроку
                Ray ray = new Ray(RaycastStartPoint.position, directionToPlayer);
                RaycastHit hit;

                Debug.DrawRay(RaycastStartPoint.position, directionToPlayer * detectionRange, Color.red);

                // Если луч попал в объект (и это игрок)
                if (Physics.Raycast(ray, out hit, detectionRange))
                {
                    if (hit.collider.GetComponent<PlayerController>() != null)
                    {
                        // Проверяем, является ли этот игрок ближайшим видимым игроком
                        if (distanceToPlayer < closestDistance)
                        {
                            closestDistance = distanceToPlayer;
                            nearestPlayer = player;
                            PlayerController pcontroller = nearestPlayer.GetComponent<PlayerController>();
                            
                            pcontroller.sound[1].pitch = speed/defaultSpeed;
                            if(pcontroller.invisible_effect <= 0) pcontroller.seeByMonsterTimer = playerVisibleTimerMax;
                            if(pcontroller.invisible_effect > 0) 
                            {
                                pcontroller.seeByMonsterTimer = 0f;
                                nearestPlayer = null;
                            }

                            if(pcontroller.start_cutscene_now)
                            {
                                int randompoint = Random.Range(0, monsterPoints.Length);
                                TeleportToPoint(randompoint);
                            }

                            if(!pcontroller.start_cutscene_now)
                            {
                                manager.monsterFinded = true;
                            }

                            //морой
                            if(manager.monsterType == 7)
                            {
                                if(pcontroller.flight_enabled)
                                {
                                    pcontroller.flight_enabled = false;
                                    GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/flashlight_off", 1f, Random.Range(0.97f,1.03f), true);
                                    GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/flashlight_broke", 1f, Random.Range(0.96f, 1.04f), false);
                                }
                            }

                            //кошмар
                            if(manager.monsterType == 9)
                            {
                                int random = Random.Range(0,10000);
                                if(pcontroller.flight_enabled && random > 9920)
                                {
                                    pcontroller.flight_enabled = false;
                                    GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/flashlight_off", 1f, Random.Range(0.97f,1.03f), true);
                                    GlobalFunc.CreateSound(pcontroller.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/flashlight_broke", 1f, Random.Range(0.96f, 1.04f), false);
                                }
                            }
                        }
                    }
                }
            }
        }
        return nearestPlayer; // Возвращаем ближайшего видимого игрока (или null, если никто не виден)
    }

    public void ResetAgr()
    {
        NavMeshHit hit;
        // Находим ближайшую точку на NavMesh в радиусе maxDistance от targetPosition
        if (NavMesh.SamplePosition(transform.position, out hit, 3f, NavMesh.AllAreas))
        {
            nav_safe_timer_pos = new Vector3(hit.position.x, hit.position.y + agent.height / 2, hit.position.z);
            agent.Warp(nav_safe_timer_pos);
        }

        agent.SetDestination(transform.position);
        afk_on_point_timer = afk_on_point_timer_max / 1.6f;
        playerSeeAlready = false;
        lastKnownPosition = transform.position;
        playerVisibleTimer = 0;
        spawnSafeTimer = spawnSafeTimeMax / 6;
    }

    void SetDifficulty(int id)
    {
    //defaultSpeed - Скорость (2.6 def)
    //afk_on_point_timer_max - Макс время проведенное афк на точке
    //teleportDontSeePlayerTimerMax - Макс время до телепорта если не видит игрока
    //spawnSafeTimeMax - Безопасное время при спавне
    //playerVisibleTimerMax - Максимальное время виденья игрока за стеной после пропажи
    //blink_visible_time, blink_invisible_time, это настройки мигания (рандом от X до Y сек.)

    //Безопасно
    if(id==0)
    {
        this.gameObject.SetActive(false);
    }

    //Новичок (нормально)
    if(id==1)
    {
        speed = 2.4f;
        afk_on_point_timer_max = 17;
        teleportDontSeePlayerTimerMax = 4.7f;
        spawnSafeTimeMax = 1.9f;
        playerVisibleTimerMax = 0.18f;

        blink_visible_time.x = 0.6f;
        blink_visible_time.y = 0.9f;
        blink_invisible_time.x = 0.2f;
        blink_invisible_time.y = 0.4f;

        flight_off_chance = 1;
    }

    //Профессионал
    if(id==2)
    {
        speed = 2.57f;
        afk_on_point_timer_max = 13;
        teleportDontSeePlayerTimerMax = 6.2f;
        spawnSafeTimeMax = 1.45f;
        playerVisibleTimerMax = 0.21f;

        blink_visible_time.x = 0.5f;
        blink_visible_time.y = 0.8f;
        blink_invisible_time.x = 0.3f;
        blink_invisible_time.y = 0.4f;
        flight_off_chance = 7;
    }

    //Эксперт
    if(id==3)
    {
        speed = 2.67f;
        afk_on_point_timer_max = 10.5f;
        teleportDontSeePlayerTimerMax = 6.5f;
        spawnSafeTimeMax = 1.2f;
        playerVisibleTimerMax = 0.35f;
        
        blink_visible_time.x = 0.5f;
        blink_visible_time.y = 0.7f;
        blink_invisible_time.x = 0.4f;
        blink_invisible_time.y = 0.5f;
        flight_off_chance = 30;
    }

    //Мастер
    if(id==4)
    {
        speed = 2.69f;
        afk_on_point_timer_max = 9.4f;
        teleportDontSeePlayerTimerMax = 6.8f;
        spawnSafeTimeMax = 1f;
        playerVisibleTimerMax = 0.98f;
        
        blink_visible_time.x = 0.3f;
        blink_visible_time.y = 0.5f;
        blink_invisible_time.x = 0.3f;
        blink_invisible_time.y = 0.6f;

        flight_off_chance = 40;
    }
    }

    void SetType()
    {
        //defaultSpeed - Скорость (2.6 def)
        //afk_on_point_timer_max - Макс время проведенное афк на точке
        //teleportDontSeePlayerTimerMax - Макс время до телепорта если не видит игрока
        //spawnSafeTimeMax - Безопасное время при спавне
        //playerVisibleTimerMax - Максимальное время виденья игрока за стеной после пропажи
        //blink_visible_time, blink_invisible_time, это настройки мигания (рандом от X до Y сек.)
        if(manager.monsterType == 0)
        {
            speedMultiplierByType = 1f;
            teleport_sounds = true;
            step_sounds = true;
            breathing_sound = true;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //агрессивный
        if(manager.monsterType == 1)
        {
            detectionRange *= 0.9f;
            speedMultiplierByType = 1.16f;
            flight_off_chance *= 1.5f;
            teleportDontSeePlayerTimerMax /= 1.35f;
            teleport_sounds = true;
            step_sounds = true;
            breathing_sound = true;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //хищник
        if(manager.monsterType == 2)
        {
            detectionRange *= 1.6f;
            teleportDontSeePlayerTimerMax *= 1.5f;
            playerVisibleTimerMax *= 2.2f;
            teleport_sounds = true;
            step_sounds = true;
            breathing_sound = false;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //тихий
        if(manager.monsterType == 3)
        {
            speedMultiplierByType = 0.86f;
            spawnSafeTimeMax *= 1.25f;
            teleport_sounds = false;
            step_sounds = false;
            breathing_sound = false;
            heartbeat_player = false;
            flashlightBlink_player = false;
        }

        //призрак
        if(manager.monsterType == 4)
        {
            blink_visible_time.x *= 0.5f;
            blink_visible_time.y *= 0.5f;
            blink_invisible_time.x *= 3f;
            blink_invisible_time.y *= 3f;
            teleport_sounds = true;
            step_sounds = false;
            breathing_sound = false;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //демон
        if(manager.monsterType == 5)
        {
            afk_on_point_timer_max *= 0.5f;
            flight_off_chance *= 1.65f;
            spawnSafeTimeMax *= 1.37f;
            teleport_sounds = true;
            step_sounds = true;
            breathing_sound = true;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //ревенант
        if(manager.monsterType == 6)
        {
            teleport_sounds = true;
            step_sounds = true;
            breathing_sound = true;
            playerVisibleTimerMax *= 1.2f;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //морой
        if(manager.monsterType == 7)
        {
            teleport_sounds = false;
            step_sounds = true;
            breathing_sound = true;
            speedMultiplierByType = 1.1f; 
            flight_off_chance = 70f;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //ракай
        if(manager.monsterType == 8)
        {
            teleport_sounds = false;
            step_sounds = false;
            breathing_sound = true;
            speedMultiplierByType = 0.83f; 
            model.SetActive(false);
            sound[0].clip = rakaiSound;
            sound[0].volume = 1.3f;
            heartbeat_player = false;
            flashlightBlink_player = false;
        }

        //кошмар
        if(manager.monsterType == 9)
        {
            teleport_sounds = true;
            step_sounds = true;
            breathing_sound = true;
            heartbeat_player = true;
            flashlightBlink_player = true;
            afk_on_point_timer_max *= 0.94f;
        }

        //мираж
        if(manager.monsterType == 10)
        {
            detectionRange *= 10f;
            afk_on_point_timer_max *= 1.15f;
            teleport_sounds = true;
            step_sounds = false;
            breathing_sound = true;
            heartbeat_player = true;
            flashlightBlink_player = true;
        }

        //мрак
        if(manager.monsterType == 11)
        {
            teleport_sounds = true;
            step_sounds = true;
            breathing_sound = false;
            heartbeat_player = true;
            flashlightBlink_player = true;
            afk_on_point_timer_max *= 0.85f;
            flight_off_chance *= 1.4f;
        }
    }
}
