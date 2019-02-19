using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    private void Start()
    {
        Game.Instance.SpawnZoneOfLevel = GameObject.Find("SpawnZone").GetComponent<SpawnZone>();
    }
}
