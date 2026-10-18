const displayEl = document.getElementById("display");
const expressionEl = document.getElementById("expression");

const SYMBOLS = { "+": "+", "-": "−", "*": "×", "/": "÷" };
const MAX_DIGITS = 15;

let current = "0";      // number being typed / shown
let previous = null;    // stored left-hand number
let operator = null;    // pending operator
let startNew = true;    // next digit starts a fresh number
let hasError = false;

function format(value) {
  // Round away floating-point noise (0.1 + 0.2 → 0.3)
  return String(Number(Number(value).toPrecision(12)));
}

function render() {
  displayEl.textContent = current;
  displayEl.classList.toggle("error", hasError);
  displayEl.classList.toggle("small", !hasError && current.length > 10);
  displayEl.classList.toggle("tiny", !hasError && current.length > 15);
}

function setExpression(text) {
  expressionEl.textContent = text;
}

function clearAll() {
  current = "0";
  previous = null;
  operator = null;
  startNew = true;
  hasError = false;
  setExpression("");
  render();
}

function showError(message) {
  current = message;
  previous = null;
  operator = null;
  startNew = true;
  hasError = true;
  render();
}

function inputDigit(d) {
  if (hasError) clearAll();
  if (startNew) {
    current = d;
    startNew = false;
  } else if (current.replace(/[-.]/g, "").length < MAX_DIGITS) {
    current = current === "0" ? d : current + d;
  }
  render();
}

function inputDot() {
  if (hasError) clearAll();
  if (startNew) {
    current = "0.";
    startNew = false;
  } else if (!current.includes(".")) {
    current += ".";
  }
  render();
}

function backspace() {
  if (hasError) return clearAll();
  if (startNew) return;
  current = current.slice(0, -1);
  if (current === "" || current === "-") {
    current = "0";
    startNew = true;
  }
  render();
}

function compute(a, b, op) {
  switch (op) {
    case "+": return a + b;
    case "-": return a - b;
    case "*": return a * b;
    case "/": return b === 0 ? null : a / b;
  }
}

function chooseOperator(op) {
  if (hasError) return;

  // Pressing two operators in a row just swaps the operator
  if (operator && startNew) {
    operator = op;
    setExpression(`${format(previous)} ${SYMBOLS[op]}`);
    return;
  }

  // Chain: 2 + 3 + … evaluates 2 + 3 first
  if (operator) {
    const result = compute(previous, Number(current), operator);
    if (result === null) return showError("Can't divide by 0");
    previous = Number(format(result));
    current = String(previous);
    render();
  } else {
    previous = Number(current);
  }

  operator = op;
  startNew = true;
  setExpression(`${format(previous)} ${SYMBOLS[op]}`);
}

function equals() {
  if (hasError || !operator) return;

  const right = Number(current);
  const result = compute(previous, right, operator);
  const text = `${format(previous)} ${SYMBOLS[operator]} ${format(right)} =`;

  if (result === null) {
    setExpression(text);
    return showError("Can't divide by 0");
  }

  setExpression(text);
  current = format(result);
  previous = null;
  operator = null;
  startNew = true;
  render();
}

/* ---------- Mouse / touch ---------- */
document.querySelector(".keys").addEventListener("click", (e) => {
  const btn = e.target.closest("button");
  if (!btn) return;

  switch (btn.dataset.action) {
    case "digit":     inputDigit(btn.dataset.value); break;
    case "dot":       inputDot(); break;
    case "operator":  chooseOperator(btn.dataset.value); break;
    case "equals":    equals(); break;
    case "backspace": backspace(); break;
    case "clear":     clearAll(); break;
  }
});

/* ---------- Keyboard ---------- */
function normalizeKey(key) {
  if (key === "Enter" || key === "=") return "=";
  if (key === "Backspace") return "backspace";
  if (key === "Escape" || key === "Delete" || key === "c" || key === "C") return "clear";
  if (key === "," ) return ".";
  if (key === "x" || key === "X") return "*";
  return key;
}

document.addEventListener("keydown", (e) => {
  if (e.ctrlKey || e.metaKey || e.altKey) return; // keep shortcuts like Ctrl+C working

  const key = normalizeKey(e.key);
  const btn = document.querySelector(`[data-key="${CSS.escape(key)}"]`);
  if (!btn) return;

  e.preventDefault(); // stops "/" quick-find, Enter re-clicking a focused button, etc.
  btn.click();

  btn.classList.add("pressed");
  setTimeout(() => btn.classList.remove("pressed"), 110);
});

render();