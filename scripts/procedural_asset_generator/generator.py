
import json
import argparse
import os
import math

# This is a hack to get the colors from the RetroPalette_Default.asset file.
# In a real-world scenario, this would be handled by a more robust system.
PALETTE = {
    "PrimaryBase": {"r": 0.105, "g": 0.121, "b": 0.165, "a": 1},
    "PrimaryConcrete": {"r": 0.196, "g": 0.204, "b": 0.227, "a": 1},
    "AccentMagenta": {"r": 0.996, "g": 0.176, "b": 0.584, "a": 1},
    "AccentCyan": {"r": 0, "g": 0.819, "b": 0.996, "a": 1},
    "TertiaryOxide": {"r": 0.655, "g": 0.341, "b": 0.235, "a": 1},
    "TertiaryWarmGrey": {"r": 0.482, "g": 0.446, "b": 0.423, "a": 1},
}

def validate_brief(asset_brief):
    """Validates the asset brief against the schema."""
    if 'assetName' not in asset_brief or not isinstance(asset_brief['assetName'], str):
        raise ValueError("Invalid or missing assetName")
    if 'type' not in asset_brief or asset_brief['type'] not in ['cube', 'bench', 'desk', 'sofa', 'fusebox', 'radio', 'television', 'lamp', 'chair', 'desk_with_computer']:
        raise ValueError("Invalid or missing type")
    if 'dimensions' not in asset_brief or not isinstance(asset_brief['dimensions'], dict):
        raise ValueError("Invalid or missing dimensions")
    if 'material' not in asset_brief or not isinstance(asset_brief['material'], dict):
        raise ValueError("Invalid or missing material")
    if 'baseColor' not in asset_brief['material'] or asset_brief['material']['baseColor'] not in PALETTE:
        raise ValueError("Invalid or missing baseColor")
    return True

def generate(asset_brief):
    """Generates a .obj and .mat file from an asset brief."""
    validate_brief(asset_brief)

    asset_name = asset_brief['assetName']
    output_dir = f'Assets/Art/Procedural/{asset_name}'
    os.makedirs(output_dir, exist_ok=True)

    obj_path = f'{output_dir}/{asset_name}.obj'
    mat_path = f'{output_dir}/{asset_name}.mat'

    vertices = []
    normals = []
    faces = []

    if asset_brief['type'] == 'cube':
        add_beveled_cube(vertices, normals, faces, asset_brief['dimensions'])
    elif asset_brief['type'] == 'bench':
        generate_bench(asset_brief, vertices, normals, faces)
    elif asset_brief['type'] == 'desk':
        generate_desk(asset_brief, vertices, normals, faces)
    elif asset_brief['type'] == 'sofa':
        generate_sofa(asset_brief, vertices, normals, faces)
    elif asset_brief['type'] == 'fusebox':
        add_beveled_cube(vertices, normals, faces, asset_brief['dimensions'])
    elif asset_brief['type'] == 'radio':
        generate_radio(asset_brief, vertices, normals, faces)
    elif asset_brief['type'] == 'television':
        generate_television(asset_brief, vertices, normals, faces)
    elif asset_brief['type'] == 'lamp':
        generate_lamp(asset_brief, vertices, normals, faces)
    elif asset_brief['type'] == 'chair':
        generate_chair(asset_brief, vertices, normals, faces)
    elif asset_brief['type'] == 'desk_with_computer':
        generate_desk_with_computer(asset_brief, vertices, normals, faces)
    else:
        print(f"Unsupported asset type: {asset_brief['type']}")
        return

    with open(obj_path, 'w') as f:
        f.write(f'o {asset_name}\n')

        for v in vertices:
            f.write(f'v {v[0]} {v[1]} {v[2]}\n')
        
        for n in normals:
            f.write(f'vn {n[0]} {n[1]} {n[2]}\n')

        f.write('s off\n')

        for i, face in enumerate(faces):
            f.write(f'f {face[0][0]}//{face[0][1]} {face[1][0]}//{face[1][1]} {face[2][0]}//{face[2][1]} {face[3][0]}//{face[3][1]}\n')

    base_color = PALETTE.get(asset_brief['material']['baseColor'], PALETTE['PrimaryBase'])
    accent_color = PALETTE.get(asset_brief['material'].get('accentColor'), {"r": 0, "g": 0, "b": 0, "a": 1})

    material_yaml = '''%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
---
!u!21 &2100000
Material:
  m_ObjectHideFlags: 0
  m_Name: {asset_name}
  m_Shader: {{fileID: 4800000, guid: 41e11d8035864607b2075d8addde602e, type: 3}}
  m_ShaderKeywords: _
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {{}}
  disabledShaderPasses: []
  m_SavedProperties:
    serializedVersion: 2
    m_TexEnvs:
    - _AccentMask: {{m_Texture: {{fileID: 0}}, m_Scale: {{x: 1, y: 1}}, m_Offset: {{x: 0, y: 0}}}}
    m_Floats:
    - _AccentIntensity: 1
    - _GlitchStrength: 0.1
    - _Metallic: 0
    - _Smoothness: 0.1
    m_Colors:
    - _BaseColor: {{r: {base_r}, g: {base_g}, b: {base_b}, a: {base_a}}}
    - _AccentColor: {{r: {accent_r}, g: {accent_g}, b: {accent_b}, a: {accent_a}}}
'''.format(
        asset_name=asset_name,
        base_r=base_color['r'], base_g=base_color['g'], base_b=base_color['b'], base_a=base_color['a'],
        accent_r=accent_color['r'], accent_g=accent_color['g'], accent_b=accent_color['b'], accent_a=accent_color['a']
    )

    with open(mat_path, 'w') as f:
        f.write(material_yaml)

    print(f'Generated {obj_path} and {mat_path}')

