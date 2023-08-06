using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MAIN : MonoBehaviour {

    public GameObject CityPrefab;
    public GameObject FirstCityPrefab;

    public List<TSP_VFX_Algorithm> TSP_solvers; //TODO (DEVERIA SER ALGO EXTERNO?)


    void Start() {
        LineRenderer pathRenderer;
        pathRenderer = new GameObject("BestPath").AddComponent<LineRenderer>();
        pathRenderer.startWidth = 0.2f;
        pathRenderer.endWidth = 0.2f;
        pathRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        pathRenderer.startColor = Color.white;
        pathRenderer.endColor = Color.white;

        TSP ulysses16 = new TSP("Assets/Tests/ulysses16.tsp");
        Debug.Log("Problem ulysses16 loaded! Best tour distance is " + ulysses16.m_BestTourDistance);

        /*TSP ulysses8 = new TSP("Assets/Tests/ulysses8.tsp");
        Debug.Log("Problem ulysses8 loaded! Best tour distance is " + ulysses8.m_BestTourDistance);*/

        /*TSP crs5 = new TSP("Assets/Tests/crs5.tsp");
        Debug.Log("Problem crs5 loaded! Best tour distance is " + crs5.m_BestTourDistance);*/

        /*TSP cirs6 = new TSP("Assets/Tests/cirs6.tsp");
        Debug.Log("Problem cirs6 loaded! Best tour distance is " + cirs6.m_BestTourDistance);*/

        /*TSP cirs8 = new TSP("Assets/Tests/cirs8.tsp");
        Debug.Log("Problem cirs8 loaded! Best tour distance is " + cirs8.m_BestTourDistance);*/

        /*TSP att48 = new TSP("Assets/Tests/att48.tsp");
        Debug.Log("Problem att48 loaded! Best tour distance is " + att48.m_BestTourDistance);*/

        TSP_solvers = new List<TSP_VFX_Algorithm> {
            //this.AddComponent<NearestNeighbour>(),
            //this.AddComponent<AntColony>(),
            //this.AddComponent<BranchAndBound>(),
            //this.AddComponent<BranchAndBoundGFG>(),
            this.AddComponent<ConvexHullGrahamScan>(),
        };

        SpawnCities(ref ulysses16.m_Cities);

        RenderBestPath(ref ulysses16.m_Cities, ref ulysses16.m_BestTourIndexes, ref pathRenderer);

        foreach (var algorithm in TSP_solvers) {
            algorithm.SolveAndFeedback(ulysses16.m_Cities);
        }
    }

    private void SpawnCities(ref List<City> cities) {
        Instantiate(this.FirstCityPrefab, cities[0].position, Quaternion.identity);

        for (int i = 1; i < cities.Count; i++) {
            Instantiate(this.CityPrefab, cities[i].position, Quaternion.identity);
        }
    }

    private void RenderBestPath(ref List<City> path, ref List<int> bestPathIndexes, ref LineRenderer pathRenderer) {
        pathRenderer.positionCount = bestPathIndexes.Count;

        for (int i = 0; i < bestPathIndexes.Count; i++)
            pathRenderer.SetPosition(i, path[bestPathIndexes[i]].position);
    }
}
