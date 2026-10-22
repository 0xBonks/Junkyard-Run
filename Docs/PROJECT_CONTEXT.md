# Junkyard Run — Project Context

This document captures the current game direction and decisions from the design discussion. It complements `GAME.md`, which remains the original seed idea. Treat items under **Open decisions** as unresolved rather than settled requirements.

## Current pitch

Up to four friends use physics nets to snag scrap in an absurd junkyard, toss their catches into a shared cart, and build an escape car—while thieves, difficult terrain, and bosses turn each run into slapstick chaos.

## Design direction

- **Main hook:** Co-op chaos. The vehicle and zone progression give the scavenging a goal, but the memorable moments should come from friends catching, hauling, bumping into, and rescuing one another.
- **Tone:** Slapstick first. Mistakes can cost unbanked loot or a crafted net, but moment-to-moment mishaps should be funny.
- **Players:** 1–4 online. One player hosts and invites others through the Steam friends list.
- **View:** Third-person 3D.
- **Escape goal:** Build one fixed ground vehicle incrementally.

## Core scavenging loop

1. At base, choose and craft single-use nets from scrap mesh. Net sizes have a simple tradeoff: smaller nets are cheaper; larger nets catch more.
2. In the junkyard, aim and set launch power. Scrap values are visible before firing. Player collisions can knock scrap into motion, rewarding players who lead a target with their shot.
3. A successful catch becomes a physical net-bag, possibly containing mixed-value scrap. Bags can be tossed into the shared cart and bounce intact. Players can sort a bag instantly in the field.
4. The cart has limited bag slots. Tossing a bag into it transfers the catch to shared cargo, but the haul is not secure until it reaches base. Unbanked cargo can be lost.
5. At base, use a simple physical crafting station (a hold interaction) to turn salvage into vehicle parts.

### Threats and co-op mishaps

- A thief can steal a stack and flee. Players can use the net to catch it and recover the stack; separate combat weapons are postponed from the initial scope.
- A net can briefly wrap a teammate. A partner frees them, but the net is still consumed.
- Hauling should create teamwork. In deep mud, some players lift the cart while others push. The group catches together and hauls together.
- Solo play remains supported, but is riskier. Bosses attack more slowly for solo players; cargo remains vulnerable if left unattended.

## Progression example

- Craft a ram from steel and a scrap frame by welding them together.
- Use the ram to break a barricade and reach a giant raccoon in a wrecked car.
- Defeat the raccoon by coordinating net shots to pin multiple limbs. Defeating it unlocks the next zone.
- A later zone introduces deep mud, which makes cart hauling a cooperative lift-and-push challenge.

## First prototype and content slice

- **Mechanic test:** Start with a solo net-physics prototype. The success signal is that players improve their aim with practice and want to take another shot.
- **First content slice:** One zone with the full basic loop: catch scrap, deal with a thief, haul the cart home, and craft one vehicle part. Keep the boss and later zones out of this first slice.
- **Scope caution:** The intended project is a solo-dev indie, while four-player online physics is a substantial risk. Prove the core net and hauling loop before expanding content.

## Open decisions

- How personal pack slots relate to the physical net-bags and cart slots. Type-based stacking was considered, but the later physical-bag design needs to be reconciled with it.
- Exact scrap types, net recipes/costs, cart capacity, and the ram recipe quantities.
- How thieves announce themselves and what happens if they escape with a stack; solo play is intentionally riskier, but the warning/response window is not defined.
- How the 1–4 player experience scales beyond slower solo bosses, especially cart handling and multi-limb boss fights.
- The full zone sequence, additional boss designs, and the exact final vehicle build requirements.
- Final title: `Junkyard Run` or `Junkyard Chaos`.
