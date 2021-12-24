using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PublicVars
{
    public static bool useGravity = true;
    public static int comboCount = 0;
    public static int max_hp = 3;
    public static int collectibles = 0;
    public static Vector2 spawnPos = new Vector2(-83.9f, -21.3f);
    public static Part part;
    public static Part nextPart = null;
    public static bool nextPartSet = false;
    public static int difficulty = 5;
}
