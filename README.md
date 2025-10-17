# MAUI Calendar and To-Do App

A cross-platform application built with .NET MAUI that combines calendar functionality with a task manager (to-do list). It allows users to create, view, and manage daily events and tasks through an intuitive interface optimized for both desktop and mobile devices.

[![App Demo](https://img.youtube.com/vi/sZxCuqYU_3A/0.jpg)](https://youtube.com/shorts/sZxCuqYU_3A?feature=share)

---

## Features

- Create and manage calendar events
- Add, edit, and delete to-do tasks
- Cross-platform support (Windows, macOS, Android, iOS)
- Intuitive and responsive UI
- Data persistence across sessions
- Date navigation and event filtering

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (version 17.8 or later) or [Visual Studio Code](https://code.visualstudio.com/)
- For mobile development:
  - Android: Android SDK API level 21 or higher
  - iOS: Xcode 14.3 or higher (macOS only)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/yourusername/MAUI-TaskCalendar.git
cd MAUI-TaskCalendar
```

### Build and Run

1. Open `Schdeuler.sln` in Visual Studio
2. Select your target platform (Android, iOS, Windows, macOS)
3. Press F5 or click "Start Debugging"

Alternatively, using .NET CLI:

```bash
cd Schdeuler
dotnet build
dotnet run
```

### Running on Mobile Devices

For Android:
```bash
dotnet build -t:Run -f net8.0-android
```

For iOS (on macOS):
```bash
dotnet build -t:Run -f net8.0-ios
```

## Project Structure

```
MAUI-TaskCalendar/
├── Schdeuler/                 # Main application project
│   ├── Platforms/             # Platform-specific code
│   ├── Resources/             # Images, fonts, styles
│   ├── ViewModel/             # Business logic and data handling
│   ├── App.xaml               # Application entry point
│   ├── MainPage.xaml          # Main application page
│   └── MauiProgram.cs         # Dependency injection setup
├── README.md                  # This file
└── .gitignore                 # Git ignore rules
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/)
- Inspired by modern calendar and task management applications