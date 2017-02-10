using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private static GameData instance;

    public static GameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameData();
            }
            return instance;
        }
    }


    public List<System.Type> ZombieTypeTypes;
    public List<System.Type> ZombieBehaviorTypes;

    public Player.PLAYERTYPE PlayerType;   

    GameData()
    {
        ZombieTypeTypes = new List<System.Type>();
        ZombieBehaviorTypes = new List<System.Type>();

        ZombieTypeTypes.Add(typeof(ZTNormal));
        ZombieTypeTypes.Add(typeof(ZTFast));
        ZombieTypeTypes.Add(typeof(ZTSmall));
        ZombieTypeTypes.Add(typeof(ZTBig));
        ZombieTypeTypes.Add(typeof(ZTHeavy));
        ZombieBehaviorTypes.Add(typeof(ZBNormal));
        ZombieBehaviorTypes.Add(typeof(ZBRunAndStop));
        ZombieBehaviorTypes.Add(typeof(ZBJump));
        ZombieBehaviorTypes.Add(typeof(ZBBomb));
        ZombieBehaviorTypes.Add(typeof(ZBClimb));
    }

}
