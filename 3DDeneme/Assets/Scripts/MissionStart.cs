using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStart : MonoBehaviour
{
    public Light volkan_isik;
    public Light volkan_isik_2;

    GameObject panel_go;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            panel_go.SetActive(true);
        }
    }
}
