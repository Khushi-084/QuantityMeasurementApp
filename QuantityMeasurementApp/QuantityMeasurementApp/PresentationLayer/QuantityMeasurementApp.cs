using System;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    class QuantityMeasurementApp
    {
        static void Main(string[] args)
        {
            QuantityMeasurementService service = new QuantityMeasurementService();

            while (true)
            {
                Console.WriteLine("\n--- Quantity Measurement Menu ---");
                Console.WriteLine("1. Compare Two Feet Values (UC-1)");
                Console.WriteLine("2. Exit");
                Console.Write("Enter your choice: ");

                string inputChoice = Console.ReadLine();

                if (!int.TryParse(inputChoice, out int choice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (choice == 1)
                {
                    try
                    {
                        Console.Write("Enter first feet value: ");
                        double value1 = double.Parse(Console.ReadLine());

                        Console.Write("Enter second feet value: ");
                        double value2 = double.Parse(Console.ReadLine());

                        bool result = service.CompareFeet(value1, value2);

                        if (result)
                            Console.WriteLine("Both feet values are equal.");
                        else
                            Console.WriteLine("Feet values are NOT equal.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else if (choice == 2)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
        }
    }
}