# CodeReview.FileConverter

Docker image: https://hub.docker.com/r/godeltech/codereview.file-converter

## Description

CodeReview.FileConverter is a tool that converts output of various code analysis tools into unified format.
The tool is designed to convert output of code analysis tools into unified format that can be used by CodeReview.Evaluator.

## Usage

### Commands And Parameters

#### roslyn
Convert .NET Compliper output into unified format
<pre>
> dotnet CodeReview.FileConverter.dll roslyn -s ./src -f ./files -o output.json
</pre>

| Agruments      | Key | Required | Type   | Description agrument                                                                                         |
|----------------|-----|----------|--------|--------------------------------------------------------------------------------------------------------------|
| --dictionaries | -d  | false    | string | Path to folder containing dictionaries with additional information about issues. "./Dictionaries" by dafault |
| --src          | -s  | true     | string | Prefix in file path which needs to be removed from location value                                            |
| --folder       | -f  | true     | string | Path to folder or file to process                                                                            |
| --mask         | -m  | false    | string | Search mask used to look for files within folder. "*" by defalut                                             |
| --recurse      | -r  | true     | bool   | Specifies if recurse search must be used for for files in folder. True by defalut                            |
| --output       | -o  | true     | string | Output file path                                                                                             |

#### resharper
Convert JetBrains ReSharper output into unified format
<pre>
> dotnet CodeReview.FileConverter.dll resharper -f ./files -o output.json
</pre>

| Agruments | Key | Required | Type   | Description agrument                                                              |
|-----------|-----|----------|--------|-----------------------------------------------------------------------------------|
| --folder  | -f  | true     | string | Path to folder or file to process                                                 |
| --mask    | -m  | false    | string | Search mask used to look for files within folder. "*" by defalut                  |
| --recurse | -r  | true     | bool   | Specifies if recurse search must be used for for files in folder. True by defalut |
| --output  | -o  | true     | string | Output file path                                                                  |
        
#### cloc
Convert cloc tool YAML output into format supported by Evaluator
<pre>
> dotnet CodeReview.FileConverter.dll cloc -f ./files -o output.json -p ./
</pre>

| Agruments | Key | Required | Type   | Description agrument                             |
|-----------|-----|----------|--------|--------------------------------------------------|
| --file    | -f  | true     | string | Path to file containing YAML output of cloc tool |
| --output  | -o  | true     | string | Output file path                                 |
| --prefix  | -p  | false    | string | File path prefix to remove. "./" by default      |

#### dependencyCheck
Convert OWASP Dependency check output into unified format
<pre>
> dotnet CodeReview.FileConverter.dll dependencyCheck -f ./files -o output.json
</pre>

| Agruments | Key | Required | Type   | Description agrument                                                              |
|-----------|-----|----------|--------|-----------------------------------------------------------------------------------|
| --folder  | -f  | true     | string | Path to folder or file to process                                                 |
| --mask    | -m  | false    | string | Search mask used to look for files within folder. "*" by defalut                  |
| --recurse | -r  | true     | bool   | Specifies if recurse search must be used for for files in folder. True by defalut |
| --output  | -o  | true     | string | Output file path                                                                  |

## License

This project is licensed under the MIT License. See the LICENSE file for more details.