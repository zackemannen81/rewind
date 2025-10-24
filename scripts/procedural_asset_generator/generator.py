import json
import argparse
import os
import math

PALETTE = {
    "PrimaryBase": {"r": 0.105, "g": 0.121, "b": 0.165, "a": 1},
    "PrimaryConcrete": {"r": 0.196, "g": 0.204, "b": 0.227, "a": 1},
    "AccentMagenta": {"r": 0.996, "g": 0.176, "b": 0.584, "a": 1},
    "AccentCyan": {"r": 0, "g": 0.819, "b": 0.996, "a": 1},
    "TertiaryOxide": {"r": 0.655, "g": 0.341, "b": 0.235, "a": 1},
    "TertiaryWarmGrey": {"r": 0.482, "g": 0.446, "b": 0.423, "a": 1},
}

def optimize_mesh(vertices, faces, face_material_indices):
    """Merges duplicate vertices to optimize the mesh."""
    vertex_map = {}
    optimized_vertices = []
    remap_indices = [-1] * len(vertices)

    for i, v in enumerate(vertices):
        v_tuple = tuple(round(c, 4) for c in v)
        if v_tuple not in vertex_map:
            new_index = len(optimized_vertices)
            vertex_map[v_tuple] = new_index
            optimized_vertices.append(v)
            remap_indices[i] = new_index
        else:
            remap_indices[i] = vertex_map[v_tuple]

    optimized_faces = []
    for face in faces:
        new_face = []
        for v_index, n_index in face:
            new_v_index = remap_indices[v_index - 1] + 1
            new_face.append((new_v_index, n_index))
        optimized_faces.append(tuple(new_face))
    
    return optimized_vertices, optimized_faces

def generate(asset_brief):
    asset_name = asset_brief['assetName']
    output_dir = f'Assets/Art/Procedural/{asset_name}'
    os.makedirs(output_dir, exist_ok=True)

    obj_path = f'{output_dir}/{asset_name}.obj'
    mtllib_path = f'{output_dir}/{asset_name}.mtl'

    vertices = []
    normals = []
    faces = []
    materials = {}
    face_material_indices = []

    for i, part in enumerate(asset_brief['parts']):
        part_name = part['partName']
        mat_info = part['material']
        mat_name = f"{asset_name}_{part_name}"
        materials[mat_name] = mat_info

        v_offset = len(vertices)
        n_offset = len(normals)

        if part['type'] == 'cube':
            add_cube(vertices, normals, faces, part['dimensions'], part['offset'], v_offset, n_offset)
        elif part['type'] == 'cylinder':
            dims = part['dimensions']
            add_cylinder(vertices, normals, faces, dims['x']/2, dims['y'], 16, part['offset'], v_offset, n_offset)
        
        num_new_faces = len(faces) - len(face_material_indices)
        face_material_indices.extend([mat_name] * num_new_faces)

    optimized_vertices, optimized_faces = optimize_mesh(vertices, faces, face_material_indices)

    # Write MTL file
    with open(mtllib_path, 'w') as f:
        for mat_name, mat_info in materials.items():
            f.write(f'newmtl {mat_name}\n')
            base_color = PALETTE.get(mat_info['baseColor'], PALETTE['PrimaryBase'])
            f.write(f'Kd {base_color["r"]} {base_color["g"]} {base_color["b"]}\n') # Diffuse color
            if mat_info.get('isEmissive', False):
                f.write(f'Ka {base_color["r"]} {base_color["g"]} {base_color["b"]}\n') # Ambient color as emissive
            else:
                f.write('Ka 0 0 0\n') # No ambient for non-emissive
            f.write('Ks 0 0 0\n') # No specular

    # Write OBJ file
    with open(obj_path, 'w') as f:
        f.write(f'mtllib {os.path.basename(mtllib_path)}\n')
        f.write(f'o {asset_name}\n')

        for v in optimized_vertices:
            f.write(f'v {v[0]} {v[1]} {v[2]}\n')
        for n in normals:
            f.write(f'vn {n[0]} {n[1]} {n[2]}\n')

        f.write('s off\n')
        
        current_material = ""
        for i, face in enumerate(optimized_faces):
            mat_name = face_material_indices[i]
            if mat_name != current_material:
                f.write(f'usemtl {mat_name}\n')
                current_material = mat_name
            # Note: This generator only produces quads.
            f.write(f'f {face[0][0]}//{face[0][1]} {face[1][0]}//{face[1][1]} {face[2][0]}//{face[2][1]} {face[3][0]}//{face[3][1]}\n')

    print(f'Generated optimized {obj_path} and {mtllib_path}')


