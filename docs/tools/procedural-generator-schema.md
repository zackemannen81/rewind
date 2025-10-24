# Procedural Asset Generator Schema

This document defines the input schema for the procedural asset generator. The schema is a JSON object with the following properties:

## Root Object

| Property | Type | Description |
|---|---|---|
| `assetName` | string | The name of the asset. This will be used for the generated file names. |
| `type` | string | The type of the asset. Currently, only `cube` is supported. |
| `dimensions` | object | The dimensions of the asset. |
| `material` | object | The material properties of the asset. |

## Dimensions Object

| Property | Type | Description |
|---|---|---|
| `x` | number | The size of the asset along the x-axis. |
| `y` | number | The size of the asset along the y-axis. |
| `z` | number | The size of the asset along the z-axis. |

## Material Object

| Property | Type | Description |
|---|---|---|
| `paletteSlot` | string | The palette slot to use for the asset. Valid values are `primary`, `secondary`, and `tertiary`. |

## Example

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
    "paletteSlot": "primary"
  }
}
```
