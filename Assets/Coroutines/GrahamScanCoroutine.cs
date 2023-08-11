using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrahamScanCoroutine : MonoBehaviour {

    private List<City> cities;

    private LineRenderer pathRenderer;
    private float lineWidth;

    public void Initialize(List<City> cities, float lineWidth) {
        this.cities = cities;
        this.lineWidth = lineWidth;
    }

    private void Awake() {
        pathRenderer = new GameObject("Graham Scan Coroutine").AddComponent<LineRenderer>();
        pathRenderer.startWidth = lineWidth;
        pathRenderer.endWidth = lineWidth;
        pathRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        pathRenderer.startColor = Color.yellow;
        pathRenderer.endColor = Color.yellow;
    }

    private void Start() {
        StartCoroutine(Solve());
    }

    private void UpdatePath(List<City> points) {
        pathRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
            pathRenderer.SetPosition(i, points[i].position);
    }

    private IEnumerator Solve() {
        ConvexHull convexHull = ConvexHull.GrahamScan(cities);

        UpdatePath(convexHull.points);

        yield return new WaitForSeconds(6f);


        /*
        * DELETE POINTS FROM ORIGINAL LIST (BADDDD PERFORMANCE)
        * (obs this deletion in lists is horrendous in performance, but im just testing things out)
        * (obs² -1 is because last point goes back to first)
        */
        for (int i = 0; i < convexHull.points.Count - 1; i++)
            cities.Remove(convexHull.points[i]);


        City minChangedPoint = cities[0];
        int p1minIndex = 0;

        do {
            float minChangedDistance = float.MaxValue;

            foreach (var point in cities) {

                for (int i = 0; i < convexHull.points.Count - 1; i++) {

                    float changedDistance = convexHull.SimulatePointInsertion(i, point);

                    if (changedDistance < minChangedDistance) {
                        minChangedPoint = point;
                        minChangedDistance = changedDistance;
                        p1minIndex = i;
                    }

                }

            }

            cities.Remove(minChangedPoint);
            convexHull.InsertPoint(p1minIndex, minChangedPoint);

            UpdatePath(convexHull.points);
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.KeypadEnter));

        } while (cities.Count > 0);

        float distance = 0;

        var points = convexHull.points;

        for (int i = 0; i < points.Count - 1; i++)
            distance += Vector2.Distance(points[i].position, points[i + 1].position);

        Debug.Log("Grahan Scan path size: " + distance);
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
