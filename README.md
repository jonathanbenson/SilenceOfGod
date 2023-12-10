
# Silence of God

<p align="center">
    <img src="sog/res/sog.png" alt="drawing" width="200"/>
</p>

## Introduction

Silence of God is a prototypical application designed to enable users to read the King James Bible using voice commands. Developed using Microsoft .NET WPF, this application offers an innovative and interactive way to engage with the scriptures. The logo was generated with OpenAI's DALLE-3.

## Project Structure

The project is organized into three main directories:

- `sog`: This directory contains the WPF application.
- `sog.src`: Located in this directory are the source files of the application, structured as a class library.
- `sog.test`: This directory houses the MSTest test files for the application.

## Getting Started

To use Silence of God, follow these steps:

1. **Launch the Application**: Start the application from the `sog` directory with the `dotnet run` command.
2. **Enter Voice Mode**: On the application's main screen, activate voice mode to start using voice commands.
3. **Start Listening**: Say 'start' for the program to begin listening to your commands.
4. **Stop Listening**: Say 'stop' for the program to stop listning to your commands, or simply deactivate voice mode in top left.

## Voice Commands

Here are some key voice commands to interact with the application:

- **exit**: Closes the current window.
- **help**: Opens a helpful window with instructions.
- **contents**: Opens the table of contents of the Bible.
- **search {book} {chapter} verse {verse}**: Navigate to a specific verse in the Bible. For example:
  - "search Genesis" navigates to Genesis 1:1
  - "search Matthew 1" navigates to Matthew 1:1
  - "search John 3 verse 16" navigates to John 3:16
  - "search First Corinthians 15 verse 1" navigates to 1 Corinthians 15:1
- **next {n}**: Go forward 'n' pages. For instance, "next 5" goes forward 5 pages.
- **back {n}**: Go back 'n' pages. For example, "back 5" goes backwards 5 pages.


## Sources
- Microsoft .NET WPF tutorial
    - https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/?view=netdesktop-8.0
- KJV bible in json
    - https://github.com/aruljohn/Bible-kjv
- .NET gitignore boilerplate
    - https://github.com/dotnet/core/blob/main/.gitignore