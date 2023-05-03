using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour
{
    public Transform startPoint;
    public Transform stopPoint;
    public Transform endPoint;
    public Transform exit;
    public Transform[] wheels;

    Vector3 lastPos;

    private void Start()
    {
        transform.position = startPoint.position;
        MoveToStopPoint();
    }

    public void ReleasePeople()
    {
        if (GridBuilder.Instance.GetFreeDormitoryPlaces() < PeopleController.Instance.people.Count)
            return;
        int count = GridBuilder.Instance.GetFreeDormitoryPlaces()-PeopleController.Instance.people.Count;
        for (int i = 0; i < count; i++)
        {
            var person = Instantiate(PeopleController.Instance.personPrefab, exit.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), Quaternion.Euler(0, 90f, 0));
            person.Init();
        }
    }

    public void MoveToStopPoint()
    {
        transform.DOMove(stopPoint.position, 10).SetDelay(1).OnComplete(() => { ReleasePeople(); MoveToEndPoint(); });
    }

    public void MoveToEndPoint()
    {
        transform.DOMove(endPoint.position, 30).SetEase(Ease.InSine).SetDelay(5).OnComplete(() => { transform.position = startPoint.position; MoveToStopPoint(); });
    }

    void Update()
    {
        Vector3 velocity = transform.position - lastPos;
        lastPos = transform.position;
        var localVelocity = transform.InverseTransformDirection(velocity);
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].transform.localRotation *= Quaternion.Euler(localVelocity.z * Mathf.Rad2Deg * 1.1f, 0, 0);
        }
            
    }
}
