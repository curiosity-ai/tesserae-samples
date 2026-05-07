using System;
using System.Collections.Generic;
using H5;
using H5.Core;
using static H5.Core.es5;
using static H5.Core.dom;
using Tesserae;
using static Tesserae.UI;

namespace ImageGallery
{
    class Program
    {
        static void Main(string[] args)
        {
            var header = TextBlock("Tesserae Image Gallery").Large().SemiBold().PaddingBottom(16.px());

            var masonry = new Masonry(columns: 4, gutter: 10);

            // Using placeholder blank png for generating later
            // Expected Image generation:
            // 1. A landscape photo of a serene mountain lake at sunrise.
            // 2. A portrait shot of a dense, misty forest.
            // 3. A square image of a bustling city street at night with neon lights.
            // 4. A macro shot of a vibrant blue butterfly resting on a flower.
            // 5. A vertical panoramic view of a sandy desert dune.
            // 6. A wide shot of a futuristic skyline with flying cars.
            // 7. A tall image of a majestic waterfall crashing into a river.
            // 8. A warm, cozy photo of a cabin in the woods during autumn.

            var placeholderDataUri = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";

            var images = new List<string>
            {
                placeholderDataUri,
                placeholderDataUri,
                placeholderDataUri,
                placeholderDataUri,
                placeholderDataUri,
                placeholderDataUri,
                placeholderDataUri,
                placeholderDataUri
            };

            foreach (var imgUrl in images)
            {
                var img = Image(imgUrl).Width(100.percent()).Height(UnitSize.Auto());
                img.InnerElement.style.borderRadius = "8px";
                img.InnerElement.onclick = (e) => {
                    var modal = Modal("Image Preview").Content(Image(imgUrl).Width(100.percent()));
                    modal.Show();
                };
                var card = Card(img).Padding(0.px());
                card.InnerElement.style.cursor = "pointer";
                masonry.Add(card);
            }

            var stack = Stack().Children(header, masonry).Padding(32.px());

            document.body.appendChild(stack.Render());
        }
    }
}
