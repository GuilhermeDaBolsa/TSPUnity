using System;
using System.Collections.Generic;
using UnityEngine;

public class BranchAndBoundBFS : TSP_VFX_Algorithm {
    protected override void Initializer() {
        base.SetUp("Branch And Bound BFS", 0.2f, Color.gray);
    }

    protected override List<City> Solve(List<City> cities) {

        TreeOfPossibilities exploredNodes = new TreeOfPossibilities(cities.Count);

        TreeNode bestNode = new TreeNode(ref cities);
        bestNode.ReduceNodeMatrix();

        exploredNodes.Add(new List<TreeNode> { bestNode });

        do {
            bestNode = exploredNodes.ExtractMin();

            List<TreeNode> newExploredNodes = new List<TreeNode>(bestNode.citiesAvailableForNextStep.Count);
            
            foreach (var nextCity in bestNode.citiesAvailableForNextStep) {
                TreeNode exploredNode = new TreeNode(ref bestNode, nextCity);
                
                exploredNode.ReduceNodeMatrix();

                newExploredNodes.Add(exploredNode);
            }

            exploredNodes.Add(newExploredNodes);

        } while (exploredNodes.Min().citiesAvailableForNextStep.Count > 0);

        exploredNodes.Min().pathTookUntilNow.Add(cities[0]);
        return exploredNodes.Min().pathTookUntilNow;
    }

    /*
     * Represents all possible paths in the BFS
     * Uses a binary heap to keep track of the next node to be explored
     */
    public class TreeOfPossibilities {
        public BinaryHeap<float, List<TreeNode>> nodesToExplore;

        public TreeOfPossibilities(int treeHeight) {
            nodesToExplore = new BinaryHeap<float, List<TreeNode>>((int)Mathf.Pow(2, treeHeight + 1));
        }

        public void Add(List<TreeNode> newUnexploredNodes) {
            newUnexploredNodes.Sort(new TreeNodeComparer());

            var costOfBestNewUnexploredNode = newUnexploredNodes[newUnexploredNodes.Count - 1].costOfCurrentNode;

            nodesToExplore.Add(costOfBestNewUnexploredNode, newUnexploredNodes);
        }

        public TreeNode Min() {
            var bestNodeList = nodesToExplore.GetMin().value;
            return bestNodeList[bestNodeList.Count - 1];
        }

        public TreeNode ExtractMin() {
            List<TreeNode> bestNodeList = nodesToExplore.GetMin().value;

            TreeNode bestNode = bestNodeList[bestNodeList.Count - 1];

            bestNodeList.RemoveAt(bestNodeList.Count - 1);

            if(bestNodeList.Count == 0) {
                nodesToExplore.ExtractMin();
            } else {
                float newHeapNodeKey = bestNodeList[bestNodeList.Count - 1].costOfCurrentNode;
                nodesToExplore.IncreaseKey(0, newHeapNodeKey); //0 because min node is allways 0
            }

            return bestNode;
        }
    }

    /*
     * Represents a node in the path chosen by the BFS
     * Contains mainly the cost of the path, cost matrix and some other helper variables
     */
    public class TreeNode {

        public float[,] distanceCostMatrix;
        public List<City> pathTookUntilNow;
        public LinkedList<City> citiesAvailableForNextStep;
        public float costOfCurrentNode;

        private bool[] disabledRows;
        private bool[] disabledColumns;


        //Constructor to be used when creating the first node of the path ONLY
        //it just initializes every variable to its initial state, like calculating the cost matrix
        public TreeNode(ref List<City> cities) {
            distanceCostMatrix = new float[cities.Count, cities.Count];

            for (int i = 0; i < cities.Count; i++) {
                for (int j = i; j < cities.Count; j++) {

                    float distance = i == j ? 
                        float.MaxValue : Vector2.Distance(cities[i].position, cities[j].position);

                    distanceCostMatrix[i, j] = distance;
                    distanceCostMatrix[j, i] = distance;
                }
            }

            pathTookUntilNow = new List<City>{ cities[0] };

            citiesAvailableForNextStep = new LinkedList<City>();
            for(int i = 1; i < cities.Count; i++) {
                citiesAvailableForNextStep.AddLast(cities[i]);
            }

            costOfCurrentNode = 0;

            disabledRows = new bool[cities.Count];
            disabledColumns = new bool[cities.Count];
            Array.Fill(disabledRows, false);
            Array.Fill(disabledColumns, false);
        }


