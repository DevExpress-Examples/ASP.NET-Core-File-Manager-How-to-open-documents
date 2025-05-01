<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/231054628/22.2.2%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T849071)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# ASP.NET Core File Manager - How to open different files in a popup dialog

This example shows how to display documents in File Manager and open them in a popup dialog. The dialog opens Word documents (DOCX, RTF), Excel files (XLSX), Diagram data (JSON) and images.

## Configure ASP.NET Core project
 If you start a new project, add required DevExpress libraries to your project as described in these tutorials:

* [Devextreme-Based Controls - Configure a non Visual Studio Project](https://docs.devexpress.com/AspNetCore/401027/devextreme-based-controls/get-started/configure-a-non-visual-studio-project)
* [Office-Inspired Controls - Configure a Visual Studio Project](https://docs.devexpress.com/AspNetCore/400321/office-inspired-controls/get-started/configure-a-visual-studio-project)

> **Note** This project targets .NET Core 3.1. To run the project in Visual Studio 2017, change the target framework in the project settings.
## Configure FileManager and Popup
1) Add FileManager to your View. Connect FileManager to your file system like in the [Physical File System](https://demos.devexpress.com/ASPNetCore/Demo/FileManager/BindingToFileSystem/) demo. 
2) Create a dialog with the [Popup](https://js.devexpress.com/Demos/WidgetsGallery/Demo/Popup/Overview/jQuery/Light/) component:
	```cs
	@(Html.DevExtreme().Popup()
		.ID("dialogPopup"))
	```
3) The dialog should display different content based on file type. To achieve this functionality, use the approach from the [Switching Templates On the Fly](https://js.devexpress.com/Documentation/Guide/Widgets/Popup/Customize_the_Appearance/Customize_the_Content/#Switching_Templates_On_the_Fly) article and create **NamedTemplate** for each control that opens files:
	```cs
	@using (Html.DevExtreme().NamedTemplate("text")) {
		//
	}
	``` 
4) Handle the [FileManager.OnSelectedFileOpened](https://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxFileManager/Configuration/#onSelectedFileOpened) event. Show your dialog and open selected file in this event handler: 
	```js
	function onSelectedFileOpened(args) {
		openFileInDialog(args.file);
	}
	```
## Add controls for different file types

This section describes how to open the most popular file types in different controls. It's not required to implement all of them. 

- [Rich Text Editor](#Rich-Text-Editor)
- [Spreadsheet](#Spreadsheet)
- [Diagram](#Diagram)
- [Image](#Image)

### Rich Text Editor
1) The RichEdit component may have a lot of configuration options, so it's better to create it in a separate Partial View: [RichEditPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Home/RichEditPartial.cshtml).
2) Render this Partial View inside a corresponding NamedTemplate: 
	```cs
	@using (Html.DevExtreme().NamedTemplate("text")) {
		await Html.RenderPartialAsync("RichEditPartial");
	}
	```
3) Use RichEdit's [openDocument](https://docs.devexpress.com/AspNetCore/js-DevExpress.RichEdit.RichEdit#js_devexpress_richedit_richedit_opendocument) method to open the selected file content. For this, convert the ArrayBuffer object returned by **getItemsContent** to the base64 string.
	```js
		FileLoader.loadRichEdit = function loadRichEdit(richEditControl, fileManager, fileItem, documentFormat) {
			fileManager.option("fileSystemProvider").getItemsContent([fileItem]).done(function (arrayBuffer) {
				var base64Content = _fromArrayBufferToBase64(arrayBuffer);
				richEditControl.openDocument(base64Content, fileItem.name, documentFormat);
			});
		};
	```

### Spreadsheet
1) Add [Spreadsheet](https://docs.devexpress.com/AspNetCore/401031/office-inspired-controls/get-started/add-controls-to-a-project#spreadsheet) to a separate Partial View: [SpreadsheetPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Home/SpreadsheetPartial.cshtml). 
2) Render the Partial View inside NamedTemplate:
	```cs
	@using (Html.DevExtreme().NamedTemplate("excel")) {
		ViewContext.Writer.Write("<div id='excelContainer'>");
		await Html.RenderPartialAsync("SpreadsheetPartial");
		ViewContext.Writer.Write("</div>");
	}
	```
3) Spreadsheet can open files only on the server side. Thus, to open a file, it's necessary to trigger a request to the server and pass the selected item key as the request parameter. When the request is completed, update Spreadsheet's container element:
	```js
	FileLoader.loadSpreadsheet = function loadSpreadsheet(spreadsheetSelector, url, fileItem) {
			//spreadsheetSelector is parent div selector, i.e."#excelContainer"
			$.post(url, { filePath: fileItem.key }, function (data) {  $(spreadsheetSelector).html(data);  });
		};
	```
4) In Controller, get a corresponding Excel document from the file system and return it as Model:
```cs
 public IActionResult OpenDocInSpreadsheet(string filePath) {
            return PartialView("SpreadsheetPartial", GetDocumentModel(filePath));
        }
```
5) Open this object in the control with the [Spreadsheet.Open](https://docs.devexpress.com/AspNetCore/DevExpress.AspNetCore.Spreadsheet.SpreadsheetBuilder.Open.overloads) method:
	```cs
	@(Html.DevExpress()
	.Spreadsheet("spreadsheet")
	.Open(Model?.DocumentID, DevExpress.Spreadsheet.DocumentFormat.Xlsx, () => { return Model?.FileBytes; }))
	```

### Diagram
1) Create a separate Partial View with your Diagram: [DiagramPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Home/DiagramPartial.cshtml).
2) Render this Partial View inside NamedTemplate:
	```cs
	@using (Html.DevExtreme().NamedTemplate("diagram")) {
		await Html.RenderPartialAsync("DiagramPartial");
	}
	```
