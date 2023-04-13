using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeopleController : MonoBehaviour
{
    public List<PersonController> people = new List<PersonController>();

    public Text population;
    public Slider currentComfortSlider;
    public float currentComfort = 0;
    public Slider currentEducationQualitySlider;
    public float currentEducationQuality = 0;
    public int currentPopulation; 
    public int maxPopulation; 

    private void Update()
    {
        currentComfortSlider.value = currentComfort;
        currentEducationQualitySlider.value = currentEducationQuality;
        population.text = currentComfort + "/" + maxPopulation;
    }
}
