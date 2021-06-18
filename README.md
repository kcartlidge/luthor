# Luthor

Extract structure from any text using a tokenising lexer.

[Available on NuGet](https://www.nuget.org/packages/Luthor/).

Using *Luthor* you can convert any single or multiple line text into a collection containing runs of token types and their content. This provides access to the content at a higher level
of abstraction, allowing further processing without having to worry about
the specifics of the raw text.

For each token you get the *offest*, the *line* number, the *column* within the line, and the *content*.

For example:

```
Sample text.
Across 3 lines.
With a "string".
```

This gives a list of tokens like this (also including line number etc):

``` ini
Letters    = "Sample"
Whitespace = " "
Letters    = "text"
Symbols    = "."
EOL        = \n
Letters    = "Across"
Whitespace = " "
Digits     = "3"
Whitespace = " "
Letters    = "lines"
Symbols    = "."
EOL        = \n
Letters    = "With"
Whitespace = " "
Letters    = "a"
Whitespace = " "
String     = ""string""
Symbols    = "."
EOF        = ""
```

* Note the difference between `Letters` and `String`, the latter of which is quoted (single, double, or backticks) and can have other quotation symbols embedded within it.

This means that instead of having to understand a stream of plain text your code
can instead deal in tokens where that text has already been sensibly split - in many cases even discarding the `Whitespace` and/or `EOL` tokens entirely, making your next steps easier still.

*Note that this uses a pre-defined set of tokens based on the English alphabet. See below for details.*

## Usage

To get the tokens from a given source text:

``` cs
var tokens = new Lexer(sourceAsString).GetTokens();
```

To do the same, but with whitespace compressed to single spaces:

``` cs
var tokens = new Lexer(sourceAsString).GetTokens(true);
```

## The output tokens

### General comments

- Linux/Unix, Mac OS, and Windows all have a `\n` (LF) in their line endings,
  so `\r` (CR) is discarded and won't appear in any tokens.
- There will always be a final EOF token, even for an empty input string.

### Token types

* *Whitespace* - spaces, tabs
* *Letters* - upper and lower case English alphabet
* *Digits* - `0` to `9`
* *Symbols* - any of `!£$%^&*()-_=+[]{};:'@#~,.<>/?\|`
* *String* - anything enclosed in either `"`, `'`, or a backtick
* *Other* - input characters not covered by other types
* *EOL* - an LF (`\n`); any CRs (`\r`) are ignored
* *EOF* - automatically added
