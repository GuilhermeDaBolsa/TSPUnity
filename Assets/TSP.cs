using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class TSP {

    private string m_filePath;
    private string m_fileName;

    public List<int> m_BestTourIndexes;
    public float m_BestTourDistance;

    public List<City> m_Cities;


    public TSP(string tspFilePath) {
        m_filePath = tspFilePath;
        m_fileName = tspFilePath.Substring(tspFilePath.LastIndexOf('/') + 1);

        ImportTSPFromFile(tspFilePath);
        CalculateBestTourDistance(m_BestTourIndexes);
        Debug.Log("Problem " + m_fileName + " loaded! Best tour distance is " + m_BestTourDistance);
    }

    private void ImportTSPFromFile(string filePath) {

        //READ FILE LINES
        string[] lines = File.ReadAllLines(filePath);


        //FILE LINE POSITIONS
        int lineOfTspSize = 0;
        int lineOfBestTour = 1;
        int lineOfCities = 2;


        //BEST TOUR DATA
        string[] bestTourIndexes = lines[lineOfBestTour].Split(' ');

        if(bestTourIndexes.Length > 0 ) {
            m_BestTourIndexes = new List<int>(bestTourIndexes.Length);

            for (int i = 0; i < bestTourIndexes.Length; i++) {
                m_BestTourIndexes.Add(Int32.Parse(bestTourIndexes[i]));
            }
        }


        //CITIES DATA
        int numberOfCities = Int32.Parse(lines[lineOfTspSize]);
        m_Cities = new List<City>(numberOfCities);

        for (int i = lineOfCities; i < lines.Length; i++) {
            string[] positionValues = lines[i].Split(' ');

            m_Cities.Add(
                new City(
                    float.Parse(positionValues[0], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(positionValues[1], CultureInfo.InvariantCulture.NumberFormat),
                    i - lineOfCities
                )
            );
        }
    }

    private void CalculateBestTourDistance(List<int> bestTourIndexes) {
        m_BestTourDistance = 0;

        for(int i = 0; i < bestTourIndexes.Count - 1; i++) {
            m_BestTourDistance += Vector2.Distance(
                m_Cities[bestTourIndexes[i]].position,
                m_Cities[bestTourIndexes[i + 1]].position);
        }
    }
}