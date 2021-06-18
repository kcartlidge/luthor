# Luthor

Extract structure from any text using a tokenising lexer.

- [Available on NuGet](https://www.nuget.org/packages/Luthor/)
- [View the change log](./CHANGELOG.md)
- [View the license](./LICENSE.txt)

Using *Luthor* you can convert any single or multiple line text into a collection containing runs of token types and their content. This provides access to the content at a higher level
of abstraction, allowing further processing without having to worry about
the specifics of the raw text.

For each token you get the *offest*, the *line* number, the *column* within the line, and the *content*.

For example:

```
Sample text.
Across 3 lines.
With a "multi 'word' string".
```

This gives a list of tokens like this (also including line number etc):

``` json
Letters    : "Sample"
Whitespace : " "
Letters    : "text"
Symbols    : "."
EOL        : \n
Letters    : "Across"
Whitespace : " "
Digits     : "3"
Whitespace : " "
Letters    : "lines"
Symbols    : "."
EOL        : \n
Letters    : "With"
Whitespace : " "
Letters    : "a"
Whitespace : " "
String     : ""multi 'word' string""
Symbols    : "."
EOF        : ""
```

* Note the difference between `Letters` and `String`, the latter of which is quoted (single, double, or backticks) and can have other quotation symbols embedded within it.

This means that instead of having to understand a stream of plain text your code can deal in tokens, making your next steps simpler by working at a higher abstraction level.

## Usage

*To get the tokens from a given source text:*

``` csharp
var tokens = new Lexer(sourceAsString).GetTokens();
```

*To do the same, but with each whitespace run compressed to a single space:*

``` csharp
var tokens = new Lexer(sourceAsString).GetTokens(true);
```

## The output tokens

### Token types

These are the default definitions of the available tokens.

* *Whitespace* - spaces, tabs
* *Letters* - upper and lower case English alphabet
* *Digits* - `0` to `9`
* *Symbols* - any of `!£$%^&*()-_=+[]{};:'@#~,.<>/?\|`
* *String* - anything enclosed in either `"`, `'`, or a backtick
* *Other* - input characters not covered by other types
* *EOL* - an LF (`\n`); any CRs (`\r`) are ignored
* *EOF* - automatically added

### Redefining the tokens

*You can change the characters underlying the different token types:*

``` csharp
var lexer = new Lexer(sourceAsString)
{
    Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
    Digits = "0123456789",
    Symbols = "!£$%^&*()-_=+[]{};:'@#~,.<>/?\\|",
    Whitespace = " \t",
    Quotes = "'\"`",
};
var tokens = lexer.GetTokens();
```

The `Quotes` characters are handled differently from the others.
Each one represents a valid start/end character ('terminators'), and the same character must be used to close the string as to open it.

Other quote characters within the string (i.e. between the terminators) are considered plain content within the current string rather than terminators in their own right.

### General comments

- Linux/Unix, Mac OS, and Windows all have a `\n` (LF) in their line endings,
  so `\r` (CR) is discarded and won't appear in any tokens.
- There will always be a final EOF token, even for an empty input string.
