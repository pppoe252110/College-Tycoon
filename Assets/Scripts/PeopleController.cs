using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeopleController : MonoBehaviour
{
    public PersonController personPrefab;
    public GameObject[] skins;
    public List<PersonController> people = new List<PersonController>();

    public Transform exitPoint;
    public Text population;

    public static PeopleController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        population.text = people.Count + "/" + GridBuilder.Instance.GetFreeDormitoryPlaces();
    }
}
