const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const dist = path.join(__dirname, "..", "UnityProject", "Assets", "Project", "Script", "dist");
const startSource = fs.readFileSync(path.join(dist, "spinStartScript.js"), "utf8");
const playerSource = fs.readFileSync(path.join(dist, "spinPlayerScript.js"), "utf8");
const stopSource = fs.readFileSync(path.join(dist, "spinStopScript.js"), "utf8");

class Vector3 {
  constructor(x, y, z) {
    Object.assign(this, { x, y, z });
  }
  lengthSq() {
    return this.x ** 2 + this.y ** 2 + this.z ** 2;
  }
  normalize() {
    const length = Math.sqrt(this.lengthSq());
    this.x /= length;
    this.y /= length;
    this.z /= length;
    return this;
  }
  applyQuaternion(rotation) {
    Object.assign(this, rotation.direction);
    return this;
  }
}

class Quaternion {
  setFromEulerAngles() {
    return this;
  }
}

function setup(direction, initialPosition) {
  const sourceId = { type: "item", id: "source" };
  const effects = [];
  const logs = [];
  let position = initialPosition;
  let update;
  let receiveItem;
  let receivePlayer;
  let frame;
  let ready;
  let overlaps = [];
  const player = {
    type: "player",
    id: "player",
    send(type, arg) {
      receivePlayer(type, arg, sourceId);
    },
  };
  const itemApi = {
    onUpdate(callback) { update = callback; },
    getOverlaps() { return overlaps; },
    setPlayerScript(handle) { effects.push(["register", handle.id]); },
    onReceive(callback) { receiveItem = callback; },
    getRotation() { return { direction }; },
    getPosition() { return new Vector3(12, 0, 6); },
    log(message) { logs.push(message); },
  };
  const playerApi = {
    sourceItemId: sourceId,
    onReceive(callback) { receivePlayer = callback; },
    onFrame(callback) { frame = callback; },
    sendTo(id, type, arg) { ready = [id, type, arg]; },
    getPosition() { return position; },
    setPosition(value) { position = value; },
    setRotation() {},
    setMoveSpeedRate(value) { effects.push(["move", value]); },
    setJumpSpeedRate(value) { effects.push(["jump", value]); },
    setGravity(value) { effects.push(["gravity", value]); },
    log(message) { logs.push(message); },
  };
  vm.runInNewContext(startSource, { $: itemApi, Vector3 });
  vm.runInNewContext(playerSource, { _: playerApi, Vector3, Quaternion });
  const initialEffects = effects.splice(0);
  return {
    effects, initialEffects, logs, frame, receivePlayer, sourceId,
    position: () => position,
    setPosition: value => { position = value; },
    enter: () => { overlaps = [{ handle: player }]; update(); },
    respond: () => receiveItem(ready[1], ready[2], player),
  };
}

test("moves toward the center gradually while ascending", () => {
  const s = setup(new Vector3(0, 1, 0.00000006), new Vector3(11.3, -2.5, 6.4));
  s.enter();
  s.frame(0.1);
  assert.deepEqual(s.effects, [["register", "player"]]);
  s.respond();
  s.frame(0.1);
  assert.ok(Math.abs(Math.hypot(s.position().x - 11.3, s.position().z - 6.4) - 0.1) < 1e-6);
  assert.ok(Math.abs(s.position().y + 2.4) < 1e-6);
  s.frame(1);
  assert.ok(Math.abs(s.position().x - 12) < 1e-6);
  assert.ok(Math.abs(s.position().z - 6) < 1e-6);
  assert.ok(Math.abs(s.position().y + 1.4) < 1e-6);
});

test("downward spin follows its commanded path despite upward position drift", () => {
  const s = setup(new Vector3(0, -1, 0), new Vector3(12.4, -0.8, 6.3));
  s.respond();
  s.frame(0.1);
  s.setPosition(new Vector3(12, 0.2, 6));
  s.frame(0.1);
  assert.ok(Math.abs(s.position().y + 1) < 1e-6);
  assert.equal(s.position().x, 12);
  assert.equal(s.position().z, 6);
});

test("horizontal spin centers only the sideways axis and preserves height", () => {
  const s = setup(new Vector3(1, 0, 0), new Vector3(11, -3, 6.4));
  s.respond();
  s.frame(0.1);
  assert.ok(Math.abs(s.position().x - 11.1) < 1e-6);
  assert.ok(Math.abs(s.position().z - 6.3) < 1e-6);
  s.frame(0.5);
  assert.ok(Math.abs(s.position().x - 11.6) < 1e-6);
  assert.equal(s.position().y, -3);
  assert.equal(s.position().z, 6);
  s.setPosition(new Vector3(11, -3, 6));
  s.frame(0.5);
  assert.equal(s.position().x, 11.5);
});

test("ignores another source and cannot restart after stop", () => {
  const s = setup(new Vector3(0, 1, 0), new Vector3(12, -3, 6));
  s.receivePlayer("spin-start", {
    direction: new Vector3(0, 1, 0), center: new Vector3(12, 0, 6),
  }, { type: "item", id: "other" });
  s.frame(0.1);
  assert.deepEqual(s.effects, []);
  s.receivePlayer("spin-stop", null, s.sourceId);
  s.respond();
  s.frame(0.1);
  assert.deepEqual(s.effects, [["move", 1], ["jump", 1], ["gravity", -9.81]]);
});

test("reports a missing setup without disabling controls", () => {
  const s = setup(new Vector3(0, 1, 0), new Vector3(12, -3, 6));
  assert.deepEqual(s.initialEffects, [["move", 1], ["jump", 1], ["gravity", -9.81]]);
  s.frame(5);
  assert.deepEqual(s.effects, []);
  assert.match(s.logs[0], /configuration was not received/);
});

test("repeats stop for a player whose script starts while already overlapping", () => {
  const s = setup(new Vector3(0, -1, 0), new Vector3(12, -1, 6));
  let stopUpdate;
  const messages = [];
  let receiving = false;
  const player = {
    type: "player",
    id: "player",
    send(type, arg) {
      messages.push(type);
      if (receiving) s.receivePlayer(type, arg, { type: "item", id: "stop" });
    },
  };
  vm.runInNewContext(stopSource, {
    $: {
      onUpdate(callback) { stopUpdate = callback; },
      getOverlaps() { return [{ handle: player }, { handle: player }]; },
    },
  });
  stopUpdate(0);
  s.respond();
  receiving = true;
  stopUpdate(0.13);
  assert.equal(messages.length, 1);
  stopUpdate(0.13);
  assert.deepEqual(messages, ["spin-stop", "spin-stop"]);
  assert.deepEqual(s.effects.slice(-3), [["move", 1], ["jump", 1], ["gravity", -9.81]]);
  s.frame(0.1);
  assert.equal(s.position().y, -1);
});

test("throttles stop notifications across overlapping players", () => {
  let stopUpdate;
  const sent = [];
  const players = ["a", "b", "c"].map(id => ({
    type: "player", id, send() { sent.push(id); },
  }));
  vm.runInNewContext(stopSource, {
    $: {
      onUpdate(callback) { stopUpdate = callback; },
      getOverlaps() { return players.map(handle => ({ handle })); },
    },
  });
  stopUpdate(0);
  stopUpdate(0.05);
  assert.deepEqual(sent, ["a"]);
  stopUpdate(0.08);
  stopUpdate(0.13);
  assert.deepEqual(sent, ["a", "b", "c"]);
});
