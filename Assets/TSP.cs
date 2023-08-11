using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class TSP {

    //-- Properties from TSPLib --//
    public string m_Name;
    public string m_Comment;
    public string m_Type;
    public int m_Size;


    private string m_filePath;
    private string m_fileName;

    public List<int> m_BestTourIndexes;
    public float m_BestTourDistance;

    public List<City> m_Cities;


    public TSP() {
        m_Cities = new List<City>();
        m_BestTourIndexes = new List<int>();
    }

    public TSP(string tspFilePath) {
        m_filePath = tspFilePath;
        m_fileName = tspFilePath.Substring(tspFilePath.LastIndexOf('/') + 1);

        ImportTSPFromFile(tspFilePath);

        ConcludeImporting();
    }

    public void ConcludeImporting() {
        CalculateBestTourDistance();
        Debug.Log("Problem " + m_Name + " loaded! Best tour distance is " + m_BestTourDistance);
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

    private void CalculateBestTourDistance() {
        m_BestTourDistance = 0;

        for(int i = 0; i < m_BestTourIndexes.Count - 1; i++) {
            m_BestTourDistance += Vector2.Distance(
                m_Cities[m_BestTourIndexes[i]].position,
                m_Cities[m_BestTourIndexes[i + 1]].position);
        }
    }
}