# AGENTS.md

ReSharper + Rider plugin for MediatR/mediator codebases. Two halves that ship together:
a .NET backend (`src/dotnet`) and a Kotlin/IntelliJ frontend (`src/rider`), wired over a
generated protocol (`protocol/`).

## Layout

| Path | What it is |
| --- | --- |
| `src/dotnet/MediatorPlugin` | Backend: actions, navigations, diagnostics, services |
| `src/dotnet/MediatorPlugin.Tests` | Backend tests (ReSharper test framework) |
| `src/rider/main` | Kotlin frontend + `META-INF/plugin.xml` |
| `protocol` | RD protocol model shared by both halves |
| `test-solution` | Sample solution the tests and manual runs operate on |

## Build and test

```bash
dotnet test src/dotnet/MediatorPlugin.Tests/MediatorPlugin.Tests.csproj
./gradlew :prepareSandbox -PPluginVersion=<version>   # full plugin build
```

- **Everything targets `net472`.** The ReSharper test framework needs the .NET Framework
  runtime, so the backend tests only run on Windows — CI runs them on `windows-latest`
  while the Gradle build runs on Ubuntu. Don't "fix" a test failure that is really a
  platform mismatch.
- `MediatorPlugin` has **two project files** in the same directory:
  `MediatorPlugin.csproj` (ReSharper) and `MediatorPlugin.Rider.csproj` (Rider). A change
  to one usually belongs in both; check before assuming a single build covers you.
- `PluginVersion` in `gradle.properties` and the `<version>` in `plugin.xml` are
  `_PLACEHOLDER_` on purpose. CI stamps them via `update-version.sh`. Never commit a
  concrete version there.

## Conventions

- Branches: `features/<slug>` or `bugs/<slug>`, PRs target `main`.
- Tests must follow [docs/unit-test-guidelines.md](docs/unit-test-guidelines.md):
  `MethodName_ScenarioInPresentTense_ExpectedResult` naming, AAA section comments, and
  one call per line in fluent chains. Read it before writing or editing a test class.

## File editing

- Modify files with your editor/patch tools (str-replace, create-file, apply-patch).
- Do not write files through the shell: no heredocs (`cat > file <<'EOF'`), no `echo >`,
  `printf >`, `tee`, or `sed -i`. Shell redirection into tracked files is prohibited.
- The shell is for read-only inspection (`cat`, `grep`, `ls`, `find`) and for running
  builds, tests, and git commands.
