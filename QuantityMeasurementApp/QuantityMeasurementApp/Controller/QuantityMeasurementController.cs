using Microsoft.Extensions.Configuration;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementModel;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementRepository;
using QuantityMeasurementBusinessLayer.Exception;
using QuantityMeasurementRepository.Database;
using QuantityMeasurementRepository.Util;

namespace QuantityMeasurementApp.Controller
{
    /// <summary>
    /// UC15/UC16/UC17: Controller Layer — Menu Driven with User Input.
    /// Handles startup (config load, repo selection), wiring, and all menus.
    /// Program.cs calls only QuantityMeasurementController.Start().
    ///
    /// Repository menu:
    ///   1. Redis Storage    — stores every operation in Redis (localhost:6379)
    ///   2. Cache Repository — SQL Server / SSMS via ADO.NET
    /// </summary>
    public class QuantityMeasurementController : IQuantityMeasurementApp
    {
        private readonly IQuantityMeasurementService    _service;
        private readonly IQuantityMeasurementRepository _repo;

        // ── Public constructor — used by tests and Start() ─────────────────
        public QuantityMeasurementController(IQuantityMeasurementService service,
                                              IQuantityMeasurementRepository repo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _repo    = repo    ?? throw new ArgumentNullException(nameof(repo));
        }

        // ════════════════════════════════════════════════════════════════
        // STATIC ENTRY POINT — called from Program.cs
        // ════════════════════════════════════════════════════════════════

        public static void Start()
        {
            // -- Load appsettings.json
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            // -- Show SELECT REPOSITORY TYPE menu
            IQuantityMeasurementRepository repository = SelectRepository(config);

            Console.WriteLine($"[App] {repository.GetRepositoryInfo()}");

            // -- Wire service and controller
            var service    = new QuantityMeasurementServiceImpl(repository);
            var controller = new QuantityMeasurementController(service, repository);

            // -- Run the main application
            controller.Run();

            // -- Shutdown report
            Console.WriteLine("\n[App] === Measurement Report ===");
            var all = repository.GetAllMeasurements();
            Console.WriteLine($"[App] Total records: {all.Count}");
            foreach (var entity in all)
                Console.WriteLine(entity.ToString());
            Console.WriteLine("[App] === End of Report ===");

            repository.ReleaseResources();
            Console.WriteLine("[App] Shutdown complete.");
        }

        // ════════════════════════════════════════════════════════════════
        // REPOSITORY SELECTION MENU
        // ════════════════════════════════════════════════════════════════

        private static IQuantityMeasurementRepository SelectRepository(IConfiguration config)
        {
            while (true)
            {
                Console.WriteLine("======================================================");
                Console.WriteLine("              SELECT REPOSITORY TYPE                  ");
                Console.WriteLine("======================================================");
                Console.WriteLine("1. In-Memory / JSON Cache  (no database needed)");
                Console.WriteLine("2. SQL Server Database     (requires SSMS connection)");
                Console.WriteLine("======================================================");
                Console.Write("\nChoice: ");

                string? choice = Console.ReadLine()?.Trim();

                if (choice == "1")
                {
                    Console.WriteLine("\n[Program] In-Memory Cache Repository selected V");
                    Console.WriteLine("[ApplicationConfig] Repository type: cache");
                    return QuantityMeasurementCacheRepository.Instance;
                }
                else if (choice == "2")
                {
                    Console.WriteLine("\n[Program] Database Repository selected V");
                    Console.WriteLine("[ApplicationConfig] Repository type: database");
                    Console.WriteLine("[App] Connecting to SQL Server (SSMS)...");
                    var dbConfig = new DatabaseConfig(config);
                    return new QuantityMeasurementDatabaseRepository(dbConfig);
                }
                else
                {
                    Console.WriteLine("\n[ERROR] Invalid choice. Please enter 1 or 2.\n");
                }
            }
        }

        // ════════════════════════════════════════════════════════════════
        // MAIN MENU — IQuantityMeasurementApp.Run()
        // ════════════════════════════════════════════════════════════════

