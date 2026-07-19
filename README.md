# Crossmod Calibrator

[tModloader Workshop Link](https://steamcommunity.com/sharedfiles/filedetails/?id=3766100815)

Crossmod Calibrator is a mod designed to enable easy balancing of different content mods without needing dedicated compatibility mods so that you can ensure one mod does not significantly overpower another, have your own fun with challenge runs, or other uses you find.

Crossmod Calibrator automatically detects vanilla and modded weapons, enemies, and bosses, then creates server-side settings for each content source:

- Weapon damage
- Boss health and damage
- Enemy health and damage

Each setting is a direct multiplier from 0.0001 to 10000. Settings remain saved when a content mod is disabled, so they return when that mod is enabled again. Only categories actually detected for a mod are shown.

Most content requires no manual support while others may be unsupported due to the vague nature in recognizing custom content. 

Note: At this moment from testing it has been indicated that one mod modifying another--e.g. Calamity Infurnum Mode modifying Calamity bosses--will choose one mod to prioritize for the boss' stats, which comes down to those mods individual implementations. If you encounter this issue, it is recommended to simply give both mods the same stats for boss/enemy stats in order to solve the vagueness.