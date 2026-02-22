using System;
using QuantityMeasurementApp.ServiceLayer;
using QuantityMeasurementApp.DomainLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    class QuantityMeasurementApp
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Quantity Measurement Menu ---");
                Console.WriteLine("1. Compare Two Feet Values (UC-1)");
                Console.WriteLine("2. Compare Feet and Inches (UC-2)");
                Console.WriteLine("3. Compare Generic Quantity (UC-3)");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        CompareFeet();
                        break;

                    case 2:
                        CompareFeetAndInches();
                        break;

                    case 3:
                        CompareGenericQuantity();
                        break;

                    case 4:
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        // ---------------- UC1 ----------------
        private static void CompareFeet()
        {
            try
            {
                Console.Write("Enter first feet value: ");
                if (!double.TryParse(Console.ReadLine(), out double value1))
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }

                Console.Write("Enter second feet value: ");
                if (!double.TryParse(Console.ReadLine(), out double value2))
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }

                bool result = QuantityMeasurementService.CompareFeet(value1, value2);
                Console.WriteLine(result ? "Equal" : "Not Equal");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // ---------------- UC2 ----------------
        private static void CompareFeetAndInches()
        {
            try
            {
                Console.Write("Enter feet value: ");
                string? feetInput = Console.ReadLine();

                if (!double.TryParse(feetInput, out double feet))
                {
                    Console.WriteLine("Invalid feet input. Skipping feet comparison.");
                    CompareOnlyInches();
                    return;
                }

                Console.Write("Enter inches value: ");
                if (!double.TryParse(Console.ReadLine(), out double inches))
                {
                    Console.WriteLine("Invalid inches input.");
                    return;
                }

                bool result = QuantityMeasurementService.CompareFeetAndInches(feet, inches);
                Console.WriteLine(result ? "Equal" : "Not Equal");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void CompareOnlyInches()
        {
            try
            {
                Console.Write("Enter first inches value: ");
                if (!double.TryParse(Console.ReadLine(), out double value1))
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }

                Console.Write("Enter second inches value: ");
                if (!double.TryParse(Console.ReadLine(), out double value2))
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }

                bool result = QuantityMeasurementService.CompareInches(value1, value2);
                Console.WriteLine(result ? "Equal" : "Not Equal");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // ---------------- UC3 ----------------
        private static void CompareGenericQuantity()
        {
            try
            {
                Console.Write("Enter first value: ");
                if (!double.TryParse(Console.ReadLine(), out double value1))
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }

                Console.Write("Enter unit (Feet/Inch): ");
                if (!Enum.TryParse(Console.ReadLine(), true, out LengthUnit unit1))
                {
                    Console.WriteLine("Invalid unit.");
                    return;
                }

                Console.Write("Enter second value: ");
                if (!double.TryParse(Console.ReadLine(), out double value2))
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }

                Console.Write("Enter unit (Feet/Inch): ");
                if (!Enum.TryParse(Console.ReadLine(), true, out LengthUnit unit2))
                {
                    Console.WriteLine("Invalid unit.");
                    return;
                }

                bool result = QuantityLengthService.Compare(value1, unit1, value2, unit2);
                Console.WriteLine(result ? "Equal" : "Not Equal");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}