        //Contructor to be used when creating every other node of the path besides the first one
        public TreeNode(ref TreeNode fatherNode, City cityDestination) {
            City cityOrigin = fatherNode.pathTookUntilNow[fatherNode.pathTookUntilNow.Count - 1];
            distanceCostMatrix = (float[,])fatherNode.distanceCostMatrix.Clone();
            pathTookUntilNow = new List<City>(fatherNode.pathTookUntilNow) { cityDestination };
            citiesAvailableForNextStep = new LinkedList<City>(fatherNode.citiesAvailableForNextStep);
            
            citiesAvailableForNextStep.Remove(cityDestination);

            costOfCurrentNode = distanceCostMatrix[cityOrigin.index, cityDestination.index] + fatherNode.costOfCurrentNode;

            disabledRows = (bool[])fatherNode.disabledRows.Clone();
            disabledColumns = (bool[])fatherNode.disabledColumns.Clone();

            //disable from city row
            disabledRows[cityOrigin.index] = true;
            //disable to city column
            disabledColumns[cityDestination.index] = true;
            //disable going back to city origin
            distanceCostMatrix[cityDestination.index, cityOrigin.index] = float.MaxValue;
        }

        public void ReduceNodeMatrix() {
            //REDUCE ROWS
            for(int i = 0; i < distanceCostMatrix.GetLength(0); i++) {

                if (IsRowDisabled(i))
                    continue;

                float lowestValue = float.MaxValue;

                for (int j = 0; j < distanceCostMatrix.GetLength(1); j++) {

                    if (IsColumnDisabled(j))
                        continue;

                    float value = distanceCostMatrix[i, j];

                    if (IsInvalidValue(value))
                        continue;

                    if (value < lowestValue)
                        lowestValue = value;

                    if (lowestValue == 0)
                        break;
                }

                if(lowestValue != float.MaxValue && lowestValue != 0) {
                    for(int j = 0; j < distanceCostMatrix.GetLength(1); j++) {

                        if (IsInvalidValue(distanceCostMatrix[i, j]))
                            continue;

                        distanceCostMatrix[i, j] -= lowestValue;
                    }

                    costOfCurrentNode += lowestValue;
                }
            }

            //REDUCE COLUMNS
            for (int j = 0; j < distanceCostMatrix.GetLength(1); j++) {

                if (IsColumnDisabled(j))
                    continue;

                float lowestValue = float.MaxValue;

                for (int i = 0; i < distanceCostMatrix.GetLength(0); i++) {

                    if (IsRowDisabled(i))
                        continue;

                    float value = distanceCostMatrix[i, j];

                    if (IsInvalidValue(value))
                        continue;

                    if (value < lowestValue)
                        lowestValue = value;

                    if (lowestValue == 0)
                        break;
                }

                if (lowestValue != float.MaxValue && lowestValue != 0) {
                    for (int i = 0; i < distanceCostMatrix.GetLength(0); i++) {
                        
                        if (IsInvalidValue(distanceCostMatrix[i, j]))
                            continue;

                        distanceCostMatrix[i, j] -= lowestValue;
                    }

                    costOfCurrentNode += lowestValue;
                }
            }
        }

        private bool IsRowDisabled(int row) {
            return disabledRows[row];
        }

        private bool IsColumnDisabled(int column) {
            return disabledColumns[column];
        }

        private bool IsInvalidValue(float value) {
            return value == float.MaxValue; //all max value floats are not to be considered (i.e invalid)
        }
    }

    public class TreeNodeComparer : IComparer<TreeNode> {
        public int Compare(TreeNode x, TreeNode y) {
            if (x.costOfCurrentNode < y.costOfCurrentNode) return 1;
            if (x.costOfCurrentNode > y.costOfCurrentNode) return -1;

            return 0;
        }
    }
}