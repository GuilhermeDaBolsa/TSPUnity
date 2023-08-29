using Graph;
using System.Collections.Generic;

public class MST{

    public static int[] PrimsBasic(float[,] costMatrix) {

        int numOfVertices = costMatrix.GetLength(0);

        int[] MSTparentArray = new int[numOfVertices];
        bool[] visited = new bool[numOfVertices];
        float[] costToVertice = new float[numOfVertices];

        for(int i = 0; i < numOfVertices; i++) {
            visited[i] = false;
            costToVertice[i] = float.MaxValue;
        }

        costToVertice[0] = 0;
        MSTparentArray[0] = -1;

        for(int i = 0; i < numOfVertices - 1; i++) { // -1 ???

            int chosenV = FindCheapestVerticeNotYetVisited(visited, costToVertice);

            visited[chosenV] = true;

            for (int v = 0; v < costToVertice.Length; v++) {

                if (!visited[v] && costMatrix[chosenV, v] < costToVertice[v] && costMatrix[chosenV, v] != 0) {
                    costToVertice[v] = costMatrix[chosenV, v];
                    MSTparentArray[v] = chosenV;
                }
            }

        }

        return MSTparentArray;
    }


    private static int FindCheapestVerticeNotYetVisited(bool[] visited, float[] costToVertice) {
        
        int verticeIndex = -1;
        float minCost = float.MaxValue;

        for(int i = 0; i < costToVertice.Length; i++) {

            if (!visited[i] && costToVertice[i] < minCost) {
                verticeIndex = i;
                minCost = costToVertice[i];
            }

        }

        return verticeIndex;
    }


    /*
     * FROM https://github.com/mikymaione/Held-Karp-algorithm
     */
    public static void PrimsGraphed(float[,] distance, Graph.Graph G, int r_id) {

        HashSet<int> S = new HashSet<int>(); // elements available

        // sorted min queue
        SortedSet<Node> Q = new SortedSet<Node>(Comparer<Node>.Create((lhs, rhs) => {
            if (lhs.key < rhs.key)
                return -1;
            if (lhs.key > rhs.key)
                return 1;

            return 0;
        }));

        foreach (var u in G.V) {
            u.key = float.MaxValue;
            u.π = null;

            S.Add(u.id);
        }

        var r = G.NodeById(r_id);
        r.key = 0;
        Q.Add(r);

        while (Q.Count > 0) {
            var u = Q.Min; // min
            Q.Remove(u);
            S.Remove(u.id);

            foreach (var v in G.Adj[u])
                if (u.id != v.id)
                    if (S.Contains(v.id) && distance[u.id, v.id] < v.key) {
                        Q.Remove(v);

                        v.π = u;
                        v.key = distance[u.id, v.id];

                        Q.Add(v);
                    }
        }
	}
}
