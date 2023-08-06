using System.Collections.Generic;
using UnityEngine;

public class ConvexHullGrahamScan : TSP_VFX_Algorithm {

    protected override void Initializer() {
        base.SetUp("Convex Hull Graham Scan", 0.2f, Color.blue);
    }

    protected override List<City> Solve(List<City> cities) {

        /*
         * IDEIA: FAZER VÁRIOS ANEIS DE CONVEX HULLS E JUNTAR ELES ->
         * 
         * PARA JUNTAR OS CONVEX HULLS (ex. c1 e c2) É POSSIVEL ANALISAR 2 COISAS:
         * 
         * - a distancia dos pontos de c2 com as arestas formadas pelos pontos de c1 (PROVAVELEMNTE ERRADO, TESTEI E FAZENDO ASSIM PODE ESCOLHER INDEVIDAMENTE)
         * ou
         * - quanto interfere no perimetro adicionar um ponto de c2 entre as arestas de c1
         * 
         * DETALHE IMPORTANTE, PROVAVELMENTE SERÁ PRECISO PASSAR POR TODOS OS PONTOS DE c2 E REALIZAR AS MEDIDAS
         * ANTES DE TOMAR UMA DECISAO POIS NÃO É GARANTIDO QUE TODOS OS PONTOS DE c2 SE UNIRAO COM c1
         * 
         * APOS PERCORRIDO TODOS OS PONTOS DE c2 E TIRADO AS MEDIDAS, SERÁ POSSIVEL MODIFICAR c1 PARA INCLUIR O NOVO PONTO
         * 
         * OS CALCULOS ANTERIORMENTE REALIZADOS ENTRE OS PONTOS DE c2 E c1 NÃO SERÃO TOTALMENTE PERDIDOS, POIS c1 APENAS
         * PERDEU UMA ARESTA E GANHOU OUTRA PROVAVELEMNTE BEM PROX. DE ONDE ERA A ANTERIOR
         * 
         * ENTÃO É POSSIVEL MANTER OS CALCULOS, EXCLUINDO OS RESULTADOS DA ARESTA MODIFICADA E CALCULANDO NOVAMENTE PARA
         * A RECEM ADICIONADA
         * 
         * 
         * obs² seria interessante?? cada convexhull ter, alem da lista de pontos, o quanto cada aresta entre 2 pontos mede
         * e também qual o perimetro da forma total... isso seria util para quando fosse inserir um ponto de c2 entre uma aresta
         * de c1, ajundando a calcular o quanto muda o perimetro...
         * 
         * 
         * obs3 em vários momentos desse algoritmo eu usei List quando se for ver ele é BEEEEEM RUIM em termos
         * de performance para este caso... mas é so pra ver se funfa, depois rever isso e muitas outras coisas
         * :)
         * 
         */





        /*
         * IDEIA ALTERADA PARA TER APENAS UM CONVEXHULL (o primeiro) O RESTO É JUNTAR OS PONTOS Q SOBRARAM
         * 
         * POIS PODE SER (n sei certo) QUE UM PONTO DO 3º CONVEXHULL FOSSE O PROXIMO MAS AINDA ESTIVESSE 
         * AGRUPANDO COM O 2º
         * 
         */


        ConvexHull convexHull = ConvexHull.GrahamScan(cities);

        /*
        * DELETE POINTS FROM ORIGINAL LIST (BADDDD PERFORMANCE)
        * (obs this deletion in lists is horrendous in performance, but im just testing things out)
        * (obs² -1 is because last point goes back to first)
        */
        for (int i = 0; i < convexHull.points.Count - 1; i++)
            cities.Remove(convexHull.points[i]);


        City minChangedPoint = cities[0];
        int p1minIndex = 0;

        do {
            float minChangedDistance = float.MaxValue;

            foreach (var point in cities) {
                
                for(int i = 0; i < convexHull.points.Count - 1; i++) {

                    float changedDistance = convexHull.SimulatePointInsertion(i, point);

                    if(changedDistance < minChangedDistance) {
                        minChangedPoint = point;
                        minChangedDistance = changedDistance;
                        p1minIndex = i;
                    }

                }

            }

            cities.Remove(minChangedPoint);
            convexHull.InsertPoint(p1minIndex, minChangedPoint);

        } while(cities.Count > 0);


        return convexHull.points;
    }
}
