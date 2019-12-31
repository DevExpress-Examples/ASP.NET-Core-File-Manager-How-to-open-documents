# ASP.NET Core File Manager - How to open documents in Rich Text Editor

This example shows how to display documents in File Manager and open them in [ASP.NET Core Rich Text Editor](https://demos.devexpress.com/ASPNetCore/Demo/RichEdit/Overview/). The control works with DOCX, RTF and TXT files.

*Files to look at*:

* [Index.cshtml](./CS/FileManagerOpenDocuments/Views/Home/Index.cshtml)
* [FileManagerApiController.cs](./CS/FileManagerOpenDocuments/Controllers/FileManagerApiController.cs)
* [_Layout.cshtml](./CS/FileManagerOpenDocuments/Views/Shared/_Layout.cshtml)


## Implementation:

1) Add required libraries to your project: 

[Configure a Visual Studio Project](https://docs.devexpress.com/AspNetCore/401026/devextreme-based-controls/get-started/configure-a-visual-studio-project).

[Office-Inspired Controls - Configure a Visual Studio Project](https://docs.devexpress.com/AspNetCore/400321/office-inspired-controls/get-started/configure-a-visual-studio-project)
> **Note** The project targets .NET Core 3.0. To run the project in Visual Studio 2017, change the target framework in the project settings.

2) Add the FileManager to your View. Connect FileManager to your file system like in the [Physical File System](https://demos.devexpress.com/ASPNetCore/Demo/FileManager/BindingToFileSystem/) demo. 

3) Add Rich Text Editor to the page (for example, to the Popup control's ContentTemplate):
```cs
@(Html.DevExtreme().Popup()
    .ID("dialogPopup")
    .ContentTemplate(@<text>
    @(Html.DevExpress().RichEdit("richEdit")
        ...
    )
    </text>))
```

4) Handle the [FileManager.OnSelectedFileOpened](https://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxFileManager/Configuration/#onSelectedFileOpened) event and call the [getItemContent](https://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxFileManager/File_Providers/Remote/Methods/#getItemContent) method in this event handler to get a selected file content. 
```js
 function onSelectedFileOpened(args) {
        let fileName = args.fileItem.name;
        let provider = args.component.option("fileProvider");
        provider.getItemContent([args.fileItem]).done(function (arrayBuffer) {
            let extension = getFileItemExtension(fileName);
            switch (extension) {
                case "docx":
                    openDocInDialog(fileName, DevExpress.RichEdit.DocumentFormat.OpenXml, arrayBuffer);
                    break;
               ...
        });
    }
```

5) Use the RichEdit's openDocument method to open the selected file content. For this, convert the ArrayBuffer object returned by **getItemContent** to the base64 string.
```js
 function openDocInDialog(fileName, fileType, content) {
        let base64Content = fromArrayBuffer(content);
        richEdit.openDocument(base64Content, fileName, fileType);
    }
 function fromArrayBuffer(buffer) {
        const binary = [];
        const bytes = new Uint8Array(buffer);
        const len = bytes.byteLength;
        for (let i = 0; i < len; i++)
            binary.push(String.fromCharCode(bytes[i]));
        return window.btoa(binary.join(''));
    }
```

