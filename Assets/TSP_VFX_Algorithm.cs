using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class TSP_VFX_Algorithm : MonoBehaviour {

    public string name;

    public Stopwatch timer;
    public List<City> bestPathFound;
    public float bestPathFoundDistance;

    private GameObject objectInScene;
    private LineRenderer pathRenderer;



    protected void SetUp(string name, float pathWidth, Color pathColor) {
        this.name = name;
        timer = new Stopwatch();
        objectInScene = new GameObject(name);

        pathRenderer = objectInScene.AddComponent<LineRenderer>();
        pathRenderer.startWidth = pathWidth;
        pathRenderer.endWidth = pathWidth;
        pathRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        pathRenderer.startColor = pathColor;
        pathRenderer.endColor = pathColor;
    }


    private void UpdateVisualPath(ref List<City> newPath) {
        pathRenderer.positionCount = newPath.Count;

        for (int i = 0; i < newPath.Count; i++)
            pathRenderer.SetPosition(i, newPath[i].position);
    }


    protected abstract void Initializer();

    protected abstract List<City> Solve(List<City> cities);


    private void Awake() {
        Initializer();
    }

    public void SolveAndFeedback(List<City> cities) {

        List<City> copieOfCities = new List<City>(cities);

        timer.Start();

        bestPathFound = Solve(copieOfCities);

        timer.Stop();

        UpdateVisualPath(ref bestPathFound);

        bestPathFoundDistance = CalculatePathDistance(ref bestPathFound);

        UnityEngine.Debug.Log(name + " took " + timer.ElapsedMilliseconds + " ms to solve and the size of the path found is " + bestPathFoundDistance);
    }


    // STATIC TSP HELPERS //

    protected static void Swap<T>(ref List<T> list, int indexA, int indexB) {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    //THERE IS A LOT OF COPIES OF THIS METHOD HERE =(
    protected static float CalculatePathDistance(ref List<City> path) {
        float distance = 0;

        for (int i = 0; i < path.Count - 1; i++)
            distance += Vector2.Distance(path[i].position, path[i + 1].position);

        return distance;
    }
}
