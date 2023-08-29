using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class ChristofidesCoroutine : MonoBehaviour {

    private List<City> cities;

    private GameObject mst;
    private List<LineRenderer> pathRenderers;
    private float lineWidth;

    public void Initialize(List<City> cities, float lineWidth) {
        this.cities = cities;
        this.lineWidth = lineWidth;

        pathRenderers = new List<LineRenderer>();
    }

    private LineRenderer CreateNewPathRenderer() {
        LineRenderer pathRenderer = new GameObject("Multiple Graham Scan Coroutine").AddComponent<LineRenderer>();
        pathRenderer.startWidth = lineWidth;
        pathRenderer.endWidth = lineWidth;
        pathRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        pathRenderer.startColor = Color.black;
        pathRenderer.endColor = Color.black;

        return pathRenderer;
    }

    private void CreateMSTpath(int[] mstParents) {
        this.mst = new GameObject("Multiple Spanning Tree");

        for(int i = 1; i < mstParents.Length; i++) {
            GameObject mstPart = new GameObject("MST - " + i);
            LineRenderer pathRenderer = mstPart.AddComponent<LineRenderer>();

            pathRenderer.startWidth = lineWidth;
            pathRenderer.endWidth = lineWidth;
            pathRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            pathRenderer.startColor = Color.black;
            pathRenderer.endColor = Color.black;

            pathRenderer.positionCount = 2;

            pathRenderer.SetPosition(0, this.cities[mstParents[i]].position);
            pathRenderer.SetPosition(1, this.cities[i].position);

            mstPart.transform.SetParent(mst.transform);
        }
    }

    private void CreateMSTpathFromGraphed(List<HashSet<int>> mstGraphed) {
        this.mst = new GameObject("Multiple Spanning Tree");

        for(int i = 0; i < mstGraphed.Count; i++) {
            foreach(var j in mstGraphed[i]) {
                GameObject mstPart = new GameObject("MST - " + i);
                LineRenderer pathRenderer = mstPart.AddComponent<LineRenderer>();

                pathRenderer.startWidth = lineWidth;
                pathRenderer.endWidth = lineWidth;
                pathRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                pathRenderer.startColor = Color.black;
                pathRenderer.endColor = Color.black;

                pathRenderer.positionCount = 2;

                pathRenderer.SetPosition(0, this.cities[i].position);
                pathRenderer.SetPosition(1, this.cities[j].position);

                mstPart.transform.SetParent(mst.transform);
            }
        }
    }

    private void CreateChristofidesPath(List<int> circuit) {
        LineRenderer pathRenderer = CreateNewPathRenderer();

        pathRenderer.positionCount = circuit.Count;

        for (int i = 0; i < circuit.Count; i++)
            pathRenderer.SetPosition(i, cities[circuit[i]].position);

        pathRenderers.Add(pathRenderer);
    }

    private void Start() {
        StartCoroutine(Solve());
    }

    private IEnumerator Solve() {
        // MOST CODE (SPECIALLY THE GRAPH AND BLOSSOM/MATCHING THINGS)
        // IS FROM https://github.com/mikymaione/Held-Karp-algorithm/blob/master/Held-Karp-algorithm/Held-Karp-algorithm


        //-- CALCULATE COST MATRIX AND MAKE A GRAPH OF IT --//

        float[,] costMatrix = new float[cities.Count, cities.Count];

        for (int i = 0; i < cities.Count; i++) {
            for(int j = 0; j  < cities.Count; j++) {
                costMatrix[i, j] = Vector2.Distance(cities[i].position, cities[j].position);
            }
        }

        Graph.Graph G = new Graph.Graph(costMatrix);

        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.KeypadEnter));

        

        //-- CALCULATE MINIMUM SPANNING TREE (MST) --//

        /*int[] mst = MST.PrimsBasic(costMatrix);

        CreateMSTpath(mst);*/

        List<HashSet<int>> mst = new List<HashSet<int>>(cities.Count);
        for (int i = 0; i < cities.Count; i++) {
            mst.Add(new HashSet<int>());
        }

        MST.PrimsGraphed(costMatrix, G, 0);

        foreach (var u in G.V) {
            if (u.π != null) {
                mst[u.id].Add(u.π.id);
                mst[u.π.id].Add(u.id);
            }
        }

        //CreateMSTpathFromGraphed(mst);

        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.KeypadEnter));



        //-- ODD VERICES FROM MST --//

        HashSet<int> oddVertices = new HashSet<int>(cities.Count);

        for(int i = 0; i < mst.Count; i++) {
            if (mst[i].Count % 2 != 0)
                oddVertices.Add(i);
        }



        //-- INDUCED SUB GRAPH FROM ODD VERTICES --//

        Graph.Graph IG = new Graph.Graph(oddVertices);
        IG.MakeConnected(costMatrix);



        //-- PERFECT MATCHING (BLOSSOM) --//

        BlossomMatching blossom = new BlossomMatching();
        var M = blossom.Solve(G);



        //-- COMBINE (multigraph) --//

        var H = mst; //copy (TODO MAYBE ITS NOT WORKING)

        foreach (var m in M) {
            if (!H[m.from.id].Contains(m.to.id))
                H[m.from.id].Add(m.to.id);

            if (!H[m.to.id].Contains(m.from.id))
                H[m.to.id].Add(m.from.id);
        }



        //-- SHORTCUT (make a eulerian circuit) --//

        List<int> E = new List<int>();
        HashSet<int> visited = new HashSet<int>();

        RecursiveHamiltonian(H, E, visited, 0);

        E.Add(0);



        // TODO SHOW PATH

        CreateChristofidesPath(E);


        //-- CALCULATE COST OF PATH --//
        int da, a;
        float cost = 0;

        for (int i = 0; i < cities.Count; i++) {
            da = E[i];
            a = E[i + 1];

            cost += costMatrix[da, a];
        }

        Debug.Log("Christofides path size: " + cost);
    }

    public void RecursiveHamiltonian(List<HashSet<int>> H, List<int> E, HashSet<int> visited, int c) {
        visited.Add(c);
        E.Add(c);

        foreach (var e in H[c])
            if (!visited.Contains(e))
                RecursiveHamiltonian(H, E, visited, e);
    }
}