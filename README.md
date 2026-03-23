### QuantityMeasurementApp
**Overview**

QuantityMeasurementApp is a C# console application for handling length measurements. It supports equality checks, unit conversions, and arithmetic operations across multiple units (feet, inches, yards, centimeters). The project demonstrates OOP concepts, DRY principle, and scalable design.

**Use Cases**

**UC1: Feet Measurement Equality**

- Compares two feet values for equality.
- Handles null, non-numeric, and type validation.
- Test cases: same value, different value, null, same reference.

**UC2: Feet and Inches Equality**

- Adds inches measurement alongside feet.
- Equality checks performed independently for feet and inches.
- Ensures numeric and type safety.

**UC3: Generic Quantity Class (DRY Principle)**

- Introduces QuantityLength class with LengthUnit enum.
- Eliminates code duplication from UC1 & UC2.
- Supports cross-unit equality (e.g., 1 foot = 12 inches).
- Key concepts: DRY, enum usage, encapsulation, value-based equality.

**UC4: Extended Unit Support**

- Adds Yards and Centimeters.
- Existing equality logic works for new units automatically.
- Demonstrates scalability and enum extensibility.
- Cross-unit checks: feet ↔ yards ↔ inches ↔ cm.

**UC5: Unit-to-Unit Conversion**

- Provides explicit conversion API between units.
- Example: QuantityLength.Convert(1.0, FEET, INCHES) → 12.0.
- Validates units and numeric input.
- Key concepts: precision, base unit normalization, bidirectional conversion.

**UC6: Addition of Two Length Units**

- Supports addition of two lengths (same category).
- Result expressed in the unit of the first operand.
- Handles same-unit and cross-unit addition.
- Maintains immutability, commutativity, and precision.
- Examples: 1 FEET + 12 INCHES → 2 FEET  ; 2.54 CM + 1 INCH → ~5.08 CM

**UC7: Addition with Target Unit Specification**

- Extends UC6: allows specifying any target unit for the addition result.
- Preserves immutability, commutativity, and precision.
- Validates operands and target unit; throws exception for invalid inputs.
- Uses method overloading and private utility method for clean, DRY implementation.
- Examples:add(1 FEET, 12 INCHES, FEET) → 2 FEET

**UC8: Subtraction of Two Length Units**

- Supports subtraction of two length quantities.
- Handles same-unit and cross-unit subtraction.
- Performs automatic unit conversion before subtraction.
- Result expressed in: Default → first operand’s unit. OR explicitly specified target unit (method overloading).
- Supports negative results.
- Maintains immutability of objects.
- Ensures precision using epsilon-based floating-point comparison.
- Validates null and invalid inputs; throws appropriate exceptions.
- Preserves arithmetic properties and safe operations.

**UC9: Multi-Category Measurement Support**

- Extends application to support multiple measurement categories.
- Introduces separate classes:QuantityLength, QuantityVolume, QuantityWeight
- Each category has its own unit enum.
- Supports equality, conversion, addition, and subtraction per category.
- Prevents cross-category operations (Length ≠ Volume ≠ Weight).
- Throws exception for invalid cross-category arithmetic.
- Demonstrates strong type safety.
- Improves functional scalability but introduces code duplication.
- Highlights design limitation (violates DRY principle).

**UC10: Generic Quantity Class with Unit Interface**

- Refactors UC9 into a single generic architecture.
- Introduces generic class: Quantity<TUnit>.
- Implements common IUnit interface for all unit types.
- Eliminates duplication across measurement categories.
- Supports unlimited categories (Length, Volume, Weight, future types).
- Maintains immutability and value-based equality.
- Prevents cross-category operations at compile-time.
- Centralizes conversion logic via interface implementation.
- Follows DRY principle strictly.
- Applies Open/Closed Principle (OCP).
- Enables scalable, maintainable, and clean design.
- Adding a new measurement category requires minimal changes.

**UC11: Multi-Category Measurement Using Generic Quantity**

- Introduces a generic Quantity<TUnit> class to support multiple measurement categories (Length, Weight, Volume).
- Uses a common IMeasurable interface implemented by all unit enums.
- Enables equality checks and unit conversions across different units within the same category.
- Ensures compile-time type safety, preventing operations between different measurement categories.
- Improves scalability and reusability by using generics instead of separate category classes.

**UC12: Arithmetic Operations on Quantities**

- Extends the generic Quantity<TUnit> class to support addition, subtraction, and division operations.
- Supports cross-unit arithmetic by converting operands to a common base unit before computation.
- Provides method overloading to allow specifying a target unit for the result.
- Division returns a dimensionless scalar value representing the ratio of two quantities.
- Ensures immutability, validation, and safe error handling for invalid inputs.

**UC13: Centralized Arithmetic Logic (DRY Principle)**

- Refactors arithmetic operations from UC12 to remove duplicated validation and conversion logic.
- Introduces a centralized private helper method that performs common arithmetic processing.
- Uses an ArithmeticOperation enum to dispatch operations such as ADD, SUBTRACT, and DIVIDE.
- Ensures consistent validation, error handling, and base-unit conversion across all operations.
- Improves maintainability and scalability, making it easy to add future operations like multiplication.

**UC14: Temperature Measurement with Selective Arithmetic Support**

- Adds Temperature measurement support to the system using a new TemperatureUnit enum (Celsius, Fahrenheit, Kelvin).
- Refactors the IMeasurable interface using default methods so some measurement categories can disable unsupported operations.
- Allows temperature equality checks and unit conversions, but blocks arithmetic operations like addition, subtraction, and division.
- Introduces a SupportsArithmetic functional interface with lambda expressions to indicate whether a unit supports arithmetic operations.
- Maintains type safety and clear error handling, throwing UnsupportedOperationException when unsupported temperature operations are attempted.

**UC15: Persistence Layer using ADO.NET**

- Introduces database integration using ADO.NET for storing quantity measurement operations.
- Implements CRUD operations with SQL Server (SSMS) using SqlConnection, SqlCommand, and DataReader.
- Stores details like input values, units, operation type, result, and error messages.
- Handles manual connection management and query execution.
- Enables retrieval of operation history for tracking and debugging.

**UC16: Repository Pattern and Structured Data Access**

- Refactors ADO.NET code using Repository Pattern for better separation of concerns.
- Creates interfaces and concrete repository classes for database operations.
- Improves code maintainability, testability, and scalability.
- Centralizes data access logic, reducing duplication across the application.
- Prepares the project for future enhancements like caching and service abstraction.
  
**UC17: ASP.NET Core Integration with REST APIs**

- Converts the application into an ASP.NET Core Web API for exposing RESTful endpoints.
- Implements controllers for operations like compare, convert, add, and history retrieval.
- Adds features like DTOs, model validation, and structured API responses.
- Integrates authentication basics (JWT), password hashing with salting, and encryption/decryption.
- Connects API with SQL Server and enables Swagger for API testing and documentation.
  
