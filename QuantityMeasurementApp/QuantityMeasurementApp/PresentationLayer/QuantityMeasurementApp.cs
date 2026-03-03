using System;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    class QuantityMeasurementApp
    {
        private static double MaxLengthValue => QuantityLength.MaxLengthValue;

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
                Console.WriteLine("6. UC6 - Addition of Two Length Units");
                Console.WriteLine("7. UC7 - Addition with Target Unit");
                Console.WriteLine("8. UC9 - Weight Measurement (kg/g/lb)");
                Console.WriteLine("9. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1": CompareFeet(); break;
                    case "2": CompareInches(); break;
                    case "3": CompareUC3(); break;
                    case "4": CompareUC4(); break;
                    case "5": RunUC5Conversion(); break;
                    case "6": RunUC6Addition(); break;
                    case "7": RunUC7AdditionWithTargetUnit(); break;
                    case "8": RunUC9WeightMenu(); break;
                    case "9": return;
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

            Feet feetNumber1 = new Feet(v1);
            Feet feetNumber2 = new Feet(v2);
            Console.WriteLine(feetNumber1.Equals(feetNumber2) ? "Equal" : "Not Equal");
        }

        private static void CompareInches()
        {
            Console.Write("Enter first inch: ");
            double.TryParse(Console.ReadLine(), out double v1);
            Console.Write("Enter second inch: ");
            double.TryParse(Console.ReadLine(), out double v2);

            Inches inchesNumber1 = new Inches(v1);
            Inches inchesNumber2 = new Inches(v2);
            Console.WriteLine(inchesNumber1.Equals(inchesNumber2) ? "Equal" : "Not Equal");
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
                Console.WriteLine($"Error: Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.");
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
                Console.WriteLine($"Error: Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.");
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
                Console.WriteLine($"Error: Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.");
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
                Console.WriteLine($"Error: Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.");
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
            if (Math.Abs(value) > MaxLengthValue)
            {
                Console.WriteLine($"Error: Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.");
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

        /// <summary>UC6: Add two length quantities. Result is in the unit of the first operand.</summary>
        private static void RunUC6Addition()
        {
            Console.WriteLine("UC6 - Addition of Two Length Units (result in first operand's unit)");
            QuantityLength? length1 = ReadQuantityLength("First");
            if (length1 is null) return;
            QuantityLength? length2 = ReadQuantityLength("Second");
            if (length2 is null) return;
            try
            {
                QuantityLength sum = QuantityLength.Add(length1, length2);
                Console.WriteLine($"Result: add({length1}, {length2}) = {sum}");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /// <summary>UC7: Add two length quantities with an explicit target unit for the result.</summary>
        private static void RunUC7AdditionWithTargetUnit()
        {
            Console.WriteLine("UC7 - Addition with Target Unit (result in specified target unit)");
            QuantityLength? length1 = ReadQuantityLength("First");
            if (length1 is null) return;
            QuantityLength? length2 = ReadQuantityLength("Second");
            if (length2 is null) return;

            Console.Write("Enter target unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit targetUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }

            try
            {
                QuantityLength sum = QuantityLength.Add(length1, length2, targetUnit);
                Console.WriteLine($"Result: add({length1}, {length2}, {targetUnit}) = {sum}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static QuantityLength? ReadQuantityLength(string which)
        {
            Console.Write($"Enter {which} value: ");
            if (!double.TryParse(Console.ReadLine(), out double value))
            {
                Console.WriteLine("Error: Invalid number.");
                return null;
            }
            if (!double.IsFinite(value))
            {
                Console.WriteLine("Error: Value must be finite (no NaN or infinity).");
                return null;
            }
            if (Math.Abs(value) > MaxLengthValue)
            {
                Console.WriteLine($"Error: Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.");
                return null;
            }
            Console.Write($"Enter {which} unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit unit))
            {
                Console.WriteLine("Error: Invalid unit.");
                return null;
            }
            try
            {
                return new QuantityLength(value, unit);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
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

        // --------------------- UC9: Weight Measurement Menu ---------------------

        private static void RunUC9WeightMenu()
        {
            while (true)
            {
                Console.WriteLine("\n----- UC9: Weight Measurement (Kilogram / Gram / Pound) -----");
                Console.WriteLine("1. Equality Comparison");
                Console.WriteLine("2. Unit Conversion");
                Console.WriteLine("3. Addition (Implicit Target Unit)");
                Console.WriteLine("4. Addition (Explicit Target Unit)");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Enter your choice for UC9: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": RunUC9Equality(); break;
                    case "2": RunUC9Conversion(); break;
                    case "3": RunUC9AdditionImplicit(); break;
                    case "4": RunUC9AdditionExplicit(); break;
                    case "5": return;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }

        private static bool TryParseWeightUnit(string? input, out WeightUnit unit)
        {
            unit = WeightUnit.Kilogram;
            if (string.IsNullOrWhiteSpace(input)) return false;
            input = input.Trim().ToLower();

            if (input == "kg" || input == "kilogram" || input == "kilograms")
            {
                unit = WeightUnit.Kilogram;
                return true;
            }
            if (input == "g" || input == "gram" || input == "grams")
            {
                unit = WeightUnit.Gram;
                return true;
            }
            if (input == "lb" || input == "lbs" || input == "pound" || input == "pounds")
            {
                unit = WeightUnit.Pound;
                return true;
            }
            return false;
        }

        private static QuantityWeight? ReadQuantityWeight(string which)
        {
            Console.Write($"Enter {which} weight value: ");
            if (!double.TryParse(Console.ReadLine(), out double value))
            {
                Console.WriteLine("Error: Invalid number.");
                return null;
            }
            if (!double.IsFinite(value))
            {
                Console.WriteLine("Error: Value must be finite (no NaN or infinity).");
                return null;
            }
            if (Math.Abs(value) > QuantityWeight.MaxWeightValue)
            {
                Console.WriteLine($"Error: Value must be between -{QuantityWeight.MaxWeightValue:N0} and {QuantityWeight.MaxWeightValue:N0}.");
                return null;
            }

            Console.Write($"Enter {which} unit (Kilogram/Gram/Pound): ");
            if (!TryParseWeightUnit(Console.ReadLine(), out WeightUnit unit))
            {
                Console.WriteLine("Error: Invalid weight unit.");
                return null;
            }

            try
            {
                return new QuantityWeight(value, unit);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        private static void RunUC9Equality()
        {
            Console.WriteLine("\nUC9 - Weight Equality Comparison");
            QuantityWeight? w1 = ReadQuantityWeight("First");
            if (w1 is null) return;
            QuantityWeight? w2 = ReadQuantityWeight("Second");
            if (w2 is null) return;

            Console.WriteLine(w1.Equals(w2) ? "Equal" : "Not Equal");
        }

        private static void RunUC9Conversion()
        {
            Console.WriteLine("\nUC9 - Weight Unit Conversion");
            QuantityWeight? source = ReadQuantityWeight("Source");
            if (source is null) return;

            Console.Write("Enter target unit (Kilogram/Gram/Pound): ");
            if (!TryParseWeightUnit(Console.ReadLine(), out WeightUnit targetUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }

            QuantityWeight converted = source.ConvertTo(targetUnit);
            Console.WriteLine($"{source} → {converted}");
        }

        private static void RunUC9AdditionImplicit()
        {
            Console.WriteLine("\nUC9 - Addition (Implicit Target Unit = First Operand Unit)");
            QuantityWeight? w1 = ReadQuantityWeight("First");
            if (w1 is null) return;
            QuantityWeight? w2 = ReadQuantityWeight("Second");
            if (w2 is null) return;

            QuantityWeight sum = QuantityWeight.Add(w1, w2);
            Console.WriteLine($"Result: add({w1}, {w2}) = {sum}");
        }

        private static void RunUC9AdditionExplicit()
        {
            Console.WriteLine("\nUC9 - Addition (Explicit Target Unit)");
            QuantityWeight? w1 = ReadQuantityWeight("First");
            if (w1 is null) return;
            QuantityWeight? w2 = ReadQuantityWeight("Second");
            if (w2 is null) return;

            Console.Write("Enter target unit (Kilogram/Gram/Pound): ");
            if (!TryParseWeightUnit(Console.ReadLine(), out WeightUnit targetUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }

            QuantityWeight sum = QuantityWeight.Add(w1, w2, targetUnit);
            Console.WriteLine($"Result: add({w1}, {w2}, {targetUnit}) = {sum}");
        }

        // UC1/UC2 placeholders
        class Feet { private double val; public Feet(double v) => val = v; public bool Equals(Feet other) => Math.Abs(val - other.val) < 0.0001; }
        class Inches { private double val; public Inches(double v) => val = v; public bool Equals(Inches other) => Math.Abs(val - other.val) < 0.0001; }
    }
}