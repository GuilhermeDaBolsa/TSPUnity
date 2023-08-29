using System.Collections.Generic;

namespace Graph {

    public enum BlossomLabel {
        not_setted = 0,
        even = 1,
        odd = 2
    };

    public enum Constraints {
        Free = 0,
        Forced = 1,
        Forbidden = 2
    };

    public class Node {
        public int id;
        public Node π;

        public int rank = 0; // Kruskal
        public float key = float.MaxValue; // Prim		

        // Blossom
        public HashSet<Node> B;
        public Node mate;
        public bool active = true, pseudo = false, marked = false, blossom = false;
        public BlossomLabel label = BlossomLabel.not_setted;
        // Blossom
    }

    public class Edge {
        public float cost = float.MaxValue;
        public Node from, to;
        public Constraints constraint = Constraints.Free;

        public Edge(float cost_, Node from_, Node to_) {
            cost = cost_;
            this.from = from_;
            this.to = to_;

            //assert(from_.id != to_.id);
            if (from_.id == to_.id)
                throw new System.Exception("nao pode");
        }
    }

    public class Graph {
        public LinkedList<Node> V;
        public List<Edge> E;

        public Dictionary<int, int> vrtx_frc, vrtx_frb;

        public Dictionary<Node, LinkedList<Node>> Adj;
        public List<List<int>> AdjIDs;

        public int NumberOfNodes = 0;


        public Graph() {
            V = new LinkedList<Node>();
            E = new List<Edge>();
            vrtx_frc = new Dictionary<int, int>();
            vrtx_frb = new Dictionary<int, int>();
            Adj = new Dictionary<Node, LinkedList<Node>>();
            AdjIDs = new List<List<int>>();
        }

        public Graph(HashSet<int> V_) : this() {
            foreach (var v in V_)
                AddNode(v);
        }

        public Graph(LinkedList<Node> V_) : this() {
            foreach (var v in V_)
                AddNode(v.id);
        }

        public Graph(List<List<float>> DistanceMatrix2D) : this(DistanceMatrix2D.Count) {
            MakeConnected(DistanceMatrix2D);
        }

        public Graph(float[,] DistanceMatrix2D) : this(DistanceMatrix2D.GetLength(0)) {
            MakeConnected(DistanceMatrix2D);
        }

        public Graph(int NumberOfNodes_) : this(NumberOfNodes_, 0, NumberOfNodes_ - 1) { }

        public Graph(int NumberOfNodes_, int from, int to) : this() {
            //AdjIDs.resize(NumberOfNodes_);
            if (AdjIDs == null)
                AdjIDs = new List<List<int>>(NumberOfNodes_);
            else
                AdjIDs.Capacity = NumberOfNodes_;
            

            for (int d = from; d <= to; d++)
                AddNode(d);
        }

        void graph_set_edge_cstr(Edge ie, Constraints c) {
            var c_old = ie.constraint;
            ie.constraint = c;

            if (c_old == Constraints.Forced) {
                vrtx_frc[ie.from.id]--;
                vrtx_frc[ie.to.id]--;
            } else if (c_old == Constraints.Forbidden) {
                vrtx_frb[ie.from.id]--;
                vrtx_frb[ie.to.id]--;
            }

            if (c == Constraints.Forced) {
                vrtx_frc[ie.from.id]++;
                vrtx_frc[ie.to.id]++;
            } else if (c == Constraints.Forbidden) {
                vrtx_frb[ie.from.id]++;
                vrtx_frb[ie.to.id]++;
            }
        }

        void AddNode(int id_) {
            Node n = new Node();
            n.id = id_;

            V.AddLast(n);

            NumberOfNodes++;
        }


        void AddEdge(Edge e) {
            E.Add(e);

            if(!Adj.ContainsKey(e.from))
                Adj.Add(e.from, new LinkedList<Node>());
                
            Adj[e.from].AddLast(e.to);

            if (AdjIDs.Count > 0)
                AdjIDs[e.from.id].Add(e.to.id);
        }

        void AddEdge(float cost, Node from, Node to) {
            Edge e = new Edge(cost, from, to);
            AddEdge(e);
        }

        void AddEdge(float cost, int from_, int to_) {
            AddEdge(cost, NodeById(from_), NodeById(to_));
        }

        public void MakeConnected(List<List<float>> DistanceMatrix2D) {
			foreach (var d in V)
				foreach (var a in V)
					if (d.id != a.id)
						AddEdge(DistanceMatrix2D[d.id][a.id], d, a);
        }

        public void MakeConnected(float[,] DistanceMatrix2D) {
            foreach (var d in V)
                foreach (var a in V)
                    if (d.id != a.id)
                        AddEdge(DistanceMatrix2D[d.id, a.id], d, a);
        }

        public Node NodeById(int id) {
            foreach (var v in V)
                if (v.id == id)
                    return v;

            return null;
        }

        Node GetANode() {
            foreach (var v in V)
                return v;

            return null;
        }

        void PreVisit(Stack<int> R, int r) {
            R.Push(r);

            foreach (var v in V)
                if (v.π != null)
                    if (v.π.id == r)
                        PreVisit(R, v.id);
        }

        int degree_out(Node n) {
            return Adj[n].Count;
        }

        void SortEdgeByWeight() {

            E.Sort((a, b) => (int)(a.cost - b.cost));

            //TODO SORTING ORDER MIGHT BE WRONG, ORIGINAL WAS

            /*sort(E.begin(), E.end(), [](Edge l, Edge r) {
                return l.cost < r.cost;
            });*/
        }

        Dictionary<int, int> Degree() {
            Dictionary<int, int> R = new Dictionary<int, int>();

            foreach (var e in E) {
                R[e.from.id] = 0;
                R[e.to.id] = 0;
            }

            foreach (var e in E)
                R[e.from.id]++;

            foreach (var e in E)
                R[e.to.id]++;

            return R;
        }

        bool HaveCycle() {
            var D = Degree();

            foreach (var v in V)
                if (D[v.id] != 2)
                    return false;
            return true;
        }

        float Cost(List<List<float>> DistanceMatrix2D) {
            float cost = 0;

            foreach (var e in E)
                cost += DistanceMatrix2D[e.from.id][e.to.id];

            return cost;
        }

        float Cost() {
            float cost = 0;

            foreach (var e in E)
                cost += e.cost;

            return cost;
        }
    }
}
