using System.Collections.ObjectModel;
using System.Diagnostics;
using Calculator.Model;
using Calculator.MVVM;

namespace Calculator.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel() 
        {
            Memory = [];
            History = new ObservableCollection<HistoryItem>();
        }

        public RelayCommand UpdateCommand => new(obj => UpdateDisplayValue(obj!));
        public RelayCommand ClearCommand => new(obj => ClearDisplayValue(obj!));
        public RelayCommand UnaryCommand => new(obj => PerformUnaryOperation(obj!));
        public RelayCommand BinaryCommand => new(obj => PerformBinaryOperation(obj!));
        public RelayCommand EvaluateCommand => new(execute => EvaluateExpression());

        public RelayCommand ClearMemoryCommand => new(obj => ClearMemory(obj), canExecute => Memory.Count > 0);
        public RelayCommand RecallCommand => new(execute => RecallMemoryValue(), canExecute => Memory.Count > 0);
        public RelayCommand AddCommand => new(obj => AddMemoryValue(obj));
        public RelayCommand SubtractCommand => new(obj => SubtractMemoryValue(obj));
        public RelayCommand StoreCommand => new(execute => StoreMemoryValue());
        public RelayCommand ShowCommand => new(execute => ShowMemoryValues(), canExecute => (Memory.Count > 0 || ShowListView));

        public RelayCommand ShowHistoryCommand => new(execute => ShowHistoryItems());
        public RelayCommand ClearHistoryCommand => new(execute => ClearHistory());

        public RelayCommand ShowModeCommand => new(execute => ShowModeItems());
        public RelayCommand ChangeCommand => new(obj => ChangeMode(obj!));

        private bool showListView = false;
        private bool showHistory = false;
        private bool showMode = false;

        public bool ShowHistory
        {
            get { return showHistory; }
            set { showHistory = value; OnPropertyChanged(); }
        }

        public bool ShowListView
        {
            get { return showListView; }
            set { showListView = value; OnPropertyChanged(); }
        }

        public bool ShowMode
        {
            get { return showMode; }
            set { showMode = value; OnPropertyChanged(); }
        }

        private double displayValue = 0;
        private string modeValue = "Standard";
        private string binaryOperator;
        private double storedDisplayValue = 0;
        private bool concat = false;
        private bool valueStored = false;
        private string currentExpression = "";

        private double selectedItem;
        public double SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        private HistoryItem selectedHistoryItem;
        public HistoryItem SelectedHistoryItem
        {
            get { return selectedHistoryItem; }
            set
            {
                selectedHistoryItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<double> Memory { get; set; }
        public ObservableCollection<HistoryItem> History { get; set; }

        public double DisplayValue
        {
            get { return displayValue; }
            set
            {
                displayValue = value;
                OnPropertyChanged();
            }
        }

        public string ModeValue
        {
            get { return modeValue; }
            set
            {
                modeValue = value;
                OnPropertyChanged();
            }
        }

        public string BinaryOperator
        {
            get { return binaryOperator; }
            set 
            {
                binaryOperator = value;
                OnPropertyChanged();
            }
        }

        public string CurrentExpression
        {
            get { return currentExpression; }
            set
            {
                currentExpression = value;
                OnPropertyChanged();
            }
        }

        private void UpdateDisplayValue(object obj)
        {
            if (concat)
                DisplayValue = double.Parse(DisplayValue.ToString() + obj.ToString()!);
            else
            {
                DisplayValue = double.Parse(obj.ToString()!);
                concat = true;
            }
        }

        private void ClearDisplayValue(object obj)
        {
            string commandType = obj.ToString()!;

            if (commandType == "⌫")
            {
                if (DisplayValue.ToString().Length == 1)
                    DisplayValue = 0;
                else
                    DisplayValue = double.Parse(DisplayValue.ToString()[..^1]);

                return;
            }

            else if (commandType == "CE" && valueStored)
            {
                DisplayValue = 0;

                if (CurrentExpression.Contains('='))
                    CurrentExpression = "";

                return;
            }

            DisplayValue = 0;
            BinaryOperator = "";
            storedDisplayValue = 0;
            valueStored = false;
            concat = false;
            CurrentExpression = "";
        }

        private void PerformUnaryOperation(object obj)
        {
            string unaryOperator = obj.ToString()!;

            if (unaryOperator == "1/x")
                DisplayValue = 1 / DisplayValue;
            else if (unaryOperator == "x²")
                DisplayValue *= DisplayValue;
            else if (unaryOperator == "√x")
                DisplayValue = Math.Sqrt(DisplayValue);
            else if (unaryOperator == "+/-")
                DisplayValue *= (double)-1;
            else if (unaryOperator == ".")
                DisplayValue = Double.Parse(DisplayValue.ToString() + ".");
            else if (unaryOperator == "%")
            {
                if (BinaryOperator == "+" || BinaryOperator == "-")
                {

                }
                else if (BinaryOperator == "×" || BinaryOperator == "÷")
                {

                }
                else
                    DisplayValue = 0;
            }
        }

        private void PerformBinaryOperation(object obj)
        {
            BinaryOperator = obj.ToString()!;
            storedDisplayValue = DisplayValue;
            valueStored = true;
            concat = false;

            CurrentExpression = storedDisplayValue.ToString() + " " + BinaryOperator;
        }

        private void EvaluateExpression()
        {
            CurrentExpression += " " + DisplayValue.ToString() + " = ";

            if (BinaryOperator == "+")
                DisplayValue += storedDisplayValue;
            else if (BinaryOperator == "-")
                DisplayValue = storedDisplayValue - DisplayValue;
            else if (BinaryOperator == "×")
                DisplayValue *= storedDisplayValue;
            else if (BinaryOperator == "÷")
                DisplayValue = storedDisplayValue / DisplayValue;

            History.Add(new HistoryItem
            {
                Expression = CurrentExpression,
                Result = DisplayValue.ToString()
            });

            BinaryOperator = "";
            storedDisplayValue = 0;
            valueStored = false;
            concat = false;
        }


        private void ClearMemory(object? index)
        {
            if (index != null)
                Memory.RemoveAt(int.Parse(index.ToString()));
            else
                Memory.Clear();
            
            concat = false;
        }

        private void RecallMemoryValue()
        {
            if (CurrentExpression.Contains('='))
                CurrentExpression = "";

            DisplayValue = Memory.Last();
            concat = false;
        }

        private void AddMemoryValue(object? index)
        {
            if (index != null)
            {
                Memory[int.Parse(index.ToString()!)] += DisplayValue;
                return;
            }

            if (Memory.Count == 0)
                Memory.Add(DisplayValue);
            else
                Memory[0] += DisplayValue;

            concat = false;
        }

        private void SubtractMemoryValue(object index)
        {
            if (index != null)
            {
                Memory[int.Parse(index.ToString())] -= DisplayValue;
                return;
            }

            if (Memory.Count == 0)
                Memory.Add(DisplayValue * (double)-1);
            else
                Memory[0] -= DisplayValue;

            concat = false;
        }

        private void StoreMemoryValue()
        {
            Memory.Add(DisplayValue);
            concat = false;
        }

        private void ShowMemoryValues()
        {
            ShowListView = !ShowListView;
            ShowHistory = false;
        }

        private void ShowHistoryItems()
        {
            ShowHistory = !ShowHistory;
            ShowListView = false;
        }

        private void ClearHistory()
        {
            History.Clear();
        }

        private void ShowModeItems()
        {
            ShowMode = !ShowMode;
        }

        private void ChangeMode(object obj)
        {
            ModeValue = obj.ToString();
            ShowMode = !ShowMode;
        }
    }
}
