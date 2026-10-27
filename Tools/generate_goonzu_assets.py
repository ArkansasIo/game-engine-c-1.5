#!/usr/bin/env python3
"""Generate a complete deterministic SVG asset pack from goonzu_prompts.json."""
from __future__ import annotations
import argparse, json, re
from pathlib import Path
from html import escape

ROOT=Path(__file__).resolve().parents[1]
DEFAULT_PROMPTS=ROOT/'goonzu_prompts.json'
DEFAULT_OUT=ROOT/'Assets'/'GoonzuGame'/'Generated'
PALETTES={
 'character':('#25354a','#79b8ff','#dbeafe'),'npc':('#3b2f25','#f2c078','#fff0d0'),
 'enemy':('#4a2028','#ff6b6b','#ffe1e1'),'monster':('#38294f','#c084fc','#f3e8ff'),
 'environment':('#19362f','#63d6a2','#dcfff0'),'terrain':('#3c321f','#d9a441','#fff4ce'),
 'item':('#2b2e42','#aab4ff','#eceeff'),'weapon':('#31343b','#c7cedb','#ffffff'),
 'armor':('#29343a','#91a4ad','#e6f2f5'),'ui':('#20242c','#6ee7f9','#e6fbff'),
 'icon':('#20242c','#fbbf24','#fff7d6'),'default':('#242a33','#9aa7b8','#edf2f7')}

def slug(s):
    s=re.sub(r'[^A-Za-z0-9._-]+','_',str(s)).strip('_')
    return s[:100] or 'asset'

def classify(name,prompt):
    s=(str(name)+' '+str(prompt)).lower()
    groups={
      'character':['character','hero','player','human','knight','warrior','mage','ranger'],
      'npc':['npc','merchant','villager','blacksmith','trainer'],
      'enemy':['enemy','bandit','soldier','guard','boss'],
      'monster':['monster','dragon','orc','goblin','beast','undead','demon'],
      'environment':['tree','forest','building','castle','town','village','dungeon','environment','rock','water'],
      'terrain':['terrain','grass','sand','snow','mountain','ground','tile','road'],
      'weapon':['sword','axe','bow','staff','spear','weapon','dagger','hammer'],
      'armor':['armor','armour','helmet','shield','boots','gauntlet','chestplate'],
      'item':['item','potion','scroll','ring','amulet','ore','resource','material'],
      'ui':['ui','button','panel','window','hud','menu','cursor','frame'],
      'icon':['icon','symbol','badge','status']}
    for k,words in groups.items():
        if any(w in s for w in words): return k
    return 'default'

def collect(obj,inherited=''):
    out=[]
    if isinstance(obj,dict):
        path=next((obj[k] for k in ('path','file','filename','asset','output','output_file') if isinstance(obj.get(k),str)),None)
        prompt=next((obj[k] for k in ('prompt','description','text','visual','image_prompt') if isinstance(obj.get(k),str)),'')
        if path or prompt: out.append((path or inherited or 'asset',prompt or str(obj)))
        for k,v in obj.items(): out.extend(collect(v,k))
    elif isinstance(obj,list):
        for i,v in enumerate(obj): out.extend(collect(v,inherited or f'group_{i}'))
    return out

def unique_assets(entries):
    seen=set(); result=[]
    for i,(name,prompt) in enumerate(entries):
        base=slug(Path(name).stem if Path(name).suffix else name)
        if base in seen: base=f'{base}_{i:04d}'
        seen.add(base); result.append((base,prompt))
    return result

