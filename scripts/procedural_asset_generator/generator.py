
import argparse
import json
import math
import os
from typing import Dict, Iterable, List, Optional, Sequence, Tuple


PALETTE = {
    "PrimaryBase": {"r": 0.105, "g": 0.121, "b": 0.165, "a": 1},
    "PrimaryConcrete": {"r": 0.196, "g": 0.204, "b": 0.227, "a": 1},
    "AccentMagenta": {"r": 0.996, "g": 0.176, "b": 0.584, "a": 1},
    "AccentCyan": {"r": 0.0, "g": 0.819, "b": 0.996, "a": 1},
    "TertiaryOxide": {"r": 0.655, "g": 0.341, "b": 0.235, "a": 1},
    "TertiaryWarmGrey": {"r": 0.482, "g": 0.446, "b": 0.423, "a": 1},
}


def generate(asset_brief: Dict) -> None:
    asset_name = asset_brief["assetName"]
    output_dir = os.path.join("Assets", "Art", "Procedural", asset_name)
    os.makedirs(output_dir, exist_ok=True)

    obj_path = os.path.join(output_dir, f"{asset_name}.obj")
    mtllib_path = os.path.join(output_dir, f"{asset_name}.mtl")

    vertices: List[Tuple[float, float, float]] = []
    faces: List[Dict] = []
    materials: Dict[str, Dict] = {}

    parts = asset_brief.get("parts")
    if not parts:
        parts = [
            {
                "partName": asset_brief.get("assetName", "Part"),
                "type": asset_brief["type"],
                "dimensions": asset_brief["dimensions"],
                "offset": asset_brief.get("offset", {"x": 0, "y": 0, "z": 0}),
                "material": asset_brief.get("material", {"baseColor": "PrimaryBase"}),
            }
        ]

    for part in parts:
        part_name = part.get("partName", part.get("type", "Part"))
        mat_name = f"{asset_name}_{part_name}"
        materials[mat_name] = part.get("material", {"baseColor": "PrimaryBase"})

        offset = _ensure_offset(part.get("offset"))
        dims = part.get("dimensions", {"x": 1, "y": 1, "z": 1})

        if part["type"] == "cube":
            add_cube(vertices, faces, dims, offset, mat_name)
        elif part["type"] == "cylinder":
            radius_value = dims.get("radius")
            if radius_value is None:
                diameter = dims.get("x", dims.get("z", 1.0))
                radius_value = float(diameter) / 2.0
            radius = float(radius_value)
            height_value = dims.get("height", dims.get("y", 1.0))
            height = float(height_value)
            add_cylinder(vertices, faces, radius, height, offset, mat_name)
        else:
            raise ValueError(f"Unsupported part type: {part['type']}")

    optimized_vertices, optimized_faces = optimize_vertices(vertices, faces)
    normals = assign_normals(optimized_faces, optimized_vertices)

    _write_mtl(mtllib_path, materials)
    _write_obj(obj_path, asset_name, mtllib_path, optimized_vertices, normals, optimized_faces)

    print(f"Generated optimized {obj_path} and {mtllib_path}")


def _ensure_offset(offset: Optional[Dict[str, float]]) -> Dict[str, float]:
    if offset is None:
        return {"x": 0.0, "y": 0.0, "z": 0.0}
    return {
        "x": float(offset.get("x", 0.0)),
        "y": float(offset.get("y", 0.0)),
        "z": float(offset.get("z", 0.0)),
    }


def optimize_vertices(
    vertices: Sequence[Tuple[float, float, float]], faces: Iterable[Dict]
) -> Tuple[List[Tuple[float, float, float]], List[Dict]]:
    vertex_map: Dict[Tuple[float, float, float], int] = {}
    optimized_vertices: List[Tuple[float, float, float]] = []
    remap_indices: Dict[int, int] = {}

    for index, vertex in enumerate(vertices):
        key = tuple(round(component, 5) for component in vertex)
        if key not in vertex_map:
            vertex_map[key] = len(optimized_vertices)
            optimized_vertices.append(vertex)
        remap_indices[index] = vertex_map[key]

    optimized_faces: List[Dict] = []
    for face in faces:
        optimized_faces.append(
            {
                "vertices": [remap_indices[idx] for idx in face["vertices"]],
                "normal": face["normal"],
                "material": face["material"],
            }
        )

    return optimized_vertices, optimized_faces


