<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/231054628/26.2.2%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T849071)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# ASP.NET Core File Manager and Popup - Select Documents and Preview Content

This example allows users to preview document content in a popup dialog. The dialog can display Word documents (DOCX, RTF), Excel files (XLSX), Diagram data (JSON), and images. Document selection is implemented using a File Manager.

## Configure an ASP.NET Core Project

If you start a new project, add required DevExpress libraries to your project as described in the following help topics:

* [DevExtreme-Based Controls - Configure a non-Visual Studio Project](https://docs.devexpress.com/AspNetCore/401027/devextreme-based-controls/get-started/configure-a-non-visual-studio-project)
* [DevExtreme-Based Controls - Configure a Visual Studio Project](https://docs.devexpress.com/AspNetCore/401026/devextreme-based-controls/get-started/configure-a-visual-studio-project)
* [Office-Inspired Controls - Get Started with the Rich Text Editor](https://docs.devexpress.com/AspNetCore/400321/rich-edit/get-started/net-core-application)
* [Office-Inspired Controls - Get Started with the Spreadsheet](https://docs.devexpress.com/AspNetCore/400374/spreadsheet/configure-a-visual-studio-project)

## Configure FileManager and Popup Controls

1. Add a FileManager to a View. Connect the FileManager to your file system as described in the following demo: [Physical File System](https://demos.devexpress.com/ASPNetCore/Demo/FileManager/BindingToFileSystem/). 
1. Create a dialog using a [Popup](https://js.devexpress.com/Demos/WidgetsGallery/Demo/Popup/Overview/jQuery/Light/) component:

	```cs
	@(Html.DevExtreme().Popup()
		.ID("dialogPopup"))
	```

1. Create a `NamedTemplate` for each file type users can preview. Refer to the following help topic for additional information: [Switching Templates on the Fly](https://js.devexpress.com/Documentation/Guide/Widgets/Popup/Customize_the_Appearance/Customize_the_Content/#Switching_Templates_On_the_Fly).

	```cs
	@using (Html.DevExtreme().NamedTemplate("text")) {
		//
	}
	``` 

1. Handle the [FileManager.OnSelectedFileOpened](https://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxFileManager/Configuration/#onSelectedFileOpened) event to open your dialog and pass the selected file: 

	```js
	function onSelectedFileOpened(args) {
		openFileInDialog(args.file);
	}
	```

## Add Controls that Display File Content

In this example, the following controls display file content:

- [Rich Text Editor](#Rich-Text-Editor)
- [Spreadsheet](#Spreadsheet)
- [Diagram](#Diagram)
- [Image](#Image)

### Rich Text Editor

1. When using the RichEdit component, you may specify dozens of configuration options. In such instances, create a separate [Partial View](./CS/FileManagerOpenDocuments/Views/Shared/RichEditPartial.cshtml). Render the Partial View within a `NamedTemplate`: 

	```cs
	@using (Html.DevExtreme().NamedTemplate("text")) {
		await Html.RenderPartialAsync("RichEditPartial");
	}
	```

1. Call the [RichEdit.openDocument](https://docs.devexpress.com/AspNetCore/js-DevExpress.RichEdit.RichEdit#js_devexpress_richedit_richedit_opendocument) method to open the selected file:

	```js
		FileLoader.loadRichEdit = function loadRichEdit(richEditControl, fileManager, fileItem, documentFormat) {
			fileManager.option("fileSystemProvider").getItemsContent([fileItem]).done(function (arrayBuffer) {
				var base64Content = _fromArrayBufferToBase64(arrayBuffer);
				richEditControl.openDocument(base64Content, fileItem.name, documentFormat);
			});
		};
	```

### Spreadsheet

1. Add a [Spreadsheet](https://docs.devexpress.com/AspNetCore/401031/office-inspired-controls/get-started/add-controls-to-a-project#spreadsheet) control to a separate [Partial View](./CS/FileManagerOpenDocuments/Views/Shared/SpreadsheetPartial.cshtml). Render the Partial View within a `NamedTemplate`: 

	```cs
	@using (Html.DevExtreme().NamedTemplate("excel")) {
		ViewContext.Writer.Write("<div id='excelContainer'>");
		await Html.RenderPartialAsync("SpreadsheetPartial", new FileManagerOpenDocuments.Models.DocumentModel());
		ViewContext.Writer.Write("</div>");
	}
	```

1. Spreadsheet can open files only on the server. Trigger a request to the server and pass the selected item's key as the request parameter. When the request is completed, update the Spreadsheet container element:

	```js
	FileLoader.loadSpreadsheet = function loadSpreadsheet(spreadsheetSelector, url, fileItem) {
			//spreadsheetSelector is parent div selector, i.e."#excelContainer"
			$.post(url, { filePath: fileItem.key }, function (data) {  $(spreadsheetSelector).html(data);  });
		};
	```

1. Obtain the Excel document from the file system in the _Controller_ and return it as a model:

	```cs
	return PartialView("SpreadsheetPartial", new DocumentModel {
		DocumentID = relativePath,
		FileBytes = System.IO.File.ReadAllBytes(path)
	});
	```

1. Call the [Spreadsheet.Open](https://docs.devexpress.com/AspNetCore/DevExpress.AspNetCore.Spreadsheet.SpreadsheetBuilder.Open.overloads) method to open the Excel document:

	```cs
	@(Html.DevExpress()
	.Spreadsheet("spreadsheet")
	.Open(Model?.DocumentID, DevExpress.Spreadsheet.DocumentFormat.Xlsx, () => { return Model?.FileBytes; }))
	```

### Diagram

1. Create a [Partial View](./CS/FileManagerOpenDocuments/Views/Shared/DiagramPartial.cshtml) that contains a Diagram. Render the Partial View within a `NamedTemplate`:

	```cs
	@using (Html.DevExtreme().NamedTemplate("diagram")) {
		await Html.RenderPartialAsync("DiagramPartial");
	}
	```

1. Call the **fileSystemProvider.getItemsContent** method to obtain selected file content. Call the [import](https://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxDiagram/Methods/#importdata_updateExistingItemsOnly) method to load that content into the Diagram:

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

1. Add an `<img>` tag to the `NamedTemplate`:

	```cs
	@using (Html.DevExtreme().NamedTemplate("image")) {
		<img id="imageViewer" />
	}
	```

1. Call the **fileSystemProvider.getItemsContent** method to obtain image content. Convert the result to a `base64` string and assign this string to the `src` attribute:

	```js
	FileLoader.loadImage = function loadImage(imageSelector, fileManager, fileItem) {
			fileManager.option("fileSystemProvider").getItemsContent([fileItem]).done(function (arrayBuffer) {
				var base64Content = _fromArrayBufferToBase64(arrayBuffer);
				$(imageSelector).attr("src","data:image/jpg;base64," + base64Content);
			});
		};
	```  


## Files to Review

* [Index.cshtml](./CS/FileManagerOpenDocuments/Pages/Index.cshtml)
* [RichEditPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Shared/RichEditPartial.cshtml)
* [SpreadsheetPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Shared/SpreadsheetPartial.cshtml)
* [DiagramPartial.cshtml](./CS/FileManagerOpenDocuments/Views/Shared/DiagramPartial.cshtml)
* [FileManagerApiController.cs](./CS/FileManagerOpenDocuments/Controllers/FileManagerApiController.cs)
* [HomeController.cs](./CS/FileManagerOpenDocuments/Controllers/HomeController.cs)
* [Program.cs](./CS/FileManagerOpenDocuments/Program.cs)
* [_Layout.cshtml](./CS/FileManagerOpenDocuments/Pages/Shared/_Layout.cshtml)
<!-- feedback -->
## Does This Example Address Your Development Requirements/Objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=ASP.NET-Core-File-Manager-How-to-open-documents&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=ASP.NET-Core-File-Manager-How-to-open-documents&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
