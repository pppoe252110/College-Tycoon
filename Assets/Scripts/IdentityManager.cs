using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IdentityManager : MonoBehaviour
{
    public IdentityPreset identityPreset;
    public float money = 10000;
    public Text moneyText;
    public float mood = 1f;
    public Slider moodSlider;
    public Slider educationQualitySlider;
    public static IdentityManager Instance { get; private set; }

    public float GetEducationQualtity()
    {
        if (PeopleController.Instance.people.Count == 0)
            return 1;
        return GetEducationSpaces() / PeopleController.Instance.people.Count;
    }

    public float GetEducationSpaces()
    {
        float result = 0f;
        var b = GridBuilder.Instance.GetAllBuildingsOfType<ColledgeBuilding>();
        for (int i = 0; i < b.Count; i++)
        {
            result += b[i].maxPeople;
        }
        return result;
    }

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        moneyText.text = SFNuffix.GetFullValue(money, 1);
        if (GridBuilder.Instance.GetFreeBuildingSpace<DormitoryBuilding>() < PeopleController.Instance.people.Count || GetEducationSpaces() < PeopleController.Instance.people.Count)
        {
            mood -= Time.deltaTime / 120f;
        }
        else
        {
            mood += Time.deltaTime / 120f;
        }
        mood = Mathf.Clamp01(mood);
        moodSlider.value = mood;
        educationQualitySlider.value = GetEducationQualtity();
    }
}
