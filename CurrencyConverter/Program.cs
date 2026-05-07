using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H5;
using H5.Core;
using Tesserae;
using static Tesserae.UI;
using static H5.Core.dom;

namespace CurrencyConverter
{
    public class CurrencyData {
        public string @base { get; set; }
        public Dictionary<string, double> rates { get; set; }
    }

    public class Program
    {
        private static Dictionary<string, double> _rates = new Dictionary<string, double>();
        private static Dropdown _fromCurrencyDropdown;
        private static Dropdown _toCurrencyDropdown;
        private static TextBox _amountTextBox;
        private static TextBlock _resultTextBlock;

        public static void Main(string[] args)
        {
            _amountTextBox = TextBox("1.0").SetPlaceholder("Amount").OnInput((sender, e) => CalculateResult());
            _fromCurrencyDropdown = Dropdown().OnChange((sender, e) => CalculateResult());
            _toCurrencyDropdown = Dropdown().OnChange((sender, e) => CalculateResult());
            _resultTextBlock = TextBlock("Loading...").XLarge().Bold();

            var card = Card(
                Stack().Children(
                    TextBlock("Currency Converter").Large().Bold(),
                    Stack().Horizontal().Children(
                        TextBlock("Amount:").AlignEnd().PaddingRight(16.px()),
                        _amountTextBox
                    ),
                    Stack().Horizontal().Children(
                        TextBlock("From:").AlignEnd().PaddingRight(16.px()),
                        _fromCurrencyDropdown
                    ),
                    Stack().Horizontal().Children(
                        TextBlock("To:").AlignEnd().PaddingRight(16.px()),
                        _toCurrencyDropdown
                    ),
                    _resultTextBlock
                ).Gap(16.px())
            );

            document.body.appendChild(
                Stack().Horizontal().JustifyContent(ItemJustify.Center).PaddingTop(32.px()).Children(card).Render()
            );

            LoadRatesAsync();
        }

        private static void LoadRatesAsync()
        {
            var req = new XMLHttpRequest();
            req.open("GET", "https://api.exchangerate-api.com/v4/latest/USD");
            req.onload = (e) => {
                try {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyData>(req.responseText);
                    _rates = data.rates;

                    var itemsFrom = new List<Dropdown.Item>();
                    var itemsTo = new List<Dropdown.Item>();
                    foreach(var r in _rates.Keys) {
                        itemsFrom.Add(new Dropdown.Item(r).SelectedIf(r == "USD"));
                        itemsTo.Add(new Dropdown.Item(r).SelectedIf(r == "EUR"));
                    }
                    _fromCurrencyDropdown.Items(itemsFrom.ToArray());
                    _toCurrencyDropdown.Items(itemsTo.ToArray());

                    CalculateResult();
                } catch(Exception ex) {
                    _resultTextBlock.Text = "Error loading rates: " + ex.Message;
                }
            };
            req.onerror = (e) => {
                _resultTextBlock.Text = "Error connecting to server.";
            };
            req.send();
        }

        private static void CalculateResult()
        {
            if (_rates == null || _rates.Count == 0) return;

            if (!double.TryParse(_amountTextBox.Text, out var amount))
            {
                _resultTextBlock.Text = "Invalid amount";
                return;
            }

            var fromItem = _fromCurrencyDropdown.SelectedItems != null && _fromCurrencyDropdown.SelectedItems.Length > 0 ? _fromCurrencyDropdown.SelectedItems[0] : null;
            var toItem = _toCurrencyDropdown.SelectedItems != null && _toCurrencyDropdown.SelectedItems.Length > 0 ? _toCurrencyDropdown.SelectedItems[0] : null;

            if (fromItem == null || toItem == null) return;

            var fromCurrency = fromItem.Text;
            var toCurrency = toItem.Text;

            if (!_rates.ContainsKey(fromCurrency) || (!_rates.ContainsKey(toCurrency))) return;

            var fromRate = _rates[fromCurrency];
            var toRate = _rates[toCurrency];

            var usdAmount = amount / fromRate;
            var finalAmount = usdAmount * toRate;

            _resultTextBlock.Text = $"{amount} {fromCurrency} = {finalAmount:0.00} {toCurrency}";
        }
    }
}
