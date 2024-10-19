# Schuub.Delphi11
C# library for wrapping Delphi types and executing Delphi BPL functions.

About
---
This demonstrates an answer to [this stack overflow question](https://stackoverflow.com/questions/1394298/how-do-i-call-delphi-functions-in-a-bpl-from-an-executable) - 
"How do I call Delphi BPL functions from a programming language/environment totally unrelated to Delphi?"

Use
---
FUTURE: when it's actually usable, document it

Development
---
Open `Schuub.Delphi11.sln` in Visual Studio 2022 and run the unit tests.

Or be a weirdo and use VS Code.

Dependencies
---
* .NET 8 SDK for tests - you need to download and install this
* Delphi 11 BPLs - included in submodule
* TCC (Tiny C Compiler) - included in repo
* Works on x86 or x64
* FUTURE: Linux support?

Release
---
FUTURE: make it a nuget package

Origin
---
This project idea started in 2024 at my day job writing C# and Delphi code 
for [SEL Grid Configurator](https://selinc.com/products/5037/). I wagered I could 
single-handedly eliminate all the product's Delphi code and replace it 
with equivalent C# code. But the wager had limitations:
* My bosses could never justify paying for such a project - I'd have to do it on my own "for fun".
* Even I handed them a finished solution on a silver platter, it could never be 
accepted unless proven to have identical behavior as the old solution.
* I couldn't use work computers outside of work hours - that's bad for my marriage.
* I couldn't bring source or tools from work home to private computers - that's 
very much against work policy.
* I couldn't glance at the source before leaving work and write down the closest-equivalent 
I can remember in C# when I get home - that would more or less violate my NDA by enabling 
competitors and black-hat actors to practically see our source.
* I couldn't even publicly reverse-engineer the publicly-released product because 
that probably conflicts with the noncompete clause of my employment agreement if it 
enables others to see/leverage the results (UPDATE: noncompetes were banned in the US 
in late 2024! I might need to revisit this thinking!)

What could I do?

I could pursue this project using the Delphi runtime source that is freely available 
with Delphi Community Edition, and verify it against the behavior of the BPLs that ship 
with the same! And it would still be valuable for the work wager because the 
runtime functions need equivalents in C#. Have you seen some of the weird features in 
Delpih due to historical design decisions that are maintained for backward compatibility?
* TStringList "Name=Value" and CommaText
* StrUtils with special behavior for null terminator characters
* Even the float-to-string formatting and string-to-float conversions are not likely 
to be the 100% same in other languages

So... this repo represents just another crazy hobby project idea of mine 😂

Licensing
---
The contents of this repo are free and unencumbered software released into the public domain 
under The Unlicense. You have complete freedom to do anything you want with the software, 
for any purpose. Please refer to <http://unlicense.org/> .
