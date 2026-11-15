let spinStartOverlappingPlayerIds = new Set<string>();

$.onUpdate(() => {
  const currentPlayerIds = new Set<string>();

  for (const overlap of $.getOverlaps()) {
    const player = overlap.handle;
    if (player?.type !== "player" || currentPlayerIds.has(player.id)) {
      continue;
    }

    currentPlayerIds.add(player.id);
    if (!spinStartOverlappingPlayerIds.has(player.id)) {
      $.setPlayerScript(player);
    }
  }

  spinStartOverlappingPlayerIds = currentPlayerIds;
});

$.onReceive(
  (messageType, _arg, sender) => {
    if (messageType !== "spin-ready" || sender.type !== "player") {
      return;
    }

    const direction = new Vector3(0, 0, 1).applyQuaternion($.getRotation());
    if (direction.lengthSq() === 0) {
      $.log("Spin movement could not start because the direction is zero.");
      return;
    }

    sender.send("spin-start", {
      direction: direction.normalize(),
      center: $.getPosition(),
    });
  },
  { player: true, item: false },
);
