<?php
session_start();
error_reporting(E_ALL);
ini_set('display_errors', 1);

// Database config
$host = "srv613.hstgr.io";
$dbname = "u412048963_survey_db";
$user = "u412048963_hephaestus";
$pass = "Hepastu5!";

try {
    $pdo = new PDO(
        "mysql:host=$host;dbname=$dbname;charset=utf8mb4",
        $user,
        $pass,
        [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]
    );
} catch (Exception $e) {
    die("DB Error: " . $e->getMessage());
}

$error = "";
$success = "";

if ($_SERVER["REQUEST_METHOD"] === "POST") {
    $username = trim($_POST["username"]);
    $full_name = trim($_POST["full_name"]);
    $password = trim($_POST["password"]);
    $confirm_password = trim($_POST["confirm_password"]);

    if (empty($username) || empty($password) || empty($confirm_password)) {
        $error = "Please fill in all required fields.";
    } elseif ($password !== $confirm_password) {
        $error = "Passwords do not match.";
    } else {
        $stmt = $pdo->prepare("SELECT * FROM admin_accounts WHERE username = ?");
        $stmt->execute([$username]);
        if ($stmt->fetch()) {
            $error = "Username already exists.";
        } else {
            $password_hash = password_hash($password, PASSWORD_DEFAULT);
            $stmt = $pdo->prepare("INSERT INTO admin_accounts (username, password_hash, full_name) VALUES (?, ?, ?)");
            if ($stmt->execute([$username, $password_hash, $full_name])) {
                $success = "Account created successfully! You can now <a href='login.php'>login</a>.";
            } else {
                $error = "Failed to create account. Please try again.";
            }
        }
    }
}
?>

<!DOCTYPE html>
<html>
<head>
    <title>Admin Signup</title>
    <link rel="stylesheet" href="admin.css">
    <!-- Font Awesome CDN for eye/eye-slash -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css"/>
    <style>
        .signup-box {
            width: 400px;
            margin: 100px auto;
            background: var(--card);
            padding: 24px;
            border-radius: 12px;
            box-shadow: 0 6px 18px rgba(12,24,40,0.08);
        }
        input {
            width: 100%;
            padding: 12px;
            border-radius: 8px;
            margin-top: 10px;
            border: 1px solid #dce3ef;
        }
        .signup-btn {
            background: var(--accent);
            color: #fff;
            font-weight: 700;
            border: none;
            padding: 12px;
            width: 100%;
            border-radius: 8px;
            margin-top: 14px;
            cursor: pointer;
        }
        .back-btn {
            display:block;
            margin-top:10px;
            padding:12px;
            width:100%;
            background:#6b7280;
            border-radius:8px;
            color:white;
            text-align:center;
            font-weight:600;
            text-decoration:none;
        }
        .error, .success {
            padding:10px;
            text-align:center;
            margin-bottom:10px;
            border-radius:6px;
        }
        .error {
            background: rgba(194,59,59,0.1);
            color: var(--danger);
            border:1px solid rgba(194,59,59,0.15);
        }
        .success {
            background: rgba(72,187,120,0.1);
            color: var(--success);
            border:1px solid rgba(72,187,120,0.3);
        }
        h2 { text-align:center; margin-bottom:10px; }

        .password-wrapper {
            position: relative;
        }
        .toggle-pass {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            user-select: none;
            color: var(--muted);
            font-size: 18px;
        }
    </style>
</head>
<body>

<div class="signup-box">
    <h2>Create Admin Account</h2>

    <?php if ($error): ?>
        <div class="error"><?= htmlspecialchars($error) ?></div>
    <?php endif; ?>

    <?php if ($success): ?>
        <div class="success"><?= $success ?></div>
    <?php endif; ?>

    <form method="POST">
        <input type="text" name="full_name" placeholder="Full Name" />
        <input type="text" name="username" placeholder="Username" required />

        <div class="password-wrapper">
            <input type="password" name="password" id="password" placeholder="Password" required />
            <span class="toggle-pass"><i class="fa-solid fa-eye" id="eye-password"></i></span>
        </div>

        <div class="password-wrapper">
            <input type="password" name="confirm_password" id="confirm_password" placeholder="Confirm Password" required />
            <span class="toggle-pass"><i class="fa-solid fa-eye" id="eye-confirm"></i></span>
        </div>

        <button class="signup-btn">Sign Up</button>
    </form>

    <a class="back-btn" href="login.php">Back to Login</a>
</div>

<script>
const togglePassword = document.getElementById('eye-password');
const passwordInput = document.getElementById('password');

togglePassword.parentElement.addEventListener('click', () => {
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        togglePassword.classList.remove('fa-eye');
        togglePassword.classList.add('fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        togglePassword.classList.remove('fa-eye-slash');
        togglePassword.classList.add('fa-eye');
    }
});

const toggleConfirm = document.getElementById('eye-confirm');
const confirmInput = document.getElementById('confirm_password');

toggleConfirm.parentElement.addEventListener('click', () => {
    if (confirmInput.type === 'password') {
        confirmInput.type = 'text';
        toggleConfirm.classList.remove('fa-eye');
        toggleConfirm.classList.add('fa-eye-slash');
    } else {
        confirmInput.type = 'password';
        toggleConfirm.classList.remove('fa-eye-slash');
        toggleConfirm.classList.add('fa-eye');
    }
});
</script>

</body>
</html>
