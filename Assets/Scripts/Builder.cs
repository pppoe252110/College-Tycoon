using DG.Tweening;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    public BuildingItem targetBuilding;
    public Material fakeBuildingAllowed;
    public Material fakeBuildingBlocked;
    private GameObject fakeBuilding;
    private MeshRenderer fakeBuildingRenderer;
    public LayerMask floorLevel = Physics.AllLayers;
    public LayerMask fakeBuildingLayer = 1<<6;
    private float rot = 0;

    private void Start()
    {
        SetBuilding(targetBuilding);
    }

    void Update()
    {

        rot += Mathf.Sign(Input.mouseScrollDelta.y) * 45f * (rot != 0 ? 1 : 0);
        if (targetBuilding)
        {
            var a = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(a, out RaycastHit hit, Mathf.Infinity, floorLevel))
            {
                if (hit.transform.gameObject.TryGetComponent(out GridBuilder builder))
                {
                    if (builder.grid==null)
                        return;
                    var point = builder.GetClosestToPoint(hit.point);
                    var targetPos = new Vector3(point.x, 0, point.y);
                    var v = builder.grid[point];
                    if (v)
                    {
                        fakeBuildingRenderer.material = fakeBuildingBlocked;
                    }
                    else
                    {
                        fakeBuildingRenderer.material = fakeBuildingAllowed;
                    }
                    fakeBuilding.transform.position = targetPos;
                    fakeBuilding.transform.rotation = Quaternion.Euler(0, rot, 0);

                    if (Input.GetKeyUp(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if (!builder.grid[point])
                        {
                            var b = Instantiate(targetBuilding.buildingPrefab, targetPos, Quaternion.Euler(0, rot, 0));
                            b.transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 0).SetEase(Ease.InBack);
                            builder.grid[point] = b;
                        }
                    }
                }
            }
        }
    }

    public void SetBuilding(BuildingItem building)
    {
        if (fakeBuilding)
        {
            Destroy(fakeBuilding);
        }
        targetBuilding = building;
        if (building == null) return;
        fakeBuilding = Instantiate(building.buildingPrefab.gameObject);
        fakeBuilding.name = "FakeBuilding";
        fakeBuilding.layer = 1>>fakeBuildingLayer;
        fakeBuildingRenderer = fakeBuilding.GetComponent<MeshRenderer>();
        fakeBuildingRenderer.material = fakeBuildingAllowed;
        if(fakeBuilding.TryGetComponent(out NavmeshCut cut)) 
            cut.enabled = false;
        foreach (var v in fakeBuilding.GetComponents<Collider>())
        {
            v.enabled = false;
        }
    }
}
