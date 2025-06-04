internal class Program
{
    static void Main()
    {
        Console.WriteLine("Simulación de velocidad difusa tipo Zuma Revenge");

        while (true)
        {
            Console.Write("\nIngresa la distancia al pozo (0 a 100, vacío para salir): ");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;

            if (double.TryParse(input, out double distancia))
            {
                double velocidad = CalcularVelocidadDifusa(distancia);
                Console.WriteLine($"→ Velocidad difusa resultante: {velocidad:F2} (0 = lento, 10 = rápido)");
            }
            else
            {
                Console.WriteLine("Entrada inválida. Intenta de nuevo.");
            }
        }
    }

    static double CalcularVelocidadDifusa(double distancia)
    {
 
        double cerca = Trapezoide(distancia, 0, 0, 20, 30);
        double media = Triangulo(distancia, 30, 50, 70);
        double lejos = Trapezoide(distancia, 60, 80, 100, 100);

        double sumaPesos = 0;
        double sumaValores = 0;

        if (cerca > 0)
        {
            sumaPesos += cerca;
            sumaValores += cerca * 2; 
        }
        if (media > 0)
        {
            sumaPesos += media;
            sumaValores += media * 5; 
        }
        if (lejos > 0)
        {
            sumaPesos += lejos;
            sumaValores += lejos * 9; 
        }

        return sumaPesos == 0 ? 0 : sumaValores / sumaPesos;
    }
    static double Triangulo(double x, double a, double b, double c)
    {
        if (x <= a || x >= c) return 0;
        if (x == b) return 1;
        if (x < b) return (x - a) / (b - a);
        else return (c - x) / (c - b);
    }

    static double Trapezoide(double x, double a, double b, double c, double d)
    {
        if (x <= a || x >= d) return 0;
        if (x >= b && x <= c) return 1;
        if (x < b) return (b - a == 0) ? 1 : (x - a) / (b - a); 
        else return (d - c == 0) ? 1 : (d - x) / (d - c);       
    }
}