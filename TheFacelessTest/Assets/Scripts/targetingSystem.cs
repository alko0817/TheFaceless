using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class targetingSystem : MonoBehaviour
//{
//    Camera cam;
//    enemyInView target;

//    int lockedEnemy;
//    bool lockedOn;

//    public static List<enemyInView> nearbyEnemies = new List<enemyInView>();

//    Transform originalLock;


//    void Start()
//    {
//        lockedOn = false;
//        lockedEnemy = 0;
//        originalLock = gameObject.GetComponent<vThirdPersonCamera>().targetLookAt;
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//        if (Input.GetButtonDown("lockOn") && !lockedOn)
//        {
//            if (nearbyEnemies.Count >= 1)
//            {
//                lockedOn = true;
//                lockedEnemy = 0;

//                target = nearbyEnemies[lockedEnemy];

//                gameObject.GetComponent<vThirdPersonCamera>().targetLookAt = target.transform;
//            }

//        }

//        else if ((Input.GetButtonDown("lockOn") && lockedOn) || nearbyEnemies.Count == 0)
//        {
//            gameObject.GetComponent<vThirdPersonCamera>().targetLookAt = originalLock;
//            lockedOn = false;
//            lockedEnemy = 0;
//            target = null;

//        }
//    }
//}
