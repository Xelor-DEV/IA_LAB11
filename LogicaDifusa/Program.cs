using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicaDifusa
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Sistema de Lógica Difusa");
                Console.WriteLine("1. Metal Gear Solid (Ruido -> Rapidez)");
                Console.WriteLine("2. Salir");
                Console.Write("Seleccione un ejemplo: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        MetalGearSolidExample.Run();
                        break;
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Intente de nuevo.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones de membresía
        public static double Trapezoidal(double x, double a, double b, double c, double d)
        {
            if (x <= a) return 0;
            if (x <= b) return (x - a) / (b - a);
            if (x <= c) return 1;
            if (x <= d) return (d - x) / (d - c);
            return 0;
        }

        public static double Triangular(double x, double a, double b, double c)
        {
            if (x <= a) return 0;
            if (x <= b) return (x - a) / (b - a);
            if (x <= c) return (c - x) / (c - b);
            return 0;
        }
    }

    public class MetalGearSolidExample
    {
        private static FuzzyVariable ruido = new FuzzyVariable("Ruido");
        private static Dictionary<string, double> rapidezSingletons = new Dictionary<string, double>();
        private static List<FuzzyRule> rules = new List<FuzzyRule>();

        static MetalGearSolidExample()
        {
            // Configuración del sistema
            ruido.AddMembershipFunction("Silencioso", x => Program.Trapezoidal(x, 0, 0, 0.2, 0.4));
            ruido.AddMembershipFunction("Moderado", x => Program.Triangular(x, 0.3, 0.5, 0.7));
            ruido.AddMembershipFunction("Ruidoso", x => Program.Trapezoidal(x, 0.6, 0.8, 1, 1));

            rapidezSingletons.Add("Silencioso", 0.2);
            rapidezSingletons.Add("Moderado", 0.5);
            rapidezSingletons.Add("Ruidoso", 0.9);

            rules.Add(new FuzzyRule("Silencioso", "Silencioso"));
            rules.Add(new FuzzyRule("Moderado", "Moderado"));
            rules.Add(new FuzzyRule("Ruidoso", "Ruidoso"));
        }

        public static void Run()
        {
            // Menú de entrada
            Console.WriteLine("\n[METAL GEAR SOLID - Sistema de Rapidez de Guardias]");
            Console.Write("Ingrese nivel de ruido (0-1) o 'r' para aleatorio: ");
            var input = Console.ReadLine();

            double ruidoValue;
            var random = new Random();

            if (input.ToLower() == "r")
            {
                ruidoValue = random.NextDouble();
                Console.WriteLine($"Valor generado: {ruidoValue:F2}");
            }
            else
            {
                ruidoValue = double.Parse(input);
            }

            // Evaluar
            double rapidez = FuzzyInference.Evaluate(ruido, rapidezSingletons, rules, ruidoValue);
            Console.WriteLine($"\nResultado:");
            Console.WriteLine($"- Ruido: {ruidoValue:F2}");
            Console.WriteLine($"- Rapidez recomendada: {rapidez:F2}");

            // Mostrar gráficos
            Console.WriteLine("\nFunciones de Pertenencia (Ruido):");
            MembershipFunctionPlotter.Plot(ruido, 0, 1, 0.05);

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    public class ZumaRevengeExample
    {
        // Implementación similar para Zuma's Revenge
        // (Se omite por brevedad según lo solicitado)
    }

    public class FuzzyVariable
    {
        public string Name { get; }
        public Dictionary<string, Func<double, double>> MembershipFunctions { get; }

        public FuzzyVariable(string name)
        {
            Name = name;
            MembershipFunctions = new Dictionary<string, Func<double, double>>();
        }

        public void AddMembershipFunction(string label, Func<double, double> function)
        {
            MembershipFunctions[label] = function;
        }

        public Dictionary<string, double> Fuzzify(double value)
        {
            return MembershipFunctions.ToDictionary(
                f => f.Key,
                f => f.Value(value)
            );
        }
    }

    public class FuzzyRule
    {
        public string InputLabel { get; }
        public string OutputLabel { get; }

        public FuzzyRule(string inputLabel, string outputLabel)
        {
            InputLabel = inputLabel;
            OutputLabel = outputLabel;
        }
    }

    public static class FuzzyInference
    {
        public static double Evaluate(
            FuzzyVariable inputVar,
            Dictionary<string, double> outputSingletons,
            List<FuzzyRule> rules,
            double inputValue)
        {
            // Paso 1: Fuzzificación
            var inputFuzzified = inputVar.Fuzzify(inputValue);

            // Paso 2: Evaluación de reglas
            var activations = new Dictionary<string, double>();

            foreach (var rule in rules)
            {
                if (inputFuzzified.ContainsKey(rule.InputLabel))
                {
                    double degree = inputFuzzified[rule.InputLabel];

                    if (activations.ContainsKey(rule.OutputLabel))
                    {
                        activations[rule.OutputLabel] = Math.Max(
                            activations[rule.OutputLabel],
                            degree
                        );
                    }
                    else
                    {
                        activations[rule.OutputLabel] = degree;
                    }
                }
            }

            // Paso 3: Defusificación (método de los singletons)
            double numerator = 0;
            double denominator = 0;

            foreach (var activation in activations)
            {
                if (outputSingletons.ContainsKey(activation.Key))
                {
                    numerator += activation.Value * outputSingletons[activation.Key];
                    denominator += activation.Value;
                }
            }

            return denominator == 0 ? 0 : numerator / denominator;
        }
    }

    public static class MembershipFunctionPlotter
    {
        public static void Plot(FuzzyVariable variable, double min, double max, double step)
        {
            // Encabezado
            Console.WriteLine($"Variable: {variable.Name}");
            Console.WriteLine($"Rango: [{min}, {max}]");

            // Preparar datos
            var points = new Dictionary<string, List<Tuple<double, double>>>();
            foreach (var mf in variable.MembershipFunctions)
            {
                points[mf.Key] = new List<Tuple<double, double>>();
            }

            // Calcular valores
            for (double x = min; x <= max; x += step)
            {
                foreach (var mf in variable.MembershipFunctions)
                {
                    double y = mf.Value(x);
                    points[mf.Key].Add(Tuple.Create(x, y));
                }
            }

            // Dibujar gráfico
            int graphWidth = 60;
            int graphHeight = 10;

            foreach (var set in points)
            {
                Console.Write($"{set.Key.PadRight(12)}: ");

                for (int i = 0; i <= graphWidth; i++)
                {
                    double x = min + i * (max - min) / graphWidth;
                    double y = set.Value.FirstOrDefault(p =>
                        Math.Abs(p.Item1 - x) < step / 2)?.Item2 ?? 0;

                    int barHeight = (int)(y * graphHeight);

                    if (barHeight > 0)
                    {
                        Console.Write(new string('▄', barHeight));
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }

            // Eje X
            Console.WriteLine(new string('_', graphWidth + 13));
            Console.Write(new string(' ', 13));
            Console.WriteLine($"{min} {new string(' ', graphWidth - 5)}{max}");
        }
    }
}