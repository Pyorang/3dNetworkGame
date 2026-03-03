# 3dNetworkGame Project Overview

## Purpose
Unity 3D multiplayer network game using Photon PUN2.

## Tech Stack
- Unity (C#)
- Photon PUN2 for networking
- TextMesh Pro for UI

## Project Structure
- `Assets/02. Scripts/` - Main scripts
  - `Bear/` - Bear monster (BearStat, BearController)
  - `Core/` - Interfaces (IDamageable), utilities (Billboard)
  - `Player/` - Player scripts (Controller, Abilities, Stats)
- `Assets/06. Animations/` - Animations
- `Assets/04. Prefabs/` - Prefabs

## Code Style
- PascalCase for public fields, methods, classes
- _camelCase for private fields
- `PlayerAbility` base class pattern for player abilities
- Static readonly hash IDs for Animator parameters
- `[RequireComponent]` attribute usage
- IDamageable interface for damage system

## Key Patterns
- Player uses ability-based architecture (PlayerAbility base class)
- Stats use property with Clamp and events (OnHpChanged)
- Photon networking with IsMine checks
