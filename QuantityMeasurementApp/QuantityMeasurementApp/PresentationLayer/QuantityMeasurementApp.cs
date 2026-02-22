using System;
using QuantityMeasurementApp.ServiceLayer;

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
                Console.WriteLine("2. Compare Feet AND Inches (UC-2)");
                Console.WriteLine("3. Exit");
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
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        // ---------------- UC-1 ----------------
        private static void CompareFeet()
        {
            try
            {
                Console.Write("Enter first feet value: ");
                if (!double.TryParse(Console.ReadLine(), out double value1))
                {
                    Console.WriteLine("Invalid numeric input.");
                    return;
                }

                Console.Write("Enter second feet value: ");
                if (!double.TryParse(Console.ReadLine(), out double value2))
                {
                    Console.WriteLine("Invalid numeric input.");
                    return;
                }

                bool result = QuantityMeasurementService.CompareFeet(value1, value2);

                Console.WriteLine(result
                    ? "Both feet values are equal."
                    : "Feet values are NOT equal.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // ---------------- UC-2 ----------------
        private static void CompareFeetAndInches()
{
    double feet1, feet2;
    double inches1, inches2;

    // ---------------- FEET SECTION ----------------
    Console.WriteLine("\n--- Feet Comparison ---");

    Console.Write("Enter first feet value: ");
    if (!double.TryParse(Console.ReadLine(), out feet1))
    {
        Console.WriteLine("Invalid input for first feet value. Skipping Feet comparison.");
        goto InchesSection;
    }

    Console.Write("Enter second feet value: ");
    if (!double.TryParse(Console.ReadLine(), out feet2))
    {
        Console.WriteLine("Invalid input for second feet value. Skipping Feet comparison.");
        goto InchesSection;
    }

    try
    {
        bool feetResult = QuantityMeasurementService.CompareFeet(feet1, feet2);

        Console.WriteLine(feetResult
            ? "Feet values are equal."
            : "Feet values are NOT equal.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Feet Error: {ex.Message}");
    }

InchesSection:

    // ---------------- INCHES SECTION ----------------
    Console.WriteLine("\n--- Inches Comparison ---");

    Console.Write("Enter first inches value: ");
    if (!double.TryParse(Console.ReadLine(), out inches1))
    {
        Console.WriteLine("Invalid input for first inches value. Skipping Inches comparison.");
        return;
    }

    Console.Write("Enter second inches value: ");
    if (!double.TryParse(Console.ReadLine(), out inches2))
    {
        Console.WriteLine("Invalid input for second inches value. Skipping Inches comparison.");
        return;
    }

    try
    {
        bool inchesResult = QuantityMeasurementService.CompareInches(inches1, inches2);

        Console.WriteLine(inchesResult
            ? "Inches values are equal."
            : "Inches values are NOT equal.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Inches Error: {ex.Message}");
    }
    }
    }
}