using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MAIN : MonoBehaviour {

    public GameObject CityPrefab;
    public GameObject FirstCityPrefab;

    private LineRenderer BestPath;

    public List<TSP_VFX_Algorithm> TSP_solvers; //TODO (DEVERIA SER ALGO EXTERNO?)


    void Start() {

        /*
         * TSPs available
         * ulysses8, ulysses16, crs5, cirs6, cirs8, att48
         */

        TSP problem = new TSP("Assets/Tests/ulysses16.tsp");

        SpawnCities(problem.m_Cities);

        if(problem.m_BestTourIndexes != null) {
            RenderBestPath(problem.m_Cities, problem.m_BestTourIndexes);
        }

        SpawnTSPSolvers(problem);
    }

    private void SpawnTSPSolvers(TSP problem) {
        TSP_solvers = new List<TSP_VFX_Algorithm> {
            //this.AddComponent<NearestNeighbour>(),
            //this.AddComponent<AntColony>(),
            //this.AddComponent<BranchAndBound>(),
            //this.AddComponent<BranchAndBoundGFG>(),
            this.AddComponent<ConvexHullGrahamScan>(),
        };

        foreach (var algorithm in TSP_solvers) {
            algorithm.SolveAndFeedback(problem.m_Cities);
        }
    }

    private void SpawnCities(List<City> cities) {
        Instantiate(this.FirstCityPrefab, cities[0].position, Quaternion.identity);

        for (int i = 1; i < cities.Count; i++) {
            Instantiate(this.CityPrefab, cities[i].position, Quaternion.identity);
        }
    }

    private void RenderBestPath(List<City> path, List<int> bestPathIndexes) {
        BestPath = CreateLineRenderer("BestPath", 0.2f, Color.white);
        BestPath.positionCount = bestPathIndexes.Count;

        for (int i = 0; i < bestPathIndexes.Count; i++)
            BestPath.SetPosition(i, path[bestPathIndexes[i]].position);
    }

    private LineRenderer CreateLineRenderer(string objName, float lineWidth, Color lineColor) {
        LineRenderer lineRenderer = new GameObject(objName).AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        return lineRenderer;
    }
}
