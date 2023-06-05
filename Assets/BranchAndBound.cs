using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BranchAndBound : TSP_VFX_Algorithm {
    protected override void Initializer() {
        base.SetUp("Branch And Bound", 0.2f, Color.gray);
    }

    protected override List<City> Solve(List<City> cities) {

        TreeOfPossibilities possibilities = new TreeOfPossibilities(cities.Count);

        TreeNode minNode = new TreeNode(ref cities);
        minNode.ReduceNodeMatrix();

        possibilities.Add(new List<TreeNode> { minNode });

        do {
            minNode = possibilities.ExtractMin();

            List<TreeNode> expandedPossibilities = new List<TreeNode>(minNode.citiesAvailableForNextStep.Count);
            
            foreach (var cityDestination in minNode.citiesAvailableForNextStep) {
                TreeNode newLeafNode = new TreeNode(ref minNode, cityDestination);
                
                newLeafNode.ReduceNodeMatrix();

                expandedPossibilities.Add(newLeafNode);
            }

            possibilities.Add(expandedPossibilities);

        } while (possibilities.Min().citiesAvailableForNextStep.Count > 0);

        possibilities.Min().pathTookUntilNow.Add(cities[0]);
        return possibilities.Min().pathTookUntilNow;
    }

    public class TreeOfPossibilities {
        public BinaryHeap<float, List<TreeNode>> heapOfPossibilities;

        public TreeOfPossibilities(int treeHeight) {
            heapOfPossibilities = new BinaryHeap<float, List<TreeNode>>((int)Mathf.Pow(2, treeHeight + 1));
        }

        public void Add(List<TreeNode> listOfPossibilities) {
            listOfPossibilities.Sort(new TreeNodeComparer());

            heapOfPossibilities.Add(
                listOfPossibilities[listOfPossibilities.Count - 1].costOfCurrentNode, listOfPossibilities);
        }

        public TreeNode Min() {
            var minPossibilitiesList = heapOfPossibilities.GetMin().value;
            return minPossibilitiesList[minPossibilitiesList.Count - 1];
        }

        public TreeNode ExtractMin() {
            List<TreeNode> minPossibilitiesList = heapOfPossibilities.GetMin().value;

            TreeNode minValue = minPossibilitiesList[minPossibilitiesList.Count - 1];

            minPossibilitiesList.RemoveAt(minPossibilitiesList.Count - 1);

            if(minPossibilitiesList.Count == 0) {
                heapOfPossibilities.ExtractMin();
            } else {
                float newHeapNodeKey = minPossibilitiesList[minPossibilitiesList.Count - 1].costOfCurrentNode;
                heapOfPossibilities.IncreaseKey(0, newHeapNodeKey); //0 because min node is allways 0
            }

            return minValue;
        }
    }

    public class TreeNode {

        public float[,] distanceCostMatrix;
        public List<City> pathTookUntilNow;
        public LinkedList<City> citiesAvailableForNextStep;
        public float costOfCurrentNode;

        private bool[] disabledRows;
        private bool[] disabledColumns;

        //This constructor should be used to create the first node only
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

        //This constructor should be used to create every other node besides the first one
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
            return value == float.MaxValue; //all max value floats are not to be considered
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