# ✨ MediatR Extensions for JetBrains Rider & ReSharper

[![Rider Plugin](https://img.shields.io/jetbrains/plugin/v/ca.nosuchcompany.rider.plugins.mediatr)](https://plugins.jetbrains.com/plugin/18313-mediatr-extensions)
[![ReSharper Plugin](https://img.shields.io/resharper/v/ca.nosuchcompany.mediatrplugin)](https://plugins.jetbrains.com/plugin/18347-mediatr-extensions)

Boost your navigation superpowers in Rider and ReSharper with this plugin for Mediator-based applications. Quickly jump between `IRequest`, `INotification`, and their respective handlers—like teleportation for your CQRS architecture.

---

## 🚀 Features

- Supports JetBrains Rider & ReSharper
- Navigate from `IRequest` / `INotification` to their handlers for both [MediatR](https://github.com/jbogard/MediatR) and [Mediator](https://github.com/martinothamar/Mediator) NuGet packages

---

## 🏗️ Plugin Architecture

### System Overview

```mermaid
graph TB
    subgraph "IDE Integration Layer"
        A1[Rider Action<br/>GoToHandlrAction.kt]
        A2[ReSharper Provider<br/>MediatorRequestNavigateFromHereProvider.cs]
    end
    
    subgraph "Core Navigation Engine"
        B1[HandlerSelector<br/>Service]
        B2[Navigation<br/>Coordinator]
    end
    
    subgraph "Library Abstraction Layer"
        C1[ILibrary<br/>Interface]
        C2[MediatRLibrary<br/>Implementation]
        C3[MediatorLibrary<br/>Implementation]
    end
    
    subgraph "External Libraries"
        D1[MediatR<br/>NuGet Package]
        D2[Mediator<br/>NuGet Package]
    end
    
    A1 --> B1
    A2 --> B1
    B1 --> B2
    B2 --> C1
    C1 --> C2
    C1 --> C3
    C2 -.-> D1
    C3 -.-> D2
```

### How It Works

#### Entry Points
The plugin provides two main entry points for navigation:

1. **Rider Integration** (`GoToHandlrAction.kt`): A Kotlin-based action that integrates with Rider's action system
2. **ReSharper Integration** (`MediatorRequestNavigateFromHereProvider.cs`): A C# provider that adds "Go to handler" to ReSharper's context navigation

#### High-Level Workflow

1. **Detection**: User right-clicks on an `IRequest` or `INotification` interface
2. **Context Analysis**: Plugin identifies the selected element and validates it's a mediator request/notification
3. **Library Resolution**: Determines whether the project uses MediatR or Mediator library
4. **Handler Discovery**: Searches the solution for corresponding `IRequestHandler` or `INotificationHandler` implementations
5. **Navigation**: Opens the handler class in the editor

#### Library Abstraction

The plugin uses an abstraction layer to support both MediatR and Mediator libraries:

**Common Interface (`ILibrary`)**:
- `FindHandlers()`: Locates handler implementations for a given request/notification
- `IsSupported()`: Determines if the library can handle the current context
- `CreateHandlrFor()`: Generates new handler classes (code generation feature)

**MediatR Support**:
- Handles `MediatR.IBaseRequest` and `MediatR.INotification`
- Supports `IRequestHandler<T>`, `IRequestHandler<T,TResponse>`, and `INotificationHandler<T>`
- Full integration with MediatR's interface contracts

**Mediator Support**:
- Handles `Mediator.IBaseRequest` and `Mediator.INotification`  
- Supports the same handler patterns as MediatR but for the Mediator library
- Provides identical functionality with different underlying type system

This abstraction allows the plugin to work seamlessly with both libraries without requiring users to configure which one they're using—the plugin automatically detects and adapts to the project's mediator implementation.

---

## 🖥️ How to Run

### 🪟 Windows

```powershell
# Open the solution
MediatorPlugin.slnx

# Ensure NuGet feed is available
https://api.nuget.org/v3/index.json

# In PowerShell:
./buildPlugin.ps1
./runVisualStudio.ps1
```

Once the new Visual Studio instance opens, go to:  
`ReSharper > Extension Manager`  
The MediatR extension should appear as version `9999.0`.

> 💡 You may need to remove the Rider project if it causes the build to fail.

---

### 🍏 macOS

```bash
./gradlew :runIde
```

---

## 👨‍💻 Contributions

Pull requests and plugin ideas are always welcome!  
If you're using this in your own workflow, we’d love to hear about it.

---

📦 JetBrains Marketplace:
- [Rider Plugin](https://plugins.jetbrains.com/plugin/18313-mediatr-extensions)
- [ReSharper Plugin](https://plugins.jetbrains.com/plugin/18347-mediatr-extensions)
