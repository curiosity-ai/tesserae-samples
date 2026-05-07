using System.Collections.Generic;
using H5.Core;
using Tesserae;
using static Tesserae.UI;

namespace JobBoard
{
    public class JobItem : ISearchableItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        private IComponent _component;

        public static async System.Threading.Tasks.Task<List<JobItem>> GetJobs()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.GetStringAsync(H5.Core.dom.window.location.origin + "/assets/data/jobs.json");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<JobItem>>(response);
            }
        }

        public bool IsMatch(string searchTerm)
        {
            var lowerTerm = searchTerm.ToLower();
            return (Title != null && Title.ToLower().Contains(lowerTerm)) ||
                   (Company != null && Company.ToLower().Contains(lowerTerm)) ||
                   (Role != null && Role.ToLower().Contains(lowerTerm));
        }

        public H5.Core.dom.HTMLElement Render()
        {
            return (this as ISearchableItem).Render().Render();
        }

        IComponent ISearchableItem.Render()
        {
            if (_component == null)
            {
                _component = Card(
                    VStack().Children(
                        TextBlock(Title).SemiBold().MediumPlus(),
                        TextBlock(Company).Primary(),
                        TextBlock($"{Role} - {Location}").Small(),
                        TextBlock(Description).Small()
                    )
                ).MB(8.px());
            }
            return _component;
        }
    }
}
