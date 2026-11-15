const horizontalMoveSpeed = 1;
const verticalMoveSpeed = 1;
const centerMoveSpeed = 1;
const spinSpeed = 360;
const defaultGravity = -9.81;
let moveDirection: Vector3 | null = null;
let pathCenter: Vector3 | null = null;
let targetPosition: Vector3 | null = null;
let elapsed = 0;
let waitingElapsed = 0;
let isStopped = false;

function restoreSpinControls() {
  _.setMoveSpeedRate(1);
  _.setJumpSpeedRate(1);
  _.setGravity(defaultGravity);
}

function alignSpinPosition(position: Vector3, center: Vector3, direction: Vector3): Vector3 {
  const horizontalLengthSq = direction.x ** 2 + direction.z ** 2;
  // Align horizontally to the start's direction without changing foot height.
  const along = horizontalLengthSq > 1e-8
    ? ((position.x - center.x) * direction.x +
       (position.z - center.z) * direction.z) / horizontalLengthSq
    : 0;
  return new Vector3(center.x + direction.x * along, position.y, center.z + direction.z * along);
}

restoreSpinControls();

_.onReceive(
  (messageType, arg, sender) => {
    if (messageType === "spin-start") {
      if (isStopped || moveDirection || sender.type !== "item" ||
          sender.id !== _.sourceItemId.id) {
        return;
      }

      if (!arg || typeof arg !== "object" || !("direction" in arg) ||
          !("center" in arg) || !(arg.direction instanceof Vector3) ||
          !(arg.center instanceof Vector3) || arg.direction.lengthSq() === 0 ||
          ![arg.direction.x, arg.direction.y, arg.direction.z,
            arg.center.x, arg.center.y, arg.center.z].every(Number.isFinite)) {
        _.log("Spin movement could not start because its configuration is invalid.");
        return;
      }

      const position = _.getPosition();
      if (!position) {
        _.log("Spin movement could not start because the player position is unavailable.");
        return;
      }

      moveDirection = new Vector3(arg.direction.x, arg.direction.y, arg.direction.z).normalize();
      pathCenter = new Vector3(arg.center.x, arg.center.y, arg.center.z);
      targetPosition = alignSpinPosition(position, pathCenter, moveDirection);
      _.setMoveSpeedRate(0);
      _.setJumpSpeedRate(0);
      _.setGravity(0);
      return;
    }

    if (messageType !== "spin-stop") {
      return;
    }

    isStopped = true;
    restoreSpinControls();
  },
  { player: false, item: true },
);

_.onFrame((deltaTime) => {
  if (isStopped) {
    return;
  }

  if (!moveDirection || !pathCenter || !targetPosition) {
    waitingElapsed += deltaTime;
    if (waitingElapsed >= 5) {
      isStopped = true;
      _.log("Spin movement could not start because its configuration was not received.");
    }
    return;
  }

  const position = _.getPosition();
  if (!position) {
    isStopped = true;
    restoreSpinControls();
    _.log("Spin movement stopped because the player position is unavailable.");
    return;
  }

  const aligned = alignSpinPosition(position, pathCenter, moveDirection);
  const offsetX = aligned.x - position.x;
  const offsetZ = aligned.z - position.z;
  const distance = Math.sqrt(offsetX ** 2 + offsetZ ** 2);
  const centerProgress = distance > 0
    ? Math.min(1, centerMoveSpeed * deltaTime / distance)
    : 0;
  // Keep the commanded vertical path independent of position drift during a jump.
  targetPosition = new Vector3(
    position.x + offsetX * centerProgress + moveDirection.x * horizontalMoveSpeed * deltaTime,
    moveDirection.y === 0
      ? position.y
      : targetPosition.y + moveDirection.y * verticalMoveSpeed * deltaTime,
    position.z + offsetZ * centerProgress + moveDirection.z * horizontalMoveSpeed * deltaTime,
  );
  _.setPosition(targetPosition);

  elapsed += deltaTime;
  _.setRotation(
    new Quaternion().setFromEulerAngles(new Vector3(0, elapsed * spinSpeed, 0)),
  );
});

_.sendTo(_.sourceItemId, "spin-ready", null);
