using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Transform centerPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        // InitSpawnPoints(6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitSpawnPoints(int count)
    {
        Vector3[] positions = GetPointsAtCircle(count - 1, centerPoint.transform.position, 0.35f);
        
        for (int i = 0; i < count - 1; i++)
        {
            Vector3 position = positions[i];
            GameObject spawnPoint = Instantiate(centerPoint.gameObject, position, Quaternion.identity, transform);
            spawnPoint.name = "Pos " + (i + 2);
            
            GetComponentInParent<TileController>().stickManSpawnPoints.Add(spawnPoint.transform);
        }
        
        GetComponentInParent<TileController>().stickManSpawnPoints.Add(centerPoint);
    }
    
    public Vector3[] GetPointsAtCircle (int pointCount, Vector3 center, float radius)
    {
        Vector3[] points = new Vector3[pointCount];
        
        for (int i = 0; i < pointCount; i++){
         
            /* Distance around the circle */  
            var radians = 2 * Mathf.PI / pointCount * i;
         
            /* Get the vector direction */ 
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians); 
         
            var spawnDir = new Vector3 (horizontal, 0, vertical);
         
            /* Get the spawn position */ 
            var spawnPos = center + spawnDir * radius; // Radius is just the distance away from the point

            points[i] = spawnPos;
        }

        return points;
    } 
}
