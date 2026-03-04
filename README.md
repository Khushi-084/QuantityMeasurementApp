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

  
**Key Concepts Across All UCs**
- Object-Oriented Design: Encapsulation, immutability, abstraction.
- Equality: Reflexive, symmetric, transitive, epsilon-based floating-point comparison.
- Unit Conversion: Centralized LengthUnit enum with scalable conversion factors.
- Arithmetic on Value Objects: Addition with automatic unit conversion.
- Scalable Design: Adding new units requires only enum modification.
- Cross-unit Addition: Explicit target unit or default first-operand unit.
- Precision & Rounding: Handles zero, negative, large, and small values with epsilon-based accuracy.
- Input Validation: Invalid or null units throw exceptions, ensuring safe operations.
