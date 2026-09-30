namespace CalculatorApp;

public partial class MainPage : ContentPage
{
    private string currentInput = "0";
    private string previousValue = "0";
    private string currentOperation = "";
    private bool isNewNumber = true;
    private bool hasDecimal = false;
    private double memory = 0;

    public MainPage()
    {
        InitializeComponent();
        UpdateDisplay();
    }

    private void OnNumberClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        string number = button.Text;

        try
        {
            if (isNewNumber)
            {
                currentInput = number;
                isNewNumber = false;
                hasDecimal = false;
            }
            else
            {
                if (currentInput.Length >= 15)
                    return;

                if (currentInput == "0")
                    currentInput = number;
                else
                    currentInput += number;
            }

            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnDecimalClicked(object sender, EventArgs e)
    {
        try
        {
            if (isNewNumber)
            {
                currentInput = "0.";
                isNewNumber = false;
                hasDecimal = true;
            }
            else if (!hasDecimal)
            {
                currentInput += ".";
                hasDecimal = true;
            }

            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnOperationClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        string operation = button.Text;

        try
        {
            if (!isNewNumber && !string.IsNullOrEmpty(currentOperation))
            {
                CalculateResult();
            }

            previousValue = currentInput;
            currentOperation = operation;
            isNewNumber = true;
            hasDecimal = false;

            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnToggleSignClicked(object sender, EventArgs e)
    {
        try
        {
            if (currentInput != "0")
            {
                double value = double.Parse(currentInput);
                value = -value;
                currentInput = value.ToString();
                UpdateDisplay();
            }
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnSquareRootClicked(object sender, EventArgs e)
    {
        try
        {
            double value = double.Parse(currentInput);
            if (value < 0)
            {
                DisplayError("Impossible: racine carrée d'un nombre négatif");
                return;
            }
            double result = Math.Sqrt(value);
            currentInput = FormatResult(result);
            isNewNumber = true;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnPowerClicked(object sender, EventArgs e)
    {
        try
        {
            currentOperation = "^";
            previousValue = currentInput;
            isNewNumber = true;
            hasDecimal = false;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnPiSquaredClicked(object sender, EventArgs e)
    {
        try
        {
            double result = Math.PI * Math.PI;
            currentInput = FormatResult(result);
            isNewNumber = true;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnMemoryClicked(object sender, EventArgs e)
    {
        try
        {
            memory = double.Parse(currentInput);
            DisplayError($"Mémorié: {memory}");
            isNewNumber = true;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnMemoryAddClicked(object sender, EventArgs e)
    {
        try
        {
            double value = double.Parse(currentInput);
            memory += value;
            DisplayError($"M+ : {memory}");
            currentInput = FormatResult(memory);
            isNewNumber = true;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnEqualsClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(currentOperation) || isNewNumber)
                return;

            CalculateResult();
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        try
        {
            if (currentInput.Length > 1)
            {
                string lastChar = currentInput[currentInput.Length - 1].ToString();
                if (lastChar == ".")
                    hasDecimal = false;

                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            else
            {
                currentInput = "0";
                isNewNumber = true;
                hasDecimal = false;
            }

            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        try
        {
            currentInput = "0";
            previousValue = "0";
            currentOperation = "";
            isNewNumber = true;
            hasDecimal = false;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
        }
    }

    private void CalculateResult()
    {
        try
        {
            double prev = double.Parse(previousValue);
            double current = double.Parse(currentInput);
            double result = 0;

            switch (currentOperation)
            {
                case "+":
                    result = prev + current;
                    break;
                case "−":
                case "-":
                    result = prev - current;
                    break;
                case "×":
                case "*":
                    result = prev * current;
                    break;
                case "÷":
                case "/":
                    if (current == 0)
                    {
                        DisplayError("Division par zéro!");
                        currentInput = "0";
                        currentOperation = "";
                        isNewNumber = true;
                        return;
                    }
                    result = prev / current;
                    break;
                case "%":
                    result = prev % current;
                    break;
                case "^":
                    result = Math.Pow(prev, current);
                    break;
                default:
                    return;
            }

            if (double.IsInfinity(result) || double.IsNaN(result))
            {
                DisplayError("Erreur de calcul!");
                currentInput = "0";
            }
            else
            {
                currentInput = FormatResult(result);
            }

            currentOperation = "";
            previousValue = "0";
            isNewNumber = true;
            hasDecimal = false;
        }
        catch (Exception ex)
        {
            DisplayError($"Erreur: {ex.Message}");
            currentInput = "0";
            currentOperation = "";
            isNewNumber = true;
        }
    }

    private string FormatResult(double result)
    {
        result = Math.Round(result, 10);
        string formatted = result.ToString();

        if (formatted.Contains("."))
        {
            formatted = formatted.TrimEnd('0').TrimEnd('.');
        }

        return formatted;
    }

    private void UpdateDisplay()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (currentInput.Length > 15)
            {
                ResultDisplay.Text = double.Parse(currentInput).ToString("E6");
            }
            else
            {
                ResultDisplay.Text = currentInput;
            }

            if (!string.IsNullOrEmpty(currentOperation))
            {
                OperationDisplay.Text = $"{previousValue} {currentOperation}";
            }
            else
            {
                OperationDisplay.Text = "";
            }
        });
    }

    private void DisplayError(string message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("Info", message, "OK");
        });
    }
}