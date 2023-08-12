using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleGrahamScanLastFirstCoroutine : MonoBehaviour {

    private List<City> cities;

    private List<LineRenderer> pathRenderers;
    private float lineWidth;

    public void Initialize(List<City> cities, float lineWidth) {
        this.cities = cities;
        this.lineWidth = lineWidth;

        pathRenderers = new List<LineRenderer>();
    }

    private LineRenderer CreateNewPathRenderer() {
        LineRenderer pathRenderer = new GameObject("Multiple Graham Scan Last First Coroutine").AddComponent<LineRenderer>();
        pathRenderer.startWidth = lineWidth;
        pathRenderer.endWidth = lineWidth;
        pathRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        pathRenderer.startColor = Color.black;
        pathRenderer.endColor = Color.black;

        return pathRenderer;
    }

    private void Start() {
        StartCoroutine(Solve());
    }

    private void CreateNewPath(List<City> points) {
        LineRenderer pathRenderer = CreateNewPathRenderer();

        pathRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
            pathRenderer.SetPosition(i, points[i].position);

        pathRenderers.Add(pathRenderer);
    }

    private void UpdatePath(int pathIndex, List<City> points) {

        if(points.Count == 0) {
            //pathRenderers.RemoveAt(pathIndex);
            pathRenderers[pathIndex].positionCount = 0;
        } else {
            LineRenderer pathRenderer = pathRenderers[pathIndex];

            pathRenderer.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
                pathRenderer.SetPosition(i, points[i].position);
        }
    }

    private IEnumerator Solve() {

        List<ConvexHull> convexHullList = new List<ConvexHull>();

        do {
            ConvexHull convexHull = ConvexHull.GrahamScan(cities);
            convexHullList.Add(convexHull);

            /*
            * DELETE POINTS FROM ORIGINAL LIST (BADDDD PERFORMANCE)
            * (obs this deletion in lists is horrendous in performance, but im just testing things out)
            * (obs² -1 is because last point goes back to first)
            */
            for (int i = 0; i < convexHull.points.Count - 1; i++)
                cities.Remove(convexHull.points[i]);

            CreateNewPath(convexHull.points);
        } while (cities.Count > 0);


        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.KeypadEnter));


        //WHITCH SET TO MERGE WITH THE FIRST (0) - LOOP
        for (int i = convexHullList.Count-1; i > 0; i--) {

            ConvexHull mainConvexHull = convexHullList[0];
            ConvexHull toBeMerged = convexHullList[i];


            City minChangedPoint = toBeMerged.points[0];
            int p1minIndex = 0;

            do {
                float minChangedDistance = float.MaxValue;

                foreach (var point in toBeMerged.points) {

                    for (int j = 0; j < mainConvexHull.points.Count - 1; j++) {

                        float changedDistance = mainConvexHull.SimulatePointInsertion(j, point);

                        if (changedDistance < minChangedDistance) {
                            minChangedPoint = point;
                            minChangedDistance = changedDistance;
                            p1minIndex = j;
                        }

                    }

                }

                toBeMerged.points.Remove(minChangedPoint);
                mainConvexHull.InsertPoint(p1minIndex, minChangedPoint);

                UpdatePath(0, mainConvexHull.points);
                UpdatePath(i, toBeMerged.points);
                //yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.KeypadEnter));

            } while (toBeMerged.points.Count > 0);
        }


        float distance = 0;

        var points = convexHullList[0].points;

        for (int i = 0; i < points.Count - 1; i++)
            distance += Vector2.Distance(points[i].position, points[i + 1].position);

        Debug.Log("Multiple Grahan Scan Last Fisrt path size: " + distance);
    }


    private class OrientationComparer : IComparer<City> {
        public City lowestCity;

        public OrientationComparer(City lowestCity) {
            this.lowestCity = lowestCity;
        }

        public int Compare(City a, City b) {
            int o = orientation(lowestCity.position, a.position, b.position);

            if (o == 0)
                return (Vector2.Distance(lowestCity.position, b.position) >= Vector2.Distance(lowestCity.position, a.position)) ? -1 : 1;

            return (o == 2) ? -1 : 1;
        }
    }

    // To find orientation of ordered triplet (p, q, r).
    // The function returns following values
    // 0 --> p, q and r are colinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    public static int orientation(Vector2 p, Vector2 q, Vector2 r) {
        float val = (q.y - p.y) * (r.x - q.x) -
                  (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0;  // colinear
        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

}
