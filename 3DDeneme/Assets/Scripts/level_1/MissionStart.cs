using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStart : MonoBehaviour
{
    public Light volkan_isik;
    public Light volkan_isik_2;


    public GameObject complate_panel;
    public bool anahtar = false;


    public GameObject panel_go;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !anahtar)
        {
            panel_go.SetActive(true);
           
        }

        if (anahtar)
        {         
            complate_panel.SetActive(true);
            Time.timeScale = 0;
        }

    }
    public void MissionPanelFalse()
    {
        panel_go.SetActive(false);
    }
  
}