def svg_asset(asset_id,prompt,kind):
    bg,accent,light=PALETTES[kind]
    label=escape(asset_id.replace('_',' ')); desc=escape(str(prompt)[:240])
    shapes={
      'character':f'<circle cx="128" cy="72" r="34" fill="{light}"/><path d="M68 210 Q72 125 128 122 Q184 125 188 210Z" fill="{accent}"/>',
      'npc':f'<circle cx="128" cy="72" r="32" fill="{light}"/><path d="M70 208 Q78 132 128 128 Q178 132 186 208Z" fill="{accent}"/>',
      'enemy':f'<path d="M48 198 L68 92 L103 112 L128 55 L153 112 L188 92 L208 198Z" fill="{accent}"/><circle cx="101" cy="140" r="9" fill="{light}"/><circle cx="155" cy="140" r="9" fill="{light}"/>',
      'monster':f'<path d="M48 205 Q45 105 90 94 L108 58 L128 94 L148 58 L166 94 Q211 105 208 205Z" fill="{accent}"/>',
      'environment':f'<rect x="54" y="126" width="148" height="84" rx="8" fill="{accent}"/><path d="M48 126 L128 62 L208 126Z" fill="{light}"/><rect x="112" y="156" width="32" height="54" fill="{bg}"/>',
      'terrain':f'<path d="M34 204 L84 130 L116 164 L156 94 L222 204Z" fill="{accent}"/><path d="M34 204 H222" stroke="{light}" stroke-width="18"/>',
      'weapon':f'<path d="M74 194 L164 70 L184 90 L94 214Z" fill="{light}"/><path d="M58 194 L88 164 L112 188 L82 218Z" fill="{accent}"/>',
      'armor':f'<path d="M82 54 Q128 38 174 54 L190 112 L164 198 H92 L66 112Z" fill="{accent}"/>',
      'item':f'<path d="M86 72 Q128 50 170 72 L184 192 Q128 220 72 192Z" fill="{accent}"/>',
      'ui':f'<rect x="48" y="58" width="160" height="140" rx="12" fill="{accent}"/><rect x="68" y="82" width="120" height="16" rx="8" fill="{light}"/>',
      'icon':f'<circle cx="128" cy="128" r="72" fill="{accent}"/><path d="M128 76 L145 111 L184 116 L156 143 L163 182 L128 164 L93 182 L100 143 L72 116 L111 111Z" fill="{light}"/>',
      'default':f'<rect x="58" y="58" width="140" height="140" rx="24" fill="{accent}"/><circle cx="128" cy="128" r="42" fill="{light}"/>'}
    return f'<svg xmlns="http://www.w3.org/2000/svg" width="256" height="256" viewBox="0 0 256 256"><rect width="256" height="256" fill="{bg}"/><g>{shapes[kind]}</g><text x="128" y="232" text-anchor="middle" font-family="Arial,sans-serif" font-size="12" font-weight="700" fill="{light}">{label[:26]}</text><title>{label}</title><desc>{desc}</desc></svg>\n'

def main():
    ap=argparse.ArgumentParser(); ap.add_argument('--prompts',default=str(DEFAULT_PROMPTS)); ap.add_argument('--out',default=str(DEFAULT_OUT)); args=ap.parse_args()
    prompts=json.loads(Path(args.prompts).read_text(encoding='utf-8'))
    entries=unique_assets(collect(prompts))
    if not entries: entries=[(f'asset_{i:04d}','generic game asset') for i in range(393)]
    out=Path(args.out); out.mkdir(parents=True,exist_ok=True); manifest=[]
    for asset_id,prompt in entries:
        kind=classify(asset_id,prompt); rel=f'{asset_id}.svg'
        (out/rel).write_text(svg_asset(asset_id,prompt,kind),encoding='utf-8')
        manifest.append({'id':asset_id,'file':rel,'kind':kind,'prompt':prompt})
    (out/'manifest.json').write_text(json.dumps({'count':len(manifest),'assets':manifest},indent=2),encoding='utf-8')
    cols=10; tile=192; rows=(len(manifest)+cols-1)//cols
    parts=[f'<svg xmlns="http://www.w3.org/2000/svg" width="{cols*tile}" height="{rows*tile}" viewBox="0 0 {cols*tile} {rows*tile}"><rect width="100%" height="100%" fill="#10141b"/>']
    for i,m in enumerate(manifest):
        x=(i%cols)*tile; y=(i//cols)*tile
        parts.append(f'<g transform="translate({x},{y})">{svg_asset(m["id"],m["prompt"],m["kind"]).replace("width=\"256\" height=\"256\"","width=\"192\" height=\"192\"")}</g>')
    parts.append('</svg>'); (out/'goonzu_asset_atlas.svg').write_text(''.join(parts),encoding='utf-8')
    print(f'Generated {len(manifest)} SVG assets in {out}')

if __name__=='__main__': main()
