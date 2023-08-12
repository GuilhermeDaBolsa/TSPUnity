using System;
using System.Collections.Generic;
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

    public void ConcludeImporting() {
        CalculateBestTourDistance();
        Debug.Log("Problem " + m_Name + " loaded! Best tour distance is " + m_BestTourDistance);
    }

    private void CalculateBestTourDistance() {
        m_BestTourDistance = 0;

        for (int i = 0; i < m_BestTourIndexes.Count - 1; i++) {
            m_BestTourDistance += Vector2.Distance(
                m_Cities[m_BestTourIndexes[i]].position,
                m_Cities[m_BestTourIndexes[i + 1]].position);
        }
    }
}