def assign_normals(faces: List[Dict], vertices: Sequence[Tuple[float, float, float]]) -> List[Tuple[float, float, float]]:
    normal_map: Dict[Tuple[float, float, float], int] = {}
    normals: List[Tuple[float, float, float]] = []

    for face in faces:
        normal = compute_face_normal(vertices, face["vertices"])
        face["normal"] = normal
        key = tuple(round(component, 6) for component in normal)
        if key not in normal_map:
            normal_map[key] = len(normals)
            normals.append(normal)
        face["normal_index"] = normal_map[key]

    return normals


def compute_face_normal(
    vertices: Sequence[Tuple[float, float, float]], vertex_indices: Sequence[int]
) -> Tuple[float, float, float]:
    if len(vertex_indices) < 3:
        raise ValueError("Faces must have at least three vertices")

    base = vertices[vertex_indices[0]]
    for i in range(1, len(vertex_indices) - 1):
        edge1 = _vector_sub(vertices[vertex_indices[i]], base)
        edge2 = _vector_sub(vertices[vertex_indices[i + 1]], base)
        cross = _cross(edge1, edge2)
        length = _vector_length(cross)
        if length > 1e-8:
            return tuple(component / length for component in cross)

    # Degenerate face, fallback to up-vector to avoid importer errors.
    return (0.0, 1.0, 0.0)


def add_cube(
    vertices: List[Tuple[float, float, float]],
    faces: List[Dict],
    dimensions: Dict[str, float],
    offset: Dict[str, float],
    material: str,
) -> None:
    x = float(dimensions.get("x", 1.0))
    y = float(dimensions.get("y", 1.0))
    z = float(dimensions.get("z", 1.0))
    dx, dy, dz = x / 2.0, y / 2.0, z / 2.0
    ox, oy, oz = offset["x"], offset["y"], offset["z"]

    base_index = len(vertices)
    local_vertices = [
        (ox - dx, oy - dy, oz - dz),
        (ox + dx, oy - dy, oz - dz),
        (ox + dx, oy + dy, oz - dz),
        (ox - dx, oy + dy, oz - dz),
        (ox - dx, oy - dy, oz + dz),
        (ox + dx, oy - dy, oz + dz),
        (ox + dx, oy + dy, oz + dz),
        (ox - dx, oy + dy, oz + dz),
    ]

    vertices.extend(local_vertices)

    faces_definitions = [
        [0, 3, 2, 1],  # Back (-Z)
        [4, 5, 6, 7],  # Front (+Z)
        [0, 4, 7, 3],  # Left (-X)
        [1, 2, 6, 5],  # Right (+X)
        [0, 1, 5, 4],  # Bottom (-Y)
        [3, 7, 6, 2],  # Top (+Y)
    ]

    for face_indices in faces_definitions:
        global_indices = [base_index + idx for idx in face_indices]
        faces.append({"vertices": global_indices, "material": material, "normal": (0.0, 1.0, 0.0)})


