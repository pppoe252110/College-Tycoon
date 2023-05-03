using UnityEngine;
using UnityEngine.UI;

public class IdentityManager : MonoBehaviour
{
    public IdentityPreset identityPreset;
    public float money = 10000;
    public Text moneyText;
    public float mood = 1f;
    public Slider moodSlider;
    public Slider Slider;
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
        if(GridBuilder.Instance.GetFreeDormitoryPlaces() < PeopleController.Instance.people.Count)
        {
            mood -= Time.deltaTime / 120f;
        }
        moodSlider.value = mood;
    }
}
