using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntColony : TSP_VFX_Algorithm {

    float[,] distancesBetweenCities;
    float[,] pheromoneBetweenCities;

    float pheromoneIncrement = 6;
    float pheromoneDecrement = 0.5f;

    protected override void Initializer() {
        base.SetUp("Ant Colony", 0.2f, Color.red);
    }

    protected override List<City> Solve(List<City> cities) {

        List<City> bestPath = new List<City>(cities.Count);

        float bestPathFoundDistance = float.MaxValue;

        InitializeBetweenCitiesMatrixes(ref cities);

        for (int i = 0; i < 400; i++) {

            Ant[] explorers = new Ant[30];

            for(int f = 0; f < 30; f++) {
                explorers[f] = new Ant(ref cities);
                explorers[f].TraverseCities(ref distancesBetweenCities, ref pheromoneBetweenCities);

                float pathDistance = CalculatePathDistance(ref explorers[f].cities);

                if(pathDistance < bestPathFoundDistance) {
                    bestPathFoundDistance = pathDistance;
                    bestPath = explorers[f].cities;
                }
            }

            UpdatePheromoneTrail(ref cities, ref bestPath);
        }

        return bestPath;
    }

    private void InitializeBetweenCitiesMatrixes(ref List<City> cities) {
        distancesBetweenCities = new float[cities.Count, cities.Count];
        pheromoneBetweenCities = new float[cities.Count, cities.Count];

        for (int i = 0; i < cities.Count; i++) {
            for(int j = i; j < cities.Count; j++) {

                float distance = Vector2.Distance(cities[i].position, cities[j].position);

                distancesBetweenCities[i, j] = distance;
                distancesBetweenCities[j, i] = distance;

                pheromoneBetweenCities[i, j] = 1;
                pheromoneBetweenCities[j, i] = 1;
            }
        }
    }

    private void UpdatePheromoneTrail(ref List<City> cities, ref List<City> bestPath) {

        for (int i = 0; i < cities.Count; i++) {
            for (int j = i; j < cities.Count; j++) {
                pheromoneBetweenCities[i, j] *= pheromoneDecrement;
                pheromoneBetweenCities[j, i] *= pheromoneDecrement;
            }
        }

        for (int i = 0; i < bestPath.Count - 1; i++) {
            pheromoneBetweenCities[bestPath[i].index, bestPath[i + 1].index] += pheromoneIncrement;
            pheromoneBetweenCities[bestPath[i + 1].index, bestPath[i].index] += pheromoneIncrement;
        }

    }

    private float CalculatePathDistance(ref List<City> path) {
        float distance = 0;

        for (int i = 0; i < path.Count - 1; i++)
            distance += Vector2.Distance(path[i].position, path[i + 1].position);

        return distance;
    }


    private class Ant {

        public List<City> cities;

        int pheromoneExponent = 1;
        int distanceExponent = 3;

        public Ant(ref List<City> cities) {
            this.cities = new List<City>(cities);
        }

        public void TraverseCities(ref float[,] distancesBetweenCities, ref float[,] pheromoneBetweenCities) {
            
            int startCityIndex = Random.Range(0, cities.Count);
            Swap(ref cities, 0, startCityIndex);

            for(int i = 1; i < cities.Count - 1; i++) {
                Swap(ref cities, i, ChoseNextCity(i - 1, ref distancesBetweenCities, ref pheromoneBetweenCities));
            }

            cities.Add(cities[0]);
        }

        private int ChoseNextCity(int currentCity, ref float[,] distancesBetweenCities, ref float[,] pheromoneBetweenCities) {

            //CALCULATE THE SUM OF THE DESIRE TO GO TO EACH CITY
            float totalDesires = 0;

            for(int i = currentCity + 1; i < cities.Count; i++) {
                totalDesires += DesireToCity(pheromoneBetweenCities[currentCity, i], distancesBetweenCities[currentCity, i]);
            }

            //CALCULATE HOW MUCH EACH CITY CONTRIBUTES TO THE SUM CALCULATED ABOVE (% OF EACH CITY OF TOTAL)
            float[] probabilities = new float[cities.Count - (currentCity + 1)];

            for (int i = 0; i < probabilities.Length; i++) {
                probabilities[i] =
                    DesireToCity(pheromoneBetweenCities[currentCity, i + currentCity + 1], distancesBetweenCities[currentCity, i + currentCity + 1]) 
                    / totalDesires;
            }

            //GENERATE RANDOM VALUE AND CALCULATE WITCH CITY WAS LUCKY
            float luckyNumber = Random.value;
            float acumulator = 0;

            for(int i = 0; i < probabilities.Length; i++) {
                acumulator += probabilities[i];

                if(acumulator > luckyNumber) {
                    return i + currentCity + 1;
                }
            }

            //NOT SUPPOSED TO REACH HERE
            return 0;
        }

        private float DesireToCity(float edgePheromone, float edgeDistance) {
            return Mathf.Pow(edgePheromone, pheromoneExponent) *
                    Mathf.Pow(1 / edgeDistance, distanceExponent);
        }
    }
}
