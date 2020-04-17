using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveLoader : MonoBehaviour
{
    GameObject player;
    playerController controller;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<playerController>();
    }

    public void SavePlayer ()
    {
        saveSystem.SavePlayer(controller);
    }

    public void LoadPlayer ()
    {
        playerData data = saveSystem.LoadPlayer();

        Vector3 pos;
        pos.x = data.position[0];
        pos.y = data.position[1];
        pos.z = data.position[2];

        player.transform.position = pos;
    }

}
