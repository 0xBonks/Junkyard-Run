# Junkyard Run — Prototype Progress

**Updated:** 2026-10-22  
**Purpose:** Running implementation status for the Unity prototype. `PROJECT_CONTEXT.md` remains the source for game direction and design decisions.

## Current prototype setup

- Unity 6 URP project; Input System is being used for controls.
- Main test scene: `NetRange`.
- The scene contains a 40-by-40 ground plane, a first-person player, a camera, three scrap targets, and a visible collidable perimeter fence.
- The player uses a `CharacterController`, WASD movement, and mouse look.

## Implemented

- **Net launcher:** Left-click fires a Rigidbody projectile from the player's muzzle toward the camera aim direction.
- **Scrap capture:** Scrap targets use the `Scrap` tag. A hit converts the projectile into a bag and parents the caught scrap to it.
- **Bag handling:** The bag stops carrying the shot's launch momentum, then settles under gravity. Press **E** to pick up or drop it; press **F** to throw it.
- **Net HUD:** A dynamic net counter appears at bottom-left. The prototype starts with `5/10` nets; the blue refill station restores the full supply.
- **Placeholder hotbar:** Five simple UI rectangles are arranged at the bottom center; these are layout placeholders, not implemented inventory slots.
- **Graybox cart:** The cart has a cargo trigger, visual base and walls, and a `CartCargo` script. Accepted bags are parented to the cart and parked in cargo positions. A Rigidbody and player-contact push allow hauling; the cart now uses a separate acceleration-based push to overcome ground friction. The HUD shows cargo and banked bag totals.
- **Banking prototype:** A separate `BankingZone` trigger at the far end of `NetRange` banks accepted bags once, clears the cart, and allows it to be reloaded. Banking currently counts bags, not scrap value.
- **Delivery marker:** The cart banking trigger now builds a bright ground pad and a labeled `CART DELIVERY` sign facing the player approach side at runtime.
- **Net refill:** A blue `NET REFILL` station near the starting area refills the launcher to maximum when the player enters it and presses **R**.
- **Thief prototype:** A capsule thief seeks unbagged scrap, carries one piece toward an escape point, and displays a red overhead alert while carrying it. Netting the thief recovers its scrap into the net-bag; the visible thief is briefly stunned instead of hidden. If it reaches the exit, it drops the scrap inside the fence and visibly returns to the yard. A bounds recovery prevents it from falling off the arena.

## Current verification: haul, bank, and thief loop

The saved scene previously had the Cart's serialized capacity set to `3`, matching the reported `CARGO 3/3`. The scene is now set to `2` for a fresh capacity check; the intended final capacity remains undecided.

**Latest Play-mode report:** the thief repeatedly stole scrap and dropped it at the far fence before returning; net refill succeeded; and the delivery trigger banked two bags. The user reports the loop is working as intended.

### Play-mode checklist

1. **Confirmed:** step onto the blue `NET REFILL` pad and press **R**; refill restored the nets.
2. **Confirmed:** the thief stole scrap, dropped it by the far fence, and returned to the yard.
3. **Confirmed:** cart delivery banked two bags.
4. **Still verify:** net a loaded thief before it escapes; the recovered stolen scrap should be inside the resulting bag.
5. **Still verify:** the thief-dropped scrap remains available for pickup/netting and can be hauled to delivery.
6. **Still verify:** capacity stays at `2/2` and rejects a third bag, and the cart/player are blocked by every side of the perimeter fence.

## Remaining prototype work

- Tune cart mass, drag, and push force after hands-on testing.
- Tune thief warning time, speed, and escape distance after testing whether players have a fair chance to react.
- Later: add the first-zone loop around the working net, bag, cart, and banking mechanics.

## Controls

| Action | Control |
|---|---|
| Move | WASD |
| Aim/look | Mouse |
| Fire net | Left mouse button |
| Pick up / drop caught bag | E |
| Throw held bag | F |
| Push cart | Walk into it |
| Refill nets | R while on the blue refill pad |
