[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/xb_1Q-N5)
Galaxian Remake - Project Documentation
This project is a tribute to the classic arcade shooter Galaxian. It features complex formation logic, spline-based enemy movement, and a modular architecture using ScriptableObjects.
The player controls a starship called the "Galaxip", the objective being to clear each round of aliens. The enemies appear in formation towards the top of the screen, with two escort ships, labeled the "Galaxian Flagship".
Enemies will make a divebomb towards the bottom of the screen while shooting projectiles in an attempt to hit the player. The Galaxip can only fire a single shot at a time, and the player must wait for it to either hit an enemy or the top of the screen before being able to fire another, due to limitations of the hardware.
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Core Game Mechanics:
The game is built on several specialized systems that handle the unique arcade feel:

1. Dynamic Formation and Escort Logic
Enemies are organized into a structured group managed by the EnemyGroupOrder script.
•	Procedural Layout: Enemies are instantiated in rows based on their specific roles.
•	Flagship Escort Requirement: A specific rule is enforced for the yellow Flagship enemies. A Flagship cannot initiate a diving attack alone; it requires two red escort enemies to dive with it. The system identifies the enemy type and checks for available red escorts before triggering the attack sequence.
•	Parent-Child Logic: While in formation, enemies move as children of a central Formation Root. When attacking, they dynamically unparent themselves to gain full movement freedom.

2. Spline-Based Combat (Diving)
The AttackDirector manages enemy behavior through specialized movement phases:
•	Diving Phase: Selected enemies transition to a Diving state, following complex Spline paths toward the player.
•	Bezier Return: Instead of teleporting back, enemies use a Quadratic Bezier Curve in the ReturnToGroup script to arc gracefully back to their specific slot in the moving formation.

3. Special Mechanic- Falling Stars:
The environment features a unique interactive star system with two distinct types of objects:
White Hazard Stars:
•	Appearance: These stars spawn at random locations, initially appearing small to blend in with the background.
•	Growth and Danger: They gradually increase in size and then fall toward the player.
•	Penalty: If a White Star hits the player, the player loses a life.
Colored Bonus Stars:
•	Interaction: These stars are colorful and start small. The player is encouraged to shoot them.
•	Mechanic: When shot, the star expands. After a growth period, it shrinks and disappears.
•	Scoring: Every successful shot grants the player 50 points. Players aim to hit these stars as many times as possible before they vanish.
________________________________________
Technical Architecture
The project utilizes modular data structures and specialized rendering tools:
1. Data-Driven Design (ScriptableObjects)
To keep the project organized, ScriptableObjects were used for core data management:
•	Enemy Data: Defines health, speed, and point values for different enemy types.
•	Sound System: Audio clips and settings are stored in ScriptableObjects, allowing for easy management of sound effects without hardcoding references.
2. Visuals and Rendering:
•	Pixel Perfect Camera: To maintain the retro arcade aesthetic, a Pixel Perfect Camera component is used. This ensures that sprites remain crisp, maintain their pixel-grid alignment, and avoid jittery movement during scaling or rotation.
3. Manager Hierarchy
•	SessionManager: Controls the high-level GameState (Attract, Playing, Victory, GameOver).
•	WaveManager: Handles the logic of each wave and rebuilding formations.
•	HudManager: Decoupled from the game logic, it listens for events to update the display.
________________________________________
Cheat System
A comprehensive cheat suite is integrated via the CheatManager for rapid testing and debugging:
Key	Action	Description:
P-	Skip Sequence	Skips the "Attract" intro animations and jumps directly into gameplay.
R-	Reset Lives	Restores player health to maximum and updates the UI via events.
H-	Take Hit	Manually triggers the player's damage logic to test invulnerability and Game Over screens.
K-	Instant Kill	Destroys all active enemies on screen to test Victory transitions.
S-	Stop Dives	Recalls all diving enemies back to the formation immediately.
Enter- Starts and restarts the game.

Technical Highlights
•	Identity Recognition: The system distinguishes between enemy types (Yellow vs. Red) to enforce specific rules like the Escort mechanic.
•	Lifecycle Management: Uses OnEnable and OnDisable for clean registration of enemies and stars in static lists.
•	Efficient UI: Uses TextMeshPro with sprite injection for an authentic arcade font feel.


