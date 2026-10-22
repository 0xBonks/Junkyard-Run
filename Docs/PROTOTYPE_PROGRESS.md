# Junkyard Run — Prototype Progress

**Updated:** 2026-10-22  
**Purpose:** Running implementation status for the Unity prototype. `PROJECT_CONTEXT.md` remains the source for game direction and design decisions.

## Current prototype setup

- Unity 6 URP project; Input System is being used for controls.
- Main test scene: `NetRange`.
- The scene contains a ground plane, a first-person player, a camera, and three scrap targets.
- The player uses a `CharacterController`, WASD movement, and mouse look.

## Implemented

- **Net launcher:** Left-click fires a Rigidbody projectile from the player's muzzle toward the camera aim direction.
- **Scrap capture:** Scrap targets use the `Scrap` tag. A hit converts the projectile into a bag and parents the caught scrap to it.
- **Bag handling:** The bag stops carrying the shot's launch momentum, then settles under gravity. Press **E** to pick up or drop it; press **F** to throw it.
- **Net HUD:** A dynamic net counter appears at bottom-left. The prototype starts with `10/10` nets.
- **Placeholder hotbar:** Five simple UI rectangles are arranged at the bottom center; these are layout placeholders, not implemented inventory slots.
- **Graybox cart:** The cart has a cargo trigger, visual base and walls, and a `CartCargo` script. Accepted bags are parented to the cart and parked in cargo positions. A TextMeshPro UI label displays `CARGO n/3`.

## Current issue: cart capacity test

The cart was expected to reject a third bag when its capacity was temporarily set to `2`. In the latest Play-mode test, three `NetProjectile(Clone)` objects appeared under `Cart`, the HUD showed `CARGO 3/3`, and the Console had no `Cart is full.` message. This indicates the active `CartCargo` component was still using capacity `3` or the edited value was applied to a different object.

### Next debugging steps

1. Stop Play mode and select the active `Cart` object in the Hierarchy.
2. In its `CartCargo (Script)` component, set **Capacity** to `2` and verify the Inspector shows `2` before Play mode.
3. Save and start a fresh Play session. Before loading any bags, the HUD should show `CARGO 0/2`.
4. Load two bags; expect `CARGO 2/2`. A third should remain unaccepted and log `Cart is full.`
5. If the HUD starts at `0/3`, check for a duplicate cart or `CartCargo` component, or confirm the selected object is the one with the cargo trigger.

## Remaining prototype work

- Finish verifying the cart's capacity/full behavior.
- Make the cart movable and prototype returning it to base to bank cargo.
- Later: add the first-zone loop around the working net, bag, cart, and banking mechanics.

## Controls

| Action | Control |
|---|---|
| Move | WASD |
| Aim/look | Mouse |
| Fire net | Left mouse button |
| Pick up / drop caught bag | E |
| Throw held bag | F |
