using System;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    class QuantityMeasurementApp
    {
        private const double MaxLengthValue = 100000;
        private const double MaxWeightValue = 100000;

        public static void Run()
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
                Console.WriteLine("9. UC11 - Volume Measurement (L/mL/gallon)");
                Console.WriteLine("10. UC12 - Subtraction & Division Operations");
                Console.WriteLine("11. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":  CompareFeet();                break;
                    case "2":  CompareInches();              break;
                    case "3":  CompareUC3();                 break;
                    case "4":  CompareUC4();                 break;
                    case "5":  RunUC5Conversion();           break;
                    case "6":  RunUC6Addition();             break;
                    case "7":  RunUC7AdditionWithTargetUnit(); break;
                    case "8":  RunUC9WeightMenu();           break;
                    case "9":  RunUC11VolumeMenu();          break;
                    case "10": RunUC12ArithmeticMenu();      break;
                    case "11": return;
                    default:   Console.WriteLine("Invalid choice"); break;
                }
            }
        }

        // ── helpers ──────────────────────────────────────────────────────
        private static LengthUnitMeasurable L(LengthUnit u) => new LengthUnitMeasurable(u);
        private static WeightUnitMeasurable W(WeightUnit u) => new WeightUnitMeasurable(u);

        // ── UC1 ──────────────────────────────────────────────────────────
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

        // ── UC2 ──────────────────────────────────────────────────────────
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

        // ── UC3 ──────────────────────────────────────────────────────────
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

        // ── UC4 ──────────────────────────────────────────────────────────
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

        // ── UC5 ──────────────────────────────────────────────────────────
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
                double result = QuantityLengthService.Convert(value, fromUnit, toUnit);
                Console.WriteLine($"Convert({value}, {fromUnit}, {toUnit}) → {result}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // ── UC6 ──────────────────────────────────────────────────────────
        private static void RunUC6Addition()
        {
            Console.WriteLine("UC6 - Addition of Two Length Units (result in first operand's unit)");
            var length1 = ReadQuantityLength("First");
            if (length1 is null) return;
            var length2 = ReadQuantityLength("Second");
            if (length2 is null) return;
            try
            {
                var sum = Quantity<LengthUnitMeasurable>.Add(length1, length2);
                Console.WriteLine($"Result: add({length1}, {length2}) = {sum}");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // ── UC7 ──────────────────────────────────────────────────────────
        private static void RunUC7AdditionWithTargetUnit()
        {
            Console.WriteLine("UC7 - Addition with Target Unit (result in specified target unit)");
            var length1 = ReadQuantityLength("First");
            if (length1 is null) return;
            var length2 = ReadQuantityLength("Second");
            if (length2 is null) return;

            Console.Write("Enter target unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit targetUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }

            try
            {
                var sum = Quantity<LengthUnitMeasurable>.Add(length1, length2, L(targetUnit));
                Console.WriteLine($"Result: add({length1}, {length2}, {targetUnit}) = {sum}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // ── Helpers: read length / weight ────────────────────────────────
        private static Quantity<LengthUnitMeasurable>? ReadQuantityLength(string which)
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
                return new Quantity<LengthUnitMeasurable>(value, L(unit));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        private static Quantity<WeightUnitMeasurable>? ReadQuantityWeight(string which)
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
            if (Math.Abs(value) > MaxWeightValue)
            {
                Console.WriteLine($"Error: Value must be between -{MaxWeightValue:N0} and {MaxWeightValue:N0}.");
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
                return new Quantity<WeightUnitMeasurable>(value, W(unit));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        // ── UC9 menu ─────────────────────────────────────────────────────
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
                    case "1": RunUC9Equality();         break;
                    case "2": RunUC9Conversion();       break;
                    case "3": RunUC9AdditionImplicit(); break;
                    case "4": RunUC9AdditionExplicit(); break;
                    case "5": return;
                    default:  Console.WriteLine("Invalid choice"); break;
                }
            }
        }

        private static void RunUC9Equality()
        {
            Console.WriteLine("\nUC9 - Weight Equality Comparison");
            var w1 = ReadQuantityWeight("First");
            if (w1 is null) return;
            var w2 = ReadQuantityWeight("Second");
            if (w2 is null) return;
            Console.WriteLine(w1.Equals(w2) ? "Equal" : "Not Equal");
        }

        private static void RunUC9Conversion()
        {
            Console.WriteLine("\nUC9 - Weight Unit Conversion");
            var source = ReadQuantityWeight("Source");
            if (source is null) return;

            Console.Write("Enter target unit (Kilogram/Gram/Pound): ");
            if (!TryParseWeightUnit(Console.ReadLine(), out WeightUnit targetUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }

            var converted = source.ConvertTo(W(targetUnit));
            Console.WriteLine($"{source} → {converted}");
        }

        private static void RunUC9AdditionImplicit()
        {
            Console.WriteLine("\nUC9 - Addition (Implicit Target Unit = First Operand Unit)");
            var w1 = ReadQuantityWeight("First");
            if (w1 is null) return;
            var w2 = ReadQuantityWeight("Second");
            if (w2 is null) return;

            var sum = Quantity<WeightUnitMeasurable>.Add(w1, w2);
            Console.WriteLine($"Result: add({w1}, {w2}) = {sum}");
        }

        private static void RunUC9AdditionExplicit()
        {
            Console.WriteLine("\nUC9 - Addition (Explicit Target Unit)");
            var w1 = ReadQuantityWeight("First");
            if (w1 is null) return;
            var w2 = ReadQuantityWeight("Second");
            if (w2 is null) return;

            Console.Write("Enter target unit (Kilogram/Gram/Pound): ");
            if (!TryParseWeightUnit(Console.ReadLine(), out WeightUnit targetUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }

            var sum = Quantity<WeightUnitMeasurable>.Add(w1, w2, W(targetUnit));
            Console.WriteLine($"Result: add({w1}, {w2}, {targetUnit}) = {sum}");
        }

        // ── Unit parsers ─────────────────────────────────────────────────
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

        private static bool TryParseWeightUnit(string? input, out WeightUnit unit)
        {
            unit = WeightUnit.Kilogram;
            if (string.IsNullOrWhiteSpace(input)) return false;
            input = input.Trim().ToLower();
            if (input == "kg" || input == "kilogram" || input == "kilograms") { unit = WeightUnit.Kilogram; return true; }
            if (input == "g"  || input == "gram"     || input == "grams")     { unit = WeightUnit.Gram;     return true; }
            if (input == "lb" || input == "lbs"      || input == "pound" || input == "pounds") { unit = WeightUnit.Pound; return true; }
            return false;
        }

        // ── UC11: Volume Measurement Menu ─────────────────────────────────

        private static void RunUC11VolumeMenu()
        {
            while (true)
            {
                Console.WriteLine("\n----- UC11: Volume Measurement (Litre / Millilitre / Gallon) -----");
                Console.WriteLine($"  Valid range: 0 to {QuantityVolume.MaxVolumeValue:N0} (any unit)");
                Console.WriteLine("1. Equality Comparison");
                Console.WriteLine("2. Unit Conversion");
                Console.WriteLine("3. Addition (Implicit Target Unit)");
                Console.WriteLine("4. Addition (Explicit Target Unit)");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Enter your choice for UC11: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": RunUC11Equality();         break;
                    case "2": RunUC11Conversion();       break;
                    case "3": RunUC11AdditionImplicit(); break;
                    case "4": RunUC11AdditionExplicit(); break;
                    case "5": return;
                    default:  Console.WriteLine("Invalid choice. Please enter 1-5."); break;
                }
            }
        }

        private static VolumeUnit ReadVolumeUnit(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim().ToLower();
                if (input == "l" || input == "litre" || input == "liter" || input == "litres")
                    return VolumeUnit.Litre;
                if (input == "ml" || input == "millilitre" || input == "milliliter" || input == "millilitres")
                    return VolumeUnit.Millilitre;
                if (input == "gal" || input == "gallon" || input == "gallons")
                    return VolumeUnit.Gallon;
                Console.WriteLine("  Error: Invalid unit. Please enter Litre, Millilitre, or Gallon.");
            }
        }

        private static QuantityVolume ReadQuantityVolume(string which)
        {
            while (true)
            {
                Console.Write($"Enter {which} volume value (0 to {QuantityVolume.MaxVolumeValue:N0}): ");
                string? raw = Console.ReadLine();

                if (!double.TryParse(raw, out double value))
                {
                    Console.WriteLine("  Error: Not a valid number. Please try again.");
                    continue;
                }
                if (!double.IsFinite(value))
                {
                    Console.WriteLine("  Error: Value must be finite (no NaN or Infinity). Please try again.");
                    continue;
                }
                if (Math.Abs(value) > QuantityVolume.MaxVolumeValue)
                {
                    Console.WriteLine($"  Error: Value {value:N0} is out of range. " +
                                      $"Allowed range: 0 to {QuantityVolume.MaxVolumeValue:N0}. Please try again.");
                    continue;
                }

                VolumeUnit unit = ReadVolumeUnit(
                    $"Enter {which} unit (Litre / Millilitre / Gallon): ");

                try
                {
                    return new QuantityVolume(value, unit);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"  Error: {ex.Message}. Please try again.");
                }
            }
        }

        private static void RunUC11Equality()
        {
            Console.WriteLine("\nUC11 - Volume Equality Comparison");
            var v1 = ReadQuantityVolume("First");
            var v2 = ReadQuantityVolume("Second");
            Console.WriteLine(v1.Equals(v2) ? "Result: Equal" : "Result: Not Equal");
        }

        private static void RunUC11Conversion()
        {
            Console.WriteLine("\nUC11 - Volume Unit Conversion");
            var source     = ReadQuantityVolume("Source");
            var targetUnit = ReadVolumeUnit("Enter target unit (Litre / Millilitre / Gallon): ");
            var converted  = source.ConvertTo(targetUnit);
            Console.WriteLine($"Result: {source} → {converted}");
        }

        private static void RunUC11AdditionImplicit()
        {
            Console.WriteLine("\nUC11 - Addition (result in first operand's unit)");
            var v1  = ReadQuantityVolume("First");
            var v2  = ReadQuantityVolume("Second");
            var sum = QuantityVolume.Add(v1, v2);
            Console.WriteLine($"Result: {v1} + {v2} = {sum}");
        }

        private static void RunUC11AdditionExplicit()
        {
            Console.WriteLine("\nUC11 - Addition (Explicit Target Unit)");
            var v1         = ReadQuantityVolume("First");
            var v2         = ReadQuantityVolume("Second");
            var targetUnit = ReadVolumeUnit("Enter target unit (Litre / Millilitre / Gallon): ");
            var sum        = QuantityVolume.Add(v1, v2, targetUnit);
            Console.WriteLine($"Result: {v1} + {v2} expressed in {targetUnit} = {sum}");
        }

        // ── UC12: Subtraction & Division Menu ────────────────────────────

        private static void RunUC12ArithmeticMenu()
        {
            while (true)
            {
                Console.WriteLine("\n----- UC12: Subtraction & Division Operations -----");
                Console.WriteLine("1. Subtraction – Length (Implicit Target Unit)");
                Console.WriteLine("2. Subtraction – Length (Explicit Target Unit)");
                Console.WriteLine("3. Subtraction – Weight");
                Console.WriteLine("4. Subtraction – Volume");
                Console.WriteLine("5. Division – Length");
                Console.WriteLine("6. Division – Weight");
                Console.WriteLine("7. Division – Volume");
                Console.WriteLine("8. Back to Main Menu");
                Console.Write("Enter your choice for UC12: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": RunUC12SubtractLengthImplicit(); break;
                    case "2": RunUC12SubtractLengthExplicit(); break;
                    case "3": RunUC12SubtractWeight();         break;
                    case "4": RunUC12SubtractVolume();         break;
                    case "5": RunUC12DivideLength();           break;
                    case "6": RunUC12DivideWeight();           break;
                    case "7": RunUC12DivideVolume();           break;
                    case "8": return;
                    default:  Console.WriteLine("Invalid choice. Please enter 1-8."); break;
                }
            }
        }

        private static void RunUC12SubtractLengthImplicit()
        {
            Console.WriteLine("\nUC12 – Subtraction: Length (result in first operand's unit)");
            var a = ReadQuantityLength("First");
            if (a is null) return;
            var b = ReadQuantityLength("Second");
            if (b is null) return;
            try
            {
                var result = a.Subtract(b);
                Console.WriteLine($"Result: {a} − {b} = {result}");
            }
            catch (ArgumentException ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private static void RunUC12SubtractLengthExplicit()
        {
            Console.WriteLine("\nUC12 – Subtraction: Length (Explicit Target Unit)");
            var a = ReadQuantityLength("First");
            if (a is null) return;
            var b = ReadQuantityLength("Second");
            if (b is null) return;
            Console.Write("Enter target unit (Feet/Inch/Yard/Centimeter): ");
            if (!TryParseUC4Unit(Console.ReadLine(), out LengthUnit targetUnit))
            {
                Console.WriteLine("Error: Invalid target unit.");
                return;
            }
            try
            {
                var result = a.Subtract(b, L(targetUnit));
                Console.WriteLine($"Result: {a} − {b} expressed in {targetUnit} = {result}");
            }
            catch (ArgumentException ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private static void RunUC12SubtractWeight()
        {
            Console.WriteLine("\nUC12 – Subtraction: Weight (result in first operand's unit)");
            var a = ReadQuantityWeight("First");
            if (a is null) return;
            var b = ReadQuantityWeight("Second");
            if (b is null) return;
            try
            {
                var result = a.Subtract(b);
                Console.WriteLine($"Result: {a} − {b} = {result}");
            }
            catch (ArgumentException ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private static void RunUC12SubtractVolume()
        {
            Console.WriteLine("\nUC12 – Subtraction: Volume (result in first operand's unit)");
            var a = ReadQuantityVolume("First");
            var b = ReadQuantityVolume("Second");
            try
            {
                var aInner = new Quantity<VolumeUnitMeasurable>(
                    a.Value, new VolumeUnitMeasurable(a.Unit));
                var bInner = new Quantity<VolumeUnitMeasurable>(
                    b.Value, new VolumeUnitMeasurable(b.Unit));
                var result = aInner.Subtract(bInner);
                Console.WriteLine($"Result: {a} − {b} = {result}");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private static void RunUC12DivideLength()
        {
            Console.WriteLine("\nUC12 – Division: Length (dimensionless ratio)");
            var a = ReadQuantityLength("Dividend (first)");
            if (a is null) return;
            var b = ReadQuantityLength("Divisor (second)");
            if (b is null) return;
            try
            {
                double ratio = a.Divide(b);
                Console.WriteLine($"Result: {a} ÷ {b} = {ratio}");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private static void RunUC12DivideWeight()
        {
            Console.WriteLine("\nUC12 – Division: Weight (dimensionless ratio)");
            var a = ReadQuantityWeight("Dividend (first)");
            if (a is null) return;
            var b = ReadQuantityWeight("Divisor (second)");
            if (b is null) return;
            try
            {
                double ratio = a.Divide(b);
                Console.WriteLine($"Result: {a} ÷ {b} = {ratio}");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private static void RunUC12DivideVolume()
        {
            Console.WriteLine("\nUC12 – Division: Volume (dimensionless ratio)");
            var a = ReadQuantityVolume("Dividend (first)");
            var b = ReadQuantityVolume("Divisor (second)");
            try
            {
                var aInner = new Quantity<VolumeUnitMeasurable>(
                    a.Value, new VolumeUnitMeasurable(a.Unit));
                var bInner = new Quantity<VolumeUnitMeasurable>(
                    b.Value, new VolumeUnitMeasurable(b.Unit));
                double ratio = aInner.Divide(bInner);
                Console.WriteLine($"Result: {a} ÷ {b} = {ratio}");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        // ── UC1/UC2 placeholder inner classes ────────────────────────────
        class Feet   { private double val; public Feet(double v)   => val = v; public bool Equals(Feet other)   => Math.Abs(val - other.val) < 0.0001; }
        class Inches { private double val; public Inches(double v) => val = v; public bool Equals(Inches other) => Math.Abs(val - other.val) < 0.0001; }
    }
}