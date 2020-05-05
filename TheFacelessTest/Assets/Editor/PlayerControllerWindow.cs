using UnityEngine;
using UnityEditor;



public class PlayerControllerWindow : EditorWindow
{
    
    playerController controller;
    Vector2 scroll;

    bool aoe;
    bool lightDamage;
    bool otherDamage;

    bool delay;
    bool connect;

    bool dodge;
    bool block;

    bool discharge;




    private void OnEnable()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
    }

    [MenuItem("Window/Player Controller")]
    public static void ShowWindow ()
    {
        GetWindow<PlayerControllerWindow>("Player Controller");
    }



    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        controller.canDie = EditorGUILayout.Toggle("Player can die", controller.canDie);
        
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        scroll = GUILayout.BeginScrollView(scroll);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("AREA OF EFFECTS", EditorStyles.boldLabel);
        aoe = EditorGUILayout.Foldout(aoe, "Radius");
        if (aoe)
        {
            controller.attackRadius = EditorGUILayout.FloatField("Normal Attacks Radius", controller.attackRadius);
            controller.aoeRadius = EditorGUILayout.FloatField("Discharge Attack Radius", controller.aoeRadius);
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DAMAGE", EditorStyles.boldLabel);

        lightDamage = EditorGUILayout.Foldout(lightDamage, "Light Attacks");
        if (lightDamage)
        {

            controller.slashDamage = EditorGUILayout.IntField("Light attack 1", controller.slashDamage);
            controller.slash2Damage = EditorGUILayout.IntField("Light attack 2", controller.slash2Damage);
            controller.slash3Damage = EditorGUILayout.IntField("Light attack 3", controller.slash3Damage);
            controller.slash4Damage = EditorGUILayout.IntField("Light attack 4", controller.slash4Damage);
        }

        otherDamage = EditorGUILayout.Foldout(otherDamage, "Heavy & Discharge Attack");
        if (otherDamage)
        {
            controller.heavyDamage = EditorGUILayout.IntField("Heavy Attack", controller.heavyDamage);
            controller.dischargeDamage = EditorGUILayout.IntField("Discharge Attack", controller.dischargeDamage);
        }

        EditorGUILayout.Space();

        //EditorGUILayout.LabelField("ATTACK INTERVALS", EditorStyles.boldLabel);
        delay = EditorGUILayout.Foldout(delay, "ATTACK INTERVALS", EditorStyles.boldLabel);
        if (delay)
        {
            controller.attackDelay1 = EditorGUILayout.FloatField("Light Attack 1", controller.attackDelay1);
            controller.attackDelay2 = EditorGUILayout.FloatField("Light Attack 2", controller.attackDelay2);
            controller.attackDelay3 = EditorGUILayout.FloatField("Light Attack 3", controller.attackDelay3);
            controller.attackDelay4 = EditorGUILayout.FloatField("Light Attack 4", controller.attackDelay4);
            controller.nextAttack = EditorGUILayout.FloatField("Delay before combo", controller.nextAttack);
            EditorGUILayout.Space();
            controller.heavyDelay1 = EditorGUILayout.FloatField("Heavy Attack", controller.heavyDelay1);
            controller.dischargeDelay = EditorGUILayout.FloatField("Discharge", controller.dischargeDelay);
        }
        EditorGUILayout.Space();
        //EditorGUILayout.LabelField("ATTACK CONNECTIONS", EditorStyles.boldLabel);
        connect = EditorGUILayout.Foldout(connect, "ATTACK CONNECTIONS", EditorStyles.boldLabel);

        if (connect)
        {

            EditorGUILayout.LabelField("Light attacks");
            controller.hitLight1 = EditorGUILayout.FloatField("Light Attack 1", controller.hitLight1);
            controller.hitLight2 = EditorGUILayout.FloatField("Light Attack 2", controller.hitLight2);
            controller.hitLight3 = EditorGUILayout.FloatField("Light Attack 3", controller.hitLight3);
            controller.hitLight4 = EditorGUILayout.FloatField("Light Attack 4", controller.hitLight4);
            EditorGUILayout.Space();
            controller.hitHeavy = EditorGUILayout.FloatField("Heavy", controller.hitHeavy);
            controller.hitDischarge = EditorGUILayout.FloatField("Discharge", controller.hitDischarge);

        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DODGING / BLOCKING MECHANICS", EditorStyles.boldLabel);

        dodge = EditorGUILayout.Foldout(dodge, "Dodging");
        if (dodge)
        {
            controller.dodgeCooldown = EditorGUILayout.Slider("Cooldown", controller.dodgeCooldown, 1f, 2f);
            controller.dodgeCost = EditorGUILayout.Slider("Stamina cost", controller.dodgeCost, .1f, .8f);
            controller.dodgeDashBoost = EditorGUILayout.Slider("Dash Boost", controller.dodgeDashBoost, 1f, 5f);
        }

        block = EditorGUILayout.Foldout(block, "Blocking");
        if (block)
        {
            controller.blockingSpeed = EditorGUILayout.FloatField("Blocking move speed", controller.blockingSpeed);
        }

        EditorGUILayout.Space();
        discharge = EditorGUILayout.Foldout(discharge, "DISCHARGE MECHANIC", EditorStyles.boldLabel);
        if (discharge)
        {
            controller.maxCharge = EditorGUILayout.FloatField("Max sword charge", controller.maxCharge);
            controller.chargeRate = EditorGUILayout.FloatField("Charge rate", controller.chargeRate);
            controller.UIChargeMultiplier = EditorGUILayout.FloatField("UI charge rate", controller.UIChargeMultiplier);
        }
        




        EditorGUILayout.Space(40);
        GUILayout.EndScrollView();
    }
}

