using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {


    private static UIManager _instance;
    public Text colCountText;

    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public int amountCoins;


    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        else
            _instance = this;
    }

    public void CointCount()
    {
        amountCoins++;
        UpdateText(colCountText , amountCoins.ToString());
    }

    public void UpdateText(Text text, string newText)
    {
        text.text = newText;
    }
}
