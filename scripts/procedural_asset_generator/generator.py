import json
import argparse
import os

def generate_cube(asset_brief):
    """Generates a cube .obj file from an asset brief."""

    asset_name = asset_brief['assetName']
    dimensions = asset_brief['dimensions']
    material = asset_brief['material']

    output_dir = f'Assets/Art/Procedural/{asset_name}'
    os.makedirs(output_dir, exist_ok=True)

    obj_path = f'{output_dir}/{asset_name}.obj'
    mtl_path = f'{output_dir}/{asset_name}.mtl'

    with open(obj_path, 'w') as f:
        f.write(f'mtllib {asset_name}.mtl\n')
        f.write(f'o {asset_name}\n')

        dx, dy, dz = dimensions['x'] / 2, dimensions['y'] / 2, dimensions['z'] / 2

        vertices = [
            (-dx, -dy, -dz),
            (dx, -dy, -dz),
            (dx, dy, -dz),
            (-dx, dy, -dz),
            (-dx, -dy, dz),
            (dx, -dy, dz),
            (dx, dy, dz),
            (-dx, dy, dz),
        ]

        for v in vertices:
            f.write(f'v {v[0]} {v[1]} {v[2]}\n')

        f.write('usemtl material_0\n')
        f.write('s off\n')

        faces = [
            (1, 2, 3, 4),
            (5, 8, 7, 6),
            (1, 5, 6, 2),
            (2, 6, 7, 3),
            (3, 7, 8, 4),
            (4, 8, 5, 1),
        ]

        for face in faces:
            f.write(f'f {face[0]} {face[1]} {face[2]} {face[3]}\n')

    with open(mtl_path, 'w') as f:
        f.write('newmtl material_0\n')
        # Placeholder material properties
        f.write('Ka 1.000000 1.000000 1.000000\n')
        f.write('Kd 1.000000 1.000000 1.000000\n')
        f.write('Ks 0.000000 0.000000 0.000000\n')
        f.write('Ns 10.000000\n')

    print(f'Generated {obj_path} and {mtl_path}')

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Procedural Asset Generator')
    parser.add_argument('brief', type=str, help='Path to the asset brief JSON file')
    args = parser.parse_args()

    with open(args.brief, 'r') as f:
        asset_brief = json.load(f)

    if asset_brief['type'] == 'cube':
        generate_cube(asset_brief)
    else:
        print(f"Unsupported asset type: {asset_brief['type']}")
