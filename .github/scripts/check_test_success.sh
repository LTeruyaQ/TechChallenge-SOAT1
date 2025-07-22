#!/bin/bash

# Run the tests and save the output to a file
dotnet test test/MecanicaOSTests/MecanicaOSTests.csproj --logger "console;verbosity=detailed" > test_output.txt

# Extract the total number of tests and the number of passed tests
total_tests=$(grep -oP 'Total tests: \K\d+' test_output.txt)
passed_tests=$(grep -oP 'Passed: \K\d+' test_output.txt)

# Check if the values are not empty
if [ -z "$total_tests" ] || [ -z "$passed_tests" ]; then
  echo "Error: Could not determine the number of tests."
  exit 1
fi

# Calculate the success percentage
success_percentage=$(( (passed_tests * 100) / total_tests ))

# Check if the success percentage is at least 80%
if [ "$success_percentage" -ge 80 ]; then
  echo "Test success percentage is $success_percentage%, which is at least 80%. Build can proceed."
  exit 0
else
  echo "Test success percentage is $success_percentage%, which is less than 80%. Build will fail."
  exit 1
fi
