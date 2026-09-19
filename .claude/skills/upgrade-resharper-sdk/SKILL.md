---
name: upgrade-resharper-sdk
description: "v0.2.0 — Upgrades this plugin to a new ReSharper/Rider SDK version: bumps ProductVersion and SdkVersion in lockstep, re-verifies the derived Wave range, raises the Gradle/Kotlin/IntelliJ-platform toolchain the new SDK demands, fixes the resulting build and classpath breaks, updates the changelog, and verifies with the task CI actually runs. When the requested build isn't yet published on both sides, offers the newest common fallback instead of just stopping. Use when a new ReSharper or Rider release or EAP needs to be supported."
---

# Upgrade the ReSharper / Rider SDK

You are a **Plugin Maintainer**.

This plugin ships two halves that must target the *same* IDE build: a .NET backend
(`src/dotnet`, against the ReSharper SDK NuGet package) and a Kotlin/IntelliJ frontend
(`src/rider`, against the Rider SDK resolved by the IntelliJ Platform Gradle Plugin).
An SDK upgrade is the coordinated bump of both, plus whatever the new SDK's own toolchain
requirements drag in behind it.

Steps 1–4 are always required. Steps 5–7 are conditional: do them when the upgrade
demands them, and say plainly when it did not.

---

## Required output artifact

A working tree in which:

- `ProductVersion` and `SdkVersion` both name the new IDE build,
- `./gradlew :prepareSandbox` succeeds against the real new SDK,
- `CHANGELOG.md` records the bump, and
- `PluginVersion` and `plugin.xml`'s `<version>` are **still `_PLACEHOLDER_`**.

Nothing is committed, pushed, or released.

---

## Inputs

| Input | Required | Description |
|---|---|---|
| Target IDE version | Yes | The ReSharper/Rider build to support, e.g. `2026.2`, `2026.1-EAP6`. |
| Channel | Yes — ask if ambiguous | `release`, `EAP`, or `nightly`. Decides how the version is spelled in step 2. |
| Issue | No | The GitHub issue requesting the support, for the changelog line. |

Do not infer "the latest". Ask for the target version if it was not given, and confirm the
build actually exists before editing anything (step 1).

---

## Hard constraints

1. **Never touch the version placeholders.** `PluginVersion` in `gradle.properties` and
   `<version>` in every `plugin.xml` are `_PLACEHOLDER_` on purpose; CI stamps them via
   `update-version.sh`. An SDK upgrade never commits a concrete value there.
2. **`ProductVersion` and `SdkVersion` move together.** A frontend on one IDE build and a
   backend on another produces a plugin that installs and then fails at runtime.
3. **Do not write files through the shell** — no heredocs, no `echo >`, no `sed -i`, no
   `tee`. Use editor/patch tools. The shell is for inspection, builds, tests, and git.
   (This is the repository-wide rule from `AGENTS.md`.)
4. **Verify against the real SDK.** `./gradlew :prepareSandbox` is the task CI runs; a
   bump is not done until it passes locally. Never report success off a diff alone.
5. **Don't "fix" platform-mismatch test failures.** Everything targets `net472` and the
   ReSharper test framework needs .NET Framework, so `dotnet test` only runs on Windows.
   On macOS/Linux, say the backend tests were not run rather than making them pass.
6. **One project file is never enough.** `MediatorPlugin.csproj` (ReSharper) and
   `MediatorPlugin.Rider.csproj` (Rider) sit in the same directory; a change to one
   usually belongs in both. Check before assuming.

---

## Step 1 — Confirm the target build exists, in both spellings

The Gradle side and the NuGet side name the same build differently, and one of them
routinely lags the other by days. Confirm both before editing:

```bash
# ReSharper SDK (the .NET half) — all published versions
curl -s https://api.nuget.org/v3-flatcontainer/jetbrains.resharper.sdk/index.json
```

