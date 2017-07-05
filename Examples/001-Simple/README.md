# Objective
  * Learning how EasyConfig works

# Prerequisite
  * Include 'XML.cs' file in your project: This file contains extension methods which is used for parsing xml data easier. This would be the only prerequisite for all example projects and in later projects this will not be mentioned.

# Steps
### 1. Creating 'Config.EasyConfig' file
This file describes the structure of config file. Let's look at the content of this file.

This file is an XML file. The root tag must be **EasyConfig** which must have a **Version** attribute for controlling compatibility. The three other attributes in the example are optional and used for validation purposes which is recommended.
  * **xmlns**: Must be 'EasyConfig'
  * **xsi:schemaLocation**: For locating the 'Schema.xsd' validator file
  * **xmlns:xsi**: Must be 'http://www.w3.org/2001/XMLSchema-instance'
```xml
<EasyConfig
	Version="4.2"
	xmlns="EasyConfig"
	xsi:schemaLocation="EasyConfig ../../../Binaries/EasyConfig/v4.5/Schema.xsd"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
```

Inside **EasyConfig** tag there is a tag with name **Root**. This tag will be mapped to the auto generated class. This tag must have two attributes:
  * **Name**: Determines the name of the auto generated class and name of the root tag in actual config file
  * **Version**: Specify the version of this file (For easier maintanace in future and better versioning)
```xml
<Root Name="Config" Version="1.0">
```

Inside the **Root** tag will be one or more **Attribute** tag which describes the data. Each **Attribute** tag will be mapped into a field in auto generated class. **Attribute** tags can have these attributes:
  * **Name**: Determines the name of the field (Also the name of the attribute in actual config file) [required]
  * **Type**: Field datatype (string,int,float,...) [required]
  * **Desc**: Description about field [optional]
```xml
<Attribute Name="Hostname" Type="string" Desc="Hostname/IP address" />
<Attribute Name="Port" Type="int" />
```

That's it. For the first example we want to store information about a service which resides in a LAN (hostname used to determine the PC address) and listens to port.

### 2. Creating 'Config.bat' file
It's a good idea to have a **bat** file to update auto generated class as soon as possible when your 'Config.EasyConfig' file changes. Also you can control the process by using different parameters.

As of version '4.5', **EasyConfig** usage is as follow:
```
Usage: 
   EasyConfig [Options] File

Options:
   -o path  | Output file (Default: Same name as file with '.cs' extension in the current folder)
   -ns name | namespace name (Default: no namespace)
   -gs path | Generate Sample xml file
   -public  | Change access modifier for all classes to public (By default it is internal)
   -w       | Makes all fields writable by default (If not specified by default fields are readonly)
   -NoSave  | Do not implement 'Save' method
```

So in the 'Config.bat' file, first we need to locate the 'EasyConfig.exe' file; then provide some parameters; And finally specify the path to the 'Config.EasyConfig' file. For this example it looks like this:
```
..\..\..\Binaries\EasyConfig\v4.5\EasyConfig -NoSave Config.EasyConfig
```

### 3. Providing Actual Data 'Config.xml' file
Finally we need to store data based on 'Config.EasyConfig' file. By **Editor** application it is possible to create or change the data.
For this project 'Config.xml' looks like this:
```xml
<Config Version="1.0" Hostname="localhost" Port="1234" />
```

### 4. Using auto generated class
The most easy task in this project is the how to use auto generated class. First you need to add the **EasyConfig** output file 'Config.cs' into the project and instantiate it like this:
```C#
var C = new Config("Config.xml");
```
Notice that you need to pass the address to the actual config file (In this case "Config.xml").

By instantiating the class it will read the config file and fill all fields. Now it is ready to use.
```C#
Console.WriteLine("Port: " + C.Port);
Console.WriteLine("Hostname: " + C.Hostname);
```
