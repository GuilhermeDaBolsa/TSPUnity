using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NearestNeighbour : TSP_VFX_Algorithm {

    protected override void Initializer() {
        base.SetUp("Nearest Neighbour", 0.2f, Color.magenta);
    }

    protected override List<City> Solve(List<City> cities) {

        for (int i = 1; i < cities.Count - 1; i++) {

            int lastVisitedCity = i - 1;

            int nearestCity = FindNearestNeighbour(lastVisitedCity, ref cities);
            
            Swap(ref cities, i, nearestCity);
        }

        cities.Add(cities[0]);

        return cities;
    }

    private int FindNearestNeighbour(int currentCity, ref List<City> cities) {

        int nearestNeighbour = currentCity + 1;
        float shortestDistance = Vector2.Distance(cities[currentCity].position, cities[nearestNeighbour].position);

        for(int i = nearestNeighbour + 1; i < cities.Count; i++) {

            float currentDistance = Vector2.Distance(cities[currentCity].position, cities[i].position);

            if (currentDistance < shortestDistance) {
                nearestNeighbour = i;
                shortestDistance = currentDistance;
            }
        }

        return nearestNeighbour;
    }
}
