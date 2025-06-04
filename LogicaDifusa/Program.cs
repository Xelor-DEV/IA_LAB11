namespace LogicaDifusa
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            // Bucle principal del sistema
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
                        // Ejecuta el ejemplo de Metal Gear Solid
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

        // Funciones de membresía para conjuntos difusos

        /// <summary>
        /// Función de membresía trapezoidal.
        /// Calcula el grado de pertenencia de un valor 'x' a un conjunto difuso.
        /// </summary>
        /// <param name="x">Valor de entrada</param>
        /// <param name="a">Inicio de la base inferior</param>
        /// <param name="b">Fin de la base inferior/inicio de la meseta</param>
        /// <param name="c">Fin de la meseta/inicio de la base superior</param>
        /// <param name="d">Fin de la base superior</param>
        /// <returns>Grado de pertenencia [0-1]</returns>
        public static double Trapezoidal(double x, double a, double b, double c, double d)
        {
            if (x <= a) return 0;          // Fuera del rango izquierdo
            if (x <= b) return (x - a) / (b - a);  // Pendiente ascendente
            if (x <= c) return 1;          // Meseta central
            if (x <= d) return (d - x) / (d - c);  // Pendiente descendente
            return 0;                       // Fuera del rango derecho
        }

        /// <summary>
        /// Función de membresía triangular.
        /// Calcula el grado de pertenencia de un valor 'x' a un conjunto difuso.
        /// </summary>
        /// <param name="x">Valor de entrada</param>
        /// <param name="a">Base izquierda</param>
        /// <param name="b">Vértice superior</param>
        /// <param name="c">Base derecha</param>
        /// <returns>Grado de pertenencia [0-1]</returns>
        public static double Triangular(double x, double a, double b, double c)
        {
            if (x <= a) return 0;          // Fuera del rango izquierdo
            if (x <= b) return (x - a) / (b - a);  // Pendiente ascendente
            if (x <= c) return (c - x) / (c - b);  // Pendiente descendente
            return 0;                       // Fuera del rango derecho
        }
    }

    /// <summary>
    /// Ejemplo práctico: Sistema de detección de ruido en Metal Gear Solid
    /// que determina la rapidez de respuesta de los guardias.
    /// </summary>
    public class MetalGearSolidExample
    {
        // Variable difusa para el ruido (entrada)
        private static FuzzyVariable ruido = new FuzzyVariable("Ruido");

        // Valores singleton para la rapidez (salida)
        // Representan valores concretos asociados a términos lingüísticos
        private static Dictionary<string, double> rapidezSingletons = new Dictionary<string, double>();

        // Lista de reglas difusas (si-entonces)
        private static List<FuzzyRule> rules = new List<FuzzyRule>();

        // Constructor estático: inicializa la configuración del sistema
        static MetalGearSolidExample()
        {
            // Configuración de funciones de membresía para el ruido
            // - Usamos expresiones lambda para encapsular los parámetros de las funciones
            ruido.AddMembershipFunction("Silencioso", x => Program.Trapezoidal(x, 0, 0, 0.2, 0.4));
            ruido.AddMembershipFunction("Moderado", x => Program.Triangular(x, 0.3, 0.5, 0.7));
            ruido.AddMembershipFunction("Ruidoso", x => Program.Trapezoidal(x, 0.6, 0.8, 1, 1));

            // Definición de singletons para la salida (rapidez):
            // Cada término lingüístico tiene un valor concreto asociado
            rapidezSingletons.Add("Silencioso", 0.2);  // Baja rapidez
            rapidezSingletons.Add("Moderado", 0.5);    // Rapidez media
            rapidezSingletons.Add("Ruidoso", 0.9);     // Alta rapidez

            // Creación de reglas difusas:
            // Relacionan términos de entrada con términos de salida
            rules.Add(new FuzzyRule("Silencioso", "Silencioso"));
            rules.Add(new FuzzyRule("Moderado", "Moderado"));
            rules.Add(new FuzzyRule("Ruidoso", "Ruidoso"));
        }

        /// <summary>
        /// Ejecuta el ejemplo completo de lógica difusa
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("\n[METAL GEAR SOLID - Sistema de Rapidez de Guardias]");
            Console.Write("Ingrese nivel de ruido (0-1) o 'r' para aleatorio: ");
            var input = Console.ReadLine();

            double ruidoValue;
            var random = new Random();

            // Manejo de entrada (manual o aleatoria)
            if (input.ToLower() == "r")
            {
                ruidoValue = random.NextDouble();
                Console.WriteLine($"Valor generado: {ruidoValue:F8}");
            }
            else
            {
                ruidoValue = double.Parse(input);
            }

            // Paso 1: FUZZIFICACIÓN
            // Convierte el valor nítido en grados de pertenencia a conjuntos difusos
            var gradosPertenencia = ruido.Fuzzify(ruidoValue);

            // Paso 2: EVALUACIÓN DEL MOTOR DIFUSO
            // Aplica reglas y métodos de inferencia para obtener salida
            double rapidez = FuzzyInference.Evaluate(ruido, rapidezSingletons, rules, ruidoValue);

            // --- MOSTRAR RESULTADOS DETALLADOS ---
            Console.WriteLine("\n=== RESULTADOS DETALLADOS ===");
            Console.WriteLine($"- Ruido: {ruidoValue:F8}");

            // Grados de pertenencia
            Console.WriteLine("\nGRADOS DE PERTENENCIA:");
            foreach (var kvp in gradosPertenencia)
            {
                Console.WriteLine($"- {kvp.Key}: {kvp.Value * 100:F2}% ({kvp.Value:F8})");
            }

            // Gráfico visual
            Console.WriteLine("\nGRÁFICO DE PERTENENCIA (normalizado):");
            MembershipFunctionPlotter.PlotBars(gradosPertenencia);

            // Activación de reglas
            Console.WriteLine("\nACTIVACIÓN DE REGLAS:");
            var activaciones = new Dictionary<string, double>();
            foreach (var regla in rules)
            {
                if (gradosPertenencia.ContainsKey(regla.InputLabel))
                {
                    double grado = gradosPertenencia[regla.InputLabel];
                    double contribucion = grado * rapidezSingletons[regla.OutputLabel];

                    Console.WriteLine($"- Si ruido es {regla.InputLabel} -> rapidez = {rapidezSingletons[regla.OutputLabel]:F2}");
                    Console.WriteLine($"  - Grado: {grado:F8}");
                    Console.WriteLine($"  - Contribución: {grado:F8} * {rapidezSingletons[regla.OutputLabel]:F2} = {contribucion:F8}");

                    // Acumular contribuciones para defusificación
                    if (!activaciones.ContainsKey(regla.OutputLabel))
                        activaciones[regla.OutputLabel] = 0;
                    activaciones[regla.OutputLabel] += contribucion;
                }
            }

            // Paso 3: DEFUSIFICACIÓN
            Console.WriteLine("\nDEFUSIFICACIÓN:");
            double numerador = activaciones.Values.Sum();
            double denominador = gradosPertenencia.Values.Sum();
            Console.WriteLine($"- Numerador (Σ(gradoᵢ × singletonᵢ)) = {numerador:F8}");
            Console.WriteLine($"- Denominador (Σ(gradoᵢ)) = {denominador:F8}");
            Console.WriteLine($"- Rapidez = {numerador:F8} / {denominador:F8} = {rapidez:F8}");

            // Resultado final
            Console.WriteLine($"\nRESULTADO FINAL:");
            Console.WriteLine($"- Rapidez recomendada: {rapidez:F8} ({rapidez * 100:F2}%)");

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    // (Clase ZumaRevengeExample omitida por brevedad)

    /// <summary>
    /// Representa una variable difusa con sus funciones de membresía
    /// </summary>
    public class FuzzyVariable
    {
        public string Name { get; }

        // Diccionario de funciones de membresía:
        // - Key: Término lingüístico (ej: "Alto")
        // - Value: Función que calcula el grado de pertenencia
        public Dictionary<string, Func<double, double>> MembershipFunctions { get; }

        public FuzzyVariable(string name)
        {
            Name = name;
            MembershipFunctions = new Dictionary<string, Func<double, double>>();
        }

        /// <summary>
        /// Añade una función de membresía a la variable
        /// </summary>
        public void AddMembershipFunction(string label, Func<double, double> function)
        {
            MembershipFunctions[label] = function;
        }

        /// <summary>
        /// Realiza la fuzzificación de un valor nítido
        /// </summary>
        /// <returns>
        /// Diccionario con los grados de pertenencia para cada término lingüístico
        /// </returns>
        public Dictionary<string, double> Fuzzify(double value)
        {
            // Usamos LINQ para transformar el diccionario:
            // - Para cada función de membresía, aplicamos al valor de entrada
            return MembershipFunctions.ToDictionary(
                f => f.Key,         // Mantenemos la misma clave (término lingüístico)
                f => f.Value(value)  // Calculamos el grado de pertenencia
            );
        }
    }

    /// <summary>
    /// Representa una regla difusa simple (SI-ENTONCES)
    /// </summary>
    public class FuzzyRule
    {
        public string InputLabel { get; }  // Término lingüístico de entrada
        public string OutputLabel { get; } // Término lingüístico de salida

        public FuzzyRule(string inputLabel, string outputLabel)
        {
            InputLabel = inputLabel;
            OutputLabel = outputLabel;
        }
    }

    /// <summary>
    /// Motor de inferencia difusa
    /// </summary>
    public static class FuzzyInference
    {
        /// <summary>
        /// Evalúa el sistema difuso completo
        /// </summary>
        /// <param name="inputVar">Variable de entrada</param>
        /// <param name="outputSingletons">Valores singleton de salida</param>
        /// <param name="rules">Reglas difusas</param>
        /// <param name="inputValue">Valor de entrada nítido</param>
        /// <returns>Valor de salida defusificado</returns>
        public static double Evaluate(
            FuzzyVariable inputVar,
            Dictionary<string, double> outputSingletons,
            List<FuzzyRule> rules,
            double inputValue)
        {
            // Paso 1: Fuzzificación (convertir entrada a grados de pertenencia)
            var inputFuzzified = inputVar.Fuzzify(inputValue);

            // Paso 2: Evaluación de reglas (activación)
            var activations = new Dictionary<string, double>();

            foreach (var rule in rules)
            {
                if (inputFuzzified.ContainsKey(rule.InputLabel))
                {
                    double degree = inputFuzzified[rule.InputLabel];

                    // Método de agregación: MAXIMO (tomamos el mayor grado de activación)
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

            // Paso 3: Defusificación (método de los singletons - media ponderada)
            double numerator = 0;
            double denominator = 0;

            foreach (var activation in activations)
            {
                if (outputSingletons.ContainsKey(activation.Key))
                {
                    // Acumular: (grado * valor_singleton)
                    numerator += activation.Value * outputSingletons[activation.Key];

                    // Acumular grados para normalización
                    denominator += activation.Value;
                }
            }

            // Evitar división por cero
            return denominator == 0 ? 0 : numerator / denominator;
        }
    }

    /// <summary>
    /// Utilidad para visualización de grados de pertenencia
    /// </summary>
    public static class MembershipFunctionPlotter
    {
        /// <summary>
        /// Muestra un gráfico de barras en consola de los grados de pertenencia
        /// </summary>
        public static void PlotBars(Dictionary<string, double> membershipValues)
        {
            const int maxBarWidth = 50; // Ancho máximo de la barra (100%)

            // Normalizar valores para que el máximo sea 1.0 (100%)
            double maxValue = membershipValues.Values.Max();
            double scaleFactor = maxValue > 0 ? 1.0 / maxValue : 1.0;

            foreach (var kvp in membershipValues)
            {
                double normalizedValue = kvp.Value * scaleFactor;
                int barWidth = (int)(normalizedValue * maxBarWidth);
                string bar = new string('█', barWidth);

                // Formatear etiqueta y barra
                string label = $"{kvp.Key.PadRight(12)}: ";
                Console.Write(label);
                Console.WriteLine($"{bar} {kvp.Value * 100:F2}%");
            }
        }
    }
}