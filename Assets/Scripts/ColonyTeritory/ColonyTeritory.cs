using System.Collections.Generic;
using UnityEngine;

public class ColonyTeritory : MonoBehaviour
{
    public GameObject availableHexPrefab;


    private readonly List<AvailableHex> availableHexes = new List<AvailableHex>();

    private bool isPlayer = true;
    
    public void InitAvailableHexes(List<Vector3> positions, bool isPlayer)
    {
        this.isPlayer = isPlayer;

        foreach (AvailableHex availableHex in availableHexes)
        {
            Destroy(availableHex.gameObject);
        }

        availableHexes.Clear();

        foreach (Vector3 position in positions)
        {
            GameObject availableHex = Instantiate(availableHexPrefab, position, Quaternion.identity, transform);
            availableHexes.Add(availableHex.GetComponent<AvailableHex>());
        }

        Unhighlight();
    }

    public void Unhighlight()
    {
        foreach (AvailableHex availableHex in availableHexes)
        {
            if (isPlayer)
            {
                availableHex.SetPlayerBaseColor();
            }
            else
            {
                availableHex.SetNpcBaseColor();
            }
        }
    }

    public void Highlight()
    {
        foreach (AvailableHex availableHex in availableHexes)
        {
            if (isPlayer)
            {
                availableHex.SetPlayerHighlightColor();
            }
            else
            {
                availableHex.SetNpcHighlightColor();
            }
        }
    }
}
