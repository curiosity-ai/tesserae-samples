using System;
using H5;
using H5.Core;
using static H5.Core.es5;
using static H5.Core.dom;
using Tesserae;
using static Tesserae.UI;

namespace Calculator
{
    class Program
    {
        private static double _previousValue = 0;
        private static string _currentOperation = "";
        private static bool _isNewInput = true;

        static void Main(string[] args)
        {
            UI.Theme.Light();

            var display = TextBlock("0").Large().Bold().TextRight().PT(16.px()).PB(16.px()).PR(16.px()).PL(16.px());
            display.Style(s => s.background = "#fff");
            display.Style(s => s.borderRadius = "8px");
            display.Style(s => s.marginBottom = "16px");
            display.Style(s => s.boxShadow = "inset 0 2px 4px rgba(0,0,0,0.1)");
            display.Style(s => s.fontSize = "32px");
            display.Style(s => s.overflow = "hidden");

            Action<string> updateDisplay = (val) => {
                if (_isNewInput) {
                    display.Text = val;
                    _isNewInput = false;
                } else {
                    if (display.Text == "0" && val != ".") {
                        display.Text = val;
                    } else {
                        display.Text += val;
                    }
                }
            };

            Action calculate = () => {
                if (string.IsNullOrEmpty(_currentOperation)) return;

                double currentValue = double.Parse(display.Text);
                double result = 0;

                switch (_currentOperation) {
                    case "+": result = _previousValue + currentValue; break;
                    case "-": result = _previousValue - currentValue; break;
                    case "*": result = _previousValue * currentValue; break;
                    case "/":
                        if (currentValue != 0) result = _previousValue / currentValue;
                        else {
                            display.Text = "Error";
                            _isNewInput = true;
                            _currentOperation = "";
                            return;
                        }
                        break;
                }

                display.Text = result.ToString();
                _isNewInput = true;
                _currentOperation = "";
            };

            Action<string> setOperation = (op) => {
                if (!_isNewInput && !string.IsNullOrEmpty(_currentOperation)) {
                    calculate();
                }
                _previousValue = double.Parse(display.Text);
                _currentOperation = op;
                _isNewInput = true;
            };

            Action clear = () => {
                display.Text = "0";
                _previousValue = 0;
                _currentOperation = "";
                _isNewInput = true;
            };

            var btnClear = Button("C").Danger().OnClick((_, __) => clear());
            var btnSign = Button("+/-").OnClick((_, __) => {
                if (display.Text.StartsWith("-")) display.Text = display.Text.Substring(1);
                else if (display.Text != "0") display.Text = "-" + display.Text;
            });
            var btnPercent = Button("%").OnClick((_, __) => {
                double val = double.Parse(display.Text);
                display.Text = (val / 100).ToString();
                _isNewInput = true;
            });
            var btnDivide = Button("/").Primary().OnClick((_, __) => setOperation("/"));

            var btn7 = Button("7").OnClick((_, __) => updateDisplay("7"));
            var btn8 = Button("8").OnClick((_, __) => updateDisplay("8"));
            var btn9 = Button("9").OnClick((_, __) => updateDisplay("9"));
            var btnMultiply = Button("x").Primary().OnClick((_, __) => setOperation("*"));

            var btn4 = Button("4").OnClick((_, __) => updateDisplay("4"));
            var btn5 = Button("5").OnClick((_, __) => updateDisplay("5"));
            var btn6 = Button("6").OnClick((_, __) => updateDisplay("6"));
            var btnMinus = Button("-").Primary().OnClick((_, __) => setOperation("-"));

            var btn1 = Button("1").OnClick((_, __) => updateDisplay("1"));
            var btn2 = Button("2").OnClick((_, __) => updateDisplay("2"));
            var btn3 = Button("3").OnClick((_, __) => updateDisplay("3"));
            var btnPlus = Button("+").Primary().OnClick((_, __) => setOperation("+"));

            var btn0 = Button("0").OnClick((_, __) => updateDisplay("0"));
            var btnDot = Button(".").OnClick((_, __) => {
                if (!display.Text.Contains(".")) updateDisplay(".");
            });
            var btnEquals = Button("=").Success().OnClick((_, __) => calculate());

            var gridStyle = "display: grid; grid-template-columns: repeat(4, 1fr); gap: 8px;";

            var row1 = Grid(new[] { 1.fr(), 1.fr(), 1.fr(), 1.fr() }).Gap(8.px()).Children(btnClear, btnSign, btnPercent, btnDivide);
            var row2 = Grid(new[] { 1.fr(), 1.fr(), 1.fr(), 1.fr() }).Gap(8.px()).Children(btn7, btn8, btn9, btnMultiply);
            var row3 = Grid(new[] { 1.fr(), 1.fr(), 1.fr(), 1.fr() }).Gap(8.px()).Children(btn4, btn5, btn6, btnMinus);
            var row4 = Grid(new[] { 1.fr(), 1.fr(), 1.fr(), 1.fr() }).Gap(8.px()).Children(btn1, btn2, btn3, btnPlus);

            var row5 = Grid(new[] { 2.fr(), 1.fr(), 1.fr() }).Gap(8.px()).Children(btn0, btnDot, btnEquals);

            var calculatorStack = VStack()
                .P(24.px())
                .Background(Theme.Secondary.Background)
                .W(320.px())
                .Children(
                    display,
                    VStack().Gap(8.px()).Children(
                        row1,
                        row2,
                        row3,
                        row4,
                        row5
                    )
                );
            calculatorStack.Style(s => s.borderRadius = "16px");
            calculatorStack.Style(s => s.boxShadow = "0 8px 16px rgba(0,0,0,0.1)");

            var centerContainer = HStack()
                .JustifyContent(ItemJustify.Center)
                .AlignItems(ItemAlign.Center)
                .W(100.vw())
                .H(100.vh())
                .Background(Theme.Default.Background)
                .Children(calculatorStack);

            document.body.appendChild(centerContainer.Render());
        }
    }
}
