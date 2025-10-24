# Procedural Asset Generator Schema

This document defines the input schema for the procedural asset generator. The schema is a JSON object that defines an asset as a collection of parts.

## Root Object

| Property | Type | Description |
|---|---|---|
| `assetName` | string | The name of the asset. This will be used for the generated file names. |
| `parts` | array of Part objects | An array defining each geometric component of the asset. |

## Part Object

| Property | Type | Description |
|---|---|---|
| `partName` | string | The name of the individual part. |
| `type` | string | The geometric primitive to use. Supported values: `cube`, `cylinder`. |
| `dimensions` | object | The dimensions of the part (x, y, z). |
| `offset` | object | The position of the part relative to the asset's origin (x, y, z). |
| `material` | object | The material definition for this part. |

## Material Object

| Property | Type | Description |
|---|---|---|
| `baseColor` | string | The palette swatch for the base color. |
| `accentColor` | string | (Optional) The palette swatch for the accent color. |
| `isEmissive` | boolean | (Optional) If true, this material will be configured to glow. Defaults to false. |

## Example (Radio)

```json
{
  "assetName": "Radio",
  "parts": [
    {
      "partName": "Body",
      "type": "cube",
      "dimensions": {"x": 0.4, "y": 0.2, "z": 0.15},
      "offset": {"x": 0, "y": 0, "z": 0},
      "material": {"baseColor": "PrimaryConcrete"}
    },
    {
      "partName": "Display",
      "type": "cube",
      "dimensions": {"x": 0.15, "y": 0.08, "z": 0.01},
      "offset": {"x": -0.08, "y": 0.02, "z": 0.07},
      "material": {"baseColor": "AccentCyan", "isEmissive": true}
    },
    {
      "partName": "VolumeKnob",
      "type": "cylinder",
      "dimensions": {"x": 0.04, "y": 0.03, "z": 0.04},
      "offset": {"x": 0.1, "y": 0.05, "z": 0.07},
      "material": {"baseColor": "TertiaryWarmGrey"}
    },
    {
      "partName": "PowerButton",
      "type": "cylinder",
      "dimensions": {"x": 0.03, "y": 0.02, "z": 0.03},
      "offset": {"x": 0.1, "y": -0.05, "z": 0.07},
      "material": {"baseColor": "AccentMagenta", "isEmissive": true}
    }
  ]
}
```