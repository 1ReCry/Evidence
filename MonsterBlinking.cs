using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBlinking : MonoBehaviour
{
    private MonsterAI ai;
    private float blink_timer;

    void Start()
    {
        ai = GetComponent<MonsterAI>();
    }

    void Update()
    {
        // основная блинк логика
        if(!ai.blink_enabled) ai.model_mesh.SetActive(true);
        if(ai.blink_enabled) ai.model_mesh.SetActive(false);

        // таймеры
        if(blink_timer > 0) blink_timer -= Time.deltaTime;
        if(blink_timer <= 0)
        {
            if(!ai.blink_enabled)
            {
                blink_timer = Random.Range(ai.blink_invisible_time.x, ai.blink_invisible_time.y);
                ai.blink_enabled = true;
            }
            else if(ai.blink_enabled)
            {
                blink_timer = Random.Range(ai.blink_visible_time.x, ai.blink_visible_time.y);
                ai.blink_enabled = false;
            }
        }
    }
}
