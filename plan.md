1.  **Implement `DrawingCanvas`:**
    *   Create a complete HTML5 Canvas drawing app inside `DrawingCanvas/Program.cs` using Tesserae.
    *   Features:
        *   HTMLCanvasElement initialized via `UI.Raw`.
        *   Mousedown, mousemove, mouseup and mouseout events to capture drawing state.
        *   Use `CanvasRenderingContext2D` to draw lines on the canvas.
        *   Controls:
            *   Color picker input using `HTMLInputElement` of type `color` wrapped in `UI.Raw`.
            *   Brush size slider using `HTMLInputElement` of type `range` wrapped in `UI.Raw`.
            *   Clear button using `UI.Button`.
    *   The layout is wrapped in `UI.VStack` and controls in `UI.HStack`.
2.  **Verify compilation & running:**
    *   Build with `dotnet build` using the locally installed global tool `h5-compiler` to transpile C# into JavaScript.
    *   Serve statically via `dotnet serve`.
3.  **Test application functionality using Playwright:**
    *   Create `tests.spec.ts` using Playwright.
    *   Test standard assertions:
        *   Verifying header presence.
        *   Verifying control elements (clear button, canvas).
        *   Checking functionality by simulating mouse drag on canvas.
        *   Checking clear functionality click.
    *   Run tests and verify everything passes.
4.  **Complete pre commit steps**
    *   Complete pre commit steps to make sure proper testing, verifications, reviews and reflections are done.
5.  **Submit the change.**
    *   Submit the fully functional project containing the UI and Playwright tests.
