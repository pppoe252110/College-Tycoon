using DG.Tweening;
using Pathfinding;
using System.Linq;
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
    public LayerMask fakeBuildingLayer = 1 << 6;
    private float rot = 0;
    public ParticleSystem destroyParticles;
    public bool destroyMode = false;
    public MeshRenderer hoveredBuiling;
    public Material hoveredBuilingMat;

    private void Start()
    {
        SetBuilding(targetBuilding);
    }

    public void SwitchDestroyMode()
    {
        destroyMode = !destroyMode;
        if (destroyMode)
        {
            if (fakeBuilding)
                fakeBuilding.SetActive(false);
        }
        else
        {
            if (hoveredBuiling)
            {
                hoveredBuiling.material = hoveredBuilingMat;
                hoveredBuiling = null;
                hoveredBuilingMat = null;
            }
        }
    }

    void Update()
    {
        rot += Input.GetKeyDown(KeyCode.R) ? 45 : 0;
        if (destroyMode)
        {
            var a = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(a, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out Building building) && building.enabled)
                {
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        if (EventSystem.current.IsPointerOverGameObject())
                            return;
                        IdentityManager.Instance.money += building.buildingItem.price / 2f;
                        Instantiate(destroyParticles, building.transform.position + Vector3.up * 5, Quaternion.identity);
                        var g = GridBuilder.Instance.grid[GridBuilder.Instance.grid.FirstOrDefault(x => x.Value == building).Key] = null;
                        Destroy(building.gameObject);
                    }
                    if (building.TryGetComponent(out MeshRenderer meshRenderer))
                    {
                        if (meshRenderer != hoveredBuiling)
                        {
                            if (hoveredBuiling)
                                hoveredBuiling.material = hoveredBuilingMat;

                            hoveredBuiling = meshRenderer;
                            hoveredBuilingMat = meshRenderer.material;
                            hoveredBuiling.material = fakeBuildingBlocked;
                        }
                    }
                }
                else if (hoveredBuiling)
                {
                    hoveredBuiling.material = hoveredBuilingMat;
                    hoveredBuiling = null;
                    hoveredBuilingMat = null;

                }
            }
        }
        else if (targetBuilding)
        {
            var a = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(a, out RaycastHit hit, Mathf.Infinity, floorLevel))
            {
                if (hit.transform.gameObject.TryGetComponent(out GridBuilder builder))
                {
                    if (builder.grid == null)
                        return;
                    var point = builder.GetClosestToPoint(hit.point);
                    var targetPos = new Vector3(point.x, 0, point.y);
                    var v = builder.grid[point];
                    if (v || !IdentityManager.Instance.CanAfford(targetBuilding.price))
                    {
                        fakeBuildingRenderer.material = fakeBuildingBlocked;
                    }
                    else
                    {
                        fakeBuildingRenderer.material = fakeBuildingAllowed;
                    }
                    fakeBuilding.transform.position = targetPos;
                    fakeBuilding.transform.rotation = Quaternion.Euler(0, rot, 0);

                    if (EventSystem.current.IsPointerOverGameObject())
                        return;

                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        if (!builder.grid[point])
                        {
                            if (IdentityManager.Instance.CanAfford(targetBuilding.price))
                            {
                                IdentityManager.Instance.money -= targetBuilding.price;
                                var b = Instantiate(targetBuilding.buildingPrefab, targetPos, Quaternion.Euler(0, rot, 0));
                                b.buildingItem = targetBuilding;
                                b.transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 0).SetEase(Ease.InBack);
                                builder.grid[point] = b;
                            }
                            else
                            {
                                fakeBuilding.transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 0).SetEase(Ease.InBack).OnComplete(() => { fakeBuilding.transform.localScale = Vector3.one; });
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetBuilding(BuildingItem building)
    {
        destroyMode = false;
        if (hoveredBuiling)
        {
            hoveredBuiling.material = hoveredBuilingMat;
            hoveredBuiling = null;
            hoveredBuilingMat = null;
        }
        if (fakeBuilding)
        {
            Destroy(fakeBuilding);
        }
        targetBuilding = building;
        if (building == null) return;
        fakeBuilding = Instantiate(building.buildingPrefab.gameObject);
        fakeBuilding.name = "FakeBuilding";
        fakeBuilding.layer = 1 >> fakeBuildingLayer;
        fakeBuildingRenderer = fakeBuilding.GetComponent<MeshRenderer>();
        fakeBuildingRenderer.material = fakeBuildingAllowed;
        if (fakeBuilding.TryGetComponent(out NavmeshCut cut))
            cut.enabled = false;
        foreach (var v in fakeBuilding.GetComponents<Collider>())
        {
            v.enabled = false;
        }
    }
}
