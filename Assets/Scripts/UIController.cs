using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update

    public Text timeValue;
    public GameObject gameoverPanel;

    public bool isGameOver;

    public float timeRemain = 600f;

    void Start()
    {
        timeRemain = 600f;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(timeRemain >= 0)
        {
            timeRemain -= Time.deltaTime;
            timeValue.text = timeRemain.ToString("0.0") + "s";
        }else if(timeRemain < 0 )
        {
            gameoverPanel.SetActive(true);
        }




    }
}
