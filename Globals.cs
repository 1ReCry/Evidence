using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;
using OPS.Obfuscator.Attribute;
using System.Security.Cryptography;
using System.Text;

[DoNotObfuscateClass]
public static class Globals
{
    // основные игровые переменные
    public static float inGameTime;
    public static int gameCompletesCount;
    public static int gamesPlayed;
    public static int gameCompletesDifficulty0;
    public static int gameCompletesDifficulty1;
    public static int gameCompletesDifficulty2;
    public static int gameCompletesDifficulty3;
    public static int gameCompletesDifficulty4;
    public static int xp;
    public static int level;
    public static int rankedPoints;
    public static int fullRankedPoints;
    public static int maxRankedPoints;
    public static int rankedLevel;

    public static int monsterMeetings;
    public static int monsterEscaped;
    public static int allGameNotesCollected;
    public static int allItemsCollected;

    public static bool controlsNotSetted = true;

    public static int archonSeals;
 
    // не основные не сохраняемые переменные
    public static string selectedMapSceneName = "";
    public static string selectedMapUIName = "";
    public static int xpPerLevel = 100;
    public static int xpAddPerLevel = 10;
    public static int rpPerLevel = 200;
    public static int difficultyID = 0;
    public static bool rankedGame = false;
    public static bool gameOver = false;
    public static bool pauseOpened = false;
    public static int curMapID;

    //save load
    private static string filePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    private static string filePathDev = Path.Combine(Application.persistentDataPath, "saveDataDev.json"); //saveDataDev
    private static string gamesPath = Path.Combine(Application.persistentDataPath, "gameData.json");
    private static string gamesPathDev = Path.Combine(Application.persistentDataPath, "gameDataDev.json"); //saveDataDev

    //settings
    public static float sensitivity;
    public static int postprocessing;
    public static int fpscounter;

    private static readonly string key = "your-secure-key-32chars"; // 32-символьный ключ для AES-256
    private static readonly string iv = "your-secure-iv-16chars"; // 16-символьный вектор инициализации для AES

    public static GameInfoListWrapper wrapper;

    public static string[] monsterTypeName = 
    {   "Обычный",
        "Агрессивный",
        "Охотник",
        "Тень",
        "Фантом",
        "Демон",
        "Ревенант",
        "Морой",
        "Ракай",
        "Кошмар",
        "Мираж",
        "Мрак"
    };

    public static string[] monsterTypeDesc = 
    {   "Без выразительных особенностей.",
        "На 16% быстрее передвигается\nНа 50% больше шанс поломать фонарик (от изначального шанса)\n-35% к времени ожидания на точке\nНа 10% меньше дальность обзора",
        "На 60% больше дальность обзора\nНа 50% дольше преследлует игрока\nНа 120% дольше чувствует игрока за стенами\nНе издаёт звука дыхания",
        "На 14% медленнее передвигается\nНа 25% больше безопасное время при спавне\nНе издаёт никаких звуков\nВы не почувствуете его приближения",
        "Не издаёт звуков шагов и дыхания\nНа 200% дольше невидим когда мигает",
        "-50% к времени ожидания на точке\nНа 65% больше шанс поломать фонарик (от изначального шанса)\nНа 37% больше безопасное время при спавне",
        "Если у жертвы включен фонарик передвигается в 2 раза быстрее\nЕсли фонарик выключен то передвигается на 3% медленнее\nНа 20% дольше чувствует игрока за стенами",
        "Вызывает частые сбои в работе электроники\nПередвигается на 10% быстрее\nШанс сломать фонарик при телепортации: 70%\nНе издаёт звука телепортации",
        "Ракай не имеет физической оболочки, при приближении Ракая вы почувствуете что то странное.\nПередвигается на 17% медленнее",
        "Если преследует игрока, может вызывать перебои в работе электронники.\nЕсли выключен фонарик передвигается в 2 раза быстрее\n-6% к времени ожидания на точке",
        "Очень большая дальность обзора\nНа 15% больше время ожидания на точке\nНе издает звуков шагов",
        "Если видит игрока, ухудшает ему видимость накладывая мрак.\n-15% к времени ожидания на точке\nНа 40% больше шанс поломать фонарик (от изначального шанса)"
    };

    public static string[] itemName = 
    {   "Кукла вуду",
        "Благовоние",
        "Зеркало",
        "Шапка невидимка",
        "Адреналин",
        "Таблетки",
        "Фонарь",
    };

    public static string[] itemDesc = 
    {   "При использовании монстр не по своей воле телепортируется в случайную точку на карте.",
        "При использовании монстру становится тяжелее ходить, он замедляется давая шанс сбежать.\n(что в мiшочке?)",
        "При использовании вы меняетесь с монстром местами, а так же даёт невидимость на 0.75 сек.",
        "При использовании даёт невидимость на небольшой промежуток времени.",
        "При использовании вы передвигаетесь и регенерируете выносливость быстрее некоторое время.",
        "При использовании вы моментально восстанавливаете всю энергию",
        "Фонарь освещает всё вокруг, он не влияет на то как чувствует монстр игрока за стенами с выключенным фонариком.",
    };

    public static void UpdateLevel()
    {
        while(xp>=100+(xpAddPerLevel*level))
        {
            xp-=100+(xpAddPerLevel*level);
            level+=1;
        }

        while(rankedPoints>=rpPerLevel)
        {
            rankedPoints-=rpPerLevel;
            rankedLevel+=1;
        }
        while(rankedPoints<0)
        {
            rankedPoints+=rpPerLevel;
            rankedLevel-=1;
        }
        //rankedPoints = fullRankedPoints - (rpPerLevel * rankedLevel);
    }

