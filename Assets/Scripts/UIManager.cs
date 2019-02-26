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

    private Slider creationSpeedSlider;
    private Slider destructionSpeedSlider;

    private void Awake()
    {
        Transform root = transform;
        creationSpeedSlider = root.Find("CreationSpeedSlider").GetComponent<Slider>();
        destructionSpeedSlider = root.Find("DestructionSpeedSlider").GetComponent<Slider>();

        creationSpeedSlider.onValueChanged.AddListener(Game.Instance.ChangeCreationSpeed);
        destructionSpeedSlider.onValueChanged.AddListener(Game.Instance.ChangeDestructionSpeed);
    }

    public void SetCreationSpeedValue(float val)
    {
        creationSpeedSlider.value = val;
    }

    public void SetDestructionSpeedValue(float val)
    {
        destructionSpeedSlider.value = val;
    }
}