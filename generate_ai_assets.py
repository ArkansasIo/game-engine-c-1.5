#!/usr/bin/env python3
"""
Goonzu Asset Generator - AI Image Generation Script
Generates actual PNG sprites from the placeholder prompts using various AI platforms
"""

import os
import json
import requests
import time
from pathlib import Path
import argparse

class GoonzuAssetGenerator:
    def __init__(self, assets_dir="Assets/GoonzuGame"):
        self.assets_dir = Path(assets_dir)
        self.prompts = {}
        self.load_prompts()

    def load_prompts(self):
        """Load all prompts from the generated .txt files"""
        for txt_file in self.assets_dir.rglob("*.txt"):
            with open(txt_file, 'r') as f:
                content = f.read()
                # Extract prompt from the file
                if "# Prompt:" in content:
                    prompt_line = [line for line in content.split('\n') if line.startswith("# Prompt:")][0]
                    prompt = prompt_line.replace("# Prompt:", "").strip()
                    png_path = txt_file.with_suffix('.png')
                    self.prompts[str(png_path)] = prompt

        print(f"Loaded {len(self.prompts)} asset prompts")

    def generate_with_recraft_ai(self, api_key, output_dir="GeneratedAssets"):
        """Generate images using Recraft AI API"""
        if not api_key:
            print("Recraft AI API key required")
            return

        output_path = Path(output_dir)
        output_path.mkdir(exist_ok=True)

        headers = {
            'Authorization': f'Bearer {api_key}',
            'Content-Type': 'application/json'
        }

        for png_path, prompt in self.prompts.items():
            try:
                # Recraft AI API call
                data = {
                    'prompt': prompt,
                    'style': 'anime',
                    'width': 512,
                    'height': 512
                }

                response = requests.post('https://api.recraft.ai/v1/generate', json=data, headers=headers)
                if response.status_code == 200:
                    result = response.json()
                    image_url = result.get('image_url')

                    # Download the image
                    img_response = requests.get(image_url)
                    if img_response.status_code == 200:
                        output_file = output_path / Path(png_path).name
                        with open(output_file, 'wb') as f:
                            f.write(img_response.content)
                        print(f"✓ Generated: {output_file}")
                    else:
                        print(f"✗ Failed to download image for {png_path}")
                else:
                    print(f"✗ API error for {png_path}: {response.status_code}")

                # Rate limiting
                time.sleep(1)

            except Exception as e:
                print(f"✗ Error generating {png_path}: {e}")

    def generate_with_scenario_ai(self, api_key, output_dir="GeneratedAssets"):
        """Generate images using Scenario AI API"""
        if not api_key:
            print("Scenario AI API key required")
            return

        output_path = Path(output_dir)
        output_path.mkdir(exist_ok=True)

        headers = {
            'Authorization': f'Bearer {api_key}',
            'Content-Type': 'application/json'
        }

        for png_path, prompt in self.prompts.items():
            try:
                data = {
                    'prompt': prompt,
                    'style': 'anime',
                    'resolution': 'square'
                }

                response = requests.post('https://api.scenario.ai/v1/generate', json=data, headers=headers)
                if response.status_code == 200:
                    result = response.json()
                    image_url = result.get('image_url')

                    img_response = requests.get(image_url)
                    if img_response.status_code == 200:
                        output_file = output_path / Path(png_path).name
                        with open(output_file, 'wb') as f:
                            f.write(img_response.content)
                        print(f"✓ Generated: {output_file}")
                    else:
                        print(f"✗ Failed to download image for {png_path}")
                else:
                    print(f"✗ API error for {png_path}: {response.status_code}")

                time.sleep(1)

            except Exception as e:
                print(f"✗ Error generating {png_path}: {e}")

    def export_prompts_json(self, output_file="goonzu_prompts.json"):
        """Export all prompts to JSON for manual processing"""
        with open(output_file, 'w') as f:
            json.dump(self.prompts, f, indent=2)
        print(f"Exported {len(self.prompts)} prompts to {output_file}")

    def export_prompts_csv(self, output_file="goonzu_prompts.csv"):
        """Export prompts to CSV format"""
        import csv
        with open(output_file, 'w', newline='') as f:
            writer = csv.writer(f)
            writer.writerow(['filename', 'prompt'])
            for filename, prompt in self.prompts.items():
                writer.writerow([filename, prompt])
        print(f"Exported {len(self.prompts)} prompts to {output_file}")

def main():
    parser = argparse.ArgumentParser(description='Generate Goonzu assets using AI')
    parser.add_argument('--platform', choices=['recraft', 'scenario', 'export'], default='export',
                       help='AI platform to use')
    parser.add_argument('--api-key', help='API key for the chosen platform')
    parser.add_argument('--output-dir', default='GeneratedAssets', help='Output directory for generated images')
    parser.add_argument('--assets-dir', default='Assets/GoonzuGame', help='Directory containing asset prompts')

    args = parser.parse_args()

    generator = GoonzuAssetGenerator(args.assets_dir)

    if args.platform == 'export':
        generator.export_prompts_json()
        generator.export_prompts_csv()
        print("\nTo generate images manually:")
        print("1. Use the JSON/CSV files with your preferred AI platform")
        print("2. Or use the individual .txt files in Assets/GoonzuGame/")
        print("3. Replace .txt files with generated .png files")

    elif args.platform == 'recraft':
        if not args.api_key:
            print("Error: --api-key required for Recraft AI")
            return
        generator.generate_with_recraft_ai(args.api_key, args.output_dir)

    elif args.platform == 'scenario':
        if not args.api_key:
            print("Error: --api-key required for Scenario AI")
            return
        generator.generate_with_scenario_ai(args.api_key, args.output_dir)

if __name__ == "__main__":
    main()