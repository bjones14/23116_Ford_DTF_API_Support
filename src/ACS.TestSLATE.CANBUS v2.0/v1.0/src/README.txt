This folder contains all of the source code for the plugin regardless of language (C#, C++, LabVIEW, etc.) and any associated resources that the source code requires (icons, bitmaps, etc).

The .csproj & .sln files are in the \src folder.

This folder is also divided in subfolders, some typical subfolders include:
\lv - this folder includes the Data Manager template and should house all LabVIEW code
\models - For .NET code, this folder stores the data models and validation code
\presenters - For .NET code, this folder stores the presenters that interact with the models and views
\ui - For .NET code, this folder stores all UI elements, controls, forms, tabbed documents, editors, and views
\unit tests - For .NET code, this folder stores all automated unit tests (xUnit, nUnit or MSUnit typically)
\interop 
	\ db	- For .NET code, this folder stores all code that provides database access or extension doc access
	\ labview - For .NET code, this folder stores all VI Wrappers or other code that talks directly to or is accessed from LabVIEW
	\ testslate - For .NET code, this folder stores the Node, Plugin and Menu files that provide the hooks that connect the plugin into Test SLATE.
\properties - For .NET code, this folder stores the AssemblyInfo file, licenses files, and Resources.resx files.

C++ and other languages should be structured similarly.