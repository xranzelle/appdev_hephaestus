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

if ($_SERVER["REQUEST_METHOD"] === "POST") {
    $username = trim($_POST["username"]);
    $password = trim($_POST["password"]);

    $stmt = $pdo->prepare("SELECT * FROM admin_accounts WHERE username = ?");
    $stmt->execute([$username]);
    $admin = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($admin && password_verify($password, $admin["password_hash"])) {
        $_SESSION["admin_logged_in"] = true;
        $_SESSION["admin_username"] = $admin["username"];
        header("Location: admin.html");
        exit;
    } else {
        $error = "Invalid username or password.";
    }
}
?>

<!DOCTYPE html>
<html>
<head>
    <title>Admin Login</title>
    <link rel="stylesheet" href="admin.css">
    <style>
        .login-box { width: 350px; margin: 120px auto; background: var(--card); padding: 24px; border-radius: 12px; box-shadow: 0 6px 18px rgba(12,24,40,0.08); }
        input { width: 100%; padding: 12px; border-radius: 8px; margin-top: 10px; border: 1px solid #dce3ef; }
        .login-btn { background: var(--accent); color: #fff; font-weight: 700; border: none; padding: 12px; width: 100%; border-radius: 8px; margin-top: 14px; cursor: pointer; }
        .signup-btn { display:block; margin-top:10px; padding:12px; width:100%; background:#6b7280; border-radius:8px; color:white; text-align:center; font-weight:600; text-decoration:none; }
        .error { padding:10px; background:rgba(194,59,59,0.1); color:var(--danger); text-align:center; margin-bottom:10px; border-radius:6px; border:1px solid rgba(194,59,59,0.15); }
        h2 { text-align:center; margin-bottom:10px; }
    </style>
</head>
<body>
<div class="login-box">
    <h2>Admin Login</h2>

    <?php if ($error): ?>
        <div class="error"><?= htmlspecialchars($error) ?></div>
    <?php endif; ?>

    <form method="POST">
        <input type="text" name="username" placeholder="Username" required />
        <input type="password" name="password" placeholder="Password" required />
        <button class="login-btn">Login</button>
    </form>

    <a class="signup-btn" href="signup.php">Create Admin Account</a>
</div>
</body>
</html>
