import "dotenv/config";
import express from "express";
import bcrypt from "bcryptjs";
import jwt from "jsonwebtoken";
import { randomUUID } from "node:crypto";

const { JWT_SECRET, JWT_EXPIRES_IN = "15m" } = process.env;

if (!JWT_SECRET) {
  throw new Error("JWT_SECRET must be configured in .env");
}

const app = express();
const PORT = 3000;

app.use(express.json());

// Initial users data
const users = [
  {
    id: "b3c1f0e2-6a41-4d6b-9a52-1f0c8d7e2a01",
    username: "john_doe",
    passwordHash: "$2b$12$r.zGhhftkEFFt7VJHccMTeCQXXphuM.QGPmlI1NFtklNvVPnCTy.W",
    role: "Customer",
  },
  {
    id: "7d9e4a18-2c3b-4f5e-8b6a-9c1d0e3f4a02",
    username: "jane_smith",
    passwordHash: "$2b$12$6eCYB.YwGN1Rcnc/KRRnzOTTSo7HLMHtJGlhEa8Aqov6VSsTF6aTC",
    role: "Customer",
  },
  {
    id: "e5a2c7b9-1d84-4c3f-a6e0-5b8f2d9c1a03",
    username: "admin",
    passwordHash: "$2b$12$eQaT5oGCbCEev9rpwBtkze3R2a2cB1AUHKkeNU5OcGrBYnxwaD0xC",
    role: "Admin",
  },
];

const BCRYPT_ROUNDS = 12;
const USERNAME_RE = /^[a-zA-Z0-9_]{3,30}$/;

// Used when a username does not exist
const DUMMY_HASH = await bcrypt.hash("dummy-password", BCRYPT_ROUNDS);

const toUserDto = (user) => ({
  id: user.id,
  username: user.username,
  role: user.role,
});

function createAccessToken(user) {
  return jwt.sign(
    {
      username: user.username,
      role: user.role,
    },
    JWT_SECRET,
    {
      algorithm: "HS256",
      subject: user.id,
      expiresIn: JWT_EXPIRES_IN,
    },
  );
}

const auth = express.Router();

// POST /api/auth/signup
auth.post("/signup", async (req, res, next) => {
  try {
    const { username, password } = req.body ?? {};

    if (typeof username !== "string" || !USERNAME_RE.test(username)) {
      return res.status(400).json({
        error: "Username must be 3-30 characters: letters, numbers or underscores.",
      });
    }

    if (typeof password !== "string" || password.length < 8 || Buffer.byteLength(password, "utf8") > 72) {
      return res.status(400).json({
        error: "Password must be at least 8 characters and at most 72 bytes.",
      });
    }

    const key = username.toLowerCase();

    const userExists = users.some((user) => user.username.toLowerCase() === key);

    if (userExists) {
      return res.status(409).json({
        error: "Username is already taken.",
      });
    }

    const passwordHash = await bcrypt.hash(password, BCRYPT_ROUNDS);

    const user = {
      id: randomUUID(),
      username,
      passwordHash,
      role: "Customer",
    };

    users.push(user);

    return res.status(201).json({
      user: toUserDto(user),
    });
  } catch (err) {
    next(err);
  }
});

// POST /api/auth/login
auth.post("/login", async (req, res, next) => {
  try {
    const { username, password } = req.body ?? {};

    if (typeof username !== "string" || typeof password !== "string") {
      return res.status(400).json({
        error: "Username and password are required.",
      });
    }

    const key = username.toLowerCase();

    const user = users.find((user) => user.username.toLowerCase() === key);

    const passwordMatches = await bcrypt.compare(password, user ? user.passwordHash : DUMMY_HASH);

    if (!user || !passwordMatches) {
      return res.status(401).json({
        error: "Invalid username or password.",
      });
    }

    return res.json({
      user: toUserDto(user),
      accessToken: createAccessToken(user),
    });
  } catch (err) {
    next(err);
  }
});

app.use("/api/auth", auth);

app.use((err, _req, res, _next) => {
  if (err.type === "entity.parse.failed") {
    return res.status(400).json({
      error: "Invalid JSON body.",
    });
  }

  console.error(err);

  return res.status(500).json({
    error: "Something went wrong.",
  });
});

app.listen(PORT, () => {
  console.log(`Listening on http://localhost:${PORT}`);
});
