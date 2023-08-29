using System.Collections.Generic;

public class TSP {

    //-- Properties from TSPLib --//
    public string m_Name;
    public string m_Comment;
    public string m_Type;
    public int m_Size;


    public List<City> m_Cities;

    public List<int> m_BestTourIndexes;
    public float m_BestTourDistance;

    public float topMostValue;
    public float bottomMostValue;
    public float leftMostValue;
    public float rightMostValue;


    public TSP() {
        m_Cities = new List<City>();
        m_BestTourIndexes = new List<int>();
    }

    public void CalculateExternalBounds() {
        topMostValue = m_Cities[0].position.y;
        bottomMostValue = m_Cities[0].position.y;
        leftMostValue = m_Cities[0].position.x;
        rightMostValue = m_Cities[0].position.x;

        for (int i = 1; i < m_Cities.Count; i++) {
            var city = m_Cities[i];

            if (city.position.y > topMostValue)
                topMostValue = city.position.y;
            else if(city.position.y < bottomMostValue)
                bottomMostValue = city.position.y;            

            if (city.position.x > leftMostValue)
                leftMostValue = city.position.x;
            else if (city.position.x < rightMostValue)
                rightMostValue = city.position.x;
        }
    }
}