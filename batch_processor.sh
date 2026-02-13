#!/bin/bash

# Goonzu Asset Batch Processor
# Helps process the generated prompts for AI image generation

echo "=== Goonzu Asset Batch Processor ==="
echo

# Check if prompts file exists
if [ ! -f "goonzu_prompts.json" ]; then
    echo "Error: goonzu_prompts.json not found. Run generate_ai_assets.py first."
    exit 1
fi

echo "Available options:"
echo "1. Show sample prompts"
echo "2. Count assets by category"
echo "3. Create batch files for AI platforms"
echo "4. Validate generated PNGs"
echo "5. Create replacement script"
echo

read -p "Choose option (1-5): " choice

case $choice in
    1)
        echo "=== Sample Prompts ==="
        echo "First 5 prompts:"
        python3 -c "
import json
with open('goonzu_prompts.json', 'r') as f:
    prompts = json.load(f)
for i, (filename, prompt) in enumerate(list(prompts.items())[:5]):
    print(f'{i+1}. {filename}:')
    print(f'   {prompt}')
    print()
"
        ;;
    2)
        echo "=== Asset Count by Category ==="
        python3 -c "
import json
from pathlib import Path

with open('goonzu_prompts.json', 'r') as f:
    prompts = json.load(f)

categories = {}
for filename in prompts.keys():
    category = Path(filename).parent.name
    categories[category] = categories.get(category, 0) + 1

for category, count in sorted(categories.items()):
    print(f'{category}: {count}')
print(f'Total: {len(prompts)}')
"
        ;;
    3)
        echo "=== Creating Batch Files ==="
        mkdir -p batch_files

        # Create batches of 10 prompts each
        python3 -c "
import json
import os

with open('goonzu_prompts.json', 'r') as f:
    prompts = json.load(f)

batch_size = 10
batch_num = 1
current_batch = []

for filename, prompt in prompts.items():
    current_batch.append(f'{filename}: {prompt}')

    if len(current_batch) >= batch_size:
        with open(f'batch_files/batch_{batch_num:02d}.txt', 'w') as f:
            f.write(f'Batch {batch_num} - {len(current_batch)} assets\\n')
            f.write('=' * 50 + '\\n\\n')
            for item in current_batch:
                f.write(item + '\\n\\n')
        batch_num += 1
        current_batch = []

# Write remaining items
if current_batch:
    with open(f'batch_files/batch_{batch_num:02d}.txt', 'w') as f:
        f.write(f'Batch {batch_num} - {len(current_batch)} assets\\n')
        f.write('=' * 50 + '\\n\\n')
        for item in current_batch:
            f.write(item + '\\n\\n')

print(f'Created {batch_num} batch files in batch_files/')
"
        ;;
    4)
        echo "=== Validating Generated PNGs ==="
        echo "Checking for PNG files in Assets/GoonzuGame/..."
        png_count=$(find Assets/GoonzuGame -name "*.png" | wc -l)
        txt_count=$(find Assets/GoonzuGame -name "*.txt" | wc -l)

        echo "PNG files: $png_count"
        echo "TXT files: $txt_count"
        echo "Conversion progress: $((png_count * 100 / (png_count + txt_count)))%"
        ;;
    5)
        echo "=== Creating Replacement Script ==="
        cat > replace_placeholders.sh << 'EOF'
#!/bin/bash
# Replace .txt placeholders with .png files from GeneratedAssets/

echo "Replacing placeholder .txt files with generated .png files..."

if [ ! -d "GeneratedAssets" ]; then
    echo "Error: GeneratedAssets directory not found"
    exit 1
fi

replaced=0
for txt_file in Assets/GoonzuGame/**/*.txt; do
    png_file="${txt_file%.txt}.png"
    generated_png="GeneratedAssets/$(basename "$png_file")"

    if [ -f "$generated_png" ]; then
        mv "$generated_png" "$png_file"
        rm "$txt_file"
        echo "✓ Replaced: $txt_file -> $png_file"
        ((replaced++))
    fi
done

echo "Replaced $replaced placeholder files"
EOF

        chmod +x replace_placeholders.sh
        echo "Created replace_placeholders.sh script"
        ;;
    *)
        echo "Invalid option"
        ;;
esac