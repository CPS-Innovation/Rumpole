import { areAlphanumericallyEqual } from "./are-alphanumerically-equal";

describe("areAlphanumericallyEqual", () => {
  it("can match values and ignore punctuation", () => {
    expect(
      areAlphanumericallyEqual(
        "a.b,c/d#e!f$g%h^i&j*k;l:m{n}o=p-q_r`s~t(u)v…w”x1",
        "abcdefghijklmnopqrstuvwx1"
      )
    ).toBe(true);
  });

  it("can match values and recognise an accented character as different to non-accented", () => {
    expect(areAlphanumericallyEqual("abc", "åbc")).toBe(false);
  });
});
