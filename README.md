# Codacy C# Coverage Reporter

[![Codacy Badge](https://api.codacy.com/project/badge/grade/62d12b85e3ea42e080d9a3c3dfa7a5bf)](https://www.codacy.com/app/Codacy/csharp-codacy-coverage)
[![Codacy Badge](https://api.codacy.com/project/badge/coverage/62d12b85e3ea42e080d9a3c3dfa7a5bf)](https://www.codacy.com/app/Codacy/csharp-codacy-coverage)
[![Circle CI](https://circleci.com/gh/codacy/csharp-codacy-coverage.svg?style=shield)](https://circleci.com/gh/codacy/csharp-codacy-coverage)

[Codacy](https://codacy.com/) coverage support for C#. This tool uses Codacy API to send the coverage report parsed from OpenCover or dotCover coverage tools.

## Build

1. Do `dotnet restore` ;
2. And then `dotnet build` .

## Usage

This is the parameters list:

- `-c`, `--commit`: Specify the commit UUID _(Required)_
- `-t`, `--token`: Specify the project token _(Required)_
- `-r`, `--report`: Path to the coverage report _(Required)_
- `-e`, `--engine`: Engine Report Type (dotcover, opencover). _(Required)_
- `-p`, `--partial`: Send report as a partial report _(Default: false)_
- `-f`, `--final`: Send final coverage request _(Default: false)_
- `--help`: Display this help screen.
- `--version`: Display version information.

#### Detecting commit UUID

On CI from Environment Variables:

- CI_COMMIT
- TRAVIS_PULL_REQUEST_SHA
- TRAVIS_COMMIT
- DRONE_COMMIT
- CIRCLE_SHA1
- CI_COMMIT_ID
- WERCKER_GIT_COMMIT
- CODEBUILD_RESOLVED_SOURCE_VERSION
- SCRUTINIZER_SHA1
- REVISION

### Windows w/ compressed archive

```ps
Invoke-WebRequest -Uri https://github.com/codacy/csharp-codacy-coverage/releases/download/$env:CODACY_COVERAGE_VERSION/csharp-codacy-coverage_win-x64.zip -OutFile csharp-codacy-coverage_win-x64.zip
Expand-Archive csharp-codacy-coverage_win-x64.zip -DestinationPath C:\temp\Codacy.CSharpCoverage\

C:\temp\Codacy.CSharpCoverage\Codacy.CSharpCoverage.exe -c $env:REPO_COMMIT_UUID -t $env:CODACY_PROJECT_TOKEN -r coverage.xml -e opencover
```

### Windows w/ self-contained binary

```ps
Invoke-WebRequest -Uri https://github.com/codacy/csharp-codacy-coverage/releases/download/$env:CODACY_COVERAGE_VERSION/Codacy.CSharpCoverage_win-x64.exe -OutFile C:\temp\Codacy.CSharpCoverage_win-x64.exe

C:\temp\Codacy.CSharpCoverage_win-x64.exe -c $env:REPO_COMMIT_UUID -t $env:CODACY_PROJECT_TOKEN -r MyProject_coverage.xml -e opencover
```

### Linux or macOS w/ compressed archive

```bash
curl -Ls https://github.com/codacy/csharp-codacy-coverage/releases/download/$CODACY_COVERAGE_VERSION/csharp-codacy-coverage_<linux|osx>-x64.tar.gz --output csharp-codacy-coverage_<linux|osx>-x64.tar.gz
tar xvzf csharp-codacy-coverage_<linux|osx>-x64.tar.gz -C /tmp/Codacy.CSharpCoverage/
/tmp/Codacy.CSharpCoverage/Codacy.CSharpCoverage.CSharpCoverage -c $REPO_COMMIT_UUID -t $CODACY_PROJECT_TOKEN -r MyProject_coverage.xml -e opencover
```

### Linux or macOS w/ self-contained binary

```bash
curl -Ls https://github.com/codacy/csharp-codacy-coverage/releases/download/$CODACY_COVERAGE_VERSION/Codacy.CSharpCoverage_<linux|osx>-x64 --output Codacy.CSharpCoverage_<linux|osx>-x64
chmod +x ./Codacy.CSharpCoverage_<linux|osx>-x64
./Codacy.CSharpCoverage_<linux|osx>-x64 -c $REPO_COMMIT_UUID -t $CODACY_PROJECT_TOKEN -r MyProject_coverage.xml -e opencover
```

## FAQ

### What's the difference between the compressed archive and self-contained binary

The compressed archive is compiled using `netcoreapp2.2` framework and has all
`.dll` dependencies inside the archive. The self-contained binary is built under
the `netcoreapp3.0` framework which compiles to a single binary containing all
dependencies bundled insides. That could be useful for portability.

## License

Apache License Version 2.0, January 2004

## What is Codacy

[Codacy](https://www.codacy.com/) is an Automated Code Review Tool that monitors your technical debt, helps you improve your code quality, teaches best practices to your developers, and helps you save time in Code Reviews.

### Among Codacy’s features

- Identify new Static Analysis issues
- Commit and Pull Request Analysis with GitHub, BitBucket/Stash, GitLab (and also direct git repositories)
- Auto-comments on Commits and Pull Requests
- Integrations with Slack, HipChat, Jira, YouTrack
- Track issues in Code Style, Security, Error Proneness, Performance, Unused Code and other categories

Codacy also helps keep track of Code Coverage, Code Duplication, and Code Complexity.

Codacy supports PHP, Python, Ruby, Java, JavaScript, and Scala, among others.

### Free for Open Source

Codacy is free for Open Source projects.
