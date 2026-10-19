var firstMissingPositive = function (nums) {
  const numbers = new Set(nums);

  let smallestPositiveNumber = 1;

  while (numbers.has(smallestPositiveNumber)) {
    smallestPositiveNumber++;
  }

  return smallestPositiveNumber;
};

const tests = [
  { nums: [1, 2, 0], expected: 3 },
  { nums: [3, 4, -1, 1], expected: 2 },
  { nums: [7, 8, 9, 11, 12], expected: 1 },
  { nums: [1, 2, 3, 4, 5], expected: 6 },
  { nums: [2], expected: 1 },
  { nums: [1], expected: 2 },
  { nums: [], expected: 1 },
  { nums: [1, 1, 2, 2], expected: 3 },
];

tests.forEach(({ nums, expected }) => {
  const result = firstMissingPositive(nums);

  console.log(`Input: [${nums}] | Expected: ${expected} | Got: ${result} | ${result === expected ? "PASS" : "FAIL"}`);
});