For the Rider side, check that the IntelliJ Platform Gradle Plugin can resolve the
product version you intend to write (the releases index at
<https://www.jetbrains.com/intellij-repository/releases> and
<https://www.jetbrains.com/intellij-repository/snapshots> list them).

If the two halves are not both available at the target build — this happens routinely on
the EAP channel, where JetBrains publishes a new Rider snapshot before the matching
ReSharper SDK NuGet package lands, sometimes by several EAP numbers — do not proceed on
the target and do not silently substitute something else either. Find the newest EAP/build
within the same channel that **is** published on both sides (walk the EAP number down from
the requested one) and offer it to the user as a fallback, stating clearly which side was
missing and what the highest common build is. Proceed only once the user picks a build,
whether that's waiting for the target to appear later or taking the offered fallback now.

---

## Step 2 — `gradle.properties`: `ProductVersion`

Spell it per channel; the comment block in the file is the source of truth:

| Channel | Form |
|---|---|
| Release | `2026.2` |
| EAP | `2026.1-EAP6-SNAPSHOT` |
| Nightly | `2026.1-SNAPSHOT` |

This is what `rider(ProductVersion)` in `build.gradle.kts` resolves against.

---

## Step 3 — `src/dotnet/Plugin.props`: `<SdkVersion>`

Must name the same build as step 2, but in **NuGet** spelling, which is different:

| Gradle `ProductVersion` | NuGet `SdkVersion` |
|---|---|
| `2026.2` | `2026.2.0` |
| `2025.3` | `2025.3.0.3` |
| `2026.1-EAP6-SNAPSHOT` | `2026.1.0-eap06` |

Take the exact string from the NuGet index in step 1 — do not construct it by pattern.
The patch component (`2025.3.0.3`) and the zero-padded EAP suffix (`-eap06`) are not
derivable from the Gradle form.

---

## Step 4 — `src/dotnet/Directory.Build.props`: re-verify the derived Wave range

`WaveVersion` / `UpperWaveVersion` are computed by **string-slicing `SdkVersion`**:

```xml
<WaveVersionBase>$(SdkVersion.Substring(2,2))$(SdkVersion.Substring(5,1))</WaveVersionBase>
<WaveVersion>$(WaveVersionBase).0.0.0</WaveVersion>
<UpperWaveVersion>$(WaveVersionBase).9999.0</UpperWaveVersion>
```

Those offsets assume a specific `SdkVersion` shape. When the shape changes — an EAP suffix
appears or disappears, a patch component is added — **the slices silently produce a wrong
Wave range**, and the plugin either refuses to install or claims compatibility it doesn't
have.

Whenever `SdkVersion`'s shape changes, compute the resulting `WaveVersion` by hand and
confirm it matches the wave the target ReSharper build actually ships. There is no
`Wave` constraint anywhere else — no `sinceBuild`/`untilBuild` exists in this repo — so
this range is the *only* thing gating installability.

---

## Step 5 — Raise the build toolchain the new SDK requires *(conditional)*

A new SDK often requires a newer IntelliJ Platform Gradle Plugin, which requires a newer
Gradle, which requires a newer Kotlin. Bump only as far as the SDK forces, and bump the
whole chain together — a partial bump produces confusing mid-chain errors.

| What | Where |
|---|---|
| IntelliJ Platform Gradle Plugin | `build.gradle.kts`, `plugins { id("org.jetbrains.intellij.platform") version "…" }` |
| Gradle wrapper | **both** `build.gradle.kts` `tasks.wrapper { gradleVersion }` **and** `gradle/wrapper/gradle-wrapper.properties` `distributionUrl` |
| Kotlin | `gradle/libs.versions.toml`, `[versions] kotlin` |
| JVM wrapper plugin | `build.gradle.kts`, `me.filippov.gradle.jvm.wrapper` |

Keep the wrapper's `distributionUrl` on the `cache-redirector.jetbrains.com` host and the
`-all.zip` distribution — don't let a wrapper regeneration rewrite it to `services.gradle.org`.

---

## Step 6 — Fix what the toolchain bump breaks *(conditional)*

Work the failures the build reports, but check this catalogue first — each entry is a
known break with a known fix:

- **`Project.exec {}` removed in Gradle 9.** Rewrite as
  `providers.exec { … }.result.get().assertNormalExitValue()`. There are five call sites:
  `setBuildTool`, `compileDotNet`, `testDotNet`, `tasks.buildPlugin`, `tasks.publishPlugin`.
  For the one that reads output (`vswhere.exe` in `setBuildTool`), take it from
  `execOutput.standardOutput.asText.get()` instead of a `ByteArrayOutputStream`.
  **Do not drop the exit-value assertion** — without it a failed `dotnet` invocation is
  silently ignored and the build "succeeds" with no plugin in it.
- **`rider(version, useInstaller = false)` overload removed in platform plugin 2.12+.**
  Becomes `rider(ProductVersion) { useInstaller.set(false) }`.
- **Kotlin can't target the auto-detected toolchain.** The new SDK may pull a much newer
  JDK (2026.2 → JDK 25) that the current Kotlin compiler doesn't recognize as a target.
  Bump Kotlin, and **delete any hardcoded `jvmTarget` override** in
  `tasks.compileKotlin` so Kotlin follows the same toolchain as Java rather than pinning
  a stale one.
- **Gradle can't provision the JDK in CI.** CI installs only Java 17
  (`.github/workflows/cicd.yml`), so anything newer must be auto-provisioned. That needs
  the Foojay resolver in `settings.gradle.kts`:
  ```kotlin
  plugins {
      id("org.gradle.toolchains.foojay-resolver-convention") version "1.0.0"
  }
  ```
  A build that passes locally on a machine that happens to have the JDK will still fail
  in CI without this.

---

## Step 7 — Fix API and classpath breaks from the new IDE layout *(conditional)*

- **Missing platform classes after an IDE re-modularization.** Rider 2026.2 moved
  `RiderAnAction` and its frontend/backend action traits off the default compile
  classpath; they had to be declared explicitly in the `intellijPlatform` dependency block:
  ```kotlin
  bundledModule("intellij.rider.rdclient.dotnet")
  bundledModule("intellij.rd.client")
  ```
  When a platform type stops resolving, the fix is almost always naming its bundled
  module — not downgrading the SDK.
- **Backend API changes.** PSI and SDK behavior can shift between builds, requiring
  adjustments to code that walks the PSI tree or uses identifier-handling APIs. Run the
  backend tests (on Windows) rather than assuming the C# still behaves the same.
- **Packages that become redundant or conflicting.** A dependency the SDK now bundles
  itself may need its own `PackageReference` removed. Remember the two-csproj rule from
  the constraints.

---

## Step 8 — `CHANGELOG.md`

Add an entry at the top, under `# Changelog`, above the previous one, matching the house
format — a `## <yy>.<mm>.<dd>XX` heading and an issue-referencing line:

```markdown
## 26.08.20XX
- Fix issue #117: updated SDK to 2026.2.
```

The `XX` stands in for the CI run number that `update-version.sh` stamps into the real
version; keep it literal.

---

## Step 9 — Verify

```bash
./gradlew :prepareSandbox -PPluginVersion=<version>   # the task CI runs — must pass
dotnet test src/dotnet/MediatorPlugin.Tests/MediatorPlugin.Tests.csproj   # Windows only
```

- `:prepareSandbox` is the gate. It resolves the real SDK, compiles both halves, and
  assembles the sandbox — a bump that hasn't passed it is not done.
- Run the backend tests when on Windows. When not, say so explicitly in the report
  instead of quietly skipping them.
- Re-read the diff for a stray concrete `PluginVersion` or `plugin.xml` `<version>`
  before handing off.

---

## Report back

- The **target build actually applied** — note plainly if it differs from what was
  originally requested because step 1 found a mismatch and the user chose a fallback.
- The two version strings written (`ProductVersion`, `SdkVersion`).
- The computed **Wave range**, and whether step 4's slices needed correcting.
- Which of steps 5–7 applied, and for each break, what the fix was.
- **Verification actually run**: the `:prepareSandbox` result, and the backend test
  result *or* an explicit note that the platform couldn't run them.
- Confirmation that the version placeholders are untouched.

---

## Quality checklist

- [ ] The target build was confirmed to exist on both the NuGet and IntelliJ sides before editing.
- [ ] `ProductVersion` uses the channel-correct form; `SdkVersion` was copied from the NuGet index, not constructed.
- [ ] The derived `WaveVersion` was computed by hand and checked against the target build's wave.
- [ ] Toolchain bumps, where needed, moved platform-plugin / Gradle / Kotlin together, and the Gradle version was changed in *both* places.
- [ ] No hardcoded `jvmTarget` survives alongside an auto-detected toolchain.
- [ ] Every `providers.exec` call site keeps `assertNormalExitValue()`.
- [ ] Changes that belong in both `MediatorPlugin.csproj` and `MediatorPlugin.Rider.csproj` are in both.
- [ ] `CHANGELOG.md` has a dated, issue-referencing entry.
- [ ] `./gradlew :prepareSandbox` passed against the real SDK.
- [ ] `PluginVersion` and every `plugin.xml` `<version>` are still `_PLACEHOLDER_`.
- [ ] No file was written through shell redirection.
- [ ] Nothing was committed, pushed, or published.
