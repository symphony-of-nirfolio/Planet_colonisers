using System;
using UnityEngine;

public class ObjectsToImages : MonoBehaviour
{
    [Serializable]
    public class GameObjectToBuildingItem
    {
        public GameObject gameObject;
        public BuildingItem buildingItem;
    }
    

    public BuildingItemList buildingItemList;

    [Tooltip("Have to be matching in same order as in Building Item List")]
    public GameObjectToBuildingItem[] gameObjectToBuildingItems;


    private void TakeAllSnapshots()
    {
        for (int i = 0; i < gameObjectToBuildingItems.Length; ++i)
        {
            gameObjectToBuildingItems[i].gameObject.SetActive(true);

            SnapshotCamera snapshotCamera =
                gameObjectToBuildingItems[i].gameObject.GetComponent<SnapshotCamera>();

            if (gameObjectToBuildingItems[i].gameObject.TryGetComponent(out PreparerTo3DSnapshot preparerTo3DSnapshot))
            {
                preparerTo3DSnapshot.Prepare();
            }

            if (!snapshotCamera)
            {
                Debug.LogError("SnapshotCamera didn't find");
            }
            else
            {
                snapshotCamera.TakeSnapshot(string.Format(
                    "{0}/RenderedImages/{1}.png",
                    Application.dataPath,
                    buildingItemList.buildingItems[i].name));
            }

            if (preparerTo3DSnapshot)
            {
                preparerTo3DSnapshot.ResetToPrevious();
            }

            gameObjectToBuildingItems[i].gameObject.SetActive(false);
        }

        Debug.Log("Completed taking snapshots");
    }

    private void DisableAll()
    {
        foreach (GameObjectToBuildingItem gameObjectToBuildingItem in gameObjectToBuildingItems)
        {
            gameObjectToBuildingItem.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (buildingItemList.buildingItems.Length !=
            gameObjectToBuildingItems.Length)
        {
            Debug.LogError("Not all building items matched");
            return;
        }

        bool is_contains_errors = false;
        for (int i = 0; i < buildingItemList.buildingItems.Length; ++i)
        {
            if (gameObjectToBuildingItems[i].buildingItem !=
                buildingItemList.buildingItems[i])
            {
                Debug.LogError("Wrong matching");
                is_contains_errors = true;
            }

            if (!gameObjectToBuildingItems[i].gameObject)
            {
                Debug.LogError("Empty game object");
                is_contains_errors = true;
            }
        }

        if (is_contains_errors)
        {
            return;
        }

        DisableAll();
        TakeAllSnapshots();
    }
}
