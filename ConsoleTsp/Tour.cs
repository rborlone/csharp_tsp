using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Tour
{
    private int largo;
    private List<Nodo> ady;
    public int costo;

public Tour(int n, Random engine) {
        largo = n;
        ady = new List<Nodo>(n);


        List<int> ciudades = Enumerable.Range(0, n).ToList();
        ciudades.Add(0);
        Revolver(ciudades, 1, ciudades.Count - 1, engine);

        int posicion = 0;
        Nodo adyAnterior = null;

        for (int index=0; index < ciudades.Count; index++){
            if (index==0) continue;
            
            posicion++;
            if(posicion == n)
                posicion = 0;

            var nodo = new Nodo {
                posicion = posicion,
                id = ciudades[index],
                anterior = ((ady.Count != 0)?ady[index - 2]:null)
            };

            ady.Add(nodo);

            if (index==ciudades.Count - 1){
                adyAnterior.siguiente = nodo;
                nodo.siguiente = ady.First();
                ady.First().anterior = nodo;
            } else if (index != 0 && adyAnterior != null) {
                adyAnterior.siguiente = nodo;
            }

            adyAnterior = nodo;
        }
    }

    /***
        Metodo para barajar las ciudades.
    */
    private static void Revolver<T>(List<T> list, int start, int end, Random rng)
    {
        int n = end - start;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = list[start + k];
            list[start + k] = list[start + n];
            list[start + n] = temp;
        }
    }

    /**
        Mostrar el tour
    */
    public void Mostrar()
    {
        Console.Write("0,");
        var inicio = ady[0];
        var actual = inicio;
        do {
            Console.Write(actual.id + ",");
            actual = actual.siguiente;
        } while(actual != inicio);
    }

    public int Evaluar(Mapa m)
    {
        var inicio = ady[0];
        var ptr = inicio;
        int suma = 0;
        do {
            suma += m.data[ptr.anterior.id][ptr.id];
            ptr = ptr.siguiente;
        } while (ptr != inicio); 

        suma += m.data[ptr.anterior.id][ptr.id];
        return suma;
    }

    public int TwoOpt(Random engine, Mapa mapa)
    {
        int aleatorio = engine.Next(0, largo - 1);

        var t0 = ady[aleatorio];
        var t1 = t0.siguiente;

        foreach (var nodo in mapa.candidatosDeNodo[t1.id])
        {
            var t2 = ady[nodo];
            if (t1.siguiente == t2 || t0 == t2) continue; 
            var t3 = t2.anterior;
            int ganancia = mapa.data[t0.id][t1.id] - mapa.data[t1.id][t2.id] + mapa.data[t2.id][t3.id] - mapa.data[t3.id][t0.id];
            if (ganancia > 0)
            {
                Mover(t0, t1, t2, t3);
                return ganancia;
            }
        }
        return 0;
    }

    private void Mover(Nodo t0, Nodo t1, Nodo t2, Nodo t3)
    {
        int pos = t3.posicion;
        var ptr = t1;
        while (ptr != t2)
        {
            ptr.posicion = pos--;
            if (pos < 0) pos = largo - 1;
            Swap(ref ptr.siguiente, ref ptr.anterior);
            ptr = ptr.anterior;
        }
        t0.siguiente = t3; t3.anterior = t0;
        t1.siguiente = t2; t2.anterior = t1;
    }

    public bool IsConexa()
    {
        var inicio = ady[0];
        var actual = inicio.siguiente;
        int cont = 1;
        int pos = inicio.posicion;
        pos++; if (pos == ady.Count) pos = 0;
        while (actual != inicio)
        {
            if (pos != actual.posicion)
            {
                Console.WriteLine("Esperada: " + pos + ", encontrada: " + actual.posicion + ", en nodo " + actual.id);
                return false;
            }
            pos++; if (pos == ady.Count) pos = 0;
            cont++;
            if (actual.siguiente.anterior != actual)
            {
                Console.WriteLine("Anterior mal configurado en nodo: " + actual.siguiente.id);
                return false;
            }
            actual = actual.siguiente;
            if (cont > ady.Count) return false;
        }
        return cont == ady.Count;
    }

    

    private void Swap(ref Nodo a, ref Nodo b)
    {
        Nodo temp = a;
        a = b;
        b = temp;
    }
}

public class Nodo
{
    public int id;
    public int posicion;
    public Nodo anterior;
    public Nodo siguiente;
}