# LG Display Configuration Authoring

> **Configuration boundary.** Examples are structural templates, not deployable credentials. Replace only documented placeholders; never commit credentials, activation codes, or site-specific addresses. The source and its configuration-deserialization tests are authoritative when this guide conflicts with any older document.

## What this configuration controls

LG display configuration binds an Essentials display device to its control transport and describes volume bounds, polling, warm/cool timing, display identity, Wake-on-LAN behavior, and friendly input metadata.

| Authoring fact | Verified source contract |
|---|---|
| Configured type alias | `lgDisplay`, `lgPlugin`, `lg`, or `lgDisplayIr` |
| Primary source authority | [`../src/LgDisplayPropertiesConfig.cs`](../src/LgDisplayPropertiesConfig.cs) |
| Use this guide when | adding a display, changing warm-up/cool-down behavior, constraining volume, selecting network versus IR control, or exposing/hiding inputs. |

## Why the relationships matter

The display key is reused by participant `video.displayKey`, destination-list `sinkKey`, and room macros. The control transport must match the physical control design. Input labels may change UI visibility but do not replace routing port identity.

## Source-declared configuration facts

`Id`, volume limits, poll/cooling/warming timing, `udpSocketKey`, `macAddress`, `SmallDisplay`, `OverrideWol`, and `FriendlyNames` are source-declared. Friendly-name entries declare `InputKey`, `Name`, and `HideInput`.

## Safe structural example

```json
{
  "key": "main-display",
  "name": "Main Display",
  "type": "lgDisplay",
  "properties": {
    "id": "<display-id>",
    "control": {
      "method": "tcpIp",
      "tcpSshProperties": { "address": "<display-address>", "port": 0, "autoReconnect": true }
    },
    "volumeUpperLimit": 100,
    "volumeLowerLimit": 0,
    "pollIntervalMs": 5000,
    "coolingTimeMs": 0,
    "warmingTimeMs": 0,
    "smallDisplay": false,
    "overrideWol": false,
    "friendlyNames": [ { "inputKey": "<input-id>", "name": "Local HDMI", "hideInput": false } ]
  }
}
```

## When and how to author it

Start with the closest repository example or a known-good template. Add the device with a stable unique `key`, use the exact factory alias, and populate only the properties needed by the selected capabilities. Build every key relationship before deployment; do not rely on permissive JSON deserialization or case-insensitive type aliases to repair a wrong design.

## Validation

Confirm transport reachability, the correct `id`, power feedback, and one input/volume command. Validate warm/cool timing only in a controlled commissioning window, because it changes power-state sequencing.

## Change and safety rules

Never document a display address, MAC address, password, or real socket key. Use the hardware’s actual input identifiers, not only human labels.

## Sources

- [`../src/LgDisplayPropertiesConfig.cs`](../src/LgDisplayPropertiesConfig.cs)
- `tests/ConfigDeserializationTests.cs` and `tests/FactoryDiscoveryTests.cs`, where present
