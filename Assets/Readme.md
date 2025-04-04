# 🏏 Cricket Ball Simulation Game

A physics-based cricket ball simulation game made in Unity! Every system is driven by real-world physics principles, giving you that a realistic bowling experience.

---

## 🎮 Gameplay Overview

1. **Choose your drop spot** — You start by selecting where you want the ball to land on the pitch.
2. **Let physics do the work** — The game calculates the ideal angle and force needed to hit the target.
3. **Pick your bowling style**:
   - 🌀 **Swing Ball**: Swings in the air, drops precisely on the target spot, then moves tangentially.
   - 🔁 **Spin Ball**: Drops straight, then turns sharply left or right (you choose which).

4. **Timing matters** — A slider moves up and down:
   - 🎯 **Blue zone**: Fastest swing / highest spin angle.
   - 🎯 **Green zone**: Medium swing / medium spin angle.
   - 🎯 **Yellow zone**: Slow swing / low spin angle.
   - 🔴 **Red zone**: Slowest swing / lowest spin angle / NO BALL.

> 💡 The faster the swing, the more the ball swings — just like real-life cricket!

---

## ⚙️ Ball Parameters

All tunable parameters can be found in the `Ball Parameters` component attached to the **`Ball`** GameObject.

### You can tweak:
- `Swing Ball Speeds` (4 options for different speeds)
- `Spin Ball Speed`
- `Swing Force`
- `Spin Force`

🛠 Each parameter has tooltips — just hover over them in the Unity Inspector to learn what they do!

---

## 📦 Built With
- Unity 6
- C#

---
