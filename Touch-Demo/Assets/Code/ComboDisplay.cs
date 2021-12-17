using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboDisplay : MonoBehaviour
{
    public TextMesh comboText;

    public void UpdateCombo()
    {
        int val = PublicVars.comboCount;
        if (val >= 1)
        {
            comboText.text = val.ToString();
        }
        else
        {
            comboText.text = "";
        }
    }
}
