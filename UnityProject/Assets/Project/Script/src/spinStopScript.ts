const spinStopLastSent = new Map<string, number>();
let spinStopTime = 0;
let spinStopCooldown = 0;

$.onUpdate((deltaTime) => {
  spinStopTime += deltaTime;
  spinStopCooldown = Math.max(0, spinStopCooldown - deltaTime);
  const currentPlayerIds = new Set<string>();
  let nextPlayer: PlayerHandle | null = null;
  let oldestSent = Infinity;

  for (const overlap of $.getOverlaps()) {
    const player = overlap.handle;
    if (player?.type !== "player" || currentPlayerIds.has(player.id)) {
      continue;
    }

    currentPlayerIds.add(player.id);
    const lastSent = spinStopLastSent.get(player.id);
    if (lastSent === undefined || spinStopTime - lastSent >= 0.25) {
      const priority = lastSent ?? -Infinity;
      if (priority < oldestSent) {
        oldestSent = priority;
        nextPlayer = player;
      }
    }
  }

  for (const playerId of spinStopLastSent.keys()) {
    if (!currentPlayerIds.has(playerId)) {
      spinStopLastSent.delete(playerId);
    }
  }

  if (nextPlayer && spinStopCooldown === 0) {
    nextPlayer.send("spin-stop", null);
    spinStopLastSent.set(nextPlayer.id, spinStopTime);
    spinStopCooldown = 0.125;
  }
});
