
using System.Collections.Generic;

public class BinaryHeap<Key, Value> where Key : System.IComparable<Key> {
    public List<Node> heapArray;

    public BinaryHeap(int capacity) {
        heapArray = new List<Node>(capacity);
    }

    private void SwapElements(int index1, int index2) {
        Node temp = heapArray[index1];
        heapArray[index1] = heapArray[index2];
        heapArray[index2] = temp;
    }

    private int ParentIndexOf(int currentIndex) {
        return (currentIndex - 1) / 2;
    }

    private int LeftChildIndexOf(int currentIndex) {
        return 2 * currentIndex + 1;
    }

    private int RightChildIndexOf(int currentIndex) {
        return 2 * currentIndex + 2;
    }

    public bool Add(Key key, Value value) {
        int i = heapArray.Count;
        heapArray.Add(new Node(key, value));

        //NOTE IF NEY KEY IS EQUAL TO PARENT KEY, THEN THE NEW KEY WILL BE FAVORED (BUT THE NORMAL WAY IS JUST < INSTEAD OF <=)
        while (heapArray[i].CompareTo(heapArray[ParentIndexOf(i)]) <= 0) {
            SwapElements(i, ParentIndexOf(i));

            i = ParentIndexOf(i);
            
            if (i == 0)
                break;
        }

        return true;
    }

    public Node GetMin() {
        return heapArray[0];
    }

    public int CurrentSize() {
        return heapArray.Count;
    }

    public Node ExtractMin() {
        if (heapArray.Count == 0)
            return null;

        if (heapArray.Count == 1) {
            Node node = heapArray[0];
            heapArray.RemoveAt(0);
            return node;
        }

        Node root = heapArray[0];

        heapArray[0] = heapArray[heapArray.Count - 1];
        heapArray.RemoveAt(heapArray.Count - 1);
        MinHeapify(0);

        return root;
    }

    public void DecreaseKey(int index, Key newVal) {
        heapArray[index].key = newVal;

        while (index != 0 && heapArray[index].CompareTo(heapArray[ParentIndexOf(index)]) < 0) {
            SwapElements(index, ParentIndexOf(index));
            index = ParentIndexOf(index);
        }
    }

    public void IncreaseKey(int index, Key newVal) {
        heapArray[index].key = newVal;
        MinHeapify(index);
    }


    private void MinHeapify(int index) {
        int lIndex = LeftChildIndexOf(index);
        int rIndex = RightChildIndexOf(index);

        int smallest = index;

        if (lIndex < heapArray.Count && heapArray[lIndex].CompareTo(heapArray[smallest]) < 0) {
            smallest = lIndex;
        }

        if (rIndex < heapArray.Count && heapArray[rIndex].CompareTo(heapArray[smallest]) < 0) {
            smallest = rIndex;
        }

        if (smallest != index) {
            SwapElements(index, smallest);
            MinHeapify(smallest);
        }
    }

    public class Node : System.IComparable<Node> {
        public Key key;
        public Value value;

        public Node(Key key, Value value) {
            this.key = key;
            this.value = value;
        }

        public int CompareTo(Node obj) {
            return this.key.CompareTo(obj.key);
        }
    }
}