# Lab3

This is a student's laboratory work on the topic “Dynamic arrays (one-dimensional and jagged)”.

## Documentation repository

Here's a [repo](https://github.com/Remenod/paal2-lab3-doc)(Github pages) with full API documentation and task descriptions.

## Lab Assignment: Dynamic Arrays

This task consists of two blocks, where different methods of handling dynamic arrays are to be implemented: one-dimensional arrays and jagged arrays. Below is the general approach for implementing each block in C#.

### Block 1: One-dimensional Arrays

#### Input and Initialization of Array:
- A method is implemented to input data either from the console, txt file or by generating it randomly.

#### Method for Array Transformation:
- A method is created to change the size of the array.
  - **Manual creation of a new array using `new`**: A new array is created, and the necessary elements from the previous array are copied.

#### Removal of Elements:
- For each variant of element removal (e.g., the first even element, the last negative element), the following steps are performed:
  - Check if the element exists for removal.
  - Perform a shift of elements to ensure there are no "gaps" in the array.
  - After removal, change the size of the array to match the new number of elements.

#### Insertion of Elements:
- Elements are inserted into the array at a specified position.
- Inserting elements requires shifting the remaining elements of the array.

#### Implementation of Handling Variants:
- Necessary operations are performed, such as:
  - Insert the minimum value at the beginning and the maximum value at the end of the array.
  - Modify the array according to specific conditions.

### Block 2: Jagged Arrays

#### Input of Jagged Array:
- A jagged array is an array of arrays. An array of arrays is used, where each inner array may have a different number of elements.

#### Processing of Jagged Array:
- For each variant, rows are added or removed from the jagged array.
- Reference reassignment is used to shift rows in the array, avoiding complex element copying.

#### Checking Conditions:
- Before removal or insertion, it is checked if there are rows or elements available to perform the operation.

#### Insertion and Removal of Rows:
- Rows are inserted at the beginning or end of the jagged array by modifying references to the corresponding arrays.
- Removal of rows is done by shifting elements or simply reassigning references.

#### Implementation of Handling Variants:
- For example, rows containing certain elements (minimum, maximum values) are removed, empty rows are inserted after every even row, and so on.

