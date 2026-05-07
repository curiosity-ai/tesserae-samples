using System;
using H5;
using H5.Core;
using static H5.Core.es5;
using static H5.Core.dom;
using Tesserae;
using static Tesserae.UI;

namespace DrawingCanvas
{
    class Program
    {
        static void Main(string[] args)
        {
            var header = TextBlock("Tesserae Drawing Canvas").SemiBold().Large();

            var canvasElement = new HTMLCanvasElement();
            canvasElement.width = 800;
            canvasElement.height = 600;
            canvasElement.style.border = "1px solid black";
            canvasElement.style.cursor = "crosshair";

            var rawElement = Raw(canvasElement);

            var ctx = canvasElement.getContext("2d").As<CanvasRenderingContext2D>();

            bool isDrawing = false;
            double lastX = 0;
            double lastY = 0;
            string currentColor = "#000000";
            double currentSize = 5;

            canvasElement.onmousedown = (e) =>
            {
                var mouseEvent = e.As<MouseEvent>();
                isDrawing = true;
                lastX = mouseEvent.offsetX;
                lastY = mouseEvent.offsetY;
            };

            canvasElement.onmousemove = (e) =>
            {
                if (!isDrawing) return;
                var mouseEvent = e.As<MouseEvent>();
                ctx.beginPath();
                ctx.moveTo(lastX, lastY);
                ctx.lineTo(mouseEvent.offsetX, mouseEvent.offsetY);
                ctx.strokeStyle = currentColor;
                ctx.lineWidth = currentSize;
                ctx.lineCap = "round";
                ctx.stroke();

                lastX = mouseEvent.offsetX;
                lastY = mouseEvent.offsetY;
            };

            canvasElement.onmouseup = (e) =>
            {
                isDrawing = false;
            };

            canvasElement.onmouseout = (e) =>
            {
                isDrawing = false;
            };

            var clearButton = Button("Clear").Primary().OnClick((s, e) => {
                ctx.clearRect(0, 0, canvasElement.width, canvasElement.height);
            });

            var colorPicker = new HTMLInputElement();
            colorPicker.type = "color";
            colorPicker.value = currentColor;
            colorPicker.onchange = (e) => {
                currentColor = colorPicker.value;
            };

            var sizeSlider = new HTMLInputElement();
            sizeSlider.type = "range";
            sizeSlider.min = "1";
            sizeSlider.max = "50";
            sizeSlider.value = currentSize.ToString();
            sizeSlider.onchange = (e) => {
                currentSize = double.Parse(sizeSlider.value);
            };

            var controls = HStack().Children(
                TextBlock("Color:"),
                Raw(colorPicker),
                TextBlock("Size:"),
                Raw(sizeSlider),
                clearButton
            ).AlignItems(ItemAlign.Center).Gap(10.px());

            var root = VStack().Children(
                header,
                controls,
                rawElement
            ).Padding(20.px()).Gap(20.px());

            document.body.appendChild(root.Render());
        }
    }
}
