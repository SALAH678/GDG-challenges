var findDifferentBinaryString = function(nums) {
    let length = nums.length;
    let casNumber = 2 ** length;
    let allcases = [];

    for (let i = 0; i < casNumber; i++) {
        allcases.push(i.toString(2).padStart(length, '0'));
    }

    let set1 = new Set(nums);
    let set2 = new Set(allcases);

    const result = [...set2].filter(num => !set1.has(num));

    const randomElement = result[
        Math.floor(Math.random() * result.length)
    ];

    return randomElement;
};