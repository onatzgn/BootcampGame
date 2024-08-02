using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class award : MonoBehaviour
{
    MissionStart missionStart;

    private void Awake()
    {
        missionStart = FindFirstObjectByType<MissionStart>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            missionStart.anahtar = true;
            gameObject.SetActive(false);
        }
    }
}
