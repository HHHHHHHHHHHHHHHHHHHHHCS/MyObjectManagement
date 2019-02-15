using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("UIRoot").GetComponent<UIManager>();
            }

            return instance;
        }
    }


    private void Awake()
    {
        Transform root = transform;
        Slider creationSpeedSlider = root.Find("CreationSpeedSlider").GetComponent<Slider>();
        Slider destructionSpeedSlider = root.Find("DestructionSpeedSlider").GetComponent<Slider>();

        creationSpeedSlider.onValueChanged.AddListener(Game.Instance.ChangeCreationSpeed);
        destructionSpeedSlider.onValueChanged.AddListener(Game.Instance.ChangeDestructionSpeed);
    }
}
