using System;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    class QuantityMeasurementApp
    {
        private const double MaxLengthValue = 100000;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n========== Quantity Measurement Menu ==========");
                Console.WriteLine("1. UC1 - Compare Feet");
                Console.WriteLine("2. UC2 - Compare Inches");
                Console.WriteLine("3. UC3 - Compare Feet & Inch (Generic)");
                Console.WriteLine("4. UC4 - Compare Yard & Centimeter");
                Console.WriteLine("5. UC5 - Unit Conversion (Same Type)");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1": CompareFeet(); break;
                    case "2": CompareInches(); break;
                    case "3": CompareUC3(); break;
                    case "4": CompareUC4(); break;
                    case "5": RunUC5Conversion(); break;
                    case "6": return;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }

        private static void CompareFeet()
        {
            Console.Write("Enter first feet: ");
            double.TryParse(Console.ReadLine(), out double v1);
            Console.Write("Enter second feet: ");
            double.TryParse(Console.ReadLine(), out double v2);

            Feet f1 = new Feet(v1);
            Feet f2 = new Feet(v2);
            Console.WriteLine(f1.Equals(f2) ? "Equal" : "Not Equal");
        }

        private static void CompareInches()
        {
            Console.Write("Enter first inch: ");
            double.TryParse(Console.ReadLine(), out double v1);
            Console.Write("Enter second inch: ");
            double.TryParse(Console.ReadLine(), out double v2);

            Inches i1 = new Inches(v1);
            Inches i2 = new Inches(v2);
            Console.WriteLine(i1.Equals(i2) ? "Equal" : "Not Equal");
        }

        private static void CompareUC3()
        {
            Console.Write("Enter first value: ");
            if (!double.TryParse(Console.ReadLine(), out double value1))
            {
                Console.WriteLine("Error: Invalid number. Please enter a valid numeric value.");
                return;
            }
            if (Math.Abs(value1) > MaxLengthValue)
            {
                Console.WriteLine("Error: Value too large. Please enter a value less than 1 trillion.");
                return;
            }
            Console.Write("Enter first unit (Feet/Inch): ");
            if (!TryParseUC3Unit(Console.ReadLine(), out LengthUnit unit1))
            {
                Console.WriteLine("Error: Invalid unit. Please enter 'Feet' or 'Inch'.");
                return;
            }

            Console.Write("Enter second value: ");
            if (!double.TryParse(Console.ReadLine(), out double value2))
            {
                Console.WriteLine("Error: Invalid number. Please enter a valid numeric value.");
                return;
            }
            if (Math.Abs(value2) > MaxLengthValue)
            {
                Console.WriteLine("Error: Value too large. Please enter a value less than 1 trillion.");
                return;
            }
            Console.Write("Enter second unit (Feet/Inch): ");
            if (!TryParseUC3Unit(Console.ReadLine(), out LengthUnit unit2))
            {
                Console.WriteLine("Error: Invalid unit. Please enter 'Feet' or 'Inch'.");
                return;
            }

            bool result = QuantityLengthService.Compare(value1, unit1, value2, unit2);
            Console.WriteLine(result ? "Equal" : "Not Equal");
        }

        private static void CompareUC4()
        {
            Console.Write("Enter first value: ");
            if (!double.TryParse(Console.ReadLine(), out double value1))
            {
                Console.WriteLine("Error: Invalid number. Please enter a valid numeric value.");
                return;
            }
            if (Math.Abs(value1) > MaxLengthValue)
            {
                Console.WriteLine("Error: Value too large. Please enter a value less than 1 trillion.");
                return;
            }
            Console.Write("Enter first unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit unit1))
            {
                Console.WriteLine("Error: Invalid unit. Please enter 'Feet', 'Inch', 'Yard', or 'Centimeter'.");
                return;
            }

            Console.Write("Enter second value: ");
            if (!double.TryParse(Console.ReadLine(), out double value2))
            {
                Console.WriteLine("Error: Invalid number. Please enter a valid numeric value.");
                return;
            }
            if (Math.Abs(value2) > MaxLengthValue)
            {
                Console.WriteLine("Error: Value too large. Please enter a value less than 1 trillion.");
                return;
            }
            Console.Write("Enter second unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit unit2))
            {
                Console.WriteLine("Error: Invalid unit. Please enter 'Feet', 'Inch', 'Yard', or 'Centimeter'.");
                return;
            }

            bool result = QuantityLengthService.Compare(value1, unit1, value2, unit2);
            Console.WriteLine(result ? "Equal" : "Not Equal");
        }

        /// <summary>UC5: Demonstrates conversion by value and units. Prints the converted numeric result.</summary>
        public static void DemonstrateLengthConversion(double value, LengthUnit fromUnit, LengthUnit toUnit)
        {
            double result = QuantityLengthService.Convert(value, fromUnit, toUnit);
            Console.WriteLine($"Convert({value}, {fromUnit}, {toUnit}) → {result}");
        }

        /// <summary>UC5: Demonstrates conversion from an existing QuantityLength to a target unit. Returns new instance.</summary>
        public static void DemonstrateLengthConversion(QuantityLength length, LengthUnit targetUnit)
        {
            QuantityLength converted = length.ConvertTo(targetUnit);
            Console.WriteLine($"{length} → {converted}");
        }

        private static void RunUC5Conversion()
        {
            Console.Write("Enter value to convert: ");
            if (!double.TryParse(Console.ReadLine(), out double value))
            {
                Console.WriteLine("Error: Invalid number.");
                return;
            }
            if (!double.IsFinite(value))
            {
                Console.WriteLine("Error: Value must be finite (no NaN or infinity).");
                return;
            }
            Console.Write("Enter source unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit fromUnit))
            {
                Console.WriteLine("Error: Invalid source unit.");
                return;
            }
            Console.Write("Enter target unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit toUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }
            try
            {
                DemonstrateLengthConversion(value, fromUnit, toUnit);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static bool TryParseUC3Unit(string? input, out LengthUnit unit)
        {
            unit = LengthUnit.Feet;
            if (string.IsNullOrWhiteSpace(input)) return false;
            input = input.Trim().ToLower();

            if (input == "feet" || input == "foot") return true;
            if (input == "inch" || input == "inches") { unit = LengthUnit.Inch; return true; }
            return false;
        }

        private static bool TryParseUC4Unit(string? input, out LengthUnit unit)
        {
            unit = LengthUnit.Feet;
            if (string.IsNullOrWhiteSpace(input)) return false;
            input = input.Trim().ToLower();

            if (input == "feet" || input == "foot") return true;
            if (input == "inch" || input == "inches") { unit = LengthUnit.Inch; return true; }
            if (input == "yard" || input == "yards") { unit = LengthUnit.Yard; return true; }
            if (input == "cm" || input == "cms" || input == "centimeter" || input == "centimeters") { unit = LengthUnit.Centimeter; return true; }
            return false;
        }

        // UC1/UC2 placeholders
        class Feet { private double val; public Feet(double v) => val = v; public bool Equals(Feet other) => Math.Abs(val - other.val) < 0.0001; }
        class Inches { private double val; public Inches(double v) => val = v; public bool Equals(Inches other) => Math.Abs(val - other.val) < 0.0001; }
    }
}