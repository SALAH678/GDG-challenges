import { countries } from "./data.js";

const searchInput = document.getElementById("search");
const suggestionBox = document.createElement("div");

suggestionBox.classList.add("suggestion-box");
searchInput.insertAdjacentElement("afterend", suggestionBox);

function autocomplete(input) {
  const query = input.trim().toLowerCase();

  if (!query) return [];

  return countries.filter((country) => country.name.toLowerCase().startsWith(query));
}

function displaySuggestions(suggestions) {
  suggestionBox.replaceChildren();

  suggestions.forEach((country) => {
    const item = document.createElement("div");
    item.classList.add("suggestion-item");
    item.textContent = country.name;

    item.addEventListener("click", () => {
      searchInput.value = country.name;
      suggestionBox.replaceChildren();
    });

    suggestionBox.appendChild(item);
  });

  suggestionBox.hidden = suggestions.length === 0;
}

searchInput.addEventListener("input", (event) => {
  const suggestions = autocomplete(event.target.value);
  displaySuggestions(suggestions);
});

document.addEventListener("click", (event) => {
  if (event.target !== searchInput && !suggestionBox.contains(event.target)) {
    suggestionBox.hidden = true;
  }
});
