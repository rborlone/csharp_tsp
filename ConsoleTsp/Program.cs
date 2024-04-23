namespace HelloWorld;

internal class Program
{
    static void Main(string[] args)
    {
        Random seeder = new Random();
        int seed = seeder.Next();
        Random engine = new Random(seed);

        Console.WriteLine("My name is Rodrigo");
        Console.WriteLine("Hello, World!");

        Mapa mapa = new Mapa("../data/berlin52.dat");
        mapa.ComputarCandidatos(25);

        Tour mejor = null;
        for (int i = 0; i < 1; i++)
        {
            
            Tour tour = new Tour(mapa.data.Count, engine);
            tour.costo = tour.Evaluar(mapa);

            tour.Mostrar();

            int nSinMejora = 0;
            while (nSinMejora < 1000)
            {
                int ganancia = tour.TwoOpt(engine, mapa);
                if (ganancia > 0)
                {
                    nSinMejora = 0;
                    tour.costo -= ganancia;
                    tour.Mostrar();
                    Console.WriteLine("TOUR: " + tour.costo.ToString());
                }
                else
                {
                    nSinMejora++;
                }
            }

            if (mejor == null || tour.costo < mejor.costo)
            {
                mejor = tour;
                Console.WriteLine("MEJOR TOUR: " + mejor.costo);
            }

        }
    }
}