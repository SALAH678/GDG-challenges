var findDifferentBinaryString = function (nums) {
  let length = nums.length;
  let casNumber = 2 ** length;
  let allcases = [];

  for (let i = 0; i < casNumber; i++) {
    allcases.push(i.toString(2).padStart(length, "0"));
  }

  let set1 = new Set(nums);
  let set2 = new Set(allcases);

  const result = [...set2].filter((num) => !set1.has(num));

  const randomElement = result[Math.floor(Math.random() * result.length)];

  return randomElement;
};

const tests = [
  {
    nums: ["01", "10"],
    expectedLength: 2,
  },
  {
    nums: ["00", "01"],
    expectedLength: 2,
  },
  {
    nums: ["111", "011", "001"],
    expectedLength: 3,
  },
  {
    nums: ["000", "001", "010"],
    expectedLength: 3,
  },
];

tests.forEach(({ nums, expectedLength }, index) => {
  const result = findDifferentBinaryString(nums);

  const isValid = result !== undefined && result.length === expectedLength && !nums.includes(result);

  console.log(
    `Test ${index + 1}:`,
    isValid ? "PASS" : "FAIL",
    `| Input: ${JSON.stringify(nums)}`,
    `| Output: ${result}`,
  );
});