3) Use the **fileSystemProvider.getItemsContent** method to get the selected file content and open it in Diagram using the [import](https://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxDiagram/Methods/#importdata_updateExistingItemsOnly) method:
	```js
	FileLoader.loadDiagram = function loadDiagram(diagramSelector, fileManager, fileItem) {
			fileManager.option("fileSystemProvider").getItemsContent([fileItem]).done(function (arrayBuffer) {
				var enc = new TextDecoder("utf-8");
				var data = enc.decode(arrayBuffer);
				$(diagramSelector).dxDiagram("instance").import(data);
			});
		};
	```

### Image
1) Add the IMG tag to the corresponding NamedTemplate:
	```cs
	@using (Html.DevExtreme().NamedTemplate("image")) {
		<img id="imageViewer" />
	}
	```
2) Get the image content with the **fileSystemProvider.getItemsContent** method. Convert the result to the base64 string and set the "src" attribute value to this string:
	```js
	FileLoader.loadImage = function loadImage(imageSelector, fileManager, fileItem) {
			fileManager.option("fileSystemProvider").getItemsContent([fileItem]).done(function (arrayBuffer) {
				var base64Content = _fromArrayBufferToBase64(arrayBuffer);
				$(imageSelector).attr("src","data:image/jpg;base64," + base64Content);
			});
		};
	```  


## Files to Review

* [Index.cshtml](./CS/FileManagerOpenDocuments/Views/Home/Index.cshtml)
* [RichEditPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Home/RichEditPartial.cshtml)
* [SpreadsheetPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Home/SpreadsheetPartial.cshtml)
* [DiagramPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Home/DiagramPartial.cshtml)
* [FileManagerApiController.cs](./CS/FileManagerOpenDocuments/Controllers/FileManagerApiController.cs)
* [HomeController.cs](./CS/FileManagerOpenDocuments/Controllers/HomeController.cs)
* [_Layout.cshtml](./CS/FileManagerOpenDocuments/Views/Shared/_Layout.cshtml)
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=ASP.NET-Core-File-Manager-How-to-open-documents&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=ASP.NET-Core-File-Manager-How-to-open-documents&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