def add_beveled_cube(vertices, normals, faces, dimensions, offset=(0,0,0), bevel_amount=0.05):
    """Generates a beveled cube."""
    # This is a simplified implementation. A full implementation is more complex.
    add_cube(vertices, normals, faces, dimensions, offset)

def add_cylinder(vertices, normals, faces, radius, height, segments, offset=(0,0,0)):
    """Generates a cylinder."""
    v_start_index = len(vertices) + 1
    n_start_index = len(normals) + 1

    # Top and bottom vertices
    top_center = (offset[0], offset[1] + height / 2, offset[2])
    bottom_center = (offset[0], offset[1] - height / 2, offset[2])
    vertices.append(top_center)
    vertices.append(bottom_center)

    # Normals
    normals.append((0, 1, 0)) # Top
    normals.append((0, -1, 0)) # Bottom

    for i in range(segments):
        angle = 2 * math.pi * i / segments
        x = offset[0] + radius * math.cos(angle)
        z = offset[2] + radius * math.sin(angle)
        
        # Side vertices
        vertices.append((x, offset[1] + height / 2, z))
        vertices.append((x, offset[1] - height / 2, z))

        # Side normals
        normals.append((math.cos(angle), 0, math.sin(angle)))

        # Faces
        v_top = v_start_index + 2 + i * 2
        v_bottom = v_start_index + 3 + i * 2
        v_next_top = v_start_index + 2 + ((i + 1) % segments) * 2
        v_next_bottom = v_start_index + 3 + ((i + 1) % segments) * 2

        n_side = n_start_index + 2 + i

        faces.append(((v_bottom, n_side), (v_next_bottom, n_side), (v_next_top, n_side), (v_top, n_side)))
        faces.append(((v_start_index, n_start_index), (v_next_top, n_start_index), (v_top, n_start_index), (v_top, n_start_index))) # Top cap
        faces.append(((v_start_index + 1, n_start_index + 1), (v_bottom, n_start_index + 1), (v_next_bottom, n_start_index + 1), (v_next_bottom, n_start_index + 1))) # Bottom cap

