using QuantityMeasurementBusinessLayer;
using QuantityMeasurementModel;
using QuantityMeasurementRepository;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// UC15: Controller Layer — Menu Driven with User Input.
    /// Accepts user input → builds QuantityDTO → calls service → displays result.
    /// No business logic here.
    /// </summary>
    public class QuantityMeasurementController
    {
        private readonly IQuantityMeasurementService    _service;
        private readonly IQuantityMeasurementRepository _repo;

        public QuantityMeasurementController(
            IQuantityMeasurementService    service,
            IQuantityMeasurementRepository repo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _repo    = repo    ?? throw new ArgumentNullException(nameof(repo));
        }

        // ── ENTRY POINT ──────────────────────────────────────────────────

        public void Start()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== Quantity Measurement App UC15 ===");
                Console.WriteLine("1. Length Operations");
                Console.WriteLine("2. Weight Operations");
                Console.WriteLine("3. Volume Operations");
                Console.WriteLine("4. Temperature Operations");
                Console.WriteLine("5. Operation History");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": RunLengthMenu();      break;
                    case "2": RunWeightMenu();      break;
                    case "3": RunVolumeMenu();      break;
                    case "4": RunTemperatureMenu(); break;
                    case "5": RunHistoryMenu();     break;
                    case "6":
                        running = false;
                        Console.WriteLine("Thank you for using Quantity Measurement App.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        // ── CONTROLLER API METHODS ────────────────────────────────────────

        public string PerformComparison(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var  result = _service.Compare(q1, q2);
                bool equal  = result.Value == 1;
                return $"Comparison Result: {(equal ? "true" : "false")}";
            }
            catch (QuantityMeasurementException ex)
            {
                return $"[ERROR] {ex.Message}";
            }
        }

        public string PerformConversion(QuantityDTO q1, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.Convert(q1, targetUnit);
                return $"Conversion Result: {result.Value} {result.UnitName}";
            }
            catch (QuantityMeasurementException ex)
            {
                return $"[ERROR] {ex.Message}";
            }
        }

        public string PerformAddition(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var result = _service.Add(q1, q2);
                return $"Addition Result: {result.Value} {result.UnitName}";
            }
            catch (QuantityMeasurementException ex)
            {
                return $"[ERROR] {ex.Message}";
            }
        }

        public string PerformSubtraction(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var result = _service.Subtract(q1, q2);
                return $"Subtraction Result: {result.Value} {result.UnitName}";
            }
            catch (QuantityMeasurementException ex)
            {
                return $"[ERROR] {ex.Message}";
            }
        }

        public string PerformDivision(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var result = _service.Divide(q1, q2);
                return $"Division Result: {result.Value} (scalar)";
            }
            catch (QuantityMeasurementException ex)
            {
                return $"[ERROR] {ex.Message}";
            }
        }

        // ── CATEGORY MENUS ────────────────────────────────────────────────

        private void RunLengthMenu()
        {
            Console.WriteLine("\n--- Length Operations ---");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
            Console.Write("Enter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("LENGTH");  break;
                case "2": RunConvert("LENGTH");  break;
                case "3": RunAdd("LENGTH");      break;
                case "4": RunSubtract("LENGTH"); break;
                case "5": RunDivide("LENGTH");   break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        private void RunWeightMenu()
        {
            Console.WriteLine("\n--- Weight Operations ---");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
            Console.Write("Enter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("WEIGHT");  break;
                case "2": RunConvert("WEIGHT");  break;
                case "3": RunAdd("WEIGHT");      break;
                case "4": RunSubtract("WEIGHT"); break;
                case "5": RunDivide("WEIGHT");   break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        private void RunVolumeMenu()
        {
            Console.WriteLine("\n--- Volume Operations ---");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
            Console.Write("Enter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("VOLUME");  break;
                case "2": RunConvert("VOLUME");  break;
                case "3": RunAdd("VOLUME");      break;
                case "4": RunSubtract("VOLUME"); break;
                case "5": RunDivide("VOLUME");   break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        private void RunTemperatureMenu()
        {
            Console.WriteLine("\n--- Temperature Operations ---");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add (not supported)");
            Console.WriteLine("4. Subtract (not supported)");
            Console.WriteLine("5. Divide (not supported)");
            Console.Write("Enter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("TEMPERATURE");  break;
                case "2": RunConvert("TEMPERATURE");  break;
                case "3": RunAdd("TEMPERATURE");      break;
                case "4": RunSubtract("TEMPERATURE"); break;
                case "5": RunDivide("TEMPERATURE");   break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        private void RunHistoryMenu()
        {
            Console.WriteLine("\n--- Operation History ---");
            var all = _repo.GetAllMeasurements();

            if (all.Count == 0)
            {
                Console.WriteLine("No operations recorded yet.");
                return;
            }

            foreach (var entity in all)
                Console.WriteLine(entity.ToString());
        }

        // ── SHARED OPERATION RUNNERS ──────────────────────────────────────

        private void RunCompare(string category)
        {
            Console.WriteLine($"\n--- {category} Comparison ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"This Quantity: {v1} {u1}");
                Console.WriteLine($"That Quantity: {v2} {u2}");
                Console.WriteLine(PerformComparison(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private void RunConvert(string category)
        {
            Console.WriteLine($"\n--- {category} Conversion ---");
            try
            {
                Console.Write("Enter value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter source unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();
                Console.Write($"Enter target unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1     = new QuantityDTO(v1, u1, category);
                var target = new QuantityDTO(0,  u2, category);

                Console.WriteLine($"This Quantity: {v1} {u1}");
                Console.WriteLine($"Target Unit  : {u2}");
                Console.WriteLine(PerformConversion(q1, target));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private void RunAdd(string category)
        {
            Console.WriteLine($"\n--- {category} Addition ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"This Quantity: {v1} {u1}");
                Console.WriteLine($"That Quantity: {v2} {u2}");
                Console.WriteLine(PerformAddition(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private void RunSubtract(string category)
        {
            Console.WriteLine($"\n--- {category} Subtraction ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"This Quantity: {v1} {u1}");
                Console.WriteLine($"That Quantity: {v2} {u2}");
                Console.WriteLine(PerformSubtraction(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private void RunDivide(string category)
        {
            Console.WriteLine($"\n--- {category} Division ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"This Quantity: {v1} {u1}");
                Console.WriteLine($"That Quantity: {v2} {u2}");
                Console.WriteLine(PerformDivision(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        // ── INPUT HELPERS ─────────────────────────────────────────────────

        private static double ReadDouble()
        {
            string? input = Console.ReadLine();
            if (!double.TryParse(input, out double val))
                throw new ArgumentException("Invalid input: value must be numeric.");
            if (val < 0)
                throw new ArgumentException($"Value cannot be negative. Got: {val}");
            if (val > 1_000_000)
                throw new ArgumentException($"Value is too large (max 1,000,000). Got: {val}");
            return val;
        }

        private static string ReadUnit()
        {
            string? input = Console.ReadLine()?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Unit cannot be empty.");
            return input;
        }

        private static string GetUnitHint(string category)
            => category.ToUpperInvariant() switch
            {
                "LENGTH"      => "(FEET/FT/F, INCHES/IN/I, YARDS/YD/Y, CENTIMETERS/CM/C)",
                "WEIGHT"      => "(KILOGRAM/KG/K, GRAM/G, POUND/LB/P)",
                "VOLUME"      => "(LITRE/L, MILLILITRE/ML, GALLON/GAL)",
                "TEMPERATURE" => "(CELSIUS/C, FAHRENHEIT/F, KELVIN/K)",
                _             => ""
            };
    }
}