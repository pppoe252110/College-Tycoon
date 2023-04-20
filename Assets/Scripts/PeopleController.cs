using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeopleController : MonoBehaviour
{
    public PersonController personPrefab;
    public GameObject[] skins;
    public List<PersonController> people = new List<PersonController>();

    public Text population;
    public Slider currentComfortSlider;
    public float currentComfort = 0;
    public Slider currentEducationQualitySlider;
    public float currentEducationQuality = 0;

    public static PeopleController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        currentComfortSlider.value = currentComfort;
        currentEducationQualitySlider.value = currentEducationQuality;
        population.text = people.Count + "/" + GridBuilder.Instance.GetFreeDormitoryPlaces();
    }
}
