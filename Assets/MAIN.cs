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
    private ConvexHullCoroutine grahamScanCoroutine;
    private MultipleConvexHullCoroutine multipleGrahamScanCoroutine;
    private MultipleConvexHullLastFirstCoroutine multipleGrahamScanLastFirstCoroutine;
    private ChristofidesCoroutine christofidesCoroutine;

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
         * 
         * OBS: ALL ULYSSES ARE 'GEO' TYPE OF EDGES NOT 'EUCL_2D'... PROB. THATS WHY I THOUGHT THE OPT TOUR WAS WRONG
         */

        TSP problem = TSPLib.Import("Assets/Tests/TSPLib/", "ulysses16");

        SpawnCities(problem.m_Cities);

        problem.CalculateExternalBounds();

        mainCamera.transform.position = new Vector3(
            (problem.leftMostValue + problem.rightMostValue) / 2,
            (problem.topMostValue + problem.bottomMostValue) / 2,
            -10
        );

        float xDistance = problem.rightMostValue - problem.leftMostValue;
        float yDistance = problem.topMostValue - problem.bottomMostValue;

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

        //grahamScanCoroutine = this.AddComponent<ConvexHullCoroutine>();
        //grahamScanCoroutine.Initialize(new List<City>(problem.m_Cities), lineWidth);

        //multipleGrahamScanCoroutine = this.AddComponent<MultipleConvexHullCoroutine>();
        //multipleGrahamScanCoroutine.Initialize(new List<City>(problem.m_Cities), lineWidth);

        //multipleGrahamScanLastFirstCoroutine = this.AddComponent<MultipleConvexHullLastFirstCoroutine>();
        //multipleGrahamScanLastFirstCoroutine.Initialize(new List<City>(problem.m_Cities), lineWidth);

        christofidesCoroutine = this.AddComponent<ChristofidesCoroutine>();
        christofidesCoroutine.Initialize(new List<City>(problem.m_Cities), lineWidth);
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

        BestPath.enabled = false;
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
