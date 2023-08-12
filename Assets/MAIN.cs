using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MAIN : MonoBehaviour {

    public Camera mainCamera;

    public GameObject CityPrefab;
    public GameObject FirstCityPrefab;

    private LineRenderer BestPath;

    public List<TSP_VFX_Algorithm> TSP_solvers; //TODO (DEVERIA SER ALGO EXTERNO?)
    private GrahamScanCoroutine grahamScanCoroutine;
    private MultipleGrahamScanCoroutine multipleGrahamScanCoroutine;
    private MultipleGrahamScanLastFirstCoroutine multipleGrahamScanLastFirstCoroutine;

    void Start() {

        /*
         * From Assets/Tests/Mine/ folder:
         * 
         * --- Confirmed opt. tour ---
         * crs5, cirs6, cirs8, ulysses8
         * 
         * 
         * 
         * From Assets/Tests/TSPLib/ folder:
         * 
         * --- Confirmed opt. tour ---
         * ulysses16
         * 
         * --- Not confirmed opt. tour ---
         * ulysses22, att48, eil51, berlin52, ch130, a280
         * 
         * --- Does not have opt. tour ---
         * burma14, bier127, att532, ali535, brd14051
         */

        TSP problem = TSPLib.Import("Assets/Tests/Mine/", "ulysses8");

        SpawnCities(problem.m_Cities);


        float[] externalValues = new float[4] { 
            problem.m_Cities[0].position.y,
            problem.m_Cities[0].position.x,
            problem.m_Cities[0].position.y,
            problem.m_Cities[0].position.x 
        };
        
        foreach(var city in problem.m_Cities) {
            if (city.position.y > externalValues[0])
                externalValues[0] = city.position.y;

            if (city.position.y < externalValues[2])
                externalValues[2] = city.position.y;

            if (city.position.x > externalValues[1])
                externalValues[1] = city.position.x;

            if (city.position.x < externalValues[3])
                externalValues[3] = city.position.x;
        }

        mainCamera.transform.position = new Vector3(
            (externalValues[1] + externalValues[3]) / 2,
            (externalValues[0] + externalValues[2]) / 2,
            -10
        );

        float xDistance = externalValues[1] - externalValues[3];
        float yDistance = externalValues[0] - externalValues[2];

        float greaterDistance = xDistance > yDistance ? xDistance : yDistance;

        float lineWidth = greaterDistance / 100 + 0.2f;



        if (problem.m_BestTourIndexes != null) {
            RenderBestPath(problem.m_Cities, problem.m_BestTourIndexes, lineWidth/2);
        }

        SpawnTSPSolvers(problem, lineWidth);
    }

    private void SpawnTSPSolvers(TSP problem, float lineWidth) {
        /*TSP_solvers = new List<TSP_VFX_Algorithm> {
            //this.AddComponent<NearestNeighbour>(),
            //this.AddComponent<AntColony>(),
            //this.AddComponent<BranchAndBound>(),
            //this.AddComponent<BranchAndBoundGFG>(),
            this.AddComponent<ConvexHullGrahamScan>(),
        };

        foreach (var algorithm in TSP_solvers) {
            algorithm.SolveAndFeedback(problem.m_Cities);
        }*/

        grahamScanCoroutine = this.AddComponent<GrahamScanCoroutine>();
        grahamScanCoroutine.Initialize(new List<City>(problem.m_Cities), lineWidth);

        multipleGrahamScanCoroutine = this.AddComponent<MultipleGrahamScanCoroutine>();
        multipleGrahamScanCoroutine.Initialize(new List<City>(problem.m_Cities), lineWidth);

        multipleGrahamScanLastFirstCoroutine = this.AddComponent<MultipleGrahamScanLastFirstCoroutine>();
        multipleGrahamScanLastFirstCoroutine.Initialize(new List<City>(problem.m_Cities), lineWidth);
    }

    private void SpawnCities(List<City> cities) {
        Instantiate(this.FirstCityPrefab, cities[0].position, Quaternion.identity);

        for (int i = 1; i < cities.Count; i++) {
            Instantiate(this.CityPrefab, cities[i].position, Quaternion.identity);
        }
    }

    private void RenderBestPath(List<City> path, List<int> bestPathIndexes, float lineWidth) {
        BestPath = CreateLineRenderer("BestPath", lineWidth, new Color(1,1,1,0.5f));
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
