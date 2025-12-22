using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public Text scoreText;
    public Text itemsText;

    // Start is called before the first frame update
    void Start()
    {
         if (GameManager.Instance != null)
        {
            GameManager.Instance.SetHUD(scoreText, itemsText);
        }
    }

   
}
