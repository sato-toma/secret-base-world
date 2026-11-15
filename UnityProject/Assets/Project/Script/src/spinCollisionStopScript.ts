$.onCollide((collision) => {
  if (collision.handle?.type === "player") {
    collision.handle.send("spin-stop", null);
  }
});
