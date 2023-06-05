using UnityEngine;

/*
 * position is the city's position in an Euclidian plane
 * index is the city's index in the original cities list
 */

public struct City {
    public Vector2 position;
    public int index;

    public City(Vector2 position, int index) {
        this.position = position;
        this.index = index;
    }

    public City(float x, float y, int index) {
        this.position = new Vector2(x, y);
        this.index = index;
    }
}
