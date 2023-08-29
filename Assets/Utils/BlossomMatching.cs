using Graph;
using System.Collections.Generic;

public class BlossomMatching {

    Dictionary<Node, LinkedList<Node>> Adj_;
    LinkedList<Node> V_;
    HashSet<Edge> M;
    Stack<Node> L;
    Graph.Graph G;

    public BlossomMatching() {
        Adj_ = new Dictionary<Node, LinkedList<Node>>();
        V_ = new LinkedList<Node>();
        M = new HashSet<Edge>();
        L = new Stack<Node>();
    }

    HashSet<Node> FindBlossom(Node p, Node i, Node j) {
        HashSet<Node> B = new HashSet<Node>();

        foreach (var n in G.V)
            n.blossom = false;

        var k = i;
        k.blossom = true;

        do {
            k = k.π;
            k.blossom = true;
        } while (k != p);

        k = j;

        do {
            k.blossom = true;
            k = k.π;
        } while (!k.blossom);

        var w = k;

        do {
            k = k.π;
            k.blossom = false;
        } while (k != p);

        foreach (var n in G.V)
            if (n.blossom) {
                B.Add(n);
                n.active = false;
            }

        return B;
    }

    void Contract(Node p, Node i, Node j) {
        Node b = new Node(); // memory node

        b.B = FindBlossom(p, i, j);
        b.pseudo = true;

        V_.AddLast(b);
        Adj_[b].Clear();

        foreach (var k in G.V)
            k.marked = false;

        foreach (var y in b.B)
            foreach (var k in Adj_[y])
                k.marked = true;

        foreach (var k in G.V)
            if (k.marked) {
                Adj_[b].AddLast(j);
                Adj_[j].AddLast(b);
            }
    }

    Node ExamineEven(Node p, Node i, ref bool found) {
        foreach (var j in Adj_[i]) {
            if (j.label == BlossomLabel.even) {
                Contract(p, i, j);

                return null;
            }

            if (j.active && j.mate == null) {
                var q = j;
                q.π = i;
                found = true;

                return q;
            }

            if (j.active && j.label == BlossomLabel.not_setted) {
                j.π = i;
                j.label = BlossomLabel.odd;

                L.Push(j);
            }
        }

        return null;
    }

    void ExamineOdd(Node p, Node i) {
        var j = i.mate;

        if (j.label == BlossomLabel.odd) {
            j.π = i;
            Contract(p, i, j);

            return;
        }

        if (j.mate == null && j.label == BlossomLabel.not_setted) {
            j.π = i;
            j.label = BlossomLabel.even;

            L.Push(j);
        }
    }

    void Search(Node p) {
        var found = false;
        Node q = new Node();

        V_ = G.V;
        Adj_ = G.Adj;

        p.label = BlossomLabel.even;
        L.Push(p);

        while (L.Count > 0 && !found) {
            var i = L.Peek();
            L.Pop();

            if (i.label == BlossomLabel.even)
                q = ExamineEven(p, i, ref found);
            else
                ExamineOdd(p, i);
        }
        if (found)
            Augment(p, q);
    }

    Node FindMarked(LinkedList<Node> Z) {
        foreach (var z in Z)
            if (z.marked)
                return z;

        return null;
    }

    void ExpandPred(Node k) {
        var b = k.π;

        foreach (var i in V_)
            i.marked = false;

        foreach (var i in b.B)
            i.marked = true;

        var j = FindMarked(Adj_[k]);
        k.π = j;
    }

    bool SetContainsThisEdge(HashSet<Edge> S, Edge e) {
        foreach (var x in S)
            if (x.from == e.from ||
                x.to == e.to ||
                x.from == e.to ||
                x.to == e.from)
                return true;

        return false;
    }

    HashSet<Edge> SymmetricDifference(HashSet<Edge> S1, HashSet<Edge> S2) {
        HashSet<Edge> symmetricDifference = new HashSet<Edge>();

        foreach (var e in S1)
            symmetricDifference.Add(e);

        foreach (var e in S2)
            if (!SetContainsThisEdge(S1, e))
                symmetricDifference.Add(e);

        return symmetricDifference;
    }

    void Augment(Node p, Node q) {
        HashSet<Edge> P = new HashSet<Edge>();
        var k = q;

        do {
            if (k.π.pseudo) {
                ExpandPred(k);
            } else {
                Edge e = new Edge(0, k.π, k);
                P.Add(e);
                k = k.π;
            }
        } while (k != p);

        M = SymmetricDifference(M, P);
    }


    public HashSet<Edge> Solve(Graph.Graph graph) {
        HashSet<int> done = new HashSet<int>();

        G = graph;

        reiterate:
        foreach (var p in G.V)
            if (done.Contains(p.id)) {
                done.Add(p.id);

                if (p.mate == null) {
                    Search(p);

                    if (p.mate == null) {
                        G.V.Remove(p);
                        G.Adj.Remove(p);

                        foreach (var v in G.V)
                            G.Adj[v].Remove(p);

                        G.NumberOfNodes--;
                        goto reiterate;
                    }
                }
            }

        return M;
    }
}