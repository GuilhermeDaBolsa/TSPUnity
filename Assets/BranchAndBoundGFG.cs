using System;
using System.Collections.Generic;
using UnityEngine;

/* THIS CODE IS FROM geeksforgeeks.org (Rohit Pradhan) 
 * at the bottom there is an explanation of what i think this code does */

public class BranchAndBoundGFG : TSP_VFX_Algorithm {

    static int N;
    static int[] final_path;
    static bool[] visited;
    static float final_res;

    static void copyToFinal(int[] curr_path) {
        for (int i = 0; i < N; i++)
            final_path[i] = curr_path[i];

        final_path[N] = curr_path[0];
    }

    static float firstMin(float[,] adj, int i) {
        float min = float.MaxValue;

        for (int k = 0; k < N; k++)
            if (adj[i, k] < min && i != k)
                min = adj[i, k];

        return min;
    }

    static float secondMin(float[,] adj, int i) {
        float first = float.MaxValue;
        float second = float.MaxValue;

        for (int j = 0; j < N; j++) {
            if (i == j)
                continue;

            if (adj[i, j] <= first) {
                second = first;
                first = adj[i, j];
            } else if (adj[i, j] <= second && adj[i, j] != first)
                second = adj[i, j];
        }
        return second;
    }

    static void TSPRec(float[,] adj, float curr_bound, float curr_weight, int level, int[] curr_path) {

        if (level == N) {
            if (adj[curr_path[level - 1], curr_path[0]] != 0) {
                float curr_res = curr_weight + adj[curr_path[level - 1], curr_path[0]];

                if (curr_res < final_res) {
                    copyToFinal(curr_path);
                    final_res = curr_res;
                }
            }
            return;
        }

        for (int i = 0; i < N; i++) {
            if (adj[curr_path[level - 1], i] != 0 && visited[i] == false) {
                float temp = curr_bound;
                curr_weight += adj[curr_path[level - 1], i];

                if (level == 1)
                    curr_bound -= ((firstMin(adj, curr_path[level - 1]) + firstMin(adj, i)) / 2);
                else
                    curr_bound -= ((secondMin(adj, curr_path[level - 1]) + firstMin(adj, i)) / 2);

                if (curr_bound + curr_weight < final_res) {
                    curr_path[level] = i;
                    visited[i] = true;

                    TSPRec(adj, curr_bound, curr_weight, level + 1, curr_path);
                }

                curr_weight -= adj[curr_path[level - 1], i];
                curr_bound = temp;

                Array.Fill(visited, false);
                for (int j = 0; j <= level - 1; j++)
                    visited[curr_path[j]] = true;
            }
        }
    }

    static void TSP(float[,] adj) {
        int[] curr_path = new int[N + 1];

        float curr_bound = 0;
        Array.Fill(curr_path, -1);
        Array.Fill(visited, false);

        for (int i = 0; i < N; i++)
            curr_bound += (firstMin(adj, i) + secondMin(adj, i));

        curr_bound = (curr_bound == 1) ? curr_bound / 2 + 1 : curr_bound / 2;

        visited[0] = true;
        curr_path[0] = 0;

        TSPRec(adj, curr_bound, 0, 1, curr_path);
    }

    protected override void Initializer() {
        base.SetUp("Branch And Bound 2", 0.2f, Color.green);
    }

    protected override List<City> Solve(List<City> cities) {

        N = cities.Count;
        final_path = new int[N + 1];
        visited = new bool[N];
        final_res = float.MaxValue;

        float[,] adj = new float[N, N];

        for (int i = 0; i < N; i++) {
            for (int j = i; j < N; j++) {

                float distance = i == j ?
                    float.MaxValue : Vector2.Distance(cities[i].position, cities[j].position);

                adj[i, j] = distance;
                adj[j, i] = distance;
            }
        }

        TSP(adj);

        List<City> final_ = new List<City>(final_path.Length);

        foreach(int i in final_path) {
            final_.Add(cities[i]);
        }

        return final_;
    }
}

/*
 * Explicação desse método aqui:
 * Ele de primeira olhada parece fazer todos os nodos recursivamente, porém
 * diferentemente do modo normal do branch and bound, este pega e na primeira iterada vai 
 * até o fim do caminho e constroi um caminho completo, mesmo que não seja o melhor, 
 * daí ele já possui um upperbound, ou seja, 
 * a partir de agora só expande os nodos com custo menor que o upperboud, se nao, passa reto. 
 * E também vai atualizando o upperbound conforme acha um caminho melhor
 */