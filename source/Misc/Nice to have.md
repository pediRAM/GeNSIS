
# todo:
- Code Auto-Gen. für Firewall-Rules + YesNo-MsgBox anwenden! + Testen!
# BUGs:

1. ~~Beim Auto-Add-All-From-Folder: Verzeichnisse als Verzeichnis kopieren~~
2. User muss sofort sehen wohin die Section kopiert wird
3. Mit Pecopaliss kleines Test-Installer erstellen + Installieren und Deinstallieren TESTEN!
4. ShortCut als eigene Sektion??? Alternativen? Optionen? TODO
5.  Die Bilder von Installer unter Design TabItem erweitern
6.  Doku in English + PDF + auf Github
7.  strong name: https://learn.microsoft.com/en-us/dotnet/standard/assembly/sign-strong-name
8.  Batch-Scripts aufrufen
9.  Service in windows registrieren + deregistrieren
10. Firewall-Rulename in windows einfügen + entfernen

# Tipps von Chat GPT4
It looks like you've introduced a shorthand or template language for describing common programming elements in object-oriented programming, such as classes, methods, properties, and specific data types. This kind of templating can be very useful for quickly sketching out structures or for generating code in different languages using a consistent notation.

To clarify, the purpose of this syntax is to provide a quick and concise way to define class structures (or potentially other data types like structs, enums, and bitsets) and their associated members, right?

Given your description, I understand the basic principles:

1. **Type Declarations**: Different types can be defined using keywords like `class`, `enum`, `flags`, and `struct`.
  
2. **Program Elements**: You have defined shorthand for various program elements like `event`, `function`, `method`, and `property`.
  
3. **Data Types**: There are shorthand notations for a number of common data types, and it looks like unrecognized data types are left as-is.
  
4. **Examples**: The examples provided demonstrate how one can use verbose, short, or middle verbose syntaxes to define the same structure.

This templating is a good starting point. However, a few considerations might be useful:

- **Scoping and Access Modifiers**: Consider how you might incorporate access modifiers (like `public`, `private`, `protected`) or static members.
  
- **Function and Method Bodies**: Do you plan to include any mechanism for detailing the logic inside functions and methods, or is this strictly for structure definition?
  
- **Default Values**: It could be useful to have a way to provide default values for properties.
  
- **Attributes or Annotations**: Many programming languages use attributes or annotations to provide meta-information about classes, methods, or properties. Consider if you'd like a way to add these to your template.
  
- **Extensions or Plugins**: If this is intended to be used in a broader tool or system, consider how others might extend or adapt the template for more specific or specialized use cases.

If you have specific questions or would like further elaboration on any part of this template language, please let me know!

# Converter die nicht mehr benötigt werden => Weg!

MessageBox: https://nsis.sourceforge.io/Reference/MessageBox

Install per User: https://nsis.sourceforge.io/Examples/install-per-user.nsi

Service Registrieren:

Flaggen: https://flagpedia.net/angola

Multi Language: https://nsis.sourceforge.io/Examples/Modern%20UI/MultiLanguage.nsi

Components page: https://nsis.sourceforge.io/Docs/Modern%20UI%202/Readme.html
-//- selection change: https://documentation.help/CTRE-NSIS/Section4.7.html#4.7.2.1.8


Environment Variables (APPDATA on Windows => location of per-user configuration)

execution of batch files in installer: https://stackoverflow.com/questions/3265141/executing-batch-file-in-nsis-installer

BEST Practices: https://nsis.sourceforge.io/Best_practices

Licenses (list of licenses): https://choosealicense.com/licenses/

NSIS command line usage: https://nsis.sourceforge.io/Docs/Chapter3.html

NSIS modern gui: https://nsis.sourceforge.io/Docs/Modern%20UI/Readme.html

RMDir /r /REBOOTOK NameOfMyDirectory  --> https://nsis.sourceforge.io/Reference/RMDir

C# new Switch expressions since C# 7 or 8: https://codebuns.com/csharp-advanced/csharp-8-switch-expressions/

