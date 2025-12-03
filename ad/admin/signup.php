<?php
session_start();
error_reporting(E_ALL);
ini_set('display_errors', 1);

$host = "srv613.hstgr.io";
$dbname = "u412048963_survey_db";
$user = "u412048963_hephaestus";
$pass = "Hepastu5!";

try {
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8mb4", $user, $pass, [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION
    ]);
} catch (Exception $e) {
    die("DB Connection Error: " . $e->getMessage());
}

$error = "";
$success = "";

if ($_SERVER["REQUEST_METHOD"] === "POST") {
    $username = trim($_POST["username"]);
    $password = trim($_POST["password"]);
    $confirm_password = trim($_POST["confirm_password"]);

    if ($password !== $confirm_password) {
        $error = "Passwords do not match.";
    } else {
        // Check if username already exists
        $stmt = $pdo->prepare("SELECT * FROM admin_accounts WHERE username = ?");
        $stmt->execute([$username]);
        if ($stmt->fetch()) {
            $error = "Username already exists.";
        } else {
            $password_hash = password_hash($password, PASSWORD_DEFAULT);
            $stmt = $pdo->prepare("INSERT INTO admin_accounts (username, password_hash) VALUES (?, ?)");
            if ($stmt->execute([$username, $password_hash])) {
                $success = "Account created successfully! <a href='login.php'>Login here</a>.";
            } else {
                $error = "Failed to create account.";
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
    <style>
        .signup-box { width: 350px; margin: 120px auto; background: var(--card); padding: 24px; border-radius: 12px; box-shadow: 0 6px 18px rgba(12,24,40,0.08); text-align: center; }
        input { width: 100%; padding: 12px; border-radius: 8px; margin-top: 10px; border: 1px solid #dce3ef; }
        .button-container { display: flex; justify-content: space-between; gap: 10px; margin-top: 14px; }
        .signup-btn, .back-btn { flex: 1; padding: 12px; border-radius: 8px; font-weight: 600; border: none; cursor: pointer; text-decoration: none; color: white; }
        .signup-btn { background: var(--accent); }
        .back-btn { background: #6b7280; display: inline-block; text-align: center; }
        .error { padding:10px; background:rgba(194,59,59,0.1); color:var(--danger); text-align:center; margin-bottom:10px; border-radius:6px; border:1px solid rgba(194,59,59,0.15); }
        .success { padding:10px; background:rgba(59,194,59,0.1); color:green; text-align:center; margin-bottom:10px; border-radius:6px; border:1px solid rgba(59,194,59,0.15); }
        h2 { margin-bottom: 15px; }
    </style>
</head>
<body>
<div class="signup-box">
    <h2>Admin Signup</h2>

    <?php if ($error): ?>
        <div class="error"><?= htmlspecialchars($error) ?></div>
    <?php endif; ?>
    <?php if ($success): ?>
        <div class="success"><?= $success ?></div>
    <?php endif; ?>

    <form method="POST">
        <input type="text" name="username" placeholder="Username" required />
        <input type="password" name="password" placeholder="Password" required />
        <input type="password" name="confirm_password" placeholder="Confirm Password" required />
        <div class="button-container">
            <button class="signup-btn" type="submit">Sign Up</button>
            <a class="back-btn" href="login.php">Back to Login</a>
        </div>
    </form>
</div>
</body>
</html>
