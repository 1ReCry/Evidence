using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    private GameManager manager;
    public PlayerHand hand;
    private FlashlightWiggle flashlight_wiggle;
    public Light light_invis;

    [Header("Движение")]
    private float moveSpeed;
    public bool isMoving;
    public float gravity = -9.81f;

    public float walkSpeed;
    public float defaultFov;

    public float runSpeed;
    public float runFov;

    private float stepTimer;
    private float stepDelayWalk = 0.7f;
    private float stepDelayRun = 0.4f;

    private bool isCrouching;

    [Header("Стамина")]
    public bool sprinting;

    public float staminaValue;
    public float staminaMax;

    public float staminaDecreaseMultip;
    public float staminaIncreaseMultip;

    private float staminaRegenDelayValue;
    public float staminaRegenDelay;

    public float seeByMonsterTimer;

    [Header("Камера и мышь")]
    public Transform cam;
    private Camera cameraComp;
    public Vector2 zoom_fovs;
    private float zoom_fov_value;
    public float mouseSensitivity;
    // Ограничение на угол поворота камеры по вертикали
    private float verticalLookRotation = 0f;

    [Header("Предметы")]
    public float handDistance = 5f;
    private GameObject targetedObject;

    [Header("Звуки")]
    public GameObject stepSoundPrefab;
    public GameObject SoundPrefab;
    public AudioSource[] sound;

    // Фонарик
    [Header("Фонарик")]
    public Light flashlight; // Объявляем публичное поле для SpotLight
    public float flashlightAngle = 30f; // Угол поворота фонарика
    public float flight_blink_intensity;
    public float flight_normal_intensity;
    public bool flight_enabled = true;
    private bool flight_blink;
    private float flight_blink_timer;
    public Texture2D flashlight_normal;
    public Texture2D flashlight_master;

    // другое
    private Vector3 velocity;
    bool hasBlockingCrouchObject = false;
    float targetHeight;
    public bool cutscene;
    public bool start_cutscene_now;
    public Animator camera_animator;
    public WeaponSway wsway;
    private bool zoom;

    // эффекты
    public float undying_effect;
    public float invisible_effect;

    public float speed_effect_timer;
    public float speed_multiplier_byeffect = 1f;

    public float fovMultiplier;
    public float fov_effect_timer;

    public float posToMonsterTimer;
    private Vector3 monsterPos;

    private float flight_range;

    void Start()
    {
        light_invis.intensity = 0f;

        controller = GetComponent<CharacterController>();

        Cursor.lockState =  CursorLockMode.Locked;

        staminaValue = staminaMax;

        manager = FindObjectOfType<GameManager>();

        cameraComp = cam.GetComponent<Camera>();

        sound = GetComponents<AudioSource>();

        flashlight_wiggle = GetComponent<FlashlightWiggle>();

        Globals.gameOver = false;

        zoom_fov_value = zoom_fovs.y;

        // буфер 40 за Alt+F4 или за выход
        if(Globals.rankedGame)
        {
            Globals.rankedPoints -= 40;
            Globals.fullRankedPoints -= 40;

            Globals.SaveData();
        } 

        if(Globals.difficultyID != 4)
        {
            flashlight.cookie = flashlight_normal;
        }
        if(Globals.difficultyID == 4)
        {
            flashlight.cookie = flashlight_master;
            flight_normal_intensity /= 1.5f;
            flight_blink_intensity /= 3.2f;
            flashlight.spotAngle /= 1.1f;
        }

        flight_range = flashlight.range;
    }


    void Update()
    {
        // Перемещение
        Vector3 move = Vector3.zero;
        if(!Globals.pauseOpened && !cutscene)
        {
            // Получаем сохранённые клавиши для управления
            KeyCode forwardKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("move_forward"));
            KeyCode backwardKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("move_backward"));
            KeyCode leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("move_left"));
            KeyCode rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("move_right"));

            if (Input.GetKey(forwardKey)) move += transform.forward;
            if (Input.GetKey(backwardKey)) move -= transform.forward;
            if (Input.GetKey(leftKey)) move -= transform.right;
            if (Input.GetKey(rightKey)) move += transform.right;
        }
        controller.Move(move.normalized * (moveSpeed * speed_multiplier_byeffect) * Time.deltaTime);
        if(!Globals.pauseOpened && !cutscene)
        {
            // Получение ввода мыши (независимо от FPS)
            float mouseX = Input.GetAxisRaw("Mouse X") * Globals.sensitivity * Time.fixedDeltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Globals.sensitivity * Time.fixedDeltaTime;

            // Поворот персонажа по горизонтали (вокруг оси Y) камеры по вертикали (вокруг оси X)
            transform.Rotate(Vector3.up * mouseX);
            verticalLookRotation -= mouseY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f); // Ограничиваем вращение камеры
            cam.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
        }


        HandleCrouch();

        KeyCode runKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("run"));
        //Стамина и бег
        if (Input.GetKeyDown(runKey) && isMoving)
        {
            if(staminaValue > 0) sprinting = true;
        }
        if (Input.GetKeyUp(runKey) && isMoving)
        {
            if(!sound[0].isPlaying)
            {
                sound[0].volume = Mathf.Clamp(1 - staminaValue / 50f, 0, 0.8f);
                sound[0].Play();
                sprinting = false;
            }
        }
        if (!Input.GetKey(runKey) && isMoving)
        {
            sprinting = false;
        }

        if(sprinting)
        {
            moveSpeed = runSpeed;
            //снятие стамины
            staminaValue -= Time.deltaTime * staminaDecreaseMultip;
            staminaRegenDelayValue = staminaRegenDelay;

            if(staminaValue <= 0)
            {   
                sound[0].Stop();
                sound[0].volume = Mathf.Clamp(1 - staminaValue / 50f, 0, 0.8f);
                sound[0].Play();
                sprinting = false;
            } 

            if(!zoom) cameraComp.fieldOfView = Mathf.Lerp(cameraComp.fieldOfView, runFov * fovMultiplier, 3.5f * Time.deltaTime);
        }
        if(!sprinting)
        {
            moveSpeed = walkSpeed;

            if(!zoom) cameraComp.fieldOfView = Mathf.Lerp(cameraComp.fieldOfView, defaultFov * fovMultiplier, 3.5f * Time.deltaTime);
            //восстановление стамины
            if(staminaRegenDelayValue <= 0 && staminaValue < staminaMax) staminaValue += Time.deltaTime * staminaIncreaseMultip * speed_multiplier_byeffect;
        }
        //таймер до восстановления стамины
        if(staminaRegenDelayValue > 0 && !sprinting) staminaRegenDelayValue -= Time.deltaTime;

        if(!isMoving)
        {
            sprinting = false;
        }


        //подбор записок
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("PlayerCollider");

        // Выполняем Raycast
        if (Physics.Raycast(ray, out hit, handDistance, layerMask) && !Globals.pauseOpened)
        {
            // Проверяем, есть ли у объекта нужный тег
            if (hit.collider.CompareTag("Note"))
            {
                targetedObject = hit.collider.gameObject; // Сохраняем объект
                // Если нажата клавиша E и объект не null, удаляем его
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact"))) && targetedObject != null)
                {
                    Destroy(targetedObject);
                    manager.notesCollected += 1;
                    Globals.allGameNotesCollected += 1;
                    targetedObject = null;
                    GlobalFunc.CreateSound(this.gameObject, SoundPrefab, "Audio/Sounds/note_collect", 1.4f, Random.Range(0.97f,1.02f), true);
                }
            }
            // Проверяем, есть ли у объекта нужный тег
            else if (hit.collider.CompareTag("Item"))
            {
                targetedObject = hit.collider.gameObject; // Сохраняем объект
                ItemObject itemobj = targetedObject.GetComponent<ItemObject>();

                // Если нажата клавиша E и объект не null, удаляем его
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact"))) && targetedObject != null)
                {
                    if(itemobj != null)
                    {
                        Globals.allItemsCollected += 1;
                        manager.itemFinded = true;
                        hand.currentItemID = itemobj.ItemID;
                        hand.itemUsable = itemobj.itemUsable;
                        manager.itemUsable = itemobj.itemUsable;
                        manager.itemInHandID = itemobj.ItemID;
                        hand.updateItem = true;
                        Destroy(targetedObject);
                        targetedObject = null;
                        //GlobalFunc.CreateSound(this.gameObject, SoundPrefab, "Audio/Sounds/note_collect", 1.4f, Random.Range(0.97f,1.02f), true);
                    }
                }
            }
            else if (hit.collider.CompareTag("Exit"))
            {
                targetedObject = hit.collider.gameObject; // Сохраняем объект
                // Если нажата клавиша E и объект не null, удаляем его
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact"))) && targetedObject != null)
                {
                    if(manager.notesCollected >= manager.notesMax)
                    {
                        Globals.gameOver = false;
                        manager.AddGameToHistory(true);
                        SceneManager.LoadScene("EndCutscene");
                    }
                    if(manager.notesCollected < manager.notesMax)
                    {
                        GlobalFunc.CreateSound(this.gameObject, SoundPrefab, "Audio/Sounds/locked", 1.12f, Random.Range(0.97f,1.02f), true);
                    }
                }
            }
            else if (hit.collider.CompareTag("LightSwitcher"))
            {
                targetedObject = hit.collider.gameObject; // Сохраняем объект

                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact"))) && targetedObject != null)
                {
                    LampSwitch lampswitch_script = targetedObject.GetComponent<LampSwitch>();

                    lampswitch_script.Interact();
                }
            }
            else if (hit.collider.CompareTag("Electropanel"))
            {
                targetedObject = hit.collider.gameObject; // Сохраняем объект

                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact"))) && targetedObject != null)
                {
                    Electropanel electr = targetedObject.GetComponent<Electropanel>();

                    electr.Interact();
                }
            }
            else
            {
                targetedObject = null; // Если объект не с нужным тегом, сбрасываем его
            }
        }
        else
        {
            targetedObject = null; // Если луч не попал в объект, сбрасываем
        }

        //ui crosshair enable
        if(targetedObject != null)
        {
            manager.crosshairHand.SetActive(true);
        }
        else
        {
            manager.crosshairHand.SetActive(false);
        }

        if(move!=Vector3.zero)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // звуки шагов
        if(stepTimer > 0 && isMoving) 
        {
            stepTimer -= Time.deltaTime;
            if(sprinting && stepTimer > stepDelayRun) stepTimer = stepDelayRun;
        }
        if(stepTimer <= 0 && isMoving) 
        {
            if(!sprinting) stepTimer = 0.7f / speed_multiplier_byeffect;
            if(sprinting) stepTimer = 0.3f / speed_multiplier_byeffect;

            string sound_name = "Audio/Sounds/Steps/step_" + Random.Range(1,11);
            GlobalFunc.CreateSound(this.gameObject, stepSoundPrefab, sound_name, 0.75f, Random.Range(0.97f,1.02f), true);
            flashlight_wiggle.FlashlightRandomWiggle();
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        controller.Move(velocity * Time.deltaTime);



        // Включение и поворот фонарика
        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlight"))) && !Globals.pauseOpened && !cutscene)
        {
            if(flight_enabled)
            {
                GlobalFunc.CreateSound(this.gameObject, SoundPrefab, "Audio/Sounds/flashlight_off", 1f, Random.Range(0.97f,1.03f), true);
                flight_enabled = false;
            }
            else if(!flight_enabled)
            {
                GlobalFunc.CreateSound(this.gameObject, SoundPrefab, "Audio/Sounds/flashlight_on", 1f, Random.Range(0.97f,1.03f), true);
                flight_enabled = true;
            }
        }

        if(flight_enabled)
        {
            flashlight.intensity = flight_normal_intensity;
        }
        if(!flight_enabled)
        {
            flashlight.intensity = 0;
        }

        //звук сердцебиения
        if(seeByMonsterTimer > 0 && MonsterAI.monster.heartbeat_player)
        {
            seeByMonsterTimer -= Time.deltaTime;
            if(!sound[1].isPlaying)
            {
               sound[1].Play(); 
            }
        }
        if(seeByMonsterTimer <= 0 && MonsterAI.monster.heartbeat_player)
        {
            if(sound[1].isPlaying)
            {
               sound[1].Stop(); 
            }
        }

        // мигание фонарика
        if(flight_blink && flight_enabled)
        {
            flashlight.intensity = Mathf.Lerp(flashlight.intensity, flight_blink_intensity, 0.26f);
        }
        if(!flight_blink && flight_enabled)
        {
            flashlight.intensity = Mathf.Lerp(flashlight.intensity, flight_normal_intensity, 0.26f);
        }

        if(flight_blink_timer > 0 && seeByMonsterTimer > 0 && MonsterAI.monster.flashlightBlink_player) flight_blink_timer -= Time.deltaTime;

        if(flight_blink_timer <= 0 && seeByMonsterTimer > 0 && MonsterAI.monster.flashlightBlink_player)
        {
            flight_blink = !flight_blink;
            flight_blink_timer = Random.Range(0.04f,0.12f);
        }

        if(seeByMonsterTimer <= 0 || !MonsterAI.monster.flashlightBlink_player)
        {
            flight_blink = false;
        }

        //мрак
        if(seeByMonsterTimer <= 0 && manager.monsterType == 11)
        {
            flashlight.range = Mathf.Lerp(flashlight.range, flight_range, 1 * Time.deltaTime);
        }
        if(seeByMonsterTimer > 0 && manager.monsterType == 11)
        {
            flashlight.range = Mathf.Lerp(flashlight.range, 0, 1 * Time.deltaTime);
        }

        Globals.inGameTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.C) && Application.isEditor)
        {
            Globals.gameOver = false;
            SceneManager.LoadScene("EndCutscene");
        }

        // звуки одежды при беге
        if(sprinting)
        {
            if(!sound[2].isPlaying) sound[2].Play();
        }
        if(!sprinting)
        {
            if(sound[2].isPlaying) sound[2].Stop();
        }

        //катсцена
        if(camera_animator.GetCurrentAnimatorStateInfo(0).IsName("StartCutscene"))
        {
            start_cutscene_now = true;
        }
        //катсцена
        if(!camera_animator.GetCurrentAnimatorStateInfo(0).IsName("StartCutscene"))
        {
            start_cutscene_now = false;
            camera_animator.enabled = false;
        }

        // можео сделать больше катсцен по бул стейту
        if(start_cutscene_now)
        {
            cutscene = true;
        }
        else cutscene = false;

        if(cutscene || Globals.pauseOpened || zoom)
        {
            GameManager.instance.hide_ui = true;
        }
        else
        {
            GameManager.instance.hide_ui = false;
        }
        wsway.cutscene = cutscene;

        if(Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("zoom")))) zoom = true;
        if(Input.GetKeyUp((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("zoom")))) zoom = false;

        if(zoom && !cutscene) cameraComp.fieldOfView = Mathf.Lerp(cameraComp.fieldOfView, zoom_fov_value,  5f * Time.deltaTime);
        
        if(zoom && !cutscene)
        {
            if(Input.GetAxis("Mouse ScrollWheel") < 0f) zoom_fov_value += 3;
            if(Input.GetAxis("Mouse ScrollWheel") > 0f) zoom_fov_value -= 3;
            zoom_fov_value = Mathf.Clamp(zoom_fov_value, zoom_fovs.x, zoom_fovs.y);
        }

        if(undying_effect > -1) undying_effect -= Time.deltaTime;
        if(invisible_effect > -1) invisible_effect -= Time.deltaTime;

        if(posToMonsterTimer > 0) {
            gameObject.transform.position = monsterPos;
            posToMonsterTimer -= Time.deltaTime;
        }

        if(invisible_effect > 0) 
        {
            manager.invisibleOverlay.SetActive(true);
            if(flight_enabled){
                flight_enabled = false;
                GlobalFunc.CreateSound(gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/flashlight_broke", 0.38f, Random.Range(0.96f, 1.04f), false);
            }
            light_invis.intensity = Mathf.Lerp(light_invis.intensity, 0.15f, 0.6f * Time.deltaTime);
        }
        if(invisible_effect <= 0) 
        {
            light_invis.intensity = Mathf.Lerp(light_invis.intensity, 0f, 1f * Time.deltaTime);
            manager.invisibleOverlay.SetActive(false);
        }

        if(speed_multiplier_byeffect != 1f) speed_effect_timer -= Time.deltaTime;
        if(speed_effect_timer <= 0) speed_multiplier_byeffect = 1f;

        if(fovMultiplier != 1f) fov_effect_timer -= Time.deltaTime;
        if(fov_effect_timer <= 0) fovMultiplier = 1f;

        if(manager.journalSound)
        {
            manager.journalSound = false;
            GlobalFunc.CreateSound(gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/journal", 0.62f, Random.Range(0.93f, 1.05f), true);
        }
    }  

    void OnTriggerEnter(Collider other)
    {
        MonsterAI monsteraicomp = other.GetComponent<MonsterAI>();
        if(monsteraicomp != null)
        {
            //если безопасное время кончилось
            if(monsteraicomp.spawnSafeTimer <= 0 && !cutscene && undying_effect <= 0)
            {
                Globals.gameOver = true;
                manager.AddGameToHistory(false);
                SceneManager.LoadScene("EndCutscene");
            }
        }
    }

    void HandleCrouch()
    {
        targetHeight = isCrouching ? 1f : 1.75f; // 1 - присед, 2 - стоя
        float currentHeight = controller.height;
        
        // Плавное изменение высоты через Lerp
        controller.height = Mathf.Lerp(currentHeight, targetHeight, 8f * Time.deltaTime);

        // Корректировка центра персонажа
        Vector3 center = controller.center;
        center.y = controller.height / 2f;
        controller.center = center;

        // Плавное изменение высоты камеры вместе с изменением высоты персонажа
        float targetCamHeight = isCrouching ? 1f : 1.5f; // Высота камеры для приседа и стоя
        Vector3 camLocalPos = new Vector3(0, cam.localPosition.y, 0);
        camLocalPos.y = Mathf.Lerp(camLocalPos.y, targetCamHeight, 8f * Time.deltaTime);
        cam.localPosition = camLocalPos;

        // Обработка ввода для приседания
        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("crouch"))) && !Globals.pauseOpened && !cutscene)
        {
            if (isCrouching)
            {
                hasBlockingCrouchObject = false;
                CheckCrouchBlocking();
                {
                    if(hasBlockingCrouchObject == false)
                    {
                        GlobalFunc.CreateSound(this.gameObject, SoundPrefab, "Audio/Sounds/sit_disable", 1f, Random.Range(0.97f, 1.03f), true);
                        isCrouching = false;
                    }
                }
            }
            else
            {
                GlobalFunc.CreateSound(this.gameObject, SoundPrefab, "Audio/Sounds/sit_enable", 1f, Random.Range(0.97f, 1.03f), true);
                isCrouching = true;
            }
        }
    }

    void CheckCrouchBlocking()
    {
        // Начальная и конечная точки капсулы (нижняя и верхняя границы)
        Vector3 capsuleStart = transform.position + Vector3.up * controller.radius;  // Нижняя точка
        Vector3 capsuleEnd = transform.position + Vector3.up * (1.75f - controller.radius);  // Верхняя точка

        // Радиус капсулы
        float capsuleRadius = controller.radius;

        // Используем Physics.OverlapCapsule для получения всех объектов, которые пересекаются с капсулой
        Collider[] hitColliders = Physics.OverlapCapsule(capsuleStart, capsuleEnd, capsuleRadius, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);

        // Проверяем наличие объектов в капсуле, кроме коллайдера самого игрока
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != this.gameObject) // Игнорируем коллайдер игрока
            {
                Debug.Log("Обнаружено препятствие! Вставание невозможно." + " Объект: " + hitCollider.gameObject.name); // Выводим имя объекта
                hasBlockingCrouchObject = true;
            }
        }
    }

    public void PosToMonster(Vector3 monsterPosGet)
    {
        monsterPos = monsterPosGet;
    }
}
