using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missions : MonoBehaviour
{
    public GameObject missionPanel;

   public void PanelExitButton()
    {
        missionPanel.SetActive(false);
    }
}