    // SAVE LOAD
    public static void SaveData()
    {
        if(!Application.isEditor)
        {
            GameData data = new GameData();

            Type type = typeof(Globals);
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                var value = field.GetValue(null);
                var fieldName = field.Name;
                var dataField = typeof(GameData).GetField(fieldName);
                if (dataField != null)
                {
                    dataField.SetValue(data, value);
                }
            }

            string json = JsonUtility.ToJson(data, true);
            string encryptedJson = EncryptionUtils.Encrypt(json);

            if (encryptedJson != null)
            {
                File.WriteAllText(filePath, encryptedJson);
                Debug.Log("Data saved to " + filePath);
            }
            else
            {
                Debug.LogError("Ошибка при шифровании данных перед сохранением.");
            }
        }


        if(Application.isEditor)
        {
            GameData data = new GameData();

            Type type = typeof(Globals);
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                var value = field.GetValue(null);
                var fieldName = field.Name;
                var dataField = typeof(GameData).GetField(fieldName);
                if (dataField != null)
                {
                    dataField.SetValue(data, value);
                }
            }

            string json = JsonUtility.ToJson(data, true);
            string encryptedJson = EncryptionUtils.Encrypt(json);

            if (encryptedJson != null)
            {
                File.WriteAllText(filePathDev, encryptedJson);
                Debug.Log("Data saved to " + filePathDev);
            }
            else
            {
                Debug.LogError("Ошибка при шифровании данных перед сохранением.");
            }
        }
        //if(gameHistory != null) GameHistoryManager.Instance.gameHistory = new List<GameInfo>(gameHistory);
    }

    public static void LoadData()
    {
        if(!Application.isEditor)
        {
            if (File.Exists(filePath))
            {
                string encryptedJson = File.ReadAllText(filePath);
                string json = EncryptionUtils.Decrypt(encryptedJson);
                GameData data = JsonUtility.FromJson<GameData>(json);;

                Type type = typeof(Globals);
                var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                foreach (var field in fields)
                {
                    var fieldName = field.Name;
                    var dataField = typeof(GameData).GetField(fieldName);
                    if (dataField != null)
                    {
                        var value = dataField.GetValue(data);
                        field.SetValue(null, value);
                    }
                }

                Debug.Log("Data loaded from " + filePath);
            }
            else
            {
                Debug.LogWarning("Save file not found at " + filePath);
            }
        }

        if(Application.isEditor)
        {
            if (File.Exists(filePathDev))
            {
                string encryptedJson = File.ReadAllText(filePathDev);
                string json = EncryptionUtils.Decrypt(encryptedJson);
                GameData data = JsonUtility.FromJson<GameData>(json);

                Type type = typeof(Globals);
                var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                foreach (var field in fields)
                {
                    var fieldName = field.Name;
                    var dataField = typeof(GameData).GetField(fieldName);
                    if (dataField != null)
                    {
                        var value = dataField.GetValue(data);
                        field.SetValue(null, value);
                    }
                }

                Debug.Log("Data loaded from " + filePathDev);
            }
            else
            {
                Debug.LogWarning("Save file not found at " + filePathDev);
            }
        }
    }

    public static void SaveGamesData()
    {
        if(!Application.isEditor)
        {
            string filePathGames = gamesPath;
            GameInfoListWrapper wrapper = new GameInfoListWrapper { Games = GameHistoryManager.Instance.gameHistory };
            string json = JsonUtility.ToJson(wrapper, true);

            Debug.Log("Current game history count: " + GameHistoryManager.Instance.gameHistory.Count);
            foreach (var game in GameHistoryManager.Instance.gameHistory)
            {
                Debug.Log("Game ID: " + game.MapID); // или другие поля для проверки
            }

            File.WriteAllText(filePathGames, json);
            string jsonDebug = File.ReadAllText(filePathGames);
            Debug.Log(jsonDebug);

            Debug.Log("Game history saved to " + filePathGames);
        }
        if(Application.isEditor)
        {
            string filePathGames = gamesPathDev;
            GameInfoListWrapper wrapper = new GameInfoListWrapper { Games = GameHistoryManager.Instance.gameHistory };
            string json = JsonUtility.ToJson(wrapper, true);

            Debug.Log("Current game history count: " + GameHistoryManager.Instance.gameHistory.Count);
            foreach (var game in GameHistoryManager.Instance.gameHistory)
            {
                Debug.Log("Game ID: " + game.MapID); // или другие поля для проверки
            }

            File.WriteAllText(filePathGames, json);
            string jsonDebug = File.ReadAllText(filePathGames);
            Debug.Log(jsonDebug);

            Debug.Log("Game history saved to " + filePathGames);
        }
    }

    public static void LoadGamesData()
    {
        if(!Application.isEditor)
        {
            string filePathGames = gamesPath;
        
            if (File.Exists(filePathGames))
            {
                string json = File.ReadAllText(filePathGames);
                GameInfoListWrapper wrapper = JsonUtility.FromJson<GameInfoListWrapper>(json);
                GameHistoryManager.Instance.gameHistory = wrapper?.Games ?? new List<GameInfo>();
                Debug.Log("Game history loaded.");
            }
            else
            {
                Debug.LogWarning("Game history file not found at " + filePathGames);
            }
        }
        if(Application.isEditor)
        {
            string filePathGames = gamesPathDev;
        
            if (File.Exists(filePathGames))
            {
                string json = File.ReadAllText(filePathGames);
                GameInfoListWrapper wrapper = JsonUtility.FromJson<GameInfoListWrapper>(json);
                GameHistoryManager.Instance.gameHistory = wrapper?.Games ?? new List<GameInfo>();
                Debug.Log("Game history loaded dev.");
            }
            else
            {
                Debug.LogWarning("Game history file not found at " + filePathGames);
            }
        }
    }
}
