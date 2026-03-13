using QuantityMeasurementApp.Interface;
using QuantityMeasurementModel;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementRepository;

namespace QuantityMeasurementApp.Controller
{
    /// <summary>
    /// UC15: Controller Layer — Menu Driven with User Input.
    /// Accepts user input → builds QuantityDTO → calls service → displays result.
    /// Implements IQuantityMeasurementApp so Program depends only on the interface.
    /// No business logic here.
    /// </summary>
    public class QuantityMeasurementController : IQuantityMeasurementApp
    {
        private readonly IQuantityMeasurementService    _service;
        private readonly IQuantityMeasurementRepository _repo;

        public QuantityMeasurementController()
        {
            // -- Factory Pattern: create instances --------------------------
            _repo    = QuantityMeasurementCacheRepository.Instance;
            _service = new QuantityMeasurementServiceImpl(_repo);
        }

        public QuantityMeasurementController(IQuantityMeasurementService service, IQuantityMeasurementRepository repo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _repo    = repo    ?? throw new ArgumentNullException(nameof(repo));
        }

        // IQuantityMeasurementApp.Run() — Entry Point
    
        public void Run()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
               
                Console.WriteLine("-----Quantity Measurement App------"); 
                Console.WriteLine("1.  Length Operations ");
                Console.WriteLine("2.  Weight Operations ");
                Console.WriteLine("3.  Volume Operations ");
                Console.WriteLine("4.  Temperature Operations ");
                Console.WriteLine("5.  Operation History ");
                Console.WriteLine("6.  Exit  ");
               
                Console.Write("\nEnter your choice: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": RunLengthMenu();      break;
                    case "2": RunWeightMenu();      break;
                    case "3": RunVolumeMenu();      break;
                    case "4": RunTemperatureMenu(); break;
                    case "5": RunHistoryMenu();     break;
                    case "6":
                        running = false;
                        Console.WriteLine("\nThank you for using Quantity Measurement App.");
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                }
            }
        }

      
        // performXXX — Controller API Methods

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

        
        // LENGTH MENU

        private void RunLengthMenu()
        {
            Console.Clear();
            Console.WriteLine("|         Length Operations                |");
            Console.WriteLine("|  1. Compare                              |");
            Console.WriteLine("|  2. Convert                              |");
            Console.WriteLine("|  3. Add                                  |");
            Console.WriteLine("|  4. Subtract                             |");
            Console.WriteLine("|  5. Divide                               |");
            Console.Write("\nEnter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("LENGTH");   break;
                case "2": RunConvert("LENGTH");   break;
                case "3": RunAdd("LENGTH");       break;
                case "4": RunSubtract("LENGTH");  break;
                case "5": RunDivide("LENGTH");    break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

       
        // WEIGHT MENU

        private void RunWeightMenu()
        {
            Console.Clear();
            Console.WriteLine("-----Weight Operations-----");
            Console.WriteLine("1. Compare ");
            Console.WriteLine("2. Convert ");
            Console.WriteLine("3. Add  ");
            Console.WriteLine("4. Subtract ");
            Console.WriteLine("5. Divide ");
            Console.Write("\nEnter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("WEIGHT");   break;
                case "2": RunConvert("WEIGHT");   break;
                case "3": RunAdd("WEIGHT");       break;
                case "4": RunSubtract("WEIGHT");  break;
                case "5": RunDivide("WEIGHT");    break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

       
        // VOLUME MENU

        private void RunVolumeMenu()
        {
            Console.Clear();
           
            Console.WriteLine("-----Volume Operations-----");
            Console.WriteLine("1. Compare ");
            Console.WriteLine("2. Convert ");
            Console.WriteLine("3. Add  ");
            Console.WriteLine("4. Subtract ");
            Console.WriteLine("5. Divide ");
            
            Console.Write("\nEnter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("VOLUME");   break;
                case "2": RunConvert("VOLUME");   break;
                case "3": RunAdd("VOLUME");       break;
                case "4": RunSubtract("VOLUME");  break;
                case "5": RunDivide("VOLUME");    break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        // ════════════════════════════════════════════════════════════════
        // TEMPERATURE MENU
        // ════════════════════════════════════════════════════════════════

        private void RunTemperatureMenu()
        {
            Console.Clear();
            Console.WriteLine("------Temperature Operations------");
            Console.WriteLine("1. Compare ");
            Console.WriteLine("2. Convert ");
            Console.WriteLine("3. Add    (not supported)  ");
            Console.WriteLine("4. Subtract (not supported) ");
            Console.WriteLine("5. Divide   (not supported) ");
            Console.Write("\nEnter your choice: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("TEMPERATURE");   break;
                case "2": RunConvert("TEMPERATURE");   break;
                case "3": RunAdd("TEMPERATURE");       break;
                case "4": RunSubtract("TEMPERATURE");  break;
                case "5": RunDivide("TEMPERATURE");    break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        // ════════════════════════════════════════════════════════════════
        // HISTORY MENU
        // ════════════════════════════════════════════════════════════════

        private void RunHistoryMenu()
        {
            Console.Clear();
            Console.WriteLine("----- Operation History-----");
            var all = _repo.GetAllMeasurements();

            if (all.Count == 0)
            {
                Console.WriteLine("\n  No operations recorded yet.");
                return;
            }

            foreach (var entity in all)
                Console.WriteLine(entity.ToString());
        }

        // ════════════════════════════════════════════════════════════════
        // SHARED OPERATION RUNNERS
        // ════════════════════════════════════════════════════════════════

        private void RunCompare(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Comparison ---");
            Console.WriteLine("Operation: COMPARISON");

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

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformComparison(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }
        }

        private void RunConvert(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Conversion ---");
            Console.WriteLine("Operation: CONVERSION");

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

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"Target Unit   : {u2}");
                Console.WriteLine(PerformConversion(q1, target));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }
        }

        private void RunAdd(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Addition ---");
            Console.WriteLine("Operation: ADD");

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

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformAddition(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }
        }

        private void RunSubtract(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Subtraction ---");
            Console.WriteLine("Operation: SUBTRACT");

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

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformSubtraction(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }
        }

        private void RunDivide(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Division ---");
            Console.WriteLine("Operation: DIVIDE");

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

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformDivision(q1, q2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════════
        // INPUT HELPERS
        // ════════════════════════════════════════════════════════════════

        private static double ReadDouble()
        {
            string? input = Console.ReadLine();
            if (!double.TryParse(input, out double val))
                throw new ArgumentException("Invalid input: value must be numeric.");
            if (val < 0)
                throw new ArgumentException("Value cannot be negative. Please enter a positive number.");
            if (val > 1_000_000)
                throw new ArgumentException("Value is too large (max: 1,000,000). Please enter a realistic measurement.");
            return val;
        }

        private static string ReadUnit()
        {
            string? input = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Unit cannot be empty.");
            return input;
        }

        private static string GetUnitHint(string category)
            => category.ToUpperInvariant() switch
            {
                "LENGTH"      => "(feet/ft, inches/in, yards/yd, centimeters/cm)",
                "WEIGHT"      => "(kilogram/kg, gram/g, pound/lb)",
                "VOLUME"      => "(litre/l, millilitre/ml, gallon/gal)",
                "TEMPERATURE" => "(celsius/c, fahrenheit/f, kelvin/k)",
                _             => ""
            };
    }
}
