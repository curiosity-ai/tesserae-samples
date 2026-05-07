using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestCurrencyConversion()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync("http://localhost:8080");

            // Wait for elements to appear
            await page.WaitForSelectorAsync("text='Currency Converter'");

            // Initial state (it should load from API, so result text should change from 'Loading...')
            await page.WaitForFunctionAsync("() => !document.querySelector('body').innerText.includes('Loading...')");

            await page.WaitForTimeoutAsync(2000); // Give it a second to load drop downs

            var amountBox = await page.QuerySelectorAsync("input[placeholder='Amount']");
            if (amountBox != null) {
                await amountBox.FillAsync("10");
                await amountBox.PressAsync("Enter");
                await page.EvaluateAsync("el => el.dispatchEvent(new Event('input', { bubbles: true }))", amountBox);
            }

            await page.WaitForTimeoutAsync(1000); // Wait for the calculation

            var resultText = await page.InnerTextAsync(".tss-textblock.tss-fontsize-xlarge.tss-fontweight-bold");
            Assert.Contains("10 USD =", resultText);
            Assert.Contains("EUR", resultText);
        }
    }
}
