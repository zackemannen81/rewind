
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

def generate(asset_brief):
    asset_name = asset_brief['assetName']
    output_dir = f'Assets/Art/Procedural/{asset_name}'
    os.makedirs(output_dir, exist_ok=True)

    obj_path = f'{output_dir}/{asset_name}.obj'
    mtllib_path = f'{output_dir}/{asset_name}.mtl' # OBJ references a .mtl file

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

    # Write MTL file
    with open(mtllib_path, 'w') as f:
        for mat_name, mat_info in materials.items():
            f.write(f'newmtl {mat_name}\n')
            base_color = PALETTE.get(mat_info['baseColor'], PALETTE['PrimaryBase'])
            f.write(f'Kd {base_color["r"]} {base_color["g"]} {base_color["b"]}\n')
            if mat_info.get('isEmissive', False):
                f.write(f'Ka {base_color["r"]} {base_color["g"]} {base_color["b"]}\n') # Emissive color
            else:
                f.write('Ka 0 0 0\n')

    # Write OBJ file
    with open(obj_path, 'w') as f:
        f.write(f'mtllib {os.path.basename(mtllib_path)}\n')
        f.write(f'o {asset_name}\n')

        for v in vertices:
            f.write(f'v {v[0]} {v[1]} {v[2]}\n')
        for n in normals:
            f.write(f'vn {n[0]} {n[1]} {n[2]}\n')

        f.write('s off\n')
        
        current_material = ""
        for i, face in enumerate(faces):
            mat_name = face_material_indices[i]
            if mat_name != current_material:
                f.write(f'usemtl {mat_name}\n')
                current_material = mat_name
            f.write(f'f {face[0][0]}//{face[0][1]} {face[1][0]}//{face[1][1]} {face[2][0]}//{face[2][1]} {face[3][0]}//{face[3][1]}\n')

    print(f'Generated {obj_path} and {mtllib_path}')

def add_cube(vertices, normals, faces, dimensions, offset, v_offset, n_offset):
    x, y, z = dimensions['x'], dimensions['y'], dimensions['z']
    dx, dy, dz = x / 2, y / 2, z / 2
    ox, oy, oz = offset['x'], offset['y'], offset['z']

    v_start_index = v_offset + 1
    n_start_index = n_offset + 1

    new_vertices = [
        (ox - dx, oy - dy, oz - dz), (ox + dx, oy - dy, oz - dz), (ox + dx, oy + dy, oz - dz), (ox - dx, oy + dy, oz - dz),
        (ox - dx, oy - dy, oz + dz), (ox + dx, oy - dy, oz + dz), (ox + dx, oy + dy, oz + dz), (ox - dx, oy + dy, oz + dz)
    ]
    vertices.extend(new_vertices)

    new_normals = [
        (0, 0, -1), (0, 0, 1), (0, -1, 0), (0, 1, 0), (-1, 0, 0), (1, 0, 0)
    ]
    normals.extend(new_normals)

    new_faces = [
        ((v_start_index + 4, n_start_index + 1), (v_start_index + 5, n_start_index + 1), (v_start_index + 6, n_start_index + 1), (v_start_index + 7, n_start_index + 1)),
        ((v_start_index + 1, n_start_index + 0), (v_start_index + 0, n_start_index + 0), (v_start_index + 3, n_start_index + 0), (v_start_index + 2, n_start_index + 0)),
        ((v_start_index + 3, n_start_index + 3), (v_start_index + 7, n_start_index + 3), (v_start_index + 6, n_start_index + 3), (v_start_index + 2, n_start_index + 3)),
        ((v_start_index + 1, n_start_index + 2), (v_start_index + 5, n_start_index + 2), (v_start_index + 4, n_start_index + 2), (v_start_index + 0, n_start_index + 2)),
        ((v_start_index + 5, n_start_index + 5), (v_start_index + 1, n_start_index + 5), (v_start_index + 2, n_start_index + 5), (v_start_index + 6, n_start_index + 5)),
        ((v_start_index + 0, n_start_index + 4), (v_start_index + 4, n_start_index + 4), (v_start_index + 7, n_start_index + 4), (v_start_index + 3, n_start_index + 4)),
    ]
    faces.extend(new_faces)

def add_cylinder(vertices, normals, faces, radius, height, segments, offset, v_offset, n_offset):
    v_start_index = v_offset + 1
    n_start_index = n_offset + 1
    ox, oy, oz = offset['x'], offset['y'], offset['z']

    top_center = (ox, oy + height / 2, oz)
    bottom_center = (ox, oy - height / 2, oz)
    vertices.append(top_center)
    vertices.append(bottom_center)

    normals.append((0, 1, 0))
    normals.append((0, -1, 0))

    for i in range(segments):
        angle = 2 * math.pi * i / segments
        x = ox + radius * math.cos(angle)
        z = oz + radius * math.sin(angle)
        
        vertices.append((x, oy + height / 2, z))
        vertices.append((x, oy - height / 2, z))

        normals.append((math.cos(angle), 0, math.sin(angle)))

        v_top = v_start_index + 2 + i * 2
        v_bottom = v_start_index + 3 + i * 2
        v_next_top = v_start_index + 2 + ((i + 1) % segments) * 2
        v_next_bottom = v_start_index + 3 + ((i + 1) % segments) * 2

        n_side = n_start_index + 2 + i

        faces.append(((v_bottom, n_side), (v_next_bottom, n_side), (v_next_top, n_side), (v_top, n_side)))
        faces.append(((v_start_index, n_start_index), (v_next_top, n_start_index), (v_top, n_start_index), (v_top, n_start_index)))
        faces.append(((v_start_index + 1, n_start_index + 1), (v_bottom, n_start_index + 1), (v_next_bottom, n_start_index + 1), (v_next_bottom, n_start_index + 1)))

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Procedural Asset Generator')
    parser.add_argument('brief', type=str, help='Path to the asset brief JSON file')
    args = parser.parse_args()

    with open(args.brief, 'r') as f:
        asset_brief = json.load(f)

    generate(asset_brief)
