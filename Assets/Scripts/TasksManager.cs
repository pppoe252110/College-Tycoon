using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TasksManager : MonoBehaviour
{
    public GameObject taskPrefab;
    public List<TaskItem> taskItems = new List<TaskItem>();
    public Transform tasksContent;
    public static TasksManager Instance;
    private TaskItem notEnoughSpace;
    private TaskItem noDormitry;
    private TaskItem toColledge;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        int freeSpace = GridBuilder.Instance.GetFreeBuildingSpace<DormitoryBuilding>();

        CheckTask(freeSpace < PeopleController.Instance.people.Count || freeSpace == 0, "Студентам негде жить", ref notEnoughSpace);
        CheckTask(GridBuilder.Instance.GetAllBuildingsOfType<DormitoryBuilding>().Count <= 0, "Постройте общежитие", ref noDormitry);
        CheckTask(GridBuilder.Instance.GetAllBuildingsOfType<ColledgeBuilding>().Count <= 0, "Постройте учебную часть", ref toColledge);
    }

    public void CheckTask(bool condition, string message, ref TaskItem taskItem)
    {
        if (condition && taskItem == null)
        {
            taskItem = GenerateTask(message);
        }
        else if(!condition && taskItem != null)
        {
            Destroy(taskItem.taskPrefab);
            taskItem = null;
        }
    }

    public TaskItem GenerateTask(string message)
    {
        var taskObj = Instantiate(taskPrefab, tasksContent);
        taskObj.GetComponentInChildren<Text>().text = message;
        return new TaskItem(message, taskObj);
    }

    
}
public class TaskItem
{
    public string message;
    public GameObject taskPrefab;
    public TaskItem(string message, GameObject prefab)
    {
        this.message = message;
        this.taskPrefab = prefab;
    }
}