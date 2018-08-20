# Luthor

Extract structure from any text using a generic lexer.

[Available on NuGet](https://www.nuget.org/packages/Luthor/).

Using *Luthor* you can quickly (and easily) convert any text string into a sequence of ```Token```, each of which represents an instance of a particular ```TokenType``` with a ```Content``` property holding the entire 'run' of that type of token.

In essence you get a sequence of things which represent the contents in a higher level of abstraction, allowing you to process the text further without having to worry about the specifics of the text as you go (a classic example would be a parser).

For example:

```
Sample text.\nAcross 2 lines.
```

This gives a list of tokens like this:

```
TokenType.Letters    - "sample"
TokenType.Whitespace - " "
TokenType.Letters    - "text"
TokenType.Symbols    - "."
TokenType.EOL        - "\n"
TokenType.Letters    - "Across"
TokenType.Whitespace - " "
TokenType.Digits     - "2"
TokenType.Whitespace - " "
TokenType.Letters    - "lines"
TokenType.Symbols    - "."
TokenType.EOF        - ""
```

Now your code can look at words, or keywords, or identifiers, or whatever it's context is expecting. No need to split text or manually hack at it.

## Usage

* Create a scanner
* Pass it to Luther
* Request the output

```cs
var scanner = new Scanner(sourceAsString);
var lexer = new Lexer(scanner);
var result = lexer.GetTokens();
```

There's an optional parameter to ```GetTokens```, which is ```compressWhitespace``` (defaults to ```false```). If set, then runs of whitespace are all compressed down to a single space.

## Output

* Linux/Unix, Mac OS, and Windows all have a ```\n``` as part (or all) of their line endings. Since Mac OS arrived, none of them rely solely on ```\r```. Therefore any ```\r``` characters are ignored entirely - they are neither in the output, nor do they impact column numbering.
* Regardless of the presence/absence of ```Ctrl+Z``` or similar, the output will always end with an EOF token.

### Token types

* Whitespace - spaces, tabs
* Letters - upper and lower case English alphabet
* Digits - 0 to 9
* Symbols - any of !£$%^&*()-_=+[]{};:'@#~,.<>/?\|
* String - anything enclosed in either ", ', or `
* Other - input characters not covered by other types
* EOL - a ```\n```, regardless of whether ```\r``` was present 
* EOF - automatically added
