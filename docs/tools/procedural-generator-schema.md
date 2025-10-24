# Procedural Asset Generator Schema

This document defines the input schema for the procedural asset generator. The schema is a JSON object with the following properties:

## Root Object

| Property | Type | Description |
|---|---|---|
| `assetName` | string | The name of the asset. This will be used for the generated file names. |
| `type` | string | The type of the asset. Supported values are `cube` and `bench`. |
| `dimensions` | object | The dimensions of the asset. |
| `material` | object | The material properties of the asset. |
| `functionalTags` | array of strings | Functional tags for the asset (e.g., `cover`, `interactable`). |

## Dimensions Object

| Property | Type | Description |
|---|---|---|
| `x` | number | The size of the asset along the x-axis. |
| `y` | number | The size of the asset along the y-axis. |
| `z` | number | The size of the asset along the z-axis. |

## Material Object

| Property | Type | Description |
|---|---|---|
| `baseColor` | string | The palette swatch for the base color. |
| `accentColor` | string | The palette swatch for the accent color. |

## Example (Cube)

```json
{
  "assetName": "MyCube",
  "type": "cube",
  "dimensions": {
    "x": 1,
    "y": 1,
    "z": 1
  },
  "material": {
    "baseColor": "PrimaryConcrete",
    "accentColor": "AccentMagenta"
  },
  "functionalTags": []
}
```

## Example (Bench)

```json
{
  "assetName": "MyBench",
  "type": "bench",
  "dimensions": {
    "x": 2,
    "y": 0.5,
    "z": 0.5
  },
  "material": {
    "baseColor": "TertiaryOxide",
    "accentColor": ""
  },
  "functionalTags": ["cover"]
}
```
