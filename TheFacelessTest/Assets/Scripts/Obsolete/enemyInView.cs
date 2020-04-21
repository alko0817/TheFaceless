using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class enemyInView : MonoBehaviour
//{

//    Camera cam;
//    bool addIn;


//    void Start()
//    {
//        cam = Camera.main;
//        addIn = true;

//    }


//    void Update()
//    {
//        Vector3 enemyPos = cam.WorldToViewportPoint(gameObject.transform.position);

//        bool onScreen = enemyPos.z > 0 && enemyPos.x > 0 && enemyPos.x < 1 && enemyPos.y > 0 && enemyPos.y < 1;

//        if (addIn && onScreen)
//        {
//            addIn = false;
//            targetingSystem.nearbyEnemies.Add(this);
//        }
//    }
//}
