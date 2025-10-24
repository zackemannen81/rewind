import json
import argparse
import os

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

def generate(asset_brief):
    """Generates a .obj and .mat file from an asset brief."""

    asset_name = asset_brief['assetName']
    output_dir = f'Assets/Art/Procedural/{asset_name}'
    os.makedirs(output_dir, exist_ok=True)

    obj_path = f'{output_dir}/{asset_name}.obj'
    mat_path = f'{output_dir}/{asset_name}.mat'

    vertices = []
    faces = []

    if asset_brief['type'] == 'cube':
        add_cube(vertices, faces, asset_brief['dimensions'])
    elif asset_brief['type'] == 'bench':
        generate_bench(asset_brief, vertices, faces)
    else:
        print(f"Unsupported asset type: {asset_brief['type']}")
        return

    with open(obj_path, 'w') as f:
        f.write(f'o {asset_name}\n')

        for v in vertices:
            f.write(f'v {v[0]} {v[1]} {v[2]}\n')

        f.write('s off\n')

        for i, face in enumerate(faces):
            f.write(f'f {face[0] + 1} {face[1] + 1} {face[2] + 1} {face[3] + 1}\n')

    base_color = PALETTE.get(asset_brief['material']['baseColor'], PALETTE['PrimaryBase'])
    accent_color = PALETTE.get(asset_brief['material'].get('accentColor'), {"r": 0, "g": 0, "b": 0, "a": 1})

    material_yaml = '''%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
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

def add_cube(vertices, faces, dimensions, offset=(0, 0, 0)):
    """Generates a cube."""
    x, y, z = dimensions['x'], dimensions['y'], dimensions['z']
    dx, dy, dz = x / 2, y / 2, z / 2
    ox, oy, oz = offset

    start_index = len(vertices)

    new_vertices = [
        (ox - dx, oy - dy, oz - dz),
        (ox + dx, oy - dy, oz - dz),
        (ox + dx, oy + dy, oz - dz),
        (ox - dx, oy + dy, oz - dz),
        (ox - dx, oy - dy, oz + dz),
        (ox + dx, oy - dy, oz + dz),
        (ox + dx, oy + dy, oz + dz),
        (ox - dx, oy + dy, oz + dz),
    ]
    vertices.extend(new_vertices)

    new_faces = [
        (start_index + 0, start_index + 1, start_index + 2, start_index + 3),
        (start_index + 4, start_index + 7, start_index + 6, start_index + 5),
        (start_index + 0, start_index + 4, start_index + 5, start_index + 1),
        (start_index + 1, start_index + 5, start_index + 6, start_index + 2),
        (start_index + 2, start_index + 6, start_index + 7, start_index + 3),
        (start_index + 3, start_index + 7, start_index + 4, start_index + 0),
    ]
    faces.extend(new_faces)


def generate_bench(asset_brief, vertices, faces):
    """Generates a bench."""
    dims = asset_brief['dimensions']
    x, y, z = dims['x'], dims['y'], dims['z']

    # Seat
    seat_dims = {'x': x, 'y': y * 0.1, 'z': z}
    add_cube(vertices, faces, seat_dims, (0, y * 0.45, 0))

    # Legs
    leg_height = y * 0.9
    leg_width = x * 0.1
    leg_depth = z * 0.1
    leg_dims = {'x': leg_width, 'y': leg_height, 'z': leg_depth}

    leg_x_offset = x / 2 - leg_width / 2
    leg_z_offset = z / 2 - leg_depth / 2

    add_cube(vertices, faces, leg_dims, (-leg_x_offset, -leg_height / 2 + y*0.4, -leg_z_offset))
    add_cube(vertices, faces, leg_dims, (leg_x_offset, -leg_height / 2+ y*0.4, -leg_z_offset))
    add_cube(vertices, faces, leg_dims, (-leg_x_offset, -leg_height / 2+ y*0.4, leg_z_offset))
    add_cube(vertices, faces, leg_dims, (leg_x_offset, -leg_height / 2+ y*0.4, leg_z_offset))


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Procedural Asset Generator')
    parser.add_argument('brief', type=str, help='Path to the asset brief JSON file')
    args = parser.parse_args()

    with open(args.brief, 'r') as f:
        asset_brief = json.load(f)

    generate(asset_brief)