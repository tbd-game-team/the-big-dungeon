# Abschlussprojekt für Einführung in die Spieleprogrammierung 2022

## The Big Dungeon
Your goal is to find the coin of wisdome! By collecting it you win the game. But be careful!...A dungeon full of dangers is awaiting you!

#### Contributors
Neele Kemper, Niklas Nett, Floraian Weber

### Task Division

#### Neele Kemper
**Tasks**
* Implementation of the **procedural dungeon generation**
    * The tilemap visualization was adopted from [SunnyValleyStudio](https://github.com/SunnyValleyStudio/Unity_2D_Procedural_Dungoen_Tutorial)!  
* Audio
    * pick out audio sources   
    * **Audio** (player, theme, traps, health potions)
    * **Audio menu/settings**
* Implementation/animation of the **health potions**
* Implementation/animation of the **peak traps**
* Implementation/animation of the **coin**
* **Loading menu**

**Scripts**
* AudioScripts
    * AudioManager
    * Sound
    * VolumeSettings
* DungeonGeneratorScripts
    * SpawnScripts  
        * CoinSpawner
        * HealthPotionSpawner
        * TrapSpawner
    * AlgorithmUtils
    * AStartAlgorithm
    * BinarySpacePartitioningAlgorithm
    * CellularAutomataAlogrithm
    * Coordinate
    * Corridor Generator
    * DungeonGenerator
    * SpawnPositionGenerator
* PeakTraps 

**Game Objects/ Prefabs**
* MusicPanel
* LoadingPanel
* DungeonGenerator
* Coin
* AudioManager
* HealthPotion
* ReverbZone
* PeakTrap

#### Niklas Nett
**Tasks**
- Conceptation/implementation/animation of different enemies
- Implementation of combat
- Implementation of AI movement

**Scripts**
- Combat/Egg
- Combat/Enemy
- Combat/GenericEnemy
- Combat/Projectile
- DungeonGeneratorScripts/SpawnScripts/EnemySpawner
- needed modifications to others, like Keys, Player, DungeonGeneratorScripts/Coordinate

**Game Objects/Prefabs**
- Animations/Generic/*
- Animations/Ranged/*
- Prefabs/Combat/Projectile
- Prefabs/Combat/ProjectileHit
- Prefabs/Combat/Resources/Enemy
- Prefabs/Combat/Resources/EnemyEgg
- Prefabs/Combat/Resources/RangedEnemy

#### Florian Weber

**Tasks**
- Conceptation/implementation of player movement and combat
- Conceptation/implementation/animation of UI/UX

**Scripts**
- cameraFollow
- cameraShake
- dashMove
- GameManager
- HealthUI
- Player
- tutorialUI

**Game Objects/ Prefabs**
* UI
    * Created all Assets in Folder 'Own Graphic Assets' 
    * Canvas (Main Menu)
    * TutorialPanel
    * HitPanel
    * PausePanel
    * PauseButton
    * HelathBar
    * GameOverPanel
    * WinPanel
* Other
    * GameManager
    * Player
    * DashEffect