def add_cylinder(
    vertices: List[Tuple[float, float, float]],
    faces: List[Dict],
    radius: float,
    height: float,
    offset: Dict[str, float],
    material: str,
    segments: int = 16,
) -> None:
    if segments < 3:
        raise ValueError("Cylinders require at least three segments")

    ox, oy, oz = offset["x"], offset["y"], offset["z"]
    half_height = height / 2.0

    base_index = len(vertices)
    top_center_index = base_index
    bottom_center_index = base_index + 1
    vertices.append((ox, oy + half_height, oz))
    vertices.append((ox, oy - half_height, oz))

    ring_indices: List[Tuple[int, int]] = []
    for i in range(segments):
        angle = 2.0 * math.pi * i / segments
        x = ox + radius * math.cos(angle)
        z = oz + radius * math.sin(angle)
        top_idx = len(vertices)
        bottom_idx = top_idx + 1
        vertices.append((x, oy + half_height, z))
        vertices.append((x, oy - half_height, z))
        ring_indices.append((top_idx, bottom_idx))

    for i in range(segments):
        top_curr, bottom_curr = ring_indices[i]
        top_next, bottom_next = ring_indices[(i + 1) % segments]

        # Side quad
        faces.append(
            {
                "vertices": [bottom_curr, bottom_next, top_next, top_curr],
                "material": material,
                "normal": (0.0, 1.0, 0.0),
            }
        )

        # Top triangle
        faces.append(
            {
                "vertices": [top_center_index, top_curr, top_next],
                "material": material,
                "normal": (0.0, 1.0, 0.0),
            }
        )

        # Bottom triangle
        faces.append(
            {
                "vertices": [bottom_center_index, bottom_next, bottom_curr],
                "material": material,
                "normal": (0.0, 1.0, 0.0),
            }
        )


def _write_mtl(mtllib_path: str, materials: Dict[str, Dict]) -> None:
    with open(mtllib_path, "w", encoding="utf-8") as handle:
        for mat_name, mat_info in materials.items():
            handle.write(f"newmtl {mat_name}\n")
            base_color = PALETTE.get(mat_info.get("baseColor"), PALETTE["PrimaryBase"])
            handle.write(
                f"Kd {base_color['r']} {base_color['g']} {base_color['b']}\n"
            )
            if mat_info.get("isEmissive", False):
                handle.write(
                    f"Ka {base_color['r']} {base_color['g']} {base_color['b']}\n"
                )
            else:
                handle.write("Ka 0 0 0\n")
            handle.write("Ks 0 0 0\n")


def _write_obj(
    obj_path: str,
    asset_name: str,
    mtllib_path: str,
    vertices: Sequence[Tuple[float, float, float]],
    normals: Sequence[Tuple[float, float, float]],
    faces: Sequence[Dict],
) -> None:
    with open(obj_path, "w", encoding="utf-8") as handle:
        handle.write(f"mtllib {os.path.basename(mtllib_path)}\n")
        handle.write(f"o {asset_name}\n")

        for vertex in vertices:
            handle.write(f"v {vertex[0]:.6f} {vertex[1]:.6f} {vertex[2]:.6f}\n")

        for normal in normals:
            handle.write(f"vn {normal[0]:.6f} {normal[1]:.6f} {normal[2]:.6f}\n")

        handle.write("s off\n")

        current_material = None
        for face in faces:
            if face["material"] != current_material:
                handle.write(f"usemtl {face['material']}\n")
                current_material = face["material"]

            normal_index = face["normal_index"] + 1
            vertex_refs = [
                f"{vertex_index + 1}//{normal_index}" for vertex_index in face["vertices"]
            ]
            handle.write(f"f {' '.join(vertex_refs)}\n")


def _vector_sub(a: Tuple[float, float, float], b: Tuple[float, float, float]) -> Tuple[float, float, float]:
    return (a[0] - b[0], a[1] - b[1], a[2] - b[2])


def _cross(
    a: Tuple[float, float, float], b: Tuple[float, float, float]
) -> Tuple[float, float, float]:
    return (
        a[1] * b[2] - a[2] * b[1],
        a[2] * b[0] - a[0] * b[2],
        a[0] * b[1] - a[1] * b[0],
    )


def _vector_length(vector: Tuple[float, float, float]) -> float:
    return math.sqrt(vector[0] ** 2 + vector[1] ** 2 + vector[2] ** 2)

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Procedural Asset Generator')
    parser.add_argument('brief', type=str, help='Path to the asset brief JSON file')
    args = parser.parse_args()

    with open(args.brief, 'r') as f:
        asset_brief = json.load(f)

    generate(asset_brief)
