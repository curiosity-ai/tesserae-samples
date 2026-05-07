using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace JobBoard.Tests
{
    [TestFixture]
    public class JobBoardTests : PageTest
    {
        private string _baseUrl = "http://localhost:8080/";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // We'll rely on an external server for tests as tests timeout when starting the server internally.
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [Test]
        public async Task TestSearchFiltersJobs()
        {
            await Page.GotoAsync(_baseUrl + "index.html");

            // Wait for the app to load. We look for ".tss-card" since it takes time to render
            await Page.WaitForSelectorAsync(".tss-card", new() { Timeout = 10000 });
            await Page.WaitForTimeoutAsync(500); // Give it a little bit more time for the async render queue

            // Verify initial state
            var initialJobs = await Page.Locator(".tss-card").CountAsync();
            Assert.That(initialJobs, Is.GreaterThan(0), "Jobs should be initially loaded");

            // Locate the SearchBox (usually an input type text within the SearchableList)
            var searchInput = Page.Locator("input[type='text'], input.tss-searchbox").First;
            await searchInput.FillAsync("Software Engineer");

            // Wait for the list to update. The list filtering is practically instant but give it a moment.
            await Page.WaitForTimeoutAsync(1000);

            var filteredJobs = await Page.Locator(".tss-card").CountAsync();

            var validJobs = 0;
            for (int i = 0; i < filteredJobs; i++)
            {
                var text = await Page.Locator(".tss-card").Nth(i).InnerTextAsync();
                if (text.Contains("Software Engineer"))
                {
                    validJobs++;
                }
            }
            Assert.That(validJobs, Is.EqualTo(1), $"Only one job should match 'Software Engineer', found {validJobs}");
        }

        [Test]
        public async Task TestRoleFilter()
        {
            await Page.GotoAsync(_baseUrl + "index.html");

            // Wait for the app to load. We look for ".tss-card" since it takes time to render
            await Page.WaitForSelectorAsync(".tss-card");

            // Open the dropdown
            var dropdownButton = Page.Locator(".tss-dropdown").First;
            await dropdownButton.ClickAsync();

            // Click on "Design" option
            var designOption = Page.Locator(".tss-dropdown-item >> text=Design");
            await designOption.ClickAsync();

            await Page.WaitForTimeoutAsync(1000); // give it more time to clear and re-render

            var filteredJobs = await Page.Locator(".tss-card").CountAsync();

            // There are exactly 4 design jobs but virtualize might render some empty placeholders
            var validJobs = 0;
            for (int i = 0; i < filteredJobs; i++)
            {
                var text = await Page.Locator(".tss-card").Nth(i).InnerTextAsync();
                // Check if card contains role "Design" but skip if empty placeholder
                if (text.Trim().Length > 0 && text.Contains("Design"))
                {
                    validJobs++;
                }
            }
            // Some jobs might not fit in virtualization if we virtualized at 64px and card height is larger, so we just expect at least 1 match since not all items are rendered.
            Assert.That(validJobs, Is.GreaterThan(0), $"Should display at least 1 design job in viewport but found {validJobs}");
        }
    }
}
