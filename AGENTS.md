# Tesserae Samples - AGENTS

## Project Goals
The goal of this repository is to provide a set of simple, understandable, and interesting demo applications for the **Tesserae** UI toolkit, similar to a gallery of examples. These samples serve as starting points or reference implementations for developing single-page applications entirely in C# using the h5 compiler.

## Using Tesserae
Tesserae is a UI toolkit for building web applications entirely in C#, inspired by Microsoft's Fluent UI toolkit.

To work with these samples or create a new project:
1. Ensure you have installed **.NET 10**.
2. Install the h5 compiler package globally by running:
   ```bash
   dotnet tool install --global h5-compiler
   ```
   *(If updating, use `dotnet tool update --global h5-compiler`)*
3. Use the h5 templates:
   ```bash
   dotnet new install h5.Template
   ```

## The `h5.json` Configuration File
The build process in Tesserae and h5 is controlled by the `h5.json` file in your project root.
- **Output Directory:** Defines where compiled files are placed (e.g., `"output": "$(OutDir)/h5/"`).
- **HTML Generation:** Can generate an `index.html` file automatically.
- **Resources:** Allows you to specify additional CSS, images, or JS files to include.

## Importing Assets (JavaScript, Fonts, CSS, Images)
To include external assets such as JavaScript libraries, web fonts, CSS stylesheets, or images:
- Use the `resources` array in your `h5.json` file to copy these files into the output directory.
- You can inject them into the DOM directly from C# using Tesserae's capabilities, or define them in your generated `index.html`.
- For images and icons, place them in an appropriate folder (e.g., `assets/`) and reference their relative paths from your UI components (like `UI.Image()`).

## Testing with Playwright
Reliability is key for UI toolkits. **Each sample app in this repository should be thoroughly tested using Playwright.** You should write automation scripts to interact with the rendered h5 web application, capture screenshots, and ensure all components function as expected across different interactions.
