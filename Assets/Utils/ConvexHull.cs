using System.Collections.Generic;
using UnityEngine;

public class ConvexHull {
    public List<City> points;
    public List<float> edges;
    public float perimeter;

    public ConvexHull() {
        points = new List<City>();
        edges = new List<float>();
        perimeter = 0;
    }

    public ConvexHull(List<City> points) {
        this.SetPoints(points);
    }

    public void SetPoints(List<City> points) {
        this.points = points;
        CalculateEdgesAndPerimeter();
    }

    private void CalculateEdgesAndPerimeter() {
        edges = new List<float>();
        perimeter = 0;

        for (int i = 0; i < points.Count - 1; i++) {

            var nthEdge = Vector2.Distance(points[i].position, points[i + 1].position);

            edges.Add(nthEdge);
            perimeter += nthEdge;
        }
    }

    public float SimulatePointInsertion(int insertAfterIndex, City toInsert) {
        float originalEdgeLength = edges[insertAfterIndex];

        float newPathLength = Vector2.Distance(points[insertAfterIndex].position, toInsert.position) +
                                Vector2.Distance(toInsert.position, points[insertAfterIndex + 1].position);

        return newPathLength - originalEdgeLength;
    }

    public void InsertPoint(int insertAfterIndex, City toInsert) {
        this.points.Insert(insertAfterIndex+1, toInsert);

        float originalEdgeLength = edges[insertAfterIndex];

        float newEdge1Length = Vector2.Distance(points[insertAfterIndex].position, points[insertAfterIndex + 1].position);
        float newEdge2Length = Vector2.Distance(points[insertAfterIndex + 1].position, points[insertAfterIndex + 2].position);

        this.edges[insertAfterIndex] = newEdge1Length;
        this.edges.Insert(insertAfterIndex + 1, newEdge2Length);

        this.perimeter += newEdge1Length + newEdge2Length - originalEdgeLength;
    }




    //-- STATIC METHODS --//

    public static ConvexHull GrahamScan(List<City> cities) {
        List<City> convexHullPoints = new List<City>();

        if (cities.Count < 3) {
            convexHullPoints.AddRange(cities);
            convexHullPoints.Add(convexHullPoints[0]);
            return new ConvexHull(convexHullPoints);
        }

        int lowestCityIndex = 0;

        /*
         * FIND LOWEST Y POINT (LEFTMOST ALSO)
         */
        for(int i = 1; i < cities.Count; i++) {
            if (cities[i].position.y < cities[lowestCityIndex].position.y ||
                (
                    cities[i].position.y == cities[lowestCityIndex].position.y &&
                    cities[i].position.x < cities[lowestCityIndex].position.x
                ))
                lowestCityIndex = i;
        }

        Swap(ref cities, 0, lowestCityIndex);


        /*
         * SORT CITIES BY ANGLE (ITS MORE ABOUT 3 POINTS ORIENTATION)
         */
        cities.Sort(1, cities.Count - 1, new OrientationComparer(cities[0]));

        /*
         * FILTER SORTED LIST TO FIND CONVEX POINTS ONLY
         */
        convexHullPoints.Add(cities[0]);
        convexHullPoints.Add(cities[1]);
        convexHullPoints.Add(cities[2]);

        for (int i = 3; i < cities.Count; i++) {
            while (
                convexHullPoints.Count > 1
                &&
                orientation(
                    convexHullPoints[convexHullPoints.Count - 2].position,
                    convexHullPoints[convexHullPoints.Count - 1].position,
                    cities[i].position
                ) != 2
            ) {
                convexHullPoints.RemoveAt(convexHullPoints.Count - 1);
            }

            convexHullPoints.Add(cities[i]);
        }

        //CLOSES THE SHAPE
        convexHullPoints.Add(convexHullPoints[0]);

        return new ConvexHull(convexHullPoints);
    }


    public class OrientationComparer : IComparer<City> {
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

    public static void Swap<T>(ref List<T> list, int indexA, int indexB) {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
}
