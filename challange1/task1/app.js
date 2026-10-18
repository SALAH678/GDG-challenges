const express = require("express");

const app = express();
const PORT = 3000;

//data
let reportedItems = [
  {
    id: 1,
    type: "Lost",
    name: "Black Wallet",
    description: "A black leather wallet containing an ID card.",
    location: "University Library",
    date: "2026-10-15",
    isRecoverd: false,
  },
  {
    id: 2,
    type: "Found",
    name: "Blue Backpack",
    description: "A blue backpack containing notebooks and pens.",
    location: "Main Cafeteria",
    date: "2026-10-16",
    isRecoverd: false,
  },
  {
    id: 3,
    type: "Lost",
    name: "Wireless Earbuds",
    description: "A pair of white wireless earbuds in a charging case.",
    location: "Room B12",
    date: "2026-10-10",
    isRecoverd: true,
  },
  {
    id: 4,
    type: "Found",
    name: "Student ID Card",
    description: "A student ID card found near the main entrance.",
    location: "Main Entrance",
    date: "2026-10-17",
    isRecoverd: true,
  },
  {
    id: 5,
    type: "Lost",
    name: "Silver Watch",
    description: "A silver wristwatch with a black strap.",
    location: "Sports Hall",
    date: "2026-10-14",
    isRecoverd: false,
  },
];

//middleware
app.use(express.json());

//report lost item
app.post("/report/lost-item", (req, res) => {
  const newReportedLostItem = {
    id: reportedItems.length ? Math.max(...reportedItems.map((i) => i.id)) + 1 : 1,
    type: "Lost",
    name: req.body.name,
    description: req.body.description,
    location: req.body.location,
    date: req.body.dateLost,
    isRecoverd: false,
  };

  reportedItems.push(newReportedLostItem);
  res.status(201).json(newReportedLostItem);
});

//report found item
app.post("/report/found-item", (req, res) => {
  const newReportedFoundItem = {
    id: reportedItems.length ? Math.max(...reportedItems.map((i) => i.id)) + 1 : 1,
    type: "Found",
    name: req.body.name,
    description: req.body.description,
    location: req.body.location,
    date: req.body.dateFound,
    isRecoverd: false,
  };

  reportedItems.push(newReportedFoundItem);
  res.status(201).json(newReportedFoundItem);
});

//search and filter reported items
app.get("/reported-items", (req, res) => {
  const { type, name, location, date, isRecoverd } = req.query;

  let filteredItems = [...reportedItems];

  if (type) {
    if (!["lost", "found"].includes(type.toLowerCase())) {
      return res.status(400).json({
        error: "Type must be lost or found",
      });
    }
    filteredItems = filteredItems.filter((item) => item.type.toLowerCase() === type.toLowerCase());
  }

  if (date) {
    filteredItems = filteredItems.filter((item) => item.date === date);
  }

  if (isRecoverd !== undefined) {
    filteredItems = filteredItems.filter((item) => item.isRecoverd === (isRecoverd === "true"));
  }

  if (name) {
    filteredItems = filteredItems.filter((item) => item.name?.toLowerCase().includes(name.toLowerCase()));
  }

  if (location) {
    filteredItems = filteredItems.filter((item) => item.location?.toLowerCase().includes(location.toLowerCase()));
  }

  res.json(filteredItems);
});

// view the details of an item
app.get("/reported-items/:id", (req, res) => {
  const id = Number(req.params.id);
  const item = reportedItems.find((item) => item.id === id);

  if (item) {
    res.json(item);
  } else {
    res.status(404).json({ error: "Item not found" });
  }
});

// update the details of an item
app.put("/reported-items/:id", (req, res) => {
  const id = Number(req.params.id);
  const item = reportedItems.find((item) => item.id === id);

  if (item) {
    Object.assign(item, req.body);
    res.json(item);
  } else {
    res.status(404).json({ error: "Item not found" });
  }
});

// make an item as recovered
app.put("/reported-items/:id/recover", (req, res) => {
  const id = Number(req.params.id);
  const item = reportedItems.find((item) => item.id === id);

  if (!item) {
    return res.status(404).json({ error: "Item not found" });
  }

  item.isRecoverd = true;
  res.json(item);
});

app.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}`);
});
