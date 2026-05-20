# ⚔️ SoulsBorne RPG — 3D Action RPG in Unity (C#)
 
![Unity](https://img.shields.io/badge/Unity-2022%20LTS%20%7C%20Unity%206-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-.NET%206-purple?logo=csharp)
![License](https://img.shields.io/badge/License-MIT-green)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow)
 
> A dark fantasy, third-person **Souls-like Action RPG** built in Unity using C#.  
> Explore a cursed kingdom, master punishing combat, level up your stats, and survive a brutal two-phase boss fight.
 
---
 
## 🎮 Gameplay Overview
 
| Feature | Description |
|---|---|
| ⚔️ Combat | Light/heavy attacks, parry, riposte, stamina management |
| 🧍 Movement | Root-motion locomotion, dodge-roll with i-frames |
| 🧠 Enemy AI | Hierarchical FSM — Idle → Patrol → Chase → Attack → Death |
| 👹 Boss Fight | Two-phase boss with unique abilities triggered at 50% HP |
| 💀 Death System | Drop souls on death, retrieve them — or lose them forever |
| 🔥 Checkpoints | Bonfire system restores HP and respawns enemies |
| 📦 Inventory | Grid-based drag-and-drop inventory using ScriptableObjects |
| 💾 Save System | Multi-slot binary save/load with `ISerializable` |
 
---
 
## 🗂️ Project Structure
 
```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Player/
│   │   │   ├── PlayerController.cs       ← Locomotion, stamina, dodge
│   │   │   ├── CombatSystem.cs           ← Attack chains, parry, riposte
│   │   │   └── LockOnSystem.cs           ← Soft lock-on + camera
│   │   ├── Enemy/
│   │   │   ├── EnemyStateMachine.cs      ← HFSM with all AI states
│   │   │   └── BossAI.cs                 ← Two-phase boss logic
│   │   ├── Combat/
│   │   │   └── HealthSystem.cs           ← Poise, damage pipeline, death
│   │   ├── Systems/
│   │   │   ├── SoulsManager.cs           ← Drop/retrieve/lose souls
│   │   │   ├── SaveSystem.cs             ← Multi-slot save/load
│   │   │   └── InventorySystem.cs        ← Grid inventory
│   │   └── Managers/
│   │       └── GameManager.cs            ← Game state, bonfire, respawn
│   ├── ScriptableObjects/
│   │   ├── Weapons/
│   │   ├── Armor/
│   │   └── Enemies/
│   ├── Animations/
│   ├── Prefabs/
│   └── Scenes/
│       ├── MainMenu
│       ├── Zone_01
│       └── BossArena
```
 
---
 
## 🧍 Player Systems
 
### Stats
| Stat | Effect |
|---|---|
| **STR** | Physical damage scaling |
| **DEX** | Attack speed, bleed scaling |
| **VIT** | Max HP |
| **END** | Max stamina, equip load |
 
### Combat Architecture
 
```csharp
public interface IDamageable {
    void TakeDamage(DamageData data);
}
 
public struct DamageData {
    public float physicalDamage;
    public float fireDamage;
    public float poiseDamage;
    public Vector3 hitDirection;
    public DamageType type; // Slash, Pierce, Blunt, Magic
}
```
 
**Damage Pipeline:**
```
Raw Damage → Stat Scaling → Armor Reduction → Poise Check → Apply
```
 
---
 
## 🤖 Enemy AI — State Machine
 
```
Idle ──► Patrol ──► Alert ──► Chase ──► Attack
                                          │
                                       Stagger
                                          │
                                        Death
```
 
- Enemies have **poise** — break it to stagger them
- Ranged enemies use **predictive arc aiming**
- Allies alert each other via `Physics.OverlapSphere`
- Boss transitions to **Phase 2** at 50% HP with new move sets
---
 
## 🌍 World Systems
 
- 🌐 **Addressables** — async scene streaming for seamless zone loading
- ☀️ **Day/Night Cycle** — rotating directional light + skybox lerp
- 🔥 **Bonfire System** — rest to restore HP, respawn enemies, unlock fast travel
- 💥 **Destructibles** — `Rigidbody` + mesh replacement on destruction
---
 
## 📷 Camera
 
- **Cinemachine** with custom geometry collision (no clipping)
- Smooth lock-on transitions via `Cinemachine Blend`
- Boss intro **cutscenes** via Timeline
---
 
## ⚡ Performance
 
- Object pooling for projectiles, VFX, and damage numbers
- AI runs on a **staggered tick system** via central `AIManager` (not every frame)
- `[ProfilerMarker]` on heavy methods for Unity Profiler visibility
- **Shader Graph** shaders: hit flash, dissolve-on-death, outline effects
---
 
## 🚀 Getting Started
 
### Prerequisites
 
- Unity **2022 LTS** or **Unity 6**
- Universal Render Pipeline (**URP**)
- Packages: `Cinemachine`, `Addressables`, `Input System`, `Netcode for GameObjects` *(optional)*
### Installation
 
```bash
# Clone the repository
git clone https://github.com/your-username/soulsborne-rpg.git
 
# Open in Unity Hub
# Select Unity 2022 LTS or Unity 6
# Open project and let packages resolve
```
 
### First Run
 
1. Open `Assets/_Game/Scenes/MainMenu`
2. Press **Play** in the Unity Editor
3. Configure player stats in `PlayerData` ScriptableObject
4. Bonfires auto-save — check `SaveSystem.cs` for save slot config
---
 
## 🔧 Configuration
 
Key values are serialized in the Inspector for easy tuning:
 
```csharp
[Header("Movement")]
[SerializeField] private float walkSpeed = 3f;
[SerializeField] private float sprintSpeed = 6f;
[SerializeField] private float dodgeDistance = 4f;
[SerializeField] private float iFrameDuration = 0.6f;
 
[Header("Stamina")]
[SerializeField] private float maxStamina = 100f;
[SerializeField] private float staminaRegenRate = 20f;
[SerializeField] private float staminaRegenDelay = 1.2f;
```
 
---
 
## 🗺️ Roadmap
 
- [x] Player locomotion & stamina system
- [x] Combat: light/heavy attack, parry, riposte
- [x] Enemy HFSM with all states
- [x] Souls drop/retrieve/lose system
- [x] Bonfire checkpoint system
- [x] Multi-slot save system
- [ ] Full inventory UI with drag-and-drop
- [ ] Dialogue & quest system
- [ ] Multiplayer co-op via Netcode for GameObjects
- [ ] Additional zones (3 planned)
- [ ] Steam release build
---
 
## 🤝 Contributing
 
Pull requests are welcome!
 
```bash
# Fork the repo
# Create your feature branch
git checkout -b feature/my-feature
 
# Commit your changes
git commit -m "Add: my awesome feature"
 
# Push and open a PR
git push origin feature/my-feature
```
 
Please follow the existing code style — XML doc comments on all public methods, `[SerializeField]` for Inspector-exposed fields.
 
---
 
## 📄 License
 
This project is licensed under the **MIT License** — see [LICENSE](LICENSE) for details.
 
---
 
## 👤 Author
 
Made with ❤️ and suffering (just like Dark Souls) by **Your Name**  
🐦 Twitter: [@yourhandle](https://twitter.com/yourhandle)  
🌐 Portfolio: [yourwebsite.com](https://yourwebsite.com)
 
---
 
> *"Darkness is not always the enemy. Sometimes it's just the beginning."*
 
