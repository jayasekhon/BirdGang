using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss : MonoBehaviour
{
    public GameObject body;
    public GameObject beak;
    public GameObject bottom_beak;
    private mouth mouth;
    public bool boss_enabled;
    private bool flag;
    public bool mouth_enabled;
    // Start is called before the first frame update
    void Start()
    {
        mouth = bottom_beak.GetComponent<mouth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flag != boss_enabled)
        {
            if (boss_enabled)
            {
                body.SetActive(true);
                beak.SetActive(true);
                bottom_beak.SetActive(true);


            }
            else
            {
                body.SetActive(false);
                beak.SetActive(false);
                bottom_beak.SetActive(false);

            }
            flag = boss_enabled;
        }
        if (boss_enabled)
        {
            mouth.mouth_enabled = mouth_enabled;

        }
    }
    public void PlayMouthMove(AudioClip clip)
    {
        mouth.PlayMouthMove(clip);
    }
}