def add_cube(vertices, normals, faces, dimensions, offset=(0, 0, 0)):
    """Generates a cube with correct face winding and normals."""
    x, y, z = dimensions['x'], dimensions['y'], dimensions['z']
    dx, dy, dz = x / 2, y / 2, z / 2
    ox, oy, oz = offset

    v_start_index = len(vertices) + 1
    n_start_index = len(normals) + 1

    # Add vertices
    new_vertices = [
        (ox - dx, oy - dy, oz - dz), (ox + dx, oy - dy, oz - dz), (ox + dx, oy + dy, oz - dz), (ox - dx, oy + dy, oz - dz),
        (ox - dx, oy - dy, oz + dz), (ox + dx, oy - dy, oz + dz), (ox + dx, oy + dy, oz + dz), (ox - dx, oy + dy, oz + dz)
    ]
    vertices.extend(new_vertices)

    # Add normals
    new_normals = [
        (0, 0, -1), (0, 0, 1), (0, -1, 0), (0, 1, 0), (-1, 0, 0), (1, 0, 0)
    ]
    normals.extend(new_normals)

    # Add faces (vertex index // normal index)
    new_faces = [
        # Front (+Z)
        ((v_start_index + 4, n_start_index + 1), (v_start_index + 5, n_start_index + 1), (v_start_index + 6, n_start_index + 1), (v_start_index + 7, n_start_index + 1)),
        # Back (-Z)
        ((v_start_index + 1, n_start_index + 0), (v_start_index + 0, n_start_index + 0), (v_start_index + 3, n_start_index + 0), (v_start_index + 2, n_start_index + 0)),
        # Top (+Y)
        ((v_start_index + 3, n_start_index + 3), (v_start_index + 7, n_start_index + 3), (v_start_index + 6, n_start_index + 3), (v_start_index + 2, n_start_index + 3)),
        # Bottom (-Y)
        ((v_start_index + 1, n_start_index + 2), (v_start_index + 5, n_start_index + 2), (v_start_index + 4, n_start_index + 2), (v_start_index + 0, n_start_index + 2)),
        # Right (+X)
        ((v_start_index + 5, n_start_index + 5), (v_start_index + 1, n_start_index + 5), (v_start_index + 2, n_start_index + 5), (v_start_index + 6, n_start_index + 5)),
        # Left (-X)
        ((v_start_index + 0, n_start_index + 4), (v_start_index + 4, n_start_index + 4), (v_start_index + 7, n_start_index + 4), (v_start_index + 3, n_start_index + 4)),
    ]
    faces.extend(new_faces)

