# Tags
  * [**EasyConfig**](#easyconfig): Root tag
  * [**Root**](#root): top level auto generated class. Actualy it's a **Node** tag with an additional *Version* attribute.
  * [**DataType**](#datatype): Useful for describing (grouping) similar piece of information. It will be mapped into a class.
    *  **Field** tags will refer to it by *Type* attribute.
    *  **Node** and **DataType** tags can inherit from **DataType** by *inherit* attribute.
    *  Contains **Field** and **Attribute** tags
    *  Can't have a nested **Node**
    *  Can't have a nested **DataType** (Will be possible in future versions)
  * [**Node**](#node): **Node** is a combination of **DataType** and **Field**. It means it describes the data (like **DataType**) and refers to itself like a **Field**.
    * So instead of defining a **DataType** and use (referring to) it  by a **Field**, it is possible to use **Node** in one place.
    * It will be mapped into a class (like a **DataType**)
    * A field member will be created in container class (like a **Field**)
    * It will be associated with a tag in actual config file (like a **Field**)
    * Can have other **Node** or **DataType** tags
  * [**Attribute**](#attribute): This tag is the leaf in the graph and describes each individual data. It will be mapped into a field.
  * [**Field**](#field): It will use a **DataType** or **Node** for describing one group of data in config file.
  * [**Enum**](#enum): Used to define an enum


## EasyConfig
**EasyConfig** is the top level tag (root tag) which specifies that this is an **EasyConfig** document.

 **EasyConfig** tag contains only one **Root** tag and may contains several **DataType** tags.

Attributes:
  * *Version* (required): Determines document is compatible with which version of **EasyConfig** tool
  * *xmlns* (optional): For validation (Must be 'EasyConfig')
  * *xsi:schemaLocation* (optional): For validation (For locating **EasyConfig** validator file 'Schema.xsd')
  * *xmlns:xsi* (optional): For validation (Must be 'http://www.w3.org/2001/XMLSchema-instance')

Example:
```xml
<EasyConfig
   Version="4.2"
   xmlns="EasyConfig"
   xsi:schemaLocation="EasyConfig path/to/EasyConfig/Schema.xsd"
   xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
```


## Attribute
Maps an attribute (in config file) into a field.

Attributes:
  * *Name* (required): Name of the field (Name of the attribute in the tag)
  * *Type* (required): Data type for the field. Supported data types are:
    * string
    * int
    * float
    * char
    * yn (bool) [Yes/No]
    * Version
    * **Enum**
  * *Default* (optional): default value for the field if attribute in the tag is missing.
    * Note: If *Default* is not provided and attribute is missing in the config file an exception will be thrown
  * *ReadOnly* (optional): If 'Yes' makes field *readonly* ('No' by default)


## Root
The only tag inside **EasyConfig** tag which acts as a **Node** and has a required *Version* attribute. For more information see [**Node**](#node)


## DataType
Represent a C# class. By the aid of **Field** tags they will correspond each tag to a class.

**DataType** contains **Field** and **Attribute** tags.

Attributes:
  * *Name* (required): Name of the generated class
  * *Inherit* (optional): Name of the class which this class will inherit
  * *Partial* (optional): If 'Yes' the generated class would be *partial* ('No' by default)
  * *Access* (optional): Access modifier for the generated class (*internal* by default)


## Node
Inherits from **DataType**. They generate a class and create a field which maps a tag in the config file to it.

**Node** in addition to **Field** and **Attribute** tags also contains **DataType** and **Node** tags.

Attributes (inherits all attributes from **DataType**):
  * *Name* (required): Name of the generated field
  * *ReadOnly* (optional): If 'Yes' makes field *readonly* ('No' by default)
  * *Multiple* (optional): If 'Yes' makes a list instead of a field and associate all tags with the list ('No' by default)
  * *TypeName* (optional): Name of the generated class (If not specified, *Name* will be used)
  * *TagName* (optional): Name of the correspinding tag(s) (If not specified, *Name* will be used)


## Field
Generates a field which maps a tag into a **DataType**. It's like an **Attribute** but instead of attribute it will map a tag.

Attributes:
  * *Name* (required): Name of the generated field
  * *Type* (required): Data type name
  * *ReadOnly* (optional): If 'Yes' makes field *readonly* ('No' by default)
  * *Multiple* (optional): If 'Yes' makes a list instead of a field and associate all tags with the list ('No' by default)
  * *TagName* (optional): Name of the correspinding tag(s) (If not specified, *Name* will be used)


## Enum
Used to generate an enum by predefined members.

Attributes:
  * *Name* (required): Name of the generated enum
  * *Members* (required): Comma Separated Member Names
  * *Access* (optional): Access modifier for the generated enum (*internal* by default)

```xml
<Enum Name="eBlood" Members="A, B, AB, O" />
```


# Misc
All tags have a string attribute named *Desc* which describes the tag and it will be used as a comment in the generated code.

If *Desc* is too long or it is better to be explained in multiple lines, **Desc** tag would be used instead of *Desc* attribute.


## Desc
Used to describe a tag in a multi line fashion. It contains multiple **Line** tags.

```xml
<Desc>
   <Line Value="Line #1" />
   <Line Value="Line #2" />
   <Line Value="Line #3" />
</Desc>
```


## Line
It has a *Value* attribute which is a string used as a single line of documentation.

