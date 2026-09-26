# LgDisplayIrController

> **Public-repository boundary.** This reference intentionally documents generic source structure only. Do not add customer-specific context, internal architecture rationale, deployment topology, credentials, or private cross-repository contracts here.


| Field | Source-grounded value |
|---|---|
| Repository | `epi-lg-display` |
| Source file | [`src/LgDisplayIr/LgDisplayIrController.cs`](../../../src/LgDisplayIr/LgDisplayIrController.cs) |
| Language | C# |
| Declaration | `class LgDisplayIrController` with declared base/contract list `DisplayBase, IBasicVolumeControls, IBridgeAdvanced, IHasInputs<string>` |
| Accessibility | `public` |
| Namespace/module | `PepperDash.Essentials.Plugins.Lg.Display` |

## What

`LgDisplayIrController` is a source-level implementation type with responsibilities defined by its members and inheritance. This description is grounded in its source declaration and declared inheritance rather than inferred product behavior.

## Why

The type exists to provide a named boundary in the codebase. Its inheritance, implemented contracts, and public members define what surrounding code may rely on. Preserve that boundary unless a deliberate repository-wide compatibility change is intended.

## How it works

Preserve the declared inheritance/contract relationship: `DisplayBase, IBasicVolumeControls, IBridgeAdvanced, IHasInputs<string>`. Public methods declared in this source file include: `LinkToApi`, `SendIrCommand`, `PowerOn`, `PowerOnPress`, `PowerOff`, `PowerOffPress`, `PowerToggle`, `PowerTogglePress`, `ListRoutingInputPorts`, `InputHdmi1`, `InputHdmi2`, `InputHdmi3`. Use repository search to identify callers, implementers, serializers, tests, and configuration references before changing a public name or shape.

## When to modify it

Edit when the responsibility implied by the declaration or its public members changes. Inspect callers, tests, and configuration before changing signatures.

## AI-agent change protocol

Before proposing a change, read this declaration, its full source file, all repository references to `LgDisplayIrController`, and its test coverage. Do not invent configuration keys, payload fields, interface members, or lifecycle ordering. Report the affected source files, tests, and consumer boundaries with any proposed change.

## Source authority

The source file linked above is authoritative. This generated reference is an index and decision aid; update it after a declaration, inheritance list, or public member contract changes.