def add_cube(vertices, normals, faces, dimensions, offset, v_offset, n_offset):
    x, y, z = dimensions['x'], dimensions['y'], dimensions['z']
    dx, dy, dz = x / 2, y / 2, z / 2
    ox, oy, oz = offset['x'], offset['y'], offset['z']

    v_start_index = v_offset + 1
    n_start_index = n_offset + 1

    vertices.extend([
        (ox - dx, oy - dy, oz - dz), (ox + dx, oy - dy, oz - dz), (ox + dx, oy + dy, oz - dz), (ox - dx, oy + dy, oz - dz),
        (ox - dx, oy - dy, oz + dz), (ox + dx, oy - dy, oz + dz), (ox + dx, oy + dy, oz + dz), (ox - dx, oy + dy, oz + dz)
    ])
    normals.extend([
        (0, 0, -1), (0, 0, 1), (0, -1, 0), (0, 1, 0), (-1, 0, 0), (1, 0, 0)
    ])

    faces.extend([
        ((v_start_index + 4, n_start_index + 1), (v_start_index + 5, n_start_index + 1), (v_start_index + 6, n_start_index + 1), (v_start_index + 7, n_start_index + 1)),
        ((v_start_index + 1, n_start_index + 0), (v_start_index + 0, n_start_index + 0), (v_start_index + 3, n_start_index + 0), (v_start_index + 2, n_start_index + 0)),
        ((v_start_index + 3, n_start_index + 3), (v_start_index + 7, n_start_index + 3), (v_start_index + 6, n_start_index + 3), (v_start_index + 2, n_start_index + 3)),
        ((v_start_index + 1, n_start_index + 2), (v_start_index + 5, n_start_index + 2), (v_start_index + 4, n_start_index + 2), (v_start_index + 0, n_start_index + 2)),
        ((v_start_index + 5, n_start_index + 5), (v_start_index + 1, n_start_index + 5), (v_start_index + 2, n_start_index + 5), (v_start_index + 6, n_start_index + 5)),
        ((v_start_index + 0, n_start_index + 4), (v_start_index + 4, n_start_index + 4), (v_start_index + 7, n_start_index + 4), (v_start_index + 3, n_start_index + 4)),
    ])

def add_cylinder(vertices, normals, faces, radius, height, segments, offset, v_offset, n_offset):
    v_start_index = v_offset + 1
    n_start_index = n_offset + 1
    ox, oy, oz = offset['x'], offset['y'], offset['z']

    # Normals
    normals.append((0, 1, 0))  # Top cap normal
    normals.append((0, -1, 0)) # Bottom cap normal
    for i in range(segments):
        angle = 2 * math.pi * i / segments
        normals.append((math.cos(angle), 0, math.sin(angle))) # Side normals

    # Vertices
    top_center_idx = v_start_index
    vertices.append((ox, oy + height / 2, oz))
    bottom_center_idx = v_start_index + 1
    vertices.append((ox, oy - height / 2, oz))

    for i in range(segments):
        angle = 2 * math.pi * i / segments
        x = ox + radius * math.cos(angle)
        z = oz + radius * math.sin(angle)
        vertices.append((x, oy + height / 2, z)) # Top edge
        vertices.append((x, oy - height / 2, z)) # Bottom edge

    # Faces
    for i in range(segments):
        v_top_curr = v_start_index + 2 + i * 2
        v_bottom_curr = v_start_index + 3 + i * 2
        v_top_next = v_start_index + 2 + ((i + 1) % segments) * 2
        v_bottom_next = v_start_index + 3 + ((i + 1) % segments) * 2

        n_top = n_start_index
        n_bottom = n_start_index + 1
        n_side = n_start_index + 2 + i

        # Side Face (CCW from outside)
        faces.append(((v_bottom_curr, n_side), (v_bottom_next, n_side), (v_top_next, n_side), (v_top_curr, n_side)))
        # Top Cap Face (CCW from outside, i.e., from top)
        faces.append(((top_center_idx, n_top), (v_top_curr, n_top), (v_top_next, n_top), (v_top_next, n_top))) # Degenerate quad
        # Bottom Cap Face (CCW from outside, i.e., from bottom)
        faces.append(((bottom_center_idx, n_bottom), (v_bottom_next, n_bottom), (v_bottom_curr, n_bottom), (v_bottom_curr, n_bottom))) # Degenerate quad

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Procedural Asset Generator')
    parser.add_argument('brief', type=str, help='Path to the asset brief JSON file')
    args = parser.parse_args()

    with open(args.brief, 'r') as f:
        asset_brief = json.load(f)

    generate(asset_brief)