        public void Run()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("============QUANTITY MEASUREMENT APPLICATION============");
                Console.WriteLine("1.  Length Operations");
                Console.WriteLine("2.  Weight Operations");
                Console.WriteLine("3.  Volume Operations");
                Console.WriteLine("4.  Temperature Operations");
                Console.WriteLine("5.  Run All Demonstrations");
                Console.WriteLine("6.  Operation History");
                Console.WriteLine("0.  Exit");
                Console.WriteLine("======================================================");
                Console.Write("\nSelect Option: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": RunLengthMenu();        break;
                    case "2": RunWeightMenu();        break;
                    case "3": RunVolumeMenu();        break;
                    case "4": RunTemperatureMenu();   break;
                    case "5": RunAllDemonstrations(); break;
                    case "6": RunHistoryMenu();       break;
                    case "0":
                        running = false;
                        Console.WriteLine("\n[App] Resources released.");
                        Console.WriteLine("Thank you for using Quantity Measurement App.");
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

        // ════════════════════════════════════════════════════════════════
        // PUBLIC PERFORM METHODS — Controller API
        // ════════════════════════════════════════════════════════════════

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

        // ════════════════════════════════════════════════════════════════
        // LENGTH MENU
        // ════════════════════════════════════════════════════════════════

        private void RunLengthMenu()
        {
            Console.Clear();
            Console.WriteLine("----- Length Operations-----");
            Console.WriteLine("1. Compare ");
            Console.WriteLine("2. Convert ");
            Console.WriteLine("3. Add  ");
            Console.WriteLine("4. Subtract ");
            Console.WriteLine("5. Divide |");
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

        // ════════════════════════════════════════════════════════════════
        // WEIGHT MENU
        // ════════════════════════════════════════════════════════════════

        private void RunWeightMenu()
        {
            Console.Clear();
            Console.WriteLine("-----Weight Operations-----");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
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

        // ════════════════════════════════════════════════════════════════
        // VOLUME MENU
        // ════════════════════════════════════════════════════════════════

        private void RunVolumeMenu()
        {
            Console.Clear();
            Console.WriteLine("-----Volume Operations-----");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
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
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add    (not supported)");
            Console.WriteLine("4. Subtract (not supported)");
            Console.WriteLine("5. Divide   (not supported)");
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
        // RUN ALL DEMONSTRATIONS
        // ════════════════════════════════════════════════════════════════

        private void RunAllDemonstrations()
        {
            Console.Clear();
            Console.WriteLine("======================================================");
            Console.WriteLine("               RUN ALL DEMONSTRATIONS                 ");
            Console.WriteLine("======================================================");
            Console.WriteLine("\n[Demo] This will run sample operations for all");
            Console.WriteLine("       measurement categories using preset values.");
            Console.WriteLine("\n[Demo] Running Length Demonstration...");
            DemoConvert("LENGTH", 12, "in", "ft");

            Console.WriteLine("\n[Demo] Running Weight Demonstration...");
            DemoConvert("WEIGHT", 1000, "g", "kg");

            Console.WriteLine("\n[Demo] Running Volume Demonstration...");
            DemoConvert("VOLUME", 1, "gal", "l");

            Console.WriteLine("\n[Demo] Running Temperature Demonstration...");
            DemoConvert("TEMPERATURE", 100, "c", "f");

            Console.WriteLine("\n======================================================");
            Console.WriteLine("[Demo] All demonstrations completed successfully.");
            Console.WriteLine("======================================================");
        }

        private void DemoConvert(string category, double value, string fromUnit, string toUnit)
        {
            try
            {
                var q1     = new QuantityDTO(value, fromUnit, category);
                var target = new QuantityDTO(0, toUnit, category);
                string result = PerformConversion(q1, target);
                Console.WriteLine($"  [{category}] {value} {fromUnit} -> {result}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"  [{category}] Demo failed: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════════
        // HISTORY MENU
        // ════════════════════════════════════════════════════════════════

        private void RunHistoryMenu()
        {
            bool historyRunning = true;
            while (historyRunning)
            {
                Console.Clear();
                Console.WriteLine("===========OPERATION HISTORY==========");
                Console.WriteLine("1.  View All History");
                Console.WriteLine("2.  View By Operation Type");
                Console.WriteLine("3.  View By Measurement Type");
                Console.WriteLine("4.  View Statistics");
                Console.WriteLine("5.  Clear All Records");
                Console.WriteLine("0.  Exit");
                Console.WriteLine("======================================================");
                Console.Write("\nSelect Option: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":  ViewAllHistory();        break;
                    case "2":  ViewByOperationType();   break;
                    case "3":  ViewByMeasurementType(); break;
                    case "4":  ViewStatistics();        break;
                    case "5":  ClearAllRecords();       break;
                    case "0":
                        historyRunning = false;
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice.");
                        break;
                }

                if (historyRunning)
                {
                    Console.WriteLine("\nPress any key to return to history menu...");
                    Console.ReadKey();
                }
            }
        }

        private void ViewAllHistory()
        {
            Console.Clear();
            Console.WriteLine("=========VIEW ALL HISTORY========== ");
            var all = _repo.GetAllMeasurements();
            if (all.Count == 0)
            {
                Console.WriteLine("\n  No operations recorded yet.");
                return;
            }

            Console.WriteLine($"\n  Total Records: {all.Count}\n");
            foreach (var entity in all)
                Console.WriteLine(entity.ToString());
        }

        private void ViewByOperationType()
        {
            Console.Clear();
            Console.WriteLine("==========VIEW BY OPERATION TYPE===========");
            Console.WriteLine("  Available types: COMPARE, CONVERT, ADD, SUBTRACT, DIVIDE");
            Console.Write("\n  Enter operation type: ");
            string? opType = Console.ReadLine()?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(opType))
            {
                Console.WriteLine("\n  Operation type cannot be empty.");
                return;
            }

            var results = _repo.GetByOperationType(opType);

            if (results.Count == 0)
            {
                Console.WriteLine($"\n  No records found for operation type: {opType}");
                return;
            }

            Console.WriteLine($"\n  Records for [{opType}] - Total: {results.Count}\n");
            foreach (var entity in results)
                Console.WriteLine(entity.ToString());
        }

        private void ViewByMeasurementType()
        {
            Console.Clear();
            Console.WriteLine("===========VIEW BY MEASUREMENT TYPE==========");
            Console.WriteLine("  Available categories: LENGTH, WEIGHT, VOLUME, TEMPERATURE");
            Console.Write("\n  Enter measurement category: ");
            string? category = Console.ReadLine()?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(category))
            {
                Console.WriteLine("\n  Category cannot be empty.");
                return;
            }

            var results = _repo.GetByCategory(category);

            if (results.Count == 0)
            {
                Console.WriteLine($"\n  No records found for category: {category}");
                return;
            }

            Console.WriteLine($"\n  Records for [{category}] - Total: {results.Count}\n");
            foreach (var entity in results)
                Console.WriteLine(entity.ToString());
        }

        private void ViewStatistics()
        {
            Console.Clear();
            Console.WriteLine("=============VIEW STATISTICS==============");
            int total = _repo.GetTotalCount();
            var all   = _repo.GetAllMeasurements();

            Console.WriteLine($"\n  Total Operations Recorded : {total}");

            if (total == 0)
            {
                Console.WriteLine("\n  No data available for statistics.");
                return;
            }

            var byCategory = all
                .GroupBy(e => e.MeasurementCategory)
                .OrderByDescending(g => g.Count());

            Console.WriteLine("\n  --- Operations by Category ---");
            foreach (var group in byCategory)
                Console.WriteLine($"  {group.Key,-15}: {group.Count()} operation(s)");

            var byOpType = all
                .GroupBy(e => e.OperationType)
                .OrderByDescending(g => g.Count());

            Console.WriteLine("\n  --- Operations by Type ---");
            foreach (var group in byOpType)
                Console.WriteLine($"  {group.Key,-15}: {group.Count()} operation(s)");

            int errorCount = all.Count(e => e.HasError);
            Console.WriteLine($"\n  Successful Operations     : {total - errorCount}");
            Console.WriteLine($"  Failed Operations         : {errorCount}");
            Console.WriteLine($"\n  Repository Info           : {_repo.GetRepositoryInfo()}");
        }

        private void ClearAllRecords()
        {
            Console.Clear();
            Console.WriteLine("============CLEAR ALL RECORDS=============");
            Console.WriteLine("\n  WARNING: This will permanently delete all history.");
            Console.Write("  Are you sure? (yes/no): ");
            string? confirm = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (confirm == "yes")
            {
                _repo.Clear();
                Console.WriteLine("\n  [Repository] Resources released.");
                Console.WriteLine("  All records cleared successfully.");
            }
            else
            {
                Console.WriteLine("\n  Operation cancelled. No records were deleted.");
            }
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
            catch (System.Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
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
            catch (System.Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
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
            catch (System.Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
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
            catch (System.Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
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
            catch (System.Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
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