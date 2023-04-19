using UnityEngine;
using UnityEngine.UI;

public class IdentityManager : MonoBehaviour
{
    public IdentityPreset identityPreset;
    public float money = 10000;
    public Text moneyText;
    public static IdentityManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool CanAfford(float value)
    {
        return value <= money;
    }

    public void Update()
    {

        moneyText.text = SFNuffix.GetFullValue(money, 1);
    }
}