def generate_bench(asset_brief, vertices, normals, faces):
    """Generates a bench."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']

    # Seat
    seat_dims = {'x': x, 'y': y * 0.1, 'z': z}
    add_beveled_cube(vertices, normals, faces, seat_dims, (0, 0, 0))

    # Legs
    leg_height = y * 0.9
    leg_width = x * 0.1
    leg_depth = z * 0.1

    leg_x_offset = x / 2 - leg_width / 2
    leg_z_offset = z / 2 - leg_depth / 2

    add_cylinder(vertices, normals, faces, leg_width/2, leg_height, 16, (-leg_x_offset, -leg_height / 2, -leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_width/2, leg_height, 16, (leg_x_offset, -leg_height / 2, -leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_width/2, leg_height, 16, (-leg_x_offset, -leg_height / 2, leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_width/2, leg_height, 16, (leg_x_offset, -leg_height / 2, leg_z_offset))

def generate_desk(asset_brief, vertices, normals, faces):
    """Generates a desk."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']

    # Top
    top_dims = {'x': x, 'y': y * 0.05, 'z': z}
    add_beveled_cube(vertices, normals, faces, top_dims, (0, y - (y*0.05)/2, 0))

    # Legs
    leg_height = y * 0.95
    leg_radius = x * 0.025

    leg_x_offset = x / 2 - leg_radius
    leg_z_offset = z / 2 - leg_radius

    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (-leg_x_offset, leg_height / 2, -leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (leg_x_offset, leg_height / 2, -leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (-leg_x_offset, leg_height / 2, leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (leg_x_offset, leg_height / 2, leg_z_offset))

def generate_sofa(asset_brief, vertices, normals, faces):
    """Generates a sofa."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']

    # Base
    base_dims = {'x': x, 'y': y * 0.4, 'z': z}
    add_beveled_cube(vertices, normals, faces, base_dims, (0, y * 0.2 - y/2, 0))

    # Back
    back_dims = {'x': x, 'y': y * 0.6, 'z': z * 0.2}
    add_beveled_cube(vertices, normals, faces, back_dims, (0, y * 0.7 - y/2, -z * 0.4))

    # Arms
    arm_dims = {'x': x * 0.1, 'y': y * 0.3, 'z': z * 0.8}
    add_beveled_cube(vertices, normals, faces, arm_dims, (-x * 0.45, y * 0.55 - y/2, z * 0.1))
    add_beveled_cube(vertices, normals, faces, arm_dims, (x * 0.45, y * 0.55 - y/2, z * 0.1))

def generate_radio(asset_brief, vertices, normals, faces):
    """Generates a radio."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']
    add_beveled_cube(vertices, normals, faces, {'x': x, 'y': y * 0.8, 'z': z}, (0, y * 0.1 - y*0.4, 0))
    add_cylinder(vertices, normals, faces, x * 0.05, y * 0.3, 12, (x * 0.4, y * 0.85 - y*0.4, 0)) # Antenna

def generate_television(asset_brief, vertices, normals, faces):
    """Generates a television."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']
    add_beveled_cube(vertices, normals, faces, {'x': x, 'y': y, 'z': z * 0.6}, (0, 0, z * 0.2))
    add_beveled_cube(vertices, normals, faces, {'x': x * 0.5, 'y': y * 0.5, 'z': z * 0.4}, (0, 0, -z * 0.3))

def generate_lamp(asset_brief, vertices, normals, faces):
    """Generates a lamp."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']
    add_cylinder(vertices, normals, faces, x/2, y * 0.1, 16, (0, -y/2 + y*0.05, 0)) # Base
    add_cylinder(vertices, normals, faces, x * 0.1, y * 0.9, 12, (0, 0, 0)) # Stand
    add_cylinder(vertices, normals, faces, x*0.4, y*0.3, 16, (0, y/2 - y*0.15, 0)) # Shade

def generate_chair(asset_brief, vertices, normals, faces):
    """Generates a chair."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']
    # Seat
    add_beveled_cube(vertices, normals, faces, {'x': x, 'y': y * 0.1, 'z': z}, (0, 0, 0))
    # Back
    add_beveled_cube(vertices, normals, faces, {'x': x, 'y': y, 'z': z * 0.1}, (0, y * 0.5, -z * 0.45))
    # Legs
    leg_height = y
    leg_radius = x * 0.05
    leg_x_offset = x / 2 - leg_radius
    leg_z_offset = z / 2 - leg_radius
    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (-leg_x_offset, -leg_height / 2, -leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (leg_x_offset, -leg_height / 2, -leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (-leg_x_offset, -leg_height / 2, leg_z_offset))
    add_cylinder(vertices, normals, faces, leg_radius, leg_height, 16, (leg_x_offset, -leg_height / 2, leg_z_offset))

def generate_desk_with_computer(asset_brief, vertices, normals, faces):
    """Generates a desk with a computer."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']
    # Desk
    generate_desk(asset_brief, vertices, normals, faces)
    # Computer
    add_beveled_cube(vertices, normals, faces, {'x': x * 0.2, 'y': y * 0.4, 'z': z * 0.05}, (0, y + y*0.2, 0)) # Monitor
    add_beveled_cube(vertices, normals, faces, {'x': x * 0.1, 'y': y * 0.5, 'z': z * 0.3}, (-x * 0.4, y/2, 0)) # Tower

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Procedural Asset Generator')
    parser.add_argument('brief', type=str, help='Path to the asset brief JSON file')
    args = parser.parse_args()

    with open(args.brief, 'r') as f:
        asset_brief = json.load(f)

    try:
        generate(asset_brief)
    except ValueError as e:
        print(f"Error: {e}")
