using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class IdentityManager : MonoBehaviour
{
    public IdentityPreset identityPreset;
    public float money = 0;
    public Text moneyText;
    public static IdentityManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        
        moneyText.text = SFNuffix.GetFullValue(money);
    }
}
