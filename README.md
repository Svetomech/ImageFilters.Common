### ImageFilters.Common is a small, low-level, cross-platform image filtering library.

Uses [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common), `unsafe` code.

Built against [.NET Standard 2.0](https://docs.microsoft.com/dotnet/standard/net-standard).

### Features

* **Fast**
* Extensible
* More coming!

### API 

Here's an example of the code required to apply a filter to an image:

```csharp
var bitmap = new Bitmap(Image.FromFile("foo.jpg"));
var image = new GrayscaleFilter(bitmap).Apply().Image;
image.Save("bar.jpg");
```

Or with parameters:

```csharp
var bitmap = new Bitmap(Image.FromFile("foo.jpg"));
var image = new ThresholdFilter(bitmap) { X = 79 }.Apply().Image;
image.Save("bar.jpg");
```

### Releases

Check out the [releases tab](https://github.com/Svetomech/ImageFilters.Common/releases).

### Manual build

If you prefer, you can compile ImageFilters.Common yourself, you'll need:

- [Visual Studio 2017](https://www.visualstudio.com/news/releasenotes/vs2017-relnotes)
- [.NET Core](https://www.microsoft.com/net/learn/get-started/windows)

Alternatively, on Linux you can use:

- [Visual Studio Code](https://code.visualstudio.com) with [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)
- [.NET Core](https://www.microsoft.com/net/learn/get-started/linuxubuntu)

To clone it locally, click the "Clone or download -> Open in Visual Studio" button above or run the following bash command:

```bash
git clone https://github.com/Svetomech/ImageFilters.Common
```
