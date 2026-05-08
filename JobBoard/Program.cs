using System;
using System.Linq;
using H5;
using H5.Core;
using static H5.Core.es5;
using static H5.Core.dom;
using Tesserae;
using static Tesserae.UI;
using System.Collections.Generic;

namespace JobBoard
{
    class Program
    {
        static async void Main(string[] args)
        {
            document.body.style.overflow = "hidden";

            var loading = TextBlock("Loading jobs...").MediumPlus().Padding(16.px());
            document.body.appendChild(loading.Render());

            try
            {
                var jobsList = await JobItem.GetJobs();
                var jobs = jobsList.ToArray();

                var searchableList = SearchableList(jobs)
                    .Virtualize(64.px())
                    .WithNoResultsMessage(() => BackgroundArea(Card(TextBlock("No matching jobs found").Padding(16.px()))).WS().HS().MinHeight(100.px()))
                    .Height(400.px());

                var roleDropdown = Dropdown().Items(
                    DropdownItem("All Roles").Selected(),
                    DropdownItem("Engineering"),
                    DropdownItem("Product"),
                    DropdownItem("Design"),
                    DropdownItem("Marketing"),
                    DropdownItem("HR"),
                    DropdownItem("Sales")
                );

                roleDropdown.OnChange((s, e) =>
                {
                    var selectedRole = roleDropdown.SelectedItems[0].Text;
                    if (selectedRole == "All Roles")
                    {
                        searchableList.Items.Clear();
                        searchableList.Items.AddRange(jobs);
                    }
                    else
                    {
                        searchableList.Items.Clear();
                        searchableList.Items.AddRange(jobs.Where(j => j.Role == selectedRole));
                    }
                });

                var header = HStack().Padding(16.px()).Children(
                    TextBlock("Job Board").XLarge().Bold().WS(),
                    roleDropdown
                );

                var layout = VStack().HeightStretch().WidthStretch().Children(
                    header,
                    searchableList
                );

                document.body.innerHTML = "";
                document.body.appendChild(layout.Render());
            }
            catch (Exception ex)
            {
                document.body.innerHTML = "";
                document.body.appendChild(TextBlock($"Error loading jobs: {ex.Message}").Danger().Render());
            }
        }
    }
}
