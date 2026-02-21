using System;
using QuantityMeasurementApp.Domain;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// Entry point of the Quantity Measurement Application.
    /// This class handles user interaction (Presentation Layer).
    /// </summary>
    class QuantityMeasurementApp
    {
        /// <summary>
        /// Main method - program execution starts here.
        /// Displays menu and handles user input in a loop.
        /// </summary>
        static void Main(string[] args)
        {
            // Infinite loop to keep application running until user exits
            while (true)
            {
                // Display menu options
                Console.WriteLine("\n--- Quantity Measurement Menu ---");
                Console.WriteLine("1. Create Feet Object");
                Console.WriteLine("2. Exit");
                Console.Write("Enter your choice: ");

                try
                {
                    // Read user choice as string
                    string inputChoice = Console.ReadLine();

                    // Validate if input is numeric
                    // TryParse prevents program crash if invalid input is entered
                    if (!int.TryParse(inputChoice, out int choice))
                    {
                        Console.WriteLine(" Please enter a valid numeric choice.");
                        continue; // Restart loop
                    }

                    // Option 1: Create Feet object
                    if (choice == 1)
                    {
                        Console.Write("Enter feet value: ");
                        string inputValue = Console.ReadLine();

                        // Validate if entered value is numeric
                        if (!double.TryParse(inputValue, out double value))
                        {
                            Console.WriteLine(" Invalid data type. Please enter a numeric value.");
                            continue; // Restart loop
                        }

                        // Create Feet object (Domain Layer)
                        // Constructor validation rules will execute here
                        Feet feet = new Feet(value);

                        Console.WriteLine($"Feet object created successfully with value: {feet.Value}");
                    }
                    // Option 2: Exit program
                    else if (choice == 2)
                    {
                        Console.WriteLine("Exiting program...");
                        break; // Break loop and terminate program
                    }
                    else
                    {
                        // If user enters invalid menu option
                        Console.WriteLine("Invalid choice. Try again.");
                    }
                }
                // Handles validation errors thrown from Feet constructor
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                // Handles any unexpected runtime errors
                catch (Exception)
                {
                    Console.WriteLine("Unexpected error occurred.");
                }
            }
        }
    }
}