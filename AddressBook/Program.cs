using System;
using System.Collections.Generic;
using H5;
using H5.Core;
using static H5.Core.es5;
using static H5.Core.dom;
using Tesserae;
using static Tesserae.UI;
using Newtonsoft.Json;
using System.Linq;

namespace AddressBook
{
    class Program
    {
        static void Main(string[] args)
        {
            var progress = ProgressRing();
            var center = Stack().Children(progress).AlignItems(ItemAlign.Center).JustifyContent(ItemJustify.Center).Height(100.vh());
            document.body.appendChild(center.Render());

            LoadData(center);
        }

        static void LoadData(Tesserae.Stack center)
        {
            var req = new XMLHttpRequest();
            req.open("GET", "https://jsonplaceholder.typicode.com/users");
            req.onload = (e) =>
            {
                if (req.status == 200)
                {
                    try
                    {
                        var users = JsonConvert.DeserializeObject<List<User>>(req.responseText);
                        BuildApp(users);
                        center.Collapse();
                    }
                    catch (Exception ex)
                    {
                        center.Clear();
                        center.Children(TextBlock("Error parsing data: " + ex.Message).Danger());
                    }
                }
                else
                {
                    center.Clear();
                    center.Children(TextBlock("Error loading data: status " + req.status).Danger());
                }
            };
            req.onerror = (e) =>
            {
                center.Clear();
                center.Children(TextBlock("Network error loading data").Danger());
            };
            req.send();
        }

        static void BuildApp(List<User> users)
        {
            var searchBox = SearchBox("Search contacts...").Width(100.percent());
            var contactList = Stack().Padding(10.px()).ScrollY().Height(100.percent());
            var detailsPane = Stack().Padding(20.px()).Width(100.percent()).Height(100.percent());

            var leftPane = Stack().Children(searchBox, contactList).Height(100.percent()).Width(300.px());

            var splitView = SplitView().Left(leftPane, "300px").Right(detailsPane);

            document.body.appendChild(splitView.Render());

            void RenderDetails(User user)
            {
                detailsPane.Clear();
                if (user == null)
                {
                    var prompt = TextBlock("Select a contact to view details").Medium();
                    prompt.TextAlign = TextAlign.Center;
                    detailsPane.Children(prompt);
                    return;
                }

                detailsPane.Children(
                    TextBlock(user.Name).Large().SemiBold(),
                    TextBlock("@" + user.Username).MediumPlus().Primary(),
                    HorizontalSeparator(""),
                    Stack().Horizontal().Children(
                        Icon(UIcons.Envelope),
                        TextBlock(user.Email).PaddingLeft(8.px())
                    ).PaddingBottom(8.px()),
                    Stack().Horizontal().Children(
                        Icon(UIcons.PhoneCall),
                        TextBlock(user.Phone).PaddingLeft(8.px())
                    ).PaddingBottom(8.px()),
                    Stack().Horizontal().Children(
                        Icon(UIcons.Globe),
                        TextBlock(user.Website).PaddingLeft(8.px())
                    ).PaddingBottom(8.px()),
                    HorizontalSeparator(""),
                    TextBlock("Company").MediumPlus().SemiBold(),
                    TextBlock(user.Company.Name).Medium(),
                    TextBlock(user.Company.CatchPhrase).Small(),
                    HorizontalSeparator(""),
                    TextBlock("Address").MediumPlus().SemiBold(),
                    TextBlock($"{user.Address.Street}, {user.Address.Suite}").Medium(),
                    TextBlock($"{user.Address.City}, {user.Address.Zipcode}").Medium()
                );
            }

            void RenderList(string filter = "")
            {
                contactList.Clear();
                var filtered = users.Where(u => string.IsNullOrWhiteSpace(filter) || u.Name.ToLower().Contains(filter.ToLower()) || u.Email.ToLower().Contains(filter.ToLower())).ToList();

                foreach (var user in filtered)
                {
                    var card = Card(
                        Stack().Horizontal().Children(
                            Icon(UIcons.User).Large().PaddingRight(10.px()),
                            Stack().Children(
                                TextBlock(user.Name).Medium().SemiBold(),
                                TextBlock(user.Email).Small()
                            )
                        )
                    ).Padding(10.px()).MarginBottom(10.px());

                    card.OnClick((s, e) => RenderDetails(user));

                    contactList.Children(card);
                }
            }

            searchBox.OnSearch((s, e) => RenderList(searchBox.Text));

            RenderList();
            RenderDetails(null);
        }
    }
}
