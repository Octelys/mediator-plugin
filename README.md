# ✨ MediatR Extensions for JetBrains Rider & ReSharper

[![Rider Plugin](https://img.shields.io/jetbrains/plugin/v/ca.nosuchcompany.rider.plugins.mediatr)](https://plugins.jetbrains.com/plugin/18313-mediatr-extensions)
[![ReSharper Plugin](https://img.shields.io/resharper/v/ca.nosuchcompany.mediatrplugin)](https://plugins.jetbrains.com/plugin/18347-mediatr-extensions)

Boost your navigation superpowers in Rider and ReSharper with this plugin for Mediator-based applications. Quickly jump between `IRequest`, `INotification`, and their respective handlers—like teleportation for your CQRS architecture.

---

## 🚀 Features

- Supports JetBrains Rider & ReSharper
- Navigate from `IRequest` / `INotification` to their handlers for both [MediatR](https://github.com/jbogard/MediatR) and [Mediator](https://github.com/martinothamar/Mediator) NuGet packages

---

## 🖥️ How to Run

### 🪟 Windows

```powershell
# Open the solution
MediatorPlugin.sln

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
