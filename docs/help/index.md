---
title: Help
description: How to use Meaty Times — meat selection, temperatures, timing, and frequently asked questions.
permalink: /help/
---

This guide explains how to use the Meaty Times roast calculator and serve-at planner.

## Getting started

1. Open the [Meaty Times](https://webfrontend.yellowcoast-79687552.ukwest.azurecontainerapps.io/) web app.
2. On the main **Roast details** form, choose your **meat type** and enter the **weight in kilograms**.
3. If **Doneness** appears, select how you would like the joint cooked.
4. Tap **Calculate** to see roasting instructions.
5. Optionally, use **Serve at** below the results to plan clock times around a target serving time.

No account or sign-in is required.

## Selecting meat types

Meaty Times supports five common UK home-roast joints:

| Meat | Doneness selector | Typical weight range (kg) |
|------|-------------------|---------------------------|
| Beef | Yes — Rare, Medium, Well done | 0.5 – 15 |
| Lamb | Yes — Rare, Medium, Well done | 0.5 – 10 |
| Pork | No — cooked through | 0.5 – 12 |
| Chicken | No — cooked through | 0.8 – 8 |
| Gammon | No — cooked through | 0.5 – 10 |

Weight must fall within the allowed range for the selected meat. If your joint is outside the range, the app shows a validation message — try the nearest supported weight or split very large joints.

**Why is doneness hidden for some meats?** Poultry, pork, and gammon must be cooked through for food safety. Meaty Times does not offer underdone options for those meats.

## Cooking temperature guidance

Results are shown as numbered steps. Depending on the meat, you may see:

- **Single-temperature roast** — one oven setting for the full cooking time (for example pork, chicken, and gammon at 180°C).
- **Two-phase roast** — start at a higher temperature, then reduce the oven for the remaining time (beef and lamb).

Each step states the oven temperature in **degrees Celsius** and how long that phase lasts.

### Food safety

- Always use a meat thermometer to confirm safe internal temperatures, especially for poultry and pork.
- Meaty Times provides home-cooking guidance based on published sources; it cannot account for every oven, joint shape, or starting temperature.
- When in doubt, cook a little longer and verify with a thermometer.

## Timing calculations

Timings are calculated from the weight you enter:

- **Per-kilogram rules** — each meat type uses a minutes-per-kg rate appropriate to that joint.
- **Doneness adjustments** — for beef and lamb, Rare cooks for less time than Well done; Medium is the baseline.
- **Resting time** — shown separately and included in **Total preparation time** (cooking plus resting).
- **Limits** — very short or very long calculated times are capped to sensible minimums and maximums for home roasting.

### Reading the results

After you calculate, you will see:

| Field | Meaning |
|-------|---------|
| **Roasting instructions** | Ordered steps with temperature and duration for each phase |
| **Total cooking** | Time the joint spends in the oven |
| **Resting time** | Time to rest after removing from the oven before carving |
| **Total preparation** | Cooking plus resting — useful for planning the whole process |

## Serve at planner

Once you have results, the **Serve at** section lets you work backwards from when you want to eat:

1. Choose a **date** and **time** for serving.
2. Tap **Plan**.
3. Review the schedule milestones:
   - **Start cooking**
   - **Reduce temperature** (only when a two-phase roast applies)
   - **Remove from oven**
   - **Resting begins**

If the serving time is too soon — for example, you would need to have started already — the app warns you and may show the **earliest possible** serving time instead.

## Frequently asked questions

### Do I need an account?

No. Meaty Times does not require registration or login.

### Is my data saved?

The calculator does not store your roast details or personal information. Each calculation is handled for that session only.

### What units does Meaty Times use?

The current version uses **kilograms** for weight and **degrees Celsius** for oven temperatures. Imperial units may be added in a future release.

### Can I use Meaty Times offline?

The app is a web application and may need a network connection depending on how it is hosted. This documentation site is fully static and works offline once loaded.

### Are the times guaranteed to be perfect?

No calculator can guarantee exact results for every oven and joint. Treat the output as reliable starting guidance, verify with a thermometer, and adjust based on experience.

### Why does my serve-at time show a warning?

The total preparation time may be longer than the time remaining until your chosen serving time. Pick a later serving time or start cooking earlier.

### Where do the cooking rules come from?

Rules are based on publicly available home-cooking guidance (for example BBC Good Food and NHS food-safety recommendations). Sources are documented in the open-source project for contributors who wish to review or improve them.

### Something looks wrong — what should I do?

Please [open an issue on GitHub](https://github.com/markheydon/meaty-times/issues) with the meat type, weight, doneness (if used), and what you expected versus what you saw. See [Support]({{ '/support/' | relative_url }}) for more options.
