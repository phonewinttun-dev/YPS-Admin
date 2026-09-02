# YPS Admin Panel Design System (Swiss + Brutalist Minimalism)

The application uses a **Swiss + Brutalist Minimalism** interface designed for high legibility, tactile feedback, and eye comfort under OS Night Light (warm color temperatures 2700K–3400K).

---

## Core Philosophy

1. **Swiss Precision**: Bold geometric typography, structured grid layout, stark hierarchy, tight letter-spacing.
2. **Neo-Brutalist Structure**: 2px solid outlines, hard offset drop shadows (`3.5px 3.5px 0px`), rounded pill buttons, tactile interactive states.
3. **Strict Zero-Emoji Policy**: Zero emojis anywhere in the UI or codebase. All visual iconography uses clean, standardized vector SVGs from [reUI](https://reui.io/) (24×24 stroke icons with `stroke-width="2"`).
4. **Night-Light Friendly Color Architecture**: Anti-glare warm-tinted base tokens prevent fluorescent white blinding glare and harsh haloing when Windows Night Light or macOS Night Shift is active.

---

## Semantic Color System

| Role | Light Mode ("Warm Alabaster") | Dark Mode ("Warm Obsidian") | Meaning / Usage |
| :--- | :--- | :--- | :--- |
| **Canvas** | `#F4F2EC` (Warm Paper) | `#111215` (Deep Obsidian) | Page backdrop |
| **Surface** | `#FCFBF8` (Soft Card) | `#191B20` (Warm Graphite) | Cards, panels, modals |
| **Foreground**| `#1A1B1E` (Charcoal Ink) | `#E8E8E2` (Warm Bone) | Primary headings & text |
| **Border** | `#1A1B1E` (2px Solid) | `#383D48` (2px Softened) | Brutalist outlines |
| **Shadow** | `#1A1B1E` (`3.5px 3.5px`) | `#0A0A0C` (`3.5px 3.5px`) | Tactile hard offset shadow |
| **Brand (YPS)**| `#E65A20` (Terracotta) | `#F26C35` (Amber Orange) | YPS branding & identity |
| **Bus** | `#1E66B6` (Steel Navy) | `#4F8FE2` (Sky Blue) | Bus lines & transit routes |
| **GPS / Stop**| `#15803D` (Forest Green) | `#22C55E` (Emerald Green) | Bus stops & geolocation |
| **Store** | `#A23F2B` (Warm Coral) | `#FF9C85` (Salmon Peach) | YPS card stores & counters |
| **Danger** | `#DC2626` (Crimson) | `#EF4444` (Coral Red) | Destructive actions |

---

## Typography

- **English (`[lang="en"]`)**: **SF Pro** (`-apple-system, BlinkMacSystemFont, "SF Pro Text", "SF Pro Display"`) with tight Swiss tracking (`-0.015em`).
- **Myanmar (`[lang="my"]`)**: **Padauk** with comfortable line height (`1.65`) and natural spacing.
- **Numbers / Telemetry**: **JetBrains Mono** with tabular lining figures (`font-feature-settings: "tnum" 1`).

---

## Iconography Standard

- **Source**: [reUI](https://reui.io/) (Lucide vector specification).
- **Style**: 24×24 viewBox, `fill="none"`, `stroke="currentColor"`, `stroke-width="2"`, `stroke-linecap="round"`, `stroke-linejoin="round"`.
- **Policy**: Emojis (☀️, 🌙, 🚌, 📍, 🗺️, 💳, 🛡️, etc.) are strictly prohibited.

---

## Component Standards

- **Buttons**: Rounded pills (`rounded-full`) with 2px solid border, `2.5px 2.5px 0px` hard shadow, active click displacement (`translate(1.5px, 1.5px)` with zero shadow). Minimum 36px–44px touch targets.
- **Cards**: `rounded-2xl`, 2px solid border, `3.5px 3.5px 0px` hard shadow.
- **Badges**: Rounded pill tags (`rounded-full`) with uppercase monospace styling and 1.5px solid borders.
- **Modals**: 2.5px solid border, `rounded-2xl`, `5px 5px 0px` hard shadow, reUI `X` close icon.
- **A11y**: Minimum 4.5:1 contrast for regular text (exceeds 7:1 in practice), visible keyboard focus rings.
